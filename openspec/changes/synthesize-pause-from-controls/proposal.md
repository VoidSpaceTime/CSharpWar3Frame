# 暂停（Pause）状态合成：眩晕/击飞/纯暂停统一驱动 PauseUnit

## 元信息

- **状态**：待审核
- **等级**：light
- **变更 ID**：synthesize-pause-from-controls
- **日期**：2026-09-03
- **复盘强度**：R0 Direct（编译验证；native 真实效果需 War3 客户端，列为非阻塞验证项）
- **前置**：`control-state-superposition`（已实施）建立了控制状态检测体系：控制属性 finalValue（经免疫压制）0↔正跳变 → `ControlStateChangedEvent` + `ControlStateNativeRequest`。

## 背景与目标

### 背景

`control-state-superposition` 实施后，控制状态检测体系已就位，但 **Stun / CrackFly 的 native 动作是空的**：`UnitControlNativeSystem` 的 switch 只有 `NoAttack / Hide / Root / NoPath / Pause / Locust / ...` 分支，其中 Pause→`PauseUnit`，而 **Stun 与 CrackFly 没有任何 case**——眩晕/击飞只在 ECS 属性层被标记，原生单位不暂停。该 change 的验收标准 #3 也明确留下缺口："Stun 对应 PauseUnit 映射…实施时确认"。

设计意图（用户 2026-09-03 确认）：

| 状态 | 语义 | ECS 表达 | Native 动作 | 附加表现 |
|---|---|---|---|---|
| Pause | 最底层暂停（过场/镜头等独立纯暂停） | Pause 属性 | PauseUnit | 无 |
| Stun | 暂停 + 标记眩晕 + 眩晕特效 | Stun 属性 | PauseUnit（合成） | 特效走事件监听 |
| CrackFly（击飞） | 暂停 + 标记击飞 | CrackFly 属性 | PauseUnit（合成） | 位移由弹道系统自理 |

即：**"单位是否需要暂停" = raw(Pause) > 0 ‖ effectiveStun > 0 ‖ effectiveCrackFly > 0**（纯 Pause 不经免疫直接读 finalValue；Stun/CrackFly 各自经免疫压制），任一为真即暂停，全解除才恢复。

### 方案选型：X2（检测层合成），否决 X1 / X3

- **X1（辅助修改器子实体）**：眩晕 buff 额外创建 Pause 修改器实体。被否决——三条删除路径（到期/净化/Replace 重建）任漏一条 → Pause 值残留 → 单位永久暂停，泄漏风险高。
- **X2（检测层合成）**：buff 只写 Stun/CrackFly 属性（现状不变）；检测系统把 Pause 状态合成为 `raw(Pause)‖effectiveStun‖effectiveCrackFly`。被采纳——buff 侧零改动、无新实体、零泄漏风险。
- **X3（一个 buff 多属性贡献）**：改 Friflo 关系建模。被否决——改动最底层。

### 目标

1. Stun / CrackFly 生效时，单位原生暂停（`PauseUnit(true)`）；全部解除后恢复。
2. 纯 Pause（独立暂停，不含眩晕/击飞）保持可用。
3. 眩晕/击飞的释放竞态天然免疫（属性 finalValue 自身累加：多个来源叠加 → 全归 0 才解除）。
4. 眩晕/击飞免疫分别生效（effectiveStun 归 0 不触发暂停，免疫不穿透到 Pause）。
5. 注册独立 `Pause` 属性支持纯暂停（过场/镜头等）；**不注册 `PauseImmunity`**——暂停是最底层硬控制（类比 War3 `PauseUnit` 连无敌/免疫也暂停），系统级强制，不可免疫。

### 非目标

- 不新增 Pause 之外的辅助属性/辅助修改器实体。
- 不把眩晕特效写进 native switch（特效由监听 `ControlStateChangedEvent(Stun)` 的业务层负责）。
- 不改动 buff 施加路径（BuffHelper.Stun 等保持只写 Stun 属性）。
- 不处理 Silence/NoAttack/Root 的暂停派生（Root 走 SetUnitPathing 已是独立 native 动作，保持现状）。

## 影响范围

| 区域 | 影响 | 说明 |
|---|---|---|
| `War3Frame/Src/Systems/ControlStateTransitionSystem.cs` | 修改 | Pause 的 active 判定从"查 Pause 属性"改为"合成判定" |
| `War3Frame/Src/Helpers/AttributeHelper.cs` | 修改 | 注册 `Pause = Register("Pause")`；不注册 PauseImmunity（合成判定中纯 Pause 不经免疫，读 finalValue） |
| `War3Frame/Src/Systems/Native/UnitControlNativeSystem.cs` | 无改动（确认） | Pause→PauseUnit 分支已存在，且检测系统只发 Pause 请求（Stun/CrackFly 不再发自己的 native 请求，或保持发但不执行） |
| `War3Frame/Src/Components/ControlState.cs` | 修改 | `ControlStateSnapshot.bits` 位宽从 `byte` 扩到 `ushort`（覆盖 12 个 ControlType 成员，序号 ≥8 不再溢出）；`ControlStateChangedEvent(ControlType)` 语义说明：Stun/CrackFly 事件保留（业务监听特效）；native 请求层面 Pause 合成 |
| `War3Frame/Src/Helpers/ControlHelper.cs` | 修改 | `GetImmunityAttrId` if-链改为读取 `ControlAttrs` 映射表（消除免疫映射双份维护）；查询语义不变 |
| `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/` | 不受影响 | |
| `Projects/test` | 可能 | 验证场景/模板示例若覆盖 Stun 暂停链路需确认 |

