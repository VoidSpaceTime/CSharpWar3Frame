# Projectile弹道系统使用说明

## 概述

重构后的弹道系统支持多种弹道类型，并通过接口回调机制实现自定义效果。

## 弹道类型

系统内置了6种弹道类型（通过`TrajectoryType`枚举）：

### 1. Linear - 直线弹道
最基础的弹道类型，投射物沿直线飞向目标点。

### 2. Tracking - 追踪弹道
实时追踪目标单位，如果目标移动，投射物会调整方向。

### 3. Bezier - 贝塞尔曲线弹道
使用三次贝塞尔曲线，投射物沿优美的弧线飞行。
- 自动计算控制点，产生30%高度的弧形轨迹

### 4. Parabolic - 抛物线弹道
模拟重力抛物线，适合投掷类技能。
- 弧高为距离的40%

### 5. Sinusoidal - 蛇形弹道
投射物沿正弦波路径飞行，产生蛇形效果。
- 振幅为距离的15%
- 频率为3个完整波形

### 6. Spiral - 螺旋弹道
投射物沿螺旋路径飞行，半径逐渐收缩。
- 初始半径为距离的20%
- 旋转6圈

## 使用方法

### 创建投射物

```csharp
var projectileEntity = world.CreateEntity();

// 添加基础组件
projectileEntity.Add(new ProjectileBase
{
    trajectoryType = TrajectoryType.Bezier,  // 选择弹道类型
    SourceEntity = casterEntity,
    SourceAbility = abilityEntity,
    TargetEntity = targetEntity,  // 追踪目标（可选）
    targetX = targetPos.x,
    targetY = targetPos.y,
    targetZ = 0,
    speed = 800f,
    height = 100f,
    startX = startPos.x,
    startY = startPos.y
});

// 添加运行时状态组件（必需）
projectileEntity.Add(new ProjectileRuntimeState());

// 添加位置组件
projectileEntity.Add(new Position { x = startPos.x, y = startPos.y, z = 100f });

// 添加生命周期标签
projectileEntity.AddTag<ProjectileOnStart>();
```

### 自定义效果回调

通过技能模板实现接口来添加自定义效果：

```csharp
public class MyCustomAbility : AbilityTemplate, 
    IProjectileOnStart,    // 投射物创建时
    IProjectileOnTravel,   // 投射物移动时（每帧）
    IProjectileOnArrive    // 投射物抵达时
{
    // 投射物创建时触发
    public void ProjectileOnStart(ref ProjectileBase projectile, ref Position position, Entity entity)
    {
        // 例如：播放发射音效
        // 例如：创建发射特效
    }

    // 投射物移动时触发（每帧）
    public bool ProjectileOnTravel(ref ProjectileBase projectile, ref Position position, Entity entity)
    {
        // 例如：检测碰撞
        // 例如：创建拖尾特效
        
        // 返回true：继续飞行
        // 返回false：阻止本帧到达判定
        return true;
    }

    // 投射物抵达时触发
    public void ProjectileOnArrive(ref ProjectileBase projectile, ref Position position, Entity entity)
    {
        // 例如：造成伤害
        // 例如：播放爆炸特效
        // 例如：施加Buff
        
        if (projectile.TargetEntity is { } target)
        {
            // 对目标造成伤害
            DamageHelper.DealDamage(projectile.SourceEntity, target, 100f);
        }
    }
}
```

## 运行时状态

`ProjectileRuntimeState`组件存储弹道计算的临时数据：

- `elapsedTime`: 已飞行时间
- `normalizedProgress`: 归一化进度（0-1）
- `controlPoint1/2`: 贝塞尔曲线控制点
- `phaseOffset`: 相位偏移（用于蛇形等）

这些字段由系统自动管理，通常不需要手动修改。

## 扩展新弹道类型

如需添加新的弹道类型：

1. 在`TrajectoryType`枚举中添加新类型
2. 在`ProjectileSystem.UpdateTrajectory`的switch中添加分支
3. 实现对应的`UpdateXxxTrajectory`方法

示例：

```csharp
// 1. 添加枚举
public enum TrajectoryType
{
    // ... 现有类型
    Zigzag  // 新增：之字形
}

// 2. 在UpdateTrajectory中添加
TrajectoryType.Zigzag => UpdateZigzagTrajectory(ref projectile, ref position, ref runtimeState, step, out dist),

// 3. 实现方法
private bool UpdateZigzagTrajectory(
    ref ProjectileBase projectile,
    ref Position position,
    ref ProjectileRuntimeState runtimeState,
    float step,
    out float dist)
{
    // 实现之字形弹道逻辑
    // ...
}
```

## 注意事项

1. **必须添加`ProjectileRuntimeState`组件**，否则系统无法正常工作
2. **追踪弹道需要设置`TargetEntity`**，否则会退化为直线弹道
3. **弹道类型在创建时设置**，运行时不应修改
4. **到达阈值为50单位**（`ArrivalThreshold`），可根据需要调整
5. **系统更新间隔为0.05秒**（`Interval`），影响弹道平滑度

## 性能考虑

- 所有弹道计算都在struct组件上进行，保持ECS性能优势
- 接口回调通过`AbilityTemplate`调用，避免委托的GC压力
- 运行时状态数据紧凑，缓存友好
