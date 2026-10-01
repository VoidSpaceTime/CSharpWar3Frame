using System.Collections.Generic;

namespace War3Frame;

/// <summary>
/// UI 控件基类。结构树、锚点与命中注册的拥有者，不直接承担长期原生语义。
/// </summary>
public abstract class UIComponent
{
    private readonly List<UIComponent> _children = [];
    private readonly List<UIComponent> _lowNodes = [];
    private bool _visible = true;

    /// <summary>War3 Frame 句柄。逻辑测试中为 0。</summary>
    public int Handle { get; protected set; }

    /// <summary>创建时的父 Frame。不是结构父节点。</summary>
    public int ParentHandle { get; protected set; }

    /// <summary>是否已销毁。</summary>
    public bool IsDestroyed { get; private set; }

    /// <summary>自身显隐。有效可见还要看祖先。</summary>
    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            if (UIRuntime.NativeReady && Handle != 0)
                FrameHelper.SetVisible(Handle, value);
            SyncInteractive();
        }
    }

    /// <summary>中心点，屏幕比例。写入后标脏，下次批算投影。</summary>
    public UIVec Position
    {
        get => new(Anchor.X, Anchor.Y);
        set
        {
            DetachPoint();
            Anchor = new UIAnchor { X = value.X, Y = value.Y, W = Anchor.W, H = Anchor.H };
            MarkDirty();
        }
    }

    /// <summary>尺寸，屏幕比例。写入后标脏，下次批算投影。</summary>
    public UIVec Size
    {
        get => new(Anchor.W, Anchor.H);
        set
        {
            Anchor = new UIAnchor { X = Anchor.X, Y = Anchor.Y, W = value.X, H = value.Y };
            MarkDirty();
            OnSizeChanged();
            Parent?.OnChildGeometryChanged();
        }
    }

    /// <summary>按下。命中胜者收到。</summary>
    public event Action<UIPointer>? Pressed;

    /// <summary>同一控件在双击窗口内再次按下。</summary>
    public event Action<UIPointer>? DoublePressed;

    /// <summary>指针进入。</summary>
    public event Action<UIPointer>? PointerEntered;

    /// <summary>指针离开。</summary>
    public event Action<UIPointer>? PointerExited;

    /// <summary>拖动开始。</summary>
    public event Action<UIPointer>? DragStart;

    /// <summary>拖动中。</summary>
    public event Action<UIPointer>? Drag;

    /// <summary>拖动结束。</summary>
    public event Action<UIPointer>? DragStop;

    /// <summary>松开。</summary>
    public event Action<UIPointer>? Released;

    /// <summary>滚轮。参数为滚轮增量。</summary>
    public event Action<UIPointer, int>? Wheel;

    /// <summary>结构父节点。根控件为空。</summary>
    public UIComponent? Parent { get; private set; }

    /// <summary>结构子控件。惰性，销毁时先子后父。</summary>
    public IReadOnlyList<UIComponent> Children => _children;

    /// <summary>依赖本控件锚点的控件。移动时只标脏这条闭包。</summary>
    public IReadOnlyList<UIComponent> LowNodes => _lowNodes;

    /// <summary>调试名，不参与命中。</summary>
    public string? Key { get; set; }

    /// <summary>注册号。销毁后不复用。</summary>
    public int ControlId { get; private set; }

    /// <summary>创建序层级。只经 CompareZ 比较。</summary>
    public int ZIndex { get; private set; }

    /// <summary>命中过滤。Stop 吞世界点击，Ignore 不参与派发。</summary>
    public MouseFilter MouseFilter { get; set; } = MouseFilter.Ignore;

    /// <summary>透明度 0-255。写入后在原生环境投影。</summary>
    public int Alpha
    {
        get => _alpha;
        set
        {
            _alpha = value;
            if (UIRuntime.NativeReady && Handle != 0)
                FrameHelper.SetAlpha(Handle, value);
        }
    }

    private int _alpha = 255;

    /// <summary>是否进入命中注册表。文本与装饰默认 false。</summary>
    public bool Interactive { get; protected set; }

    /// <summary>最近一次批算矩形。</summary>
    public UIAnchor Anchor { get; internal set; }

    /// <summary>位置、尺寸或参考变化后为 true。</summary>
    public bool AnchorDirty { get; internal set; } = true;

    /// <summary>已得到正尺寸矩形。false 不得进命中集。</summary>
    public bool AnchorResolved { get; internal set; }

    /// <summary>批算深度，仅供排序。</summary>
    internal int Depth { get; set; }

    /// <summary>相对定位。为空表示矩形已是绝对中心。</summary>
    internal UIPointSpec? Point { get; private set; }

    /// <summary>祖先都可见时为 true。</summary>
    public bool IsVisibleEffective => _visible && (Parent?.IsVisibleEffective ?? true) && !IsDestroyed;

    protected UIComponent(int parentHandle)
    {
        ParentHandle = parentHandle;
        (ControlId, ZIndex) = UIHitRegistry.Allocate();
    }

    /// <summary>挂到结构父节点。子控件创建序必须晚于父节点。</summary>
    public void AddChild(UIComponent child)
    {
        if (child.Parent == this) return;
        child.Parent?._children.Remove(child);
        child.Parent = this;
        _children.Add(child);
        child.AnchorDirty = true;
        SyncInteractive();
        OnChildAdded(child);
    }

    /// <summary>子节点加入后。容器用来重排。</summary>
    protected virtual void OnChildAdded(UIComponent child)
    {
    }

    /// <summary>自身尺寸变化后。容器用来重排。</summary>
    protected virtual void OnSizeChanged()
    {
    }

    /// <summary>子控件尺寸变化后。容器用来重排。</summary>
    protected internal virtual void OnChildGeometryChanged()
    {
    }

    /// <summary>本控件锚点完成一次批算后。</summary>
    internal virtual void OnResolved()
    {
    }

    /// <summary>记录相对锚点。参考必须是可计算控件，不能是 UINative。</summary>
    public void SetPoint(int point, UIComponent reference, int referencePoint, float offsetX = 0, float offsetY = 0)
    {
        if (reference.IsNativeReference)
            throw new InvalidOperationException("UINative 不能作为可计算锚点参考。");
        DetachPoint();
        Point = new UIPointSpec(point, reference, referencePoint, offsetX, offsetY);
        reference._lowNodes.Add(this);
        MarkDirty();
    }

    /// <summary>保留旧签名。相对句柄定位不再直接写 Frame，测试与新代码使用控件重载。</summary>
    public void SetPoint(int point, int relFrame, int relPoint, float offsetX = 0, float offsetY = 0)
    {
        _ = (point, relFrame, relPoint, offsetX, offsetY);
        MarkDirty();
    }

    /// <summary>铺满结构父节点。批算写出矩形，不调用 DzFrameSetAllPoints。</summary>
    public void FillParent() => ApplyPreset(UIPreset.FullRect);

    /// <summary>按比例预设对齐到结构父节点。FullRect 同时铺满尺寸。</summary>
    public void ApplyPreset(UIPreset preset)
    {
        if (Parent == null) return;
        var point = preset switch
        {
            UIPreset.TopLeft => FramePoint.TopLeft,
            UIPreset.Top => FramePoint.Top,
            UIPreset.TopRight => FramePoint.TopRight,
            UIPreset.Left => FramePoint.Left,
            UIPreset.Right => FramePoint.Right,
            UIPreset.BottomLeft => FramePoint.BottomLeft,
            UIPreset.Bottom => FramePoint.Bottom,
            UIPreset.BottomRight => FramePoint.BottomRight,
            _ => FramePoint.Center
        };
        SetPoint(point, Parent, point);
        if (preset == UIPreset.FullRect)
            Size = new UIVec(Parent.Anchor.W, Parent.Anchor.H);
    }

    /// <summary>投递按下。</summary>
    internal void RaisePressed(UIPointer pointer) => Pressed?.Invoke(pointer);

    /// <summary>投递双击。</summary>
    internal void RaiseDoublePressed(UIPointer pointer) => DoublePressed?.Invoke(pointer);

    /// <summary>投递进入。</summary>
    internal void RaisePointerEntered(UIPointer pointer) => PointerEntered?.Invoke(pointer);

    /// <summary>投递离开。</summary>
    internal void RaisePointerExited(UIPointer pointer) => PointerExited?.Invoke(pointer);

    /// <summary>投递拖动开始。</summary>
    internal void RaiseDragStart(UIPointer pointer) => DragStart?.Invoke(pointer);

    /// <summary>投递拖动。</summary>
    internal void RaiseDrag(UIPointer pointer) => Drag?.Invoke(pointer);

    /// <summary>投递拖动结束。</summary>
    internal void RaiseDragStop(UIPointer pointer) => DragStop?.Invoke(pointer);

    /// <summary>投递松开。</summary>
    internal void RaiseReleased(UIPointer pointer) => Released?.Invoke(pointer);

    /// <summary>投递滚轮。</summary>
    internal void RaiseWheel(UIPointer pointer, int delta) => Wheel?.Invoke(pointer, delta);

    /// <summary>先子后父销毁，摘掉命中与 LowNodes，再释放自身 Frame 一次。</summary>
    public void Destroy()
    {
        if (IsDestroyed) return;
        IsDestroyed = true;
        for (var i = _children.Count - 1; i >= 0; i--)
            _children[i].Destroy();
        _children.Clear();
        DetachPoint();
        _lowNodes.Clear();
        UIHitRegistry.Remove(this);
        OnDestroy();
        if (UIRuntime.NativeReady && Handle != 0)
            FrameHelper.Destroy(Handle);
        Handle = 0;
        Parent?._children.Remove(this);
        Parent = null;
    }

    /// <summary>释放不属于 Children 的裸 Frame。结构子控件不得在这里再销毁。</summary>
    protected virtual void OnDestroy()
    {
    }

    /// <summary>原生 Frame 父节点，没有可回读矩形。</summary>
    public bool IsNativeReference { get; protected set; }

    /// <summary>命中前批算本控件所在树的脏锚点，并把已解析矩形投影到 Frame。</summary>
    public void FlushLayout()
    {
        var root = this;
        while (root.Parent != null)
            root = root.Parent;
        if (UIInputQueue.ConsumeResize())
            UILayout.MarkResize(root);
        UILayout.ResolveDirty(root);
        if (UIRuntime.NativeReady)
            Project(root);
    }

    private void MarkDirty()
    {
        AnchorDirty = true;
        foreach (var node in _lowNodes)
            node.AnchorDirty = true;
    }

    private void DetachPoint()
    {
        if (Point is not { } spec) return;
        spec.Reference._lowNodes.Remove(this);
        Point = null;
    }

    private void SyncInteractive()
    {
        if (IsVisibleEffective && AnchorResolved && Interactive)
            UIHitRegistry.Add(this);
        else
            UIHitRegistry.Remove(this);
        foreach (var child in _children)
            child.SyncInteractive();
    }

    private static void Project(UIComponent control)
    {
        if (control.Handle != 0 && control.AnchorResolved)
        {
            var left = control.Anchor.X - control.Anchor.W * 0.5f;
            var bottom = control.Anchor.Y - control.Anchor.H * 0.5f;
            FrameHelper.ClearPoints(control.Handle);
            FrameHelper.SetSize(control.Handle, control.Anchor.W, control.Anchor.H);
            FrameHelper.SetAbsPos(control.Handle, left, bottom);
        }

        foreach (var child in control.Children)
            Project(child);
    }
}

