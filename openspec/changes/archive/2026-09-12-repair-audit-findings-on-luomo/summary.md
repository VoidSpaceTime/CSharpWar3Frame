# luomo 全局审查修复总结

日期：2026-09-12。等级 architecture，复盘 R3。状态：已实施并完成官方归档。基线 `ECS-Framework@79326e8`，分支 `luomo`。

## 实施结果

本轮修复原审查的 21 项主要缺陷及 5 项次级问题，并覆盖复核中确认的同类边界；逐项证据见 review.md。主要结果：默认系统树可运行，项目模板能跨程序集注册，时间不重复推进；Buff、Once、光环、装备和技能等级重算恢复一致生命周期。玩家关系由 ECS 联盟位派生，特效累计矩阵与句柄顺序正确，同步 token 增加版本及代次。

构建采用受控进程执行器和独立工作目录：每个阶段传播结果，失败保留已有地图、外部 JIT 模块和游戏插件；文件同步覆盖修改及删除，WE 标记在回同步全部成功后消费。资源转换按真实签名保留默认值、命名参数与求值顺序，编译派生文件而不修改源清单。模型工具修复容量记账、用户路径和非交互退出；demo/New 与固定 JIT 加载入口一致。

特效位置按 Compare-Sync 更新，待处理集合和查询委托按实例复用。原样系统源码的受控测量显示，预热后静止特效的 1,000 次更新为 0 次原生调用、0 字节本线程托管分配。该结果仅描述测量场景，真实客户端性能仍需集成测量。

## 阶段与多项目影响

| 阶段/区域 | 最终状态 |
|---|---|
| War3Frame | 生命周期、归属、目标过滤、同步身份和已有 Native 投影完成；新 Native 能力保留 TODO |
| War3Frame.Generator | 框架/消费者双路径、确定性符号生成与定位诊断完成 |
| FrameBuild | 进程、阶段、文件集、资源输入、发布回滚完成 |
| CSharpWar3Frame | 有界多开、真实失败退出码完成 |
| Projects | 11 个原场景及新增回归可 SDK 执行；demo/test/新项目接入注册器和 JIT 入口 |
| FastMDX / ModelFormat | 容量、短流、空贴图与命令行输入边界修复 |
| BridgeToJIT / Vendor | 未修改；仅核对加载接口，实际宿主和原生工具验证未执行 |

## 验证与复盘

- .NET SDK 10.0.401；相关托管项目 Release 构建成功。
- 全量回归 **43/43 PASS**：existing 11、runtime 14、generator 5、native-projection 1、build 8、integration 4。
- 实际 New 脚手架和 test 的 win-x86 JIT publish 通过，PE 元数据与模块依赖符合 Bootstrap.BridgeMain / project.dll 契约。
- 原样 Native 源码配调用记录器验证矩阵、缩放、句柄配对、32 种联盟位及位置差异同步。
- 官方 OpenSpec strict validate、Git diff 检查通过。R3 的目标约束、技术、安全资源、QA、上下文五个视角分别有证据及 PASS verdict，见 review.md。
- 复核发现的地图发布回滚异常类型、生成器测试记录器和模型空贴图问题均已修正并重验，最终没有失败场景。

回归入口及操作说明位于 `Projects/Regression/README.md`。本地完整日志留在本任务的 `repair-evidence` 目录，仓库内保留可重复的源码、命令、测试结果及问题映射。

## 迁移、回滚与剩余风险

同步身份格式为 `e1:<base36 id>:<base36 revision>`，旧 token 被拒绝。发送接收双方需同版本重启会话；不把 token 持久化到其他 Store/会话，Friflo short revision 回绕后的永久唯一性不作保证。光环按新会话的 AuraOwner 关系重新创建。技能等级重算旧提案已标记“已取代”并通过官方 skip-specs 归档，行为只在本 change 合并。

真实 War3/WE、w2l 地图打包、多人锁步及 AOT/C++/CLI 宿主组合尚未执行：当前没有可自动执行的客户端验证协议，本次以 SDK、纯 ECS、调用记录和受控外部进程验证逻辑与编排。proposal 已在审核记录中明确客户端/AOT 非阻塞；剩余风险是实际原生实现、视觉、宿主加载和平台工具兼容，不能据此声称客户端已通过。

发布普通 I/O 失败会恢复旧模块目录；两个文件系统对象的切换不构成断电/强杀恢复事务，异常中断留下的 previous/staging 目录需保留用于恢复。代码回滚按提交依赖逆序 revert，协议发送接收同步回滚并重启会话，不 reset/rebase 原分支。

保留物品 Native 表现、类型自动识别等 TODO，以及远程攻击模式和五个待审功能提案。其他热路径池化、光环空间预选及内容版本缓存仍属需测量的优化方向，未以缺少实测的重构扩张本次范围。

## Git 管理

| 提交 | 内容 |
|---|---|
| b50373d | OpenSpec 正式提案、设计、delta 与验证计划 |
| bb05837 | 运行时、领域、同步身份、模板注册及首批回归 |
| 2e08f1b | 优化特效差异同步并补齐生成器边界回归 |
| 461cb6e | 修复构建失败传播、资源转换与模型工具 |

前两笔早于用户的中文要求；后续提交标题和说明均使用中文。原 ECS-Framework 保持 `79326e8`，修复在 luomo 分批提交，没有 push 或合并。官方归档与本总结作为最后的治理提交，最终提交哈希由 `git log -1` 查询。

## 官方归档记录

`openspec archive repair-audit-findings-on-luomo --yes` 返回 0；31 项实施任务全部完成。官方合并 6 个 capability、27 条 ADDED requirements，目录迁至 `openspec/changes/archive/2026-09-12-repair-audit-findings-on-luomo/`。自动创建的六份当前 spec 的 Purpose 已按实际职责补齐，并把玩家“关系缓存”文案对齐为实际的 ECS 派生查询；Requirements 与归档 delta 一致。

归档工具提示超过 10 个 delta 可考虑拆分，为非阻塞建议。本提案按全局审查确定 architecture / R3，代码已按依赖分批提交，六个 capability 分别保存行为契约。被取代的技能等级提案也已独立官方归档；剩余五个待审 change 保持原内容。归档后当前 7 份 spec 的 strict validate 全部通过，6 个新 capability 的 27 条 requirement 与归档 delta 一致。最终提交及工作区状态以交付时的 Git 核对结果为准。
