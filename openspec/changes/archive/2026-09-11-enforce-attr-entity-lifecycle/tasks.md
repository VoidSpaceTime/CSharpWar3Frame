# 任务清单：属性实体生命周期完整化

对应提案：`proposal.md`（`enforce-attr-entity-lifecycle`，`full`）
状态：`实施中`（编译 + 静态验证通过；新增场景已就位，War3 运行时执行待完成）

## 0. 审核前置

- [x] 用户确认范围：Part A + Part B
- [x] 用户确认 Part A 收窄 `AttributeHelper.CreateAttr` 可见性

## 1. Part A：唯一性 + 入口收敛

- [x] `War3Frame/Src/Helpers/AttributeHelper.cs`：
  - [x] `CreateAttr` 收窄为 `internal`
  - [x] 创建原语内加防御性幂等：先 `TryGetAttr`，命中直接返回既有实体（强制 INV-1）
  - [x] `GetOrCreateAttr` 保持为对外唯一获取/创建入口
- [x] `War3Frame/Src/Helpers/ModifyHelper.cs`：
  - [x] `AddModifier(attrEntity, ...)` 收窄为 `internal`
  - [x] `AddModifierToUnit` 保持 public，内部 `GetOrCreateAttr` + 委托
- [x] 调用点核对（无需代码改动，可见性收窄不影响同程序集调用）：
  - [x] `War3Frame/Src/Helpers/UnitSpecBuilder.cs`
  - [x] `War3Frame/Src/Systems/LevelExperienceSystem.cs`
- [x] 静态核对：全仓无 `AddModifier(attrEntity)` 外部调用；`CreateAttr` 无跨程序集引用

## 2. Part B：`AttrCalculationSystem` 回收逻辑

- [x] `War3Frame/Src/Systems/Attribute/AttrCalculationSystem.cs`：
  - [x] 重算后追加回收判定
  - [x] 判据：无 `ModifyTarget` 入边 + `baseValue==0` + `current==0` + `flatBonus==0` + `percentBonus==0` + `CanReclaimByOwner`
  - [x] `CanReclaimByOwner`：owner 必须带 `UnitSpecData`；未声明 → 可回收；无归属/无模板 → **保守保留**
  - [x] 循环外删除：`toReclaim` 按 `entity.Id` 升序
  - [x] 删除序列：`owner.RemoveRelation<HasAttr, Entity>(attr)` → `attr.DeleteEntity()`；`IsNull` 校验
  - [x] 确认无迭代内结构变更

### 与提案的判据收紧（已同步文档）

- 提案原写"owner 无 `UnitSpecData` 视为未声明 → 可回收"。
- 实现改为**保守保留**：无 `UnitSpecData` 时无法判定声明，不回收。
- 原因：本地/工具单位（无模板）的 base=0 常驻属性会被误回收；该收紧不损失真实目标（模板单位始终带 `UnitSpecData`）。
- 已同步：`proposal.md` §6.1、`design.md` §4.1/§5、`spec.md` AEL-2/AEL-3。

## 3. 验证

- [x] `dotnet build War3Frame/War3Frame.csproj` → 0 error
- [x] `dotnet build Projects/test/test.csproj` → 0 error
- [x] 新增验证场景 `Projects/test/Scripts/Process/AttributeLifecycleValidationScenario.cs`，覆盖：
  - [x] 声明属性（base 0 / base 100，无 modifier）→ 保留
  - [x] 未声明属性 + modifier → 保留；移除 modifier → 回收
  - [x] 未声明属性 + current!=0 → 保留；current 归零 → 回收
  - [x] 回收后 `HasAttr` 关系已移除
- [x] 场景已注册到 `Projects/test/Program.cs`
- [x] **场景运行时执行**：本地场景均为 native-independent，通过临时宿主运行器逐进程执行：
  - [x] `AttributeLifecycleValidationScenario: PASS`
  - [x] 回归：`ControlState` / `Trigger` / `DamagePipeline` / `Cost` / `Cast` / `SkillPoint` 全部 `PASS`
- [x] 静态检查：
  - [x] 全仓 grep 无迭代内删除模式
  - [x] `IsDeclaredByOwner` 无残留；`CreateAttr` 无外部真实调用
  - [x] 确认无系统长期缓存 attr `Entity`（`UnitNativeSyncSnapshot` 仅存 `attrTypeId`）

## 4. 总结与收尾

- [x] 写 `summary.md`（记录改动、验证覆盖、R2 证据、待办）
- [x] `proposal.md` 状态更新为 `已实施`
- [ ] 未决项（`priority` channel、技能域统一、`ModifierSourceType` 重复）转后续提案

## 5. 阻塞与待办

- [x] 无阻塞项（运行时验证已执行）。
- [ ] 未决后续项（另行提案）：见 `summary.md` §4。
