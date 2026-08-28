using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Systems.Native;

/// <summary>
/// 玩家原生同步系统。消费 PlayerDirty 脏标记，把 ECS 中的玩家持久状态
/// （名称/颜色/联盟位）同步到 War3 原生玩家，同步完成后清除标记。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class PlayerNativeSyncSystem : QuerySystem<PlayerNative, PlayerDirty>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref PlayerNative player, ref PlayerDirty dirty, Entity entity) =>
        {
            if (dirty.flags.HasFlag(PlayerDirtyFlags.Name))
            {
                JassApi.SetPlayerName(player.player, player.name);
            }

            if (dirty.flags.HasFlag(PlayerDirtyFlags.Color))
            {
                JassApi.SetPlayerColor(player.player, new JPlayerColor(player.color));
            }

            if (dirty.flags.HasFlag(PlayerDirtyFlags.Alliance))
            {
                SyncAlliance(player);
            }

            entity.RemoveComponent<PlayerDirty>();
        });
    }

    /// <summary>
    /// 按 ECS 联盟状态增量同步该玩家的原生联盟位：只处理被标记 dirty 的目标玩家，
    /// 同步后清除 dirty 标记。不做全量重放，避免覆盖地图初始默认结盟。
    /// </summary>
    private static void SyncAlliance(PlayerNative source)
    {
        if (!source.getentity.TryGetComponent<PlayerAllianceState>(out var alliance))
        {
            return;
        }

        for (var targetIndex = 0; targetIndex < alliance.bits.Length; targetIndex++)
        {
            if (alliance.dirty[targetIndex] == 0)
            {
                continue;
            }

            var targetPlayer = PlayerHelper.GetPlayer(targetIndex);
            var bits = alliance.bits[targetIndex];

            ApplyAllianceBits(source.player, targetPlayer.player, bits);

            // 同步完成后清除该目标的 dirty 标记。
            alliance.dirty[targetIndex] = 0;
        }

        source.getentity.AddComponent(alliance);
    }

    /// <summary>
    /// 把单个目标的联盟位展开为原生联盟类型。
    /// Basic 与 Neutral 互斥：Neutral 存在时 PASSIVE 由 Neutral 决定；
    /// 否则 PASSIVE 由 Basic 决定，避免两路写入互相覆盖。
    /// </summary>
    private static void ApplyAllianceBits(JPlayer source, JPlayer target, byte bits)
    {
        var isNeutral = (bits & PlayerAllianceState.AllianceBitNeutral) != 0;
        var isBasic = !isNeutral && (bits & PlayerAllianceState.AllianceBitBasic) != 0;
        var isVision = (bits & PlayerAllianceState.AllianceBitVision) != 0;
        var isControl = (bits & PlayerAllianceState.AllianceBitControl) != 0;
        var isFullControl = (bits & PlayerAllianceState.AllianceBitFullControl) != 0;

        // PASSIVE：Basic 同盟与 Neutral 共用，但按互斥优先级取值，同一原生位只写一次语义。
        JassApi.SetPlayerAlliance(source, target,
            new JAllianceType(Blizzard.ALLIANCE_PASSIVE), isNeutral || isBasic);
        JassApi.SetPlayerAlliance(source, target,
            new JAllianceType(Blizzard.ALLIANCE_HELP_REQUEST), isBasic);
        JassApi.SetPlayerAlliance(source, target,
            new JAllianceType(Blizzard.ALLIANCE_HELP_RESPONSE), isBasic);
        JassApi.SetPlayerAlliance(source, target,
            new JAllianceType(Blizzard.ALLIANCE_SHARED_SPELLS), isBasic);
        JassApi.SetPlayerAlliance(source, target,
            new JAllianceType(Blizzard.ALLIANCE_SHARED_VISION), isVision);
        JassApi.SetPlayerAlliance(source, target,
            new JAllianceType(Blizzard.ALLIANCE_SHARED_CONTROL), isControl);
        JassApi.SetPlayerAlliance(source, target,
            new JAllianceType(Blizzard.ALLIANCE_SHARED_ADVANCED_CONTROL), isFullControl);
    }
}