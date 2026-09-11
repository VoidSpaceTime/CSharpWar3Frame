## Phase 1: API 编译验证

- [x] 先增加字符串、完整 inline Ability、即时 Effect 三种调用形式的编译场景。
- [x] 验证两个 lambda 重载不存在 CS0121；若存在歧义，回到提案调整 API，不使用强制类型转换规避。

## Phase 2: 内部模板能力

- [x] 新增持有结构化 `AbilitySpec` 的内部 `IAbilityTemplate` wrapper。
- [x] 让 wrapper 复用 `AbilitySpecBuilder` 的内部 Apply 逻辑并按传入 level 解析。
- [x] 实现保留前缀、专用原子注册入口和普通注册拒绝保留名称规则。
- [x] 实现 Item template owner registry 与覆盖全部 authoring 字段的规范化 `AbilitySpec` 指纹。
- [x] 实现同 owner 同指纹幂等、同 owner 不同指纹失败和晚到普通注册不可覆盖规则。
- [x] 注册前递归拒绝 `AbilitySpec` 中全部非空 Entity，并确保 registry 不保存 authoring lambda。

## Phase 3: ItemSpecBuilder 语法糖

- [x] 保留 `UseAbility(string)`，增加完整 inline Ability 重载。
- [x] 增加即时 `None` 目标 Effect 简写重载。
- [x] 限制同一 Builder 只能调用一次任意 `UseAbility` 入口。
- [x] 最终只写入生成的 `useAbilityTemplateName`，不新增 Item runtime 配置分支。

## Phase 4: 场景验证

- [x] 验证即时 Heal inline Ability 走 companion/Casting/Effect。
- [x] 验证完整 Unit、Point、Area inline Ability 的目标与配置。
- [x] 验证同 Item template 复用定义但 companion/cooldown 独立。
- [x] 验证不同 Item template 名称隔离、保留名称冲突和重复配置失败。
- [x] 验证 initialize 前后注册顺序一致、同 owner 不同 spec 失败和非空 Entity spec 拒绝。
- [x] 验证 ItemLevel 变化重新应用 inline AbilitySpec。
- [x] 验证资源上限、严格 UTF-8、循环引用、生命周期行为拒绝和快照隔离。

## Phase 5: 构建与复审

- [x] 构建 `War3Frame/War3Frame.csproj` Release。
- [x] 构建 `Projects/test/test.csproj` Release。
- [x] 运行纯 ECS Item companion 场景，确认 0 failures。
- [x] 扫描确认未恢复 `ItemUseEffectData`、`useEffectSpec` 或 Item 直接 Effect 路径。
- [x] 记录复审处置：初轮阻塞项已修复，最终五路复审按用户要求取消，不声明五路全 PASS。
- [x] 记录真实 War3 客户端验证状态：本轮未执行，仍需在独立客户端中验收。
