## 0. 基本信息

- Change ID: `unify-item-use-through-companion-ability`
- 提案等级: `architecture`
- 目标一句话: 将所有主动物品统一建模为物品持有的 companion ability，并让 ItemUse 复用既有 Casting/Effect 流程。
- 请求来源: 用户决定取消 Item `UseEffect`，统一只使用 Ability。

## 1. 背景与目标

当前 ItemUse 同时存在 `UseAbility` 与 `UseEffect` 两条运行时路径。两条路径分别处理目标校验、来源追踪、效果创建和后续施法语义，导致简单物品和复杂技能无法共享冷却、施法阶段、模板配置与统一结算流程。

本变更将 Item 与 Ability 保持为两个独立实体：Item 只表达物品身份、拥有者和物品状态；每个可使用 Item 关联一个 item-granted companion ability。ItemUse 校验通过后只负责发起该 ability 的 `CastRequest`，实际目标、施法阶段、冷却和 EffectSpec 均由既有 Ability workflow 负责。

## 2. 架构目标与非目标

### 目标

- 删除 Item 层 `UseEffect` 公共配置、组件和直接执行路径。
- 保留 Item 与 Ability 的独立实体边界。
- 建立 item -> companion ability 的单向 Link 关系，并让 companion ability 通过 `AbilityOwner` 归属当前单位。
- 使用 `AbilityMountType.ItemGranted` 表达物品赋予能力，不占普通技能槽。
- 简单物品效果也通过零前摇、零引导、零后摇的即时 Ability 执行。
- 保留 item/user origin，使延迟 Effect 能正确追溯来源。

### 非目标

- 本变更不引入 UI、技能栏显示或新的 War3 Native 调用。
- 本变更不实现由物品使用触发的消费、堆叠扣减、自动删除或网络/token/outcome 同步；显式 Item 删除仅走本变更定义的受控删除入口。
- 本变更不处理 stack split/merge；一个 Item entity/stack 始终对应一个 companion 和一份共享冷却。
- 本变更不重写完整 Mount/Trigger/Flow/Settlement DSL，只将 ItemUse 接入既有 Ability flow。
- 本变更不修改 `War3Frame.Generator`、`FrameBuild` 或 `CSharpWar3Frame` 的行为。

## 3. 方案比较

### 方案 A：Item entity 直接兼任 Ability entity

不采用。这样会把物品身份、物品状态、技能施法状态和技能所有权混在同一实体中，破坏 Item/Ability 的生命周期边界，也会使一个物品同时参与 ItemSlot 与 AbilityOwner 查询。

### 方案 B：Item 继续直接执行 EffectSpec

不采用。该方案实现成本最低，但会保留第二套目标、冷却、施法和来源语义，继续扩大 ItemUse 与 CastingSystem 的分叉。

### 方案 C：Item 指向独立 companion ability

采用。Item 保留物品语义，Ability 保留施法语义；复杂能力与简单即时效果统一进入 Casting/Effect pipeline。Item 与 companion 的 Link 关系也能明确表达转移、掉落和删除边界。

## 4. 影响范围与全局分析

- `War3Frame/`：受影响。修改 Item authoring/runtime、ItemUse workflow、Ability companion 创建和生命周期辅助逻辑。
- `War3Frame.Generator/`：生成器代码与生成契约不变；Casting/Cooldown 系统接入 `SystemRegisterAttribute` 后，下游系统注册生成内容会变化。
- `FrameBuild/`：不受影响。不改变构建、发布、JIT/AOT 或地图资源流程。
- `CSharpWar3Frame/`：不受影响。不改变 CLI 入口和配置。
- `Projects/`：受影响。现有 ItemUse 场景需要改为验证 companion ability 的 Casting/Effect 路径。

## 5. 阶段、迁移与回滚

### 阶段拆分

