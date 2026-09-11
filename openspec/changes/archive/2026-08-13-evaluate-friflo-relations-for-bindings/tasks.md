## 1. 文档与规则

- [ ] 1.1 审核 Friflo 官方 `Relations` / `Relationships` 文档结论
- [ ] 1.2 审核项目级 `.opencode/skills/friflo-ecs-relations/SKILL.md`
- [ ] 1.3 确认 `IRelation<TKey>` / `ILinkComponent` / `ILinkRelation` 项目使用边界
- [ ] 1.4 确认本提案阶段不直接迁移运行时代码

## 2. 当前关系清单

- [ ] 2.1 记录已使用 `ILinkComponent` / `ILinkRelation` 的正例
- [ ] 2.2 记录普通 `Entity` 字段候选清单
- [ ] 2.3 标注长期绑定与瞬时上下文的区别
- [ ] 2.4 标注 slot/order/index 等不宜直接 relation 化的关系

## 3. 风险规则

- [ ] 3.1 记录 link component 更新必须用 `AddComponent` 的规则
- [ ] 3.2 记录 relation 遍历时不得 add/remove 的规则
- [ ] 3.3 记录单 entity 同类型 relation 数量建议上限
- [ ] 3.4 记录 Native / Execution 分层约束

## 4. 后续迁移规划

- [ ] 4.1 选择后续试点候选，例如 `EffectAttachment`
- [ ] 4.2 为试点定义 incoming links 和 target 删除清理验证
- [ ] 4.3 明确 Ability / Item slot 迁移必须保留 index 语义
- [ ] 4.4 明确瞬时 payload 默认不迁移

## 5. 验证

- [ ] 5.1 检查 OpenSpec 工件完整性
- [ ] 5.2 检查项目 skill 内容与官方文档一致
- [ ] 5.3 总结本轮只形成规范和后续迁移提案，不改运行时代码
