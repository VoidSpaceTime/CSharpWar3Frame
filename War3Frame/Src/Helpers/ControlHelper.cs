using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 控制效果帮助类 - 检查单位的控制状态
/// 控制效果通过属性系统实现，值 > 0 表示效果生效
/// </summary>
public static class ControlHelper
{
    /// <summary>
    /// 检查单位是否处于任何禁止行动的控制效果中（眩晕/击飞）
    /// </summary>
    public static bool IsIncapacitated(Entity unit)
    {
        return GetEffectiveValue(unit, AttributeHelper.Stun) > 0
               || GetEffectiveValue(unit, AttributeHelper.Knockback) > 0;
    }

    /// <summary>
    /// 检查单位是否无法移动（眩晕/定身/击飞）
    /// </summary>
    public static bool IsImmobilized(Entity unit)
    {
        return GetEffectiveValue(unit, AttributeHelper.Stun) > 0
               || GetEffectiveValue(unit, AttributeHelper.Root) > 0
               || GetEffectiveValue(unit, AttributeHelper.Knockback) > 0;
    }

    /// <summary>
    /// 检查单位是否无法施法（眩晕/沉默/击飞）
    /// </summary>
    public static bool IsSilenced(Entity unit)
    {
        return GetEffectiveValue(unit, AttributeHelper.Stun) > 0
               || GetEffectiveValue(unit, AttributeHelper.Silence) > 0
               || GetEffectiveValue(unit, AttributeHelper.Knockback) > 0;
    }

    /// <summary>
    /// 检查单位是否无法攻击（眩晕/缴械/击飞）
    /// </summary>
    public static bool IsDisarmed(Entity unit)
    {
        return GetEffectiveValue(unit, AttributeHelper.Stun) > 0
               || GetEffectiveValue(unit, AttributeHelper.Disarm) > 0
               || GetEffectiveValue(unit, AttributeHelper.Knockback) > 0;
    }

    /// <summary>
    /// 获取单位某控制效果的有效值（考虑免疫）
    /// </summary>
    public static float GetEffectiveValue(Entity unit, int controlAttrId)
    {
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
    /// 获取控制效果对应的免疫属性 ID
    /// </summary>
    private static int? GetImmunityAttrId(int controlAttrId)
    {
        if (controlAttrId == AttributeHelper.Stun) return AttributeHelper.StunImmunity;
        if (controlAttrId == AttributeHelper.Silence) return AttributeHelper.SilenceImmunity;
        if (controlAttrId == AttributeHelper.Disarm) return AttributeHelper.DisarmImmunity;
        if (controlAttrId == AttributeHelper.Root) return AttributeHelper.RootImmunity;
        if (controlAttrId == AttributeHelper.Knockback) return AttributeHelper.KnockbackImmunity;
        return null;
    }
}