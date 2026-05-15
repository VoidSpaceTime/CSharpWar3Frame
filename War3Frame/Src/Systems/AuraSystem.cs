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
        Query.ForEachEntity((ref AuraConfig config, ref AuraEffect effect, Entity auraEntity) =>
        {
            config.timeSinceUpdate += Tick.deltaTime;
            if (config.timeSinceUpdate < config.updateInterval)
                return;

            config.timeSinceUpdate = 0;

            if (!auraEntity.TryGetComponent<AuraOwner>(out var ownerLink))
                return;

            var owner = ownerLink.owner;
            if (owner.IsNull)
                return;

            var ownerPos = GetUnitPosition(owner);
            var configCopy = config;
            var effectCopy = effect;
            UpdateAuraEffects(auraEntity, owner, ownerPos, configCopy, effectCopy);
        });
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
                var attrOwnerLinks = target.target.GetIncomingLinks<AttrOwner>();
                foreach (var ownerLink in attrOwnerLinks)
                {
                    currentlyAffected.Add(ownerLink.Entity.Id);
                    break;
                }
            }
        }

        var unitsInRange = new HashSet<int>();
        var query = store.Query<UnitNative>();

        query.ForEachEntity((ref UnitNative unit, Entity unitEntity) =>
        {
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
                AddAuraBuffToUnit(store, auraEntity, unitEntity, effect, config);
            }
        });

        var toDelete = new List<Entity>();
        foreach (var link in auraBufs)
        {
            var buffEntity = link.Entity;
            if (buffEntity.TryGetComponent<ModifyTarget>(out var target) &&
                target.target.TryGetComponent<AttrOwner>(out var attrOwner) &&
                !unitsInRange.Contains(attrOwner.owner.Id))
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
            buff.DeleteEntity();
        }
    }

    private void AddAuraBuffToUnit(EntityStore store, Entity auraEntity, Entity unit, AuraEffect effect, AuraConfig config)
    {
        var buff = BuffHelper.AddPermanentBuff(
            store,
            unit,
            auraEntity,
            $"aura:{config.auraId}",
            effect.attrType,
            effect.modifyType,
            effect.value);

        if (!buff.IsNull)
        {
            buff.AddComponent(new AuraBuffLink(auraEntity));
        }
    }

    private bool ShouldAffectUnit(Entity owner, Entity target, AuraConfig config)
    {
        if (owner.Id == target.Id)
            return config.affectSelf;

        return config.affectAllies;
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
