# Proposal: wire-attribute-stat-calculation-systems

**等级**: full
**状态**: 待审核

## 0. 基本信息

- Change ID: `wire-attribute-stat-calculation-systems`
- 目标一句话: 为 `AttrCalculationSystem` 与 `AbilityStatCalculationSystem` 补上 `[SystemRegister]` 接线并修复查询内结构变更，恢复属性/技能数值重算主链路。
- 请求来源: 仓库结构方向审查（两个计算系统未注册，`AttrDirty`/`AbilityStatDirty` 标签无人消费）。
- 默认实施后审查强度: `R2 Targeted`
- 命中的审查升级触发器: 核心业务流程、跨系统状态协作
- 最终实施后审查强度: `R2 Targeted`
- Oracle 可用性与 `R1` 回退方式: 不适用（`full` 默认 `R2`）
- 完整 `review-work` 授权来源: 无

### 0.1 工件矩阵

`full`：proposal / design / tasks / specs/attribute-stat-calculation/spec.md

### 0.2 总结深度矩阵

`full`：完整总结，覆盖改动范围、全局影响、验证覆盖、风险与后续建议。

### 0.3 实施后审查强度矩阵

命中"核心业务流程""跨系统状态协作"，至少 `R2 Targeted`，需覆盖属性计算时序正确性、查询结构变更安全、未接线系统清单完整性三个视角。

## 1. 分级判定

### 1.1 为什么是这个等级

- 影响范围: `War3Frame` 的 `AttrCalculationSystem`、`AbilityStatCalculationSystem`，及其消费方（`UnitNativeSystem`、Buff/Aura/Item/效果结算链路）
- 风险等级: 中高——接线后属性/技能数值从"从不更新"变为"实时重算"，影响伤害、治疗、Buff、原生同步等核心数值链路
- 可逆性: 高——改动集中在两个系统文件与注册标注，可单独回滚
- 是否跨项目: 否——仅 `War3Frame/` 运行时框架内
- 是否改公共契约: 是——`AttrCalculationSystem`/`AbilityStatCalculationSystem` 从"未注册类"变为"可运行系统"，是框架对外运行行为的变化

### 1.2 升级触发器检查

- [x] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为
- [x] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

### 1.3 实施后审查升级触发器

- [ ] 公共 API、生成器输出、配置格式、构建链或发布契约
- [ ] 持久化、迁移、数据兼容性或数据丢失风险
- [x] 性能回归、资源泄漏、实时性或大规模数据影响
- [x] 多系统、多项目或跨边界状态协作

### 1.4 工具授权与回退

- [x] `R2` 的视角与 verdict 已记录，不按代理数量计数
- [x] 未因 `R3` 自动启用完整 `review-work`
- [x] 完整 `review-work` 无授权来源

## 2. 背景 / Why

### 2.1 属性计算系统未注册

`War3Frame/Src/Systems/AttrCalculationSystem.cs` 定义了完整的属性重算逻辑：

```csharp
public class AttrCalculationSystem : QuerySystem<AttrValue>
{
    public AttrCalculationSystem()
    {
        Filter.AnyTags(Tags.Get<AttrDirty>());
    }
    // (base + flat) × (1 + percentAdd) × percentMul 主公式
}
```

但**没有 `[SystemRegister]` 标注**，生成器不会注册它。全仓 `AttrDirty` 标签有 10+ 处写入方（`BuffHelper`、`AuraHelper`、`ModifyHelper`、`BuffSystem`、`AuraSystem`、`LevelExperienceSystem`、`AbilityEffectSystems` 等），但**没有任何消费方**。

**后果**：属性 Modifier 体系的最终值 `AttrValue.finalValue` 一旦被修改器标记 dirty，永远不会重算。伤害、治疗、Buff 属性、原生血蓝同步（`UnitNativeSyncRegistry` 读 `finalValue`）全部基于过期数值。

### 2.2 技能数值计算系统同样未注册

`War3Frame/Src/Systems/Ability/AbilityStatCalculationSystem.cs` 与属性系统镜像（`AbilityStatValue` + `AbilityStatDirty`），同样无 `[SystemRegister]`。`AbilityHelper.Stat.cs:178/210` 写入 `AbilityStatDirty`，无人消费。

### 2.3 查询循环内结构变更风险

