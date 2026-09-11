# 规范 ECS Tag 分类与命名

## 0. 基本信息

- Change ID: `standardize-tag-taxonomy`
- 提案等级: `light`
- 目标一句话: 建立 `ITag` 的分类、命名、所有权与清理责任规范，降低 Tag 滥用和语义漂移风险。
- 请求来源: 用户提出当前大量流程通过 Tag 过滤执行，需要判断是否集中存储并规范命名。

## 1. 分级判定

### 1.1 为什么是 light

- 影响范围: 主要影响 `War3Frame/Src/Components` 内 `ITag` 命名与文档约束。
- 风险等级: 中低；本提案阶段不改变运行时行为。
- 可逆性: 高；命名规范和局部重命名可分批回滚。
- 是否跨项目: 否；`Projects/` 仅在后续实际重命名触及时受影响。
- 是否改公共契约: 当前只定义规范，不立即改公共 API；后续若批量重命名公共 Tag，应另开实现提案或升级范围。

### 1.2 升级触发器检查

- [ ] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [ ] 涉及 `Projects/` 示例或集成验证行为
- [ ] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

## 2. 背景与目标

当前 Tag 用于 ECS 过滤、一次性请求、脏标记、运行时状态和生命周期阶段，例如：

- 状态类: `MovingTag`, `ItemEquippedTag`, `ProjectileArrived`
- 请求类: `ItemAttrApplyRequest`, `ProjectileArriveRequest`
- 脏标记: `AttrDirty`, `AbilityStatDirty`, `ProjectilePositionDirty`
- 生命周期: `EffectPending`, `EffectCompleted`, `EffectExpired`, `BuffExpired`
- 领域标识: `Buff`, `Aura`

问题是命名后缀混用：有些使用 `Tag`，有些直接用名词，有些请求没有 `Tag` 后缀，有些生命周期阶段缺少统一后缀。后续数量继续增长时，容易出现：

- 不知道某个 Tag 是长期状态还是一次性请求。
- 只添加不移除，或只查询不添加。
- helper 和 system 对同一个 Tag 的 owner 不明确。
- 新 Tag 难以按领域和生命周期归档。

目标：建立一套轻量命名与分类规范，先约束新增 Tag，再分批整理存量 Tag。

## 3. 影响范围

- 模块: `War3Frame/` ECS 组件与系统。
- 文件: 本提案仅新增 OpenSpec 记录；后续实施可能新增 Tag 规范文档或局部注释。
- 不受影响区域:
  - `War3Frame.Generator/`: 不改 generator。
  - `FrameBuild/`: 不影响构建链路。
  - `CSharpWar3Frame/`: 不影响 CLI。
  - `Projects/`: 当前不迁移示例代码。

## 4. 方案摘要

### 4.1 不引入运行时静态 Tag registry

不创建“存储所有 Tag 的静态类”作为执行入口。原因：Friflo Tag 的核心是泛型类型本身，`AddTag<T>()` / `Tags.Has<T>()` / `RemoveTag<T>()` 已经提供强类型约束。静态 registry 只能保存元数据，不能替代泛型调用，容易增加间接层。

### 4.2 建立分类命名规则

新增或重命名 Tag 时按语义选择后缀：

- 领域身份 / 长期标识: 使用名词或 `*Tag`，例如 `Buff`, `Aura`, `ItemGroundTag`。
- 运行时状态: 使用 `*StateTag` 或明确状态名，例如 `MovingTag`, `ItemEquippedTag`。
- 一次性请求: 使用 `*RequestTag`，例如后续可将 `ProjectileArriveRequest` 规范为 `ProjectileArriveRequestTag`。
- 脏标记: 使用 `*DirtyTag`，例如后续可将 `AttrDirty` 规范为 `AttrDirtyTag`。
- 生命周期完成/过期: 使用 `*PendingTag`、`*CompletedTag`、`*ExpiredTag`。
- 阶段/事件标记: 使用 `*StageTag` 或 `*EventTag`，避免 `ProjectileOnStart` 这种像回调的命名。

### 4.3 记录 owner 和清理责任

每类 Tag 应明确：

- 谁添加。
- 谁消费。
- 谁移除。
- 是否允许跨帧存在。
- 是否是长期语义真相。

### 4.4 存量迁移策略

不一次性批量重命名全部 Tag。建议分两步：

1. 先添加规范文档或 `TagTaxonomy` 文档注释索引，约束新增 Tag。
2. 后续按领域分批重命名存量 Tag，每批保持可验证、可回滚。

## 5. 风险与回滚

- 风险: 如果后续直接批量重命名，可能影响大量系统与 helper 调用点。
- 控制: 本 change 只建立规范，不立刻做大规模重命名。
- 回滚方式: 删除规范文档或撤回对应命名要求即可，不影响运行时行为。

## 6. 验收标准

- 明确 Tag 分类和命名后缀规则。
- 明确不引入运行时静态 registry 的原因。
- 明确新增 Tag 必须描述 owner、consumer、cleanup。
- 明确存量 Tag 后续按领域分批迁移，而不是一次性全局重命名。
- 若后续实施，仅做文档/规范新增时执行文档检查；若重命名代码，则执行 `dotnet build War3Frame/War3Frame.csproj` 和相关项目构建。
