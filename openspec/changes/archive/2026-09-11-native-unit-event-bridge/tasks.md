# tasks.md — native-unit-event-bridge

## T1：反查表 NativeEntityIndex
- [ ] 新建 `War3Frame/Src/Systems/Native/NativeEntityIndex.cs`：静态 Dictionary<int, Entity> + Register(Entity, JHandle)/Unregister(JHandle)/TryGetByNative(JHandle, out Entity)，key 用 GetHandleId
- [ ] 命名空间归 `War3Frame`（与 HandleHelper 一致），中文职责注释

## T2：登记/注销配对
- [ ] `UnitCreateNativeSystem`：创建原生单位后紧邻 `HandleHelper.HandleAdd` 处调 `NativeEntityIndex.Register(entity, native.unit)`
- [ ] `UnitRemoveNativeSystem`（及唯一销毁执行点）：`HandleHelper.HandleRemove` 前调 `NativeEntityIndex.Unregister(native.unit)`
- [ ] 确认无第二条销毁路径漏注销

## T3：AttackType 状态组件 + Helper
- [ ] `Units.cs`：+`public enum AttackType { Melee, Ranged, Chain }`（Chain 注释标注后续实现）
- [ ] `Units.cs`：+`public struct AttackTypeState : IComponent { public AttackType value; }`（缺省 Melee 语义注释）
- [ ] `UnitSpecBuilder`：+`.AttackType(AttackType)` 链式方法（BuildTo 时写初始 AttackTypeState）
- [ ] 新建 `AttackHelper.cs`：`SetAttackType(Entity unit, AttackType type)`（GetOrAddComponent 改写）

## T4：UnitAttackedEvent + 事件注册
- [ ] 新增事件组件 `UnitAttackedEvent : IComponent { Entity attacker; Entity target; }`（放 Settlement.cs 或近邻事件文件，中文注释）
- [ ] `EventTypeRegistry.RegisterBuiltIn()` 补 `Register<UnitAttackedEvent>()`

## T5：NativePlayerEventBridge
- [ ] 新建 `War3Frame/Src/Systems/Native/NativePlayerEventBridge.cs`
- [ ] `Initialize()`：遍历 PlayerHelper.Players 调 RegisterPlayerUnitAttack(每个 p.player)
- [ ] `RegisterPlayerUnitAttack(JPlayer)`：CreateTrigger + HandleAdd + Condition(回调) + TriggerAddCondition + TriggerRegisterPlayerUnitEvent(EVENT_PLAYER_UNIT_ATTACKED)
- [ ] 回调内：GetAttacker/GetTriggerUnit → NativeEntityIndex 反查 → 反查失败 return → CreateEntity(UnitAttackedEvent + TriggerEventMarker)
- [ ] 静态 `_triggers` List<JTrigger> 保活
- [ ] 中文注释：说明"只做桥接，不跑业务"

## T6：初始化挂载
- [ ] `War3Init.cs`：`PlayerHelper.InitializePlayers` 之后调 `NativePlayerEventBridge.Initialize()`

## T7：近战攻击模拟（首轮最小）
- [ ] 新建消费系统或方法：收到 `UnitAttackedEvent` 后读 target 的 `AttackTypeState`（无组件 = Melee）
- [ ] Melee → 构造 DamageRequest（DamageBase: source=attacker, target=target, damage = AttackDamage finalValue，AttackDamage 用 GetOrCreateAttr 读）
- [ ] Ranged/Chain → 本轮跳过（留 TODO 占位注释）

## T8：验证
- [ ] `dotnet build War3Frame` 0 error
- [ ] `dotnet build Projects/test` 0 error
- [ ] 全仓扫描：确认无 AddTag<...> 新增、无 `UnitAttackedEvent`/`AttackTypeState` 命名冲突
- [ ] 代码风格：新 IComponent 按组件规范（命名/注释/无裸 List<string>）

## T9：文档
- [ ] proposal 状态 → 已实施（实施完成后）
- [ ] 写 summary.md
