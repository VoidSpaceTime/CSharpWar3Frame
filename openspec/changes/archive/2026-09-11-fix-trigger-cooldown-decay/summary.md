# 总结：修复触发器 Cooldown 策略永不恢复

对应提案：`proposal.md`（`fix-trigger-cooldown-decay`，`light`）
状态：`已实施`
日期：2026-09-11

## 改动

`War3Frame/Src/Systems/Trigger/TriggerSystems.cs`：新增 `TriggerCooldownSystem`：

```csharp
[SystemRegister(SystemKind.Interval, 44)]
public class TriggerCooldownSystem : QuerySystem<TriggerSpec, TriggerRuntime>
{
    protected override void OnUpdate()
    {
        var deltaTime = Tick.deltaTime;
        Query.ForEachEntity((ref TriggerSpec spec, ref TriggerRuntime runtime, Entity ruleEntity) =>
        {
            if (spec.policy.kind != TriggerPolicyKind.Cooldown) return;
            if (runtime.cooldownRemain > 0f)
            {
                runtime.cooldownRemain -= deltaTime;
                if (runtime.cooldownRemain < 0f) runtime.cooldownRemain = 0f;
            }
        });
    }
}
```

- 仅 `ref` 字段原地写，符合 Query 循环结构安全规则（见 `fix-query-loop-structural-mutation`）。
- `order 44`：早于 `TriggerSystem(131)`，同帧读取到已递减的冷却。
- 只影响 `Cooldown` 策略；`Once` / `Count` 行为不变。

## 验证

- 新增 `TriggerCooldownValidationScenario`：
  - 首次触发成功；
  - 冷却期内第二次被拦截；
  - 推进 > 冷却时长后可再次触发（`HealRequest` 计数 1 → 1 → 2）。
- 宿主 runner 执行 `PASS`；既有 9 场景回归全 PASS；编译 0 error。

## 后续

无需后续提案；如需冷却剩余时间查询接口，另行提案。
