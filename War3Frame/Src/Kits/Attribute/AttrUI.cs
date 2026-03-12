using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame;

/// <summary>
/// 属性面板 — 显示单位各项属性数值
/// </summary>
public class AttrPanel : UIPanel
{
    private const float PanelWidth = 0.18f;
    private const float RowHeight = 0.015f;
    private const float RowGap = 0.003f;

    private readonly Dictionary<int, UIText> _rows = new();
    private readonly List<AttrRowConfig> _configs = new();
    private Entity _boundUnit;

    public record struct AttrRowConfig(int AttrTypeId, string Label, string Format);

    public AttrPanel() : base("attr") { }

    /// <summary>添加要显示的属性行（调用顺序即显示顺序）</summary>
    public AttrPanel AddRow(int attrTypeId, string label, string format = "{0:F0}/{1:F0}")
    {
        _configs.Add(new AttrRowConfig(attrTypeId, label, format));
        return this;
    }

    public void BindUnit(Entity unit) { _boundUnit = unit; Refresh(); }

    protected override void OnCreate(int parentFrame)
    {
        float totalH = _configs.Count * (RowHeight + RowGap) + 0.01f;

        Root = new UIFrame(parentFrame, "UI\\Widgets\\EscMenu\\Human\\human-options-menu-background.blp");
        Root.SetSize(PanelWidth, totalH);
        Root.SetPos(0.62f, 0.35f);

        for (int i = 0; i < _configs.Count; i++)
        {
            var cfg = _configs[i];
            float y = totalH - 0.005f - RowHeight - i * (RowHeight + RowGap);

            var label = new UIText(Root.Handle, cfg.Label + ":");
            label.SetColor(255, 200, 200, 200);
            FrameHelper.SetSize(label.Handle, PanelWidth * 0.4f, RowHeight);
            FrameHelper.ClearPoints(label.Handle);
            FrameHelper.SetPoint(label.Handle, FramePoint.TopLeft,
                Root.Handle, FramePoint.BottomLeft, 0.005f, y + RowHeight);

            var value = new UIText(Root.Handle, "---");
            value.SetColor(255, 255, 255, 255);
            FrameHelper.SetSize(value.Handle, PanelWidth * 0.5f, RowHeight);
            FrameHelper.ClearPoints(value.Handle);
            FrameHelper.SetPoint(value.Handle, FramePoint.TopLeft,
                label.Handle, FramePoint.TopRight, 0.005f, 0f);

            _rows[cfg.AttrTypeId] = value;
        }
        Hide();
    }

    public override void Refresh()
    {
        if (!IsVisible || _rows.Count == 0 || _boundUnit.IsNull) return;

        foreach (var cfg in _configs)
        {
            if (!_rows.TryGetValue(cfg.AttrTypeId, out var textUI)) continue;

            if (AttrHelper.TryGetAttr(_boundUnit, cfg.AttrTypeId, out var attrEntity)
                && attrEntity.TryGetComponent<AttrValue>(out var val))
            {
                textUI.Text = string.Format(cfg.Format, val.current, val.finalValue);
            }
            else
            {
                textUI.Text = "---";
            }
        }
    }

    public override void Destroy()
    {
        foreach (var t in _rows.Values) t.Destroy();
        _rows.Clear();
        base.Destroy();
    }
}

/// <summary>
/// 属性面板 UI 系统
/// </summary>
public class AttrUISystem : BaseSystem
{
    private AttrPanel? _panel;
    private float _timer;

    protected override void OnUpdateGroupBegin()
    {
        if (_panel == null)
        {
            _panel = new AttrPanel()
                .AddRow(AttributeHelper.Health, "生命值")
                .AddRow(AttributeHelper.Mana, "魔法值")
                .AddRow(AttributeHelper.Damage, "攻击力", "{1:F0}")
                .AddRow(AttributeHelper.HealthRegen, "生命回复", "{1:F1}/s")
                .AddRow(AttributeHelper.ManaRegen, "魔法回复", "{1:F1}/s");

            UIManager.Register(_panel);
        }
    }

    protected override void OnUpdateGroupEnd()
    {
        _timer += Tick.deltaTime;
        if (_timer >= 0.2f) { _timer = 0; _panel?.Refresh(); }
    }
}
