# 设计：UI OOP 控件层

## 1. 控件树增强（`UIComponent`）

### 1.1 现有基类（保留不动）
`Handle / ParentHandle / IsDestroyed / Visible / SetPos / SetPoint / SetSize / Show / Hide / SetAlpha / Destroy`——全部保留，原生调用继续走 `FrameHelper`。

### 1.2 新增树成员
| 成员 | 说明 |
|---|---|
| `List<UIComponent>? Children` | 子控件集合（惰性创建，避免空控件分配） |
| `UIComponent? Parent` | 父控件引用 |
| `string Key` | 控件键（面板内唯一，默认自动生成） |
| `int Depth` | 树深度（渲染/遍历用，可选） |

### 1.3 生命周期钩子
- `virtual void OnConstruct()`：子类构造扩展点（xlik construct 链的 C# 等价：基类构造后由子类调用或 `Initialize()` 约定——采用**构造后显式 `Initialize()` 约定**，避免虚方法在基类构造器内被调用的 C# 陷阱）。
- `virtual void OnDestroy()`：销毁前清理（事件注销、子控件先销毁）。
- `Show(bool)` 增强：级联刷新子树可见性（`IsVisibleEffective = Visible && Parent 可见链`），并按可见性注册/注销鼠标事件（隐藏控件不占事件资源）。
- `Destroy()` 增强：先递归 `Destroy` 子控件 → 注销事件 → 调 `FrameHelper.Destroy`。

### 1.4 定位模型
- 现有 `SetPoint(point, relFrame, relPoint, ox, oy)` 保留（相对任意 Frame 定位，即"相对定位图"，父子树与定位图解耦）。
- 新增 `ResetAnchor()`：沿相对链（父锚点 + 偏移）递归换算屏幕绝对矩形 `Anchor { X, Y, W, H }`（左下角原点，坐标由 `FrameHelper.GetFrameRect` 或缓存换算），供命中检测/Tooltip 定位/拖拽边界复用；控件位置变化或父树变化时由调用方触发重算（显式，不做自动监听）。

## 2. 锚点缓存与命中检测（`UIAnchor`）

```csharp
public struct UIAnchor { public float X, Y, W, H; }

public static class UIAnchorHelper
{
    public static UIAnchor ResetAnchor(UIComponent ui);   // 递归换算并缓存到 ui.Anchor
    public static bool IsInside(UIComponent ui, float rx, float ry);  // 命中检测（含可见链过滤）
    public static bool IsBorder(UIComponent ui, float rx, float ry, float border);
}
```
- 命中检测前置条件：`IsVisibleEffective == true`，且无祖先遮挡（见第 5 节阻挡池）。
- Tooltip 定位复用：`UITooltip.PlaceNear(UIComponent ui)` 用 `ui.Anchor` 选弹出方向，屏幕边缘自动翻转（xlik 同思路）。

## 3. 事件桥接（`UIEventBridge`）

### 3.1 事件种类（`UIEventType` 枚举）
`LeftClick / LeftRelease / RightClick / RightRelease / Move / Enter / Leave / Wheel / DragStart / DragStop`（对照 xlik eventKind UI 组）。

### 3.2 注册与分发
```csharp
public static class UIEventBridge
{
    // 控件侧注册（由 UIComponent.OnConstruct 或面板显式调用）
    public static void Register(UIComponent ui, UIEventType evt, Action<UIEventContext> handler);
    public static void Unregister(UIComponent ui, UIEventType evt);

    // Native 回调入口（由 FrameHelper 事件回调调用，仅在 UINative 上下文）
    internal static void OnNativeEvent(UIComponent ui, UIEventType evt, int playerIndex, float x, float y);
}
```
- `OnNativeEvent` 流程：可见性过滤 → 命中检测（Enter/Leave 走状态机：控件级 `_hovered` 标记，边界切换才触发）→ 分发给控件 handler → **转写为 `UIEvent` ECS 实体**（由 `UINativeSystem` 在 ECS tick 内批量创建，避免 War3 事件回调上下文直接改 ECS——回调只入队）。
- enter/leave 状态机：`Register(Enter)` 时启用该控件悬停跟踪；鼠标移动事件统一入口检测所有"已注册 Enter"控件（数量少，全量遍历可控）。

