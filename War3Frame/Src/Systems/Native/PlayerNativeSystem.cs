using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Systems.Native;

[SystemRegister(SystemKind.Immediate)]
public class PlayerNameNativeSystem : QuerySystem<PlayerNative, PlayerNameNativeRequest>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref PlayerNative player, ref PlayerNameNativeRequest request, Entity entity) =>
        {
            // Native 层消费一次性改名请求，业务层只需写入 PlayerNameNativeRequest。
            JassApi.SetPlayerName(player.player, request.name);
            entity.RemoveComponent<PlayerNameNativeRequest>();
        });
    }
}

[SystemRegister(SystemKind.Immediate)]
public class PlayerColorNativeSystem : QuerySystem<PlayerNative, PlayerColorNativeRequest>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref PlayerNative player, ref PlayerColorNativeRequest request, Entity entity) =>
        {
            // 颜色同步是原生副作用，执行后立即移除请求组件保持幂等。
            JassApi.SetPlayerColor(player.player, new JPlayerColor(request.color));
            entity.RemoveComponent<PlayerColorNativeRequest>();
        });
    }
}

[SystemRegister(SystemKind.Immediate)]
public class PlayerAllianceNativeSystem : QuerySystem<PlayerNative, PlayerAllianceNativeRequest>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref PlayerNative source, ref PlayerAllianceNativeRequest request, Entity entity) =>
        {
            if (!request.target.TryGetComponent<PlayerNative>(out var target))
            {
                // 目标玩家尚未绑定原生句柄时丢弃请求，避免 native 调用拿到无效 handle。
                entity.RemoveComponent<PlayerAllianceNativeRequest>();
                return;
            }

            switch (request.kind)
            {
                case PlayerAllianceNativeKind.BasicAlliance:
                    SetBasicAlliance(source.player, target.player, request.flag);
                    break;
                case PlayerAllianceNativeKind.Vision:
                    JassApi.SetPlayerAlliance(source.player, target.player,
                        new JAllianceType(Blizzard.ALLIANCE_SHARED_VISION), request.flag);
                    break;
                case PlayerAllianceNativeKind.Control:
                    JassApi.SetPlayerAlliance(source.player, target.player,
                        new JAllianceType(Blizzard.ALLIANCE_SHARED_CONTROL), request.flag);
                    break;
                case PlayerAllianceNativeKind.FullControl:
                    JassApi.SetPlayerAlliance(source.player, target.player,
                        new JAllianceType(Blizzard.ALLIANCE_SHARED_ADVANCED_CONTROL), request.flag);
                    break;
                case PlayerAllianceNativeKind.Neutral:
                    JassApi.SetPlayerAlliance(source.player, target.player,
                        new JAllianceType(Blizzard.ALLIANCE_PASSIVE), request.flag);
                    break;
            }

            entity.RemoveComponent<PlayerAllianceNativeRequest>();
        });
    }

    private static void SetBasicAlliance(JPlayer source, JPlayer target, bool allied)
    {
        // 基础同盟由多组联盟位共同构成，集中在这里保持语义一致。
        JassApi.SetPlayerAlliance(source, target, new JAllianceType(Blizzard.ALLIANCE_PASSIVE), allied);
        JassApi.SetPlayerAlliance(source, target, new JAllianceType(Blizzard.ALLIANCE_HELP_REQUEST), allied);
        JassApi.SetPlayerAlliance(source, target, new JAllianceType(Blizzard.ALLIANCE_HELP_RESPONSE), allied);
        JassApi.SetPlayerAlliance(source, target, new JAllianceType(Blizzard.ALLIANCE_SHARED_SPELLS), allied);
    }
}
