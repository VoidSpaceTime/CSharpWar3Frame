using Friflo.Engine.ECS;
using War3Frame;

// 包装 Entity，限制可用方法
public readonly struct UnitEntityRef
{
    public Entity Entity { get; }
    public UnitEntityRef(Entity e) => Entity = e;

    public UnitEntityRef(nint h)
    {
        var entities = Game.Store.Query<UnitNative>();
        Entity e = new Entity();
        entities.ForEachEntity(((ref UnitNative native, Entity entity) =>
        {
            if (native.unit.Handle == h)
            {
                e = entity;
            }
        }));
        Entity = e;
    }

    // 只暴露单位相关的扩展
    public UnitEntityRef WithHealth(float max, float regen)
    {
        return this;
    }

    public UnitEntityRef WithAttack(float damage)
    {
        return this;
    }
}