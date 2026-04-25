using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 区域搜索辅助类 - 提供各种形状的目标搜索
/// 内部使用 SpatialGrid 网格分区加速查询
/// 自动集成 TargetFilterRegistry 进行目标筛选
/// </summary>
public static class GroupHelper
{
    /// <summary>全局空间网格（由 SpatialGridSystem 每帧重建）</summary>
    public static SpatialGrid Grid { get; } = new(256f);

    /// <summary>复用的临时列表（避免每次查询分配新 List）</summary>
    private static readonly List<Entity> _tempBuffer = new(64);

    private static List<Entity> GetBuffer()
    {
        _tempBuffer.Clear();
        return _tempBuffer;
    }

    #region 圆形搜索

    /// <summary>
    /// 在圆形区域内搜索目标
    /// </summary>
    /// <param name="caster">施法者（用于阵营判断）</param>
    /// <param name="cx">中心 X</param>
    /// <param name="cy">中心 Y</param>
    /// <param name="radius">半径</param>
    /// <param name="filter">预设筛选</param>
    /// <param name="customFilterId">自定义筛选器 ID（可选）</param>
    /// <param name="maxTargets">最大目标数（0 = 无限制）</param>
    public static List<Entity> FindInCircle(
        Entity caster, float cx, float cy, float radius,
        TargetFilter filter = TargetFilter.None,
        string? customFilterId = null,
        int maxTargets = 0)
    {
        var buffer = GetBuffer();
        Grid.QueryCircle(cx, cy, radius, buffer);

        return ApplyFilter(buffer, caster, filter, customFilterId, maxTargets);
    }

    #endregion

    #region 矩形搜索

    /// <summary>
    /// 在矩形区域内搜索目标
    /// </summary>
    /// <param name="caster">施法者</param>
    /// <param name="cx">中心 X</param>
    /// <param name="cy">中心 Y</param>
    /// <param name="halfW">半宽</param>
    /// <param name="halfH">半高</param>
    /// <param name="filter">预设筛选</param>
    /// <param name="customFilterId">自定义筛选器 ID</param>
    /// <param name="maxTargets">最大数量</param>
    public static List<Entity> FindInRect(
        Entity caster, float cx, float cy, float halfW, float halfH,
        TargetFilter filter = TargetFilter.None,
        string? customFilterId = null,
        int maxTargets = 0)
    {
        var buffer = GetBuffer();
        Grid.QueryRect(cx, cy, halfW, halfH, buffer);

        return ApplyFilter(buffer, caster, filter, customFilterId, maxTargets);
    }

    #endregion

    #region 线形搜索

    /// <summary>
    /// 在线段范围内搜索目标（用于线性弹道）
    /// </summary>
    /// <param name="caster">施法者</param>
    /// <param name="startX">起点 X</param>
    /// <param name="startY">起点 Y</param>
    /// <param name="endX">终点 X</param>
    /// <param name="endY">终点 Y</param>
    /// <param name="width">线宽（碰撞检测范围）</param>
    /// <param name="filter">预设筛选</param>
    /// <param name="customFilterId">自定义筛选器 ID</param>
    /// <param name="maxTargets">最大数量</param>
    public static List<Entity> FindInLine(
        Entity caster, float startX, float startY, float endX, float endY,
        float width,
        TargetFilter filter = TargetFilter.None,
        string? customFilterId = null,
        int maxTargets = 0)
    {
        var buffer = GetBuffer();
        Grid.QueryLine(startX, startY, endX, endY, width, buffer);

        return ApplyFilter(buffer, caster, filter, customFilterId, maxTargets);
    }

    #endregion

    #region 扇形搜索

    /// <summary>
    /// 在扇形区域内搜索目标
    /// </summary>
    /// <param name="caster">施法者</param>
    /// <param name="cx">扇形顶点 X</param>
    /// <param name="cy">扇形顶点 Y</param>
    /// <param name="dirX">朝向 X（归一化）</param>
    /// <param name="dirY">朝向 Y（归一化）</param>
    /// <param name="radius">扇形半径</param>
    /// <param name="angleDeg">扇形角度（度数，例如 60 表示 ±30°）</param>
    /// <param name="filter">预设筛选</param>
    /// <param name="customFilterId">自定义筛选器 ID</param>
    /// <param name="maxTargets">最大数量</param>
    public static List<Entity> FindInCone(
        Entity caster, float cx, float cy, float dirX, float dirY,
        float radius, float angleDeg,
        TargetFilter filter = TargetFilter.None,
        string? customFilterId = null,
        int maxTargets = 0)
    {
        var buffer = GetBuffer();
        float halfAngle = angleDeg * 0.5f * MathF.PI / 180f;
        Grid.QueryCone(cx, cy, dirX, dirY, radius, halfAngle, buffer);

        return ApplyFilter(buffer, caster, filter, customFilterId, maxTargets);
    }

    #endregion

    #region 最近目标

    /// <summary>
    /// 查找最近的目标
    /// </summary>
    /// <returns>最近的目标 Entity，如果没有则返回 default</returns>
    public static Entity FindNearest(
        Entity caster, float cx, float cy, float radius,
        TargetFilter filter = TargetFilter.None,
        string? customFilterId = null)
    {
        var targets = FindInCircle(caster, cx, cy, radius, filter, customFilterId);

        Entity nearest = default;
        float nearestDistSq = float.MaxValue;

        foreach (var target in targets)
        {
            if (!target.TryGetComponent<Position>(out var pos)) continue;

            float dx = pos.x - cx;
            float dy = pos.y - cy;
            float distSq = dx * dx + dy * dy;

            if (distSq < nearestDistSq)
            {
                nearestDistSq = distSq;
                nearest = target;
            }
        }

        return nearest;
    }

    #endregion

    #region 内部方法

    /// <summary>
    /// 对查询结果应用 TargetFilter 筛选
    /// </summary>
    private static List<Entity> ApplyFilter(
        List<Entity> raw, Entity caster,
        TargetFilter filter, string? customFilterId,
        int maxTargets)
    {
        // 无筛选且无限制，直接返回副本
        if (filter == TargetFilter.None && string.IsNullOrEmpty(customFilterId) && maxTargets <= 0)
        {
            return new List<Entity>(raw);
        }

        var results = new List<Entity>();
        foreach (var entity in raw)
        {
            // 跳过施法者本身（除非 filter 包含 Self）
            if (entity == caster && !filter.HasFlag(TargetFilter.Self))
                continue;

            // 应用预设 + 自定义筛选
            if (filter != TargetFilter.None || !string.IsNullOrEmpty(customFilterId))
            {
                if (!TargetFilterRegistry.PassFilter(filter, customFilterId, caster, entity))
                    continue;
            }

            results.Add(entity);

            if (maxTargets > 0 && results.Count >= maxTargets)
                break;
        }

        return results;
    }

    #endregion
}