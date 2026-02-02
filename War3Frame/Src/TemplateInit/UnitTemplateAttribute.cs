namespace War3Frame.TemplateInit;

// 定义 Attribute
[AttributeUsage(AttributeTargets.Class)]
public class UnitTemplateAttribute : Attribute
{
    public string Name { get; }
    public UnitTemplateAttribute(string name) => Name = name;
}

/*// 每个模板文件
[UnitTemplate("footman")]
public partial class FootmanTemplate : IUnitTemplate
{
    public void Configure(Entity e) 
    {
        e.AddComponent(new Health { max = 420 });
    }
}*/