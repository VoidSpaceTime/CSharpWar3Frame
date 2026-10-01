## 状态

- 状态：待审核
- 等级：light（纯评估；不改行为契约）
- 复盘强度：R0 Direct（仅文档与 `openspec validate`）
- 范围：只评估持久化（存档 / 读档）是否值得纳入框架。本 change 不包含实现、不写 tasks、不写 spec delta。

## 背景

Todo 31（W5.7）要求回答：框架要不要做存档。系统清单结论是 persistence 域 0 系统。本提案只冻结现状、候选需求、与 ECS / 版本化身份的关系、成本和建议，供用户裁决。

裁决前不创建存档组件、序列化系统、读档重建系统或测试。若裁决为纳入，必须另立实施提案；本 change 不预写实施任务。

## 现状证据

绑定存在，运行时没有存档系统，也没有调用方。

| 事实 | 位置 | 含义 |
|---|---|---|
| 战役存档 | `JassApi.cs:787-792` 函数指针；`6686-6721` `LoadGame` / `SaveGame` / `CopySaveGame` / `SaveGameExists` | 整局进度快照。方法体只在 `Library/Api` |
| 本地 GameCache | `JassApi.cs:820-823`、`6867-6893` `ReloadGameCachesFromDisk` / `InitGameCache` / `SaveGameCache` / `StoreInteger` | 注释写明只对单机有效，用于战役关卡间传值。无调用方 |
| 局内 Hashtable | `JassApi.cs:851-859`、`7093-7212` `SaveInteger` / `SaveReal` / `SaveBoolean` / `SaveStr` / `SaveUnitHandle` | 当前局内存表，不是跨局存档。`SaveUnitHandle` 保存的是本局原生句柄 |
| 平台服存档 | `KKApi.cs:544-553` `DzAPI_Map_SaveServerValue` / `DzAPI_Map_GetServerValue`；`868-982` `DzAPI_Map_StoreInteger*` 等包装 | 按玩家 key/value 写平台服，单值最长 64。无调用方 |
| 平台随机存档计数 | `KKApi.cs:1574` `KKApiRandomSaveGameCount` | 绑定存在，无调用方 |
| 唯一 `SaveInteger` 使用 | `KKApi.cs:3171-3174` | 写入局内 `JHashtable` 做时间戳，不是存档 |

检索结论（2026-10-01，`ECS-Framework`）：

- `War3Frame/Src` 与 `Projects/` 中没有 `SaveGame`、`LoadGame`、`InitGameCache`、`StoreInteger`、`DzAPI_Map_SaveServerValue`、`DzAPI_Map_GetServerValue` 的调用。
- `[SystemRegister]` 系统清单（`War3Frame/Src/Systems`）没有 persistence / save / load 系统。
- `War3Frame/Src` 检索 `inventory` / `Inventory` / `背包`：命中的是物品使用、物品系统、`ItemHelper` 和同步示例注释，没有背包存档或库存序列化。
- 两处「持久」字样不是存档：`TimerTaskSystem.cs:44,75` 指 ref 字段原地写回；`AbilityEffect.cs:127` 指 `TargetFilter` 位值不要复用，避免外部按整数存档时错位。框架自己不存这个枚举。

对照已落地的原生对象（本提案不改这些文件）：

- 单位：`UnitCreateNativeSystem.cs:30-32` 在 `CreateUnit` 后相邻 `HandleAdd` + `NativeEntityIndex.Register`，再把 `JUnit` 写入 `UnitNative`（`Units.cs:56-60`）。
- 句柄反查：`NativeEntityIndex.cs:16-22` 是进程内 `Dictionary<int, Entity>`，键是本局 `GetHandleId`。
- 特效 / 玩家同样把 `JEffect` / `JPlayer` 放在 `EffectNative` / `PlayerNative` 里。这些字段离开当前进程就无效。

## 候选需求

以下都是可能用法，不是本 change 的实施范围。

