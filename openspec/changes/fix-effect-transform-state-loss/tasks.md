# 任务清单：修复特效变换累积状态丢失

## 1. 组件层修改

- [x] `War3Frame/Src/Components/Effects.cs`
  - [x] 新增 `EffectTransform` 结构体（`IComponent`）
    - 字段：`rotateX`, `rotateY`, `rotateZ` (float)
    - 字段：`needsReset` (bool) - 标记是否需要重置矩阵
  - [x] 删除 `EffectTransformRequest` 结构体
  - [x] 删除 `EffectTransformOperation` 枚举
  - [x] `EffectDirtyFlags` 枚举新增 `Transform = 1 << 6` 标志位

## 2. 系统层修改

- [x] `War3Frame/Src/Systems/Native/EffectNativeSystem.cs`
  - [x] 删除 `EffectTransformRequest` 查询和消费逻辑
  - [x] 新增 `EffectTransform` 同步逻辑（在 Dirty 检查块内）
    - 检测 `EffectDirtyFlags.Transform`
    - 如果 `needsReset` 为 true，调用 `YDApi.EXEffectMatReset`
    - 调用 `YDApi.EXEffectMatRotateX/Y/Z` 同步累积角度
    - 重置 `needsReset` 为 false

## 3. Helper 层修改

- [x] `War3Frame/Src/Helpers/EffectHelper.cs`
  - [x] `ResetTransform` / `RotateX` / `RotateY` / `RotateZ` 静态方法：写 `EffectTransform` + 打 Transform Dirty
  - [x] 组件缺失时自动创建（TryGetComponent 模式）
  - [x] 保留现有静态方法作为快捷方式

## 4. 代码审查

- [x] 全仓搜索 `EffectTransformRequest` 使用点，确认零残留
- [x] 验证所有修改方法都自动打了对应 Dirty 标记

## 5. 验证

- [x] `dotnet build War3Frame/War3Frame.csproj` 0 错误
- [x] 运行时验证（`Projects/test`）推迟：War3 客户端验收阶段统一执行
- [x] 补 `specs/effect-transform-state.md`

## 6. 文档

- [x] `openspec/changes/fix-effect-transform-state-loss/summary.md` 总结实施结果