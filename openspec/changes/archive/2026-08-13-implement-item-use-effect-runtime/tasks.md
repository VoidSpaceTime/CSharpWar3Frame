# 任务清单：Revision 2

## 1. 审核

- [x] 1.1 用户审核并批准 Revision 2 的 proposal、design、tasks、capability spec 与 client-validation。
- [x] 1.2 确认 ItemUse 仅负责 EffectSpec handoff。
- [x] 1.3 确认无 outcome、token、幂等、限流、消费、自动移除和 UI 同步。
- [x] 1.4 确认 `ItemUseAbilityData` runtime 继续不支持。

## 2. 精简公共契约

- [x] 2.1 将 `ItemUseRequest` 精简为 user + item。
- [x] 2.2 保留 `ItemUseTargetKind` 与 `ItemUseTarget`。
- [x] 2.3 删除 ItemUse reject reason、committed/rejected outcome、receipt、replay、deferred 和 retention。
- [x] 2.4 将 `ItemEffectOrigin` 精简为 item + user。

## 3. 精简工作流

- [x] 3.1 将 ItemUse systems 收敛为单个薄 EffectSpec 执行系统。
- [x] 3.2 保留 live user/item、owner、state、isUsable、UseEffect-only 和 target 基础校验。
- [x] 3.3 保留有界 preflight、深快照与 item-origin root Effect 创建。
- [x] 3.4 成功或失败后删除 request，不创建 outcome。
- [x] 3.5 删除 guard、deferred merge、outcome cleanup、排序、限流和幂等逻辑。
- [x] 3.6 确认 ItemBase、owner、slot、tags 和 remove state 均不被修改。

## 4. 保留 Effect 执行适配

- [x] 4.1 保留 `CreateItemEffectEntity`，移除 requestToken 参数。
- [x] 4.2 保留 item-origin formula allowlist、lazy ability fallback 和 owner-to-user 映射。
- [x] 4.3 保留 nested EffectSpec 深复制与预算限制。
- [x] 4.4 保留 child、arrive、GroundArea、reaction 的 item-origin 传播。
- [x] 4.5 保留 `UseAbility` / `UseEffect` authoring 互斥校验。

## 5. 回退 Revision 1 非目标改动

- [x] 5.1 回退 ItemRemove request identity、reject outcome、claim 和两阶段扩展。
- [x] 5.2 回退全部 ItemRemove producer 迁移及 Inventory drop/swap 连带重构。
- [x] 5.3 回退 AsyncHelper token overload 与本 change 引入的 SyncHelper identity 协议。
- [x] 5.4 回退 Inventory UI `inv/use`、token、deferred 和 mutation budget。
- [x] 5.5 仅回退 ECSInit、solution 等文件中可由 diff 明确归因于 Revision 1 的 hunk；保留所有用户或其他 change 的未提交改动。

## 6. Helper 与场景

- [x] 6.1 仅保留无 token 的 `ItemHelper.RequestUse(...)` 重载。
- [x] 6.2 将 `Projects/test` 场景改为直接 ECS request，不依赖 Inventory UI/Sync。
- [x] 6.3 覆盖 self、Unit、Point handoff。
- [x] 6.4 覆盖 invalid owner/state/target/spec 不创建 Effect。
- [x] 6.5 验证深快照与 item-origin 传播。
- [x] 6.6 验证 stack、owner、slot、tags 和 remove state 不变。
- [x] 6.7 明确验证 child、arrive、GroundArea、reaction 四类派生实体的 item-origin 传播。
- [x] 6.8 保留 `War3Frame.Game.BridgeMain` payload 契约，并通过 project-reference alias 访问框架 `Game` 状态。

## 7. 验证

- [x] 7.1 执行 `dotnet build War3Frame/War3Frame.csproj --no-restore --configuration Release --verbosity minimal`。
- [x] 7.2 执行 `dotnet build Projects/test/test.csproj --no-restore --configuration Release --verbosity minimal`。
- [x] 7.3 执行 `git diff --check` 与冲突标记扫描。
- [x] 7.4 静态扫描非 Native 改动未新增 War3 native 调用。
- [x] 7.5 扫描确认 `ItemUseRejectReason`、ItemUse outcome/receipt/replay/deferred、token overload、Revision 1 systems、`inv/use` handler 和 ItemRemove 扩展已从本 change 中删除。
- [ ] 7.6 独立 War3 客户端运行精简场景并记录真实结果。

## 8. 总结与提交

- [x] 8.1 提供 Revision 2 full 级总结并列明 Friflo 残余风险。
- [x] 8.2 未收到提交请求，保持工作树未提交。
