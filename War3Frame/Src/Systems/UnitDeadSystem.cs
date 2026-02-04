using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems;

public class UnitDeadSystem : QuerySystem<UnitNative>
{
    public UnitDeadSystem()
    {
        // 只处理有 AttrsDirty 标记的单位
        Filter.AnyTags(Tags.Get<UnitDeadTag, UnitFalseDeadTag>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitNative unitNative, Entity unit) =>
        {
            if (unit.Tags.Has<UnitDeadTag>())
            {
                /* 播放死亡动画
                 * 清除数据
                 * 删除实体
                 */

                unit.RemoveTag<UnitDeadTag>();
            }

            if (unit.Tags.Has<UnitFalseDeadTag>())
            {
                /*
                 *
                 */
                unit.RemoveTag<UnitFalseDeadTag>();
            }
        });
    }
}