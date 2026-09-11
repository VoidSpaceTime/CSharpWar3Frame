## Phase 1: 测试先行

- [x] 将测试中的 `ItemUseTargetKind` 全部迁移为 `AbilityTargetType`。
- [x] 为 `None`、`Unit`、`Point`、`Area` 增加精确匹配场景。
- [x] 验证 `Point` 请求拒绝 `Area` Ability，`Area` 请求拒绝 `Point` Ability。

## Phase 2: 公共契约迁移

- [x] 删除 `ItemUseTargetKind`。
- [x] 将 `ItemUseTarget.kind` 改为 `AbilityTargetType`。
- [x] 将 `ItemHelper` 默认请求和调用方迁移到 `AbilityTargetType`。

## Phase 3: 运行时规则

- [x] 为 `ItemUseSystem.TryNormalizeTarget` 增加独立 `Area` 分支。
- [x] 复用既有坐标有限性、边界和同 Store 校验。
- [x] 删除目标兼容矩阵，改为请求类型与 `AbilityBase.targetType` 精确相等。

## Phase 4: 验证与复审

- [x] 全仓库扫描，确认 `ItemUseTargetKind` 零残留。
- [x] 构建 `War3Frame/War3Frame.csproj` Release。
- [x] 构建 `Projects/test/test.csproj` Release。
- [x] 构建 `CSharpWar3Frame.slnx` Release；托管项目通过，`BridgeToJIT.vcxproj` 因缺少 `$(VCTargetsPath)\Microsoft.Cpp.Default.props` 受 Native 工具链环境阻塞。
- [x] 运行 `ItemCompanionAbilityValidationScenario`，确认 0 failures。
- [x] 执行目标一致性、代码质量和安全复审；五路复审最终全部 PASS。
- [ ] 单独记录真实 War3 客户端验证结果。
