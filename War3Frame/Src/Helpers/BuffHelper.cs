using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame;

/// <summary>
///     Buff Tick 行为接口
/// </summary>
public interface IBuffTickAction
{
    void Execute(Entity buffEntity, Entity target);
}

/// <summary>
///     Buff Tick 行为注册表
/// </summary>
public static class BuffTickActionRegistry
{
    private static readonly Dictionary<string, IBuffTickAction> _actions = new();

    static BuffTickActionRegistry()
    {
        // 注册内置行为
        Register("DealDamage", new DealDamageTickAction());
    }

    public static void Register(string id, IBuffTickAction action)
    {
        _actions[id] = action;
    }

    public static IBuffTickAction? Get(string? id)
    {
        return id != null && _actions.TryGetValue(id, out var action) ? action : null;
    }
}

/// <summary>
///     内置 Tick 行为：发送伤害请求
/// </summary>
internal class DealDamageTickAction : IBuffTickAction
{
    public void Execute(Entity buffEntity, Entity target)
    {
        if (!buffEntity.TryGetComponent<Buff>(out var buff))
        {
            return;
        }

        var store = buffEntity.Store;
        // 伤害来源：优先 Buff.caster（创建时解析的施法者单位）；回退 ModifySource.source
        Entity sourceUnit = !buff.caster.IsNull ? buff.caster : default;
        if (sourceUnit.IsNull && buffEntity.TryGetComponent<ModifySource>(out var source))
        {
            sourceUnit = source.source;
        }
        var damage = buff.tickValue;

        if (sourceUnit.IsNull)
        {
            return; // 无有效来源，跳过（避免产生无源伤害）
        }

        // 创建伤害请求
        store.CreateEntity(
            new DamageRequest
            {
                damage = new DamageBase
                {
                    damage = damage,
                    damageType = DamageType.Real, // DoT 默认真实伤害
                    damageSrc = DamageSrc.Skill,
                    source = sourceUnit,
                    target = target
                }
            }
        );
    }
}

/// <summary>
///     Buff 规格（数据容器）
/// </summary>
public readonly struct BuffSpec
{
    public readonly string buffId;
    public readonly string? icon;
    public readonly int attrTypeId;
    public readonly ModifyType modifyType;
    public readonly float value;
    public readonly float duration;
    public readonly int maxStacks;
    public readonly BuffRefreshBehavior onDuplicate;
    public readonly float tickInterval;
    public readonly string? tickActionId;
    public readonly BuffTag tags;
    /// <summary>每跳数值；BuffKind.Tick 时作为每跳伤害，不参与属性贡献</summary>
    public readonly float tickValue;
    /// <summary>实体类型（Attribute/Tick/PureTag），构造时定型</summary>
    public readonly BuffKind kind;

    public BuffSpec(
        string buffId,
        string? icon,
        int attrTypeId,
        ModifyType modifyType,
        float value,
        float duration,
        int maxStacks,
        BuffRefreshBehavior onDuplicate,
        float tickInterval,
        string? tickActionId,
        BuffTag tags,
        float tickValue = 0f,
        BuffKind kind = BuffKind.Attribute)
    {
        this.buffId = buffId;
        this.icon = icon;
        this.attrTypeId = attrTypeId;
        this.modifyType = modifyType;
        this.value = value;
        this.duration = duration;
        this.maxStacks = maxStacks;
        this.onDuplicate = onDuplicate;
        this.tickInterval = tickInterval;
        this.tickActionId = tickActionId;
        this.tags = tags;
        this.tickValue = tickValue;
        this.kind = kind;
    }
}

/// <summary>
///     Buff 辅助类 - 提供便捷的 Buff 操作
/// </summary>
public static class BuffHelper
{
    private static long _nextBuffId = 0;
    #region 添加 Buff

    /// <summary>
    ///     为单位添加一个简单的限时 Buff
    /// </summary>
    public static Entity AddTimedBuff(
        EntityStore store,
        Entity unit,
        Entity source,
        string buffId,
        int attrTypeId,
        ModifyType modifyType,
        float value,
        float duration,
        BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshDuration)
    {
        return ApplyBuff(store, unit, source, new BuffSpec(
            buffId: buffId,
            icon: null,
            attrTypeId: attrTypeId,
            modifyType: modifyType,
            value: value,
            duration: duration,
            maxStacks: 1,
            onDuplicate: refreshBehavior,
            tickInterval: 0f,
            tickActionId: null,
            tags: BuffTag.None
        ));
    }

