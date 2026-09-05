using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Src.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 控制状态跳变检测系统。
/// 复用属性系统的 finalValue 作为多来源叠加真相（ModifyValue 修改器聚合），
/// 检测"有效值（经免疫压制）0↔正"跳变，生成 ControlStateChangedEvent（业务监听）
/// 与 ControlStateNativeRequest（Native 副作用），并维护单位上的 ControlStateSnapshot。
/// </summary>
/// <remarks>
/// order 46：位于 AttrCalculationSystem（45）之后，保证读到已重算的 finalValue；
/// 位于效果结算（100+）之前。仅处理"有控制或免疫属性"的单位，避免全量扫描。
/// </remarks>
[SystemRegister(SystemKind.Interval, 46)]
public class ControlStateTransitionSystem : QuerySystem<AttrTypeId, AttrOwner>
{
    /// <summary>单个控制属性的映射：属性 ID + 免疫属性 ID + 对应 ControlType（显式关联，不再依赖数组下标与枚举序号对齐）。</summary>
    private readonly record struct ControlAttrEntry(int AttrId, int ImmunityAttrId, ControlType ControlType);

    /// <summary>控制属性映射表（与免疫一一对应）。新增控制类型时在此追加一条，无需关心枚举序号。</summary>
    private static readonly ControlAttrEntry[] ControlAttrs =
    {
        new(AttributeHelper.Stun,        AttributeHelper.StunImmunity,        ControlType.Stun),
        new(AttributeHelper.Silence,     AttributeHelper.SilenceImmunity,     ControlType.Silence),
        new(AttributeHelper.NoAttack,    AttributeHelper.NoAttackImmunity,    ControlType.NoAttack),
        new(AttributeHelper.Root,        AttributeHelper.RootImmunity,        ControlType.Root),
        new(AttributeHelper.CrackFly,    AttributeHelper.CrackFlyImmunity,    ControlType.CrackFly),
    };

    /// <summary>本 tick 需要检测的单位集合（复用避免分配）。</summary>
    private readonly HashSet<Entity> _units = new();

    /// <summary>
    /// 绑定的 EntityStore（首次收集时从单位实体缓存）。
    /// 不依赖 Game.Store 静态全局，本地验证场景可用独立 store 驱动本系统。
    /// </summary>
    private EntityStore? _store;

    protected override void OnUpdate()
    {
        // 1. 收集"有控制属性或免疫属性"的单位：两类属性的增减都会影响有效值。
        Query.ForEachEntity((ref AttrTypeId attrType, ref AttrOwner owner, Entity attrEntity) =>
        {
            var typeId = attrType.typeId;
            if (!IsControlOrImmunity(typeId))
                return;

            if (owner.owner.IsNull)
                return;

            _store ??= owner.owner.Store;
            _units.Add(owner.owner);
        });

        // 2. 对每个单位对比快照与当前有效值，跳变时发事件 + 请求。
        foreach (var unit in _units)
        {
            if (!unit.TryGetComponent<ControlStateSnapshot>(out var snapshot))
            {
                snapshot = new ControlStateSnapshot();
                unit.AddComponent(snapshot);
            }

            foreach (var entry in ControlAttrs)
            {
                var controlType = entry.ControlType;
                var active = ControlHelper.GetEffectiveValue(unit, entry.AttrId) > 0f;
                if (snapshot.IsActive(controlType) == active)
                    continue;

                snapshot.SetActive(controlType, active);
                unit.AddComponent(snapshot);

                _store.CreateEntity(new ControlStateChangedEvent
                {
                    unit = unit,
                    controlType = controlType,
                    entered = active,
                }).AddComponent(new TriggerEventMarker
                {
                    eventTypeId = EventTypeRegistry.Get<ControlStateChangedEvent>()
                });
                _store.CreateEntity(new ControlStateNativeRequest
                {
                    unit = unit,
                    controlType = controlType,
                    entered = active,
                });
            }
        }

        // 3. 清理：不再持有任何控制/免疫属性的单位移除快照（单位销毁时组件自动清除，此处处理属性被整体移除的情况）。
        // 若有残留激活位（属性整体移除前仍处于控制中），先补发解除事件 + 请求，保证原生状态收敛（避免永久暂停）。
        // 收集后统一处理，避免在迭代中做结构变更。store 为 null 时（从未有控制属性单位）无需清理。
        var toRemove = default(List<Entity>);
        var toRelease = default(List<(Entity unit, byte bits)>);
        if (_store != null)
        {
            _store.Query<ControlStateSnapshot>().ForEachEntity((ref ControlStateSnapshot snapshot, Entity unit) =>
            {
                if (_units.Contains(unit))
                    return;

                if (snapshot.bits != 0)
                {
                    toRelease ??= new List<(Entity, byte)>();
                    toRelease.Add((unit, snapshot.bits));
                }

                toRemove ??= new List<Entity>();
                toRemove.Add(unit);
            });
        }

        if (toRelease != null)
        {
            foreach (var (unit, bits) in toRelease)
            {
                foreach (var entry in ControlAttrs)
                {
                    if ((bits & (1 << (int)entry.ControlType)) == 0)
                        continue;

                    _store.CreateEntity(new ControlStateChangedEvent
                    {
                        unit = unit,
                        controlType = entry.ControlType,
                        entered = false,
                    }).AddComponent(new TriggerEventMarker
                    {
                        eventTypeId = EventTypeRegistry.Get<ControlStateChangedEvent>()
                    });
                    _store.CreateEntity(new ControlStateNativeRequest
                    {
                        unit = unit,
                        controlType = entry.ControlType,
                        entered = false,
                    });
                }
            }
        }

        if (toRemove != null)
        {
            foreach (var unit in toRemove)
                unit.RemoveComponent<ControlStateSnapshot>();
        }

        _units.Clear();
    }

    /// <summary>判断属性类型是否属于控制或免疫体系。</summary>
    private static bool IsControlOrImmunity(int typeId)
    {
        foreach (var entry in ControlAttrs)
        {
            if (typeId == entry.AttrId || typeId == entry.ImmunityAttrId)
                return true;
        }

        return false;
    }
}