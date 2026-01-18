namespace War3Frame
{
    public partial class AssetsInit
    {
        public static void SoundsInit(Library.Assets assets)
        {
            //【 打击音效 】

            assets.AddVWP("metal_slice_medium");//武器声音，每种3段
            //【 常规音效 】

            assets.AddVCM("Sound\\Interface\\Error.wav","war3_Error","Sound\\Interface\\Error.wav",616);
            assets.AddVCM("Sound\\Interface\\HeroDropItem1.wav","war3_HeroDropItem1","Sound\\Interface\\HeroDropItem1.wav",489);
            assets.AddVCM("Sound\\Interface\\MapPing.wav","war3_MapPing","Sound\\Interface\\MapPing.wav",1639);
            assets.AddVCM("Sound\\Interface\\MouseClick1.wav","war3_MouseClick1","Sound\\Interface\\MouseClick1.wav",241);
            assets.AddVCM("Sound\\Interface\\MouseClick2.wav","war3_MouseClick2","Sound\\Interface\\MouseClick2.wav",232);
            assets.AddVCM("Sound\\Interface\\PickUpItem.wav","war3_PickUpItem","Sound\\Interface\\PickUpItem.wav",200);
            assets.AddVCM("Sound\\Interface\\QuestLog.wav","war3_QuestLog","Sound\\Interface\\QuestLog.wav",2276);
            assets.AddVCM("Sound\\Interface\\ReceiveGold.wav","war3_ReceiveGold","Sound\\Interface\\ReceiveGold.wav",591);
        }
    }
}
