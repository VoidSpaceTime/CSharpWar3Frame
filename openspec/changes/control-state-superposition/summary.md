# 实施总结：控制状态叠加态（superposition，方案 B）

**状态**：已实施
**等级**：light（方案 B：复用属性系统作为计数真相，仅加临界检测层）
**实施日期**：2026-08-31

## 实际改动范围

- **新增** `War3Frame/Src/Components/ControlState.cs`：`ControlType` 枚举（Stun/Silence/Disarm/Root/Knockback）、`ControlStateSnapshot`（单位位域快照）、`ControlStateChangedEvent`（独立事件实体）、`ControlStateNativeRequest`（Native 请求）。
- **新增** `War3Frame/Src/Systems/ControlStateTransitionSystem.cs`（`Interval, 46`）：扫控制/免疫属性实体收集单位 → 对比快照与免疫压制后有效值（`ControlHelper.GetEffectiveValue`）→ 0↔正 跳变时更新快照并创建事件 + 请求；**属性被整体移除时对残留激活位补发解除事件 + 请求**（原生状态收敛，防止 PauseUnit 永久暂停）；无控制属性的单位快照清理。store 从单位实体缓存（`Entity.Store`），不依赖 `Game.Store`，本地场景可独立驱动。
- **新增** `War3Frame/Src/Systems/Native/UnitControlNativeSystem.cs`（Immediate）：消费 `ControlStateNativeRequest`；Stun → `JassApi.PauseUnit(entered)`；Silence/Disarm/Root/Knockback 留 TODO（1.27 无原生沉默/缴械函数，能力映射待定）；消费后删除请求。
- **新增** `Projects/test/Scripts/Process/ControlStateValidationScenario.cs` + `Program.cs` 接入：5 Phase 验证（双来源叠加恰好一次进入、部分移除静默、全部移除恰好一次解除、免疫压制/解除、快照清理）。

## 验证结果

- `dotnet build War3Frame/War3Frame.csproj`：0 错误。
- `dotnet build Projects/test/test.csproj`：0 错误。
- 本地同步场景（临时 runner，已清理）`ControlStateValidationScenario: PASS`，5 个 Phase 全部断言通过。

## 实施中发现的既有缺陷修复（fast 级，未单独提案）

1. **`AttributeHelper` 静态构造 NRE**（`Src/Helpers/AttributeHelper.cs`）：partial 类跨文件静态字段初始化顺序未定义，`Combat.cs` 的 `Register()` 调用先于 `_types` 初始化器执行。修复：`Register` 内懒初始化 `_types`。此前未被暴露是因为现有场景与游戏主流程从未触发属性系统。
2. **`AttrCalculationSystem` StructuralChangeException**（`Src/Systems/Attribute/AttrCalculationSystem.cs`）：Friflo 禁止 Query 循环内 `RemoveTag`（结构变更）。修复：收集后统一循环外移除。技能文档（friflo-ecs-query）已预警此边界。

若需正式留痕，可补 1 个 fast 级 proposal 记录以上两处修复。

## 遗留风险与后续事项

- **Silence/Disarm/Root/Knockback 的 Native 能力映射未实现**：事件与请求照常发出，业务可先监听 `ControlStateChangedEvent` 自定义响应；需要可用 API（物编虚拟技能 / KK 扩展）时单独提案。
- **Friflo 实际依赖为 4.0.0-preview.2**（`War3Frame.csproj`），`.opencode/skills/friflo-ecs-*` 文档基线仍写 3.4.2，已过时，建议后续更新技能文档基线。
- `ControlStateTransitionSystem` 每 tick 全量扫描属性实体收集单位；单位规模大时可优化为增量索引（当前规模可接受）。
- Native 副作用（PauseUnit）未做真实 War3 客户端验证（本地无句柄环境）；建议后续在 War3 测试客户端跑一次眩晕/解除流程。