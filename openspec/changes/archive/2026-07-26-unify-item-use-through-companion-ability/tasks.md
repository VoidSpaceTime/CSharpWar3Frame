## Phase 1: 契约与关系

- [ ] 新增 item 到 companion ability 的 Link 组件，并补充中文职责注释。
- [ ] 明确 companion ability 的 owner、mount 和 slot 约束。
- [ ] 新增独立的 companion 创建/绑定入口，复用 Ability template 应用逻辑但不修改普通 slot 计数。
- [ ] 保证模板存在性、ItemLevel 同步、同 EntityStore 和创建失败原子回滚。
- [ ] 为 owner 转移、掉落、重新拾取和删除定义可重复调用的生命周期操作。
- [ ] 新增 Item 受控删除请求、pending tag 与延迟删除系统。
- [ ] 按阶段实现施法清理和 Ability 状态闭环：提交前恢复 Ready，Channeling/Backswing 清理后进入正常 Cooldown/Ready，且不触发 OnInterrupted/OnFinished。

## Phase 2: Casting 前置修正

- [ ] 为 CastRequest/Casting/Channeling/Cooldown 系统补齐运行时注册与执行顺序。
- [ ] 修复 cooldown `<= 0` 时直接回到 Ready 的状态转换。
- [ ] 在 Cast 消费时重新校验 AbilityOwner、Item link 和 Item pending 状态。
- [ ] 按 request entity ID 升序实现同帧首个有效 ItemUse 胜出，禁止覆盖已有施法。

## Phase 3: ItemUse 迁移

- [ ] 将 `ItemUseEffectSystem` 重命名为 `ItemUseSystem`，从直接 Effect 创建改为 companion `CastRequest` 派发。
- [ ] 保留当前 user/item/owner/state/target 校验和 request 清理语义。
- [ ] 实施 None/Unit/Point 与 Ability target type 的兼容矩阵。
- [ ] 将 item/user origin 从 CastRequest 传播到 CastState、root/child/arrive/ground-area Effect。
- [ ] 确保 companion cooldown 对 Item entity/stack 共享。

## Phase 4: 删除旧模型

- [ ] 从 `ItemSpec` 删除 `useEffectSpec`。
- [ ] 删除 `ItemUseEffectData` 和 `ItemSpecBuilder.UseEffect(...)`。
- [ ] 删除 ItemUse 专用 `EffectSpec` snapshot/直接创建路径。
- [ ] 删除旧的互斥校验和测试模板，迁移到 Ability template。
- [ ] 全仓库扫描并清理 `UseEffect`、`ItemUseEffectData`、`CreateItemEffectEntity` 旧引用。

## Phase 5: 生命周期与集成验证

- [ ] 更新 `Projects/test` 场景，覆盖即时、Unit、Point、Projectile、Area 和 delayed origin。
- [ ] 增加 companion 唯一性、owner 转移、drop/re-pickup、delete 场景。
- [ ] 增加重复请求、Moving/Casting/Channeling/Backswing 中转移或删除、pending ItemUse 拒绝、projectile/ground-area 延迟删除场景。
- [ ] 验证多个 companion 并存时只取消和等待目标 companion。
- [ ] 构建 `War3Frame` Release。
- [ ] 构建 `Projects/test` Release。
- [ ] 构建 `CSharpWar3Frame.slnx` Release，确认生成注册和公共 API 删除后的下游兼容性。
- [ ] 对变更文件执行诊断和静态旧符号扫描。
- [ ] 进行 Oracle 代码质量、安全和目标一致性复审。
- [ ] 单独记录真实 War3 客户端验证结果；不以 .NET 测试替代客户端验证。
