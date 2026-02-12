using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;

namespace War3Frame;

/// <summary>
/// 背包 UI 系统 - Inventory 套件的 UISystem
/// 
/// 职责：
/// 1. 创建并管理 InventoryPanel
/// 2. 注册同步处理器（接收 "inv" 前缀的同步数据）
/// 3. 定时刷新面板数据
/// </summary>
public class InventoryUISystem : BaseSystem
{
    private InventoryPanel? _panel;
    private float _refreshTimer;
    private const float RefreshInterval = 0.1f;

    protected override void OnUpdateGroupBegin()
    {
        // 初始化（首次执行）
        if (_panel == null)
        {
            _panel = new InventoryPanel(slotCount: 6, columns: 2);
            UIManager.Register(_panel);

            // 注册同步处理器
            SyncHelper.Register("inv", OnSyncReceived);
        }
    }

    protected override void OnUpdateGroupEnd()
    {
        // 定时刷新 UI（不需要每帧刷新）
        _refreshTimer += Tick.deltaTime;
        if (_refreshTimer >= RefreshInterval)
        {
            _refreshTimer = 0;
            _panel?.Refresh();
        }
    }

    /// <summary>
    /// 接收同步数据回调 — 所有玩家统一执行
    /// </summary>
    private void OnSyncReceived(JPlayer player, string data)
    {
        var args = SyncHelper.Decode(data);
        if (args.Length == 0) return;

        switch (args[0])
        {
            case "use":
                HandleUseItem(player, args);
                break;
            case "drop":
                HandleDropItem(player, args);
                break;
            case "swap":
                HandleSwapItems(player, args);
                break;
        }
    }

    private void HandleUseItem(JPlayer player, string[] args)
    {
        if (args.Length < 3) return;
        var unitEntity = SyncHelper.DecodeEntity(args[1]);
        int slotIndex = int.Parse(args[2]);

        // TODO: 执行物品使用逻辑
        // 查找该单位第 slotIndex 个物品，触发物品效果
    }

    private void HandleDropItem(JPlayer player, string[] args)
    {
        if (args.Length < 2) return;
        var itemEntity = SyncHelper.DecodeEntity(args[1]);

        // TODO: 执行物品丢弃逻辑
    }

    private void HandleSwapItems(JPlayer player, string[] args)
    {
        if (args.Length < 4) return;
        var unitEntity = SyncHelper.DecodeEntity(args[1]);
        int fromSlot = int.Parse(args[2]);
        int toSlot = int.Parse(args[3]);

        // TODO: 执行物品交换逻辑
    }
}
