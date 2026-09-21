using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 控制效果帮助类 - 检查单位的控制状态
/// 控制效果通过属性系统实现，值 > 0 表示效果生效
/// </summary>
public static class ControlHelper
{
    /// <summary>
    /// 单个控制属性的映射：属性 ID + 免疫属性 ID + 对应 ControlType。
    /// 显式关联，不依赖数组下标与枚举序号对齐。
    /// </summary>
    public readonly record struct ControlAttrEntry(int AttrId, int ImmunityAttrId, ControlType ControlType);

    /// <summary>
    /// 控制属性映射表——控制/免疫映射的唯一权威来源（检测系统与免疫查询共用）。
    /// 新增控制类型时只在本表追加一条。
    /// Stun / CrackFly 的原生暂停动作由 Pause 合成承担；Pause 不进本表（由合成判定单独消费）。
    /// </summary>
    public static readonly ControlAttrEntry[] ControlAttrs =
    {
        new(AttributeHelper.Stun, AttributeHelper.StunImmunity, ControlType.Stun),
        new(AttributeHelper.Silence, AttributeHelper.SilenceImmunity, ControlType.Silence),
        new(AttributeHelper.NoAttack, AttributeHelper.NoAttackImmunity, ControlType.NoAttack),
        new(AttributeHelper.Root, AttributeHelper.RootImmunity, ControlType.Root),
        new(AttributeHelper.CrackFly, AttributeHelper.CrackFlyImmunity, ControlType.CrackFly),
    };

    /// <summary>
    /// 检查单位是否处于任何禁止行动的控制效果中（眩晕/击飞）
    /// </summary>
    public static bool IsIncapacitated(Entity unit)
    {
        return GetEffectiveValue(unit, AttributeHelper.Stun) > 0
               || GetEffectiveValue(unit, AttributeHelper.CrackFly) > 0;
    }

    /// <summary>
    /// 检查单位是否无法移动（眩晕/定身/击飞）
    /// </summary>
    public static bool IsImmobilized(Entity unit)
    {
        return GetEffectiveValue(unit, AttributeHelper.Stun) > 0
               || GetEffectiveValue(unit, AttributeHelper.Root) > 0
               || GetEffectiveValue(unit, AttributeHelper.CrackFly) > 0;
    }

    /// <summary>
    /// 检查单位是否无法施法（眩晕/沉默/击飞）
    /// </summary>
    public static bool IsSilenced(Entity unit)
    {
        return GetEffectiveValue(unit, AttributeHelper.Stun) > 0
               || GetEffectiveValue(unit, AttributeHelper.Silence) > 0
               || GetEffectiveValue(unit, AttributeHelper.CrackFly) > 0;
    }

    /// <summary>
    /// 检查单位当前是否允许发起新施法。
    /// 眩晕/沉默/击飞期间禁止发起新施法（不打断已开始的前摇吟唱——吟唱打断见 CastingSystem tick 语义）。
    /// </summary>
    public static bool CanCast(Entity unit)
    {
        return !IsSilenced(unit);
    }

    /// <summary>
    /// 检查单位是否无法攻击（眩晕/缴械/击飞）
    /// </summary>
    public static bool IsNoAttack(Entity unit)
    {
        return GetEffectiveValue(unit, AttributeHelper.Stun) > 0
               || GetEffectiveValue(unit, AttributeHelper.NoAttack) > 0
               || GetEffectiveValue(unit, AttributeHelper.CrackFly) > 0;
    }

    /// <summary>
    /// 获取单位某控制效果的有效值（考虑无敌与免疫）
    /// </summary>
    public static float GetEffectiveValue(Entity unit, int controlAttrId)
    {
        // 无敌压制（通用免疫）：Invulnerable 叠加态 >0 时五类控制全部读取为 0。
        // 与 StunImmunity 等免疫同构，在读取出口统一压制，避免施加入口漏拦。
        float invulnerable = GetAttrValue(unit, AttributeHelper.Invulnerable);
        if (invulnerable > 0) return 0;

        float value = GetAttrValue(unit, controlAttrId);
        if (value <= 0) return 0;

        // 检查对应的免疫
        int? immunityId = GetImmunityAttrId(controlAttrId);
        if (immunityId.HasValue)
        {
            float immunity = GetAttrValue(unit, immunityId.Value);
            if (immunity > 0) return 0; // 免疫
        }

        return value;
    }

    /// <summary>
    /// 获取属性值
    /// </summary>
    private static float GetAttrValue(Entity unit, int attrId)
    {
        var attr = AttributeHelper.GetAttr(unit, attrId);
        return attr?.GetComponent<AttrValue>().finalValue ?? 0;
    }

    /// <summary>
    /// 获取控制效果对应的免疫属性 ID；不在控制表内（如纯 Pause）返回 null。
    /// 查 ControlAttrs 权威表，避免与检测系统双份维护映射。
    /// </summary>
    private static int? GetImmunityAttrId(int controlAttrId)
    {
        foreach (var entry in ControlAttrs)
        {
            if (controlAttrId == entry.AttrId)
                return entry.ImmunityAttrId;
        }

        return null;
    }
}