# Effect vertical slice 覆盖矩阵

## 1. 盘点范围与判定口径

本矩阵记录 Phase 2 对现有 Effect 能力的只读盘点结果，不授权运行时代码修改。

覆盖层级：

1. Authoring：`EffectChainBuilder` 或领域 lambda 入口。
2. Spec：`EffectStepKind` 与对应 payload。
3. Resolver / ECS：语义解释、request/state/outcome 与生命周期。
4. Native / Execution：需要 War3 副作用时的执行归属。
5. Validation：builder/spec 测试、resolver/ECS 测试、集成示例、构建与真实 War3 手测。

状态口径：

- `Complete`：当前层存在明确实现和可核验证据。
- `Partial`：主路径存在，但有结构限制、兼容 truth 或验证缺口。
- `Missing`：没有对应实现或验证证据。

“总体状态”同时考虑实现和验证。因此即使运行时主链完整，只要缺少专门自动化测试或真实 War3 验证，仍记为 `Partial`。

## 2. 覆盖矩阵

| Capability | Authoring / Spec | Resolver / ECS | Native / Execution | Validation | 总体 |
| --- | --- | --- | --- | --- | --- |
| Damage | `Damage(...)`；`DamageEffectStepSpec` | `DamageEffectSystem` -> `DamageRequest` -> `DamageResolveSystem` / `DamageEvent`；结算后进入统一完成清理 | 不需要 Effect native 副作用 | `Projects/test` 有 Area、Projectile、Line 等组合示例；无 builder/spec 与 resolver 断言 | `Partial` |
| Heal | `Heal(...)`；`HealEffectStepSpec` | `HealEffectSystem` -> `HealRequest` -> `HealResolveSystem` / `HealEvent` | 不需要 Effect native 副作用 | 技能与物品 `UseEffect` 有示例；无自动化断言 | `Partial` |
| Buff | `Buff(...)`；`BuffEffectStepSpec` | `BuffEffectSystem` -> `BuffApplyRequest` -> `BuffApplyResolveSystem` / `BuffAppliedEvent` | 不需要 Effect native 副作用 | Frost Nova、Battle Shout 有示例；无自动化断言 | `Partial` |
| AreaSearch | `Area(...)`；`AreaSearchEffectStepSpec` | `AreaSearchSystem` 为命中目标创建子 effect，父 effect 完成；受 pending Projectile gate 约束 | 无 native 调用 | 多个技能有范围组合示例；无子 effect/目标上下文断言 | `Partial` |
| LineSearch | `Line(...)`；`LineSearchEffectStepSpec` | `LineSearchSystem` 创建子 effect，可发 `GroundAreaReactionRequest`；无 caster `Position` 时直接完成 | 无 native 调用 | Flamethrower 有示例；无搜索结果、reaction 或缺失位置断言 | `Partial` |
| GroundAreaCreate | `GroundArea(...)`；`GroundAreaCreateEffectStepSpec` | `GroundAreaCreateSystem` 创建 `GroundAreaData`、source、lifetime、`Position` 及可选 buff/damage/reaction 数据 | 无 native 调用 | Napalm Oil 与 Flamethrower 有组合示例；无持续区域 resolver 测试 | `Partial` |
| Projectile | `Projectile(...)`；`ProjectileEffectStepSpec` | `ProjectileSystem` + `ProjectileLifecycleApplySystem`；消费 arrival/expire request，并分别进入完成或通用 `EffectExpired` 清理 | Effect resolver 无 native 调用；视觉由独立 Effect Native 路径承担 | Lava Ball、Arcane Missile、Meteor Strike 有示例；无运动、到达、过期断言 | `Partial` |
| Projectile arrive | `OnProjectileArrive(...)` 绑定最近 Projectile；单个 `arriveEffect` 槽位 | arrival request 经 lifecycle apply 创建独立 arrive effect；到达主链完整 | 无 native 调用 | Lava Ball 与 Meteor Strike 有嵌套链示例；无非法绑定、到达派生和幂等断言 | `Partial` |
| EffectVisual | `Effect(...)` / `RemoveEffectByKey(...)`；`EffectVisualStepSpec`；支持视觉队列 | `EffectVisualSystem` 创建/删除视觉 ECS intent；持续状态由 effect components 持有 | `EffectNativeSystem` 统一执行创建、同步、动画和销毁 native effect | 点特效、目标附着、Projectile 到达视觉有示例；无 ECS/native 边界自动化测试和 War3 显示记录 | `Partial` |
| Lifecycle owner/key visual | `OnGranted(...)` + `OnRemoved(...)`，owner/key 成对维护 | owner/key 关系进入 ECS；移除通过 `RemoveByKey` / destroy request | 原生销毁由 `EffectNativeSystem` 执行 | Talent Wings 有示例；无授予/移除、handle 重建和清理断言 | `Partial` |
| Item `UseEffect` | `ItemSpecBuilder.UseEffect(...)` lambda / prebuilt `EffectSpec`；`ItemUseEffectData` 可被写入 | `Missing`：未发现消费 `ItemUseEffectData` 并创建/触发 Effect 的运行时系统 | 尚未进入执行层 | Amulet of Vigor 有 Heal authoring 示例，但不能作为可执行行为证据；无物品触发与 consumable 行为断言 | `Partial` |

