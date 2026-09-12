using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 作者声明的目标类型和离散筛选状态。仅保存 Hero/Normal/Building/Summon/Ward/Invisible/MagicImmune。
/// TODO Native：原生对象类型自动识别尚未实现；缺少本组件时不猜测原生类型或魔免状态。
/// </summary>
public struct UnitTargetTraits : IComponent
{
    public const TargetFilter Allowed = TargetFilter.Hero | TargetFilter.Normal | TargetFilter.Building
        | TargetFilter.Summon | TargetFilter.Ward | TargetFilter.Invisible | TargetFilter.MagicImmune;
    public TargetFilter flags;
}
