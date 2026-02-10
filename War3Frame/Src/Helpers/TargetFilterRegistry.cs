using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 目标筛选注册表 - 支持注册自定义过滤函数
/// 预设的 TargetFilter Flags 负责常用过滤（阵营、类型、状态）
/// 自定义筛选器负责复杂逻辑（如：只选择血量低于 50% 的目标）
/// </summary>
public static class TargetFilterRegistry
{
    /// <summary>
    /// 自定义筛选委托
    /// 参数：(施法者, 候选目标) → 是否通过筛选
    /// </summary>
    public delegate bool FilterFunc(Entity caster, Entity target);

    /// <summary>已注册的自定义筛选器</summary>
    private static readonly Dictionary<string, FilterFunc> _filters = new();

    #region 注册与移除

    /// <summary>
    /// 注册自定义筛选器
    /// </summary>
    /// <param name="filterId">筛选器 ID</param>
    /// <param name="filter">筛选函数</param>
    public static void Register(string filterId, FilterFunc filter)
    {
        _filters[filterId] = filter;
    }

    /// <summary>
    /// 移除自定义筛选器
    /// </summary>
    public static void Unregister(string filterId)
    {
        _filters.Remove(filterId);
    }

    #endregion

    #region 查询

    /// <summary>
    /// 获取自定义筛选器
    /// </summary>
    public static FilterFunc? Get(string filterId)
    {
        return _filters.GetValueOrDefault(filterId);
    }

    /// <summary>
    /// 检查目标是否通过自定义筛选
    /// 如果 filterId 为空或未注册，默认通过
    /// </summary>
    public static bool PassCustomFilter(string? filterId, Entity caster, Entity target)
    {
        if (string.IsNullOrEmpty(filterId)) return true;
        var filter = Get(filterId);
        return filter == null || filter(caster, target);
    }

    #endregion

    #region 预设筛选逻辑

    /// <summary>
    /// 检查目标是否通过 Flags 预设筛选
    /// TODO: 根据你的单位组件实现具体判断逻辑
    /// </summary>
    public static bool PassPresetFilter(TargetFilter filter, Entity caster, Entity target)
    {
        // 如果未设置任何筛选条件，全部通过
        if (filter == TargetFilter.None) return true;

        // ========== 阵营检查 ==========
        bool hasTeamFilter = filter.HasFlag(TargetFilter.Self) ||
                             filter.HasFlag(TargetFilter.Ally) ||
                             filter.HasFlag(TargetFilter.Enemy) ||
                             filter.HasFlag(TargetFilter.Neutral);

        if (hasTeamFilter)
        {
            bool teamPass = false;

            // 自己
            if (filter.HasFlag(TargetFilter.Self) && target == caster)
                teamPass = true;

            // 友方（需要实现阵营判断）
            if (!teamPass && filter.HasFlag(TargetFilter.Ally) && target != caster)
                teamPass = IsAlly(caster, target);

            // 敌方
            if (!teamPass && filter.HasFlag(TargetFilter.Enemy))
                teamPass = IsEnemy(caster, target);

            // 中立
            if (!teamPass && filter.HasFlag(TargetFilter.Neutral))
                teamPass = IsNeutral(target);

            if (!teamPass) return false;
        }

        // ========== 单位类型检查 ==========
        bool hasTypeFilter = filter.HasFlag(TargetFilter.Hero) ||
                             filter.HasFlag(TargetFilter.Normal) ||
                             filter.HasFlag(TargetFilter.Building) ||
                             filter.HasFlag(TargetFilter.Summon) ||
                             filter.HasFlag(TargetFilter.Ward);

        if (hasTypeFilter)
        {
            bool typePass = false;

            // TODO: 根据你的单位类型组件判断
            // 示例：
            // if (filter.HasFlag(TargetFilter.Hero) && target.Tags.Has<HeroTag>())
            //     typePass = true;
            // if (filter.HasFlag(TargetFilter.Normal) && !target.Tags.Has<HeroTag>() && !target.Tags.Has<BuildingTag>())
            //     typePass = true;

            // 暂时默认通过（等你添加单位类型标签后启用上面的逻辑）
            typePass = true;

            if (!typePass) return false;
        }

        // ========== 状态检查 ==========
        if (filter.HasFlag(TargetFilter.Alive))
        {
            // TODO: 检查单位是否存活
            // if (target.Tags.Has<Dead>()) return false;
        }

        if (filter.HasFlag(TargetFilter.Dead))
        {
            // TODO: 检查单位是否死亡
            // if (!target.Tags.Has<Dead>()) return false;
        }

        if (filter.HasFlag(TargetFilter.MagicImmune))
        {
            // TODO: 检查单位是否魔免
        }

        return true;
    }

    /// <summary>
    /// 综合检查：预设筛选 + 自定义筛选（AND 关系）
    /// </summary>
    public static bool PassFilter(TargetFilter presetFilter, string? customFilterId,
        Entity caster, Entity target)
    {
        // 先过预设筛选
        if (!PassPresetFilter(presetFilter, caster, target))
            return false;

        // 再过自定义筛选
        if (!PassCustomFilter(customFilterId, caster, target))
            return false;

        return true;
    }

    #endregion

    #region 阵营判断（需要根据你的实现修改）

    /// <summary>是否是友方</summary>
    private static bool IsAlly(Entity caster, Entity target)
    {
        // TODO: 根据你的玩家/阵营系统实现
        // 例如检查 Owner 组件的 playerId 是否友方
        return false;
    }

    /// <summary>是否是敌方</summary>
    private static bool IsEnemy(Entity caster, Entity target)
    {
        // TODO: 根据你的玩家/阵营系统实现
        return true; // 默认都是敌方
    }

    /// <summary>是否是中立</summary>
    private static bool IsNeutral(Entity target)
    {
        // TODO: 根据你的玩家/阵营系统实现
        return false;
    }

    #endregion
}
