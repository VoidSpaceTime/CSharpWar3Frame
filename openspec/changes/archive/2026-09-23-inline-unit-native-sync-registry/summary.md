# 总结：inline-unit-native-sync-registry

**改动范围**：`UnitNativeSyncRegistry`（`UnitNativeSyncSpec` + `Specs` + `ApplyHealth`/`ApplyMana`/`ToNativeStateValue`）整体内联为 `UnitNativeSystem` 的私有嵌套类型与私有成员；`UnitNativeSyncRegistry.cs` 删除。改动为纯新增 + 一行引用替换（`UnitNativeSyncRegistry.Specs` → `SyncSpecs`），零逻辑变更。

**验证结果**：`War3Frame` Release 构建 0 error（185 个既有 nullable 警告，未见新增）；全仓对 `UnitNativeSyncRegistry` / `UnitNativeSyncSpec` 的引用为 0（含 `Projects/`）；`[SystemRegister]` 计数 68 处 / 33 文件，与改动前一致（未新增或删除系统注册）；搬移的 15 行代码经逐行比对与原实现等价；`git diff` 确认仅新增成员与一行引用替换。

**文件效果**：`Systems/Native/` 下 6 → 5 个文件。

## 阻塞项（既有，非本次引入）

回归宿主 `Projects/Regression` 报告 `Executed 0; passed 0; failed 0`（exit 1）——**在 stash 掉本次改动后的干净树上结果相同**，属既有状态：该宿主在本分支尚未接通，与 `repair-audit-findings-on-luomo`（3/31）"建立可在 .NET SDK 中执行的回归宿主"任务一致。因此本次**未能**取得回归宿主级别的功能验证证据，`R0 Direct` 仅覆盖构建与静态引用核对。既有 `native-projection` 用例覆盖的是 Effect/Player 投影，本就不覆盖 `UnitNativeSystem` 的单位 compare-sync。

**遗留风险**：单位生命/法力投影路径在当前分支无自动化用例覆盖，本次改动的行为正确性依赖"代码等价性比对"而非运行期断言；待回归宿主接通后应补该路径用例。
