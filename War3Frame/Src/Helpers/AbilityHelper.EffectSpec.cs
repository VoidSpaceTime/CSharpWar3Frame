using Friflo.Engine.ECS;

namespace War3Frame.Helpers;

public static partial class AbilityHelper
{
    // 将可配置效果链挂到技能实体上；执行时由 AbilityEffectSystems 解释 spec。
    public static void SetEffectSpec(Entity ability, EffectSpec spec)
    {
        ability.AddComponent(new EffectSpecData { spec = spec });
    }

    // 获取配置化效果链；没有 spec 时返回 false，调用方可回退到旧 AbilityEffect 流程。
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
