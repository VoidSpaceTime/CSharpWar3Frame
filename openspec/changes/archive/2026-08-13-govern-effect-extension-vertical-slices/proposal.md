# 治理 Effect 扩展 vertical slice 与执行边界

## 0. 基本信息

- Change ID: `govern-effect-extension-vertical-slices`
- 提案等级: `architecture`
- 目标一句话: 将 `EffectChainBuilder` 固化为新增效果能力的唯一 authoring 扩展入口，并要求每种效果按 authoring、spec、ECS resolver、Native/Execution、验证的完整 vertical slice 落地。
- 请求来源: Effect Builder 已完成收层，后续需要避免再次出现平行 Builder、helper 持有长期语义或业务系统直接执行 War3 native 副作用。

## 1. 分级判定

### 1.1 为什么是 `architecture`

- 影响范围: 定义 `War3Frame` 内 Effect authoring、数据契约、语义解释、ECS intent、Native/Execution 与验证的长期扩展边界。
- 风险等级: 高；虽然本 change 先建立治理工件，但后续按该规则实施的效果能力可能改公共 API、核心执行流程与跨模块协作。
- 可逆性: 中；规则本身可回滚，但一旦多个效果能力按新规则落地，逆转会产生迁移成本。
- 是否跨项目: 主要约束 `War3Frame`，并要求 `Projects/*` 提供集成示例；其他项目必须完成非影响检查。
- 是否改公共契约: 本轮只新增规格，但规格约束未来公开 `EffectChainBuilder` 和 `EffectSpec` 扩展。
- 是否涉及架构边界: 是，明确 authoring、semantic resolver、ECS truth 与 native execution 的职责及依赖方向。

### 1.2 升级触发器检查

- [x] 涉及 `War3Frame/` 与 `Projects/*` 联动验证
- [ ] 当前不修改 `War3Frame.Generator/` 输出，但未来新增系统必须复核注册影响
- [ ] 当前不修改 `FrameBuild/`，但每个实现 change 必须复核构建影响
- [ ] 当前不修改 `CSharpWar3Frame/`，但每个实现 change 必须复核初始化/CLI 影响
- [x] 涉及公共 API / 数据结构 / 核心效果流程
- [x] 涉及架构与分层边界

## 2. 背景 / Why

当前 Effect 体系已经具备 `Damage`、`Heal`、`Buff`、`Area`、`Line`、`GroundArea`、`Projectile`、视觉特效和 Projectile arrive 嵌套链。`consolidate-effect-builder-authoring` 又将公开 Builder 收敛为 `EffectChainBuilder`。

但如果后续每种新效果只解决局部需求，仍可能重新出现：

- 为某个 Ability、Item、Aura 或 Projectile 单独新增平行 Builder。
- Builder API 已存在，但 `EffectSpec` 或 resolver 没有完整语义，形成“可写不可执行”的假能力。
- 业务系统或 helper 为了快速实现而直接调用 War3 native API。
- ECS 与 native handle 同时持有长期语义真相。
- 新效果只有模板示例，没有 builder/spec/resolver 的结构验证与 Native 边界检查。

仓库已有 `formalize-ecs-native-effect-separation`，它定义了 ECS/native effect ownership。本 change 不重复该架构，而是在其上补充“新增 Effect 能力如何进入系统”的治理闭环。

## 3. 变更范围 / What

- 新增 `effect-extension-vertical-slice` capability 规格。
- 规定 `EffectChainBuilder` 是普通 Effect authoring 的唯一公开扩展 Builder。
- 规定新效果能力必须形成以下闭环：
  1. Authoring API
  2. `EffectSpec` / step payload
  3. 语义 resolver / ECS intent
  4. Native/Execution（仅在确有原生副作用时）
  5. builder/spec 测试、运行时测试与 `Projects/*` 示例
- 规定新增 native 调用必须进入 `Systems/Native/*`、`*NativeSystem` 或 `*ExecutionSystem`，并说明无法复用既有执行层的理由。
- 规定每个具体效果能力仍需独立 OpenSpec change；本 change 不授权批量实现所有候选效果。

## 4. 全局影响分析

- `War3Frame/`: 主要受约束区域；未来 Effect API、spec、resolver、ECS intent 和 Native/Execution 实现均必须遵守 vertical-slice 准入规则。
- `War3Frame.Generator/`: 本 change 不修改；未来新增 `[SystemRegister]` 系统时必须验证生成注册和执行顺序。
- `FrameBuild/`: 本 change 不修改；未来若效果资源、模板生成或发布资产受影响，应在具体实现 change 中升级范围。
- `CSharpWar3Frame/`: 本 change 不修改；未来若新增初始化参数、配置或 CLI 行为，应在具体实现 change 中单独审查。
- `Projects/`: 作为集成消费方，未来每个新增效果能力至少提供一个真实可编译示例；需要真实 War3 环境的表现验证应明确列为手测项。

