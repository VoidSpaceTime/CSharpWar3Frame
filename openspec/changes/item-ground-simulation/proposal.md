# 提案：地面物品特效模拟（Item Ground Simulation）

## 0. 基本信息

- Change ID: `item-ground-simulation`
- 提案等级: `full`
- 状态: `待审核`
- 日期: 2026-08-31
- 目标一句话: 落地 `ItemCreateNativeSystem` 占位，用 ECS 实体 + 特效表现（可选 UI 拾取提示）替代 War3 原生物品作为地面物品方案。
- 请求来源: 用户确认按 full 级推进地面物品模拟。
- 默认实施后审查强度: `R2 Targeted`
- 命中的审查升级触发器: 跨 Item / Native / UI 区域；核心业务流程（地面创建、拾取转背包）；Native 句柄配对。
- 最终实施后审查强度: `R2 Targeted`（Native 句柄泄漏与跨系统状态协作；不升 `R3`：非安全敏感、非架构级基础设施替换）
- Oracle 可用性与 `R1` 回退方式: 实施后技术准确性复核优先 Oracle；不可用时由代码审查核对句柄配对与 Tag 流转，并记录替代原因。
- 完整 `review-work` 授权来源: 无

### 0.1 工件矩阵

- 本 change 为 `full`：`proposal.md`、`design.md`、`tasks.md`、`specs/item-ground.md`

### 0.2 总结深度矩阵

- 实施完成后写完整 `summary.md`，覆盖改动范围、全局影响、验证、风险与后续。

---

## 1. 分级判定

### 1.1 为什么是这个等级

- 影响范围: Item 组件与请求语义、Native 创建/销毁、拾取工作流、Helper 入口；P1 触及 UI 拾取提示。
- 风险等级: 中高。Native 特效句柄泄漏、地面/背包 Tag 错乱、与现有 `ItemUseSystem` 使用路径混淆。
- 可逆性: 可回滚到占位 `NotImplementedException` 与现有四态 Tag 语义，但实施后若已有地图依赖地面表现则需同步回退。
- 是否跨项目: 否。仅 `War3Frame/` 运行时。
- 是否改公共契约: 是。`ItemCreateNativeRequest` 语义从“原生物品 CreateItem”改为“创建地面物品表现”；`ItemHelper` 新增地面创建/拾取入口。

### 1.2 升级触发器检查

- [ ] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约（新系统用现有 `SystemRegisterAttribute`，生成器逻辑不变）
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [ ] 涉及 `Projects/` 示例或集成验证行为（本提案不改 demo/test；可选后续验证提案）
- [x] 涉及公共 API / 数据结构 / 配置契约（`ItemCreateNativeRequest`、Helper 签名、地面视觉组件）
- [ ] 涉及架构边界、目录结构、依赖关系重组

用户已确认按 `full` 处理（跨 Item / Native / UI）。

### 1.3 实施后审查升级触发器

- [x] 公共 API、生成器输出、配置格式、构建链或发布契约（Helper / Request 语义）
- [ ] 持久化、迁移、数据兼容性或数据丢失风险
- [x] 性能回归、资源泄漏、实时性或大规模数据影响（特效句柄配对）
- [x] 多系统、多项目或跨边界状态协作（Workflow + Native + Effect）

`R3` 条件均未命中。

---

## 2. 背景 / Why

`ItemCreateNativeSystem` 当前对 `ItemCreateNativeRequest` 直接 `throw NotImplementedException()`。请求字段仍按原生物品建模（`itemTypeId`、`facing`），与仓库已选定方向冲突：注释已写明“决定走 UI+特效”。

War3 原生物品（`CreateItem`）带攻击、死亡、拾取触发、物品栏句柄等复杂语义，地图模组里容易和自定义背包、技能 companion、生命周期打架。xlik 一类 Lua 框架用特效模型模拟地面物品，避开原生物品缺陷。本仓库已有：

- 四态 Tag：`ItemGroundTag` / `ItemInventoryTag` / `ItemEquippedTag` / `ItemStoredTag`
- `ItemLifecycleOperations.Detach` 丢弃时已写 `ItemGroundTag` + `Position`，但没有视觉与拾取闭环
- `EffectHelper.CreatePosition` + `EffectNativeSystem`：`AddSpecialEffect` 后 `HandleAdd`，销毁前 `HandleRemove`
- `ItemUseSystem` 只处理已有 owner 的使用请求，不是地面拾取

需要把“地面物品”定义为 ECS 真相 + 特效代理，而不是原生物品句柄。

---

## 3. 变更范围 / What

### 3.1 目标

- 地面物品 = 同一物品 ECS 实体 + `ItemGroundTag` + `Position` + 特效表现（复用 Effect 体系或物品实体上的视觉组件，由 Native 创建 `AddSpecialEffect`）。
- 将 `ItemCreateNativeRequest` 语义改为“创建地面物品表现”，由改造后的 `ItemCreateNativeSystem`（或拆出的地面视觉 Native 系统）消费。
- 拾取：距离内选中地面物品实体 → 销毁/隐藏特效并注销句柄 → 去掉 `ItemGroundTag` → 走现有 `ItemAttachRequest` 转入背包。
- 丢弃：现有 `DropToGround` / `Detach(..., dropToGround: true)` 补写视觉创建请求。
- 句柄：创建后立即 `HandleAdd`，销毁前相邻 `HandleRemove`，唯一销毁执行点在 Native 层。

