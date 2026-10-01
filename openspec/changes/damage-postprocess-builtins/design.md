## Context

见 `proposal.md` 的 Why。当前代码约束：

- `DamagePostProcessRegistry`（`War3Frame/Src/Helpers/DamagePostProcessRegistry.cs:18-36`）只有 `Register` / `Run`，`_handlers` 为空。委托注释要求只读 `ctx`、只发新 Request、禁止改 `finalDamage`、禁止 War3 原生 API。
- `DamageResolveSystem`（`AbilityEffectSystems.cs:988`，order 125）顺序是 PreCheck → Calculator → Apply（`:1047-1049` 整段 `ModifyCurrent(Health, -finalDamage)`）→ `DamageEvent`（`:1052`）→ `DamagePostProcessRegistry.Run`（`:1067`）→ 死亡判定（`:1072`）。
- `DamageContext.absorbed`（`Damage.cs:63-64`）注释为护盾扩展位，没有任何写入点。`HealResolveSystem` order 126，同一次 `Update` 内晚于伤害结算，能消费本帧 `HealRequest`。
- 生命 `current` 由 `AttributeHelper.ModifyCurrent`（`:188-196`）夹紧到 `[0, finalValue]`；原生生命由 `UnitNativeSystem` Compare-Sync，不打 Dirty。
- 主 specs 没有 `damage-pipeline`。归档 `damage-resolution-pipeline` 把护盾放在 Apply、把吸血/反射放在 Event 之后，且明确“本提案不注册内置实现”。

## Goals / Non-Goals

**Goals:**

- 三个内置行为只通过注册表进入结算，主流程不出现吸血/反射/护盾分支。
- 护盾改变本次实际扣血；吸血和反射只产生下一跳请求。
- 属性来源、顺序、互斥和“反射不再后处理”可被具名回归场景断言。

**Non-Goals:**

- 不实现格挡、溅射、治疗加成、护盾回复、护盾类型（物理/魔法分开）或按时长衰减的护盾 Buff。
- 不改暴击、护甲、魔抗、无敌判定本身。
- 不新增 Native 调用，不把护盾做成 War3 原生护盾技能。

## Decisions

### 1. 两段调用，而不是把护盾塞进现有 Event 后钩子

护盾必须减少本次 `ModifyCurrent` 的扣血量。现有 `Run` 在扣血和 `DamageEvent` 之后，此时再改 `ctx.finalDamage` 已经无法影响 HP 与事件。

采用：

1. PreCheck、Calculator 不变，得到 `pending = max(0, ctx.finalDamage)`。
2. **Apply 前**：只跑护盾处理器。它可写 `ctx.absorbed` 与 `ctx.finalDamage`。
3. Apply：`ModifyCurrent(Health, -ctx.finalDamage)`。无敌路径保持 `finalDamage = 0`，不调用护盾。
4. 用 Apply 后的 `finalDamage` 与 `absorbed` 创建 `DamageEvent`。
5. **Event 后、死亡判定前**：跑吸血与反射。它们只读 ctx。

备选：把护盾留在 Calculator 内。否决，因为那是减伤公式位，且会把护盾特例写进主计算器。另一备选是新增 `DamageShield` 组件。否决，资源池已经有 `AttrValue.current` / `finalValue`，不必再造一套真相。

### 2. 属性来源

在 `AttributeHelper` 增三个 ID，与 `Health` / `Armor` 一样由 `Register` 分配，可用修改器叠加：

| 行为 | 读取对象 | 属性 | 读哪个字段 | 语义 |
|---|---|---|---|---|
| 护盾吸收 | 受击者 | `DamageShield` | `current`，上限 `finalValue` | 可消耗资源池。`finalValue` 是池上限，`current` 是剩余。未建属性视为 0 |
| 吸血 | 攻击者 `ctx.source` | `Lifesteal` | `finalValue` | 比例，夹紧到 `[0, 1]`。`0.25` = 回复实际扣血的 25% |
| 反射 | 受击者 | `DamageReflect` | `finalValue` | 比例，夹紧到 `[0, 1]`。按实际扣血反射，不按护盾吸收量 |

不从技能 `EffectValueSpec`、Buff tick 或常量配置读这些比例。物品/Buff 若要提供吸血，应贡献到上述属性，而不是注册第二个后处理器。

### 3. 顺序与互斥

