using Friflo.Engine.ECS;
using War3Frame.EntityRef;

namespace War3Frame;

public static class AbilityEntityExtensions
{
    public static AbilityEntityRef AsAbility(this Entity entity)
    {
        return AbilityEntityRef.From(entity);
    }

    public static bool TryAsAbility(this Entity entity, out AbilityEntityRef ability)
    {
        return AbilityEntityRef.TryFrom(entity, out ability);
    }
}
