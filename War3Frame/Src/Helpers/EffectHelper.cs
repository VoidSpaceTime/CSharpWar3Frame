using Friflo.Engine.ECS;
using War3Frame.Library.Api;

namespace War3Frame;

/// <summary>
/// 特效帮助类 - 创建、销毁、操作特效
/// 基于 effector.lua 移植
/// </summary>
public static class EffectHelper
{
    #region 创建特效

    /// <summary>
    /// 创建点特效（在指定坐标）
    /// </summary>
    /// <param name="model">模型路径</param>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <param name="z">Z坐标（默认使用地形高度）</param>
    /// <param name="duration">持续时间。0=立即销毁，-1=永久，>0=持续指定秒数</param>
    /// <returns>特效 Entity，如果 duration=0 则返回 null</returns>
    public static Entity? CreatePosition(string model, float x, float y, float z = 0, float duration = -1)
    {
        if (duration == 0)
        {
            var onceEffect = JassApi.AddSpecialEffect(model, x, y);
            YDApi.EXSetEffectZ(onceEffect, z);
            JassApi.DestroyEffect(onceEffect);
            return null;
        }

        // 创建 ECS Entity
        var entity = Game.Store.CreateEntity(
            new EffectBase
            {
                model = model,
                sizeScale = 1.0f,
                speed = 1.0f,
                visible = true,
                alpha = 255,
                red = 255,
                green = 255,
                blue = 255,
                duration = duration,
                effectType = EffectType.Position
            },
            new Position { x = x, y = y, z = z }
        );

        // 如果有持续时间，添加定时销毁逻辑（需要 TimerSystem 支持）
        // if (duration > 0) { ... }

        return entity;
    }

    /// <summary>
    /// 创建附着特效（绑定到单位）
    /// </summary>
    /// <param name="unit">目标单位 Entity</param>
    /// <param name="model">模型路径</param>
    /// <param name="attachPoint">附着点</param>
    /// <param name="duration">持续时间。0=立即销毁，-1=永久，>0=持续指定秒数</param>
    /// <returns>特效 Entity</returns>
    public static Entity? CreateAttached(Entity unit, string model,
        EffectAttachType attachPoint = EffectAttachType.Origin, float duration = -1)
    {
        if (duration == 0)
        {
            if (!unit.TryGetComponent<UnitNative>(out var onceUnitNative)) return null;
            var onceAttachPoint = GetAttachPointString(attachPoint);
            var onceHandle = JassApi.AddSpecialEffectTarget(model, onceUnitNative.unit, onceAttachPoint);
            JassApi.DestroyEffect(onceHandle);
            return null;
        }

        var entity = Game.Store.CreateEntity(
            new EffectBase
            {
                model = model,
                sizeScale = 1.0f,
                speed = 1.0f,
                visible = true,
                alpha = 255,
                red = 255,
                green = 255,
                blue = 255,
                duration = duration,
                effectType = EffectType.Attach,
                effectAttachType = attachPoint
            },
            new EffectAttachment
            {
                target = unit,
                attachType = attachPoint
            }
        );

        // 建立 Unit -> Effect 关系（可选）
        // unit.AddRelation(new HasEffect(entity));

        return entity;
    }

    #endregion

    #region 销毁特效

    /// <summary>
    /// 销毁特效
    /// </summary>
    /// <param name="entity">特效 Entity</param>
    /// <param name="hideFirst">销毁前是否先隐藏（避免闪烁）</param>
    public static void Destroy(Entity entity, bool hideFirst = false)
    {
        if (entity.TryGetComponent<EffectNative>(out var native))
        {
            if (hideFirst)
            {
                KKApi.DzSetEffectVisible(native.effect, false);
            }

            JassApi.DestroyEffect(native.effect);
        }

        entity.DeleteEntity();
    }

    #endregion

    #region 属性设置（自动标记脏）

    /// <summary>设置特效显示/隐藏</summary>
    public static void SetVisible(Entity entity, bool visible)
    {
        if (!entity.TryGetComponent<EffectBase>(out var effect)) return;
        effect.visible = visible;
        entity.AddComponent(effect);
        MarkDirty(entity, EffectDirtyFlags.Visible);
    }

    /// <summary>设置特效大小</summary>
    public static void SetScale(Entity entity, float scale)
    {
        if (!entity.TryGetComponent<EffectBase>(out var effect)) return;
        effect.sizeScale = scale;
        entity.AddComponent(effect);
        MarkDirty(entity, EffectDirtyFlags.Scale);
    }

