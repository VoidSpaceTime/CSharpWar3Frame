# 设计：Effect 扩展 vertical slice 与执行边界

## 1. Context

`EffectChainBuilder` 已成为技能、物品和生命周期行为的统一效果链 authoring 入口。现有系统也已经形成大致分层：Builder 生成 `EffectSpec`，语义层解释 step，ECS 表达长期状态或请求，Native/Execution 层执行 War3 副作用。

现有 `formalize-ecs-native-effect-separation` 已规定：

- ECS 是长期 effect 语义的唯一真相。
- native handle 只是 execution-only resource。
- helper 不拥有长期真相。
- Projectile visuals 从 projectile ECS 状态派生。

本设计补充扩展准入：未来新增 Effect 能力必须如何穿过这些层，而不会再次产生平行 Builder 或 native bypass。

## 2. Goals / Non-Goals

### Goals

- 固定唯一公开 authoring Builder：`EffectChainBuilder`。
- 建立从 authoring 到验证的完整 vertical slice。
- 明确每层输入、输出和禁止职责。
- 允许没有 native 副作用的纯语义 effect 停在 ECS/outcome 层。
- 要求具体效果能力独立提案、独立验证和独立回滚。

### Non-Goals

- 不实现具体效果能力。
- 不强制所有 effect step 都有单独 Native system。
- 不把 Builder 变成依赖注入容器或 runtime dispatcher。
- 不在本 change 中迁移已有 helper/native ownership。

## 3. 全局影响分析

- `War3Frame/`: 本治理规格直接约束未来 Effect authoring API、spec payload、semantic resolver、ECS state/request/outcome 与 Native/Execution 实现；Phase 1 不修改运行时代码。
- `War3Frame.Generator/`: Phase 1 不受影响。未来具体 Effect change 若新增 `[SystemRegister]` 系统，必须验证生成注册、系统类型和执行顺序，并按其实际范围升级提案。
- `FrameBuild/`: Phase 1 不受影响。未来具体 Effect change 若引入资源生成、模板生成或发布资产约定，必须在独立 change 中说明构建链路影响。
- `CSharpWar3Frame/`: Phase 1 不受影响。未来具体 Effect change 若新增初始化参数、配置或 CLI 行为，必须独立审查入口和兼容性。
- `Projects/`: Phase 1 不修改示例。未来每个新增 Effect 能力至少提供一个真实可编译消费示例，并将只能在 War3 环境验证的行为列为手测项。

## 4. 推荐 vertical slice

### 4.1 Authoring 层

- 对普通模板作者，新增入口 MUST 添加到 `EffectChainBuilder`。
- Ability / Item / lifecycle API SHOULD 通过 lambda 传入同一个 Builder。
- SHALL NOT 为单一领域新增平行公开 Builder。
- Builder MUST 只构造数据，不读取 `EntityStore`、不创建 entity、不调用 native。

### 4.2 Spec 层

- 每个新 step MUST 有明确的 kind 与 payload。
- 必填字段、fallback、目标上下文和嵌套链行为 MUST 可从 spec 独立解释。
- 非法组合 SHOULD 在 Builder 阶段拒绝；无法在 Builder 阶段判断的约束 MUST 在 resolver 中给出显式 outcome，而不是静默忽略。

### 4.3 Semantic resolver 层

- resolver MUST 将 step 解释为业务结果、ECS request/command/state 或目标上下文变换。
- resolver SHALL NOT 直接调用 War3 native API。
- Area / Line 等上下文 step 与 Damage / Heal / Buff 等结算 step 的顺序语义 MUST 明确。
- 异步或延迟行为（如 Projectile arrive）MUST 保存足以重建后续效果链的 ECS 数据。

### 4.4 ECS truth 层

- 长期 attachment、lifetime、motion、appearance、animation request、owner/key cleanup 等 MUST 遵循 `ecs-native-effect-separation`。
- 一次性、无需长期语义的 effect 可以形成瞬时 request 或 outcome，但不能让 helper 成为隐藏 owner。

### 4.5 Native/Execution 层

