using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 空间网格重建系统 - 每帧将所有有 Position 的 Entity 插入到 GroupHelper.Grid 中
/// 
/// 执行顺序：应在所有移动系统之后、所有搜索/效果系统之前执行
/// 即: MoveSystem → SpatialGridSystem → AreaSearchSystem / GroupHelper 查询
/// </summary>
public class SpatialGridSystem : QuerySystem<Position>
{
    protected override void OnUpdate()
    {
        // 每帧清空网格后重新插入
        GroupHelper.Grid.Clear();

        Query.ForEachEntity((ref Position pos, Entity entity) =>
        {
            GroupHelper.Grid.Insert(entity, pos.x, pos.y);
        });
    }
}