    /// <summary>设置特效播放速度</summary>
    public static void SetSpeed(Entity entity, float speed)
    {
        if (!entity.TryGetComponent<EffectBase>(out var effect)) return;
        effect.speed = speed;
        entity.AddComponent(effect);
        MarkDirty(entity, EffectDirtyFlags.Speed);
    }

    /// <summary>设置特效透明度 (0-255)</summary>
    public static void SetAlpha(Entity entity, int alpha)
    {
        if (!entity.TryGetComponent<EffectBase>(out var effect)) return;
        effect.alpha = Math.Clamp(alpha, 0, 255);
        entity.AddComponent(effect);
        MarkDirty(entity, EffectDirtyFlags.Alpha);
    }

    /// <summary>设置特效颜色 (RGB 0-255)</summary>
    public static void SetColor(Entity entity, int red, int green, int blue)
    {
        if (!entity.TryGetComponent<EffectBase>(out var effect)) return;
        effect.red = Math.Clamp(red, 0, 255);
        effect.green = Math.Clamp(green, 0, 255);
        effect.blue = Math.Clamp(blue, 0, 255);
        entity.AddComponent(effect);
        MarkDirty(entity, EffectDirtyFlags.Color);
    }

    /// <summary>设置特效颜色和透明度</summary>
    public static void SetColorWithAlpha(Entity entity, int red, int green, int blue, int alpha)
    {
        if (!entity.TryGetComponent<EffectBase>(out var effect)) return;
        effect.red = Math.Clamp(red, 0, 255);
        effect.green = Math.Clamp(green, 0, 255);
        effect.blue = Math.Clamp(blue, 0, 255);
        effect.alpha = Math.Clamp(alpha, 0, 255);
        entity.AddComponent(effect);
        MarkDirty(entity, EffectDirtyFlags.Color | EffectDirtyFlags.Alpha);
    }

    /// <summary>设置特效队伍颜色</summary>
    public static void SetTeamColor(Entity entity, int playerId)
    {
        if (!entity.TryGetComponent<EffectBase>(out var effect)) return;
        effect.teamColor = playerId;
        entity.AddComponent(effect);
        MarkDirty(entity, EffectDirtyFlags.TeamColor);
    }

    /// <summary>设置特效位置（仅点特效有效）</summary>
    public static void SetPosition(Entity entity, float x, float y, float z)
    {
        var pos = entity.TryGetComponent<Position>(out var existing)
            ? existing
            : new Position();
        pos.x = x;
        pos.y = y;
        pos.z = z;
        entity.AddComponent(pos);
    }

    /// <summary>播放特效动画</summary>
    public static void PlayAnimation(Entity entity, string animName, string link = "")
    {
        entity.AddComponent(new EffectAnimationRequest
        {
            animation = animName,
            link = link
        });
    }

    /// <summary>重置特效矩阵（旋转归零）</summary>
    public static void Reset(Entity entity)
    {
        if (!entity.TryGetComponent<EffectNative>(out var native)) return;
        YDApi.EXEffectMatReset(native.effect);
    }

    /// <summary>设置特效 X 轴旋转</summary>
    public static void SetRotateX(Entity entity, float angle)
    {
        if (!entity.TryGetComponent<EffectNative>(out var native)) return;
        YDApi.EXEffectMatRotateX(native.effect, angle);
    }

    /// <summary>设置特效 Y 轴旋转</summary>
    public static void SetRotateY(Entity entity, float angle)
    {
        if (!entity.TryGetComponent<EffectNative>(out var native)) return;
        YDApi.EXEffectMatRotateY(native.effect, angle);
    }

    /// <summary>设置特效 Z 轴旋转</summary>
    public static void SetRotateZ(Entity entity, float angle)
    {
        if (!entity.TryGetComponent<EffectNative>(out var native)) return;
        YDApi.EXEffectMatRotateZ(native.effect, angle);
    }

    #endregion

    #region 私有方法

    private static void MarkDirty(Entity entity, EffectDirtyFlags flag)
    {
        if (entity.TryGetComponent<EffectDirty>(out var dirty))
        {
            dirty.flags |= flag;
            entity.AddComponent(dirty);
        }
        else
        {
            entity.AddComponent(new EffectDirty { flags = flag });
        }
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

    #endregion
}
