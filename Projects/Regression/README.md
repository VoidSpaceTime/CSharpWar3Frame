# SDK 回归宿主

在仓库根目录使用 Windows 和 .NET 10 SDK 执行，无需启动 War3 或 WE：

```powershell
dotnet build Projects/Regression/Regression.csproj -c Release
dotnet Projects/Regression/bin/Release/net10.0/War3Frame.Regression.dll
```

使用不在 PATH 中的 SDK 时，用该 `dotnet.exe` 的完整路径运行以上两条命令；子进程默认复用当前 SDK，也可用 `DOTNET_HOST_PATH` 指定。首次构建及发布需要还原 NuGet 包。测试必须从仓库内的构建输出运行，以便读取实际源码。

宿主打印每个场景的 PASS/FAIL，并在任一失败或组名无匹配时返回非零。可在 DLL 路径后追加一个组名：

| 组名 | 验证范围 |
|---|---|
| `existing` | 直接复用 test 中的 11 个纯 ECS 场景 |
| `runtime` | 调度、Once、Buff、属性 Store、阵营过滤、光环、装备、技能等级、同步身份、空间索引及默认注册 |
| `generator` | 实际 Roslyn 生成器与编译输出；框架/消费者、零模板、符号、字符串、诊断及顺序 |
| `native-projection` | 编译原样 Native 系统源码，以记录器替换原生函数，检查 ECS 投影和调用顺序 |
| `build` | 真实受控子进程、构建失败注入、文件同步、资源转换的实际 MSBuild 输入、模型缓冲区 |
| `integration` | 实际 CLI 退出码、New 脚手架和 test 的 win-x86 JIT publish、ModelFormat 非交互运行 |

`native-projection` 还测量预热后的静止特效 1,000 次更新，要求无重复原生调用和每轮托管分配。该结果只描述此受控场景，不代表游戏内所有特效路径均无分配。

构建测试使用独立的 `War3Frame-regression-<随机值>` 临时目录，并在场景结束后清理。脚手架验证从当前框架和 demo 源码复制，排除 bin/obj 等缓存；test 和 ModelFormat 的集成构建会刷新各自的正常 bin/obj 输出。CLI 配置使用临时夹具，不修改用户的 appsettings.yml。没有启动游戏或编辑器。

此宿主不验证真实 War3 原生实现、客户端视觉、多人锁步、AOT 或 C++/CLI 宿主组合。相应 Native TODO 保留；这些结果不能替代客户端集成验证。
