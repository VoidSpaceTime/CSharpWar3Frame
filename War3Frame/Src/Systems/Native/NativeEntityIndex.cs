using System.Collections.Generic;
using Friflo.Engine.ECS;
using War3Frame;

namespace War3Frame;

/// <summary>
/// Native 句柄 → ECS 实体反查索引。
/// 原生回调（如单位事件触发）只拿到 native handle，经此表反查对应 ECS 实体。
/// 单表存储：War3 handle id 全局唯一（JWidget/JDestructable 统一 handle 空间），
/// 查出的实体自身组件即表达类型（有 UnitNative 是单位、有 DestructableNative 是可破坏物）。
/// Register/Unregister 须与 HandleHelper.HandleAdd/HandleRemove 在同一 Native 系统内配对相邻。
/// </summary>
public static class NativeEntityIndex
{
    private static readonly Dictionary<int, Entity> _byNativeHandle = new();

    /// <summary>登记 native handle → entity 映射（重复登记以新 entity 覆盖）。</summary>
    public static void Register(Entity entity, JHandle native)
    {
        _byNativeHandle[JassApi.GetHandleId(native)] = entity;
    }

    /// <summary>注销 native handle 映射（不存在时静默）。</summary>
    public static void Unregister(JHandle native)
    {
        _byNativeHandle.Remove(JassApi.GetHandleId(native));
    }

    /// <summary>按 native handle 反查 ECS 实体；未登记返回 false。</summary>
    public static bool TryGetByNative(JHandle native, out Entity entity)
    {
        return _byNativeHandle.TryGetValue(JassApi.GetHandleId(native), out entity);
    }
}
