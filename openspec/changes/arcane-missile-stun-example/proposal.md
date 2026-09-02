# 提案：奥术飞弹示例增加命中眩晕效果

- Change ID: `arcane-missile-stun-example`
- 提案等级: `light`
- 状态: `已批准`
- 目标一句话: 给 `arcane_missile` 示例技能加命中眩晕 3 秒，作为"单体追踪弹道 + 命中后施加控制 Buff"的示例。
- 请求来源: 用户查看奥术飞弹示例时要求展示眩晕控制效果。

## 背景与目标

当前 `arcane_missile` 只演示"单体追踪弹道命中后结算伤害"。用户希望示例技能同时展示命中后附加控制效果，并验证 `AttributeHelper.Stun` + `Buff` 步骤 + `ControlStateTransitionSystem` 控制状态链路的可用性。

## 影响范围

- 模块：`Projects/test`（示例模板）。
- 文件：`Projects/test/Scripts/Template/Ability.cs`（`ArcaneMissileTemplate` 增加 `.Buff` 步骤并更新描述）；本 change 的 `proposal.md`。
- 不受影响区域：`War3Frame/` 运行时框架零改动——Buff 步骤、`Stun` 属性、控制状态系统均为已存在能力；`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/demo` 均不受影响。

## 方案摘要

在 `ArcaneMissileTemplate` 的 `.OnEffect` 链 `.Damage(...)` 后追加：

```
.Buff("arcane_missile_stun", AbilityValue.Constant(3f),
      AttributeHelper.Stun, ModifyType.Flat,
      AbilityValue.Constant(1f), BuffRefreshBehavior.RefreshDuration)
```

- Damage 与 Buff 同挂一条 effect 链，结算系统 `CanSettle` 等待弹道到达后才执行 → 眩晕是命中后施加到追踪目标。
- Stun Flat +1（>0 即激活控制），3 秒到期自动移除；Buff 到期移除贡献后 `ControlStateTransitionSystem` 检测 0 跳变自动解除原生暂停。
- 与 `frost_nova_root`（Root 定身）同构，仅属性 ID 换成 `AttributeHelper.Stun`。

## 风险与回滚

- 风险：无运行时改动，仅示例模板。若测试场景驱动该技能，只影响 test 项目演示内容，不影响契约。
- 回滚：移除新增的 `.Buff(...)` 链与描述改动即可。

## 验收标准

- `ArcaneMissileTemplate` 命中目标后施加 3 秒眩晕；描述文案含"眩晕 3 秒"。
- 无 `War3Frame/` 框架改动；test 项目可正常编译。
- 本 change 目录存在 `summary.md`。