    /// <summary>
    ///     添加可堆叠的 Buff
    /// </summary>
    public static Entity AddStackableBuff(
        EntityStore store,
        Entity unit,
        Entity source,
        string buffId,
        int attrTypeId,
        ModifyType modifyType,
        float valuePerStack,
        int maxStacks,
        float duration,
        BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshAndStack)
    {
        return ApplyBuff(store, unit, source, new BuffSpec(
            buffId: buffId,
            icon: null,
            attrTypeId: attrTypeId,
            modifyType: modifyType,
            value: valuePerStack,
            duration: duration,
            maxStacks: maxStacks,
            onDuplicate: refreshBehavior,
            tickInterval: 0f,
            tickActionId: null,
            tags: BuffTag.None
        ));
    }

    /// <summary>
    ///     添加永久 Buff（不会自动消失）
    /// </summary>
    public static Entity AddPermanentBuff(
        EntityStore store,
        Entity unit,
        Entity source,
        string buffId,
        int attrTypeId,
        ModifyType modifyType,
        float value)
    {
        return ApplyBuff(store, unit, source, new BuffSpec(
            buffId: buffId,
            icon: null,
            attrTypeId: attrTypeId,
            modifyType: modifyType,
            value: value,
            duration: -1f,
            maxStacks: 1,
            onDuplicate: BuffRefreshBehavior.Independent,
            tickInterval: 0f,
            tickActionId: null,
            tags: BuffTag.None
        ));
    }

    #endregion

    #region 移除 Buff

    /// <summary>
    ///     移除单位身上的指定 Buff
    /// </summary>
    public static void RemoveBuff(Entity unit, string buffId)
    {
        var buff = FindBuffByIdOnUnit(unit, buffId);
        if (!buff.IsNull)
        {
            // 标记属性需要刷新
            if (buff.TryGetComponent<ModifyTarget>(out var target) && !target.target.IsNull)
            {
                target.target.AddTag<AttrDirty>();
            }
            buff.DeleteEntity();
        }
    }

    /// <summary>
    ///     移除单位的所有 Buff
    /// </summary>
    public static void RemoveAllBuffs(Entity unit)
    {
        // 获取单位的所有属性
        var attrs = AttributeHelper.GetAllAttrs(unit);
        var affectedAttrs = new HashSet<Entity>();
        var toDelete = new List<Entity>();

        foreach (var (typeId, attrEntity) in attrs)
        {
            // 获取指向该属性的所有修改器
            var modifiers = attrEntity.GetIncomingLinks<ModifyTarget>();
            foreach (var link in modifiers)
            {
                if (link.Entity.TryGetComponent<Buff>(out _))
                {
                    toDelete.Add(link.Entity);
                    affectedAttrs.Add(attrEntity);
                }
            }
        }

        foreach (var buff in toDelete)
        {
            buff.DeleteEntity();
        }

        foreach (var attr in affectedAttrs)
        {
            if (!attr.IsNull)
            {
                attr.AddTag<AttrDirty>();
            }
        }
    }

    #endregion

    #region 查询

    /// <summary>
    ///     查找单位身上的指定 Buff
    /// </summary>
    public static Entity FindBuffByIdOnUnit(Entity unit, string buffId)
    {
        // 遍历单位的所有属性，查找 Buff
        var attrs = AttributeHelper.GetAllAttrs(unit);

        foreach (var (typeId, attrEntity) in attrs)
        {
            var modifiers = attrEntity.GetIncomingLinks<ModifyTarget>();
            foreach (var link in modifiers)
            {
                var buffEntity = link.Entity;
                if (buffEntity.TryGetComponent<Buff>(out var buff) && buff.buffId == buffId)
                {
                    return buffEntity;
                }
            }
        }

        return default;
    }

    /// <summary>
    ///     检查单位是否有指定 Buff
    /// </summary>
    public static bool HasBuff(Entity unit, string buffId)
    {
        return !FindBuffByIdOnUnit(unit, buffId).IsNull;
    }

