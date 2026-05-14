using System.Numerics;
using System.Runtime.CompilerServices;

namespace War3Frame;

public static class Vector2Extensions
{
    /// <summary>
    /// 二次贝塞尔
    /// </summary>
    /// <param name="start"></param>
    /// <param name="control"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 BezierQuadratic(this Vector2 start, Vector2 control, Vector2 end, float t)
    {
        float u = 1f - t;
        return (u * u * start) + (2f * u * t * control) + (t * t * end);
    }

    /// <summary>
    /// 三次贝塞尔
    /// </summary>
    /// <param name="start"></param>
    /// <param name="control1"></param>
    /// <param name="control2"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 BezierCubic(this Vector2 start, Vector2 control1, Vector2 control2, Vector2 end, float t)
    {
        float u = 1f - t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t2 = t * t;
        float t3 = t2 * t;

        return (u3 * start) +
               (3f * u2 * t * control1) +
               (3f * u * t2 * control2) +
               (t3 * end);
    }
    /// <summary>
    /// 基于极坐标系的位移 (2D)
    /// </summary>
    /// <param name="origin">起点</param>
    /// <param name="distance">移动距离</param>
    /// <param name="angleRadians">角度 (弧度制！)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 MovePolar(this Vector2 origin, float distance, float angleRadians)
    {
        float x = distance * MathF.Cos(angleRadians);
        float y = distance * MathF.Sin(angleRadians);
        return new Vector2(origin.X + x, origin.Y + y);
    }
}