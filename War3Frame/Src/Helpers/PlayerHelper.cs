using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 玩家领域 Helper。持久状态（名称/颜色/联盟）写入 ECS 组件并打 Dirty，
/// Native 层消费 PlayerDirty 同步到 War3；静态数组仅为查询缓存镜像。
/// </summary>
public static class PlayerHelper
{
    private const int MaxPlayers = 16;

    // 关系矩阵是 ECS 外的快速查询缓存；联盟状态真相在 PlayerAllianceState 组件。
    private static readonly PlayerTeamState[,] Relations = new PlayerTeamState[MaxPlayers, MaxPlayers];
    private static PlayerNative[] _players = Array.Empty<PlayerNative>();

    /// <summary>
    /// 全部玩家的查询镜像（ECS 组件的值拷贝）。
    /// </summary>
    public static ReadOnlySpan<PlayerNative> Players => _players;

    /// <summary>
    /// 按索引取玩家镜像（ref 返回，调用方不得直接改字段绕过 Dirty）。
    /// </summary>
    public static ref PlayerNative GetPlayer(int index)
    {
        return ref _players[index];
    }

    /// <summary>
    /// 初始化玩家镜像数组：建立默认敌对关系缓存，并为每个玩家实体挂载联盟状态组件。
    /// </summary>
    public static void InitializePlayers(ref PlayerNative[] players)
    {
        _players = players;

        // 初始化默认敌对关系，同一玩家视为友方，后续通过 SetAlliance/SetNeutral 覆盖。
        foreach (var sourcePlayer in _players)
        foreach (var targetPlayer in _players)
        {
            Relations[sourcePlayer.index, targetPlayer.index] = sourcePlayer.index == targetPlayer.index
                ? PlayerTeamState.Allie
                : PlayerTeamState.Enemy;
        }

        // 玩家实体挂载联盟状态组件（ECS 真相）。
        foreach (var player in _players)
        {
            player.getentity.AddComponent(PlayerAllianceState.Create(MaxPlayers));
        }
    }

    /// <summary>
    /// 设置玩家名称：写 ECS PlayerNative 组件 + 镜像数组，并打 Name Dirty。
    /// </summary>
    public static void SetName(ref PlayerNative player, string name)
    {
        // ECS 组件是真相：更新实体上的 PlayerNative，再同步数组镜像。
        player.name = name;
        player.getentity.AddComponent(player);
        _players[player.index] = player;

        MarkDirty(player, PlayerDirtyFlags.Name);
    }

    /// <summary>
    /// 设置玩家颜色：写 ECS PlayerNative 组件 + 镜像数组，并打 Color Dirty。
    /// </summary>
    public static void SetColor(ref PlayerNative player, int color)
    {
        player.color = color;
        player.getentity.AddComponent(player);
        _players[player.index] = player;

        MarkDirty(player, PlayerDirtyFlags.Color);
    }

    /// <summary>
    /// 设置基础同盟（双向关系）：更新阵营缓存，并双向写联盟位 + 打 Alliance Dirty。
    /// </summary>
    public static void SetAlliance(PlayerNative playerA, PlayerNative playerB, bool allied)
    {
        var state = allied ? PlayerTeamState.Allie : PlayerTeamState.Enemy;
        Relations[playerA.index, playerB.index] = state;
        Relations[playerB.index, playerA.index] = state;

        // 基础同盟是双向关系：A→B 与 B→A 都要写位并打 Dirty。
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitBasic, allied);
        SetAllianceBit(playerB, playerA, PlayerAllianceState.AllianceBitBasic, allied);
    }

    /// <summary>
    /// 设置共享视野（单向：A 授予 B 视野），不改变阵营缓存。
    /// </summary>
    public static void SetVision(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        // 视野/控制类关系不改变阵营缓存，只同步联盟位（单向：A 授予 B 视野）。
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitVision, flag);
    }

    /// <summary>
    /// 设置共享控制权（单向：A 授予 B 控制），不改变阵营缓存。
    /// </summary>
    public static void SetControl(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitControl, flag);
    }

    /// <summary>
    /// 设置完全控制权（单向：A 授予 B 完全控制），不改变阵营缓存。
    /// </summary>
    public static void SetFullControl(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitFullControl, flag);
    }

    /// <summary>
    /// 设置中立关系（双向）：更新阵营缓存为 Neutral，并双向写联盟位 + 打 Alliance Dirty。
    /// </summary>
    public static void SetNeutral(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        Relations[playerA.index, playerB.index] = PlayerTeamState.Neutral;
        Relations[playerB.index, playerA.index] = PlayerTeamState.Neutral;

        // 中立也是双向关系。
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitNeutral, flag);
        SetAllianceBit(playerB, playerA, PlayerAllianceState.AllianceBitNeutral, flag);
    }

    /// <summary>
    /// 查询两玩家阵营关系（读静态缓存，非 ECS 组件）。
    /// </summary>
    public static PlayerTeamState GetRelation(PlayerNative playerA, PlayerNative playerB)
    {
        // 查询始终读缓存，避免把原生 alliance 状态当作长期语义真相。
        return Relations[playerA.index, playerB.index];
    }

    /// <summary>
    /// 判断两玩家是否为盟友。
    /// </summary>
    public static bool IsAlly(PlayerNative playerA, PlayerNative playerB)
    {
        return Relations[playerA.index, playerB.index] == PlayerTeamState.Allie;
    }

    /// <summary>
    /// 判断两玩家是否为敌对。
    /// </summary>
    public static bool IsEnemy(PlayerNative playerA, PlayerNative playerB)
    {
        return Relations[playerA.index, playerB.index] == PlayerTeamState.Enemy;
    }

    /// <summary>
    /// 修改源玩家对目标玩家的单个联盟位，并标记目标待同步。
    /// 组件缺失时自动初始化，避免未初始化崩溃。
    /// </summary>
    private static void SetAllianceBit(PlayerNative source, PlayerNative target, int bit, bool enabled)
    {
        // 联盟位真相在 PlayerAllianceState 组件：读-改-写并标记目标待同步。
        // 用 TryGetComponent 避免未初始化时 GetComponent 抛异常（对齐 SyncAlliance 的容错）。
        if (!source.getentity.TryGetComponent<PlayerAllianceState>(out var state))
        {
            state = PlayerAllianceState.Create(MaxPlayers);
            source.getentity.AddComponent(state);
        }

        ref var targetBits = ref state.bits[target.index];

        if (enabled)
        {
            targetBits |= (byte)bit;
        }
        else
        {
            targetBits &= (byte)~bit;
        }

        // 标记该目标为脏：Native 同步只处理被修改的目标，避免全量重放覆盖默认结盟。
        state.dirty[target.index] = 1;
        source.getentity.AddComponent(state);
        MarkDirty(source, PlayerDirtyFlags.Alliance);
    }

    /// <summary>
    /// 合并玩家 Dirty flags（按位 OR），等待 Native 层消费。
    /// </summary>
    private static void MarkDirty(PlayerNative player, PlayerDirtyFlags flag)
    {
        if (player.getentity.TryGetComponent<PlayerDirty>(out var dirty))
        {
            dirty.flags |= flag;
            player.getentity.AddComponent(dirty);
        }
        else
        {
            player.getentity.AddComponent(new PlayerDirty { flags = flag });
        }
    }
}