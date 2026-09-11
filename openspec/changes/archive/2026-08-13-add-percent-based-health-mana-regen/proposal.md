## Why

当前生命与魔法回复系统只读取固定回复属性 `HealthRegen` / `ManaRegen`，而仓库中已经存在 `HealthRegenPercent` / `ManaRegenPercent` 属性定义，但没有纳入实际回复计算。这导致百分比回复属性无法生效，属性模型与运行时行为不一致。

## What Changes

- 为生命回复系统加入 `HealthRegenPercent` 的计算。
- 为魔法回复系统加入 `ManaRegenPercent` 的计算。
- 将生命/魔法最终回复量统一定义为“固定回复 + 基于最大值的百分比回复”。
- 保持现有的 interval 更新节奏与 native dirty 同步方式不变。

## Capabilities

### New Capabilities
- `percent-based-regen`: 定义生命与魔法回复系统如何同时处理固定回复和百分比回复属性。

### Modified Capabilities

## Impact

- 主要影响 `War3Frame/Src/Systems/Unit/HealthSystem.cs` 与 `War3Frame/Src/Systems/Unit/ManaSystem.cs`。
- 不改变属性注册表结构，因为 `HealthRegenPercent` / `ManaRegenPercent` 已经存在于 `AttributeHelper`。
- 不改变 `UnitNativeDirtyFlags`、`UnitNativeSystem`、创建/死亡生命周期或其他属性系统。