1. **War3 `SaveGame` / `LoadGame`。** 保存整局原生进度（战役 / 单机续关）。它拍的是 War3 世界，不是 ECS Store。读档后原生单位、物品、特效由引擎重建，句柄全部换新；框架侧 ECS 不会跟着恢复。
2. **GameCache。** 单机、关卡之间传少量整数 / 实数 / 字符串。注释已限制为单机。不能携带 `JUnit` 作为跨局身份。
3. **平台服 key/value（`DzAPI_Map_*`）。** 按玩家存短字符串，适合等级、货币、解锁标记。不适合整份 ECS 快照，也不在 1.27 纯 Jass 基线内。
4. **自定义 ECS 快照。** 只序列化可重建的语义：组件字段、Tag、关系的目标键。加载时新建实体，再经 Native 层按创建请求重建单位 / 物品 / 特效。这是唯一和「ECS 为真相」一致的路径，也是成本最高的路径。
5. **显式不做。** 不把 `SaveUnitHandle`、`NativeEntityIndex`、`UnitNative.unit` 当作跨会话身份。不在业务系统里直接调 `SaveGame` 或 `DzAPI_Map_SaveServerValue`。

### 与 ECS 模型的关系

可保存的是语义，不是本局代理：

| 种类 | 跨会话 | 原因 |
|---|---|---|
| 普通 `IComponent` 的值字段（生命、等级、冷却剩余、名称） | 可以，但要白名单 | 字段本身无句柄；默认全量序列化会把 Request / Dirty / 快照一起写出去 |
| `ITag` | 可以 | 零数据分类；加载时按名单重新 `AddTag` |
| `ILinkComponent` / Relation（`UnitOwner`、`ItemOwner`、`ItemActiveAbility` 等） | 只能存稳定键，不能存 `Entity` | `Entity` 是本 Store 的 id + revision |
| `UnitNative` / `EffectNative` / `PlayerNative` | 不可以 | 内含 `JUnit` / `JEffect` / `JPlayer`，本局句柄 |
| Request / Command / Outcome / Event / Dirty | 不可以 | 一次性意图、本帧事实或待同步标记，不是长期真相 |
| 委托、注册表、系统实例 | 不可以 | 代码身份，不是数据 |

实体身份不能跨会话沿用。Friflo `Entity.Id` 由当前 `EntityStore` 分配；删除后 id 会复用。加载必须 `CreateEntity` 得到新 id，再用一张「存档键 → 新 Entity」的表把关系接回去。存档键应是业务稳定 id（玩家槽、模板名、物品实例 guid），不是 `Entity.Id`。

### 版本化身份能复用什么

`openspec/specs/entity-sync-identity/spec.md` 的 Purpose 写明：token 只服务**当前游戏会话**，防止被回收的 id 被旧同步消息命中。

可复用的是边界，不是 token：

- 编码只接受当前 `SyncHelper.Store` 的存活实体；解码校验 id 与 `Revision`；不接受裸 id（`SyncHelper.cs:71-95`）。
- 回归 `sync-identity-and-input`（`DomainRegression.cs:161-186`）已证明：实体删除、id 被复用后，旧 token 解不出新实体；跨 Store 编码直接拒绝。
- 规格原文：「token 仅用于当前会话，不作为跨 Store、跨会话或无限次 ID 复用后的永久身份。」
- 协议迁移要求发送接收同版本，旧格式直接拒绝，不恢复有碰撞风险的 id 查找。

因此 `e1:{id}:{revision}` **不能**写入存档再在下一局解码。若另立实施提案，跨会话身份必须另做稳定键；当前 token 继续只做局内同步。版本号思路可以借鉴（存档格式带版本，不匹配就拒绝），但那是存档 schema 版本，不是 Friflo revision。

## 成本与风险

