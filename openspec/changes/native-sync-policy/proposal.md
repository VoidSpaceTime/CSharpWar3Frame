# 提案：Native 同步三模式规则落地与既有代码对齐

**状态**：已批准（用户 2026-08-28 确认"这个提案可以录入 agents.md"并指示全项目落实）→ 已实施  
**等级**：architecture  
**提案日期**：2026-08-28

---

## 背景

框架此前存在两套并行的 Native 同步写法：

1. `UnitNativeSystem`：Compare-Sync（高频血量/蓝量，每轮对比快照）
2. `EffectNativeSystem`：Dirty-Driven（特效外观，`EffectBase + EffectDirty`）
3. `PlayerNativeSystem` 与 `ItemCreateNativeSystem`：全部走 Request（`PlayerNameNativeRequest` / `PlayerColorNativeRequest` / `PlayerAllianceNativeRequest` / `ItemCreateNativeRequest`）

用户确认采用混合策略，并要求把决策规则写入 `AGENTS.md`，扫描整个项目按规则落实职责。此前 `EffectTransformRequest` 的累积状态丢失问题已单独修复（`fix-effect-transform-state-loss`）。

## 目标

1. 在 `AGENTS.md` 录入「Native 同步三模式规则」：Compare-Sync / Dirty-Driven / Request 的决策树、Dirty 契约、Helper/Modifier 分层、迁移原则。
2. 扫描 `War3Frame/Src` 全部 Native 同步路径，按规则归类：
   - 单位血量/蓝量 → Compare-Sync（保持不动）
   - 特效外观 → Dirty-Driven（已落地，保持不动）
   - **玩家名称/颜色 → 从 Request 改为 Dirty-Driven**（`PlayerNative` 已是状态组件，同步应消费 Dirty 而非 Request）
   - **玩家联盟 → 从 Request 改为 Dirty-Driven**（`PlayerNative` 持有关系矩阵语义，改为状态 + Dirty）
   - **物品创建 → 保持 Request**（一次性副作用；`ItemCreateNativeSystem` 当前为未实现占位，不做功能扩展）
3. 输出问题清单：扫描中发现但与本次范围无关或需后续决策的问题，总结给用户。

## 非目标

- 不实现 `ItemCreateNativeSystem` 的功能（物品地面显示已确定走特效+UI 方案，另行提案）
- 不改动 `UnitNativeSystem` 的 Compare-Sync 机制
- 不改造特效层（已完成）
- 不引入通用事件总线或触发器系统

## 影响范围

### 治理文档
- `AGENTS.md`：新增「Native 同步三模式规则」章节（已落地）

### Player 领域（重点改造）
- `War3Frame/Src/Components/Player.cs`：新增 `PlayerDirty : IComponent`（flags：Name/Color/Alliance）；`PlayerNative` 增加持久联盟状态表达（`PlayerAllianceState` 或等价组件）
- `War3Frame/Src/Helpers/PlayerHelper.cs`：`SetName/SetColor/SetAlliance*` 改为写状态 + 打 Dirty，不再写 Request
- `War3Frame/Src/Systems/Native/PlayerNativeSystem.cs`：改为消费 `PlayerDirty` 的 Native 同步系统，同步后清除 Dirty

### 组件层
- `War3Frame/Src/Components/Player.cs`：删除 `PlayerNameNativeRequest` / `PlayerColorNativeRequest`（被 Dirty 替代）；`PlayerAllianceNativeRequest` 视设计保留或删除

### 其他 Native 系统
- 审计 `UnitCreateNativeSystem` / `UnitMoveNativeSystem` / `ItemCreateNativeSystem`：确认均属 Request 模式，符合规则，不动

## 方案摘要

### Player Dirty 设计

```csharp
[Flags]
public enum PlayerDirtyFlags
{
    None = 0,
    Name = 1 << 0,
    Color = 1 << 1,
    Alliance = 1 << 2,
}

public struct PlayerDirty : IComponent
{
    public PlayerDirtyFlags flags;
}
```

- `PlayerHelper.SetName`：写 `PlayerNative.name` + 合并 `PlayerDirtyFlags.Name`
- `PlayerHelper.SetColor`：写 `PlayerNative.color` + 合并 `PlayerDirtyFlags.Color`
- `PlayerHelper.SetAlliance/SetVision/SetControl/SetFullControl/SetNeutral`：更新关系矩阵（静态缓存保持）+ 打 `PlayerDirtyFlags.Alliance`
- `PlayerNativeSystem`（改名 `PlayerSyncNativeSystem` 或保留三个类）消费 `PlayerDirty`，按 flags 调用 `JassApi.SetPlayerName/SetPlayerColor/SetPlayerAlliance`，同步后清除 Dirty

### 联盟状态表达

`PlayerHelper` 的 `Relations` 静态矩阵是查询缓存。按"ECS 是唯一真相"原则，联盟状态应落到 ECS：在 `PlayerNative` 上新增 `PlayerAllianceState`（16 个目标位）或在玩家实体挂关系组件。考虑到 16 玩家上限，用 `PlayerNative` 内嵌 `PlayerAllianceFlags[16]`（按位记录 BasicAlliance/Vision/Control/FullControl/Neutral）即可，Native 同步时对比 flags 变化。

## 风险与回滚

### 风险
1. `PlayerAllianceNativeRequest` 删除会影响所有调用方（已核对为 `PlayerHelper` 内部 + `PlayerNativeSystem`）
2. 联盟状态从静态矩阵迁移到 ECS 组件，涉及 `PlayerHelper.GetRelation/IsAlly/IsEnemy` 读取路径调整
3. Dirty 合并逻辑需要与 `EffectDirty` 保持一致（按位 OR，同步后清除）

### 回滚
- 恢复 `Player*NativeRequest` 组件与旧 `PlayerNativeSystem` 消费逻辑；`PlayerDirty` 保留不影响编译。
- 成本：低（Player 领域独立，无跨模块依赖）

## 验收标准

1. `AGENTS.md` 含完整三模式规则章节
2. 全仓无 `PlayerNameNativeRequest` / `PlayerColorNativeRequest` 引用
3. `PlayerHelper` 所有写入口均打 Dirty；无直接 `AddComponent(Player*NativeRequest)` 残留
4. `PlayerNativeSystem` 消费 `PlayerDirty`，同步后清除
5. `dotnet build War3Frame/War3Frame.csproj` 0 错误
6. 问题清单输出给用户

## 相关文档

- `AGENTS.md`「Native 同步三模式规则」（本提案落地）
- `ARCHITECTURE.md`「Native 状态投影 (compare-sync)」
- `War3Frame/Src/Components/Player.cs`
- `War3Frame/Src/Helpers/PlayerHelper.cs`
- `War3Frame/Src/Systems/Native/PlayerNativeSystem.cs`