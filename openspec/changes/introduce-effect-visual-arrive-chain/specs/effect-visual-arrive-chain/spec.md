# effect-visual-arrive-chain 规格

## ADDED Requirements

### Requirement: Effect visual steps

效果 authoring SHALL 支持独立视觉特效 step，并使用公开命名 `EffectVisualKind` 表达视觉创建或移除方式。

#### Scenario: Point visual effect

- **WHEN** 技能效果链声明一个点视觉特效
- **THEN** 运行时 MUST 通过 ECS 数据创建视觉特效实体
- **AND** MUST NOT 在 Builder 或业务效果系统中直接调用 War3 native API

#### Scenario: Attached visual effect

- **WHEN** 技能效果链声明绑定到 caster、target、owner 或 each target 的视觉特效
- **THEN** 运行时 MUST 使用 ECS attachment 语义表达目标和挂点
- **AND** 原生附着创建 MUST 由 `EffectNativeSystem` 执行

### Requirement: Visual lifetime and key cleanup

长期视觉特效 SHALL 支持通过 key 和 owner 语义定位，以便光环、天赋、被动或 Buff 移除时清理。

#### Scenario: Talent wings cleanup

- **WHEN** 天赋获得效果链声明 `AttachOwner` 视觉并指定 key
- **AND** 天赋移除效果链声明按该 key 移除视觉
- **THEN** 运行时 MUST 能定位并清理对应 ECS 视觉特效实体或写入清理请求

### Requirement: Area remains target selection only

`Area(...)` 和 `Line(...)` SHALL 只表达目标选择或空间上下文，不承载视觉特效参数。

#### Scenario: Explosion area

- **WHEN** 技能需要范围爆炸视觉和范围伤害
- **THEN** authoring SHOULD 使用独立 `Effect(...)` step 表达爆炸视觉
- **AND** 使用 `Area(...)` 表达范围目标选择
- **AND** 使用 `Damage(...)` 表达结算

### Requirement: Nested projectile arrival chain

Projectile effect authoring SHALL 支持 `Projectile(...).OnProjectileArrive(arrive => ...)` 形态的嵌套到达效果链。

#### Scenario: Bomb arrives and explodes

- **WHEN** Projectile 到达目标点或目标单位
- **THEN** Projectile 到达处理 MUST 执行 nested arrive effect chain
- **AND** 该链 MAY 包含 `Effect(...)`、`Area(...)`、`Damage(...)` 等普通 effect steps
- **AND** Projectile 系统 MUST NOT 把爆炸视觉写死为 Projectile 参数

#### Scenario: OnProjectileArrive without projectile

- **WHEN** Builder 在没有前置 Projectile step 的情况下声明 `OnProjectileArrive(...)`
- **THEN** Builder MUST 拒绝该调用或产生清晰错误，避免到达链绑定到不明确对象

### Requirement: Native boundary preservation

视觉特效相关 War3 原生副作用 SHALL 继续集中在 `EffectNativeSystem` 或明确 Native 执行层。

#### Scenario: Runtime chain interpretation

- **WHEN** `AbilityEffectSystems`、Projectile lifecycle 系统或 helper 解释视觉 step
- **THEN** 它们 MUST 只创建 ECS 组件、状态、请求或 dirty 标记
- **AND** MUST NOT 直接调用 `JassApi`、`KKApi`、`YDApi` 或 `DzApi`

### Requirement: Runtime-free validation

本能力 SHALL 可通过 builder/spec 结构验证、静态 native 边界检查和项目构建完成基础验收，不要求真实 War3 环境。

#### Scenario: Local validation

- **WHEN** 完成本 change 实现
- **THEN** `dotnet build War3Frame/War3Frame.csproj` MUST pass
- **AND** `dotnet build Projects/test/test.csproj` MUST pass
- **AND** War3 真实显示效果 MAY 作为后续手测项记录
