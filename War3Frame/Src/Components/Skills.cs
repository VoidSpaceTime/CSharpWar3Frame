using Friflo.Engine.ECS;

namespace War3Frame
{
    public enum SkillState
    {
        Ready,      // 就绪
        Casting,    // 吟唱中
        Channeling, // 持续施法中
        Cooldown,    // 冷却中
        Ban,
    }
    public enum SkillType
    {
        Active,     // 主动
        Passive     // 被动
    }
    public enum SkillTargetType
    {
        None,       // 无目标
        Unit,       // 单位
        Point,      // 点
        Area        // 区域
    }

    /// 技能基础（所有技能都有）
    public struct SkillBase : IComponent
    {
        public int level;
        public SkillState state;
        public float cooldown;      // CD 总时间
        public float currentCd;     // 当前 CD 剩余
        public float manaCost;
        public string hotkey;
    }
    public struct SkillOwner : IComponent
    {
        public Entity GetRelationKey() => owner;
        public Entity owner;
    }
    public struct SkillItem : IComponent
    {
        public Entity GetRelationKey() => owner;
        public Entity owner;
    }
    public struct SkillBan : IComponent
    {
        public string banReason;
        public float banDurtion;
        public float banCurrent;
        public bool isBan;

    }
}