/// <summary>
/// 背景框，用于容器和面板背景。默认吞掉世界点击。
/// </summary>
public class UIFrame : UIComponent
{
    public UIFrame(int parent, string? texture = null) : base(parent)
    {
        MouseFilter = MouseFilter.Stop;
        Interactive = true;
        if (UIRuntime.NativeReady)
            Handle = FrameHelper.CreateBackdrop(parent, texture);
    }

    /// <summary>设置背景贴图。</summary>
    public void SetTexture(string path)
    {
        if (UIRuntime.NativeReady && Handle != 0)
            FrameHelper.SetTexture(Handle, path);
    }
}

/// <summary>
/// 文字控件。默认不参与命中，也不吞世界点击。
/// </summary>
public class UIText : UIComponent
{
    private string _text = "";

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            if (UIRuntime.NativeReady && Handle != 0)
                FrameHelper.SetText(Handle, value);
        }
    }

    public UIText(int parent, string text = "") : base(parent)
    {
        MouseFilter = MouseFilter.Ignore;
        Interactive = false;
        _text = text;
        if (UIRuntime.NativeReady)
            Handle = FrameHelper.CreateText(parent, text);
    }

    /// <summary>设置文字颜色 (0-255)。</summary>
    public void SetColor(int a, int r, int g, int b)
    {
        if (UIRuntime.NativeReady && Handle != 0)
            FrameHelper.SetTextColor(Handle, a, r, g, b);
    }

    /// <summary>设置字体。</summary>
    public void SetFont(string fontFile, float height)
    {
        if (UIRuntime.NativeReady && Handle != 0)
            FrameHelper.SetFont(Handle, fontFile, height);
    }
}

