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

    /// <summary>原生命令类型。</summary>
    public MoveOrderType orderType;

    /// <summary>命令令牌，用于区分不同移动请求。</summary>
    public int commandToken;

    /// <summary>是否已经向原生层下发过命令。</summary>
    public bool issued;
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
/// 原生移动命令类型。
/// </summary>
public enum MoveOrderType
{
    Move,
    Stop,
    Hold,
}

/// <summary>
/// 移动结果类型。
/// </summary>
public enum MoveOutcomeType
{
    Arrived,
    Cancelled,
    Overridden,
    Interrupted,
    Failed,
}

/// <summary>
/// 移动执行状态。
/// </summary>
public struct MoveExecutionState : IComponent
{
    public int commandToken;
    public bool hasStarted;
}

/// <summary>
/// 移动结果组件。
/// 由 move 子系统产出，供上层调用方消费。
/// </summary>
public struct MoveOutcome : IComponent
{
    public int commandToken;
    public MoveOutcomeType outcome;
}

/// <summary>
/// 移动后的后续动作类型。
/// </summary>
public enum MoveContinuationKind
{
    None,
    CastAbility,
    ExecuteTask,
}

/// <summary>
/// 移动后的后续动作数据。
/// 由上层调用方填写，由上层调用方解释。
/// </summary>
public struct MoveContinuation : IComponent
{
    public MoveContinuationKind kind;
    public Entity ability;
    public Entity targetUnit;
    public float targetX;
    public float targetY;
}

/// <summary>
/// 通用移动任务状态。
/// 用于证明 move 子系统可被施法之外的工作流复用。
/// </summary>
public struct MoveTaskState : IComponent
{
    public bool completed;
    public bool cancelled;
}

/// <summary>
/// 待执行的原生命令请求。
/// 仅由执行层消费。
/// </summary>
public struct MoveNativeCommandRequest : IComponent
{
    public int commandToken;
    public MoveOrderType orderType;
    public float targetX;
    public float targetY;
}

/// <summary>
/// 移动中标记 - 标识单位正在移动
/// </summary>
public struct MovingTag : ITag { }
