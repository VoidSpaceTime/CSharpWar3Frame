# 提案：控制状态叠加态（superposition，方案 B）

**状态**：已批准（用户 2026-08-31 批准实施，方案 B）→ 已实施（2026-08-31，见 summary.md）
**等级**：light
**提案日期**：2026-08-31（首次）；2026-08-31 修订：用户确认采用**方案 B**（复用现有属性系统作为计数真相，取消独立计数组件）
**请求来源**：lik/xlik 框架对标分析（`common/superposition.lua` 计数式状态叠加）；用户已确认按此方向提案。

---

## 背景与目标

当前控制效果已通过属性系统表达：`Stun`/`Silence`/`Disarm`/`Root`/`Knockback` 属性（`AttributeHelper` 注册），`finalValue > 0` 表示生效，免疫由对应 `*Immunity` 属性压制（`ControlHelper.GetEffectiveValue`）。

**关键现状：多来源叠加已天然成立**——`ModifyValue` 修改器（Flat 型）由多个来源（Buff/技能/物品）各自挂载到同一控制属性，`AttrCalculationSystem` 聚合 `finalValue`，全部来源移除才归零。缺的只是三件事：

1. **0↔正 临界检测**：`finalValue`（经免疫压制后的有效值）从 0 变正 / 正变 0 的跳变目前无人感知。
2. **领域事件**：跳变时创建 `ControlStateChangedEvent`（独立事实实体，对齐 `DamageEvent`），供业务监听。
3. **Native 副作用**：跳变时生成 `ControlStateNativeRequest`，由 Native 系统执行 `PauseUnit` 等能力开关（进入时开启、退出时恢复）。

## 非目标

- **不新增独立计数组件**（StunCount 等）——避免与属性系统形成双真相。
- 不改变 `ControlHelper` 查询 API 语义（有效值 > 0 且未免疫）。
- 不改变施加控制的现有路径（Buff/ModifyHelper 挂 Flat 修改器继续有效）。
- 免疫不把计数清零（保持"压有效值"语义，检测基于有效值）。

## 影响范围

- 新增 `War3Frame/Src/Components/ControlState.cs`：
  - `ControlStateSnapshot`（IComponent，挂单位实体）：位域记录 5 种控制当前是否生效（0/1），供跳变对比。
  - `ControlStateChangedEvent`（独立事件实体）：`unit` / `controlType` / `entered`。
  - `ControlStateNativeRequest`（独立请求实体）：`unit` / `controlType` / `entered`，Native 消费后删除。**有意偏离 AGENTS「NativeRequest 挂主体」惯例**：消费端按独立实体查询（`QuerySystem<ControlStateNativeRequest>`），本地无句柄场景可直接删除请求实体；实施时避免被 unify-native-request 规则误改。
- 新增系统：
  - `ControlStateTransitionSystem`（`[SystemRegister(SystemKind.Interval, 46)]`，**非 Immediate**）：order 46 在 `AttrCalculationSystem`(45) 之后；无 `ITimedSystem`，周期跟随 Root 默认间隔（约 0.03125s）。对比快照与当前有效值（经免疫压制），跳变 → 更新快照 → 创建事件 + 请求实体。
  - `UnitControlNativeSystem`（Native 层，Immediate）：消费 `ControlStateNativeRequest`，entered → 执行对应能力开关（如 `PauseUnit`），exited → 恢复；完成后删除请求实体。具体能力映射表实施时按可用 API 确定。
- 可选：`ControlHelper.AddControl/RemoveControl` 便利包装（内部走 ModifyHelper，非必需——Buff/技能现有路径已覆盖）。
- 不受影响区域：
  - `War3Frame.Generator/`：新系统用现有 `[SystemRegister]`，无生成器契约变化。
  - `FrameBuild/`、`CSharpWar3Frame/`：构建链与 CLI 不涉及。
  - `Projects/`：查询签名与施加路径不变；**新增** `ControlStateValidationScenario` 验证场景并接入 `Projects/test`（验证为本变更交付物）。

