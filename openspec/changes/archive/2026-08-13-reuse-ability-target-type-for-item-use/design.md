## 1. 契约统一

`ItemUseTarget` 不再定义独立的 Item 目标类型：

```csharp
public struct ItemUseTarget : IComponent
{
    public AbilityTargetType kind;
    public Entity targetUnit;
    public float targetX;
    public float targetY;
}
```

`ItemUseTargetKind` 直接删除，不保留别名、转换扩展或兼容分支。

## 2. 目标规范化

- `None`：以使用者自身作为 `targetUnit`，记录使用者当前位置快照。
- `Unit`：要求目标实体与请求位于同一 `EntityStore`，并记录有限且位于边界内的单位坐标。
- `Point`：要求 `targetX/targetY` 有限且位于边界内，并清空 `targetUnit`。
- `Area`：与 `Point` 使用相同的坐标校验和存储形式，但坐标语义为范围中心，并保留 `AbilityTargetType.Area`。

## 3. 匹配规则

ItemUse 请求只有在以下条件成立时才能派发 `CastRequest`：

```text
ItemUseTarget.kind == companion AbilityBase.targetType
```

不再允许 `Point` 请求兼容 `Area` Ability，也不进行其他目标类型降级。调用方必须根据 Ability 的目标类型产生准确请求。

## 4. 下游边界

`CastRequest` 和 `CastState` 继续使用 `targetUnit/targetX/targetY` 传递规范化结果，不新增第二个目标类型字段。Casting、Channeling、Cooldown 和 Effect 流程不改；本变更不新增 War3 Native 调用。

## 5. 实施顺序

1. 先更新验证场景，覆盖四种精确匹配和 `Point/Area` 交叉拒绝。
2. 删除 enum，并修改组件、helper 与 `ItemUseSystem`。
3. 执行旧符号扫描、构建和纯 ECS 场景。
4. 单独记录真实 War3 客户端结果。
