using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Frame;
using War3Frame.Library.Api;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace War3Frame.Library.Api
{
    public static class DzApi
    {

        // title = "获取鼠标在游戏内的坐标X"        
        public static float DzGetMouseTerrainX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetMouseTerrainX"));
        }

        // title = "获取鼠标在游戏内的坐标Y"
        public static float DzGetMouseTerrainY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetMouseTerrainY"));
        }

        // title = "获取鼠标在游戏内的坐标Z"
        public static float DzGetMouseTerrainZ()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetMouseTerrainZ"));
        }

        // title = "鼠标是否在游戏内"
        public static bool DzIsMouseOverUI()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsMouseOverUI"));
        }

        // title = "获取鼠标在屏幕的坐标X"
        public static int DzGetMouseX()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseX"));
        }

        // title = "获取鼠标在屏幕的坐标Y"
        public static int DzGetMouseY()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseY"));
        }

        // title = "获取鼠标游戏窗口坐标X"
        public static int DzGetMouseXRelative()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseXRelative"));
        }

        // title = "获取鼠标游戏窗口坐标Y"
        public static int DzGetMouseYRelative()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseYRelative"));
        }

        // title = "设置鼠标的坐标"
        public static void DzSetMousePos(int x, int y)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetMousePos"), x, y);
        }

        public static void DzTriggerRegisterMouseEvent(JTrigger trig, int btn, int status, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseEvent"), trig, btn, status, sync, func);
        }

        public static void DzTriggerRegisterMouseEventByCode(JTrigger trig, int btn, int status, bool sync, JCode funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseEventByCode"), trig, btn, status, sync, funcHandle);
        }

        public static void DzTriggerRegisterKeyEvent(JTrigger trig, int key, int status, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterKeyEvent"), trig, key, status, sync, func);
        }

        public static void DzTriggerRegisterKeyEventByCode(JTrigger trig, int key, int status, bool sync, JCode funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterKeyEventByCode"), trig, key, status, sync, funcHandle);
        }

        public static void DzTriggerRegisterMouseWheelEvent(JTrigger trig, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseWheelEvent"), trig, sync, func);
        }

        public static void DzTriggerRegisterMouseWheelEventByCode(JTrigger trig, bool sync, JCode funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseWheelEventByCode"), trig, sync, funcHandle);
        }

        public static void DzTriggerRegisterMouseMoveEvent(JTrigger trig, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseMoveEvent"), trig, sync, func);
        }

        public static void DzTriggerRegisterMouseMoveEventByCode(JTrigger trig, bool sync, JCode funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseMoveEventByCode"), trig, sync, funcHandle);
        }

        // title = "事件响应 - 获取触发的按键"
        public static int DzGetTriggerKey()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetTriggerKey"));
        }

        // title = "事件响应 - 获取滚轮变化值"
        public static int DzGetWheelDelta()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWheelDelta"));
        }

        // title = "判断按键是否按下"
        public static bool DzIsKeyDown(int iKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsKeyDown"), iKey);
        }

        // title = "事件响应 - 获取触发硬件事件的玩家"
        public static JPlayer DzGetTriggerKeyPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("DzGetTriggerKeyPlayer"));
        }

        // title = "获取war3窗口宽度"
        public static int DzGetWindowWidth()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWindowWidth"));
        }

        // title = "获取魔兽窗口高度"
        public static int DzGetWindowHeight()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWindowHeight"));
        }

        // title = "获取魔兽窗口X坐标"
        public static int DzGetWindowX()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWindowX"));
        }

        // title = "获取魔兽窗口Y坐标"
        public static int DzGetWindowY()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWindowY"));
        }

        public static void DzTriggerRegisterWindowResizeEvent(JTrigger trig, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterWindowResizeEvent"), trig, sync, func);
        }

        public static void DzTriggerRegisterWindowResizeEventByCode(JTrigger trig, bool sync, JCode funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterWindowResizeEventByCode"), trig, sync, funcHandle);
        }

        // title = "判断游戏窗口是否处于活动状态"
        public static bool DzIsWindowActive()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsWindowActive"));
        }

        // title = "设置可破坏物位置 [BZAPI]"
        public static void DzDestructablePosition(JDestructable d, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzDestructablePosition"), d, x, y);
        }

        // title = "设置单位位置 - 本地调用 [BZAPI]"
        public static void DzSetUnitPosition(JUnit whichUnit, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitPosition"), whichUnit, x, y);
        }

        // title = "异步执行函数"
        public static void DzExecuteFunc(string funcName)
        {
            War3.CallNative(War3.GetNativeFunction("DzExecuteFunc"), funcName);
        }

        // title = "获取鼠标指向的单位(异步)"
        public static JUnit DzGetUnitUnderMouse()
        {
            return War3.CallNative<JUnit>(War3.GetNativeFunction("DzGetUnitUnderMouse"));
        }

        // title = "替换单位贴图 [BZAPI]"
        public static void DzSetUnitTexture(JUnit whichUnit, string path, int texId)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitTexture"), whichUnit, path, texId);
        }

        // title = "设置内存数值"
        public static void DzSetMemory(int address, float value)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetMemory"), address, value);
        }

        // title = "替换单位类型 [BZAPI]"
        public static void DzSetUnitID(JUnit whichUnit, int id)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitID"), whichUnit, id);
        }

        // title = "替换单位模型 [BZAPI]"
        public static void DzSetUnitModel(JUnit whichUnit, string path)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitModel"), whichUnit, path);
        }

        // title = "原生 - 设置小地图背景贴图"
        public static void DzSetWar3MapMap(string map)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetWar3MapMap"), map);
        }

        // title = "获取客户端语言"
        public static string DzGetLocale()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetLocale"));
        }

        // title = "获取升级所需经验 "
        public static int DzGetUnitNeededXP(JUnit whichUnit, int level)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitNeededXP"), whichUnit, level);
        }

        // title = "数据同步"
        public static void DzTriggerRegisterSyncData(JTrigger trig, string prefix, bool server)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterSyncData"), trig, prefix, server);
        }

        // title = "同步游戏数据"
        public static void DzSyncData(string prefix, string data)
        {
            War3.CallNative(War3.GetNativeFunction("DzSyncData"), prefix, data);
        }

        // title = "事件响应 - 获取同步的数据前缀 "
        public static string DzGetTriggerSyncPrefix()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetTriggerSyncPrefix"));
        }

        // title = "事件响应 - 获取同步的数据"
        public static string DzGetTriggerSyncData()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetTriggerSyncData"));
        }

        // title = "事件响应 - 获取同步数据的玩家"
        public static JPlayer DzGetTriggerSyncPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("DzGetTriggerSyncPlayer"));
        }

        // title = "同步游戏数据（指定长度）"
        public static void DzSyncBuffer(string prefix, string data, int dataLen)
        {
            War3.CallNative(War3.GetNativeFunction("DzSyncBuffer"), prefix, data, dataLen);
        }

        // title = "同步游戏数据（立即）"
        public static void DzSyncDataImmediately(string prefix, string data)
        {
            War3.CallNative(War3.GetNativeFunction("DzSyncDataImmediately"), prefix, data);
        }

        // title = "原生 - 隐藏界面元素"
        public static void DzFrameHideInterface()
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameHideInterface"));
        }

        // title = "原生 - 修改游戏渲染黑边范围"
        public static void DzFrameEditBlackBorders(float upperHeight, float bottomHeight)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameEditBlackBorders"), upperHeight, bottomHeight);
        }

        // title = "原生 - 单位大头像"
        public static int DzFrameGetPortrait()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetPortrait"));
        }

        // title = "原生 - 小地图"
        public static int DzFrameGetMinimap()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetMinimap"));
        }

        // title = "原生 - 技能按钮"
        public static int DzFrameGetCommandBarButton(int row, int column)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButton"), row, column);
        }

        // title = "原生 - 英雄按钮"
        public static int DzFrameGetHeroBarButton(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetHeroBarButton"), buttonId);
        }

        // title = "原生 - 英雄血条"
        public static int DzFrameGetHeroHPBar(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetHeroHPBar"), buttonId);
        }

        // title = "原生 - 英雄蓝条"
        public static int DzFrameGetHeroManaBar(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetHeroManaBar"), buttonId);
        }

        // title = "原生 - 物品栏按钮"
        public static int DzFrameGetItemBarButton(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetItemBarButton"), buttonId);
        }

        // title = "原生 - 小地图按钮"
        public static int DzFrameGetMinimapButton(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetMinimapButton"), buttonId);
        }

        // title = "原生 - 界面按钮"
        public static int DzFrameGetUpperButtonBarButton(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetUpperButtonBarButton"), buttonId);
        }

        // title = "原生 - 鼠标提示"
        public static int DzFrameGetTooltip()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetTooltip"));
        }

        // title = "原生 - 玩家聊天信息框"
        public static int DzFrameGetChatMessage()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetChatMessage"));
        }

        // title = "原生 - 系统消息框"
        public static int DzFrameGetUnitMessage()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetUnitMessage"));
        }

        // title = "原生 - 上方消息框"
        public static int DzFrameGetTopMessage()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetTopMessage"));
        }

        // title = "取 RGBA 色值"
        public static int DzGetColor(int r, int g, int b, int a)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetColor"), r, g, b, a);
        }

        public static void DzFrameSetUpdateCallback(string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetUpdateCallback"), func);
        }

        public static void DzFrameSetUpdateCallbackByCode(JCode funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetUpdateCallbackByCode"), funcHandle);
        }

        // title = "显示/隐藏"
        public static void DzFrameShow(int frame, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameShow"), frame, enable);
        }

        // title = "新建Frame"
        public static int DzCreateFrame(string frame, int parent, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzCreateFrame"), frame, parent, id);
        }

        public static int DzCreateSimpleFrame(string frame, int parent, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzCreateSimpleFrame"), frame, parent, id);
        }

        // title = "销毁"
        public static void DzDestroyFrame(int frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzDestroyFrame"), frame);
        }

        // title = "加载Toc文件列表"
        public static void DzLoadToc(string fileName)
        {
            War3.CallNative(War3.GetNativeFunction("DzLoadToc"), fileName);
        }

        // title = "设置相对位置"
        public static void DzFrameSetPoint(int frame, int point, int relativeFrame, int relativePoint, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetPoint"), frame, point, relativeFrame, relativePoint, x, y);
        }

        // title = "设置绝对位置"
        public static void DzFrameSetAbsolutePoint(int frame, int point, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetAbsolutePoint"), frame, point, x, y);
        }

        // title = "清空所有锚点"
        public static void DzFrameClearAllPoints(int frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameClearAllPoints"), frame);
        }

        // title = "启用/禁用"
        public static void DzFrameSetEnable(int name, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetEnable"), name, enable);
        }

        // title = "注册UI事件回调(func name)[观战、录像不响应]"
        public static void DzFrameSetScript(int frame, int eventId, string func, bool sync)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScript"), frame, eventId, func, sync);
        }

        // title = "注册UI事件回调(func JHandle)[观战、录像不响应]"
        public static void DzFrameSetScriptByCode(int frame, int eventId, JCode funcHandle, bool sync)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptByCode"), frame, eventId, funcHandle, sync);
        }

        // title = "注册UI事件回调-异步(func JHandle)[观战、录像不响应][new]"
        public static void DzFrameSetScriptBlock(int frame, int eventId, JCode funcHandle, bool sync)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptBlock"), frame, eventId, funcHandle, sync);
        }

        // title = "注册UI事件回调-异步(func name)[观战、录像可响应][new]"
        public static void DzFrameSetScriptAsync(int frame, int eventId, string funcName)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptAsync"), frame, eventId, funcName);
        }

        // title = "注册UI事件回调-异步(func JHandle)[观战、录像可响应][new]"
        public static void DzFrameSetScriptByCodeAsync(int frame, int eventId, JCode func)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptByCodeAsync"), frame, eventId, func);
        }

        // title = "注册UI事件回调-异步(func JHandle)[观战、录像可响应][new]"
        public static void DzFrameSetScriptBlockAsync(int frame, int eventId, JCode func)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptBlockAsync"), frame, eventId, func);
        }

        // title = "事件响应 - 获取触发ui的玩家"
        public static JPlayer DzGetTriggerUIEventPlayer()
        {
            return War3.CallNative<JPlayer>(War3.GetNativeFunction("DzGetTriggerUIEventPlayer"));
        }

        // title = "事件响应 - 触发的Frame"
        public static int DzGetTriggerUIEventFrame()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetTriggerUIEventFrame"));
        }

        // title = "获取子Frame"
        public static int DzFrameFindByName(string name, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameFindByName"), name, id);
        }

        // title = "获取子SimpleFrame"
        public static int DzSimpleFrameFindByName(string name, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzSimpleFrameFindByName"), name, id);
        }

        // title = "获取子SimpleFontString"
        public static int DzSimpleFontStringFindByName(string name, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzSimpleFontStringFindByName"), name, id);
        }

        // title = "获取子SimpleTexture"
        public static int DzSimpleTextureFindByName(string name, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzSimpleTextureFindByName"), name, id);
        }

        // title = "原生 - 游戏UI"
        public static int DzGetGameUI()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetGameUI"));
        }

        // title = "点击"
        public static void DzClickFrame(int frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzClickFrame"), frame);
        }

        // title = "原生 - 修改屏幕比例(FOV)"
        public static void DzSetCustomFovFix(float value)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetCustomFovFix"), value);
        }

        // title = "开启/关闭宽屏模式"
        public static void DzEnableWideScreen(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzEnableWideScreen"), enable);
        }

        // title = "设置文本"
        public static void DzFrameSetText(int frame, string text)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetText"), frame, text);
        }

        // title = "获取 Frame 内的文字"
        public static string DzFrameGetText(int frame)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzFrameGetText"), frame);
        }

        // title = "设置字数限制"
        public static void DzFrameSetTextSizeLimit(int frame, int size)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTextSizeLimit"), frame, size);
        }

        // title = "获取 Frame 的 字数限制"
        public static int DzFrameGetTextSizeLimit(int frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetTextSizeLimit"), frame);
        }

        public static void DzFrameSetTextColor(int frame, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTextColor"), frame, color);
        }

        // title = "鼠标所在的Frame控件指针"
        public static int DzGetMouseFocus()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseFocus"));
        }

        // title = "移动所有锚点到Frame"
        public static bool DzFrameSetAllPoints(int frame, int relativeFrame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameSetAllPoints"), frame, relativeFrame);
        }

        // title = "设置焦点"
        public static bool DzFrameSetFocus(int frame, bool enable)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameSetFocus"), frame, enable);
        }

        // title = "设置模型"
        public static void DzFrameSetModel(int frame, string modelFile, int modelType, int flag)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModel"), frame, modelFile, modelType, flag);
        }

        // title = "控件是否启用"
        public static bool DzFrameGetEnable(int frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameGetEnable"), frame);
        }

        // title = "设置透明度(0-255)"
        public static void DzFrameSetAlpha(int frame, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetAlpha"), frame, alpha);
        }

        // title = "获取 Frame 的 透明度(0-255)"
        public static int DzFrameGetAlpha(int frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetAlpha"), frame);
        }

        // title = "设置动画"
        public static void DzFrameSetAnimate(int frame, int animId, bool autocast)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetAnimate"), frame, animId, autocast);
        }

        // title = "设置动画进度"
        public static void DzFrameSetAnimateOffset(int frame, float offset)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetAnimateOffset"), frame, offset);
        }

        // title = "设置贴图"
        public static void DzFrameSetTexture(int frame, string texture, int flag)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTexture"), frame, texture, flag);
        }

        // title = "设置缩放"
        public static void DzFrameSetScale(int frame, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScale"), frame, scale);
        }

        // title = "设置提示"
        public static void DzFrameSetTooltip(int frame, int tooltip)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTooltip"), frame, tooltip);
        }

        // title = "限制鼠标移动"
        public static void DzFrameCageMouse(int frame, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameCageMouse"), frame, enable);
        }

        // title = "获取当前值"
        public static float DzFrameGetValue(int frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetValue"), frame);
        }

        // title = "设置最大/最小值"
        public static void DzFrameSetMinMaxValue(int frame, float minValue, float maxValue)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetMinMaxValue"), frame, minValue, maxValue);
        }

        // title = "设置步进值"
        public static void DzFrameSetStepValue(int frame, float step)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetStepValue"), frame, step);
        }

        // title = "设置当前值"
        public static void DzFrameSetValue(int frame, float value)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetValue"), frame, value);
        }

        // title = "设置大小"
        public static void DzFrameSetSize(int frame, float w, float h)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetSize"), frame, w, h);
        }

        // title = "新建Frame [Tag]"
        public static int DzCreateFrameByTagName(string frameType, string name, int parent, string template, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzCreateFrameByTagName"), frameType, name, parent, template, id);
        }

        // title = "设置颜色"
        public static void DzFrameSetVertexColor(int frame, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetVertexColor"), frame, color);
        }

        public static void DzOriginalUIAutoResetPoint(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzOriginalUIAutoResetPoint"), enable);
        }

        // title = "设置优先级"
        public static void DzFrameSetPriority(int frame, int priority)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetPriority"), frame, priority);
        }

        // title = "设置父窗口 "
        public static void DzFrameSetParent(int frame, int parent)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetParent"), frame, parent);
        }

        // title = "获取 Frame 的 高度"
        public static float DzFrameGetHeight(int frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetHeight"), frame);
        }

        // title = "设置字体 "
        public static void DzFrameSetFont(int frame, string fileName, float height, int flag)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetFont"), frame, fileName, height, flag);
        }

        // title = "获取 Frame 的 Parent"
        public static int DzFrameGetParent(int frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetParent"), frame);
        }

        // title = "设置对齐方式"
        public static void DzFrameSetTextAlignment(int frame, int align)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTextAlignment"), frame, align);
        }

        // title = "获取 Frame 的 名称"
        public static string DzFrameGetName(int frame)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzFrameGetName"), frame);
        }

        // title = "获取魔兽窗口宽度"
        public static int DzGetClientWidth()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetClientWidth"));
        }

        // title = "获取魔兽窗口高度"
        public static int DzGetClientHeight()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetClientHeight"));
        }

        // title = "控件是否显示"
        public static bool DzFrameIsVisible(int frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameIsVisible"), frame);
        }

        // title = "显示/隐藏SimpleFrame"
        public static void DzSimpleFrameShow(int frame, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzSimpleFrameShow"), frame, enable);
        }

        // title = "追加文本"
        public static void DzFrameAddText(int frame, string text)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameAddText"), frame, text);
        }

        // title = "沉默单位"
        public static void DzUnitSilence(JUnit whichUnit, bool disable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitSilence"), whichUnit, disable);
        }

        // title = "禁用攻击"
        public static void DzUnitDisableAttack(JUnit whichUnit, bool disable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitDisableAttack"), whichUnit, disable);
        }

        // title = "禁用道具"
        public static void DzUnitDisableInventory(JUnit whichUnit, bool disable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitDisableInventory"), whichUnit, disable);
        }

        // title = "刷新小地图"
        public static void DzUpdateMinimap()
        {
            War3.CallNative(War3.GetNativeFunction("DzUpdateMinimap"));
        }

        // title = "修改单位透明度"
        public static void DzUnitChangeAlpha(JUnit whichUnit, int alpha, bool forceUpdate)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitChangeAlpha"), whichUnit, alpha, forceUpdate);
        }

        // title = "修改单位选中状态"
        public static void DzUnitSetCanSelect(JUnit whichUnit, bool state)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitSetCanSelect"), whichUnit, state);
        }

        // title = "修改单位是否可以被设置为目标"
        public static void DzUnitSetTargetable(JUnit whichUnit, bool state)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitSetTargetable"), whichUnit, state);
        }

        // title = "保存内存数据"
        public static void DzSaveMemoryCache(string cache)
        {
            War3.CallNative(War3.GetNativeFunction("DzSaveMemoryCache"), cache);
        }

        // title = "读取内存数据"
        public static string DzGetMemoryCache()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetMemoryCache"));
        }

        // title = "设置加速倍率"
        public static void DzSetSpeed(float ratio)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetSpeed"), ratio);
        }

        // title = "转换地图坐标为屏幕坐标-异步"
        public static bool DzConvertWorldPosition(float x, float y, float z, JCode callback)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzConvertWorldPosition"), x, y, z, callback);
        }

        // title = "获取转换后的屏幕 X 坐标"
        public static float DzGetConvertWorldPositionX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetConvertWorldPositionX"));
        }

        // title = "获取转换后的屏幕 Y 坐标"
        public static float DzGetConvertWorldPositionY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetConvertWorldPositionY"));
        }

        // title = "创建技能按钮 "
        public static int DzCreateCommandButton(int parent, string icon, string name, string desc)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzCreateCommandButton"), parent, icon, name, desc);
        }

        // title = "注册鼠标事件"
        public static void DzTriggerRegisterMouseEventTrg(JTrigger trg, int status, int btn)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterMouseEvent(trg, btn, status, true, null);
        }

        // title = "键盘事件"
        public static void DzTriggerRegisterKeyEventTrg(JTrigger trg, int status, int btn)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterKeyEvent(trg, btn, status, true, null);
        }

        // title = "鼠标移动事件"
        public static void DzTriggerRegisterMouseMoveEventTrg(JTrigger trg)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterMouseMoveEvent(trg, true, null);
        }

        // title = "鼠标滚轮事件"
        public static void DzTriggerRegisterMouseWheelEventTrg(JTrigger trg)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterMouseWheelEvent(trg, true, null);
        }

        // title = "窗口大小变化事件"
        public static void DzTriggerRegisterWindowResizeEventTrg(JTrigger trg)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterWindowResizeEvent(trg, true, null);
        }

        // title = "转换 Frame 为 整数"
        public static int DzF2I(int i)
        {
            return i;
        }

        // title = "转换 整数 为 Frame"
        public static int DzI2F(int i)
        {
            return i;
        }

        // title = "转换 按键 为 整数"
        public static int DzK2I(int i)
        {
            return i;
        }

        // title = "转换 整数 为 按键"
        public static int DzI2K(int i)
        {
            return i;
        }

        // title = "玩家获得地图商城道具事件（实时）"
        public static void DzTriggerRegisterMallItemSyncData(JTrigger trig)
        {
            DzTriggerRegisterSyncData(trig, "DZMIA", true);
        }

        // title = "玩家消耗地图商城道具事件（实时）"
        public static void DzTriggerRegisterMallItemConsumeEvent(JTrigger trig)
        {
            DzTriggerRegisterSyncData(trig, "DZMIC", true);
        }

        // title = "玩家删除地图商城道具事件（实时）"
        public static void DzTriggerRegisterMallItemRemoveEvent(JTrigger trig)
        {
            DzTriggerRegisterSyncData(trig, "DZMID", true);
        }

        // title = "事件响应 - 触发的商城道具事件的玩家"
        public static JPlayer DzGetTriggerMallItemPlayer()
        {
            // title = "事件响应 - 获取同步数据的玩家"            
            return DzGetTriggerSyncPlayer();
        }

        // title = "事件响应 - 触发的商城道具（实时）"
        public static string DzGetTriggerMallItem()
        {
            return DzGetTriggerSyncData();
        }



        // title = "玩家是否拥有地图商城道具"
        public static bool DzAPI_Map_HasMallItem(JPlayer whichPlayer, string key)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzAPI_Map_HasMallItem"), whichPlayer, key);
        }

        // title = "玩家地图等级"
        public static int DzAPI_Map_GetMapLevel(JPlayer whichPlayer)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzAPI_Map_GetMapLevel"), whichPlayer);
        }

        public static int RequestExtraIntegerData(int dataType, JPlayer whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("RequestExtraIntegerData"), dataType, whichPlayer, param1, param2, param3, param4, param5, param6);
        }

        public static bool RequestExtraBooleanData(int dataType, JPlayer whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("RequestExtraBooleanData"), dataType, whichPlayer, param1, param2, param3, param4, param5, param6);
        }

        public static string RequestExtraStringData(int dataType, JPlayer whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("RequestExtraStringData"), dataType, whichPlayer, param1, param2, param3, param4, param5, param6);
        }

        public static float RequestExtraRealData(int dataType, JPlayer whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("RequestExtraRealData"), dataType, whichPlayer, param1, param2, param3, param4, param5, param6);
        }

        // title = "保存服务器存档"
        public static bool DzAPI_Map_SaveServerValue(JPlayer whichPlayer, string key, string value)
        {
            return RequestExtraBooleanData(4, whichPlayer, key, value, false, 0, 0, 0);
        }

        // title = "读取服务器存储的数据"
        public static string DzAPI_Map_GetServerValue(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(5, whichPlayer, key, null, false, 0, 0, 0);
        }

        // title = "本局游戏的开始时间"
        public static int DzAPI_Map_GetGameStartTime()
        {
            return RequestExtraIntegerData(11, null, null, null, false, 0, 0, 0);
        }

        // title = "本局游戏是否天梯排位赛"
        public static bool DzAPI_Map_IsRPGLadder()
        {
            return RequestExtraBooleanData(12, null, null, null, false, 0, 0, 0);
        }

        // title = "本局游戏的地图模式"
        public static int DzAPI_Map_GetMatchType()
        {
            return RequestExtraIntegerData(13, null, null, null, false, 0, 0, 0);
        }

        // title = "上报房间内显示的数据"
        public static void DzAPI_Map_Stat_SetStat(JPlayer whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(7, whichPlayer, key, value, false, 0, 0, 0);
        }

        // title = "天梯提交字符串数据"
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

        // title = "玩家天梯等级"
        public static int DzAPI_Map_GetLadderLevel(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(14, whichPlayer, null, null, false, 0, 0, 0);
        }

        // title = "获取玩家身份类型"
        public static bool KKApiPlayerIdentityType(JPlayer whichPlayer, int id)
        {
            return RequestExtraBooleanData(92, whichPlayer, null, null, false, id, 0, 0);
        }

        // title = "玩家是否平台认证的职业选手"
        public static bool DzAPI_Map_IsRedVIP(JPlayer whichPlayer)
        {
            return KKApiPlayerIdentityType(whichPlayer, 4);
        }

        // title = "玩家是否平台认证的主播"
        public static bool DzAPI_Map_IsBlueVIP(JPlayer whichPlayer)
        {
            return KKApiPlayerIdentityType(whichPlayer, 3);
        }

        // title = "玩家天梯排名"
        public static int DzAPI_Map_GetLadderRank(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(17, whichPlayer, null, null, false, 0, 0, 0);
        }

        // title = "玩家在地图等级排行榜上的排名"
        public static int DzAPI_Map_GetMapLevelRank(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(18, whichPlayer, null, null, false, 0, 0, 0);
        }

        // title = "玩家在公会的职责【废弃】"
        public static int DzAPI_Map_GetGuildRole(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(20, whichPlayer, null, null, false, 0, 0, 0);
        }

        // title = "本局游戏是否处于RPG游戏大厅"
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

        // title = "地图配置参数"
        public static string DzAPI_Map_GetMapConfig(string key)
        {
            return RequestExtraStringData(21, null, key, null, false, 0, 0, 0);
        }

        // title = "保存服务器存档组"
        public static bool DzAPI_Map_SavePublicArchive(JPlayer whichPlayer, string key, string value)
        {
            return RequestExtraBooleanData(31, whichPlayer, key, value, false, 0, 0, 0);
        }

        // title = "读取服务器存档组"
        public static string DzAPI_Map_GetPublicArchive(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(32, whichPlayer, key, null, false, 0, 0, 0);
        }

        // title = "使用地图商城道具（局数型）"
        public static void DzAPI_Map_UseConsumablesItem(JPlayer whichPlayer, string key)
        {
            RequestExtraIntegerData(33, whichPlayer, key, null, false, 0, 0, 0);
        }

        // title = "触发BOSS击杀"
        public static void DzAPI_Map_OrpgTrigger(JPlayer whichPlayer, string key)
        {
            RequestExtraIntegerData(28, whichPlayer, key, null, false, 0, 0, 0);
        }

        // title = "BOSS击杀后的掉落内容"
        public static string DzAPI_Map_GetServerArchiveDrop(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(27, whichPlayer, key, null, false, 0, 0, 0);
        }

        // title = "BOSS击杀后的掉落数量"
        public static int DzAPI_Map_GetServerArchiveEquip(JPlayer whichPlayer, string key)
        {
            return RequestExtraIntegerData(26, whichPlayer, key, null, false, 0, 0, 0);
        }

        public static int DzAPI_Map_GetPlatformVIP(JPlayer whichPlayer)
        {
            return RequestExtraIntegerData(30, whichPlayer, null, null, false, 0, 0, 0);
        }

        // title = "玩家是否平台尊享会员"
        public static bool DzAPI_Map_IsPlatformVIP(JPlayer whichPlayer)
        {
            return DzAPI_Map_GetPlatformVIP(whichPlayer) > 0;
        }

        // title = "读取全局存档"
        public static string DzAPI_Map_Global_GetStoreString(string key)
        {
            return RequestExtraStringData(36, JassApi.GetLocalPlayer(), key, null, false, 0, 0, 0);
        }

        // title = "保存全局存档"
        public static void DzAPI_Map_Global_StoreString(string key, string value)
        {
            RequestExtraBooleanData(37, GetLocalPlayer(), key, value, false, 0, 0, 0);
        }

        // title = "全局存档变化事件"
        public static void DzAPI_Map_Global_ChangeMsg(JTrigger trig)
        {
            DzTriggerRegisterSyncData(trig, "DZGAU", true);
        }

        // title = "读取服务器存档（区分大小写）"
        public static string DzAPI_Map_ServerArchive(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(38, whichPlayer, key, null, false, 0, 0, 0);
        }

        // title = "保存服务器存档（区分大小写）"
        public static void DzAPI_Map_SaveServerArchive(JPlayer whichPlayer, string key, string value)
        {
            RequestExtraBooleanData(39, whichPlayer, key, value, false, 0, 0, 0);
        }

        // title = "本局游戏是否快速匹配"
        public static bool DzAPI_Map_IsRPGQuickMatch()
        {
            return RequestExtraBooleanData(40, null, null, null, false, 0, 0, 0);
        }

        // title = "玩家地图商城道具剩余数量"
        public static int DzAPI_Map_GetMallItemCount(JPlayer whichPlayer, string key)
        {
            return RequestExtraIntegerData(41, whichPlayer, key, null, false, 0, 0, 0);
        }

        // title = "使用地图商城道具（数量型）"
        public static bool DzAPI_Map_ConsumeMallItem(JPlayer whichPlayer, string key, int count)
        {
            return RequestExtraBooleanData(42, whichPlayer, key, null, false, count, 0, 0);
        }

        // title = "开启/关闭游戏内辅助功能"
        public static bool DzAPI_Map_EnablePlatformSettings(JPlayer whichPlayer, int option, bool enable)
        {
            return RequestExtraBooleanData(43, whichPlayer, null, null, enable, option, 0, 0);
        }

        // title = "玩家服务器存档是否读取成功"
        public static bool GetPlayerServerValueSuccess(JPlayer whichPlayer)
        {
            if (DzAPI_Map_GetServerValueErrorCode(whichPlayer) == 0) then;
            return true;
            else
            {
                return false;
            }
        }

        // title = "服务器存储整数（区分大小写）"
        public static void DzAPI_Map_StoreIntegerEX(JPlayer whichPlayer, string key, int value)
        {
            set key = "I" + key;
            RequestExtraBooleanData(39, whichPlayer, key, I2S(value), false, 0, 0, 0);
            set key = null;
            set whichPlayer = null;
        }

        // title = "获取服务器存储的整数（区分大小写）"
        public static int DzAPI_Map_GetStoredIntegerEX(JPlayer whichPlayer, string key)
        {
            local integer value;
            set key = "I" + key;
            set value = S2I(RequestExtraStringData(38, whichPlayer, key, null, false, 0, 0, 0));
            set key = null;
            set whichPlayer = null;
            return value;
        }

        // title = "保存整数变量至服务器"
        public static void DzAPI_Map_StoreInteger(JPlayer whichPlayer, string key, int value)
        {
            set key = "I" + key;
            DzAPI_Map_SaveServerValue(whichPlayer, key, I2S(value));
            set key = null;
            set whichPlayer = null;
        }

        // title = "读取服务器上的整数变量"
        public static int DzAPI_Map_GetStoredInteger(JPlayer whichPlayer, string key)
        {
            local integer value;
            set key = "I" + key;
            set value = S2I(DzAPI_Map_GetServerValue(whichPlayer, key));
            set key = null;
            set whichPlayer = null;
            return value;
        }

        // title = "玩家在地图自定义排行榜上的排名"
        public static int DzAPI_Map_CommentTotalCount1(JPlayer whichPlayer, int id)
        {
            return RequestExtraIntegerData(52, whichPlayer, null, null, false, id, 0, 0);
        }

        // title = "保存实数变量至服务器"
        public static void DzAPI_Map_StoreReal(JPlayer whichPlayer, string key, float value)
        {
            set key = "R" + key;
            DzAPI_Map_SaveServerValue(whichPlayer, key, R2S(value));
            set key = null;
            set whichPlayer = null;
        }

        // title = "读取服务器上的实数变量"
        public static float DzAPI_Map_GetStoredReal(JPlayer whichPlayer, string key)
        {
            local real value;
            set key = "R" + key;
            set value = S2R(DzAPI_Map_GetServerValue(whichPlayer, key));
            set key = null;
            set whichPlayer = null;
            return value;
        }

        // title = "保存布尔值变量至服务器"
        public static void DzAPI_Map_StoreBoolean(JPlayer whichPlayer, string key, bool value)
        {
            set key = "B" + key;
            // title = "保存服务器存档"            if(value)then;
            DzAPI_Map_SaveServerValue(whichPlayer, key, "1");
            else
            {
                DzAPI_Map_SaveServerValue(whichPlayer, key, "0");
            }
            set key = null;
            set whichPlayer = null;
        }

        // title = "读取服务器上的布尔变量"
        public static bool DzAPI_Map_GetStoredBoolean(JPlayer whichPlayer, string key)
        {
            local boolean value;
            set key = "B" + key;
            set key = DzAPI_Map_GetServerValue(whichPlayer, key);
            if (key == "1") then;
            set value = true;
            else
            {
                set value = false;
            }
            set key = null;
            set whichPlayer = null;
            return value;
        }

        // title = "保存字符串变量至服务器"
        public static void DzAPI_Map_StoreString(JPlayer whichPlayer, string key, string value)
        {
            set key = "S" + key;
            DzAPI_Map_SaveServerValue(whichPlayer, key, value);
            set key = null;
            set whichPlayer = null;
        }

        // title = "读取服务器上的字符串变量"
        public static string DzAPI_Map_GetStoredString(JPlayer whichPlayer, string key)
        {
            // title = "读取服务器存储的数据"            return DzAPI_Map_GetServerValue(whichPlayer,"S"+key);
        }

        // title = "服务器存储字符串（区分大小写）"
        public static void DzAPI_Map_StoreStringEX(JPlayer whichPlayer, string key, string value)
        {
            set key = "S" + key;
            RequestExtraBooleanData(39, whichPlayer, key, value, false, 0, 0, 0);
            set key = null;
            set whichPlayer = null;
        }

        // title = "获取服务器存储的字符串（区分大小写）"
        public static string DzAPI_Map_GetStoredStringEX(JPlayer whichPlayer, string key)
        {
            return RequestExtraStringData(38, whichPlayer, "S" + key, null, false, 0, 0, 0);
        }

        // title = "读取服务器存储的单位类型"
        public static int DzAPI_Map_GetStoredUnitType(JPlayer whichPlayer, string key)
        {
            local integer value;
            set key = "I" + key;
            set value = S2I(DzAPI_Map_GetServerValue(whichPlayer, key));
            set key = null;
            set whichPlayer = null;
            return value;
        }

        // title = "读取服务器存储的技能类型"
        public static int DzAPI_Map_GetStoredAbilityId(JPlayer whichPlayer, string key)
        {
            local integer value;
            set key = "I" + key;
            set value = S2I(DzAPI_Map_GetServerValue(whichPlayer, key));
            set key = null;
            set whichPlayer = null;
            return value;
        }

        // title = "清理服务器数据"
        public static void DzAPI_Map_FlushStoredMission(JPlayer whichPlayer, string key)
      // title = "保存服务器存档"        {
            DzAPI_Map_SaveServerValue(whichPlayer, key, null);
        set key = null;
        set whichPlayer = null;
    }

        // title = "天梯提交整数数据"
        public static void DzAPI_Map_Ladder_SubmitIntegerData(JPlayer whichPlayer, string key, int value)
      // title = "天梯提交字符串数据"        {
            DzAPI_Map_Ladder_SetStat(whichPlayer, key, I2S(value));
    }

        // title = "天梯提交单位类型数据"
        public static void DzAPI_Map_Stat_SubmitUnitIdData(JPlayer whichPlayer, string key, int value)
{
    if (value == 0) then;
    // title = "天梯提交字符串数据"            //call DzAPI_Map_Ladder_SetStat(whichPlayer,key,"0");
    else
    {
        DzAPI_Map_Ladder_SetStat(whichPlayer, key, I2S(value));
    }
}

