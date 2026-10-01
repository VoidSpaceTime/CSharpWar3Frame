# 提案：sound 域评估（sound-support-eval）

## 状态

- 状态：待用户裁决
- 等级：fast（纯评估，不改行为契约）
- 复盘强度：R0 Direct（检索核对 + `openspec validate`）
- 范围：只评估原生 sound 是否纳入框架。本 change 不包含实现、不写 tasks、不写 spec delta。

## Why

W5.3 要求回答：War3 声音要不要进入 ECS。绑定层已有 `JSound` 与 `CreateSound*` / `StartSound` / `StopSound`，但 `War3Frame/Src` 与 `Projects/` 没有任何调用。资源目录里登记了 wav 路径和时长，那是打包清单，不是播放器。

本提案只冻结现状、候选用法、句柄所有权和建议。用户裁决前不新增组件、系统、Helper 或原生调用。

## 现状证据

检索日：2026-10-01，分支 `ECS-Framework`。

### 绑定存在

| 事实 | 位置 | 含义 |
|---|---|---|
| 句柄类型 | `War3Frame/Library/Api/JassType.cs:313-326` `JSound : JAgent` | 只有三个构造函数，无行为 |
| 声音种类 | `JassType.cs:1303-1316` `JSoundType`；`Blizzard.cs:910-911` `SOUND_TYPE_EFFECT` / `SOUND_TYPE_EFFECT_LOOPED` | 给 `GetAbilitySound*` 用的枚举句柄，不是可播放实例 |
| 创建 | `JassApi.cs:10214` `CreateSound`；`:10226` `CreateSoundFilenameWithLabel`；`:10238` `CreateSoundFromLabel`；`:10250` `CreateMIDISound` | 返回新 `JSound`。函数指针在 `:1231-1237` |
| 播放 / 停止 | `:10352` `StartSound`；`:10357` `StopSound(killWhenDone, fadeOut)`；`:10362` `KillSoundWhenDone` | 计划里的 `PlaySound*` 在绑定中不存在；1.27 对应物是 `StartSound` |
| 3D / 附着 | `:10333` `SetSoundPosition`；`:10347` `AttachSoundToUnit`；`:10306` `SetSoundDistances`；`:10342` `SetSoundVelocity` | 坐标、挂单位、衰减、速度均有绑定 |
| 参数 | `:10283` `SetSoundVolume`（注释：0-127）；`:10288` `SetSoundPitch`；`:10274` `SetSoundChannel`；`:10453` `SetSoundDuration` | 音量、音高、声道、时长 |
| 查询 | `:10458` `GetSoundDuration`；`:10486` `GetSoundIsPlaying`；`:10495` `GetSoundIsLoading` | 可回读，无调用方 |
| 其它 | `:4000` `TriggerWaitForSound`；`:10504` `RegisterStackedSound`；`:7398` `SaveSoundHandle`；`:8091` `LoadSoundHandle`；`:10770` `GetAbilitySound` | 等待、堆叠、存档、技能音效路径。均无调用方 |
| 地图音乐 | `:10371-10450` `SetMapMusic` / `PlayMusic` / `PlayThematicMusic` 等 | 全局音乐，不返回 `JSound`，与 3D 句柄不是同一条生命周期 |

`KKApi.cs`、`YDApi.cs`、`DzApi.cs` 无 `CreateSound` / `StartSound` / `JSound`。声音能力全部在 1.27 `JassApi` 绑定内，不依赖 Reforged 扩展。

绑定注释里的 `[R]` / `[C]` / `[new]` 是 YDWE 动作分类（限制级 / 普通 / 新增），不是版本门。`CreateSound`、`StartSound`、`AttachSoundToUnit`、`SetSoundPosition` 都走 `War3.GetNativeFunction`，与单位、特效同一条 1.27 调用路径。

### 运行时未使用

`CreateSound`、`CreateSoundFilenameWithLabel`、`CreateSoundFromLabel`、`CreateMIDISound`、`StartSound`、`StopSound`、`KillSoundWhenDone`、`AttachSoundToUnit`、`SetSoundPosition`、`JSound`、`PlaySound`：

- `War3Frame/Src`：0 命中。
- `Projects/`：0 命中。
- 全仓 `*.cs`：方法体只在 `War3Frame/Library/Api/JassApi.cs`；类型只在 `JassType.cs` 与 `Blizzard.cs`。

