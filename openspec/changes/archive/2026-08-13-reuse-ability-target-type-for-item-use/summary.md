## 实施总结

### 实际改动

- 删除 `ItemUseTargetKind`，将 `ItemUseTarget.kind` 改为 `AbilityTargetType`。
- `ItemHelper.RequestUse` 默认创建 `AbilityTargetType.None` 请求。
- `ItemUseSystem` 对 `None`、`Unit`、`Point`、`Area` 进行目标规范化；`Point` 与 `Area` 均使用有限且有边界的坐标校验，但保留各自的 Ability 语义。
- 删除原有 `Point -> Point/Area` 兼容矩阵，改为请求目标类型与 companion `AbilityBase.targetType` 精确相等。
- 验证场景增加 `Point -> Area`、`Area -> Point` 拒绝，以及跨 Store `GroundAreaReactionRequest` 不得修改其他 Store 区域的回归检查。
- `GroundAreaReactionSystem` 在删除原区域前缓存 `EntityStore`，并为请求处理增加同 Store 校验与 `finally` 清理，避免删除后访问失效实体、跨 Store 操作和异常请求滞留。

### 已执行验证

- `ItemUseTargetKind` 全仓库 C# 扫描为零。
- 变更文件 LSP diagnostics 无 error。
- `dotnet build War3Frame/War3Frame.csproj -c Release --no-incremental -p:UseSharedCompilation=false` 通过。
- `dotnet build Projects/test/test.csproj -c Release --no-incremental -p:UseSharedCompilation=false` 通过。
- win-x86 纯 ECS reflection harness 调用 `ItemCompanionAbilityValidationScenario`，输出 `finished with 0 failure(s)`。
- `git diff --check` 通过；仅报告既有工作区文件的 LF/CRLF 提示。

### 环境阻塞与遗留项

- `dotnet build CSharpWar3Frame.slnx -c Release --no-incremental -p:UseSharedCompilation=false` 的托管项目通过；`BridgeToJIT/BridgeToJIT.vcxproj` 因缺少 `$(VCTargetsPath)\Microsoft.Cpp.Default.props` 失败，需要安装或配置 Visual C++ MSBuild 工具链。
- 真实 War3 客户端验证尚未执行，需在单独启动客户端后记录。

### 复审结果

- 目标与约束一致性：PASS。
- 独立运行 QA：PASS；两个 Release 构建通过，纯 ECS 场景 0 failures，临时目录已清理。
- 代码质量：PASS。
- 安全复审：首次发现 `GroundAreaReactionRequest` 跨 Store 与异常残留问题；修复后复审 PASS。
- 仓库上下文挖掘：PASS；未发现遗漏调用方、生成契约、模板或历史约束。
