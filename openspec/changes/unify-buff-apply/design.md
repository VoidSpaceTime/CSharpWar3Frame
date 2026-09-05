# Buff 应用入口统一与生命周期扩展 — Design

## 架构决策

### 决策 1：Buff 保持"薄状态"模型，复杂效果通过外部系统实现

**选择**：Buff 实体仍然是"状态标记 + 可选单属性贡献"，不承载复杂子效果列表。

**理由**：
1. **ECS 亲和性**：Buff = Component，复杂行为 = System。如果 Buff 内部持有子效果列表（OOP 模式），会破坏 ECS 的数据与行为分离原则。
2. **简洁性**：当前 95% 的 buff（眩晕/减速/护甲/吸血）只是单属性加减，无需复杂容器。
3. **扩展性**：少数复杂 buff（光环/召唤）通过额外 Component（`Aura` / `SummonOwner`）+ `sourceBuffId` 关联，保持核心 `Buff` 组件轻量。

**权衡**：如果未来需要"单 buff 同时减速 + DoT + 护甲"这类复合效果，需要创建多个 buff 实体（如 "poison_slow" + "poison_dot" + "poison_armor"）。这增加了实体数量，但保持了组件单一职责。

**参考**：GAS `GameplayEffect` 同样是"一个 GE = 一组 Modifiers + 一个 Duration"，复合效果通过 `ConditionalGameplayEffects` 附加多个 GE 实现。

---

### 决策 2：旧入口改为别名，不直接废弃

**选择**：保留 `AddTimedBuff` / `AddStackableBuff` / `AddPermanentBuff` 三个方法，但内部统一调用 `ApplyBuff(BuffSpec)`。

**理由**：
1. **向后兼容**：仓库内已有 5 处调用点（`BuffApplyResolveSystem` / `GroundAreaBuffSystem` / `AuraSystem` / test 项目模板），全部重构风险高。
2. **渐进迁移**：旧代码继续工作，新代码直接用 `ApplyBuff`，给未来全面切换留缓冲期。
3. **语义清晰**：`AddTimedBuff` 比 `ApplyBuff(spec with duration>0, maxStacks=1, RefreshDuration)` 更直观。

**权衡**：维护两套 API 有小幅代码重复，但通过"旧入口 = 构造 `BuffSpec` + 调用 `ApplyBuff`"实现，重复仅 3 行/方法。

**未来迁移路径**：当所有调用点改为 `ApplyBuff` 后，标记旧方法 `[Obsolete]` 并在后续版本移除。

---

### 决策 3：Tick 由 `BuffSystem` 驱动，而非独立 `BuffTickSystem`

**选择**：在现有 `BuffSystem.Update` 里增加 tick 循环，不新建系统。

**理由**：
1. **职责内聚**：Buff 的时长推进和 tick 触发都是 buff 生命周期的一部分，属于同一职责。
2. **避免跨系统依赖**：如果独立 `BuffTickSystem`，需要确保它在 `BuffSystem` 之后执行（否则 buff 刚创建，tick 系统读不到），引入 order 依赖。
3. **性能**：Tick 检查与 duration 推进共享同一次 `Query<Buff, Duration>()`，零额外遍历。

**权衡**：`BuffSystem` 代码行数增加（+30 行），但逻辑内聚性更强。

**实现细节**：
```csharp
// BuffSystem.Update 伪代码
foreach (var (buff, duration, behavior) in Query<Buff, Duration, BuffBehavior>()) {
    // 原有逻辑：推进 duration.elapsed
    duration.elapsed += deltaTime;
    if (duration.elapsed >= duration.total && duration.total > 0) {
        // 删除过期 buff
    }
    
    // 新增逻辑：tick 检查
    if (buff.tickInterval > 0) {
        while (duration.elapsed - buff.lastTick >= buff.tickInterval) {
            buff.lastTick += buff.tickInterval;
            BuffTickActionRegistry.Get(buff.tickActionId)?.Execute(buff.entity, GetTarget(buff));
        }
    }
}
```

---

### 决策 4：`buffInstanceId` 使用递增 ID，而非 GUID

**选择**：`buffInstanceId = Interlocked.Increment(ref _nextBuffId)`（线程安全递增）。

