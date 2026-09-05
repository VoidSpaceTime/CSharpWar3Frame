# Tasks：暂停（Pause）状态合成

- **变更 ID**：synthesize-pause-from-controls
- **等级**：light

## T0. 前置：快照位宽修复（Blocker）

- [ ] `ControlState.cs`：`ControlStateSnapshot.bits` 从 `byte` 扩到 `ushort`，`BitOf` 返回 `ushort`。当前 `ControlType` 有 12 个成员（序号 0-11），`(byte)(1<<10)` 溢出截断为 0，Pause/Invisible/Sorcery 位恒失效。改后序号 0-11 全部可表示。

## T1. 属性注册与免疫映射

- [x] `AttributeHelper.cs`：属性命名统一已完成（`Disarm`→`NoAttack`、`Knockback`→`CrackFly`，Register 字符串同步）。
- [ ] `AttributeHelper.cs`：注册 `Pause = Register("Pause")`。**不注册 `PauseImmunity`**——暂停是最底层硬控制，不可免疫。
- [ ] 纯 Pause 在合成判定中不经过免疫压制：直接读 `AttrValue.finalValue`（Pause 无免疫条目时 `GetEffectiveValue` 也会原样返回，但语义上用直读更明确）。
- [ ] 明确枚举语义：`ControlType.Pause` 保持为"暂停"；`ControlType.Stun` / `ControlType.CrackFly` 语义说明更新为"暂停 + 标记 + 表现"。
- [ ] `ControlHelper.GetImmunityAttrId` if-链改读 `ControlAttrs` 映射表（消双份维护）。注意 ControlAttrs 在 ControlStateTransitionSystem 内——需把表提升为共享权威（放 ControlHelper 或新静态类）。

## T2. 检测系统：Pause 合成判定

- [x] `ControlStateTransitionSystem.cs`：控制属性表已从"数组下标=枚举序号"改为显式 `ControlAttrEntry(AttrId, ImmunityAttrId, ControlType)` 映射记录（随命名统一一并落地）。
- [ ] 循环内：Stun / CrackFly 跳变时**保留 `ControlStateChangedEvent`，移除 `ControlStateNativeRequest`**（它们的 native 动作由 Pause 合成承担）。
- [ ] **Pause 不进 `ControlAttrs` 主表**：在主循环外单独合成判定（避免与主循环重复发请求）：
  ```csharp
  var pauseActive = ControlHelper.GetEffectiveValue(unit, AttributeHelper.Stun) > 0
                 || ControlHelper.GetEffectiveValue(unit, AttributeHelper.CrackFly) > 0
                 || AttributeHelper.GetFinalValue(unit, AttributeHelper.Pause) > 0;  // 纯 Pause 不经免疫
  ```
- [ ] Pause 快照位与合成结果对比：跳变时发一次 `ControlStateNativeRequest(Pause, entered)`；同帧内多控制变化只合成一次，避免抖动。
- [ ] Pause 合成使用"当前帧 effective 值"与"上一帧 Pause 快照"对比，不累积两帧误差。
- [ ] release/清理路径确认：单位属性整体移除时，Pause 位也需被正确释放（`toRelease` 遍历新增 Pause 位）。

## T3. Native 层确认

- [ ] `UnitControlNativeSystem.cs`：确认 `case ControlType.Pause → JassApi.PauseUnit` 分支存在且正确（无需新增 Stun/CrackFly case——它们不再发 native 请求）。
- [ ] 清理 Stun/CrackFly 若残留的 native 请求产生路径（无）。

## T4. 验证场景 / 模板

- [ ] （可选）`Projects/test`：若存在控制验证场景，补 Stun→PauseUnit 断言；否则编译级验证即可。
- [ ] 眩晕特效示例监听 `ControlStateChangedEvent(Stun, entered=true)` 的路径确认（不属于本 change 实现，仅确认不被破坏）。

## T5. 构建与验证

- [ ] `dotnet build War3Frame/War3Frame.csproj` 0 error（命名统一后已通过）。
- [ ] `dotnet build Projects/test/test.csproj` 0 error（命名统一后已通过）。
- [ ] 静态核对：Stun/CrackFly 不再发各自 native 请求；Pause 只在合成跳变时发一次；免疫路径不穿透。
- [ ] War3 客户端真实暂停表现验证（非阻塞，记录未执行原因）。
