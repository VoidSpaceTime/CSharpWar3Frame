# Design: fix-abilityeffectextend-projectile-motion

## Context

仓库内当前至少存在两条 projectile 路线：

- `War3Frame/Src/Systems/AbilityEffectExtend/ProjectileSystem.cs`：旧版、标签驱动、依赖 `ProjectileBase`。
- `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs`：较新的 effect-entity projectile 流程。

本次只处理旧版 `AbilityEffectExtend` 路线，目标是把“固定坐标推进”和“追踪目标推进”拆清楚，同时不改现有标签与回调契约。

## Decisions

### 1. 保持 `OnUpdate()` 为调度入口

`OnUpdate()` 继续负责：

- 执行 `ProjectileOnStart` / `ProjectileOnTravel` / `ProjectileOnArrive` 三段标签流。
- 统一处理到达切换与 `ProjectilePositionDirty` 标记。
- 根据 `TargetEntity` 是否存在选择不同控制分支。

### 2. 将运动控制拆为两个私有函数

计划拆分为：

- `ControlFixedCoordinateProjectile(...)`
- `ControlTrackingTargetProjectile(...)`

其中：

- 固定坐标分支只消费当前保存的 `targetX/targetY`。
- 追踪目标分支先尝试从 `projectile.TargetEntity` 读取 `Position`，刷新 `targetX/targetY` 后再推进。

### 3. 只修正 XY 推进，不扩展 Z 语义

当前 `ProjectileBase` 虽然包含 `targetZ` / `height`，但旧系统的主要问题是 XY 方向推进和追踪坐标刷新顺序。本次不重新定义抛物线、高度曲线或贝塞尔轨迹，不新增第二套位置真相。

### 4. 保留现有局部行为边界

本次默认保持以下行为不变：

- 到达阈值仍为 `50f`
- `ProjectileOnTravel` 返回 `false` 的现有含义不重写
- `ProjectileOnStart` / `ProjectileOnTravel` / `ProjectileOnArrive` 标签顺序不变
- `ProjecttileArriveSystem.cs` 空 stub 继续保持 out of scope
- 新版 `Ability/AbilityEffectSystems.cs` projectile 路线不改

## Non-Goals

- 不引入新组件或新系统。
- 不把旧版 projectile 迁移到新版 effect projectile 架构。
- 不统一 `Tick.deltaTime` 与 `Interval` 节奏差异，除非实现时发现必须最小修正才能保持正确推进。
- 不在本次实现贝塞尔/曲线弹道。

## Risk Notes

- 如果实现过程中发现旧版 projectile 路径在其他项目或测试中有额外依赖，需要停止并升级提案范围。
- 如果修正 XY 推进后暴露出 `height` / `targetZ` 的真实业务需求，应单独提案，而不是在本次顺手扩展。
