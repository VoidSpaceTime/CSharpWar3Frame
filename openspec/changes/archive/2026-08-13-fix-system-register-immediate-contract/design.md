# 设计：SystemRegister Immediate 契约修复

## 1. 目标

修复 `SystemGenerator` 无法识别 `SystemKind.Immediate` 的契约断裂，恢复立即系统的注册目标与刷新语义，同时保证 Interval 系统行为完全不变。

## 2. 根因分析

### 2.1 枚举取值错误

Roslyn `TypedConstant.Value` 对枚举参数返回**底层整数值**（boxed `int`）。`SystemKind.Interval = 0`、`SystemKind.Immediate = 1`，因此：

```csharp
var kind = kindArg.Value?.ToString();  // "0" 或 "1"
```

永远不等于字符串 `"Immediate"`。

### 2.2 修复方式对比

| 方案 | 做法 | 优点 | 缺点 |
|------|------|------|------|
| A. 解析枚举成员名（推荐） | 从 `kindArg.Type`（`INamedTypeSymbol`）遍历 `IFieldSymbol`，按 `ConstantValue` 匹配，取成员名 | 生成器不硬编码枚举值；输出仍是 `"Interval"/"Immediate"` 字符串，`== "Immediate"` 契约不变 | 代码略增（~10 行） |
| B. 数值比较 | `Convert.ToInt32(kindArg.Value) == (int)SystemKind.Immediate` | 更短 | 生成器不能引用运行时枚举类型（Analyzer 不引用输出程序集），需硬编码 `1`，与枚举声明顺序耦合，脆弱 |
| C. 改属性构造传字符串 | 属性加字符串参数 | 无 Roslyn 枚举解析 | 改公共属性契约，需要迁移所有 17 处标注，范围扩大 |

**选择方案 A**：改动最小、契约稳定、不引入硬编码。

## 3. ImmediateRoot 设计与 flush 时机

### 3.1 类型选择

`Game.ImmediateRoot` 使用 friflo 原生 `SystemRoot`：

```csharp
public static SystemRoot ImmediateRoot { get; private set; }

public static void ECSInit()
{
    Root = new TimedSystemRoot(Store);
    ImmediateRoot = new SystemRoot(Store);
    RegisterGeneratedSystems();
}
```

理由：
- Immediate 系统是"触发即执行"，不需要 interval 调度。
- `SystemRoot.Update()` 每次会执行其下全部系统，符合"立即 flush"语义。
- 与历史实现（`57ca030`）一致。

### 3.2 flush 入口

```csharp
/// <summary>
/// 立即刷新 ImmediateRoot 下的系统。
/// </summary>
public static void FlushImmediateSystems()
{
    ImmediateRoot?.Update(default);
}
```

### 3.3 flush 时机决策

当前无任何 flush 调用点。候选方案：

| 方案 | 做法 | 优点 | 缺点 |
|------|------|------|------|
| A. 主循环每 tick flush（推荐） | `War3Init` 计时器回调中 `Root.Update(tick)` 后调用 `FlushImmediateSystems()` | 语义近似"立即"；无需逐个补 helper 调用点；修复一个断点同时解决注册与刷新 | Immediate 系统每 0.01s 执行一次，频率高于原轮询 |
| B. 保持手动 flush | 恢复 `FlushImmediateSystems()` 但不在主循环调用，由写 request 的业务 helper 显式调用 | 语义最精确 | 当前无任何调用点，需要逐个补齐 17 处系统的触发路径，工作量大、易遗漏 |

**选择方案 A**。理由：
- 当前所有 Immediate 系统都是"查询 request 组件后消费"的守卫型系统，无 request 时空转开销极低。
- 每 tick flush（0.01s）对绝大多数 request 场景足够及时，且相比当前实际生效的 0.03125s 轮询是行为改善而非劣化。
- 避免"修好枚举却没人 flush 导致全挂"的回归风险。

### 3.4 ITimedSystem 与 Immediate 的共存

标注 `[SystemRegister(SystemKind.Immediate)]` 且实现 `ITimedSystem` 的系统（如 `CastRequestSystem` Interval=0.02）：
- 进入 `ImmediateRoot`（`SystemRoot`）后，其 `Interval` 不再被读取。
- 行为变为每 tick flush 执行一次（0.01s），比原 0.02s 更频繁。
- 这类系统都有 request/state 组件守卫，空转开销可忽略。

## 4. 生成代码形态

修复后 `Game.SystemRegistration.g.cs` 应为：

```csharp
static partial void RegisterGeneratedSystems()
{
    Root.Add(new ProjectileSystem());
    ImmediateRoot.Add(new CastRequestSystem());
    ImmediateRoot.Add(new ItemUseSystem());
    ImmediateRoot.Add(new UnitCreateNativeSystem());
    // ... Interval 进 Root，Immediate 进 ImmediateRoot
}
```

`SystemGenerator` 按 `Order` 升序、类名稳定的排序规则保持现状。

## 5. 验证方案

1. 构建 `War3Frame.Generator`，确认枚举解析逻辑编译通过。
2. 构建 `War3Frame`，确认 `Game.ImmediateRoot` 被生成代码引用且可解析。
3. 反编译 / 字符串搜索 `War3Frame.dll`，确认 `ImmediateRoot` 符号与 `RegisterGeneratedSystems` 存在。
4. 静态核对生成代码中 17 个 Immediate 系统进入 `ImmediateRoot.Add(...)`。
5. `Projects/test` 运行时验证：施法、物品使用、单位创建/移除、生命周期推进在每 tick flush 下行为正确。
6. 确认 Interval 系统（`UnitNativeSystem` 等）注册行为与修复前完全一致。

## 6. 边界与拒绝项

- **不迁移 Request 模式**：`GAMEPROCESSOR_REFACTOR.md` 描述的 GameProcessor 直调方向不在本提案范围。
- **不删除 17 处 `SystemKind.Immediate` 标注**：标注是正确意图，bug 在生成器取值。
- **不改变 Interval 系统语义**：生成器对 `"Interval"` 分支逻辑不动。
- **不新增性能系统**：flush 频率由主循环 tick 决定，不引入额外定时器。
