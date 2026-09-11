# 设计：属性实体生命周期完整化

对应提案：`proposal.md`（`enforce-attr-entity-lifecycle`，`full`）

## 1. 问题模型

属性系统当前有两条创建路径，产出的属性实体结构**完全相同**，无法从数据上区分来源：

| 路径 | 入口 | 语义 | 期望生命周期 |
|---|---|---|---|
| 声明 | `AttributeHelper.CreateAttr` | 模板声明 / 升级补属性 | 随单位常驻 |
| 懒建 | `AttributeHelper.GetOrCreateAttr` | 装备/Buff 贡献触发 | 贡献者消失后可回收 |

两条路径都不强制唯一性。`HasAttr` 关系键为 `attrEntity`，`GetAttr` 只返回首个匹配，因此重复实体的第二条起不可见。

## 2. 不变量（本次要强制的契约）

- **INV-1（唯一性）**：同一 unit 同一 `typeId` 至多存在一个属性实体。
- **INV-2（可达性）**：不存在无法通过 `HasAttr` 关系访问到的属性实体。
- **INV-3（保留性）**：单位声明过的属性实体，在单位存活期间不被回收。
- **INV-4（回收性）**：未被声明、无贡献者、且处于初始值状态的属性实体，不长期滞留。

## 3. Part A：唯一性 + 入口收敛

### 3.1 创建原语收敛

现状 `CreateAttr` 为 `public` 且无去重。方案：

- 将 `AttributeHelper.CreateAttr` 收窄为 `internal`（或重命名为 `CreateAttrInternal`），并加**防御性幂等**：创建前先 `TryGetAttr`，命中则直接返回既有实体。这样即便误用也无法产生重复。
- 对外唯一入口为 `GetOrCreateAttr(unit, typeId, baseValue = 0)`。
- 调用点适配：
  - `UnitSpecBuilder`：改用 `CreateAttrInternal`（或 `GetOrCreateAttr`）。初始声明时属性不应存在，幂等不影响语义。
  - `LevelExperienceSystem`：已先 `TryGetAttr` 再 `CreateAttr`，改为直接走收敛入口；现有"存在则更新 baseValue + 打脏"的分支保留（升级语义，非回收）。

**为什么不用"谁创建的"做标记**：`GetOrCreateAttr` 可能抢在声明之前创建，产物与 `CreateAttr` 无差别。用来源标记会误伤"lazy 抢先的声明属性"。故唯一性靠单一原语 + 幂等，回收靠"是否被 `UnitSpecData` 声明"，而非来源。

### 3.2 `ModifyHelper` 入口收敛

- `ModifyHelper.AddModifier(attrEntity, source, type, value, priority)` → 收窄为 `internal`/`private`。其唯一调用方是 `AddModifierToUnit`（仓库已核对为 1 caller），收窄安全。
- 对外保留 `ModifyHelper.AddModifierToUnit(unit, typeId, source, type, value)`，内部先 `GetOrCreateAttr` 再委托。
- 效果：调用方无法再传入一个重复/不属于该 unit 的属性实体，INV-1 在 API 层不可破。

### 3.3 不改动项

- `AbilityHelper.AddModifier`（技能属性域，抛异常语义）保持不动，避免范围外变更；其与单位域语义不一致列为后续提案候选。

## 4. Part B：孤儿回收

### 4.1 判据

属性实体 `A` 可回收，当且仅当：

```
NOT A.HasTag<AttrDirty>()
AND A.GetIncomingLinks<ModifyTarget>().Count == 0
AND A.AttrValue.baseValue  == 0
AND A.AttrValue.current    == 0
AND A.AttrValue.flatBonus  == 0
AND A.AttrValue.percentBonus == 0
AND CanReclaimByOwner(A)
```

`CanReclaimByOwner(A)`（保守策略）：

- 取 `A.GetComponent<AttrOwner>().owner`；
- 若 `A` 无 `AttrOwner`、owner 为 null / 非 unit / 无 `UnitSpecData` → 返回 **false（保留）**：无法判定声明，不冒误删风险；
- 否则遍历 `UnitSpecData.spec.attributes`：命中 `typeId` → 返回 false（模板已声明，保留）；未命中 → 返回 true（可回收）。

实现签名：`private static bool CanReclaimByOwner(Entity attrEntity)`。

判据 1（非 dirty）保证 `flatBonus` / `percentBonus` 是重算后的最新值；判据 4（current==0）保守排除资源现值残留；判据 6 的 owner 判定同样保守——无 `UnitSpecData` 时保留不回收。

### 4.2 放置与 order

并入 `AttrCalculationSystem`（`[SystemRegister(SystemKind.Interval, 45)]`）。

```
OnUpdate:
  收集 recalculated（重算）
  收集 toReclaim（命中判据）
  循环外：
    先 RemoveTag<AttrDirty>（对 recalculated）
    再按 id 排序，逐个 RemoveRelation<HasAttr> + DeleteEntity（对 toReclaim）
```

