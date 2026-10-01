# R3 复核与验证证据

日期：2026-09-12。代码基线 `79326e8`，修复分支 `luomo`，实现审查点 `461cb6e`。本轮按五个风险视角分别审查并给出 verdict；由当前代理使用源码、实际 SDK、受控进程和文件夹具完成，没有调用完整 review-work。以下结论仅覆盖 proposal 的已批准范围。

## 1. 目标与约束

**证据**：逐项对照原始审查 F01–F21 / S01–S05、最终差异和下方映射。`git branch --show-current` 为 `luomo`；原 `ECS-Framework` 仍为 `79326e8`。用户授权及中文提交要求已留在 proposal；新增提交 `2e08f1b`、`461cb6e` 均使用中文。`rg` 核对仍保留 ItemCreateNativeSystem、原生类型自动识别与复杂远程攻击 TODO；未给未实现能力返回伪造成功。

**Verdict：PASS**。确认的逻辑缺陷已修复并验证；Native 新能力和大型待审功能没有被当作简单封装实施。架构使用 ECS 数据与生命周期、OOP 模板/入口、Native 投影和独立构建执行器，按各自职责处理。

## 2. 技术质量

**证据**：时间回归验证输入 1 秒不再推进 1.14 秒，卡顿补跳受 Duration 存活窗口约束。AuraOwner、ModifyTarget → AttrOwner、Item owner/slot 和玩家联盟位的创建、查询与移除链路逐一核对；技能创建与升级共用 `ApplyLevelValues`。实际生成器输出再次编译，消费者、零模板、全局/嵌套类型、字符串及诊断均通过。空间索引 order 99 在移动后、搜索 110–112 前；事件清理仍为 132，外部 SystemRegister 给出 WFGEN004，不绕过清理边界追加系统。

**Verdict：PASS**。未用空 catch 隐藏原启动错误，没有在查询迭代中新增结构变更。同步身份仅承诺当前会话、当前 Store 和 Friflo revision 范围；跨会话持久化与 revision 回绕不在该身份协议保证内。

## 3. 安全与资源

**独立证据**：真实子进程分别写出超过管道容量的 stdout/stderr，输出尾部各限制 32 KiB；非零退出、启动失败、调用方取消和超时均有断言。失败注入覆盖准备、publish、缺失产物、pack；旧地图、外部 project.dll 与游戏插件保持原样。故意让地图目标为目录，触发文件系统拒绝后验证模块目录已回滚。路径规范化和重叠目录拒绝在写入前执行，清理范围为工具拥有的临时目录。模型流测试覆盖扩容后数据、容量、零容量、重复释放、释放后写入和短流。

**Native 证据**：编译原样 Effect/Player Native 系统，仅替换底层函数为调用记录器；验证创建后 HandleAdd、销毁前 HandleRemove、累计矩阵重建后缩放及 32 种联盟位组合。静止特效预热后 1,000 次更新：0 次原生调用、0 字节本线程托管分配。XY 单独变化仅写 XY，Z 单独变化仅写 Z。

**Verdict：PASS（本次范围）**。普通 I/O 失败有回滚证据。两个文件系统对象的发布不保证断电或强杀期间整体原子性；真实原生实现与完整不可信模型解析未做全面验证。现有原生调用的调整集中在 Native 系统，没有为了补全规则 TODO 新增原生 API。

## 4. QA

**实际环境**：Windows；隔离安装并校验下载摘要的 .NET SDK 10.0.401 / runtime 10.0.12；Friflo 3.6.0；Roslyn 5.9.0；官方 OpenSpec 1.13.0。未修改全局 PATH。

**命令及结果**：

| 命令/验证 | 结果 |
|---|---|
| `dotnet build Projects/Regression/Regression.csproj -c Release` | 成功；包括 War3Frame、Generator、FrameBuild、CLI 和 FastMDX |
| `dotnet Projects/Regression/bin/Release/net10.0/War3Frame.Regression.dll` | 43/43 PASS，退出 0 |
| `dotnet build Projects/demo/demo.csproj -c Release` | 成功；输出 win-x86 的 project.dll |
| 集成组的 demo → New → win-x86 JIT publish | 成功；实际复制当前源码并排除 bin/obj，检查 Bootstrap.BridgeMain、注册器与依赖文件 |
| 集成组的 test win-x86 JIT publish | 成功；检查实际 PE 元数据和依赖文件 |
| 集成组的 ModelFormat Release build/执行 | 成功；有效、缺参、根目录、损坏模型均按契约退出；原模型未修改 |
| `openspec validate repair-audit-findings-on-luomo --strict` | 通过 |
| 归档后 `openspec validate --specs --strict` | 当前 7 份 spec 全部通过；6 个新 capability 的 27 条 requirement 与归档 delta 一致 |
| `git diff --check` / staged diff 检查 | 通过 |

全量场景分布：existing 11、runtime 14、generator 5、native-projection 1、build 8、integration 4。最终增量 Release 构建为 0 error / 0 warning；前序重编译仍可见项目既有警告，本次没有清零全仓警告。

**失败处理记录**：首次全局系统回归的记录器误标 Add 重载，按实际 TimedSystemRoot 契约修正后 5/5 通过；地图发布失败夹具在 Windows 抛 UnauthorizedAccessException，扩大为明确的两种 I/O 失败类型后 8/8 通过；ModelFormat 的空贴图模型暴露空数组边界，修复后集成组及完整 43 项通过。最终无失败断言。

