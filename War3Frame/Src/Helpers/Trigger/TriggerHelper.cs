using System;
using Friflo.Engine.ECS;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 触发器规则注册入口。
/// 创建独立触发器实体（TriggerSpec 配置 + TriggerRuntime 状态），由 TriggerSystem 统一匹配。
/// 注销 = 删除返回的实体。
/// </summary>
public static class TriggerHelper
{
    /// <summary>按 Builder 配置创建触发器规则实体。</summary>
    public static Entity Register(EntityStore store, Func<TriggerSpecBuilder, TriggerSpecBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(new TriggerSpecBuilder())
                      ?? throw new InvalidOperationException("触发器配置必须返回 TriggerSpecBuilder");
        return Register(store, builder.Build());
    }

    /// <summary>按已构建的 TriggerSpec 创建触发器规则实体。</summary>
    public static Entity Register(EntityStore store, TriggerSpec spec)
    {
        return store.CreateEntity(spec, new TriggerRuntime());
    }
}