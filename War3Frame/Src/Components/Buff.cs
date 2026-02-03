using Friflo.Engine.ECS;

namespace War3Frame;

#region Buff 组件

/// <summary>
///     Buff 标记 - 表示这是一个 Buff 实体
/// </summary>
public struct Buff : ITag;

/// <summary>
///     Buff 持续时间
/// </summary>
public struct BuffDuration : IComponent
{
    /// <summary>总持续时间</summary>
    public float duration;

    /// <summary>剩余时间</summary>
    public float remaining;

    /// <summary>是否永久（remaining 不减少）</summary>
    public bool isPermanent;

    public static BuffDuration Create(float duration, bool permanent = false)
    {
        return new BuffDuration
        {
            duration = duration,
            remaining = duration,
            isPermanent = permanent
        };
    }

    /// <summary>刷新持续时间</summary>
    public void Refresh()
    {
        remaining = duration;
    }
}

/// <summary>
///     Buff 层数
/// </summary>
public struct BuffStacks : IComponent
{
    /// <summary>当前层数</summary>
    public int current;

    /// <summary>最大层数</summary>
    public int max;

    /// <summary>每层的属性值（用于计算）</summary>
    public float valuePerStack;

    public static BuffStacks Create(int maxStacks, float valuePerStack)
    {
        return new BuffStacks
        {
            current = 1,
            max = maxStacks,
            valuePerStack = valuePerStack
        };
    }

    /// <summary>添加层数，返回是否成功</summary>
    public bool AddStack()
    {
        if (current >= max) return false;
        current++;
        return true;
    }

    /// <summary>移除层数，返回剩余层数</summary>
    public int RemoveStack()
    {
        current--;
        return current;
    }

    /// <summary>获取总加成值</summary>
    public float TotalValue => current * valuePerStack;
}

/// <summary>
///     Buff 刷新行为
/// </summary>
public enum BuffRefreshBehavior
{
    /// <summary>刷新持续时间</summary>
    RefreshDuration,

    /// <summary>叠加层数</summary>
    AddStack,

    /// <summary>刷新 + 叠加</summary>
    RefreshAndStack,

    /// <summary>不做任何事（独立存在）</summary>
    Independent
}

/// <summary>
///     Buff 行为配置
/// </summary>
public struct BuffBehavior : IComponent
{
    /// <summary>Buff 模板 ID（用于判断同类 Buff）</summary>
    public string buffId;

    /// <summary>刷新行为</summary>
    public BuffRefreshBehavior refreshBehavior;

    /// <summary>移除时是否移除所有层数</summary>
    public bool removeAllStacksOnExpire;
}

/// <summary>
///     标记 Buff 已过期需要移除
/// </summary>
public struct BuffExpired : ITag;

#endregion

#region 光环组件

/// <summary>
///     光环标记
/// </summary>
public struct Aura : ITag;

/// <summary>
///     光环配置
/// </summary>
public struct AuraConfig : IComponent
{
    /// <summary>光环 ID</summary>
    public string auraId;

    /// <summary>影响范围</summary>
    public float radius;

    /// <summary>更新间隔（秒）</summary>
    public float updateInterval;

    /// <summary>距上次更新的时间</summary>
    public float timeSinceUpdate;

    /// <summary>是否影响自己</summary>
    public bool affectSelf;

    /// <summary>是否影响友军</summary>
    public bool affectAllies;

    /// <summary>是否影响敌军</summary>
    public bool affectEnemies;
}

/// <summary>
///     光环效果定义
/// </summary>
public struct AuraEffect : IComponent
{
    /// <summary>属性类型</summary>
    public AttrType attrType;

    /// <summary>修改类型</summary>
    public ModifyType modifyType;

    /// <summary>修改值</summary>
    public float value;
}

/// <summary>
///     光环 Buff 关系 - 标记某个 Buff 是由哪个光环产生的
/// </summary>
public struct AuraBuffLink : ILinkComponent
{
    public Entity GetIndexedValue() => auraSource;

    /// <summary>产生此 Buff 的光环 Entity</summary>
    public Entity auraSource;

    public AuraBuffLink(Entity source)
    {
        auraSource = source;
    }
}

#endregion
