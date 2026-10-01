using System.Reflection;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Regression;

/// <summary>
/// native-bridge-completion 回归：WE 预置实体 adoption 与 Silence 原生映射。
/// 不启动 War3：把生产 Native 源文件与替身 JassApi/HandleHelper/DzApi 一起编译，
/// 用记录器断言调用序列与句柄配对，不链接 game.dll。
/// </summary>
internal static class NativeBridgeCompletionRegression
{
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("runtime", "PresetUnitAttackBridge", PresetUnitAttackBridge));
        tests.Add(new("runtime", "SilenceNativeMapping", SilenceNativeMapping));
    }

    /// <summary>预置单位/可破坏物 adoption：防双登记、代理组件、HandleAdd 先于 Register、枚举工具句柄销毁、攻击桥反查。</summary>
    private static void PresetUnitAttackBridge()
    {
        var assembly = CompilationTest.Emit(CompilationTest.Create(
            "global using System; global using System.Collections.Generic;",
            CompilationTest.Read("War3Frame/Src/Systems/Native/NativeEntityIndex.cs"),
            CompilationTest.Read("War3Frame/Src/Systems/Native/PresetEntityAdoption.cs"),
            AdoptionStubs));
        var recording = assembly.GetType("War3Frame.NativeRecording")!;
        var calls = (List<string>)recording.GetField("Calls")!.GetValue(null)!;
        var units = (System.Collections.IList)recording.GetField("Units")!.GetValue(null)!;
        var destructables = (System.Collections.IList)recording.GetField("Destructables")!.GetValue(null)!;

        var store = new EntityStore();
        var h1 = new JUnit(1001);
        var h2 = new JUnit(2001);
        var h3 = new JDestructable(3001);
        var e1 = store.CreateEntity();

        var indexType = assembly.GetType("War3Frame.NativeEntityIndex")!;
        var tryGet = indexType.GetMethod("TryGetByNative")!;
        indexType.GetMethod("Register")!.Invoke(null, [e1, h1]);

        units.Add(h1);
        units.Add(h2);
        destructables.Add(h3);
        calls.Clear();

        assembly.GetType("War3Frame.Systems.Native.PresetEntityAdoption")!
            .GetMethod("AdoptAll")!.Invoke(null, [store]);

        var trace = calls.ToList();

        // 已登记框架实体不被代理覆盖，也不重复 HandleAdd。
        Check.That(Lookup(tryGet, h1, out var e1Back) && e1Back == e1, "已登记句柄必须仍反查到原框架实体");
        Check.That(!e1Back.Tags.Has<PresetNativeTag>(), "框架实体不得带预置标记");
        Check.That(!trace.Contains("add:1001"), "已登记句柄不得再次 HandleAdd");

        // 新预置单位得到带 UnitNative + PresetNativeTag 的代理，且 HandleAdd 恰在 TryGet 与 Register 之间。
        Check.That(Lookup(tryGet, h2, out var e2), "新预置单位必须可反查");
        Check.That(e2.HasComponent<UnitNative>() && e2.Tags.Has<PresetNativeTag>(), "预置单位代理组件");
        Check.That(trace.Count(c => c == "add:2001") == 1, "预置句柄 HandleAdd 恰好一次");
        var firstGet = trace.IndexOf("getid:2001");
        var addAt = trace.IndexOf("add:2001");
        var lastGet = trace.LastIndexOf("getid:2001");
        Check.That(firstGet >= 0 && firstGet < addAt && addAt < lastGet, "HandleAdd 必须发生在 TryGet 与 Register 之间");

        // 预置可破坏物同样得到代理。
        Check.That(Lookup(tryGet, h3, out var e3), "预置可破坏物必须可反查");
        Check.That(e3.HasComponent<DestructableNative>() && e3.Tags.Has<PresetNativeTag>(), "预置可破坏物代理组件");
        Check.That(trace.Count(c => c == "add:3001") == 1, "预置可破坏物 HandleAdd 恰好一次");

        // 枚举工具句柄就地销毁且不进入反查表。
        Check.That(trace.Contains("destroygroup:900002") && trace.Contains("removerect:900001"), "枚举工具句柄必须销毁");
        Check.That(!Lookup(tryGet, new JGroup(900002), out _), "group 工具句柄不得进入反查表");
        Check.That(!Lookup(tryGet, new JRect(900001), out _), "rect 工具句柄不得进入反查表");

        // 攻击桥分支：两侧反查成功才建 UnitAttackedRequest，未登记句柄不产生请求。
        if (Lookup(tryGet, h2, out var attacker) && Lookup(tryGet, h2, out var target))
            store.CreateEntity(new UnitAttackedRequest { attacker = attacker, target = target });
        Check.That(store.Query<UnitAttackedRequest>().Count == 1, "反查命中后必须产生攻击请求");
        var unregistered = new JUnit(7777);
        if (Lookup(tryGet, unregistered, out var a) && Lookup(tryGet, h2, out var t))
            store.CreateEntity(new UnitAttackedRequest { attacker = a, target = t });
        Check.That(store.Query<UnitAttackedRequest>().Count == 1, "未登记句柄不得产生攻击请求");
    }

    /// <summary>Silence 原生映射：进入/解除 ACsp 开关序列、已有非默认等级不触碰、空分支与无句柄不调用。</summary>
    private static void SilenceNativeMapping()
    {
        var assembly = CompilationTest.Emit(CompilationTest.Create(
            "global using System; global using System.Collections.Generic;",
            CompilationTest.Read("War3Frame/Src/Systems/Native/UnitNativeRequestSystems.cs"),
            ControlStubs));
        var recording = assembly.GetType("War3Frame.NativeRecording")!;
        var calls = (List<string>)recording.GetField("Calls")!.GetValue(null)!;
        var levels = (Dictionary<int, int>)recording.GetField("AbilityLevels")!.GetValue(null)!;

        var store = new EntityStore();
        var unit = store.CreateEntity(new UnitNative { unit = new JUnit(7), player = new JPlayer(0) });
        var root = new SystemRoot(store);
        root.Add((BaseSystem)Activator.CreateInstance(
            assembly.GetType("War3Frame.Systems.Native.UnitControlNativeSystem")!)!);

        // 进入：单位原本没有 ACsp → 添加后开启。
        calls.Clear();
        var enter = store.CreateEntity(new ControlStateNativeRequest
        {
            unit = unit,
            controlType = ControlType.Silence,
            entered = true
        });
        root.Update(new UpdateTick(.02f, .02f));
        Check.That(calls.SequenceEqual(new[] { "getlevel:7", "unitadd:7", "order:spellshield:7" }),
            $"进入沉默调用序列异常: {string.Join(",", calls)}");
        Check.That(enter.IsNull, "Silence 请求消费后删除");
        Check.That(!calls.Any(c => c.StartsWith("silence")), "不得调用 DzUnitSilence");

        // 解除：等级为 1 → 关闭后移除。
        calls.Clear();
        var exit = store.CreateEntity(new ControlStateNativeRequest
        {
            unit = unit,
            controlType = ControlType.Silence,
            entered = false
        });
        root.Update(new UpdateTick(.02f, .04f));
        Check.That(calls.SequenceEqual(new[] { "getlevel:7", "order:spellshield:7", "unitremove:7" }),
            $"解除沉默调用序列异常: {string.Join(",", calls)}");
        Check.That(exit.IsNull, "解除请求消费后删除");
        Check.That(!calls.Any(c => c.StartsWith("silence")), "不得调用 DzUnitSilence");

        // 地图已有非默认等级（2）：进入与解除都不添加、不移除。
        levels[7] = 2;
        calls.Clear();
        var mapEnter = store.CreateEntity(new ControlStateNativeRequest
        {
            unit = unit,
            controlType = ControlType.Silence,
            entered = true
        });
        var mapExit = store.CreateEntity(new ControlStateNativeRequest
        {
            unit = unit,
            controlType = ControlType.Silence,
            entered = false
        });
        root.Update(new UpdateTick(.02f, .06f));
        Check.That(calls.All(c => c.StartsWith("getlevel")), $"已有非默认魔法盾不得触碰: {string.Join(",", calls)}");
        Check.That(mapEnter.IsNull && mapExit.IsNull, "两请求均被消费");

        // 空分支控制：不产生任何原生调用。
        calls.Clear();
        var locust = store.CreateEntity(new ControlStateNativeRequest
        {
            unit = unit,
            controlType = ControlType.Locust,
            entered = true
        });
        root.Update(new UpdateTick(.02f, .08f));
        Check.That(calls.Count == 0, "Locust 空分支不得产生原生调用");
        Check.That(locust.IsNull, "空分支请求仍被删除");

        // 无 UnitNative：删除请求，不调用原生。
        calls.Clear();
        var naked = store.CreateEntity();
        var nakedRequest = store.CreateEntity(new ControlStateNativeRequest
        {
            unit = naked,
            controlType = ControlType.Silence,
            entered = true
        });
        root.Update(new UpdateTick(.02f, .1f));
        Check.That(calls.Count == 0, "无 UnitNative 不得产生原生调用");
        Check.That(nakedRequest.IsNull, "无 UnitNative 请求仍被删除");
    }

    private static bool Lookup(MethodInfo tryGet, JHandle handle, out Entity entity)
    {
        var args = new object?[] { handle, null };
        var found = (bool)tryGet.Invoke(null, args)!;
        entity = found ? (Entity)args[1]! : default;
        return found;
    }

    private const string AdoptionStubs = """
        namespace War3Frame {
            public static class NativeRecording {
                public static readonly System.Collections.Generic.List<string> Calls = new();
                public static readonly System.Collections.Generic.List<JUnit> Units = new();
                public static readonly System.Collections.Generic.List<JDestructable> Destructables = new();
                public static JUnit CurrentUnit;
                public static JDestructable CurrentDestructable;
            }
            public static class HandleHelper {
                public static void HandleAdd(JHandle h) => NativeRecording.Calls.Add("add:" + h.Handle.ToInt32());
                public static void HandleRemove(JHandle h) => NativeRecording.Calls.Add("remove:" + h.Handle.ToInt32());
            }
            public static class JassApi {
                public static int GetHandleId(JHandle h) { NativeRecording.Calls.Add("getid:" + h.Handle.ToInt32()); return h.Handle.ToInt32(); }
                public static JRect GetWorldBounds() => new JRect(900001);
                public static JGroup CreateGroup() => new JGroup(900002);
                public static void GroupEnumUnitsInRect(JGroup g, JRect r, JBoolExpr f) { }
                public static void ForGroup(JGroup g, System.Action callback) {
                    foreach (var u in NativeRecording.Units) { NativeRecording.CurrentUnit = u; callback(); }
                }
                public static JUnit GetEnumUnit() => NativeRecording.CurrentUnit;
                public static void EnumDestructablesInRect(JRect r, JBoolExpr f, System.Action callback) {
                    foreach (var d in NativeRecording.Destructables) { NativeRecording.CurrentDestructable = d; callback(); }
                }
                public static JDestructable GetEnumDestructable() => NativeRecording.CurrentDestructable;
                public static void DestroyGroup(JGroup g) => NativeRecording.Calls.Add("destroygroup:" + g.Handle.ToInt32());
                public static void RemoveRect(JRect r) => NativeRecording.Calls.Add("removerect:" + r.Handle.ToInt32());
                public static JPlayer GetOwningPlayer(JUnit u) => new JPlayer(0);
            }
        }
        """;

    private const string ControlStubs = """
        namespace War3Frame {
            public static class NativeRecording {
                public static readonly System.Collections.Generic.List<string> Calls = new();
                public static readonly System.Collections.Generic.Dictionary<int, int> AbilityLevels = new();
            }
            public static class JassApi {
                public static int GetUnitAbilityLevel(JUnit u, int abil) {
                    NativeRecording.Calls.Add("getlevel:" + u.Handle.ToInt32());
                    return NativeRecording.AbilityLevels.TryGetValue(u.Handle.ToInt32(), out var level) ? level : 0;
                }
                public static bool UnitAddAbility(JUnit u, int abil) {
                    NativeRecording.Calls.Add("unitadd:" + u.Handle.ToInt32());
                    NativeRecording.AbilityLevels[u.Handle.ToInt32()] = 1;
                    return true;
                }
                public static bool UnitRemoveAbility(JUnit u, int abil) {
                    NativeRecording.Calls.Add("unitremove:" + u.Handle.ToInt32());
                    NativeRecording.AbilityLevels[u.Handle.ToInt32()] = 0;
                    return true;
                }
                public static bool IssueImmediateOrder(JUnit u, string order) {
                    NativeRecording.Calls.Add("order:" + order + ":" + u.Handle.ToInt32());
                    return true;
                }
                public static bool IssuePointOrder(JUnit u, string order, float x, float y) {
                    NativeRecording.Calls.Add("point:" + order);
                    return true;
                }
                public static void ShowUnit(JUnit u, bool v) { NativeRecording.Calls.Add("show:" + v); }
                public static void SetUnitPathing(JUnit u, bool v) { NativeRecording.Calls.Add("path:" + v); }
                public static void PauseUnit(JUnit u, bool v) { NativeRecording.Calls.Add("pause:" + v); }
            }
            public static class DzApi {
                public static void DzUnitDisableAttack(JUnit u, bool v) { NativeRecording.Calls.Add("disableattack:" + v); }
                public static void DzUnitSilence(JUnit u, bool v) { NativeRecording.Calls.Add("silence:" + v); }
            }
        }
        """;
}