**理由**：
1. **性能**：`long` 比 `Guid` (128 bit) 更轻，比较/哈希更快。
2. **可读性**：日志/debug 时 `buffInstanceId=42` 比 `Guid=3f2504e0-...` 更友好。
3. **单线程保证**：War3 框架是单线程（所有逻辑在主 timer 回调里），不需要 GUID 的分布式唯一性。

**权衡**：如果未来需要跨进程/跨机器同步 buff（如服务器/客户端分离架构），递增 ID 可能冲突。但当前 War3 框架是单进程架构，无此风险。

**实现**：
```csharp
private static long _nextBuffId = 0;

public static Entity ApplyBuff(...) {
    var buff = buffEntity.Add<Buff>();
    buff.buffInstanceId = Interlocked.Increment(ref _nextBuffId);
    // ...
}
```

---

### 决策 5：`tags` 使用 `List<string>` 而非 `HashSet<string>`

**选择**：`Buff.tags` 字段类型为 `List<string>`。

**理由**：
1. **数量小**：单个 buff 的 tag 数量通常 2-5 个（如 `["Debuff", "Fire", "DoT"]`），`List.Contains` 的 O(N) 查询足够快。
2. **内存占用**：`HashSet` 初始分配 ~40 bytes overhead（桶数组 + 链表节点），`List` 仅 16 bytes（数组指针 + count/capacity）。
3. **序列化友好**：如果未来需要网络同步或持久化，`List` 比 `HashSet` 更简单（顺序稳定）。

**权衡**：如果 tag 数量增长到 >20 个/buff，`HashSet` 查询会更快。但这不符合实际使用场景（tag 是分类标签，不是数据存储）。

**后续优化空间**：如果性能分析发现 `tags.Contains` 是热点，可改用 `HashSet` 或位标志（`ulong tagBits`）。

---

### 决策 6：`tickActionId` 使用字符串 ID + 静态注册表，而非委托

**选择**：`Buff.tickActionId` 是 `string?`（如 `"DealDamage"`），指向 `BuffTickActionRegistry` 的静态注册项。

**理由**：
1. **序列化友好**：字符串可以序列化；委托/函数指针无法序列化（如果未来需要保存 buff 状态）。
2. **模板友好**：模板作者只需填 `tickActionId = "DealDamage"`，不需要写 lambda。
3. **扩展性**：新增 tick 行为 = 注册新条目，无需改 `Buff` 组件结构。

**权衡**：查表有小幅开销（`Dictionary<string, IBuffTickAction>` 查询），但 tick 频率通常 ≤1 Hz，开销可忽略。

**注册表接口**：
```csharp
public interface IBuffTickAction {
    void Execute(Entity buffEntity, Entity target);
}

public static class BuffTickActionRegistry {
    private static readonly Dictionary<string, IBuffTickAction> _actions = new();
    
    public static void Register(string id, IBuffTickAction action) {
        _actions[id] = action;
    }
    
    public static IBuffTickAction? Get(string? id) {
        return id != null && _actions.TryGetValue(id, out var action) ? action : null;
    }
}

// 内置 tick 行为（在 BuffSystem.Initialize 里注册）
BuffTickActionRegistry.Register("DealDamage", new DealDamageTickAction());
```

---

### 决策 7：刷新语义修正采用"新 spec 值覆盖"而非"增量调整"

**选择**：`RefreshDuration` / `RefreshAndStack` 触发时，使用新 `BuffSpec` 的 `duration` 和 `value` 完整覆盖。

**理由**：
1. **符合直觉**：施加 5s 眩晕 → 当前剩 2s → 再施加 5s → 预期剩 5s（不是 7s）。
2. **修复 P2/P3 bug**：当前 `RefreshDuration` 保留旧 value（导致同技能升级后刷新无效），`RefreshAndStack` 覆盖所有层为新 value（导致叠层后反而变弱）。
3. **一致性**：所有刷新行为统一为"按新 spec 重置"，只有 `AddStack` 是纯增量（不刷新时长）。

**对比当前行为**（bug 示例）：
```
当前（bug）：
  施加 +20 攻速 buff (2s) → 再施加 +30 攻速 buff (3s)
  RefreshDuration → duration=3s, value=+20（旧值，错误！）
  
修正后：
  RefreshDuration → duration=3s, value=+30（新值，正确）
```

