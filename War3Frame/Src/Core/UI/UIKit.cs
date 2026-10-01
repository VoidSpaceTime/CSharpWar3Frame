using System.Collections.Generic;

namespace War3Frame;

/// <summary>面板基类。只持有一棵控件树和显隐钩子，由 UIKit 按层创建。</summary>
public abstract class UIPanel
{
    /// <summary>面板名称。</summary>
    public string Name { get; }

    /// <summary>所属层。由 kit 注册时写入。</summary>
    public UILayer Layer { get; internal set; } = UILayer.Panel;

    /// <summary>根控件。</summary>
    public UIFrame Root { get; protected set; } = null!;

    /// <summary>是否可见。</summary>
    public bool IsVisible { get; private set; }

    /// <summary>是否已创建。</summary>
    public bool IsInitialized { get; private set; }

    /// <summary>构造面板。</summary>
    protected UIPanel(string name) => Name = name;

    /// <summary>创建控件树。由 kit 按层序调用。</summary>
    public void Initialize(int parentFrame)
    {
        if (IsInitialized) return;
        IsInitialized = true;
        OnCreate(parentFrame);
    }

    /// <summary>子类创建控件。</summary>
    protected abstract void OnCreate(int parentFrame);

    /// <summary>从 ECS 刷新显示。</summary>
    public abstract void Refresh();

    /// <summary>显示面板。</summary>
    public virtual void Show()
    {
        IsVisible = true;
        if (Root != null) Root.Visible = true;
    }

    /// <summary>隐藏面板。</summary>
    public virtual void Hide()
    {
        IsVisible = false;
        if (Root != null) Root.Visible = false;
    }

    /// <summary>销毁根控件。</summary>
    public virtual void Destroy()
    {
        Root?.Destroy();
        IsInitialized = false;
    }
}

/// <summary>一套 UI 的分组。按层序创建面板，同名 key 幂等。</summary>
public sealed class UIKit
{
    private readonly struct Registration
    {
        public Registration(UILayer layer, int seq, UIPanel panel)
        {
            Layer = layer;
            Seq = seq;
            Panel = panel;
        }

        public UILayer Layer { get; }
        public int Seq { get; }
        public UIPanel Panel { get; }
    }

    private readonly Dictionary<string, UIPanel> _panels = new(StringComparer.Ordinal);
    private readonly List<Registration> _registrations = [];
    private int _sequence;
    private int _gameUI;
    private bool _built;

    /// <summary>构造一套 UI。非原生环境不查询游戏根节点。</summary>
    public UIKit()
    {
        _gameUI = UIRuntime.NativeReady ? FrameHelper.GetGameUI() : 0;
    }

    /// <summary>游戏根 Frame。</summary>
    public int GameUI => _gameUI;

    /// <summary>本组提示控件。创建时属于 Tooltip 层。</summary>
    public UITooltip? Tooltip { get; private set; }

    /// <summary>记录面板。重复 key 返回已有实例。</summary>
    public UIPanel Add(UIPanel panel, UILayer layer = UILayer.Panel)
    {
        if (_panels.TryGetValue(panel.Name, out var existing))
            return existing;
        panel.Layer = layer;
        _panels[panel.Name] = panel;
        _registrations.Add(new Registration(layer, _sequence++, panel));
        return panel;
    }

    /// <summary>按 (层序, 注册序) 创建。创建后保持隐藏。</summary>
    public void Build()
    {
        if (_built) return;
        _built = true;
        var ordered = new List<Registration>(_registrations);
        ordered.Sort(static (a, b) =>
        {
            var byLayer = a.Layer.CompareTo(b.Layer);
            return byLayer != 0 ? byLayer : a.Seq.CompareTo(b.Seq);
        });
        foreach (var registration in ordered)
        {
            registration.Panel.Initialize(_gameUI);
            registration.Panel.Hide();
        }

        Tooltip = new UITooltip(_gameUI);
        Tooltip.Visible = false;
        if (ordered.Count > 0)
            UIInput.Root = ordered[^1].Panel.Root;
    }

    /// <summary>按名称取面板。</summary>
    public T? Get<T>(string name) where T : UIPanel
        => _panels.TryGetValue(name, out var panel) ? panel as T : null;

    /// <summary>显示面板。</summary>
    public void Show(string name)
    {
        if (_panels.TryGetValue(name, out var panel))
            panel.Show();
    }

    /// <summary>隐藏面板。</summary>
    public void Hide(string name)
    {
        if (_panels.TryGetValue(name, out var panel))
            panel.Hide();
    }

    /// <summary>销毁本组全部面板与提示。</summary>
    public void Destroy()
    {
        for (var i = _registrations.Count - 1; i >= 0; i--)
            _registrations[i].Panel.Destroy();
        _registrations.Clear();
        _panels.Clear();
        Tooltip?.Destroy();
        Tooltip = null;
        _built = false;
    }
}
