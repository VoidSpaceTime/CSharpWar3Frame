using static War3Frame.Assets;

namespace War3Frame
{
    public partial class AssetsInit
    {
        public static void SoundsInit(Assets assets)
        {
            //【 打击音效 】
            // 武器声音：metal_slice_medium，每种材质多段
            assets.AddVWP("metal_slice_medium", new VwpSound("metal_slice_medium", new Dictionary<string, List<(string pickPath, int duration)>> { { "flesh", new List<(string pickPath, int duration)> { ("Sound\\Units\\Combat\\MetalMediumSliceFlesh1.wav", 659), ("Sound\\Units\\Combat\\MetalMediumSliceFlesh2.wav", 602), ("Sound\\Units\\Combat\\MetalMediumSliceFlesh3.wav", 605) } }, { "metal", new List<(string pickPath, int duration)> { ("Sound\\Units\\Combat\\MetalMediumSliceMetal1.wav", 625), ("Sound\\Units\\Combat\\MetalMediumSliceMetal2.wav", 617), ("Sound\\Units\\Combat\\MetalMediumSliceMetal3.wav", 670) } }, { "rock", new List<(string pickPath, int duration)> { ("Sound\\Units\\Combat\\MetalMediumSliceStone1.wav", 688), ("Sound\\Units\\Combat\\MetalMediumSliceStone2.wav", 731), ("Sound\\Units\\Combat\\MetalMediumSliceStone3.wav", 645) } }, { "wood", new List<(string pickPath, int duration)> { ("Sound\\Units\\Combat\\MetalMediumSliceWood1.wav", 571), ("Sound\\Units\\Combat\\MetalMediumSliceWood2.wav", 786), ("Sound\\Units\\Combat\\MetalMediumSliceWood3.wav", 813) } } })); //武器声音，每种3段
            //【 常规音效 】
            assets.AddVCM("Sound\\Interface\\Error.wav", "war3_Error", "Sound\\Interface\\Error.wav", 616);
            assets.AddVCM("Sound\\Interface\\HeroDropItem1.wav", "war3_HeroDropItem1", "Sound\\Interface\\HeroDropItem1.wav", 489);
            assets.AddVCM("Sound\\Interface\\MapPing.wav", "war3_MapPing", "Sound\\Interface\\MapPing.wav", 1639);
            assets.AddVCM("Sound\\Interface\\MouseClick1.wav", "war3_MouseClick1", "Sound\\Interface\\MouseClick1.wav", 241);
            assets.AddVCM("Sound\\Interface\\MouseClick2.wav", "war3_MouseClick2", "Sound\\Interface\\MouseClick2.wav", 232);
            assets.AddVCM("Sound\\Interface\\PickUpItem.wav", "war3_PickUpItem", "Sound\\Interface\\PickUpItem.wav", 200);
            assets.AddVCM("Sound\\Interface\\QuestLog.wav", "war3_QuestLog", "Sound\\Interface\\QuestLog.wav", 2276);
            assets.AddVCM("Sound\\Interface\\ReceiveGold.wav", "war3_ReceiveGold", "Sound\\Interface\\ReceiveGold.wav", 591);
        }
    }
}