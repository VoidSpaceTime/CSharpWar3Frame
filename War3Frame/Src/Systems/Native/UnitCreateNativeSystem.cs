using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 原生单位创建系统 - 立即消费创建请求
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class UnitCreateNativeSystem : QuerySystem<NativeUnitCreateRequest>
{
    // 原生单位创建集中在 Native 层，创建出的句柄回写为 UnitNative。

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref NativeUnitCreateRequest request, Entity entity) =>
        {
            if (entity.HasComponent<UnitNative>())
            {
                entity.RemoveComponent<NativeUnitCreateRequest>();
                return;
            }

            var junit = JassApi.CreateUnit(request.player, request.unitTypeId, request.x, request.y, request.facing);
            HandleHelper.HandleAdd(junit);

            entity.AddComponent(new UnitNative
            {
                unit = junit,
                player = request.player
            });

            entity.RemoveComponent<NativeUnitCreateRequest>();
        });
    }
}
