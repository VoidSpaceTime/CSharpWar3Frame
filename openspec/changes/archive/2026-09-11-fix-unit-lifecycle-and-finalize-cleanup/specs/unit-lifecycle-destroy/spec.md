# Spec：UnitLifecycleDestroy

变更：`fix-unit-lifecycle-and-finalize-cleanup`
能力：单位生命周期销毁顺序与终态清理

## 目标

定义单位从死亡到销毁的阶段推进、原生副作用执行顺序与终态清理完整性。

## 需求

### ULD-1 阶段模型
- 阶段序列：`Alive → Death → Corpse → ClearCorpse → Remove → Disposing → (实体删除)`。
- `Disposing` 表示“原生移除已完成，ECS 待销毁”。

### ULD-2 原生移除顺序（I1/I2）
- `Death` 阶段的 `KillUnit` 必须在 `Death→Corpse` 之前执行。
- `RemoveUnit` + `HandleHelper.HandleRemove` 必须在 ECS `DeleteEntity` 之前执行，且每个单位恰好一次。
- 生命周期系统的执行顺序必须显式声明：`原生移除 < 阶段推进 < ECS 销毁`。

### ULD-3 唯一销毁执行点
- 原生单位销毁只允许出现在 `UnitRemoveNativeSystem` 一处；ECS 实体销毁只允许出现在 `UnitLifecycleDisposeSystem`（经 `CleanupFinalizeEntityDispose`）一处。

### ULD-4 终态清理完整性
- `CleanupFinalizeEntityDispose` 必须清除单位全部附属实体与关系：计时标记、光环（含 aura buff）、物品绑定、全部技能（含无 slot 的伴生技能）、属性，最后删除本体。
- 清理完成后不得残留指向该单位的悬挂关系（`ModifyTarget` / `ItemOwner` / `AbilityOwner` / `AuraBuffLink` 等）。

### ULD-5 句柄配对
- `RemoveUnit` 前相邻调用 `HandleHelper.HandleRemove`，并按 `AGENTS.md` 配对规则登记/注销。
- `NativeEntityIndex` 在该单位原生移除时注销。

## 验收

- Death 路径确实触发 `KillUnit`；直删路径确实触发 `RemoveUnit`+`HandleRemove` 且先于实体删除。
- 单位销毁后无附属实体孤儿、无悬挂关系。
- 编译 0 error，既有场景无回归。
