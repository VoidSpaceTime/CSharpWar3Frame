using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

[SystemRegister(SystemKind.Interval, 42)]
public class AuraSystem : QuerySystem<AuraConfig, AuraEffect>, ITimedSystem
{
    public float Interval => 0.1f;

    public AuraSystem()
    {
        Filter.AnyTags(Tags.Get<Aura>());
    }

    protected override void OnUpdate()
    {
        // Aura 更新会产生结构变更（创建/删除 buff、打脏），不能在 Query 迭代内执行：
        // 先收集本轮到期的光环，循环外再更新。
        var dueAuras = new List<(Entity aura, Entity owner, (float x, float y) ownerPos, AuraConfig config, AuraEffect effect)>();
        var orphaned = new List<Entity>();

        Query.ForEachEntity((ref AuraConfig config, ref AuraEffect effect, Entity auraEntity) =>
        {
            config.timeSinceUpdate += Tick.deltaTime;
            if (config.timeSinceUpdate < config.updateInterval)
                return;

            config.timeSinceUpdate = 0;

            if (!auraEntity.TryGetComponent<AuraOwner>(out var ownerLink) || ownerLink.owner.IsNull)
            {
                orphaned.Add(auraEntity);
                return;
            }

            var owner = ownerLink.owner;
            dueAuras.Add((auraEntity, owner, GetUnitPosition(owner), config, effect));
        });

        foreach (var aura in orphaned)
        {
            AuraHelper.RemoveAuraBuffs(aura);
            aura.DeleteEntity();
        }

        foreach (var due in dueAuras)
        {
            if (!due.aura.IsNull && !due.owner.IsNull)
                UpdateAuraEffects(due.aura, due.owner, due.ownerPos, due.config, due.effect);
        }
    }

    private void UpdateAuraEffects(Entity auraEntity, Entity owner, (float x, float y) ownerPos,
        AuraConfig config, AuraEffect effect)
    {
        var store = CommandBuffer.EntityStore;
        var radiusSq = config.radius * config.radius;

        var currentlyAffected = new HashSet<int>();
        var auraBufs = auraEntity.GetIncomingLinks<AuraBuffLink>();

        foreach (var link in auraBufs)
        {
            var buffEntity = link.Entity;
            if (buffEntity.TryGetComponent<ModifyTarget>(out var target) && !target.target.IsNull)
            {
                if (target.target.TryGetComponent<AttrOwner>(out var attrOwner) && !attrOwner.owner.IsNull)
                {
                    currentlyAffected.Add(attrOwner.owner.Id);
                }
            }
        }

        var unitsInRange = new HashSet<int>();
        var unitsToAdd = new List<Entity>();
        var query = store.Query<Position>();

        // 嵌套查询只收集"需要新增 buff"的单位，结构变更留到循环外执行。
        query.ForEachEntity((ref Position position, Entity unitEntity) =>
        {
            if (!unitEntity.HasComponent<UnitNative>() && !unitEntity.HasComponent<UnitBase>()
                && !unitEntity.HasComponent<UnitLifeState>()) return;
            var unitPos = GetUnitPosition(unitEntity);
            var distSq = (unitPos.x - ownerPos.x) * (unitPos.x - ownerPos.x) +
                         (unitPos.y - ownerPos.y) * (unitPos.y - ownerPos.y);

            if (distSq > radiusSq)
                return;

            if (!ShouldAffectUnit(owner, unitEntity, config))
                return;

            unitsInRange.Add(unitEntity.Id);

            if (!currentlyAffected.Contains(unitEntity.Id))
            {
                unitsToAdd.Add(unitEntity);
            }
        });

        foreach (var unitEntity in unitsToAdd)
        {
            if (!unitEntity.IsNull)
                AddAuraBuffToUnit(store, auraEntity, unitEntity, effect, config);
        }

        var toDelete = new List<Entity>();
        foreach (var link in auraBufs)
        {
            var buffEntity = link.Entity;
            if (!buffEntity.TryGetComponent<ModifyTarget>(out var target) ||
                target.target.IsNull ||
                !target.target.TryGetComponent<AttrOwner>(out var attrOwner) ||
                attrOwner.owner.IsNull || !unitsInRange.Contains(attrOwner.owner.Id))
            {
                toDelete.Add(buffEntity);
                if (!target.target.IsNull)
                {
                    target.target.AddTag<AttrDirty>();
                }
            }
        }

        foreach (var buff in toDelete)
        {
            if (!buff.IsNull)
                buff.DeleteEntity();
        }
    }

    private void AddAuraBuffToUnit(EntityStore store, Entity auraEntity, Entity unit, AuraEffect effect, AuraConfig config)
    {
        // 来源唯一性由 AuraBuffLink 与当前目标集合管理，不与其他同名光环共享 Buff 实体。
        var buff = BuffHelper.CreateBuffInternal(store, unit, auraEntity,
            new BuffSpec($"aura:{config.auraId}", null, effect.attrType, effect.modifyType,
                effect.value, -1f, 1, BuffRefreshBehavior.Independent, 0, null, BuffTag.None));

        if (!buff.IsNull)
        {
            buff.AddComponent(new AuraBuffLink(auraEntity));
        }
    }

    private bool ShouldAffectUnit(Entity owner, Entity target, AuraConfig config)
    {
        if (owner == target)
            return config.affectSelf;

        if (!UnitHelper.TryGetRelation(owner, target, out var relation)) return false;
        return relation == PlayerTeamState.Allie && config.affectAllies
            || relation == PlayerTeamState.Enemy && config.affectEnemies;
    }

    private (float x, float y) GetUnitPosition(Entity unit)
    {
        if (unit.TryGetComponent<Position>(out var position))
            return (position.x, position.y);

        return (0, 0);
    }
}

public struct AuraOwner : ILinkComponent
{
    public Entity GetIndexedValue() => owner;
    public Entity owner;

    public AuraOwner(Entity owner) => this.owner = owner;
}
