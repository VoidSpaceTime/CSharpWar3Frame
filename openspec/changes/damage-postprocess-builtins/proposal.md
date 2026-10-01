# 提案：伤害后处理内置（吸血 / 反射 / 护盾吸收）

## Why

`DamagePostProcessRegistry` 已是伤害落地后的唯一扩展点，但静态列表为空，吸血、反射和伤害护盾都没有消费方。`DamageContext.absorbed` 与 `finalDamage` 注释已预留“含护盾吸收后剩余”，实际结算仍把计算器输出整段扣进 `Health`。现在补内置处理器，避免后续技能把这三类特例写进 `DamageResolveSystem`。

## What Changes

- 在 `DamagePostProcessRegistry` 按固定顺序注册三个内置处理器：伤害护盾吸收、吸血、反射。注册仍走现有 `Register`，不在 `DamageResolveSystem` 内联业务分支。
- 护盾是扣血前吸收：读取目标 `DamageShield` 的 `current`，吸收 `min(current, 待扣伤害)`，回写剩余护盾，并把 `ctx.absorbed` / `ctx.finalDamage` 交给同一次 Apply。这是对现有“钩子只读 ctx”约束的一处显式收窄，只允许护盾处理器在 Apply 前改这两个字段。
- 吸血与反射在 `DamageEvent` 之后、死亡判定之前执行，只发新的 `HealRequest` / `DamageRequest`，不改本次 `finalDamage`，不调用 War3 原生 API。
- 数值来源是单位属性最终值或资源当前值，不用技能表或硬编码常量：`DamageShield`（资源池）、`Lifesteal`（攻击者，0..1）、`DamageReflect`（受击者，0..1）。
- 互斥写死：无敌不吸收、不吸血、不反射；护盾吸收后的实际扣血为 0 时不吸血、不反射；反射伤害不再进入后处理，避免互反射循环。
- 反射请求带 `skipPostProcess`。`DamageEvent` 追加只读的 `absorbed`，既有 `finalDamage` 改为实际扣血。

## Capabilities

### New Capabilities

- `damage-postprocess`: 内置伤害护盾吸收、吸血与反射的数值来源、结算顺序、互斥，以及不得把业务特例写入伤害主流程的约束。

### Modified Capabilities

- 无。`openspec/specs/` 当前没有伤害管线 capability；`damage-resolution-pipeline` 的“钩子不注册内置实现”只存在于未合并进主 specs 的归档 delta，本变更不修改任何现有 requirement。

## Impact

- `War3Frame/`：`DamagePostProcessRegistry`、`DamageResolveSystem` 的调用时序、`DamageContext` / `DamageRequest` / `DamageEvent`、`AttributeHelper` 新增三个属性 ID。生命仍由 `ModifyCurrent` 写入，原生同步继续走 `UnitNativeSystem` 的 Compare-Sync。
- `Projects/`：`DamagePipelineValidationScenario` 增补具名断言（护盾数值、吸血、反射、无敌与零扣血互斥、反射不再后处理）。不改 demo。
- `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`：不受影响。不新增系统、不改生成器契约、不改构建或 CLI。

## 分级与审查

- 等级：`full`。公共结算契约与跨系统协作（伤害 → 治疗 / 反射请求 → 死亡），但目录、依赖和生成器边界不变，不升 `architecture`。
- 状态：待审核。未批准前不实现。
- 复盘：`R2 Targeted`（结算契约 + 跨系统协作）。未授权完整 `review-work`。
- 工件：`proposal.md`、`design.md`、`tasks.md`、`specs/damage-postprocess/spec.md`。实施后写完整 `summary.md`。
