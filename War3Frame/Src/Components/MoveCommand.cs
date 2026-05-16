using Friflo.Engine.ECS;

namespace War3Frame;

// ============================================================================
// 移动命令组件
// ============================================================================

/// <summary>
/// 移动命令 - 表示单位需要移动到指定位置
/// </summary>
// 规则层移动意图：业务系统只写入目标、原因和命令 token，原生命令由 Native/Execution 层消费。
public struct MoveCommand : IComponent
{
    /// <summary>目标 X 坐标</summary>
    // 目标点坐标，后续移动检测与原生命令执行共用同一份 ECS 真相。
    public float targetX;

    /// <summary>目标 Y 坐标</summary>
    public float targetY;

    /// <summary>到达距离（离目标多近算到达）</summary>
    // 到达判定半径；MoveSystem 使用它决定何时产出 MoveOutcome。
    public float arrivalDistance;

    /// <summary>移动原因（用于标识是什么触发的移动）</summary>
    public MoveReason reason;

    /// <summary>原生移动命令类型。</summary>
    public MoveOrderType orderType;

    /// <summary>命令 token，用于区分不同移动请求。</summary>
    public int commandToken;

    /// <summary>是否已经向原生层下发过命令。</summary>
    // false 表示尚未投递给原生层；执行层收到请求后再回写，避免重复下发同一移动命令。
    public bool issued;
}

/// <summary>
/// 移动原因枚举
/// </summary>
// 移动来源用于调试、覆盖策略和后续 continuation 分流，不直接决定原生 API 调用。
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
// 原生命令类型保持为小枚举，方便 Native 层统一映射到 Issue*Order。
public enum MoveOrderType
{
    Move,
    Stop,
    Hold,
}

/// <summary>
/// 移动结果类型。
/// </summary>
// 移动结果由规则层产出，施法、任务等上层流程只消费 outcome，不反查原生状态。
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
// 记录当前移动命令是否已经进入执行流程，用于幂等保护。
public struct MoveExecutionState : IComponent
{
    public int commandToken;
    public bool hasStarted;
}

/// <summary>
/// 移动结果组件。
/// 由 move 子系统产出，供上层调用方消费。
/// </summary>
// 移动完成/失败的短生命周期结果，消费者处理完后应移除或覆盖。
public struct MoveOutcome : IComponent
{
    public int commandToken;
    public MoveOutcomeType outcome;
}

/// <summary>
/// 移动后的后续动作类型。
/// </summary>
// 到达后的后续动作类型，避免 MoveSystem 直接依赖具体业务流程。
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
// continuation 保存上层流程恢复所需的最小上下文，例如到达后继续施法或执行任务。
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
// 任务型移动的本地状态，供任务系统判断移动子流程是否已经结束。
public struct MoveTaskState : IComponent
{
    public bool completed;
    public bool cancelled;
}

/// <summary>
/// 待执行的原生命令请求。
/// 仅由执行层消费。
/// </summary>
// 发往原生执行层的一次性请求；它只表达“请执行命令”，不拥有长期移动语义。
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
// 标记单位当前处于移动流程中，便于查询系统快速筛选。
public struct MovingTag : ITag { }