public static void DzAPI_Map_Stat_SubmitUnitData(JPlayer whichPlayer, string key, JUnit value)
      // title = "天梯提交单位类型数据"        {
            DzAPI_Map_Stat_SubmitUnitIdData(whichPlayer, key, GetUnitTypeId(value));
        }

        // title = "天梯提交技能数据"
        public static void DzAPI_Map_Ladder_SubmitAblityIdData(JPlayer whichPlayer, string key, int value)
{
    if (value == 0) then;
    call DzAPI_Map_Ladder_SetStat(whichPlayer, key,"0");
    else
    {
        DzAPI_Map_Ladder_SetStat(whichPlayer, key, I2S(value));
    }
}

// title = "天梯提交物品数据"
public static void DzAPI_Map_Ladder_SubmitItemIdData(JPlayer whichPlayer, string key, int value)
{
    local string S;
    if (value == 0) then;
    set S = "0";
            else
    {
        set S = I2S(value);
        DzAPI_Map_Ladder_SetStat(whichPlayer, key, S);
    }
    // title = "天梯提交字符串数据"            //call DzAPI_Map_Ladder_SetStat(whichPlayer,key,S);
    set S = null;
    set whichPlayer = null;
}

public static void DzAPI_Map_Ladder_SubmitItemData(JPlayer whichPlayer, string key, JItem value)
// title = "天梯提交物品数据"
{
    DzAPI_Map_Ladder_SubmitItemIdData(whichPlayer, key, GetItemTypeId(value));
}

