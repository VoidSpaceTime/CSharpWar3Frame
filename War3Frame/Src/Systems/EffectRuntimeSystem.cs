using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems;

/// <summary>
/// 持续特效运行时系统。
/// 负责 0.02s cadence 下的附着跟随与到期销毁：
/// - Attach 特效：跟随目标单位位置（与 Duration 无关）
/// - DurationExpired 标记：销毁特效（隐藏后销毁）
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class EffectRuntimeSystem : QuerySystem<EffectBase>, ITimedSystem
{
    public float Interval => 0.02f;

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref EffectBase effect, Entity entity) =>
        {
            if (entity.Tags.Has<DurationExpired>())
            {
                toDelete.Add(entity);
                return;
            }

            if (effect.effectType == EffectType.Attach &&
                entity.TryGetComponent<EffectAttachment>(out var attachment))
            {
                if (attachment.target.IsNull || !attachment.target.TryGetComponent<Position>(out var targetPos))
                {
                    toDelete.Add(entity);
                    return;
                }

                var pos = entity.TryGetComponent<Position>(out var existingPos)
                    ? existingPos
                    : new Position();
                pos.x = targetPos.x;
                pos.y = targetPos.y;
                pos.z = targetPos.z;
                entity.AddComponent(pos);
            }
        });

        foreach (var entity in toDelete)
        {
            EffectHelper.Destroy(entity, hideFirst: true);
        }
    }
}