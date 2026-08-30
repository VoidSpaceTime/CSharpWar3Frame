using Friflo.Engine.ECS;
using War3Frame.Components;

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
        int attrTypeId,
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

        // 获取对应的属性 Entity
        var attrEntity = AttributeHelper.GetAttr(unit, attrTypeId);
        if (attrEntity == null) return default;

        // 创建新 Buff
        var buff = store.CreateEntity(
            new ModifyValue
            {
                modifyType = modifyType,
                value = value,
                priority = 0
            },
            new ModifyTarget(attrEntity.Value),
            new ModifySource(source),
            BuffDuration.Create(duration),
            Duration.Create(duration),
            new BuffBehavior
            {
                buffId = buffId,
                refreshBehavior = refreshBehavior,
                removeAllStacksOnExpire = true
            }
        );

        buff.AddTag<Buff>();
        attrEntity.Value.AddTag<AttrDirty>();

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
        int attrTypeId,
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

        // 获取对应的属性 Entity
        var attrEntity = AttributeHelper.GetAttr(unit, attrTypeId);
        if (attrEntity == null) return default;

        // 创建新 Buff
        var buff = store.CreateEntity(
            new ModifyValue
            {
                modifyType = modifyType,
                value = valuePerStack,  // 初始值 = 1层
                priority = 0
            },
            new ModifyTarget(attrEntity.Value),
            new ModifySource(source),
            BuffDuration.Create(duration),
            Duration.Create(duration),
            BuffStacks.Create(maxStacks, valuePerStack),
            new BuffBehavior
            {
                buffId = buffId,
                refreshBehavior = refreshBehavior,
                removeAllStacksOnExpire = true
            }
        );

        buff.AddTag<Buff>();
        attrEntity.Value.AddTag<AttrDirty>();

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
        int attrTypeId,
        ModifyType modifyType,
        float value)
    {
        // 获取对应的属性 Entity
        var attrEntity = AttributeHelper.GetAttr(unit, attrTypeId);
        if (attrEntity == null) return default;

        var buff = store.CreateEntity(
            new ModifyValue
            {
                modifyType = modifyType,
                value = value,
                priority = 0
            },
            new ModifyTarget(attrEntity.Value),
            new ModifySource(source),
            BuffDuration.Create(0),
            Duration.Create(-1),
            new BuffBehavior
            {
                buffId = buffId,
                refreshBehavior = BuffRefreshBehavior.Independent
            }
        );

        buff.AddTag<Buff>();
        attrEntity.Value.AddTag<AttrDirty>();

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
            // 标记属性需要刷新
            if (buff.TryGetComponent<ModifyTarget>(out var target) && !target.target.IsNull)
            {
                target.target.AddTag<AttrDirty>();
            }
            buff.DeleteEntity();
        }
    }

    /// <summary>
    ///     移除单位的所有 Buff
    /// </summary>
    public static void RemoveAllBuffs(Entity unit)
    {
        // 获取单位的所有属性
        var attrs = AttributeHelper.GetAllAttrs(unit);
        var affectedAttrs = new HashSet<Entity>();
        var toDelete = new List<Entity>();

        foreach (var (typeId, attrEntity) in attrs)
        {
            // 获取指向该属性的所有修改器
            var modifiers = attrEntity.GetIncomingLinks<ModifyTarget>();
            foreach (var link in modifiers)
            {
                if (link.Entity.Tags.Has<Buff>())
                {
                    toDelete.Add(link.Entity);
                    affectedAttrs.Add(attrEntity);
                }
            }
        }

        foreach (var buff in toDelete)
        {
            buff.DeleteEntity();
        }

        foreach (var attr in affectedAttrs)
        {
            if (!attr.IsNull)
            {
                attr.AddTag<AttrDirty>();
            }
        }
    }

    #endregion

    #region 查询

    /// <summary>
    ///     查找单位身上的指定 Buff
    /// </summary>
    public static Entity FindBuffByIdOnUnit(Entity unit, string buffId)
    {
        // 遍历单位的所有属性，查找 Buff
        var attrs = AttributeHelper.GetAllAttrs(unit);

        foreach (var (typeId, attrEntity) in attrs)
        {
            var modifiers = attrEntity.GetIncomingLinks<ModifyTarget>();
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

        if (buff.TryGetComponent<Duration>(out var duration))
        {
            return duration.remaining < 0f ? -1f : duration.remaining;
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
                if (existing.TryGetComponent<Duration>(out var dur))
                {
                    dur.remaining = existing.GetComponent<BuffDuration>().duration;
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
                        if (existing.TryGetComponent<ModifyValue>(out var mod))
                        {
                            mod.value = stacks.TotalValue;
                            existing.AddComponent(mod);
                        }
                        needsRefresh = true;
                    }
                }
                break;

            case BuffRefreshBehavior.RefreshDuration:
                if (existing.TryGetComponent<Duration>(out var dur))
                {
                    dur.remaining = existing.GetComponent<BuffDuration>().duration;
                    existing.AddComponent(dur);
                }
                break;

            case BuffRefreshBehavior.RefreshAndStack:
                // 刷新时间
                if (existing.TryGetComponent<Duration>(out var duration2))
                {
                    duration2.remaining = existing.GetComponent<BuffDuration>().duration;
                    existing.AddComponent(duration2);
                }
                // 叠加层数
                if (existing.TryGetComponent<BuffStacks>(out var stacks2))
                {
                    if (stacks2.AddStack())
                    {
                        existing.AddComponent(stacks2);

                        if (existing.TryGetComponent<ModifyValue>(out var mod2))
                        {
                            mod2.value = stacks2.TotalValue;
                            existing.AddComponent(mod2);
                        }
                        needsRefresh = true;
                    }
                }
                break;
        }

        if (needsRefresh && existing.TryGetComponent<ModifyTarget>(out var target))
        {
            target.target.AddTag<AttrDirty>();
        }

        return existing;
    }

    #endregion
}
