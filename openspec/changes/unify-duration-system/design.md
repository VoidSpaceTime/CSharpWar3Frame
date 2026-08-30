# 设计：统一 Duration 组件 + 单一 DurationSystem

## 1. 现状与问题

仓库现有 3 处独立的"倒计时-到期"模式（第 4 处 `TimerInfo` 为任务调度，明确不纳入）：

| 领域 | 组件 | 字段 | 递减系统 | 到期动作 |
|---|---|---|---|---|
| 特效 | `EffectBase.duration` | `float duration`（-1 永久） | `EffectRuntimeSystem`（0.02s） | `EffectHelper.Destroy`（隐藏后销毁） |
| Buff | `BuffDuration` | `float duration` / `float remaining` / `bool isPermanent` | `BuffDurationSystem` | 移除 Buff（含过期前 TimerInfo 预警） |
| 地面区域 | `GroundAreaLifetime` | `float duration` / `float remaining` | `GroundAreaLifetimeSystem` | 删除区域 + `GroundAreaQueryHelper.DeleteAreaBuffs` |

每个领域重复"递减 → 判断到期 → 清理"循环。新领域（陷阱、临时光环、场地效果）会继续复制模板。

## 2. 目标设计

### 2.1 新组件 `Duration`

```csharp
/// <summary>
/// 统一持续时间组件。-1 永久，0 立即到期，>0 剩余秒数。
/// </summary>
public struct Duration : IComponent
{
    public float remaining;   // 剩余秒数；-1 = 永久
    public float total;       // 初始值（供进度显示）
}
```

位置：`War3Frame/Src/Components/Time/Duration.cs`（已有 `Time/` 领域目录）。

### 2.2 新内部阶段 Tag `DurationExpired`

```csharp
/// <summary>
/// 持续时间到期标记（内部阶段）。
/// </summary>
public struct DurationExpired : ITag;
```

位置：`War3Frame/Src/Components/Time/Duration.cs`。命名符合 AGENTS.md 过去式阶段规则（不加 `Tag` 后缀）。

### 2.3 新系统 `DurationSystem`

```csharp
/// <summary>
/// 统一持续时间推进系统。递减所有 Duration，到期打 DurationExpired。
/// 不做任何领域清理——到期动作由各领域系统消费 DurationExpired 执行。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class DurationSystem : QuerySystem<Duration>
{
    public float Interval => 0.02f;

    protected override void OnUpdate()
    {
        var expired = new List<Entity>();

        Query.ForEachEntity((ref Duration duration, Entity entity) =>
        {
            if (duration.remaining < 0f) return;          // -1 永久
            duration.remaining -= Tick.deltaTime;
            entity.AddComponent(duration);

            if (duration.remaining <= 0f)
            {
                duration.remaining = 0f;
                entity.AddComponent(duration);
                if (!entity.Tags.Has<DurationExpired>())
                    entity.AddTag<DurationExpired>();
            }
        });
    }
}
```

位置：`War3Frame/Src/Systems/Time/DurationSystem.cs`。注册顺序 0（与现有 EffectRuntimeSystem 同档，先于领域清理系统）。

### 2.4 领域迁移

#### Effect（`EffectBase.duration` → `Duration`）

- `EffectHelper.CreatePosition/CreateAttached`：`duration` 参数仍为公共签名（`-1/0/>0` 语义不变），内部改挂 `Duration` 组件；`0` 时保持现行为（转 0.02f 下一 tick 到期）
- `EffectRuntimeSystem` 改造：去掉递减逻辑，改为 `QuerySystem<EffectBase, DurationExpired>`——实体有 `DurationExpired` 时执行销毁（`EffectHelper.Destroy(hideFirst: true)`）+ 移除 Tag；保留 Attach 跟随逻辑（与 Duration 无关）
- 注意：`EffectBase.duration` 字段保留？—— 移除。所有引用点（EffectRuntimeSystem、EffectHelper 创建、AbilityEffectHelper 传递）迁移。

#### Buff（`BuffDuration` → `Duration`）

