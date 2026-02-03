using Friflo.Engine.ECS;

namespace War3Frame.Factories;

/// <summary>
///     技能工厂 - 提供技能的创建、移除、交换等操作
/// </summary>
public static class AbilityFactory
{
    /// <summary>
    ///     为单位添加技能到指定槽位
    /// </summary>
    /// <param name="store">EntityStore</param>
    /// <param name="unit">目标单位 Entity</param>
    /// <param name="abilityId">技能类型 ID</param>
    /// <param name="slotIndex">目标槽位索引</param>
    /// <param name="configure">可选的额外配置</param>
    /// <returns>创建的技能 Entity</returns>
    /// <exception cref="InvalidOperationException">槽位已被占用或超出范围</exception>
    public static Entity AddAbilityToSlot(
        EntityStore store,
        Entity unit,
        string templateName,
        int slotIndex,
        Action<Entity>? configure = null)
    {
        // 1. 检查单位是否有技能槽容器
        if (!unit.TryGetComponent<AbilitySlotContainer>(out var container))
            throw new InvalidOperationException($"单位 {unit.Id} 没有 AbilitySlotContainer 组件");

        // 2. 检查槽位是否在有效范围内
        if (slotIndex < 0 || slotIndex >= container.maxSlots)
            throw new InvalidOperationException($"槽位索引 {slotIndex} 超出范围 [0, {container.maxSlots})");

        // 3. 检查槽位是否已被占用
        if (IsSlotOccupied(unit, slotIndex)) throw new InvalidOperationException($"槽位 {slotIndex} 已被占用");

        // 4. 创建技能 Entity
        var ability = store.CreateEntity(
            new AbilityBase
            {
                templateName = templateName,
                level = 1,
                state = AbilityState.Ready
            },
            new AbilitySlotIndex
            {
                slotIndex = slotIndex
            }
        );

        // 5. 添加关系到单位
        ability.AddComponent(new AbilityOwner(unit));

        // 6. 更新单位的槽位计数
        container.currentCount++;
        unit.AddComponent(container);

        // 7. 应用额外配置
        configure?.Invoke(ability);

        return ability;
    }

    /// <summary>
    ///     从槽位移除技能
    /// </summary>
    /// <param name="unit">目标单位 Entity</param>
    /// <param name="slotIndex">要移除的槽位索引</param>
    /// <returns>是否成功移除</returns>
    public static bool RemoveAbilityFromSlot(Entity unit, int slotIndex)
    {
        var ability = GetAbilityAtSlot(unit, slotIndex);
        if (ability == null) return false;

        // 更新槽位计数
        if (unit.TryGetComponent<AbilitySlotContainer>(out var container))
        {
            container.currentCount = Math.Max(0, container.currentCount - 1);
            unit.AddComponent(container);
        }

        // 删除技能 Entity
        ability.Value.DeleteEntity();
        return true;
    }

    /// <summary>
    ///     交换两个槽位的技能
    /// </summary>
    /// <param name="unit">目标单位 Entity</param>
    /// <param name="slotA">槽位 A 索引</param>
    /// <param name="slotB">槽位 B 索引</param>
    public static void SwapAbilities(Entity unit, int slotA, int slotB)
    {
        if (slotA == slotB) return;

        var abilityA = GetAbilityAtSlot(unit, slotA);
        var abilityB = GetAbilityAtSlot(unit, slotB);

        // 交换槽位索引
        if (abilityA.HasValue)
        {
            var indexA = abilityA.Value.GetComponent<AbilitySlotIndex>();
            indexA.slotIndex = slotB;
            abilityA.Value.AddComponent(indexA);
        }

        if (abilityB.HasValue)
        {
            var indexB = abilityB.Value.GetComponent<AbilitySlotIndex>();
            indexB.slotIndex = slotA;
            abilityB.Value.AddComponent(indexB);
        }
    }

