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

            // 同步单位血量
            if (AttributeHelper.TryGetAttr(entity, AttributeHelper.Health, out var health) &&
                health.TryGetComponent<AttrValue>(out var hpVal))
            {
                var healthChanged = !hasSnapshot
                    || !snapshot.initialized
                    || HasMeaningfulDifference(snapshot.lastHealthCurrent, hpVal.current)
                    || HasMeaningfulDifference(snapshot.lastHealthFinal, hpVal.finalValue);

                if (healthChanged)
                {
                    var set = ToNativeStateValue(hpVal.current, hpVal.finalValue);
                    JassApi.SetUnitState(native.unit, new JUnitState(Blizzard.UNIT_STATE_LIFE), set);

                    snapshot.lastHealthCurrent = hpVal.current;
                    snapshot.lastHealthFinal = hpVal.finalValue;
                    snapshot.initialized = true;
                    hasSnapshot = true;
                }
            }

            // 同步单位魔法
            if (AttributeHelper.TryGetAttr(entity, AttributeHelper.Mana, out var mana) &&
                mana.TryGetComponent<AttrValue>(out var manaVal))
            {
                var manaChanged = !hasSnapshot
                    || !snapshot.initialized
                    || HasMeaningfulDifference(snapshot.lastManaCurrent, manaVal.current)
                    || HasMeaningfulDifference(snapshot.lastManaFinal, manaVal.finalValue);

                if (manaChanged)
                {
                    var set = ToNativeStateValue(manaVal.current, manaVal.finalValue);
                    JassApi.SetUnitState(native.unit, new JUnitState(Blizzard.UNIT_STATE_MANA), set);

                    snapshot.lastManaCurrent = manaVal.current;
                    snapshot.lastManaFinal = manaVal.finalValue;
                    snapshot.initialized = true;
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

    /// <summary>
    /// 将当前值/最终值换算为 Warcraft 原生单位状态值。
    /// </summary>
    private static float ToNativeStateValue(float current, float finalValue)
    {
        if (finalValue <= 0f)
        {
            return 0f;
        }

        return (current / finalValue) * 10000f;
    }
}
