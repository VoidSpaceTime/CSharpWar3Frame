# Capability Spec：UIControlLayer（UI OOP 控件层）

## 能力描述

UI OOP 控件层提供基于对象树组织的 War3 Frame 封装：控件以父子树表达复杂面板结构，显隐与销毁级联递归；相对定位链可换算为屏幕绝对锚点缓存，供命中检测、Tooltip 定位与拖拽边界复用；控件交互事件经统一桥接分发并回灌为 `UIEvent` 事实组件进入 ECS；可阻挡控件登记全局阻挡池，统一判定鼠标安全区。所有原生 Frame 调用集中在 `FrameHelper` 封装，业务层不直接接触原生 API。

## Requirement

| ID | 需求 | 验证方式 |
|---|---|---|
| UI-01 | `UIComponent` 支持父子树结构（Children/Parent），父控件销毁递归销毁子树 | 场景验证 + 代码审查 |
| UI-02 | 控件显隐沿父子链级联（子可见 = 自身 Visible && 父可见链），隐藏子树不占鼠标事件资源 | 场景验证：父隐藏后子树 IsVisibleEffective 全 false |
| UI-03 | 现有控件子类（UIFrame/UIText/UITexture/UIBar/UIButton/UISlot）构造签名不破坏，接入树结构 | 构建验证 + 现有调用方 grep |
| UI-04 | 相对定位链可换算屏幕绝对锚点并缓存，命中检测基于锚点与可见链 | 场景验证：ResetAnchor 后 IsInside 与屏幕实际一致 |
| UI-05 | 控件事件（点击/悬停/拖拽）统一注册分发，enter/leave 不重不漏 | 场景验证：状态机边界断言 |
| UI-06 | UI 交互事件回灌为 `UIEvent` 事实组件（独立实体，多监听者，统一清理） | 场景验证：模拟点击产生 UIEvent 且类型正确 |
| UI-07 | War3 原生事件回调上下文不直接修改 ECS，经 UINativeSystem 转写 | 代码审查：回调内仅入队 |
| UI-08 | 可阻挡控件登记全局阻挡池，鼠标点击被挡时不穿透到游戏操作 | 场景验证：IsBlocked 命中 |
| UI-09 | 业务层（面板）不直接调用 Frame 原生 API，仅经控件方法 / FrameHelper | grep 审查：业务目录无原生 Frame 调用 |
| UI-10 | Tooltip 支持内容驱动布局与锚点定位（PlaceNear + 边缘翻转） | 场景验证（P1 断言） |

## 边界

- 控件树保持 OOP（不 ECS 化），符合 AGENTS.md OOP 倾向判定（强父子关系 + 生命周期绑定）。
- 不做 UIModel / UIAnimate（按需后续提案）。
- 不做 ESC 栈 / 自适应 / 组合控件演进（P2 可选阶段，不在首期 Requirement 内）。