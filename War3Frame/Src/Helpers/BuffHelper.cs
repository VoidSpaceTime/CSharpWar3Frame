using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
///     Buff 辅助类 - 提供便捷的 Buff 操作
/// </summary>
public static class BuffHelper
{
    #region 添加 Buff

    /// <summary>
    ///     为单位添加一个简单的限时 Buff
    /// </summary>
    public static Entity AddTimedBuff(
        EntityStore store,
        Entity unit,
        Entity source,
        string buffId,
        AttrType attrType,
        ModifyType modifyType,
        float value,
        float duration,
        BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshDuration)
    {
        // 检查是否已有同类 Buff
        var existing = FindBuffByIdOnUnit(unit, buffId);

        if (!existing.IsNull)
        {
            return HandleExistingBuff(existing, value, duration, refreshBehavior);
        }

        // 创建新 Buff
        var buff = store.CreateEntity(
            new AttrModifier
            {
                attrType = attrType,
                modifyType = modifyType,
                value = value,
                sourceType = ModifierSourceType.Buff
            },
            new ModifierTarget(unit),
            new ModifierSource(source),
            BuffDuration.Create(duration),
            new BuffBehavior
            {
                buffId = buffId,
                refreshBehavior = refreshBehavior,
                removeAllStacksOnExpire = true
            }
        );

        buff.AddTag<Buff>();
        unit.AddTag<AttrsDirty>();

        return buff;
    }

    /// <summary>
    ///     添加可堆叠的 Buff
    /// </summary>
    public static Entity AddStackableBuff(
        EntityStore store,
        Entity unit,
        Entity source,
        string buffId,
        AttrType attrType,
        ModifyType modifyType,
        float valuePerStack,
        int maxStacks,
        float duration,
        BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshAndStack)
    {
        // 检查是否已有同类 Buff
        var existing = FindBuffByIdOnUnit(unit, buffId);

        if (!existing.IsNull)
        {
            return HandleExistingStackableBuff(existing, valuePerStack, duration, refreshBehavior);
        }

        // 创建新 Buff
        var buff = store.CreateEntity(
            new AttrModifier
            {
                attrType = attrType,
                modifyType = modifyType,
                value = valuePerStack,  // 初始值 = 1层
                sourceType = ModifierSourceType.Buff
            },
            new ModifierTarget(unit),
            new ModifierSource(source),
            BuffDuration.Create(duration),
            BuffStacks.Create(maxStacks, valuePerStack),
            new BuffBehavior
            {
                buffId = buffId,
                refreshBehavior = refreshBehavior,
                removeAllStacksOnExpire = true
            }
        );

        buff.AddTag<Buff>();
        unit.AddTag<AttrsDirty>();

        return buff;
    }

    /// <summary>
    ///     添加永久 Buff（不会自动消失）
    /// </summary>
    public static Entity AddPermanentBuff(
        EntityStore store,
        Entity unit,
        Entity source,
        string buffId,
        AttrType attrType,
        ModifyType modifyType,
        float value)
    {
        var buff = store.CreateEntity(
            new AttrModifier
            {
                attrType = attrType,
                modifyType = modifyType,
                value = value,
                sourceType = ModifierSourceType.Buff
            },
            new ModifierTarget(unit),
            new ModifierSource(source),
            BuffDuration.Create(0, permanent: true),
            new BuffBehavior
            {
                buffId = buffId,
                refreshBehavior = BuffRefreshBehavior.Independent
            }
        );

        buff.AddTag<Buff>();
        unit.AddTag<AttrsDirty>();

        return buff;
    }

    #endregion

    #region 移除 Buff

    /// <summary>
    ///     移除单位身上的指定 Buff
    /// </summary>
    public static void RemoveBuff(Entity unit, string buffId)
    {
        var buff = FindBuffByIdOnUnit(unit, buffId);
        if (!buff.IsNull)
        {
            buff.DeleteEntity();
            unit.AddTag<AttrsDirty>();
        }
    }

    /// <summary>
    ///     移除单位的所有 Buff
    /// </summary>
    public static void RemoveAllBuffs(Entity unit)
    {
        var modifiers = unit.GetIncomingLinks<ModifierTarget>();
        var toDelete = new List<Entity>();

        foreach (var link in modifiers)
        {
            if (link.Entity.Tags.Has<Buff>())
            {
                toDelete.Add(link.Entity);
            }
        }

        foreach (var buff in toDelete)
        {
            buff.DeleteEntity();
        }

        if (toDelete.Count > 0 && !unit.IsNull)
        {
            unit.AddTag<AttrsDirty>();
        }
    }

