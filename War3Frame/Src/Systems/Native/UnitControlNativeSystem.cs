using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;

namespace War3Frame.Systems.Native;

/// <summary>
/// 控制状态原生执行系统。
/// 消费 ControlStateNativeRequest，把控制进入/解除同步为 War3 原生能力开关；
/// 消费后删除请求。业务层不得直接调用这些原生能力。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class UnitControlNativeSystem : QuerySystem<ControlStateNativeRequest>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ControlStateNativeRequest request, Entity requestEntity) =>
        {
            // 单位已销毁或非原生单位（无句柄缓存）：跳过副作用，仅清理请求。
            if (!request.unit.TryGetComponent<UnitNative>(out var native))
            {
                requestEntity.DeleteEntity();
                return;
            }

            switch (request.controlType)
            {
                case ControlType.NoAttack:
                    DzApi.DzUnitDisableAttack(native.unit, request.entered);
                    break;
                case ControlType.Hide:
                    JassApi.ShowUnit(native.unit, request.entered);
                    break;
                case ControlType.Root:
                case ControlType.NoPath:
                    JassApi.SetUnitPathing(native.unit, request.entered);
                    break;
                case ControlType.Pause:
                    JassApi.PauseUnit(native.unit, request.entered);
                    break;
                // Locust / Invulnerable / Invisible / Sorcery 依赖默认地图模板提供对应
                // ability，模板就绪后再在此实现原生开关；当前不做原生副作用。
                case ControlType.Locust:
                case ControlType.Invulnerable:
                case ControlType.Invisible:
                case ControlType.Sorcery:
                    break;
            }

            requestEntity.DeleteEntity();
        });
    }
}