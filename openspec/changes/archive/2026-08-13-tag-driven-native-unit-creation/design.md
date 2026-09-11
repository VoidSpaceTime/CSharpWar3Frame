## Context

当前仓库已经具备一套可用的 ECS 调度基础：`Game.Root` 负责常规 interval system，`Game.ImmediateRoot` 负责 immediate system，系统注册由 `SystemRegisterAttribute` + Source Generator 自动分流。与此同时，单位原生同步已经存在明显的“tag → native system”模式，例如单位删除通过 `NativeUnitDeathDirty` 触发 immediate/native 处理，血量与魔法等同步通过 `UnitNative` + tag 完成。

与之相对，单位创建仍然走模板层直连 native：`UnitTemplate.Create(templateName, player, x, y, facing)` 内直接调用 `JassApi.CreateUnit(...)`，随后才创建带 `UnitNative` 的 entity。`UnitHelper.CreateUnit(...)` 只是将该路径暴露出去。这造成单位生命周期在“创建”阶段仍绕过 ECS 请求/消费模型，和现有删除/同步语义不统一。

## Goals / Non-Goals

**Goals:**
- 让 `UnitHelper` 不再通过现有同步创建路径直接触发 War3 native 创建。
- 把原生单位创建调整为 ECS 请求/标签驱动，并由 `ImmediateRoot` 下的立即系统消费。
- 让原生句柄与原生侧基础数据最终落到 entity/component 上，供后续系统和逻辑直接读取。
- 允许本次设计不受当前局部实现结构约束，优先保证生命周期语义统一。

**Non-Goals:**
- 不在本次方案中重做所有现有 helper 的职责边界。
- 不在本次方案中完整重构所有依赖 `UnitNative` 的系统。
- 不强制兼容“创建函数返回时必须已经存在 native handle”这一旧语义；若需要变化，以 immediate 更新后的新时序为准。

## Decisions

### 1. 引入“创建请求组件/标签 + immediate 创建系统”模式
**Decision:** 单位创建不再在模板层直接完成 native 创建，而是先在 ECS 实体上写入创建请求数据，再由一个注册到 `ImmediateRoot` 的 system 立即消费。

**Rationale:** 现有仓库已经证明 immediate system 适合处理需要即时落地的 native 生命周期动作。把创建纳入同一调度模型，可以让创建、删除、同步三类生命周期动作保持一致的架构语义。

**Alternatives considered:**
- 继续让模板层直接 `CreateUnit`：与现有 dirty/tag 模式不一致，helper 与模板层继续承担 native 侧副作用。
- 改到 interval system：会引入至少一帧延迟，不符合“通过 immediate 立即 update system 创建原生单位”的要求。

### 2. `UnitHelper` 只保留 ECS 侧语义，不承担 native 副作用
**Decision:** `UnitHelper` 负责创建 entity、应用模板配置、附加创建请求与必要标签/组件，但不在 helper 内直接调 War3 native。

**Rationale:** helper 应该是调用入口，而不是原生副作用的最终执行者。这样可以让时序控制集中在 immediate system，减少 helper 内部隐式行为。

**Alternatives considered:**
- 让 `UnitHelper` 直接先创建 native 再补 entity：会继续保留当前问题。

### 3. 原生数据以 entity/component 为直接读取入口
**Decision:** 原生单位 handle、玩家归属及后续可能的原生基础数据，统一以组件形式挂在实体上；创建完成后，由 consuming system 补齐 `UnitNative` 及相关初始化数据，其他系统直接从组件读取。

**Rationale:** 这与现有 `UnitNative` 的使用方式一致，也符合你要求的“原生数据的话，在 entity 或者 component 中，直接获取即可”。

**Alternatives considered:**
- 通过 helper/service 查询 native 数据：会把读取路径重新收回到过程式接口，不利于 ECS 数据流。

### 4. 旧创建时序视为可调整约束
**Decision:** 本方案不优先为旧的“创建返回即有 `UnitNative`”语义做兼容兜底，而是接受“创建请求写入后，由 immediate update 同步完成原生绑定”的新时序模型。

**Rationale:** 你已明确“这个可以不考虑现有代码结构”，因此设计以目标架构一致性优先，而不是为当前路径做折中包装。

## Risks / Trade-offs

- [风险] 某些调用点默认创建返回后即可读取 `UnitNative` → [缓解] 在实现前先审计这些调用点，并用 immediate update 的调用时序或显式约束修正使用方式。
- [风险] 模板配置与 native 创建请求之间的先后顺序不一致，导致创建参数不完整 → [缓解] 明确请求组件的最小必需字段，并要求请求写入发生在模板配置完成后。
- [风险] 创建请求被重复消费，生成多个原生单位 → [缓解] 在 immediate system 消费后移除请求标签/组件，并以 `UnitNative` 已存在作为幂等保护。
- [风险] 原生创建参数未来扩展时耦合到单一组件 → [缓解] 将创建请求设计为独立组件，而不是把所有临时状态塞进 `UnitNative`。

## Migration Plan

1. 定义 native unit 创建请求的组件/标签与完成态约束。
2. 调整 `UnitHelper` / `UnitTemplate` 创建入口，使其只创建 entity、应用模板并写入请求。
3. 新增 immediate system 消费请求并创建 War3 native，然后补齐 `UnitNative` 与初始化数据。
4. 审计依赖旧时序的调用点，明确哪些需要改到 immediate update 之后访问。
5. 验证创建、删除、原生数据读取三条路径在统一模型下可工作。

## Open Questions

- 创建请求组件中是否仅保留 `template/player/x/y/facing`，还是同时携带更抽象的原生单位类型标识；本轮先不锁死具体字段命名。
- 创建完成后是否需要额外的“已创建”标签供后续 immediate 链式系统消费；本轮只定义最小必需能力。
