using System;
using War3Frame.Src.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 纯逻辑验证控件树、锚点批算、计算命中与控件事件。断言路径不调用 DzApi。
/// </summary>
public static class UIControlValidationScenario
{
    private const string ScenarioName = "UIControlValidationScenario";

    /// <summary>运行不依赖 War3 句柄的同步验证。</summary>
    public static void Initialize(JPlayer player)
    {
        _ = player;
        UIRuntime.NativeReady = false;
        UIHitRegistry.Reset();
        UIInputQueue.Reset();
        UIInput.ResetDispatch();
        CheckTreeAndDestroy();
        CheckRelativeHitAndResize();
        CheckOverlapSwallowAndCapture();
        CheckTooltipPlaceNear();
        CheckDirectEvents();
        CheckContainers();
    }

    private static void CheckTreeAndDestroy()
    {
        var panel = new UIFrame(0);
        panel.Size = new UIVec(0.2f, 0.2f);
        panel.Position = new UIVec(0.4f, 0.3f);
        var button = new UIButton(0, "icon");
        panel.AddChild(button);
        button.Size = new UIVec(0.05f, 0.05f);
        button.Position = new UIVec(0.4f, 0.3f);
        Require(button.Icon != null && button.Children.Count == 1, "按钮图标必须进入 Children");
        Require(button.ZIndex > panel.ZIndex, "子控件层级晚于父控件");

        panel.Visible = false;
        panel.FlushLayout();
        Require(!button.IsVisibleEffective && !UIHitRegistry.Contains(button), "父隐藏后子控件离开命中集");
        panel.Visible = true;

        var slot = new UISlot(0, 1, 0.04f);
        panel.AddChild(slot);
        Require(slot.Children.Count == 2, "格子图标与数量文字进入 Children");
        slot.Destroy();
        Require(slot.IsDestroyed && slot.Icon.IsDestroyed && slot.CountText.IsDestroyed, "格子递归销毁");
        Require(slot.CooldownOverlay == 0, "冷却遮罩只释放一次");

        panel.Destroy();
        Require(button.IsDestroyed && button.Icon!.IsDestroyed, "父销毁先销毁子控件");
        button.Destroy();
    }

    private static void CheckRelativeHitAndResize()
    {
        var host = new UIFrame(0);
        host.Size = new UIVec(0.2f, 0.1f);
        host.Position = new UIVec(0.3f, 0.25f);
        var child = new UIFrame(0);
        host.AddChild(child);
        child.Size = new UIVec(0.04f, 0.04f);
        child.SetPoint(FramePoint.Center, host, FramePoint.TopRight, 0.01f, 0.01f);
        host.FlushLayout();
        Require(child.AnchorResolved, "相对锚点完成批算");
        var inside = UILayout.Hit(host, child.Anchor.X, child.Anchor.Y, null);
        Require(inside.Winner == child, "命中使用换算后的子矩形");

        host.Position = new UIVec(0.4f, 0.3f);
        Require(child.AnchorDirty, "参考移动标脏 LowNodes");
        host.FlushLayout();
        Require(!child.AnchorDirty && child.Anchor.X > 0.3f, "下一次批算按深度结算闭包");

        UIInputQueue.Enqueue(new UIRawInput(UIRawKind.Resize, 0, 0, 0));
        Require(UIInputQueue.ResizeDirty, "resize 只置脏");
        host.FlushLayout();
        Require(!UIInputQueue.ResizeDirty && child.AnchorResolved, "下一 tick 全量重算");
        host.Destroy();
    }

