using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
///     特效辅助类 - 提供特效的创建操作
/// </summary>
public static class EffectHelper
{
    /// <summary>
    ///     创建特效
    /// </summary>
    public static Entity CreateEffect(EntityStore store,
        string modelAlias,
        float x, float y, float z = 0)
    {
        var entity = store.CreateEntity(
            new UnitState { isAlive = true },
            new Position { x = x, y = y }
        );
        return entity;
    }

    /// <summary>
    ///     创建附着特效
    /// </summary>
    public static Entity CreateEffectAttach(EntityStore store,
        string modelAlias,
        Entity attachTo,
        string attachPoint = "origin")
    {
        var entity = store.CreateEntity(
            new UnitState { isAlive = true }
        );
        // TODO: 添加附着关系
        return entity;
    }
}
