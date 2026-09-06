# 原生玩家单位事件桥 + 攻击模拟（NativePlayerEventBridge）

## 元信息

- **状态**：已实施
- **等级**：light
- **创建日期**：2026-09-04
- **变更 ID**：native-unit-event-bridge

## 背景与目标

### 背景

War3 触发器的原生单位事件（如 `EVENT_PLAYER_UNIT_ATTACKED`）目前**完全无法到达 ECS**：`JassApi.TriggerRegisterPlayerUnitEvent`/`TriggerAddCondition` 等封装已就位，但全仓零注册、零回调桥接；现有触发器体系（TriggerSpec + TriggerEventMarker）只消费 ECS 内部事件。lik 的做法是每个玩家构造时把玩家单位事件注册进共享 condition trigger，回调内直接结算。

本变更打通第一条"原生事件 → ECS"链路，并以"近战/远程模拟攻击"作为首个消费场景：
- War3 单位被攻击命中时触发原生事件 → 桥接为 ECS 事件实体
- ECS 侧模拟攻击：近战直发 `DamageRequest`（走现有 DamageResolveSystem），远程挂投射物（后续增量）
- 为后续原生事件（伤害/死亡/施法/玩家行为）建立可复制的桥模式

### 目标

1. 建立 **native handle → ECS entity 反查索引**（NativeEntityIndex），供所有原生回调反查单位。
2. 建立 **NativePlayerEventBridge**：C# 循环 16 玩家注册 `EVENT_PLAYER_UNIT_ATTACKED`，回调翻译为 `UnitAttackedEvent` 事件实体（带 TriggerEventMarker），供触发器体系与攻击模拟消费。
3. 攻击分流所需**单位攻击形态状态**：`AttackTypeState`（Melee/Ranged/Chain，运行时可变）+ `UnitSpecBuilder.AttackType(...)` 预设入口。
4. 首轮攻击模拟：**近战单位命中 → DamageRequest**（打通 原生触发 → ECS 事件 → 伤害结算 全链路）。

### 非目标

- **不做远程投射物模拟**（Ranged 的投射物系统、跟踪、命中）——后续增量。
- **不做闪电链攻击**（Chain）——用户明确后续再实现细节。
- **不做 WE 预置单位/可破坏物 adoption**——反查表首轮只登记框架创建的 ECS 单位，adoption 留 TODO。
- **不改原生伤害数值**（不 BlzSetEventDamage 归零）——本框架伤害全由 ECS 结算后 compare-sync 同步原生，原生单位由 ECS 创建时不产生独立原生伤害；归零问题在 UnitNative 无原生攻击的假设下不存在，如有竞态由后续 DAMAGED 桥处理。
- **不做 UnitDamagedEvent / UnitDeathEvent**——ATTACKED 先行，模式可复制后按需补。

## 影响范围

| 文件 | 改动 |
|---|---|
| `War3Frame/Src/Components/Unit/Units.cs` | +`AttackType` 枚举、+`AttackTypeState` 组件（挂单位，可运行时改写）|
| `War3Frame/Src/Components/Unit/UnitAttackedEvent.cs`（新） | +`UnitAttackedRequest`（模拟攻击意图）+`UnitAttackedEvent`（对外广播，带 TriggerEventMarker）|
| `War3Frame/Src/Systems/Native/NativeEntityIndex.cs`（新） | native handle → Entity 反查表（Register/Unregister/TryGet）|
| `War3Frame/Src/Components/Unit/UnitItemAuthoringSpec.cs` | UnitSpec 加 `attackType` 字段（缺省 Melee）|
| `War3Frame/Src/Helpers/UnitSpecBuilder.cs` | +`.AttackType(...)` 链式方法（写 spec + BuildTo 挂 AttackTypeState）|
| `War3Frame/Src/Helpers/AttackHelper.cs`（新） | +`GetAttackType`/`SetAttackType`（运行时改形态）|
| `War3Frame/initialization/War3Init/NativePlayerEventBridge.cs`（新） | 16 玩家 ATTACKED 触发注册 + 回调桥接（用户指定放 initialization/War3Init/）|
| `War3Frame/Src/Systems/Unit/AttackSimulationSystem.cs`（新） | 消费 UnitAttackedRequest → Melee 发 DamageRequest（order 124，在 DamageResolveSystem 125 前）|
| `War3Frame/Src/Systems/Native/UnitCreateNativeSystem.cs` | 创建原生单位后 Register 反查 |
| `War3Frame/Src/Systems/Native/UnitRemoveNativeSystem.cs` | 移除前 Unregister |
| `War3Frame/Src/Components/Trigger/TriggerComponents.cs` | EventTypeRegistry.RegisterBuiltIn 补 UnitAttackedEvent |
| `War3Frame/initialization/War3Init.cs` | +`NativePlayerEventBridge.Initialize()`；删除废弃 `AttackInit()` 空壳 |

