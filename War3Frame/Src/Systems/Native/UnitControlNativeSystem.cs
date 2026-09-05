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
                case ControlType.Locust:
                    /*
                     /// TODO 需要默认map有蝗虫的模板
                    if (request.entered)
                    {
                        if (JassApi.GetUnitAbilityLevel(native.unit, FRAMEWORK_ID["ability_locust"]) < 1)
                            JassApi.UnitAddAbility(native.unit, FRAMEWORK_ID["ability_locust"]);
                    }
                    else
                    {
                        if (JassApi.GetUnitAbilityLevel(native.unit, FRAMEWORK_ID["ability_locust"]) >= 1)
                            JassApi.UnitRemoveAbility(native.unit, FRAMEWORK_ID["ability_locust"]);
                    }
                    */

                    break;
                case ControlType.Invulnerable:
                    /*
                     /// TODO 需要默认map有无敌的模板
                    if (request.entered)
                    {
                        if (JassApi.GetUnitAbilityLevel(native.unit, FRAMEWORK_ID["ability_invulnerable"]) < 1)
                            JassApi.UnitAddAbility(native.unit, FRAMEWORK_ID["ability_invulnerable"]);
                    }
                    else
                    {
                        if (JassApi.GetUnitAbilityLevel(native.unit, FRAMEWORK_ID["ability_invulnerable"]) >= 1)
                            JassApi.UnitRemoveAbility(native.unit, FRAMEWORK_ID["ability_invulnerable"]);
                    }
                    */

                    break;
                case ControlType.Invisible:
                    /*
                     /// TODO 需要默认map有无敌的模板
                    if (request.entered)
                    {
                        if (JassApi.GetUnitAbilityLevel(native.unit, FRAMEWORK_ID["ability_invisible"]) < 1)
                            JassApi.UnitAddAbility(native.unit, FRAMEWORK_ID["ability_invisible"]);
                    }
                    else
                    {
                        if (JassApi.GetUnitAbilityLevel(native.unit, FRAMEWORK_ID["ability_invisible"]) >= 1)
                            JassApi.UnitRemoveAbility(native.unit, FRAMEWORK_ID["ability_invisible"]);
                    }
                    */

                    break;
                case ControlType.Sorcery:
                    /*
                /// TODO 需要默认map有巫术的模板
               if (request.entered)
               {
                   if (JassApi.GetUnitAbilityLevel(native.unit, FRAMEWORK_ID["ability_invisible"]) < 1)
                       JassApi.UnitAddAbility(native.unit, FRAMEWORK_ID["ability_invisible"]);
               }
               else
               {
                   if (JassApi.GetUnitAbilityLevel(native.unit, FRAMEWORK_ID["ability_invisible"]) >= 1)
                       JassApi.UnitRemoveAbility(native.unit, FRAMEWORK_ID["ability_invisible"]);
               }
               */

                    break;
            }

            requestEntity.DeleteEntity();
        });
    }
}