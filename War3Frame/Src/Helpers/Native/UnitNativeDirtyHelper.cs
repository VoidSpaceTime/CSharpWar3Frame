using Friflo.Engine.ECS;

namespace War3Frame;


/// <summary>
/// 原生单位脏位辅助方法
/// </summary>
public static class UnitNativeDirtyHelper
{
    public static void Mark(Entity entity, UnitNativeDirtyFlags flags)
    {
        if (entity.TryGetComponent<UnitNativeDirty>(out var dirty))
        {
            dirty.flags |= flags;
            entity.AddComponent(dirty);
            return;
        }

        entity.AddComponent(new UnitNativeDirty { flags = flags });
    }

    public static void MarkImmediate(Entity entity, UnitNativeDirtyFlags flags)
    {
        Mark(entity, flags);
        Game.FlushImmediateSystems();
    }

    public static bool Has(Entity entity, UnitNativeDirtyFlags flags)
    {
        return entity.TryGetComponent<UnitNativeDirty>(out var dirty) && dirty.flags.HasFlag(flags);
    }

    public static void Clear(Entity entity, UnitNativeDirtyFlags flags)
    {
        if (!entity.TryGetComponent<UnitNativeDirty>(out var dirty))
        {
            return;
        }

        dirty.flags &= ~flags;
        if (dirty.flags == UnitNativeDirtyFlags.None)
        {
            entity.RemoveComponent<UnitNativeDirty>();
            return;
        }

        entity.AddComponent(dirty);
    }
}