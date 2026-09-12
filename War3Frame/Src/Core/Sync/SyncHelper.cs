using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 同步数据接收端 - 注册 prefix 处理器，接收所有玩家同步过来的数据
/// 
/// War3 的 UI 操作是异步的（只有操作玩家本地触发）。
/// 要执行游戏逻辑必须先通过 DzSyncData 同步给所有玩家，
/// 所有玩家在 SyncHelper 回调中统一执行，保证 lockstep 一致。
/// </summary>
public static class SyncHelper
{
    private static readonly SortedDictionary<string, Action<JPlayer, string>> _handlers = new(StringComparer.Ordinal);
    private static JTrigger _syncTrigger;
    private static bool _initialized;

    /// <summary>Entity Store 引用（用于 DecodeEntity）</summary>
    public static EntityStore? Store { get; set; }

    /// <summary>
    /// 初始化同步系统 — 在游戏开始时调用一次
    /// 创建 trigger 并注册同步事件
    /// </summary>
    public static void Initialize(EntityStore store)
    {
        if (_initialized) return;
        _initialized = true;
        Store = store;

        _syncTrigger = JassApi.CreateTrigger();
        JassApi.TriggerAddAction(_syncTrigger, OnSyncReceived);
    }

    /// <summary>
    /// 注册同步前缀处理器
    /// 每个套件注册自己的 prefix（如 "inv", "abl", "buf"）
    /// </summary>
    /// <param name="prefix">前缀标识（最多 3 字符以节省同步带宽）</param>
    /// <param name="handler">处理回调 (发送者玩家, 数据字符串)</param>
    public static void Register(string prefix, Action<JPlayer, string> handler)
    {
        _handlers[prefix] = handler;

        // 注册 War3 同步事件监听
        DzApi.DzTriggerRegisterSyncData(_syncTrigger, prefix, false);
    }

    /// <summary>
    /// Trigger 回调 — 所有玩家都会执行
    /// </summary>
    private static void OnSyncReceived()
    {
        var prefix = DzApi.DzGetTriggerSyncPrefix();
        var data = DzApi.DzGetTriggerSyncData();
        var player = DzApi.DzGetTriggerSyncPlayer();

        if (_handlers.TryGetValue(prefix, out var handler))
        {
            handler(player, data);
        }
    }

    #region Entity 编解码

    // 使用 Base36 编码 Entity.Id，比十进制更紧凑
    private const string Base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz";

    /// <summary>编码当前同步 Store 的实体身份。e1:id:revision 不占用消息分隔符 |。</summary>
    public static string EncodeEntity(Entity entity)
    {
        if (entity.IsNull || Store == null || !ReferenceEquals(entity.Store, Store))
            throw new ArgumentException("同步实体必须存活并属于当前 Store", nameof(entity));
        return $"e1:{IntToBase36(entity.Id)}:{IntToBase36(entity.Revision)}";
    }

    /// <summary>将紧凑字符串解码为 Entity</summary>
    public static Entity DecodeEntity(string encoded)
    {
        return TryDecodeEntity(encoded, out var entity) ? entity : default;
    }

    public static bool TryDecodeEntity(string? encoded, out Entity entity)
    {
        entity = default;
        if (Store == null || encoded == null || encoded.Length > 24) return false;
        var parts = encoded.Split(':');
        if (parts.Length != 3 || parts[0] != "e1"
            || !TryBase36ToInt(parts[1], out var id) || id <= 0
            || !TryBase36ToInt(parts[2], out var revision) || revision < short.MinValue || revision > short.MaxValue
            || !Store.TryGetEntityById(id, out var candidate) || candidate.IsNull || candidate.Revision != revision)
            return false;
        entity = candidate;
        return true;
    }

    /// <summary>将 int 编码为 Base36 字符串</summary>
    public static string IntToBase36(int value)
    {
        if (value == 0) return "0";

        var result = "";
        bool negative = value < 0;
        long magnitude = negative ? -(long)value : value;

        while (magnitude > 0)
        {
            result = Base36Chars[(int)(magnitude % 36)] + result;
            magnitude /= 36;
        }

        return negative ? "-" + result : result;
    }

    /// <summary>将 Base36 字符串解码为 int</summary>
    public static int Base36ToInt(string encoded)
    {
        if (!TryBase36ToInt(encoded, out var value))
            throw new FormatException("无效或超出 Int32 范围的 Base36 数值");
        return value;
    }

    private static bool TryBase36ToInt(string encoded, out int value)
    {
        value = 0;
        if (string.IsNullOrEmpty(encoded) || encoded.Length > 8) return false;
        bool negative = encoded[0] == '-';
        int start = negative ? 1 : 0;
        if (start == encoded.Length) return false;
        long result = 0;

        for (int i = start; i < encoded.Length; i++)
        {
            result *= 36;
            char c = encoded[i];
            if (c >= '0' && c <= '9')
                result += c - '0';
            else if (c >= 'a' && c <= 'z')
                result += c - 'a' + 10;
            else
                return false;
            if (result > (negative ? 2147483648L : int.MaxValue)) return false;
        }

        value = (int)(negative ? -result : result);
        return true;
    }

    #endregion

    #region 数据编解码

    private const char Separator = '|';

    /// <summary>
    /// 编码多个参数为紧凑字符串
    /// 示例: Encode("use", entity1, entity2) → "use|a3|b7"
    /// </summary>
    public static string Encode(string action, params string[] args)
    {
        if (args.Length == 0) return action;
        return action + Separator + string.Join(Separator, args);
    }

    /// <summary>
    /// 解码紧凑字符串为参数数组
    /// 示例: Decode("use|a3|b7") → ["use", "a3", "b7"]
    /// </summary>
    public static string[] Decode(string data)
    {
        return data.Split(Separator);
    }

    /// <summary>
    /// 编码浮点数（保留1位小数，减少字符数）
    /// </summary>
    public static string EncodeFloat(float value)
    {
        return ((int)(value * 10)).ToString();
    }

    /// <summary>
    /// 解码浮点数
    /// </summary>
    public static float DecodeFloat(string encoded)
    {
        return int.Parse(encoded) / 10f;
    }

    #endregion
}
