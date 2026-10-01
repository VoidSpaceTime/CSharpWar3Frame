namespace War3Frame;

/// <summary>按方向自动排布子控件。子控件坐标由容器计算。</summary>
public abstract class UIBoxContainer : UIComponent
{
    private bool _arranging;

    /// <summary>子控件间距，屏幕比例。</summary>
    public float Spacing { get; set; }

    /// <summary>构造容器。</summary>
    protected UIBoxContainer(int parent, bool vertical) : base(parent)
    {
        Vertical = vertical;
        MouseFilter = MouseFilter.Ignore;
        Interactive = false;
    }

    /// <summary>true 为竖排。</summary>
    protected bool Vertical { get; }

    /// <summary>子节点加入后重排。</summary>
    protected override void OnChildAdded(UIComponent child) => Arrange();

    /// <summary>自身尺寸变化后重排。</summary>
    protected override void OnSizeChanged() => Arrange();

    /// <summary>子控件尺寸变化后重排。</summary>
    protected internal override void OnChildGeometryChanged() => Arrange();

    /// <summary>本容器完成批算后重排。</summary>
    internal override void OnResolved() => Arrange();

    /// <summary>按当前尺寸与子控件重算矩形。</summary>
    public void Arrange()
    {
        if (_arranging || !AnchorResolved) return;
        _arranging = true;
        var cursor = Vertical
            ? Anchor.Y + Anchor.H * 0.5f
            : Anchor.X - Anchor.W * 0.5f;
        foreach (var child in Children)
        {
            if (child.IsDestroyed) continue;
            var size = Vertical ? child.Anchor.H : child.Anchor.W;
            if (size <= 0f) continue;
            var center = Vertical
                ? cursor - size * 0.5f
                : cursor + size * 0.5f;
            child.Position = Vertical
                ? new UIVec(Anchor.X, center)
                : new UIVec(center, Anchor.Y);
            cursor = Vertical ? cursor - size - Spacing : cursor + size + Spacing;
        }

        _arranging = false;
    }
}

/// <summary>竖排容器。</summary>
public sealed class VBoxContainer : UIBoxContainer
{
    /// <summary>构造竖排容器。</summary>
    public VBoxContainer(int parent) : base(parent, true)
    {
    }
}

/// <summary>横排容器。</summary>
public sealed class HBoxContainer : UIBoxContainer
{
    /// <summary>构造横排容器。</summary>
    public HBoxContainer(int parent) : base(parent, false)
    {
    }
}

/// <summary>把唯一子控件居中到容器矩形。</summary>
public sealed class CenterContainer : UIComponent
{
    private bool _arranging;

    /// <summary>构造居中容器。</summary>
    public CenterContainer(int parent) : base(parent)
    {
        MouseFilter = MouseFilter.Ignore;
        Interactive = false;
    }

    /// <summary>子节点加入后重排。</summary>
    protected override void OnChildAdded(UIComponent child) => Arrange();

    /// <summary>自身尺寸变化后重排。</summary>
    protected override void OnSizeChanged() => Arrange();

    /// <summary>本容器完成批算后重排。</summary>
    internal override void OnResolved() => Arrange();

    /// <summary>居中第一个有效子控件。</summary>
    public void Arrange()
    {
        if (_arranging || !AnchorResolved) return;
        _arranging = true;
        foreach (var child in Children)
        {
            if (child.IsDestroyed) continue;
            child.Position = new UIVec(Anchor.X, Anchor.Y);
            break;
        }

        _arranging = false;
    }
}

/// <summary>按四边距收缩子控件。</summary>
public sealed class MarginContainer : UIComponent
{
    private bool _arranging;

    /// <summary>四边距，屏幕比例。</summary>
    public float Margin { get; set; }

    /// <summary>构造边距容器。</summary>
    public MarginContainer(int parent) : base(parent)
    {
        MouseFilter = MouseFilter.Ignore;
        Interactive = false;
    }

    /// <summary>子节点加入后重排。</summary>
    protected override void OnChildAdded(UIComponent child) => Arrange();

    /// <summary>自身尺寸变化后重排。</summary>
    protected override void OnSizeChanged() => Arrange();

    /// <summary>本容器完成批算后重排。</summary>
    internal override void OnResolved() => Arrange();

    /// <summary>把子控件缩进到内矩形。</summary>
    public void Arrange()
    {
        if (_arranging || !AnchorResolved) return;
        _arranging = true;
        var width = Math.Max(0f, Anchor.W - Margin * 2f);
        var height = Math.Max(0f, Anchor.H - Margin * 2f);
        foreach (var child in Children)
        {
            if (child.IsDestroyed) continue;
            child.Size = new UIVec(width, height);
            child.Position = new UIVec(Anchor.X, Anchor.Y);
        }

        _arranging = false;
    }
}
