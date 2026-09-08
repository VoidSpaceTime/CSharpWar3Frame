using System;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Src.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 以本地 ECS Store 同步验证战斗结算管线（damage-resolution-pipeline）：
/// 基线裸扣一致、护甲 War3/Dota2 减免、魔抗独立、Real 无视、暴击激活、无敌伤害拦截、无敌控制读取压制。
/// 说明：本地无 War3 客户端，暴击 RNG 通过替换 War3Random.Next01Provider 为确定性 fake 规避 native 调用；
/// DamageResolveSystem 绑定本地 root store，事件/扣血全在本地 ECS 验证（同 ControlStateValidationScenario 范式）。
/// 本场景与验证 runner 逻辑一致，供真实 War3 客户端运行（Program.cs 注册）。
/// </summary>
public static class DamagePipelineValidationScenario
{
    private const string ScenarioName = "DamagePipelineValidationScenario";

    /// <summary>本地 root 系统树（复用，避免反复构建）。</summary>
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
        // 保存原 RNG（War3 同步），验证结束后恢复，避免污染真实战斗。
        var originalRng = War3Random.Next01Provider;
        try
        {
            RunValidationCore(originalRng);
        }
        finally
        {
            War3Random.Next01Provider = originalRng;
        }
    }

    private static void RunValidationCore(Func<float> originalRng)
    {
        var store = new EntityStore();
        _root = CreateSystemRoot(store);

        // 无属性攻击源（复用于不涉及暴击的用例）
        var plainSource = CreateUnit(store, maxHp: 100f, armor: 0f, magicResist: 0f, critChance: 0f);

        // ---- 1. REQ-DMG-002 基线：无护甲/暴击/无敌，与历史裸扣一致 ----
        var baseline = CreateUnit(store, maxHp: 100f, armor: 0f, magicResist: 0f, critChance: 0f);
        ResetRng(alwaysCrit: false);
        ApplyDamage(store, plainSource, baseline, 100f, DamageType.Physical, DamageSrc.Melee);
        Require(GetHp(baseline) == 0f, "phase1/final: 无护甲 100 伤应扣至 0（与历史一致）");
        var evt = LastDamageEvent(store, baseline);
        Require(evt.HasValue && Math.Abs(evt.Value.finalDamage - 100f) < 0.001f && !evt.Value.isCrit && !evt.Value.isImmune,
            "phase1/event: finalDamage=100、非暴击、非免疫");

        // ---- 2. REQ-DMG-003/004 护甲减免（War3 公式，armor=10）----
        var armored = CreateUnit(store, maxHp: 1000f, armor: 10f, magicResist: 0f, critChance: 0f);
        ResetRng(alwaysCrit: false);
        ApplyDamage(store, plainSource, armored, 100f, DamageType.Physical, DamageSrc.Melee);
        // War3 factor = 1 - 0.06*10/(1+0.06*10) = 0.625 → final = 62.5
        Require(Math.Abs(GetHp(armored) - (1000f - 62.5f)) < 0.01f, $"phase2/war3: 实际扣 {1000f - GetHp(armored)}");

        // ---- 3. REQ-DMG-004 切 Dota2 公式 ----
        ArmorFormulaRegistry.SetPhysicalFormula("Dota2");
        var armoredDota = CreateUnit(store, maxHp: 1000f, armor: 10f, magicResist: 0f, critChance: 0f);
        ResetRng(alwaysCrit: false);
        ApplyDamage(store, plainSource, armoredDota, 100f, DamageType.Physical, DamageSrc.Melee);
        Require(Math.Abs(GetHp(armoredDota) - (1000f - 65.789f)) < 0.05f,
            $"phase3/dota2: 实际扣 {1000f - GetHp(armoredDota)}");
        ArmorFormulaRegistry.SetPhysicalFormula("War3"); // 恢复

        // ---- 4. REQ-DMG-003 魔抗独立 ----
        var magicTarget = CreateUnit(store, maxHp: 1000f, armor: 10f, magicResist: 10f, critChance: 0f);
        ResetRng(alwaysCrit: false);
        ApplyDamage(store, plainSource, magicTarget, 100f, DamageType.Magical, DamageSrc.Skill);
        Require(Math.Abs(GetHp(magicTarget) - (1000f - 62.5f)) < 0.01f, "phase4/magicResist: Magical 受 MagicResist");

        var mixed = CreateUnit(store, maxHp: 1000f, armor: 10f, magicResist: 0f, critChance: 0f);
        ResetRng(alwaysCrit: false);
        ApplyDamage(store, plainSource, mixed, 100f, DamageType.Physical, DamageSrc.Melee);
        Require(Math.Abs(GetHp(mixed) - (1000f - 62.5f)) < 0.01f, "phase4/physical: Physical 受 Armor");

        // ---- 5. REQ-DMG-003 Real 无视护甲 ----
        var realTarget = CreateUnit(store, maxHp: 1000f, armor: 100f, magicResist: 100f, critChance: 0f);
        ResetRng(alwaysCrit: false);
        ApplyDamage(store, plainSource, realTarget, 100f, DamageType.Real, DamageSrc.Skill);
        Require(Math.Abs(GetHp(realTarget) - 900f) < 0.001f, "phase5/real: Real 满额");

        // ---- 6a. REQ-DMG-005 暴击激活（fake 命中；暴击属性在攻击者） ----
        War3Random.Next01Provider = () => 0f; // 命中
        var critter = CreateUnit(store, maxHp: 1000f, armor: 0f, magicResist: 0f, critChance: 0f);
        var critSource = CreateUnit(store, maxHp: 100f, armor: 0f, magicResist: 0f, critChance: 1f, critMultiplier: 2f);
        ApplyDamage(store, critSource, critter, 100f, DamageType.Physical, DamageSrc.Melee);
        var critEvt = LastDamageEvent(store, critter);
        Require(critEvt.HasValue && critEvt.Value.isCrit && Math.Abs(critEvt.Value.finalDamage - 200f) < 0.001f,
            "phase6/crit: 暴击率 1.0/倍率 2.0 → isCrit=true 且 finalDamage=200");

        // ---- 6b. CritChance=0 永不暴击（即便 fake 命中） ----
        var noCritTarget = CreateUnit(store, maxHp: 1000f, armor: 0f, magicResist: 0f, critChance: 0f);
        var noCritSource = CreateUnit(store, maxHp: 100f, armor: 0f, magicResist: 0f, critChance: 0f);
        ApplyDamage(store, noCritSource, noCritTarget, 50f, DamageType.Physical, DamageSrc.Melee);
        var noCritEvt = LastDamageEvent(store, noCritTarget);
        Require(noCritEvt.HasValue && !noCritEvt.Value.isCrit && Math.Abs(noCritEvt.Value.finalDamage - 50f) < 0.001f,
            "phase6/noCrit: CritChance=0 永不暴击");

        // ---- 7. REQ-DMG-006 无敌伤害拦截 ----
        War3Random.Next01Provider = () => 0.99f;
        var invuln = CreateUnit(store, maxHp: 1000f, armor: 0f, magicResist: 0f, critChance: 0f, invulnerable: true);
        ApplyDamage(store, plainSource, invuln, 500f, DamageType.Physical, DamageSrc.Melee);
        Require(GetHp(invuln) == 1000f, "phase7/immuneHp: 无敌伤害不扣血");
        var immuneEvt = LastDamageEvent(store, invuln);
        Require(immuneEvt.HasValue && immuneEvt.Value.isImmune && Math.Abs(immuneEvt.Value.finalDamage) < 0.001f,
            "phase7/immuneEvent: isImmune=true 且 finalDamage=0");

        // ---- 8. REQ-DMG-007 无敌控制读取压制 ----
        var controlUnit = CreateUnit(store, maxHp: 1000f, armor: 0f, magicResist: 0f, critChance: 0f);
        var stunAttr = CreateAttr(store, controlUnit, AttributeHelper.Stun, 1f);
        Require(ControlHelper.GetEffectiveValue(controlUnit, AttributeHelper.Stun) > 0f, "phase8/noInvuln");
        var invulnAttr = CreateAttr(store, controlUnit, AttributeHelper.Invulnerable, 1f);
        Require(ControlHelper.GetEffectiveValue(controlUnit, AttributeHelper.Stun) == 0f, "phase8/invulnPress");
        invulnAttr.DeleteEntity();
        Require(ControlHelper.GetEffectiveValue(controlUnit, AttributeHelper.Stun) > 0f, "phase8/exit");
        _ = stunAttr;

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    /// <summary>构建本地系统树：属性计算（45）+ 伤害结算（125）。不注册 Native 系统。</summary>
    private static TimedSystemRoot CreateSystemRoot(EntityStore store)
    {
        var root = new TimedSystemRoot(store);
        root.Add(new AttrCalculationSystem(), 0f);
        root.Add(new DamageResolveSystem(), 0f);
        return root;
    }

    /// <summary>创建测试单位（含 Health/Armor/MagicResist/CritChance 属性实体）。</summary>
    private static Entity CreateUnit(EntityStore store, float maxHp, float armor, float magicResist,
        float critChance, float critMultiplier = 1f, bool invulnerable = false)
    {
        var unit = store.CreateEntity();
        CreateAttr(store, unit, AttributeHelper.Health, maxHp);
        CreateAttr(store, unit, AttributeHelper.Armor, armor);
        CreateAttr(store, unit, AttributeHelper.MagicResist, magicResist);
        CreateAttr(store, unit, AttributeHelper.CritChance, critChance);
        if (critMultiplier > 1f)
            CreateAttr(store, unit, AttributeHelper.CritMultiplier, critMultiplier);
        if (invulnerable)
            CreateAttr(store, unit, AttributeHelper.Invulnerable, 1f);
        return unit;
    }

    /// <summary>创建 DamageRequest 实体并驱动一帧结算。</summary>
    private static void ApplyDamage(EntityStore store, Entity source, Entity target, float amount, DamageType type,
        DamageSrc src)
    {
        store.CreateEntity(new DamageRequest
        {
            source = source,
            target = target,
            damage = new DamageBase
            {
                damage = amount,
                damageType = type,
                damageSrc = src,
                source = source,
                target = target,
            }
        });
        _root!.Update(new UpdateTick(0f, 0f));
    }

    /// <summary>设置暴击 RNG：alwaysCrit=true 总是命中（0），false 不命中（0.99）。</summary>
    private static void ResetRng(bool alwaysCrit)
    {
        War3Random.Next01Provider = alwaysCrit
            ? (Func<float>)(() => 0f)
            : (Func<float>)(() => 0.99f);
    }

    private static float GetHp(Entity unit) => AttributeHelper.GetCurrent(unit, AttributeHelper.Health);

    /// <summary>取最后一次针对该目标的 DamageEvent。</summary>
    private static DamageEvent? LastDamageEvent(EntityStore store, Entity target)
    {
        DamageEvent? result = null;
        store.Query<DamageEvent>().ForEachEntity((ref DamageEvent evt, Entity _) =>
        {
            if (evt.target == target)
                result = evt;
        });
        return result;
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
