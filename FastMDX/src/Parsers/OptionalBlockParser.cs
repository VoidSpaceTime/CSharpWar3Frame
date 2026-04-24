namespace FastMDX;

internal class OptionalBlockParser<T, P> : IOptionalBlocksParser<P>
    where T : struct, IDataRW, IOptionalBlock where P : struct, IDataRW
{
    private readonly _getRef Get;

    internal OptionalBlockParser(_getRef get)
    {
        Get = get;
    }

    public void ReadFrom(ref P p, DataStream ds)
    {
        ds.ReadData(ref Get(ref p));
    }

    public void WriteTo(ref P p, DataStream ds)
    {
        ds.WriteData(ref Get(ref p));
    }

    public bool HasData(ref P p)
    {
        return Get(ref p).HasData;
    }

    internal delegate ref T _getRef(ref P p);
}

internal interface IOptionalBlocksParser<P> where P : struct, IDataRW
{
    void ReadFrom(ref P p, DataStream ds);
    void WriteTo(ref P p, DataStream ds);
    bool HasData(ref P p);
}

internal interface IOptionalBlock
{
    internal bool HasData { get; }
}