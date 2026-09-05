# Buff 应用入口统一与生命周期扩展 — Tasks

## 阶段划分

本提案分 6 个阶段实施，每个阶段独立验证，确保渐进式交付。

---

## T1. 枚举与组件扩展

### T1.1 扩展 BuffRefreshBehavior 枚举

- [ ] 在 `War3Frame/Src/Components/Buff.cs` 的 `BuffRefreshBehavior` 枚举中增加两个成员：
  - `Replace = 4`：删旧建新（完整替换）
  - `ReplaceIfLonger = 5`：仅当新 duration > 旧剩余时长时替换
- [ ] 验证：编译通过，无枚举值冲突。

### T1.2 扩展 Buff 组件

- [ ] 在 `War3Frame/Src/Components/Buff.cs` 的 `Buff` struct 中增加字段：
  ```csharp
  public long buffInstanceId;        // 实例 ID（全局唯一，用于级联清理）
  public List<string> tags;          // 分类标签（["Debuff", "Fire", "DoT"]）
  public float tickInterval;         // 周期 tick 间隔（秒，0 = 不 tick）
  public string? tickActionId;       // Tick 行为 ID（指向注册表）
  public float lastTick;             // 上次 tick 时间（内部字段，用于累积判断）
  ```
- [ ] 验证：编译通过，`Buff` 组件仍是 `IComponent`。

### T1.3 扩展 BuffBehavior 组件

- [ ] 在 `War3Frame/Src/Components/Buff.cs` 的 `BuffBehavior` struct 中增加字段：
  ```csharp
  public string? icon;  // UI 图标路径（如 "ReplaceableTextures\\CommandButtons\\BTNFireBolt.blp"）
  ```
- [ ] 验证：编译通过。

### T1.4 新增 IBuffTickAction 接口与注册表

