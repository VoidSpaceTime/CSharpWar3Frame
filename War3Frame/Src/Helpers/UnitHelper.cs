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

    #region 生命周期推进

    /// <summary>
    /// 推进单位生命周期阶段。
    /// 根据当前阶段执行对应的副作用：Death → Corpse、ClearCorpse → Remove、Remove → Dispose
    /// </summary>
    public static void TransitionLifecycle(Entity entity)
    {
        if (!entity.TryGetComponent<UnitLifeState>(out var state))
        {
            return;
        }

        if (state.lifePhase == UnitLifecyclePhase.Death)
        {
            // Death → Corpse
            state.lifePhase = UnitLifecyclePhase.Corpse;
            entity.AddComponent(state);
        }
        else if (state.lifePhase == UnitLifecyclePhase.ClearCorpse)
        {
            // ClearCorpse → Remove
            state.lifePhase = UnitLifecyclePhase.Remove;
            entity.AddComponent(state);
        }
        else if (state.lifePhase == UnitLifecyclePhase.Remove)
        {
            // Remove → Dispose
            CleanupFinalizeEntityDispose(entity);
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
        entity.RemoveTag<TimerExpired>();
        AttributeHelper.RemoveAllAttrs(entity);
        AbilitySlotHelper.RemoveAllAbilities(entity);
        entity.DeleteEntity();
    }
}
