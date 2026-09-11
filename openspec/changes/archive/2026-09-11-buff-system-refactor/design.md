# Design：Buff 系统重构

- **变更 ID**：buff-system-refactor
- **等级**：full
- **日期**：2026-09-03

## ADR 汇总

### ADR-1：tags 用 `[Flags] BuffTag` 枚举，不用 List&lt;string&gt;（S4）

**决策**：类型化 `[Flags] BuffTag` 替代 `List<string>`。

**备选**：
| 方案 | 优劣 |
|---|---|
| `List<string>`（现状） | 灵活支持任意 tag，但每次创建分配 List、净化线性扫描 Contains、拼写错误静默、语义散落字符串 |
| `[Flags] BuffTag` 枚举 | 位运算 O(1)、编译期检查、标签集合固定。代价：无法表达任意字符串 |
| `HashSet<BuffTag>` | 位运算语义但仍是分配 |
| string + 注册表解析 | 灵活但复杂，框架内标签集合实际固定（净化 Debuff/Control、免疫元素）|

**结论**：标签是**框架内固定概念**（debuff/control/元素），作者不需要任意字符串标签 → `[Flags] BuffTag`。未来若要自定义 tag，走"注册 BuffTagId 到注册表"的扩展而非放开成字符串。

### ADR-2：删除 BuffDuration，总时长用 Duration.total（D2）

**决策**：删除 `BuffDuration` 组件，`Duration.total` 承担"初始总时长"。

**依据**：`Duration`（unify-duration 提案产物）已有 `remaining`（递减）+ `total`（不变）。`BuffDuration.duration` ≡ `Duration.total`。删除后刷新改一次 `AddComponent(Duration)` 而非成对改两个组件。

**备选**：
| 方案 | 优劣 |
|---|---|
| 保留 BuffDuration | 双组件表达同一概念，刷新 3 处重复成对写，query 需手筛 |
| 并入 Duration.total（采纳）| 单一事实源；BuffDurationSystem 收窄 Query 天然解决 D1 |

### ADR-3：显式 `BuffKind` 而非 `isDot` 布尔（D3）

**决策**：`enum BuffKind { Attribute, Tick, PureTag }`。

**备选**：
| 方案 | 优劣 |
|---|---|
| `bool isDoT` | 双态够用但表达不出"纯标记"第三态；true/false 无语义自明 |
| `BuffKind` 枚举（采纳）| 三态明确（属性贡献/周期行为/纯标记），构造时定型 |

**备注**：`PureTag` 三态服务未来"仅标记无数值"的 buff（如燃油标记，TriggerSpec 反应用）。当前便捷方法只用到 Attribute（控制门闩）+ Tick（DoT）。

### ADR-4：BuffBehavior 作统一 Query 锚点（D1）

**决策**：所有 buff 实体经共享 `CreateBuffInternal` 必挂 `BuffBehavior` → 清理/到期系统以 `Buff+BuffBehavior+...` 为 Query 锚点。

**依据**：`Buff` 是 IComponent 后 Query 本可精确匹配 `Buff+Duration`，但 `BuffDurationSystem` 用宽 Query + 手筛（因非 buff 实体也有 BuffDuration+Duration）。删除 BuffDuration 后 Query 收窄为 `Buff+BuffBehavior+Duration` 自然消除手筛。BuffBehavior 保留 icon 作"buff 身份配置"，比裸 `Buff` 更精确表达"业务 buff 实体"。

### ADR-5：BuffBehavior 瘦身（C2/C3）

- 删 `refreshBehavior`：刷新决策读**新请求** spec.onDuplicate（unify-buff-apply 已修正 P2），旧 buff 上的记录无读者 → 死数据。
- 删 `buffId`：`Buff.buffId` 已是唯一同义源。
- 删 `removeAllStacksOnExpire`：当前到期即全删、无"逐层减"实现者。未来需要再补语义。
- 保留 `icon`：唯一有实际读者（UI 层）的字段。

### ADR-6：Buff 索引（S2）

**决策**：单位挂 `BuffIndex` 组件，`FindBuffByIdOnUnit` O(1)。

**备选**：
| 方案 | 优劣 |
|---|---|
| 现状全扫 O(属性×modifier) | 无存储开销但 ApplyBuff/HasBuff/RemoveBuff 高频调用热点 |
| 单位挂 `Dictionary<string,long>` 索引（采纳）| O(1)；需维护一致性（写入创建、删除路径移除、判空回退全扫）|
| Friflo Relation 索引 | Friflo 本身对 ILinkComponent 有反向索引（GetIncomingLinks 已是 O(n) 遍历但带缓存）——**先评估 Friflo 内置反向索引是否够用**，不够再上显式 Dictionary |

