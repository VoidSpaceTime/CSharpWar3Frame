## ADDED Requirements

### Requirement: Unit creation SHALL be request-driven in ECS
当外部逻辑请求创建一个 War3 单位时，系统 MUST 先创建或配置对应的 ECS entity，并把原生单位创建参数以请求组件或标签的形式挂载到该实体上，而不是在 helper 或模板入口中直接完成 War3 native 创建。

#### Scenario: Helper initiates unit creation
- **WHEN** 调用方通过单位创建入口请求创建单位
- **THEN** 系统 MUST 先得到带有模板配置和原生创建请求数据的 entity，而不是立即在入口层直接生成并返回已绑定 native 的最终状态

### Requirement: Immediate system MUST consume native creation requests
系统 MUST 提供一个注册到 `ImmediateRoot` 的 immediate system，用于消费原生单位创建请求，并在同一立即更新阶段完成 War3 native 单位创建。

#### Scenario: Pending native creation request exists
- **WHEN** 某个 entity 挂载了待消费的原生单位创建请求
- **THEN** immediate system MUST 在本次立即更新中创建对应的 War3 native 单位，并清除或完成该请求的待处理状态

### Requirement: Native unit data MUST be attached to entity components
一旦原生单位创建完成，系统 MUST 将原生单位 handle 及其基础原生数据写回该 entity 的组件中，使后续系统与逻辑可以直接从 entity/component 读取，而不依赖额外 helper 查询。

#### Scenario: Native unit creation completes
- **WHEN** immediate system 成功完成某个 entity 的 War3 native 单位创建
- **THEN** 该 entity MUST 持有可直接读取的原生单位组件数据，并可供后续系统通过组件访问原生 handle 与相关原生信息

### Requirement: Native creation request consumption MUST be idempotent
原生单位创建请求在被消费后 MUST 进入不可重复创建的完成态。系统 SHALL NOT 因为重复 update 或重复读取同一请求而为同一实体创建多个原生单位。

#### Scenario: Immediate system runs again after completion
- **WHEN** 已经拥有原生单位组件的 entity 再次进入 immediate system 的更新流程
- **THEN** 系统 MUST 识别该请求已完成，并避免重复执行 War3 native 创建