**权衡**：如果某些设计确实需要"保留旧值刷新时长"，可新增 `RefreshDurationKeepValue` 枚举值（本提案暂不加，按需扩展）。

---

### 决策 8：级联清理采用"按需扩展"而非"预埋全链路"

**选择**：`PurgeDebuffsWithCascade` 当前只删 buff 本身，预留子效果扩展点（注释标注）。

**理由**：
1. **YAGNI 原则**：当前没有独立子效果实体（召唤物/延迟效果），提前实现级联查询是过度设计。
2. **清晰的扩展契约**：方法注释显式标注"扩展点：新增子效果实体时，在此补充级联查询"，未来补充成本低。
3. **避免空查询开销**：如果预埋 `Query<SummonedUnit>()` 但该组件不存在，仍有查询开销（虽然极小）。

**扩展示例**（未来实现召唤物时）：
```csharp
// 在 PurgeDebuffsWithCascade 方法末尾补充
foreach (var summon in store.Query<SummonedUnit>()) {
    if (buffIdsToRemove.Contains(summon.sourceBuffId)) {
        summon.entity.DeleteEntity();
    }
}
```

---

## 数据流设计

### 1. Buff 应用流程（ApplyBuff）

```
用户调用 ApplyBuff(unit, source, BuffSpec)
  ↓
查询同 key 既有 buff：Query<Buff, ModifyTarget> where buffId==spec.buffId && target==unit
  ↓
未命中 → 创建新 buff 实体：
  - Buff { buffId, buffInstanceId=NextId(), tags, tickInterval, tickActionId }
  - Duration { total=spec.duration, elapsed=0 }
  - BuffBehavior { icon, onDuplicate, maxStacks }
  - ModifyValue { attrTypeId, modifyType, value }（如果 spec.attrTypeId != 0）
  - ModifyTarget { target=unit }
  ↓
命中 + Replace → 删旧 buff → 创建新 buff（同"未命中"分支）
  ↓
命中 + ReplaceIfLonger → 比较 remaining vs spec.duration：
  - 新更长 → 走 Replace 分支
  - 旧更长 → 返回既有 buff，不操作
  ↓
命中 + RefreshDuration → 重置 duration.elapsed=0, duration.total=spec.duration；覆盖 modifyValue.value=spec.value
  ↓
命中 + RefreshAndStack → 同 RefreshDuration，且 stacks = min(stacks+1, maxStacks)
  ↓
命中 + AddStack → 不重置 duration；stacks = min(stacks+1, maxStacks)；重算 effective value（value * stacks）
  ↓
命中 + Independent → 返回既有 buff，不操作
  ↓
打脏：unit.AddTag<AttrDirty>()（触发 AttrCalculationSystem 重算）
  ↓
返回 buff 实体
```

### 2. Buff Tick 流程

```
BuffSystem.Update(deltaTime)
  ↓
Query<Buff, Duration, BuffBehavior>()
  ↓
foreach buff:
  duration.elapsed += deltaTime
  ↓
  if (buff.tickInterval > 0):
    while (elapsed - lastTick >= tickInterval):
      lastTick += tickInterval
      ↓
      action = BuffTickActionRegistry.Get(buff.tickActionId)
      ↓
      action?.Execute(buff.entity, GetTarget(buff))
        ↓ (以 "DealDamage" 为例)
        读取 buff.damagePerTick（或 ModifyValue.value）
        ↓
        创建 DamageRequest { source=buff.source, target=buff.target, damage=damagePerTick, type=Fire }
        ↓
        DamageResolveSystem 消费 Request → 扣血
```

### 3. 净化流程（PurgeDebuffsWithCascade）

```
PurgeDebuffsWithCascade(unit, tagFilter="Debuff")
  ↓
收集阶段：
  buffIdsToRemove = HashSet<long>()
  ↓
  Query<Buff, ModifyTarget>() where target==unit && tags.Contains(tagFilter)
    → 加入 buffIdsToRemove
  ↓
删除阶段：
  Query<Buff>() where buffInstanceId in buffIdsToRemove
    → buff.entity.DeleteEntity()
  ↓
级联删除阶段（预留扩展）：
  // 当前无独立子效果实体，跳过
  // 未来实现 SummonedUnit 时补充：
  // Query<SummonedUnit>() where sourceBuffId in buffIdsToRemove
  //   → summon.entity.DeleteEntity()
  ↓
打脏：
  unit.AddTag<AttrDirty>()
```

