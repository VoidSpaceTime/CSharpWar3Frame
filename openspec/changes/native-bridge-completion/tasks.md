# 任务：native-bridge-completion

状态：待审核。批准前不实现。

## 1. 提案

- [x] 1.1 用合并后代码核对 `NativeEntityIndex` 的唯一 `Register`/`Unregister`、`NativePlayerEventBridge` 反查、`ControlType` 与 `UnitControlNativeSystem` case
- [x] 1.2 对照 1.27a 否决 `DzUnitSilence`（`GetNativeFunction` 未命中返回 0），选定 `ACsp` + `spellshield`
- [x] 1.3 写 `proposal.md`、`design.md`、`tasks.md`、`specs/native-state-projection/spec.md`
- [x] 1.4 `openspec validate native-bridge-completion` 通过
- [ ] 1.5 用户审核批准（批准门；未批准不得勾选，不得开始第 2 节。若否决，在 `proposal.md` 记「未批准」并关闭，不实施）

## 2. 实施（批准后）

- [ ] 2.1 新增 `PresetNativeTag`（`ITag`）与 `DestructableNative`（含 `JDestructable` 字段及中文职责注释）
- [ ] 2.2 新增 `PresetEntityAdoption.AdoptAll`：`GetWorldBounds` + `GroupEnumUnitsInRect` / `ForGroup` 枚举单位，`EnumDestructablesInRect` 枚举可破坏物；`TryGetByNative` 已命中则跳过；否则建代理、`HandleAdd`、`Register`；结束时 `DestroyGroup` 与 `RemoveRect`
- [ ] 2.3 `Game.War3Init` 在 `PlayerHelper.InitializePlayers` 之后、`NativePlayerEventBridge.Initialize()` 之前调用 `AdoptAll(Store)`
- [ ] 2.4 `UnitControlNativeSystem` 增加 `ControlType.Silence`：进入时 level 不为 1 才 `UnitAddAbility('ACsp')` 并 `IssueImmediateOrder("spellshield")`；解除时 level 为 1 才 `spellshield` 后 `UnitRemoveAbility`。不调用 `DzUnitSilence`。不改其它 case，不改 `ControlStateTransitionSystem`

## 3. 验证（批准后，与实施同一闭环）

具名场景注册在 `Projects/Regression` 的 `runtime` 组：

- [ ] 3.1 `runtime/PresetUnitAttackBridge`：已登记句柄保持原实体且无 `PresetNativeTag`；新句柄得到带 `UnitNative` 与 `PresetNativeTag` 的代理，`HandleAdd` 在 `Register` 前恰好一次；反查命中后存在 `UnitAttackedRequest`，未登记句柄不产生请求
- [ ] 3.2 `runtime/SilenceNativeMapping`：`Silence` 进入/解除各产生 `ACsp` 添加或移除与 `spellshield`，记录中无 `DzUnitSilence`；`Locust` 与无 `UnitNative` 的请求无原生调用且请求被删除
- [ ] 3.3 `dotnet build War3Frame/War3Frame.csproj` 与 `dotnet build Projects/Regression/Regression.csproj` 0 错误；`dotnet run --project Projects/Regression -- runtime` 上列两个场景 PASS
- [ ] 3.4 真实 War3 客户端：空地图预置单位被攻击能进桥，沉默开关能锁/解锁命令卡。默认阻塞归档；审核若声明非阻塞，写入 `summary.md` 的未执行原因与剩余风险

## 4. 收口（批准并实施后）

- [ ] 4.1 写完整 `summary.md`（R2：范围、防双登记、1.27a 函数选择、验证、客户端缺口）
- [ ] 4.2 `openspec archive native-bridge-completion --yes`

## 明确不做

- 不枚举物品，不给预置单位补 `UnitLifeState` 或模板
- 不订阅预置单位死亡，不在 `UnitRemoveNativeSystem` 增加第二条销毁路径
- 不修改 `NativeEntityIndex.Register` 的覆盖语义
- 不实现 `Locust` / `Invulnerable` / `Invisible` / `Sorcery` 的 ability 模板
- 不修改 `Hide` / `Root` / `NoPath` / `NoAttack` / `Pause` 的现有调用
- 不编辑 `EffectNativeSystem`（Todo 24(c)）
- 不提交实现，直到 1.5 被用户明确批准
