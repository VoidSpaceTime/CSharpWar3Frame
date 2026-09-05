using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame;

#region Buff 组件

/// <summary>
///     Buff 分类标签（位组合，供净化/免疫判定）。
///     替代早期的字符串标签：位运算 O(1)、编译期检查、避免每次创建分配 List。
/// </summary>
[Flags]
public enum BuffTag
{
    None = 0,
    Debuff = 1 << 0,
    Control = 1 << 1,
    Stun = 1 << 2,        // 便捷方法细分
    Root = 1 << 3,
    Silence = 1 << 4,
    DoT = 1 << 5,
    Fire = 1 << 6,        // 元素预留
    Frost = 1 << 7,
    Poison = 1 << 8,
    // 后续元素/词缀按需扩展
}

/// <summary>
///     Buff 实体类型。
///     Attribute = 属性贡献（挂 ModifyValue）；Tick = 周期行为（DoT，不挂 ModifyValue）；PureTag = 纯标记。
/// </summary>
public enum BuffKind
{
    Attribute,
    Tick,
    PureTag
}

/// <summary>
///     Buff 标记 - 表示这是一个 Buff 实体
/// </summary>
public struct Buff : IComponent
{
    /// <summary>Buff 类型 ID</summary>
    public string buffId;

    /// <summary>实例 ID（全局唯一，用于级联清理）</summary>
    public long buffInstanceId;

    /// <summary>实体类型（Attribute/Tick/PureTag）</summary>
    public BuffKind kind;

    /// <summary>分类标签（位组合，净化/免疫用）</summary>
    public BuffTag tags;

    /// <summary>造成效果的施法者单位（DoT 伤害来源；无单位来源时为 default）</summary>
    public Entity caster;

    /// <summary>周期 tick 间隔（秒，0 = 不 tick）</summary>
    public float tickInterval;

    /// <summary>Tick 行为 ID（指向注册表）</summary>
    public string? tickActionId;

    /// <summary>上次 tick 时间（内部字段）</summary>
    public float lastTick;

    /// <summary>每跳数值（DoT 模式用，不参与属性贡献；非 tick 型忽略）</summary>
    public float tickValue;
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
            current = 1, // 初始即为 1 层（首个 Buff 即占一层）
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
    Independent,

    /// <summary>删旧建新（完整替换）</summary>
    Replace,

    /// <summary>仅当新 duration 更长时替换</summary>
    ReplaceIfLonger
}

/// <summary>
///     Buff 行为配置。
///     每个 buff 实体经 CreateBuffInternal 必挂，作清理/到期系统的 Query 锚点；仅承载表现配置。
/// </summary>
public struct BuffBehavior : IComponent
{
    /// <summary>UI 图标路径</summary>
    public string? icon;
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
    public int attrType;

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