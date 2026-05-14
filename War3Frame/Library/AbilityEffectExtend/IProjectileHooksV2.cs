using Friflo.Engine.ECS;

namespace War3Frame;

public enum ProjectileTravelDecision
{
    Continue,
    SuppressArrivalThisTick,
    RequestExpire
}

public interface IProjectileHooksV2
{
    public void OnStart(
        Entity effectEntity,
        ref EffectSource source,
        ref EffectTargetInfo target,
        ref Position position,
        ref ProjectileRuntimeState runtimeState);

    public ProjectileTravelDecision OnTravel(
        Entity effectEntity,
        ref EffectSource source,
        ref EffectTargetInfo target,
        ref Position position,
        ref ProjectileRuntimeState runtimeState);

    public void OnArrive(
        Entity effectEntity,
        ref EffectSource source,
        ref EffectTargetInfo target,
        ref Position position,
        ref ProjectileRuntimeState runtimeState);
}
