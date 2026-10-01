# Tasks：无敌/免疫期间施加的控制被吸收

- **Change ID**：`absorb-control-applied-while-immune`
- **等级**：`light`

## T1. 判定入口

- [x] `ControlHelper.cs`：新增 `IsControlAttr(int attrTypeId)`（查 `ControlAttrs` 权威表）。
- [x] `ControlHelper.cs`：新增 `ShouldAbsorbControl(Entity unit, int attrTypeId)`——非控制属性返回 false；`Invulnerable > 0` 返回 true；对应免疫属性 > 0 返回 true。

## T2. 两条写入路径

- [x] `BuffHelper.cs`：`CreateBuffInternal` 内按吸收判定计算 `contribution`，`ModifyValue.value` 与 `BuffStacks.Create(...)` 统一使用该值；未命中时行为与现状完全一致（含 DoT/PureTag 分支）。
- [x] `ModifyHelper.cs`：`AddModifier` 内新增 `ResolveAbsorbedValue`——从 `attrEntity` 取 `AttrTypeId` 与 `AttrOwner.owner`，命中则写 0；非控制属性 / 零值 / 解析失败原样返回。

## T3. 验证

- [x] `Projects/Regression/DomainRegression.cs`：新增 `absorb-control-under-immune` 用例，覆盖验收标准 1–6。
- [x] `dotnet build War3Frame/War3Frame.csproj` 0 error。
- [x] `dotnet build Projects/test/test.csproj` 0 error（4 个存量 CS8629 warning）。
- [x] `dotnet run --project Projects/Regression`（32 用例）全绿，既有 31 用例无回归。
- [x] 静态核对：吸收判定只走 `ControlHelper.ShouldAbsorbControl` 单一入口；`GetEffectiveValue` 未被改动。

## T4. 收尾

- [x] `summary.md` 记录改动范围、验证结果、剩余风险（同 tick 时序约束 + 客户端验证非阻塞）。
- [x] `openspec validate absorb-control-applied-while-immune`。

## 实施中发现

- 吸收判定读取 `finalValue`，与 `GetEffectiveValue` 的读取压制同源；因此同一 tick 内"先施加无敌/免疫、再施加控制"（中间无属性重算）会漏吸收。已在测试中以"先用一个 tick 结算无敌"的写法固化，并记入 `summary.md` 剩余风险。若要收紧需在判定中做脏贡献聚合，属独立议题。