两个系统都在 `Query.ForEachEntity` 回调内执行 `attrEntity.RemoveTag<AttrDirty>()`。这与仓库已确立的"**查询内收集、查询外统一 apply**"模式冲突（`CastingSystem`、`ItemUseSystem`、`ProjectileSystem`、`BuffExpireSystem` 均采用该模式），在 Friflo ECS 中对查询迭代中的实体做结构变更可能触发 `StructuralChangeException` 或导致迭代行为未定义。

因此本提案不能只加 `[SystemRegister]`——接线时须同时把 `RemoveTag` 移出查询循环，否则接线后可能在运行时崩溃。

### 2.4 未接线系统清单

除上述两个外，`SpatialGridSystem`（空间网格重建）、`AbilitySlotSystem`（空壳）也未注册。本提案**只处理有真实计算逻辑且影响核心数值链路的两个系统**，其余列入非目标。

## 3. 变更范围 / What

1. 为 `AttrCalculationSystem` 增加 `[SystemRegister(SystemKind.Interval, order)]`。
2. 为 `AbilityStatCalculationSystem` 增加 `[SystemRegister(SystemKind.Interval, order)]`。
3. 将两个系统的 `RemoveTag<...Dirty>` 从查询循环内移出，改为"查询内收集 dirty 实体 → 查询后统一移除"。
4. 保持计算公式、组件、标签语义完全不变。
5. 确定 order 位置（详见 design.md）使其位于 dirty 写入方之后、数值消费方之前。

## 4. 全局影响分析

- `War3Frame/`：两个系统进入运行调度；属性/技能最终值从此实时重算，影响伤害、治疗、Buff、Aura、物品属性、原生血蓝同步、等级加成等所有读 `finalValue` 的链路。
- `War3Frame.Generator/`：不受影响——只新增两个标注，不修改生成器。
- `FrameBuild/`：不受影响——不涉及构建编排或发布链路。
- `CSharpWar3Frame/`：不受影响——不涉及 CLI。
- `Projects/`：`test` 的模板（`talent_vitality`/`talent_mana_focus` 属性贡献、`battle_shout` 等）将首次获得实时属性计算；需在验证场景确认数值正确。

## 5. 设计要点

详见 `design.md`。核心决策：

- **order 选择**：`AttrCalculationSystem` 需位于 dirty 写入方（Buff 40/41、Aura 42、Item/Level 0）之后、数值消费方（效果结算 100+、原生同步）之前，建议 `order=45`；`AbilityStatCalculationSystem` 建议 `order=30`（早于能力效果 100+）。
- **结构变更修复**：采用"循环内 `_pending` 收集 + 循环后统一 `RemoveTag`"模式，与 `BuffExpireSystem`/`CastingSystem` 一致。
- **不加新组件**：复用现有 `AttrDirty` / `AbilityStatDirty` 标签与计算公式。

## 6. 风险、兼容性、迁移

- **风险 A（行为变化）**：接线后属性/技能数值从"从不更新"变为"实时更新"，所有读 `finalValue` 的系统行为改变。这是修复的目标效果，但需在 `test` 场景验证伤害/治疗/同步数值正确。
- **风险 B（结构变更异常）**：若只接线不修复 `RemoveTag` 位置，运行时可能触发 Friflo 结构变更异常。→ 必须在同一 change 内完成两处修复。
- **风险 C（性能）**：属性重算按 dirty 触发，仅标记实体参与查询；`RemoveTag` 移出循环避免迭代中结构变更的额外开销。无持续每帧全量计算。
- **回滚**：移除两个系统的 `[SystemRegister]` 标注即可回到现状；结构变更修复本身是纯安全改进，可保留。

## 7. 验证计划

- `dotnet build War3Frame/War3Frame.csproj`
- `dotnet build Projects/test/test.csproj`
- 静态检查：两个系统带 `[SystemRegister]`，且查询循环内无 `RemoveTag`/`AddTag`/`DeleteEntity` 结构变更
- 运行时验证（`Projects/test` 场景）：属性贡献加成生效、技能数值随等级/物品变化、Buff 过期后数值回落、原生血蓝同步使用最新 `finalValue`
- 确认 `SpatialGridSystem`/`AbilitySlotSystem` 维持未注册状态（本 change 非目标）

## 8. 拆分任务

详见 `tasks.md`。
