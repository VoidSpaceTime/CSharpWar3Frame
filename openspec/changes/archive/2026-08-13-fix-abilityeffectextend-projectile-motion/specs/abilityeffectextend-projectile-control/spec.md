## ADDED Requirements

### Requirement: Legacy fixed-coordinate projectiles MUST advance toward stored target coordinates
旧版 `AbilityEffectExtend` projectile 系统在没有可用 `TargetEntity` 位置可追踪时，MUST 基于 `ProjectileBase.targetX` 与 `ProjectileBase.targetY` 推进 projectile 的 XY 位置，而 SHALL NOT 使用与目标方向无关的占位式位移。

#### Scenario: Fixed-coordinate projectile advances toward stored target
- **WHEN** 某个 projectile 没有可读取的目标实体位置
- **THEN** 系统 MUST 使用当前 `Position` 与 `targetX/targetY` 计算朝目标的 XY 推进
- **AND** 每次实际移动后 MUST 标记 `ProjectilePositionDirty`

### Requirement: Legacy tracking projectiles MUST refresh target coordinates before arrival and movement checks
旧版 `AbilityEffectExtend` projectile 系统在存在 `TargetEntity` 且其拥有 `Position` 时，MUST 先从目标实体刷新 `targetX/targetY`，再执行距离判断、到达切换与位移推进。

#### Scenario: Tracking projectile follows moving target
- **WHEN** 某个 projectile 持有有效 `TargetEntity`
- **AND** 该目标实体可提供最新 `Position`
- **THEN** 系统 MUST 先刷新 `projectile.targetX` 与 `projectile.targetY`
- **AND** 后续到达判定与 XY 推进 MUST 基于刷新后的目标坐标

### Requirement: Legacy projectile callback cadence MUST remain unchanged during motion split
将旧版 projectile 运动控制拆为固定坐标与追踪目标两种分支时，现有 `ProjectileOnStart`、`ProjectileOnTravel`、`ProjectileOnArrive` 的标签节奏与触发顺序 MUST 保持不变。

#### Scenario: Projectile completes normal lifecycle after motion split
- **WHEN** 某个 projectile 从创建推进到到达
- **THEN** `ProjectileOnStart` MUST 先于持续推进逻辑触发
- **AND** `ProjectileOnTravel` MUST 继续在飞行阶段参与当前逻辑
- **AND** 到达后系统 MUST 仍通过 `ProjectileOnArrive` 进入收尾与删除流程

### Requirement: Legacy arrival threshold and local-only scope MUST remain stable in this change
本次旧版 projectile motion 修正 MUST 保持当前本地到达阈值与局部作用域，不得顺手改写新版 projectile 管线、`ProjectileBase` 结构或高度/Z 语义。

#### Scenario: Motion fix is reviewed for scope boundaries
- **WHEN** 本次 change 被审核或实施
- **THEN** 改动 MUST 限定在旧版 `AbilityEffectExtend/ProjectileSystem` 的运动控制层
- **AND** 它 SHALL NOT 同时改动 `Ability/AbilityEffectSystems.cs` 的新版 projectile 流程
- **AND** 它 SHALL NOT 重新定义 `targetZ`、`height`、`distance`、`startX` 或 `startY` 的业务语义