**实现注意**：Friflo `GetIncomingLinks<ModifyTarget>` 已是反向关系缓存。若单单位 modifier 数小（<几十），全扫未必热点。**T6 先做基准测量再决定是否显式索引**——若 Friflo 反向索引 + 属性粒度已够，跳过显式 Dictionary，避免一致性负担。

### ADR-7：caster 解析（C1）

**决策**：`Buff.caster` 存伤害来源单位；创建时解析：source 是单位直接用，否则沿 GroundAreaSource.caster 等领域链。

**语义区分**：
- `ModifySource.source`：产生者实体（技能/区域/光环/单位），服务级联清理——**不改**。
- `Buff.caster`：实际造成效果的施法者单位，服务 DoT 伤害 source——**新增**。

**备选**：
| 方案 | 优劣 |
|---|---|
| 复用 ModifySource.source 当伤害源 | 区域 DoT 时 source=areaEntity 非单位，DamageRequest.source 语义错 |
| 新增 Buff.caster（采纳）| 语义清晰；创建时一次解析；DoT/后续效果直接用 |

### ADR-8：刷新路径收敛（S1）

**决策**：抽 `RefreshCore(existing, spec, refresh, stack)` + `DirtyAttr` helper，六分支声明式。

**效果**：删 8 处复制打脏、3 处成对组件写；刷新/叠层组合由参数表达，避免分支内重复。

### ADR-9：EffectChainBuilder.Buff 方法族（D4）

**决策**：主方法收敛核心参数（buffId/duration/attr/value/modifyType/onDuplicate），icon/tick/tags 走链式 `.WithIcon/.WithTick/.WithTags`。

**备选**：
| 方案 | 优劣 |
|---|---|
| 11 参现状 | 调用难读、易错位、双重载爆炸 |
| Options 对象 | 集中但作者需先 new options |
| 方法族 + 链式（采纳）| 主干短、可选扩展链式、与现有 fluent DSL 风格一致 |

### ADR-10：BuffIndex 与 BuffTag 是否过度设计（防御）

**风险自检**：BuffTag 枚举是"标签集合固定"假设——若作者频繁要自定义标签会受限。缓解：预留扩展位 + 若频繁需任意 tag 再评估注册表。BuffIndex 若 Friflo 反向索引够用则跳过。两项都在 T6/实现期二次确认，不做 speculative 复杂度。

### ADR-11：Buff 链路合并为 2 层（D5 agent 结论）

**决策**：删除 `ApplyBuffData`，并入 `BuffApplyRequest`（buff 链路从 3 relay struct 减为 2）。

**依据**（D5 探索 bg_6238a5aa）：
- `BuffApplyRequest` 是唯一真正的公共契约：TriggerActionRegistry.BuffApply 绕过 payload 直接写它；TriggerValidationScenario 直接断言其字段。`ApplyBuffData` 只是 effect 实体与 Request 之间的拷贝层。
- `BuffEffectStepSpec`（authoring，可被弹道/子效果复用，与 Damage/Heal 同构）与 `BuffSpec`（helper 工厂边界）不是并入对象。
- relay 有实害证据：ApplyEffectSpec Buff 分支只复制 6/11 字段（P0 bug），中继层越少漂移面越小。

**trade-off 表**：

| 维度 | 保持三层 | 合并为二层（采纳） | 注册表引用 Request |
|---|---|---|---|
| 耦合 | 同字段散落 3 struct，改一字段改三处 | 改两处（spec+request）；resolver 需一行 guard | request 瘦身但强依赖注册表时序 |
| 可测试性 | 测试需过 payload→request 两跳 | 契约单点，现有场景测试直接守护 | 测试需预置注册数据 |
| 分配 | 两跳 struct 拷贝 + 重复 List tags 引用 | 少一跳拷贝，略降 | 字段最少但 key 查找/缓存引入新分配面 |
| TriggerSpec 扩展 | 只能写 Request，无法观察 payload | 不变（公共层未动，可观测性完整）| 只能引用已注册 buff，破坏 param 任意 buffId 通道 |
| AOT 友好 | 纯 struct，好 | 纯 struct，好 | Dictionary/索引需额外约束 |

**结论**：合并为二层，**不建注册表引用**（Trigger 依赖"任意 param 直接写 Request"；引用式削弱可观测/可回放性）。留注释：未来数据驱动时把"读配置收敛到 Resolve→BuffSpec 一处"单点替换。
