using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 玩家在线状态。
/// </summary>
public enum PlayerState
{
    Playing,
    Leave,
    Empty,
    Computer
}

/// <summary>
/// 玩家阵营关系。
/// </summary>
public enum PlayerTeamState
{
    Allie,
    Enemy,
    Neutral
}

/// <summary>
/// 玩家原生句柄与持久状态。
/// 名称/颜色/联盟位均为 ECS 真相，Native 层通过 PlayerDirty 同步。
/// </summary>
public struct PlayerNative : IComponent
{
    public string name;
    public int color;
    public JPlayer player;
    public int index;
    public Entity getentity;
}

/// <summary>
/// 待同步到原生玩家的脏标记。修改持久状态后由 PlayerHelper 打标。
/// </summary>
public struct PlayerDirty : IComponent
{
    public PlayerDirtyFlags flags;
}

/// <summary>
/// 玩家待同步的原生状态位。
/// </summary>
[Flags]
public enum PlayerDirtyFlags
{
    None = 0,
    Name = 1 << 0,
    Color = 1 << 1,
    Alliance = 1 << 2
}

/// <summary>
/// 玩家间联盟状态（ECS 真相）。
/// bits 按目标玩家 index 索引，每元素低 5 位分别表示
/// BasicAlliance / Vision / Control / FullControl / Neutral。
/// dirty 按目标玩家 index 标记"该目标位已修改，待 Native 同步"，同步后清除，
/// 避免首次同步全量重放 0 位覆盖地图默认结盟。
/// </summary>
public struct PlayerAllianceState : IComponent
{
    public const int AllianceBitBasic = 1 << 0;
    public const int AllianceBitVision = 1 << 1;
    public const int AllianceBitControl = 1 << 2;
    public const int AllianceBitFullControl = 1 << 3;
    public const int AllianceBitNeutral = 1 << 4;

    public byte[] bits;
    public byte[] dirty;

    /// <summary>
    /// 创建指定玩家数量的联盟状态组件（bits/dirty 双数组）。
    /// </summary>
    public static PlayerAllianceState Create(int playerCount)
    {
        return new PlayerAllianceState
        {
            bits = new byte[playerCount],
            dirty = new byte[playerCount]
        };
    }
}