    /// <summary>
    ///     获取 Buff 剩余时间
    /// </summary>
    public static float GetBuffRemainingTime(Entity unit, string buffId)
    {
        var buff = FindBuffByIdOnUnit(unit, buffId);
        if (buff.IsNull) return 0;

        if (buff.TryGetComponent<Duration>(out var duration))
        {
            return duration.remaining < 0f ? -1f : duration.remaining;
        }
        return 0;
    }

    /// <summary>
    ///     获取 Buff 当前层数
    /// </summary>
    public static int GetBuffStacks(Entity unit, string buffId)
    {
        var buff = FindBuffByIdOnUnit(unit, buffId);
        if (buff.IsNull) return 0;

        if (buff.TryGetComponent<BuffStacks>(out var stacks))
        {
            return stacks.current;
        }
        return 1;  // 没有层数组件视为 1 层
    }

    #endregion

    #region 统一工厂方法

    /// <summary>
    ///     应用 Buff（统一入口）
    /// </summary>
    public static Entity ApplyBuff(EntityStore store, Entity unit, Entity source, BuffSpec spec)
    {
        // 检查是否已有同类 Buff
        var existing = FindBuffByIdOnUnit(unit, spec.buffId);

        if (!existing.IsNull)
        {
            return HandleExistingBuff(store, unit, source, existing, spec);
        }

        return CreateBuffInternal(store, unit, source, spec);
    }

    /// <summary>
    ///     给 buff 贡献的属性打脏（触发重算）。
    /// </summary>
    private static void DirtyAttr(Entity buffEntity)
    {
        if (buffEntity.TryGetComponent<ModifyTarget>(out var target) && !target.target.IsNull)
        {
            target.target.AddTag<AttrDirty>();
        }
    }

    /// <summary>
    ///     处理已存在的 Buff（统一刷新逻辑）
    ///     决策依据是新请求的 onDuplicate，而非旧 buff 上记录的 refreshBehavior。
    /// </summary>
    private static Entity HandleExistingBuff(
        EntityStore store, Entity unit, Entity source, Entity existing, BuffSpec spec)
    {
        if (spec.duration <= 0f && spec.onDuplicate != BuffRefreshBehavior.Replace)
        {
            // 非 Replace 的永久 buff：同 key 唯一，返回既有即可
            return existing;
        }

        switch (spec.onDuplicate)
        {
            case BuffRefreshBehavior.Independent:
                // 独立存在，不做任何处理
                return existing;

            case BuffRefreshBehavior.Replace:
                // 删旧建新：以新 spec 完整生效
                DirtyAttr(existing);
                existing.DeleteEntity();
                return CreateBuffInternal(store, unit, source, spec);

            case BuffRefreshBehavior.ReplaceIfLonger:
                // 独占控制：仅当新 duration 更长时才替换（取最晚结束时间）
                if (existing.TryGetComponent<Duration>(out var oldDuration) &&
                    oldDuration.remaining >= 0f && spec.duration > oldDuration.remaining)
                {
                    DirtyAttr(existing);
                    existing.DeleteEntity();
                    return CreateBuffInternal(store, unit, source, spec);
                }
                // 旧 buff 剩余更长，保持旧的
                return existing;

            case BuffRefreshBehavior.RefreshDuration:
                // 刷新时长（用新 spec 的 duration，修正 P2）
                RefreshCore(existing, spec, refresh: true, stack: false);
                break;

            case BuffRefreshBehavior.AddStack:
                // 仅叠层
                RefreshCore(existing, spec, refresh: false, stack: true);
                break;

            case BuffRefreshBehavior.RefreshAndStack:
                // 刷新 + 叠层
                RefreshCore(existing, spec, refresh: true, stack: true);
                break;
        }

        return existing;
    }

