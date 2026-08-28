# 提案：把历史 Request:ITag 改成符合命名规则的类型

- Change ID: `fix-historical-request-tags`
- 提案等级: `light`
- 状态: `已实施`
- 目标一句话: 把 6 个 `XxxRequest : ITag` 按 `AGENTS.md` 规则改为 `IComponent` 请求，弹道到达/过期仍由后续阶段 Tag 表达。
- 请求来源: 用户要求按命名规则修正现存 `Request : ITag`。
- 默认实施后审查强度: `R0 Direct`
- 命中的审查升级触发器: 公共组件标识符类型变更（无业务流程改写）
- 最终实施后审查强度: `R1 Focused`（引用扫描 + 编译）
- Oracle 可用性与 `R1` 回退方式: 全仓符号扫描与 `dotnet build War3Frame/War3Frame.csproj`
- 完整 `review-work` 授权来源: 无

## 3.1 背景与目标

`AGENTS.md` 规定意图必须是 `IComponent` 的 `XxxRequest`，即使零字段也不允许做成 `ITag`。仓库仍有 6 处历史违规：

- `ItemAttrApplyRequest` / `ItemAttrRemoveRequest`
- `AbilityAttrApplyRequest` / `AbilityAttrRemoveRequest`
- `ProjectileArriveRequest` / `ProjectileExpireRequest`

属性四项是真正的一次性意图，改成空 `IComponent` 即可。弹道两项也是“请下一系统结算”的意图，不是已完成阶段；`ProjectileArrived` 仍保留为内部阶段 `ITag`。

## 3.2 影响范围

- 模块：`War3Frame` 组件、物品/技能属性系统、弹道结算、Helper、`AGENTS.md`
- 不受影响区域：`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/`（无这些类型的作者向引用）
- 不改：属性 modifier 计算、装备/卸下/施法工作流、弹道轨迹公式、Native 调用

## 3.3 方案摘要

| 现名 | 新类型 | 说明 |
|---|---|---|
| `ItemAttrApplyRequest` | `IComponent` | 保留名字 |
| `ItemAttrRemoveRequest` | `IComponent` | 保留名字 |
| `AbilityAttrApplyRequest` | `IComponent` | 保留名字 |
| `AbilityAttrRemoveRequest` | `IComponent` | 保留名字 |
| `ProjectileArriveRequest` | `IComponent` | 意图；结算后仍打 `ProjectileArrived` |
| `ProjectileExpireRequest` | `IComponent` | 意图；过期路径仍打 `EffectExpired` |

查询从 `Filter.AnyTags` 改为把请求放进 `QuerySystem<...>` 泛型；写入/删除从 `AddTag`/`RemoveTag` 改为 `AddComponent`/`RemoveComponent`。

同步删除 `AGENTS.md` 中这 6 项“新代码不要复制”的反例。

## 3.4 风险与回滚

- 风险：漏改 `Tags.Get` / `AddTag` 会导致编译失败。缓解：全仓扫描后编译。
- 无运行时语义改写；回滚即还原类型与 API。

## 3.5 验收标准

- 仓库内不再出现 `XxxRequest : ITag`。
- `dotnet build War3Frame/War3Frame.csproj` 通过。
