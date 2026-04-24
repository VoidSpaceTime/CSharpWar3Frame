using System.Collections.Generic;

namespace FastMDX;

using static MainBlocks;
using Parsers = Dictionary<MainBlocks, IBlockParser>;

public partial class MDX
{
    private static readonly Parsers _knownParsers = new()
    {
        [MODL] = new StructParser<ModelInfo>(mdx => ref mdx.Info),
        [SEQS] = new StructArrayParser<Sequence>(mdx => ref mdx.Sequences),
        [GLBS] = new StructArrayParser<GlobalSequence>(mdx => ref mdx.GlobalSequences),
        [MTLS] = new DataArrayParser<Material>(mdx => ref mdx.Materials),
        [TEXS] = new StructArrayParser<Texture>(mdx => ref mdx.Textures),
        [TXAN] = new DataArrayParser<TextureAnimation>(mdx => ref mdx.TextureAnimations),
        [GEOS] = new DataArrayParser<Geoset>(mdx => ref mdx.Geosets),
        [GEOA] = new DataArrayParser<GeosetAnimation>(mdx => ref mdx.GeosetAnimations),
        [BONE] = new DataArrayParser<Bone>(mdx => ref mdx.Bones),
        [LITE] = new DataArrayParser<Light>(mdx => ref mdx.Lights),
        [HELP] = new DataArrayParser<Helper>(mdx => ref mdx.Helpers),
        [ATCH] = new DataArrayParser<Attachment>(mdx => ref mdx.Attachments),
        [PIVT] = new StructArrayParser<Pivot>(mdx => ref mdx.Pivots),
        [PREM] = new DataArrayParser<ParticleEmitter>(mdx => ref mdx.ParticleEmitters),
        [PRE2] = new DataArrayParser<ParticleEmitter2>(mdx => ref mdx.ParticleEmitters2),
        [RIBB] = new DataArrayParser<RibbonEmitter>(mdx => ref mdx.RibbonEmitters),
        [EVTS] = new DataArrayParser<EventObject>(mdx => ref mdx.EventObjects),
        [CAMS] = new DataArrayParser<Camera>(mdx => ref mdx.Cameras),
        [CLID] = new DataArrayParser<CollisionShape>(mdx => ref mdx.CollisionShapes)
    };

    public Attachment[] Attachments;
    public Bone[] Bones;
    public Camera[] Cameras;
    public CollisionShape[] CollisionShapes;
    public EventObject[] EventObjects;
    public GeosetAnimation[] GeosetAnimations;
    public Geoset[] Geosets;
    public GlobalSequence[] GlobalSequences;
    public Helper[] Helpers;
    public ModelInfo Info;
    public Light[] Lights;
    public Material[] Materials;
    public ParticleEmitter[] ParticleEmitters;
    public ParticleEmitter2[] ParticleEmitters2;
    public Pivot[] Pivots;
    public RibbonEmitter[] RibbonEmitters;
    public Sequence[] Sequences;
    public TextureAnimation[] TextureAnimations;
    public Texture[] Textures;

    public BinaryBlock[] UnknownBlocks;
}

internal interface IBlockParser
{
    public void ReadFrom(MDX mdx, DataStream ds, uint blockSize);
    public void WriteTo(MDX mdx, DataStream ds);
    public bool HasData(MDX mdx);
}