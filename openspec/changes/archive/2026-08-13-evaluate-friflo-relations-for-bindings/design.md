## 设计原则

本设计不要求把所有 `Entity` 字段都改成 Friflo relation。判断标准是：

1. 是否是长期语义绑定？
2. 是否需要 target 反查 source？
3. target 删除时是否应自动清理 source 上的 link？
4. 是否存在 slot/order/index 等 relation 本身无法表达的额外语义？
5. 是否属于瞬时命令、请求或结算上下文？

## 选择规则

### ILinkComponent

用于 source 到单一 target 的长期 entity link。

适合：

```csharp
struct AbilityOwner : ILinkComponent
{
    public Entity target;
    public Entity GetIndexedValue() => target;
}
```

推荐场景：

- child -> owner
- effect -> attached target
- projectile -> source effect，前提是它是长期运行时语义而非一次性上下文

注意：更新 target 时必须重新 `AddComponent(new Link { target = target })`，不能 ref 修改字段。

### ILinkRelation

用于 source 到多个 target 的长期 entity links。

适合：

```csharp
struct HasAttr : ILinkRelation
{
    public Entity target;
    public Entity GetRelationKey() => target;
}
```

推荐场景：

- owner -> attrs
- owner -> ability stats
- aura source -> buff targets，如果需要显式多目标集合

注意：单 entity 同类型 relation 不宜超过 100 条。

### IRelation<TKey>

用于 entity + 非 entity key 的多条同类型数据。

适合：

- enum / int / string key 的配置或状态。
- 非 entity target 的多项数据。

不适合：

- entity-to-entity 绑定。此类绑定应使用 `ILinkComponent` 或 `ILinkRelation`。

## 不应迁移的默认类别

以下默认保留普通 component / request，除非后续提案证明是长期绑定：

- 伤害/治疗/结算的一次性上下文。
- cast / move / settlement 命令 payload。
- attach request / remove request 命令本身。
- 仅用于单帧或短流程传参的 source / target。
- 带复杂 slot/order/index 语义但没有单独保存 index 的关系。

## 迁移候选分层

### 高优先级候选

- `EffectAttachment`: effect -> target 的长期挂载关系，可能适合 `ILinkComponent`。
- `ProjectileData.effectEntity`: projectile -> effect 的长期运行时关系，需确认生命周期。
- `GroundAreaSource`: ground area -> source ability/effect 的来源关系，需确认是否用于反查。

### 中优先级候选

- Ability / Item slot 当前绑定：已有 `AbilityOwner` / `ItemOwner`，slot index 仍需保留，不应只靠 relation 替代。
- Aura / Buff 多目标关系：已有 `AuraBuffLink`，可评估是否需要 source -> targets 的 `ILinkRelation`。

### 低优先级或不迁移

- `AbilityEffectContext`
- `EffectTargetInfo`
- damage / settlement source-target payload
- cast / move command source-target payload

这些更像瞬时流程上下文，不应因字段是 `Entity` 就迁移。

## 与 War3 分层的关系

relation 只表达 ECS 语义绑定，不执行 native 副作用。

- 业务 workflow 可发出 request 或写入 ECS 语义。
- 关系执行或维护系统可增删 relation。
- Native / Execution 层继续消费 ECS 真相并执行 War3 原生调用。

## 后续迁移策略

1. 先修正文档和 stale comment，使当前关系命名真实可信。
2. 选择一个低风险长期绑定试点，例如 `EffectAttachment`。
3. 为试点补 targeted 验证：incoming links、删除 target 后自动清理、现有 native 表现不变。
4. 再评估 projectile / ground area / slot 关系。
