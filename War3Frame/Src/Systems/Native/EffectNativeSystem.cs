using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Library.Api;

namespace War3Frame;

public class EffectNativeSystem : QuerySystem<EffectNative, EffectBase, EffectDirty>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref EffectNative native, ref EffectBase effect, ref EffectDirty dirty, Entity entity) =>
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
        });
    }
}