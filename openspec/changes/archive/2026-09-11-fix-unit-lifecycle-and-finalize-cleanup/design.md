# 设计：单位生命周期销毁顺序修正 + 终态清理补全

对应提案：`proposal.md`（`fix-unit-lifecycle-and-finalize-cleanup`，`full`）

## 1. 现状与根因

### 1.1 注册顺序（已核实）
`War3Frame.Generator/SystemGenerator.cs:141-142`：
```csharp
systemInfos.OrderBy(t => t.Order).ThenBy(t => t.ClassName, StringComparer.Ordinal)
```
`Order` 缺省 = 1（`SystemGenerator.cs:62`）。三个系统均 `[SystemRegister(SystemKind.Immediate)]` 无 order →
按类名字典序：`UnitLifecycleDisposeSystem` → `UnitLifecycleTransitionSystem` → `UnitRemoveNativeSystem`。

### 1.2 阶段机现状
```
Alive ─(KillUnit helper)→ Death ─(Transition)→ Corpse ─(TimerTask 到期置 ClearCorpse)
      → ClearCorpse ─(Transition)→ Remove ─(Dispose)→ 删除
```
- `UnitLifecycleTransitionSystem`：`Death→Corpse`、`ClearCorpse→Remove`。
- `UnitRemoveNativeSystem`：`Death→KillUnit`、`Remove→RemoveUnit+HandleRemove`。
- `UnitLifecycleDisposeSystem`：`Remove→CleanupFinalizeEntityDispose`。

### 1.3 竞态分析
| 路径 | 现顺序后果 |
|---|---|
| Death | Transition 先转 Corpse，RemoveNative 看不到 Death → **KillUnit 不执行** |
| ClearCorpse | Transition 转 Remove；下一 tick RemoveNative 执行 RemoveUnit（**该路径侥幸正确**） |
| 直删 Remove | Dispose 先删实体 → RemoveNative 看不到 → **RemoveUnit/HandleRemove 不执行（泄漏）** |

## 2. 目标顺序

必须满足两条不变式：
- **I1**：`Death` 的 `KillUnit` 在 `Death→Corpse` 之前执行。
- **I2**：`RemoveUnit`+`HandleRemove` 在 ECS `DeleteEntity` 之前执行，且**恰好一次**。

## 3. 方案

### 3.1 推荐方案 A：显式 order + `Disposing` 确认阶段

新增阶段 `UnitLifecyclePhase.Disposing`（原生已移除，ECS 待销毁），显式 order：

| order | 系统 | 行为 |
|---|---|---|
| 10 | `UnitRemoveNativeSystem` | `Death→KillUnit`；`Remove→(有 UnitNative) RemoveUnit+HandleRemove → phase=Disposing`；无 UnitNative 也置 `Disposing` |
| 20 | `UnitLifecycleTransitionSystem` | `Death→Corpse`；`ClearCorpse→Remove` |
| 30 | `UnitLifecycleDisposeSystem` | 仅 `Disposing→CleanupFinalizeEntityDispose` |

时序验证：
- **Death tick**：RemoveNative(10) 见 `Death`→KillUnit；Transition(20) `Death→Corpse`；Dispose(30) 非 Disposing→跳过。✅ I1
- **ClearCorpse tick**：RemoveNative 见 `ClearCorpse` 无操作；Transition→`Remove`；Dispose 见 `Remove` 跳过（等待）。下一 tick：RemoveNative 见 `Remove`→RemoveUnit+`Disposing`；Dispose 见 `Disposing`→删除。✅ I2
- **直删 tick**：RemoveNative 见 `Remove`→RemoveUnit+`Disposing`；Dispose→删除。✅ I2

> 关键：`Dispose` 只在 `Disposing` 阶段删除，杜绝“抢在原生移除之前”。

### 3.2 备选方案
- **方案 B：合并为单一 Finalize 系统**。内部按 `Death→KillUnit→Corpse`、`ClearCorpse→Remove`、`Remove→RemoveUnit→dispose` 顺序处理。最稳，但破坏“transition / native / dispose”职责分离。
- **方案 C：确认布尔标记**（`UnitLifeState.nativeDisposed`）替代新阶段。与 A 等价，但状态含义不如显式阶段清晰。
- **结论**：采用 **A**（保留职责分离 + 显式阶段可测）。

## 4. 终态清理补全

`UnitHelper.CleanupFinalizeEntityDispose(entity)` 追加（顺序即依赖顺序）：

```
RemoveTag<TimerExpired>
RemoveAllAuras(entity)            // 新增：删除 aura 实体及其 aura buff
DetachAllItems(entity)            // 新增：解绑 ItemOwner、回收 item companion
RemoveAllAbilities(entity)        // 现有：需覆盖无 slot 的 companion
RemoveAllAttrs(entity)
entity.DeleteEntity()
```

- **Aura**：`AuraHelper.RemoveAllAuras(owner)` 已存在，补调用即可。
- **物品（安全解绑 + 回收）**：对每件 `ItemOwner` 指向该单位的物品调用 `ItemLifecycleOperations.Detach(item, dropToGround: false, 0,0,0)`（移除 `ItemOwner`/`ItemSlotIndex`、递减 `ItemSlotContainer.currentCount`、`ItemCompanionAbilityHelper.UnbindOwner`、`RemoveModifiersFromSource`），再置 `ItemDestroyPendingTag` 交由 `ItemCompanionDeferredDeleteSystem`(order 131) 在 companion 引用释放后删除物品实体。
  - **死亡路径不掉落**（已定）：若业务需要物品落地，应在进入销毁**之前**由外部显式调用 `ItemHelper.DropToGround(item, x, y)` 把物品转为地面物品（`ItemGroundTag`），届时它已不挂在单位上，死亡清理不会再回收它。
- **伴生技能**：`AbilitySlotHelper.RemoveAllAbilities` 目前只处理带 `AbilitySlotIndex` 的技能；需扩展为同时清理 `AbilityOwner` 指向该单位的全部技能（含无 slot 的 companion）。
- 该 helper 在删除实体，需按 `fix-query-loop-structural-mutation` 规则**在循环外**执行。

## 5. `UnitHelper.RemoveUnit`（已定：保留并启用）

零调用点，但仍保留，作为“**非死亡直接移除**”入口（与 `KillUnit` 死亡语义区分：`RemoveUnit` 不广播 `UnitDiedEvent`，直接把 phase 置 `Remove`）。
启用后的正确时序由 §3 的阶段机保证：`Remove → RemoveNative(RemoveUnit+HandleRemove+Disposing) → Dispose`。
实施时补充一个验证场景覆盖该入口（直删路径）。

## 6. 验证设计

| 场景 | 断言 |
|---|---|
| Alive→Death | phase 依次 `Death→Corpse`；KillUnit 分支被消费（探针） |
| 直删 Remove | 顺序探针：RemoveUnit 先于实体删除；`Disposing` 被消费 |
| 死亡终态 | aura/item/companion 全部回收；无悬挂 `ModifyTarget`/`ItemOwner`/`AbilityOwner` |
| 回归 | 既有场景全量 PASS |

## 7. 实施顺序

1. 阶段枚举 + order。
2. `RemoveNative` 与 `Dispose` 语义改造（配合循环外结构变更修复）。
3. 终态清理补全。
4. 场景验证 → 回归 → `summary.md`。
