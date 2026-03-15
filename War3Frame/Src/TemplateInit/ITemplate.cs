using Friflo.Engine.ECS;

namespace War3Frame.TemplateInit;

public interface ITemplate
{
    /// <summary>
    /// 配置技能 Entity 的组件
    /// </summary>
    void Configure(Entity entity);
}