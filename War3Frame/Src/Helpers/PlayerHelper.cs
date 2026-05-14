namespace War3Frame;

public static class PlayerHelper
{
    private const int MaxPlayers = 16;

    private static readonly PlayerTeamState[,] Relations = new PlayerTeamState[MaxPlayers, MaxPlayers];
    private static PlayerNative[] _players = Array.Empty<PlayerNative>();

    public static ReadOnlySpan<PlayerNative> Players => _players;

    public static ref PlayerNative GetPlayer(int index)
    {
        return ref _players[index];
    }

    public static void InitializePlayers(ref PlayerNative[] players)
    {
        _players = players;

        foreach (var sourcePlayer in _players)
        foreach (var targetPlayer in _players)
        {
            Relations[sourcePlayer.index, targetPlayer.index] = sourcePlayer.index == targetPlayer.index
                ? PlayerTeamState.Allie
                : PlayerTeamState.Enemy;
        }
    }

    public static void SetName(ref PlayerNative player, string name)
    {
        player.name = name;
        player.getentity.AddComponent(new PlayerNameNativeRequest { name = name });
    }

    public static void SetColor(ref PlayerNative player, int color)
    {
        player.color = color;
        player.getentity.AddComponent(new PlayerColorNativeRequest { color = color });
    }

    public static void SetAlliance(PlayerNative playerA, PlayerNative playerB, bool allied)
    {
        var state = allied ? PlayerTeamState.Allie : PlayerTeamState.Enemy;
        Relations[playerA.index, playerB.index] = state;
        Relations[playerB.index, playerA.index] = state;

        playerA.getentity.AddComponent(new PlayerAllianceNativeRequest
        {
            target = playerB.getentity,
            kind = PlayerAllianceNativeKind.BasicAlliance,
            flag = allied
        });
    }

    public static void SetVision(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        playerA.getentity.AddComponent(new PlayerAllianceNativeRequest
        {
            target = playerB.getentity,
            kind = PlayerAllianceNativeKind.Vision,
            flag = flag
        });
    }

    public static void SetControl(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        playerA.getentity.AddComponent(new PlayerAllianceNativeRequest
        {
            target = playerB.getentity,
            kind = PlayerAllianceNativeKind.Control,
            flag = flag
        });
    }

    public static void SetFullControl(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        playerA.getentity.AddComponent(new PlayerAllianceNativeRequest
        {
            target = playerB.getentity,
            kind = PlayerAllianceNativeKind.FullControl,
            flag = flag
        });
    }

    public static void SetNeutral(PlayerNative playerA, PlayerNative playerB, bool flag)
    {
        Relations[playerA.index, playerB.index] = PlayerTeamState.Neutral;
        playerA.getentity.AddComponent(new PlayerAllianceNativeRequest
        {
            target = playerB.getentity,
            kind = PlayerAllianceNativeKind.Neutral,
            flag = flag
        });
    }

    public static PlayerTeamState GetRelation(PlayerNative playerA, PlayerNative playerB)
    {
        return Relations[playerA.index, playerB.index];
    }

    public static bool IsAlly(PlayerNative playerA, PlayerNative playerB)
    {
        return Relations[playerA.index, playerB.index] == PlayerTeamState.Allie;
    }

    public static bool IsEnemy(PlayerNative playerA, PlayerNative playerB)
    {
        return Relations[playerA.index, playerB.index] == PlayerTeamState.Enemy;
    }
}
