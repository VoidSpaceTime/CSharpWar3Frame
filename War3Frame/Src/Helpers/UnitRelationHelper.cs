using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>只读 ECS 玩家归属与联盟；未知归属不推断为敌人或友军。</summary>
public static class UnitRelationHelper
{
    public static bool TryGetPlayer(Entity unit, out PlayerNative player)
    {
        player = default;
        if (unit.IsNull) return false;
        if (unit.TryGetComponent<UnitOwner>(out var owner) && !owner.player.IsNull
            && owner.player.TryGetComponent(out player))
            return true;
        // 已创建原生单位的缓存只用于定位玩家，不进行原生状态查询。
        if (unit.TryGetComponent<UnitNative>(out var native) && native.player != null)
            foreach (var known in PlayerHelper.Players)
                if (!known.getentity.IsNull && ReferenceEquals(unit.Store, known.getentity.Store)
                    && known.player != null && known.player.Handle == native.player.Handle)
                {
                    player = known;
                    return true;
                }
        return false;
    }

    public static bool TryGetRelation(Entity source, Entity target, out PlayerTeamState relation)
    {
        relation = default;
        if (source.IsNull || target.IsNull || !ReferenceEquals(source.Store, target.Store)) return false;
        if (source == target) { relation = PlayerTeamState.Allie; return true; }
        if (!TryGetPlayer(source, out var playerA) || !TryGetPlayer(target, out var playerB)) return false;
        relation = PlayerHelper.GetRelation(playerA, playerB);
        return true;
    }
}
