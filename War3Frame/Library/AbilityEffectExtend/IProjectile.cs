using Friflo.Engine.ECS;
using War3Frame.Components.AbilityEffectExtend;

namespace War3Frame;

public interface IProjectileOnTravel
{
    public bool ProjectileOnTravel(ref ProjectileBase projectile, ref Position position, Entity entity);
}

public interface IProjectileOnArrive
{
    public void ProjectileOnArrive(ref ProjectileBase projectile, ref Position position, Entity entity);
}

public interface IProjectileOnStart
{
    public void ProjectileOnStart(ref ProjectileBase projectile, ref Position position, Entity entity);
}