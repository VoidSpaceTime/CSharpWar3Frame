using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace War3Frame.Library.Api
{
    public partial class KKApi
    {
        // title = "游戏 - 模拟按键 (窗口消息)"
        public static void DzSendKeyboard(JPlayer p, int key_code, int is_down)
        {
            War3.CallNative(War3.GetNativeFunction("DzSendKeyboard"), p, key_code, is_down);
        }

        // title = "游戏 - 模拟按键 (游戏UI消息)"
        public static void DzForceUiKeyboard(JPlayer p, int key_code, int is_down)
        {
            War3.CallNative(War3.GetNativeFunction("DzForceUiKeyboard"), p, key_code, is_down);
        }

        // title = "游戏 - 屏蔽按键 (窗口消息)"
        public static void DzDisableWindowKeyboard(JPlayer p, int key_code)
        {
            War3.CallNative(War3.GetNativeFunction("DzDisableWindowKeyboard"), p, key_code);
        }

        // title = "游戏 - 屏蔽按键 (游戏UI消息)"
        public static void DzDisableGameUIKeyboard(JPlayer p, int key_code)
        {
            War3.CallNative(War3.GetNativeFunction("DzDisableGameUIKeyboard"), p, key_code);
        }

        // title = "单位 - 是否可以被放置到坐标"
        public static bool DzUnitCanPlaceAround(JWidget obj, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzUnitCanPlaceAround"), obj, x, y);
        }

        // title = "单位 - 是否可以被放置到点"
        public static bool KKUnitCanPlaceAroundLoc(JWidget obj, JLocation loc)
        {
            return DzUnitCanPlaceAround(obj, JassApi.GetLocationX(loc), JassApi.GetLocationY(loc));
        }

        // title = "物品 - 是否可以被放置到坐标"
        public static bool kkUnitCanPlaceAroundItem(JWidget obj, float x, float y)
        {
            return DzUnitCanPlaceAround(obj, x, y);
        }

        // title = "物品 - 是否可以被放置到点"
        public static bool KKUnitCanPlaceAroundLocItem(JWidget obj, JLocation loc)
        {
            return DzUnitCanPlaceAround(obj, JassApi.GetLocationX(loc), JassApi.GetLocationY(loc));
        }

        // title = "坐标 - 是否可以能够通过物体"
        public static bool DzPositionCanPlaceAround(float x, float y, float collision_size, int collision_type)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzPositionCanPlaceAround"), x, y, collision_size, collision_type);
        }

        // title = "点 - 是否可以能够通过物体"
        public static bool KKPositionCanPlaceAroundLoc(JLocation loc, float collision_size, int collision_type)
        {
            return DzPositionCanPlaceAround(JassApi.GetLocationX(loc), JassApi.GetLocationY(loc), collision_size, collision_type);
        }

        // title = "坐标 - 获取地形Z轴高度"
        public static float DzGetTerrainZ(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetTerrainZ"), x, y);
        }

        // title = "单位 - 获取单位Z轴高度"
        public static float DzGetUnitZ(JUnit u)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitZ"), u);
        }

        // title = "单位 - 获取单位头顶高度偏移"
        public static float DzGetUnitOverheadOffset(JWidget u)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitOverheadOffset"), u);
        }

        // title = "界面 - ui模型 - 设置宽屏补丁"
        public static void DzFrameSetModelEnableWideScreen(int frame, bool is_enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelEnableWideScreen"), frame, is_enable);
        }

        // title = "技能 - 设置技能启用"
        public static bool DzSetUnitAbilityEnable(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityEnable"), u, abil_id);
        }

        // title = "技能 - 设置技能禁用"
        public static bool DzSetUnitAbilityDisable(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDisable"), u, abil_id);
        }

        // title = "技能 - 获取当前是否禁用状态"
        public static bool DzGetUnitAbilityIsDisabled(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzGetUnitAbilityIsDisabled"), u, abil_id);
        }

        // title = "技能 - 获取当前禁用的内部计数"
        public static int DzGetUnitAbilityDisabledCount(JUnit u, int abil_id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityDisabledCount"), u, abil_id);
        }

        // title = "技能 - 设置技能科技条件达成"
        public static bool DzSetUnitAbilityTechReach(JUnit u, int abil_id, bool reach)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityTechReach"), u, abil_id, reach);
        }

        // title = "技能 - 获取当前科技条件是否达成"
        public static bool DzGetUnitAbilityTechReach(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzGetUnitAbilityTechReach"), u, abil_id);
        }

        // title = "技能 - 设置技能科技条件文本"
        public static bool DzSetUnitAbilityTechReachTip(JUnit u, int abil_id, string tip)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityTechReachTip"), u, abil_id, tip);
        }

        // title = "建造 - 异步获取当前正在建造的技能Id"
        public static int DzAsyncGetCurrentBuildingAbilityId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzAsyncGetCurrentBuildingAbilityId"));
        }

        // title = "建造 - 异步获取当前正在建造的单位Id"
        public static int DzAsyncGetCurrentBuildingUnitId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzAsyncGetCurrentBuildingUnitId"));
        }

        // title = "界面 - 解锁右下角区域鼠标焦点限制"
        public static void DzFrameUnlockMouseRectLimit(bool is_unlock)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameUnlockMouseRectLimit"), is_unlock);
        }

        // title = "界面 - 判断SimpleFrame类型控件是否显示"
        public static bool KKSimpleFrameIsVisible(int simple_frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("KKSimpleFrameIsVisible"), simple_frame);
        }

        // title = "界面 - 原生 - 获取聊天输入栏控件"
        public static int DzFrameGetChatEditBar()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetChatEditBar"));
        }

        // title = "玩家 - 获取本地玩家的聊天频道"
        public static int DzGetLocalChatRecipient()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetLocalChatRecipient"));
        }

        // title = "玩家 - 发送聊天消息(触发同步事件)"
        public static void DzPlayerSendChat(JPlayer p, string msg, int recipient)
        {
            War3.CallNative(War3.GetNativeFunction("DzPlayerSendChat"), p, msg, recipient);
        }
    }
}
