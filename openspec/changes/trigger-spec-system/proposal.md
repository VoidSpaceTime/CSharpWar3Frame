# 提案：触发器体系 TriggerSpec（ECS 规则实体 + 动作注册表）

**状态**：待审核
**等级**：full（新增公共 authoring API + 跨 Components/Systems/Helpers 多区域 + 事件实体契约小改，符合 full 判定）
**提案日期**：2026-08-31
**请求来源**：lik/xlik 框架事件体系（`event.lua`/`eventKind.lua`，100+ 事件常量与对象域注册）对标分析；用户确认规则载体采用 **A 方案（ECS 实体挂 TriggerSpec 组件）**，并确认配套 Helper + System 分工。

---

## 背景

仓库已具备"事实事件实体"模式（`DamageEvent`/`HealEvent`/`BuffAppliedEvent`，独立实体、只读、多监听者），技能内部生命周期触发已有 `AbilityBehaviorTrigger`（OnEffect/OnChannelTick/OnInterrupted/OnFinished/OnGranted/OnRemoved）。但**跨领域的规则系统尚未落地**：血量阈值、事件组合、条件筛选、一次性/冷却/次数策略这些"规则"能力散落在各业务系统里，无法声明式配置。

`AGENTS.md` 已定义触发器设计原则（规则 = 匹配事件 + 条件 + 策略 + 动作；触发只产生已有 `XxxRequest`；不建万能 TriggerBus；Friflo 原生结构事件不承担游戏触发器职责），但尚无对应提案与代码。

## 目标

1. 新增 `TriggerSpec` 数据组件（IComponent，挂独立触发器实体）：EventMatcher + Conditions[All/Any/Not] + TriggerPolicy[一次性/冷却/次数/优先级] + Actions。
2. 新增 `TriggerSpecBuilder`/`TriggerHelper`：链式配置 + 快捷创建触发器实体（写 ECS 意图，同 `EffectChainBuilder` 族）。
3. 新增 `TriggerSystem`：统一扫描事件实体 → 匹配规则 → 判定条件 → 策略消耗（冷却计时/次数递减/一次性移除，状态存 `TriggerRuntime` 组件）→ 执行动作。
4. 新增 `TriggerConditionRegistry`/`TriggerActionRegistry`：复杂条件/动作注册表扩展（同 `EffectFormulaRegistry` 形态）。
5. 动作只生成已有 `XxxRequest`（DamageRequest/BuffApplyRequest/CastRequest/HealRequest），不得在触发器回调内直接调用 War3 原生 API。

## 非目标

- **不建万能 TriggerBus**：技能内部生命周期保持 `AbilityBehaviorTrigger`，内部计时保持 `TimerTask`，动作 DSL 保持 `EffectChainBuilder`。
- **不把 Friflo 原生结构变化事件当触发器**：区域进入、血量阈值等状态条件仍由空间/状态系统检测生成领域事件（AGENTS.md #26）。
- 不改变现有 `DamageEvent` 等事件实体的创建语义与命名；事件实体仍由统一清理系统删除，TriggerSystem 只读。

## 影响范围

- 新增：`War3Frame/Src/Components/Trigger/TriggerSpec.cs`（TriggerSpec/TriggerRuntime/TriggerEventTag 等）、`War3Frame/Src/Systems/Trigger/TriggerSystem.cs`、`War3Frame/Src/Helpers/TriggerHelper.cs`、`TriggerSpecBuilder.cs`、`TriggerConditionRegistry.cs`、`TriggerActionRegistry.cs`、`TriggerEventType.cs`。
- 小改：`War3Frame/Src/Components/Damage.cs`、`Settlement.cs`（事件实体创建处补挂 `TriggerEventTag`，涉及 `AbilityEffectSystems.cs` 中 3 个结算系统）。
- 验证：`Projects/test/Scripts/Process/` 新增 `TriggerValidationScenario.cs`。
- 不受影响区域：
  - `War3Frame.Generator/`：TriggerSystem 用现有 `[SystemRegister]`，无生成器契约变化。
  - `FrameBuild/`、`CSharpWar3Frame/`：构建链与 CLI 不涉及。
  - `BridgeToJIT/`、`FastMDX/`、`ModelFormat/`：不涉及。
  - 现有 `AbilityBehaviorTrigger`/`TimerTask`/`EffectChainBuilder`：不动。

## 方案摘要

```
业务系统创建事件实体（DamageEvent 等）+ 挂 TriggerEventTag
        ↓
TriggerHelper.Register(entity, TriggerSpecBuilder...Build())  → 触发器实体（TriggerSpec + TriggerRuntime）
        ↓
TriggerSystem（结算后 order）：
  事件实体 × 触发器规则 → EventMatcher 匹配 → Conditions(All/Any/Not) 判定
  → TriggerPolicy 消耗（Once/Cooldown/Count，写 TriggerRuntime）
  → TriggerActionRegistry 动作 → 生成已有 XxxRequest → 现有 ResolveSystem 执行
        ↓
事件实体由统一清理系统删除（TriggerSystem 不删）
```

详见 `design.md`。

## 风险与回滚

- 风险：
  1. **事件实体契约小改**：现有 3 个结算系统创建事件处需补挂 `TriggerEventTag`；逐点核对，改动极小。
  2. **匹配性能**：规则数 × 事件数 的全量匹配在极端规模下可能成为热点；design.md 中按 eventTypeId 分组索引缓解，先不做复杂索引（可后续升级）。
  3. **规则与事件的耦合**：事件实体若被领域系统提前删除会漏触发；明确"事件实体由统一清理系统删除"纪律，TriggerSystem order 需在清理前。
- 回滚：全部为新增文件 + 3 处 Tag 挂载点，`git revert` 可完整回退；不动现有公开 API。

## 验收标准

1. `dotnet build War3Frame/War3Frame.csproj` 0 错误。
2. `TriggerValidationScenario`：注册"单位死亡→BuffApplyRequest"与"伤害值>阈值→DamageRequest"两类规则，验证触发正确、重复触发按策略（Once/Cooldown/Count）收敛。
3. Conditions 的 All/Any/Not 组合判定正确。
4. 动作经注册表生成 Request，无任何 War3 原生调用出现在触发器回调路径。
5. 事件实体不因触发被删除（清理纪律保持）。

## 分级判定

- 影响范围：`War3Frame` 内新增 3 个区域（Components/Systems/Helpers）+ 事件实体契约小改 + 新增公共 authoring API。
- 风险等级：中（新机制，性能与生命周期纪律需验证）。
- 是否跨项目：否（Projects 仅加验证场景）。
- 是否改公共契约：新增 API（无破坏）；`TriggerEventTag` 挂载属内部契约。
- 按 AGENTS.md，新增公共 authoring API + 跨模块 → 至少 `full`。工件齐全：`proposal.md` + `design.md` + `tasks.md` + `specs/trigger-spec.md`。