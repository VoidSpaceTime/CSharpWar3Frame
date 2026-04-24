using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame;

/// <summary>
///     光环辅助类
/// </summary>
public static class AuraHelper
{
    /// <summary>
    ///     为单位创建光环
    /// </summary>
    /// <param name="store">EntityStore</param>
    /// <param name="owner">光环持有者</param>
    /// <param name="auraId">光环ID</param>
    /// <param name="radius">影响范围</param>
    /// <param name="attrType">属性类型</param>
    /// <param name="modifyType">修改类型</param>
    /// <param name="value">修改值</param>
    /// <param name="affectSelf">是否影响自己</param>
    /// <param name="affectAllies">是否影响友军</param>
    /// <param name="affectEnemies">是否影响敌军</param>
    /// <param name="updateInterval">更新间隔（秒）</param>
    public static Entity CreateAura(
        EntityStore store,
        Entity owner,
        string auraId,
        float radius,
        int attrType,
        ModifyType modifyType,
        float value,
        bool affectSelf = false,
        bool affectAllies = true,
        bool affectEnemies = false,
        float updateInterval = 0.5f)
    {
        var aura = store.CreateEntity(
            new AuraConfig
            {
                auraId = auraId,
                radius = radius,
                updateInterval = updateInterval,
                timeSinceUpdate = updateInterval,  // 立即生效
                affectSelf = affectSelf,
                affectAllies = affectAllies,
                affectEnemies = affectEnemies
            },
            new AuraEffect
            {
                attrType = attrType,
                modifyType = modifyType,
                value = value
            },
            new ModifyTarget(owner)  // 光环挂载在持有者身上
        );

        aura.AddTag<Aura>();

        return aura;
    }

    /// <summary>
    ///     移除单位的指定光环
    /// </summary>
    public static void RemoveAura(Entity owner, string auraId)
    {
        var modifiers = owner.GetIncomingLinks<ModifyTarget>();

        foreach (var link in modifiers)
        {
            var entity = link.Entity;
            if (entity.Tags.Has<Aura>() &&
                entity.TryGetComponent<AuraConfig>(out var config) &&
                config.auraId == auraId)
            {
                // 先移除所有光环产生的 Buff
                RemoveAuraBuffs(entity);
                // 再删除光环本身
                entity.DeleteEntity();
                break;
            }
        }
    }

    /// <summary>
    ///     移除单位的所有光环
    /// </summary>
    public static void RemoveAllAuras(Entity owner)
    {
        var modifiers = owner.GetIncomingLinks<ModifyTarget>();
        var toDelete = new List<Entity>();

        foreach (var link in modifiers)
        {
            if (link.Entity.Tags.Has<Aura>())
            {
                toDelete.Add(link.Entity);
            }
        }

        foreach (var aura in toDelete)
        {
            RemoveAuraBuffs(aura);
            aura.DeleteEntity();
        }
    }

    /// <summary>
    ///     移除光环产生的所有 Buff
    /// </summary>
    private static void RemoveAuraBuffs(Entity aura)
    {
        var buffs = aura.GetIncomingLinks<AuraBuffLink>();
        var toDelete = new List<Entity>();
        var unitsToRefresh = new HashSet<Entity>();

        foreach (var link in buffs)
        {
            var buff = link.Entity;
            if (buff.TryGetComponent<ModifyTarget>(out var target))
            {
                unitsToRefresh.Add(target.target);
            }
            toDelete.Add(buff);
        }

        foreach (var buff in toDelete)
        {
            buff.DeleteEntity();
        }

        foreach (var unit in unitsToRefresh)
        {
            if (!unit.IsNull)
            {
                unit.AddTag<AttrDirty>();
            }
        }
    }

    /// <summary>
    ///     检查单位是否有指定光环
    /// </summary>
    public static bool HasAura(Entity owner, string auraId)
    {
        var modifiers = owner.GetIncomingLinks<ModifyTarget>();

        foreach (var link in modifiers)
        {
            if (link.Entity.Tags.Has<Aura>() &&
                link.Entity.TryGetComponent<AuraConfig>(out var config) &&
                config.auraId == auraId)
            {
                return true;
            }
        }

        return false;
    }
}
