# Buff 系统重构：类型化标签、统一时长、行为锚点与刷新收敛

## 元信息

- **提案等级**：full（公共组件数据布局变更，涉及多个数据契约类型）
- **变更 ID**：buff-system-refactor
- **状态**：已实施
- **创建日期**：2026-09-03
- **影响模块**：`War3Frame/Src/Components/Buff.cs`, `War3Frame/Src/Components/Attribute/ModifyValue.cs`, `War3Frame/Src/Helpers/BuffHelper.cs`, `War3Frame/Src/Systems/BuffSystem.cs`, `War3Frame/Src/Helpers/AttributeHelper.cs`, `War3Frame/Src/Helpers/EffectChainBuilder.cs`, `War3Frame/Src/Components/Settlement.cs`, `War3Frame/Src/Components/Ability/EffectSpec.cs`, `War3Frame/Src/Components/Ability/AbilityEffect.cs`, `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs`, `Projects/test/Scripts/Template/`
- **前置**：`unify-buff-apply`（已实施）建立了统一 ApplyBuff/BuffSpec 入口；本 change 是对该实施的结构性收敛。

## 背景与目标

### 背景

`unify-buff-apply` 实施后 Buff 系统功能完整，但多轮叠加累积了结构性重复与类型表达脆弱。2026-09-03 全链路审查（Buff.cs → BuffHelper → BuffSystem → 效果链路 → 5 数据契约）发现以下问题：

1. **`Buff` 是 IComponent 却被当标签用**（D1）：`BuffDurationSystem` 用宽 Query `BuffDuration+Duration` + 手筛 `TryGetComponent<Buff>`，因普通计时器实体也有 `BuffDuration+Duration`。`Buff` 已是组件，Query 可精确匹配却绕路手筛。
2. **`BuffDuration` 与 `Duration` 语义完全重复**（D2）：`BuffDuration.duration`（总时长）≡ `Duration.total`（初始总时长）。刷新时需成对改两个组件（BuffHelper 3 个分支重复 3 遍），实际是 `Duration` 统一提案（memory #49）落地前的残留。
3. **DoT 型用字段组合推断**（D3）：`tickValue>0 && tickActionId!=null` 在 BuffSpec / CreateBuffInternal / BuffApplyResolveSystem / BuffEffectStepSpec 4 处重复，脆弱易错。
4. **效果链 Buff 签名爆炸**（D4）：`EffectChainBuilder.Buff` 11 参数 + 2 重载。
5. **buff 标签用 `List<string>`**（S4）：每次创建分配 List，净化 `tags.Contains("Debuff")` 线性扫描。
6. **刷新逻辑巨型方法 + 打脏代码复制 8 次**（S1）：`HandleExistingBuff` 115 行 6 分支，打脏代码（TryGetComponent<ModifyTarget> → AddTag<AttrDirty>）复制粘贴 8 次。
7. **Buff 查找无索引**（S2）：`FindBuffByIdOnUnit` O(属性×modifier) 全扫，`ApplyBuff`/`HasBuff`/`RemoveBuff` 高频调用。
8. **字段冗余**：`BuffBehavior.refreshBehavior` 是死数据（刷新决策读新 spec 不读旧 buff，C2）；`BuffBehavior.buffId` 与 `Buff.buffId` 双份（C3）。
9. **DoT 伤害来源语义错位**（C1）：`ModifySource.source` 记录"产生者实体"（供级联清理），但 DoT tick 伤害需要"伤害来源单位"。地面区域 DoT 的 source 是 areaEntity 而非单位。
10. **风格卫生**：工厂/初始化风格不一（S3）、`PurgeDebuffsWithCascade` 名不副实（S5）。

### 设计目标

