using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 校验物品使用请求，并将首个有效请求转换为 companion ability 的施法请求。
/// </summary>
[SystemRegister(SystemKind.Immediate, 2)]
public sealed class ItemUseSystem : QuerySystem<ItemUseRequest>
{
    private const float MaxTargetCoordinate = 1_000_000f;
    private readonly List<(Entity entity, ItemUseRequest request)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref ItemUseRequest request, Entity requestEntity) =>
        {
            _pending.Add((requestEntity, request));
        });

        _pending.Sort(static (left, right) => left.entity.Id.CompareTo(right.entity.Id));
        foreach (var pending in _pending)
            Process(pending.entity, pending.request);
    }

    /// <summary>
    /// 在请求查询结束后完成校验与 CastRequest 派发，并始终清理请求。
    /// </summary>
    private static void Process(Entity requestEntity, ItemUseRequest request)
    {
        try
        {
            if (!TryPrepare(requestEntity, request, out var target, out var companion))
                return;

            request.user.AddComponent(new CastRequest
            {
                ability = companion,
                targetUnit = target.targetUnit,
                targetX = target.targetX,
                targetY = target.targetY,
                itemOrigin = new ItemCastOrigin
                {
                    item = request.item,
                    user = request.user
                }
            });
        }
        finally
        {
            if (!requestEntity.IsNull)
                requestEntity.DeleteEntity();
        }
    }

    /// <summary>
    /// 重新校验物品、目标与 companion 状态，确保不会覆盖已有施法。
    /// </summary>
    private static bool TryPrepare(Entity requestEntity, ItemUseRequest request,
        out ItemUseTarget target, out Entity companion)
    {
        target = default;
        companion = default;

        if (request.user.IsNull || request.item.IsNull)
            return false;
        if (!ReferenceEquals(requestEntity.Store, request.user.Store)
            || !ReferenceEquals(requestEntity.Store, request.item.Store))
        {
            return false;
        }

        if (!request.item.TryGetComponent<ItemBase>(out var itemBase)
            || !request.item.TryGetComponent<ItemOwner>(out var owner)
            || owner.unit != request.user
            || request.item.Tags.Has<ItemDestroyPendingTag>())
        {
            return false;
        }

        if (!HasUsableState(request.item) || !itemBase.isUsable)
            return false;

        if (!request.item.HasComponent<ItemUseAbilityData>())
            return false;

        if (!TryNormalizeTarget(requestEntity, request.user, out target))
            return false;

        if (request.user.HasComponent<CastRequest>()
            || request.user.HasComponent<CastState>()
            || request.user.HasComponent<ChannelState>())
        {
            return false;
        }

        if (!ItemCompanionAbilityHelper.TryEnsureCompanion(request.item, out companion)
            || !companion.TryGetComponent<AbilityBase>(out var abilityBase)
            || abilityBase.state != AbilityState.Ready)
        {
            return false;
        }

        return IsCompatibleTarget(target.kind, abilityBase.targetType);
    }

    private static bool HasUsableState(Entity item)
    {
        return !item.Tags.Has<ItemGroundTag>()
               && !item.Tags.Has<ItemStoredTag>()
               && (item.Tags.Has<ItemInventoryTag>() || item.Tags.Has<ItemEquippedTag>());
    }

    /// <summary>
    /// 将 None、Unit 或 Point 意图转换为稳定的单位与坐标快照。
    /// </summary>
    private static bool TryNormalizeTarget(Entity requestEntity, Entity user, out ItemUseTarget target)
    {
        target = default;
        if (!requestEntity.TryGetComponent<ItemUseTarget>(out var requestedTarget))
            return false;

        switch (requestedTarget.kind)
        {
            case AbilityTargetType.None:
                if (!TryGetFinitePosition(user, out var userPosition))
                    return false;

                target = new ItemUseTarget
                {
                    kind = AbilityTargetType.None,
                    targetUnit = user,
                    targetX = userPosition.x,
                    targetY = userPosition.y
                };
                return true;
            case AbilityTargetType.Unit:
                if (requestedTarget.targetUnit.IsNull
                    || !ReferenceEquals(requestEntity.Store, requestedTarget.targetUnit.Store)
                    || !TryGetFinitePosition(requestedTarget.targetUnit, out var targetPosition))
                {
                    return false;
                }

                target = new ItemUseTarget
                {
                    kind = AbilityTargetType.Unit,
                    targetUnit = requestedTarget.targetUnit,
                    targetX = targetPosition.x,
                    targetY = targetPosition.y
                };
                return true;
            case AbilityTargetType.Point:
            case AbilityTargetType.Area:
                if (!float.IsFinite(requestedTarget.targetX)
                    || !float.IsFinite(requestedTarget.targetY)
                    || Math.Abs(requestedTarget.targetX) > MaxTargetCoordinate
                    || Math.Abs(requestedTarget.targetY) > MaxTargetCoordinate)
                {
                    return false;
                }

                target = requestedTarget;
                target.targetUnit = default;
                return true;
            default:
                return false;
        }
    }

    private static bool TryGetFinitePosition(Entity entity, out Position position)
    {
        return entity.TryGetComponent(out position)
               && float.IsFinite(position.x)
               && float.IsFinite(position.y)
               && Math.Abs(position.x) <= MaxTargetCoordinate
               && Math.Abs(position.y) <= MaxTargetCoordinate;
    }

    /// <summary>
    /// 应用批准的 ItemUse 与 Ability target type 兼容矩阵。
    /// </summary>
    private static bool IsCompatibleTarget(AbilityTargetType itemTarget, AbilityTargetType abilityTarget)
    {
        return itemTarget == abilityTarget;
    }
}
