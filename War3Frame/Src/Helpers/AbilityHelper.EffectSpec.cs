using Friflo.Engine.ECS;

namespace War3Frame.Helpers;

public static partial class AbilityHelper
{
    public static void SetEffectSpec(Entity ability, EffectSpec spec)
    {
        ability.AddComponent(new EffectSpecData { spec = spec });
    }

    public static bool TryGetEffectSpec(Entity ability, out EffectSpec spec)
    {
        if (ability.TryGetComponent<EffectSpecData>(out var data) && data.spec != null)
        {
            spec = data.spec;
            return true;
        }

        spec = null!;
        return false;
    }
}
