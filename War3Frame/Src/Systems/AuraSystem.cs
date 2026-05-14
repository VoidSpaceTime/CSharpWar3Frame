using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;

namespace War3Frame.Src.Systems;

/// <summary>
///     光环系统 - 处理光环效果的施加和移除
/// </summary>
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
            // 更新计时器
            config.timeSinceUpdate += Tick.deltaTime;

            if (config.timeSinceUpdate < config.updateInterval)
                return;

            config.timeSinceUpdate = 0;

            // 获取光环持有者（通过 AuraOwner 组件）
            if (!auraEntity.TryGetComponent<AuraOwner>(out var ownerLink))
                return;

            var owner = ownerLink.owner;
            if (owner.IsNull) return;

            // 获取持有者位置
            var ownerPos = GetUnitPosition(owner);

            // 复制值用于 lambda
            var configCopy = config;
            var effectCopy = effect;

            // 查找范围内的单位并施加效果
            UpdateAuraEffects(auraEntity, owner, ownerPos, configCopy, effectCopy);
        });
    }

    private void UpdateAuraEffects(Entity auraEntity, Entity owner, (float x, float y) ownerPos,
                                   AuraConfig config, AuraEffect effect)
    {
        var store = CommandBuffer.EntityStore;
        var radiusSq = config.radius * config.radius;

        // 获取当前受光环影响的单位（属性 Entity）
        var currentlyAffected = new HashSet<int>();
        var auraBufs = auraEntity.GetIncomingLinks<AuraBuffLink>();

        foreach (var link in auraBufs)
        {
            var buffEntity = link.Entity;
            if (buffEntity.TryGetComponent<ModifyTarget>(out var target))
            {
                // 从属性 Entity 获取其所有者单位
                var attrOwnerLinks = target.target.GetIncomingLinks<AttrOwner>();
                foreach (var ownerLink in attrOwnerLinks)
                {
                    currentlyAffected.Add(ownerLink.Entity.Id);
                    break;
                }
            }
        }

        // 查询所有单位
        var unitsInRange = new HashSet<int>();
        var query = store.Query<UnitNative>();

        query.ForEachEntity((ref UnitNative unit, Entity unitEntity) =>
        {
            // 检查是否在范围内
            var unitPos = GetUnitPosition(unitEntity);
            var distSq = (unitPos.x - ownerPos.x) * (unitPos.x - ownerPos.x) +
                         (unitPos.y - ownerPos.y) * (unitPos.y - ownerPos.y);

            if (distSq > radiusSq) return;

            // 检查是否应该影响（自己/友军/敌军）
            if (!ShouldAffectUnit(owner, unitEntity, config)) return;

            unitsInRange.Add(unitEntity.Id);

            // 如果还没有光环效果，添加
            if (!currentlyAffected.Contains(unitEntity.Id))
            {
                AddAuraBuffToUnit(store, auraEntity, unitEntity, effect);
            }
        });

        // 移除离开范围的单位的光环效果
        var toDelete = new List<Entity>();
        foreach (var link in auraBufs)
        {
            var buffEntity = link.Entity;
            if (buffEntity.TryGetComponent<ModifyTarget>(out var target))
            {
                // 获取属性的所有者单位
                if (target.target.TryGetComponent<AttrOwner>(out var attrOwner))
                {
                    if (!unitsInRange.Contains(attrOwner.owner.Id))
                    {
                        toDelete.Add(buffEntity);
                        if (!target.target.IsNull)
                        {
                            target.target.AddTag<AttrDirty>();
                        }
                    }
                }
            }
        }

        foreach (var buff in toDelete)
        {
            buff.DeleteEntity();
        }
    }

    private void AddAuraBuffToUnit(EntityStore store, Entity auraEntity, Entity unit, AuraEffect effect)
    {
        // 获取对应的属性 Entity
        var attrEntity = AttributeHelper.GetAttr(unit, effect.attrType);
        if (attrEntity == null) return;

        var buff = store.CreateEntity(
            new ModifyValue
            {
                modifyType = effect.modifyType,
                value = effect.value,
                priority = 0
            },
            new ModifyTarget(attrEntity.Value),
            new AuraBuffLink(auraEntity)
        );

        attrEntity.Value.AddTag<AttrDirty>();
    }

    private bool ShouldAffectUnit(Entity owner, Entity target, AuraConfig config)
    {
        if (owner.Id == target.Id)
            return config.affectSelf;

        // TODO: 实现友军/敌军判断
        // 这需要根据你的玩家/队伍系统来实现
        // if (IsAlly(owner, target)) return config.affectAllies;
        // if (IsEnemy(owner, target)) return config.affectEnemies;

        return config.affectAllies;  // 默认影响所有其他单位
    }

    private (float x, float y) GetUnitPosition(Entity unit)
    {
        if (unit.TryGetComponent<Position>(out var position))
        {
            return (position.x, position.y);
        }

        return (0, 0);
    }
}

/// <summary>
/// 光环持有者关系
/// </summary>
public struct AuraOwner : ILinkComponent
{
    public Entity GetIndexedValue() => owner;
    public Entity owner;

    public AuraOwner(Entity owner) => this.owner = owner;
}
