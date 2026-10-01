# 提案：无敌/免疫期间施加的控制被吸收（贡献计 0）

## 元信息

- **Change ID**：`absorb-control-applied-while-immune`
- **等级**：`light`
- **状态**：`已实施`（用户 2026-09-17 明确决策：被吸收、贡献 +0；无敌与免疫都吸收）
- **日期**：2026-09-17
- **默认实施后审查强度**：`R0 Direct`（构建 + 回归宿主）
- **命中升级触发器**：无新增公共契约签名；改变控制施加的行为语义 → 保持 `R0`，由回归用例锁定

---

## 背景

### 现状（已核实）

控制属性（Stun / Silence / NoAttack / Root / CrackFly）的贡献由两条独立路径写入 `ModifyValue`：

| 路径 | 入口 | 覆盖场景 |
|---|---|---|
| Buff | `BuffHelper.CreateBuffInternal`（直接建 `ModifyValue`） | `BuffHelper.Stun/Root/Silence`、`EffectChainBuilder.Stun/Root/Silence`、模板 `.Buff(..., AttributeHelper.Stun, ...)` |
| 属性贡献 | `ModifyHelper.AddModifier`（`AddModifierToUnit` 为公开入口） | 技能/物品的属性贡献 |

无敌与免疫目前**只压制读取**（`ControlHelper.GetEffectiveValue`）：Invulnerable > 0 或对应免疫属性 > 0 时返回 0，但属性 `finalValue` 照常累加。

### 缺陷

无敌期间施加的眩晕等控制会被**完整保留**在属性上，无敌结束（buff 未过期）时立刻生效——表现为"延迟引爆"：无敌一消失，单位突然被暂停且没有对应的新施加动作。

## 目标

1. 目标处于无敌（`Invulnerable > 0`）或持有对应免疫属性（`StunImmunity` 等）时，**新施加**的控制贡献计 0，不产生任何效果。
2. 无敌结束后，被吸收的控制**不会**延迟生效。
3. **已施加的不变更**：无敌/免疫生效前已存在的控制贡献保留原值与时长的既有语义（无敌期间不生效，无敌结束后恢复）——`GetEffectiveValue` 的读取压制保持不变。
4. 覆盖两条写入路径，避免只堵一条。

## 非目标

- 不回收/清理已存在的控制贡献。
- 不改变 `ControlHelper.GetEffectiveValue` 的读取压制语义（`DamagePipelineValidationScenario` phase8 等既有断言不变）。
- 不改变 buff 刷新/叠层路径（`RefreshCore` 对既有 `ModifyValue.value` 的改动）——本次只覆盖"新创建贡献"。
- 不新增 `BuffHelper.Pause` 便捷入口（用户明确不做）。
- 不引入递减、韧性或免疫穿透规则。

## 影响范围

| 区域 | 影响 | 说明 |
|---|---|---|
| `War3Frame/Src/Helpers/ControlHelper.cs` | 修改 | 新增 `IsControlAttr` / `ShouldAbsorbControl` 判定入口（复用既有 `ControlAttrs` 权威表与 `GetImmunityAttrId`） |
| `War3Frame/Src/Helpers/BuffHelper.cs` | 修改 | `CreateBuffInternal` 按吸收判定把 `ModifyValue.value` 与 `BuffStacks` 每层值降为 0 |
| `War3Frame/Src/Helpers/ModifyHelper.cs` | 修改 | `AddModifier` 内按 `AttrTypeId` + `AttrOwner` 解析目标单位，控制属性命中吸收则写 0 |
| `Projects/Regression/DomainRegression.cs` | 修改 | 新增 `absorb-control-under-immune` 用例 |
| `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/` | 不受影响 | 无生成器契约、构建链、CLI 改动 |
| `Projects/test` | 不受影响 | 既有场景使用本地私有 `AddModifier`（不经过 `ModifyHelper`），语义不变 |

## 方案摘要

```csharp
// ControlHelper：施加时吸收判定（读取压制仍由 GetEffectiveValue 负责）
public static bool ShouldAbsorbControl(Entity unit, int attrTypeId)
{
    if (!IsControlAttr(attrTypeId)) return false;          // 只对控制属性生效
    if (GetAttrValue(unit, AttributeHelper.Invulnerable) > 0) return true;
    var immunity = GetImmunityAttrId(attrTypeId);
    return immunity.HasValue && GetAttrValue(unit, immunity.Value) > 0;
}
```

- `BuffHelper.CreateBuffInternal`：`BuffKind.Attribute` 时 `value = ShouldAbsorbControl ? 0f : spec.value`，`ModifyValue` 与 `BuffStacks` 统一用该值。
- `ModifyHelper.AddModifier`：从 `attrEntity` 取 `AttrTypeId` 与 `AttrOwner.owner`，命中吸收则写 0；非控制属性或解析失败原样返回。

两处都走 `ControlHelper` 单一判定，避免规则分裂。

## 风险与回滚

- **风险 1**：`ModifyHelper.AddModifier` 是通用原语。缓解：以 `IsControlAttr` 严格守门，非控制属性零影响。
- **风险 2**：无敌期间叠加多次控制只计一次 0 贡献，无敌结束后不留残余——符合预期，但会让"无敌期间的控制"在 UI/时长上不可见（buff 实体仍存在）。
- **回滚**：删除三处判定调用即可，无数据结构变更、无持久化影响。

## 验收标准

1. 无敌期间施加 Stun → `finalValue(Stun) == 0`；移除无敌后仍为 0（不延迟生效）。
2. 无无敌/免疫时施加 Stun → `finalValue == 1`（正常路径未破坏）。
3. 持有 `StunImmunity` 时施加 Stun → 0；移除免疫后仍为 0。
4. 先施加 Stun（=1）再施加 Invulnerable → `finalValue == 1` 且 `GetEffectiveValue == 0`；移除 Invulnerable → effective 恢复 1（已施加的不变更）。
5. 无敌期间施加 Stun 不产生 `ControlStateNativeRequest(Pause, entered: true)`。
6. Buff 路径（`BuffHelper.Stun`）与属性贡献路径（`ModifyHelper.AddModifierToUnit`）行为一致。
7. `dotnet build War3Frame` 0 error；`Projects/test` 0 error；回归宿主全绿。
