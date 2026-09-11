# Proposal: clean-projectile-data-legacy-fields

**等级**: light
**状态**: 待审核

## 现状

`ProjectileData`（`AbilityEffect.cs:334`）目前对 4 个数值存在双字段：

```csharp
public EffectValueSpec speedValue;       // 新路径
public float speed;                       // 旧 float，注释标注"兼容回退"

public EffectValueSpec arrivalThresholdValue;
public float arrivalThreshold;

public EffectValueSpec maxDistanceValue;
public float maxDistance;

public EffectValueSpec hitRadiusValue;
public float hitRadius;
```

每帧 `NormalizeProjectileDefaults` 为所有飞行中的弹道依次 Resolve 这 4 对字段，
其中 `speed` 的 fallback 链最重：先判断 `legacySpeed > 0f`，再回退到 `AbilityHelper.GetFinalValue`
做一次 stat 实体查询。这是每帧每条弹道都在跑的开销。

旧 `ProjectileBase` / `IProjectileOnStart/Travel/Arrive` 路径已被包裹进
`ProjectileHookBridge`（`AbilityEffectSystems.cs:1425`），不再是独立运行时路径。
Bridge 在 Dispatch 时自己用运行时 `speed` 构造临时 `ProjectileBase`，
不依赖 `ProjectileData` 上的 legacy float 字段。

## 目标

删除 `ProjectileData` 上的 4 个 legacy float 字段，消除 `NormalizeProjectileDefaults`
的双字段 Resolve 逻辑及 lambda 闭包分配。

## 非目标

- 不动 `ProjectileHookBridge` 和 `ProjectileBase`（legacy hook surface 保留）
- 不改弹道运动算法或轨迹类型
- 不改 `EffectValueSpec` 的 authoring 方式

## 影响范围

| 文件 | 操作 |
|------|------|
| `War3Frame/Src/Components/Ability/AbilityEffect.cs` | 删除 `speed`/`arrivalThreshold`/`maxDistance`/`hitRadius` 4 个 float 字段 |
| `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs` | 重写 `NormalizeProjectileDefaults`：直接 Resolve `speedValue` 等，去掉 fallback lambda；更新 `ProjectileHookBridge.DispatchArriveHooks` 读 speed 的方式 |
| `War3Frame/Src/Components/Ability/EffectSpec.cs` | 检查 `ProjectileEffectStepSpec` 是否持有旧字段，清理 |
| `Projects/test` | 检查 template 文件是否用了旧 float 字段，改成 `EffectValueSpec` |

## 迁移规则

- `speed` → 改为 `EffectValueSpec.Constant(value)` 或 `EffectValueSpec.Stat(statId)`
- `arrivalThreshold` → 若未设置则系统默认 30f，可在 Resolve 后的常量路径里保留该默认值
- `maxDistance` / `hitRadius` → 同 speed

## 验收标准

- 构建通过，无 `legacy` / `speed` / `arrivalThreshold` float 字段残留
- `NormalizeProjectileDefaults` 中无 `legacySpeed`、无 fallback lambda、无闭包
- `Projects/test` 弹道模板仍可正常构建
