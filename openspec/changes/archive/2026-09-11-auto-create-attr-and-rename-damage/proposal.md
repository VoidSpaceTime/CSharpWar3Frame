# 属性系统收敛：auto-create 消除静默丢弃 + 攻击力属性命名统一

## 元信息

- **状态**：已实施
- **等级**：light
- **变更 ID**：auto-create-attr-and-rename-damage
- **日期**：2026-09-03
- **复盘强度**：R0 Direct（编译验证已通过）

## 背景与目标

### 背景

排查属性系统时发现两个独立但相关的模型缺陷：

1. **攻击力属性存在重复注册**。`AttributeHelper.cs`（基础部分）注册了 `Damage`，`Combat.cs`（战斗部分，同一 `AttributeHelper` partial 类）注册了 `AttackDamage`。两者语义几乎必然同义（都指攻击力），导致：
   - 全仓引用混乱：footman 单位模板 `.Attr(AttributeHelper.Damage, 24)` 用 `Damage`，而 Combat.cs 战斗属性组命名风格是 `AttackDamage`。
   - 用户已确认以 Combat 为准：攻击力属性统一为 `AttackDamage`。

2. **属性贡献路径存在静默丢弃**。`ModifyHelper.AddModifierToUnit` 与 `BuffHelper.CreateBuffInternal` 在目标属性实体不存在时直接返回/放弃：
   ```csharp
   // ModifyHelper.AddModifierToUnit 原文
   var attr = AttributeHelper.GetAttr(unit, attrTypeId);
   if (attr == null) return null;   // 单位模板未声明该属性 → 静默丢弃贡献
   ```
   单位模板只显式 `.Attr(...)` 声明少量属性（footman 只声明 Health/Mana/AttackDamage 三个）。任何对单位**未声明属性**（MoveSpeed、Armor、CritChance、Stun……）的挂载技能/物品贡献或 buff，都会被静默丢弃，无报错无效果，作者极难排查。

### 目标

- 攻击力属性统一为 `AttackDamage`（Combat.cs 为准），删除重复的 `Damage` 注册并迁移引用。
- 消除属性贡献静默丢弃：贡献目标属性实体不存在时，自动创建 base=0 的属性实体。
- 保持"ECS 是唯一属性真相源"的既有架构原则（native 只做投影，从不回读为真相）。

### 非目标

- **不引入 native 属性回读**（用户已明确否决：native 单位需要预先初始化时会在 ECS 创建同步，不反向回读）。
- 不为 Percent 系修改器落空设计专门诊断/报错（作者责任：往无 base 属性上挂百分比本就无意义，正确姿势是模板先声明 base）。
- 不引入运行时日志增强（用户暂缓，后续单独处理）。

## 影响范围

| 区域 | 影响 | 说明 |
|---|---|---|
| `War3Frame/Src/Helpers/AttributeHelper.cs` | 修改 | 删 `Damage` 注册；新增 `GetOrCreateAttr` |
| `War3Frame/Src/Components/Combat.cs` | 无改动 | `AttackDamage` 定义保留（为准） |
| `War3Frame/Src/Helpers/ModifyHelper.cs` | 修改 | `AddModifierToUnit` 改用 `GetOrCreateAttr` |
| `War3Frame/Src/Helpers/BuffHelper.cs` | 修改 | `CreateBuffInternal` 改用 `GetOrCreateAttr` |
| `Projects/test/Scripts/Template/Unit.cs` | 修改 | `AttributeHelper.Damage` → `AttackDamage` |
| `Projects/test/Scripts/Template/Ability.cs` | 修改 | battle_shout 的 `AttributeHelper.Damage` → `AttackDamage` |
| `War3Frame.Generator/` | 不受影响 | 无生成器涉及属性 ID 注册 |
| `FrameBuild/` | 不受影响 | 无引用属性 ID |
| `CSharpWar3Frame/` | 不受影响 | 无引用属性 ID |

## 方案摘要

### 1. 命名统一：`Damage` → `AttackDamage`

`AttributeHelper.cs` 删除 `Damage = Register("Damage")`；两处引用迁移到 `AttributeHelper.AttackDamage`：
- `Projects/test/Scripts/Template/Unit.cs` footman `.Attr(AttributeHelper.AttackDamage, 24)`
- `Projects/test/Scripts/Template/Ability.cs` battle_shout 效果链 buff 的 attrTypeId

### 2. 属性实体 auto-create

`AttributeHelper` 新增统一入口：

```csharp
public static Entity GetOrCreateAttr(Entity unit, int typeId, float baseValue = 0f)
{
    if (TryGetAttr(unit, typeId, out var attr))
        return attr;
    return CreateAttr(unit, typeId, baseValue);
}
```

两个贡献路径改用此入口：
- `ModifyHelper.AddModifierToUnit`（挂载技能/物品贡献）
- `BuffHelper.CreateBuffInternal`（buff/DoT 贡献）

**语义**：base=0 表示"该属性在 ECS 没有预设底子，一切由修改器贡献"。Flat 加法在 base=0 上完全正确（0+X=X）；Percent 系需作者在模板先声明 base，否则按 0 计算是作者责任（行为可预期，非静默失败）。

### 验证方式

- 双项目编译通过（War3Frame + Projects/test，0 error）。
- 全仓 grep 确认 `AttributeHelper.Damage\b` 无残留。

## 风险与回滚

- **低风险**：命名收敛是纯引用迁移；auto-create 只在"属性原本缺失"时新增行为，已存在属性路径不变。
- 回滚：还原 AttributeHelper/ModifyHelper/BuffHelper 三处改动 + 两处模板引用即可。

## 验收标准

1. `AttributeHelper.Damage` 全仓零引用，footman/battle_shout 均用 `AttackDamage`。
2. 对未声明 MoveSpeed 的单位施加减速 buff（Flat），属性实体自动创建且 finalValue 正确为 `0+flat`。
3. 对未声明 Armor 的单位装备带护甲贡献的物品，贡献不再静默丢弃。
4. War3Frame + Projects/test 编译通过。