// title = "天梯提交布尔值数据"
public static void DzAPI_Map_Ladder_SubmitBooleanData(JPlayer whichPlayer, string key, bool value)
{
    if (value) then;
    DzAPI_Map_Ladder_SetStat(whichPlayer, key, "1");
            else
    {
        DzAPI_Map_Ladder_SetStat(whichPlayer, key, "0");
    }
}

// title = "天梯提交获得称号"
public static void DzAPI_Map_Ladder_SubmitTitle(JPlayer whichPlayer, string value)
{
    DzAPI_Map_Ladder_SetStat(whichPlayer, value, "1");
}

// title = "天梯提交玩家排名"
public static void DzAPI_Map_Ladder_SubmitPlayerRank(JPlayer whichPlayer, int value)
{
    DzAPI_Map_Ladder_SetPlayerStat(whichPlayer, "RankIndex", I2S(value));
}

// title = "天梯设置玩家额外分"
public static void DzAPI_Map_Ladder_SubmitPlayerExtraExp(JPlayer whichPlayer, int value)
      // title = "天梯提交字符串数据"
      /{
    DzAPI_Map_Ladder_SetStat(whichPlayer, "ExtraExp", I2S(value));
}

// title = "玩家累计游戏局数"
public static int DzAPI_Map_PlayedGames(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(45, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "评论次数【废弃】"
public static int DzAPI_Map_CommentCount(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(46, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "玩家好友数量【废弃】"
public static int DzAPI_Map_FriendCount(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(47, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "玩家是否平台认证的鉴赏家[废弃]"
public static bool DzAPI_Map_IsConnoisseur(JPlayer whichPlayer)
{
    return RequestExtraBooleanData(48, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "玩家是否当前地图作者"
public static bool DzAPI_Map_IsAuthor(JPlayer whichPlayer)
{
    return RequestExtraBooleanData(50, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "地图评论次数"
public static int DzAPI_Map_CommentTotalCount()
{
    return RequestExtraIntegerData(51, null, null, null, false, 0, 0, 0);
}

// title = "上报埋点数据"
public static void DzAPI_Map_Statistics(JPlayer whichPlayer, string eventKey, string eventType, int value)
{
    RequestExtraBooleanData(34, whichPlayer, eventKey, eventType, false, value, 0, 0);
}

// title = "是否回流/收藏过地图的用户"
public static bool DzAPI_Map_Returns(JPlayer whichPlayer, int label)
{
    return RequestExtraBooleanData(53, whichPlayer, null, null, false, label, 0, 0);
}

// title = "玩家签到天数"
public static int DzAPI_Map_ContinuousCount(JPlayer whichPlayer, int id)
{
    return RequestExtraIntegerData(54, whichPlayer, null, null, false, id, 0, 0);
}

// title = "玩家是否为真实玩家"
public static bool DzAPI_Map_IsPlayer(JPlayer whichPlayer)
{
    return RequestExtraBooleanData(55, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "玩家累计游戏时长"
public static int DzAPI_Map_MapsTotalPlayed(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(56, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "玩家在指定地图的地图等级"
public static int DzAPI_Map_MapsLevel(JPlayer whichPlayer, int mapId)
{
    return RequestExtraIntegerData(57, whichPlayer, null, null, false, mapId, 0, 0);
}

// title = "玩家在指定地图累计消耗平台金币【已废弃】"
public static int DzAPI_Map_MapsConsumeGold(JPlayer whichPlayer, int mapId)
{
    return RequestExtraIntegerData(58, whichPlayer, null, null, false, mapId, 0, 0);
}

// title = "玩家在指定地图的平台木材消耗【已废弃】"
public static int DzAPI_Map_MapsConsumeLumber(JPlayer whichPlayer, int mapId)
{
    return RequestExtraIntegerData(59, whichPlayer, null, null, false, mapId, 0, 0);
}

// title = "玩家在指定地图累计消费金额区间（1~199）"
public static bool DzAPI_Map_MapsConsumeLv1(JPlayer whichPlayer, int mapId)
{
    return RequestExtraBooleanData(60, whichPlayer, null, null, false, mapId, 0, 0);
}

// title = "玩家在指定地图累计消费金额区间（200~499）"
public static bool DzAPI_Map_MapsConsumeLv2(JPlayer whichPlayer, int mapId)
{
    return RequestExtraBooleanData(61, whichPlayer, null, null, false, mapId, 0, 0);
}

// title = "玩家在指定地图累计消费金额区间（500~999）"
public static bool DzAPI_Map_MapsConsumeLv3(JPlayer whichPlayer, int mapId)
{
    return RequestExtraBooleanData(62, whichPlayer, null, null, false, mapId, 0, 0);
}

// title = "玩家在指定地图累计消费金额区间（1000+）"
public static bool DzAPI_Map_MapsConsumeLv4(JPlayer whichPlayer, int mapId)
{
    return RequestExtraBooleanData(63, whichPlayer, null, null, false, mapId, 0, 0);
}

// title = "玩家是否装备指定平台装饰"
public static bool DzAPI_Map_IsPlayerUsingSkin(JPlayer whichPlayer, int skinType, int id)
{
    return RequestExtraBooleanData(64, whichPlayer, null, null, false, skinType, id, 0);
}

// title = "玩家在地图社区上的互动数据"
public static int DzAPI_Map_GetForumData(JPlayer whichPlayer, int whichData)
{
    return RequestExtraIntegerData(65, whichPlayer, null, null, false, whichData, 0, 0);
}

// title = "玩家标记"
public static bool DzAPI_Map_PlayerFlags(JPlayer whichPlayer, int label)
{
    return RequestExtraBooleanData(53, whichPlayer, null, null, false, label, 0, 0);
}

// title = "玩家抽取指定地图宝箱次数"
public static int DzAPI_Map_GetLotteryUsedCountEx(JPlayer whichPlayer, int index)
{
    return RequestExtraIntegerData(68, whichPlayer, null, null, false, index, 0, 0);
}

// title = "玩家抽取地图宝箱总次数"
public static int DzAPI_Map_GetLotteryUsedCount(JPlayer whichPlayer)
{
    // title = "玩家抽取指定地图宝箱次数"            return DzAPI_Map_GetLotteryUsedCountEx(whichPlayer,0)+DzAPI_Map_GetLotteryUsedCountEx(whichPlayer,1)+DzAPI_Map_GetLotteryUsedCountEx(whichPlayer,2);
}

// title = "打开地图商城道具购买界面"
public static bool DzAPI_Map_OpenMall(JPlayer whichPlayer, string whichkey)
{
    return RequestExtraBooleanData(66, whichPlayer, whichkey, null, false, 0, 0, 0);
}

// title = "上报本局游戏玩家数据"
public static void DzAPI_Map_GameResult_CommitData(JPlayer whichPlayer, string key, string value)
{
    RequestExtraIntegerData(69, whichPlayer, key, value, false, 0, 0, 0);
}

// title = "上报本局游戏玩家称号"
public static void DzAPI_Map_GameResult_CommitTitle(JPlayer whichPlayer, string value)
{
    DzAPI_Map_GameResult_CommitData(whichPlayer, value, "1");
    set whichPlayer = null;
    set value = null;
}

// title = "上报本局游戏玩家排名"
public static void DzAPI_Map_GameResult_CommitPlayerRank(JPlayer whichPlayer, int value)
{
    DzAPI_Map_GameResult_CommitData(whichPlayer, "RankIndex", I2S(value));
    set whichPlayer = null;
    set value = 0;
}

// title = "上报本局游戏模式"
public static void DzAPI_Map_GameResult_CommitGameMode(string value)
{
    DzAPI_Map_GameResult_CommitData(GetLocalPlayer(), "InnerGameMode", value);
    set value = null;
}

// title = "上报本局游戏结果"
public static void DzAPI_Map_GameResult_CommitGameResult(JPlayer whichPlayer, int value)
{
    DzAPI_Map_GameResult_CommitData(whichPlayer, "GameResult", I2S(value));
    set whichPlayer = null;
}

// title = "上报本局游戏结果（不结束游戏）"
public static void DzAPI_Map_GameResult_CommitGameResultNoEnd(JPlayer whichPlayer, int value)
{
    DzAPI_Map_GameResult_CommitData(whichPlayer, "GameResultNoEnd", I2S(value));
    set whichPlayer = null;
}

// title = "玩家本局游戏距上一局游戏的时间差"
public static int DzAPI_Map_GetSinceLastPlayedSeconds(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(70, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "使用U币快速购买地图商城道具"
public static bool DzAPI_Map_QuickBuy(JPlayer whichPlayer, string key, int count, int seconds)
{
    return RequestExtraBooleanData(72, whichPlayer, key, null, false, count, seconds, 0);
}

// title = "关闭U币快速购买界面"
public static bool DzAPI_Map_CancelQuickBuy(JPlayer whichPlayer)
{
    return RequestExtraBooleanData(73, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "本局游戏是否处于平台自测服"
public static bool DzAPI_Map_IsMapTest()
{
    return RequestExtraBooleanData(74, null, null, null, false, 0, 0, 0);
}

// title = "玩家地图商城道具是否读取成功"
public static bool DzAPI_Map_PlayerLoadedItems(JPlayer whichPlayer)
{
    return RequestExtraBooleanData(77, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "自定义排行榜上榜人数"
public static int DzAPI_Map_CustomRankCount(int id)
{
    return RequestExtraIntegerData(78, null, null, null, false, id, 0, 0);
}

// title = "自定义排行榜上的玩家昵称"
public static string DzAPI_Map_CustomRankPlayerName(int id, int ranking)
{
    return RequestExtraStringData(79, null, null, null, false, id, ranking, 0);
}

// title = "自定义排行榜上的玩家数值"
public static int DzAPI_Map_CustomRankValue(int id, int ranking)
{
    return RequestExtraIntegerData(80, null, null, null, false, id, ranking, 0);
}

// title = "玩家在KK对战平台的完整昵称"
public static string DzAPI_Map_GetPlayerUserName(JPlayer whichPlayer)
{
    return RequestExtraStringData(81, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "获取服务器存档限制余额"
public static int KKApiGetServerValueLimitLeft(JPlayer whichPlayer, string key)
{
    return RequestExtraIntegerData(82, whichPlayer, key, null, false, 0, 0, 0);
}

// title = "【随机只读存档】生成随机数"
public static bool KKApiRequestBackendLogic(JPlayer whichPlayer, string key, string groupkey)
{
    return RequestExtraBooleanData(83, whichPlayer, key, groupkey, false, 0, 0, 0);
}

// title = "【随机只读存档】判断随机数是否存在"
public static bool KKApiCheckBackendLogicExists(JPlayer whichPlayer, string key)
{
    return RequestExtraBooleanData(84, whichPlayer, key, null, false, 0, 0, 0);
}

// title = "【随机只读存档】读取随机数的值"
public static int KKApiGetBackendLogicIntResult(JPlayer whichPlayer, string key)
{
    return RequestExtraIntegerData(85, whichPlayer, key, null, false, 0, 0, 0);
}

// title = "【随机只读存档】读取随机数的值"
public static string KKApiGetBackendLogicStrResult(JPlayer whichPlayer, string key)
{
    return RequestExtraStringData(86, whichPlayer, key, null, false, 0, 0, 0);
}

// title = "【随机只读存档】读取随机数的生成时间"
public static int KKApiGetBackendLogicUpdateTime(JPlayer whichPlayer, string key)
{
    return RequestExtraIntegerData(87, whichPlayer, key, null, false, 0, 0, 0);
}

// title = "【随机只读存档】读取随机数的组ID"
public static string KKApiGetBackendLogicGroup(JPlayer whichPlayer, string key)
{
    return RequestExtraStringData(88, whichPlayer, key, null, false, 0, 0, 0);
}

// title = "【随机只读存档】删除随机数"
public static bool KKApiRemoveBackendLogicResult(JPlayer whichPlayer, string key)
{
    return RequestExtraBooleanData(89, whichPlayer, key, null, false, 0, 0, 0);
}

// title = "【随机只读存档】剩余次数"
public static int KKApiRandomSaveGameCount(JPlayer whichPlayer, string groupkey)
{
    return RequestExtraIntegerData(101, whichPlayer, groupkey, null, false, 0, 0, 0);
}

// title = "注册随机存档更新事件"
public static void KKApiTriggerRegisterBackendLogicUpdata(JTrigger trig)
{
    DzTriggerRegisterSyncData(trig, "DZBLU", true);
}

// title = "注册随机存档删除事件"
public static void KKApiTriggerRegisterBackendLogicDelete(JTrigger trig)
{
    DzTriggerRegisterSyncData(trig, "DZBLD", true);
}

// title = "获取变动的随机存档"
public static string KKApiGetSyncBackendLogic()
{
    return DzGetTriggerSyncData();
}

// title = "是否在平台正常游戏中"
public static bool KKApiIsGameMode()
{
    return RequestExtraBooleanData(90, null, null, null, false, 0, 0, 0);
}

// title = "初始化平台键位显示设置"
public static bool KKApiInitializeGameKey(JPlayer whichPlayer, int setIndex, string k, string data)
{
    return RequestExtraBooleanData(91, whichPlayer, "[{\"name\":\"" + data + "\",\"key\":\"" + k + "\"}]", null, false, setIndex, 0, 0);
}

// title = "获取玩家的平台ID"
public static string KKApiPlayerGUID(JPlayer whichPlayer)
{
    return RequestExtraStringData(93, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "玩家地图任务状态"
public static bool KKApiIsTaskInProgress(JPlayer whichPlayer, int setIndex, int taskstat)
{
    return RequestExtraIntegerData(94, whichPlayer, null, null, false, setIndex, 0, 0) == taskstat;
}

// title = "玩家地图任务当前进度"
public static int KKApiQueryTaskCurrentProgress(JPlayer whichPlayer, int setIndex)
{
    return RequestExtraIntegerData(95, whichPlayer, null, null, false, setIndex, 0, 0);
}

// title = "玩家地图任务总进度"
public static int KKApiQueryTaskTotalProgress(JPlayer whichPlayer, int setIndex)
{
    return RequestExtraIntegerData(96, whichPlayer, null, null, false, setIndex, 0, 0);
}

// title = "玩家平台该地图成就是否完成"
public static bool KKApiIsAchievementCompleted(JPlayer whichPlayer, string id)
{
    return RequestExtraBooleanData(98, whichPlayer, id, null, false, 0, 0, 0);
}

// title = "玩家平台该地图成就点数"
public static int KKApiAchievementPoints(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(99, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "判定测试大厅游戏时长区间"
public static bool KKApiPlayedTime(JPlayer whichPlayer, int minHours, int maxHours)
{
    return RequestExtraBooleanData(100, whichPlayer, null, null, false, minHours, maxHours, 0);
}

// title = "【批量存档】开始保存"
public static bool KKApiBeginBatchSaveArchive(JPlayer whichPlayer)
{
    return RequestExtraBooleanData(102, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "【批量存档】添加条目"
public static bool KKApiAddBatchSaveArchive(JPlayer whichPlayer, string key, string value, bool caseInsensitive)
{
    return RequestExtraBooleanData(103, whichPlayer, key, value, caseInsensitive, 0, 0, 0);
}

// title = "【批量存档】结束保存"
public static bool KKApiEndBatchSaveArchive(JPlayer whichPlayer, bool abandon)
{
    return RequestExtraBooleanData(104, whichPlayer, null, null, abandon, 0, 0, 0);
}

// title = "【批量存档】添加条目-整数"
public static void KKApiAddBatchSaveArchiveInteger(JPlayer whichPlayer, string key, int value)
{
    set key = "I" + key;
    KKApiAddBatchSaveArchive(whichPlayer, key, I2S(value), false);
    set key = null;
    set whichPlayer = null;
}

// title = "【批量存档】添加条目-实数"
public static void KKApiAddBatchSaveArchiveReal(JPlayer whichPlayer, string key, float value)
{
    set key = "R" + key;
    KKApiAddBatchSaveArchive(whichPlayer, key, R2S(value), false);
    set key = null;
    set whichPlayer = null;
}

// title = "【批量存档】添加条目-布尔值"
public static void KKApiAddBatchSaveArchiveBoolean(JPlayer whichPlayer, string key, bool value)
{
    set key = "B" + key;
    // title = "【批量存档】添加条目"            if(value)then;
    KKApiAddBatchSaveArchive(whichPlayer, key, "1", false);
            else
    {
        KKApiAddBatchSaveArchive(whichPlayer, key, "0", false);
    }
    set key = null;
    set whichPlayer = null;
}

// title = "【批量存档】添加条目-字符串"
public static void KKApiAddBatchSaveArchiveString(JPlayer whichPlayer, string key, string value)
{
    set key = "S" + key;
    KKApiAddBatchSaveArchive(whichPlayer, key, value, false);
    set key = null;
    set whichPlayer = null;
}

// title = "注册天梯投降事件"
public static void KKApiTriggerRegisterLadderSurrender(JTrigger trig)
{
    DzTriggerRegisterSyncData(trig, "DZSR", true);
}

// title = "获取天梯投降的队伍ID"
public static int KKApiGetLadderSurrenderTeamId()
{
    return S2I(DzGetTriggerSyncData());
}

// title = "玩家在公会的等级"
public static int KKApiGetGuildLevel(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(106, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "宠物探险次数"
public static int KKApiMapExplorationNum(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(107, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "宠物探险时间"
public static int KKApiMapExplorationTime(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(108, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "测试大厅预约人数"
public static int KKApiMapOrderNum()
{
    return RequestExtraIntegerData(109, null, null, null, false, 0, 0, 0);
}

// title = "发送云脚本数据"
public static bool KKApiMlScriptEvent(JPlayer whichPlayer, string eventName, string payload)
{
    return RequestExtraBooleanData(1009, whichPlayer, eventName, payload, false, 0, 0, 0);
}

// title = "事件响应 - 商城道具最后变动的数量"
public static int KKApiGetMallItemUpdateCount(JPlayer whichPlayer, string key)
{
    return RequestExtraIntegerData(110, whichPlayer, key, null, false, 0, 0, 0);
}

// title = "获取地图版本号[new]"
public static string KKApiGetMapVersion()
{
    return RequestExtraStringData(111, null, null, null, false, 0, 0, 0);
}

// title = "获取赛事模式[new]"
public static string KKApiGetCompetitionGameMode()
{
    return RequestExtraStringData(112, null, null, null, false, 0, 0, 0);
}

// title = "获取玩家当天总游戏局数[new]"
public static int KKApiDayRounds(JPlayer whichPlayer)
{
    return RequestExtraIntegerData(113, whichPlayer, null, null, false, 0, 0, 0);
}

// title = "获取玩家在指定地图会员等级[new]"
public static int KKApiConsumeLevel(JPlayer whichPlayer, int mapId)
{
    return RequestExtraIntegerData(115, whichPlayer, null, null, false, mapId, 0, 0);
}

// title = "判断玩家当前地图在游戏大厅置顶状态[new]"
public static bool KKApiIsPinned(JPlayer whichPlayer)
{
    return RequestExtraBooleanData(117, whichPlayer, null, null, false, 0, 0, 0);
}

    }
}
