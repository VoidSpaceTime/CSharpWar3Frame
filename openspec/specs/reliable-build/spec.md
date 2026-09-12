# reliable-build Specification

## Purpose
定义 CLI、多开、外部进程、构建阶段、文件与资源转换、模型工具以及 SDK 回归的成功和失败契约，保护用户源码与已有产物。

## Requirements

### Requirement: 有界客户端启动
multi N SHALL 表示额外启动 N 个客户端，N 为正，既有进程不改变循环进度。

#### Scenario: 已有客户端或启动失败
- **WHEN** 已有游戏进程或某次启动失败
- **THEN** 启动请求数不超过 N，失败返回非零，不终止用户既有进程

### Requirement: 受控进程执行
执行器 SHALL 并发读取重定向输出，支持超时/取消并传播真实退出状态。

#### Scenario: 输出超过管道容量
- **WHEN** 工具同时产生大量 stdout/stderr
- **THEN** 不因管道互等挂起，日志保留有界结果，退出码正确

### Requirement: 构建失败停止与产物保护
构建 SHALL 检查准备、publish、产物、pack 各阶段；失败不执行后续阶段、不删除有效地图或清理游戏插件。

#### Scenario: 发布或打包失败
- **WHEN** 任一前置阶段失败或缺少必需产物
- **THEN** CLI 返回非零且原有效地图及其外部 JIT 模块保持不变

#### Scenario: 成功替换
- **WHEN** 临时 w3x 打包和验证成功
- **THEN** 替换最终地图并保持 callback 的模块目录路径有效

#### Scenario: 脚手架发布
- **WHEN** 从 demo 创建并发布一个项目
- **THEN** 输出 project.dll、Bootstrap.BridgeMain 与项目模板注册器，不复制模板构建缓存

### Requirement: 内容同步与标记事务
非缓存同步 SHALL 反映文件新增、修改、删除，不以目录时间代表内容；WE 标记仅在解包及回同步全部成功后删除。

#### Scenario: 文件内容改变
- **WHEN** table 中现有文件改变但目录时间不变
- **THEN** 下一次非缓存构建使用新内容

### Requirement: 资源转换保留契约
资源转换 SHALL 按方法签名保留默认/命名参数及显式表达式的源码求值顺序，派生代码进入构建目录且不改用户源清单。

#### Scenario: 省略参数与再次构建
- **WHEN** 使用 AddModel(path) 或 AddV3D(path, alias)，再重复构建
- **THEN** 生成结果可编译，默认 volume 保持 127，转换确定且不改源文件

#### Scenario: 命名参数求值
- **WHEN** volume、alias 等命名参数表达式带有可观察副作用，且源码顺序不同于声明顺序
- **THEN** 转换保留源码求值顺序，只追加省略的派生参数

### Requirement: 配置与模型工具正确性
工具 SHALL 在使用前验证必需配置，载入图写回目标 INI，ModelFormat 使用用户输入并可非交互结束。

#### Scenario: 无效或有效输入
- **WHEN** 配置/exe 不存在或传入合法模型目录
- **THEN** 无效配置明确失败；合法模型目录不被固定开发机路径覆盖

### Requirement: 内存容量独立记账
模型缓冲区 SHALL 独立记录分配容量，扩容成功后更新记账，释放前保存容量，Dispose 幂等。

#### Scenario: 分配扩容释放
- **WHEN** 分配 1024 字节后扩容并两次 Dispose
- **THEN** GC pressure 只按真实容量增减一次，不从已修改指针计算容量

### Requirement: SDK 回归可执行
修复 SHALL 提供可从 .NET SDK 执行的自动回归，覆盖既有纯 ECS 场景和本次真实失败边界。

#### Scenario: 回归失败
- **WHEN** 任一断言失败
- **THEN** 宿主返回非零，不能以捕获日志替代失败状态