没有 `SoundNative`、`SoundCreateNativeRequest`、`SoundNativeSystem`，也没有把声音登记进 `NativeEntityIndex` 的代码。

### 资源清单不是播放器

| 事实 | 位置 | 含义 |
|---|---|---|
| 目录 | `War3Frame/Library/Assets.cs:11-17`、`:23-31`、`:49-69` | `Sound` / `VcmSound` / `V3dSound` / `VwpSound` 只存 path、时长、音量 |
| 示例登记 | `Projects/demo/Assets/Sounds.cs:5-19`；`Projects/test/Assets/Sounds.cs:6-19` | `AddVCM` / `AddVWP` 写入清单；`AssetsInit` 只调用 `SoundsInit` |
| 构建消费 | `FrameBuild/CommandManager/AssetsBuild.cs:22-23`、`:159-166`；`AssetManifest.cs:82-92` | 把 wav 打进地图，并用 `war3sounds.yaml` 补时长 |
| 回归 | `Projects/Regression/BuildRegression.cs:178-206` | 只断言 volume / duration 被保留，不调用 `JassApi` |

`VcmSounds` / `V3dSounds` / `VwpSounds` 的读取方只有 `Assets.cs` 自己的 `Add*` 和上面这条构建回归。没有任何系统按别名取出路径再 `CreateSound`。

## 候选需求

以下是可能用法，不是本 change 的实施范围。当前 0 个调用方。

1. **一次性 3D 音效。** 业务写 `SoundPlayNativeRequest`（路径、是否循环、世界坐标、音量）。Native 系统 `CreateSound(..., is3D: true)` 后立刻 `HandleAdd`，`SetSoundPosition` + `SetSoundDistances`，再 `StartSound`。播完即忘的短音效用 `KillSoundWhenDone`，不在 ECS 里留最终播放位置。
2. **附着到单位。** 同一条请求改为携带目标实体。Native 系统经 `UnitNative` 取 `JUnit`，调用 `AttachSoundToUnit`。单位没有原生句柄时不得调用；附着关系若要跟着单位移动或在单位销毁时停播，必须留在 ECS（主体上的 sound 状态），不能把 `JSound` 当长期真相。
3. **持续声的播放 / 停止所有权。** 循环环境声、引导施法声需要「谁可以 Stop」。建议：播放中的句柄只放在 Native 系统持有的 `SoundNative` 组件上；业务发 `SoundStopNativeRequest`。`StopSound` 的 `killWhenDone` 与 `KillSoundWhenDone` 不是同一次 `HandleRemove`：停止播放不等于句柄已销毁。销毁点必须唯一，且 `HandleRemove` 与真正释放相邻。
4. **与现有特效对齐，而不是塞进特效。** `EffectNativeSystem`（`EffectNativeSystem.cs:174-194` 创建后 `HandleAdd`；`:135-145` `HandleRemove` 后 `DestroyEffect`）是句柄配对的正例。声音没有 `DestroyEffect` 等价物：`JassApi` 里没有 `KillSound` / `DestroySound`。不能把 sound 句柄挂进 `EffectNative` 复用那条销毁。
5. **显式不做。** 不把 `PlayMusic` / `SetMapMusic` 收成 3D 声音实体（全局单曲，无句柄）。不在施法或伤害系统里直接 `StartSound`。不把 `Assets` 清单升级成运行时播放器。`TriggerWaitForSound` 会按真实时间等待，与锁步无关，不作为框架等待原语。

若裁决为纳入，实施提案至少要先定：短音效走一次性 Request（播完 `KillSoundWhenDone`），循环声走主体上的 Command + `SoundNative`，停止与释放分成两个请求。本 change 不预写这些类型。

## 成本与风险

