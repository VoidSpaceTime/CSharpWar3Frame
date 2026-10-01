## Purpose

规定 `AsyncHelper` / `SyncHelper` 批量同步传输的可观察契约：发送侧改为按 prefix 入队并对同一 prefix 合并 flush，单次发送长度受上限约束且超长自动续发，接收侧先拆多消息再按既有字段格式解析；`AsyncHelper` / `SyncHelper` 公开方法签名保持不变。

## ADDED Requirements

### Requirement: 发送侧入队与 flush 调度

系统 SHALL 将 `AsyncHelper.Send(prefix, data)` 的发送由即时直发改为写入按 prefix 的待发队列。某 prefix 首次入队时，系统 SHALL 立即执行一次 flush，并 SHALL 建立周期 flush（设计值约 0.07s）；队列清空后 SHALL 停止该 prefix 的周期 flush。同一 prefix 内消息 MUST 保持入队顺序，不同 prefix MUST 相互独立。一次 flush 对每个 prefix MUST 最多执行一次 `DzSyncData`。

#### Scenario: 首次入队立即 flush

- **WHEN** 调用方对某 prefix 调用 `AsyncHelper.Send`，且该 prefix 当前没有待发消息
- **THEN** 系统立即对该 prefix 执行一次 flush，发送其待发消息，并建立周期 flush 调度

#### Scenario: 周期续发剩余消息

- **WHEN** 某 prefix 在首次 flush 后队列仍有剩余消息
- **THEN** 系统在后续周期 flush 中继续发送剩余消息，直到队列清空

#### Scenario: 队列清空后停止周期 flush

- **WHEN** 某 prefix 的待发队列被清空
- **THEN** 系统停止该 prefix 的周期 flush，不再产生空的 `DzSyncData`

### Requirement: 单次发送长度上限与截断续发

系统 SHALL 将同一 prefix 的待发消息拼接为批量载荷发送。单次发送载荷 MUST 不超过 512 字符；超出的消息 MUST 留在队列由后续 flush 续发，MUST NOT 丢失。单条消息自身超过 512 字符时，系统 MUST 将该条原样发送，MUST NOT 在消息内部截断。

#### Scenario: 超长批量截断续发

- **WHEN** 某 prefix 的待发消息拼接后超过 512 字符
- **THEN** 系统只发送不超过 512 字符的部分，剩余消息保留在队列并在下一轮 flush 续发

#### Scenario: 单条超过上限原样发送

- **WHEN** 单条待发消息自身长度超过 512 字符
- **THEN** 系统按该条原样发送，不改写或截断其内部字段结构

### Requirement: 接收端多消息解析

`SyncHelper.OnSyncReceived` SHALL 先按 `||` 将同步数据拆分为多条消息，再按既有 `|` 字段格式逐条解析分发；空消息片段 MUST 被跳过。对不含 `||` 的同步数据，接收行为 MUST 与批量特性引入前一致。

#### Scenario: 批量数据逐条分发

- **WHEN** 接收到含 `||` 分隔的同步数据
- **THEN** 系统按 `||` 拆分为多条消息，并逐条按 `|` 拆分字段后分发

#### Scenario: 单条数据兼容

- **WHEN** 接收到不含 `||` 的同步数据
- **THEN** 系统将其视为单条消息按 `|` 解析分发，行为与批量特性引入前一致

#### Scenario: 空消息片段跳过

- **WHEN** `||` 拆分结果包含空字符串片段
- **THEN** 系统跳过该片段，不产生分发

### Requirement: 队列积压保护

系统 SHALL 对单 prefix 的待发队列设置长度上限（设计值 64 条）。超过上限时，系统 SHALL 丢弃最旧消息并记录诊断，MUST NOT 让队列无限增长。

#### Scenario: 超过上限丢弃最旧消息

- **WHEN** 某 prefix 的待发队列已达到上限后继续入队
- **THEN** 系统丢弃最旧消息并记录诊断，队列长度保持在上限内

### Requirement: 公开 API 兼容

本变更 MUST NOT 改变 `AsyncHelper` 与 `SyncHelper` 的公开方法签名。

#### Scenario: 既有调用方无感知

- **WHEN** 现有调用方按原签名调用 `AsyncHelper` / `SyncHelper` 公开 API
- **THEN** 调用保持可编译，语义不变
