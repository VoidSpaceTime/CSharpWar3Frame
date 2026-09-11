# 设计：统一效果结算管线

## 当前上下文

当前能力效果流程大致分两段：

1. Projectile、Area、Effect 系统判断 effect 什么时候可以结算。
2. Damage、Heal、Buff 各自执行最终副作用。

Damage 已经接近目标模型：effect system 只发出 `DamageRequest`，`DamageResolveSystem` 负责扣血、死亡判断和 `DamageEvent`。Heal 和 Buff 仍然在各自 effect system 中直接结算。Aura 则绕过 Buff 系统，直接创建属性 modifier。

## 目标

- Damage / Heal / Buff 都通过一致的 settlement request / outcome 边界表达。
- 保留 `DamageEvent` 作为权威伤害结果。
- Heal 也拥有可监听的结算结果，而不是静默直接修改 Health。
- Buff 和 Aura 复用已有 Buff 生命周期、刷新、堆叠、过期和 `AttrDirty` 语义。
- 尽量保持现有 ability template 和示例声明源码兼容。

## 非目标

- 不重写 projectile、area search 或 casting workflow。
- 不新增 War3 native 调用；本 change 只处理 ECS/runtime 结算语义。
- 不重做完整属性系统。
- 不在本 change 中实现护盾、吸血、治疗加成、减伤链等高级战斗公式。

## 方案

### 1. Effect system 只提交结算意图

`DamageEffectSystem`、`HealEffectSystem`、`BuffEffectSystem` 继续负责：

- 检查 `EffectSettlementHelper.CanSettle(effectEntity)`。
- 根据 `EffectSource`、`EffectTargetInfo` 和 effect data 计算基础 payload。
- 提交 settlement request。
- 标记当前 effect data 已经提交。

这些系统不再长期拥有最终副作用语义。

### 2. Settlement resolver 拥有最终副作用

新增或调整 resolver system 消费 settlement request：

- Damage：继续通过 `DamageRequest`，由 `DamageResolveSystem` 发布 `DamageEvent`。
- Heal：消费 heal request，修改 Health，并发布 heal outcome / event。
- Buff：消费 buff request，调用 `BuffHelper` 或等价 Buff 入口，并发布 buff outcome / event。

实现上可以使用 `HealRequest`、`BuffApplyRequest` 这类分类型 request，也可以使用统一 request 结构；优先推荐分类型 request，因为更符合当前 Friflo ECS 查询风格，字段也更清晰。

### 3. DamageEvent 保持权威

Damage settlement 必须保持以下边界：

- `DamageResolveSystem` 仍是最终扣血和死亡判断 owner。
- 需要监听已结算伤害的系统监听 `DamageEvent`。
- 统一 settlement 不能绕过或替代 `DamageEvent`。

### 4. Heal 新增 outcome

Heal 应新增类似 `HealEvent` 的 outcome：

- 包含 source、target、base heal、final heal、remaining health。
- final heal 至少 clamp 到非负。
- 为后续治疗修正链预留字段/语义空间，但本 change 不实现复杂修正。

### 5. Buff 与 Aura 使用已有 Buff 系统

Buff settlement 通过 `BuffHelper.AddTimedBuff(...)` 或等价 Buff 入口处理。

AuraSystem 不再创建裸 `ModifyValue` / `ModifyTarget`：

- 单位进入光环范围时，创建由 Aura 拥有的 Buff entity。
- 单位离开光环范围时，删除关联 Buff entity，并标记受影响属性 dirty。
- Aura 创建的 Buff 通过 `AuraBuffLink` 或等价关系指回来源 Aura。
- Aura buffId 必须稳定，避免同一个 Aura 意外创建重复 modifier。

## 系统顺序

推荐保持接近现有顺序：

- Projectile / Area Search 先完成目标展开。
- Damage / Heal / Buff effect systems 提交 request。
- Settlement resolver 消费 request。
- Effect lifecycle 只在所有 payload 都提交后删除 completed / expired effect entity。

如果实现阶段发现 resolver 顺序无法保证，需要在任务中补系统注册顺序调整，但不改变 casting workflow。

## 备选方案

- 保留 Heal/Buff 直写，只抽 helper：改动小，但无法提供统一事件边界。
- 所有 payload 放进一个大的 `SettlementRequest`：入口统一，但组件字段会变宽，查询可能不清晰。
- 分类型 request + 共享 settlement helper：最贴合当前 ECS 风格，作为推荐方案。

## 风险与缓解

- 多 payload effect 重复结算：用 `EffectSettlementHelper` 或替代 marker 确保每个 payload 只提交一次。
- Heal 时序变化：用现有 healing wave / item heal 场景验证。
- Aura Buff 残留：验证 Aura 移除和离开范围时的 `AuraBuffLink` 清理与 `AttrDirty`。
- Buff 刷新行为变化：复用 `BuffHelper` 和现有 `BuffRefreshBehavior`，不在 AuraSystem 中引入第二套刷新规则。

## 全局影响

- `War3Frame/`：核心 runtime settlement 和 Buff/Aura 协作会变更。
- `War3Frame.Generator/`：无变更。
- `FrameBuild/`：无变更。
- `CSharpWar3Frame/`：无变更。
- `Projects/`：作为现有 ability/item 模板的验证目标。
