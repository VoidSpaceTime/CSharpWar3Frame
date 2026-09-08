# 设计：施法打断 + 控制校验 + 目标合法性二次确认

> 对应 change：`cast-interrupt-and-validation`（light）。本文件给出时序选型、语义决策与实现点。

## 1. 现状时序图（核实）

```
CastRequestSystem(order3/Immediate, ITimed 0.02s)
  └─ Process: [无控制校验] → 范围内? StartCasting → CastState(Casting,timer=castTime)
                             范围外? CastState(MovingToCast)+MoveCommand+MoveContinuation
CastingSystem(order20/Interval, tick 0.05s)
  └─ 每 tick: Has<CastInterruptedTag> → InterruptCast (但无人 AddTag)
              Casting timer→0 → TryCommitEffect(CheckCost+ApplyCost+OnEffect) → Channeling 或 Backswing
MoveToCastSystem(order4/Immediate)  已在 L201 IsControlled 检查（仅移动路径）
ControlStateTransitionSystem(order46/Interval) → 发 ControlStateChangedEvent + ControlStateNativeRequest
```

## 2. 选型：打断接线方式

### 方案 A：事件驱动（监听 ControlStateChangedEvent）
- 新系统（order 47~50，晚于 46、早于效果结算 100+）查询 `ControlStateChangedEvent`，当 `entered && controlType ∈ {Stun, CrackFly}` 且单位带 `CastState/ChannelState` → `unit.AddTag<CastInterruptedTag>()`。
- 优点：语义清晰（"控制进入 → 打断施法"），与事件体系一致；只处理跳变，无常驻轮询。
- 缺点：新增 1 个系统；需保证 order < 132（事件清理边界）；同帧先于控制进入的施法 tick 需下帧才打断（可接受，控制系统 order46 在施法 20/21 之后，事件下一个 Interval 才被打断系统消费——时序为"控制进入后 ≤1 逻辑帧内打断"）。

### 方案 B：施法 tick 轮询（CastingSystem/ChannelingSystem 内检查）
- 在 CastingSystem/ChannelingSystem 每 tick 开头检查 `ControlHelper.IsIncapacitated(unit)`（或按语义清单）→ 非空则 `InterruptCast`。
- 优点：无新系统、无 order 依赖、打断及时（同 tick）；与 MoveToCast 现有 IsControlled 检查一致。
- 缺点：语义是"轮询当前状态"而非"响应进入事件"；控制"解除瞬间"不误判（IsIncapacitated 只在持续控制期间 true，退出即恢复施法？——但打断是一次性的，需保证只打一次，可用 `cast.effectCommitted==false && 阶段在 Casting/Channeling` 判定当前未打断过）。

**推荐：方案 B（tick 轮询）**。理由：打断本质是"只要在禁止行动的控制中就应停止施法"，轮询天然覆盖"施法中途进入控制"（无论控制何时进入）；无需新增系统与 order 约束；MoveToCast 已用同款检查（IsControlled），模式统一。风险是每施法单位每 tick 多一次 `ControlHelper.IsIncapacitated`（两次属性 finalValue 读取 + 关系遍历），施法单位数量远小于单位总量，可接受。

### 打断处理路径（复用现有）
- CastingSystem：`InterruptCast` → `OnInterrupted` 回调 → `ResetAbility`（状态回 Ready、移除 CastState）。**不进入冷却、不结算生效点**（现状语义，保留）。
- ChannelingSystem：`InterruptChannel` → `OnInterrupted` → 移除 ChannelState+CastState、状态回 Ready。
- 打断后需保证不重复打断：移除 CastState 后不再有可打断对象。

## 3. 语义决策：哪些控制打断施法

| 控制 | 禁止发起新施法？ | 打断已开始的吟唱？ | 打断持续引导？ | 理由 |
|---|---|---|---|---|
| Stun（眩晕） | ✅ | ✅ | ✅ | 禁止一切行动 |
| CrackFly（击飞） | ✅ | ✅ | ✅ | 位移打断吟唱（War3 直觉） |
| Silence（沉默） | ✅ | ❌（不打断吟唱） | ✅ | 常见 MOBA 语义：沉默禁"新施法"与"引导"，但已开始的前摇/吟唱让其完成（若本地机制无法区分再按 Stun 同权） |
| Root（定身） | ❌ | ❌ | ❌ | 只禁移动，允许施法 |
| NoAttack（缴械） | ❌ | ❌ | ❌ | 只禁攻击 |

