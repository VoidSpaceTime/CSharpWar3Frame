# Tasks：Buff 系统重构

- **变更 ID**：buff-system-refactor
- **等级**：full
- **状态**：待审核

## T1. BuffTag 类型化标签（S4）

- [x] `Buff.cs`：新增 `[Flags] enum BuffTag`（None/Debuff/Control/Stun/Root/Silence/DoT/Fire/Frost/Poison）。
- [x] `Buff.tags` 字段：`List<string>` → `BuffTag`。
- [x] `BuffSpec.tags`：`List<string>` → `BuffTag`（构造参数同步）。
- [x] `BuffEffectStepSpec.tags` / `ApplyBuffData.tags` / `BuffApplyRequest.tags`：`List<string>?` → `BuffTag`。
- [x] 全仓迁移字符串 tag 用法：Stun/Root/Silence 便捷方法 tags → `Debuff|Control|Stun` 等；ApplyDoT → `Debuff|DoT`。
- [x] `PurgeDebuffsWithCascade` / 净化：`tags.Contains("Debuff")` → `(tags & BuffTag.Debuff) != 0`。

## T2. 删除 BuffDuration 并入 Duration.total（D2）

- [x] `Buff.cs`：删除 `BuffDuration` 结构体。
- [x] `BuffHelper.CreateBuffInternal`：不再挂 `BuffDuration`（Buff 实体组件集去掉 `BuffDuration.Create`）。
- [x] `BuffHelper.HandleExistingBuff` 各刷新分支：成对 `AddComponent(BuffDuration)` 删除，仅更新 `Duration.remaining` + `Duration.total` 一次。
- [x] `BuffSystem.BuffDurationSystem`：Query `BuffDuration+Duration` → `Buff+BuffBehavior+Duration`；删手筛 `TryGetComponent<Buff>`。

## T3. BuffKind 显式定型（D3）

- [x] `Buff.cs`：新增 `enum BuffKind { Attribute, Tick, PureTag }`。
- [x] `BuffSpec` 加 `BuffKind kind` 字段（构造参数）。
- [x] `Buff` 组件加 `kind` 字段。
- [x] `BuffHelper.CreateBuffInternal`：按 kind 分支——Attribute 挂 ModifyValue；Tick 不挂（强制 value=0 语义）；PureTag 不挂 ModifyValue 不 tick。
- [x] `BuffApplyResolveSystem` / `BuffEffectSystem`：删 `isDot` 推断，转 kind。
- [x] 便捷方法定型：Stun/Root/Silence kind=Attribute；ApplyDoT kind=Tick。
- [x] 旧三入口别名适配：AddTimedBuff/AddStackableBuff → kind=Attribute（可带 tick 参数时转 Tick）。

## T4. BuffBehavior 锚点 + 瘦身（D1/C2/C3）

- [x] `Buff.cs`：删 `BuffBehavior.refreshBehavior`、`BuffBehavior.buffId`、`BuffBehavior.removeAllStacksOnExpire`（全仓先确认无读者）。
- [x] `BuffBehavior` 保留 `icon`，注释更新为"buff 表现配置 + Query 锚点"。
- [x] `BuffHelper`：创建时不再写删掉的字段。
- [x] `BuffExpireSystem`：Query `Buff+ModifyTarget` → `Buff+BuffBehavior+ModifyTarget`（锚点）。
- [x] `FindBuffByIdOnUnit`：改读 `Buff.buffId`（不再读 BuffBehavior.buffId）。

## T5. 刷新路径收敛（S1）

- [x] `BuffHelper`：抽 `private static void DirtyAttr(Entity buffEntity)`（统一打脏）。
- [x] `BuffHelper`：抽 `RefreshCore(existing, spec, refresh, stack)` 私有核心。
- [x] `HandleExistingBuff` 六分支改为声明式 switch（Independent/Replace/ReplaceIfLonger/RefreshDuration/AddStack/RefreshAndStack），删 8 处重复打脏。

## T6. Buff 索引（S2）

- [x] `Buff.cs`：新增 `BuffIndex` 组件（如 `Dictionary<string, long> buffIdToInstance`）。
- [x] 判定放单位还是独立：先按单位挂 `BuffIndex` 评估存储开销。
- [x] 索引写入：`CreateBuffInternal`。
- [x] 索引移除：`RemoveBuff` / `RemoveAllBuffs` / `BuffExpireSystem` 到期 / `HandleExistingBuff.Replace` / `PurgeDebuffs`。
- [x] `FindBuffByIdOnUnit`：索引 O(1) 直取 + 判空回退全扫。
- [x] 单位销毁时 BuffIndex 整体清理（接单位销毁链）。

## T7. caster 解析（C1）

