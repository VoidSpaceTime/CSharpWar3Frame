using System.Collections.Generic;

namespace War3Frame;

/// <summary>原始鼠标事件种类。回调只入队，不改控件树。</summary>
public enum UIRawKind
{
    LeftDown,
    LeftUp,
    RightDown,
    RightUp,
    Move,
    Wheel,
    KeyDown,
    KeyUp,
    Resize
}

/// <summary>一帧原始输入。坐标已是屏幕比例。</summary>
public readonly struct UIRawInput
{
    public UIRawInput(UIRawKind kind, float x, float y, int playerIndex, int wheel = 0, int key = 0)
    {
        Kind = kind;
        X = x;
        Y = y;
        PlayerIndex = playerIndex;
        Wheel = wheel;
        Key = key;
    }

    public UIRawKind Kind { get; }
    public float X { get; }
    public float Y { get; }
    public int PlayerIndex { get; }
    public int Wheel { get; }

    /// <summary>虚拟键码。仅按键事件使用。</summary>
    public int Key { get; }
}

/// <summary>鼠标回调与系统之间的队列。resize 只置全局脏标记。</summary>
public static class UIInputQueue
{
    private static readonly Queue<UIRawInput> _pending = new();

    /// <summary>窗口尺寸已变，下一 tick 全量重算。</summary>
    public static bool ResizeDirty { get; private set; }

    /// <summary>入队。resize 不携带控件。</summary>
    public static void Enqueue(UIRawInput input)
    {
        if (input.Kind == UIRawKind.Resize)
            ResizeDirty = true;
        _pending.Enqueue(input);
    }

    /// <summary>消费窗口脏标记。调用方负责全量标脏并重算。</summary>
    public static bool ConsumeResize()
    {
        if (!ResizeDirty) return false;
        ResizeDirty = false;
        return true;
    }

    /// <summary>取出本 tick 输入并清除 resize 标记。</summary>
    public static List<UIRawInput> Drain()
    {
        var batch = new List<UIRawInput>(_pending.Count);
        while (_pending.Count > 0)
            batch.Add(_pending.Dequeue());
        ResizeDirty = false;
        return batch;
    }

    /// <summary>测试隔离。</summary>
    public static void Reset()
    {
        _pending.Clear();
        ResizeDirty = false;
    }
}
