## 0. 基本信息

- Change ID: `simplify-inline-item-ability-authoring`
- 提案等级: `light`
- 目标一句话: 删除 inline 物品 Ability 的 `InlineAbilitySpecCanonicalizer`(指纹/深拷贝/反射 schema 校验),改为按名 first-wins 幂等 + 直接共享只读 spec,消除每个物品实例创建时的重复规范化开销。
- 请求来源: 用户认为 `InlineAbilitySpecCanonicalizer` 过度设计、暂时用不到,应优先做框架主体功能;并确认保留 inline authoring 功能本身,只删过度设计。
- 默认实施后审查强度: `R0 Direct`
- 命中的审查升级触发器: 含多步骤技术推理与"共享只读 spec 是否安全"这一技术准确性判断 → 升级 `R1 Focused`
- 最终实施后审查强度: `R1 Focused`
- Oracle 可用性与 `R1` 回退方式: 优先用 Oracle 复核"共享 spec 只读安全 + 按名 first-wins 幂等正确性";Oracle 不可用时,以对全部 spec 运行时消费点的静态引用审查作为等价回退并记录证据与 verdict。
- 完整 `review-work` 授权来源: 无(未启用完整 `review-work`)

### 0.1 工件矩阵

- 本变更为 `light`,必须 `proposal.md`;因涉及既有 capability 的局部规格约束与多步骤,补充 `design.md`、`tasks.md`、`specs/inline-item-ability-authoring/spec.md`(delta)。

## 1. 分级判定

### 1.1 为什么是这个等级

- 影响范围:仅 `War3Frame` 单模块,2 个 `internal` 文件(`InlineItemAbilityTemplate.cs`、`AbilityTemplateAttribute.cs`)。
- 风险等级:低。删除的是 authoring 期防御性校验;合法用法的可观测行为与运行时路径不变。
- 可逆性:高。`git revert` 即可恢复;回滚不涉及数据或契约迁移。
- 是否跨项目:否。`Projects/test` 仅需保持编译与场景通过,不改测试预期行为。
- 是否改公共契约:否。改动的 `InlineAbilitySpecCanonicalizer`、`RegisterInlineItemAbility`、`InlineItemAbilityTemplate` 全部 `internal`;三种 `UseAbility` 公共写法与 Item runtime 契约(`ItemSpec`/`ItemUseAbilityData` 仅存 template name)不变。

### 1.2 升级触发器检查

- [ ] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [ ] 涉及 `Projects/` 示例或集成验证行为(仅需保持编译/场景通过,合法行为不变)
- [ ] 涉及公共 API / 数据结构 / 配置契约(改动均为 `internal`)
- [ ] 涉及架构边界、目录结构、依赖关系重组

未命中 `full`/`architecture` 触发器。改动会放宽既有 capability 的 spec 内部校验类 MUST,因此仍产出 spec delta,但不改变对外可观测契约,判定维持 `light`。

### 1.3 实施后审查升级触发器

- [ ] 公共 API、生成器输出、配置格式、构建链或发布契约
- [ ] 持久化、迁移、数据兼容性或数据丢失风险
- [ ] 性能回归、资源泄漏、实时性或大规模数据影响(本变更为性能改善方向,非回归)
- [ ] 多系统、多项目或跨边界状态协作

未命中 `R2`/`R3` 强制项。因含"共享只读 spec 安全性"这一技术准确性判断,按 `light` 复杂/技术事实不确定规则升级至 `R1 Focused`。

### 1.4 工具授权与回退

- [x] `R1` 优先使用 Oracle;不可用时以 spec 运行时消费点静态引用审查作为等价回退并记录证据与 verdict。
- [x] 未因任何原因自动启用完整 `review-work`。

## 2. 背景与目标(LIGHT）

### 2.1 背景

`InlineAbilitySpecCanonicalizer`(约 800 行)为 inline 物品 Ability 提供:反射 schema 一致性校验、递归结构校验、深拷贝快照、SHA-256 规范指纹、UTF-8/嵌套/数量上限、循环引用检测。这套设计针对"不可信/跨进程 spec 漂移"威胁模型,但本框架 inline Ability 完全由开发者在 **进程内、编译期确定** 的 template 代码里 authoring,不存在该威胁。

更关键的是执行频率问题:`ItemTemplate.Apply(name, item)` 对 **每个物品实例** 调用 `template.Configure(item)`,而 inline `UseAbility(...)` 把注册塞在 `Configure` 内。因此每 spawn 一个 inline 物品,都会执行:
- `RegisterInlineItemAbility` → `CreateSnapshotAndFingerprint`:反射校验约 15 个结构体 + 全量深拷贝 + SHA-256(算完发现指纹一致 → no-op);
- companion 实体创建时 `InlineItemAbilityTemplate.Configure` → `CloneSnapshot`:再做一次反射 + 深拷贝。

即每个 inline 物品实例白烧 2 次深拷贝 + 2 次反射 schema 校验 + 1 次 SHA-256,全部只为防一个不存在的场景。

### 2.2 目标

