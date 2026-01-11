using Microsoft.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static War3Frame.Library.Assets;

namespace War3Frame.Library.Api
{
    public static class JassApi
    {
        public static JRace ConvertRace(int i)
        {
            return War3.CallNative<JRace>(War3.GetNativeFunction("ConvertRace"), i);
        }

        public static JAllianceType ConvertAllianceType(int i)
        {
            return War3.CallNative<JAllianceType>(War3.GetNativeFunction("ConvertAllianceType"), i);
        }

        public static JRacePreference ConvertRacePref(int i)
        {
            return War3.CallNative<JRacePreference>(War3.GetNativeFunction("ConvertRacePref"), i);
        }

        public static JIGameState ConvertIGameState(int i)
        {
            return War3.CallNative<JIGameState>(War3.GetNativeFunction("ConvertIGameState"), i);
        }

        public static JFGameState ConvertFGameState(int i)
        {
            return War3.CallNative<JFGameState>(War3.GetNativeFunction("ConvertFGameState"), i);
        }

        public static JPlayerState ConvertPlayerState(int i)
        {
            return War3.CallNative<JPlayerState>(War3.GetNativeFunction("ConvertPlayerState"), i);
        }

        public static JPlayerScore ConvertPlayerScore(int i)
        {
            return War3.CallNative<JPlayerScore>(War3.GetNativeFunction("ConvertPlayerScore"), i);
        }

        public static JPlayerGameResult ConvertPlayerGameResult(int i)
        {
            return War3.CallNative<JPlayerGameResult>(War3.GetNativeFunction("ConvertPlayerGameResult"), i);
        }

        public static JUnitState ConvertUnitState(int i)
        {
            return War3.CallNative<JUnitState>(War3.GetNativeFunction("ConvertUnitState"), i);
        }

        public static JAiDifficulty ConvertAIDifficulty(int i)
        {
            return War3.CallNative<JAiDifficulty>(War3.GetNativeFunction("ConvertAIDifficulty"), i);
        }

        public static JGameEvent ConvertGameEvent(int i)
        {
            return War3.CallNative<JGameEvent>(War3.GetNativeFunction("ConvertGameEvent"), i);
        }

        public static JPlayerEvent ConvertPlayerEvent(int i)
        {
            return War3.CallNative<JPlayerEvent>(War3.GetNativeFunction("ConvertPlayerEvent"), i);
        }

        public static JPlayerUnitEvent ConvertPlayerUnitEvent(int i)
        {
            return War3.CallNative<JPlayerUnitEvent>(War3.GetNativeFunction("ConvertPlayerUnitEvent"), i);
        }

        public static JWidgetEvent ConvertWidgetEvent(int i)
        {
            return War3.CallNative<JWidgetEvent>(War3.GetNativeFunction("ConvertWidgetEvent"), i);
        }

        public static JDialogEvent ConvertDialogEvent(int i)
        {
            return War3.CallNative<JDialogEvent>(War3.GetNativeFunction("ConvertDialogEvent"), i);
        }

        public static JUnitEvent ConvertUnitEvent(int i)
        {
            return War3.CallNative<JUnitEvent>(War3.GetNativeFunction("ConvertUnitEvent"), i);
        }

        public static JLimitOp ConvertLimitOp(int i)
        {
            return War3.CallNative<JLimitOp>(War3.GetNativeFunction("ConvertLimitOp"), i);
        }

        public static JUnitType ConvertUnitType(int i)
        {
            return War3.CallNative<JUnitType>(War3.GetNativeFunction("ConvertUnitType"), i);
        }

        public static JGameSpeed ConvertGameSpeed(int i)
        {
            return War3.CallNative<JGameSpeed>(War3.GetNativeFunction("ConvertGameSpeed"), i);
        }

        public static JPlacement ConvertPlacement(int i)
        {
            return War3.CallNative<JPlacement>(War3.GetNativeFunction("ConvertPlacement"), i);
        }

        public static JStartLocPrio ConvertStartLocPrio(int i)
        {
            return War3.CallNative<JStartLocPrio>(War3.GetNativeFunction("ConvertStartLocPrio"), i);
        }

        public static JGameDifficulty ConvertGameDifficulty(int i)
        {
            return War3.CallNative<JGameDifficulty>(War3.GetNativeFunction("ConvertGameDifficulty"), i);
        }

        public static JGameType ConvertGameType(int i)
        {
            return War3.CallNative<JGameType>(War3.GetNativeFunction("ConvertGameType"), i);
        }

        public static JMapFlag ConvertMapFlag(int i)
        {
            return War3.CallNative<JMapFlag>(War3.GetNativeFunction("ConvertMapFlag"), i);
        }

        public static JMapVisibility ConvertMapVisibility(int i)
        {
            return War3.CallNative<JMapVisibility>(War3.GetNativeFunction("ConvertMapVisibility"), i);
        }

        public static JMapSetting ConvertMapSetting(int i)
        {
            return War3.CallNative<JMapSetting>(War3.GetNativeFunction("ConvertMapSetting"), i);
        }

        public static JMapDensity ConvertMapDensity(int i)
        {
            return War3.CallNative<JMapDensity>(War3.GetNativeFunction("ConvertMapDensity"), i);
        }

        public static JMapControl ConvertMapControl(int i)
        {
            return War3.CallNative<JMapControl>(War3.GetNativeFunction("ConvertMapControl"), i);
        }

        public static JPlayerColor ConvertPlayerColor(int i)
        {
            return War3.CallNative<JPlayerColor>(War3.GetNativeFunction("ConvertPlayerColor"), i);
        }

        public static JPlayerSlotState ConvertPlayerSlotState(int i)
        {
            return War3.CallNative<JPlayerSlotState>(War3.GetNativeFunction("ConvertPlayerSlotState"), i);
        }

        public static JVolumeGroup ConvertVolumeGroup(int i)
        {
            return War3.CallNative<JVolumeGroup>(War3.GetNativeFunction("ConvertVolumeGroup"), i);
        }

        public static JCameraField ConvertCameraField(int i)
        {
            return War3.CallNative<JCameraField>(War3.GetNativeFunction("ConvertCameraField"), i);
        }

        public static JBlendMode ConvertBlendMode(int i)
        {
            return War3.CallNative<JBlendMode>(War3.GetNativeFunction("ConvertBlendMode"), i);
        }

        public static JRarityControl ConvertRarityControl(int i)
        {
            return War3.CallNative<JRarityControl>(War3.GetNativeFunction("ConvertRarityControl"), i);
        }

        public static JTexMapFlags ConvertTexMapFlags(int i)
        {
            return War3.CallNative<JTexMapFlags>(War3.GetNativeFunction("ConvertTexMapFlags"), i);
        }

        public static JFogState ConvertFogState(int i)
        {
            return War3.CallNative<JFogState>(War3.GetNativeFunction("ConvertFogState"), i);
        }

        public static JEffectType ConvertEffectType(int i)
        {
            return War3.CallNative<JEffectType>(War3.GetNativeFunction("ConvertEffectType"), i);
        }

        public static JVersion ConvertVersion(int i)
        {
            return War3.CallNative<JVersion>(War3.GetNativeFunction("ConvertVersion"), i);
        }

        public static JItemType ConvertItemType(int i)
        {
            return War3.CallNative<JItemType>(War3.GetNativeFunction("ConvertItemType"), i);
        }

        public static JAttackType ConvertAttackType(int i)
        {
            return War3.CallNative<JAttackType>(War3.GetNativeFunction("ConvertAttackType"), i);
        }

        public static JDamageType ConvertDamageType(int i)
        {
            return War3.CallNative<JDamageType>(War3.GetNativeFunction("ConvertDamageType"), i);
        }

        public static JWeaponType ConvertWeaponType(int i)
        {
            return War3.CallNative<JWeaponType>(War3.GetNativeFunction("ConvertWeaponType"), i);
        }

        public static JSoundType ConvertSoundType(int i)
        {
            return War3.CallNative<JSoundType>(War3.GetNativeFunction("ConvertSoundType"), i);
        }

        public static JPathingType ConvertPathingType(int i)
        {
            return War3.CallNative<JPathingType>(War3.GetNativeFunction("ConvertPathingType"), i);
        }