    /// <summary>
    ///     扩展槽位数量
    /// </summary>
    /// <param name="unit">目标单位 Entity</param>
    /// <param name="additionalSlots">要增加的槽位数</param>
    public static void ExpandSlots(Entity unit, int additionalSlots)
    {
        if (additionalSlots <= 0) return;

        if (unit.TryGetComponent<AbilitySlotContainer>(out var container))
        {
            container.maxSlots += additionalSlots;
            unit.AddComponent(container);
        }
    }

    /// <summary>
    ///     设置槽位数量（可增可减，但不能低于当前已使用数量）
    /// </summary>
    /// <param name="unit">目标单位 Entity</param>
    /// <param name="newMaxSlots">新的最大槽位数</param>
    public static void SetMaxSlots(Entity unit, int newMaxSlots)
    {
        if (unit.TryGetComponent<AbilitySlotContainer>(out var container))
        {
            if (newMaxSlots < container.currentCount)
                throw new InvalidOperationException(
                    $"无法将槽位数设置为 {newMaxSlots}，当前已使用 {container.currentCount} 个槽位");
            container.maxSlots = newMaxSlots;
            unit.AddComponent(container);
        }
    }

    /// <summary>
    ///     获取指定槽位的技能
    /// </summary>
    /// <param name="unit">目标单位 Entity</param>
    /// <param name="slotIndex">槽位索引</param>
    /// <returns>技能 Entity，如果槽位为空则返回 null</returns>
    public static Entity? GetAbilityAtSlot(Entity unit, int slotIndex)
    {
        var links = unit.GetIncomingLinks<AbilityOwner>();
        foreach (var link in links)
        {
            var abilityEntity = link.Entity;
            if (abilityEntity.TryGetComponent<AbilitySlotIndex>(out var index) && index.slotIndex == slotIndex)
                return abilityEntity;
        }

        return null;
    }

    /// <summary>
    ///     检查槽位是否已被占用
    /// </summary>
    /// <param name="unit">目标单位 Entity</param>
    /// <param name="slotIndex">槽位索引</param>
    /// <returns>是否已被占用</returns>
    public static bool IsSlotOccupied(Entity unit, int slotIndex)
    {
        return GetAbilityAtSlot(unit, slotIndex) != null;
    }

    /// <summary>
    ///     获取单位的所有技能
    /// </summary>
    /// <param name="unit">目标单位 Entity</param>
    /// <returns>技能 Entity 列表</returns>
    public static List<Entity> GetAllAbilities(Entity unit)
    {
        var abilities = new List<Entity>();
        var links = unit.GetIncomingLinks<AbilityOwner>();
        foreach (var link in links) abilities.Add(link.Entity);
        return abilities;
    }

    /// <summary>
    ///     获取第一个空闲槽位索引
    /// </summary>
    /// <param name="unit">目标单位 Entity</param>
    /// <returns>空闲槽位索引，如果没有空闲槽位则返回 -1</returns>
    public static int GetFirstFreeSlot(Entity unit)
    {
        if (!unit.TryGetComponent<AbilitySlotContainer>(out var container)) return -1;

        for (var i = 0; i < container.maxSlots; i++)
            if (!IsSlotOccupied(unit, i))
                return i;

        return -1;
    }

    /// <summary>
    ///     添加技能到第一个空闲槽位
    /// </summary>
    /// <param name="store">EntityStore</param>
    /// <param name="unit">目标单位 Entity</param>
    /// <param name="templateName">技能模板名称</param>
    /// <param name="configure">可选的额外配置</param>
    /// <returns>创建的技能 Entity</returns>
    /// <exception cref="InvalidOperationException">没有空闲槽位</exception>
    public static Entity AddAbility(
        EntityStore store,
        Entity unit,
        string templateName,
        Action<Entity>? configure = null)
    {
        var freeSlot = GetFirstFreeSlot(unit);
        if (freeSlot < 0) throw new InvalidOperationException($"单位 {unit.Id} 没有空闲的技能槽位");
        return AddAbilityToSlot(store, unit, templateName, freeSlot, configure);
    }
}