using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace War3Frame.Library.Api
{
    /*
    [[
    if Jass == nil then
        Jass = {}
        Jass.Runtime = require("jass.runtime")
        Jass.Console = require("jass.console")
        Jass.Debug = require("jass.debug")
        Jass.Common = require("jass.common")
        Jass.Globals = require("jass.globals")
        Jass.Slk = require("jass.slk")
        Jass.Japi = require("jass.japi")
    end
    ]]*/

    public static class JassApi
    {

        public static class Common
        {

            /// @CSharpLua.Template = "Jass.Common["GetLocalPlayer"]()"
            public static extern int GetLocalPlayer();
            // abilityIdString string
            /// @CSharpLua.Template = "Jass.Common["AbilityId"]({0})"
            public static extern int AbilityId(string abilityIdString);

            // abilityId number int
            /// @CSharpLua.Template = "Jass.Common["AbilityId2String"]({0})"
            public static extern string AbilityId2String(int abilityId);

            // 增加经验值 [R]
            // 增加[某个英雄][Quantity]点经验值,[显示/隐藏]升级动画
            // 经验值不能倒退.
            // whichHero number unit
            // xpToAdd number int
            // showEyeCandy boolean
            /// @CSharpLua.Template = "Jass.Common["AddHeroXP"]({0}, {1}, {2})"
            public static extern void AddHeroXP(float whichHero, float xpToAdd, bool showEyeCandy);

            // whichWidget number widget
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["AddIndicator"]({0}, {1}, {2}, {3}, {4})"
            public static extern void AddIndicator(float whichWidget, float red, float green, float blue, float alpha);

            // 添加物品(所有市场)
            // 添加[物品类型]到所有市场并设置库存量:[Count]最大库存量:[Max]
            // 影响所有拥有'出售物品'技能的单位.
            // itemId number int
            // currentStock number int
            // stockMax number int
            /// @CSharpLua.Template = "Jass.Common["AddItemToAllStock"]({0}, {1}, {2})"
            public static extern void AddItemToAllStock(float itemId, float currentStock, float stockMax);

            // 添加物品(指定市场)
            // 添加[物品类型]到[Marketplace]并设置库存量:[Count]最大库存量:[Max]
            // 只影响有'出售物品'技能的单位.
            // whichUnit number unit
            // itemId number int
            // currentStock number int
            // stockMax number int
            /// @CSharpLua.Template = "Jass.Common["AddItemToStock"]({0}, {1}, {2}, {3})"
            public static extern void AddItemToStock(int whichUnit, float itemId, float currentStock, float stockMax);

            // 创建闪电效果
            // 创建一道[Type]闪电效果,从[坐标]到[坐标]
            // codeName string
            // checkVisibility boolean
            // x1 number
            // y1 number
            // x2 number
            // y2 number
            /// @CSharpLua.Template = "Jass.Common["AddLightning"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern float AddLightning(string codeName, bool checkVisibility, float x1, float y1, float x2, float y2);

            // codeName string
            // checkVisibility boolean
            // x1 number
            // y1 number
            // z1 number
            // x2 number
            // y2 number
            // z2 number
            /// @CSharpLua.Template = "Jass.Common["AddLightningEx"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
            public static extern float AddLightningEx(string codeName, bool checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2);

            // 增加科技等级
            // 增加[玩家]的[科技]科技[整数]级
            // 科技等级不能倒退。
            // whichPlayer number player
            // techid number int
            // levels number int
            /// @CSharpLua.Template = "Jass.Common["AddPlayerTechResearched"]({0}, {1}, {2})"
            public static extern void AddPlayerTechResearched(int whichPlayer, float techid, float levels);

            // 增加黄金储量
            // 增加[Quantity]黄金储量给[金矿]
            // 使用负数来减少黄金储量.
            // whichUnit number unit
            // amount number int
            /// @CSharpLua.Template = "Jass.Common["AddResourceAmount"]({0}, {1})"
            public static extern void AddResourceAmount(int whichUnit, float amount);

            // modelName string
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["AddSpecialEffect"]({0}, {1}, {2})"
            public static extern int AddSpecialEffect(string modelName, float x, float y);

            // 创建特效(指定点)
            // 在[指定点]创建特效:[Model File]
            // modelName string
            // where number location
            /// @CSharpLua.Template = "Jass.Common["AddSpecialEffectLoc"]({0}, {1})"
            public static extern int AddSpecialEffectLoc(string modelName, int where);

            // modelName string
            // targetWidget number widget
            // attachPointName string
            /// @CSharpLua.Template = "Jass.Common["AddSpecialEffectTarget"]({0}, {1}, {2})"
            public static extern int AddSpecialEffectTarget(string modelName, float targetWidget, string attachPointName);

            // abilityString string
            // t any effecttype
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["AddSpellEffect"]({0}, {1}, {2}, {3})"
            public static extern int AddSpellEffect(string abilityString, object t, float x, float y);

            // abilityId number int
            // t any effecttype
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["AddSpellEffectById"]({0}, {1}, {2}, {3})"
            public static extern int AddSpellEffectById(int abilityId, object t, float x, float y);

            // abilityId number int
            // t any effecttype
            // where number location
            /// @CSharpLua.Template = "Jass.Common["AddSpellEffectByIdLoc"]({0}, {1}, {2})"
            public static extern int AddSpellEffectByIdLoc(int abilityId, object t, int where);

            // abilityString string
            // t any effecttype
            // where number location
            /// @CSharpLua.Template = "Jass.Common["AddSpellEffectLoc"]({0}, {1}, {2})"
            public static extern int AddSpellEffectLoc(string abilityString, object t, int where);

            // modelName string
            // t any effecttype
            // targetWidget number widget
            // attachPoint string
            /// @CSharpLua.Template = "Jass.Common["AddSpellEffectTarget"]({0}, {1}, {2}, {3})"
            public static extern int AddSpellEffectTarget(string modelName, object t, float targetWidget, string attachPoint);

            // abilityId number int
            // t any effecttype
            // targetWidget number widget
            // attachPoint string
            /// @CSharpLua.Template = "Jass.Common["AddSpellEffectTargetById"]({0}, {1}, {2}, {3})"
            public static extern int AddSpellEffectTargetById(int abilityId, object t, float targetWidget, string attachPoint);

            // 添加/删除 单位动画附加名 [R]
            // 给[单位]附加动作[Tag],状态为[Add/Remove]
            // 比如恶魔猎手添加'alternate'会显示为恶魔形态;农民添加'gold'则为背负黄金形态.
            // whichUnit number unit
            // animProperties string
            // add boolean
            /// @CSharpLua.Template = "Jass.Common["AddUnitAnimationProperties"]({0}, {1}, {2})"
            public static extern void AddUnitAnimationProperties(int whichUnit, string animProperties, bool add);

            // 添加单位(所有市场)
            // 添加[单位类型]到所有市场并设置库存量:[Count]最大库存量:[Max]
            // 影响所有拥有'出售单位'技能的单位.
            // unitId number int
            // currentStock number int
            // stockMax number int
            /// @CSharpLua.Template = "Jass.Common["AddUnitToAllStock"]({0}, {1}, {2})"
            public static extern void AddUnitToAllStock(int unitId, float currentStock, float stockMax);

            // 添加单位(指定市场)
            // 添加[单位类型]到[Marketplace]并设置库存量:[Count]最大库存量:[Max]
            // 只影响有'出售单位'技能的单位.
            // whichUnit number unit
            // unitId number int
            // currentStock number int
            // stockMax number int
            /// @CSharpLua.Template = "Jass.Common["AddUnitToStock"]({0}, {1}, {2}, {3})"
            public static extern void AddUnitToStock(int whichUnit, int unitId, float currentStock, float stockMax);

            // where number rect
            // effectID number int
            /// @CSharpLua.Template = "Jass.Common["AddWeatherEffect"]({0}, {1})"
            public static extern int AddWeatherEffect(int where, int effectID);

            // whichField any camerafield
            // offset number
            // duration number
            /// @CSharpLua.Template = "Jass.Common["AdjustCameraField"]({0}, {1}, {2})"
            public static extern void AdjustCameraField(object whichField, float offset, float duration);

            // 绑定单位
            // 将[3D音效]绑定到[单位]
            // 该动作仅用于3D音效.
            // soundHandle number sound
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["AttachSoundToUnit"]({0}, {1})"
            public static extern void AttachSoundToUnit(float soundHandle, int whichUnit);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["CachePlayerHeroData"]({0})"
            public static extern void CachePlayerHeroData(int whichPlayer);

            // 设置镜头平滑参数
            // 设置镜头平滑参数为[Factor]
            // 数值越大,镜头转换越平滑.
            // factor number
            /// @CSharpLua.Template = "Jass.Common["CameraSetSmoothingFactor"]({0})"
            public static extern void CameraSetSmoothingFactor(float factor);

            // mag number
            // velocity number
            /// @CSharpLua.Template = "Jass.Common["CameraSetSourceNoise"]({0}, {1})"
            public static extern void CameraSetSourceNoise(float mag, float velocity);

            // 摇晃镜头源(所有玩家) [R]
            // 摇晃玩家的镜头源, 摇晃幅度:[Magnitude]速率:[Velocity]摇晃方式:[方式]
            // 使用'镜头 - 重置镜头'或设置摇晃幅度和速率为0来停止摇晃.
            // mag number
            // velocity number
            // vertOnly boolean
            /// @CSharpLua.Template = "Jass.Common["CameraSetSourceNoiseEx"]({0}, {1}, {2})"
            public static extern void CameraSetSourceNoiseEx(float mag, float velocity, bool vertOnly);

            // mag number
            // velocity number
            /// @CSharpLua.Template = "Jass.Common["CameraSetTargetNoise"]({0}, {1})"
            public static extern void CameraSetTargetNoise(float mag, float velocity);

            // 摇晃镜头目标(所有玩家) [R]
            // 摇晃玩家的镜头源, 摇晃幅度:[Magnitude]速率:[Velocity]摇晃方式:[方式]
            // 使用'镜头 - 重置镜头'或设置摇晃幅度和速率为0来停止摇晃.
            // mag number
            // velocity number
            // vertOnly boolean
            /// @CSharpLua.Template = "Jass.Common["CameraSetTargetNoiseEx"]({0}, {1}, {2})"
            public static extern void CameraSetTargetNoiseEx(float mag, float velocity, bool vertOnly);

            // whichSetup any camerasetup
            // doPan boolean
            // panTimed boolean
            /// @CSharpLua.Template = "Jass.Common["CameraSetupApply"]({0}, {1}, {2})"
            public static extern void CameraSetupApply(object whichSetup, bool doPan, bool panTimed);

            // 应用镜头(所有玩家)(限时) [R]
            // 将[镜头]应用方式设置为[Apply Method],持续[Time]秒
            // whichSetup any camerasetup
            // doPan boolean
            // forceDuration number
            /// @CSharpLua.Template = "Jass.Common["CameraSetupApplyForceDuration"]({0}, {1}, {2})"
            public static extern void CameraSetupApplyForceDuration(object whichSetup, bool doPan, float forceDuration);

            // whichSetup any camerasetup
            // zDestOffset number
            // forceDuration number
            /// @CSharpLua.Template = "Jass.Common["CameraSetupApplyForceDurationWithZ"]({0}, {1}, {2})"
            public static extern void CameraSetupApplyForceDurationWithZ(object whichSetup, float zDestOffset, float forceDuration);

            // whichSetup any camerasetup
            // zDestOffset number
            /// @CSharpLua.Template = "Jass.Common["CameraSetupApplyWithZ"]({0}, {1})"
            public static extern void CameraSetupApplyWithZ(object whichSetup, float zDestOffset);

            // whichSetup any camerasetup
            /// @CSharpLua.Template = "Jass.Common["CameraSetupGetDestPositionLoc"]({0})"
            public static extern float CameraSetupGetDestPositionLoc(object whichSetup);

            // whichSetup any camerasetup
            /// @CSharpLua.Template = "Jass.Common["CameraSetupGetDestPositionX"]({0})"
            public static extern float CameraSetupGetDestPositionX(object whichSetup);

            // whichSetup any camerasetup
            /// @CSharpLua.Template = "Jass.Common["CameraSetupGetDestPositionY"]({0})"
            public static extern float CameraSetupGetDestPositionY(object whichSetup);

            // whichSetup any camerasetup
            // whichField any camerafield
            /// @CSharpLua.Template = "Jass.Common["CameraSetupGetField"]({0}, {1})"
            public static extern float CameraSetupGetField(object whichSetup, object whichField);

            // whichSetup any camerasetup
            // x number
            // y number
            // duration number
            /// @CSharpLua.Template = "Jass.Common["CameraSetupSetDestPosition"]({0}, {1}, {2}, {3})"
            public static extern void CameraSetupSetDestPosition(object whichSetup, float x, float y, float duration);

            // whichSetup any camerasetup
            // whichField any camerafield
            // value number
            // duration number
            /// @CSharpLua.Template = "Jass.Common["CameraSetupSetField"]({0}, {1}, {2}, {3})"
            public static extern void CameraSetupSetField(object whichSetup, object whichField, float value, float duration);

            // 切换关卡 [R]
            // 切换到关卡:[Filename]([Show/Skip]计分屏)
            // newLevel string
            // doScoreScreen boolean
            /// @CSharpLua.Template = "Jass.Common["ChangeLevel"]({0}, {1})"
            public static extern void ChangeLevel(string newLevel, bool doScoreScreen);

            // 输入作弊码 [R]
            // 输入作弊码:[String]
            // 作弊码只在单机有效.
            // cheatStr string
            /// @CSharpLua.Template = "Jass.Common["Cheat"]({0})"
            public static extern void Cheat(string cheatStr);

            // level number int
            /// @CSharpLua.Template = "Jass.Common["ChooseRandomCreep"]({0})"
            public static extern int ChooseRandomCreep(int level);

            // level number int
            /// @CSharpLua.Template = "Jass.Common["ChooseRandomItem"]({0})"
            public static extern int ChooseRandomItem(int level);

            // whichType any itemtype
            // level number int
            /// @CSharpLua.Template = "Jass.Common["ChooseRandomItemEx"]({0}, {1})"
            public static extern int ChooseRandomItemEx(object whichType, int level);

            /// @CSharpLua.Template = "Jass.Common["ChooseRandomNPBuilding"]()"
            public static extern int ChooseRandomNPBuilding();

            // 清空背景音乐列表
            /// @CSharpLua.Template = "Jass.Common["ClearMapMusic"]()"
            public static extern void ClearMapMusic();

            // 清空选择(所有玩家)
            // 清空所有玩家的选择
            // 使玩家取消选择所有已选单位.
            /// @CSharpLua.Template = "Jass.Common["ClearSelection"]()"
            public static extern void ClearSelection();

            // 清空文本信息(所有玩家) [R]
            // 清空玩家屏幕上的文本信息
            /// @CSharpLua.Template = "Jass.Common["ClearTextMessages"]()"
            public static extern void ClearTextMessages();

            // 发送AI命令
            // 对[某个玩家]发送AI命令:([命令],[数据])
            // 发送的AI命令将被AI脚本所使用.
            // num number player
            // command number int
            // data number int
            /// @CSharpLua.Template = "Jass.Common["CommandAI"]({0}, {1}, {2})"
            public static extern void CommandAI(float num, float command, float data);

            // func any code
            /// @CSharpLua.Template = "Jass.Common["Condition"]({0})"
            public static extern object Condition(object func);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertAIDifficulty"]({0})"
            public static extern int ConvertAIDifficulty(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertAllianceType"]({0})"
            public static extern int ConvertAllianceType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertAttackType"]({0})"
            public static extern int ConvertAttackType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertBlendMode"]({0})"
            public static extern int ConvertBlendMode(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertCameraField"]({0})"
            public static extern int ConvertCameraField(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertDamageType"]({0})"
            public static extern int ConvertDamageType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertDialogEvent"]({0})"
            public static extern int ConvertDialogEvent(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertEffectType"]({0})"
            public static extern int ConvertEffectType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertFGameState"]({0})"
            public static extern int ConvertFGameState(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertFogState"]({0})"
            public static extern int ConvertFogState(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertGameDifficulty"]({0})"
            public static extern int ConvertGameDifficulty(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertGameEvent"]({0})"
            public static extern int ConvertGameEvent(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertGameSpeed"]({0})"
            public static extern int ConvertGameSpeed(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertGameType"]({0})"
            public static extern int ConvertGameType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertIGameState"]({0})"
            public static extern int ConvertIGameState(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertItemType"]({0})"
            public static extern int ConvertItemType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertLimitOp"]({0})"
            public static extern int ConvertLimitOp(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertMapControl"]({0})"
            public static extern int ConvertMapControl(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertMapDensity"]({0})"
            public static extern int ConvertMapDensity(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertMapFlag"]({0})"
            public static extern int ConvertMapFlag(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertMapSetting"]({0})"
            public static extern int ConvertMapSetting(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertMapVisibility"]({0})"
            public static extern int ConvertMapVisibility(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertPathingType"]({0})"
            public static extern int ConvertPathingType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertPlacement"]({0})"
            public static extern int ConvertPlacement(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertPlayerColor"]({0})"
            public static extern int ConvertPlayerColor(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertPlayerEvent"]({0})"
            public static extern int ConvertPlayerEvent(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertPlayerGameResult"]({0})"
            public static extern int ConvertPlayerGameResult(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertPlayerScore"]({0})"
            public static extern int ConvertPlayerScore(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertPlayerSlotState"]({0})"
            public static extern int ConvertPlayerSlotState(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertPlayerState"]({0})"
            public static extern int ConvertPlayerState(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertPlayerUnitEvent"]({0})"
            public static extern int ConvertPlayerUnitEvent(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertRace"]({0})"
            public static extern int ConvertRace(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertRacePref"]({0})"
            public static extern int ConvertRacePref(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertRarityControl"]({0})"
            public static extern int ConvertRarityControl(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertSoundType"]({0})"
            public static extern int ConvertSoundType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertStartLocPrio"]({0})"
            public static extern int ConvertStartLocPrio(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertTexMapFlags"]({0})"
            public static extern int ConvertTexMapFlags(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertUnitEvent"]({0})"
            public static extern int ConvertUnitEvent(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertUnitState"]({0})"
            public static extern int ConvertUnitState(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertUnitType"]({0})"
            public static extern int ConvertUnitType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertVersion"]({0})"
            public static extern int ConvertVersion(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertVolumeGroup"]({0})"
            public static extern int ConvertVolumeGroup(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertWeaponType"]({0})"
            public static extern int ConvertWeaponType(int i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["ConvertWidgetEvent"]({0})"
            public static extern int ConvertWidgetEvent(int i);

            // 复制存档文件
            // 复制[源文件]并保存为[目标文件]
            // 该动作只在响应'保存/读取进度'时有效,每个事件中最多能用16次.
            // sourceSaveName string
            // destSaveName string
            /// @CSharpLua.Template = "Jass.Common["CopySaveGame"]({0}, {1})"
            public static extern bool CopySaveGame(string sourceSaveName, string destSaveName);

            // id number player
            // x number
            // y number
            // face number
            /// @CSharpLua.Template = "Jass.Common["CreateBlightedGoldmine"]({0}, {1}, {2}, {3})"
            public static extern int CreateBlightedGoldmine(int id, float x, float y, int face);

            /// @CSharpLua.Template = "Jass.Common["CreateCameraSetup"]()"
            public static extern int CreateCameraSetup();

            // whichPlayer number player
            // unitid number int
            // x number
            // y number
            // face number
            /// @CSharpLua.Template = "Jass.Common["CreateCorpse"]({0}, {1}, {2}, {3}, {4})"
            public static extern int CreateCorpse(int whichPlayer, int unitid, float x, float y, int face);

            // objectid number int
            // x number
            // y number
            // face number
            // scale number
            // variation number int
            /// @CSharpLua.Template = "Jass.Common["CreateDeadDestructable"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern int CreateDeadDestructable(int objectid, float x, float y, int face, float scale, int variation);

            // objectid number int
            // x number
            // y number
            // z number
            // face number
            // scale number
            // variation number int
            /// @CSharpLua.Template = "Jass.Common["CreateDeadDestructableZ"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern int CreateDeadDestructableZ(int objectid, float x, float y, float z, int face, float scale, int variation);

            // 创建失败条件
            // 创建失败条件:[文字]
            // 失败条件会在每个任务中显示.
            /// @CSharpLua.Template = "Jass.Common["CreateDefeatCondition"]()"
            public static extern object CreateDefeatCondition();

            // 创建可破坏物
            // 创建[可破坏物类型]在[指定点],面向角度:[Direction]尺寸缩放:[Scale]样式:[Variation]
            // 面向角度采用角度制,0度为正东方向,90度为正北方向. 使用'最后创建的可破坏物'来获取创建的物体.
            // objectid number int
            // x number
            // y number
            // face number
            // scale number
            // variation number int
            /// @CSharpLua.Template = "Jass.Common["CreateDestructable"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern int CreateDestructable(int objectid, float x, float y, int face, float scale, int variation);

            // objectid number int
            // x number
            // y number
            // z number
            // face number
            // scale number
            // variation number int
            /// @CSharpLua.Template = "Jass.Common["CreateDestructableZ"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern int CreateDestructableZ(int objectid, float x, float y, float z, int face, float scale, int variation);

            // forWhichPlayer number player
            // whichState any fogstate
            // centerx number
            // centerY number
            // radius number
            // useSharedVision boolean
            // afterUnits boolean
            /// @CSharpLua.Template = "Jass.Common["CreateFogModifierRadius"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern int CreateFogModifierRadius(float forWhichPlayer, object whichState, float centerx, float centerY, float radius, bool useSharedVision, bool afterUnits);

            // 创建可见度修正器(圆范围)
            // 创建一个状态为[Enabled/Disabled]的可见度修正器给[某个玩家],设置[Visibility State]在圆心为[指定点]半径为[Radius]的圆范围
            // 会创建可见度修正器.
            // forWhichPlayer number player
            // whichState any fogstate
            // center number location
            // radius number
            // useSharedVision boolean
            // afterUnits boolean
            /// @CSharpLua.Template = "Jass.Common["CreateFogModifierRadiusLoc"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern int CreateFogModifierRadiusLoc(float forWhichPlayer, object whichState, float center, float radius, bool useSharedVision, bool afterUnits);

            // 创建可见度修正器(矩形区域)
            // 创建一个状态为[Enabled/Disabled]的可见度修正器给[某个玩家],设置[Visibility State]在[Region]
            // 会创建可见度修正器.
            // forWhichPlayer number player
            // whichState any fogstate
            // where number rect
            // useSharedVision boolean
            // afterUnits boolean
            /// @CSharpLua.Template = "Jass.Common["CreateFogModifierRect"]({0}, {1}, {2}, {3}, {4})"
            public static extern int CreateFogModifierRect(float forWhichPlayer, object whichState, int where, bool useSharedVision, bool afterUnits);

            /// @CSharpLua.Template = "Jass.Common["CreateForce"]()"
            public static extern int CreateForce();

            /// @CSharpLua.Template = "Jass.Common["CreateGroup"]()"
            public static extern int CreateGroup();

            // 创建
            // 使用图像:[某个图像]大小:[Size]创建点:[指定点]Z轴偏移:[Z]图像类型:[Type]
            // 使用'图像 - 设置永久渲染状态'才能显示图像. 创建点作为图像的左下角位置. 该功能存在Bug,会在图像上和右面多出256象素. 所以需要支持Alpha通道的图像且上和右面最后一行像素为透明才能完美显示.
            // file string
            // sizeX number
            // sizeY number
            // sizeZ number
            // posX number
            // posY number
            // posZ number
            // originX number
            // originY number
            // originZ number
            // imageType number int
            /// @CSharpLua.Template = "Jass.Common["CreateImage"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})"
            public static extern int CreateImage(string file, float sizeX, float sizeY, float sizeZ, float posX, float posY, float posZ, float originX, float originY, float originZ, float imageType);

            // 创建
            // 新建[物品]在([X],[Y])
            // itemid number int
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["CreateItem"]({0}, {1}, {2})"
            public static extern int CreateItem(float itemid, float x, float y);

            /// @CSharpLua.Template = "Jass.Common["CreateItemPool"]()"
            public static extern int CreateItemPool();

            // 创建
            // 对[玩家组]创建排行榜,使用标题:[文字]
            // 排行榜不能在地图初始化时显示. 标题为空则不显示标题栏.
            /// @CSharpLua.Template = "Jass.Common["CreateLeaderboard"]()"
            public static extern int CreateLeaderboard();

            // soundLabel string
            // fadeInRate number int
            // fadeOutRate number int
            /// @CSharpLua.Template = "Jass.Common["CreateMIDISound"]({0}, {1}, {2})"
            public static extern int CreateMIDISound(string soundLabel, float fadeInRate, float fadeOutRate);

            // 创建
            // 创建一个列数为[Columns]行数为[Rows]标题为[文字]的多面板
            // 多面板不能在地图初始化时显示.
            /// @CSharpLua.Template = "Jass.Common["CreateMultiboard"]()"
            public static extern int CreateMultiboard();

            // 创建任务
            // 创建一个[Quest Type]任务,标题:[文字]任务说明:[文字]任务图标:[Icon Path]
            /// @CSharpLua.Template = "Jass.Common["CreateQuest"]()"
            public static extern int CreateQuest();

            /// @CSharpLua.Template = "Jass.Common["CreateRegion"]()"
            public static extern int CreateRegion();

            // fileName string
            // looping boolean
            // is3D boolean
            // stopwhenoutofrange boolean
            // fadeInRate number int
            // fadeOutRate number int
            // eaxSetting string
            /// @CSharpLua.Template = "Jass.Common["CreateSound"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern int CreateSound(string fileName, bool looping, bool is3D, bool stopwhenoutofrange, float fadeInRate, float fadeOutRate, string eaxSetting);

            // fileName string
            // looping boolean
            // is3D boolean
            // stopwhenoutofrange boolean
            // fadeInRate number int
            // fadeOutRate number int
            // SLKEntryName string
            /// @CSharpLua.Template = "Jass.Common["CreateSoundFilenameWithLabel"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern int CreateSoundFilenameWithLabel(string fileName, bool looping, bool is3D, bool stopwhenoutofrange, float fadeInRate, float fadeOutRate, string SLKEntryName);

            // soundLabel string
            // looping boolean
            // is3D boolean
            // stopwhenoutofrange boolean
            // fadeInRate number int
            // fadeOutRate number int
            /// @CSharpLua.Template = "Jass.Common["CreateSoundFromLabel"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern int CreateSoundFromLabel(string soundLabel, bool looping, bool is3D, bool stopwhenoutofrange, float fadeInRate, float fadeOutRate);

            /// @CSharpLua.Template = "Jass.Common["CreateTextTag"]()"
            public static extern int CreateTextTag();

            /// @CSharpLua.Template = "Jass.Common["CreateTimer"]()"
            public static extern int CreateTimer();

            // 创建计时器窗口
            // 为[计时器]创建计时器窗口,标题:[文字]
            // 计时器窗口不能在地图初始化时显示.
            // t number timer
            /// @CSharpLua.Template = "Jass.Common["CreateTimerDialog"]({0})"
            public static extern int CreateTimerDialog(float t);

            // trackableModelPath string
            // x number
            // y number
            // facing number
            /// @CSharpLua.Template = "Jass.Common["CreateTrackable"]({0}, {1}, {2}, {3})"
            public static extern object CreateTrackable(string trackableModelPath, float x, float y, float facing);

            /// @CSharpLua.Template = "Jass.Common["CreateTrigger"]()"
            public static extern int CreateTrigger();

            // 创建地面纹理变化
            // 创建一个地面纹理变化在[指定点],使用图像:[Type]颜色值:([Red]%,[Green]%,[Blue]%) 透明度[Alpha]% ([Enable/Disable]暂停状态,[Enble/Disable]跳过出生动画)
            // 颜色格式为(红,绿,蓝). 透明度100%是不可见的. 使用'地面纹理变化 - 设置永久渲染状态' 来显示创建的纹理变化. 暂停状态表示动画播放完毕后是否继续保留该纹理变化.
            // x number
            // y number
            // name string
            // red number int
            // green number int
            // blue number int
            // alpha number int
            // forcePaused boolean
            // noBirthTime boolean
            /// @CSharpLua.Template = "Jass.Common["CreateUbersplat"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})"
            public static extern int CreateUbersplat(float x, float y, string name, float red, float green, float blue, float alpha, bool forcePaused, bool noBirthTime);

            // id number player
            // unitid number int
            // x number
            // y number
            // face number
            /// @CSharpLua.Template = "Jass.Common["CreateUnit"]({0}, {1}, {2}, {3}, {4})"
            public static extern int CreateUnit(float id, int unitid, float x, float y, int face);

            // id number player
            // unitid number int
            // whichLocation number location
            // face number
            /// @CSharpLua.Template = "Jass.Common["CreateUnitAtLoc"]({0}, {1}, {2}, {3})"
            public static extern int CreateUnitAtLoc(float id, int unitid, float whichLocation, int face);

            // id number player
            // unitname string
            // whichLocation number location
            // face number
            /// @CSharpLua.Template = "Jass.Common["CreateUnitAtLocByName"]({0}, {1}, {2}, {3})"
            public static extern int CreateUnitAtLocByName(float id, string unitname, float whichLocation, int face);

            // whichPlayer number player
            // unitname string
            // x number
            // y number
            // face number
            /// @CSharpLua.Template = "Jass.Common["CreateUnitByName"]({0}, {1}, {2}, {3}, {4})"
            public static extern int CreateUnitByName(int whichPlayer, string unitname, float x, float y, int face);

            /// @CSharpLua.Template = "Jass.Common["CreateUnitPool"]()"
            public static extern int CreateUnitPool();

            // whichPlayer number player
            // toWhichPlayers number force
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["CripplePlayer"]({0}, {1}, {2})"
            public static extern void CripplePlayer(int whichPlayer, float toWhichPlayers, bool flag);

            // 降低技能等级 [R]
            // 使[单位]的[技能]等级降低1级
            // 改变死亡单位的光环技能会导致魔兽崩溃.
            // whichUnit number unit
            // abilcode number int
            /// @CSharpLua.Template = "Jass.Common["DecUnitAbilityLevel"]({0}, {1})"
            public static extern int DecUnitAbilityLevel(int whichUnit, float abilcode);

            // 改变失败条件说明
            // 改变[Defeat Condition]的说明为:[文字]
            // whichCondition function defeatcondition
            // description string
            /// @CSharpLua.Template = "Jass.Common["DefeatConditionSetDescription"]({0}, {1})"
            public static extern void DefeatConditionSetDescription(int whichCondition, string description);

            // whichStartLoc number int
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["DefineStartLocation"]({0}, {1}, {2})"
            public static extern void DefineStartLocation(float whichStartLoc, float x, float y);

            // whichStartLoc number int
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["DefineStartLocationLoc"]({0}, {1})"
            public static extern void DefineStartLocationLoc(float whichStartLoc, float whichLocation);

            // degrees number
            /// @CSharpLua.Template = "Jass.Common["Deg2Rad"]({0})"
            public static extern float Deg2Rad(float degrees);

            // e function boolexpr
            /// @CSharpLua.Template = "Jass.Common["DestroyBoolExpr"]({0})"
            public static extern void DestroyBoolExpr(object e);

            // c function conditionfunc
            /// @CSharpLua.Template = "Jass.Common["DestroyCondition"]({0})"
            public static extern void DestroyCondition(object c);

            // 删除失败条件
            // 删除[Defeat Condition]
            // 被删除的失败条件会从每个任务中移除.
            // whichCondition function defeatcondition
            /// @CSharpLua.Template = "Jass.Common["DestroyDefeatCondition"]({0})"
            public static extern void DestroyDefeatCondition(int whichCondition);

            // 删除特效
            // 删除[Special Effect]
            // whichEffect number effect
            /// @CSharpLua.Template = "Jass.Common["DestroyEffect"]({0})"
            public static extern void DestroyEffect(int whichEffect);

            // f function filterfunc
            /// @CSharpLua.Template = "Jass.Common["DestroyFilter"]({0})"
            public static extern void DestroyFilter(int f);

            // 删除可见度修正器
            // 删除[Visibility Modifier]
            // whichFogModifier number fogmodifier
            /// @CSharpLua.Template = "Jass.Common["DestroyFogModifier"]({0})"
            public static extern void DestroyFogModifier(int whichFogModifier);

            // 删除玩家组 [R]
            // 删除[玩家组]
            // 注意: 不要删除系统预置的玩家组.
            // whichForce number force
            /// @CSharpLua.Template = "Jass.Common["DestroyForce"]({0})"
            public static extern void DestroyForce(int whichForce);

            // 删除单位组 [R]
            // 删除[单位组]
            // whichGroup number group
            /// @CSharpLua.Template = "Jass.Common["DestroyGroup"]({0})"
            public static extern void DestroyGroup(int whichGroup);

            // 删除
            // 删除[图像]
            // whichImage number image
            /// @CSharpLua.Template = "Jass.Common["DestroyImage"]({0})"
            public static extern void DestroyImage(int whichImage);

            // 删除物品池 [R]
            // 删除[物品池]
            // whichItemPool number itempool
            /// @CSharpLua.Template = "Jass.Common["DestroyItemPool"]({0})"
            public static extern void DestroyItemPool(int whichItemPool);

            // 删除
            // 删除[排行榜]
            // lb number leaderboard
            /// @CSharpLua.Template = "Jass.Common["DestroyLeaderboard"]({0})"
            public static extern void DestroyLeaderboard(int lb);

            // 删除闪电效果
            // 删除[某个闪电效果]
            // whichBolt number lightning
            /// @CSharpLua.Template = "Jass.Common["DestroyLightning"]({0})"
            public static extern bool DestroyLightning(int whichBolt);

            // 删除
            // 删除[某个多面板]
            // lb number multiboard
            /// @CSharpLua.Template = "Jass.Common["DestroyMultiboard"]({0})"
            public static extern void DestroyMultiboard(int lb);

            // 删除任务
            // 删除[某个任务]
            // 被删除的任务将不再显示在任务列表.
            // whichQuest number quest
            /// @CSharpLua.Template = "Jass.Common["DestroyQuest"]({0})"
            public static extern void DestroyQuest(int whichQuest);

            // 删除
            // 删除[Floating Text]
            // 游戏最多允许存在100个漂浮文字,所以请及时删除不再使用的漂浮文字.
            // t number texttag
            /// @CSharpLua.Template = "Jass.Common["DestroyTextTag"]({0})"
            public static extern void DestroyTextTag(int t);

            // 删除计时器 [R]
            // 删除[计时器]
            // 一般来说,计时器并不需要删除.只为某些有特别需求的用户提供.
            // whichTimer number timer
            /// @CSharpLua.Template = "Jass.Common["DestroyTimer"]({0})"
            public static extern void DestroyTimer(int whichTimer);

            // 删除计时器窗口
            // 删除[计时器窗口]
            // whichDialog number timerdialog
            /// @CSharpLua.Template = "Jass.Common["DestroyTimerDialog"]({0})"
            public static extern void DestroyTimerDialog(int whichDialog);

            // 删除触发器 [R]
            // 删除[触发器]
            // 对不再使用的触发器可以使用该动作来删除.
            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["DestroyTrigger"]({0})"
            public static extern void DestroyTrigger(int whichTrigger);

            // 删除地面纹理变化
            // 删除[某个地形纹理]
            // whichSplat any ubersplat
            /// @CSharpLua.Template = "Jass.Common["DestroyUbersplat"]({0})"
            public static extern void DestroyUbersplat(int whichSplat);

            // 删除单位池 [R]
            // 删除[单位池]
            // whichPool number unitpool
            /// @CSharpLua.Template = "Jass.Common["DestroyUnitPool"]({0})"
            public static extern void DestroyUnitPool(int whichPool);

            // 复活
            // 复活[某个可破坏物],设置生命值为[Value]并[显示/隐藏]生长动画
            // d number destructable
            // life number
            // birth boolean
            /// @CSharpLua.Template = "Jass.Common["DestructableRestoreLife"]({0}, {1}, {2})"
            public static extern void DestructableRestoreLife(int d, float life, bool birth);

            // 添加对话按钮
            // 为[某个对话框]添加一个对话按钮,按钮标签为[Text]
            // 使用'最后创建的对话按钮'来获得创建的对话按钮.
            // whichDialog number dialog
            // buttonText string
            // hotkey number int
            /// @CSharpLua.Template = "Jass.Common["DialogAddButton"]({0}, {1}, {2})"
            public static extern float DialogAddButton(int whichDialog, string buttonText, int hotkey);

            // 添加退出游戏按钮 [R]
            // 为[对话框]添加退出游戏按钮([跳过]计分屏) 按钮标题为:[文字]快捷键为:[HotKey]
            // 该函数创建的按钮并不被纪录到'最后创建的对话框按钮'.当该按钮被点击时会退出游戏
            // whichDialog number dialog
            // doScoreScreen boolean
            // buttonText string
            // hotkey number int
            /// @CSharpLua.Template = "Jass.Common["DialogAddQuitButton"]({0}, {1}, {2}, {3})"
            public static extern float DialogAddQuitButton(int whichDialog, bool doScoreScreen, string buttonText, int hotkey);

            // 清空
            // 清空[某个对话框]
            // 清除对话框的标题和按钮.
            // whichDialog number dialog
            /// @CSharpLua.Template = "Jass.Common["DialogClear"]({0})"
            public static extern void DialogClear(int whichDialog);

            /// @CSharpLua.Template = "Jass.Common["DialogCreate"]()"
            public static extern float DialogCreate();

            // 删除 [R]
            // 删除[对话框]
            // 将对话框清除出内存.一般来说对话框并不需要删除.
            // whichDialog number dialog
            /// @CSharpLua.Template = "Jass.Common["DialogDestroy"]({0})"
            public static extern void DialogDestroy(int whichDialog);

            // 显示/隐藏 [R]
            // 对[某个玩家]设置[对话框]的状态为[显示/隐藏]
            // 对话框不能应用于地图初始化事件.
            // whichPlayer number player
            // whichDialog number dialog
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["DialogDisplay"]({0}, {1}, {2})"
            public static extern void DialogDisplay(int whichPlayer, int whichDialog, bool flag);

            // 改变标题
            // 改变[某个对话框]的标题为[Title]
            // whichDialog number dialog
            // messageText string
            /// @CSharpLua.Template = "Jass.Common["DialogSetMessage"]({0}, {1})"
            public static extern void DialogSetMessage(int whichDialog, string messageText);

            // 禁用 重新开始任务按钮
            // 设置 重新开始任务按钮可以点击为[]
            // 当单人游戏时，可以设置重新开始任务按钮能否允许点击。
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["DisableRestartMission"]({0})"
            public static extern void DisableRestartMission(bool flag);

            // 关闭触发
            // 关闭[某个触发器]
            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["DisableTrigger"]({0})"
            public static extern void DisableTrigger(int whichTrigger);

            // 显示/隐藏 滤镜
            // [显示/隐藏]滤镜
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["DisplayCineFilter"]({0})"
            public static extern void DisplayCineFilter(bool flag);

            /// @CSharpLua.Template = "Jass.Common["DisplayLoadDialog"]()"
            public static extern void DisplayLoadDialog();

            // 对玩家显示文本消息(自动限时) [R]
            // 对[玩家]在屏幕位移([X],[Y])处显示文本:[文字]
            // 显示时间取决于文字长度. 位移的取值在0-1之间. 可使用'本地玩家'实现对所有玩家发送消息.
            // toPlayer number player
            // x number
            // y number
            // message string
            /// @CSharpLua.Template = "Jass.Common["DisplayTextToPlayer"]({0}, {1}, {2}, {3})"
            public static extern void DisplayTextToPlayer(int toPlayer, float x, float y, string message);

            // toPlayer number player
            // x number
            // y number
            // duration number
            // message string
            /// @CSharpLua.Template = "Jass.Common["DisplayTimedTextFromPlayer"]({0}, {1}, {2}, {3}, {4})"
            public static extern void DisplayTimedTextFromPlayer(int toPlayer, float x, float y, float duration, string message);

            // 对玩家显示文本消息(指定时间) [R]
            // 对[玩家]在屏幕位移([X],[Y])处显示[时间]秒的文本信息:[文字]
            // 位移的取值在0-1之间. 可使用'本地玩家[R]'实现对所有玩家发送消息.
            // toPlayer number player
            // x number
            // y number
            // duration number
            // message string
            /// @CSharpLua.Template = "Jass.Common["DisplayTimedTextToPlayer"]({0}, {1}, {2}, {3}, {4})"
            public static extern void DisplayTimedTextToPlayer(int toPlayer, float x, float y, float duration, string message);

            // 关闭游戏录像功能 [R]
            // 关闭游戏录像功能
            // 游戏结束时不保存游戏录像.
            /// @CSharpLua.Template = "Jass.Common["DoNotSaveReplay"]()"
            public static extern void DoNotSaveReplay();

            // 允许/禁用框选
            // [Enable/Disable]框选功能 ([Enable/Disable]显示选择框)
            // state boolean
            // ui boolean
            /// @CSharpLua.Template = "Jass.Common["EnableDragSelect"]({0}, {1})"
            public static extern void EnableDragSelect(bool state, bool ui);

            // 允许/禁用小地图按钮
            // [Enable/Disable]联盟颜色显示按钮,[Enable/Disable]中立生物显示按钮
            // enableAlly boolean
            // enableCreep boolean
            /// @CSharpLua.Template = "Jass.Common["EnableMinimapFilterButtons"]({0}, {1})"
            public static extern void EnableMinimapFilterButtons(bool enableAlly, bool enableCreep);

            // 允许/禁止闭塞(所有玩家) [R]
            // [Enable/Disable]闭塞
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["EnableOcclusion"]({0})"
            public static extern void EnableOcclusion(bool flag);

            // 允许/禁用预选
            // [Enable/Disable]预选功能 ([Enable/Disable]显示预选圈,生命槽,物体信息)
            // state boolean
            // ui boolean
            /// @CSharpLua.Template = "Jass.Common["EnablePreSelect"]({0}, {1})"
            public static extern void EnablePreSelect(bool state, bool ui);

            // 允许/禁用选择
            // [Enable/Disable]选择和取消选择功能 ([Enable/Disable]显示选择圈)
            // 禁用选择后仍可以通过触发来选择物体. 只有允许选择功能时才会显示选择圈.
            // state boolean
            // ui boolean
            /// @CSharpLua.Template = "Jass.Common["EnableSelect"]({0}, {1})"
            public static extern void EnableSelect(bool state, bool ui);

            // 开启触发
            // 开启[某个触发器]
            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["EnableTrigger"]({0})"
            public static extern void EnableTrigger(int whichTrigger);

            // 启用/禁用玩家控制权(所有玩家) [R]
            // [启用/禁用]玩家控制权
            // b boolean
            /// @CSharpLua.Template = "Jass.Common["EnableUserControl"]({0})"
            public static extern void EnableUserControl(bool b);

            // b boolean
            /// @CSharpLua.Template = "Jass.Common["EnableUserUI"]({0})"
            public static extern void EnableUserUI(bool b);

            // 启用/禁用 天气效果
            // 设置[Weather Effect]的状态为:[On/Off]
            // 可以使用'环境 - 创建天气效果'动作来创建天气效果.
            // whichEffect number weathereffect
            // enable boolean
            /// @CSharpLua.Template = "Jass.Common["EnableWeatherEffect"]({0}, {1})"
            public static extern void EnableWeatherEffect(int whichEffect, bool enable);

            // 允许/禁止 边界染色(所有玩家) [R]
            // [Enable/Disable]边界染色,应用于所有玩家
            // 禁用边界染色时边界为普通地形,不显示为黑色,但仍是不可通行的.
            // b boolean
            /// @CSharpLua.Template = "Jass.Common["EnableWorldFogBoundary"]({0})"
            public static extern void EnableWorldFogBoundary(bool b);

            /// @CSharpLua.Template = "Jass.Common["EndCinematicScene"]()"
            public static extern void EndCinematicScene();

            // doScoreScreen boolean
            /// @CSharpLua.Template = "Jass.Common["EndGame"]({0})"
            public static extern void EndGame(bool doScoreScreen);

            // 停止主题音乐[C]
            // 停止正在播放的主题音乐
            /// @CSharpLua.Template = "Jass.Common["EndThematicMusic"]()"
            public static extern void EndThematicMusic();

            // r number rect
            // filter function boolexpr
            // actionFunc any code
            /// @CSharpLua.Template = "Jass.Common["EnumDestructablesInRect"]({0}, {1}, {2})"
            public static extern void EnumDestructablesInRect(float r, object filter, object actionFunc);

            // 选取矩形区域内物品做动作
            // 选取[矩形区域]内所有物品[做动作]
            // 组动作中可使用'选取物品'来获取对应的物品. 区域内每个物品都会运行一次动作(包括隐藏物品,不包括单位身上的物品). 等待不能在组动作中运行.
            // r number rect
            // filter function boolexpr
            // actionFunc any code
            /// @CSharpLua.Template = "Jass.Common["EnumItemsInRect"]({0}, {1}, {2})"
            public static extern void EnumItemsInRect(float r, object filter, object actionFunc);

            // 运行函数 [R]
            // 运行:[函数名]
            // 使用该功能运行的函数与触发独立, 只能运行自定义无参数函数.
            // funcName string
            /// @CSharpLua.Template = "Jass.Common["ExecuteFunc"]({0})"
            public static extern void ExecuteFunc(string funcName);

            // func any code
            /// @CSharpLua.Template = "Jass.Common["Filter"]({0})"
            public static extern object Filter(object func);

            // 结束地面纹理变化
            // 结束[某个地形纹理]
            // 在动画播放完毕时自动清除该地面纹理变化.
            // whichSplat any ubersplat
            /// @CSharpLua.Template = "Jass.Common["FinishUbersplat"]({0})"
            public static extern void FinishUbersplat(int whichSplat);

            // whichGroup number group
            /// @CSharpLua.Template = "Jass.Common["FirstOfGroup"]({0})"
            public static extern int FirstOfGroup(int whichGroup);

            // 闪动任务按钮
            /// @CSharpLua.Template = "Jass.Common["FlashQuestDialogButton"]()"
            public static extern void FlashQuestDialogButton();

            // @1.24 清空哈希表主索引 [C]
            // 清空[Hashtable]中位于主索引[Value] 之内的所有数据
            // 清空哈希表主索引下的所有数据
            // table any hashtable
            // parentKey number int
            /// @CSharpLua.Template = "Jass.Common["FlushChildHashtable"]({0}, {1})"
            public static extern void FlushChildHashtable(object table, float parentKey);

            // 删除缓存 [C]
            // 删除[GameCache]
            // 删除并清空该缓存的所有数据.和原版UI完全一致，但却不能兼容原版UI，它的存在完全是在添乱啊
            // cache any gamecache
            /// @CSharpLua.Template = "Jass.Common["FlushGameCache"]({0})"
            public static extern void FlushGameCache(object cache);

            // @1.24 清空哈希表 [C]
            // 清空[Hashtable]
            // 清空哈希表所有数据
            // table any hashtable
            /// @CSharpLua.Template = "Jass.Common["FlushParentHashtable"]({0})"
            public static extern void FlushParentHashtable(object table);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["FlushStoredBoolean"]({0}, {1}, {2})"
            public static extern void FlushStoredBoolean(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["FlushStoredInteger"]({0}, {1}, {2})"
            public static extern void FlushStoredInteger(object cache, string missionKey, string key);

            // 删除类别
            // 删除缓存[GameCache]中[Category]类别
            // 清空该类别下的所有缓存数据.
            // cache any gamecache
            // missionKey string
            /// @CSharpLua.Template = "Jass.Common["FlushStoredMission"]({0}, {1})"
            public static extern void FlushStoredMission(object cache, string missionKey);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["FlushStoredReal"]({0}, {1}, {2})"
            public static extern void FlushStoredReal(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["FlushStoredString"]({0}, {1}, {2})"
            public static extern void FlushStoredString(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["FlushStoredUnit"]({0}, {1}, {2})"
            public static extern void FlushStoredUnit(object cache, string missionKey, string key);

            // 启用/禁用 战争迷雾 [R]
            // [启用/禁用]战争迷雾
            // enable boolean
            /// @CSharpLua.Template = "Jass.Common["FogEnable"]({0})"
            public static extern void FogEnable(bool enable);

            // 启用/禁用黑色阴影 [R]
            // [启用/禁用]黑色阴影
            // enable boolean
            /// @CSharpLua.Template = "Jass.Common["FogMaskEnable"]({0})"
            public static extern void FogMaskEnable(bool enable);

            // 启用可见度修正器
            // 启用[Visibility Modifier]
            // whichFogModifier number fogmodifier
            /// @CSharpLua.Template = "Jass.Common["FogModifierStart"]({0})"
            public static extern void FogModifierStart(float whichFogModifier);

            // 禁用可见度修正器
            // 禁用[Visibility Modifier]
            // whichFogModifier number fogmodifier
            /// @CSharpLua.Template = "Jass.Common["FogModifierStop"]({0})"
            public static extern void FogModifierStop(float whichFogModifier);

            // 选取玩家组内玩家做动作
            // 选取[玩家组]内所有玩家[做动作]
            // 玩家组动作中可使用'选取玩家'来获取对应的玩家. 等待不能在组动作中运行.
            // whichForce number force
            // callback any code
            /// @CSharpLua.Template = "Jass.Common["ForForce"]({0}, {1})"
            public static extern void ForForce(int whichForce, object callback);

            // 选取单位组内单位做动作
            // 选取[单位组]内所有单位[做动作]
            // 使用'选取单位'来取代相应的单位. 对于单位组内每个单位都会运行一次动作(包括死亡的,不包括隐藏的). 等待不能在组动作中运行.
            // whichGroup number group
            // callback any code
            /// @CSharpLua.Template = "Jass.Common["ForGroup"]({0}, {1})"
            public static extern void ForGroup(float whichGroup, object callback);

            // 添加玩家 [R]
            // [玩家组]添加[玩家]
            // 并不影响玩家本身.
            // whichForce number force
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["ForceAddPlayer"]({0}, {1})"
            public static extern void ForceAddPlayer(int whichForce, int whichPlayer);

            /// @CSharpLua.Template = "Jass.Common["ForceCampaignSelectScreen"]()"
            public static extern void ForceCampaignSelectScreen();

            // 字幕显示
            // [On/Off]电影字幕显示功能
            // 该功能和'游戏菜单-声音选项'中的字幕选项中有一项为开时即能够显示电影字幕.
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["ForceCinematicSubtitles"]({0})"
            public static extern void ForceCinematicSubtitles(bool flag);

            // 清空玩家组
            // 清空[玩家组]内所有玩家
            // 并不影响玩家本身.
            // whichForce number force
            /// @CSharpLua.Template = "Jass.Common["ForceClear"]({0})"
            public static extern void ForceClear(int whichForce);

            // whichForce number force
            // whichPlayer number player
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["ForceEnumAllies"]({0}, {1}, {2})"
            public static extern void ForceEnumAllies(int whichForce, int whichPlayer, object filter);

            // whichForce number force
            // whichPlayer number player
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["ForceEnumEnemies"]({0}, {1}, {2})"
            public static extern void ForceEnumEnemies(int whichForce, int whichPlayer, object filter);

            // whichForce number force
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["ForceEnumPlayers"]({0}, {1})"
            public static extern void ForceEnumPlayers(int whichForce, object filter);

            // whichForce number force
            // filter function boolexpr
            // countLimit number int
            /// @CSharpLua.Template = "Jass.Common["ForceEnumPlayersCounted"]({0}, {1}, {2})"
            public static extern void ForceEnumPlayersCounted(int whichForce, object filter, float countLimit);

            // whichPlayer number player
            // startLocIndex number int
            /// @CSharpLua.Template = "Jass.Common["ForcePlayerStartLocation"]({0}, {1})"
            public static extern void ForcePlayerStartLocation(int whichPlayer, float startLocIndex);

            /// @CSharpLua.Template = "Jass.Common["ForceQuestDialogUpdate"]()"
            public static extern void ForceQuestDialogUpdate();

            // 移除玩家 [R]
            // 从[玩家组]中移除[玩家]
            // 并不影响玩家本身.
            // whichForce number force
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["ForceRemovePlayer"]({0}, {1})"
            public static extern void ForceRemovePlayer(int whichForce, int whichPlayer);

            // 强制按Esc键
            // 命令[某个玩家]按下Esc键
            /// @CSharpLua.Template = "Jass.Common["ForceUICancel"]()"
            public static extern void ForceUICancel();

            // 强制按键
            // 命令[某个玩家]按下[Key]键
            // key string
            /// @CSharpLua.Template = "Jass.Common["ForceUIKey"]({0})"
            public static extern void ForceUIKey(string key);

            // num number player
            /// @CSharpLua.Template = "Jass.Common["GetAIDifficulty"]({0})"
            public static extern object GetAIDifficulty(int num);

            // abilityString string
            // t any effecttype
            // index number int
            /// @CSharpLua.Template = "Jass.Common["GetAbilityEffect"]({0}, {1}, {2})"
            public static extern string GetAbilityEffect(string abilityString, object t, float index);

            // abilityId number int
            // t any effecttype
            // index number int
            /// @CSharpLua.Template = "Jass.Common["GetAbilityEffectById"]({0}, {1}, {2})"
            public static extern string GetAbilityEffectById(int abilityId, object t, float index);

            // abilityString string
            // t any soundtype
            /// @CSharpLua.Template = "Jass.Common["GetAbilitySound"]({0}, {1})"
            public static extern string GetAbilitySound(string abilityString, object t);

            // abilityId number int
            // t any soundtype
            /// @CSharpLua.Template = "Jass.Common["GetAbilitySoundById"]({0}, {1})"
            public static extern string GetAbilitySoundById(int abilityId, object t);

            /// @CSharpLua.Template = "Jass.Common["GetAllyColorFilterState"]()"
            public static extern float GetAllyColorFilterState();

            /// @CSharpLua.Template = "Jass.Common["GetAttacker"]()"
            public static extern float GetAttacker();

            /// @CSharpLua.Template = "Jass.Common["GetBuyingUnit"]()"
            public static extern float GetBuyingUnit();

            /// @CSharpLua.Template = "Jass.Common["GetCameraBoundMaxX"]()"
            public static extern float GetCameraBoundMaxX();

            /// @CSharpLua.Template = "Jass.Common["GetCameraBoundMaxY"]()"
            public static extern float GetCameraBoundMaxY();

            /// @CSharpLua.Template = "Jass.Common["GetCameraBoundMinX"]()"
            public static extern float GetCameraBoundMinX();

            /// @CSharpLua.Template = "Jass.Common["GetCameraBoundMinY"]()"
            public static extern float GetCameraBoundMinY();

            /// @CSharpLua.Template = "Jass.Common["GetCameraEyePositionLoc"]()"
            public static extern float GetCameraEyePositionLoc();

            /// @CSharpLua.Template = "Jass.Common["GetCameraEyePositionX"]()"
            public static extern float GetCameraEyePositionX();

            /// @CSharpLua.Template = "Jass.Common["GetCameraEyePositionY"]()"
            public static extern float GetCameraEyePositionY();

            /// @CSharpLua.Template = "Jass.Common["GetCameraEyePositionZ"]()"
            public static extern float GetCameraEyePositionZ();

            // whichField any camerafield
            /// @CSharpLua.Template = "Jass.Common["GetCameraField"]({0})"
            public static extern float GetCameraField(object whichField);

            // whichMargin number int
            /// @CSharpLua.Template = "Jass.Common["GetCameraMargin"]({0})"
            public static extern float GetCameraMargin(float whichMargin);

            /// @CSharpLua.Template = "Jass.Common["GetCameraTargetPositionLoc"]()"
            public static extern float GetCameraTargetPositionLoc();

            /// @CSharpLua.Template = "Jass.Common["GetCameraTargetPositionX"]()"
            public static extern float GetCameraTargetPositionX();

            /// @CSharpLua.Template = "Jass.Common["GetCameraTargetPositionY"]()"
            public static extern float GetCameraTargetPositionY();

            /// @CSharpLua.Template = "Jass.Common["GetCameraTargetPositionZ"]()"
            public static extern float GetCameraTargetPositionZ();

            /// @CSharpLua.Template = "Jass.Common["GetCancelledStructure"]()"
            public static extern float GetCancelledStructure();

            /// @CSharpLua.Template = "Jass.Common["GetChangingUnit"]()"
            public static extern float GetChangingUnit();

            /// @CSharpLua.Template = "Jass.Common["GetChangingUnitPrevOwner"]()"
            public static extern float GetChangingUnitPrevOwner();

            /// @CSharpLua.Template = "Jass.Common["GetClickedButton"]()"
            public static extern float GetClickedButton();

            /// @CSharpLua.Template = "Jass.Common["GetClickedDialog"]()"
            public static extern float GetClickedDialog();

            /// @CSharpLua.Template = "Jass.Common["GetConstructedStructure"]()"
            public static extern float GetConstructedStructure();

            /// @CSharpLua.Template = "Jass.Common["GetConstructingStructure"]()"
            public static extern float GetConstructingStructure();

            /// @CSharpLua.Template = "Jass.Common["GetCreatureDensity"]()"
            public static extern object GetCreatureDensity();

            /// @CSharpLua.Template = "Jass.Common["GetCreepCampFilterState"]()"
            public static extern bool GetCreepCampFilterState();

            // whichButton number int
            /// @CSharpLua.Template = "Jass.Common["GetCustomCampaignButtonVisible"]({0})"
            public static extern bool GetCustomCampaignButtonVisible(float whichButton);

            /// @CSharpLua.Template = "Jass.Common["GetDecayingUnit"]()"
            public static extern float GetDecayingUnit();

            /// @CSharpLua.Template = "Jass.Common["GetDefaultDifficulty"]()"
            public static extern object GetDefaultDifficulty();

            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["GetDestructableLife"]({0})"
            public static extern float GetDestructableLife(float d);

            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["GetDestructableMaxLife"]({0})"
            public static extern float GetDestructableMaxLife(float d);

            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["GetDestructableName"]({0})"
            public static extern string GetDestructableName(float d);

            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["GetDestructableOccluderHeight"]({0})"
            public static extern float GetDestructableOccluderHeight(float d);

            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["GetDestructableTypeId"]({0})"
            public static extern float GetDestructableTypeId(float d);

            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["GetDestructableX"]({0})"
            public static extern float GetDestructableX(float d);

            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["GetDestructableY"]({0})"
            public static extern float GetDestructableY(float d);

            /// @CSharpLua.Template = "Jass.Common["GetDetectedUnit"]()"
            public static extern float GetDetectedUnit();

            /// @CSharpLua.Template = "Jass.Common["GetDyingUnit"]()"
            public static extern float GetDyingUnit();

            /// @CSharpLua.Template = "Jass.Common["GetEnteringUnit"]()"
            public static extern float GetEnteringUnit();

            /// @CSharpLua.Template = "Jass.Common["GetEnumDestructable"]()"
            public static extern float GetEnumDestructable();

            /// @CSharpLua.Template = "Jass.Common["GetEnumItem"]()"
            public static extern float GetEnumItem();

            /// @CSharpLua.Template = "Jass.Common["GetEnumPlayer"]()"
            public static extern float GetEnumPlayer();

            /// @CSharpLua.Template = "Jass.Common["GetEnumUnit"]()"
            public static extern float GetEnumUnit();

            /// @CSharpLua.Template = "Jass.Common["GetEventDamage"]()"
            public static extern float GetEventDamage();

            /// @CSharpLua.Template = "Jass.Common["GetEventDamageSource"]()"
            public static extern float GetEventDamageSource();

            /// @CSharpLua.Template = "Jass.Common["GetEventDetectingPlayer"]()"
            public static extern float GetEventDetectingPlayer();

            /// @CSharpLua.Template = "Jass.Common["GetEventGameState"]()"
            public static extern object GetEventGameState();

            /// @CSharpLua.Template = "Jass.Common["GetEventPlayerChatString"]()"
            public static extern string GetEventPlayerChatString();

            /// @CSharpLua.Template = "Jass.Common["GetEventPlayerChatStringMatched"]()"
            public static extern string GetEventPlayerChatStringMatched();

            /// @CSharpLua.Template = "Jass.Common["GetEventPlayerState"]()"
            public static extern object GetEventPlayerState();

            /// @CSharpLua.Template = "Jass.Common["GetEventTargetUnit"]()"
            public static extern float GetEventTargetUnit();

            /// @CSharpLua.Template = "Jass.Common["GetEventUnitState"]()"
            public static extern object GetEventUnitState();

            /// @CSharpLua.Template = "Jass.Common["GetExpiredTimer"]()"
            public static extern float GetExpiredTimer();

            /// @CSharpLua.Template = "Jass.Common["GetFilterDestructable"]()"
            public static extern float GetFilterDestructable();

            /// @CSharpLua.Template = "Jass.Common["GetFilterItem"]()"
            public static extern float GetFilterItem();

            /// @CSharpLua.Template = "Jass.Common["GetFilterPlayer"]()"
            public static extern float GetFilterPlayer();

            /// @CSharpLua.Template = "Jass.Common["GetFilterUnit"]()"
            public static extern float GetFilterUnit();

            // whichFloatGameState any fgamestate
            /// @CSharpLua.Template = "Jass.Common["GetFloatGameState"]({0})"
            public static extern float GetFloatGameState(object whichFloatGameState);

            // unitId number int
            /// @CSharpLua.Template = "Jass.Common["GetFoodMade"]({0})"
            public static extern float GetFoodMade(int unitId);

            // unitId number int
            /// @CSharpLua.Template = "Jass.Common["GetFoodUsed"]({0})"
            public static extern float GetFoodUsed(int unitId);

            /// @CSharpLua.Template = "Jass.Common["GetGameDifficulty"]()"
            public static extern object GetGameDifficulty();

            /// @CSharpLua.Template = "Jass.Common["GetGamePlacement"]()"
            public static extern object GetGamePlacement();

            /// @CSharpLua.Template = "Jass.Common["GetGameSpeed"]()"
            public static extern object GetGameSpeed();

            /// @CSharpLua.Template = "Jass.Common["GetGameTypeSelected"]()"
            public static extern object GetGameTypeSelected();

            // h any handle
            /// @CSharpLua.Template = "Jass.Common["GetHandleId"]({0})"
            public static extern float GetHandleId(object h);

            // whichHero number unit
            // includeBonuses boolean
            /// @CSharpLua.Template = "Jass.Common["GetHeroAgi"]({0}, {1})"
            public static extern float GetHeroAgi(float whichHero, bool includeBonuses);

            // whichHero number unit
            // includeBonuses boolean
            /// @CSharpLua.Template = "Jass.Common["GetHeroInt"]({0}, {1})"
            public static extern float GetHeroInt(float whichHero, bool includeBonuses);

            // whichHero number unit
            /// @CSharpLua.Template = "Jass.Common["GetHeroLevel"]({0})"
            public static extern float GetHeroLevel(float whichHero);

            // whichHero number unit
            /// @CSharpLua.Template = "Jass.Common["GetHeroProperName"]({0})"
            public static extern string GetHeroProperName(float whichHero);

            // whichHero number unit
            /// @CSharpLua.Template = "Jass.Common["GetHeroSkillPoints"]({0})"
            public static extern float GetHeroSkillPoints(float whichHero);

            // whichHero number unit
            // includeBonuses boolean
            /// @CSharpLua.Template = "Jass.Common["GetHeroStr"]({0}, {1})"
            public static extern float GetHeroStr(float whichHero, bool includeBonuses);

            // whichHero number unit
            /// @CSharpLua.Template = "Jass.Common["GetHeroXP"]({0})"
            public static extern float GetHeroXP(float whichHero);

            // whichIntegerGameState any igamestate
            /// @CSharpLua.Template = "Jass.Common["GetIntegerGameState"]({0})"
            public static extern float GetIntegerGameState(object whichIntegerGameState);

            /// @CSharpLua.Template = "Jass.Common["GetIssuedOrderId"]()"
            public static extern float GetIssuedOrderId();

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["GetItemCharges"]({0})"
            public static extern float GetItemCharges(float whichItem);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["GetItemLevel"]({0})"
            public static extern float GetItemLevel(float whichItem);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["GetItemName"]({0})"
            public static extern string GetItemName(float whichItem);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["GetItemPlayer"]({0})"
            public static extern float GetItemPlayer(float whichItem);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["GetItemType"]({0})"
            public static extern object GetItemType(float whichItem);

            // i number item
            /// @CSharpLua.Template = "Jass.Common["GetItemTypeId"]({0})"
            public static extern float GetItemTypeId(float i);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["GetItemUserData"]({0})"
            public static extern float GetItemUserData(float whichItem);

            // i number item
            /// @CSharpLua.Template = "Jass.Common["GetItemX"]({0})"
            public static extern float GetItemX(float i);

            // i number item
            /// @CSharpLua.Template = "Jass.Common["GetItemY"]({0})"
            public static extern float GetItemY(float i);

            /// @CSharpLua.Template = "Jass.Common["GetKillingUnit"]()"
            public static extern float GetKillingUnit();

            /// @CSharpLua.Template = "Jass.Common["GetLearnedSkill"]()"
            public static extern float GetLearnedSkill();

            /// @CSharpLua.Template = "Jass.Common["GetLearnedSkillLevel"]()"
            public static extern float GetLearnedSkillLevel();

            /// @CSharpLua.Template = "Jass.Common["GetLearningUnit"]()"
            public static extern float GetLearningUnit();

            /// @CSharpLua.Template = "Jass.Common["GetLeavingUnit"]()"
            public static extern float GetLeavingUnit();

            /// @CSharpLua.Template = "Jass.Common["GetLevelingUnit"]()"
            public static extern float GetLevelingUnit();

            // whichBolt number lightning
            /// @CSharpLua.Template = "Jass.Common["GetLightningColorA"]({0})"
            public static extern float GetLightningColorA(float whichBolt);

            // whichBolt number lightning
            /// @CSharpLua.Template = "Jass.Common["GetLightningColorB"]({0})"
            public static extern float GetLightningColorB(float whichBolt);

            // whichBolt number lightning
            /// @CSharpLua.Template = "Jass.Common["GetLightningColorG"]({0})"
            public static extern float GetLightningColorG(float whichBolt);

            // whichBolt number lightning
            /// @CSharpLua.Template = "Jass.Common["GetLightningColorR"]({0})"
            public static extern float GetLightningColorR(float whichBolt);

            /// @CSharpLua.Template = "Jass.Common["GetLoadedUnit"]()"
            public static extern float GetLoadedUnit();

            // source string
            /// @CSharpLua.Template = "Jass.Common["GetLocalizedHotkey"]({0})"
            public static extern float GetLocalizedHotkey(string source);

            // source string
            /// @CSharpLua.Template = "Jass.Common["GetLocalizedString"]({0})"
            public static extern string GetLocalizedString(string source);

            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["GetLocationX"]({0})"
            public static extern float GetLocationX(float whichLocation);

            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["GetLocationY"]({0})"
            public static extern float GetLocationY(float whichLocation);

            /// @CSharpLua.Template = "Jass.Common["GetManipulatedItem"]()"
            public static extern float GetManipulatedItem();

            /// @CSharpLua.Template = "Jass.Common["GetManipulatingUnit"]()"
            public static extern float GetManipulatingUnit();

            // objectId number int
            /// @CSharpLua.Template = "Jass.Common["GetObjectName"]({0})"
            public static extern string GetObjectName(float objectId);

            /// @CSharpLua.Template = "Jass.Common["GetOrderPointLoc"]()"
            public static extern float GetOrderPointLoc();

            /// @CSharpLua.Template = "Jass.Common["GetOrderPointX"]()"
            public static extern float GetOrderPointX();

            /// @CSharpLua.Template = "Jass.Common["GetOrderPointY"]()"
            public static extern float GetOrderPointY();

            /// @CSharpLua.Template = "Jass.Common["GetOrderTarget"]()"
            public static extern float GetOrderTarget();

            /// @CSharpLua.Template = "Jass.Common["GetOrderTargetDestructable"]()"
            public static extern float GetOrderTargetDestructable();

            /// @CSharpLua.Template = "Jass.Common["GetOrderTargetItem"]()"
            public static extern float GetOrderTargetItem();

            /// @CSharpLua.Template = "Jass.Common["GetOrderTargetUnit"]()"
            public static extern float GetOrderTargetUnit();

            /// @CSharpLua.Template = "Jass.Common["GetOrderedUnit"]()"
            public static extern float GetOrderedUnit();

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetOwningPlayer"]({0})"
            public static extern float GetOwningPlayer(int whichUnit);

            // sourcePlayer number player
            // otherPlayer number player
            // whichAllianceSetting any alliancetype
            /// @CSharpLua.Template = "Jass.Common["GetPlayerAlliance"]({0}, {1}, {2})"
            public static extern bool GetPlayerAlliance(float sourcePlayer, float otherPlayer, object whichAllianceSetting);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerColor"]({0})"
            public static extern object GetPlayerColor(int whichPlayer);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerController"]({0})"
            public static extern object GetPlayerController(int whichPlayer);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerHandicap"]({0})"
            public static extern float GetPlayerHandicap(int whichPlayer);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerHandicapXP"]({0})"
            public static extern float GetPlayerHandicapXP(int whichPlayer);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerId"]({0})"
            public static extern float GetPlayerId(int whichPlayer);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerName"]({0})"
            public static extern string GetPlayerName(int whichPlayer);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerRace"]({0})"
            public static extern object GetPlayerRace(int whichPlayer);

            // whichPlayer number player
            // whichPlayerScore any playerscore
            /// @CSharpLua.Template = "Jass.Common["GetPlayerScore"]({0}, {1})"
            public static extern float GetPlayerScore(int whichPlayer, object whichPlayerScore);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerSelectable"]({0})"
            public static extern bool GetPlayerSelectable(int whichPlayer);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerSlotState"]({0})"
            public static extern object GetPlayerSlotState(int whichPlayer);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerStartLocation"]({0})"
            public static extern float GetPlayerStartLocation(int whichPlayer);

            // whichPlayer number player
            // whichPlayerState any playerstate
            /// @CSharpLua.Template = "Jass.Common["GetPlayerState"]({0}, {1})"
            public static extern float GetPlayerState(int whichPlayer, object whichPlayerState);

            // whichPlayer number player
            // includeIncomplete boolean
            /// @CSharpLua.Template = "Jass.Common["GetPlayerStructureCount"]({0}, {1})"
            public static extern float GetPlayerStructureCount(int whichPlayer, bool includeIncomplete);

            // sourcePlayer number player
            // otherPlayer number player
            // whichResource any playerstate
            /// @CSharpLua.Template = "Jass.Common["GetPlayerTaxRate"]({0}, {1}, {2})"
            public static extern float GetPlayerTaxRate(float sourcePlayer, float otherPlayer, object whichResource);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetPlayerTeam"]({0})"
            public static extern float GetPlayerTeam(int whichPlayer);

            // whichPlayer number player
            // techid number int
            // specificonly boolean
            /// @CSharpLua.Template = "Jass.Common["GetPlayerTechCount"]({0}, {1}, {2})"
            public static extern float GetPlayerTechCount(int whichPlayer, float techid, bool specificonly);

            // whichPlayer number player
            // techid number int
            /// @CSharpLua.Template = "Jass.Common["GetPlayerTechMaxAllowed"]({0}, {1})"
            public static extern float GetPlayerTechMaxAllowed(int whichPlayer, float techid);

            // whichPlayer number player
            // techid number int
            // specificonly boolean
            /// @CSharpLua.Template = "Jass.Common["GetPlayerTechResearched"]({0}, {1}, {2})"
            public static extern bool GetPlayerTechResearched(int whichPlayer, float techid, bool specificonly);

            // whichPlayer number player
            // unitName string
            // includeIncomplete boolean
            // includeUpgrades boolean
            /// @CSharpLua.Template = "Jass.Common["GetPlayerTypedUnitCount"]({0}, {1}, {2}, {3})"
            public static extern float GetPlayerTypedUnitCount(int whichPlayer, string unitName, bool includeIncomplete, bool includeUpgrades);

            // whichPlayer number player
            // includeIncomplete boolean
            /// @CSharpLua.Template = "Jass.Common["GetPlayerUnitCount"]({0}, {1})"
            public static extern float GetPlayerUnitCount(int whichPlayer, bool includeIncomplete);

            /// @CSharpLua.Template = "Jass.Common["GetPlayers"]()"
            public static extern float GetPlayers();

            // lowBound number int
            // highBound number int
            /// @CSharpLua.Template = "Jass.Common["GetRandomInt"]({0}, {1})"
            public static extern float GetRandomInt(float lowBound, float highBound);

            // lowBound number
            // highBound number
            /// @CSharpLua.Template = "Jass.Common["GetRandomReal"]({0}, {1})"
            public static extern float GetRandomReal(float lowBound, float highBound);

            // whichRect number rect
            /// @CSharpLua.Template = "Jass.Common["GetRectCenterX"]({0})"
            public static extern float GetRectCenterX(float whichRect);

            // whichRect number rect
            /// @CSharpLua.Template = "Jass.Common["GetRectCenterY"]({0})"
            public static extern float GetRectCenterY(float whichRect);

            // whichRect number rect
            /// @CSharpLua.Template = "Jass.Common["GetRectMaxX"]({0})"
            public static extern float GetRectMaxX(float whichRect);

            // whichRect number rect
            /// @CSharpLua.Template = "Jass.Common["GetRectMaxY"]({0})"
            public static extern float GetRectMaxY(float whichRect);

            // whichRect number rect
            /// @CSharpLua.Template = "Jass.Common["GetRectMinX"]({0})"
            public static extern float GetRectMinX(float whichRect);

            // whichRect number rect
            /// @CSharpLua.Template = "Jass.Common["GetRectMinY"]({0})"
            public static extern float GetRectMinY(float whichRect);

            /// @CSharpLua.Template = "Jass.Common["GetRescuer"]()"
            public static extern float GetRescuer();

            /// @CSharpLua.Template = "Jass.Common["GetResearched"]()"
            public static extern float GetResearched();

            /// @CSharpLua.Template = "Jass.Common["GetResearchingUnit"]()"
            public static extern float GetResearchingUnit();

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetResourceAmount"]({0})"
            public static extern float GetResourceAmount(int whichUnit);

            /// @CSharpLua.Template = "Jass.Common["GetResourceDensity"]()"
            public static extern object GetResourceDensity();

            /// @CSharpLua.Template = "Jass.Common["GetRevivableUnit"]()"
            public static extern float GetRevivableUnit();

            /// @CSharpLua.Template = "Jass.Common["GetRevivingUnit"]()"
            public static extern float GetRevivingUnit();

            /// @CSharpLua.Template = "Jass.Common["GetSaveBasicFilename"]()"
            public static extern string GetSaveBasicFilename();

            /// @CSharpLua.Template = "Jass.Common["GetSellingUnit"]()"
            public static extern float GetSellingUnit();

            /// @CSharpLua.Template = "Jass.Common["GetSoldItem"]()"
            public static extern float GetSoldItem();

            /// @CSharpLua.Template = "Jass.Common["GetSoldUnit"]()"
            public static extern float GetSoldUnit();

            // soundHandle number sound
            /// @CSharpLua.Template = "Jass.Common["GetSoundDuration"]({0})"
            public static extern float GetSoundDuration(float soundHandle);

            // musicFileName string
            /// @CSharpLua.Template = "Jass.Common["GetSoundFileDuration"]({0})"
            public static extern float GetSoundFileDuration(string musicFileName);

            // soundHandle number sound
            /// @CSharpLua.Template = "Jass.Common["GetSoundIsLoading"]({0})"
            public static extern bool GetSoundIsLoading(float soundHandle);

            // soundHandle number sound
            /// @CSharpLua.Template = "Jass.Common["GetSoundIsPlaying"]({0})"
            public static extern bool GetSoundIsPlaying(float soundHandle);

            /// @CSharpLua.Template = "Jass.Common["GetSpellAbility"]()"
            public static extern float GetSpellAbility();

            /// @CSharpLua.Template = "Jass.Common["GetSpellAbilityId"]()"
            public static extern float GetSpellAbilityId();

            /// @CSharpLua.Template = "Jass.Common["GetSpellAbilityUnit"]()"
            public static extern float GetSpellAbilityUnit();

            /// @CSharpLua.Template = "Jass.Common["GetSpellTargetDestructable"]()"
            public static extern float GetSpellTargetDestructable();

            /// @CSharpLua.Template = "Jass.Common["GetSpellTargetItem"]()"
            public static extern float GetSpellTargetItem();

            /// @CSharpLua.Template = "Jass.Common["GetSpellTargetLoc"]()"
            public static extern float GetSpellTargetLoc();

            /// @CSharpLua.Template = "Jass.Common["GetSpellTargetUnit"]()"
            public static extern float GetSpellTargetUnit();

            /// @CSharpLua.Template = "Jass.Common["GetSpellTargetX"]()"
            public static extern float GetSpellTargetX();

            /// @CSharpLua.Template = "Jass.Common["GetSpellTargetY"]()"
            public static extern float GetSpellTargetY();

            // whichStartLoc number int
            // prioSlotIndex number int
            /// @CSharpLua.Template = "Jass.Common["GetStartLocPrio"]({0}, {1})"
            public static extern object GetStartLocPrio(float whichStartLoc, float prioSlotIndex);

            // whichStartLoc number int
            // prioSlotIndex number int
            /// @CSharpLua.Template = "Jass.Common["GetStartLocPrioSlot"]({0}, {1})"
            public static extern float GetStartLocPrioSlot(float whichStartLoc, float prioSlotIndex);

            // whichStartLocation number int
            /// @CSharpLua.Template = "Jass.Common["GetStartLocationLoc"]({0})"
            public static extern float GetStartLocationLoc(float whichStartLocation);

            // whichStartLocation number int
            /// @CSharpLua.Template = "Jass.Common["GetStartLocationX"]({0})"
            public static extern float GetStartLocationX(float whichStartLocation);

            // whichStartLocation number int
            /// @CSharpLua.Template = "Jass.Common["GetStartLocationY"]({0})"
            public static extern float GetStartLocationY(float whichStartLocation);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["GetStoredBoolean"]({0}, {1}, {2})"
            public static extern bool GetStoredBoolean(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["GetStoredInteger"]({0}, {1}, {2})"
            public static extern float GetStoredInteger(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["GetStoredReal"]({0}, {1}, {2})"
            public static extern float GetStoredReal(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["GetStoredString"]({0}, {1}, {2})"
            public static extern string GetStoredString(object cache, string missionKey, string key);

            /// @CSharpLua.Template = "Jass.Common["GetSummonedUnit"]()"
            public static extern float GetSummonedUnit();

            /// @CSharpLua.Template = "Jass.Common["GetSummoningUnit"]()"
            public static extern float GetSummoningUnit();

            /// @CSharpLua.Template = "Jass.Common["GetTeams"]()"
            public static extern float GetTeams();

            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["GetTerrainCliffLevel"]({0}, {1})"
            public static extern float GetTerrainCliffLevel(float x, float y);

            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["GetTerrainType"]({0}, {1})"
            public static extern float GetTerrainType(float x, float y);

            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["GetTerrainVariance"]({0}, {1})"
            public static extern float GetTerrainVariance(float x, float y);

            /// @CSharpLua.Template = "Jass.Common["GetTimeOfDayScale"]()"
            public static extern float GetTimeOfDayScale();

            /// @CSharpLua.Template = "Jass.Common["GetTournamentFinishNowPlayer"]()"
            public static extern float GetTournamentFinishNowPlayer();

            /// @CSharpLua.Template = "Jass.Common["GetTournamentFinishNowRule"]()"
            public static extern float GetTournamentFinishNowRule();

            /// @CSharpLua.Template = "Jass.Common["GetTournamentFinishSoonTimeRemaining"]()"
            public static extern float GetTournamentFinishSoonTimeRemaining();

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["GetTournamentScore"]({0})"
            public static extern float GetTournamentScore(int whichPlayer);

            /// @CSharpLua.Template = "Jass.Common["GetTrainedUnit"]()"
            public static extern float GetTrainedUnit();

            /// @CSharpLua.Template = "Jass.Common["GetTrainedUnitType"]()"
            public static extern float GetTrainedUnitType();

            /// @CSharpLua.Template = "Jass.Common["GetTransportUnit"]()"
            public static extern float GetTransportUnit();

            /// @CSharpLua.Template = "Jass.Common["GetTriggerDestructable"]()"
            public static extern float GetTriggerDestructable();

            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["GetTriggerEvalCount"]({0})"
            public static extern float GetTriggerEvalCount(int whichTrigger);

            /// @CSharpLua.Template = "Jass.Common["GetTriggerEventId"]()"
            public static extern float GetTriggerEventId();

            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["GetTriggerExecCount"]({0})"
            public static extern float GetTriggerExecCount(int whichTrigger);

            /// @CSharpLua.Template = "Jass.Common["GetTriggerPlayer"]()"
            public static extern float GetTriggerPlayer();

            /// @CSharpLua.Template = "Jass.Common["GetTriggerUnit"]()"
            public static extern float GetTriggerUnit();

            /// @CSharpLua.Template = "Jass.Common["GetTriggerWidget"]()"
            public static extern float GetTriggerWidget();

            /// @CSharpLua.Template = "Jass.Common["GetTriggeringRegion"]()"
            public static extern float GetTriggeringRegion();

            /// @CSharpLua.Template = "Jass.Common["GetTriggeringTrackable"]()"
            public static extern object GetTriggeringTrackable();

            /// @CSharpLua.Template = "Jass.Common["GetTriggeringTrigger"]()"
            public static extern float GetTriggeringTrigger();

            // whichUnit number unit
            // abilcode number int
            /// @CSharpLua.Template = "Jass.Common["GetUnitAbilityLevel"]({0}, {1})"
            public static extern float GetUnitAbilityLevel(int whichUnit, float abilcode);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitAcquireRange"]({0})"
            public static extern float GetUnitAcquireRange(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitCurrentOrder"]({0})"
            public static extern float GetUnitCurrentOrder(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitDefaultAcquireRange"]({0})"
            public static extern float GetUnitDefaultAcquireRange(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitDefaultFlyHeight"]({0})"
            public static extern float GetUnitDefaultFlyHeight(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitDefaultMoveSpeed"]({0})"
            public static extern float GetUnitDefaultMoveSpeed(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitDefaultPropWindow"]({0})"
            public static extern float GetUnitDefaultPropWindow(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitDefaultTurnSpeed"]({0})"
            public static extern float GetUnitDefaultTurnSpeed(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitFacing"]({0})"
            public static extern float GetUnitFacing(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitFlyHeight"]({0})"
            public static extern float GetUnitFlyHeight(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitFoodMade"]({0})"
            public static extern float GetUnitFoodMade(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitFoodUsed"]({0})"
            public static extern float GetUnitFoodUsed(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitLevel"]({0})"
            public static extern int GetUnitLevel(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitLoc"]({0})"
            public static extern float GetUnitLoc(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitMoveSpeed"]({0})"
            public static extern float GetUnitMoveSpeed(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitName"]({0})"
            public static extern string GetUnitName(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitPointValue"]({0})"
            public static extern float GetUnitPointValue(int whichUnit);

            // unitType number int
            /// @CSharpLua.Template = "Jass.Common["GetUnitPointValueByType"]({0})"
            public static extern float GetUnitPointValueByType(float unitType);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitPropWindow"]({0})"
            public static extern float GetUnitPropWindow(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitRace"]({0})"
            public static extern object GetUnitRace(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitRallyDestructable"]({0})"
            public static extern float GetUnitRallyDestructable(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitRallyPoint"]({0})"
            public static extern float GetUnitRallyPoint(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitRallyUnit"]({0})"
            public static extern float GetUnitRallyUnit(int whichUnit);

            // whichUnit number unit
            // whichUnitState any unitstate
            /// @CSharpLua.Template = "Jass.Common["GetUnitState"]({0}, {1})"
            public static extern float GetUnitState(int whichUnit, object whichUnitState);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitTurnSpeed"]({0})"
            public static extern float GetUnitTurnSpeed(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitTypeId"]({0})"
            public static extern int GetUnitTypeId(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitUserData"]({0})"
            public static extern float GetUnitUserData(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitX"]({0})"
            public static extern float GetUnitX(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GetUnitY"]({0})"
            public static extern float GetUnitY(int whichUnit);

            // whichWidget number widget
            /// @CSharpLua.Template = "Jass.Common["GetWidgetLife"]({0})"
            public static extern float GetWidgetLife(float whichWidget);

            // whichWidget number widget
            /// @CSharpLua.Template = "Jass.Common["GetWidgetX"]({0})"
            public static extern float GetWidgetX(float whichWidget);

            // whichWidget number widget
            /// @CSharpLua.Template = "Jass.Common["GetWidgetY"]({0})"
            public static extern float GetWidgetY(float whichWidget);

            /// @CSharpLua.Template = "Jass.Common["GetWinningPlayer"]()"
            public static extern float GetWinningPlayer();

            /// @CSharpLua.Template = "Jass.Common["GetWorldBounds"]()"
            public static extern float GetWorldBounds();

            // 添加单位 [R]
            // 为[单位组]添加[单位]
            // 并不影响单位本身.
            // whichGroup number group
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GroupAddUnit"]({0}, {1})"
            public static extern void GroupAddUnit(float whichGroup, int whichUnit);

            // 清空单位组
            // 清空[单位组]内所有单位
            // 并不影响单位本身.
            // whichGroup number group
            /// @CSharpLua.Template = "Jass.Common["GroupClear"]({0})"
            public static extern void GroupClear(float whichGroup);

            // 选取单位添加到单位组(坐标)
            // 为[单位组]添加以([坐标X],[坐标Y])为圆心，[半径]为半径的圆范围内，满足[条件]的单位
            // whichGroup number group
            // x number
            // y number
            // radius number
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsInRange"]({0}, {1}, {2}, {3}, {4})"
            public static extern void GroupEnumUnitsInRange(float whichGroup, float x, float y, float radius, object filter);

            // 选取单位添加到单位组(坐标)(不建议使用)
            // 为[单位组]添加以([坐标X],[坐标Y])为圆心，[半径]为半径的圆范围内，满足[条件]的单位。无效项([N])
            // 最后一项是无效项，建议用上一个UI
            // whichGroup number group
            // x number
            // y number
            // radius number
            // filter function boolexpr
            // countLimit number int
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsInRangeCounted"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern void GroupEnumUnitsInRangeCounted(float whichGroup, float x, float y, float radius, object filter, float countLimit);

            // 选取单位添加到单位组(点)
            // 为[单位组]添加以[坐标]为圆心，[半径]为半径的圆范围内，满足[条件]的单位
            // whichGroup number group
            // whichLocation number location
            // radius number
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsInRangeOfLoc"]({0}, {1}, {2}, {3})"
            public static extern void GroupEnumUnitsInRangeOfLoc(float whichGroup, float whichLocation, float radius, object filter);

            // 选取单位添加到单位组(点)(不建议使用)
            // 为[单位组]添加以[坐标]为圆心，[半径]为半径的圆范围内，满足[条件]的单位。无效项([N])
            // 最后一项是无效项，建议用上一个UI
            // whichGroup number group
            // whichLocation number location
            // radius number
            // filter function boolexpr
            // countLimit number int
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsInRangeOfLocCounted"]({0}, {1}, {2}, {3}, {4})"
            public static extern void GroupEnumUnitsInRangeOfLocCounted(float whichGroup, float whichLocation, float radius, object filter, float countLimit);

            // whichGroup number group
            // r number rect
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsInRect"]({0}, {1}, {2})"
            public static extern void GroupEnumUnitsInRect(float whichGroup, float r, object filter);

            // whichGroup number group
            // r number rect
            // filter function boolexpr
            // countLimit number int
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsInRectCounted"]({0}, {1}, {2}, {3})"
            public static extern void GroupEnumUnitsInRectCounted(float whichGroup, float r, object filter, float countLimit);

            // whichGroup number group
            // whichPlayer number player
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsOfPlayer"]({0}, {1}, {2})"
            public static extern void GroupEnumUnitsOfPlayer(float whichGroup, int whichPlayer, object filter);

            // whichGroup number group
            // unitname string
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsOfType"]({0}, {1}, {2})"
            public static extern void GroupEnumUnitsOfType(float whichGroup, string unitname, object filter);

            // whichGroup number group
            // unitname string
            // filter function boolexpr
            // countLimit number int
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsOfTypeCounted"]({0}, {1}, {2}, {3})"
            public static extern void GroupEnumUnitsOfTypeCounted(float whichGroup, string unitname, object filter, float countLimit);

            // whichGroup number group
            // whichPlayer number player
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["GroupEnumUnitsSelected"]({0}, {1}, {2})"
            public static extern void GroupEnumUnitsSelected(float whichGroup, int whichPlayer, object filter);

            // 发布命令(无目标)
            // 对[单位组]发布[Order]
            // 最多只能对单位组中12个单位发布命令.
            // whichGroup number group
            // order string
            /// @CSharpLua.Template = "Jass.Common["GroupImmediateOrder"]({0}, {1})"
            public static extern bool GroupImmediateOrder(float whichGroup, string order);

            // 发布命令(无目标)(ID)
            // 对[单位组]发布[Order]
            // 最多只能对单位组中12个单位发布命令.
            // whichGroup number group
            // order number int
            /// @CSharpLua.Template = "Jass.Common["GroupImmediateOrderById"]({0}, {1})"
            public static extern bool GroupImmediateOrderById(float whichGroup, float order);

            // 发布命令(指定坐标) [R]
            // 对[单位组]发布[Order]命令,目标点:([X],[Y])
            // 最多只能对单位组中12个单位发布命令.
            // whichGroup number group
            // order string
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["GroupPointOrder"]({0}, {1}, {2}, {3})"
            public static extern bool GroupPointOrder(float whichGroup, string order, float x, float y);

            // 发布命令(指定坐标)(ID)
            // 对[单位组]发布[Order]命令,目标点:([X],[Y])
            // 最多只能对单位组中12个单位发布命令.
            // whichGroup number group
            // order number int
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["GroupPointOrderById"]({0}, {1}, {2}, {3})"
            public static extern bool GroupPointOrderById(float whichGroup, float order, float x, float y);

            // 发布命令(指定点)(ID)
            // 对[单位组]发布[Order]命令,目标:[指定点]
            // 最多只能对单位组中12个单位发布命令.
            // whichGroup number group
            // order number int
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["GroupPointOrderByIdLoc"]({0}, {1}, {2})"
            public static extern bool GroupPointOrderByIdLoc(float whichGroup, float order, float whichLocation);

            // 发布命令(指定点)
            // 对[单位组]发布[Order]命令,目标:[指定点]
            // 最多只能对单位组中12个单位发布命令.
            // whichGroup number group
            // order string
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["GroupPointOrderLoc"]({0}, {1}, {2})"
            public static extern bool GroupPointOrderLoc(float whichGroup, string order, float whichLocation);

            // 移除单位 [R]
            // 为[单位组]删除[单位]
            // 并不影响单位本身.
            // whichGroup number group
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["GroupRemoveUnit"]({0}, {1})"
            public static extern void GroupRemoveUnit(float whichGroup, int whichUnit);

            // 发布命令(指定单位)
            // 对[单位组]发布[Order]命令,目标:[单位]
            // 最多只能对单位组中12个单位发布命令.
            // whichGroup number group
            // order string
            // targetWidget number widget
            /// @CSharpLua.Template = "Jass.Common["GroupTargetOrder"]({0}, {1}, {2})"
            public static extern bool GroupTargetOrder(float whichGroup, string order, float targetWidget);

            // 发布命令(指定单位)(ID)
            // 对[单位组]发布[Order]命令,目标:[单位]
            // 最多只能对单位组中12个单位发布命令.
            // whichGroup number group
            // order number int
            // targetWidget number widget
            /// @CSharpLua.Template = "Jass.Common["GroupTargetOrderById"]({0}, {1}, {2})"
            public static extern bool GroupTargetOrderById(float whichGroup, float order, float targetWidget);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["HaveSavedBoolean"]({0}, {1}, {2})"
            public static extern bool HaveSavedBoolean(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["HaveSavedHandle"]({0}, {1}, {2})"
            public static extern bool HaveSavedHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["HaveSavedInteger"]({0}, {1}, {2})"
            public static extern bool HaveSavedInteger(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["HaveSavedReal"]({0}, {1}, {2})"
            public static extern bool HaveSavedReal(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["HaveSavedString"]({0}, {1}, {2})"
            public static extern bool HaveSavedString(object table, float parentKey, float childKey);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["HaveStoredBoolean"]({0}, {1}, {2})"
            public static extern bool HaveStoredBoolean(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["HaveStoredInteger"]({0}, {1}, {2})"
            public static extern bool HaveStoredInteger(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["HaveStoredReal"]({0}, {1}, {2})"
            public static extern bool HaveStoredReal(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["HaveStoredString"]({0}, {1}, {2})"
            public static extern bool HaveStoredString(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["HaveStoredUnit"]({0}, {1}, {2})"
            public static extern bool HaveStoredUnit(object cache, string missionKey, string key);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["I2R"]({0})"
            public static extern float I2R(float i);

            // i number int
            /// @CSharpLua.Template = "Jass.Common["I2S"]({0})"
            public static extern string I2S(float i);

            // 提升技能等级 [R]
            // 使[单位]的[技能]等级提升1级
            // 改变死亡单位的光环技能会导致魔兽崩溃.
            // whichUnit number unit
            // abilcode number int
            /// @CSharpLua.Template = "Jass.Common["IncUnitAbilityLevel"]({0}, {1})"
            public static extern float IncUnitAbilityLevel(int whichUnit, float abilcode);

            // 创建游戏缓存
            // 创建游戏缓存,使用文件名:[Filename]
            // campaignFile string
            /// @CSharpLua.Template = "Jass.Common["InitGameCache"]({0})"
            public static extern object InitGameCache(string campaignFile);

            // @1.24 新建哈希表 [C]
            // 创建一个新的哈希表
            // 您可以使用哈希表来储存临时数据
            /// @CSharpLua.Template = "Jass.Common["InitHashtable"]()"
            public static extern object InitHashtable();

            /// @CSharpLua.Template = "Jass.Common["IsCineFilterDisplayed"]()"
            public static extern bool IsCineFilterDisplayed();

            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["IsDestructableInvulnerable"]({0})"
            public static extern bool IsDestructableInvulnerable(float d);

            /// @CSharpLua.Template = "Jass.Common["IsFogEnabled"]()"
            public static extern bool IsFogEnabled();

            /// @CSharpLua.Template = "Jass.Common["IsFogMaskEnabled"]()"
            public static extern bool IsFogMaskEnabled();

            // x number
            // y number
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsFoggedToPlayer"]({0}, {1}, {2})"
            public static extern bool IsFoggedToPlayer(float x, float y, int whichPlayer);

            // whichGameType any gametype
            /// @CSharpLua.Template = "Jass.Common["IsGameTypeSupported"]({0})"
            public static extern bool IsGameTypeSupported(object whichGameType);

            // unitId number int
            /// @CSharpLua.Template = "Jass.Common["IsHeroUnitId"]({0})"
            public static extern bool IsHeroUnitId(int unitId);

            // itemId number int
            /// @CSharpLua.Template = "Jass.Common["IsItemIdPawnable"]({0})"
            public static extern bool IsItemIdPawnable(float itemId);

            // itemId number int
            /// @CSharpLua.Template = "Jass.Common["IsItemIdPowerup"]({0})"
            public static extern bool IsItemIdPowerup(float itemId);

            // itemId number int
            /// @CSharpLua.Template = "Jass.Common["IsItemIdSellable"]({0})"
            public static extern bool IsItemIdSellable(float itemId);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["IsItemInvulnerable"]({0})"
            public static extern bool IsItemInvulnerable(float whichItem);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["IsItemOwned"]({0})"
            public static extern bool IsItemOwned(float whichItem);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["IsItemPawnable"]({0})"
            public static extern bool IsItemPawnable(float whichItem);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["IsItemPowerup"]({0})"
            public static extern bool IsItemPowerup(float whichItem);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["IsItemSellable"]({0})"
            public static extern bool IsItemSellable(float whichItem);

            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["IsItemVisible"]({0})"
            public static extern bool IsItemVisible(float whichItem);

            // lb number leaderboard
            /// @CSharpLua.Template = "Jass.Common["IsLeaderboardDisplayed"]({0})"
            public static extern bool IsLeaderboardDisplayed(float lb);

            // whichLocation number location
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsLocationFoggedToPlayer"]({0}, {1})"
            public static extern bool IsLocationFoggedToPlayer(float whichLocation, int whichPlayer);

            // whichRegion number region
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["IsLocationInRegion"]({0}, {1})"
            public static extern bool IsLocationInRegion(float whichRegion, float whichLocation);

            // whichLocation number location
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsLocationMaskedToPlayer"]({0}, {1})"
            public static extern bool IsLocationMaskedToPlayer(float whichLocation, int whichPlayer);

            // whichLocation number location
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsLocationVisibleToPlayer"]({0}, {1})"
            public static extern bool IsLocationVisibleToPlayer(float whichLocation, int whichPlayer);

            // whichMapFlag any mapflag
            /// @CSharpLua.Template = "Jass.Common["IsMapFlagSet"]({0})"
            public static extern bool IsMapFlagSet(object whichMapFlag);

            // x number
            // y number
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsMaskedToPlayer"]({0}, {1}, {2})"
            public static extern bool IsMaskedToPlayer(float x, float y, int whichPlayer);

            // lb number multiboard
            /// @CSharpLua.Template = "Jass.Common["IsMultiboardDisplayed"]({0})"
            public static extern bool IsMultiboardDisplayed(float lb);

            // lb number multiboard
            /// @CSharpLua.Template = "Jass.Common["IsMultiboardMinimized"]({0})"
            public static extern bool IsMultiboardMinimized(float lb);

            /// @CSharpLua.Template = "Jass.Common["IsNoDefeatCheat"]()"
            public static extern bool IsNoDefeatCheat();

            /// @CSharpLua.Template = "Jass.Common["IsNoVictoryCheat"]()"
            public static extern bool IsNoVictoryCheat();

            // whichPlayer number player
            // otherPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsPlayerAlly"]({0}, {1})"
            public static extern bool IsPlayerAlly(int whichPlayer, float otherPlayer);

            // whichPlayer number player
            // otherPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsPlayerEnemy"]({0}, {1})"
            public static extern bool IsPlayerEnemy(int whichPlayer, float otherPlayer);

            // whichPlayer number player
            // whichForce number force
            /// @CSharpLua.Template = "Jass.Common["IsPlayerInForce"]({0}, {1})"
            public static extern bool IsPlayerInForce(int whichPlayer, int whichForce);

            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsPlayerObserver"]({0})"
            public static extern bool IsPlayerObserver(int whichPlayer);

            // whichPlayer number player
            // pref any racepreference
            /// @CSharpLua.Template = "Jass.Common["IsPlayerRacePrefSet"]({0}, {1})"
            public static extern bool IsPlayerRacePrefSet(int whichPlayer, object pref);

            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["IsPointBlighted"]({0}, {1})"
            public static extern bool IsPointBlighted(float x, float y);

            // whichRegion number region
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["IsPointInRegion"]({0}, {1}, {2})"
            public static extern bool IsPointInRegion(float whichRegion, float x, float y);

            // whichQuest number quest
            /// @CSharpLua.Template = "Jass.Common["IsQuestCompleted"]({0})"
            public static extern bool IsQuestCompleted(float whichQuest);

            // whichQuest number quest
            /// @CSharpLua.Template = "Jass.Common["IsQuestDiscovered"]({0})"
            public static extern bool IsQuestDiscovered(float whichQuest);

            // whichQuest number quest
            /// @CSharpLua.Template = "Jass.Common["IsQuestEnabled"]({0})"
            public static extern bool IsQuestEnabled(float whichQuest);

            // whichQuest number quest
            /// @CSharpLua.Template = "Jass.Common["IsQuestFailed"]({0})"
            public static extern bool IsQuestFailed(float whichQuest);

            // whichQuestItem number questitem
            /// @CSharpLua.Template = "Jass.Common["IsQuestItemCompleted"]({0})"
            public static extern bool IsQuestItemCompleted(float whichQuestItem);

            // whichQuest number quest
            /// @CSharpLua.Template = "Jass.Common["IsQuestRequired"]({0})"
            public static extern bool IsQuestRequired(float whichQuest);

            // whichHero number unit
            /// @CSharpLua.Template = "Jass.Common["IsSuspendedXP"]({0})"
            public static extern bool IsSuspendedXP(float whichHero);

            // x number
            // y number
            // t any pathingtype
            /// @CSharpLua.Template = "Jass.Common["IsTerrainPathable"]({0}, {1}, {2})"
            public static extern bool IsTerrainPathable(float x, float y, object t);

            // whichDialog number timerdialog
            /// @CSharpLua.Template = "Jass.Common["IsTimerDialogDisplayed"]({0})"
            public static extern bool IsTimerDialogDisplayed(int whichDialog);

            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["IsTriggerEnabled"]({0})"
            public static extern bool IsTriggerEnabled(int whichTrigger);

            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["IsTriggerWaitOnSleeps"]({0})"
            public static extern bool IsTriggerWaitOnSleeps(int whichTrigger);

            // whichUnit number unit
            // whichSpecifiedUnit number unit
            /// @CSharpLua.Template = "Jass.Common["IsUnit"]({0}, {1})"
            public static extern bool IsUnit(int whichUnit, float whichSpecifiedUnit);

            // whichUnit number unit
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsUnitAlly"]({0}, {1})"
            public static extern bool IsUnitAlly(int whichUnit, int whichPlayer);

            // whichUnit number unit
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsUnitDetected"]({0}, {1})"
            public static extern bool IsUnitDetected(int whichUnit, int whichPlayer);

            // whichUnit number unit
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsUnitEnemy"]({0}, {1})"
            public static extern bool IsUnitEnemy(int whichUnit, int whichPlayer);

            // whichUnit number unit
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsUnitFogged"]({0}, {1})"
            public static extern bool IsUnitFogged(int whichUnit, int whichPlayer);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["IsUnitHidden"]({0})"
            public static extern bool IsUnitHidden(int whichUnit);

            // unitId number int
            // whichUnitType any unittype
            /// @CSharpLua.Template = "Jass.Common["IsUnitIdType"]({0}, {1})"
            public static extern bool IsUnitIdType(int unitId, object whichUnitType);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["IsUnitIllusion"]({0})"
            public static extern bool IsUnitIllusion(int whichUnit);

            // whichUnit number unit
            // whichForce number force
            /// @CSharpLua.Template = "Jass.Common["IsUnitInForce"]({0}, {1})"
            public static extern bool IsUnitInForce(int whichUnit, int whichForce);

            // whichUnit number unit
            // whichGroup number group
            /// @CSharpLua.Template = "Jass.Common["IsUnitInGroup"]({0}, {1})"
            public static extern bool IsUnitInGroup(int whichUnit, float whichGroup);

            // whichUnit number unit
            // otherUnit number unit
            // distance number
            /// @CSharpLua.Template = "Jass.Common["IsUnitInRange"]({0}, {1}, {2})"
            public static extern bool IsUnitInRange(int whichUnit, float otherUnit, float distance);

            // whichUnit number unit
            // whichLocation number location
            // distance number
            /// @CSharpLua.Template = "Jass.Common["IsUnitInRangeLoc"]({0}, {1}, {2})"
            public static extern bool IsUnitInRangeLoc(int whichUnit, float whichLocation, float distance);

            // whichUnit number unit
            // x number
            // y number
            // distance number
            /// @CSharpLua.Template = "Jass.Common["IsUnitInRangeXY"]({0}, {1}, {2}, {3})"
            public static extern bool IsUnitInRangeXY(int whichUnit, float x, float y, float distance);

            // whichRegion number region
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["IsUnitInRegion"]({0}, {1})"
            public static extern bool IsUnitInRegion(float whichRegion, int whichUnit);

            // whichUnit number unit
            // whichTransport number unit
            /// @CSharpLua.Template = "Jass.Common["IsUnitInTransport"]({0}, {1})"
            public static extern bool IsUnitInTransport(int whichUnit, float whichTransport);

            // whichUnit number unit
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsUnitInvisible"]({0}, {1})"
            public static extern bool IsUnitInvisible(int whichUnit, int whichPlayer);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["IsUnitLoaded"]({0})"
            public static extern bool IsUnitLoaded(int whichUnit);

            // whichUnit number unit
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsUnitMasked"]({0}, {1})"
            public static extern bool IsUnitMasked(int whichUnit, int whichPlayer);

            // whichUnit number unit
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsUnitOwnedByPlayer"]({0}, {1})"
            public static extern bool IsUnitOwnedByPlayer(int whichUnit, int whichPlayer);

            // whichHero number unit
            /// @CSharpLua.Template = "Jass.Common["IsUnitPaused"]({0})"
            public static extern bool IsUnitPaused(float whichHero);

            // whichUnit number unit
            // whichRace any race
            /// @CSharpLua.Template = "Jass.Common["IsUnitRace"]({0}, {1})"
            public static extern bool IsUnitRace(int whichUnit, object whichRace);

            // whichUnit number unit
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsUnitSelected"]({0}, {1})"
            public static extern bool IsUnitSelected(int whichUnit, int whichPlayer);

            // whichUnit number unit
            // whichUnitType any unittype
            /// @CSharpLua.Template = "Jass.Common["IsUnitType"]({0}, {1})"
            public static extern bool IsUnitType(int whichUnit, object whichUnitType);

            // whichUnit number unit
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsUnitVisible"]({0}, {1})"
            public static extern bool IsUnitVisible(int whichUnit, int whichPlayer);

            // x number
            // y number
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["IsVisibleToPlayer"]({0}, {1}, {2})"
            public static extern bool IsVisibleToPlayer(float x, float y, int whichPlayer);

            // whichPeon number unit
            // unitToBuild string
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["IssueBuildOrder"]({0}, {1}, {2}, {3})"
            public static extern bool IssueBuildOrder(float whichPeon, string unitToBuild, float x, float y);

            // 发布建造命令(指定坐标) [R]
            // 命令[单位]建造[单位类型]在坐标:([X],[Y])
            // whichPeon number unit
            // unitId number int
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["IssueBuildOrderById"]({0}, {1}, {2}, {3})"
            public static extern bool IssueBuildOrderById(float whichPeon, int unitId, float x, float y);

            // 发布命令(无目标)
            // 对[单位]发布[Order]命令
            // whichUnit number unit
            // order string
            /// @CSharpLua.Template = "Jass.Common["IssueImmediateOrder"]({0}, {1})"
            public static extern bool IssueImmediateOrder(int whichUnit, string order);

            // 发布命令(无目标)(ID)
            // 对[单位]发布[Order]命令
            // whichUnit number unit
            // order number int
            /// @CSharpLua.Template = "Jass.Common["IssueImmediateOrderById"]({0}, {1})"
            public static extern bool IssueImmediateOrderById(int whichUnit, float order);

            // whichUnit number unit
            // order string
            // x number
            // y number
            // instantTargetWidget number widget
            /// @CSharpLua.Template = "Jass.Common["IssueInstantPointOrder"]({0}, {1}, {2}, {3}, {4})"
            public static extern bool IssueInstantPointOrder(int whichUnit, string order, float x, float y, float instantTargetWidget);

            // whichUnit number unit
            // order number int
            // x number
            // y number
            // instantTargetWidget number widget
            /// @CSharpLua.Template = "Jass.Common["IssueInstantPointOrderById"]({0}, {1}, {2}, {3}, {4})"
            public static extern bool IssueInstantPointOrderById(int whichUnit, float order, float x, float y, float instantTargetWidget);

            // whichUnit number unit
            // order string
            // targetWidget number widget
            // instantTargetWidget number widget
            /// @CSharpLua.Template = "Jass.Common["IssueInstantTargetOrder"]({0}, {1}, {2}, {3})"
            public static extern bool IssueInstantTargetOrder(int whichUnit, string order, float targetWidget, float instantTargetWidget);

            // whichUnit number unit
            // order number int
            // targetWidget number widget
            // instantTargetWidget number widget
            /// @CSharpLua.Template = "Jass.Common["IssueInstantTargetOrderById"]({0}, {1}, {2}, {3})"
            public static extern bool IssueInstantTargetOrderById(int whichUnit, float order, float targetWidget, float instantTargetWidget);

            // 发布中介命令(无目标)
            // 使[玩家]对[单位]发布[Order]命令
            // 可以用来对非本玩家单位发布命令.
            // forWhichPlayer number player
            // neutralStructure number unit
            // unitToBuild string
            /// @CSharpLua.Template = "Jass.Common["IssueNeutralImmediateOrder"]({0}, {1}, {2})"
            public static extern bool IssueNeutralImmediateOrder(float forWhichPlayer, float neutralStructure, string unitToBuild);

            // 发布中介命令(无目标)(ID)
            // 使[玩家]对[单位]发布[Order]命令
            // 可以用来对非本玩家单位发布命令.
            // forWhichPlayer number player
            // neutralStructure number unit
            // unitId number int
            /// @CSharpLua.Template = "Jass.Common["IssueNeutralImmediateOrderById"]({0}, {1}, {2})"
            public static extern bool IssueNeutralImmediateOrderById(float forWhichPlayer, float neutralStructure, int unitId);

            // 发布中介命令(指定坐标)
            // 使[玩家]对[单位]发布[Order]命令到坐标:([X],[Y])
            // 可以用来对非本玩家单位发布命令.
            // forWhichPlayer number player
            // neutralStructure number unit
            // unitToBuild string
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["IssueNeutralPointOrder"]({0}, {1}, {2}, {3}, {4})"
            public static extern bool IssueNeutralPointOrder(float forWhichPlayer, float neutralStructure, string unitToBuild, float x, float y);

            // 发布中介命令(指定坐标)(ID)
            // 使[玩家]对[单位]发布[Order]命令到坐标:([X],[Y])
            // 可以用来对非本玩家单位发布命令.
            // forWhichPlayer number player
            // neutralStructure number unit
            // unitId number int
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["IssueNeutralPointOrderById"]({0}, {1}, {2}, {3}, {4})"
            public static extern bool IssueNeutralPointOrderById(float forWhichPlayer, float neutralStructure, int unitId, float x, float y);

            // 发布中介命令(指定单位)
            // 使[玩家]对[单位]发布[Order]命令到目标:[单位]
            // 可以用来对非本玩家单位发布命令.
            // forWhichPlayer number player
            // neutralStructure number unit
            // unitToBuild string
            // target number widget
            /// @CSharpLua.Template = "Jass.Common["IssueNeutralTargetOrder"]({0}, {1}, {2}, {3})"
            public static extern bool IssueNeutralTargetOrder(float forWhichPlayer, float neutralStructure, string unitToBuild, float target);

            // 发布中介命令(指定单位)(ID)
            // 使[玩家]对[单位]发布[Order]命令到目标:[单位]
            // 可以用来对非本玩家单位发布命令.
            // forWhichPlayer number player
            // neutralStructure number unit
            // unitId number int
            // target number widget
            /// @CSharpLua.Template = "Jass.Common["IssueNeutralTargetOrderById"]({0}, {1}, {2}, {3})"
            public static extern bool IssueNeutralTargetOrderById(float forWhichPlayer, float neutralStructure, int unitId, float target);

            // 发布命令(指定坐标)
            // 对[单位]发布[Order]命令到坐标:([X],[Y])
            // whichUnit number unit
            // order string
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["IssuePointOrder"]({0}, {1}, {2}, {3})"
            public static extern bool IssuePointOrder(int whichUnit, string order, float x, float y);

            // 发布命令(指定坐标)(ID)
            // 对[单位]发布[Order]命令到坐标:([X],[Y])
            // whichUnit number unit
            // order number int
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["IssuePointOrderById"]({0}, {1}, {2}, {3})"
            public static extern bool IssuePointOrderById(int whichUnit, float order, float x, float y);

            // 发布命令(指定点)(ID)
            // 对[单位]发布[Order]命令到目标点:[指定点]
            // whichUnit number unit
            // order number int
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["IssuePointOrderByIdLoc"]({0}, {1}, {2})"
            public static extern bool IssuePointOrderByIdLoc(int whichUnit, float order, float whichLocation);

            // 发布命令(指定点)
            // 对[单位]发布[Order]命令到目标点:[指定点]
            // whichUnit number unit
            // order string
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["IssuePointOrderLoc"]({0}, {1}, {2})"
            public static extern bool IssuePointOrderLoc(int whichUnit, string order, float whichLocation);

            // 发布命令(指定单位)
            // 对[单位]发布[Order]命令到目标:[单位]
            // whichUnit number unit
            // order string
            // targetWidget number widget
            /// @CSharpLua.Template = "Jass.Common["IssueTargetOrder"]({0}, {1}, {2})"
            public static extern bool IssueTargetOrder(int whichUnit, string order, float targetWidget);

            // 发布命令(指定单位)(ID)
            // 对[单位]发布[Order]命令到目标:[单位]
            // whichUnit number unit
            // order number int
            // targetWidget number widget
            /// @CSharpLua.Template = "Jass.Common["IssueTargetOrderById"]({0}, {1}, {2})"
            public static extern bool IssueTargetOrderById(int whichUnit, float order, float targetWidget);

            // 添加物品类型 [R]
            // 在[物品池]中添加一个[物品]比重为[数值]
            // 比重越高被选择的机率越大.
            // whichItemPool number itempool
            // itemId number int
            // weight number
            /// @CSharpLua.Template = "Jass.Common["ItemPoolAddItemType"]({0}, {1}, {2})"
            public static extern void ItemPoolAddItemType(float whichItemPool, float itemId, float weight);

            // 删除物品类型 [R]
            // 从[物品池]中删除[物品]
            // whichItemPool number itempool
            // itemId number int
            /// @CSharpLua.Template = "Jass.Common["ItemPoolRemoveItemType"]({0}, {1})"
            public static extern void ItemPoolRemoveItemType(float whichItemPool, float itemId);

            // 杀死
            // 杀死[可破坏物]
            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["KillDestructable"]({0})"
            public static extern void KillDestructable(float d);

            // 删除音效
            // 删除[音效]
            // 如果音效正在播放则在播放结束时删除.
            // soundHandle number sound
            /// @CSharpLua.Template = "Jass.Common["KillSoundWhenDone"]({0})"
            public static extern void KillSoundWhenDone(float soundHandle);

            // 杀死
            // 杀死[单位]
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["KillUnit"]({0})"
            public static extern void KillUnit(int whichUnit);

            // 添加玩家
            // 添加[某个玩家]到[某个排行榜],使用名字:[文字]设置分数:[Value]
            // lb number leaderboard
            // label string
            // value number int
            // p number player
            /// @CSharpLua.Template = "Jass.Common["LeaderboardAddItem"]({0}, {1}, {2}, {3})"
            public static extern void LeaderboardAddItem(float lb, string label, float value, float p);

            // 清空 [R]
            // 清空[排行榜]
            // 清空排行榜中所有内容.
            // lb number leaderboard
            /// @CSharpLua.Template = "Jass.Common["LeaderboardClear"]({0})"
            public static extern void LeaderboardClear(float lb);

            // 显示/隐藏 [R]
            // 设置[排行榜][显示/隐藏]
            // 排行榜不能在地图初始化时显示.
            // lb number leaderboard
            // show boolean
            /// @CSharpLua.Template = "Jass.Common["LeaderboardDisplay"]({0}, {1})"
            public static extern void LeaderboardDisplay(float lb, bool show);

            // lb number leaderboard
            /// @CSharpLua.Template = "Jass.Common["LeaderboardGetItemCount"]({0})"
            public static extern float LeaderboardGetItemCount(float lb);

            // lb number leaderboard
            /// @CSharpLua.Template = "Jass.Common["LeaderboardGetLabelText"]({0})"
            public static extern string LeaderboardGetLabelText(float lb);

            // lb number leaderboard
            // p number player
            /// @CSharpLua.Template = "Jass.Common["LeaderboardGetPlayerIndex"]({0}, {1})"
            public static extern float LeaderboardGetPlayerIndex(float lb, float p);

            // lb number leaderboard
            // p number player
            /// @CSharpLua.Template = "Jass.Common["LeaderboardHasPlayerItem"]({0}, {1})"
            public static extern bool LeaderboardHasPlayerItem(float lb, float p);

            // lb number leaderboard
            // index number int
            /// @CSharpLua.Template = "Jass.Common["LeaderboardRemoveItem"]({0}, {1})"
            public static extern void LeaderboardRemoveItem(float lb, float index);

            // 移除玩家
            // 把[某个玩家]从[某个排行榜]移除
            // lb number leaderboard
            // p number player
            /// @CSharpLua.Template = "Jass.Common["LeaderboardRemovePlayerItem"]({0}, {1})"
            public static extern void LeaderboardRemovePlayerItem(float lb, float p);

            // lb number leaderboard
            // whichItem number int
            // val string
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetItemLabel"]({0}, {1}, {2})"
            public static extern void LeaderboardSetItemLabel(float lb, float whichItem, string val);

            // lb number leaderboard
            // whichItem number int
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetItemLabelColor"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern void LeaderboardSetItemLabelColor(float lb, float whichItem, float red, float green, float blue, float alpha);

            // lb number leaderboard
            // whichItem number int
            // showLabel boolean
            // showValue boolean
            // showIcon boolean
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetItemStyle"]({0}, {1}, {2}, {3}, {4})"
            public static extern void LeaderboardSetItemStyle(float lb, float whichItem, bool showLabel, bool showValue, bool showIcon);

            // lb number leaderboard
            // whichItem number int
            // val number int
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetItemValue"]({0}, {1}, {2})"
            public static extern void LeaderboardSetItemValue(float lb, float whichItem, float val);

            // lb number leaderboard
            // whichItem number int
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetItemValueColor"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern void LeaderboardSetItemValueColor(float lb, float whichItem, float red, float green, float blue, float alpha);

            // 设置标题
            // 设置[某个排行榜]的标题为[文字]
            // lb number leaderboard
            // label string
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetLabel"]({0}, {1})"
            public static extern void LeaderboardSetLabel(float lb, string label);

            // 设置文字颜色 [R]
            // 设置[某个排行榜]的文字颜色为([Red],[Green],[Blue]) Alpha通道值为:[Alpha]
            // 颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和Alpha通道值取值范围为0-255.
            // lb number leaderboard
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetLabelColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void LeaderboardSetLabelColor(float lb, float red, float green, float blue, float alpha);

            // lb number leaderboard
            // count number int
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetSizeByItemCount"]({0}, {1})"
            public static extern void LeaderboardSetSizeByItemCount(float lb, float count);

            // 设置显示样式
            // 设置[某个排行榜]的显示样式:[显示/隐藏]标题,[显示/隐藏]文字,[显示/隐藏]分数,[显示/隐藏]图标
            // lb number leaderboard
            // showLabel boolean
            // showNames boolean
            // showValues boolean
            // showIcons boolean
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetStyle"]({0}, {1}, {2}, {3}, {4})"
            public static extern void LeaderboardSetStyle(float lb, bool showLabel, bool showNames, bool showValues, bool showIcons);

            // 设置数值颜色 [R]
            // 设置[某个排行榜]的数值颜色为([Red],[Green],[Blue]) Alpha通道值为:[Alpha]
            // 颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和Alpha通道值取值范围为0-255.
            // lb number leaderboard
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSetValueColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void LeaderboardSetValueColor(float lb, float red, float green, float blue, float alpha);

            // lb number leaderboard
            // ascending boolean
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSortItemsByLabel"]({0}, {1})"
            public static extern void LeaderboardSortItemsByLabel(float lb, bool ascending);

            // lb number leaderboard
            // ascending boolean
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSortItemsByPlayer"]({0}, {1})"
            public static extern void LeaderboardSortItemsByPlayer(float lb, bool ascending);

            // lb number leaderboard
            // ascending boolean
            /// @CSharpLua.Template = "Jass.Common["LeaderboardSortItemsByValue"]({0}, {1})"
            public static extern void LeaderboardSortItemsByValue(float lb, bool ascending);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadAbilityHandle"]({0}, {1}, {2})"
            public static extern float LoadAbilityHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadBoolean"]({0}, {1}, {2})"
            public static extern bool LoadBoolean(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadBooleanExprHandle"]({0}, {1}, {2})"
            public static extern object LoadBooleanExprHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadButtonHandle"]({0}, {1}, {2})"
            public static extern float LoadButtonHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadDefeatConditionHandle"]({0}, {1}, {2})"
            public static extern object LoadDefeatConditionHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadDestructableHandle"]({0}, {1}, {2})"
            public static extern float LoadDestructableHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadDialogHandle"]({0}, {1}, {2})"
            public static extern float LoadDialogHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadEffectHandle"]({0}, {1}, {2})"
            public static extern float LoadEffectHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadFogModifierHandle"]({0}, {1}, {2})"
            public static extern float LoadFogModifierHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadFogStateHandle"]({0}, {1}, {2})"
            public static extern object LoadFogStateHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadForceHandle"]({0}, {1}, {2})"
            public static extern float LoadForceHandle(object table, float parentKey, float childKey);

            // 读取进度
            // 读取游戏进度[Filename]([Show/Skip]计分屏)
            // saveFileName string
            // doScoreScreen boolean
            /// @CSharpLua.Template = "Jass.Common["LoadGame"]({0}, {1})"
            public static extern void LoadGame(string saveFileName, bool doScoreScreen);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadGroupHandle"]({0}, {1}, {2})"
            public static extern float LoadGroupHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadHashtableHandle"]({0}, {1}, {2})"
            public static extern object LoadHashtableHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadImageHandle"]({0}, {1}, {2})"
            public static extern float LoadImageHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadInteger"]({0}, {1}, {2})"
            public static extern float LoadInteger(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadItemHandle"]({0}, {1}, {2})"
            public static extern float LoadItemHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadItemPoolHandle"]({0}, {1}, {2})"
            public static extern float LoadItemPoolHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadLeaderboardHandle"]({0}, {1}, {2})"
            public static extern float LoadLeaderboardHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadLightningHandle"]({0}, {1}, {2})"
            public static extern float LoadLightningHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadLocationHandle"]({0}, {1}, {2})"
            public static extern float LoadLocationHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadMultiboardHandle"]({0}, {1}, {2})"
            public static extern float LoadMultiboardHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadMultiboardItemHandle"]({0}, {1}, {2})"
            public static extern float LoadMultiboardItemHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadPlayerHandle"]({0}, {1}, {2})"
            public static extern float LoadPlayerHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadQuestHandle"]({0}, {1}, {2})"
            public static extern float LoadQuestHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadQuestItemHandle"]({0}, {1}, {2})"
            public static extern float LoadQuestItemHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadReal"]({0}, {1}, {2})"
            public static extern float LoadReal(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadRectHandle"]({0}, {1}, {2})"
            public static extern float LoadRectHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadRegionHandle"]({0}, {1}, {2})"
            public static extern float LoadRegionHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadSoundHandle"]({0}, {1}, {2})"
            public static extern float LoadSoundHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadStr"]({0}, {1}, {2})"
            public static extern string LoadStr(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadTextTagHandle"]({0}, {1}, {2})"
            public static extern float LoadTextTagHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadTimerDialogHandle"]({0}, {1}, {2})"
            public static extern float LoadTimerDialogHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadTimerHandle"]({0}, {1}, {2})"
            public static extern float LoadTimerHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadTrackableHandle"]({0}, {1}, {2})"
            public static extern object LoadTrackableHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadTriggerActionHandle"]({0}, {1}, {2})"
            public static extern object LoadTriggerActionHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadTriggerConditionHandle"]({0}, {1}, {2})"
            public static extern object LoadTriggerConditionHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadTriggerEventHandle"]({0}, {1}, {2})"
            public static extern object LoadTriggerEventHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadTriggerHandle"]({0}, {1}, {2})"
            public static extern float LoadTriggerHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadUbersplatHandle"]({0}, {1}, {2})"
            public static extern object LoadUbersplatHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadUnitHandle"]({0}, {1}, {2})"
            public static extern float LoadUnitHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadUnitPoolHandle"]({0}, {1}, {2})"
            public static extern float LoadUnitPoolHandle(object table, float parentKey, float childKey);

            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["LoadWidgetHandle"]({0}, {1}, {2})"
            public static extern float LoadWidgetHandle(object table, float parentKey, float childKey);

            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["Location"]({0}, {1})"
            public static extern float Location(float x, float y);

            // 移动闪电效果(指定点)
            // 移动[某个闪电效果],使其连接[坐标]到[坐标]
            // whichBolt number lightning
            // checkVisibility boolean
            // x1 number
            // y1 number
            // x2 number
            // y2 number
            /// @CSharpLua.Template = "Jass.Common["MoveLightning"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern bool MoveLightning(float whichBolt, bool checkVisibility, float x1, float y1, float x2, float y2);

            // 移动闪电效果(指定坐标) [R]
            // 移动[某个闪电效果]到新位置,([Boolean]检查可见性) 新起始点: ([X],[Y],[Z]) 新终结点: ([X],[Y],[Z])
            // 可指定Z坐标. 允许检查可见性则在指定起始点和终结点都不可见时将不移动闪电效果.
            // whichBolt number lightning
            // checkVisibility boolean
            // x1 number
            // y1 number
            // z1 number
            // x2 number
            // y2 number
            // z2 number
            /// @CSharpLua.Template = "Jass.Common["MoveLightningEx"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
            public static extern bool MoveLightningEx(float whichBolt, bool checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2);

            // 移动点 [R]
            // 移动[坐标]到([X],[Y])
            // 该点必须是一个变量. 因为移动一个不可重用的点是无意义的.
            // whichLocation number location
            // newX number
            // newY number
            /// @CSharpLua.Template = "Jass.Common["MoveLocation"]({0}, {1}, {2})"
            public static extern void MoveLocation(float whichLocation, float newX, float newY);

            // 移动矩形区域(指定坐标) [R]
            // 移动[矩形区域]到([X],[Y])
            // 该区域必须是一个变量. 目标点将作为该区域的新中心点.
            // whichRect number rect
            // newCenterX number
            // newCenterY number
            /// @CSharpLua.Template = "Jass.Common["MoveRectTo"]({0}, {1}, {2})"
            public static extern void MoveRectTo(float whichRect, float newCenterX, float newCenterY);

            // 移动矩形区域(指定点)
            // 移动[矩形区域]到[目标点]
            // 该区域必须是一个变量. 目标点将作为该区域的新中心点.
            // whichRect number rect
            // newCenterLoc number location
            /// @CSharpLua.Template = "Jass.Common["MoveRectToLoc"]({0}, {1})"
            public static extern void MoveRectToLoc(float whichRect, float newCenterLoc);

            // 清空多面板
            // 清空[某个多面板]
            // 清空该多面板中的所有行和列.
            // lb number multiboard
            /// @CSharpLua.Template = "Jass.Common["MultiboardClear"]({0})"
            public static extern void MultiboardClear(float lb);

            // 显示/隐藏 [R]
            // 设置[某个多面板][显示/隐藏]
            // 多面板不能在地图初始化时显示.
            // lb number multiboard
            // show boolean
            /// @CSharpLua.Template = "Jass.Common["MultiboardDisplay"]({0}, {1})"
            public static extern void MultiboardDisplay(float lb, bool show);

            // lb number multiboard
            /// @CSharpLua.Template = "Jass.Common["MultiboardGetColumnCount"]({0})"
            public static extern float MultiboardGetColumnCount(float lb);

            // lb number multiboard
            // row number int
            // column number int
            /// @CSharpLua.Template = "Jass.Common["MultiboardGetItem"]({0}, {1}, {2})"
            public static extern float MultiboardGetItem(float lb, float row, float column);

            // lb number multiboard
            /// @CSharpLua.Template = "Jass.Common["MultiboardGetRowCount"]({0})"
            public static extern float MultiboardGetRowCount(float lb);

            // lb number multiboard
            /// @CSharpLua.Template = "Jass.Common["MultiboardGetTitleText"]({0})"
            public static extern string MultiboardGetTitleText(float lb);

            // 最大/最小化 [R]
            // 设置[某个多面板][Minimize/Maximize]
            // 最小化的多面板只显示标题.
            // lb number multiboard
            // minimize boolean
            /// @CSharpLua.Template = "Jass.Common["MultiboardMinimize"]({0}, {1})"
            public static extern void MultiboardMinimize(float lb, bool minimize);

            // 删除多面板项目 [R]
            // 删除[多面板项目]
            // 并不会影响对多面板的显示. 多面板项目指向多面板但不附属于多面板.
            // mbi number multiboarditem
            /// @CSharpLua.Template = "Jass.Common["MultiboardReleaseItem"]({0})"
            public static extern void MultiboardReleaseItem(float mbi);

            // 设置列数
            // 设置[某个多面板]的列数为[Columns]
            // lb number multiboard
            // count number int
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetColumnCount"]({0}, {1})"
            public static extern void MultiboardSetColumnCount(float lb, float count);

            // 设置指定项目图标 [R]
            // 设置[多面板项目]的项目图标为[Icon File]
            // 颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255.
            // mbi number multiboarditem
            // iconFileName string
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemIcon"]({0}, {1})"
            public static extern void MultiboardSetItemIcon(float mbi, string iconFileName);

            // 设置指定项目显示风格 [R]
            // 设置[多面板项目]的显示风格:[显示/隐藏]文字[显示/隐藏]图标
            // mbi number multiboarditem
            // showValue boolean
            // showIcon boolean
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemStyle"]({0}, {1}, {2})"
            public static extern void MultiboardSetItemStyle(float mbi, bool showValue, bool showIcon);

            // 设置指定项目文本 [R]
            // 设置[多面板项目]的项目文本为[文字]
            // mbi number multiboarditem
            // val string
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemValue"]({0}, {1})"
            public static extern void MultiboardSetItemValue(float mbi, string val);

            // 设置指定项目颜色 [R]
            // 设置[多面板项目]的项目颜色为([Red],[Green],[Blue]), Alpha值为[Alpha]
            // 颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255.
            // mbi number multiboarditem
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemValueColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void MultiboardSetItemValueColor(float mbi, float red, float green, float blue, float alpha);

            // 设置指定项目宽度 [R]
            // 设置[多面板项目]的项目宽度为[Width]倍屏幕宽度
            // 颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255.
            // mbi number multiboarditem
            // width number
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemWidth"]({0}, {1})"
            public static extern void MultiboardSetItemWidth(float mbi, float width);

            // 设置所有项目图标 [R]
            // 设置[多面板]的所有项目图标为[Icon File]
            // 可以设置行/列数为0来指代所有的行/列.
            // lb number multiboard
            // iconPath string
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemsIcon"]({0}, {1})"
            public static extern void MultiboardSetItemsIcon(float lb, string iconPath);

            // 设置所有项目显示风格 [R]
            // 设置[多面板]的所有项目显示风格:[显示/隐藏]文字[显示/隐藏]图标
            // 颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255.
            // lb number multiboard
            // showValues boolean
            // showIcons boolean
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemsStyle"]({0}, {1}, {2})"
            public static extern void MultiboardSetItemsStyle(float lb, bool showValues, bool showIcons);

            // 设置所有项目文本 [R]
            // 设置[多面板]的所有项目文本为[文字]
            // 颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255.
            // lb number multiboard
            // value string
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemsValue"]({0}, {1})"
            public static extern void MultiboardSetItemsValue(float lb, string value);

            // 设置所有项目颜色 [R]
            // 设置[多面板]的所有项目颜色为([Red],[Green],[Blue]), Alpha值为[Alpha]
            // 颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255.
            // lb number multiboard
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemsValueColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void MultiboardSetItemsValueColor(float lb, float red, float green, float blue, float alpha);

            // 设置所有项目宽度 [R]
            // 设置[多面板]的所有项目宽度为[Width]倍屏幕宽度
            // 可以设置行/列数为0来指代所有的行/列.
            // lb number multiboard
            // width number
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetItemsWidth"]({0}, {1})"
            public static extern void MultiboardSetItemsWidth(float lb, float width);

            // 设置行数
            // 设置[某个多面板]的行数为[Rows]
            // lb number multiboard
            // count number int
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetRowCount"]({0}, {1})"
            public static extern void MultiboardSetRowCount(float lb, float count);

            // 设置标题
            // 设置[某个多面板]的标题为[文字]
            // lb number multiboard
            // label string
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetTitleText"]({0}, {1})"
            public static extern void MultiboardSetTitleText(float lb, string label);

            // 设置标题颜色 [R]
            // 设置[某个多面板]的标题颜色为([Red],[Green],[Blue]), Alpha值为[Alpha]
            // 颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255.
            // lb number multiboard
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["MultiboardSetTitleTextColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void MultiboardSetTitleTextColor(float lb, float red, float green, float blue, float alpha);

            // 显示/隐藏多面板模式 [R]
            // [打开/关闭]隐藏多面板模式
            // 隐藏多面板模式将无法显示多面板.
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["MultiboardSuppressDisplay"]({0})"
            public static extern void MultiboardSuppressDisplay(bool flag);

            // environmentName string
            /// @CSharpLua.Template = "Jass.Common["NewSoundEnvironment"]({0})"
            public static extern void NewSoundEnvironment(string environmentName);

            // orderIdString string
            /// @CSharpLua.Template = "Jass.Common["OrderId"]({0})"
            public static extern float OrderId(string orderIdString);

            // orderId number int
            /// @CSharpLua.Template = "Jass.Common["OrderId2String"]({0})"
            public static extern string OrderId2String(float orderId);

            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["PanCameraTo"]({0}, {1})"
            public static extern void PanCameraTo(float x, float y);

            // 平移镜头(所有玩家)(限时) [R]
            // 平移玩家镜头到([X],[Y]),持续[Time]秒
            // x number
            // y number
            // duration number
            /// @CSharpLua.Template = "Jass.Common["PanCameraToTimed"]({0}, {1}, {2})"
            public static extern void PanCameraToTimed(float x, float y, float duration);

            // 指定高度平移镜头(所有玩家)(限时) [R]
            // 平移玩家镜头到([X],[Y]),镜头距离地面高度为[Z],持续[Time]秒
            // 在指定移动路径上镜头不会低于地面高度.
            // x number
            // y number
            // zOffsetDest number
            // duration number
            /// @CSharpLua.Template = "Jass.Common["PanCameraToTimedWithZ"]({0}, {1}, {2}, {3})"
            public static extern void PanCameraToTimedWithZ(float x, float y, float zOffsetDest, float duration);

            // x number
            // y number
            // zOffsetDest number
            /// @CSharpLua.Template = "Jass.Common["PanCameraToWithZ"]({0}, {1}, {2})"
            public static extern void PanCameraToWithZ(float x, float y, float zOffsetDest);

            // 暂停/恢复 AI脚本运行 [R]
            // 设定[某个玩家][暂停/恢复]当前AI脚本的运行
            // 事实上该函数是有问题的,可以这么理解:设玩家当前AI脚本的运行状态R为0,暂停1次则R+1,恢复1次则R-1,仅当R=0时该玩家才会运行AI. 在使用前请先理解这段话的意思.
            // p number player
            // pause boolean
            /// @CSharpLua.Template = "Jass.Common["PauseCompAI"]({0}, {1})"
            public static extern void PauseCompAI(float p, bool pause);

            // 暂停/恢复游戏 [R]
            // [暂停/恢复]游戏
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["PauseGame"]({0})"
            public static extern void PauseGame(bool flag);

            // 暂停计时器 [R]
            // 暂停[计时器]
            // whichTimer number timer
            /// @CSharpLua.Template = "Jass.Common["PauseTimer"]({0})"
            public static extern void PauseTimer(int whichTimer);

            // 暂停/恢复 [R]
            // 设置[单位][Pause/Unpause]
            // whichUnit number unit
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["PauseUnit"]({0}, {1})"
            public static extern void PauseUnit(int whichUnit, bool flag);

            // 小地图信号(所有玩家) [R]
            // 对所有玩家发送小地图信号到坐标([X],[Y]) 持续时间:[Duration]秒
            // x number
            // y number
            // duration number
            /// @CSharpLua.Template = "Jass.Common["PingMinimap"]({0}, {1}, {2})"
            public static extern void PingMinimap(float x, float y, float duration);

            // 小地图信号(指定颜色)(所有玩家) [R]
            // 对所有玩家发送小地图信号到坐标([X],[Y]) 持续时间:[Duration]秒, 信号颜色:([Red],[Green],[Blue]) 信号类型:[Style]
            // 颜色格式为(红,绿,蓝). 颜色值取值范围为0-255.
            // x number
            // y number
            // duration number
            // red number int
            // green number int
            // blue number int
            // extraEffects boolean
            /// @CSharpLua.Template = "Jass.Common["PingMinimapEx"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern void PingMinimapEx(float x, float y, float duration, float red, float green, float blue, bool extraEffects);

            // 选择放置物品 [R]
            // 从[物品池]中任意选择一个物品并放置到([X],[Y])点
            // whichItemPool number itempool
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["PlaceRandomItem"]({0}, {1}, {2})"
            public static extern float PlaceRandomItem(float whichItemPool, float x, float y);

            // 选择放置单位 [R]
            // 从[单位池]中为[玩家]任意选择一个单位并放置到点([X],[Y]) 面向[度]
            // whichPool number unitpool
            // forWhichPlayer number player
            // x number
            // y number
            // facing number
            /// @CSharpLua.Template = "Jass.Common["PlaceRandomUnit"]({0}, {1}, {2}, {3}, {4})"
            public static extern float PlaceRandomUnit(float whichPool, float forWhichPlayer, float x, float y, float facing);

            // movieName string
            /// @CSharpLua.Template = "Jass.Common["PlayCinematic"]({0})"
            public static extern void PlayCinematic(string movieName);

            // modelName string
            /// @CSharpLua.Template = "Jass.Common["PlayModelCinematic"]({0})"
            public static extern void PlayModelCinematic(string modelName);

            // 播放背景音乐
            // 播放[背景音乐]
            // musicName string
            /// @CSharpLua.Template = "Jass.Common["PlayMusic"]({0})"
            public static extern void PlayMusic(string musicName);

            // 跳播背景音乐
            // 播放[背景音乐],跳过开始[Offset]秒,淡入时间:[Fade Time]秒
            // musicName string
            // frommsecs number int
            // fadeinmsecs number int
            /// @CSharpLua.Template = "Jass.Common["PlayMusicEx"]({0}, {1}, {2})"
            public static extern void PlayMusicEx(string musicName, float frommsecs, float fadeinmsecs);

            // 播放主题音乐 [C]
            // 播放[Music Theme]主题音乐
            // 播放主题音乐一次,然后恢复原来的音乐.
            // musicFileName string
            /// @CSharpLua.Template = "Jass.Common["PlayThematicMusic"]({0})"
            public static extern void PlayThematicMusic(string musicFileName);

            // 跳播主题音乐 [R]
            // 播放[Music Theme]主题音乐,跳过开始[Offset]毫秒
            // 播放主题音乐一次,然后恢复原来的音乐.
            // musicFileName string
            // frommsecs number int
            /// @CSharpLua.Template = "Jass.Common["PlayThematicMusicEx"]({0}, {1})"
            public static extern void PlayThematicMusicEx(string musicFileName, float frommsecs);

            // number number int
            /// @CSharpLua.Template = "Jass.Common["Player"]({0})"
            public static extern float Player(float number);

            // toPlayer number player
            /// @CSharpLua.Template = "Jass.Common["PlayerGetLeaderboard"]({0})"
            public static extern float PlayerGetLeaderboard(int toPlayer);

            // 设置玩家使用的排行榜 [R]
            // 设置[某个玩家]使用[排行榜]
            // 每个玩家只能显示一个排行榜.
            // toPlayer number player
            // lb number leaderboard
            /// @CSharpLua.Template = "Jass.Common["PlayerSetLeaderboard"]({0}, {1})"
            public static extern void PlayerSetLeaderboard(int toPlayer, float lb);

            // 预载文件
            // 预载[文件]
            // 可以事先载入文件并调入到游戏内存,以加快游戏的速度.
            // filename string
            /// @CSharpLua.Template = "Jass.Common["Preload"]({0})"
            public static extern void Preload(string filename);

            // 开始预载
            // 开始预载, 超时设置[Time]秒
            // 将文件调入到游戏内存中.
            // timeout number
            /// @CSharpLua.Template = "Jass.Common["PreloadEnd"]({0})"
            public static extern void PreloadEnd(float timeout);

            /// @CSharpLua.Template = "Jass.Common["PreloadEndEx"]()"
            public static extern void PreloadEndEx();

            /// @CSharpLua.Template = "Jass.Common["PreloadGenClear"]()"
            public static extern void PreloadGenClear();

            // filename string
            /// @CSharpLua.Template = "Jass.Common["PreloadGenEnd"]({0})"
            public static extern void PreloadGenEnd(string filename);

            /// @CSharpLua.Template = "Jass.Common["PreloadGenStart"]()"
            public static extern void PreloadGenStart();

            /// @CSharpLua.Template = "Jass.Common["PreloadRefresh"]()"
            public static extern void PreloadRefresh();

            /// @CSharpLua.Template = "Jass.Common["PreloadStart"]()"
            public static extern void PreloadStart();

            // 批量预载
            // 预载所有在[文件]中列出的文件
            // filename string
            /// @CSharpLua.Template = "Jass.Common["Preloader"]({0})"
            public static extern void Preloader(string filename);

            // whichQuest number quest
            /// @CSharpLua.Template = "Jass.Common["QuestCreateItem"]({0})"
            public static extern float QuestCreateItem(float whichQuest);

            // 设置任务项目完成
            // 设置[Quest Requirement][Completed/Incomplete]
            // whichQuestItem number questitem
            // completed boolean
            /// @CSharpLua.Template = "Jass.Common["QuestItemSetCompleted"]({0}, {1})"
            public static extern void QuestItemSetCompleted(float whichQuestItem, bool completed);

            // 改变任务项目说明
            // 改变[Quest Requirement]的说明为:[文字]
            // whichQuestItem number questitem
            // description string
            /// @CSharpLua.Template = "Jass.Common["QuestItemSetDescription"]({0}, {1})"
            public static extern void QuestItemSetDescription(float whichQuestItem, string description);

            // 设置任务完成
            // 设置[某个任务][Completed/Incomplete]
            // whichQuest number quest
            // completed boolean
            /// @CSharpLua.Template = "Jass.Common["QuestSetCompleted"]({0}, {1})"
            public static extern void QuestSetCompleted(float whichQuest, bool completed);

            // 设置任务说明
            // 设置[某个任务]的任务说明为:[文字]
            // whichQuest number quest
            // description string
            /// @CSharpLua.Template = "Jass.Common["QuestSetDescription"]({0}, {1})"
            public static extern void QuestSetDescription(float whichQuest, string description);

            // 设置任务被发现
            // 设置[某个任务][Discovered/Undiscovered]
            // whichQuest number quest
            // discovered boolean
            /// @CSharpLua.Template = "Jass.Common["QuestSetDiscovered"]({0}, {1})"
            public static extern void QuestSetDiscovered(float whichQuest, bool discovered);

            // 启用/禁用 任务 [R]
            // 设置[某个任务][Enable/Disable]
            // 被禁用的任务将不会显示在任务列表.
            // whichQuest number quest
            // enabled boolean
            /// @CSharpLua.Template = "Jass.Common["QuestSetEnabled"]({0}, {1})"
            public static extern void QuestSetEnabled(float whichQuest, bool enabled);

            // 设置任务失败
            // 设置[某个任务][Failed/Not Failed]
            // whichQuest number quest
            // failed boolean
            /// @CSharpLua.Template = "Jass.Common["QuestSetFailed"]({0}, {1})"
            public static extern void QuestSetFailed(float whichQuest, bool failed);

            // whichQuest number quest
            // iconPath string
            /// @CSharpLua.Template = "Jass.Common["QuestSetIconPath"]({0}, {1})"
            public static extern void QuestSetIconPath(float whichQuest, string iconPath);

            // whichQuest number quest
            // required boolean
            /// @CSharpLua.Template = "Jass.Common["QuestSetRequired"]({0}, {1})"
            public static extern void QuestSetRequired(float whichQuest, bool required);

            // 设置任务标题
            // 设置[某个任务]的标题为[文字]
            // whichQuest number quest
            // title string
            /// @CSharpLua.Template = "Jass.Common["QuestSetTitle"]({0}, {1})"
            public static extern void QuestSetTitle(float whichQuest, string title);

            // 将可破坏物动画加入队列
            // 将[可破坏物]的[Animation Name]动作加入队列
            // d number destructable
            // whichAnimation string
            /// @CSharpLua.Template = "Jass.Common["QueueDestructableAnimation"]({0}, {1})"
            public static extern void QueueDestructableAnimation(float d, string whichAnimation);

            // 单位动画加入队列
            // 把[单位]的[Animation Name]动作添加到动作队列
            // 单位按队列中运作的先后顺序播放动作.
            // whichUnit number unit
            // whichAnimation string
            /// @CSharpLua.Template = "Jass.Common["QueueUnitAnimation"]({0}, {1})"
            public static extern void QueueUnitAnimation(int whichUnit, string whichAnimation);

            // minx number
            // miny number
            // maxx number
            // maxy number
            /// @CSharpLua.Template = "Jass.Common["Rect"]({0}, {1}, {2}, {3})"
            public static extern float Rect(float minx, float miny, float maxx, float maxy);

            // min number location
            // max number location
            /// @CSharpLua.Template = "Jass.Common["RectFromLoc"]({0}, {1})"
            public static extern float RectFromLoc(float min, float max);

            // 恢复指定单位的警戒点
            // 恢复[单位]的警戒点
            // 这个动作通过 AI 来恢复特定单位的警戒点.
            // hUnit number unit
            /// @CSharpLua.Template = "Jass.Common["RecycleGuardPosition"]({0})"
            public static extern void RecycleGuardPosition(float hUnit);

            // 添加单元点(指定坐标) [R]
            // 对[不规则区域]添加单元点: ([X],[Y])
            // 单元点大小为32x32.
            // whichRegion number region
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["RegionAddCell"]({0}, {1}, {2})"
            public static extern void RegionAddCell(float whichRegion, float x, float y);

            // 添加单元点(指定点) [R]
            // 对[不规则区域]添加单元点:[坐标]
            // 单元点大小为32x32.
            // whichRegion number region
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["RegionAddCellAtLoc"]({0}, {1})"
            public static extern void RegionAddCellAtLoc(float whichRegion, float whichLocation);

            // 添加区域 [R]
            // 对[不规则区域]添加[矩形区域]
            // 区域是游戏中一个游戏地区的集合体,可以包含地区和点.
            // whichRegion number region
            // r number rect
            /// @CSharpLua.Template = "Jass.Common["RegionAddRect"]({0}, {1})"
            public static extern void RegionAddRect(float whichRegion, float r);

            // 移除单元点(指定坐标) [R]
            // 在[不规则区域]中移除单元点: ([X],[Y])
            // 单元点大小为32x32.
            // whichRegion number region
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["RegionClearCell"]({0}, {1}, {2})"
            public static extern void RegionClearCell(float whichRegion, float x, float y);

            // 移除单元点(指定点) [R]
            // 在[不规则区域]中移除单元点:[坐标]
            // 单元点大小为32x32.
            // whichRegion number region
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["RegionClearCellAtLoc"]({0}, {1})"
            public static extern void RegionClearCellAtLoc(float whichRegion, float whichLocation);

            // 移除区域 [R]
            // 在[不规则区域]中移除[矩形区域]
            // whichRegion number region
            // r number rect
            /// @CSharpLua.Template = "Jass.Common["RegionClearRect"]({0}, {1})"
            public static extern void RegionClearRect(float whichRegion, float r);

            // soundHandle number sound
            // byPosition boolean
            // rectwidth number
            // rectheight number
            /// @CSharpLua.Template = "Jass.Common["RegisterStackedSound"]({0}, {1}, {2}, {3})"
            public static extern void RegisterStackedSound(float soundHandle, bool byPosition, float rectwidth, float rectheight);

            /// @CSharpLua.Template = "Jass.Common["ReloadGame"]()"
            public static extern void ReloadGame();

            // 读取本地缓存数据
            // 从本地硬盘读取缓存数据
            // 只对单机游戏有效,从本地硬盘读取缓存数据,主要用来实现战役关卡间的数据传递.
            /// @CSharpLua.Template = "Jass.Common["ReloadGameCachesFromDisk"]()"
            public static extern bool ReloadGameCachesFromDisk();

            // 忽视所有单位的警戒点
            // 忽视[某个玩家]的所有单位的警戒点
            // 单位将不会自动返回原警戒点. 一个很有用的功能就是刷怪进攻时忽视单位警戒范围的话,怪就不会想家了.
            // num number player
            /// @CSharpLua.Template = "Jass.Common["RemoveAllGuardPositions"]({0})"
            public static extern void RemoveAllGuardPositions(float num);

            // 删除
            // 删除[可破坏物]
            // d number destructable
            /// @CSharpLua.Template = "Jass.Common["RemoveDestructable"]({0})"
            public static extern void RemoveDestructable(float d);

            // 忽视指定单位的警戒点
            // 忽视[单位]的警戒点
            // 单位将不会自动返回原警戒点. 一个很有用的功能就是刷怪进攻时忽视单位警戒范围的话,怪就不会想家了.
            // hUnit number unit
            /// @CSharpLua.Template = "Jass.Common["RemoveGuardPosition"]({0})"
            public static extern void RemoveGuardPosition(float hUnit);

            // 删除
            // 删除[物品]
            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["RemoveItem"]({0})"
            public static extern void RemoveItem(float whichItem);

            // 删除物品(所有市场)
            // 删除[物品类型]从所有市场
            // 影响所有拥有'出售物品'技能的单位.
            // itemId number int
            /// @CSharpLua.Template = "Jass.Common["RemoveItemFromAllStock"]({0})"
            public static extern void RemoveItemFromAllStock(float itemId);

            // 删除物品(指定市场)
            // 删除[物品类型]从[Marketplace]
            // 只影响有'出售物品'技能的单位
            // whichUnit number unit
            // itemId number int
            /// @CSharpLua.Template = "Jass.Common["RemoveItemFromStock"]({0}, {1})"
            public static extern void RemoveItemFromStock(int whichUnit, float itemId);

            // 清除点 [R]
            // 清除[坐标]
            // 点是堆积最多的垃圾资源,不需要再使用的点都要记得清除掉.
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["RemoveLocation"]({0})"
            public static extern void RemoveLocation(float whichLocation);

            // 踢除玩家
            // 踢除[某个玩家]，玩家的游戏结果为[文字]
            // whichPlayer number player
            // gameResult any playergameresult
            /// @CSharpLua.Template = "Jass.Common["RemovePlayer"]({0}, {1})"
            public static extern void RemovePlayer(int whichPlayer, object gameResult);

            // 删除矩形区域 [R]
            // 删除[矩形区域]
            // whichRect number rect
            /// @CSharpLua.Template = "Jass.Common["RemoveRect"]({0})"
            public static extern void RemoveRect(float whichRect);

            // 删除不规则区域 [R]
            // 删除[不规则区域]
            // whichRegion number region
            /// @CSharpLua.Template = "Jass.Common["RemoveRegion"]({0})"
            public static extern void RemoveRegion(float whichRegion);

            // 删除存档文件夹
            // 删除[文件夹]
            // 文件夹内的内容都会被删除.
            // sourceDirName string
            /// @CSharpLua.Template = "Jass.Common["RemoveSaveDirectory"]({0})"
            public static extern bool RemoveSaveDirectory(string sourceDirName);

            // 清理哈希项存储的布尔值 @new
            // 清空[Hashtable] 主索引 [Value] 子索引[childKey] 之内的布尔值
            // 清空哈希表主索引下的布尔值
            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["RemoveSavedBoolean"]({0}, {1}, {2})"
            public static extern void RemoveSavedBoolean(object table, float parentKey, float childKey);

            // 清理哈希项存储的句柄 @new
            // 清空[Hashtable] 主索引 [Value] 子索引[childKey] 之内的句柄
            // 清空哈希表主索引下的句柄
            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["RemoveSavedHandle"]({0}, {1}, {2})"
            public static extern void RemoveSavedHandle(object table, float parentKey, float childKey);

            // 清理哈希项存储的整数值 @new
            // 清空[Hashtable] 主索引 [Value] 子索引[childKey] 之内的整数值
            // 清空哈希表主索引下的整数值
            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["RemoveSavedInteger"]({0}, {1}, {2})"
            public static extern void RemoveSavedInteger(object table, float parentKey, float childKey);

            // 清理哈希项存储的实数值 @new
            // 清空[Hashtable] 主索引 [Value] 子索引[childKey] 之内的实数值
            // 清空哈希表主索引下的实数值
            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["RemoveSavedReal"]({0}, {1}, {2})"
            public static extern void RemoveSavedReal(object table, float parentKey, float childKey);

            // 清理哈希项存储的字符串 @new
            // 清空[Hashtable] 主索引 [Value] 子索引[childKey] 之内的字符串
            // 清空哈希表主索引下的字符串
            // table any hashtable
            // parentKey number int
            // childKey number int
            /// @CSharpLua.Template = "Jass.Common["RemoveSavedString"]({0}, {1}, {2})"
            public static extern void RemoveSavedString(object table, float parentKey, float childKey);

            // 删除
            // 删除[单位]
            // 被删除的单位不会留下尸体. 如果是英雄则不能再被复活.
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["RemoveUnit"]({0})"
            public static extern void RemoveUnit(int whichUnit);

            // 删除单位(所有市场)
            // 删除[单位类型]从所有市场
            // 影响所有拥有'出售单位'技能的单位.
            // unitId number int
            /// @CSharpLua.Template = "Jass.Common["RemoveUnitFromAllStock"]({0})"
            public static extern void RemoveUnitFromAllStock(int unitId);

            // 删除单位(指定市场)
            // 删除[单位类型]从[Marketplace]
            // 只影响有'出售单位'技能的单位.
            // whichUnit number unit
            // unitId number int
            /// @CSharpLua.Template = "Jass.Common["RemoveUnitFromStock"]({0}, {1})"
            public static extern void RemoveUnitFromStock(int whichUnit, int unitId);

            // 删除天气效果
            // 删除[天气效果]
            // whichEffect number weathereffect
            /// @CSharpLua.Template = "Jass.Common["RemoveWeatherEffect"]({0})"
            public static extern void RemoveWeatherEffect(int whichEffect);

            // 重命名存档文件夹
            // 更改[源文件夹]的名字为[目标文件夹]
            // sourceDirName string
            // destDirName string
            /// @CSharpLua.Template = "Jass.Common["RenameSaveDirectory"]({0}, {1})"
            public static extern bool RenameSaveDirectory(string sourceDirName, string destDirName);

            // 重置迷雾
            // 重置迷雾为默认设置
            /// @CSharpLua.Template = "Jass.Common["ResetTerrainFog"]()"
            public static extern void ResetTerrainFog();

            // 重置游戏镜头(所有玩家) [R]
            // 重置玩家镜头为游戏默认状态,持续 [Time]秒
            // duration number
            /// @CSharpLua.Template = "Jass.Common["ResetToGameCamera"]({0})"
            public static extern void ResetToGameCamera(float duration);

            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["ResetTrigger"]({0})"
            public static extern void ResetTrigger(int whichTrigger);

            // 重置地面纹理变化
            // 重置[某个地形纹理]
            // whichSplat any ubersplat
            /// @CSharpLua.Template = "Jass.Common["ResetUbersplat"]({0})"
            public static extern void ResetUbersplat(object whichSplat);

            // 重置身体朝向
            // 重置[单位]的身体朝向
            // 恢复单位的身体朝向为正常状态.
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["ResetUnitLookAt"]({0})"
            public static extern void ResetUnitLookAt(int whichUnit);

            // doScoreScreen boolean
            /// @CSharpLua.Template = "Jass.Common["RestartGame"]({0})"
            public static extern void RestartGame(bool doScoreScreen);

            // cache any gamecache
            // missionKey string
            // key string
            // forWhichPlayer number player
            // x number
            // y number
            // facing number
            /// @CSharpLua.Template = "Jass.Common["RestoreUnit"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern float RestoreUnit(object cache, string missionKey, string key, float forWhichPlayer, float x, float y, float facing);

            // 恢复背景音乐
            /// @CSharpLua.Template = "Jass.Common["ResumeMusic"]()"
            public static extern void ResumeMusic();

            // 恢复计时器 [R]
            // 恢复[计时器]
            // whichTimer number timer
            /// @CSharpLua.Template = "Jass.Common["ResumeTimer"]({0})"
            public static extern void ResumeTimer(int whichTimer);

            // 立即复活(指定坐标) [R]
            // 立即复活[英雄]在([X],[Y]),[显示/隐藏]复活动画
            // 如果英雄正在祭坛复活,则会退回部分花费(默认为100%).
            // whichHero number unit
            // x number
            // y number
            // doEyecandy boolean
            /// @CSharpLua.Template = "Jass.Common["ReviveHero"]({0}, {1}, {2}, {3})"
            public static extern bool ReviveHero(float whichHero, float x, float y, bool doEyecandy);

            // 立即复活(指定点)
            // 立即复活[英雄]在[指定点],[显示/隐藏]复活动画
            // 如果英雄正在祭坛复活,则会退回部分花费(默认为100%).
            // whichHero number unit
            // loc number location
            // doEyecandy boolean
            /// @CSharpLua.Template = "Jass.Common["ReviveHeroLoc"]({0}, {1}, {2})"
            public static extern bool ReviveHeroLoc(float whichHero, float loc, bool doEyecandy);

            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichAbility number ability
            /// @CSharpLua.Template = "Jass.Common["SaveAbilityHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveAbilityHandle(object table, float parentKey, float childKey, float whichAbility);

            // @1.24 保存布尔 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存布尔[Value]
            // 使用 '哈希表 - 从哈希表提取布尔' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // value boolean
            /// @CSharpLua.Template = "Jass.Common["SaveBoolean"]({0}, {1}, {2}, {3})"
            public static extern void SaveBoolean(object table, float parentKey, float childKey, bool value);

            // @1.24 保存布尔表达式 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存布尔表达式[Value]
            // 使用 '哈希表 - 从哈希表提取布尔表达式' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichBoolexpr function boolexpr
            /// @CSharpLua.Template = "Jass.Common["SaveBooleanExprHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveBooleanExprHandle(object table, float parentKey, float childKey, object whichBoolexpr);

            // @1.24 保存对话框按钮 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存对话框按钮[Value]
            // 使用 '哈希表 - 从哈希表提取对话框按钮' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichButton number button
            /// @CSharpLua.Template = "Jass.Common["SaveButtonHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveButtonHandle(object table, float parentKey, float childKey, float whichButton);

            // @1.24 保存失败条件 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存失败条件[Value]
            // 使用 '哈希表 - 从哈希表提取失败条件' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichDefeatcondition function defeatcondition
            /// @CSharpLua.Template = "Jass.Common["SaveDefeatConditionHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveDefeatConditionHandle(object table, float parentKey, float childKey, object whichDefeatcondition);

            // @1.24 保存可破坏物 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存可破坏物[Value]
            // 使用 '哈希表 - 从哈希表提取可破坏物' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichDestructable number destructable
            /// @CSharpLua.Template = "Jass.Common["SaveDestructableHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveDestructableHandle(object table, float parentKey, float childKey, float whichDestructable);

            // @1.24 保存对话框 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存对话框[Value]
            // 使用 '哈希表 - 从哈希表提取对话框' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichDialog number dialog
            /// @CSharpLua.Template = "Jass.Common["SaveDialogHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveDialogHandle(object table, float parentKey, float childKey, int whichDialog);

            // @1.24 保存特效 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存特效[Value]
            // 使用 '哈希表 - 从哈希表提取特效' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichEffect number effect
            /// @CSharpLua.Template = "Jass.Common["SaveEffectHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveEffectHandle(object table, float parentKey, float childKey, int whichEffect);

            // @1.24 保存可见度修正器 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存可见度修正器[Value]
            // 使用 '哈希表 - 从哈希表提取可见度修正器' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichFogModifier number fogmodifier
            /// @CSharpLua.Template = "Jass.Common["SaveFogModifierHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveFogModifierHandle(object table, float parentKey, float childKey, float whichFogModifier);

            // @1.24 保存迷雾状态 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存迷雾状态[Value]
            // 使用 '哈希表 - 从哈希表提取迷雾状态' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichFogState any fogstate
            /// @CSharpLua.Template = "Jass.Common["SaveFogStateHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveFogStateHandle(object table, float parentKey, float childKey, object whichFogState);

            // @1.24 保存玩家组 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存玩家组[Value]
            // 使用 '哈希表 - 从哈希表提取玩家组' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichForce number force
            /// @CSharpLua.Template = "Jass.Common["SaveForceHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveForceHandle(object table, float parentKey, float childKey, int whichForce);

            // 保存进度 [R]
            // 保存游戏进度为:[Filename]
            // saveFileName string
            /// @CSharpLua.Template = "Jass.Common["SaveGame"]({0})"
            public static extern void SaveGame(string saveFileName);

            // 本地保存游戏缓存
            // 保存[Game Cache]到本地硬盘
            // 只对单机游戏有效,保存缓存数据到本地硬盘,主要用来实现战役关卡间的数据传递.
            // whichCache any gamecache
            /// @CSharpLua.Template = "Jass.Common["SaveGameCache"]({0})"
            public static extern bool SaveGameCache(object whichCache);

            // saveName string
            /// @CSharpLua.Template = "Jass.Common["SaveGameExists"]({0})"
            public static extern bool SaveGameExists(string saveName);

            // @1.24 保存单位组 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存单位组[Value]
            // 使用 '哈希表 - 从哈希表提取单位组' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichGroup number group
            /// @CSharpLua.Template = "Jass.Common["SaveGroupHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveGroupHandle(object table, float parentKey, float childKey, float whichGroup);

            // @1.24 保存图像 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存图像[Value]
            // 使用 '哈希表 - 从哈希表提取图像' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichImage number image
            /// @CSharpLua.Template = "Jass.Common["SaveImageHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveImageHandle(object table, float parentKey, float childKey, float whichImage);

            // @1.24 保存整数 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存整数[Value]
            // 使用 '哈希表 - 从哈希表提取整数' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // value number int
            /// @CSharpLua.Template = "Jass.Common["SaveInteger"]({0}, {1}, {2}, {3})"
            public static extern void SaveInteger(object table, float parentKey, float childKey, float value);

            // @1.24 保存物品 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存物品[Value]
            // 使用 '哈希表 - 从哈希表提取物品' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["SaveItemHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveItemHandle(object table, float parentKey, float childKey, float whichItem);

            // @1.24 保存物品池 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存物品池[Value]
            // 使用 '哈希表 - 从哈希表提取物品池' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichItempool number itempool
            /// @CSharpLua.Template = "Jass.Common["SaveItemPoolHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveItemPoolHandle(object table, float parentKey, float childKey, float whichItempool);

            // @1.24 保存排行榜 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存排行榜[Value]
            // 使用 '哈希表 - 从哈希表提取排行榜' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichLeaderboard number leaderboard
            /// @CSharpLua.Template = "Jass.Common["SaveLeaderboardHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveLeaderboardHandle(object table, float parentKey, float childKey, float whichLeaderboard);

            // @1.24 保存闪电效果 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存闪电效果[Value]
            // 使用 '哈希表 - 从哈希表提取闪电效果' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichLightning number lightning
            /// @CSharpLua.Template = "Jass.Common["SaveLightningHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveLightningHandle(object table, float parentKey, float childKey, float whichLightning);

            // @1.24 保存点 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存点[Value]
            // 使用 '哈希表 - 从哈希表提取点' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["SaveLocationHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveLocationHandle(object table, float parentKey, float childKey, float whichLocation);

            // @1.24 保存多面板 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存多面板[Value]
            // 使用 '哈希表 - 从哈希表提取多面板' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichMultiboard number multiboard
            /// @CSharpLua.Template = "Jass.Common["SaveMultiboardHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveMultiboardHandle(object table, float parentKey, float childKey, float whichMultiboard);

            // @1.24 保存多面板项目 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存多面板项目[Value]
            // 使用 '哈希表 - 从哈希表提取多面板项目' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichMultiboarditem number multiboarditem
            /// @CSharpLua.Template = "Jass.Common["SaveMultiboardItemHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveMultiboardItemHandle(object table, float parentKey, float childKey, float whichMultiboarditem);

            // @1.24 保存玩家 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存玩家[Value]
            // 使用 '哈希表 - 从哈希表提取玩家' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichPlayer number player
            /// @CSharpLua.Template = "Jass.Common["SavePlayerHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SavePlayerHandle(object table, float parentKey, float childKey, int whichPlayer);

            // @1.24 保存任务 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存任务[Value]
            // 使用 '哈希表 - 从哈希表提取任务' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichQuest number quest
            /// @CSharpLua.Template = "Jass.Common["SaveQuestHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveQuestHandle(object table, float parentKey, float childKey, float whichQuest);

            // @1.24 保存任务要求 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存任务要求[Value]
            // 使用 '哈希表 - 从哈希表提取任务要求' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichQuestitem number questitem
            /// @CSharpLua.Template = "Jass.Common["SaveQuestItemHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveQuestItemHandle(object table, float parentKey, float childKey, float whichQuestitem);

            // @1.24 保存实数 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存实数[Value]
            // 使用 '哈希表 - 从哈希表提取实数' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // value number
            /// @CSharpLua.Template = "Jass.Common["SaveReal"]({0}, {1}, {2}, {3})"
            public static extern void SaveReal(object table, float parentKey, float childKey, float value);

            // @1.24 保存区域(矩型) [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存区域(矩型)[Value]
            // 使用 '哈希表 - 从哈希表提取区域(矩型)' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichRect number rect
            /// @CSharpLua.Template = "Jass.Common["SaveRectHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveRectHandle(object table, float parentKey, float childKey, float whichRect);

            // @1.24 保存区域(不规则) [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存区域(不规则)[Value]
            // 使用 '哈希表 - 从哈希表提取区域(不规则)' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichRegion number region
            /// @CSharpLua.Template = "Jass.Common["SaveRegionHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveRegionHandle(object table, float parentKey, float childKey, float whichRegion);

            // @1.24 保存音效 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存音效[Value]
            // 使用 '哈希表 - 从哈希表提取音效' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichSound number sound
            /// @CSharpLua.Template = "Jass.Common["SaveSoundHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveSoundHandle(object table, float parentKey, float childKey, float whichSound);

            // @1.24 保存字符串 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存字符串[Value]
            // 使用 '哈希表 - 从哈希表提取字符串' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // value string
            /// @CSharpLua.Template = "Jass.Common["SaveStr"]({0}, {1}, {2}, {3})"
            public static extern bool SaveStr(object table, float parentKey, float childKey, string value);

            // @1.24 保存漂浮文字 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存漂浮文字[Value]
            // 使用 '哈希表 - 从哈希表提取漂浮文字' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichTexttag number texttag
            /// @CSharpLua.Template = "Jass.Common["SaveTextTagHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveTextTagHandle(object table, float parentKey, float childKey, float whichTexttag);

            // @1.24 保存计时器窗口 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存计时器窗口[Value]
            // 使用 '哈希表 - 从哈希表提取计时器窗口' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichTimerdialog number timerdialog
            /// @CSharpLua.Template = "Jass.Common["SaveTimerDialogHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveTimerDialogHandle(object table, float parentKey, float childKey, float whichTimerdialog);

            // @1.24 保存计时器 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存计时器[Value]
            // 使用 '哈希表 - 从哈希表提取计时器' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichTimer number timer
            /// @CSharpLua.Template = "Jass.Common["SaveTimerHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveTimerHandle(object table, float parentKey, float childKey, int whichTimer);

            // @1.24 保存可追踪物 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存可追踪物[Value]
            // 使用 '哈希表 - 从哈希表提取可追踪物' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichTrackable any trackable
            /// @CSharpLua.Template = "Jass.Common["SaveTrackableHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveTrackableHandle(object table, float parentKey, float childKey, object whichTrackable);

            // @1.24 保存触发动作 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存触发动作[Value]
            // 使用 '哈希表 - 从哈希表提取触发动作' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichTriggeraction function triggeraction
            /// @CSharpLua.Template = "Jass.Common["SaveTriggerActionHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveTriggerActionHandle(object table, float parentKey, float childKey, object whichTriggeraction);

            // @1.24 保存触发条件 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存触发条件[Value]
            // 使用 '哈希表 - 从哈希表提取触发条件' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichTriggercondition function triggercondition
            /// @CSharpLua.Template = "Jass.Common["SaveTriggerConditionHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveTriggerConditionHandle(object table, float parentKey, float childKey, object whichTriggercondition);

            // @1.24 保存触发事件 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存触发事件[Value]
            // 使用 '哈希表 - 从哈希表提取触发事件' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichEvent any event
            /// @CSharpLua.Template = "Jass.Common["SaveTriggerEventHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveTriggerEventHandle(object table, float parentKey, float childKey, object whichEvent);

            // @1.24 保存触发器 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存触发器[Value]
            // 使用 '哈希表 - 从哈希表提取触发器' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["SaveTriggerHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveTriggerHandle(object table, float parentKey, float childKey, int whichTrigger);

            // @1.24 保存地面纹理变化 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存地面纹理变化[Value]
            // 使用 '哈希表 - 从哈希表提取地面纹理变化' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichUbersplat any ubersplat
            /// @CSharpLua.Template = "Jass.Common["SaveUbersplatHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveUbersplatHandle(object table, float parentKey, float childKey, object whichUbersplat);

            // @1.24 保存单位 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存单位[Value]
            // 使用 '哈希表 - 从哈希表提取单位' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["SaveUnitHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveUnitHandle(object table, float parentKey, float childKey, int whichUnit);

            // @1.24 保存单位池 [C]
            // 在[Hashtable]的主索引[Value]子索引[Value]中保存单位池[Value]
            // 使用 '哈希表 - 从哈希表提取单位池' 可以取出保存的值
            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichUnitpool number unitpool
            /// @CSharpLua.Template = "Jass.Common["SaveUnitPoolHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveUnitPoolHandle(object table, float parentKey, float childKey, float whichUnitpool);

            // table any hashtable
            // parentKey number int
            // childKey number int
            // whichWidget number widget
            /// @CSharpLua.Template = "Jass.Common["SaveWidgetHandle"]({0}, {1}, {2}, {3})"
            public static extern bool SaveWidgetHandle(object table, float parentKey, float childKey, float whichWidget);

            // 学习技能
            // 命令[某个英雄]学习技能[Skill]
            // 只有当英雄有剩余技能点时有效.
            // whichHero number unit
            // abilcode number int
            /// @CSharpLua.Template = "Jass.Common["SelectHeroSkill"]({0}, {1})"
            public static extern void SelectHeroSkill(float whichHero, float abilcode);

            // whichUnit number unit
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SelectUnit"]({0}, {1})"
            public static extern void SelectUnit(int whichUnit, bool flag);

            // 限制物品种类(所有市场)
            // 限制所有市场的可出售物品种类数为[Quantity]
            // 影响所有拥有'出售物品'技能的单位.
            // slots number int
            /// @CSharpLua.Template = "Jass.Common["SetAllItemTypeSlots"]({0})"
            public static extern void SetAllItemTypeSlots(float slots);

            // 限制单位种类(所有市场)
            // 限制所有市场的可出售单位种类数为[Quantity]
            // 影响所有拥有'出售单位'技能的单位.
            // slots number int
            /// @CSharpLua.Template = "Jass.Common["SetAllUnitTypeSlots"]({0})"
            public static extern void SetAllUnitTypeSlots(float slots);

            // 设置联盟颜色显示
            // 设置联盟颜色显示状态为[State]
            // 0为不开启. 1为小地图显示. 2为小地图和游戏都显示. 相当于游戏中Alt+A功能.
            // state number int
            /// @CSharpLua.Template = "Jass.Common["SetAllyColorFilterState"]({0})"
            public static extern void SetAllyColorFilterState(float state);

            // 设置小地图特殊标志
            // 设置小地图特殊标志为[某个图像]
            // 必须使用16x16的图像.
            // iconPath string
            /// @CSharpLua.Template = "Jass.Common["SetAltMinimapIcon"]({0})"
            public static extern void SetAltMinimapIcon(string iconPath);

            // 创建/删除荒芜地表(圆范围)(指定坐标) [R]
            // 为[某个玩家]在圆心为([X],[Y]),半径为[R]的圆范围内[Create/Remove]一块荒芜地表
            // whichPlayer number player
            // x number
            // y number
            // radius number
            // addBlight boolean
            /// @CSharpLua.Template = "Jass.Common["SetBlight"]({0}, {1}, {2}, {3}, {4})"
            public static extern void SetBlight(int whichPlayer, float x, float y, float radius, bool addBlight);

            // whichPlayer number player
            // whichLocation number location
            // radius number
            // addBlight boolean
            /// @CSharpLua.Template = "Jass.Common["SetBlightLoc"]({0}, {1}, {2}, {3})"
            public static extern void SetBlightLoc(int whichPlayer, float whichLocation, float radius, bool addBlight);

            // whichPlayer number player
            // x number
            // y number
            // addBlight boolean
            /// @CSharpLua.Template = "Jass.Common["SetBlightPoint"]({0}, {1}, {2}, {3})"
            public static extern void SetBlightPoint(int whichPlayer, float x, float y, bool addBlight);

            // 创建/删除荒芜地表(矩形区域) [R]
            // 为[某个玩家]在[Region][Create/Remove]一块荒芜地表
            // whichPlayer number player
            // r number rect
            // addBlight boolean
            /// @CSharpLua.Template = "Jass.Common["SetBlightRect"]({0}, {1}, {2})"
            public static extern void SetBlightRect(int whichPlayer, float r, bool addBlight);

            // 设置可用镜头区域(所有玩家) [R]
            // 设置玩家可用镜头区域: 左下角([X],[Y]), 左上角([X],[Y]), 右上角([X],[Y]), 右下角([X],[Y])
            // 该动作同样会影响小地图的显示. 但小地图的图片是无法改变的. 实际可用区域要大于可用镜头区域.
            // x1 number
            // y1 number
            // x2 number
            // y2 number
            // x3 number
            // y3 number
            // x4 number
            // y4 number
            /// @CSharpLua.Template = "Jass.Common["SetCameraBounds"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
            public static extern void SetCameraBounds(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);

            // 设置镜头属性(所有玩家)(限时) [R]
            // 设置玩家的镜头属性[Field]为[数值],持续[Time]秒
            // whichField any camerafield
            // value number
            // duration number
            /// @CSharpLua.Template = "Jass.Common["SetCameraField"]({0}, {1}, {2})"
            public static extern void SetCameraField(object whichField, float value, float duration);

            // 锁定镜头到单位(固定镜头源)(所有玩家) [R]
            // 锁定玩家镜头到[单位], 偏移坐标([X],[Y])
            // 偏移坐标(X,Y)以单位脚底为原点坐标.
            // whichUnit number unit
            // xoffset number
            // yoffset number
            /// @CSharpLua.Template = "Jass.Common["SetCameraOrientController"]({0}, {1}, {2})"
            public static extern void SetCameraOrientController(int whichUnit, float xoffset, float yoffset);

            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["SetCameraPosition"]({0}, {1})"
            public static extern void SetCameraPosition(float x, float y);

            // 设置空格键转向点(所有玩家) [R]
            // 设置玩家的空格键转向点为([X],[Y])
            // 按下空格键时镜头转向的位置.
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["SetCameraQuickPosition"]({0}, {1})"
            public static extern void SetCameraQuickPosition(float x, float y);

            // 指定点旋转镜头(所有玩家)(弧度)(限时) [R]
            // 以([X],[Y])为中心,旋转弧度为[Rad], 持续:[Time]秒
            // x number
            // y number
            // radiansToSweep number
            // duration number
            /// @CSharpLua.Template = "Jass.Common["SetCameraRotateMode"]({0}, {1}, {2}, {3})"
            public static extern void SetCameraRotateMode(float x, float y, float radiansToSweep, float duration);

            // 锁定镜头到单位(所有玩家) [R]
            // 锁定玩家镜头到[单位], 偏移坐标([X],[Y]) ,使用[Rotation Source]
            // 偏移坐标(X,Y)以单位脚底为原点坐标.
            // whichUnit number unit
            // xoffset number
            // yoffset number
            // inheritOrientation boolean
            /// @CSharpLua.Template = "Jass.Common["SetCameraTargetController"]({0}, {1}, {2}, {3})"
            public static extern void SetCameraTargetController(int whichUnit, float xoffset, float yoffset, bool inheritOrientation);

            // 允许/禁止 战役
            // [Enable/Disable][Campaign]
            // campaignNumber number int
            // available boolean
            /// @CSharpLua.Template = "Jass.Common["SetCampaignAvailable"]({0}, {1})"
            public static extern void SetCampaignAvailable(float campaignNumber, bool available);

            // 设置战役背景
            // 设置战役背景为[Campaign]
            // r any race
            /// @CSharpLua.Template = "Jass.Common["SetCampaignMenuRace"]({0})"
            public static extern void SetCampaignMenuRace(object r);

            // campaignIndex number int
            /// @CSharpLua.Template = "Jass.Common["SetCampaignMenuRaceEx"]({0})"
            public static extern void SetCampaignMenuRaceEx(float campaignIndex);

            // whichMode any blendmode
            /// @CSharpLua.Template = "Jass.Common["SetCineFilterBlendMode"]({0})"
            public static extern void SetCineFilterBlendMode(object whichMode);

            // duration number
            /// @CSharpLua.Template = "Jass.Common["SetCineFilterDuration"]({0})"
            public static extern void SetCineFilterDuration(float duration);

            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["SetCineFilterEndColor"]({0}, {1}, {2}, {3})"
            public static extern void SetCineFilterEndColor(float red, float green, float blue, float alpha);

            // minu number
            // minv number
            // maxu number
            // maxv number
            /// @CSharpLua.Template = "Jass.Common["SetCineFilterEndUV"]({0}, {1}, {2}, {3})"
            public static extern void SetCineFilterEndUV(float minu, float minv, float maxu, float maxv);

            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["SetCineFilterStartColor"]({0}, {1}, {2}, {3})"
            public static extern void SetCineFilterStartColor(float red, float green, float blue, float alpha);

            // minu number
            // minv number
            // maxu number
            // maxv number
            /// @CSharpLua.Template = "Jass.Common["SetCineFilterStartUV"]({0}, {1}, {2}, {3})"
            public static extern void SetCineFilterStartUV(float minu, float minv, float maxu, float maxv);

            // whichFlags any texmapflags
            /// @CSharpLua.Template = "Jass.Common["SetCineFilterTexMapFlags"]({0})"
            public static extern void SetCineFilterTexMapFlags(object whichFlags);

            // filename string
            /// @CSharpLua.Template = "Jass.Common["SetCineFilterTexture"]({0})"
            public static extern void SetCineFilterTexture(string filename);

            // 播放电影镜头(所有玩家) [R]
            // 对所有玩家播放电影镜头:[Camera File]
            // 在'Objects\\CinematicCameras'目录下有一些电影镜头,可用Mpq工具来查询.
            // cameraModelFile string
            /// @CSharpLua.Template = "Jass.Common["SetCinematicCamera"]({0})"
            public static extern void SetCinematicCamera(string cameraModelFile);

            // portraitUnitId number int
            // color any playercolor
            // speakerTitle string
            // text string
            // sceneDuration number
            // voiceoverDuration number
            /// @CSharpLua.Template = "Jass.Common["SetCinematicScene"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern void SetCinematicScene(float portraitUnitId, object color, string speakerTitle, string text, float sceneDuration, float voiceoverDuration);

            // whichdensity any mapdensity
            /// @CSharpLua.Template = "Jass.Common["SetCreatureDensity"]({0})"
            public static extern void SetCreatureDensity(object whichdensity);

            // 设置小地图中立生物显示
            // 小地图[显示/隐藏]中立生物
            // 相当于游戏中Alt+R功能.
            // state boolean
            /// @CSharpLua.Template = "Jass.Common["SetCreepCampFilterState"]({0})"
            public static extern void SetCreepCampFilterState(bool state);

            // whichButton number int
            // visible boolean
            /// @CSharpLua.Template = "Jass.Common["SetCustomCampaignButtonVisible"]({0}, {1})"
            public static extern void SetCustomCampaignButtonVisible(float whichButton, bool visible);

            // terrainDNCFile string
            // unitDNCFile string
            /// @CSharpLua.Template = "Jass.Common["SetDayNightModels"]({0}, {1})"
            public static extern void SetDayNightModels(string terrainDNCFile, string unitDNCFile);

            // g any gamedifficulty
            /// @CSharpLua.Template = "Jass.Common["SetDefaultDifficulty"]({0})"
            public static extern void SetDefaultDifficulty(object g);

            // 播放可破坏物动画
            // 播放[可破坏物]的[Animation Name]动作
            // d number destructable
            // whichAnimation string
            /// @CSharpLua.Template = "Jass.Common["SetDestructableAnimation"]({0}, {1})"
            public static extern void SetDestructableAnimation(float d, string whichAnimation);

            // 改变可破坏物动画播放速度 [R]
            // 改变[可破坏物]的动画播放速度为正常的[Percent]倍
            // 设置1倍动画播放速度来恢复正常状态.
            // d number destructable
            // speedFactor number
            /// @CSharpLua.Template = "Jass.Common["SetDestructableAnimationSpeed"]({0}, {1})"
            public static extern void SetDestructableAnimationSpeed(float d, float speedFactor);

            // 设置无敌/可攻击
            // 设置[可破坏物][Invulnerable/Vulnerable]
            // d number destructable
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetDestructableInvulnerable"]({0}, {1})"
            public static extern void SetDestructableInvulnerable(float d, bool flag);

            // 设置生命值(指定值)
            // 设置[可破坏物]的生命值为[Value]
            // d number destructable
            // life number
            /// @CSharpLua.Template = "Jass.Common["SetDestructableLife"]({0}, {1})"
            public static extern void SetDestructableLife(float d, float life);

            // 设置最大生命值
            // 设置[可破坏物]的最大生命值为[Value]
            // d number destructable
            // max number
            /// @CSharpLua.Template = "Jass.Common["SetDestructableMaxLife"]({0}, {1})"
            public static extern void SetDestructableMaxLife(float d, float max);

            // 设置闭塞高度
            // 设置[可破坏物]的闭塞高度为[Height]
            // d number destructable
            // height number
            /// @CSharpLua.Template = "Jass.Common["SetDestructableOccluderHeight"]({0}, {1})"
            public static extern void SetDestructableOccluderHeight(float d, float height);

            // 播放圆范围内地形装饰物动画 [R]
            // 选取圆心为([X],[Y]),半径为[半径]的圆范围内的[装饰物类型](选取方式:[选取方式]), 做[Animation Name]动作([允许/禁止]随机播放)
            // 特殊动画名: 'show', 'hide', 'soundon', 'soundoff'. 随机播放:比如某装饰物有好几个'stand'动作,则允许该项时会随机抽取某个动作播放,而禁止该项时只播放首个动作.
            // x number
            // y number
            // radius number
            // doodadID number int
            // nearestOnly boolean
            // animName string
            // animRandom boolean
            /// @CSharpLua.Template = "Jass.Common["SetDoodadAnimation"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern void SetDoodadAnimation(float x, float y, float radius, float doodadID, bool nearestOnly, string animName, bool animRandom);

            // 播放矩形区域内地形装饰物动画 [R]
            // 播放[Rect]内所有[装饰物类型]的[Animation Name]动作([允许/禁止]随机播放)
            // 特殊动画名: 'show', 'hide', 'soundon', 'soundoff'. 随机播放:比如某装饰物有好几个'stand'动作,则允许该项时会随机抽取某个动作播放,而禁止该项时只播放首个动作.
            // r number rect
            // doodadID number int
            // animName string
            // animRandom boolean
            /// @CSharpLua.Template = "Jass.Common["SetDoodadAnimationRect"]({0}, {1}, {2}, {3})"
            public static extern void SetDoodadAnimationRect(float r, float doodadID, string animName, bool animRandom);

            // campaignNumber number int
            // available boolean
            /// @CSharpLua.Template = "Jass.Common["SetEdCinematicAvailable"]({0}, {1})"
            public static extern void SetEdCinematicAvailable(float campaignNumber, bool available);

            // whichFloatGameState any fgamestate
            // value number
            /// @CSharpLua.Template = "Jass.Common["SetFloatGameState"]({0}, {1})"
            public static extern void SetFloatGameState(object whichFloatGameState, float value);

            // 设置地图迷雾(圆范围) [R]
            // 为[玩家]设置[FogStateVisible]在圆心为([X],[Y]) 半径为[数值]的范围, (对盟友[共享]视野)
            // forWhichPlayer number player
            // whichState any fogstate
            // centerx number
            // centerY number
            // radius number
            // useSharedVision boolean
            /// @CSharpLua.Template = "Jass.Common["SetFogStateRadius"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern void SetFogStateRadius(float forWhichPlayer, object whichState, float centerx, float centerY, float radius, bool useSharedVision);

            // forWhichPlayer number player
            // whichState any fogstate
            // center number location
            // radius number
            // useSharedVision boolean
            /// @CSharpLua.Template = "Jass.Common["SetFogStateRadiusLoc"]({0}, {1}, {2}, {3}, {4})"
            public static extern void SetFogStateRadiusLoc(float forWhichPlayer, object whichState, float center, float radius, bool useSharedVision);

            // 设置地图迷雾(矩形区域) [R]
            // 为[玩家]设置[FogStateVisible]在[矩形区域](对盟友[共享]视野)
            // forWhichPlayer number player
            // whichState any fogstate
            // where number rect
            // useSharedVision boolean
            /// @CSharpLua.Template = "Jass.Common["SetFogStateRect"]({0}, {1}, {2}, {3})"
            public static extern void SetFogStateRect(float forWhichPlayer, object whichState, int where, bool useSharedVision);

            // 设置游戏难度 [R]
            // 设置当前游戏难度为[GameDifficulty]
            // 游戏难度只是作为运行AI的一个参考值,没有AI的地图该功能无用.
            // whichdifficulty any gamedifficulty
            /// @CSharpLua.Template = "Jass.Common["SetGameDifficulty"]({0})"
            public static extern void SetGameDifficulty(object whichdifficulty);

            // whichPlacementType any placement
            /// @CSharpLua.Template = "Jass.Common["SetGamePlacement"]({0})"
            public static extern void SetGamePlacement(object whichPlacementType);

            // 设定游戏速度
            // 设定游戏速度为[Speed]
            // 你可以通过'游戏 - 锁定游戏速度'动作来锁定游戏速度.
            // whichspeed any gamespeed
            /// @CSharpLua.Template = "Jass.Common["SetGameSpeed"]({0})"
            public static extern void SetGameSpeed(object whichspeed);

            // whichGameType any gametype
            // value boolean
            /// @CSharpLua.Template = "Jass.Common["SetGameTypeSupported"]({0}, {1})"
            public static extern void SetGameTypeSupported(object whichGameType, bool value);

            // 设置英雄敏捷 [R]
            // 设置[英雄]的敏捷为[Value],([Permanent]永久奖励)
            // 永久奖励貌似无效项,不需要理会.
            // whichHero number unit
            // newAgi number int
            // permanent boolean
            /// @CSharpLua.Template = "Jass.Common["SetHeroAgi"]({0}, {1}, {2})"
            public static extern void SetHeroAgi(float whichHero, float newAgi, bool permanent);

            // 设置英雄智力 [R]
            // 设置[英雄]的智力为[Value],([Permanent]永久奖励)
            // 永久奖励貌似无效项,不需要理会.
            // whichHero number unit
            // newInt number int
            // permanent boolean
            /// @CSharpLua.Template = "Jass.Common["SetHeroInt"]({0}, {1}, {2})"
            public static extern void SetHeroInt(float whichHero, float newInt, bool permanent);

            // 设置等级
            // 设置[某个英雄]的英雄等级为[Level],[显示/隐藏]升级动画
            // 如果等级有变动,英雄经验将重置为该等级的初始值.
            // whichHero number unit
            // level number int
            // showEyeCandy boolean
            /// @CSharpLua.Template = "Jass.Common["SetHeroLevel"]({0}, {1}, {2})"
            public static extern void SetHeroLevel(float whichHero, float level, bool showEyeCandy);

            // 设置英雄力量 [R]
            // 设置[英雄]的力量为[Value],([Permanent]永久奖励)
            // 永久奖励貌似无效项,不需要理会.
            // whichHero number unit
            // newStr number int
            // permanent boolean
            /// @CSharpLua.Template = "Jass.Common["SetHeroStr"]({0}, {1}, {2})"
            public static extern void SetHeroStr(float whichHero, float newStr, bool permanent);

            // 设置经验值
            // 设置[某个英雄]的经验值为[Quantity],[显示/隐藏]升级动画
            // 经验值不能倒退.
            // whichHero number unit
            // newXpVal number int
            // showEyeCandy boolean
            /// @CSharpLua.Template = "Jass.Common["SetHeroXP"]({0}, {1}, {2})"
            public static extern void SetHeroXP(float whichHero, float newXpVal, bool showEyeCandy);

            // 图像水面显示状态
            // 设置[某个图像]:[Enable/Disable]水面显示,[Enable/Disable]水的Alpha通道
            // 前者设置图像在水面或是水底显示. 后者设置图像是否受水的Alpha通道影响.
            // whichImage number image
            // flag boolean
            // useWaterAlpha boolean
            /// @CSharpLua.Template = "Jass.Common["SetImageAboveWater"]({0}, {1}, {2})"
            public static extern void SetImageAboveWater(float whichImage, bool flag, bool useWaterAlpha);

            // 改变图像颜色 [R]
            // 设置[某个图像]的颜色值为([Red],[Green],[Blue]) Alpha值为[Alpha]
            // 颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围0-255.
            // whichImage number image
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["SetImageColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void SetImageColor(float whichImage, float red, float green, float blue, float alpha);

            // 设置图像高度
            // 设置[某个图像][Enable/Disable]Z轴显示,并设置高度为[Height]
            // 实际显示高度为图像高度+Z轴偏移. 只有允许Z轴显示时才有效.
            // whichImage number image
            // flag boolean
            // height number
            /// @CSharpLua.Template = "Jass.Common["SetImageConstantHeight"]({0}, {1}, {2})"
            public static extern void SetImageConstantHeight(float whichImage, bool flag, float height);

            // 改变图像位置(指定坐标) [R]
            // 改变[某个图像]的位置为([X],[Y]),Z轴偏移为[Z]
            // 指图像的左下角位置.
            // whichImage number image
            // x number
            // y number
            // z number
            /// @CSharpLua.Template = "Jass.Common["SetImagePosition"]({0}, {1}, {2}, {3})"
            public static extern void SetImagePosition(float whichImage, float x, float y, float z);

            // 设置图像渲染状态
            // 设置[某个图像]:[Enable/Disable]显示状态
            // 未发现有任何作用.
            // whichImage number image
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetImageRender"]({0}, {1})"
            public static extern void SetImageRender(float whichImage, bool flag);

            // 设置图像永久渲染状态
            // 设置[某个图像]:[Enable/Disable]永久显示状态
            // 要显示图像则必须开启该项.
            // whichImage number image
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetImageRenderAlways"]({0}, {1})"
            public static extern void SetImageRenderAlways(float whichImage, bool flag);

            // 改变图像类型
            // 改变[某个图像]类型为[Type]
            // whichImage number image
            // imageType number int
            /// @CSharpLua.Template = "Jass.Common["SetImageType"]({0}, {1})"
            public static extern void SetImageType(float whichImage, float imageType);

            // whichIntegerGameState any igamestate
            // value number int
            /// @CSharpLua.Template = "Jass.Common["SetIntegerGameState"]({0}, {1})"
            public static extern void SetIntegerGameState(object whichIntegerGameState, float value);

            // introModelPath string
            /// @CSharpLua.Template = "Jass.Common["SetIntroShotModel"]({0})"
            public static extern void SetIntroShotModel(string introModelPath);

            // introText string
            /// @CSharpLua.Template = "Jass.Common["SetIntroShotText"]({0})"
            public static extern void SetIntroShotText(string introText);

            // 设置物品使用次数
            // 设置[物品]的使用次数为[Charges]
            // 设置为0可以使物品能无限次使用.
            // whichItem number item
            // charges number int
            /// @CSharpLua.Template = "Jass.Common["SetItemCharges"]({0}, {1})"
            public static extern void SetItemCharges(float whichItem, float charges);

            // 设置重生神符的产生单位类型
            // 设置[物品]产生[单位类型]
            // 设置重生神符对应的单位类型后，当英雄吃了重生神符，则会产生指定类型的单位。
            // whichItem number item
            // unitId number int
            /// @CSharpLua.Template = "Jass.Common["SetItemDropID"]({0}, {1})"
            public static extern void SetItemDropID(float whichItem, int unitId);

            // 设置物品死亡是否掉落
            // 设置[物品][Drop from/Stay with]在持有者死亡时
            // whichItem number item
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetItemDropOnDeath"]({0}, {1})"
            public static extern void SetItemDropOnDeath(float whichItem, bool flag);

            // 设置物品可否丢弃
            // 设置[物品][Droppable/Undroppable]
            // 不可掉落物品在被捡起之后就不能移动和丢弃.(但可通过触发实现)
            // i number item
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetItemDroppable"]({0}, {1})"
            public static extern void SetItemDroppable(float i, bool flag);

            // 设置物品无敌/可攻击
            // 设置[物品][Invulnerable/Vulnerable]
            // whichItem number item
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetItemInvulnerable"]({0}, {1})"
            public static extern void SetItemInvulnerable(float whichItem, bool flag);

            // 设置物品可否抵押
            // 设置[物品][Pawnable/Unpawnable]
            // 不可抵押物品不能被卖到商店.
            // i number item
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetItemPawnable"]({0}, {1})"
            public static extern void SetItemPawnable(float i, bool flag);

            // 改变物品所属玩家
            // 改变[物品]的所属玩家为:[某个玩家]并[Change/Retain Color]
            // 不是所有物品都能改变颜色. 所属玩家与持有者无关,默认为中立被动玩家.
            // whichItem number item
            // whichPlayer number player
            // changeColor boolean
            /// @CSharpLua.Template = "Jass.Common["SetItemPlayer"]({0}, {1}, {2})"
            public static extern void SetItemPlayer(float whichItem, int whichPlayer, bool changeColor);

            // 移动物品到坐标(立即)(指定坐标) [R]
            // 移动[物品]到([X],[Y])
            // i number item
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["SetItemPosition"]({0}, {1}, {2})"
            public static extern void SetItemPosition(float i, float x, float y);

            // 限制物品种类(指定市场)
            // 限制[Marketplace]的可出售物品种类数为[Quantity]
            // 只影响有'出售物品'技能的单位.
            // whichUnit number unit
            // slots number int
            /// @CSharpLua.Template = "Jass.Common["SetItemTypeSlots"]({0}, {1})"
            public static extern void SetItemTypeSlots(int whichUnit, float slots);

            // 设置物品自定义值
            // 设置[物品]的自定义值为[Index]
            // 物品自定义值只用于触发器. 可以用来为物品绑定一个整型数据.
            // whichItem number item
            // data number int
            /// @CSharpLua.Template = "Jass.Common["SetItemUserData"]({0}, {1})"
            public static extern void SetItemUserData(float whichItem, float data);

            // 显示/隐藏 [R]
            // 设置[物品]的状态为:[显示/隐藏]
            // 只对在地面的物品有效,不会影响在物品栏中的物品. 单位通过触发得到一个隐藏物品时,会自动显示该物品.
            // whichItem number item
            // show boolean
            /// @CSharpLua.Template = "Jass.Common["SetItemVisible"]({0}, {1})"
            public static extern void SetItemVisible(float whichItem, bool show);

            // 改变闪电效果颜色
            // 改变[某个闪电效果]的颜色值为([Red][Green][Blue]) Alpha通道值为[Alpha]
            // 颜色格式为(红,绿,蓝). 颜色和Alpha通道值取值范围0-1. Alpha通道值为0即完全透明.
            // whichBolt number lightning
            // r number
            // g number
            // b number
            // a number
            /// @CSharpLua.Template = "Jass.Common["SetLightningColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern bool SetLightningColor(float whichBolt, float r, float g, float b, float a);

            // description string
            /// @CSharpLua.Template = "Jass.Common["SetMapDescription"]({0})"
            public static extern void SetMapDescription(string description);

            // 设置地图参数
            // 设置[Map Flag]为[On/Off]
            // whichMapFlag any mapflag
            // value boolean
            /// @CSharpLua.Template = "Jass.Common["SetMapFlag"]({0}, {1})"
            public static extern void SetMapFlag(object whichMapFlag, bool value);

            // 设置背景音乐列表 [R]
            // 设置背景音乐列表为:[Music],[允许/禁止]随机播放, 开始播放序号为[Index]
            // 可指定播放文件或播放目录.
            // musicName string
            // random boolean
            // index number int
            /// @CSharpLua.Template = "Jass.Common["SetMapMusic"]({0}, {1}, {2})"
            public static extern void SetMapMusic(string musicName, bool random, float index);

            // name string
            /// @CSharpLua.Template = "Jass.Common["SetMapName"]({0})"
            public static extern void SetMapName(string name);

            // 允许/禁止 关卡
            // [Enable/Disable][Mission]
            // campaignNumber number int
            // missionNumber number int
            // available boolean
            /// @CSharpLua.Template = "Jass.Common["SetMissionAvailable"]({0}, {1}, {2})"
            public static extern void SetMissionAvailable(float campaignNumber, float missionNumber, bool available);

            // 设置背景音乐播放时间点 [R]
            // 设置当前背景音乐的播放时间点为[Offset]毫秒
            // millisecs number int
            /// @CSharpLua.Template = "Jass.Common["SetMusicPlayPosition"]({0})"
            public static extern void SetMusicPlayPosition(float millisecs);

            // 设置背景音乐音量 [R]
            // 设置背景音乐音量为[Volume]
            // 音量取值范围为0-127.
            // volume number int
            /// @CSharpLua.Template = "Jass.Common["SetMusicVolume"]({0})"
            public static extern void SetMusicVolume(float volume);

            // campaignNumber number int
            // available boolean
            /// @CSharpLua.Template = "Jass.Common["SetOpCinematicAvailable"]({0}, {1})"
            public static extern void SetOpCinematicAvailable(float campaignNumber, bool available);

            // 允许/禁用 技能 [R]
            // 设置[某个玩家]的[技能]为[Enable/Disable]
            // 设置玩家能否使用该技能.
            // whichPlayer number player
            // abilid number int
            // avail boolean
            /// @CSharpLua.Template = "Jass.Common["SetPlayerAbilityAvailable"]({0}, {1}, {2})"
            public static extern void SetPlayerAbilityAvailable(int whichPlayer, float abilid, bool avail);

            // 设置联盟状态(指定项目) [R]
            // 命令[某个玩家]对[某个玩家]设置[Alliance Type][On/Off]
            // 注意:可以对玩家自己设置联盟状态. 可用来实现一些特殊效果.
            // sourcePlayer number player
            // otherPlayer number player
            // whichAllianceSetting any alliancetype
            // value boolean
            /// @CSharpLua.Template = "Jass.Common["SetPlayerAlliance"]({0}, {1}, {2}, {3})"
            public static extern void SetPlayerAlliance(float sourcePlayer, float otherPlayer, object whichAllianceSetting, bool value);

            // 改变玩家颜色 [R]
            // 将[某个玩家]的玩家颜色改为[Color]
            // 不改变现有单位的颜色.
            // whichPlayer number player
            // color any playercolor
            /// @CSharpLua.Template = "Jass.Common["SetPlayerColor"]({0}, {1})"
            public static extern void SetPlayerColor(int whichPlayer, object color);

            // whichPlayer number player
            // controlType any mapcontrol
            /// @CSharpLua.Template = "Jass.Common["SetPlayerController"]({0}, {1})"
            public static extern void SetPlayerController(int whichPlayer, object controlType);

            // 设置生命上限 [R]
            // 设置[某个玩家]的生命障碍为正常的[Percent]倍
            // 生命上限影响玩家拥有单位的生命最大值. 生命之书并不受生命上限限制,所以对英雄血量可能会有偏差.
            // whichPlayer number player
            // handicap number
            /// @CSharpLua.Template = "Jass.Common["SetPlayerHandicap"]({0}, {1})"
            public static extern void SetPlayerHandicap(int whichPlayer, float handicap);

            // 设置经验获得率 [R]
            // 设置[某个玩家]的经验获得率为正常的[Value]倍
            // whichPlayer number player
            // handicap number
            /// @CSharpLua.Template = "Jass.Common["SetPlayerHandicapXP"]({0}, {1})"
            public static extern void SetPlayerHandicapXP(int whichPlayer, float handicap);

            // 更改名字
            // 更改[某个玩家]的名字为[文字]
            // whichPlayer number player
            // name string
            /// @CSharpLua.Template = "Jass.Common["SetPlayerName"]({0}, {1})"
            public static extern void SetPlayerName(int whichPlayer, string name);

            // 显示/隐藏计分屏显示 [R]
            // 设置[某个玩家][显示/隐藏]在计分屏的显示.
            // whichPlayer number player
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetPlayerOnScoreScreen"]({0}, {1})"
            public static extern void SetPlayerOnScoreScreen(int whichPlayer, bool flag);

            // whichPlayer number player
            // whichRacePreference any racepreference
            /// @CSharpLua.Template = "Jass.Common["SetPlayerRacePreference"]({0}, {1})"
            public static extern void SetPlayerRacePreference(int whichPlayer, object whichRacePreference);

            // whichPlayer number player
            // value boolean
            /// @CSharpLua.Template = "Jass.Common["SetPlayerRaceSelectable"]({0}, {1})"
            public static extern void SetPlayerRaceSelectable(int whichPlayer, bool value);

            // whichPlayer number player
            // startLocIndex number int
            /// @CSharpLua.Template = "Jass.Common["SetPlayerStartLocation"]({0}, {1})"
            public static extern void SetPlayerStartLocation(int whichPlayer, float startLocIndex);

            // 设置属性
            // 设置[某个玩家]的[Property]为[Value]
            // whichPlayer number player
            // whichPlayerState any playerstate
            // value number int
            /// @CSharpLua.Template = "Jass.Common["SetPlayerState"]({0}, {1}, {2})"
            public static extern void SetPlayerState(int whichPlayer, object whichPlayerState, float value);

            // 设置税率 [R]
            // 设置[某个玩家]交纳给[某个玩家]的[Resource]所得税为[Rate]%
            // 缴纳所得税所损失的资源可以通过'玩家得分'的'税务损失的黄金/木材'来获取. 所得税最高为100%. 且玩家1对玩家2和玩家3都交纳80%所得税.则玩家1采集黄金时将给玩家2 8黄金,玩家3 2黄金.
            // sourcePlayer number player
            // otherPlayer number player
            // whichResource any playerstate
            // rate number int
            /// @CSharpLua.Template = "Jass.Common["SetPlayerTaxRate"]({0}, {1}, {2}, {3})"
            public static extern void SetPlayerTaxRate(float sourcePlayer, float otherPlayer, object whichResource, float rate);

            // 设置玩家队伍
            // 设置[某个玩家]的队伍为[队伍ID]
            // whichPlayer number player
            // whichTeam number int
            /// @CSharpLua.Template = "Jass.Common["SetPlayerTeam"]({0}, {1})"
            public static extern void SetPlayerTeam(int whichPlayer, float whichTeam);

            // whichPlayer number player
            // techid number int
            // maximum number int
            /// @CSharpLua.Template = "Jass.Common["SetPlayerTechMaxAllowed"]({0}, {1}, {2})"
            public static extern void SetPlayerTechMaxAllowed(int whichPlayer, float techid, float maximum);

            // whichPlayer number player
            // techid number int
            // setToLevel number int
            /// @CSharpLua.Template = "Jass.Common["SetPlayerTechResearched"]({0}, {1}, {2})"
            public static extern void SetPlayerTechResearched(int whichPlayer, float techid, float setToLevel);

            // whichPlayer number player
            // newOwner number int
            /// @CSharpLua.Template = "Jass.Common["SetPlayerUnitsOwner"]({0}, {1})"
            public static extern void SetPlayerUnitsOwner(int whichPlayer, float newOwner);

            // playercount number int
            /// @CSharpLua.Template = "Jass.Common["SetPlayers"]({0})"
            public static extern void SetPlayers(float playercount);

            // 设置随机种子
            // 设置随机种子数为：[整数]
            // 设置游戏的随机种子，随机种子会影响随机整数，攻击骰子之类的随机数。
            // seed number int
            /// @CSharpLua.Template = "Jass.Common["SetRandomSeed"]({0})"
            public static extern void SetRandomSeed(float seed);

            // 设置矩形区域(指定坐标) [R]
            // 重新设置[矩形区域],左下角坐标为([X],[Y]), 右上角坐标为([X],[Y])
            // 该区域必须是一个变量. 重新设置矩形区域的大小和位置.
            // whichRect number rect
            // minx number
            // miny number
            // maxx number
            // maxy number
            /// @CSharpLua.Template = "Jass.Common["SetRect"]({0}, {1}, {2}, {3}, {4})"
            public static extern void SetRect(float whichRect, float minx, float miny, float maxx, float maxy);

            // 设置矩形区域(指定点) [R]
            // 重新设置[矩形区域],左下角点为[坐标]右上角点为[坐标]
            // 该区域必须是一个变量. 重新设置矩形区域的大小和位置.
            // whichRect number rect
            // min number location
            // max number location
            /// @CSharpLua.Template = "Jass.Common["SetRectFromLoc"]({0}, {1}, {2})"
            public static extern void SetRectFromLoc(float whichRect, float min, float max);

            // 保留英雄图标
            // 为玩家保留[Number]个左上角英雄图标.
            // 因为共享单位而被控制的其他玩家英雄的图标将在保留位置之后开始显示.
            // reserved number int
            /// @CSharpLua.Template = "Jass.Common["SetReservedLocalHeroButtons"]({0})"
            public static extern void SetReservedLocalHeroButtons(float reserved);

            // 设置黄金储量
            // 设置[金矿]的黄金储量为[Quantity]
            // whichUnit number unit
            // amount number int
            /// @CSharpLua.Template = "Jass.Common["SetResourceAmount"]({0}, {1})"
            public static extern void SetResourceAmount(int whichUnit, float amount);

            // whichdensity any mapdensity
            /// @CSharpLua.Template = "Jass.Common["SetResourceDensity"]({0})"
            public static extern void SetResourceDensity(object whichdensity);

            // 设置天空
            // 设置天空模型为[Sky]
            // skyModelFile string
            /// @CSharpLua.Template = "Jass.Common["SetSkyModel"]({0})"
            public static extern void SetSkyModel(string skyModelFile);

            // soundHandle number sound
            // channel number int
            /// @CSharpLua.Template = "Jass.Common["SetSoundChannel"]({0}, {1})"
            public static extern void SetSoundChannel(float soundHandle, float channel);

            // soundHandle number sound
            // inside number
            // outside number
            // outsideVolume number int
            /// @CSharpLua.Template = "Jass.Common["SetSoundConeAngles"]({0}, {1}, {2}, {3})"
            public static extern void SetSoundConeAngles(float soundHandle, float inside, float outside, float outsideVolume);

            // soundHandle number sound
            // x number
            // y number
            // z number
            /// @CSharpLua.Template = "Jass.Common["SetSoundConeOrientation"]({0}, {1}, {2}, {3})"
            public static extern void SetSoundConeOrientation(float soundHandle, float x, float y, float z);

            // 设置声音截断距离
            // 设置[音效]的截断距离为[数值]
            // 地图距离,玩家镜头距离音源超过该范围则切断声音.
            // soundHandle number sound
            // cutoff number
            /// @CSharpLua.Template = "Jass.Common["SetSoundDistanceCutoff"]({0}, {1})"
            public static extern void SetSoundDistanceCutoff(float soundHandle, float cutoff);

            // 设置3D音效衰减范围
            // 设置[3D音效]的衰减最小范围:[数值]最大范围:[数值]
            // 该动作仅用于3D音效. 注意不一定要达到最大范围,音量衰减到一定程度也会变没的.
            // soundHandle number sound
            // minDist number
            // maxDist number
            /// @CSharpLua.Template = "Jass.Common["SetSoundDistances"]({0}, {1}, {2})"
            public static extern void SetSoundDistances(float soundHandle, float minDist, float maxDist);

            // soundHandle number sound
            // duration number int
            /// @CSharpLua.Template = "Jass.Common["SetSoundDuration"]({0}, {1})"
            public static extern void SetSoundDuration(float soundHandle, float duration);

            // soundHandle number sound
            // soundLabel string
            /// @CSharpLua.Template = "Jass.Common["SetSoundParamsFromLabel"]({0}, {1})"
            public static extern void SetSoundParamsFromLabel(float soundHandle, string soundLabel);

            // 设置声音速率
            // 设置[音效]的速率为[数值]
            // 表示正常速率的倍数.
            // soundHandle number sound
            // pitch number
            /// @CSharpLua.Template = "Jass.Common["SetSoundPitch"]({0}, {1})"
            public static extern void SetSoundPitch(float soundHandle, float pitch);

            // 设置音效播放时间点 [R]
            // 设置[音效]的播放时间点为[Offset]毫秒
            // 音效必须是正在播放的. 不能用于3D音效.
            // soundHandle number sound
            // millisecs number int
            /// @CSharpLua.Template = "Jass.Common["SetSoundPlayPosition"]({0}, {1})"
            public static extern void SetSoundPlayPosition(float soundHandle, float millisecs);

            // 设置3D音效位置(指定坐标) [R]
            // 设置[3D音效]的播放位置为([X],[Y]), Z轴高度为[Z]
            // 该动作仅用于3D音效.
            // soundHandle number sound
            // x number
            // y number
            // z number
            /// @CSharpLua.Template = "Jass.Common["SetSoundPosition"]({0}, {1}, {2}, {3})"
            public static extern void SetSoundPosition(float soundHandle, float x, float y, float z);

            // soundHandle number sound
            // x number
            // y number
            // z number
            /// @CSharpLua.Template = "Jass.Common["SetSoundVelocity"]({0}, {1}, {2}, {3})"
            public static extern void SetSoundVelocity(float soundHandle, float x, float y, float z);

            // 设置音效音量 [R]
            // 设置[音效]的音量为[Volume]
            // 音量取值范围0-127.
            // soundHandle number sound
            // volume number int
            /// @CSharpLua.Template = "Jass.Common["SetSoundVolume"]({0}, {1})"
            public static extern void SetSoundVolume(float soundHandle, float volume);

            // whichStartLoc number int
            // prioSlotIndex number int
            // otherStartLocIndex number int
            // priority any startlocprio
            /// @CSharpLua.Template = "Jass.Common["SetStartLocPrio"]({0}, {1}, {2}, {3})"
            public static extern void SetStartLocPrio(float whichStartLoc, float prioSlotIndex, float otherStartLocIndex, object priority);

            // whichStartLoc number int
            // prioSlotCount number int
            /// @CSharpLua.Template = "Jass.Common["SetStartLocPrioCount"]({0}, {1})"
            public static extern void SetStartLocPrioCount(float whichStartLoc, float prioSlotCount);

            // teamcount number int
            /// @CSharpLua.Template = "Jass.Common["SetTeams"]({0})"
            public static extern void SetTeams(float teamcount);

            // a number
            // b number
            // c number
            // d number
            // e number
            /// @CSharpLua.Template = "Jass.Common["SetTerrainFog"]({0}, {1}, {2}, {3}, {4})"
            public static extern void SetTerrainFog(float a, float b, float c, float d, float e);

            // 设置迷雾 [R]
            // 迷雾风格:[Style], Z轴开始端:[Z-Start], Z轴结束端:[Z-End], 密度:[Density]颜色:([Red],[Green],[Blue])
            // 颜色格式为(红,绿,蓝). 取值范围0.00-1.00.
            // style number int
            // zstart number
            // zend number
            // density number
            // red number
            // green number
            // blue number
            /// @CSharpLua.Template = "Jass.Common["SetTerrainFogEx"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern void SetTerrainFogEx(float style, float zstart, float zend, float density, float red, float green, float blue);

            // 设置地形通行状态(指定坐标) [R]
            // 设置([X],[Y])处单元点的[Pathing]地形通行状态为:[On/Off]
            // 例:设置'建造'通行状态为开,则该点可以建造建筑. 一个单元点范围为32x32.
            // x number
            // y number
            // t any pathingtype
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetTerrainPathable"]({0}, {1}, {2}, {3})"
            public static extern void SetTerrainPathable(float x, float y, object t, bool flag);

            // 改变地形类型(指定坐标) [R]
            // 改变([X],[Y])处的地形为[Terrain Type],使用样式:[Variation]范围:[Area]形状:[Shape]
            // 地形样式-1表示随机样式. 范围即地形编辑器中的刷子大小.1表示128x128范围
            // x number
            // y number
            // terrainType number int
            // variation number int
            // area number int
            // shape number int
            /// @CSharpLua.Template = "Jass.Common["SetTerrainType"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern void SetTerrainType(float x, float y, float terrainType, int variation, float area, float shape);

            // 设置已存在时间
            // 设置[Floating Text]的已存在时间为[Time]秒
            // 该动作并不影响永久性漂浮文字.
            // t number texttag
            // age number
            /// @CSharpLua.Template = "Jass.Common["SetTextTagAge"]({0}, {1})"
            public static extern void SetTextTagAge(float t, float age);

            // 改变颜色 [R]
            // 改变[Floating Text]的颜色为([Red],[Green],[Blue]) 透明值为[Alpha]
            // 颜色格式为(红,绿,蓝). 透明值0为不可见. 颜色值和透明值取值范围为0-255.
            // t number texttag
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["SetTextTagColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void SetTextTagColor(float t, float red, float green, float blue, float alpha);

            // 设置消逝时间点
            // 设置[Floating Text]的消逝时间点为[Time]秒
            // 该动作并不影响永久性漂浮文字. 当漂浮文字存在时间到达该值时会开始淡化消逝.
            // t number texttag
            // fadepoint number
            /// @CSharpLua.Template = "Jass.Common["SetTextTagFadepoint"]({0}, {1})"
            public static extern void SetTextTagFadepoint(float t, float fadepoint);

            // 设置显示时间
            // 设置[Floating Text]的显示时间为[Time]秒
            // 该动作并不影响永久性漂浮文字. 当显示时间到期时,系统会自动清除该漂浮文字.
            // t number texttag
            // lifespan number
            /// @CSharpLua.Template = "Jass.Common["SetTextTagLifespan"]({0}, {1})"
            public static extern void SetTextTagLifespan(float t, float lifespan);

            // 设置永久显示
            // 设置[Floating Text]:[Enable/Disable]永久显示.
            // t number texttag
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetTextTagPermanent"]({0}, {1})"
            public static extern void SetTextTagPermanent(float t, bool flag);

            // 改变位置(坐标) [R]
            // 改变[Floating Text]的位置为([X],[Y]) ,Z轴高度为[Z]
            // t number texttag
            // x number
            // y number
            // heightOffset number
            /// @CSharpLua.Template = "Jass.Common["SetTextTagPos"]({0}, {1}, {2}, {3})"
            public static extern void SetTextTagPos(float t, float x, float y, float heightOffset);

            // 改变位置(单位)
            // 改变[Floating Text]的位置到[单位]头顶Z轴偏移[Z]处
            // t number texttag
            // whichUnit number unit
            // heightOffset number
            /// @CSharpLua.Template = "Jass.Common["SetTextTagPosUnit"]({0}, {1}, {2})"
            public static extern void SetTextTagPosUnit(float t, int whichUnit, float heightOffset);

            // 暂停/恢复
            // 设置[Floating Text]:[Enable/Disable]暂停状态
            // 暂停状态暂停漂浮文字的移动和生命计时.
            // t number texttag
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetTextTagSuspended"]({0}, {1})"
            public static extern void SetTextTagSuspended(float t, bool flag);

            // 改变文字内容 [R]
            // 改变[Floating Text]的内容为[文字],字体大小:[Size]
            // 采用原始字体大小单位. 字体大小不能超过0.5.
            // t number texttag
            // s string
            // height number
            /// @CSharpLua.Template = "Jass.Common["SetTextTagText"]({0}, {1}, {2})"
            public static extern void SetTextTagText(float t, string s, float height);

            // 设置速率 [R]
            // 设置[Floating Text]的X轴速率:[XSpeed],Y轴速率:[YSpeed]
            // 对移动后的漂浮文字设置速率,该漂浮文字会先回到原点再向设定的角度移动. 这里的1约等于游戏中的1800速度.
            // t number texttag
            // xvel number
            // yvel number
            /// @CSharpLua.Template = "Jass.Common["SetTextTagVelocity"]({0}, {1}, {2})"
            public static extern void SetTextTagVelocity(float t, float xvel, float yvel);

            // 显示/隐藏 (所有玩家) [R]
            // 对所有玩家[显示/隐藏][Floating Text]
            // t number texttag
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetTextTagVisibility"]({0}, {1})"
            public static extern void SetTextTagVisibility(float t, bool flag);

            // 设置主题音乐播放时间点 [R]
            // 设置当前主题音乐播放时间点为[Offset]毫秒
            // millisecs number int
            /// @CSharpLua.Template = "Jass.Common["SetThematicMusicPlayPosition"]({0})"
            public static extern void SetThematicMusicPlayPosition(float millisecs);

            // 设置昼夜时间流逝速度 [R]
            // 设置昼夜时间流逝速度为默认值的[Value]倍
            // 设置100%来恢复正常值. 该值并不影响游戏速度.
            // r number
            /// @CSharpLua.Template = "Jass.Common["SetTimeOfDayScale"]({0})"
            public static extern void SetTimeOfDayScale(float r);

            // cleared boolean
            /// @CSharpLua.Template = "Jass.Common["SetTutorialCleared"]({0})"
            public static extern void SetTutorialCleared(bool cleared);

            // 设置渲染状态
            // 设置[某个地形纹理]:[Enable/Disable]渲染状态
            // 未发现有任何作用.
            // whichSplat any ubersplat
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetUbersplatRender"]({0}, {1})"
            public static extern void SetUbersplatRender(object whichSplat, bool flag);

            // 设置永久渲染状态
            // 设置[某个地形纹理]:[Enable/Disable]永久渲染状态
            // 要显示地面纹理变化则必须开启该项.
            // whichSplat any ubersplat
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetUbersplatRenderAlways"]({0}, {1})"
            public static extern void SetUbersplatRenderAlways(object whichSplat, bool flag);

            // 设置技能等级 [R]
            // 设置[单位]的[技能]等级为[Level]
            // 改变死亡单位的光环技能会导致魔兽崩溃.
            // whichUnit number unit
            // abilcode number int
            // level number int
            /// @CSharpLua.Template = "Jass.Common["SetUnitAbilityLevel"]({0}, {1}, {2})"
            public static extern float SetUnitAbilityLevel(int whichUnit, float abilcode, float level);

            // 设置主动攻击范围
            // 设置[单位]的主动攻击范围为[数值]
            // whichUnit number unit
            // newAcquireRange number
            /// @CSharpLua.Template = "Jass.Common["SetUnitAcquireRange"]({0}, {1})"
            public static extern void SetUnitAcquireRange(int whichUnit, float newAcquireRange);

            // 播放单位动画
            // 播放[Unit]的[动画名]动作
            // 通过 '重置单位动作' 恢复到普通的动作.
            // whichUnit number unit
            // whichAnimation string
            /// @CSharpLua.Template = "Jass.Common["SetUnitAnimation"]({0}, {1})"
            public static extern void SetUnitAnimation(int whichUnit, string whichAnimation);

            // 播放单位指定序号动动作 [R]
            // 播放[单位]的第[序号]号动作
            // 可以指定播放所有的单位动画,不过需要自己多尝试.每个单位的动作序号不一样的.
            // whichUnit number unit
            // whichAnimation number int
            /// @CSharpLua.Template = "Jass.Common["SetUnitAnimationByIndex"]({0}, {1})"
            public static extern void SetUnitAnimationByIndex(int whichUnit, float whichAnimation);

            // 播放单位动运作(指定概率)
            // 播放[单位]的[Animation Name]动作,只用[Rarity]动作
            // 通过 '重置单位动作' 恢复到普通的动作.
            // whichUnit number unit
            // whichAnimation string
            // rarity any raritycontrol
            /// @CSharpLua.Template = "Jass.Common["SetUnitAnimationWithRarity"]({0}, {1}, {2})"
            public static extern void SetUnitAnimationWithRarity(int whichUnit, string whichAnimation, object rarity);

            // 改变单位混合时间
            // 改变[单位]的混合时间为[数值]
            // 单位动画图像混合时间. 决定身体部件连接的快慢,比如攻击时手臂挥舞的速度. 默认值0.15,增大该值会导致动作僵硬化.
            // whichUnit number unit
            // blendTime number
            /// @CSharpLua.Template = "Jass.Common["SetUnitBlendTime"]({0}, {1})"
            public static extern void SetUnitBlendTime(int whichUnit, float blendTime);

            // 改变队伍颜色
            // 改变[单位]的队伍颜色为[Color]
            // 改变队伍颜色并不会改变单位所属.
            // whichUnit number unit
            // whichColor any playercolor
            /// @CSharpLua.Template = "Jass.Common["SetUnitColor"]({0}, {1})"
            public static extern void SetUnitColor(int whichUnit, object whichColor);

            // 锁定指定单位的警戒点 [R]
            // 设置[单位]的警戒点:[option]
            // 锁定并防止 AI 脚本改动单位警戒点.
            // whichUnit number unit
            // creepGuard boolean
            /// @CSharpLua.Template = "Jass.Common["SetUnitCreepGuard"]({0}, {1})"
            public static extern void SetUnitCreepGuard(int whichUnit, bool creepGuard);

            // 设置死亡方式
            // 设置[单位][Explode/Die Normally]在死亡时
            // whichUnit number unit
            // exploded boolean
            /// @CSharpLua.Template = "Jass.Common["SetUnitExploded"]({0}, {1})"
            public static extern void SetUnitExploded(int whichUnit, bool exploded);

            // 设置单位面向角度 [R]
            // 设置[单位]的面向角度为[Angle]度
            // 面向角度采用角度制,0度为正东方向,90度为正北方向。速度等于单位的转身速度。
            // whichUnit number unit
            // facingAngle number
            /// @CSharpLua.Template = "Jass.Common["SetUnitFacing"]({0}, {1})"
            public static extern void SetUnitFacing(int whichUnit, float facingAngle);

            // 设置单位面向角度(指定时间)
            // 设置[单位]的面向角度为[Angle]度,使用时间[Time]秒
            // 面向角度采用角度制,0度为正东方向,90度为正北方向。不能超过单位的转身速度。
            // whichUnit number unit
            // facingAngle number
            // duration number
            /// @CSharpLua.Template = "Jass.Common["SetUnitFacingTimed"]({0}, {1}, {2})"
            public static extern void SetUnitFacingTimed(int whichUnit, float facingAngle, float duration);

            // 改变单位飞行高度
            // 改变[单位]的飞行高度为[数值],变换速率:[数值]
            // 飞行单位可以直接改变飞行高度. 其他单位通过添加/删除 替换为飞行单位的变身技能(如乌鸦形态)之后,也能改变飞行高度.
            // whichUnit number unit
            // newHeight number
            // rate number
            /// @CSharpLua.Template = "Jass.Common["SetUnitFlyHeight"]({0}, {1}, {2})"
            public static extern void SetUnitFlyHeight(int whichUnit, float newHeight, float rate);

            // a number
            // b number
            // c number
            // d number
            // e number
            /// @CSharpLua.Template = "Jass.Common["SetUnitFog"]({0}, {1}, {2}, {3}, {4})"
            public static extern void SetUnitFog(float a, float b, float c, float d, float e);

            // 设置无敌/可攻击
            // 设置[单位][Invulnerable/Vulnerable]
            // whichUnit number unit
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetUnitInvulnerable"]({0}, {1})"
            public static extern void SetUnitInvulnerable(int whichUnit, bool flag);

            // 锁定身体朝向
            // 锁定[单位]的[Source]朝向[目标单位],偏移坐标 ([X],[Y],[Z])
            // 单位的该身体部件会一直朝向目标单位的偏移坐标点处,直到使用'重置身体朝向'. 坐标偏移以目标单位脚下为坐标原点.
            // whichUnit number unit
            // whichBone string
            // lookAtTarget number unit
            // offsetX number
            // offsetY number
            // offsetZ number
            /// @CSharpLua.Template = "Jass.Common["SetUnitLookAt"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern void SetUnitLookAt(int whichUnit, string whichBone, float lookAtTarget, float offsetX, float offsetY, float offsetZ);

            // 设置移动速度
            // 设置[单位]的移动速度为[Speed]
            // whichUnit number unit
            // newSpeed number
            /// @CSharpLua.Template = "Jass.Common["SetUnitMoveSpeed"]({0}, {1})"
            public static extern void SetUnitMoveSpeed(int whichUnit, float newSpeed);

            // 改变所属
            // 改变[单位]所属为[某个玩家]并[Change/Retain Color]
            // whichUnit number unit
            // whichPlayer number player
            // changeColor boolean
            /// @CSharpLua.Template = "Jass.Common["SetUnitOwner"]({0}, {1}, {2})"
            public static extern void SetUnitOwner(int whichUnit, int whichPlayer, bool changeColor);

            // 设置碰撞开关
            // 设置[单位][On/Off]碰撞
            // 关闭碰撞的单位无视障碍物,但其他单位仍视其为障碍物.
            // whichUnit number unit
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetUnitPathing"]({0}, {1})"
            public static extern void SetUnitPathing(int whichUnit, bool flag);

            // 移动单位(立即)(指定坐标) [R]
            // 立即移动[单位]到([X],[Y])
            // whichUnit number unit
            // newX number
            // newY number
            /// @CSharpLua.Template = "Jass.Common["SetUnitPosition"]({0}, {1}, {2})"
            public static extern void SetUnitPosition(int whichUnit, float newX, float newY);

            // 移动单位(立即)(指定点)
            // 立即移动[单位]到[指定点]
            // whichUnit number unit
            // whichLocation number location
            /// @CSharpLua.Template = "Jass.Common["SetUnitPositionLoc"]({0}, {1})"
            public static extern void SetUnitPositionLoc(int whichUnit, float whichLocation);

            // 改变单位转向角度(弧度制) [R]
            // 改变[单位]的转向角度为[数值](弧度制)
            // 设置单位转身时的转向角度. 数值越大转向幅度越大.
            // whichUnit number unit
            // newPropWindowAngle number
            /// @CSharpLua.Template = "Jass.Common["SetUnitPropWindow"]({0}, {1})"
            public static extern void SetUnitPropWindow(int whichUnit, float newPropWindowAngle);

            // 设置可否营救(对玩家) [R]
            // 设置[单位]对[玩家][Rescuable/Unrescuable]
            // whichUnit number unit
            // byWhichPlayer number player
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SetUnitRescuable"]({0}, {1}, {2})"
            public static extern void SetUnitRescuable(int whichUnit, float byWhichPlayer, bool flag);

            // 设置营救范围
            // 设置[单位]的营救范围为[Range]
            // whichUnit number unit
            // range number
            /// @CSharpLua.Template = "Jass.Common["SetUnitRescueRange"]({0}, {1})"
            public static extern void SetUnitRescueRange(int whichUnit, float range);

            // 改变单位尺寸(按倍数) [R]
            // 改变[单位]的尺寸缩放为:([X],[Y],[Z])
            // 缩放尺寸使用(长,宽,高)格式.
            // whichUnit number unit
            // scaleX number
            // scaleY number
            // scaleZ number
            /// @CSharpLua.Template = "Jass.Common["SetUnitScale"]({0}, {1}, {2}, {3})"
            public static extern void SetUnitScale(int whichUnit, float scaleX, float scaleY, float scaleZ);

            // 设置单位属性 [R]
            // 设置[单位]的[属性]为[Value]
            // whichUnit number unit
            // whichUnitState any unitstate
            // newVal number
            /// @CSharpLua.Template = "Jass.Common["SetUnitState"]({0}, {1}, {2})"
            public static extern void SetUnitState(int whichUnit, object whichUnitState, float newVal);

            // 改变单位动画播放速度(按倍数) [R]
            // 改变[单位]的动画播放速度为正常速度的[Percent]倍
            // 设置1倍动画播放速度来恢复正常状态.
            // whichUnit number unit
            // timeScale number
            /// @CSharpLua.Template = "Jass.Common["SetUnitTimeScale"]({0}, {1})"
            public static extern void SetUnitTimeScale(int whichUnit, float timeScale);

            // 改变单位转身速度
            // 改变[单位]的转身速度为[Value]
            // 转身速度表示单位改变面向方向时的速度，数值(0-1)越小表示转身越慢，为0则无法转身。
            // whichUnit number unit
            // newTurnSpeed number
            /// @CSharpLua.Template = "Jass.Common["SetUnitTurnSpeed"]({0}, {1})"
            public static extern void SetUnitTurnSpeed(int whichUnit, float newTurnSpeed);

            // 限制单位种类(指定市场)
            // 限制[Marketplace]的可出售单位种类数为[Quantity]
            // 只影响有'出售单位'技能的单位.
            // whichUnit number unit
            // slots number int
            /// @CSharpLua.Template = "Jass.Common["SetUnitTypeSlots"]({0}, {1})"
            public static extern void SetUnitTypeSlots(int whichUnit, float slots);

            // 允许/禁止 人口占用 [R]
            // 设置[单位]:[Enable/Disable]其人口占用
            // whichUnit number unit
            // useFood boolean
            /// @CSharpLua.Template = "Jass.Common["SetUnitUseFood"]({0}, {1})"
            public static extern void SetUnitUseFood(int whichUnit, bool useFood);

            // 设置自定义值
            // 设置[单位]的自定义值为[Index]
            // 单位自定义值仅用于触发器. 可用来给单位绑定一个整型数据.
            // whichUnit number unit
            // data number int
            /// @CSharpLua.Template = "Jass.Common["SetUnitUserData"]({0}, {1})"
            public static extern void SetUnitUserData(int whichUnit, float data);

            // 改变单位的颜色(RGB:0-255) [R]
            // 改变[单位]的颜色值: ([Red],[Green],[Blue]), 透明值:[Alpha]
            // 颜色格式为(红,绿,蓝). 大多数单位使用(255,255,255)的颜色值和255的Alpha值. 透明值为0是不可见的.颜色值和Alpha值取值范围为0-255.
            // whichUnit number unit
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["SetUnitVertexColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void SetUnitVertexColor(int whichUnit, float red, float green, float blue, float alpha);

            // 设置X坐标 [R]
            // 设置[单位]的X坐标为[X]
            // 注意如果坐标超出地图边界是会出错的.
            // whichUnit number unit
            // newX number
            /// @CSharpLua.Template = "Jass.Common["SetUnitX"]({0}, {1})"
            public static extern void SetUnitX(int whichUnit, float newX);

            // 设置Y坐标 [R]
            // 设置[单位]的Y坐标为[Y]
            // 注意如果坐标超出地图边界是会出错的.
            // whichUnit number unit
            // newY number
            /// @CSharpLua.Template = "Jass.Common["SetUnitY"]({0}, {1})"
            public static extern void SetUnitY(int whichUnit, float newY);

            // 设置水颜色 [R]
            // 设置水颜色为:([Red],[Green],[Blue]), 透明值为:[Alpha]
            // 颜色格式为(红,绿,蓝). 透明值0为不可见. 颜色值和透明道值取值范围为0-255.
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["SetWaterBaseColor"]({0}, {1}, {2}, {3})"
            public static extern void SetWaterBaseColor(float red, float green, float blue, float alpha);

            // 开启/关闭 水面变形
            // [On/Off]水面变形
            // 开启时当发生地形变化时水面高度也会随着变化. 对永久变形无效.
            // val boolean
            /// @CSharpLua.Template = "Jass.Common["SetWaterDeforms"]({0})"
            public static extern void SetWaterDeforms(bool val);

            // whichWidget number widget
            // newLife number
            /// @CSharpLua.Template = "Jass.Common["SetWidgetLife"]({0}, {1})"
            public static extern void SetWidgetLife(float whichWidget, float newLife);

            // 显示/隐藏 [R]
            // 设置[可破坏物]的状态为[显示/隐藏]
            // 隐藏的可破坏物不被显示,但仍影响通行和视线.
            // d number destructable
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["ShowDestructable"]({0}, {1})"
            public static extern void ShowDestructable(float d, bool flag);

            // 显示/隐藏 [R]
            // 设置[某个图像][显示/隐藏]
            // whichImage number image
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["ShowImage"]({0}, {1})"
            public static extern void ShowImage(float whichImage, bool flag);

            // 开启/关闭 信箱模式(所有玩家) [R]
            // [开启/关闭]信箱模式,转换时间为[Duration]秒
            // 使用电影镜头模式,隐藏游戏界面.
            // flag boolean
            // fadeDuration number
            /// @CSharpLua.Template = "Jass.Common["ShowInterface"]({0}, {1})"
            public static extern void ShowInterface(bool flag, float fadeDuration);

            // 显示/隐藏 地面纹理变化[R]
            // 设置[某个地形纹理]状态为[显示/隐藏]
            // whichSplat any ubersplat
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["ShowUbersplat"]({0}, {1})"
            public static extern void ShowUbersplat(object whichSplat, bool flag);

            // 显示/隐藏 [R]
            // 设置[单位]的状态为[显示/隐藏]
            // 隐藏单位不会被'区域内单位'所选取.
            // whichUnit number unit
            // show boolean
            /// @CSharpLua.Template = "Jass.Common["ShowUnit"]({0}, {1})"
            public static extern void ShowUnit(int whichUnit, bool show);

            // x number
            /// @CSharpLua.Template = "Jass.Common["SquareRoot"]({0})"
            public static extern float SquareRoot(float x);

            // 启用战役AI
            // 为[某个玩家]启用战役AI:[Script]
            // AI只能对电脑玩家使用,当运行该动作后,与之配匹的电脑玩家会强制执行该AI脚本.
            // num number player
            // script string
            /// @CSharpLua.Template = "Jass.Common["StartCampaignAI"]({0}, {1})"
            public static extern void StartCampaignAI(float num, string script);

            // 启用对战AI
            // 为[某个玩家]启用对战AI:[Script]
            // AI只能对电脑玩家使用,当运行该动作后,与之配匹的电脑玩家会强制执行该AI脚本.
            // num number player
            // script string
            /// @CSharpLua.Template = "Jass.Common["StartMeleeAI"]({0}, {1})"
            public static extern void StartMeleeAI(float num, string script);

            // soundHandle number sound
            /// @CSharpLua.Template = "Jass.Common["StartSound"]({0})"
            public static extern void StartSound(float soundHandle);

            // 停止播放镜头(所有玩家) [R]
            // 让所有玩家停止播放镜头
            // 比如在平移镜头的过程中可用该动作来中断平移.
            /// @CSharpLua.Template = "Jass.Common["StopCamera"]()"
            public static extern void StopCamera();

            // 停止背景音乐
            // 停止背景音乐[After Fading/Immediately]
            // fadeOut boolean
            /// @CSharpLua.Template = "Jass.Common["StopMusic"]({0})"
            public static extern void StopMusic(bool fadeOut);

            // 停止音效
            // 停止播放[音效][After Fading/Immediately]
            // soundHandle number sound
            // killWhenDone boolean
            // fadeOut boolean
            /// @CSharpLua.Template = "Jass.Common["StopSound"]({0}, {1}, {2})"
            public static extern void StopSound(float soundHandle, bool killWhenDone, bool fadeOut);

            // 记录布尔值
            // 缓存:[Game Cache] 类别名:[Category]使用名称:[文字]记录:[布尔值]
            // 使用'游戏缓存 - 读取布尔值'来读取该值. 名称和类别名不能包含空格.
            // cache any gamecache
            // missionKey string
            // key string
            // value boolean
            /// @CSharpLua.Template = "Jass.Common["StoreBoolean"]({0}, {1}, {2}, {3})"
            public static extern void StoreBoolean(object cache, string missionKey, string key, bool value);

            // 记录整数
            // 缓存:[Game Cache] 类别名:[Category]使用名称:[文字]记录:[整数]
            // 使用'游戏缓存 - 读取整数'来读取该数值. 名称和类别名不能包含空格.
            // cache any gamecache
            // missionKey string
            // key string
            // value number int
            /// @CSharpLua.Template = "Jass.Common["StoreInteger"]({0}, {1}, {2}, {3})"
            public static extern void StoreInteger(object cache, string missionKey, string key, float value);

            // 记录实数
            // 缓存:[Game Cache] 类别名:[Category]使用名称:[文字]记录:[实数]
            // 使用'游戏缓存 - 读取实数'来读取该数值. 名称和类别名不能包含空格.
            // cache any gamecache
            // missionKey string
            // key string
            // value number
            /// @CSharpLua.Template = "Jass.Common["StoreReal"]({0}, {1}, {2}, {3})"
            public static extern void StoreReal(object cache, string missionKey, string key, float value);

            // 记录字符串
            // 缓存:[Game Cache] 类别名:[Category]使用名称:[文字]记录:[字符串]
            // 使用'游戏缓存 - 读取字符串'来读取该值. 名称和类别名不能包含空格.
            // cache any gamecache
            // missionKey string
            // key string
            // value string
            /// @CSharpLua.Template = "Jass.Common["StoreString"]({0}, {1}, {2}, {3})"
            public static extern bool StoreString(object cache, string missionKey, string key, string value);

            // 记录单位
            // 记录[单位],使用名称:[文字]类别名:[Category]缓存:[Game Cache]
            // 使用'游戏缓存 - 读取单位'来读取该数值. 名称和类别名不能包含空格.
            // cache any gamecache
            // missionKey string
            // key string
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["StoreUnit"]({0}, {1}, {2}, {3})"
            public static extern bool StoreUnit(object cache, string missionKey, string key, int whichUnit);

            // source string
            // upper boolean
            /// @CSharpLua.Template = "Jass.Common["StringCase"]({0}, {1})"
            public static extern string StringCase(string source, bool upper);

            // s string
            /// @CSharpLua.Template = "Jass.Common["StringHash"]({0})"
            public static extern float StringHash(string s);

            // s string
            /// @CSharpLua.Template = "Jass.Common["StringLength"]({0})"
            public static extern float StringLength(string s);

            // source string
            // start number int
            // end number int
            /// @CSharpLua.Template = "Jass.Common["SubString"]({0}, {1}, {2})"
            public static extern string SubString(string source, float start, float end);

            // 允许/禁止经验获取 [R]
            // [Enable/Disable][某个英雄]的经验获取
            // whichHero number unit
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["SuspendHeroXP"]({0}, {1})"
            public static extern void SuspendHeroXP(float whichHero, bool flag);

            // b boolean
            /// @CSharpLua.Template = "Jass.Common["SuspendTimeOfDay"]({0})"
            public static extern void SuspendTimeOfDay(bool b);

            /// @CSharpLua.Template = "Jass.Common["SyncSelections"]()"
            public static extern void SyncSelections();

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["SyncStoredBoolean"]({0}, {1}, {2})"
            public static extern void SyncStoredBoolean(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["SyncStoredInteger"]({0}, {1}, {2})"
            public static extern void SyncStoredInteger(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["SyncStoredReal"]({0}, {1}, {2})"
            public static extern void SyncStoredReal(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["SyncStoredString"]({0}, {1}, {2})"
            public static extern void SyncStoredString(object cache, string missionKey, string key);

            // cache any gamecache
            // missionKey string
            // key string
            /// @CSharpLua.Template = "Jass.Common["SyncStoredUnit"]({0}, {1}, {2})"
            public static extern void SyncStoredUnit(object cache, string missionKey, string key);

            // x number
            // y number
            // radius number
            // depth number
            // duration number int
            // permanent boolean
            /// @CSharpLua.Template = "Jass.Common["TerrainDeformCrater"]({0}, {1}, {2}, {3}, {4}, {5})"
            public static extern object TerrainDeformCrater(float x, float y, float radius, float depth, float duration, bool permanent);

            // x number
            // y number
            // radius number
            // minDelta number
            // maxDelta number
            // duration number int
            // updateInterval number int
            /// @CSharpLua.Template = "Jass.Common["TerrainDeformRandom"]({0}, {1}, {2}, {3}, {4}, {5}, {6})"
            public static extern object TerrainDeformRandom(float x, float y, float radius, float minDelta, float maxDelta, float duration, float updateInterval);

            // x number
            // y number
            // radius number
            // depth number
            // duration number int
            // count number int
            // spaceWaves number
            // timeWaves number
            // radiusStartPct number
            // limitNeg boolean
            /// @CSharpLua.Template = "Jass.Common["TerrainDeformRipple"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})"
            public static extern object TerrainDeformRipple(float x, float y, float radius, float depth, float duration, float count, float spaceWaves, float timeWaves, float radiusStartPct, bool limitNeg);

            // 停止地形变化 [R]
            // 停止[Terrain Deformation],衰退时间:[Duration]毫秒
            // 地形变化会平滑地过渡到无.
            // deformation any terraindeformation
            // duration number int
            /// @CSharpLua.Template = "Jass.Common["TerrainDeformStop"]({0}, {1})"
            public static extern void TerrainDeformStop(object deformation, float duration);

            // 停止所有地形变化
            // 包括由技能引起的地形变化.
            /// @CSharpLua.Template = "Jass.Common["TerrainDeformStopAll"]()"
            public static extern void TerrainDeformStopAll();

            // x number
            // y number
            // dirX number
            // dirY number
            // distance number
            // speed number
            // radius number
            // depth number
            // trailTime number int
            // count number int
            /// @CSharpLua.Template = "Jass.Common["TerrainDeformWave"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})"
            public static extern object TerrainDeformWave(float x, float y, float dirX, float dirY, float distance, float speed, float radius, float depth, float trailTime, float count);

            // 显示/隐藏 计时器窗口(所有玩家) [R]
            // 设置[计时器窗口]的状态为[显示/隐藏]
            // 计时器窗口不能在地图初始化时显示.
            // whichDialog number timerdialog
            // display boolean
            /// @CSharpLua.Template = "Jass.Common["TimerDialogDisplay"]({0}, {1})"
            public static extern void TimerDialogDisplay(int whichDialog, bool display);

            // whichDialog number timerdialog
            // timeRemaining number
            /// @CSharpLua.Template = "Jass.Common["TimerDialogSetRealTimeRemaining"]({0}, {1})"
            public static extern void TimerDialogSetRealTimeRemaining(int whichDialog, float timeRemaining);

            // 设置计时器窗口速率 [R]
            // 设置[Timer Window]的时间流逝速度为[Factor]倍
            // 同时计时器显示时间也会随之变化. 就是说60秒的计时器设置为2倍速则显示时间也会变为120秒.
            // whichDialog number timerdialog
            // speedMultFactor number
            /// @CSharpLua.Template = "Jass.Common["TimerDialogSetSpeed"]({0}, {1})"
            public static extern void TimerDialogSetSpeed(int whichDialog, float speedMultFactor);

            // 改变计时器窗口计时颜色 [R]
            // 改变[Timer Window]的计间颜色为([Red],[Green],[Blue]) 透明值为:[Alpha]
            // 颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和透明值值取值范围为0-255.
            // whichDialog number timerdialog
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["TimerDialogSetTimeColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void TimerDialogSetTimeColor(int whichDialog, float red, float green, float blue, float alpha);

            // 改变计时器窗口标题
            // 改变[Timer Window]的标题为[Title]
            // whichDialog number timerdialog
            // title string
            /// @CSharpLua.Template = "Jass.Common["TimerDialogSetTitle"]({0}, {1})"
            public static extern void TimerDialogSetTitle(int whichDialog, string title);

            // 改变计时器窗口文字颜色 [R]
            // 改变[Timer Window]文字颜色为([Red],[Green],[Blue]) 透明值为:[Alpha]
            // 颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和透明值值取值范围为0-255.
            // whichDialog number timerdialog
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["TimerDialogSetTitleColor"]({0}, {1}, {2}, {3}, {4})"
            public static extern void TimerDialogSetTitleColor(int whichDialog, float red, float green, float blue, float alpha);

            // whichTimer number timer
            /// @CSharpLua.Template = "Jass.Common["TimerGetElapsed"]({0})"
            public static extern float TimerGetElapsed(int whichTimer);

            // whichTimer number timer
            /// @CSharpLua.Template = "Jass.Common["TimerGetRemaining"]({0})"
            public static extern float TimerGetRemaining(int whichTimer);

            // whichTimer number timer
            /// @CSharpLua.Template = "Jass.Common["TimerGetTimeout"]({0})"
            public static extern float TimerGetTimeout(int whichTimer);

            // 运行计时器 [C]
            // 运行[计时器]，周期:[时间]秒，模式:[模式]，运行函数:[函数]
            // 等同于TimerStart
            // whichTimer number timer
            // timeout number
            // periodic boolean
            // handlerFunc any code
            /// @CSharpLua.Template = "Jass.Common["TimerStart"]({0}, {1}, {2}, {3})"
            public static extern void TimerStart(int whichTimer, float timeout, bool periodic, object handlerFunc);

            // whichTrigger number trigger
            // actionFunc any code
            /// @CSharpLua.Template = "Jass.Common["TriggerAddAction"]({0}, {1})"
            public static extern object TriggerAddAction(int whichTrigger, object actionFunc);

            // whichTrigger number trigger
            // condition function boolexpr
            /// @CSharpLua.Template = "Jass.Common["TriggerAddCondition"]({0}, {1})"
            public static extern object TriggerAddCondition(int whichTrigger, object condition);

            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["TriggerClearActions"]({0})"
            public static extern void TriggerClearActions(int whichTrigger);

            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["TriggerClearConditions"]({0})"
            public static extern void TriggerClearConditions(int whichTrigger);

            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["TriggerEvaluate"]({0})"
            public static extern bool TriggerEvaluate(int whichTrigger);

            // 运行触发(无视条件)
            // 运行[触发](无视条件)
            // 无视事件和条件,运行触发动作.
            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["TriggerExecute"]({0})"
            public static extern void TriggerExecute(int whichTrigger);

            // whichTrigger number trigger
            /// @CSharpLua.Template = "Jass.Common["TriggerExecuteWait"]({0})"
            public static extern void TriggerExecuteWait(int whichTrigger);

            // whichTrigger number trigger
            // whichWidget number widget
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterDeathEvent"]({0}, {1})"
            public static extern object TriggerRegisterDeathEvent(int whichTrigger, float whichWidget);

            // whichTrigger number trigger
            // whichButton number button
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterDialogButtonEvent"]({0}, {1})"
            public static extern object TriggerRegisterDialogButtonEvent(int whichTrigger, float whichButton);

            // whichTrigger number trigger
            // whichDialog number dialog
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterDialogEvent"]({0}, {1})"
            public static extern object TriggerRegisterDialogEvent(int whichTrigger, int whichDialog);

            // whichTrigger number trigger
            // whichRegion number region
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterEnterRegion"]({0}, {1}, {2})"
            public static extern object TriggerRegisterEnterRegion(int whichTrigger, float whichRegion, object filter);

            // whichTrigger number trigger
            // whichUnit number unit
            // whichEvent any unitevent
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterFilterUnitEvent"]({0}, {1}, {2}, {3})"
            public static extern object TriggerRegisterFilterUnitEvent(int whichTrigger, int whichUnit, object whichEvent, object filter);

            // whichTrigger number trigger
            // whichGameEvent any gameevent
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterGameEvent"]({0}, {1})"
            public static extern object TriggerRegisterGameEvent(int whichTrigger, object whichGameEvent);

            // whichTrigger number trigger
            // whichState any gamestate
            // opcode any limitop
            // limitval number
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterGameStateEvent"]({0}, {1}, {2}, {3})"
            public static extern object TriggerRegisterGameStateEvent(int whichTrigger, object whichState, object opcode, float limitval);

            // whichTrigger number trigger
            // whichRegion number region
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterLeaveRegion"]({0}, {1}, {2})"
            public static extern object TriggerRegisterLeaveRegion(int whichTrigger, float whichRegion, object filter);

            // whichTrigger number trigger
            // whichPlayer number player
            // whichAlliance any alliancetype
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterPlayerAllianceChange"]({0}, {1}, {2})"
            public static extern object TriggerRegisterPlayerAllianceChange(int whichTrigger, int whichPlayer, object whichAlliance);

            // whichTrigger number trigger
            // whichPlayer number player
            // chatMessageToDetect string
            // exactMatchOnly boolean
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterPlayerChatEvent"]({0}, {1}, {2}, {3})"
            public static extern object TriggerRegisterPlayerChatEvent(int whichTrigger, int whichPlayer, string chatMessageToDetect, bool exactMatchOnly);

            // whichTrigger number trigger
            // whichPlayer number player
            // whichPlayerEvent any playerevent
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterPlayerEvent"]({0}, {1}, {2})"
            public static extern object TriggerRegisterPlayerEvent(int whichTrigger, int whichPlayer, object whichPlayerEvent);

            // whichTrigger number trigger
            // whichPlayer number player
            // whichState any playerstate
            // opcode any limitop
            // limitval number
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterPlayerStateEvent"]({0}, {1}, {2}, {3}, {4})"
            public static extern object TriggerRegisterPlayerStateEvent(int whichTrigger, int whichPlayer, object whichState, object opcode, float limitval);

            // whichTrigger number trigger
            // whichPlayer number player
            // whichPlayerUnitEvent any playerunitevent
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterPlayerUnitEvent"]({0}, {1}, {2}, {3})"
            public static extern object TriggerRegisterPlayerUnitEvent(int whichTrigger, int whichPlayer, object whichPlayerUnitEvent, object filter);

            // whichTrigger number trigger
            // timeout number
            // periodic boolean
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterTimerEvent"]({0}, {1}, {2})"
            public static extern object TriggerRegisterTimerEvent(int whichTrigger, float timeout, bool periodic);

            // whichTrigger number trigger
            // t number timer
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterTimerExpireEvent"]({0}, {1})"
            public static extern object TriggerRegisterTimerExpireEvent(int whichTrigger, float t);

            // whichTrigger number trigger
            // t any trackable
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterTrackableHitEvent"]({0}, {1})"
            public static extern object TriggerRegisterTrackableHitEvent(int whichTrigger, object t);

            // whichTrigger number trigger
            // t any trackable
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterTrackableTrackEvent"]({0}, {1})"
            public static extern object TriggerRegisterTrackableTrackEvent(int whichTrigger, object t);

            // whichTrigger number trigger
            // whichUnit number unit
            // whichEvent any unitevent
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterUnitEvent"]({0}, {1}, {2})"
            public static extern object TriggerRegisterUnitEvent(int whichTrigger, int whichUnit, object whichEvent);

            // whichTrigger number trigger
            // whichUnit number unit
            // range number
            // filter function boolexpr
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterUnitInRange"]({0}, {1}, {2}, {3})"
            public static extern object TriggerRegisterUnitInRange(int whichTrigger, int whichUnit, float range, object filter);

            // whichTrigger number trigger
            // whichUnit number unit
            // whichState any unitstate
            // opcode any limitop
            // limitval number
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterUnitStateEvent"]({0}, {1}, {2}, {3}, {4})"
            public static extern object TriggerRegisterUnitStateEvent(int whichTrigger, int whichUnit, object whichState, object opcode, float limitval);

            // whichTrigger number trigger
            // varName string
            // opcode any limitop
            // limitval number
            /// @CSharpLua.Template = "Jass.Common["TriggerRegisterVariableEvent"]({0}, {1}, {2}, {3})"
            public static extern object TriggerRegisterVariableEvent(int whichTrigger, string varName, object opcode, float limitval);

            // whichTrigger number trigger
            // whichAction function triggeraction
            /// @CSharpLua.Template = "Jass.Common["TriggerRemoveAction"]({0}, {1})"
            public static extern void TriggerRemoveAction(int whichTrigger, object whichAction);

            // whichTrigger number trigger
            // whichCondition function triggercondition
            /// @CSharpLua.Template = "Jass.Common["TriggerRemoveCondition"]({0}, {1})"
            public static extern void TriggerRemoveCondition(int whichTrigger, int whichCondition);

            // 等待(玩家时间)
            // 等待[Time]秒
            // 该延迟功能受真实时间的影响(也就是玩家机器上的时间). 因此每个玩家所延迟的时间可能不一致.
            // timeout number
            /// @CSharpLua.Template = "Jass.Common["TriggerSleepAction"]({0})"
            public static extern void TriggerSleepAction(float timeout);

            /// @CSharpLua.Template = "Jass.Common["TriggerSyncReady"]()"
            public static extern void TriggerSyncReady();

            /// @CSharpLua.Template = "Jass.Common["TriggerSyncStart"]()"
            public static extern void TriggerSyncStart();

            // s number sound
            // offset number
            /// @CSharpLua.Template = "Jass.Common["TriggerWaitForSound"]({0}, {1})"
            public static extern void TriggerWaitForSound(float s, float offset);

            // whichTrigger number trigger
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["TriggerWaitOnSleeps"]({0}, {1})"
            public static extern void TriggerWaitOnSleeps(int whichTrigger, bool flag);

            // 添加技能 [R]
            // 为[单位]添加[技能]
            // whichUnit number unit
            // abilityId number int
            /// @CSharpLua.Template = "Jass.Common["UnitAddAbility"]({0}, {1})"
            public static extern bool UnitAddAbility(int whichUnit, int abilityId);

            // 闪动指示器(对单位) [R]
            // 对[单位]闪动指示器,使用颜色:([Red]%,[Green]%,[Blue]%) Alpha通道值:[Alpha]
            // 颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和Alpha通道值取值范围为0-255.
            // whichUnit number unit
            // red number int
            // green number int
            // blue number int
            // alpha number int
            /// @CSharpLua.Template = "Jass.Common["UnitAddIndicator"]({0}, {1}, {2}, {3}, {4})"
            public static extern void UnitAddIndicator(int whichUnit, float red, float green, float blue, float alpha);

            // 给予物品 [R]
            // 给予[单位][物品]
            // whichUnit number unit
            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["UnitAddItem"]({0}, {1})"
            public static extern bool UnitAddItem(int whichUnit, float whichItem);

            // whichUnit number unit
            // itemId number int
            /// @CSharpLua.Template = "Jass.Common["UnitAddItemById"]({0}, {1})"
            public static extern float UnitAddItemById(int whichUnit, float itemId);

            // 新建物品到指定物品栏 [R]
            // 给予[单位][物品类型]并放在物品栏#[数值]
            // 注意: 物品栏编号从0-5,而不是1-6. 该动作创建的物品不被'最后创建的物品'所记录.
            // whichUnit number unit
            // itemId number int
            // itemSlot number int
            /// @CSharpLua.Template = "Jass.Common["UnitAddItemToSlotById"]({0}, {1}, {2})"
            public static extern bool UnitAddItemToSlotById(int whichUnit, float itemId, float itemSlot);

            // whichUnit number unit
            // add boolean
            /// @CSharpLua.Template = "Jass.Common["UnitAddSleep"]({0}, {1})"
            public static extern void UnitAddSleep(int whichUnit, bool add);

            // 控制单位睡眠状态
            // 使[单位][Sleep/Remain Awake]
            // 使用该功能前必须用触发为单位添加'一直睡眠'技能.
            // whichUnit number unit
            // add boolean
            /// @CSharpLua.Template = "Jass.Common["UnitAddSleepPerm"]({0}, {1})"
            public static extern void UnitAddSleepPerm(int whichUnit, bool add);

            // 添加类别 [R]
            // 为[单位]添加[Classification]类别
            // 已去除所有无效类别.
            // whichUnit number unit
            // whichUnitType any unittype
            /// @CSharpLua.Template = "Jass.Common["UnitAddType"]({0}, {1})"
            public static extern bool UnitAddType(int whichUnit, object whichUnitType);

            // 设置生命周期 [R]
            // 为[单位]设置[Buff Type]类型的生命周期,持续时间为[Duration]秒
            // whichUnit number unit
            // buffId number int
            // duration number
            /// @CSharpLua.Template = "Jass.Common["UnitApplyTimedLife"]({0}, {1}, {2})"
            public static extern void UnitApplyTimedLife(int whichUnit, float buffId, float duration);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["UnitCanSleep"]({0})"
            public static extern bool UnitCanSleep(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["UnitCanSleepPerm"]({0})"
            public static extern bool UnitCanSleepPerm(int whichUnit);

            // whichUnit number unit
            // removePositive boolean
            // removeNegative boolean
            // magic boolean
            // physical boolean
            // timedLife boolean
            // aura boolean
            // autoDispel boolean
            /// @CSharpLua.Template = "Jass.Common["UnitCountBuffsEx"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
            public static extern float UnitCountBuffsEx(int whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel);

            // 伤害区域 [R]
            // 命令[单位]在[Seconds]秒后对半径为[Size]圆心为([X],[Y])的范围造成[Amount]点伤害([是]攻击伤害,[是]远程攻击) 攻击类型:[AttackType]伤害类型:[DamageType]装甲类型:[WeaponType]
            // 该动作不会打断单位动作. 由该动作伤害/杀死单位同样正常触发'受到伤害'和'死亡'单位事件.
            // whichUnit number unit
            // delay number
            // radius number
            // x number
            // y number
            // amount number
            // attack boolean
            // ranged boolean
            // attackType any attacktype
            // damageType any damagetype
            // weaponType any weapontype
            /// @CSharpLua.Template = "Jass.Common["UnitDamagePoint"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})"
            public static extern bool UnitDamagePoint(int whichUnit, float delay, float radius, float x, float y, float amount, bool attack, bool ranged, object attackType, object damageType, object weaponType);

            // 伤害目标 [R]
            // 命令[单位]对[Target]造成[Amount]点伤害([是]攻击伤害,[是]远程攻击) 攻击类型:[AttackType]伤害类型:[DamageType]武器类型:[WeaponType]
            // 该动作不会打断单位动作. 由该动作伤害/杀死单位同样正常触发'受到伤害'和'死亡'单位事件.
            // whichUnit number unit
            // target number widget
            // amount number
            // attack boolean
            // ranged boolean
            // attackType any attacktype
            // damageType any damagetype
            // weaponType any weapontype
            /// @CSharpLua.Template = "Jass.Common["UnitDamageTarget"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
            public static extern bool UnitDamageTarget(int whichUnit, float target, float amount, bool attack, bool ranged, object attackType, object damageType, object weaponType);

            // 发布丢弃物品命令(指定坐标) [R]
            // 命令[单位]丢弃物品[物品]到坐标:([X],[Y])
            // whichUnit number unit
            // whichItem number item
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["UnitDropItemPoint"]({0}, {1}, {2}, {3})"
            public static extern bool UnitDropItemPoint(int whichUnit, float whichItem, float x, float y);

            // 移动物品到物品栏 [R]
            // 命令[单位]移动[物品]到物品栏#[Index]
            // 只有当单位持有该物品时才有效. 注意: 该函数中物品栏编号从0-5,而不是1-6.
            // whichUnit number unit
            // whichItem number item
            // slot number int
            /// @CSharpLua.Template = "Jass.Common["UnitDropItemSlot"]({0}, {1}, {2})"
            public static extern bool UnitDropItemSlot(int whichUnit, float whichItem, float slot);

            // 发布给予物品命令
            // 命令[单位]把[物品]给[单位]
            // whichUnit number unit
            // whichItem number item
            // target number widget
            /// @CSharpLua.Template = "Jass.Common["UnitDropItemTarget"]({0}, {1}, {2})"
            public static extern bool UnitDropItemTarget(int whichUnit, float whichItem, float target);

            // whichUnit number unit
            // removePositive boolean
            // removeNegative boolean
            // magic boolean
            // physical boolean
            // timedLife boolean
            // aura boolean
            // autoDispel boolean
            /// @CSharpLua.Template = "Jass.Common["UnitHasBuffsEx"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
            public static extern bool UnitHasBuffsEx(int whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel);

            // whichUnit number unit
            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["UnitHasItem"]({0}, {1})"
            public static extern bool UnitHasItem(int whichUnit, float whichItem);

            // unitIdString string
            /// @CSharpLua.Template = "Jass.Common["UnitId"]({0})"
            public static extern float UnitId(string unitIdString);

            // unitId number int
            /// @CSharpLua.Template = "Jass.Common["UnitId2String"]({0})"
            public static extern string UnitId2String(int unitId);

            // whichUnit number unit
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["UnitIgnoreAlarm"]({0}, {1})"
            public static extern bool UnitIgnoreAlarm(int whichUnit, bool flag);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["UnitIgnoreAlarmToggled"]({0})"
            public static extern bool UnitIgnoreAlarmToggled(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["UnitInventorySize"]({0})"
            public static extern float UnitInventorySize(int whichUnit);

            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["UnitIsSleeping"]({0})"
            public static extern bool UnitIsSleeping(int whichUnit);

            // whichUnit number unit
            // itemSlot number int
            /// @CSharpLua.Template = "Jass.Common["UnitItemInSlot"]({0}, {1})"
            public static extern float UnitItemInSlot(int whichUnit, float itemSlot);

            // 设置技能永久性 [R]
            // 设置[单位][是否][技能]永久性
            // 如触发添加给单位的技能就是非永久性的,非永久性技能在变身并回复之后会丢失掉. 这类情况就需要设置技能永久性.
            // whichUnit number unit
            // permanent boolean
            // abilityId number int
            /// @CSharpLua.Template = "Jass.Common["UnitMakeAbilityPermanent"]({0}, {1}, {2})"
            public static extern bool UnitMakeAbilityPermanent(int whichUnit, bool permanent, int abilityId);

            // 添加剩余技能点 [R]
            // 增加[英雄][Value]点剩余技能点
            // whichHero number unit
            // skillPointDelta number int
            /// @CSharpLua.Template = "Jass.Common["UnitModifySkillPoints"]({0}, {1})"
            public static extern bool UnitModifySkillPoints(float whichHero, float skillPointDelta);

            // 暂停/恢复生命周期 [R]
            // 使[单位][Pause/Unpause]生命周期
            // 只有召唤单位有生命周期.
            // whichUnit number unit
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["UnitPauseTimedLife"]({0}, {1})"
            public static extern void UnitPauseTimedLife(int whichUnit, bool flag);

            // 添加单位类型 [R]
            // 在[单位池]中添加一个[单位]比重为[数值]
            // 比重越高被选择的机率越大
            // whichPool number unitpool
            // unitId number int
            // weight number
            /// @CSharpLua.Template = "Jass.Common["UnitPoolAddUnitType"]({0}, {1}, {2})"
            public static extern void UnitPoolAddUnitType(float whichPool, int unitId, float weight);

            // 删除单位类型 [R]
            // 从[单位池]中删除[单位]
            // whichPool number unitpool
            // unitId number int
            /// @CSharpLua.Template = "Jass.Common["UnitPoolRemoveUnitType"]({0}, {1})"
            public static extern void UnitPoolRemoveUnitType(float whichPool, int unitId);

            // 删除技能 [R]
            // 为[单位]删除[技能]
            // whichUnit number unit
            // abilityId number int
            /// @CSharpLua.Template = "Jass.Common["UnitRemoveAbility"]({0}, {1})"
            public static extern bool UnitRemoveAbility(int whichUnit, int abilityId);

            // 删除魔法效果(指定极性) [R]
            // 删除[单位]的附带Buff,([Include/Exclude]正面Buff,[Include/Exclude]负面Buff)
            // whichUnit number unit
            // removePositive boolean
            // removeNegative boolean
            /// @CSharpLua.Template = "Jass.Common["UnitRemoveBuffs"]({0}, {1}, {2})"
            public static extern void UnitRemoveBuffs(int whichUnit, bool removePositive, bool removeNegative);

            // 删除魔法效果(详细类别) [R]
            // 删除[单位]的附带Buff,([Include/Exclude]正面Buff,[Include/Exclude]负面Buff[Include/Exclude]魔法Buff,[Include/Exclude]物理Buff[Include/Exclude]生命周期,[Include/Exclude]光环效果[Include/Exclude]不可驱散Buff)
            // whichUnit number unit
            // removePositive boolean
            // removeNegative boolean
            // magic boolean
            // physical boolean
            // timedLife boolean
            // aura boolean
            // autoDispel boolean
            /// @CSharpLua.Template = "Jass.Common["UnitRemoveBuffsEx"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
            public static extern void UnitRemoveBuffsEx(int whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel);

            // whichUnit number unit
            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["UnitRemoveItem"]({0}, {1})"
            public static extern void UnitRemoveItem(int whichUnit, float whichItem);

            // whichUnit number unit
            // itemSlot number int
            /// @CSharpLua.Template = "Jass.Common["UnitRemoveItemFromSlot"]({0}, {1})"
            public static extern float UnitRemoveItemFromSlot(int whichUnit, float itemSlot);

            // 删除类别 [R]
            // 为[单位]删除[Classification]类别
            // 已去除所有无效类别.
            // whichUnit number unit
            // whichUnitType any unittype
            /// @CSharpLua.Template = "Jass.Common["UnitRemoveType"]({0}, {1})"
            public static extern bool UnitRemoveType(int whichUnit, object whichUnitType);

            // 重置技能CD
            // 重置[单位]的所有技能冷却时间
            // 如果要重置单一技能的CD,可以通过删除技能+添加技能+设置技能等级来完成.
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["UnitResetCooldown"]({0})"
            public static extern void UnitResetCooldown(int whichUnit);

            // 设置建筑建造进度条
            // 设置[某个建筑]的建造进度条为[Progress]%
            // 只作用于正在建造的建筑.
            // whichUnit number unit
            // constructionPercentage number int
            /// @CSharpLua.Template = "Jass.Common["UnitSetConstructionProgress"]({0}, {1})"
            public static extern void UnitSetConstructionProgress(int whichUnit, float constructionPercentage);

            // 设置建筑升级进度条
            // 设置[某个建筑]的升级进度条为[Progress]%
            // 只作用于正在升级的建筑. 是建筑A升级为建筑B的升级,不是科技的研究.
            // whichUnit number unit
            // upgradePercentage number int
            /// @CSharpLua.Template = "Jass.Common["UnitSetUpgradeProgress"]({0}, {1})"
            public static extern void UnitSetUpgradeProgress(int whichUnit, float upgradePercentage);

            // 开启/关闭 小地图特殊标志
            // [On/Off][单位]的小地图特殊标志
            // 使用'中立建筑 - 设置小地图特殊标志'动作来设置显示的标志. 默认为中立建筑标志.
            // whichUnit number unit
            // flag boolean
            /// @CSharpLua.Template = "Jass.Common["UnitSetUsesAltIcon"]({0}, {1})"
            public static extern void UnitSetUsesAltIcon(int whichUnit, bool flag);

            // 共享视野 [R]
            // 设置[单位]的视野对[某个玩家][on/off]
            // whichUnit number unit
            // whichPlayer number player
            // share boolean
            /// @CSharpLua.Template = "Jass.Common["UnitShareVision"]({0}, {1}, {2})"
            public static extern void UnitShareVision(int whichUnit, int whichPlayer, bool share);

            // 降低等级 [R]
            // 降低[某个英雄][Level]个等级
            // 只能降低等级. 英雄经验将重置为该等级的初始值.
            // whichHero number unit
            // howManyLevels number int
            /// @CSharpLua.Template = "Jass.Common["UnitStripHeroLevel"]({0}, {1})"
            public static extern bool UnitStripHeroLevel(float whichHero, float howManyLevels);

            // 暂停尸体腐烂 [R]
            // 设置[单位]的尸体腐烂状态:[Suspend/Resume]
            // 只对已完成死亡动作的尸体有效.
            // whichUnit number unit
            // suspend boolean
            /// @CSharpLua.Template = "Jass.Common["UnitSuspendDecay"]({0}, {1})"
            public static extern void UnitSuspendDecay(int whichUnit, bool suspend);

            // 使用物品(无目标)
            // 命令[单位]使用[物品]
            // whichUnit number unit
            // whichItem number item
            /// @CSharpLua.Template = "Jass.Common["UnitUseItem"]({0}, {1})"
            public static extern bool UnitUseItem(int whichUnit, float whichItem);

            // 使用物品(指定坐标)
            // 命令[单位]使用[物品],目标坐标:([X],[Y])
            // whichUnit number unit
            // whichItem number item
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["UnitUseItemPoint"]({0}, {1}, {2}, {3})"
            public static extern bool UnitUseItemPoint(int whichUnit, float whichItem, float x, float y);

            // 使用物品(对单位)
            // 命令[单位]使用[物品],目标:[单位]
            // whichUnit number unit
            // whichItem number item
            // target number widget
            /// @CSharpLua.Template = "Jass.Common["UnitUseItemTarget"]({0}, {1}, {2})"
            public static extern bool UnitUseItemTarget(int whichUnit, float whichItem, float target);

            // 叫醒
            // 叫醒[单位]
            // 不影响催眠魔法效果.
            // whichUnit number unit
            /// @CSharpLua.Template = "Jass.Common["UnitWakeUp"]({0})"
            public static extern void UnitWakeUp(int whichUnit);

            // soundHandle number sound
            // byPosition boolean
            // rectwidth number
            // rectheight number
            /// @CSharpLua.Template = "Jass.Common["UnregisterStackedSound"]({0}, {1}, {2}, {3})"
            public static extern void UnregisterStackedSound(float soundHandle, bool byPosition, float rectwidth, float rectheight);

            // whichVersion any version
            /// @CSharpLua.Template = "Jass.Common["VersionCompatible"]({0})"
            public static extern bool VersionCompatible(object whichVersion);

            /// @CSharpLua.Template = "Jass.Common["VersionGet"]()"
            public static extern object VersionGet();

            // whichVersion any version
            /// @CSharpLua.Template = "Jass.Common["VersionSupported"]({0})"
            public static extern bool VersionSupported(object whichVersion);

            // 重置多通道音量
            // 重置所有通道音量为预设值.
            /// @CSharpLua.Template = "Jass.Common["VolumeGroupReset"]()"
            public static extern void VolumeGroupReset();

            // 设置多通道音量 [R]
            // 设置[Volume Channel]的音量为[Volume]
            // 音量取值范围0-1.
            // vgroup any volumegroup
            // scale number
            /// @CSharpLua.Template = "Jass.Common["VolumeGroupSetVolume"]({0}, {1})"
            public static extern void VolumeGroupSetVolume(object vgroup, float scale);

            // 启用/禁用 传送门
            // [Enable/Disable][传送门]
            // waygate number unit
            // activate boolean
            /// @CSharpLua.Template = "Jass.Common["WaygateActivate"]({0}, {1})"
            public static extern void WaygateActivate(float waygate, bool activate);

            // waygate number unit
            /// @CSharpLua.Template = "Jass.Common["WaygateGetDestinationX"]({0})"
            public static extern float WaygateGetDestinationX(float waygate);

            // waygate number unit
            /// @CSharpLua.Template = "Jass.Common["WaygateGetDestinationY"]({0})"
            public static extern float WaygateGetDestinationY(float waygate);

            // waygate number unit
            /// @CSharpLua.Template = "Jass.Common["WaygateIsActive"]({0})"
            public static extern bool WaygateIsActive(float waygate);

            // 设置传送门目的坐标 [R]
            // 设置[传送门]的目的地为([X],[Y])
            // waygate number unit
            // x number
            // y number
            /// @CSharpLua.Template = "Jass.Common["WaygateSetDestination"]({0}, {1}, {2})"
            public static extern void WaygateSetDestination(float waygate, float x, float y);
        }
        public static partial class Debug
        {

            /// @CSharpLua.Template = "Jass.Debug["handle_ref"]({0})"
            public static extern void HandleRef(int handle);
            /// @CSharpLua.Template = "Jass.Debug["handle_unref"]({0})"
            public static extern void HandleUnRef(int handle);
            /// @CSharpLua.Template = "Jass.Debug["handlemax"]()"
            public static extern int HandleMax();
            /// @CSharpLua.Template = "Jass.Debug["handledef"]({0})"
            public static extern dynamic HandleDef(int handle);
        }
    }
}
