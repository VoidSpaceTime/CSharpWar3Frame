# 任务：fix-runtime-correctness

## 0. 提案

- [x] 0.1 `openspec new change fix-runtime-correctness`
- [x] 0.2 写 `proposal.md` / `design.md` / `tasks.md` / spec delta
- [x] 0.3 `openspec validate fix-runtime-correctness` 通过
- [x] 0.4 用户批准范围与决策 D-1 / D-2 / D-3（2026-10-23：A–F 全部，三项均取推荐值）

## 1. 回归先行（先红）

- [x] 1.1 新建 `Projects/Regression/RuntimeCorrectnessRegression.cs` 并在 `Program.cs` 注册
- [x] 1.2 写入 A–F 各项断言，在未修复代码上确认失败项与预期一致

## 2. A 施法 / 移动（`Src/Systems/Unit/MoveSystem.cs`、`Src/Systems/Ability/CastingSystem.cs`、`Src/Components/MoveCommand.cs`、`Src/Helpers/UnitHelper.cs`）

- [x] 2.1 A1 `MoveSystem`：旧 token 发 `Overridden`，只移除陈旧 `MoveExecutionState`，新命令同轮继续；死亡单位按 `Interrupted`
- [x] 2.2 A2 `MoveContinuation.commandToken`；`CastRequestSystem`、`UnitHelper.MoveToTask` 写入；`MoveToCastSystem`、`MoveToTaskSystem` 丢弃陈旧 outcome（token 0 不校验）
- [x] 2.3 A3 `CastMovement.Clear`；`CancelCastMovement` / Arrived / 失败 / 孤儿分支统一走它
- [x] 2.4 A4 `CastRequestSystem`：校验通过后按阶段结束已有施法（design §2 A4 表）
- [x] 2.5 A5 `UnitHelper.IsDead`（internal）；`CastRequestSystem`、`CastingSystem`、`ChannelingSystem`、`MoveToCastSystem` 接入
- [x] 2.6 A6 `InterruptCast`（已提交 → `EnterCooldownOrReady`）、`InterruptChannel`（一律 `EnterCooldownOrReady`）
- [x] 2.7 A7 `StartCasting` 使用解析后坐标（`CastingSystem.cs:50-56` → `113-114`）
- [x] 2.8 A8 `AbilitySlotHelper.RemoveAbilityFromSlot`、`AbilityRemoveWorkflowSystem.RemoveAbility` 先 `ItemCompanionCastCleanup.Cleanup`；施法/引导系统对已删除技能防御

## 3. B tick 中断异常

- [x] 3.1 B1 `AbilityEffectSystems.cs:572-583` 先收集再 `Destroy`，查询用 `owner.Store`
- [x] 3.2 B2 `AttributeHelper.GetAttr` / `GetAllAttrs` 空实体安全
- [x] 3.3 B3 `UISyncResolveSystem` 每条请求 `try/finally` 删除
- [x] 3.4 B4 `EffectNativeSystem.CreateNativeEffect` 空附着目标守卫

## 4. C 死亡 / 生命

- [x] 4.1 C1 `DamageResolveSystem` 无 `Health` 属性不判死（`AbilityEffectSystems.cs:1125,1150`）
- [x] 4.2 C2 `HealthSystem`、`ManaSystem`、`HealResolveSystem` 跳过死亡单位
- [x] 4.3 C3 `TimerTaskSystem`（RebornWait）、`UnitHelper.AcquirePooled` 回满 ECS `Health.current`
- [x] 4.4 C4 `AuraSystem` 排除死亡目标；owner 死亡时回收其 Buff
- [x] 4.5 C5 `AttackSimulationSystem.FindNextChainTarget` 加 `Alive`
- [x] 4.6 C6 `AttrCalculationSystem` 重算后 `current` 向下夹紧

## 5. D Buff / 属性贡献

- [x] 5.1 D1 `BuffHelper.HandleExistingBuff` 永久 Buff 先处理叠层（`BuffHelper.cs:391-395`）
- [x] 5.2 D2 `ApplyBuff` 按 (buffId, 目标属性) 查重；`RemoveBuff` 删除全部同名实例
- [x] 5.3 D3 两条技能移除路径直接回收技能为来源的修改器

## 6. E 效果 / 弹道（`Src/Systems/Ability/AbilityEffectSystems.cs`、`Src/Helpers/AbilityEffectHelper.cs`、`Src/Components/Ability/AbilityEffect.cs`）

- [x] 6.1 E1 `ProjectileRuntimeState` 起点字段；`UpdateCurve` 锁定起点（`:269-271`）
- [x] 6.2 E2 `EffectSettlementHelper.CanSettle` 拦截 `LineSearchData`（`:1531-1537`）
- [x] 6.3 E3 `CreateChildEffect` 目标坐标取命中目标位置（`AbilityEffectHelper.cs:110-123`）
- [x] 6.4 E4 `AbilityEffectSystems.cs` 内 `Game.Store` 改为实体所属 Store
- [x] 6.5 E5 `GroundAreaPeriodicDamageSystem` 保留余数（`:1386`）；`ChannelingSystem` 末跳按总时长判定

## 7. F 维护

- [x] 7.1 F1 `GroupHelper` 去掉静态 `_tempBuffer`
- [x] 7.2 F2 删除 `War3NativeBootstrap.cs:45` 调试输出
- [x] 7.3 F3 `UIInput.Pump` 无根时清空队列
- [x] 7.4 F4 `ItemPickupWorkflowSystem` 同批去重
- [x] 7.5 F5 `IntegrationRegression` 超时 10 分钟

## 8. 验证与收尾

- [x] 8.1 `dotnet build War3Frame/War3Frame.csproj -c Debug` 0 error、警告不增
- [x] 8.2 `dotnet run --project Projects/Regression -c Debug` 全部通过
- [x] 8.3 R2 视角 1 技术准确性、视角 2 QA 回归，各有证据与 verdict
- [x] 8.4 写 `summary.md`（含未执行客户端验证的原因与剩余风险、E5 数值变化）
- [x] 8.5 清理临时探针目录
- [x] 8.6 归档前等待用户确认（2026-11-01 用户确认：提交后归档）
