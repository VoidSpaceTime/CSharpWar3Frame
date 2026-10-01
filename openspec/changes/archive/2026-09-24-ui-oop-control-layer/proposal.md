# 提案：UI OOP 控件层（控件树 + 锚点 + 计算命中）

**状态**：已实施
**等级**：full（跨 UI 控件层 / 输入调度 / ECS 事件桥接，新增公共 authoring API）
**提案日期**：2026-08-31
**修订日期**：2026-09-24
**请求来源**：对标 xlik UI（`class/ui/*`、全局鼠标流、`UIEvent2Mouse` 计算命中），并按本仓库「UI 属 OOP 域」定案。

---

## 修订说明（2026-09-24）

本修订取代 2026-09-01 中与定案冲突的部分。已锁定，不再改口径：

1. **控件不是 ECS 实体**。控件树、锚点、z、命中、tooltip 全是 OOP。删除旧稿 `UIEvent.controlEntity`。事件只带稳定 `controlId`（int）。
2. **点击不再走逐帧 `DzFrameSetScript`**。`UIInputSystem` 只注册一次全局鼠标流，用锚点矩形做命中。`FrameHelper.OnEvent` 降为 EDITBOX/TEXTAREA 专用。
3. **相对 `SetPoint` 锚点从 P1 提入 P0**。DzApi 无 `GetFrameRect`，位置真相只在 `UIAnchor`。定位图用反向索引 `LowNodes`。
4. **层级与吞点击拆开**。`ZIndex` + `CompareZ` 决定谁收事件，默认 topmost-wins。`BlocksLower` 决定是否吞掉游戏世界点击。P0 禁止 `FrameSetPriority`。
5. **D 组不进本期**：脏投影替代 `RefreshAll`、技能/物品/Buff UI 元数据、槽位冷却投影，另开提案。

2026-09-01 仍有效的只有：`Show` 非虚、`ShowInternal` 扩展点、Frame 排除 `HandleHelper`、只处理本地玩家。`UIEvent` 不再挂 `TriggerEventMarker`，也不再占用 132 之前的 order。

---

## 背景

`War3Frame/Src/Core/UI/` 已有控件封装，但没有可复用的交互层：

| 现有件 | 已有 | 缺口 |
|---|---|---|
| `UIComponent` 及子类 | 创建、绝对/相对位置、文本、贴图都走 `FrameHelper` | 无父子集合、无级联显隐/销毁、无锚点缓存 |
| `UIPanel` / `UIManager` | 面板注册与 `RefreshAll` | 点击/悬停无统一命中；UI 事件不进 ECS |
| `FrameHelper.OnEvent` | `DzFrameSetScriptByCode` | 逐帧脚本是已知故障源，不能当通用点击入口 |

xlik 的点击不是逐帧 Frame 脚本，而是全局 mouse 流 + `isInsideUI`。它没有通用最上层仲裁，重叠控件会双触发。本提案保留计算命中，补上 topmost-wins。

## 目标

1. **控件树（P0）**：`Children` / `Parent`，显隐与销毁级联。`Show(bool)` 保持非虚；子类扩展走 `ShowInternal`。
2. **锚点（P0）**：中心 + 尺寸的 `UIAnchor`。绝对定位与相对 `SetPoint` 都换算成屏幕矩形。脏标记后按深度批算。
3. **输入（P0）**：一次注册全局鼠标/滚轮/移动。命中只遍历 `VisibleInteractiveRegistry`。
4. **层级（P0）**：创建序单调 `ZIndex`，`CompareZ` 封装比较。默认只派给最上层。`BlocksLower` 吞世界点击。
5. **事件桥（P1）**：派发结果写成 `UIEvent` 事实实体，供业务监听后发 `XxxRequest`。纯客户端显隐不进 ECS。
6. **边界**：原生 Frame 调用只在 `FrameHelper` 与 `UIInputSystem`。不建独立程序集。

## 非目标

