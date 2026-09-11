## Context

当前仓库已经有若干 effect-related building blocks：

- `AbilityEffectHelper` 会创建 effect entity
- `ProjectileSystem` / effect systems 已经把 projectile 与 payload 分层
- `EffectNativeSystem` 具备原生执行层雏形

但 ownership model 尚未被正式收敛：

- helper 仍可能越过 ECS 直接 shape 持续行为
- native handle 的创建与后续持续状态之间还没有被明确定义为“execution-only”关系
- `Position` 已经能作为 projectile / effect 运动真相，但 effect 体系尚未明确禁止第二套 position-truth

同时，外部 `effector.lua` 提供了一个可借鉴的操作面：

- 创建 / 销毁
- 显示 / 颜色 / 缩放 / 速度 / 透明度
- 旋转 / 动画 / 位置 / 绑定

但该 Lua 实现不能成为 semantic truth owner。它适合借鉴“操作种类”，不适合借鉴“句柄对象即真相”的架构模式。

## Goals / Non-Goals

**Goals:**
- 让 ECS components 成为 long-lived effect semantics 的唯一真相源。
- 让 native effect handle 成为 execution-only resource。
- 让 `Position` 成为 movable effect 与 projectile visual 的唯一 motion truth。
- 让 sustained effect reconciliation 统一在 `0.02s` cadence 上推进。
- 保留 helper 的即时 ergonomics，但去除 helper 对长期 effect truth 的 ownership。
- 为 attachment、appearance、animation request、projectile visuals 建立统一 ownership model。

**Non-Goals:**
- 本提案不引入第二套 movement-position 组件。
- 本提案不重构单位运动系统本身。
- 本提案不让 Lua/native integration 定义 effect semantics。
- 本提案不在提案阶段进入运行时代码实现。

## Decisions

### 1. ECS owns long-lived effect state
**Decision:** attachment、lifetime、appearance、animation request、projectile visual ownership 与 motion semantics MUST 以 ECS components 表达，并 SHALL NOT 长期依赖 native/Lua-side truth。

**Rationale:** effect semantics 必须与生命周期、投射物逻辑、payload 逻辑一样可在仓库内被完整验证。

### 2. Native effect handles are execution-only resources
**Decision:** native effect handles MAY 被创建、更新、绑定、播放动画、销毁，但 SHALL NOT 拥有 long-lived semantic ownership。

**Rationale:** 原生句柄只是表现层资源，不应反向定义 ECS 语义。

### 3. Motion reuses `Position` directly
**Decision:** movable effects 与 projectile visuals MUST 直接复用 `Position` 作为 motion truth，而 SHALL NOT 发明第二套 effect-position truth model。

**Rationale:** 仓库已经有统一的空间位置表达；再次分裂 position truth 会破坏 projectile/effect 一致性。

### 4. Sustained effect reconciliation runs at `0.02s`
**Decision:** persistent follow、motion synchronization、lifetime progression 与类似持续 effect 行为 MUST 统一通过 `0.02s` cadence 的更新路径推进。

**Rationale:** 这既满足特效平滑度，也避免持续 effect 逻辑散落在 helper 与即时调用中。

### 5. Helpers remain immediate-oriented only
**Decision:** helpers MAY 立即创建 effect entity、发出一次性 animation/appearance 请求、创建 fire-and-forget visual effects；但 SHALL NOT 拥有 persistent effect semantics。

**Rationale:** helper 是 ergonomics layer，不是 runtime ownership layer。

### 6. Attachment is modeled as ECS intent
**Decision:** attached effects MUST 在 ECS 中显式表达 target entity、attachment point 与相关 intent，即使 native API 内部负责 attach execution。

**Rationale:** attach 是否存在、何时失效、何时清理，必须由 ECS 语义决定。

### 7. Projectile visuals are derived from projectile ECS state
**Decision:** projectile visuals MUST 从 projectile ECS state 派生，而 SHALL NOT 成为第二个 motion owner。

**Rationale:** 命中、到达、销毁的真正真相在 projectile logic，不在视觉句柄。

## Proposed Runtime Shape

1. helper/request entry
2. ECS effect entity creation/configuration
3. native spawn/bind execution
4. sustained `0.02s` reconciliation
5. native property / animation application
6. cleanup / despawn

## Effect Semantics To Model Explicitly

- attachment
- lifetime
- appearance
- animation request
- projectile visual ownership
- native execution handle
- `Position`-based motion

## Risks / Trade-offs

- [风险] 新旧 effect path 迁移期共存，导致 helper bypass 或 native bypass 仍残留。  
  [缓解] 先定义 ownership spec，再按 vertical slice 逐步迁移 positional / attached / projectile visuals。

- [风险] 持续更新集中到 `0.02s` cadence 后，可能暴露出此前被即时 helper 掩盖的语义问题。  
  [缓解] 通过 red-first tests 和 runtime smoke scenarios 锁定 attachment、lifetime、animation、projectile arrival 行为。

- [风险] effect 与 projectile 视觉逻辑重新出现双位置真相。  
  [缓解] 在 spec 中显式禁止第二套 position-truth model。

## Migration Plan

1. 审核通过本次 OpenSpec 架构提案。
2. 先定义 red-first tests，覆盖 effect ownership、attachment、lifetime、animation request、projectile visuals 与 `0.02s` cadence。
3. 对 positional effects 做 vertical-slice migration，确保 `Position` 成为唯一 motion truth。
4. 对 attached sustained effects 做 migration，确保 attach semantics 在 ECS 中显式化。
5. 对 animation request 与 appearance synchronization 做 migration。
6. 对 projectile visuals 做 migration，使其从 projectile ECS state 派生。
7. 在验证通过后，收缩或移除 helper-owned sustained truth。

## Open Questions

- attachment 初期是否继续用 native attach API 做执行，而 ECS 仅持有 intent；还是未来需要完全以 Position/offset 推导。
- effect appearance 是否拆成独立组件，还是保持在较薄的基础 effect 组件中。
- projectile visual entity 是否与 projectile effect entity 合一，还是保留事件实体与视觉实体的轻分层。
