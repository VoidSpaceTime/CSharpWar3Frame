# 实施总结：Item UseEffect Revision 2

## 状态

Revision 2 已完成代码实施与本地验证。独立 War3 客户端验收尚未执行，`tasks.md` 7.6 保持未完成。

## 实际改动

- `ItemUseRequest` 仅保留 user/item，target intent 继续独立表达 None、Unit、Point。
- ItemUse 收敛为单个薄系统：重新校验 item 与 target、创建有界深快照、生成 item-origin root Effect，并始终清理 request。
- ItemUse 不再持有 token、outcome、receipt、replay、deferred、限流、消费、自动移除、Inventory UI 同步或 Ability casting 语义。
- `ItemEffectOrigin` 仅保存 item/user；owner formula、lazy ability fallback 和 child/arrive/GroundArea/reaction 传播继续复用现有 Effect 层。
- Revision 1 的 ItemRemove、Sync 和 Inventory UI 扩展已按 hunk 回退；无关 CLI 文案 `HelpText = "多开"` 保持不变。
- `Projects/test` 改为直接 ECS request 场景，实际运行 Area/Line/Projectile lifecycle 与 GroundArea/reaction 系统，并覆盖 malformed EffectSpec 和 item 不变性。
- 测试程序集继续暴露 `War3Frame.Game.BridgeMain`，通过 `War3FrameRuntime` project-reference alias 访问框架程序集的 `Game.Store/Root/ECSInit`。

## 全局影响

- `War3Frame/`：新增薄 ItemUse 契约/系统及 item-origin Effect 适配。
- `War3Frame.Generator/`：未修改，继续由现有 `[SystemRegister]` 发现系统。
- `FrameBuild/`：未修改。
- `CSharpWar3Frame/`：未修改本 change 逻辑；保留用户原有未提交文案。
- `Projects/`：仅修改 `Projects/test` 场景、入口和 project-reference alias；`Projects/demo` 不受影响。

## 验证结果

- `dotnet build War3Frame/War3Frame.csproj --no-restore --configuration Release --verbosity minimal`：通过，0 error；175 个既有 nullable warning。
- `dotnet build Projects/test/test.csproj --no-restore --configuration Release --verbosity minimal`：通过，0 warning / 0 error。
- `git diff --check`、冲突标记、Revision 1 旧符号和非 Native 原生调用扫描：通过。
- 部分已回退文件因工作树混合行尾仍显示 `M`，但不出现在 `git diff --raw` 中，不包含 semantic diff。
- bridge 交叉引用确认 payload 仍提供 `War3Frame.Game.BridgeMain`。
- 最终 Oracle 复核：目标一致性 PASS、代码质量 PASS、安全 PASS。
- 后台 QA 与上下文审计因长时间无回传，由用户明确要求取消；不计为通过。

## 残余风险

- 尚未在真实 War3 客户端观察 `[ItemUseEffectThinValidation] finished with 0 failure(s)`。
- Area/Line/GroundArea 等既有 Effect systems 仍可能在 Friflo query callback 内触发 `StructuralChangeException`；该修复已被用户取消，本次不隐藏或规避该风险。
- 共享 Effect settlement 的异常隔离和通用空间数值上限仍是全局架构议题；当前 EffectSpec 来自可信开发者配置，不属于 ItemUseRequest 外部输入边界。
- 若客户端触发上述风险，应记录为外部验收失败并另立提案，不得以构建通过替代。

## 提交状态

未创建 git commit。
