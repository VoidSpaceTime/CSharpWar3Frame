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
                case ControlType.Stun:
                    // 眩晕 = 暂停单位一切行动；解除时恢复。
                    JassApi.PauseUnit(native.unit, request.entered);
                    break;

                case ControlType.Silence:
                case ControlType.Disarm:
                case ControlType.Root:
                case ControlType.Knockback:
                    // TODO(控制状态): 沉默/缴械/定身/击飞的 War3 原生能力映射待定
                    // （1.27 无原生沉默/缴械函数，需按可用扩展 API 或物编虚拟技能实现）。
                    // 事件已照常发出，业务可先监听 ControlStateChangedEvent 自定义响应。
                    break;
            }

            requestEntity.DeleteEntity();
        });
    }
}