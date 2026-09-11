# 设计：AsyncHelper 批量队列 + 周期 Flush 发送

配套提案：`batched-sync-send/proposal.md`

---

## 1. 现状

`AsyncHelper.Send(prefix, data)` 直调 `DzApi.DzSyncData(prefix, data)`：

- 无批量：同帧 N 次调用 = N 次原生发送。
- 无频率控制：不规避 `DzSyncData` 的每帧调用频率限制。
- 无长度控制：单次数据超长直接整体发送。

接收端 `SyncHelper.OnSyncReceived` 按 `DzGetTriggerSyncPrefix` 分发，数据按 `|` 拆成 `action + args`，单消息模型。

## 2. 目标

- 同 prefix 的待发消息合并发送，一次 flush 每 prefix 最多一次 `DzSyncData`。
- 单次发送 ≤512 字符，超长自动截断续发，不丢消息。
- 公开 API 签名不变；接收端解析兼容多消息。

## 3. 方案比较：flush 调度载体

| 方案 | 机制 | 优点 | 缺点 |
|---|---|---|---|
| A：ECS 系统驱动 | 新增 `SyncFlushSystem`（`[SystemRegister(Interval, order)]`，间隔 0.07s），每轮 flush 所有非空队列 | 符合框架分层（定时推进归 ECS）；生命周期由 `TimedSystemRoot` 管理，随游戏启停 | 新增一个系统注册；flush 逻辑与 SyncHelper 静态状态需要共享访问 |
| B：原生 Timer | `SyncHelper` 内部 `JassApi.CreateTimer` + `TimerStart` | 内聚，不新增系统 | 原生 timer 生命周期需自行管理；框架规则倾向由 ECS 统一驱动定时推进 |

**推荐方案 A**：与框架"定时推进归 ECS 系统"的既有模式一致，避免原生 timer 游离于系统树之外。`SyncFlushSystem` 只做纯发送调度，不承载业务决策，符合 Native 分层规则（`DzApi.DzSyncData` 属于框架内既有的同步执行点，不新增原生调用语义）。

## 4. 数据设计

### 4.1 队列

```csharp
// SyncHelper 内部（静态，单线程访问，无需并发容器）
private static readonly Dictionary<string, List<string>> _pending = new(StringComparer.Ordinal);
private static readonly HashSet<string> _flushing = new(StringComparer.Ordinal); // 已有周期 flush 的 prefix
```

- key = prefix（"inv"/"abl"/"buf"...），与现有 `DzTriggerRegisterSyncData` 频道一致。
- 同一 prefix 的消息保序（List 追加），跨 prefix 相互独立。
- 上限保护：单 prefix 队列积压超过阈值（如 64 条）时丢弃最旧并记录诊断，防止异常路径无限增长。

### 4.2 消息格式

```
单条：   action|arg1|arg2      （现有格式不变）
批量：   m1||m2||m3            （同一 prefix 的多次 Send 合并）
```

- 分隔符 `||` 与现有 `|` 分层：`|` 拆字段，`||` 拆消息。
- 约束：消息内容不得包含 `|`/`||`（沿用现有约束，调用方通过 `SyncHelper.Encode` 生成合法内容）。

### 4.3 flush 流程

```
Send(prefix, data):
  1. _pending[prefix].Add(data)
  2. 若 prefix 不在 _flushing：立即 flush 一次；加入 _flushing

Flush(prefix):                       // SyncFlushSystem 每 0.07s 对 _flushing 中 prefix 执行
  1. 从 _pending[prefix] 依次取消息拼 "||"，超过 512 字符截断
  2. 剩余消息留在队列
  3. 若拼出内容：DzApi.DzSyncData(prefix, payload)
  4. 若队列空：移出 _flushing（周期 flush 停止）
```

- 立即 flush + 周期续 flush 与 lik 的 `sync.query` 策略一致，保证首批消息低延迟、持续产生时不堆积。
- 接收端 `OnSyncReceived`：`data.Split("||")` 逐条，再按 `|` 拆字段分发；空串跳过。

## 5. 边界与异常

- 单条消息本身 >512 字符：保持单条原样发送（不做条内截断，避免破坏字段结构），接收端按现有逻辑处理。
- 队列积压阈值：超限丢最旧 + `War3.ReportNativeSafetyIssue` 诊断。
- flush 频率：0.07s（约 7 帧），与 lik 一致；后续若需更低延迟可调，不属本提案范围。
- 游戏结束清理：`SyncHelper` 静态队列在重建场景时清空（现有 `_initialized` 重置路径一并处理）。

## 6. 兼容性

- `AsyncHelper` 全部重载签名不变。
- `SyncHelper.Register/Encode/Decode/EncodeEntity` 等公开 API 不变。
- 接收端新增 `||` 拆分逻辑，对单消息数据（无 `||`）行为与旧版完全一致，可向后兼容。
- 同步两端必须同版本部署（wire format 变化属框架内自洽契约）。

## 7. 验证

- 单元级：构造同 prefix 多消息队列，验证合并、512 截断、续发、队列空停止。
- 集成级：`Projects/test` 中高频调用 `AsyncHelper.SendAction` 场景，验证接收端逐条执行且无丢失。
- 构建：`dotnet build War3Frame/War3Frame.csproj` 0 错误。