using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems;

public class EffectSystem : QuerySystem<EffectBase>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref EffectBase fx, ref EffectNative native, ref EffectDirty dirty, Entity e) =>
        {
            if (dirty.flags.HasFlag(EffectDirtyFlags.Color))
                JassApi.SetEffectColor(native.effect, fx.red, fx.green, fx.blue);

            if (dirty.flags.HasFlag(EffectDirtyFlags.Scale))
                JassApi.SetEffectScale(native.effect, fx.sizeScale);

            // ... 其他属性

            dirty.flags = EffectDirtyFlags.None;
        });
    }
}