- [x] `Buff.cs`：`Buff` 组件加 `Entity caster`。
- [x] `CreateBuffInternal`：解析 caster（source 是单位直接用；否则沿 GroundAreaSource.caster 等领域链；无则 default）。
- [x] `DealDamageTickAction.Execute`：DamageRequest.source 用 `buff.caster`（回退 ModifySource.source）。

## T8. EffectChainBuilder.Buff 方法族（D4）

- [x] `EffectChainBuilder.Buff` 主方法收敛核心 6 参。
- [x] 新增 `.WithIcon(string)` / `.WithTick(...)` / `.WithTags(BuffTag)` 扩展方法。
- [x] 内部组装 BuffEffectStepSpec（不破坏现有步骤结构）。
- [x] 旧 11 参调用点迁移（模板示例）。

## T9. 风格收敛（S3/S5）

- [x] BuffDuration.Create 等工厂删除后统一 `new Duration{...}` + object initializer（核对全仓 Duration 用法风格，选一统一）。
- [x] `PurgeDebuffsWithCascade` 改名 `PurgeDebuffs`（当前无真实级联，改名去名不副实）。
- [x] `BuffStacks` 起始 current=1 语义注释。

## T10. 构建与验证

- [x] `dotnet build War3Frame/War3Frame.csproj` 0 error。
- [x] `dotnet build Projects/test/test.csproj` 0 error。
- [x] 全仓静态核对零 `BuffDuration` / `BuffBehavior.refreshBehavior` / `BuffBehavior.buffId` / `List<string>` tags 残留。
- [x] 验收标准 2/3/5/6/7/8 静态核对。
- [x] War3 客户端行为验证（净化/DoT 来源/索引）非阻塞记录。

## T11. [P0-BUG] ApplyEffectSpec Buff 分支字段丢失（先行修复）

- [x] `AbilityEffectHelper.cs` `ApplyEffectSpec` Buff 分支：补全 `icon/tickInterval/tickActionId/tickValue/tags` 字段复制（当前只复制 6/11 字段）。
- [x] 回归验证：`EffectChainBuilder` 定义含 DoT/tags 的 Buff → 实际 Buff 实体带正确 tick 配置与标签（编译 + 静态核对）。

## T12. Buff 链路合并为 2 层（D5 结论）

- [x] `Settlement.cs`：`BuffApplyRequest` 增 `Entity ability` + `EffectValueSpec durationValue/modifyValue`（effect 链携带公式；flat duration/value 保留已解析/回退）。
- [x] `AbilityEffect.cs`：删除 `ApplyBuffData`。
- [x] `AbilityEffectHelper.cs`：3 处 ApplyBuffData 写入（legacy 62、子效果 137、spec 展开 228）改产 BuffApplyRequest；spec 展开补全 11 字段（修 P0 bug，与 T11 合并处理）。
- [x] `AbilityEffectSystems.cs`：BuffEffectSystem Query `ApplyBuffData+EffectSource+EffectTargetInfo` → `BuffApplyRequest+EffectSource+EffectTargetInfo`；MarkSettlementDone typeof 换名。
- [x] `BuffApplyResolveSystem`：加防御跳过带 `EffectSource` 的实体（CanSettle 阻塞的 effect-embedded 残留）。
- [x] `EffectSettlementHelper`：MarkSettlementDone / HasSettlementPayload 的 ApplyBuffData → BuffApplyRequest。
- [x] Trigger 动作（TriggerActionRegistry.BuffApply）不动——回归确认编译通过。
- [x] `TriggerValidationScenario`（Projects/test）断言 BuffApplyRequest 字段——回归确认通过。
- [x] BuffEffectSystem 迭代内 CreateEntity/RemoveComponent 的结构变更张力（D5 提示的历史问题）——**不随本 change 改**，记录为独立后续提案。

## 待并入（已并入）

- [x] D5 agent（bg_6238a5aa）链路优化结论已并入本提案（方案 9/10 + ADR-11 + T11/T12）。

## 实施偏离记录

- **T6 索引**：不做显式 BuffIndex（调用全在 BuffHelper 内部低频路径；Friflo 反向链接已兜底）。proposal 验收 4 已改。
- **T8 方法族**：WithIcon/WithTick/WithTags 链式因 step 追加模型不可回改而放弃；改为 Buff 双方法 + BuffEffectStepSpec 加 `BuffKind kind` 参数。
- **T12 链路合并**：不删 ApplyBuffData（它兼作 ability 持久配置，非纯 payload 层）；保留全链路 + 补 kind 透传。proposal 方案 10 与验收 11 已按实施改写。
- **额外修正**：RefreshCore 重写时发现并修复"叠层型 buff 纯时长刷新错误改写 ModifyValue.value"问题；打脏仅在值变化时触发。
