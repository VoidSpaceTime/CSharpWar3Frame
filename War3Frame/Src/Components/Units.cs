using Friflo.Engine.ECS;

namespace War3Frame;

public struct UnitOwner : ILinkComponent
{
    public Entity GetIndexedValue()
    {
        return player;
    }

    public UnitOwner(Entity player)
    {
        this.player = player;
    }

    public Entity player;
}

public struct UnitNative : IComponent
{
    public JUnit unit;
    public JPlayer player;
}

//原生单位删除标签
public struct UnitRemoveTag : ITag;

/// <summary>
/// 原生单位创建请求
/// </summary>
public struct NativeUnitCreateRequest : IComponent
{
    public JPlayer player;
    public float x;
    public float y;
    public float facing;
    public int unitTypeId;
}

public struct UnitNativeDirty : IComponent
{
    public UnitNativeDirtyFlags flags;
}

[Flags]
public enum UnitNativeDirtyFlags
{
    None = 0,
    Health = 1 << 0,
    Mana = 1 << 1,
    Poison = 1 << 2,
    Move = 1 << 3,
    // 死亡动作请求：执行原生死亡，不等于最终删除
    Death = 1 << 4,
    // 终态移除请求：执行原生移除与最终删除
    Remove = 1 << 5,
    Reborn = 1 << 6,
}
