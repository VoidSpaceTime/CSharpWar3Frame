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
        // title = "当前选择的单位(异步)"        
        public static JUnit DzGetSelectedLeaderUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("DzGetSelectedLeaderUnit"));
        }

        // title = "聊天框是否打开"
        public static bool DzIsChatBoxOpen()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsChatBoxOpen"));
        }

        // title = "设置单位的鼠标指向UI和血条显示/隐藏"
        public static void DzSetUnitPreselectUIVisible(JUnit whichUnit, bool visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitPreselectUIVisible"), whichUnit, visible);
        }

        // title = "设置特效播放动画"
        public static void DzSetEffectAnimation(JEffect whichEffect, int index, int flag)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectAnimation"), whichEffect, index, flag);
        }

        // title = "设置特效坐标"
        public static void DzSetEffectPos(JEffect whichEffect, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectPos"), whichEffect, x, y, z);
        }

        // title = "设置特效颜色"
        public static void DzSetEffectVertexColor(JEffect whichEffect, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectVertexColor"), whichEffect, color);
        }

        // title = "设置特效透明度"
        public static void DzSetEffectVertexAlpha(JEffect whichEffect, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectVertexAlpha"), whichEffect, alpha);
        }

        // title = "设置特效模型"
        public static void DzSetEffectModel(JEffect whichEffect, string model)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectModel"), whichEffect, model);
        }

        // title = "设置特效队伍颜色"
        public static void DzSetEffectTeamColor(JEffect whichHandle, int playerId)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectTeamColor"), whichHandle, playerId);
        }

        // title = "设置控件视口"
        public static void DzFrameSetClip(int whichframe, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetClip"), whichframe, enable);
        }

        // title = "设置魔兽窗口大小"
        public static bool DzChangeWindowSize(int width, int height)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzChangeWindowSize"), width, height);
        }

        // title = "设置特效播放动画"
        public static void DzPlayEffectAnimation(JEffect whichEffect, string anim, string link)
        {
            War3.CallNative(War3.GetNativeFunction("DzPlayEffectAnimation"), whichEffect, anim, link);
        }

        // title = "绑定特效"
        public static void DzBindEffect(JWidget parent, string attachPoint, JEffect whichEffect)
        {
            War3.CallNative(War3.GetNativeFunction("DzBindEffect"), parent, attachPoint, whichEffect);
        }

        // title = "解除绑定特效"
        public static void DzUnbindEffect(JEffect whichEffect)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnbindEffect"), whichEffect);
        }

        // title = "单位缩放"
        public static void DzSetWidgetSpriteScale(JWidget whichUnit, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetWidgetSpriteScale"), whichUnit, scale);
        }

        // title = "特效缩放"
        public static void DzSetEffectScale(JEffect whichHandle, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectScale"), whichHandle, scale);
        }

        // title = "获取特效颜色"
        public static int DzGetEffectVertexColor(JEffect whichEffect)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetEffectVertexColor"), whichEffect);
        }

        // title = "获取特效透明度"
        public static int DzGetEffectVertexAlpha(JEffect whichEffect)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetEffectVertexAlpha"), whichEffect);
        }

        // title = "获取物品技能"
        public static JAbility DzGetItemAbility(JItem whichEffect, int index)
        {
            return War3.CallNative<JAbility>(War3.GetNativeFunction("DzGetItemAbility"), whichEffect, index);
        }

        // title = "获取子控件数量"
        public static int DzFrameGetChildrenCount(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetChildrenCount"), whichframe);
        }

        // title = "获取子控件"
        public static int DzFrameGetChild(int whichframe, int index)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetChild"), whichframe, index);
        }

        // title = "解锁BLP像素限制"
        public static void DzUnlockBlpSizeLimit(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnlockBlpSizeLimit"), enable);
        }

        // title = "获取商店目标"
        public static JUnit DzGetActivePatron(JUnit store, JPlayer p)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("DzGetActivePatron"), store, p);
        }

        // title = "获取玩家选中的单位数量"
        public static int DzGetLocalSelectUnitCount()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetLocalSelectUnitCount"));
        }

        // title = "获取玩家选中的单位"
        public static JUnit DzGetLocalSelectUnit(int index)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("DzGetLocalSelectUnit"), index);
        }

        // title = "获取字符串数量"
        public static int DzGetJassStringTableCount()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetJassStringTableCount"));
        }

        // title = "清除模型内存缓存"
        public static void DzModelRemoveFromCache(string path)
        {
            War3.CallNative(War3.GetNativeFunction("DzModelRemoveFromCache"), path);
        }

        // title = "清除所有模型内存缓存"
        public static void DzModelRemoveAllFromCache()
        {
            War3.CallNative(War3.GetNativeFunction("DzModelRemoveAllFromCache"));
        }

        // title = "获取框选控件"
        public static int DzFrameGetInfoPanelSelectButton(int index)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetInfoPanelSelectButton"), index);
        }

        // title = "获取BUFF控件"
        public static int DzFrameGetInfoPanelBuffButton(int index)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetInfoPanelBuffButton"), index);
        }

        // title = "获取农民控件"
        public static int DzFrameGetPeonBar()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetPeonBar"));
        }

        // title = "获取技能右下角数字文本控件"
        public static int DzFrameGetCommandBarButtonNumberText(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButtonNumberText"), whichframe);
        }

        // title = "获取技能右下角数字文本框体"
        public static int DzFrameGetCommandBarButtonNumberOverlay(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButtonNumberOverlay"), whichframe);
        }

        // title = "获取技能冷却指示器"
        public static int DzFrameGetCommandBarButtonCooldownIndicator(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButtonCooldownIndicator"), whichframe);
        }

        // title = "获取技能自动施法指示器"
        public static int DzFrameGetCommandBarButtonAutoCastIndicator(int whichframe)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButtonAutoCastIndicator"), whichframe);
        }

        // title = "设置FPS显示/隐藏"
        public static void DzToggleFPS(bool show)
        {
            War3.CallNative(War3.GetNativeFunction("DzToggleFPS"), show);
        }

        // title = "获取 FPS 帧数"
        public static int DzGetFPS()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetFPS"));
        }

        // title = "转换地图坐标为小地图x坐标"
        public static float DzFrameWorldToMinimapPosX(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameWorldToMinimapPosX"), x, y);
        }

        // title = "转换地图坐标为小地图y坐标"
        public static float DzFrameWorldToMinimapPosY(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameWorldToMinimapPosY"), x, y);
        }

        // title = "自定义指定单位的小地图图标"
        public static void DzWidgetSetMinimapIcon(JUnit whichunit, string path)
        {
            War3.CallNative(War3.GetNativeFunction("DzWidgetSetMinimapIcon"), whichunit, path);
        }

        // title = "开启/关闭自定义指定单位的小地图图标"
        public static void DzWidgetSetMinimapIconEnable(JUnit whichunit, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzWidgetSetMinimapIconEnable"), whichunit, enable);
        }

        // title = "游戏提示信息界面"
        public static int DzFrameGetWorldFrameMessage()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetWorldFrameMessage"));
        }

        // title = "显示游戏提示信息"
        public static void DzSimpleMessageFrameAddMessage(int whichframe, string text, int color, float duration, bool permanent)
        {
            War3.CallNative(War3.GetNativeFunction("DzSimpleMessageFrameAddMessage"), whichframe, text, color, duration, permanent);
        }

        // title = "清理游戏提示信息"
        public static void DzSimpleMessageFrameClear(int whichframe)
        {
            War3.CallNative(War3.GetNativeFunction("DzSimpleMessageFrameClear"), whichframe);
        }

        // title = "转换屏幕坐标到世界x坐标"
        public static float DzConvertScreenPositionX(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzConvertScreenPositionX"), x, y);
        }

        // title = "转换屏幕坐标到世界y坐标"
        public static float DzConvertScreenPositionY(float x, float y)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzConvertScreenPositionY"), x, y);
        }

        // title = "监听建筑选位置"
        public static void DzRegisterOnBuildLocal(JCode func)
        {
            War3.CallNative(War3.GetNativeFunction("DzRegisterOnBuildLocal"), func);
        }

        // title = "获取建造的命令id"
        public static int DzGetOnBuildOrderId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnBuildOrderId"));
        }

        // title = "获取建造的命令类型"
        public static int DzGetOnBuildOrderType()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnBuildOrderType"));
        }

        // title = "获取预建造对象"
        public static JWidget DzGetOnBuildAgent()
        {
            return War3.CallNative<JWidget>(War3.GetNativeFunction("DzGetOnBuildAgent"));
        }

        // title = "监听技能预选目标"
        public static void DzRegisterOnTargetLocal(JCode func)
        {
            War3.CallNative(War3.GetNativeFunction("DzRegisterOnTargetLocal"), func);
        }

        // title = "获取监听到的技能"
        public static int DzGetOnTargetAbilId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnTargetAbilId"));
        }

        // title = "获取监听到技能预选命令"
        public static int DzGetOnTargetOrderId()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnTargetOrderId"));
        }

        // title = "获取监听到技能预选命令类型"
        public static int DzGetOnTargetOrderType()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetOnTargetOrderType"));
        }

        // title = "获取监听到技能预选目标"
        public static JWidget DzGetOnTargetAgent()
        {
            return War3.CallNative<JWidget>(War3.GetNativeFunction("DzGetOnTargetAgent"));
        }

        // title = "获取监听到技能预选目标"
        public static JWidget DzGetOnTargetInstantTarget()
        {
            return War3.CallNative<JWidget>(War3.GetNativeFunction("DzGetOnTargetInstantTarget"));
        }

        // title = "打开QQ群链接"
        public static bool DzOpenQQGroupUrl(string url)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzOpenQQGroupUrl"), url);
        }

        // title = "游戏界面限制设置"
        public static void DzFrameEnableClipRect(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameEnableClipRect"), enable);
        }

        // title = "设置单位名字"
        public static void DzSetUnitName(JUnit whichUnit, string name)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitName"), whichUnit, name);
        }

        // title = "设置单位头像模型"
        public static void DzSetUnitPortrait(JUnit whichUnit, string modelFile)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitPortrait"), whichUnit, modelFile);
        }

        // title = "设置单位描述"
        public static void DzSetUnitDescription(JUnit whichUnit, string value)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitDescription"), whichUnit, value);
        }

        // title = "设置单位普攻弹道弧度"
        public static void DzSetUnitMissileArc(JUnit whichUnit, float arc)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitMissileArc"), whichUnit, arc);
        }

        // title = "设置单位普攻弹道模型"
        public static void DzSetUnitMissileModel(JUnit whichUnit, string modelFile)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitMissileModel"), whichUnit, modelFile);
        }

        // title = "设置英雄称谓"
        public static void DzSetUnitProperName(JUnit whichUnit, string name)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitProperName"), whichUnit, name);
        }

        // title = "设置单位普攻弹道自导允许"
        public static void DzSetUnitMissileHoming(JUnit whichUnit, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitMissileHoming"), whichUnit, enable);
        }

        // title = "设置单位普攻弹道速度"
        public static void DzSetUnitMissileSpeed(JUnit whichUnit, float speed)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitMissileSpeed"), whichUnit, speed);
        }

        // title = "特效显示/隐藏"
        public static void DzSetEffectVisible(JEffect whichHandle, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectVisible"), whichHandle, enable);
        }

        // title = "复活单位"
        public static void DzReviveUnit(JUnit whichUnit, JPlayer whichPlayer, float hp, float mp, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzReviveUnit"), whichUnit, whichPlayer, hp, mp, x, y);
        }

        // title = "获取普攻技能"
        public static JAbility DzGetAttackAbility(JUnit whichUnit)
        {
            return War3.CallNative<JAbility>(War3.GetNativeFunction("DzGetAttackAbility"), whichUnit);
        }

        // title = "结束普攻技能CD"
        public static void DzAttackAbilityEndCooldown(JAbility whichHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzAttackAbilityEndCooldown"), whichHandle);
        }

        public static bool EXSetUnitArrayString(int uid, int id, int n, string name)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetUnitArrayString"), uid, id, n, name);
        }

        public static bool EXSetUnitInteger(int uid, int id, int n)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetUnitInteger"), uid, id, n);
        }

        // title = "设置英雄类型称谓名[API][new]"
        public static void DzSetHeroTypeProperName(int uid, string name)
        {
            EXSetUnitArrayString(uid, 61, 0, name);
            EXSetUnitInteger(uid, 61, 1);
        }

        // title = "设置单位类型名[API][new]"
        public static void DzSetUnitTypeName(int uid, string name)
        {
            EXSetUnitArrayString(uid, 10, 0, name);
            EXSetUnitInteger(uid, 10, 1);
        }

        // title = "攻击类型[JAPI]"
        public static bool DzIsUnitAttackType(JUnit whichUnit, int index, JAttackType JAttackType)
        {
            return JassApi.ConvertAttackType(JassApi.R2I(JassApi.GetUnitState(whichUnit, JassApi.ConvertUnitState(16 + 19 * index)))) == JAttackType;
        }

        // title = "设置攻击类型[API]"
        public static void DzSetUnitAttackType(JUnit whichUnit, int index, JAttackType JAttackType)
        {
            JassApi.SetUnitState(whichUnit, JassApi.ConvertUnitState(16 + 19 * index), JassApi.GetHandleId(JAttackType));
        }

        // title = "护甲类型[JAPI]"
        public static bool DzIsUnitDefenseType(JUnit whichUnit, int defenseType)
        {
            return JassApi.R2I(JassApi.GetUnitState(whichUnit, JassApi.ConvertUnitState(0x50))) == defenseType;
        }

        // title = "设置护甲类型[API]"
        public static void DzSetUnitDefenseType(JUnit whichUnit, int defenseType)
        {
            JassApi.SetUnitState(whichUnit, JassApi.ConvertUnitState(0x50), defenseType);
        }

        // title = "新建地形装饰物"
        public static int DzDoodadCreate(int id, int var, float x, float y, float z, float rotate, float scale)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadCreate"), id, var, x, y, z, rotate, scale);
        }

        // title = "装饰物的类型ID"
        public static int DzDoodadGetTypeId(int doodad)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadGetTypeId"), doodad);
        }

        // title = "设置装饰物模型"
        public static void DzDoodadSetModel(int doodad, string modelFile)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetModel"), doodad, modelFile);
        }

        // title = "改变装饰物队伍颜色"
        public static void DzDoodadSetTeamColor(int doodad, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetTeamColor"), doodad, color);
        }

        // title = "设置装饰物颜色"
        public static void DzDoodadSetColor(int doodad, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetColor"), doodad, color);
        }

        // title = "装饰物的X坐标"
        public static float DzDoodadGetX(int doodad)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzDoodadGetX"), doodad);
        }

        // title = "装饰物的Y坐标"
        public static float DzDoodadGetY(int doodad)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzDoodadGetY"), doodad);
        }

        // title = "装饰物的Z坐标"
        public static float DzDoodadGetZ(int doodad)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzDoodadGetZ"), doodad);
        }

        // title = "设置装饰物位置"
        public static void DzDoodadSetPosition(int doodad, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetPosition"), doodad, x, y, z);
        }

        // title = "设置装饰物旋转"
        public static void DzDoodadSetOrientMatrixRotate(int doodad, float angle, float axisX, float axisY, float axisZ)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetOrientMatrixRotate"), doodad, angle, axisX, axisY, axisZ);
        }

        // title = "修改装饰物尺寸"
        public static void DzDoodadSetOrientMatrixScale(int doodad, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetOrientMatrixScale"), doodad, x, y, z);
        }

        // title = "装饰物重置大小"
        public static void DzDoodadSetOrientMatrixResize(int doodad)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetOrientMatrixResize"), doodad);
        }

        // title = "装饰物显示/隐藏"
        public static void DzDoodadSetVisible(int doodad, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetVisible"), doodad, enable);
        }

        // title = "装饰物播放动画"
        public static void DzDoodadSetAnimation(int doodad, string animName, bool animRandom)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetAnimation"), doodad, animName, animRandom);
        }

        // title = "设置装饰物动画播放速度"
        public static void DzDoodadSetTimeScale(int doodad, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadSetTimeScale"), doodad, scale);
        }

        // title = "装饰物动画播放速度"
        public static float DzDoodadGetTimeScale(int doodad)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzDoodadGetTimeScale"), doodad);
        }

        // title = "装饰物当前动画编号"
        public static int DzDoodadGetCurrentAnimationIndex(int doodad)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadGetCurrentAnimationIndex"), doodad);
        }

        // title = "装饰物动画数量"
        public static int DzDoodadGetAnimationCount(int doodad)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadGetAnimationCount"), doodad);
        }

        // title = "装饰物动画名"
        public static string DzDoodadGetAnimationName(int doodad, int index)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzDoodadGetAnimationName"), doodad, index);
        }

        // title = "装饰物动画时间"
        public static int DzDoodadGetAnimationTime(int doodad, int index)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzDoodadGetAnimationTime"), doodad, index);
        }

        // title = "解锁JASS字节码限制 [NEW]"
        public static void DzUnlockOpCodeLimit(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnlockOpCodeLimit"), enable);
        }

        // title = "设置剪切板 [NEW]"
        public static bool DzSetClipboard(string content)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetClipboard"), content);
        }

        // title = "删除装饰物  [NEW]"
        public static void DzDoodadRemove(int doodad)
        {
            War3.CallNative(War3.GetNativeFunction("DzDoodadRemove"), doodad);
        }

        // title = "降低玩家科技等级 [NEW]"
        public static void DzRemovePlayerTechResearched(JPlayer whichPlayer, int techid, int removelevels)
        {
            War3.CallNative(War3.GetNativeFunction("DzRemovePlayerTechResearched"), whichPlayer, techid, removelevels);
        }

        // title = "获取单位的指定技能"
        public static JAbility DzUnitFindAbility(JUnit whichUnit, int abilcode)
        {
            return War3.CallNative<JAbility>(War3.GetNativeFunction("DzUnitFindAbility"), whichUnit, abilcode);
        }

        // title = "设置技能数据-字符串"
        public static void DzAbilitySetStringData(JAbility whichAbility, string key, string value)
        {
            War3.CallNative(War3.GetNativeFunction("DzAbilitySetStringData"), whichAbility, key, value);
        }

        // title = "设置技能启用/禁用"
        public static void DzAbilitySetEnable(JAbility whichAbility, bool enable, bool hideUI)
        {
            War3.CallNative(War3.GetNativeFunction("DzAbilitySetEnable"), whichAbility, enable, hideUI);
        }

        // title = "设置单位实例的移动类型"
        public static void DzUnitSetMoveType(JUnit whichUnit, string moveType)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitSetMoveType"), whichUnit, moveType);
        }

        // title = "获取 Frame 的 宽度"
        public static float DzFrameGetWidth(int frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetWidth"), frame);
        }

        // title = "设置模型界面播放动画（编号）"
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

        // title = "设置单位整数物编数据"
        public static void KKWESetUnitDataCacheInteger(int uid, int id, int v)
        {
            DzSetUnitDataCacheInteger(uid, id, 0, v);
        }

        // title = "设置单位物编数据(建筑升级)"
        public static void KKWEUnitUIAddUpgradesIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 94, id, v);
        }

        // title = "设置单位物编数据(农民可建造建筑)"
        public static void KKWEUnitUIAddBuildsIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 100, id, v);
        }

        // title = "设置单位物编数据(可研究的科技)"
        public static void KKWEUnitUIAddResearchesIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 112, id, v);
        }

        // title = "设置单位物编数据(可训练的单位)"
        public static void KKWEUnitUIAddTrainsIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 106, id, v);
        }

        // title = "设置单位物编数据(出售的单位)"
        public static void KKWEUnitUIAddSellsUnitIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 118, id, v);
        }

        // title = "设置单位物编数据(出售的物品)"
        public static void KKWEUnitUIAddSellsItemIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 124, id, v);
        }

        // title = "设置单位物编数据(制造的物品)"
        public static void KKWEUnitUIAddMakesItemIds(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 130, id, v);
        }

        // title = "设置单位物编数据(科技需求)[单位]"
        public static void KKWEUnitUIAddRequiresUnitCode(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 166, id, v);
        }

        // title = "设置单位物编数据(科技需求)[科技]"
        public static void KKWEUnitUIAddRequiresTechcode(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 166, id, v);
        }

        // title = "设置单位物编数据(科技需求值)"
        public static void KKWEUnitUIAddRequiresAmounts(int uid, int id, int v)
        {
            DzUnitUIAddLevelArrayInteger(uid, 172, id, v);
        }

        // title = "设置物品模型 [NEW]"
        public static void DzItemSetModel(JItem whichItem, string file)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetModel"), whichItem, file);
        }

        // title = "设置物品颜色 [NEW]"
        public static void DzItemSetVertexColor(JItem whichItem, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetVertexColor"), whichItem, color);
        }

        // title = "设置物品透明度(0-255) [NEW]"
        public static void DzItemSetAlpha(JItem whichItem, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetAlpha"), whichItem, color);
        }

        // title = "设置物品头像 [NEW]"
        public static void DzItemSetPortrait(JItem whichItem, string modelPath)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetPortrait"), whichItem, modelPath);
        }

        // title = "血条刷新事件 [NEW]"
        public static void DzFrameHookHpBar(JCode func)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameHookHpBar"), func);
        }

        // title = "触发血条的单位 [NEW]"
        public static JUnit DzFrameGetTriggerHpBarUnit()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("DzFrameGetTriggerHpBarUnit"));
        }

        // title = "触发的血条 [NEW]"
        public static int DzFrameGetTriggerHpBar()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetTriggerHpBar"));
        }

        // title = "获取单位血条 [NEW]"
        public static int DzFrameGetUnitHpBar(JUnit whichUnit)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetUnitHpBar"), whichUnit);
        }

        // title = "鼠标界面 [NEW]"
        public static int DzGetCursorFrame()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetCursorFrame"));
        }

        // title = "是否有指定锚点 [NEW]"
        public static bool DzFrameGetPointValid(int frame, int anchor)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameGetPointValid"), frame, anchor);
        }

        // title = "获取相对锚点所在界面 [NEW]"
        public static int DzFrameGetPointRelative(int frame, int anchor)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetPointRelative"), frame, anchor);
        }

        // title = "获取相对锚点的界面锚点 [NEW]"
        public static int DzFrameGetPointRelativePoint(int frame, int anchor)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetPointRelativePoint"), frame, anchor);
        }

        // title = "获取锚点X坐标 [NEW]"
        public static float DzFrameGetPointX(int frame, int anchor)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetPointX"), frame, anchor);
        }

        // title = "获取锚点Y坐标 [NEW]"
        public static float DzFrameGetPointY(int frame, int anchor)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetPointY"), frame, anchor);
        }

        /*      public static bool DzIsLeapYear(int year)
              {
                  return (ModuloInteger(year, 4) == 0 and ModuloInteger(year , 100) != 0) or(ModuloInteger(year, 400) == 0);
              }*/

        /*        public static string DzGetTimeDateFromTimestamp(int timestamp)
                {
                    local integer totalSeconds = timestamp + 28800;
                    local integer days = 0;
                    local integer day = 0;
                    local integer secondsInDay = 86400;
                    local integer remainingSeconds = ModuloInteger(totalSeconds, secondsInDay);
                    local integer year = 1970;
                    local integer totalDays = (totalSeconds + 86399) / (secondsInDay);
                    local integer num = 0;
                    local integer month = 0;
                    local integer hour;
                    local integer minute;
                    local integer second;
                    local string str;
                    loop;
                    if (DzIsLeapYear(year))
                    {
                        set num = num + 366;
                    else
                        {
                            set num = num + 365;
                        }
                        if (num > totalDays)
                        {
                            set totalDays = totalDays - days;
                            exitwhen true;
                    else
                            {
                                set days = num;
                            }
                            set year = year + 1;
                            endloop;
                            set month = 1;
                            set num = 0;
                            set days = 0;
                            if (DzIsLeapYear(year)) then;
                            loop;
                            set num = num + MonthDay[100 + month];
                            if (num >= totalDays)
                            {
                                set day = totalDays - days;
                                exitwhen true;
                    else
                                {
                                    set days = num;
                                }
                                set month = month + 1;
                                endloop;
                    else
                                {
                                    loop;
                                    set num = num + MonthDay[month];
                                    if (num >= totalDays)
                                    {
                                        set day = totalDays - days;
                                        exitwhen true;
                                else
                                        {
                                            set days = num;
                                        }
                                        set month = month + 1;
                                        endloop;
                                    }
                                    if (day == 0) then;
                                    set day = 1;
                                }
                                set hour = remainingSeconds / 3600;
                                set remainingSeconds = ModuloInteger(remainingSeconds, 3600);
                                set minute = remainingSeconds / 60;
                                set second = ModuloInteger(remainingSeconds, 60);
                                set str = I2S(year) + "-" + I2S(month) + "-" + I2S(day) + " " + I2S(hour) + ":" + I2S(minute) + ":" + I2S(second);
                                // title = "<1.24> 保存整数 [C]"            SaveInteger(Hash,timestamp,1,year);
                                SaveInteger(Hash, timestamp, 2, month);
                                // title = "<1.24> 保存字符串 [C]"            SaveInteger(Hash,timestamp,3,day);
                                SaveStr(Hash, timestamp, 4, str);
                                return str;
                            }*/
        /*
                // title = "转换时间戳为具体时间 [NEW]"
                public static string KKAPIGetTimeDateFromTimestamp(int timestamp)
                {
                    set timestamp = IMaxBJ(timestamp, 0);
                    if (HaveSavedString(Hash, timestamp, 4)) then;
                    return LoadStr(Hash, timestamp, 4);
                    else
                    {
                        return DzGetTimeDateFromTimestamp(timestamp);
                    }
                }*/

        /*        // title = "获取时间戳年份 [NEW]"
                public static int KKAPIGetTimestampYear(int timestamp)
                {
                    set timestamp = IMaxBJ(timestamp, 0);
                    if (HaveSavedInteger(Hash, timestamp, 1) == false) then;
                    DzGetTimeDateFromTimestamp(timestamp);
                }
                return LoadInteger(Hash, timestamp,1);
                }*/

        /*       // title = "获取时间戳月份 [NEW]"
               public static int KKAPIGetTimestampMonth(int timestamp)
               {
                   set timestamp = IMaxBJ(timestamp, 0);
                   if (HaveSavedInteger(Hash, timestamp, 2) == false) then;
                   DzGetTimeDateFromTimestamp(timestamp);
               }
             return LoadInteger(Hash, timestamp,2);
           }*/

        /*        // title = "获取时间戳日份 [NEW]"
                public static int KKAPIGetTimestampDay(int timestamp)
        {
            set timestamp = IMaxBJ(timestamp, 0);
            if (HaveSavedInteger(Hash, timestamp, 3) == false) then;
            DzGetTimeDateFromTimestamp(timestamp);
        }
        return LoadInteger(Hash, timestamp, 3);
                }*/

        // title = "打印调试信息到平台日志 [NEW]"
        public static void DzWriteLog(string msg)
        {
            War3.CallNative(War3.GetNativeFunction("DzWriteLog"), msg);
        }

        // title = "获取当前漂浮文字的字体 [NEW]"
        public static string DzTextTagGetFont()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzTextTagGetFont"));
        }

        // title = "设置漂浮文字字体 [NEW]"
        public static void DzTextTagSetFont(string fileName)
        {
            War3.CallNative(War3.GetNativeFunction("DzTextTagSetFont"), fileName);
        }

        // title = "设置漂浮文字透明度 [NEW]"
        public static void DzTextTagSetStartAlpha(JTextTag t, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("DzTextTagSetStartAlpha"), t, alpha);
        }

        // title = "获取漂浮文字的阴影颜色 [NEW]"
        public static int DzTextTagGetShadowColor(JTextTag t)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzTextTagGetShadowColor"), t);
        }

        // title = "设置漂浮文字阴影颜色 [NEW]"
        public static void DzTextTagSetShadowColor(JTextTag t, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzTextTagSetShadowColor"), t, color);
        }

        // title = "获取单位组里单位数量 [NEW]"
        public static int DzGroupGetCount(JGroup g)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGroupGetCount"), g);
        }

        // title = "获取单位组里指定索引的单位 [NEW]"
        public static JUnit DzGroupGetUnitAt(JGroup g, int index)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("DzGroupGetUnitAt"), g, index);
        }

        // title = "创建幻象单位 [NEW]"
        public static JUnit DzUnitCreateIllusion(JPlayer p, int unitId, float x, float y, float face)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("DzUnitCreateIllusion"), p, unitId, x, y, face);
        }

        // title = "为单位创建幻象 [NEW]"
        public static JUnit DzUnitCreateIllusionFromUnit(JUnit u)
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("DzUnitCreateIllusionFromUnit"), u);
        }

        // title = "检查字符串是否包含指定的子字符串 [NEW]"
        public static bool DzStringContains(string s, string whichString, bool caseSensitive)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzStringContains"), s, whichString, caseSensitive);
        }

        // title = "字符串中查找子字符串并返回其位置 [NEW]"
        public static int DzStringFind(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFind"), s, whichString, off, caseSensitive);
        }

        // title = "检测字符串里第一个包含指定字符串里任意字符的位置 [NEW]"
        public static int DzStringFindFirstOf(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFindFirstOf"), s, whichString, off, caseSensitive);
        }

        // title = "检查字符串第一个不包含指定字符串里任意字符的位置 [NEW]"
        public static int DzStringFindFirstNotOf(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFindFirstNotOf"), s, whichString, off, caseSensitive);
        }

        // title = "从后往前查找字符串中包含指定字符串任意字符的所在位置 [NEW]"
        public static int DzStringFindLastOf(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFindLastOf"), s, whichString, off, caseSensitive);
        }

        // title = "从后往前查找字符串中不包含指定字符串任意字符的所在位置 [NEW]"
        public static int DzStringFindLastNotOf(string s, string whichString, int off, bool caseSensitive)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzStringFindLastNotOf"), s, whichString, off, caseSensitive);
        }

        // title = "删除字符串左边的空格 [NEW]"
        public static string DzStringTrimLeft(string s)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringTrimLeft"), s);
        }

        // title = "删除字符串右边的空格 [NEW]"
        public static string DzStringTrimRight(string s)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringTrimRight"), s);
        }

        // title = "删除字符串两边的空格 [NEW]"
        public static string DzStringTrim(string s)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringTrim"), s);
        }

        // title = "反转字符串 [NEW]"
        public static string DzStringReverse(string s)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringReverse"), s);
        }

        // title = "替换字符串 [NEW]"
        public static string DzStringReplace(string s, string whichString, string replaceWith, bool caseSensitive)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringReplace"), s, whichString, replaceWith, caseSensitive);
        }

        // title = "插入字符串 [NEW]"
        public static string DzStringInsert(string s, int whichPosition, string whichString)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzStringInsert"), s, whichPosition, whichString);
        }

        // title = "整数的2进制的位值 [NEW]"
        public static int DzBitGet(int i, int byteIndex)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitGet"), i, byteIndex);
        }

        // title = "设置整数的2进制的位值 [NEW]"
        public static int DzBitSet(int i, int byteIndex, int byteValue)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitSet"), i, byteIndex, byteValue);
        }

        // title = "整数的256进制的位值 [NEW]"
        public static int DzBitGetByte(int i, int byteIndex)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitGetByte"), i, byteIndex);
        }

        // title = "设置整数的256进制的位值 [NEW]"
        public static int DzBitSetByte(int i, int byteIndex, int byteValue)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitSetByte"), i, byteIndex, byteValue);
        }

        // title = "按位取反 [NEW]"
        public static int DzBitNot(int i)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitNot"), i);
        }

        // title = "按位与 [NEW]"
        public static int DzBitAnd(int a, int b)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitAnd"), a, b);
        }

        // title = "按位或 [NEW]"
        public static int DzBitOr(int a, int b)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitOr"), a, b);
        }

        // title = "按位异或 [NEW]"
        public static int DzBitXor(int a, int b)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitXor"), a, b);
        }

        // title = "按位左移 [NEW]"
        public static int DzBitShiftLeft(int i, int bitsToShift)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitShiftLeft"), i, bitsToShift);
        }

        // title = "按位右移 [NEW]"
        public static int DzBitShiftRight(int i, int bitsToShift)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitShiftRight"), i, bitsToShift);
        }

        // title = "4字节组合为整数 [NEW]"
        public static int DzBitToInt(int b1, int b2, int b3, int b4)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzBitToInt"), b1, b2, b3, b4);
        }

        // title = "对单位组添加命令到队列(无目标) [NEW]"
        public static bool DzQueueGroupImmediateOrderById(JGroup whichGroup, int order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueGroupImmediateOrderById"), whichGroup, order);
        }

        // title = "对单位组添加命令到队列(指定坐标) [NEW]"
        public static bool DzQueueGroupPointOrderById(JGroup whichGroup, int order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueGroupPointOrderById"), whichGroup, order, x, y);
        }

        public static bool DzQueueGroupTargetOrderById(JGroup whichGroup, int order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueGroupTargetOrderById"), whichGroup, order, targetWidget);
        }

        // title = "对单位添加命令到队列(无目标) [NEW]"
        public static bool DzQueueIssueImmediateOrderById(JUnit whichUnit, int order)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueImmediateOrderById"), whichUnit, order);
        }

        // title = "对单位添加命令到队列(指定坐标) [NEW]"
        public static bool DzQueueIssuePointOrderById(JUnit whichUnit, int order, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssuePointOrderById"), whichUnit, order, x, y);
        }

        public static bool DzQueueIssueTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueTargetOrderById"), whichUnit, order, targetWidget);
        }

        public static bool DzQueueIssueInstantPointOrderById(JUnit whichUnit, int order, float x, float y, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueInstantPointOrderById"), whichUnit, order, x, y, instantTargetWidget);
        }

        public static bool DzQueueIssueInstantTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget, JWidget instantTargetWidget)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueInstantTargetOrderById"), whichUnit, order, targetWidget, instantTargetWidget);
        }

        // title = "对单位添加建造命令到队列 [NEW]"
        public static bool DzQueueIssueBuildOrderById(JUnit whichPeon, int unitId, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueBuildOrderById"), whichPeon, unitId, x, y);
        }

        // title = "添加中介命令到队列(无目标) [NEW]"
        public static bool DzQueueIssueNeutralImmediateOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueNeutralImmediateOrderById"), forWhichPlayer, neutralStructure, unitId);
        }

        // title = "添加中介命令到队列(指定坐标) [NEW]"
        public static bool DzQueueIssueNeutralPointOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId, float x, float y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueNeutralPointOrderById"), forWhichPlayer, neutralStructure, unitId, x, y);
        }

        public static bool DzQueueIssueNeutralTargetOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzQueueIssueNeutralTargetOrderById"), forWhichPlayer, neutralStructure, unitId, target);
        }

        // title = "获取单位的命令数量 [NEW]"
        public static int DzUnitOrdersCount(JUnit u)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzUnitOrdersCount"), u);
        }

        // title = "清除单位命令队列 [NEW]"
        public static void DzUnitOrdersClear(JUnit u, bool onlyQueued)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitOrdersClear"), u, onlyQueued);
        }

        // title = "执行单位的命令队列 [NEW]"
        public static void DzUnitOrdersExec(JUnit u)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitOrdersExec"), u);
        }

        // title = "强制停止单位当前命令 [NEW]"
        public static void DzUnitOrdersForceStop(JUnit u, bool clearQueue)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitOrdersForceStop"), u, clearQueue);
        }

        // title = "反转单位命令队列 [NEW]"
        public static void DzUnitOrdersReverse(JUnit u)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitOrdersReverse"), u);
        }

        // title = "打开Excel文件 [NEW]"
        public static int DzXlsxOpen(string filePath)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxOpen"), filePath);
        }

        // title = "关闭工作表 [NEW]"
        public static bool DzXlsxClose(int docHandle)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzXlsxClose"), docHandle);
        }

        // title = "工作表的总行数 [NEW]"
        public static int DzXlsxWorksheetGetRowCount(int docHandle, string sheetName)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxWorksheetGetRowCount"), docHandle, sheetName);
        }

        // title = "工作表的总列数 [NEW]"
        public static int DzXlsxWorksheetGetColumnCount(int docHandle, string sheetName)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxWorksheetGetColumnCount"), docHandle, sheetName);
        }

        // title = "单元格的数据类型 [NEW]"
        public static int DzXlsxWorksheetGetCellType(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxWorksheetGetCellType"), docHandle, sheetName, row, column);
        }

        // title = "工作表的值（字符串） [NEW]"
        public static string DzXlsxWorksheetGetCellString(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzXlsxWorksheetGetCellString"), docHandle, sheetName, row, column);
        }

        // title = "工作表的值（整数） [NEW]"
        public static int DzXlsxWorksheetGetCellInteger(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzXlsxWorksheetGetCellInteger"), docHandle, sheetName, row, column);
        }

        // title = "工作表的值（布尔值） [NEW]"
        public static bool DzXlsxWorksheetGetCellBoolean(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzXlsxWorksheetGetCellBoolean"), docHandle, sheetName, row, column);
        }

        // title = "工作表的值（实数） [NEW]"
        public static float DzXlsxWorksheetGetCellFloat(int docHandle, string sheetName, int row, int column)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzXlsxWorksheetGetCellFloat"), docHandle, sheetName, row, column);
        }

        // title = "设置界面纹理坐标 [NEW]"
        public static void DzFrameSetTexCoord(int frame, float left, float top, float right, float bottom)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTexCoord"), frame, left, top, right, bottom);
        }

        // title = "技能 - 设置技能施法距离（通魔）"
        public static bool DzSetUnitAbilityRange(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityRange"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能施法距离（通魔）"
        public static float DzGetUnitAbilityRange(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityRange"), Unit, abil_code);
        }

        // title = "技能 - 设置技能施法范围（通魔）"
        public static bool DzSetUnitAbilityArea(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityArea"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能施法范围（通魔）"
        public static float DzGetUnitAbilityArea(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityArea"), Unit, abil_code);
        }

        // title = "技能 - 设置技能冷却时间（通魔）"
        public static bool DzSetUnitAbilityCool(JUnit Unit, int abil_code, float cool, float max_cool)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityCool"), Unit, abil_code, cool, max_cool);
        }

        // title = "技能 - 获取技能当前冷却时间（通魔）"
        public static float DzGetUnitAbilityCool(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityCool"), Unit, abil_code);
        }

        // title = "技能 - 获取技能最大冷却时间（通魔）"
        public static float DzGetUnitAbilityMaxCool(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMaxCool"), Unit, abil_code);
        }

        // title = "技能 - 设置技能数据A（通魔）"
        public static bool DzSetUnitAbilityDataA(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataA"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能数据A（通魔）"
        public static float DzGetUnitAbilityDataA(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataA"), Unit, abil_code);
        }

        // title = "技能 - 设置技能数据B（通魔）"
        public static bool DzSetUnitAbilityDataB(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataB"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能数据B（通魔）"
        public static float DzGetUnitAbilityDataB(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataB"), Unit, abil_code);
        }

        // title = "技能 - 设置技能数据C（通魔）"
        public static bool DzSetUnitAbilityDataC(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataC"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能数据C（通魔）"
        public static float DzGetUnitAbilityDataC(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataC"), Unit, abil_code);
        }

        // title = "技能 - 设置技能数据D（通魔）"
        public static bool DzSetUnitAbilityDataD(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataD"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能数据D（通魔）"
        public static float DzGetUnitAbilityDataD(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataD"), Unit, abil_code);
        }

        // title = "技能 - 设置技能数据E（通魔）"
        public static bool DzSetUnitAbilityDataE(JUnit Unit, int abil_code, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityDataE"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能数据E（通魔）"
        public static float DzGetUnitAbilityDataE(JUnit Unit, int abil_code)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityDataE"), Unit, abil_code);
        }

        // title = "技能 - 设置技能按钮位置（通魔）"
        public static bool DzSetUnitAbilityButtonPos(JUnit Unit, int abil_code, int x, int y)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityButtonPos"), Unit, abil_code, x, y);
        }

        // title = "技能 - 设置技能快捷键（通魔）"
        public static bool DzSetUnitAbilityHotkey(JUnit Unit, int abil_code, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityHotkey"), Unit, abil_code, key);
        }

        // title = "转化 - 目标允许整数转字符串"
        public static string DzConvertTargs2Str(int targs)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzConvertTargs2Str"), targs);
        }

        // title = "转化 - 目标允许字符串转整数"
        public static int DzConvertStr2Targs(string targs)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzConvertStr2Targs"), targs);
        }

        // title = "技能 - 设置技能目标允许（通魔）"
        public static bool DzSetUnitAbilityTargs(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityTargs"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能目标允许（通魔）"
        public static int DzGetUnitAbilityTargs(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityTargs"), Unit, abil_code);
        }

        // title = "技能 - 设置技能魔法消耗（通魔）"
        public static bool DzSetUnitAbilityCost(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityCost"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能魔法消耗（通魔）"
        public static int DzGetUnitAbilityCost(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityCost"), Unit, abil_code);
        }

        // title = "技能 - 设置技能等级要求（通魔）"
        public static bool DzSetUnitAbilityReqLevel(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityReqLevel"), Unit, abil_code, value);
        }

        // title = "技能 - 获取技能等级要求（通魔）"
        public static int DzGetUnitAbilityReqLevel(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityReqLevel"), Unit, abil_code);
        }

        // title = "技能 - 设置建造技能单位ID（象牙塔）"
        public static bool DzSetUnitAbilityUnitId(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityUnitId"), Unit, abil_code, value);
        }

        // title = "技能 - 获取建造技能单位ID（象牙塔）"
        public static int DzGetUnitAbilityUnitId(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityUnitId"), Unit, abil_code);
        }

        // title = "技能 - 设置建造技能命令ID（象牙塔）"
        public static bool DzSetUnitAbilityBuildOrderId(JUnit Unit, int abil_code, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityBuildOrderId"), Unit, abil_code, value);
        }

        // title = "技能 - 获取建造技能命令ID（象牙塔）"
        public static int DzGetUnitAbilityBuildOrderId(JUnit Unit, int abil_code)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityBuildOrderId"), Unit, abil_code);
        }

        // title = "技能 - 设置建造技能模型（象牙塔）"
        public static bool DzSetUnitAbilityBuildModel(JUnit Unit, int abil_code, string model_path, float model_scale)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityBuildModel"), Unit, abil_code, model_path, model_scale);
        }

        // title = "技能 - 判断单位是否拥有技能 (包含模版技能)"
        public static bool DzUnitHasAbility(JUnit Unit, int abil_code)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzUnitHasAbility"), Unit, abil_code);
        }

        // title = "技能 - 创建技能按钮控件"
        public static int KKCreateCommandButton()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("KKCreateCommandButton"));
        }

        // title = "技能按钮 - 删除技能按钮"
        public static void KKDestroyCommandButton(int btn)
        {
            War3.CallNative(War3.GetNativeFunction("KKDestroyCommandButton"), btn);
        }

        // title = "技能按钮 - 鼠标点击技能按钮 (无目标施法 或 激活目标指示器)"
        public static void KKCommandButtonClick(int btn, int mouse_type)
        {
            War3.CallNative(War3.GetNativeFunction("KKCommandButtonClick"), btn, mouse_type);
        }

        // title = "技能按钮 - 目标指示器点击目标单位"
        public static bool KKCommandTargetClick(int mouse_type, JWidget target)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("KKCommandTargetClick"), mouse_type, target);
        }

        // title = "技能按钮 - 目标指示器点击地面坐标"
        public static bool KKCommandTerrainClick(int mouse_type, float x, float y, float z)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("KKCommandTerrainClick"), mouse_type, x, y, z);
        }

        // title = "技能按钮 - 绑定单位技能"
        public static void KKSetCommandUnitAbility(int btn, JUnit Unit, int abil_code)
        {
            War3.CallNative(War3.GetNativeFunction("KKSetCommandUnitAbility"), btn, Unit, abil_code);
        }

        // title = "物品 - 获取物品颜色"
        public static int DzItemGetVertexColor(JItem Item)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzItemGetVertexColor"), Item);
        }

        // title = "物品 - 物品大小"
        public static void DzItemSetSize(JItem Item, float size)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemSetSize"), Item, size);
        }

        // title = "物品 - 获取物品大小"
        public static float DzItemGetSize(JItem Item)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzItemGetSize"), Item);
        }

        // title = "物品 - 模型按照X轴旋转"
        public static void DzItemMatRotateX(JItem Item, float x)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatRotateX"), Item, x);
        }

        // title = "物品 - 模型按照Y轴旋转"
        public static void DzItemMatRotateY(JItem Item, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatRotateY"), Item, y);
        }

        // title = "物品 - 模型按照Z轴旋转"
        public static void DzItemMatRotateZ(JItem Item, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatRotateZ"), Item, z);
        }

        // title = "物品 - 模型按照XYZ轴缩放"
        public static void DzItemMatScale(JItem Item, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatScale"), Item, x, y, z);
        }

        // title = "物品 - 模型重置旋转缩"
        public static void DzItemMatReset(JItem Item)
        {
            War3.CallNative(War3.GetNativeFunction("DzItemMatReset"), Item);
        }

        // title = "物品 - 当前选择的物品(异步)"
        public static JItem DzGetLastSelectedItem()
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("DzGetLastSelectedItem"));
        }

        public static void DzSetPariticle2Size(JAgent Widget, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetPariticle2Size"), Widget, scale);
        }

        // title = "单位 - 修改单位碰撞体积"
        public static void DzSetUnitCollisionSize(JUnit Unit, float size)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitCollisionSize"), Unit, size);
        }

        // title = "单位 - 获取单位的碰撞体积"
        public static float DzGetUnitCollisionSize(JUnit Unit)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitCollisionSize"), Unit);
        }

        public static void DzSetWidgetTexture(JAgent Handle, string TexturePath, int ReplaceId)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetWidgetTexture"), Handle, TexturePath, ReplaceId);
        }

        // title = "单位 - 修改单位选择圈缩放"
        public static void DzSetUnitSelectScale(JUnit Unit, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitSelectScale"), Unit, scale);
        }

        // title = "单位 - 设置单位是否忽略点击"
        public static void DzSetUnitHitIgnore(JUnit Unit, bool ignore)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitHitIgnore"), Unit, ignore);
        }

        // title = "特效 - 特效绑定特效"
        public static void DzEffectBindEffect(JAgent Handle, string AttachName, JEffect eff)
        {
            War3.CallNative(War3.GetNativeFunction("DzEffectBindEffect"), Handle, AttachName, eff);
        }

        // title = "转化 - 整数为技能ID"
        public static int KKConvertInt2AbilId(int i)
        {
            return i;
        }

        // title = "转化 - 技能ID为整数"
        public static int KKConvertAbilId2Int(int i)
        {
            return i;
        }

        // title = "转化 - 转整数为颜色"
        public static int KKConvertInt2Color(int i)
        {
            return i;
        }

        // title = "转化 - 转颜色为整数"
        public static int KKConvertColor2Int(int i)
        {
            return i;
        }

        // title = "界面 - 设置Frame控件忽略点击事件"
        public static void DzFrameSetIgnoreTrackEvents(int frame, bool ignore)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetIgnoreTrackEvents"), frame, ignore);
        }

        // title = "界面 - 创建ui模型控件"
        public static int DzFrameAddModel(int parent_frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameAddModel"), parent_frame);
        }

        // title = "界面 - ui模型 - 设置模型文件"
        public static void DzFrameSetModel2(int model_frame, string model_file, int team_color_id)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModel2"), model_frame, model_file, team_color_id);
        }

        // title = "界面 - ui模型 - 添加绑定特效"
        public static int DzFrameAddModelEffect(int model_frame, string attach_point, string model_file)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameAddModelEffect"), model_frame, attach_point, model_file);
        }

        // title = "界面 - ui模型 - 移除绑定特效"
        public static void DzFrameRemoveModelEffect(int model_frame, int effect_frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameRemoveModelEffect"), model_frame, effect_frame);
        }

        // title = "界面 - ui模型 - 播放动画指定索引"
        public static void DzFrameSetModelAnimationByIndex(int model_frame, int anim_index)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelAnimationByIndex"), model_frame, anim_index);
        }

        // title = "界面 - ui模型 - 播放动画指定动画名"
        public static void DzFrameSetModelAnimation(int model_frame, string animation)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelAnimation"), model_frame, animation);
        }

        // title = "界面 - ui模型 - 设置场景内镜头源点"
        public static void DzFrameSetModelCameraSource(int model_frame, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelCameraSource"), model_frame, x, y, z);
        }

        // title = "界面 - ui模型 - 设置场景内镜头目标点"
        public static void DzFrameSetModelCameraTarget(int model_frame, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelCameraTarget"), model_frame, x, y, z);
        }

        // title = "界面 - ui模型 - 设置缩放大小"
        public static void DzFrameSetModelSize(int model_frame, float size)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelSize"), model_frame, size);
        }

        // title = "界面 - ui模型 - 获取缩放大小"
        public static float DzFrameGetModelSize(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelSize"), model_frame);
        }

        // title = "界面 - ui模型 - 设置场景内的坐标(X Y Z)"
        public static void DzFrameSetModelPosition(int model_frame, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelPosition"), model_frame, x, y, z);
        }

        // title = "界面 - ui模型 - 设置场景内的坐标X轴"
        public static void DzFrameSetModelX(int model_frame, float x)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelX"), model_frame, x);
        }

        // title = "界面 - ui模型 - 获取场景内的坐标X轴"
        public static float DzFrameGetModelX(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelX"), model_frame);
        }

        // title = "界面 - ui模型 - 设置场景内的坐标Y轴"
        public static void DzFrameSetModelY(int model_frame, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelY"), model_frame, y);
        }

        // title = "界面 - ui模型 - 获取场景内的坐标Y轴"
        public static float DzFrameGetModelY(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelY"), model_frame);
        }

        // title = "界面 - ui模型 - 设置场景内的坐标Z轴"
        public static void DzFrameSetModelZ(int model_frame, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelZ"), model_frame, z);
        }

        // title = "界面 - ui模型 - 获取场景内的坐标Z轴"
        public static float DzFrameGetModelZ(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelZ"), model_frame);
        }

        // title = "界面 - ui模型 - 设置动画播放速度"
        public static void DzFrameSetModelSpeed(int model_frame, float speed)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelSpeed"), model_frame, speed);
        }

        // title = "界面 - ui模型 - 获取动画播放速度"
        public static float DzFrameGetModelSpeed(int model_frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetModelSpeed"), model_frame);
        }

        // title = "界面 - ui模型 - 设置矩阵缩放"
        public static void DzFrameSetModelScale(int model_frame, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelScale"), model_frame, x, y, z);
        }

        // title = "界面 - ui模型 - 设置矩阵重置"
        public static void DzFrameSetModelMatReset(int model_frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelMatReset"), model_frame);
        }

        // title = "界面 - ui模型 - 设置矩阵旋转X轴"
        public static void DzFrameSetModelRotateX(int model_frame, float x)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelRotateX"), model_frame, x);
        }

        // title = "界面 - ui模型 - 设置矩阵旋转Y轴"
        public static void DzFrameSetModelRotateY(int model_frame, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelRotateY"), model_frame, y);
        }

        // title = "界面 - ui模型 - 设置矩阵旋转Z轴"
        public static void DzFrameSetModelRotateZ(int model_frame, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelRotateZ"), model_frame, z);
        }

        // title = "界面 - ui模型 - 设置模型颜色"
        public static void DzFrameSetModelColor(int model_frame, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelColor"), model_frame, color);
        }

        // title = "界面 - ui模型 - 获取颜色"
        public static int DzFrameGetModelColor(int model_frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetModelColor"), model_frame);
        }

        // title = "界面 - ui模型 - 替换模型id贴图"
        public static void DzFrameSetModelTexture(int model_frame, string texture_file, int replace_texutre_id)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelTexture"), model_frame, texture_file, replace_texutre_id);
        }

        // title = "界面 - ui模型 - 设置粒子2缩放大小"
        public static void DzFrameSetModelParticle2Size(int model_frame, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModelParticle2Size"), model_frame, scale);
        }

        // title = "界面 - 获取游戏外界面底层"
        public static int DzGetGlueUI()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetGlueUI"));
        }

        // title = "界面 - 获取鼠标控件"
        public static int DzFrameGetMouse()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetMouse"));
        }

        // title = "界面 - 获取控件绑定的整数"
        public static int DzFrameGetContext(int frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetContext"), frame);
        }

        // title = "获取 Frame 的 名称"
        public static string DzFrameGetName(int frame)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzFrameGetName"), frame);
        }

        // title = "界面 - 设置控件全局名字跟绑定整数"
        public static void DzFrameSetNameContext(int frame, string name, int context)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetNameContext"), frame, name, context);
        }

        // title = "界面 - 设置文本控件字间距"
        public static void DzFrameSetTextFontSpacing(int text_frame, float spacing)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTextFontSpacing"), text_frame, spacing);
        }

        // title = "界面 - 获取技能/物品按钮的冷却模型控件"
        public static int KKCommandGetCooldownModel(int cmd_btn)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("KKCommandGetCooldownModel"), cmd_btn);
        }

        // title = "界面 - 设置技能/物品按钮的冷却模型缩放大小"
        public static void KKCommandSetCooldownModelSize(int cmd_btn, float size)
        {
            War3.CallNative(War3.GetNativeFunction("KKCommandSetCooldownModelSize"), cmd_btn, size);
        }

        // title = "界面 - 设置技能/物品按钮的冷却模型缩放指定宽高比例"
        public static void KKCommandSetCooldownModelSize2(int cmd_btn, float width, float height)
        {
            War3.CallNative(War3.GetNativeFunction("KKCommandSetCooldownModelSize2"), cmd_btn, width, height);
        }

        // title = "物品 - 玩家当前选择的物品(同步)"
        public static JItem DzGetPlayerLastSelectedItem(JPlayer p)
        {
            return War3.CallNative<JItem>(War3.GetNativeFunction("DzGetPlayerLastSelectedItem"), p);
        }

        // title = "获取当前缓存模型的数量"
        public static int DzGetCacheModelCount()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetCacheModelCount"));
        }

        // title = "游戏 - 限制最高帧数"
        public static void DzSetMaxFps(int max_fps)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetMaxFps"), max_fps);
        }

        // title = "允许查看指定单位技能"
        public static void DzEnableDrawSkillPanel(JUnit u, bool is_enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzEnableDrawSkillPanel"), u, is_enable);
        }

        // title = "允许查看指定玩家单位技能"
        public static void DzEnableDrawSkillPanelByPlayer(JPlayer p, bool is_enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzEnableDrawSkillPanelByPlayer"), p, is_enable);
        }

        // title = "特效 - 设置特效迷雾可见"
        public static void DzSetEffectFogVisible(JEffect eff, bool is_visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectFogVisible"), eff, is_visible);
        }

        // title = "特效 - 设置特效黑色阴影可见"
        public static void DzSetEffectMaskVisible(JEffect eff, bool is_visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetEffectMaskVisible"), eff, is_visible);
        }

        // title = "世界坐标 - 绑定Frame到单位实时位置"
        public static void DzFrameBindWidget(int frame, JWidget u, float world_x, float world_y, float world_z, float screen_x, float screen_y, bool fog_visible, bool unit_visible, bool dead_visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameBindWidget"), frame, u, world_x, world_y, world_z, screen_x, screen_y, fog_visible, unit_visible, dead_visible);
        }

        // title = "世界坐标 - 绑定Frame到世界坐标实时位置"
        public static void DzFrameBindWorldPos(int frame, float world_x, float world_y, float world_z, float screen_x, float screen_y, bool fog_visible)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameBindWorldPos"), frame, world_x, world_y, world_z, screen_x, screen_y, fog_visible);
        }

        // title = "世界坐标 - 解除Frame的绑定"
        public static void DzFrameUnBind(int frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameUnBind"), frame);
        }

        // title = "世界坐标 - 绑定Frame到物品实时位置"
        public static void KKFrameBindItem(int frame, JWidget u, float world_x, float world_y, float world_z, float screen_x, float screen_y, bool fog_visible, bool item_visible)
        {
            DzFrameBindWidget(frame, u, world_x, world_y, world_z, screen_x, screen_y, fog_visible, item_visible, false);
        }

        // title = "界面 - 屏蔽所有单位指向UI跟血条"
        public static void DzDisableUnitPreselectUi()
        {
            War3.CallNative(War3.GetNativeFunction("DzDisableUnitPreselectUi"));
        }

        // title = "界面 - 屏蔽所有物品指向UI"
        public static void DzDisableItemPreselectUi()
        {
            War3.CallNative(War3.GetNativeFunction("DzDisableItemPreselectUi"));
        }

        // title = "界面 - 获取低于控制台的底层Frame"
        public static int DzFrameGetLowerLevelFrame()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetLowerLevelFrame"));
        }

        // title = "界面 - 设置复选框勾选状态"
        public static void DzFrameSetCheckBoxState(int check_box_frame, bool check)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetCheckBoxState"), check_box_frame, check);
        }

        // title = "界面 - 获取复选框勾选状态"
        public static bool DzFrameGetCheckBoxState(int check_box_frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameGetCheckBoxState"), check_box_frame);
        }

        // title = "界面 - 获取控件是否焦点"
        public static bool DzFrameIsFocus(int frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameIsFocus"), frame);
        }

        // title = "界面 - 设置编辑框激活状态"
        public static void DzFrameSetEditBoxActive(int frame, bool is_active)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetEditBoxActive"), frame, is_active);
        }

        // title = "界面 - 设置编辑框禁用输入法"
        public static void DzFrameSetEditBoxDisableIme(int frame, bool is_disable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetEditBoxDisableIme"), frame, is_disable);
        }

        // title = "硬件 - 当前游戏是否窗口化模式"
        public static bool DzIsWindowMode()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsWindowMode"));
        }

        // title = "判断游戏窗口是否处于活动状态"
        public static bool DzIsWindowActive()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsWindowActive"));
        }

        // title = "硬件 - 设置游戏窗口位置"
        public static void DzWindowSetPoint(int x, int y)
        {
            War3.CallNative(War3.GetNativeFunction("DzWindowSetPoint"), x, y);
        }

        // title = "硬件 - 设置游戏窗口大小"
        public static void DzWindowSetSize(int width, int height)
        {
            War3.CallNative(War3.GetNativeFunction("DzWindowSetSize"), width, height);
        }

        // title = "硬件 - 获取屏幕设备宽度"
        public static int DzGetSystemMetricsWidth()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetSystemMetricsWidth"));
        }

        // title = "硬件 - 获取屏幕设备高度"
        public static int DzGetSystemMetricsHeight()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetSystemMetricsHeight"));
        }

        // title = "装饰物 - 获取当前地形装饰物数量"
        public static int DzGetDoodadsCount()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetDoodadsCount"));
        }

        // title = "装饰物 - 设置地形装饰物矩阵缩放"
        public static void DzSetDoodadsMatScale(int doodads_index, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatScale"), doodads_index, x, y, z);
        }

        // title = "装饰物 - 设置地形装饰物矩阵旋转X轴"
        public static void DzSetDoodadsMatRotateX(int doodads_index, float x)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatRotateX"), doodads_index, x);
        }

        // title = "装饰物 - 设置地形装饰物矩阵旋转Y轴"
        public static void DzSetDoodadsMatRotateY(int doodads_index, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatRotateY"), doodads_index, y);
        }

        // title = "装饰物 - 设置装饰物矩阵旋转Z轴"
        public static void DzSetDoodadsMatRotateZ(int doodads_index, float z)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatRotateZ"), doodads_index, z);
        }

        // title = "装饰物 - 设置地形装饰物矩阵重置"
        public static void DzSetDoodadsMatReset(int doodads_index)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetDoodadsMatReset"), doodads_index);
        }

        // title = "技能 - 设置技能图标"
        public static bool DzSetUnitAbilityArt(JUnit u, int abil_id, string art_path)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityArt"), u, abil_id, art_path);
        }

        // title = "技能 - 获取技能图标"
        public static string DzGetUnitAbilityArt(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilityArt"), u, abil_id);
        }

        // title = "技能 - 设置技能提示"
        public static bool DzSetUnitAbilityTip(JUnit u, int abil_id, string tip)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityTip"), u, abil_id, tip);
        }

        // title = "技能 - 获取技能提示"
        public static string DzGetUnitAbilityTip(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilityTip"), u, abil_id);
        }

        // title = "技能 - 设置技能提示扩展"
        public static bool DzSetUnitAbilityUberTip(JUnit u, int abil_id, string ubertip)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityUberTip"), u, abil_id, ubertip);
        }

        // title = "技能 - 获取技能提示扩展"
        public static string DzGetUnitAbilityUberTip(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilityUberTip"), u, abil_id);
        }

        // title = "技能 - 设置刷新数据"
        public static bool DzSetUnitAbilityUpdate(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityUpdate"), u, abil_id);
        }

        // title = "技能 - 设置技能命令ID"
        public static bool DzSetUnitAbilityOrderId(JUnit u, int abil_id, int order_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityOrderId"), u, abil_id, order_id);
        }

        // title = "技能 - 获取技能命令ID"
        public static int DzGetUnitAbilityOrderId(JUnit u, int abil_id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityOrderId"), u, abil_id);
        }

        // title = "技能 - 设置魔法书的技能列表"
        public static bool DzSetUnitAbilitySpellBookList(JUnit u, int abil_id, string abil_list, bool save_cooldown)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilitySpellBookList"), u, abil_id, abil_list, save_cooldown);
        }

        // title = "技能 - 获取魔法书的技能列表"
        public static string DzGetUnitAbilitySpellBookList(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilitySpellBookList"), u, abil_id);
        }

        // title = "技能 - 设置技能投射物模型"
        public static bool DzSetUnitAbilityMissileArt(JUnit u, int abil_id, string missile_art)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileArt"), u, abil_id, missile_art);
        }

        // title = "技能 - 获取技能投射物模型"
        public static string DzGetUnitAbilityMissileArt(JUnit u, int abil_id)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetUnitAbilityMissileArt"), u, abil_id);
        }

        // title = "技能 - 设置技能投射物速度"
        public static bool DzSetUnitAbilityMissileSpeed(JUnit u, int abil_id, float missile_speed)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileSpeed"), u, abil_id, missile_speed);
        }

        // title = "技能 - 获取技能投射物速度"
        public static float DzGetUnitAbilityMissileSpeed(JUnit u, int abil_id)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMissileSpeed"), u, abil_id);
        }

        // title = "技能 - 设置技能投射物弧度"
        public static bool DzSetUnitAbilityMissileArc(JUnit u, int abil_id, float missile_arc)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileArc"), u, abil_id, missile_arc);
        }

        // title = "技能 - 获取技能投射物弧度"
        public static float DzGetUnitAbilityMissileArc(JUnit u, int abil_id)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMissileArc"), u, abil_id);
        }

        // title = "技能 - 设置技能投射物允许自导"
        public static bool DzSetUnitAbilityMissileHoming(JUnit u, int abil_id, bool missile_homing)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileHoming"), u, abil_id, missile_homing);
        }

        // title = "技能 - 获取技能投射物允许自导"
        public static bool DzGetUnitAbilityMissileHoming(JUnit u, int abil_id)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzGetUnitAbilityMissileHoming"), u, abil_id);
        }

        // title = "技能 - 设置技能投射物数量 (弹幕攻击)"
        public static bool DzSetUnitAbilityMissileCount(JUnit u, int abil_id, int missile_count)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileCount"), u, abil_id, missile_count);
        }

        // title = "技能 - 获取技能投射物数量 (弹幕攻击)"
        public static int DzGetUnitAbilityMissileCount(JUnit u, int abil_id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitAbilityMissileCount"), u, abil_id);
        }

        // title = "技能 - 设置技能投射物伤害 (弹幕攻击)"
        public static bool DzSetUnitAbilityMissileDamage(JUnit u, int abil_id, float damage, float max_damage, JAttackType atktp, JDamageType dmgtp)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzSetUnitAbilityMissileDamage"), u, abil_id, damage, max_damage, atktp, dmgtp);
        }

        // title = "技能 - 获取技能投射物伤害 (弹幕攻击)"
        public static float DzGetUnitAbilityMissileDamage(JUnit u, int abil_id)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMissileDamage"), u, abil_id);
        }

        // title = "技能 - 获取技能投射物最大伤害 (弹幕攻击)"
        public static float DzGetUnitAbilityMissileMaxDamage(JUnit u, int abil_id)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetUnitAbilityMissileMaxDamage"), u, abil_id);
        }

    }
}
