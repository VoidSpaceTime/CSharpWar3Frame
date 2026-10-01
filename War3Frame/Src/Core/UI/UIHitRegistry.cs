using System.Collections.Generic;

namespace War3Frame;

/// <summary>
/// 有效可见且锚点已解析的交互控件。命中只扫这里，不扫整棵树。
/// </summary>
public static class UIHitRegistry
{
    private static readonly Dictionary<int, UIComponent> _byId = [];
    private static int _nextId = 1;
    private static int _nextZ = 1;

    /// <summary>分配不复用的控件号与创建序层级。</summary>
    public static (int ControlId, int ZIndex) Allocate() => (_nextId++, _nextZ++);

    /// <summary>进入命中集。未解析锚点不得进入。</summary>
    public static void Add(UIComponent control)
    {
        if (!control.AnchorResolved || !control.Interactive || !control.IsVisibleEffective) return;
        _byId[control.ControlId] = control;
    }

    /// <summary>离开命中集。销毁与隐藏都走这里。</summary>
    public static void Remove(UIComponent control) => _byId.Remove(control.ControlId);

    /// <summary>当前可命中控件。</summary>
    public static IReadOnlyCollection<UIComponent> Visible => _byId.Values;

    /// <summary>是否已注册。</summary>
    public static bool Contains(UIComponent control) => _byId.ContainsKey(control.ControlId);

    /// <summary>测试隔离。不在玩法路径调用。</summary>
    public static void Reset()
    {
        _byId.Clear();
        _nextId = 1;
        _nextZ = 1;
    }
}
