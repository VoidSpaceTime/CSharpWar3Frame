using Friflo.Engine.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame
{
    public struct HealthDirty : ITag;
    public struct ManaDirty : ITag;

    public struct Health : IComponent
    {
        public float current;     // 当前生命值
        public float max;         // 最大生命值
        public float regen;       // 生命回复
    }
    public struct Mana : IComponent
    {
        public float current;     // 当前魔法值
        public float max;         // 最大魔法值
        public float regen;       // 魔法回复
    }

    /// 攻击属性（攻击相关的一起）
    public struct Attack : IComponent
    {
        public float damage;      // 攻击力
        public float speed;       // 攻速
        public float range;       // 攻击范围
        public float attackRangeMin;  // 最小攻击范围
        public float attackRangeAcquire;  // 主动攻击范围
        public float attackSpaceBase;  // 基础攻击间隔
        public float attackSpeed;  // 攻击速度
        public float currentAttackSpace;  // 当前攻击间隔
        public bool isRanged; //是否远程攻击
    }
    /// 防御属性
    public struct Defend : IComponent
    {
        public float armor;       // 护甲
    }
    public struct MagicResist : IComponent
    {
        public float magicResist; // 魔法抗性
    }
    /// 暴击属性（可选）
    public struct Crit : IComponent
    {
        public float chance;      // 暴击率
        public float multiplier;  // 暴击倍率
    }

    /// 视野
    public struct Sight : IComponent
    {
        public float sightRange;  // 视野范围
        public float nsightRange;  // 夜晚视野范围
    }
    public struct Move : IComponent
    {
        public float speed;       // 移动速度
    }
    public struct Visible : IComponent
    {
        public float visibleRange;
    }

}
