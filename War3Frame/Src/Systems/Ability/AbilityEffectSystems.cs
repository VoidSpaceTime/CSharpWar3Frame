using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Src.Systems;

namespace War3Frame;

// ============================================================================
// 技能效果处理系统
// 按照处理顺序排列：范围搜索 → 弹道 → 伤害 → 治疗 → Buff 施加
// ============================================================================

/// <summary>
/// 范围搜索系统 - 处理 AOE 效果
/// 在指定区域内搜索目标，为每个目标创建子效果 Entity
/// 处理优先级最高，因为需要先找到目标才能应用其他效果
/// </summary>
public class AreaSearchSystem : QuerySystem<AreaSearchData, EffectSource, EffectTargetInfo>
{
    public AreaSearchSystem()
    {
        // 只处理待处理的效果
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref AreaSearchData area, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            // 如果有弹道组件，先等弹道到达再搜索
            if (effectEntity.HasComponent<ProjectileData>() && !effectEntity.Tags.Has<ProjectileArrived>())
                return;

            // 搜索范围内的目标
            var targets = FindTargetsInArea(
                source.caster, area.centerX, area.centerY,
                area.radius, area.filter, area.customFilterId, area.maxTargets);

            // 为每个目标创建子效果 Entity
            foreach (var targetUnit in targets)
            {
                AbilityEffectHelper.CreateChildEffect(effectEntity, targetUnit);
            }

            // 范围搜索效果 Entity 处理完毕，标记删除
            toDelete.Add(effectEntity);
        });

        // 删除已处理的 AOE 效果 Entity
        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }
    }

    /// <summary>
    /// 在指定区域搜索目标
    /// TODO: 根据你的碰撞/空间查询系统实现遍历逻辑
    /// </summary>
    private List<Entity> FindTargetsInArea(Entity caster, float x, float y,
        float radius, TargetFilter filter, string? customFilterId, int maxTargets)
    {
        var results = new List<Entity>();
        float radiusSq = radius * radius;

        // TODO: 替换为你的空间查询或单位遍历逻辑
        // 示例：遍历所有带 Position 的单位
        // foreach (var (pos, unit) in allUnits)
        // {
        //     float dx = pos.x - x;
        //     float dy = pos.y - y;
        //     if (dx * dx + dy * dy > radiusSq) continue;
        //
        //     // 使用 TargetFilterRegistry 进行综合筛选
        //     if (!TargetFilterRegistry.PassFilter(filter, customFilterId, caster, unit))
        //         continue;
        //
        //     results.Add(unit);
        //     if (maxTargets > 0 && results.Count >= maxTargets) break;
        // }

        return results;
    }
}

/// <summary>
/// 弹道系统 - 处理弹道飞行
/// 弹道到达目标后，添加 ProjectileArrived 标记
/// 其他效果系统（伤害/治疗/Buff）检测到此标记后才执行
/// </summary>
public class ProjectileSystem : QuerySystem<ProjectileData, EffectTargetInfo, Position>
{
    public ProjectileSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ProjectileData proj, ref EffectTargetInfo target,
            ref Position pos, Entity effectEntity) =>
        {
            // 如果已到达，跳过
            if (effectEntity.Tags.Has<ProjectileArrived>()) return;

            // 计算目标位置（如果是追踪目标，实时更新坐标）
            float tx = target.targetX;
            float ty = target.targetY;
            if (!target.targetUnit.IsNull &&
                target.targetUnit.TryGetComponent<Position>(out var targetPos))
            {
                tx = targetPos.x;
                ty = targetPos.y;
                // 更新目标坐标以便后续使用
                target.targetX = tx;
                target.targetY = ty;
            }

            // 计算到目标的距离
            float dx = tx - pos.x;
            float dy = ty - pos.y;
            float dist = MathF.Sqrt(dx * dx + dy * dy);

            if (dist <= proj.arrivalThreshold)
            {
                // ★ 到达目标！
                effectEntity.AddTag<ProjectileArrived>();

                // 销毁弹道特效
                if (!proj.effectEntity.IsNull)
                {
                    EffectHelper.Destroy(proj.effectEntity, hideFirst: true);
                }
            }
            else
            {
                // 继续飞行
                float move = proj.speed * Tick.deltaTime;
                pos.x += dx / dist * move;
                pos.y += dy / dist * move;

                // 更新弹道特效位置
                if (!proj.effectEntity.IsNull)
                {
                    EffectHelper.SetPosition(proj.effectEntity, pos.x, pos.y, 0);
                }
            }
        });
    }
}

