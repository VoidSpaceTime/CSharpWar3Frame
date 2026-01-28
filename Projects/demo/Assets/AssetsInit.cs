namespace War3Frame
{

    public static partial class AssetsInit
    { 
        public static Assets Init()
        {
            Assets AssetsList = new();
            FontInit(AssetsList);
            SoundsInit(AssetsList);
            ModeOriginInit(AssetsList);
            ImageBuffInit(AssetsList);
            return AssetsList;
        }
    }
}
