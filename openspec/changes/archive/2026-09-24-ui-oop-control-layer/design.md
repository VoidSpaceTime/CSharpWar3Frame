# 设计：UI OOP 控件层

## 1. 锁定决策

| 项 | 决定 |
|---|---|
| 结构 | OOP 组合。新控件拼子控件，不新增长继承链 |
| 控件身份 | 不是 ECS 实体。`controlId` 由注册表分配 |
| 点击 | 全局鼠标流 + 矩形命中。禁止用逐帧 `DzFrameSetScript` 做通用点击 |
| 锚点 | P0 同时覆盖绝对位置和相对 `SetPoint` |
| 层级 | 创建序 `ZIndex`，只通过 `CompareZ` 比较 |
| 派发 | 默认 topmost-wins。`Consumed` 默认 true |
| 吞世界点击 | `BlocksLower`，与 z 分开 |
| `FrameSetPriority` | P0 不暴露。以后若暴露，必须同一处写入 `ZIndex` |
| 原生例外 | EDITBOX/TEXTAREA 的输入语义；滚动裁剪 `DzFrameSetClip` |
| 程序集 | 留在 `War3Frame`，目录分为 Core/Components/Systems |
| D 组 | 不在本期。`RefreshAll` 继续存在 |

## 2. 分层

```
UIPanel 子类
  读 ECS，调用控件方法
  禁止 FrameHelper 以外的 Frame 写操作，也禁止业务层直接 FrameHelper
        ↓
UIComponent 树（Src/Core/UI）
  父子、锚点、z、BlocksLower、pointer capture
        ↓
UIInputSystem（Src/Systems/UI）
  消费鼠标队列，批算锚点，命中，派发，创建 UIEvent
        ↓
FrameHelper
  DzCreate/Destroy、SetPoint、SetSize、SetVisible、文本、贴图、Clip
```

`UIInputSystem` 是原生执行系统：可以调用鼠标查询与窗口事件。控件类只通过 `FrameHelper` 改 Frame。

## 3. 控件树

保留现有 `Handle / SetPos / SetPoint / SetSize / Show / Hide` 签名。`Destroy()` 保持公开方法名，但去掉 `virtual`。

| 新增 | 规则 |
|---|---|
| `Parent` / `Children` | 结构树，管生命周期。惰性创建子列表 |
| `Key` | 面板内调试名，不参与命中 |
| `ControlId` | 注册时单调递增，销毁后不复用 |
| `ZIndex` | 创建时取单调序号，子控件晚于父控件 |
| `BlocksLower` | 默认：面板、按钮、槽位、阻挡为 true；文本、装饰贴图为 false |
| `Anchor` | 最近一次批算结果 |
| `AnchorDirty` | 位置、尺寸、定位父、窗口尺寸变化时置位 |

`Show(bool)` 非虚：写 `Visible`，调用 `FrameHelper.SetVisible`，重算子树 `IsVisibleEffective`，并维护命中注册表。子类只重载 `ShowInternal`。

`Destroy()` 改为非虚：先子后父，摘除命中注册与 `LowNodes`，调用 `OnDestroy`，再 `FrameHelper.Destroy` 一次。删除 `UIButton`、`UISlot`、`UITooltip` 的 `override Destroy`。`UISlot.CooldownOverlay` 只在 `OnDestroy` 里经 `FrameHelper.Destroy` 释放一次，不进入子控件销毁循环。

`Initialize()` 在构造完成后显式调用，避免基类构造器里调虚方法。

组合例子：按钮 = 底图 + 文本两个子控件，由 `UIButton` 创建并持有，不从 `UIPlate` 继承出拖拽类。`UIButton.Icon`、`UISlot.Icon`、`UISlot.CountText`、`UITooltip.Title`、`UITooltip.Description` 必须进入 `Children`。`CooldownOverlay` 不是子控件。

## 4. 锚点

```csharp
public struct UIAnchor
{
    public float X; // 中心，屏幕比例，左下原点
    public float Y;
    public float W;
    public float H;
}
```

- 几何写入口只有锚点批算。`SetAbsPos`、`SetPoint`、`SetSize`、`FillParent`、`SetScale`、`ClearPoints` 都不得由控件构造或业务面板直接改变命中矩形。`UIButton`、`UISlot` 现有的 `FillParent` 调用改为批算写出的矩形；未解析出 `AnchorResolved` 的控件不得进入 `VisibleInteractiveRegistry`。
- 相对定位：`SetPoint` 记录参考控件 `ControlId`、双方锚点枚举与偏移。参考控件必须已注册。允许不是结构父节点。
- `LowNodes`：参考控件指向依赖它的控件。一个控件移动，只把自己和 `LowNodes` 传递闭包标脏。
- 换算：九宫格把参考矩形的对应点加上偏移，得到本控件锚点，再由中心与尺寸得到矩形。`UIGame` 参考矩形是 `{X:0.4, Y:0.3, W:0.8, H:0.6}`，只作为根参考，不是唯一父节点。
- 批算：`UIInputSystem` 命中前按 `Depth` 升序处理脏控件。同 tick 多次改动只算一次。
- 窗口 `DzTriggerRegisterWindowResizeEventByCode` 只置全局脏标记，下一 tick 全量重算。
- `UINative`（`DzGetGameUI` 等不可回读位置的原生 Frame）可做创建父节点，不进入命中集，也不能当 `LowNodes` 的可计算参考。相对它的控件必须自己有绝对矩形。

