namespace War3Frame
{
    public partial class AssetsInit
    {
        public static void SoundsInit(Library.Assets assets)
        {
            //【 打击音效 】

            assets.AddVWP("metal_slice_medium");//武器声音，每种3段
            //【 常规音效 】

            assets.AddVCM("Sound\\Interface\\Error.wav", "war3_Error");
            assets.AddVCM("Sound\\Interface\\HeroDropItem1.wav", "war3_HeroDropItem1");
            assets.AddVCM("Sound\\Interface\\MapPing.wav", "war3_MapPing");
            assets.AddVCM("Sound\\Interface\\MouseClick1.wav", "war3_MouseClick1");
            assets.AddVCM("Sound\\Interface\\MouseClick2.wav", "war3_MouseClick2");
            assets.AddVCM("Sound\\Interface\\PickUpItem.wav", "war3_PickUpItem");
            assets.AddVCM("Sound\\Interface\\QuestLog.wav", "war3_QuestLog");
            assets.AddVCM("Sound\\Interface\\ReceiveGold.wav", "war3_ReceiveGold");
        }
    }
}
