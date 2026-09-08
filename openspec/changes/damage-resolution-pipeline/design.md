# 设计：伤害结算管线（Damage Resolution Pipeline）

> 对应 change：`damage-resolution-pipeline`（full）。本文件给出可落地的架构形态、数据流、公式与扩展点。

- **目标一句话**（同步 proposal）：升级为带**无敌拦截（伤害+控制免疫）+ 可注册伤害计算器 + 钩子**的完整结算管线。

## 1. 目标形态（GAS 映射）

```
DamageRequest(GE) 
  → ① PreCheck（Invulnerable 无敌拦截） = GAS ApplicationRequirement / GE Tags 免疫
  → ② DamageCalculator(ExecCalc)  = 内聚公式：暴击 roll → 减免公式 → 输出最终值
  → ③ Apply（扣血 + 护盾吸收位）   = GAS Modifier 落地
  → ④ DamageEvent(PostExecute)    = GAS PostGameplayEffectExecute（吸血/死亡/伤害数字钩子）

无敌控制免疫（旁路）：
  Stun/Silence/NoAttack/Root/CrackFly 施加点 → 检查 target Invulnerable finalValue>0 → 忽略新施加
```

与 xlik 的"多步骤 Flow 链"关键区别：伤害是**每次同构的主流程**，把公式内聚进一个可注册 Calculator 比拆多条顺序步骤更贴 GAS 也更可控；免伤公式通过子注册表实现"War3 / Dota2 可切换"。

## 2. 数据契约

### 2.1 DamageBase（扩展）

```csharp
public struct DamageBase
{
    public float damage;          // 入口基础伤害（不变）
    public DamageType damageType; // Physical / Magical / Real
    public DamageSrc damageSrc;   // Melee / Ranged / Skill
    public Entity source;
    public Entity target;
    public int calcKey;           // 0 = 按 damageType 取默认计算器；>0 = 显式自定义计算器 key
}
```

> `DamageRequest.damage` 保留原语义：请求进入管线的初始伤害，不做减法。新增字段均有默认值，既有构造无感。

### 2.2 DamageContext（一次结算的流转上下文）

```csharp
public struct DamageContext
{
    public Entity source;
    public Entity target;
    public DamageType damageType;
    public DamageSrc damageSrc;
    public float baseDamage;      // 进入计算器前的初始值
    public float preMitigation;   // 减免前（暴击后）伤害
    public float mitigation;      // 减免量
    public float finalDamage;     // 实际扣血值（已扣护盾吸收后）
    public float absorbed;        // 被护盾/吸收吃掉的部分（本提案恒 0，扩展位）
    public bool isCrit;
    public bool isImmune;         // 前置免疫拦截命中
}
```

### 2.3 DamageRequest / DamageEvent 扩展

```csharp
public struct DamageRequest : IComponent
{
    public DamageBase damage;
    public Entity source;
    public Entity target;         // 冗余，便于 Query；与 damage.source/target 一致
}

public struct DamageEvent : IComponent
{
    public DamageBase damage;     // 原始请求（含 calcKey）
    public float finalDamage;     // 实际扣血
    public float remainingHealth; // 结算后剩余 HP
    public Entity source;
    public Entity target;
    public bool isCrit;           // 新增：本次是否暴击
    public float mitigatedAmount; // 新增：减免量（暴击后 - 最终扣血）
    public bool isImmune;         // 新增：是否被免疫拦截（finalDamage=0）
}
```

## 3. 管线阶段

### 3.1 DamageResolveSystem（重构，order 125）

OnUpdate 对每个 `DamageRequest` 实体：

1. **PreCheck**
   - `target`/`source` 空 → 删除请求，不发事件。
   - target 已死亡/实体已销毁 → 同上（可选，避免死单位吃伤害）。
   - **无敌伤害拦截**：`AttributeHelper.GetFinalValue(target, AttributeHelper.Invulnerable) > 0` → `isImmune=true`，跳 2-3，直接结算 0（仍发 Event，便于触发/UI 感知"免疫"）。`Invulnerable` 是属性叠加态，多个来源叠加值>0 即生效（原生表现未实现不影响 ECS 判定）。
2. **Calculator 分派**
   - `calcKey>0` → `DamageCalculatorRegistry.Get(calcKey)`
   - 否则按 `damageType` 取默认：Physical → 物理默认；Magical → 魔法默认；Real → Real 默认（仅暴击可选，无减免）。
   - 计算器签名（统一内聚）：
     ```csharp
     public delegate void DamageCalculator(ref DamageContext ctx, EntityStore store);
     ```
