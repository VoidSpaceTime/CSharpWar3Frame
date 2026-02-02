using Friflo.Engine.ECS;

// 包装 Entity，限制可用方法
public readonly struct UnitEntity
{
    public Entity Entity { get; }
    public UnitEntity(Entity e) => Entity = e;

    // 只暴露单位相关的扩展
    public UnitEntity WithHealth(float max, float regen)
    {
        return this;
    }

    public UnitEntity WithAttack(float damage)
    {
        return this;
    }
}