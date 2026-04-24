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
        entity.AddComponent(new UnitLifeState()
        {
            isAlive =  true,
            lifePhase = UnitLifecyclePhase.Alive,
        });
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

        if (unit.TryGetComponent<UnitLifeState>(out var state))
        {
            if (state.lifePhase != UnitLifecyclePhase.Alive && state.lifePhase != UnitLifecyclePhase.Corpse)
            {
                return;
            }

            state.isAlive = false;
            state.lifePhase = UnitLifecyclePhase.Remove;
            unit.AddComponent(state);
        }

        // 生命周期仅写入意图并立即触发执行层
        Game.FlushImmediateSystems();
    }

    #endregion

    #region 查询

    /// <summary>
    ///     检查单位是否存活
    /// </summary>
    public static bool IsAlive(Entity unit)
    {
        if (unit.IsNull) return false;
        if (unit.TryGetComponent<UnitLifeState>(out var state))
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

    /// <summary>
    /// 让单位进入死亡流程。
    /// 会写入生命周期阶段、挂接尸体清理计时器，并立即刷新即时系统。
    /// </summary>
    public static void KillUnit(Entity unit)
    {
        if (unit.IsNull) return;

        if (unit.TryGetComponent<UnitLifeState>(out var state))
        {
            if (state.lifePhase != UnitLifecyclePhase.Alive)
            {
                return;
            }

            state.isAlive = false;
            state.lifePhase = UnitLifecyclePhase.Death;
            unit.AddComponent(state);
            unit.AddComponent(new TimerTask
            {
                mode = TimerTaskMode.Once,
                interval = Game.DefaultCorpseDuration,
                remaining = Game.DefaultCorpseDuration,
                paused = false,
                owner = unit,
                kind = TimerTaskKind.CorpseCleanup,
                triggerCount = 0,
                maxTriggerCount = 1
            });
            Game.FlushImmediateSystems();
        }
    }

    /// <summary>
    /// 让单位移动到指定位置，并在到达后交由上层任务流继续处理。
    /// helper 只写入 move 意图与 continuation，不拥有任务执行语义。
    /// </summary>
    public static void MoveToTask(Entity unit, float targetX, float targetY, float arrivalDistance, MoveReason reason = MoveReason.PlayerCommand)
    {
        if (unit.IsNull) return;

        var commandToken = War3Frame.Src.Systems.Unit.MoveSystem.NextCommandToken();

        unit.AddComponent(new MoveCommand
        {
            targetX = targetX,
            targetY = targetY,
            arrivalDistance = arrivalDistance,
            reason = reason,
            orderType = MoveOrderType.Move,
            commandToken = commandToken,
            issued = false
        });

        unit.AddComponent(new MoveContinuation
        {
            kind = MoveContinuationKind.ExecuteTask,
            ability = default,
            targetUnit = default,
            targetX = targetX,
            targetY = targetY
        });

        Game.FlushImmediateSystems();
    }

    /// <summary>
    /// 执行单位的 ECS 终态收尾。
    /// 包括移除计时标记、清空属性、清空技能并删除实体。
    /// </summary>
    public static void CleanupFinalizeEntityDispose(Entity entity)
    {
        entity.RemoveTag<TimerExpired>();
        AttributeHelper.RemoveAllAttrs(entity);
        AbilitySlotHelper.RemoveAllAbilities(entity);
        entity.DeleteEntity();
    }
}