- [ ] 在 `War3Frame/Src/Helpers/BuffHelper.cs` 中新增：
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
  ```
- [ ] 验证：编译通过。

---

## T2. BuffSpec 结构体与 ApplyBuff 工厂方法

### T2.1 新增 BuffSpec 结构体

- [ ] 在 `War3Frame/Src/Helpers/BuffHelper.cs` 中新增：
  ```csharp
  public readonly struct BuffSpec {
      public readonly string buffId;
      public readonly string? icon;
      public readonly int attrTypeId;         // 0 = 纯标记 buff（无属性贡献）
      public readonly ModifyType modifyType;
      public readonly float value;
      public readonly float duration;         // -1 = 永久
      public readonly int maxStacks;
      public readonly BuffRefreshBehavior onDuplicate;
      public readonly float tickInterval;     // 0 = 不 tick
      public readonly string? tickActionId;
      public readonly List<string> tags;
      
      // 构造函数（所有字段必填）
      public BuffSpec(
          string buffId,
          string? icon,
          int attrTypeId,
          ModifyType modifyType,
          float value,
          float duration,
          int maxStacks,
          BuffRefreshBehavior onDuplicate,
          float tickInterval,
          string? tickActionId,
          List<string> tags
      ) {
          this.buffId = buffId;
          this.icon = icon;
          this.attrTypeId = attrTypeId;
          this.modifyType = modifyType;
          this.value = value;
          this.duration = duration;
          this.maxStacks = maxStacks;
          this.onDuplicate = onDuplicate;
          this.tickInterval = tickInterval;
          this.tickActionId = tickActionId;
          this.tags = tags;
      }
  }
  ```
- [ ] 验证：编译通过，`BuffSpec` 是 `readonly struct`。

### T2.2 实现 ApplyBuff 工厂方法

- [ ] 在 `War3Frame/Src/Helpers/BuffHelper.cs` 中新增：
  ```csharp
  private static long _nextBuffId = 0;
  
  public static Entity ApplyBuff(EntityStore store, Entity unit, Entity source, in BuffSpec spec) {
      // 1. 查询既有 buff
      Entity? existingBuff = FindBuffByIdOnUnit(store, unit, spec.buffId);
      
      // 2. 处理重复触发
      if (existingBuff != null) {
          return HandleExistingBuff(store, existingBuff.Value, unit, source, in spec);
      }
      
      // 3. 创建新 buff
      return CreateNewBuff(store, unit, source, in spec);
  }
  
  private static Entity CreateNewBuff(EntityStore store, Entity unit, Entity source, in BuffSpec spec) {
      var buffEntity = store.CreateEntity();
      
      // Buff 组件
      var buff = buffEntity.Add<Buff>();
      buff.buffId = spec.buffId;
      buff.buffInstanceId = Interlocked.Increment(ref _nextBuffId);
      buff.tags = spec.tags;
      buff.tickInterval = spec.tickInterval;
      buff.tickActionId = spec.tickActionId;
      buff.lastTick = 0f;
      
      // Duration 组件
      buffEntity.Add<Duration>() = Duration.Create(spec.duration);
      
      // BuffBehavior 组件
      var behavior = buffEntity.Add<BuffBehavior>();
      behavior.buffId = spec.buffId;
      behavior.icon = spec.icon;
      behavior.refreshBehavior = spec.onDuplicate;
      behavior.removeAllStacksOnExpire = true;
      
      // ModifyValue 组件（仅当有属性贡献时）
      if (spec.attrTypeId != 0) {
          var attrEntity = AttributeHelper.GetAttr(unit, spec.attrTypeId);
          buffEntity.Add<ModifyValue>() = new ModifyValue {
              modifyType = spec.modifyType,
              value = spec.value,
              priority = (int)spec.modifyType
          };
          buffEntity.AddLink(new ModifyTarget(), attrEntity);
      }
      
      // ModifySource link（记录来源）
      buffEntity.AddLink(new ModifySource(), source);
      
      // ModifyTarget link（指向目标单位，即使无属性贡献也挂，用于反查）
      buffEntity.AddLink(new ModifyTarget(), unit);
      
      // BuffStacks 组件（仅当可叠层时）
      if (spec.maxStacks > 1) {
          buffEntity.Add<BuffStacks>() = new BuffStacks {
              current = 1,
              max = spec.maxStacks,
              valuePerStack = spec.value
          };
      }
      
      // 打脏
      unit.AddTag<AttrDirty>();
      
      return buffEntity;
  }
  
  private static Entity HandleExistingBuff(EntityStore store, Entity existingBuff, Entity unit, Entity source, in BuffSpec spec) {
      var behavior = existingBuff.Get<BuffBehavior>();
      
      switch (behavior.refreshBehavior) {
          case BuffRefreshBehavior.Replace:
              existingBuff.DeleteEntity();
              return CreateNewBuff(store, unit, source, in spec);
          
          case BuffRefreshBehavior.ReplaceIfLonger: {
              var duration = existingBuff.Get<Duration>();
              float remaining = duration.duration - duration.elapsed;
              if (spec.duration > remaining) {
                  existingBuff.DeleteEntity();
                  return CreateNewBuff(store, unit, source, in spec);
              }
              return existingBuff;
          }
          
          case BuffRefreshBehavior.RefreshDuration: {
              var duration = existingBuff.Get<Duration>();
              duration.elapsed = 0f;
              duration.duration = spec.duration;
              
              // 修正 P2：使用新 value
              if (existingBuff.Has<ModifyValue>()) {
                  var modifyValue = existingBuff.Get<ModifyValue>();
                  modifyValue.value = spec.value;
              }
              
              unit.AddTag<AttrDirty>();
              return existingBuff;
          }
          
          case BuffRefreshBehavior.RefreshAndStack: {
              var duration = existingBuff.Get<Duration>();
              duration.elapsed = 0f;
              duration.duration = spec.duration;
              
              if (existingBuff.Has<BuffStacks>()) {
                  var stacks = existingBuff.Get<BuffStacks>();
                  stacks.current = Math.Min(stacks.current + 1, stacks.max);
                  stacks.valuePerStack = spec.value;  // 修正 P3：更新每层值
                  
                  // 重算总值
                  if (existingBuff.Has<ModifyValue>()) {
                      var modifyValue = existingBuff.Get<ModifyValue>();
                      modifyValue.value = stacks.valuePerStack * stacks.current;
                  }
              }
              
              unit.AddTag<AttrDirty>();
              return existingBuff;
          }
          
          case BuffRefreshBehavior.AddStack: {
              // 不刷新 duration，只叠层
              if (existingBuff.Has<BuffStacks>()) {
                  var stacks = existingBuff.Get<BuffStacks>();
                  stacks.current = Math.Min(stacks.current + 1, stacks.max);
                  
                  if (existingBuff.Has<ModifyValue>()) {
                      var modifyValue = existingBuff.Get<ModifyValue>();
                      modifyValue.value = stacks.valuePerStack * stacks.current;
                  }
              }
              
              unit.AddTag<AttrDirty>();
              return existingBuff;
          }
          
          case BuffRefreshBehavior.Independent:
          default:
              return existingBuff;
      }
  }
  ```
- [ ] 验证：编译通过，逻辑覆盖 6 种 `onDuplicate` 行为。

---

## T3. 旧入口改为兼容别名

### T3.1 修改 AddTimedBuff

- [ ] 在 `War3Frame/Src/Helpers/BuffHelper.cs` 中修改 `AddTimedBuff` 方法：
  ```csharp
  public static Entity AddTimedBuff(
      EntityStore store,
      Entity unit,
      Entity source,
      int attrTypeId,
      ModifyType modifyType,
      float value,
      float duration,
      BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshDuration
  ) {
      return ApplyBuff(store, unit, source, new BuffSpec(
          buffId: $"attr:{attrTypeId}",  // 默认 buffId = 属性类型
          icon: null,
          attrTypeId: attrTypeId,
          modifyType: modifyType,
          value: value,
          duration: duration,
          maxStacks: 1,
          onDuplicate: refreshBehavior,
          tickInterval: 0f,
          tickActionId: null,
          tags: new List<string>()
      ));
  }
  ```
- [ ] 验证：编译通过，旧调用点行为不变。

### T3.2 修改 AddStackableBuff

- [ ] 修改为：
  ```csharp
  public static Entity AddStackableBuff(
      EntityStore store,
      Entity unit,
      Entity source,
      int attrTypeId,
      ModifyType modifyType,
      float valuePerStack,
      int maxStacks,
      float duration,
      BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshAndStack
  ) {
      return ApplyBuff(store, unit, source, new BuffSpec(
          buffId: $"attr:{attrTypeId}:stack",
          icon: null,
          attrTypeId: attrTypeId,
          modifyType: modifyType,
          value: valuePerStack,
          duration: duration,
          maxStacks: maxStacks,
          onDuplicate: refreshBehavior,
          tickInterval: 0f,
          tickActionId: null,
          tags: new List<string>()
      ));
  }
  ```
- [ ] 验证：编译通过。

### T3.3 修改 AddPermanentBuff

- [ ] 修改为：
  ```csharp
  public static Entity AddPermanentBuff(
      EntityStore store,
      Entity unit,
      Entity source,
      int attrTypeId,
      ModifyType modifyType,
      float value
  ) {
      return ApplyBuff(store, unit, source, new BuffSpec(
          buffId: $"attr:{attrTypeId}:perm",
          icon: null,
          attrTypeId: attrTypeId,
          modifyType: modifyType,
          value: value,
          duration: -1f,  // 永久
          maxStacks: 1,
          onDuplicate: BuffRefreshBehavior.Independent,
          tickInterval: 0f,
          tickActionId: null,
          tags: new List<string>()
      ));
  }
  ```
- [ ] 验证：编译通过。

---

## T4. 控制类 buff 与 DoT 便捷方法

### T4.1 新增控制 buff 便捷方法

- [ ] 在 `War3Frame/Src/Helpers/BuffHelper.cs` 中新增：
  ```csharp
  public static Entity Stun(EntityStore store, Entity unit, Entity source, float duration) {
      var stunAttrId = AttributeHelper.Register("Stun");
      return ApplyBuff(store, unit, source, new BuffSpec(
          buffId: "control:stun",
          icon: "ReplaceableTextures\\CommandButtons\\BTNStun.blp",
          attrTypeId: stunAttrId,
          modifyType: ModifyType.Flat,
          value: 1f,
          duration: duration,
          maxStacks: 1,
          onDuplicate: BuffRefreshBehavior.ReplaceIfLonger,
          tickInterval: 0f,
          tickActionId: null,
          tags: new List<string> { "Debuff", "Control", "Stun" }
      ));
  }
  
  public static Entity Root(EntityStore store, Entity unit, Entity source, float duration) {
      var rootAttrId = AttributeHelper.Register("Root");
      return ApplyBuff(store, unit, source, new BuffSpec(
          buffId: "control:root",
          icon: "ReplaceableTextures\\CommandButtons\\BTNEntangle.blp",
          attrTypeId: rootAttrId,
          modifyType: ModifyType.Flat,
          value: 1f,
          duration: duration,
          maxStacks: 1,
          onDuplicate: BuffRefreshBehavior.ReplaceIfLonger,
          tickInterval: 0f,
          tickActionId: null,
          tags: new List<string> { "Debuff", "Control", "Root" }
      ));
  }
  
  public static Entity Silence(EntityStore store, Entity unit, Entity source, float duration) {
      var silenceAttrId = AttributeHelper.Register("Silence");
      return ApplyBuff(store, unit, source, new BuffSpec(
          buffId: "control:silence",
          icon: "ReplaceableTextures\\CommandButtons\\BTNSilence.blp",
          attrTypeId: silenceAttrId,
          modifyType: ModifyType.Flat,
          value: 1f,
          duration: duration,
          maxStacks: 1,
          onDuplicate: BuffRefreshBehavior.ReplaceIfLonger,
          tickInterval: 0f,
          tickActionId: null,
          tags: new List<string> { "Debuff", "Control", "Silence" }
      ));
  }
  ```
- [ ] 验证：编译通过。

### T4.2 新增 DoT 便捷方法

- [ ] 新增：
  ```csharp
  public static Entity ApplyDoT(
      EntityStore store,
      Entity unit,
      Entity source,
      string buffId,
      float damagePerTick,
      float tickInterval,
      float duration,
      string? icon = null,
      List<string>? tags = null
  ) {
      return ApplyBuff(store, unit, source, new BuffSpec(
          buffId: buffId,
          icon: icon,
          attrTypeId: 0,  // 纯 tick，无属性贡献
          modifyType: ModifyType.Flat,
          value: damagePerTick,  // 存在 ModifyValue.value 里（虽然不挂 ModifyTarget）
          duration: duration,
          maxStacks: 1,
          onDuplicate: BuffRefreshBehavior.RefreshDuration,
          tickInterval: tickInterval,
          tickActionId: "DealDamage",
          tags: tags ?? new List<string> { "Debuff", "DoT" }
      ));
  }
  ```
- [ ] **注意**：`ApplyDoT` 的 `damagePerTick` 存在 `ModifyValue.value` 字段，但不挂 `ModifyTarget`（因为 `attrTypeId=0`）。Tick 行为从 `ModifyValue.value` 读取伤害值。
- [ ] 验证：编译通过。

---

## T5. BuffSystem tick 逻辑与内置 tick 行为

### T5.1 BuffSystem 增加 tick 循环

- [ ] 在 `War3Frame/Src/Systems/BuffSystem.cs` 的 `BuffSystem.OnUpdate` 方法中，**在现有 duration 推进逻辑之后**，增加：
  ```csharp
  // Tick 循环
  foreach (var (buff, duration, behavior) in Query<Buff, Duration, BuffBehavior>()) {
      if (buff.tickInterval <= 0) continue;
      
      while (duration.elapsed - buff.lastTick >= buff.tickInterval) {
          buff.lastTick += buff.tickInterval;
          
          var target = GetBuffTarget(buff.entity);  // 从 ModifyTarget link 读取目标
          if (target == null) continue;
          
          var action = BuffTickActionRegistry.Get(buff.tickActionId);
          action?.Execute(buff.entity, target.Value);
      }
  }
  
  private Entity? GetBuffTarget(Entity buffEntity) {
      if (buffEntity.TryGetIncomingLink<ModifyTarget>(out var link)) {
          return link.Target;
      }
      return null;
  }
  ```
- [ ] 验证：编译通过，逻辑在 duration 推进之后执行。

### T5.2 实现内置 DealDamage tick 行为

- [ ] 在 `War3Frame/Src/Systems/BuffSystem.cs` 中新增内部类：
  ```csharp
  private class DealDamageTickAction : IBuffTickAction {
      public void Execute(Entity buffEntity, Entity target) {
          if (!buffEntity.Has<ModifyValue>()) return;
          
          var modifyValue = buffEntity.Get<ModifyValue>();
          float damage = modifyValue.value;
          
          // 创建 DamageRequest
          var damageReq = Game.Store.CreateEntity();
          damageReq.Add<DamageRequest>() = new DamageRequest {
              // 从 ModifySource link 读取来源
              source = GetBuffSource(buffEntity),
              target = target,
              damage = damage,
              damageType = DamageType.Pure  // 默认真实伤害，技能可自定义
          };
      }
      
      private Entity GetBuffSource(Entity buffEntity) {
          if (buffEntity.TryGetIncomingLink<ModifySource>(out var link)) {
              return link.Target;
          }
          return buffEntity;  // fallback
      }
  }
  ```
- [ ] 在 `BuffSystem` 的静态初始化方法中注册：
  ```csharp
  static BuffSystem() {
      BuffTickActionRegistry.Register("DealDamage", new DealDamageTickAction());
  }
  ```
- [ ] 验证：编译通过。

---

## T6. 级联清理方法

### T6.1 实现 PurgeDebuffsWithCascade

- [ ] 在 `War3Frame/Src/Helpers/BuffHelper.cs` 中新增：
  ```csharp
  public static void PurgeDebuffsWithCascade(EntityStore store, Entity unit, string tagFilter = "Debuff") {
      var buffIdsToRemove = new HashSet<long>();
      
      // 收集要删的 buff 实例 ID
      foreach (var (buff, target) in store.Query<Buff, ModifyTarget>()) {
          if (target.Target == unit && buff.tags.Contains(tagFilter)) {
              buffIdsToRemove.Add(buff.buffInstanceId);
          }
      }
      
      // 删 buff 本身
      foreach (var (buff, _) in store.Query<Buff, ModifyTarget>()) {
          if (buffIdsToRemove.Contains(buff.buffInstanceId)) {
              buff.Entity.DeleteEntity();
          }
      }
      
      // 级联删子效果（预留扩展点）
      // 当前无独立子效果实体（SummonedUnit / DotEffect），跳过
      // 未来实现时在此补充：
      // foreach (var summon in store.Query<SummonedUnit>()) {
      //     if (buffIdsToRemove.Contains(summon.sourceBuffId)) {
      //         summon.Entity.DeleteEntity();
      //     }
      // }
      
      // 打脏
      unit.AddTag<AttrDirty>();
  }
  ```
- [ ] 验证：编译通过。

---

## T7. 调用点迁移

### T7.1 BuffApplyResolveSystem

- [ ] 在 `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs` 的 `BuffApplyResolveSystem.OnUpdate` 中：
  - 找到 `BuffHelper.AddTimedBuff(...)` 调用。
  - **保持不变**（因为 `AddTimedBuff` 已改为别名，行为自动升级）。
- [ ] 验证：调用点行为不变（刷新使用新 value，修复 P2）。

### T7.2 GroundAreaBuffSystem

- [ ] 在 `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs` 的 `GroundAreaBuffSystem.OnUpdate` 中：
  - 找到 `BuffHelper.AddPermanentBuff(...)` 调用。
  - **保持不变**。
- [ ] 验证：地面区域 buff 行为不变。

### T7.3 AuraSystem

- [ ] 在 `War3Frame/Src/Systems/AuraSystem.cs` 的 `AuraSystem.OnUpdate` 中：
  - 找到 `BuffHelper.AddPermanentBuff(...)` 调用。
  - **保持不变**（保留 `AuraBuffLink` 挂载逻辑）。
- [ ] 验证：光环 buff 行为不变。

---

## T8. EffectChainBuilder 便捷步骤

### T8.1 新增控制步骤

- [ ] 在 `War3Frame/Src/Helpers/EffectChainBuilder.cs` 中新增：
  ```csharp
  public EffectChainBuilder Stun(float duration) {
      return Buff(new BuffEffectStepSpec {
          buffId = "control:stun",
          icon = "ReplaceableTextures\\CommandButtons\\BTNStun.blp",
          attrTypeId = AttributeHelper.Register("Stun"),
          modifyType = ModifyType.Flat,
          value = 1f,
          durationFormula = Formula.Constant(duration),
          maxStacks = 1,
          onDuplicate = BuffRefreshBehavior.ReplaceIfLonger,
          tags = new List<string> { "Debuff", "Control", "Stun" }
      });
  }
  
  public EffectChainBuilder Root(float duration) {
      return Buff(new BuffEffectStepSpec {
          buffId = "control:root",
          icon = "ReplaceableTextures\\CommandButtons\\BTNEntangle.blp",
          attrTypeId = AttributeHelper.Register("Root"),
          modifyType = ModifyType.Flat,
          value = 1f,
          durationFormula = Formula.Constant(duration),
          maxStacks = 1,
          onDuplicate = BuffRefreshBehavior.ReplaceIfLonger,
          tags = new List<string> { "Debuff", "Control", "Root" }
      });
  }
  ```
- [ ] 验证：编译通过。

### T8.2 新增 DoT 步骤

- [ ] 新增：
  ```csharp
  public EffectChainBuilder DoT(string buffId, float damagePerTick, float tickInterval, float duration, string? icon = null) {
      return Buff(new BuffEffectStepSpec {
          buffId = buffId,
          icon = icon,
          attrTypeId = 0,  // 无属性贡献
          modifyType = ModifyType.Flat,
          value = damagePerTick,
          durationFormula = Formula.Constant(duration),
          maxStacks = 1,
          onDuplicate = BuffRefreshBehavior.RefreshDuration,
          tickInterval = tickInterval,
          tickActionId = "DealDamage",
          tags = new List<string> { "Debuff", "DoT" }
      });
  }
  ```
- [ ] **注意**：需要在 `BuffEffectStepSpec` 中增加 `tickInterval` / `tickActionId` / `tags` 字段。
- [ ] 验证：编译通过。

---

## T9. 模板示例补充

### T9.1 控制技能示例

- [ ] 在 `Projects/test/Scripts/Template/Ability.cs` 中新增：
  ```csharp
  [AbilityTemplate("stun_hammer")]
  public class StunHammerAbility : IAbilityTemplate {
      public void Configure(AbilitySpecBuilder builder) {
          builder
              .TargetUnit(castRange: 600f)
              .OnEffect(e => e
                  .ProjectileLine(speed: 1200f)
                  .OnArrive(a => a
                      .Stun(duration: 2f)
                      .Damage(damageFormula: Formula.Constant(100f), damageType: DamageType.Physical)
                  )
              );
      }
  }
  ```
- [ ] 验证：编译通过，模板注册成功。

### T9.2 DoT 技能示例

- [ ] 新增：
  ```csharp
  [AbilityTemplate("ignite")]
  public class IgniteAbility : IAbilityTemplate {
      public void Configure(AbilitySpecBuilder builder) {
          builder
              .TargetUnit(castRange: 600f)
              .OnEffect(e => e
                  .ProjectileLine(speed: 800f)
                  .OnArrive(a => a
                      .DoT(
                          buffId: "ignite",
                          damagePerTick: 10f,
                          tickInterval: 1f,
                          duration: 5f,
                          icon: "ReplaceableTextures\\CommandButtons\\BTNFireBolt.blp"
                      )
                  )
              );
      }
  }
  ```
- [ ] 验证：编译通过。

---

## T10. 验证与测试

### T10.1 局部静态核对

- [ ] 三种形态组件集等价：验证 `AddTimedBuff` / `AddStackableBuff` / `AddPermanentBuff` 与直接调用 `ApplyBuff` 的结果组件集相同。
- [ ] `Replace` / `ReplaceIfLonger` 时序正确：
  - 2s 后 5s → 剩 5s（Replace）
  - 5s 后 2s → 剩 5s（ReplaceIfLonger 不覆盖）
- [ ] P2/P3 修复验证：`RefreshDuration` / `RefreshAndStack` 使用新 spec 的 duration/value。

### T10.2 Tick 行为验证

- [ ] 创建点燃 buff（5s，1s tick，10 伤害/次）→ 模拟推进 5.5s → 验证目标受到 5 次 `DamageRequest`（总 50 伤害）。
- [ ] 删除 buff 中途 → 验证 tick 停止。

### T10.3 净化验证

- [ ] 创建 2 个 debuff（点燃 + 减速）+ 1 个 buff（加速）→ 调用 `PurgeDebuffsWithCascade(unit, "Debuff")` → 验证只删 debuff，buff 保留。
- [ ] 验证删除后触发 `AttrDirty`，属性重算正确。

### T10.4 集成测试（test 项目）

- [ ] 在 `Projects/test` 创建 validation item：
  - 使用 `stun_hammer` 技能 → 验证目标被眩晕 2s。
  - 使用 `ignite` 技能 → 验证目标每秒掉血。
- [ ] 运行 `dotnet run --project CSharpWar3Frame -- run test` → 验证场景通过。

### T10.5 War3 客户端验证

- [ ] 构建 test 地图 → 进入 War3 客户端 → 施放眩晕/点燃技能 → 验证：
  - UI 显示 buff 图标。
  - 时长倒计时正确。
  - DoT 每秒触发伤害数字。
  - 净化技能清除 debuff。

---

## T11. 文档更新

- [ ] 更新 `ARCHITECTURE.md` 补充 Buff tick 系统描述。
- [ ] 更新 `STRUCTURE.md` 补充 `BuffTickActionRegistry` 位置。

---

## 里程碑

- **T1-T3 完成**：组件扩展 + 入口统一，旧代码兼容。
- **T4-T5 完成**：控制/DoT 便捷方法 + tick 系统落地。
- **T6-T7 完成**：净化 + 调用点迁移。
- **T8-T9 完成**：模板示例补充。
- **T10 完成**：全面验证通过，提案可合并。
