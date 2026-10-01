namespace War3Frame;

/// <summary>
/// 控件矩形真相：中心与尺寸，屏幕比例，左下原点。
/// </summary>
public struct UIAnchor
{
    public float X;
    public float Y;
    public float W;
    public float H;

    /// <summary>屏幕根参考，对应 xlik UIGame 基准。</summary>
    public static UIAnchor ScreenRoot => new() { X = 0.4f, Y = 0.3f, W = 0.8f, H = 0.6f };

    /// <summary>点是否落在矩形内，边界计入。</summary>
    public readonly bool Contains(float x, float y)
    {
        var halfW = W * 0.5f;
        var halfH = H * 0.5f;
        return x >= X - halfW && x <= X + halfW && y >= Y - halfH && y <= Y + halfH;
    }
}
