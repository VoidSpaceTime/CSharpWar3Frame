using System.Numerics;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Native;

/// <summary>
/// 移动原生命令执行系统。
/// 仅负责将 ECS 中的命令请求翻译成 Warcraft 原生命令。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class MoveNativeExecutionSystem : QuerySystem<MoveNativeCommandRequest, UnitNative>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref MoveNativeCommandRequest request, ref UnitNative native, Entity unit) =>
        {
            switch (request.orderType)
            {
                case MoveOrderType.Move:
                    JassApi.IssuePointOrder(native.unit, "move", request.targetX, request.targetY);
                    break;
                case MoveOrderType.Stop:
                    JassApi.IssueImmediateOrder(native.unit, "stop");
                    break;
                case MoveOrderType.Hold:
                    JassApi.IssueImmediateOrder(native.unit, "holdposition");
                    break;
            }
            unit.RemoveComponent<MoveNativeCommandRequest>();
        });
    }
}