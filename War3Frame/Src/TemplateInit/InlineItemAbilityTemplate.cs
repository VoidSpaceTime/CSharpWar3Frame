using Friflo.Engine.ECS;
using War3Frame.Helpers;

namespace War3Frame.TemplateInit;

/// <summary>
/// 保存物品私有 AbilitySpec，并通过统一 Apply 路径配置 companion ability。
/// </summary>
internal sealed class InlineItemAbilityTemplate : IAbilityTemplate
{
    private readonly AbilitySpec _spec;

    /// <summary>规范化后的物品模板逻辑 owner。</summary>
    internal string Owner { get; }

    /// <summary>
    /// 创建只持有结构化规格的内部模板，不保留 authoring lambda。
    /// </summary>
    internal InlineItemAbilityTemplate(string owner, AbilitySpec spec)
    {
        Owner = owner;
        _spec = spec;
    }

    /// <summary>
    /// 按 companion 当前等级应用规格；spec 在注册后只读，可安全跨实例共享。
    /// </summary>
    public void Configure(Entity entity, int level)
    {
        AbilitySpecBuilder.Apply(entity, level, _spec);
    }
}
