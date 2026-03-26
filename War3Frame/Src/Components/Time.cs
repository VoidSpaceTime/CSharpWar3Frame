using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 通用计时模式
/// </summary>
public enum TimerTaskMode
{
    /// <summary>
    /// 触发一次后结束。
    /// </summary>
    Once,

    /// <summary>
    /// 按固定间隔循环触发。
    /// </summary>
    Loop
}

/// <summary>
/// 通用计时任务类型
/// </summary>
public enum TimerTaskKind
{
    /// <summary>
    /// 未指定类型。
    /// </summary>
    None,

    /// <summary>
    /// 单位尸体清理。
    /// </summary>
    CorpseCleanup,

    /// <summary>
    /// Buff 到期。
    /// </summary>
    BuffExpire,

    /// <summary>
    /// 特效到期。
    /// </summary>
    EffectExpire,

    /// <summary>
    /// 周期性效果触发。
    /// </summary>
    PeriodicEffect,
}

/// <summary>
/// 通用计时任务组件
/// </summary>
public struct TimerTask : IComponent
{
    /// <summary>计时模式。</summary>
    public TimerTaskMode mode;
    /// <summary>循环间隔。</summary>
    public float interval;
    /// <summary>剩余时间。</summary>
    public float remaining;
    /// <summary>是否暂停。</summary>
    public bool paused;
    /// <summary>所属实体。</summary>
    public Entity owner;
    /// <summary>任务类型。</summary>
    public TimerTaskKind kind;
    /// <summary>已触发次数。</summary>
    public int triggerCount;
    /// <summary>最大触发次数。0 表示不限制。</summary>
    public int maxTriggerCount;
}

/// <summary>
/// 计时任务过期标记
/// </summary>
public struct TimerExpired : ITag;