    /// <summary>
    ///     刷新核心：按需刷新时长（Duration.remaining/total 一次写回）与叠层。
    ///     时长刷新用新 spec 的 duration（修正 P2）；值更新按当前层数重算并打脏（P3）。
    ///     叠层型（有 BuffStacks）buff 的属性值由层数决定，刷新时长不改变值。
    /// </summary>
    private static void RefreshCore(Entity existing, BuffSpec spec, bool refresh, bool stack)
    {
        var valueChanged = false;

        if (refresh && existing.TryGetComponent<Duration>(out var duration))
        {
            duration.remaining = spec.duration;
            duration.total = spec.duration;
            existing.AddComponent(duration);
        }

        var hasStacks = existing.TryGetComponent<BuffStacks>(out _);

        if (stack)
        {
            if (existing.TryGetComponent<BuffStacks>(out var stacks) && stacks.AddStack())
            {
                existing.AddComponent(stacks);
                valueChanged = true;
            }
        }
        else if (!hasStacks && existing.TryGetComponent<ModifyValue>(out var mod) && mod.value != spec.value)
        {
            // 非叠层普通 buff：按新 spec 更新值（值不同才写）
            mod.value = spec.value;
            existing.AddComponent(mod);
            valueChanged = true;
        }
        // 叠层型（hasStacks）且非叠层刷新：值由层数决定，保持不变。

        // 叠层后值按总层数重算
        if (stack && valueChanged && existing.TryGetComponent<ModifyValue>(out var stackMod) &&
            existing.TryGetComponent<BuffStacks>(out var updatedStacks))
        {
            stackMod.value = updatedStacks.TotalValue;
            existing.AddComponent(stackMod);
        }

        // 仅值变化需打脏重算（纯时长刷新不改变属性值，无需重算）
        if (valueChanged)
        {
            DirtyAttr(existing);
        }
    }

    /// <summary>
    ///     创建 Buff 实体的核心逻辑（不查重，供 ApplyBuff 与 Replace 重建使用）
    ///     按 BuffSpec.kind 定型：Attribute 挂 ModifyValue；Tick/PureTag 不挂（Tick 的 tick 由行为读 tickValue）。
    /// </summary>
    private static Entity CreateBuffInternal(EntityStore store, Entity unit, Entity source, BuffSpec spec)
    {
        var isDot = spec.kind == BuffKind.Tick;

        // 获取对应的属性 Entity（普通 buff 贡献到该属性；DoT 以该属性作载体供反查/净化）。
        // 单位模板未声明该属性时自动创建 base=0 的属性实体，避免 buff 静默挂不上。
        var attrEntity = AttributeHelper.GetOrCreateAttr(unit, spec.attrTypeId);
        if (attrEntity.IsNull) return default;

        // 生成唯一实例 ID
        var instanceId = System.Threading.Interlocked.Increment(ref _nextBuffId);

        // 创建新 Buff 实体（先挂公共组件，ModifyValue 按 kind 后补）
        var buff = store.CreateEntity(
            new Buff
            {
                buffId = spec.buffId,
                buffInstanceId = instanceId,
                kind = spec.kind,
                tags = spec.tags,
                caster = ResolveCaster(source),
                tickInterval = spec.tickInterval,
                tickActionId = spec.tickActionId,
                lastTick = 0f,
                tickValue = spec.tickValue
            },
            new ModifyTarget(attrEntity),
            new ModifySource(source),
            Duration.Create(spec.duration),
            new BuffBehavior
            {
                icon = spec.icon
            }
        );

        // DoT 型不挂 ModifyValue（避免伤害值污染属性计算）；普通型挂 ModifyValue
        if (!isDot)
        {
            buff.AddComponent(new ModifyValue
            {
                modifyType = spec.modifyType,
                value = spec.value,
                priority = 0
            });
        }

        // 如果是堆叠型 buff，添加 BuffStacks
        if (spec.maxStacks > 1)
        {
            buff.AddComponent(BuffStacks.Create(spec.maxStacks, spec.value));
        }

        attrEntity.AddTag<AttrDirty>();

        return buff;
    }

    /// <summary>
    ///     解析施法者单位：source 本身是单位则返回；否则沿其领域链路解析（如地面区域 caster）。
    ///     无单位来源（如纯区域/光环产生）时返回 default。
    /// </summary>
    private static Entity ResolveCaster(Entity source)
    {
        if (source.IsNull) return default;

        // source 直接是单位（挂有属性关系）→ 返回本身
        if (source.HasComponent<AttrOwner>()) return source;

        // 地面区域实体 → 取 caster
        if (source.TryGetComponent<GroundAreaSource>(out var areaSource))
        {
            return areaSource.caster;
        }

        // 可在此扩展其他产生者链（光环 owner 等）
        return default;
    }

    #endregion

    #region 便捷方法

