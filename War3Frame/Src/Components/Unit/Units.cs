using Friflo.Engine.ECS;

namespace War3Frame;

public struct UnitOwner : ILinkComponent
{
    public Entity GetIndexedValue()
    {
        return player;
    }

    public UnitOwner(Entity player)
    {
        this.player = player;
    }

    public Entity player;
}

/// <summary>
/// 单位攻击形态。区分攻击方式的离散分类，与数值属性（AttackRange 等）分离：
/// 攻击类型是"形态切换"语义（近战单位装备弓可临时变远程），需运行时改写，故为状态组件而非 UnitBase 静态字段。
/// </summary>
public enum AttackType
{
    /// <summary>近战：攻击命中直接结算伤害。</summary>
    Melee,

    /// <summary>远程：攻击命中生成投射物，命中后再结算伤害（投射物模拟为后续增量）。</summary>
    Ranged,

    /// <summary>闪电链攻击（实现细节待后续增量）。</summary>
    Chain,
}

/// <summary>
/// 单位攻击形态状态。缺省 Melee（无此组件 = 近战）。
/// 运行时通过 AttackHelper.SetAttackType 改写（装备/技能/buff 临时变更），
/// 不进入 Attr 数值模型——形态切换是覆盖语义，非数值加减。
/// </summary>
public struct AttackTypeState : IComponent
{
    public AttackType value;
}

public struct UnitBase : IComponent
{
    public string templateName;
    public string name;
}

/// <summary>
/// War3 原生单位句柄缓存。
/// 仅供 Native/Execution 层执行副作用使用，长期语义仍以 ECS 组件为准。
/// </summary>
public struct UnitNative : IComponent
{
    public JUnit unit;
    public JPlayer player;
}

/// <summary>
/// 原生同步快照，仅用于连续字段 compare-sync。
/// </summary>
public struct UnitNativeSyncSnapshot : IComponent
{
    public UnitNativeSyncEntry entry0;
    public UnitNativeSyncEntry entry1;
}

/// <summary>
/// 单个原生同步条目快照。
/// </summary>
public struct UnitNativeSyncEntry
{
    public int attrTypeId;
    public bool initialized;
    public float lastCurrent;
    public float lastFinal;
}

/// <summary>
/// 原生单位创建请求
/// </summary>
public struct UnitCreateNativeRequest : IComponent
{
    public JPlayer player;
    public float x;
    public float y;
    public float facing;
    public int unitTypeId;
}
