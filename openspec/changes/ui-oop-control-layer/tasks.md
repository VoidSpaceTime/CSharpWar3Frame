# 任务清单：UI OOP 控件层

状态约定：`[ ]` 待办 / `[x]` 完成。按 P0/P1/P2 阶段拆分，验证任务内联标注。

## P0：控件树 + 锚点缓存（SetAbsPos 定位控件）+ 阻挡池

- [ ] `War3Frame/Src/Core/UI/UIComponent.cs`：新增树成员（Children/Parent/Key/Depth）+ `Initialize()` 生命周期约定 + `OnConstruct`/`OnDestroy` 虚钩子
- [ ] `UIComponent.Show(bool)` 增强：**保持非虚方法**，级联刷新子树可见性（IsVisibleEffective = Visible && 父可见链）；新增 `protected virtual ShowInternal(bool visible)` 供子类扩展；P0 不包含鼠标事件注册/注销（移入 P1）
- [ ] `UIComponent.Destroy()` 增强：递归销毁子控件 → FrameHelper.Destroy（P0 无事件注销）
- [ ] 新增 `War3Frame/Src/Core/UI/UIAnchor.cs`：`UIAnchor` 结构 + `UIAnchorHelper`（ResetAnchor/IsInside/IsBorder）；**P0 仅缓存 SetAbsPos 定位控件锚点**，SetPoint 链换算推迟 P1
- [ ] 新增 `War3Frame/Src/Core/UI/UIBlock.cs`：透明阻挡控件 + 显隐联动登记
- [ ] 新增 `War3Frame/Src/Core/UI/UIBlockPool.cs`：Register/Unregister/IsBlocked
- [ ] 核对现有子类（UIFrame/UIText/UITexture/UIBar/UIButton/UISlot）构造签名不变，接入树；UIButton/UISlot 当前未重载 Show，无迁移任务
- [ ] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误
- [ ] 验证：SetAbsPos 定位控件 ResetAnchor 后 IsInside 命中与屏幕实际一致；SetPoint 链换算标记为 P1

## P1：事件桥接 + UIEvent 回灌 + SetPoint 链锚点换算 + Tooltip 增强

- [ ] 新增 `War3Frame/Src/Components/UI/UIEvent.cs`：`UIEventType` 枚举 + `UIEvent` 事实组件（playerIndex/controlEntity/type/x/y）；**必须挂载 `TriggerEventMarker`**
- [ ] 新增 `War3Frame/Src/Core/UI/UIEventBridge.cs`：Register/Unregister（控件侧，按可见性注册/注销）+ OnNativeEvent（Native 入口：可见性过滤→命中检测→enter/leave 状态机→分发→入队）
- [ ] enter/leave 状态机：控件级 `_hovered` 标记 + 边界切换触发
- [ ] 新增 `War3Frame/Src/Systems/UI/UINativeSystem.cs`：消费事件队列 → 创建 `UIEvent` 实体（ECS tick 内转写，回调上下文不入 ECS）；**order < 132**（早于 EventCleanupSystem）
- [ ] `UIComponent.Show(bool)` P1 补充：按可见性注册/注销鼠标事件（隐藏控件不占事件资源）
- [ ] `UIComponent.Destroy()` P1 补充：递归销毁子控件 → 注销事件 → FrameHelper.Destroy
- [ ] `UIAnchorHelper` P1 补充：实现 SetPoint 链锚点递归换算（相对定位控件）
- [ ] `UITooltip` 增强：内容驱动布局（icons/texts/bars）+ `PlaceNear(ui)` 锚点定位 + 屏幕边缘翻转
- [ ] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误
- [ ] 验证：UIEvent 挂载 TriggerEventMarker；UINativeSystem order < 132；业务监听系统过滤 playerIndex == GetLocalPlayer() 生效

## P2：验证场景

- [ ] 新增 `Projects/test/Scripts/Process/UIPanelValidationScenario.cs`：根 Frame + 子 Button + UIBlock + Tooltip
- [ ] **P0 断言**：父隐藏→子树 IsVisibleEffective 全 false；SetAbsPos 定位控件 ResetAnchor 后 IsInside 命中正确；UIBlock 登记后 IsBlocked 命中；Destroy 递归销毁子控件；ShowInternal 虚钩子可被子类重载
- [ ] **P1 断言**：模拟点击产生 UIEvent 实体且类型正确，挂载 TriggerEventMarker；业务监听系统过滤 playerIndex == GetLocalPlayer() 生效；enter/leave 不重不漏；监听系统 order < 132；SetPoint 链定位控件命中检测生效
- [ ] 验证：`dotnet build Projects/test/` 0 错误

## P3：实施后验证

- [ ] 全仓 grep：业务层（非 `War3Frame/Src/Core/UI/`）无直接 Frame 原生调用（除 FrameHelper 封装）
- [ ] 事件回调内无直接 ECS 修改（全部经 UINativeSystem 转写）
- [ ] grep 审查：War3 Frame 创建/销毁点无 HandleAdd/HandleRemove 调用（显式排除出句柄引用计数）
- [ ] 按 full 级 `R2 Targeted` 复盘：技术准确性 + 兼容性视角