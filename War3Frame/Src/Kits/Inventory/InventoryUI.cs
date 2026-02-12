using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame;

/// <summary>
/// 背包面板 - 显示单位的物品格子
/// 属于 Inventory 套件，只依赖 Core/UI 基础层和本套件的组件
/// </summary>
public class InventoryPanel : UIPanel
{
    private const float SlotSize = 0.035f;
    private const float SlotGap = 0.003f;

    private UISlot[] _slots = null!;
    private Entity _boundUnit;
    private readonly int _slotCount;
    private readonly int _columns;

    public InventoryPanel(int slotCount = 6, int columns = 2)
        : base("inventory")
    {
        _slotCount = slotCount;
        _columns = columns;
    }

    /// <summary>绑定到指定单位</summary>
    public void BindUnit(Entity unit)
    {
        _boundUnit = unit;
        Refresh();
    }

    public Entity BoundUnit => _boundUnit;

    protected override void OnCreate(int parentFrame)
    {
        int rows = (int)Math.Ceiling((float)_slotCount / _columns);
        float totalW = _columns * SlotSize + (_columns - 1) * SlotGap + 0.01f;
        float totalH = rows * SlotSize + (rows - 1) * SlotGap + 0.01f;

        Root = new UIFrame(parentFrame, "UI\\Widgets\\EscMenu\\Human\\human-options-menu-background.blp");
        Root.SetSize(totalW, totalH);
        Root.SetPos(0.55f, 0.15f);

        _slots = new UISlot[_slotCount];
        for (int i = 0; i < _slotCount; i++)
        {
            int col = i % _columns;
            int row = i / _columns;

            _slots[i] = new UISlot(Root.Handle, i, SlotSize);

            float x = 0.005f + col * (SlotSize + SlotGap);
            float y = totalH - 0.005f - SlotSize - row * (SlotSize + SlotGap);

            FrameHelper.ClearPoints(_slots[i].Handle);
            FrameHelper.SetPoint(_slots[i].Handle, FramePoint.BottomLeft,
                Root.Handle, FramePoint.BottomLeft, x, y);

            int slotIdx = i;
            // 点击事件通过 AsyncHelper 发送同步请求
            _slots[i].OnClick(() =>
            {
                if (_boundUnit.IsNull || _slots[slotIdx].IsEmpty) return;
                AsyncHelper.SendAction("inv", "use", _boundUnit, slotIdx, 0);
            });

            _slots[i].OnHover(
                () => OnSlotHover(slotIdx, true),
                () => OnSlotHover(slotIdx, false)
            );
        }

        Hide();
    }

    public override void Refresh()
    {
        if (!IsVisible || _slots == null) return;
        if (_boundUnit.IsNull) return;

        // 先清空
        for (int i = 0; i < _slots.Length; i++)
            _slots[i].Clear();

        // TODO: 当 ItemOwner 迁移为 ILinkComponent 后，使用 GetIncomingLinks<ItemOwner>
        // 当前 ItemOwner 是 IComponent，无法直接反向查询
        // 需要在 Inventory 套件中将 ItemOwner 改为 ILinkComponent 或使用 EntityStore Query
    }

    private void OnSlotHover(int slotIndex, bool enter)
    {
        if (enter && !_slots[slotIndex].IsEmpty)
            UIManager.Tooltip?.ShowAt(0.45f, 0.3f, "物品名称", "物品描述");
        else
            UIManager.Tooltip?.Hide();
    }

    public override void Destroy()
    {
        if (_slots != null)
            foreach (var slot in _slots)
                slot.Destroy();
        base.Destroy();
    }
}
