## MODIFIED Requirements

### Requirement: Inline authoring creates a standard Ability template

Inline lambda MUST 在 authoring 阶段立即构造成标准 `AbilitySpec`，并注册为实现 `IAbilityTemplate` 的内部模板。系统 MUST NOT 保存 authoring lambda，也 MUST NOT 新增 Item 专用 Effect 执行路径。

内部模板 MUST 通过既有 `AbilitySpecBuilder.Apply`、companion 生命周期、Casting、Cooldown 和 Effect pipeline 执行。

系统 MUST NOT 在注册前对 inline `AbilitySpec` 做反射 schema 一致性校验、深拷贝、规范化指纹、资源上限、严格 UTF-8 或循环引用校验。inline spec 属编译期开发者 authoring 输入，不视为不可信数据。原"注册前递归拒绝非空运行时 Entity 字段"的强约束不再要求；builder 正常入口不产生非空 Entity 字段。

### Requirement: Generated identity is deterministic and collision-safe

内部 template name MUST 由规范化 Item template name 确定，并 MUST 使用框架保留前缀。不同 Item template MUST 产生不同名称。

相同 Item template 的重复配置 MUST 幂等复用首个已注册的内部模板（first-wins）：当保留名称已由 `InlineItemAbilityTemplate` 占用时，注册 MUST 直接复用该模板而不重新构造。系统 MUST NOT 再基于结构指纹比较同 owner 的多次注册；"同 owner 不同 spec 必须失败"不再要求。

当生成名称已被普通 Ability template 或其他非 inline 模板占用时，系统 MUST 明确失败且 MUST NOT 静默覆盖。

普通 `AbilityTemplate.Register` MUST 拒绝保留前缀，inline 注册 MUST 使用专用入口。该保护 MUST 不依赖 `AbilitySpec.Initialize()` 调用顺序，后到的普通或 generated 注册 MUST NOT 覆盖 inline template。

### Requirement: Overloads and failure modes are verifiable

验证 MUST 覆盖三个 `UseAbility` 重载的文档化调用形式编译、重复配置、初始化顺序、晚到覆盖、注册冲突、等级同步和模板复用。相同 Item template 的实例 MUST 复用同一内部模板名称，且每个 Item entity MUST 拥有独立 companion、cooldown 和施法状态（实例间共享同一份只读内部 `AbilitySpec` 是允许的）。

验证 MUST NOT 再要求同 owner 不同 spec 失败、运行时 Entity 拒绝、资源上限、严格 UTF-8、循环引用或快照隔离等已移除的 authoring 期约束。`War3Frame` 与 `Projects/test` Release 构建以及纯 ECS Item companion 场景 MUST 通过。
