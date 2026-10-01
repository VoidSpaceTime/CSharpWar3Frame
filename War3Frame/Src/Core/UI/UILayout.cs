namespace War3Frame;

/// <summary>
/// 一次相对定位请求：本控件锚点、参考锚点与偏移。
/// </summary>
public readonly struct UIPointSpec
{
    public UIPointSpec(int point, UIComponent reference, int referencePoint, float offsetX, float offsetY)
    {
        Point = point;
        Reference = reference;
        ReferencePoint = referencePoint;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public int Point { get; }
    public UIComponent Reference { get; }
    public int ReferencePoint { get; }
    public float OffsetX { get; }
    public float OffsetY { get; }
}

/// <summary>
/// 锚点批算与命中派发。不调用 DzApi，原生投影由调用方在 War3 就绪后写出。
/// </summary>
public static class UILayout
{
    private static readonly List<UIComponent> _order = [];
    private static readonly HashSet<UIComponent> _seen = [];

    /// <summary>按深度一次结算脏控件及其 LowNodes 闭包。</summary>
    public static void ResolveDirty(UIComponent root)
    {
        if (root.IsDestroyed) return;
        _order.Clear();
        _seen.Clear();
        Collect(root, 0);
        _order.Sort(static (a, b) => a.Depth != b.Depth ? a.Depth.CompareTo(b.Depth) : a.ControlId.CompareTo(b.ControlId));
        foreach (var control in _order)
            ResolveOne(control);
    }

    /// <summary>窗口尺寸变化后全量重算，不在回调里改树。</summary>
    public static void MarkResize(UIComponent root)
    {
        if (root.IsDestroyed) return;
        MarkTree(root);
    }

    /// <summary>计算命中。capture 优先；否则 CompareZ 取唯一胜者。任一已注册 Stop 命中即吞世界点击。</summary>
    public static UIHit Hit(UIComponent root, float x, float y, UIComponent? capture)
    {
        if (capture is { IsDestroyed: false, IsVisibleEffective: true, AnchorResolved: true })
            return new UIHit(capture, AnyBlockingHit(root, x, y));

        UIComponent? winner = null;
        var swallowed = false;
        foreach (var control in UIHitRegistry.Visible)
        {
            if (!InTree(control, root) || !control.Anchor.Contains(x, y))
                continue;
            if (winner == null || CompareZ(control, winner) > 0)
                winner = control;
            if (control.MouseFilter == MouseFilter.Stop)
                swallowed = true;
        }

        return new UIHit(winner, swallowed);
    }

    /// <summary>贴边翻转：优先放在锚点右上，越界则翻到左或下。</summary>
    public static UIAnchor PlaceNear(UIAnchor host, float width, float height, float gap = 0.005f)
    {
        var right = host.X + host.W * 0.5f + gap + width * 0.5f;
        var left = host.X - host.W * 0.5f - gap - width * 0.5f;
        var above = host.Y + host.H * 0.5f + gap + height * 0.5f;
        var below = host.Y - host.H * 0.5f - gap - height * 0.5f;
        var x = right + width * 0.5f <= 0.8f ? right : left;
        var y = above + height * 0.5f <= 0.6f ? above : below;
        return new UIAnchor { X = x, Y = y, W = width, H = height };
    }

    /// <summary>九宫格上的点。原点在矩形中心。</summary>
    public static (float X, float Y) PointOf(UIAnchor anchor, int point)
    {
        var (nx, ny) = point switch
        {
            FramePoint.TopLeft => (-1f, 1f),
            FramePoint.Top => (0f, 1f),
            FramePoint.TopRight => (1f, 1f),
            FramePoint.Left => (-1f, 0f),
            FramePoint.Right => (1f, 0f),
            FramePoint.BottomLeft => (-1f, -1f),
            FramePoint.Bottom => (0f, -1f),
            FramePoint.BottomRight => (1f, -1f),
            _ => (0f, 0f)
        };
        return (anchor.X + nx * anchor.W * 0.5f, anchor.Y + ny * anchor.H * 0.5f);
    }

    private static void Collect(UIComponent control, int depth)
    {
        control.Depth = depth;
        if (control.AnchorDirty && _seen.Add(control))
            _order.Add(control);
        if (!control.AnchorDirty && control.LowNodes.Count == 0)
        {
            foreach (var child in control.Children)
                Collect(child, depth + 1);
            return;
        }

        foreach (var child in control.Children)
            Collect(child, depth + 1);
        foreach (var dependent in control.LowNodes)
            CollectClosure(dependent);
    }

    private static void CollectClosure(UIComponent control)
    {
        if (!_seen.Add(control)) return;
        _order.Add(control);
        control.AnchorDirty = true;
        foreach (var dependent in control.LowNodes)
            CollectClosure(dependent);
    }

    private static void ResolveOne(UIComponent control)
    {
        if (!control.AnchorDirty || control.IsDestroyed) return;
        if (control.Point is { } spec)
        {
            if (!spec.Reference.AnchorResolved || spec.Reference.IsDestroyed)
            {
                control.AnchorResolved = false;
                control.AnchorDirty = false;
                SyncRegistry(control);
                return;
            }

            var reference = PointOf(spec.Reference.Anchor, spec.ReferencePoint);
            var targetX = reference.X + spec.OffsetX;
            var targetY = reference.Y + spec.OffsetY;
            var self = PointOf(new UIAnchor { X = 0, Y = 0, W = control.Anchor.W, H = control.Anchor.H }, spec.Point);
            control.Anchor = new UIAnchor
            {
                X = targetX - self.X,
                Y = targetY - self.Y,
                W = control.Anchor.W,
                H = control.Anchor.H
            };
        }

        control.AnchorResolved = control.Anchor.W > 0f && control.Anchor.H > 0f;
        control.AnchorDirty = false;
        SyncRegistry(control);
        control.OnResolved();
    }

    private static void MarkTree(UIComponent control)
    {
        control.AnchorDirty = true;
        foreach (var child in control.Children)
            MarkTree(child);
        foreach (var dependent in control.LowNodes)
            if (!dependent.AnchorDirty)
                MarkTree(dependent);
    }

    /// <summary>控件是否位于这次命中的根子树内。相对子控件可以落在父矩形外。</summary>
    private static bool InTree(UIComponent control, UIComponent root)
    {
        for (var node = control; node != null; node = node.Parent)
            if (ReferenceEquals(node, root))
                return true;
        return false;
    }

    /// <summary>已注册且 MouseFilter 为 Stop 的控件是否盖住该点。不按父矩形裁剪子控件。</summary>
    private static bool AnyBlockingHit(UIComponent root, float x, float y)
    {
        foreach (var control in UIHitRegistry.Visible)
        {
            if (control.MouseFilter == MouseFilter.Stop && InTree(control, root) && control.Anchor.Contains(x, y))
                return true;
        }

        return false;
    }

    /// <summary>正数表示 a 压过 b。</summary>
    public static int CompareZ(UIComponent a, UIComponent b)
    {
        if (a.ZIndex != b.ZIndex) return a.ZIndex.CompareTo(b.ZIndex);
        return a.ControlId.CompareTo(b.ControlId);
    }

    private static void SyncRegistry(UIComponent control)
    {
        var visible = control.IsVisibleEffective && control.AnchorResolved && control.Interactive;
        if (visible)
            UIHitRegistry.Add(control);
        else
            UIHitRegistry.Remove(control);
    }
}

/// <summary>一次命中结果：派发胜者与是否吞掉世界点击。</summary>
public readonly struct UIHit
{
    public UIHit(UIComponent? winner, bool worldClickSwallowed)
    {
        Winner = winner;
        WorldClickSwallowed = worldClickSwallowed;
    }

    public UIComponent? Winner { get; }
    public bool WorldClickSwallowed { get; }
}