3. **内置默认计算器流程**
   - a. 暴击判定（仅当来源允许 + 目标不是免疫）：
     - 读取源 `CritChance`（skill 源默认禁暴，melee/ranged 允许；由计算器可配）。
     - `pct = finalValue(CritChance)`；`roll = War3Random.Next01()`（**走 `JassApi.GetRandomReal(0,1)`，引擎同步**）；`roll < pct` → `isCrit=true`，`preMitigation = base × max(1, CritMultiplier)`。
     - 无暴击属性或未命中 → `preMitigation = base`。
   - b. 减免：
     - `damageType==Physical`：查 `ArmorFormulaRegistry` 当前 armor 公式，`armor = finalValue(target, Armor)` → mitigation。
     - `damageType==Magical`：查 magicResist 公式，`mr = finalValue(target, MagicResist)` → mitigation。
     - `damageType==Real`：mitigation = 0。
   - c. `finalDamage = max(0, preMitigation - mitigation)`。
4. **Apply**
   - 护盾吸收位：查目标护盾组件（本提案无），有则先扣 absorbed，再扣剩余；无则 `absorbed=0`。
   - `remaining = ModifyCurrent(target, Health, -finalDamage)`。
5. **PostProcess（钩子位，默认空）**
   - 注册表 `DamagePostProcessRegistry`：吸血/反射/溅射/格挡的实际逻辑后续实现，本提案只留注册位与调用点（在 Event 发出后、死亡判定前或后按语义约定）。
6. **事件与死亡**
   - 创建 `DamageEvent`（带新增字段）+ `TriggerEventMarker`。
   - `remaining<=0 && !isImmune && 目标仍存活状态` → `UnitHelper.KillUnit(target)`。

### 3.1b 无敌拦截控制（读取侧压制）

- **机制**：在 `ControlHelper.GetEffectiveValue(unit, controlAttrId)` 读取出口最前面增加无敌压制——`AttributeHelper.GetFinalValue(unit, Invulnerable) > 0` 时直接返回 0（对 Stun/Silence/NoAttack/Root/CrackFly 五类全压制）。
- **理由**：这是框架既有免疫的承载点（StunImmunity 压 Stun 等），把 Invulnerable 当作"对五类的通用免疫"，单点实现、无施加入口漏拦，不需要改任何 Buff 施加路径。
- **语义**：无敌期间控制 Buff 仍正常创建/计时（图标可见），但读取为 0 → `ControlStateTransitionSystem`/`CastingSystem`/`MoveSystem` 读到的都是"无控制"；无敌结束且 buff 未到期时自动恢复生效。
- **不做**：不驱散/不删除已存在的控制 Buff；不做净化/强驱散。

### 3.2 结算钩子扩展位

```csharp
public static class DamagePostProcessRegistry
{
    public static void Register(string id, DamagePostProcessHandler handler); // 本提案不注册任何内置实现
}
```
调用时机：DamageEvent 创建后。语义约定：只允许读 `ctx` 并发新的 Request（吸血→HealRequest、反射→对 source 的 DamageRequest），禁止直接改 `ctx.finalDamage` 或调原生 API。

## 4. 免伤公式注册表

### 4.1 API

```csharp
public delegate float DamageReductionFormula(float armorOrResist, float incoming);

public static class ArmorFormulaRegistry
{
    public static int Register(string name, DamageReductionFormula formula); // 返回 key
    public static void SetCurrent(string name);        // 全局切换生效公式（运行时）
    public static DamageReductionFormula GetCurrent();
}
```

按伤害类型分开存还是共用一张表：**共用一张表但按 key 存 `Physical/Armor` 与 `Magical/MagicResist` 两条命名槽**，实现上允许分别为物理和魔法选不同公式（`SetPhysicalFormula(name)` / `SetMagicFormula(name)`，默认都指向 War3 公式）。Real 不查表。

### 4.2 内置公式

**War3 公式**（默认；贴近魔兽玩家直觉）
```
Armor(正):    mitigation = armor * k / (1 + armor * k) * incoming     // k≈0.06
Armor(负):    mitigation = (2 - 0.94^(-armor)) ... 取负护甲增伤：incoming × (1 - 0.94^armor) …（armor<0 → 增伤）
```
简化且与 War3 一致：
```
ratio = 1 - 0.06*armor/(1+0.06*|armor|)   // 正负统一平滑
```
落地取 War3 标准近似：`effectiveDamage = incoming × (1 - 0.06×armor/(1+0.06×|armor|))`；armor<0 时该项为增伤系数。