1. 增加 item companion 关系和原子化的 companion 实例化入口。
2. 接通 Casting/Cooldown 系统注册，并修复零冷却回到 Ready 的状态机边界。
3. 将 `ItemUseEffectSystem` 重命名为 `ItemUseSystem`，改为校验并发起 `CastRequest`。
4. 删除 `UseEffect` authoring/runtime API，并迁移模板和场景。
5. 补充 Item 转移、掉落、受控删除以及跨帧 Cast/Effect 引用清理。
6. 构建、静态扫描、运行集成场景，并进行代码复审。

### 迁移策略

- `ItemSpec` 仅保留 `useAbilityTemplateName`，模板中的效果通过 Ability template 配置。
- `ItemSpecBuilder.UseEffect(...)`、`ItemUseEffectData` 和 `useEffectSpec` 直接删除，不保留兼容 shim。
- 物品首次获得有效 owner 时创建或绑定 companion；同一 Item entity/stack 只允许一个 companion。
- companion 在 Item 所属 `EntityStore` 中创建，以当前 `ItemLevel` 应用 Ability template；模板失败时删除半成品且不写 Link。
- companion 的 owner 随 Item owner 迁移；掉落或无 owner 时解除 `AbilityOwner` 并保留 companion，重新拾取时重新绑定。
- Item 删除统一经过受控删除请求：先禁止新使用、取消引用该 companion 的未提交施法，再等待已提交 Effect/GroundArea 完成后删除 companion 与 Item。

### 回滚策略

- 在正式合并前，通过回滚本 change 的运行时代码和模板迁移恢复现有 Revision 2 ItemUse 路径。
- 不引入数据库或地图持久化格式迁移，因此回滚只涉及源码和测试场景。
- 不保留运行时兼容 API；回滚必须整体回退本变更，而不是在新旧路径间长期并行。

### 长期维护影响

- Companion 创建、owner 迁移、掉落和删除必须统一经过本变更建立的生命周期入口，后续系统不得直接拼装或修改部分关系。
- 未来 consumable、stack split/merge 和 UI 同步必须复用 companion 生命周期契约，不得绕过唯一性、共享冷却和受控删除规则。
- 直接调用 `Entity.DeleteEntity()` 删除 Item 不在框架保证范围内；若未来需要拦截任意删除，必须通过独立生命周期治理提案扩展。

## 6. 风险与验收

### 风险

- Item owner 转移与 companion owner 更新时可能触发 Friflo relation 结构变更异常。
- 同一 Item entity/stack 的 companion 生命周期若不唯一，可能造成重复冷却或重复施法。
- 删除 `UseEffect` 会影响既有模板和测试编译，必须完成全仓库旧符号扫描。
- 当前 Casting/Cooldown 系统未接入生成注册，且零冷却会停留在 Cooldown；两项都必须作为本变更的运行时前置修复。
- 已提交的延迟 Effect 会继续读取 companion template/stats，Item 删除必须采用受控延迟销毁。

### 验收标准

- ItemUse 不再直接调用 `CreateItemEffectEntity`。
- `ItemSpec`、Builder 和 runtime 中不再存在 `UseEffect` 相关公共契约。
- 主动物品统一产生 Ability `CastRequest`，并复用 CastingSystem 和现有 Effect pipeline。
- Casting/Channeling/Cooldown 系统已注册，零冷却 Ability 完成后立即回到 Ready。
- companion 不占 `AbilitySlotContainer.currentCount`，且不具有 `AbilitySlotIndex`。
- Item owner、掉落、重新拾取和删除场景没有重复 companion 或悬空 owner link。
- Cast 状态按阶段和 companion identity 清理：提交前取消、Channeling 停止后续 tick、Backswing 立即结束；已提交 projectile/ground-area 完成前不会销毁 companion。
- item/user origin 从 CastRequest 传播至 root/derived Effect。
- 解决方案、`War3Frame` 与 `Projects/test` Release 构建通过；真实 War3 客户端场景作为独立验收项记录。

## 7. 实施前置

本提案及其 `design.md`、`tasks.md`、`specs/item-companion-ability/spec.md` 必须先经用户正式审核批准，批准前不得修改运行时代码。
