# 提案：属性实体生命周期完整化（唯一性强制 + 孤儿回收）

## 元信息

- **状态**：已实施
- **等级**：`full`
- **变更 ID**：`enforce-attr-entity-lifecycle`
- **日期**：2026-09-10
- **请求来源**：用户在属性系统复盘时提出（唯一性未强制 / 入口不一致 / 孤儿属性不回收）
- **前置变更**：`auto-create-attr-and-rename-damage`（`light`，已实施）——本提案是其直接后续
- **默认实施后审查强度**：`R2 Targeted`
- **命中的审查升级触发器**：公共 API / 数据结构契约（`AttributeHelper`、`ModifyHelper` 行为契约）；跨系统状态协作（属性域 ↔ 装备/Buff/Aura/原生同步）
- **最终实施后审查强度**：`R2 Targeted`
- **Oracle 可用性与 `R1` 回退方式**：不适用（本提案为 `full`，默认 `R2`；若某专项复核需降为单视角，按 Oracle 可用性记录回退）
- **完整 `review-work` 授权来源**：无（未获用户明确要求，不启用）

## 1. 分级判定

### 1.1 为什么是这个等级

- **影响范围**：`War3Frame/` 属性域核心（Helper / System / Component 契约）。
- **风险等级**：中高——涉及删除 ECS 实体与关系，判断错误会导致属性丢失或悬挂引用。
- **可逆性**：代码可回滚；但错误删除运行期实体不可逆（须靠判据保守 + 测试防住）。
- **是否跨项目**：否（`War3Frame.Generator` / `FrameBuild` / `CSharpWar3Frame` 均不受影响）。
- **是否改公共契约**：是——`AttributeHelper.CreateAttr` 可见性与 `GetOrCreateAttr` 语义、`ModifyHelper` 入口契约。

命中"改公共 API / 数据结构契约"与"跨系统状态协作"，故至少 `full`。

### 1.2 升级触发器检查

- [ ] 涉及 `War3Frame/` 与其他项目联动（否）
- [ ] 涉及 `War3Frame.Generator/` 输出或契约（否）
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程（否）
- [ ] 涉及 `CSharpWar3Frame/` 入口行为（否）
- [ ] 涉及 `Projects/` 示例或集成验证行为（间接：验证场景会用到，但契约不变）
- [x] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组（否）

### 1.3 不升级到 `architecture` 的理由

- 不改变分层边界、不引入基础设施、不调整项目依赖。
- 属性集合策略仍保持"开放集合"（与 GAS 式封闭集合相反），不构成架构级改造。
- 若无 Part A，仅 Part B 为局部行为增强，可降为 `light`；本提案因包含公共契约收敛而取 `full`。

## 2. 背景 / Why

`auto-create-attr-and-rename-damage` 为解决"贡献静默丢弃"引入了 `AttributeHelper.GetOrCreateAttr`：单位未声明某属性时，贡献路径（装备/Buff）会动态创建 `base=0` 的属性实体。这解决了当时的问题，但暴露了属性实体生命周期中两个**从未被强制**的不变量：

1. **唯一性未强制**。`AttributeHelper.CreateAttr` 为 `public` 且无去重；`HasAttr` 关系键是 `attrEntity`（`Attribute.cs`）而非 `typeId`，因此同一 unit 可挂多个同 `typeId` 的属性实体。`GetAttr` 遍历关系只返回第一个匹配，重复实体成为不可见孤儿。触发条件无需乱调 API——只要某贡献方的 lazy 创建早于模板/升级声明，就会发生。

2. **孤儿属性不回收**。`GetOrCreateAttr` 创建的属性实体，在贡献者（装备/Buff）移除后不被回收：`ModifyHelper.RemoveModifiersFromSource` 只删 modifier（`ModifyHelper.cs`），属性实体随单位常驻。

3. **入口不一致**。`ModifyHelper.AddModifier(attrEntity, ...)` 接受任意属性实体，可绕过 get-or-create；与 `ModifyHelper.AddModifierToUnit(unit, typeId, ...)` 并存，使唯一性约束在 API 层不可达。

现状核查（事实源为仓库当前代码）：

