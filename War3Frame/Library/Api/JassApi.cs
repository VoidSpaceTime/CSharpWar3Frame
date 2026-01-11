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

        // title = "单位所在X轴坐标 [R]"

        public static float GetUnitX(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitX"), whichUnit);
        }
        // title = "单位所在Y轴坐标 [R]"
        public static float GetUnitY(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitY"), whichUnit);
        }
        // title = "单位位置"
        public static JLocation GetUnitLoc(JUnit whichUnit)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("GetUnitLoc"), whichUnit);
        }
        // title = "面向角度"
        public static float GetUnitFacing(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitFacing"), whichUnit);
        }
        // title = "当前移动速度"
        public static float GetUnitMoveSpeed(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitMoveSpeed"), whichUnit);
        }
        // title = "默认移动速度"
        public static float GetUnitDefaultMoveSpeed(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultMoveSpeed"), whichUnit);
        }
        // title = "属性 [R]"
        public static float GetUnitState(JUnit whichUnit, JUnitState whichUnitState)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitState"), whichUnit, whichUnitState);
        }
        // title = "单位所有者"
        public static JPlayer GetOwningPlayer(JUnit whichUnit)
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetOwningPlayer"), whichUnit);
        }
        // title = "指定单位的类型"
        public static int GetUnitTypeId(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitTypeId"), whichUnit);
        }
        // title = "单位种族"
        public static JRace GetUnitRace(JUnit whichUnit)
        {
            return War3.CallNative<JRace>(War3.GetNativeFunction("GetUnitRace"), whichUnit);
        }
        // title = "单位名字"
        public static string GetUnitName(JUnit whichUnit)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetUnitName"), whichUnit);
        }
        // title = "单位使用人口数量"
        public static int GetUnitFoodUsed(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitFoodUsed"), whichUnit);
        }
        // title = "单位提供人口数量"
        public static int GetUnitFoodMade(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitFoodMade"), whichUnit);
        }
        // title = "单位提供人口数量(指定单位类型)"
        public static int GetFoodMade(int unitId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("UnitId2String"), unitId);
        }
        // title = "单位使用人口数量(指定单位类型)"
        public static int GetFoodUsed(int unitId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("UnitId2String"), unitId);
        }


        // title = "转换角度为弧度"        
        public static float Deg2Rad(float degrees)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Deg2Rad"), degrees);
        }

        // title = "转换弧度为角度"
        public static float Rad2Deg(float radians)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Rad2Deg"), radians);
        }

        // title = "正弦(弧度) [R]"
        public static float Sin(float radians)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Sin"), radians);
        }

        // title = "余弦(弧度) [R]"
        public static float Cos(float radians)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Cos"), radians);
        }

        // title = "正切(弧度) [R]"
        public static float Tan(float radians)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Tan"), radians);
        }

        // title = "反正弦(弧度) [R]"
        public static float Asin(float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Asin"), y);
        }

        // title = "反余弦(弧度) [R]"
        public static float Acos(float x)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Acos"), x);
        }

        // title = "反正切(弧度) [R]"
        public static float Atan(float x)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Atan"), x);
        }

        // title = "反正切(Y:X)(弧度) [R]"
        public static float Atan2(float y, float x)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Atan2"), y, x);
        }

        // title = "平方根"
        public static float SquareRoot(float x)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("SquareRoot"), x);
        }

        // title = "幂运算"
        public static float Pow(float x, float power)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("Pow"), x, power);
        }

        // title = "转换整数为实数"
        public static float I2R(int i)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("I2R"), i);
        }

        // title = "转换实数为整数"
        public static int R2I(float r)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("R2I"), r);
        }

        // title = "转换整数为字符串"
        public static string I2S(int i)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("I2S"), i);
        }

        // title = "转换实数为字符串"
        public static string R2S(float r)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("R2S"), r);
        }

        // title = "格式转换实数为字符串"
        public static string R2SW(float r, int width, int precision)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("R2SW"), r, width, precision);
        }

        // title = "转换字符串为整数"
        public static int S2I(string s)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("S2I"), s);
        }

        // title = "转换字符串为实数"
        public static float S2R(string s)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("S2R"), s);
        }

        // title = "<1.24> 获取对象的h2i值 [C]"
        public static int GetHandleId(JHandle h)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHandleId"), h);
        }

        // title = "截取字符串 [R]"
        public static string SubString(string source, int start, int end)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("SubString"), source, start, end);
        }

        // title = "字符串长度"
        public static int StringLength(string s)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("StringLength"), s);
        }

        // title = "大小写转换"
        public static string StringCase(string source, bool upper)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("StringCase"), source, upper);
        }

        // title = "获取字符串的哈希值"
        public static int StringHash(string s)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("StringHash"), s);
        }

        // title = "本地字符串 [R]"
        public static string GetLocalizedString(string source)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetLocalizedString"), source);
        }

        // title = "本地热键 "
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
            War3.CallNative(War3.GetNativeFunction("DefineStartLocationLoc"), whichStartLoc, whichLocation);
        }

        public static void SetStartLocPrioCount(int whichStartLoc, int prioSlotCount)
        {
            War3.CallNative(War3.GetNativeFunction("SetStartLocPrioCount"), whichStartLoc, prioSlotCount);
        }

        public static void SetStartLocPrio(int whichStartLoc, int prioSlotIndex, int otherStartLocIndex, JStartLocPrio priority)
        {
            War3.CallNative(War3.GetNativeFunction("SetStartLocPrio"), whichStartLoc, prioSlotIndex, otherStartLocIndex, priority);
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
            War3.CallNative(War3.GetNativeFunction("SetGameTypeSupported"), whichGameType, value);
        }

        // title = "设置地图参数"
        public static void SetMapFlag(JMapFlag whichMapFlag, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("SetMapFlag"), whichMapFlag, value);
        }

        public static void SetGamePlacement(JPlacement whichPlacementType)
        {
            War3.CallNative(War3.GetNativeFunction("SetGamePlacement"), whichPlacementType);
        }

        // title = "设定游戏速度"
        public static void SetGameSpeed(JGameSpeed whichspeed)
        {
            War3.CallNative(War3.GetNativeFunction("SetGameSpeed"), whichspeed);
        }

        // title = "设置游戏难度 [R]"
        public static void SetGameDifficulty(JGameDifficulty whichdifficulty)
        {
            War3.CallNative(War3.GetNativeFunction("SetGameDifficulty"), whichdifficulty);
        }

        public static void SetResourceDensity(JMapDensity whichdensity)
        {
            War3.CallNative(War3.GetNativeFunction("SetResourceDensity"), whichdensity);
        }

        public static void SetCreatureDensity(JMapDensity whichdensity)
        {
            War3.CallNative(War3.GetNativeFunction("SetCreatureDensity"), whichdensity);
        }

        // title = "队伍数量"
        public static int GetTeams()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTeams"));
        }

        // title = "玩家数量"
        public static int GetPlayers()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayers"));
        }

        public static bool IsGameTypeSupported(JGameType whichGameType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsGameTypeSupported"), whichGameType);
        }

        public static JGameType GetGameTypeSelected()
        {
            return War3.CallNative<JGameType>(War3.GetNativeFunction("GetGameTypeSelected"));
        }

        // title = "地图参数设置"
        public static bool IsMapFlagSet(JMapFlag whichMapFlag)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsMapFlagSet"), whichMapFlag);
        }

        // title = "设置玩家队伍"
        public static void SetPlayerTeam(JPlayer whichPlayer, int whichTeam)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerTeam"), whichPlayer, whichTeam);
        }

        public static void SetPlayerStartLocation(JPlayer whichPlayer, int startLocIndex)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerStartLocation"), whichPlayer, startLocIndex);
        }

        public static void ForcePlayerStartLocation(JPlayer whichPlayer, int startLocIndex)
        {
            War3.CallNative(War3.GetNativeFunction("ForcePlayerStartLocation"), whichPlayer, startLocIndex);
        }

        // title = "改变玩家颜色 [R]"
        public static void SetPlayerColor(JPlayer whichPlayer, JPlayerColor color)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerColor"), whichPlayer, color);
        }

        // title = "设置联盟状态(指定项目) [R]"
        public static void SetPlayerAlliance(JPlayer sourcePlayer, JPlayer otherPlayer, JAllianceType whichAllianceSetting, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerAlliance"), sourcePlayer, otherPlayer, whichAllianceSetting, value);
        }

        // title = "设置税率 [R]"
        public static void SetPlayerTaxRate(JPlayer sourcePlayer, JPlayer otherPlayer, JPlayerState whichResource, int rate)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerTaxRate"), sourcePlayer, otherPlayer, whichResource, rate);
        }

        public static void SetPlayerRacePreference(JPlayer whichPlayer, JRacePreference whichRacePreference)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerRacePreference"), whichPlayer, whichRacePreference);
        }

        public static void SetPlayerRaceSelectable(JPlayer whichPlayer, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerRaceSelectable"), whichPlayer, value);
        }

        public static void SetPlayerController(JPlayer whichPlayer, JMapControl controlType)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerController"), whichPlayer, controlType);
        }

        // title = "更改名字"
        public static void SetPlayerName(JPlayer whichPlayer, string name)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerName"), whichPlayer, name);
        }

        // title = "显示/隐藏计分屏显示 [R]"
        public static void SetPlayerOnScoreScreen(JPlayer whichPlayer, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerOnScoreScreen"), whichPlayer, flag);
        }

        // title = "玩家队伍"
        public static int GetPlayerTeam(JPlayer whichPlayer)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerTeam"), whichPlayer);
        }

        public static int GetPlayerStartLocation(JPlayer whichPlayer)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerStartLocation"), whichPlayer);
        }

        // title = "玩家颜色"
        public static JPlayerColor GetPlayerColor(JPlayer whichPlayer)
        {
            return War3.CallNative<JPlayerColor>(War3.GetNativeFunction("GetPlayerColor"), whichPlayer);
        }

        public static bool GetPlayerSelectable(JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetPlayerSelectable"), whichPlayer);
        }

        // title = "玩家控制者"
        public static JMapControl GetPlayerController(JPlayer whichPlayer)
        {
            return War3.CallNative<JMapControl>(War3.GetNativeFunction("GetPlayerController"), whichPlayer);
        }

        // title = "玩家游戏状态"
        public static JPlayerSlotState GetPlayerSlotState(JPlayer whichPlayer)
        {
            return War3.CallNative<JPlayerSlotState>(War3.GetNativeFunction("GetPlayerSlotState"), whichPlayer);
        }

        // title = "玩家税率 [R]"
        public static int GetPlayerTaxRate(JPlayer sourcePlayer, JPlayer otherPlayer, JPlayerState whichResource)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetPlayerTaxRate"), sourcePlayer, otherPlayer, whichResource);
        }

        // title = "玩家的种族选择"
        public static bool IsPlayerRacePrefSet(JPlayer whichPlayer, JRacePreference pref)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPlayerRacePrefSet"), whichPlayer, pref);
        }

        // title = "玩家名字"
        public static string GetPlayerName(JPlayer whichPlayer)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetPlayerName"), whichPlayer);
        }

        // title = "新建计时器 [R]"
        public static JTimer CreateTimer()
        {
            return War3.CallNative<JTimer>(War3.GetNativeFunction("CreateTimer"));
        }

        // title = "删除计时器 [R]"
        public static void DestroyTimer(JTimer whichTimer)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyTimer"), whichTimer);
        }

        // title = "运行计时器 [C]"
        public static void TimerStart(JTimer whichTimer, float timeout, bool periodic, JCode handlerFunc)
        {
            War3.CallNative(War3.GetNativeFunction("TimerStart"), whichTimer, timeout, periodic, handlerFunc);
        }

        // title = "逝去时间"
        public static float TimerGetElapsed(JTimer whichTimer)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("TimerGetElapsed"), whichTimer);
        }

        // title = "剩余时间"
        public static float TimerGetRemaining(JTimer whichTimer)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("TimerGetRemaining"), whichTimer);
        }

        // title = "设置时间"
        public static float TimerGetTimeout(JTimer whichTimer)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("TimerGetTimeout"), whichTimer);
        }

        // title = "暂停计时器 [R]"
        public static void PauseTimer(JTimer whichTimer)
        {
            War3.CallNative(War3.GetNativeFunction("PauseTimer"), whichTimer);
        }

        // title = "恢复计时器 [R]"
        public static void ResumeTimer(JTimer whichTimer)
        {
            War3.CallNative(War3.GetNativeFunction("ResumeTimer"), whichTimer);
        }

        // title = "到期的计时器"
        public static JTimer GetExpiredTimer()
        {
            return War3.CallNative<JTimer>(War3.GetNativeFunction("GetExpiredTimer"));
        }

        // title = "新建的单位组 [R]"
        public static JGroup CreateGroup()
        {
            return War3.CallNative<JGroup>(War3.GetNativeFunction("CreateGroup"));
        }

        // title = "删除单位组 [R]"
        public static void DestroyGroup(JGroup whichGroup)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyGroup"), whichGroup);
        }

        // title = "添加单位 [R]"
        public static void GroupAddUnit(JGroup whichGroup, JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupAddUnit"), whichGroup, whichUnit);
        }

        // title = "移除单位 [R]"
        public static void GroupRemoveUnit(JGroup whichGroup, JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupRemoveUnit"), whichGroup, whichUnit);
        }

        // title = "清空单位组"
        public static void GroupClear(JGroup whichGroup)
        {
            War3.CallNative(War3.GetNativeFunction("GroupClear"), whichGroup);
        }

        public static void GroupEnumUnitsOfType(JGroup whichGroup, string unitname, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsOfType"), whichGroup, unitname, filter);
        }

        public static void GroupEnumUnitsOfPlayer(JGroup whichGroup, JPlayer whichPlayer, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsOfPlayer"), whichGroup, whichPlayer, filter);
        }

        public static void GroupEnumUnitsOfTypeCounted(JGroup whichGroup, string unitname, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsOfTypeCounted"), whichGroup, unitname, filter, countLimit);
        }

        public static void GroupEnumUnitsInRect(JGroup whichGroup, JRect r, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRect"), whichGroup, r, filter);
        }

        public static void GroupEnumUnitsInRectCounted(JGroup whichGroup, JRect r, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRectCounted"), whichGroup, r, filter, countLimit);
        }

        // title = "选取单位添加到单位组(坐标)"
        public static void GroupEnumUnitsInRange(JGroup whichGroup, float x, float y, float radius, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRange"), whichGroup, x, y, radius, filter);
        }

        // title = "选取单位添加到单位组(点)"
        public static void GroupEnumUnitsInRangeOfLoc(JGroup whichGroup, JLocation whichLocation, float radius, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRangeOfLoc"), whichGroup, whichLocation, radius, filter);
        }

        // title = "选取单位添加到单位组(坐标)(不建议使用)"
        public static void GroupEnumUnitsInRangeCounted(JGroup whichGroup, float x, float y, float radius, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRangeCounted"), whichGroup, x, y, radius, filter, countLimit);
        }

        // title = "选取单位添加到单位组(点)(不建议使用)"
        public static void GroupEnumUnitsInRangeOfLocCounted(JGroup whichGroup, JLocation whichLocation, float radius, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsInRangeOfLocCounted"), whichGroup, whichLocation, radius, filter, countLimit);
        }

        public static void GroupEnumUnitsSelected(JGroup whichGroup, JPlayer whichPlayer, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("GroupEnumUnitsSelected"), whichGroup, whichPlayer, filter);
        }

        // title = "发布命令(无目标)"
        public static bool GroupImmediateOrder(JGroup whichGroup, string order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupImmediateOrder"), whichGroup, order);
        }

        // title = "发布命令(无目标)(ID)"
        public static bool GroupImmediateOrderById(JGroup whichGroup, int order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupImmediateOrderById"), whichGroup, order);
        }

        // title = "发布命令(指定坐标) [R]"
        public static bool GroupPointOrder(JGroup whichGroup, string order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupPointOrder"), whichGroup, order, x, y);
        }

        // title = "发布命令(指定点)"
        public static bool GroupPointOrderLoc(JGroup whichGroup, string order, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupPointOrderLoc"), whichGroup, order, whichLocation);
        }

        // title = "发布命令(指定坐标)(ID)"
        public static bool GroupPointOrderById(JGroup whichGroup, int order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupPointOrderById"), whichGroup, order, x, y);
        }

        // title = "发布命令(指定点)(ID)"
        public static bool GroupPointOrderByIdLoc(JGroup whichGroup, int order, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupPointOrderByIdLoc"), whichGroup, order, whichLocation);
        }

        // title = "发布命令(指定单位)"
        public static bool GroupTargetOrder(JGroup whichGroup, string order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupTargetOrder"), whichGroup, order, targetWidget);
        }

        // title = "发布命令(指定单位)(ID)"
        public static bool GroupTargetOrderById(JGroup whichGroup, int order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GroupTargetOrderById"), whichGroup, order, targetWidget);
        }

        // title = "选取单位组内单位做动作"
        public static void ForGroup(JGroup whichGroup, JCode callback)
        {
            War3.CallNative(War3.GetNativeFunction("ForGroup"), whichGroup, callback);
        }

        // title = "单位组中第一个单位"
        public static JUnit FirstOfGroup(JGroup whichGroup)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("FirstOfGroup"), whichGroup);
        }

        // title = "新建玩家组 [R]"
        public static JForce CreateForce()
        {
            return War3.CallNative<JForce>(War3.GetNativeFunction("CreateForce"));
        }

        // title = "删除玩家组 [R]"
        public static void DestroyForce(JForce whichForce)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyForce"), whichForce);
        }

        // title = "添加玩家 [R]"
        public static void ForceAddPlayer(JForce whichForce, JPlayer whichPlayer)
        {
            War3.CallNative(War3.GetNativeFunction("ForceAddPlayer"), whichForce, whichPlayer);
        }

        // title = "移除玩家 [R]"
        public static void ForceRemovePlayer(JForce whichForce, JPlayer whichPlayer)
        {
            War3.CallNative(War3.GetNativeFunction("ForceRemovePlayer"), whichForce, whichPlayer);
        }

        // title = "清空玩家组"
        public static void ForceClear(JForce whichForce)
        {
            War3.CallNative(War3.GetNativeFunction("ForceClear"), whichForce);
        }

        public static void ForceEnumPlayers(JForce whichForce, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("ForceEnumPlayers"), whichForce, filter);
        }

        public static void ForceEnumPlayersCounted(JForce whichForce, JBoolExpr filter, int countLimit)
        {
            War3.CallNative(War3.GetNativeFunction("ForceEnumPlayersCounted"), whichForce, filter, countLimit);
        }

        public static void ForceEnumAllies(JForce whichForce, JPlayer whichPlayer, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("ForceEnumAllies"), whichForce, whichPlayer, filter);
        }

        public static void ForceEnumEnemies(JForce whichForce, JPlayer whichPlayer, JBoolExpr filter)
        {
            War3.CallNative(War3.GetNativeFunction("ForceEnumEnemies"), whichForce, whichPlayer, filter);
        }

        // title = "选取玩家组内玩家做动作"
        public static void ForForce(JForce whichForce, JCode callback)
        {
            War3.CallNative(War3.GetNativeFunction("ForForce"), whichForce, callback);
        }

        // title = "新建矩形区域(指定边角坐标)"
        public static JRect Rect(float minx, float miny, float maxx, float maxy)
        {
            return War3.CallNative<JRect>(War3.GetNativeFunction("Rect"), minx, miny, maxx, maxy);
        }

        // title = "新建矩形区域(指定边角点)"
        public static JRect RectFromLoc(JLocation min, JLocation max)
        {
            return War3.CallNative<JRect>(War3.GetNativeFunction("RectFromLoc"), min, max);
        }

        // title = "删除矩形区域 [R]"
        public static void RemoveRect(JRect whichRect)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveRect"), whichRect);
        }

        // title = "设置矩形区域(指定坐标) [R]"
        public static void SetRect(JRect whichRect, float minx, float miny, float maxx, float maxy)
        {
            War3.CallNative(War3.GetNativeFunction("SetRect"), whichRect, minx, miny, maxx, maxy);
        }

        // title = "设置矩形区域(指定点) [R]"
        public static void SetRectFromLoc(JRect whichRect, JLocation min, JLocation max)
        {
            War3.CallNative(War3.GetNativeFunction("SetRectFromLoc"), whichRect, min, max);
        }

        // title = "移动矩形区域(指定坐标) [R]"
        public static void MoveRectTo(JRect whichRect, float newCenterX, float newCenterY)
        {
            War3.CallNative(War3.GetNativeFunction("MoveRectTo"), whichRect, newCenterX, newCenterY);
        }

        // title = "移动矩形区域(指定点)"
        public static void MoveRectToLoc(JRect whichRect, JLocation newCenterLoc)
        {
            War3.CallNative(War3.GetNativeFunction("MoveRectToLoc"), whichRect, newCenterLoc);
        }

        // title = "中心X坐标"
        public static float GetRectCenterX(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectCenterX"), whichRect);
        }

        // title = "中心Y坐标"
        public static float GetRectCenterY(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectCenterY"), whichRect);
        }

        // title = "左下角X坐标"
        public static float GetRectMinX(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectMinX"), whichRect);
        }

        // title = "左下角Y坐标"
        public static float GetRectMinY(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectMinY"), whichRect);
        }

        // title = "右上角X坐标"
        public static float GetRectMaxX(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectMaxX"), whichRect);
        }

        // title = "右上角Y坐标"
        public static float GetRectMaxY(JRect whichRect)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRectMaxY"), whichRect);
        }

        // title = "新建区域 [R]"
        public static JRegion CreateRegion()
        {
            return War3.CallNative<JRegion>(War3.GetNativeFunction("CreateRegion"));
        }

        // title = "删除不规则区域 [R]"
        public static void RemoveRegion(JRegion whichRegion)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveRegion"), whichRegion);
        }

        // title = "添加区域 [R]"
        public static void RegionAddRect(JRegion whichRegion, JRect r)
        {
            War3.CallNative(War3.GetNativeFunction("RegionAddRect"), whichRegion, r);
        }

        // title = "移除区域 [R]"
        public static void RegionClearRect(JRegion whichRegion, JRect r)
        {
            War3.CallNative(War3.GetNativeFunction("RegionClearRect"), whichRegion, r);
        }

        // title = "添加单元点(指定坐标) [R]"
        public static void RegionAddCell(JRegion whichRegion, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("RegionAddCell"), whichRegion, x, y);
        }

        // title = "添加单元点(指定点) [R]"
        public static void RegionAddCellAtLoc(JRegion whichRegion, JLocation whichLocation)
        {
            War3.CallNative(War3.GetNativeFunction("RegionAddCellAtLoc"), whichRegion, whichLocation);
        }

        // title = "移除单元点(指定坐标) [R]"
        public static void RegionClearCell(JRegion whichRegion, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("RegionClearCell"), whichRegion, x, y);
        }

        // title = "移除单元点(指定点) [R]"
        public static void RegionClearCellAtLoc(JRegion whichRegion, JLocation whichLocation)
        {
            War3.CallNative(War3.GetNativeFunction("RegionClearCellAtLoc"), whichRegion, whichLocation);
        }

        // title = "坐标点"
        public static JLocation Location(float x, float y)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("Location"), x, y);
        }

        // title = "清除点 [R]"
        public static void RemoveLocation(JLocation whichLocation)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveLocation"), whichLocation);
        }

        // title = "移动点 [R]"
        public static void MoveLocation(JLocation whichLocation, float newX, float newY)
        {
            War3.CallNative(War3.GetNativeFunction("MoveLocation"), whichLocation, newX, newY);
        }

        // title = "点的X轴坐标"
        public static float GetLocationX(JLocation whichLocation)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLocationX"), whichLocation);
        }

        // title = "点的Y轴坐标"
        public static float GetLocationY(JLocation whichLocation)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLocationY"), whichLocation);
        }

        // title = "点的Z轴高度 [R]"
        public static float GetLocationZ(JLocation whichLocation)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLocationZ"), whichLocation);
        }

        // title = "在不规则区域内 [R]"
        public static bool IsUnitInRegion(JRegion whichRegion, JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitInRegion"), whichRegion, whichUnit);
        }

        // title = "包含坐标"
        public static bool IsPointInRegion(JRegion whichRegion, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPointInRegion"), whichRegion, x, y);
        }

        // title = "包含点"
        public static bool IsLocationInRegion(JRegion whichRegion, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsLocationInRegion"), whichRegion, whichLocation);
        }

        public static JRect GetWorldBounds()
        {
            return War3.CallNative<JRect>(War3.GetNativeFunction("GetWorldBounds"));
        }

        // title = "新建触发 [R]"
        public static JTrigger CreateTrigger()
        {
            return War3.CallNative<JTrigger>(War3.GetNativeFunction("CreateTrigger"));
        }

        // title = "删除触发器 [R]"
        public static void DestroyTrigger(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyTrigger"), whichTrigger);
        }

        public static void ResetTrigger(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("ResetTrigger"), whichTrigger);
        }

        // title = "开启触发"
        public static void EnableTrigger(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("EnableTrigger"), whichTrigger);
        }

        // title = "关闭触发"
        public static void DisableTrigger(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("DisableTrigger"), whichTrigger);
        }

        // title = "触发开启"
        public static bool IsTriggerEnabled(JTrigger whichTrigger)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsTriggerEnabled"), whichTrigger);
        }

        public static void TriggerWaitOnSleeps(JTrigger whichTrigger, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerWaitOnSleeps"), whichTrigger, flag);
        }

        public static bool IsTriggerWaitOnSleeps(JTrigger whichTrigger)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsTriggerWaitOnSleeps"), whichTrigger);
        }

        // title = "运行函数 [R]"
        public static void ExecuteFunc(string funcName)
        {
            War3.CallNative(War3.GetNativeFunction("ExecuteFunc"), funcName);
        }

        public static JBoolExpr And(JBoolExpr operandA, JBoolExpr operandB)
        {
            return War3.CallNative<JBoolExpr>(War3.GetNativeFunction("And"), operandA, operandB);
        }

        public static JBoolExpr Or(JBoolExpr operandA, JBoolExpr operandB)
        {
            return War3.CallNative<JBoolExpr>(War3.GetNativeFunction("Or"), operandA, operandB);
        }

        public static JBoolExpr Not(JBoolExpr operand)
        {
            return War3.CallNative<JBoolExpr>(War3.GetNativeFunction("Not"), operand);
        }

        public static JConditionFunc Condition(JCode func)
        {
            return War3.CallNative<JConditionFunc>(War3.GetNativeFunction("Condition"), func);
        }

        public static void DestroyCondition(JConditionFunc c)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyCondition"), c);
        }

        public static JFilterFunc Filter(JCode func)
        {
            return War3.CallNative<JFilterFunc>(War3.GetNativeFunction("Filter"), func);
        }

        public static void DestroyFilter(JFilterFunc f)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyFilter"), f);
        }

        public static void DestroyBoolExpr(JBoolExpr e)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyBoolExpr"), e);
        }

        // title = "实数变量事件"
        public static JEvent TriggerRegisterVariableEvent(JTrigger whichTrigger, string varName, JLimitOp opcode, float limitval)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterVariableEvent"), whichTrigger, varName, opcode, limitval);
        }

        public static JEvent TriggerRegisterTimerEvent(JTrigger whichTrigger, float timeout, bool periodic)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterTimerEvent"), whichTrigger, timeout, periodic);
        }

        public static JEvent TriggerRegisterTimerExpireEvent(JTrigger whichTrigger, JTimer t)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterTimerExpireEvent"), whichTrigger, t);
        }

        public static JEvent TriggerRegisterGameStateEvent(JTrigger whichTrigger, JGameState whichState, JLimitOp opcode, float limitval)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterGameStateEvent"), whichTrigger, whichState, opcode, limitval);
        }

        public static JEvent TriggerRegisterDialogEvent(JTrigger whichTrigger, JDialog whichDialog)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterDialogEvent"), whichTrigger, whichDialog);
        }

        // title = "对话框按钮被点击 [R]"
        public static JEvent TriggerRegisterDialogButtonEvent(JTrigger whichTrigger, JButton whichButton)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterDialogButtonEvent"), whichTrigger, whichButton);
        }

        // title = "比赛游戏事件"
        public static JEvent TriggerRegisterGameEvent(JTrigger whichTrigger, JGameEvent whichGameEvent)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterGameEvent"), whichTrigger, whichGameEvent);
        }

        // title = "单位进入不规则区域(指定条件) [R]"
        public static JEvent TriggerRegisterEnterRegion(JTrigger whichTrigger, JRegion whichRegion, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterEnterRegion"), whichTrigger, whichRegion, filter);
        }

        // title = "单位离开不规则区域(指定条件) [R]"
        public static JEvent TriggerRegisterLeaveRegion(JTrigger whichTrigger, JRegion whichRegion, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterLeaveRegion"), whichTrigger, whichRegion, filter);
        }

        // title = "鼠标点击可追踪物 [R]"
        public static JEvent TriggerRegisterTrackableHitEvent(JTrigger whichTrigger, JTrackable t)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterTrackableHitEvent"), whichTrigger, t);
        }

        // title = "鼠标移动到追踪对象 [R]"
        public static JEvent TriggerRegisterTrackableTrackEvent(JTrigger whichTrigger, JTrackable t)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterTrackableTrackEvent"), whichTrigger, t);
        }

        public static JEvent TriggerRegisterPlayerEvent(JTrigger whichTrigger, JPlayer whichPlayer, JPlayerEvent whichPlayerEvent)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerEvent"), whichTrigger, whichPlayer, whichPlayerEvent);
        }

        public static JEvent TriggerRegisterPlayerUnitEvent(JTrigger whichTrigger, JPlayer whichPlayer, JPlayerUnitEvent whichPlayerUnitEvent, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerUnitEvent"), whichTrigger, whichPlayer, whichPlayerUnitEvent, filter);
        }

        // title = "联盟状态更改(指定项目)"
        public static JEvent TriggerRegisterPlayerAllianceChange(JTrigger whichTrigger, JPlayer whichPlayer, JAllianceType whichAlliance)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerAllianceChange"), whichTrigger, whichPlayer, whichAlliance);
        }

        // title = "属性事件"
        public static JEvent TriggerRegisterPlayerStateEvent(JTrigger whichTrigger, JPlayer whichPlayer, JPlayerState whichState, JLimitOp opcode, float limitval)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerStateEvent"), whichTrigger, whichPlayer, whichState, opcode, limitval);
        }

        // title = "输入聊天信息"
        public static JEvent TriggerRegisterPlayerChatEvent(JTrigger whichTrigger, JPlayer whichPlayer, string chatMessageToDetect, bool exactMatchOnly)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterPlayerChatEvent"), whichTrigger, whichPlayer, chatMessageToDetect, exactMatchOnly);
        }

        // title = "可破坏物死亡"
        public static JEvent TriggerRegisterDeathEvent(JTrigger whichTrigger, JWidget whichWidget)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterDeathEvent"), whichTrigger, whichWidget);
        }

        public static JEvent TriggerRegisterUnitStateEvent(JTrigger whichTrigger, JUnit whichUnit, JUnitState whichState, JLimitOp opcode, float limitval)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterUnitStateEvent"), whichTrigger, whichUnit, whichState, opcode, limitval);
        }

        // title = "指定单位事件"
        public static JEvent TriggerRegisterUnitEvent(JTrigger whichTrigger, JUnit whichUnit, JUnitEvent whichEvent)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterUnitEvent"), whichTrigger, whichUnit, whichEvent);
        }

        public static JEvent TriggerRegisterFilterUnitEvent(JTrigger whichTrigger, JUnit whichUnit, JUnitEvent whichEvent, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterFilterUnitEvent"), whichTrigger, whichUnit, whichEvent, filter);
        }

        public static JEvent TriggerRegisterUnitInRange(JTrigger whichTrigger, JUnit whichUnit, float range, JBoolExpr filter)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("TriggerRegisterUnitInRange"), whichTrigger, whichUnit, range, filter);
        }

        public static JTriggerCondition TriggerAddCondition(JTrigger whichTrigger, JBoolExpr condition)
        {
            return War3.CallNative<JTriggerCondition>(War3.GetNativeFunction("TriggerAddCondition"), whichTrigger, condition);
        }

        public static void TriggerRemoveCondition(JTrigger whichTrigger, JTriggerCondition whichCondition)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerRemoveCondition"), whichTrigger, whichCondition);
        }

        public static void TriggerClearConditions(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerClearConditions"), whichTrigger);
        }

        public static JTriggerAction TriggerAddAction(JTrigger whichTrigger, JCode actionFunc)
        {
            return War3.CallNative<JTriggerAction>(War3.GetNativeFunction("TriggerAddAction"), whichTrigger, actionFunc);
        }

        public static void TriggerRemoveAction(JTrigger whichTrigger, JTriggerAction whichAction)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerRemoveAction"), whichTrigger, whichAction);
        }

        public static void TriggerClearActions(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerClearActions"), whichTrigger);
        }

        // title = "等待(玩家时间)"
        public static void TriggerSleepAction(float timeout)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerSleepAction"), timeout);
        }

        public static void TriggerWaitForSound(JSound s, float offset)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerWaitForSound"), s, offset);
        }

        // title = "触发条件成立"
        public static bool TriggerEvaluate(JTrigger whichTrigger)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("TriggerEvaluate"), whichTrigger);
        }

        // title = "运行触发(无视条件)"
        public static void TriggerExecute(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerExecute"), whichTrigger);
        }

        public static void TriggerExecuteWait(JTrigger whichTrigger)
        {
            War3.CallNative(War3.GetNativeFunction("TriggerExecuteWait"), whichTrigger);
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
            return War3.CallNative<float>(War3.GetNativeFunction("GetWidgetLife"), whichWidget);
        }

        public static void SetWidgetLife(JWidget whichWidget, float newLife)
        {
            War3.CallNative(War3.GetNativeFunction("SetWidgetLife"), whichWidget, newLife);
        }

        public static float GetWidgetX(JWidget whichWidget)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetWidgetX"), whichWidget);
        }

        public static float GetWidgetY(JWidget whichWidget)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetWidgetY"), whichWidget);
        }

        public static JDestructable CreateDestructable(int objectid, float x, float y, float face, float scale, int variation)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("CreateDestructable"), objectid, x, y, face, scale, variation);
        }

        // title = "新建可破坏物 [R]"
        public static JDestructable CreateDestructableZ(int objectid, float x, float y, float z, float face, float scale, int variation)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("CreateDestructableZ"), objectid, x, y, z, face, scale, variation);
        }

        public static JDestructable CreateDeadDestructable(int objectid, float x, float y, float face, float scale, int variation)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("CreateDeadDestructable"), objectid, x, y, face, scale, variation);
        }

        // title = "新建可破坏物(死亡的) [R]"
        public static JDestructable CreateDeadDestructableZ(int objectid, float x, float y, float z, float face, float scale, int variation)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("CreateDeadDestructableZ"), objectid, x, y, z, face, scale, variation);
        }

        // title = "删除"
        public static void RemoveDestructable(JDestructable d)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveDestructable"), d);
        }

        // title = "杀死"
        public static void KillDestructable(JDestructable d)
        {
            War3.CallNative(War3.GetNativeFunction("KillDestructable"), d);
        }

        public static void SetDestructableInvulnerable(JDestructable d, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableInvulnerable"), d, flag);
        }

        public static bool IsDestructableInvulnerable(JDestructable d)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsDestructableInvulnerable"), d);
        }

        public static void EnumDestructablesInRect(JRect r, JBoolExpr filter, JCode actionFunc)
        {
            War3.CallNative(War3.GetNativeFunction("EnumDestructablesInRect"), r, filter, actionFunc);
        }

        // title = "指定可破坏物的类型"
        public static int GetDestructableTypeId(JDestructable d)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetDestructableTypeId"), d);
        }

        // title = "可破坏物所在X轴坐标 [R]"
        public static float GetDestructableX(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableX"), d);
        }

        // title = "可破坏物所在Y轴坐标 [R]"
        public static float GetDestructableY(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableY"), d);
        }

        // title = "设置生命值(指定值)"
        public static void SetDestructableLife(JDestructable d, float life)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableLife"), d, life);
        }

        // title = "生命值"
        public static float GetDestructableLife(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableLife"), d);
        }

        public static void SetDestructableMaxLife(JDestructable d, float max)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableMaxLife"), d, max);
        }

        // title = "最大生命值"
        public static float GetDestructableMaxLife(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableMaxLife"), d);
        }

        // title = "复活"
        public static void DestructableRestoreLife(JDestructable d, float life, bool birth)
        {
            War3.CallNative(War3.GetNativeFunction("DestructableRestoreLife"), d, life, birth);
        }

        public static void QueueDestructableAnimation(JDestructable d, string whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("QueueDestructableAnimation"), d, whichAnimation);
        }

        public static void SetDestructableAnimation(JDestructable d, string whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableAnimation"), d, whichAnimation);
        }

        // title = "改变可破坏物动画播放速度 [R]"
        public static void SetDestructableAnimationSpeed(JDestructable d, float speedFactor)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableAnimationSpeed"), d, speedFactor);
        }

        // title = "显示/隐藏 [R]"
        public static void ShowDestructable(JDestructable d, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("ShowDestructable"), d, flag);
        }

        // title = "闭塞高度"
        public static float GetDestructableOccluderHeight(JDestructable d)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetDestructableOccluderHeight"), d);
        }

        // title = "设置闭塞高度"
        public static void SetDestructableOccluderHeight(JDestructable d, float height)
        {
            War3.CallNative(War3.GetNativeFunction("SetDestructableOccluderHeight"), d, height);
        }

        // title = "物件名字"
        public static string GetDestructableName(JDestructable d)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetDestructableName"), d);
        }

        // title = "创建"
        public static JItem CreateItem(int itemid, float x, float y)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("CreateItem"), itemid, x, y);
        }

        // title = "删除"
        public static void RemoveItem(JItem whichItem)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveItem"), whichItem);
        }

        // title = "物品所属玩家"
        public static JPlayer GetItemPlayer(JItem whichItem)
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("GetItemPlayer"), whichItem);
        }

        // title = "指定物品的类型"
        public static int GetItemTypeId(JItem i)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetItemTypeId"), i);
        }

        // title = "物品的X轴坐标 [R]"
        public static float GetItemX(JItem i)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetItemX"), i);
        }

        // title = "物品的Y轴坐标 [R]"
        public static float GetItemY(JItem i)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetItemY"), i);
        }

        // title = "移动物品到坐标(立即)(指定坐标) [R]"
        public static void SetItemPosition(JItem i, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemPosition"), i, x, y);
        }

        public static void SetItemDropOnDeath(JItem whichItem, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemDropOnDeath"), whichItem, flag);
        }

        public static void SetItemDroppable(JItem i, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemDroppable"), i, flag);
        }

        // title = "设置物品可否抵押"
        public static void SetItemPawnable(JItem i, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemPawnable"), i, flag);
        }

        public static void SetItemPlayer(JItem whichItem, JPlayer whichPlayer, bool changeColor)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemPlayer"), whichItem, whichPlayer, changeColor);
        }

        public static void SetItemInvulnerable(JItem whichItem, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemInvulnerable"), whichItem, flag);
        }

        // title = "物品无敌"
        public static bool IsItemInvulnerable(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemInvulnerable"), whichItem);
        }

        // title = "显示/隐藏 [R]"
        public static void SetItemVisible(JItem whichItem, bool show)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemVisible"), whichItem, show);
        }

        // title = "物品可见 [R]"
        public static bool IsItemVisible(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemVisible"), whichItem);
        }

        // title = "物品被持有"
        public static bool IsItemOwned(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemOwned"), whichItem);
        }

        // title = "物品是拾取时自动使用的 [R]"
        public static bool IsItemPowerup(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemPowerup"), whichItem);
        }

        // title = "物品可被市场随机出售 [R]"
        public static bool IsItemSellable(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemSellable"), whichItem);
        }

        // title = "物品可被抵押 [R]"
        public static bool IsItemPawnable(JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsItemPawnable"), whichItem);
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
            War3.CallNative(War3.GetNativeFunction("EnumItemsInRect"), r, filter, actionFunc);
        }

        // title = "物品等级"
        public static int GetItemLevel(JItem whichItem)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetItemLevel"), whichItem);
        }

        // title = "指定物品的分类"
        public static JItemType GetItemType(JItem whichItem)
        {
            return War3.CallNative<JItemType>(War3.GetNativeFunction("GetItemType"), whichItem);
        }

        // title = "设置重生神符的产生单位类型"
        public static void SetItemDropID(JItem whichItem, int unitId)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemDropID"), whichItem, unitId);
        }

        // title = "使用次数"
        public static int GetItemCharges(JItem whichItem)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetItemCharges"), whichItem);
        }

        // title = "设置物品使用次数"
        public static void SetItemCharges(JItem whichItem, int charges)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemCharges"), whichItem, charges);
        }

        // title = "物品自定义值"
        public static int GetItemUserData(JItem whichItem)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetItemUserData"), whichItem);
        }

        // title = "设置物品自定义值"
        public static void SetItemUserData(JItem whichItem, int data)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemUserData"), whichItem, data);
        }

        // title = "新建单位(指定坐标) [R]"
        public static JUnit CreateUnit(JPlayer id, int unitid, float x, float y, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateUnit"), id, unitid, x, y, face);
        }

        public static JUnit CreateUnitByName(JPlayer whichPlayer, string unitname, float x, float y, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateUnitByName"), whichPlayer, unitname, x, y, face);
        }

        // title = "新建单位(指定点) [R]"
        public static JUnit CreateUnitAtLoc(JPlayer id, int unitid, JLocation whichLocation, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateUnitAtLoc"), id, unitid, whichLocation, face);
        }

        public static JUnit CreateUnitAtLocByName(JPlayer id, string unitname, JLocation whichLocation, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateUnitAtLocByName"), id, unitname, whichLocation, face);
        }

        // title = "新建尸体 [R]"
        public static JUnit CreateCorpse(JPlayer whichPlayer, int unitid, float x, float y, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateCorpse"), whichPlayer, unitid, x, y, face);
        }

        // title = "杀死"
        public static void KillUnit(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("KillUnit"), whichUnit);
        }

        // title = "删除"
        public static void RemoveUnit(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveUnit"), whichUnit);
        }

        // title = "显示/隐藏 [R]"
        public static void ShowUnit(JUnit whichUnit, bool show)
        {
            War3.CallNative(War3.GetNativeFunction("ShowUnit"), whichUnit, show);
        }

        // title = "设置单位属性 [R]"
        public static void SetUnitState(JUnit whichUnit, JUnitState whichUnitState, float newVal)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitState"), whichUnit, whichUnitState, newVal);
        }

        // title = "设置X坐标 [R]"
        public static void SetUnitX(JUnit whichUnit, float newX)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitX"), whichUnit, newX);
        }

        // title = "设置Y坐标 [R]"
        public static void SetUnitY(JUnit whichUnit, float newY)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitY"), whichUnit, newY);
        }

        // title = "移动单位(立即)(指定坐标) [R]"
        public static void SetUnitPosition(JUnit whichUnit, float newX, float newY)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitPosition"), whichUnit, newX, newY);
        }

        // title = "移动单位(立即)(指定点)"
        public static void SetUnitPositionLoc(JUnit whichUnit, JLocation whichLocation)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitPositionLoc"), whichUnit, whichLocation);
        }

        // title = "设置单位面向角度 [R]"
        public static void SetUnitFacing(JUnit whichUnit, float facingAngle)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitFacing"), whichUnit, facingAngle);
        }

        // title = "设置单位面向角度(指定时间)"
        public static void SetUnitFacingTimed(JUnit whichUnit, float facingAngle, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitFacingTimed"), whichUnit, facingAngle, duration);
        }

        // title = "设置移动速度"
        public static void SetUnitMoveSpeed(JUnit whichUnit, float newSpeed)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitMoveSpeed"), whichUnit, newSpeed);
        }

        public static void SetUnitFlyHeight(JUnit whichUnit, float newHeight, float rate)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitFlyHeight"), whichUnit, newHeight, rate);
        }

        public static void SetUnitTurnSpeed(JUnit whichUnit, float newTurnSpeed)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitTurnSpeed"), whichUnit, newTurnSpeed);
        }

        // title = "改变单位转向角度(弧度制) [R]"
        public static void SetUnitPropWindow(JUnit whichUnit, float newPropWindowAngle)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitPropWindow"), whichUnit, newPropWindowAngle);
        }

        public static void SetUnitAcquireRange(JUnit whichUnit, float newAcquireRange)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitAcquireRange"), whichUnit, newAcquireRange);
        }

        // title = "锁定指定单位的警戒点 [R]"
        public static void SetUnitCreepGuard(JUnit whichUnit, bool creepGuard)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitCreepGuard"), whichUnit, creepGuard);
        }

        // title = "当前主动攻击范围"
        public static float GetUnitAcquireRange(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitAcquireRange"), whichUnit);
        }

        // title = "当前转身速度"
        public static float GetUnitTurnSpeed(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitTurnSpeed"), whichUnit);
        }

        // title = "当前转向角度(弧度制) [R]"
        public static float GetUnitPropWindow(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitPropWindow"), whichUnit);
        }

        // title = "当前飞行高度"
        public static float GetUnitFlyHeight(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitFlyHeight"), whichUnit);
        }

        // title = "默认主动攻击范围"
        public static float GetUnitDefaultAcquireRange(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultAcquireRange"), whichUnit);
        }

        // title = "默认转身速度"
        public static float GetUnitDefaultTurnSpeed(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultTurnSpeed"), whichUnit);
        }

        public static float GetUnitDefaultPropWindow(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultPropWindow"), whichUnit);
        }

        // title = "默认飞行高度"
        public static float GetUnitDefaultFlyHeight(JUnit whichUnit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetUnitDefaultFlyHeight"), whichUnit);
        }

        // title = "改变所属"
        public static void SetUnitOwner(JUnit whichUnit, JPlayer whichPlayer, bool changeColor)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitOwner"), whichUnit, whichPlayer, changeColor);
        }

        // title = "改变队伍颜色"
        public static void SetUnitColor(JUnit whichUnit, JPlayerColor whichColor)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitColor"), whichUnit, whichColor);
        }

        // title = "改变单位尺寸(按倍数) [R]"
        public static void SetUnitScale(JUnit whichUnit, float scaleX, float scaleY, float scaleZ)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitScale"), whichUnit, scaleX, scaleY, scaleZ);
        }

        // title = "改变单位动画播放速度(按倍数) [R]"
        public static void SetUnitTimeScale(JUnit whichUnit, float timeScale)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitTimeScale"), whichUnit, timeScale);
        }

        public static void SetUnitBlendTime(JUnit whichUnit, float blendTime)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitBlendTime"), whichUnit, blendTime);
        }

        // title = "改变单位的颜色(RGB:0-255) [R]"
        public static void SetUnitVertexColor(JUnit whichUnit, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitVertexColor"), whichUnit, red, green, blue, alpha);
        }

        public static void QueueUnitAnimation(JUnit whichUnit, string whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("QueueUnitAnimation"), whichUnit, whichAnimation);
        }

        // title = "播放单位动画"
        public static void SetUnitAnimation(JUnit whichUnit, string whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitAnimation"), whichUnit, whichAnimation);
        }

        // title = "播放单位指定序号动动作 [R]"
        public static void SetUnitAnimationByIndex(JUnit whichUnit, int whichAnimation)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitAnimationByIndex"), whichUnit, whichAnimation);
        }

        // title = "播放单位动运作(指定概率)"
        public static void SetUnitAnimationWithRarity(JUnit whichUnit, string whichAnimation, JRarityControl rarity)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitAnimationWithRarity"), whichUnit, whichAnimation, rarity);
        }

        // title = "添加/删除 单位动画附加名 [R]"
        public static void AddUnitAnimationProperties(JUnit whichUnit, string animProperties, bool add)
        {
            War3.CallNative(War3.GetNativeFunction("AddUnitAnimationProperties"), whichUnit, animProperties, add);
        }

        // title = "锁定身体朝向"
        public static void SetUnitLookAt(JUnit whichUnit, string whichBone, JUnit lookAtTarget, float offsetX, float offsetY, float offsetZ)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitLookAt"), whichUnit, whichBone, lookAtTarget, offsetX, offsetY, offsetZ);
        }

        // title = "重置身体朝向"
        public static void ResetUnitLookAt(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("ResetUnitLookAt"), whichUnit);
        }

        // title = "设置可否营救(对玩家) [R]"
        public static void SetUnitRescuable(JUnit whichUnit, JPlayer byWhichPlayer, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitRescuable"), whichUnit, byWhichPlayer, flag);
        }

        // title = "设置营救范围"
        public static void SetUnitRescueRange(JUnit whichUnit, float range)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitRescueRange"), whichUnit, range);
        }

        // title = "设置英雄力量 [R]"
        public static void SetHeroStr(JUnit whichHero, int newStr, bool permanent)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroStr"), whichHero, newStr, permanent);
        }

        // title = "设置英雄敏捷 [R]"
        public static void SetHeroAgi(JUnit whichHero, int newAgi, bool permanent)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroAgi"), whichHero, newAgi, permanent);
        }

        // title = "设置英雄智力 [R]"
        public static void SetHeroInt(JUnit whichHero, int newInt, bool permanent)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroInt"), whichHero, newInt, permanent);
        }

        // title = "英雄力量 [R]"
        public static int GetHeroStr(JUnit whichHero, bool includeBonuses)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroStr"), whichHero, includeBonuses);
        }

        // title = "英雄敏捷 [R]"
        public static int GetHeroAgi(JUnit whichHero, bool includeBonuses)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroAgi"), whichHero, includeBonuses);
        }

        // title = "英雄智力 [R]"
        public static int GetHeroInt(JUnit whichHero, bool includeBonuses)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroInt"), whichHero, includeBonuses);
        }

        // title = "降低等级 [R]"
        public static bool UnitStripHeroLevel(JUnit whichHero, int howManyLevels)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitStripHeroLevel"), whichHero, howManyLevels);
        }

        // title = "英雄经验值"
        public static int GetHeroXP(JUnit whichHero)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroXP"), whichHero);
        }

        // title = "设置经验值"
        public static void SetHeroXP(JUnit whichHero, int newXpVal, bool showEyeCandy)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroXP"), whichHero, newXpVal, showEyeCandy);
        }

        // title = "未分配技能点数"
        public static int GetHeroSkillPoints(JUnit whichHero)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetHeroSkillPoints"), whichHero);
        }

        // title = "添加剩余技能点 [R]"
        public static bool UnitModifySkillPoints(JUnit whichHero, int skillPointDelta)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitModifySkillPoints"), whichHero, skillPointDelta);
        }

        // title = "增加经验值 [R]"
        public static void AddHeroXP(JUnit whichHero, int xpToAdd, bool showEyeCandy)
        {
            War3.CallNative(War3.GetNativeFunction("AddHeroXP"), whichHero, xpToAdd, showEyeCandy);
        }

        // title = "设置等级"
        public static void SetHeroLevel(JUnit whichHero, int level, bool showEyeCandy)
        {
            War3.CallNative(War3.GetNativeFunction("SetHeroLevel"), whichHero, level, showEyeCandy);
        }

        // title = "英雄称谓"
        public static string GetHeroProperName(JUnit whichHero)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetHeroProperName"), whichHero);
        }

        // title = "允许/禁止经验获取 [R]"
        public static void SuspendHeroXP(JUnit whichHero, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SuspendHeroXP"), whichHero, flag);
        }

        // title = "经验不可获得"
        public static bool IsSuspendedXP(JUnit whichHero)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsSuspendedXP"), whichHero);
        }

        // title = "学习技能"
        public static void SelectHeroSkill(JUnit whichHero, int abilcode)
        {
            War3.CallNative(War3.GetNativeFunction("SelectHeroSkill"), whichHero, abilcode);
        }

        // title = "单位技能等级 [R]"
        public static int GetUnitAbilityLevel(JUnit whichUnit, int abilcode)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitAbilityLevel"), whichUnit, abilcode);
        }

        // title = "降低技能等级 [R]"
        public static int DecUnitAbilityLevel(JUnit whichUnit, int abilcode)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DecUnitAbilityLevel"), whichUnit, abilcode);
        }

        // title = "提升技能等级 [R]"
        public static int IncUnitAbilityLevel(JUnit whichUnit, int abilcode)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("IncUnitAbilityLevel"), whichUnit, abilcode);
        }

        // title = "设置技能等级 [R]"
        public static int SetUnitAbilityLevel(JUnit whichUnit, int abilcode, int level)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("SetUnitAbilityLevel"), whichUnit, abilcode, level);
        }

        // title = "立即复活(指定坐标) [R]"
        public static bool ReviveHero(JUnit whichHero, float x, float y, bool doEyecandy)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("ReviveHero"), whichHero, x, y, doEyecandy);
        }

        // title = "立即复活(指定点)"
        public static bool ReviveHeroLoc(JUnit whichHero, JLocation loc, bool doEyecandy)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("ReviveHeroLoc"), whichHero, loc, doEyecandy);
        }

        public static void SetUnitExploded(JUnit whichUnit, bool exploded)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitExploded"), whichUnit, exploded);
        }

        // title = "设置无敌/可攻击"
        public static void SetUnitInvulnerable(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitInvulnerable"), whichUnit, flag);
        }

        // title = "暂停/恢复 [R]"
        public static void PauseUnit(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("PauseUnit"), whichUnit, flag);
        }

        public static bool IsUnitPaused(JUnit whichHero)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsUnitPaused"), whichHero);
        }

        // title = "设置碰撞开关"
        public static void SetUnitPathing(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitPathing"), whichUnit, flag);
        }

        // title = "清空选择(所有玩家)"
        public static void ClearSelection()
        {
            War3.CallNative(War3.GetNativeFunction("ClearSelection"));
        }

        public static void SelectUnit(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SelectUnit"), whichUnit, flag);
        }

        // title = "单位附加值"
        public static int GetUnitPointValue(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitPointValue"), whichUnit);
        }

        // title = "单位附加值(指定单位类型)"
        public static int GetUnitPointValueByType(int JUnitType)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitPointValueByType"), JUnitType);
        }

        // title = "给予物品 [R]"
        public static bool UnitAddItem(JUnit whichUnit, JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitAddItem"), whichUnit, whichItem);
        }

        public static JItem UnitAddItemById(JUnit whichUnit, int itemId)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("UnitAddItemById"), whichUnit, itemId);
        }

        // title = "新建物品到指定物品栏 [R]"
        public static bool UnitAddItemToSlotById(JUnit whichUnit, int itemId, int itemSlot)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitAddItemToSlotById"), whichUnit, itemId, itemSlot);
        }

        public static void UnitRemoveItem(JUnit whichUnit, JItem whichItem)
        {
            War3.CallNative(War3.GetNativeFunction("UnitRemoveItem"), whichUnit, whichItem);
        }

        public static JItem UnitRemoveItemFromSlot(JUnit whichUnit, int itemSlot)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("UnitRemoveItemFromSlot"), whichUnit, itemSlot);
        }

        // title = "持有物品"
        public static bool UnitHasItem(JUnit whichUnit, JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitHasItem"), whichUnit, whichItem);
        }

        // title = "单位持有物品"
        public static JItem UnitItemInSlot(JUnit whichUnit, int itemSlot)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("UnitItemInSlot"), whichUnit, itemSlot);
        }

        public static int UnitInventorySize(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("UnitInventorySize"), whichUnit);
        }

        // title = "发布丢弃物品命令(指定坐标) [R]"
        public static bool UnitDropItemPoint(JUnit whichUnit, JItem whichItem, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDropItemPoint"), whichUnit, whichItem, x, y);
        }

        // title = "移动物品到物品栏 [R]"
        public static bool UnitDropItemSlot(JUnit whichUnit, JItem whichItem, int slot)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDropItemSlot"), whichUnit, whichItem, slot);
        }

        public static bool UnitDropItemTarget(JUnit whichUnit, JItem whichItem, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDropItemTarget"), whichUnit, whichItem, target);
        }

        // title = "使用物品(无目标)"
        public static bool UnitUseItem(JUnit whichUnit, JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitUseItem"), whichUnit, whichItem);
        }

        // title = "使用物品(指定坐标)"
        public static bool UnitUseItemPoint(JUnit whichUnit, JItem whichItem, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitUseItemPoint"), whichUnit, whichItem, x, y);
        }

        // title = "使用物品(对单位)"
        public static bool UnitUseItemTarget(JUnit whichUnit, JItem whichItem, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitUseItemTarget"), whichUnit, whichItem, target);
        }

        // title = "允许/禁止 人口占用 [R]"
        public static void SetUnitUseFood(JUnit whichUnit, bool useFood)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitUseFood"), whichUnit, useFood);
        }

        // title = "共享视野 [R]"
        public static void UnitShareVision(JUnit whichUnit, JPlayer whichPlayer, bool share)
        {
            War3.CallNative(War3.GetNativeFunction("UnitShareVision"), whichUnit, whichPlayer, share);
        }

        // title = "暂停尸体腐烂 [R]"
        public static void UnitSuspendDecay(JUnit whichUnit, bool suspend)
        {
            War3.CallNative(War3.GetNativeFunction("UnitSuspendDecay"), whichUnit, suspend);
        }

        // title = "添加类别 [R]"
        public static bool UnitAddType(JUnit whichUnit, JUnitType whichUnitType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitAddType"), whichUnit, whichUnitType);
        }

        // title = "删除类别 [R]"
        public static bool UnitRemoveType(JUnit whichUnit, JUnitType whichUnitType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitRemoveType"), whichUnit, whichUnitType);
        }

        // title = "添加技能 [R]"
        public static bool UnitAddAbility(JUnit whichUnit, int abilityId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitAddAbility"), whichUnit, abilityId);
        }

        // title = "删除技能 [R]"
        public static bool UnitRemoveAbility(JUnit whichUnit, int abilityId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitRemoveAbility"), whichUnit, abilityId);
        }

        // title = "设置技能永久性 [R]"
        public static bool UnitMakeAbilityPermanent(JUnit whichUnit, bool permanent, int abilityId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitMakeAbilityPermanent"), whichUnit, permanent, abilityId);
        }

        // title = "删除魔法效果(指定极性) [R]"
        public static void UnitRemoveBuffs(JUnit whichUnit, bool removePositive, bool removeNegative)
        {
            War3.CallNative(War3.GetNativeFunction("UnitRemoveBuffs"), whichUnit, removePositive, removeNegative);
        }

        // title = "删除魔法效果(详细类别) [R]"
        public static void UnitRemoveBuffsEx(JUnit whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel)
        {
            War3.CallNative(War3.GetNativeFunction("UnitRemoveBuffsEx"), whichUnit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel);
        }

        public static bool UnitHasBuffsEx(JUnit whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitHasBuffsEx"), whichUnit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel);
        }

        // title = "拥有Buff数量 [R]"
        public static int UnitCountBuffsEx(JUnit whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("UnitCountBuffsEx"), whichUnit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel);
        }

        public static void UnitAddSleep(JUnit whichUnit, bool add)
        {
            War3.CallNative(War3.GetNativeFunction("UnitAddSleep"), whichUnit, add);
        }

        public static bool UnitCanSleep(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitCanSleep"), whichUnit);
        }

        // title = "控制单位睡眠状态"
        public static void UnitAddSleepPerm(JUnit whichUnit, bool add)
        {
            War3.CallNative(War3.GetNativeFunction("UnitAddSleepPerm"), whichUnit, add);
        }

        // title = "允许控制睡眠状态"
        public static bool UnitCanSleepPerm(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitCanSleepPerm"), whichUnit);
        }

        public static bool UnitIsSleeping(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitIsSleeping"), whichUnit);
        }

        public static void UnitWakeUp(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("UnitWakeUp"), whichUnit);
        }

        // title = "设置生命周期 [R]"
        public static void UnitApplyTimedLife(JUnit whichUnit, int buffId, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("UnitApplyTimedLife"), whichUnit, buffId, duration);
        }

        public static bool UnitIgnoreAlarm(JUnit whichUnit, bool flag)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitIgnoreAlarm"), whichUnit, flag);
        }

        public static bool UnitIgnoreAlarmToggled(JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitIgnoreAlarmToggled"), whichUnit);
        }

        // title = "重置技能CD"
        public static void UnitResetCooldown(JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("UnitResetCooldown"), whichUnit);
        }

        // title = "设置建筑建造进度条"
        public static void UnitSetConstructionProgress(JUnit whichUnit, int constructionPercentage)
        {
            War3.CallNative(War3.GetNativeFunction("UnitSetConstructionProgress"), whichUnit, constructionPercentage);
        }

        // title = "设置建筑升级进度条"
        public static void UnitSetUpgradeProgress(JUnit whichUnit, int upgradePercentage)
        {
            War3.CallNative(War3.GetNativeFunction("UnitSetUpgradeProgress"), whichUnit, upgradePercentage);
        }

        // title = "暂停/恢复生命周期 [R]"
        public static void UnitPauseTimedLife(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("UnitPauseTimedLife"), whichUnit, flag);
        }

        public static void UnitSetUsesAltIcon(JUnit whichUnit, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("UnitSetUsesAltIcon"), whichUnit, flag);
        }

        // title = "伤害区域 [R]"
        public static bool UnitDamagePoint(JUnit whichUnit, float delay, float radius, float x, float y, float amount, bool attack, bool ranged, JAttackType JAttackType, JDamageType JDamageType, JWeaponType JWeaponType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDamagePoint"), whichUnit, delay, radius, x, y, amount, attack, ranged, JAttackType, JDamageType, JWeaponType);
        }

        // title = "伤害目标 [R]"
        public static bool UnitDamageTarget(JUnit whichUnit, JWidget target, float amount, bool attack, bool ranged, JAttackType JAttackType, JDamageType JDamageType, JWeaponType JWeaponType)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("UnitDamageTarget"), whichUnit, target, amount, attack, ranged, JAttackType, JDamageType, JWeaponType);
        }

        // title = "发布命令(无目标)"
        public static bool IssueImmediateOrder(JUnit whichUnit, string order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueImmediateOrder"), whichUnit, order);
        }

        // title = "发布命令(无目标)(ID)"
        public static bool IssueImmediateOrderById(JUnit whichUnit, int order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueImmediateOrderById"), whichUnit, order);
        }

        // title = "发布命令(指定坐标)"
        public static bool IssuePointOrder(JUnit whichUnit, string order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssuePointOrder"), whichUnit, order, x, y);
        }

        // title = "发布命令(指定点)"
        public static bool IssuePointOrderLoc(JUnit whichUnit, string order, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssuePointOrderLoc"), whichUnit, order, whichLocation);
        }

        // title = "发布命令(指定坐标)(ID)"
        public static bool IssuePointOrderById(JUnit whichUnit, int order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssuePointOrderById"), whichUnit, order, x, y);
        }

        // title = "发布命令(指定点)(ID)"
        public static bool IssuePointOrderByIdLoc(JUnit whichUnit, int order, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssuePointOrderByIdLoc"), whichUnit, order, whichLocation);
        }

        // title = "发布命令(指定单位)"
        public static bool IssueTargetOrder(JUnit whichUnit, string order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueTargetOrder"), whichUnit, order, targetWidget);
        }

        // title = "发布命令(指定单位)(ID)"
        public static bool IssueTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueTargetOrderById"), whichUnit, order, targetWidget);
        }

        public static bool IssueInstantPointOrder(JUnit whichUnit, string order, float x, float y, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueInstantPointOrder"), whichUnit, order, x, y, instantTargetWidget);
        }

        public static bool IssueInstantPointOrderById(JUnit whichUnit, int order, float x, float y, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueInstantPointOrderById"), whichUnit, order, x, y, instantTargetWidget);
        }

        public static bool IssueInstantTargetOrder(JUnit whichUnit, string order, JWidget targetWidget, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueInstantTargetOrder"), whichUnit, order, targetWidget, instantTargetWidget);
        }

        public static bool IssueInstantTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueInstantTargetOrderById"), whichUnit, order, targetWidget, instantTargetWidget);
        }

        public static bool IssueBuildOrder(JUnit whichPeon, string unitToBuild, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueBuildOrder"), whichPeon, unitToBuild, x, y);
        }

        // title = "发布建造命令(指定坐标) [R]"
        public static bool IssueBuildOrderById(JUnit whichPeon, int unitId, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueBuildOrderById"), whichPeon, unitId, x, y);
        }

        // title = "发布中介命令(无目标)"
        public static bool IssueNeutralImmediateOrder(JPlayer forWhichPlayer, JUnit neutralStructure, string unitToBuild)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralImmediateOrder"), forWhichPlayer, neutralStructure, unitToBuild);
        }

        // title = "发布中介命令(无目标)(ID)"
        public static bool IssueNeutralImmediateOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralImmediateOrderById"), forWhichPlayer, neutralStructure, unitId);
        }

        // title = "发布中介命令(指定坐标)"
        public static bool IssueNeutralPointOrder(JPlayer forWhichPlayer, JUnit neutralStructure, string unitToBuild, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralPointOrder"), forWhichPlayer, neutralStructure, unitToBuild, x, y);
        }

        // title = "发布中介命令(指定坐标)(ID)"
        public static bool IssueNeutralPointOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralPointOrderById"), forWhichPlayer, neutralStructure, unitId, x, y);
        }

        // title = "发布中介命令(指定单位)"
        public static bool IssueNeutralTargetOrder(JPlayer forWhichPlayer, JUnit neutralStructure, string unitToBuild, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralTargetOrder"), forWhichPlayer, neutralStructure, unitToBuild, target);
        }

        // title = "发布中介命令(指定单位)(ID)"
        public static bool IssueNeutralTargetOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IssueNeutralTargetOrderById"), forWhichPlayer, neutralStructure, unitId, target);
        }

        // title = "当前命令ID"
        public static int GetUnitCurrentOrder(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitCurrentOrder"), whichUnit);
        }

        // title = "设置黄金储量"
        public static void SetResourceAmount(JUnit whichUnit, int amount)
        {
            War3.CallNative(War3.GetNativeFunction("SetResourceAmount"), whichUnit, amount);
        }

        public static void AddResourceAmount(JUnit whichUnit, int amount)
        {
            War3.CallNative(War3.GetNativeFunction("AddResourceAmount"), whichUnit, amount);
        }

        // title = "储金量"
        public static int GetResourceAmount(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetResourceAmount"), whichUnit);
        }

        // title = "传送门目的地X坐标"
        public static float WaygateGetDestinationX(JUnit waygate)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("WaygateGetDestinationX"), waygate);
        }

        // title = "传送门目的地Y坐标"
        public static float WaygateGetDestinationY(JUnit waygate)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("WaygateGetDestinationY"), waygate);
        }

        // title = "设置传送门目的坐标 [R]"
        public static void WaygateSetDestination(JUnit waygate, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("WaygateSetDestination"), waygate, x, y);
        }

        public static void WaygateActivate(JUnit waygate, bool activate)
        {
            War3.CallNative(War3.GetNativeFunction("WaygateActivate"), waygate, activate);
        }

        public static bool WaygateIsActive(JUnit waygate)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("WaygateIsActive"), waygate);
        }

        // title = "添加物品(所有市场)"
        public static void AddItemToAllStock(int itemId, int currentStock, int stockMax)
        {
            War3.CallNative(War3.GetNativeFunction("AddItemToAllStock"), itemId, currentStock, stockMax);
        }

        public static void AddItemToStock(JUnit whichUnit, int itemId, int currentStock, int stockMax)
        {
            War3.CallNative(War3.GetNativeFunction("AddItemToStock"), whichUnit, itemId, currentStock, stockMax);
        }

        // title = "添加单位(所有市场)"
        public static void AddUnitToAllStock(int unitId, int currentStock, int stockMax)
        {
            War3.CallNative(War3.GetNativeFunction("AddUnitToAllStock"), unitId, currentStock, stockMax);
        }

        public static void AddUnitToStock(JUnit whichUnit, int unitId, int currentStock, int stockMax)
        {
            War3.CallNative(War3.GetNativeFunction("AddUnitToStock"), whichUnit, unitId, currentStock, stockMax);
        }

        // title = "删除物品(所有市场)"
        public static void RemoveItemFromAllStock(int itemId)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveItemFromAllStock"), itemId);
        }

        public static void RemoveItemFromStock(JUnit whichUnit, int itemId)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveItemFromStock"), whichUnit, itemId);
        }

        // title = "删除单位(所有市场)"
        public static void RemoveUnitFromAllStock(int unitId)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveUnitFromAllStock"), unitId);
        }

        public static void RemoveUnitFromStock(JUnit whichUnit, int unitId)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveUnitFromStock"), whichUnit, unitId);
        }

        // title = "限制物品种类(所有市场)"
        public static void SetAllItemTypeSlots(int slots)
        {
            War3.CallNative(War3.GetNativeFunction("SetAllItemTypeSlots"), slots);
        }

        // title = "限制单位种类(所有市场)"
        public static void SetAllUnitTypeSlots(int slots)
        {
            War3.CallNative(War3.GetNativeFunction("SetAllUnitTypeSlots"), slots);
        }

        // title = "限制物品种类(指定市场)"
        public static void SetItemTypeSlots(JUnit whichUnit, int slots)
        {
            War3.CallNative(War3.GetNativeFunction("SetItemTypeSlots"), whichUnit, slots);
        }

        // title = "限制单位种类(指定市场)"
        public static void SetUnitTypeSlots(JUnit whichUnit, int slots)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitTypeSlots"), whichUnit, slots);
        }

        // title = "单位自定义值"
        public static int GetUnitUserData(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetUnitUserData"), whichUnit);
        }

        // title = "设置自定义值"
        public static void SetUnitUserData(JUnit whichUnit, int data)
        {
            War3.CallNative(War3.GetNativeFunction("SetUnitUserData"), whichUnit, data);
        }

        public static void SetPlayerUnitsOwner(JPlayer whichPlayer, int newOwner)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerUnitsOwner"), whichPlayer, newOwner);
        }

        public static void CripplePlayer(JPlayer whichPlayer, JForce toWhichPlayers, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("CripplePlayer"), whichPlayer, toWhichPlayers, flag);
        }

        // title = "允许/禁用 技能 [R]"
        public static void SetPlayerAbilityAvailable(JPlayer whichPlayer, int abilid, bool avail)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerAbilityAvailable"), whichPlayer, abilid, avail);
        }

        // title = "设置属性"
        public static void SetPlayerState(JPlayer whichPlayer, JPlayerState whichPlayerState, int value)
        {
            War3.CallNative(War3.GetNativeFunction("SetPlayerState"), whichPlayer, whichPlayerState, value);
        }

        // title = "踢除玩家"
        public static void RemovePlayer(JPlayer whichPlayer, JPlayerGameResult gameResult)
        {
            War3.CallNative(War3.GetNativeFunction("RemovePlayer"), whichPlayer, gameResult);
        }

        public static void CachePlayerHeroData(JPlayer whichPlayer)
        {
            War3.CallNative(War3.GetNativeFunction("CachePlayerHeroData"), whichPlayer);
        }

        // title = "设置地图迷雾(矩形区域) [R]"
        public static void SetFogStateRect(JPlayer forWhichPlayer, JFogState whichState, JRect where, bool useSharedVision)
        {
            War3.CallNative(War3.GetNativeFunction("SetFogStateRect"), forWhichPlayer, whichState, where, useSharedVision);
        }

        // title = "设置地图迷雾(圆范围) [R]"
        public static void SetFogStateRadius(JPlayer forWhichPlayer, JFogState whichState, float centerx, float centerY, float radius, bool useSharedVision)
        {
            War3.CallNative(War3.GetNativeFunction("SetFogStateRadius"), forWhichPlayer, whichState, centerx, centerY, radius, useSharedVision);
        }

        public static void SetFogStateRadiusLoc(JPlayer forWhichPlayer, JFogState whichState, JLocation center, float radius, bool useSharedVision)
        {
            War3.CallNative(War3.GetNativeFunction("SetFogStateRadiusLoc"), forWhichPlayer, whichState, center, radius, useSharedVision);
        }

        // title = "启用/禁用黑色阴影 [R]"
        public static void FogMaskEnable(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("FogMaskEnable"), enable);
        }

        // title = "黑色阴影开启"
        public static bool IsFogMaskEnabled()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsFogMaskEnabled"));
        }

        // title = "启用/禁用 战争迷雾 [R]"
        public static void FogEnable(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("FogEnable"), enable);
        }

        // title = "战争迷雾开启"
        public static bool IsFogEnabled()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsFogEnabled"));
        }

        // title = "新建可见度修正器(矩形区域) [R]"
        public static JFogModifier CreateFogModifierRect(JPlayer forWhichPlayer, JFogState whichState, JRect where, bool useSharedVision, bool afterUnits)
        {
            return War3.CallNative<JFogModifier>(War3.GetNativeFunction("CreateFogModifierRect"), forWhichPlayer, whichState, where, useSharedVision, afterUnits);
        }

        // title = "新建可见度修正器(圆范围) [R]"
        public static JFogModifier CreateFogModifierRadius(JPlayer forWhichPlayer, JFogState whichState, float centerx, float centerY, float radius, bool useSharedVision, bool afterUnits)
        {
            return War3.CallNative<JFogModifier>(War3.GetNativeFunction("CreateFogModifierRadius"), forWhichPlayer, whichState, centerx, centerY, radius, useSharedVision, afterUnits);
        }

        public static JFogModifier CreateFogModifierRadiusLoc(JPlayer forWhichPlayer, JFogState whichState, JLocation center, float radius, bool useSharedVision, bool afterUnits)
        {
            return War3.CallNative<JFogModifier>(War3.GetNativeFunction("CreateFogModifierRadiusLoc"), forWhichPlayer, whichState, center, radius, useSharedVision, afterUnits);
        }

        // title = "删除可见度修正器"
        public static void DestroyFogModifier(JFogModifier whichFogModifier)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyFogModifier"), whichFogModifier);
        }

        // title = "启用可见度修正器"
        public static void FogModifierStart(JFogModifier whichFogModifier)
        {
            War3.CallNative(War3.GetNativeFunction("FogModifierStart"), whichFogModifier);
        }

        // title = "禁用可见度修正器"
        public static void FogModifierStop(JFogModifier whichFogModifier)
        {
            War3.CallNative(War3.GetNativeFunction("FogModifierStop"), whichFogModifier);
        }

        public static JVersion VersionGet()
        {
            return War3.CallNative<JVersion>(War3.GetNativeFunction("VersionGet"));
        }

        public static bool VersionCompatible(JVersion whichVersion)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("VersionCompatible"), whichVersion);
        }

        public static bool VersionSupported(JVersion whichVersion)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("VersionSupported"), whichVersion);
        }

        public static void EndGame(bool doScoreScreen)
        {
            War3.CallNative(War3.GetNativeFunction("EndGame"), doScoreScreen);
        }

        // title = "切换关卡 [R]"
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
            War3.CallNative(War3.GetNativeFunction("SetCampaignMenuRace"), r);
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

        // title = "保存进度 [R]"
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

        // title = "游戏存档存在"
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
            War3.CallNative(War3.GetNativeFunction("SetFloatGameState"), whichFloatGameState, value);
        }

        public static void SetIntegerGameState(JIGameState whichIntegerGameState, int value)
        {
            War3.CallNative(War3.GetNativeFunction("SetIntegerGameState"), whichIntegerGameState, value);
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
            War3.CallNative(War3.GetNativeFunction("SetDefaultDifficulty"), g);
        }

        public static void SetCustomCampaignButtonVisible(int whichButton, bool visible)
        {
            War3.CallNative(War3.GetNativeFunction("SetCustomCampaignButtonVisible"), whichButton, visible);
        }

        public static bool GetCustomCampaignButtonVisible(int whichButton)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetCustomCampaignButtonVisible"), whichButton);
        }

        // title = "关闭游戏录像功能 [R]"
        public static void DoNotSaveReplay()
        {
            War3.CallNative(War3.GetNativeFunction("DoNotSaveReplay"));
        }

        // title = "新建对话框 [R]"
        public static JDialog DialogCreate()
        {
            return War3.CallNative<JDialog>(War3.GetNativeFunction("DialogCreate"));
        }

        // title = "删除 [R]"
        public static void DialogDestroy(JDialog whichDialog)
        {
            War3.CallNative(War3.GetNativeFunction("DialogDestroy"), whichDialog);
        }

        public static void DialogClear(JDialog whichDialog)
        {
            War3.CallNative(War3.GetNativeFunction("DialogClear"), whichDialog);
        }

        public static void DialogSetMessage(JDialog whichDialog, string messageText)
        {
            War3.CallNative(War3.GetNativeFunction("DialogSetMessage"), whichDialog, messageText);
        }

        // title = "添加对话框按钮 [R]"
        public static JButton DialogAddButton(JDialog whichDialog, string buttonText, int hotkey)
        {
            return War3.CallNative<JButton>(War3.GetNativeFunction("DialogAddButton"), whichDialog, buttonText, hotkey);
        }

        // title = "添加退出游戏按钮 [R]"
        public static JButton DialogAddQuitButton(JDialog whichDialog, bool doScoreScreen, string buttonText, int hotkey)
        {
            return War3.CallNative<JButton>(War3.GetNativeFunction("DialogAddQuitButton"), whichDialog, doScoreScreen, buttonText, hotkey);
        }

        // title = "显示/隐藏 [R]"
        public static void DialogDisplay(JPlayer whichPlayer, JDialog whichDialog, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("DialogDisplay"), whichPlayer, whichDialog, flag);
        }

        // title = "读取本地缓存数据"
        public static bool ReloadGameCachesFromDisk()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("ReloadGameCachesFromDisk"));
        }

        // title = "新建游戏缓存 [R]"
        public static JGameCache InitGameCache(string campaignFile)
        {
            return War3.CallNative<JGameCache>(War3.GetNativeFunction("InitGameCache"), campaignFile);
        }

        public static bool SaveGameCache(JGameCache whichCache)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveGameCache"), whichCache);
        }

        // title = "记录整数"
        public static void StoreInteger(JGameCache cache, string missionKey, string key, int value)
        {
            War3.CallNative(War3.GetNativeFunction("StoreInteger"), cache, missionKey, key, value);
        }

        // title = "记录实数"
        public static void StoreReal(JGameCache cache, string missionKey, string key, float value)
        {
            War3.CallNative(War3.GetNativeFunction("StoreReal"), cache, missionKey, key, value);
        }

        // title = "记录布尔值"
        public static void StoreBoolean(JGameCache cache, string missionKey, string key, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("StoreBoolean"), cache, missionKey, key, value);
        }

        public static bool StoreUnit(JGameCache cache, string missionKey, string key, JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("StoreUnit"), cache, missionKey, key, whichUnit);
        }

        // title = "记录字符串"
        public static bool StoreString(JGameCache cache, string missionKey, string key, string value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("StoreString"), cache, missionKey, key, value);
        }

        public static void SyncStoredInteger(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredInteger"), cache, missionKey, key);
        }

        public static void SyncStoredReal(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredReal"), cache, missionKey, key);
        }

        public static void SyncStoredBoolean(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredBoolean"), cache, missionKey, key);
        }

        public static void SyncStoredUnit(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredUnit"), cache, missionKey, key);
        }

        public static void SyncStoredString(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("SyncStoredString"), cache, missionKey, key);
        }

        public static bool HaveStoredInteger(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredInteger"), cache, missionKey, key);
        }

        public static bool HaveStoredReal(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredReal"), cache, missionKey, key);
        }

        public static bool HaveStoredBoolean(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredBoolean"), cache, missionKey, key);
        }

        public static bool HaveStoredUnit(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredUnit"), cache, missionKey, key);
        }

        public static bool HaveStoredString(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveStoredString"), cache, missionKey, key);
        }

        // title = "删除缓存 [C]"
        public static void FlushGameCache(JGameCache cache)
        {
            War3.CallNative(War3.GetNativeFunction("FlushGameCache"), cache);
        }

        // title = "删除类别"
        public static void FlushStoredMission(JGameCache cache, string missionKey)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredMission"), cache, missionKey);
        }

        public static void FlushStoredInteger(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredInteger"), cache, missionKey, key);
        }

        public static void FlushStoredReal(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredReal"), cache, missionKey, key);
        }

        public static void FlushStoredBoolean(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredBoolean"), cache, missionKey, key);
        }

        public static void FlushStoredUnit(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredUnit"), cache, missionKey, key);
        }

        public static void FlushStoredString(JGameCache cache, string missionKey, string key)
        {
            War3.CallNative(War3.GetNativeFunction("FlushStoredString"), cache, missionKey, key);
        }

        // title = "缓存读取整数 [C]"
        public static int GetStoredInteger(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetStoredInteger"), cache, missionKey, key);
        }

        // title = "缓存读取实数 [C]"
        public static float GetStoredReal(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetStoredReal"), cache, missionKey, key);
        }

        // title = "读取布尔值[R]"
        public static bool GetStoredBoolean(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetStoredBoolean"), cache, missionKey, key);
        }

        // title = "读取字符串 [C]"
        public static string GetStoredString(JGameCache cache, string missionKey, string key)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetStoredString"), cache, missionKey, key);
        }

        public static JUnit RestoreUnit(JGameCache cache, string missionKey, string key, JPlayer forWhichPlayer, float x, float y, float facing)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("RestoreUnit"), cache, missionKey, key, forWhichPlayer, x, y, facing);
        }

        // title = "<1.24> 新建哈希表 [C]"
        public static JHashtable InitHashtable()
        {
            return War3.CallNative<JHashtable>(War3.GetNativeFunction("InitHashtable"));
        }

        // title = "<1.24> 保存整数 [C]"
        public static void SaveInteger(JHashtable table, int parentKey, int childKey, int value)
        {
            War3.CallNative(War3.GetNativeFunction("SaveInteger"), table, parentKey, childKey, value);
        }

        // title = "<1.24> 保存实数 [C]"
        public static void SaveReal(JHashtable table, int parentKey, int childKey, float value)
        {
            War3.CallNative(War3.GetNativeFunction("SaveReal"), table, parentKey, childKey, value);
        }

        // title = "<1.24> 保存布尔 [C]"
        public static void SaveBoolean(JHashtable table, int parentKey, int childKey, bool value)
        {
            War3.CallNative(War3.GetNativeFunction("SaveBoolean"), table, parentKey, childKey, value);
        }

        // title = "<1.24> 保存字符串 [C]"
        public static bool SaveStr(JHashtable table, int parentKey, int childKey, string value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveStr"), table, parentKey, childKey, value);
        }

        // title = "<1.24> 保存玩家 [C]"
        public static bool SavePlayerHandle(JHashtable table, int parentKey, int childKey, JPlayer whichPlayer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SavePlayerHandle"), table, parentKey, childKey, whichPlayer);
        }

        public static bool SaveWidgetHandle(JHashtable table, int parentKey, int childKey, JWidget whichWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveWidgetHandle"), table, parentKey, childKey, whichWidget);
        }

        // title = "<1.24> 保存可破坏物 [C]"
        public static bool SaveDestructableHandle(JHashtable table, int parentKey, int childKey, JDestructable whichDestructable)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveDestructableHandle"), table, parentKey, childKey, whichDestructable);
        }

        // title = "<1.24> 保存物品 [C]"
        public static bool SaveItemHandle(JHashtable table, int parentKey, int childKey, JItem whichItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveItemHandle"), table, parentKey, childKey, whichItem);
        }

        // title = "<1.24> 保存单位 [C]"
        public static bool SaveUnitHandle(JHashtable table, int parentKey, int childKey, JUnit whichUnit)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveUnitHandle"), table, parentKey, childKey, whichUnit);
        }

        public static bool SaveAbilityHandle(JHashtable table, int parentKey, int childKey, JAbility whichAbility)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveAbilityHandle"), table, parentKey, childKey, whichAbility);
        }

        // title = "<1.24> 保存计时器 [C]"
        public static bool SaveTimerHandle(JHashtable table, int parentKey, int childKey, JTimer whichTimer)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTimerHandle"), table, parentKey, childKey, whichTimer);
        }

        // title = "<1.24> 保存触发器 [C]"
        public static bool SaveTriggerHandle(JHashtable table, int parentKey, int childKey, JTrigger whichTrigger)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTriggerHandle"), table, parentKey, childKey, whichTrigger);
        }

        // title = "<1.24> 保存触发条件 [C]"
        public static bool SaveTriggerConditionHandle(JHashtable table, int parentKey, int childKey, JTriggerCondition whichTriggercondition)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTriggerConditionHandle"), table, parentKey, childKey, whichTriggercondition);
        }

        // title = "<1.24> 保存触发动作 [C]"
        public static bool SaveTriggerActionHandle(JHashtable table, int parentKey, int childKey, JTriggerAction whichTriggeraction)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTriggerActionHandle"), table, parentKey, childKey, whichTriggeraction);
        }

        // title = "<1.24> 保存触发事件 [C]"
        public static bool SaveTriggerEventHandle(JHashtable table, int parentKey, int childKey, JEvent whichEvent)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTriggerEventHandle"), table, parentKey, childKey, whichEvent);
        }

        // title = "<1.24> 保存玩家组 [C]"
        public static bool SaveForceHandle(JHashtable table, int parentKey, int childKey, JForce whichForce)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveForceHandle"), table, parentKey, childKey, whichForce);
        }

        // title = "<1.24> 保存单位组 [C]"
        public static bool SaveGroupHandle(JHashtable table, int parentKey, int childKey, JGroup whichGroup)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveGroupHandle"), table, parentKey, childKey, whichGroup);
        }

        // title = "<1.24> 保存点 [C]"
        public static bool SaveLocationHandle(JHashtable table, int parentKey, int childKey, JLocation whichLocation)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveLocationHandle"), table, parentKey, childKey, whichLocation);
        }

        // title = "<1.24> 保存区域(矩型) [C]"
        public static bool SaveRectHandle(JHashtable table, int parentKey, int childKey, JRect whichRect)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveRectHandle"), table, parentKey, childKey, whichRect);
        }

        // title = "<1.24> 保存布尔表达式 [C]"
        public static bool SaveBooleanExprHandle(JHashtable table, int parentKey, int childKey, JBoolExpr whichBoolexpr)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveBooleanExprHandle"), table, parentKey, childKey, whichBoolexpr);
        }

        // title = "<1.24> 保存音效 [C]"
        public static bool SaveSoundHandle(JHashtable table, int parentKey, int childKey, JSound whichSound)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveSoundHandle"), table, parentKey, childKey, whichSound);
        }

        // title = "<1.24> 保存特效 [C]"
        public static bool SaveEffectHandle(JHashtable table, int parentKey, int childKey, JEffect whichEffect)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveEffectHandle"), table, parentKey, childKey, whichEffect);
        }

        // title = "<1.24> 保存单位池 [C]"
        public static bool SaveUnitPoolHandle(JHashtable table, int parentKey, int childKey, JUnitPool whichUnitpool)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveUnitPoolHandle"), table, parentKey, childKey, whichUnitpool);
        }

        // title = "<1.24> 保存物品池 [C]"
        public static bool SaveItemPoolHandle(JHashtable table, int parentKey, int childKey, JItemPool whichItempool)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveItemPoolHandle"), table, parentKey, childKey, whichItempool);
        }

        // title = "<1.24> 保存任务 [C]"
        public static bool SaveQuestHandle(JHashtable table, int parentKey, int childKey, JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveQuestHandle"), table, parentKey, childKey, whichQuest);
        }

        // title = "<1.24> 保存任务要求 [C]"
        public static bool SaveQuestItemHandle(JHashtable table, int parentKey, int childKey, JQuestItem whichQuestitem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveQuestItemHandle"), table, parentKey, childKey, whichQuestitem);
        }

        // title = "<1.24> 保存失败条件 [C]"
        public static bool SaveDefeatConditionHandle(JHashtable table, int parentKey, int childKey, JDefeatCondition whichDefeatcondition)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveDefeatConditionHandle"), table, parentKey, childKey, whichDefeatcondition);
        }

        // title = "<1.24> 保存计时器窗口 [C]"
        public static bool SaveTimerDialogHandle(JHashtable table, int parentKey, int childKey, JTimerDialog whichTimerdialog)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTimerDialogHandle"), table, parentKey, childKey, whichTimerdialog);
        }

        // title = "<1.24> 保存排行榜 [C]"
        public static bool SaveLeaderboardHandle(JHashtable table, int parentKey, int childKey, JLeaderboard whichLeaderboard)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveLeaderboardHandle"), table, parentKey, childKey, whichLeaderboard);
        }

        // title = "<1.24> 保存多面板 [C]"
        public static bool SaveMultiboardHandle(JHashtable table, int parentKey, int childKey, JMultiboard whichMultiboard)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveMultiboardHandle"), table, parentKey, childKey, whichMultiboard);
        }

        // title = "<1.24> 保存多面板项目 [C]"
        public static bool SaveMultiboardItemHandle(JHashtable table, int parentKey, int childKey, JMultiboardItem whichMultiboarditem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveMultiboardItemHandle"), table, parentKey, childKey, whichMultiboarditem);
        }

        // title = "<1.24> 保存可追踪物 [C]"
        public static bool SaveTrackableHandle(JHashtable table, int parentKey, int childKey, JTrackable whichTrackable)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTrackableHandle"), table, parentKey, childKey, whichTrackable);
        }

        // title = "<1.24> 保存对话框 [C]"
        public static bool SaveDialogHandle(JHashtable table, int parentKey, int childKey, JDialog whichDialog)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveDialogHandle"), table, parentKey, childKey, whichDialog);
        }

        // title = "<1.24> 保存对话框按钮 [C]"
        public static bool SaveButtonHandle(JHashtable table, int parentKey, int childKey, JButton whichButton)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveButtonHandle"), table, parentKey, childKey, whichButton);
        }

        // title = "<1.24> 保存漂浮文字 [C]"
        public static bool SaveTextTagHandle(JHashtable table, int parentKey, int childKey, JTextTag whichTexttag)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveTextTagHandle"), table, parentKey, childKey, whichTexttag);
        }

        // title = "<1.24> 保存闪电效果 [C]"
        public static bool SaveLightningHandle(JHashtable table, int parentKey, int childKey, JLightning whichLightning)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveLightningHandle"), table, parentKey, childKey, whichLightning);
        }

        // title = "<1.24> 保存图像 [C]"
        public static bool SaveImageHandle(JHashtable table, int parentKey, int childKey, JImage whichImage)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveImageHandle"), table, parentKey, childKey, whichImage);
        }

        // title = "<1.24> 保存地面纹理变化 [C]"
        public static bool SaveUbersplatHandle(JHashtable table, int parentKey, int childKey, JUbersplat whichUbersplat)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveUbersplatHandle"), table, parentKey, childKey, whichUbersplat);
        }

        // title = "<1.24> 保存区域(不规则) [C]"
        public static bool SaveRegionHandle(JHashtable table, int parentKey, int childKey, JRegion whichRegion)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveRegionHandle"), table, parentKey, childKey, whichRegion);
        }

        // title = "<1.24> 保存迷雾状态 [C]"
        public static bool SaveFogStateHandle(JHashtable table, int parentKey, int childKey, JFogState whichFogState)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveFogStateHandle"), table, parentKey, childKey, whichFogState);
        }

        // title = "<1.24> 保存可见度修正器 [C]"
        public static bool SaveFogModifierHandle(JHashtable table, int parentKey, int childKey, JFogModifier whichFogModifier)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveFogModifierHandle"), table, parentKey, childKey, whichFogModifier);
        }

        // title = "<1.24> 保存实体对象 [C]"
        public static bool SaveAgentHandle(JHashtable table, int parentKey, int childKey, JAgent whichAgent)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveAgentHandle"), table, parentKey, childKey, whichAgent);
        }

        // title = "<1.24> 保存哈希表 [C]"
        public static bool SaveHashtableHandle(JHashtable table, int parentKey, int childKey, JHashtable whichHashtable)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SaveHashtableHandle"), table, parentKey, childKey, whichHashtable);
        }

        // title = "<1.24> 从哈希表提取整数 [C]"
        public static int LoadInteger(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("LoadInteger"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取实数 [C]"
        public static float LoadReal(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("LoadReal"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取布尔 [C]"
        public static bool LoadBoolean(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("LoadBoolean"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取字符串 [C]"
        public static string LoadStr(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("LoadStr"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取玩家 [C]"
        public static JPlayer LoadPlayerHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("LoadPlayerHandle"), table, parentKey, childKey);
        }

        public static JWidget LoadWidgetHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JWidget>(War3.GetNativeFunction("LoadWidgetHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取可破坏物 [C]"
        public static JDestructable LoadDestructableHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JDestructable>(War3.GetNativeFunction("LoadDestructableHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取物品 [C]"
        public static JItem LoadItemHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("LoadItemHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取单位 [C]"
        public static JUnit LoadUnitHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("LoadUnitHandle"), table, parentKey, childKey);
        }

        public static JAbility LoadAbilityHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JAbility>(War3.GetNativeFunction("LoadAbilityHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取计时器 [C]"
        public static JTimer LoadTimerHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTimer>(War3.GetNativeFunction("LoadTimerHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取触发器 [C]"
        public static JTrigger LoadTriggerHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTrigger>(War3.GetNativeFunction("LoadTriggerHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取触发条件 [C]"
        public static JTriggerCondition LoadTriggerConditionHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTriggerCondition>(War3.GetNativeFunction("LoadTriggerConditionHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取触发动作 [C]"
        public static JTriggerAction LoadTriggerActionHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTriggerAction>(War3.GetNativeFunction("LoadTriggerActionHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取触发事件 [C]"
        public static JEvent LoadTriggerEventHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JEvent>(War3.GetNativeFunction("LoadTriggerEventHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取玩家组 [C]"
        public static JForce LoadForceHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JForce>(War3.GetNativeFunction("LoadForceHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取单位组 [C]"
        public static JGroup LoadGroupHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JGroup>(War3.GetNativeFunction("LoadGroupHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取点 [C]"
        public static JLocation LoadLocationHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("LoadLocationHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取区域(矩型) [C]"
        public static JRect LoadRectHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JRect>(War3.GetNativeFunction("LoadRectHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取布尔表达式 [C]"
        public static JBoolExpr LoadBooleanExprHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JBoolExpr>(War3.GetNativeFunction("LoadBooleanExprHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取音效 [C]"
        public static JSound LoadSoundHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("LoadSoundHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取特效 [C]"
        public static JEffect LoadEffectHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("LoadEffectHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取单位池 [C]"
        public static JUnitPool LoadUnitPoolHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JUnitPool>(War3.GetNativeFunction("LoadUnitPoolHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取物品池 [C]"
        public static JItemPool LoadItemPoolHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JItemPool>(War3.GetNativeFunction("LoadItemPoolHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取任务 [C]"
        public static JQuest LoadQuestHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JQuest>(War3.GetNativeFunction("LoadQuestHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取任务要求 [C]"
        public static JQuestItem LoadQuestItemHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JQuestItem>(War3.GetNativeFunction("LoadQuestItemHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取失败条件 [C]"
        public static JDefeatCondition LoadDefeatConditionHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JDefeatCondition>(War3.GetNativeFunction("LoadDefeatConditionHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取计时器窗口 [C]"
        public static JTimerDialog LoadTimerDialogHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTimerDialog>(War3.GetNativeFunction("LoadTimerDialogHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取排行榜 [C]"
        public static JLeaderboard LoadLeaderboardHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JLeaderboard>(War3.GetNativeFunction("LoadLeaderboardHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取多面板 [C]"
        public static JMultiboard LoadMultiboardHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JMultiboard>(War3.GetNativeFunction("LoadMultiboardHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取多面板项目 [C]"
        public static JMultiboardItem LoadMultiboardItemHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JMultiboardItem>(War3.GetNativeFunction("LoadMultiboardItemHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取可追踪物 [C]"
        public static JTrackable LoadTrackableHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTrackable>(War3.GetNativeFunction("LoadTrackableHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取对话框 [C]"
        public static JDialog LoadDialogHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JDialog>(War3.GetNativeFunction("LoadDialogHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取对话框按钮 [C]"
        public static JButton LoadButtonHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JButton>(War3.GetNativeFunction("LoadButtonHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取漂浮文字 [C]"
        public static JTextTag LoadTextTagHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JTextTag>(War3.GetNativeFunction("LoadTextTagHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取闪电效果 [C]"
        public static JLightning LoadLightningHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JLightning>(War3.GetNativeFunction("LoadLightningHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取图象 [C]"
        public static JImage LoadImageHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JImage>(War3.GetNativeFunction("LoadImageHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取地面纹理变化 [C]"
        public static JUbersplat LoadUbersplatHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JUbersplat>(War3.GetNativeFunction("LoadUbersplatHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取区域(不规则) [C]"
        public static JRegion LoadRegionHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JRegion>(War3.GetNativeFunction("LoadRegionHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取迷雾状态 [C]"
        public static JFogState LoadFogStateHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JFogState>(War3.GetNativeFunction("LoadFogStateHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取可见度修正器 [C]"
        public static JFogModifier LoadFogModifierHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JFogModifier>(War3.GetNativeFunction("LoadFogModifierHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 从哈希表提取哈希表 [C]"
        public static JHashtable LoadHashtableHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<JHashtable>(War3.GetNativeFunction("LoadHashtableHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 哈希项存有整数值 <new>"
        public static bool HaveSavedInteger(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedInteger"), table, parentKey, childKey);
        }

        // title = "<1.24> 哈希项存有实数值 <new>"
        public static bool HaveSavedReal(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedReal"), table, parentKey, childKey);
        }

        // title = "<1.24> 哈希项存有布尔值 <new>"
        public static bool HaveSavedBoolean(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedBoolean"), table, parentKey, childKey);
        }

        // title = "<1.24> 哈希项存有字符串 <new>"
        public static bool HaveSavedString(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedString"), table, parentKey, childKey);
        }

        // title = "<1.24> 哈希项存有句柄 <new>"
        public static bool HaveSavedHandle(JHashtable table, int parentKey, int childKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("HaveSavedHandle"), table, parentKey, childKey);
        }

        // title = "清理哈希项存储的整数值 <new>"
        public static void RemoveSavedInteger(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedInteger"), table, parentKey, childKey);
        }

        // title = "清理哈希项存储的实数值 <new>"
        public static void RemoveSavedReal(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedReal"), table, parentKey, childKey);
        }

        // title = "清理哈希项存储的布尔值 <new>"
        public static void RemoveSavedBoolean(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedBoolean"), table, parentKey, childKey);
        }

        // title = "清理哈希项存储的字符串 <new>"
        public static void RemoveSavedString(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedString"), table, parentKey, childKey);
        }

        // title = "清理哈希项存储的句柄 <new>"
        public static void RemoveSavedHandle(JHashtable table, int parentKey, int childKey)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveSavedHandle"), table, parentKey, childKey);
        }

        // title = "<1.24> 清空哈希表 [C]"
        public static void FlushParentHashtable(JHashtable table)
        {
            War3.CallNative(War3.GetNativeFunction("FlushParentHashtable"), table);
        }

        // title = "<1.24> 清空哈希表主索引 [C]"
        public static void FlushChildHashtable(JHashtable table, int parentKey)
        {
            War3.CallNative(War3.GetNativeFunction("FlushChildHashtable"), table, parentKey);
        }

        // title = "随机整数"
        public static int GetRandomInt(int lowBound, int highBound)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetRandomInt"), lowBound, highBound);
        }

        // title = "随机实数"
        public static float GetRandomReal(float lowBound, float highBound)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetRandomReal"), lowBound, highBound);
        }

        // title = "新建单位池 [R]"
        public static JUnitPool CreateUnitPool()
        {
            return War3.CallNative<JUnitPool>(War3.GetNativeFunction("CreateUnitPool"));
        }

        // title = "删除单位池 [R]"
        public static void DestroyUnitPool(JUnitPool whichPool)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyUnitPool"), whichPool);
        }

        // title = "添加单位类型 [R]"
        public static void UnitPoolAddUnitType(JUnitPool whichPool, int unitId, float weight)
        {
            War3.CallNative(War3.GetNativeFunction("UnitPoolAddUnitType"), whichPool, unitId, weight);
        }

        // title = "删除单位类型 [R]"
        public static void UnitPoolRemoveUnitType(JUnitPool whichPool, int unitId)
        {
            War3.CallNative(War3.GetNativeFunction("UnitPoolRemoveUnitType"), whichPool, unitId);
        }

        // title = "选择放置单位 [R]"
        public static JUnit PlaceRandomUnit(JUnitPool whichPool, JPlayer forWhichPlayer, float x, float y, float facing)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("PlaceRandomUnit"), whichPool, forWhichPlayer, x, y, facing);
        }

        // title = "新建物品池 [R]"
        public static JItemPool CreateItemPool()
        {
            return War3.CallNative<JItemPool>(War3.GetNativeFunction("CreateItemPool"));
        }

        // title = "删除物品池 [R]"
        public static void DestroyItemPool(JItemPool whichItemPool)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyItemPool"), whichItemPool);
        }

        // title = "添加物品类型 [R]"
        public static void ItemPoolAddItemType(JItemPool whichItemPool, int itemId, float weight)
        {
            War3.CallNative(War3.GetNativeFunction("ItemPoolAddItemType"), whichItemPool, itemId, weight);
        }

        // title = "删除物品类型 [R]"
        public static void ItemPoolRemoveItemType(JItemPool whichItemPool, int itemId)
        {
            War3.CallNative(War3.GetNativeFunction("ItemPoolRemoveItemType"), whichItemPool, itemId);
        }

        // title = "选择放置物品 [R]"
        public static JItem PlaceRandomItem(JItemPool whichItemPool, float x, float y)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("PlaceRandomItem"), whichItemPool, x, y);
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
            return War3.CallNative<int>(War3.GetNativeFunction("ChooseRandomItemEx"), whichType, level);
        }

        // title = "设置随机种子"
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

        // title = "设置迷雾 [R]"
        public static void SetTerrainFogEx(int style, float zstart, float zend, float density, float red, float green, float blue)
        {
            War3.CallNative(War3.GetNativeFunction("SetTerrainFogEx"), style, zstart, zend, density, red, green, blue);
        }

        // title = "对玩家显示文本消息(自动限时) [R]"
        public static void DisplayTextToPlayer(JPlayer toPlayer, float x, float y, string message)
        {
            War3.CallNative(War3.GetNativeFunction("DisplayTextToPlayer"), toPlayer, x, y, message);
        }

        // title = "对玩家显示文本消息(指定时间) [R]"
        public static void DisplayTimedTextToPlayer(JPlayer toPlayer, float x, float y, float duration, string message)
        {
            War3.CallNative(War3.GetNativeFunction("DisplayTimedTextToPlayer"), toPlayer, x, y, duration, message);
        }

        public static void DisplayTimedTextFromPlayer(JPlayer toPlayer, float x, float y, float duration, string message)
        {
            War3.CallNative(War3.GetNativeFunction("DisplayTimedTextFromPlayer"), toPlayer, x, y, duration, message);
        }

        // title = "清空文本信息(所有玩家) [R]"
        public static void ClearTextMessages()
        {
            War3.CallNative(War3.GetNativeFunction("ClearTextMessages"));
        }

        public static void SetDayNightModels(string terrainDNCFile, string unitDNCFile)
        {
            War3.CallNative(War3.GetNativeFunction("SetDayNightModels"), terrainDNCFile, unitDNCFile);
        }

        // title = "设置天空"
        public static void SetSkyModel(string skyModelFile)
        {
            War3.CallNative(War3.GetNativeFunction("SetSkyModel"), skyModelFile);
        }

        // title = "启用/禁用玩家控制权(所有玩家) [R]"
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

        // title = "设置昼夜时间流逝速度 [R]"
        public static void SetTimeOfDayScale(float r)
        {
            War3.CallNative(War3.GetNativeFunction("SetTimeOfDayScale"), r);
        }

        public static float GetTimeOfDayScale()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetTimeOfDayScale"));
        }

        // title = "开启/关闭 信箱模式(所有玩家) [R]"
        public static void ShowInterface(bool flag, float fadeDuration)
        {
            War3.CallNative(War3.GetNativeFunction("ShowInterface"), flag, fadeDuration);
        }

        // title = "暂停/恢复游戏 [R]"
        public static void PauseGame(bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("PauseGame"), flag);
        }

        // title = "闪动指示器(对单位) [R]"
        public static void UnitAddIndicator(JUnit whichUnit, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("UnitAddIndicator"), whichUnit, red, green, blue, alpha);
        }

        public static void AddIndicator(JWidget whichWidget, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("AddIndicator"), whichWidget, red, green, blue, alpha);
        }

        // title = "小地图信号(所有玩家) [R]"
        public static void PingMinimap(float x, float y, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("PingMinimap"), x, y, duration);
        }

        // title = "小地图信号(指定颜色)(所有玩家) [R]"
        public static void PingMinimapEx(float x, float y, float duration, int red, int green, int blue, bool extraEffects)
        {
            War3.CallNative(War3.GetNativeFunction("PingMinimapEx"), x, y, duration, red, green, blue, extraEffects);
        }

        // title = "允许/禁止闭塞(所有玩家) [R]"
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

        // title = "允许/禁止 边界染色(所有玩家) [R]"
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

        // title = "设置小地图特殊标志"
        public static void SetAltMinimapIcon(string iconPath)
        {
            War3.CallNative(War3.GetNativeFunction("SetAltMinimapIcon"), iconPath);
        }

        // title = "禁用 重新开始任务按钮"
        public static void DisableRestartMission(bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("DisableRestartMission"), flag);
        }

        // title = "新建漂浮文字 [R]"
        public static JTextTag CreateTextTag()
        {
            return War3.CallNative<JTextTag>(War3.GetNativeFunction("CreateTextTag"));
        }

        public static void DestroyTextTag(JTextTag t)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyTextTag"), t);
        }

        // title = "改变文字内容 [R]"
        public static void SetTextTagText(JTextTag t, string s, float height)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagText"), t, s, height);
        }

        // title = "改变位置(坐标) [R]"
        public static void SetTextTagPos(JTextTag t, float x, float y, float heightOffset)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagPos"), t, x, y, heightOffset);
        }

        public static void SetTextTagPosUnit(JTextTag t, JUnit whichUnit, float heightOffset)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagPosUnit"), t, whichUnit, heightOffset);
        }

        // title = "改变颜色 [R]"
        public static void SetTextTagColor(JTextTag t, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagColor"), t, red, green, blue, alpha);
        }

        // title = "设置速率 [R]"
        public static void SetTextTagVelocity(JTextTag t, float xvel, float yvel)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagVelocity"), t, xvel, yvel);
        }

        // title = "显示/隐藏 (所有玩家) [R]"
        public static void SetTextTagVisibility(JTextTag t, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagVisibility"), t, flag);
        }

        public static void SetTextTagSuspended(JTextTag t, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagSuspended"), t, flag);
        }

        public static void SetTextTagPermanent(JTextTag t, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagPermanent"), t, flag);
        }

        public static void SetTextTagAge(JTextTag t, float age)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagAge"), t, age);
        }

        public static void SetTextTagLifespan(JTextTag t, float lifespan)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagLifespan"), t, lifespan);
        }

        public static void SetTextTagFadepoint(JTextTag t, float fadepoint)
        {
            War3.CallNative(War3.GetNativeFunction("SetTextTagFadepoint"), t, fadepoint);
        }

        // title = "保留英雄图标"
        public static void SetReservedLocalHeroButtons(int reserved)
        {
            War3.CallNative(War3.GetNativeFunction("SetReservedLocalHeroButtons"), reserved);
        }

        // title = "联盟颜色显示设置"
        public static int GetAllyColorFilterState()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetAllyColorFilterState"));
        }

        // title = "设置联盟颜色显示"
        public static void SetAllyColorFilterState(int state)
        {
            War3.CallNative(War3.GetNativeFunction("SetAllyColorFilterState"), state);
        }

        // title = "小地图中立生物显示开启"
        public static bool GetCreepCampFilterState()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetCreepCampFilterState"));
        }

        // title = "设置小地图中立生物显示"
        public static void SetCreepCampFilterState(bool state)
        {
            War3.CallNative(War3.GetNativeFunction("SetCreepCampFilterState"), state);
        }

        // title = "允许/禁用小地图按钮"
        public static void EnableMinimapFilterButtons(bool enableAlly, bool enableCreep)
        {
            War3.CallNative(War3.GetNativeFunction("EnableMinimapFilterButtons"), enableAlly, enableCreep);
        }

        // title = "允许/禁用框选"
        public static void EnableDragSelect(bool state, bool ui)
        {
            War3.CallNative(War3.GetNativeFunction("EnableDragSelect"), state, ui);
        }

        // title = "允许/禁用预选"
        public static void EnablePreSelect(bool state, bool ui)
        {
            War3.CallNative(War3.GetNativeFunction("EnablePreSelect"), state, ui);
        }

        // title = "允许/禁用选择"
        public static void EnableSelect(bool state, bool ui)
        {
            War3.CallNative(War3.GetNativeFunction("EnableSelect"), state, ui);
        }

        // title = "新建可追踪物 [R]"
        public static JTrackable CreateTrackable(string trackableModelPath, float x, float y, float facing)
        {
            return War3.CallNative<JTrackable>(War3.GetNativeFunction("CreateTrackable"), trackableModelPath, x, y, facing);
        }

        // title = "新建任务 [R]"
        public static JQuest CreateQuest()
        {
            return War3.CallNative<JQuest>(War3.GetNativeFunction("CreateQuest"));
        }

        public static void DestroyQuest(JQuest whichQuest)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyQuest"), whichQuest);
        }

        public static void QuestSetTitle(JQuest whichQuest, string title)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetTitle"), whichQuest, title);
        }

        public static void QuestSetDescription(JQuest whichQuest, string description)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetDescription"), whichQuest, description);
        }

        public static void QuestSetIconPath(JQuest whichQuest, string iconPath)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetIconPath"), whichQuest, iconPath);
        }

        public static void QuestSetRequired(JQuest whichQuest, bool required)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetRequired"), whichQuest, required);
        }

        public static void QuestSetCompleted(JQuest whichQuest, bool completed)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetCompleted"), whichQuest, completed);
        }

        public static void QuestSetDiscovered(JQuest whichQuest, bool discovered)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetDiscovered"), whichQuest, discovered);
        }

        public static void QuestSetFailed(JQuest whichQuest, bool failed)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetFailed"), whichQuest, failed);
        }

        // title = "启用/禁用 任务 [R]"
        public static void QuestSetEnabled(JQuest whichQuest, bool enabled)
        {
            War3.CallNative(War3.GetNativeFunction("QuestSetEnabled"), whichQuest, enabled);
        }

        // title = "是主要任务"
        public static bool IsQuestRequired(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestRequired"), whichQuest);
        }

        // title = "任务完成"
        public static bool IsQuestCompleted(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestCompleted"), whichQuest);
        }

        // title = "任务被发现"
        public static bool IsQuestDiscovered(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestDiscovered"), whichQuest);
        }

        // title = "任务失败"
        public static bool IsQuestFailed(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestFailed"), whichQuest);
        }

        // title = "任务激活"
        public static bool IsQuestEnabled(JQuest whichQuest)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestEnabled"), whichQuest);
        }

        public static JQuestItem QuestCreateItem(JQuest whichQuest)
        {
            return War3.CallNative<JQuestItem>(War3.GetNativeFunction("QuestCreateItem"), whichQuest);
        }

        public static void QuestItemSetDescription(JQuestItem whichQuestItem, string description)
        {
            War3.CallNative(War3.GetNativeFunction("QuestItemSetDescription"), whichQuestItem, description);
        }

        public static void QuestItemSetCompleted(JQuestItem whichQuestItem, bool completed)
        {
            War3.CallNative(War3.GetNativeFunction("QuestItemSetCompleted"), whichQuestItem, completed);
        }

        // title = "任务项目完成"
        public static bool IsQuestItemCompleted(JQuestItem whichQuestItem)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsQuestItemCompleted"), whichQuestItem);
        }

        public static JDefeatCondition CreateDefeatCondition()
        {
            return War3.CallNative<JDefeatCondition>(War3.GetNativeFunction("CreateDefeatCondition"));
        }

        public static void DestroyDefeatCondition(JDefeatCondition whichCondition)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyDefeatCondition"), whichCondition);
        }

        public static void DefeatConditionSetDescription(JDefeatCondition whichCondition, string description)
        {
            War3.CallNative(War3.GetNativeFunction("DefeatConditionSetDescription"), whichCondition, description);
        }

        public static void FlashQuestDialogButton()
        {
            War3.CallNative(War3.GetNativeFunction("FlashQuestDialogButton"));
        }

        public static void ForceQuestDialogUpdate()
        {
            War3.CallNative(War3.GetNativeFunction("ForceQuestDialogUpdate"));
        }

        // title = "新建计时器窗口 [R]"
        public static JTimerDialog CreateTimerDialog(JTimer t)
        {
            return War3.CallNative<JTimerDialog>(War3.GetNativeFunction("CreateTimerDialog"), t);
        }

        public static void DestroyTimerDialog(JTimerDialog whichDialog)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyTimerDialog"), whichDialog);
        }

        public static void TimerDialogSetTitle(JTimerDialog whichDialog, string title)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetTitle"), whichDialog, title);
        }

        // title = "改变计时器窗口文字颜色 [R]"
        public static void TimerDialogSetTitleColor(JTimerDialog whichDialog, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetTitleColor"), whichDialog, red, green, blue, alpha);
        }

        // title = "改变计时器窗口计时颜色 [R]"
        public static void TimerDialogSetTimeColor(JTimerDialog whichDialog, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetTimeColor"), whichDialog, red, green, blue, alpha);
        }

        // title = "设置计时器窗口速率 [R]"
        public static void TimerDialogSetSpeed(JTimerDialog whichDialog, float speedMultFactor)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetSpeed"), whichDialog, speedMultFactor);
        }

        // title = "显示/隐藏 计时器窗口(所有玩家) [R]"
        public static void TimerDialogDisplay(JTimerDialog whichDialog, bool display)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogDisplay"), whichDialog, display);
        }

        public static bool IsTimerDialogDisplayed(JTimerDialog whichDialog)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsTimerDialogDisplayed"), whichDialog);
        }

        public static void TimerDialogSetRealTimeRemaining(JTimerDialog whichDialog, float timeRemaining)
        {
            War3.CallNative(War3.GetNativeFunction("TimerDialogSetRealTimeRemaining"), whichDialog, timeRemaining);
        }

        // title = "新建排行榜 [R]"
        public static JLeaderboard CreateLeaderboard()
        {
            return War3.CallNative<JLeaderboard>(War3.GetNativeFunction("CreateLeaderboard"));
        }

        public static void DestroyLeaderboard(JLeaderboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyLeaderboard"), lb);
        }

        // title = "显示/隐藏 [R]"
        public static void LeaderboardDisplay(JLeaderboard lb, bool show)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardDisplay"), lb, show);
        }

        public static bool IsLeaderboardDisplayed(JLeaderboard lb)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsLeaderboardDisplayed"), lb);
        }

        // title = "行数"
        public static int LeaderboardGetItemCount(JLeaderboard lb)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("LeaderboardGetItemCount"), lb);
        }

        public static void LeaderboardSetSizeByItemCount(JLeaderboard lb, int count)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetSizeByItemCount"), lb, count);
        }

        public static void LeaderboardAddItem(JLeaderboard lb, string label, int value, JPlayer p)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardAddItem"), lb, label, value, p);
        }

        public static void LeaderboardRemoveItem(JLeaderboard lb, int index)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardRemoveItem"), lb, index);
        }

        public static void LeaderboardRemovePlayerItem(JLeaderboard lb, JPlayer p)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardRemovePlayerItem"), lb, p);
        }

        // title = "清空 [R]"
        public static void LeaderboardClear(JLeaderboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardClear"), lb);
        }

        public static void LeaderboardSortItemsByValue(JLeaderboard lb, bool ascending)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSortItemsByValue"), lb, ascending);
        }

        public static void LeaderboardSortItemsByPlayer(JLeaderboard lb, bool ascending)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSortItemsByPlayer"), lb, ascending);
        }

        public static void LeaderboardSortItemsByLabel(JLeaderboard lb, bool ascending)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSortItemsByLabel"), lb, ascending);
        }

        public static bool LeaderboardHasPlayerItem(JLeaderboard lb, JPlayer p)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("LeaderboardHasPlayerItem"), lb, p);
        }

        public static int LeaderboardGetPlayerIndex(JLeaderboard lb, JPlayer p)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("LeaderboardGetPlayerIndex"), lb, p);
        }

        public static void LeaderboardSetLabel(JLeaderboard lb, string label)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetLabel"), lb, label);
        }

        public static string LeaderboardGetLabelText(JLeaderboard lb)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("LeaderboardGetLabelText"), lb);
        }

        // title = "设置玩家使用的排行榜 [R]"
        public static void PlayerSetLeaderboard(JPlayer toPlayer, JLeaderboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("PlayerSetLeaderboard"), toPlayer, lb);
        }

        public static JLeaderboard PlayerGetLeaderboard(JPlayer toPlayer)
        {
            return War3.CallNative<JLeaderboard>(War3.GetNativeFunction("PlayerGetLeaderboard"), toPlayer);
        }

        // title = "设置文字颜色 [R]"
        public static void LeaderboardSetLabelColor(JLeaderboard lb, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetLabelColor"), lb, red, green, blue, alpha);
        }

        // title = "设置数值颜色 [R]"
        public static void LeaderboardSetValueColor(JLeaderboard lb, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetValueColor"), lb, red, green, blue, alpha);
        }

        public static void LeaderboardSetStyle(JLeaderboard lb, bool showLabel, bool showNames, bool showValues, bool showIcons)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetStyle"), lb, showLabel, showNames, showValues, showIcons);
        }

        public static void LeaderboardSetItemValue(JLeaderboard lb, int whichItem, int val)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemValue"), lb, whichItem, val);
        }

        public static void LeaderboardSetItemLabel(JLeaderboard lb, int whichItem, string val)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemLabel"), lb, whichItem, val);
        }

        public static void LeaderboardSetItemStyle(JLeaderboard lb, int whichItem, bool showLabel, bool showValue, bool showIcon)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemStyle"), lb, whichItem, showLabel, showValue, showIcon);
        }

        public static void LeaderboardSetItemLabelColor(JLeaderboard lb, int whichItem, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemLabelColor"), lb, whichItem, red, green, blue, alpha);
        }

        public static void LeaderboardSetItemValueColor(JLeaderboard lb, int whichItem, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("LeaderboardSetItemValueColor"), lb, whichItem, red, green, blue, alpha);
        }

        // title = "新建多面板 [R]"
        public static JMultiboard CreateMultiboard()
        {
            return War3.CallNative<JMultiboard>(War3.GetNativeFunction("CreateMultiboard"));
        }

        public static void DestroyMultiboard(JMultiboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyMultiboard"), lb);
        }

        // title = "显示/隐藏 [R]"
        public static void MultiboardDisplay(JMultiboard lb, bool show)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardDisplay"), lb, show);
        }

        // title = "多面板显示"
        public static bool IsMultiboardDisplayed(JMultiboard lb)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsMultiboardDisplayed"), lb);
        }

        // title = "最大/最小化 [R]"
        public static void MultiboardMinimize(JMultiboard lb, bool minimize)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardMinimize"), lb, minimize);
        }

        // title = "多面板最小化"
        public static bool IsMultiboardMinimized(JMultiboard lb)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsMultiboardMinimized"), lb);
        }

        // title = "清空多面板"
        public static void MultiboardClear(JMultiboard lb)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardClear"), lb);
        }

        // title = "设置标题"
        public static void MultiboardSetTitleText(JMultiboard lb, string label)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetTitleText"), lb, label);
        }

        // title = "多面板标题"
        public static string MultiboardGetTitleText(JMultiboard lb)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("MultiboardGetTitleText"), lb);
        }

        // title = "设置标题颜色 [R]"
        public static void MultiboardSetTitleTextColor(JMultiboard lb, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetTitleTextColor"), lb, red, green, blue, alpha);
        }

        // title = "行数"
        public static int MultiboardGetRowCount(JMultiboard lb)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("MultiboardGetRowCount"), lb);
        }

        // title = "列数"
        public static int MultiboardGetColumnCount(JMultiboard lb)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("MultiboardGetColumnCount"), lb);
        }

        // title = "设置列数"
        public static void MultiboardSetColumnCount(JMultiboard lb, int count)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetColumnCount"), lb, count);
        }

        // title = "设置行数"
        public static void MultiboardSetRowCount(JMultiboard lb, int count)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetRowCount"), lb, count);
        }

        // title = "设置所有项目显示风格 [R]"
        public static void MultiboardSetItemsStyle(JMultiboard lb, bool showValues, bool showIcons)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsStyle"), lb, showValues, showIcons);
        }

        // title = "设置所有项目文本 [R]"
        public static void MultiboardSetItemsValue(JMultiboard lb, string value)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsValue"), lb, value);
        }

        // title = "设置所有项目颜色 [R]"
        public static void MultiboardSetItemsValueColor(JMultiboard lb, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsValueColor"), lb, red, green, blue, alpha);
        }

        // title = "设置所有项目宽度 [R]"
        public static void MultiboardSetItemsWidth(JMultiboard lb, float width)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsWidth"), lb, width);
        }

        // title = "设置所有项目图标 [R]"
        public static void MultiboardSetItemsIcon(JMultiboard lb, string iconPath)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemsIcon"), lb, iconPath);
        }

        // title = "多面板项目 [R]"
        public static JMultiboardItem MultiboardGetItem(JMultiboard lb, int row, int column)
        {
            return War3.CallNative<JMultiboardItem>(War3.GetNativeFunction("MultiboardGetItem"), lb, row, column);
        }

        // title = "删除多面板项目 [R]"
        public static void MultiboardReleaseItem(JMultiboardItem mbi)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardReleaseItem"), mbi);
        }

        // title = "设置指定项目显示风格 [R]"
        public static void MultiboardSetItemStyle(JMultiboardItem mbi, bool showValue, bool showIcon)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemStyle"), mbi, showValue, showIcon);
        }

        // title = "设置指定项目文本 [R]"
        public static void MultiboardSetItemValue(JMultiboardItem mbi, string val)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemValue"), mbi, val);
        }

        // title = "设置指定项目颜色 [R]"
        public static void MultiboardSetItemValueColor(JMultiboardItem mbi, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemValueColor"), mbi, red, green, blue, alpha);
        }

        // title = "设置指定项目宽度 [R]"
        public static void MultiboardSetItemWidth(JMultiboardItem mbi, float width)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemWidth"), mbi, width);
        }

        // title = "设置指定项目图标 [R]"
        public static void MultiboardSetItemIcon(JMultiboardItem mbi, string iconFileName)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSetItemIcon"), mbi, iconFileName);
        }

        // title = "显示/隐藏多面板模式 [R]"
        public static void MultiboardSuppressDisplay(bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("MultiboardSuppressDisplay"), flag);
        }

        public static void SetCameraPosition(float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraPosition"), x, y);
        }

        // title = "设置空格键转向点(所有玩家) [R]"
        public static void SetCameraQuickPosition(float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraQuickPosition"), x, y);
        }

        // title = "设置可用镜头区域(所有玩家) [R]"
        public static void SetCameraBounds(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraBounds"), x1, y1, x2, y2, x3, y3, x4, y4);
        }

        // title = "停止播放镜头(所有玩家) [R]"
        public static void StopCamera()
        {
            War3.CallNative(War3.GetNativeFunction("StopCamera"));
        }

        // title = "重置游戏镜头(所有玩家) [R]"
        public static void ResetToGameCamera(float duration)
        {
            War3.CallNative(War3.GetNativeFunction("ResetToGameCamera"), duration);
        }

        public static void PanCameraTo(float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("PanCameraTo"), x, y);
        }

        // title = "平移镜头(所有玩家)(限时) [R]"
        public static void PanCameraToTimed(float x, float y, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("PanCameraToTimed"), x, y, duration);
        }

        public static void PanCameraToWithZ(float x, float y, float zOffsetDest)
        {
            War3.CallNative(War3.GetNativeFunction("PanCameraToWithZ"), x, y, zOffsetDest);
        }

        // title = "指定高度平移镜头(所有玩家)(限时) [R]"
        public static void PanCameraToTimedWithZ(float x, float y, float zOffsetDest, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("PanCameraToTimedWithZ"), x, y, zOffsetDest, duration);
        }

        // title = "播放电影镜头(所有玩家) [R]"
        public static void SetCinematicCamera(string cameraModelFile)
        {
            War3.CallNative(War3.GetNativeFunction("SetCinematicCamera"), cameraModelFile);
        }

        // title = "指定点旋转镜头(所有玩家)(弧度)(限时) [R]"
        public static void SetCameraRotateMode(float x, float y, float radiansToSweep, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraRotateMode"), x, y, radiansToSweep, duration);
        }

        // title = "设置镜头属性(所有玩家)(限时) [R]"
        public static void SetCameraField(JCameraField whichField, float value, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraField"), whichField, value, duration);
        }

        public static void AdjustCameraField(JCameraField whichField, float offset, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("AdjustCameraField"), whichField, offset, duration);
        }

        // title = "锁定镜头到单位(所有玩家) [R]"
        public static void SetCameraTargetController(JUnit whichUnit, float xoffset, float yoffset, bool inheritOrientation)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraTargetController"), whichUnit, xoffset, yoffset, inheritOrientation);
        }

        // title = "锁定镜头到单位(固定镜头源)(所有玩家) [R]"
        public static void SetCameraOrientController(JUnit whichUnit, float xoffset, float yoffset)
        {
            War3.CallNative(War3.GetNativeFunction("SetCameraOrientController"), whichUnit, xoffset, yoffset);
        }

        public static JCameraSetup CreateCameraSetup()
        {
            return War3.CallNative<JCameraSetup>(War3.GetNativeFunction("CreateCameraSetup"));
        }

        public static void CameraSetupSetField(JCameraSetup whichSetup, JCameraField whichField, float value, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupSetField"), whichSetup, whichField, value, duration);
        }

        // title = "镜头属性(指定镜头) [R]"
        public static float CameraSetupGetField(JCameraSetup whichSetup, JCameraField whichField)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("CameraSetupGetField"), whichSetup, whichField);
        }

        public static void CameraSetupSetDestPosition(JCameraSetup whichSetup, float x, float y, float duration)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupSetDestPosition"), whichSetup, x, y, duration);
        }

        // title = "镜头目标点"
        public static JLocation CameraSetupGetDestPositionLoc(JCameraSetup whichSetup)
        {
            return War3.CallNative<JLocation>(War3.GetNativeFunction("CameraSetupGetDestPositionLoc"), whichSetup);
        }

        public static float CameraSetupGetDestPositionX(JCameraSetup whichSetup)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("CameraSetupGetDestPositionX"), whichSetup);
        }

        public static float CameraSetupGetDestPositionY(JCameraSetup whichSetup)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("CameraSetupGetDestPositionY"), whichSetup);
        }

        public static void CameraSetupApply(JCameraSetup whichSetup, bool doPan, bool panTimed)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupApply"), whichSetup, doPan, panTimed);
        }

        public static void CameraSetupApplyWithZ(JCameraSetup whichSetup, float zDestOffset)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupApplyWithZ"), whichSetup, zDestOffset);
        }

        // title = "应用镜头(所有玩家)(限时) [R]"
        public static void CameraSetupApplyForceDuration(JCameraSetup whichSetup, bool doPan, float forceDuration)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupApplyForceDuration"), whichSetup, doPan, forceDuration);
        }

        public static void CameraSetupApplyForceDurationWithZ(JCameraSetup whichSetup, float zDestOffset, float forceDuration)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetupApplyForceDurationWithZ"), whichSetup, zDestOffset, forceDuration);
        }

        public static void CameraSetTargetNoise(float mag, float velocity)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetTargetNoise"), mag, velocity);
        }

        public static void CameraSetSourceNoise(float mag, float velocity)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetSourceNoise"), mag, velocity);
        }

        // title = "摇晃镜头目标(所有玩家) [R]"
        public static void CameraSetTargetNoiseEx(float mag, float velocity, bool vertOnly)
        {
            War3.CallNative(War3.GetNativeFunction("CameraSetTargetNoiseEx"), mag, velocity, vertOnly);
        }

        // title = "摇晃镜头源(所有玩家) [R]"
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
            War3.CallNative(War3.GetNativeFunction("SetCineFilterBlendMode"), whichMode);
        }

        public static void SetCineFilterTexMapFlags(JTexMapFlags whichFlags)
        {
            War3.CallNative(War3.GetNativeFunction("SetCineFilterTexMapFlags"), whichFlags);
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
            War3.CallNative(War3.GetNativeFunction("SetCinematicScene"), portraitUnitId, color, speakerTitle, text, sceneDuration, voiceoverDuration);
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

        // title = "设置环境音效 [new]"
        public static void NewSoundEnvironment(string environmentName)
        {
            War3.CallNative(War3.GetNativeFunction("NewSoundEnvironment"), environmentName);
        }

        // title = "创建声音 [new]"
        public static JSound CreateSound(string fileName, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate, int fadeOutRate, string eaxSetting)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("CreateSound"), fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, eaxSetting);
        }

        // title = "根据文件名和标签创建声音 [new]"
        public static JSound CreateSoundFilenameWithLabel(string fileName, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate, int fadeOutRate, string SLKEntryName)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("CreateSoundFilenameWithLabel"), fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, SLKEntryName);
        }

        // title = "从标签创建声音 [new]"
        public static JSound CreateSoundFromLabel(string soundLabel, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate, int fadeOutRate)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("CreateSoundFromLabel"), soundLabel, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate);
        }

        // title = "创建MIDI音乐 [new]"
        public static JSound CreateMIDISound(string soundLabel, int fadeInRate, int fadeOutRate)
        {
            return War3.CallNative<JSound>(War3.GetNativeFunction("CreateMIDISound"), soundLabel, fadeInRate, fadeOutRate);
        }

        // title = "设置声音标签 [new]"
        public static void SetSoundParamsFromLabel(JSound soundHandle, string soundLabel)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundParamsFromLabel"), soundHandle, soundLabel);
        }

        public static void SetSoundDistanceCutoff(JSound soundHandle, float cutoff)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundDistanceCutoff"), soundHandle, cutoff);
        }

        // title = "设置声音的声道 [new]"
        public static void SetSoundChannel(JSound soundHandle, int channel)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundChannel"), soundHandle, channel);
        }

        // title = "设置音效音量 [R]"
        public static void SetSoundVolume(JSound soundHandle, int volume)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundVolume"), soundHandle, volume);
        }

        public static void SetSoundPitch(JSound soundHandle, float pitch)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundPitch"), soundHandle, pitch);
        }

        // title = "设置音效播放时间点 [R]"
        public static void SetSoundPlayPosition(JSound soundHandle, int millisecs)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundPlayPosition"), soundHandle, millisecs);
        }

        // title = "设置3D音效衰减范围"
        public static void SetSoundDistances(JSound soundHandle, float minDist, float maxDist)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundDistances"), soundHandle, minDist, maxDist);
        }

        // title = "设置声音的角度 [new]"
        public static void SetSoundConeAngles(JSound soundHandle, float inside, float outside, int outsideVolume)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundConeAngles"), soundHandle, inside, outside, outsideVolume);
        }

        // title = "设置声音的传播方向 [new]"
        public static void SetSoundConeOrientation(JSound soundHandle, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundConeOrientation"), soundHandle, x, y, z);
        }

        // title = "设置3D音效位置(指定坐标) [R]"
        public static void SetSoundPosition(JSound soundHandle, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundPosition"), soundHandle, x, y, z);
        }

        // title = "设置声音的速度 [new]"
        public static void SetSoundVelocity(JSound soundHandle, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundVelocity"), soundHandle, x, y, z);
        }

        public static void AttachSoundToUnit(JSound soundHandle, JUnit whichUnit)
        {
            War3.CallNative(War3.GetNativeFunction("AttachSoundToUnit"), soundHandle, whichUnit);
        }

        public static void StartSound(JSound soundHandle)
        {
            War3.CallNative(War3.GetNativeFunction("StartSound"), soundHandle);
        }

        public static void StopSound(JSound soundHandle, bool killWhenDone, bool fadeOut)
        {
            War3.CallNative(War3.GetNativeFunction("StopSound"), soundHandle, killWhenDone, fadeOut);
        }

        public static void KillSoundWhenDone(JSound soundHandle)
        {
            War3.CallNative(War3.GetNativeFunction("KillSoundWhenDone"), soundHandle);
        }

        // title = "设置背景音乐列表 [R]"
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

        // title = "播放主题音乐 [C]"
        public static void PlayThematicMusic(string musicFileName)
        {
            War3.CallNative(War3.GetNativeFunction("PlayThematicMusic"), musicFileName);
        }

        // title = "跳播主题音乐 [R]"
        public static void PlayThematicMusicEx(string musicFileName, int frommsecs)
        {
            War3.CallNative(War3.GetNativeFunction("PlayThematicMusicEx"), musicFileName, frommsecs);
        }

        // title = "停止主题音乐[C]"
        public static void EndThematicMusic()
        {
            War3.CallNative(War3.GetNativeFunction("EndThematicMusic"));
        }

        // title = "设置背景音乐音量 [R]"
        public static void SetMusicVolume(int volume)
        {
            War3.CallNative(War3.GetNativeFunction("SetMusicVolume"), volume);
        }

        // title = "设置背景音乐播放时间点 [R]"
        public static void SetMusicPlayPosition(int millisecs)
        {
            War3.CallNative(War3.GetNativeFunction("SetMusicPlayPosition"), millisecs);
        }

        // title = "设置主题音乐播放时间点 [R]"
        public static void SetThematicMusicPlayPosition(int millisecs)
        {
            War3.CallNative(War3.GetNativeFunction("SetThematicMusicPlayPosition"), millisecs);
        }

        public static void SetSoundDuration(JSound soundHandle, int duration)
        {
            War3.CallNative(War3.GetNativeFunction("SetSoundDuration"), soundHandle, duration);
        }

        public static int GetSoundDuration(JSound soundHandle)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetSoundDuration"), soundHandle);
        }

        public static int GetSoundFileDuration(string musicFileName)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetSoundFileDuration"), musicFileName);
        }

        // title = "设置多通道音量 [R]"
        public static void VolumeGroupSetVolume(JVolumeGroup vgroup, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("VolumeGroupSetVolume"), vgroup, scale);
        }

        public static void VolumeGroupReset()
        {
            War3.CallNative(War3.GetNativeFunction("VolumeGroupReset"));
        }

        // title = "声音在播放 [new]"
        public static bool GetSoundIsPlaying(JSound soundHandle)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetSoundIsPlaying"), soundHandle);
        }

        // title = "声音在加载 [new]"
        public static bool GetSoundIsLoading(JSound soundHandle)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("GetSoundIsLoading"), soundHandle);
        }

        // title = "注册堆叠音效 [new]"
        public static void RegisterStackedSound(JSound soundHandle, bool byPosition, float rectwidth, float rectheight)
        {
            War3.CallNative(War3.GetNativeFunction("RegisterStackedSound"), soundHandle, byPosition, rectwidth, rectheight);
        }

        // title = "取消注册的堆叠音效设置 [new]"
        public static void UnregisterStackedSound(JSound soundHandle, bool byPosition, float rectwidth, float rectheight)
        {
            War3.CallNative(War3.GetNativeFunction("UnregisterStackedSound"), soundHandle, byPosition, rectwidth, rectheight);
        }

        // title = "新建天气效果 [R]"
        public static JWeatherEffect AddWeatherEffect(JRect where, int effectID)
        {
            return War3.CallNative<JWeatherEffect>(War3.GetNativeFunction("AddWeatherEffect"), where, effectID);
        }

        public static void RemoveWeatherEffect(JWeatherEffect whichEffect)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveWeatherEffect"), whichEffect);
        }

        // title = "启用/禁用 天气效果"
        public static void EnableWeatherEffect(JWeatherEffect whichEffect, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("EnableWeatherEffect"), whichEffect, enable);
        }

        // title = "新建地形变化:弹坑 [R]"
        public static JTerrainDeformation TerrainDeformCrater(float x, float y, float radius, float depth, int duration, bool permanent)
        {
            return War3.CallNative<JTerrainDeformation>(War3.GetNativeFunction("TerrainDeformCrater"), x, y, radius, depth, duration, permanent);
        }

        // title = "新建地形变化:波纹 [R]"
        public static JTerrainDeformation TerrainDeformRipple(float x, float y, float radius, float depth, int duration, int count, float spaceWaves, float timeWaves, float radiusStartPct, bool limitNeg)
        {
            return War3.CallNative<JTerrainDeformation>(War3.GetNativeFunction("TerrainDeformRipple"), x, y, radius, depth, duration, count, spaceWaves, timeWaves, radiusStartPct, limitNeg);
        }

        // title = "新建地形变化:冲击波 [R]"
        public static JTerrainDeformation TerrainDeformWave(float x, float y, float dirX, float dirY, float distance, float speed, float radius, float depth, int trailTime, int count)
        {
            return War3.CallNative<JTerrainDeformation>(War3.GetNativeFunction("TerrainDeformWave"), x, y, dirX, dirY, distance, speed, radius, depth, trailTime, count);
        }

        // title = "新建地形变化:随机 [R]"
        public static JTerrainDeformation TerrainDeformRandom(float x, float y, float radius, float minDelta, float maxDelta, int duration, int updateInterval)
        {
            return War3.CallNative<JTerrainDeformation>(War3.GetNativeFunction("TerrainDeformRandom"), x, y, radius, minDelta, maxDelta, duration, updateInterval);
        }

        // title = "停止地形变化 [R]"
        public static void TerrainDeformStop(JTerrainDeformation deformation, int duration)
        {
            War3.CallNative(War3.GetNativeFunction("TerrainDeformStop"), deformation, duration);
        }

        // title = "停止所有地形变化"
        public static void TerrainDeformStopAll()
        {
            War3.CallNative(War3.GetNativeFunction("TerrainDeformStopAll"));
        }

        // title = "新建特效(创建到坐标) [R]"
        public static JEffect AddSpecialEffect(string modelName, float x, float y)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpecialEffect"), modelName, x, y);
        }

        // title = "新建特效(创建到点) [R]"
        public static JEffect AddSpecialEffectLoc(string modelName, JLocation where)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpecialEffectLoc"), modelName, where);
        }

        // title = "新建特效(创建到单位) [R]"
        public static JEffect AddSpecialEffectTarget(string modelName, JWidget targetWidget, string attachPointName)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpecialEffectTarget"), modelName, targetWidget, attachPointName);
        }

        public static void DestroyEffect(JEffect whichEffect)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyEffect"), whichEffect);
        }

        public static JEffect AddSpellEffect(string abilityString, JEffectType t, float x, float y)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffect"), abilityString, t, x, y);
        }

        public static JEffect AddSpellEffectLoc(string abilityString, JEffectType t, JLocation where)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectLoc"), abilityString, t, where);
        }

        // title = "新建特效(指定技能，创建到坐标) [R]"
        public static JEffect AddSpellEffectById(int abilityId, JEffectType t, float x, float y)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectById"), abilityId, t, x, y);
        }

        // title = "新建特效(指定技能，创建到点) [R]"
        public static JEffect AddSpellEffectByIdLoc(int abilityId, JEffectType t, JLocation where)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectByIdLoc"), abilityId, t, where);
        }

        public static JEffect AddSpellEffectTarget(string modelName, JEffectType t, JWidget targetWidget, string attachPoint)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectTarget"), modelName, t, targetWidget, attachPoint);
        }

        // title = "新建特效(指定技能，创建到单位) [R]"
        public static JEffect AddSpellEffectTargetById(int abilityId, JEffectType t, JWidget targetWidget, string attachPoint)
        {
            return War3.CallNative<JEffect>(War3.GetNativeFunction("AddSpellEffectTargetById"), abilityId, t, targetWidget, attachPoint);
        }

        // title = "新建闪电效果 [R]"
        public static JLightning AddLightning(string codeName, bool checkVisibility, float x1, float y1, float x2, float y2)
        {
            return War3.CallNative<JLightning>(War3.GetNativeFunction("AddLightning"), codeName, checkVisibility, x1, y1, x2, y2);
        }

        // title = "新建闪电效果(指定Z轴) [R]"
        public static JLightning AddLightningEx(string codeName, bool checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return War3.CallNative<JLightning>(War3.GetNativeFunction("AddLightningEx"), codeName, checkVisibility, x1, y1, z1, x2, y2, z2);
        }

        public static bool DestroyLightning(JLightning whichBolt)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DestroyLightning"), whichBolt);
        }

        public static bool MoveLightning(JLightning whichBolt, bool checkVisibility, float x1, float y1, float x2, float y2)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("MoveLightning"), whichBolt, checkVisibility, x1, y1, x2, y2);
        }

        // title = "移动闪电效果(指定坐标) [R]"
        public static bool MoveLightningEx(JLightning whichBolt, bool checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("MoveLightningEx"), whichBolt, checkVisibility, x1, y1, z1, x2, y2, z2);
        }

        public static float GetLightningColorA(JLightning whichBolt)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLightningColorA"), whichBolt);
        }

        public static float GetLightningColorR(JLightning whichBolt)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLightningColorR"), whichBolt);
        }

        public static float GetLightningColorG(JLightning whichBolt)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLightningColorG"), whichBolt);
        }

        public static float GetLightningColorB(JLightning whichBolt)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("GetLightningColorB"), whichBolt);
        }

        public static bool SetLightningColor(JLightning whichBolt, float r, float g, float b, float a)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("SetLightningColor"), whichBolt, r, g, b, a);
        }

        public static string GetAbilityEffect(string abilityString, JEffectType t, int index)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetAbilityEffect"), abilityString, t, index);
        }

        public static string GetAbilityEffectById(int abilityId, JEffectType t, int index)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetAbilityEffectById"), abilityId, t, index);
        }

        public static string GetAbilitySound(string abilityString, JSoundType t)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetAbilitySound"), abilityString, t);
        }

        public static string GetAbilitySoundById(int abilityId, JSoundType t)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("GetAbilitySoundById"), abilityId, t);
        }

        // title = "地形悬崖高度(指定坐标) [R]"
        public static int GetTerrainCliffLevel(float x, float y)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTerrainCliffLevel"), x, y);
        }

        // title = "设置水颜色 [R]"
        public static void SetWaterBaseColor(int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetWaterBaseColor"), red, green, blue, alpha);
        }

        // title = "开启/关闭 水面变形"
        public static void SetWaterDeforms(bool val)
        {
            War3.CallNative(War3.GetNativeFunction("SetWaterDeforms"), val);
        }

        // title = "指定坐标地形 [R]"
        public static int GetTerrainType(float x, float y)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTerrainType"), x, y);
        }

        // title = "地形样式(指定坐标) [R]"
        public static int GetTerrainVariance(float x, float y)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("GetTerrainVariance"), x, y);
        }

        // title = "改变地形类型(指定坐标) [R]"
        public static void SetTerrainType(float x, float y, int terrainType, int variation, int area, int shape)
        {
            War3.CallNative(War3.GetNativeFunction("SetTerrainType"), x, y, terrainType, variation, area, shape);
        }

        // title = "地形通行状态关闭(指定坐标) [R]"
        public static bool IsTerrainPathable(float x, float y, JPathingType t)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsTerrainPathable"), x, y, t);
        }

        // title = "设置地形通行状态(指定坐标) [R]"
        public static void SetTerrainPathable(float x, float y, JPathingType t, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetTerrainPathable"), x, y, t, flag);
        }

        // title = "新建图像 [R]"
        public static JImage CreateImage(string file, float sizeX, float sizeY, float sizeZ, float posX, float posY, float posZ, float originX, float originY, float originZ, int imageType)
        {
            return War3.CallNative<JImage>(War3.GetNativeFunction("CreateImage"), file, sizeX, sizeY, sizeZ, posX, posY, posZ, originX, originY, originZ, imageType);
        }

        // title = "删除"
        public static void DestroyImage(JImage whichImage)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyImage"), whichImage);
        }

        // title = "显示/隐藏 [R]"
        public static void ShowImage(JImage whichImage, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("ShowImage"), whichImage, flag);
        }

        // title = "设置图像高度"
        public static void SetImageConstantHeight(JImage whichImage, bool flag, float height)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageConstantHeight"), whichImage, flag, height);
        }

        // title = "改变图像位置(指定坐标) [R]"
        public static void SetImagePosition(JImage whichImage, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("SetImagePosition"), whichImage, x, y, z);
        }

        // title = "改变图像颜色 [R]"
        public static void SetImageColor(JImage whichImage, int red, int green, int blue, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageColor"), whichImage, red, green, blue, alpha);
        }

        // title = "设置图像渲染状态"
        public static void SetImageRender(JImage whichImage, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageRender"), whichImage, flag);
        }

        // title = "设置图像永久渲染状态"
        public static void SetImageRenderAlways(JImage whichImage, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageRenderAlways"), whichImage, flag);
        }

        // title = "图像水面显示状态"
        public static void SetImageAboveWater(JImage whichImage, bool flag, bool useWaterAlpha)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageAboveWater"), whichImage, flag, useWaterAlpha);
        }

        // title = "改变图像类型"
        public static void SetImageType(JImage whichImage, int imageType)
        {
            War3.CallNative(War3.GetNativeFunction("SetImageType"), whichImage, imageType);
        }

        // title = "新建地面纹理变化 [R]"
        public static JUbersplat CreateUbersplat(float x, float y, string name, int red, int green, int blue, int alpha, bool forcePaused, bool noBirthTime)
        {
            return War3.CallNative<JUbersplat>(War3.GetNativeFunction("CreateUbersplat"), x, y, name, red, green, blue, alpha, forcePaused, noBirthTime);
        }

        // title = "删除地面纹理变化"
        public static void DestroyUbersplat(JUbersplat whichSplat)
        {
            War3.CallNative(War3.GetNativeFunction("DestroyUbersplat"), whichSplat);
        }

        // title = "重置地面纹理变化"
        public static void ResetUbersplat(JUbersplat whichSplat)
        {
            War3.CallNative(War3.GetNativeFunction("ResetUbersplat"), whichSplat);
        }

        // title = "结束地面纹理变化"
        public static void FinishUbersplat(JUbersplat whichSplat)
        {
            War3.CallNative(War3.GetNativeFunction("FinishUbersplat"), whichSplat);
        }

        // title = "显示/隐藏 地面纹理变化[R]"
        public static void ShowUbersplat(JUbersplat whichSplat, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("ShowUbersplat"), whichSplat, flag);
        }

        // title = "设置渲染状态"
        public static void SetUbersplatRender(JUbersplat whichSplat, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUbersplatRender"), whichSplat, flag);
        }

        // title = "设置永久渲染状态"
        public static void SetUbersplatRenderAlways(JUbersplat whichSplat, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("SetUbersplatRenderAlways"), whichSplat, flag);
        }

        // title = "创建/删除荒芜地表(圆范围)(指定坐标) [R]"
        public static void SetBlight(JPlayer whichPlayer, float x, float y, float radius, bool addBlight)
        {
            War3.CallNative(War3.GetNativeFunction("SetBlight"), whichPlayer, x, y, radius, addBlight);
        }

        // title = "创建/删除荒芜地表(矩形区域) [R]"
        public static void SetBlightRect(JPlayer whichPlayer, JRect r, bool addBlight)
        {
            War3.CallNative(War3.GetNativeFunction("SetBlightRect"), whichPlayer, r, addBlight);
        }

        public static void SetBlightPoint(JPlayer whichPlayer, float x, float y, bool addBlight)
        {
            War3.CallNative(War3.GetNativeFunction("SetBlightPoint"), whichPlayer, x, y, addBlight);
        }

        public static void SetBlightLoc(JPlayer whichPlayer, JLocation whichLocation, float radius, bool addBlight)
        {
            War3.CallNative(War3.GetNativeFunction("SetBlightLoc"), whichPlayer, whichLocation, radius, addBlight);
        }

        // title = "新建不死族金矿 [R]"
        public static JUnit CreateBlightedGoldmine(JPlayer id, float x, float y, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("CreateBlightedGoldmine"), id, x, y, face);
        }

        // title = "坐标点被荒芜地表覆盖 [R]"
        public static bool IsPointBlighted(float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsPointBlighted"), x, y);
        }

        // title = "播放圆范围内地形装饰物动画 [R]"
        public static void SetDoodadAnimation(float x, float y, float radius, int doodadID, bool nearestOnly, string animName, bool animRandom)
        {
            War3.CallNative(War3.GetNativeFunction("SetDoodadAnimation"), x, y, radius, doodadID, nearestOnly, animName, animRandom);
        }

        // title = "播放矩形区域内地形装饰物动画 [R]"
        public static void SetDoodadAnimationRect(JRect r, int doodadID, string animName, bool animRandom)
        {
            War3.CallNative(War3.GetNativeFunction("SetDoodadAnimationRect"), r, doodadID, animName, animRandom);
        }

        // title = "启用对战AI"
        public static void StartMeleeAI(JPlayer num, string script)
        {
            War3.CallNative(War3.GetNativeFunction("StartMeleeAI"), num, script);
        }

        // title = "启用战役AI"
        public static void StartCampaignAI(JPlayer num, string script)
        {
            War3.CallNative(War3.GetNativeFunction("StartCampaignAI"), num, script);
        }

        // title = "发送AI命令"
        public static void CommandAI(JPlayer num, int command, int data)
        {
            War3.CallNative(War3.GetNativeFunction("CommandAI"), num, command, data);
        }

        // title = "暂停/恢复 AI脚本运行 [R]"
        public static void PauseCompAI(JPlayer p, bool pause)
        {
            War3.CallNative(War3.GetNativeFunction("PauseCompAI"), p, pause);
        }

        // title = "玩家的AI难度"
        public static JAiDifficulty GetAIDifficulty(JPlayer num)
        {
            return War3.CallNative<JAiDifficulty>(War3.GetNativeFunction("GetAIDifficulty"), num);
        }

        // title = "忽视指定单位的警戒点"
        public static void RemoveGuardPosition(JUnit hUnit)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveGuardPosition"), hUnit);
        }

        // title = "恢复指定单位的警戒点"
        public static void RecycleGuardPosition(JUnit hUnit)
        {
            War3.CallNative(War3.GetNativeFunction("RecycleGuardPosition"), hUnit);
        }

        // title = "忽视所有单位的警戒点"
        public static void RemoveAllGuardPositions(JPlayer num)
        {
            War3.CallNative(War3.GetNativeFunction("RemoveAllGuardPositions"), num);
        }

        // title = "输入作弊码 [R]"
        public static void Cheat(string cheatStr)
        {
            War3.CallNative(War3.GetNativeFunction("Cheat"), cheatStr);
        }

        // title = "无法胜利 [R]"
        public static bool IsNoVictoryCheat()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsNoVictoryCheat"));
        }

        // title = "无法失败 [R]"
        public static bool IsNoDefeatCheat()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("IsNoDefeatCheat"));
        }

        // title = "预载文件"
        public static void Preload(string filename)
        {
            War3.CallNative(War3.GetNativeFunction("Preload"), filename);
        }

        // title = "开始预载"
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

        // title = "批量预载"
        public static void Preloader(string filename)
        {
            War3.CallNative(War3.GetNativeFunction("Preloader"), filename);
        }

    }

}