### 3.3 ECS 回灌组件（`War3Frame/Src/Components/UI/UIEvent.cs`）
```csharp
public struct UIEvent : IComponent
{
    public int playerIndex;      // 触发玩家
    public int uiKeyHash;        // 控件键哈希（或 Entity 引用，design 定稿时选）
    public UIEventType type;     // 事件类型
    public float x, y;           // 屏幕坐标
}
```
- `UIEvent` 为独立事实实体（多监听者、只读），由统一清理系统删除；业务系统监听它生成 `XxxRequest`（如 `ItemUseRequest`、面板切换请求）。
- 命名遵守事件规则：独立实体 `XxxEvent`，零数据意图用 Request（本组件为事实，不改名）。

## 4. 控件清单（首期落地）

| 控件 | 基类 | 关键能力 | 状态 |
|---|---|---|---|
| `UIFrame`（背景容器） | UIComponent | 贴图 + 子控件容器 | 已有，增强树 |
| `UIText` | UIComponent | 文本/颜色/字体 | 已有，增强树 |
| `UITexture` | UIComponent | 贴图 | 已有，增强树 |
| `UIBar` | UIComponent | 数值条（遮罩比例） | 已有，增强树 + 比例 API 核对 |
| `UIButton` | UIComponent | 贴图 + 文本 + 点击事件 + 高亮状态 | 已有，增强事件桥接 |
| `UISlot` | UIComponent | 图标 + 数量文本 + 冷却遮罩 | 已有，增强事件桥接 |
| `UIBlock` | **新增** | 透明阻挡控件，登记全局阻挡池 | 新增 |

- 组合控件（Button 多子帧、Plate 带关闭按钮）用现有子类 + 树结构表达，不新增复杂组合类（P0 不引入 xlik 的 6 子帧组合，按需演进）。

## 5. 全局注册表

### 5.1 UIBlockPool（阻挡池）
```csharp
public static class UIBlockPool
{
    public static void Register(UIBlock block);   // 可见 + 开启阻挡时登记
    public static void Unregister(UIBlock block);
    public static bool IsBlocked(float x, float y);  // 命中检测：鼠标点是否被阻挡
}
```
- 鼠标操作安全区判定：游戏内点击/命令执行前查询 `UIBlockPool.IsBlocked`，被挡则忽略（对应 xlik `mouse.isSafety`）。
- 登记时机：`UIBlock.Show(true)` 且 `blockEnabled` 时入池，`Show(false)`/`Destroy` 出池（显隐联动，避免残留）。

### 5.2 Tooltip 增强
- 现有 `UITooltip` 增加内容驱动布局：`Show(icons[], texts[], bars[])` 按内容行数自动排版 + `PlaceNear(ui)` 锚点定位（P1 阶段）。

### 5.3 ESC 栈（可选 P2）
- `UIEscStack`：`Push(UIComponent)` / `Pop()`，后开先关（对照 xlik `UISetEsc`）；首期不做，标记为后续。

## 6. Native 边界（分层固化）

```
业务面板（UIPanel 子类）── 读 ECS 数据 → 调控件方法（写控件状态）
        │ 不直接调 FrameHelper / 原生 API
        ▼
OOP 控件层（UIComponent 族）── 薄入口，唯一持有 Frame handle
        │ 所有原生 Frame 调用集中在 FrameHelper（创建/销毁/位置/文本/贴图/事件注册）
        ▼
FrameHelper（现有）── Frame API 封装（DzFrame/BlzFrame 系）
        │
        ▼
UINativeSystem（新增，可选调度载体）── 事件队列 → UIEvent 实体；RefreshAll 调度
```

- 业务层禁止出现 `FrameHelper` 直接调用之外的 Frame 操作；`UIPanel.Refresh()` 只读 ECS + 调控件方法。
- 控件事件回调内禁止直接修改 ECS（只能入队，由 UINativeSystem 转写）。

## 7. 阶段拆分

| 阶段 | 内容 | 验收 |
|---|---|---|
| P0 | 控件树 + 锚点缓存 + UIBlock 阻挡池 | 树级联/命中/遮挡生效 |
| P1 | 事件桥接 + UIEvent 组件 + Tooltip 增强 | 点击/悬停产生 UIEvent 实体 |
| P2 | ESC 栈、自适应、组合控件演进（按需） | 可选 |

## 8. 验证场景（`Projects/test/Scripts/Process/UIPanelValidationScenario.cs`）

1. 构建测试面板：根 Frame + 子 Button + 子 UIBlock + Tooltip。
2. 断言：父隐藏 → 子树全部 `IsVisibleEffective=false`；`ResetAnchor` 后 `IsInside` 命中正确；UIBlock 登记后 `IsBlocked` 命中；模拟点击产生 `UIEvent` 实体且类型正确；Destroy 递归销毁子控件。