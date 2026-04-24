using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Ability;

/// <summary>
///     技能冷却系统 - 处理所有技能的冷却时间递减
/// </summary>
public class AbilityCooldownSystem : QuerySystem<AbilityBase, AbilityCooldownState>, ITimedSystem
{
    public float Interval => 0.1f; // 每 0.1 秒更新一次

    protected override void OnUpdate()
    {
        var deltaTime = Tick.deltaTime;

        Query.ForEachEntity((ref AbilityBase ability, ref AbilityCooldownState cooldown, Entity entity) =>
        {
            // 只有在冷却状态时才更新
            if (ability.state == AbilityState.Cooldown && cooldown.remaining > 0)
            {
                cooldown.remaining -= deltaTime;

                // 冷却完成，切换到就绪状态
                if (cooldown.remaining <= 0)
                {
                    cooldown.remaining = 0;
                    ability.state = AbilityState.Ready;
                }

                entity.AddComponent(ability);
                entity.AddComponent(cooldown);
            }
        });
    }
}