- 只有需要 War3 副作用的能力才需要 Native/Execution 实现。
- 新 native 调用 MUST 位于 `Systems/Native/*`、`*NativeSystem` 或 `*ExecutionSystem`；例外 helper 必须满足瞬时、无长期语义且在具体提案中说明理由。
- Native/Execution MUST 消费 ECS truth / command，不得反向定义业务状态。
- 执行失败需要桥接回 ECS outcome 时，MUST 使用显式结果状态，不能把 native handle 状态当作唯一反馈。

### 4.6 验证层

- Builder/spec 测试：输入 authoring 调用，断言生成 payload 和嵌套关系。
- Resolver 测试：输入 spec/context，断言 ECS state/request/outcome。
- Native 边界测试或静态检查：非 Native 层无直接 native 调用。
- 集成示例：`Projects/test` 至少一个真实可编译消费入口。
- War3 手测：仅用于无法在纯 ECS/.NET 环境验证的表现和原生命令结果。

## 5. 依赖方向

允许：

```text
Template / Authoring
        -> EffectChainBuilder
        -> EffectSpec
        -> Semantic Resolver
        -> ECS State / Command / Request
        -> Native / Execution
        -> ECS Outcome（如需要）
```

禁止：

```text
Template -> Native API
Builder -> EntityStore / Native API
Business Resolver -> Native API
Native Handle -> Long-lived semantic truth
Domain-specific Builder -> duplicate Effect chain semantics
```

## 6. 新效果准入检查

每个具体效果提案必须回答：

1. 它是新的 step，还是现有 step 的参数扩展？
2. 为什么不能用现有 step 组合表达？
3. Builder API 是否保持领域中立？
4. Spec 如何表达合法性、fallback 和目标上下文？
5. resolver 产生什么 ECS state/request/outcome？
6. 是否需要 native 副作用？如果需要，为什么不能复用现有 Native/Execution 层？
7. 长期语义由哪个 ECS 组件持有？
8. 如何测试 Builder、resolver、Native 边界和集成消费？
9. 哪些行为必须在真实 War3 环境手测？

## 7. 候选方案与取舍

### 7.1 平行领域 Builder

拒绝。它会重新引入已删除的 Builder 包装层，并让 Item、Ability、Aura 对同一效果产生不同 API。

### 7.2 公开手工构造 `EffectSpec`

保留为高级入口，但不作为模板教学默认。手工构造无法集中维护结构合法性和 authoring ergonomics。

### 7.3 注册表/插件化 Effect handler

暂不采用。当前 step 数量与仓库规模尚不足以证明需要动态 handler registry；过早引入会增加初始化、生成器注册和调试复杂度。若未来 step 数量或第三方扩展需求显著增长，再另开架构提案。

### 7.4 `EffectChainBuilder` + 显式 switch/resolver vertical slice

当前推荐。保持编译期可发现性和现有代码风格，同时通过治理矩阵避免漏层。

## 8. 迁移路径

1. 批准治理规格，不改代码。
2. 建立现有效果覆盖矩阵：step -> builder -> spec -> resolver -> ECS/native -> tests/examples。
3. 将矩阵发现的缺口转化为独立 change，不在盘点任务中顺手修改。
4. 新效果能力从批准之日起必须遵循本规格。
5. 待 `formalize-ecs-native-effect-separation` 的相关阶段完成后，再评估是否清理旧 helper bypass。

## 9. 回滚策略

- 治理阶段回滚：删除本 change 工件即可，不触及运行时。
- 具体能力回滚：按其独立 change 恢复 Builder、spec、resolver、Native 和示例。
- 不允许通过保留第二公开 Builder 作为回滚手段；如确有外部兼容需求，应独立提案说明兼容期限和删除条件。

## 10. 长期维护影响

- 新效果开发成本略增，但漏掉 resolver、Native 边界或测试的概率下降。
- API 认知面稳定在 `EffectChainBuilder`，模板作者不需要学习领域专用 Builder。
- Native 副作用位置可审计，便于后续重放、测试和 War3 运行时诊断。
- 规格矩阵可作为 code review checklist，但不要求引入新基础设施或代码生成器。

## 11. Open Questions

- 现有效果覆盖矩阵最终放在 OpenSpec change 内，还是稳定后迁入长期架构文档？推荐先放在后续盘点 change，避免本治理提案记录易变实现状态。
- 当 step 数量显著增长时，是否从集中 resolver 转为静态 handler registry？当前不决定，待有实际规模证据再评估。
