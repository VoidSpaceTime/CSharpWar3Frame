using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

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

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitNative native, Entity entity) =>
        {
            var hasSnapshot = entity.TryGetComponent<UnitNativeSyncSnapshot>(out var snapshot);

            foreach (var spec in UnitNativeSyncRegistry.Specs)
            {
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
            if (entity.TryGetComponent<Position>(out var position))
            {
                position.x = JassApi.GetUnitX(native.unit);
                position.y = JassApi.GetUnitY(native.unit);
            }

            if (hasSnapshot)
            {
                entity.AddComponent(snapshot);
            }
        });
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
