using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Helpers;

namespace War3Frame.EntityRef;

/// <summary>
/// 技能实体薄包装
/// 只保存 Entity 句柄，所有读写仍然回到 ECS 组件
/// </summary>
public readonly struct AbilityEntityRef
{
    public Entity Entity { get; }
    public AbilityEntityRef(Entity e) => Entity = e;

    public int Id => Entity.Id;
    public bool IsNull => Entity.IsNull;

    public static AbilityEntityRef From(Entity entity) => new(entity);

    public static bool TryFrom(Entity entity, out AbilityEntityRef ability)
    {
        if (!entity.IsNull && entity.TryGetComponent<AbilityBase>(out _))
        {
            ability = new AbilityEntityRef(entity);
            return true;
        }

        ability = default;
        return false;
    }

    public bool IsValid()
    {
        return !Entity.IsNull && Entity.TryGetComponent<AbilityBase>(out _);
    }

    public AbilityBase GetBase()
    {
        return Entity.GetComponent<AbilityBase>();
    }

    public AbilityRuntime GetRuntime()
    {
        return Entity.TryGetComponent<AbilityRuntime>(out var runtime)
            ? runtime
            : default;
    }

    public AbilityStatValue GetStatValue(int typeId)
    {
        return AbilityHelper.GetValue(Entity, typeId);
    }

    public string GetTemplateName() => GetBase().templateName;
    public string GetName() => GetBase().Name;
    public string GetDescription() => GetBase().Description;
    public AbilityState GetState() => GetBase().state;
    public AbilityTargetType GetTargetType() => GetBase().targetType;
    public int GetSlotIndex() => Entity.TryGetComponent<AbilitySlotIndex>(out var slot) ? slot.slotIndex : -1;
    public Entity GetOwner() => Entity.TryGetComponent<AbilityOwner>(out var owner) ? owner.owner : default;
    public float GetStatBaseValue(int typeId) => AbilityHelper.GetBaseValue(Entity, typeId);
    public float GetStatCurrentValue(int typeId) => AbilityHelper.GetCurrentValue(Entity, typeId);
    public float GetStatFinalValue(int typeId) => AbilityHelper.GetFinalValue(Entity, typeId);
    public float GetCooldownRemaining() => GetRuntime().cooldownRemaining;
    public float GetCastRemaining() => GetRuntime().castRemaining;
    public float GetChannelRemaining() => GetRuntime().channelRemaining;

    public AbilityEntityRef SetTemplateName(string templateName)
    {
        var ab = GetBase();
        ab.templateName = templateName;
        Entity.AddComponent(ab);
        return this;
    }

    public AbilityEntityRef SetName(string name)
    {
        var ab = GetBase();
        ab.Name = name;
        Entity.AddComponent(ab);
        return this;
    }

    public AbilityEntityRef SetDescription(string description)
    {
        var ab = GetBase();
        ab.Description = description;
        Entity.AddComponent(ab);
        return this;
    }

    public AbilityEntityRef SetState(AbilityState state)
    {
        var ab = GetBase();
        ab.state = state;
        Entity.AddComponent(ab);
        return this;
    }

    public AbilityEntityRef SetTargetType(AbilityTargetType targetType)
    {
        var ab = GetBase();
        ab.targetType = targetType;
        Entity.AddComponent(ab);
        return this;
    }

    public AbilityEntityRef EnsureStat(int typeId, float baseValue, float? currentValue = null)
    {
        AbilityHelper.EnsureStat(Entity, typeId, baseValue, currentValue);
        return this;
    }

    public AbilityEntityRef SetStatValue(int typeId, float baseValue, float currentValue, float finalValue)
    {
        AbilityHelper.SetValue(Entity, typeId, baseValue, currentValue, finalValue);
        return this;
    }

    public AbilityEntityRef SetStatBaseValue(int typeId, float value)
    {
        AbilityHelper.SetBaseValue(Entity, typeId, value);
        return this;
    }

    public AbilityEntityRef SetStatCurrentValue(int typeId, float value)
    {
        AbilityHelper.SetCurrentValue(Entity, typeId, value);
        return this;
    }

    public AbilityEntityRef SetStatFinalValue(int typeId, float value)
    {
        AbilityHelper.SetFinalValue(Entity, typeId, value);
        return this;
    }

    public AbilityEntityRef AddStatModifier(int typeId, Entity source, ModifyType modifyType, float value, int priority = 0)
    {
        AbilityHelper.AddModifier(Entity, typeId, source, modifyType, value, priority);
        return this;
    }

    public AbilityEntityRef SetSlot(int slotIndex, string? hotkey = null)
    {
        var slot = Entity.TryGetComponent<AbilitySlotIndex>(out var current)
            ? current
            : default;

        slot.slotIndex = slotIndex;
        slot.hotkey = hotkey;
        Entity.AddComponent(slot);
        return this;
    }

    public AbilityEntityRef SetCooldownRemaining(float value)
    {
        var runtime = GetRuntime();
        runtime.cooldownRemaining = value;
        Entity.AddComponent(runtime);
        return this;
    }

    public AbilityEntityRef SetCastRemaining(float value)
    {
        var runtime = GetRuntime();
        runtime.castRemaining = value;
        Entity.AddComponent(runtime);
        return this;
    }

    public AbilityEntityRef SetChannelRemaining(float value)
    {
        var runtime = GetRuntime();
        runtime.channelRemaining = value;
        Entity.AddComponent(runtime);
        return this;
    }

    public AbilityEntityRef ClearRuntime()
    {
        Entity.AddComponent(new AbilityRuntime());
        return this;
    }

    public AbilityEntityRef AttachTo(Entity unit, int slotIndex)
    {
        AbilitySlotHelper.AttachAbilityToSlot(unit, Entity, slotIndex);
        return this;
    }

    public AbilityEntityRef Remove()
    {
        if (Entity.TryGetComponent<AbilityOwner>(out var owner)
            && Entity.TryGetComponent<AbilitySlotIndex>(out var slot)
            && !owner.owner.IsNull)
        {
            AbilitySlotHelper.RemoveAbilityFromSlot(owner.owner, slot.slotIndex);
        }
        else
        {
            AbilityHelper.RemoveAbility(Entity);
        }

        return this;
    }
}
