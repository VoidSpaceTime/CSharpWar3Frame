using System;
using War3Frame.Library.Api;

namespace War3Frame.Helpers;

/// <summary>
/// War3 同步随机数薄封装（默认转发 JassApi，不做任何状态管理）。
/// 登记为 AGENTS Native 分层例外：无句柄、无表现副作用、纯读引擎同步随机状态的一次性便利调用，
/// 不承载长期语义，供战斗结算（暴击 roll 等）在锁步环境下取同步随机值。
/// </summary>
public static class War3Random
{
    /// <summary>
    /// [0,1) 随机源提供器。默认走 War3 引擎同步 RNG（JassApi.GetRandomReal，全端同种子）。
    /// 仅供本地无客户端验证场景临时替换为确定性 fake；生产/游戏内必须保持默认 War3 RNG。
    /// </summary>
    public static Func<float> Next01Provider { get; set; } = () => JassApi.GetRandomReal(0f, 1f);

    /// <summary>返回 [0,1) 的同步随机浮点数。</summary>
    public static float Next01()
    {
        return Next01Provider();
    }

    /// <summary>返回 [minInclusive, maxInclusive] 的同步随机整数。</summary>
    public static int NextInt(int minInclusive, int maxInclusive)
    {
        return JassApi.GetRandomInt(minInclusive, maxInclusive);
    }

    /// <summary>返回 [minInclusive, maxInclusive] 的同步随机浮点数。</summary>
    public static float Next(float minInclusive, float maxInclusive)
    {
        return JassApi.GetRandomReal(minInclusive, maxInclusive);
    }
}
