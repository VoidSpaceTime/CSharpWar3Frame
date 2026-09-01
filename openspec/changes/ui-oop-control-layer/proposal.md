# 提案：UI OOP 控件层（控件树 + 锚点缓存 + 事件桥接）

**状态**：待审核
**等级**：full（跨 UI 控件层/ECS 事件桥接/业务面板多区域 + 新增公共 authoring API，用户确认按 full 审核）
**提案日期**：2026-08-31
**请求来源**：lik/xlik 框架 UI 帧体系（`class/ui/*`：类原型 + 单例工厂 + 继承链 construct/destruct + 双图结构 + 锚点缓存 + 事件桥接 + 全局注册表）对标分析；用户确认 UI 走 OOP 组件模式（AGENTS.md 明确 UI 属 OOP 倾向域：强父子关系 + 生命周期绑定）。

---

## 修订说明（2026-09-01）

基于 opus5 审查结果的关键修订：

1. **Show/Hide 虚方法决策**：`UIComponent.Show(bool)` 与 `Hide()` 保持非虚方法，级联逻辑在基类实现；新增 `protected virtual ShowInternal(bool)` 供子类扩展显隐行为。UIButton 和 UISlot 当前未重载 Show，无需迁移。
2. **P0/P1 边界调整**：鼠标事件注册/注销移出 P0，归入 P1 的 `UIEventBridge`；P0 仅覆盖控件树、锚点缓存与阻挡池，可独立验收。
3. **GetFrameRect 决策**：`FrameHelper` 现无 `GetFrameRect` 方法，DzApi 也无对应原生 API。P0 仅缓存 `SetAbsPos` 定位控件的锚点；相对 `SetPoint` 链换算推迟到 P1 或后续阶段。
4. **HandleAdd/HandleRemove 明确**：War3 Frame 当前不在 `HandleHelper` 登记范围内（仅 Unit/Effect 等游戏对象）；UI Frame 生命周期由 `FrameHelper.Destroy` 管理，显式排除出句柄引用计数系统。
5. **UIEvent 锁步规则**：UI 事件为本地玩家事件，`UIEvent.playerIndex` 是触发玩家；业务监听系统必须过滤 `playerIndex == GetLocalPlayer()`；跨玩家同步属业务层责任，不由 UI 层处理。
6. **uiKeyHash vs Entity**：采用 `Entity` 引用标识控件（内存控件直接引用），不使用 `uiKeyHash`；同步/回放标识如需要则由业务层单独设计。
7. **TriggerEventMarker 与清理契约**：`UIEvent` 作为独立事件实体必须挂载 `TriggerEventMarker`；监听系统 order 必须 < 132（`EventCleanupSystem` order 132 统一清理）。

---

## 背景

现有 UI 能力（`War3Frame/Src/Core/UI/`）已具备正确的基础形态：

| 现有件 | 形态 | 缺口（对照 xlik） |
|---|---|---|
| `UIComponent` 基类 + 子类（UIFrame/UIText/UITexture/UIBar/UIButton/UISlot/UITooltip） | 控件封装，原生调用全部走 `FrameHelper` | **无控件树**：无父子集合、无级联显隐、无递归销毁；无法表达复杂面板结构 |
| `UIPanel` + `UIManager` | 面板生命周期 + 注册表 + RefreshAll | **无事件桥接**：控件点击/悬停/拖拽无统一分发与命中检测；UI 事件未回灌 ECS |
| `FrameHelper` | Frame 原生封装（创建/位置/文本/贴图/事件） | **无锚点缓存**：命中检测/Tooltip 定位/拖拽边界每次现算；**无阻挡池**：UI 遮挡鼠标操作无法统一判定 |

业务面板（背包、技能栏、属性面板）已有雏形，但控件组合、交互、遮挡判定均需手写，无法复用。

## 目标