    private static void CheckOverlapSwallowAndCapture()
    {
        var panel = new UIFrame(0);
        panel.Size = new UIVec(0.3f, 0.3f);
        panel.Position = new UIVec(0.4f, 0.3f);
        var label = new UIText(0, "decor");
        panel.AddChild(label);
        label.Size = new UIVec(0.05f, 0.02f);
        label.Position = new UIVec(0.28f, 0.2f);
        var child = new PassFrame();
        panel.AddChild(child);
        child.Size = new UIVec(0.06f, 0.06f);
        child.Position = new UIVec(0.4f, 0.3f);
        var higher = new UIFrame(0);
        panel.AddChild(higher);
        higher.Size = new UIVec(0.04f, 0.04f);
        higher.Position = new UIVec(0.4f, 0.3f);
        panel.FlushLayout();

        var overlap = UILayout.Hit(panel, 0.4f, 0.3f, null);
        Require(overlap.Winner == higher, "重叠只派 CompareZ 胜者");
        Require(overlap.WorldClickSwallowed, "阻挡父面板即使胜者不阻挡也吞世界点击");

        var blank = UILayout.Hit(panel, 0.28f, 0.2f, null);
        Require(blank.Winner == panel && blank.WorldClickSwallowed, "空白处命中阻挡面板");
        var textOnly = UILayout.Hit(label, label.Anchor.X, label.Anchor.Y, null);
        Require(textOnly.Winner == null && !textOnly.WorldClickSwallowed, "未注册装饰文本不派发也不吞点击");

        UIInput.CapturePointer(child);
        var captured = UILayout.Hit(panel, 0.9f, 0.9f, UIInput.Capture);
        Require(captured.Winner == child, "捕获后移出矩形仍发给原控件");
        panel.Destroy();
        UIInput.ResetDispatch();
    }

    /// <summary>贴边时提示翻到宿主左侧或下方。</summary>
    private static void CheckTooltipPlaceNear()
    {
        var open = UILayout.PlaceNear(new UIAnchor { X = 0.2f, Y = 0.2f, W = 0.04f, H = 0.04f }, 0.1f, 0.06f);
        Require(open.X > 0.2f && open.Y > 0.2f, "空间足够时提示放在右上方");

        var flipped = UILayout.PlaceNear(new UIAnchor { X = 0.74f, Y = 0.54f, W = 0.04f, H = 0.04f }, 0.1f, 0.06f);
        Require(flipped.X < 0.74f && flipped.Y < 0.54f, "贴边时提示翻到左下方");
        Require(flipped.X + flipped.W * 0.5f <= 0.8f && flipped.Y + flipped.H * 0.5f <= 0.6f, "翻转后仍落在屏幕内");
    }

    private static void CheckDirectEvents()
    {
        var panel = new UIFrame(0);
        panel.Size = new UIVec(0.8f, 0.6f);
        panel.Position = new UIVec(0.4f, 0.3f);
        var button = new UIButton(0);
        panel.AddChild(button);
        button.Size = new UIVec(0.1f, 0.1f);
        button.Position = new UIVec(0.4f, 0.3f);
        var other = new UIButton(0);
        panel.AddChild(other);
        other.Size = new UIVec(0.08f, 0.08f);
        other.Position = new UIVec(0.2f, 0.2f);
        panel.FlushLayout();
        UIInput.Root = panel;

        var pressed = 0;
        var doubled = 0;
        var entered = 0;
        var exited = 0;
        var dragStart = 0;
        var dragStop = 0;
        button.Pressed += _ => pressed++;
        button.DoublePressed += _ => doubled++;
        button.PointerEntered += _ => entered++;
        button.PointerExited += _ => exited++;
        button.DragStart += _ => dragStart++;
        button.DragStop += _ => dragStop++;
        other.PointerEntered += _ => entered++;

        Pump(UIRawKind.LeftDown, 0.4f, 0.3f, 1, 0.01f);
        Require(pressed == 0, "非本地玩家不投递");

        Pump(UIRawKind.LeftDown, 0.4f, 0.3f, 0, 0.02f);
        Pump(UIRawKind.LeftUp, 0.4f, 0.3f, 0, 0.03f);
        Require(pressed == 1 && dragStart == 1 && dragStop == 1, "本地点击直投控件");

        Pump(UIRawKind.LeftDown, 0.4f, 0.3f, 0, 0.2f);
        Pump(UIRawKind.LeftUp, 0.4f, 0.3f, 0, 0.21f);
        Require(doubled == 1, "窗口内第二次按下触发双击");

        button.Visible = false;
        Pump(UIRawKind.Move, 0.9f, 0.9f, 0, 0.3f);
        Require(pressed == 2 && entered == 0, "隐藏控件与空白处不产生进入");

        button.Visible = true;
        Pump(UIRawKind.Move, 0.4f, 0.3f, 0, 0.4f);
        Pump(UIRawKind.Move, 0.4f, 0.3f, 0, 0.41f);
        Require(entered == 1, "同一控件连续移动只进入一次");

        Pump(UIRawKind.LeftDown, 0.4f, 0.3f, 0, 1f);
        Pump(UIRawKind.Move, 0.2f, 0.2f, 0, 1.01f);
        Require(dragStart == 3 && entered == 1, "捕获期间不给其他控件发进入");

        Pump(UIRawKind.LeftUp, 0.2f, 0.2f, 0, 1.02f);
        Pump(UIRawKind.Move, 0.2f, 0.2f, 0, 1.03f);
        Require(dragStop == 3 && exited == 1 && entered == 2, "松开后才离开原控件并进入新控件");

        panel.Destroy();
        UIInput.ResetDispatch();
    }

