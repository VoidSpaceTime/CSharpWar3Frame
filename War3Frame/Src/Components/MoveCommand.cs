using Friflo.Engine.ECS;

namespace War3Frame;

// ============================================================================
// 移动命令组件
// ============================================================================

/// <summary>
/// 移动命令 - 表示单位需要移动到指定位置
/// </summary>
public struct MoveCommand : IComponent
{
    /// <summary>目标 X 坐标</summary>
    public float targetX;

    /// <summary>目标 Y 坐标</summary>
    public float targetY;

    /// <summary>到达距离（离目标多近算到达）</summary>
    public float arrivalDistance;

    /// <summary>移动原因（用于标识是什么触发的移动）</summary>
    public MoveReason reason;
}

/// <summary>
/// 移动原因枚举
/// </summary>
public enum MoveReason
{
    PlayerCommand,   // 玩家直接命令
    CastingAbility,  // 施法需要
    Following,       // 跟随目标
    Fleeing,         // 逃跑
    Patrol,          // 巡逻
    AutoAttack,      // 自动攻击靠近
}

/// <summary>
/// 移动中标记 - 标识单位正在移动
/// </summary>
public struct MovingTag : ITag { }

/// <summary>
/// 到达目标标记 - 标识单位已到达移动目标
/// </summary>
public struct ArrivedTag : ITag { }
