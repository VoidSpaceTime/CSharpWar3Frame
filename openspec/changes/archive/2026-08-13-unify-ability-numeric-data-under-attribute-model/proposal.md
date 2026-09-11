## Why

当前仓库对 ability numeric data 存在重复建模：一方面单位侧已有较完整的 attribute/value/modifier 模型，另一方面技能侧又维护了独立的 ability-stat ownership 与若干直接写在行为组件中的数值字段。这会带来两类问题：

- 同样属于“可被等级、buff、装备、天赋修正的数值”，却需要两套心智模型
- ability runtime reader 很容易长期处于“有的数值读组件字段，有的数值读 ability-stat，有的数值未来再读 attribute”的混合状态

与此同时，能力模板本身已经天然具备 level-aware authoring 语义，而单位/物品模板并不共享这一语义。最优结构不是把三类模板强行统一成同一个签名，而是：

- ability numeric values 统一收敛到 attribute/value/modifier 模型
- ability behavior/structure fields 继续保留为普通 ECS 组件
- ability templates 保持 `Configure(Entity entity, int level)`
- unit/item templates 不被强迫引入 ability-like level semantics

## What Changes

- 将 ability numeric values 统一迁移到 attribute/value/modifier 模型。
- 将 mana cost、cooldown、cast range、damage、heal、radius、projectile speed、duration 等数值视为 ability-owned attributes。
- 保留 targeting、projectile kind、search/filter、damage type、damage source、model path 等结构字段为普通组件。
- 保持 ability template 为 level-aware authoring 接口。
- 明确 unit/item templates 不因 ability 设计而被强制引入 level 参数。

## Capabilities

### New Capabilities
- `ability-numeric-attribute-ownership`: 定义 ability numeric ownership 与 behavior structure ownership 的边界。

### Modified Capabilities
- `repository-governance`

## Impact

- 直接影响 `War3Frame` 的 ability template authoring、runtime numeric read path、modifier pipeline 与技能数值建模方式。
- `War3Frame.Generator` 需要确认模板注册/生成逻辑不会被新的接口边界破坏。
- `FrameBuild` 需要确认构建编排与资源/模板生成链不依赖旧的 ability numeric placement。
- `CSharpWar3Frame` 需要确认 CLI / tooling / 初始化入口不依赖旧能力模板数值结构。
- `Projects/*` 后续实现后必须验证现有技能模板、demo 内容、测试内容与新数值结构兼容。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。
