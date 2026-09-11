## 1. 审核

- [ ] 1.1 用户审核并批准 `proposal.md` / `design.md` / `tasks.md` / `spec.md`
- [ ] 1.2 确认提案等级为 `full`
- [ ] 1.3 确认纯数字默认语义为固定值
- [ ] 1.4 确认系统命名为 `LevelStatRebuildSystem`

## 2. 通用等级数值模型

- [ ] 2.1 新增 `LevelValueKind`
- [ ] 2.2 新增 `LevelValue` 与 `Resolve(level)`
- [ ] 2.3 支持 `Fixed` / `PerLevel` / `LevelTable`
- [ ] 2.4 为新增公共类型和公共方法添加必要中文注释

## 3. Unit / Item / Ability authoring 适配

- [ ] 3.1 将 `UnitAttributeSpec` 扩展为保存 `LevelValue`
- [ ] 3.2 将 `ItemAttributeContributionSpec` 扩展为保存 `LevelValue`
- [ ] 3.3 将 `AbilitySpec.baseValues` 扩展为保存 `LevelValue`
- [ ] 3.4 为 `UnitSpecBuilder` / `ItemSpecBuilder` / `AbilitySpecBuilder` 增加 `LevelValue` 重载
- [ ] 3.5 保留纯数字重载，并转换为固定值

## 4. 等级与 Dirty

- [ ] 4.1 新增 `LevelStatDirty`
- [ ] 4.2 确认或新增 Unit 当前等级组件
- [ ] 4.3 确认或新增 Item 当前等级组件
- [ ] 4.4 Ability 复用 `AbilityBase.level`

## 5. LevelStatRebuildSystem

- [ ] 5.1 实现 Ability 等级基础数值重算
- [ ] 5.2 实现 Unit 等级基础属性重算
- [ ] 5.3 实现 Item 等级属性贡献重算
- [ ] 5.4 重算后触发后续 dirty / apply request
- [ ] 5.5 重算后移除 `LevelStatDirty`
- [ ] 5.6 确认系统不直接调用 War3 native

## 6. 示例与验证

- [ ] 6.1 添加或迁移一个 Ability 线性成长示例
- [ ] 6.2 添加或迁移一个 Unit 固定值兼容示例
- [ ] 6.3 添加或迁移一个 Item 等级表示例
- [ ] 6.4 构建 `War3Frame/War3Frame.csproj`
- [ ] 6.5 构建 `Projects/test/test.csproj`
- [ ] 6.6 总结兼容性、风险和后续曲线扩展方向
