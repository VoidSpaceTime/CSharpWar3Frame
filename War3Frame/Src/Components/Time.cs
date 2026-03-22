using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 通用计时模式
/// </summary>
public enum TimerTaskMode
{
    Once,
    Loop
}

/// <summary>
/// 通用计时任务类型
/// </summary>
public enum TimerTaskKind
{
    None,
    CorpseCleanup,
    BuffExpire,
    EffectExpire,
    PeriodicEffect,
}

/// <summary>
/// 通用计时任务组件
/// </summary>
public struct TimerTask : IComponent
{
    public TimerTaskMode mode;
    public float interval;
    public float remaining;
    public bool paused;
    public Entity owner;
    public TimerTaskKind kind;
    public int triggerCount;
    public int maxTriggerCount;
}

/// <summary>
/// 计时任务过期标记
/// </summary>
public struct TimerExpired : ITag;