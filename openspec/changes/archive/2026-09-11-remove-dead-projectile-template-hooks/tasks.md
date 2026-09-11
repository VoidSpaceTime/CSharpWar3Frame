# Tasks: remove-dead-projectile-template-hooks

## 1. 删除空转 hook 分发层

- [x] 删除 `AbilityEffectSystems.cs:1418-1457` 的 `ProjectileHookBridge` 整个静态类
- [x] 删除 `AbilityEffectSystems.cs:45` 的 `DispatchStartHooks` 调用
- [x] 删除 `AbilityEffectSystems.cs:49` 的 `DispatchTravelHooks` 调用及 `decision` 局部变量
- [x] 删除 `AbilityEffectSystems.cs:50-55` 的 `RequestExpire` 分支
- [x] 将 `AbilityEffectSystems.cs:72` 的 `if (arrived && decision != ProjectileTravelDecision.SuppressArrivalThisTick)`
      简化为 `if (arrived)`
- [x] 删除 `AbilityEffectSystems.cs:320` 的 `DispatchArriveHooks` 调用

## 2. 删除零子类抽象基类

- [x] 删除 `AbilityTemplateAttribute.cs:20-51` 的 `AbilityTemplateBase`
- [x] 删除 `AbilityTemplateAttribute.cs:10-18` 的 `ProjectileTravelDecision` 枚举
- [x] 确认 `IAbilityTemplate` 保持单方法纯接口不变

## 3. 标注休眠管线（防止下轮复盘误删）

- [x] `AbilityEffectSystems.cs:21` `_expireRequests` 字段加注释：为超距/超时能力保留
- [x] `AbilityEffectSystems.cs:328` `toExpire` foreach 上方加同类注释
- [x] `AbilityEffect.cs:297-298` `ExpireRequested` / `Expired` 枚举值加注释
- [x] `AbilityEffect.cs:354` `ProjectileExpireRequest` tag 加注释
- [x] `AbilityEffect.cs:59` `EffectExpired` tag 加注释
- [x] `AbilityEffect.cs:301` 修正 `ProjectileTrajectoryType.Custom` 注释
      （「保留给模板 hook」已失效，改为「保留给后续扩展」）

## 4. 文档

- [x] 核查 `War3Frame/Docs/ProjectileSystem使用说明.md` 是否描述了 hook 机制，有则移除
- [x] 核查 `AGENTS.md` 是否引用 `AbilityTemplateBase` 作为示例

## 5. 验证

- [x] 构建 `War3Frame`（DLL 被锁时用 `-p:BuildProjectReferences=false`）
- [x] 构建 `Projects/test`，确认无需改动模板文件
- [x] `grep -rn "AbilityTemplateBase\|ProjectileHookBridge\|ProjectileTravelDecision"` 零命中
- [x] 目视确认 `ProjectileSystem.OnUpdate` 中过期管线 6 处代码仍在且带注释
- [x] R1 技术准确性复核：确认删除未破坏 `PendingStart → InFlight → ArriveRequested → Arrived` 主流程

## 6. 收尾

- [x] 写 `summary.md`（light 级：一段话覆盖范围、验证、后续事项）
- [x] 将状态改为 `已实施`
