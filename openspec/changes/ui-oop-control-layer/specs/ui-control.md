# Capability Spec：UIControlLayer（UI OOP 控件层）

## 能力描述

UI OOP 控件层提供基于对象树组织的 War3 Frame 封装：控件以父子树表达复杂面板结构，显隐与销毁级联递归；`Show(bool)` 保持非虚方法实现级联，新增 `ShowInternal(bool)` 虚钩子供子类扩展；锚点缓存优先支持 `SetAbsPos` 定位控件（P0），相对 `SetPoint` 链换算推迟到 P1；控件交互事件经统一桥接分发并回灌为 `UIEvent` 事实组件进入 ECS（挂载 `TriggerEventMarker`，监听系统 order < 132）；可阻挡控件登记全局阻挡池，统一判定鼠标安全区；所有原生 Frame 调用集中在 `FrameHelper` 封装，业务层不直接接触原生 API；War3 Frame 显式排除出 `HandleHelper` 句柄引用计数系统。

## Requirement

| ID | 需求 | 验证方式 |
|---|---|---|
| UI-01 | `UIComponent` 支持父子树结构（Children/Parent），父控件销毁递归销毁子树 | 场景验证 + 代码审查 |
| UI-02 | 控件显隐沿父子链级联（子可见 = 自身 Visible && 父可见链），隐藏子树不占鼠标事件资源（P1）；`Show(bool)` 保持非虚，新增 `ShowInternal(bool)` 虚钩子供子类扩展 | 场景验证：父隐藏后子树 IsVisibleEffective 全 false；子类可重载 ShowInternal |
| UI-03 | 现有控件子类（UIFrame/UIText/UITexture/UIBar/UIButton/UISlot）构造签名不破坏，接入树结构；UIButton/UISlot 当前未重载 Show，无迁移任务 | 构建验证 + 现有调用方 grep |
| UI-04 | P0 锚点缓存支持 `SetAbsPos` 定位控件，P1 扩展 `SetPoint` 链换算；命中检测基于锚点与可见链 | 场景验证：P0 SetAbsPos 定位控件 ResetAnchor 后 IsInside 与屏幕实际一致；P1 SetPoint 链定位控件命中生效 |
| UI-05 | 控件事件（点击/悬停/拖拽）统一注册分发，enter/leave 不重不漏（P1） | 场景验证：状态机边界断言 |
| UI-06 | UI 交互事件回灌为 `UIEvent` 事实组件（独立实体，挂载 `TriggerEventMarker`，多监听者，监听系统 order < 132，统一清理）；`playerIndex` 为触发玩家，业务层过滤本地玩家事件；控件标识用 `Entity` 引用 | 场景验证：模拟点击产生 UIEvent 且类型正确，挂载 TriggerEventMarker；业务监听系统过滤 playerIndex == GetLocalPlayer() |
| UI-07 | War3 原生事件回调上下文不直接修改 ECS，经 UINativeSystem（order < 132）转写 | 代码审查：回调内仅入队 |
| UI-08 | 可阻挡控件登记全局阻挡池，鼠标点击被挡时不穿透到游戏操作 | 场景验证：IsBlocked 命中 |
| UI-09 | 业务层（面板）不直接调用 Frame 原生 API，仅经控件方法 / FrameHelper | grep 审查：业务目录无原生 Frame 调用 |
| UI-10 | Tooltip 支持内容驱动布局与锚点定位（PlaceNear + 边缘翻转） | 场景验证（P1 断言） |
| UI-11 | War3 Frame 显式排除出 `HandleHelper` 句柄引用计数，创建/销毁点无 HandleAdd/HandleRemove 调用 | grep 审查：Frame 创建/销毁点无 HandleAdd/HandleRemove |

## 边界

- 控件树保持 OOP（不 ECS 化），符合 AGENTS.md OOP 倾向判定（强父子关系 + 生命周期绑定）。
- 不做 UIModel / UIAnimate（按需后续提案）。
- 不做 ESC 栈 / 自适应 / 组合控件演进（P2 可选阶段，不在首期 Requirement 内）。
- P0 锚点缓存仅支持 `SetAbsPos` 定位控件（DzApi 无 GetFrameRect 原生方法），`SetPoint` 链换算推迟 P1。