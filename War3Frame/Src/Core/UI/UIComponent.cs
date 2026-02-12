namespace War3Frame;

/// <summary>
/// UI 组件基类 - 封装一个 War3 Frame 句柄
/// </summary>
public abstract class UIComponent
{
    /// <summary>War3 Frame 句柄</summary>
    public int Handle { get; protected set; }

    /// <summary>父组件句柄</summary>
    public int ParentHandle { get; protected set; }

    /// <summary>是否已销毁</summary>
    public bool IsDestroyed { get; private set; }

    /// <summary>是否可见</summary>
    public bool Visible { get; private set; } = true;

    protected UIComponent(int parentHandle)
    {
        ParentHandle = parentHandle;
    }

    /// <summary>设置绝对位置</summary>
    public void SetPos(float x, float y, int point = FramePoint.BottomLeft)
    {
        FrameHelper.SetAbsPos(Handle, x, y, point);
    }

    /// <summary>设置相对位置（相对于另一个 Frame）</summary>
    public void SetPoint(int point, int relFrame, int relPoint, float offsetX = 0, float offsetY = 0)
    {
        FrameHelper.SetPoint(Handle, point, relFrame, relPoint, offsetX, offsetY);
    }

    /// <summary>设置大小</summary>
    public void SetSize(float w, float h) => FrameHelper.SetSize(Handle, w, h);

    /// <summary>显示或隐藏</summary>
    public void Show(bool visible = true)
    {
        Visible = visible;
        FrameHelper.SetVisible(Handle, visible);
    }

    /// <summary>隐藏</summary>
    public void Hide() => Show(false);

    /// <summary>设置透明度 (0-255)</summary>
    public void SetAlpha(int alpha) => FrameHelper.SetAlpha(Handle, alpha);

    /// <summary>销毁 Frame</summary>
    public virtual void Destroy()
    {
        if (IsDestroyed) return;
        IsDestroyed = true;
        FrameHelper.Destroy(Handle);
        Handle = 0;
    }
}

/// <summary>
/// 背景框 - 用于容器、面板背景
/// </summary>
public class UIFrame : UIComponent
{
    public UIFrame(int parent, string? texture = null) : base(parent)
    {
        Handle = FrameHelper.CreateBackdrop(parent, texture);
    }

    /// <summary>设置背景贴图</summary>
    public void SetTexture(string path) => FrameHelper.SetTexture(Handle, path);
}

/// <summary>
/// 文字控件
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
            FrameHelper.SetText(Handle, value);
        }
    }

    public UIText(int parent, string text = "") : base(parent)
    {
        Handle = FrameHelper.CreateText(parent, text);
        _text = text;
    }

    /// <summary>设置文字颜色 (0-255)</summary>
    public void SetColor(int a, int r, int g, int b)
    {
        FrameHelper.SetTextColor(Handle, a, r, g, b);
    }

    /// <summary>设置字体</summary>
    public void SetFont(string fontFile, float height)
    {
        FrameHelper.SetFont(Handle, fontFile, height);
    }
}

/// <summary>
/// 贴图/图标
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
            FrameHelper.SetTexture(Handle, value);
        }
    }

    public UITexture(int parent, string texturePath = "") : base(parent)
    {
        Handle = FrameHelper.CreateBackdrop(parent, texturePath.Length > 0 ? texturePath : null);
        _path = texturePath;
    }
}

/// <summary>
/// 进度条（血条/蓝条/经验条）
/// </summary>
public class UIBar : UIComponent
{
    private float _value;
    private float _min;
    private float _max = 1f;

    /// <summary>当前值（会自动 clamp 到 min~max）</summary>
    public float Value
    {
        get => _value;
        set
        {
            _value = Math.Clamp(value, _min, _max);
            FrameHelper.SetValue(Handle, _value);
        }
    }

    public UIBar(int parent, string? texture = null, float min = 0f, float max = 1f) : base(parent)
    {
        Handle = FrameHelper.CreateStatusBar(parent, texture);
        _min = min;
        _max = max;
        FrameHelper.SetMinMaxValue(Handle, min, max);
    }

    /// <summary>设置进度条贴图</summary>
    public void SetTexture(string path) => FrameHelper.SetTexture(Handle, path);

    /// <summary>设置范围</summary>
    public void SetMinMax(float min, float max)
    {
        _min = min;
        _max = max;
        FrameHelper.SetMinMaxValue(Handle, min, max);
    }
}

/// <summary>
/// 可点击按钮
/// </summary>
public class UIButton : UIComponent
{
    /// <summary>是否启用</summary>
    public bool Enabled { get; private set; } = true;

    /// <summary>图标贴图 Frame</summary>
    public UITexture? Icon { get; private set; }

    public UIButton(int parent, string? iconPath = null) : base(parent)
    {
        // 创建一个 BACKDROP 作为按钮容器（可点击区域）
        Handle = FrameHelper.CreateBackdrop(parent);

        if (iconPath != null)
        {
            Icon = new UITexture(Handle, iconPath);
            FrameHelper.FillParent(Icon.Handle, Handle);
        }
    }

    /// <summary>注册点击事件</summary>
    public void OnClick(Action callback)
    {
        FrameHelper.OnClick(Handle, callback);
    }

    /// <summary>启用/禁用按钮</summary>
    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
        FrameHelper.SetEnable(Handle, enabled);
    }

    /// <summary>设置图标</summary>
    public void SetIcon(string path)
    {
        if (Icon != null)
            Icon.Path = path;
    }

    public override void Destroy()
    {
        Icon?.Destroy();
        base.Destroy();
    }
}

