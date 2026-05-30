using Friflo.Engine.ECS;

namespace War3Frame;

// ============================================================================
// 施法状态机组件
// ============================================================================

/// <summary>
/// 施法阶段枚举
/// </summary>
public enum CastPhase
{
    None,           // 空闲
    MovingToCast,   // 移动到施法范围
    Casting,        // 吟唱中
    Channeling,     // 持续施法中
    Backswing,      // 后摇中
}

/// <summary>
/// 施法请求组件 - 表示玩家/AI 想要施放技能的意图
/// 由输入系统添加，由施法系统处理后移除
/// </summary>
public struct CastRequest : IComponent
{
    /// <summary>要施放的技能 Entity</summary>
    public Entity ability;

    /// <summary>目标单位（对单位施法时）</summary>
    public Entity targetUnit;

    /// <summary>目标点 X 坐标（对点施法时）</summary>
    public float targetX;

    /// <summary>目标点 Y 坐标（对点施法时）</summary>
    public float targetY;
}

/// <summary>
/// 施法状态组件 - 附加到单位 Entity 上，表示当前的施法状态
/// </summary>
public struct CastState : IComponent
{
    /// <summary>当前施法阶段</summary>
    public CastPhase phase;

    /// <summary>当前施放的技能 Entity</summary>
    public Entity ability;

    /// <summary>目标单位</summary>
    public Entity targetUnit;

    /// <summary>目标点 X</summary>
    public float targetX;

    /// <summary>目标点 Y</summary>
    public float targetY;

    /// <summary>阶段计时器（吟唱/持续时间倒计时）</summary>
    public float timer;

    /// <summary>生效点是否已经提交。</summary>
    public bool effectCommitted;
}

/// <summary>
/// 持续施法状态组件 - 当技能需要持续施法时添加
/// </summary>
public struct ChannelState : IComponent
{
    /// <summary>持续施法剩余时间</summary>
    public float remaining;

    /// <summary>持续施法总时长</summary>
    public float duration;

    /// <summary>持续施法的技能 Entity</summary>
    public Entity ability;

    /// <summary>持续施法 tick 间隔。</summary>
    public float tickInterval;

    /// <summary>下一次 tick 的剩余时间。</summary>
    public float tickTimer;
}

/// <summary>
/// 移动到施法范围标记 - 用于标识单位正在因为施法而移动
/// </summary>
public struct MovingForCastTag : ITag { }

/// <summary>
/// 施法被打断标记 - 用于标识施法被外部因素打断
/// </summary>
public struct CastInterruptedTag : ITag { }
