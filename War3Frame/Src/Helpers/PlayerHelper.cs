using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 玩家领域 Helper。持久状态（名称/颜色/联盟）写入 ECS 组件并打 Dirty，
/// Native 层消费 PlayerDirty 同步到 War3；静态数组仅为查询缓存镜像。
/// </summary>
public static class PlayerHelper
{
    private const int MaxPlayers = 16;

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
    /// 初始化玩家镜像数组：为每个玩家实体挂载默认联盟状态组件。
    /// </summary>
    public static void InitializePlayers(ref PlayerNative[] players)
    {
        // 修正：原为 _players = PlayerHelper._players（自赋值），导致玩家镜像恒空、
        // 原生事件桥未注册、联盟矩阵保持默认全 Allie。此处必须引用调用方传入的数组。
        _players = players;

        // 玩家实体挂载联盟状态组件（ECS 真相）。
        foreach (var player in _players)
        {
            var state = PlayerAllianceState.Create(MaxPlayers);
            state.bits[player.index] = PlayerAllianceState.AllianceBitBasic;
            player.getentity.AddComponent(state);
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
    /// 设置基础同盟（双向关系）：双向写联盟位并打 Alliance Dirty，查询由联盟位派生。
    /// </summary>
    public static void SetAlliance(PlayerNative playerA, PlayerNative playerB, bool allied)
    {
        // 基础同盟是双向关系：A→B 与 B→A 都要写位并打 Dirty。
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitNeutral, false);
        SetAllianceBit(playerB, playerA, PlayerAllianceState.AllianceBitNeutral, false);
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitBasic, allied);
        SetAllianceBit(playerB, playerA, PlayerAllianceState.AllianceBitBasic, allied);
    }

    /// <summary>
    /// 设置共享视野（单向：A 授予 B 视野），不改变阵营关系。
    /// </summary>
    public static void SetVision(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        // 视野/控制类关系不改变阵营关系，只同步联盟位（单向：A 授予 B 视野）。
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitVision, flag);
    }

    /// <summary>
    /// 设置共享控制权（单向：A 授予 B 控制），不改变阵营关系。
    /// </summary>
    public static void SetControl(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitControl, flag);
    }

    /// <summary>
    /// 设置完全控制权（单向：A 授予 B 完全控制），不改变阵营关系。
    /// </summary>
    public static void SetFullControl(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitFullControl, flag);
    }

    /// <summary>
    /// 设置中立关系（双向）：双向写中立位并打 Alliance Dirty。
    /// </summary>
    public static void SetNeutral(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        SetAllianceBit(playerA, playerB, PlayerAllianceState.AllianceBitNeutral, flag);
        SetAllianceBit(playerB, playerA, PlayerAllianceState.AllianceBitNeutral, flag);
    }

    /// <summary>
    /// 查询两玩家阵营关系，直接从 ECS 联盟位派生。
    /// </summary>
    public static PlayerTeamState GetRelation(PlayerNative playerA, PlayerNative playerB)
    {
        // 不维护第二份可漂移的关系矩阵，也不读取原生 alliance。
        if (playerA.index == playerB.index) return PlayerTeamState.Allie;
        if (!playerA.getentity.IsNull && playerA.getentity.TryGetComponent<PlayerAllianceState>(out var alliance)
            && playerB.index >= 0 && playerB.index < alliance.bits.Length)
            return GetRelationFromBits(alliance.bits[playerB.index]);
        return PlayerTeamState.Enemy;
    }

    /// <summary>
    /// 判断两玩家是否为盟友。
    /// </summary>
    public static bool IsAlly(PlayerNative playerA, PlayerNative playerB)
    {
        return GetRelation(playerA, playerB) == PlayerTeamState.Allie;
    }

    /// <summary>
    /// 判断两玩家是否为敌对。
    /// </summary>
    public static bool IsEnemy(PlayerNative playerA, PlayerNative playerB)
    {
        return GetRelation(playerA, playerB) == PlayerTeamState.Enemy;
    }

    /// <summary>唯一联盟位解释规则，供领域查询与 Native 投影共同使用。</summary>
    public static PlayerTeamState GetRelationFromBits(byte bits)
        => (bits & PlayerAllianceState.AllianceBitNeutral) != 0 ? PlayerTeamState.Neutral
            : (bits & PlayerAllianceState.AllianceBitBasic) != 0 ? PlayerTeamState.Allie
            : PlayerTeamState.Enemy;

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
