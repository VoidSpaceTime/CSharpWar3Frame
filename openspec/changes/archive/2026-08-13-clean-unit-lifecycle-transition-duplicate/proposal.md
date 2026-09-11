# Proposal: clean-unit-lifecycle-transition-duplicate

**等级**: light
**状态**: 待审核

## 现状

单位生命周期的推进逻辑目前在两处并存：

### 1. `UnitLifecycleTransitionSystem`（系统层，正确位置）

```csharp
// Death → Corpse
// ClearCorpse → Remove
```

每帧查询所有带 `UnitLifeState` 的实体，推进 phase。这是提案确定的唯一推进层。

### 2. `UnitHelper.TransitionLifecycle`（helper 层，冗余）

```csharp
// UnitHelper.cs:114 — 注释写明"兼容旧调用入口"
// 逻辑与 TransitionSystem 完全相同：
// Death → Corpse / ClearCorpse → Remove / Remove → Dispose
```

该方法还额外承担了 `Remove → Dispose`（调 `CleanupFinalizeEntityDispose`），
而 `UnitLifecycleDisposeSystem` 已经在系统层做了同样的事。

**风险**：如果某处在 system tick 外直接调 `TransitionLifecycle`，
会导致 phase 被推进两次（helper 推一次，system 再推一次），
Death 可能跳过 Corpse 直接到 Remove，尸体停留计时器失效。

### 3. `RebornPending` / `Pooled` 枚举值无实现

`UnitLifecyclePhase` 中有 `RebornPending` 和 `Pooled`，
但全仓没有任何 System 消费这两个 phase。

```csharp
RebornPending, // 复活等待 — 无 System
Pooled         // 单位池   — 无 System
```

## 问题分类

| 问题 | 类型 | 风险 |
|------|------|------|
| `TransitionLifecycle` 与 `TransitionSystem` 逻辑重复 | 冗余/时序风险 | 中：double-push 可能静默跳过 Corpse |
| `Remove → Dispose` 在 helper 和 DisposeSystem 各做一遍 | 冗余/double-dispose 风险 | 中：可能重复 DeleteEntity |
| `RebornPending` 无 System | 功能空洞 | 低：暂无复活需求时不触发 |
| `Pooled` 无 System | 功能空洞 | 低：暂未使用 |

## 目标

1. 删除 `UnitHelper.TransitionLifecycle` 方法（或替换为仅写 intent 的薄调用）
2. 确认 `Remove → Dispose` 路径唯一，只由 `UnitLifecycleDisposeSystem` 持有
3. 为 `RebornPending` / `Pooled` 做明确标注：
   - 若当前项目不需要复活/单位池，从枚举中移除这两个值
   - 若未来需要，在枚举注释里标明"待实现"，并在 TransitionSystem 中加空分支占位

## 非目标

- 不改死亡/移除/尸体计时的主路径语义
- 不引入新的 System
- 不改 `UnitHelper.KillUnit` / `UnitHelper.RemoveUnit`（这两个已经只写 phase intent，是正确的）

## 影响范围

| 文件 | 操作 |
|------|------|
| `War3Frame/Src/Helpers/UnitHelper.cs` | 删除 `TransitionLifecycle` 方法；确认 `CleanupFinalizeEntityDispose` 只被 `UnitLifecycleDisposeSystem` 调用 |
| `War3Frame/Src/Components/Unit/UnitLifecycle.cs` | 视决策移除或注释 `RebornPending`/`Pooled` |
| `War3Frame/Src/Systems/Unit/UnitLifecycleTransitionSystem.cs` | 可选：加空分支或注释说明 Reborn/Pool 路径预留 |

## 验收标准

- 全仓无 `TransitionLifecycle` 调用
- `CleanupFinalizeEntityDispose` 只有 `UnitLifecycleDisposeSystem` 一处调用
- `UnitLifecyclePhase` 枚举中 `RebornPending`/`Pooled` 有明确处置（删除或注释标注"待实现"）
- 构建通过
