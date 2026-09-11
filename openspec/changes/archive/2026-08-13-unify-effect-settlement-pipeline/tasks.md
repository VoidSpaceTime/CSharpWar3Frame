# 任务清单

## 1. 提案闸门

- [ ] 1.1 用户批准本 full 级 OpenSpec 提案后，才能进入实现。
- [ ] 1.2 实现前重新检查范围；如果影响 Generator、Build、CLI 或跨项目契约，必须升级/补充提案后重新审核。

## 2. Settlement 模型

- [ ] 2.1 定义 Heal 和 Buff settlement 的 request / outcome 组件。
- [ ] 2.2 保持 Damage 继续走 `DamageRequest` 和 `DamageEvent`。
- [ ] 2.3 更新 settlement helper 逻辑，确保同一个 effect 的 Damage/Heal/Buff payload 各自只提交一次，并在全部 payload 提交后才完成。

## 3. Effect systems

- [ ] 3.1 必要时调整 `DamageEffectSystem`，接入统一 helper，同时保留 `DamageEvent`。
- [ ] 3.2 将 `HealEffectSystem` 中直接修改 Health 的逻辑改为提交 heal settlement request。
- [ ] 3.3 将 `BuffEffectSystem` 中直接应用 Buff 的逻辑改为提交 buff settlement request。
- [ ] 3.4 新增 Heal 和 Buff settlement request 的 resolver systems。

## 4. Buff 与 Aura 接入

- [ ] 4.1 确保 Buff settlement 使用已有 `BuffHelper` / `BuffSystem` 生命周期语义。
- [ ] 4.2 将 Aura 应用逻辑改为通过 Buff 语义创建影响，而不是裸属性 modifier。
- [ ] 4.3 确保 Aura 移除和单位离开范围时会删除关联 Buff，并标记受影响属性 dirty。

## 5. 验证

- [ ] 5.1 构建 solution 或最小等价项目集。
- [ ] 5.2 验证现有 Damage ability 仍会产生 `DamageEvent`。
- [ ] 5.3 验证 Heal ability/item 通过 settlement 改变 Health，并发布 heal outcome。
- [ ] 5.4 验证 Buff ability 仍能创建、刷新、过期并移除 modifier。
- [ ] 5.5 验证 Aura 能正确添加和移除 Buff-backed modifier。

## 6. 总结

- [ ] 6.1 总结实际修改文件、全局影响、验证结果和剩余风险。
- [ ] 6.2 除非用户明确要求，不创建 git commit。
