using Friflo.Engine.ECS;

namespace War3Frame.Src.Systems;

/// <summary>
/// 单位原生同步声明。
/// 负责声明某个属性如何投影到原生层。
/// </summary>
public readonly record struct UnitNativeSyncSpec(
    int AttrTypeId,
    Action<UnitNative, float, float> Apply);

/// <summary>
/// 单位原生同步注册表。
/// </summary>
public static class UnitNativeSyncRegistry
{
    public static readonly UnitNativeSyncSpec[] Specs =
    [
        new(AttributeHelper.Health, ApplyHealth),
        new(AttributeHelper.Mana, ApplyMana)
    ];

    private static void ApplyHealth(UnitNative native, float current, float final)
    {
        var value = ToNativeStateValue(current, final);
        JassApi.SetUnitState(native.unit, Blizzard.UNIT_STATE_LIFE, value);
    }

    private static void ApplyMana(UnitNative native, float current, float final)
    {
        var value = ToNativeStateValue(current, final);
        JassApi.SetUnitState(native.unit, Blizzard.UNIT_STATE_MANA, value);
    }

    private static float ToNativeStateValue(float current, float final)
    {
        if (final <= 0f)
        {
            return 0f;
        }

        return (current / final) * 10000f;
    }
}
