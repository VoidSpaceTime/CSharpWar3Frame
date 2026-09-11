using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;
using War3Frame.Systems.Native;

namespace War3Frame.Src.Systems;

/// <summary>
/// 原生单位创建系统 - 立即消费创建请求
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class UnitCreateNativeSystem : QuerySystem<UnitCreateNativeRequest>
{
    // 原生单位创建集中在 Native 层，创建出的句柄回写为 UnitNative。

    protected override void OnUpdate()
    {
        // 循环内 AddComponent/RemoveComponent 属结构变更：原生创建仍在循环内完成，ECS 写回收集到循环外。
        var toAddNative = new List<(Entity entity, UnitNative native)>();
        var toRemoveRequest = new List<Entity>();

        Query.ForEachEntity((ref UnitCreateNativeRequest request, Entity entity) =>
        {
            if (entity.HasComponent<UnitNative>())
            {
                toRemoveRequest.Add(entity);
                return;
            }

            var junit = JassApi.CreateUnit(request.player, request.unitTypeId, request.x, request.y, request.facing);
            HandleHelper.HandleAdd(junit);
            NativeEntityIndex.Register(entity, junit);

            toAddNative.Add((entity, new UnitNative
            {
                unit = junit,
                player = request.player
            }));
            toRemoveRequest.Add(entity);
        });

        foreach (var (entity, native) in toAddNative)
        {
            if (!entity.IsNull && !entity.HasComponent<UnitNative>())
                entity.AddComponent(native);
        }

        foreach (var entity in toRemoveRequest)
        {
            if (!entity.IsNull)
                entity.RemoveComponent<UnitCreateNativeRequest>();
        }
    }
}
