# Change ID: fix-abilityeffectextend-projectile-motion

- 提案等级: `light`
- 目标一句话: 修正 `AbilityEffectExtend/ProjectileSystem` 的旧版弹道推进逻辑，并拆分固定坐标与追踪目标两种控制分支。
- 请求来源: 用户要求在 `War3Frame/Src/Systems/AbilityEffectExtend/ProjectileSystem.cs` 中生成两个私有控制函数。

## Why

`War3Frame/Src/Systems/AbilityEffectExtend/ProjectileSystem.cs` 当前存在几个局部但实质性的运动问题：

- 追踪目标分支在读取目标坐标时使用了错误的 entity。
- 距离计算发生在追踪目标坐标刷新之前，追踪模式下到达判定可能基于过期坐标。
- 位移使用 `MovePolar(step, 0f, step)`，没有体现出明确的朝目标 XY 推进语义。

同时，仓库里已经存在一条较新的 `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs` projectile 路线；本次变更需要明确只修正 `AbilityEffectExtend` 下的旧版本地系统，不扩散到新路径。

## Why This Is `light`

- 改动局限于 `War3Frame/` 运行时单模块内部。
- 不改公共接口、不改 generator、不改构建链路、不改项目依赖关系。
- 可回滚性强，行为边界可以通过局部验证确认。
- 由于本次存在两条控制分支和一条并行的新 projectile 管线，因此在 `light` 基础上补一份短 `design.md` 与 `spec delta` 便于审查。

## What Changes

- 在旧版 `ProjectileSystem` 内将弹道推进拆为两个私有控制分支：
  - 固定坐标分支：使用已存储的 `targetX/targetY` 朝目标推进。
  - 追踪目标分支：先从 `TargetEntity` 刷新 `targetX/targetY`，再朝刷新后的目标推进。
- 保留现有 `IProjectileOnStart` / `IProjectileOnTravel` / `IProjectileOnArrive` 的标签触发顺序与生命周期语义。
- 保留当前本地到达阈值 `50f`、`height` / `targetZ` 相关 Z 语义，以及 `ProjectileOnTravel` 返回值的现有含义，不在本次顺手扩展。
- 不改 `AbilityEffectSystems.cs` 中的新 projectile 流程。
- 不改 `ProjectileBase` 结构字段。

## Capabilities

### New Capabilities

- `abilityeffectextend-projectile-control`: 定义旧版 `AbilityEffectExtend` projectile 系统在固定坐标与追踪目标两种模式下的控制要求。

### Modified Capabilities

- 无。

## Impact

- 主要影响：
  - `War3Frame/Src/Systems/AbilityEffectExtend/ProjectileSystem.cs`
  - `openspec/changes/fix-abilityeffectextend-projectile-motion/*`
- 参考但不修改：
  - `War3Frame/Src/Components/AbilityEffectExtend/Projectile.cs`
  - `War3Frame/Library/AbilityEffectExtend/IProjectile.cs`
  - `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs`
  - `War3Frame/Src/Systems/AbilityEffectExtend/ProjecttileArriveSystem.cs`

## Cross-Project Impact Analysis

- `War3Frame/`: **受影响**。旧版 projectile 运行时推进逻辑位于此处。
- `War3Frame.Generator/`: **不受影响**。无 source generator 输出或契约改动。
- `FrameBuild/`: **不受影响**。无构建编排改动。
- `CSharpWar3Frame/`: **不受影响**。无入口项目行为改动。
- `Projects/`: **预计不受影响**。本次不改示例/集成工程；若后续验证发现有测试工程依赖该旧路径，再单独补充。

## Risks And Rollback

- 风险：
  - 旧版 projectile 路径可能与仓库里较新的 effect projectile 路径并存，若误扩大范围，容易把两套语义混在一起。
  - 当前 `height` / `targetZ` / `distance` / `startX` / `startY` 的字段使用并不完整，本次若顺手解释过度，容易引入额外行为变化。
  - `ProjectileOnTravel` 返回 `false` 的现有含义看起来更像“抑制到达”而不是“停止移动”，本次默认保持现状。
- 回滚方式：
  - 仅回滚 `ProjectileSystem.cs` 与本次 change 目录即可恢复。

## Validation

- 代码审查：确认固定坐标与追踪目标分别进入独立私有控制函数。
- 逻辑校验：
  - 固定坐标弹道沿 `targetX/targetY` 推进。
  - 追踪目标弹道先刷新 `TargetEntity` 坐标，再做距离/到达判断与推进。
  - `ProjectileOnStart` / `ProjectileOnTravel` / `ProjectileOnArrive` 触发顺序保持不变。
  - 移动时仍打 `ProjectilePositionDirty`。
- 构建验证：
  - `dotnet build D:\CSharp\CSharpWar3Frame\War3Frame\War3Frame.csproj`
  - 如需补充信心，再执行 `dotnet build D:\CSharp\CSharpWar3Frame\Projects\test\test.csproj`

## Review Request

本提案批准后，实施阶段将只在 `ProjectileSystem.cs` 中落地两个私有控制函数与最小必要的调度调整，不额外扩展到新的 projectile 架构或其他系统。
