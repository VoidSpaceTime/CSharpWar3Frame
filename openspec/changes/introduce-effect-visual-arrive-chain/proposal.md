# 引入 Effect 视觉步骤与 Projectile 到达效果链

## 0. 基本信息

- Change ID: `introduce-effect-visual-arrive-chain`
- 提案等级: `full`
- 目标一句话: 扩展 `EffectSpecBuilder`，支持独立视觉特效步骤与 `Projectile(...).OnProjectileArrive(...)` 嵌套到达效果链。
- 请求来源: 用户希望技能效果可声明绑定胸部/脚/手/武器的视觉特效、指定点爆炸特效、光环/天赋获得后的长期附着特效，并确认采用 Projectile 内嵌 `OnProjectileArrive` 的第一种设计。

## 1. 分级判定

### 1.1 为什么是 full

- 影响范围: 修改 `War3Frame/` 内 effect authoring、effect spec 数据结构、Projectile 到达后的效果链解释逻辑，并可能补充 `Projects/test` 示例。
- 风险等级: 中高；涉及公共 Builder API、效果链执行语义、Projectile 生命周期与视觉特效生命周期协作。
- 可逆性: 中；Builder API 和 spec 结构一旦被示例或下游使用，回滚需要同步调整模板。
- 是否跨项目: 主要实现位于 `War3Frame/`，验证可能触及 `Projects/test`。
- 是否改公共契约: 是；会新增公开 authoring API，例如 `EffectVisualKind`、`Effect(...)`、`OnProjectileArrive(...)`。

### 1.2 升级触发器检查

- [x] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为
- [x] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

## 2. 背景 / Why

当前效果系统已经有两类能力：

- `EffectSpecBuilder`: 描述 Damage / Heal / Buff / Area / Line / GroundArea / Projectile 等技能效果链。
- `EffectHelper` + `EffectRuntimeSystem` + `EffectNativeSystem`: 创建和推进视觉特效实体，并由 Native 层执行 War3 原生特效调用。

问题是视觉特效在技能 authoring 中缺少统一声明入口。典型需求包括：

- 技能命中或范围爆炸时，在目标点创建短生命周期爆炸特效。
- 技能命中目标时，在目标胸部、脚、手、武器等挂载短时特效。
- 光环、天赋、被动获得后，为 owner 长期绑定翅膀、脚底光圈或武器特效，并在移除时清理。
- Projectile 抵达后先生成爆炸视觉，再执行 Area + Damage 等后续效果。

这些需求应当进入效果链 authoring，而不是把视觉逻辑塞入 Projectile 运动本身，也不应让 `EffectHelper` 成为长期语义 owner。

## 3. 变更范围 / What

- 新增视觉效果 step 概念，公开命名使用 `EffectVisualKind`。
- 扩展 `EffectSpecBuilder` / `AbilityEffectSpecBuilder`，支持 `Effect(...)` 与 `RemoveEffectByKey(...)` 类入口。
- 扩展 Projectile step，支持嵌套 `OnProjectileArrive(arrive => ...)` 效果链。
- 运行时在 Projectile 到达阶段解释嵌套效果链，不把到达视觉写死到 Projectile 参数中。
- 保持 `EffectHelper` 为薄 ECS 操作封装，只负责创建/销毁/dirty/动画/变换等基础意图。

## 4. 全局影响分析

- `War3Frame/`: 主要影响 effect spec、builder、Ability effect 执行系统、Projectile 到达处理和视觉特效 ECS 创建。
- `War3Frame.Generator/`: 不预期修改；如果新增系统才需要确认 `[SystemRegister]` 输出，但本提案不要求 generator 契约变更。
- `FrameBuild/`: 不影响构建编排。
- `CSharpWar3Frame/`: 不影响 CLI。
- `Projects/`: 建议补充 `Projects/test` 中的炸弹到达爆炸、命中特效或天赋翅膀示例，用于编译验证。

## 5. 设计要点

- `Area` / `Line` 继续只负责选择目标或空间上下文，不携带视觉特效参数。
- `Visual` 是独立 effect step，可在普通效果链、`OnGranted`、`OnRemoved`、`OnProjectileArrive` 中使用。
- `Projectile` 只负责飞行模型、轨迹、速度、到达/命中条件；到达后发生什么由嵌套效果链声明。
- 推荐 authoring 形态：

```csharp
.OnEffect(e => e
    .Projectile("bomb.mdl", speed: ...)
    .OnProjectileArrive(arrive => arrive
        .Effect(EffectVisualKind.Point, "explosion.mdl", duration: ...)
        .Area(TargetFilter.Enemy, radius: ...)
        .Damage(...)))
```

- 光环/天赋长期视觉应使用 `key` 标识，便于 `OnRemoved` 或后续清理：

```csharp
.OnGranted(e => e
    .Effect(EffectVisualKind.AttachOwner, "wings.mdl", attachType: EffectAttachType.Chest, key: "talent_wings", duration: -1))
.OnRemoved(e => e
    .RemoveEffectByKey("talent_wings"))
```

## 6. 风险、兼容性、迁移

- 风险: 如果 `Visual` 与 `Area` 的执行顺序不清晰，可能造成“先选目标再播视觉”或“先播范围视觉再选目标”的语义误解。
- 风险: 长期附着视觉若没有 `key` 和 owner 关系，移除天赋/光环时难以清理。
- 风险: Projectile 到达链若直接耦合 native 特效，会破坏 ECS/Native 分层。
- 控制: 明确 `Visual` 是效果链 step，执行时只创建 ECS 视觉实体或移除请求；Native 副作用只在 `EffectNativeSystem`。
- 迁移: 现有 `Projectile(...)` 和 Damage/Heal/Buff/Area 调用应继续可用；新能力作为增量入口加入。

## 7. 验证计划

- 构建 `War3Frame/War3Frame.csproj`。
- 构建 `Projects/test/test.csproj`。
- 静态检查 `EffectSpecBuilder`、`AbilityEffectSpecBuilder`、`AbilityEffectSystems` 不直接调用 War3 native。
- 示例编译验证：包含至少一个 Projectile 到达爆炸链和一个长期附着视觉声明。
- 如能添加纯 ECS 测试，优先验证 builder 生成的 spec 结构与 Projectile 到达链语义，不依赖真实 War3 环境。

## 8. 拆分任务

- 见 `tasks.md`。

## 9. 非目标

- 不实现硬件输入。
- 不把视觉特效参数塞入 `Area(...)`。
- 不让 Projectile 系统直接调用 `JassApi` / `KKApi` / `YDApi` / `DzApi`。
- 不让 `EffectHelper` 管理长期视觉生命周期真相。
- 不修改 Source Generator 或构建链路。
