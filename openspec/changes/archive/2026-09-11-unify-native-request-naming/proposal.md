# 提案：统一 Native 请求与系统命名

- Change ID: `unify-native-request-naming`
- 提案等级: `light`
- 状态: `已实施`
- 目标一句话: 把 Native 副作用意图统一为 `{领域}{动作}NativeRequest`，系统统一为 `{领域}{动作}NativeSystem`，并修正现有混用与拼写错误。
- 请求来源: 用户要求统一 `NativeUnitCreateRequest` 一类命名。
- 默认实施后审查强度: `R0 Direct`
- 命中的审查升级触发器: 公共组件标识符重命名（无行为变化）
- 最终实施后审查强度: `R1 Focused`（核对全部引用替换与编译）
- Oracle 可用性与 `R1` 回退方式: 编译 + 引用扫描；Oracle 可用时做命名一致性核对
- 完整 `review-work` 授权来源: 无

## 3.1 背景与目标

`AGENTS.md` 规定 Native 副作用意图用 `XxxNativeRequest`，但现有代码和文档正例混用两种词序：

- `NativeUnitCreateRequest`、`NativeItemCreateRequest`（Native 在前）
- `PlayerNameNativeRequest`、`MoveNativeCommandRequest`（Native 在中/后）

系统侧还有拼写错误：`UnitMoveNaitveSystem`。

目标：只统一**请求组件**和**执行系统**的标识符，不改运行时行为。

## 3.2 影响范围

- 模块：`War3Frame` 组件/系统/Helper、`AGENTS.md`
- 文件：
  - `War3Frame/Src/Components/Unit/Units.cs`
  - `War3Frame/Src/Components/Item/Items.cs`
  - `War3Frame/Src/Components/MoveCommand.cs`
  - `War3Frame/Src/Systems/Native/UnitCreateNativeSystem.cs`
  - `War3Frame/Src/Systems/Native/ItemCreateNativeSystem.cs`
  - `War3Frame/Src/Systems/Native/UnitMoveNaitveSystem.cs`（改名）
  - `War3Frame/Src/TemplateInit/UnitTemplateAttribute.cs`
  - `War3Frame/Src/Helpers/UnitHelper.cs`
  - `War3Frame/Src/Systems/Unit/MoveSystem.cs`
  - `AGENTS.md`
- 不受影响区域：`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/`（无这些类型的作者向引用则不改；实施时全仓扫描确认）
- 不改：`UnitNative` / `PlayerNative` / `EffectNative` 句柄组件；`MoveCommand` 业务命令

## 3.3 方案摘要

统一公式：

```text
句柄/快照     XxxNative
副作用意图    {领域}{动作}NativeRequest
执行系统      {领域}{动作}NativeSystem
```

重命名：

| 现名 | 新名 |
|---|---|
| `NativeUnitCreateRequest` | `UnitCreateNativeRequest` |
| `NativeItemCreateRequest` | `ItemCreateNativeRequest` |
| `MoveNativeCommandRequest` | `MoveNativeRequest` |
| `UnitMoveNaitveSystem` | `UnitMoveNativeSystem` |

保持不变（已符合）：

- `PlayerNameNativeRequest`
- `PlayerColorNativeRequest`
- `PlayerAllianceNativeRequest`
- `UnitCreateNativeSystem`、`ItemCreateNativeSystem`、`EffectNativeSystem`、`PlayerNativeSystem`

`MoveNativeCommandRequest` 去掉 `Command`：业务层已有 `MoveCommand`；Native 请求只表示“请执行原生命令”，不应再叠一层 Command。

同步修正 `AGENTS.md` 正例，删除混用。

## 3.4 风险与回滚

- 风险：公开组件名变更，未扫全的引用会编译失败。缓解：全仓符号替换后 `dotnet build War3Frame/War3Frame.csproj`。
- 无运行时语义变化；回滚即还原标识符。

## 3.5 验收标准

- 仓库内不再出现 `NativeUnitCreateRequest`、`NativeItemCreateRequest`、`MoveNativeCommandRequest`、`Naitve`。
- `AGENTS.md` 正例只使用 `{领域}{动作}NativeRequest`。
- `dotnet build War3Frame/War3Frame.csproj` 通过。
