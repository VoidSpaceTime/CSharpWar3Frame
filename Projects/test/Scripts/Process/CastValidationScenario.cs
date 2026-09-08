using System;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 以本地 ECS Store 同步验证施法打断 + 控制校验 + 目标死亡取消（cast-interrupt-and-validation）：
/// 受控禁止发起新施法、眩晕打断前摇吟唱、沉默打断持续引导、前摇中目标死亡取消施法。
/// 说明：本地无 War3 客户端，无 Native 依赖（控制属性直接在 ECS 叠加）；同 ControlStateValidationScenario 范式。
/// 本场景与验证 runner 逻辑一致，供真实 War3 客户端运行（Program.cs 注册）。
/// </summary>
public static class CastValidationScenario
{
    private const string ScenarioName = "CastValidationScenario";
    private static TimedSystemRoot? _root;

    public static void Initialize(JPlayer player)
    {
        _ = player;
        RunValidation();
    }

    public static void Update()
    {
        // 同步验证已在 Initialize 中完成，运行时更新无需操作。
    }

    private static void RunValidation()
    {
        var store = new EntityStore();
        _root = CreateSystemRoot(store);

        // ===== Phase 1：REQ 受控（眩晕）禁止发起新施法 =====
        {
            var unit = CreateCaster(store, stunned: true);
            var ability = CreateAbility(store, castTime: 5f, channelDuration: 0f);
            RequestCast(store, unit, ability, targetUnit: default);
            Step(0.05f);
            Require(!unit.HasComponent<CastRequest>(), "phase1/reqRemoved: CastRequest 应被移除");
            Require(!unit.HasComponent<CastState>(), "phase1/noCast: 受控单位不应进入 CastState");
            Require(GetAbilityState(ability) == AbilityState.Ready, "phase1/state: 技能应保持 Ready");
        }

        // ===== Phase 2：眩晕打断前摇吟唱 =====
        {
            var unit = CreateCaster(store, stunned: false);
            var ability = CreateAbility(store, castTime: 5f, channelDuration: 0f);
            RequestCast(store, unit, ability, targetUnit: default);
            Step(0.05f);
            Require(unit.HasComponent<CastState>(), "phase2/entered: 应进入 Casting");
            Require(unit.GetComponent<CastState>().phase == CastPhase.Casting, "phase2/phase: 应处于 Casting");

            var stunAttr = CreateAttr(store, unit, AttributeHelper.Stun, 1f);
            Step(0.05f);
            Require(!unit.HasComponent<CastState>(), "phase2/interrupted: 眩晕后 CastState 应被移除");
            Require(GetAbilityState(ability) == AbilityState.Ready, "phase2/ready: 打断后技能回 Ready");
            Require(!ability.HasComponent<AbilityCooldownState>(), "phase2/noCooldown: 打断不应进冷却");
            _ = stunAttr;
        }

        // ===== Phase 3：沉默打断持续引导 =====
        {
            var unit = CreateCaster(store, stunned: false);
            var ability = CreateAbility(store, castTime: 0f, channelDuration: 5f);
            RequestCast(store, unit, ability, targetUnit: default);
            Step(0.06f);
            Require(unit.TryGetComponent<CastState>(out var cast0) && cast0.phase == CastPhase.Channeling,
                "phase3/channeling: 应进入 Channeling");
            Require(unit.HasComponent<ChannelState>(), "phase3/channelState: 应挂 ChannelState");

            var silenceAttr = CreateAttr(store, unit, AttributeHelper.Silence, 1f);
            Step(0.06f);
            Require(!unit.HasComponent<CastState>(), "phase3/interrupted: 沉默后引导应被打断");
            Require(!unit.HasComponent<ChannelState>(), "phase3/channelGone: ChannelState 应被移除");
            Require(GetAbilityState(ability) == AbilityState.Ready, "phase3/ready: 打断后技能回 Ready");
            _ = silenceAttr;
        }

        // ===== Phase 4：前摇吟唱中目标死亡 → 生效点前取消施法 =====
        {
            var unit = CreateCaster(store, stunned: false);
            var target = CreateTarget(store, alive: true);
            var ability = CreateAbility(store, castTime: 1f, channelDuration: 0f);
            RequestCast(store, unit, ability, targetUnit: target);
            Step(0.1f);
            Require(unit.TryGetComponent<CastState>(out var c1) && c1.phase == CastPhase.Casting,
                "phase4/casting: 应处于 Casting");

            SetAlive(target, false);
            Step(1.1f);
            Require(!unit.HasComponent<CastState>(), "phase4/cancelled: 目标死亡应取消施法");
            Require(GetAbilityState(ability) == AbilityState.Ready, "phase4/ready: 取消后技能回 Ready");
        }

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    private static TimedSystemRoot CreateSystemRoot(EntityStore store)
    {
        var root = new TimedSystemRoot(store);
        root.Add(new CastRequestSystem(), 0f);
        root.Add(new CastingSystem(), 0f);
        root.Add(new ChannelingSystem(), 0f);
        return root;
    }

    private static Entity CreateCaster(EntityStore store, bool stunned)
    {
        var unit = store.CreateEntity(new Position { x = 0f, y = 0f, z = 0f });
        if (stunned)
            CreateAttr(store, unit, AttributeHelper.Stun, 1f);
        return unit;
    }

    private static Entity CreateTarget(EntityStore store, bool alive)
    {
        var target = store.CreateEntity(new Position { x = 0f, y = 0f, z = 0f });
        target.AddComponent(new UnitLifeState
        {
            isAlive = alive,
            lifePhase = alive ? UnitLifecyclePhase.Alive : UnitLifecyclePhase.Death,
        });
        return target;
    }

    private static void SetAlive(Entity target, bool alive)
    {
        target.AddComponent(new UnitLifeState
        {
            isAlive = alive,
            lifePhase = alive ? UnitLifecyclePhase.Alive : UnitLifecyclePhase.Death,
        });
    }

    /// <summary>创建技能实体：AbilityBase Ready + stat（CastTime/ChannelDuration/Range/ManaCost）。</summary>
    private static Entity CreateAbility(EntityStore store, float castTime, float channelDuration)
    {
        var ability = store.CreateEntity(new AbilityBase
        {
            templateName = "verify_ability",
            Name = "Verify",
            Description = "",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.None,
        });
        SetStat(ability, AbilityHelper.CastTime, castTime);
        SetStat(ability, AbilityHelper.ChannelDuration, channelDuration);
        SetStat(ability, AbilityHelper.ChannelTickInterval, 0f);
        SetStat(ability, AbilityHelper.Range, 9999f);
        SetStat(ability, AbilityHelper.ManaCost, 0f);
        SetStat(ability, AbilityHelper.BackswingDuration, 0f);
        return ability;
    }

    /// <summary>本地 stat 辅助：直接建 AbilityStatValue（finalValue 一次到位，不依赖重算系统）。</summary>
    private static void SetStat(Entity ability, int typeId, float value)
    {
        var stat = ability.Store.CreateEntity(
            new AbilityStatValue { baseValue = value, currentValue = value, finalValue = value },
            new AbilityStatOwner(ability));
        ability.AddRelation(new HasAbilityStat(stat, typeId));
    }

    private static void RequestCast(EntityStore store, Entity unit, Entity ability, Entity targetUnit)
    {
        unit.AddComponent(new CastRequest
        {
            ability = ability,
            targetUnit = targetUnit,
            targetX = 0f,
            targetY = 0f,
        });
    }

    private static void Step(float delta)
    {
        _root!.Update(new UpdateTick(delta, 0f));
    }

    private static AbilityState GetAbilityState(Entity ability)
    {
        return ability.GetComponent<AbilityBase>().state;
    }

    /// <summary>本地建属性实体（对应 AttributeHelper.CreateAttr，绕开 Game.Store）。</summary>
    private static Entity CreateAttr(EntityStore store, Entity unit, int typeId, float baseValue)
    {
        var attr = store.CreateEntity(
            new AttrTypeId { typeId = typeId },
            new AttrValue { baseValue = baseValue, finalValue = baseValue, current = baseValue },
            new AttrOwner(unit));
        unit.AddRelation(new HasAttr(attr, typeId));
        return attr;
    }

    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}
