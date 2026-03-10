using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame;

/// <summary>
/// 技能栏面板 — 显示技能图标和冷却
/// </summary>
public class AbilityPanel : UIPanel
{
    private const float SlotSize = 0.04f;
    private const float SlotGap = 0.003f;

    private UISlot[] _slots = null!;
    private Entity _boundUnit;
    private readonly int _maxSlots;

    public AbilityPanel(int maxSlots = 8) : base("ability")
    {
        _maxSlots = maxSlots;
    }

    public void BindUnit(Entity unit) { _boundUnit = unit; Refresh(); }

    protected override void OnCreate(int parentFrame)
    {
        float totalW = _maxSlots * SlotSize + (_maxSlots - 1) * SlotGap + 0.01f;

        Root = new UIFrame(parentFrame, "UI\\Widgets\\EscMenu\\Human\\human-options-menu-background.blp");
        Root.SetSize(totalW, SlotSize + 0.01f);
        Root.SetPos(0.2f, 0.01f);

        _slots = new UISlot[_maxSlots];
        for (int i = 0; i < _maxSlots; i++)
        {
            _slots[i] = new UISlot(Root.Handle, i, SlotSize);
            FrameHelper.ClearPoints(_slots[i].Handle);
            FrameHelper.SetPoint(_slots[i].Handle, FramePoint.BottomLeft,
                Root.Handle, FramePoint.BottomLeft,
                0.005f + i * (SlotSize + SlotGap), 0.005f);

            int idx = i;
            _slots[i].OnClick(() =>
            {
                if (_boundUnit.IsNull) return;
                AsyncHelper.SendAction("abl", "cast", _boundUnit, idx, 0);
            });
        }
        Hide();
    }

    public override void Refresh()
    {
        if (!IsVisible || _slots == null || _boundUnit.IsNull) return;

        for (int i = 0; i < _slots.Length; i++) _slots[i].Clear();

        var abilities = _boundUnit.GetIncomingLinks<AbilityOwner>();
        foreach (var link in abilities)
        {
            var entity = link.Entity;
            if (!entity.TryGetComponent<AbilitySlotIndex>(out var slot)) continue;
            if (slot.slotIndex < 0 || slot.slotIndex >= _slots.Length) continue;
            if (!entity.TryGetComponent<AbilityBase>(out var ab)) continue;

            _slots[slot.slotIndex].SetContent("ReplaceableTextures\\CommandButtons\\BTNAbility.blp");
            if (ab.currentCd > 0 && ab.cooldown > 0)
                _slots[slot.slotIndex].SetCooldown(ab.currentCd / ab.cooldown);
        }
    }

    public override void Destroy()
    {
        if (_slots != null) foreach (var s in _slots) s.Destroy();
        base.Destroy();
    }
}

/// <summary>
/// 技能栏 UI 系统
/// </summary>
public class AbilityUISystem : BaseSystem
{
    private AbilityPanel? _panel;
    private float _timer;

    protected override void OnUpdateGroupBegin()
    {
        if (_panel == null)
        {
            _panel = new AbilityPanel();
            UIManager.Register(_panel);
            SyncHelper.Register("abl", OnSync);
        }
    }

    protected override void OnUpdateGroupEnd()
    {
        _timer += Tick.deltaTime;
        if (_timer >= 0.1f) { _timer = 0; _panel?.Refresh(); }
    }

    private void OnSync(JPlayer player, string data)
    {
        var args = SyncHelper.Decode(data);
        if (args.Length < 3 || args[0] != "cast") return;
        var unit = SyncHelper.DecodeEntity(args[1]);
        if (unit.IsNull) return;

        int slotIdx = int.Parse(args[2]);

        // 查找对应槽位的技能
        var ability = FindAbilityInSlot(unit, slotIdx);
        if (ability.IsNull) return;

        // 创建施法请求
        // 注意：目前只支持无目标施法（如立即施法技能）
        // 如果需要支持目标施法，UI 需要先进入选择目标模式，然后 SyncHelper 发送包含目标 Entity 或 坐标的数据
        unit.AddComponent(new CastRequest
        {
            ability = ability,
            targetUnit = default, // 无目标
            targetX = 0,
            targetY = 0
        });

        Console.WriteLine($"[Sync] Unit {unit.Id} requested cast ability {ability.Id} in slot {slotIdx}");
    }

    private Entity FindAbilityInSlot(Entity unit, int slotIndex)
    {
        foreach (var link in unit.GetIncomingLinks<AbilityOwner>())
        {
            var ability = link.Entity;
            if (ability.TryGetComponent<AbilitySlotIndex>(out var slot) && slot.slotIndex == slotIndex)
            {
                return ability;
            }
        }
        return default;
    }
}
