using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 以本地 ECS Store 同步验证物品 companion ability 的创建、施法来源与受控删除。
/// </summary>
public static class ItemCompanionAbilityValidationScenario
{
    private const string ScenarioName = "ItemCompanionAbilityValidationScenario";
    private const float CastPointSeconds = 0.25f;

    /// <summary>
    /// 运行不依赖玩家句柄或 War3 Native 的同步 ECS 验证。
    /// </summary>
    public static void Initialize(JPlayer player)
    {
        _ = player;
        RunValidation();
    }

    /// <summary>
    /// 同步验证已在 Initialize 中完成，因此运行时更新无需执行操作。
    /// </summary>
    public static void Update()
    {
        // 保留入口以匹配测试客户端时钟，场景本身不依赖运行时 tick。
    }

    /// <summary>
    /// 执行 companion 创建、Unit 目标派发、效果来源和删除生命周期验证。
    /// </summary>
    private static void RunValidation()
    {
        var store = new EntityStore();
        var root = CreateSystemRoot(store);

        var user = store.CreateEntity(
            new Position { x = 10.5f, y = -20.25f, z = 0f },
            new ItemSlotContainer { maxSlots = 1, currentCount = 0 });
        var target = store.CreateEntity(
            new Position { x = 125.5f, y = -87.25f, z = 3f });
        var item = store.CreateEntity();

        ItemSpecBuilder.Create("validation_item_companion_unit")
            .Name("Companion Unit Validation Item")
            .Stack(count: 1, max: 1)
            .Usable(consumable: false)
            .UseAbility(ability => ability
                .Name("Companion Unit Validation Ability")
                .TargetType(AbilityTargetType.Unit)
                .CastPoint(CastPointSeconds)
                .Channel(0f, 0f)
                .Backswing(0f)
                .BaseValue(AbilityHelper.Range, 99_999f)
                .BaseValue(AbilityHelper.ManaCost, 0f)
                .BaseValue(AbilityHelper.CooldownDuration, 0f)
                .OnEffect(effect => effect.Heal(AbilityValue.Constant(1f))))
            .BuildTo(item);

        ItemHelper.EquipToUnit(item, user, 0);
        root.Update(new UpdateTick(0f, 0f));

        Require(item.TryGetComponent<ItemActiveAbility>(out var activeAbility),
            "companion/create: 装备后缺少 ItemActiveAbility");
        var companion = activeAbility.ability;
        Require(!companion.IsNull,
            "companion/create: ItemActiveAbility 未指向有效实体");
        var companionSourceItem = default(Entity);
        var companionSourceCount = 0;
        foreach (var link in companion.GetIncomingLinks<ItemActiveAbility>())
        {
            companionSourceItem = link.Entity;
            companionSourceCount++;
        }
        Require(companionSourceCount == 1 && companionSourceItem == item,
            "companion/source: ItemActiveAbility 反查未唯一指回物品");
        Require(companion.TryGetComponent<AbilityMountInfo>(out var mountInfo)
                && mountInfo.mountType == AbilityMountType.ItemGranted,
            "companion/mount: AbilityMountInfo 不是 ItemGranted");
        Require(!companion.HasComponent<AbilitySlotIndex>(),
            "companion/slot: companion 不应具有 AbilitySlotIndex");
        Require(companion.TryGetComponent<AbilityOwner>(out var abilityOwner)
                && abilityOwner.owner == user,
            "companion/owner: AbilityOwner 未绑定物品使用者");
        Require(ItemCompanionAbilityHelper.TryEnsureCompanion(item, out var ensuredCompanion)
                && ensuredCompanion == companion,
            "companion/uniqueness: 重复确保 companion 未返回同一实体");

        var companionCount = 0;
        store.Query<AbilityMountInfo>().ForEachEntity((ref AbilityMountInfo mount, Entity candidate) =>
        {
            if (mount.mountType == AbilityMountType.ItemGranted)
                companionCount++;
        });
        Require(companionCount == 1,
            $"companion/uniqueness: 预期 1 个 ItemGranted companion，实际 {companionCount} 个");

        ItemHelper.RequestUse(user, item, new ItemUseTarget
        {
            kind = AbilityTargetType.Unit,
            targetUnit = target,
            targetX = -1_234f,
            targetY = 5_678f
        });
        root.Update(new UpdateTick(0f, 0f));

        Require(user.TryGetComponent<CastState>(out var castState),
            "cast/dispatch: 零增量派发后缺少 CastState");
        Require(castState.phase == CastPhase.Casting && castState.timer > 0f,
            "cast/state: CastState 未稳定停留在正计时前摇阶段");
        Require(castState.ability == companion,
            "cast/ability: CastState 未引用 companion");
        Require(castState.targetUnit == target,
            "cast/target: CastState 未保留 Unit 目标");
        var targetPosition = target.GetComponent<Position>();
        Require(castState.targetX == targetPosition.x && castState.targetY == targetPosition.y,
            "cast/coordinates: Unit 目标坐标未规范化为目标 Position");
        Require(castState.itemOrigin.item == item && castState.itemOrigin.user == user,
            "cast/origin: ItemCastOrigin 未保留 item 与 user");

        root.Update(new UpdateTick(CastPointSeconds, CastPointSeconds));
        var rootEffect = FindRootEffect(store, item);
        Require(rootEffect.TryGetComponent<ItemEffectOrigin>(out var effectOrigin)
                && effectOrigin.item == item
                && effectOrigin.user == user,
            "effect/origin: 根 Effect 未保留 item 与 user");
        Require(rootEffect.TryGetComponent<EffectSource>(out var effectSource)
                && effectSource.caster == user
                && effectSource.ability == companion,
            "effect/source: 根 Effect 未保留 user 与 companion ability");
        Require(rootEffect.TryGetComponent<EffectTargetInfo>(out var effectTarget)
                && effectTarget.targetUnit == target,
            "effect/target: 根 Effect 未保留 Unit 目标");
        rootEffect.DeleteEntity();

        ItemHelper.RequestDestroy(item);
        root.Update(new UpdateTick(0f, CastPointSeconds));

        Require(item.IsNull,
            "destroy/item: 受控删除后物品实体仍然存在");
        Require(companion.IsNull,
            "destroy/companion: 受控删除后 companion 实体仍然存在");

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    /// <summary>
    /// 按验证数据流顺序创建全部本地系统，并强制每次更新立即执行。
    /// </summary>
    private static TimedSystemRoot CreateSystemRoot(EntityStore store)
    {
        var root = new TimedSystemRoot(store);
        root.Add(new ItemAttachWorkflowSystem(), 0f);
        root.Add(new ItemUseSystem(), 0f);
        root.Add(new CastRequestSystem(), 0f);
        root.Add(new CastingSystem(), 0f);
        root.Add(new ItemDestroyRequestSystem(), 0f);
        root.Add(new ItemCompanionDeferredDeleteSystem(), 0f);
        return root;
    }

    /// <summary>
    /// 查找且确认当前物品唯一的根效果实体。
    /// </summary>
    private static Entity FindRootEffect(EntityStore store, Entity item)
    {
        var rootEffect = default(Entity);
        var rootEffectCount = 0;
        store.Query<ItemEffectOrigin>().ForEachEntity((ref ItemEffectOrigin origin, Entity candidate) =>
        {
            if (origin.item != item
                || !candidate.TryGetComponent<AbilityEffectContext>(out var context)
                || !context.sourceEffect.IsNull)
            {
                return;
            }

            rootEffect = candidate;
            rootEffectCount++;
        });

        Require(rootEffectCount == 1,
            $"effect/root: 预期 1 个带 ItemEffectOrigin 的根 Effect，实际 {rootEffectCount} 个");
        return rootEffect;
    }

    /// <summary>
    /// 在不变量失败时抛出包含场景与检查上下文的异常。
    /// </summary>
    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}
