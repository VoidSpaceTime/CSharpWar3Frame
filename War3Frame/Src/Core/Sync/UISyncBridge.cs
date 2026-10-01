using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>UI 同步请求。处理器只写入请求，不直接改游戏状态；由 UISyncResolveSystem 分发后删除。</summary>
public struct UISyncRequest : IComponent
{
    /// <summary>同步前缀。</summary>
    public string Prefix;

    /// <summary>同步数据。</summary>
    public string Data;
}

/// <summary>
/// UI 同步桥。发送走 DzSyncData；接收只落 UISyncRequest。
/// 生命周期：Consume 注册 → Send → 接收入队 → Flush 建实体 → UISyncResolveSystem 分发消费后删除。
/// </summary>
public static class UISyncBridge
{
    private static readonly Queue<UISyncRequest> _pending = new();
    private static readonly HashSet<string> _registeredPrefixes = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, Action<string>> _consumers = new(StringComparer.Ordinal);

    /// <summary>
    /// 单入口注册：登记消费处理器，原生环境再接入 SyncHelper 接收（重复调用幂等）。
    /// 须在原生就绪（War3Init 之后）调用；处理器由 UISyncResolveSystem 分发，所有玩家一致。
    /// </summary>
    public static void Consume(string prefix, Action<string> handler)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefix);
        ArgumentNullException.ThrowIfNull(handler);

        _consumers[prefix] = handler;
        if (UIRuntime.NativeReady && _registeredPrefixes.Add(prefix))
            SyncHelper.Register(prefix, (_, data) => _pending.Enqueue(new UISyncRequest { Prefix = prefix, Data = data }));
    }

    /// <summary>发送一次同步数据。非原生环境直接入队，便于逻辑验证。</summary>
    public static void Send(string prefix, string data)
    {
        if (!UIRuntime.NativeReady)
        {
            _pending.Enqueue(new UISyncRequest { Prefix = prefix, Data = data });
            return;
        }

        AsyncHelper.Send(prefix, data);
    }

    /// <summary>把本帧收到的同步数据写入 ECS 请求。</summary>
    public static void Flush(EntityStore store)
    {
        while (_pending.Count > 0)
            store.CreateEntity(_pending.Dequeue());
    }

    /// <summary>分发一条请求到消费处理器；未注册前缀丢弃（实体仍由消费系统删除）。</summary>
    internal static void Dispatch(string prefix, string data)
    {
        if (_consumers.TryGetValue(prefix, out var handler))
            handler(data);
    }

    /// <summary>测试隔离：清空待处理队列、接收注册与消费处理器。</summary>
    public static void Reset()
    {
        _pending.Clear();
        _consumers.Clear();
        _registeredPrefixes.Clear();
    }
}
