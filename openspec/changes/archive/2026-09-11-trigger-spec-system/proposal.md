# 提案：触发器体系 TriggerSpec（ECS 规则实体 + 动作注册表）

**状态**：已批准（用户 2026-08-31 确认落地；含 grok-4.6 验证修订：order 重排、事件清理交付、标记组件、参数模型、条件组合、动作挂载）→ 已实施（2026-08-31，见 summary.md）
**等级**：full（新增公共 authoring API + 跨 Components/Systems/Helpers 多区域 + 事件实体契约小改，符合 full 判定）
**提案日期**：2026-08-31
**请求来源**：lik/xlik 框架事件体系（`event.lua`/`eventKind.lua`，100+ 事件常量与对象域注册）对标分析；用户确认规则载体采用 **A 方案（ECS 实体挂 TriggerSpec 组件）**，并确认配套 Helper + System 分工。

---

## 背景

仓库已具备"事实事件实体"模式（`DamageEvent`/`HealEvent`/`BuffAppliedEvent`，独立实体、只读、多监听者），技能内部生命周期触发已有 `AbilityBehaviorTrigger`（OnEffect/OnChannelTick/OnInterrupted/OnFinished/OnGranted/OnRemoved）。但**跨领域的规则系统尚未落地**：血量阈值、事件组合、条件筛选、一次性/冷却/次数策略这些"规则"能力散落在各业务系统里，无法声明式配置。

`AGENTS.md` 已定义触发器设计原则（规则 = 匹配事件 + 条件 + 策略 + 动作；触发只产生已有 `XxxRequest`；不建万能 TriggerBus；Friflo 原生结构事件不承担游戏触发器职责），但尚无对应提案与代码。

**grok-4.6 验证结论（2026-08-31）**：原草案存在 6 处需修订——order 128 与 GroundArea 系统撞车；全仓无统一事件清理（`DamageEvent` 等创建后永不删除）；`TriggerEventTag` 命名违规（Tag 名含 Event）；动作参数 `Dictionary<string,float>` 装不下 buffId 字符串；All/Any/Not 扁平数组语义不闭合；`CastRequest` 挂单位主体而非独立实体。本提案已按修订结论更新。

## 目标

1. 新增 `TriggerSpec` 数据组件（IComponent，挂独立触发器实体）：eventTypeId + Conditions（单根组合 All/Any + 叶子 not）+ TriggerPolicy[Once/Cooldown/Count] + Actions。
2. 新增 `TriggerEventMarker` 标记组件（IComponent，带 eventTypeId）：事件创建点挂载，TriggerSystem 单查询发现全部事件实体。
3. 新增 `EventCleanupSystem`（order 132）：事件实体的帧边界消费窗口清理（**本提案交付物**，解决全仓事件泄漏）。
4. 新增 `TriggerSystem`（order 131）：扫描事件实体 → 按 eventTypeId 匹配规则 → 判定条件 → 策略消耗（冷却/次数/一次性，状态存 `TriggerRuntime`）→ 执行动作。
5. 新增 `TriggerConditionRegistry`/`TriggerActionRegistry`：复杂条件/动作注册表扩展（同 `EffectFormulaRegistry` 形态）；动作参数三通道（float/string/Entity）。
6. 新增 `TriggerSpecBuilder`/`TriggerHelper`：链式配置 + 快捷创建触发器实体（写 ECS 意图，同 `EffectChainBuilder` 族）。
7. 动作只生成已有 `XxxRequest`（DamageRequest/BuffApplyRequest/CastRequest/HealRequest），**CastRequest 挂单位主体**（`unit.AddComponent<CastRequest>()`）；不得在触发器回调内直接调用 War3 原生 API。

## 非目标

- **不建万能 TriggerBus**：技能内部生命周期保持 `AbilityBehaviorTrigger`，内部计时保持 `TimerTask`，动作 DSL 保持 `EffectChainBuilder`。
- **不把 Friflo 原生结构变化事件当触发器**：区域进入、血量阈值等状态条件仍由空间/状态系统检测生成领域事件（AGENTS.md #26）。
- 不改变现有 `DamageEvent` 等事件实体的创建语义与命名；事件实体生命周期交给 `EventCleanupSystem`（本提案交付），TriggerSystem 只读。
- 不做嵌套条件树（首期单根组合）；不做时间窗口/事件序列匹配（CEP 扩展留后续）。
- 不做 `odds` 随机条件（仓库无同步随机源，随机条件会导致锁步分叉）。

## 影响范围

