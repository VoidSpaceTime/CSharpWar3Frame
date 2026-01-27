using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System.Collections.Generic;

namespace War3Frame;

/// <summary>
/// System 可实现此接口来声明自己的更新频率
/// </summary>
public interface ITimedSystem
{
    /// <summary>
    /// 更新间隔（秒），0 表示使用 Root 默认频率
    /// </summary>
    float Interval { get; }
}

/// <summary>
/// 支持每个 System 独立更新频率的 SystemRoot
/// 直接继承 SystemRoot，完全兼容 ECS 生态
/// </summary>
public class TimedSystemRoot : SystemRoot
{
    private readonly SortedDictionary<BaseSystem, TimerInfo> _timerInfos = new();

    /// <summary>
    /// 默认更新间隔（秒），System 未指定时使用此值，0 表示每帧更新
    /// </summary>
    public float DefaultInterval { get; set; } = 0.03125f;

    private class TimerInfo
    {
        public float Interval;
        public float Accumulated;
    }

    public TimedSystemRoot(EntityStore store) : base(store) { }

    /// <summary>
    /// 添加 System，自动读取 ITimedSystem.Interval，否则使用 DefaultInterval
    /// </summary>
    public new void Add(BaseSystem system)
    {
        base.Add(system);

        // 优先使用 System 自定义的 Interval
        float interval = DefaultInterval;
        if (system is ITimedSystem timedSystem && timedSystem.Interval > 0)
        {
            interval = timedSystem.Interval;
        }

        if (interval > 0)
        {
            _timerInfos[system] = new TimerInfo { Interval = interval };
        }
    }

    /// <summary>
    /// 添加 System 并手动指定更新间隔（覆盖 System 自定义值）
    /// </summary>
    public void Add(BaseSystem system, float interval)
    {
        base.Add(system);
        if (interval > 0)
        {
            _timerInfos[system] = new TimerInfo { Interval = interval };
        }
    }

    /// <summary>
    /// 动态设置 System 更新间隔
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
    /// 重写 Update - 对有间隔的 System，临时移除再添加回来以跳过本帧更新
    /// </summary>
    public new void Update(UpdateTick tick)
    {
        // 收集本帧需要跳过的 System
        var toSkip = new List<BaseSystem>();

        foreach (var kvp in _timerInfos)
        {
            var system = kvp.Key;
            var info = kvp.Value;

            info.Accumulated += tick.deltaTime;
            if (info.Accumulated < info.Interval)
            {
                toSkip.Add(system); // 还没到时间，跳过
            }
            else
            {
                info.Accumulated -= info.Interval; // 到时间了，执行
            }
        }

        // 临时移除需要跳过的 System
        foreach (var system in toSkip)
        {
            Remove(system);
        }

        // 执行更新
        base.Update(tick);

        // 恢复被移除的 System
        foreach (var system in toSkip)
        {
            base.Add(system);
        }
    }
}