/// <summary>
/// 伤害效果处理系统 - 对目标造成伤害
/// </summary>
public class DamageEffectSystem : QuerySystem<DamageEffectData, EffectSource, EffectTargetInfo>
{
    public DamageEffectSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref DamageEffectData dmg, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            // 如果有弹道且未到达，等待
            if (effectEntity.HasComponent<ProjectileData>() && !effectEntity.Tags.Has<ProjectileArrived>())
                return;

            // 如果有范围搜索组件，由 AreaSearchSystem 处理（不在这里直接处理）
            if (effectEntity.HasComponent<AreaSearchData>())
                return;

            // 必须有目标单位
            if (target.targetUnit.IsNull) return;

            // 1. 计算最终伤害
            //    TODO: 根据护甲、减伤、增伤等修改最终数值
            float finalDamage = dmg.amount;

            // 2. 扣除目标生命值
            float remaining = AttributeHelper.ModifyCurrent(
                target.targetUnit, AttributeHelper.Health, -finalDamage);

            // 3. 标记血量需要同步到 Native
            target.targetUnit.AddTag<NativeealthDirty>();

            // 4. 检查是否死亡
            if (remaining <= 0)
            {
                // TODO: 调用 UnitHelper.Kill(target.targetUnit, source.caster);
            }

            // 5. 标记已处理（如果没有其他效果需要处理则删除）
            if (!effectEntity.HasComponent<HealEffectData>() &&
                !effectEntity.HasComponent<ApplyBuffData>())
            {
                toDelete.Add(effectEntity);
            }
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }
    }
}

/// <summary>
/// 治疗效果处理系统 - 回复目标生命值
/// </summary>
public class HealEffectSystem : QuerySystem<HealEffectData, EffectSource, EffectTargetInfo>
{
    public HealEffectSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref HealEffectData heal, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            // 如果有弹道且未到达，等待
            if (effectEntity.HasComponent<ProjectileData>() && !effectEntity.Tags.Has<ProjectileArrived>())
                return;

            // 如果有范围搜索组件，由 AreaSearchSystem 处理
            if (effectEntity.HasComponent<AreaSearchData>())
                return;

            // 必须有目标单位
            if (target.targetUnit.IsNull) return;

            // 1. 计算最终治疗量
            //    TODO: 根据增加治疗效果等修改
            float finalHeal = heal.amount;

            // 2. 回复目标生命值
            AttributeHelper.ModifyCurrent(
                target.targetUnit, AttributeHelper.Health, finalHeal);

            // 3. 标记血量需要同步
            target.targetUnit.AddTag<NativeealthDirty>();

            // 4. 标记已处理
            if (!effectEntity.HasComponent<DamageEffectData>() &&
                !effectEntity.HasComponent<ApplyBuffData>())
            {
                toDelete.Add(effectEntity);
            }
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }
    }
}

/// <summary>
/// Buff 施加效果系统 - 给目标添加 Buff
/// </summary>
public class BuffEffectSystem : QuerySystem<ApplyBuffData, EffectSource, EffectTargetInfo>
{
    public BuffEffectSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref ApplyBuffData buffData, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            // 如果有弹道且未到达，等待
            if (effectEntity.HasComponent<ProjectileData>() && !effectEntity.Tags.Has<ProjectileArrived>())
                return;

            // 如果有范围搜索组件，由 AreaSearchSystem 处理
            if (effectEntity.HasComponent<AreaSearchData>())
                return;

            // 必须有目标单位
            if (target.targetUnit.IsNull) return;

            // 施加 Buff（使用已有的 BuffHelper）
            BuffHelper.AddTimedBuff(
                Game.Store,
                target.targetUnit,
                source.caster,
                buffData.buffId,
                buffData.attrTypeId,
                buffData.modifyType,
                buffData.value,
                buffData.duration,
                buffData.refreshBehavior
            );

            // 标记已处理
            toDelete.Add(effectEntity);
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }
    }
}

/// <summary>
/// 效果清理系统 - 清理所有已处理完毕的效果 Entity
/// 作为最后的兜底，确保没有遗留的效果 Entity
/// </summary>
public class EffectCleanupSystem : QuerySystem<EffectSource>
{
    public EffectCleanupSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref EffectSource source, Entity effectEntity) =>
        {
            // 还有弹道在飞行中，不清理
            if (effectEntity.HasComponent<ProjectileData>() && !effectEntity.Tags.Has<ProjectileArrived>())
                return;

            // 还有范围搜索未处理，不清理
            if (effectEntity.HasComponent<AreaSearchData>())
                return;

            // 所有效果都已处理，清理
            bool hasUnprocessed =
                effectEntity.HasComponent<DamageEffectData>() ||
                effectEntity.HasComponent<HealEffectData>() ||
                effectEntity.HasComponent<ApplyBuffData>();

            if (!hasUnprocessed)
            {
                toDelete.Add(effectEntity);
            }
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }
    }
}
