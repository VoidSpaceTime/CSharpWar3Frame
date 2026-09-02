# 总结：奥术飞弹示例增加命中眩晕效果

- 实际改动：`Projects/test/Scripts/Template/Ability.cs` 的 `ArcaneMissileTemplate` 在 `.Damage(...)` 后追加 `.Buff("arcane_missile_stun", AbilityValue.Constant(3f), AttributeHelper.Stun, ModifyType.Flat, AbilityValue.Constant(1f), BuffRefreshBehavior.RefreshDuration)`，并把描述改为"发射追踪飞弹，命中目标后造成魔法伤害并眩晕 3 秒"。
- 运行链路（复用既有能力，无新代码）：Damage/Buff 同链 → 弹道 `CanSettle` 等待 `ProjectileArrived` 后对命中单位依次结算 → Buff 步骤 → `BuffApplyRequest` → `BuffHelper.AddTimedBuff`（Stun Flat+1，3 秒）→ `ControlStateTransitionSystem` 检测 Stun 0→正跳变，发 `ControlStateChangedEvent` + 原生暂停请求；到期回到 0 解除。
- 验证：`dotnet build Projects/test/test.csproj` 通过（0 错误；178 个为仓库存量 nullable warning）。
- 风险：无运行时/契约改动，仅示例模板 + 描述；回滚为删除该 `.Buff` 链即可。
