## Why

当前 `run test -r` 的发布链存在明确的产物模型冲突：`FrameBuild/CommandManager/Run.cs` 在 Release 模式下固定执行 `dotnet publish ... -p:PublishAot=true`，而 `Projects/test/test.csproj` 又显式引用了 `DNNE` 并启用了动态加载相关属性。这导致发布流程在 DNNE 的原生导出工具链阶段失败，并因缺少 Win10 SDK 而中止，最终地图运行期所需模块未生成。

更关键的是，上层 callback/runtime integration 期望加载的是模块 DLL，而不是一个抽象的“随便什么 publish 产物”。因此，这不是单纯的环境问题，而是**Release 发布策略与项目实际运行时模块模型不一致**。

## What Changes

- 统一 Release 发布链与项目实际运行时模块模型。
- 明确 JIT/DNNE 路线与 AOT 路线不能继续在同一个发布分支中混用。
- 定义 `Projects/test` 在 Release 下应采用的唯一产物策略。
- 约束 callback/module naming 与生成产物必须一致。

## Capabilities

### New Capabilities
- `release-publish-runtime-module-alignment`: 定义 Release 发布链与运行时模块模型的一致性要求。

## Impact

- 直接影响 `FrameBuild/CommandManager/Run.cs` 的发布策略。
- 直接影响 `Projects/test` / `Projects/demo` 的 csproj 发布行为与模块生成方式。
- 间接影响 callback 写入逻辑、地图运行期模块加载路径与最终黑屏问题。
- 本次变更仅新增 OpenSpec 工件，不进入代码实现。
