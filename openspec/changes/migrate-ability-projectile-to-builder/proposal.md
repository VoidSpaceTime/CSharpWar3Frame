## 基本信息

- Change ID: `migrate-ability-projectile-to-builder`
- 提案等级: `light`
- 目标一句话: 将 `Projects/test` 中技能示例统一迁移到 `AbilitySpecBuilder`，并把 Projectile 表达收敛到 `OnCast` 效果链。
- 请求来源: 用户确认“Projectile 也并入 AbilitySpecBuilder”的优化方向并要求落地。

## 分级判定

### 为什么是 light

- 影响范围: 主要影响 `Projects/test/Scripts/Template/Ability.cs` 示例写法，必要时少量补充 `War3Frame/Src/Helpers/AbilitySpecBuilder.cs` 或 `AbilityEffectSpecBuilder.cs` 的便利入口。
- 风险等级: 中低。现有底层 `AbilityEffectSpecBuilder.Projectile(...)` 已能表达弹道，本变更以迁移示例为主。
- 可逆性: 可通过回滚示例模板和 helper 入口恢复旧写法。
- 是否跨项目: 运行时能力位于 `War3Frame/`，消费示例位于 `Projects/test/`；但不改生成器、构建链路或 CLI。
- 是否改公共契约: 优先不改公共数据结构；若补 helper 只增加便利方法，不破坏现有调用。

### 升级触发器检查

- [x] 涉及 `War3Frame/` 与其他项目联动：可能涉及 helper 与示例消费，但不改变运行时核心流程。
- [ ] 涉及 `War3Frame.Generator/` 输出或契约。
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程。
- [ ] 涉及 `CSharpWar3Frame/` 入口行为。
- [x] 涉及 `Projects/` 示例或集成验证行为。
- [ ] 涉及公共 API / 数据结构 / 配置契约：不计划修改；如实现中发现必须修改，应升级提案。
- [ ] 涉及架构边界、目录结构、依赖关系重组。

## 背景与目标

`Ability.cs` 中目前存在三种写法混用：

- 新写法：`AbilitySpecBuilder.Create(...).OnCast(...)`，例如 `fire_blast`。
- 过渡写法：手动 `AbilityBase` + `AbilityHelper.SetEffectSpec(... EffectSpecBuilder.Chain())`。
- 旧写法：手动添加 `ProjectileData` / `AreaSearchData` / `DamageEffectData`，例如 `lava_ball`。

目标是把示例技能统一到 `AbilitySpecBuilder`，并把弹道作为 `OnCast` 效果链中的 `Projectile` step，而不是直接挂旧组件。

## 影响范围

- 模块: 技能 authoring 示例与技能 builder 便利入口。
- 文件:
  - `Projects/test/Scripts/Template/Ability.cs`
  - 必要时: `War3Frame/Src/Helpers/AbilitySpecBuilder.cs`
  - 必要时: `War3Frame/Src/Helpers/AbilityEffectSpecBuilder.cs`
- 不受影响区域:
  - `War3Frame.Generator/`: 不改系统注册或生成输出。
  - `FrameBuild/`: 不改构建编排。
  - `CSharpWar3Frame/`: 不改 CLI。
  - `War3Frame` Native/Execution 层: Projectile 仍只描述 ECS 语义，不新增 War3 原生调用。

## 方案摘要

- 将 `Ability.cs` 内主动技能迁移为 `AbilitySpecBuilder.Create(...).BaseValue(...).OnCast(...).BuildTo(...)`。
- 将 `lava_ball` 的 `ProjectileData` + `AreaSearchData` + `DamageEffectData` 改为 `OnCast(e => e.Projectile(...).Area(...).Damage(...))`。
- 将 `arcane_missile`、`meteor_strike` 等已有 projectile effect 示例改为通过 `AbilitySpecBuilder.OnCast` 配置。
- 将治疗、范围 Buff、地面区域、线形效果等示例也尽量统一到 `OnCast`，减少 `AbilityHelper.SetEffectSpec` 的直接使用。
- 被动/天赋类技能如 `talent_vitality`、`talent_mana_focus` 若当前 `AbilitySpecBuilder` 尚无属性贡献入口，可保留现状或只迁移基础信息；不在本次强行引入属性贡献公共契约。

## 非目标

- 不重写 Projectile runtime、移动、命中或 Native 同步系统。
- 不新增 War3 原生调用。
- 不调整 Source Generator。
- 不改变 `EffectSpec` / `AbilityEffectSpec` 的底层数据结构。
- 不在本次解决被动技能属性贡献 builder 化，除非实现中发现已有稳定入口可复用。

## 风险与回滚

- 风险: 迁移后示例行为与旧组件直挂路径存在差异。
  - 缓解: 优先使用现有 `AbilityEffectSpecBuilder.Projectile/Area/Damage` 等已存在能力，并通过 `Projects/test` 构建验证。
- 风险: 被动技能也强行迁移会扩大公共 API 范围。
  - 缓解: 被动属性贡献入口不在本次目标内，必要时保留旧写法并单独后续提案。
- 回滚方式: 回退 `Ability.cs` 示例迁移和少量 helper 便利入口即可。

## 验收标准

- `Ability.cs` 中主动技能不再手动添加 `AbilityBase` 和直接调用 `AbilityHelper.SetEffectSpec`。
- `lava_ball` 不再直接添加 `ProjectileData`、`AreaSearchData`、`DamageEffectData`。
- Projectile 通过 `AbilitySpecBuilder.OnCast(... Projectile ... )` 表达。
- 不新增 War3 原生调用。
- `dotnet build War3Frame/War3Frame.csproj` 通过。
- `dotnet build Projects/test/test.csproj` 通过。

