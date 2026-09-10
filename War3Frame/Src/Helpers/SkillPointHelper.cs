using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
///     技能点薄入口：发点/读池/创建加点请求。不长期持有技能点真相，只做一次性便利与 Request 写入。
/// </summary>
public static class SkillPointHelper
{
    /// <summary>读取单位技能点池；未挂池返回 false。</summary>
    public static bool TryGetPool(Entity unit, out SkillPointPool pool)
    {
        pool = default;
        return !unit.IsNull && unit.TryGetComponent<SkillPointPool>(out pool);
    }

    /// <summary>读取单位未分配技能点，未挂池返回 0。</summary>
    public static int GetUnspent(Entity unit)
    {
        return TryGetPool(unit, out var pool) ? pool.unspent : 0;
    }

    /// <summary>
    ///     按升级级数发放技能点（仅对挂了点池的单位生效）。
    ///     升级级数来自一次经验结算的 (toLevel - fromLevel)。
    /// </summary>
    public static void GrantLevels(Entity unit, int levelsGained)
    {
        if (unit.IsNull || levelsGained <= 0 || !TryGetPool(unit, out var pool))
            return;

        var delta = levelsGained * pool.perLevel;
        pool.unspent += delta;
        pool.earned += delta;
        unit.AddComponent(pool);
    }

    /// <summary>从单位点池扣除点数，不足或未挂池返回 false 且不改动。</summary>
    public static bool SpendPoints(Entity unit, int count)
    {
        if (unit.IsNull || count <= 0 || !TryGetPool(unit, out var pool) || pool.unspent < count)
            return false;

        pool.unspent -= count;
        unit.AddComponent(pool);
        return true;
    }

    /// <summary>
    ///     创建技能加点请求（薄入口，只写 Request；校验由工作流系统完成）。
    /// </summary>
    public static Entity Upgrade(Entity unit, Entity ability, int levels = 1)
    {
        if (unit.IsNull)
            throw new ArgumentException("unit 不能为空", nameof(unit));
        if (ability.IsNull)
            throw new ArgumentException("ability 不能为空", nameof(ability));

        return unit.Store.CreateEntity(new AbilityUpgradeRequest
        {
            unit = unit,
            ability = ability,
            levels = Math.Max(1, levels)
        });
    }
}
