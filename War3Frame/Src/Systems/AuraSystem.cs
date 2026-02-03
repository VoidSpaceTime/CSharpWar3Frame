using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

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

            // 获取光环持有者
            if (!auraEntity.TryGetComponent<ModifierTarget>(out var ownerLink))
                return;

            var owner = ownerLink.target;
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

        // 获取当前受光环影响的单位
        var currentlyAffected = new HashSet<int>();
        var auraBufs = auraEntity.GetIncomingLinks<AuraBuffLink>();

        foreach (var link in auraBufs)
        {
            var buffEntity = link.Entity;
            if (buffEntity.TryGetComponent<ModifierTarget>(out var target))
            {
                currentlyAffected.Add(target.target.Id);
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
        foreach (var link in auraBufs)
        {
            var buffEntity = link.Entity;
            if (buffEntity.TryGetComponent<ModifierTarget>(out var target))
            {
                if (!unitsInRange.Contains(target.target.Id))
                {
                    buffEntity.DeleteEntity();
                    if (!target.target.IsNull)
                    {
                        target.target.AddTag<AttrsDirty>();
                    }
                }
            }
        }
    }

    private void AddAuraBuffToUnit(EntityStore store, Entity auraEntity, Entity unit, AuraEffect effect)
    {
        var buff = store.CreateEntity(
            new AttrModifier
            {
                attrType = effect.attrType,
                modifyType = effect.modifyType,
                value = effect.value,
                sourceType = ModifierSourceType.Aura
            },
            new ModifierTarget(unit),
            new AuraBuffLink(auraEntity)
        );

        unit.AddTag<AttrsDirty>();
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
        if (unit.TryGetComponent<UnitNative>(out var native))
        {
            return (JassApi.GetUnitX(native.unit), JassApi.GetUnitY(native.unit));
        }

        return (0, 0);
    }
}
