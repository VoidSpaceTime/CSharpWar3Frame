# Spec：战斗结算管线（伤害管线 + 无敌拦截伤害与控制）

> 本文件定义结算管线的 capability requirement，是实施与验收的**第一事实源**。若与 proposal/design 冲突，以本文件 requirement 为准。
> 对应 change：`damage-resolution-pipeline`（full）。范围：伤害结算 + 无敌（Invulnerable）拦截伤害与控制施加。不含吸血/护盾/反射的实际实现、不含治疗管线、不含 DamageType 可扩展 table。

## 1. Requirement 列表

### REQ-DMG-001：伤害走统一结算管线

- 所有伤害（技能/平A近战/Trigger 动作产生的 `DamageRequest`）在 `DamageResolveSystem` 统一结算，禁止在结算点之外直接 `ModifyCurrent(Health, 负值)` 表达伤害（治疗/消耗不受此限）。
- 管线顺序固定：PreCheck → Calculator → Apply → Event → 死亡判定 → PostProcess 钩子。

### REQ-DMG-002：默认行为与现状一致（无回归）

- 当 源 CritChance finalValue ≤ 0、目标 Armor/MagicResist finalValue = 0、目标非无敌 时，`finalDamage = max(0, 请求伤害)`，扣血结果与改造前 `DamageResolveSystem` 完全一致。

### REQ-DMG-003：死属性激活（减伤）

- Physical 伤害按目标 `Armor` finalValue 经当前物理减免公式减免；Magical 按 `MagicResist` 经魔法减免公式；Real 不查任何减免公式，满额。
- 减免公式输入 armor=0 时输出 0 减免。

### REQ-DMG-004：减免公式可注册可切换

- 提供 `ArmorFormulaRegistry`：物理槽与魔法槽可分别 `Register` 并 `SetCurrent`；至少内置 War3 公式与 Dota2 式公式两种，默认物理/魔法槽都指向 War3。
- 切换在运行时生效，不要求重启。

### REQ-DMG-005：暴击激活且随机走 War3 同步 RNG

- `DamageRequest` 进入结算时，若来源允许暴击且 `CritChance` finalValue > 0，用 **War3 同步 RNG**（`JassApi.GetRandomReal`，经薄封装 `War3Random`）roll 判暴；命中则 `preMitigation = base × max(1, CritMultiplier)`。
- 暴击结果写入 `DamageEvent.isCrit`。
- Melee/Ranged 默认允许暴击，Skill 来源默认禁暴（计算器按 `damageSrc`/`calcKey` 可配）。
- **禁止** `System.Random`/`DateTime`/`Guid` 等非确定性源参与管线随机。
- 实现备注：`War3Random.Next01Provider` 默认指向 War3 引擎同步 RNG；仅供本地无客户端验证场景临时替换为确定性 fake，生产/游戏内必须保持默认。

### REQ-DMG-006：无敌拦截伤害

- 判定：`AttributeHelper.GetFinalValue(target, AttributeHelper.Invulnerable) > 0`（叠加态，多来源叠加值>0 即生效）。
- 命中无敌时：不扣血、不触发死亡，`finalDamage=0`、`isImmune=true`，**仍发 `DamageEvent`**（含 `TriggerEventMarker`），供 UI/触发器感知。

### REQ-DMG-007：无敌拦截控制（读取侧压制）

- **机制**：在 `ControlHelper.GetEffectiveValue` 读取出口增加无敌压制——目标 `Invulnerable` finalValue>0 时，Stun/Silence/NoAttack/Root/CrackFly 五类控制全部返回 0。
- 与既有免疫机制同构（StunImmunity 压 Stun 等），无敌视为对五类控制的**通用免疫**；单点压制、无施加入口漏拦。
- **控制 Buff 仍落地**：无敌期间 Stun 等 Buff 正常创建/计时，但读取被压制为 0（不触发控制原生效果）。
- **结束恢复**：无敌结束后，若控制 Buff 未到期且对应免疫仍为 0，则重新生效（无需重新施加）。
- 非上述五类的效果（移动命令、环境效果等）不受此限。

### REQ-DMG-008：事件契约扩展兼容

- `DamageEvent` 新增 `isCrit`/`mitigatedAmount`/`isImmune`；既有字段（finalDamage/remainingHealth 等）语义不变。新增字段有默认值，存量读取方无感。

### REQ-DMG-009：死亡判定语义不变

