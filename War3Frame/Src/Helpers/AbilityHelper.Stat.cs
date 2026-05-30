using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 技能属性辅助逻辑
/// 作为 AbilityHelper 的分文件实现，对外统一走 AbilityHelper
/// </summary>
public static partial class AbilityHelper
{
    /// <summary>
    /// 已注册的技能数值类型名称表。
    /// </summary>
    private static readonly SortedDictionary<int, string> _types = new();

    /// <summary>
    /// 下一个技能数值类型 ID。
    /// </summary>
    private static int _nextId = 0;

    /// <summary>技能生命消耗</summary>
    public static readonly int HealthCost = Register("HealthCost");
    /// <summary>技能魔法消耗</summary>
    public static readonly int ManaCost = Register("ManaCost");
    /// <summary>技能施法时间</summary>
    public static readonly int CastTime = Register("CastTime");
    /// <summary>技能冷却时长</summary>
    public static readonly int CooldownDuration = Register("CooldownDuration");
    /// <summary>技能施法距离</summary>
    public static readonly int Range = Register("Range");
    /// <summary>技能范围半径</summary>
    public static readonly int Radius = Register("Radius");
    /// <summary>技能宽度</summary>
    public static readonly int Width = Register("Width");
    /// <summary>技能高度</summary>
    public static readonly int Height = Register("Height");
    /// <summary>技能引导时长</summary>
    public static readonly int ChannelDuration = Register("ChannelDuration");
    /// <summary>技能引导 tick 间隔</summary>
    public static readonly int ChannelTickInterval = Register("ChannelTickInterval");
    /// <summary>技能释放后摇时长</summary>
    public static readonly int BackswingDuration = Register("BackswingDuration");
    /// <summary>技能伤害数值</summary>
    public static readonly int DamageAmount = Register("DamageAmount");
    /// <summary>技能治疗数值</summary>
    public static readonly int HealAmount = Register("HealAmount");
    /// <summary>弹道速度</summary>
    public static readonly int ProjectileSpeed = Register("ProjectileSpeed");
    /// <summary>弹道距离</summary>
    public static readonly int ProjectileDistance = Register("ProjectileDistance");
    /// <summary>命中宽度</summary>
    public static readonly int HitWidth = Register("HitWidth");
    /// <summary>到达阈值</summary>
    public static readonly int ArrivalThreshold = Register("ArrivalThreshold");
    /// <summary>最大目标数</summary>
    public static readonly int MaxTargets = Register("MaxTargets");

    /// <summary>注册技能属性类型</summary>
    public static int Register(string name)
    {
        var id = _nextId++;
        _types[id] = name;
        return id;
    }

    /// <summary>获取技能属性名称</summary>
    public static string? GetName(int statId)
    {
        return _types.TryGetValue(statId, out var name) ? name : null;
    }

    /// <summary>为技能创建一个属性实体</summary>
    public static Entity CreateStat(Entity ability, int typeId, float baseValue, float? currentValue = null)
    {
        var current = currentValue ?? baseValue;
        var stat = ability.Store.CreateEntity(
            new AbilityStatTypeId { typeId = typeId },
            new AbilityStatValue
            {
                baseValue = baseValue,
                currentValue = current,
                finalValue = baseValue
            },
            new AbilityStatOwner(ability));

        ability.AddRelation(new HasAbilityStat(stat, typeId));
        return stat;
    }

    /// <summary>确保技能存在指定属性</summary>
    public static Entity EnsureStat(Entity ability, int typeId, float baseValue, float? currentValue = null)
    {
        return TryGetStat(ability, typeId, out var stat)
            ? stat
            : CreateStat(ability, typeId, baseValue, currentValue);
    }

    /// <summary>获取技能属性实体</summary>
    public static Entity? GetStat(Entity ability, int typeId)
    {
        var relations = ability.GetRelations<HasAbilityStat>();
        foreach (ref var relation in relations)
        {
            if (relation.typeId == typeId)
                return relation.statEntity;
        }

        return null;
    }

    /// <summary>尝试获取技能属性实体</summary>
    public static bool TryGetStat(Entity ability, int typeId, out Entity stat)
    {
        var result = GetStat(ability, typeId);
        if (result != null)
        {
            stat = result.Value;
            return true;
        }

        stat = default;
        return false;
    }

