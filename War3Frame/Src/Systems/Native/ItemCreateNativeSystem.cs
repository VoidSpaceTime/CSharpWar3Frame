using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Components.Item;
using War3Frame.Helpers;
using War3Frame.Systems;
using War3Frame;

namespace War3Frame.Systems.Native;

[SystemRegister(SystemKind.Immediate)]
/// <summary>消费地面物品特效请求并写回独立视觉实体引用。</summary>
public class ItemCreateNativeSystem : QuerySystem<ItemCreateNativeRequest>
{
    private readonly List<(Entity requestEntity, ItemCreateNativeRequest request)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref ItemCreateNativeRequest request, Entity requestEntity) =>
        {
            _pending.Add((requestEntity, request));
        });

        foreach (var (requestEntity, request) in _pending)
        {
            if (!request.item.IsNull
                && ReferenceEquals(requestEntity.Store, request.item.Store)
                && request.item.TryGetComponent<ItemBase>(out _)
                && request.item.TryGetComponent<ItemGroundVisual>(out var visual))
            {
                var effect = EffectHelper.CreatePosition(request.model, request.x, request.y, request.z, -1);
                if (effect is { } created)
                {
                    visual.effect = created;
                    request.item.AddComponent(visual);
                }
            }

        }

        foreach (var (requestEntity, _) in _pending)
        {
            if (!requestEntity.IsNull && requestEntity.HasComponent<ItemCreateNativeRequest>())
                requestEntity.RemoveComponent<ItemCreateNativeRequest>();
        }
    }
}

