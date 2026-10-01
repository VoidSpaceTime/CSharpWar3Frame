using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Native;

/// <summary>
/// 单位复活/显隐原生执行系统。
/// 消费 UnitRebornNativeRequest：有 UnitNative 时按 ECS 生命上限回满并显示或隐藏，
/// 消费后删除请求；不创建、不销毁原生单位，也不触碰句柄引用登记。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class UnitRebornNativeSystem : QuerySystem<UnitRebornNativeRequest>
{
    // 与 UnitNativeSystem 的 0..10000 归一化约定一致：满生命 = 10000。
    private const float NativeFullLifeState = 10000f;

    protected override void OnUpdate()
    {
        // 循环内 RemoveComponent 属结构变更：先执行副作用并收集，循环外移除请求。
        var consumed = new List<Entity>();

        Query.ForEachEntity((ref UnitRebornNativeRequest request, Entity unit) =>
        {
            if (unit.TryGetComponent<UnitNative>(out var native))
            {
                if (request.show
                    && AttributeHelper.TryGetAttr(unit, AttributeHelper.Health, out var attr)
                    && attr.TryGetComponent<AttrValue>(out var value)
                    && value.finalValue > 0f)
                {
                    // 回到存活表现：以 ECS 生命上限为准把原生生命拉满，不读原生当前生命当真相。
                    JassApi.SetUnitState(native.unit, Blizzard.UNIT_STATE_LIFE, NativeFullLifeState);
                }

                JassApi.ShowUnit(native.unit, request.show);
            }

            consumed.Add(unit);
        });

        foreach (var unit in consumed)
        {
            if (!unit.IsNull)
                unit.RemoveComponent<UnitRebornNativeRequest>();
        }
    }
}
