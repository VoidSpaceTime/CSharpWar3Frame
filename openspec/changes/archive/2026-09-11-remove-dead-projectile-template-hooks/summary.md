# Summary: remove-dead-projectile-template-hooks

**状态**: 已实施
**实际复盘强度**: R1（技术准确性复核已完成，见下「验证」）

## 改动范围

删除了两处零消费者代码：

- `AbilityTemplateBase`（零子类抽象基类）及 `ProjectileTravelDecision` 枚举
  —— `AbilityTemplateAttribute.cs` 现与 `UnitTemplateAttribute.cs` / `ItemTemplateAttribute.cs`
  结构对称（纯接口 + Attribute + 静态注册表），无基类。
- `ProjectileHookBridge` 静态类及其 3 处调用点
  —— 其 `TryResolveTemplate` 的 `raw as AbilityTemplateBase` 转型因无任何模板继承基类而恒为 false，
  三个 hook 每帧被调用但全部提前返回。

连带简化：`ProjectileSystem.OnUpdate` 中 `decision` 局部变量消失，
`if (arrived && decision != SuppressArrivalThisTick)` 简化为 `if (arrived)`。

按提案「预留」章节，**过期管线 6 处代码全部保留并加了休眠标注**（防止下轮复盘误删）：
`_expireRequests` 字段、`toExpire` 结算 foreach、`ApplyRequests` expire 半边、
`ProjectileExpireRequest` / `EffectExpired` tag、`ExpireRequested` / `Expired` 枚举值。
顺带修正 `ProjectileTrajectoryType.Custom` 的注释（「保留给模板 hook」已失效）。

## 验证

- `War3Frame` 构建：**0 错误**，174 warning（全部为既有 `KKApi.cs` CS8625 / `SyncHelper.cs` CS8618，与本次改动无关）
- `Projects/test` 构建：**0 错误 0 warning**，17 个技能模板无需任何修改
- `grep AbilityTemplateBase|ProjectileHookBridge|ProjectileTravelDecision` 全仓零命中
- `grep decision` 在 `AbilityEffectSystems.cs` 零命中
- 6 处休眠管线目视确认在位且带注释
- 主流程 `PendingStart → InFlight → ArriveRequested → Arrived` 未受影响：
  hook 调用点均为独立语句，删除不改变阶段推进或到达结算顺序
- 文档核查：`War3Frame/Docs/` 与 `AGENTS.md` 均无 hook 机制描述，无需更新

## 后续事项

超距 / 超时能力需独立 `full` 级提案，实施时须解决三个已核实的前置问题：

1. `maxDistance` 当前走**到达**分支（`UpdateLinear:180` 返回 `arrived = true`），
   弹道飞到射程尽头会照常触发落点伤害；超距应改走过期，属行为变更。
2. `maxDistance` **仅 `Linear` 轨迹读取**，其余 5 种轨迹忽略它；
   `Bezier` / `Parabolic` / `Sinusoidal` 走 `UpdateCurve` 按 `normalizedProgress` 推进，
   无距离累加，需补 `traveled`。
3. 超时无数据源：`ProjectileRuntimeState.elapsedTime`（`AbilityEffect.cs:319`）
   声明后从未被任何代码递增，需新增 `maxLifetime` 并在推进时累加 `Tick.deltaTime`。

该提案会改 `ProjectileData`、`ProjectileEffectStepSpec` 与
`EffectChainBuilder.Projectile` 两个重载签名，属公共 authoring 契约变更。

## 遗留风险

过期管线暂时无写入方，若超距提案长期不落地会积累成真死代码。
缓解措施为代码注释显式标注 + 本文档留痕。
