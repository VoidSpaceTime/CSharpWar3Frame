# 任务清单：UI OOP 控件层

状态：`[ ]` 待办 / `[x]` 完成。未批准前不开始实现。

## P0：树、锚点、命中

- [x] `UIComponent` 增加 `Parent/Children/ControlId/ZIndex/BlocksLower/Anchor/AnchorDirty/AnchorResolved`。保留现有构造与 `Show/SetPos/SetPoint/SetSize` 签名。`Destroy()` 去掉 `virtual`。
- [x] `Show(bool)` 保持非虚，级联 `IsVisibleEffective`，并同步 `VisibleInteractiveRegistry`。新增 `protected virtual ShowInternal(bool)`。
- [x] 非虚 `Destroy()` 先销毁子控件并摘除 `LowNodes`，再调用 `OnDestroy` 与一次 `FrameHelper.Destroy`。删除 `UIButton`、`UISlot`、`UITooltip` 的 override。`CooldownOverlay` 只在 `OnDestroy` 释放一次。
- [x] 新增 `UIAnchor` 与批算：绝对定位、相对 `SetPoint`、九宫格、`LowNodes` 脏闭包、按深度一次结算。
- [x] 根参考矩形使用 `{X:0.4, Y:0.3, W:0.8, H:0.6}`。`UINative` 不进命中集，也不能充当可计算参考。
- [x] 新增命中注册表、`CompareZ`、topmost-wins、`BlocksLower` 世界点击标记、pointer capture。
- [x] `UIInputSystem` 注册一次鼠标按下/松开/移动/滚轮与窗口 resize。回调只入队。命中前先批算脏锚点。
- [x] 不添加 `FrameSetPriority` 包装。删除 `UIButton.OnClick` 的逐帧脚本路径。checkbox/slider 不注册通用原生点击。`FillParent`、`SetScale`、`ClearPoints` 只允许锚点批算调用。未解析锚点不得进注册表。
- [x] 现有 `UIFrame/UIText/UITexture/UIBar/UIButton/UISlot` 构造签名不改。按钮与槽位默认 `BlocksLower=true`；文本与装饰默认 false。`Icon`、`CountText`、`Title`、`Description` 进入 `Children`。
- [x] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误。
- [x] 验证：父隐藏、递归销毁、绝对/相对命中、重叠只派胜者、resize 置脏、capture 移出后仍收到移动。非阻挡子控件压在 `BlocksLower=true` 的已注册父面板上时，点击派给胜者且 `WorldClickSwallowed=true`。

## P1：UIEvent 与 Tooltip

- [x] 新增 `UIEvent`：`PlayerIndex/ControlId/Type/X/Y`。不挂 `TriggerEventMarker`，不出现控件 `Entity` 字段。
- [x] 实现前确认没有 `SystemKind.Interval` 的 order ≥ 133。`UIInputSystem` 使用 133，`UIEventCleanupSystem` 使用 140，玩法桥接使用 134–139。面板开关与页签不创建 `UIEvent`。桥接只写 `XxxRequest`，不改既有系统 order。
- [x] Enter/Leave 只在命中胜者变化时触发；capture 期间不切换到其他控件。
- [x] Tooltip `PlaceNear` 使用锚点矩形，贴边翻转。
- [x] 验证：本地玩家过滤、非本地不处理、监听方能在清理前读到事件。

## 验证与门禁

- [x] 新增 `Projects/test` 场景，DzApi 不在断言路径里。
- [x] `dotnet build Projects/test/test.csproj` 0 错误。
- [x] grep：控件几何写入只来自锚点批算，覆盖 `DzFrameSetPoint`、`DzFrameSetSize`、`DzFrameSetAbsolutePoint`、`DzFrameSetAllPoints`、`DzFrameSetScale`、`DzFrameClearAllPoints`。
- [x] grep：通用点击路径无 `DzFrameSetScript`；例外仅 EDITBOX/TEXTAREA。
- [x] grep：Frame 创建/销毁无 `HandleAdd/HandleRemove`。
- [x] 真实 War3 鼠标、IME、resize 未自动化。实现后的 `summary.md` 记录是否已手动执行。
- [x] 按 `R2` 复盘技术准确性与公共 API 兼容，不把 D 组脏投影混入本期。
