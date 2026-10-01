# 设计：UI 控件层对齐 Godot

## 1. 总原则

- UI 是**本地 OOP 域**，不进 ECS。控件、布局、交互、分组全是 OOP。
- 与 ECS 的唯一接触面是**同步出口**：UI 动作需要同步时，走 `DzSyncData` → `SyncHelper` → 落 ECS 请求 → 框架系统消费。
- `FrameHelper` 是 UI 的 **Native 执行层**（唯一 `DzFrame*` 写入口）；控件只写语义，不直接调原生。
- 复用内核：`UIComponent` 树、`UIAnchor`、命中注册表、`LowNodes`、`UILayer`。

## 2. Godot 对齐表

| Godot | 采用 | 偏离（War3 约束） |
|---|---|---|
| `Node` 树 + `AddChild` | ✅ 直接采用 | — |
| `Control` 属性（`Position/Size/Visible`） | ✅ 采用 | `Size`/位置用**比例**，不用像素 |
| 信号 / C# 事件 | ✅ 采用（`Pressed/DoublePressed/PointerEntered/PointerExited/Drag`） | 双击内建（Godot 无） |
| `mouse_filter` STOP/PASS/IGNORE | ✅ 采用，替换 `BlocksLower` | — |
| anchors + offsets | ✅ 锚点用比例 | offsets 也是比例，不做像素语义 |
| `Container`（VBox/HBox/Center/Margin） | ✅ 首批四个 | 其余（Grid/Scroll/Tab）后续 |
| `CanvasLayer` / `z_index` | ❌ | **创建期** `UILayer` 分层；z 运行期不可改 |
| `Theme` / `StyleBox` | ❌ 二期 | — |
| `Focus` / 键盘导航 | ❌ 二期 | 需键盘输入源 |
| `Window` / `Popup` | ❌ | 用 `UILayer.Modal` + 遮罩模拟 |

## 3. 分层

```
业务 / 面板（UIKit / UIPanel）
   读 ECS，绑控件 C# 事件
        ↓
控件层（UIComponent + 容器）
   属性、事件、AddChild、比例锚点    ← 纯 OOP，无 ECS
        ↓
输入层（UIInputSystem 重定位为 OOP 服务）
   War3 鼠标/滚轮/移动/resize/键盘 单次注册 → 队列 → 命中 → 直投控件事件
        ↓
FrameHelper（Native 执行层，唯一 DzFrame*）
        ↓
同步出口（仅需同步时）
   DzSyncData → SyncHelper.Register → ECS 请求 → 框架桥系统
```

## 4. 控件 API

属性 + `AddChild` + C# 事件，示例（英雄选择）：

```csharp
var row = new HBoxContainer { Spacing = 0.01f };
row.Size = new Vector2(0.4f, 0.12f);
panel.AddChild(row);

foreach (var hero in roster)
{
    var p = new UITextureButton(hero.Icon);
    p.Size = new Vector2(0.06f, 0.06f);
    p.DoublePressed += () => SyncHero(hero);   // 双击内建
    row.AddChild(p);
}
```

对照现状：曾需 `UIPanel` + `QuerySystem<UIEvent>` + `ControlId` 字典 + 手算 0.3s 窗口 + `UIManager/Build/Show` 工序。

## 5. 输入与交互

- **来源**：只走 War3/DzApi（鼠标按下/松开/移动/滚轮/resize/键盘）。**不引 Win32**。
- **队列**：回调只入队（`UIInputQueue` 保留，作为可插拔抽象点）。
- **命中**：批算脏锚点 → 遍历交互注册表 → `CompareZ` 取唯一胜者（沿用现状）。
- **投递**：命中后**直接调用控件 C# 事件**；不再创建 `UIEvent`。
- **双击**：输入层用 `Tick.time`（系统层）或队列时间戳内建判定，窗口默认 0.3s。
- **PointerEntered/Exited**：由胜者切换驱动；捕获期间不切换。

## 6. 层级