**Dota2 式公式**（第二内置，供切换）
```
effectiveDamage = incoming × (1 - 0.052×armor/(1+0.052×|armor|))   // Dota 默认系数 0.052
```
或更 Dota 的线性表（数值设计空间大，但偏离 War3 直觉）。实现为一条独立 `DamageReductionFormula`，数值实现时确认系数并写注释。

> 关键：两个公式在 `armor=0` 时都输出 0 减免 → 与现状行为一致，是默认无回归的根基。

## 5. 随机数：走 War3 同步 RNG

### 5.1 为什么走 War3 RNG

- 暴击 roll 若走 `System.Random`/`Guid`/`DateTime`，不同玩家帧序/种子不同 → desync。
- War3 引擎的 `GetRandomInt/GetRandomReal` 由引擎保证全端同种子序列，天然锁步安全（`TriggerConditionRegistry.cs` L13"禁止非确定性源"约束的正解）。
- **用户 2026-09-08 确认：随机数一律走 `JassApi.GetRandomInt/GetRandomReal`，不引入自建确定性 RNG。**

### 5.2 薄封装

```csharp
/// <summary>
/// War3 同步随机数薄封装（只转发 JassApi，不做任何状态管理）。
/// 属于 AGENTS Native 分层例外：无句柄、无表现副作用、纯读引擎同步随机状态的一次性便利调用，不承载长期语义。
/// </summary>
public static class War3Random
{
    public static float Next01()      => JassApi.GetRandomReal(0f, 1f);
    public static int NextInt(int lo, int hi) => JassApi.GetRandomInt(lo, hi);
    // 可视需要追加 Next(min,max) 等转发
}
```

- 调用时机仅在伤害结算的计算器内（暴击 roll），每次调用即时返回，无持久状态。
- 代码注释登记分层例外身份，便于后续分层审查放行。

## 6. 兼容性

| 场景 | 现状 | 接入后 | 一致 |
|---|---|---|---|
| 目标无 Armor/MagicResist（finalValue=0） | 扣 damage | 扣 damage | ✅ |
| 源无 CritChance | 永不暴击 | 永不暴击 | ✅ |
| Real 伤害 | 扣 damage | 扣 damage（不查减免） | ✅ |
| 有 Armor 的目标 | 扣满额 | 扣减免后 | 行为变化（预期，激活死属性） |
| 有 CritChance 的源 | 永不暴击 | 可暴击 | 行为变化（预期） |
| 无敌目标 | 仍扣血 | finalDamage=0 + Event.isImmune | 行为变化（预期修复） |
| 无敌目标被施加 Stun 等控制 | 控制落地 | 控制施加被忽略 | 行为变化（预期修复） |

新增字段均带默认，`Projects/*` 现有 `DamageRequest`/`DamageEvent` 构造读取无感。

## 7. 分层与原生边界

- 本 change 只引入**一处** War3 原生调用：`War3Random`（转发 `JassApi.GetRandomReal/GetRandomInt`），按 AGENTS Native 分层例外登记——纯读引擎同步随机状态、无句柄、无表现副作用、无长期语义。
- 其余全部在 ECS 层：读属性、算减免、扣 current、发 Event、触发 KillUnit（KillUnit 只写生命周期，原生移除由现有 Native 系统消费）。
- `DamageResolveSystem` 属业务结算系统，除 `War3Random` 例外外不直接调 JassApi。

## 8. 已确认决策与开放问题

### 已确认（2026-09-08）

1. 随机数走 `JassApi.GetRandomInt/GetRandomReal`（War3 同步），不建自研确定性 RNG。
2. 无敌判定复用 `Invulnerable` 属性叠加态（finalValue>0 即生效），不新增 `DamageImmunity` 属性位。
3. 免疫命中仍发 `DamageEvent`（finalDamage=0 + isImmune），不 KillUnit。
4. 无敌免疫大部分控制纳入本次：`ControlHelper.GetEffectiveValue` 读取侧压制（Invulnerable>0 → 五类控制返回 0），控制 Buff 落地但无效，结束恢复；不新增施加入口拦截。

### 开放问题（实施时解决，不必阻塞审核）

1. War3 负护甲精确公式以哪种近似落地？（实现时按 War3 1.27 实测公式核对）
2. 护盾吸收位是否现在就放 `ShieldComponent` 空壳？倾向**不建空壳**，只在 `ctx.absorbed` 留字段，等真实护盾提案再落地组件。
