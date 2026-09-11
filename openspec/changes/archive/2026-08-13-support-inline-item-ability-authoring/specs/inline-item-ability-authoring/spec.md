## Capability: inline-item-ability-authoring

### Requirement: Item templates support shared and private Ability authoring

`ItemSpecBuilder` MUST 保留 `.UseAbility(string abilityTemplateName)`，并 MUST 支持通过 lambda 声明物品私有的完整 inline Ability。系统 MUST 支持 `.UseAbility(e => e.Heal(...))` 形式的即时 `None` 目标 Effect 简写。

三个入口 MUST 生成相同的 Item runtime 契约：`ItemSpec` 与 `ItemUseAbilityData` 仅保存 Ability template name。

### Requirement: Inline authoring creates a standard Ability template

Inline lambda MUST 在 authoring 阶段立即构造成标准 `AbilitySpec`，并注册为实现 `IAbilityTemplate` 的内部模板。系统 MUST NOT 保存 authoring lambda，也 MUST NOT 新增 Item 专用 Effect 执行路径。

Inline 注册前系统 MUST 递归验证 AbilitySpec，并 MUST 拒绝任何非空运行时 Entity 字段，包括 Projectile step 的 `effectEntity`。

内部模板 MUST 通过既有 `AbilityTemplate.Apply`、companion 生命周期、Casting、Cooldown 和 Effect pipeline 执行。

### Requirement: Generated identity is deterministic and collision-safe

内部 template name MUST 由规范化 Item template name 确定，并 MUST 使用框架保留前缀。相同 Item template 的重复配置 MUST 仅在规范化 `AbilitySpec` 结构指纹相同时幂等复用；同 owner 产生不同指纹时 MUST 明确失败。不同 Item template MUST 产生不同名称。

当生成名称已被普通 Ability template 或其他 Item template 占用时，系统 MUST 明确失败且 MUST NOT 静默覆盖。

普通 `AbilityTemplate.Register` MUST 拒绝保留前缀，inline 注册 MUST 使用专用原子入口。该保护 MUST 不依赖 `AbilityTemplate.Initialize()` 调用顺序，后到的普通或 generated 注册 MUST NOT 覆盖 inline template。

### Requirement: One item has one active Ability definition

同一 `ItemSpecBuilder` MUST 只允许一次 `UseAbility` 配置。字符串、完整 inline Ability 和即时 Effect 简写之间 MUST 互斥；第二次配置 MUST 明确失败。

### Requirement: Inline Ability preserves level and instance semantics

内部 AbilitySpec MUST 使用 companion 创建或等级同步时传入的 `ItemLevel` 解析。相同 Item template 的实例 MAY 共享静态 Ability 定义，但每个 Item entity MUST 拥有独立 companion、cooldown 和施法状态。

### Requirement: Instant Effect shorthand has explicit defaults

即时 Effect 简写 MUST 生成 `AbilityTargetType.None`、零前摇、零引导、零后摇和零冷却的 Ability，并 MUST 将 lambda 作为 `OnEffect` 行为。需要 Unit、Point、Area 或其他施法配置时，调用方 MUST 使用完整 inline Ability 重载。

### Requirement: Existing runtime architecture remains single-path

本能力 MUST NOT 恢复 `ItemUseEffectData`、`useEffectSpec`、`CreateItemEffectEntity` 或任何 ItemUse 直接 Effect 路径。所有物品主动行为 MUST 继续通过 companion Ability 执行。

### Requirement: Overloads and failure modes are verifiable

验证 MUST 覆盖三个 `UseAbility` 重载的文档化调用形式编译、重复配置、初始化顺序、晚到覆盖、同 owner 不同 spec、运行时 Entity 拒绝、注册冲突、等级同步、模板复用和 companion 状态隔离。`War3Frame` 与 `Projects/test` Release 构建以及纯 ECS Item companion 场景 MUST 通过。
