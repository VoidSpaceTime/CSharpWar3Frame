# 提案：区域系统 Region（ECS 进出事件）

**状态**：待审核
**等级**：light
**提案日期**：2026-08-31
**请求来源**：框架无 Region 能力；lik/xlik 用原生触发器或计时轮询 diff 进出集合。`AGENTS.md` 规定区域进入等状态条件由空间/状态系统检测并生成领域事件，不依赖 Friflo 结构变化通知。

---

## 背景与目标

仓库没有区域进出语义。现有 `SpatialGrid` / `SpatialGridSystem` 已提供每帧空间索引，查询 API 为 `QueryCircle` / `QueryRect`（`War3Frame/Src/Helpers/SpatialGrid.cs`）。

目标：新增区域组件、周期检测系统、薄 Helper。系统用网格查询当前区域内单位，与上一快照 diff，生成独立事件实体 `RegionEnterEvent` / `RegionLeaveEvent`（对齐 `DamageEvent`：只读事实、多监听者、由统一清理删除）。不接触 War3 原生 `rect` / `region` 触发器。

## 影响范围

- 模块：`War3Frame` 组件、系统、Helper。
- 文件（各新增 1-2 个）：
  - `War3Frame/Src/Components/`：`RegionComponent`（形状 Rectangle/Circle、中心、宽高或半径、可选标签）；进出事件组件。
  - `War3Frame/Src/Systems/`：`RegionSystem`（`SystemKind.Interval`，约 0.1s），`[SystemRegister]` 自动注册。
  - `War3Frame/Src/Helpers/`：`RegionHelper` 创建/查询/销毁（写 ECS，不调原生）。
- 不受影响区域：
  - `War3Frame.Generator/`：仅 attribute 发现新系统，无契约变更。
  - `FrameBuild/`、`CSharpWar3Frame/`：不涉及。
  - `Projects/`：本提案不改示例。
  - Native 分层：禁止 `JassApi.Rect` / 原生 region 触发器。

## 方案摘要

```
RegionHelper.CreateRect / CreateCircle
  → 区域实体 + RegionComponent（可选 Occupancy 快照组件）

RegionSystem（Interval ~0.1s，须在 SpatialGridSystem 之后）
  → QueryRect / QueryCircle 得到当前集合
  → diff 上一快照
  → 新增单位：独立实体挂 RegionEnterEvent（region, unit）
  → 消失单位：独立实体挂 RegionLeaveEvent
  → 写回快照

监听方只读事件，不得删除事件实体。
```

形状首期只做矩形与圆。重复进出：同一单位连续两拍都在区内不重复 Enter；离开后再进再发 Enter。销毁区域时对仍占用单位发 Leave，再删区域实体。

## 风险与回滚

- 风险：
  1. Interval 与网格重建时序错位会导致漏检；系统 order 必须晚于 `SpatialGridSystem`。
  2. 区域多、单位多时每拍 Query 有成本；0.1s 与网格复用可接受，不做每帧。
  3. 与效果链 `GroundArea` 语义不同：GroundArea 是技能地面效果，Region 是持久空间条件。本提案不合并二者。
- 回滚：删除新增文件并去掉系统 attribute。无公开契约依赖。

## 验收标准

1. 单位进入矩形区域产生恰好一次 `RegionEnterEvent`。
2. 离开产生恰好一次 `RegionLeaveEvent`。
3. 区内停留不重复 Enter；再进再出不漏。
4. `dotnet build War3Frame/War3Frame.csproj` 0 错误。
5. 无任何新增 War3 原生 region/rect 触发器调用。

## 分级判定

- 影响范围：`War3Frame` 单模块新能力。
- 风险等级：低到中（新系统 + 事件，无跨项目、无 Native）。
- 可逆性：高。
- 是否跨项目：否。
- 是否改公共契约：否（新增类型，无既有 API 破坏）。
- 实施后审查：`R0 Direct`。若实施时系统 order 与事件清理路径需核对现有 Event 清理系统，可升 `R1 Focused`，不升 `full`。

## 后续事项

- 多边形、原生 rect 可视化同步另案。
- 不把 Region 接到 Trigger 规则引擎；监听方自行 Query 事件实体。
