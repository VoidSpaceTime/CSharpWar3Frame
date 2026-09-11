## 基本信息

- Change ID: `introduce-level-stat-rebuild`
- 提案等级: `full`
- 目标一句话: 为 Unit / Item / Ability 引入统一的按等级数值规格与 `LevelStatRebuildSystem`，让等级变化只触发等级驱动的基础数值重算。
- 请求来源: 用户确认“先实现掉”，并指定系统命名为 `LevelStatRebuildSystem`。

## Why

当前 Unit、Item、Ability 的模板数值都以 `float` 写死：

- `UnitAttributeSpec.baseValue`
- `ItemAttributeContributionSpec.value`
- `AbilitySpec.baseValues`

这能表达固定值，但不能自然表达“1 级耗蓝 20，2 级耗蓝 40”或策划手填的非线性成长。后续如果各模块各自增加等级成长逻辑，会导致 Unit / Item / Ability authoring 语义不一致，也难以接入编辑器或配置化。

因此需要引入通用 `LevelValue`，并通过 `LevelStatRebuildSystem` 在等级变化时统一解析等级相关基础数值。

## What Changes

计划新增或调整：

- 新增通用等级数值规格：`LevelValue`、`LevelValueKind`。
- 纯数字 authoring 继续保留，并明确等价于固定值 `LevelValue.Fixed(value)`。
- Unit / Item / Ability 的模板数值存储从固定 `float` 扩展为 `LevelValue`。
- 新增 `LevelStatDirty` 标签，作为等级变化后的重算触发。
- 新增 `LevelStatRebuildSystem`，只负责“当前等级 + 模板 LevelValue -> 等级解析后的基础数值”。
- Unit / Item / Ability builder 增加接收 `LevelValue` 的重载。

## 非目标

- 不实现 Buff 最终合成。
- 不实现装备总属性聚合重构。
- 不改变 War3 Native 同步流程。
- 不实现复杂 delegate 公式作为主路径。
- 不在本变更中引入编辑器或外部配置格式。
- 不让 `LevelStatRebuildSystem` 处理伤害、治疗或即时结算。

## 影响范围

- `War3Frame/`: 直接影响 Unit / Item / Ability authoring 数据结构、builder 和等级重算系统。
- `War3Frame.Generator/`: 预期无影响；新增系统仍使用现有 `SystemRegisterAttribute`。
- `FrameBuild/`: 预期无影响；构建流程不变。
- `CSharpWar3Frame/`: 预期无影响；CLI 入口不变。
- `Projects/`: 示例模板可逐步迁移为 `LevelValue` 写法，并用于验证固定值兼容。

## 验收标准

- `.BaseValue(statId, 20)`、`.Attr(attrId, 20)` 等纯数字语义保持固定值。
- 能用 `LevelValue.PerLevel(20, 20)` 表达 1 级 20、2 级 40、3 级 60。
- 能用 `LevelValue.LevelTable(20, 40, 75)` 表达非线性手填数值。
- 等级变化时通过 `LevelStatDirty` 触发 `LevelStatRebuildSystem` 重算。
- `LevelStatRebuildSystem` 不直接调用 War3 native，不承担最终属性聚合或 Buff 合成。
- `dotnet build War3Frame/War3Frame.csproj` 与 `dotnet build Projects/test/test.csproj` 通过。

## 风险与回滚

- 风险: `float` 到 `LevelValue` 的存储变更影响已有 builder 和示例。
  - 缓解: 保留纯数字重载，纯数字始终按固定值解析。
- 风险: Item 装备属性贡献已有单条/多条两套数据形态，等级重算可能与现有申请系统重复。
  - 缓解: 第一阶段只解析模板基础贡献，不改装备聚合边界。
- 回滚: 可移除 `LevelValue` 重载和 `LevelStatRebuildSystem`，恢复模板数据为固定 `float`。