| 创建/写入点 | 入口 | 是否去重 |
|---|---|---|
| `UnitSpecBuilder` | `CreateAttr` | 否 |
| `LevelExperienceSystem` | `CreateAttr` | 否 |
| `ModifyHelper.AddModifierToUnit` | `GetOrCreateAttr` | 是 |
| `BuffHelper.CreateBuffInternal` | `GetOrCreateAttr` | 是 |

## 3. 变更范围 / What

### Part A：唯一性强制 + 入口收敛

- 属性实体创建原语收敛为唯一入口，强制"同一 unit 同一 `typeId` 至多一个属性实体"这一不变量。
- 处理 `CreateAttr` 可见性与 `ModifyHelper.AddModifier` 的裸 `attrEntity` 入口。

### Part B：孤儿属性回收

- 在 `AttrCalculationSystem`（order 45）重算时，顺带回收满足全部条件的属性实体，并同步移除 `HasAttr` 关系。

### 范围选择（提交审核时请确认）

- **默认**：Part A + Part B 一起做。
- **可选**：只批准 Part B（回收），Part A 另行提案。Part B 可独立成立，Part A 是更彻底的入口治理。
- 无论选哪种，未批准的范围**不得实现**。

## 4. 非目标

- **不采用 GAS 式封闭属性集合**。属性仍可在运行时按需创建（本框架的装备/技能可授予任意属性，比 GAS 的预声明更灵活）。GAS 对标结论见 `design.md`。
- **不改属性计算公式与 modifier 语义**（`(base+flat)×(1+percentAdd)×percentMul` 保持）。
- **不实现多来源优先级 / evaluation channel**（`ModifyValue.priority` 仍为保留字段，另行提案）。
- **不改 native 同步机制**（血量/蓝量仍 Compare-Sync）。
- **不统一技能属性域**（`AbilityHelper.AddModifier` 的抛异常语义与 `AbilityStatDirty` 不在本次范围）。
- **不引入通用 GC / 引用计数框架**。

## 5. 全局影响分析

- **`War3Frame/`**：受影响。`AttributeHelper`、`ModifyHelper`、`AttrCalculationSystem` 为改动核心；`UnitSpecBuilder`、`LevelExperienceSystem` 因 Part A 需调用点适配。
- **`War3Frame.Generator/`**：不受影响。本提案不涉及 `[SystemRegister]` 生成契约或任何生成器输出；仅复用现有系统注册机制。
- **`FrameBuild/`**：不受影响。不涉及构建链路、发布流程或产物契约。
- **`CSharpWar3Frame/`**：不受影响。CLI/入口不引用属性实体创建原语。
- **`Projects/`**：仅验证/示例层面可能用到；验证场景自带本地 `CreateAttr` 辅助方法，不依赖 `AttributeHelper.CreateAttr` 可见性，故示例文件不需要因 Part A 而改动。

## 6. 设计要点（详见 `design.md`）

### 6.1 回收判据（精确定义）

属性实体可回收，当且仅当**全部**成立：

1. 该属性实体**不带** `AttrDirty`（已重算，`flatBonus` / `percentBonus` 为最新值）；
2. `GetIncomingLinks<ModifyTarget>()` **为空**（无任何 modifier 指向它）；
3. `baseValue == 0`；
4. `current == 0`；
5. `flatBonus == 0 && percentBonus == 0`；
6. 该 `attrTypeId` **未被 owner 单位的 `UnitSpecData` 声明**；且 owner **必须带 `UnitSpecData`**（模板单位）。若 owner 无 `UnitSpecData` 或非 unit，视为"无法判定声明"，**保守保留，不回收**。

`current == 0` 用于排除"曾挂资源类 modifier 把现值写成非 0"的情形——宁可漏收，不可误删。同理，owner 无 `UnitSpecData` 时无法判定哪些属性是模板声明的，一律保守保留，避免误删无模板单位的属性。

### 6.2 放置与时机

- **并入 `AttrCalculationSystem`**（order 45，重算后判定）。
- 优点：只处理 `AttrDirty` 属性，天然满足判据 1，成本 `O(处理量)`。
- 已知盲区：**从不 dirty 的属性不会被扫到**。这些恰多为"声明但未使用"的属性，本应保留；直接删 modifier 而不打脏的路径属其他系统的缺陷，不在本次兜底范围。

