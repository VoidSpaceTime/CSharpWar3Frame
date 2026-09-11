## Why

当前仓库已经出现了“移动只是某个上层动作的一个阶段”这一正确方向，但移动主语义仍然容易被施法流程绑架。例如 `MoveToCastSystem` 已经体现了“为了施法而移动”的场景，但如果继续沿着这个方向扩展，移动将被隐式定义为施法子系统的一部分，而不是一个可复用的命令/执行/监控框架。

真实项目中，单位移动不仅服务于施法，还服务于：

- 预设任务
- 交互
- 跟随
- 巡逻
- AI 行为
- 自定义脚本流程

同时，Warcraft 原生命令又允许玩家通过新命令、Stop、Hold Position 等方式覆盖当前移动。如果不把“命令发布”“native 执行”“结果监控”“覆盖/中断语义”从施法系统里抽离出来，移动与行动流程会长期处于 patchwork 状态。

## What Changes

- 将 move architecture 正式定义为独立子系统。
- 明确 ECS 负责发布命令与监控执行，native 负责执行实际移动命令。
- 定义 move command ownership、execution state、outcome、continuation 与 override/interruption 语义。
- 明确施法系统、预设任务系统、AI 系统等只是 move 的调用方，而不是 move owner。

## Capabilities

### New Capabilities
- `independent-move-command-architecture`: 定义独立的移动命令发布、native 执行、结果监控与 continuation 语义。

### Modified Capabilities
- `unit-lifecycle-a-plan-architecture`

## Impact

- 直接影响 `War3Frame` 的移动、施法前置移动、预设任务链、AI 行为与命令覆盖语义。
- `War3Frame.Generator` 预期无直接行为变更，但后续实现要确认生成/注册逻辑不依赖旧 `MoveToCast` 风格耦合。
- `FrameBuild` 预期无直接行为变更，但后续实现要确认构建与运行时初始化不依赖旧移动 ownership。
- `CSharpWar3Frame` 预期无直接行为变更，但后续实现要确认 CLI/tooling 不依赖旧移动语义。
- `Projects/*` 后续实现后必须验证施法前移动、任务链移动、玩家覆盖命令、Stop/Hold 与到达事件行为。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。
