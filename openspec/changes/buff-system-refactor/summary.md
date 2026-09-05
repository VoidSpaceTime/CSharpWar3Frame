# Summary：Buff 系统重构

- **变更 ID**：buff-system-refactor
- **等级**：full
- **状态**：已实施
- **实施日期**：2026-09-03

## 实际改动范围

### 组件层（Buff.cs）
- **新增** `[Flags] BuffTag` 枚举（None/Debuff/Control/Stun/Root/Silence/DoT/Fire/Frost/Poison）替代字符串标签。
- **新增** `BuffKind` 枚举（Attribute/Tick/PureTag）定型 Buff 语义。
- **新增** `Buff.kind` / `Buff.caster` 字段（施法者单位，DoT 伤害源）。
- **改** `Buff.tags`：`List<string>` → `BuffTag`。
- **删除** `BuffDuration` 结构体（总时长并入 `Duration.total`）。
- **瘦身** `BuffBehavior`：删 `buffId` / `refreshBehavior` / `removeAllStacksOnExpire`，只留 `icon`（每 buff 必挂，作清理/到期系统 Query 锚点）。
- **BuffStacks.Create** 补 `current=1` 起始语义注释。

### Helper 层
- **BuffHelper**：
  - 刷新路径收敛：抽 `DirtyAttr`（统一打脏）+ `RefreshCore(existing, spec, refresh, stack)`（刷新/叠层声明式），`HandleExistingBuff` 六分支只声明策略，删 8 处复制打脏。
  - `RefreshCore` 修正叠层型 buff 纯时长刷新不再错误改写 `ModifyValue.value`；打脏仅在值变化时触发。
  - `BuffSpec` 加 `BuffKind kind`（默认 Attribute）+ tags 改 BuffTag；`CreateBuffInternal` 按 kind 定型挂 ModifyValue。
  - `ResolveCaster`：source 是单位直接用，否则沿 `GroundAreaSource.caster` 链解析（扩展点）。
  - `DealDamageTickAction` 伤害源改 `buff.caster`（回退 ModifySource）。
  - `FindBuffByIdOnUnit` 改读 `Buff.buffId`（不再依赖 BuffBehavior）。
  - `PurgeDebuffsWithCascade` → **`PurgeDebuffs`**（去掉名不副实的"级联"）+ 位运算净化。
- **EffectChainBuilder**：Buff 双方法 + BuffEffectStepSpec 加 `BuffKind kind`（默认 Attribute）——作者可用链式 Buff 表达自定义载体 Tick DoT。

### 系统层（BuffSystem.cs）
- `BuffDurationSystem` Query 收窄为 `Buff+BuffBehavior+Duration`，删手筛（原宽 Query `BuffDuration+Duration` + TryGetComponent）。
- `BuffExpireSystem` Query 锚定 `Buff+BuffBehavior+ModifyTarget`。

### 契约层（tags → BuffTag + kind 透传）
- `BuffEffectStepSpec.tags/kind`、`ApplyBuffData.tags/kind`、`BuffApplyRequest.tags/kind`、`BuffSpec.tags/kind` 全链路统一。
- kind 透传：BuffEffectStepSpec → ApplyBuffData → BuffApplyRequest → BuffSpec → Buff 组件。
- `BuffApplyResolveSystem`：`request.kind` 优先 + 旧路径按 tick 参数推断兼容。

### P0 bug 修复（AbilityEffectHelper）
- `ApplyEffectSpec` Buff 分支原只复制 6/11 字段，**补全 icon/tickInterval/tickActionId/tickValue/tags/kind**——效果链 DoT 不再丢 tick 配置与标签。

## 与提案的偏离

| 项 | 原案 | 实际 | 原因 |
|---|---|---|---|
| T6 索引 | 可选显式 BuffIndex | **不做索引** | 调用全在 BuffHelper 低频路径；Friflo 反向链接已兜底；显式索引一致性负担 > 收益 |
| T8 方法族 | WithIcon/WithTick/WithTags 链式 | **Buff 加 kind 参数** | step 追加模型无法回改上一步，链式扩展语义别扭 |
| T12 链路合并 | 删 ApplyBuffData 并入 BuffApplyRequest | **保留 ApplyBuffData** + kind 透传 | ApplyBuffData 兼作 ability 持久配置（62/137），删除会使配置被 ResolveSystem 误消费；与 Damage/Heal 同构；BuffApplyRequest 有独立生产者（Trigger）不能删 |

## 验证

- ✅ War3Frame / Projects/test / Projects/demo 三项目编译 0 error。
- ✅ 全仓零残留：`BuffDuration` 结构体 / `BuffBehavior.buffId` / `BuffBehavior.refreshBehavior` / `BuffBehavior.removeAllStacksOnExpire` / `List<string>` tags / `PurgeDebuffsWithCascade`。
- ✅ 静态核对：BuffDurationSystem/BuffExpireSystem Query 锚 BuffBehavior 无手筛；PurgeDebuffs 位运算；DealDamageTickAction 用 buff.caster；ApplyEffectSpec 复制 11 字段；便捷方法 tags 位组合语义等价。

## 遗留 / 后续

- War3 客户端行为验证（净化位运算、DoT caster 来源、Tick kind 路径）——非阻塞，需真实客户端环境。
- BuffEffectSystem 迭代内 CreateEntity/RemoveComponent 与 Friflo 结构变更约束的历史张力——独立后续提案。
- 数据驱动 Buff 注册表（BuffSpec 注册 + request 瘦身）——design.md 记录，未来单点替换。
- `PureTag` 三态为燃油标记等纯状态预留，当前无便捷入口（作者可直传 BuffSpec kind=PureTag）。
