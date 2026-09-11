# Spec：AttributeEntityLifecycle

变更：`enforce-attr-entity-lifecycle`
能力：属性实体生命周期完整性（唯一性 + 可达性 + 保留性 + 回收性）

## 目标

定义属性实体（挂 `AttrValue` + `AttrOwner` 的独立实体）的创建、唯一性与回收契约，消除 `auto-create-attr-and-rename-damage` 引入动态创建后遗留的重复实体与孤儿实体问题。

## 需求

### AEL-1 唯一性

- 同一 unit 同一 `attrTypeId` 至多存在一个属性实体。
- 属性实体的获取/创建统一经 `AttributeHelper.GetOrCreateAttr`；底层创建原语不对外暴露，且自身幂等（先查后建）。
- 禁止任何入口接受裸 `attrEntity` 绕过 get-or-create 建立贡献。

### AEL-2 声明归属

- 属性是否属于单位"声明"由 `UnitSpecData.spec.attributes` 判定。
- 判定通过属性实体的 `AttrOwner` 反查 owner unit。
- owner 无 `UnitSpecData`（或非 unit）时，视为"无法判定声明"，属性**保守保留、不回收**。

### AEL-3 回收判据

属性实体可回收，当且仅当全部成立：

1. 不带 `AttrDirty`；
2. `GetIncomingLinks<ModifyTarget>()` 为空；
3. `AttrValue.baseValue == 0`；
4. `AttrValue.current == 0`；
5. `AttrValue.flatBonus == 0` 且 `AttrValue.percentBonus == 0`；
6. 该 `attrTypeId` 未被 owner 的 `UnitSpecData` 声明，且 owner 带 `UnitSpecData`（AEL-2）。

任一条件不满足则保留。判据只允许向"保留"方向保守放宽。

### AEL-4 回收执行位置

- 回收判定与执行位于 `AttrCalculationSystem`（order 45）的重算之后。
- 只对带 `AttrDirty` 的属性实体做判定；从不 dirty 的属性不参与回收。
- 不新增独立全量 sweep 系统。

### AEL-5 删除顺序与确定性

- 删除不在 `Query.ForEachEntity` 迭代内执行；先收集待删列表，循环外统一删除。
- 待删列表按 `entity.Id` 升序处理，保证锁步确定性。
- 删除序列固定为：先从 owner 移除 `HasAttr` 关系，再删除属性实体；删除前校验实体非空。
- 禁止在删除属性实体时遗留 `HasAttr` 悬挂关系。

### AEL-6 计算与同步不受影响

- 回收不改变属性计算公式、不改变 modifier 数据形状、不改变属性 ID 注册。
- 回收不改变 native 同步机制；`UnitNativeSyncSnapshot` 仍按 `attrTypeId` 反查属性，不缓存属性实体引用。

## 验收

- 同一 unit 同 `attrTypeId` 连续 get-or-create 后属性实体数为 1。
- 未声明属性在被装备/Buff 授予后再移除，属性实体被回收且 `HasAttr` 关系已清理。
- 声明属性即使无 modifier 且归零，仍被保留。
- `current != 0` 的属性不被回收。
- `War3Frame` 与 `Projects/test` 编译 0 错误。
- 回收路径无迭代内结构变更、无悬挂关系、顺序确定。