| 项 | 判断 |
|---|---|
| 1.27 API | 创建、3D 位置、附着、播放、停止、`KillSoundWhenDone`、`GetSoundIsPlaying` 都在现有 `JassApi` 绑定内。缺的是显式销毁函数，不是缺绑定 |
| 句柄配对 | `AGENTS.md`：创建后下一行 `HandleAdd`，释放前相邻 `HandleRemove`，同一类型只有一个释放点。`KillSoundWhenDone` 是「播完再释放」的延迟副作用，调用当下句柄仍活着；若立刻 `HandleRemove` 会提前丢掉引用，若从不 `HandleRemove` 则泄漏。循环声更不能靠它回收 |
| 播放状态谁拥有 | `GetSoundIsPlaying` 是原生回读，不是 ECS 真相。多系统各自 `StartSound` 会抢同一句柄。持续声必须有唯一 Native owner；一次性音效不应变成可查询的长期实体 |
| 性能 | 每次命中都 `CreateSound` 会在原生侧堆句柄。战斗打击音（`Sounds.cs` 已登记 metal slice）若全量走 ECS 实体，成本高于「播完即毁」。默认不应给每个弹道/伤害事件建持久 sound 实体 |
| 位置同步 | 附着后单位移动由 War3 带动，ECS 不必每帧 `SetSoundPosition`。未附着的 3D 声若要跟着弹道走，才需要 Dirty 或跟随请求；多数打击声是一次性坐标，不需要 Compare-Sync |
| 与特效生命周期 | 特效用 `Duration` + `EffectDestroyRequest` 回收。声音时长在资源清单里是打包元数据（`Assets.cs:23`），运行时时长以 `GetSoundDuration` 为准。两套时长不能混成同一个组件 |
| 爆炸半径（若另立实施） | 主要在 `War3Frame/`：请求组件、一个 `SoundNativeSystem`、句柄字段。生成器只因新 `[SystemRegister]` 多注册一个系统，不改生成器。`FrameBuild/` 的资源打包可保持不动。`Projects/` 只在批准实施后补场景 |
| 本 change | 无代码风险。唯一风险是把评估误读成已批准实施 |

## 建议

**建议：不纳入当前框架范围。** 绑定与资源清单保留，不新增 ECS 声音域。

理由：

- `War3Frame/Src` 与 `Projects/` 没有播放调用。已登记的 wav 只被构建链打包，没有玩法在等一个 Sound API。
- 最小闭环并不小：一次性播放、循环声的停止所有权、`KillSoundWhenDone` 与 `HandleRemove` 的时序，三者必须先定契约。现在没有消费方来验证这套契约。
- 地图与物编自带的单位语音、技能音效继续由 War3 播放即可。框架要的是「ECS 事件发生时在某个坐标响一声」时再立项，避免先做无人调用的 Native 系统。

若用户否决本建议、决定纳入，另立实施提案，第一阶段只做：一次性 3D 播放（`CreateSound` → `HandleAdd` → `SetSoundPosition` → `StartSound` → `KillSoundWhenDone`）加一个停止请求。不接循环环境声，不把声音并进 `EffectNativeSystem`，不改 `Assets` 打包。

## 待裁决项

**待用户裁决。** 本提案不代用户选择。

- 选项 A：不纳入。归档本评估，不另立实施提案。
- 选项 B：纳入。另立实施提案，范围限于上一节的第一阶段。
- 选项 C：维持现状并挂起。不归档、不实施，等出现具体播放调用再开。

未收到用户回复前，本 change 停在审核门。

## Capabilities

### New Capabilities

- 无。纯评估，不新增当前系统行为契约。

### Modified Capabilities

- 无。`.openspec.yaml` 设置 `skip_specs: true`。没有可归档的行为 delta。

## Impact

- `War3Frame/`：仅作证据。不改 `JassApi`、`Assets`、组件、系统或 Helper。
- `War3Frame.Generator/`：无影响。未新增 `[SystemRegister]` 类型。
- `FrameBuild/`：无影响。不改 `AssetsBuild` / `AssetManifest`。
- `CSharpWar3Frame/`：无影响。
- `Projects/`：无影响。不改 `Sounds.cs`，不新增回归场景。
- `openspec/specs/`：无变更。

## 验收标准

- `proposal.md` 覆盖背景、现状证据、候选需求、成本与风险、建议、待裁决项。
- 证据含文件与行号，并区分「绑定存在」「运行时未调用」「资源清单只被打包」。
- 建议明确为不纳入，同时标为「待用户裁决」，不把建议写成已批准。
- `openspec validate sound-support-eval` 通过。
- 不创建实现代码、`tasks.md` 实施清单或 spec delta。

> 裁决（2026-10-01，用户确认）：不纳入；变更归档关闭。
