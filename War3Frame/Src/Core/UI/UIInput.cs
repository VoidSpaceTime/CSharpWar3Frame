using System.Collections.Generic;

namespace War3Frame;

/// <summary>
/// UI 输入服务。消费原始队列，命中后直接投递控件 C# 事件，不创建 ECS 实体。
/// </summary>
public static class UIInput
{
    /// <summary>双击窗口默认 0.3 秒。</summary>
    public const float DoubleClickSeconds = 0.3f;

    private static UIComponent? _capture;
    private static UIComponent? _hover;
    private static UIComponent? _root;
    private static UIComponent? _lastPressed;
    private static float _lastPressedTime = float.NegativeInfinity;
    private static bool _nativeRegistered;
    private static readonly List<JTrigger> _triggers = [];

    /// <summary>当前派发根。面板创建后由 kit 或测试设置。</summary>
    public static UIComponent? Root
    {
        get => _root;
        set => _root = value;
    }

    /// <summary>当前指针捕获。松开后清空。</summary>
    public static UIComponent? Capture => _capture;

    /// <summary>War3 启动后注册一次全局鼠标、滚轮、移动和窗口缩放。回调只入队。</summary>
    public static void RegisterNative()
    {
        if (_nativeRegistered || !UIRuntime.NativeReady) return;
        _nativeRegistered = true;
        RegisterButton(1, true, UIRawKind.LeftDown);
        RegisterButton(1, false, UIRawKind.LeftUp);
        RegisterButton(2, true, UIRawKind.RightDown);
        RegisterButton(2, false, UIRawKind.RightUp);

        var move = JassApi.CreateTrigger();
        HandleHelper.HandleAdd(move);
        DzApi.DzTriggerRegisterMouseMoveEventByCode(move, true, () => EnqueuePointer(UIRawKind.Move, 0));
        _triggers.Add(move);

        var wheel = JassApi.CreateTrigger();
        HandleHelper.HandleAdd(wheel);
        DzApi.DzTriggerRegisterMouseWheelEventByCode(wheel, true, () => EnqueuePointer(UIRawKind.Wheel, DzApi.DzGetWheelDelta()));
        _triggers.Add(wheel);

        var resize = JassApi.CreateTrigger();
        HandleHelper.HandleAdd(resize);
        DzApi.DzTriggerRegisterWindowResizeEventByCode(resize, true, () => UIInputQueue.Enqueue(new UIRawInput(UIRawKind.Resize, 0, 0, LocalPlayerIndex())));
        _triggers.Add(resize);
        RegisterKeys();
    }

    /// <summary>按下即捕获，直到松开。</summary>
    public static void CapturePointer(UIComponent control) => _capture = control;

    /// <summary>消费队列并投递事件。time 由帧循环传入。</summary>
    public static void Pump(float time)
    {
        if (_root == null || _root.IsDestroyed)
        {
            // 无派发根时丢弃本轮输入，避免积压的旧按下在之后设置的根上被当成点击或双击。
            // 窗口缩放仍需要在有根后重算一次，保留为一条待处理的 resize。
            var dropped = UIInputQueue.Drain();
            if (dropped.Exists(static input => input.Kind == UIRawKind.Resize))
                UIInputQueue.Enqueue(new UIRawInput(UIRawKind.Resize, 0, 0, LocalPlayerIndex()));
            return;
        }

        var batch = UIInputQueue.Drain();
        if (batch.Count == 0) return;
        var local = LocalPlayerIndex();
        foreach (var input in batch)
        {
            if (input.Kind == UIRawKind.Resize)
            {
                UILayout.MarkResize(_root);
                _root.FlushLayout();
                continue;
            }

            if (input.Kind is UIRawKind.KeyDown or UIRawKind.KeyUp)
                continue;

            if (input.PlayerIndex != local) continue;
            _root.FlushLayout();
            Dispatch(input, time);
        }
    }

    /// <summary>测试隔离。</summary>
    public static void ResetDispatch()
    {
        _capture = null;
        _hover = null;
        _root = null;
        _lastPressed = null;
        _lastPressedTime = float.NegativeInfinity;
    }

