using System.Numerics;
using System.Runtime.CompilerServices;

namespace War3Frame;

public static class Vector3Extensions
{
    /// <summary>
    /// 二次贝塞尔曲线 (3个点)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 BezierQuadratic(this Vector3 start, Vector3 control, Vector3 end, float t)
    {
        float u = 1f - t;
        return (u * u * start) + (2f * u * t * control) + (t * t * end);
    }

    /// <summary>
    /// 三次贝塞尔曲线 (4个点)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 BezierCubic(this Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
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
    /// 基于球坐标系的位移 (Z轴向上 Z-Up，适用 War3 / 虚幻引擎 逻辑)
    /// </summary>
    /// <param name="origin">起点</param>
    /// <param name="distance">移动距离</param>
    /// <param name="yawRadians">水平旋转角 (在 XY 平面上，绕 Z 轴的偏航角)</param>
    /// <param name="pitchRadians">垂直仰角 (与 XY 水平面的夹角)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 MovePolar(this Vector3 origin, float distance, float yawRadians, float pitchRadians)
    {
        // 1. Z-Up 逻辑：高度直接由 Z 轴决定
        float z = distance * MathF.Sin(pitchRadians);

        // 2. 计算投影在水平地面 (XY平面) 上的真实长度
        float horizontalDist = distance * MathF.Cos(pitchRadians);

        // 3. 根据水平投影长度，结合偏航角计算地面的 X 和 Y
        float x = horizontalDist * MathF.Cos(yawRadians);
        float y = horizontalDist * MathF.Sin(yawRadians);

        // 最终将算出的偏移量加到原坐标上
        return new Vector3(origin.X + x, origin.Y + y, origin.Z + z);
    }
}