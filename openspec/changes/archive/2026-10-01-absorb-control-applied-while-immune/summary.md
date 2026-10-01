# 实施总结：无敌/免疫期间施加的控制被吸收

- **变更 ID**：`absorb-control-applied-while-immune`
- **等级**：`light`（复盘强度 `R0 Direct`）
- **状态**：`已实施`（2026-09-17）
- **决策来源**：用户 2026-09-17 明确选择"被吸收、贡献 +0"且"无敌与免疫都吸收"；已施加的控制不变更。

## 1. 问题

无敌/免疫此前只压制**读取**（`ControlHelper.GetEffectiveValue`），`finalValue` 照常累加。结果是无敌期间施加的眩晕被完整保留，无敌一结束（buff 未过期）立即生效——"延迟引爆"，且没有对应的新施加动作。

## 2. 实际改动范围

| 文件 | 改动 |
|---|---|
| `War3Frame/Src/Helpers/ControlHelper.cs` | 新增 `IsControlAttr`（查 `ControlAttrs` 权威表）与 `ShouldAbsorbControl`（无敌 > 0 或对应免疫 > 0）；复用既有 `GetAttrValue` / `GetImmunityAttrId` |
| `War3Frame/Src/Helpers/BuffHelper.cs` | `CreateBuffInternal` 计算 `contribution`，控制属性命中吸收时 `ModifyValue.value` 与 `BuffStacks` 每层值均为 0；DoT/PureTag 分支不受影响 |
| `War3Frame/Src/Helpers/ModifyHelper.cs` | `AddModifier` 新增 `ResolveAbsorbedValue(attrEntity, value)`，经 `AttrTypeId` + `AttrOwner.owner` 解析目标单位后在控制属性上降为 0 |
| `Projects/Regression/DomainRegression.cs` | 新增 `runtime/absorb-control-under-immune` 用例（A–F 六段） |

`ControlHelper.GetEffectiveValue`（读取压制）与 `ControlStateTransitionSystem`（Pause 合成）**未改动**，两套机制并存：

- 已存在的控制贡献：无敌期间不生效，无敌结束后恢复（读取压制）。
- 新施加的控制：直接被吸收为 0，不会留下残余。

## 3. 行为契约

| 场景 | Stun 属性 finalValue | 是否暂停 |
|---|---|---|
| 无敌期间施加 Stun | 0（吸收） | 否，且无敌结束后仍否 |
| 免疫期间施加 Stun | 0（吸收） | 否，且免疫结束后仍否 |
| 无无敌/免疫施加 Stun | 1 | 是 |
| 先 Stun 后无敌 | 1（保留） | 无敌期间否；无敌结束后是 |
| 无敌期间施加非控制属性贡献 | 原值 | 不适用 |

## 4. 验证结果

- `dotnet build War3Frame/War3Frame.csproj`：**0 error**（存量 nullable warning 与本次无关）。
- `dotnet build Projects/test/test.csproj`：**0 error**（4 个存量 CS8629 warning）。
- `dotnet run --project Projects/Regression`：**Executed 32; passed 32; failed 0**；既有 31 用例（含 `pause-synthesis`、11 个 `Projects/test` 场景、`DamagePipelineValidationScenario` phase8 无敌压制）全部无回归。
- 新用例断言：无敌期间属性贡献路径与 buff 路径均吸收为 0；无敌/免疫解除后仍为 0 且不产生 Pause 请求；无无敌/免疫时正常累加并暂停；先眩晕后无敌时贡献保留、有效值被压制、无敌解除后恢复；非控制属性（Health）贡献不受影响。

## 5. 全局影响分析

| 区域 | 影响 |
|---|---|
| `War3Frame/` | 三个 Helper（本次范围） |
| `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/` | 不受影响：无生成器契约、构建链、CLI 改动 |
| `Projects/` | 仅 `Regression` 新增用例；`test` 场景使用本地私有 `AddModifier`（不经过 `ModifyHelper`），语义不变 |

## 6. 剩余风险与约束

- **同 tick 时序约束**：吸收判定读 `finalValue`，与 `GetEffectiveValue` 同源。若同一 tick 内先施加无敌/免疫、再施加控制，且中间没有属性重算，则无敌/免疫的 `finalValue` 尚为 0，本次吸收会漏判（控制贡献照常写入）。这是既有读取压制机制的同一特性，非本次新引入。若要收紧，需在判定中做脏贡献聚合，建议单独立项。
- **buff 刷新/叠层路径未覆盖**：`RefreshCore` 对既有 `ModifyValue.value` 的改动不在吸收范围内；被吸收的 buff 仍存在于实体与时长上（UI 可能仍显示），只是贡献为 0。
- **未执行**：War3 客户端运行时验证（缺可自动执行的客户端验证协议，用户已确认暂不执行）。
