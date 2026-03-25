using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems;

/// <summary>
///     技能槽系统 - 管理技能槽相关逻辑
/// </summary>
public class AbilitySlotSystem : QuerySystem<AbilitySlotContainer>
{
    protected override void OnUpdate()
    {
        // 技能槽容器本身不需要每帧更新
        // 这个系统可以用于处理槽位相关的事件或同步
    }
}

