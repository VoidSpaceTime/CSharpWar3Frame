using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;
using War3Frame.Library.Api;

namespace War3Frame;

[SystemRegister(SystemKind.Interval, 0)]
public class EffectNativeSystem : QuerySystem<EffectBase>, ITimedSystem
{
    public float Interval => 0.02f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref EffectBase effect, Entity entity) =>
        {
            if (!entity.TryGetComponent<EffectNative>(out var native))
            {
                native = new EffectNative
                {
                    effect = CreateNativeEffect(entity, effect)
                };
                entity.AddComponent(native);
                entity.AddComponent(new EffectDirty
                {
                    flags = EffectDirtyFlags.Color | EffectDirtyFlags.Scale | EffectDirtyFlags.Speed |
                            EffectDirtyFlags.Visible | EffectDirtyFlags.Alpha | EffectDirtyFlags.TeamColor
                });
            }

            if (entity.TryGetComponent<Position>(out var position))
            {
                YDApi.EXSetEffectXY(native.effect, position.x, position.y);
                YDApi.EXSetEffectZ(native.effect, position.z);
            }

            if (entity.TryGetComponent<EffectDirty>(out var dirty))
            {
                if (dirty.flags.HasFlag(EffectDirtyFlags.Alpha))
                {
                    KKApi.DzSetEffectVertexAlpha(native.effect, effect.alpha);
                }

                if (dirty.flags.HasFlag(EffectDirtyFlags.Color))
                {
                    KKApi.DzSetEffectVertexColor(native.effect,
                        DzApi.DzGetColor(effect.red, effect.green, effect.blue, effect.alpha));
                }

                if (dirty.flags.HasFlag(EffectDirtyFlags.Scale))
                {
                    YDApi.EXSetEffectSize(native.effect, effect.sizeScale);
                }

                if (dirty.flags.HasFlag(EffectDirtyFlags.Speed))
                {
                    YDApi.EXSetEffectSpeed(native.effect, effect.speed);
                }

                if (dirty.flags.HasFlag(EffectDirtyFlags.TeamColor))
                {
                    KKApi.DzSetEffectTeamColor(native.effect, effect.teamColor);
                }

                if (dirty.flags.HasFlag(EffectDirtyFlags.Visible))
                {
                    KKApi.DzSetEffectVisible(native.effect, effect.visible);
                }

                entity.RemoveComponent<EffectDirty>();
            }

            if (entity.TryGetComponent<EffectAnimationRequest>(out var animation))
            {
                KKApi.DzPlayEffectAnimation(native.effect, animation.animation, animation.link);
                entity.RemoveComponent<EffectAnimationRequest>();
            }
        });
    }

    private static JEffect CreateNativeEffect(Entity entity, EffectBase effect)
    {
        if (effect.effectType == EffectType.Attach &&
            entity.TryGetComponent<EffectAttachment>(out var attachment) &&
            attachment.target.TryGetComponent<UnitNative>(out var unitNative))
        {
            return JassApi.AddSpecialEffectTarget(effect.model, unitNative.unit,
                GetAttachPointString(attachment.attachType));
        }

        if (entity.TryGetComponent<Position>(out var position))
        {
            var handle = JassApi.AddSpecialEffect(effect.model, position.x, position.y);
            YDApi.EXSetEffectZ(handle, position.z);
            return handle;
        }

        return JassApi.AddSpecialEffect(effect.model, 0, 0);
    }

    private static string GetAttachPointString(EffectAttachType attachType)
    {
        return attachType switch
        {
            EffectAttachType.Head => "head",
            EffectAttachType.Origin => "origin",
            EffectAttachType.Weapon => "weapon",
            EffectAttachType.Chest => "chest",
            _ => "origin"
        };
    }
}
