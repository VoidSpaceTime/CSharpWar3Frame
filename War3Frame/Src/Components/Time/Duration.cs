using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 统一持续时间组件。
/// remaining = -1 表示永久；0 表示立即到期；>0 表示剩余秒数。
/// 由 DurationSystem 统一递减，业务代码只读不写 remaining。
/// </summary>
public struct Duration : IComponent
{
    /// <summary>剩余秒数；-1 = 永久</summary>
    public float remaining;

    /// <summary>初始总时长，供进度显示</summary>
    public float total;

    /// <summary>
    /// 创建持续时间。-1 或 0 表示永久/立即到期的约定由调用方语义决定。
    /// </summary>
    public static Duration Create(float seconds)
    {
        return new Duration
        {
            remaining = seconds,
            total = seconds
        };
    }
}

/// <summary>
/// 持续时间到期标记（内部阶段）。
/// 由 DurationSystem 打标，各领域系统消费后执行各自的到期动作并清除。
/// </summary>
public struct DurationExpired : ITag;