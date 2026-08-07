using Friflo.Engine.ECS;

namespace War3Frame.Components;

/// <summary>
/// 物品使用请求携带的技能目标意图，运行时处理时再校验并规范化。
/// </summary>
public struct ItemUseTarget : IComponent
{
    public AbilityTargetType kind;
    public Entity targetUnit;
    public float targetX;
    public float targetY;
}

/// <summary>
/// 物品使用请求，仅记录使用者与物品身份。
/// </summary>
public struct ItemUseRequest : IComponent
{
    public Entity user;
    public Entity item;
}
