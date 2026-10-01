using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;

namespace War3Frame.Systems.Native;

// 本文件收拢单位域的两个一次性原生请求消费系统（均为 Immediate / 默认 order 1）。
//
// 合并前提：生成注册顺序为 OrderBy(Order).ThenBy(FQN, Ordinal)，且 TimedSystemRoot 按插入顺序
// 执行（TimedSystemRoot.cs:122）——即 order 相同的系统，执行顺序由完全限定类型名的字母序决定。
// 这两个类已同命名空间、同 Kind/Order，且在排序结果中相邻，故合并文件不改变任何 FQN 与执行位次。
//
// 注意：UnitCreateNativeSystem 不得挪入本文件。它当前属 War3Frame.Src.Systems，排在
// UnitNativeSystem（同样属 Src.Systems）之前；一旦并入 War3Frame.Systems.Native，它会排到全部
// Src.Systems.* 的 order-1 系统之后，从而翻到 UnitNativeSystem 之后，使新建单位的血蓝投影晚一帧。
// 若确要合并，必须先显式化 order 契约，不能靠"整理文件"顺手完成。

/// <summary>
/// 控制状态原生执行系统。
/// 消费 ControlStateNativeRequest，把控制进入/解除同步为 War3 原生能力开关；
/// 消费后删除请求。业务层不得直接调用这些原生能力。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class UnitControlNativeSystem : QuerySystem<ControlStateNativeRequest>
{
    // 1.27a 原生表没有 DzUnitSilence（GetNativeFunction 未命中返回 0，调用即空指针）。
    // 改用原版技能 'ACsp'（控制魔法）的开关命令 "spellshield" 锁/解原生施法。
    private const int AbilitySpellShield = 0x41437370; // 'ACsp'

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
                case ControlType.Silence:
                    ApplySilence(native.unit, request.entered);
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

    /// <summary>
    /// 以原版技能 'ACsp' 的开启/关闭切换原生施法锁。
    /// 仅当单位尚无该技能（等级 0）时添加并开启；仅当等级为 1（本分支添加的实例）时关闭并移除。
    /// 已有等级不为 1 的实例（地图自带）不添加也不移除，避免拆掉地图作者的技能。
    /// </summary>
    private static void ApplySilence(JUnit unit, bool entered)
    {
        var level = JassApi.GetUnitAbilityLevel(unit, AbilitySpellShield);
        if (entered)
        {
            // 新添加的 ACsp 默认关闭，必须 spellshield 一次才锁施法。
            if (level == 0)
            {
                JassApi.UnitAddAbility(unit, AbilitySpellShield);
                JassApi.IssueImmediateOrder(unit, "spellshield");
            }
        }
        else
        {
            // 先关再移除，避免移除瞬间命令落在已删除技能上。
            if (level == 1)
            {
                JassApi.IssueImmediateOrder(unit, "spellshield");
                JassApi.UnitRemoveAbility(unit, AbilitySpellShield);
            }
        }
    }
}

/// <summary>
/// 移动原生命令执行系统。
/// 仅负责将 ECS 中的命令请求翻译成 Warcraft 原生命令。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class UnitMoveNativeSystem : QuerySystem<MoveNativeRequest, UnitNative>
{
    // Native 执行层只翻译并下发移动命令，不推进施法、任务等业务流程。

    protected override void OnUpdate()
    {
        // 循环内 RemoveComponent 属结构变更：先执行原生命令并收集，循环外移除请求。
        var resolved = new List<Entity>();

        Query.ForEachEntity((ref MoveNativeRequest request, ref UnitNative native, Entity unit) =>
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
            resolved.Add(unit);
        });

        foreach (var unit in resolved)
        {
            if (!unit.IsNull)
                unit.RemoveComponent<MoveNativeRequest>();
        }
    }
}
