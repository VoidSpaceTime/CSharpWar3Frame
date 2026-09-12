## Why

2026-09-12 全局审查确认 21 项主要缺陷：测试及系统启动阻断、模板跨程序集注册失效、时间重复推进、光环/消息/物品/同步错误，以及构建失败传播、资源处理和模型工具问题。需在独立 luomo 分支修复根因并建立可重复验证，形成可回退的 Git 历史。

## Authorization

- 等级：architecture；复盘强度：R3，覆盖目标/约束、技术质量、安全、QA、上下文五类独立证据，不使用未获授权的完整 review-work。
- 状态：已批准范围，设计记录完成后进入实施。
- 基线：ECS-Framework@79326e8；工作分支：luomo，用户指定名称优先于默认前缀。
- 用户授权原文：“分叉一个luomo分支进行修复，并做好git管理”；追加：“按agents.md 要求,不考虑临时修复按 设计模式最优解决全部问题; 部分todo暂未实现,如果简单的封装你可以补全, 涉及原生native 的暂时留着todo; 其他代码逻辑性问题你直接修复提交”。
- 本记录承接已交付审查及上述明确直接修复、提交授权；不为既有 Native TODO 实现新增原生能力。新发现若超出逻辑修复和简单封装范围，返回提案审核。

## What Changes

- 修复审查 F01–F21 与 S01–S05，详见 tasks.md；采用统一时间记账、领域归属/阵营查询、显式模板注册、版本化实体身份、分阶段构建结果与受控进程执行。
- 补全依赖现有 ECS 状态的阵营、存活等简单过滤封装；缺少原生信息的识别能力保留明确 TODO，不通过调用 Native 或伪造返回值掩盖缺口。
- 修复已确认的技能等级变化未更新施法阶段参数逻辑，纳入本批并与既有待审提案建立取代记录；不实施 UI 重构、同步批处理、控制 Pause 合成、复活/对象池/远程攻击等新增能力。
- 建立可在 .NET SDK 中执行的回归宿主，纳入 11 个既有纯 ECS 场景、真实生成器跨程序集测试、构建失败注入与 Native 调用记录。
- **BREAKING**：同步实体 token 升级为包含版本、实体 ID 和代次的格式；旧 token 被拒绝，发送接收需同版本重启会话。
- 不重写用户业务资源清单以保存派生参数；生成资源代码进入构建生成目录，转换失败保留源文件及既有有效地图。
- 分批本地提交，每批包括相应验证；不自动 push、不合并原分支、不改写已有提交。

## Capabilities

### New Capabilities

- runtime-execution: 默认系统注册、时间守恒、Buff/事件/物品生命周期与技能等级重算。
- target-selection: ECS 阵营、光环归属、空间查询和目标过滤。
- native-state-projection: 已实现玩家/特效状态同步及句柄注销顺序；未实现 Native 能力不进入默认执行链。
- generated-registration: 同/跨程序集模板注册、类型与字面量生成及启动接入。
- entity-sync-identity: 版本化实体身份与安全解析。
- reliable-build: 构建阶段、进程、文件同步、资源转换、模型工具及可重复验证。

### Modified Capabilities

无。当前 openspec/specs 仅有 repository-governance；本变更不修改治理契约，新增以上已存在功能的行为规格。

## Impact

| 区域 | 影响 |
|---|---|
| War3Frame | 修复运行时逻辑、生成注册调用、玩家/特效现有投影、同步身份；不增加原生函数 |
| War3Frame.Generator | 语义符号生成、消费者 registrar、合法字面量、确定性输出与诊断 |
| FrameBuild | 构建步骤、受控进程执行、资源派生输出、地图/表同步及最终地图替换 |
| CSharpWar3Frame | 多开数量与退出码；失败信息与实际阶段对齐 |
| Projects | demo/test 注册接入、测试夹具、SDK 回归宿主及脚手架兼容验证 |
| FastMDX / ModelFormat | 显式容量记账、输入路径与非交互执行 |
| BridgeToJIT / Vendor | 不改宿主 ABI 或第三方工具；核对 JIT 产物与回调路径，避免移动路径造成回归 |

## Verification

阻塞门禁：官方 OpenSpec validate；修改的托管项目 Release SDK 构建；回归宿主全绿；test 的 win-x86 JIT publish；受控失败/并发输出/取消测试；每批 Git diff 检查及最终干净状态；R3 五视角 verdict；summary 和官方 archive。

本次范围以纯逻辑、现有 Native 调用参数/顺序修复及源码交付为验收对象，不承诺实施新增 Native TODO。真实 War3/WE 运行、AOT 与 C++/CLI 宿主游戏组合列为本次审核阶段明确声明的非阻塞集成验证：当前缺可自动执行的客户端验证协议，未执行时必须在 summary 保留原因和风险，不能称客户端已验证。SDK 构建与回归失败不享有该例外。