**Verdict：PASS**。不依赖 War3 的门禁全部通过。真实 War3/WE、w2l 地图打包、多人锁步、AOT/C++/CLI 宿主组合未执行；proposal 已明确客户端/AOT 为非阻塞验证，w2l 使用受控阶段模拟验证编排。实际原生工具与宿主兼容仍须集成环境验证。

## 5. 上下文与治理

**证据**：核对根 AGENTS、OpenSpec README、repository-governance 当前 spec、多项目入口和 BridgeEntry.cpp 的固定加载契约。demo 修正为 project.dll / Bootstrap.BridgeMain；test 与 demo 均在 ECS 初始化后显式调用项目 registrar。与基线相比，其余五个待审 change 的文件差异为空。技能等级缺陷的旧提案由本变更取代，已执行官方 `archive ... --yes --skip-specs`，归档日期 2026-09-12。

**Verdict：PASS**。行为规格归入六个 capability，避免将历史 delta 作为当前真相。旧提案仅关闭取代关系，实际行为只由本变更同步一次。Git 按设计、运行时、优化回归、构建工具分批提交；最终另提交官方归档结果。

## 缺陷到修复及证据的映射

| ID | 根因处理 | 可重复证据 |
|---|---|---|
| F01 | Item companion 目标补齐有效生命状态夹具 | existing/ItemCompanionAbilityValidationScenario |
| F02 | 未实现物品 Native 占位退出默认注册 | runtime/default-registration |
| F03 | 按程序集生成显式 registrar，接入 demo/test | generator 组、integration 两种 publish |
| F04 | 空间索引 order 99 注册 | runtime/spatial-search、默认链及 order 源码 |
| F05 | 分离调度余量与未交付经过时间 | runtime/clock-conservation |
| F06 | 统一 AuraOwner、目标关系及多来源贡献 | runtime/aura-source-and-range |
| F07 | multi N 为额外 N 次、有界启动并等待 | build/config-and-bounded-multi、实际 CLI |
| F08 | 按阶段检查结果，暂存成功后发布并回滚 | build/pipeline-failure-and-artifact-boundaries、artifact-promotion-rollback |
| F09 | 同时排空 stdout/stderr，支持超时取消 | build/process-output-exit-timeout |
| F10 | Once 首次命中立即消耗 | runtime/once-batch |
| F11 | Buff 续期撤销过期阶段标记 | runtime/buff-refresh |
| F12 | 根据实际存活时间补跳 | runtime/buff-catchup、buff-final-ticks |
| F13 | 同槽背包物品可重新装备且不重复计数 | runtime/equipment-roundtrip |
| F14 | 阵营直接由 ECS bits 派生并共用投影规则 | runtime/relation-and-filter-matrix、32 种 Native bits 记录 |
| F15 | 累计旋转重建矩阵并在之后重放缩放 | native-projection/production-source-call-order |
| F16 | e1:id:revision 身份与输入校验 | runtime/sync-identity-and-input |
| F17 | 语义绑定真实参数，派生代码作为 MSBuild 输入 | build/asset-parameters-and-sdk-input |
| F18 | 同步完整文件集，包含内容变化和删除 | build/file-set-and-we-markers |
| F19 | 独立容量字段和配对 GC pressure 记账 | build/buffer-capacity-and-model-roundtrip、DataStream 源码 |
| F20 | Roslyn 字符串 literal 保留准确注册键 | generator/consumer-types-literals-idempotence |
| F21 | 完整符号名覆盖全局、嵌套和同名类 | generator/consumer-types-literals-idempotence、framework-global-system-order |
| S01 | 配置目录与必需 exe 的有效性校验 | build/config-and-bounded-multi |
| S02 | 载入图校验完整输入后写原 w3i.ini | build/loading-validation-and-ini |
| S03 | ModelFormat 使用入参且非交互退出 | integration/model-format-input-and-exit |
| S04 | Buff 目标沿 ModifyTarget → AttrOwner 解析 | runtime/aura-source-and-range |
| S05 | Effect 销毁前相邻注销句柄 | native-projection/production-source-call-order |

补充修复证据：技能等级四项阶段值、跨 Store 属性/修饰器、同来源多修饰器安全删除、实体过滤分组，以及脚手架缓存隔离、空贴图/短流模型和命名参数求值顺序均在上述 43 项中覆盖。

## TODO 与优化边界

- 保留 Native TODO：ItemCreateNativeSystem 物品原生表现；UnitTargetTraits / TargetFilterRegistry 原生类型、魔免等自动识别。目前只消费明确写入的 ECS traits。
- 保留大型后续功能：远程投射物/闪电链攻击模式，以及 item-ground、同步批处理/context、Pause 合成、UI 控件层五个待审 change。
- 已落实优化：SDK 回归入口、统一进程和产物编排、生成器诊断与确定性、行为规格与归属、特效位置 Compare-Sync 及集合/委托复用、资源生成不污染源文件。
- 后续规模测量方向：Timer/Buff/Aura/Damage 等其他系统的集合复用、光环空间预选、资源内容版本缓存和实际游戏帧耗时。当前没有目标实体规模的测量证据，不把这些方向宣称为已经实现或已证明收益。
