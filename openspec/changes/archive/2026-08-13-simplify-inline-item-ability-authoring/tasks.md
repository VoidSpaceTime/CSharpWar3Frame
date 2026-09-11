## Phase 1: 删除 canonicalizer

- [x] 删除 `InlineAbilitySpecCanonicalizer` 整个类（schema 反射、深拷贝、指纹、UTF-8/上限/循环引用校验）。
- [x] `InlineItemAbilityTemplate` 去掉 `Fingerprint` 字段，构造函数只接收 `(owner, spec)`。
- [x] `Configure` 直接 `AbilitySpecBuilder.Apply(entity, level, _spec)`，不再 `CloneSnapshot`。

## Phase 2: 简化注册入口

- [x] `RegisterInlineItemAbility` 改为按名 first-wins：命中已存在 `InlineItemAbilityTemplate` 直接返回 templateName。
- [x] 保留：保留前缀校验、`existing is not InlineItemAbilityTemplate` 冲突失败、`spec.templateName` 一致性检查。
- [x] 删除 `_inlineTemplateCount`、`MaxInlineItemAbilityRegistrations` 及相关上限分支。
- [x] `NormalizeInlineOwner` 去掉 `InlineAbilitySpecCanonicalizer.ValidateString`，保留 null/whitespace 与长度检查。

## Phase 3: 去除单线程下无效的 `lock`

会话中追加范围。运行时为 WC3 单线程 + JassVM 回调，框架内无 `Thread`/`Task`/`ThreadPool`，
Friflo 未启用并行 job，注册表锁永不产生竞争。

- [x] 删除 `AbilityTemplate.RegistryLock` 字段。
- [x] 去除 8 处 `lock`：`Initialize`、`Register`、`RegisterInlineItemAbility`、`Get`、`TryGet`、`Apply`、`HasTemplate`、`GetAllTemplateNames`。
- [x] `Apply` 简化为单次 `TryGetValue(out var template)`，不再为锁作用域预声明局部变量。
- [x] 删除 `Initialize` 中关于 Monitor 重入的注释（无锁后不适用）。
- [x] 保留 `GetAllTemplateNames` 的 `.ToArray()` 快照：防的是调用方遍历期间注册新模板，与并发无关。
- [x] 保留 `_initialized` 标志：防重复初始化，与线程无关。
- [ ] 未处理：`FourCc.cs` 的 `ConcurrentDictionary` 同属单线程下的多余开销，本次未纳入范围。

## Phase 4: 验证

- [x] 构建 `War3Frame/War3Frame.csproj`：0 错误（174 warning 均为 `KKApi.cs`/`SyncHelper.cs` 既有 nullable 告警）。
- [x] 构建 `Projects/test/test.csproj`：0 错误。
- [x] 静态扫描确认无残留 `InlineAbilitySpecCanonicalizer` / `Fingerprint` / `CloneSnapshot` / `RegistryLock` 引用。
- [ ] 跳过：运行纯 ECS Item companion 场景。用户决定以编译通过作为打包门槛，不执行运行时场景。
- [ ] 未执行：R1 技术准确性复核（共享只读 spec 的 aliasing 副作用）。依据为三个运行时消费点均只读、
      EffectSpec swap 只重指组件指针不改集合，已记录于 design.md，未经 Oracle 复核。

## 备注

- 整解决方案无法用 dotnet CLI 构建：`BridgeToJIT.vcxproj` 为 C++ 项目，需 MSBuild。与本变更无关。
- 待确认（超出本变更范围）：Release 路径 `dotnet publish -r win-x86 -p:PublishAot=true` 是否能产出
  native dll。NativeAOT 是否支持 `win-x86` RID 未经本次验证；Debug 走 `BridgeToJIT` 的 JIT 路径不受影响。