## 5. 架构目标

- 保持一个公开效果链 Builder，不因领域入口不同而复制 Builder。
- 让“能被模板声明”与“能被运行时正确解释”保持一一对应。
- 让 ECS / workflow 持有长期语义，Native/Execution 只执行副作用。
- 让新效果可通过结构测试、运行时测试和示例构建获得可重复验证。
- 允许具体能力按独立 vertical slice 增量落地，而不是一次性重写整个 Effect 系统。

## 6. 候选方案比较

### 方案 A：按领域建立平行 Builder

- 为 Ability、Item、Aura、Projectile 等分别维护效果 Builder。
- 优点: 每个入口可快速定制。
- 缺点: API 重复、认知层级反弹、同一效果在不同入口产生语义漂移。
- 结论: 拒绝。

### 方案 B：只有通用 `EffectSpec`，调用方手工构造数据

- 优点: 数据层最直接，新增 API 少。
- 缺点: 模板作者必须理解底层 payload 和 fallback 字段，难以保证结构合法性。
- 结论: 仅保留为高级/内部入口，不作为普通 authoring 方案。

### 方案 C：统一 `EffectChainBuilder` + vertical-slice 准入规则

- 优点: authoring 入口稳定；每个能力必须同时完成 spec、resolver、执行边界和验证；可逐个能力增量实施。
- 缺点: 新增效果前需要更多设计与测试工作。
- 结论: 推荐。

## 7. 阶段拆分

- Phase 1: 批准本治理规格；不改运行时代码。
- Phase 2: 盘点现有效果 step，记录 authoring/spec/resolver/native/test 覆盖矩阵，识别缺口但不在本 change 中顺手修复。
- Phase 3: 对每个新增或补齐的具体效果能力建立独立 OpenSpec，并按 vertical slice 实施。
- Phase 4: 在多个能力验证成熟后，清理确认冗余的 helper 路径或重复入口；清理仍需独立审核。

## 8. 风险、兼容性与迁移

- 风险: 治理规则过重，阻碍小效果能力迭代。
  - 控制: 具体能力仍按风险分级；不涉及公共契约和 native 的局部 step 可用 `light`，但必须满足闭环检查。
- 风险: 与 `formalize-ecs-native-effect-separation` 重复或冲突。
  - 控制: `ecs-native-effect-separation` 是长期 Effect ownership 的唯一规范来源；本 change 只要求具体 Effect change 证明其符合该规范，不另行定义平行 ownership 规则。
- 风险: 为了统一入口，让 `EffectChainBuilder` 承担运行时职责。
  - 控制: Builder 只构造数据，禁止创建 ECS entity 或调用 native。
- 风险: 现有 step 不完全符合新矩阵。
  - 控制: 先盘点、后按独立 change 迁移；本 change 不追溯性破坏现有 API。
- 回滚: 删除本 change 的治理规格；不影响当前运行时代码。已按规则落地的具体能力通过各自 change 独立回滚。

## 9. 验证计划

- 校验本 change 的 `proposal.md`、`design.md`、`tasks.md`、`spec.md` 完整。
- 审查其与 `formalize-ecs-native-effect-separation` 的依赖关系和无冲突性。
- 后续每个具体效果 change 必须至少验证：
  - Builder 生成正确 spec。
  - resolver 产生正确 ECS 语义或 outcome。
  - 非 Native 层无新增 `JassApi` / `KKApi` / `YDApi` / `DzApi` 调用。
  - `War3Frame/War3Frame.csproj` 构建通过。
  - 对应 `Projects/*` 消费项目构建通过。
  - 若涉及真实视觉/命令副作用，记录 War3 手测结果或未覆盖项。

## 10. 非目标

- 不在本 change 中新增任何具体效果 step。
- 不批量重构现有 Damage / Heal / Buff / Area / Line / GroundArea / Projectile / Visual 实现。
- 不替代 `formalize-ecs-native-effect-separation` 的 ownership 迁移任务。
- 不修改 Source Generator、构建链路、CLI 或项目依赖。

## 11. 审核 Gate

- 用户必须同时审核 `proposal.md`、`design.md`、`tasks.md` 和 capability spec。
- 批准本治理提案不等于批准任何具体 Effect 功能实现。
- 若未来某个效果能力影响两个及以上项目区域、公共数据结构或核心流程，必须按 `full` 或 `architecture` 单独提案。
