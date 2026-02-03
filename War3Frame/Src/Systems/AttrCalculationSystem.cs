using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Src.Systems;
 
/// <summary>
///     属性计算系统 - 当单位标记为 AttrsDirty 时重新计算属性
///     使用 AttrWriterRegistry 自动处理所有已注册的属性类型
/// </summary>
public class AttrCalculationSystem : QuerySystem<BaseAttrs>
{
    public AttrCalculationSystem()
    {
        // 只处理有 AttrsDirty 标记的单位
        Filter.AnyTags(Tags.Get<AttrsDirty>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref BaseAttrs _, Entity entity) =>
        {
            // 遍历所有已注册的属性写入器
            foreach (var writer in AttrWriterRegistry.GetAllWriters())
            {
                // 读取基础值
                var baseValue = writer.ReadBase(entity);

                // 计算最终值
                var finalValue = CalculateAttr(entity, writer.AttrType, baseValue);

                // 写入目标组件
                writer.Write(entity, finalValue);
            }

            // 移除脏标记
            entity.RemoveTag<AttrsDirty>();
        });
    }

    /// <summary>
    ///     计算单个属性的最终值
    ///     公式: (base + flatSum) * (1 + percentAddSum) * percentMulProduct
    /// </summary>
    private static float CalculateAttr(Entity unit, AttrType attrType, float baseValue)
    {
        float flatSum = 0f;
        float percentAddSum = 0f;
        float percentMulProduct = 1f;

        // 获取指向该单位的所有修改器
        var modifiers = unit.GetIncomingLinks<ModifierTarget>();

        foreach (var link in modifiers)
        {
            var modifierEntity = link.Entity;
            if (!modifierEntity.TryGetComponent<AttrModifier>(out var mod))
                continue;

            if (mod.attrType != attrType)
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

        // 应用公式
        return (baseValue + flatSum) * (1 + percentAddSum) * percentMulProduct;
    }
}
