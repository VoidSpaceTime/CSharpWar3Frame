# 实施总结：native-bridge-completion

**状态**：已实施并归档
**等级**：`full`
**实施后审查强度**：`R2 Targeted`（技术准确性 + 句柄配对/跨系统契约）
**用户批准依据**：`.omo/evidence/gates/user-ruling-2026-10-01.md` 第 5 项 W4 批准门「全部批准（21 仅设计）」
**实施日期**：2026-10-01

## 1. 变更范围

归档 `2026-09-11-native-unit-event-bridge`（预置实体未进反查表）与 `2026-09-11-control-state-superposition`（Silence 无原生动作）两处遗留缺口，按合并后代码形态补全。

### (a) WE/编辑器预置实体 adoption

| 文件 | 改动 |
|---|---|
| `War3Frame/Src/Components/PresetNativeComponents.cs` | 新增 `PresetNativeTag`（`ITag`，零数据分类）与 `DestructableNative`（`IComponent`，含 `JDestructable` 句柄缓存） |
| `War3Frame/Src/Systems/Native/PresetEntityAdoption.cs` | 新增。`AdoptAll(store)`：`GetWorldBounds` + `CreateGroup`，`GroupEnumUnitsInRect`/`ForGroup`/`GetEnumUnit` 收单位，`EnumDestructablesInRect`/`GetEnumDestructable` 收可破坏物；枚举结束 `DestroyGroup`/`RemoveRect`；再 `AdoptHandles`。`AdoptHandles` 为纯索引部分（供回归以句柄列表驱动） |
| `War3Frame/initialization/War3Init.cs` | 在 `PlayerHelper.InitializePlayers` 之后、`NativePlayerEventBridge.Initialize()` 之前调用 `PresetEntityAdoption.AdoptAll(Store)` |

adoption 流程：对每个枚举句柄先 `NativeEntityIndex.TryGetByNative`，命中即跳过（防双登记，不建代理、不再次 `Register`）；未命中则 `CreateEntity` → 挂 `UnitNative`(或 `DestructableNative`) + `PresetNativeTag` → `HandleHelper.HandleAdd` → `NativeEntityIndex.Register`。预置对象的原生创建者是地图加载，本框架只 `HandleAdd` 保持引用计数，绝不 `RemoveUnit` / `RemoveDestructable`。代理不挂 `UnitLifeState`，不会进入 `UnitRemoveNativeSystem` 的唯一销毁点。枚举过滤器传零句柄 `JBoolExpr`（等价 null），不按玩家/联盟过滤，使中立敌对、中立被动与玩家预置单位都能被攻击桥反查。

### (b) Silence 原生动作映射

`War3Frame/Src/Systems/Native/UnitNativeRequestSystems.cs` 的 `UnitControlNativeSystem` 在既有 switch 增加 `ControlType.Silence`，调用新增 `ApplySilence`：

- 进入：`GetUnitAbilityLevel(unit,'ACsp') == 0`（单位尚无该技能）时 `UnitAddAbility('ACsp')` + `IssueImmediateOrder(unit,"spellshield")`。
- 解除：等级为 1（本分支添加的实例）时 `spellshield` 后 `UnitRemoveAbility('ACsp')`。
- 已有等级不为 1 的实例（地图自带）不添加也不移除；无 `UnitNative` 的请求仍被删除且无原生调用。
- `'ACsp'` 以 `private const int AbilitySpellShield = 0x41437370` 表示（1.27 原版技能，不经过 `GetNativeFunction`）。

## 2. 关键设计决策与偏差

1. **防双登记以索引查询为准**：不依赖「初始化早于创建」的时序假设；`NativeEntityIndex.Register` 的覆盖语义未改，创建路径仍可覆盖，adoption 路径由前置 `TryGetByNative` 保证不走到覆盖。
2. **`DzUnitSilence` 否决**：1.27a `GetNativeFunction("DzUnitSilence")` 未命中返回 0，调用即空指针；改用原版 `ACsp` + `spellshield`。
3. **delta 措辞歧义的收敛（相对 design 4.2 伪码的偏差）**：design 4.2 伪码为「level != 1 就 `UnitAddAbility`」，但其同节说明与 delta 规范性 REQUIREMENT（"当单位已存在等级不为 1 的 `ACsp` 时 MUST NOT 添加或移除"）及 Scenario「地图已有非默认魔法盾」相冲突。实现取「level == 0 才添加、level == 1 才移除」：既覆盖「进入沉默」的正常路径（单位原本无技能），又满足 MUST NOT 不拆地图作者的魔法盾。此偏差已在 tasks.md 2.4 与本节记录。
4. **不改动已有映射**：`NoAttack→DzUnitDisableAttack`、`Hide→ShowUnit`、`Root`/`NoPath→SetUnitPathing`、`Pause→PauseUnit` 保持原样；`Stun`/`CrackFly` 仍不进 switch，暂停由 `Pause` 合成承担；`Locust`/`Invulnerable`/`Invisible`/`Sorcery` 保持空分支。`EffectNativeSystem` 未编辑（Todo 24(c) 范围）。

