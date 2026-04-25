using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 网格空间分区 - 把地图划分为固定大小的格子
/// 每帧由 SpatialGridSystem 重建，GroupHelper 用来查询
///
/// 工作原理：
///   地图 (0,0)~(mapWidth,mapHeight) → 划分为 cellSize×cellSize 的格子
///   每个格子存储该区域内的所有单位 Entity
///   查询时只需遍历目标区域覆盖的几个格子，而非所有单位
///
/// 性能特点：
///   插入: O(1)
///   查询: O(k)，k 为覆盖格子内的单位数
///   重建: O(n)，n 为总单位数
/// </summary>
public class SpatialGrid
{
    /// <summary>格子大小</summary>
    public readonly float cellSize;

    /// <summary>格子大小的倒数（避免每次除法）</summary>
    private readonly float _invCellSize;

    /// <summary>网格数据：格子坐标 → 该格子内的所有 Entity</summary>
    private readonly Dictionary<long, List<Entity>> _cells = new();

    /// <summary>对象池：回收 List 避免 GC</summary>
    private readonly Stack<List<Entity>> _listPool = new();

    /// <summary>当前帧中的单位总数（调试用）</summary>
    public int TotalEntities { get; private set; }

    /// <summary>当前帧中有实体的格子数（调试用）</summary>
    public int ActiveCells => _cells.Count;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="cellSize">格子大小（建议 256~512，接近常用技能范围）</param>
    public SpatialGrid(float cellSize = 256f)
    {
        this.cellSize = cellSize;
        _invCellSize = 1f / cellSize;
    }

    #region 构建（每帧调用）

    /// <summary>
    /// 清空网格（每帧开始时调用）
    /// 将所有 List 回收到池中，避免 GC
    /// </summary>
    public void Clear()
    {
        foreach (var (_, list) in _cells)
        {
            list.Clear();
            _listPool.Push(list);
        }
        _cells.Clear();
        TotalEntities = 0;
    }

    /// <summary>
    /// 插入一个单位到网格中
    /// </summary>
    public void Insert(Entity entity, float x, float y)
    {
        long key = CellKey(x, y);
        if (!_cells.TryGetValue(key, out var list))
        {
            list = _listPool.Count > 0 ? _listPool.Pop() : new List<Entity>(8);
            _cells[key] = list;
        }
        list.Add(entity);
        TotalEntities++;
    }

    #endregion

    #region 查询

