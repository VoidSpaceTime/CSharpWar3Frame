namespace War3Frame.Systems;

public enum SystemKind
{
    Interval,
    Immediate
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SystemRegisterAttribute(SystemKind systemKind, int order = 1) : Attribute
{
    public SystemKind SystemKind { get; } = systemKind;
    public int Order { get; } = order;
}