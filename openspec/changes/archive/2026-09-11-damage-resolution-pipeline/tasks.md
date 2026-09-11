# 任务：战斗结算管线（伤害管线 + 无敌拦截伤害与控制）

> change：`damage-resolution-pipeline`（full）。所有任务在 proposal 批准后按序执行；验证在每步可用时尽早跑，最终统一走 proposal §7 验证计划。

## Phase 1：War3 同步 RNG 薄封装（最小，最先落地）

- [ ] T1.1 `War3Frame/Src/Helpers/War3Random.cs`：转发 `JassApi.GetRandomReal(0,1)` / `GetRandomInt`，无状态；代码注释登记 Native 分层例外身份。
- [ ] 验证：编译通过；调用返回 [0,1)。

## Phase 2：数据契约扩展

- [ ] T2.1 `Components/Damage.cs`：扩展 `DamageBase`（新增 `calcKey`，默认 0）、`DamageEvent`（新增 `isCrit`/`mitigatedAmount`/`isImmune`，默认 false/0）。
- [ ] T2.2 新增 `DamageContext` struct（design/spec §2 字段）。
- [ ] T2.3 核对所有 `DamageEvent` 构造点（AbilityEffectSystems、AttackSimulationSystem、TriggerActionRegistry 等）编译兼容。

## Phase 3：免伤公式注册表

- [ ] T3.1 新增 `ArmorFormulaRegistry`：物理槽 + 魔法槽 Register/SetCurrent/GetCurrent，默认都指向 War3。
- [ ] T3.2 实现 War3 公式（`armor=0` 时零减免；负护甲近似实施时按 War3 1.27 核对）。
- [ ] T3.3 实现 Dota2 式公式（系数 0.052 类，独立实现）。
- [ ] 验证：两公式在 armor=0 输出 0；armor>0 递减；armor<0 增伤。

## Phase 4：伤害计算器注册表

- [ ] T4.1 新增 `DamageCalculatorRegistry`（delegate 签名 `ref DamageContext`）。
- [ ] T4.2 内置默认计算器：暴击判定（`War3Random`，Melee/Ranged 可暴击、Skill 默认禁暴由 calcKey 或计算器可配）→ Physical 查物理减免 / Magical 查魔法减免 / Real 跳过。

## Phase 5：伤害结算系统重构

- [ ] T5.1 `DamageResolveSystem` 重构接入管线：PreCheck → 计算器 → 扣血 → 事件 → 死亡。
- [ ] T5.2 无敌伤害拦截：`GetFinalValue(target, Invulnerable) > 0` → finalDamage=0、isImmune=true、不 KillUnit、仍发 Event（含 TriggerEventMarker）。
- [ ] T5.3 事件扩展字段回填。
- [ ] 验证：默认无护甲/暴击/非无敌行为与重构前一致。

## Phase 6：无敌拦截控制（读取侧压制）

- [ ] T6.1 `ControlHelper.GetEffectiveValue`：入口最前面加 `Invulnerable` finalValue>0 → 直接返回 0（对 Stun/Silence/NoAttack/Root/CrackFly 五类通用压制）。
- [ ] T6.2 确认不误伤非控制查询路径；不新增施加入口拦截；不驱散已有 Buff。
- [ ] 验证：无敌目标 GetEffectiveValue(Stun) 返回 0；无敌结束 buff 未到期恢复生效。

## Phase 7：结算钩子扩展位

- [ ] T7.1 新增 `DamagePostProcessRegistry`（只留注册位，无内置实现），在 Event 发出后调用。

## Phase 8：测试与回归

- [ ] T8.1 `Projects/test` 新增结算管线验证场景（spec §3 验收矩阵全项）。
- [ ] T8.2 `dotnet build War3Frame/War3Frame.csproj` 0 error。
- [ ] T8.3 `dotnet build Projects/test/test.csproj` 0 error。
- [ ] T8.4 全仓构建（FrameBuild 或等价）确认无死引用。
- [ ] T8.5 完成 `summary.md`。
