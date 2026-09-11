## Why

当前单位创建路径与现有的 ECS 脏标记同步模式不一致：删除与部分原生同步已经通过 tag + system 驱动，但创建仍然在模板层直接调用 War3 native。为了让单位生命周期统一进入 ECS 调度模型，并让 `UnitHelper` 不再直接承担 War3 原生创建职责，需要把原生单位创建改成标签/请求驱动，并由 immediate system 立即消费。

## What Changes

- 将单位创建语义从“helper/模板层直接调用 War3 native”调整为“Entity 先携带创建请求，再由 immediate system 创建原生单位”。
- 让 `UnitHelper` 不再通过现有创建路径直接触发 War3 原生单位创建，而是只负责组装 ECS 侧实体、模板配置与创建请求标记。
- 新增用于原生单位创建的请求数据与标签语义，支持在 immediate system 中消费并补齐 `UnitNative` 等组件。
- 明确原生单位句柄与原生侧基础数据继续存放在 entity/component 上，后续逻辑直接从 ECS 组件读取，而不是回退到 helper 隐式调用。
- **BREAKING**：现有“创建时立即返回已绑定原生 `JUnit` 的 entity”语义将转为“创建后由 immediate system 在同一帧/立即更新阶段补齐原生绑定”。

## Capabilities

### New Capabilities
- `native-unit-creation-request`: 定义 ECS 侧原生单位创建请求、immediate 消费规则，以及创建完成后 `UnitNative` 数据如何挂载到实体上。

### Modified Capabilities

## Impact

- 受影响的核心范围包括 `War3Frame/Src/Helpers/UnitHelper.cs`、`War3Frame/Src/TemplateInit/UnitTemplateAttribute.cs`、`War3Frame/Src/Components/Units.cs`、`War3Frame/Src/Systems/Native/*`、`War3Frame/initialization/ECSInit.cs` 以及 immediate system 注册链路。
- 影响单位创建时序与原生绑定时机，但不要求本轮提案兼容现有局部结构设计，也不要求保留当前模板层直接创建 native 的内部实现。
- 后续实现需评估依赖“创建后立刻可读取 `UnitNative`”的调用点，并决定是通过立即更新保障时序，还是通过显式约束调用顺序规避错误使用。