## 方案摘要

```
Buff / 技能 / 物品 → ModifyValue(Flat) 挂到控制属性（现有路径，不变）
        ↓ AttrCalculationSystem 聚合 finalValue（天然多来源叠加）
ControlStateTransitionSystem：对比 ControlStateSnapshot 与有效值（含免疫压制）
  → 0↔正 跳变：更新快照 + 创建 ControlStateChangedEvent（业务监听）+ ControlStateNativeRequest
        ↓
UnitControlNativeSystem（Native 层）：entered → PauseUnit 等能力开关；exited → 恢复；删请求
查询：ControlHelper.IsSilenced 等语义不变（有效值 > 0 且未免疫）
```

## 风险与回滚

- 风险：
  1. **跳变检测遗漏**：若属性重算与检测系统同帧乱序，可能漏检；检测系统 order 必须在 `AttrCalculationSystem`(45) 之后，且与 Buff 结算系统（40-41）顺序核对。
  2. **快照一致性**：单位销毁时快照组件随实体自动清理（无泄漏）；单位复活/重建时快照初始化需在检测前完成。
  3. **Native 能力映射**：`PauseUnit` 与移动/施法工作流（`CastState`/`MoveCommand`）存在交互，具体能力选择实施时验证（如眩晕用暂停而非禁用移动，避免与 Root 冲突）。
  4. 免疫压制的有效值变化（免疫 Buff 增减）也会引起跳变——符合预期语义（免疫生效即视为解除），验收时覆盖该路径。
- 回滚：全部为新增文件（组件/系统），移除即可；不动现有属性与查询逻辑，成本低。

## 验收标准

1. 同一单位叠加 2 个眩晕来源（2 个 Flat 修改器），移除 1 个后仍处于眩晕（`finalValue` 仍 > 0）；全部移除才恢复。
2. 眩晕进入/退出各产生**恰好一次** `ControlStateChangedEvent`（中间 +1/-1 静默），`entered` 方向正确。
3. `ControlStateNativeRequest` 由 Native 系统消费执行副作用：Stun → `PauseUnit`（进入开启、退出恢复）；Silence/Disarm/Root/Knockback 首期发请求后删除（原生能力映射另案，事件仍照发供业务监听）；无重复执行。
4. 免疫生效时按"有效值"判定：免疫 Buff 挂载视为退出（发 exited），免疫移除且仍有效源时视为进入。
5. 沉默/缴械/定身/击飞同样按来源叠加，互不覆盖。
6. 现有 `ControlHelper` 查询 API 语义不变。
7. `dotnet build War3Frame/War3Frame.csproj` 0 错误。

## 后续事项（不阻塞本变更）

- **事件清理**：`ControlStateChangedEvent` 与 `DamageEvent` 等独立事件实体当前无统一清理系统（全仓缺口）；待 `EventCleanupSystem` 基础提案落地后一并清理。
- **Silence/Disarm/Root/Knockback Native 映射**：需要可用 API（物编虚拟技能 / KK 扩展）时单独提案。
- **真实客户端验证**：`PauseUnit` 副作用未在 War3 测试客户端验证（本地无句柄环境），建议后续跑一次眩晕/解除流程。

## 分级判定

- 影响范围：`War3Frame` 内 Control 单领域，少量新增文件。
- 风险等级：低到中（新增检测/事件/Native 消费，不改现有契约与施加路径）。
- 可逆性：高（新增而非替换）。
- 是否跨项目：否。
- 是否改公共契约：否（新增类型与系统；`ControlHelper` 查询签名不变）。
- 结论：`light` 合理。默认复盘 `R0 Direct`；实施时涉及 Native 能力映射与工作流交互，按 `R1 Focused` 执行（Oracle 优先做技术准确性复核）。