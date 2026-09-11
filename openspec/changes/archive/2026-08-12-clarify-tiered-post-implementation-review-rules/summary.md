# 实施总结：分级实施后复盘治理

## 实际改动

- 将治理模型明确为三个独立层次：OpenSpec 提案等级、实施后复盘强度、具体工具启用。
- 将默认映射统一为 `fast -> R0`、`light -> R0/R1`、`full -> R2`、`architecture -> R3`。
- 明确安全敏感事项和重大实现独立触发 `R3 Comprehensive`。
- 明确公共 API、生成器/构建/发布契约、持久化、性能和跨系统协作等普通风险至少进入 `R2`，但不因分类名称自动进入 `R3`。
- 将 Oracle 定义为 `light/R1` 技术准确性复核的首选工具，并提供有记录的等价回退。
- 将完整五路复盘与完整 `review-work` 分离；只有用户明确要求全面复盘、完整 QA、指定 `review-work`，或更高优先级指令要求时才启用完整 `review-work`。
- 同步更新 `AGENTS.md`、`openspec/README.md`、`proposal-levels.md`、`review-checklist.md` 和本 change 的正式工件。

## 阶段结果与迁移状态

- Phase 1：正式 proposal、design、tasks 和 capability spec 已修订并重新获得用户批准。
- Phase 2：四个生效入口文档已同步完成。
- Phase 3：直接验证、代表场景检查和 `R3` 五视角复盘已完成。
- 新规则已在当前工作区生效；历史已完成 change 不追溯复盘。
- 未修改 `War3Frame/`、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/` 或 `Projects/` 的运行时与构建行为。

## 直接验证

- 映射与门禁断言：`AGENTS.md`、README、模板和 capability spec 的 `R0/R1/R2/R3`、Oracle 回退、普通 `R2` 风险与 `review-work` 显式门禁全部通过。
- 场景矩阵：fast 文档、light 版本敏感、full 局部生成器契约、architecture、安全边界、重大实现、未授权 `R3`、明确完整 QA、一次性文件锁、未批准重大范围共 10 个场景通过。
- 安全断言：安全定义、普通 native 非自动安全敏感、安全强制 `R3`、范围扩大返回 review、不可跳过直接验证共 5 项通过。
- 上下文断言：生命周期、实施前批准、spec 优先级、五项目影响、无运行时行为变化、更高优先级指令和显式 commit 门禁共 8 项通过。
- 文档质量：9 个治理文件均为有效 UTF-8，尾随空白为 0，Markdown 标题/代码围栏结构问题为 0。
- `git diff --check` 通过；仅输出仓库既有的 LF/CRLF 转换提示。

## R3 五视角复盘

- 目标与约束：通过。正式 spec 与用户确认的四级映射、两类 `R3` 风险和工具授权边界一致。
- 技术质量：通过。术语和有序强度可执行；发现 `AGENTS.md` 对高优先级来源的措辞少了 `system / developer`，已统一并重验通过。
- 安全：通过。安全敏感无法被 `fast/light` 绕过；新安全范围必须返回 OpenSpec review 门禁；普通原生调用不会被机械判高。
- QA：通过。10 个代表场景覆盖默认矩阵、风险升级、失败处理和工具授权，结果全部符合预期。
- 上下文：通过。既有生命周期、工件矩阵、spec 优先级、全局影响分析和仅用户要求才 commit 的规则保持不变。

本次未启用完整 `review-work`：提案本身要求 `R3` 五视角，但用户没有明确要求全面复盘、完整 QA 或 `review-work`，因此按新工具门禁使用当前获准的直接检查覆盖五类视角。

## 剩余风险与环境事项

- 当前全局 `openspec` 命令入口损坏：shim 指向缺失的 `@fission-ai/openspec` 模块，因此未能执行 OpenSpec CLI 校验。未擅自安装外部依赖；本次改用结构、内容和场景断言完成等价文档验证。
- `.git/info/exclude` 忽略 `openspec/README.md`、`openspec/templates/` 和 `openspec/changes/`，因此这些同步内容目前只存在于本地工作区；`AGENTS.md` 是唯一 Git 可见的本次治理改动。
- 工作区已有 Item/Casting/Effect 等运行时改动属于其他进行中的工作，本 change 未触碰或回退这些文件。

## 未完成事项

- 治理内容无阻塞项。
- OpenSpec CLI 安装修复不在本提案范围内，后续如需处理应单独提案。
- 未执行 commit；只有用户明确要求时才允许提交。
