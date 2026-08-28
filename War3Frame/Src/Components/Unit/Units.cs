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

/// <summary>
/// War3 原生单位句柄缓存。
/// 仅供 Native/Execution 层执行副作用使用，长期语义仍以 ECS 组件为准。
/// </summary>
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
    public UnitNativeSyncEntry entry0;
    public UnitNativeSyncEntry entry1;
}

/// <summary>
/// 单个原生同步条目快照。
/// </summary>
public struct UnitNativeSyncEntry
{
    public int attrTypeId;
    public bool initialized;
    public float lastCurrent;
    public float lastFinal;
}

/// <summary>
/// 原生单位创建请求
/// </summary>
public struct UnitCreateNativeRequest : IComponent
{
    public JPlayer player;
    public float x;
    public float y;
    public float facing;
    public int unitTypeId;
}
