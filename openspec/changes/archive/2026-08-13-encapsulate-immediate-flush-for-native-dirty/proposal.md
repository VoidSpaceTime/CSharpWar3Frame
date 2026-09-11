## Why

当前业务层直接出现了 `Game.ImmediateRoot.Update(default(UpdateTick))` 这样的调用，导致业务代码暴露了 immediate 调度细节。同时，`UnitNativeDirtyHelper.Mark(...)` 目前只负责写脏位，缺少一个专门服务于“标记后立即落地”的统一入口，容易让立即型生命周期逻辑在业务层分散实现。

## What Changes

- 在 `Game` 上新增统一的 immediate flush 封装入口。
- 在 `UnitNativeDirtyHelper` 上新增仅用于立即型 dirty 的便捷入口。
- 将现有业务层对 `ImmediateRoot.Update(default(UpdateTick))` 的裸调用收口到上述封装中。
- 保持普通 `Mark(...)` 的语义不变，不让所有 dirty 标记都自动触发 immediate update。

## Capabilities

### New Capabilities
- `immediate-flush-encapsulation`: 定义 immediate system 刷新如何通过统一封装暴露给 native dirty 生命周期逻辑使用。

### Modified Capabilities

## Impact

- 主要影响 `War3Frame/initialization/ECSInit.cs`、`War3Frame/Src/Components/Units.cs`、`War3Frame/Src/Helpers/UnitHelper.cs`。
- 不改变 `HealthSystem`、`ManaSystem`、`UnitNativeSystem` 的 interval 同步语义。
- 不改变创建/死亡的业务结果，只改变 immediate flush 的调用位置和封装方式。
