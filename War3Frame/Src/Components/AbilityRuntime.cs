using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 技能运行时状态
/// 只承载运行中的瞬时状态，不承载技能配置型数值
/// </summary>
public struct AbilityRuntime : IComponent
{
    public float cooldownRemaining;
    public float castRemaining;
    public float channelRemaining;
}
