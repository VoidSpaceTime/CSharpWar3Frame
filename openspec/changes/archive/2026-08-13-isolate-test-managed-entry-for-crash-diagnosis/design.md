## Context

当前已证实：

- `Build/test/map` 产物链已基本齐全
- callback 当前加载目标与实际产物已经匹配

但 `Projects/test/Program.cs` 的托管入口在 `Main(false)` 中仍然过早执行了大量高风险 native 调用：

- `War3.EnableConsole(...)`
- `War3.GetNativeFunction(...)`
- `War3.CallNative(...)`
- `JassApi.Player(...)`
- `JassApi.CreateUnit(...)`
- `JassApi.Condition(...)`
- `TriggerAddCondition(...)`

这导致当前故障排查无法区分：

1. 模块是否根本没成功进入托管侧
2. 还是刚进入托管入口后就被这些原生交互打崩

## Goals / Non-Goals

**Goals:**
- 将 `Program.cs` 启动路径拆成多阶段验证。
- 先验证最小托管入口可达，再逐步恢复高风险调用。
- 用 staged bootstrap 明确区分“加载失败”和“入口崩溃”。

**Non-Goals:**
- 本提案不继续修改 callback 命名或 Build/test 产物布局。
- 本提案不在提案阶段直接改代码。
- 本提案不解决所有 War3 native bridge 问题，只提高定位分辨率。

## Decisions

### 1. Managed entry diagnosis should be staged
**Decision:** `Projects/test/Program.cs` SHOULD 被设计为分阶段托管入口验证器，而不是一次性执行所有 native startup 逻辑。

**Rationale:** 这样可以快速判断崩溃发生在“进入托管前”还是“进入托管后”。

### 2. Minimal entry must be validated first
**Decision:** 第一阶段 MUST 只验证最小托管入口可达，不应立刻执行高风险原生调用。

**Rationale:** 若最小入口都无法到达，则问题不在业务逻辑，而在 host/load 链。

### 3. Native startup should be reintroduced incrementally
**Decision:** `War3.EnableConsole`、`GetNativeFunction`、`CallNative`、`CreateUnit`、`Condition`、`TriggerAddCondition` 等逻辑 SHOULD 按阶段逐步恢复。

**Rationale:** 这样能快速定位哪一类调用触发崩溃。

## Suggested Stages

### Stage 0
- 仅进入 `main`
- 输出最小日志

### Stage 1
- 进入托管侧
- 不做任何 War3 native 交互

### Stage 2
- 恢复最小只读 native 查询

### Stage 3
- 恢复对象创建（如 `CreateUnit`）

### Stage 4
- 恢复 callback / trigger 相关逻辑

## Risks / Trade-offs

- [风险] 只做入口最小化，短期会影响 `Projects/test` 的原始示例功能。  
  [缓解] 把它明确定位为诊断入口，不是长期业务示例终态。

- [风险] 如果 stage 划分过粗，仍然难以定位。  
  [缓解] 把高风险 native 调用拆成多个阶段逐步恢复。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 将 `Projects/test/Program.cs` 拆成 staged bootstrap。
3. 先验证 Stage 0/1 是否可稳定进入托管侧。
4. 再逐步恢复后续 native 调用。
5. 根据首次崩溃发生的 stage，定位具体责任区间。

## Open Questions

- 最小入口使用控制台输出、文件打点、还是 Jass/War3 侧可见信号作为首阶段验证媒介。
- 某些高风险调用是否需要专门的 try/catch + 日志包裹以提高定位效率。
