# Tasks：暂停（Pause）状态合成

- **变更 ID**：synthesize-pause-from-controls
- **等级**：light
- **实施范围**：用户 2026-09-17 指定 T0 + T1-2 + T2-2~5；追加指定 T1-4 / T1-5；其余项标注暂缓原因。

## T0. 前置：快照位宽修复（Blocker）

- [x] `ControlState.cs`：`ControlStateSnapshot.bits` 从 `byte` 扩到 `ushort`，`BitOf` 返回 `ushort`。当前 `ControlType` 有 12 个成员（序号 0-11），`(byte)(1<<10)` 溢出截断为 0，Pause/Invisible/Sorcery 位恒失效。改后序号 0-11 全部可表示。

## T1. 属性注册与免疫映射

- [x] `AttributeHelper.cs`：属性命名统一已完成（`Disarm`→`NoAttack`、`Knockback`→`CrackFly`，Register 字符串同步）。
- [x] `AttributeHelper.cs`：注册 `Pause = Register("Pause")`。**不注册 `PauseImmunity`**——暂停是最底层硬控制，不可免疫。
- [x] 纯 Pause 在合成判定中不经过免疫压制：直接读 `AttrValue.finalValue`（Pause 无免疫条目时 `GetEffectiveValue` 也会原样返回，但语义上用直读更明确）。
- [x] 明确枚举语义：`ControlType.Pause` 为"最底层硬控制、由合成驱动"；`ControlType.Stun` / `ControlType.CrackFly` 更新为"暂停（由 Pause 合成驱动原生）+ 标记 + 表现"。
- [x] `ControlHelper.GetImmunityAttrId` if-链改读共享 `ControlAttrs` 映射表（消双份维护）：表提升到 `ControlHelper.ControlAttrs`（`public readonly record struct ControlAttrEntry`），检测系统改为引用同一权威表，不再各持一份。

## T2. 检测系统：Pause 合成判定

- [x] `ControlStateTransitionSystem.cs`：控制属性表已从"数组下标=枚举序号"改为显式 `ControlAttrEntry(AttrId, ImmunityAttrId, ControlType)` 映射记录（随命名统一一并落地）。
- [x] 循环内：Stun / CrackFly 跳变时**保留 `ControlStateChangedEvent`，移除 `ControlStateNativeRequest`**（它们的 native 动作由 Pause 合成承担）。
- [x] **Pause 不进 `ControlAttrs` 主表**：在主循环外单独合成判定（避免与主循环重复发请求）：
  ```csharp
  var pauseActive = ControlHelper.GetEffectiveValue(unit, AttributeHelper.Stun) > 0
                 || ControlHelper.GetEffectiveValue(unit, AttributeHelper.CrackFly) > 0
                 || AttributeHelper.GetFinalValue(unit, AttributeHelper.Pause) > 0;  // 纯 Pause 不经免疫
  ```
- [x] Pause 快照位与合成结果对比：跳变时发一次 `ControlStateNativeRequest(Pause, entered)`；同帧内多控制变化只合成一次，避免抖动。
- [x] Pause 合成使用"当前帧 effective 值"与"上一帧 Pause 快照"对比，不累积两帧误差。
- [x] release/清理路径确认：单位属性整体移除时，Pause 位也需被正确释放（`toRelease` 遍历新增 Pause 位）。
- [x] 附带补齐：`IsControlOrImmunity` 纳入纯 `Pause` 属性，否则"只有 Pause 属性、无眩晕/击飞"的单位不会被收集（属 T2-3 的必要前置，实施中发现）。

## T3. Native 层确认

- [x] `UnitControlNativeSystem.cs`：确认 `case ControlType.Pause → JassApi.PauseUnit` 分支存在且正确（无需新增 Stun/CrackFly case——它们不再发 native 请求）。
- [x] 清理 Stun/CrackFly 若残留的 native 请求产生路径（无；`ControlStateNativeRequest(Stun/CrackFly)` 已无任何产生点）。

## T4. 验证场景 / 模板

- [x] （可选）验证改用 `Projects/Regression` 的 `runtime/pause-synthesis` 用例覆盖 Stun/CrackFly/纯 Pause → Pause 请求链路（`Projects/test` 场景为 War3 端运行，本地不可执行）。
- [ ] **暂缓（用户决定）** 眩晕特效示例监听 `ControlStateChangedEvent(Stun, entered=true)` 的路径确认（不属于本 change 实现，仅确认不被破坏）。

## T5. 构建与验证

- [x] `dotnet build War3Frame/War3Frame.csproj` 0 error（185 个存量 nullable warning）。
- [x] `dotnet build Projects/test/test.csproj` 0 error 0 warning。
- [x] 静态核对：Stun/CrackFly 不再发各自 native 请求；Pause 只在合成跳变时发一次；免疫路径不穿透（均有 `pause-synthesis` 断言）。
- [x] `Projects/Regression` 回归宿主 31/31 通过（含 11 个既有 Projects/test 场景）。T1-4 / T1-5 追加后重跑仍 31/31。
- [ ] War3 客户端真实暂停表现验证（非阻塞；缺可自动执行的客户端验证协议，未执行，风险见 summary.md）。