---

## 边界条件处理

### 1. Buff 创建时 attrTypeId == 0（纯标记 buff）

**场景**：油层 buff 只是触发器条件标记，不修改任何属性。

**处理**：`ApplyBuff` 检查 `spec.attrTypeId == 0` 时，**不挂** `ModifyValue` 组件；只挂 `Buff` / `Duration` / `BuffBehavior` / `ModifyTarget`。

**验证**：
```csharp
if (spec.attrTypeId != 0) {
    buffEntity.Add<ModifyValue>() = new ModifyValue {
        attrTypeId = spec.attrTypeId,
        modifyType = spec.modifyType,
        value = spec.value
    };
}
```

---

### 2. Duration <= 0（永久 buff）

**场景**：光环 buff、被动技能、装备加成。

**处理**：`Duration` 组件的 `total` 字段用 `-1` 表示永久（当前约定）。`BuffSystem` 检查 `duration.total < 0` 时跳过删除逻辑。

**注意**：永久 buff 仍然可以被 `PurgeDebuffs` 清除（按 tag 匹配），只是不会自然过期。

---

### 3. TickInterval == 0（不 tick 的 buff）

**场景**：控制 buff（眩晕/定身）、属性加成 buff。

**处理**：`BuffSystem` 检查 `buff.tickInterval <= 0` 时跳过 tick 逻辑。

**优化**：可以用 `Query<Buff, Duration, BuffBehavior>().Where(b => b.tickInterval > 0)` 过滤，但当前 Friflo ECS 的 `Where` 是 LINQ 延迟执行（无索引加速），性能与手动 `if` 无差异。

---

### 4. MaxStacks == 1（不可叠层 buff）

**场景**：限时 buff（如加速）。

**处理**：不挂 `BuffStacks` 组件；`RefreshAndStack` / `AddStack` 行为退化为 `RefreshDuration`。

**实现**：
```csharp
if (spec.maxStacks > 1) {
    buffEntity.Add<BuffStacks>() = new BuffStacks { current = 1, max = spec.maxStacks };
}
```

---

### 5. 同 buffId + 不同来源（source）

**场景**：两个技能各给目标 -30% 移速，应叠加为 -60%。

**处理**：`ApplyBuff` 查询既有 buff 时，**不检查 `source`**（只匹配 `buffId` + `target`），因此同 buffId 的 buff 会触发 `onDuplicate` 行为（如 `RefreshDuration`）。

**来源独立方案**：通过 `buffId` 编码来源信息实现，如：
```csharp
// 技能 A：
ApplyBuff(unit, sourceA, new BuffSpec { buffId = "slow:skillA", ... });

// 技能 B：
ApplyBuff(unit, sourceB, new BuffSpec { buffId = "slow:skillB", ... });
```

两个 buff 的 `buffId` 不同，自然独立共存。`AttrCalculationSystem` 读取所有 `ModifyValue` 时会累加两者。

**UI 显示**：可以按 `buffId` 前缀分组（如 `slow:*` 都显示减速图标），或用 `tags` 匹配（都带 `"Slow"` tag）。

---

### 6. 净化时 buff 正在 tick

**场景**：点燃 buff tick 中途被净化。

**处理**：`PurgeDebuffs` 直接 `DeleteEntity()`，`BuffSystem` 下次遍历时该实体已不存在（Friflo 的 `Query` 返回快照，删除的实体不在结果集中），tick 逻辑自然跳过。

**注意**：如果 tick 行为正在执行（如 `DealDamageTickAction.Execute` 里），需要确保不依赖 buff 实体继续存在。当前设计下，tick 行为只读取 buff 的字段后立即发 `DamageRequest`，不持有 buff 引用，安全。

---

## 性能分析

### Tick 循环开销

