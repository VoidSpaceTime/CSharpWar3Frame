using Friflo.Engine.ECS;

namespace War3Frame;

public struct AbilityStatTypeId : IComponent
{
    public int typeId;
}

public struct AbilityStatValue : IComponent
{
    public float baseValue;
    public float currentValue;
    public float finalValue;
}

public struct AbilityStatOwner : ILinkComponent
{
    public Entity ability;

    public AbilityStatOwner(Entity ability)
    {
        this.ability = ability;
    }

    public Entity GetIndexedValue() => ability;
}

public struct HasAbilityStat : IRelation<Entity>
{
    public Entity statEntity;
    public int typeId;

    public HasAbilityStat(Entity statEntity, int typeId)
    {
        this.statEntity = statEntity;
        this.typeId = typeId;
    }

    public Entity GetRelationKey() => statEntity;
}

public struct AbilityStatDirty : ITag;