    /// <summary>横排与边距由容器计算，调用方不写子坐标。</summary>
    private static void CheckContainers()
    {
        var row = new HBoxContainer(0) { Spacing = 0.02f };
        row.Size = new UIVec(0.4f, 0.12f);
        row.Position = new UIVec(0.4f, 0.3f);
        var first = new UIFrame(0);
        var second = new UIFrame(0);
        var third = new UIFrame(0);
        row.AddChild(first);
        row.AddChild(second);
        row.AddChild(third);
        first.Size = new UIVec(0.08f, 0.08f);
        second.Size = new UIVec(0.08f, 0.08f);
        third.Size = new UIVec(0.08f, 0.08f);
        row.FlushLayout();
        Require(first.Anchor.X < second.Anchor.X && second.Anchor.X < third.Anchor.X, "横排按加入顺序从左到右");
        row.Size = new UIVec(0.5f, 0.12f);
        row.FlushLayout();
        Require(first.Anchor.X < 0.22f, "容器尺寸变化后重算子控件");

        var box = new MarginContainer(0) { Margin = 0.02f };
        box.Size = new UIVec(0.2f, 0.2f);
        box.Position = new UIVec(0.4f, 0.3f);
        var inner = new UIFrame(0);
        box.AddChild(inner);
        inner.Size = new UIVec(0.2f, 0.2f);
        box.FlushLayout();
        Require(Math.Abs(inner.Anchor.W - 0.16f) < 0.0001f && Math.Abs(inner.Anchor.X - 0.4f) < 0.0001f, "边距容器四边缩进");

        var host = new UIFrame(0);
        host.Size = new UIVec(0.2f, 0.1f);
        host.Position = new UIVec(0.4f, 0.3f);
        var filled = new UIFrame(0);
        host.AddChild(filled);
        filled.ApplyPreset(UIPreset.FullRect);
        host.FlushLayout();
        Require(Math.Abs(filled.Anchor.W - 0.2f) < 0.0001f && Math.Abs(filled.Anchor.X - 0.4f) < 0.0001f, "FullRect 铺满父矩形");
        row.Destroy();
        box.Destroy();
        host.Destroy();
    }

    private static void Pump(UIRawKind kind, float x, float y, int player, float time)
    {
        UIInputQueue.Enqueue(new UIRawInput(kind, x, y, player));
        UIInput.Pump(time);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {message}");
    }

    /// <summary>已注册但 Pass，用来验证胜者与吞点击分开判定。</summary>
    private sealed class PassFrame : UIFrame
    {
        public PassFrame() : base(0) => MouseFilter = MouseFilter.Pass;
    }
}
