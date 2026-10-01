## ADDED Requirements

### Requirement: 代次安全的实体身份
同步 token SHALL 包含版本、ID 与 Friflo revision；编码只接受当前同步 Store 的存活实体，解码验证该 Store 内的 ID 与代次，不接受裸 ID 降级。token 仅用于当前会话，不作为跨 Store、跨会话或无限次 ID 复用后的永久身份。

#### Scenario: ID 回收
- **WHEN** A 编码后被删除，B 复用 A 的 ID
- **THEN** A 的旧 token 不得解码为 B

#### Scenario: 无效输入
- **WHEN** 输入旧格式、越界、畸形、已销毁 token 或无 Store
- **THEN** 安全拒绝，不投递给其他有效实体

### Requirement: 协议迁移明确
新协议 SHALL 要求发送接收同版本，不含未来外层批量消息使用的分隔符。

#### Scenario: 混合版本
- **WHEN** 旧客户端发送没有代次的消息
- **THEN** 接收侧拒绝旧身份格式，不恢复有碰撞风险的 ID 查找
