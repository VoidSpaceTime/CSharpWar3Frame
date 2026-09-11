## 1. Proposal completeness

- [ ] 1.1 审查 `proposal.md` 是否已明确标注 `architecture` 等级、docs-only 边界与非目标
- [ ] 1.2 审查 `design.md` 是否已比较“复用 `Projects/test.bridge`”与“独立 `BridgeToJIT/` contract”两条路线
- [ ] 1.3 审查 `specs/manual-bridge-to-jit-bootstrap/spec.md` 是否已覆盖 payload 存在、payload 缺失、位数或 runtime prerequisite 不匹配、managed entry 缺失四类行为场景

## 2. Evidence review

- [ ] 2.1 复核 `BridgeToJIT/BridgeToJIT.vcxproj` 的当前状态，确认它现在仍是 plain native Win32/x64 DLL 模板，而不是已完成的 C++/CLI bridge
- [ ] 2.2 复核 `BridgeToJIT/dllmain.cpp` 的当前状态，确认 `DllMain` 仍应保持被动角色，且不能作为 managed bootstrap 的落点
- [ ] 2.3 复核 `Projects/test.bridge/testNE.vcxproj` 与 `Projects/test.bridge/BridgeMain.cpp`，把它们定位为参考样例，而不是本次 change 的直接修改对象

## 3. Manual implementation guidance preparation

- [ ] 3.1 准备逐步手工实施说明，解释未来如何把 `BridgeToJIT/BridgeToJIT.vcxproj` 从 plain native 模板收敛到 x86/Win32 C++/CLI `/clr:netcore` 路线
- [ ] 3.2 准备逐步手工实施说明，解释未来如何在 `DllMain` 之外添加稳定 native entry、如何按已加载模块路径定位同目录 `project.dll`
- [ ] 3.3 准备逐步手工实施说明，解释未来需要记录哪些 managed runtime prerequisite 与同目录 companion files，避免把 prerequisite 留成隐含假设
- [ ] 3.4 准备逐步手工实施说明，解释 future validation 应如何覆盖 payload 缺失、位数不匹配、runtime 缺失、managed entry 缺失等失败路径

## 4. Review and readiness gate

- [ ] 4.1 在用户明确批准前，不修改 `BridgeToJIT/` 或其他项目文件
- [ ] 4.2 在用户明确批准前，不启动 `FrameBuild/` wiring、callback 脚本调整或任何 64 位路线讨论
- [ ] 4.3 在用户明确批准前，不更新现有 change package，例如 `replace-test-dnne-route-with-cppcli-bridge`
- [ ] 4.4 若后续进入实现轮次，先基于本 docs-only change 补齐 readiness checklist，再单独审批
