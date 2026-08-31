using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 控制效果类型。每种控制对应一个控制属性（AttributeHelper.Stun 等）与一个免疫属性。
/// </summary>
public enum ControlType
{
    /// <summary>眩晕：禁止一切行动</summary>
    Stun,

    /// <summary>沉默：禁止施法</summary>
    Silence,

    /// <summary>缴械：禁止攻击</summary>
    Disarm,

    /// <summary>定身：禁止移动</summary>
    Root,

    /// <summary>击飞/击退：位移类控制</summary>
    Knockback,
}

/// <summary>
/// 控制状态快照（挂单位实体）。
/// 位域记录各控制类型当前是否生效（0/1），供跳变检测对比上一帧状态。
/// </summary>
public struct ControlStateSnapshot : IComponent
{
    /// <summary>位 0..4 依次对应 ControlType 各类型是否生效</summary>
    public byte bits;

    /// <summary>获取某控制类型当前是否生效。</summary>
    public bool IsActive(ControlType controlType)
    {
        return (bits & BitOf(controlType)) != 0;
    }

    /// <summary>设置某控制类型生效状态。</summary>
    public void SetActive(ControlType controlType, bool active)
    {
        var bit = BitOf(controlType);
        if (active)
            bits |= bit;
        else
            bits &= (byte)~bit;
    }

    private static byte BitOf(ControlType controlType)
    {
        return (byte)(1 << (int)controlType);
    }
}

/// <summary>
/// 控制状态跳变事件（独立事件实体，只读事实）。
/// 控制属性有效值（经免疫压制）从 0 变正或正变 0 时产生，供业务系统监听。
/// </summary>
public struct ControlStateChangedEvent : IComponent
{
    /// <summary>发生跳变的单位</summary>
    public Entity unit;

    /// <summary>控制类型</summary>
    public ControlType controlType;

    /// <summary>true=进入控制（0→正），false=解除控制（正→0）</summary>
    public bool entered;
}

/// <summary>
/// 控制状态原生副作用请求（一次性意图，Native 系统消费后删除）。
/// entered=true 时开启对应原生能力（如 PauseUnit），false 时恢复。
/// </summary>
public struct ControlStateNativeRequest : IComponent
{
    /// <summary>目标单位</summary>
    public Entity unit;

    /// <summary>控制类型</summary>
    public ControlType controlType;

    /// <summary>true=进入控制，false=解除控制</summary>
    public bool entered;
}