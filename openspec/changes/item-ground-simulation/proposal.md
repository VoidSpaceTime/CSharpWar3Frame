# 提案：地面物品特效模拟（Item Ground Simulation）

## 修订说明

**日期**: 2026-09-01  
**原因**: 根据 opus5 审查反馈修订，澄清独立特效实体引用、同帧创建+拾取边界、Attach/Detach/Destroy 视觉清理、空槽算法、ItemCreateNativeRequest 生命周期、Duration(-1) 约束、句柄配对、Native 分层、事件标记与文件路径。

**主要变更**:
1. ItemGroundVisual.effect 明确为独立特效实体引用，物品实体不挂 EffectBase。
2. 同帧 Create+Pickup 边界：允许拾取，跳过特效销毁，额外删除未消费的 ItemCreateNativeRequest。
3. Attach / EquipToUnit / BeginDestroy / ItemDestroy 路径补充：若带 ItemGroundTag 且 visual 有效，必须先销毁视觉。
4. 空槽算法：复用现有 Attach 逻辑（container.currentCount < maxSlots 或现有空槽扫描）。
5. ItemCreateNativeSystem 不调用 JassApi，改为调用 EffectHelper.CreatePosition；请求挂载方式调整为独立请求实体或 RemoveComponent，避免删请求时误删物品。
6. Duration(-1) 约束：明确 EffectHelper.CreatePosition 的 duration 参数语义。
7. 保持 Native 分层：Helper 与 Workflow 不调 War3 原生 API。
8. TriggerEventMarker：本 change 不引入新 XxxEvent 实体，不涉及此标记。
9. 文件路径：匹配现有 Components/Item、Systems/Native、Helpers 约定。
10. SystemGenerator 注册：澄清 Immediate 与非 Immediate 系统统一进入 Root，通过 Order 排序，不存在 ImmediateRoot vs Root 分裂。

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

- 地面物品 = 同一物品 ECS 实体 + `ItemGroundTag` + `Position` + `ItemGroundVisual`（模型路径与独立特效实体引用）。
- 特效表现：独立特效实体（带 `EffectBase` + `Position` + `Duration(-1)`），由 `EffectHelper.CreatePosition` 创建，`EffectNativeSystem` 负责句柄配对。物品实体**不挂** `EffectBase`。
- 将 `ItemCreateNativeRequest` 语义改为"创建地面物品表现"，由 `ItemCreateNativeSystem` 消费并调用 `EffectHelper.CreatePosition`，写回 `ItemGroundVisual.effect`，不直接调 JassApi。
- 拾取：距离内 `RequestPickup` → `ItemPickupWorkflowSystem` 校验 → 成功则销毁特效（`EffectHelper.Destroy`）+ 清引用 + `ItemAttachRequest` 转背包；同帧 Create+Pickup 边界：若 `visual.effect.IsNull`，跳过特效销毁，**额外删除未消费的 ItemCreateNativeRequest**。
- 丢弃：`Detach(..., dropToGround: true)` 补 `ItemGroundVisual` + `ItemCreateNativeRequest`。
- 地面物品直接 Attach / EquipToUnit / BeginDestroy / ItemDestroy：若仍带 `ItemGroundTag` 且 `visual.effect` 有效，**先销毁视觉**。
- 句柄配对：创建后立即 `HandleAdd`（§12 行 140-142），销毁前相邻 `HandleRemove`（§12 行 112），唯一销毁执行点在 `EffectNativeSystem`。

### 3.2 非目标

- 不调用 `CreateItem` / 不持有原生物品句柄，不做物品栏内原生物品。
- 不做掉落物物理、弹跳、被单位自动走过拾取（War3 默认拾取）。
- 不做物品被攻击、死亡、售卖原生事件。
- P0 不做 UI 按键拾取提示（P1 可选）。
- 不改 `War3Frame.Generator` 发现逻辑、不改 FrameBuild / CLI / BridgeToJIT。
- 不在本提案实现完整自定义物品栏 UI。

### 3.3 影响文件（实施阶段，本提案只写文档）