> 语义区分说明：施法 tick 轮询用哪个函数需细分——`ControlHelper.IsIncapacitated` = Stun||CrackFly（仅禁行动）。发起新施法的检查需另用"Stun||Silence||CrackFly"。设计在 ControlHelper 增加/复用组合谓词（如 `CanCast(unit)` = 非眩晕/沉默/击飞），避免三处手写判断。

## 4. 目标合法性复查

### 现状
- 发起时只查距离；移动施法跟踪目标位移（每 tick 若目标移动>100 更新 MoveCommand 目标）。
- 进入吟唱后不复查目标。

### 目标语义
| 情形 | 推荐行为 |
|---|---|
| 目标死亡（非 Alive） | **取消施法**：吟唱阶段若 `targetUnit` 已死亡 → 走打断（OnInterrupted）。施法者是"点目标技能"（无 targetUnit）不受影响。 |
| 目标被隐身/无敌 | **不取消**（本次不做）：隐身/无敌只是伤害/选取免疫，施法动作可完成（生效点结算天然 0/免疫）。若要"对隐身目标不可施放"属 TargetFilter 复杂规则，后续方向。 |
| 目标变敌/变友 | **不取消**（本次不做）：阵营动态判定超范围。 |

- 复查点：吟唱 `CompleteCastPoint`（施法生效点 TryCommitEffect 前）检查 `targetUnit.IsNull == false && UnitHelper.IsAlive(targetUnit)==false` → 走 `InterruptCast`（或新增 `TargetLost` 取消路径，语义等同打断但不触发 OnInterrupted？——**设计：触发 OnInterrupted**，保持"被打断回调"一致，项目可在回调里区分原因后续扩展）。

## 5. CastInterruptedTag 语义

- 现状：仅 MoveToCast/Casting/Channeling 读取。若采用方案 B（轮询），该 Tag 的"唯一外部来源"仍是空的——Tag 仍保留供"外部显式打断"（如剧情、主动取消）使用。
- 本 change 补注：Tag = "外部/控制触发的施法中断请求"，消费方为 CastingSystem/ChannelingSystem；轮询打断不依赖 Tag（直接 InterruptCast），Tag 路径保留作为显式打断 API（后续可由 CancelAbility 等产生）。

## 6. 施法入口控制校验实现点

`CastRequestSystem.Process`（CastingSystem.cs L37-47）在 `abilityBase.state != Ready` 检查旁新增：
```
if (!ControlHelper.CanCast(unit))  // Stun/Silence/CrackFly 禁发
    { unit.RemoveComponent<CastRequest>(); return; }
```

## 7. 对 synthesize-pause-from-controls 的依赖

- 若待审的 `synthesize-pause`（Stun/CrackFly 统一驱动 PauseUnit）已实施，则 Stun 进入会触发原生暂停 → 打断轮询读取 `ControlHelper` 属性同样为 >0，二者不冲突。
- 本 change 不依赖原生暂停是否实现：ECS 层控制属性（Stun 等）即可驱动打断。

## 8. 文件清单

- 改 `War3Frame/Src/Helpers/ControlHelper.cs`：新增 `CanCast(unit)`（= 非 Stun/Silence/CrackFly），内部组合 `IsSilenced`（Stun||Silence||CrackFly）语义已有 `IsSilenced` 但名字含沉默；补充 `CanCast` 更清晰。
- 改 `War3Frame/Src/Systems/Ability/CastingSystem.cs`：入口 Process 加 `CanCast`；CastingSystem/ChannelingSystem tick 加受控即打断；`CompleteCastPoint` 加目标死亡复查。
- 改 `War3Frame/Src/Components/Ability/CastState.cs`：CastInterruptedTag 语义注释。
- 改 `Projects/test/...`：新增验证场景或并入现有。
