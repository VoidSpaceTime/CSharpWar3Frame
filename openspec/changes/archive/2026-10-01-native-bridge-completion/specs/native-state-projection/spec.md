## ADDED Requirements

### Requirement: WE 预置单位与可破坏物进入反查表

系统 SHALL 在注册玩家单位攻击桥之前，枚举当前地图已存在的单位与可破坏物。对尚未出现在 `NativeEntityIndex` 中的句柄，系统 SHALL 创建只携带原生句柄缓存与预置标记的 ECS 代理，并在增加句柄引用之后登记反查。系统 MUST NOT 为这些预置对象再次调用 `CreateUnit` 或 `CreateDestructable`。

#### Scenario: 预置单位可被攻击桥反查

- **WHEN** 地图加载后存在一个不是由 `UnitCreateNativeSystem` 创建的单位，且攻击桥收到以该单位为攻击者或目标的事件
- **THEN** `NativeEntityIndex.TryGetByNative` 返回该单位的代理实体，攻击桥创建 `UnitAttackedRequest`

#### Scenario: 预置可破坏物可被反查

- **WHEN** 地图加载后存在一个编辑器预置可破坏物
- **THEN** `NativeEntityIndex.TryGetByNative` 返回带可破坏物句柄缓存与预置标记的代理实体

#### Scenario: 枚举工具句柄不进入反查表

- **WHEN** 预置实体枚举结束
- **THEN** 用于枚举的 group 与 world rect 已被销毁，且它们的句柄不在 `NativeEntityIndex` 中

### Requirement: 预置登记不覆盖框架实体

预置实体登记 SHALL 先按 native handle 查询 `NativeEntityIndex`。查询已命中时，系统 MUST NOT 创建新代理，也 MUST NOT 再次 `Register` 同一句柄。

#### Scenario: 已登记单位被枚举到

- **WHEN** 某个单位句柄已经登记到框架创建的实体，随后预置枚举再次见到该句柄
- **THEN** 反查结果仍是原来的框架实体，该实体没有预置标记

#### Scenario: 未登记句柄只登记一次

- **WHEN** 同一个未登记句柄在同一次枚举中被处理
- **THEN** 系统只创建一个代理，且 `Register` 只发生一次

### Requirement: Silence 使用 1.27 原版技能锁施法

`UnitControlNativeSystem` 在消费 `ControlType.Silence` 的 `ControlStateNativeRequest` 时 SHALL 使用 `UnitAddAbility`、`UnitRemoveAbility`、`GetUnitAbilityLevel` 与 `IssueImmediateOrder` 切换原版技能 `ACsp`。系统 MUST NOT 调用 `DzUnitSilence`。当单位已存在等级不为 1 的 `ACsp` 时，系统 MUST NOT 添加或移除该技能。无 `UnitNative` 的请求 SHALL 被删除且不产生原生调用。

#### Scenario: 进入沉默

- **WHEN** 带 `UnitNative` 的单位收到 `entered` 为 true 的 `Silence` 请求，且当前 `ACsp` 等级不为 1
- **THEN** 系统先 `UnitAddAbility` 再以 `spellshield` 下达立即命令，请求被删除，且没有 `DzUnitSilence` 调用

#### Scenario: 解除沉默

- **WHEN** 带 `UnitNative` 的单位收到 `entered` 为 false 的 `Silence` 请求，且当前 `ACsp` 等级为 1
- **THEN** 系统先以 `spellshield` 下达立即命令，再 `UnitRemoveAbility`，请求被删除

#### Scenario: 地图已有非默认魔法盾

- **WHEN** 单位的 `ACsp` 等级已经不是 1
- **THEN** `Silence` 请求不添加也不移除该技能，请求仍被删除

### Requirement: 无 1.27 函数的控制保持事件层响应

对 `Locust`、`Invulnerable`、`Invisible`、`Sorcery`，以及没有单独原生函数的 `Root` 精确停步与 `CrackFly` 位移，系统 SHALL 继续发布既有的 `ControlStateChangedEvent`（`Pause` 合成位除外），且 MUST NOT 新增原生调用。已有的 `NoAttack`、`Hide`、`Root`/`NoPath` 的 `SetUnitPathing`、`Pause` 映射 SHALL 保持不变。

#### Scenario: 空分支控制

- **WHEN** 单位收到 `Locust`、`Invulnerable`、`Invisible` 或 `Sorcery` 的 `ControlStateNativeRequest`
- **THEN** 系统删除请求，不调用 `UnitAddAbility`、`PauseUnit` 或 `DzUnitSilence`

#### Scenario: 沉默缺失函数时施法判定仍在

- **WHEN** `Silence` 的原生技能切换未能改变单位命令，但沉默属性有效值仍大于 0
- **THEN** `ControlHelper.CanCast` 仍返回 false