- 系统已带 `Filter.AnyTags(Tags.Get<AttrDirty>())`，因此只处理脏属性。
- 判据 6 的 `UnitSpecData` 扫描仅在其余条件全部满足时执行（罕见路径），成本可忽略。

### 4.3 删除序列与确定性

1. 不在 `Query.ForEachEntity` 内做结构变更（Friflo 硬约束）。
2. `toReclaim` 先按 `entity.Id` 升序排序。
3. 对每个属性实体：`owner.RemoveRelation<HasAttr, Entity>(attr)` → `attr.DeleteEntity()`。
4. `DeleteEntity` 前再校验 `!attr.IsNull`（防同帧已被删）。

### 4.4 链路闭合性

所有贡献移除路径都已触发重算，使回收判定能在同一帧或下一帧到达：

| 路径 | 位置 |
|---|---|
| `ModifyHelper.RemoveModifiersFromSource` | `ModifyHelper.cs`（打 `AttrDirty`） |
| `BuffExpireSystem` | `BuffSystem.cs`（打 `AttrDirty`） |
| `AuraSystem` | `AuraSystem.cs`（打 `AttrDirty`） |
| Item apply/remove 流程 | `ItemSystem.cs`（`RemoveModifiersFromSource`） |

故"贡献者消失 → 属性变脏 → 重算时判定回收"闭合。

### 4.5 已知盲区（显式声明，不掩盖）

- **从不 dirty 的属性不被扫到**。这类多为"声明但未使用"，INV-3 要求保留，符合预期。
- 若某路径删 modifier 却不打脏，回收不会触发——这属于该路径的缺陷，不在本系统兜底；验证计划中列入检查项。

## 5. 备选方案比较

### 方案 A：并入 `AttrCalculationSystem`（采用）

- 优点：只处理脏属性，成本 `O(处理量)`；天然满足判据"已重算"；无新增系统。
- 缺点：不覆盖从不 dirty 的属性（可接受，见 4.5）。

### 方案 B：独立 sweep 系统每帧全扫

- 优点：覆盖全量属性。
- 缺点：每帧 `O(属性实体数)`；且需自行保证"已重算"（要读 flatBonus/percentBonus 的时效）。收益不抵成本，**不采用**。

### 方案 C：provenance 标记（给 lazy 创建的属性打 `AutoCreatedAttrTag`）

- 优点：直接区分来源。
- 缺点：无法处理"lazy 抢先于声明"的实体（被标记却在后来被声明 → 误删风险）；仍需声明判定兜底。**不采用**。

### 方案 D（GAS 式）：属性封闭集合，永不动态创建，不回收

- 优点：从根上消除孤儿属性；属性生命周期简单。
- 缺点：本框架的装备/技能需要授予任意属性，封闭集合要求改 native 代码重新编译，牺牲数据驱动灵活性。与 `auto-create-attr-and-rename-damage` 已确立的"开放集合"方向相悖。**不采用**，但保留其 BaseValue/聚合值分离、脏驱动重算两项纪律（本框架已具备）。

## 6. 数据流

```
装备/Buff 贡献
  → ModifyHelper.AddModifierToUnit(unit, typeId, ...)      [Part A 唯一入口]
      → AttributeHelper.GetOrCreateAttr(unit, typeId)       [INV-1 保证]
      → 建/取 modifier 实体 (ModifyValue + ModifyTarget + ModifySource)
      → attr.AddTag<AttrDirty>()
  → AttrCalculationSystem (45)
      重算 finalValue/flatBonus/percentBonus
      若命中回收判据 → 收集 toReclaim
  → 循环外：RemoveTag<AttrDirty> / RemoveRelation<HasAttr> + DeleteEntity
```

## 7. 测试映射

| 验收项 | 测试方式 |
|---|---|
| INV-1 唯一性 | 同 unit 同 typeId 连续 get-or-create，断言属性实体数为 1 |
| INV-2 可达性 | 回收后断言 unit 无悬挂 `HasAttr` |
| INV-3 保留性 | 声明属性无 modifier + 归零，断言不被回收 |
| 回收正向（装备） | 未声明属性 + 装备，脱下后断言实体消失 |
| 回收正向（Buff） | 未声明属性 + Buff，过期后断言实体消失 |
| 资源现值保护 | current≠0 的属性，贡献移除后断言不回收 |
| 重算正确性 | 回收后剩余属性 finalValue 与 dirty 清除正常 |

建议新增/扩展 `Projects/test/Scripts/Process/` 下的属性生命周期验证场景。

## 8. 实施顺序

1. Part A（唯一性 + 入口收敛）→ 编译通过。
2. Part B（回收逻辑）→ 编译通过。
3. 验证场景 → 全部通过。
4. `summary.md`。

## 9. 未决/后续

- `ModifyValue.priority` 的 evaluation channel 语义（GAS 参考）——另行提案。
- 技能属性域 `AbilityHelper.AddModifier` 与单位域语义统一——另行提案。
- `ModifierSourceType` 枚举在两个 namespace 重复定义——独立清理项，不并入本提案。