**假设**：100 个单位，每个带 3 个 buff，其中 1 个是 DoT（tick）。
- Buff 总数：300
- 需 tick 的 buff：100
- Tick 频率：1 Hz

**每帧开销**（60 FPS）：
- 遍历 300 个 buff：~300 次 `if (tickInterval > 0)` 检查（分支预测友好，~300 ns）
- 其中 100 个进入 tick 逻辑：100 次 `while (elapsed - lastTick >= interval)` 检查（每帧只有 ~2 个满足条件，平均每秒 100 次 tick）
- 每次 tick：查表 `BuffTickActionRegistry.Get` (~50 ns) + 执行 `DealDamageTickAction.Execute`（创建 `DamageRequest`，~200 ns）

**总开销**：~300 ns（遍历）+ 2 × 250 ns（平均每帧 tick 数）= ~800 ns/帧 = 0.0013% CPU（60 FPS 下单帧 16.6 ms）。

**结论**：可忽略。

---

### 净化查询开销

**假设**：净化技能施放，清除范围内 10 个单位的 debuff。
- 每个单位 5 个 buff（2 个 debuff，3 个 buff）
- Buff 总数：全场 300 个

**单次净化开销**：
- 遍历 10 个单位 × Query<Buff, ModifyTarget>（筛 target 匹配）：~50 次组件读取（~500 ns）
- 收集 20 个 debuff 的 `buffInstanceId`：~20 次 `HashSet.Add`（~200 ns）
- 删除 20 个 buff：~20 次 `DeleteEntity`（~2 µs）
- 打脏 10 个单位：~10 次 `AddTag<AttrDirty>`（~100 ns）

**总开销**：~3 µs = 0.00005 帧时间（60 FPS）。

**结论**：可忽略。

---

## 测试策略

### 单元测试（可选，框架当前无单测）

如果未来引入单测，覆盖以下场景：

1. **ApplyBuff 基础行为**：
   - 创建新 buff → 验证组件挂载正确。
   - 同 key 重复 → 验证 `onDuplicate` 行为（6 种枚举值各一条测试）。

2. **Tick 行为**：
   - 创建带 tick 的 buff → 模拟时间推进 → 验证 tick 回调触发次数正确。

3. **净化行为**：
   - 创建 buff/debuff 混合 → 调用 `PurgeDebuffs("Debuff")` → 验证只删 debuff。

### 集成测试（test 项目 validation scenario）

当前框架已有 `Projects/test/Scripts/Process/ItemCompanionAbilityValidationScenario.cs`，可扩展或新增 `BuffValidationScenario.cs`：

1. **场景 1：控制独占**
   - 施加 2s 眩晕 → 1s 后施加 5s 眩晕 → 验证剩余时长 = 5s（不是 6s）。
   - 施加 5s 眩晕 → 1s 后施加 2s 眩晕 → 验证剩余时长 = 4s（不被覆盖）。

2. **场景 2：DoT tick**
   - 施加点燃 buff（5s，1s tick，10 伤害/次）→ 推进 5.5s → 验证目标受到 5 次伤害（总 50）。

3. **场景 3：净化级联**
   - 施加 debuff A → A 创建子 buff B（如果实现了子效果）→ 净化 A → 验证 B 也被删除。

### 手动测试（War3 客户端）

1. 创建测试技能（眩晕/点燃/净化），在 War3 客户端实际施放 → 验证 UI 图标显示、时长倒计时、DoT 伤害数字、净化清除效果。
2. 验证多单位同时受 DoT 时帧率稳定（100 单位各带 3 DoT = 300 tick/秒）。

---

## 回滚计划

如果本提案实施后发现严重问题（如性能瓶颈、逻辑 bug 导致游戏崩溃），回滚步骤：

1. **代码回滚**：`git revert` 提案的 commit。
2. **调用点恢复**：旧入口是兼容别名，调用点无需改动（除非已迁移到 `ApplyBuff`，需手工恢复）。
3. **数据迁移**：如果有保存的 buff 状态（当前框架无持久化），需清空。

**回滚风险**：如果已有技能依赖新增的 `ReplaceIfLonger` / tick / 净化功能，回滚后这些技能失效，需要重新设计。因此**提案验收必须严格**，确保核心逻辑正确后再合并。