    /// <summary>
    /// 查询圆形区域内的所有 Entity
    /// </summary>
    public void QueryCircle(float cx, float cy, float radius, List<Entity> results)
    {
        float radiusSq = radius * radius;

        // 计算圆覆盖的格子范围
        int minCellX = (int)MathF.Floor((cx - radius) * _invCellSize);
        int maxCellX = (int)MathF.Floor((cx + radius) * _invCellSize);
        int minCellY = (int)MathF.Floor((cy - radius) * _invCellSize);
        int maxCellY = (int)MathF.Floor((cy + radius) * _invCellSize);

        // 遍历覆盖的格子
        for (int gx = minCellX; gx <= maxCellX; gx++)
        {
            for (int gy = minCellY; gy <= maxCellY; gy++)
            {
                long key = PackKey(gx, gy);
                if (!_cells.TryGetValue(key, out var list)) continue;

                foreach (var entity in list)
                {
                    if (!entity.TryGetComponent<Position>(out var pos)) continue;

                    float dx = pos.x - cx;
                    float dy = pos.y - cy;
                    if (dx * dx + dy * dy <= radiusSq)
                    {
                        results.Add(entity);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 查询矩形区域内的所有 Entity
    /// </summary>
    public void QueryRect(float x, float y, float halfW, float halfH, List<Entity> results)
    {
        float minX = x - halfW, maxX = x + halfW;
        float minY = y - halfH, maxY = y + halfH;

        int minCellX = (int)MathF.Floor(minX * _invCellSize);
        int maxCellX = (int)MathF.Floor(maxX * _invCellSize);
        int minCellY = (int)MathF.Floor(minY * _invCellSize);
        int maxCellY = (int)MathF.Floor(maxY * _invCellSize);

        for (int gx = minCellX; gx <= maxCellX; gx++)
        {
            for (int gy = minCellY; gy <= maxCellY; gy++)
            {
                long key = PackKey(gx, gy);
                if (!_cells.TryGetValue(key, out var list)) continue;

                foreach (var entity in list)
                {
                    if (!entity.TryGetComponent<Position>(out var pos)) continue;

                    if (pos.x >= minX && pos.x <= maxX &&
                        pos.y >= minY && pos.y <= maxY)
                    {
                        results.Add(entity);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 查询线段范围内的所有 Entity（线宽 = width）
    /// 用于线性弹道沿途碰撞检测
    /// </summary>
    public void QueryLine(float startX, float startY, float endX, float endY,
        float width, List<Entity> results)
    {
        float halfW = width * 0.5f;

        // 计算线段的 AABB 包围盒
        float minX = MathF.Min(startX, endX) - halfW;
        float maxX = MathF.Max(startX, endX) + halfW;
        float minY = MathF.Min(startY, endY) - halfW;
        float maxY = MathF.Max(startY, endY) + halfW;

        // 线段方向
        float dx = endX - startX;
        float dy = endY - startY;
        float lenSq = dx * dx + dy * dy;

        int minCellX = (int)MathF.Floor(minX * _invCellSize);
        int maxCellX = (int)MathF.Floor(maxX * _invCellSize);
        int minCellY = (int)MathF.Floor(minY * _invCellSize);
        int maxCellY = (int)MathF.Floor(maxY * _invCellSize);

        float halfWSq = halfW * halfW;

        for (int gx = minCellX; gx <= maxCellX; gx++)
        {
            for (int gy = minCellY; gy <= maxCellY; gy++)
            {
                long key = PackKey(gx, gy);
                if (!_cells.TryGetValue(key, out var list)) continue;

                foreach (var entity in list)
                {
                    if (!entity.TryGetComponent<Position>(out var pos)) continue;

                    // 点到线段的距离平方
                    float distSq = PointToSegmentDistSq(
                        pos.x, pos.y, startX, startY, dx, dy, lenSq);

                    if (distSq <= halfWSq)
                    {
                        results.Add(entity);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 查询扇形区域内的所有 Entity
    /// </summary>
    /// <param name="cx">扇形顶点 X</param>
    /// <param name="cy">扇形顶点 Y</param>
    /// <param name="dirX">朝向 X（归一化）</param>
    /// <param name="dirY">朝向 Y（归一化）</param>
    /// <param name="radius">扇形半径</param>
    /// <param name="halfAngle">半角（弧度）。例如 60° 扇形，传入 π/6</param>
    public void QueryCone(float cx, float cy, float dirX, float dirY,
        float radius, float halfAngle, List<Entity> results)
    {
        float radiusSq = radius * radius;
        float cosHalfAngle = MathF.Cos(halfAngle);

        // 用圆形包围盒粗筛
        int minCellX = (int)MathF.Floor((cx - radius) * _invCellSize);
        int maxCellX = (int)MathF.Floor((cx + radius) * _invCellSize);
        int minCellY = (int)MathF.Floor((cy - radius) * _invCellSize);
        int maxCellY = (int)MathF.Floor((cy + radius) * _invCellSize);

        for (int gx = minCellX; gx <= maxCellX; gx++)
        {
            for (int gy = minCellY; gy <= maxCellY; gy++)
            {
                long key = PackKey(gx, gy);
                if (!_cells.TryGetValue(key, out var list)) continue;

                foreach (var entity in list)
                {
                    if (!entity.TryGetComponent<Position>(out var pos)) continue;

                    float dx = pos.x - cx;
                    float dy = pos.y - cy;
                    float distSq = dx * dx + dy * dy;

                    // 1. 距离检查
                    if (distSq > radiusSq) continue;
                    if (distSq < 0.001f) { results.Add(entity); continue; } // 重叠

                    // 2. 角度检查（用点积代替 atan2，更快）
                    float invDist = 1f / MathF.Sqrt(distSq);
                    float dot = (dx * invDist) * dirX + (dy * invDist) * dirY;

                    if (dot >= cosHalfAngle)
                    {
                        results.Add(entity);
                    }
                }
            }
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>将世界坐标转换为格子 Key</summary>
    private long CellKey(float x, float y)
    {
        int gx = (int)MathF.Floor(x * _invCellSize);
        int gy = (int)MathF.Floor(y * _invCellSize);
        return PackKey(gx, gy);
    }

    /// <summary>将格子坐标打包为 long（高32位 = x，低32位 = y）</summary>
    private static long PackKey(int gx, int gy)
    {
        return ((long)gx << 32) | (uint)gy;
    }

    /// <summary>点到线段的距离平方</summary>
    private static float PointToSegmentDistSq(float px, float py,
        float sx, float sy, float dx, float dy, float lenSq)
    {
        if (lenSq < 0.001f) // 退化为点
            return (px - sx) * (px - sx) + (py - sy) * (py - sy);

        // 投影参数 t ∈ [0, 1]
        float t = ((px - sx) * dx + (py - sy) * dy) / lenSq;
        t = MathF.Max(0, MathF.Min(1, t));

        // 最近点
        float closestX = sx + t * dx;
        float closestY = sy + t * dy;

        float ex = px - closestX;
        float ey = py - closestY;
        return ex * ex + ey * ey;
    }

    #endregion
}
