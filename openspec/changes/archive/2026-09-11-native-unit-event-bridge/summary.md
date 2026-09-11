# summary.md — native-unit-event-bridge

## 变更总结

打通第一条「War3 原生玩家单位事件 → ECS」链路，并落地近战攻击模拟。原生 `EVENT_PLAYER_UNIT_ATTACKED` 触发后经桥接为 ECS 实体，近战单位命中直发伤害（复用现有 DamageResolveSystem 结算）。

## 实际改动范围

### 新增文件（5）
| 文件 | 内容 |
|---|---|
| `War3Frame/Src/Systems/Native/NativeEntityIndex.cs` | native handle → ECS entity 反查表（Register/Unregister/TryGetByNative，根命名空间 War3Frame）|
| `War3Frame/Src/Components/Unit/UnitAttackedEvent.cs` | `UnitAttackedRequest`（模拟攻击意图）+ `UnitAttackedEvent`（对外广播）双实体 |
| `War3Frame/Src/Helpers/AttackHelper.cs` | `GetAttackType`/`SetAttackType`（运行时形态改写）|
| `War3Frame/initialization/War3Init/NativePlayerEventBridge.cs` | 16 玩家 ATTACKED 触发注册 + 回调桥接（按用户要求放 initialization/War3Init/ 目录）|
| `War3Frame/Src/Systems/Unit/AttackSimulationSystem.cs` | 消费 UnitAttackedRequest → Melee 发 DamageRequest（order 124 Immediate，DamageResolveSystem 125 前）|

### 修改文件（7）
| 文件 | 改动 |
|---|---|
| `Src/Components/Unit/Units.cs` | +AttackType 枚举 +AttackTypeState 组件 |
| `Src/Components/Unit/UnitItemAuthoringSpec.cs` | UnitSpec +attackType 字段（缺省 Melee）|
| `Src/Helpers/UnitSpecBuilder.cs` | +AttackType 链式方法；BuildTo 非 Melee 才挂 AttackTypeState（无组件 = Melee）|
| `Src/Systems/Native/UnitCreateNativeSystem.cs` | 创建后 NativeEntityIndex.Register |
| `Src/Systems/Native/UnitRemoveNativeSystem.cs` | 移除前 Unregister |
| `Src/Components/Trigger/TriggerComponents.cs` | EventTypeRegistry.RegisterBuiltIn 补 UnitAttackedEvent |
| `initialization/War3Init.cs` | +NativePlayerEventBridge.Initialize()；删除废弃 AttackInit() 空壳 |

## 关键设计决策

1. **Request + Event 双实体**（对照 BuffApplyRequest/BuffAppliedEvent 仓库模式）：桥回调同时建 `UnitAttackedRequest`（AttackSimulationSystem 唯一消费后删）与 `UnitAttackedEvent`（TriggerSpec 只读广播，EventCleanupSystem 132 清理）——避免"事件被系统消费"与"多监听者只读"的矛盾。
2. **玩家闭包捕获**：C# 循环 16 玩家 TriggerRegisterPlayerUnitEvent（替代 AnyUnitEventBJ），回调闭包已捕获所属玩家，无需 GetTriggerPlayer 反查。
3. **AttackTypeState 独立组件 + Helper**（对照 Dota MODIFIER_STATE 形态切换）：攻击类型是可运行时改写的形态切换，不入数值 Attr 模型；无组件 = Melee（近战零组件开销）。
4. **攻击伤害取 ECS AttackDamage finalValue**（GetFinalValue，ECS 优先），DamageType.Physical + DamageSrc.Melee。
5. **首轮只做近战**：Ranged/Chain 留 TODO 占位（后续投射物/闪电链增量）。

## 验证覆盖

- ✅ War3Frame 编译 0 error
- ✅ Projects/test 编译 0 error
- ✅ 废弃 AttackInit 清除（全仓零残留）
- ⏸ War3 客户端行为验证非阻塞（需真实客户端触发攻击验证桥 → 伤害链路）

## 风险与遗留

- **非 ECS 创建单位被攻击不桥接**（TryGet 失败 return）——WE 预置单位/可破坏物 adoption 未做，留 TODO（后续 Register 进同一反查表即可）。
- **原生伤害 + ECS 伤害双重扣血风险**：当前假设框架单位原生无自动伤害（ECS 创建 + compare-sync）；若出现竞态由后续 DAMAGED 桥 + BlzSetEventDamage 处理。
- 攻击模拟未做护甲减免（与现有技能伤害一致，护甲减免是独立伤害模型后续统一做）。
- 未做范围检查：War3 ATTACKED 事件只在攻击实际发生时触发，攻击者已进入射程，故无需额外范围判定（近战直发）。
