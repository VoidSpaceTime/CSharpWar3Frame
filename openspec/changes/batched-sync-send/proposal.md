# 提案：AsyncHelper 批量队列 + 周期 Flush 发送

**状态**：待审核
**等级**：light（补 design.md）
**提案日期**：2026-08-31
**请求来源**：分析 lik 框架（`D:\Games\lik\library\foundation\sync.lua`）同步设计后提出的借鉴需求；用户指示"先生成需求暂时不落地"。

---

## 背景与目标

当前 `AsyncHelper.Send` 每次调用直接执行一次 `DzApi.DzSyncData(prefix, data)`，无批量、无频率控制。War3 1.27 的 `DzSyncData` 存在每帧调用频率限制与单次数据长度限制，高频操作（连点物品、同帧多事件）可能触发限制导致丢消息。

lik 框架的 `sync.lua` 用「队列合并 + 立即 flush 一次 + 周期续 flush + 单次长度截断」规避了该问题。本提案将该思路落地到框架的 `AsyncHelper/SyncHelper`：

1. 发送侧改为入队 + 批量 flush：同一 prefix 的待发消息合并为一次 `DzSyncData`。
2. 接收侧支持多消息解析：一次同步数据内可包含多条消息。
3. 公开 API 签名保持不变，业务调用方（`Projects/*`）无感知。

## 影响范围

- 模块：`War3Frame/Src/Core/Sync/`
- 文件：
  - `AsyncHelper.cs`：发送入口由"即时发送"改为"入队 + 触发 flush"。
  - `SyncHelper.cs`：新增按 prefix 的待发队列、flush 调度、接收端多消息解析（`||` 分隔）。
  - 可能新增 1 个 flush 调度载体（见 design.md 方案比较）。
- 不受影响区域：
  - `War3Frame.Generator/`：无生成器契约变化。
  - `FrameBuild/`、`CSharpWar3Frame/`：构建链与 CLI 不涉及。
  - `Projects/`：业务代码只通过 `AsyncHelper` 公开 API 调用，签名不变。
  - Native 分层规则：不新增任何 War3 原生调用点，仍只有 `SyncHelper/AsyncHelper` 接触 `DzApi`。

## 方案摘要

```
AsyncHelper.Send(prefix, data)
  → 写入 SyncHelper 的按 prefix 队列
  → 立即 flush 一次（若该 prefix 尚未有周期 flush 定时器）
  → 周期 flush（约 7 帧）持续清空队列，队列空则停止

flush 时：该 prefix 全部待发消息拼为 "m1||m2||..."，单次 ≤512 字符，
          超出部分留在队列下轮续发，一次 flush 每个 prefix 最多一次 DzSyncData。

接收端：SyncHelper.OnSyncReceived 先 explode("||") 拆多条，
        再按现有 "|" 格式逐条解析分发。
```

详见 `design.md`。

## 风险与回滚

- 风险：
  1. **同步协议格式变更**：接收端需要解析 `||` 多消息；两端同版本部署，无历史数据兼容问题，但属 wire format 契约变化。
  2. **消息延迟**：批量策略引入 0~7 帧延迟；首次立即 flush 缓解，对 UI 操作类语义可接受，时序敏感操作需评估。
  3. **队列积压**：异常路径下队列可能无限增长，需要上限保护。
- 回滚方式：恢复 `AsyncHelper.Send` 直发 + `SyncHelper` 单消息解析；队列结构为内部实现，不影响公开 API。成本低。

## 验收标准

1. 同帧多次 `Send(prefix, ...)` 合并为一次 `DzSyncData`（同 prefix）。
2. 单次发送字符串 ≤512 字符，超长消息下轮续发不丢失。
3. 现有调用方（`Projects/test` 等通过 `AsyncHelper` 的调用）行为语义不变。
4. `dotnet build War3Frame/War3Frame.csproj` 0 错误。
5. 调试场景验证：连续高频 Send 无丢消息、无异常增长。

## 分级判定

- 影响范围：`War3Frame/Src/Core/Sync/` 单模块。
- 风险等级：中（同步链路为框架核心机制，协议格式与发送时序变化）。
- 可逆性：高（内部实现，公开 API 不变，可快速回滚）。
- 是否跨项目：否。
- 是否改公共契约：`AsyncHelper/SyncHelper` 公开签名不变；同步 wire format 变化（`||` 多消息），故审查强度按 `R2 Targeted` 执行（协议契约 + 性能/实时性两个视角）。

## 后续事项

- 实施时若发现单机降级（lik 的 `playingQuantityStart() <= 1` 本地直执行）有明确收益，另行提案评估。
- 不做 lik 的同帧去重（`_queue[aid][sData]` 语义会吞掉同帧相同消息，属于设计缺陷，不采纳）。