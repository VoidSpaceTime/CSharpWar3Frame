namespace War3Frame;

/// <summary>
/// Frame 锚点常量（对应 War3 FRAMEPOINT）
/// </summary>
public static class FramePoint
{
    public const int TopLeft = 0;
    public const int Top = 1;
    public const int TopRight = 2;
    public const int Left = 3;
    public const int Center = 4;
    public const int Right = 5;
    public const int BottomLeft = 6;
    public const int Bottom = 7;
    public const int BottomRight = 8;
}

/// <summary>
/// Frame UI 事件类型（对应 DzFrameSetScript 的 eventId）
/// </summary>
public static class FrameEvent
{
    public const int Click = 1;
    public const int MouseEnter = 2;
    public const int MouseLeave = 3;
    public const int MouseUp = 4;
    public const int MouseDown = 5;
    public const int MouseWheel = 6;
    public const int CheckboxChecked = 7;
    public const int CheckboxUnchecked = 8;
    public const int EditboxTextChanged = 9;
    public const int PopupMenuItemChanged = 10;
    public const int MouseDoubleClick = 11;
    public const int SpriteAnimUpdate = 12;
    public const int SliderValueChanged = 13;
    public const int DialogCancel = 14;
    public const int DialogAccept = 15;
    public const int EditboxEnter = 16;
}

/// <summary>
/// Frame 帮助类 - 封装 DzApi 的 Frame 操作
/// 提供语义化的 API 供 UIComponent 调用
/// </summary>
public static class FrameHelper
{
    #region 创建 / 销毁

    /// <summary>获取游戏根 UI 节点</summary>
    public static int GetGameUI() => DzApi.DzGetGameUI();

    /// <summary>
    /// 通过 Tag 创建 Frame（可重复创建同名）
    /// </summary>
    /// <param name="frameType">fdf 中的类型名，如 "BACKDROP", "TEXT", "SIMPLESTATUSBAR"</param>
    /// <param name="name">自定义名字</param>
    /// <param name="parent">父 Frame 句柄</param>
    /// <param name="template">fdf 模板名</param>
    /// <param name="id">ID，默认 0</param>
    public static int CreateByTag(string frameType, string name, int parent, string template, int id = 0)
    {
        return DzApi.DzCreateFrameByTagName(frameType, name, parent, template, id);
    }

    /// <summary>
    /// 通过 fdf 名创建 Frame
    /// </summary>
    public static int Create(string fdfName, int parent, int id = 0)
    {
        return DzApi.DzCreateFrame(fdfName, parent, id);
    }

    /// <summary>创建 SimpleFrame</summary>
    public static int CreateSimple(string fdfName, int parent, int id = 0)
    {
        return DzApi.DzCreateSimpleFrame(fdfName, parent, id);
    }

    /// <summary>销毁 Frame</summary>
    public static void Destroy(int frame) => DzApi.DzDestroyFrame(frame);

    #endregion

    #region 快捷创建（通过 Tag）

    private static int _tagCounter = 0;
    private static string NextTag(string prefix) => $"{prefix}_{_tagCounter++}";

    /// <summary>创建背景框（BACKDROP）</summary>
    public static int CreateBackdrop(int parent, string? texture = null)
    {
        var frame = CreateByTag("BACKDROP", NextTag("bd"), parent, "");
        if (texture != null)
            SetTexture(frame, texture);
        return frame;
    }

    /// <summary>创建文字控件（TEXT）</summary>
    public static int CreateText(int parent, string? text = null)
    {
        var frame = CreateByTag("TEXT", NextTag("txt"), parent, "");
        if (text != null)
            SetText(frame, text);
        return frame;
    }

    /// <summary>创建状态条（SIMPLESTATUSBAR）</summary>
    public static int CreateStatusBar(int parent, string? texture = null)
    {
        var frame = CreateByTag("SIMPLESTATUSBAR", NextTag("bar"), parent, "");
        DzApi.DzFrameSetMinMaxValue(frame, 0f, 1f);
        if (texture != null)
            SetTexture(frame, texture);
        return frame;
    }

    #endregion

    #region 位置

    /// <summary>设置绝对位置（锚点默认 BottomLeft）</summary>
    public static void SetAbsPos(int frame, float x, float y, int point = FramePoint.BottomLeft)
    {
        DzApi.DzFrameClearAllPoints(frame);
        DzApi.DzFrameSetAbsolutePoint(frame, point, x, y);
    }