    private static void Dispatch(UIRawInput input, float time)
    {
        var hit = UILayout.Hit(_root!, input.X, input.Y, _capture);
        var pointer = new UIPointer(input.X, input.Y, ButtonOf(input.Kind), time);
        var kind = input.Kind;
        if (kind is UIRawKind.LeftDown or UIRawKind.RightDown)
        {
            if (hit.Winner != null)
            {
                _capture = hit.Winner;
                hit.Winner.RaiseDragStart(pointer);
                hit.Winner.RaisePressed(pointer);
                if (ReferenceEquals(_lastPressed, hit.Winner) && time - _lastPressedTime <= DoubleClickSeconds)
                {
                    hit.Winner.RaiseDoublePressed(pointer);
                    _lastPressed = null;
                    _lastPressedTime = float.NegativeInfinity;
                }
                else
                {
                    _lastPressed = hit.Winner;
                    _lastPressedTime = time;
                }
            }

            return;
        }

        if (kind is UIRawKind.LeftUp or UIRawKind.RightUp)
        {
            var target = _capture ?? hit.Winner;
            target?.RaiseReleased(pointer);
            if (_capture != null)
                _capture.RaiseDragStop(pointer);
            _capture = null;
            return;
        }

        if (kind == UIRawKind.Move)
        {
            var target = _capture ?? hit.Winner;
            if (_capture == null && !ReferenceEquals(_hover, hit.Winner))
            {
                _hover?.RaisePointerExited(pointer);
                hit.Winner?.RaisePointerEntered(pointer);
                _hover = hit.Winner;
            }

            target?.RaiseDrag(pointer);
            return;
        }

        if (kind == UIRawKind.Wheel)
            hit.Winner?.RaiseWheel(pointer, input.Wheel);
    }

    private static int ButtonOf(UIRawKind kind)
        => kind is UIRawKind.RightDown or UIRawKind.RightUp ? 2 : 1;

    private static void RegisterButton(int button, bool down, UIRawKind kind)
    {
        var trigger = JassApi.CreateTrigger();
        HandleHelper.HandleAdd(trigger);
        DzApi.DzTriggerRegisterMouseEventByCode(trigger, button, down ? 1 : 0, true, () => EnqueuePointer(kind, 0));
        _triggers.Add(trigger);
    }

    /// <summary>常用键各注册按下与松开一次。回调只入队，不派发焦点。</summary>
    private static void RegisterKeys()
    {
        Span<int> keys = stackalloc int[46];
        var count = 0;
        keys[count++] = 0x08;
        keys[count++] = 0x09;
        keys[count++] = 0x0D;
        keys[count++] = 0x1B;
        keys[count++] = 0x20;
        for (var key = 0x25; key <= 0x28; key++)
            keys[count++] = key;
        for (var key = 0x30; key <= 0x39; key++)
            keys[count++] = key;
        for (var key = 0x41; key <= 0x5A; key++)
            keys[count++] = key;

        for (var i = 0; i < count; i++)
        {
            var key = keys[i];
            RegisterKey(key, 1, UIRawKind.KeyDown);
            RegisterKey(key, 0, UIRawKind.KeyUp);
        }
    }

    /// <summary>注册单个按键。status 与鼠标一致：1 按下，0 松开。</summary>
    private static void RegisterKey(int key, int status, UIRawKind kind)
    {
        var trigger = JassApi.CreateTrigger();
        HandleHelper.HandleAdd(trigger);
        DzApi.DzTriggerRegisterKeyEventByCode(trigger, key, status, true, () => EnqueueKey(kind));
        _triggers.Add(trigger);
    }

    /// <summary>按键回调只记录键码与玩家。</summary>
    private static void EnqueueKey(UIRawKind kind)
    {
        if (!UIRuntime.NativeReady) return;
        var player = JassApi.GetPlayerId(DzApi.DzGetTriggerKeyPlayer());
        UIInputQueue.Enqueue(new UIRawInput(kind, 0, 0, player, 0, DzApi.DzGetTriggerKey()));
    }

    private static void EnqueuePointer(UIRawKind kind, int wheel)
    {
        var player = LocalPlayerIndex();
        if (!UIRuntime.NativeReady) return;
        var width = Math.Max(1, DzApi.DzGetClientWidth());
        var height = Math.Max(1, DzApi.DzGetClientHeight());
        var x = DzApi.DzGetMouseXRelative() / width * 0.8f;
        var y = DzApi.DzGetMouseYRelative() / height * 0.6f;
        UIInputQueue.Enqueue(new UIRawInput(kind, x, y, player, wheel));
    }

    private static int LocalPlayerIndex()
    {
        if (!UIRuntime.NativeReady) return 0;
        return JassApi.GetPlayerId(JassApi.GetLocalPlayer());
    }
}