### 3.2 非目标

- 不调用 `CreateItem` / 不持有原生物品句柄，不做物品栏内原生物品。
- 不做掉落物物理、弹跳、被单位自动走过拾取（War3 默认拾取）。
- 不做物品被攻击、死亡、售卖原生事件。
- P0 不做 UI 按键拾取提示（P1 可选）。
- 不改 `War3Frame.Generator` 发现逻辑、不改 FrameBuild / CLI / BridgeToJIT。
- 不在本提案实现完整自定义物品栏 UI。

### 3.3 影响文件（实施阶段，本提案只写文档）

- `War3Frame/Src/Components/Item/Items.cs`：请求字段改为模型路径、拾取半径等
- `War3Frame/Src/Components/Item.cs`：地面视觉/拾取范围组件（若新增）
- `War3Frame/Src/Systems/Native/ItemCreateNativeSystem.cs`：落地或改为委托 Effect Native
- 可能新增：地面视觉销毁 Native、拾取工作流系统
- `War3Frame/Src/Helpers/ItemHelper.cs`：`CreateOnGround` / `TryPickup`
- `War3Frame/Src/Systems/Item/ItemSystem.cs`：Detach 后补视觉请求
- 复用：`EffectHelper`、`EffectNativeSystem`（优先复用，避免第二套特效句柄路径）

---

## 4. 全局影响分析

- `War3Frame/`：**受影响**。Item 请求语义、Native 占位落地、拾取工作流、Helper 入口。
- `War3Frame.Generator/`：**不受影响**。新系统继续用 `[SystemRegister]`，不改生成器。
- `FrameBuild/`：**不受影响**。无构建/模板/资源管线变更。
- `CSharpWar3Frame/`：**不受影响**。无 CLI 参数或入口行为变更。
- `Projects/`：**本提案不改**。demo/test 可在后续提案加场景；不作为本 change 阻塞项。
- `BridgeToJIT/` / `FastMDX/` / `ModelFormat/`：**不受影响**。不改 native bridge 或模型格式。

---

## 5. 方案摘要

优先复用特效实体或在物品实体上挂 `EffectBase` + `Position`，让现有 `EffectNativeSystem` 创建/同步/销毁特效，避免 `ItemCreateNativeSystem` 再开一条 `AddSpecialEffect` 路径。

若物品实体不能同时作为 Effect 查询主体（生命周期、Duration、Destroy 会误删物品），则：物品实体只持有 `ItemGroundVisual`（模型、特效实体引用或 pending 标记），由 Item Native 写 `EffectHelper.CreatePosition(..., duration: -1)`，拾取时 `EffectHelper.Destroy`。`ItemCreateNativeSystem` 不再 throw，改为消费“确保地面视觉存在”的请求，或降为薄转发。

拾取不走 `ItemUseSystem`（那是 companion 施法）。新增 `ItemPickupRequest` + 工作流：校验 `ItemGroundTag`、距离、背包空槽，再 `ItemAttachRequest`。

P0：创建、显示、丢弃、拾取转背包、句柄配对。P1：靠近时 UI 按键提示。

---

## 6. 风险、兼容性、迁移

- 句柄泄漏：特效创建/销毁必须走现有 Effect Native 配对，或 Item Native 内相邻成对。审查清单见 design.md。
- 误删物品：禁止对物品实体直接 `EffectDestroyRequest` 导致 `entity.DeleteEntity()`。视觉必须是独立特效实体，或销毁路径只拆特效不删物品。
- 与 `ItemUseSystem` 混淆：地面物品无 `ItemOwner`，使用请求应失败；拾取用独立 Request。
- `ItemCreateNativeRequest.itemTypeId` 语义废弃：无调用方（仅占位系统），可直接改字段；若保留兼容字段须在 design 标明且默认不走 CreateItem。
- 回滚：恢复占位 throw；去掉新组件/系统；Detach 仍只写 Tag+Position。

---

## 7. 验证计划

- 文档：四件套字段完整，状态 `待审核`。
- 实施后：`dotnet build War3Frame/War3Frame.csproj` 0 错误。
- 代码审查：创建后 `HandleAdd`、销毁前 `HandleRemove`；无 `CreateItem`。
- 逻辑：创建地面 → 有 `ItemGroundTag` + 视觉 → 拾取成功转 `ItemInventoryTag`/`ItemEquippedTag` 且特效销毁；超距拾取失败。
- 真实 War3 客户端看模型：默认**非阻塞**（本提案审核即声明）。阻塞项为构建与句柄审查。若后续用户要求客户端验证，另开验证记录。

---

## 8. 验收标准

- 地面物品创建后 ECS 为 `ItemGroundTag` + `Position`，并有特效表现意图（独立 Effect 实体或等价 Native 创建）。
- 拾取距离内成功：视觉消失、句柄注销、物品进入背包槽位。
- 不出现 `CreateItem` / 原生物品句柄。
- `ItemCreateNativeSystem` 不再 `NotImplementedException`。
- `dotnet build War3Frame/War3Frame.csproj` 0 错误。
- 句柄配对审查通过。

---

## 9. 拆分任务

见 `tasks.md`。P0 核心模拟，P1 UI 提示。
