# 简化 Item UseEffect 运行时适配层

## 0. 基本信息

- Change ID: `implement-item-use-effect-runtime`
- 修订版本: `revision 2`
- 提案等级: `full`
- 状态: 实施与本地验证完成，等待独立 War3 客户端验收
- 目标一句话: 仅保留 `ItemUseRequest -> item-origin EffectSpec` 执行适配，不在 ItemUse 中实现消费、结果、同步或幂等协议。

## 1. 修订背景

Revision 1 已在未提交工作树中实现，但尚未完成真实 War3 客户端验收，也未形成 commit。复核后确认其中 token、receipt、replay、deferred high-water、消费事务、ItemRemove 扩展和 Inventory UI 同步协议早于真实 UI/多人需求，复杂度超过当前能力目标。

用户已明确选择缩减范围：ItemUse 只负责校验请求并调用统一 EffectSpec 执行层。Revision 2 取代 Revision 1；旧实现不得作为兼容层保留。

## 2. 分级判定

本次仍按 `full` 处理：

- 删除 Revision 1 新增的公共 request/outcome/replay 契约。
- 调整 ItemUse 核心工作流和系统注册。
- 回退 Sync、Inventory UI、ItemRemove 等跨模块改动。
- 重写 `Projects/test` 客户端验证场景。
- 影响 `War3Frame/` 与 `Projects/test/` 两个区域。

## 3. 目标

- 保留最小 `ItemUseRequest` 和 `ItemUseTarget`。
- 处理时重新校验 user、item、owner、item state、`isUsable`、target 和 `ItemUseEffectData`。
- 生成与 authoring 数据隔离的 EffectSpec 运行时快照。
- 以 `ItemEffectOrigin` 明确记录 item 与 user，不冒充 ability。
- 创建 root Effect 后删除 request；无效请求同样直接删除。
- 继续复用既有 Effect resolver 与 Native/Execution 分层。

## 4. 明确取消

- `requestToken`、token 生成与显式 token 重载。
- `ItemUseRejectReason`、`ItemUseCommitted`、`ItemUseRejected`。
- `ItemUseReceipt`、`ItemUseReplayState`、`ItemUseDeferredHighWater`。
- guard、receipt、deferred merge、outcome cleanup 系统。
- ItemUse 内的幂等、重放保护、排序、限流和并发事务语义。
- consumable stack 扣减、零栈自动移除和 ItemRemove 两阶段扩展。
- Revision 1 引入的 `ItemRemoveRejectReason`、claim、outcome 及 producer 迁移。
- `InventoryUISystem` 的 `inv/use` 同步协议和共享 mutation budget。
- `AsyncHelper` token overload 与仅为本 change 引入的 `SyncHelper id.revision` 协议。
- `ItemUseAbilityData -> CastRequest` 适配。

取消后的明确后果：重复创建两个 `ItemUseRequest` 可以产生两个 Effect；失败没有 outcome；ItemUse 不修改 stack、owner、slot、tag 或 remove 状态。

## 5. 保留范围

- `ItemUseRequest { user, item }`。
- `ItemUseTargetKind` 与 `ItemUseTarget` 的 `None`、`Unit`、`Point` 意图。
- `ItemHelper.RequestUse(...)` 的无 token 薄入口。
- `ItemEffectOrigin { item, user }`。
- `AbilityEffectHelper.CreateItemEffectEntity(...)`。
- item-origin formula allowlist、空 ability 防护、有界递归 preflight 与深快照。
- child、arrive、GroundArea、reaction 的 item-origin context 传播。
- `ItemSpecBuilder` 对 `UseAbility` / `UseEffect` 同时配置的 authoring 拒绝。

## 6. 全局影响

- `War3Frame/`: 精简 ItemUse 契约与系统；保留 Effect item-origin 适配；回退 Revision 1 的 ItemRemove、Sync 和 Inventory UI 改动。
- `War3Frame.Generator/`: 不修改生成器；系统增删继续由现有 `[SystemRegister]` 发现。
- `FrameBuild/`: 不受影响。
- `CSharpWar3Frame/`: 不受影响。
- `Projects/`: 仅重写 `Projects/test` 的直接 ECS/War3 客户端场景；`Projects/demo` 不受影响。

## 7. 风险与控制

- 风险: 无 outcome 时调用方无法区分具体失败原因。
  - 控制: 当前阶段只提供 fire-and-forget Effect handoff；需要反馈时另开提案定义最小结果契约。
- 风险: 无幂等保护时重复 request 会重复执行。
  - 控制: 当前不提供网络重试入口；未来在真实同步协议确定后于 ingress 层设计 request ID。
- 风险: ItemUse 不消费 stack，可能与“消耗品”直觉不一致。
  - 控制: 文档和验证明确 stack 完全不变；消费策略另开提案。
- 风险: item-origin 为空 ability，部分公式可能错误读取 ability。
  - 控制: 保留递归 allowlist、显式 `hasValue`、lazy fallback 和深快照。
- 残余风险: 既有 Effect settlement 中存在 Friflo 查询回调内结构变更，可能触发 `StructuralChangeException`。
  - 控制: 用户已取消本轮修复；客户端验收必须如实记录，不能用静态构建代替。

## 8. 验收标准

- self、Unit、Point request 能创建携带 `ItemEffectOrigin` 的 root Effect。
- invalid user/item/owner/state/target/spec 请求被删除且不创建 root Effect。
- 请求前后 ItemBase stack、owner、slot、tags 和 remove 状态不变。
- delayed/nested Effect 使用深快照并传播 item-origin context。
- 工作树不再包含 Revision 1 的 token、outcome、receipt、replay、deferred、ItemRemove 和 `inv/use` 扩展。
- 静态符号扫描确认 Revision 1 公共类型、helper overload、同步 handler 和系统均已删除。
- `War3Frame` 与 `Projects/test` 构建通过。
- 非 Native 改动不新增直接 War3 native 调用。

## 9. 非目标

- 不实现 UI 或同步使用协议。
- 不实现 consumable、charge、cooldown、cost 或自动删除。
- 不实现 ItemUse outcome、receipt、replay 或 rate limit。
- 不实现 `ItemUseAbilityData` casting runtime。
- 不修复本 change 之外的 Effect/Friflo 结构变更问题。

## 10. 审核 Gate

- 用户必须审核并批准 Revision 2 的 `proposal.md`、`design.md`、`tasks.md`、capability spec 与 `client-validation.md`。
- 批准前不得删除或修改当前运行时代码。
- 实施中若重新引入消费、outcome、同步或 Ability casting，必须回到提案阶段重新审核。