- 仅当实际扣血后 `remaining <= 0` 且非免疫拦截时调用 `UnitHelper.KillUnit(target)`；`KillUnit` 只写生命周期（现状语义），原生移除由现有 Native 系统消费。

### REQ-DMG-010：计算器可注册

- 提供 `DamageCalculatorRegistry`，可按 `calcKey`（>0 显式）或默认按 `damageType` 分派计算器；默认计算器为内置（REQ-003/005 的暴击+减免内聚实现）。

### REQ-DMG-011：结算后钩子扩展位

- 提供 `DamagePostProcessRegistry`，在 `DamageEvent` 发出后按注册顺序调用；**本提案不注册任何内置实现**。
- 钩子约束：只允许读上下文并发新 Request（如 `HealRequest`/`DamageRequest`），禁止直接改 `finalDamage`、禁止调 War3 原生 API（分层约束）。

### REQ-DMG-012：分层与原生边界

- 本 change 只引入一处 War3 原生调用：`War3Random` 转发 `JassApi.GetRandomReal/GetRandomInt`（登记为 Native 分层例外：无句柄、无表现副作用、纯读引擎同步随机状态、无长期语义）。其余逻辑全部在 ECS/属性/Helper 层完成。
- `DamageResolveSystem` 属业务结算系统，除 `War3Random` 例外外不直接调 JassApi。

## 2. 数据契约定义

```csharp
public struct DamageBase
{
    public float damage;          // 入口基础伤害（管线不做减法，语义不变）
    public DamageType damageType; // Physical / Magical / Real
    public DamageSrc damageSrc;   // Melee / Ranged / Skill
    public Entity source;
    public Entity target;
    public int calcKey;           // 0 = 按 damageType 默认计算器；>0 = 显式计算器 key（新增，默认 0）
}

public struct DamageContext
{
    public Entity source;
    public Entity target;
    public DamageType damageType;
    public DamageSrc damageSrc;
    public float baseDamage;      // 进入计算器前的初始值
    public float preMitigation;   // 减免前（暴击后）伤害
    public float mitigation;      // 减免量
    public float finalDamage;     // 实际扣血值
    public float absorbed;        // 护盾吸收量（本提案恒 0，扩展位）
    public bool isCrit;
    public bool isImmune;
}

public struct DamageEvent : IComponent
{
    public DamageBase damage;     // 原始请求
    public float finalDamage;     // 实际扣血
    public float remainingHealth; // 结算后剩余 HP
    public Entity source;
    public Entity target;
    public bool isCrit;           // 新增：本次是否暴击
    public float mitigatedAmount; // 新增：暴击后 - 最终扣血（含护盾/减免总减少量）
    public bool isImmune;         // 新增：是否免疫拦截
}
```

## 3. 验收矩阵

| Requirement | 场景验证 | 期望 |
|---|---|---|
| REQ-DMG-002 | 无护甲/暴击/非无敌，DamageRequest 100 | remaining = maxHP-100；Event.finalDamage=100 |
| REQ-DMG-003/004 | 目标 Armor=10 物理 100（War3） | finalDamage < 100 且符合 War3 公式 |
| REQ-DMG-004 | 切 Dota2 公式 | finalDamage 按 Dota 系数变化 |
| REQ-DMG-003 | Magical 100 vs MagicResist；Physical 100 同目标 | 魔抗只减魔法；物理不减（armor=0） |
| REQ-DMG-003 | Real 100 vs Armor=100 | 满额 100 |
| REQ-DMG-005 | CritChance=1.0, Multiplier=2.0 | isCrit=true；finalDamage=200（无护甲） |
| REQ-DMG-005 | CritChance=0 | 永不 isCrit |
| REQ-DMG-006 | 目标 Invulnerable>0 | Event.isImmune=true、finalDamage=0、HP 不变、不 KillUnit |
| REQ-DMG-007 | 目标 Invulnerable>0 时施加 Stun | Stun Buff 落地但 `GetEffectiveValue` 返回 0，不触发眩晕 |
| REQ-DMG-007 | 目标带 Stun 后进入无敌 | 无敌期间 Stun 被压制为 0；无敌结束若 buff 未到期则恢复生效 |
| REQ-DMG-009 | 伤害致 HP≤0 | KillUnit 触发一次 |
| REQ-DMG-008 | 既有 TriggerValidationScenario | 编译通过、读 finalDamage 语义不变 |
