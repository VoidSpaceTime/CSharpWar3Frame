using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace War3Frame.Library.Api
{
    public partial class KKApi
    {
        /// title = "游戏 - 模拟按键 (窗口消息)"        /// description = "让玩家 ${player} 发送窗口消息 模拟键盘 ${key_code} 进行 ${is_down} 的消息"        /// comment = "[[会触发响应同步事件, 发送模拟按键窗口消息给魔兽, 相当于SendMessage, 点击记得释放, 不然可能会键盘按键会在下一次失效, 聊天框显示时不执行]]"
        public static void DzSendKeyboard(JPlayer p, int key_code, int is_down)
        {
            War3.CallNative(War3.GetNativeFunction("DzSendKeyboard"), p.Handle, key_code, is_down);
        }

        /// title = "游戏 - 模拟按键 (游戏UI消息)"
        /// description = "让玩家 ${player} 发送游戏UI消息 模拟键盘 ${key_code} 进行 ${is_down} 的消息"
        /// comment = "[[发送UI按键消息给魔兽GameUI, 不会触发响应同步事件, 相当于自带ForceUiKey函数支持更多按键的版本, 某些特殊键无效。]]"

        public static void DzForceUiKeyboard(JPlayer p, int key_code, int is_down)
        {
            War3.CallNative(War3.GetNativeFunction("DzForceUiKeyboard"), p.Handle, key_code, is_down);
        }


        /// title = "游戏 - 屏蔽按键 (窗口消息)"
        /// description = "让玩家 ${player} 屏蔽窗口键盘 ${key_code} 的消息"
        /// comment = "[[会屏蔽硬件同步事件, 聊天框显示时不屏蔽]]"

        public static void DzDisableWindowKeyboard(JPlayer p, int key_code)
        {
            War3.CallNative(War3.GetNativeFunction("DzDisableWindowKeyboard"), p.Handle, key_code);
        }


        /// title = "游戏 - 屏蔽按键 (游戏UI消息)"
        /// description = "让玩家 ${player} 屏蔽游戏UI按键 ${key_code} 的消息"
        /// comment = "[[在屏蔽游戏UI的按键消息, 某些特殊键无效。 dz的硬件同步事件依旧会执行]]"

        public static void DzDisableGameUIKeyboard(JPlayer p, int key_code)
        {
            War3.CallNative(War3.GetNativeFunction("DzDisableGameUIKeyboard"), p.Handle, key_code);
        }


        /// title = "单位 - 是否可以被放置到坐标"
        /// description = "判断单位 ${单位} 是否可以放置到该坐标(${X轴}, ${Y轴})"
        /// comment = "判断地面通行条件以及碰撞范围"

        public static bool DzUnitCanPlaceAround(JWidget obj, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzUnitCanPlaceAround"), obj.Handle, x, y);
        }


        /// title = "单位 - 是否可以被放置到点"
        /// description = "判断单位 ${单位} 是否可以放置到该点 ${location}"
        /// comment = "判断地面通行条件以及碰撞范围"

        public static bool KKUnitCanPlaceAroundLoc(JWidget obj, JLocation loc)
        {
            return DzUnitCanPlaceAround(obj, JassApi.GetLocationX(loc), JassApi.GetLocationY(loc));
        }


        /// title = "物品 - 是否可以被放置到坐标"
        /// description = "判断物品 ${物品} 是否可以放置到该坐标(${X轴}, ${Y轴})"
        /// comment = "判断地面通行条件以及碰撞范围"

        public static bool kkUnitCanPlaceAroundItem(JWidget obj, float x, float y)
        {
            return DzUnitCanPlaceAround(obj, x, y);
        }


        /// title = "物品 - 是否可以被放置到点"
        /// description = "判断物品 ${物品} 是否可以放置到该点 ${location}"
        /// comment = "判断地面通行条件以及碰撞范围"

        public static bool KKUnitCanPlaceAroundLocItem(JWidget obj, JLocation loc)
        {
            return DzUnitCanPlaceAround(obj, JassApi.GetLocationX(loc), JassApi.GetLocationY(loc));
        }


        /// title = "坐标 - 是否可以能够通过物体"
        /// description = "判断 地形坐标(${X轴}, ${Y轴}) 是否可以能够通过 碰撞范围 ${collision} 碰撞类型 ${collision_type} 的物体"
        /// comment = "根据碰撞范围 碰撞类型判断地面通行条件"

        public static bool DzPositionCanPlaceAround(float x, float y, float collision_size, int collision_type)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzPositionCanPlaceAround"), x, y, collision_size, collision_type);
        }


        /// title = "点 - 是否可以能够通过物体"
        /// description = "判断 地形点 ${location} 是否可以能够通过 碰撞范围 ${collision} 碰撞类型 ${collision_type} 的物体"
        /// comment = "根据碰撞范围 碰撞类型判断地面通行条件"

        public static bool KKPositionCanPlaceAroundLoc(JLocation loc, float collision_size, int collision_type)
        {
            return DzPositionCanPlaceAround(JassApi.GetLocationX(loc), JassApi.GetLocationY(loc), collision_size, collision_type);
        }


        /// title = "坐标 - 获取地形Z轴高度"
        /// description = "获取 地形坐标(${X轴}, ${Y轴}) 的Z轴高度"
        /// comment = "跟GetLocationZ获取的结果一致, 地形z轴在某些特殊情况下可能会是异步的，请小心使用。"

        public static float DzGetTerrainZ(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetTerrainZ"), x, y);
        }


        /// title = "单位 - 获取单位Z轴高度"
        /// description = "获取 单位 ${unit} 地形Z轴高度"
        /// comment = "相当于 飞行高度+GetLocationZ, 地形z轴在某些特殊情况下可能会是异步的，请小心使用。"

        public static float DzGetUnitZ(JUnit u)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitZ"), u.Handle);
        }


        /// title = "单位 - 获取单位头顶高度偏移"
        /// description = "获取 单位 ${unit} 头顶高度偏移"
        /// comment = "加上单位z轴高度 相当于血条高度, 头顶高度偏移,因本地模型可能不一致结果可能是异步的,请小心使用"

        public static float DzGetUnitOverheadOffset(JWidget u)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitOverheadOffset"), u.Handle);
        }


        /// title = "界面 - ui模型 - 设置宽屏补丁"
        /// description = "设置ui模型控件 ${model_frame} 强制开启或关闭 ${is_enable} 宽屏补丁"
        /// comment = "只能是ui模型控件, true时强制指定ui模型开启宽屏补丁, false时强制指定ui模型关闭宽屏补丁。"

        public static void DzFrameSetModelEnableWideScreen(int frame, bool is_enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelEnableWideScreen"), frame, is_enable);
        }


        /// title = "技能 - 设置技能启用"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的启用状态 "
        /// comment = "[[内部有累计次数， 只有次数为小于等于0的时候才会真的启用。 单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityEnable(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityEnable"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能禁用"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 禁用状态"
        /// comment = "[[内部有累计次数， 只有次数大于0的时候才会真的禁用。 单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityDisable(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDisable"), u.Handle, abil_id);
        }


        /// title = "技能 - 获取当前是否禁用状态"
        /// description = "获取单位 ${单位} 的技能 ${技能id} 当前是否禁用状态"
        /// comment = "默被禁用后返回true"

        public static bool DzGetUnitAbilityIsDisabled(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzGetUnitAbilityIsDisabled"), u.Handle, abil_id);
        }


        /// title = "技能 - 获取当前禁用的内部计数"
        /// description = "获取单位 ${单位} 的技能 ${技能id} 当前禁用的内部计数"
        /// comment = "大于0是禁用状态，否则是开启状态"

        public static int DzGetUnitAbilityDisabledCount(JUnit u, int abil_id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityDisabledCount"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能科技条件达成"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的科技条件是否 ${reach} 达成"
        /// comment = "[[支持大部分技能、魔法书内技能、收费技能等。单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityTechReach(JUnit u, int abil_id, bool reach)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityTechReach"), u.Handle, abil_id, reach);
        }


        /// title = "技能 - 获取当前科技条件是否达成"
        /// description = "获取单位 ${单位} 的技能 ${技能id} 当前科技条件是否达成"
        /// comment = "true是已经解锁，false是未解锁科技"

        public static bool DzGetUnitAbilityTechReach(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzGetUnitAbilityTechReach"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能科技条件文本"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的科技条件的文本为 ${tip}"
        /// comment = "[[只有当科技未达成时才会显示,支持大部分技能、魔法书内技能、收费技能等。单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityTechReachTip(JUnit u, int abil_id, string tip)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityTechReachTip"), u.Handle, abil_id, tip);
        }


        /// title = "建造 - 异步获取当前正在建造的技能Id"
        /// description = "异步获取当前正在建造的技能Id"
        /// comment = "默认返回0,准备建造状态下 返回技能id, 注意该函数返回值是异步的，请谨慎使用。"

        public static int DzAsyncGetCurrentBuildingAbilityId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzAsyncGetCurrentBuildingAbilityId"));
        }


        /// title = "建造 - 异步获取当前正在建造的单位Id"
        /// description = "异步获取当前正在建造的单位Id"
        /// comment = "默认返回0,准备建造状态下 返回单位id, 注意该函数返回值是异步的，请谨慎使用。"

        public static int DzAsyncGetCurrentBuildingUnitId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzAsyncGetCurrentBuildingUnitId"));
        }


        /// title = "界面 - 解锁右下角区域鼠标焦点限制"
        /// description = "解锁右下角区域鼠标焦点限制 是否解锁 ${is_unlock} "
        /// comment = "[[解锁后右下角技能栏附近的Frame进入离开事件跟焦点可以生效。开局调用一次即可]]"

        public static void DzFrameUnlockMouseRectLimit(bool is_unlock)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameUnlockMouseRectLimit"), is_unlock);
        }


        /// title = "界面 - 判断SimpleFrame类型控件是否显示"
        /// description = "判断SimpleFrame类型控件 ${SimpleFrame} 是否显示"
        /// comment = "支持SimpleFrame、SimpleTexture、SimpleFontString及其扩展控件, 以及判断聊天框控件是否显示"

        public static bool KKSimpleFrameIsVisible(int simple_frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("KKSimpleFrameIsVisible"), simple_frame);
        }


        /// title = "界面 - 原生 - 获取聊天输入栏控件"
        /// description = "获取聊天输入栏控件"
        /// comment = "返回回车键按下时打开的聊天输入框控件。"

        public static int DzFrameGetChatEditBar()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetChatEditBar"));
        }


        /// title = "玩家 - 获取本地玩家的聊天频道"
        /// description = "获取本地玩家的聊天频道"
        /// comment = "返回值是异步的, 用在发生聊天消息时 获取玩家当前的聊天频道"

        public static int DzGetLocalChatRecipient()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetLocalChatRecipient"));
        }


        /// title = "玩家 - 发送聊天消息(触发同步事件)"
        /// description = "玩家 ${player} 发送聊天消息 ${message} 使用聊天频道 ${recipient} "
        /// comment = "[[player在异步事件里可以使用本地玩家, 在同步事件里可以指定玩家, 该函数会进行网络同步, 同步后会响应所有触发器聊天事件, 由于有同步请不要高频率执行。]]"

        public static void DzPlayerSendChat(JPlayer p, string msg, int recipient)
        {
            War3.CallNative(War3.GetNativeFunction("DzPlayerSendChat"), p.Handle, msg, recipient);
        }
    }
}
