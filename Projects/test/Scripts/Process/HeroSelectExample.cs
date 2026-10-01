using System;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Src.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 英雄选择页示例：横排头像，双击直接投递控件事件，经同步桥落 ECS 请求，
/// 由 UISyncResolveSystem 分发到消费处理器并删除。断言路径不调用 DzApi。
/// </summary>
public static class HeroSelectExample
{
    private const string ScenarioName = "HeroSelectExample";

    /// <summary>运行不依赖 War3 句柄的全链路同步验证。</summary>
    public static void Initialize(JPlayer player)
    {
        _ = player;
        UIRuntime.NativeReady = false;
        UIHitRegistry.Reset();
        UIInputQueue.Reset();
        UIInput.ResetDispatch();
        UISyncBridge.Reset();

        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        root.Add(new UISyncResolveSystem(), 0f);

        string? consumed = null;
        UISyncBridge.Consume("hero", hero => consumed = hero);

        var kit = new UIKit();
        var panel = new HeroSelectPanel();
        kit.Add(panel, UILayer.Panel);
        kit.Build();
        kit.Show("HeroSelect");
        panel.Root.FlushLayout();
        UIInput.Root = panel.Root;

        // 第一次单击：不触发选择，无请求、无残留。
        Press(0.28f, 0.01f);
        Release(0.28f, 0.015f);
        Resolve(store, root);
        Require(consumed == null && store.Query<UISyncRequest>().Count == 0, "单击不选择英雄");

        // 第二次单击（窗口内）：构成双击 → 请求被消费系统分发后删除。
        Press(0.28f, 0.02f);
        Release(0.28f, 0.025f);
        Resolve(store, root);
        Require(consumed == "英雄1", "双击第一个头像选择英雄1");
        Require(store.Query<UISyncRequest>().Count == 0, "请求消费后删除");

        // 双击第二个头像。
        Press(0.40f, 0.03f);
        Release(0.40f, 0.035f);
        Press(0.40f, 0.04f);
        Release(0.40f, 0.045f);
        Resolve(store, root);
        Require(consumed == "英雄2", "双击第二个头像选择英雄2");
        Require(store.Query<UISyncRequest>().Count == 0, "请求消费后删除");

        // 超出双击窗口：两次单击不构成双击，不产生新请求。
        consumed = null;
        Press(0.52f, 1.0f);
        Release(0.52f, 1.005f);
        Press(0.52f, 1.5f);
        Release(0.52f, 1.505f);
        Resolve(store, root);
        Require(consumed == null && store.Query<UISyncRequest>().Count == 0, "超出双击窗口不触发选择");

        kit.Destroy();
        UIInput.ResetDispatch();
        UISyncBridge.Reset();
    }

    /// <summary>把待处理同步数据落成 ECS 请求，并推进消费系统（模拟一个 tick）。</summary>
    private static void Resolve(EntityStore store, TimedSystemRoot root)
    {
        UISyncBridge.Flush(store);
        root.Update(new UpdateTick(0.01f, 0f));
    }

    private static void Press(float x, float time)
    {
        UIInputQueue.Enqueue(new UIRawInput(UIRawKind.LeftDown, x, 0.3f, 0));
        UIInput.Pump(time);
    }

    private static void Release(float x, float time)
    {
        UIInputQueue.Enqueue(new UIRawInput(UIRawKind.LeftUp, x, 0.3f, 0));
        UIInput.Pump(time);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {message}");
    }
}

/// <summary>英雄选择页：横排三个头像，双击发送同步请求。</summary>
public sealed class HeroSelectPanel : UIPanel
{
    /// <summary>三个可选英雄名。</summary>
    public static readonly string[] HeroNames = ["英雄1", "英雄2", "英雄3"];

    private static readonly string[] HeroIcons =
    [
        "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin.blp",
        "ReplaceableTextures\\CommandButtons\\BTNHeroArchMage.blp",
        "ReplaceableTextures\\CommandButtons\\BTNHeroMountainKing.blp"
    ];

    /// <summary>构造英雄选择页。</summary>
    public HeroSelectPanel() : base("HeroSelect")
    {
    }

    /// <summary>创建横排头像。双击只发同步数据。</summary>
    protected override void OnCreate(int parentFrame)
    {
        Root = new UIFrame(parentFrame);
        Root.Size = new UIVec(0.5f, 0.18f);
        Root.Position = new UIVec(0.4f, 0.3f);

        var row = new HBoxContainer(Root.Handle) { Spacing = 0.02f };
        Root.AddChild(row);
        row.Size = new UIVec(0.4f, 0.12f);
        row.Position = new UIVec(0.4f, 0.3f);
        row.FlushLayout();
        for (var i = 0; i < HeroNames.Length; i++)
        {
            var hero = HeroNames[i];
            var portrait = new UIFrame(Root.Handle, HeroIcons[i]);
            row.AddChild(portrait);
            portrait.Size = new UIVec(0.12f, 0.12f);
            portrait.DoublePressed += _ => UISyncBridge.Send("hero", hero);
        }

        row.Arrange();
    }

    /// <summary>本示例不读 ECS 数据刷新。</summary>
    public override void Refresh()
    {
    }
}