## 3. Authoring 与 spec 结论

- 当前 8 个 `EffectStepKind` 均有公开 authoring 路径，没有发现无 Builder 入口的现有 kind，也没有发现公开 Builder 方法完全缺少 spec 表达。
- `AbilityValue` 直接覆盖 Damage、Heal、Buff、Projectile 的常用 authoring；其他值槽位通过到 `EffectValueSpec` 的隐式转换使用。
- `EffectVisual` 明确支持多个视觉 step 队列。
- Damage、Heal、Buff、AreaSearch、LineSearch、GroundAreaCreate 和 Projectile 等非视觉 payload 当前以单组件形态展开；重复同类 step 通常表现为覆盖，而不是多实例队列。该限制没有形成统一公开基数契约。
- Projectile 只有一个 `arriveEffect` 槽位；`OnProjectileArrive(...)` 必须紧跟 Projectile，并以最后一次配置为准。

## 4. Resolver、ECS 与 Native 边界结论

- `AbilityEffectHelper.ApplyEffectSpec` 负责把 step 展开为 ECS payload；实际结算由各 resolver 系统、组件 gate 和系统 order 推进。
- Damage、Heal、Buff、AreaSearch、LineSearch、GroundAreaCreate 和 Projectile 主流程都有对应 resolver 与生命周期收口。
- Effect 相关非 Native helper/system 中未发现 `JassApi`、`KKApi`、`YDApi` 或 `DzApi` 直调；视觉 native 调用集中在 `Systems/Native/EffectNativeSystem.cs`。
- `EffectHelper` 主要创建 ECS intent、dirty 标记和销毁请求，没有持有长期 native truth。
- Projectile arrival 与 expiry request 都由 `ProjectileLifecycleApplySystem` 消费；expiry 进入 `ProjectileLifecyclePhase.Expired` 并产生通用 `EffectExpired`。当前功能表面仍存在差异：arrival 支持 hook 与派生 `arriveEffect`，expiry 没有对应 hook、专用终态 tag 或派生效果链，但这属于待评估的能力边界，不是未消费 bug。
- legacy `ProjectileHookBridge` 会构造临时 `ProjectileBase`，并把 hook 修改后的 speed、target 等字段写回 ECS。这证明存在兼容 mutation surface，但不足以证明 `ProjectileBase` 持有并行的长期 truth；是否迁移或退役需要独立审查。
- Item `UseEffect` 当前存在 Builder/spec 写入但缺少运行时消费者，是明确的 authorable-but-not-executable 缺口。

## 5. 验证与消费证据

### 5.1 已有证据

- `Projects/test/Scripts/Template/Ability.cs` 覆盖 Damage、Heal、Buff、Area、Line、GroundArea、Projectile、Projectile arrive、EffectVisual 和 owner/key 生命周期视觉。
- `Projects/test/Scripts/Template/Item.cs` 覆盖 item `UseEffect`。
- 当前会话曾执行完整构建（属于会话命令输出，不是持久化 CI 或测试证据）：
  - `dotnet build War3Frame/War3Frame.csproj`：0 warning，0 error。
  - `dotnet build Projects/test/test.csproj`：0 warning，0 error。
- 相关 OpenSpec 已定义 Effect visual、Projectile arrive、settlement、ECS/native ownership 与 vertical-slice 验收规则。

### 5.2 缺失证据

