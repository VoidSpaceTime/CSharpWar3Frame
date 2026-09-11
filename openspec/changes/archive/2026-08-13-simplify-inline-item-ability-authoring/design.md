# 设计说明

## 为什么 canonicalizer 非承重

`InlineAbilitySpecCanonicalizer` 做四件事,逐一评估其在本框架的必要性:

| 职责 | 目的 | 本框架是否需要 |
| --- | --- | --- |
| 反射 schema 一致性(`EnsureSupportedSchema`) | 结构体新增字段时强制同步序列化逻辑 | 不需要。仅为服务指纹序列化而存在;指纹删除后失去意义。 |
| 深拷贝(`CloneAbilitySpec`/`CloneSnapshot`) | 隔离实例间可变集合 | 不需要。运行时对 spec 集合只读(见下)。 |
| SHA-256 规范指纹(`CanonicalHashWriter`) | 检测"同 owner 不同 spec" | 不需要。确定性模板每次产出相同 spec;差异只在代码 bug 时出现。 |
| 上限/UTF-8/循环引用校验 | 防不可信/超大/恶意 spec | 不需要。inline spec 由开发者编译期 authoring,无不可信输入,builder 正常用法不构造循环。 |

## 共享只读 spec 的安全性论证(R1 复核焦点)

去掉深拷贝后,同一 inline 物品模板的所有 companion 实例共享 `RegisterInlineItemAbility` 时传入的同一 `AbilitySpec` 对象。安全的前提是:运行时从不写回该 spec 的引用型成员(`behaviors`、`baseValues`、各级 `EffectSpec.steps`、`parameters`)。已核实的消费点:

- `AbilitySpecBuilder.Apply(ability, level, spec)`:读 `spec.baseValues` 并 `Resolve(level)` 成 float 写入组件;把 `spec`、`spec.behaviors`、`OnEffect` 的 `EffectSpec` 按引用挂到组件上,**不修改集合内容**。
- `AbilityLevelStatRebuildSystem`(`LevelExperienceSystem.cs`):只读 `specData.spec.baseValues` 并 Resolve,不写回。
- EffectSpec "swap"(`AbilityEffectHelper` 的 `GetCurrentEffectSpec`/`RestoreEffectSpec`):只改 `EffectSpecData` 组件指向的 `EffectSpec` 引用,**不 mutate `behaviors`/`steps` 集合本身**。

结论:spec 在运行时为只读,共享安全。companion 之间真正需要隔离的是冷却与施法状态,而这些是每个 companion 实体各自的组件(`AbilityBase`、cooldown 组件等),与共享 spec 无关。

若未来引入写回 spec 集合的运行时逻辑,正确做法是在写入方按需 copy-on-write,而不是恢复 authoring 期无条件深拷贝。

## first-wins 幂等语义

内部 template name 由 `NormalizeInlineOwner(itemTemplateName)` 加保留前缀确定,同一物品模板每次实例化都产生同一 `templateName`。`RegisterInlineItemAbility` 在单锁内:

1. `templateName` 已存在且为 `InlineItemAbilityTemplate` → 直接返回该名(first-wins,后续实例 no-op)。
2. `templateName` 已存在但为非 inline 模板 → 抛错(保留前缀已由 `Register` 拒绝普通注册占用,此处为纵深防御)。
3. 不存在 → `Add` 新 `InlineItemAbilityTemplate(owner, spec)` 并返回。

放弃原"同 owner 不同指纹 → 失败":该失败只可能由物品模板 `Configure` 非确定性触发,属代码 bug,不值得每实例一次 SHA-256 去探测。

## 每实例开销变化

改动后,inline 物品每实例仍会执行 `Configure` → builder 构造一个 `AbilitySpec`(分配),但 `RegisterInlineItemAbility` 在名字已存在时立即返回,新建的 spec 被丢弃由 GC 回收。相比之前"2 次深拷贝 + 2 次反射 + 1 次 SHA-256",降为"1 次 builder 分配 + 字典查一次"。

进一步在 builder 层"名字已存在就跳过 spec 构造"可完全省掉重复分配,但会把去重逻辑下沉到 builder、且改变 authoring lambda 是否每实例求值的语义,超出本次最小范围,记为后续可选优化,不在本提案实施。
