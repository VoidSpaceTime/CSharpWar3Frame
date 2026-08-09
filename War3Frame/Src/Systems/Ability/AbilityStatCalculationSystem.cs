using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 技能数值计算系统 - 消费 AbilityStatDirty 并重算 AbilityStatValue.finalValue。
/// </summary>
/// <remarks>
/// order 30：位于 Buff/Aura 写入之后、技能效果结算（100+）之前。
/// </remarks>
[SystemRegister(SystemKind.Interval, 30)]
public class AbilityStatCalculationSystem : QuerySystem<AbilityStatValue>
{
    public AbilityStatCalculationSystem() 
    {
        Filter.AnyTags(Tags.Get<AbilityStatDirty>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AbilityStatValue stat, Entity statEntity) =>
        {
            var modifiers = statEntity.GetIncomingLinks<ModifyTarget>();

            float flatSum = 0;
            float percentAddSum = 0;
            float percentMulProduct = 1f;

            foreach (var link in modifiers)
            {
                var modEntity = link.Entity;
                if (!modEntity.TryGetComponent<ModifyValue>(out var mod))
                    continue;

                switch (mod.modifyType)
                {
                    case ModifyType.Flat:
                        flatSum += mod.value;
                        break;
                    case ModifyType.PercentAdd:
                        percentAddSum += mod.value;
                        break;
                    case ModifyType.PercentMul:
                        percentMulProduct *= (1 + mod.value);
                        break;
                }
            }

            stat.finalValue = (stat.baseValue + flatSum) * (1 + percentAddSum) * percentMulProduct;
            statEntity.RemoveTag<AbilityStatDirty>();
        });
    }
}
