using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FastMDX;

using static OptionalBlocks;
using Transforms = Dictionary<OptionalBlocks, IOptionalBlocksParser<Node>>;

public unsafe struct Node : IDataRW
{
    public LocalProperties Properties;
    public Transform<Vec3> Translation, Scaling;
    public Transform<Vec4> Rotation;

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
        [KGTR] = new OptionalBlockParser<Transform<Vec3>, Node>((ref p) => ref p.Translation),
        [KGRT] = new OptionalBlockParser<Transform<Vec4>, Node>((ref p) => ref p.Rotation),
        [KGSC] = new OptionalBlockParser<Transform<Vec3>, Node>((ref p) => ref p.Scaling)
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LocalProperties
    {
        private const uint NAME_LEN = 80;

        private fixed byte name[(int)NAME_LEN];
        public int ObjectId, ParentId;
        public Flags Flags;

        public string Name
        {
            get
            {
                fixed (byte* n = name)
                {
                    return BinaryString.Decode(n, NAME_LEN);
                }
            }
            set
            {
                fixed (byte* n = name)
                {
                    BinaryString.Encode(value, n, NAME_LEN);
                }
            }
        }
    }

    [Flags]
    public enum Flags : uint
    {
        Helper = 0x0,
        DontInheritTranslation = 0x1,
        DontInheritRotation = 0x2,
        DontInheritScaling = 0x4,
        Billboarded = 0x8,
        BillboardedLockX = 0x10,
        BillboardedLockY = 0x20,
        BillboardedLockZ = 0x40,
        CameraAnchored = 0x80,
        Bone = 0x100,
        Light = 0x200,
        EventObject = 0x400,
        Attachment = 0x800,
        ParticleEmitter = 0x1000,
        CollisionShape = 0x2000,
        RibbonEmitter = 0x4000,
        PEUsesMdlPE2Unshaded = 0x8000,
        PEUsesTgaPE2SortPrimitivesFarZ = 0x10000,
        LineEmitter = 0x20000,
        Unfogged = 0x40000,
        ModelSpace = 0x80000,
        XYQuad = 0x100000
    }
}