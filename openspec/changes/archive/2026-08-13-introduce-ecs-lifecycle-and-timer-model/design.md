## Context

当前 War3Frame 已具备两类明确的时间/生命周期执行轨道：

- `ImmediateRoot`：处理需要立即落地的生命周期动作
- `TimedSystemRoot` + `UpdateTick.deltaTime`：处理随时间推进的状态变化

与此同时，现有单位生命周期状态仍然较薄，仅通过 `UnitState { isAlive, rebornTime }` 表达；而未来需求已经明显超出这个范围，包括：

- 单位死亡立即生效，但原生句柄延迟解绑
- 尸体保留一段时间后再清理
- 技能效果持续一段时间后结束
- 周期性临时效果（DoT / HoT / Aura Tick / Channel Tick）
- 计时任务需要支持暂停、循环次数与 owner 生命周期约束

## Goals / Non-Goals

**Goals:**
- 用组件化状态模型替代单一布尔生死状态。
- 定义统一的计时任务模型，兼容一次性与循环型需求。
- 让 timer 与 owner 生命周期显式绑定，避免“owner 已销毁但 timer 仍在漂浮”的问题。
- 保持 AOT 友好：纯数据组件 + 静态系统分发，不依赖委托持久化、反射或运行时代码生成。

**Non-Goals:**
- 不在本次提案里一次性改完所有 buff/effect/skill/死亡链路的实现。
- 不引入基于 `Action`/闭包的通用 callback timer 设计。
- 不把所有延时行为都塞进一个万能系统直接执行业务逻辑。

## Decisions

### 1. 生命周期状态用组件持久表达，tag 只表达瞬时事件
**Decision:** 像单位生死这种长期状态用组件表达，而不是用 tag 作为唯一真相源；tag 仅用于 `DeathRequested`、`Expired`、`RecycleRequested` 这类瞬时事件。

**Rationale:** 生命周期状态会不断长出更多字段与阶段，组件比 tag 更适合作为持久状态容器。

### 2. 引入统一 `TimerTask` 模型
**Decision:** 定义通用计时任务组件模型，至少包含：

- `mode`：单次 / 循环
- `interval`：周期
- `remaining`：剩余时间
- `paused`：暂停
- `owner`：归属实体
- `kind`：到期路由类型
- `triggerCount`：已触发次数
- `maxTriggerCount`：最大触发次数

**Rationale:** 这些字段共同决定计时任务的生命周期，缺一就难以稳定承载尸体保留、持续效果、周期技能等行为。

### 3. 时间推进与业务副作用分离
**Decision:** 时间系统只负责：

- 扣减 `remaining`
- 判断是否到期
- 按 `kind` 产生到期信号或状态

具体业务副作用由各领域系统负责处理，如尸体清理、buff 到期、effect 销毁、技能持续结束。

**Rationale:** 这样能保持系统职责清晰，避免 timer system 演变成一个万能脚本执行器。

### 4. owner 作为 timer 生命周期约束锚点
**Decision:** 计时任务必须显式绑定 `owner`，当 owner 已销毁或失效时，timer 应按规则自动失效或清理。

**Rationale:** 这是保证 ECS 生命周期一致性的关键，尤其适用于挂载在单位、buff、特效、技能实例上的计时任务。

### 5. 循环次数内建到 timer，而不是散落在业务层
**Decision:** `triggerCount / maxTriggerCount` 属于 timer 核心字段，由时间系统与业务系统共同使用，而不是交给每个业务逻辑各自重复实现。

**Rationale:** 循环触发是通用时间语义，不应该在每个技能或特效系统里手写一遍。

## Risks / Trade-offs

- [风险] 统一 timer 模型过度抽象，影响可读性 → [缓解] 底层统一，业务层仍可保留具名领域组件/系统。
- [风险] 生命周期状态设计过早做大 → [缓解] 第一版只先定义稳定状态机骨架，不一次纳入所有字段。
- [风险] owner 绑定过强导致某些独立全局 timer 不便表达 → [缓解] 可允许显式“无 owner”或根 owner，但默认必须有 owner。

## Migration Plan

1. 定义生命周期状态组件模型，替代 `isAlive` 布尔主导表达。
2. 定义通用计时任务组件模型。
3. 新增时间推进系统与到期路由规则。
4. 按领域逐步迁移：尸体保留 / buff / effect / skill duration。

## Open Questions

- 生命周期状态是否直接采用一个统一枚举，还是一个状态枚举 + 若干补充字段；本次先不锁死具体字段名。
- `kind` 是否用 enum 还是 int 常量注册；本次倾向 enum/静态常量，不走运行时动态分发。
