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
    public static Entity CreateUnit(
        string templateName,
        JPlayer player,
        float x, float y,
        float facing = 270)
    {
        // 模板创建负责填充单位基础组件，这里只补充生命周期入口状态。
        var entity = UnitTemplate.Create(templateName, player, x, y, facing);
        entity.AddComponent(new UnitLifeState()
        {
            isAlive =  true,
            lifePhase = UnitLifecyclePhase.Alive,
        });

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

            // 直接移除跳过尸体停留，但仍只修改 ECS 生命周期，不在 helper 中调用 native remove。
            state.isAlive = false;
            state.lifePhase = UnitLifecyclePhase.Remove;
            unit.AddComponent(state);
        }

    }

    /// <summary>
    /// 让单位进入死亡流程。
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

            // 死亡入口只写生命周期和尸体清理计时器；原生表现由对应系统消费状态后执行。
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
        }
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

    #region 移动

    /// <summary>
    /// 让单位移动到指定位置，并在到达后交由上层任务流继续处理。
    /// </summary>
    public static void MoveToTask(Entity unit, float targetX, float targetY, float arrivalDistance, MoveReason reason = MoveReason.PlayerCommand)
    {
        if (unit.IsNull) return;

        var commandToken = War3Frame.Src.Systems.Unit.MoveSystem.NextCommandToken();

        // 移动任务写入规则层命令；Native 层负责真正下发 IssuePointOrder。
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
    }

    /// <summary>
    /// 执行原生移动命令
    /// </summary>
    public static void RequestMoveCommand(Entity unit, MoveOrderType orderType, float targetX, float targetY, int commandToken = 0)
    {
        if (!unit.HasComponent<UnitNative>())
        {
            return;
        }

        // 这是薄入口：只产生一次性 native 请求，不持有长期移动语义。
        unit.AddComponent(new MoveNativeCommandRequest
        {
            commandToken = commandToken,
            orderType = orderType,
            targetX = targetX,
            targetY = targetY
        });
    }

    #endregion

    /// <summary>
    /// 执行单位的 ECS 终态收尾。
    /// 包括移除计时标记、清空属性、清空技能并删除实体。
    /// </summary>
    public static void CleanupFinalizeEntityDispose(Entity entity)
    {
        // ECS 终态清理顺序：先移除外围状态，再删除属性/技能等子实体，最后删除单位本体。
        entity.RemoveTag<TimerExpired>();
        AttributeHelper.RemoveAllAttrs(entity);
        AbilitySlotHelper.RemoveAllAbilities(entity);
        entity.DeleteEntity();
    }
}
