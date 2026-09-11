## 1. 架构边界

Item 与 Ability 是两个独立实体：

```text
Item --ItemActiveAbility--> Companion Ability --AbilityOwner--> Unit
```

- Item 持有 `ItemBase`、`ItemOwner`、背包/装备状态和 `ItemActiveAbility`。
- Companion 持有 `AbilityBase`、`AbilityRuntime`、模板产生的 flow/effect 配置、`AbilityMountInfo(ItemGranted)` 和可选的 `AbilityOwner`。
- Companion 不持有 `AbilitySlotIndex`，不增加单位的普通技能槽计数。
- 一个 Item entity/stack 最多关联一个 companion ability；该 companion 的 cooldown 对整个 Item entity/stack 生效。
- `ItemActiveAbility` 是 source=Item、target=companion 的 `ILinkComponent`；`AbilityOwner` 是 source=companion、target=Unit 的 `ILinkComponent`。
- Item、companion 与 owner 必须位于同一个 `EntityStore`。indexed Link 更新必须使用 `AddComponent(new Link(...))`，不得原地修改 target 字段。
- 删除 companion 时 Friflo 会清理指向它的 Item Link；删除 Item 不会自动级联删除 companion，因此 Item 删除必须使用本变更定义的受控入口。

## 2. Authoring 模型

`ItemSpec` 保留 `useAbilityTemplateName`，删除 `useEffectSpec`。Item 模板只能声明“使用哪个 Ability template”，而不能声明第二种独立 Effect 执行模型。

Ability template 负责定义：

- target type；
- cast/channel/backswing/cooldown；
- cost 与 range；
- flow 节点和最终 `EffectSpec`；
- 简单即时效果的零时长参数。

`ItemSpecBuilder.UseAbility(...)` 保留并成为唯一主动使用配置入口。`UseEffect(...)`、`ItemUseEffectData` 和 `ItemEffectSpecRuntimeHelper` 的 ItemUse 专用分支删除。

## 3. Companion 实例化

新增一个职责明确的 helper 或 factory，用于：

1. 校验 Item 仍有效且带有 `ItemUseAbilityData`。
2. 在 Item 所属 `EntityStore` 中创建 Ability 基础实体，以当前 `ItemLevel` 作为 companion level。
3. 校验 Ability template 存在并应用模板；该步骤不能依赖 slot helper 的槽位副作用。
4. 设置 `AbilityMountInfo.mountType = ItemGranted`。
5. 设置 `AbilityOwner` 为当前 Item owner（若存在）。
6. 模板和配置全部成功后，在 Item 上写入 `ItemActiveAbility` Link。

模板不存在、配置抛错或任一步骤失败时，入口必须删除半创建 Ability 且不写 Link、不派发 Cast。Item 等级变化时，companion 必须通过明确的同步入口重新应用对应 level，不能静默保留旧等级数据。该入口不得直接调用 War3 Native，也不得持有长期业务状态之外的原生句柄。

本阶段禁止 item companion template 产生 `OnGranted`、`OnRemoved` 或被动属性贡献；这些 Mount 生命周期行为需要独立提案后才能开放。

## 4. ItemUse 工作流

`ItemUseSystem` 保留当前请求收集和最终删除模式，但 `Process` 改为：

1. 校验 user、item、ItemOwner、ItemBase、Item 状态和 target intent。
2. 获取或创建 item companion。
3. 将规范化后的 target 按 Ability target type 兼容矩阵转换成 `CastRequest`。
4. 将 Item identity/user identity 写入 Cast origin，并随状态机传入 Effect 上下文。
5. 无论校验或派发成功与否，清理 ItemUse request。

ItemUseSystem 不创建 Effect entity，不计算 EffectSpec snapshot，不推进 Ability cooldown，也不直接调用 Native。

### 请求仲裁与目标矩阵

- `CastRequest` 是 Unit 上的单组件；同一 Unit 同帧出现多个有效 ItemUse 时，按 ItemUse request entity ID 升序采用“首个有效请求胜出”，其余请求被消费并拒绝，不覆盖已有请求。
- Unit 已存在 `CastRequest`、活动 `CastState` 或 `ChannelState` 时，新 ItemUse 不得覆盖当前施法。
- `None` 只兼容 `AbilityTargetType.None`；`Unit` 只兼容 `Unit`；`Point` 兼容 `Point` 或 `Area`。
- Cast 消费时必须重新校验 `AbilityOwner.owner == caster`、companion 仍由目标 Item Link 持有，且 Item 不处于删除等待状态。

### Origin 数据流

新增明确的 item cast origin 数据契约，至少包含 `item` 与使用时的 `user`：

```text
ItemUseRequest -> CastRequest -> CastState -> root Effect -> child/arrive/ground-area Effect
```

- origin 在 Effect 提交时复制为现有 `ItemEffectOrigin`，不能在延迟结算时从当前 `AbilityOwner` 反推 user。
- root Effect、子 Effect、arrive Effect 和 ground-area source 必须传播相同 origin。
- Cast 被拒绝或在提交前取消时，origin 随对应 CastRequest/CastState 一起清理。

