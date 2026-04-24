using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FastMDX;

using static OptionalBlocks;
using Transforms = Dictionary<OptionalBlocks, IOptionalBlocksParser<ParticleEmitter>>;

public unsafe struct ParticleEmitter : IDataRW
{
    public Node Node;
    public LocalProperties Properties;

    public Transform<float> EmissionRateTransform,
        GravityTransform,
        LongitudeTransform,
        LatitudeTransform,
        LifespanTransform,
        SpeedTransform,
        VisibilityTransform;

    void IDataRW.ReadFrom(DataStream ds)
    {
        var end = ds.Offset + ds.ReadStruct<uint>();
        ds.ReadData(ref Node);
        ds.ReadStruct(ref Properties);
        ds.ReadOptionalBlocks(ref this, _knownTransforms, end);
    }

    void IDataRW.WriteTo(DataStream ds)
    {
        var offset = ds.Offset;
        ds.Skip(sizeof(uint));
        ds.WriteData(ref Node);
        ds.WriteStruct(ref Properties);
        ds.WriteOptionalBlocks(ref this, _knownTransforms);
        ds.SetValueAt(offset, ds.Offset - offset);
    }

    private static readonly Transforms _knownTransforms = new()
    {
        [KPEE] = new OptionalBlockParser<Transform<float>, ParticleEmitter>((ref p) => ref p.EmissionRateTransform),
        [KPEG] = new OptionalBlockParser<Transform<float>, ParticleEmitter>((ref p) => ref p.GravityTransform),
        [KPLN] = new OptionalBlockParser<Transform<float>, ParticleEmitter>((ref p) => ref p.LongitudeTransform),
        [KPLT] = new OptionalBlockParser<Transform<float>, ParticleEmitter>((ref p) => ref p.LatitudeTransform),
        [KPEL] = new OptionalBlockParser<Transform<float>, ParticleEmitter>((ref p) => ref p.LifespanTransform),
        [KPES] = new OptionalBlockParser<Transform<float>, ParticleEmitter>((ref p) => ref p.SpeedTransform),
        [KPEV] = new OptionalBlockParser<Transform<float>, ParticleEmitter>((ref p) => ref p.VisibilityTransform)
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LocalProperties
    {
        private const uint PATH_LEN = 260;

        public float EmissionRate, Gravity, Longitude, Latitude;
        private fixed byte name[(int)PATH_LEN];
        public float Lifespan, Speed;

        public string Path
        {
            get
            {
                fixed (byte* n = name)
                {
                    return BinaryString.Decode(n, PATH_LEN);
                }
            }
            set
            {
                fixed (byte* n = name)
                {
                    BinaryString.Encode(value, n, PATH_LEN);
                }
            }
        }
    }
}