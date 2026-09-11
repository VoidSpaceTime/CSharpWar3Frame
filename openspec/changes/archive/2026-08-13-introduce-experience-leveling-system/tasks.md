## 1. 审核

- [ ] 1.1 用户审核并批准 `proposal.md` / `design.md` / `tasks.md` / `spec.md`
- [ ] 1.2 确认提案等级为 `full`
- [ ] 1.3 确认经验系统只负责经验曲线、获得经验、升级判定和添加 `LevelStatDirty`
- [ ] 1.4 确认经验系统依赖 `introduce-level-stat-rebuild` 的等级 dirty 出口

## 2. 经验数据模型

- [ ] 2.1 新增 `ExperienceData`
- [ ] 2.2 新增 `ExperienceCurveKind`
- [ ] 2.3 新增 `ExperienceCurve`
- [ ] 2.4 支持 `FixedStep` / `Linear` / `LevelTable`
- [ ] 2.5 为新增公共类型和公共方法添加必要中文注释

## 3. 获得经验请求

- [ ] 3.1 新增 `ExperienceGainRequest`
- [ ] 3.2 支持基础经验 `amount`
- [ ] 3.3 支持倍率 `multiplier`
- [ ] 3.4 保留经验来源 `source` / `sourceType` 以便后续扩展

## 4. Unit / Ability / Item 等级适配

- [ ] 4.1 确认或新增 Unit 等级组件
- [ ] 4.2 Ability 复用 `AbilityBase.level`
- [ ] 4.3 确认或新增 Item 等级组件
- [ ] 4.4 经验升级后只修改对应等级字段或组件
- [ ] 4.5 经验升级后添加 `LevelStatDirty`

## 5. ExperienceSystem

- [ ] 5.1 消费 `ExperienceGainRequest`
- [ ] 5.2 应用 `amount * multiplier`
- [ ] 5.3 根据 `ExperienceCurve.RequiredForNextLevel(level)` 判断升级
- [ ] 5.4 支持一次经验连升多级
- [ ] 5.5 达到 `maxLevel` 后停止升级
- [ ] 5.6 清理已消费请求
- [ ] 5.7 确认系统不直接调用 War3 native
- [ ] 5.8 确认系统不直接重算属性、技能数值或物品贡献

## 6. 示例与验证

- [ ] 6.1 添加或迁移一个 Unit 经验成长示例
- [ ] 6.2 添加或迁移一个 Ability 熟练度成长示例
- [ ] 6.3 添加或迁移一个 Item 杀敌成长示例
- [ ] 6.4 构建 `War3Frame/War3Frame.csproj`
- [ ] 6.5 构建 `Projects/test/test.csproj`
- [ ] 6.6 总结经验系统边界、兼容性和后续 `ExperienceKind` 扩展方向
