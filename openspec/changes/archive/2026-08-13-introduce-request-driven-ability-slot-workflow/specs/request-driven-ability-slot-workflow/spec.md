## ADDED Requirements

### Requirement: Ability slot operations MUST be request-driven immediate workflows
技能槽位的 attach/remove/swap/resize MUST 通过 request-driven immediate workflow 处理，而 SHALL NOT 主要依赖周期轮询系统。

#### Scenario: Ability is attached to a unit slot
- **WHEN** 某个技能被请求挂载到单位槽位
- **THEN** the slot workflow MUST process it as a discrete request

### Requirement: Helper SHALL NOT remain the primary slot workflow owner
helper SHALL NOT 继续作为主要槽位业务流程 owner。

#### Scenario: Caller adds an ability through helper API
- **WHEN** 调用方通过 helper 发起技能装配
- **THEN** helper SHOULD enqueue or forward a request rather than directly own the full workflow semantics

### Requirement: Canonical workflow layer MUST own slot state mutation
canonical immediate workflow layer MUST 统一负责：合法性检查、`AbilityOwner`、`AbilitySlotIndex`、`AbilitySlotContainer.currentCount` 的变更，以及 contribution request 的触发。

#### Scenario: A slot mutation occurs
- **WHEN** 某个技能槽状态发生变化
- **THEN** the canonical workflow layer MUST be the owner of that mutation

### Requirement: Remove and destroy SHOULD be separable
技能从槽位移除与技能实体销毁 SHOULD 保持可分离。

#### Scenario: Ability is detached from a slot
- **WHEN** 某个技能从槽位解绑
- **THEN** the design SHOULD allow detaching without necessarily forcing entity destruction

### Requirement: Swap SHOULD NOT re-trigger contribution application by default
槽位交换 SHOULD 优先被视为索引变更，而 SHALL NOT 默认走 remove+attach 双流程。

#### Scenario: Two equipped abilities swap slots
- **WHEN** 两个技能交换槽位
- **THEN** contribution semantics SHOULD remain stable unless explicit design says otherwise
