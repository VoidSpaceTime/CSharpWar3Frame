using System;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 以本地 ECS Store + 手工系统树同步验证技能点闭环与死亡事件模型：
/// 击杀 → UnitDiedEvent 广播 → KillRewardSystem 派发经验 → ExperienceSystem 升级同步发点 + UnitLeveledEvent
/// → 加点工作流升级槽位技能 → 失败矩阵 → 熟练度守卫 → 同帧超杀只发一次 → 非击杀/无奖励不派发。
/// </summary>
public static class SkillPointUpgradeScenario
{
    private const string ScenarioName = "SkillPointUpgradeScenario";

    public static void Initialize(JPlayer player)
    {
        _ = player;
        RunValidation();
    }

    public static void Update()
    {
        // 同步验证已在 Initialize 中完成。
    }

    private static void RunValidation()
    {
        RunDeathKillRewardClosedLoop();
        RunUpgradeWorkflowValidation();
        RunLethalKillOnceValidation();
        RunNonKillNoRewardValidation();

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    // =====================================================================
    // A. 击杀 → UnitDiedEvent → KillRewardSystem → ExperienceSystem 完整闭环
    // =====================================================================

    /// <summary>
    /// 验证击杀奖励闭环：KillUnit 广播一条 UnitDiedEvent → KillRewardSystem 按死者等级解析派发
    /// → ExperienceSystem 升级并同步发点、广播一条 UnitLeveledEvent。
    /// </summary>
    private static void RunDeathKillRewardClosedLoop()
    {
        var store = new EntityStore();
        // 同帧顺序模拟生产 order：KillRewardSystem(126) 先于 ExperienceSystem(0) 前一个结算帧内新建请求；
        // 本 root 先 add KillRewardSystem，保证一次 Update 完成 事件→请求→经验 全链路。
        var root = new TimedSystemRoot(store);
        root.Add(new KillRewardSystem(), 0f);
        root.Add(new ExperienceSystem(), 0f);

        // 击杀者：带经验曲线（LevelTable 50/100）与点池 perLevel=1。
        var hero = store.CreateEntity(
            new UnitLevel { level = 1 },
            new ExperienceData
            {
                currentExp = 0f,
                totalExp = 0f,
                maxLevel = 5,
                curve = ExperienceCurve.LevelTable(50f, 100f)
            },
            new SkillPointPool { unspent = 0, earned = 0, perLevel = 1 });

        // victim 等级 2，经验奖励 PerLevel(50,25) => 50 + 1*25 = 75。
        var victim = store.CreateEntity(
            new UnitLevel { level = 2 },
            new UnitKillRewardData { expReward = LevelValue.PerLevel(50f, 25f) },
            new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive });

        // Phase 1：致死当次 KillUnit 广播一条死亡事件（payload + marker）。
        Require(UnitHelper.KillUnit(victim, hero), "kill/transition: KillUnit 应成功转移 Alive→Death");
        Require(CountEvents<UnitDiedEvent>(store) == 1, "kill/diedEvent: 应恰有一条 UnitDiedEvent");
        var died = FindEvent<UnitDiedEvent>(store);
        Require(died.HasValue && died.Value.value.unit == victim && died.Value.value.source == hero,
            "kill/diedPayload: UnitDiedEvent 应记录 unit=victim / source=hero");
        Require(HasMarker(died.Value.entity), "kill/diedMarker: 死亡事件应挂 TriggerEventMarker");

        // Phase 2：一次 Update 内 KillRewardSystem 派发 75 经验，ExperienceSystem 消费升级。
        root.Update(new UpdateTick(0f, 0f));
        Require(CountEvents<ExperienceGainRequest>(store) == 0, "kill/consume: 经验请求应被消费删除");
        Require(store.GetEntityById(hero.Id).TryGetComponent<UnitLevel>(out var heroLevel) && heroLevel.level == 2,
            "kill/up: 英雄应升到 2 级");
        // currentExp == 25 反证奖励量为 75：若只发 50 则升级后剩 0，发 75 剩 25。
        Require(Math.Abs(store.GetEntityById(hero.Id).GetComponent<ExperienceData>().currentExp - 25f) < 0.001f,
            "kill/amount: 击杀奖励经验应为 75（升级后余 25）");
        Require(store.GetEntityById(hero.Id).TryGetComponent<SkillPointPool>(out var poolAfterUp)
                && poolAfterUp.unspent == 1 && poolAfterUp.earned == 1,
            "kill/grant: 升 1 级应按 perLevel=1 发 1 点");
        Require(CountEvents<UnitLeveledEvent>(store) == 1, "kill/levelEvent: 应恰有一条 UnitLeveledEvent");
        var leveledEvent = FindEvent<UnitLeveledEvent>(store);
        Require(leveledEvent.HasValue && leveledEvent.Value.value.toLevel == 2 && leveledEvent.Value.value.fromLevel == 1,
            "kill/levelRange: UnitLeveledEvent 应为 1 -> 2");
        Require(HasMarker(leveledEvent.Value.entity), "kill/levelMarker: 升级事件应挂 TriggerEventMarker");
    }

