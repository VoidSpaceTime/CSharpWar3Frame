# 设计：Effect 视觉步骤与 Projectile 到达效果链

## 1. 目标

本设计为技能效果链增加“视觉表现 step”，并允许 Projectile 到达后执行嵌套效果链。目标是让视觉特效、弹道、范围筛选和结算逻辑可以组合，同时保持 ECS/Native 分层清晰。

## 2. 核心分层

### 2.1 Authoring 层

- `EffectSpecBuilder` 是低层效果链 Builder。
- `AbilityEffectSpecBuilder` 是技能 authoring 友好包装。
- 新增入口应优先出现在 `EffectSpecBuilder`，再由 `AbilityEffectSpecBuilder` 委托。
- Builder 只构建 `EffectSpec` 数据，不执行 ECS 修改或 native 调用。

### 2.2 语义执行层

- `AbilityEffectHelper` / `AbilityEffectSystems` 负责解释 `EffectSpec` steps。
- 视觉 step 被解释为创建 ECS 视觉特效实体、写 `EffectBase` / `Position` / `EffectAttachment` / key 关联，或写删除请求。
- Damage / Heal / Buff / Area / Line 继续按当前目标上下文执行。

### 2.3 Projectile 层

- `ProjectileSystem` 继续推进弹道运动、轨迹、到达和过期请求。
- `ProjectileLifecycleApplySystem` 在到达时派发到达 hooks，并可触发 nested arrive effect chain。
- Projectile 不直接拥有爆炸视觉、范围伤害等语义；这些属于到达效果链。

### 2.4 Native 层

- `EffectNativeSystem` 仍是唯一创建、同步、动画播放、销毁 War3 原生特效的执行层。
- 新视觉 step 不新增非 Native 层原生调用。
- `EffectHelper` 可以保留基础便捷入口，但只写 ECS 组件、请求或 dirty 标记。

## 3. 数据模型建议

### 3.1 EffectVisualKind

公开命名使用 `EffectVisualKind`：

```csharp
public enum EffectVisualKind
{
    Point,
    TargetPoint,
    AttachCaster,
    AttachTarget,
    AttachOwner,
    AttachEachTarget,
    RemoveByKey
}
```

说明：

- `Point`: 使用显式坐标或当前效果上下文坐标创建视觉。
- `TargetPoint`: 在当前目标点创建视觉，适合范围爆炸。
- `AttachCaster`: 附着到施法者。
- `AttachTarget`: 附着到当前目标。
- `AttachOwner`: 附着到技能、光环、天赋等拥有者。
- `AttachEachTarget`: 对当前目标集中的每个目标附着视觉。
- `RemoveByKey`: 移除由 key 标识的长期视觉。

### 3.2 EffectVisualStepSpec

建议新增视觉 step payload：

```csharp
public struct EffectVisualStepSpec
{
    public EffectVisualKind kind;
    public string model;
    public EffectAttachType attachType;
    public EffectValueSpec duration;
    public string? key;
    public float x;
    public float y;
    public float z;
}
```

长期视觉应使用 `key`，后续清理时通过 owner + key 定位。

### 3.3 Projectile arrive chain

`ProjectileEffectStepSpec` 建议新增 `EffectSpec? arriveEffect` 或等价嵌套结构。Builder 表达为：

```csharp
.Projectile("bomb.mdl", speed: ...)
.OnProjectileArrive(arrive => arrive
    .Effect(...)
    .Area(...)
    .Damage(...))
```

约束：`OnProjectileArrive(...)` 绑定最近一个 Projectile step；若前序没有 Projectile，应在 builder 阶段拒绝或产生清晰错误。

## 4. 执行顺序语义

- Effect chain 按 step 顺序执行。
- `Area` / `Line` 改变当前目标集或空间上下文。
- `Visual` 按当前上下文创建视觉特效：
  - 在 `Area` 前使用，通常表示中心点或目标点爆炸视觉。
  - 在 `Area` 后使用 `AttachEachTarget`，表示对筛选出的每个目标挂特效。
- `Projectile` 创建或推进 projectile 语义；其 `OnProjectileArrive` 不立即执行，而是在 projectile 到达阶段执行。

## 5. 示例

### 5.1 炸弹到达爆炸

```csharp
.OnEffect(e => e
    .Projectile("bomb.mdl", speed: AbilityValue.AbilityStat(AbilityHelper.ProjectileSpeed))
    .OnProjectileArrive(arrive => arrive
        .Effect(EffectVisualKind.TargetPoint, "explosion.mdl", duration: AbilityValue.Constant(1f))
        .Area(TargetFilter.Enemy, radius: AbilityValue.AbilityStat(AbilityHelper.Radius))
        .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount))))
```

### 5.2 命中目标胸口特效

```csharp
.OnEffect(e => e
    .Area(TargetFilter.Enemy, radius: AbilityValue.AbilityStat(AbilityHelper.Radius))
    .Effect(EffectVisualKind.AttachEachTarget, "hit.mdl", attachType: EffectAttachType.Chest, duration: AbilityValue.Constant(0.8f))
    .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount)))
```

### 5.3 天赋翅膀

```csharp
.OnGranted(e => e
    .Effect(EffectVisualKind.AttachOwner, "wings.mdl", attachType: EffectAttachType.Chest, key: "talent_wings", duration: AbilityValue.Constant(-1f)))
.OnRemoved(e => e
    .RemoveEffectByKey("talent_wings"))
```

## 6. Rejected Designs

### 6.1 Area 参数内嵌视觉特效

拒绝原因：`Area` 的职责是筛选目标或空间范围。把视觉参数塞进 `Area` 会混淆“范围选择”和“视觉表现”，也不利于 `Line`、`Projectile arrive`、`OnGranted` 复用。

### 6.2 Projectile 参数自带爆炸特效

拒绝原因：Projectile 应只描述飞行和命中。爆炸视觉、范围伤害、Buff 等属于到达后的效果链，应通过 `OnProjectileArrive` 组合。

### 6.3 EffectHelper 管理长期视觉生命周期

拒绝原因：helper 可以提供薄入口，但长期 owner/key/cleanup 应进入 ECS 数据与系统解释，不应隐藏在 helper 内部。

## 7. 验证与边界

- 可通过 builder spec 结构检查、项目构建和静态 native 调用搜索验证主要边界。
- 不要求真实 War3 环境作为本 change 的验收前提。
- 真实地图中视觉效果显示、挂点字符串是否准确，可作为后续手测项记录。
