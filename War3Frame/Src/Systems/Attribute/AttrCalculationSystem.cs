using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
///     属性计算系统 - 当属性标记为 AttrDirty 时重新计算 finalValue，
///     并顺带回收"无贡献者、未被模板声明、且处于初始值"的孤儿属性实体。
/// </summary>
/// <remarks>
/// order 45：位于 dirty 写入方（Item/Level 0、Buff 40/41、Aura 42）之后、
/// 效果结算（100+）之前，确保结算读到的是已重算的 finalValue。
/// 回收只针对本轮 dirty 属性，从不 dirty 的属性不参与（多为声明但未使用，应保留）。
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
        // Friflo 禁止在 Query 循环内做结构变更（RemoveTag/DeleteEntity 亦属结构变更），
        // 先收集需要重算与待回收的属性实体，循环外统一处理。
        var recalculated = new List<Entity>();
        var toReclaim = new List<Entity>();

        Query.ForEachEntity((ref AttrValue attr, Entity attrEntity) =>
        {
            // 1. 收集所有指向此属性的修改器
            var modifiers = attrEntity.GetIncomingLinks<ModifyTarget>();

            float flatSum = 0;
            float percentAddSum = 0;
            float percentMulProduct = 1f;
            var hasIncoming = false;

            // 2. 累加修改器
            foreach (var link in modifiers)
            {
                // 任何入边（含无 ModifyValue 的 DoT 载体）都视为"有贡献者"，阻止回收。
                hasIncoming = true;
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

            // 4. 孤儿属性判定：重算后 flat/percent 即为最新值，满足"非脏"前提。
            if (!hasIncoming
                && attr.baseValue == 0f
                && attr.current == 0f
                && attr.flatBonus == 0f
                && attr.percentBonus == 0f
                && CanReclaimByOwner(attrEntity))
            {
                toReclaim.Add(attrEntity);
            }
        });

        // 5. 循环外移除脏标记
        foreach (var attrEntity in recalculated)
        {
            if (!attrEntity.IsNull)
                attrEntity.RemoveTag<AttrDirty>();
        }

        // 6. 孤儿属性回收：按 entity id 升序，保证锁步确定性；
        //    先移除 owner 的 HasAttr 反向关系，再删除属性实体，避免悬挂关系。
        if (toReclaim.Count > 0)
        {
            toReclaim.Sort(static (left, right) => left.Id.CompareTo(right.Id));

            foreach (var attrEntity in toReclaim)
            {
                if (attrEntity.IsNull)
                    continue;

                if (attrEntity.TryGetComponent<AttrOwner>(out var attrOwner) && !attrOwner.owner.IsNull)
                    attrOwner.owner.RemoveRelation<HasAttr, Entity>(attrEntity);

                attrEntity.DeleteEntity();
            }
        }
    }

    /// <summary>
    /// 判定属性实体是否满足"按模板声明归属"的回收条件。
    /// 保守策略：属性无归属、owner 非模板单位（无 UnitSpecData）时一律保留，无法判定声明则不回收；
    /// 仅当 owner 带模板且该 typeId 未被模板声明时返回 true。
    /// </summary>
    private static bool CanReclaimByOwner(Entity attrEntity)
    {
        if (!attrEntity.TryGetComponent<AttrOwner>(out var attrOwner))
            return false;

        var owner = attrOwner.owner;
        if (owner.IsNull || !owner.TryGetComponent<UnitSpecData>(out var specData) || specData.spec == null)
            return false;

        if (!attrEntity.TryGetComponent<AttrTypeId>(out var attrType))
            return false;

        foreach (var attribute in specData.spec.attributes)
        {
            if (attribute.attrTypeId == attrType.typeId)
                return false;
        }

        return true;
    }
}