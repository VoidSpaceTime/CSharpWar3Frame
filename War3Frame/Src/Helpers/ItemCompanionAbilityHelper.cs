using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.TemplateInit;

namespace War3Frame;

/// <summary>
/// 统一创建、同步和绑定物品的 companion ability，不产生普通技能槽副作用。
/// </summary>
public static class ItemCompanionAbilityHelper
{
    /// <summary>
    /// 获取或创建物品唯一的 companion，并按当前物品 owner 完成绑定。
    /// </summary>
    public static bool TryEnsureCompanion(Entity item, out Entity companion)
    {
        companion = default;
        if (!TryReadConfiguration(item, out var useData, out var level))
            return false;

        if (item.TryGetComponent<ItemActiveAbility>(out var active) && !active.ability.IsNull)
        {
            companion = active.ability;
            if (!IsOwnedCompanion(item, companion))
            {
                companion = default;
                return false;
            }

            if (!TrySynchronizeLevel(companion, useData.abilityTemplateName, level))
                return false;
            BindCurrentOwner(item, companion);
            return true;
        }

        if (!AbilityTemplate.HasTemplate(useData.abilityTemplateName))
            return false;

        var created = AbilityHelper.CreateAbility(useData.abilityTemplateName, level, store: item.Store);
        try
        {
            AbilityTemplate.Apply(useData.abilityTemplateName, created, level);
            ValidateCompanionTemplate(created);
            created.RemoveComponent<AbilitySlotIndex>();
            created.AddComponent(new AbilityMountInfo { mountType = AbilityMountType.ItemGranted });
            BindCurrentOwner(item, created);
            item.AddComponent(new ItemActiveAbility(created));
            companion = created;
            return true;
        }
        catch
        {
            if (!created.IsNull)
                AbilityHelper.RemoveAbility(created);
            throw;
        }
    }

    /// <summary>
    /// 将已存在 companion 的 owner 更新为物品当前 owner；无 owner 时解除绑定。
    /// </summary>
    public static void BindCurrentOwner(Entity item)
    {
        if (item.TryGetComponent<ItemActiveAbility>(out var active)
            && !active.ability.IsNull
            && IsOwnedCompanion(item, active.ability))
        {
            BindCurrentOwner(item, active.ability);
        }
    }

    /// <summary>
    /// 在 owner 变化前按 companion identity 清理旧施法，再解除 AbilityOwner。
    /// </summary>
    public static void UnbindOwner(Entity item)
    {
        if (!item.TryGetComponent<ItemActiveAbility>(out var active)
            || active.ability.IsNull
            || !IsOwnedCompanion(item, active.ability))
            return;

        ItemCompanionCastCleanup.Cleanup(active.ability);
        active.ability.RemoveComponent<AbilityOwner>();
    }

    /// <summary>
    /// 在 ItemLevel 变化后显式重新应用 companion 模板等级。
    /// </summary>
    public static bool SynchronizeLevel(Entity item)
    {
        if (!TryReadConfiguration(item, out var useData, out var level)
            || !item.TryGetComponent<ItemActiveAbility>(out var active)
            || active.ability.IsNull)
        {
            return true;
        }

        if (!IsOwnedCompanion(item, active.ability))
            return false;

        return TrySynchronizeLevel(active.ability, useData.abilityTemplateName, level);
    }

    /// <summary>
    /// 验证 companion 的 Store、来源、挂载类型和反向身份均属于指定物品。
    /// </summary>
    internal static bool IsOwnedCompanion(Entity item, Entity companion)
    {
        return !item.IsNull
               && !companion.IsNull
               && ReferenceEquals(item.Store, companion.Store)
               && item.TryGetComponent<ItemActiveAbility>(out var active)
               && active.ability == companion
               && companion.TryGetComponent<AbilityMountInfo>(out var mount)
               && mount.mountType == AbilityMountType.ItemGranted
               && !companion.HasComponent<AbilitySlotIndex>();
    }

    private static bool TryReadConfiguration(Entity item, out ItemUseAbilityData useData, out int level)
    {
        useData = default;
        level = 1;
        if (item.IsNull || item.Tags.Has<ItemDestroyPendingTag>()
            || !item.TryGetComponent<ItemBase>(out _)
            || !item.TryGetComponent(out useData)
            || string.IsNullOrWhiteSpace(useData.abilityTemplateName))
        {
            return false;
        }

        if (item.TryGetComponent<ItemLevel>(out var itemLevel))
            level = Math.Max(1, itemLevel.level);
        return true;
    }

