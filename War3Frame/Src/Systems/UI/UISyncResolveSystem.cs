using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// UI 同步请求消费系统：收集 UISyncRequest → 分发到 Consume 注册的消费处理器 → 删除实体。
/// order 5：位于即时/请求系统（0–4）之后、玩法系统（≥10）之前；消费结果可在同 tick 被后续系统读取。
/// 先收集后处理：删除实体属结构变更，不能在 Query 迭代内执行（沿用 DamageResolveSystem 模式）。
/// </summary>
[SystemRegister(SystemKind.Immediate, 5)]
public class UISyncResolveSystem : QuerySystem<UISyncRequest>
{
    private readonly List<(UISyncRequest request, Entity entity)> _pending = new();

    /// <summary>分发并删除本 tick 的全部同步请求；未注册前缀的请求直接丢弃。</summary>
    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref UISyncRequest request, Entity entity) =>
        {
            _pending.Add((request, entity));
        });

        foreach (var (request, entity) in _pending)
        {
            // 处理器抛异常时也要删除请求，否则同一条同步数据会每 tick 重放、重抛。
            try
            {
                UISyncBridge.Dispatch(request.Prefix, request.Data);
            }
            finally
            {
                if (!entity.IsNull)
                    entity.DeleteEntity();
            }
        }
    }
}