    /// <summary>
    ///     眩晕目标
    /// </summary>
    public static Entity Stun(EntityStore store, Entity unit, Entity source, float duration, string? icon = null)
    {
        return ApplyBuff(store, unit, source, new BuffSpec(
            buffId: "control:stun",
            icon: icon,
            attrTypeId: AttributeHelper.Stun,
            modifyType: ModifyType.Flat,
            value: 1f,
            duration: duration,
            maxStacks: 1,
            onDuplicate: BuffRefreshBehavior.ReplaceIfLonger,
            tickInterval: 0f,
            tickActionId: null,
            tags: BuffTag.Debuff | BuffTag.Control | BuffTag.Stun
        ));
    }

    /// <summary>
    ///     定身目标
    /// </summary>
    public static Entity Root(EntityStore store, Entity unit, Entity source, float duration, string? icon = null)
    {
        return ApplyBuff(store, unit, source, new BuffSpec(
            buffId: "control:root",
            icon: icon,
            attrTypeId: AttributeHelper.Root,
            modifyType: ModifyType.Flat,
            value: 1f,
            duration: duration,
            maxStacks: 1,
            onDuplicate: BuffRefreshBehavior.ReplaceIfLonger,
            tickInterval: 0f,
            tickActionId: null,
            tags: BuffTag.Debuff | BuffTag.Control | BuffTag.Root
        ));
    }

    /// <summary>
    ///     沉默目标
    /// </summary>
    public static Entity Silence(EntityStore store, Entity unit, Entity source, float duration, string? icon = null)
    {
        return ApplyBuff(store, unit, source, new BuffSpec(
            buffId: "control:silence",
            icon: icon,
            attrTypeId: AttributeHelper.Silence,
            modifyType: ModifyType.Flat,
            value: 1f,
            duration: duration,
            maxStacks: 1,
            onDuplicate: BuffRefreshBehavior.ReplaceIfLonger,
            tickInterval: 0f,
            tickActionId: null,
            tags: BuffTag.Debuff | BuffTag.Control | BuffTag.Silence
        ));
    }

    /// <summary>
    ///     应用持续伤害 Buff（DoT）
    ///     以 Health 属性作载体挂 ModifyTarget（供净化/反查），不挂 ModifyValue（不污染属性），
    ///     每 tick 由 DealDamage 行为读 Buff.tickValue 发 DamageRequest。
    /// </summary>
    public static Entity ApplyDoT(
        EntityStore store,
        Entity unit,
        Entity source,
        string buffId,
        float damagePerTick,
        float tickInterval,
        float duration,
        string? icon = null,
        int carrierAttrTypeId = 0)
    {
        return ApplyBuff(store, unit, source, new BuffSpec(
            buffId: buffId,
            icon: icon,
            attrTypeId: carrierAttrTypeId > 0 ? carrierAttrTypeId : AttributeHelper.Health,
            modifyType: ModifyType.Flat,
            value: 0f, // 不参与属性贡献
            duration: duration,
            maxStacks: 1,
            onDuplicate: BuffRefreshBehavior.RefreshDuration,
            tickInterval: tickInterval,
            tickActionId: "DealDamage",
            tags: BuffTag.Debuff | BuffTag.DoT,
            tickValue: damagePerTick,
            kind: BuffKind.Tick
        ));
    }

    /// <summary>
    ///     净化目标身上所有 Debuff（位运算按 BuffTag.Debuff 判定）
    /// </summary>
    public static void PurgeDebuffs(EntityStore store, Entity unit)
    {
        var toDelete = new List<Entity>();

        // 收集要删的 buff（含 DoT：其 ModifyTarget 载体也在此遍历中）
        var attrs = AttributeHelper.GetAllAttrs(unit);
        foreach (var (typeId, attrEntity) in attrs)
        {
            var modifiers = attrEntity.GetIncomingLinks<ModifyTarget>();
            foreach (var link in modifiers)
            {
                if (link.Entity.TryGetComponent<Buff>(out var buff) &&
                    (buff.tags & BuffTag.Debuff) != 0)
                {
                    toDelete.Add(link.Entity);
                }
            }
        }

        // 删 buff 本身
        foreach (var buff in toDelete)
        {
            buff.DeleteEntity();
        }

        // 触发属性重算
        foreach (var (typeId, attrEntity) in attrs)
        {
            if (!attrEntity.IsNull)
            {
                attrEntity.AddTag<AttrDirty>();
            }
        }
    }

    #endregion
}
