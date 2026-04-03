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

public struct UnitBase : IComponent
{
    public string templateName;
    public string name;
}

public struct UnitNative : IComponent
{
    public JUnit unit;
    public JPlayer player;
}

/// <summary>
/// 原生同步快照，仅用于连续字段 compare-sync。
/// </summary>
public struct UnitNativeSyncSnapshot : IComponent
{
    public bool initialized;
    public float lastHealthCurrent;
    public float lastHealthFinal;
    public float lastManaCurrent;
    public float lastManaFinal;
}

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