using System;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 验证面板按层注册与创建：创建序由层序决定而非注册调用序，显隐不改层序，未显示不进命中集。
/// 断言路径不调用 DzApi。
/// </summary>
public static class UIPanelLayeringScenario
{
    private const string ScenarioName = "UIPanelLayeringScenario";

    /// <summary>运行不依赖 War3 句柄的同步验证。</summary>
    public static void Initialize(JPlayer player)
    {
        _ = player;
        UIRuntime.NativeReady = false;
        UIHitRegistry.Reset();
        UIInputQueue.Reset();
        UIInput.ResetDispatch();

        var kit = new UIKit();
        kit.Add(new LayeredPanel("Popup"), UILayer.Popup);
        kit.Add(new LayeredPanel("A"), UILayer.Panel);
        kit.Add(new LayeredPanel("B"), UILayer.Panel);
        kit.Add(new LayeredPanel("A"), UILayer.Panel);
        kit.Build();

        var popup = kit.Get<LayeredPanel>("Popup")!;
        var a = kit.Get<LayeredPanel>("A")!;
        var b = kit.Get<LayeredPanel>("B")!;

        Require(b.Root.ZIndex > a.Root.ZIndex, "同层按注册序，后注册者层级更高");
        Require(popup.Root.ZIndex > b.Root.ZIndex, "创建序按层序，而非注册调用序");
        Require(ReferenceEquals(kit.Add(new LayeredPanel("A")), a), "同名 key 幂等");
        Require(UILayout.CompareZ(b.Root, a.Root) > 0, "同层 CompareZ 胜者为后注册者");
        Require(UILayout.CompareZ(popup.Root, b.Root) > 0, "高层面板压过低层");

        a.Root.FlushLayout();
        Require(!UIHitRegistry.Contains(a.Root), "未显示的面板不进命中集");

        var z = b.Root.ZIndex;
        kit.Show("B");
        b.Root.FlushLayout();
        Require(UIHitRegistry.Contains(b.Root), "显示后进入命中集");
        kit.Hide("B");
        Require(b.Root.ZIndex == z, "显隐不改变 ZIndex");

        kit.Destroy();
        UIInput.ResetDispatch();
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {message}");
    }

    /// <summary>测试面板：只创建一个可解析的根控件。</summary>
    private sealed class LayeredPanel : UIPanel
    {
        public LayeredPanel(string name) : base(name)
        {
        }

        protected override void OnCreate(int parentFrame)
        {
            Root = new UIFrame(parentFrame);
            Root.Size = new UIVec(0.1f, 0.1f);
            Root.Position = new UIVec(0.4f, 0.3f);
        }

        public override void Refresh()
        {
        }
    }
}
