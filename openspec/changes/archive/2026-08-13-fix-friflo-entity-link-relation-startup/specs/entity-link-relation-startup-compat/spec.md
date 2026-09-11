# entity-link-relation-startup-compat

## Requirement: Entity-key link relations MUST implement `ILinkRelation`

在 `War3Frame/` 中，凡是以 `Entity` 作为 relation key，且语义上表示 entity-to-entity link 的公开 relation struct，MUST 使用 `ILinkRelation` 契约，而 SHALL NOT 继续仅使用 `IRelation<Entity>`。

### Scenario: Ability stat link relation uses link contract

- **GIVEN** `HasAbilityStat` 表示 ability entity 到 stat entity 的 link relation
- **WHEN** Friflo 对其执行 relation schema materialization
- **THEN** 该类型 MUST 以 `ILinkRelation` 身份参与注册
- **AND** SHALL NOT 因 `EntityLinkRelations<TRelation>` 泛型约束不满足而失败

### Scenario: Attribute link relation uses link contract

- **GIVEN** `HasAttr` 表示 owner entity 到 attribute entity 的 link relation
- **WHEN** Friflo 对其执行 relation schema materialization
- **THEN** 该类型 MUST 以 `ILinkRelation` 身份参与注册
- **AND** SHALL NOT 因 relation contract 不匹配而失败

## Requirement: JIT EntityStore startup MUST survive entity-link relation materialization

JIT 启动路径下，当 `Projects/test` 或等效验证入口创建 `EntityStore()` 时，Friflo 对本地 entity-link relation 的 schema materialization MUST 成功完成，并继续执行后续启动逻辑。

### Scenario: JIT startup continues past relation materialization

- **GIVEN** `BridgeToJIT.dll -> project.dll -> War3Frame.Game.BridgeMain()` 的托管入口已成功执行
- **AND** ability / attribute relation 类型已按 `ILinkRelation` 对齐
- **WHEN** JIT 路径调用 `new EntityStore()`
- **THEN** relation materialization MUST 不再因 `HasAbilityStat` 或 `HasAttr` 触发 Friflo 泛型约束异常
- **AND** 启动流程 MUST 继续执行到 `EntityStore()` 之后的日志或等效验证点
