using Friflo.Engine.ECS;

namespace War3Frame.Components.Attribute;

public struct HealthAttr : IComponent
{
    public float current;
    // 关联的属性 Entity（通过 HasAttr 查找更灵活，此处可选）

    /*     public Entity maxAttrEntity; // 指向 MaxHealth 属性 Entity
        public Entity regenAttrEntity; // 指向 HealthRegen 属性 Entity

        // Helper 方法
        public float GetMax(EntityStore store)
            => maxAttrEntity.GetComponent<AttrValue>().finalValue;

        public float GetRegen(EntityStore store)
            => regenAttrEntity.GetComponent<AttrValue>().finalValue; */
}

public struct HealthNativeDirty : ITag;