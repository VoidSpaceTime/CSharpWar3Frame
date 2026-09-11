# 统一 AttrDirty 实体归属

## 变更信息
- Change ID: `unify-attr-dirty-entity-ownership`
- 等级: `light`
- 范围: 仅 `War3Frame/`

## 背景
`AttrDirty` 目前被用作属性重算触发器，但现有调用点把 unit 侧和 attr 侧的挂载方式混在一起了。这会让 dirty 的归属语义变模糊，也让 `AttrCalculationSystem` 承担了不必要的语义面。

## 目标
把 `AttrDirty` 收束为只能挂在 attr entity 上的 tag。

具体要求:
- `AttrDirty` 只标记拥有重算状态的 attr entity。
- unit 侧代码先解析出对应的 attr entity，再给该 attr entity 打 dirty。
- 不再保留 `unit` / `attr` 混挂的语义。

## 关于 UnitDirty 的判断
本次检查后，不建议同步引入一个通用的 `UnitDirty`。

原因是当前仓库里的 unit 侧状态并没有形成一条统一的“单位脏标记”语义链，现有做法更接近下面这种分工：
- 属性重算由 `AttrDirty` 驱动，目标始终是 attr entity。
- `HealthSystem` / `ManaSystem` 直接处理 attr entity 上的数值推进。
- `UnitNativeSystem` 负责把 attr entity 的结果同步到原生 unit。

因此，unit 侧若将来出现独立的脏语义，也应按具体状态单独定义，不与 `AttrDirty` 混用，更不应为了这次收束顺手造一个通用 `UnitDirty`。

## 影响范围
- `War3Frame/`: 受影响，主要是属性 helper、buff/aura helper 和属性计算路径。
- `War3Frame.Generator/`: 预期不受影响。
- `FrameBuild/`: 预期不受影响。
- `CSharpWar3Frame/`: 预期不受影响。
- `Projects/`: 预期不受影响。

## 可能受影响的文件
- `War3Frame/Src/Components/Attribute/Attribute.cs`
- `War3Frame/Src/Systems/AttrCalculationSystem.cs`
- `War3Frame/Src/Systems/BuffSystem.cs`
- `War3Frame/Src/Systems/AuraSystem.cs`
- `War3Frame/Src/Helpers/ModifyHelper.cs`
- `War3Frame/Src/Helpers/BuffHelper.cs`
- `War3Frame/Src/Helpers/AuraHelper.cs`

这次只做 dirty 归属收束，不重做属性模型本身。

## 风险
- 现有 unit 侧代码可能隐式依赖 `AttrDirty` 直接挂在 unit 上。
- 若遗漏某个调用点，属性变更可能延后到下一次变更才被重算。
- 若未来确实出现 unit 自身的独立重算需求，需要另起提案定义专用 dirty 语义，不能复用本次的 `AttrDirty` 规则。

## 回滚
- 如果验证发现遗漏路径，回退这次调用点归属调整，恢复原有 dirty 挂载方式。

## 验收标准
- `AttrDirty` 只会加到 attr entity 上。
- 不再存在 `unit.AddTag<AttrDirty>()`。
- 所有属性变更路径仍能正确触发重算。
- buff / aura / modify 相关的属性刷新行为保持正常。

## 验证方式
- 全局搜索 `AttrDirty` 调用点，确认只落在 attr entity 上。
- 编译并运行 `War3Frame` 相关验证或局部编译检查。
- 确认属性变更路径在重构后仍会触发重算。
