namespace FastMDX;

internal class StructArrayParser<T> : IBlockParser where T : unmanaged
{
    private readonly _getRef Get;

    internal StructArrayParser(_getRef get)
    {
        Get = get;
    }

    public unsafe void ReadFrom(MDX mdx, DataStream ds, uint blockSize)
    {
        if (blockSize % sizeof(T) > 0)
            throw new ParsingException();

        Get(mdx) = ds.ReadStructArray<T>(blockSize / (uint)sizeof(T));
    }

    public void WriteTo(MDX mdx, DataStream ds)
    {
        ds.WriteStructArray(Get(mdx), false);
    }

    public bool HasData(MDX mdx)
    {
        return Get(mdx)?.Length > 0;
    }

    internal delegate ref T[] _getRef(MDX mdx);
}