1. **控件树（P0）**：`UIComponent` 增强为树节点（Children 集合 + 父引用），显隐/销毁级联递归，生命周期走继承链（construct/destruct 约定，同 xlik）；`Show(bool)` 保持非虚，新增 `protected virtual ShowInternal(bool)` 供子类扩展。
2. **锚点缓存（P0）**：`ResetAnchor()` 换算 `SetAbsPos` 定位控件的屏幕绝对矩形 `{x,y,w,h}` 缓存，供命中检测、Tooltip 定位、拖拽边界复用；相对 `SetPoint` 链换算推迟到 P1 或后续（DzApi 无 `GetFrameRect` 原生方法）。
3. **阻挡池（P0）**：可阻挡控件登记全局阻挡池，鼠标安全区判定；P0 可独立验收，不依赖事件桥接。
4. **事件桥接（P1）**：控件事件（左/右键点击、悬停进出、拖拽开始/结束）统一注册分发 + 命中检测 + enter/leave 状态机；**转发为 `UIEvent` 事实组件回灌 ECS**（业务系统可监听，不直接持有控件回调）。
5. **UIEvent 规则（P1）**：`UIEvent` 为独立事件实体，必须挂载 `TriggerEventMarker`，监听系统 order < 132；`playerIndex` 为触发玩家，业务层必须过滤本地玩家事件；控件标识用 `Entity` 引用，不用 `uiKeyHash`。
6. **Tooltip 增强（P1）**：现有 Tooltip 增强为内容驱动布局。
7. **分层边界固化**：OOP 控件层为语义层（持有 handle 的薄入口），原生 Frame 调用全部集中在 `FrameHelper` 封装；业务面板从 ECS 读数据刷新；UI 交互事件经 `UIEvent` 进入 ECS；War3 Frame 不纳入 `HandleHelper` 句柄引用计数（显式排除）。

## 非目标

- 不做 ECS 化 UI 树（控件树保持 OOP，符合 AGENTS.md OOP 倾向判定）。
- 不做 3D 模型控件（UIModel）、贴图序列动画（UIAnimate）——按需后续提案。
- 不改 `FrameHelper` 的现有公共签名（只在其上补充封装）。
- 不迁移现有 `UIPanel`/`UIManager` 公共 API（`UIPanel` 子类化继承增强）。

## 影响范围

- `War3Frame/Src/Core/UI/UIComponent.cs`：增强为树节点（Children/Parent/级联 Show/Destroy/生命周期钩子）；`Show(bool)` 保持非虚，新增 `protected virtual ShowInternal(bool)`。
- `War3Frame/Src/Core/UI/`：新增 `UIAnchor.cs`（锚点缓存，P0 仅缓存 SetAbsPos 定位控件）、`UIBlock.cs`（阻挡控件）、`UIBlockPool.cs`（阻挡池）、`UIEventBridge.cs`（P1：事件桥接 + 命中检测 + 状态机）、`UITooltip` 增强（P1：内容驱动布局）。
- `War3Frame/Src/Components/UI/UIEvent.cs`（新目录，P1）：`UIEvent` 事实组件（playerIndex/controlEntity/type/x/y，挂载 `TriggerEventMarker`）+ `UIEventType` 枚举。
- `War3Frame/Src/Systems/UI/UINativeSystem.cs`（新，P1）：消费 UI 原生事件队列回灌 `UIEvent` 实体（ECS tick 内转写，回调上下文不改 ECS）；order < 132。
- `Projects/test`：新增 `UIPanelValidationScenario`（构建面板 + 事件断言，P1 验证）。
- 不受影响区域：
  - `War3Frame.Generator/`：无生成器契约变化（控件是 OOP 类，无需生成注册）。
  - `FrameBuild/`、`CSharpWar3Frame/`、`BridgeToJIT/`、`FastMDX/`、`ModelFormat/`：不涉及。
  - 现有 `UIPanel` 子类（如有）：公共 API 不变，可渐进迁移。
  - `HandleHelper`：War3 Frame 不纳入句柄引用计数，生命周期由 `FrameHelper.Destroy` 管理。