- `War3Frame/Src/Components/Item/Items.cs`：`ItemCreateNativeRequest` 字段改为 `item` / `x` / `y` / `z` / `model`，删除 `itemTypeId` / `facing`
- `War3Frame/Src/Components/Item/Items.cs` 或新增文件：`ItemGroundVisual` / `ItemGroundPickRange` / `ItemPickupRequest`
- `War3Frame/Src/Systems/Native/ItemCreateNativeSystem.cs`：落地实现，调用 `EffectHelper.CreatePosition`，不调 JassApi
- 新增：`War3Frame/Src/Systems/Item/ItemPickupWorkflowSystem.cs`（拾取工作流）
- `War3Frame/Src/Helpers/ItemHelper.cs`：新增 `CreateOnGround` / `RequestPickup`；修改 `EquipToUnit`（补地面视觉销毁）
- `War3Frame/Src/Systems/Item/ItemSystem.cs`：`ItemLifecycleOperations.Detach` 补视觉请求；`Attach` / `BeginDestroy` / `ItemCompanionDeferredDeleteSystem` 补地面视觉销毁路径
- 复用：`War3Frame/Src/Helpers/EffectHelper.cs`（`CreatePosition` / `Destroy`）、`War3Frame/Src/Systems/Native/EffectNativeSystem.cs`（句柄配对）

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

强制复用独立特效实体。`ItemGroundVisual.effect` 为独立特效实体引用，物品实体**不挂** `EffectBase`（避免 `EffectDestroyRequest` 删物品）。

`ItemCreateNativeSystem` 消费 `ItemCreateNativeRequest` 后调用 `EffectHelper.CreatePosition(model, x, y, z, duration: -1)`（**-1 表示永久，见 §13 行 19**），将返回的特效实体写回物品的 `ItemGroundVisual.effect`，然后删除请求（独立实体用 `DeleteEntity(requestEntity)`，挂物品用 `RemoveComponent`）。**禁止**直接调 `JassApi.AddSpecialEffect`。

拾取工作流 `ItemPickupWorkflowSystem` 校验 `ItemGroundTag`、距离（≤ `pickRange.radius`）、背包空槽（复用 Attach 现有逻辑：`container.currentCount < maxSlots` 或 `GetItemAtSlot` 空槽扫描，§10 行 194-228）、非 `DestroyPending`。成功则：

1. **同帧边界检查**：若 `visual.effect.IsNull`（同帧 Create+Pickup，Native 未完成），跳过 `EffectHelper.Destroy`，**额外删除未消费的 ItemCreateNativeRequest**。
2. 否则 `EffectHelper.Destroy(visual.effect)` + 清 `visual.effect` 引用。
3. 写 `ItemAttachRequest` 转入背包。

失败则删请求，不改物品状态。

丢弃时 `Detach(..., dropToGround: true)` 补 `ItemGroundVisual` + `ItemCreateNativeRequest`。

**新增强制路径**：`Attach` / `EquipToUnit` / `BeginDestroy` / `ItemCompanionDeferredDeleteSystem` 若物品仍带 `ItemGroundTag` 且 `visual.effect` 有效，**先销毁视觉**（避免从地面直接装备或销毁时句柄泄漏）。

P0：创建、显示、丢弃、拾取转背包、多路径视觉清理、句柄配对。P1：靠近时 UI 按键提示（独立 change）。

---

## 6. 风险、兼容性、迁移

