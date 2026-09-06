using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 模拟攻击系统：消费 UnitAttackedRequest，按攻击形态分流模拟攻击结算。
/// 近战（缺省 Melee）：直接生成 DamageRequest（伤害取 AttackDamage finalValue，ECS 优先）；
/// 远程/闪电链：后续增量实现（投射物/闪电链模拟），当前跳过并删除请求。
/// 执行顺序在 DamageResolveSystem(125) 之前，保证同帧请求可被结算。
/// </summary>
[SystemRegister(SystemKind.Immediate, 124)]
public class AttackSimulationSystem : QuerySystem<UnitAttackedRequest>
{
    private readonly List<Entity> _resolved = new();

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitAttackedRequest request, Entity requestEntity) =>
        {
            if (request.attacker.IsNull || request.target.IsNull)
            {
                _resolved.Add(requestEntity);
                return;
            }

            var attackType = AttackHelper.GetAttackType(request.attacker);
            switch (attackType)
            {
                case AttackType.Melee:
                    // 近战：命中即结算伤害（伤害 = AttackDamage finalValue）
                    var damage = GetAttackDamage(request.attacker);
                    if (damage > 0f)
                    {
                        Game.Store.CreateEntity(new DamageRequest
                        {
                            source = request.attacker,
                            target = request.target,
                            damage = new DamageBase
                            {
                                damage = damage,
                                damageType = DamageType.Physical,
                                damageSrc = DamageSrc.Melee,
                                source = request.attacker,
                                target = request.target
                            }
                        });
                    }
                    break;

                case AttackType.Ranged:
                case AttackType.Chain:
                    // TODO(后续增量)：远程生成投射物跟踪命中结算；闪电链走闪电链模拟。
                    break;
            }

            _resolved.Add(requestEntity);
        });

        foreach (var entity in _resolved)
            entity.DeleteEntity();
        _resolved.Clear();
    }

    /// <summary>读取单位攻击力（AttackDamage finalValue，ECS 优先；无属性视为 0）。</summary>
    private static float GetAttackDamage(Entity unit)
    {
        return AttributeHelper.GetFinalValue(unit, AttributeHelper.AttackDamage);
    }
}
