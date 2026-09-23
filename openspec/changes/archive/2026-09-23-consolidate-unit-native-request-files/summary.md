# 总结：consolidate-unit-native-request-files

**改动范围**：`UnitControlNativeSystem` 与 `UnitMoveNativeSystem` 搬入新文件 `Systems/Native/UnitNativeRequestSystems.cs`；删除 `UnitControlNativeSystem.cs`、`UnitMoveNativeSystem.cs`。两个 `[SystemRegister(SystemKind.Immediate)]` 原样保留，命名空间与类名未变。文件头记录"`UnitCreateNativeSystem` 不得挪入本文件"的原因（FQN 字母序即隐式执行顺序）。

**验证结果**：`Order|FQN|Kind` 全表 67 项与改动前基线**逐行一致**（`Compare-Object` 无差异）→ 零执行顺序变更，这是本变更的核心证据；`War3Frame` Release 构建 0 error；两个被删文件的 62 行代码经逐行比对在新文件中全部命中（0 缺失）；`Systems/Native/` 下单位相关文件 5 → 4，系统类数仍为 7。

**文件效果**：`Systems/Native/` 6 → 4（单位相关）／目录总文件 11 → 9；两步累计（`inline-unit-native-sync-registry` + 本变更）单位相关文件 6 → 4。

## 阻塞项（既有，非本次引入）

回归宿主 `Projects/Regression` 仍报 `Executed 0; passed 0; failed 0`（exit 1），与 `inline-unit-native-sync-registry` 阶段一致：该宿主在本分支尚未接通（对应 `repair-audit-findings-on-luomo` 3/31 的"建立可执行回归宿主"任务）。因此 `R0 Direct` 仅覆盖构建、注册顺序表比对与代码等价性比对——**本变更未依赖运行期证据**，因为零行为变更是由 FQN 表逐行一致直接证明的。

**遗留风险**：无新增。既有风险沿用：单位原生投影路径无自动化用例覆盖；order-1 桶仍依赖 FQN 字母序（根因未处理，后续独立变更）。

## 附带差异（已记录，无行为影响）

- 合并文件未保留 `UnitMoveNativeSystem.cs` 中的 `using System.Numerics;`（该文件无 `Vector2/3` 引用，属未使用）。
- `UnitCreateNativeSystem` 未纳入本次合并，原因见 proposal §1：并入 `War3Frame.Systems.Native` 会使其翻到 `UnitNativeSystem` 之后，属顺序契约变更，需独立提案。
