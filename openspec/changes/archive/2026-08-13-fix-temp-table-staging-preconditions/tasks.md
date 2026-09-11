## 1. SyncW3xFile one-file implementation

- [ ] 1.1 在 `ProjectResourceToTemp()` 中为首次 bootstrap 与增量刷新统一建立 `.temp/<project>/table` 前置条件
- [ ] 1.2 在访问 `tempTableDir` 的时间比较、删除和复制前处理缺失目录，避免再次抛出 `DirectoryNotFoundException`
- [ ] 1.3 把 table staging 目标固定为 flat `.temp/<project>/table`，不允许出现 `table/table` 嵌套目标

## 2. Downstream compatibility verification

- [ ] 2.1 重跑导致报错的 `BuildMap()` 路径，确认不再在 `SyncW3xFile.cs:93` 因缺失 `.temp/<project>/table` 失败
- [ ] 2.2 检查 `.temp/test/table` 是否直接包含 `misc.ini` 与 `w3i.ini`
- [ ] 2.3 检查 `Run.cs` 复制后 `.temp/<mode>/test/table/w3i.ini` 是否仍存在
- [ ] 2.4 确认 `AssetsBuild.cs` 继续按 `BuildDstPath/table/w3i.ini` 读取，无需任何 consumer 侧改动

## 3. Approval gating

- [ ] 3.1 在 proposal、design、tasks、spec 获得审核批准前，不进入代码实现
- [ ] 3.2 禁止把范围扩展到 `Run.cs`、`AssetsBuild.cs`、template 内容、项目资源内容或广义 staging 重构
