## 1. 实施结果

已实现三种 `ItemSpecBuilder.UseAbility(...)` 入口：共享具名模板、完整 inline `AbilitySpecBuilder` 和即时 `EffectChainBuilder` 简写。Inline 配置在 authoring 阶段立即生成标准 `AbilitySpec`，注册为当前 Item template 私有的内部 Ability template；Item runtime 仍只保存 template name，并继续复用 companion、Casting、Cooldown 和 Effect 单一路径。

内部注册增加了保留前缀、owner 规范化、结构化 SHA-256 指纹、同 owner 幂等/冲突检查、初始化顺序保护和深快照。注册前会拒绝非空运行时 Entity、循环 Effect 图、`OnGranted` / `OnRemoved` 以及超出深度、节点、集合、数组、字符串和 registry 预算的配置；指纹字符串使用严格 UTF-8，非法 surrogate 会在注册前失败。

## 2. 实际影响

- `War3Frame/`：扩展 Item authoring API、Ability template registry 和内部 inline template wrapper；未增加 ItemUse 运行时分支。
- `War3Frame.Generator/`：未修改生成器规则或输出契约，仅执行 Release 构建验证。
- `FrameBuild/`：未修改。
- `CSharpWar3Frame/`：未修改本变更相关逻辑；保留工作区中已有的独立 `HelpText = "多开"` 改动。
- `Projects/`：测试模板和 Item companion 场景增加 inline、边界与隔离验证。

## 3. 验证结果

- `War3Frame.Generator/War3Frame.Generator.csproj` Release：0 error。
- `War3Frame/War3Frame.csproj` Release：0 error；现有 Native API nullable warning 保持不变，未出现 `IL2090`。
- `Projects/test/test.csproj` Release：0 error。
- 一次性 win-x86 纯 ECS harness：`[ItemCompanionAbilityValidation] finished with 0 failure(s)`；临时目录已清理。
- 静态扫描：`ItemUseEffectData`、`CreateItemEffectEntity`、`useEffectSpec`、`ItemUseTargetKind` 零匹配。
- `git diff --check`：通过，仅输出工作区既有 LF/CRLF 转换提示。
- OpenSpec 文档人工结构检查通过且 `tasks.md` 无未处置项；`openspec validate` 因本机命令入口缺失 `@fission-ai/openspec/bin/openspec.js` 未能执行。

场景覆盖三种重载、初始化前后注册、保留名称冲突、同 owner 幂等与差异拒绝、Entity 和循环引用拒绝、Unit/Point/Area/None、等级同步、companion 状态隔离、资源上限、严格 UTF-8、生命周期行为早期拒绝、源集合 mutation 和应用快照隔离。场景出现断言失败时会抛出异常，使 harness 返回失败。

## 4. 复审与遗留事项

初轮复审发现无界递归/集合资源消耗、默认 UTF-8 replacement fallback、场景失败未传播，以及 companion 不支持的生命周期行为拒绝过晚等问题；对应修复和回归场景均已实施并通过上述构建与纯 ECS 验证。最终五路复审按用户要求取消，因此本总结不声明五路全 PASS。

真实 War3 客户端本轮未启动，仍需单独确认场景输出为 0 failures 且无 native/运行时异常。当前改动未 stage、未 commit。
