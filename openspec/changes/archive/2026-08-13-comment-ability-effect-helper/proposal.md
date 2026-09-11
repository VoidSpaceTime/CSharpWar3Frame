# 注释 AbilityEffectHelper 提案

## 0. 基本信息

- Change ID: `comment-ability-effect-helper`
- 提案等级: `fast`
- 目标一句话: 为 `AbilityEffectHelper` 补充必要中文注释，解释 ability 配置如何转换为运行时 effect entity。
- 请求来源: 用户要求解释 `AbilityEffectHelper`，并添加必要中文注释。

## 1. 分级判定

- 影响范围: 仅注释 `War3Frame/Src/Helpers/AbilityEffectHelper.cs`。
- 风险等级: 低；不改变代码行为、公共接口、数据结构、生成器或构建链路。
- 可逆性: 可删除新增注释快速回滚。
- 是否跨项目: 否。
- 是否改公共契约: 否。

## 2. 变更目标

在不改变任何执行逻辑的前提下，补充以下说明：

- `CreateEffectEntity` 是施法完成后的效果实例化入口。
- ability entity 上的旧 payload 与新 `EffectSpecData` 都会被复制/展开到 runtime effect entity。
- `EffectPending` 表示后续 `AbilityEffectSystems` 负责处理，不在 helper 内结算。
- `CreateChildEffect` 用于区域搜索命中目标后，为每个目标生成子 effect。
- `ApplyEffectSpec` 只做数据转换，不执行伤害、治疗、Buff 或 native 副作用。
- 弹道初始化只补运行时必需的 `Position` 和 `ProjectileRuntimeState`。

## 3. 影响文件

- `War3Frame/Src/Helpers/AbilityEffectHelper.cs`

## 4. 低风险理由

- 仅新增/调整中文注释。
- 不修改方法签名、组件字段、条件逻辑或系统注册。
- 不触碰 `Projects/`、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`。

## 5. 验证方式

- 读取文件确认注释落点清晰。
- 可选运行 `dotnet build War3Frame/War3Frame.csproj`，确认注释改动不影响编译。