    /// <summary>获取技能的全部属性实体</summary>
    public static IEnumerable<(int typeId, Entity statEntity)> GetAllStats(Entity ability)
    {
        var relations = ability.GetRelations<HasAbilityStat>();
        foreach (ref var relation in relations)
        {
            yield return (relation.typeId, relation.statEntity);
        }
    }

    /// <summary>获取技能属性值组件</summary>
    public static AbilityStatValue GetValue(Entity ability, int typeId)
    {
        return TryGetStat(ability, typeId, out var stat) && stat.TryGetComponent<AbilityStatValue>(out var value)
            ? value
            : default;
    }

    /// <summary>
    /// 获取技能属性基础值。
    /// </summary>
    public static float GetBaseValue(Entity ability, int typeId) => GetValue(ability, typeId).baseValue;

    /// <summary>
    /// 获取技能属性当前值。
    /// </summary>
    public static float GetCurrentValue(Entity ability, int typeId) => GetValue(ability, typeId).currentValue;

    /// <summary>
    /// 获取技能属性最终值。
    /// </summary>
    public static float GetFinalValue(Entity ability, int typeId) => GetValue(ability, typeId).finalValue;

    /// <summary>整体设置技能属性值</summary>
    public static void SetValue(Entity ability, int typeId, float baseValue, float currentValue, float finalValue)
    {
        var stat = EnsureStat(ability, typeId, baseValue, currentValue);
        stat.AddComponent(new AbilityStatValue
        {
            baseValue = baseValue,
            currentValue = currentValue,
            finalValue = finalValue
        });
    }

    /// <summary>设置技能属性基础值</summary>
    public static void SetBaseValue(Entity ability, int typeId, float value)
    {
        var stat = EnsureStat(ability, typeId, value, value);
        var statValue = stat.GetComponent<AbilityStatValue>();
        statValue.baseValue = value;
        stat.AddComponent(statValue);
        stat.AddTag<AbilityStatDirty>();
    }

    /// <summary>设置技能属性当前值</summary>
    public static void SetCurrentValue(Entity ability, int typeId, float value)
    {
        var stat = EnsureStat(ability, typeId, value, value);
        var statValue = stat.GetComponent<AbilityStatValue>();
        statValue.currentValue = value;
        stat.AddComponent(statValue);
    }

    /// <summary>设置技能属性最终值</summary>
    public static void SetFinalValue(Entity ability, int typeId, float value)
    {
        var stat = EnsureStat(ability, typeId, value, value);
        var statValue = stat.GetComponent<AbilityStatValue>();
        statValue.finalValue = value;
        stat.AddComponent(statValue);
    }

    /// <summary>给技能属性添加修改器</summary>
    public static Entity AddModifier(Entity ability, int typeId, Entity source, ModifyType modifyType, float value, int priority = 0)
    {
        if (!TryGetStat(ability, typeId, out var stat))
            throw new InvalidOperationException($"技能 {ability.Id} 不存在属性 {GetName(typeId) ?? typeId.ToString()}");

        var mod = ability.Store.CreateEntity(
            new ModifyValue { modifyType = modifyType, value = value, priority = priority },
            new ModifyTarget(stat),
            new ModifySource(source));

        stat.AddTag<AbilityStatDirty>();
        return mod;
    }

    /// <summary>移除技能的全部属性实体与其修改器</summary>
    public static void RemoveAllStats(Entity ability)
    {
        var relations = ability.GetRelations<HasAbilityStat>();
        var toDelete = new List<Entity>();

        foreach (ref var relation in relations)
        {
            toDelete.Add(relation.statEntity);
        }

        foreach (var stat in toDelete)
        {
            var modifiers = stat.GetIncomingLinks<ModifyTarget>();
            var modsToDelete = new List<Entity>();
            foreach (var link in modifiers)
            {
                modsToDelete.Add(link.Entity);
            }

            foreach (var mod in modsToDelete)
            {
                mod.DeleteEntity();
            }

            ability.RemoveRelation<HasAbilityStat, Entity>(stat);
            stat.DeleteEntity();
        }
    }
}
