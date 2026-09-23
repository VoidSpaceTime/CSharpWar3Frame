using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 单位原生同步系统。
/// 将 ECS 属性真相投影到 War3 native，并从 native 读取位置回写到 ECS。
/// </summary>
[SystemRegister(SystemKind.Interval)]
public class UnitNativeSystem : QuerySystem<UnitNative>, ITimedSystem
{
    /// <summary>
    /// 用于判断数值是否发生有效变化的容差。
    /// </summary>
    private const float CompareTolerance = 0.0001f;

    /// <summary>
    /// 原生同步执行间隔。
    /// </summary>
    public float Interval => 0.03125f;

    /// <summary>
    /// 单位原生同步声明：声明某个属性如何投影到原生层。
    /// 仅供本系统使用，不对外暴露投影规则。
    /// </summary>
    private readonly record struct UnitNativeSyncSpec(
        int AttrTypeId,
        Action<UnitNative, float, float> Apply);

    // 需要投影到 War3 原生单位状态的属性集中登记，避免业务系统各自调用 JassApi。
    private static readonly UnitNativeSyncSpec[] SyncSpecs =
    [
        new(AttributeHelper.Health, ApplyHealth),
        new(AttributeHelper.Mana, ApplyMana)
    ];

    /// <summary>
    /// 把生命投影为原生单位状态值。
    /// </summary>
    private static void ApplyHealth(UnitNative native, float current, float final)
    {
        var value = ToNativeStateValue(current, final);
        JassApi.SetUnitState(native.unit, Blizzard.UNIT_STATE_LIFE, value);
    }

    /// <summary>
    /// 把法力投影为原生单位状态值。
    /// </summary>
    private static void ApplyMana(UnitNative native, float current, float final)
    {
        var value = ToNativeStateValue(current, final);
        JassApi.SetUnitState(native.unit, Blizzard.UNIT_STATE_MANA, value);
    }

    /// <summary>
    /// 把 ECS 的 current/final 归一化映射为原生单位状态值（0..10000）。
    /// </summary>
    private static float ToNativeStateValue(float current, float final)
    {
        if (final <= 0f)
        {
            // 避免除零；final 非法时同步为 0，让 ECS 侧后续计算再修正。
            return 0f;
        }

        // 这里沿用现有原生同步比例：ECS current/final 归一化后映射到 0..10000。
        return (current / final) * 10000f;
    }

    protected override void OnUpdate()
    {
        // Position / 快照写回是 AddComponent（结构变更），不能在 Query 迭代内执行：先收集，循环外写回。
        var posUpdates = new List<(Entity entity, Position pos)>();
        var snapUpdates = new List<(Entity entity, UnitNativeSyncSnapshot snapshot)>();

        Query.ForEachEntity((ref UnitNative native, Entity entity) =>
        {
            var hasSnapshot = entity.TryGetComponent<UnitNativeSyncSnapshot>(out var snapshot);

            foreach (var spec in SyncSpecs)
            {
                // 只同步登记表声明的属性，避免业务系统直接散落 native setter。
                if (!AttributeHelper.TryGetAttr(entity, spec.AttrTypeId, out var attr)
                    || !attr.TryGetComponent<AttrValue>(out var attrVal))
                {
                    continue;
                }

                ref var entry = ref GetEntry(ref snapshot, spec.AttrTypeId);
                var changed = !hasSnapshot
                    || !entry.initialized
                    || HasMeaningfulDifference(entry.lastCurrent, attrVal.current)
                    || HasMeaningfulDifference(entry.lastFinal, attrVal.finalValue);

                if (changed)
                {
                    spec.Apply(native, attrVal.current, attrVal.finalValue);

                    entry.attrTypeId = spec.AttrTypeId;
                    entry.lastCurrent = attrVal.current;
                    entry.lastFinal = attrVal.finalValue;
                    entry.initialized = true;
                    hasSnapshot = true;
                }
            }

            // 同步单位位置
            // 位置以 native 世界为准回写到 ECS，供距离、弹道、区域搜索等系统读取。
            // Position 是 struct，TryGetComponent 返回副本，必须显式写回（收集到循环外）。
            if (entity.TryGetComponent<Position>(out var position))
            {
                position.x = JassApi.GetUnitX(native.unit);
                position.y = JassApi.GetUnitY(native.unit);
                posUpdates.Add((entity, position));
            }

            if (hasSnapshot)
            {
                snapUpdates.Add((entity, snapshot));
            }
        });

        foreach (var (entity, position) in posUpdates)
        {
            if (!entity.IsNull)
                entity.AddComponent(position);
        }

        foreach (var (entity, snapshot) in snapUpdates)
        {
            if (!entity.IsNull)
                entity.AddComponent(snapshot);
        }
    }

    private static bool HasMeaningfulDifference(float left, float right)
    {
        return MathF.Abs(left - right) > CompareTolerance;
    }

    private static ref UnitNativeSyncEntry GetEntry(ref UnitNativeSyncSnapshot snapshot, int attrTypeId)
    {
        if (snapshot.entry0.attrTypeId == attrTypeId || !snapshot.entry0.initialized)
        {
            return ref snapshot.entry0;
        }

        if (snapshot.entry1.attrTypeId == attrTypeId || !snapshot.entry1.initialized)
        {
            return ref snapshot.entry1;
        }

        return ref snapshot.entry0;
    }
}
