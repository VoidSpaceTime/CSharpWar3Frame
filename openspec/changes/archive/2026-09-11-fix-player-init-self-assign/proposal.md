# 提案：修复玩家初始化自赋值导致的玩家系统失效

## 元信息

- **状态**：已实施
- **等级**：`light`
- **变更 ID**：`fix-player-init-self-assign`
- **日期**：2026-09-11
- **请求来源**：仓库疏漏扫描（ULW 只读审计）
- **默认实施后审查强度**：`R0 Direct`；本变更含单点逻辑修复，编译 + 运行时场景即可验证

## 1. 背景与目标

### 背景
`War3Frame/Src/Helpers/PlayerHelper.cs:35`：
```csharp
public static void InitializePlayers(ref PlayerNative[] players)
{
    _players = PlayerHelper._players;   // 自赋值（CS1717）
```
`_players` 为 `static PlayerNative[]`，此处置为自身 → 始终是 `Array.Empty()`。后果链：
- `PlayerHelper.Players` 恒为空。
- `NativePlayerEventBridge.Initialize()`（`initialization/War3Init/NativePlayerEventBridge.cs:24`）遍历空集合 → **攻击等原生玩家事件桥全部未注册**。
- 联盟矩阵双重循环空转 → `Relations[,]` 保持默认值（0 = `Allie`）→ 所有玩家互为盟友。
- `PlayerHelper.GetPlayer(index)` 越界风险。

### 目标
让 `_players` 正确指向 `CreatePlayers` 产出的玩家数组，恢复玩家镜像、联盟矩阵与原生事件桥。

## 2. 影响范围

- `War3Frame/Src/Helpers/PlayerHelper.cs`（1 行）
- 间接：`initialization/War3Init/NativePlayerEventBridge.cs`（初始化后生效）、玩家联盟查询
- 不受影响：`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`

## 3. 方案摘要

```csharp
_players = players;   // 修正：引用调用方传入的玩家数组
```
可顺带将方法签名由 `ref PlayerNative[] players` 收敛为 `PlayerNative[] players`（值传递已足够，因数组本身是引用类型）——如改动则同步更新唯一调用点 `War3Init.cs:15`。

## 4. 风险与回滚

- **风险**：低。修正后 `_players` 非空，会首次真正执行联盟初始化与事件桥注册（此前被静默跳过）。
- **回滚**：还原该行。

## 5. 验收标准

1. `InitializePlayers` 后 `PlayerHelper.Players.Length == 16`。
2. `NativePlayerEventBridge.Initialize()` 为每个有效玩家注册攻击事件桥。
3. 联盟矩阵初始化后对角线为 `Allie`、非对角线为 `Enemy`（默认敌对）。
4. `War3Frame` 编译 0 error，且 `PlayerHelper.cs(35,9)` CS1717 警告消失。
5. 新增本地场景断言上述 1/3（无需 War3 客户端）。

## 6. 非目标

- 不实现 `UnitHelper.IsEnemy` 阵营判断（TODO，另行处理）。
- 不改动 `PlayerHelper` 的 Dirty 同步机制（`native-sync-policy` 已定义）。
