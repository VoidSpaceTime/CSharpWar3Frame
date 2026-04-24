namespace War3Frame;

/// <summary>
/// UI 面板基类
/// 所有业务面板（背包、技能栏、属性面板等）继承此类
/// </summary>
public abstract class UIPanel
{
    /// <summary>面板名称（用于查找和切换）</summary>
    public string Name { get; }

    /// <summary>根容器 Frame</summary>
    public UIFrame Root { get; protected set; } = null!;

    /// <summary>面板是否可见</summary>
    public bool IsVisible { get; private set; }

    /// <summary>面板是否已初始化</summary>
    public bool IsInitialized { get; private set; }

    protected UIPanel(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 初始化面板（创建所有 Frame 控件）
    /// 由 UIManager.Register 调用
    /// </summary>
    public void Initialize(int parentFrame)
    {
        if (IsInitialized) return;
        IsInitialized = true;
        OnCreate(parentFrame);
    }

    /// <summary>
    /// 子类实现：创建 UI 控件
    /// </summary>
    protected abstract void OnCreate(int parentFrame);

    /// <summary>
    /// 子类实现：从 ECS 数据刷新 UI 显示
    /// 由 UINativeSystem 定时调用
    /// </summary>
    public abstract void Refresh();

    /// <summary>显示面板</summary>
    public virtual void Show()
    {
        IsVisible = true;
        Root?.Show();
    }

    /// <summary>隐藏面板</summary>
    public virtual void Hide()
    {
        IsVisible = false;
        Root?.Hide();
    }

    /// <summary>切换显示/隐藏</summary>
    public void Toggle()
    {
        if (IsVisible) Hide();
        else Show();
    }

    /// <summary>销毁面板</summary>
    public virtual void Destroy()
    {
        Root?.Destroy();
        IsInitialized = false;
    }
}

/// <summary>
/// UI 管理器 - 管理所有 UIPanel 的生命周期和刷新
/// </summary>
public static class UIManager
{
    private static readonly Dictionary<string, UIPanel> _panels = new();
    private static int _gameUI;
    private static bool _initialized;

    /// <summary>全局 Tooltip 实例</summary>
    public static UITooltip? Tooltip { get; private set; }

    /// <summary>
    /// 初始化 UI 管理器
    /// 应在游戏初始化时（地图加载后）调用
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        _gameUI = FrameHelper.GetGameUI();

        // 创建全局 Tooltip
        Tooltip = new UITooltip(_gameUI);
    }

    /// <summary>获取游戏根 UI 节点</summary>
    public static int GameUI => _gameUI;

    /// <summary>
    /// 注册并初始化一个面板
    /// </summary>
    public static void Register(UIPanel panel)
    {
        if (_panels.ContainsKey(panel.Name)) return;

        _panels[panel.Name] = panel;
        panel.Initialize(_gameUI);
    }

    /// <summary>获取面板</summary>
    public static T? GetPanel<T>(string name) where T : UIPanel
    {
        return _panels.TryGetValue(name, out var panel) ? panel as T : null;
    }

    /// <summary>显示指定面板</summary>
    public static void ShowPanel(string name)
    {
        if (_panels.TryGetValue(name, out var panel))
            panel.Show();
    }

    /// <summary>隐藏指定面板</summary>
    public static void HidePanel(string name)
    {
        if (_panels.TryGetValue(name, out var panel))
            panel.Hide();
    }

    /// <summary>切换面板显隐</summary>
    public static void TogglePanel(string name)
    {
        if (_panels.TryGetValue(name, out var panel))
            panel.Toggle();
    }

    /// <summary>
    /// 刷新所有可见面板的数据
    /// 由 UINativeSystem 定时调用
    /// </summary>
    public static void RefreshAll()
    {
        foreach (var panel in _panels.Values)
        {
            if (panel.IsVisible)
                panel.Refresh();
        }
    }

    /// <summary>销毁所有面板</summary>
    public static void DestroyAll()
    {
        foreach (var panel in _panels.Values)
            panel.Destroy();
        _panels.Clear();

        Tooltip?.Destroy();
        Tooltip = null;
        _initialized = false;
    }
}
