using Friflo.Engine.ECS;

namespace War3Frame.Helpers;

public static partial class AbilityHelper
{
    #region 创建技能

    public static Entity CreateAbility(
        string templateName,
        int level = 1,
        Action<Entity>? configure = null,
        EntityStore? store = null)
    {
        store ??= Game.Store;

        var entity = store.CreateEntity(new AbilityBase
            {
                templateName = templateName,
                Name = templateName,
                Description = string.Empty,
                state = AbilityState.Ready
            },
            new AbilityRuntime()
        );

        configure?.Invoke(entity);
        return entity;
    }

    public static void RemoveAbility(Entity ability)
    {
        if (ability.IsNull) return;
        RemoveAllStats(ability);
        ability.DeleteEntity();
    }

    public static Entity GrantAbilityToSlot(
        Entity unit,
        string templateName,
        int slotIndex,
        int level = 1,
        Action<Entity>? configure = null)
    {
        return AbilitySlotHelper.AddAbilityToSlot(unit, templateName, slotIndex, level, configure);
    }

    public static Entity GrantAbility(
        Entity unit,
        string templateName,
        int level = 1,
        Action<Entity>? configure = null)
    {
        return AbilitySlotHelper.AddAbility(unit, templateName, level, configure);
    }

    public static Entity AddAbilityToSlot(
        Entity unit,
        string templateName,
        int slotIndex,
        int level = 1,
        Action<Entity>? configure = null)
    {
        return GrantAbilityToSlot(unit, templateName, slotIndex, level, configure);
    }

    public static Entity AddAbility(
        Entity unit,
        string templateName,
        int level = 1,
        Action<Entity>? configure = null)
    {
        return GrantAbility(unit, templateName, level, configure);
    }

    #endregion
}