1. **BuffBehavior 成为统一锚点**（D1 采纳）：每个 buff 实体经共享 `CreateBuffInternal` 必挂 `BuffBehavior` → 清理/到期系统以其作 Query 锚点，收窄查询、去手筛。
2. **删除 `BuffDuration`**（D2）：总时长用 `Duration.total`，刷新改一次 `AddComponent(Duration)`。
3. **显式 `BuffKind`**（D3）：BuffSpec/buff 实体定型 Attribute/Tick 语义，消除散落推断。
4. **方法族收敛签名**（D4）：EffectChainBuilder.Buff 拆主方法 + `.WithIcon/.WithTick/.WithTags` 链式扩展。
5. **`[Flags] BuffTag` 类型化标签**（S4）：替代 `List<string>`，净化/免疫变位运算。
6. **刷新路径收敛**（S1）：抽 `RefreshBuffCore` + 统一打脏 helper。
7. **Buff 索引**（S2）：单位挂 Buff 索引组件，`FindBuffByIdOnUnit` O(1)。
8. **字段清理**（C2/C3）：删 `BuffBehavior.refreshBehavior` 与 `BuffBehavior.buffId`。
9. **caster 解析**（C1）：buff 实体加 `caster` 字段，DoT 伤害来源用 `buff.caster`。
10. **风格收敛**（S3/S5）：统一工厂、`PurgeDebuffs` 改名。

## 非目标

- **不改变 Buff 底层存储模型**（独立实体 + ModifyTarget link + 属性贡献）。
- **不引入数据驱动 Buff 注册表**（未来单点替换，见 design ADR-11）；保留 `BuffEffectStepSpec`（authoring 层）与 `BuffSpec`（helper 工厂边界）——它们不是 relay 拷贝层。
- **不改动 Aura/地面区域系统的驱动逻辑**（只受 BuffHelper 内部变化影响）。
- **不改 native 层**（Pause 合成等由 synthesize-pause-from-controls 独立处理）。

## 影响范围

| 区域 | 影响 | 说明 |
|---|---|---|
| `War3Frame/Src/Components/Buff.cs` | 修改 | 删 `BuffDuration`；Buff 加 `caster`/`kind` 字段 + BuffTag 枚举；BuffBehavior 瘦身（删 buffId/refreshBehavior/removeAllStacksOnExpire 评估）；BuffStacks 初始化统一 |
| `War3Frame/Src/Helpers/BuffHelper.cs` | 修改 | 刷新路径收敛、打脏 helper、BuffSpec 加 kind、索引维护、便捷方法同步 |
| `War3Frame/Src/Systems/BuffSystem.cs` | 修改 | BuffDurationSystem 收窄（删 BuffDuration 依赖）；BuffExpireSystem 锚 BuffBehavior；BuffTickSystem 用 caster |
| `War3Frame/Src/Helpers/AttributeHelper.cs` | 修改 | GetAllAttrs/索引辅助 |
| `War3Frame/Src/Components/Attribute/ModifyValue.cs` | 无 | ModifySource 语义保持（服务级联清理）|
| `War3Frame/Src/Helpers/EffectChainBuilder.cs` | 修改 | Buff 方法族拆分 |
| `War3Frame/Src/Components/Settlement.cs` | 修改 | tags → BuffTag；**`BuffApplyRequest` 扩展**（+ability/durationValue/modifyValue，承接 ApplyBuffData，D5）|
| `War3Frame/Src/Components/Ability/AbilityEffect.cs` | 修改 | tags → BuffTag；**删除 `ApplyBuffData`**（并入 BuffApplyRequest，D5）|
| `War3Frame/Src/Components/Ability/EffectSpec.cs` | 修改 | BuffEffectStepSpec.tags → BuffTag（保留，authoring 层）|
| `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs` | 修改 | BuffEffectSystem Query 改 `BuffApplyRequest`；BuffApplyResolveSystem 适配 kind + EffectSource 防御；settlement typeof 换名 |
| `War3Frame/Src/Helpers/AbilityEffectHelper.cs` | 修改 | 3 处 ApplyBuffData 写入改产 BuffApplyRequest；spec 展开**补全 11 字段（P0 bug）**|
| `War3Frame/Src/Helpers/Trigger/TriggerActionRegistry.cs` | 无（回归确认）| BuffApply 直写 BuffApplyRequest 的路径不动 |
| `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/` | 不受影响 | 无生成器/构建契约变化 |
| `Projects/test` | 修改 | 模板示例 tags 字符串 → BuffTag；TriggerValidationScenario 回归确认（断言 BuffApplyRequest 字段）|

## 方案摘要

### 1. `[Flags] BuffTag` 类型化标签（S4）

