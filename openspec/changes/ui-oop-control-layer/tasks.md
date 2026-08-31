# 任务清单：UI OOP 控件层

状态约定：`[ ]` 待办 / `[x]` 完成。按 P0/P1/P2 阶段拆分，验证任务内联标注。

## P0：控件树 + 锚点缓存 + 阻挡池

- [ ] `War3Frame/Src/Core/UI/UIComponent.cs`：新增树成员（Children/Parent/Key/Depth）+ `Initialize()` 生命周期约定 + `OnConstruct`/`OnDestroy` 虚钩子
- [ ] `UIComponent.Show(bool)` 增强：级联刷新子树可见性（IsVisibleEffective = Visible && 父可见链）+ 按可见性注册/注销鼠标事件
- [ ] `UIComponent.Destroy()` 增强：递归销毁子控件 → 注销事件 → FrameHelper.Destroy
- [ ] 新增 `War3Frame/Src/Core/UI/UIAnchor.cs`：`UIAnchor` 结构 + `UIAnchorHelper`（ResetAnchor/IsInside/IsBorder）
- [ ] 新增 `War3Frame/Src/Core/UI/UIBlock.cs`：透明阻挡控件 + 显隐联动登记
- [ ] 新增 `War3Frame/Src/Core/UI/UIBlockPool.cs`：Register/Unregister/IsBlocked
- [ ] 核对现有子类（UIFrame/UIText/UITexture/UIBar/UIButton/UISlot）构造签名不变，接入树
- [ ] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误

## P1：事件桥接 + UIEvent 回灌 + Tooltip 增强

- [ ] 新增 `War3Frame/Src/Components/UI/UIEvent.cs`：`UIEventType` 枚举 + `UIEvent` 事实组件（playerIndex/uiKeyHash/type/x/y）
- [ ] 新增 `War3Frame/Src/Core/UI/UIEventBridge.cs`：Register/Unregister（控件侧）+ OnNativeEvent（Native 入口：可见性过滤→命中检测→enter/leave 状态机→分发→入队）
- [ ] enter/leave 状态机：控件级 `_hovered` 标记 + 边界切换触发
- [ ] 新增 `War3Frame/Src/Systems/UI/UINativeSystem.cs`：消费事件队列 → 创建 `UIEvent` 实体（ECS tick 内转写，回调上下文不入 ECS）
- [ ] `UITooltip` 增强：内容驱动布局（icons/texts/bars）+ `PlaceNear(ui)` 锚点定位 + 屏幕边缘翻转
- [ ] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误

## P2：验证场景

- [ ] 新增 `Projects/test/Scripts/Process/UIPanelValidationScenario.cs`：根 Frame + 子 Button + UIBlock + Tooltip
- [ ] 断言：父隐藏→子树 IsVisibleEffective 全 false；ResetAnchor 后 IsInside 命中正确；UIBlock 登记后 IsBlocked 命中；模拟点击产生 UIEvent 实体且类型正确；Destroy 递归销毁子控件
- [ ] 验证：`dotnet build Projects/test/` 0 错误

## P3：实施后验证

- [ ] 全仓 grep：业务层（非 `War3Frame/Src/Core/UI/`）无直接 Frame 原生调用（除 FrameHelper 封装）
- [ ] 事件回调内无直接 ECS 修改（全部经 UINativeSystem 转写）
- [ ] 按 full 级 `R2 Targeted` 复盘：技术准确性 + 兼容性视角