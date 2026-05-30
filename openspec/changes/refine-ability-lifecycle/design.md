## 设计目标

本设计把技能释放拆成“请求、前摇、生效、持续吟唱、后摇、结束/打断”几个明确阶段，避免 `OnCast` 同时表示开始施法和技能生效。

## 生命周期阶段

1. `CastRequest`
   - 来源于 UI / AI / 脚本。
   - 检查技能是否存在、状态是否可用、目标和距离是否满足基础条件。
   - 可做资源预检查，但不扣资源。

2. `CastPoint`
   - 表示释放前摇。
   - 单位进入 `AbilityState.Casting`。
   - 被打断时只清理状态，不返还资源。

3. `OnEffect`
   - 前摇完成后的生效点。
   - 重新检查资源、目标和技能状态。
   - 条件通过后扣除资源，创建/触发 `OnEffect` 对应效果链。
   - 条件失败时结束释放并回到可用或失败状态，不触发效果。

4. `Channel`
   - 可选持续吟唱阶段。
   - 记录持续时长、tick 间隔和下一次 tick 时间。
   - `OnChannelTick` 每跳先检查条件，再按配置扣除资源并触发效果链。
   - 被打断只清理状态，不返还已经扣除的资源。

5. `Backswing`
   - 可选释放后摇。
   - 技能已生效，单位处于收招阶段。
   - 第一阶段仅推进计时，不定义复杂取消策略。

6. `Finished`
   - 正常结束后进入冷却。
   - 可选触发 `OnFinished`。

## Authoring API

推荐示例：

```csharp
AbilitySpecBuilder
    .Create("blizzard")
    .Name("暴风雪")
    .TargetType(AbilityTargetType.Point)
    .CastPoint(1.0f)
    .Channel(duration: 6f, tickInterval: 0.5f)
    .Backswing(0.3f)
    .OnEffect(e => e.Area(TargetFilter.EnemyAlive))
    .OnChannelTick(e => e.Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount)))
    .BuildTo(ability, level);
```

## 数据结构建议

- `AbilityBehaviorTrigger`
  - `OnEffect`
  - `OnChannelTick`
  - `OnInterrupted`
  - `OnFinished`
  - 保留 `OnGranted` / `OnRemoved`

- `AbilitySpec`
  - `castPoint`
  - `backswing`
  - `channelDuration`
  - `channelTickInterval`
  - `channelCostPerTick` 或等效显式字段（第一阶段可不启用）

- `CastPhase`
  - `MovingToCast`
  - `Casting`
  - `Channeling`
  - `Backswing`

- `ChannelState`
  - `remaining`
  - `duration`
  - `tickInterval`
  - `tickTimer`
  - `ability`

## 系统边界

- `CastRequestSystem`
  - 处理请求、移动、基础预检查。
  - 不扣资源。

- `CastingSystem`
  - 推进前摇。
  - 前摇完成后执行 `TryCommitEffect`。
  - `TryCommitEffect` 重新检查资源，通过后扣资源并触发 `OnEffect`。

- `ChannelingSystem`
  - 推进持续吟唱和 tick。
  - 每次 tick 调用 `TryCommitChannelTick`。
  - tick 条件通过后触发 `OnChannelTick`。

- `BackswingSystem` 或 `CastingSystem` 内部后摇分支
  - 推进后摇结束。
  - 完成后进入冷却。

## 资源消耗策略

- `CastRequest` 阶段：只预检查，不扣除。
- `OnEffect` 阶段：重新检查并扣除基础消耗。
- `OnChannelTick` 阶段：若配置逐跳消耗，则 tick 条件通过后扣除；否则只触发 tick 效果。
- 打断：不返还资源。

## 迁移策略

1. 修改 enum / builder 命名：`OnCast` -> `OnEffect`。
2. 迁移 `Ability.cs` 示例。
3. 调整施法系统扣费时机。
4. 引入 channel tick 字段和触发点。
5. 构建 `War3Frame` 与 `Projects/test` 验证。