```csharp
[Flags]
public enum BuffTag
{
    None = 0,
    Debuff = 1 << 0,
    Control = 1 << 1,
    Stun = 1 << 2,        // 便捷方法细分
    Root = 1 << 3,
    Silence = 1 << 4,
    DoT = 1 << 5,
    Fire = 1 << 6,        // 元素预留
    Frost = 1 << 7,
    Poison = 1 << 8,
    // 后续元素/词缀按需扩展
}
```

- Buff.tags / BuffSpec.tags / BuffEffectStepSpec.tags / ApplyBuffData.tags / BuffApplyRequest.tags 全部 `List<string>?` → `BuffTag`（默认 None）。
- 净化：`(buff.tags & BuffTag.Debuff) != 0`；免疫按元素同理。
- 兼容旧字符串语义：`Stun` 便捷方法 tags = `Debuff|Control|Stun`（原 `["Debuff","Control","Stun"]`）。

### 2. 删除 `BuffDuration`，并入 `Duration.total`（D2）

- `Duration` 已有 `remaining`（递减）+ `total`（初始总时长，不变）。
- 删除 `BuffDuration` 组件；`CreateBuffInternal` 不再挂它。
- 刷新路径改一次 `Duration`：`duration.remaining = spec.duration; duration.total = spec.duration; AddComponent(duration)`。
- `BuffDurationSystem` → Query `Buff + BuffBehavior + Duration`（D1 锚点），删手筛。

### 3. `BuffKind` 显式定型（D3）

```csharp
public enum BuffKind
{
    Attribute,   // 属性贡献型（挂 ModifyValue）
    Tick,        // 周期行为型（DoT，不挂 ModifyValue，tick 读 tickValue）
    PureTag,     // 纯标记（不挂 ModifyValue 也不 tick，仅状态标记）
}
```

- BuffSpec 加 `BuffKind kind`（构造时定型）；删 CreateBuffInternal / BuffApplyResolveSystem 的 `isDot` 推断。
- `Tick` 型强制 attrTypeId 语义 = 载体（净化反查用）；`Attribute` 型强制挂 ModifyValue。
- Control 便捷方法（Stun/Root/Silence）kind = Attribute（Flat+1 属性门闩）。

### 4. BuffBehavior 统一锚点 + 瘦身（D1/C2/C3）

- 保留 `BuffBehavior`（buff 独有、必挂）作 Query 锚点：到期/清理系统 `QuerySystem<Buff, BuffBehavior, ...>`。
- **删 `BuffBehavior.refreshBehavior`**（死数据，刷新决策读新 spec 的 onDuplicate）。
- **删 `BuffBehavior.buffId`**（Buff.buffId 已是唯一同义源，FindBuffByIdOnUnit 改读 Buff.buffId）。
- **删 `BuffBehavior.removeAllStacksOnExpire`**（当前无实现者，到期即全删；如未来需要"逐层减"再单独加语义）。
- BuffBehavior 最终只剩 `icon`。

### 5. 刷新路径收敛（S1）

- 抽 `DirtyAttr(Entity buffEntity)`：TryGetComponent<ModifyTarget> → target.target.AddTag<AttrDirty>()。
- 抽 `RefreshDurationOnly(existing, spec)` / `RefreshDurationAndStack(existing, spec)` 私有核心，六个 switch 分支只声明策略：
  ```csharp
  switch (spec.onDuplicate)
  {
      case Independent: return existing;
      case Replace:    DirtyAttr(existing); existing.DeleteEntity(); return CreateBuffInternal(...);
      case ReplaceIfLonger: if (spec.duration > existingDuration.remaining) { ...Replace... } return existing;
      case RefreshDuration:    RefreshCore(existing, spec, refresh:true,  stack:false); break;
      case AddStack:           RefreshCore(existing, spec, refresh:false, stack:true);  break;
      case RefreshAndStack:    RefreshCore(existing, spec, refresh:true,  stack:true);  break;
  }
  ```

### 6. Buff 索引（S2）

