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
    // Native 执行层只翻译并下发移动命令，不推进施法、任务等业务流程。

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref MoveNativeCommandRequest request, ref UnitNative native, Entity unit) =>
        {
            // commandToken 由上层 move 系统用于匹配结果；native 层只执行当前请求。
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
