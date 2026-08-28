using Friflo.Engine.ECS.Systems;
using War3Frame.Components.Item;
using War3Frame.Systems;

namespace War3Frame.Systems.Native;

/// <summary>
/// 原生物品创建系统。决定走UI+特效了
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class ItemCreateNativeSystem : QuerySystem<ItemCreateNativeRequest>
{
    protected override void OnUpdate()
    {
        throw new NotImplementedException();
    }
}
 