/// <summary>
/// 贴图。默认为装饰，不参与命中。
/// </summary>
public class UITexture : UIComponent
{
    private string _path = "";

    public string Path
    {
        get => _path;
        set
        {
            _path = value;
            if (UIRuntime.NativeReady && Handle != 0)
                FrameHelper.SetTexture(Handle, value);
        }
    }

    public UITexture(int parent, string texturePath = "") : base(parent)
    {
        MouseFilter = MouseFilter.Ignore;
        Interactive = false;
        _path = texturePath;
        if (UIRuntime.NativeReady)
            Handle = FrameHelper.CreateBackdrop(parent, texturePath.Length > 0 ? texturePath : null);
    }
}

/// <summary>
/// 进度条。数值写原生，矩形仍走锚点。
/// </summary>
public class UIBar : UIComponent
{
    private float _value;
    private float _min;
    private float _max = 1f;

    /// <summary>当前值，自动夹到范围内。</summary>
    public float Value
    {
        get => _value;
        set
        {
            _value = Math.Clamp(value, _min, _max);
            if (UIRuntime.NativeReady && Handle != 0)
                FrameHelper.SetValue(Handle, _value);
        }
    }

    public UIBar(int parent, string? texture = null, float min = 0f, float max = 1f) : base(parent)
    {
        MouseFilter = MouseFilter.Ignore;
        Interactive = false;
        _min = min;
        _max = max;
        if (!UIRuntime.NativeReady) return;
        Handle = FrameHelper.CreateStatusBar(parent, texture);
        FrameHelper.SetMinMaxValue(Handle, min, max);
    }

