# Spec：特效变换累积状态

## 能力：EffectTransformState

### 目标
特效的累积变换（旋转）是持久 ECS 状态，不是一次性 Request 操作；修改通过 Dirty 同步到原生特效。

### 需求

#### ET-1：状态组件
- `EffectTransform : IComponent` 保存 `rotateX/rotateY/rotateZ` 累积角度与 `needsReset`。
- `EffectDirtyFlags` 含 `Transform` 标志位。

#### ET-2：修改入口
- `EffectHelper.RotateX/Y/Z(entity, delta)`：累加角度 + 打 Transform Dirty；组件缺失时自动创建。
- `EffectHelper.ResetTransform(entity)`：归零 + `needsReset=true` + 打 Dirty。
- 禁止直接 `AddComponent(EffectTransformRequest)`（该类型已删除）。

#### ET-3：Native 同步
- `EffectNativeSystem` 消费 `EffectDirtyFlags.Transform`：`needsReset` 时先 `EXEffectMatReset`，再按累积角度应用 `EXEffectMatRotateX/Y/Z`；同步后清除 Dirty（不清除状态组件）。

### 验收
- 全仓无 `EffectTransformRequest` / `EffectTransformOperation`。
- `EffectHelper.RotateX(90).RotateX(45)` 后 `EffectTransform.rotateX == 135`。
- `dotnet build War3Frame.csproj` 0 错误。