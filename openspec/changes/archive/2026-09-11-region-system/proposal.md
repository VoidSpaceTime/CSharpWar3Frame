# 提案：区域系统 Region（ECS 进出事件）

**状态**：已取消
**等级**：light
**提案日期**：2026-08-31
**修订日期**：2026-09-01
**取消日期**：2026-09-01
**请求来源**：框架无 Region 能力；lik/xlik 用原生触发器或计时轮询 diff 进出集合。`AGENTS.md` 规定区域进入等状态条件由空间/状态系统检测并生成领域事件，不依赖 Friflo 结构变化通知。

---

## 取消原因

用户重新评估后认为独立 Region 系统**不符合框架实际需求**，理由如下：

1. **现有能力已覆盖核心场景**：
   - `SpatialGrid` 已提供 `QueryCircle/Rect/Cone/Line` 完整空间查询 API。
   - `AuraSystem` 已实现"周期检测 + diff 快照 + 自动添加/移除"，正是 Region 的核心逻辑，只是输出形式绑定为 Buff。
   - `GroundAreaQueryHelper` 已有地面区域持续影响 + `GroundAreaBuffLink` 追踪机制，比 Region 提案的 `RegionOccupancy` 快照更精细。
   - `TriggerConditionRegistry` 可注册自定义条件，直接调 `SpatialGrid` 即可判定"单位在区域"。

2. **抽象层复用价值存疑**：
   - 需要"持久区域 + 通用进出事件"的场景数量不明确（当前 0 个实际需求）。
   - 每个空间条件场景的"进入后做什么"差异巨大（加 Buff / 触发剧情 / 改变地形 / 开启 Boss），统一事件反而不如直接写 Query + 特定逻辑。
   - lik/xlik 也没有把 Region 做成独立抽象，因为复用价值低。

3. **不照搬 Lua 框架**：
   - 提案源于对标 lik/xlik，但用户明确要求"不按 Lua 框架逻辑来，有更好的肯定要上更好的"。
   - C# ECS 框架的 `SpatialGrid` + 组合式系统（Aura/GroundArea/Trigger）已经比 Lua 的原生触发器 + 手写轮询更强。

4. **按需组合优于预设抽象**：
   - 后续如果真有 3+ 场景需要"持久区域 + 进出事件"，再提轻量 Region 提案。
   - 当前只需修复 `SpatialGridSystem` 注册（前置死代码修复），不引入新抽象层。

---

## 保留价值（已拆分处理）

提案中唯一需要立即修复的部分：

- **`SpatialGridSystem` 补 `[SystemRegister(SystemKind.Immediate, 15)]`**：
  - 现状：`SpatialGridSystem.cs` 存在但未注册，网格每帧重建逻辑是死代码。
  - 修复：补注册，验证 `GroupHelper.Grid.TotalEntities > 0`。
  - 等级：fast（修复已有死代码）。
  - 独立提案：`openspec/changes/fix-spatial-grid-registration/`（如需要）或直接修复。

其余设计（`RegionComponent` / `RegionSystem` / `RegionHelper` / 进出事件）全部取消。

---

## 原始提案摘要（存档）

<details>
<summary>展开查看原提案内容（已作废）</summary>

### 背景与目标

仓库没有区域进出语义。现有 `SpatialGrid` / `SpatialGridSystem` 已提供每帧空间索引，查询 API 为 `QueryCircle` / `QueryRect`（`War3Frame/Src/Helpers/SpatialGrid.cs`）。

目标：新增区域组件、周期检测系统、薄 Helper。系统用网格查询当前区域内单位，与上一快照 diff，生成独立事件实体 `RegionEnterEvent` / `RegionLeaveEvent`（对齐 `DamageEvent`：只读事实、多监听者、由统一清理删除）。不接触 War3 原生 `rect` / `region` 触发器。

### 组件设计

```csharp
public struct RegionComponent : IComponent
{
    public RegionShape shape;        // Rectangle / Circle
    public float centerX, centerY;
    public float halfWidth, halfHeight;
    public float radius;
    public string tag;
}

public struct RegionOccupancy : IComponent
{
    public HashSet<Entity> members;
}

public struct RegionEnterEvent : IComponent
{
    public Entity region;
    public Entity unit;
}
public struct RegionLeaveEvent : IComponent
{
    public Entity region;
    public Entity unit;
}
```

### 系统执行流程

```
前置：SpatialGridSystem（order 15，每帧 Immediate）→ 重建 GroupHelper.Grid
RegionSystem（order 50，ITimedSystem.Interval = 0.1f）→ diff 快照 → 发事件
EventCleanupSystem（order 132）→ 删除事件
```

### 验收标准（已作废）

1. `SpatialGridSystem` 注册验证。
2. 单位进入矩形区域产生恰好一次 `RegionEnterEvent`。
3. 离开产生恰好一次 `RegionLeaveEvent`。
4. 区内停留不重复 Enter；再进再出不漏。
5. 区域销毁时对 `RegionOccupancy.members` 发 `RegionLeaveEvent`。
6. 事件实体带 `TriggerEventMarker`，在 `EventCleanupSystem` 后被删除。

</details>

---

## 后续建议

1. **立即修复** `SpatialGridSystem` 注册（fast 级，独立 commit）。
2. **需要空间条件时**，直接调 `SpatialGrid.QueryCircle/Rect`，不做通用事件抽象。
3. **需要周期检测进出时**，照抄 `AuraSystem` 的 diff 模式写特定系统（如 `QuestAreaSystem`），不做通用 `RegionSystem`。
4. **如果未来有 3+ 场景需要通用进出事件**，再重新提轻量 Region 提案。
