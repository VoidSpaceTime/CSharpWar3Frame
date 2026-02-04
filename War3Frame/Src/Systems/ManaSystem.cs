using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components.Attribute;

namespace War3Frame.Src.Systems;

public class ManaSystem : QuerySystem<ManaAttr>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ManaAttr mana, Entity unit) =>
        {
            // 查找生命恢复属性
            if (UnitAttrHelper.TryGetAttr(unit, AttributeHelper.ManaRegen, out var regenAttr) ||
                UnitAttrHelper.TryGetAttr(unit, AttributeHelper.Mana, out var manaAttr)) return;

            var regen = regenAttr.Value.GetComponent<AttrValue>().finalValue;
            var attr = manaAttr.Value.GetComponent<AttrValue>().finalValue;

            mana.current += regen * Tick.deltaTime;
            mana.current = Math.Min(mana.current, attr);
        });
    }
}