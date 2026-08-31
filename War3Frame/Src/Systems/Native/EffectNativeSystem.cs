using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;
using War3Frame.Library.Api;

namespace War3Frame;

/// <summary>
/// 特效原生执行系统。
/// 这是 EffectBase 到 War3 原生特效句柄的边界层；业务系统只写 ECS 意图和脏标记。
/// </summary>
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
                // 首次遇到 EffectBase 时创建原生句柄，并标记所有可同步字段需要下刷。
                native = new EffectNative
                {
                    effect = CreateNativeEffect(entity, effect)
                };
                entity.AddComponent(native);
                entity.AddComponent(new EffectDirty
                {
                    flags = EffectDirtyFlags.Color | EffectDirtyFlags.Scale | EffectDirtyFlags.Speed |
                            EffectDirtyFlags.Visible | EffectDirtyFlags.Alpha | EffectDirtyFlags.TeamColor |
                            EffectDirtyFlags.Transform
                });
            }

            // 更新位置
            if (entity.TryGetComponent<Position>(out var position))
            {
                YDApi.EXSetEffectXY(native.effect, position.x, position.y);
                YDApi.EXSetEffectZ(native.effect, position.z);
            }

            // 同步外观与变换脏标记
            if (entity.TryGetComponent<EffectDirty>(out var dirty))
            {
                // 只同步被标记为 dirty 的字段，避免每帧重复写所有原生属性。
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

                // 同步累积变换
                if (dirty.flags.HasFlag(EffectDirtyFlags.Transform)
                    && entity.TryGetComponent<EffectTransform>(out var transform))
                {
                    if (transform.needsReset)
                    {
                        YDApi.EXEffectMatReset(native.effect);
                    }

                    YDApi.EXEffectMatRotateX(native.effect, transform.rotateX);
                    YDApi.EXEffectMatRotateY(native.effect, transform.rotateY);
                    YDApi.EXEffectMatRotateZ(native.effect, transform.rotateZ);
                }

                entity.RemoveComponent<EffectDirty>();
            }

            // 播放动画 
            if (entity.TryGetComponent<EffectAnimationRequest>(out var animation))
            {
                KKApi.DzPlayEffectAnimation(native.effect, animation.animation, animation.link);
                entity.RemoveComponent<EffectAnimationRequest>();
            }

            if (entity.TryGetComponent<EffectDestroyRequest>(out var destroy))
            {
                // 销毁请求是一次性 native 副作用，执行后直接删除 ECS 特效实体。
                if (destroy.hideFirst)
                {
                    KKApi.DzSetEffectVisible(native.effect, false);
                }

                JassApi.DestroyEffect(native.effect);
                HandleHelper.HandleRemove(native.effect);
                entity.DeleteEntity();
            }
        });
    }

    private static JEffect CreateNativeEffect(Entity entity, EffectBase effect)
    {
        JEffect handle;
        if (effect.effectType == EffectType.Attach &&
            entity.TryGetComponent<EffectAttachment>(out var attachment) &&
            attachment.target.TryGetComponent<UnitNative>(out var unitNative))
        {
            handle = JassApi.AddSpecialEffectTarget(effect.model, unitNative.unit,
                GetAttachPointString(attachment.attachType));
        }
        else if (entity.TryGetComponent<Position>(out var position))
        {
            handle = JassApi.AddSpecialEffect(effect.model, position.x, position.y);
            YDApi.EXSetEffectZ(handle, position.z);
        }
        else
        {
            handle = JassApi.AddSpecialEffect(effect.model, 0, 0);
        }

        // 创建原生对象后立即登记句柄引用（配对规则：销毁前 HandleRemove，见 AGENTS.md）。
        if (handle.Handle > 0)
        {
            HandleHelper.HandleAdd(handle);
        }

        return handle;
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