固定顺序：**护盾吸收 → 扣血 → DamageEvent → 吸血 → 反射 → 死亡判定**。

- 无敌：`isImmune` 时三段都不跑。不消耗护盾，不发治疗，不发反射。
- 护盾与吸血/反射：吸血和反射的基数是护盾之后的 `finalDamage`（实际扣血）。吸收后为 0 则两者都不发请求。
- 护盾与死亡：只按剩余 HP 判定。护盾挡满则不死亡。
- 反射与后处理：反射 `DamageRequest` 设 `skipPostProcess = true`。该请求仍走 PreCheck、Calculator、护盾吸收、扣血、事件和死亡判定，但跳过吸血与反射。因此反射可打掉对方护盾，但不会吸血，也不会再反射。
- 同帧多次伤害：每次请求独立吸收当前 `DamageShield.current`，先结算的请求先消耗护盾。

吸血治疗走现有 `HealRequest` → `HealResolveSystem`（order 126），不在伤害系统里第二次 `ModifyCurrent`。反射走新的 `DamageRequest`，不在钩子里递归调用结算。

### 4. 契约收窄

保留“禁止 War3 原生 API”。把“禁止改 `ctx.finalDamage`”收窄为：

- 护盾处理器是唯一允许在 Apply 前写 `absorbed` 与 `finalDamage` 的内置处理器，且 `finalDamage` 只能减少或保持。
- 吸血、反射及以后由外部 `Register` 追加的处理器仍只读 ctx，只允许 `CreateEntity` 新 Request。
- 注册表拆两个列表：`absorb`（Apply 前，仅内置护盾）与 `post`（Event 后）。外部 `Register` 只进入 `post`，不能插入护盾之前，避免双重吸收。

`DamageEvent` 增加 `absorbed`。`mitigatedAmount` 保持“减免量”，不把护盾吸收并进去；护盾用 `absorbed` 单独表达。

### 5. ECS 数据流

```text
DamageRequest
  → PreCheck（无敌：finalDamage=0, isImmune）
  → Calculator（暴击、护甲/魔抗；absorbed 仍为 0）
  → absorb：DamageShield.current -= absorbed；finalDamage -= absorbed
  → ModifyCurrent(Health, -finalDamage)          // 唯一扣血
  → DamageEvent(finalDamage, absorbed, ...) + TriggerEventMarker
  → post：
       Lifesteal>0 且 finalDamage>0 → HealRequest(source 治疗 source)
       DamageReflect>0 且 finalDamage>0 且 !skipPostProcess
         → DamageRequest(target→source, Real, skipPostProcess=true)
  → remaining<=0 且非免疫 → KillUnit
```

反射类型用 `DamageType.Real`，避免反射再吃一次护甲/魔抗。`damageSrc` 沿用原请求。反射来源是原受击者，目标是原攻击者；任一方 `IsNull` 则不发。

生命与护盾的 `current` 都由 Helper 直接写。护盾不是原生投影字段，不打 Dirty、不进 `UnitNativeSystem`。这符合“执行完即可回读、但真相在 ECS”的资源池，而不是 Request 模式。

## Risks / Trade-offs

- [Apply 前吸收与旧注释“钩子只读”不一致] → 只开放 `absorb` 列表，并在注册表注释写明；外部注册进不了这段。
- [反射请求同帧再次进入 `DamageResolveSystem`] → 本次 `OnUpdate` 已快照 pending 列表（`:994-998`），同帧新建请求要到下一帧。`skipPostProcess` 保证下一帧不再发第三跳。
- [吸血与击杀同帧] → 先发 `HealRequest` 再 `KillUnit`。治疗在 order 126 落地；若击杀已把单位带出可治疗状态，治疗结算按现有空目标规则丢弃。不在本变更里改死亡语义。
- [比例属性被修改器加到 1 以上] → 处理器夹紧到 `[0, 1]`，不出现超额治疗或超额反射。
- [旧场景读取 `finalDamage`] → 无护盾时数值不变。有护盾时 `finalDamage` 变为实际扣血，`absorbed` 单独给出吸收量。

## Migration Plan

无存档迁移。新属性缺省为 0，未配置单位行为与现在一致。回滚即移除三个内置注册、两段调用和新增字段的写入；`DamageRequest` 新字段默认 `false`。

## Open Questions

- 无。比例夹紧、反射类型、反射跳过后处理、护盾先于吸血，均已在本设计决定，不留给实现期改口径。
