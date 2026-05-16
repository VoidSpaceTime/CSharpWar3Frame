using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame;

/// <summary>
///     System 可实现此接口来声明自己的更新频率
/// </summary>
public interface ITimedSystem
{
    // 返回 0 表示跟随 Root 默认调度频率，正数表示该系统拥有独立间隔。
    /// <summary>
    ///     更新间隔（秒），0 表示使用 Root 默认频率
    /// </summary>
    float Interval { get; }
}

/// <summary>
///     支持每个 System 独立更新频率的 SystemRoot
///     通过条件执行保证系统顺序稳定
/// </summary>
public class TimedSystemRoot : SystemRoot
{
    // 每个业务 System 被包进单独 SystemGroup，确保 Friflo 的执行顺序仍由 Root 管理。
    private readonly Dictionary<BaseSystem, TimerInfo> _timerInfos = new();
    private readonly Dictionary<BaseSystem, SystemGroup> _systemGroups = new();
    private readonly List<BaseSystem> _systems = [];

    public TimedSystemRoot(EntityStore store) : base(store)
    {
    }

    /// <summary>
    ///     默认更新间隔（秒），System 未指定时使用此值，0 表示每帧更新
    /// </summary>
    public float DefaultInterval { get; set; } = 0.03125f;

    /// <summary>
    ///     添加 System，自动读取 ITimedSystem.Interval，否则使用 DefaultInterval
    /// </summary>
    public new void Add(BaseSystem system)
    {
        var interval = DefaultInterval;
        if (system is ITimedSystem timedSystem && timedSystem.Interval > 0)
        {
            // 系统声明自己的节奏时优先使用系统值，便于低频逻辑降低开销。
            interval = timedSystem.Interval;
        }

        Add(system, interval);
    }

    /// <summary>
    ///     添加 System 并手动指定更新间隔（覆盖 System 自定义值）
    /// </summary>
    public void Add(BaseSystem system, float interval)
    {
        if (_systemGroups.ContainsKey(system))
        {
            SetInterval(system, interval);
            return;
        }

        var group = new SystemGroup(system.Name);
        group.Add(system);

        base.Add(group);
        _systemGroups[system] = group;
        _systems.Add(system);

        if (interval > 0)
        {
            _timerInfos[system] = new TimerInfo { Interval = interval };
        }
    }

    /// <summary>
    ///     动态设置 System 更新间隔
    /// </summary>
    public void SetInterval(BaseSystem system, float interval)
    {
        if (interval <= 0)
        {
            _timerInfos.Remove(system);
            return;
        }

        if (!_timerInfos.TryGetValue(system, out var info))
        {
            info = new TimerInfo();
            _timerInfos[system] = info;
        }

        info.Interval = interval;
        info.Accumulated = 0;
    }

    /// <summary>
    ///     移除 System
    /// </summary>
    public new void Remove(BaseSystem system)
    {
        if (_systemGroups.Remove(system, out var group))
        {
            base.Remove(group);
        }
        else
        {
            base.Remove(system);
        }

        _systems.Remove(system);
        _timerInfos.Remove(system);
    }

    /// <summary>
    ///     重写 Update - 按 System 独立计时并为每个被调度的 System 传入实际累计 deltaTime
    /// </summary>
    public new void Update(UpdateTick tick)
    {
        foreach (var system in _systems)
        {
            if (!_systemGroups.TryGetValue(system, out var group))
            {
                continue;
            }

            if (!system.Enabled)
            {
                continue;
            }

            if (!_timerInfos.TryGetValue(system, out var info))
            {
                // 没有 timer 信息的系统按每次 Root Update 立即执行。
                group.Update(tick);
                continue;
            }

            info.Accumulated += tick.deltaTime;
            if (info.Accumulated < info.Interval)
            {
                continue;
            }

            var elapsed = info.Accumulated;
            var overrun = elapsed % info.Interval;
            info.Accumulated = overrun;

            // 传入累计 elapsed，避免低频系统在卡顿帧丢失应推进的时间。
            group.Update(new UpdateTick(elapsed, tick.time));
        }
    }

    private class TimerInfo
    {
        public float Accumulated;
        public float Interval;
    }
}
