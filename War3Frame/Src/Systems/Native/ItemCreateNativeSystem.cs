using Friflo.Engine.ECS.Systems;
using War3Frame.Components.Item;
using War3Frame.Systems;

namespace War3Frame.Systems.Native;

/// <summary>
/// 原生物品创建系统。决定走UI+特效了
/// </summary>
// TODO Native：地面物品表现尚未实现，不注册为默认执行能力。
// 请求类型保留给独立的 item-ground-simulation 设计；不得吞异常假装已创建。
public class ItemCreateNativeSystem : QuerySystem<ItemCreateNativeRequest>
{
    protected override void OnUpdate()
    {
        throw new NotImplementedException();
    }
}