- 句柄泄漏：特效创建/销毁必须走现有 `EffectNativeSystem` 配对（§12 行 118-144 创建，104-114 销毁）。`ItemCreateNativeSystem` / `ItemHelper` / 工作流**禁止**直接调 JassApi。审查清单见 design.md 第 8 节。
- 误删物品：强制要求 `ItemGroundVisual.effect` 为独立特效实体引用，禁止物品实体挂 `EffectBase`。`EffectDestroyRequest` 只删特效实体，不删物品。
- 与 `ItemUseSystem` 混淆：地面物品无 `ItemOwner`，Use 校验失败；拾取用独立 `ItemPickupRequest`。
- 同帧 Create+Pickup：若 `visual.effect.IsNull`（Native 未完成特效创建），拾取工作流跳过 `EffectHelper.Destroy`，**额外删除未消费的 ItemCreateNativeRequest**，避免后续 Native 消费产生孤儿特效。
- 多路径视觉清理遗漏：`Attach` / `EquipToUnit` / `BeginDestroy` / `ItemCompanionDeferredDeleteSystem` 若物品仍在地面且 `visual.effect` 有效，必须先拆视觉。审查清单见 tasks.md P0 工作流。
- `ItemCreateNativeRequest` 挂载方式：实施时二选一（独立请求实体或挂物品组件），确保删除请求不误删物品，并在 summary 中说明实际路径。
- `ItemCreateNativeRequest` 字段语义废弃：现有 `itemTypeId` / `facing`（§9）无调用方（仅占位系统），可直接替换。
- 回滚：恢复占位 throw；去掉新组件/系统；Detach 仍只写 Tag+Position；恢复 `ItemCreateNativeRequest` 旧字段（若有外部依赖）。

---

## 7. 验证计划

- 文档：四件套字段完整，状态 `待审核`，修订说明已补充。
- 实施后：`dotnet build War3Frame/War3Frame.csproj` 0 错误。
- 句柄审查（AGENTS.md 强制清单，design.md 第 8 节）：`ItemCreateNativeSystem` / `ItemHelper` / 工作流无直接 JassApi / DzApi / YDApi；创建 `HandleAdd`（§12 行 140-142）与销毁 `HandleRemove`（§12 行 112）相邻成对；销毁点唯一（只在 `EffectNativeSystem`）。
- 代码审查（静态）：grep 地面路径无 `CreateItem` / `JassApi.AddSpecialEffect`（Item 层）；组件注释禁止物品挂 `EffectBase`；`ItemCreateNativeRequest` 字段为 `item`/`x`/`y`/`z`/`model`，无 `itemTypeId` / `facing`。
- 逻辑走查或单元测试：创建地面 → `ItemGroundTag` + `ItemCreateNativeRequest` → Native 消费 → `visual.effect` 非空 → 拾取成功 → `ItemInventoryTag`/`ItemEquippedTag` + 特效删除 + 引用清空；超距拾取失败物品状态不变；同帧 Create+Pickup 不崩溃，未消费请求被删除；从地面直接 Attach / EquipToUnit / BeginDestroy 无句柄泄漏。
- 真实 War3 客户端看模型：默认**非阻塞**（本 proposal 审核即声明为非阻塞）。阻塞项为构建、句柄审查与代码审查。若后续用户要求真实客户端验证，另开验证记录提案。

---

## 8. 验收标准

- 地面物品创建后 ECS 为 `ItemGroundTag` + `Position` + `ItemGroundVisual`，`ItemCreateNativeRequest` 被 Native 消费后 `visual.effect` 为独立特效实体（`EffectBase` + `Position` + `Duration`）。
- 拾取距离内成功：特效实体删除、`visual.effect` 引用清空（`IsNull` 或 `default`）、物品进入背包槽位（`ItemInventoryTag` + `ItemEquippedTag` + `ItemOwner` + `ItemSlotIndex`）、无句柄泄漏。
- 同帧 Create+Pickup：允许拾取，不崩溃，未消费的 `ItemCreateNativeRequest` 被删除，无孤儿特效。
- 拾取失败（超距/无空槽/非地面/DestroyPending）：请求删除，物品状态不变（仍为 `ItemGroundTag`，`visual.effect` 仍在）。
- 从地面直接 Attach / EquipToUnit / BeginDestroy / ItemDestroy：视觉先销毁，无句柄泄漏。
- 不出现 `CreateItem` / 原生物品句柄。`ItemCreateNativeSystem` 不直接调 `JassApi.AddSpecialEffect`，只调 `EffectHelper.CreatePosition`。
- `ItemCreateNativeSystem` 不再 `NotImplementedException`。
- `dotnet build War3Frame/War3Frame.csproj` 0 错误。
- 句柄审查清单（design.md 第 8 节）全通过。
- 组件注释禁止物品实体挂 `EffectBase`。

---

## 9. 拆分任务

见 `tasks.md`。P0 核心模拟，P1 UI 提示。
