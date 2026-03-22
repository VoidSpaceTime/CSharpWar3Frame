using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.TemplateInit;

namespace War3Frame;

/// <summary>
///     单位辅助类 - 提供单位的创建和操作
/// </summary>
public static class UnitHelper
{
    #region 创建单位

    /// <summary>
    ///     从模板创建单位
    /// </summary>
    /// <param name="templateName">模板名称</param>
    /// <param name="player">所属玩家</param>
    /// <param name="x">X 坐标</param>
    /// <param name="y">Y 坐标</param>
    /// <param name="facing">朝向角度（默认 270）</param>
    /// <returns>创建的单位 Entity</returns>
    public static Entity CreateUnit(
        string templateName,
        JPlayer player,
        float x, float y,
        float facing = 270)
    {
        var entity = UnitTemplate.Create(templateName, player, x, y, facing);
        Game.FlushImmediateSystems();
        return entity;
    }

    /// <summary>
    ///     从模板创建单位 + 额外配置
    /// </summary>
    public static Entity CreateUnit(
        string templateName,
        JPlayer player,
        float x, float y,
        Action<Entity> extraConfig)
    {
        var entity = CreateUnit(templateName, player, x, y);
        extraConfig(entity);
        return entity;
    }

    #endregion

    #region 删除单位

    /// <summary>
    ///     删除单位
    /// </summary>
    public static void RemoveUnit(Entity unit)
    {
        if (unit.IsNull) return;

        // 有原生单位时走立即死亡流程
        if (unit.TryGetComponent<UnitNative>(out var native))
        {
            UnitNativeDirtyHelper.MarkImmediate(unit, UnitNativeDirtyFlags.Remove);
        }

        // 清理 ECS 数据
        AttributeHelper.RemoveAllAttrs(unit);
        // 移除所有技能
        AbilitySlotHelper.RemoveAllAbilities(unit);
        // 删除单位 Entity
        unit.DeleteEntity();
    }

    #endregion

    #region 查询

    /// <summary>
    ///     检查单位是否存活
    /// </summary>
    public static bool IsAlive(Entity unit)
    {
        if (unit.IsNull) return false;
        if (unit.TryGetComponent<UnitState>(out var state))
            return state.isAlive;
        return false;
    }

    /// <summary>
    ///     检查两个单位是否敌对
    /// </summary>
    public static bool IsEnemy(Entity unit, Entity other)
    {
        // TODO: 实现基于玩家/队伍的敌对判断
        return true;
    }

    /// <summary>
    ///     检查两个单位是否友方
    /// </summary>
    public static bool IsAlly(Entity unit, Entity other)
    {
        return !IsEnemy(unit, other);
    }

    #endregion

    public static void KillUnit(Entity unit)
    {
        if (unit.IsNull) return;

        if (unit.TryGetComponent<UnitState>(out var state))
        {
            state.isAlive = false;
            state.lifePhase = UnitLifecyclePhase.Corpse;
            unit.AddComponent(state);
        }

        // 有原生单位时走立即死亡流程
        if (unit.TryGetComponent<UnitNative>(out var native))
        {
            UnitNativeDirtyHelper.MarkImmediate(unit, UnitNativeDirtyFlags.Death);

            unit.AddComponent(new TimerTask
            {
                mode = TimerTaskMode.Once,
                interval = 3f,
                remaining = 3f,
                paused = false,
                owner = unit,
                kind = TimerTaskKind.CorpseCleanup,
                triggerCount = 0,
                maxTriggerCount = 1
            });

            return;
        }

        // 清理 ECS 数据
        AttributeHelper.RemoveAllAttrs(unit);
        // 移除所有技能
        AbilitySlotHelper.RemoveAllAbilities(unit);
        // 删除单位 Entity
        unit.DeleteEntity();
    }
}
