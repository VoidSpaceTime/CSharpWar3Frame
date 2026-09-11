## 1. Authoring API

`ItemSpecBuilder` 增加两个重载，并保留现有字符串入口：

```csharp
public ItemSpecBuilder UseAbility(string abilityTemplateName);
public ItemSpecBuilder UseAbility(Func<AbilitySpecBuilder, AbilitySpecBuilder> configure);
public ItemSpecBuilder UseAbility(Func<EffectChainBuilder, EffectChainBuilder> configure);
```

完整重载构造标准 `AbilitySpecBuilder`。Effect 简写等价于创建 `None` 目标、零阶段和零冷却的 Ability，并把 lambda 写入 `OnEffect(...)`。

## 2. 稳定身份与注册

- 内部 template name 使用确定性格式 `__item_inline__:{itemTemplateName}`。
- 该前缀为框架保留；公开 `AbilityTemplate.Register` 必须拒绝普通手动或 generated template 使用该前缀。
- Inline 配置在 `UseAbility(...)` 调用时立即构造成 `AbilitySpec`，随后注册内部 `IAbilityTemplate`。
- `AbilityTemplate` 提供程序集内部专用的原子 inline 注册入口，单次完成保留名称检查、owner 检查、spec 指纹比较与写入。
- Registry 以规范化 Item template name 作为逻辑 owner；两个实现若声明相同 Item template name，按同一逻辑 owner 处理。
- Registry 为 inline `AbilitySpec` 生成覆盖所有 authoring 字段的规范化结构指纹。相同 owner、相同指纹必须幂等复用；相同 owner、不同指纹必须明确失败。
- 相同名称已由普通 Ability template 占用，或名称/owner 不一致时必须明确失败，不得覆盖。
- 上述规则不得依赖 `AbilityTemplate.Initialize()` 调用顺序：inline 先注册时，后续普通/generated 注册也不能覆盖；普通注册先执行时，保留前缀已被统一拒绝。
- 同一 `ItemSpecBuilder` 第二次调用任意 `UseAbility` 重载必须失败，避免配置顺序决定最终行为。

“物品私有”指 Ability 定义只由该 Item template 引用；同一模板产生的所有 Item 实例共享定义，但每个 Item entity 仍拥有独立 companion 与运行时状态。

## 3. AbilitySpec 应用

新增内部 inline template wrapper，持有构建完成的 `AbilitySpec` 和来源 Item template name。其 `Configure(Entity ability, int level)` 复用 `AbilitySpecBuilder` 现有 Apply 逻辑，并按 companion 当前 `ItemLevel` 解析 `LevelValue`。

`AbilitySpecBuilder.Apply(...)` 只调整为程序集内部可复用，不新增公共运行时入口。Inline wrapper 只保存验证后的结构化 `AbilitySpec`，不得保存 authoring lambda。

注册前必须递归检查 `AbilitySpec` 中的运行时引用字段。任何非空 Entity（包括 `ProjectileEffectStepSpec.effectEntity`）都必须拒绝；结构指纹只对通过该检查的纯 authoring 数据计算。后续新增可承载 Entity 的 spec 字段时必须同步扩展验证器与指纹器。

## 4. 运行时保持不变

Inline authoring 最终仍执行：

```text
ItemSpec.useAbilityTemplateName
  -> ItemUseAbilityData.abilityTemplateName
  -> ItemCompanionAbilityHelper
  -> AbilityTemplate.Apply
  -> CastRequest / Casting / Effect
```

不得新增 `ItemInlineEffectData`、`ItemInlineAbilityData` 或 ItemUse 专用 Effect 创建路径。Companion 唯一性、owner、等级同步、冷却和受控删除规则保持不变。

## 5. 初始化与确定性

Item template 的 `Configure` 在 `ItemSpecBuilder.BuildTo` 前执行 inline `UseAbility(...)`，因此内部 Ability template 会先注册，再写入 `ItemUseAbilityData`。无需修改 Source Generator。

配置 lambda 必须是确定性的纯 authoring 描述。框架通过立即求值避免长期捕获，并通过同 owner 指纹比较拒绝重复配置产生不同定义。

## 6. 验证策略

- 编译验证三个重载的真实 lambda 调用，特别检查 CS0121 重载歧义。
- 只承诺文档展示的成员特征明确 lambda 可推断；`x => x` 等无特征 lambda 不属于支持用法。
- 两个同模板 Item 必须得到同一生成 template name、不同 companion entity 和独立 cooldown。
- 两个不同 Item template 必须得到不同生成名称。
- 保留名称冲突、重复 `UseAbility`、无效 inline Ability 配置必须明确失败且不写半成品 Item 配置。
- 覆盖 inline-before-initialize、initialize-before-inline、晚到普通注册、同 owner 不同指纹和非空 Entity spec 拒绝。
- 即时 Heal 和完整 Unit/Point/Area inline Ability 必须进入现有 Casting/Effect 场景。
