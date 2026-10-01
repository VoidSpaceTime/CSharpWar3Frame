# ui-control Specification

## Purpose
TBD - created by archiving change ui-oop-control-layer. Update Purpose after archive.

## Requirements

### Requirement: 控件树保持 OOP

系统 SHALL 用 `UIComponent` 父子树表达面板结构。控件 SHALL NOT 作为 ECS 实体或组件存在。`Show(bool)` SHALL 保持非虚并级联有效可见性；子类扩展 SHALL 只重载 `ShowInternal`。

#### Scenario: 父控件隐藏

- **WHEN** 父控件 `Show(false)`
- **THEN** 子树 `IsVisibleEffective` 全部为 false，且子控件不在命中注册表中

#### Scenario: 递归销毁

- **WHEN** 销毁父控件
- **THEN** 子控件先销毁，每个 Frame 只经 `FrameHelper.Destroy` 销毁一次

### Requirement: 锚点是位置真相

系统 SHALL 用 `UIAnchor` 的中心与尺寸记录屏幕比例矩形，并在 P0 换算绝对定位与相对 `SetPoint`。系统 SHALL NOT 从 DzApi 回读 Frame 矩形。改变点、尺寸、缩放或铺满父节点的调用 SHALL 只由锚点批算进入 `FrameHelper`，包括 `FillParent`、`SetScale` 与 `ClearPoints`。`AnchorResolved=false` 的控件 SHALL NOT 进入 `VisibleInteractiveRegistry`。

#### Scenario: 相对链命中

- **WHEN** 控件通过 `SetPoint` 锚到另一个已注册控件并完成批算
- **THEN** `IsInside` 使用换算后的子矩形，而不是未解析的偏移

#### Scenario: 参考控件移动

- **WHEN** 被依赖控件的位置改变
- **THEN** 该控件与其 `LowNodes` 传递闭包被标脏，并在下一次命中前按深度完成一次批算

#### Scenario: 窗口尺寸变化

- **WHEN** 窗口 resize 事件入队
- **THEN** 下一 tick 全量重算锚点，回调本身不修改控件树

### Requirement: 计算命中与单胜者派发

系统 SHALL 只注册一次全局鼠标、滚轮和移动事件。通用点击 SHALL NOT 使用 `DzFrameSetScript`。命中 SHALL 只遍历有效可见的交互注册表，并用 `CompareZ` 选择唯一胜者。

#### Scenario: 重叠控件

- **WHEN** 两个可见交互控件都包含鼠标点
- **THEN** 只有 `ZIndex` 更大的控件收到点击；`ZIndex` 相等时 `ControlId` 更大者胜出

#### Scenario: 指针捕获

- **WHEN** slider 已按下并捕获指针，随后鼠标移出其矩形
- **THEN** 移动和松开仍发给该 slider，直到松开

#### Scenario: 原生输入例外

- **WHEN** 控件是 EDITBOX 或 TEXTAREA
- **THEN** 它可以使用原生输入脚本，且不进入通用点击派发

### Requirement: 吞点击与层级分离

`ZIndex` SHALL 只决定 UI 派发胜者。`BlocksLower` SHALL 只决定是否吞掉游戏世界点击。P0 SHALL NOT 暴露 `FrameSetPriority`。

#### Scenario: 面板空白处

- **WHEN** 已注册面板 `BlocksLower=true`，鼠标点落在其矩形内，且没有更高的已注册子控件包含该点
- **THEN** `WorldClickSwallowed` 为 true。本变更不断言单位选择系统已经消费该标记

#### Scenario: 装饰文本

- **WHEN** 鼠标点只落在 `BlocksLower=false` 且未注册交互的文本上
- **THEN** 不产生 UI 点击胜者，世界点击不被吞掉

#### Scenario: 阻挡面板上的非阻挡子控件

- **WHEN** 已注册子控件 `BlocksLower=false` 且位于已注册父面板 `BlocksLower=true` 的矩形内
- **THEN** 点击派发给 `CompareZ` 胜者，同时 `WorldClickSwallowed` 为 true

### Requirement: UIEvent 只表达玩法交互

系统 SHALL 把需要进入玩法的 UI 交互写成独立 `UIEvent` 实体，字段为 `PlayerIndex`、`ControlId`、`Type`、`X`、`Y`。`UIEvent` SHALL NOT 引用控件实体，也 SHALL NOT 挂 `TriggerEventMarker`。`UIInputSystem` SHALL 使用 order 133 创建事件。`UIEventCleanupSystem` SHALL 使用 order 140 删除事件。玩法桥接系统 order SHALL 为 134–139。纯客户端显隐 SHALL NOT 创建 `UIEvent`。桥接写出的 `XxxRequest` SHALL 由既有系统在后续 tick 消费。

#### Scenario: 本地玩法点击

- **WHEN** 本地玩家点击已映射到玩法命令的控件
- **THEN** order 134–139 的桥接能在 order 140 清理前读到对应 `ControlId` 的 `UIEvent`，且 order 131 的 `TriggerSystem` 本 tick 看不到该实体

#### Scenario: 非本地或纯 UI

- **WHEN** 事件不是本地玩家，或动作只是打开面板、切换页签
- **THEN** 玩法监听不产生 `XxxRequest`

### Requirement: Frame 不进入句柄引用计数

War3 Frame 的创建与销毁 SHALL NOT 调用 `HandleHelper.HandleAdd` 或 `HandleHelper.HandleRemove`。

#### Scenario: 创建并销毁控件

- **WHEN** 控件完成创建并销毁
- **THEN** 该路径没有 Frame 的 `HandleAdd` 或 `HandleRemove`

### Requirement: 面板层序由创建期决定

系统 SHALL 用固定层桶 `UILayer` 表达面板层序，并按层序在创建期确定 `ZIndex`。层序 SHALL NOT 在运行期改变。系统 SHALL NOT 暴露运行期提升层级的方法（例如 `Raise`、`BringToFront`）。此为匹配 War3 原生 Frame 层序等于创建序且不可重排的约束，避免命中层与视觉层分叉。

#### Scenario: 按层创建

- **WHEN** 面板按 `UILayer` 顺序（背景 → 面板 → 弹窗 → 模态 → 提示）创建
- **THEN** 后创建层的 `ZIndex` 更大，`CompareZ` 选出的命中胜者等于视觉最上层

#### Scenario: 显隐不改变层序

- **WHEN** 运行期对面板反复 `Show(true)` / `Show(false)`
- **THEN** 控件 `ZIndex` 保持不变，重叠命中胜者不因显隐而改变

#### Scenario: 无运行期提层入口

- **WHEN** 检查 UI 公共 API
- **THEN** 不存在修改已有控件 `ZIndex` 的方法，`ZIndex` 保持 `private set`

### Requirement: UIManager 按层注册面板

系统 SHALL 让 `UIManager` 按 `UILayer` 注册并创建面板。同层内的层序 SHALL 由注册顺序固定；同层面板若可能同时可见且重叠，其先后 SHALL 在设计期由注册顺序决定。注册 SHALL 只创建控件树与层序，不因创建进入命中注册表；面板初始可见性由业务显式 `Show` 决定。

#### Scenario: 同层重叠

- **WHEN** 同层两个面板都可见且包含同一鼠标点
- **THEN** `CompareZ` 胜者是后注册者，且与视觉最上层一致

#### Scenario: 注册后不自动进命中集

- **WHEN** 面板按层注册完成且业务未显式显示
- **THEN** 面板树已创建，但不在 `UIHitRegistry` 中
