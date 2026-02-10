using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 技能效果辅助类 - 负责从技能模板创建效果 Entity
/// 这是 ExecuteAbility 的核心逻辑
/// </summary>
public static class AbilityEffectHelper
{
    /// <summary>
    /// 从技能创建效果 Entity
    /// 将技能上的效果组件复制到新的效果事件 Entity 上
    /// </summary>
    /// <param name="caster">施法者</param>
    /// <param name="ability">技能 Entity</param>
    /// <param name="targetUnit">目标单位</param>
    /// <param name="targetX">目标点 X</param>
    /// <param name="targetY">目标点 Y</param>
    /// <returns>创建的效果 Entity</returns>
    public static Entity CreateEffectEntity(Entity caster, Entity ability,
        Entity targetUnit, float targetX, float targetY)
    {
        // 1. 创建效果 Entity，携带基础来源和目标信息
        var effectEntity = Game.Store.CreateEntity(
            new EffectSource { caster = caster, ability = ability },
            new EffectTargetInfo
            {
                targetUnit = targetUnit,
                targetX = targetX,
                targetY = targetY
            }
        );
        effectEntity.AddTag<EffectPending>();

        // 2. 从技能模板复制效果组件
        //    每种效果组件由对应的 System 处理

        // 伤害效果
        if (ability.TryGetComponent<DamageEffectData>(out var dmg))
            effectEntity.AddComponent(dmg);

        // 治疗效果
        if (ability.TryGetComponent<HealEffectData>(out var heal))
            effectEntity.AddComponent(heal);

        // Buff 施加效果
        if (ability.TryGetComponent<ApplyBuffData>(out var buff))
            effectEntity.AddComponent(buff);

        // 范围搜索效果（AOE）
        if (ability.TryGetComponent<AreaSearchData>(out var area))
        {
            // 如果未指定中心点，使用目标坐标
            if (area.centerX == 0 && area.centerY == 0)
            {
                area.centerX = targetX;
                area.centerY = targetY;
            }
            effectEntity.AddComponent(area);
        }

        // 弹道效果
        if (ability.TryGetComponent<ProjectileData>(out var proj))
        {
            // 设置弹道到达阈值默认值
            if (proj.arrivalThreshold <= 0)
                proj.arrivalThreshold = 30f;

            effectEntity.AddComponent(proj);

            // 弹道需要位置组件来跟踪飞行
            if (caster.TryGetComponent<Position>(out var casterPos))
            {
                effectEntity.AddComponent(new Position
                {
                    x = casterPos.x,
                    y = casterPos.y
                });
            }
        }

        return effectEntity;
    }

    /// <summary>
    /// 为范围搜索到的每个目标创建子效果 Entity
    /// 子效果继承父效果的伤害/治疗/Buff 组件，但目标改为单个单位
    /// </summary>
    /// <param name="parentEffect">父效果 Entity（包含 AreaSearch）</param>
    /// <param name="target">搜索到的目标单位</param>
    /// <returns>子效果 Entity</returns>
    public static Entity CreateChildEffect(Entity parentEffect, Entity target)
    {
        var source = parentEffect.GetComponent<EffectSource>();

        // 创建子效果 Entity，目标改为具体单位
        var childEntity = Game.Store.CreateEntity(
            source,
            new EffectTargetInfo
            {
                targetUnit = target,
                targetX = 0,
                targetY = 0
            }
        );
        childEntity.AddTag<EffectPending>();

        // 复制伤害效果到子效果
        if (parentEffect.TryGetComponent<DamageEffectData>(out var dmg))
            childEntity.AddComponent(dmg);

        // 复制治疗效果到子效果
        if (parentEffect.TryGetComponent<HealEffectData>(out var heal))
            childEntity.AddComponent(heal);

        // 复制 Buff 效果到子效果
        if (parentEffect.TryGetComponent<ApplyBuffData>(out var buff))
            childEntity.AddComponent(buff);

        // 注意：不复制 AreaSearch 和 Projectile，子效果是直接生效的

        return childEntity;
    }
}
