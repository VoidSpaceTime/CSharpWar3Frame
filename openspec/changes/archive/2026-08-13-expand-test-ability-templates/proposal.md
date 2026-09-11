# 扩充 test 项目技能样板提案

## 0. 基本信息

- Change ID: `expand-test-ability-templates`
- 提案等级: `light`
- 目标一句话: 基于现有技能系统，在 `Projects/test/Scripts/Template/Ability.cs` 中补充更完整的示例技能模板。
- 请求来源: 用户希望“根据现有的技能系统，生成完善几个技能样板”。

## 1. 分级判定

### 1.1 为什么是 `light`

- 影响范围: 仅计划修改 `Projects/test/Scripts/Template/Ability.cs` 示例模板文件。
- 风险等级: 低到中；只新增示例技能定义，不改变框架公共 API、运行时系统、生成器输出或构建链路。
- 可逆性: 可通过删除新增模板类快速回滚。
- 是否跨项目: 否，仅 `Projects/test`。
- 是否改公共契约: 否。

### 1.2 升级触发器检查

- [ ] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为
- [ ] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

命中 `Projects/`，但不涉及跨项目或公共契约，维持 `light`。

## 2. 背景与目标

当前 `Ability.cs` 已有：

- `fire_blast`: 范围伤害样板。
- `lava_ball`: 弹道 + 范围伤害样板。
- `talent_vitality`: 被动属性贡献样板。
- `healing_wave`: 单体治疗样板。

目标是在不改变框架能力的前提下，补充覆盖更多常见技能类型的模板，方便后续开发新功能时参考。

## 3. 影响范围

- 模块: `Projects/test` 示例脚本。
- 文件: `Projects/test/Scripts/Template/Ability.cs`。
- 不受影响区域:
  - `War3Frame/`: 不修改运行时框架和 ECS 系统。
  - `War3Frame.Generator/`: 不修改模板注册生成器或输出契约。
  - `FrameBuild/`: 不修改构建编排。
  - `CSharpWar3Frame/`: 不修改 CLI 入口。
  - `Projects/demo/`: 不修改 demo 项目。

## 4. 方案摘要

拟新增 5 个技能样板，均使用现有组件和 helper：

1. `frost_nova`: 点目标范围伤害 + 减速 Buff，展示 Area + Damage + Buff 组合。
2. `arcane_missile`: 单体追踪弹道伤害，展示 Projectile + Damage 的单目标链路。
3. `battle_shout`: 无目标自我/友方增益 Buff，展示 Buff 型辅助技能。
4. `blink_strike`: 单体突进/瞬移类占位模板，优先用现有 Move settlement 语义表达，不新增 native 调用；若当前 Move effect 结构不足，则降级为说明性模板或不纳入实现。
5. `mana_surge`: 资源恢复/属性类样板，若现有效果系统没有 Mana 恢复 resolver，则改为被动/临时属性贡献样板，避免添加框架能力。

实施时遵循现有技能系统边界：

- 普通数值用 `AbilityHelper.SetBaseValue` 和 `EffectValueSpec`。
- 效果链优先用 `EffectSpecBuilder`。
- 不在模板中直接调用 `JassApi` / `KKApi` / `YDApi` / `DzApi`。
- 不新增框架组件、系统、helper 或 generator 逻辑。
- 如果某个样板无法由现有系统真实表达，则不强行实现假逻辑，改为选择现有系统可真实支持的替代样板。

## 5. 风险与回滚

- 风险: 示例模板可能引用当前效果系统尚未完整支持的组合，导致构建失败或误导后续开发。
- 控制: 只使用已存在类型和已验证路径；不添加 TODO 技能作为“假可用”样板。
- 回滚: 删除新增模板类，恢复 `Ability.cs` 到变更前状态。

## 6. 验收标准

- `Projects/test/Scripts/Template/Ability.cs` 新增多个可编译技能模板。
- 新模板覆盖至少 4 类能力：范围伤害、弹道伤害、Buff/控制、被动/属性贡献或资源类。
- 不新增 War3 native 直接调用。
- 不修改 `War3Frame/`、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`。
- `dotnet build Projects/test/test.csproj` 通过，或若存在预先环境限制，明确说明失败原因。
