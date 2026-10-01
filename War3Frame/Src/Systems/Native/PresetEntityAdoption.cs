using System.Collections.Generic;
using Friflo.Engine.ECS;

namespace War3Frame.Systems.Native;

/// <summary>
/// WE/编辑器预置单位与可破坏物 adoption：把地图加载时就存在的原生句柄登记进 <see cref="NativeEntityIndex"/>，
/// 使原生玩家单位事件桥能反查它们。只建立承载句柄缓存的代理实体，不创建、不销毁预置原生对象。
/// </summary>
/// <remarks>
/// 属于 Native 执行步骤：只做原生枚举、HandleAdd 与索引登记，不推进业务状态。
/// 须在攻击桥注册之前、主计时器启动之前调用；防双登记以索引查询为准，不依赖「初始化早于创建」的时序假设。
/// </remarks>
public static class PresetEntityAdoption
{
    // 过滤器传零句柄（等价 null）：中立敌对、中立被动与玩家预置单位都要能被攻击桥反查，不按玩家/联盟过滤。
    private static readonly JBoolExpr NoFilter = new();

    /// <summary>枚举当前地图预置单位与可破坏物并登记反查；启动时调用一次。</summary>
    public static void AdoptAll(EntityStore store)
    {
        // GetWorldBounds 覆盖地图边界外摆放；GetPlayableMapRect 会漏掉边界外预置物。
        var world = JassApi.GetWorldBounds();
        var group = JassApi.CreateGroup();

        // 枚举回调内只读取句柄，不做创建/销毁/订阅；枚举结束后统一销毁工具句柄。
        var units = new List<JUnit>();
        JassApi.GroupEnumUnitsInRect(group, world, NoFilter);
        JassApi.ForGroup(group, () => units.Add(JassApi.GetEnumUnit()));

        var destructables = new List<JDestructable>();
        JassApi.EnumDestructablesInRect(world, NoFilter, () => destructables.Add(JassApi.GetEnumDestructable()));

        // 枚举工具句柄就地销毁，不登记反查表。
        JassApi.DestroyGroup(group);
        JassApi.RemoveRect(world);

        AdoptHandles(store, units, destructables);
    }

    /// <summary>
    /// 对给定句柄建立反查代理；已登记句柄跳过，不覆盖框架实体。
    /// 拆出纯索引部分便于在不启动 War3 的回归中以句柄列表驱动。
    /// </summary>
    public static void AdoptHandles(EntityStore store, IReadOnlyList<JUnit> units, IReadOnlyList<JDestructable> destructables)
    {
        foreach (var unit in units)
            AdoptUnit(store, unit);
        foreach (var destructable in destructables)
            AdoptDestructable(store, destructable);
    }

    // 单位代理：只挂句柄缓存与预置标记，不挂 UnitLifeState / 模板属性，因此不会进入 UnitRemoveNativeSystem。
    private static void AdoptUnit(EntityStore store, JUnit unit)
    {
        // 防双登记：框架创建路径（UnitCreateNativeSystem）已登记时跳过，代理不得覆盖。
        if (NativeEntityIndex.TryGetByNative(unit, out _))
            return;

        var entity = store.CreateEntity(new UnitNative
        {
            unit = unit,
            player = JassApi.GetOwningPlayer(unit)
        });
        entity.AddTag<PresetNativeTag>();
        // 预置对象由地图加载创建；HandleAdd 只增加引用计数，避免句柄在桥接回调期间被回收，绝不 RemoveUnit。
        HandleHelper.HandleAdd(unit);
        NativeEntityIndex.Register(entity, unit);
    }

    // 可破坏物代理：只挂句柄缓存与预置标记。
    private static void AdoptDestructable(EntityStore store, JDestructable destructable)
    {
        if (NativeEntityIndex.TryGetByNative(destructable, out _))
            return;

        var entity = store.CreateEntity(new DestructableNative { destructable = destructable });
        entity.AddTag<PresetNativeTag>();
        HandleHelper.HandleAdd(destructable);
        NativeEntityIndex.Register(entity, destructable);
    }
}