    /// <summary>设置进度条贴图。</summary>
    public void SetTexture(string path)
    {
        if (UIRuntime.NativeReady && Handle != 0)
            FrameHelper.SetTexture(Handle, path);
    }

    /// <summary>设置范围。</summary>
    public void SetMinMax(float min, float max)
    {
        _min = min;
        _max = max;
        if (UIRuntime.NativeReady && Handle != 0)
            FrameHelper.SetMinMaxValue(Handle, min, max);
    }
}

/// <summary>
/// 按钮。图标是子控件，点击走计算命中，不注册逐帧脚本。
/// </summary>
public class UIButton : UIComponent
{
    /// <summary>是否启用。</summary>
    public bool Enabled { get; private set; } = true;

    /// <summary>图标子控件。</summary>
    public UITexture? Icon { get; private set; }

    public UIButton(int parent, string? iconPath = null) : base(parent)
    {
        MouseFilter = MouseFilter.Stop;
        Interactive = true;
        if (UIRuntime.NativeReady)
            Handle = FrameHelper.CreateBackdrop(parent);
        if (iconPath == null) return;
        Icon = new UITexture(parent, iconPath);
        AddChild(Icon);
        Icon.FillParent();
    }

    /// <summary>启用或禁用。</summary>
    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
        if (UIRuntime.NativeReady && Handle != 0)
            FrameHelper.SetEnable(Handle, enabled);
    }

    /// <summary>替换图标贴图。</summary>
    public void SetIcon(string path)
    {
        if (Icon != null)
            Icon.Path = path;
    }
}

/// <summary>
/// 物品或技能格子。图标与数量是子控件，冷却遮罩是裸 Frame。
/// </summary>
public class UISlot : UIComponent
{
    /// <summary>图标子控件。</summary>
    public UITexture Icon { get; private set; }

    /// <summary>数量文字子控件。</summary>
    public UIText CountText { get; private set; }

    /// <summary>冷却遮罩 Frame。不属于 Children，只在 OnDestroy 释放一次。</summary>
    public int CooldownOverlay { get; private set; }

    /// <summary>格子是否为空。</summary>
    public bool IsEmpty { get; private set; } = true;

    /// <summary>格子索引。</summary>
    public int SlotIndex { get; set; }

