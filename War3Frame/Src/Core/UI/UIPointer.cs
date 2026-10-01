namespace War3Frame;

/// <summary>指针过滤。决定控件是否消费事件，以及是否吞掉世界点击。</summary>
public enum MouseFilter
{
    /// <summary>命中即消费，并吞掉世界点击。</summary>
    Stop,

    /// <summary>命中即消费，但不单独吞掉世界点击。</summary>
    Pass,

    /// <summary>不参与命中，也不吞世界点击。</summary>
    Ignore
}

/// <summary>相对父节点的比例对齐。FullRect 会同时铺满父矩形。</summary>
public enum UIPreset
{
    FullRect,
    Center,
    TopLeft,
    Top,
    TopRight,
    Left,
    Right,
    BottomLeft,
    Bottom,
    BottomRight
}

/// <summary>屏幕比例二维量。X/Y 或宽/高。</summary>
public readonly struct UIVec
{
    /// <summary>构造比例向量。</summary>
    public UIVec(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>X 或宽。</summary>
    public float X { get; }

    /// <summary>Y 或高。</summary>
    public float Y { get; }
}

/// <summary>一次指针事件的坐标与按键。</summary>
public readonly struct UIPointer
{
    /// <summary>构造一次指针事件。</summary>
    public UIPointer(float x, float y, int button, float time)
    {
        X = x;
        Y = y;
        Button = button;
        Time = time;
    }

    /// <summary>屏幕比例 X。</summary>
    public float X { get; }

    /// <summary>屏幕比例 Y。</summary>
    public float Y { get; }

    /// <summary>1 为左键，2 为右键。</summary>
    public int Button { get; }

    /// <summary>事件时间，用于内建双击。</summary>
    public float Time { get; }
}
