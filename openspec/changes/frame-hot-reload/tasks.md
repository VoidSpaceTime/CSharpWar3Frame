## 1. 提案与事实基线

- [x] 1.1 复核 `HEAD=3a5f1ba` 的 `Run.cs`、`Execution/{BuildWorkspace,ProcessRunner}.cs`、`WE.cs` 与 `BridgeToJIT/`，以 post-merge 结构作为事实基线
- [x] 1.2 通过 `git log -p -- FrameBuild/CommandManager/Run.cs` 与 `git show bb05837:FrameBuild/CommandManager/Run.cs` 还原 `//Hot();` 历史线索，并标注其不是现有能力
- [x] 1.3 确认本 change 为纯设计/评估，设置 `.openspec.yaml` 的 `skip_specs: true`，不新增 spec delta

## 2. 架构设计与候选裁决

- [x] 2.1 定义“运行时载荷替换”与普通重建、地图重载、客户端重启的边界
- [x] 2.2 分析 JIT bridge、AOT publish、WE 长生命周期进程、构建 staging/rollback 的约束
- [x] 2.3 比较原路径覆盖、版本化 ALC、JIT 会话重启、完整客户端重启、.NET Hot Reload 和“仅设计、暂缓实施”
- [x] 2.4 记录推荐顺序：仅设计暂缓；未来只在隔离实验通过后考虑 JIT 会话重启；AOT 采用显式 fallback

## 3. 审核门与未来实施（当前不执行）

- [ ] 3.1 用户审核并裁决“仅设计、暂缓实施”或批准后续 JIT spike
- [ ] 3.2 若批准实施，建立独立实现范围，补充行为 delta、具名 Regression 场景和完整回滚验收
- [ ] 3.3 完成 P0-P5 阶段门后，再决定是否进入代码实现与归档

## 4. 本 change 验证

- [x] 4.1 运行 `openspec validate frame-hot-reload`
- [ ] 4.2 未经用户批准不得实现、归档或宣称热更已支持