    /// <summary>
    /// 原地重配 companion 等级，保留施法运行时状态，失败时 companion 不变。
    /// </summary>
    private static bool TrySynchronizeLevel(Entity companion, string templateName, int level)
    {
        if (!companion.TryGetComponent<AbilityBase>(out var abilityBase))
            throw new InvalidOperationException($"实体 {companion.Id} 不是合法 companion ability");
        if (!string.Equals(abilityBase.templateName, templateName, StringComparison.Ordinal))
            throw new InvalidOperationException("ItemUseAbilityData 与现有 companion 模板不一致");
        if (abilityBase.level == level)
            return true;
        if (!AbilityTemplate.HasTemplate(templateName))
            throw new InvalidOperationException($"技能模板 '{templateName}' 未找到");
        if (ItemCompanionCastCleanup.HasReferences(companion))
            return false;

        // 保存运行时状态（Apply 会重写 AbilityBase）
        var savedState = abilityBase.state;
        companion.TryGetComponent<AbilityCooldownState>(out var savedCooldown);

        // 原地重配模板（所有等级相关组件原地更新）
        AbilityTemplate.Apply(templateName, companion, level);

        // 还原运行时状态
        if (companion.TryGetComponent<AbilityBase>(out var updated))
        {
            updated.state = savedState;
            companion.AddComponent(updated);
        }
        if (savedState == AbilityState.Cooldown && savedCooldown.remaining > 0f)
            companion.AddComponent(savedCooldown);
        else
            companion.RemoveComponent<AbilityCooldownState>();

        // 确保 companion 元数据不被 Apply 重置
        companion.RemoveComponent<AbilitySlotIndex>();
        companion.AddComponent(new AbilityMountInfo { mountType = AbilityMountType.ItemGranted });
        ValidateCompanionTemplate(companion);

        return true;
    }

    private static void BindCurrentOwner(Entity item, Entity companion)
    {
        if (!item.TryGetComponent<ItemOwner>(out var itemOwner) || itemOwner.unit.IsNull)
        {
            companion.RemoveComponent<AbilityOwner>();
            return;
        }

        if (!ReferenceEquals(item.Store, itemOwner.unit.Store)
            || !ReferenceEquals(item.Store, companion.Store))
        {
            throw new InvalidOperationException("Item、companion ability 与 owner 必须位于同一个 EntityStore");
        }

        if (!companion.TryGetComponent<AbilityOwner>(out var owner) || owner.owner != itemOwner.unit)
            companion.AddComponent(new AbilityOwner(itemOwner.unit));
    }

    private static void ValidateCompanionTemplate(Entity companion)
    {
        if (companion.HasComponent<AbilitySlotIndex>() || companion.HasComponent<AttributeContributionEntry>())
            throw new InvalidOperationException("Item companion 不允许槽位或被动属性贡献");

        if (!companion.TryGetComponent<AbilityBehaviorData>(out var behaviorData) || behaviorData.behaviors == null)
            return;

        foreach (var behavior in behaviorData.behaviors)
        {
            if (behavior.trigger is AbilityBehaviorTrigger.OnGranted or AbilityBehaviorTrigger.OnRemoved)
                throw new InvalidOperationException("Item companion 不允许 OnGranted 或 OnRemoved 行为");
        }
    }
}

/// <summary>
/// 按 companion identity 清理施法阶段，并提供受控删除所需的引用检查。
/// </summary>
internal static class ItemCompanionCastCleanup
{
    /// <summary>
    /// 清理所有引用指定 companion 的请求、移动和施法状态。
    /// </summary>
    public static void Cleanup(Entity companion)
    {
        if (companion.IsNull)
            return;

        var units = CollectReferencingUnits(companion);
        foreach (var unit in units.Values)
            CleanupUnit(unit, companion);
    }

    /// <summary>
    /// 判断 companion 是否仍被未完成施法、Effect 或 GroundArea 引用。
    /// </summary>
    public static bool HasReferences(Entity companion)
    {
        if (companion.IsNull)
            return false;

        var referenced = false;
        companion.Store.Query<CastRequest>().ForEachEntity((ref CastRequest request, Entity unit) =>
        {
            if (request.ability == companion)
                referenced = true;
        });
        if (referenced) return true;

        companion.Store.Query<CastState>().ForEachEntity((ref CastState cast, Entity unit) =>
        {
            if (cast.ability == companion)
                referenced = true;
        });
        if (referenced) return true;

        companion.Store.Query<ChannelState>().ForEachEntity((ref ChannelState channel, Entity unit) =>
        {
            if (channel.ability == companion)
                referenced = true;
        });
        if (referenced) return true;

        companion.Store.Query<MoveContinuation>().ForEachEntity((ref MoveContinuation continuation, Entity unit) =>
        {
            if (continuation.kind == MoveContinuationKind.CastAbility && continuation.ability == companion)
                referenced = true;
        });
        if (referenced) return true;

        companion.Store.Query<EffectSource>().ForEachEntity((ref EffectSource source, Entity effect) =>
        {
            if (source.ability == companion)
                referenced = true;
        });
        if (referenced) return true;

        companion.Store.Query<GroundAreaSource>().ForEachEntity((ref GroundAreaSource source, Entity area) =>
        {
            if (source.ability == companion)
                referenced = true;
        });
        return referenced;
    }

