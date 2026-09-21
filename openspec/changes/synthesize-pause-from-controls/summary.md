# 实施总结：Pause 状态合成（Stun / CrackFly / 纯 Pause 统一驱动 PauseUnit）

- **变更 ID**：`synthesize-pause-from-controls`
- **等级**：`light`（复盘强度 `R0 Direct`）
- **状态**：`已实施`（2026-09-17）
- **实施范围**：用户指定 T0 + T1-2 + T2-2~5，追加 T1-4 / T1-5；T4-2 与客户端验证按用户决定暂缓。

## 1. 问题与目标

`9695e6e`（2026-09-06）把原 `case ControlType.Stun → PauseUnit` 挪给了新枚举 `ControlType.Pause`，但**从未实现 Pause 的产生路径**（无 `Pause` 属性、`ControlAttrs` 不含 Pause）。结果是 Stun / CrackFly 只剩 ECS 属性与 `ControlStateChangedEvent`，原生单位不再暂停——本 change 补齐合成链路。

## 2. 实际改动范围

| 文件 | 改动 |
|---|---|
| `War3Frame/Src/Components/ControlState.cs` | `ControlStateSnapshot.bits` `byte`→`ushort`；`BitOf` / `SetActive` 同步；补 Pause 位与合成语义注释 |
| `War3Frame/Src/Helpers/AttributeHelper.cs` | 注册 `Pause = Register("Pause")`；不注册 `PauseImmunity`，并注释说明不可免疫理由 |
| `War3Frame/Src/Systems/ControlStateTransitionSystem.cs` | Stun / CrackFly 保留 `ControlStateChangedEvent`、**不再发各自 native 请求**；主循环外单独做 Pause 合成判定（`raw(Pause) ‖ effective(Stun) ‖ effective(CrackFly)`）并按快照位去抖发一次 `ControlStateNativeRequest(Pause)`；`toRelease` / `bits` 适配 `ushort` 并补 Pause 位释放；`IsControlOrImmunity` 纳入纯 `Pause` 属性 |
| `War3Frame/Src/Helpers/ControlHelper.cs` | `ControlAttrEntry` + `ControlAttrs` 提升为共享权威（`public`）；`GetImmunityAttrId` 由 if-链改为查表 |
| `War3Frame/Src/Components/ControlState.cs`（注释） | `ControlType.Stun` / `CrackFly` 语义更新为"暂停（Pause 合成驱动）+ 标记 + 表现" |
| `Projects/Regression/DomainRegression.cs` | 新增 `runtime/pause-synthesis` 用例（12 帧断言，见下） |
| `openspec/changes/synthesize-pause-from-controls/specs/control-state/spec.md` | 补齐 delta（该 change 写于官方 OpenSpec 迁移前，原无 spec delta，`openspec validate` 不通过）；归档时创建 `control-state` 能力 |

`UnitControlNativeSystem` 无改动：`case ControlType.Pause → JassApi.PauseUnit` 已存在且现在终于有请求源。

## 3. 行为契约

- 单位是否暂停 = `raw(Pause) > 0 ‖ effectiveStun > 0 ‖ effectiveCrackFly > 0`，任一为真即暂停，全归零才恢复。
- 同帧内多个控制变化只合成一次 Pause 请求（快照去抖，无抖动）。
- Stun / CrackFly 的免疫压制会影响合成（免疫生效即不暂停，免疫移除即恢复）；**纯 Pause 不经免疫**。
- Stun / CrackFly 跳变仍广播 `ControlStateChangedEvent`（特效、位移由业务层响应），但不再各自产生 native 请求。
- 属性被整体移除时，Pause 合成位同样补发解除请求，不会留下永久暂停。

## 4. 验证结果

- `dotnet build War3Frame/War3Frame.csproj`：**0 error**（185 个存量 nullable warning，与本次无关）。
- `dotnet build Projects/test/test.csproj`：**0 error 0 warning**。
- `dotnet run --project Projects/Regression`：**Executed 31; passed 31; failed 0**，含 11 个既有 `Projects/test` 场景（`ControlStateValidationScenario` 仍 PASS）。
- 新增 `runtime/pause-synthesis` 覆盖：Stun 合成一次进入 + Pause 位不溢出 + Stun/CrackFly 无自身 native 请求 + Stun 事件仍广播；同帧多来源只发一次；Stun 解除而 CrackFly 仍在时保持暂停；全部解除恰好一次恢复；免疫压制不暂停、免疫移除恢复；纯 Pause 不受 `StunImmunity` 影响独立进出。

## 5. 全局影响分析

| 区域 | 影响 |
|---|---|
| `War3Frame/` | 控制检测系统 + 快照位宽 + 新属性注册（本次范围） |
| `War3Frame.Generator/` | 不受影响：无生成器契约变化 |
| `FrameBuild/`、`CSharpWar3Frame/` | 不受影响：构建链与 CLI 不涉及 |
| `Projects/` | 仅 `Regression` 新增用例；`test` 无需改动（既有场景全绿） |

## 6. 剩余风险与未执行项

- **未执行**：War3 客户端 `PauseUnit` 真实表现验证。缺可自动执行的客户端验证协议，属非阻塞集成验证；本轮仅到 SDK 构建 + 回归宿主层。运行前需确认 Stun/CrackFly 不再有任何 native 请求产生点（已静态核对 + 断言覆盖）。
- **暂缓（用户决定）**：眩晕特效监听示例确认（T4-2）。
- `bits` 字段类型变化属公共结构体契约变化（`byte`→`ushort`），仓库内读取点仅 `ControlStateTransitionSystem` 一处，已同步；外部消费方若直接写 `bits` 需按 12 位重新按位。
