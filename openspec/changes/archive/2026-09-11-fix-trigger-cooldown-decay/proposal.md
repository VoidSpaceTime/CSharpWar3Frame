# 提案：修复触发器 Cooldown 策略永不恢复

## 元信息

- **状态**：已实施
- **等级**：`light`
- **变更 ID**：`fix-trigger-cooldown-decay`
- **日期**：2026-09-11
- **请求来源**：仓库疏漏扫描（ULW 只读审计）
- **默认实施后审查强度**：`R0 Direct`

## 1. 背景与目标

### 背景
`War3Frame/Src/Systems/Trigger/TriggerSystems.cs`：
- `ConsumePolicy` 在命中后设置 `runtime.cooldownRemain = policy.cooldown`（第 197 行）。
- `CanTrigger` 在 `runtime.cooldownRemain > 0f` 时拒绝触发（第 124 行）。
- **全仓无任何位置递减 `cooldownRemain`**（无 `Duration` 组件、无计时系统消费）。

后果：`TriggerPolicyKind.Cooldown` 规则**触发一次后永久锁死**，等价于一次性的 Once，与设计意图不符。

### 目标
让冷却随 tick 递减，冷却结束后规则可再次触发。

## 2. 影响范围

- `War3Frame/Src/Systems/Trigger/TriggerSystems.cs`（新增冷却递减系统或并入）
- 不受影响：`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/` 其余

## 3. 方案摘要

**推荐**：新增 `TriggerCooldownSystem : QuerySystem<TriggerSpec, TriggerRuntime>`，在 `TriggerSystem`（order 131）之前执行（order 取空闲值，建议 `44`，早于 131 且不占用既有 order）。

```csharp
protected override void OnUpdate()
{
    Query.ForEachEntity((ref TriggerSpec spec, ref TriggerRuntime runtime, Entity rule) =>
    {
        if (runtime.cooldownRemain > 0f)
        {
            runtime.cooldownRemain -= Tick.deltaTime;   // ref 原地写，循环内安全
            if (runtime.cooldownRemain < 0f) runtime.cooldownRemain = 0f;
        }
    });
}
```
- 仅 `ref` 字段原地写，符合 Query 循环结构安全规则（见 `fix-query-loop-structural-mutation`）。

**备选**：给规则实体挂 `Duration`，复用 `DurationSystem` 递减 + `DurationExpired` 清冷却。更通用但引入额外组件与清理路径，本变更不采用。

## 4. 风险与回滚

- **风险**：低。递减逻辑独立，不改变其它策略（Once/Count）。
- **回滚**：删除新增系统即可。

## 5. 验收标准

1. `Cooldown(seconds)` 规则在冷却期内不触发、冷却结束后可再次触发。
2. `Once` / `Count` 行为不变。
3. 新增本地场景：注册 Cooldown 规则 → 触发一次 → 推进超过冷却 → 断言可再次触发。
4. 编译 0 error，既有 `TriggerValidationScenario` 无回归。

## 6. 非目标

- 不实现 `Cooldown` 的 UI/剩余时间查询接口（如需要另行提案）。
