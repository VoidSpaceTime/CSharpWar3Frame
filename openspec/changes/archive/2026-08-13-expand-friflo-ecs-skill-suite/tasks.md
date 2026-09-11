# 任务：扩展 Friflo ECS 项目级 Skill 套件

> 只有在本提案获得用户明确批准后，才执行以下任务。

## 1. 建立核心 Skill

- [x] 新增 `friflo-ecs-core`，覆盖 `3.4.2` 下的 Store、Entity、Component、Tag 和结构变更规则。
- [x] 新增 `friflo-ecs-query`，覆盖 Query、Filter、迭代与延迟结构变更边界。
- [x] 新增 `friflo-ecs-systems`，覆盖 Friflo 系统树及 CSharpWar3Frame 集成边界。

## 2. 校订关系 Skill

- [x] 逐项核验 `friflo-ecs-relations` 的行为、性能断言和示例是否适用于 `3.4.2`。
- [x] 明确 Relations、Relationships 与普通请求组件的职责边界。

## 3. 补充来源与版本信息

- [x] 为四个 skill 增加官方主题页、API Reference 和版本基线。
- [x] 将 3.5/3.6 内容统一标为升级后候选并注明最低版本。
- [x] 记录当前未采用的条件性能力及引入前检查。

## 4. 验证

- [x] 核对所有仓库路径和代码标识符可检索。
- [x] 按覆盖矩阵检查当前项目主路径均有唯一主 skill。
- [x] 交叉检查四个 skill 无冲突、无大段官方正文复制。
- [x] 检查 Git diff，确认未改动运行时代码与项目行为。
