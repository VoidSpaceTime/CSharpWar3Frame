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

/// <summary>
///     技能冷却系统 - 处理所有技能的冷却时间递减
/// </summary>
public class AbilityCooldownSystem : QuerySystem<AbilityBase>, ITimedSystem
{
    public float Interval => 0.1f; // 每 0.1 秒更新一次

    protected override void OnUpdate()
    {
        var deltaTime = Tick.deltaTime;

        Query.ForEachEntity((ref ability, entity) =>
        {
            // 只有在冷却状态时才更新
            if (ability.state == AbilityState.Cooldown && ability.currentCd > 0)
            {
                ability.currentCd -= deltaTime;

                // 冷却完成，切换到就绪状态
                if (ability.currentCd <= 0)
                {
                    ability.currentCd = 0;
                    ability.state = AbilityState.Ready;
                }
            }
        });
    }
}