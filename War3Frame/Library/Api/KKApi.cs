using System;
using System.Numerics;
using System.Reflection;
using System.Threading;
using War3Frame;
using War3Frame.Library.Api;

namespace War3Frame.Library.Api
{
    // JAPI自kk库，包含kk平台引擎自带的japi方法，方法以 KK_ 开头

    public static partial class KKApi
    {
        /// title = "玩家是否拥有地图商城道具"        /// description = "获取 ${whichPlayer} 是否拥有: ${key} 对应的地图商城道具."        /// comment = "检测玩家背包中是否拥该道具且处于有效状态。已过期的时效性道具、剩余数量为0的数量型道具均视为无效；"
        public static bool DzAPI_Map_HasMallItem(JPlayer whichPlayer, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzAPI_Map_HasMallItem"), whichPlayer.Handle, key);
        }


        /// title = "玩家地图等级"
        /// description = "获取 ${player} 的地图等级"
        /// comment = "玩家地图等级【RPG大厅限定】"

        public static int DzAPI_Map_GetMapLevel(JPlayer whichPlayer)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzAPI_Map_GetMapLevel"), whichPlayer.Handle);
        }

        public static int RequestExtraIntegerData(int dataType, JPlayer whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("RequestExtraIntegerData"), dataType, whichPlayer.Handle, param1, param2, param3, param4, param5, param6);
        }

        public static bool RequestExtraBooleanData(int dataType, JPlayer whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("RequestExtraBooleanData"), dataType, whichPlayer.Handle, param1, param2, param3, param4, param5, param6);
        }

        public static string RequestExtraStringData(int dataType, JPlayer whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("RequestExtraStringData"), dataType, whichPlayer.Handle, param1, param2, param3, param4, param5, param6);
        }

        public static float RequestExtraRealData(int dataType, JPlayer whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("RequestExtraRealData"), dataType, whichPlayer.Handle, param1, param2, param3, param4, param5, param6);
        }


        /// title = "保存服务器存档"
        /// description = " ${玩家} 保存存档[ ${存档名称} ][ ${存档内容} ]最大长度64位"
        /// comment = "保存服务器存档"

        public static bool DzAPI_Map_SaveServerValue(JPlayer whichPlayer, string key, string value)
        {
            return RequestExtraBooleanData(4, whichPlayer, key, value, false, 0, 0, 0);
        }


        /// title = "读取服务器存储的数据"
        /// description = "获取 ${whichPlayer}  ${key} 里的数据"
        /// comment = ""

        public static string DzAPI_Map_GetServerValue(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(5, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "本局游戏的开始时间"
        /// description = "本局游戏的开始时间"
        /// comment = "获取本局游戏开始时的服务器时间。"

        public static int DzAPI_Map_GetGameStartTime()
        {
            return RequestExtraIntegerData(11, null, null, null, false, 0, 0, 0);
        }


        /// title = "本局游戏是否天梯排位赛"
        /// description = "判断地图是否在RPG天梯"
        /// comment = "本局游戏是否通过RPG天梯启动，如果地图配置了多个天梯模式，可通过获取本局游戏的地图模式接口获取具体选定的是哪一个天梯模式。"

        public static bool DzAPI_Map_IsRPGLadder()
        {
            return RequestExtraBooleanData(12, null, null, null, false, 0, 0, 0);
        }


        /// title = "本局游戏的地图模式"
        /// description = "本局游戏的地图模式"
        /// comment = "获取本局游戏所选择地图模式，地图模式均由作者在开发者平台进行配置（包括天梯排位赛模式、快速匹配模式、建房间时房主所选定的地图模式）。"

        public static int DzAPI_Map_GetMatchType()
        {
            return RequestExtraIntegerData(13, null, null, null, false, 0, 0, 0);
        }


        /// title = "上报房间内显示的数据"
        /// description = "设置 ${whichPlayer} 房间的 ${key} 项目显示 ${value} "
        /// comment = "作者可以将游戏内的关键数值或结果上报给平台，用于在平台游戏房间内展示以方便玩家相互快速了解实力，数据上报后需在开发者平台进行配置后才能展示出来。比如：比如获得MVP次数、最高通关难度等。"

        public static void DzAPI_Map_Stat_SetStat(JPlayer whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(7, whichPlayer, key, value, false, 0, 0, 0);
        }


        /// title = "天梯提交字符串数据"
        /// description = "提交 ${whichPlayer} 天梯项目: ${key} 的值为: ${value} "
        /// comment = ""

        public static void DzAPI_Map_Ladder_SetStat(JPlayer whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(8, whichPlayer, key, value, false, 0, 0, 0);
        }

        public static void DzAPI_Map_Ladder_SetPlayerStat(JPlayer whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(9, whichPlayer, key, value, false, 0, 0, 0);
        }

        public static int DzAPI_Map_GetServerValueErrorCode(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(6, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家天梯等级"
        /// description = "获取 ${player} 天梯等级"
        /// comment = "获取玩家在当前游戏局所采用的天梯模式下的天梯等级，仅天梯模式下的游戏局有效。"

        public static int DzAPI_Map_GetLadderLevel(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(14, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "获取玩家身份类型"
        /// description = " ${whichPlayer} 的身份是 ${key} "
        /// comment = ""

        public static bool KKApiPlayerIdentityType(JPlayer whichPlayer, int id)
        {
            return RequestExtraBooleanData(92, whichPlayer, null, null, false, id, 0, 0);
        }


        /// title = "玩家是否平台认证的职业选手"
        /// description = "判断 ${player} 是否是职业选手"
        /// comment = "判断玩家是否平台认证的职业选手（红V）。"

        public static bool DzAPI_Map_IsRedVIP(JPlayer whichPlayer)
        {
            return KKApiPlayerIdentityType(whichPlayer, 4);
        }


        /// title = "玩家是否平台认证的主播"
        /// description = "判断 ${player} 平台认证的主播（蓝V）"
        /// comment = "判断指定玩家是否平台认证的主播（蓝V）。"

        public static bool DzAPI_Map_IsBlueVIP(JPlayer whichPlayer)
        {
            return KKApiPlayerIdentityType(whichPlayer, 3);
        }


        /// title = "玩家天梯排名"
        /// description = "获取 ${player} 天梯排名"
        /// comment = "排名>1000的获取值为0，获取玩家在当前游戏局所采用的天梯模式下的天梯排名，仅天梯模式下的游戏局有效。"

        public static int DzAPI_Map_GetLadderRank(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(17, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家在地图等级排行榜上的排名"
        /// description = "获取 ${player} 在地图等级排行榜上的排名"
        /// comment = "排名>100的获取值为0"

        public static int DzAPI_Map_GetMapLevelRank(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(18, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家在公会的职责【废弃】"
        /// description = "获取 ${whichPlayer} 公会职责"
        /// comment = "获取玩家公会职责 Member=10 Admin=20 Leader=30"

        public static int DzAPI_Map_GetGuildRole(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(20, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "本局游戏是否处于RPG游戏大厅"
        /// description = "本局游戏是否处于RPG游戏大厅"
        /// comment = "获取当前游戏局是否通过RPG游戏大厅启动。"

        public static bool DzAPI_Map_IsRPGLobby()
        {
            return RequestExtraBooleanData(10, null, null, null, false, 0, 0, 0);
        }

        public static void DzAPI_Map_MissionComplete(JPlayer whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(1, whichPlayer, key, value, false, 0, 0, 0);
        }

        public static string DzAPI_Map_GetActivityData()
        {
            return RequestExtraStringData(2, null, null, null, false, 0, 0, 0);
        }


        /// title = "地图配置参数"
        /// description = "获取: ${key} 地图配置参数"
        /// comment = "获取作者在开发者平台配置的地图参数（原只读类型的地图全局存档），作者可以通过此接口实现节日活动开关、口令等功能。"

        public static string DzAPI_Map_GetMapConfig(string key)
        {
            return RequestExtraStringData(21, null, key, null, false, 0, 0, 0);
        }


        /// title = "保存服务器存档组"
        /// description = "在服务器存档组:存储 ${whichPlayer} 数据,名称： ${key} ,值: ${value} "
        /// comment = "将变量保存到当前作者账号下的服务器存档组中。"

        public static bool DzAPI_Map_SavePublicArchive(JPlayer whichPlayer, string key, string value)
        {
            return RequestExtraBooleanData(31, whichPlayer, key, value, false, 0, 0, 0);
        }


        /// title = "读取服务器存档组"
        /// description = "在服务器存档组读取 ${whichPlayer} ,名称: ${key} 里的数据"
        /// comment = "读取当前作者账号下的服务器存档组变量数据。。"

        public static string DzAPI_Map_GetPublicArchive(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(32, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "使用地图商城道具（局数型）"
        /// description = "设置该局消耗 ${whichPlayer} 地图商城道具： ${key} 一个"
        /// comment = "扣减玩家背包中的局数型道具1个，多次对同一个道具调用也只扣减1次。需先通过获取道具剩余数量确保玩家背包中的道具剩余数量大于0。"

        public static void DzAPI_Map_UseConsumablesItem(JPlayer whichPlayer, string key)
        {
            RequestExtraIntegerData(33, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "触发BOSS击杀"
        /// description = "设置 ${whichPlayer} 触发击杀 ${bosskey} "
        /// comment = "告知平台服务器游戏内发生了BOSS击杀，请求平台服务器计算BOSS掉落内容。触发之后用[DzAPI_Map_GetServerArchiveEquip]和[DzAPI_Map_GetServerArchiveDrop]读取"

        public static void DzAPI_Map_OrpgTrigger(JPlayer whichPlayer, string key)
        {
            RequestExtraIntegerData(28, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "BOSS击杀后的掉落内容"
        /// description = "获取 ${whichPlayer} 击杀boss ${bosskey} 掉落的装备"
        /// comment = "游戏内调用触发BOSS击杀后，获取本次掉落内容。"

        public static string DzAPI_Map_GetServerArchiveDrop(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(27, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "BOSS击杀后的掉落数量"
        /// description = "读取 ${whichPlayer} BOSS击杀后的 ${itemkey} 掉落的数量"
        /// comment = "游戏内调用[触发BOSS击杀]后，获取本次掉落数量。"

        public static int DzAPI_Map_GetServerArchiveEquip(JPlayer whichPlayer, string key)
        {
            return RequestExtraIntegerData(26, whichPlayer, key, null, false, 0, 0, 0);
        }

        public static int DzAPI_Map_GetPlatformVIP(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(30, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家是否平台尊享会员"
        /// description = "玩家 ${player} 是平台尊享会员"
        /// comment = "判断玩家是否平台的尊享会员。"

        public static bool DzAPI_Map_IsPlatformVIP(JPlayer whichPlayer)
        {
            return DzAPI_Map_GetPlatformVIP(whichPlayer) > 0;
        }


        /// title = "读取全局存档"
        /// description = "读取全局存档 名称 ${key}"
        /// comment = "只读的公共存档请使用另一个API读取"

        public static string DzAPI_Map_Global_GetStoreString(string key)
        {
            return RequestExtraStringData(36, JassApi.GetLocalPlayer(), key, null, false, 0, 0, 0);
        }


        /// title = "保存全局存档"
        /// description = "在全局存档 ${key} ,存储值: ${value} "
        /// comment = "将变量数据存储到平台服务器上的全局存档中，保存时会受到开发者平台所配置的保存规则限制。保存成功后本局游戏及同一时间正在进行的其他游戏局内的所有玩家都会收到全局存档变化事件的事件广播。"

        public static void DzAPI_Map_Global_StoreString(string key, string value)
        {
            RequestExtraBooleanData(37, JassApi.GetLocalPlayer(), key, value, false, 0, 0, 0);
        }


        /// title = "全局存档变化事件"
        /// description = "全局存档发生变化"
        /// comment = "本局游戏或其他游戏保存的全局存档都会触发这个事件，请使用'同步'分类下的获取同步数据来获得发生变化的全局存档KEY值"

        public static void DzAPI_Map_Global_ChangeMsg(JTrigger trig)
        {
            DzApi.DzTriggerRegisterSyncData(trig, "DZGAU", true);
        }


        /// title = "读取服务器存档（区分大小写）"
        /// description = "获取服务器存储的数据（区分大小写） 玩家 ${player} 名称 ${key}"
        /// comment = "从服务器上读取变量数据，存档变量Key区分大小写，保存时会受到开发者平台所配置的防刷分规则限制。"

        public static string DzAPI_Map_ServerArchive(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(38, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "保存服务器存档（区分大小写）"
        /// description = "保存服务器存档（区分大小写） 玩家 ${player} 名称 ${key} 值 ${value}"
        /// comment = "用将变量存储到服务器，存档变量Key区分大小写。"

        public static void DzAPI_Map_SaveServerArchive(JPlayer whichPlayer, string key, string value)
        {
            RequestExtraBooleanData(39, whichPlayer, key, value, false, 0, 0, 0);
        }


        /// title = "本局游戏是否快速匹配"
        /// description = "判断玩家是否是通过匹配模式进入游戏"
        /// comment = "本局游戏是否通过RPG快速匹配启动，如果地图配置了多个匹配模式，可通过[获取本局游戏的地图模式]接口获取具体选定的是哪一个匹配模式。"

        public static bool DzAPI_Map_IsRPGQuickMatch()
        {
            return RequestExtraBooleanData(40, null, null, null, false, 0, 0, 0);
        }


        /// title = "玩家地图商城道具剩余数量"
        /// description = "获取 玩家 ${player} 地图商城道具 ${key} 的剩余库存次数"
        /// comment = "获取玩家地图商城道具剩余数量。仅对次数消耗型商品有效"

        public static int DzAPI_Map_GetMallItemCount(JPlayer whichPlayer, string key)
        {
            return RequestExtraIntegerData(41, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "使用地图商城道具（数量型）"
        /// description = "设置 ${whichPlayer} 使用地图商城道具 ${key}  ${数量} 个"
        /// comment = "扣减玩家地图商城中的数量消耗型道具，可以多次调用。"

        public static bool DzAPI_Map_ConsumeMallItem(JPlayer whichPlayer, string key, int count)
        {
            return RequestExtraBooleanData(42, whichPlayer, key, null, false, count, 0, 0);
        }


        /// title = "开启/关闭游戏内辅助功能"
        /// description = "设置 ${player} 的 ${option} 号平台功能为 ${enable}"
        /// comment = "地图可以根据自身特点，强制打开或关闭视距调整、显示血条/蓝条、智能施法功能。1号功能为锁定镜头距离，2号功能为显示血、蓝条"

        public static bool DzAPI_Map_EnablePlatformSettings(JPlayer whichPlayer, int option, bool enable)
        {
            return RequestExtraBooleanData(43, whichPlayer, null, null, enable, option, 0, 0);
        }


        /// title = "玩家服务器存档是否读取成功"
        /// description = " ${whichPlayer} 服务器存档是否读取成功."
        /// comment = "如果返回false代表读取失败,反之成功,之后游戏里平台不会再发送“服务器保存失败”的信息，所以希望地图作者在游戏开始给玩家发下信息服务器存档是否正确读取。"

        public static bool GetPlayerServerValueSuccess(JPlayer whichPlayer)
        {
            if (DzAPI_Map_GetServerValueErrorCode(whichPlayer) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// title = "服务器存储整数（区分大小写）"
        /// description = "服务器存储整数（区分大小写） 玩家 ${player} 名称 ${key} 值 ${value}"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”I”，与普通存档共用KEY"

        public static void DzAPI_Map_StoreIntegerEX(JPlayer whichPlayer, string key, int value)
        {
            key = "I" + key;
            RequestExtraBooleanData(39, whichPlayer, key, value.ToString(), false, 0, 0, 0);
        }


        /// title = "获取服务器存储的整数（区分大小写）"
        /// description = "获取服务器存储的整数（区分大小写） 玩家 ${player} 名称 ${key}"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”I”，用于开发者平台设置的防刷分存档，与普通存档共用KEY"

        public static int DzAPI_Map_GetStoredIntegerEX(JPlayer whichPlayer, string key)
        {
            int value = 0;
            key = "I" + key;
            value = JassApi.S2I(RequestExtraStringData(38, whichPlayer, key, null, false, 0, 0, 0));
            return value;
        }


        /// title = "保存整数变量至服务器"
        /// description = "服务器存档:存储 ${whichPlayer} 数据,名称： ${key} ,值: ${value} 最大长度63位"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”I,（如您的key是AA，实际key为IAA。【IAA用于开发者平台填写，在编辑器上获取和读都填写AA就可以了】）”"

        public static void DzAPI_Map_StoreInteger(JPlayer whichPlayer, string key, int value)
        {
            key = "I" + key;
            DzAPI_Map_SaveServerValue(whichPlayer, key, value.ToString());


        }


        /// title = "读取服务器上的整数变量"
        /// description = "获取 ${whichPlayer} 数据名称: ${key} 里存储的整数."
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”I”"

        public static int DzAPI_Map_GetStoredInteger(JPlayer whichPlayer, string key)
        {
            int value = 0;
            key = "I" + key;
            value = JassApi.S2I(DzAPI_Map_GetServerValue(whichPlayer, key));


            return value;
        }


        /// title = "玩家在地图自定义排行榜上的排名"
        /// description = "获取玩家 ${whichPlayer} 自定义排行榜KEY(开发者平台填写)： ${key} 的排名，【请注意100名以外的玩家获取的值为0！】"
        /// comment = "【100名以外的玩家排名为0】该功能适用于开发者平台-服务器存档-自定义排行榜 "

        public static int DzAPI_Map_CommentTotalCount1(JPlayer whichPlayer, int id)
        {
            return RequestExtraIntegerData(52, whichPlayer, null, null, false, id, 0, 0);
        }


        /// title = "保存实数变量至服务器"
        /// description = "服务器存档:存储 ${whichPlayer} 数据,名称： ${key} ,值: ${value} 最大长度63位"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”R,（如您的key是AA，实际key为RAA。【RAA用于开发者平台填写，在编辑器上获取和读都填写AA就可以了】”"

        public static void DzAPI_Map_StoreReal(JPlayer whichPlayer, string key, float value)
        {
            key = "R" + key;
            DzAPI_Map_SaveServerValue(whichPlayer, key, value.ToString());


        }


        /// title = "读取服务器上的实数变量"
        /// description = "获取 ${whichPlayer} 数据名称: ${key} 里存储的实数"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”R”"

        public static float DzAPI_Map_GetStoredReal(JPlayer whichPlayer, string key)
        {
            float.TryParse(DzAPI_Map_GetServerValue(whichPlayer, key), out float f);
            float value = 0f;
            key = "R" + key;
            value = f;


            return value;
        }


        /// title = "保存布尔值变量至服务器"
        /// description = "服务器存档:存储 ${whichPlayer} 数据,名称: ${key} ,值: ${value} 最大长度63位"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”B，（如您的key是AA，实际key为BAA。【BAA用于开发者平台填写，在编辑器上获取和读都填写AA就可以了】）”"

        public static void DzAPI_Map_StoreBoolean(JPlayer whichPlayer, string key, bool value)
        {
            key = "B" + key;
            if (value)
            {
                DzAPI_Map_SaveServerValue(whichPlayer, key, "1");
            }
            else
            {
                DzAPI_Map_SaveServerValue(whichPlayer, key, "0");
            }


        }


        /// title = "读取服务器上的布尔变量"
        /// description = "获取 ${whichPlayer} 数据名称: ${key} 里存储的布尔值"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”B”"

        public static bool DzAPI_Map_GetStoredBoolean(JPlayer whichPlayer, string key)
        {
            bool value = false;
            key = "B" + key;
            key = DzAPI_Map_GetServerValue(whichPlayer, key);
            if (key == "1")
            {
                value = true;
            }
            else
            {
                value = false;
            }


            return value;
        }


        /// title = "保存字符串变量至服务器"
        /// description = "服务器存档:存储 ${whichPlayer} 数据,名称: ${key} ,值: ${value} 最大长度63位"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”S,（如您的key是AA，实际key为SAA。【SAA用于开发者平台填写，在编辑器上获取和读都填写AA就可以了】”"

        public static void DzAPI_Map_StoreString(JPlayer whichPlayer, string key, string value)
        {
            key = "S" + key;
            DzAPI_Map_SaveServerValue(whichPlayer, key, value);


        }


        /// title = "读取服务器上的字符串变量"
        /// description = "获取 ${whichPlayer} 数据名称: ${key} 里存储的字符串"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”S”"

        public static string DzAPI_Map_GetStoredString(JPlayer whichPlayer, string key)
        {
            return DzAPI_Map_GetServerValue(whichPlayer, "S" + key);
        }


        /// title = "服务器存储字符串（区分大小写）"
        /// description = "服务器存储字符串（区分大小写） 玩家 ${player} 名称 ${key} 值 ${value}"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”S”，与普通存档共用KEY"

        public static void DzAPI_Map_StoreStringEX(JPlayer whichPlayer, string key, string value)
        {
            key = "S" + key;
            RequestExtraBooleanData(39, whichPlayer, key, value, false, 0, 0, 0);


        }


        /// title = "获取服务器存储的字符串（区分大小写）"
        /// description = "获取服务器存储的字符串（区分大小写） 玩家 ${player} 名称 ${key}"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”S”，用于开发者平台设置的防刷分存档，与普通存档共用KEY"

        public static string DzAPI_Map_GetStoredStringEX(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(38, whichPlayer, "S" + key, null, false, 0, 0, 0);
        }


        /// title = "读取服务器存储的单位类型"
        /// description = "获取 ${whichPlayer} 数据名称: ${key} 里存储的单位类型"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”I”"

        public static int DzAPI_Map_GetStoredUnitType(JPlayer whichPlayer, string key)
        {
            int.TryParse(DzAPI_Map_GetServerValue(whichPlayer, key), out int i);
            int value = 0;
            key = "I" + key;
            value = i;


            return value;
        }


        /// title = "读取服务器存储的技能类型"
        /// description = "获取 ${whichPlayer} 数据名称: ${key} 里存储的技能类型"
        /// comment = "这是经过封装的接口，实际Key会在原Key前面加”I”"

        public static int DzAPI_Map_GetStoredAbilityId(JPlayer whichPlayer, string key)
        {
            int.TryParse(DzAPI_Map_GetServerValue(whichPlayer, key), out int i);
            int value = 0;
            key = "I" + key;
            value = i;


            return value;
        }


        /// title = "清理服务器数据"
        /// description = "服务器数据：清理 ${whichPlayer} 数据,名称： ${key} "
        /// comment = "清理封装的接口记得在前面加对应的B、I、R、S"

        public static void DzAPI_Map_FlushStoredMission(JPlayer whichPlayer, string key)
        {
            DzAPI_Map_SaveServerValue(whichPlayer, key, null);


        }


        /// title = "天梯提交整数数据"
        /// description = "提交 ${whichPlayer} 天梯项目: ${key} 的值为: ${value} "
        /// comment = ""

        public static void DzAPI_Map_Ladder_SubmitIntegerData(JPlayer whichPlayer, string key, int value)
        {
            DzAPI_Map_Ladder_SetStat(whichPlayer, key, value.ToString());
        }


        /// title = "天梯提交单位类型数据"
        /// description = "提高 ${whichPlayer} 天梯项目: ${key} 的值为: ${value} "
        /// comment = ""

        public static void DzAPI_Map_Stat_SubmitUnitIdData(JPlayer whichPlayer, string key, int value)
        {
            if (value == 0)
            {
                //call DzAPI_Map_Ladder_SetStat(whichPlayer,key,"0");
            }
            else
            {
                DzAPI_Map_Ladder_SetStat(whichPlayer, key, value.ToString());
            }
        }

        public static void DzAPI_Map_Stat_SubmitUnitData(JPlayer whichPlayer, string key, JUnit value)
        {
            DzAPI_Map_Stat_SubmitUnitIdData(whichPlayer, key, JassApi.GetUnitTypeId(value));
        }


        /// title = "天梯提交技能数据"
        /// description = "提交 ${whichPlayer} 天梯项目: ${key} 的值为: ${value} "
        /// comment = ""

        public static void DzAPI_Map_Ladder_SubmitAblityIdData(JPlayer whichPlayer, string key, int value)
        {


            DzAPI_Map_Ladder_SetStat(whichPlayer, key, value.ToString());

        }


        /// title = "天梯提交物品数据"
        /// description = "提交 ${whichPlayer} 天梯项目: ${key} 的值为: ${value} "
        /// comment = ""

        public static void DzAPI_Map_Ladder_SubmitItemIdData(JPlayer whichPlayer, string key, int value)
        {
            DzAPI_Map_Ladder_SetStat(whichPlayer, key, value.ToString());


        }

        public static void DzAPI_Map_Ladder_SubmitItemData(JPlayer whichPlayer, string key, JItem value)
        {
            DzAPI_Map_Ladder_SubmitItemIdData(whichPlayer, key, JassApi.GetItemTypeId(value));
        }


        /// title = "天梯提交布尔值数据"
        /// description = "提交 ${whichPlayer} 天梯项目: ${key} 的目的 ${value} "
        /// comment = ""

        public static void DzAPI_Map_Ladder_SubmitBooleanData(JPlayer whichPlayer, string key, bool value)
        {
            if (value)
            {
                DzAPI_Map_Ladder_SetStat(whichPlayer, key, "1");
            }
            else
            {
                DzAPI_Map_Ladder_SetStat(whichPlayer, key, "0");
            }
        }


        /// title = "天梯提交获得称号"
        /// description = "提交 ${whichPlayer} 获得称号: ${key} "
        /// comment = ""

        public static void DzAPI_Map_Ladder_SubmitTitle(JPlayer whichPlayer, string value)
        {
            DzAPI_Map_Ladder_SetStat(whichPlayer, value, "1");
        }


        /// title = "天梯提交玩家排名"
        /// description = "设置 ${whichPlayer} 的游戏排名为: ${value} "
        /// comment = ""

        public static void DzAPI_Map_Ladder_SubmitPlayerRank(JPlayer whichPlayer, int value)
        {
            DzAPI_Map_Ladder_SetPlayerStat(whichPlayer, "RankIndex", value.ToString());
        }


        /// title = "天梯设置玩家额外分"
        /// description = "设置 ${whichPlayer} 的额外分为 ${value} "
        /// comment = "最多30分"

        public static void DzAPI_Map_Ladder_SubmitPlayerExtraExp(JPlayer whichPlayer, int value)
        {
            DzAPI_Map_Ladder_SetStat(whichPlayer, "ExtraExp", value.ToString());
        }


        /// title = "玩家累计游戏局数"
        /// description = "获取 ${whichPlayer} 游戏局数"
        /// comment = "获取玩家中游戏局数"

        public static int DzAPI_Map_PlayedGames(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(45, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "评论次数【废弃】"
        /// description = "获取 ${whichPlayer} 评论次数"
        /// comment = "获取玩家的评论次数，该功能已失效，始终返回1"

        public static int DzAPI_Map_CommentCount(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(46, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家好友数量【废弃】"
        /// description = "获取 ${whichPlayer} 好友数量"
        /// comment = "该功能废弃"

        public static int DzAPI_Map_FriendCount(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(47, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家是否平台认证的鉴赏家[废弃]"
        /// description = " ${whichPlayer} 是鉴赏家"
        /// comment = "评论里的鉴赏家"

        public static bool DzAPI_Map_IsConnoisseur(JPlayer whichPlayer)
        {
            return RequestExtraBooleanData(48, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家是否当前地图作者"
        /// description = " ${whichPlayer} 是本图作者"
        /// comment = "判断指定玩家是否为本地图的作者。"

        public static bool DzAPI_Map_IsAuthor(JPlayer whichPlayer)
        {
            return RequestExtraBooleanData(50, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "地图评论次数"
        /// description = "该图总评论次数"
        /// comment = "获取该图总评论次数"

        public static int DzAPI_Map_CommentTotalCount()
        {
            return RequestExtraIntegerData(51, null, null, null, false, 0, 0, 0);
        }


        /// title = "上报埋点数据"
        /// description = "上报埋点数据： ${whichPlayer} ，埋点key： ${eventKey} ，子key： ${不填} ，次数 ${value} "
        /// comment = "可以在游戏内的关键行为操作进行埋点，以便进行游戏内的玩家行为数据统计分析（比如某个英雄选择次数），上报前需先在开发者平台创建埋点。"

        public static void DzAPI_Map_Statistics(JPlayer whichPlayer, string eventKey, string eventType, int value)
        {
            RequestExtraBooleanData(34, whichPlayer, eventKey, eventType, false, value, 0, 0);
        }


        /// title = "是否回流/收藏过地图的用户"
        /// description = " ${whichPlayer}  ${label} "
        /// comment = "超过7天未玩地图的用户再次登录被称为地图回流用户，地图回流BUFF会存在7天，7天后消失。平台回流用户的BUFF存在15天，15天后消失。建议设置奖励，鼓励玩家回来玩地图！"

        public static bool DzAPI_Map_Returns(JPlayer whichPlayer, int label)
        {
            return RequestExtraBooleanData(53, whichPlayer, null, null, false, label, 0, 0);
        }


        /// title = "玩家签到天数"
        /// description = " ${whichPlayer}  ${label} "
        /// comment = "获取玩家在指定地图的地图签到数据。"

        public static int DzAPI_Map_ContinuousCount(JPlayer whichPlayer, int id)
        {
            return RequestExtraIntegerData(54, whichPlayer, null, null, false, id, 0, 0);
        }


        /// title = "玩家是否为真实玩家"
        /// description = " ${whichPlayer} 是否为真实玩家"
        /// comment = "当作者开启匹配模式的虚拟电脑玩家(AI)补位功能后，可通过此接口判定是否真实玩家。"

        public static bool DzAPI_Map_IsPlayer(JPlayer whichPlayer)
        {
            return RequestExtraBooleanData(55, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家累计游戏时长"
        /// description = " ${whichPlayer} 累计游戏时长"
        /// comment = "获取玩家在当前地图的累计游戏时长"

        public static int DzAPI_Map_MapsTotalPlayed(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(56, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家在指定地图的地图等级"
        /// description = " ${whichPlayer} 在地图: ${mapId} 的地图等级"
        /// comment = "获取玩家在指定地图的地图等级。"

        public static int DzAPI_Map_MapsLevel(JPlayer whichPlayer, int mapId)
        {
            return RequestExtraIntegerData(57, whichPlayer, null, null, false, mapId, 0, 0);
        }


        /// title = "玩家在指定地图累计消耗平台金币【已废弃】"
        /// description = " ${whichPlayer} 在地图： ${mapId} 的平台金币消耗"
        /// comment = "获取玩家在指定地图的累计消耗平台金币数量。"

        public static int DzAPI_Map_MapsConsumeGold(JPlayer whichPlayer, int mapId)
        {
            return RequestExtraIntegerData(58, whichPlayer, null, null, false, mapId, 0, 0);
        }


        /// title = "玩家在指定地图的平台木材消耗【已废弃】"
        /// description = " ${whichPlayer} 地图： ${mapId} 的平台木材消耗"
        /// comment = ""

        public static int DzAPI_Map_MapsConsumeLumber(JPlayer whichPlayer, int mapId)
        {
            return RequestExtraIntegerData(59, whichPlayer, null, null, false, mapId, 0, 0);
        }


        /// title = "玩家在指定地图累计消费金额区间（1~199）"
        /// description = " ${whichPlayer} 在地图 ${地图id} 消费在（1~199）区间"
        /// comment = "检测消费是否在（1~199）区间"

        public static bool DzAPI_Map_MapsConsumeLv1(JPlayer whichPlayer, int mapId)
        {
            return RequestExtraBooleanData(60, whichPlayer, null, null, false, mapId, 0, 0);
        }


        /// title = "玩家在指定地图累计消费金额区间（200~499）"
        /// description = " ${whichPlayer} 在地图 ${地图id} 消费在（200~499）区间"
        /// comment = "检测消费是否在（200~499）区间"

        public static bool DzAPI_Map_MapsConsumeLv2(JPlayer whichPlayer, int mapId)
        {
            return RequestExtraBooleanData(61, whichPlayer, null, null, false, mapId, 0, 0);
        }


        /// title = "玩家在指定地图累计消费金额区间（500~999）"
        /// description = " ${whichPlayer} 在地图 ${地图id} 消费在（500~999）区间"
        /// comment = "检测消费是否在（500~999）区间"

        public static bool DzAPI_Map_MapsConsumeLv3(JPlayer whichPlayer, int mapId)
        {
            return RequestExtraBooleanData(62, whichPlayer, null, null, false, mapId, 0, 0);
        }


        /// title = "玩家在指定地图累计消费金额区间（1000+）"
        /// description = " ${whichPlayer} 在地图 ${地图id} 消费在（1000+）区间"
        /// comment = "检测消费是否在（1000+）区间"

        public static bool DzAPI_Map_MapsConsumeLv4(JPlayer whichPlayer, int mapId)
        {
            return RequestExtraBooleanData(63, whichPlayer, null, null, false, mapId, 0, 0);
        }


        /// title = "玩家是否装备指定平台装饰"
        /// description = " ${whichPlayer} 装备了 ${skinType} 的 ${id} 道具"
        /// comment = "检查玩家是否装备着指定平台装饰（仅限平台和地图的合作装饰）。"

        public static bool DzAPI_Map_IsPlayerUsingSkin(JPlayer whichPlayer, int skinType, int id)
        {
            return RequestExtraBooleanData(64, whichPlayer, null, null, false, skinType, id, 0);
        }


        /// title = "玩家在地图社区上的互动数据"
        /// description = " ${whichPlayer}  ${whichData} "
        /// comment = "“获取玩家在当前地图的社区内的行为统计数据及身份数据。"

        public static int DzAPI_Map_GetForumData(JPlayer whichPlayer, int whichData)
        {
            return RequestExtraIntegerData(65, whichPlayer, null, null, false, whichData, 0, 0);
        }


        /// title = "玩家标记"
        /// description = " ${whichPlayer} 是 ${label} "
        /// comment = "获取玩家在当前地图上的身份标记（当前是否回流用户、是否收藏地图）。"

        public static bool DzAPI_Map_PlayerFlags(JPlayer whichPlayer, int label)
        {
            return RequestExtraBooleanData(53, whichPlayer, null, null, false, label, 0, 0);
        }


        /// title = "玩家抽取指定地图宝箱次数"
        /// description = "获取 ${whichPlayer} 第 ${n} 个地图宝箱的累计抽取次数"
        /// comment = "第二个参数为0代表第一个宝箱也为默认宝箱，为1代表第二个宝箱"

        public static int DzAPI_Map_GetLotteryUsedCountEx(JPlayer whichPlayer, int index)
        {
            return RequestExtraIntegerData(68, whichPlayer, null, null, false, index, 0, 0);
        }


        /// title = "玩家抽取地图宝箱总次数"
        /// description = " ${whichPlayer} 玩家抽取地图宝箱总次数"
        /// comment = ""

        public static int DzAPI_Map_GetLotteryUsedCount(JPlayer whichPlayer)
        {
            return DzAPI_Map_GetLotteryUsedCountEx(whichPlayer, 0) + DzAPI_Map_GetLotteryUsedCountEx(whichPlayer, 1) + DzAPI_Map_GetLotteryUsedCountEx(whichPlayer, 2);
        }


        /// title = "打开地图商城道具购买界面"
        /// description = " ${whichPlayer} 打开地图商城道具 ${道具key} 购买界面"
        /// comment = "打开游戏内置商城的道具购买页面，用于作者在地图内开发引导消费场景。购买成功后可通过玩家获得平台道具事件实现在游戏内立即生效。"

        public static bool DzAPI_Map_OpenMall(JPlayer whichPlayer, string whichkey)
        {
            return RequestExtraBooleanData(66, whichPlayer, whichkey, null, false, 0, 0, 0);
        }


        /// title = "上报本局游戏玩家数据"
        /// description = "上报本局游戏： ${whichPlayer} 项目： ${key} 数据： ${value} "
        /// comment = "上报本局游戏的玩家数据，比如战斗力、杀敌数等。"

        public static void DzAPI_Map_GameResult_CommitData(JPlayer whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(69, whichPlayer, key, value, false, 0, 0, 0);
        }


        /// title = "上报本局游戏玩家称号"
        /// description = "上报本局游戏： ${whichPlayer} 称号： ${key} "
        /// comment = "上报本局游戏玩家所获得的称号，请注意**称号Key**不能和[上报本局游戏玩家数据]的**数据项Key**重复。"

        public static void DzAPI_Map_GameResult_CommitTitle(JPlayer whichPlayer, string value)
        {
            DzAPI_Map_GameResult_CommitData(whichPlayer, value, "1");


        }


        /// title = "上报本局游戏玩家排名"
        /// description = "上报本局游戏 ${whichPlayer} 的排名为： ${value} "
        /// comment = "对于乱斗模式的地图，上报每一名玩家的名次。"

        public static void DzAPI_Map_GameResult_CommitPlayerRank(JPlayer whichPlayer, int value)
        {
            DzAPI_Map_GameResult_CommitData(whichPlayer, "RankIndex", value.ToString());

            value = 0;
        }


        /// title = "上报本局游戏模式"
        /// description = "上报本局游戏模式： ${模式名} "
        /// comment = "上报本局游戏所选择的地图模式名称。"

        public static void DzAPI_Map_GameResult_CommitGameMode(string value)
        {
            DzAPI_Map_GameResult_CommitData(JassApi.GetLocalPlayer(), "InnerGameMode", value);

        }


        /// title = "上报本局游戏结果"
        /// description = "上报本局游戏结果： ${whichPlayer}  ${value} "
        /// comment = "上报本局游戏玩家游戏结果（胜负），提交后会立即结束游戏。"

        public static void DzAPI_Map_GameResult_CommitGameResult(JPlayer whichPlayer, int value)
        {
            DzAPI_Map_GameResult_CommitData(whichPlayer, "GameResult", value.ToString());

        }


        /// title = "上报本局游戏结果（不结束游戏）"
        /// description = "上报本局游戏结果： ${whichPlayer}  ${value} [不结束游戏]"
        /// comment = "上报本局游戏玩家游戏结果（胜负），提交后不会立即结束游戏，适用于游戏正常结束后还有奖励关的地图。"

        public static void DzAPI_Map_GameResult_CommitGameResultNoEnd(JPlayer whichPlayer, int value)
        {
            DzAPI_Map_GameResult_CommitData(whichPlayer, "GameResultNoEnd", value.ToString());

        }


        /// title = "玩家本局游戏距上一局游戏的时间差"
        /// description = " ${whichPlayer} 本局游戏距上一局游戏的时间差"
        /// comment = "返查询该玩家上次玩游戏时间至本次玩游戏时间的差值，可以利用此接口实现离线收益之类的功能。"

        public static int DzAPI_Map_GetSinceLastPlayedSeconds(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(70, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "使用U币快速购买地图商城道具"
        /// description = "向 ${whichPlayer} 弹出商品 ${key} 的购买窗口，购买数量 ${数量} 窗口持续时间 ${时间} 秒"
        /// comment = "弹出提示框询问玩家是否使用U币直接购买指定道具，作者需已在商城上架对应商品（商品信息中的**道具和数量**与接口所请求的参数一致）。如果前一次购买的提示框未关闭的情况下再次调用此接口，后续请求无效。购买成功后可通过[玩家获得平台道具事件]实现在游戏内立即生效。"

        public static bool DzAPI_Map_QuickBuy(JPlayer whichPlayer, string key, int count, int seconds)
        {
            return RequestExtraBooleanData(72, whichPlayer, key, null, false, count, seconds, 0);
        }


        /// title = "关闭U币快速购买界面"
        /// description = "关闭 ${whichPlayer} U币快速购买窗口"
        /// comment = "关闭最后一次打开的U币快速购买窗口，结合打开U币快速购买窗口使用。"

        public static bool DzAPI_Map_CancelQuickBuy(JPlayer whichPlayer)
        {
            return RequestExtraBooleanData(73, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "本局游戏是否处于平台自测服"
        /// description = "本局游戏是否处于平台自测服"
        /// comment = "获取当前游戏局所处的平台环境。"

        public static bool DzAPI_Map_IsMapTest()
        {
            return RequestExtraBooleanData(74, null, null, null, false, 0, 0, 0);
        }


        /// title = "玩家地图商城道具是否读取成功"
        /// description = " ${whichPlayer} 地图商城道具是否读取成功"
        /// comment = "判断本局游戏中玩家的商城道具是否正确加载。"

        public static bool DzAPI_Map_PlayerLoadedItems(JPlayer whichPlayer)
        {
            return RequestExtraBooleanData(77, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "自定义排行榜上榜人数"
        /// description = "获取自定义排行榜 ${key} 的上榜人数"
        /// comment = "key为开发者平台所配置的自定义排行榜key值,值为1~9"

        public static int DzAPI_Map_CustomRankCount(int id)
        {
            return RequestExtraIntegerData(78, null, null, null, false, id, 0, 0);
        }


        /// title = "自定义排行榜上的玩家昵称"
        /// description = "获取自定义排行榜 ${key} 的排名第 ${ranking} 的玩家昵称"
        /// comment = "key为开发者平台所配置的自定义排行榜key值,值为1~9"

        public static string DzAPI_Map_CustomRankPlayerName(int id, int ranking)
        {
            return RequestExtraStringData(79, null, null, null, false, id, ranking, 0);
        }


        /// title = "自定义排行榜上的玩家数值"
        /// description = "获取自定义排行榜 ${key} 排名第 ${ranking} 的数值"
        /// comment = "key为开发者平台所配置的自定义排行榜key值,值为1~9"

        public static int DzAPI_Map_CustomRankValue(int id, int ranking)
        {
            return RequestExtraIntegerData(80, null, null, null, false, id, ranking, 0);
        }


        /// title = "玩家在KK对战平台的完整昵称"
        /// description = " ${whichPlayer} 在KK对战平台的完整昵称"
        /// comment = "获取玩家的KK平台完整昵称“基础昵称#编号”"

        public static string DzAPI_Map_GetPlayerUserName(JPlayer whichPlayer)
        {
            return RequestExtraStringData(81, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "获取服务器存档限制余额"
        /// description = "获取 ${whichPlayer} 存档 ${key} 上限余额"
        /// comment = "获取服务器存档当天上限余额，需要在开发者平台对指定KEY设置每天上限，获取的值为余额。如存档A上限为100，当天使用了80，返回20"

        public static int KKApiGetServerValueLimitLeft(JPlayer whichPlayer, string key)
        {
            return RequestExtraIntegerData(82, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "【随机只读存档】生成随机数"
        /// description = "设置 ${whichPlayer} 对随机只读存档 ${key} 的组 ${groupkey} 生成随机数"
        /// comment = "生成一个服务器随机数并关联组ID，可以在开发者平台对组ID进行防刷分管理，同组ID下各个Key共享CD和次数。"

        public static bool KKApiRequestBackendLogic(JPlayer whichPlayer, string key, string groupkey)
        {
            return RequestExtraBooleanData(83, whichPlayer, key, groupkey, false, 0, 0, 0);
        }


        /// title = "【随机只读存档】判断随机数是否存在"
        /// description = " ${whichPlayer} 随机只读存档 ${key} 是否存在"
        /// comment = "判断指定KEY生成的随机数是否存在，存在返回true"

        public static bool KKApiCheckBackendLogicExists(JPlayer whichPlayer, string key)
        {
            return RequestExtraBooleanData(84, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "【随机只读存档】读取随机数的值"
        /// description = "获取 ${whichPlayer} 随机只读存档 ${key} 的值"
        /// comment = "读取指定KEY生成的服务器随机数的值，返回整数。"

        public static int KKApiGetBackendLogicIntResult(JPlayer whichPlayer, string key)
        {
            return RequestExtraIntegerData(85, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "【随机只读存档】读取随机数的值"
        /// description = "获取 ${whichPlayer} 随机只读存档 ${key} 的值"
        /// comment = "读取指定KEY生成的服务器随机数的值"

        public static string KKApiGetBackendLogicStrResult(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(86, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "【随机只读存档】读取随机数的生成时间"
        /// description = "获取 ${whichPlayer} 随机只读存档 ${key} 的生成时间"
        /// comment = "读取指定KEY生成的服务器随机数生成的时间戳，返回整数。"

        public static int KKApiGetBackendLogicUpdateTime(JPlayer whichPlayer, string key)
        {
            return RequestExtraIntegerData(87, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "【随机只读存档】读取随机数的组ID"
        /// description = "获取 ${whichPlayer} 随机只读存档 ${key} 的组ID"
        /// comment = "读取指定KEY生成的服务器随机数生成的组ID，返回整数"

        public static string KKApiGetBackendLogicGroup(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(88, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "【随机只读存档】删除随机数"
        /// description = "删除 ${whichPlayer} 随机只读存档 ${key} 的随机数"
        /// comment = "删除指定KEY生成的服务器生成的随机数"

        public static bool KKApiRemoveBackendLogicResult(JPlayer whichPlayer, string key)
        {
            return RequestExtraBooleanData(89, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "【随机只读存档】剩余次数"
        /// description = " ${whichPlayer} 随机只读存档的组 ${groupkey} 今日的剩余次数"
        /// comment = ""

        public static int KKApiRandomSaveGameCount(JPlayer whichPlayer, string groupkey)
        {
            return RequestExtraIntegerData(101, whichPlayer, groupkey, null, false, 0, 0, 0);
        }


        /// title = "注册随机存档更新事件"
        /// description = "注册随机存档更新事件"
        /// comment = "当玩家随机存档更新的时候触发该事件。用'当前变动的随机存档'来获取变动的随机存档key。"

        public static void KKApiTriggerRegisterBackendLogicUpdata(JTrigger trig)
        {
            DzApi.DzTriggerRegisterSyncData(trig, "DZBLU", true);
        }


        /// title = "注册随机存档删除事件"
        /// description = "注册随机存档删除事件"
        /// comment = "当玩家随机存档删除的时候触发该事件。用'当前变动的随机存档”来获取变动的随机存档key"

        public static void KKApiTriggerRegisterBackendLogicDelete(JTrigger trig)
        {
            DzApi.DzTriggerRegisterSyncData(trig, "DZBLD", true);
        }


        /// title = "获取变动的随机存档"
        /// description = "获取变动的随机存档"
        /// comment = "用在注册随机存档更新和删除事件之后"

        public static string KKApiGetSyncBackendLogic()
        {
            return DzApi.DzGetTriggerSyncData();
        }


        /// title = "是否在平台正常游戏中"
        /// description = "是否在平台正常游戏中"
        /// comment = "主要试用于平台运行中区分正常游戏和观战模式，返回true代表是正常游戏模式，反之为观战模式"

        public static bool KKApiIsGameMode()
        {
            return RequestExtraBooleanData(90, null, null, null, false, 0, 0, 0);
        }


        /// title = "初始化平台键位显示设置"
        /// description = "设置 ${whichPlayer} 的第 ${n} 套方案的键位： ${key} 设置描述： ${描述} "
        /// comment = "初始化键位设置会显示在平台改键界面上，最多2套方案。"

        public static bool KKApiInitializeGameKey(JPlayer whichPlayer, int setIndex, string k, string data)
        {
            return RequestExtraBooleanData(91, whichPlayer, "[{\"name\":\"" + data + "\",\"key\":\"" + k + "\"}]", null, false, setIndex, 0, 0);
        }


        /// title = "获取玩家的平台ID"
        /// description = " ${whichPlayer} 平台ID"
        /// comment = "返回的是一个32位的字符串"

        public static string KKApiPlayerGUID(JPlayer whichPlayer)
        {
            return RequestExtraStringData(93, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "玩家地图任务状态"
        /// description = " ${whichPlayer} 地图任务： ${setIndex} 在 ${taskstat} 状态"
        /// comment = ""

        public static bool KKApiIsTaskInProgress(JPlayer whichPlayer, int setIndex, int taskstat)
        {
            return RequestExtraIntegerData(94, whichPlayer, null, null, false, setIndex, 0, 0) == taskstat;
        }


        /// title = "玩家地图任务当前进度"
        /// description = " ${whichPlayer} 地图任务： ${setIndex} 的当前进度"
        /// comment = ""

        public static int KKApiQueryTaskCurrentProgress(JPlayer whichPlayer, int setIndex)
        {
            return RequestExtraIntegerData(95, whichPlayer, null, null, false, setIndex, 0, 0);
        }


        /// title = "玩家地图任务总进度"
        /// description = " ${whichPlayer} 地图任务： ${setIndex} 总进度"
        /// comment = ""

        public static int KKApiQueryTaskTotalProgress(JPlayer whichPlayer, int setIndex)
        {
            return RequestExtraIntegerData(96, whichPlayer, null, null, false, setIndex, 0, 0);
        }


        /// title = "玩家平台该地图成就是否完成"
        /// description = " ${whichPlayer} 平台该地图成就： ${key} 已经完成"
        /// comment = "完成返回true"

        public static bool KKApiIsAchievementCompleted(JPlayer whichPlayer, string id)
        {
            return RequestExtraBooleanData(98, whichPlayer, id, null, false, 0, 0, 0);
        }


        /// title = "玩家平台该地图成就点数"
        /// description = " ${whichPlayer} 平台该地图成就点数"
        /// comment = ""

        public static int KKApiAchievementPoints(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(99, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "判定测试大厅游戏时长区间"
        /// description = " ${whichPlayer} 在测试大厅的游戏时长是否在区间（ ${minHours} 到 ${maxHours} ）小时"
        /// comment = "判断测试大厅游戏时长是否满足该区间 ，0表示不限制，单位为小时"

        public static bool KKApiPlayedTime(JPlayer whichPlayer, int minHours, int maxHours)
        {
            return RequestExtraBooleanData(100, whichPlayer, null, null, false, minHours, maxHours, 0);
        }


        /// title = "【批量存档】开始保存"
        /// description = " ${whichPlayer} 开始批量保存存档"
        /// comment = "对添加批量保存存档条目进行保存。"

        public static bool KKApiBeginBatchSaveArchive(JPlayer whichPlayer)
        {
            return RequestExtraBooleanData(102, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "【批量存档】添加条目"
        /// description = "设置 ${whichPlayer} 批量存档添加条目 ${key} ，数据： ${value} ， ${caseInsensitive} 区分大小写"
        /// comment = "对添加批量保存存档条目进行保存。"

        public static bool KKApiAddBatchSaveArchive(JPlayer whichPlayer, string key, string value, bool caseInsensitive)
        {
            return RequestExtraBooleanData(103, whichPlayer, key, value, caseInsensitive, 0, 0, 0);
        }


        /// title = "【批量存档】结束保存"
        /// description = "设置 ${whichPlayer}  ${abandon} 结束批量保存存档"
        /// comment = "结束批量保存存档。"

        public static bool KKApiEndBatchSaveArchive(JPlayer whichPlayer, bool abandon)
        {
            return RequestExtraBooleanData(104, whichPlayer, null, null, abandon, 0, 0, 0);
        }


        /// title = "【批量存档】添加条目-整数"
        /// description = "设置 ${whichPlayer} 批量存档添加条目 ${key} ，数据： ${value}"
        /// comment = "对添加批量保存存档条目进行保存。KEY不区分大小写"

        public static void KKApiAddBatchSaveArchiveInteger(JPlayer whichPlayer, string key, int value)
        {
            key = "I" + key;
            KKApiAddBatchSaveArchive(whichPlayer, key, value.ToString(), false);


        }


        /// title = "【批量存档】添加条目-实数"
        /// description = "设置 ${whichPlayer} 批量存档添加条目 ${key} ，数据： ${value}"
        /// comment = "对添加批量保存存档条目进行保存。KEY不区分大小写"

        public static void KKApiAddBatchSaveArchiveReal(JPlayer whichPlayer, string key, float value)
        {
            key = "R" + key;
            KKApiAddBatchSaveArchive(whichPlayer, key, JassApi.R2S(value), false);


        }


        /// title = "【批量存档】添加条目-布尔值"
        /// description = "设置 ${whichPlayer} 批量存档添加条目 ${key} ，数据： ${value}"
        /// comment = "对添加批量保存存档条目进行保存。KEY不区分大小写"

        public static void KKApiAddBatchSaveArchiveBoolean(JPlayer whichPlayer, string key, bool value)
        {
            key = "B" + key;
            if (value)
            {
                KKApiAddBatchSaveArchive(whichPlayer, key, "1", false);
            }
            else
            {
                KKApiAddBatchSaveArchive(whichPlayer, key, "0", false);
            }


        }


        /// title = "【批量存档】添加条目-字符串"
        /// description = "设置 ${whichPlayer} 批量存档添加条目 ${key} ，数据： ${value}"
        /// comment = "对添加批量保存存档条目进行保存。KEY不区分大小写"

        public static void KKApiAddBatchSaveArchiveString(JPlayer whichPlayer, string key, string value)
        {
            key = "S" + key;
            KKApiAddBatchSaveArchive(whichPlayer, key, value, false);


        }


        /// title = "注册天梯投降事件"
        /// description = "注册天梯投降事件"
        /// comment = "当玩家在天梯投降时候触发该事件。用'获取投降的队伍id'来获取。"

        public static void KKApiTriggerRegisterLadderSurrender(JTrigger trig)
        {
            DzApi.DzTriggerRegisterSyncData(trig, "DZSR", true);
        }


        /// title = "获取天梯投降的队伍ID"
        /// description = "获取天梯投降的队伍ID"
        /// comment = "用于天梯投降事件动作里"

        public static int KKApiGetLadderSurrenderTeamId()
        {
            int.TryParse(DzApi.DzGetTriggerSyncData(), out int i);
            return i;
        }


        /// title = "玩家在公会的等级"
        /// description = "获取 ${whichPlayer} 公会等级"
        /// comment = "获取玩家公会等级"

        public static int KKApiGetGuildLevel(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(106, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "宠物探险次数"
        /// description = " ${whichPlayer} 宠物探险次数"
        /// comment = "获取平台宠物探险次数"

        public static int KKApiMapExplorationNum(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(107, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "宠物探险时间"
        /// description = " ${whichPlayer} 宠物探险时间"
        /// comment = "获取平台宠物探险时间"

        public static int KKApiMapExplorationTime(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(108, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "测试大厅预约人数"
        /// description = "测试大厅预约人数"
        /// comment = ""

        public static int KKApiMapOrderNum()
        {
            return RequestExtraIntegerData(109, null, null, null, false, 0, 0, 0);
        }


        /// title = "发送云脚本数据"
        /// description = "发送云脚本数据 ${玩家}  ${事件}  ${数据} "
        /// comment = "发送云脚本数据"

        public static bool KKApiMlScriptEvent(JPlayer whichPlayer, string eventName, string payload)
        {
            return RequestExtraBooleanData(1009, whichPlayer, eventName, payload, false, 0, 0, 0);
        }


        /// title = "事件响应 - 商城道具最后变动的数量"
        /// description = " ${whichPlayer} 商城道具： ${key} 最后更新的数量"
        /// comment = "获取的是当次玩家商城背包新增或消耗的数量，如果是时效型道具获取的是剩余时间，可以用于【玩家获取商城道具事件】、【玩家消耗使用商城道具事件】和【玩家删除商城道具事件】后。"

        public static int KKApiGetMallItemUpdateCount(JPlayer whichPlayer, string key)
        {
            return RequestExtraIntegerData(110, whichPlayer, key, null, false, 0, 0, 0);
        }


        /// title = "获取地图版本号[new]"
        /// description = "获取地图版本号"
        /// comment = ""

        public static string KKApiGetMapVersion()
        {
            return RequestExtraStringData(111, null, null, null, false, 0, 0, 0);
        }


        /// title = "获取赛事模式[new]"
        /// description = "获取赛事模式"
        /// comment = ""

        public static string KKApiGetCompetitionGameMode()
        {
            return RequestExtraStringData(112, null, null, null, false, 0, 0, 0);
        }


        /// title = "获取玩家当天总游戏局数[new]"
        /// description = "获取 ${whichPlayer} 当天总游戏局数"
        /// comment = "为当天玩家玩该地图的有效局数，10分钟算一局，每天05:00刷新"

        public static int KKApiDayRounds(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(113, whichPlayer, null, null, false, 0, 0, 0);
        }


        /// title = "获取玩家在指定地图会员等级[new]"
        /// description = "获取 ${whichPlayer} 在地图 ${mapId} 的会员等级"
        /// comment = ""

        public static int KKApiConsumeLevel(JPlayer whichPlayer, int mapId)
        {
            return RequestExtraIntegerData(115, whichPlayer, null, null, false, mapId, 0, 0);
        }


        /// title = "判断玩家当前地图在游戏大厅置顶状态[new]"
        /// description = " ${whichPlayer} 当前地图在游戏大厅置顶状态"
        /// comment = "玩家在游戏大厅首页置顶该地图后返回true"

        public static bool KKApiIsPinned(JPlayer whichPlayer)
        {
            return RequestExtraBooleanData(117, whichPlayer, null, null, false, 0, 0, 0);
        }



        /// title = "当前选择的单位(异步)"        /// description = "获取主控单位"        /// comment = "获取的单位是异步的，请谨慎操作"
        public static JUnit DzGetSelectedLeaderUnit()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetSelectedLeaderUnit"));
            return new(handle);
        }


        /// title = "聊天框是否打开"
        /// description = "获取聊天框是否打开"
        /// comment = ""

        public static bool DzIsChatBoxOpen()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsChatBoxOpen"));
        }


        /// title = "设置单位的鼠标指向UI和血条显示/隐藏"
        /// description = "设置 ${单位} 的鼠标指向UI和血条 ${显示}"
        /// comment = ""

        public static void DzSetUnitPreselectUIVisible(JUnit whichUnit, bool visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitPreselectUIVisible"), whichUnit.Handle, visible);
        }


        /// title = "设置特效播放动画"
        /// description = "设置 ${特效} 播放第 ${编号} 号动画，播放方式${方式} "
        /// comment = ""

        public static void DzSetEffectAnimation(JEffect whichEffect, int index, int flag)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectAnimation"), whichEffect.Handle, index, flag);
        }


        /// title = "设置特效坐标"
        /// description = "设置 ${特效} 的坐标（ ${x},${y},${z}）"
        /// comment = ""

        public static void DzSetEffectPos(JEffect whichEffect, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectPos"), whichEffect.Handle, x, y, z);
        }


        /// title = "设置特效颜色"
        /// description = "设置 ${特效} 的颜色：${color} "
        /// comment = ""

        public static void DzSetEffectVertexColor(JEffect whichEffect, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectVertexColor"), whichEffect.Handle, color);
        }


        /// title = "设置特效透明度"
        /// description = "设置 ${特效} 的透明度：${alpha} "
        /// comment = ""

        public static void DzSetEffectVertexAlpha(JEffect whichEffect, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectVertexAlpha"), whichEffect.Handle, alpha);
        }


        /// title = "设置特效模型"
        /// description = "设置 ${特效} 的模型： ${modelFile}"
        /// comment = ""

        public static void DzSetEffectModel(JEffect whichEffect, string model)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectModel"), whichEffect.Handle, model);
        }


        /// title = "设置特效队伍颜色"
        /// description = "设置 ${特效} 的队伍颜色： ${Color}"
        /// comment = ""

        public static void DzSetEffectTeamColor(JEffect whichHandle, int playerId)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectTeamColor"), whichHandle.Handle, playerId);
        }


        /// title = "设置控件视口"
        /// description = "设置 ${frame} 的视口为 ${状态}"
        /// comment = "设置控件视口后，他的子控件在边缘超出部分不会显示"

        public static void DzFrameSetClip(int whichframe, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetClip"), whichframe, enable);
        }


        /// title = "设置魔兽窗口大小"
        /// description = "设置魔兽窗口大小：${宽}/${高}"
        /// comment = "如修改窗口模式下的窗口大小为 1920/1080"

        public static bool DzChangeWindowSize(int width, int height)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzChangeWindowSize"), width, height);
        }


        /// title = "设置特效播放动画"
        /// description = "设置 ${特效} 播放 ${death} 动画，附加动画链接名 ${alternate}"
        /// comment = "变身动画才需要附加链接名 一般情况填 “” 空字符串就行"

        public static void DzPlayEffectAnimation(JEffect whichEffect, string anim, string link)
        {
            War3.CallNative(War3.GetNativeFunction("DzPlayEffectAnimation"), whichEffect.Handle, anim, link);
        }


        /// title = "绑定特效"
        /// description = "在 ${单位} 附着点 ${origin} 绑定特效 ${特效}"
        /// comment = ""

        public static void DzBindEffect(JWidget parent, string attachPoint, JEffect whichEffect)
        {
            War3.CallNative(War3.GetNativeFunction("DzBindEffect"), parent.Handle, attachPoint, whichEffect.Handle);
        }


        /// title = "解除绑定特效"
        /// description = "解除绑定的特效 ${特效}"
        /// comment = "可以让绑定在单位身上的特效分离出来，被分离的特效能设置坐标、缩放"

        public static void DzUnbindEffect(JEffect whichEffect)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnbindEffect"), whichEffect.Handle);
        }


        /// title = "单位缩放"
        /// description = "设置 ${单位} 的缩放为 ${scale} "
        /// comment = "可以用来缩放单位"

        public static void DzSetWidgetSpriteScale(JWidget whichUnit, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetWidgetSpriteScale"), whichUnit.Handle, scale);
        }


        /// title = "特效缩放"
        /// description = "设置 ${特效} 的缩放为 ${scale} "
        /// comment = "可以用来缩放特效"

        public static void DzSetEffectScale(JEffect whichHandle, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectScale"), whichHandle.Handle, scale);
        }


        /// title = "获取特效颜色"
        /// description = "获取 ${特效} 的颜色"
        /// comment = ""

        public static int DzGetEffectVertexColor(JEffect whichEffect)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetEffectVertexColor"), whichEffect.Handle);
        }


        /// title = "获取特效透明度"
        /// description = "获取 ${特效} 的透明度"
        /// comment = ""

        public static int DzGetEffectVertexAlpha(JEffect whichEffect)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetEffectVertexAlpha"), whichEffect.Handle);
        }


        /// title = "获取物品技能"
        /// description = "获取 ${物品} 的第 ${编号} 个技能"
        /// comment = ""

        public static JAbility DzGetItemAbility(JItem whichEffect, int index)
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetItemAbility"), whichEffect.Handle, index);
            return new(handle);
        }


        /// title = "获取子控件数量"
        /// description = "获取 ${界面} 的子控件数量"
        /// comment = ""

        public static int DzFrameGetChildrenCount(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetChildrenCount"), whichframe);
        }


        /// title = "获取子控件"
        /// description = "获取 ${界面} 的第 ${编号} 个子控件"
        /// comment = ""

        public static int DzFrameGetChild(int whichframe, int index)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetChild"), whichframe, index);
        }


        /// title = "解锁BLP像素限制"
        /// description = "解锁BLP像素限制：${true}"
        /// comment = "填true会解除原本魔兽高清图片的512像素限制"

        public static void DzUnlockBlpSizeLimit(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnlockBlpSizeLimit"), enable);
        }


        /// title = "获取商店目标"
        /// description = "获取 ${商店} 选中的 ${玩家} 单位"
        /// comment = ""

        public static JUnit DzGetActivePatron(JUnit store, JPlayer p)
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetActivePatron"), store.Handle, p.Handle);
            return new(handle);
        }


        /// title = "获取玩家选中的单位数量"
        /// description = "获取玩家选中的单位数量"
        /// comment = "返回值是异步的，谨慎使用"

        public static int DzGetLocalSelectUnitCount()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetLocalSelectUnitCount"));
        }


        /// title = "获取玩家选中的单位"
        /// description = "获取玩家选中的第 ${编号} 个单位"
        /// comment = "返回值是异步的，谨慎使用"

        public static JUnit DzGetLocalSelectUnit(int index)
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetLocalSelectUnit"), index);
            return new(handle);
        }


        /// title = "获取字符串数量"
        /// description = "获取字符串数量"
        /// comment = ""

        public static int DzGetJassStringTableCount()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetJassStringTableCount"));
        }


        /// title = "清除模型内存缓存"
        /// description = "清除模型 ${模型} 内存缓存"
        /// comment = ""

        public static void DzModelRemoveFromCache(string path)
        {
            War3.CallNative(War3.GetNativeFunction("DzModelRemoveFromCache"), path);
        }


        /// title = "清除所有模型内存缓存"
        /// description = "清除所有模型内存缓存"
        /// comment = ""

        public static void DzModelRemoveAllFromCache()
        {
            War3.CallNative(War3.GetNativeFunction("DzModelRemoveAllFromCache"));
        }


        /// title = "获取框选控件"
        /// description = "获取第 ${编号} 个框选控件"
        /// comment = "鼠标框选单位后，控制面板的队列单位头像控件"

        public static int DzFrameGetInfoPanelSelectButton(int index)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetInfoPanelSelectButton"), index);
        }


        /// title = "获取BUFF控件"
        /// description = "获取第 ${编号} 个BUFF控件"
        /// comment = "控制面板的单位属性下面的魔法效果buff控件"

        public static int DzFrameGetInfoPanelBuffButton(int index)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetInfoPanelBuffButton"), index);
        }


        /// title = "获取农民控件"
        /// description = "获取农民控件"
        /// comment = ""

        public static int DzFrameGetPeonBar()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetPeonBar"));
        }


        /// title = "获取技能右下角数字文本控件"
        /// description = "获取 ${技能控件} 右下角数字文本控件"
        /// comment = ""

        public static int DzFrameGetCommandBarButtonNumberText(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButtonNumberText"), whichframe);
        }


        /// title = "获取技能右下角数字文本框体"
        /// description = "获取 ${技能控件} 右下角数字文本框体"
        /// comment = ""

        public static int DzFrameGetCommandBarButtonNumberOverlay(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButtonNumberOverlay"), whichframe);
        }


        /// title = "获取技能冷却指示器"
        /// description = "获取 ${技能控件} 冷却指示器"
        /// comment = "控件类型为sprite frame"

        public static int DzFrameGetCommandBarButtonCooldownIndicator(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButtonCooldownIndicator"), whichframe);
        }


        /// title = "获取技能自动施法指示器"
        /// description = "获取 ${技能控件} 自动施法指示器"
        /// comment = "控件类型为sprite frame"

        public static int DzFrameGetCommandBarButtonAutoCastIndicator(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButtonAutoCastIndicator"), whichframe);
        }


        /// title = "设置FPS显示/隐藏"
        /// description = "设置 FPS ${显示/隐藏}"
        /// comment = ""

        public static void DzToggleFPS(bool show)
        {
            War3.CallNative(War3.GetNativeFunction("DzToggleFPS"), show);
        }


        /// title = "获取 FPS 帧数"
        /// description = "获取 FPS 帧数"
        /// comment = ""

        public static int DzGetFPS()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetFPS"));
        }


        /// title = "转换地图坐标为小地图x坐标"
        /// description = "转换地图坐标(${x},${y})为小地图x坐标"
        /// comment = "小地图左下角为（0,0）"

        public static float DzFrameWorldToMinimapPosX(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameWorldToMinimapPosX"), x, y);
        }


        /// title = "转换地图坐标为小地图y坐标"
        /// description = "转换地图坐标(${x},${y})为小地图y坐标"
        /// comment = "小地图左下角为（0,0）"

        public static float DzFrameWorldToMinimapPosY(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameWorldToMinimapPosY"), x, y);
        }


        /// title = "自定义指定单位的小地图图标"
        /// description = "设置指定 ${单位} 的小地图图标：${imagefile}"
        /// comment = "图标大小只支持16×16，设置图标之前需要开启：中立建筑 - 小地图特殊标志"

        public static void DzWidgetSetMinimapIcon(JUnit whichunit, string path)
        {
            War3.CallNative(War3.GetNativeFunction("DzWidgetSetMinimapIcon"), whichunit.Handle, path);
        }


        /// title = "开启/关闭自定义指定单位的小地图图标"
        /// description = "设置 ${单位} ${On/Off} 自定义小地图图标"
        /// comment = ""

        public static void DzWidgetSetMinimapIconEnable(JUnit whichunit, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzWidgetSetMinimapIconEnable"), whichunit.Handle, enable);
        }


        /// title = "游戏提示信息界面"
        /// description = "游戏提示信息界面"
        /// comment = "如建筑升级完成提示"

        public static int DzFrameGetWorldFrameMessage()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetWorldFrameMessage"));
        }


        /// title = "显示游戏提示信息"
        /// description = "设置 ${消息界面} 显示 ${消息} 颜色：${颜色}，持续 ${时间} 秒，${是/否}永久显示"
        /// comment = ""

        public static void DzSimpleMessageFrameAddMessage(int whichframe, string text, int color, float duration, bool permanent)
        {
            War3.CallNative(War3.GetNativeFunction("DzSimpleMessageFrameAddMessage"), whichframe, text, color, duration, permanent);
        }


        /// title = "清理游戏提示信息"
        /// description = "清理 ${消息界面} 的游戏提示信息"
        /// comment = ""

        public static void DzSimpleMessageFrameClear(int whichframe)
        {
            War3.CallNative(War3.GetNativeFunction("DzSimpleMessageFrameClear"), whichframe);
        }


        /// title = "转换屏幕坐标到世界x坐标"
        /// description = "转换屏幕坐标（ ${x}，${Y}）为世界x坐标"
        /// comment = ""

        public static float DzConvertScreenPositionX(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzConvertScreenPositionX"), x, y);
        }


        /// title = "转换屏幕坐标到世界y坐标"
        /// description = "转换屏幕坐标（ ${x}，${Y}）为世界y坐标"
        /// comment = ""

        public static float DzConvertScreenPositionY(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzConvertScreenPositionY"), x, y);
        }


        /// title = "监听建筑选位置"
        /// description = "监听建筑选位置"
        /// comment = ""

        public static void DzRegisterOnBuildLocal(Action func)
        {
            War3.CallNative(War3.GetNativeFunction("DzRegisterOnBuildLocal"), func);
        }


        /// title = "获取建造的命令id"
        /// description = "获取建造的命令id"
        /// comment = "用于监听建筑选位置后"

        public static int DzGetOnBuildOrderId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnBuildOrderId"));
        }


        /// title = "获取建造的命令类型"
        /// description = "获取建造的命令类型"
        /// comment = "用于监听建筑选位置后"

        public static int DzGetOnBuildOrderType()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnBuildOrderType"));
        }


        /// title = "获取预建造对象"
        /// description = "获取预建造对象"
        /// comment = "用于监听建筑选位置后"

        public static JWidget DzGetOnBuildAgent()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetOnBuildAgent"));
            return new(handle);
        }


        /// title = "监听技能预选目标"
        /// description = "监听技能预选目标"
        /// comment = ""

        public static void DzRegisterOnTargetLocal(Action func)
        {
            War3.CallNative(War3.GetNativeFunction("DzRegisterOnTargetLocal"), func);
        }


        /// title = "获取监听到的技能"
        /// description = "获取监听到的技能"
        /// comment = "用于监听技能预选后"

        public static int DzGetOnTargetAbilId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnTargetAbilId"));
        }


        /// title = "获取监听到技能预选命令"
        /// description = "获取监听到技能预选命令"
        /// comment = "用于监听技能预选后"

        public static int DzGetOnTargetOrderId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnTargetOrderId"));
        }


        /// title = "获取监听到技能预选命令类型"
        /// description = "获取监听到技能预选命令类型"
        /// comment = "用于监听技能预选后"

        public static int DzGetOnTargetOrderType()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnTargetOrderType"));
        }


        /// title = "获取监听到技能预选目标"
        /// description = "获取监听到技能预选目标"
        /// comment = "用于监听技能预选后"

        public static JWidget DzGetOnTargetAgent()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetOnTargetAgent"));
            return new(handle);
        }


        /// title = "获取监听到技能预选目标"
        /// description = "获取监听到技能预选目标"
        /// comment = "用于监听技能预选后"

        public static JWidget DzGetOnTargetInstantTarget()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetOnTargetInstantTarget"));
            return new(handle);
        }


        /// title = "打开QQ群链接"
        /// description = "打开QQ群链接：${地址}"
        /// comment = "必须包括http://qm.qq.com开头的QQ群链接，调用后触发玩家加群，注意每分钟只能触发一次。"

        public static bool DzOpenQQGroupUrl(string url)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzOpenQQGroupUrl"), url);
        }


        /// title = "游戏界面限制设置"
        /// description = "界面 ${是/否} 限制在游戏窗口内"
        /// comment = "当为 ‘否’ 时游戏里的界面可以拖到游戏外面 "

        public static void DzFrameEnableClipRect(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameEnableClipRect"), enable);
        }


        /// title = "设置单位名字"
        /// description = "设置 ${单位} 名字：${name} "
        /// comment = ""

        public static void DzSetUnitName(JUnit whichUnit, string name)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitName"), whichUnit.Handle, name);
        }


        /// title = "设置单位头像模型"
        /// description = "设置 ${单位} 头像模型：${modelFile} "
        /// comment = ""

        public static void DzSetUnitPortrait(JUnit whichUnit, string modelFile)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitPortrait"), whichUnit.Handle, modelFile);
        }


        /// title = "设置单位描述"
        /// description = "设置 ${单位} 的描述： ${value} "
        /// comment = ""

        public static void DzSetUnitDescription(JUnit whichUnit, string value)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitDescription"), whichUnit.Handle, value);
        }


        /// title = "设置单位普攻弹道弧度"
        /// description = "设置 ${单位} 普攻弹道弧度：${arc} "
        /// comment = ""

        public static void DzSetUnitMissileArc(JUnit whichUnit, float arc)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitMissileArc"), whichUnit.Handle, arc);
        }


        /// title = "设置单位普攻弹道模型"
        /// description = "设置 ${单位} 普攻弹道模型：${modelFile} "
        /// comment = ""

        public static void DzSetUnitMissileModel(JUnit whichUnit, string modelFile)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitMissileModel"), whichUnit.Handle, modelFile);
        }


        /// title = "设置英雄称谓"
        /// description = "设置 ${英雄} 称谓：${name} "
        /// comment = ""

        public static void DzSetUnitProperName(JUnit whichUnit, string name)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitProperName"), whichUnit.Handle, name);
        }


        /// title = "设置单位普攻弹道自导允许"
        /// description = "设置 ${单位} 普攻弹道自导：${enable} "
        /// comment = ""

        public static void DzSetUnitMissileHoming(JUnit whichUnit, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitMissileHoming"), whichUnit.Handle, enable);
        }


        /// title = "设置单位普攻弹道速度"
        /// description = "设置 ${单位} 普攻弹道速度：${seed} "
        /// comment = ""

        public static void DzSetUnitMissileSpeed(JUnit whichUnit, float speed)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitMissileSpeed"), whichUnit.Handle, speed);
        }


        /// title = "特效显示/隐藏"
        /// description = "设置 ${whichEffect}  ${Show/Hide}"
        /// comment = ""

        public static void DzSetEffectVisible(JEffect whichHandle, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectVisible"), whichHandle.Handle, enable);
        }


        /// title = "复活单位"
        /// description = "设置 ${whichUnit} 复活为 ${whichPlayer} 的单位，生命值：${hp}，魔法值：${mp}，坐标(${x},${y})"
        /// comment = ""

        public static void DzReviveUnit(JUnit whichUnit, JPlayer whichPlayer, float hp, float mp, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzReviveUnit"), whichUnit.Handle, whichPlayer.Handle, hp, mp, x, y);
        }


        /// title = "获取普攻技能"
        /// description = "${whichUnit} 普攻技能"
        /// comment = ""

        public static JAbility DzGetAttackAbility(JUnit whichUnit)
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetAttackAbility"), whichUnit.Handle);
            return new(handle);
        }


        /// title = "结束普攻技能CD"
        /// description = "结束普攻 ${whichAbility} 的技能CD"
        /// comment = ""

        public static void DzAttackAbilityEndCooldown(JAbility whichHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzAttackAbilityEndCooldown"), whichHandle.Handle);
        }

        public static bool EXSetUnitArrayString(int uid, int id, int n, string name)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetUnitArrayString"), uid, id, n, name);
        }

        public static bool EXSetUnitInteger(int uid, int id, int n)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetUnitInteger"), uid, id, n);
        }


        /// title = "设置英雄类型称谓名[API][new]"
        /// description = "设置 ${英雄} 类型的称谓名： ${name} "
        /// comment = ""

        public static void DzSetHeroTypeProperName(int uid, string name)
        {
            EXSetUnitArrayString(uid, 61, 0, name);
            EXSetUnitInteger(uid, 61, 1);
        }


        /// title = "设置单位类型名[API][new]"
        /// description = "设置 ${单位} 类型名字： ${name} "
        /// comment = ""

        public static void DzSetUnitTypeName(int uid, string name)
        {
            EXSetUnitArrayString(uid, 10, 0, name);
            EXSetUnitInteger(uid, 10, 1);
        }


        /// title = "攻击类型[JAPI]"
        /// description = "[${whichUnit}] ${index} 的攻击类型为 ${attackType}"
        /// comment = ""

        public static bool DzIsUnitAttackType(JUnit whichUnit, int index, JAttackType attackType)
        {
            return JassApi.ConvertAttackType(Convert.ToInt32(JassApi.GetUnitState(whichUnit, JassApi.ConvertUnitState(16 + 19 * index)))) == attackType;
        }


        /// title = "设置攻击类型[API]"
        /// description = "设置 ${单位} ${~index} 的攻击类型为 ${attackType} "
        /// comment = ""

        public static void DzSetUnitAttackType(JUnit whichUnit, int index, JAttackType attackType)
        {
            JassApi.SetUnitState(whichUnit, JassApi.ConvertUnitState(16 + 19 * index), JassApi.GetHandleId(attackType));
        }


        /// title = "护甲类型[JAPI]"
        /// description = "[${whichUnit}] 的护甲类型为 ${defenseType}"
        /// comment = ""

        public static bool DzIsUnitDefenseType(JUnit whichUnit, int defenseType)
        {
            return (Convert.ToInt32(JassApi.GetUnitState(whichUnit, JassApi.ConvertUnitState(0x50)))) == defenseType;
        }


        /// title = "设置护甲类型[API]"
        /// description = "设置 ${单位} 的护甲类型为 ${defenseType} "
        /// comment = ""

        public static void DzSetUnitDefenseType(JUnit whichUnit, int defenseType)
        {
            JassApi.SetUnitState(whichUnit, JassApi.ConvertUnitState(0x50), defenseType);
        }


        /// title = "新建地形装饰物"
        /// description = "创建 ${doodadid} 版本：${var} 坐标：(${x}，${Y}，${z}) 角度：${rotate} 缩放：${scale}"
        /// comment = ""

        public static int DzDoodadCreate(int id, int var, float x, float y, float z, float rotate, float scale)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadCreate"), id, var, x, y, z, rotate, scale);
        }


        /// title = "装饰物的类型ID"
        /// description = "获取 ${doodadid} 的类型ID"
        /// comment = ""

        public static int DzDoodadGetTypeId(int doodad)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadGetTypeId"), doodad);
        }


        /// title = "设置装饰物模型"
        /// description = "设置 ${doodad} 的模型：${modelFile} "
        /// comment = ""

        public static void DzDoodadSetModel(int doodad, string modelFile)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetModel"), doodad, modelFile);
        }


        /// title = "改变装饰物队伍颜色"
        /// description = "改变 ${doodad} 的队伍颜色：${Color}"
        /// comment = ""

        public static void DzDoodadSetTeamColor(int doodad, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetTeamColor"), doodad, color);
        }


        /// title = "设置装饰物颜色"
        /// description = "设置 ${doodad} 的颜色：${color} "
        /// comment = ""

        public static void DzDoodadSetColor(int doodad, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetColor"), doodad, color);
        }


        /// title = "装饰物的X坐标"
        /// description = "获取 ${doodad} 的X坐标"
        /// comment = ""

        public static float DzDoodadGetX(int doodad)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzDoodadGetX"), doodad);
        }


        /// title = "装饰物的Y坐标"
        /// description = "获取 ${doodad} 的Y坐标"
        /// comment = ""

        public static float DzDoodadGetY(int doodad)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzDoodadGetY"), doodad);
        }


        /// title = "装饰物的Z坐标"
        /// description = "获取 ${doodad} 的Z坐标"
        /// comment = ""

        public static float DzDoodadGetZ(int doodad)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzDoodadGetZ"), doodad);
        }


        /// title = "设置装饰物位置"
        /// description = "设置 ${doodad} 的坐标：(${x}，${Y}，${z})"
        /// comment = ""

        public static void DzDoodadSetPosition(int doodad, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetPosition"), doodad, x, y, z);
        }


        /// title = "设置装饰物旋转"
        /// description = "设置 ${doodad} 旋转，角度：${rotate} 方向：(${axisX}，${axisY}，${axisZ})"
        /// comment = ""

        public static void DzDoodadSetOrientMatrixRotate(int doodad, float angle, float axisX, float axisY, float axisZ)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetOrientMatrixRotate"), doodad, angle, axisX, axisY, axisZ);
        }


        /// title = "修改装饰物尺寸"
        /// description = "设置 ${doodad} 缩放：(${x}，${y}，${z})"
        /// comment = ""

        public static void DzDoodadSetOrientMatrixScale(int doodad, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetOrientMatrixScale"), doodad, x, y, z);
        }


        /// title = "装饰物重置大小"
        /// description = "设置 ${doodad} 重置大小"
        /// comment = ""

        public static void DzDoodadSetOrientMatrixResize(int doodad)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetOrientMatrixResize"), doodad);
        }


        /// title = "装饰物显示/隐藏"
        /// description = "设置 ${doodad} ${Show/Hide}"
        /// comment = ""

        public static void DzDoodadSetVisible(int doodad, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetVisible"), doodad, enable);
        }


        /// title = "装饰物播放动画"
        /// description = "设置 ${doodad} 播放${Animation Name}  随机播放${inNoAnim}"
        /// comment = ""

        public static void DzDoodadSetAnimation(int doodad, string animName, bool animRandom)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetAnimation"), doodad, animName, animRandom);
        }


        /// title = "设置装饰物动画播放速度"
        /// description = "设置 ${doodad} 的动画播放速度为正常速度的 ${Percent} 倍"
        /// comment = ""

        public static void DzDoodadSetTimeScale(int doodad, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetTimeScale"), doodad, scale);
        }


        /// title = "装饰物动画播放速度"
        /// description = " ${doodad} 的动画播放速度"
        /// comment = ""

        public static float DzDoodadGetTimeScale(int doodad)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzDoodadGetTimeScale"), doodad);
        }


        /// title = "装饰物当前动画编号"
        /// description = " ${doodad} 的当前动画编号"
        /// comment = ""

        public static int DzDoodadGetCurrentAnimationIndex(int doodad)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadGetCurrentAnimationIndex"), doodad);
        }


        /// title = "装饰物动画数量"
        /// description = " ${doodad} 的动画数量"
        /// comment = ""

        public static int DzDoodadGetAnimationCount(int doodad)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadGetAnimationCount"), doodad);
        }


        /// title = "装饰物动画名"
        /// description = " ${doodad} 第 ${index} 个动画的动画名"
        /// comment = ""

        public static string DzDoodadGetAnimationName(int doodad, int index)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzDoodadGetAnimationName"), doodad, index);
        }


        /// title = "装饰物动画时间"
        /// description = " ${doodad} 第 ${index} 个动画的时间"
        /// comment = ""

        public static int DzDoodadGetAnimationTime(int doodad, int index)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadGetAnimationTime"), doodad, index);
        }


        /// title = "解锁JASS字节码限制 [NEW]"
        /// description = " ${是/否} 解锁JASS字节码限制"
        /// comment = ""

        public static void DzUnlockOpCodeLimit(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnlockOpCodeLimit"), enable);
        }


        /// title = "设置剪切板 [NEW]"
        /// description = "设置剪切板 ${内容}"
        /// comment = ""

        public static bool DzSetClipboard(string content)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetClipboard"), content);
        }


        /// title = "删除装饰物  [NEW]"
        /// description = "删除 ${doodad} "
        /// comment = ""

        public static void DzDoodadRemove(int doodad)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadRemove"), doodad);
        }


        /// title = "降低玩家科技等级 [NEW]"
        /// description = " 降低${玩家} 的 ${科技} ${等级} 个等级"
        /// comment = ""

        public static void DzRemovePlayerTechResearched(JPlayer whichPlayer, int techid, int removelevels)
        {
            War3.CallNative(War3.GetNativeFunction("DzRemovePlayerTechResearched"), whichPlayer.Handle, techid, removelevels);
        }


        /// title = "获取单位的指定技能"
        /// description = "查找 ${单位} 的指定技能 ${技能}"
        /// comment = ""

        public static JAbility DzUnitFindAbility(JUnit whichUnit, int abilcode)
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzUnitFindAbility"), whichUnit.Handle, abilcode);
            return new(handle);
        }


        /// title = "设置技能数据-字符串"
        /// description = "设置 ${技能} 的字符串 ${名字} 数据为 ${内容}"
        /// comment = ""

        public static void DzAbilitySetStringData(JAbility whichAbility, string key, string value)
        {
            War3.CallNative(War3.GetNativeFunction("DzAbilitySetStringData"), whichAbility.Handle, key, value);
        }


        /// title = "设置技能启用/禁用"
        /// description = "设置 ${技能} 的启用状态为 ${启用} ，隐藏UI为 ${隐藏}"
        /// comment = ""

        public static void DzAbilitySetEnable(JAbility whichAbility, bool enable, bool hideUI)
        {
            War3.CallNative(War3.GetNativeFunction("DzAbilitySetEnable"), whichAbility.Handle, enable, hideUI);
        }


        /// title = "设置单位实例的移动类型"
        /// description = "设置 ${单位} 实例的移动类型为 ${Value}"
        /// comment = ""

        public static void DzUnitSetMoveType(JUnit whichUnit, string moveType)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitSetMoveType"), whichUnit.Handle, moveType);
        }


        /// title = "获取 Frame 的 宽度"
        /// description = "获取 ${frame} 的宽度"
        /// comment = ""

        public static float DzFrameGetWidth(int frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetWidth"), frame);
        }


        /// title = "设置模型界面播放动画（编号）"
        /// description = "设置 ${模型界面} 播放第 ${index}）个动画，播放方式${flag}"
        /// comment = ""

        public static void DzFrameSetAnimateByIndex(int frame, int index, int flag)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetAnimateByIndex"), frame, index, flag);
        }

        public static void DzSetUnitDataCacheInteger(int uid, int id, int index, int v)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitDataCacheInteger"), uid, id, index, v);
        }

        public static void DzUnitUIAddLevelArrayInteger(int uid, int id, int lv, int v)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitUIAddLevelArrayInteger"), uid, id, lv, v);
        }


        /// title = "设置单位整数物编数据"
        /// description = "设置 ${单位} 的 ${id} 数值为 ${data}"
        /// comment = ""

        public static void KKWESetUnitDataCacheInteger(int uid, int id, int v)
        {
            DzSetUnitDataCacheInteger(uid, id, 0, v);
        }


        /// title = "设置单位物编数据(建筑升级)"
        /// description = "设置 ${单位} 的第 ${id} 个建筑升级 ${单位类型}"
        /// comment = ""

        public static void KKWEUnitUIAddUpgradesIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 94, id, v);
        }


        /// title = "设置单位物编数据(农民可建造建筑)"
        /// description = "设置 ${单位} 的第 ${id} 个可建造建筑 ${单位类型}"
        /// comment = ""

        public static void KKWEUnitUIAddBuildsIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 100, id, v);
        }


        /// title = "设置单位物编数据(可研究的科技)"
        /// description = "设置 ${单位} 的第 ${id} 个可研究的科技 ${科技类型}"
        /// comment = ""

        public static void KKWEUnitUIAddResearchesIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 112, id, v);
        }


        /// title = "设置单位物编数据(可训练的单位)"
        /// description = "设置 ${单位} 的第 ${id} 个可训练的单位 ${单位类型}"
        /// comment = ""

        public static void KKWEUnitUIAddTrainsIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 106, id, v);
        }


        /// title = "设置单位物编数据(出售的单位)"
        /// description = "设置 ${单位} 的第 ${id} 个可出售的单位 ${单位类型}"
        /// comment = ""

        public static void KKWEUnitUIAddSellsUnitIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 118, id, v);
        }


        /// title = "设置单位物编数据(出售的物品)"
        /// description = "设置 ${单位} 的第 ${id} 个可出售的物品 ${物品类型}"
        /// comment = ""

        public static void KKWEUnitUIAddSellsItemIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 124, id, v);
        }


        /// title = "设置单位物编数据(制造的物品)"
        /// description = "设置 ${单位} 的第 ${id} 个可制造的物品 ${物品类型}"
        /// comment = ""

        public static void KKWEUnitUIAddMakesItemIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 130, id, v);
        }


        /// title = "设置单位物编数据(科技需求)[单位]"
        /// description = "设置 ${单位} 的第 ${id} 个科技需求 ${单位类型}"
        /// comment = ""

        public static void KKWEUnitUIAddRequiresUnitCode(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 166, id, v);
        }


        /// title = "设置单位物编数据(科技需求)[科技]"
        /// description = "设置 ${单位} 的第 ${id} 个科技需求 ${科技类型}"
        /// comment = ""

        public static void KKWEUnitUIAddRequiresTechcode(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 166, id, v);
        }


        /// title = "设置单位物编数据(科技需求值)"
        /// description = "设置 ${单位} 的第 ${id} 个科技需求值 ${数量}"
        /// comment = ""

        public static void KKWEUnitUIAddRequiresAmounts(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 172, id, v);
        }


        /// title = "设置物品模型 [NEW]"
        /// description = "设置 ${whichItem} 的模型：${modelFile} "
        /// comment = ""

        public static void DzItemSetModel(JItem whichItem, string file)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetModel"), whichItem.Handle, file);
        }


        /// title = "设置物品颜色 [NEW]"
        /// description = "设置 ${whichItem} 的颜色：${color} "
        /// comment = ""

        public static void DzItemSetVertexColor(JItem whichItem, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetVertexColor"), whichItem.Handle, color);
        }


        /// title = "设置物品透明度(0-255) [NEW]"
        /// description = "设置 ${whichItem} 的透明度为 ${alpha}"
        /// comment = ""

        public static void DzItemSetAlpha(JItem whichItem, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetAlpha"), whichItem.Handle, color);
        }


        /// title = "设置物品头像 [NEW]"
        /// description = "设置 ${whichItem} 的头像：${modelFile} "
        /// comment = ""

        public static void DzItemSetPortrait(JItem whichItem, string modelPath)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetPortrait"), whichItem.Handle, modelPath);
        }


        /// title = "血条刷新事件 [NEW]"
        /// description = "血条刷新事件"
        /// comment = ""

        public static void DzFrameHookHpBar(Action func)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameHookHpBar"), func);
        }


        /// title = "触发血条的单位 [NEW]"
        /// description = "触发血条的单位"
        /// comment = "用于血条刷新事件下"

        public static JUnit DzFrameGetTriggerHpBarUnit()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetTriggerHpBarUnit"));
            return new(handle);
        }


        /// title = "触发的血条 [NEW]"
        /// description = "触发的血条"
        /// comment = "用于血条刷新事件下"

        public static int DzFrameGetTriggerHpBar()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetTriggerHpBar"));
        }


        /// title = "获取单位血条 [NEW]"
        /// description = "获取 ${whichUnit} 血条"
        /// comment = "获取单位血条"

        public static int DzFrameGetUnitHpBar(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetUnitHpBar"), whichUnit.Handle);
        }


        /// title = "鼠标界面 [NEW]"
        /// description = "鼠标界面"
        /// comment = ""

        public static int DzGetCursorFrame()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetCursorFrame"));
        }


        /// title = "是否有指定锚点 [NEW]"
        /// description = "判断 ${whichFrame} 是否有 ${anchor} 锚点"
        /// comment = ""

        public static bool DzFrameGetPointValid(int frame, int anchor)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameGetPointValid"), frame, anchor);
        }


        /// title = "获取相对锚点所在界面 [NEW]"
        /// description = "判断 ${whichFrame} 的相对锚点 ${anchor} 所在界面"
        /// comment = ""

        public static int DzFrameGetPointRelative(int frame, int anchor)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetPointRelative"), frame, anchor);
        }


        /// title = "获取相对锚点的界面锚点 [NEW]"
        /// description = "判断 ${whichFrame} 的相对锚点 ${anchor} 所在界面的锚点"
        /// comment = ""

        public static int DzFrameGetPointRelativePoint(int frame, int anchor)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetPointRelativePoint"), frame, anchor);
        }


        /// title = "获取锚点X坐标 [NEW]"
        /// description = "${whichFrame} 的 ${anchor} X坐标"
        /// comment = ""

        public static float DzFrameGetPointX(int frame, int anchor)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetPointX"), frame, anchor);
        }


        /// title = "获取锚点Y坐标 [NEW]"
        /// description = "${whichFrame} 的 ${anchor} Y坐标"
        /// comment = ""

        public static float DzFrameGetPointY(int frame, int anchor)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetPointY"), frame, anchor);
        }
        /*  
              public static bool DzIsLeapYear(int year)
              {
                  return (ModuloInteger(year, 4) == 0 and ModuloInteger(year , 100) != 0) or(ModuloInteger(year, 400) == 0);
              }

          public static string DzGetTimeDateFromTimestamp(int timestamp)
              {
                  int totalSeconds = 0;
                  int days = 0;
                  int day = 0;
                  int secondsInDay = 0;
                  int remainingSeconds = 0;
                  int year = 0;
                  int totalDays = 0;
                  int num = 0;
                  int month = 0;
                  int hour = 0;
                  int minute = 0;
                  int second = 0;
                  string str = null;
                  loop;
                  if (DzIsLeapYear(year))
                  {
                      num = num + 366;
                  }
                  else
                  {
                      num = num + 365;
                  }
                  if (num > totalDays)
                  {
                      totalDays = totalDays - days;
                      exitwhen true;
                  }
                  else
                  {
                      days = num;
                  }
                  year = year + 1;
                  endloop;
                  month = 1;
                  num = 0;
                  days = 0;
                  if (DzIsLeapYear(year))
                  {
                      loop;
                      num = num + MonthDay[100 + month];
                      if (num >= totalDays)
                      {
                          day = totalDays - days;
                          exitwhen true;
                      }
                      else
                      {
                          days = num;
                      }
                      month = month + 1;
                      endloop;
                  }
                  else
                  {
                      loop;
                      num = num + MonthDay[month];
                      if (num >= totalDays)
                      {
                          day = totalDays - days;
                          exitwhen true;
                      }
                      else
                      {
                          days = num;
                      }
                      month = month + 1;
                      endloop;
                  }
                  if (day == 0)
                  {
                      day = 1;
                  }
                  hour = remainingSeconds / 3600;
                  remainingSeconds = ModuloInteger(remainingSeconds, 3600);
                  minute = remainingSeconds / 60;
                  second = ModuloInteger(remainingSeconds, 60);
                  str = I2S(year) + "-" + I2S(month) + "-" + I2S(day) + " " + I2S(hour) + ":" + I2S(minute) + ":" + I2S(second);
                  SaveInteger(Hash, timestamp, 1, year);
                  SaveInteger(Hash, timestamp, 2, month);
                  SaveInteger(Hash, timestamp, 3, day);
                  SaveStr(Hash, timestamp, 4, str);
                  return str;
              }


              /// title = "转换时间戳为具体时间 [NEW]"
              /// description = "转换 ${时间戳} 为具体时间"
              /// comment = "返回值类似：2025-1-10 17:4:40"

              public static string KKAPIGetTimeDateFromTimestamp(int timestamp)
              {
                  timestamp = IMaxBJ(timestamp, 0);
                  if (HaveSavedString(Hash, timestamp, 4))
                  {
                      return LoadStr(Hash, timestamp, 4);
                  }
                  else
                  {
                      return DzGetTimeDateFromTimestamp(timestamp);
                  }
              }


              /// title = "获取时间戳年份 [NEW]"
              /// description = " ${时间戳} 的年份"
              /// comment = ""

              public static int KKAPIGetTimestampYear(int timestamp)
              {
                  timestamp = IMaxBJ(timestamp, 0);
                  if (HaveSavedInteger(Hash, timestamp, 1) == false)
                  {
                      DzGetTimeDateFromTimestamp(timestamp);
                  }
                  return LoadInteger(Hash, timestamp, 1);
              }


              /// title = "获取时间戳月份 [NEW]"
              /// description = " ${时间戳} 的月份"
              /// comment = ""

              public static int KKAPIGetTimestampMonth(int timestamp)
              {
                  timestamp = IMaxBJ(timestamp, 0);
                  if (HaveSavedInteger(Hash, timestamp, 2) == false)
                  {
                      DzGetTimeDateFromTimestamp(timestamp);
                  }
                  return LoadInteger(Hash, timestamp, 2);
              }


              /// title = "获取时间戳日份 [NEW]"
              /// description = " ${时间戳} 的日份"
              /// comment = ""

              public static int KKAPIGetTimestampDay(int timestamp)
              {
                  timestamp = IMaxBJ(timestamp, 0);
                  if (HaveSavedInteger(Hash, timestamp, 3) == false)
                  {
                      DzGetTimeDateFromTimestamp(timestamp);
                  }
                  return LoadInteger(Hash, timestamp, 3);
              }
      */

        /// title = "打印调试信息到平台日志 [NEW]"
        /// description = "打印 ${信息} 到平台日志"
        /// comment = "用于调试，打印信息到平台日志文件"

        public static void DzWriteLog(string msg)
        {
            War3.CallNative(War3.GetNativeFunction("DzWriteLog"), msg);
        }


        /// title = "获取当前漂浮文字的字体 [NEW]"
        /// description = "漂浮文字的字体"
        /// comment = ""

        public static string DzTextTagGetFont()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzTextTagGetFont"));
        }


        /// title = "设置漂浮文字字体 [NEW]"
        /// description = "设置漂浮文字字体：${font}"
        /// comment = ""

        public static void DzTextTagSetFont(string fileName)
        {
            War3.CallNative(War3.GetNativeFunction("DzTextTagSetFont"), fileName);
        }


        /// title = "设置漂浮文字透明度 [NEW]"
        /// description = "设置 ${漂浮文字} 透明度：${alpha}"
        /// comment = ""

        public static void DzTextTagSetStartAlpha(JTextTag t, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("DzTextTagSetStartAlpha"), t.Handle, alpha);
        }


        /// title = "获取漂浮文字的阴影颜色 [NEW]"
        /// description = "获取 ${漂浮文字} 的阴影颜色"
        /// comment = ""

        public static int DzTextTagGetShadowColor(JTextTag t)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzTextTagGetShadowColor"), t.Handle);
        }


        /// title = "设置漂浮文字阴影颜色 [NEW]"
        /// description = "设置 ${漂浮文字} 阴影颜色：${color}"
        /// comment = ""

        public static void DzTextTagSetShadowColor(JTextTag t, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzTextTagSetShadowColor"), t.Handle, color);
        }


        /// title = "获取单位组里单位数量 [NEW]"
        /// description = "获取 ${group} 里单位数量"
        /// comment = ""

        public static int DzGroupGetCount(JGroup g)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGroupGetCount"), g.Handle);
        }


        /// title = "获取单位组里指定索引的单位 [NEW]"
        /// description = "获取 ${group} 里第 ${index} 个单位"
        /// comment = ""

        public static JUnit DzGroupGetUnitAt(JGroup g, int index)
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGroupGetUnitAt"), g.Handle, index);
            return new(handle);
        }


        /// title = "创建幻象单位 [NEW]"
        /// description = "为 ${player} 创建一个 ${unitId} 类型的幻象，在坐标(${x},${y}),面向角度：${face}"
        /// comment = ""

        public static JUnit DzUnitCreateIllusion(JPlayer p, int unitId, float x, float y, float face)
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzUnitCreateIllusion"), p.Handle, unitId, x, y, face);
            return new(handle);
        }


        /// title = "为单位创建幻象 [NEW]"
        /// description = "为 ${unit} 创建一个幻象"
        /// comment = ""

        public static JUnit DzUnitCreateIllusionFromUnit(JUnit u)
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzUnitCreateIllusionFromUnit"), u.Handle);
            return new(handle);
        }


        /// title = "检查字符串是否包含指定的子字符串 [NEW]"
        /// description = "检测 ${targetStr} 是否包含 ${searchStr} 字符串，判定规则：${caseSensitive} 区分大小写"
        /// comment = ""

        public static bool DzStringContains(string s, string whichString, bool caseSensitive)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzStringContains"), s, whichString, caseSensitive);
        }


        /// title = "字符串中查找子字符串并返回其位置 [NEW]"
        /// description = "检测 ${targetStr} 包含 ${searchStr} 的位置，从第 ${caseSensitive} 位开始，判定规则：${caseSensitive} 区分大小写"
        /// comment = ""

        public static int DzStringFind(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFind"), s, whichString, off, caseSensitive);
        }


        /// title = "检测字符串里第一个包含指定字符串里任意字符的位置 [NEW]"
        /// description = "检测 ${targetStr} 第一个包含 ${searchStr} 里任意字符的位置，从第 ${caseSensitive} 位开始，判定规则：${caseSensitive} 区分大小写"
        /// comment = ""

        public static int DzStringFindFirstOf(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFindFirstOf"), s, whichString, off, caseSensitive);
        }


        /// title = "检查字符串第一个不包含指定字符串里任意字符的位置 [NEW]"
        /// description = "检测 ${targetStr} 第一个不包含 ${searchStr} 里任意字符的位置，从第 ${caseSensitive} 位开始，判定规则：${caseSensitive} 区分大小写"
        /// comment = ""

        public static int DzStringFindFirstNotOf(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFindFirstNotOf"), s, whichString, off, caseSensitive);
        }


        /// title = "从后往前查找字符串中包含指定字符串任意字符的所在位置 [NEW]"
        /// description = "从后往前检测 ${targetStr} 包含指定字符串 ${searchStr} 任意字符的位置，从第 ${caseSensitive} 位开始，判定规则：${caseSensitive} 区分大小写"
        /// comment = ""

        public static int DzStringFindLastOf(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFindLastOf"), s, whichString, off, caseSensitive);
        }


        /// title = "从后往前查找字符串中不包含指定字符串任意字符的所在位置 [NEW]"
        /// description = "从后往前检测 ${targetStr} 不包含指定字符串 ${searchStr} 任意字符的位置，从第 ${caseSensitive} 位开始，判定规则：${caseSensitive} 区分大小写"
        /// comment = ""

        public static int DzStringFindLastNotOf(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFindLastNotOf"), s, whichString, off, caseSensitive);
        }


        /// title = "删除字符串左边的空格 [NEW]"
        /// description = "删除 ${targetStr} 左边的空格"
        /// comment = ""

        public static string DzStringTrimLeft(string s)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringTrimLeft"), s);
        }


        /// title = "删除字符串右边的空格 [NEW]"
        /// description = "删除 ${targetStr} 右边的空格"
        /// comment = ""

        public static string DzStringTrimRight(string s)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringTrimRight"), s);
        }


        /// title = "删除字符串两边的空格 [NEW]"
        /// description = "删除 ${targetStr} 两边的空格"
        /// comment = ""

        public static string DzStringTrim(string s)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringTrim"), s);
        }


        /// title = "反转字符串 [NEW]"
        /// description = "反转 ${targetStr}"
        /// comment = ""

        public static string DzStringReverse(string s)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringReverse"), s);
        }


        /// title = "替换字符串 [NEW]"
        /// description = "替换 ${targetStr} 里的 ${searchStr} 为 ${newStr} "
        /// comment = ""

        public static string DzStringReplace(string s, string whichString, string replaceWith, bool caseSensitive)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringReplace"), s, whichString, replaceWith, caseSensitive);
        }


        /// title = "插入字符串 [NEW]"
        /// description = "在 ${targetStr} 的位置 ${whichPosition} 插入 ${newStr} "
        /// comment = ""

        public static string DzStringInsert(string s, int whichPosition, string whichString)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringInsert"), s, whichPosition, whichString);
        }


        /// title = "整数的2进制的位值 [NEW]"
        /// description = " ${i} 的2进制的第 ${byteIndex} 位的值"
        /// comment = ""

        public static int DzBitGet(int i, int byteIndex)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitGet"), i, byteIndex);
        }


        /// title = "设置整数的2进制的位值 [NEW]"
        /// description = "设置 ${i} 的2进制的第 ${byteIndex} 位的值：${byteValue}"
        /// comment = ""

        public static int DzBitSet(int i, int byteIndex, int byteValue)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitSet"), i, byteIndex, byteValue);
        }


        /// title = "整数的256进制的位值 [NEW]"
        /// description = " ${i} 256进制第 ${byteIndex} 位的值"
        /// comment = ""

        public static int DzBitGetByte(int i, int byteIndex)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitGetByte"), i, byteIndex);
        }


        /// title = "设置整数的256进制的位值 [NEW]"
        /// description = "设置 ${i} 的256进制的第 ${byteIndex} 位的值：${byteValue}"
        /// comment = ""

        public static int DzBitSetByte(int i, int byteIndex, int byteValue)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitSetByte"), i, byteIndex, byteValue);
        }


        /// title = "按位取反 [NEW]"
        /// description = " ${i} 按位取反"
        /// comment = ""

        public static int DzBitNot(int i)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitNot"), i);
        }


        /// title = "按位与 [NEW]"
        /// description = "${a} 和 ${b} 按位与"
        /// comment = ""

        public static int DzBitAnd(int a, int b)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitAnd"), a, b);
        }


        /// title = "按位或 [NEW]"
        /// description = "${a} 和 ${b} 按位或"
        /// comment = ""

        public static int DzBitOr(int a, int b)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitOr"), a, b);
        }


        /// title = "按位异或 [NEW]"
        /// description = "${a} 和 ${b} 按位异或"
        /// comment = ""

        public static int DzBitXor(int a, int b)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitXor"), a, b);
        }


        /// title = "按位左移 [NEW]"
        /// description = " ${i} 的所有位向左移动 ${bitsToShift} 位"
        /// comment = ""

        public static int DzBitShiftLeft(int i, int bitsToShift)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitShiftLeft"), i, bitsToShift);
        }


        /// title = "按位右移 [NEW]"
        /// description = " ${i} 的所有位向右移动 ${bitsToShift} 位"
        /// comment = ""

        public static int DzBitShiftRight(int i, int bitsToShift)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitShiftRight"), i, bitsToShift);
        }


        /// title = "4字节组合为整数 [NEW]"
        /// description = "在4个字节(${A},${B},${C},${D})组合成一个整数。"
        /// comment = "这里组合是256进制，组合的结果其实是DCBA"

        public static int DzBitToInt(int b1, int b2, int b3, int b4)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitToInt"), b1, b2, b3, b4);
        }


        /// title = "对单位组添加命令到队列(无目标) [NEW]"
        /// description = "对单位组 ${whichGroup} 添加 ${order} 命令到队列"
        /// comment = ""

        public static bool DzQueueGroupImmediateOrderById(JGroup whichGroup, int order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueGroupImmediateOrderById"), whichGroup.Handle, order);
        }


        /// title = "对单位组添加命令到队列(指定坐标) [NEW]"
        /// description = "对单位组 ${whichGroup} 添加 ${order} 命令到队列，位置 (${x}, ${y}) "
        /// comment = ""

        public static bool DzQueueGroupPointOrderById(JGroup whichGroup, int order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueGroupPointOrderById"), whichGroup.Handle, order, x, y);
        }

        public static bool DzQueueGroupTargetOrderById(JGroup whichGroup, int order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueGroupTargetOrderById"), whichGroup.Handle, order, targetWidget.Handle);
        }


        /// title = "对单位添加命令到队列(无目标) [NEW]"
        /// description = "对单位 ${whichUnit} 添加 ${order} 命令到队列"
        /// comment = ""

        public static bool DzQueueIssueImmediateOrderById(JUnit whichUnit, int order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueImmediateOrderById"), whichUnit.Handle, order);
        }


        /// title = "对单位添加命令到队列(指定坐标) [NEW]"
        /// description = "对单位 ${whichUnit} 添加 ${order} 命令到队列，位置 (${x}, ${y}) "
        /// comment = ""

        public static bool DzQueueIssuePointOrderById(JUnit whichUnit, int order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssuePointOrderById"), whichUnit.Handle, order, x, y);
        }

        public static bool DzQueueIssueTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueTargetOrderById"), whichUnit.Handle, order, targetWidget.Handle);
        }

        public static bool DzQueueIssueInstantPointOrderById(JUnit whichUnit, int order, float x, float y, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueInstantPointOrderById"), whichUnit.Handle, order, x, y, instantTargetWidget.Handle);
        }

        public static bool DzQueueIssueInstantTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueInstantTargetOrderById"), whichUnit.Handle, order, targetWidget.Handle, instantTargetWidget.Handle);
        }


        /// title = "对单位添加建造命令到队列 [NEW]"
        /// description = "对单位 ${whichPeon} 添加建造 ${unitId} 命令到队列，位置 (${x}, ${y})"
        /// comment = ""

        public static bool DzQueueIssueBuildOrderById(JUnit whichPeon, int unitId, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueBuildOrderById"), whichPeon.Handle, unitId, x, y);
        }


        /// title = "添加中介命令到队列(无目标) [NEW]"
        /// description = "使 ${forWhichPlayer} 对 ${neutralStructure} 添加 ${unitId} 命令到队列"
        /// comment = ""

        public static bool DzQueueIssueNeutralImmediateOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueNeutralImmediateOrderById"), forWhichPlayer.Handle, neutralStructure.Handle, unitId);
        }


        /// title = "添加中介命令到队列(指定坐标) [NEW]"
        /// description = "使 ${forWhichPlayer} 对 ${neutralStructure} 添加 ${unitId} 命令到队列，位置 (${x}, ${y})"
        /// comment = ""

        public static bool DzQueueIssueNeutralPointOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueNeutralPointOrderById"), forWhichPlayer.Handle, neutralStructure.Handle, unitId, x, y);
        }

        public static bool DzQueueIssueNeutralTargetOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueNeutralTargetOrderById"), forWhichPlayer.Handle, neutralStructure.Handle, unitId, target.Handle);
        }


        /// title = "获取单位的命令数量 [NEW]"
        /// description = "获取单位 ${u} 的命令数量"
        /// comment = ""

        public static int DzUnitOrdersCount(JUnit u)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzUnitOrdersCount"), u.Handle);
        }


        /// title = "清除单位命令队列 [NEW]"
        /// description = "清除单位 ${u} 命令，清理规则： ${b} 仅清理队列里的命令"
        /// comment = ""

        public static void DzUnitOrdersClear(JUnit u, bool onlyQueued)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitOrdersClear"), u.Handle, onlyQueued);
        }


        /// title = "执行单位的命令队列 [NEW]"
        /// description = "执行单位 ${u} 的命令队列"
        /// comment = ""

        public static void DzUnitOrdersExec(JUnit u)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitOrdersExec"), u.Handle);
        }


        /// title = "强制停止单位当前命令 [NEW]"
        /// description = "强制停止单位 ${u} 的当前命令，${b} 清理队列里的命令"
        /// comment = ""

        public static void DzUnitOrdersForceStop(JUnit u, bool clearQueue)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitOrdersForceStop"), u.Handle, clearQueue);
        }


        /// title = "反转单位命令队列 [NEW]"
        /// description = "反转 ${u} 命令队列"
        /// comment = "原本单位添加命令队列后，会"

        public static void DzUnitOrdersReverse(JUnit u)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitOrdersReverse"), u.Handle);
        }


        /// title = "打开Excel文件 [NEW]"
        /// description = "打开Excel文件 ${filePath} "
        /// comment = "会返回一个工作表"

        public static int DzXlsxOpen(string filePath)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxOpen"), filePath);
        }


        /// title = "关闭工作表 [NEW]"
        /// description = "关闭工作表：${docHandle}"
        /// comment = ""

        public static bool DzXlsxClose(int docHandle)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzXlsxClose"), docHandle);
        }


        /// title = "工作表的总行数 [NEW]"
        /// description = " ${docHandle} 里 ${sheetName} 页的总行数"
        /// comment = ""

        public static int DzXlsxWorksheetGetRowCount(int docHandle, string sheetName)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxWorksheetGetRowCount"), docHandle, sheetName);
        }


        /// title = "工作表的总列数 [NEW]"
        /// description = " ${docHandle} 里 ${sheetName} 的列数"
        /// comment = ""

        public static int DzXlsxWorksheetGetColumnCount(int docHandle, string sheetName)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxWorksheetGetColumnCount"), docHandle, sheetName);
        }


        /// title = "单元格的数据类型 [NEW]"
        /// description = " ${docHandle} 里 ${sheetName} 中单元格 (${row}, ${column}) 的数据类型"
        /// comment = "0=None,1=String,2=Integer,3=Boolean,4=Real"

        public static int DzXlsxWorksheetGetCellType(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxWorksheetGetCellType"), docHandle, sheetName, row, column);
        }


        /// title = "工作表的值（字符串） [NEW]"
        /// description = "${docHandle} 里 ${sheetName} 中单元格 (${row}, ${column}) 的字符串值"
        /// comment = ""

        public static string DzXlsxWorksheetGetCellString(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzXlsxWorksheetGetCellString"), docHandle, sheetName, row, column);
        }


        /// title = "工作表的值（整数） [NEW]"
        /// description = "${docHandle} 里 ${sheetName} 中单元格 (${row}, ${column}) 的整数值"
        /// comment = ""

        public static int DzXlsxWorksheetGetCellInteger(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxWorksheetGetCellInteger"), docHandle, sheetName, row, column);
        }


        /// title = "工作表的值（布尔值） [NEW]"
        /// description = " ${docHandle} 里 ${sheetName} 中单元格 (${row}, ${column}) 的布尔值"
        /// comment = ""

        public static bool DzXlsxWorksheetGetCellBoolean(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzXlsxWorksheetGetCellBoolean"), docHandle, sheetName, row, column);
        }


        /// title = "工作表的值（实数） [NEW]"
        /// description = "${docHandle} 里 ${sheetName} 中单元格 (${row}, ${column}) 的实数值"
        /// comment = ""

        public static float DzXlsxWorksheetGetCellFloat(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzXlsxWorksheetGetCellFloat"), docHandle, sheetName, row, column);
        }


        /// title = "设置界面纹理坐标 [NEW]"
        /// description = "设置 ${frame} 的纹理坐标为 (${left}, ${top}, ${right}, ${bottom})"
        /// comment = ""

        public static void DzFrameSetTexCoord(int frame, float left, float top, float right, float bottom)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTexCoord"), frame, left, top, right, bottom);
        }


        /// title = "技能 - 设置技能施法距离（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的施法距离 ${range} "
        /// comment = "[[单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityRange(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityRange"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能施法距离（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的施法距离"
        /// comment = ""

        public static float DzGetUnitAbilityRange(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityRange"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能施法范围（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的施法范围 ${area} "
        /// comment = "[[单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityArea(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityArea"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能施法范围（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的施法范围"
        /// comment = ""

        public static float DzGetUnitAbilityArea(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityArea"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能冷却时间（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的当前冷却时间 ${cool} /最大冷却时间 ${max_cool} "
        /// comment = "[[单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityCool(JUnit Unit, int abil_code, float cool, float max_cool)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityCool"), Unit.Handle, abil_code, cool, max_cool);
        }


        /// title = "技能 - 获取技能当前冷却时间（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的当前冷却时间"
        /// comment = ""

        public static float DzGetUnitAbilityCool(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityCool"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 获取技能最大冷却时间（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的最大冷却时间"
        /// comment = ""

        public static float DzGetUnitAbilityMaxCool(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMaxCool"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能数据A（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的数据A ${DataA} "
        /// comment = "[[通魔的数据A是施法持续时间;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityDataA(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataA"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能数据A（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的数据A"
        /// comment = ""

        public static float DzGetUnitAbilityDataA(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataA"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能数据B（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的数据B ${DataB} "
        /// comment = "[[通魔的数据B是目标类型;0无目标;1目标单位;2目标点;3目标单位或点;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityDataB(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataB"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能数据B（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的数据B"
        /// comment = ""

        public static float DzGetUnitAbilityDataB(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataB"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能数据C（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的数据C ${DataC} "
        /// comment = "[[通魔的数据C是选项;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityDataC(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataC"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能数据C（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的数据C"
        /// comment = ""

        public static float DzGetUnitAbilityDataC(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataC"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能数据D（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的数据D ${DataD} "
        /// comment = "[[通魔的数据D是动作持续时间;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityDataD(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataD"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能数据D（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的数据D"
        /// comment = ""

        public static float DzGetUnitAbilityDataD(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataD"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能数据E（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的数据E ${DataE} "
        /// comment = "[[通魔的数据E是否使其他技能失效;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityDataE(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataE"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能数据E（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的数据E"
        /// comment = ""

        public static float DzGetUnitAbilityDataE(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataE"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能按钮位置（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的按钮X轴 ${X} , Y轴 ${Y} "
        /// comment = "[[x轴0~3, y轴0~2且 y轴-11可以隐藏技能;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityButtonPos(JUnit Unit, int abil_code, int x, int y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityButtonPos"), Unit.Handle, abil_code, x, y);
        }


        /// title = "技能 - 设置技能快捷键（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的按钮快捷键 ${Hotkey} "
        /// comment = "[[必须显示在按钮上的技能才有效,单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityHotkey(JUnit Unit, int abil_code, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityHotkey"), Unit.Handle, abil_code, key);
        }


        /// title = "转化 - 目标允许整数转字符串"
        /// description = "转换 ${目标允许} 为字符串"
        /// comment = ""

        public static string DzConvertTargs2Str(int targs)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzConvertTargs2Str"), targs);
        }


        /// title = "转化 - 目标允许字符串转整数"
        /// description = "转换 ${目标允许} 为整数"
        /// comment = "例如“ground,friend,vuln,invu”"

        public static int DzConvertStr2Targs(string targs)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzConvertStr2Targs"), targs);
        }


        /// title = "技能 - 设置技能目标允许（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的目标允许 ${Targs} "
        /// comment = "[[单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityTargs(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityTargs"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能目标允许（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的目标允许"
        /// comment = ""

        public static int DzGetUnitAbilityTargs(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityTargs"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能魔法消耗（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的魔法消耗 ${Cost} "
        /// comment = "[[单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityCost(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityCost"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能魔法消耗（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的魔法消耗"
        /// comment = ""

        public static int DzGetUnitAbilityCost(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityCost"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置技能等级要求（通魔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的等级要求 ${ReqLevel} "
        /// comment = "[[2级以上可以无视魔法免疫;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;保证通魔技能有效,其他类型的技能也可以尝试使用不保证有效, 如无效果可以尝试刷新数据]]"

        public static bool DzSetUnitAbilityReqLevel(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityReqLevel"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取技能等级要求（通魔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的等级要求"
        /// comment = ""

        public static int DzGetUnitAbilityReqLevel(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityReqLevel"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置建造技能单位ID（象牙塔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的单位id ${UnitId} "
        /// comment = "[[用在象牙塔或变身类技能;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;]]"

        public static bool DzSetUnitAbilityUnitId(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityUnitId"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取建造技能单位ID（象牙塔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的单位ID"
        /// comment = "象牙塔或者变身类技能有效"

        public static int DzGetUnitAbilityUnitId(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityUnitId"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置建造技能命令ID（象牙塔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的命令id ${OrderId} "
        /// comment = "[[用在象牙塔类的技能;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;]]"

        public static bool DzSetUnitAbilityBuildOrderId(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityBuildOrderId"), Unit.Handle, abil_code, value);
        }


        /// title = "技能 - 获取建造技能命令ID（象牙塔）"
        /// description = "获取 单位 ${unit} 当前拥有的技能 ${id} 的命令ID"
        /// comment = "象牙塔类的技能有效"

        public static int DzGetUnitAbilityBuildOrderId(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityBuildOrderId"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 设置建造技能模型（象牙塔）"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 模型 ${model} 缩放 ${scale} "
        /// comment = "[[修改建造时鼠标模型，不改单位;用在象牙塔类的技能;单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除;]]"

        public static bool DzSetUnitAbilityBuildModel(JUnit Unit, int abil_code, string model_path, float model_scale)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityBuildModel"), Unit.Handle, abil_code, model_path, model_scale);
        }


        /// title = "技能 - 判断单位是否拥有技能 (包含模版技能)"
        /// description = "单位 ${单位} 是否拥有技能 ${技能id} "
        /// comment = "单位拥有指定id 或者 指定模板id, 真实的模板技能id在 编辑器里units\abilitydata.slk 里面的code列里"

        public static bool DzUnitHasAbility(JUnit Unit, int abil_code)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzUnitHasAbility"), Unit.Handle, abil_code);
        }


        /// title = "技能 - 创建技能按钮控件"
        /// description = "创建技能按钮控件"
        /// comment = "创建的技能按钮可以拿来绑定技能。"

        public static int KKCreateCommandButton()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("KKCreateCommandButton"));
        }


        /// title = "技能按钮 - 删除技能按钮"
        /// description = "删除技能按钮 ${技能按钮} "
        /// comment = "[[只能是来自“创建技能按钮控件”的控件,不能删原生哈。]]"

        public static void KKDestroyCommandButton(int btn)
        {
            War3.CallNative(War3.GetNativeFunction("KKDestroyCommandButton"), btn);
        }


        /// title = "技能按钮 - 鼠标点击技能按钮 (无目标施法 或 激活目标指示器)"
        /// description = "点击技能按钮 ${技能按钮} , 按照鼠标 ${鼠标类型} 类型来点击"
        /// comment = "[[鼠标类型1是左键，4是右键，无目标技能左键之后可以释放，目标类技能左键后会激活目标指示器]]"

        public static void KKCommandButtonClick(int btn, int mouse_type)
        {
            War3.CallNative(War3.GetNativeFunction("KKCommandButtonClick"), btn, mouse_type);
        }


        /// title = "技能按钮 - 目标指示器点击目标单位"
        /// description = "鼠标 ${鼠标类型} 类型点击目标 ${单位} "
        /// comment = "[[需要先激活目标指示器后, 鼠标类型1是左键点击目标，4是右键取消]]"

        public static bool KKCommandTargetClick(int mouse_type, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("KKCommandTargetClick"), mouse_type, target.Handle);
        }


        /// title = "技能按钮 - 目标指示器点击地面坐标"
        /// description = "鼠标 ${鼠标类型} 类型点击坐标 x轴 ${X} , y轴 ${Y} , z轴 ${Z} "
        /// comment = "[[坐标类的技能 需要先激活点或范围指示器后, 鼠标类型1是左键点击目标，4是右键取消]]"

        public static bool KKCommandTerrainClick(int mouse_type, float x, float y, float z)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("KKCommandTerrainClick"), mouse_type, x, y, z);
        }


        /// title = "技能按钮 - 绑定单位技能"
        /// description = "技能按钮 ${技能按钮} 绑定单位 ${单位} 的技能 ${技能id} "
        /// comment = "[[需要 先添加单位技能， 然后改按钮y轴-11隐藏, 然后再计时器循环0.1秒绑定, 技能id填0是取消绑定。]]"

        public static void KKSetCommandUnitAbility(int btn, JUnit Unit, int abil_code)
        {
            War3.CallNative(War3.GetNativeFunction("KKSetCommandUnitAbility"), btn, Unit.Handle, abil_code);
        }


        /// title = "物品 - 获取物品颜色"
        /// description = "获取 ${物品} 的颜色"
        /// comment = ""

        public static int DzItemGetVertexColor(JItem Item)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzItemGetVertexColor"), Item.Handle);
        }


        /// title = "物品 - 物品大小"
        /// description = "物品 ${物品} 按照 ${大小} 进行缩放"
        /// comment = "[[]]"

        public static void DzItemSetSize(JItem Item, float size)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetSize"), Item.Handle, size);
        }


        /// title = "物品 - 获取物品大小"
        /// description = "获取 ${物品} 的缩放大小"
        /// comment = ""

        public static float DzItemGetSize(JItem Item)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzItemGetSize"), Item.Handle);
        }


        /// title = "物品 - 模型按照X轴旋转"
        /// description = "物品 ${物品} 按照X轴 ${X} 进行旋转"
        /// comment = "[[多次调用是会乘法累计旋转的, 拾取丢弃物品会重置]]"

        public static void DzItemMatRotateX(JItem Item, float x)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatRotateX"), Item.Handle, x);
        }


        /// title = "物品 - 模型按照Y轴旋转"
        /// description = "物品 ${物品} 按照Y轴 ${Y} 进行旋转"
        /// comment = "[[多次调用是会乘法累计旋转的, 拾取丢弃物品会重置]]"

        public static void DzItemMatRotateY(JItem Item, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatRotateY"), Item.Handle, y);
        }


        /// title = "物品 - 模型按照Z轴旋转"
        /// description = "物品 ${物品} 按照Z轴 ${Z} 进行旋转"
        /// comment = "[[多次调用是会乘法累计旋转的, 拾取丢弃物品会重置]]"

        public static void DzItemMatRotateZ(JItem Item, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatRotateZ"), Item.Handle, z);
        }


        /// title = "物品 - 模型按照XYZ轴缩放"
        /// description = "物品 ${物品} 按照X轴 ${X} ,Y轴 ${Y} ,Z轴 ${Z} 进行缩放"
        /// comment = "[[多次调用是会乘法累计缩放的, 拾取丢弃物品会重置]]"

        public static void DzItemMatScale(JItem Item, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatScale"), Item.Handle, x, y, z);
        }


        /// title = "物品 - 模型重置旋转缩"
        /// description = "物品 ${物品} 模型重置旋转缩"
        /// comment = "[[旋转清零,缩放重置为1]]"

        public static void DzItemMatReset(JItem Item)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatReset"), Item.Handle);
        }


        /// title = "物品 - 当前选择的物品(异步)"
        /// description = "获取主控物品"
        /// comment = "获取的物品是异步的，请谨慎操作"

        public static JItem DzGetLastSelectedItem()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetLastSelectedItem"));
            return new(handle);
        }

        public static void DzSetPariticle2Size(JAgent Widget, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetPariticle2Size"), Widget.Handle, scale);
        }


        /// title = "单位 - 修改单位碰撞体积"
        /// description = "修改单位 ${单位} 的碰撞体积为 ${碰撞体积} "
        /// comment = "[[修改之后移动一下单位或者重新设置一下位置就会刷新了]]"

        public static void DzSetUnitCollisionSize(JUnit Unit, float size)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitCollisionSize"), Unit.Handle, size);
        }


        /// title = "单位 - 获取单位的碰撞体积"
        /// description = "获取 ${单位} 的碰撞体积"
        /// comment = ""

        public static float DzGetUnitCollisionSize(JUnit Unit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitCollisionSize"), Unit.Handle);
        }

        public static void DzSetWidgetTexture(JAgent Handle, string TexturePath, int ReplaceId)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetWidgetTexture"), Handle.Handle, TexturePath, ReplaceId);
        }


        /// title = "单位 - 修改单位选择圈缩放"
        /// description = "修改单位 ${单位} 的选择圈缩放为 ${缩放} "
        /// comment = "[[可以0隐藏或者显示修改指定单位的选择圈大小]]"

        public static void DzSetUnitSelectScale(JUnit Unit, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitSelectScale"), Unit.Handle, scale);
        }


        /// title = "单位 - 设置单位是否忽略点击"
        /// description = "设置单位 ${单位} 的点击球是否忽略 ${忽略} "
        /// comment = "[[true为忽略, false会恢复，删除蝗虫技能后, 隐藏显示单位, 再设置忽略点击false, 关闭打开碰撞 即完美删除蝗虫。]]"

        public static void DzSetUnitHitIgnore(JUnit Unit, bool ignore)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitHitIgnore"), Unit.Handle, ignore);
        }


        /// title = "特效 - 特效绑定特效"
        /// description = "给特效 ${特效} 的附加点 ${附加点} 绑定特效 ${特效} "
        /// comment = ""

        public static void DzEffectBindEffect(JAgent Handle, string AttachName, JEffect eff)
        {
            War3.CallNative(War3.GetNativeFunction("DzEffectBindEffect"), Handle.Handle, AttachName, eff.Handle);
        }


        /// title = "转化 - 整数为技能ID"
        /// description = "转换 ${整数} 为技能ID"
        /// comment = ""

        public static int KKConvertInt2AbilId(int i)
        {
            return i;
        }


        /// title = "转化 - 技能ID为整数"
        /// description = "转换 ${技能ID} 为整数"
        /// comment = ""

        public static int KKConvertAbilId2Int(int i)
        {
            return i;
        }


        /// title = "转化 - 转整数为颜色"
        /// description = "转换 ${整数} 为颜色"
        /// comment = ""

        public static int KKConvertInt2Color(int i)
        {
            return i;
        }


        /// title = "转化 - 转颜色为整数"
        /// description = "转换 ${颜色} 为整数"
        /// comment = ""

        public static int KKConvertColor2Int(int i)
        {
            return i;
        }


        /// title = "界面 - 设置Frame控件忽略点击事件"
        /// description = "设置Frame控件 ${frame} 忽略点击事件为 ${是否} "
        /// comment = "只能用在Frame类型的控件, 对SimpleFrame类型的控件无效, 忽略后可以鼠标穿透地面, 不忽略则会挡住鼠标点击。"

        public static void DzFrameSetIgnoreTrackEvents(int frame, bool ignore)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetIgnoreTrackEvents"), frame, ignore);
        }


        /// title = "界面 - 创建ui模型控件"
        /// description = "创建ui模型控件 指定父控件 ${parent} "
        /// comment = "用来显示3d模型用的，需要手动设置镜头参数，旋转缩放才能正确显示。"

        public static int DzFrameAddModel(int parent_frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameAddModel"), parent_frame);
        }


        /// title = "界面 - ui模型 - 设置模型文件"
        /// description = "设置ui模型控件 ${model_frame} 的文件路径为 ${model_path} , 队伍颜色id为 ${color_id} "
        /// comment = "只能是ui模型控件, 队伍颜色id是0~15 0为红色"

        public static void DzFrameSetModel2(int model_frame, string model_file, int team_color_id)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModel2"), model_frame, model_file, team_color_id);
        }


        /// title = "界面 - ui模型 - 添加绑定特效"
        /// description = "为ui模型控件 ${model_frame} 绑定特效, 附加点 ${attach_point} , 特效模型文件路径 ${model_path} "
        /// comment = "重置模型后自动失效，只能是ui模型控件, 返回effect_frame"

        public static int DzFrameAddModelEffect(int model_frame, string attach_point, string model_file)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameAddModelEffect"), model_frame, attach_point, model_file);
        }


        /// title = "界面 - ui模型 - 移除绑定特效"
        /// description = "为ui模型控件 ${model_frame} 移除绑定的特效 ${effect_frame} "
        /// comment = "effect_frame只能是ui模型添加绑定特效的返回值"

        public static void DzFrameRemoveModelEffect(int model_frame, int effect_frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameRemoveModelEffect"), model_frame, effect_frame);
        }


        /// title = "界面 - ui模型 - 播放动画指定索引"
        /// description = "ui模型控件 ${model_frame} 播放动画指定索引 ${anim_index} "
        /// comment = "可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelAnimationByIndex(int model_frame, int anim_index)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelAnimationByIndex"), model_frame, anim_index);
        }


        /// title = "界面 - ui模型 - 播放动画指定动画名"
        /// description = "ui模型控件 ${model_frame} 播放动画指定动画名 ${animation} "
        /// comment = "可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelAnimation(int model_frame, string animation)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelAnimation"), model_frame, animation);
        }


        /// title = "界面 - ui模型 - 设置场景内镜头源点"
        /// description = "ui模型控件 ${model_frame} 设置镜头源点 x轴 ${x轴} ,y轴 ${y轴} ,z轴 ${z轴} "
        /// comment = "只能是ui模型控件"

        public static void DzFrameSetModelCameraSource(int model_frame, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelCameraSource"), model_frame, x, y, z);
        }


        /// title = "界面 - ui模型 - 设置场景内镜头目标点"
        /// description = "ui模型控件 ${model_frame} 设置镜头目标点 x轴 ${x轴} ,y轴 ${y轴} ,z轴 ${z轴} "
        /// comment = "只能是ui模型控件"

        public static void DzFrameSetModelCameraTarget(int model_frame, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelCameraTarget"), model_frame, x, y, z);
        }


        /// title = "界面 - ui模型 - 设置缩放大小"
        /// description = "ui模型控件 ${model_frame} 设置 缩放 ${size} "
        /// comment = "设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelSize(int model_frame, float size)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelSize"), model_frame, size);
        }


        /// title = "界面 - ui模型 - 获取缩放大小"
        /// description = "获取ui模型控件 ${model_frame} 的缩放大小"
        /// comment = "可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static float DzFrameGetModelSize(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelSize"), model_frame);
        }


        /// title = "界面 - ui模型 - 设置场景内的坐标(X Y Z)"
        /// description = "ui模型控件 ${model_frame} 设置 X轴 ${X轴} , Y轴 ${Y轴} , Z轴 ${Z轴} "
        /// comment = "设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelPosition(int model_frame, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelPosition"), model_frame, x, y, z);
        }


        /// title = "界面 - ui模型 - 设置场景内的坐标X轴"
        /// description = "ui模型控件 ${model_frame} 设置 X轴 ${X轴} "
        /// comment = "设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelX(int model_frame, float x)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelX"), model_frame, x);
        }


        /// title = "界面 - ui模型 - 获取场景内的坐标X轴"
        /// description = "获取ui模型控件 ${model_frame} 场景内的坐标X轴"
        /// comment = "可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static float DzFrameGetModelX(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelX"), model_frame);
        }


        /// title = "界面 - ui模型 - 设置场景内的坐标Y轴"
        /// description = "ui模型控件 ${model_frame} 设置 Y轴 ${Y轴} "
        /// comment = "设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelY(int model_frame, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelY"), model_frame, y);
        }


        /// title = "界面 - ui模型 - 获取场景内的坐标Y轴"
        /// description = "获取ui模型控件 ${model_frame} 场景内的坐标Y轴"
        /// comment = "可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static float DzFrameGetModelY(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelY"), model_frame);
        }


        /// title = "界面 - ui模型 - 设置场景内的坐标Z轴"
        /// description = "ui模型控件 ${model_frame} 设置 Z轴 ${Z轴} "
        /// comment = "设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelZ(int model_frame, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelZ"), model_frame, z);
        }


        /// title = "界面 - ui模型 - 获取场景内的坐标Z轴"
        /// description = "获取ui模型控件 ${model_frame} 场景内的坐标Z轴"
        /// comment = "可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static float DzFrameGetModelZ(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelZ"), model_frame);
        }


        /// title = "界面 - ui模型 - 设置动画播放速度"
        /// description = "ui模型控件 ${model_frame} 设置 动画播放速度 ${倍率} "
        /// comment = "设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelSpeed(int model_frame, float speed)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelSpeed"), model_frame, speed);
        }


        /// title = "界面 - ui模型 - 获取动画播放速度"
        /// description = "获取ui模型控件 ${model_frame} 场景内的动画播放速度"
        /// comment = "可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static float DzFrameGetModelSpeed(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelSpeed"), model_frame);
        }


        /// title = "界面 - ui模型 - 设置矩阵缩放"
        /// description = "ui模型控件 ${model_frame} 设置 矩阵缩放 (X轴 ${x轴} , Y轴 ${y轴} , Z轴 ${z轴})"
        /// comment = "每次调用累计乘法计算缩放, 设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件 "

        public static void DzFrameSetModelScale(int model_frame, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelScale"), model_frame, x, y, z);
        }


        /// title = "界面 - ui模型 - 设置矩阵重置"
        /// description = "ui模型控件 ${model_frame} 设置矩阵重置"
        /// comment = "缩放重置为1, 旋转清零, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelMatReset(int model_frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelMatReset"), model_frame);
        }


        /// title = "界面 - ui模型 - 设置矩阵旋转X轴"
        /// description = "ui模型控件 ${model_frame} 设置矩阵旋转X轴 ${X轴} "
        /// comment = "每次调用累计乘法计算, 不想累计的重置后再设置, 设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelRotateX(int model_frame, float x)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelRotateX"), model_frame, x);
        }


        /// title = "界面 - ui模型 - 设置矩阵旋转Y轴"
        /// description = "ui模型控件 ${model_frame} 设置矩阵旋转Y轴 ${Y轴} "
        /// comment = "每次调用累计乘法计算, 不想累计的重置后再设置, 设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelRotateY(int model_frame, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelRotateY"), model_frame, y);
        }


        /// title = "界面 - ui模型 - 设置矩阵旋转Z轴"
        /// description = "ui模型控件 ${model_frame} 设置矩阵旋转Z轴 ${Z轴} "
        /// comment = "每次调用累计乘法计算, 不想累计的重置后再设置, 设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelRotateZ(int model_frame, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelRotateZ"), model_frame, z);
        }


        /// title = "界面 - ui模型 - 设置模型颜色"
        /// description = "ui模型控件 ${model_frame} 设置模型颜色 ${color} "
        /// comment = "包含透明通道, 设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelColor(int model_frame, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelColor"), model_frame, color);
        }


        /// title = "界面 - ui模型 - 获取颜色"
        /// description = "获取ui模型控件 ${model_frame} 的颜色"
        /// comment = "可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static int DzFrameGetModelColor(int model_frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetModelColor"), model_frame);
        }


        /// title = "界面 - ui模型 - 替换模型id贴图"
        /// description = "ui模型控件 ${model_frame} 设置贴图路径 ${texture_file} , 指定id ${id} "
        /// comment = "id是指模型里指定的纹理id, 设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelTexture(int model_frame, string texture_file, int replace_texutre_id)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelTexture"), model_frame, texture_file, replace_texutre_id);
        }


        /// title = "界面 - ui模型 - 设置粒子2缩放大小"
        /// description = "ui模型控件 ${model_frame} 设置粒子2缩放大小 ${scale} "
        /// comment = "必须是有粒子2的模型, 缩放是乘法计算需要大于0, 设置模型后会重置, 可以是ui模型控件、SPRITE、MODEL类型的控件"

        public static void DzFrameSetModelParticle2Size(int model_frame, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelParticle2Size"), model_frame, scale);
        }


        /// title = "界面 - 获取游戏外界面底层"
        /// description = "获取游戏外界面底层"
        /// comment = "返回GlueUI, 从config房间界面到加载界面完成 都属于GlueUI, 进入游戏后属于GameUI"

        public static int DzGetGlueUI()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetGlueUI"));
        }


        /// title = "界面 - 获取鼠标控件"
        /// description = "获取鼠标控件"
        /// comment = "返回的是SPRITE模型控件，可以通过缩放大小为0来隐藏鼠标, 需要注意的是游戏内跟游戏外鼠标UI不一样，可以游戏开始0秒之后再获取使用。"

        public static int DzFrameGetMouse()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetMouse"));
        }


        /// title = "界面 - 获取控件绑定的整数"
        /// description = "获取控件 ${frame} 绑定的整数"
        /// comment = "相当于获取 <<新建Frame [Tag]:DzCreateFrameByTagName>> 函数最后一个参数"

        public static int DzFrameGetContext(int frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetContext"), frame);
        }


        /// title = "界面 - 获取控件的全局名字"
        /// description = "获取控件 ${frame} 的全局名字"
        /// comment = "相当于获取 <<新建Frame [Tag]:DzCreateFrameByTagName>> 函数第2个参数"

        public static string DzFrameGetName(int frame)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzFrameGetName"), frame);
        }


        /// title = "界面 - 设置控件全局名字跟绑定整数"
        /// description = "设置控件 ${frame} 全局名字 ${name} 绑定整数 ${context} "
        /// comment = "相当于修改 <<新建Frame [Tag]:DzCreateFrameByTagName>> 函数的第2个参数，跟最后一个参数,全局名字不能重复，否则退出游戏时会崩溃，可以使用修改后的名字跟整数查找控件，支持Frame、SimpleFrame、SimpleTexture、SimpleStringFont"

        public static void DzFrameSetNameContext(int frame, string name, int context)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetNameContext"), frame, name, context);
        }


        /// title = "界面 - 设置文本控件字间距"
        /// description = "设置文本控件 ${frame} 设置字间距 ${spacint} "
        /// comment = "只能TEXT类型控件使用"

        public static void DzFrameSetTextFontSpacing(int text_frame, float spacing)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTextFontSpacing"), text_frame, spacing);
        }


        /// title = "界面 - 获取技能/物品按钮的冷却模型控件"
        /// description = "获取技能/物品按钮 ${command_button} 的冷却模型控件"
        /// comment = "获取技能或者物品按钮上面的 冷却模型控件，相当于是SPRITE类型的控件"

        public static int KKCommandGetCooldownModel(int cmd_btn)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("KKCommandGetCooldownModel"), cmd_btn);
        }


        /// title = "界面 - 设置技能/物品按钮的冷却模型缩放大小"
        /// description = "设置技能/物品按钮 ${comand_button} 的冷却模型缩放大小 ${size} "
        /// comment = "只能技能/物品按钮使用, 修改按钮大小后，需要手动缩放一次cd模型的缩放比例"

        public static void KKCommandSetCooldownModelSize(int cmd_btn, float size)
        {
            War3.CallNative(War3.GetNativeFunction("KKCommandSetCooldownModelSize"), cmd_btn, size);
        }


        /// title = "界面 - 设置技能/物品按钮的冷却模型缩放指定宽高比例"
        /// description = "设置技能/物品按钮 ${comand_button} 的冷却模型缩放宽比例 ${width_size} , 高比例 ${height_size} "
        /// comment = "只能技能/物品按钮使用, 修改按钮大小后，需要手动缩放一次cd模型的缩放比例"

        public static void KKCommandSetCooldownModelSize2(int cmd_btn, float width, float height)
        {
            War3.CallNative(War3.GetNativeFunction("KKCommandSetCooldownModelSize2"), cmd_btn, width, height);
        }


        /// title = "物品 - 玩家当前选择的物品(同步)"
        /// description = "获取玩家 ${玩家} 当前选择的物品(同步)"
        /// comment = "返回值是同步的。每次选择物品后会延迟0.1秒刷新返回值。"

        public static JItem DzGetPlayerLastSelectedItem(JPlayer p)
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetPlayerLastSelectedItem"), p.Handle);
            return new(handle);
        }


        /// title = "获取当前缓存模型的数量"
        /// description = "获取当前缓存模型的数量"
        /// comment = "返回值异步的，用来检测当前游戏模型数量用的"

        public static int DzGetCacheModelCount()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetCacheModelCount"));
        }


        /// title = "游戏 - 限制最高帧数"
        /// description = "限制最高帧数 为 ${max_fps} "
        /// comment = "跟解锁上限不同，只能60之内, 例如30帧用来模拟卡顿的游戏环境"

        public static void DzSetMaxFps(int max_fps)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetMaxFps"), max_fps);
        }


        /// title = "允许查看指定单位技能"
        /// description = "允许查看指定单位 ${单位} 的技能, 是否开启 ${enable} "
        /// comment = "开启后可以看友军或敌军单位的技能"

        public static void DzEnableDrawSkillPanel(JUnit u, bool is_enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzEnableDrawSkillPanel"), u.Handle, is_enable);
        }


        /// title = "允许查看指定玩家单位技能"
        /// description = "允许查看指定玩家 ${玩家} 的单位技能, 是否开启 ${enable} "
        /// comment = "开启后可以看友军或敌军单位的技能"

        public static void DzEnableDrawSkillPanelByPlayer(JPlayer p, bool is_enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzEnableDrawSkillPanelByPlayer"), p.Handle, is_enable);
        }


        /// title = "特效 - 设置特效迷雾可见"
        /// description = "设置特效 ${特效} 在迷雾里可见 ${enable} "
        /// comment = "只能对创建到地面的特效使用。迷雾即非视野 非黑色阴影的区域"

        public static void DzSetEffectFogVisible(JEffect eff, bool is_visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectFogVisible"), eff.Handle, is_visible);
        }


        /// title = "特效 - 设置特效黑色阴影可见"
        /// description = "设置特效 ${特效} 在黑色阴影里可见 ${enable} "
        /// comment = "只能对创建到地面的特效使用。 黑色阴影即未解锁的区域"

        public static void DzSetEffectMaskVisible(JEffect eff, bool is_visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectMaskVisible"), eff.Handle, is_visible);
        }


        /// title = "世界坐标 - 绑定Frame到单位实时位置"
        /// description = "绑定Frame ${frame} 到单位 ${unit} 的实时位置, 偏移世界坐标(X ${X} , Y ${Y} , Z ${Z})偏移屏幕坐标(X ${SX} , Y ${SY})战争迷雾可见 ${fog_visible} 有单位视野可见 ${unit_visible} 单位死亡可见 ${dead_visible} "
        /// comment = "绑定后会清除控件锚点, 每帧设置控件中心坐标为 世界坐标+偏移 转屏幕坐标 +偏移后的位置, 超出屏幕，或者不满足条件的情况下会对控件隐藏, 在删除单位，或者删除控件前解除绑定。"

        public static void DzFrameBindWidget(int frame, JWidget u, float world_x, float world_y, float world_z, float screen_x, float screen_y, bool fog_visible, bool unit_visible, bool dead_visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameBindWidget"), frame, u.Handle, world_x, world_y, world_z, screen_x, screen_y, fog_visible, unit_visible, dead_visible);
        }


        /// title = "世界坐标 - 绑定Frame到世界坐标实时位置"
        /// description = "绑定Frame ${frame} 世界坐标(X ${X} , Y ${Y} , Z ${Z})偏移屏幕坐标(X ${SX} , Y ${SY})战争迷雾可见 ${fog_visible} "
        /// comment = "绑定后会清除控件锚点, 每帧设置控件中心坐标为 世界坐标 转屏幕坐标 +偏移后的位置, 超出屏幕，或者不满足条件的情况下会对控件隐藏, 在删除控件前解除绑定。"

        public static void DzFrameBindWorldPos(int frame, float world_x, float world_y, float world_z, float screen_x, float screen_y, bool fog_visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameBindWorldPos"), frame, world_x, world_y, world_z, screen_x, screen_y, fog_visible);
        }


        /// title = "世界坐标 - 解除Frame的绑定"
        /// description = "解除Frame ${frame} 的绑定"
        /// comment = "解除绑定后不会再刷新位置跟改变隐藏显示"

        public static void DzFrameUnBind(int frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameUnBind"), frame);
        }


        /// title = "世界坐标 - 绑定Frame到物品实时位置"
        /// description = "绑定Frame ${frame} 到物品 ${item} 的实时位置, 偏移世界坐标(X ${X} , Y ${Y} , Z ${Z})偏移屏幕坐标(X ${SX} , Y ${SY})战争迷雾可见 ${fog_visible} 物品隐藏时一起隐藏 ${item_visible} "
        /// comment = "绑定后会清除控件锚点, 每帧设置控件中心坐标为 世界坐标+偏移 转屏幕坐标 +偏移后的位置, 超出屏幕，或者不满足条件的情况下会对控件隐藏, 在删除物品，或者删除控件前解除绑定。"

        public static void KKFrameBindItem(int frame, JWidget u, float world_x, float world_y, float world_z, float screen_x, float screen_y, bool fog_visible, bool item_visible)
        {
            DzFrameBindWidget(frame, u, world_x, world_y, world_z, screen_x, screen_y, fog_visible, item_visible, false);
        }


        /// title = "界面 - 屏蔽所有单位指向UI跟血条"
        /// description = "屏蔽所有单位指向UI跟血条"
        /// comment = "屏蔽会保留选择圈，开局调用一次后即可屏蔽所有单位的，以便重写血条"

        public static void DzDisableUnitPreselectUi()
        {
            War3.CallNative(War3.GetNativeFunction("DzDisableUnitPreselectUi"));
        }


        /// title = "界面 - 屏蔽所有物品指向UI"
        /// description = "屏蔽所有物品指向UI"
        /// comment = "屏蔽会保留选择圈，开局调用一次后即可屏蔽所有物品的，以便写物品地面UI"

        public static void DzDisableItemPreselectUi()
        {
            War3.CallNative(War3.GetNativeFunction("DzDisableItemPreselectUi"));
        }


        /// title = "界面 - 获取低于控制台的底层Frame"
        /// description = "获取低于控制台的底层Frame"
        /// comment = "返回FRAME控件，用来创建比控制台大头像更低级的ui, 例如控制台背景, 血条等。"

        public static int DzFrameGetLowerLevelFrame()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetLowerLevelFrame"));
        }


        /// title = "界面 - 设置复选框勾选状态"
        /// description = "设置复选框 ${check_box} 的勾选状态为 ${check} "
        /// comment = "只能对CHECKBOX、GLUECHECKBOX 类型使用"

        public static void DzFrameSetCheckBoxState(int check_box_frame, bool check)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetCheckBoxState"), check_box_frame, check);
        }


        /// title = "界面 - 获取复选框勾选状态"
        /// description = "获取复选框 ${checkbox} 勾选状态"
        /// comment = "只能对CHECKBOX、GLUECHECKBOX 类型使用"

        public static bool DzFrameGetCheckBoxState(int check_box_frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameGetCheckBoxState"), check_box_frame);
        }


        /// title = "界面 - 获取控件是否焦点"
        /// description = "获取控件 ${frame} 是否焦点"
        /// comment = "支持Frame控件使用, 不支持SimpleFrame"

        public static bool DzFrameIsFocus(int frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameIsFocus"), frame);
        }


        /// title = "界面 - 设置编辑框激活状态"
        /// description = "设置编辑框 ${editbox} 激活状态 ${active} "
        /// comment = "true可以主动调用激活焦点的同时激活输入法, false关闭输入法, 只能对EDITBOX、GLUEEDITBOX类型使用"

        public static void DzFrameSetEditBoxActive(int frame, bool is_active)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetEditBoxActive"), frame, is_active);
        }


        /// title = "界面 - 设置编辑框禁用输入法"
        /// description = "设置编辑框 ${editbox} 是否禁用输入法 ${disable} "
        /// comment = "true禁用输入法, 禁用后只能输入英文字母跟数字, 不禁用可以打中文, 只能对EDITBOX、GLUEEDITBOX类型使用"

        public static void DzFrameSetEditBoxDisableIme(int frame, bool is_disable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetEditBoxDisableIme"), frame, is_disable);
        }


        /// title = "硬件 - 当前游戏是否窗口化模式"
        /// description = "当前游戏是否窗口化模式"
        /// comment = "判断启动参数是否带-window 返回值是异步的，因为每个玩家的窗口都不同"

        public static bool DzIsWindowMode()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsWindowMode"));
        }


        /// title = "硬件 - 当前游戏窗口是否活动窗口"
        /// description = "当前游戏窗口是否活动窗口"
        /// comment = "切游戏出去后会返回false, 返回值是异步的，因为每个玩家的窗口都不同"

        public static bool DzIsWindowActive()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsWindowActive"));
        }


        /// title = "硬件 - 设置游戏窗口位置"
        /// description = "设置游戏窗口位置 屏幕X轴 ${X轴} , 屏幕Y轴 ${Y轴} "
        /// comment = "只有窗口模式才有效，屏幕XY轴是指用户屏幕 0,0 为右上角"

        public static void DzWindowSetPoint(int x, int y)
        {
            War3.CallNative(War3.GetNativeFunction("DzWindowSetPoint"), x, y);
        }


        /// title = "硬件 - 设置游戏窗口大小"
        /// description = "设置游戏窗口大小 屏幕宽度 ${width} , 高度 ${height} "
        /// comment = "只有窗口模式才有效, 改变大小之后 可以通过屏幕大小 设置窗口位置来居中"

        public static void DzWindowSetSize(int width, int height)
        {
            War3.CallNative(War3.GetNativeFunction("DzWindowSetSize"), width, height);
        }


        /// title = "硬件 - 获取屏幕设备宽度"
        /// description = "获取屏幕设备宽度"
        /// comment = "返回值是异步的，因为每个玩家都不同"

        public static int DzGetSystemMetricsWidth()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetSystemMetricsWidth"));
        }


        /// title = "硬件 - 获取屏幕设备高度"
        /// description = "获取屏幕设备高度"
        /// comment = "返回值是异步的，因为每个玩家都不同"

        public static int DzGetSystemMetricsHeight()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetSystemMetricsHeight"));
        }


        /// title = "装饰物 - 获取当前地形装饰物数量"
        /// description = "获取当前地形装饰物数量"
        /// comment = "返回值地形上的装饰物数量"

        public static int DzGetDoodadsCount()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetDoodadsCount"));
        }


        /// title = "装饰物 - 设置地形装饰物矩阵缩放"
        /// description = "装饰物 ${doodads} 设置 X轴 ${X轴} , Y轴 ${Y轴} , Z轴 ${Z轴} 缩放"
        /// comment = "每次调用是乘法计算， 需要填大于0的数值，填0会直接导致之后的计算失效"

        public static void DzSetDoodadsMatScale(int doodads_index, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatScale"), doodads_index, x, y, z);
        }


        /// title = "装饰物 - 设置地形装饰物矩阵旋转X轴"
        /// description = "装饰物 ${doodads} 设置 X轴 ${X轴} 旋转"
        /// comment = "每次调用是乘法计算"

        public static void DzSetDoodadsMatRotateX(int doodads_index, float x)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatRotateX"), doodads_index, x);
        }


        /// title = "装饰物 - 设置地形装饰物矩阵旋转Y轴"
        /// description = "装饰物 ${doodads} 设置 Y轴 ${Y轴} 旋转"
        /// comment = "每次调用是乘法计算"

        public static void DzSetDoodadsMatRotateY(int doodads_index, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatRotateY"), doodads_index, y);
        }


        /// title = "装饰物 - 设置装饰物矩阵旋转Z轴"
        /// description = "装饰物 ${doodads} 设置 Z轴 ${Z轴} 旋转"
        /// comment = "每次调用是乘法计算"

        public static void DzSetDoodadsMatRotateZ(int doodads_index, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatRotateZ"), doodads_index, z);
        }


        /// title = "装饰物 - 设置地形装饰物矩阵重置"
        /// description = "装饰物 ${doodads} 矩阵重置"
        /// comment = "将缩放重置为1，将旋转角度重置为0"

        public static void DzSetDoodadsMatReset(int doodads_index)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatReset"), doodads_index);
        }


        /// title = "技能 - 设置技能图标"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的图标为 ${art_path} "
        /// comment = "[[单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityArt(JUnit u, int abil_id, string art_path)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityArt"), u.Handle, abil_id, art_path);
        }


        /// title = "技能 - 获取技能图标"
        /// description = "获取单位 ${单位} 当前的技能 ${技能id} 的技能图标"
        /// comment = "返回当前技能图标"

        public static string DzGetUnitAbilityArt(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilityArt"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能提示"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的提示为 ${tip} "
        /// comment = "[[单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityTip(JUnit u, int abil_id, string tip)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityTip"), u.Handle, abil_id, tip);
        }


        /// title = "技能 - 获取技能提示"
        /// description = "获取单位 ${单位} 当前的技能 ${技能id} 的技能提示"
        /// comment = "返回当前技能提示tip"

        public static string DzGetUnitAbilityTip(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilityTip"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能提示扩展"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的提示扩展为 ${ubertip} "
        /// comment = "[[单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityUberTip(JUnit u, int abil_id, string ubertip)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityUberTip"), u.Handle, abil_id, ubertip);
        }


        /// title = "技能 - 获取技能提示扩展"
        /// description = "获取单位 ${单位} 当前的技能 ${技能id} 的技能提示扩展"
        /// comment = "返回当前技能提示扩展ubertip"

        public static string DzGetUnitAbilityUberTip(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilityUberTip"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置刷新数据"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 数据刷新"
        /// comment = "[[不能异步调用。用来替代升级降级的刷新数据用的, 1级技能也能用。]]"

        public static bool DzSetUnitAbilityUpdate(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityUpdate"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能命令ID"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的命令ID ${order_id} "
        /// comment = "[[可以动态修改大部分技能的命令ID，让同类型技能不冲突。单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityOrderId(JUnit u, int abil_id, int order_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityOrderId"), u.Handle, abil_id, order_id);
        }


        /// title = "技能 - 获取技能命令ID"
        /// description = "获取单位 ${单位} 当前的技能 ${技能id} 的当前的命令ID"
        /// comment = "返回当前使用的命令ID"

        public static int DzGetUnitAbilityOrderId(JUnit u, int abil_id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityOrderId"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置魔法书的技能列表"
        /// description = "设置单位 ${unit} 当前拥有的魔法书技能 ${id} 的技能列表 ${list} 是否保留cd ${save_cool} "
        /// comment = "[[保留cd的话， 同ID技能在修改列表后才能保持cd， 列表超过12个无效, 单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除]]"

        public static bool DzSetUnitAbilitySpellBookList(JUnit u, int abil_id, string abil_list, bool save_cooldown)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilitySpellBookList"), u.Handle, abil_id, abil_list, save_cooldown);
        }


        /// title = "技能 - 获取魔法书的技能列表"
        /// description = "获取单位 ${单位} 的魔法书技能 ${技能id} 的技能列表"
        /// comment = "返回当前魔法书的技能列表"

        public static string DzGetUnitAbilitySpellBookList(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilitySpellBookList"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能投射物模型"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的投射物模型 ${missile_art} "
        /// comment = "[[单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityMissileArt(JUnit u, int abil_id, string missile_art)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileArt"), u.Handle, abil_id, missile_art);
        }


        /// title = "技能 - 获取技能投射物模型"
        /// description = "获取单位 ${单位} 的技能 ${技能id} 的投射物模型"
        /// comment = "返回技能投射物的路径"

        public static string DzGetUnitAbilityMissileArt(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilityMissileArt"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能投射物速度"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的投射物速度 ${missile_speed} "
        /// comment = "[[弹道飞行速度。单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityMissileSpeed(JUnit u, int abil_id, float missile_speed)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileSpeed"), u.Handle, abil_id, missile_speed);
        }


        /// title = "技能 - 获取技能投射物速度"
        /// description = "获取单位 ${单位} 的技能 ${技能id} 的投射物速度"
        /// comment = "返回技能投射物的速度"

        public static float DzGetUnitAbilityMissileSpeed(JUnit u, int abil_id)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMissileSpeed"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能投射物弧度"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的投射物弧度 ${missile_arc} "
        /// comment = "[[抛物线的弧度。单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityMissileArc(JUnit u, int abil_id, float missile_arc)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileArc"), u.Handle, abil_id, missile_arc);
        }


        /// title = "技能 - 获取技能投射物弧度"
        /// description = "获取单位 ${单位} 的技能 ${技能id} 的投射物弧度"
        /// comment = "返回技能投射物的弧度"

        public static float DzGetUnitAbilityMissileArc(JUnit u, int abil_id)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMissileArc"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能投射物允许自导"
        /// description = "设置单位 ${unit} 当前拥有的技能 ${id} 的投射物允许自导 ${missile_homing} "
        /// comment = "[[true相当于发射投射物后，目标单位移动了会持续追踪的意思, false则不追踪会砸到地面。单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityMissileHoming(JUnit u, int abil_id, bool missile_homing)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileHoming"), u.Handle, abil_id, missile_homing);
        }


        /// title = "技能 - 获取技能投射物允许自导"
        /// description = "获取单位 ${单位} 的技能 ${技能id} 的投射物允许自导"
        /// comment = "返回技能投射物是否允许自导"

        public static bool DzGetUnitAbilityMissileHoming(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzGetUnitAbilityMissileHoming"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能投射物数量 (弹幕攻击)"
        /// description = "设置单位 ${unit} 当前拥有的弹幕攻击技能 ${id} 的投射物数量 ${missile_count} "
        /// comment = "[[只对Aroc弹幕攻击技能生效, 修改DataC也能改变数量，但此函数数量更精准。 单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityMissileCount(JUnit u, int abil_id, int missile_count)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileCount"), u.Handle, abil_id, missile_count);
        }


        /// title = "技能 - 获取技能投射物数量 (弹幕攻击)"
        /// description = "获取单位 ${单位} 的弹幕攻击技能 ${技能id} 的投射物数量"
        /// comment = "返回Aroc弹幕攻击的技能投射物数量"

        public static int DzGetUnitAbilityMissileCount(JUnit u, int abil_id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityMissileCount"), u.Handle, abil_id);
        }


        /// title = "技能 - 设置技能投射物伤害 (弹幕攻击)"
        /// description = "设置单位 ${unit} 当前拥有的弹幕攻击技能 ${id} 的投射物单目标伤害 ${damage} 多目标伤害限制 ${max_damage} 攻击类型 ${attacktype} 伤害类型 ${damagetype} "
        /// comment = "[[只对Aroc弹幕攻击技能生效, damage等同修正原物编DataA无效项, max_damage等同修改原物编DataB。该伤害对主目标无效，只对弹幕分裂目标有效。单位的独立修改,不会影响其他单位身上的技能;删除技能后改动即清除, 如无效果刷新技能数据即可]]"

        public static bool DzSetUnitAbilityMissileDamage(JUnit u, int abil_id, float damage, float max_damage, JAttackType atktp, JDamageType dmgtp)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileDamage"), u.Handle, abil_id, damage, max_damage, atktp.Handle, dmgtp.Handle);
        }


        /// title = "技能 - 获取技能投射物伤害 (弹幕攻击)"
        /// description = "获取单位 ${单位} 的弹幕攻击技能 ${技能id} 的投射物伤害"
        /// comment = "返回Aroc弹幕攻击的技能投射物伤害"

        public static float DzGetUnitAbilityMissileDamage(JUnit u, int abil_id)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMissileDamage"), u.Handle, abil_id);
        }


        /// title = "技能 - 获取技能投射物最大伤害 (弹幕攻击)"
        /// description = "获取单位 ${单位} 的弹幕攻击技能 ${技能id} 的投射物最大伤害"
        /// comment = "返回Aroc弹幕攻击的技能投射物最大伤害"

        public static float DzGetUnitAbilityMissileMaxDamage(JUnit u, int abil_id)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMissileMaxDamage"), u.Handle, abil_id);
        }

    }
}
