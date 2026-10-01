namespace War3Frame;

/// <summary>
/// UI 面板层桶。值越小越靠底；层序即创建序，后创建者压住先创建者。
/// War3 原生层序等于创建序且不可重排，因此层序必须在创建期确定。
/// </summary>
public enum UILayer
{
    /// <summary>背景：全屏遮罩、常驻底板。</summary>
    Background = 0,

    /// <summary>主面板：属性看板、商店等。</summary>
    Panel = 1,

    /// <summary>弹窗：浮在主面板上的二级层。</summary>
    Popup = 2,

    /// <summary>模态：需要用户先处理的对话框。</summary>
    Modal = 3,

    /// <summary>提示：Tooltip、飘字，永远最上。</summary>
    Tooltip = 4
}
