# 提案：修复特效变换累积状态丢失

**状态**：已实施  
**等级**：light  
**提案日期**：2026-08-27

---

## 背景

当前 `EffectTransformRequest` 只携带单次操作（`RotateX(90°)`），执行后即删除，ECS 没有保存累积旋转状态。这导致：
1. 无法查询"当前旋转了多少度"
2. 多次旋转无法累加（`RotateX(90) → RotateX(45)` 结果是 45° 而不是 135°）
3. 违反"ECS 是唯一真相"架构原则

`EffectTransformRequest` 属于历史遗留设计，与当前 `EffectBase + EffectDirty` 的 Dirty-Driven 模式不一致。

---

## 目标

1. 新增 `EffectTransform` 组件保存累积变换状态（旋转 X/Y/Z）
2. 删除 `EffectTransformRequest`（用 `EffectTransform + EffectDirty` 替代）
3. 封装 `EffectModifier.RotateX/Y/Z()` 链式 API，自动累加并打 Dirty 标记
4. 修改 `EffectNativeSystem` 从 `EffectTransform + EffectDirty` 读取并同步到 War3 原生特效

---

## 非目标

- 不改动 `UnitNativeSystem` 的 Compare-Sync 机制
- 不统一所有 Native 同步为 Dirty-Driven（留待后续提案）
- 不修改其他 Effect 属性（Scale/Color/Visible 已是 Dirty-Driven，保持不变）

---

## 影响范围

### 组件层
- **新增**：`War3Frame/Src/Components/Effects.cs` 中 `EffectTransform : IComponent`
- **删除**：`War3Frame/Src/Components/Effects.cs` 中 `EffectTransformRequest : IComponent`
- **修改**：`War3Frame/Src/Components/Effects.cs` 中 `EffectDirtyFlags` 枚举新增 `Transform` 标志位

### 系统层
- **修改**：`War3Frame/Src/Systems/Native/EffectNativeSystem.cs`
  - 删除 `EffectTransformRequest` 消费逻辑
  - 新增 `EffectTransform + EffectDirty.Transform` 同步逻辑

### Helper 层
- **新增**：`War3Frame/Src/Helpers/EffectHelper.cs` 中 `EffectModifier` 结构体
- **新增**：`EffectModifier.RotateX/Y/Z()` 方法（累加旋转 + 打 Dirty）
- **新增**：`EffectModifier.ResetTransform()` 方法（重置累积旋转）
- **修改**：现有 `EffectHelper.SetScale/SetColor/SetVisible` 改为 `EffectModifier` 内部实现

### 使用方式变更
**修改前**：
```csharp
entity.Add(new EffectTransformRequest { 
    operation = EffectTransformOperation.RotateX, 
    value = 90f 
});
```

**修改后**：
```csharp
EffectHelper.Modify(entity).RotateX(90f);  // 自动累加到 EffectTransform.rotateX
```

---

## 风险与回滚

### 风险
1. **API 破坏性变更**：现有代码中使用 `EffectTransformRequest` 的地方需要改用 `EffectModifier`
2. **状态迁移**：运行中的特效实体没有 `EffectTransform` 组件，需要在首次 `RotateX/Y/Z` 时自动创建

### 回滚
1. 恢复 `EffectTransformRequest` 组件定义
2. 恢复 `EffectNativeSystem` 中 Request 消费逻辑
3. 删除 `EffectTransform` 组件和 `EffectModifier` 封装

成本：低（仅 Effect 层，无跨模块依赖）

---

## 验收标准

1. **编译通过**：删除 `EffectTransformRequest` 后无编译错误
2. **LSP 诊断清理**：修改的 5 个文件均无类型错误
3. **运行时验证**（Projects/test）：
   - 创建特效实体
   - 调用 `.RotateX(90).RotateX(45)` 累加旋转
   - 读取 `EffectTransform.rotateX` 验证为 135°
   - 观察 War3 客户端中特效旋转效果正确
4. **代码审查**：所有 `EffectModifier` 方法都自动打了对应 Dirty 标记

---

## 相关文档

- 架构文档：`ARCHITECTURE.md` "War3 原生调用分层规则"
- 组件层：`War3Frame/Src/Components/Effects.cs`
- 系统层：`War3Frame/Src/Systems/Native/EffectNativeSystem.cs`
- Helper 层：`War3Frame/Src/Helpers/EffectHelper.cs`
