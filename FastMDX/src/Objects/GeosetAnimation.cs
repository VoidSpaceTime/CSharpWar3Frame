using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FastMDX;

using static OptionalBlocks;
using Transforms = Dictionary<OptionalBlocks, IOptionalBlocksParser<GeosetAnimation>>;

public struct GeosetAnimation : IDataRW
{
    public LocalProperties Properties;
    public Transform<float> GeosetAlphaTransform;
    public Transform<Color> GeosetColorTransform;

    void IDataRW.ReadFrom(DataStream ds)
    {
        var end = ds.Offset + ds.ReadStruct<uint>();
        ds.ReadStruct(ref Properties);
        ds.ReadOptionalBlocks(ref this, _knownTransforms, end);
    }

    void IDataRW.WriteTo(DataStream ds)
    {
        var offset = ds.Offset;
        ds.Skip(sizeof(uint));
        ds.WriteStruct(ref Properties);
        ds.WriteOptionalBlocks(ref this, _knownTransforms);
        ds.SetValueAt(offset, ds.Offset - offset);
    }

    private static readonly Transforms _knownTransforms = new()
    {
        [KGAO] = new OptionalBlockParser<Transform<float>, GeosetAnimation>((ref p) => ref p.GeosetAlphaTransform),
        [KGAC] = new OptionalBlockParser<Transform<Color>, GeosetAnimation>((ref p) => ref p.GeosetColorTransform)
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LocalProperties
    {
        public float Alpha;
        public Flags Flags;
        public Color Color;
        public int GeosetId;
    }

    [Flags]
    public enum Flags : uint
    {
        None = 0x0,
        DropShadow = 0x1,
        Color = 0x2
    }
}