- 仓库未发现 xUnit、NUnit、MSTest 或 `Microsoft.NET.Test.Sdk` 测试项目。
- 未发现专门的 Builder/spec 结构断言。
- 未发现 resolver/ECS state/request/outcome 自动化断言。
- 未发现 native handle 丢失重建、owner/key cleanup、Projectile expiry 对称性等自动化验证。
- 未发现真实 War3 环境下的视觉显示、附着点、Projectile 到达/过期和清理记录。

### 5.3 不能计为 Effect 验证的内容

- `Projects/test/Program.cs` 是运行时 smoke 入口，不是 Effect 行为测试。
- `Projects/demo` 当前只有 timer smoke，空模板不能作为 Effect 消费证据。
- `ScrollFireballTemplate.UseAbility(...)` 只引用技能，不是 item `UseEffect` 证据。
- 属性贡献模板不属于 Effect step 覆盖。

## 6. 缺口优先级

### 高优先级

1. Item `UseEffect` 可声明但不可执行：`ItemUseEffectData` 没有运行时消费者，违反“authorable capabilities 必须具有 executable semantics”的治理要求。
2. 自动化 vertical-slice 测试缺失：现有能力主要只有示例和构建证据，无法验证 spec 结构、resolver outcome 和生命周期幂等。

### 中优先级

3. Projectile terminal 与 legacy compatibility 边界需审查：arrival/expiry 的公开能力表面不同，legacy bridge 允许 hook 修改后回写 ECS，但当前证据不足以预设删除或迁移结论。
4. 重复 step 基数语义不明确：视觉支持队列，其他同类 step 多为覆盖，模板作者无法从公共契约判断哪些重复步骤合法。
5. 真实 War3 验证记录缺失：视觉模型、挂点、native handle 重建和清理只能通过真实宿主确认。

### 低优先级

6. `Projects/demo` 缺少 Effect 示例；当前 `Projects/test` 已满足至少一个消费项目的治理要求，因此不是阻塞项。

## 7. 后续独立 OpenSpec 建议

以下 change-id 仅作为拆分建议，不代表已获实现批准：

1. `implement-item-use-effect-runtime`（建议 `full`）
   - 定义物品使用请求、目标上下文、Effect entity 创建与 consumable/stack 结算顺序。
   - 消费现有 `ItemUseEffectData`，并复用统一 Effect 解释管线。
   - 添加物品使用成功、失败、重复触发和消耗行为验证。

2. `add-effect-vertical-slice-tests`（建议 `full`）
   - 建立测试项目或测试基础设施。
   - 覆盖 Builder/spec、resolver/ECS、生命周期和错误组合。
   - 优先覆盖 Damage、Heal、Buff、Area/Line 子 effect、Projectile arrive/expiry、EffectVisual owner/key。

3. `review-projectile-terminal-semantics`（建议 `full`）
   - 记录 arrival/expiry 当前 request、phase、tag、hook 和派生效果链能力。
   - 决定 expiry 是否需要专用 hook、终态 tag 或派生效果链；不预设必须与 arrival 完全对称。
   - 无论是否扩展能力，都补充完成、过期、清理和幂等验证。

4. `review-legacy-projectile-hook-compatibility`（建议 `full`；若决定移除或重划 ownership 再升级为 `architecture`）
   - 盘点 `ProjectileHookBridge` / `ProjectileBase` 的兼容职责。
   - 评估临时对象和回写 mutation surface 是否造成实际语义冲突。
   - 先形成保留、约束或迁移结论，不预设退役。

5. `formalize-effect-step-cardinality`（建议 `full`）
   - 为每种 step 定义 single、last-wins、queue 或 reject 的公共契约。
   - 决定非视觉重复 step 是否需要多 payload 数据结构。
   - 增加非法/重复组合测试。

6. `validate-effect-runtime-in-war3`（建议 `light`；若引入自动化宿主或构建链则升级）
   - 记录点特效、附着特效、owner/key cleanup、Projectile arrival/expiry 的真实 War3 验证。
   - 明确模型资源、挂点字符串和 native handle 重建的剩余风险。

## 8. Phase 2 结论

- 现有 Effect authoring 与运行时主链可继续作为开发基线。
- Native/Execution 分层当前未发现直接违规。
- 现有能力的总体覆盖均为 `Partial`，原因主要是验证证据不足，而不是全部运行时缺失。
- 后续不得在本 change 中顺手修复上述缺口；应从第 7 节选择独立 change，完成提案审核后再实施。
