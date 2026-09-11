## Why

当前技能槽功能已经具备较完整的 helper 层，但业务流程仍主要由 `AbilitySlotHelper` 直接完成：

- 检查槽位合法性
- 检查是否占用
- 变更 `AbilityOwner`
- 变更 `AbilitySlotIndex`
- 更新 `AbilitySlotContainer.currentCount`
- 触发属性贡献 apply/remove request

这意味着 helper 实际上已经承担了大量 workflow ownership，而非单纯的入口封装。随着后续继续加入：

- 技能挂载属性贡献
- 槽位规则
- UI/输入层调用
- 技能解绑与保留实体的差异

当前“强 helper，弱 workflow system”的结构会越来越难扩展。

## What Changes

- 将技能槽挂载/删除/交换/扩槽收敛为 request-driven immediate workflow。
- 明确 helper 只负责发 request，不再直接拥有槽位业务流程。
- 明确 canonical workflow system 负责合法性校验、owner/index/container 更新和 contribution request 触发。

## Capabilities

### New Capabilities
- `request-driven-ability-slot-workflow`: 定义技能槽位操作的请求、即时工作流与所有权边界。

## Impact

- 直接影响 `War3Frame` 的能力挂载/卸下/交换/扩槽主流程。
- 间接影响 ability contribution apply/remove 的触发时机。
- 对 `War3Frame.Generator` / `FrameBuild` / `CSharpWar3Frame` 无直接行为变更预期。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。
