# Tasks

- [ ] 1. 审核并批准 `fix-friflo-entity-link-relation-startup` 提案范围
- [ ] 2. 将 `HasAbilityStat` 从 `IRelation<Entity>` 调整为 `ILinkRelation`
- [ ] 3. 将 `HasAttr` 从 `IRelation<Entity>` 调整为 `ILinkRelation`
- [ ] 4. 若接口调整引发编译错误，仅修复它们的直接 helper callsite
- [ ] 5. 重新构建 `War3Frame` 与 `Projects/test`
- [ ] 6. 重新运行 JIT 诊断，确认 relation materialization 与 `EntityStore()` 启动通过
- [ ] 7. 汇总结果，判断是否需要为仓内其他 `IRelation<Entity>` 类型开后续提案
