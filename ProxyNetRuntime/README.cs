/*
================================================================================
                    ProxyNetRuntime 使用说明
================================================================================

## 项目结构

ProxyNetRuntime/
├── ProxyNetRuntime.csproj   -- 项目配置（AOT 发布）
├── HostFxr.cs               -- hostfxr API 绑定
├── RuntimeLauncher.cs       -- War3 入口点
└── README.cs                -- 本文件

## 工作原理

1. War3 通过 P/Invoke 调用 ProxyNetRuntime.dll 的 `main` 函数
2. ProxyNetRuntime 使用 hostfxr API 在当前进程中启动 .NET CLR
3. CLR 加载 GameLogic.dll（JIT 编译）并调用其 MainAOT 方法
4. GameLogic.dll 中的代码可以直接访问 JassVM（因为在同一进程内）

## 使用方法

### 1. 发布 ProxyNetRuntime（AOT，只需一次）

```powershell
cd ProxyNetRuntime
dotnet publish -c Release --aot -r win-x64 -o ../output/launcher
```

输出:
- output/launcher/ProxyNetRuntime.dll  -- 给 War3 调用的原生 DLL

### 2. 编译你的游戏逻辑（JIT，每次修改后）

```powershell
cd Projects/test
dotnet publish -c Debug -o ../../output/launcher
```

输出:
- output/launcher/GameLogic.dll
- output/launcher/GameLogic.runtimeconfig.json
- 其他依赖 DLL

### 3. 部署到 War3

复制 output/launcher/ 目录下所有文件到 War3 插件目录

### 4. 开发迭代流程

```
修改代码 →  dotnet build  →  复制 DLL  →  重启 War3 测试
    ↑         (秒级)          (秒级)
    └──────────────────────────────────────────────┘
```

## 文件放置要求

War3 插件目录结构:
```
plugins/
├── ProxyNetRuntime.dll           -- AOT 启动器（War3 直接调用）
├── GameLogic.dll                 -- 你的游戏逻辑（JIT）
├── GameLogic.runtimeconfig.json  -- 运行时配置
├── War3Frame.dll                 -- 框架依赖
└── 其他依赖...
```

## 自定义配置

### 修改默认加载的 DLL

在 RuntimeLauncher.cs 中修改:
```csharp
private const string DEFAULT_JIT_DLL_NAME = "GameLogic.dll";
private const string DEFAULT_ENTRY_TYPE = "War3Frame.Game";
private const string DEFAULT_ENTRY_METHOD = "MainAOT";
```

### JIT DLL 的入口方法签名

你的 JIT DLL 中的入口方法必须符合以下签名:
```csharp
public static int MainAOT()
{
    // 你的游戏初始化代码
    return 0;
}
```

注意：JIT DLL 中的方法不需要 [UnmanagedCallersOnly] 特性！
hostfxr 会通过反射调用它。

## 常见问题

### Q: 找不到 hostfxr.dll
A: 确保安装了 .NET Runtime，或设置 DOTNET_ROOT 环境变量

### Q: 加载 DLL 失败
A: 检查:
   1. GameLogic.dll 和 .runtimeconfig.json 是否在同一目录
   2. 所有依赖 DLL 是否都复制了
   3. .NET 版本是否匹配

### Q: 方法找不到
A: 检查类型名和方法名是否完全匹配（区分大小写）

================================================================================
*/