    // =====================================================================
    // B. 加点工作流（成功 / 失败矩阵 / 熟练度守卫）
    // =====================================================================

    /// <summary>
    /// 验证加点闭环：1 点升级槽位技能、baseValues 随等级重算；点数不足/满级/Item companion/
    /// maxLevel&lt;=0/非 owner 失败且状态不变；maxLevel&gt;0 技能熟练度经验被守卫只累计不升级。
    /// </summary>
    private static void RunUpgradeWorkflowValidation()
    {
        var store = new EntityStore();
        var root = CreateSystemRoot(store);

        // 英雄从击杀闭环的等价状态出发：2 级 + 1 点。
        var hero = store.CreateEntity(
            new UnitLevel { level = 2 },
            new SkillPointPool { unspent = 1, earned = 1, perLevel = 1 },
            AbilitySlotContainer.WithSlots(6));

        // ---- 加点成功：1 点升 1 级 ----
        var spec = new AbilitySpec
        {
            templateName = "sp_sp_bolt",
            name = "点法技能",
            description = string.Empty,
            maxLevel = 3
        };
        spec.baseValues[AbilityHelper.DamageAmount] = LevelValue.PerLevel(100f, 50f);
        var ability = CreateSlotAbility(store, hero, "sp_sp_bolt", spec, level: 1, slotIndex: 0);

        SkillPointHelper.Upgrade(hero, ability, 1);
        root.Update(new UpdateTick(0f, 0f));
        Require(ability.TryGetComponent<AbilityBase>(out var upgraded) && upgraded.level == 2,
            "upgrade/level: 加点后技能应为 2 级");
        Require(store.GetEntityById(hero.Id).TryGetComponent<SkillPointPool>(out var poolAfterSpend)
                && poolAfterSpend.unspent == 0,
            "upgrade/spend: 加点后应扣 1 点");
        Require(CountEvents<AbilityUpgradedEvent>(store) == 1, "upgrade/event: 应恰有一条 AbilityUpgradedEvent");
        var upgradedEvent = FindEvent<AbilityUpgradedEvent>(store);
        Require(upgradedEvent.HasValue && upgradedEvent.Value.value.fromLevel == 1
                && upgradedEvent.Value.value.toLevel == 2 && upgradedEvent.Value.value.pointsSpent == 1,
            "upgrade/eventData: AbilityUpgradedEvent 应为 1 -> 2, 消耗 1 点");
        Require(HasMarker(upgradedEvent.Value.entity), "upgrade/eventMarker: 加点事件应挂 TriggerEventMarker");
        Require(!ability.Tags.Has<LevelStatDirty>(), "upgrade/rebuild: 重算后应清除 LevelStatDirty");

        // baseValues 已按 2 级重算（100 + (2-1)*50 = 150）。
        Require(AbilityHelper.TryGetStat(ability, AbilityHelper.DamageAmount, out var stat)
                && Math.Abs(stat.GetComponent<AbilityStatValue>().baseValue - 150f) < 0.001f,
            "upgrade/stat: DamageAmount baseValue 应按 2 级解析为 150");

        // ---- 失败矩阵 ----
        // 4a 点数不足（当前 0 点）。
        SkillPointHelper.Upgrade(hero, ability, 1);
        root.Update(new UpdateTick(0f, 0f));
        Require(ability.GetComponent<AbilityBase>().level == 2, "fail/noPoint: 无点升级应失败且等级不变");
        Require(CountEvents<AbilityUpgradedEvent>(store) == 1, "fail/noPointEvent: 无点失败不应发事件");

        // 4b 升到满级后再升失败。
        SkillPointHelper.GrantLevels(hero, 2); // unspent 0 -> 2
        SkillPointHelper.Upgrade(hero, ability, 1); // 2 -> 3 (满级)
        root.Update(new UpdateTick(0f, 0f));
        Require(ability.GetComponent<AbilityBase>().level == 3, "fail/maxReach: 应可升到 3 级(满级)");
        SkillPointHelper.Upgrade(hero, ability, 1); // 3 -> 4 超上限
        root.Update(new UpdateTick(0f, 0f));
        Require(ability.GetComponent<AbilityBase>().level == 3, "fail/max: 满级后升级应失败");
        Require(store.GetEntityById(hero.Id).GetComponent<SkillPointPool>().unspent == 1,
            "fail/maxNoSpend: 满级失败不应扣点");

        // 4c Item companion 拒绝（非 Slot 挂载）。
        var companionSpec = new AbilitySpec { templateName = "sp_item_use", maxLevel = 3 };
        var companion = store.CreateEntity(
            new AbilityBase { templateName = "sp_item_use", level = 1, state = AbilityState.Ready },
            new AbilityMountInfo { mountType = AbilityMountType.ItemGranted },
            new AbilitySpecData { spec = companionSpec });
        companion.AddComponent(new AbilityOwner(hero));
        SkillPointHelper.Upgrade(hero, companion, 1);
        root.Update(new UpdateTick(0f, 0f));
        Require(companion.GetComponent<AbilityBase>().level == 1, "fail/companion: Item companion 加点应被拒绝");

        // 4d maxLevel <= 0 的技能不可加点。
        var noMaxSpec = new AbilitySpec { templateName = "sp_proficiency" };
        var noMaxAbility = CreateSlotAbility(store, hero, "sp_proficiency", noMaxSpec, level: 1, slotIndex: 1);
        SkillPointHelper.Upgrade(hero, noMaxAbility, 1);
        root.Update(new UpdateTick(0f, 0f));
        Require(noMaxAbility.GetComponent<AbilityBase>().level == 1, "fail/noMax: maxLevel<=0 加点应被拒绝");

        // 4e 非 owner 拒绝。
        var otherUnit = store.CreateEntity(new UnitLevel { level = 1 });
        var otherSpec = new AbilitySpec { templateName = "sp_other", maxLevel = 3 };
        var otherAbility = CreateSlotAbility(store, otherUnit, "sp_other", otherSpec, level: 1, slotIndex: 0);
        SkillPointHelper.Upgrade(hero, otherAbility, 1);
        root.Update(new UpdateTick(0f, 0f));
        Require(otherAbility.GetComponent<AbilityBase>().level == 1, "fail/notOwner: 非 owner 加点应被拒绝");

        // ---- 熟练度守卫 ----
        // maxLevel>0 的技能收到熟练度经验后，等级不被经验系统改动，经验仍累计。
        var guardSpec = new AbilitySpec { templateName = "sp_guard", maxLevel = 3 };
        var guardAbility = CreateSlotAbility(store, hero, "sp_guard", guardSpec, level: 2, slotIndex: 2);
        guardAbility.AddComponent(new ExperienceData
        {
            currentExp = 0f,
            totalExp = 0f,
            maxLevel = 99,
            curve = ExperienceCurve.FixedStep(50f)
        });
        store.CreateEntity(new ExperienceGainRequest
        {
            target = guardAbility,
            amount = 200f,
            multiplier = 1f,
            source = hero,
            sourceType = "proficiency"
        });
        root.Update(new UpdateTick(0f, 0f));
        Require(guardAbility.GetComponent<AbilityBase>().level == 2, "guard/level: 加点技能不应被经验自动升级");
        Require(Math.Abs(guardAbility.GetComponent<ExperienceData>().currentExp - 200f) < 0.001f,
            "guard/exp: 加点技能经验仍应累计");
    }

