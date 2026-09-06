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
    // 需要投影到 War3 原生单位状态的属性集中登记，避免业务系统各自调用 JassApi。
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
            // 避免除零；final 非法时同步为 0，让 ECS 侧后续计算再修正。
            return 0f;
        }

        // 这里沿用现有原生同步比例：ECS current/final 归一化后映射到 0..10000。
        return (current / final) * 10000f;
    }
}
