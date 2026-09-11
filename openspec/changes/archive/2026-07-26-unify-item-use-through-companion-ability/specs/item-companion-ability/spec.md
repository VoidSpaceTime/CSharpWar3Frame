## Capability: item-companion-ability

### Requirement: Item and Ability remain separate entities

系统 MUST 使用独立 Item entity 和 Ability entity 表达物品与物品能力。Item entity 不得通过添加 AbilityBase 来兼任 Ability entity。

### Requirement: Each active item has at most one companion

对于带有 `ItemUseAbilityData` 的 Item entity，系统 MUST 通过一个 item-to-ability Link 关联最多一个 companion ability。重复绑定 MUST 复用现有 companion，不得创建第二个并行冷却实例。

Item、companion 与 owner MUST 位于同一个 `EntityStore`。`ItemActiveAbility` MUST 以 Item 为 source、companion 为 target；indexed Link 更新 MUST 通过结构变更 API 完成，不得原地修改 target 字段。

### Requirement: Companion mount semantics

Companion ability MUST 具有 `AbilityMountInfo.mountType = AbilityMountType.ItemGranted`，且 MUST NOT 具有 `AbilitySlotIndex`。创建 companion 不得增加 `AbilitySlotContainer.currentCount`。

Companion MUST 以当前 `ItemLevel` 应用存在的 Ability template。模板应用成功前不得写入 Item Link；模板缺失或配置失败时 MUST 删除半创建 companion 且不得派发 Cast。

### Requirement: Ability template is the only active behavior source

`ItemSpec` MUST 通过 `useAbilityTemplateName` 指定主动行为。系统 MUST 删除 Item 层 `useEffectSpec`、`ItemUseEffectData` 和 `UseEffect(...)` builder API，不得保留兼容分支。

### Requirement: ItemUse dispatches Ability

有效 `ItemUseRequest` MUST 经过 Item owner、可使用状态和 target intent 校验后，向 companion ability 发起 `CastRequest`。`ItemUseSystem` MUST NOT 直接创建 Item-origin Effect entity、推进 Ability cooldown 或执行 War3 Native 调用。

同一 Unit 同帧存在多个有效 ItemUse 时 MUST 按 request entity ID 升序采用首个有效请求胜出，其余请求消费并拒绝。已有 `CastRequest`、活动 `CastState` 或 `ChannelState` 时 MUST NOT 被新 ItemUse 覆盖。Cast 消费时 MUST 重新校验 companion owner、Item Link 和 Item pending 状态。

### Requirement: Target compatibility

ItemUse `None` MUST 只兼容 `AbilityTargetType.None`，`Unit` MUST 只兼容 `Unit`，`Point` MUST 兼容 `Point` 或 `Area`。不兼容请求 MUST 被消费且不得创建 CastState 或 Effect。

### Requirement: Target and origin preservation

ItemUse 规范化后的 None、Unit、Point target MUST 能被 Ability Cast workflow 消费。系统 MUST 保留 Item 和 user origin，使 delayed Effect 能在 Effect 创建后继续追溯物品来源。

Item/user origin MUST 从 `CastRequest` 传播到 `CastState`，并在提交时复制到 root Effect、child Effect、arrive Effect 和 ground-area source。解绑 `AbilityOwner` 后，已提交 Effect 不得从新的 owner 或空 owner 反推原始 user。

### Requirement: Companion ownership follows item ownership

当 Item owner 从单位 A 转移到单位 B 时，companion MUST 解除 A 的 `AbilityOwner` 并绑定 B。Item 掉落到地面时，companion MUST 解除单位 owner；重新拾取时 MUST 重新绑定当前 owner。

Owner 转移或掉落前，系统 MUST 按 companion identity 清理旧 Unit 的施法状态：取消 `CastRequest`、`MovingToCast`、未提交的 `Casting` 及对应 `MoveContinuation`，并将 companion 恢复为 `Ready` 且不进入 cooldown；`Channeling` MUST 同时清除 `ChannelState` 和对应 `CastState`、停止后续 tick 并保留已创建 Effect；`Backswing` MUST 立即清除对应 `CastState`。Channeling/Backswing 提前结束后 MUST 按正常 cooldown 值进入 `Cooldown`，或在 cooldown `<= 0` 时进入 `Ready`。该清理 MUST NOT 触发 `OnInterrupted` 或 `OnFinished`，也不得通过通用中断标记影响其他 Ability。

### Requirement: Item deletion cleans companion

框架内 Item 删除 MUST 通过 `ItemDestroyRequest` 进入受控流程。系统 MUST 添加 `ItemDestroyPendingTag`、拒绝后续 ItemUse，并按 owner 变化使用的同一阶段规则清理引用 companion 的施法状态后解除 owner。

系统 MUST 保留 companion 及其 template/stats，直到不存在引用 companion 的 Cast/Move 状态、`EffectSource.ability` 和 `GroundAreaSource.ability`，随后先删除 companion 再删除 Item。系统 MUST NOT 主动取消已提交 projectile/area，也不得因超时强删 companion。

直接调用 `Entity.DeleteEntity()` 删除 Item 属于本能力不支持的旁路；本变更不实现通用引用计数或任意组件引用发现。

### Requirement: Shared stack cooldown

同一个 Item entity/stack 的所有使用请求 MUST 共享其 companion ability 的 cooldown。系统本阶段 MUST NOT 因一次使用自动扣减 stack 或删除 Item，除非后续独立提案批准该行为。

本阶段 MUST NOT 处理 stack split/merge，也 MUST NOT 为 item companion 启用 `OnGranted`、`OnRemoved` 或被动属性贡献。

### Requirement: Casting runtime availability

CastRequest、Casting、Channeling 和 AbilityCooldown 系统 MUST 接入现有系统注册机制并按可验证顺序运行。Ability 生效后 cooldown `<= 0` 时 MUST 直接回到 `AbilityState.Ready`；只有正冷却才进入并停留在 `Cooldown`。

### Requirement: Validation

实现完成后 MUST：

- `War3Frame` Release build 成功；
- `Projects/test` Release build 成功；
- `CSharpWar3Frame.slnx` Release build 成功；
- 旧 `UseEffect` API 和直接 Item Effect 路径无残留引用；
- 测试场景覆盖即时、Unit、Point、Projectile、Area、delayed origin 以及 companion 生命周期；
- 真实 War3 客户端验证单独记录，不得以托管测试替代。
