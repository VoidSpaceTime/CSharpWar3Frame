using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Components.Item;
using War3Frame.Systems;
using War3Frame;

namespace War3Frame.Src.Systems;

/// <summary>校验并消费地面物品拾取请求。</summary>
[SystemRegister(SystemKind.Immediate, 1)]
public sealed class ItemPickupWorkflowSystem : QuerySystem<ItemPickupRequest>
{
    private const float DefaultPickRadius = 160f;
    private readonly List<(Entity requestEntity, ItemPickupRequest request)> _pending = new();
    private readonly HashSet<Entity> _claimedItems = new();
    private readonly HashSet<(Entity owner, int slot)> _claimedSlots = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        _claimedItems.Clear();
        _claimedSlots.Clear();
        Query.ForEachEntity((ref ItemPickupRequest request, Entity requestEntity) =>
        {
            _pending.Add((requestEntity, request));
        });

        foreach (var (requestEntity, request) in _pending)
        {
            try
            {
                if (CanPickUp(request) && TryClaim(request))
                    PickUp(request);
            }
            finally
            {
                if (!requestEntity.IsNull)
                    requestEntity.DeleteEntity();
            }
        }
    }

    /// <summary>验证地面状态、距离、指定槽位和销毁阶段。</summary>
    private static bool CanPickUp(ItemPickupRequest request)
    {
        if (request.user.IsNull || request.item.IsNull
            || !ReferenceEquals(request.user.Store, request.item.Store)
            || !request.item.Tags.Has<ItemGroundTag>()
            || request.item.Tags.Has<ItemDestroyPendingTag>()
            || !ItemLifecycleOperations.CanAttach(request.user, request.item, request.slotIndex))
        {
            return false;
        }

        if (!request.user.TryGetComponent<Position>(out var userPosition)
            || !request.item.TryGetComponent<Position>(out var itemPosition))
        {
            return false;
        }

        var radius = request.item.TryGetComponent<ItemGroundPickRange>(out var pickRange)
            ? pickRange.radius
            : DefaultPickRadius;
        if (!float.IsFinite(radius) || radius < 0)
            return false;

        var dx = userPosition.x - itemPosition.x;
        var dy = userPosition.y - itemPosition.y;
        return dx * dx + dy * dy <= radius * radius;
    }

    /// <summary>
    /// 同批去重：挂载在后续 Attach 工作流才生效，CanAttach 看不到本批已接受的请求，
    /// 因此每件物品、每个 (单位, 槽位) 只接受第一个有效请求，其余请求不改动物品。
    /// </summary>
    private bool TryClaim(ItemPickupRequest request)
    {
        var slot = (request.user, request.slotIndex);
        if (_claimedItems.Contains(request.item) || _claimedSlots.Contains(slot))
            return false;

        _claimedItems.Add(request.item);
        _claimedSlots.Add(slot);
        return true;
    }

    /// <summary>先拆视觉，再把指定槽位的挂载意图交给 Attach 工作流。</summary>
    private static void PickUp(ItemPickupRequest request)
    {
        if (request.item.TryGetComponent<ItemGroundVisual>(out var visual))
        {
            if (visual.effect.IsNull)
            {
                request.item.RemoveComponent<ItemCreateNativeRequest>();
            }
            else
            {
                EffectHelper.Destroy(visual.effect);
            }

            visual.effect = default;
            request.item.AddComponent(visual);
        }
        else
        {
            request.item.RemoveComponent<ItemCreateNativeRequest>();
        }

        request.item.Store.CreateEntity(new ItemAttachRequest
        {
            owner = request.user,
            item = request.item,
            slotIndex = request.slotIndex
        });
    }
}
