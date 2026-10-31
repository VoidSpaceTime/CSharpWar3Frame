# 实施总结：fix-runtime-correctness

- **等级 / 复盘强度**：`full` / `R2 Targeted`
- **批准**：2026-10-23，范围 A–F 全部；D-1 新施法请求打断旧施法；D-2 效果已提交后的打断进入冷却；D-3 Buff 实例按 (buffId, 目标属性) 区分。

## 1. 改动范围

32 个缺陷全部修复，每项对应一条先红后绿的回归。

| 组 | 修复点 | 主要文件 |
|---|---|---|
| A 施法/移动 | A1 顶替方向；A2 `MoveContinuation.commandToken` 对齐；A3 `CastMovement.Clear` 统一清理，`MoveToCastSystem` 不再等待移动结果即可取消，并启用原先不可达的目标跟随；A4 新请求打断当前施法；A5 死亡施法者/移动者；A6 已提交效果打断进冷却；A7 范围内施法坐标；A8 施法中移除技能 | `CastingSystem.cs`、`MoveSystem.cs`、`MoveCommand.cs`、`UnitHelper.cs`、`AbilitySlotHelper.cs`、`AbilitySlotSystem.cs` |
| B tick 中断异常 | B1 `RemoveByKey` 循环外销毁；B2 属性读取空实体安全；B3 同步请求 `finally` 删除；B4 已删附着目标守卫 | `AbilityEffectSystems.cs`、`AttributeHelper.cs`、`UISyncResolveSystem.cs`、`EffectNativeSystem.cs` |
| C 死亡/生命 | C1 无 Health 不判死；C2 死者不回复不受治疗；C3 复活/出池 ECS 生命回满；C4 光环排除死者；C5 闪电链只跳存活敌人；C6 current 向下夹紧 | `AbilityEffectSystems.cs`、`HealthSystem.cs`、`ManaSystem.cs`、`TimerTaskSystem.cs`、`UnitHelper.cs`、`AuraSystem.cs`、`AttackSimulationSystem.cs`、`AttrCalculationSystem.cs` |
| D Buff/贡献 | D1 永久 Buff 叠层；D2 (buffId, 属性) 实例身份，`RemoveBuff` 删全部同名；D3 技能移除直接回收贡献 | `BuffHelper.cs`、`AbilitySlotHelper.cs` |
| E 效果/弹道 | E1 曲线弹道固定起点；E2 线形父效果不结算；E3 子效果目标坐标；E4 效果管线 10 处 `Game.Store` → 实体 Store；E5 地面周期余数、引导末跳 | `AbilityEffectSystems.cs`、`AbilityEffectHelper.cs`、`AbilityEffect.cs`、`CastingSystem.cs` |
| F 维护 | F1 `GroupHelper` 去静态缓冲；F2 删调试输出；F3 无根时丢弃输入；F4 同批拾取去重；F5 integration 超时 10 分钟 | `GroupHelper.cs`、`War3NativeBootstrap.cs`、`UIInput.cs`、`ItemPickupWorkflowSystem.cs`、`IntegrationRegression.cs` |

公共 API 只做加法：`MoveContinuation.commandToken`、`ProjectileRuntimeState.startX/startY/startZ/startInitialized`。`MoveToCastSystem` 的基类由 `QuerySystem<CastState, MoveOutcome, MoveContinuation>` 改为 `QuerySystem<CastState>`，这是可见的类型签名变化，仓库内没有代码依赖它。没有新增、删除或调整任何系统的 order，生成器输出不变。

## 2. 验证

