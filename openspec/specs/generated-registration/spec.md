# generated-registration Specification

## Purpose
定义框架与消费项目的编译期模板注册、合法确定的代码生成、诊断及 demo/test/脚手架启动契约。

## Requirements

### Requirement: 按程序集注册模板
生成器 SHALL 为消费者生成自身的显式注册入口，调用框架公共注册 API；不得用跨程序集 partial 扩展框架。

#### Scenario: 框架与项目分离
- **WHEN** 三类模板声明在引用框架的项目中
- **THEN** 项目注册入口调用后 Unit/Ability/Item 模板全部可查

#### Scenario: 零模板与重复调用
- **WHEN** 项目无模板或重复初始化
- **THEN** 入口仍能编译，成功初始化后不重复构造模板，失败不伪标成功

### Requirement: 生成代码合法且确定
生成器 SHALL 使用语义完整类型名、转义字符串 literal 和 Ordinal 排序，对不能支持的声明给定位诊断。

#### Scenario: 特殊名称和类型
- **WHEN** 模板名称包含引号/反斜杠/换行，或类型在全局/嵌套命名范围
- **THEN** 生成代码合法且注册键与原字符串一致

#### Scenario: 外部系统顺序
- **WHEN** 消费者声明当前不支持的跨程序集系统注册
- **THEN** 明确报告输入不支持，不将系统追加到事件清理之后

### Requirement: 项目与脚手架启动
demo/test 及其复制出的新项目 SHALL 在使用模板之前调用本项目 registrar，并具有 Analyzer 引用。

#### Scenario: 重命名项目
- **WHEN** New 从 demo 创建不同名称项目
- **THEN** 无需按程序集名手改 registrar 调用即可编译和注册