        public static int OrderId(string orderIdString)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("OrderId"), orderIdString);
        }

        public static string OrderId2String(int orderId)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("OrderId2String"), orderId);
        }

        public static int UnitId(string unitIdString)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("UnitId"), unitIdString);
        }

        public static string UnitId2String(int unitId)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("UnitId2String"), unitId);
        }

        public static int AbilityId(string abilityIdString)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("AbilityId"), abilityIdString);
        }

        public static string AbilityId2String(int abilityId)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("AbilityId2String"), abilityId);
        }


        /// title = "物体名称 [C]"
        /// description = "${物体ID} 的名称"
        /// comment = "如'A01Z',物体编辑器中物体的名字"

        public static string GetObjectName(int objectId)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetObjectName"), objectId);
        }


        /// title = "转换角度为弧度"
        /// description = "转换角度 ${Degrees} 为弧度"
        /// comment = ""

        public static float Deg2Rad(float degrees)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Deg2Rad"), degrees);
        }


        /// title = "转换弧度为角度"
        /// description = "转换弧度 ${Radians} 为角度"
        /// comment = ""

        public static float Rad2Deg(float radians)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Rad2Deg"), radians);
        }


        /// title = "正弦(弧度) [R]"
        /// description = "Sin(${Angle})"
        /// comment = "采用弧度制计算. "

        public static float Sin(float radians)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Sin"), radians);
        }


        /// title = "余弦(弧度) [R]"
        /// description = "Cos(${Angle})"
        /// comment = "采用弧度制计算. "

        public static float Cos(float radians)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Cos"), radians);
        }


        /// title = "正切(弧度) [R]"
        /// description = "Tan(${Angle})"
        /// comment = "采用弧度制计算. "

        public static float Tan(float radians)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Tan"), radians);
        }


        /// title = "反正弦(弧度) [R]"
        /// description = "Asin(${数值})"
        /// comment = "采用弧度制计算. 返回弧度取值-π/2 — π/2. "

        public static float Asin(float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Asin"), y);
        }


        /// title = "反余弦(弧度) [R]"
        /// description = "Acos(${数值})"
        /// comment = "采用弧度制计算. 返回弧度取值0 — π. "

        public static float Acos(float x)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Acos"), x);
        }


        /// title = "反正切(弧度) [R]"
        /// description = "Atan(${数值})"
        /// comment = "采用弧度制计算. 返回弧度取值-π/2 — π/2. "

        public static float Atan(float x)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Atan"), x);
        }


        /// title = "反正切(Y:X)(弧度) [R]"
        /// description = "Atan(${Y} : ${X})"
        /// comment = "采用弧度制计算. 返回弧度取值-π/2 — π/2. "

        public static float Atan2(float y, float x)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Atan2"), y, x);
        }


        /// title = "平方根"
        /// description = "${实数} 的平方根"
        /// comment = ""

        public static float SquareRoot(float x)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("SquareRoot"), x);
        }


        /// title = "幂运算"
        /// description = "${实数} 的 ${实数} 次幂"
        /// comment = ""

        public static float Pow(float x, float power)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Pow"), x, power);
        }


        /// title = "转换整数为实数"
        /// description = "转换 ${Integer} 为实数"
        /// comment = ""

        public static float I2R(int i)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("I2R"), i);
        }


        /// title = "转换实数为整数"
        /// description = "转换 ${Real} 为整数"
        /// comment = "舍弃小数部分."

        public static int R2I(float r)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("R2I"), r);
        }


        /// title = "转换整数为字符串"
        /// description = "转换 ${Integer} 为字符串"
        /// comment = ""

        public static string I2S(int i)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("I2S"), i);
        }


        /// title = "转换实数为字符串"
        /// description = "转换 ${Real} 为字符串"
        /// comment = ""

        public static string R2S(float r)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("R2S"), r);
        }


        /// title = "格式转换实数为字符串"
        /// description = "转换 ${Real} 为字符串,最小宽度: ${Width} ,小数位数: ${Precision}"
        /// comment = "如: 转换(1.234, 7, 2)后为''   1.23''. 转换(1.234, 2, 5)后为''1.23400''."

        public static string R2SW(float r, int width, int precision)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("R2SW"), r, width, precision);
        }


        /// title = "转换字符串为整数"
        /// description = "转换 ${String} 为整数"
        /// comment = ""

        public static int S2I(string s)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("S2I"), s);
        }


        /// title = "转换字符串为实数"
        /// description = "转换 ${String} 为实数"
        /// comment = ""

        public static float S2R(string s)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("S2R"), s);
        }


        /// title = "<1.24> 获取对象的h2i值 [C]"
        /// description = "转换 ${Handle} 为整数"
        /// comment = "创建一个对应该handle的临时密钥,可以在哈希表中作为索引号使用.当该handle被彻底销毁时,密钥会被回收."

        public static int GetHandleId(JHandle h)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHandleId"), h.Handle);
        }


        /// title = "截取字符串 [R]"
        /// description = "截取 ${字符串} 的 ${Start} - ${End} 字节部分(不包括首字节)"
        /// comment = "例: 截取''Grunts stink''的2 - 4字节部分 = ''un''."

        public static string SubString(string source, int start, int end)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("SubString"), source, start, end);
        }


        /// title = "字符串长度"
        /// description = "${String}的长度"
        /// comment = ""

        public static int StringLength(string s)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("StringLength"), s);
        }


        /// title = "大小写转换"
        /// description = "转换 ${字符串} 为 ${Lower/Upper Case} 形式"
        /// comment = ""

        public static string StringCase(string source, bool upper)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("StringCase"), source, upper);
        }


        /// title = "获取字符串的哈希值"
        /// description = "(${String})的哈希值"
        /// comment = "获取一个对应该字符串的密钥，不同的字符串的密钥基本不可能相同，也很难找到两个不同的字符串他们有着相同的密钥。可以用于制作密码等功能。"

        public static int StringHash(string s)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("StringHash"), s);
        }


        /// title = "本地字符串 [R]"
        /// description = "本地字符串: ${文字}"
        /// comment = "获取ui\\framedef\\globalstrings.fdf中定义的字符串."

        public static string GetLocalizedString(string source)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetLocalizedString"), source);
        }


        /// title = "本地热键 "
        /// description = "本地热键: ${文字}"
        /// comment = "获取ui\\miscui.txt中定义的热键."

        public static int GetLocalizedHotkey(string source)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetLocalizedHotkey"), source);
        }

        public static void SetMapName(string name)
        {
            War3.CallNative(War3.GetNativeFunction("SetMapName"), name);
        }

        public static void SetMapDescription(string description)
        {
            War3.CallNative(War3.GetNativeFunction("SetMapDescription"), description);
        }

        public static void SetTeams(int teamcount)
        {
            War3.CallNative(War3.GetNativeFunction("SetTeams"), teamcount);
        }

        public static void SetPlayers(int playercount)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayers"), playercount);
        }

        public static void DefineStartLocation(int whichStartLoc, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DefineStartLocation"), whichStartLoc, x, y);
        }

        public static void DefineStartLocationLoc(int whichStartLoc, JLocation whichLocation)
        {
            War3.CallNative(War3.GetNativeFunction("DefineStartLocationLoc"), whichStartLoc, whichLocation.Handle);
        }

        public static void SetStartLocPrioCount(int whichStartLoc, int prioSlotCount)
        {
            War3.CallNative(War3.GetNativeFunction("SetStartLocPrioCount"), whichStartLoc, prioSlotCount);
        }

        public static void SetStartLocPrio(int whichStartLoc, int prioSlotIndex, int otherStartLocIndex, JStartLocPrio priority)
        {
            War3.CallNative(War3.GetNativeFunction("SetStartLocPrio"), whichStartLoc, prioSlotIndex, otherStartLocIndex, priority.Handle);
        }

        public static int GetStartLocPrioSlot(int whichStartLoc, int prioSlotIndex)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetStartLocPrioSlot"), whichStartLoc, prioSlotIndex);
        }

        public static JStartLocPrio GetStartLocPrio(int whichStartLoc, int prioSlotIndex)
        {
            return War3.CallNative<JStartLocPrio>(War3.GetNativeFunction("GetStartLocPrio"), whichStartLoc, prioSlotIndex);
        }

        public static void SetGameTypeSupported(JGameType whichGameType, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("SetGameTypeSupported"), whichGameType.Handle, value);
        }


        /// title = "设置地图参数"
        /// description = "设置 ${Map Flag} 为 ${On/Off}"
        /// comment = ""

        public static void SetMapFlag(JMapFlag whichMapFlag, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("SetMapFlag"), whichMapFlag.Handle, value);
        }

        public static void SetGamePlacement(JPlacement whichPlacementType)
        {
            War3.CallNative(War3.GetNativeFunction("SetGamePlacement"), whichPlacementType.Handle);
        }


        /// title = "设定游戏速度"
        /// description = "设定游戏速度为 ${Speed}"
        /// comment = "你可以通过'游戏 - 锁定游戏速度'动作来锁定游戏速度."

        public static void SetGameSpeed(JGameSpeed whichspeed)
        {
            War3.CallNative(War3.GetNativeFunction("SetGameSpeed"), whichspeed.Handle);
        }


        /// title = "设置游戏难度 [R]"
        /// description = "设置当前游戏难度为 ${GameDifficulty}"
        /// comment = "游戏难度只是作为运行AI的一个参考值,没有AI的地图该功能无用."

        public static void SetGameDifficulty(JGameDifficulty whichdifficulty)
        {
            War3.CallNative(War3.GetNativeFunction("SetGameDifficulty"), whichdifficulty.Handle);
        }

        public static void SetResourceDensity(JMapDensity whichdensity)
        {
            War3.CallNative(War3.GetNativeFunction("SetResourceDensity"), whichdensity.Handle);
        }

        public static void SetCreatureDensity(JMapDensity whichdensity)
        {
            War3.CallNative(War3.GetNativeFunction("SetCreatureDensity"), whichdensity.Handle);
        }


        /// title = "队伍数量"
        /// description = "队伍数量"
        /// comment = ""

        public static int GetTeams()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTeams"));
        }


        /// title = "玩家数量"
        /// description = "玩家数量"
        /// comment = "地图编辑器中开启的玩家数量(1-12)."

        public static int GetPlayers()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayers"));
        }

        public static bool IsGameTypeSupported(JGameType whichGameType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsGameTypeSupported"), whichGameType.Handle);
        }

        public static JGameType GetGameTypeSelected()
        {
            return War3.CallNative<JGameType>(War3.GetNativeFunction("GetGameTypeSelected"));
        }


        /// title = "地图参数设置"
        /// description = "${Map Flag} 已设置"
        /// comment = ""

        public static bool IsMapFlagSet(JMapFlag whichMapFlag)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsMapFlagSet"), whichMapFlag.Handle);
        }

        public static JPlacement GetGamePlacement()
        {
            return War3.CallNative<JPlacement>(War3.GetNativeFunction("GetGamePlacement"));
        }


        /// title = "当前游戏速度"
        /// description = "当前游戏速度"
        /// comment = ""

        public static JGameSpeed GetGameSpeed()
        {
            return War3.CallNative<JGameSpeed>(War3.GetNativeFunction("GetGameSpeed"));
        }


        /// title = "当前游戏难度"
        /// description = "当前游戏难度"
        /// comment = ""

        public static JGameDifficulty GetGameDifficulty()
        {
            return War3.CallNative<JGameDifficulty>(War3.GetNativeFunction("GetGameDifficulty"));
        }

        public static JMapDensity GetResourceDensity()
        {
            return War3.CallNative<JMapDensity>(War3.GetNativeFunction("GetResourceDensity"));
        }

        public static JMapDensity GetCreatureDensity()
        {
            return War3.CallNative<JMapDensity>(War3.GetNativeFunction("GetCreatureDensity"));
        }

        public static float GetStartLocationX(int whichStartLocation)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetStartLocationX"), whichStartLocation);
        }

        public static float GetStartLocationY(int whichStartLocation)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetStartLocationY"), whichStartLocation);
        }

        public static JLocation GetStartLocationLoc(int whichStartLocation)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("GetStartLocationLoc"), whichStartLocation);
        }


        /// title = "设置玩家队伍"
        /// description = "设置 ${Player} 的队伍为 ${队伍ID}"
        /// comment = ""

        public static void SetPlayerTeam(JPlayer whichPlayer, int whichTeam)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerTeam"), whichPlayer.Handle, whichTeam);
        }

        public static void SetPlayerStartLocation(JPlayer whichPlayer, int startLocIndex)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerStartLocation"), whichPlayer.Handle, startLocIndex);
        }

        public static void ForcePlayerStartLocation(JPlayer whichPlayer, int startLocIndex)
        {
            War3.CallNative(War3.GetNativeFunction("ForcePlayerStartLocation"), whichPlayer.Handle, startLocIndex);
        }


        /// title = "改变玩家颜色 [R]"
        /// description = "将 ${Player} 的玩家颜色改为 ${Color}"
        /// comment = "不改变现有单位的颜色."

        public static void SetPlayerColor(JPlayer whichPlayer, JPlayerColor color)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerColor"), whichPlayer.Handle, color.Handle);
        }


        /// title = "设置联盟状态(指定项目) [R]"
        /// description = "命令 ${Player} 对 ${Player} 设置 ${Alliance Type} ${On/Off}"
        /// comment = "注意:可以对玩家自己设置联盟状态. 可用来实现一些特殊效果."

        public static void SetPlayerAlliance(JPlayer sourcePlayer, JPlayer otherPlayer, JAllianceType whichAllianceSetting, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerAlliance"), sourcePlayer.Handle, otherPlayer.Handle, whichAllianceSetting.Handle, value);
        }


        /// title = "设置税率 [R]"
        /// description = "设置 ${Player} 交纳给 ${Player} 的 ${Resource} 所得税为 ${Rate} %"
        /// comment = "缴纳所得税所损失的资源可以通过'玩家得分'的'税务损失的黄金/木材'来获取. 所得税最高为100%. 且玩家1对玩家2和玩家3都交纳80%所得税.则玩家1采集黄金时将给玩家2 8黄金,玩家3 2黄金."

        public static void SetPlayerTaxRate(JPlayer sourcePlayer, JPlayer otherPlayer, JPlayerState whichResource, int rate)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerTaxRate"), sourcePlayer.Handle, otherPlayer.Handle, whichResource.Handle, rate);
        }

        public static void SetPlayerRacePreference(JPlayer whichPlayer, JRacePreference whichRacePreference)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerRacePreference"), whichPlayer.Handle, whichRacePreference.Handle);
        }

        public static void SetPlayerRaceSelectable(JPlayer whichPlayer, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerRaceSelectable"), whichPlayer.Handle, value);
        }

        public static void SetPlayerController(JPlayer whichPlayer, JMapControl controlType)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerController"), whichPlayer.Handle, controlType.Handle);
        }


        /// title = "更改名字"
        /// description = "更改 ${Player} 的名字为 ${文字}"
        /// comment = ""

        public static void SetPlayerName(JPlayer whichPlayer, string name)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerName"), whichPlayer.Handle, name);
        }


        /// title = "显示/隐藏计分屏显示 [R]"
        /// description = "设置 ${Player} ${Show/Hide} 在计分屏的显示."
        /// comment = ""

        public static void SetPlayerOnScoreScreen(JPlayer whichPlayer, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerOnScoreScreen"), whichPlayer.Handle, flag);
        }


        /// title = "玩家队伍"
        /// description = "${Player} 所属队伍编号"
        /// comment = ""

        public static int GetPlayerTeam(JPlayer whichPlayer)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerTeam"), whichPlayer.Handle);
        }

        public static int GetPlayerStartLocation(JPlayer whichPlayer)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerStartLocation"), whichPlayer.Handle);
        }


        /// title = "玩家颜色"
        /// description = "${Player} 的颜色"
        /// comment = ""

        public static JPlayerColor GetPlayerColor(JPlayer whichPlayer)
        {
            return War3.CallNative<JPlayerColor>(War3.GetNativeFunction("GetPlayerColor"), whichPlayer.Handle);
        }

        public static bool GetPlayerSelectable(JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetPlayerSelectable"), whichPlayer.Handle);
        }


        /// title = "玩家控制者"
        /// description = "${Player} 的控制者"
        /// comment = ""

        public static JMapControl GetPlayerController(JPlayer whichPlayer)
        {
            return War3.CallNative<JMapControl>(War3.GetNativeFunction("GetPlayerController"), whichPlayer.Handle);
        }


        /// title = "玩家游戏状态"
        /// description = "${Player} 的游戏状态"
        /// comment = ""

        public static JPlayerSlotState GetPlayerSlotState(JPlayer whichPlayer)
        {
            return War3.CallNative<JPlayerSlotState>(War3.GetNativeFunction("GetPlayerSlotState"), whichPlayer.Handle);
        }


        /// title = "玩家税率 [R]"
        /// description = "${Player} 需要交纳给 ${Player} 的 ${Resource} 所得税"
        /// comment = "所得税取值范围0-100."

        public static int GetPlayerTaxRate(JPlayer sourcePlayer, JPlayer otherPlayer, JPlayerState whichResource)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerTaxRate"), sourcePlayer.Handle, otherPlayer.Handle, whichResource.Handle);
        }


        /// title = "玩家的种族选择"
        /// description = "${Player} 选择了 ${RacePreference}"
        /// comment = ""

        public static bool IsPlayerRacePrefSet(JPlayer whichPlayer, JRacePreference pref)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPlayerRacePrefSet"), whichPlayer.Handle, pref.Handle);
        }


        /// title = "玩家名字"
        /// description = "${Player} 的名字"
        /// comment = ""

        public static string GetPlayerName(JPlayer whichPlayer)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetPlayerName"), whichPlayer.Handle);
        }


        /// title = "新建计时器 [R]"
        /// description = "新建的计时器"
        /// comment = "会创建计时器."

        public static JTimer CreateTimer()
        {
            return War3.CallNative<JTimer>(War3.GetNativeFunction("CreateTimer"));
        }


        /// title = "删除计时器 [R]"
        /// description = "删除 ${计时器}"
        /// comment = "一般来说,计时器并不需要删除.只为某些有特别需求的用户提供."

        public static void DestroyTimer(JTimer whichTimer)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyTimer"), whichTimer.Handle);
        }


        /// title = "运行计时器 [C]"
        /// description = "运行 ${计时器}，周期: ${时间} 秒，模式: ${模式}，运行函数: ${函数}"
        /// comment = "等同于TimerStart"

        public static void TimerStart(JTimer whichTimer, float timeout, bool periodic, JCode handlerFunc)
        {
            War3.CallNative(War3.GetNativeFunction("TimerStart"), whichTimer.Handle, timeout, periodic, handlerFunc);
        }


        /// title = "逝去时间"
        /// description = "${计时器} 的逝去时间"
        /// comment = ""

        public static float TimerGetElapsed(JTimer whichTimer)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("TimerGetElapsed"), whichTimer.Handle);
        }


        /// title = "剩余时间"
        /// description = "${计时器} 的剩余时间"
        /// comment = ""

        public static float TimerGetRemaining(JTimer whichTimer)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("TimerGetRemaining"), whichTimer.Handle);
        }


        /// title = "设置时间"
        /// description = "${计时器} 设置的时间"
        /// comment = ""

        public static float TimerGetTimeout(JTimer whichTimer)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("TimerGetTimeout"), whichTimer.Handle);
        }


        /// title = "暂停计时器 [R]"
        /// description = "暂停 ${计时器}"
        /// comment = ""

        public static void PauseTimer(JTimer whichTimer)
        {
            War3.CallNative(War3.GetNativeFunction("PauseTimer"), whichTimer.Handle);
        }


        /// title = "恢复计时器 [R]"
        /// description = "恢复 ${计时器}"
        /// comment = ""

        public static void ResumeTimer(JTimer whichTimer)
        {
            War3.CallNative(War3.GetNativeFunction("ResumeTimer"), whichTimer.Handle);
        }


        /// title = "到期的计时器"
        /// description = "到期的计时器"
        /// comment = "响应'时间 - 计时器到期'事件."

        public static JTimer GetExpiredTimer()
        {
            return War3.CallNative<JTimer>(War3.GetNativeFunction("GetExpiredTimer"));
        }


        /// title = "新建的单位组 [R]"
        /// description = "新建的空单位组"
        /// comment = "会创建单位组."

        public static JGroup CreateGroup()
        {
            return War3.CallNative<JGroup>(War3.GetNativeFunction("CreateGroup"));
        }


        /// title = "删除单位组 [R]"
        /// description = "删除 ${单位组}"
        /// comment = ""

        public static void DestroyGroup(JGroup whichGroup)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyGroup"), whichGroup.Handle);
        }


        /// title = "添加单位 [R]"
        /// description = "为 ${单位组} 添加 ${单位}"
        /// comment = "并不影响单位本身."

        public static void GroupAddUnit(JGroup whichGroup, JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupAddUnit"), whichGroup.Handle, whichUnit.Handle);
        }


        /// title = "移除单位 [R]"
        /// description = "为 ${单位组} 删除 ${单位}"
        /// comment = "并不影响单位本身."

        public static void GroupRemoveUnit(JGroup whichGroup, JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupRemoveUnit"), whichGroup.Handle, whichUnit.Handle);
        }


        /// title = "清空单位组"
        /// description = "清空 ${单位组} 内所有单位"
        /// comment = "并不影响单位本身."

        public static void GroupClear(JGroup whichGroup)
        {
            War3.CallNative(War3.GetNativeFunction("GroupClear"), whichGroup.Handle);
        }

        public static void GroupEnumUnitsOfType(JGroup whichGroup, string unitname, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsOfType"), whichGroup.Handle, unitname, filter.Handle);
        }

        public static void GroupEnumUnitsOfPlayer(JGroup whichGroup, JPlayer whichPlayer, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsOfPlayer"), whichGroup.Handle, whichPlayer.Handle, filter.Handle);
        }

        public static void GroupEnumUnitsOfTypeCounted(JGroup whichGroup, string unitname, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsOfTypeCounted"), whichGroup.Handle, unitname, filter.Handle, countLimit);
        }

        public static void GroupEnumUnitsInRect(JGroup whichGroup, JRect r, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRect"), whichGroup.Handle, r.Handle, filter.Handle);
        }

        public static void GroupEnumUnitsInRectCounted(JGroup whichGroup, JRect r, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRectCounted"), whichGroup.Handle, r.Handle, filter.Handle, countLimit);
        }


        /// title = "选取单位添加到单位组(坐标)"
        /// description = "为 ${单位组} 添加以( ${坐标X} , ${坐标Y} )为圆心，${半径} 为半径的圆范围内，满足 ${条件} 的单位"
        /// comment = ""

        public static void GroupEnumUnitsInRange(JGroup whichGroup, float x, float y, float radius, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRange"), whichGroup.Handle, x, y, radius, filter.Handle);
        }


        /// title = "选取单位添加到单位组(点)"
        /// description = "为 ${单位组} 添加以 ${点} 为圆心，${半径} 为半径的圆范围内，满足 ${条件} 的单位"
        /// comment = ""

        public static void GroupEnumUnitsInRangeOfLoc(JGroup whichGroup, JLocation whichLocation, float radius, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRangeOfLoc"), whichGroup.Handle, whichLocation.Handle, radius, filter.Handle);
        }


        /// title = "选取单位添加到单位组(坐标)(不建议使用)"
        /// description = "为 ${单位组} 添加以( ${坐标X} , ${坐标Y} )为圆心，${半径} 为半径的圆范围内，满足 ${条件} 的单位。无效项( ${N} )"
        /// comment = "最后一项是无效项，建议用上一个UI"

        public static void GroupEnumUnitsInRangeCounted(JGroup whichGroup, float x, float y, float radius, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRangeCounted"), whichGroup.Handle, x, y, radius, filter.Handle, countLimit);
        }


        /// title = "选取单位添加到单位组(点)(不建议使用)"
        /// description = "为 ${单位组} 添加以 ${点} 为圆心，${半径} 为半径的圆范围内，满足 ${条件} 的单位。无效项( ${N} )"
        /// comment = "最后一项是无效项，建议用上一个UI"

        public static void GroupEnumUnitsInRangeOfLocCounted(JGroup whichGroup, JLocation whichLocation, float radius, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRangeOfLocCounted"), whichGroup.Handle, whichLocation.Handle, radius, filter.Handle, countLimit);
        }

        public static void GroupEnumUnitsSelected(JGroup whichGroup, JPlayer whichPlayer, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsSelected"), whichGroup.Handle, whichPlayer.Handle, filter.Handle);
        }


        /// title = "发布命令(无目标)"
        /// description = "对 ${单位组}发布 ${Order}"
        /// comment = "最多只能对单位组中12个单位发布命令."

        public static bool GroupImmediateOrder(JGroup whichGroup, string order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupImmediateOrder"), whichGroup.Handle, order);
        }


        /// title = "发布命令(无目标)(ID)"
        /// description = "对 ${单位组}发布 ${Order}"
        /// comment = "最多只能对单位组中12个单位发布命令."

        public static bool GroupImmediateOrderById(JGroup whichGroup, int order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupImmediateOrderById"), whichGroup.Handle, order);
        }


        /// title = "发布命令(指定坐标) [R]"
        /// description = "对 ${单位组}发布 ${Order} 命令,目标点:(${X},${Y})"
        /// comment = "最多只能对单位组中12个单位发布命令."

        public static bool GroupPointOrder(JGroup whichGroup, string order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupPointOrder"), whichGroup.Handle, order, x, y);
        }


        /// title = "发布命令(指定点)"
        /// description = "对 ${单位组}发布 ${Order} 命令,目标: ${指定点}"
        /// comment = "最多只能对单位组中12个单位发布命令."

        public static bool GroupPointOrderLoc(JGroup whichGroup, string order, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupPointOrderLoc"), whichGroup.Handle, order, whichLocation.Handle);
        }


        /// title = "发布命令(指定坐标)(ID)"
        /// description = "对 ${单位组}发布 ${Order} 命令,目标点:(${X},${Y})"
        /// comment = "最多只能对单位组中12个单位发布命令."

        public static bool GroupPointOrderById(JGroup whichGroup, int order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupPointOrderById"), whichGroup.Handle, order, x, y);
        }


        /// title = "发布命令(指定点)(ID)"
        /// description = "对 ${单位组}发布 ${Order} 命令,目标: ${指定点}"
        /// comment = "最多只能对单位组中12个单位发布命令."

        public static bool GroupPointOrderByIdLoc(JGroup whichGroup, int order, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupPointOrderByIdLoc"), whichGroup.Handle, order, whichLocation.Handle);
        }


        /// title = "发布命令(指定单位)"
        /// description = "对 ${单位组} 发布 ${Order} 命令,目标: ${单位}"
        /// comment = "最多只能对单位组中12个单位发布命令."

        public static bool GroupTargetOrder(JGroup whichGroup, string order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupTargetOrder"), whichGroup.Handle, order, targetWidget.Handle);
        }


        /// title = "发布命令(指定单位)(ID)"
        /// description = "对 ${单位组} 发布 ${Order} 命令,目标: ${单位}"
        /// comment = "最多只能对单位组中12个单位发布命令."

        public static bool GroupTargetOrderById(JGroup whichGroup, int order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupTargetOrderById"), whichGroup.Handle, order, targetWidget.Handle);
        }


        /// title = "选取单位组内单位做动作"
        /// description = "选取 ${单位组} 内所有单位 ${做动作}"
        /// comment = "使用'选取单位'来取代相应的单位. 对于单位组内每个单位都会运行一次动作(包括死亡的,不包括隐藏的). 等待不能在组动作中运行."

        public static void ForGroup(JGroup whichGroup, JCode callback)
        {
            War3.CallNative(War3.GetNativeFunction("ForGroup"), whichGroup.Handle, callback);
        }


        /// title = "单位组中第一个单位"
        /// description = "${单位组} 中第一个单位"

        public static JUnit FirstOfGroup(JGroup whichGroup)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("FirstOfGroup"), whichGroup.Handle);
        }


        /// title = "新建玩家组 [R]"
        /// description = "新建空玩家组"
        /// comment = "会创建玩家组."

        public static JForce CreateForce()
        {
            return War3.CallNative<JForce>(War3.GetNativeFunction("CreateForce"));
        }


        /// title = "删除玩家组 [R]"
        /// description = "删除 ${玩家组}"
        /// comment = "注意: 不要删除系统预置的玩家组."

        public static void DestroyForce(JForce whichForce)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyForce"), whichForce.Handle);
        }


        /// title = "添加玩家 [R]"
        /// description = " ${玩家组} 添加 ${玩家}"
        /// comment = "并不影响玩家本身."

        public static void ForceAddPlayer(JForce whichForce, JPlayer whichPlayer)
        {
            War3.CallNative(War3.GetNativeFunction("ForceAddPlayer"), whichForce.Handle, whichPlayer.Handle);
        }


        /// title = "移除玩家 [R]"
        /// description = "从 ${玩家组} 中移除 ${玩家}"
        /// comment = "并不影响玩家本身."

        public static void ForceRemovePlayer(JForce whichForce, JPlayer whichPlayer)
        {
            War3.CallNative(War3.GetNativeFunction("ForceRemovePlayer"), whichForce.Handle, whichPlayer.Handle);
        }


        /// title = "清空玩家组"
        /// description = "清空 ${玩家组} 内所有玩家"
        /// comment = "并不影响玩家本身."

        public static void ForceClear(JForce whichForce)
        {
            War3.CallNative(War3.GetNativeFunction("ForceClear"), whichForce.Handle);
        }

        public static void ForceEnumPlayers(JForce whichForce, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("ForceEnumPlayers"), whichForce.Handle, filter.Handle);
        }

        public static void ForceEnumPlayersCounted(JForce whichForce, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("ForceEnumPlayersCounted"), whichForce.Handle, filter.Handle, countLimit);
        }

        public static void ForceEnumAllies(JForce whichForce, JPlayer whichPlayer, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("ForceEnumAllies"), whichForce.Handle, whichPlayer.Handle, filter.Handle);
        }

        public static void ForceEnumEnemies(JForce whichForce, JPlayer whichPlayer, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("ForceEnumEnemies"), whichForce.Handle, whichPlayer.Handle, filter.Handle);
        }


        /// title = "选取玩家组内玩家做动作"
        /// description = "选取 ${玩家组} 内所有玩家 ${做动作}"
        /// comment = "玩家组动作中可使用'选取玩家'来获取对应的玩家. 等待不能在组动作中运行."

        public static void ForForce(JForce whichForce, JCode callback)
        {
            War3.CallNative(War3.GetNativeFunction("ForForce"), whichForce.Handle, callback);
        }


        /// title = "新建矩形区域(指定边角坐标)"
        /// description = "左下角为(${X1}, ${Y1}),右上角为(${X2}, ${Y2})的矩形区域"
        /// comment = "会创建矩形区域."

        public static JRect Rect(float minx, float miny, float maxx, float maxy)
        {
            return War3.CallNative<JRect>(War3.GetNativeFunction("Rect"), minx, miny, maxx, maxy);
        }


        /// title = "新建矩形区域(指定边角点)"
        /// description = "左下角为 ${点1} ,右上角为 ${点2} 的矩形区域"
        /// comment = "会创建矩形区域."

        public static JRect RectFromLoc(JLocation min, JLocation max)
        {
            return War3.CallNative<JRect>(War3.GetNativeFunction("RectFromLoc"), min.Handle, max.Handle);
        }


        /// title = "删除矩形区域 [R]"
        /// description = "删除 ${矩形区域}"
        /// comment = ""

        public static void RemoveRect(JRect whichRect)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveRect"), whichRect.Handle);
        }


        /// title = "设置矩形区域(指定坐标) [R]"
        /// description = "重新设置 ${矩形区域} ,左下角坐标为(${X},${Y}), 右上角坐标为(${X},${Y})"
        /// comment = "该区域必须是一个变量. 重新设置矩形区域的大小和位置."

        public static void SetRect(JRect whichRect, float minx, float miny, float maxx, float maxy)
        {
            War3.CallNative(War3.GetNativeFunction("SetRect"), whichRect.Handle, minx, miny, maxx, maxy);
        }


        /// title = "设置矩形区域(指定点) [R]"
        /// description = "重新设置 ${矩形区域} ,左下角点为 ${点} 右上角点为 ${点}"
        /// comment = "该区域必须是一个变量. 重新设置矩形区域的大小和位置."

        public static void SetRectFromLoc(JRect whichRect, JLocation min, JLocation max)
        {
            War3.CallNative(War3.GetNativeFunction("SetRectFromLoc"), whichRect.Handle, min.Handle, max.Handle);
        }


        /// title = "移动矩形区域(指定坐标) [R]"
        /// description = "移动 ${矩形区域} 到(${X},${Y})"
        /// comment = "该区域必须是一个变量. 目标点将作为该区域的新中心点."

        public static void MoveRectTo(JRect whichRect, float newCenterX, float newCenterY)
        {
            War3.CallNative(War3.GetNativeFunction("MoveRectTo"), whichRect.Handle, newCenterX, newCenterY);
        }


        /// title = "移动矩形区域(指定点)"
        /// description = "移动 ${矩形区域} 到 ${目标点}"
        /// comment = "该区域必须是一个变量. 目标点将作为该区域的新中心点."

        public static void MoveRectToLoc(JRect whichRect, JLocation newCenterLoc)
        {
            War3.CallNative(War3.GetNativeFunction("MoveRectToLoc"), whichRect.Handle, newCenterLoc.Handle);
        }


        /// title = "中心X坐标"
        /// description = "${矩形区域} 的中心X坐标"
        /// comment = ""

        public static float GetRectCenterX(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectCenterX"), whichRect.Handle);
        }


        /// title = "中心Y坐标"
        /// description = "${矩形区域} 的中心Y坐标"
        /// comment = ""

        public static float GetRectCenterY(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectCenterY"), whichRect.Handle);
        }


        /// title = "左下角X坐标"
        /// description = "${矩形区域} 的左下角X坐标"
        /// comment = ""

        public static float GetRectMinX(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectMinX"), whichRect.Handle);
        }


        /// title = "左下角Y坐标"
        /// description = "${矩形区域} 的左下角Y坐标"
        /// comment = ""

        public static float GetRectMinY(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectMinY"), whichRect.Handle);
        }


        /// title = "右上角X坐标"
        /// description = "${矩形区域} 的右上角X坐标"
        /// comment = ""

        public static float GetRectMaxX(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectMaxX"), whichRect.Handle);
        }


        /// title = "右上角Y坐标"
        /// description = "${矩形区域} 的右上角Y坐标"
        /// comment = ""

        public static float GetRectMaxY(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectMaxY"), whichRect.Handle);
        }


        /// title = "新建区域 [R]"
        /// description = "新建区域"
        /// comment = "会创建一个新的不规则区域,如果不往该区域添加点或地区,则该区域无效果."

        public static JRegion CreateRegion()
        {
            return War3.CallNative<JRegion>(War3.GetNativeFunction("CreateRegion"));
        }


        /// title = "删除不规则区域 [R]"
        /// description = "删除 ${不规则区域}"
        /// comment = ""

        public static void RemoveRegion(JRegion whichRegion)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveRegion"), whichRegion.Handle);
        }


        /// title = "添加区域 [R]"
        /// description = "对 ${不规则区域} 添加 ${矩形区域}"
        /// comment = "区域是游戏中一个游戏地区的集合体,可以包含地区和点."

        public static void RegionAddRect(JRegion whichRegion, JRect r)
        {
            War3.CallNative(War3.GetNativeFunction("RegionAddRect"), whichRegion.Handle, r.Handle);
        }


        /// title = "移除区域 [R]"
        /// description = "在 ${不规则区域} 中移除 ${矩形区域}"
        /// comment = ""

        public static void RegionClearRect(JRegion whichRegion, JRect r)
        {
            War3.CallNative(War3.GetNativeFunction("RegionClearRect"), whichRegion.Handle, r.Handle);
        }


        /// title = "添加单元点(指定坐标) [R]"
        /// description = "对 ${不规则区域} 添加单元点: (${X},${Y})"
        /// comment = "单元点大小为32x32."

        public static void RegionAddCell(JRegion whichRegion, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("RegionAddCell"), whichRegion.Handle, x, y);
        }


        /// title = "添加单元点(指定点) [R]"
        /// description = "对 ${不规则区域} 添加单元点: ${点}"
        /// comment = "单元点大小为32x32."

        public static void RegionAddCellAtLoc(JRegion whichRegion, JLocation whichLocation)
        {
            War3.CallNative(War3.GetNativeFunction("RegionAddCellAtLoc"), whichRegion.Handle, whichLocation.Handle);
        }


        /// title = "移除单元点(指定坐标) [R]"
        /// description = "在 ${不规则区域} 中移除单元点: (${X},${Y})"
        /// comment = "单元点大小为32x32."

        public static void RegionClearCell(JRegion whichRegion, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("RegionClearCell"), whichRegion.Handle, x, y);
        }


        /// title = "移除单元点(指定点) [R]"
        /// description = "在 ${不规则区域} 中移除单元点: ${点}"
        /// comment = "单元点大小为32x32."

        public static void RegionClearCellAtLoc(JRegion whichRegion, JLocation whichLocation)
        {
            War3.CallNative(War3.GetNativeFunction("RegionClearCellAtLoc"), whichRegion.Handle, whichLocation.Handle);
        }


        /// title = "坐标点"
        /// description = "坐标(${X}, ${Y})"
        /// comment = "会创建点."

        public static JLocation Location(float x, float y)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("Location"), x, y);
        }


        /// title = "清除点 [R]"
        /// description = "清除 ${点}"
        /// comment = "点是堆积最多的垃圾资源,不需要再使用的点都要记得清除掉."

        public static void RemoveLocation(JLocation whichLocation)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveLocation"), whichLocation.Handle);
        }


        /// title = "移动点 [R]"
        /// description = "移动 ${点} 到(${X},${Y})"
        /// comment = "该点必须是一个变量. 因为移动一个不可重用的点是无意义的."

        public static void MoveLocation(JLocation whichLocation, float newX, float newY)
        {
            War3.CallNative(War3.GetNativeFunction("MoveLocation"), whichLocation.Handle, newX, newY);
        }


        /// title = "点的X轴坐标"
        /// description = "${点} 的X轴坐标"
        /// comment = ""

        public static float GetLocationX(JLocation whichLocation)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLocationX"), whichLocation.Handle);
        }


        /// title = "点的Y轴坐标"
        /// description = "${点} 的Y轴坐标"
        /// comment = ""

        public static float GetLocationY(JLocation whichLocation)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLocationY"), whichLocation.Handle);
        }


        /// title = "点的Z轴高度 [R]"
        /// description = "${点} 的Z轴高度"
        /// comment = ""

        public static float GetLocationZ(JLocation whichLocation)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLocationZ"), whichLocation.Handle);
        }


        /// title = "在不规则区域内 [R]"
        /// description = "${不规则区域} 内存在 ${单位}"
        /// comment = ""

        public static bool IsUnitInRegion(JRegion whichRegion, JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitInRegion"), whichRegion.Handle, whichUnit.Handle);
        }


        /// title = "包含坐标"
        /// description = "${不规则区域} 内包含坐标(${X},${Y})"
        /// comment = "TC_REGION"

        public static bool IsPointInRegion(JRegion whichRegion, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPointInRegion"), whichRegion.Handle, x, y);
        }


        /// title = "包含点"
        /// description = "${不规则区域} 内包含点: ${点}"
        /// comment = "TC_REGION"

        public static bool IsLocationInRegion(JRegion whichRegion, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsLocationInRegion"), whichRegion.Handle, whichLocation.Handle);
        }

        public static JRect GetWorldBounds()
        {
            return War3.CallNative<JRect>(War3.GetNativeFunction("GetWorldBounds"));
        }


        /// title = "新建触发 [R]"
        /// description = "新建触发"
        /// comment = "会创建一个新的触发器,如果对该功能不熟悉请慎用."

        public static JTrigger CreateTrigger()
        {
            return War3.CallNative<JTrigger>(War3.GetNativeFunction("CreateTrigger"));
        }


        /// title = "删除触发器 [R]"
        /// description = "删除 ${触发器}"
        /// comment = "对不再使用的触发器可以使用该动作来删除."

        public static void DestroyTrigger(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyTrigger"), whichTrigger.Handle);
        }

        public static void ResetTrigger(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("ResetTrigger"), whichTrigger.Handle);
        }


        /// title = "开启触发"
        /// description = "开启 ${Trigger}"
        /// comment = ""

        public static void EnableTrigger(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("EnableTrigger"), whichTrigger.Handle);
        }


        /// title = "关闭触发"
        /// description = "关闭 ${Trigger}"
        /// comment = ""

        public static void DisableTrigger(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("DisableTrigger"), whichTrigger.Handle);
        }


        /// title = "触发开启"
        /// description = "${触发} 处于开启状态"
        /// comment = ""

        public static bool IsTriggerEnabled(JTrigger whichTrigger)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsTriggerEnabled"), whichTrigger.Handle);
        }

        public static void TriggerWaitOnSleeps(JTrigger whichTrigger, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerWaitOnSleeps"), whichTrigger.Handle, flag);
        }

        public static bool IsTriggerWaitOnSleeps(JTrigger whichTrigger)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsTriggerWaitOnSleeps"), whichTrigger.Handle);
        }


        /// title = "匹配单位"
        /// description = "匹配单位"
        /// comment = "在'选取单位满足条件'之类功能的条件中,指代被判断单位."

        public static JUnit GetFilterUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetFilterUnit"));
        }


        /// title = "选取单位"
        /// description = "选取单位"
        /// comment = "使用'选取单位做动作'时, 指代相应的单位."

        public static JUnit GetEnumUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetEnumUnit"));
        }


        /// title = "匹配的可破坏物"
        /// description = "匹配的可破坏物"
        /// comment = "在'选取可破坏物满足条件'之类功能的条件中,指代被判断的可破坏物."

        public static JDestructable GetFilterDestructable()
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("GetFilterDestructable"));
        }


        /// title = "选取的可破坏物"
        /// description = "选取的可破坏物"
        /// comment = "使用'选取可破坏物做动作'时, 指代相应的可破坏物."

        public static JDestructable GetEnumDestructable()
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("GetEnumDestructable"));
        }


        /// title = "匹配物品"
        /// description = "匹配物品"
        /// comment = "在'选取物品满足条件'之类功能的条件中,指代被判断单位."

        public static JItem GetFilterItem()
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("GetFilterItem"));
        }


        /// title = "选取物品"
        /// description = "选取物品"
        /// comment = "使用'选取物品做动作'时, 指代相应的物品."

        public static JItem GetEnumItem()
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("GetEnumItem"));
        }


        /// title = "匹配玩家"
        /// description = "匹配玩家"
        /// comment = "在'选取玩家满足条件'之类功能的条件中,指代被判断玩家."

        public static JPlayer GetFilterPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetFilterPlayer"));
        }


        /// title = "选取玩家"
        /// description = "选取玩家"
        /// comment = "使用'选取玩家做动作'时, 指代相应的玩家."

        public static JPlayer GetEnumPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetEnumPlayer"));
        }


        /// title = "当前触发"
        /// description = "当前触发"
        /// comment = "当前所运行的触发器."

        public static JTrigger GetTriggeringTrigger()
        {
            return War3.CallNative<JTrigger>(War3.GetNativeFunction("GetTriggeringTrigger"));
        }

        public static JEventId GetTriggerEventId()
        {
            return War3.CallNative<JEventId>(War3.GetNativeFunction("GetTriggerEventId"));
        }


        /// title = "触发条件判断次数"
        /// description = "${Trigger} 的触发条件判断次数"
        /// comment = ""

        public static int GetTriggerEvalCount(JTrigger whichTrigger)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTriggerEvalCount"), whichTrigger.Handle);
        }


        /// title = "触发动作运行次数"
        /// description = "${Trigger} 的触发动作运行次数"
        /// comment = ""

        public static int GetTriggerExecCount(JTrigger whichTrigger)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTriggerExecCount"), whichTrigger.Handle);
        }


        /// title = "运行函数 [R]"
        /// description = "运行: ${函数名}"
        /// comment = "使用该功能运行的函数与触发独立, 只能运行自定义无参数函数."

        public static void ExecuteFunc(string funcName)
        {
            War3.CallNative(War3.GetNativeFunction("ExecuteFunc"), funcName);
        }

        public static JBoolExpr And(JBoolExpr operandA, JBoolExpr operandB)
        {
            return War3.CallNative<JBoolExpr>(War3.GetNativeFunction("And"), operandA.Handle, operandB.Handle);
        }

        public static JBoolExpr Or(JBoolExpr operandA, JBoolExpr operandB)
        {
            return War3.CallNative<JBoolExpr>(War3.GetNativeFunction("Or"), operandA.Handle, operandB.Handle);
        }

        public static JBoolExpr Not(JBoolExpr operand)
        {
            return War3.CallNative<JBoolExpr>(War3.GetNativeFunction("Not"), operand.Handle);
        }

        public static JConditionFunc Condition(JCode func)
        {
            return War3.CallNative<JConditionFunc>(War3.GetNativeFunction("Condition"), func);
        }

        public static void DestroyCondition(JConditionFunc c)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyCondition"), c.Handle);
        }

        public static JFilterFunc Filter(JCode func)
        {
            return War3.CallNative<JFilterFunc>(War3.GetNativeFunction("Filter"), func);
        }

        public static void DestroyFilter(JFilterFunc f)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyFilter"), f.Handle);
        }

        public static void DestroyBoolExpr(JBoolExpr e)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyBoolExpr"), e.Handle);
        }


        /// title = "实数变量事件"
        /// description = "${变量} 的值 ${Operation} ${值}"
        /// comment = "这个事件只适用于实数类型的变量."

        public static JEvent TriggerRegisterVariableEvent(JTrigger whichTrigger, string varName, JLimitOp opcode, float limitval)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterVariableEvent"), whichTrigger.Handle, varName, opcode.Handle, limitval);
        }

        public static JEvent TriggerRegisterTimerEvent(JTrigger whichTrigger, float timeout, bool periodic)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterTimerEvent"), whichTrigger.Handle, timeout, periodic);
        }

        public static JEvent TriggerRegisterTimerExpireEvent(JTrigger whichTrigger, JTimer t)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterTimerExpireEvent"), whichTrigger.Handle, t.Handle);
        }

        public static JEvent TriggerRegisterGameStateEvent(JTrigger whichTrigger, JGameState whichState, JLimitOp opcode, float limitval)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterGameStateEvent"), whichTrigger.Handle, whichState.Handle, opcode.Handle, limitval);
        }

        public static JEvent TriggerRegisterDialogEvent(JTrigger whichTrigger, JDialog whichDialog)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterDialogEvent"), whichTrigger.Handle, whichDialog.Handle);
        }


        /// title = "对话框按钮被点击 [R]"
        /// description = "${对话框按钮} 被点击"
        /// comment = "指定对话框按钮被点击,该事件一般需要在其他触发为其添加."

        public static JEvent TriggerRegisterDialogButtonEvent(JTrigger whichTrigger, JButton whichButton)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterDialogButtonEvent"), whichTrigger.Handle, whichButton.Handle);
        }

        public static JGameState GetEventGameState()
        {
            return War3.CallNative<JGameState>(War3.GetNativeFunction("GetEventGameState"));
        }


        /// title = "比赛游戏事件"
        /// description = "该游戏将在 ${Event Type} 后结束"
        /// comment = "该事件只出现在Battle.net的自动匹配游戏."

        public static JEvent TriggerRegisterGameEvent(JTrigger whichTrigger, JGameEvent whichGameEvent)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterGameEvent"), whichTrigger.Handle, whichGameEvent.Handle);
        }

        public static JPlayer GetWinningPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetWinningPlayer"));
        }


        /// title = "单位进入不规则区域(指定条件) [R]"
        /// description = "单位进入 ${区域} 并满足 ${条件}"
        /// comment = "使用'事件响应 - 进入的单位'来响应进入该区域的单位. 该事件需要在其他触发为其添加."

        public static JEvent TriggerRegisterEnterRegion(JTrigger whichTrigger, JRegion whichRegion, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterEnterRegion"), whichTrigger.Handle, whichRegion.Handle, filter.Handle);
        }


        /// title = "触发区域 [R]"
        /// description = "触发区域"
        /// comment = "响应单位进入/离开不规则区域事件."

        public static JRegion GetTriggeringRegion()
        {
            return War3.CallNative<JRegion>(War3.GetNativeFunction("GetTriggeringRegion"));
        }


        /// title = "进入的单位"
        /// description = "进入的单位"
        /// comment = "响应'单位进入区域'单位事件."

        public static JUnit GetEnteringUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetEnteringUnit"));
        }


        /// title = "单位离开不规则区域(指定条件) [R]"
        /// description = "单位离开 ${区域} 并满足 ${条件}"
        /// comment = "使用'事件响应 - 离开的单位'来响应离开该区域的单位. 该事件需要在其他触发为其添加."

        public static JEvent TriggerRegisterLeaveRegion(JTrigger whichTrigger, JRegion whichRegion, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterLeaveRegion"), whichTrigger.Handle, whichRegion.Handle, filter.Handle);
        }


        /// title = "离开的单位"
        /// description = "离开的单位"
        /// comment = "响应'单位离开区域'单位事件."

        public static JUnit GetLeavingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetLeavingUnit"));
        }


        /// title = "鼠标点击可追踪物 [R]"
        /// description = "鼠标点击 ${可追踪物}"
        /// comment = ""

        public static JEvent TriggerRegisterTrackableHitEvent(JTrigger whichTrigger, JTrackable t)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterTrackableHitEvent"), whichTrigger.Handle, t.Handle);
        }


        /// title = "鼠标移动到追踪对象 [R]"
        /// description = "鼠标移动到 ${可追踪物}"
        /// comment = ""

        public static JEvent TriggerRegisterTrackableTrackEvent(JTrigger whichTrigger, JTrackable t)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterTrackableTrackEvent"), whichTrigger.Handle, t.Handle);
        }


        /// title = "事件响应 - 触发可追踪物 [R]"
        /// description = "事件响应 - 触发可追踪物"
        /// comment = ""

        public static JTrackable GetTriggeringTrackable()
        {
            return War3.CallNative<JTrackable>(War3.GetNativeFunction("GetTriggeringTrackable"));
        }

        public static JButton GetClickedButton()
        {
            return War3.CallNative<JButton>(War3.GetNativeFunction("GetClickedButton"));
        }

        public static JDialog GetClickedDialog()
        {
            return War3.CallNative<JDialog>(War3.GetNativeFunction("GetClickedDialog"));
        }


        /// title = "比赛剩余时间"
        /// description = "比赛剩余时间"
        /// comment = "响应'比赛事件'游戏将要结束. 单位为秒."

        public static float GetTournamentFinishSoonTimeRemaining()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetTournamentFinishSoonTimeRemaining"));
        }


        /// title = "比赛结束规则"
        /// description = "比赛结束规则"
        /// comment = "1表示游戏开始时间已经超过限定时,无法以平局结束. 其他值表示游戏还在初期阶段,此时退出游戏将以平局结束.."

        public static int GetTournamentFinishNowRule()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTournamentFinishNowRule"));
        }

        public static JPlayer GetTournamentFinishNowPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetTournamentFinishNowPlayer"));
        }


        /// title = "对战比赛得分"
        /// description = "${Player} 的当前对战比赛得分"
        /// comment = "对战游戏时如果游戏时间过长,系统将以该值来决定胜负."

        public static int GetTournamentScore(JPlayer whichPlayer)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTournamentScore"), whichPlayer.Handle);
        }


        /// title = "存档文件名"
        /// description = "存档文件名"
        /// comment = "响应'游戏 - 保存进度'事件."

        public static string GetSaveBasicFilename()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetSaveBasicFilename"));
        }

        public static JEvent TriggerRegisterPlayerEvent(JTrigger whichTrigger, JPlayer whichPlayer, JPlayerEvent whichPlayerEvent)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerEvent"), whichTrigger.Handle, whichPlayer.Handle, whichPlayerEvent.Handle);
        }


        /// title = "触发玩家"
        /// description = "触发玩家"
        /// comment = ""

        public static JPlayer GetTriggerPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetTriggerPlayer"));
        }

        public static JEvent TriggerRegisterPlayerUnitEvent(JTrigger whichTrigger, JPlayer whichPlayer, JPlayerUnitEvent whichPlayerUnitEvent, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerUnitEvent"), whichTrigger.Handle, whichPlayer.Handle, whichPlayerUnitEvent.Handle, filter.Handle);
        }


        /// title = "升级的英雄"
        /// description = "升级的英雄"
        /// comment = "响应'提升等级'单位事件."

        public static JUnit GetLevelingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetLevelingUnit"));
        }


        /// title = "学习技能的英雄"
        /// description = "学习技能的英雄"
        /// comment = "响应'学习技能'单位事件."

        public static JUnit GetLearningUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetLearningUnit"));
        }


        /// title = "学习技能 [R]"
        /// description = "学习技能"
        /// comment = "响应'学习技能'单位事件, 指代被学习的技能."

        public static int GetLearnedSkill()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetLearnedSkill"));
        }


        /// title = "学习技能等级"
        /// description = "学习技能等级"
        /// comment = "响应'学习技能'单位事件,指代被学习技能的等级. 注意,该值为学习后的等级."

        public static int GetLearnedSkillLevel()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetLearnedSkillLevel"));
        }


        /// title = "可复活英雄"
        /// description = "可复活英雄"
        /// comment = "响应'变为可复活的'单位事件."

        public static JUnit GetRevivableUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetRevivableUnit"));
        }


        /// title = "复活英雄"
        /// description = "复活英雄"
        /// comment = "响应'开始/取消/完成复活'单位事件."

        public static JUnit GetRevivingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetRevivingUnit"));
        }


        /// title = "攻击单位"
        /// description = "攻击单位"
        /// comment = "响应'被攻击'单位事件."

        public static JUnit GetAttacker()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetAttacker"));
        }

        public static JUnit GetRescuer()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetRescuer"));
        }


        /// title = "死亡单位"
        /// description = "死亡单位"
        /// comment = "响应'死亡'单位事件."

        public static JUnit GetDyingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetDyingUnit"));
        }

        public static JUnit GetKillingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetKillingUnit"));
        }


        /// title = "腐化的单位"
        /// description = "腐化的单位"
        /// comment = "响应'开始腐化'单位事件."

        public static JUnit GetDecayingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetDecayingUnit"));
        }


        /// title = "正在建造的建筑"
        /// description = "正在建造的建筑"
        /// comment = "响应'开始建造建筑'单位事件."

        public static JUnit GetConstructingStructure()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetConstructingStructure"));
        }


        /// title = "被取消的建筑"
        /// description = "被取消的建筑"
        /// comment = "响应'取消建造建筑'单位事件."

        public static JUnit GetCancelledStructure()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetCancelledStructure"));
        }


        /// title = "完成的建筑"
        /// description = "完成的建筑"
        /// comment = "响应'完成建造建筑'单位事件."

        public static JUnit GetConstructedStructure()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetConstructedStructure"));
        }


        /// title = "研究科技的单位"
        /// description = "研究科技的单位"
        /// comment = "响应'开始/取消/完成科技研究'单位事件."

        public static JUnit GetResearchingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetResearchingUnit"));
        }


        /// title = "被研究科技"
        /// description = "被研究科技"
        /// comment = "响应'开始/取消/完成科技研究'单位事件."

        public static int GetResearched()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetResearched"));
        }


        /// title = "训练单位类型"
        /// description = "训练单位类型"
        /// comment = "响应'开始/取消/完成训练单位'单位事件, 指代所训练的单位类型."

        public static int GetTrainedUnitType()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTrainedUnitType"));
        }


        /// title = "训练单位"
        /// description = "训练单位"
        /// comment = "响应'完成训练单位'单位事件,指代被训练单位."

        public static JUnit GetTrainedUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetTrainedUnit"));
        }

        public static JUnit GetDetectedUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetDetectedUnit"));
        }


        /// title = "召唤者"
        /// description = "召唤者"
        /// comment = "响应'召唤单位'单位事件."

        public static JUnit GetSummoningUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetSummoningUnit"));
        }


        /// title = "召唤单位"
        /// description = "召唤单位"
        /// comment = "响应'召唤单位'单位事件,指代被召唤单位."

        public static JUnit GetSummonedUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetSummonedUnit"));
        }

        public static JUnit GetTransportUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetTransportUnit"));
        }

        public static JUnit GetLoadedUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetLoadedUnit"));
        }


        /// title = "贩卖者"
        /// description = "贩卖者"
        /// comment = "响应'出售单位','出售物品'或'抵押物品'单位事件."

        public static JUnit GetSellingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetSellingUnit"));
        }


        /// title = "被贩卖单位"
        /// description = "被贩卖单位"
        /// comment = "响应'出售单位'单位事件."

        public static JUnit GetSoldUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetSoldUnit"));
        }


        /// title = "购买者"
        /// description = "购买者"
        /// comment = "响应'出售单位','出售物品'或'抵押物品'单位事件."

        public static JUnit GetBuyingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetBuyingUnit"));
        }


        /// title = "被售出物品"
        /// description = "被售出物品"
        /// comment = "响应'出售物品'或'抵押物品'单位事件."

        public static JItem GetSoldItem()
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("GetSoldItem"));
        }


        /// title = "被改变所有者的单位"
        /// description = "被改变所有者的单位"
        /// comment = "响应'改变所有者'单位事件."

        public static JUnit GetChangingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetChangingUnit"));
        }


        /// title = "原所有者"
        /// description = "原所有者"
        /// comment = "响应'改变所有者'单位事件,指代单位原来的所有者."

        public static JPlayer GetChangingUnitPrevOwner()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetChangingUnitPrevOwner"));
        }


        /// title = "操作物品的单位"
        /// description = "操作物品的单位"
        /// comment = "响应'使用/获得/丢失物品'单位事件."

        public static JUnit GetManipulatingUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetManipulatingUnit"));
        }


        /// title = "被操作物品"
        /// description = "被操作物品"
        /// comment = "响应'使用/得到/丢弃物品'单位事件."

        public static JItem GetManipulatedItem()
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("GetManipulatedItem"));
        }


        /// title = "发布命令的单位"
        /// description = "发布命令的单位"
        /// comment = "响应'发布命令'单位事件."

        public static JUnit GetOrderedUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetOrderedUnit"));
        }

        public static int GetIssuedOrderId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetIssuedOrderId"));
        }


        /// title = "命令发布点X坐标 [R]"
        /// description = "命令发布点X坐标"
        /// comment = "用坐标代替点可以省去创建、删除点的麻烦."

        public static float GetOrderPointX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetOrderPointX"));
        }


        /// title = "命令发布点Y坐标 [R]"
        /// description = "命令发布点Y坐标"
        /// comment = "用坐标代替点可以省去创建、删除点的麻烦."

        public static float GetOrderPointY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetOrderPointY"));
        }


        /// title = "命令发布点"
        /// description = "命令发布点"
        /// comment = "响应'发布指定点目标命令'单位事件. 会创建点."

        public static JLocation GetOrderPointLoc()
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("GetOrderPointLoc"));
        }

        public static JWidget GetOrderTarget()
        {
            return War3.CallNative<JWidget>(War3.GetNativeFunction("GetOrderTarget"));
        }


        /// title = "命令发布目标(可破坏物)"
        /// description = "命令发布目标"
        /// comment = "响应'发布指定物体目标命令'单位事件并以可破坏物为目标时."

        public static JDestructable GetOrderTargetDestructable()
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("GetOrderTargetDestructable"));
        }


        /// title = "命令发布目标"
        /// description = "命令发布目标"
        /// comment = "响应'发布指定物体目标命令'单位事件并以物品为目标时."

        public static JItem GetOrderTargetItem()
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("GetOrderTargetItem"));
        }


        /// title = "命令发布目标"
        /// description = "命令发布目标"
        /// comment = "响应'发布指定物体目标命令'单位事件并以单位为目标时."

        public static JUnit GetOrderTargetUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetOrderTargetUnit"));
        }


        /// title = "施法单位"
        /// description = "施法单位"
        /// comment = "响应'施放技能'单位事件."

        public static JUnit GetSpellAbilityUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetSpellAbilityUnit"));
        }


        /// title = "施放技能"
        /// description = "施放技能"
        /// comment = "响应施放技能单位事件, 指代被施放的技能."

        public static int GetSpellAbilityId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetSpellAbilityId"));
        }

        public static JAbility GetSpellAbility()
        {
            return War3.CallNative<JAbility>(War3.GetNativeFunction("GetSpellAbility"));
        }


        /// title = "技能施放点"
        /// description = "技能施放点"
        /// comment = "响应'技能施放'单位事件. 注意技能施放结束将无法捕获该点. 会创建点."

        public static JLocation GetSpellTargetLoc()
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("GetSpellTargetLoc"));
        }


        /// title = "技能施放点X坐标"
        /// description = "获取技能目标点的X坐标"
        /// comment = "这是1.24的函数，但已加入函数库，在1.20也可以使用。"

        public static float GetSpellTargetX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetSpellTargetX"));
        }


        /// title = "技能施放点Y坐标"
        /// description = "获取技能目标点的Y坐标"
        /// comment = "这是1.24的函数，但已加入函数库，在1.20也可以使用。"

        public static float GetSpellTargetY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetSpellTargetY"));
        }


        /// title = "技能施放目标(可破坏物)"
        /// description = "技能施放目标"
        /// comment = "响应'施放技能'单位事件并以可破坏物为目标时. 注意: 技能施放结束将无法捕获施放目标."

        public static JDestructable GetSpellTargetDestructable()
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("GetSpellTargetDestructable"));
        }


        /// title = "技能施放目标"
        /// description = "技能施放目标"
        /// comment = "响应施放技能单位事件并以物品为目标时. 注意: 技能施放结束将无法捕获施放目标."

        public static JItem GetSpellTargetItem()
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("GetSpellTargetItem"));
        }


        /// title = "技能施放目标"
        /// description = "技能施放目标"
        /// comment = "响应'施放技能'单位事件并以单位为目标时. 注意: 技能施放结束将无法捕获施放目标."

        public static JUnit GetSpellTargetUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetSpellTargetUnit"));
        }


        /// title = "联盟状态更改(指定项目)"
        /// description = "${Player} 更改 ${Alliance Type} 设置"
        /// comment = "当改变项目为【共享单位】时，(触发玩家)会不生效，此时不建议使用【任意玩家】事件。"

        public static JEvent TriggerRegisterPlayerAllianceChange(JTrigger whichTrigger, JPlayer whichPlayer, JAllianceType whichAlliance)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerAllianceChange"), whichTrigger.Handle, whichPlayer.Handle, whichAlliance.Handle);
        }


        /// title = "属性事件"
        /// description = "${玩家} 的 ${Property} 属性 ${Operation} ${值}"
        /// comment = ""

        public static JEvent TriggerRegisterPlayerStateEvent(JTrigger whichTrigger, JPlayer whichPlayer, JPlayerState whichState, JLimitOp opcode, float limitval)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerStateEvent"), whichTrigger.Handle, whichPlayer.Handle, whichState.Handle, opcode.Handle, limitval);
        }

        public static JPlayerState GetEventPlayerState()
        {
            return War3.CallNative<JPlayerState>(War3.GetNativeFunction("GetEventPlayerState"));
        }


        /// title = "输入聊天信息"
        /// description = "${玩家} 输入 ${Text} ,信息过滤方式 ${Match Type}"
        /// comment = "事件ID是(096)"

        public static JEvent TriggerRegisterPlayerChatEvent(JTrigger whichTrigger, JPlayer whichPlayer, string chatMessageToDetect, bool exactMatchOnly)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerChatEvent"), whichTrigger.Handle, whichPlayer.Handle, chatMessageToDetect, exactMatchOnly);
        }


        /// title = "输入的聊天信息"
        /// description = "输入的聊天信息"
        /// comment = ""

        public static string GetEventPlayerChatString()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetEventPlayerChatString"));
        }


        /// title = "匹配的聊天信息"
        /// description = "匹配的聊天信息"
        /// comment = ""

        public static string GetEventPlayerChatStringMatched()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetEventPlayerChatStringMatched"));
        }


        /// title = "可破坏物死亡"
        /// description = "${可破坏物} 死亡"
        /// comment = "使用'事件响应 - 死亡的可破坏物'来获取死亡物体."

        public static JEvent TriggerRegisterDeathEvent(JTrigger whichTrigger, JWidget whichWidget)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterDeathEvent"), whichTrigger.Handle, whichWidget.Handle);
        }


        /// title = "触发单位"
        /// description = "触发单位"
        /// comment = ""

        public static JUnit GetTriggerUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetTriggerUnit"));
        }

        public static JEvent TriggerRegisterUnitStateEvent(JTrigger whichTrigger, JUnit whichUnit, JUnitState whichState, JLimitOp opcode, float limitval)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterUnitStateEvent"), whichTrigger.Handle, whichUnit.Handle, whichState.Handle, opcode.Handle, limitval);
        }

        public static JUnitState GetEventUnitState()
        {
            return War3.CallNative<JUnitState>(War3.GetNativeFunction("GetEventUnitState"));
        }


        /// title = "指定单位事件"
        /// description = "${指定单位} ${事件}"
        /// comment = ""

        public static JEvent TriggerRegisterUnitEvent(JTrigger whichTrigger, JUnit whichUnit, JUnitEvent whichEvent)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterUnitEvent"), whichTrigger.Handle, whichUnit.Handle, whichEvent.Handle);
        }


        /// title = "伤害值"
        /// description = "单位所受伤害"
        /// comment = "响应'受到伤害'单位事件,指代单位所受伤害."

        public static float GetEventDamage()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetEventDamage"));
        }


        /// title = "伤害来源"
        /// description = "伤害来源"
        /// comment = "响应'受到伤害'单位事件."

        public static JUnit GetEventDamageSource()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetEventDamageSource"));
        }

        public static JPlayer GetEventDetectingPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetEventDetectingPlayer"));
        }

        public static JEvent TriggerRegisterFilterUnitEvent(JTrigger whichTrigger, JUnit whichUnit, JUnitEvent whichEvent, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterFilterUnitEvent"), whichTrigger.Handle, whichUnit.Handle, whichEvent.Handle, filter.Handle);
        }


        /// title = "事件目标单位"
        /// description = "事件目标单位"
        /// comment = "响应'注意到/获取攻击目标'单位事件,指代目标单位."

        public static JUnit GetEventTargetUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetEventTargetUnit"));
        }

        public static JEvent TriggerRegisterUnitInRange(JTrigger whichTrigger, JUnit whichUnit, float range, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterUnitInRange"), whichTrigger.Handle, whichUnit.Handle, range, filter.Handle);
        }

        public static JTriggerCondition TriggerAddCondition(JTrigger whichTrigger, JBoolExpr condition)
        {
            return War3.CallNative<JTriggerCondition>(War3.GetNativeFunction("TriggerAddCondition"), whichTrigger.Handle, condition.Handle);
        }

        public static void TriggerRemoveCondition(JTrigger whichTrigger, JTriggerCondition whichCondition)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerRemoveCondition"), whichTrigger.Handle, whichCondition.Handle);
        }

        public static void TriggerClearConditions(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerClearConditions"), whichTrigger.Handle);
        }

        public static JTriggerAction TriggerAddAction(JTrigger whichTrigger, JCode actionFunc)
        {
            return War3.CallNative<JTriggerAction>(War3.GetNativeFunction("TriggerAddAction"), whichTrigger.Handle, actionFunc);
        }

        public static void TriggerRemoveAction(JTrigger whichTrigger, JTriggerAction whichAction)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerRemoveAction"), whichTrigger.Handle, whichAction.Handle);
        }

        public static void TriggerClearActions(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerClearActions"), whichTrigger.Handle);
        }


        /// title = "等待(玩家时间)"
        /// description = "等待 ${Time} 秒"
        /// comment = "该延迟功能受真实时间的影响(也就是玩家机器上的时间). 因此每个玩家所延迟的时间可能不一致."

        public static void TriggerSleepAction(float timeout)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerSleepAction"), timeout);
        }

        public static void TriggerWaitForSound(JSound s, float offset)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerWaitForSound"), s.Handle, offset);
        }


        /// title = "触发条件成立"
        /// description = "${触发} 的条件成立"
        /// comment = ""

        public static bool TriggerEvaluate(JTrigger whichTrigger)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("TriggerEvaluate"), whichTrigger.Handle);
        }


        /// title = "运行触发(无视条件)"
        /// description = "运行 ${触发} (无视条件)"
        /// comment = "无视事件和条件,运行触发动作."

        public static void TriggerExecute(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerExecute"), whichTrigger.Handle);
        }

        public static void TriggerExecuteWait(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerExecuteWait"), whichTrigger.Handle);
        }

        public static void TriggerSyncStart()
        {
            War3.CallNative(War3.GetNativeFunction("TriggerSyncStart"));
        }

        public static void TriggerSyncReady()
        {
            War3.CallNative(War3.GetNativeFunction("TriggerSyncReady"));
        }

        public static float GetWidgetLife(JWidget whichWidget)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetWidgetLife"), whichWidget.Handle);
        }

        public static void SetWidgetLife(JWidget whichWidget, float newLife)
        {
            War3.CallNative(War3.GetNativeFunction("SetWidgetLife"), whichWidget.Handle, newLife);
        }

        public static float GetWidgetX(JWidget whichWidget)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetWidgetX"), whichWidget.Handle);
        }

        public static float GetWidgetY(JWidget whichWidget)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetWidgetY"), whichWidget.Handle);
        }

        public static JWidget GetTriggerWidget()
        {
            return War3.CallNative<JWidget>(War3.GetNativeFunction("GetTriggerWidget"));
        }

        public static JDestructable CreateDestructable(int objectid, float x, float y, float face, float scale, int variation)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("CreateDestructable"), objectid, x, y, face, scale, variation);
        }


        /// title = "新建可破坏物 [R]"
        /// description = "新建的 ${可破坏物类型} 在(${X},${Y},${Z}),面向角度: ${Direction} 尺寸缩放: ${Scale} 样式: ${Variation}"
        /// comment = "坐标为(X,Y,Z)格式. 面向角度采用角度制,0度为正东方向,90度为正北方向."

        public static JDestructable CreateDestructableZ(int objectid, float x, float y, float z, float face, float scale, int variation)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("CreateDestructableZ"), objectid, x, y, z, face, scale, variation);
        }

        public static JDestructable CreateDeadDestructable(int objectid, float x, float y, float face, float scale, int variation)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("CreateDeadDestructable"), objectid, x, y, face, scale, variation);
        }


        /// title = "新建可破坏物(死亡的) [R]"
        /// description = "新建死亡的 ${可破坏物类型} 在(${X},${Y},${Z\"),面向角度: \"}${Direction} 尺寸缩放: ${Scale} 样式: ${Variation}"
        /// comment = "坐标为(X,Y,Z)格式. 面向角度采用角度制,0度为正东方向,90度为正北方向."

        public static JDestructable CreateDeadDestructableZ(int objectid, float x, float y, float z, float face, float scale, int variation)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("CreateDeadDestructableZ"), objectid, x, y, z, face, scale, variation);
        }


        /// title = "删除"
        /// description = "删除 ${可破坏物}"
        /// comment = ""

        public static void RemoveDestructable(JDestructable d)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveDestructable"), d.Handle);
        }


        /// title = "杀死"
        /// description = "杀死 ${可破坏物}"
        /// comment = ""

        public static void KillDestructable(JDestructable d)
        {
            War3.CallNative(War3.GetNativeFunction("KillDestructable"), d.Handle);
        }

        public static void SetDestructableInvulnerable(JDestructable d, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableInvulnerable"), d.Handle, flag);
        }

        public static bool IsDestructableInvulnerable(JDestructable d)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsDestructableInvulnerable"), d.Handle);
        }

        public static void EnumDestructablesInRect(JRect r, JBoolExpr filter, JCode actionFunc)
        {
            War3.CallNative(War3.GetNativeFunction("EnumDestructablesInRect"), r.Handle, filter.Handle, actionFunc);
        }


        /// title = "指定可破坏物的类型"
        /// description = "${可破坏物} 的类型"
        /// comment = ""

        public static int GetDestructableTypeId(JDestructable d)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetDestructableTypeId"), d.Handle);
        }


        /// title = "可破坏物所在X轴坐标 [R]"
        /// description = "${可破坏物} 所在X轴坐标"
        /// comment = ""

        public static float GetDestructableX(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableX"), d.Handle);
        }


        /// title = "可破坏物所在Y轴坐标 [R]"
        /// description = "${可破坏物} 所在Y轴坐标"

        public static float GetDestructableY(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableY"), d.Handle);
        }


        /// title = "设置生命值(指定值)"
        /// description = "设置 ${可破坏物} 的生命值为 ${Value}"
        /// comment = ""

        public static void SetDestructableLife(JDestructable d, float life)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableLife"), d.Handle, life);
        }


        /// title = "生命值"
        /// description = "${可破坏物} 的当前生命值"
        /// comment = ""

        public static float GetDestructableLife(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableLife"), d.Handle);
        }

        public static void SetDestructableMaxLife(JDestructable d, float max)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableMaxLife"), d.Handle, max);
        }


        /// title = "最大生命值"
        /// description = "${可破坏物} 的最大生命值"
        /// comment = ""

        public static float GetDestructableMaxLife(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableMaxLife"), d.Handle);
        }


        /// title = "复活"
        /// description = "复活 ${Destructible} ,设置生命值为 ${Value} 并 ${Show/Hide} 生长动画"
        /// comment = ""

        public static void DestructableRestoreLife(JDestructable d, float life, bool birth)
        {
            War3.CallNative(War3.GetNativeFunction("DestructableRestoreLife"), d.Handle, life, birth);
        }

        public static void QueueDestructableAnimation(JDestructable d, string whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("QueueDestructableAnimation"), d.Handle, whichAnimation);
        }

        public static void SetDestructableAnimation(JDestructable d, string whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableAnimation"), d.Handle, whichAnimation);
        }


        /// title = "改变可破坏物动画播放速度 [R]"
        /// description = "改变 ${可破坏物} 的动画播放速度为正常的 ${Percent}倍"
        /// comment = "设置1倍动画播放速度来恢复正常状态."

        public static void SetDestructableAnimationSpeed(JDestructable d, float speedFactor)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableAnimationSpeed"), d.Handle, speedFactor);
        }


        /// title = "显示/隐藏 [R]"
        /// description = "设置 ${可破坏物} 的状态为 ${Show/Hide}"
        /// comment = "隐藏的可破坏物不被显示,但仍影响通行和视线."

        public static void ShowDestructable(JDestructable d, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("ShowDestructable"), d.Handle, flag);
        }


        /// title = "闭塞高度"
        /// description = "${可破坏物} 的闭塞高度"
        /// comment = ""

        public static float GetDestructableOccluderHeight(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableOccluderHeight"), d.Handle);
        }


        /// title = "设置闭塞高度"
        /// description = "设置 ${可破坏物} 的闭塞高度为 ${Height}"
        /// comment = ""

        public static void SetDestructableOccluderHeight(JDestructable d, float height)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableOccluderHeight"), d.Handle, height);
        }


        /// title = "物件名字"
        /// description = "${物件} 的名字"
        /// comment = ""

        public static string GetDestructableName(JDestructable d)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetDestructableName"), d.Handle);
        }

        public static JDestructable GetTriggerDestructable()
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("GetTriggerDestructable"));
        }


        /// title = "新建物品 [R]"
        /// description = "新建 ${物品} 在(${X},${Y})"
        /// comment = ""

        public static JItem CreateItem(int itemid, float x, float y)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("CreateItem"), itemid, x, y);
        }


        /// title = "删除"
        /// description = "删除 ${物品}"
        /// comment = ""

        public static void RemoveItem(JItem whichItem)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveItem"), whichItem.Handle);
        }


        /// title = "物品所属玩家"
        /// description = "${物品} 的所属玩家"
        /// comment = "与持有者无关,默认为中立被动玩家."

        public static JPlayer GetItemPlayer(JItem whichItem)
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetItemPlayer"), whichItem.Handle);
        }


        /// title = "指定物品的类型"
        /// description = "${物品} 的类型"
        /// comment = ""

        public static int GetItemTypeId(JItem i)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetItemTypeId"), i.Handle);
        }


        /// title = "物品的X轴坐标 [R]"
        /// description = "${物品} 的X轴坐标"
        /// comment = ""

        public static float GetItemX(JItem i)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetItemX"), i.Handle);
        }


        /// title = "物品的Y轴坐标 [R]"
        /// description = "${物品} 的Y轴坐标"
        /// comment = ""

        public static float GetItemY(JItem i)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetItemY"), i.Handle);
        }


        /// title = "移动物品到坐标(立即)(指定坐标) [R]"
        /// description = "移动 ${物品} 到(${X},${Y})"
        /// comment = ""

        public static void SetItemPosition(JItem i, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemPosition"), i.Handle, x, y);
        }

        public static void SetItemDropOnDeath(JItem whichItem, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemDropOnDeath"), whichItem.Handle, flag);
        }

        public static void SetItemDroppable(JItem i, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemDroppable"), i.Handle, flag);
        }


        /// title = "设置物品可否抵押"
        /// description = "设置 ${物品} ${Pawnable/Unpawnable}"
        /// comment = "不可抵押物品不能被卖到商店."

        public static void SetItemPawnable(JItem i, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemPawnable"), i.Handle, flag);
        }

        public static void SetItemPlayer(JItem whichItem, JPlayer whichPlayer, bool changeColor)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemPlayer"), whichItem.Handle, whichPlayer.Handle, changeColor);
        }

        public static void SetItemInvulnerable(JItem whichItem, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemInvulnerable"), whichItem.Handle, flag);
        }


        /// title = "物品无敌"
        /// description = "${物品} 是无敌的"
        /// comment = ""

        public static bool IsItemInvulnerable(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemInvulnerable"), whichItem.Handle);
        }


        /// title = "显示/隐藏 [R]"
        /// description = "设置 ${物品} 的状态为: ${Show/Hide}"
        /// comment = "只对在地面的物品有效,不会影响在物品栏中的物品. 单位通过触发得到一个隐藏物品时,会自动显示该物品."

        public static void SetItemVisible(JItem whichItem, bool show)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemVisible"), whichItem.Handle, show);
        }


        /// title = "物品可见 [R]"
        /// description = "${物品} 是可见的"
        /// comment = "物品不被隐藏且不被单位持有时即为可见的."

        public static bool IsItemVisible(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemVisible"), whichItem.Handle);
        }


        /// title = "物品被持有"
        /// description = "${物品} 是被持有的"
        /// comment = "在物品栏中的物品都是被持有的. 就算单位正处于死亡状态."

        public static bool IsItemOwned(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemOwned"), whichItem.Handle);
        }


        /// title = "物品是拾取时自动使用的 [R]"
        /// description = "${物品} 是拾取时自动使用类物品"
        /// comment = ""

        public static bool IsItemPowerup(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemPowerup"), whichItem.Handle);
        }


        /// title = "物品可被市场随机出售 [R]"
        /// description = "${物品} 可被市场随机出售"
        /// comment = ""

        public static bool IsItemSellable(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemSellable"), whichItem.Handle);
        }


        /// title = "物品可被抵押 [R]"
        /// description = "${物品} 可被抵押"
        /// comment = ""

        public static bool IsItemPawnable(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemPawnable"), whichItem.Handle);
        }

        public static bool IsItemIdPowerup(int itemId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemIdPowerup"), itemId);
        }

        public static bool IsItemIdSellable(int itemId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemIdSellable"), itemId);
        }

        public static bool IsItemIdPawnable(int itemId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemIdPawnable"), itemId);
        }

        public static void EnumItemsInRect(JRect r, JBoolExpr filter, JCode actionFunc)
        {
            War3.CallNative(War3.GetNativeFunction("EnumItemsInRect"), r.Handle, filter.Handle, actionFunc);
        }


        /// title = "物品等级"
        /// description = "${物品} 的物品等级"
        /// comment = ""

        public static int GetItemLevel(JItem whichItem)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetItemLevel"), whichItem.Handle);
        }


        /// title = "指定物品的分类"
        /// description = "${物品} 的分类"
        /// comment = ""

        public static JItemType GetItemType(JItem whichItem)
        {
            return War3.CallNative<JItemType>(War3.GetNativeFunction("GetItemType"), whichItem.Handle);
        }


        /// title = "设置重生神符的产生单位类型"
        /// description = "设置 ${物品} 产生 ${单位类型}"
        /// comment = "设置重生神符对应的单位类型后，当英雄吃了重生神符，则会产生指定类型的单位。"

        public static void SetItemDropID(JItem whichItem, int unitId)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemDropID"), whichItem.Handle, unitId);
        }


        /// title = "物品名字"
        /// description = "${物品} 的名字"
        /// comment = ""

        public static string GetItemName(JItem whichItem)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetItemName"), whichItem.Handle);
        }


        /// title = "使用次数"
        /// description = "${物品} 的使用次数"
        /// comment = "无限使用物品将返回0."

        public static int GetItemCharges(JItem whichItem)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetItemCharges"), whichItem.Handle);
        }


        /// title = "设置物品使用次数"
        /// description = "设置 ${物品} 的使用次数为 ${Charges}"
        /// comment = "设置为0可以使物品能无限次使用."

        public static void SetItemCharges(JItem whichItem, int charges)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemCharges"), whichItem.Handle, charges);
        }


        /// title = "物品自定义值"
        /// description = "${物品} 的自定义值"
        /// comment = "可以使用'物品 - 设置自定义值'来设置该值."

        public static int GetItemUserData(JItem whichItem)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetItemUserData"), whichItem.Handle);
        }


        /// title = "设置物品自定义值"
        /// description = "设置 ${物品} 的自定义值为 ${Index}"
        /// comment = "物品自定义值只用于触发器. 可以用来为物品绑定一个整型数据."

        public static void SetItemUserData(JItem whichItem, int data)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemUserData"), whichItem.Handle, data);
        }


        /// title = "新建单位(指定坐标) [R]"
        /// description = "新建 ${玩家} 的 ${单位} 在(${X},${Y}),面向角度:${Face} 度"
        /// comment = "在坐标创建单位，不能被'最后创建的单位'获得。"

        public static JUnit CreateUnit(JPlayer id, int unitid, float x, float y, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateUnit"), id.Handle, unitid, x, y, face);
        }

        public static JUnit CreateUnitByName(JPlayer whichPlayer, string unitname, float x, float y, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateUnitByName"), whichPlayer.Handle, unitname, x, y, face);
        }


        /// title = "新建单位(指定点) [R]"
        /// description = "新建 ${玩家} 的 ${单位} 在 ${点} 面向角度:${Face} 度"
        /// comment = ""

        public static JUnit CreateUnitAtLoc(JPlayer id, int unitid, JLocation whichLocation, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateUnitAtLoc"), id.Handle, unitid, whichLocation.Handle, face);
        }

        public static JUnit CreateUnitAtLocByName(JPlayer id, string unitname, JLocation whichLocation, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateUnitAtLocByName"), id.Handle, unitname, whichLocation.Handle, face);
        }


        /// title = "新建尸体 [R]"
        /// description = "新建 ${玩家} 的 ${单位} 的尸体在(${X},${Y}),面向角度:${Face} 度"
        /// comment = ""

        public static JUnit CreateCorpse(JPlayer whichPlayer, int unitid, float x, float y, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateCorpse"), whichPlayer.Handle, unitid, x, y, face);
        }


        /// title = "杀死"
        /// description = "杀死 ${单位}"
        /// comment = ""

        public static void KillUnit(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("KillUnit"), whichUnit.Handle);
        }


        /// title = "删除"
        /// description = "删除 ${单位}"
        /// comment = "被删除的单位不会留下尸体. 如果是英雄则不能再被复活."

        public static void RemoveUnit(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveUnit"), whichUnit.Handle);
        }


        /// title = "显示/隐藏 [R]"
        /// description = "设置 ${单位} 的状态为 ${显示/隐藏}"
        /// comment = "隐藏单位不会被'区域内单位'所选取."

        public static void ShowUnit(JUnit whichUnit, bool show)
        {
            War3.CallNative(War3.GetNativeFunction("ShowUnit"), whichUnit.Handle, show);
        }


        /// title = "设置单位属性 [R]"
        /// description = "设置 ${单位} 的 ${属性} 为 ${Value}"
        /// comment = ""

        public static void SetUnitState(JUnit whichUnit, JUnitState whichUnitState, float newVal)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitState"), whichUnit.Handle, whichUnitState.Handle, newVal);
        }


        /// title = "设置X坐标 [R]"
        /// description = "设置 ${单位} 的X坐标为 ${X}"
        /// comment = "注意如果坐标超出地图边界是会出错的."

        public static void SetUnitX(JUnit whichUnit, float newX)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitX"), whichUnit.Handle, newX);
        }


        /// title = "设置Y坐标 [R]"
        /// description = "设置 ${单位} 的Y坐标为 ${Y}"
        /// comment = "注意如果坐标超出地图边界是会出错的."

        public static void SetUnitY(JUnit whichUnit, float newY)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitY"), whichUnit.Handle, newY);
        }


        /// title = "移动单位(立即)(指定坐标) [R]"
        /// description = "立即移动 ${单位} 到(${X},${Y})"
        /// comment = ""

        public static void SetUnitPosition(JUnit whichUnit, float newX, float newY)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitPosition"), whichUnit.Handle, newX, newY);
        }


        /// title = "移动单位(立即)(指定点)"
        /// description = "立即移动 ${单位} 到 ${指定点}"
        /// comment = ""

        public static void SetUnitPositionLoc(JUnit whichUnit, JLocation whichLocation)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitPositionLoc"), whichUnit.Handle, whichLocation.Handle);
        }


        /// title = "设置单位面向角度 [R]"
        /// description = "设置 ${单位} 的面向角度为 ${Angle} 度"
        /// comment = "面向角度采用角度制,0度为正东方向,90度为正北方向。速度等于单位的转身速度。"

        public static void SetUnitFacing(JUnit whichUnit, float facingAngle)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitFacing"), whichUnit.Handle, facingAngle);
        }


        /// title = "设置单位面向角度(指定时间)"
        /// description = "设置 ${单位} 的面向角度为 ${Angle} 度,使用时间 ${Time} 秒"
        /// comment = "面向角度采用角度制,0度为正东方向,90度为正北方向。不能超过单位的转身速度。"

        public static void SetUnitFacingTimed(JUnit whichUnit, float facingAngle, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitFacingTimed"), whichUnit.Handle, facingAngle, duration);
        }


        /// title = "设置移动速度"
        /// description = "设置 ${单位} 的移动速度为 ${Speed}"
        /// comment = ""

        public static void SetUnitMoveSpeed(JUnit whichUnit, float newSpeed)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitMoveSpeed"), whichUnit.Handle, newSpeed);
        }

        public static void SetUnitFlyHeight(JUnit whichUnit, float newHeight, float rate)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitFlyHeight"), whichUnit.Handle, newHeight, rate);
        }

        public static void SetUnitTurnSpeed(JUnit whichUnit, float newTurnSpeed)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitTurnSpeed"), whichUnit.Handle, newTurnSpeed);
        }


        /// title = "改变单位转向角度(弧度制) [R]"
        /// description = "改变 ${单位} 的转向角度为 ${数值} (弧度制)"
        /// comment = "设置单位转身时的转向角度. 数值越大转向幅度越大. "

        public static void SetUnitPropWindow(JUnit whichUnit, float newPropWindowAngle)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitPropWindow"), whichUnit.Handle, newPropWindowAngle);
        }

        public static void SetUnitAcquireRange(JUnit whichUnit, float newAcquireRange)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitAcquireRange"), whichUnit.Handle, newAcquireRange);
        }


        /// title = "锁定指定单位的警戒点 [R]"
        /// description = "设置 ${单位} 的警戒点: ${option}"
        /// comment = "锁定并防止 AI 脚本改动单位警戒点."

        public static void SetUnitCreepGuard(JUnit whichUnit, bool creepGuard)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitCreepGuard"), whichUnit.Handle, creepGuard);
        }


        /// title = "当前主动攻击范围"
        /// description = "${单位} 的当前主动攻击范围"
        /// comment = ""

        public static float GetUnitAcquireRange(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitAcquireRange"), whichUnit.Handle);
        }


        /// title = "当前转身速度"
        /// description = "${单位} 的当前转身速度"
        /// comment = "转身速度表示单位改变面向方向时的速度. 数值越小表示转身越慢."

        public static float GetUnitTurnSpeed(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitTurnSpeed"), whichUnit.Handle);
        }


        /// title = "当前转向角度(弧度制) [R]"
        /// description = "${单位} 的当前转向角度(弧度制)"
        /// comment = "单位转身时的转向角度. 数值越大转向幅度越大. "

        public static float GetUnitPropWindow(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitPropWindow"), whichUnit.Handle);
        }


        /// title = "当前飞行高度"
        /// description = "${单位} 的当前飞行高度"
        /// comment = "飞行单位可以直接改变飞行高度. 其他单位通过添加/删除 替换为飞行单位的变身技能(如乌鸦形态)之后,也能改变飞行高度."

        public static float GetUnitFlyHeight(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitFlyHeight"), whichUnit.Handle);
        }


        /// title = "默认主动攻击范围"
        /// description = "${单位} 的默认主动攻击范围"
        /// comment = ""

        public static float GetUnitDefaultAcquireRange(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultAcquireRange"), whichUnit.Handle);
        }


        /// title = "默认转身速度"
        /// description = "${单位} 的默认转身速度"
        /// comment = "转身速度表示单位改变面向方向时的速度. 数值越小表示转身越慢."

        public static float GetUnitDefaultTurnSpeed(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultTurnSpeed"), whichUnit.Handle);
        }

        public static float GetUnitDefaultPropWindow(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultPropWindow"), whichUnit.Handle);
        }


        /// title = "默认飞行高度"
        /// description = "${单位} 的默认飞行高度"
        /// comment = "飞行单位可以直接改变飞行高度. 其他单位通过添加/删除 替换为飞行单位的变身技能(如乌鸦形态)之后,也能改变飞行高度."

        public static float GetUnitDefaultFlyHeight(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultFlyHeight"), whichUnit.Handle);
        }


        /// title = "改变所属"
        /// description = "改变 ${单位} 所属为 ${Player} 并 ${Change/Retain Color}"
        /// comment = ""

        public static void SetUnitOwner(JUnit whichUnit, JPlayer whichPlayer, bool changeColor)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitOwner"), whichUnit.Handle, whichPlayer.Handle, changeColor);
        }


        /// title = "改变队伍颜色"
        /// description = "改变 ${单位} 的队伍颜色为 ${Color}"
        /// comment = "改变队伍颜色并不会改变单位所属."

        public static void SetUnitColor(JUnit whichUnit, JPlayerColor whichColor)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitColor"), whichUnit.Handle, whichColor.Handle);
        }


        /// title = "改变单位尺寸(按倍数) [R]"
        /// description = "改变 ${单位} 的尺寸缩放为:(${X},${Y},${Z})"
        /// comment = "缩放尺寸使用(长,宽,高)格式."

        public static void SetUnitScale(JUnit whichUnit, float scaleX, float scaleY, float scaleZ)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitScale"), whichUnit.Handle, scaleX, scaleY, scaleZ);
        }


        /// title = "改变单位动画播放速度(按倍数) [R]"
        /// description = "改变 ${单位} 的动画播放速度为正常速度的 ${Percent} 倍"
        /// comment = "设置1倍动画播放速度来恢复正常状态."

        public static void SetUnitTimeScale(JUnit whichUnit, float timeScale)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitTimeScale"), whichUnit.Handle, timeScale);
        }

        public static void SetUnitBlendTime(JUnit whichUnit, float blendTime)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitBlendTime"), whichUnit.Handle, blendTime);
        }


        /// title = "改变单位的颜色(RGB:0-255) [R]"
        /// description = "改变 ${单位} 的颜色值: (${Red},${Green},${Blue}), 透明值: ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). 大多数单位使用(255,255,255)的颜色值和255的Alpha值. 透明值为0是不可见的.颜色值和Alpha值取值范围为0-255."

        public static void SetUnitVertexColor(JUnit whichUnit, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitVertexColor"), whichUnit.Handle, red, green, blue, alpha);
        }

        public static void QueueUnitAnimation(JUnit whichUnit, string whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("QueueUnitAnimation"), whichUnit.Handle, whichAnimation);
        }


        /// title = "播放单位动画"
        /// description = "播放 ${Unit} 的 ${动画名} 动作"
        /// comment = "通过 '重置单位动作' 恢复到普通的动作."

        public static void SetUnitAnimation(JUnit whichUnit, string whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitAnimation"), whichUnit.Handle, whichAnimation);
        }


        /// title = "播放单位指定序号动动作 [R]"
        /// description = "播放 ${单位} 的第${序号} 号动作"
        /// comment = "可以指定播放所有的单位动画,不过需要自己多尝试.每个单位的动作序号不一样的."

        public static void SetUnitAnimationByIndex(JUnit whichUnit, int whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitAnimationByIndex"), whichUnit.Handle, whichAnimation);
        }


        /// title = "播放单位动运作(指定概率)"
        /// description = "播放 ${单位} 的 ${Animation Name} 动作,只用 ${Rarity} 动作"
        /// comment = "通过 '重置单位动作' 恢复到普通的动作."

        public static void SetUnitAnimationWithRarity(JUnit whichUnit, string whichAnimation, JRarityControl rarity)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitAnimationWithRarity"), whichUnit.Handle, whichAnimation, rarity.Handle);
        }


        /// title = "添加/删除 单位动画附加名 [R]"
        /// description = "给 ${单位} 附加动作 ${Tag} ,状态为 ${Add/Remove}"
        /// comment = "比如恶魔猎手添加'alternate'会显示为恶魔形态;农民添加'gold'则为背负黄金形态."

        public static void AddUnitAnimationProperties(JUnit whichUnit, string animProperties, bool add)
        {
            War3.CallNative(War3.GetNativeFunction("AddUnitAnimationProperties"), whichUnit.Handle, animProperties, add);
        }


        /// title = "锁定身体朝向"
        /// description = "锁定 ${单位} 的 ${Source} 朝向 ${目标单位} ,偏移坐标 (${X}, ${Y}, ${Z})"
        /// comment = "单位的该身体部件会一直朝向目标单位的偏移坐标点处,直到使用'重置身体朝向'. 坐标偏移以目标单位脚下为坐标原点."

        public static void SetUnitLookAt(JUnit whichUnit, string whichBone, JUnit lookAtTarget, float offsetX, float offsetY, float offsetZ)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitLookAt"), whichUnit.Handle, whichBone, lookAtTarget.Handle, offsetX, offsetY, offsetZ);
        }


        /// title = "重置身体朝向"
        /// description = "重置 ${单位} 的身体朝向"
        /// comment = "恢复单位的身体朝向为正常状态."

        public static void ResetUnitLookAt(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("ResetUnitLookAt"), whichUnit.Handle);
        }


        /// title = "设置可否营救(对玩家) [R]"
        /// description = "设置 ${单位} 对 ${玩家}${Rescuable/Unrescuable}"
        /// comment = ""

        public static void SetUnitRescuable(JUnit whichUnit, JPlayer byWhichPlayer, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitRescuable"), whichUnit.Handle, byWhichPlayer.Handle, flag);
        }


        /// title = "设置营救范围"
        /// description = "设置 ${单位} 的营救范围为 ${Range}"
        /// comment = ""

        public static void SetUnitRescueRange(JUnit whichUnit, float range)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitRescueRange"), whichUnit.Handle, range);
        }


        /// title = "设置英雄力量 [R]"
        /// description = "设置 ${英雄} 的力量为 ${Value} ,(${Permanent}永久奖励)"
        /// comment = "永久奖励貌似无效项,不需要理会."

        public static void SetHeroStr(JUnit whichHero, int newStr, bool permanent)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroStr"), whichHero.Handle, newStr, permanent);
        }


        /// title = "设置英雄敏捷 [R]"
        /// description = "设置 ${英雄} 的敏捷为 ${Value} ,(${Permanent}永久奖励)"
        /// comment = "永久奖励貌似无效项,不需要理会."

        public static void SetHeroAgi(JUnit whichHero, int newAgi, bool permanent)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroAgi"), whichHero.Handle, newAgi, permanent);
        }


        /// title = "设置英雄智力 [R]"
        /// description = "设置 ${英雄} 的智力为 ${Value} ,(${Permanent}永久奖励)"
        /// comment = "永久奖励貌似无效项,不需要理会."

        public static void SetHeroInt(JUnit whichHero, int newInt, bool permanent)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroInt"), whichHero.Handle, newInt, permanent);
        }


        /// title = "英雄力量 [R]"
        /// description = "${英雄} 的力量值(${Include/Exclude} 加成)"
        /// comment = ""

        public static int GetHeroStr(JUnit whichHero, bool includeBonuses)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroStr"), whichHero.Handle, includeBonuses);
        }


        /// title = "英雄敏捷 [R]"
        /// description = "${英雄} 的敏捷值(${Include/Exclude} 加成)"
        /// comment = ""

        public static int GetHeroAgi(JUnit whichHero, bool includeBonuses)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroAgi"), whichHero.Handle, includeBonuses);
        }


        /// title = "英雄智力 [R]"
        /// description = "${英雄} 的智力值(${Include/Exclude} 加成)"
        /// comment = ""

        public static int GetHeroInt(JUnit whichHero, bool includeBonuses)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroInt"), whichHero.Handle, includeBonuses);
        }


        /// title = "降低等级 [R]"
        /// description = "降低 ${Hero} ${Level} 个等级"
        /// comment = "只能降低等级. 英雄经验将重置为该等级的初始值."

        public static bool UnitStripHeroLevel(JUnit whichHero, int howManyLevels)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitStripHeroLevel"), whichHero.Handle, howManyLevels);
        }


        /// title = "英雄经验值"
        /// description = "${英雄} 的经验值"
        /// comment = ""

        public static int GetHeroXP(JUnit whichHero)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroXP"), whichHero.Handle);
        }


        /// title = "设置经验值"
        /// description = "设置 ${Hero} 的经验值为 ${Quantity} , ${Show/Hide} 升级动画"
        /// comment = "经验值不能倒退."

        public static void SetHeroXP(JUnit whichHero, int newXpVal, bool showEyeCandy)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroXP"), whichHero.Handle, newXpVal, showEyeCandy);
        }


        /// title = "未分配技能点数"
        /// description = "${英雄} 的未分配技能点数"
        /// comment = ""

        public static int GetHeroSkillPoints(JUnit whichHero)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroSkillPoints"), whichHero.Handle);
        }


        /// title = "添加剩余技能点 [R]"
        /// description = "增加 ${英雄} ${Value} 点剩余技能点"
        /// comment = ""

        public static bool UnitModifySkillPoints(JUnit whichHero, int skillPointDelta)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitModifySkillPoints"), whichHero.Handle, skillPointDelta);
        }


        /// title = "增加经验值 [R]"
        /// description = "增加 ${Hero} ${Quantity} 点经验值, ${Show/Hide} 升级动画"
        /// comment = "经验值不能倒退."

        public static void AddHeroXP(JUnit whichHero, int xpToAdd, bool showEyeCandy)
        {
            War3.CallNative(War3.GetNativeFunction("AddHeroXP"), whichHero.Handle, xpToAdd, showEyeCandy);
        }


        /// title = "设置等级"
        /// description = "设置 ${Hero} 的英雄等级为 ${Level} , ${Show/Hide} 升级动画"
        /// comment = "如果等级有变动,英雄经验将重置为该等级的初始值."

        public static void SetHeroLevel(JUnit whichHero, int level, bool showEyeCandy)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroLevel"), whichHero.Handle, level, showEyeCandy);
        }


        /// title = "英雄等级"
        /// description = "${英雄} 的英雄等级"
        /// comment = ""

        public static int GetHeroLevel(JUnit whichHero)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroLevel"), whichHero.Handle);
        }


        /// title = "单位等级"
        /// description = "${单位} 的等级"
        /// comment = "对于英雄则会返回其英雄等级."

        public static int GetUnitLevel(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitLevel"), whichUnit.Handle);
        }


        /// title = "英雄称谓"
        /// description = "${Hero} 的称谓"
        /// comment = "如圣骑士会返回'无惧的布赞恩'而不是'圣骑士'."

        public static string GetHeroProperName(JUnit whichHero)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetHeroProperName"), whichHero.Handle);
        }


        /// title = "允许/禁止经验获取 [R]"
        /// description = "${Enable/Disable} ${Hero} 的经验获取"
        /// comment = ""

        public static void SuspendHeroXP(JUnit whichHero, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SuspendHeroXP"), whichHero.Handle, flag);
        }


        /// title = "经验不可获得"
        /// description = "${Hero} 不可获得经验"
        /// comment = "可使用'英雄 - 允许/禁止经验获取'来设置该项."

        public static bool IsSuspendedXP(JUnit whichHero)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsSuspendedXP"), whichHero.Handle);
        }


        /// title = "学习技能"
        /// description = "命令 ${Hero} 学习技能 ${Skill}"
        /// comment = "只有当英雄有剩余技能点时有效."

        public static void SelectHeroSkill(JUnit whichHero, int abilcode)
        {
            War3.CallNative(War3.GetNativeFunction("SelectHeroSkill"), whichHero.Handle, abilcode);
        }


        /// title = "单位技能等级 [R]"
        /// description = "${单位} 的 ${技能} 技能等级"
        /// comment = "如果单位没有该技能,则返回0."

        public static int GetUnitAbilityLevel(JUnit whichUnit, int abilcode)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitAbilityLevel"), whichUnit.Handle, abilcode);
        }


        /// title = "降低技能等级 [R]"
        /// description = "使 ${单位} 的 ${技能} 等级降低1级"
        /// comment = "改变死亡单位的光环技能会导致魔兽崩溃."

        public static int DecUnitAbilityLevel(JUnit whichUnit, int abilcode)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DecUnitAbilityLevel"), whichUnit.Handle, abilcode);
        }


        /// title = "提升技能等级 [R]"
        /// description = "使 ${单位} 的 ${技能} 等级提升1级"
        /// comment = "改变死亡单位的光环技能会导致魔兽崩溃."

        public static int IncUnitAbilityLevel(JUnit whichUnit, int abilcode)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("IncUnitAbilityLevel"), whichUnit.Handle, abilcode);
        }


        /// title = "设置技能等级 [R]"
        /// description = "设置 ${单位} 的 ${技能} 等级为 ${Level}"
        /// comment = "改变死亡单位的光环技能会导致魔兽崩溃."

        public static int SetUnitAbilityLevel(JUnit whichUnit, int abilcode, int level)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("SetUnitAbilityLevel"), whichUnit.Handle, abilcode, level);
        }


        /// title = "立即复活(指定坐标) [R]"
        /// description = "立即复活 ${英雄} 在(${X},${Y}), ${Show/Hide} 复活动画"
        /// comment = "如果英雄正在祭坛复活,则会退回部分花费(默认为100%)."

        public static bool ReviveHero(JUnit whichHero, float x, float y, bool doEyecandy)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("ReviveHero"), whichHero.Handle, x, y, doEyecandy);
        }


        /// title = "立即复活(指定点)"
        /// description = "立即复活 ${英雄} 在 ${指定点} , ${Show/Hide} 复活动画"
        /// comment = "如果英雄正在祭坛复活,则会退回部分花费(默认为100%)."

        public static bool ReviveHeroLoc(JUnit whichHero, JLocation loc, bool doEyecandy)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("ReviveHeroLoc"), whichHero.Handle, loc.Handle, doEyecandy);
        }

        public static void SetUnitExploded(JUnit whichUnit, bool exploded)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitExploded"), whichUnit.Handle, exploded);
        }


        /// title = "设置无敌/可攻击"
        /// description = "设置 ${单位} ${Invulnerable/Vulnerable}"
        /// comment = ""

        public static void SetUnitInvulnerable(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitInvulnerable"), whichUnit.Handle, flag);
        }


        /// title = "暂停/恢复 [R]"
        /// description = "设置 ${单位} ${Pause/Unpause}"
        /// comment = ""

        public static void PauseUnit(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("PauseUnit"), whichUnit.Handle, flag);
        }

        public static bool IsUnitPaused(JUnit whichHero)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitPaused"), whichHero.Handle);
        }


        /// title = "设置碰撞开关"
        /// description = "设置 ${单位} ${On/Off} 碰撞"
        /// comment = "关闭碰撞的单位无视障碍物,但其他单位仍视其为障碍物."

        public static void SetUnitPathing(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitPathing"), whichUnit.Handle, flag);
        }


        /// title = "清空选择(所有玩家)"
        /// description = "清空所有玩家的选择"
        /// comment = "使玩家取消选择所有已选单位."

        public static void ClearSelection()
        {
            War3.CallNative(War3.GetNativeFunction("ClearSelection"));
        }

        public static void SelectUnit(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SelectUnit"), whichUnit.Handle, flag);
        }


        /// title = "单位附加值"
        /// description = "${单位} 的附加值"
        /// comment = "单位附加值不可改变. 可以做一些特殊用途. 比如TD地图中的建筑贩卖价格."

        public static int GetUnitPointValue(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitPointValue"), whichUnit.Handle);
        }


        /// title = "单位附加值(指定单位类型)"
        /// description = "${单位类型} 的附加值"
        /// comment = "单位附加值不可改变. 可以做一些特殊用途. 比如TD地图中的建筑贩卖价格."

        public static int GetUnitPointValueByType(int unitType)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitPointValueByType"), unitType);
        }


        /// title = "给予物品 [R]"
        /// description = "给予 ${单位} ${物品}"
        /// comment = ""

        public static bool UnitAddItem(JUnit whichUnit, JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitAddItem"), whichUnit.Handle, whichItem.Handle);
        }

        public static JItem UnitAddItemById(JUnit whichUnit, int itemId)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("UnitAddItemById"), whichUnit.Handle, itemId);
        }


        /// title = "新建物品到指定物品栏 [R]"
        /// description = "给予 ${单位} ${物品类型} 并放在物品栏# ${数值}"
        /// comment = "注意: 物品栏编号从0-5,而不是1-6. 该动作创建的物品不被'最后创建的物品'所记录."

        public static bool UnitAddItemToSlotById(JUnit whichUnit, int itemId, int itemSlot)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitAddItemToSlotById"), whichUnit.Handle, itemId, itemSlot);
        }

        public static void UnitRemoveItem(JUnit whichUnit, JItem whichItem)
        {
            War3.CallNative(War3.GetNativeFunction("UnitRemoveItem"), whichUnit.Handle, whichItem.Handle);
        }

        public static JItem UnitRemoveItemFromSlot(JUnit whichUnit, int itemSlot)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("UnitRemoveItemFromSlot"), whichUnit.Handle, itemSlot);
        }


        /// title = "持有物品"
        /// description = "${Hero} 拥有 ${物品}"
        /// comment = ""

        public static bool UnitHasItem(JUnit whichUnit, JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitHasItem"), whichUnit.Handle, whichItem.Handle);
        }


        /// title = "单位持有物品"
        /// description = "${单位} 物品栏第 ${Index} 格的物品"
        /// comment = "第一个单位格的位置为0."

        public static JItem UnitItemInSlot(JUnit whichUnit, int itemSlot)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("UnitItemInSlot"), whichUnit.Handle, itemSlot);
        }

        public static int UnitInventorySize(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("UnitInventorySize"), whichUnit.Handle);
        }


        /// title = "发布丢弃物品命令(指定坐标) [R]"
        /// description = "命令 ${单位} 丢弃物品 ${物品} 到坐标:(${X},${Y})"
        /// comment = ""

        public static bool UnitDropItemPoint(JUnit whichUnit, JItem whichItem, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDropItemPoint"), whichUnit.Handle, whichItem.Handle, x, y);
        }


        /// title = "移动物品到物品栏 [R]"
        /// description = "命令 ${单位} 移动 ${物品} 到物品栏# ${Index}"
        /// comment = "只有当单位持有该物品时才有效. 注意: 该函数中物品栏编号从0-5,而不是1-6."

        public static bool UnitDropItemSlot(JUnit whichUnit, JItem whichItem, int slot)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDropItemSlot"), whichUnit.Handle, whichItem.Handle, slot);
        }

        public static bool UnitDropItemTarget(JUnit whichUnit, JItem whichItem, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDropItemTarget"), whichUnit.Handle, whichItem.Handle, target.Handle);
        }


        /// title = "使用物品(无目标)"
        /// description = "命令 ${单位} 使用 ${物品}"
        /// comment = ""

        public static bool UnitUseItem(JUnit whichUnit, JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitUseItem"), whichUnit.Handle, whichItem.Handle);
        }


        /// title = "使用物品(指定坐标)"
        /// description = "命令 ${单位} 使用 ${物品} ,目标坐标:(${X},${Y})"
        /// comment = ""

        public static bool UnitUseItemPoint(JUnit whichUnit, JItem whichItem, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitUseItemPoint"), whichUnit.Handle, whichItem.Handle, x, y);
        }


        /// title = "使用物品(对单位)"
        /// description = "命令 ${单位} 使用 ${物品} ,目标: ${单位}"
        /// comment = ""

        public static bool UnitUseItemTarget(JUnit whichUnit, JItem whichItem, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitUseItemTarget"), whichUnit.Handle, whichItem.Handle, target.Handle);
        }


        /// title = "单位所在X轴坐标 [R]"
        /// description = "${单位} 所在X轴坐标"
        /// comment = ""

        public static float GetUnitX(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitX"), whichUnit.Handle);
        }


        /// title = "单位所在Y轴坐标 [R]"
        /// description = "${单位} 所在Y轴坐标"
        /// comment = ""

        public static float GetUnitY(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitY"), whichUnit.Handle);
        }


        /// title = "单位位置"
        /// description = "${单位} 的位置"
        /// comment = "会创建点."

        public static JLocation GetUnitLoc(JUnit whichUnit)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("GetUnitLoc"), whichUnit.Handle);
        }


        /// title = "面向角度"
        /// description = "${单位} 的面向角度"
        /// comment = "采用角度制. 0度为正东方向, 90度为正北方向."

        public static float GetUnitFacing(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitFacing"), whichUnit.Handle);
        }


        /// title = "当前移动速度"
        /// description = "${单位} 的当前移动速度"
        /// comment = ""

        public static float GetUnitMoveSpeed(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitMoveSpeed"), whichUnit.Handle);
        }


        /// title = "默认移动速度"
        /// description = "${单位} 的默认移动速度"
        /// comment = ""

        public static float GetUnitDefaultMoveSpeed(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultMoveSpeed"), whichUnit.Handle);
        }


        /// title = "属性 [R]"
        /// description = "${单位} 的 ${Property}"
        /// comment = ""

        public static float GetUnitState(JUnit whichUnit, JUnitState whichUnitState)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitState"), whichUnit.Handle, whichUnitState.Handle);
        }


        /// title = "单位所有者"
        /// description = "${单位} 的所有者"
        /// comment = ""

        public static JPlayer GetOwningPlayer(JUnit whichUnit)
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetOwningPlayer"), whichUnit.Handle);
        }


        /// title = "指定单位的类型"
        /// description = "${单位} 的类型"
        /// comment = ""

        public static int GetUnitTypeId(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitTypeId"), whichUnit.Handle);
        }


        /// title = "单位种族"
        /// description = "${单位} 所属种族"
        /// comment = "物体编辑器中设置的单位所属种族."

        public static JRace GetUnitRace(JUnit whichUnit)
        {
            return War3.CallNative<JRace>(War3.GetNativeFunction("GetUnitRace"), whichUnit.Handle);
        }


        /// title = "单位名字"
        /// description = "${单位} 的名字"
        /// comment = ""

        public static string GetUnitName(JUnit whichUnit)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetUnitName"), whichUnit.Handle);
        }


        /// title = "单位使用人口数量"
        /// description = "${单位} 使用的人口数量"
        /// comment = ""

        public static int GetUnitFoodUsed(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitFoodUsed"), whichUnit.Handle);
        }


        /// title = "单位提供人口数量"
        /// description = "${单位} 提供的人口数量"
        /// comment = ""

        public static int GetUnitFoodMade(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitFoodMade"), whichUnit.Handle);
        }


        /// title = "单位提供人口数量(指定单位类型)"
        /// description = "${单位类型} 提供的人口数量"
        /// comment = ""

        public static int GetFoodMade(int unitId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetFoodMade"), unitId);
        }


        /// title = "单位使用人口数量(指定单位类型)"
        /// description = "${单位类型} 使用的人口数量"
        /// comment = ""

        public static int GetFoodUsed(int unitId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetFoodUsed"), unitId);
        }


        /// title = "允许/禁止 人口占用 [R]"
        /// description = "设置 ${单位} : ${Enable/Disable} 其人口占用"
        /// comment = ""

        public static void SetUnitUseFood(JUnit whichUnit, bool useFood)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitUseFood"), whichUnit.Handle, useFood);
        }


        /// title = "单位集结点"
        /// description = "${单位} 的集结点位置"
        /// comment = "如果单位没有设置集结点,则返回null. 设置自己为集结点可取消集结点设置. 会创建点."

        public static JLocation GetUnitRallyPoint(JUnit whichUnit)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("GetUnitRallyPoint"), whichUnit.Handle);
        }


        /// title = "单位集结点目标"
        /// description = "${单位} 的集结点目标"
        /// comment = "如果指定单位没有设置集结点到单位目标,则返回null."

        public static JUnit GetUnitRallyUnit(JUnit whichUnit)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("GetUnitRallyUnit"), whichUnit.Handle);
        }


        /// title = "单位集结点目标"
        /// description = "${单位} 的集结点目标"
        /// comment = "如果指定单位没有设置集结点到可破坏物上,则返回null."

        public static JDestructable GetUnitRallyDestructable(JUnit whichUnit)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("GetUnitRallyDestructable"), whichUnit.Handle);
        }


        /// title = "在单位组"
        /// description = "${单位} 在 ${单位组} 中"
        /// comment = ""

        public static bool IsUnitInGroup(JUnit whichUnit, JGroup whichGroup)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitInGroup"), whichUnit.Handle, whichGroup.Handle);
        }


        /// title = "是玩家组里玩家的单位"
        /// description = "${单位} 属于 ${玩家组} 里的玩家"
        /// comment = "判断单位是否属于这个玩家组里的玩家。"

        public static bool IsUnitInForce(JUnit whichUnit, JForce whichForce)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitInForce"), whichUnit.Handle, whichForce.Handle);
        }


        /// title = "是玩家的单位"
        /// description = "${单位} 属于 ${Player}"
        /// comment = "判断单位是否属于这个玩家。"

        public static bool IsUnitOwnedByPlayer(JUnit whichUnit, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitOwnedByPlayer"), whichUnit.Handle, whichPlayer.Handle);
        }


        /// title = "是玩家的同盟单位"
        /// description = "${单位} 是 ${Player} 的同盟单位"
        /// comment = "包括中立状态. 单向判断玩家对单位是否为不侵犯状态."

        public static bool IsUnitAlly(JUnit whichUnit, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitAlly"), whichUnit.Handle, whichPlayer.Handle);
        }


        /// title = "是玩家的敌对单位"
        /// description = "${单位} 是 ${Player} 的敌对单位"
        /// comment = "不包括中立状态. 单向判断玩家对单位是否为敌对侵犯."

        public static bool IsUnitEnemy(JUnit whichUnit, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitEnemy"), whichUnit.Handle, whichPlayer.Handle);
        }


        /// title = "单位可见"
        /// description = "${单位} 对 ${Player} 可见"
        /// comment = ""

        public static bool IsUnitVisible(JUnit whichUnit, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitVisible"), whichUnit.Handle, whichPlayer.Handle);
        }


        /// title = "被检测到"
        /// description = "${单位} 处在 ${玩家} 的真实视野范围内"
        /// comment = "用来判断单位在这个玩家反隐形范围内，注：不包含该玩家同盟的反隐范围。"

        public static bool IsUnitDetected(JUnit whichUnit, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitDetected"), whichUnit.Handle, whichPlayer.Handle);
        }


        /// title = "单位不可见"
        /// description = "${单位} 对 ${Player} 不可见"
        /// comment = ""

        public static bool IsUnitInvisible(JUnit whichUnit, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitInvisible"), whichUnit.Handle, whichPlayer.Handle);
        }


        /// title = "单位在迷雾中"
        /// description = "${单位} 在 ${Player} 的迷雾范围内"
        /// comment = "黑色阴影内的单位不被计算在内."

        public static bool IsUnitFogged(JUnit whichUnit, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitFogged"), whichUnit.Handle, whichPlayer.Handle);
        }


        /// title = "单位在黑色阴影中"
        /// description = "${单位} 在 ${Player} 的黑色阴影内"
        /// comment = ""

        public static bool IsUnitMasked(JUnit whichUnit, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitMasked"), whichUnit.Handle, whichPlayer.Handle);
        }


        /// title = "被玩家选择"
        /// description = "${单位} 被 ${Player} 选择"
        /// comment = ""

        public static bool IsUnitSelected(JUnit whichUnit, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitSelected"), whichUnit.Handle, whichPlayer.Handle);
        }


        /// title = "单位种族检查"
        /// description = "${单位} 是 ${Race}"
        /// comment = ""

        public static bool IsUnitRace(JUnit whichUnit, JRace whichRace)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitRace"), whichUnit.Handle, whichRace.Handle);
        }


        /// title = "单位类别检查"
        /// description = "${单位} 是 ${Type}"
        /// comment = ""

        public static bool IsUnitType(JUnit whichUnit, JUnitType whichUnitType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitType"), whichUnit.Handle, whichUnitType.Handle);
        }


        /// title = "单位检查"
        /// description = "${单位} 与 ${单位}相同"
        /// comment = "用来判断两个单位是否相等。"

        public static bool IsUnit(JUnit whichUnit, JUnit whichSpecifiedUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnit"), whichUnit.Handle, whichSpecifiedUnit.Handle);
        }


        /// title = "在指定单位范围内 [R]"
        /// description = "${单位} 在距离 ${指定单位} ${范围} 范围内"
        /// comment = ""

        public static bool IsUnitInRange(JUnit whichUnit, JUnit otherUnit, float distance)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitInRange"), whichUnit.Handle, otherUnit.Handle, distance);
        }


        /// title = "在指定坐标范围内 [R]"
        /// description = "${单位} 在距离坐标(${X},${Y}) ${范围} 范围内"
        /// comment = ""

        public static bool IsUnitInRangeXY(JUnit whichUnit, float x, float y, float distance)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitInRangeXY"), whichUnit.Handle, x, y, distance);
        }


        /// title = "在指定点范围内 [R]"
        /// description = "${单位} 在距离 ${指定点} ${范围} 范围内"
        /// comment = ""

        public static bool IsUnitInRangeLoc(JUnit whichUnit, JLocation whichLocation, float distance)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitInRangeLoc"), whichUnit.Handle, whichLocation.Handle, distance);
        }

        public static bool IsUnitHidden(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitHidden"), whichUnit.Handle);
        }

        public static bool IsUnitIllusion(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitIllusion"), whichUnit.Handle);
        }

        public static bool IsUnitInTransport(JUnit whichUnit, JUnit whichTransport)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitInTransport"), whichUnit.Handle, whichTransport.Handle);
        }

        public static bool IsUnitLoaded(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitLoaded"), whichUnit.Handle);
        }


        /// title = "单位类型是英雄单位"
        /// description = "${UnitType} 是英雄单位"

        public static bool IsHeroUnitId(int unitId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsHeroUnitId"), unitId);
        }


        /// title = "单位类别检查(指定单位类型)"
        /// description = "${单位类型} 是 ${Type}"
        /// comment = ""

        public static bool IsUnitIdType(int unitId, JUnitType whichUnitType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitIdType"), unitId, whichUnitType.Handle);
        }


        /// title = "共享视野 [R]"
        /// description = "设置 ${单位} 的视野对 ${Player} ${on/off}"
        /// comment = ""

        public static void UnitShareVision(JUnit whichUnit, JPlayer whichPlayer, bool share)
        {
            War3.CallNative(War3.GetNativeFunction("UnitShareVision"), whichUnit.Handle, whichPlayer.Handle, share);
        }


        /// title = "暂停尸体腐烂 [R]"
        /// description = " 设置 ${单位} 的尸体腐烂状态: ${Suspend/Resume}"
        /// comment = "只对已完成死亡动作的尸体有效."

        public static void UnitSuspendDecay(JUnit whichUnit, bool suspend)
        {
            War3.CallNative(War3.GetNativeFunction("UnitSuspendDecay"), whichUnit.Handle, suspend);
        }


        /// title = "添加类别 [R]"
        /// description = "为 ${单位} 添加 ${Classification} 类别"
        /// comment = "已去除所有无效类别."

        public static bool UnitAddType(JUnit whichUnit, JUnitType whichUnitType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitAddType"), whichUnit.Handle, whichUnitType.Handle);
        }


        /// title = "删除类别 [R]"
        /// description = "为 ${单位} 删除 ${Classification} 类别"
        /// comment = "已去除所有无效类别."

        public static bool UnitRemoveType(JUnit whichUnit, JUnitType whichUnitType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitRemoveType"), whichUnit.Handle, whichUnitType.Handle);
        }


        /// title = "添加技能 [R]"
        /// description = "为 ${单位} 添加 ${技能}"
        /// comment = ""

        public static bool UnitAddAbility(JUnit whichUnit, int abilityId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitAddAbility"), whichUnit.Handle, abilityId);
        }


        /// title = "删除技能 [R]"
        /// description = "为 ${单位} 删除 ${技能}"
        /// comment = ""

        public static bool UnitRemoveAbility(JUnit whichUnit, int abilityId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitRemoveAbility"), whichUnit.Handle, abilityId);
        }


        /// title = "设置技能永久性 [R]"
        /// description = "设置 ${单位} ${是否} ${技能} 永久性"
        /// comment = "如触发添加给单位的技能就是非永久性的,非永久性技能在变身并回复之后会丢失掉. 这类情况就需要设置技能永久性."

        public static bool UnitMakeAbilityPermanent(JUnit whichUnit, bool permanent, int abilityId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitMakeAbilityPermanent"), whichUnit.Handle, permanent, abilityId);
        }


        /// title = "删除魔法效果(指定极性) [R]"
        /// description = "删除 ${单位} 的附带Buff,(${Include/Exclude} 正面Buff, ${Include/Exclude} 负面Buff)"

        public static void UnitRemoveBuffs(JUnit whichUnit, bool removePositive, bool removeNegative)
        {
            War3.CallNative(War3.GetNativeFunction("UnitRemoveBuffs"), whichUnit.Handle, removePositive, removeNegative);
        }


        /// title = "删除魔法效果(详细类别) [R]"
        /// description = "删除 ${单位} 的附带Buff,(${Include/Exclude} 正面Buff, ${Include/Exclude} 负面Buff${Include/Exclude} 魔法Buff, ${Include/Exclude} 物理Buff${Include/Exclude} 生命周期, ${Include/Exclude} 光环效果${Include/Exclude} 不可驱散Buff)"

        public static void UnitRemoveBuffsEx(JUnit whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel)
        {
            War3.CallNative(War3.GetNativeFunction("UnitRemoveBuffsEx"), whichUnit.Handle, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel);
        }

        public static bool UnitHasBuffsEx(JUnit whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitHasBuffsEx"), whichUnit.Handle, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel);
        }


        /// title = "拥有Buff数量 [R]"
        /// description = "${单位} 的附带Buff数量,(${Include/Exclude} 正面Buff, ${Include/Exclude} 负面Buff${Include/Exclude} 魔法Buff, ${Include/Exclude} 物理Buff${Include/Exclude} 生命周期, ${Include/Exclude} 光环效果${Include/Exclude} 不可驱散Buff)"
        /// comment = ""

        public static int UnitCountBuffsEx(JUnit whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("UnitCountBuffsEx"), whichUnit.Handle, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel);
        }

        public static void UnitAddSleep(JUnit whichUnit, bool add)
        {
            War3.CallNative(War3.GetNativeFunction("UnitAddSleep"), whichUnit.Handle, add);
        }

        public static bool UnitCanSleep(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitCanSleep"), whichUnit.Handle);
        }


        /// title = "控制单位睡眠状态"
        /// description = "使 ${单位} ${Sleep/Remain Awake}"
        /// comment = "使用该功能前必须用触发为单位添加'一直睡眠'技能."

        public static void UnitAddSleepPerm(JUnit whichUnit, bool add)
        {
            War3.CallNative(War3.GetNativeFunction("UnitAddSleepPerm"), whichUnit.Handle, add);
        }


        /// title = "允许控制睡眠状态"
        /// description = "允许控制 ${单位} 的睡眠状态"
        /// comment = "即该单位拥有'一直睡眠'技能."

        public static bool UnitCanSleepPerm(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitCanSleepPerm"), whichUnit.Handle);
        }

        public static bool UnitIsSleeping(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitIsSleeping"), whichUnit.Handle);
        }

        public static void UnitWakeUp(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("UnitWakeUp"), whichUnit.Handle);
        }


        /// title = "设置生命周期 [R]"
        /// description = "为 ${单位} 设置 ${Buff Type} 类型的生命周期,持续时间为 ${Duration} 秒"
        /// comment = ""

        public static void UnitApplyTimedLife(JUnit whichUnit, int buffId, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("UnitApplyTimedLife"), whichUnit.Handle, buffId, duration);
        }

        public static bool UnitIgnoreAlarm(JUnit whichUnit, bool flag)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitIgnoreAlarm"), whichUnit.Handle, flag);
        }

        public static bool UnitIgnoreAlarmToggled(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitIgnoreAlarmToggled"), whichUnit.Handle);
        }


        /// title = "重置技能CD"
        /// description = "重置 ${单位} 的所有技能冷却时间"
        /// comment = "如果要重置单一技能的CD,可以通过删除技能+添加技能+设置技能等级来完成."

        public static void UnitResetCooldown(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("UnitResetCooldown"), whichUnit.Handle);
        }


        /// title = "设置建筑建造进度条"
        /// description = "设置 ${Building} 的建造进度条为 ${Progress}%"
        /// comment = "只作用于正在建造的建筑."

        public static void UnitSetConstructionProgress(JUnit whichUnit, int constructionPercentage)
        {
            War3.CallNative(War3.GetNativeFunction("UnitSetConstructionProgress"), whichUnit.Handle, constructionPercentage);
        }


        /// title = "设置建筑升级进度条"
        /// description = "设置 ${Building} 的升级进度条为 ${Progress}%"
        /// comment = "只作用于正在升级的建筑. 是建筑A升级为建筑B的升级,不是科技的研究."

        public static void UnitSetUpgradeProgress(JUnit whichUnit, int upgradePercentage)
        {
            War3.CallNative(War3.GetNativeFunction("UnitSetUpgradeProgress"), whichUnit.Handle, upgradePercentage);
        }


        /// title = "暂停/恢复生命周期 [R]"
        /// description = "使 ${单位} ${Pause/Unpause} 生命周期"
        /// comment = "只有召唤单位有生命周期."

        public static void UnitPauseTimedLife(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("UnitPauseTimedLife"), whichUnit.Handle, flag);
        }

        public static void UnitSetUsesAltIcon(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("UnitSetUsesAltIcon"), whichUnit.Handle, flag);
        }


        /// title = "伤害区域 [R]"
        /// description = "命令 ${单位} 在 ${Seconds} 秒后对半径为 ${Size} 圆心为(${X},${Y})的范围造成 ${Amount} 点伤害(${是} 攻击伤害, ${是}远程攻击) 攻击类型: ${AttackType} 伤害类型: ${DamageType} 装甲类型: ${WeaponType}"
        /// comment = "该动作不会打断单位动作. 由该动作伤害/杀死单位同样正常触发'受到伤害'和'死亡'单位事件."

        public static bool UnitDamagePoint(JUnit whichUnit, float delay, float radius, float x, float y, float amount, bool attack, bool ranged, JAttackType attackType, JDamageType damageType, JWeaponType weaponType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDamagePoint"), whichUnit.Handle, delay, radius, x, y, amount, attack, ranged, attackType.Handle, damageType.Handle, weaponType.Handle);
        }


        /// title = "伤害目标 [R]"
        /// description = "命令 ${单位} 对 ${Target} 造成 ${Amount} 点伤害(${是} 攻击伤害, ${是}远程攻击) 攻击类型: ${AttackType} 伤害类型: ${DamageType} 武器类型: ${WeaponType}"
        /// comment = "该动作不会打断单位动作. 由该动作伤害/杀死单位同样正常触发'受到伤害'和'死亡'单位事件."

        public static bool UnitDamageTarget(JUnit whichUnit, JWidget target, float amount, bool attack, bool ranged, JAttackType attackType, JDamageType damageType, JWeaponType weaponType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDamageTarget"), whichUnit.Handle, target.Handle, amount, attack, ranged, attackType.Handle, damageType.Handle, weaponType.Handle);
        }


        /// title = "发布命令(无目标)"
        /// description = "对 ${单位} 发布 ${Order} 命令"
        /// comment = ""

        public static bool IssueImmediateOrder(JUnit whichUnit, string order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueImmediateOrder"), whichUnit.Handle, order);
        }


        /// title = "发布命令(无目标)(ID)"
        /// description = "对 ${单位} 发布 ${Order} 命令"
        /// comment = ""

        public static bool IssueImmediateOrderById(JUnit whichUnit, int order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueImmediateOrderById"), whichUnit.Handle, order);
        }


        /// title = "发布命令(指定坐标) [R]"
        /// description = "对 ${单位} 发布 ${Order} 命令到坐标:(${X},${Y})"
        /// comment = ""

        public static bool IssuePointOrder(JUnit whichUnit, string order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssuePointOrder"), whichUnit.Handle, order, x, y);
        }


        /// title = "发布命令(指定点)"
        /// description = "对 ${单位} 发布 ${Order} 命令到目标点: ${指定点}"
        /// comment = ""

        public static bool IssuePointOrderLoc(JUnit whichUnit, string order, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssuePointOrderLoc"), whichUnit.Handle, order, whichLocation.Handle);
        }


        /// title = "发布命令(指定坐标)(ID)"
        /// description = "对 ${单位} 发布 ${Order} 命令到坐标:(${X},${Y})"
        /// comment = ""

        public static bool IssuePointOrderById(JUnit whichUnit, int order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssuePointOrderById"), whichUnit.Handle, order, x, y);
        }


        /// title = "发布命令(指定点)(ID)"
        /// description = "对 ${单位} 发布 ${Order} 命令到目标点: ${指定点}"
        /// comment = ""

        public static bool IssuePointOrderByIdLoc(JUnit whichUnit, int order, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssuePointOrderByIdLoc"), whichUnit.Handle, order, whichLocation.Handle);
        }


        /// title = "发布命令(指定单位)"
        /// description = "对 ${单位} 发布 ${Order} 命令到目标: ${单位}"
        /// comment = ""

        public static bool IssueTargetOrder(JUnit whichUnit, string order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueTargetOrder"), whichUnit.Handle, order, targetWidget.Handle);
        }


        /// title = "发布命令(指定单位)(ID)"
        /// description = "对 ${单位} 发布 ${Order} 命令到目标: ${单位}"
        /// comment = ""

        public static bool IssueTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueTargetOrderById"), whichUnit.Handle, order, targetWidget.Handle);
        }

        public static bool IssueInstantPointOrder(JUnit whichUnit, string order, float x, float y, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueInstantPointOrder"), whichUnit.Handle, order, x, y, instantTargetWidget.Handle);
        }

        public static bool IssueInstantPointOrderById(JUnit whichUnit, int order, float x, float y, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueInstantPointOrderById"), whichUnit.Handle, order, x, y, instantTargetWidget.Handle);
        }

        public static bool IssueInstantTargetOrder(JUnit whichUnit, string order, JWidget targetWidget, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueInstantTargetOrder"), whichUnit.Handle, order, targetWidget.Handle, instantTargetWidget.Handle);
        }

        public static bool IssueInstantTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueInstantTargetOrderById"), whichUnit.Handle, order, targetWidget.Handle, instantTargetWidget.Handle);
        }

        public static bool IssueBuildOrder(JUnit whichPeon, string unitToBuild, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueBuildOrder"), whichPeon.Handle, unitToBuild, x, y);
        }


        /// title = "发布建造命令(指定坐标) [R]"
        /// description = "命令 ${单位} 建造 ${单位类型} 在坐标:(${X},${Y})"
        /// comment = ""

        public static bool IssueBuildOrderById(JUnit whichPeon, int unitId, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueBuildOrderById"), whichPeon.Handle, unitId, x, y);
        }


        /// title = "发布中介命令(无目标)"
        /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令"
        /// comment = "可以用来对非本玩家单位发布命令."

        public static bool IssueNeutralImmediateOrder(JPlayer forWhichPlayer, JUnit neutralStructure, string unitToBuild)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralImmediateOrder"), forWhichPlayer.Handle, neutralStructure.Handle, unitToBuild);
        }


        /// title = "发布中介命令(无目标)(ID)"
        /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令"
        /// comment = "可以用来对非本玩家单位发布命令."

        public static bool IssueNeutralImmediateOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralImmediateOrderById"), forWhichPlayer.Handle, neutralStructure.Handle, unitId);
        }


        /// title = "发布中介命令(指定坐标)"
        /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令到坐标:(${X},${Y})"
        /// comment = "可以用来对非本玩家单位发布命令."

        public static bool IssueNeutralPointOrder(JPlayer forWhichPlayer, JUnit neutralStructure, string unitToBuild, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralPointOrder"), forWhichPlayer.Handle, neutralStructure.Handle, unitToBuild, x, y);
        }


        /// title = "发布中介命令(指定坐标)(ID)"
        /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令到坐标:(${X},${Y})"
        /// comment = "可以用来对非本玩家单位发布命令."

        public static bool IssueNeutralPointOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralPointOrderById"), forWhichPlayer.Handle, neutralStructure.Handle, unitId, x, y);
        }


        /// title = "发布中介命令(指定单位)"
        /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令到目标: ${单位}"
        /// comment = "可以用来对非本玩家单位发布命令."

        public static bool IssueNeutralTargetOrder(JPlayer forWhichPlayer, JUnit neutralStructure, string unitToBuild, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralTargetOrder"), forWhichPlayer.Handle, neutralStructure.Handle, unitToBuild, target.Handle);
        }


        /// title = "发布中介命令(指定单位)(ID)"
        /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令到目标: ${单位}"
        /// comment = "可以用来对非本玩家单位发布命令."

        public static bool IssueNeutralTargetOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralTargetOrderById"), forWhichPlayer.Handle, neutralStructure.Handle, unitId, target.Handle);
        }


        /// title = "当前命令ID"
        /// description = "${单位} 的当前命令ID."
        /// comment = ""

        public static int GetUnitCurrentOrder(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitCurrentOrder"), whichUnit.Handle);
        }


        /// title = "设置黄金储量"
        /// description = "设置 ${金矿} 的黄金储量为 ${Quantity}"
        /// comment = ""

        public static void SetResourceAmount(JUnit whichUnit, int amount)
        {
            War3.CallNative(War3.GetNativeFunction("SetResourceAmount"), whichUnit.Handle, amount);
        }

        public static void AddResourceAmount(JUnit whichUnit, int amount)
        {
            War3.CallNative(War3.GetNativeFunction("AddResourceAmount"), whichUnit.Handle, amount);
        }


        /// title = "储金量"
        /// description = "${金矿} 的储金量"
        /// comment = "只对金矿有效."

        public static int GetResourceAmount(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetResourceAmount"), whichUnit.Handle);
        }


        /// title = "传送门目的地X坐标"
        /// description = "${传送门} 的目的地X坐标"
        /// comment = ""

        public static float WaygateGetDestinationX(JUnit waygate)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("WaygateGetDestinationX"), waygate.Handle);
        }


        /// title = "传送门目的地Y坐标"
        /// description = "${传送门} 的目的地Y坐标"
        /// comment = ""

        public static float WaygateGetDestinationY(JUnit waygate)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("WaygateGetDestinationY"), waygate.Handle);
        }


        /// title = "设置传送门目的坐标 [R]"
        /// description = "设置 ${传送门} 的目的地为(${X},${Y})"
        /// comment = ""

        public static void WaygateSetDestination(JUnit waygate, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("WaygateSetDestination"), waygate.Handle, x, y);
        }

        public static void WaygateActivate(JUnit waygate, bool activate)
        {
            War3.CallNative(War3.GetNativeFunction("WaygateActivate"), waygate.Handle, activate);
        }

        public static bool WaygateIsActive(JUnit waygate)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("WaygateIsActive"), waygate.Handle);
        }


        /// title = "添加物品(所有市场)"
        /// description = "添加 ${物品类型} 到所有市场并设置库存量: ${Count} 最大库存量: ${Max}"
        /// comment = "影响所有拥有'出售物品'技能的单位."

        public static void AddItemToAllStock(int itemId, int currentStock, int stockMax)
        {
            War3.CallNative(War3.GetNativeFunction("AddItemToAllStock"), itemId, currentStock, stockMax);
        }

        public static void AddItemToStock(JUnit whichUnit, int itemId, int currentStock, int stockMax)
        {
            War3.CallNative(War3.GetNativeFunction("AddItemToStock"), whichUnit.Handle, itemId, currentStock, stockMax);
        }


        /// title = "添加单位(所有市场)"
        /// description = "添加 ${单位类型} 到所有市场并设置库存量: ${Count} 最大库存量: ${Max}"
        /// comment = "影响所有拥有'出售单位'技能的单位."

        public static void AddUnitToAllStock(int unitId, int currentStock, int stockMax)
        {
            War3.CallNative(War3.GetNativeFunction("AddUnitToAllStock"), unitId, currentStock, stockMax);
        }

        public static void AddUnitToStock(JUnit whichUnit, int unitId, int currentStock, int stockMax)
        {
            War3.CallNative(War3.GetNativeFunction("AddUnitToStock"), whichUnit.Handle, unitId, currentStock, stockMax);
        }


        /// title = "删除物品(所有市场)"
        /// description = "删除 ${物品类型} 从所有市场"
        /// comment = "影响所有拥有'出售物品'技能的单位."

        public static void RemoveItemFromAllStock(int itemId)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveItemFromAllStock"), itemId);
        }

        public static void RemoveItemFromStock(JUnit whichUnit, int itemId)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveItemFromStock"), whichUnit.Handle, itemId);
        }


        /// title = "删除单位(所有市场)"
        /// description = "删除 ${单位类型} 从所有市场"
        /// comment = "影响所有拥有'出售单位'技能的单位."

        public static void RemoveUnitFromAllStock(int unitId)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveUnitFromAllStock"), unitId);
        }

        public static void RemoveUnitFromStock(JUnit whichUnit, int unitId)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveUnitFromStock"), whichUnit.Handle, unitId);
        }


        /// title = "限制物品种类(所有市场)"
        /// description = "限制所有市场的可出售物品种类数为 ${Quantity}"
        /// comment = "影响所有拥有'出售物品'技能的单位."

        public static void SetAllItemTypeSlots(int slots)
        {
            War3.CallNative(War3.GetNativeFunction("SetAllItemTypeSlots"), slots);
        }


        /// title = "限制单位种类(所有市场)"
        /// description = "限制所有市场的可出售单位种类数为 ${Quantity}"
        /// comment = "影响所有拥有'出售单位'技能的单位."

        public static void SetAllUnitTypeSlots(int slots)
        {
            War3.CallNative(War3.GetNativeFunction("SetAllUnitTypeSlots"), slots);
        }


        /// title = "限制物品种类(指定市场)"
        /// description = "限制 ${Marketplace} 的可出售物品种类数为 ${Quantity}"
        /// comment = "只影响有'出售物品'技能的单位."

        public static void SetItemTypeSlots(JUnit whichUnit, int slots)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemTypeSlots"), whichUnit.Handle, slots);
        }


        /// title = "限制单位种类(指定市场)"
        /// description = "限制 ${Marketplace} 的可出售单位种类数为 ${Quantity}"
        /// comment = "只影响有'出售单位'技能的单位."

        public static void SetUnitTypeSlots(JUnit whichUnit, int slots)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitTypeSlots"), whichUnit.Handle, slots);
        }


        /// title = "单位自定义值"
        /// description = "${单位} 的自定义值"
        /// comment = "可使用'单位 - 设置自定义值'来设置该值."

        public static int GetUnitUserData(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitUserData"), whichUnit.Handle);
        }


        /// title = "设置自定义值"
        /// description = "设置 ${单位} 的自定义值为 ${Index}"
        /// comment = "单位自定义值仅用于触发器. 可用来给单位绑定一个整型数据."

        public static void SetUnitUserData(JUnit whichUnit, int data)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitUserData"), whichUnit.Handle, data);
        }

        public static JPlayer Player(int number)
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("Player"), number);
        }


        /// title = "本地玩家 [R]"
        /// description = "本地玩家"
        /// comment = "指代玩家自己,所以对每个玩家返回值都不一样. 如果不清楚该函数的话千万别用,因为很可能因为不同步而导致掉线."

        public static JPlayer GetLocalPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetLocalPlayer"));
        }


        /// title = "是玩家的盟友"
        /// description = "${Player} 是 ${Player} 的盟友"
        /// comment = "包括中立状态. 单向判断玩家A对玩家B联盟不侵犯,即表示玩家A是玩家B的盟友."

        public static bool IsPlayerAlly(JPlayer whichPlayer, JPlayer otherPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPlayerAlly"), whichPlayer.Handle, otherPlayer.Handle);
        }


        /// title = "是玩家的敌人"
        /// description = "${Player} 是 ${Player} 的敌人"
        /// comment = "不包括中立状态. 单向判断玩家A对玩家B敌对侵犯,即表示玩家A是玩家B的盟友."

        public static bool IsPlayerEnemy(JPlayer whichPlayer, JPlayer otherPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPlayerEnemy"), whichPlayer.Handle, otherPlayer.Handle);
        }


        /// title = "在玩家组"
        /// description = "${Player} 在 ${玩家组} 中"
        /// comment = ""

        public static bool IsPlayerInForce(JPlayer whichPlayer, JForce whichForce)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPlayerInForce"), whichPlayer.Handle, whichForce.Handle);
        }


        /// title = "玩家是裁判或观察者 [R]"
        /// description = "${Player}是裁判或观察者"
        /// comment = ""

        public static bool IsPlayerObserver(JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPlayerObserver"), whichPlayer.Handle);
        }


        /// title = "坐标可见"
        /// description = "坐标(${x},${y}) 对 ${玩家} 可见"
        /// comment = ""

        public static bool IsVisibleToPlayer(float x, float y, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsVisibleToPlayer"), x, y, whichPlayer.Handle);
        }


        /// title = "点可见"
        /// description = "${指定点}对 ${Player} 可见"
        /// comment = ""

        public static bool IsLocationVisibleToPlayer(JLocation whichLocation, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsLocationVisibleToPlayer"), whichLocation.Handle, whichPlayer.Handle);
        }


        /// title = "坐标在迷雾中"
        /// description = "坐标(${x},${y}) 在 ${玩家} 的迷雾范围内"
        /// comment = "黑色阴影内的坐标不被计算在内。"

        public static bool IsFoggedToPlayer(float x, float y, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsFoggedToPlayer"), x, y, whichPlayer.Handle);
        }


        /// title = "点在迷雾中"
        /// description = "${指定点} 在 ${Player} 的迷雾范围内"
        /// comment = "黑色阴影内的点不被计算在内."

        public static bool IsLocationFoggedToPlayer(JLocation whichLocation, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsLocationFoggedToPlayer"), whichLocation.Handle, whichPlayer.Handle);
        }


        /// title = "坐标在黑色阴影中"
        /// description = "坐标(${x},${y}) 在 ${玩家} 的黑色阴影内"
        /// comment = ""

        public static bool IsMaskedToPlayer(float x, float y, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsMaskedToPlayer"), x, y, whichPlayer.Handle);
        }


        /// title = "点在黑色阴影中"
        /// description = "${指定点} 在 ${Player} 的黑色阴影内"
        /// comment = ""

        public static bool IsLocationMaskedToPlayer(JLocation whichLocation, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsLocationMaskedToPlayer"), whichLocation.Handle, whichPlayer.Handle);
        }


        /// title = "玩家的种族"
        /// description = "${Player} 的种族"
        /// comment = ""

        public static JRace GetPlayerRace(JPlayer whichPlayer)
        {
            return War3.CallNative<JRace>(War3.GetNativeFunction("GetPlayerRace"), whichPlayer.Handle);
        }


        /// title = "玩家ID - 1 [R]"
        /// description = "${Player} 的玩家ID - 1"
        /// comment = "玩家ID取值1-16."

        public static int GetPlayerId(JPlayer whichPlayer)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerId"), whichPlayer.Handle);
        }


        /// title = "非建筑单位数量"
        /// description = "${Player} 拥有的非建筑单位数量(${Include/Exclude} 未完成的)"
        /// comment = ""

        public static int GetPlayerUnitCount(JPlayer whichPlayer, bool includeIncomplete)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerUnitCount"), whichPlayer.Handle, includeIncomplete);
        }

        public static int GetPlayerTypedUnitCount(JPlayer whichPlayer, string unitName, bool includeIncomplete, bool includeUpgrades)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerTypedUnitCount"), whichPlayer.Handle, unitName, includeIncomplete, includeUpgrades);
        }


        /// title = "建筑数量"
        /// description = "${Player} 拥有的建筑数量(${Include/Exclude} 未完成的)"
        /// comment = ""

        public static int GetPlayerStructureCount(JPlayer whichPlayer, bool includeIncomplete)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerStructureCount"), whichPlayer.Handle, includeIncomplete);
        }


        /// title = "玩家属性"
        /// description = "${Player} ${Property}"
        /// comment = ""

        public static int GetPlayerState(JPlayer whichPlayer, JPlayerState whichPlayerState)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerState"), whichPlayer.Handle, whichPlayerState.Handle);
        }


        /// title = "玩家得分"
        /// description = "${Player} ${Score}"
        /// comment = ""

        public static int GetPlayerScore(JPlayer whichPlayer, JPlayerScore whichPlayerScore)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerScore"), whichPlayer.Handle, whichPlayerScore.Handle);
        }


        /// title = "联盟状态设置"
        /// description = "${Player} 对 ${Player} 开启 ${Alliance Type}"
        /// comment = ""

        public static bool GetPlayerAlliance(JPlayer sourcePlayer, JPlayer otherPlayer, JAllianceType whichAllianceSetting)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetPlayerAlliance"), sourcePlayer.Handle, otherPlayer.Handle, whichAllianceSetting.Handle);
        }

        public static float GetPlayerHandicap(JPlayer whichPlayer)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetPlayerHandicap"), whichPlayer.Handle);
        }

        public static float GetPlayerHandicapXP(JPlayer whichPlayer)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetPlayerHandicapXP"), whichPlayer.Handle);
        }


        /// title = "设置生命上限 [R]"
        /// description = "设置 ${Player} 的生命障碍为正常的 ${Percent}倍"
        /// comment = "生命上限影响玩家拥有单位的生命最大值. 生命之书并不受生命上限限制,所以对英雄血量可能会有偏差."

        public static void SetPlayerHandicap(JPlayer whichPlayer, float handicap)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerHandicap"), whichPlayer.Handle, handicap);
        }


        /// title = "设置经验获得率 [R]"
        /// description = "设置 ${Player} 的经验获得率为正常的 ${Value} 倍"
        /// comment = ""

        public static void SetPlayerHandicapXP(JPlayer whichPlayer, float handicap)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerHandicapXP"), whichPlayer.Handle, handicap);
        }

        public static void SetPlayerTechMaxAllowed(JPlayer whichPlayer, int techid, int maximum)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerTechMaxAllowed"), whichPlayer.Handle, techid, maximum);
        }

        public static int GetPlayerTechMaxAllowed(JPlayer whichPlayer, int techid)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerTechMaxAllowed"), whichPlayer.Handle, techid);
        }


        /// title = "增加科技等级"
        /// description = "增加 ${玩家} 的 ${科技} 科技 ${整数} 级"
        /// comment = "科技等级不能倒退。"

        public static void AddPlayerTechResearched(JPlayer whichPlayer, int techid, int levels)
        {
            War3.CallNative(War3.GetNativeFunction("AddPlayerTechResearched"), whichPlayer.Handle, techid, levels);
        }

        public static void SetPlayerTechResearched(JPlayer whichPlayer, int techid, int setToLevel)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerTechResearched"), whichPlayer.Handle, techid, setToLevel);
        }

        public static bool GetPlayerTechResearched(JPlayer whichPlayer, int techid, bool specificonly)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetPlayerTechResearched"), whichPlayer.Handle, techid, specificonly);
        }

        public static int GetPlayerTechCount(JPlayer whichPlayer, int techid, bool specificonly)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerTechCount"), whichPlayer.Handle, techid, specificonly);
        }

        public static void SetPlayerUnitsOwner(JPlayer whichPlayer, int newOwner)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerUnitsOwner"), whichPlayer.Handle, newOwner);
        }

        public static void CripplePlayer(JPlayer whichPlayer, JForce toWhichPlayers, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("CripplePlayer"), whichPlayer.Handle, toWhichPlayers.Handle, flag);
        }


        /// title = "允许/禁用 技能 [R]"
        /// description = "设置 ${Player} 的 ${技能} 为 ${Enable/Disable}"
        /// comment = "设置玩家能否使用该技能."

        public static void SetPlayerAbilityAvailable(JPlayer whichPlayer, int abilid, bool avail)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerAbilityAvailable"), whichPlayer.Handle, abilid, avail);
        }


        /// title = "设置属性"
        /// description = "设置 ${Player} 的 ${Property} 为 ${Value}"
        /// comment = ""

        public static void SetPlayerState(JPlayer whichPlayer, JPlayerState whichPlayerState, int value)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerState"), whichPlayer.Handle, whichPlayerState.Handle, value);
        }


        /// title = "踢除玩家"
        /// description = "踢除 ${Player} ，玩家的游戏结果为 ${文字}"
        /// comment = ""

        public static void RemovePlayer(JPlayer whichPlayer, JPlayerGameResult gameResult)
        {
            War3.CallNative(War3.GetNativeFunction("RemovePlayer"), whichPlayer.Handle, gameResult.Handle);
        }

        public static void CachePlayerHeroData(JPlayer whichPlayer)
        {
            War3.CallNative(War3.GetNativeFunction("CachePlayerHeroData"), whichPlayer.Handle);
        }


        /// title = "设置地图迷雾(矩形区域) [R]"
        /// description = "为 ${玩家} 设置 ${FogStateVisible} 在 ${矩形区域} (对盟友 ${共享} 视野)"
        /// comment = ""

        public static void SetFogStateRect(JPlayer forWhichPlayer, JFogState whichState, JRect where, bool useSharedVision)
        {
            War3.CallNative(War3.GetNativeFunction("SetFogStateRect"), forWhichPlayer.Handle, whichState.Handle, where.Handle, useSharedVision);
        }


        /// title = "设置地图迷雾(圆范围) [R]"
        /// description = "为 ${玩家} 设置 ${FogStateVisible} 在圆心为(${X},${Y}) 半径为 ${数值} 的范围, (对盟友 ${共享} 视野)"
        /// comment = ""

        public static void SetFogStateRadius(JPlayer forWhichPlayer, JFogState whichState, float centerx, float centerY, float radius, bool useSharedVision)
        {
            War3.CallNative(War3.GetNativeFunction("SetFogStateRadius"), forWhichPlayer.Handle, whichState.Handle, centerx, centerY, radius, useSharedVision);
        }

        public static void SetFogStateRadiusLoc(JPlayer forWhichPlayer, JFogState whichState, JLocation center, float radius, bool useSharedVision)
        {
            War3.CallNative(War3.GetNativeFunction("SetFogStateRadiusLoc"), forWhichPlayer.Handle, whichState.Handle, center.Handle, radius, useSharedVision);
        }


        /// title = "启用/禁用黑色阴影 [R]"
        /// description = "${启用/禁用} 黑色阴影"
        /// comment = ""

        public static void FogMaskEnable(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("FogMaskEnable"), enable);
        }


        /// title = "黑色阴影开启"
        /// description = "黑色阴影开启"
        /// comment = ""

        public static bool IsFogMaskEnabled()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsFogMaskEnabled"));
        }


        /// title = "启用/禁用 战争迷雾 [R]"
        /// description = "${启用/禁用} 战争迷雾"
        /// comment = ""

        public static void FogEnable(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("FogEnable"), enable);
        }


        /// title = "战争迷雾开启"
        /// description = "战争迷雾开启"
        /// comment = ""

        public static bool IsFogEnabled()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsFogEnabled"));
        }


        /// title = "新建可见度修正器(矩形区域) [R]"
        /// description = "新建的 ${玩家} 可见度修正器. 可见度: ${FogStateVisible} 影响区域: ${矩形区域} (对盟友 ${共享} 视野, ${覆盖} 单位视野)"
        /// comment = "会创建可见度修正器."

        public static JFogModifier CreateFogModifierRect(JPlayer forWhichPlayer, JFogState whichState, JRect where, bool useSharedVision, bool afterUnits)
        {
            return War3.CallNative<JFogModifier>(War3.GetNativeFunction("CreateFogModifierRect"), forWhichPlayer.Handle, whichState.Handle, where.Handle, useSharedVision, afterUnits);
        }


        /// title = "新建可见度修正器(圆范围) [R]"
        /// description = "新建的 ${玩家} 可见度修正器. 可见度: ${FogStateVisible} 圆心坐标:(${X},${Y}) 半径: ${数值} (对盟友 ${共享} 视野, ${覆盖} 单位视野)"
        /// comment = "会创建可见度修正器."

        public static JFogModifier CreateFogModifierRadius(JPlayer forWhichPlayer, JFogState whichState, float centerx, float centerY, float radius, bool useSharedVision, bool afterUnits)
        {
            return War3.CallNative<JFogModifier>(War3.GetNativeFunction("CreateFogModifierRadius"), forWhichPlayer.Handle, whichState.Handle, centerx, centerY, radius, useSharedVision, afterUnits);
        }

        public static JFogModifier CreateFogModifierRadiusLoc(JPlayer forWhichPlayer, JFogState whichState, JLocation center, float radius, bool useSharedVision, bool afterUnits)
        {
            return War3.CallNative<JFogModifier>(War3.GetNativeFunction("CreateFogModifierRadiusLoc"), forWhichPlayer.Handle, whichState.Handle, center.Handle, radius, useSharedVision, afterUnits);
        }


        /// title = "删除可见度修正器"
        /// description = "删除 ${Visibility Modifier}"
        /// comment = ""

        public static void DestroyFogModifier(JFogModifier whichFogModifier)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyFogModifier"), whichFogModifier.Handle);
        }


        /// title = "启用可见度修正器"
        /// description = "启用 ${Visibility Modifier}"
        /// comment = ""

        public static void FogModifierStart(JFogModifier whichFogModifier)
        {
            War3.CallNative(War3.GetNativeFunction("FogModifierStart"), whichFogModifier.Handle);
        }


        /// title = "禁用可见度修正器"
        /// description = "禁用 ${Visibility Modifier}"
        /// comment = ""

        public static void FogModifierStop(JFogModifier whichFogModifier)
        {
            War3.CallNative(War3.GetNativeFunction("FogModifierStop"), whichFogModifier.Handle);
        }

        public static JVersion VersionGet()
        {
            return War3.CallNative<JVersion>(War3.GetNativeFunction("VersionGet"));
        }

        public static bool VersionCompatible(JVersion whichVersion)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("VersionCompatible"), whichVersion.Handle);
        }

        public static bool VersionSupported(JVersion whichVersion)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("VersionSupported"), whichVersion.Handle);
        }

        public static void EndGame(bool doScoreScreen)
        {
            War3.CallNative(War3.GetNativeFunction("EndGame"), doScoreScreen);
        }


        /// title = "切换关卡 [R]"
        /// description = "切换到关卡: ${Filename} (${Show/Skip} 计分屏)"
        /// comment = ""

        public static void ChangeLevel(string newLevel, bool doScoreScreen)
        {
            War3.CallNative(War3.GetNativeFunction("ChangeLevel"), newLevel, doScoreScreen);
        }

        public static void RestartGame(bool doScoreScreen)
        {
            War3.CallNative(War3.GetNativeFunction("RestartGame"), doScoreScreen);
        }

        public static void ReloadGame()
        {
            War3.CallNative(War3.GetNativeFunction("ReloadGame"));
        }

        public static void SetCampaignMenuRace(JRace r)
        {
            War3.CallNative(War3.GetNativeFunction("SetCampaignMenuRace"), r.Handle);
        }

        public static void SetCampaignMenuRaceEx(int campaignIndex)
        {
            War3.CallNative(War3.GetNativeFunction("SetCampaignMenuRaceEx"), campaignIndex);
        }

        public static void ForceCampaignSelectScreen()
        {
            War3.CallNative(War3.GetNativeFunction("ForceCampaignSelectScreen"));
        }

        public static void LoadGame(string saveFileName, bool doScoreScreen)
        {
            War3.CallNative(War3.GetNativeFunction("LoadGame"), saveFileName, doScoreScreen);
        }


        /// title = "保存进度 [R]"
        /// description = "保存游戏进度为: ${Filename}"
        /// comment = ""

        public static void SaveGame(string saveFileName)
        {
            War3.CallNative(War3.GetNativeFunction("SaveGame"), saveFileName);
        }

        public static bool RenameSaveDirectory(string sourceDirName, string destDirName)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("RenameSaveDirectory"), sourceDirName, destDirName);
        }

        public static bool RemoveSaveDirectory(string sourceDirName)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("RemoveSaveDirectory"), sourceDirName);
        }

        public static bool CopySaveGame(string sourceSaveName, string destSaveName)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("CopySaveGame"), sourceSaveName, destSaveName);
        }


        /// title = "游戏存档存在"
        /// description = "${存档文件} 已存在"
        /// comment = ""

        public static bool SaveGameExists(string saveName)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveGameExists"), saveName);
        }

        public static void SyncSelections()
        {
            War3.CallNative(War3.GetNativeFunction("SyncSelections"));
        }

        public static void SetFloatGameState(JFGameState whichFloatGameState, float value)
        {
            War3.CallNative(War3.GetNativeFunction("SetFloatGameState"), whichFloatGameState.Handle, value);
        }

        public static float GetFloatGameState(JFGameState whichFloatGameState)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetFloatGameState"), whichFloatGameState.Handle);
        }

        public static void SetIntegerGameState(JIGameState whichIntegerGameState, int value)
        {
            War3.CallNative(War3.GetNativeFunction("SetIntegerGameState"), whichIntegerGameState.Handle, value);
        }

        public static int GetIntegerGameState(JIGameState whichIntegerGameState)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetIntegerGameState"), whichIntegerGameState.Handle);
        }

        public static void SetTutorialCleared(bool cleared)
        {
            War3.CallNative(War3.GetNativeFunction("SetTutorialCleared"), cleared);
        }

        public static void SetMissionAvailable(int campaignNumber, int missionNumber, bool available)
        {
            War3.CallNative(War3.GetNativeFunction("SetMissionAvailable"), campaignNumber, missionNumber, available);
        }

        public static void SetCampaignAvailable(int campaignNumber, bool available)
        {
            War3.CallNative(War3.GetNativeFunction("SetCampaignAvailable"), campaignNumber, available);
        }

        public static void SetOpCinematicAvailable(int campaignNumber, bool available)
        {
            War3.CallNative(War3.GetNativeFunction("SetOpCinematicAvailable"), campaignNumber, available);
        }

        public static void SetEdCinematicAvailable(int campaignNumber, bool available)
        {
            War3.CallNative(War3.GetNativeFunction("SetEdCinematicAvailable"), campaignNumber, available);
        }

        public static JGameDifficulty GetDefaultDifficulty()
        {
            return War3.CallNative<JGameDifficulty>(War3.GetNativeFunction("GetDefaultDifficulty"));
        }

        public static void SetDefaultDifficulty(JGameDifficulty g)
        {
            War3.CallNative(War3.GetNativeFunction("SetDefaultDifficulty"), g.Handle);
        }

        public static void SetCustomCampaignButtonVisible(int whichButton, bool visible)
        {
            War3.CallNative(War3.GetNativeFunction("SetCustomCampaignButtonVisible"), whichButton, visible);
        }

        public static bool GetCustomCampaignButtonVisible(int whichButton)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetCustomCampaignButtonVisible"), whichButton);
        }


        /// title = "关闭游戏录像功能 [R]"
        /// description = "关闭游戏录像功能"
        /// comment = "游戏结束时不保存游戏录像."

        public static void DoNotSaveReplay()
        {
            War3.CallNative(War3.GetNativeFunction("DoNotSaveReplay"));
        }


        /// title = "新建对话框 [R]"
        /// description = "新建对话框"
        /// comment = "创建新的对话框."

        public static JDialog DialogCreate()
        {
            return War3.CallNative<JDialog>(War3.GetNativeFunction("DialogCreate"));
        }


        /// title = "删除 [R]"
        /// description = "删除 ${对话框}"
        /// comment = "将对话框清除出内存.一般来说对话框并不需要删除."

        public static void DialogDestroy(JDialog whichDialog)
        {
            War3.CallNative(War3.GetNativeFunction("DialogDestroy"), whichDialog.Handle);
        }

        public static void DialogClear(JDialog whichDialog)
        {
            War3.CallNative(War3.GetNativeFunction("DialogClear"), whichDialog.Handle);
        }

        public static void DialogSetMessage(JDialog whichDialog, string messageText)
        {
            War3.CallNative(War3.GetNativeFunction("DialogSetMessage"), whichDialog.Handle, messageText);
        }


        /// title = "添加对话框按钮 [R]"
        /// description = "给 ${对话框} 添加按钮, 使用标题: ${文字} 快捷键: ${HotKey}"
        /// comment = "会创建对话框按钮."

        public static JButton DialogAddButton(JDialog whichDialog, string buttonText, int hotkey)
        {
            return War3.CallNative<JButton>(War3.GetNativeFunction("DialogAddButton"), whichDialog.Handle, buttonText, hotkey);
        }


        /// title = "添加退出游戏按钮 [R]"
        /// description = "为 ${对话框} 添加退出游戏按钮(${跳过} 计分屏) 按钮标题为: ${文字} 快捷键为: ${HotKey}"
        /// comment = "该函数创建的按钮并不被纪录到'最后创建的对话框按钮'.当该按钮被点击时会退出游戏"

        public static JButton DialogAddQuitButton(JDialog whichDialog, bool doScoreScreen, string buttonText, int hotkey)
        {
            return War3.CallNative<JButton>(War3.GetNativeFunction("DialogAddQuitButton"), whichDialog.Handle, doScoreScreen, buttonText, hotkey);
        }


        /// title = "显示/隐藏 [R]"
        /// description = "对 ${Player} 设置 ${对话框} 的状态为 ${Show/Hide}"
        /// comment = "对话框不能应用于地图初始化事件."

        public static void DialogDisplay(JPlayer whichPlayer, JDialog whichDialog, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("DialogDisplay"), whichPlayer.Handle, whichDialog.Handle, flag);
        }


        /// title = "读取本地缓存数据"
        /// description = "从本地硬盘读取缓存数据"
        /// comment = "只对单机游戏有效,从本地硬盘读取缓存数据,主要用来实现战役关卡间的数据传递."

        public static bool ReloadGameCachesFromDisk()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("ReloadGameCachesFromDisk"));
        }


        /// title = "新建游戏缓存 [R]"
        /// description = "新建游戏缓存: ${缓存名}"
        /// comment = "创建一个新的游戏缓存,一个地图最多只有有256个游戏缓存."

        public static JGameCache InitGameCache(string campaignFile)
        {
            return War3.CallNative<JGameCache>(War3.GetNativeFunction("InitGameCache"), campaignFile);
        }

        public static bool SaveGameCache(JGameCache whichCache)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveGameCache"), whichCache.Handle);
        }


        /// title = "记录整数"
        /// description = "缓存: ${Game Cache}  类别名: ${Category} 使用名称: ${文字} 记录: ${整数}"
        /// comment = "使用'游戏缓存 - 读取整数'来读取该数值. 名称和类别名不能包含空格."

        public static void StoreInteger(JGameCache cache, string missionKey, string key, int value)
        {
            War3.CallNative(War3.GetNativeFunction("StoreInteger"), cache.Handle, missionKey, key, value);
        }


        /// title = "记录实数"
        /// description = "缓存: ${Game Cache}  类别名: ${Category} 使用名称: ${文字} 记录: ${实数}"
        /// comment = "使用'游戏缓存 - 读取实数'来读取该数值. 名称和类别名不能包含空格."

        public static void StoreReal(JGameCache cache, string missionKey, string key, float value)
        {
            War3.CallNative(War3.GetNativeFunction("StoreReal"), cache.Handle, missionKey, key, value);
        }


        /// title = "记录布尔值"
        /// description = "缓存: ${Game Cache}  类别名: ${Category} 使用名称: ${文字} 记录: ${布尔值}"
        /// comment = "使用'游戏缓存 - 读取布尔值'来读取该值. 名称和类别名不能包含空格."

        public static void StoreBoolean(JGameCache cache, string missionKey, string key, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("StoreBoolean"), cache.Handle, missionKey, key, value);
        }

        public static bool StoreUnit(JGameCache cache, string missionKey, string key, JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("StoreUnit"), cache.Handle, missionKey, key, whichUnit.Handle);
        }


        /// title = "记录字符串"
        /// description = "缓存: ${Game Cache}  类别名: ${Category} 使用名称: ${文字} 记录: ${字符串}"
        /// comment = "使用'游戏缓存 - 读取字符串'来读取该值. 名称和类别名不能包含空格."

        public static bool StoreString(JGameCache cache, string missionKey, string key, string value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("StoreString"), cache.Handle, missionKey, key, value);
        }

        public static void SyncStoredInteger(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredInteger"), cache.Handle, missionKey, key);
        }

        public static void SyncStoredReal(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredReal"), cache.Handle, missionKey, key);
        }

        public static void SyncStoredBoolean(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredBoolean"), cache.Handle, missionKey, key);
        }

        public static void SyncStoredUnit(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredUnit"), cache.Handle, missionKey, key);
        }

        public static void SyncStoredString(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredString"), cache.Handle, missionKey, key);
        }

        public static bool HaveStoredInteger(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredInteger"), cache.Handle, missionKey, key);
        }

        public static bool HaveStoredReal(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredReal"), cache.Handle, missionKey, key);
        }

        public static bool HaveStoredBoolean(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredBoolean"), cache.Handle, missionKey, key);
        }

        public static bool HaveStoredUnit(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredUnit"), cache.Handle, missionKey, key);
        }

        public static bool HaveStoredString(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredString"), cache.Handle, missionKey, key);
        }


        /// title = "删除缓存 [C]"
        /// description = "删除 ${GameCache}"
        /// comment = "删除并清空该缓存的所有数据.和原版UI完全一致，但却不能兼容原版UI，它的存在完全是在添乱啊"

        public static void FlushGameCache(JGameCache cache)
        {
            War3.CallNative(War3.GetNativeFunction("FlushGameCache"), cache.Handle);
        }


        /// title = "删除类别"
        /// description = "删除缓存 ${GameCache} 中 ${Category} 类别"
        /// comment = "清空该类别下的所有缓存数据."

        public static void FlushStoredMission(JGameCache cache, string missionKey)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredMission"), cache.Handle, missionKey);
        }

        public static void FlushStoredInteger(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredInteger"), cache.Handle, missionKey, key);
        }

        public static void FlushStoredReal(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredReal"), cache.Handle, missionKey, key);
        }

        public static void FlushStoredBoolean(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredBoolean"), cache.Handle, missionKey, key);
        }

        public static void FlushStoredUnit(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredUnit"), cache.Handle, missionKey, key);
        }

        public static void FlushStoredString(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredString"), cache.Handle, missionKey, key);
        }


        /// title = "缓存读取整数 [C]"
        /// description = "从${Game Cache}中读取整数值,类别: ${Category},名称: ${文字}"
        /// comment = "如果该值不存在则返回0."

        public static int GetStoredInteger(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetStoredInteger"), cache.Handle, missionKey, key);
        }


        /// title = "缓存读取实数 [C]"
        /// description = "从 ${Game Cache} 中读取实数,类别: ${Category} 名称: ${文字}"
        /// comment = "如果该值不存在则返回0."

        public static float GetStoredReal(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetStoredReal"), cache.Handle, missionKey, key);
        }


        /// title = "读取布尔值[R]"
        /// description = "从${Game Cache}中读取布尔值,类别: ${Category},名称: ${文字}"
        /// comment = "如果该值不存在则返回false."

        public static bool GetStoredBoolean(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetStoredBoolean"), cache.Handle, missionKey, key);
        }


        /// title = "读取字符串 [C]"
        /// description = "从 ${Game Cache} 中读取字符串,类别: ${Category} 名称: ${文字}"
        /// comment = "如果该值不存在,则返回空字符串. 注意,空字符串不等于null"

        public static string GetStoredString(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetStoredString"), cache.Handle, missionKey, key);
        }

        public static JUnit RestoreUnit(JGameCache cache, string missionKey, string key, JPlayer forWhichPlayer, float x, float y, float facing)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("RestoreUnit"), cache.Handle, missionKey, key, forWhichPlayer.Handle, x, y, facing);
        }


        /// title = "<1.24> 新建哈希表 [C]"
        /// description = "创建一个新的哈希表"
        /// comment = "您可以使用哈希表来储存临时数据"

        public static JHashtable InitHashtable()
        {
            return War3.CallNative<JHashtable>(War3.GetNativeFunction("InitHashtable"));
        }


        /// title = "<1.24> 保存整数 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存整数 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取整数' 可以取出保存的值"

        public static void SaveInteger(JHashtable table, int parentKey, int childKey, int value)
        {
            War3.CallNative(War3.GetNativeFunction("SaveInteger"), table.Handle, parentKey, childKey, value);
        }


        /// title = "<1.24> 保存实数 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存实数 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取实数' 可以取出保存的值"

        public static void SaveReal(JHashtable table, int parentKey, int childKey, float value)
        {
            War3.CallNative(War3.GetNativeFunction("SaveReal"), table.Handle, parentKey, childKey, value);
        }


        /// title = "<1.24> 保存布尔 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存布尔 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取布尔' 可以取出保存的值"

        public static void SaveBoolean(JHashtable table, int parentKey, int childKey, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("SaveBoolean"), table.Handle, parentKey, childKey, value);
        }


        /// title = "<1.24> 保存字符串 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存字符串 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取字符串' 可以取出保存的值"

        public static bool SaveStr(JHashtable table, int parentKey, int childKey, string value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveStr"), table.Handle, parentKey, childKey, value);
        }


        /// title = "<1.24> 保存玩家 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存玩家 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取玩家' 可以取出保存的值"

        public static bool SavePlayerHandle(JHashtable table, int parentKey, int childKey, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SavePlayerHandle"), table.Handle, parentKey, childKey, whichPlayer.Handle);
        }

        public static bool SaveWidgetHandle(JHashtable table, int parentKey, int childKey, JWidget whichWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveWidgetHandle"), table.Handle, parentKey, childKey, whichWidget.Handle);
        }


        /// title = "<1.24> 保存可破坏物 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存可破坏物 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取可破坏物' 可以取出保存的值"

        public static bool SaveDestructableHandle(JHashtable table, int parentKey, int childKey, JDestructable whichDestructable)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveDestructableHandle"), table.Handle, parentKey, childKey, whichDestructable.Handle);
        }


        /// title = "<1.24> 保存物品 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存物品 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取物品' 可以取出保存的值"

        public static bool SaveItemHandle(JHashtable table, int parentKey, int childKey, JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveItemHandle"), table.Handle, parentKey, childKey, whichItem.Handle);
        }


        /// title = "<1.24> 保存单位 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存单位 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取单位' 可以取出保存的值"

        public static bool SaveUnitHandle(JHashtable table, int parentKey, int childKey, JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveUnitHandle"), table.Handle, parentKey, childKey, whichUnit.Handle);
        }

        public static bool SaveAbilityHandle(JHashtable table, int parentKey, int childKey, JAbility whichAbility)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveAbilityHandle"), table.Handle, parentKey, childKey, whichAbility.Handle);
        }


        /// title = "<1.24> 保存计时器 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存计时器 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取计时器' 可以取出保存的值"

        public static bool SaveTimerHandle(JHashtable table, int parentKey, int childKey, JTimer whichTimer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTimerHandle"), table.Handle, parentKey, childKey, whichTimer.Handle);
        }


        /// title = "<1.24> 保存触发器 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存触发器 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取触发器' 可以取出保存的值"

        public static bool SaveTriggerHandle(JHashtable table, int parentKey, int childKey, JTrigger whichTrigger)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTriggerHandle"), table.Handle, parentKey, childKey, whichTrigger.Handle);
        }


        /// title = "<1.24> 保存触发条件 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存触发条件 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取触发条件' 可以取出保存的值"

        public static bool SaveTriggerConditionHandle(JHashtable table, int parentKey, int childKey, JTriggerCondition whichTriggercondition)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTriggerConditionHandle"), table.Handle, parentKey, childKey, whichTriggercondition.Handle);
        }


        /// title = "<1.24> 保存触发动作 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存触发动作 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取触发动作' 可以取出保存的值"

        public static bool SaveTriggerActionHandle(JHashtable table, int parentKey, int childKey, JTriggerAction whichTriggeraction)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTriggerActionHandle"), table.Handle, parentKey, childKey, whichTriggeraction.Handle);
        }


        /// title = "<1.24> 保存触发事件 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存触发事件 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取触发事件' 可以取出保存的值"

        public static bool SaveTriggerEventHandle(JHashtable table, int parentKey, int childKey, JEvent whichEvent)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTriggerEventHandle"), table.Handle, parentKey, childKey, whichEvent.Handle);
        }


        /// title = "<1.24> 保存玩家组 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存玩家组 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取玩家组' 可以取出保存的值"

        public static bool SaveForceHandle(JHashtable table, int parentKey, int childKey, JForce whichForce)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveForceHandle"), table.Handle, parentKey, childKey, whichForce.Handle);
        }


        /// title = "<1.24> 保存单位组 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存单位组 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取单位组' 可以取出保存的值"

        public static bool SaveGroupHandle(JHashtable table, int parentKey, int childKey, JGroup whichGroup)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveGroupHandle"), table.Handle, parentKey, childKey, whichGroup.Handle);
        }


        /// title = "<1.24> 保存点 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存点 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取点' 可以取出保存的值"

        public static bool SaveLocationHandle(JHashtable table, int parentKey, int childKey, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveLocationHandle"), table.Handle, parentKey, childKey, whichLocation.Handle);
        }


        /// title = "<1.24> 保存区域(矩型) [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存区域(矩型) ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取区域(矩型)' 可以取出保存的值"

        public static bool SaveRectHandle(JHashtable table, int parentKey, int childKey, JRect whichRect)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveRectHandle"), table.Handle, parentKey, childKey, whichRect.Handle);
        }


        /// title = "<1.24> 保存布尔表达式 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存布尔表达式 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取布尔表达式' 可以取出保存的值"

        public static bool SaveBooleanExprHandle(JHashtable table, int parentKey, int childKey, JBoolExpr whichBoolexpr)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveBooleanExprHandle"), table.Handle, parentKey, childKey, whichBoolexpr.Handle);
        }


        /// title = "<1.24> 保存音效 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存音效 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取音效' 可以取出保存的值"

        public static bool SaveSoundHandle(JHashtable table, int parentKey, int childKey, JSound whichSound)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveSoundHandle"), table.Handle, parentKey, childKey, whichSound.Handle);
        }


        /// title = "<1.24> 保存特效 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存特效 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取特效' 可以取出保存的值"

        public static bool SaveEffectHandle(JHashtable table, int parentKey, int childKey, JEffect whichEffect)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveEffectHandle"), table.Handle, parentKey, childKey, whichEffect.Handle);
        }


        /// title = "<1.24> 保存单位池 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存单位池 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取单位池' 可以取出保存的值"

        public static bool SaveUnitPoolHandle(JHashtable table, int parentKey, int childKey, JUnitPool whichUnitpool)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveUnitPoolHandle"), table.Handle, parentKey, childKey, whichUnitpool.Handle);
        }


        /// title = "<1.24> 保存物品池 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存物品池 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取物品池' 可以取出保存的值"

        public static bool SaveItemPoolHandle(JHashtable table, int parentKey, int childKey, JItemPool whichItempool)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveItemPoolHandle"), table.Handle, parentKey, childKey, whichItempool.Handle);
        }


        /// title = "<1.24> 保存任务 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存任务 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取任务' 可以取出保存的值"

        public static bool SaveQuestHandle(JHashtable table, int parentKey, int childKey, JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveQuestHandle"), table.Handle, parentKey, childKey, whichQuest.Handle);
        }


        /// title = "<1.24> 保存任务要求 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存任务要求 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取任务要求' 可以取出保存的值"

        public static bool SaveQuestItemHandle(JHashtable table, int parentKey, int childKey, JQuestItem whichQuestitem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveQuestItemHandle"), table.Handle, parentKey, childKey, whichQuestitem.Handle);
        }


        /// title = "<1.24> 保存失败条件 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存失败条件 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取失败条件' 可以取出保存的值"

        public static bool SaveDefeatConditionHandle(JHashtable table, int parentKey, int childKey, JDefeatCondition whichDefeatcondition)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveDefeatConditionHandle"), table.Handle, parentKey, childKey, whichDefeatcondition.Handle);
        }


        /// title = "<1.24> 保存计时器窗口 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存计时器窗口 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取计时器窗口' 可以取出保存的值"

        public static bool SaveTimerDialogHandle(JHashtable table, int parentKey, int childKey, JTimerDialog whichTimerdialog)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTimerDialogHandle"), table.Handle, parentKey, childKey, whichTimerdialog.Handle);
        }


        /// title = "<1.24> 保存排行榜 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存排行榜 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取排行榜' 可以取出保存的值"

        public static bool SaveLeaderboardHandle(JHashtable table, int parentKey, int childKey, JLeaderboard whichLeaderboard)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveLeaderboardHandle"), table.Handle, parentKey, childKey, whichLeaderboard.Handle);
        }


        /// title = "<1.24> 保存多面板 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存多面板 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取多面板' 可以取出保存的值"

        public static bool SaveMultiboardHandle(JHashtable table, int parentKey, int childKey, JMultiboard whichMultiboard)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveMultiboardHandle"), table.Handle, parentKey, childKey, whichMultiboard.Handle);
        }


        /// title = "<1.24> 保存多面板项目 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存多面板项目 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取多面板项目' 可以取出保存的值"

        public static bool SaveMultiboardItemHandle(JHashtable table, int parentKey, int childKey, JMultiboardItem whichMultiboarditem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveMultiboardItemHandle"), table.Handle, parentKey, childKey, whichMultiboarditem.Handle);
        }


        /// title = "<1.24> 保存可追踪物 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存可追踪物 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取可追踪物' 可以取出保存的值"

        public static bool SaveTrackableHandle(JHashtable table, int parentKey, int childKey, JTrackable whichTrackable)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTrackableHandle"), table.Handle, parentKey, childKey, whichTrackable.Handle);
        }


        /// title = "<1.24> 保存对话框 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存对话框 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取对话框' 可以取出保存的值"

        public static bool SaveDialogHandle(JHashtable table, int parentKey, int childKey, JDialog whichDialog)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveDialogHandle"), table.Handle, parentKey, childKey, whichDialog.Handle);
        }


        /// title = "<1.24> 保存对话框按钮 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存对话框按钮 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取对话框按钮' 可以取出保存的值"

        public static bool SaveButtonHandle(JHashtable table, int parentKey, int childKey, JButton whichButton)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveButtonHandle"), table.Handle, parentKey, childKey, whichButton.Handle);
        }


        /// title = "<1.24> 保存漂浮文字 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存漂浮文字 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取漂浮文字' 可以取出保存的值"

        public static bool SaveTextTagHandle(JHashtable table, int parentKey, int childKey, JTextTag whichTexttag)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTextTagHandle"), table.Handle, parentKey, childKey, whichTexttag.Handle);
        }


        /// title = "<1.24> 保存闪电效果 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存闪电效果 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取闪电效果' 可以取出保存的值"

        public static bool SaveLightningHandle(JHashtable table, int parentKey, int childKey, JLightning whichLightning)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveLightningHandle"), table.Handle, parentKey, childKey, whichLightning.Handle);
        }


        /// title = "<1.24> 保存图像 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存图像 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取图像' 可以取出保存的值"

        public static bool SaveImageHandle(JHashtable table, int parentKey, int childKey, JImage whichImage)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveImageHandle"), table.Handle, parentKey, childKey, whichImage.Handle);
        }


        /// title = "<1.24> 保存地面纹理变化 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存地面纹理变化 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取地面纹理变化' 可以取出保存的值"

        public static bool SaveUbersplatHandle(JHashtable table, int parentKey, int childKey, JUbersplat whichUbersplat)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveUbersplatHandle"), table.Handle, parentKey, childKey, whichUbersplat.Handle);
        }


        /// title = "<1.24> 保存区域(不规则) [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存区域(不规则) ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取区域(不规则)' 可以取出保存的值"

        public static bool SaveRegionHandle(JHashtable table, int parentKey, int childKey, JRegion whichRegion)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveRegionHandle"), table.Handle, parentKey, childKey, whichRegion.Handle);
        }


        /// title = "<1.24> 保存迷雾状态 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存迷雾状态 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取迷雾状态' 可以取出保存的值"

        public static bool SaveFogStateHandle(JHashtable table, int parentKey, int childKey, JFogState whichFogState)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveFogStateHandle"), table.Handle, parentKey, childKey, whichFogState.Handle);
        }


        /// title = "<1.24> 保存可见度修正器 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存可见度修正器 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取可见度修正器' 可以取出保存的值"

        public static bool SaveFogModifierHandle(JHashtable table, int parentKey, int childKey, JFogModifier whichFogModifier)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveFogModifierHandle"), table.Handle, parentKey, childKey, whichFogModifier.Handle);
        }


        /// title = "<1.24> 保存实体对象 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存对象 ${Value}"
        /// comment = "实体对象即一切需要计算变量连接数的对象"

        public static bool SaveAgentHandle(JHashtable table, int parentKey, int childKey, JAgent whichAgent)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveAgentHandle"), table.Handle, parentKey, childKey, whichAgent.Handle);
        }


        /// title = "<1.24> 保存哈希表 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存哈希表 ${Value}"
        /// comment = "使用 '哈希表 - 从哈希表提取哈希表' 可以取出保存的值"

        public static bool SaveHashtableHandle(JHashtable table, int parentKey, int childKey, JHashtable whichHashtable)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveHashtableHandle"), table.Handle, parentKey, childKey, whichHashtable.Handle);
        }


        /// title = "<1.24> 从哈希表提取整数 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取整数"
        /// comment = "如果不存在则返回0"

        public static int LoadInteger(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("LoadInteger"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取实数 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取实数"
        /// comment = "如果不存在则返回0.00"

        public static float LoadReal(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("LoadReal"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取布尔 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取布尔"
        /// comment = "如果不存在则返回False"

        public static bool LoadBoolean(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("LoadBoolean"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取字符串 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取字符串"
        /// comment = "如果不存在则返回空"

        public static string LoadStr(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("LoadStr"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取玩家 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取玩家"
        /// comment = "如果不存在则返回空"

        public static JPlayer LoadPlayerHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("LoadPlayerHandle"), table.Handle, parentKey, childKey);
        }

        public static JWidget LoadWidgetHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JWidget>(War3.GetNativeFunction("LoadWidgetHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取可破坏物 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取可破坏物"
        /// comment = "如果不存在则返回空"

        public static JDestructable LoadDestructableHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("LoadDestructableHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取物品 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取物品"
        /// comment = "如果不存在则返回空"

        public static JItem LoadItemHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("LoadItemHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取单位 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取单位"
        /// comment = "如果不存在则返回空"

        public static JUnit LoadUnitHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("LoadUnitHandle"), table.Handle, parentKey, childKey);
        }

        public static JAbility LoadAbilityHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JAbility>(War3.GetNativeFunction("LoadAbilityHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取计时器 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取计时器"
        /// comment = "如果不存在则返回空"

        public static JTimer LoadTimerHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTimer>(War3.GetNativeFunction("LoadTimerHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取触发器 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取触发器"
        /// comment = "如果不存在则返回空"

        public static JTrigger LoadTriggerHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTrigger>(War3.GetNativeFunction("LoadTriggerHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取触发条件 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取触发条件"
        /// comment = "如果不存在则返回空"

        public static JTriggerCondition LoadTriggerConditionHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTriggerCondition>(War3.GetNativeFunction("LoadTriggerConditionHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取触发动作 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取触发动作"
        /// comment = "如果不存在则返回空"

        public static JTriggerAction LoadTriggerActionHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTriggerAction>(War3.GetNativeFunction("LoadTriggerActionHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取触发事件 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取触发事件"
        /// comment = "如果不存在则返回空"

        public static JEvent LoadTriggerEventHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("LoadTriggerEventHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取玩家组 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取玩家组"
        /// comment = "如果不存在则返回空"

        public static JForce LoadForceHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JForce>(War3.GetNativeFunction("LoadForceHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取单位组 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取单位组"
        /// comment = "如果不存在则返回空"

        public static JGroup LoadGroupHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JGroup>(War3.GetNativeFunction("LoadGroupHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取点 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取点"
        /// comment = "如果不存在则返回空"

        public static JLocation LoadLocationHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("LoadLocationHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取区域(矩型) [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取区域(矩型)"
        /// comment = "如果不存在则返回空"

        public static JRect LoadRectHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JRect>(War3.GetNativeFunction("LoadRectHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取布尔表达式 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取布尔表达式"
        /// comment = "如果不存在则返回空"

        public static JBoolExpr LoadBooleanExprHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JBoolExpr>(War3.GetNativeFunction("LoadBooleanExprHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取音效 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取音效"
        /// comment = "如果不存在则返回空"

        public static JSound LoadSoundHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("LoadSoundHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取特效 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取特效"
        /// comment = "如果不存在则返回空"

        public static JEffect LoadEffectHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("LoadEffectHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取单位池 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取单位池"
        /// comment = "如果不存在则返回空"

        public static JUnitPool LoadUnitPoolHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JUnitPool>(War3.GetNativeFunction("LoadUnitPoolHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取物品池 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取物品池"
        /// comment = "如果不存在则返回空"

        public static JItemPool LoadItemPoolHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JItemPool>(War3.GetNativeFunction("LoadItemPoolHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取任务 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取任务"
        /// comment = "如果不存在则返回空"

        public static JQuest LoadQuestHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JQuest>(War3.GetNativeFunction("LoadQuestHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取任务要求 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取任务要求"
        /// comment = "如果不存在则返回空"

        public static JQuestItem LoadQuestItemHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JQuestItem>(War3.GetNativeFunction("LoadQuestItemHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取失败条件 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取失败条件"
        /// comment = "如果不存在则返回空"

        public static JDefeatCondition LoadDefeatConditionHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JDefeatCondition>(War3.GetNativeFunction("LoadDefeatConditionHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取计时器窗口 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取计时器窗口"
        /// comment = "如果不存在则返回空"

        public static JTimerDialog LoadTimerDialogHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTimerDialog>(War3.GetNativeFunction("LoadTimerDialogHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取排行榜 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取排行榜"
        /// comment = "如果不存在则返回空"

        public static JLeaderboard LoadLeaderboardHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JLeaderboard>(War3.GetNativeFunction("LoadLeaderboardHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取多面板 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取多面板"
        /// comment = "如果不存在则返回空"

        public static JMultiboard LoadMultiboardHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JMultiboard>(War3.GetNativeFunction("LoadMultiboardHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取多面板项目 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取多面板项目"
        /// comment = "如果不存在则返回空"

        public static JMultiboardItem LoadMultiboardItemHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JMultiboardItem>(War3.GetNativeFunction("LoadMultiboardItemHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取可追踪物 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取可追踪物"
        /// comment = "如果不存在则返回空"

        public static JTrackable LoadTrackableHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTrackable>(War3.GetNativeFunction("LoadTrackableHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取对话框 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取对话框"
        /// comment = "如果不存在则返回空"

        public static JDialog LoadDialogHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JDialog>(War3.GetNativeFunction("LoadDialogHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取对话框按钮 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取对话框按钮"
        /// comment = "如果不存在则返回空"

        public static JButton LoadButtonHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JButton>(War3.GetNativeFunction("LoadButtonHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取漂浮文字 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取漂浮文字"
        /// comment = "如果不存在则返回空"

        public static JTextTag LoadTextTagHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTextTag>(War3.GetNativeFunction("LoadTextTagHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取闪电效果 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取闪电效果"
        /// comment = "如果不存在则返回空"

        public static JLightning LoadLightningHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JLightning>(War3.GetNativeFunction("LoadLightningHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取图象 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取图象"
        /// comment = "如果不存在则返回空"

        public static JImage LoadImageHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JImage>(War3.GetNativeFunction("LoadImageHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取地面纹理变化 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取地面纹理变化"
        /// comment = "如果不存在则返回空"

        public static JUbersplat LoadUbersplatHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JUbersplat>(War3.GetNativeFunction("LoadUbersplatHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取区域(不规则) [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取区域(不规则)"
        /// comment = "如果不存在则返回空"

        public static JRegion LoadRegionHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JRegion>(War3.GetNativeFunction("LoadRegionHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取迷雾状态 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取迷雾状态"
        /// comment = "如果不存在则返回空"

        public static JFogState LoadFogStateHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JFogState>(War3.GetNativeFunction("LoadFogStateHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取可见度修正器 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取可见度修正器"
        /// comment = "如果不存在则返回空"

        public static JFogModifier LoadFogModifierHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JFogModifier>(War3.GetNativeFunction("LoadFogModifierHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 从哈希表提取哈希表 [C]"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取哈希表"
        /// comment = "如果不存在则返回空"

        public static JHashtable LoadHashtableHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JHashtable>(War3.GetNativeFunction("LoadHashtableHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 哈希项存有整数值 <new>"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有整数值"
        /// comment = ""

        public static bool HaveSavedInteger(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedInteger"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 哈希项存有实数值 <new>"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有实数值"
        /// comment = ""

        public static bool HaveSavedReal(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedReal"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 哈希项存有布尔值 <new>"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有布尔值"
        /// comment = ""

        public static bool HaveSavedBoolean(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedBoolean"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 哈希项存有字符串 <new>"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有字符串"
        /// comment = ""

        public static bool HaveSavedString(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedString"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 哈希项存有句柄 <new>"
        /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有句柄"
        /// comment = ""

        public static bool HaveSavedHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "清理哈希项存储的整数值 <new>"
        /// description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的整数值"
        /// comment = "清空哈希表主索引下的整数值"

        public static void RemoveSavedInteger(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedInteger"), table.Handle, parentKey, childKey);
        }


        /// title = "清理哈希项存储的实数值 <new>"
        /// description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的实数值"
        /// comment = "清空哈希表主索引下的实数值"

        public static void RemoveSavedReal(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedReal"), table.Handle, parentKey, childKey);
        }


        /// title = "清理哈希项存储的布尔值 <new>"
        /// description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的布尔值"
        /// comment = "清空哈希表主索引下的布尔值"

        public static void RemoveSavedBoolean(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedBoolean"), table.Handle, parentKey, childKey);
        }


        /// title = "清理哈希项存储的字符串 <new>"
        /// description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的字符串"
        /// comment = "清空哈希表主索引下的字符串"

        public static void RemoveSavedString(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedString"), table.Handle, parentKey, childKey);
        }


        /// title = "清理哈希项存储的句柄 <new>"
        /// description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的句柄"
        /// comment = "清空哈希表主索引下的句柄"

        public static void RemoveSavedHandle(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedHandle"), table.Handle, parentKey, childKey);
        }


        /// title = "<1.24> 清空哈希表 [C]"
        /// description = "清空 ${Hashtable}"
        /// comment = "清空哈希表所有数据"

        public static void FlushParentHashtable(JHashtable table)
        {
            War3.CallNative(War3.GetNativeFunction("FlushParentHashtable"), table.Handle);
        }


        /// title = "<1.24> 清空哈希表主索引 [C]"
        /// description = "清空 ${Hashtable} 中位于主索引 ${Value}  之内的所有数据"
        /// comment = "清空哈希表主索引下的所有数据"

        public static void FlushChildHashtable(JHashtable table, int parentKey)
        {
            War3.CallNative(War3.GetNativeFunction("FlushChildHashtable"), table.Handle, parentKey);
        }


        /// title = "随机整数"
        /// description = "随机整数,最小值: ${Minimum} 最大值: ${Maximum}"
        /// comment = ""

        public static int GetRandomInt(int lowBound, int highBound)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetRandomInt"), lowBound, highBound);
        }


        /// title = "随机实数"
        /// description = "随机实数,最小值: ${Minimum} 最大值: ${Maximum}"
        /// comment = ""

        public static float GetRandomReal(float lowBound, float highBound)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRandomReal"), lowBound, highBound);
        }


        /// title = "新建单位池 [R]"
        /// description = "新建的空单位池"
        /// comment = "会创建单位池。"

        public static JUnitPool CreateUnitPool()
        {
            return War3.CallNative<JUnitPool>(War3.GetNativeFunction("CreateUnitPool"));
        }


        /// title = "删除单位池 [R]"
        /// description = "删除 ${单位池}"
        /// comment = ""

        public static void DestroyUnitPool(JUnitPool whichPool)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyUnitPool"), whichPool.Handle);
        }


        /// title = "添加单位类型 [R]"
        /// description = "在 ${单位池} 中添加一个 ${单位} 比重为 ${数值}"
        /// comment = "比重越高被选择的机率越大"

        public static void UnitPoolAddUnitType(JUnitPool whichPool, int unitId, float weight)
        {
            War3.CallNative(War3.GetNativeFunction("UnitPoolAddUnitType"), whichPool.Handle, unitId, weight);
        }


        /// title = "删除单位类型 [R]"
        /// description = "从 ${单位池} 中删除 ${单位}"
        /// comment = ""

        public static void UnitPoolRemoveUnitType(JUnitPool whichPool, int unitId)
        {
            War3.CallNative(War3.GetNativeFunction("UnitPoolRemoveUnitType"), whichPool.Handle, unitId);
        }


        /// title = "选择放置单位 [R]"
        /// description = "从 ${单位池} 中为 ${玩家} 任意选择一个单位并放置到点( ${X} , ${Y} ) 面向 ${度}"
        /// comment = "从单位池中随机选取一个单位类型."

        public static JUnit PlaceRandomUnit(JUnitPool whichPool, JPlayer forWhichPlayer, float x, float y, float facing)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("PlaceRandomUnit"), whichPool.Handle, forWhichPlayer.Handle, x, y, facing);
        }


        /// title = "新建物品池 [R]"
        /// description = "新建的空物品池"
        /// comment = "会创建物品池."

        public static JItemPool CreateItemPool()
        {
            return War3.CallNative<JItemPool>(War3.GetNativeFunction("CreateItemPool"));
        }


        /// title = "删除物品池 [R]"
        /// description = "删除 ${物品池}"
        /// comment = ""

        public static void DestroyItemPool(JItemPool whichItemPool)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyItemPool"), whichItemPool.Handle);
        }


        /// title = "添加物品类型 [R]"
        /// description = "在 ${物品池} 中添加一个 ${物品} 比重为 ${数值}"
        /// comment = "比重越高被选择的机率越大."

        public static void ItemPoolAddItemType(JItemPool whichItemPool, int itemId, float weight)
        {
            War3.CallNative(War3.GetNativeFunction("ItemPoolAddItemType"), whichItemPool.Handle, itemId, weight);
        }


        /// title = "删除物品类型 [R]"
        /// description = "从 ${物品池} 中删除 ${物品}"
        /// comment = ""

        public static void ItemPoolRemoveItemType(JItemPool whichItemPool, int itemId)
        {
            War3.CallNative(War3.GetNativeFunction("ItemPoolRemoveItemType"), whichItemPool.Handle, itemId);
        }


        /// title = "选择放置物品 [R]"
        /// description = "从 ${物品池} 中任意选择一个物品并放置到( ${X} , ${Y} )点"
        /// comment = ""

        public static JItem PlaceRandomItem(JItemPool whichItemPool, float x, float y)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("PlaceRandomItem"), whichItemPool.Handle, x, y);
        }

        public static int ChooseRandomCreep(int level)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("ChooseRandomCreep"), level);
        }

        public static int ChooseRandomNPBuilding()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("ChooseRandomNPBuilding"));
        }

        public static int ChooseRandomItem(int level)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("ChooseRandomItem"), level);
        }

        public static int ChooseRandomItemEx(JItemType whichType, int level)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("ChooseRandomItemEx"), whichType.Handle, level);
        }


        /// title = "设置随机种子"
        /// description = "设置随机种子数为：${整数}"
        /// comment = "设置游戏的随机种子，随机种子会影响随机整数，攻击骰子之类的随机数。"

        public static void SetRandomSeed(int seed)
        {
            War3.CallNative(War3.GetNativeFunction("SetRandomSeed"), seed);
        }

        public static void SetTerrainFog(float a, float b, float c, float d, float e)
        {
            War3.CallNative(War3.GetNativeFunction("SetTerrainFog"), a, b, c, d, e);
        }

        public static void ResetTerrainFog()
        {
            War3.CallNative(War3.GetNativeFunction("ResetTerrainFog"));
        }

        public static void SetUnitFog(float a, float b, float c, float d, float e)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitFog"), a, b, c, d, e);
        }


        /// title = "设置迷雾 [R]"
        /// description = "迷雾风格: ${Style}, Z轴开始端: ${Z-Start}, Z轴结束端: ${Z-End}, 密度: ${Density} 颜色:(${Red},${Green},${Blue})"
        /// comment = "颜色格式为(红,绿,蓝). 取值范围0.00-1.00."

        public static void SetTerrainFogEx(int style, float zstart, float zend, float density, float red, float green, float blue)
        {
            War3.CallNative(War3.GetNativeFunction("SetTerrainFogEx"), style, zstart, zend, density, red, green, blue);
        }


        /// title = "对玩家显示文本消息(自动限时) [R]"
        /// description = "对 ${玩家} 在屏幕位移(${X},${Y})处显示文本: ${文字}"
        /// comment = "显示时间取决于文字长度. 位移的取值在0-1之间. 可使用'本地玩家'实现对所有玩家发送消息."

        public static void DisplayTextToPlayer(JPlayer toPlayer, float x, float y, string message)
        {
            War3.CallNative(War3.GetNativeFunction("DisplayTextToPlayer"), toPlayer.Handle, x, y, message);
        }


        /// title = "对玩家显示文本消息(指定时间) [R]"
        /// description = "对 ${玩家} 在屏幕位移(${X},${Y})处显示 ${时间} 秒的文本信息: ${文字}"
        /// comment = "位移的取值在0-1之间. 可使用'本地玩家[R]'实现对所有玩家发送消息."

        public static void DisplayTimedTextToPlayer(JPlayer toPlayer, float x, float y, float duration, string message)
        {
            War3.CallNative(War3.GetNativeFunction("DisplayTimedTextToPlayer"), toPlayer.Handle, x, y, duration, message);
        }

        public static void DisplayTimedTextFromPlayer(JPlayer toPlayer, float x, float y, float duration, string message)
        {
            War3.CallNative(War3.GetNativeFunction("DisplayTimedTextFromPlayer"), toPlayer.Handle, x, y, duration, message);
        }


        /// title = "清空文本信息(所有玩家) [R]"
        /// description = "清空玩家屏幕上的文本信息"
        /// comment = ""

        public static void ClearTextMessages()
        {
            War3.CallNative(War3.GetNativeFunction("ClearTextMessages"));
        }

        public static void SetDayNightModels(string terrainDNCFile, string unitDNCFile)
        {
            War3.CallNative(War3.GetNativeFunction("SetDayNightModels"), terrainDNCFile, unitDNCFile);
        }


        /// title = "设置天空"
        /// description = "设置天空模型为 ${Sky}"
        /// comment = ""

        public static void SetSkyModel(string skyModelFile)
        {
            War3.CallNative(War3.GetNativeFunction("SetSkyModel"), skyModelFile);
        }


        /// title = "启用/禁用玩家控制权(所有玩家) [R]"
        /// description = "${启用/禁用} 玩家控制权"
        /// comment = ""

        public static void EnableUserControl(bool b)
        {
            War3.CallNative(War3.GetNativeFunction("EnableUserControl"), b);
        }

        public static void EnableUserUI(bool b)
        {
            War3.CallNative(War3.GetNativeFunction("EnableUserUI"), b);
        }

        public static void SuspendTimeOfDay(bool b)
        {
            War3.CallNative(War3.GetNativeFunction("SuspendTimeOfDay"), b);
        }


        /// title = "设置昼夜时间流逝速度 [R]"
        /// description = "设置昼夜时间流逝速度为默认值的 ${Value}倍"
        /// comment = "设置100%来恢复正常值. 该值并不影响游戏速度."

        public static void SetTimeOfDayScale(float r)
        {
            War3.CallNative(War3.GetNativeFunction("SetTimeOfDayScale"), r);
        }

        public static float GetTimeOfDayScale()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetTimeOfDayScale"));
        }


        /// title = "开启/关闭 信箱模式(所有玩家) [R]"
        /// description = "${开启/关闭} 信箱模式,转换时间为 ${Duration} 秒"
        /// comment = "使用电影镜头模式,隐藏游戏界面."

        public static void ShowInterface(bool flag, float fadeDuration)
        {
            War3.CallNative(War3.GetNativeFunction("ShowInterface"), flag, fadeDuration);
        }


        /// title = "暂停/恢复游戏 [R]"
        /// description = "${暂停/恢复} 游戏"
        /// comment = ""

        public static void PauseGame(bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("PauseGame"), flag);
        }


        /// title = "闪动指示器(对单位) [R]"
        /// description = "对 ${单位} 闪动指示器,使用颜色:(${Red}%, ${Green}%, ${Blue}%) Alpha通道值: ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和Alpha通道值取值范围为0-255."

        public static void UnitAddIndicator(JUnit whichUnit, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("UnitAddIndicator"), whichUnit.Handle, red, green, blue, alpha);
        }

        public static void AddIndicator(JWidget whichWidget, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("AddIndicator"), whichWidget.Handle, red, green, blue, alpha);
        }


        /// title = "小地图信号(所有玩家) [R]"
        /// description = "对所有玩家发送小地图信号到坐标(${X},${Y}) 持续时间: ${Duration} 秒"
        /// comment = ""

        public static void PingMinimap(float x, float y, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("PingMinimap"), x, y, duration);
        }


        /// title = "小地图信号(指定颜色)(所有玩家) [R]"
        /// description = "对所有玩家发送小地图信号到坐标(${X},${Y}) 持续时间: ${Duration} 秒, 信号颜色:(${Red},${Green},${Blue}) 信号类型: ${Style}"
        /// comment = "颜色格式为(红,绿,蓝). 颜色值取值范围为0-255."

        public static void PingMinimapEx(float x, float y, float duration, int red, int green, int blue, bool extraEffects)
        {
            War3.CallNative(War3.GetNativeFunction("PingMinimapEx"), x, y, duration, red, green, blue, extraEffects);
        }


        /// title = "允许/禁止闭塞(所有玩家) [R]"
        /// description = "${Enable/Disable} 闭塞"
        /// comment = ""

        public static void EnableOcclusion(bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("EnableOcclusion"), flag);
        }

        public static void SetIntroShotText(string introText)
        {
            War3.CallNative(War3.GetNativeFunction("SetIntroShotText"), introText);
        }

        public static void SetIntroShotModel(string introModelPath)
        {
            War3.CallNative(War3.GetNativeFunction("SetIntroShotModel"), introModelPath);
        }


        /// title = "允许/禁止 边界染色(所有玩家) [R]"
        /// description = "${Enable/Disable} 边界染色,应用于所有玩家"
        /// comment = "禁用边界染色时边界为普通地形,不显示为黑色,但仍是不可通行的."

        public static void EnableWorldFogBoundary(bool b)
        {
            War3.CallNative(War3.GetNativeFunction("EnableWorldFogBoundary"), b);
        }

        public static void PlayModelCinematic(string modelName)
        {
            War3.CallNative(War3.GetNativeFunction("PlayModelCinematic"), modelName);
        }

        public static void PlayCinematic(string movieName)
        {
            War3.CallNative(War3.GetNativeFunction("PlayCinematic"), movieName);
        }

        public static void ForceUIKey(string key)
        {
            War3.CallNative(War3.GetNativeFunction("ForceUIKey"), key);
        }

        public static void ForceUICancel()
        {
            War3.CallNative(War3.GetNativeFunction("ForceUICancel"));
        }

        public static void DisplayLoadDialog()
        {
            War3.CallNative(War3.GetNativeFunction("DisplayLoadDialog"));
        }


        /// title = "设置小地图特殊标志"
        /// description = "设置小地图特殊标志为 ${Image}"
        /// comment = "必须使用16x16的图像."

        public static void SetAltMinimapIcon(string iconPath)
        {
            War3.CallNative(War3.GetNativeFunction("SetAltMinimapIcon"), iconPath);
        }


        /// title = "禁用 重新开始任务按钮"
        /// description = "设置 重新开始任务按钮可以点击为 ${}"
        /// comment = "当单人游戏时，可以设置重新开始任务按钮能否允许点击。"

        public static void DisableRestartMission(bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("DisableRestartMission"), flag);
        }


        /// title = "新建漂浮文字 [R]"
        /// description = "新建的空漂浮文字"
        /// comment = "会创建漂浮文字."

        public static JTextTag CreateTextTag()
        {
            return War3.CallNative<JTextTag>(War3.GetNativeFunction("CreateTextTag"));
        }

        public static void DestroyTextTag(JTextTag t)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyTextTag"), t.Handle);
        }


        /// title = "改变文字内容 [R]"
        /// description = "改变 ${Floating Text} 的内容为 ${文字} ,字体大小: ${Size}"
        /// comment = "采用原始字体大小单位. 字体大小不能超过0.5."

        public static void SetTextTagText(JTextTag t, string s, float height)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagText"), t.Handle, s, height);
        }


        /// title = "改变位置(坐标) [R]"
        /// description = "改变 ${Floating Text} 的位置为(${X},${Y}) ,Z轴高度为 ${Z}"
        /// comment = ""

        public static void SetTextTagPos(JTextTag t, float x, float y, float heightOffset)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagPos"), t.Handle, x, y, heightOffset);
        }

        public static void SetTextTagPosUnit(JTextTag t, JUnit whichUnit, float heightOffset)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagPosUnit"), t.Handle, whichUnit.Handle, heightOffset);
        }


        /// title = "改变颜色 [R]"
        /// description = "改变 ${Floating Text} 的颜色为(${Red},${Green},${Blue}) 透明值为 ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). 透明值0为不可见. 颜色值和透明值取值范围为0-255."

        public static void SetTextTagColor(JTextTag t, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagColor"), t.Handle, red, green, blue, alpha);
        }


        /// title = "设置速率 [R]"
        /// description = "设置 ${Floating Text} 的X轴速率: ${XSpeed} ,Y轴速率: ${YSpeed}"
        /// comment = "对移动后的漂浮文字设置速率,该漂浮文字会先回到原点再向设定的角度移动. 这里的1约等于游戏中的1800速度."

        public static void SetTextTagVelocity(JTextTag t, float xvel, float yvel)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagVelocity"), t.Handle, xvel, yvel);
        }


        /// title = "显示/隐藏 (所有玩家) [R]"
        /// description = "对所有玩家 ${Show/Hide} ${Floating Text}"
        /// comment = ""

        public static void SetTextTagVisibility(JTextTag t, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagVisibility"), t.Handle, flag);
        }

        public static void SetTextTagSuspended(JTextTag t, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagSuspended"), t.Handle, flag);
        }

        public static void SetTextTagPermanent(JTextTag t, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagPermanent"), t.Handle, flag);
        }

        public static void SetTextTagAge(JTextTag t, float age)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagAge"), t.Handle, age);
        }

        public static void SetTextTagLifespan(JTextTag t, float lifespan)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagLifespan"), t.Handle, lifespan);
        }

        public static void SetTextTagFadepoint(JTextTag t, float fadepoint)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagFadepoint"), t.Handle, fadepoint);
        }


        /// title = "保留英雄图标"
        /// description = "为玩家保留 ${Number} 个左上角英雄图标."
        /// comment = "因为共享单位而被控制的其他玩家英雄的图标将在保留位置之后开始显示."

        public static void SetReservedLocalHeroButtons(int reserved)
        {
            War3.CallNative(War3.GetNativeFunction("SetReservedLocalHeroButtons"), reserved);
        }


        /// title = "联盟颜色显示设置"
        /// description = "联盟颜色显示设置"
        /// comment = "0为不开启. 1为小地图显示. 2为小地图和游戏都显示."

        public static int GetAllyColorFilterState()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetAllyColorFilterState"));
        }


        /// title = "设置联盟颜色显示"
        /// description = "设置联盟颜色显示状态为 ${State}"
        /// comment = "0为不开启. 1为小地图显示. 2为小地图和游戏都显示. 相当于游戏中Alt+A功能."

        public static void SetAllyColorFilterState(int state)
        {
            War3.CallNative(War3.GetNativeFunction("SetAllyColorFilterState"), state);
        }


        /// title = "小地图中立生物显示开启"
        /// description = "小地图中立生物显示开启"
        /// comment = ""

        public static bool GetCreepCampFilterState()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetCreepCampFilterState"));
        }


        /// title = "设置小地图中立生物显示"
        /// description = "小地图 ${Show/Hide} 中立生物"
        /// comment = "相当于游戏中Alt+R功能."

        public static void SetCreepCampFilterState(bool state)
        {
            War3.CallNative(War3.GetNativeFunction("SetCreepCampFilterState"), state);
        }


        /// title = "允许/禁用小地图按钮"
        /// description = "${Enable/Disable} 联盟颜色显示按钮, ${Enable/Disable} 中立生物显示按钮"
        /// comment = ""

        public static void EnableMinimapFilterButtons(bool enableAlly, bool enableCreep)
        {
            War3.CallNative(War3.GetNativeFunction("EnableMinimapFilterButtons"), enableAlly, enableCreep);
        }


        /// title = "允许/禁用框选"
        /// description = "${Enable/Disable} 框选功能 (${Enable/Disable} 显示选择框)"
        /// comment = ""

        public static void EnableDragSelect(bool state, bool ui)
        {
            War3.CallNative(War3.GetNativeFunction("EnableDragSelect"), state, ui);
        }


        /// title = "允许/禁用预选"
        /// description = "${Enable/Disable} 预选功能 (${Enable/Disable} 显示预选圈,生命槽,物体信息)"
        /// comment = ""

        public static void EnablePreSelect(bool state, bool ui)
        {
            War3.CallNative(War3.GetNativeFunction("EnablePreSelect"), state, ui);
        }


        /// title = "允许/禁用选择"
        /// description = "${Enable/Disable} 选择和取消选择功能 (${Enable/Disable} 显示选择圈)"
        /// comment = "禁用选择后仍可以通过触发来选择物体. 只有允许选择功能时才会显示选择圈."

        public static void EnableSelect(bool state, bool ui)
        {
            War3.CallNative(War3.GetNativeFunction("EnableSelect"), state, ui);
        }


        /// title = "新建可追踪物 [R]"
        /// description = "新建的可追踪物, 使用模型: ${模型名字} 所在位置: ( ${X轴} , ${Y轴} ) 面向角度: ${数值} 度"
        /// comment = "可用来响应鼠标的移动和点击. 会创建可追踪物."

        public static JTrackable CreateTrackable(string trackableModelPath, float x, float y, float facing)
        {
            return War3.CallNative<JTrackable>(War3.GetNativeFunction("CreateTrackable"), trackableModelPath, x, y, facing);
        }


        /// title = "新建任务 [R]"
        /// description = "新建任务 "
        /// comment = "新建一个任务.注：这条毫无用处，别用——everguo"

        public static JQuest CreateQuest()
        {
            return War3.CallNative<JQuest>(War3.GetNativeFunction("CreateQuest"));
        }

        public static void DestroyQuest(JQuest whichQuest)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyQuest"), whichQuest.Handle);
        }

        public static void QuestSetTitle(JQuest whichQuest, string title)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetTitle"), whichQuest.Handle, title);
        }

        public static void QuestSetDescription(JQuest whichQuest, string description)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetDescription"), whichQuest.Handle, description);
        }

        public static void QuestSetIconPath(JQuest whichQuest, string iconPath)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetIconPath"), whichQuest.Handle, iconPath);
        }

        public static void QuestSetRequired(JQuest whichQuest, bool required)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetRequired"), whichQuest.Handle, required);
        }

        public static void QuestSetCompleted(JQuest whichQuest, bool completed)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetCompleted"), whichQuest.Handle, completed);
        }

        public static void QuestSetDiscovered(JQuest whichQuest, bool discovered)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetDiscovered"), whichQuest.Handle, discovered);
        }

        public static void QuestSetFailed(JQuest whichQuest, bool failed)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetFailed"), whichQuest.Handle, failed);
        }


        /// title = "启用/禁用 任务 [R]"
        /// description = "设置 ${Quest} ${Enable/Disable}"
        /// comment = "被禁用的任务将不会显示在任务列表."

        public static void QuestSetEnabled(JQuest whichQuest, bool enabled)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetEnabled"), whichQuest.Handle, enabled);
        }


        /// title = "是主要任务"
        /// description = "${Quest} 是主要任务"
        /// comment = ""

        public static bool IsQuestRequired(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestRequired"), whichQuest.Handle);
        }


        /// title = "任务完成"
        /// description = "${Quest} 已完成"
        /// comment = ""

        public static bool IsQuestCompleted(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestCompleted"), whichQuest.Handle);
        }


        /// title = "任务被发现"
        /// description = "${Quest} 已被发现"
        /// comment = ""

        public static bool IsQuestDiscovered(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestDiscovered"), whichQuest.Handle);
        }


        /// title = "任务失败"
        /// description = "${Quest} 失败"
        /// comment = ""

        public static bool IsQuestFailed(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestFailed"), whichQuest.Handle);
        }


        /// title = "任务激活"
        /// description = "${Quest} 已激活"
        /// comment = ""

        public static bool IsQuestEnabled(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestEnabled"), whichQuest.Handle);
        }

        public static JQuestItem QuestCreateItem(JQuest whichQuest)
        {
            return War3.CallNative<JQuestItem>(War3.GetNativeFunction("QuestCreateItem"), whichQuest.Handle);
        }

        public static void QuestItemSetDescription(JQuestItem whichQuestItem, string description)
        {
            War3.CallNative(War3.GetNativeFunction("QuestItemSetDescription"), whichQuestItem.Handle, description);
        }

        public static void QuestItemSetCompleted(JQuestItem whichQuestItem, bool completed)
        {
            War3.CallNative(War3.GetNativeFunction("QuestItemSetCompleted"), whichQuestItem.Handle, completed);
        }


        /// title = "任务项目完成"
        /// description = "${Quest Requirement} 已完成"
        /// comment = ""

        public static bool IsQuestItemCompleted(JQuestItem whichQuestItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestItemCompleted"), whichQuestItem.Handle);
        }

        public static JDefeatCondition CreateDefeatCondition()
        {
            return War3.CallNative<JDefeatCondition>(War3.GetNativeFunction("CreateDefeatCondition"));
        }

        public static void DestroyDefeatCondition(JDefeatCondition whichCondition)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyDefeatCondition"), whichCondition.Handle);
        }

        public static void DefeatConditionSetDescription(JDefeatCondition whichCondition, string description)
        {
            War3.CallNative(War3.GetNativeFunction("DefeatConditionSetDescription"), whichCondition.Handle, description);
        }

        public static void FlashQuestDialogButton()
        {
            War3.CallNative(War3.GetNativeFunction("FlashQuestDialogButton"));
        }

        public static void ForceQuestDialogUpdate()
        {
            War3.CallNative(War3.GetNativeFunction("ForceQuestDialogUpdate"));
        }


        /// title = "新建计时器窗口 [R]"
        /// description = "为 ${计时器} 新建计时窗口"
        /// comment = "为一个计时器创建一个新建计时器窗口."

        public static JTimerDialog CreateTimerDialog(JTimer t)
        {
            return War3.CallNative<JTimerDialog>(War3.GetNativeFunction("CreateTimerDialog"), t.Handle);
        }

        public static void DestroyTimerDialog(JTimerDialog whichDialog)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyTimerDialog"), whichDialog.Handle);
        }

        public static void TimerDialogSetTitle(JTimerDialog whichDialog, string title)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetTitle"), whichDialog.Handle, title);
        }


        /// title = "改变计时器窗口文字颜色 [R]"
        /// description = "改变 ${Timer Window} 文字颜色为(${Red},${Green},${Blue}) 透明值为: ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和透明值值取值范围为0-255."

        public static void TimerDialogSetTitleColor(JTimerDialog whichDialog, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetTitleColor"), whichDialog.Handle, red, green, blue, alpha);
        }


        /// title = "改变计时器窗口计时颜色 [R]"
        /// description = "改变 ${Timer Window} 的计间颜色为(${Red},${Green},${Blue}) 透明值为: ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和透明值值取值范围为0-255."

        public static void TimerDialogSetTimeColor(JTimerDialog whichDialog, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetTimeColor"), whichDialog.Handle, red, green, blue, alpha);
        }


        /// title = "设置计时器窗口速率 [R]"
        /// description = "设置 ${Timer Window} 的时间流逝速度为 ${Factor} 倍"
        /// comment = " 同时计时器显示时间也会随之变化. 就是说60秒的计时器设置为2倍速则显示时间也会变为120秒."

        public static void TimerDialogSetSpeed(JTimerDialog whichDialog, float speedMultFactor)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetSpeed"), whichDialog.Handle, speedMultFactor);
        }


        /// title = "显示/隐藏 计时器窗口(所有玩家) [R]"
        /// description = "设置 ${计时器窗口} 的状态为${Show/Hide}"
        /// comment = "计时器窗口不能在地图初始化时显示."

        public static void TimerDialogDisplay(JTimerDialog whichDialog, bool display)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogDisplay"), whichDialog.Handle, display);
        }

        public static bool IsTimerDialogDisplayed(JTimerDialog whichDialog)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsTimerDialogDisplayed"), whichDialog.Handle);
        }

        public static void TimerDialogSetRealTimeRemaining(JTimerDialog whichDialog, float timeRemaining)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetRealTimeRemaining"), whichDialog.Handle, timeRemaining);
        }


        /// title = "新建排行榜 [R]"
        /// description = "新建的空排行榜"
        /// comment = "会创建排行榜."

        public static JLeaderboard CreateLeaderboard()
        {
            return War3.CallNative<JLeaderboard>(War3.GetNativeFunction("CreateLeaderboard"));
        }

        public static void DestroyLeaderboard(JLeaderboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyLeaderboard"), lb.Handle);
        }


        /// title = "显示/隐藏 [R]"
        /// description = "设置 ${排行榜} ${Show/Hide}"
        /// comment = "排行榜不能在地图初始化时显示."

        public static void LeaderboardDisplay(JLeaderboard lb, bool show)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardDisplay"), lb.Handle, show);
        }

        public static bool IsLeaderboardDisplayed(JLeaderboard lb)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsLeaderboardDisplayed"), lb.Handle);
        }


        /// title = "行数"
        /// description = "${Leaderboard} 的行数"
        /// comment = ""

        public static int LeaderboardGetItemCount(JLeaderboard lb)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("LeaderboardGetItemCount"), lb.Handle);
        }

        public static void LeaderboardSetSizeByItemCount(JLeaderboard lb, int count)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetSizeByItemCount"), lb.Handle, count);
        }

        public static void LeaderboardAddItem(JLeaderboard lb, string label, int value, JPlayer p)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardAddItem"), lb.Handle, label, value, p.Handle);
        }

        public static void LeaderboardRemoveItem(JLeaderboard lb, int index)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardRemoveItem"), lb.Handle, index);
        }

        public static void LeaderboardRemovePlayerItem(JLeaderboard lb, JPlayer p)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardRemovePlayerItem"), lb.Handle, p.Handle);
        }


        /// title = "清空 [R]"
        /// description = "清空 ${排行榜}"
        /// comment = "清空排行榜中所有内容."

        public static void LeaderboardClear(JLeaderboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardClear"), lb.Handle);
        }

        public static void LeaderboardSortItemsByValue(JLeaderboard lb, bool ascending)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSortItemsByValue"), lb.Handle, ascending);
        }

        public static void LeaderboardSortItemsByPlayer(JLeaderboard lb, bool ascending)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSortItemsByPlayer"), lb.Handle, ascending);
        }

        public static void LeaderboardSortItemsByLabel(JLeaderboard lb, bool ascending)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSortItemsByLabel"), lb.Handle, ascending);
        }

        public static bool LeaderboardHasPlayerItem(JLeaderboard lb, JPlayer p)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("LeaderboardHasPlayerItem"), lb.Handle, p.Handle);
        }

        public static int LeaderboardGetPlayerIndex(JLeaderboard lb, JPlayer p)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("LeaderboardGetPlayerIndex"), lb.Handle, p.Handle);
        }

        public static void LeaderboardSetLabel(JLeaderboard lb, string label)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetLabel"), lb.Handle, label);
        }

        public static string LeaderboardGetLabelText(JLeaderboard lb)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("LeaderboardGetLabelText"), lb.Handle);
        }


        /// title = "设置玩家使用的排行榜 [R]"
        /// description = "设置 ${Player} 使用 ${排行榜}"
        /// comment = "每个玩家只能显示一个排行榜."

        public static void PlayerSetLeaderboard(JPlayer toPlayer, JLeaderboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("PlayerSetLeaderboard"), toPlayer.Handle, lb.Handle);
        }

        public static JLeaderboard PlayerGetLeaderboard(JPlayer toPlayer)
        {
            return War3.CallNative<JLeaderboard>(War3.GetNativeFunction("PlayerGetLeaderboard"), toPlayer.Handle);
        }


        /// title = "设置文字颜色 [R]"
        /// description = "设置 ${Leaderboard} 的文字颜色为(${Red},${Green},${Blue}) Alpha通道值为: ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和Alpha通道值取值范围为0-255."

        public static void LeaderboardSetLabelColor(JLeaderboard lb, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetLabelColor"), lb.Handle, red, green, blue, alpha);
        }


        /// title = "设置数值颜色 [R]"
        /// description = "设置 ${Leaderboard} 的数值颜色为(${Red},${Green},${Blue}) Alpha通道值为: ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和Alpha通道值取值范围为0-255."

        public static void LeaderboardSetValueColor(JLeaderboard lb, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetValueColor"), lb.Handle, red, green, blue, alpha);
        }

        public static void LeaderboardSetStyle(JLeaderboard lb, bool showLabel, bool showNames, bool showValues, bool showIcons)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetStyle"), lb.Handle, showLabel, showNames, showValues, showIcons);
        }

        public static void LeaderboardSetItemValue(JLeaderboard lb, int whichItem, int val)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemValue"), lb.Handle, whichItem, val);
        }

        public static void LeaderboardSetItemLabel(JLeaderboard lb, int whichItem, string val)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemLabel"), lb.Handle, whichItem, val);
        }

        public static void LeaderboardSetItemStyle(JLeaderboard lb, int whichItem, bool showLabel, bool showValue, bool showIcon)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemStyle"), lb.Handle, whichItem, showLabel, showValue, showIcon);
        }

        public static void LeaderboardSetItemLabelColor(JLeaderboard lb, int whichItem, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemLabelColor"), lb.Handle, whichItem, red, green, blue, alpha);
        }

        public static void LeaderboardSetItemValueColor(JLeaderboard lb, int whichItem, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemValueColor"), lb.Handle, whichItem, red, green, blue, alpha);
        }


        /// title = "新建多面板 [R]"
        /// description = "新建的空多面板"
        /// comment = "会创建多面板."

        public static JMultiboard CreateMultiboard()
        {
            return War3.CallNative<JMultiboard>(War3.GetNativeFunction("CreateMultiboard"));
        }

        public static void DestroyMultiboard(JMultiboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyMultiboard"), lb.Handle);
        }


        /// title = "显示/隐藏 [R]"
        /// description = "设置 ${Multiboard} ${Show/Hide}"
        /// comment = "多面板不能在地图初始化时显示."

        public static void MultiboardDisplay(JMultiboard lb, bool show)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardDisplay"), lb.Handle, show);
        }


        /// title = "多面板显示"
        /// description = "${Multiboard} 是显示的"
        /// comment = ""

        public static bool IsMultiboardDisplayed(JMultiboard lb)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsMultiboardDisplayed"), lb.Handle);
        }


        /// title = "最大/最小化 [R]"
        /// description = "设置 ${Multiboard} ${Minimize/Maximize}"
        /// comment = "最小化的多面板只显示标题."

        public static void MultiboardMinimize(JMultiboard lb, bool minimize)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardMinimize"), lb.Handle, minimize);
        }


        /// title = "多面板最小化"
        /// description = "${Multiboard} 是最小化的"
        /// comment = ""

        public static bool IsMultiboardMinimized(JMultiboard lb)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsMultiboardMinimized"), lb.Handle);
        }


        /// title = "清空多面板"
        /// description = "清空 ${Multiboard}"
        /// comment = "清空该多面板中的所有行和列."

        public static void MultiboardClear(JMultiboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardClear"), lb.Handle);
        }


        /// title = "设置标题"
        /// description = "设置 ${Multiboard} 的标题为 ${文字}"
        /// comment = ""

        public static void MultiboardSetTitleText(JMultiboard lb, string label)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetTitleText"), lb.Handle, label);
        }


        /// title = "多面板标题"
        /// description = "${Multiboard} 的标题"
        /// comment = ""

        public static string MultiboardGetTitleText(JMultiboard lb)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("MultiboardGetTitleText"), lb.Handle);
        }


        /// title = "设置标题颜色 [R]"
        /// description = "设置 ${Multiboard} 的标题颜色为(${Red},${Green},${Blue}), Alpha值为 ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255."

        public static void MultiboardSetTitleTextColor(JMultiboard lb, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetTitleTextColor"), lb.Handle, red, green, blue, alpha);
        }


        /// title = "行数"
        /// description = "${Multiboard} 的行数"
        /// comment = ""

        public static int MultiboardGetRowCount(JMultiboard lb)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("MultiboardGetRowCount"), lb.Handle);
        }


        /// title = "列数"
        /// description = "${Multiboard} 的列数"
        /// comment = ""

        public static int MultiboardGetColumnCount(JMultiboard lb)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("MultiboardGetColumnCount"), lb.Handle);
        }


        /// title = "设置列数"
        /// description = "设置 ${Multiboard} 的列数为 ${Columns}"
        /// comment = ""

        public static void MultiboardSetColumnCount(JMultiboard lb, int count)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetColumnCount"), lb.Handle, count);
        }


        /// title = "设置行数"
        /// description = "设置 ${Multiboard} 的行数为 ${Rows}"
        /// comment = ""

        public static void MultiboardSetRowCount(JMultiboard lb, int count)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetRowCount"), lb.Handle, count);
        }


        /// title = "设置所有项目显示风格 [R]"
        /// description = "设置 ${多面板} 的所有项目显示风格: ${Show/Hide} 文字 ${Show/Hide} 图标"

        public static void MultiboardSetItemsStyle(JMultiboard lb, bool showValues, bool showIcons)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsStyle"), lb.Handle, showValues, showIcons);
        }


        /// title = "设置所有项目文本 [R]"
        /// description = "设置 ${多面板} 的所有项目文本为 ${文字}"

        public static void MultiboardSetItemsValue(JMultiboard lb, string value)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsValue"), lb.Handle, value);
        }


        /// title = "设置所有项目颜色 [R]"
        /// description = "设置 ${多面板} 的所有项目颜色为(${Red},${Green},${Blue}), Alpha值为 ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255."

        public static void MultiboardSetItemsValueColor(JMultiboard lb, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsValueColor"), lb.Handle, red, green, blue, alpha);
        }


        /// title = "设置所有项目宽度 [R]"
        /// description = "设置 ${多面板} 的所有项目宽度为 ${Width} 倍屏幕宽度"

        public static void MultiboardSetItemsWidth(JMultiboard lb, float width)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsWidth"), lb.Handle, width);
        }


        /// title = "设置所有项目图标 [R]"
        /// description = "设置 ${多面板} 的所有项目图标为 ${Icon File}"

        public static void MultiboardSetItemsIcon(JMultiboard lb, string iconPath)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsIcon"), lb.Handle, iconPath);
        }


        /// title = "多面板项目 [R]"
        /// description = "${多面板} 的第 ${X} 行,第 ${Y} 列项"
        /// comment = "(0,0)作为多面板首项,会创建多面板项目."

        public static JMultiboardItem MultiboardGetItem(JMultiboard lb, int row, int column)
        {
            return War3.CallNative<JMultiboardItem>(War3.GetNativeFunction("MultiboardGetItem"), lb.Handle, row, column);
        }


        /// title = "删除多面板项目 [R]"
        /// description = "删除 ${多面板项目}"
        /// comment = "并不会影响对多面板的显示. 多面板项目指向多面板但不附属于多面板."

        public static void MultiboardReleaseItem(JMultiboardItem mbi)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardReleaseItem"), mbi.Handle);
        }


        /// title = "设置指定项目显示风格 [R]"
        /// description = "设置 ${多面板项目} 的显示风格: ${Show/Hide} 文字 ${Show/Hide} 图标"
        /// comment = ""

        public static void MultiboardSetItemStyle(JMultiboardItem mbi, bool showValue, bool showIcon)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemStyle"), mbi.Handle, showValue, showIcon);
        }


        /// title = "设置指定项目文本 [R]"
        /// description = "设置 ${多面板项目} 的项目文本为 ${文字}"
        /// comment = ""

        public static void MultiboardSetItemValue(JMultiboardItem mbi, string val)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemValue"), mbi.Handle, val);
        }


        /// title = "设置指定项目颜色 [R]"
        /// description = "设置 ${多面板项目} 的项目颜色为(${Red},${Green},${Blue}), Alpha值为 ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255."

        public static void MultiboardSetItemValueColor(JMultiboardItem mbi, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemValueColor"), mbi.Handle, red, green, blue, alpha);
        }


        /// title = "设置指定项目宽度 [R]"
        /// description = "设置 ${多面板项目} 的项目宽度为 ${Width} 倍屏幕宽度"

        public static void MultiboardSetItemWidth(JMultiboardItem mbi, float width)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemWidth"), mbi.Handle, width);
        }


        /// title = "设置指定项目图标 [R]"
        /// description = "设置 ${多面板项目} 的项目图标为 ${Icon File}"

        public static void MultiboardSetItemIcon(JMultiboardItem mbi, string iconFileName)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemIcon"), mbi.Handle, iconFileName);
        }


        /// title = "显示/隐藏多面板模式 [R]"
        /// description = "${打开/关闭} 隐藏多面板模式"
        /// comment = "隐藏多面板模式将无法显示多面板."

        public static void MultiboardSuppressDisplay(bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSuppressDisplay"), flag);
        }

        public static void SetCameraPosition(float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraPosition"), x, y);
        }


        /// title = "设置空格键转向点(所有玩家) [R]"
        /// description = "设置玩家的空格键转向点为(${X},${Y})"
        /// comment = "按下空格键时镜头转向的位置."

        public static void SetCameraQuickPosition(float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraQuickPosition"), x, y);
        }


        /// title = "设置可用镜头区域(所有玩家) [R]"
        /// description = "设置玩家可用镜头区域: 左下角(${X},${Y}), 左上角(${X},${Y}), 右上角(${X},${Y}), 右下角(${X},${Y})"
        /// comment = "该动作同样会影响小地图的显示. 但小地图的图片是无法改变的. 实际可用区域要大于可用镜头区域."

        public static void SetCameraBounds(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraBounds"), x1, y1, x2, y2, x3, y3, x4, y4);
        }


        /// title = "停止播放镜头(所有玩家) [R]"
        /// description = "让所有玩家停止播放镜头"
        /// comment = "比如在平移镜头的过程中可用该动作来中断平移."

        public static void StopCamera()
        {
            War3.CallNative(War3.GetNativeFunction("StopCamera"));
        }


        /// title = "重置游戏镜头(所有玩家) [R]"
        /// description = "重置玩家镜头为游戏默认状态,持续  ${Time} 秒"
        /// comment = ""

        public static void ResetToGameCamera(float duration)
        {
            War3.CallNative(War3.GetNativeFunction("ResetToGameCamera"), duration);
        }

        public static void PanCameraTo(float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("PanCameraTo"), x, y);
        }


        /// title = "平移镜头(所有玩家)(限时) [R]"
        /// description = "平移玩家镜头到(${X},${Y}),持续 ${Time} 秒"
        /// comment = ""

        public static void PanCameraToTimed(float x, float y, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("PanCameraToTimed"), x, y, duration);
        }

        public static void PanCameraToWithZ(float x, float y, float zOffsetDest)
        {
            War3.CallNative(War3.GetNativeFunction("PanCameraToWithZ"), x, y, zOffsetDest);
        }


        /// title = "指定高度平移镜头(所有玩家)(限时) [R]"
        /// description = "平移玩家镜头到(${X},${Y}),镜头距离地面高度为 ${Z},持续 ${Time} 秒"
        /// comment = "在指定移动路径上镜头不会低于地面高度."

        public static void PanCameraToTimedWithZ(float x, float y, float zOffsetDest, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("PanCameraToTimedWithZ"), x, y, zOffsetDest, duration);
        }


        /// title = "播放电影镜头(所有玩家) [R]"
        /// description = "对所有玩家播放电影镜头: ${Camera File}"
        /// comment = "在'Objects\\CinematicCameras'目录下有一些电影镜头,可用Mpq工具来查询."

        public static void SetCinematicCamera(string cameraModelFile)
        {
            War3.CallNative(War3.GetNativeFunction("SetCinematicCamera"), cameraModelFile);
        }


        /// title = "指定点旋转镜头(所有玩家)(弧度)(限时) [R]"
        /// description = "以(${X},${Y})为中心,旋转弧度为${Rad}, 持续: ${Time} 秒"
        /// comment = ""

        public static void SetCameraRotateMode(float x, float y, float radiansToSweep, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraRotateMode"), x, y, radiansToSweep, duration);
        }


        /// title = "设置镜头属性(所有玩家)(限时) [R]"
        /// description = "设置玩家的镜头属性 ${Field} 为 ${数值},持续 ${Time} 秒"
        /// comment = ""

        public static void SetCameraField(JCameraField whichField, float value, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraField"), whichField.Handle, value, duration);
        }

        public static void AdjustCameraField(JCameraField whichField, float offset, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("AdjustCameraField"), whichField.Handle, offset, duration);
        }


        /// title = "锁定镜头到单位(所有玩家) [R]"
        /// description = "锁定玩家镜头到 ${单位}, 偏移坐标(${X}, ${Y}) ,使用 ${Rotation Source}"
        /// comment = "偏移坐标(X,Y)以单位脚底为原点坐标."

        public static void SetCameraTargetController(JUnit whichUnit, float xoffset, float yoffset, bool inheritOrientation)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraTargetController"), whichUnit.Handle, xoffset, yoffset, inheritOrientation);
        }


        /// title = "锁定镜头到单位(固定镜头源)(所有玩家) [R]"
        /// description = "锁定玩家镜头到 ${单位}, 偏移坐标(${X}, ${Y})"
        /// comment = "偏移坐标(X,Y)以单位脚底为原点坐标."

        public static void SetCameraOrientController(JUnit whichUnit, float xoffset, float yoffset)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraOrientController"), whichUnit.Handle, xoffset, yoffset);
        }

        public static JCameraSetup CreateCameraSetup()
        {
            return War3.CallNative<JCameraSetup>(War3.GetNativeFunction("CreateCameraSetup"));
        }

        public static void CameraSetupSetField(JCameraSetup whichSetup, JCameraField whichField, float value, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupSetField"), whichSetup.Handle, whichField.Handle, value, duration);
        }


        /// title = "镜头属性(指定镜头) [R]"
        /// description = "${镜头} 的 ${Camera Field}"
        /// comment = ""

        public static float CameraSetupGetField(JCameraSetup whichSetup, JCameraField whichField)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("CameraSetupGetField"), whichSetup.Handle, whichField.Handle);
        }

        public static void CameraSetupSetDestPosition(JCameraSetup whichSetup, float x, float y, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupSetDestPosition"), whichSetup.Handle, x, y, duration);
        }


        /// title = "镜头目标点"
        /// description = "${镜头} 的目标点"
        /// comment = "会创建点."

        public static JLocation CameraSetupGetDestPositionLoc(JCameraSetup whichSetup)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("CameraSetupGetDestPositionLoc"), whichSetup.Handle);
        }

        public static float CameraSetupGetDestPositionX(JCameraSetup whichSetup)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("CameraSetupGetDestPositionX"), whichSetup.Handle);
        }

        public static float CameraSetupGetDestPositionY(JCameraSetup whichSetup)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("CameraSetupGetDestPositionY"), whichSetup.Handle);
        }

        public static void CameraSetupApply(JCameraSetup whichSetup, bool doPan, bool panTimed)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupApply"), whichSetup.Handle, doPan, panTimed);
        }

        public static void CameraSetupApplyWithZ(JCameraSetup whichSetup, float zDestOffset)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupApplyWithZ"), whichSetup.Handle, zDestOffset);
        }


        /// title = "应用镜头(所有玩家)(限时) [R]"
        /// description = "将 ${镜头} 应用方式设置为 ${Apply Method},持续 ${Time} 秒"
        /// comment = ""

        public static void CameraSetupApplyForceDuration(JCameraSetup whichSetup, bool doPan, float forceDuration)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupApplyForceDuration"), whichSetup.Handle, doPan, forceDuration);
        }

        public static void CameraSetupApplyForceDurationWithZ(JCameraSetup whichSetup, float zDestOffset, float forceDuration)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupApplyForceDurationWithZ"), whichSetup.Handle, zDestOffset, forceDuration);
        }

        public static void CameraSetTargetNoise(float mag, float velocity)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetTargetNoise"), mag, velocity);
        }

        public static void CameraSetSourceNoise(float mag, float velocity)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetSourceNoise"), mag, velocity);
        }


        /// title = "摇晃镜头目标(所有玩家) [R]"
        /// description = "摇晃玩家的镜头源, 摇晃幅度: ${Magnitude} 速率: ${Velocity} 摇晃方式: ${方式}"
        /// comment = "使用'镜头 - 重置镜头'或设置摇晃幅度和速率为0来停止摇晃."

        public static void CameraSetTargetNoiseEx(float mag, float velocity, bool vertOnly)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetTargetNoiseEx"), mag, velocity, vertOnly);
        }


        /// title = "摇晃镜头源(所有玩家) [R]"
        /// description = "摇晃玩家的镜头源, 摇晃幅度: ${Magnitude} 速率: ${Velocity} 摇晃方式: ${方式}"
        /// comment = "使用'镜头 - 重置镜头'或设置摇晃幅度和速率为0来停止摇晃."

        public static void CameraSetSourceNoiseEx(float mag, float velocity, bool vertOnly)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetSourceNoiseEx"), mag, velocity, vertOnly);
        }

        public static void CameraSetSmoothingFactor(float factor)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetSmoothingFactor"), factor);
        }

        public static void SetCineFilterTexture(string filename)
        {
            War3.CallNative(War3.GetNativeFunction("SetCineFilterTexture"), filename);
        }

        public static void SetCineFilterBlendMode(JBlendMode whichMode)
        {
            War3.CallNative(War3.GetNativeFunction("SetCineFilterBlendMode"), whichMode.Handle);
        }

        public static void SetCineFilterTexMapFlags(JTexMapFlags whichFlags)
        {
            War3.CallNative(War3.GetNativeFunction("SetCineFilterTexMapFlags"), whichFlags.Handle);
        }

        public static void SetCineFilterStartUV(float minu, float minv, float maxu, float maxv)
        {
            War3.CallNative(War3.GetNativeFunction("SetCineFilterStartUV"), minu, minv, maxu, maxv);
        }

        public static void SetCineFilterEndUV(float minu, float minv, float maxu, float maxv)
        {
            War3.CallNative(War3.GetNativeFunction("SetCineFilterEndUV"), minu, minv, maxu, maxv);
        }

        public static void SetCineFilterStartColor(int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetCineFilterStartColor"), red, green, blue, alpha);
        }

        public static void SetCineFilterEndColor(int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetCineFilterEndColor"), red, green, blue, alpha);
        }

        public static void SetCineFilterDuration(float duration)
        {
            War3.CallNative(War3.GetNativeFunction("SetCineFilterDuration"), duration);
        }

        public static void DisplayCineFilter(bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("DisplayCineFilter"), flag);
        }

        public static bool IsCineFilterDisplayed()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsCineFilterDisplayed"));
        }

        public static void SetCinematicScene(int portraitUnitId, JPlayerColor color, string speakerTitle, string text, float sceneDuration, float voiceoverDuration)
        {
            War3.CallNative(War3.GetNativeFunction("SetCinematicScene"), portraitUnitId, color.Handle, speakerTitle, text, sceneDuration, voiceoverDuration);
        }

        public static void EndCinematicScene()
        {
            War3.CallNative(War3.GetNativeFunction("EndCinematicScene"));
        }

        public static void ForceCinematicSubtitles(bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("ForceCinematicSubtitles"), flag);
        }

        public static float GetCameraMargin(int whichMargin)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraMargin"), whichMargin);
        }

        public static float GetCameraBoundMinX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraBoundMinX"));
        }

        public static float GetCameraBoundMinY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraBoundMinY"));
        }

        public static float GetCameraBoundMaxX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraBoundMaxX"));
        }

        public static float GetCameraBoundMaxY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraBoundMaxY"));
        }


        /// title = "镜头属性(当前镜头)"
        /// description = "当前镜头的 ${Camera Field}"
        /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."

        public static float GetCameraField(JCameraField whichField)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraField"), whichField.Handle);
        }


        /// title = "当前镜头目标X坐标"
        /// description = "当前镜头目标X坐标"
        /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."

        public static float GetCameraTargetPositionX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraTargetPositionX"));
        }


        /// title = "当前镜头目标Y坐标"
        /// description = "当前镜头目标Y坐标"
        /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."

        public static float GetCameraTargetPositionY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraTargetPositionY"));
        }


        /// title = "当前镜头目标Z坐标"
        /// description = "当前镜头目标Z坐标"
        /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."

        public static float GetCameraTargetPositionZ()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraTargetPositionZ"));
        }


        /// title = "当前镜头目标点"
        /// description = "当前镜头目标点"
        /// comment = "会创建点. 注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."

        public static JLocation GetCameraTargetPositionLoc()
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("GetCameraTargetPositionLoc"));
        }


        /// title = "当前镜头源X坐标"
        /// description = "当前镜头源X坐标"
        /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."

        public static float GetCameraEyePositionX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraEyePositionX"));
        }


        /// title = "当前镜头源Y坐标"
        /// description = "当前镜头源Y坐标"
        /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."

        public static float GetCameraEyePositionY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraEyePositionY"));
        }


        /// title = "当前镜头源Z坐标"
        /// description = "当前镜头源Z坐标"
        /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."

        public static float GetCameraEyePositionZ()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetCameraEyePositionZ"));
        }


        /// title = "当前镜头源位置"
        /// description = "当前镜头源位置"
        /// comment = "会创建点. 注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."

        public static JLocation GetCameraEyePositionLoc()
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("GetCameraEyePositionLoc"));
        }


        /// title = "设置环境音效 [new]"
        /// description = "设置环境音效为 ${environmentName}"
        /// comment = ""

        public static void NewSoundEnvironment(string environmentName)
        {
            War3.CallNative(War3.GetNativeFunction("NewSoundEnvironment"), environmentName);
        }


        /// title = "创建声音 [new]"
        /// description = "创建声音，路径：${fileName}，是否循环播放 ${looping}，是否为3D声音 ${is3D}，超出范围是否停止 ${stopwhenoutofrange}，淡入速率 ${fadeInRate}，淡出速率 ${fadeOutRate}，效果 ${eaxSetting}"
        /// comment = ""

        public static JSound CreateSound(string fileName, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate, int fadeOutRate, string eaxSetting)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("CreateSound"), fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, eaxSetting);
        }


        /// title = "根据文件名和标签创建声音 [new]"
        /// description = "创建声音， 路径：${fileName} ，是否循环播放 ${looping}，是否为3D声音 ${is3D}，超出范围是否停止 ${stopwhenoutofrange}，淡入速率 ${fadeInRate}，淡出速率 ${fadeOutRate} 设置标签 ${SLKEntryName}"
        /// comment = ""

        public static JSound CreateSoundFilenameWithLabel(string fileName, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate, int fadeOutRate, string SLKEntryName)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("CreateSoundFilenameWithLabel"), fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, SLKEntryName);
        }


        /// title = "从标签创建声音 [new]"
        /// description = "根据标签 ${soundLabel} 创建声音，是否循环播放 ${looping}，是否为3D声音 ${is3D}，超出范围是否停止 ${stopwhenoutofrange}，淡入速率 ${fadeInRate}，淡出速率 ${fadeOutRate}"
        /// comment = ""

        public static JSound CreateSoundFromLabel(string soundLabel, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate, int fadeOutRate)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("CreateSoundFromLabel"), soundLabel, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate);
        }


        /// title = "创建MIDI音乐 [new]"
        /// description = "创建MIDI音乐，标签为 ${soundLabel}，淡入速率 ${fadeInRate}，淡出速率 ${fadeOutRate}"
        /// comment = ""

        public static JSound CreateMIDISound(string soundLabel, int fadeInRate, int fadeOutRate)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("CreateMIDISound"), soundLabel, fadeInRate, fadeOutRate);
        }


        /// title = "设置声音标签 [new]"
        /// description = "设置 ${soundLabel} 的标签：${soundHandle}"
        /// comment = ""

        public static void SetSoundParamsFromLabel(JSound soundHandle, string soundLabel)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundParamsFromLabel"), soundHandle.Handle, soundLabel);
        }

        public static void SetSoundDistanceCutoff(JSound soundHandle, float cutoff)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundDistanceCutoff"), soundHandle.Handle, cutoff);
        }


        /// title = "设置声音的声道 [new]"
        /// description = "设置 ${soundHandle} 的声道：${channel}"
        /// comment = ""

        public static void SetSoundChannel(JSound soundHandle, int channel)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundChannel"), soundHandle.Handle, channel);
        }


        /// title = "设置音效音量 [R]"
        /// description = "设置 ${音效} 的音量为 ${Volume}"
        /// comment = "音量取值范围0-127."

        public static void SetSoundVolume(JSound soundHandle, int volume)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundVolume"), soundHandle.Handle, volume);
        }

        public static void SetSoundPitch(JSound soundHandle, float pitch)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundPitch"), soundHandle.Handle, pitch);
        }


        /// title = "设置音效播放时间点 [R]"
        /// description = "设置 ${音效} 的播放时间点为 ${Offset} 毫秒"
        /// comment = "音效必须是正在播放的. 不能用于3D音效."

        public static void SetSoundPlayPosition(JSound soundHandle, int millisecs)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundPlayPosition"), soundHandle.Handle, millisecs);
        }


        /// title = "设置3D音效衰减范围"
        /// description = "设置 ${3D音效} 的衰减最小范围: ${数值} 最大范围: ${数值}"
        /// comment = "该动作仅用于3D音效. 注意不一定要达到最大范围,音量衰减到一定程度也会变没的."

        public static void SetSoundDistances(JSound soundHandle, float minDist, float maxDist)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundDistances"), soundHandle.Handle, minDist, maxDist);
        }


        /// title = "设置声音的角度 [new]"
        /// description = "设置 ${soundHandle} 的内圆锥角度： ${inside}，外圆锥角度：${outside}，外圆锥音量：${outsideVolume}"
        /// comment = ""

        public static void SetSoundConeAngles(JSound soundHandle, float inside, float outside, int outsideVolume)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundConeAngles"), soundHandle.Handle, inside, outside, outsideVolume);
        }


        /// title = "设置声音的传播方向 [new]"
        /// description = "设置 ${soundHandle} 的传播方向(${x}, ${y}, ${z})"
        /// comment = ""

        public static void SetSoundConeOrientation(JSound soundHandle, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundConeOrientation"), soundHandle.Handle, x, y, z);
        }


        /// title = "设置3D音效位置(指定坐标) [R]"
        /// description = "设置 ${3D音效} 的播放位置为(${X},${Y}), Z轴高度为 ${Z}"
        /// comment = "该动作仅用于3D音效."

        public static void SetSoundPosition(JSound soundHandle, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundPosition"), soundHandle.Handle, x, y, z);
        }


        /// title = "设置声音的速度 [new]"
        /// description = "设置 ${soundHandle} 的速度设置为 (${x}, ${y}, ${z})"
        /// comment = ""

        public static void SetSoundVelocity(JSound soundHandle, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundVelocity"), soundHandle.Handle, x, y, z);
        }

        public static void AttachSoundToUnit(JSound soundHandle, JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("AttachSoundToUnit"), soundHandle.Handle, whichUnit.Handle);
        }

        public static void StartSound(JSound soundHandle)
        {
            War3.CallNative(War3.GetNativeFunction("StartSound"), soundHandle.Handle);
        }

        public static void StopSound(JSound soundHandle, bool killWhenDone, bool fadeOut)
        {
            War3.CallNative(War3.GetNativeFunction("StopSound"), soundHandle.Handle, killWhenDone, fadeOut);
        }

        public static void KillSoundWhenDone(JSound soundHandle)
        {
            War3.CallNative(War3.GetNativeFunction("KillSoundWhenDone"), soundHandle.Handle);
        }


        /// title = "设置背景音乐列表 [R]"
        /// description = "设置背景音乐列表为: ${Music} , ${允许/禁止} 随机播放, 开始播放序号为 ${Index}"
        /// comment = "可指定播放文件或播放目录."

        public static void SetMapMusic(string musicName, bool random, int index)
        {
            War3.CallNative(War3.GetNativeFunction("SetMapMusic"), musicName, random, index);
        }

        public static void ClearMapMusic()
        {
            War3.CallNative(War3.GetNativeFunction("ClearMapMusic"));
        }

        public static void PlayMusic(string musicName)
        {
            War3.CallNative(War3.GetNativeFunction("PlayMusic"), musicName);
        }

        public static void PlayMusicEx(string musicName, int frommsecs, int fadeinmsecs)
        {
            War3.CallNative(War3.GetNativeFunction("PlayMusicEx"), musicName, frommsecs, fadeinmsecs);
        }

        public static void StopMusic(bool fadeOut)
        {
            War3.CallNative(War3.GetNativeFunction("StopMusic"), fadeOut);
        }

        public static void ResumeMusic()
        {
            War3.CallNative(War3.GetNativeFunction("ResumeMusic"));
        }


        /// title = "播放主题音乐 [C]"
        /// description = "播放 ${Music Theme} 主题音乐"
        /// comment = "播放主题音乐一次,然后恢复原来的音乐."

        public static void PlayThematicMusic(string musicFileName)
        {
            War3.CallNative(War3.GetNativeFunction("PlayThematicMusic"), musicFileName);
        }


        /// title = "跳播主题音乐 [R]"
        /// description = "播放 ${Music Theme} 主题音乐,跳过开始 ${Offset} 毫秒"

        public static void PlayThematicMusicEx(string musicFileName, int frommsecs)
        {
            War3.CallNative(War3.GetNativeFunction("PlayThematicMusicEx"), musicFileName, frommsecs);
        }


        /// title = "停止主题音乐[C]"
        /// description = "停止正在播放的主题音乐"
        /// comment = ""

        public static void EndThematicMusic()
        {
            War3.CallNative(War3.GetNativeFunction("EndThematicMusic"));
        }


        /// title = "设置背景音乐音量 [R]"
        /// description = "设置背景音乐音量为 ${Volume}"
        /// comment = "音量取值范围为0-127."

        public static void SetMusicVolume(int volume)
        {
            War3.CallNative(War3.GetNativeFunction("SetMusicVolume"), volume);
        }


        /// title = "设置背景音乐播放时间点 [R]"
        /// description = "设置当前背景音乐的播放时间点为 ${Offset} 毫秒"

        public static void SetMusicPlayPosition(int millisecs)
        {
            War3.CallNative(War3.GetNativeFunction("SetMusicPlayPosition"), millisecs);
        }


        /// title = "设置主题音乐播放时间点 [R]"
        /// description = "设置当前主题音乐播放时间点为 ${Offset} 毫秒"
        /// comment = ""

        public static void SetThematicMusicPlayPosition(int millisecs)
        {
            War3.CallNative(War3.GetNativeFunction("SetThematicMusicPlayPosition"), millisecs);
        }

        public static void SetSoundDuration(JSound soundHandle, int duration)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundDuration"), soundHandle.Handle, duration);
        }

        public static int GetSoundDuration(JSound soundHandle)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetSoundDuration"), soundHandle.Handle);
        }

        public static int GetSoundFileDuration(string musicFileName)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetSoundFileDuration"), musicFileName);
        }


        /// title = "设置多通道音量 [R]"
        /// description = "设置 ${Volume Channel} 的音量为 ${Volume}"
        /// comment = "音量取值范围0-1."

        public static void VolumeGroupSetVolume(JVolumeGroup vgroup, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("VolumeGroupSetVolume"), vgroup.Handle, scale);
        }

        public static void VolumeGroupReset()
        {
            War3.CallNative(War3.GetNativeFunction("VolumeGroupReset"));
        }


        /// title = "声音在播放 [new]"
        /// description = " ${soundHandle} 在播放"
        /// comment = ""

        public static bool GetSoundIsPlaying(JSound soundHandle)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetSoundIsPlaying"), soundHandle.Handle);
        }


        /// title = "声音在加载 [new]"
        /// description = " ${soundHandle} 在加载"
        /// comment = ""

        public static bool GetSoundIsLoading(JSound soundHandle)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetSoundIsLoading"), soundHandle.Handle);
        }


        /// title = "注册堆叠音效 [new]"
        /// description = "设置 ${soundHandle} 的堆叠，是否按位置来处理堆叠：${byPosition},堆叠区域的宽度：${rectwidth}, 堆叠区域的高度：${rectheight}"
        /// comment = ""

        public static void RegisterStackedSound(JSound soundHandle, bool byPosition, float rectwidth, float rectheight)
        {
            War3.CallNative(War3.GetNativeFunction("RegisterStackedSound"), soundHandle.Handle, byPosition, rectwidth, rectheight);
        }


        /// title = "取消注册的堆叠音效设置 [new]"
        /// description = "取消 ${soundHandle} 的堆叠，是否按位置来处理堆叠：${byPosition},堆叠区域的宽度：${rectwidth}, 堆叠区域的高度：${rectheight}"
        /// comment = ""

        public static void UnregisterStackedSound(JSound soundHandle, bool byPosition, float rectwidth, float rectheight)
        {
            War3.CallNative(War3.GetNativeFunction("UnregisterStackedSound"), soundHandle.Handle, byPosition, rectwidth, rectheight);
        }


        /// title = "新建天气效果 [R]"
        /// description = "新建的在 ${矩形区域} 的天气效果: ${WeatherId}"
        /// comment = "会创建天气效果."

        public static JWeatherEffect AddWeatherEffect(JRect where, int effectID)
        {
            return War3.CallNative<JWeatherEffect>(War3.GetNativeFunction("AddWeatherEffect"), where.Handle, effectID);
        }

        public static void RemoveWeatherEffect(JWeatherEffect whichEffect)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveWeatherEffect"), whichEffect.Handle);
        }


        /// title = "启用/禁用 天气效果"
        /// description = "设置 ${Weather Effect} 的状态为: ${On/Off}"
        /// comment = "可以使用'环境 - 创建天气效果'动作来创建天气效果."

        public static void EnableWeatherEffect(JWeatherEffect whichEffect, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("EnableWeatherEffect"), whichEffect.Handle, enable);
        }


        /// title = "新建地形变化:弹坑 [R]"
        /// description = "新建的弹坑变形. 中心坐标:(${X},${Y}) 半径: ${Radius} 深度: ${Depth} 持续时间: ${Duration} 毫秒, 变化类型: ${Type}"
        /// comment = "深度可取负数. 永久地形变化在保存游戏时不会被记录."

        public static JTerrainDeformation TerrainDeformCrater(float x, float y, float radius, float depth, int duration, bool permanent)
        {
            return War3.CallNative<JTerrainDeformation>(War3.GetNativeFunction("TerrainDeformCrater"), x, y, radius, depth, duration, permanent);
        }


        /// title = "新建地形变化:波纹 [R]"
        /// description = "新建的波纹变形. 中心坐标:(${X},${Y}) 最终半径: ${Radius} 深度: ${Depth} 持续时间: ${Duration} 毫秒, 变化次数: ${Count} 面波数: ${SpaceWave} 总波数: ${TimeWave} 初始半径率: ${数值} 变化类型: ${Type}"
        /// comment = "初始半径率=初始半径/最终半径."

        public static JTerrainDeformation TerrainDeformRipple(float x, float y, float radius, float depth, int duration, int count, float spaceWaves, float timeWaves, float radiusStartPct, bool limitNeg)
        {
            return War3.CallNative<JTerrainDeformation>(War3.GetNativeFunction("TerrainDeformRipple"), x, y, radius, depth, duration, count, spaceWaves, timeWaves, radiusStartPct, limitNeg);
        }


        /// title = "新建地形变化:冲击波 [R]"
        /// description = "新建的冲击波变形. 起始坐标:(${X},${Y}) 波方向:(${X},${Y}) 波距离: ${distance} 波速度: ${speed} 波半径: ${radius} 深度: ${Depth} 变形效果持续时间: ${Delay} 毫秒, 变化次数: ${Count}"
        /// comment = "深度可取负数. 方向以(X,Y)坐标形式表示,如(1,1)表示45度."

        public static JTerrainDeformation TerrainDeformWave(float x, float y, float dirX, float dirY, float distance, float speed, float radius, float depth, int trailTime, int count)
        {
            return War3.CallNative<JTerrainDeformation>(War3.GetNativeFunction("TerrainDeformWave"), x, y, dirX, dirY, distance, speed, radius, depth, trailTime, count);
        }


        /// title = "新建地形变化:随机 [R]"
        /// description = "新建的随机变形. 中心坐标:(${X},${Y}) 半径: ${Radius} 最小高度变化: ${Depth} 最大高度变化: ${Depth} 持续时间: ${Duration} 毫秒, 变化周期: ${Duration} 毫秒"
        /// comment = ""

        public static JTerrainDeformation TerrainDeformRandom(float x, float y, float radius, float minDelta, float maxDelta, int duration, int updateInterval)
        {
            return War3.CallNative<JTerrainDeformation>(War3.GetNativeFunction("TerrainDeformRandom"), x, y, radius, minDelta, maxDelta, duration, updateInterval);
        }


        /// title = "停止地形变化 [R]"
        /// description = "停止 ${Terrain Deformation} ,衰退时间: ${Duration} 毫秒"
        /// comment = "地形变化会平滑地过渡到无."

        public static void TerrainDeformStop(JTerrainDeformation deformation, int duration)
        {
            War3.CallNative(War3.GetNativeFunction("TerrainDeformStop"), deformation.Handle, duration);
        }


        /// title = "停止所有地形变化"
        /// description = "停止所有地形变化"
        /// comment = "包括由技能引起的地形变化."

        public static void TerrainDeformStopAll()
        {
            War3.CallNative(War3.GetNativeFunction("TerrainDeformStopAll"));
        }


        /// title = "新建特效(创建到坐标) [R]"
        /// description = "新建特效 ${Model File} 在(${X},${Y})处"
        /// comment = "会创建特效."

        public static JEffect AddSpecialEffect(string modelName, float x, float y)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpecialEffect"), modelName, x, y);
        }


        /// title = "新建特效(创建到点) [R]"
        /// description = "新建特效 ${Model File} 在 ${指定点} 处"
        /// comment = "会创建特效."

        public static JEffect AddSpecialEffectLoc(string modelName, JLocation where)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpecialEffectLoc"), modelName, where.Handle);
        }


        /// title = "新建特效(创建到单位) [R]"
        /// description = "新建特效 ${Model File} 并绑定到 ${单位} 的 ${Attachment Point} 附加点上"
        /// comment = "会创建特效."

        public static JEffect AddSpecialEffectTarget(string modelName, JWidget targetWidget, string attachPointName)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpecialEffectTarget"), modelName, targetWidget.Handle, attachPointName);
        }

        public static void DestroyEffect(JEffect whichEffect)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyEffect"), whichEffect.Handle);
        }

        public static JEffect AddSpellEffect(string abilityString, JEffectType t, float x, float y)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffect"), abilityString, t.Handle, x, y);
        }

        public static JEffect AddSpellEffectLoc(string abilityString, JEffectType t, JLocation where)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectLoc"), abilityString, t.Handle, where.Handle);
        }


        /// title = "新建特效(指定技能，创建到坐标) [R]"
        /// description = "${技能} 的 ${EffectType} , 创建到坐标(${X},${Y})"
        /// comment = "会创建特效."

        public static JEffect AddSpellEffectById(int abilityId, JEffectType t, float x, float y)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectById"), abilityId, t.Handle, x, y);
        }


        /// title = "新建特效(指定技能，创建到点) [R]"
        /// description = "${技能} 的 ${EffectType} , 创建到 ${指定点}"
        /// comment = "会创建特效."

        public static JEffect AddSpellEffectByIdLoc(int abilityId, JEffectType t, JLocation where)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectByIdLoc"), abilityId, t.Handle, where.Handle);
        }

        public static JEffect AddSpellEffectTarget(string modelName, JEffectType t, JWidget targetWidget, string attachPoint)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectTarget"), modelName, t.Handle, targetWidget.Handle, attachPoint);
        }


        /// title = "新建特效(指定技能，创建到单位) [R]"
        /// description = "${技能} 的 ${EffectType} , 绑定到 ${单位} 的 ${String} 附加点"
        /// comment = "会创建特效."

        public static JEffect AddSpellEffectTargetById(int abilityId, JEffectType t, JWidget targetWidget, string attachPoint)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectTargetById"), abilityId, t.Handle, targetWidget.Handle, attachPoint);
        }


        /// title = "新建闪电效果 [R]"
        /// description = "新建闪电效果: ${闪电效果} (${Boolean}检查可见性) 起始点:(${X},${Y}) 终结点:(${X},${Y})"
        /// comment = "会创建闪电效果. 允许检查可见性则在起始点和终结点都不可见时将不创建闪电效果."

        public static JLightning AddLightning(string codeName, bool checkVisibility, float x1, float y1, float x2, float y2)
        {
            return War3.CallNative<JLightning>(War3.GetNativeFunction("AddLightning"), codeName, checkVisibility, x1, y1, x2, y2);
        }


        /// title = "新建闪电效果(指定Z轴) [R]"
        /// description = "新建闪电效果: ${闪电效果} (${Boolean}检查可见性) 起始点:(${X},${Y},${Z}) 终结点:(${X},${Y},${Z})"
        /// comment = "会创建闪电效果. 允许检查可见性则在起始点和终结点都不可见时将不创建闪电效果."

        public static JLightning AddLightningEx(string codeName, bool checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return War3.CallNative<JLightning>(War3.GetNativeFunction("AddLightningEx"), codeName, checkVisibility, x1, y1, z1, x2, y2, z2);
        }

        public static bool DestroyLightning(JLightning whichBolt)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DestroyLightning"), whichBolt.Handle);
        }

        public static bool MoveLightning(JLightning whichBolt, bool checkVisibility, float x1, float y1, float x2, float y2)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("MoveLightning"), whichBolt.Handle, checkVisibility, x1, y1, x2, y2);
        }


        /// title = "移动闪电效果(指定坐标) [R]"
        /// description = "移动 ${Lightning} 到新位置,(${Boolean} 检查可见性) 新起始点: (${X},${Y},${Z}) 新终结点: (${X},${Y},${Z})"
        /// comment = "可指定Z坐标. 允许检查可见性则在指定起始点和终结点都不可见时将不移动闪电效果."

        public static bool MoveLightningEx(JLightning whichBolt, bool checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("MoveLightningEx"), whichBolt.Handle, checkVisibility, x1, y1, z1, x2, y2, z2);
        }

        public static float GetLightningColorA(JLightning whichBolt)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLightningColorA"), whichBolt.Handle);
        }

        public static float GetLightningColorR(JLightning whichBolt)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLightningColorR"), whichBolt.Handle);
        }

        public static float GetLightningColorG(JLightning whichBolt)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLightningColorG"), whichBolt.Handle);
        }

        public static float GetLightningColorB(JLightning whichBolt)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLightningColorB"), whichBolt.Handle);
        }

        public static bool SetLightningColor(JLightning whichBolt, float r, float g, float b, float a)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SetLightningColor"), whichBolt.Handle, r, g, b, a);
        }

        public static string GetAbilityEffect(string abilityString, JEffectType t, int index)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetAbilityEffect"), abilityString, t.Handle, index);
        }

        public static string GetAbilityEffectById(int abilityId, JEffectType t, int index)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetAbilityEffectById"), abilityId, t.Handle, index);
        }

        public static string GetAbilitySound(string abilityString, JSoundType t)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetAbilitySound"), abilityString, t.Handle);
        }

        public static string GetAbilitySoundById(int abilityId, JSoundType t)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetAbilitySoundById"), abilityId, t.Handle);
        }


        /// title = "地形悬崖高度(指定坐标) [R]"
        /// description = "坐标(${X},${Y})处的地形悬崖高度"
        /// comment = "悬崖高度:深水区为0, 浅水区为1, 平原为2, 之后每层+1."

        public static int GetTerrainCliffLevel(float x, float y)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTerrainCliffLevel"), x, y);
        }


        /// title = "设置水颜色 [R]"
        /// description = "设置水颜色为:(${Red},${Green},${Blue}), 透明值为: ${Transparency}"
        /// comment = "颜色格式为(红,绿,蓝). 透明值0为不可见. 颜色值和透明道值取值范围为0-255."

        public static void SetWaterBaseColor(int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetWaterBaseColor"), red, green, blue, alpha);
        }


        /// title = "开启/关闭 水面变形"
        /// description = "${On/Off} 水面变形"
        /// comment = "开启时当发生地形变化时水面高度也会随着变化. 对永久变形无效."

        public static void SetWaterDeforms(bool val)
        {
            War3.CallNative(War3.GetNativeFunction("SetWaterDeforms"), val);
        }


        /// title = "指定坐标地形 [R]"
        /// description = "坐标(${X},${Y})处的地形"
        /// comment = ""

        public static int GetTerrainType(float x, float y)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTerrainType"), x, y);
        }


        /// title = "地形样式(指定坐标) [R]"
        /// description = "坐标(${X},${Y})处的地形样式"
        /// comment = ""

        public static int GetTerrainVariance(float x, float y)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTerrainVariance"), x, y);
        }


        /// title = "改变地形类型(指定坐标) [R]"
        /// description = "改变(${X},${Y})处的地形为 ${Terrain Type} ,使用样式: ${Variation} 范围: ${Area} 形状: ${Shape}"
        /// comment = "地形样式-1表示随机样式. 范围即地形编辑器中的刷子大小.1表示128x128范围"

        public static void SetTerrainType(float x, float y, int terrainType, int variation, int area, int shape)
        {
            War3.CallNative(War3.GetNativeFunction("SetTerrainType"), x, y, terrainType, variation, area, shape);
        }


        /// title = "地形通行状态关闭(指定坐标) [R]"
        /// description = "坐标(${X},${Y})处的 ${Pathing Type} 通行状态为关闭"
        /// comment = "指定类型单位不能通行即通行状态为关闭. 如该点不能造建筑就是'建造'通行状态为关闭. 可使用'环境 - 设置地形通行状态'来改变通行状态."

        public static bool IsTerrainPathable(float x, float y, JPathingType t)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsTerrainPathable"), x, y, t.Handle);
        }


        /// title = "设置地形通行状态(指定坐标) [R]"
        /// description = "设置(${X},${Y})处单元点的 ${Pathing} 地形通行状态为: ${On/Off}"
        /// comment = "例:设置'建造'通行状态为开,则该点可以建造建筑. 一个单元点范围为32x32."

        public static void SetTerrainPathable(float x, float y, JPathingType t, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetTerrainPathable"), x, y, t.Handle, flag);
        }


        /// title = "新建图像 [R]"
        /// description = "新建图像: ${Image} 大小:(${X},${Y},${Z}) 创建点:(${X},${Y},${Z}) 原点坐标:(${X},${Y},${Z}) 图像类型: ${Type}"
        /// comment = "使用'图像 - 设置永久渲染状态'动作才能显示图像. 大小、创建点和原点格式为(X,Y,Z). 创建点作为图像的左下角位置. 会创建图像."

        public static JImage CreateImage(string file, float sizeX, float sizeY, float sizeZ, float posX, float posY, float posZ, float originX, float originY, float originZ, int imageType)
        {
            return War3.CallNative<JImage>(War3.GetNativeFunction("CreateImage"), file, sizeX, sizeY, sizeZ, posX, posY, posZ, originX, originY, originZ, imageType);
        }


        /// title = "删除"
        /// description = "删除 ${图像}"
        /// comment = ""

        public static void DestroyImage(JImage whichImage)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyImage"), whichImage.Handle);
        }


        /// title = "显示/隐藏 [R]"
        /// description = "设置 ${Image} ${Show/Hide}"
        /// comment = ""

        public static void ShowImage(JImage whichImage, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("ShowImage"), whichImage.Handle, flag);
        }


        /// title = "设置图像高度"
        /// description = "设置 ${Image} ${Enable/Disable} Z轴显示,并设置高度为 ${Height}"
        /// comment = "实际显示高度为图像高度+Z轴偏移. 只有允许Z轴显示时才有效."

        public static void SetImageConstantHeight(JImage whichImage, bool flag, float height)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageConstantHeight"), whichImage.Handle, flag, height);
        }


        /// title = "改变图像位置(指定坐标) [R]"
        /// description = "改变 ${Image} 的位置为(${X},${Y}),Z轴偏移为 ${Z}"
        /// comment = "指图像的左下角位置."

        public static void SetImagePosition(JImage whichImage, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("SetImagePosition"), whichImage.Handle, x, y, z);
        }


        /// title = "改变图像颜色 [R]"
        /// description = "设置 ${Image} 的颜色值为(${Red},${Green},${Blue}) Alpha值为 ${Alpha}"
        /// comment = "颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围0-255."

        public static void SetImageColor(JImage whichImage, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageColor"), whichImage.Handle, red, green, blue, alpha);
        }


        /// title = "设置图像渲染状态"
        /// description = "设置 ${Image} : ${Enable/Disable} 显示状态"
        /// comment = "未发现有任何作用."

        public static void SetImageRender(JImage whichImage, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageRender"), whichImage.Handle, flag);
        }


        /// title = "设置图像永久渲染状态"
        /// description = "设置 ${Image} : ${Enable/Disable} 永久显示状态"
        /// comment = "要显示图像则必须开启该项."

        public static void SetImageRenderAlways(JImage whichImage, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageRenderAlways"), whichImage.Handle, flag);
        }


        /// title = "图像水面显示状态"
        /// description = "设置 ${Image} : ${Enable/Disable} 水面显示, ${Enable/Disable} 水的Alpha通道"
        /// comment = "前者设置图像在水面或是水底显示. 后者设置图像是否受水的Alpha通道影响. "

        public static void SetImageAboveWater(JImage whichImage, bool flag, bool useWaterAlpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageAboveWater"), whichImage.Handle, flag, useWaterAlpha);
        }


        /// title = "改变图像类型"
        /// description = "改变 ${Image} 类型为 ${Type}"
        /// comment = ""

        public static void SetImageType(JImage whichImage, int imageType)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageType"), whichImage.Handle, imageType);
        }


        /// title = "新建地面纹理变化 [R]"
        /// description = "新建的地面纹理变化在点(${X},${Y}),使用图像: ${Type} 颜色值为(${Red},${Green},${Blue}) Alpha值为${Transparency} (${Enable/Disable} 暂停状态, ${Enble/Disable} 跳过出生动画)"
        /// comment = "颜色值和Alpha值取值范围0-255. 使用'地面纹理变化 - 设置永久渲染状态' 来显示创建的纹理变化. 暂停状态表示动画播放完毕后是否继续保留该纹理变化. 会创建纹理变化."

        public static JUbersplat CreateUbersplat(float x, float y, string name, int red, int green, int blue, int alpha, bool forcePaused, bool noBirthTime)
        {
            return War3.CallNative<JUbersplat>(War3.GetNativeFunction("CreateUbersplat"), x, y, name, red, green, blue, alpha, forcePaused, noBirthTime);
        }


        /// title = "删除地面纹理变化"
        /// description = "删除 ${Ubersplat}"
        /// comment = ""

        public static void DestroyUbersplat(JUbersplat whichSplat)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyUbersplat"), whichSplat.Handle);
        }


        /// title = "重置地面纹理变化"
        /// description = "重置 ${Ubersplat}"
        /// comment = ""

        public static void ResetUbersplat(JUbersplat whichSplat)
        {
            War3.CallNative(War3.GetNativeFunction("ResetUbersplat"), whichSplat.Handle);
        }


        /// title = "结束地面纹理变化"
        /// description = "结束 ${Ubersplat}"
        /// comment = "在动画播放完毕时自动清除该地面纹理变化."

        public static void FinishUbersplat(JUbersplat whichSplat)
        {
            War3.CallNative(War3.GetNativeFunction("FinishUbersplat"), whichSplat.Handle);
        }


        /// title = "显示/隐藏 地面纹理变化[R]"
        /// description = "设置 ${Ubersplat} 状态为 ${Show/Hide}"
        /// comment = ""

        public static void ShowUbersplat(JUbersplat whichSplat, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("ShowUbersplat"), whichSplat.Handle, flag);
        }


        /// title = "设置渲染状态"
        /// description = "设置 ${Ubersplat} : ${Enable/Disable} 渲染状态"
        /// comment = "未发现有任何作用."

        public static void SetUbersplatRender(JUbersplat whichSplat, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUbersplatRender"), whichSplat.Handle, flag);
        }


        /// title = "设置永久渲染状态"
        /// description = "设置 ${Ubersplat} : ${Enable/Disable} 永久渲染状态"
        /// comment = "要显示地面纹理变化则必须开启该项."

        public static void SetUbersplatRenderAlways(JUbersplat whichSplat, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUbersplatRenderAlways"), whichSplat.Handle, flag);
        }


        /// title = "创建/删除荒芜地表(圆范围)(指定坐标) [R]"
        /// description = "为 ${Player} 在圆心为(${X},${Y}),半径为 ${R} 的圆范围内 ${Create/Remove} 一块荒芜地表"
        /// comment = ""

        public static void SetBlight(JPlayer whichPlayer, float x, float y, float radius, bool addBlight)
        {
            War3.CallNative(War3.GetNativeFunction("SetBlight"), whichPlayer.Handle, x, y, radius, addBlight);
        }


        /// title = "创建/删除荒芜地表(矩形区域) [R]"
        /// description = "为 ${Player} 在 ${Region} ${Create/Remove} 一块荒芜地表"
        /// comment = ""

        public static void SetBlightRect(JPlayer whichPlayer, JRect r, bool addBlight)
        {
            War3.CallNative(War3.GetNativeFunction("SetBlightRect"), whichPlayer.Handle, r.Handle, addBlight);
        }

        public static void SetBlightPoint(JPlayer whichPlayer, float x, float y, bool addBlight)
        {
            War3.CallNative(War3.GetNativeFunction("SetBlightPoint"), whichPlayer.Handle, x, y, addBlight);
        }

        public static void SetBlightLoc(JPlayer whichPlayer, JLocation whichLocation, float radius, bool addBlight)
        {
            War3.CallNative(War3.GetNativeFunction("SetBlightLoc"), whichPlayer.Handle, whichLocation.Handle, radius, addBlight);
        }


        /// title = "新建不死族金矿 [R]"
        /// description = "新建 ${玩家} 的不死族金矿在(${X},${Y}),面向角度:${Face} 度"
        /// comment = ""

        public static JUnit CreateBlightedGoldmine(JPlayer id, float x, float y, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateBlightedGoldmine"), id.Handle, x, y, face);
        }


        /// title = "坐标点被荒芜地表覆盖 [R]"
        /// description = "坐标点(${X},${Y})被荒芜地表覆盖"
        /// comment = ""

        public static bool IsPointBlighted(float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPointBlighted"), x, y);
        }


        /// title = "播放圆范围内地形装饰物动画 [R]"
        /// description = "选取圆心为(${X},${Y}),半径为 ${半径} 的圆范围内的 ${装饰物类型}(选取方式:${选取方式}), 做 ${Animation Name} 动作(${允许/禁止} 随机播放)"
        /// comment = "特殊动画名: 'show', 'hide', 'soundon', 'soundoff'. 随机播放:比如某装饰物有好几个'stand'动作,则允许该项时会随机抽取某个动作播放,而禁止该项时只播放首个动作."

        public static void SetDoodadAnimation(float x, float y, float radius, int doodadID, bool nearestOnly, string animName, bool animRandom)
        {
            War3.CallNative(War3.GetNativeFunction("SetDoodadAnimation"), x, y, radius, doodadID, nearestOnly, animName, animRandom);
        }


        /// title = "播放矩形区域内地形装饰物动画 [R]"
        /// description = "播放 ${Rect} 内所有 ${装饰物类型} 的 ${Animation Name} 动作(${允许/禁止} 随机播放)"
        /// comment = "特殊动画名: 'show', 'hide', 'soundon', 'soundoff'. 随机播放:比如某装饰物有好几个'stand'动作,则允许该项时会随机抽取某个动作播放,而禁止该项时只播放首个动作."

        public static void SetDoodadAnimationRect(JRect r, int doodadID, string animName, bool animRandom)
        {
            War3.CallNative(War3.GetNativeFunction("SetDoodadAnimationRect"), r.Handle, doodadID, animName, animRandom);
        }


        /// title = "启用对战AI"
        /// description = "为 ${Player} 启用对战AI: ${Script}"
        /// comment = "AI只能对电脑玩家使用,当运行该动作后,与之配匹的电脑玩家会强制执行该AI脚本."

        public static void StartMeleeAI(JPlayer num, string script)
        {
            War3.CallNative(War3.GetNativeFunction("StartMeleeAI"), num.Handle, script);
        }


        /// title = "启用战役AI"
        /// description = "为 ${Player} 启用战役AI: ${Script}"
        /// comment = "AI只能对电脑玩家使用,当运行该动作后,与之配匹的电脑玩家会强制执行该AI脚本."

        public static void StartCampaignAI(JPlayer num, string script)
        {
            War3.CallNative(War3.GetNativeFunction("StartCampaignAI"), num.Handle, script);
        }


        /// title = "发送AI命令"
        /// description = "对 ${Player} 发送AI命令:(${命令}, ${数据})"
        /// comment = "发送的AI命令将被AI脚本所使用."

        public static void CommandAI(JPlayer num, int command, int data)
        {
            War3.CallNative(War3.GetNativeFunction("CommandAI"), num.Handle, command, data);
        }


        /// title = "暂停/恢复 AI脚本运行 [R]"
        /// description = "设定 ${Player} ${暂停/恢复} 当前AI脚本的运行"
        /// comment = "事实上该函数是有问题的,可以这么理解:设玩家当前AI脚本的运行状态R为0,暂停1次则R+1,恢复1次则R-1,仅当R=0时该玩家才会运行AI. 在使用前请先理解这段话的意思."

        public static void PauseCompAI(JPlayer p, bool pause)
        {
            War3.CallNative(War3.GetNativeFunction("PauseCompAI"), p.Handle, pause);
        }


        /// title = "玩家的AI难度"
        /// description = "${Player} 的对战AI难度"
        /// comment = "对非AI玩家返回普通难度."

        public static JAiDifficulty GetAIDifficulty(JPlayer num)
        {
            return War3.CallNative<JAiDifficulty>(War3.GetNativeFunction("GetAIDifficulty"), num.Handle);
        }


        /// title = "忽视指定单位的警戒点"
        /// description = "忽视 ${单位} 的警戒点"
        /// comment = "单位将不会自动返回原警戒点. 一个很有用的功能就是刷怪进攻时忽视单位警戒范围的话,怪就不会想家了."

        public static void RemoveGuardPosition(JUnit hUnit)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveGuardPosition"), hUnit.Handle);
        }


        /// title = "恢复指定单位的警戒点"
        /// description = "恢复 ${单位} 的警戒点"
        /// comment = "这个动作通过 AI 来恢复特定单位的警戒点."

        public static void RecycleGuardPosition(JUnit hUnit)
        {
            War3.CallNative(War3.GetNativeFunction("RecycleGuardPosition"), hUnit.Handle);
        }


        /// title = "忽视所有单位的警戒点"
        /// description = "忽视 ${Player} 的所有单位的警戒点"
        /// comment = "单位将不会自动返回原警戒点. 一个很有用的功能就是刷怪进攻时忽视单位警戒范围的话,怪就不会想家了."

        public static void RemoveAllGuardPositions(JPlayer num)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveAllGuardPositions"), num.Handle);
        }


        /// title = "输入作弊码 [R]"
        /// description = "输入作弊码: ${String}"
        /// comment = "作弊码只在单机有效."

        public static void Cheat(string cheatStr)
        {
            War3.CallNative(War3.GetNativeFunction("Cheat"), cheatStr);
        }


        /// title = "无法胜利 [R]"
        /// description = "玩家开启作弊模式: 无法胜利"
        /// comment = "单机作弊码开启的模式."

        public static bool IsNoVictoryCheat()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsNoVictoryCheat"));
        }


        /// title = "无法失败 [R]"
        /// description = "玩家开启作弊模式: 无法失败"
        /// comment = "单机作弊码开启的模式."

        public static bool IsNoDefeatCheat()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsNoDefeatCheat"));
        }


        /// title = "预载文件"
        /// description = "预载 ${文件}"
        /// comment = "可以事先载入文件并调入到游戏内存,以加快游戏的速度."

        public static void Preload(string filename)
        {
            War3.CallNative(War3.GetNativeFunction("Preload"), filename);
        }


        /// title = "开始预载"
        /// description = "开始预载, 超时设置 ${Time} 秒"
        /// comment = "将文件调入到游戏内存中."

        public static void PreloadEnd(float timeout)
        {
            War3.CallNative(War3.GetNativeFunction("PreloadEnd"), timeout);
        }

        public static void PreloadStart()
        {
            War3.CallNative(War3.GetNativeFunction("PreloadStart"));
        }

        public static void PreloadRefresh()
        {
            War3.CallNative(War3.GetNativeFunction("PreloadRefresh"));
        }

        public static void PreloadEndEx()
        {
            War3.CallNative(War3.GetNativeFunction("PreloadEndEx"));
        }

        public static void PreloadGenClear()
        {
            War3.CallNative(War3.GetNativeFunction("PreloadGenClear"));
        }

        public static void PreloadGenStart()
        {
            War3.CallNative(War3.GetNativeFunction("PreloadGenStart"));
        }

        public static void PreloadGenEnd(string filename)
        {
            War3.CallNative(War3.GetNativeFunction("PreloadGenEnd"), filename);
        }


        /// title = "批量预载"
        /// description = "预载所有在 ${文件} 中列出的文件"
        /// comment = ""

        public static void Preloader(string filename)
        {
            War3.CallNative(War3.GetNativeFunction("Preloader"), filename);
        }

    }

}
