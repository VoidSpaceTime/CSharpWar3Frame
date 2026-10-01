# 实施总结：UI OOP 控件层

- **变更 ID**：`ui-oop-control-layer`
- **等级**：`full`（复盘强度 `R2 Targeted`，实施者复核，未另开独立审查代理）
- **状态**：`已实施`（2026-09-24）
- **决策来源**：用户批准提案；Momus 第二轮 accept。控件保持 OOP，不进 ECS。

## 1. 实际改动

控件树、锚点、命中和 `UIEvent` 落在 `War3Frame` 与 `Projects/test`。D 组（`RefreshAll` 替换、技能/物品/Buff UI 投影）未做。

| 区域 | 改动 |
|---|---|
| `War3Frame/Src/Core/UI/` | `UIComponent` 树、非虚 `Destroy`/`Show`、`UIAnchor`、`UILayout`、`UIHitRegistry`、`UIInputQueue`、`UINative`、`UIRuntime` |
| `War3Frame/Src/Components/UI/UIEvent.cs` | `PlayerIndex/ControlId/Type/X/Y`，不挂 `TriggerEventMarker` |
| `War3Frame/Src/Systems/UI/UIInputSystem.cs` | order 133 派发，140 清理；鼠标回调只入队 |
| `War3Frame/initialization/War3Init.cs` | 启动后 `NativeReady=true`，并注册一次全局鼠标 |
| `Projects/test/.../UIControlValidationScenario.cs` | 树、相对命中、resize、阻挡、捕获、贴边、事件窗口 |

`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/` 无契约改动。既有 `SystemKind.Interval` 最高仍是 132（`EventCleanupSystem`），没有为 UI 改 order。

## 2. 行为

- 几何只记在 `UIAnchor`。`FlushLayout` 先消费 `ResizeDirty` 再按深度批算；`NativeReady` 时才 `ClearPoints` + `SetSize` + `SetAbsPos`。
- 命中扫已注册可见控件，不用父矩形剪掉落在外面的相对子控件。`CompareZ` 取胜者；任一已注册命中且 `BlocksLower=true` 则 `WorldClickSwallowed`。
- 按下捕获并发 `DragStart`，松开发 `DragStop` 并清捕获。`Enter/Leave` 只在胜者变化时发出，捕获期间不切到其他控件。
- `UIEvent` 在 133 创建、140 删除。测试桥接放在二者之间，只计数，不改 0–132 的系统。面板开关与页签没有建 `UIEvent` 的路径。
- 根参考是 `{X:0.4, Y:0.3, W:0.8, H:0.6}`。`UINative` 不能当锚点参考，也不进命中集。

## 3. 验证

- `dotnet build War3Frame/War3Frame.csproj`：0 错误（经 Regression 工程连带编译）。
- `dotnet build Projects/test/test.csproj`：0 错误。存量警告在 `SkillPointUpgradeScenario` 与 `Program.cs`，与本次无关。
- `War3Frame.Regression.dll existing`：**Executed 12; passed 12; failed 0**，含 `UIControlValidationScenario`。
- grep：控件侧 `SetSize` / `ClearPoints` / `SetAbsPos` 只在 `UIComponent.Project`。`FillParent`、`SetScale`、`SetPoint(int,int,...)` 无控件调用方。`DzFrameSetScript` 仍只留在 `FrameHelper`，控件点击路径无调用方。UI 创建/销毁无 `HandleAdd` / `HandleRemove`。

## 4. R2 复核

**技术准确性：通过，带残留。** 相对命中曾被父矩形提前剪掉，已改为扫注册表并用 `InTree` 过滤。resize 标记曾只在 `Drain` 清除，测试直接调用 `FlushLayout` 时清不掉；现由 `ConsumeResize()` 在刷新时消费。空 `QuerySystem<UIEvent>` 的 `OnUpdate` 已由回归实际跑通，不再停留在文档推断。

**公共 API：通过，带兼容缺口。** `UIFrame/UIText/UITexture/UIBar/UIButton/UISlot` 构造签名未改。`Show` 仍非虚。`Destroy` 去掉 `virtual`，三个子类 override 已删。旧 `SetPoint(int,int,int,float,float)` 保留签名但只标脏，不再写 Frame。`FrameHelper.SetPriority` / `OnClick` 是原有方法，本期没有新包装，也没有控件再调用。

D 组未混入。`UIPanel` / `UIManager.RefreshAll` 签名未改。

## 5. 未执行与剩余

- 真实 War3 鼠标、IME、窗口 resize **未手动执行**。无客户端自动化协议。鼠标按钮码仍按左 1 / 右 2、按下 1 / 松开 0，未对照 `common.j`。
- 没有生产用的 134–139 桥接系统。测试桥只验证窗口和本地玩家过滤，不写具体 `XxxRequest`。面板命令映射不在本期。
- `FrameHelper` 仍保留 `DzFrameSetScript` 与 `SetPriority` 入口，供 EDITBOX/TEXTAREA 等例外；代码未把事件类型收窄到这两类。
- `WorldClickSwallowed` 只发布标记，不改单位选择。
- 未归档。需要用户确认后再 `openspec archive`。
