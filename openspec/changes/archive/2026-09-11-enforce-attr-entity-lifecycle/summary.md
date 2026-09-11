# 总结：属性实体生命周期完整化

对应提案：`proposal.md`（`enforce-attr-entity-lifecycle`，`full`）
状态：`已实施`（编译 + 静态 + 运行时场景全部通过）
日期：2026-09-10

## 1. 实际改动范围

### Part A：唯一性 + 入口收敛

- `War3Frame/Src/Helpers/AttributeHelper.cs`
  - `CreateAttr` 由 `public` 收窄为 `internal`，并加防御性幂等：先 `TryGetAttr`，命中直接返回既有实体（强制 INV-1，同一 unit 同一 `typeId` 至多一个属性实体）。
  - `GetOrCreateAttr` 保持唯一对外获取/创建入口。
- `War3Frame/Src/Helpers/ModifyHelper.cs`
  - `AddModifier(attrEntity, ...)` 由 `public` 收窄为 `internal`；`AddModifierToUnit` 保持 public 并内部委托。
- 调用点：`UnitSpecBuilder`、`LevelExperienceSystem` 仍在同程序集调用，无需改动。

### Part B：孤儿属性回收

- `War3Frame/Src/Systems/Attribute/AttrCalculationSystem.cs`（order 45）
  - 重算后追加回收判定，判据全部满足才回收：
    1. 无 `ModifyTarget` 入边（含无 `ModifyValue` 的 DoT 载体）；
    2. `baseValue == 0`；
    3. `current == 0`；
    4. `flatBonus == 0`；
    5. `percentBonus == 0`；
    6. `CanReclaimByOwner`：owner 带 `UnitSpecData` 且该 `typeId` 未被模板声明。
  - 删除在 Query 迭代外执行，`toReclaim` 按 `entity.Id` 升序（锁步确定性）；先 `RemoveRelation<HasAttr, Entity>` 再 `DeleteEntity`。

### 与提案的判据收紧（重要）

提案原写"owner 无 `UnitSpecData` 视为未声明 → 可回收"。实现改为**保守保留**（无 `UnitSpecData` → 不回收），原因：

- 本地/工具单位（`store.CreateEntity()`，无模板）的 base=0 常驻属性会被误回收；
- 已核实现有 `ControlStateValidationScenario` 依赖 base=0 常驻属性（Stun/StunImmunity），原判据会破坏其 Phase 4；
- 该收紧不损失真实目标：真实模板单位始终带 `UnitSpecData`，装备/护盾场景照常回收。

已同步 `proposal.md` §6.1、`design.md` §4.1/§5、`spec.md` AEL-2/AEL-3。

### 测试

- 新增 `Projects/test/Scripts/Process/AttributeLifecycleValidationScenario.cs`，注册到 `Projects/test/Program.cs`。

## 2. 验证覆盖

| 验证项 | 方式 | 结果 |
|---|---|---|
| War3Frame 编译 | `dotnet build War3Frame/War3Frame.csproj` | 0 error（182 warning，均既有） |
| test 编译 | `dotnet build Projects/test/test.csproj` | 0 error（186 warning，均既有） |
| 唯一性/入口静态核对 | 全仓 grep | `CreateAttr` 无外部真实调用；`AddModifier(attrEntity)` 无外部调用 |
| 迭代内结构变更 | 代码审查 | 回收删除在 `ForEachEntity` 外；`Systems/Attribute` 仅一处 `DeleteEntity` |
| 场景用例（代码就位） | `AttributeLifecycleValidationScenario` | 已覆盖声明保留 / 贡献者保留 / 现值保留 / 回收 / 关系清理 |
| **场景运行时执行** | 临时宿主 runner（native-independent 场景逐进程执行） | `AttributeLifecycleValidationScenario: PASS` |
| **回归** | 同 runner 执行既有本地场景 | `ControlState` / `Trigger` / `DamagePipeline` / `Cost` / `Cast` / `SkillPoint` 全部 `PASS` |

> 说明：上述场景均不依赖 War3 原生句柄（本地 `EntityStore` + 纯 ECS 逻辑），故通过等价宿主执行即可获得与测试客户端一致的运行结果，不需要真实 War3 客户端。

## 3. R2 Targeted 视角与 verdict

> 未获完整 `review-work` 授权；以下为与风险匹配的专项视角，以构建、运行时执行与静态审查证据支撑。

- **视角 1｜技术正确性（判据与删除安全）**：判据全部为只读值比对；`current==0` 与 `CanReclaimByOwner` 均向"保留"保守；删除前先移除 `HasAttr` 关系。运行时：新场景三类用例（声明保留 / 贡献者保留→回收 / 现值保留→归零后回收）全部通过。verdict：**通过**。
- **视角 2｜兼容性与回归**：`CreateAttr` / `AddModifier` 收窄后两项目编译 0 error，无外部调用；回收改为保守保留后，既有 6 个本地场景逐进程执行全部 `PASS`，无回归。verdict：**通过**。
- **视角 3｜确定性与结构变更约束**：删除集合按 `entity.Id` 升序；删除在迭代外；`UnitNativeSyncSnapshot` 仅存 `attrTypeId`，不缓存实体引用，无悬空风险。verdict：**通过**。

## 4. 风险与遗留

- **从不 dirty 的属性不参与回收**：显式盲区，符合"声明但未使用应保留"预期。
- **保守边界**：无 `UnitSpecData` 的单位不回收（无法判定声明），属有意取舍，不失真实目标。
- **未决后续**（另行提案）：`ModifyValue.priority` evaluation channel 语义；技能属性域 `AbilityHelper.AddModifier` 与单位域语义统一；`ModifierSourceType` 两处 namespace 重复定义。

## 5. 状态

- 批准范围（Part A + Part B）全部完成。
- 验证：两项目编译 0 error；静态检查通过；新场景 + 6 个既有场景运行时全部 `PASS`。
- 无阻塞项。状态标记为 **`已实施`**。
