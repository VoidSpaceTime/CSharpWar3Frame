# 提案：消除 Query 循环内结构变更（StructuralChangeException 系统性修复）

## 元信息

- **状态**：已实施
- **等级**：`full`
- **变更 ID**：`fix-query-loop-structural-mutation`
- **日期**：2026-09-11
- **请求来源**：仓库疏漏扫描（ULW 只读审计 + 实测复现）
- **默认实施后审查强度**：`R2 Targeted`
- **命中的审查升级触发器**：改核心业务流程 / 全局状态流转 / 跨系统协作
- **最终实施后审查强度**：`R2 Targeted`
- **完整 `review-work` 授权来源**：无（未获用户明确要求，不启用）

## 1. 背景 / Why

`Friflo.Engine.ECS 3.6.0` 在 `Query.ForEachEntity` 循环内对**结构变更**抛 `StructuralChangeException`。本仓库大量系统在循环内执行结构变更，导致相关路径在运行时直接崩溃。

### 实测证据（本次审计用仓库真实系统复现）

在 Friflo.Engine.ECS 3.6.0 上实测的精确矩阵：

| 循环内操作 | 结果 |
|---|---|
| `CreateEntity` | ✅ 不抛 |
| `DeleteEntity` | ✅ 不抛 |
| 对 `ref` 参数做字段原地写 | ✅ 不抛 |
| `AddComponent`（新增组件） | ❌ 抛 `StructuralChangeException` |
| `AddComponent`（组件已存在） | ❌ 抛 `StructuralChangeException` |
| `AddTag` / `RemoveTag` | ❌ 抛 `StructuralChangeException` |
| `RemoveComponent` | ❌ 抛 `StructuralChangeException` |

用**真实仓库系统**跑出的异常：

```
UnitLifecycleTransitionSystem(Death): THROW StructuralChangeException
DurationSystem(remaining=0.5):        THROW StructuralChangeException
UnitLifecycleDisposeSystem(Remove):   THROW StructuralChangeException
```

### 问题根源（两类）

1. **冗余写回式 `AddComponent`**：系统以为"修改实体上的组件"要 `entity.AddComponent(x)`，实际 `ForEachEntity` 的 `ref` 参数已是存储引用，字段写入即持久化。该 `AddComponent` 既多余又致命。
   - 例：`DurationSystem.cs:37`、`TimerTaskSystem.cs:37/70`、`UnitLifecycleTransitionSystem.cs:21/29`。
2. **用 `TryGetComponent(out var copy)` 取副本再 `AddComponent` 写回**：`out` 是副本，必须写回——但在循环内写回即抛异常。此类点需改为 `ref` 访问或移出循环。
3. **循环内 `AddTag`/`RemoveTag`/`RemoveComponent`**：需改为"收集 → 循环外应用"。

### 现状为何没暴露

本地验证场景只挂被测系统、且循环体常因脏标记查询为空而未执行，故从未触发。

### 规模

`War3Frame/Src/Systems` 下 `.AddComponent/.AddTag/.RemoveTag/.RemoveComponent` 共 **184 处 / 25 文件**（非全部在循环内，需逐点审计）。

## 2. 目标 / What

- 清除所有"循环内结构变更"，使相关路径不再抛 `StructuralChangeException`。
- 建立并记录统一修法规则，防止复发。
- 移除冗余写回式 `AddComponent`。
- 补充**能真正驱动循环体**的本地验证场景（死亡、Buff 到期、移动、物品、光环、特效）。

## 3. 非目标

- 不改变任何业务语义、系统边界或 order 契约。
- 不引入 `CommandBuffer`（作为备选方案记录；本变更优先用"收集后循环外应用"，与仓库既有惯例一致）。
- 不顺手实现 `TODO` 或死代码清理（另见 `remove-dead-code-and-unread-fields`）。

## 4. 影响范围

- `War3Frame/Src/Systems/*`（25 个文件，见 `tasks.md` 清单）
- `War3Frame/Src/Helpers/*` 中在查询循环内被调用的结构变更路径（间接）
- `Projects/test/`：新增/扩展验证场景

## 5. 全局影响分析

- **`War3Frame/`**：受影响。全部改动集中在此。
- **`War3Frame.Generator/`**：不受影响。不改 `[SystemRegister]` 或生成契约。
- **`FrameBuild/`**：不受影响。
- **`CSharpWar3Frame/`**：不受影响。
- **`Projects/`**：仅验证场景增量；示例行为语义不变。

## 6. 设计要点（详见 `design.md`）

统一规则：**Query 回调只允许 ① `ref` 字段原地写 ② `CreateEntity` ③ `DeleteEntity`；禁止 `AddComponent`/`AddTag`/`RemoveTag`/`RemoveComponent`。**

- **R1 冗余写回**：`entity.AddComponent(refParam)` 且组件已在查询内 → 直接删除该行。
- **R2 副本写回**：`TryGetComponent(out var copy)` → 改 `ref` 查询访问；无法改 `ref` 的 → 收集到列表循环外写回。
- **R3 加/删 Tag 或组件**：收集 `(entity, ...)` 到局部列表，循环外统一应用。
- **R4 删除实体**：引擎允许循环内 `DeleteEntity`；但删除"当前查询匹配集"内的实体时，沿用仓库既有"收集后循环外删"惯例以保确定性。

## 7. 风险与回滚

- **风险**：结构调整引入行为回归；漏改导致仍抛异常。
- **缓解**：逐文件审计 + 新增驱动循环体的验证场景 + 全量重跑既有场景。
- **回滚**：逐文件 revert；改动无数据/持久化影响。

## 8. 验证计划

1. `dotnet build War3Frame` + `Projects/test` → 0 error。
2. 新增场景**必须真正执行循环体**（构造阶段匹配实体），覆盖：
   - 单位死亡（`UnitLifecycleTransitionSystem`）；
   - `Duration` 递减到期（`DurationSystem`）；
   - Buff 计时到期（`TimerTaskSystem`）；
   - 移动命令推进（`MoveSystem`）；
   - 物品挂载/卸下（`ItemSystem`）；
   - 光环增删（`AuraSystem`）。
3. 现有全部本地场景重跑 PASS。
4. 静态：全仓 grep 复核循环内已无四类禁止操作（人工确认不在 `ForEachEntity` 内）。
5. 按 `R2 Targeted` 记录证据与 verdict，完成后写 `summary.md`。

## 9. 拆分任务

见 `tasks.md`（按文件分组，逐文件 RED/GREEN/SURFACE）。

## 10. 工件

- `proposal.md`（本文件）、`design.md`、`tasks.md`
- `specs/query-structural-safety/spec.md`

## 11. 相关文档

- `AGENTS.md`（ECS 命名 / 系统 order 契约）
- `.opencode/skills/friflo-ecs-query/SKILL.md`（循环内结构变更边界）
- `War3Frame/Src/Systems/Time/DurationSystem.cs`、`Systems/Unit/UnitLifecycleTransitionSystem.cs`
