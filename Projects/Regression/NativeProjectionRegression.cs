using System.Reflection;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Regression;

internal static class NativeProjectionRegression
{
    internal static void Register(List<RegressionCase> tests)
        => tests.Add(new("native-projection", "production-source-call-order", Projection));

    private static void Projection()
    {
        // 编译原样 Native 系统源文件，仅把原生函数替换为记录器。真实 ECS/Helper 来自框架程序集。
        var assembly = CompilationTest.Emit(CompilationTest.Create(
            "global using System; global using System.Collections.Generic;",
            CompilationTest.Read("War3Frame/Src/Systems/Native/EffectNativeSystem.cs"),
            CompilationTest.Read("War3Frame/Src/Systems/Native/PlayerNativeSystem.cs"),
            Stubs));
        var recording = assembly.GetType("War3Frame.NativeRecording")!;
        var calls = (List<string>)recording.GetField("Calls")!.GetValue(null)!;
        var store = new EntityStore();
        var effect = store.CreateEntity(new EffectBase { model = "test", sizeScale = 2, visible = true }, new Position());
        EffectHelper.RotateZ(effect, 10);
        var root = new SystemRoot(store);
        root.Add((BaseSystem)Activator.CreateInstance(assembly.GetType("War3Frame.EffectNativeSystem")!)!);
        root.Update(new UpdateTick(.02f, .02f));
        Check.That(calls[0] == "create" && calls[1] == "add", "handle registration immediately follows creation");
        Check.That(!effect.HasComponent<EffectDirty>(), "first sync consumes dirty state");
        calls.Clear();
        for (var tick = 0; tick < 100; tick++) root.Update(new UpdateTick(.02f, tick * .02f));
        Check.That(calls.Count == 0, "stationary effects do not repeat position setters");
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var tick = 0; tick < 1000; tick++) root.Update(new UpdateTick(.02f, tick * .02f));
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Check.That(calls.Count == 0, "warmed stationary projection has no native calls");
        Console.WriteLine($"MEASURE stationary-effect: 1000 updates, {calls.Count} native calls, {allocated} allocated bytes");
        Check.That(allocated == 0, "warmed stationary projection does not allocate per update");
        effect.GetComponent<Position>().x = 5;
        root.Update(new UpdateTick(.02f, 2.02f));
        Check.That(calls.SequenceEqual(new[] { "xy" }), "XY-only movement does not rewrite Z");
        calls.Clear();
        effect.GetComponent<Position>().z = 7;
        root.Update(new UpdateTick(.02f, 2.04f));
        Check.That(calls.SequenceEqual(new[] { "z" }), "Z-only movement does not rewrite XY");
        EffectHelper.RotateZ(effect, 10);
        root.Update(new UpdateTick(.02f, .04f));
        Check.Near((float)recording.GetField("Angle")!.GetValue(null)!, 20, "native cumulative rotation equals ECS");
        Check.Near((float)recording.GetField("Scale")!.GetValue(null)!, 2, "reset preserves scale");
        Check.That(calls.LastIndexOf("reset") < calls.LastIndexOf("scale"), "scale follows matrix reset");
        EffectHelper.ResetTransform(effect);
        EffectHelper.SetScale(effect, 3);
        root.Update(new UpdateTick(.02f, .06f));
        Check.Near((float)recording.GetField("Angle")!.GetValue(null)!, 0, "explicit reset");
        Check.Near((float)recording.GetField("Scale")!.GetValue(null)!, 3, "same-frame reset and scale");
        EffectHelper.Destroy(effect);
        root.Update(new UpdateTick(.02f, .08f));
        Check.That(effect.IsNull && calls[^2] == "remove" && calls[^1] == "destroy", "unregister before unique destruction");

        var projectAlliance = assembly.GetType("War3Frame.Systems.Native.PlayerNativeSyncSystem")!
            .GetMethod("ApplyAllianceBits", BindingFlags.Static | BindingFlags.NonPublic)!;
        for (byte bits = 0; bits < 32; bits++)
        {
            calls.Clear();
            projectAlliance.Invoke(null, [new JPlayer(1), new JPlayer(2), bits]);
            var relation = PlayerHelper.GetRelationFromBits(bits);
            Check.That(calls[0] == "alliance:0:" + (relation != PlayerTeamState.Enemy), "native passive matches ECS relation");
            Check.That(calls[1] == "alliance:1:" + (relation == PlayerTeamState.Allie), "native help matches ECS relation");
            Check.That(calls.Count == 7, "each native alliance bit projected once");
        }
    }

    private const string Stubs = """
        namespace War3Frame {
            public static class NativeRecording {
                public static readonly System.Collections.Generic.List<string> Calls = new();
                public static float Angle, Scale;
            }
            public static class HandleHelper {
                public static void HandleAdd(JHandle h) => NativeRecording.Calls.Add("add");
                public static void HandleRemove(JHandle h) => NativeRecording.Calls.Add("remove");
            }
            public static class JassApi {
                public static JEffect AddSpecialEffect(string m,float x,float y) { NativeRecording.Calls.Add("create"); return new JEffect(1); }
                public static JEffect AddSpecialEffectTarget(string m,JUnit u,string p) { NativeRecording.Calls.Add("create"); return new JEffect(1); }
                public static void DestroyEffect(JEffect e) => NativeRecording.Calls.Add("destroy");
                public static void SetPlayerName(JPlayer p,string n) {}
                public static void SetPlayerColor(JPlayer p,JPlayerColor c) {}
                public static int ConvertPlayerColor(int c) => c;
                public static void SetPlayerAlliance(JPlayer p,JPlayer q,int a,bool value) => NativeRecording.Calls.Add("alliance:" + a + ":" + value);
            }
            public static class Blizzard {
                public const int ALLIANCE_PASSIVE=0, ALLIANCE_HELP_REQUEST=1, ALLIANCE_HELP_RESPONSE=2, ALLIANCE_SHARED_SPELLS=3,
                    ALLIANCE_SHARED_VISION=4, ALLIANCE_SHARED_CONTROL=5, ALLIANCE_SHARED_ADVANCED_CONTROL=6;
            }
            public static class YDApi {
                public static void EXSetEffectXY(JEffect e,float x,float y) => NativeRecording.Calls.Add("xy");
                public static void EXSetEffectZ(JEffect e,float z) => NativeRecording.Calls.Add("z");
                public static void EXSetEffectSize(JEffect e,float value) { NativeRecording.Scale=value; NativeRecording.Calls.Add("scale"); }
                public static void EXSetEffectSpeed(JEffect e,float v) {}
                public static void EXEffectMatReset(JEffect e) { NativeRecording.Angle=0; NativeRecording.Scale=1; NativeRecording.Calls.Add("reset"); }
                public static void EXEffectMatRotateX(JEffect e,float v) {}
                public static void EXEffectMatRotateY(JEffect e,float v) {}
                public static void EXEffectMatRotateZ(JEffect e,float v) { NativeRecording.Angle+=v; }
            }
            public static class KKApi {
                public static void DzSetEffectVertexAlpha(JEffect e,int a) {}
                public static void DzSetEffectVertexColor(JEffect e,int c) {}
                public static void DzSetEffectTeamColor(JEffect e,int c) {}
                public static void DzSetEffectVisible(JEffect e,bool v) {}
                public static void DzPlayEffectAnimation(JEffect e,string a,string l) {}
            }
            public static class DzApi { public static int DzGetColor(int r,int g,int b,int a) => 0; }
        }
        """;
}