- 单位挂 `BuffIndex : IComponent { Dictionary<string, long> buffIdToInstance; }`（或引入 link 组件存储）。
- `FindBuffByIdOnUnit`：先查 BuffIndex O(1) → 若命中，`store.GetEntityById` 按实例 ID 直取。
- 索引维护：`CreateBuffInternal` 写入、删除路径（BuffExpireSystem/RemoveBuff/RemoveAllBuffs/Replace）移除；单位销毁时整体清除。
- **风险**：索引与实体删除需原子维护——删除路径集中在删除 helper，不在分散的 DeleteEntity 调用点。

### 7. caster 解析（C1）

- Buff 组件加 `Entity caster`（伤害来源单位）。
- `CreateBuffInternal` 解析 caster：source 本身是单位（有 HasAttr 关系/AttrOwner）→ 直接用；否则沿 source 领域链路解析（如 GroundAreaSource.caster、光环 owner），无则 null。
- `DealDamageTickAction.Execute` 用 `buff.caster`（回退 source.source）作 DamageRequest.source。
- ModifySource.source 语义保持"产生者实体"（供级联清理）。

### 8. EffectChainBuilder.Buff 方法族（D4）

```csharp
// 主方法收敛为核心参数
public EffectChainBuilder Buff(string buffId, AbilityValue duration, int attrTypeId,
    AbilityValue value, ModifyType modifyType = ModifyType.Flat,
    BuffRefreshBehavior onDuplicate = BuffRefreshBehavior.RefreshDuration);
// 链式扩展
public EffectChainBuilder WithIcon(string icon);
public EffectChainBuilder WithTick(BuffTickSpec tick);   // tickInterval + tickActionId + tickValue
public EffectChainBuilder WithTags(BuffTag tags);
```

- 内部组装 BuffEffectStepSpec；能力值双载（EffectValueSpec/AbilityValue）保留核心重载。

### 9. [P0-BUG] ApplyEffectSpec Buff 分支字段丢失（D5 发现）

`AbilityEffectHelper.ApplyEffectSpec` 的 `EffectStepKind.Buff` 分支（行 228-237）只复制 6/11 字段——**丢弃 `icon/tickInterval/tickActionId/tickValue/tags`**（unify-buff-apply 后补的 5 个新字段）。后果：
- 经 `EffectChainBuilder` 定义的效果链 Buff（含 DoT）→ 实际 buff 实体 tick 配置与标签全丢 → DoT 不 tick、Debuff 标签缺失净化扫不到。
- BuffHelper 直调路径（Stun/DoT 便捷方法）不走此分支 → 正常。**两条路径行为不一致，效果链 DoT 是坏的。**

**修复**：Buff 分支补全 11 字段复制（或整体复制 step.buff → data 的同名字段）。

### 10. 链路评估结论：保留 ApplyBuffData，不做结构删除（T12 实施偏离）

**原案**（D5 推荐）：删除 `ApplyBuffData` 并入 `BuffApplyRequest`，链路 3→2 relay。

**实施中发现的否决事实**：`ApplyBuffData` 不单是 effect payload——`AbilityEffectHelper:62/137` 把它**直接挂在 ability / 父效果实体上作为持久配置**（`CreateEffectEntity`/`CreateChildEffect` 复制到 effect 实体），与 DamageEffectData/HealEffectData 等"既有 payload 组件"模式完全同构。若删并入 BuffApplyRequest：
- ability/父效果上的 BuffApplyRequest 会被 `BuffApplyResolveSystem`（Query `BuffApplyRequest`）误当请求消费（ability 无 EffectSource 可 guard）。
- 破坏与 Damage/Heal 的 effect 架构一致性（它们都有 spec → payload → request → resolve 的同构四层）。
- `BuffApplyRequest` 有独立外部生产者（`TriggerActionRegistry.BuffApply` 直写），request 层是真实公共契约不能删。

**实际落地**：保留 `ApplyBuffData`（legacy 配置 + payload 同构层）与 `BuffApplyRequest`（公共 request 契约）；**补 kind 全链路透传**（BuffEffectStepSpec.kind → ApplyBuffData.kind → BuffApplyRequest.kind → BuffSpec.kind → Buff 组件），消除 DoT 推断散落（T3）。链路字段重复是同构架构的必然成本，非冗余——不再强行删层。

## 风险与回滚

