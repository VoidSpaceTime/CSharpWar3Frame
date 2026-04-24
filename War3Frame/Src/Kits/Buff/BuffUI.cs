using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame;

/// <summary>
/// Buff 状态栏面板 — 显示 Buff 图标和剩余时间
/// </summary>
public class BuffPanel : UIPanel
{
    private const float IconSize = 0.025f;
    private const float IconGap = 0.002f;
    private const int MaxDisplay = 10;

    private UITexture[] _icons = null!;
    private UIText[] _timers = null!;
    private Entity _boundUnit;

    public BuffPanel() : base("buff") { }
    public void BindUnit(Entity unit) { _boundUnit = unit; Refresh(); }

    protected override void OnCreate(int parentFrame)
    {
        float totalW = MaxDisplay * (IconSize + IconGap) + 0.005f;
        Root = new UIFrame(parentFrame);
        Root.SetSize(totalW, IconSize + 0.018f);
        Root.SetPos(0.2f, 0.06f);

        _icons = new UITexture[MaxDisplay];
        _timers = new UIText[MaxDisplay];

        for (int i = 0; i < MaxDisplay; i++)
        {
            float x = 0.003f + i * (IconSize + IconGap);

            _icons[i] = new UITexture(Root.Handle);
            FrameHelper.SetSize(_icons[i].Handle, IconSize, IconSize);
            FrameHelper.ClearPoints(_icons[i].Handle);
            FrameHelper.SetPoint(_icons[i].Handle, FramePoint.TopLeft,
                Root.Handle, FramePoint.TopLeft, x, -0.002f);
            _icons[i].Hide();

            _timers[i] = new UIText(Root.Handle, "");
            _timers[i].SetColor(255, 255, 255, 200);
            FrameHelper.SetSize(_timers[i].Handle, IconSize, 0.012f);
            FrameHelper.ClearPoints(_timers[i].Handle);
            FrameHelper.SetPoint(_timers[i].Handle, FramePoint.Top,
                _icons[i].Handle, FramePoint.Bottom, 0f, -0.001f);
            _timers[i].Hide();
        }
        Hide();
    }

    public override void Refresh()
    {
        if (!IsVisible || _icons == null || _boundUnit.IsNull) return;

        for (int i = 0; i < MaxDisplay; i++) { _icons[i].Hide(); _timers[i].Hide(); }

        int idx = 0;
        var links = _boundUnit.GetIncomingLinks<AbilityOwner>();
        foreach (var link in links)
        {
            if (idx >= MaxDisplay) break;
            var e = link.Entity;
            if (!e.Tags.Has<Buff>()) continue;

            _icons[idx].Path = "ReplaceableTextures\\CommandButtons\\BTNTemp.blp";
            _icons[idx].Show();

            if (e.TryGetComponent<BuffDuration>(out var dur))
            {
                _timers[idx].Text = dur.isPermanent ? "∞" : $"{dur.remaining:F0}s";
                _timers[idx].Show();
            }
            idx++;
        }
    }

    public override void Destroy()
    {
        if (_icons != null)
            for (int i = 0; i < MaxDisplay; i++) { _icons[i].Destroy(); _timers[i].Destroy(); }
        base.Destroy();
    }
}

/// <summary>
/// Buff 状态栏 UI 系统
/// </summary>
public class BuffUISystem : BaseSystem
{
    private BuffPanel? _panel;
    private float _timer;

    protected override void OnUpdateGroupBegin()
    {
        if (_panel == null)
        {
            _panel = new BuffPanel();
            UIManager.Register(_panel);
        }
    }

    protected override void OnUpdateGroupEnd()
    {
        _timer += Tick.deltaTime;
        if (_timer >= 0.2f) { _timer = 0; _panel?.Refresh(); }
    }
}
