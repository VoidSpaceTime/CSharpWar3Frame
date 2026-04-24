using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 异步数据发送端 - UI 事件调用此类发送同步请求
/// 
/// 调用 AsyncHelper.Send() 会通过 DzSyncData 将数据发送给所有玩家。
/// 数据到达后由 SyncHelper 的对应处理器统一执行游戏逻辑。
/// 
/// 使用示例:
///   // 使用物品（背包第3格，对目标entityB）
///   AsyncHelper.SendAction("inv", "use", entityA, entityB);
///   
///   // 释放技能（对坐标）
///   AsyncHelper.SendAction("abl", "cast", casterEntity, 120.5f, 340.2f);
/// </summary>
public static class AsyncHelper
{
    /// <summary>
    /// 发送原始同步数据
    /// </summary>
    /// <param name="prefix">套件标识（如 "inv", "abl", "buf"）</param>
    /// <param name="data">已编码的数据字符串</param>
    public static void Send(string prefix, string data)
    {
        DzApi.DzSyncData(prefix, data);
    }

    /// <summary>
    /// 发送操作（无额外参数）
    /// 示例: SendAction("inv", "open") → prefix="inv", data="open"
    /// </summary>
    public static void SendAction(string prefix, string action)
    {
        Send(prefix, action);
    }

    /// <summary>
    /// 发送操作 + 源 Entity
    /// 示例: SendAction("inv", "drop", itemEntity) → data="drop|a3"
    /// </summary>
    public static void SendAction(string prefix, string action, Entity source)
    {
        var data = SyncHelper.Encode(action, SyncHelper.EncodeEntity(source));
        Send(prefix, data);
    }

    /// <summary>
    /// 发送操作 + 源 Entity + 目标 Entity
    /// 示例: SendAction("inv", "use", itemEntity, targetEntity) → data="use|a3|b7"
    /// </summary>
    public static void SendAction(string prefix, string action, Entity source, Entity target)
    {
        var data = SyncHelper.Encode(action,
            SyncHelper.EncodeEntity(source),
            SyncHelper.EncodeEntity(target));
        Send(prefix, data);
    }

    /// <summary>
    /// 发送操作 + 源 Entity + 坐标
    /// 示例: SendAction("abl", "cast", caster, 120.5f, 340.2f) → data="cast|1f|4b2|d4e"
    /// </summary>
    public static void SendAction(string prefix, string action, Entity source, float x, float y)
    {
        var data = SyncHelper.Encode(action,
            SyncHelper.EncodeEntity(source),
            SyncHelper.EncodeFloat(x),
            SyncHelper.EncodeFloat(y));
        Send(prefix, data);
    }

    /// <summary>
    /// 发送操作 + 源 Entity + 整数参数
    /// 示例: SendAction("inv", "swap", unit, 2, 5) → data="swap|a3|2|5"
    /// </summary>
    public static void SendAction(string prefix, string action, Entity source, int param1, int param2)
    {
        var data = SyncHelper.Encode(action,
            SyncHelper.EncodeEntity(source),
            param1.ToString(),
            param2.ToString());
        Send(prefix, data);
    }

    /// <summary>
    /// 发送操作 + 整数参数（无 Entity）
    /// 示例: SendAction("ui", "toggle", 1) → data="toggle|1"
    /// </summary>
    public static void SendAction(string prefix, string action, int param)
    {
        var data = SyncHelper.Encode(action, param.ToString());
        Send(prefix, data);
    }
}
