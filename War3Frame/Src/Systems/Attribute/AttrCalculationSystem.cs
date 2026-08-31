using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
///     属性计算系统 - 当单位标记为 AttrsDirty 时重新计算属性
///     使用 AttrWriterRegistry 自动处理所有已注册的属性类型
/// </summary>
/// <remarks>
/// order 45：位于 dirty 写入方（Item/Level 0、Buff 40/41、Aura 42）之后、
/// 效果结算（100+）之前，确保结算读到的是已重算的 finalValue。
/// </remarks>
[SystemRegister(SystemKind.Interval, 45)]
public class AttrCalculationSystem : QuerySystem<AttrValue>
{
    public AttrCalculationSystem()
    {
        // 只处理有 AttrsDirty 标记的单位
        Filter.AnyTags(Tags.Get<AttrDirty>());
    }

    protected override void OnUpdate()
    {
        // Friflo 禁止在 Query 循环内做结构变更（RemoveTag 亦属结构变更），
        // 先收集需要重算的属性实体，循环外统一移除脏标记。
        var recalculated = new List<Entity>();

        Query.ForEachEntity((ref AttrValue attr, Entity attrEntity) =>
        {
            // 1. 收集所有指向此属性的修改器
            var modifiers = attrEntity.GetIncomingLinks<ModifyTarget>();

            float flatSum = 0;
            float percentAddSum = 0;
            float percentMulProduct = 1f;

            // 2. 累加修改器
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

            // 3. 计算最终值
            // 公式: (base + flat) × (1 + percentAdd) × percentMul
            attr.flatBonus = flatSum;
            attr.percentBonus = percentAddSum;
            attr.finalValue = (attr.baseValue + flatSum)
                              * (1 + percentAddSum)
                              * percentMulProduct;

            recalculated.Add(attrEntity);
        });

        // 4. 循环外移除脏标记
        foreach (var attrEntity in recalculated)
        {
            attrEntity.RemoveTag<AttrDirty>();
        }
    }
}