## 方案摘要

```
业务面板（UIPanel 子类）构建 OOP 控件树（UIComponent 增强：Children + 相对定位 + 锚点缓存）
        ↓
控件事件 → UIEventBridge（命中检测 + enter/leave 状态机）→ UIEvent 事实组件（ECS 实体）
        ↓
业务系统监听 UIEvent（如点击按钮 → 生成 XxxRequest）；UINativeSystem 负责原生侧调度
        ↓
原生 Frame 调用：全部集中 FrameHelper（控件层薄入口，禁止业务层直调）
```

详见 `design.md`。

## 风险与回滚

- 风险：
  1. **控件树改造影响现有控件子类**：`UIComponent` 基类改动（构造/销毁链），需逐个核对 UIFrame/UIText/UIBar/UIButton/UISlot 子类；改造以"新增字段 + 保留现有构造签名"方式兼容。UIButton 和 UISlot 当前未重载 `Show`，无迁移负担。
  2. **锚点缓存局限性**：P0 仅缓存 `SetAbsPos` 定位控件（DzApi 无 `GetFrameRect` 原生方法）；相对 `SetPoint` 链换算推迟到 P1，P0 阶段命中检测仅覆盖绝对定位控件。
  3. **事件桥接命中检测性能（P1）**：每帧鼠标移动对可见控件树做命中遍历；以锚点缓存 + 可见性过滤控制成本，规模小（<数百控件）无压力。
  4. **UIEvent 回灌时序（P1）**：原生 UI 事件在 War3 事件回调上下文触发，与 ECS tick 同步需经 UINativeSystem 转写（order < 132），避免业务回调内直接改 ECS。业务监听系统必须过滤 `playerIndex == GetLocalPlayer()`，跨玩家同步属业务层责任。
- 回滚：控件树增强向后兼容（现有构造签名不变）；新增文件可独立移除；`UIEvent` 组件新增无破坏；War3 Frame 显式排除出 `HandleHelper`，无引用计数副作用。

## 验收标准

1. `dotnet build War3Frame/War3Frame.csproj` 0 错误。
2. **P0 控件树**：父控件 Show(false) 级联隐藏子树；Destroy 递归销毁子树；现有 UIFrame/UIText/UITexture/UIBar/UIButton/UISlot 构造调用不破坏；`ShowInternal` 虚钩子可被子类重载。
3. **P0 锚点缓存**：`ResetAnchor()` 后 `SetAbsPos` 定位控件的命中检测结果与屏幕实际位置一致（面板验证场景断言）；相对定位控件锚点缓存标记为 P1。
4. **P0 阻挡池**：登记控件遮挡时鼠标点击不穿透到游戏操作（安全区判定生效）。
5. **P1 事件桥接**：点击/悬停产生 `UIEvent` 实体（含 playerIndex/controlEntity/type/x/y + `TriggerEventMarker`），业务系统可监听；enter/leave 不重不漏；监听系统 order < 132。
6. **P1 UIEvent 锁步**：业务监听系统过滤 `playerIndex == GetLocalPlayer()` 生效；跨玩家同步由业务层处理，UI 层不强制同步。
7. `Projects/test` 面板验证场景通过 `Require` 断言（P1 完整验证）。
8. War3 Frame 未在 `HandleHelper` 登记（grep 审查 HandleAdd/HandleRemove 不涉及 Frame）。

## 分级判定

- 影响范围：`War3Frame/Src/Core/UI/` 核心改造 + 新增组件/系统区域 + 新增公共 API。
- 风险等级：中（基类改造兼容性 + 事件时序）。
- 是否跨项目：否（Projects 仅加验证场景）。
- 是否改公共契约：`UIComponent` 基类新增成员（向后兼容）；新增 `UIEvent` 组件与 Builder/桥接 API。
- 按 AGENTS.md，新增公共 authoring API + 跨多区域 → `full`。工件齐全：`proposal.md` + `design.md` + `tasks.md` + `specs/ui-control.md`。