    #endregion

    #region 查询

    /// <summary>
    ///     查找单位身上的指定 Buff
    /// </summary>
    public static Entity FindBuffByIdOnUnit(Entity unit, string buffId)
    {
        var modifiers = unit.GetIncomingLinks<ModifierTarget>();

        foreach (var link in modifiers)
        {
            var buffEntity = link.Entity;
            if (buffEntity.Tags.Has<Buff>() &&
                buffEntity.TryGetComponent<BuffBehavior>(out var behavior) &&
                behavior.buffId == buffId)
            {
                return buffEntity;
            }
        }

        return default;
    }

    /// <summary>
    ///     检查单位是否有指定 Buff
    /// </summary>
    public static bool HasBuff(Entity unit, string buffId)
    {
        return !FindBuffByIdOnUnit(unit, buffId).IsNull;
    }

    /// <summary>
    ///     获取 Buff 剩余时间
    /// </summary>
    public static float GetBuffRemainingTime(Entity unit, string buffId)
    {
        var buff = FindBuffByIdOnUnit(unit, buffId);
        if (buff.IsNull) return 0;

        if (buff.TryGetComponent<BuffDuration>(out var duration))
        {
            return duration.remaining;
        }
        return 0;
    }

    /// <summary>
    ///     获取 Buff 当前层数
    /// </summary>
    public static int GetBuffStacks(Entity unit, string buffId)
    {
        var buff = FindBuffByIdOnUnit(unit, buffId);
        if (buff.IsNull) return 0;

        if (buff.TryGetComponent<BuffStacks>(out var stacks))
        {
            return stacks.current;
        }
        return 1;  // 没有层数组件视为 1 层
    }

    #endregion

    #region 内部方法

    private static Entity HandleExistingBuff(Entity existing, float value, float duration, BuffRefreshBehavior behavior)
    {
        switch (behavior)
        {
            case BuffRefreshBehavior.RefreshDuration:
                if (existing.TryGetComponent<BuffDuration>(out var dur))
                {
                    dur.Refresh();
                    existing.AddComponent(dur);
                }
                break;

            case BuffRefreshBehavior.Independent:
                // 不做任何事
                break;
        }

        return existing;
    }

    private static Entity HandleExistingStackableBuff(Entity existing, float valuePerStack, float duration, BuffRefreshBehavior behavior)
    {
        var needsRefresh = false;

        switch (behavior)
        {
            case BuffRefreshBehavior.AddStack:
                if (existing.TryGetComponent<BuffStacks>(out var stacks))
                {
                    if (stacks.AddStack())
                    {
                        existing.AddComponent(stacks);

                        // 更新修改器的值
                        if (existing.TryGetComponent<AttrModifier>(out var mod))
                        {
                            mod.value = stacks.TotalValue;
                            existing.AddComponent(mod);
                        }
                        needsRefresh = true;
                    }
                }
                break;

            case BuffRefreshBehavior.RefreshDuration:
                if (existing.TryGetComponent<BuffDuration>(out var dur))
                {
                    dur.Refresh();
                    existing.AddComponent(dur);
                }
                break;

            case BuffRefreshBehavior.RefreshAndStack:
                // 刷新时间
                if (existing.TryGetComponent<BuffDuration>(out var duration2))
                {
                    duration2.Refresh();
                    existing.AddComponent(duration2);
                }
                // 叠加层数
                if (existing.TryGetComponent<BuffStacks>(out var stacks2))
                {
                    if (stacks2.AddStack())
                    {
                        existing.AddComponent(stacks2);

                        if (existing.TryGetComponent<AttrModifier>(out var mod2))
                        {
                            mod2.value = stacks2.TotalValue;
                            existing.AddComponent(mod2);
                        }
                        needsRefresh = true;
                    }
                }
                break;
        }

        if (needsRefresh && existing.TryGetComponent<ModifierTarget>(out var target))
        {
            target.target.AddTag<AttrsDirty>();
        }

        return existing;
    }

    #endregion
}
