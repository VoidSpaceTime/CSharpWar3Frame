# 总结：Native 同步三模式规则落地与既有代码对齐

**状态**：已实施  
**日期**：2026-08-28

## 实际改动

### 治理文档
1. `AGENTS.md` 新增「Native 同步三模式规则」章节：
   - 模式决策树（Compare-Sync / Dirty-Driven / Request）与判定规则
   - Dirty 契约（合并 flags、同步后清除、有载荷用 IComponent / 无载荷用 ITag、累积状态必须存组件）
   - Helper/Modifier 修改入口分层（ECS 组件层 ← Helper ← Modifier ← Native 层）
   - 迁移原则（新代码按决策树选型、按领域分批迁移、高频数值保持 Compare-Sync）

### Player 领域（Request → Dirty-Driven）
2. `War3Frame/Src/Components/Player.cs`：
   - 删除 `PlayerNameNativeRequest` / `PlayerColorNativeRequest` / `PlayerAllianceNativeRequest`（及 `PlayerAllianceNativeKind`）
   - 新增 `PlayerDirty : IComponent` + `PlayerDirtyFlags`（Name/Color/Alliance）
   - 新增 `PlayerAllianceState : IComponent`（`bits[target]` 低 5 位：Basic/Vision/Control/FullControl/Neutral；`dirty[target]` 增量同步标记）
3. `War3Frame/Src/Helpers/PlayerHelper.cs`：
   - `SetName/SetColor`：写 ECS `PlayerNative` 组件 + 同步数组镜像 + 打 Dirty
   - `SetAlliance/SetNeutral`：双向写 `PlayerAllianceState` 位 + dirty 标记 + 打 Alliance Dirty
   - `SetVision/SetControl/SetFullControl`：单向（A 授予 B）
   - `SetAllianceBit` 用 TryGetComponent + 自动初始化，防未初始化崩溃
4. `War3Frame/Src/Systems/Native/PlayerNativeSystem.cs`：
   - 三个 Request 消费系统合并为 `PlayerNativeSyncSystem`（QuerySystem<PlayerNative, PlayerDirty>）
   - 增量同步 dirty 目标，同步后清 Dirty 与 dirty[target]
   - Basic/Neutral 互斥：`PASSIVE = isNeutral || isBasic`，修复 PASSIVE 位双写覆盖

### 既有模式核对（保持不动）
5. `UnitNativeSystem` + `UnitNativeSyncRegistry`：Compare-Sync（血量/蓝量）✓ 符合规则
6. `EffectNativeSystem`：Dirty-Driven（已在前序 change 完成）✓
7. `UnitCreateNativeSystem` / `UnitMoveNativeSystem` / `ItemCreateNativeSystem`：Request（一次性副作用）✓

## 验证

- `dotnet build War3Frame/War3Frame.csproj`：0 错误（174 个存量 KKApi nullable 警告，与本次无关）
- 全仓 `PlayerNameNativeRequest` / `PlayerColorNativeRequest` / `PlayerAllianceNativeRequest` 零引用
- `TeamHelper`（唯一业务调用方）签名兼容，编译通过
- review 代理审核后修复：PASSIVE 位冲突、首次全量重放抹默认结盟、联盟单向同步、OpenSpec 缺件（design.md/spec.md/tasks 全勾）

## 遗留与问题清单

1. `ItemCreateNativeSystem` 为未实现占位（NotImplementedException），物品地面显示已确定走特效+UI 方案，后续单独提案
2. `PlayerHelper._players` 数组为 ECS 组件值镜像（`GetPlayer` 返回 ref），存在双写风险（本次已保证 SetName/SetColor 双写一致；长期可考虑去除镜像改 EntityRef 查询）
3. War3 客户端运行时验证未执行（阻塞验证推迟，需在验收阶段统一执行）
4. `fix-effect-transform-state-loss` 提案原承诺 `EffectModifier` 链式 API，实现为 `EffectHelper` 静态方法（RotateX/Y/Z）；功能等价，API 形态漂移已记录