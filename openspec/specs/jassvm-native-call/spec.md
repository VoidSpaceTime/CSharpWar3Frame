# jassvm-native-call Specification

## Purpose
规定 JassVM 托管侧 cdecl 调用可接受的参数槽数量、允许封送的类型，以及既有 0 至 12 槽路径不得改变行为。

## Requirements

### Requirement: 13 至 16 个参数槽必须按序调用
cdecl 分发 SHALL 接受 13、14、15、16 个 32 位参数槽。每个槽 MUST 按调用方给出的顺序传入被调函数，返回的 32 位值 MUST 继续沿用现有返回值解包。

#### Scenario: 13 槽调用成功
- **WHEN** 以 13 个可封送参数调用一个 cdecl 函数
- **THEN** 被调函数收到全部 13 个槽且顺序与入参一致
- **AND** 调用不抛出参数个数不支持异常

#### Scenario: 16 槽仍在上限内
- **WHEN** 以 16 个可封送参数调用一个 cdecl 函数
- **THEN** 被调函数收到全部 16 个槽且顺序与入参一致

### Requirement: 超过 16 槽必须失败且不得截断
参数槽数量大于 16 时，分发 MUST 抛出 `NotSupportedException` 并在异常信息中给出实际个数与上限 16。分发 MUST NOT 丢弃多余参数后继续调用。

#### Scenario: 17 槽被拒绝
- **WHEN** 以 17 个可封送参数调用 cdecl 分发
- **THEN** 抛出 `NotSupportedException`
- **AND** 被调函数没有被进入

### Requirement: 0 至 12 槽路径行为不变
0 至 12 个参数槽的调用 MUST 保持现有函数指针签名、参数顺序、返回值解包和失败类型。扩展 13 至 16 槽 MUST NOT 改变这些路径的可观察结果。

#### Scenario: 既有槽数仍走原路径
- **WHEN** 以 0 至 12 个之间的任意槽数调用 cdecl 分发
- **THEN** 调用结果与扩展前相同
- **AND** 不因新增上限分支改变成功或失败类型

### Requirement: 数值与句柄槽的封送边界保持不变
可封送类型 SHALL 仍仅为 `int`、`nint`、`float`、`bool`、`string`、`Action`、`DzAction`。`double`、`long`、`ulong`、`uint`、`byte`、`short` 与 `enum` MUST 继续被拒绝并抛出 `InvalidCastException`。`float` 参数槽 MUST 继续传递该浮点值的地址，而不是把 float 位型当作整数直传。`nint` MUST 继续以 `Convert.ToInt32` 收入 32 位槽，超出 `int` 范围时 MUST 抛出 `OverflowException`。

#### Scenario: double 被拒绝
- **WHEN** 参数列表含有 `double`
- **THEN** 抛出 `InvalidCastException`
- **AND** 不把该值截成一个 32 位槽后继续调用

#### Scenario: float 参数仍是地址槽
- **WHEN** 参数列表含有 `float`
- **THEN** 对应槽是该 float 存储的地址
- **AND** 槽值不是该 float 的 IEEE 位型
