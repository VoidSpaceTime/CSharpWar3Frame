using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>预设条件与自定义筛选的统一入口。阵营/类型组内为 OR，各组及状态条件之间为 AND。</summary>
public static class TargetFilterRegistry
{
    public delegate bool FilterFunc(Entity caster, Entity target);
    private static readonly SortedDictionary<string, FilterFunc> _filters = new(StringComparer.Ordinal);
    private const TargetFilter Teams = TargetFilter.Self | TargetFilter.Ally | TargetFilter.Enemy | TargetFilter.Neutral;
    private const TargetFilter Types = TargetFilter.Hero | TargetFilter.Normal | TargetFilter.Building | TargetFilter.Summon | TargetFilter.Ward;
    private const TargetFilter Life = TargetFilter.Alive | TargetFilter.Dead;
    private const TargetFilter Traits = TargetFilter.Invisible | TargetFilter.MagicImmune;

    public static void Register(string filterId, FilterFunc filter) => _filters[filterId] = filter;
    public static void Unregister(string filterId) => _filters.Remove(filterId);
    public static FilterFunc? Get(string filterId) => _filters.GetValueOrDefault(filterId);

    /// <summary>未指定或未注册的自定义筛选保持通过；预设条件仍须满足。</summary>
    public static bool PassCustomFilter(string? filterId, Entity caster, Entity target)
        => string.IsNullOrEmpty(filterId) || Get(filterId) is not { } filter || filter(caster, target);

    public static bool PassPresetFilter(TargetFilter filter, Entity caster, Entity target)
    {
        if (target.IsNull) return false;
        if ((filter & Teams) != 0)
        {
            TargetFilter team;
            if (!caster.IsNull && caster == target)
                team = TargetFilter.Self;
            else if (UnitRelationHelper.TryGetRelation(caster, target, out var relation))
                team = relation switch
                {
                    PlayerTeamState.Allie => TargetFilter.Ally,
                    PlayerTeamState.Enemy => TargetFilter.Enemy,
                    _ => TargetFilter.Neutral
                };
            else
                return false;
            if ((filter & team) == 0) return false;
        }

        // TODO Native：自动识别原生类型尚未实现；只消费作者声明的 ECS traits，不猜测原生状态。
        target.TryGetComponent<UnitTargetTraits>(out var traits);
        if ((filter & Types) != 0 && (filter & Types & traits.flags) == 0) return false;
        if ((filter & Traits & traits.flags) != (filter & Traits)) return false;
        if ((filter & Life) != 0)
        {
            if (!target.TryGetComponent<UnitLifeState>(out var life)) return false;
            var state = life.isAlive ? TargetFilter.Alive : TargetFilter.Dead;
            if ((filter & state) == 0) return false;
        }
        if ((filter & TargetFilter.Invulnerable) != 0
            && AttributeHelper.GetFinalValue(target, AttributeHelper.Invulnerable) <= 0) return false;
        return true;
    }

    public static bool PassFilter(TargetFilter presetFilter, string? customFilterId, Entity caster, Entity target)
        => PassPresetFilter(presetFilter, caster, target) && PassCustomFilter(customFilterId, caster, target);
}