- 不把控件树做成 ECS，也不把血条/漂浮字做进本期。
- 不做 UIModel、UIAnimate、ESC 栈。
- 不改 `UIPanel` / `UIManager.RefreshAll` 的现有签名（D 组后续再替换全刷）。
- 不把 slider/checkbox 做成原生控件。原生交互只留 EDITBOX/TEXTAREA，裁剪只留 `DzFrameSetClip`。
- 技能/物品/Buff 的图标、冷却、充能元数据不进本期。

## 影响范围

- `War3Frame/Src/Core/UI/`：增强 `UIComponent`；新增锚点、阻挡、命中注册、输入派发。
- `War3Frame/Src/Components/UI/`（P1）：`UIEvent` 事实组件。控件 id 不是实体。
- `War3Frame/Src/Systems/UI/`（P1）：`UIInputSystem` 消费鼠标队列并创建 `UIEvent`。
- `Projects/test`：面板级联、锚点命中、阻挡与事件断言。
- 不受影响：
  - `War3Frame.Generator/`：控件是普通类，不新增生成注册契约。`UIInputSystem` 只用现有 `[SystemRegister]`。
  - `FrameBuild/`、`CSharpWar3Frame/`、`BridgeToJIT/`、`FastMDX/`、`ModelFormat/`：不改构建与 CLI。
  - `HandleHelper`：Frame 仍不登记。

## 方案摘要

```
业务面板（OOP）读 ECS，只调控件方法
        ↓
UIComponent 树：锚点、ZIndex、BlocksLower、组合子控件
        ↓
UIInputSystem（order 133）：全局鼠标队列 → 批算锚点 → topmost-wins → UIEvent；order 140 清理
        ↓
UIEvent 事实实体（controlId，不是控件实体）→ 业务系统发 XxxRequest
        ↓
FrameHelper：唯一常规 Frame 写入口
```

## 风险与回滚

- **基类字段**会波及现有控件。构造签名保持不变；`Show` 保持非虚，避免子类丢掉级联。
- **锚点是唯一位置真相**。绕过 `FrameHelper` 直接 `DzFrameSetPoint/SetSize` 会使命中和画面分叉。P0 用单一写入口 + grep 门禁，不能靠原生回读补救。
- **全局单调 z** 不做父子局部叠层。嵌套面板若要「父面板整体压过另一面板，但子控件仍在父面板内」，必须保证子控件创建序晚于其面板、早于更高面板。这个约束写进验收；P0 不引入分组 z。
- **回调不能改 ECS**。鼠标回调只入队。`UIInputSystem` 在 order 133 写 `UIEvent`，不挂 `TriggerEventMarker`；桥接只在 134–139 读取，order 140 清理。不得插入 131 与 132 之间。
- 回滚：新增类型可删；`UIComponent` 旧构造与 `SetPos/SetPoint/SetSize/Show/Destroy` 签名保留。

## 验收标准

1. `dotnet build War3Frame/War3Frame.csproj` 0 错误。
2. 父 `Show(false)` 使子树 `IsVisibleEffective=false`；`Destroy` 递归销毁且只经 `FrameHelper.Destroy`。
3. `SetAbsPos` 与相对 `SetPoint` 链在 `ResetAnchor` 后，`IsInside` 与换算矩形一致；resize 置脏后下一 tick 全量重算。
4. 重叠控件只有 `CompareZ` 胜者收到点击；`BlocksLower` 命中时世界点击被标记吞掉。
5. P1：`UIEvent` 不挂 `TriggerEventMarker`。order 133 创建，134–139 能读到，140 删除；order 131 的触发器看不到它。只处理本地玩家。
6. grep：`SetPoint`、`SetSize`、`SetAbsPos`、`FillParent`、`SetScale`、`ClearPoints` 的控件侧调用只来自锚点批算；Frame 创建/销毁无 `HandleAdd/HandleRemove`；`UIButton.OnClick` 不再调用 `DzFrameSetScript`。

## 分级判定

- 影响 `War3Frame` 的 UI 核心与新增系统，不改生成器、构建、CLI。
- 公共 API 与输入契约变化，默认复盘 `R2`。
- 工件：`proposal.md`、`design.md`、`tasks.md`、`specs/ui-control.md`。
