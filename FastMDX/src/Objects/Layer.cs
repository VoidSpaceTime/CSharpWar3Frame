using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FastMDX;

using static OptionalBlocks;
using Transforms = Dictionary<OptionalBlocks, IOptionalBlocksParser<Layer>>;

public struct Layer : IDataRW
{
    public LocalProperties Properties;
    public Transform<int> MaterialTextureIdTransform;
    public Transform<float> MaterialAlphaTransform;

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
        [KMTF] = new OptionalBlockParser<Transform<int>, Layer>((ref p) => ref p.MaterialTextureIdTransform),
        [KMTA] = new OptionalBlockParser<Transform<float>, Layer>((ref p) => ref p.MaterialAlphaTransform)
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LocalProperties
    {
        public FilterMode FilterMode;
        public ShadingFlags ShadingFlags;
        public int TextureId, TextureAnimationId, CoordId;
        public float Alpha;
    }

    public enum FilterMode : uint
    {
        None,
        Transparent,
        Blend,
        Additive,
        AddAlpha,
        Modulate,
        Modulate2x
    }

    [Flags]
    public enum ShadingFlags : uint
    {
        Unshaded = 0x1,
        SphereEnvironmentMap = 0x2,
        TwoSided = 0x10,
        Unfogged = 0x20,
        NoDepthTest = 0x40,
        NoDepthSet = 0x80
    }
}