using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Library.Api;

namespace War3Frame;

public class EffectNativeSystem : QuerySystem<EffectNative>
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
                KKApi.DzSetEffectVertexColor(native.effect, effect.red, effect.green, effect.blue);
            }

            if (dirty.flags.HasFlag(EffectDirtyFlags.Scale))
            {
                JassApi.SetEffectScale(native.effect, effect.sizeScale);
            }

            if (dirty.flags.HasFlag(EffectDirtyFlags.Speed))
            {
                JassApi.SetEffectSpeed(native.effect, effect.speed);
            }

            if (dirty.flags.HasFlag(EffectDirtyFlags.TeamColor))
            {
                JassApi.SetEffectTeamColor(native.effect, effect.teamColor);
            }

            if (dirty.flags.HasFlag(EffectDirtyFlags.Visible))
            {
                JassApi.SetEffectVisible(native.effect, effect.visible);
            }

            entity.RemoveComponent<EffectDirty>();
        });
    }
}