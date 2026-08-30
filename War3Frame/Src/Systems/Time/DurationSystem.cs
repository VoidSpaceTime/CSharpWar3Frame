using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;

namespace War3Frame.Systems.Time;

/// <summary>
/// 统一持续时间推进系统。
/// 递减所有挂 Duration 组件的实体；-1 永久跳过；<=0 打 DurationExpired 标记。
/// 不做任何领域清理——到期动作由各领域系统消费 DurationExpired 执行。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class DurationSystem : QuerySystem<Duration>
{
    /// <summary>推进 cadence：0.02s（与游戏主 tick 对齐）</summary>
    public float Interval => 0.02f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref Duration duration, Entity entity) =>
        {
            if (duration.remaining < 0f)
            {
                return; // 永久，不递减
            }

            duration.remaining -= Tick.deltaTime;
            if (duration.remaining <= 0f)
            {
                duration.remaining = 0f;
                if (!entity.Tags.Has<DurationExpired>())
                {
                    entity.AddTag<DurationExpired>();
                }
            }

            entity.AddComponent(duration);
        });
    }
}