**不影响**：War3Frame.Generator（无新增生成需求）、FrameBuild、BridgeToJIT、其它项目边界。UnitCreateNativeSystem/UnitRemoveNativeSystem 只加配对登记，不改原生创建/销毁逻辑。

## 方案摘要

### 1. NativeEntityIndex（反查表）

```csharp
// 单表：所有 native handle → ECS entity。handle id 全局唯一（JWidget 统一空间），
// 单位/可破坏物/物品可共用；查出的 entity 自身组件即表达类型。
public static class NativeEntityIndex
{
    private static readonly Dictionary<int, Entity> _byNativeHandle = new();
    public static void Register(Entity entity, JHandle native);   // key = GetHandleId(native)
    public static void Unregister(JHandle native);
    public static bool TryGetByNative(JHandle native, out Entity entity);
}
```

登记/注销配对点（与 HandleHelper 相邻）：
- `UnitCreateNativeSystem` 创建原生单位后 → `Register(entity, native.unit)`
- `UnitRemoveNativeSystem` 移除前 → `Unregister(native.unit)`

adoption（WE 预置单位/可破坏物）留 TODO：初始化时全图枚举后 Register 进同一张表。

### 2. NativePlayerEventBridge

```csharp
public static class NativePlayerEventBridge
{
    private static readonly List<JTrigger> _triggers = new();   // 保活，防 GC

    public static void Initialize()
    {
        for (int i = 0; i < PlayerHelper.Players.Length; i++)     // 16 玩家
            RegisterPlayerUnitAttack(PlayerHelper.GetPlayer(i).player);
    }

    private static void RegisterPlayerUnitAttack(JPlayer player)
    {
        var trigger = JassApi.CreateTrigger();
        HandleHelper.HandleAdd(trigger);
        var condition = JassApi.Condition(() =>
        {
            var attackerNative = JassApi.GetAttacker();      // JUnit
            var targetNative = JassApi.GetTriggerUnit();     // JUnit
            if (!NativeEntityIndex.TryGetByNative(attackerNative, out var attacker)) return;
            if (!NativeEntityIndex.TryGetByNative(targetNative, out var target)) return;
            // 双实体：Request 驱动模拟攻击（唯一消费），Event 对外广播（Trigger 只读）
            Game.Store.CreateEntity(new UnitAttackedRequest
            {
                attacker = attacker,
                target = target
            });
            Game.Store.CreateEntity(new UnitAttackedEvent
            {
                attacker = attacker,
                target = target
            }, new TriggerEventMarker { eventTypeId = EventTypeRegistry.Get<UnitAttackedEvent>() });
        });
        JassApi.TriggerAddCondition(trigger, condition);
        JassApi.TriggerRegisterPlayerUnitEvent(trigger, player,
            Blizzard.EVENT_PLAYER_UNIT_ATTACKED, null);
        _triggers.Add(trigger);
    }
}
```

回调内只做桥接翻译（反查 + 建 Request/Event 实体），不跑业务——与 SyncHelper 已验证的回调内 CreateEntity 安全模式一致。玩家闭包捕获（每玩家注册时已确定），无需 GetTriggerPlayer 反查。

### 3. AttackType 状态组件 + Helper

```csharp
public enum AttackType { Melee, Ranged, Chain }   // Chain 后续实现

public struct AttackTypeState : IComponent
{
    public AttackType value;   // 缺省 Melee（无组件 = 近战）
}

// UnitSpecBuilder 预设（BuildTo 时写初始值）
public UnitSpecBuilder AttackType(AttackType type)

// AttackHelper（运行时改形态：装备/buff/技能）
public static class AttackHelper
{
    public static void SetAttackType(Entity unit, AttackType type);  // 写/改 AttackTypeState
}
```