## 3. 全局影响

| 区域 | 是否受影响 | 结论 |
|---|---|---|
| `War3Frame/` | 是 | 新增组件、Native adoption 步骤、War3Init 调用点、Silence case。 |
| `War3Frame.Generator/` | 否 | 未新增系统，未改 `SystemRegisterAttribute` 或生成输出。 |
| `FrameBuild/` | 否 | 未改构建/模板/WE/发布。 |
| `CSharpWar3Frame/` | 否 | 未改 CLI。 |
| `Projects/` | 是 | `Regression` 新增一个文件两个具名 `runtime` 场景 + `Program.cs` 注册一行。 |

## 4. 验证

- `dotnet build War3Frame/War3Frame.csproj -c Release`：0 错误（181 警告，均为既有 nullable 噪声）。
- `dotnet build Projects/Regression/Regression.csproj -c Release`：0 错误（17 警告，既有）。
- `dotnet run --project Projects/Regression -c Release`：**78/78 PASS**（基线 76 + 本变更 2；`runtime/PresetUnitAttackBridge`、`runtime/SilenceNativeMapping` 均 PASS），失败 0。
- 证据：`.omo/evidence/task-32-build.txt`、`.omo/evidence/task-32-regression.txt`。
- 回归不启动 War3，采用与 `native-projection` 相同的「编译生产 Native 源 + 替身 JassApi/HandleHelper/DzApi」方式，不链接 `game.dll`。

### R2 专项视角

- **adoption 不重复登记**：`PresetUnitAttackBridge` 先登记框架句柄 H1 到实体 E1，再让 adoption 枚举到 H1 与未登记 H2/H3。断言 H1 仍反查回 E1 且无 `PresetNativeTag`、无第二次 `HandleAdd`；H2/H3 各得到一个带对应句柄组件与预置标记的代理，`HandleAdd` 恰好一次且位于 `TryGetByNative` 与 `Register` 两次 handle-id 解析之间。工具 group/rect 被 `DestroyGroup`/`RemoveRect` 且不进入反查表。攻击桥分支（两侧反查成功才建 `UnitAttackedRequest`）对 H2 产生 1 个请求，对未登记句柄不产生请求。
- **Silence 生命周期 Add/Remove 配对**：断言进入序列 `getlevel → unitadd → order:spellshield`，解除序列 `getlevel → order:spellshield → unitremove`；已有等级 2 时进入/解除都只 `getlevel`；`Locust` 空分支与无 `UnitNative` 均零原生调用；全程无 `DzUnitSilence` 记录。
- **1.27 能力边界**：`Hide` 极性、`Root` 精确停步、`CrackFly` 位移、四个 ability 类控制均无 1.27 可用函数/依赖地图模板，保持事件层响应与空分支，未新增原生调用。

## 5. 风险与剩余风险

- **客户端未验证**：本机无 War3 客户端，`GetWorldBounds` 实际枚举与 `ACsp`/`spellshield` 是否锁住命令卡未在真实 1.27a 观察。按用户批准与 GOAL 收口指令非阻塞归档。剩余风险：空地图枚举集合为空（正常）、物编改过 `ACsp` 命令或等级时 `spellshield` 可能无效（缓解：只在 level 0 添加、只移除 level 1 实例；失败时 `ControlHelper.CanCast` 仍禁止施法）。
- **预置单位死亡索引残留**：未订阅死亡事件，代理与索引条目保留到进程结束；`HandleAdd` 的引用计数同样保持到进程结束。若后续要回收，需单独提案并保持唯一注销点。
- **全图枚举开销**：只在初始化执行一次，不进 tick；单位极多的地图会拉长 `War3Init`。
- **delta 措辞歧义**已按第 2.3 节收敛并留存记录。

## 6. 回滚

删除 `War3Init` 中的 `AdoptAll` 调用、`PresetEntityAdoption.cs`、`PresetNativeComponents.cs`，以及 `UnitControlNativeSystem` 的 `Silence` case 与 `ApplySilence`。`NativeEntityIndex` 与其余 case 不变；无持久化数据。