    private static Dictionary<int, Entity> CollectReferencingUnits(Entity companion)
    {
        var units = new Dictionary<int, Entity>();
        companion.Store.Query<CastRequest>().ForEachEntity((ref CastRequest request, Entity unit) =>
        {
            if (request.ability == companion)
                units[unit.Id] = unit;
        });
        companion.Store.Query<CastState>().ForEachEntity((ref CastState cast, Entity unit) =>
        {
            if (cast.ability == companion)
                units[unit.Id] = unit;
        });
        companion.Store.Query<ChannelState>().ForEachEntity((ref ChannelState channel, Entity unit) =>
        {
            if (channel.ability == companion)
                units[unit.Id] = unit;
        });
        companion.Store.Query<MoveContinuation>().ForEachEntity((ref MoveContinuation continuation, Entity unit) =>
        {
            if (continuation.kind == MoveContinuationKind.CastAbility && continuation.ability == companion)
                units[unit.Id] = unit;
        });
        return units;
    }

    private static void CleanupUnit(Entity unit, Entity companion)
    {
        if (unit.TryGetComponent<CastRequest>(out var request) && request.ability == companion)
            unit.RemoveComponent<CastRequest>();

        if (unit.TryGetComponent<CastState>(out var cast) && cast.ability == companion)
        {
            switch (cast.phase)
            {
                case CastPhase.MovingToCast:
                case CastPhase.Casting when !cast.effectCommitted:
                    unit.RemoveComponent<CastState>();
                    RemoveCastMovement(unit, companion);
                    RestoreReady(companion);
                    break;
                case CastPhase.Channeling:
                    if (unit.TryGetComponent<ChannelState>(out var channel) && channel.ability == companion)
                        unit.RemoveComponent<ChannelState>();
                    unit.RemoveComponent<CastState>();
                    AbilityHelper.EnterCooldownOrReady(companion);
                    break;
                case CastPhase.Backswing:
                    unit.RemoveComponent<CastState>();
                    AbilityHelper.EnterCooldownOrReady(companion);
                    break;
                default:
                    unit.RemoveComponent<CastState>();
                    if (cast.effectCommitted)
                        AbilityHelper.EnterCooldownOrReady(companion);
                    else
                        RestoreReady(companion);
                    break;
            }
        }

        if (unit.TryGetComponent<ChannelState>(out var orphanChannel) && orphanChannel.ability == companion)
        {
            unit.RemoveComponent<ChannelState>();
            AbilityHelper.EnterCooldownOrReady(companion);
        }

        RemoveCastMovement(unit, companion);
    }

    private static void RemoveCastMovement(Entity unit, Entity companion)
    {
        if (!unit.TryGetComponent<MoveContinuation>(out var continuation)
            || continuation.kind != MoveContinuationKind.CastAbility
            || continuation.ability != companion)
        {
            return;
        }

        unit.RemoveComponent<MoveContinuation>();
        if (unit.TryGetComponent<MoveCommand>(out var move) && move.reason == MoveReason.CastingAbility)
            unit.RemoveComponent<MoveCommand>();
        unit.RemoveComponent<MoveExecutionState>();
        unit.RemoveComponent<MoveOutcome>();
        unit.RemoveTag<MovingTag>();
        unit.RemoveTag<MovingForCastTag>();
    }

    private static void RestoreReady(Entity companion)
    {
        companion.RemoveComponent<AbilityCooldownState>();
        if (!companion.TryGetComponent<AbilityBase>(out var abilityBase))
            return;

        abilityBase.state = AbilityState.Ready;
        companion.AddComponent(abilityBase);
    }

    /// <summary>
    /// 单次扫描 Store 中所有受控引用载体，供批量延迟删除避免逐物品重复全表查询。
    /// </summary>
    internal static HashSet<int> CollectReferencedAbilityIds(EntityStore store)
    {
        var referenced = new HashSet<int>();
        store.Query<CastRequest>().ForEachEntity((ref CastRequest request, Entity unit) =>
        {
            if (!request.ability.IsNull)
                referenced.Add(request.ability.Id);
        });
        store.Query<CastState>().ForEachEntity((ref CastState cast, Entity unit) =>
        {
            if (!cast.ability.IsNull)
                referenced.Add(cast.ability.Id);
        });
        store.Query<ChannelState>().ForEachEntity((ref ChannelState channel, Entity unit) =>
        {
            if (!channel.ability.IsNull)
                referenced.Add(channel.ability.Id);
        });
        store.Query<MoveContinuation>().ForEachEntity((ref MoveContinuation continuation, Entity unit) =>
        {
            if (continuation.kind == MoveContinuationKind.CastAbility && !continuation.ability.IsNull)
                referenced.Add(continuation.ability.Id);
        });
        store.Query<EffectSource>().ForEachEntity((ref EffectSource source, Entity effect) =>
        {
            if (!source.ability.IsNull)
                referenced.Add(source.ability.Id);
        });
        store.Query<GroundAreaSource>().ForEachEntity((ref GroundAreaSource source, Entity area) =>
        {
            if (!source.ability.IsNull)
                referenced.Add(source.ability.Id);
        });
        return referenced;
    }
}
