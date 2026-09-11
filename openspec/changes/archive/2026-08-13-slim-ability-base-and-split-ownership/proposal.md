## Why

当前 `AbilityBase` 在仓库中的角色已经失衡。它一方面承担“技能身份层”的职责，另一方面又被周边系统当作数值桶、运行时状态桶和施法过程容器来读取。这使得 `AbilityBase` 同时混入了四类完全不同的 ownership：

- 技能身份与基础语义
- 可修正的能力数值
- 运行时过程状态
- 技能行为结构细节

这种设计会让 ability architecture 长期停留在“大杂烩基类”状态，并且阻碍此前已经确认的方向：

- ability numeric values 逐步统一到 attribute/value/modifier 模型
- cooldown/channel 等运行时过程状态收拢为专门 state 组件
- projectile / area / damage / buff / targeting 结构保持组件化

因此，需要正式收敛 `AbilityBase` 的职责，让它只保留“技能身份层 / 最小公共语义层”，并把数值、状态和行为结构拆回各自的 canonical owner。

## What Changes

- 正式定义 `AbilityBase` 仅拥有最小公共语义。
- 规定 `AbilityBase` 的白名单字段与黑名单字段。
- 规定 ability numeric values 由 `AbilityAttribute` ownership 持有。
- 规定 cooldown / channel / charges 等过程值由 runtime state 组件持有。
- 规定 projectile / area search / damage / heal / buff / targeting 结构由独立行为组件持有。

## Capabilities

### New Capabilities
- `ability-base-identity-ownership`: 定义 `AbilityBase` 作为技能身份层的最小 ownership 边界。

### Modified Capabilities
- `ability-numeric-attribute-ownership`

## Impact

- 直接影响 `War3Frame` 的 ability core architecture、casting path、UI/slot 读取路径与 template authoring 边界。
- `War3Frame.Generator` 需要确认能力模板注册、生成代码与 `AbilityBase` 瘦身后的接口边界兼容。
- `FrameBuild` 需要确认构建编排与模板处理不依赖旧 `AbilityBase` 大杂烩字段。
- `CSharpWar3Frame` 需要确认 CLI / tooling / 初始化入口不依赖旧 `AbilityBase` 字段布局。
- `Projects/*` 在实现后必须重新验证技能模板、施法流程、冷却显示与 UI 读取行为。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。