- **序列化范围：** Friflo 组件是结构体，能被反射扫到，但不存在现成的存档序列化器，仓库也没有使用 Friflo 序列化 API。全量 dump 会写入原生句柄、事件实体和脏标记。必须先有显式白名单，否则读档无法重建。
- **加载时的原生重建：** ECS 快照恢复后，单位 / 物品 / 特效在 War3 里并不存在。必须重新走 Native 层：`UnitCreateNativeRequest` → `CreateUnit` → `HandleAdd` + `Register` → 回写新的 `UnitNative`。不能把旧 `JUnit` 写回去，也不能在加载系统里直接调 `CreateUnit`。特效、物品同理。`NativeEntityIndex` 是空表，要随新建句柄重新登记。
- **关系重接：** `UnitOwner.player`、`ItemOwner.unit`、`ItemActiveAbility.ability` 都是本局 `Entity`。加载顺序必须先建被引用方，再按存档键回填；顺序错了链接会指向空实体或错实体。
- **版本与迁移：** 组件字段改名、拆分、删除后，旧档要么迁移要么拒绝。`TargetFilter` 已把「位值不复用」写成注释，说明外部整数布局一变就会错位。没有迁移器之前，不兼容就应拒绝读取。
- **多人 / 锁步：** 框架同步是 `DzSyncData` 局内消息（`AsyncHelper` / `SyncHelper`），不是存档通道。若存档只在单机或平台服按玩家读写，不进入锁步。若在联机对局里读档并改 ECS，必须当成一次全体一致的状态替换，不能只让一个客户端读本地文件。`SaveGame` 会切掉当前局，和锁步会话不兼容。
- **平台与 1.27：** `DzAPI_Map_SaveServerValue` 依赖平台扩展，单值 64 字符，有错误码（`KKApi.cs:608`）。1.27 基线只能考虑 `SaveGame` 或 GameCache，且二者都恢复不了 ECS。
- **爆炸半径（若另立实施提案）：** 主要在 `War3Frame/`：白名单、存档文档、加载编排、Native 创建请求。`War3Frame.Generator/` 只在新增 `[SystemRegister]` 系统时多注册，不改生成器。`FrameBuild/`、`CSharpWar3Frame/` 不受影响。`Projects/` 只在批准实施后补场景。`entity-sync-identity` 规格不改。
- **本 change 的风险：** 无代码风险。唯一风险是把评估误读成已批准实施。

## 建议

**建议：不纳入当前框架范围。** API 绑定保留，不新增存档域。

理由：

- 没有任何调用方、示例或测试在存档或读档。persistence 域是 0 系统。
- `SaveGame` 恢复的是 War3 原生局，不是 ECS。读档后句柄全变，框架真相对不上。
- 自定义快照的最小闭环并不小：白名单、稳定键、关系重接、版本拒绝、以及经 Native 层重建单位 / 物品 / 特效。
- 版本化同步 token 已明确拒绝跨会话复用。把它当成存档 id 会直接违反 `entity-sync-identity`。
- 平台服 key/value 适合地图玩法自己的几个整数，不适合做成框架级 ECS 持久化。

若用户否决本建议、决定纳入，另立实施提案，且第一阶段只做：自定义语义白名单 + 存档格式版本（不匹配即拒绝）+ 稳定键重接关系 + 经现有 Native 创建请求重建单位。不调用 `SaveGame` / `LoadGame`，不持久化 `Entity.Id` / revision / 原生句柄，不把 `DzAPI_Map_*` 当作默认后端。

## 待裁决项

**待用户裁决。** 本提案不代用户选择。

- 选项 A：不纳入。归档本评估，不另立实施提案。
- 选项 B：纳入。另立实施提案，范围限于上一节的第一阶段。
- 选项 C：维持现状并挂起。不归档、不实施，等出现具体玩法再开。

未收到用户回复前，本 change 停在审核门。

## Capabilities

### New Capabilities

- 无。纯评估，不新增当前系统行为契约。

### Modified Capabilities

- 无。`.openspec.yaml` 设置 `skip_specs: true`。没有可归档的行为 delta。

## Impact

- `War3Frame/`：仅作证据。不改 API、组件、系统、Helper 或同步身份。
- `War3Frame.Generator/`：无影响。未新增 `[SystemRegister]` 类型。
- `FrameBuild/`：无影响。
- `CSharpWar3Frame/`：无影响。
- `Projects/`：无影响。未加示例或回归场景。
- `openspec/specs/`：无变更。`entity-sync-identity` 只被引用，不修改。

## 验收标准

- `proposal.md` 覆盖背景、现状证据、候选需求、身份复用、成本与风险、建议、待裁决项。
- 证据含文件与行号，并区分「绑定存在」与「运行时未使用」。
- 写明 `entity-sync-identity` 的 token 不可跨会话复用，以及加载时必须经 Native 层重建句柄。
- 建议明确，同时标为「待用户裁决」，不把建议写成已批准。
- `openspec validate persistence-eval` 通过。
- 不创建实现代码、`tasks.md` 实施清单或 spec delta。
