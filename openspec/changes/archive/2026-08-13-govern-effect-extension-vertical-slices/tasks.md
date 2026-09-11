# 任务清单

## 1. 审核

- [x] 1.1 用户审核并批准 `proposal.md`、`design.md`、`tasks.md` 与 capability spec。
- [x] 1.2 确认等级为 `architecture`。
- [x] 1.3 确认本 change 只建立治理规则，不实现具体 Effect step。
- [x] 1.4 确认本 change 继承而不替代 `formalize-ecs-native-effect-separation`。

## 2. 规格一致性

- [x] 2.1 检查唯一公开 Builder 要求与 `consolidate-effect-builder-authoring` 一致。
- [x] 2.2 检查 ECS truth / native execution-only 要求与 `ecs-native-effect-separation` 一致。
- [x] 2.3 检查跨项目影响分析覆盖 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame`、`Projects/*`。

## 3. 后续盘点（需独立批准后执行）

- [x] 3.1 建立现有效果 step 覆盖矩阵。
- [x] 3.2 记录 Builder/spec 缺口。
- [x] 3.3 记录 resolver/ECS outcome 缺口。
- [x] 3.4 记录 native bypass 或 helper-owned sustained truth 风险。
- [x] 3.5 记录测试、模板和 War3 手测覆盖缺口。
- [x] 3.6 将每个修复点拆成独立 OpenSpec 建议，不顺手修改代码。

## 4. 新效果 change 必备任务模板

- [ ] 4.1 说明为何现有 step 组合无法表达需求。
- [ ] 4.2 设计 `EffectChainBuilder` authoring API。
- [ ] 4.3 设计 step kind/payload 与非法组合处理。
- [ ] 4.4 实现并测试 semantic resolver。
- [ ] 4.5 如需长期状态，设计 ECS truth ownership。
- [ ] 4.6 如需 native 副作用，放入 Native/Execution 层并说明无法复用现有执行层的理由。
- [ ] 4.7 添加 Builder/spec 测试。
- [ ] 4.8 添加 resolver/ECS 测试。
- [ ] 4.9 添加 `Projects/*` 集成示例。
- [ ] 4.10 构建 `War3Frame` 与对应消费项目。
- [ ] 4.11 静态检查非 Native 层未新增 War3 native 调用。
- [ ] 4.12 记录真实 War3 环境手测项与结果。

## 5. 实施 Gate

- [x] 5.1 本治理提案获批前，未以其名义修改运行时代码。

持续约束：

- 本治理提案获批后，具体能力仍必须独立获批。
- 若具体能力影响 Source Generator、构建链路、CLI 或跨项目依赖，自动升级其提案等级。