P0 不做自适应字号，不做最小/最大约束。

## 5. 输入与命中

注册一次：

- `DzTriggerRegisterMouseEventByCode`
- `DzTriggerRegisterMouseWheelEventByCode`
- `DzTriggerRegisterMouseMoveEventByCode`
- 坐标用 `DzGetMouseXRelative` / `DzGetMouseYRelative`
- 回调只写入原始输入队列

命中：

1. 若存在 pointer capture，移动和松开都发给捕获控件，直到松开清除 capture。
2. 否则遍历 `VisibleInteractiveRegistry`。条件：`IsVisibleEffective` 且点在锚点矩形内。
3. `CompareZ` 取最大 `ZIndex`；相等时取更大 `ControlId`。
4. 只把事件派给胜者。胜者默认 `Consumed=true`，不再传给更低控件。
5. 派发胜者与吞世界点击分开判定。胜者仍是注册表内 `CompareZ` 最大者。`WorldClickSwallowed` 为 true 的条件是：注册表内至少一个命中控件的 `BlocksLower=true`，即使胜者本身为 false。未注册控件既不派发，也不参与吞点击。本变更只发布标记，不改单位选择或命令系统；验收只断言标记。

`VisibleInteractiveRegistry` 在有效可见性变化时进出，不在命中时扫描整棵树。

checkbox、slider 用锚点矩形和捕获状态自己算值。slider 按下即捕获。EDITBOX/TEXTAREA 仍可注册原生脚本，且不参与这套点击派发。

`FrameSetPriority` 在 P0 没有包装方法。

## 6. ECS 接缝

| 接缝 | 本期 |
|---|---|
| 游戏数据 → UI | 面板 `Refresh()` 继续读 ECS。不把控件状态写成组件 |
| UI → 游戏 | 仅玩法交互创建 `UIEvent`。开关面板、页签、本地选中不创建 |
| 实体绑定控件 | 不实现。以后由系统创建/销毁 OOP 控件，控件本身仍不是组件 |

```csharp
public struct UIEvent : IComponent
{
    public int PlayerIndex;
    public int ControlId;
    public UIEventType Type;
    public float X;
    public float Y;
}
```

`UIEvent` 不挂 `TriggerEventMarker`。`TriggerSystem` 现为 order 131，会扫描全部带该标记的实体，且 `eventTypeId == 0` 的规则匹配所有事件。131 与 `EventCleanupSystem`（132）之间没有空位，现有 120–131 也被伤害、治疗、效果和物品系统占用。

因此 `UIInputSystem` 使用 order 133，在 `TriggerSystem` 与 `EventCleanupSystem` 之后创建 `UIEvent`。专用 `UIEventCleanupSystem` 使用 order 140 删除这些实体。玩法桥接系统 order 只能是 134–139，并在清理前读到本 tick 的 `UIEvent`。它们只写 `XxxRequest`；既有 0–132 系统不改 order，请求在后续 tick 消费。

业务桥接用 `ControlId` 查面板注册的命令映射。映射表属于面板，不是实体组件。

`PlayerIndex` 是触发玩家。监听方只处理本地玩家。跨玩家同步由后续业务请求负责。

## 7. 事件种类

`LeftClick / LeftRelease / RightClick / RightRelease / Move / Enter / Leave / Wheel / DragStart / DragStop`。

Enter/Leave 由命中胜者切换驱动：同一控件连续移动不重复 Enter。捕获期间不给其他控件发 Enter/Leave。

P1 才把派发结果写入 `UIEvent`。P0 只完成 OOP 命中与 `BlocksLower` 标记，测试可直接断言派发目标。

## 8. 销毁与句柄

Frame 不调用 `HandleAdd` / `HandleRemove`。结构子控件只由非虚 `Destroy()` 销毁一次。不属于 `Children` 的裸 Frame 只在 `OnDestroy` 销毁一次。`UIButton.OnClick` 现有的 `FrameHelper.OnClick` 属于逐帧脚本入口，P0 删除，不再作为通用点击路径。

## 9. 阶段

| 阶段 | 范围 |
|---|---|
| P0 | 树、锚点（含相对链）、注册表、z、`BlocksLower`、鼠标队列与 OOP 派发、capture |
| P1 | `UIEvent`、本地玩家过滤、Tooltip 用锚点做 `PlaceNear` |
| 后续 | ESC 栈、分组 z、D 组脏投影、实体血条、漂浮字 |

分组 z 的升级条件：出现「面板 A 的晚创建子控件必须压过面板 B，同时 B 的子控件必须压过 A」且单序号无法表达。P0 用创建顺序规避，不预建分组字段。

## 10. 验证

`Projects/test` 增加纯逻辑场景；不能在无 War3 客户端时调用 DzApi。锚点换算、`CompareZ`、topmost-wins、`BlocksLower`、脏闭包用可注入矩形与队列测试。

War3 内手动项（真实鼠标、IME、resize）记在任务里，不作为本提案归档的阻塞替代：实现完成前必须有自动化逻辑断言；客户端项未跑时，`summary.md` 记录未执行原因。

构建：`dotnet build War3Frame/War3Frame.csproj`，以及包含新场景的 `Projects/test` 项目。