## 5. 生命周期

### Owner 绑定

- Item 被添加到单位背包或装备状态且拥有 `ItemUseAbilityData` 时，确保 companion 存在并绑定 `AbilityOwner`。
- 已存在 companion 时只更新 owner，不重复创建。

### Owner 转移

- Item 从单位 A 转移到单位 B 时，先按 ability identity 执行下述施法阶段清理，再解除旧 `AbilityOwner` 并绑定 B。
- Item 的 companion link 始终指向同一 Ability entity，除非该 Item entity 被删除。

### 掉落

- Item 进入 `ItemGroundTag` 前，按 ability identity 执行下述施法阶段清理，再解除单位 `AbilityOwner`。
- companion 保留并继续由 Item link 持有，以便重新拾取时复用其运行时冷却；是否保留冷却由本阶段统一采用“保留”规则。

### 受控删除

本变更新增 `ItemDestroyRequest`、`ItemDestroyPendingTag` 与 `ItemCompanionDeferredDeleteSystem`，定义框架内受控删除入口：

1. 消费删除请求并添加 `ItemDestroyPendingTag`，ItemUse 从此拒绝该 Item。
2. 解除 Item slot/owner，并按 companion identity 执行下述施法阶段清理。
3. 解除 companion 的 `AbilityOwner`，但保留 `ItemActiveAbility`、companion template 和 stats。
4. 等待不存在引用 companion 的 Cast/Move 状态、`EffectSource.ability` 和 `GroundAreaSource.ability`。
5. 先删除 companion，再删除 Item。

已提交 projectile、area 和 settlement 不主动取消。永久或自定义 Effect 若始终保留 companion source，Item 将保持 pending；本阶段不提供超时强删或通用引用计数。调用方直接执行 `Entity.DeleteEntity()` 属于不受支持的生命周期旁路，框架保证仅覆盖 `ItemDestroyRequest`。

### 施法阶段清理规则

Owner 转移、掉落和受控删除采用同一规则，且只处理引用目标 companion 的状态：

- `CastRequest`、`MovingToCast`、`effectCommitted == false` 的 `Casting`：取消请求/状态及对应 `MoveContinuation`，不创建 Effect；companion 恢复 `Ready`，不进入 cooldown。
- `Channeling`：保留已经提交和创建的 Effect，同时移除该 companion 的 `ChannelState` 与对应 `CastState`，停止后续 channel tick；随后按正常 cooldown 值进入 `Cooldown`，或在 cooldown `<= 0` 时进入 `Ready`。
- `Backswing`：立即移除该 companion 的 `CastState`，不等待自然结束；随后按正常 cooldown 值进入 `Cooldown`，或在 cooldown `<= 0` 时进入 `Ready`。
- 上述生命周期清理均不得触发 `OnInterrupted` 或 `OnFinished`，避免 owner 变化或删除流程创建新的行为 Effect。
- 已提交的 root/child/projectile/ground-area Effect 不受上述状态清理影响，继续进入延迟删除 gate。

## 6. Casting 运行时前置修正

- 为 `CastRequestSystem`、`CastingSystem`、`ChannelingSystem` 和 `AbilityCooldownSystem` 接入现有 `SystemRegisterAttribute`，明确执行顺序并进入生成的运行时 Root。
- Ability 在生效后若 cooldown `<= 0`，必须直接回到 `AbilityState.Ready`；正冷却才进入 `Cooldown` 并由 cooldown system 推进。
- `ItemCompanionDeferredDeleteSystem` 必须运行在 Effect 和 ground-area 生命周期清理之后，避免同帧提前删除 companion。

## 7. 与既有四层 Ability 架构的关系

- Mount：`ItemGranted` 和 `ItemActiveAbility` 表达能力来源。
- Trigger：ItemUse 只产生 `ActiveCast` 触发。
- Flow：继续使用现有 Casting/Effect/Projectile/Area 等执行流。
- Settlement：继续使用现有 Damage/Heal/Buff 等结算层。

本变更不新增完整 flow DSL，也不把 ItemUse 变成 settlement owner。

## 8. 方案约束与边界

- 不修改 `War3Frame.Generator` 代码或生成契约，但新增系统注册会改变下游生成内容。
- 不增加 UI 或普通技能槽映射。
- 不在本阶段处理 consumable stack mutation；`isConsumable` 继续由后续独立提案负责。
- 不建立 `UseEffect` 兼容层，旧模板必须迁移到 Ability template。
- 不处理 stack split/merge；未来拆分出的 Item entity 必须由后续流程获得独立 companion。

## 9. 迁移顺序

1. 增加关系、factory、Cast origin 和受控删除数据契约。
2. 注册 Casting/Cooldown 系统并修复零冷却状态转换。
3. 将 `ItemUseEffectSystem` 重命名为 `ItemUseSystem`，改走 companion cast。
4. 迁移所有模板和场景到 Ability template。
5. 删除旧字段、组件、helper 分支和测试。
6. 扫描旧符号并执行解决方案级构建、场景验证和复审。