    // =====================================================================
    // C. 同帧超杀：只发一次（真实 DamageResolveSystem 死亡分支）
    // =====================================================================

    /// <summary>
    /// 同帧两发都能把血打到 0 的伤害经 DamageResolveSystem：只有致死当次 KillUnit 广播事件并派发一次经验请求。
    /// </summary>
    private static void RunLethalKillOnceValidation()
    {
        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        root.Add(new DamageResolveSystem(), 0f);
        root.Add(new KillRewardSystem(), 0f);

        var killer = store.CreateEntity(new UnitLevel { level = 1 });
        var victim = store.CreateEntity(
            new UnitLevel { level = 1 },
            new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive },
            new UnitKillRewardData { expReward = LevelValue.Fixed(100f) });
        CreateLocalAttr(store, victim, AttributeHelper.Health, 50f);

        CreateDamageRequest(store, killer, victim, 100f);
        CreateDamageRequest(store, killer, victim, 100f);
        root.Update(new UpdateTick(0f, 0f));

        Require(CountEvents<UnitDiedEvent>(store) == 1, "kill/once: 同帧超杀只应广播一条 UnitDiedEvent");
        Require(CountExperienceRequests(store, killer) == 1, "kill/onceReq: 同帧超杀只应派发一次击杀经验");
        Require(store.GetEntityById(victim.Id).TryGetComponent<UnitLifeState>(out var life)
                && life.lifePhase == UnitLifecyclePhase.Death,
            "kill/phase: 受害者应进入 Death");
    }

    // =====================================================================
    // D. 非击杀死亡 / 无奖励死者：事件照发，奖励不派发
    // =====================================================================

    /// <summary>
    /// source 为空（非击杀死亡）或死者无 expReward：UnitDiedEvent 照常广播，但 KillRewardSystem 不派发经验。
    /// </summary>
    private static void RunNonKillNoRewardValidation()
    {
        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        root.Add(new KillRewardSystem(), 0f);

        var attacker = store.CreateEntity(new UnitLevel { level = 1 });
        var victimNoSource = store.CreateEntity(
            new UnitLevel { level = 1 },
            new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive },
            new UnitKillRewardData { expReward = LevelValue.Fixed(100f) });
        var victimNoReward = store.CreateEntity(
            new UnitLevel { level = 1 },
            new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive });

        Require(UnitHelper.KillUnit(victimNoSource), "nonkill/killUnit: 无击杀者也应能广播死亡");
        Require(UnitHelper.KillUnit(victimNoReward, attacker), "nonkill/noRewardKill: 无奖励死者也应能广播死亡");
        Require(CountEvents<UnitDiedEvent>(store) == 2, "nonkill/diedEvents: 两例都应广播死亡事件");

        root.Update(new UpdateTick(0f, 0f));
        Require(CountEvents<ExperienceGainRequest>(store) == 0,
            "nonkill/noReward: 非击杀死亡与无奖励死者均不应派发经验");
    }

    /// <summary>按执行顺序创建加点链路本地系统树：经验 → 加点 → 等级重算。</summary>
    private static TimedSystemRoot CreateSystemRoot(EntityStore store)
    {
        var root = new TimedSystemRoot(store);
        root.Add(new ExperienceSystem(), 0f);
        root.Add(new AbilityUpgradeWorkflowSystem(), 0f);
        root.Add(new AbilityLevelStatRebuildSystem(), 0f);
        return root;
    }

    /// <summary>本地构建槽位技能实体（模板名/等级/spec/归属/槽位/挂载）。</summary>
    private static Entity CreateSlotAbility(EntityStore store, Entity unit, string templateName, AbilitySpec spec,
        int level, int slotIndex)
    {
        var ability = store.CreateEntity(
            new AbilityBase
            {
                templateName = templateName,
                level = level,
                Name = templateName,
                Description = string.Empty,
                state = AbilityState.Ready
            },
            new AbilityMountInfo { mountType = AbilityMountType.Slot },
            new AbilitySlotIndex { slotIndex = slotIndex },
            new AbilitySpecData { spec = spec });
        ability.AddComponent(new AbilityOwner(unit));
        return ability;
    }

    /// <summary>本地建属性实体（绕开 AttributeHelper.CreateAttr 对 Game.Store 的依赖）。</summary>
    private static void CreateLocalAttr(EntityStore store, Entity unit, int typeId, float baseValue)
    {
        var attr = store.CreateEntity(
            new AttrTypeId { typeId = typeId },
            new AttrValue { baseValue = baseValue, finalValue = baseValue, current = baseValue },
            new AttrOwner(unit));
        unit.AddRelation(new HasAttr(attr, typeId));
    }

    /// <summary>写入一条伤害请求，供 DamageResolveSystem 消费。</summary>
    private static void CreateDamageRequest(EntityStore store, Entity source, Entity target, float amount)
    {
        store.CreateEntity(new DamageRequest
        {
            source = source,
            target = target,
            damage = new DamageBase
            {
                damage = amount,
                damageType = DamageType.Real,
                damageSrc = DamageSrc.Skill,
                source = source,
                target = target
            }
        });
    }

    /// <summary>统计指向指定单位的经验请求数量。</summary>
    private static int CountExperienceRequests(EntityStore store, Entity target)
    {
        var count = 0;
        store.Query<ExperienceGainRequest>().ForEachEntity((ref ExperienceGainRequest request, Entity _) =>
        {
            if (request.target == target)
                count++;
        });
        return count;
    }

    private static (T value, Entity entity)? FindEvent<T>(EntityStore store) where T : struct, IComponent
    {
        (T value, Entity entity)? found = null;
        store.Query<T>().ForEachEntity((ref T value, Entity entity) => { found = (value, entity); });
        return found;
    }

    private static int CountEvents<T>(EntityStore store) where T : struct, IComponent
    {
        var count = 0;
        store.Query<T>().ForEachEntity((ref T _, Entity _) => count++);
        return count;
    }

    private static bool HasMarker(Entity entity)
    {
        return !entity.IsNull && entity.HasComponent<TriggerEventMarker>();
    }

    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}