    /// <summary>设置相对位置</summary>
    public static void SetPoint(int frame, int point, int relFrame, int relPoint, float offsetX, float offsetY)
    {
        DzApi.DzFrameSetPoint(frame, point, relFrame, relPoint, offsetX, offsetY);
    }

    /// <summary>清空所有锚点</summary>
    public static void ClearPoints(int frame) => DzApi.DzFrameClearAllPoints(frame);

    /// <summary>填充父 Frame</summary>
    public static void FillParent(int frame, int parent) => DzApi.DzFrameSetAllPoints(frame, parent);

    #endregion

    #region 大小

    /// <summary>设置大小</summary>
    public static void SetSize(int frame, float w, float h) => DzApi.DzFrameSetSize(frame, w, h);

    #endregion

    #region 显示 / 隐藏

    /// <summary>显示或隐藏</summary>
    public static void SetVisible(int frame, bool visible) => DzApi.DzFrameShow(frame, visible);

    /// <summary>是否可见</summary>
    public static bool IsVisible(int frame) => DzApi.DzFrameIsVisible(frame);

    #endregion

    #region 文本

    /// <summary>设置文本</summary>
    public static void SetText(int frame, string text) => DzApi.DzFrameSetText(frame, text);

    /// <summary>获取文本</summary>
    public static string GetText(int frame) => DzApi.DzFrameGetText(frame);

    /// <summary>设置文本颜色</summary>
    public static void SetTextColor(int frame, int a, int r, int g, int b)
    {
        DzApi.DzFrameSetTextColor(frame, DzApi.DzGetColor(r, g, b, a));
    }

    /// <summary>设置字体</summary>
    public static void SetFont(int frame, string fontFile, float height, int flag = 0)
    {
        DzApi.DzFrameSetFont(frame, fontFile, height, flag);
    }

    /// <summary>设置对齐方式</summary>
    public static void SetTextAlignment(int frame, int align) => DzApi.DzFrameSetTextAlignment(frame, align);

    #endregion

    #region 贴图 / 模型

    /// <summary>设置贴图（flag: 0 = 不平铺）</summary>
    public static void SetTexture(int frame, string path, int flag = 0)
    {
        DzApi.DzFrameSetTexture(frame, path, flag);
    }

    /// <summary>设置模型</summary>
    public static void SetModel(int frame, string modelPath, int modelType = 0, int flag = 0)
    {
        DzApi.DzFrameSetModel(frame, modelPath, modelType, flag);
    }

    #endregion

    #region 进度条 / 值

    /// <summary>设置当前值（用于 StatusBar/Slider）</summary>
    public static void SetValue(int frame, float value) => DzApi.DzFrameSetValue(frame, value);

    /// <summary>获取当前值</summary>
    public static float GetValue(int frame) => DzApi.DzFrameGetValue(frame);

    /// <summary>设置最大最小值</summary>
    public static void SetMinMaxValue(int frame, float min, float max)
    {
        DzApi.DzFrameSetMinMaxValue(frame, min, max);
    }

    #endregion

    #region 透明度 / 颜色

    /// <summary>设置透明度 (0-255)</summary>
    public static void SetAlpha(int frame, int alpha) => DzApi.DzFrameSetAlpha(frame, alpha);

    /// <summary>获取透明度</summary>
    public static int GetAlpha(int frame) => DzApi.DzFrameGetAlpha(frame);

    /// <summary>设置顶点颜色</summary>
    public static void SetVertexColor(int frame, int a, int r, int g, int b)
    {
        DzApi.DzFrameSetVertexColor(frame, DzApi.DzGetColor(r, g, b, a));
    }

    #endregion

    #region 缩放 / 优先级

    /// <summary>设置缩放</summary>
    public static void SetScale(int frame, float scale) => DzApi.DzFrameSetScale(frame, scale);

    /// <summary>设置层级优先级</summary>
    public static void SetPriority(int frame, int priority) => DzApi.DzFrameSetPriority(frame, priority);

    /// <summary>设置父节点</summary>
    public static void SetParent(int frame, int parent) => DzApi.DzFrameSetParent(frame, parent);

    #endregion

    #region 启用 / 禁用

