## 1. 契约与属性

- [x] 1.1 在 `AttributeHelper` 注册 `DamageShield`、`Lifesteal`、`DamageReflect`，并核对三个 ID 可被 `GetFinalValue` / `GetCurrent` / `ModifyCurrent` 使用
- [x] 1.2 为 `DamageRequest` 增加默认 `false` 的 `skipPostProcess`，为 `DamageEvent` 增加 `absorbed`；核对现有 `DamageRequest` 构造与 `DamagePipelineValidationScenario` 仍可编译

## 2. 注册表与结算时序

- [x] 2.1 把 `DamagePostProcessRegistry` 拆成 Apply 前吸收列表与 Event 后列表；静态注册护盾、吸血、反射三个内置处理器，外部 `Register` 只进入后列表
- [x] 2.2 调整 `DamageResolveSystem`：减免后、扣血前只跑护盾吸收；用吸收后的 `finalDamage` 扣血并写入 `DamageEvent`；Event 后、死亡判定前跑吸血与反射。无敌路径不调用两段处理器
- [x] 2.3 核对主流程没有吸血、反射或护盾的业务分支，且三个处理器都不调用 `JassApi` / `KKApi` / `YDApi` / `DzApi`

## 3. 具名回归

- [x] 3.1 在回归宿主 `Projects/Regression/DamagePostProcessRegression.cs`（按 task-20 host 约定，新文件 + `Program.cs` 注册）增加并跑通以下具名断言：护盾部分吸收（100/40 → 扣 60、吸收 40）、护盾完全吸收不扣血不死亡、吸血 60×0.25=15、反射 60×0.5=30 且为 Real 并跳过后处理、无敌不消耗护盾且不产生治疗/反射、实际扣血为 0 不吸血不反射、无新属性时与现有基线一致
- [x] 3.2 断言反射结算可被对方护盾吸收，且该次结算不再产生第二跳吸血或反射，排除双重结算

## 4. 验证与收口

- [x] 4.1 `dotnet build War3Frame/War3Frame.csproj` 与 `dotnet build Projects/test/test.csproj` 均为 0 错误
- [x] 4.2 `dotnet run --project Projects/Regression -- existing` 中 `DamagePipelineValidationScenario` 通过，且该组无新增失败
- [x] 4.3 写完整 `summary.md` 后执行 `openspec validate damage-postprocess-builtins` 与 `openspec archive damage-postprocess-builtins --yes`；归档前不把真实 War3 客户端验证当作阻塞项，在 summary 记录未执行原因与剩余风险
