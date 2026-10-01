**状态**：已实施
**等级**：light（局部 UI 约定 + `UIManager` 小改）
**提案日期**：2026-09-24

## Why

War3 原生 Frame 层序等于创建序，且创建后无法重排（`DzFrameSetPriority` 不可用）。属性看板、天赋、商店等多面板同屏时，需要**稳定且可预测**的层序；运行期只应 `Show`/`Hide`，不应改变层序。当前 `UIManager` 没有层概念（`_panels` 是 `SortedDictionary<string, UIPanel>`，迭代顺序按名字而非创建序），`ui-control` 也只是把 `ZIndex` 定为创建序，缺少"面板按层创建"的约定与机制。若不补，使用者很可能在运行期尝试提层，造成命中层与视觉层分叉。

## What Changes

- 新增固定层桶 `UILayer`，顺序即层序：背景 → 面板 → 弹窗 → 模态 → 提示。
- `UIManager` 支持按 `UILayer` 注册面板；同一层内的先后由**注册顺序**固定。
- 明确 **SHALL NOT** 提供运行期提层 API（无 `Raise` / `BringToFront`）；`UIComponent.ZIndex` 保持创建期分配、`private set`。
- 同层面板若可能同时可见且重叠，其层序在**设计期**由注册顺序决定，运行期 `Show`/`Hide` 不改变层序。
- 不改动：控件不是 ECS 实体；`UIEvent` 仍是唯一 ECS 接缝；order 133/140 不变；不做有状态全局收进实例（A/B）的重构；不碰 D 组脏投影。

无 **BREAKING** 变更。

## Capabilities

### New Capabilities

<!-- 无新增能力，复用现有 ui-control。 -->

### Modified Capabilities

- `ui-control`: 新增"面板层序由创建期决定"与"UIManager 按层注册面板"两项 requirement。

## Impact

- `War3Frame/Src/Core/UI/UIManager.cs`：新增 `UILayer` 与按层注册；注册顺序参与层序。
- `War3Frame/Src/Core/UI/UIComponent.cs`：`ZIndex` 保持 `private set`，不新增提层方法。
- `War3Frame/Src/Systems/UI/`：不改 order，不新增系统。
- `Projects/test`：新增层序断言场景。
- 全局影响分析：`War3Frame.Generator/`（无新系统注册契约，不受影响）、`FrameBuild/`（构建编排不变）、`CSharpWar3Frame/`（CLI 不变）、`BridgeToJIT/`（不变）、`Projects/`（仅测试新增场景）。本变更只触及 UI 核心与测试，不涉及 ECS 状态、Native 同步层与生成器输出。
- 分级：`light`。复盘默认 `R0`。