设计依据：攻击类型是"形态切换"语义（近战单位装备弓变远程、buff 变身后复原），非数值加减——不入 Attr 数值模型；入 UnitBase 则静态不可变。独立 state 组件 + Helper 写值 + buff 复原语义清晰（对照 Dota MODIFIER_STATE 形态切换）。

### 4. 攻击模拟（首轮近战）

`AttackSimulationSystem`（order 124，Immediate，在 DamageResolveSystem 125 前）消费 `UnitAttackedRequest` 请求实体：
- 读 attacker 的 `AttackTypeState`：
  - `Melee`（或缺省）→ 直接 `DamageRequest`（DamageBase：source=attacker, target=target, damage=AttackDamage finalValue, DamageType.Physical, DamageSrc.Melee）
  - `Ranged` → 本轮跳过（后续投射物增量，留 TODO 占位）
  - `Chain` → 本轮跳过（后续闪电链增量，留 TODO 占位）
- 处理完成后删除请求实体（唯一消费）
- DamageRequest 走现有 DamageResolveSystem(125) 结算 → DamageEvent

攻击力数值取 ECS `AttributeHelper.GetFinalValue(attacker, AttackDamage)`（ECS 优先）。

### 5. 注册与初始化

- EventTypeRegistry.RegisterBuiltIn 补 `Register<UnitAttackedEvent>()`。
- War3Init 在 `PlayerHelper.InitializePlayers` 后调 `NativePlayerEventBridge.Initialize()`。

## 风险与回滚

| 风险 | 等级 | 缓解 |
|---|---|---|
| 原生回调在 ECS Query 迭代中插队建实体 | 低 | SyncHelper 已验证回调内 CreateEntity 安全；桥只建事件实体不遍历 |
| 反查表字典并发/一致性 | 低 | 单线程 War3 主循环；Register/Unregister 配对在 Native 系统内相邻 |
| 触发句柄被 GC | 低 | `_triggers` 静态 List 保活 |
| 双重扣血（原生伤害 + ECS 伤害）| 低 | 框架单位由 ECS 创建、原生无自动伤害；如出现由后续 DAMAGED 桥 + BlzSetEventDamage 处理 |
| 单位先于 ECS 注册被攻击 | 低 | TryGet 失败即 return（非 ECS 单位不桥接）|

回滚：删除新文件 + 还原 3 个修改点（UnitCreate/Remove 配对、War3Init 调用、RegisterBuiltIn 一行）即可。

## 验收标准

1. `War3Frame` 编译 0 error；`Projects/test` 0 error。
2. 模板创建 footman（AttackType 缺省 = Melee）后攻击另一单位，原生 ATTACKED 触发 → 桥建 `UnitAttackedRequest` + `UnitAttackedEvent`（带 TriggerEventMarker）；AttackSimulationSystem 消费 Request 发出 `DamageRequest`（DamageSrc.Melee），单位 Health 按 AttackDamage 扣减（War3 客户端验证非阻塞）。
3. 通过 `UnitSpecBuilder.AttackType(Ranged)` 预设的单位，其 `AttackTypeState.value == Ranged`；攻击模拟系统对该单位命中**不**发 DamageRequest（远程路径留 TODO 占位）。
4. `AttackHelper.SetAttackType(unit, Ranged)` 运行时改写生效（后续近战转远程场景可验证）。
5. 单位移除后反查表条目被 Unregister（无泄漏）；非 ECS 创建单位（无登记）触发时不崩、被跳过。
6. `EventTypeRegistry.Get<UnitAttackedEvent>()` 返回非 0；TriggerSpec 能以 eventTypeId 监听 UnitAttackedEvent。
7. 无新增 `AddTag<...>` 调用；新组件按 IComponent 规范命名（`UnitAttackedEvent`、`AttackTypeState`）。

## 后续（非阻塞）

- 远程投射物模拟（Ranged → 投射物实体跟踪 → 命中 DamageRequest）
- 闪电链攻击（Chain）
- WE 预置单位/可破坏物 adoption（初始化全图枚举 Register）
- UnitDamagedEvent / UnitDeathEvent / 玩家行为事件（LEAVE/CHAT）等后续桥