### 6.3 删除序列（不可逆操作纪律）

- 迭代内禁止结构变更 → 收集待删列表后**循环外**执行。
- 待删列表**按 entity id 排序**，保证锁步确定性。
- 删除顺序：先 `unit.RemoveRelation<HasAttr, Entity>(attr)`，再 `attr.DeleteEntity()`（删除实体不会自动清理反向关系）。

## 7. 风险、兼容性、迁移

### 风险

1. **误删模板声明的财产性属性** → 由判据 6（`UnitSpecData` 未声明）规避；不用"谁创建的"作为判据（无法区分 lazy 抢先）。
2. **悬挂 `HasAttr` 关系** → 删除前显式 `RemoveRelation`。
3. **悬空 `Entity` 引用** → 需确认无系统长期缓存 attr 实体；已知 `UnitNativeSyncSnapshot` 只存 `attrTypeId` 并按 typeId 反查，安全。
4. **确定性** → 排序 + 固定遍历。
5. **`current` 边界** → 判据 4 保守处理；不额外 clamp。
6. **Part A 收窄 `CreateAttr` 可见性** → 属公共契约变更，需确认无框架外部调用；仓库内调用点为 `UnitSpecBuilder` / `LevelExperienceSystem`，同程序集。

### 兼容性

- 运行时行为变化：无贡献、未声明、归零的属性实体将从 ECS 消失。读取侧经 `AttributeHelper`（缺失返回 0）语义不变。
- 不改变属性 ID 注册、不改变 modifier 数据形状。

### 回滚

- 回滚本次改动文件即可（`AttributeHelper` / `ModifyHelper` / `AttrCalculationSystem` / 两处调用点）。
- 成本：低——改动集中，无持久化、无跨项目契约。

## 8. 验证计划

1. **编译**：`War3Frame` + `Projects/test` 0 error。
2. **回收正向**：装备授予单位未声明的护盾属性（base=0）→ 属性实体创建；脱下装备 → 属性实体被回收，且 `HasAttr` 关系已移除。
3. **回收正向（Buff）**：对未声明属性施加减速/加成 Buff → 属性创建；Buff 过期 → 属性回收。
4. **回收反向（声明属性）**：模板声明 `Health` 等 → 即使无 modifier 且归零，**不得**被回收。
5. **回收反向（资源现值）**：曾把 `current` 写为非 0 的属性，贡献者移除后**不得**被回收。
6. **唯一性（Part A）**：对同一 unit 同一 `typeId` 连续两次 get-or-create → 只存在一个属性实体。
7. **重算链路**：回收后，剩余属性的 `finalValue` 与 `AttrDirty` 清除行为正常。
8. **静态检查**：全仓 grep 确认 `CreateAttr` 无跨程序集引用；确认无迭代内删除。
9. 按 `R2 Targeted` 记录证据与 verdict，完成后写 `summary.md`。

## 9. 拆分任务

1. **Part A**：唯一性 + 入口收敛（`AttributeHelper` / `ModifyHelper` / 两处调用点）。
2. **Part B**：`AttrCalculationSystem` 回收逻辑（判据 + 删除序列 + `HasAttr` 清理）。
3. **验证**：编译 + 正向/反向场景 + 静态检查。
4. **总结**：`summary.md` + 状态更新（仅测试通过后）。

## 10. 工件

- `proposal.md`（本文件）
- `design.md`
- `tasks.md`
- `specs/attribute-entity-lifecycle/spec.md`

## 11. 相关文档

- `openspec/changes/auto-create-attr-and-rename-damage/proposal.md`（前置）
- `openspec/changes/archive/2026-08-13-unify-attr-dirty-entity-ownership/proposal.md`（`AttrDirty` 归属契约）
- `AGENTS.md`（ECS 命名 / Native 同步 / Helper 分层）
- `War3Frame/Src/Helpers/AttributeHelper.cs`
- `War3Frame/Src/Helpers/ModifyHelper.cs`
- `War3Frame/Src/Systems/Attribute/AttrCalculationSystem.cs`