    /// <summary>启用或禁用（按钮灰化）</summary>
    public static void SetEnable(int frame, bool enable) => DzApi.DzFrameSetEnable(frame, enable);

    /// <summary>是否启用</summary>
    public static bool IsEnabled(int frame) => DzApi.DzFrameGetEnable(frame);

    #endregion

    #region 事件

    /// <summary>注册 UI 事件（同步）</summary>
    public static void OnEvent(int frame, int eventId, Action callback, bool sync = true)
    {
        DzApi.DzFrameSetScriptByCode(frame, eventId, callback, sync);
    }

    /// <summary>注册点击事件</summary>
    public static void OnClick(int frame, Action callback, bool sync = true)
    {
        OnEvent(frame, FrameEvent.Click, callback, sync);
    }

    /// <summary>注册鼠标进入事件（异步，用于 Tooltip）</summary>
    public static void OnMouseEnter(int frame, Action callback)
    {
        DzApi.DzFrameSetScriptByCodeAsync(frame, FrameEvent.MouseEnter, callback);
    }

    /// <summary>注册鼠标离开事件（异步，用于 Tooltip）</summary>
    public static void OnMouseLeave(int frame, Action callback)
    {
        DzApi.DzFrameSetScriptByCodeAsync(frame, FrameEvent.MouseLeave, callback);
    }

    /// <summary>设置 Tooltip</summary>
    public static void SetTooltip(int frame, int tooltipFrame)
    {
        DzApi.DzFrameSetTooltip(frame, tooltipFrame);
    }

    /// <summary>获取触发事件的玩家</summary>
    public static JPlayer GetEventPlayer() => DzApi.DzGetTriggerUIEventPlayer();

    /// <summary>获取触发事件的 Frame</summary>
    public static int GetEventFrame() => DzApi.DzGetTriggerUIEventFrame();

    #endregion

    #region 原生 UI 获取

    /// <summary>获取原生技能按钮</summary>
    public static int GetCommandButton(int row, int col) => DzApi.DzFrameGetCommandBarButton(row, col);

    /// <summary>获取原生物品栏按钮</summary>
    public static int GetItemButton(int index) => DzApi.DzFrameGetItemBarButton(index);

    /// <summary>获取原生英雄按钮</summary>
    public static int GetHeroButton(int index) => DzApi.DzFrameGetHeroBarButton(index);

    /// <summary>获取原生英雄血条</summary>
    public static int GetHeroHPBar(int index) => DzApi.DzFrameGetHeroHPBar(index);

    /// <summary>获取原生英雄蓝条</summary>
    public static int GetHeroManaBar(int index) => DzApi.DzFrameGetHeroManaBar(index);

    /// <summary>获取原生小地图</summary>
    public static int GetMinimap() => DzApi.DzFrameGetMinimap();

    /// <summary>获取原生大头像</summary>
    public static int GetPortrait() => DzApi.DzFrameGetPortrait();

    /// <summary>获取原生 Tooltip 框</summary>
    public static int GetTooltipFrame() => DzApi.DzFrameGetTooltip();

    /// <summary>隐藏原生界面</summary>
    public static void HideInterface() => DzApi.DzFrameHideInterface();

    /// <summary>修改黑边</summary>
    public static void EditBlackBorders(float upper, float bottom)
    {
        DzApi.DzFrameEditBlackBorders(upper, bottom);
    }

    /// <summary>加载 TOC 文件</summary>
    public static void LoadToc(string fileName) => DzApi.DzLoadToc(fileName);

    #endregion

    #region 动画

    /// <summary>设置动画</summary>
    public static void SetAnimate(int frame, int animId, bool autocast)
    {
        DzApi.DzFrameSetAnimate(frame, animId, autocast);
    }

    /// <summary>设置动画进度</summary>
    public static void SetAnimateOffset(int frame, float offset)
    {
        DzApi.DzFrameSetAnimateOffset(frame, offset);
    }

    #endregion

    #region 查找

    /// <summary>按名字查找 Frame</summary>
    public static int FindByName(string name, int id = 0) => DzApi.DzFrameFindByName(name, id);

    /// <summary>按名字查找 SimpleFrame</summary>
    public static int FindSimpleByName(string name, int id = 0) => DzApi.DzSimpleFrameFindByName(name, id);

    #endregion
}
