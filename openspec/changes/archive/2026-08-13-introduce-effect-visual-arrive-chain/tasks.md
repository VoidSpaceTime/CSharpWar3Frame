# 任务清单

## 1. 审核

- [ ] 1.1 用户审核并批准 `proposal.md` / `design.md` / `tasks.md` / `spec.md`。
- [ ] 1.2 确认提案等级为 `full`。
- [ ] 1.3 确认本 change 只处理 Effect 视觉 step 与 Projectile arrive chain，不处理硬件输入。

## 2. TDD / 静态验证准备

- [ ] 2.1 补充或准备纯 ECS / builder 结构验证，覆盖 `EffectVisualKind` step 创建。
- [ ] 2.2 验证 `Projectile(...).OnProjectileArrive(...)` 能把 nested arrive effect 记录到 spec。
- [ ] 2.3 验证无 Projectile 前调用 `OnProjectileArrive(...)` 会被拒绝或给出清晰错误。
- [ ] 2.4 验证 `Visual` 不需要 War3 runtime 即可形成 ECS 语义数据。

## 3. 数据结构

- [ ] 3.1 新增 `EffectVisualKind`。
- [ ] 3.2 新增 `EffectVisualStepSpec`。
- [ ] 3.3 扩展 `EffectStepKind` 和 `EffectStepSpec`，支持 visual step。
- [ ] 3.4 扩展 `ProjectileEffectStepSpec`，记录 nested arrive effect chain。
- [ ] 3.5 如需要长期视觉清理，新增 owner/key 关联组件或等价 ECS 数据。

## 4. Builder API

- [ ] 4.1 在 `EffectSpecBuilder` 新增 `Effect(...)`。
- [ ] 4.2 在 `EffectSpecBuilder` 新增 `RemoveEffectByKey(...)`。
- [ ] 4.3 在 `EffectSpecBuilder` 新增 `OnProjectileArrive(...)`，绑定最近一个 Projectile step。
- [ ] 4.4 在 `AbilityEffectSpecBuilder` 暴露对应委托入口。
- [ ] 4.5 保持既有 `Damage` / `Heal` / `Buff` / `Area` / `Line` / `GroundArea` / `Projectile` 调用兼容。

## 5. 运行时解释

- [ ] 5.1 在效果执行逻辑中解释 visual step，创建 ECS 视觉特效实体或删除请求。
- [ ] 5.2 支持 `Point` / `TargetPoint` / `AttachCaster` / `AttachTarget` / `AttachOwner` / `AttachEachTarget`。
- [ ] 5.3 支持 `RemoveByKey` 或 `RemoveEffectByKey(...)` 清理长期视觉。
- [ ] 5.4 在 Projectile 到达阶段执行 nested arrive effect chain。
- [ ] 5.5 确认 Projectile 系统不直接执行 native 视觉副作用。
- [ ] 5.6 确认 `EffectHelper` 仍只负责基础 ECS 操作，不拥有长期语义真相。

## 6. 示例

- [ ] 6.1 在 `Projects/test` 补充炸弹到达后爆炸视觉 + Area + Damage 示例。
- [ ] 6.2 补充命中目标附着短时视觉示例。
- [ ] 6.3 补充天赋/光环获得后长期附着视觉并通过 key 移除的示例。

## 7. 验证

- [ ] 7.1 执行 `dotnet build War3Frame/War3Frame.csproj`。
- [ ] 7.2 执行 `dotnet build Projects/test/test.csproj`。
- [ ] 7.3 静态搜索确认 `EffectSpecBuilder`、`AbilityEffectSpecBuilder`、Projectile / Ability effect 系统未新增 War3 native 直接调用。
- [ ] 7.4 总结本 change 的实际改动范围、验证结果、剩余 War3 真实环境手测项。
