using Friflo.Engine.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame.Src.Components
{
    public enum DamageType
    {
        Physical,
        Magical,
        Real,
    }
    public enum DamageSrc
    {
        Melee, Ranged, Skill,
    }
    public struct DamageBase
    {
        public float damage;
        public DamageType damageType; // 伤害类型，物理、魔法、真实等
        public DamageSrc damageSrc;   // 伤害来源，近战、远程等
        public Entity source;
        public Entity target;
    }
    public struct DamageEvent : IComponent
    {
        public DamageBase damage;
        public Entity source;
        public Entity target;
    }
}
