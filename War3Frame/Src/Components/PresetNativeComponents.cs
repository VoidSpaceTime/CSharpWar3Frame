using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 预置实体标记：标识该实体由 WE/编辑器预置 adoption 创建，不是框架创建链产物。
/// 零数据，仅用于分类；不挂生命周期与模板组件。
/// </summary>
public struct PresetNativeTag : ITag { }

/// <summary>
/// 原生可破坏物句柄缓存，仅供 Native 反查与执行层读取；长期语义仍以 ECS 组件为准。
/// </summary>
public struct DestructableNative : IComponent
{
    public JDestructable destructable;
}