## 方案摘要

### 检测系统：Pause 合成判定

`ControlStateTransitionSystem` 当前对 `ControlAttrIds`（Stun/Silence/NoAttack/Root/CrackFly）逐个做"快照对比 → 跳变发事件+请求"。改动点：

1. **给 Pause 一个"合成源"声明**：Pause 的 active = `raw(Pause) > 0 ‖ effective(Stun) > 0 ‖ effective(CrackFly) > 0`（纯 Pause **不经免疫**，直接读 finalValue；Stun/CrackFly 经各自免疫压制）。
2. **Stun/CrackFly 的跳变**：保留 `ControlStateChangedEvent`（供业务监听特效/位移），**不再各自发 `ControlStateNativeRequest`**（它们的 native 动作就是 Pause）。
3. **Pause 的跳变**：由合成判定结果驱动快照位 + 发一次 `ControlStateNativeRequest(Pause, entered)`。同帧内 Stun 跳变导致 Pause 合成值变化 → 只发一次 Pause 请求，避免抖动。
4. **纯 Pause 属性**：本 change **注册独立 `Pause` 属性**，直接并入合成源，支持过场/镜头等纯暂停。
5. **Pause 不进 `ControlAttrs` 主表**：Stun/CrackFly/Silence/NoAttack/Root 仍在主表逐条检测（各自发事件，不再发自己的 native 请求）；**Pause 由合成判定单独计算**（读取 raw(Pause) finalValue + effectiveStun + effectiveCrackFly），只与 Pause 快照位对比、跳变时发一次请求，避免与主循环重复发。
6. **快照位宽前置**：先扩 `bits` 到 `ushort`（Blocker），否则 Pause 序号 10 的位 `(byte)(1<<10)=0`，快照对比与去抖失效。
7. **预留成员说明**：`ControlType` 其余成员（Hide/NoPath/Locust/Invulnerable/Invisible/Sorcery）与 native switch 对应 case 为**预留兜底**，当前无属性接入、无检测驱动路径，不在本 change 驱动范围，仅保证枚举/switch 完整性。

实现要点：避免对每帧对所有单位重算时产生重复跳变——Pause 快照位与合成结果对比，仅在变化时发请求。

### 免疫语义

- Stun 免疫 → `effectiveStun = 0` → Pause 合成不含它 → 不暂停（免疫穿透被切断）。
- CrackFly 免疫同理。
- **纯 Pause 不设免疫**：`Pause` 是最底层硬控制（类比 War3 `PauseUnit` 连无敌/免疫单位也暂停），系统级强制。合成判定中纯 Pause 贡献**直接读 finalValue，不经过免疫压制**。

### Native 层

`UnitControlNativeSystem` 保持现状即可：收到 `ControlStateNativeRequest(Pause, entered)` → `PauseUnit(entered)`。Stun/CrackFly 自身不再发 native 请求（无 case 也不会有泄漏——它们根本不发）。

## 风险与回滚

- 回滚：还原 ControlStateTransitionSystem 的合成判定（Pause 不参与检测）即可，native/buff 侧无持久改动。
- 主要风险：快照位宽已前置修复（`byte`→`ushort`）；合成判定放在 Stun/CrackFly 检测同帧的顺序——若 Pause 合成在 Stun 检测之前计算，需确保用"旧快照 + 新 effective"对比而不是累积两帧。
- 结构变更安全（Friflo 3.6.0）：收集阶段结束后的 `HashSet<Entity>` 遍历中 `AddComponent`/`CreateEntity`/`DeleteEntity` 均不在 Query 迭代内，安全（CreateEntity/DeleteEntity 本就不算结构变更）。

## 复盘修复项（Hephaestus 2026-09-03）

| # | 严重度 | 修复 |
|---|---|---|
| 1 | Blocker | `ControlStateSnapshot.bits` byte→ushort，`BitOf` 同步（否则 Pause 序号 10 位溢出为 0） |
| 2 | Blocker | Pause 不进 `ControlAttrs` 主表，单独合成 + 快照去抖（依赖 #1） |
| 3 | Major | 预留成员（Hide/NoPath/Locust/Invulnerable/Invisible/Sorcery）补"预留兜底"说明，防误导 |
| 4 | Minor | `ControlHelper.GetImmunityAttrId` if-链改查 `ControlAttrs` 表（消双份维护） |
| 5 | Nit | 验收标准 #5 依赖 Pause 属性先注册（tasks T1 已覆盖） |

## 验收标准

1. Stun 生效（immune 未压）→ 单位被 `PauseUnit(true)`；Stun 解除（值归 0）→ `PauseUnit(false)`。
2. CrackFly 同理。
3. Stun + CrackFly 同时生效，其一先解除 → 单位仍暂停；全解除才恢复。
4. Stun 被免疫 → 不触发 PauseUnit。
5. 纯 Pause（独立 `Pause` 属性，无免疫）独立触发 PauseUnit，与 Stun/CrackFly 互不干扰。
6. Stun 跳变仍发 `ControlStateChangedEvent`（特效监听可用），但不再单独发 Stun 的 native 请求。
7. 同帧多控制变化 → Pause 只发一次请求（无抖动）。
8. `dotnet build War3Frame/War3Frame.csproj` 0 error；Projects/test 0 error。

## 后续工作

- 眩晕特效模板示例（监听 Stun entered 播 attachment 特效）。
- War3 客户端真实验证 PauseUnit 表现（非阻塞）。
