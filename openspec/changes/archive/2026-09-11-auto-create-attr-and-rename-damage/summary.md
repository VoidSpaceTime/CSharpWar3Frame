# 变更总结：属性系统收敛

## 实际改动范围

### 命名统一：`Damage` → `AttackDamage`

- `War3Frame/Src/Helpers/AttributeHelper.cs`：删除 `Damage = Register("Damage")` 注册（与 Combat.cs 的 `AttackDamage` 重复）。
- `Projects/test/Scripts/Template/Unit.cs`：footman 模板 `.Attr(AttributeHelper.Damage, 24)` → `.Attr(AttributeHelper.AttackDamage, 24)`。
- `Projects/test/Scripts/Template/Ability.cs`：battle_shout 效果链 buff 的 attrTypeId 从 `AttributeHelper.Damage` → `AttributeHelper.AttackDamage`。

### 属性实体 auto-create（消除静默丢弃）

- `War3Frame/Src/Helpers/AttributeHelper.cs`：新增 `GetOrCreateAttr(unit, typeId, baseValue = 0f)`。
- `War3Frame/Src/Helpers/ModifyHelper.cs`：`AddModifierToUnit` 从 `GetAttr` 判空返回 → `GetOrCreateAttr`（挂载技能/物品贡献不再静默丢弃）。
- `War3Frame/Src/Helpers/BuffHelper.cs`：`CreateBuffInternal` 从 `GetAttr` 判空返回 default → `GetOrCreateAttr`（buff/DoT 对未声明属性的目标不再挂不上）。

## 验证结果

- War3Frame 编译通过：0 error。
- Projects/test 编译通过：0 error。
- 全仓 grep `AttributeHelper.Damage\b`：零残留。

## 设计说明

- base=0 auto-create 语义：ECS 不预设底子，纯由修改器贡献。Flat 加法正确（0+X=X）；Percent 系需作者在模板声明 base，按 0 计算为作者责任（行为可预期）。
- 未引入 native 属性回读（用户明确否决）；运行时日志增强暂缓（用户决定后续处理）。

## 遗留风险与后续事项

- 无阻塞性未完成项。
- 后续可选项：运行时日志增强（"对不存在属性挂 Percent 修改器"输出诊断）；如需 Percent 系在无 base 属性上自动落 1 或报错，需另行决策（当前语义是算出 0）。