`UILayer`（背景 → 面板 → 弹窗 → 模态 → 提示）保持不变；层序=创建序，运行期不可改。`UIKit.Build()` 是唯一的按层创建入口。

## 7. 同步出口

- **发送**：控件事件处理器内调 `DzSyncData(prefix, data)`（或封装 `SyncHelper` 发送助手）。
- **接收**：`SyncHelper.Register(prefix, handler)`，所有玩家执行同一处理器。
- **处理器职责**：只把数据落成 **ECS 请求/状态**（如 `HeroPickedRequest`），**不直接改游戏状态**。
- **消费**：由 `War3Frame` 内的**框架通用桥系统**消费该请求（`(a)` 方案）。
- **约束**：`TriggerSystem(131)` / `EventCleanupSystem(132)` 不动；不再有 UI 专用的 133/140。

## 8. kit 与面板

- `UIKit`：**实例**（非静态），持有本组面板；`Add(panel, layer)` / `Build()` / `Show(name)` / `Hide(name)` / `Destroy()`；`Build` 按 `(layer, 注册序)` 创建，创建后隐藏。
- `UIPanel`：瘦身为"一棵子树 + 生命周期钩子"（`OnBuild / OnShow / OnHide / Refresh`），`Root` 为子树根；归所属 kit 管理。
- **key 缓存**：同名 key 幂等创建，重复获取返回同一实例（吸收 xlik）。
- 全局静态 `UIManager` 删除；全局 `Tooltip` 移入 kit/显式根。

## 9. 迁移

| 现状 | 迁移到 |
|---|---|
| `UIEvent` + `UIInputSystem(133)` + `UIEventCleanupSystem(140)` | 删除；改控件 C# 事件直投 |
| `UIManager.Register/Build/ShowPanel`（静态） | `new UIKit(kit).Add(..).Build().Show(..)` |
| `UIComponent.SetSize/SetPos/Show` | 属性 `Size/Position/Visible`（不留旧方法） |
| `BlocksLower` | `MouseFilter`（Stop/Pass/Ignore） |
| `SetPoint` 手写相对定位 | anchors/preset + `Relation` |
| `HeroSelectExample`（`QuerySystem<UIEvent>` 版） | C# 事件 + 同步请求版 |

## 10. 回滚

- 变更集中在 `War3Frame/Src/Core/UI`、`Systems/UI`、`Components/UI/UIEvent.cs` 与 `Projects/test`。
- 回滚 = 恢复 `UIEvent` 三件与前一轮 `ui-control` 规约；`UIAnchor`/命中注册表/`UILayer` 不受影响。

## 11. 分期

| 阶段 | 范围 |
|---|---|
| P0 | 属性化控件 + `AddChild` + C# 事件直投 + 内建双击；删 `UIEvent` 三件 |
| P1 | anchors/preset + 容器 `VBox/HBox/Center/Margin` |
| P2 | `UIKit` 实例 + `UIPanel` 瘦身 + key 缓存 + UI→ECS 同步桥 |
| 二期 | Focus/键盘导航、Theme/StyleBox、Grid/Scroll 容器 |

## 12. 风险

- **公共 API 大改**：`SetXxx`→属性、`UIEvent` 删除是 BREAKING；靠"几乎无外部调用方"降低，但需逐项核对调用点。
- **同步桥是新增框架件**：必须落 ECS 请求、不在回调改状态，否则破坏确定性。
- **容器布局**：需接入现有脏批算，避免每帧全量重算。
- **输入重定位**：`UIInputSystem` 从 ECS 系统降为 OOP 服务后，要保证仍只注册一次原生事件。

## 13. 影响分析

- `Generator` / `FrameBuild` / `CSharpWar3Frame` / `BridgeToJIT`：不受影响（系统数减少，无新注册契约）。
- ECS 事件系统 `131/132`：不动。
- `HandleHelper`：Frame 仍不登记。
- `Projects/test`：示例与回归重写。