| 项 | 命令 | 结果 |
|---|---|---|
| 先红 | 未修复代码上运行新回归 | `correctness` 30/30 失败、`native-projection/deleted-attach-target` 失败，失败原因与各缺陷一致 |
| 框架构建 | `dotnet build War3Frame/War3Frame.csproj -c Debug -t:Rebuild` | 0 error；警告 CS8625×346、CS8601×12、RS2008×8、CS8618×4，与改动前完全相同 |
| 解决方案构建 | `dotnet build CSharpWar3Frame.slnx -c Debug` | 9 个项目成功；唯一错误仍是 `BridgeToJIT.vcxproj` MSB4278（dotnet CLI 无 C++ 工具链，改动前已存在） |
| 全量回归 | `dotnet run -c Debug --no-build`（`Projects/Regression`） | **Executed 111; passed 111; failed 0**（原 78 项 + 新增 33 项：`correctness` 32、`native-projection` 1），26 秒 |
| 规格 | `openspec validate fix-runtime-correctness --strict` | valid |

实施中出现过一次失败：首轮全量回归 `existing/AttributeLifecycleValidationScenario` 第 1 阶段失败，原因是 C6 的夹紧让“现值非 0 应保留”的属性在同一轮被回收。改为用夹紧前的现值判定回收后通过。

### R2 视角

| 视角 | 证据 | verdict |
|---|---|---|
| 1 技术准确性（独立子代理复核全部 diff，11 项检查 + 临时探针） | Friflo 结构变更、顶替应用顺序、token 对齐、施法替换各阶段表、D-2 冷却与 `CastValidationScenario` 契约、引导 tick 14 组 float32 组合、Buff 对光环/地面/控制/DoT 的影响、夹紧与回收、DetachRuntime、GroupHelper、空实体 | 有条件通过。发现 1 个新引入缺口（外部替换施法移动命令后施法永久停在 `MovingToCast`）与 1 个既有同类问题（移动任务替换施法后续）。两者均已修复，新增回归 `A3-replaced-cast-move-cancels-cast`，并逐一关掉守卫确认用例失败。同时修正 `CancelCastMovement` 未清除打断标记、`BuffHelper` 注释错位。修复后通过 |
| 2 QA / 回归覆盖 | 32 个缺陷编号逐一对应 `correctness` 组或 `native-projection` 组用例；`existing` 组 14 个场景（含 `CastValidationScenario`、`ItemCompanionAbilityValidationScenario`、`SkillPointUpgradeScenario`、`AttributeLifecycleValidationScenario`）全部通过 | 通过 |

## 3. 数值与行为变化（需要知晓）

- **E5**：引导时长为 tick 间隔整数倍的技能多出最后一跳（例：2 秒/0.5 秒由 3 跳变为 4 跳）。地面周期伤害不再因丢余数而拉长周期。
- **A6**：引导或后摇被打断后进入冷却；前摇未提交时仍回 Ready、不进冷却。
- **A4**：施法中对另一个就绪技能发出有效请求会打断当前施法。物品使用（`ItemUseSystem`）仍在施法中拒绝，两者不一致，已列入延后。
- **A3**：走位施法会跟随移动超过 100 的目标单位重新下发移动（原代码有此分支但不可达）。阈值内的偏移不重定位，施法开始前也不复查距离，施法距离很小的技能仍可能在略超距离处开始吟唱。走位施法的移动命令或后续被其他命令替换时，施法取消。
- **C2**：池内单位（`isAlive=false`）不再回血回蓝；出池时 C3 回满生命。
- **D2**：`RemoveBuff(unit, id)` 删除该单位上全部同名实例。

## 4. 未执行的验证与剩余风险

- **真实 War3 客户端**：未执行（本环境无客户端）。按提案声明为非阻塞：本变更未新增或改写原生调用。A1 修复后被顶替的新移动命令会真正下发原生移动，这是修复目标；原生层表现仍需在客户端确认。
- **原生侧既有问题**（Root/NoPath `SetUnitPathing` 极性、打断不发原生 Stop、复活原生有效性、UI 鼠标坐标整数除法与多面板输入根等）未在本变更修复，见 design.md §6。
- **同类效果步骤互相覆盖**（`poison_bite` 丢减速）仍存在，需要单独提案。
