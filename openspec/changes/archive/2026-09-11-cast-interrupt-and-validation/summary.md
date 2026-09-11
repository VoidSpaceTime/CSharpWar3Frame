# 实施总结：施法打断 + 控制校验 + 目标合法性二次确认

**状态**：已实施
**等级**：light（R1 Focused）
**实施日期**：2026-09-08

## 实际改动范围

### 修改文件（War3Frame/Src/）
- `Helpers/ControlHelper.cs`：新增 `CanCast(unit)` 谓词（= 非眩晕/沉默/击飞，即 `!IsSilenced`），作为"是否允许发起新施法"的统一判定入口，避免入口/tick 多处手写组合判断。
- `Systems/Ability/CastingSystem.cs`（3 处检查点）：
  1. **施法入口控制校验**（`CastRequestSystem.Process`）：新增 `!ControlHelper.CanCast(unit)` → 拒绝受控单位（Stun/Silence/CrackFly）发起新施法。
  2. **前摇吟唱受控打断**（`CastingSystem.OnUpdate` Casting 阶段）：`ControlHelper.IsIncapacitated(unit)`（Stun/CrackFly）→ `InterruptCast`（复用现有链：OnInterrupted 回调 → 状态回 Ready → 不结算生效点/不进冷却）。Backswing（后摇，效果已提交）不打断，保持现状推进完成。
  3. **目标死亡复查**（`CompleteCastPoint` 生效点提交前）：单位目标技能若 `targetUnit` 已死亡 → `InterruptCast`（取消施法）；点/无目标技能不受影响。
- `Systems/Ability/CastingSystem.cs`（ChannelingSystem）：**持续引导受控打断**——`!ControlHelper.CanCast(unit)`（Stun/Silence/CrackFly）→ `InterruptChannel`。语义：沉默可打断持续引导，区别于前摇吟唱（设计决策，用户确认符合预期）。
- `Components/Ability/CastState.cs`：`CastInterruptedTag` 语义注释更新——消费方（施法/引导/移动桥接系统）+ 添加方（外部显式打断）；受控打断走 tick 轮询直接调用打断，不依赖本 Tag。

### Projects/test
- 新增 `Scripts/Process/CastValidationScenario.cs`（4 Phase：受控禁发 / 眩晕打断前摇 / 沉默打断引导 / 目标死亡取消施法）；`Program.cs` 注册。

## 验证结果

- `dotnet build War3Frame/War3Frame.csproj`：0 错误（182 个存量 nullable warning，与本次无关）。
- `dotnet build Projects/test/test.csproj`：0 错误（1 个存量 warning）。
- **本地同步 runner**（临时工程，已清理）：`CastVerify: PASS`，4 个 Phase 全部断言通过：
  - 眩晕单位 CastRequest → 被拒绝移除、无 CastState、技能保持 Ready。
  - 进入 Casting 后加 Stun → 一帧内 CastState 移除、技能回 Ready、无 AbilityCooldownState（打断不进冷却）。
  - castPoint=0 + channel 技能进入 Channeling 后加 Silence → 引导打断、ChannelState 移除、技能回 Ready。
  - Casting 中目标 `isAlive=false` → 越过生效点后施法被取消（无 CastState、技能回 Ready）。

## 全局影响

- `War3Frame/`：施法状态机与控制状态协作。行为变化：此前"受控单位仍可发起施法 / 施法中进入眩晕仍可完成"为缺陷，现按语义修复。
- `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`：未涉及。
- `Projects/`：新增验证场景；无技能依赖"受控仍施法"旧行为，现有 ItemCompanion 场景编译通过、流程不受影响。

## 设计说明

- **打断接线选型**：方案 B（施法/引导系统 tick 轮询 `ControlHelper`），不新增系统、无 order 依赖、与 MoveToCast 既有 `IsControlled` 检查模式一致。
- **语义决策（用户确认符合预期）**：Silence 禁止发起新施法 + 打断持续引导，但不打断已开始的前摇吟唱；Stun/CrackFly 同时打断前摇与引导；Root/NoAttack 不影响施法。
- **CastInterruptedTag 保留**：作为外部显式打断 API（剧情/主动取消），受控打断不依赖 Tag。

## 剩余风险与后续事项

1. **真实 War3 客户端验证未执行**（本地无句柄环境）：眩晕期间真实原生表现（暂停/被控动画）需客户端跑一次确认。列为非阻塞遗留。
2. **目标合法性复查仅覆盖"死亡"**：目标隐身/无敌/阵营动态切换的取消语义未做（design §4 明确非目标），如需可后续扩展 TargetFilter 规则。
3. 待审 `synthesize-pause-from-controls`（Stun/CrackFly 原生 Pause 合成）与本 change 正交；其落地后原生表现层由控制状态体系驱动，不改变本 change ECS 层语义。

## 归档建议

本 change 已满足"已实施"条件（范围完成、验证通过、summary 存在、无阻塞项）。建议后续提交时归档至 `openspec/changes/archive/2026-09-08-cast-interrupt-and-validation/`。