- 删除 `InlineAbilitySpecCanonicalizer` 及其指纹/深拷贝/反射校验。
- `InlineItemAbilityTemplate` 直接持有并共享只读 `AbilitySpec`,`Configure` 不再克隆。
- `RegisterInlineItemAbility` 改为按内部 template name **first-wins** 幂等:首次注册胜出,后续同名调用直接复用。
- 保留低成本且仍有意义的保护:保留前缀拒绝(`Register` 已实现)、生成名被非 inline 模板占用时明确失败、同一 Builder 只能配置一次 `UseAbility`、companion 生命周期禁用 `OnGranted`/`OnRemoved`(`ValidateFullInlineBehaviors`,与本 canonicalizer 无关,保留)。

## 3. 影响范围

- 模块:`War3Frame`(`Src/TemplateInit`)。
- 文件:
  - 删除 `War3Frame/Src/TemplateInit/InlineItemAbilityTemplate.cs` 中的 `InlineAbilitySpecCanonicalizer`;精简 `InlineItemAbilityTemplate`(去 `Fingerprint`、去克隆)。
  - 改 `War3Frame/Src/TemplateInit/AbilityTemplateAttribute.cs`:`RegisterInlineItemAbility` 改 first-wins;`NormalizeInlineOwner` 去 `ValidateString`;删 `_inlineTemplateCount`、`MaxInlineItemAbilityRegistrations`。
- 不受影响区域:
  - `War3Frame.Generator/`:inline 注册为 authoring 期命令式调用,不涉及生成器发现/输出。
  - `FrameBuild/`、`CSharpWar3Frame/`:不涉及构建、发布、CLI、配置。
  - `Projects/`:三种 `UseAbility` 写法与既有测试模板保持编译与合法行为;仅移除对"注册期强校验失败"的隐含依赖(现有测试模板均为合法 spec,不触发这些失败分支)。

## 4. 方案摘要

1. 删除 `InlineAbilitySpecCanonicalizer` 整个静态类及内部 `CanonicalHashWriter`、`ValidationState`。
2. `InlineItemAbilityTemplate`:移除 `Fingerprint` 属性与构造参数;字段 `_spec` 直接保存注册时传入的 spec;`Configure` 改为 `AbilitySpecBuilder.Apply(entity, level, _spec)`(共享只读引用,不克隆)。
3. `RegisterInlineItemAbility`:
   - 保留 `NormalizeInlineOwner` + `spec.templateName == templateName` 校验;
   - 单锁内:若 `templateName` 已存在且为 `InlineItemAbilityTemplate` → 直接返回(first-wins);若被非 inline 模板占用 → 抛错;否则 `_templates.Add(templateName, new InlineItemAbilityTemplate(owner, spec))` 并返回。
4. `NormalizeInlineOwner`:保留非空/trim/长度上限检查,移除 `InlineAbilitySpecCanonicalizer.ValidateString` 调用。
5. 删除 `_inlineTemplateCount` 与 `MaxInlineItemAbilityRegistrations`(inline template 集合由编译期物品模板数天然有界)。

## 5. 风险与回滚

- 风险:去掉深拷贝后多个 companion 共享同一 `AbilitySpec` 对象;若未来出现运行时写回 `spec.behaviors`/`baseValues`/`EffectSpec` 集合的代码,将造成实例间串改。**当前已核实全部运行时消费点只读**(`AbilitySpecBuilder.Apply` 只读并 Resolve;`AbilityLevelStatRebuildSystem` 只读 Resolve;EffectSpec swap 只重指组件指针不改集合),`R1` 复核将再确认此点。缓解:如后续需要可变副本,应在写入方按需 copy-on-write,而非在 authoring 期无条件深拷贝。
- 风险:first-wins 放弃"同 owner 不同 spec 明确失败"。该场景仅在物品模板 `Configure` 非确定性(代码 bug)时出现;确定性模板每次产出相同 spec,first-wins 语义等价。
- 风险:删除资源上限/UTF-8/循环引用校验。inline spec 由开发者编译期 authoring,规模有界、无不可信输入;`EffectChainBuilder`/`AbilitySpecBuilder` 正常用法不构造循环。
- 回滚:`git revert` 恢复两个文件即可,无数据/契约迁移。

## 6. 验收标准

- `War3Frame/War3Frame.csproj` 与 `Projects/test/test.csproj` Release 构建通过。
- 三种 `UseAbility` 写法仍编译;`inline_healing_charm`、`inline_point_scroll` 及 companion 验证场景运行 0 failures。
- 同一 inline 物品模板多次实例化复用同一 internal template;不同物品模板名称隔离;生成名被非 inline 模板占用时仍明确失败。
- 每个物品实例仍创建独立 companion,冷却与施法状态不共享。
- 静态扫描确认 `InlineAbilitySpecCanonicalizer`、`Fingerprint`、`_inlineTemplateCount`、`MaxInlineItemAbilityRegistrations` 引用已全部移除,且未恢复任何 Item 直接 Effect 路径。

## 7. 实施前置

本 `proposal.md` 及 `design.md`、`tasks.md`、`specs/inline-item-ability-authoring/spec.md`(delta)经用户审核批准后,才能修改代码。
