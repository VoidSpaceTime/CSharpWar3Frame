using Friflo.Engine.ECS;

// 包装 Entity，限制可用方法
public readonly struct UnitEntityRef
{
    public Entity Entity { get; }
    public UnitEntityRef(Entity e) => Entity = e;

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