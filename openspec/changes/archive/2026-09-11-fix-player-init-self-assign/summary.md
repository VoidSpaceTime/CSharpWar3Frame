# 总结：修复玩家初始化自赋值

对应提案：`proposal.md`（`fix-player-init-self-assign`，`light`）
状态：`已实施`
日期：2026-09-11

## 改动

`War3Frame/Src/Helpers/PlayerHelper.cs`：`InitializePlayers` 中
`_players = PlayerHelper._players;`（自赋值，CS1717）→ `_players = players;`。

## 影响

修正后 `_players` 指向 `CreatePlayers` 产出的数组，恢复：玩家镜像（`PlayerHelper.Players` 非空）、默认敌对联盟矩阵、`NativePlayerEventBridge.Initialize()` 的原生事件桥注册。

## 验证

- 编译 0 error，`PlayerHelper.cs` 的 CS1717 警告消失。
- 新增 `QueryStructuralSafetyScenario.CheckPlayerInit`：断言 `Players.Length == 16`、`GetRelation(p0,p1) == Enemy`、`GetRelation(p0,p0) == Allie`；宿主 runner 执行 `PASS`。

## 后续

`UnitHelper.IsEnemy` 的阵营判断仍为 TODO（恒 true），不在本变更范围。
