# Spec：QueryStructuralSafety

变更：`fix-query-loop-structural-mutation`
能力：Query 循环结构与引擎契约安全

## 目标

定义 `Query.ForEachEntity` 回调内允许/禁止的操作，保证运行期不触发 `Friflo.Engine.ECS 3.6.0` 的 `StructuralChangeException`。

## 需求

### QSS-1 循环内允许操作
- 仅允许：`ref` 字段原地写、`CreateEntity`、`DeleteEntity`。
- 修改查询内组件字段必须通过 `ref` 参数直接写，不得用 `AddComponent` 写回。

### QSS-2 循环内禁止操作
- 禁止在 `ForEachEntity` 回调内调用 `AddComponent`（新增或已存在组件）、`AddTag`、`RemoveTag`、`RemoveComponent`。
- 违反将抛 `StructuralChangeException`，视为阻止性缺陷。

### QSS-3 结构变更延迟应用
- 需要增删组件/Tag 时，必须在回调内收集目标（`List<Entity>` 或 `List<(Entity, T)>`），在循环外统一应用。
- 收集结果应用前必须做 `IsNull` 校验，并按 `entity.Id` 升序处理以保证锁步确定性。

### QSS-4 副本写回
- 通过 `TryGetComponent(out var copy)` 取得副本后需要修改时，若该组件可加入查询则改用 `ref`；否则收集后循环外写回。

### QSS-5 验证覆盖
- 任何修复过结构变更的系统，必须有本地场景在**循环体真正执行**（存在匹配实体）的前提下验证不抛异常。
- 既有场景必须全量重跑无回归。

## 验收

- 全仓 `ForEachEntity` 回调内无 QSS-2 禁止操作。
- 新增结构安全场景全部 PASS，既有场景无回归。
- `dotnet build` 0 error。