- `BuffDuration` 组件**保留**（含 `isPermanent`），但 `remaining` 递减移交给 `DurationSystem`？—— 不行，`BuffDuration.isPermanent` 语义是"remaining 不减少"，而 `Duration.remaining=-1` 等价。为最小迁移：Buff 实体同时挂 `Duration`，`BuffDuration` 瘦身为只读元数据（duration/isPermanent），`BuffDurationSystem` 改为监听 `DurationExpired` 移除 Buff。
- 更简单方案：Buff 直接挂 `Duration`，删除 `BuffDuration` 的 remaining 推进，`BuffHelper.CreateBuff` 挂 `Duration.Create(duration, permanent)`——`Duration` 增加 `isPermanent` 字段？还是用 -1？

设计决策：**`Duration` 不增加 isPermanent**，`-1` 即永久。`BuffDuration` 保留字段 `duration`（原始值，供刷新计算），删除 `remaining`/`isPermanent`（迁移到 Duration）。`BuffHelper.GetRemaining` 改读 `Duration.remaining`。
`BuffDurationSystem` 到期动作：移除 Buff 前需处理"过期前 TimerInfo 预警"——现状 `BuffDurationSystem` 在到期前创建 `TimerInfo(interval=remaining)` 用于 `BuffExpired` 触发。迁移：`DurationSystem` 统一打 `DurationExpired`，`BuffDurationSystem` 监听它做移除 + 触发 `BuffExpired` 逻辑（不再需要提前 TimerInfo，到期即触发）。

#### GroundArea（`GroundAreaLifetime` → `Duration`）

- 删除 `GroundAreaLifetime`，区域实体挂 `Duration`
- `GroundAreaLifetimeSystem` 改造：`QuerySystem<GroundAreaData, DurationExpired>`——监听到期，执行 `GroundAreaQueryHelper.DeleteAreaBuffs` + `DeleteEntity`

## 3. 备选方案比较

| 方案 | 说明 | 优点 | 缺点 |
|---|---|---|---|
| A（选定） | 统一 Duration + 统一递减 + 领域消费 DurationExpired | 消除重复；-1 语义统一；新领域零模板 | 跨 3 领域迁移成本 |
| B | 保持现状 | 零迁移 | 每新领域复制模板；4 套递减逻辑 |
| C | 统一 Duration + 到期动作回调注册表 | 动作也统一 | 过度设计；回调注册表与 AGENTS 分层冲突（动作应留在领域系统） |
| D | Duration 并入 TimerInfo/TimerTaskSystem | 一套计时 | 语义混杂（任务调度 vs 生命周期）；违背"触发器动作走请求"原则 |

## 4. 迁移路径（分 3 批）

1. **批 1 - 基础**：新增 `Duration` + `DurationExpired` + `DurationSystem`；不改任何现有领域（纯新增，可独立编译）
2. **批 2 - Effect**：`EffectBase.duration` → `Duration`；`EffectRuntimeSystem` 改造；`EffectHelper`/`AbilityEffectHelper` 引用迁移
3. **批 3 - Buff + GroundArea**：`BuffDuration` 瘦身 + `BuffDurationSystem` 改造；`GroundAreaLifetime` → `Duration` + `GroundAreaLifetimeSystem` 改造

每批完成后构建验证。

## 5. 风险与回滚

- **风险 1（Effect）**：`EffectBase.duration` 被多处读写（创建、递减、`EffectRuntimeSystem`）。缓解：批 2 单独实施，`EffectRuntimeSystem` 改造时保留 Attach 跟随逻辑。
- **风险 2（Buff）**：`BuffDuration` 的"过期前 TimerInfo 预警"逻辑（`BuffSystem.cs:37-38`）行为变化——原实现到期前创建 TimerInfo 用于 `BuffExpired`；新实现改为 `DurationExpired` 直接触发。需确认无消费方依赖"提前量"。
- **风险 3（GroundArea）**：`GroundAreaLifetimeSystem` 顺序（Interval 128）晚于 `DurationSystem`（0），`DurationExpired` 当帧可见，无顺序问题。
- **回滚**：批 1 纯新增可随时保留；批 2/3 为替换式迁移，`git revert` 对应提交即可完整回退。公共 authoring 签名（EffectHelper/BuffHelper）不变，模板层零影响。

## 6. 长期维护影响

- 新领域（陷阱/场地/临时单位）创建时挂 `Duration` 即可获得统一到期语义，无需复制递减代码
- `DurationExpired` 是内部阶段 Tag，对外广播另发领域 Event（如 `BuffExpiredEvent`），符合 AGENTS 消息规则
- `DurationSystem` 单查询性能：与现有 3 个递减系统总成本相当（合并遍历，实际更低）