/// <summary>
/// 格子控件（物品/技能格子）
/// 包含：图标 + 冷却遮罩 + 数量文字 + 点击事件
/// </summary>
public class UISlot : UIComponent
{
    /// <summary>图标</summary>
    public UITexture Icon { get; private set; }

    /// <summary>数量文字（右下角）</summary>
    public UIText CountText { get; private set; }

    /// <summary>冷却遮罩 Frame（用于设置动画进度模拟冷却）</summary>
    public int CooldownOverlay { get; private set; }

    /// <summary>该格子是否为空</summary>
    public bool IsEmpty { get; private set; } = true;

    /// <summary>格子索引</summary>
    public int SlotIndex { get; set; }

    public UISlot(int parent, int slotIndex, float size) : base(parent)
    {
        SlotIndex = slotIndex;

        // 主容器（背景 + 点击区域）
        Handle = FrameHelper.CreateBackdrop(parent, "UI\\Widgets\\EscMenu\\Human\\editbox-background.blp");
        FrameHelper.SetSize(Handle, size, size);

        // 图标
        Icon = new UITexture(Handle);
        FrameHelper.FillParent(Icon.Handle, Handle);
        FrameHelper.SetVisible(Icon.Handle, false);

        // 数量文字（右下角）
        CountText = new UIText(Handle, "");
        FrameHelper.SetSize(CountText.Handle, size * 0.4f, size * 0.3f);
        FrameHelper.SetPoint(CountText.Handle, FramePoint.BottomRight,
            Handle, FramePoint.BottomRight, -0.001f, 0.001f);
        FrameHelper.SetVisible(CountText.Handle, false);

        // 冷却遮罩（半透明黑色覆盖层）
        CooldownOverlay = FrameHelper.CreateBackdrop(Handle);
        FrameHelper.FillParent(CooldownOverlay, Handle);
        FrameHelper.SetVisible(CooldownOverlay, false);
        FrameHelper.SetAlpha(CooldownOverlay, 128);
    }

    /// <summary>设置格子内容</summary>
    public void SetContent(string iconPath, int count = 0)
    {
        IsEmpty = false;
        Icon.Path = iconPath;
        FrameHelper.SetVisible(Icon.Handle, true);

        if (count > 1)
        {
            CountText.Text = count.ToString();
            FrameHelper.SetVisible(CountText.Handle, true);
        }
        else
        {
            FrameHelper.SetVisible(CountText.Handle, false);
        }
    }

    /// <summary>清空格子</summary>
    public void Clear()
    {
        IsEmpty = true;
        FrameHelper.SetVisible(Icon.Handle, false);
        FrameHelper.SetVisible(CountText.Handle, false);
        SetCooldown(0);
    }

    /// <summary>设置冷却进度 (0 = 无冷却, 0~1 = 冷却中)</summary>
    public void SetCooldown(float ratio)
    {
        if (ratio <= 0)
        {
            FrameHelper.SetVisible(CooldownOverlay, false);
        }
        else
        {
            FrameHelper.SetVisible(CooldownOverlay, true);
            // 通过调整遮罩大小来模拟冷却效果
            // ratio=1 完全遮盖, ratio=0 无遮盖
        }
    }

    /// <summary>注册点击事件</summary>
    public void OnClick(Action callback)
    {
        FrameHelper.OnClick(Handle, callback);
    }

    /// <summary>注册鼠标悬浮事件（用于显示 Tooltip）</summary>
    public void OnHover(Action? onEnter, Action? onLeave)
    {
        if (onEnter != null)
            FrameHelper.OnMouseEnter(Handle, onEnter);
        if (onLeave != null)
            FrameHelper.OnMouseLeave(Handle, onLeave);
    }

    public override void Destroy()
    {
        Icon.Destroy();
        CountText.Destroy();
        FrameHelper.Destroy(CooldownOverlay);
        base.Destroy();
    }
}

/// <summary>
/// 全局悬浮提示框（单例使用）
/// </summary>
public class UITooltip : UIComponent
{
    /// <summary>标题</summary>
    public UIText Title { get; private set; }

    /// <summary>描述文字</summary>
    public UIText Description { get; private set; }

    public UITooltip(int parent, float width = 0.15f) : base(parent)
    {
        // 背景
        Handle = FrameHelper.CreateBackdrop(parent, "UI\\Widgets\\ToolTips\\Human\\human-tooltip-background.blp");
        FrameHelper.SetSize(Handle, width, 0.08f);
        FrameHelper.SetVisible(Handle, false);

        // 标题
        Title = new UIText(Handle, "");
        Title.SetColor(255, 255, 204, 0); // 金色
        FrameHelper.SetSize(Title.Handle, width - 0.01f, 0.02f);
        FrameHelper.SetPoint(Title.Handle, FramePoint.TopLeft,
            Handle, FramePoint.TopLeft, 0.005f, -0.005f);

        // 描述
        Description = new UIText(Handle, "");
        FrameHelper.SetSize(Description.Handle, width - 0.01f, 0.05f);
        FrameHelper.SetPoint(Description.Handle, FramePoint.TopLeft,
            Title.Handle, FramePoint.BottomLeft, 0f, -0.003f);
    }

    /// <summary>显示 Tooltip</summary>
    public void ShowAt(float x, float y, string title, string description)
    {
        Title.Text = title;
        Description.Text = description;
        SetPos(x, y);
        Show();
    }

    /// <summary>隐藏 Tooltip</summary>
    public new void Hide()
    {
        base.Hide();
    }

    public override void Destroy()
    {
        Title.Destroy();
        Description.Destroy();
        base.Destroy();
    }
}
