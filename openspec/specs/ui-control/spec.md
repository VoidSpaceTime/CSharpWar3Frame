# ui-control Specification

## Purpose
Godot 风格 UI 控件层：以 OOP 控件树、属性化控件、锚点布局、命中派发与容器/kit 提供控件能力，并通过 `DzSyncData` 提供 UI 输入到 ECS 请求的同步出口。

## Requirements

### Requirement: 控件树保持 OOP

系统 SHALL 用 `UIComponent` 父子树表达 UI 结构，子控件通过 `AddChild` 加入。控件 SHALL NOT 作为 ECS 实体或组件存在。可见性 SHALL 用 `Visible` 属性表达并级联到子树；父隐藏时子树 SHALL 不进入命中注册表。

#### Scenario: 父控件隐藏

- **WHEN** 父控件 `Visible = false`
- **THEN** 子树 `IsVisibleEffective` 全部为 false，且子控件不在命中注册表中

#### Scenario: 递归销毁

- **WHEN** 销毁父控件
- **THEN** 子控件先销毁，每个 Frame 只经 `FrameHelper.Destroy` 销毁一次

### Requirement: 锚点是位置真相

系统 SHALL 以屏幕比例矩形（`UIAnchor` 的锚点与尺寸）作为控件位置真相。系统 SHALL NOT 从 DzApi 回读 Frame 矩形。定位 SHALL 由 anchors/preset 与相对关系决定，并 SHALL 只由布局批算写入 `FrameHelper`；`AnchorResolved=false` 的控件 SHALL NOT 进入命中注册表。

#### Scenario: 相对链命中

- **WHEN** 控件通过相对关系锚到另一个已注册控件并完成批算
- **THEN** 命中使用换算后的子矩形，而不是未解析的偏移

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

`ZIndex` SHALL 只决定 UI 派发胜者。`MouseFilter`（`Stop` / `Pass` / `Ignore`）SHALL 决定控件是否消费指针事件以及是否吞掉游戏世界点击。系统 SHALL NOT 暴露 `FrameSetPriority`。

#### Scenario: 面板空白处

- **WHEN** 已注册面板 `MouseFilter = Stop`，鼠标点落在其矩形内，且没有更高的已注册子控件包含该点
- **THEN** `WorldClickSwallowed` 为 true；本变更不断言单位选择系统已消费该标记

#### Scenario: 装饰文本

- **WHEN** 鼠标点只落在 `MouseFilter = Ignore` 的文本上
- **THEN** 不产生 UI 点击胜者，世界点击不被吞掉

#### Scenario: 阻挡面板上的非阻挡子控件

- **WHEN** 已注册子控件 `MouseFilter = Pass` 且位于已注册父面板 `MouseFilter = Stop` 的矩形内
- **THEN** 点击派发给 `CompareZ` 胜者，同时 `WorldClickSwallowed` 为 true

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

### Requirement: 控件 API 属性化

系统 SHALL 用属性表达控件状态：`Position`、`Size`、`Alpha`。位置与 `Size` SHALL 使用屏幕比例，SHALL NOT 使用像素语义。`ZIndex` SHALL 只读，由创建期层序决定。控件 SHALL 通过 `AddChild` 加入父节点，构造 SHALL NOT 隐式挂树。

#### Scenario: 属性写入即生效

- **WHEN** 设置控件的 `Position` / `Size` / `Alpha`
- **THEN** 锚点被标脏并在下一次批算投影到 Frame，调用方无需手写坐标换算

#### Scenario: ZIndex 只读

- **WHEN** 检查 UI 公共 API
- **THEN** 不存在可写 `ZIndex` 的成员，层序只能由创建序决定

### Requirement: 控件级交互事件

命中胜者 SHALL 由输入层直接投递 C# 事件到控件，事件至少覆盖 `Pressed`、`DoublePressed`、`PointerEntered`、`PointerExited`、`Drag`。双击 SHALL 由输入层内建判定，调用方 SHALL NOT 自行实现双击窗口。命中 SHALL NOT 创建 ECS 实体。输入源 SHALL 只使用 War3/DzApi，SHALL NOT 引入 Win32 硬件输入。

#### Scenario: 点击直投

- **WHEN** 本地玩家点击某控件且它是命中胜者
- **THEN** 该控件的 `Pressed` 事件被调用，且本 tick 不产生任何 ECS 实体

#### Scenario: 内建双击

- **WHEN** 同一控件在双击窗口内收到两次按下
- **THEN** 触发一次 `DoublePressed`；超出窗口时只算两次单击

#### Scenario: 捕获期间不切换

- **WHEN** 控件已捕获指针
- **THEN** 移动与松开仍投递给捕获控件，其他控件不收到 `PointerEntered`

### Requirement: 容器自动布局

系统 SHALL 提供容器控件，由容器计算子控件矩形：`VBoxContainer`（竖排）、`HBoxContainer`（横排）、`CenterContainer`（居中）、`MarginContainer`（内边距）。容器 SHALL 在自身尺寸或子控件集合变化时重算子控件矩形；调用方 SHALL NOT 为容器子控件手写坐标。

#### Scenario: 横排子控件

- **WHEN** 向 `HBoxContainer` 连续 `AddChild` 三个子控件
- **THEN** 三者按横排顺序排布，位置由容器计算；容器尺寸变化后重算

#### Scenario: 内边距

- **WHEN** 子控件加入 `MarginContainer`
- **THEN** 子控件矩形在容器内缩进，四周保留配置的边距

### Requirement: kit 分组与幂等创建

系统 SHALL 用 `UIKit` 实例（非全局静态）分组管理面板：`Add` 记录层桶与注册序，`Build` 按 `(层序, 注册序)` 创建，创建后初始隐藏；同层先后由注册序固定。同名 key SHALL 幂等创建，重复获取返回同一实例。系统 SHALL NOT 依赖全局静态 UI 管理器。

#### Scenario: 按层创建

- **WHEN** 向 kit 注册不同层的面板后调用 `Build`
- **THEN** 创建序按层序，后创建层 `ZIndex` 更大

#### Scenario: 幂等获取

- **WHEN** 以相同 key 重复获取同一控件
- **THEN** 返回同一实例，不重复创建

#### Scenario: 显隐不改变层序

- **WHEN** 运行期反复显示/隐藏面板
- **THEN** 控件 `ZIndex` 不变，重叠命中胜者不变

### Requirement: UI 同步出口

需要同步的 UI 动作 SHALL 通过 `DzSyncData` 发送；接收 SHALL 由 `SyncHelper.Register` 的 prefix 处理器承担。处理器 SHALL 只写入 ECS 请求/状态，游戏逻辑 SHALL 由框架系统消费该请求，SHALL NOT 在同步回调中直接改写游戏状态。ECS 事件系统 `131/132` SHALL NOT 被改动，SHALL NOT 保留 UI 专用的 133/140 事件通道。

#### Scenario: 双击选择英雄同步

- **WHEN** 本地玩家双击某英雄头像
- **THEN** 发送同步数据，所有玩家在同步处理器中写入同一 ECS 请求，由框架系统消费，结果一致

#### Scenario: 无 UI 专用事件通道

- **WHEN** 检查 interval order 占用
- **THEN** 不存在 UI 专用的 133/140 事件系统，`UIEvent` 类型不参与命中派发
