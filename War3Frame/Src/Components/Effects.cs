using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 特效类型。
/// </summary>
public enum EffectType
{
    Position,
    Attach
}

/// <summary>
/// 特效附着点类型。
/// </summary>
public enum EffectAttachType
{
    Head,
    Origin,
    Weapon,
    Chest,
}

/// <summary>
/// 原生特效句柄缓存。
/// 仅作为执行层资源，不承载长期语义真相。
/// </summary>
public struct EffectNative : IComponent
{
    public JEffect effect;
}

/// <summary>
/// 附着意图，由 ECS 持有语义真相。
/// </summary>
public struct EffectAttachment : IComponent
{
    public Entity target;
    public EffectAttachType attachType;
}

/// <summary>
/// 一次性动画播放请求。
/// </summary>
public struct EffectAnimationRequest : IComponent
{
    /// <summary>动画名称。</summary>
    public string animation;

    /// <summary>动画链接名。</summary>
    public string link;
}

/// <summary>
/// 一次性特效销毁请求。
/// Native 系统消费后会销毁原生句柄并删除 ECS 特效实体。
/// </summary>
public struct EffectDestroyRequest : IComponent
{
    public bool hideFirst;
}

/// <summary>
/// 特效累积变换状态。
/// 保存旋转等变换的累积值，作为 ECS 真相。
/// </summary>
public struct EffectTransform : IComponent
{
    /// <summary>X 轴累积旋转角度（度）。</summary>
    public float rotateX;

    /// <summary>Y 轴累积旋转角度（度）。</summary>
    public float rotateY;

    /// <summary>Z 轴累积旋转角度（度）。</summary>
    public float rotateZ;

    /// <summary>标记是否需要重置矩阵。</summary>
    public bool needsReset;
}

/// <summary>
/// 特效基础数据。
/// 承载模型、外观与生命周期等公共信息。
/// </summary>
public struct EffectBase : IComponent
{
    /// <summary>模型路径。</summary>
    public string model;

    /// <summary>队伍颜色。</summary>
    public int teamColor;

    /// <summary>缩放倍率。</summary>
    public float sizeScale;

    /// <summary>播放速度。</summary>
    public float speed;

    /// <summary>是否可见。</summary>
    public bool visible;

    /// <summary>透明度。</summary>
    public int alpha;

    /// <summary>红色通道。</summary>
    public int red;

    /// <summary>绿色通道。</summary>
    public int green;

    /// <summary>蓝色通道。</summary>
    public int blue;

    /// <summary>特效类型。</summary>
    public EffectType effectType;

    /// <summary>附着点类型。</summary>
    public EffectAttachType effectAttachType;

}

/// <summary>
/// 特效脏标记位。
/// 用于控制哪些外观数据需要同步到原生特效。
/// </summary>
[Flags]
public enum EffectDirtyFlags
{
    None = 0,
    Color = 1 << 0,
    Scale = 1 << 1,
    Speed = 1 << 2,
    Visible = 1 << 3,
    Alpha = 1 << 4,
    TeamColor = 1 << 5,
    Transform = 1 << 6,
}

/// <summary>
/// 特效外观脏标记组件。
/// </summary>
public struct EffectDirty : IComponent
{
    public EffectDirtyFlags flags;
}

/// <summary>
/// 记录视觉特效与施法/天赋来源的关系，便于按 key 清理长期特效。
/// </summary>
public struct EffectVisualLink : IComponent
{
    public Entity owner;
    public string key;
}
