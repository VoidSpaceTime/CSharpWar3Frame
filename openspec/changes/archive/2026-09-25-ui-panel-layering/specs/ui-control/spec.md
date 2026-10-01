## ADDED Requirements

### Requirement: 面板层序由创建期决定

系统 SHALL 用固定层桶 `UILayer` 表达面板层序，并按层序在创建期确定 `ZIndex`。层序 SHALL NOT 在运行期改变。系统 SHALL NOT 暴露运行期提升层级的方法（例如 `Raise`、`BringToFront`）。此为匹配 War3 原生 Frame 层序等于创建序且不可重排的约束，避免命中层与视觉层分叉。

#### Scenario: 按层创建

- **WHEN** 面板按 `UILayer` 顺序（背景 → 面板 → 弹窗 → 模态 → 提示）创建
- **THEN** 后创建层的 `ZIndex` 更大，`CompareZ` 选出的命中胜者等于视觉最上层

#### Scenario: 显隐不改变层序

- **WHEN** 运行期对面板反复 `Show(true)` / `Show(false)`
- **THEN** 控件 `ZIndex` 保持不变，重叠命中胜者不因显隐而改变

#### Scenario: 无运行期提层入口

- **WHEN** 检查 UI 公共 API
- **THEN** 不存在修改已有控件 `ZIndex` 的方法，`ZIndex` 保持 `private set`

### Requirement: UIManager 按层注册面板

系统 SHALL 让 `UIManager` 按 `UILayer` 注册并创建面板。同层内的层序 SHALL 由注册顺序固定；同层面板若可能同时可见且重叠，其先后 SHALL 在设计期由注册顺序决定。注册 SHALL 只创建控件树与层序，不因创建进入命中注册表；面板初始可见性由业务显式 `Show` 决定。

#### Scenario: 同层重叠

- **WHEN** 同层两个面板都可见且包含同一鼠标点
- **THEN** `CompareZ` 胜者是后注册者，且与视觉最上层一致

#### Scenario: 注册后不自动进命中集

- **WHEN** 面板按层注册完成且业务未显式显示
- **THEN** 面板树已创建，但不在 `UIHitRegistry` 中
