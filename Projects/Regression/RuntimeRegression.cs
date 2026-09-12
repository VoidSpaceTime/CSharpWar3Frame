using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Src.Systems;
using War3Frame.Systems.Time;

namespace War3Frame.Regression;

internal static class RuntimeRegression
{
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("runtime", "clock-conservation", ClockConservation));
        tests.Add(new("runtime", "once-batch", OnceBatch));
        tests.Add(new("runtime", "buff-refresh", BuffRefresh));
        tests.Add(new("runtime", "buff-catchup", BuffCatchup));
        tests.Add(new("runtime", "attribute-store", AttributeStore));
        tests.Add(new("runtime", "default-registration", DefaultRegistration));
    }

    private static void ClockConservation()
    {
        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        var capture = new ClockCapture();
        root.Add(capture, 0.03125f);
        for (var i = 0; i < 100; i++) root.Update(new UpdateTick(0.01f, i * 0.01f));
        Check.Near(capture.Total, 1, "elapsed time must not be counted twice");
        var before = capture.Total;
        root.Update(new UpdateTick(0.5f, 1.5f));
        Check.Near(capture.Total - before, .5, "a long frame delivers actual elapsed time");
        root.SetInterval(capture, 0);
        root.Update(new UpdateTick(.01f, 1.51f));
        Check.Near(capture.Total, 1.51, "immediate update");
    }

    private static void OnceBatch()
    {
        var store = new EntityStore();
        var root = new SystemRoot(store);
        root.Add(new TriggerSystem());
        var count = 0;
        var action = TriggerActionRegistry.Register((_, _) => count++);
        TriggerHelper.Register(store, new TriggerSpec
        {
            eventTypeId = 90123,
            policy = new TriggerPolicy { kind = TriggerPolicyKind.Once },
            actions = [new TriggerAction { actionId = action }]
        });
        store.CreateEntity(new TriggerEventMarker { eventTypeId = 90123 });
        store.CreateEntity(new TriggerEventMarker { eventTypeId = 90123 });
        root.Update(new UpdateTick(.01f, .01f));
        Check.That(count == 1, "Once must execute only once in a batch");
    }

    private static void BuffRefresh()
    {
        var store = new EntityStore();
        var unit = store.CreateEntity();
        var buff = BuffHelper.AddTimedBuff(store, unit, unit, "regression:refresh",
            AttributeHelper.Health, Components.ModifyType.Flat, 10, .01f);
        var clock = new SystemRoot(store);
        clock.Add(new DurationSystem());
        clock.Update(new UpdateTick(.02f, .02f));
        Check.That(buff.Tags.Has<DurationExpired>(), "fixture must reach expiry");
        BuffHelper.AddTimedBuff(store, unit, unit, "regression:refresh",
            AttributeHelper.Health, Components.ModifyType.Flat, 10, 5);
        var cleanup = new SystemRoot(store);
        cleanup.Add(new BuffDurationSystem());
        cleanup.Add(new BuffExpireSystem());
        cleanup.Update(new UpdateTick(.1f, .1f));
        Check.That(!buff.IsNull && !buff.Tags.Has<DurationExpired>(), "refreshed buff survives cleanup");
        Check.Near(buff.GetComponent<Duration>().remaining, 5, "new remaining duration");
    }

    private static void BuffCatchup()
    {
        var store = new EntityStore();
        var unit = store.CreateEntity();
        BuffHelper.ApplyDoT(store, unit, unit, "regression:tick", 1, .1f, 5);
        var root = new SystemRoot(store);
        root.Add(new DurationSystem());
        root.Add(new BuffTickSystem());
        root.Update(new UpdateTick(.5f, .5f));
        Check.That(store.Query<DamageRequest>().Count == 5, "long frame must produce five due ticks");
    }

    private static void AttributeStore()
    {
        var store = new EntityStore();
        var unit = store.CreateEntity();
        var attr = AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.Health, 100);
        Check.That(ReferenceEquals(attr.Store, store), "attribute must belong to owner's store");
        Check.That(attr.GetComponent<AttrOwner>().owner == unit, "attribute owner identity");
    }

    private static void DefaultRegistration()
    {
        Game.ECSInit();
        Check.That(typeof(SpatialGridSystem).GetCustomAttributes(typeof(Systems.SystemRegisterAttribute), false).Length == 1,
            "grid must be registered");
        var unavailable = typeof(Systems.Native.ItemCreateNativeSystem);
        Check.That(unavailable.GetCustomAttributes(typeof(Systems.SystemRegisterAttribute), false).Length == 0,
            "unimplemented native feature must not be active");
        // Empty store: no native handles or requests, so the real generated chain can safely run.
        Game.Root.Update(new UpdateTick(.01f, .01f));
    }

    private sealed class ClockCapture : QuerySystem<Duration>
    {
        internal float Total;
        protected override void OnUpdate() => Total += Tick.deltaTime;
    }
}