- **回滚**：逐项还原组件字段与 Helper 即可；无外部持久化。删除 BuffDuration 是最小破坏（3 文件 8 处引用）。
- **主要风险**：tags 类型变更（List<string> → BuffTag）触及 5 个公共契约 + 模板示例 + 潜在外部读者——需全仓搜索字符串 tag 用法逐一迁移；BuffTag 无法表达任意作者字符串（如需"自定义 tag"需加注册表回退，先按枚举够用处理）。
- **BuffIndex 一致性**：索引与实际实体删除路径必须同一 helper 维护，否则 O(1) 查到已删实体会 return default（需判空回退全扫兜底）。
- **BuffBehavior 瘦身**：删字段前全仓确认无读者（已核 refreshBehavior 无外部读者、buffId 双份）。
- **DurationSystem order**：BuffDurationSystem 现有 order 40 改 Query 后行为不变（仍只做 DurationExpired → BuffExpired 翻译）。

## 验收标准

1. `dotnet build War3Frame/War3Frame.csproj` 0 error；`Projects/test` 0 error；`Projects/demo` 0 error；全仓零 `BuffDuration` 结构体 / `BuffBehavior.refreshBehavior` / `BuffBehavior.buffId` / `BuffBehavior.removeAllStacksOnExpire` / `List<string>` tags / `PurgeDebuffsWithCascade` 残留。
2. 三旧入口（AddTimedBuff/AddStackableBuff/AddPermanentBuff）行为不变（仍是 ApplyBuff 别名）；Stun/Root/Silence/ApplyDoT 便捷方法 tags 从 List → BuffTag 后语义等价（净化可清、免疫可判）。
3. 刷新语义回归：RefreshDuration 更新 remaining+total 一次；ReplaceIfLonger 取最晚结束；AddStack 层数正确（单元级验证或编译 + 静态核对）。
4. **不做显式 Buff 索引**（T6 结论）：`FindBuffByIdOnUnit` 等调用全部集中在 BuffHelper 内部低频路径，无外部高频调用方；Friflo `GetIncomingLinks<ModifyTarget>` 反向链接已兜底，显式 `Dictionary` 索引引入一致性负担而无实际收益。保留全扫实现。
5. DoT（BuffKind.Tick）由地面区域产生时，DamageRequest.source 是 caster 单位而非 areaEntity（静态核对 DealDamageTickAction 用 buff.caster）。
6. `BuffDurationSystem` Query 收窄为 `Buff+BuffBehavior+Duration` 后不再手筛 Buff（静态核对）。
7. 净化（PurgeDebuffs）用位运算 `(tags & BuffTag.Debuff) != 0` 命中原 List.Contains("Debuff") 语义。
8. 效果链 Buff 表达 Tick 能力：`.Buff(...).WithTick` 方案因 step 追加模型不可回改而收敛为 **Buff 方法族加 `kind` 参数**（默认 Attribute，DoT 用 Tick），作者可用链式 Buff 造自定义载体 DoT（模板 DoT 便捷方法保留）。
9. 无 AuraSystem/GroundAreaBuffSystem 回归（AddPermanentBuff 别名路径编译 + 行为不变）。
10. **[P0 bug] 效果链 DoT 经 EffectChainBuilder 定义 → 实际 buff 带正确 tickActionId/tickValue/tags**（静态核对 AbilityEffectHelper Buff 分支复制 11 字段，不再丢 5 字段）。
11. **[T12] `ApplyBuffData` 保留**（legacy ability 配置 + 与 Damage/Heal 同构）：ability/父效果配置路径编译通过（回归）；TriggerActionRegistry.BuffApply 直写 BuffApplyRequest 编译通过（回归）；kind 全链路透传（BuffEffectStepSpec → ApplyBuffData → BuffApplyRequest → BuffSpec → Buff）无遗漏（静态核对）。

## 后续工作

- 客户端真实验证（DoT 来源单位、净化位运算）列为非阻塞。
- BuffEffectSystem 迭代内 CreateEntity/RemoveComponent 的结构变更张力（D5 提示的历史问题）——独立后续提案。
- 数据驱动 Buff 注册表（BuffSpec 注册表 + request 瘦身）——未来单点替换（design ADR-11 记录）。