    public UISlot(int parent, int slotIndex, float size) : base(parent)
    {
        MouseFilter = MouseFilter.Stop;
        Interactive = true;
        SlotIndex = slotIndex;
        Size = new UIVec(size, size);
        if (UIRuntime.NativeReady)
            Handle = FrameHelper.CreateBackdrop(parent, "UI\\Widgets\\EscMenu\\Human\\editbox-background.blp");

        Icon = new UITexture(parent);
        AddChild(Icon);
        Icon.FillParent();

        CountText = new UIText(parent, "");
        AddChild(CountText);
        CountText.Size = new UIVec(size * 0.4f, size * 0.3f);
        CountText.SetPoint(FramePoint.BottomRight, this, FramePoint.BottomRight, -0.001f, 0.001f);

        if (!UIRuntime.NativeReady) return;
        CooldownOverlay = FrameHelper.CreateBackdrop(Handle);
        FrameHelper.SetAlpha(CooldownOverlay, 128);
        FrameHelper.SetVisible(CooldownOverlay, false);
    }

    /// <summary>设置格子内容。</summary>
    public void SetContent(string iconPath, int count = 0)
    {
        IsEmpty = false;
        Icon.Path = iconPath;
        Icon.Visible = true;
        CountText.Text = count > 1 ? count.ToString() : "";
        CountText.Visible = count > 1;
    }

    /// <summary>清空格子。</summary>
    public void Clear()
    {
        IsEmpty = true;
        Icon.Visible = false;
        CountText.Visible = false;
        SetCooldown(0);
    }

    /// <summary>设置冷却进度。0 为无冷却。</summary>
    public void SetCooldown(float ratio)
    {
        if (!UIRuntime.NativeReady || CooldownOverlay == 0) return;
        FrameHelper.SetVisible(CooldownOverlay, ratio > 0f);
    }

    /// <summary>冷却遮罩不属于子控件树。</summary>
    protected override void OnDestroy()
    {
        if (UIRuntime.NativeReady && CooldownOverlay != 0)
            FrameHelper.Destroy(CooldownOverlay);
        CooldownOverlay = 0;
    }
}

/// <summary>
/// 悬浮提示。标题与描述是子控件，位置由锚点矩形决定。
/// </summary>
public class UITooltip : UIComponent
{
    /// <summary>标题子控件。</summary>
    public UIText Title { get; private set; }

    /// <summary>描述子控件。</summary>
    public UIText Description { get; private set; }

    public UITooltip(int parent, float width = 0.15f) : base(parent)
    {
        MouseFilter = MouseFilter.Ignore;
        Interactive = false;
        Size = new UIVec(width, 0.08f);
        if (UIRuntime.NativeReady)
        {
            Handle = FrameHelper.CreateBackdrop(parent, "UI\\Widgets\\ToolTips\\Human\\human-tooltip-background.blp");
            FrameHelper.SetVisible(Handle, false);
        }

        Visible = false;
        Title = new UIText(parent, "");
        AddChild(Title);
        Title.Size = new UIVec(width - 0.01f, 0.02f);
        Title.SetPoint(FramePoint.TopLeft, this, FramePoint.TopLeft, 0.005f, -0.005f);

        Description = new UIText(parent, "");
        AddChild(Description);
        Description.Size = new UIVec(width - 0.01f, 0.05f);
        Description.SetPoint(FramePoint.TopLeft, Title, FramePoint.BottomLeft, 0f, -0.003f);
    }

    /// <summary>用宿主锚点贴边放置并显示。</summary>
    public void PlaceNear(UIAnchor host, string title, string description)
    {
        Title.Text = title;
        Description.Text = description;
        var placed = UILayout.PlaceNear(host, Anchor.W, Anchor.H);
        Position = new UIVec(placed.X, placed.Y);
        Visible = true;
    }

    /// <summary>在绝对位置显示。</summary>
    public void ShowAt(float x, float y, string title, string description)
    {
        Title.Text = title;
        Description.Text = description;
        Position = new UIVec(x + Anchor.W * 0.5f, y + Anchor.H * 0.5f);
        Visible = true;
    }
}

/// <summary>
/// 不可回读位置的原生 Frame。可做创建父节点，不能进命中集，也不能当锚点参考。
/// </summary>
public sealed class UINative : UIComponent
{
    public UINative(int parent) : base(parent)
    {
        IsNativeReference = true;
        Interactive = false;
        MouseFilter = MouseFilter.Ignore;
        Handle = parent;
    }
}

/// <summary>
/// 原生投影开关。回归与逻辑测试保持关闭，War3 启动后打开。
/// </summary>
public static class UIRuntime
{
    /// <summary>为 true 时才创建和改写 Frame。</summary>
    public static bool NativeReady { get; set; }
}
