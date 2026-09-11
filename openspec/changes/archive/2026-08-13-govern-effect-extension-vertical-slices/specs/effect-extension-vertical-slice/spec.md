## ADDED Requirements

### Requirement: `EffectChainBuilder` MUST be the canonical public Effect authoring builder
新增普通 Effect step 或组合语义时，公开 authoring API MUST 优先扩展 `EffectChainBuilder`，并 SHALL NOT 为 Ability、Item、Aura、Projectile 或其他领域创建语义重复的平行效果链 Builder。

#### Scenario: Item and ability use the same effect capability
- **WHEN** 某个新效果同时可被技能与物品使用
- **THEN** 两者 MUST 通过同一个 `EffectChainBuilder` API 声明该效果
- **AND** 领域入口 MAY 提供 lambda 适配，但 SHALL NOT 复制效果链实现

### Requirement: Builder execution MUST remain data-only
`EffectChainBuilder` MUST 只构造 `EffectSpec` 数据，并 SHALL NOT 访问 `EntityStore`、创建 ECS entity、推进业务流程或调用 War3 native API。

#### Scenario: A new visual step is authored
- **WHEN** 模板通过 Builder 声明视觉 step
- **THEN** Builder MUST 只记录 step payload
- **AND** native visual creation MUST NOT occur during authoring

### Requirement: Every new Effect capability MUST form a complete vertical slice
每个新增效果能力 MUST 明确覆盖 authoring API、spec payload、semantic resolver、ECS state/request/outcome、必要的 Native/Execution 与验证；不适用的层 MUST 说明不适用原因。

#### Scenario: A new effect proposal is reviewed
- **WHEN** 提案新增一个 Effect step 或改变既有 step 语义
- **THEN** proposal/design MUST document each vertical-slice layer
- **AND** reviewers MUST be able to identify where the effect is authored, represented, resolved, executed, and verified

### Requirement: Authorable capabilities MUST have executable semantics
公开 Builder 不得暴露缺少 resolver 或明确 outcome 的“假能力”。如果能力尚未可执行，MUST 保持未公开，或在独立实验范围内明确标记且不得进入正式模板。

#### Scenario: Builder method has no resolver
- **WHEN** 某个拟新增 Builder 方法尚无可验证的 runtime semantic path
- **THEN** 该方法 MUST NOT 进入公开 authoring API

### Requirement: Invalid Effect combinations MUST fail explicitly
Builder 可判断的非法组合 SHOULD 在构建阶段抛出清晰错误；只能在运行时判断的非法状态 MUST 形成显式失败 outcome 或可观察诊断，并 SHALL NOT 被静默忽略。

#### Scenario: Nested arrive chain has no projectile context
- **WHEN** authoring 在没有可绑定 Projectile step 的情况下声明 arrival chain
- **THEN** Builder MUST reject the configuration with a clear error

### Requirement: Semantic resolvers SHALL NOT execute War3 native side effects
效果语义 resolver MUST 只推进上下文、结算业务结果或写入 ECS state/request/command/outcome，并 SHALL NOT 直接调用 `JassApi`、`KKApi`、`YDApi` 或 `DzApi`。

#### Scenario: Effect step requires a native visual
- **WHEN** resolver 解释需要视觉副作用的 step
- **THEN** resolver MUST create ECS-visible intent or state
- **AND** a Native/Execution system MUST perform the native call

### Requirement: Native side effects MUST respect existing execution boundaries
新增 native 调用 MUST 位于 `Systems/Native/*`、`*NativeSystem` 或 `*ExecutionSystem`；若使用即时 helper 例外，具体提案 MUST 证明其为瞬时、无长期语义、无需重放的调用。

#### Scenario: A new effect needs a War3 order or handle
- **WHEN** 新效果能力需要原生命令或句柄操作
- **THEN** its proposal MUST identify the owning Native/Execution path
- **AND** it MUST explain why an existing execution system cannot be reused when adding a new one

### Requirement: Long-lived Effect truth MUST follow `ecs-native-effect-separation`
`ecs-native-effect-separation` MUST remain the sole normative source for long-lived Effect ownership。具体 Effect change MUST 证明 attachment、lifetime、motion、appearance、animation request、owner/key cleanup 与 projectile visual ownership 符合该 capability，而 SHALL NOT 在本 capability 下重新定义平行 ownership 规则。

#### Scenario: New sustained effect loses its native handle
- **WHEN** 持续效果的 native handle 丢失或重建
- **THEN** effect semantics MUST remain reconstructable from ECS-owned state

### Requirement: Every new Effect capability MUST provide layered verification
新增效果能力 MUST 提供与影响层对应的验证，至少包括 Builder/spec 验证、resolver/ECS 验证、native 边界检查和一个 `Projects/*` 可编译消费示例；不适用项 MUST 在 change 总结中说明。

#### Scenario: A new effect change is ready for acceptance
- **WHEN** 新效果能力准备验收
- **THEN** `War3Frame` and the affected consumer project MUST build successfully
- **AND** non-Native changed files MUST be checked for direct War3 native calls
- **AND** real-War3-only behavior MUST be listed as manual verification or an explicit remaining risk

### Requirement: Concrete Effect features MUST require independent approval
批准本治理规格 SHALL NOT 自动批准任何具体效果能力、公共 API 或运行时迁移。每个具体能力 MUST 通过独立 OpenSpec change 获得审核。

#### Scenario: A developer wants to add a new Effect step
- **WHEN** 本治理提案已经批准
- **THEN** the developer MUST still create and obtain approval for a scoped implementation change