- 新增：`War3Frame/Src/Components/Trigger/`（TriggerSpec/TriggerCondition/TriggerAction/TriggerPolicy/TriggerRuntime/TriggerEventMarker/EventTypeRegistry/TriggerContext）、`War3Frame/Src/Systems/Trigger/`（TriggerSystem/EventCleanupSystem）、`War3Frame/Src/Helpers/Trigger/`（TriggerHelper/TriggerSpecBuilder/TriggerConditionRegistry/TriggerActionRegistry）。
- 小改：事件创建点挂 `TriggerEventMarker`——`AbilityEffectSystems.cs`（DamageEvent/HealEvent/BuffAppliedEvent 创建处，3 处）+ `ControlStateTransitionSystem.cs`（ControlStateChangedEvent 创建处，1 处）。
- 验证：`Projects/test/Scripts/Process/` 新增 `TriggerValidationScenario.cs`。
- 不受影响区域：
  - `War3Frame.Generator/`：新系统用现有 `[SystemRegister]`，无生成器契约变化。
  - `FrameBuild/`、`CSharpWar3Frame/`：构建链与 CLI 不涉及。
  - `BridgeToJIT/`、`FastMDX/`、`ModelFormat/`：不涉及。
  - 现有 `AbilityBehaviorTrigger`/`TimerTask`/`EffectChainBuilder`：不动。

## 方案摘要

```
业务系统创建事件实体（DamageEvent 等）→ 创建处挂 TriggerEventMarker（eventTypeId）
        ↓
TriggerHelper.Register(TriggerSpecBuilder...Build())  → 触发器实体（TriggerSpec + TriggerRuntime）
        ↓
TriggerSystem（Interval 131，晚于 GroundArea 128/129/130 与 EffectLifecycle 130）：
  Query<TriggerEventMarker> 事件实体 × 规则索引（按 eventTypeId 分组）
  → 条件判定（单根组合 All/Any + 叶子 not，注册表扩展）
  → TriggerPolicy 消耗（Once/Cooldown/Count，写 TriggerRuntime）
  → TriggerActionRegistry 动作 → 生成已有 XxxRequest → 现有 ResolveSystem 执行
        ↓
EventCleanupSystem（Interval 132）：删除已过消费窗口的事件实体（本提案交付物）
```

详见 `design.md`。

## 风险与回滚

- 风险：
  1. **事件实体契约小改**：4 处事件创建点需补挂 `TriggerEventMarker`；逐点核对，改动极小。
  2. **匹配性能**：规则数 × 事件数的全量匹配在极端规模下可能成为热点；design.md 中按 eventTypeId 分组索引缓解（`Dictionary<int, List<Entity>>`），先不做复杂索引。
  3. **事件清理时序**：EventCleanupSystem 必须严格晚于 TriggerSystem 与所有事件监听系统（order 132）；若有系统 order 落在 131-132 之间监听事件，需在实施时核对。
  4. **确定性**：条件/动作注册表禁止使用非确定性源（`Random.Shared`、时间差异）；首期无随机条件。
- 回滚：全部为新增文件 + 4 处标记挂载点，`git revert` 可完整回退；不动现有公开 API。

## 验收标准

1. `dotnet build War3Frame/War3Frame.csproj` 0 错误。
2. `TriggerValidationScenario`：注册"伤害值 > 阈值 → DamageRequest 加深"与"→ BuffApplyRequest"规则，验证触发正确、重复触发按策略（Once/Count）收敛。
3. Conditions 的 All/Any + 叶子 not 组合判定正确。
4. 动作经注册表生成 Request（DamageRequest/BuffApplyRequest 独立实体；CastRequest 挂单位主体），无任何 War3 原生调用出现在触发器回调路径。
5. 事件实体由 `EventCleanupSystem` 在消费窗口后删除（不再泄漏）；TriggerSystem 不删事件实体。
6. 新事件类型（如 `ControlStateChangedEvent`）登记后无需改 TriggerSystem 即可被匹配。

## 分级判定

- 影响范围：`War3Frame` 内新增 3 个区域（Components/Systems/Helpers）+ 事件实体契约小改 + 新增公共 authoring API。
- 风险等级：中（新机制，性能与生命周期纪律需验证）。
- 是否跨项目：否（Projects 仅加验证场景）。
- 是否改公共契约：新增 API（无破坏）；`TriggerEventMarker` 挂载属内部契约。
- 按 AGENTS.md，新增公共 authoring API + 跨模块 → 至少 `full`。工件齐全：`proposal.md` + `design.md` + `tasks.md` + `specs/trigger-spec.md`。