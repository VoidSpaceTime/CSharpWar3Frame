using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Src.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 控制状态跳变检测系统：
/// 复用属性系统的 finalValue 作为多来源叠加真相（ModifyValue 修改器聚合），
/// 检测"有效值（经免疫压制）0↔正"跳变，生成 ControlStateChangedEvent（业务监听）
/// 与 ControlStateNativeRequest（Native 副作用），并维护单位上的 ControlStateSnapshot。
/// Pause 是合成位：raw(Pause) ‖ effectiveStun ‖ effectiveCrackFly，任一为真即暂停，
/// 由本系统单独合成判定并只发一次 Pause 请求；纯 Pause 不经免疫，直读 finalValue。
/// </summary>
/// <remarks>
/// order 46：位于 AttrCalculationSystem（45）之后，保证读到已重算的 finalValue；
/// 位于效果结算（100+）之前。仅处理"有控制 / 免疫 / Pause 属性"的单位，避免全量扫描。
/// </remarks>
[SystemRegister(SystemKind.Interval, 46)]
public class ControlStateTransitionSystem : QuerySystem<AttrTypeId, AttrOwner>
{
    /// <summary>
    /// 控制属性映射表（权威表在 <see cref="ControlHelper.ControlAttrs"/>，与免疫查询共用一份）。
    /// Stun / CrackFly 在该表内（用于广播事实事件与免疫压制），原生暂停动作由 Pause 合成承担；
    /// Pause 不进该表，由合成判定单独计算，避免与主循环重复发请求。
    /// </summary>
    private static ControlHelper.ControlAttrEntry[] ControlAttrs => ControlHelper.ControlAttrs;

    /// <summary>本 tick 需要检测的单位集合（复用避免分配）。</summary>
    private readonly HashSet<Entity> _units = new();

    /// <summary>
    /// 绑定的 EntityStore（首次收集时从单位实体缓存）。
    /// 不依赖 Game.Store 静态全局，本地验证场景可用独立 store 驱动本系统。
    /// </summary>
    private EntityStore? _store;

    protected override void OnUpdate()
    {
        // 1. 收集"有控制 / 免疫 / Pause 属性"的单位：任一类属性的增减都会影响检测或合成结果。
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

            var changed = false;

            foreach (var entry in ControlAttrs)
            {
                var controlType = entry.ControlType;
                var active = ControlHelper.GetEffectiveValue(unit, entry.AttrId) > 0f;
                if (snapshot.IsActive(controlType) == active)
                    continue;

                snapshot.SetActive(controlType, active);
                changed = true;

                // 事实事件照常广播（眩晕/击飞特效与位移由业务层监听）。
                CreateChangedEvent(unit, controlType, active);

                // Stun / CrackFly 的原生暂停由下方 Pause 合成统一承担，不各自发 native 请求。
                if (controlType != ControlType.Stun && controlType != ControlType.CrackFly)
                    CreateNativeRequest(unit, controlType, active);
            }

            // Pause 合成判定：raw(Pause) ‖ effective(Stun) ‖ effective(CrackFly)。
            // 纯 Pause 不经免疫直读 finalValue；Stun/CrackFly 经各自免疫压制（免疫生效即不暂停）。
            // 每 tick 用"当前 effective ↔ 上一帧 Pause 快照位"对比，不累积两帧误差；
            // 同帧内多个控制变化只在此处合成一次请求，避免抖动。
            var pauseActive = ControlHelper.GetEffectiveValue(unit, AttributeHelper.Stun) > 0f
                              || ControlHelper.GetEffectiveValue(unit, AttributeHelper.CrackFly) > 0f
                              || AttributeHelper.GetFinalValue(unit, AttributeHelper.Pause) > 0f;

            if (snapshot.IsActive(ControlType.Pause) != pauseActive)
            {
                snapshot.SetActive(ControlType.Pause, pauseActive);
                changed = true;
                CreateNativeRequest(unit, ControlType.Pause, pauseActive);
            }

            if (changed)
                unit.AddComponent(snapshot);
        }

        // 3. 清理：不再持有任何控制 / 免疫 / Pause 属性的单位移除快照（单位销毁时组件自动清除，此处处理属性被整体移除的情况）。
        // 若有残留激活位（属性整体移除前仍处于控制中），先补发解除事件 + 请求，保证原生状态收敛（避免永久暂停）。
        // 收集后统一处理，避免在迭代中做结构变更。store 为 null 时（从未有控制属性单位）无需清理。
        var toRemove = default(List<Entity>);
        var toRelease = default(List<(Entity unit, ushort bits)>);
        if (_store != null)
        {
            _store.Query<ControlStateSnapshot>().ForEachEntity((ref ControlStateSnapshot snapshot, Entity unit) =>
            {
                if (_units.Contains(unit))
                    return;

                if (snapshot.bits != 0)
                {
                    toRelease ??= new List<(Entity, ushort)>();
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
                    if ((bits & ControlStateSnapshot.BitOf(entry.ControlType)) == 0)
                        continue;

                    CreateChangedEvent(unit, entry.ControlType, false);

                    if (entry.ControlType != ControlType.Stun && entry.ControlType != ControlType.CrackFly)
                        CreateNativeRequest(unit, entry.ControlType, false);
                }

                // 合成位同样收敛：属性被整体移除后必须恢复原生暂停，否则会永久暂停。
                if ((bits & ControlStateSnapshot.BitOf(ControlType.Pause)) != 0)
                    CreateNativeRequest(unit, ControlType.Pause, false);
            }
        }

        if (toRemove != null)
        {
            foreach (var unit in toRemove)
                unit.RemoveComponent<ControlStateSnapshot>();
        }

        _units.Clear();
    }

    /// <summary>创建控制跳变事实事件（独立事件实体 + 事件清理标记）。</summary>
    private void CreateChangedEvent(Entity unit, ControlType controlType, bool entered)
    {
        _store!.CreateEntity(new ControlStateChangedEvent
        {
            unit = unit,
            controlType = controlType,
            entered = entered,
        }).AddComponent(new TriggerEventMarker
        {
            eventTypeId = EventTypeRegistry.Get<ControlStateChangedEvent>()
        });
    }

    /// <summary>创建原生副作用请求（一次性意图，由 UnitControlNativeSystem 消费后删除）。</summary>
    private void CreateNativeRequest(Entity unit, ControlType controlType, bool entered)
    {
        _store!.CreateEntity(new ControlStateNativeRequest
        {
            unit = unit,
            controlType = controlType,
            entered = entered,
        });
    }

    /// <summary>判断属性类型是否属于控制 / 免疫 / 纯 Pause 合成源。</summary>
    private static bool IsControlOrImmunity(int typeId)
    {
        // 纯 Pause 属性不在 ControlAttrs 主表中（由合成判定单独消费），但必须参与收集；
        // 否则"只有 Pause 属性、无眩晕/击飞"的单位永远不会进入检测。
        if (typeId == AttributeHelper.Pause)
            return true;

        foreach (var entry in ControlAttrs)
        {
            if (typeId == entry.AttrId || typeId == entry.ImmunityAttrId)
                return true;
        }

        return false;
    }
}
