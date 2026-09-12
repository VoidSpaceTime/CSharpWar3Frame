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
    private readonly List<(Entity entity, EffectNative native)> _toAddNative = new();
    private readonly List<Entity> _toClearDirty = new();
    private readonly List<Entity> _toClearAnim = new();
    private readonly List<Entity> _toDelete = new();
    private ForEachEntity<EffectBase>? _syncEffect;

    protected override void OnUpdate()
    {
        // Friflo 约束：Query 迭代内禁止 AddComponent/RemoveComponent/DeleteEntity。
        // 原生副作用（YDApi/KKApi/JassApi）仍在循环内执行，ECS 结构变更收集到循环外应用。
        _toAddNative.Clear();
        _toClearDirty.Clear();
        _toClearAnim.Clear();
        _toDelete.Clear();

        Query.ForEachEntity(_syncEffect ??= (ref EffectBase effect, Entity entity) =>
        {
            var needsFullSync = false;
            if (!entity.TryGetComponent<EffectNative>(out var native))
            {
                // 首次遇到 EffectBase 时创建原生句柄；本轮直接全字段同步，无需再挂一次脏标记。
                native = new EffectNative
                {
                    effect = CreateNativeEffect(entity, effect)
                };
                needsFullSync = true;
            }

            // 更新位置
            if (entity.TryGetComponent<Position>(out var position))
            {
                if (!native.positionSynced || native.syncedPosition.x != position.x || native.syncedPosition.y != position.y)
                    YDApi.EXSetEffectXY(native.effect, position.x, position.y);
                if (!native.positionSynced || native.syncedPosition.z != position.z)
                    YDApi.EXSetEffectZ(native.effect, position.z);
                native.syncedPosition = position;
                native.positionSynced = true;
            }
            else native.positionSynced = false;

            if (needsFullSync) _toAddNative.Add((entity, native));
            else entity.GetComponent<EffectNative>() = native;

            // 同步外观与变换脏标记：首次创建按全字段同步，否则只处理被标记的字段。
            EffectDirtyFlags flags;
            var hasDirty = false;
            if (needsFullSync)
            {
                flags = EffectDirtyFlags.Color | EffectDirtyFlags.Scale | EffectDirtyFlags.Speed |
                        EffectDirtyFlags.Visible | EffectDirtyFlags.Alpha | EffectDirtyFlags.TeamColor |
                        EffectDirtyFlags.Transform;
                hasDirty = true;
            }
            else if (entity.TryGetComponent<EffectDirty>(out var dirty))
            {
                flags = dirty.flags;
                hasDirty = true;
            }
            else
            {
                flags = default;
            }

            if (hasDirty)
            {
                if (flags.HasFlag(EffectDirtyFlags.Alpha))
                {
                    KKApi.DzSetEffectVertexAlpha(native.effect, effect.alpha);
                }

                if (flags.HasFlag(EffectDirtyFlags.Color))
                {
                    KKApi.DzSetEffectVertexColor(native.effect,
                        DzApi.DzGetColor(effect.red, effect.green, effect.blue, effect.alpha));
                }

                if (flags.HasFlag(EffectDirtyFlags.Speed))
                {
                    YDApi.EXSetEffectSpeed(native.effect, effect.speed);
                }

                if (flags.HasFlag(EffectDirtyFlags.TeamColor))
                {
                    KKApi.DzSetEffectTeamColor(native.effect, effect.teamColor);
                }

                if (flags.HasFlag(EffectDirtyFlags.Visible))
                {
                    KKApi.DzSetEffectVisible(native.effect, effect.visible);
                }

                // 同步累积变换
                if (flags.HasFlag(EffectDirtyFlags.Transform)
                    && entity.TryGetComponent<EffectTransform>(out var transform))
                {
                    // 原生旋转是增量操作，ECS 保存累计角度：每次重建，不能重复叠加旧角度。
                    YDApi.EXEffectMatReset(native.effect);
                    YDApi.EXEffectMatRotateX(native.effect, transform.rotateX);
                    YDApi.EXEffectMatRotateY(native.effect, transform.rotateY);
                    YDApi.EXEffectMatRotateZ(native.effect, transform.rotateZ);
                }

                // Reset 会清除缩放，变换后重放 ECS 的绝对缩放值。
                if ((flags & (EffectDirtyFlags.Scale | EffectDirtyFlags.Transform)) != 0)
                    YDApi.EXSetEffectSize(native.effect, effect.sizeScale);

                if (entity.HasComponent<EffectDirty>())
                {
                    _toClearDirty.Add(entity);
                }
            }

            // 播放动画
            if (entity.TryGetComponent<EffectAnimationRequest>(out var animation))
            {
                KKApi.DzPlayEffectAnimation(native.effect, animation.animation, animation.link);
                _toClearAnim.Add(entity);
            }

            if (entity.TryGetComponent<EffectDestroyRequest>(out var destroy))
            {
                // 销毁请求是一次性 native 副作用，执行后删除 ECS 特效实体（删除收集到循环外）。
                if (destroy.hideFirst)
                {
                    KKApi.DzSetEffectVisible(native.effect, false);
                }

                HandleHelper.HandleRemove(native.effect);
                JassApi.DestroyEffect(native.effect);
                _toDelete.Add(entity);
            }
        });

        foreach (var (entity, native) in _toAddNative)
        {
            if (!entity.IsNull && !entity.HasComponent<EffectNative>())
                entity.AddComponent(native);
        }

        foreach (var entity in _toClearDirty)
        {
            if (!entity.IsNull && entity.HasComponent<EffectDirty>())
                entity.RemoveComponent<EffectDirty>();
        }

        foreach (var entity in _toClearAnim)
        {
            if (!entity.IsNull && entity.HasComponent<EffectAnimationRequest>())
                entity.RemoveComponent<EffectAnimationRequest>();
        }

        foreach (var entity in _toDelete)
        {
            if (!entity.IsNull)
                entity.DeleteEntity();
        }
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
            HandleHelper.HandleAdd(handle);
        }
        else if (entity.TryGetComponent<Position>(out var position))
        {
            handle = JassApi.AddSpecialEffect(effect.model, position.x, position.y);
            HandleHelper.HandleAdd(handle);
        }
        else
        {
            handle = JassApi.AddSpecialEffect(effect.model, 0, 0);
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
