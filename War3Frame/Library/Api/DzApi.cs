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
        /// title = "获取鼠标在游戏内的坐标X"        
        /// description = "获取鼠标在游戏内的坐标X"        
        /// comment = ""
        public static float DzGetMouseTerrainX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetMouseTerrainX"));
        }


        /// title = "获取鼠标在游戏内的坐标Y"
        /// description = "获取鼠标在游戏内的坐标Y"
        /// comment = ""

        public static float DzGetMouseTerrainY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetMouseTerrainY"));
        }


        /// title = "获取鼠标在游戏内的坐标Z"
        /// description = "获取鼠标在游戏内的坐标Z"
        /// comment = ""

        public static float DzGetMouseTerrainZ()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetMouseTerrainZ"));
        }


        /// title = "鼠标是否在游戏内"
        /// description = "鼠标是否在游戏内"
        /// comment = ""

        public static bool DzIsMouseOverUI()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsMouseOverUI"));
        }


        /// title = "获取鼠标在屏幕的坐标X"
        /// description = "获取鼠标在屏幕的坐标X"
        /// comment = ""

        public static int DzGetMouseX()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseX"));
        }


        /// title = "获取鼠标在屏幕的坐标Y"
        /// description = "获取鼠标在屏幕的坐标Y"
        /// comment = ""

        public static int DzGetMouseY()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseY"));
        }


        /// title = "获取鼠标游戏窗口坐标X"
        /// description = "获取鼠标游戏窗口坐标X"
        /// comment = ""

        public static int DzGetMouseXRelative()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseXRelative"));
        }


        /// title = "获取鼠标游戏窗口坐标Y"
        /// description = "获取鼠标游戏窗口坐标Y"
        /// comment = ""

        public static int DzGetMouseYRelative()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseYRelative"));
        }


        /// title = "设置鼠标的坐标"
        /// description = "设置鼠标的坐标为 (${x}, ${y})"
        /// comment = ""

        public static void DzSetMousePos(int x, int y)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetMousePos"), x, y);
        }

        public static void DzTriggerRegisterMouseEvent(JTrigger trig, int btn, int status, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseEvent"), trig.Handle, btn, status, sync, func);
        }

        public static void DzTriggerRegisterMouseEventByCode(JTrigger trig, int btn, int status, bool sync, Action funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseEventByCode"), trig.Handle, btn, status, sync, funcHandle);
        }

        public static void DzTriggerRegisterKeyEvent(JTrigger trig, int key, int status, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterKeyEvent"), trig.Handle, key, status, sync, func);
        }

        public static void DzTriggerRegisterKeyEventByCode(JTrigger trig, int key, int status, bool sync, Action funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterKeyEventByCode"), trig.Handle, key, status, sync, funcHandle);
        }

        public static void DzTriggerRegisterMouseWheelEvent(JTrigger trig, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseWheelEvent"), trig.Handle, sync, func);
        }

        public static void DzTriggerRegisterMouseWheelEventByCode(JTrigger trig, bool sync, Action funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseWheelEventByCode"), trig.Handle, sync, funcHandle);
        }

        public static void DzTriggerRegisterMouseMoveEvent(JTrigger trig, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseMoveEvent"), trig.Handle, sync, func);
        }

        public static void DzTriggerRegisterMouseMoveEventByCode(JTrigger trig, bool sync, Action funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterMouseMoveEventByCode"), trig.Handle, sync, funcHandle);
        }


        /// title = "事件响应 - 获取触发的按键"
        /// description = "获取触发的按键"
        /// comment = "响应 [硬件] - 按键事件"

        public static int DzGetTriggerKey()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetTriggerKey"));
        }


        /// title = "事件响应 - 获取滚轮变化值"
        /// description = "获取滚轮变化值"
        /// comment = "响应 [硬件] - 鼠标滚轮事件，正负区分上下"

        public static int DzGetWheelDelta()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWheelDelta"));
        }


        /// title = "判断按键是否按下"
        /// description = "判断 ${按键} 是否按下"
        /// comment = ""

        public static bool DzIsKeyDown(int iKey)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsKeyDown"), iKey);
        }


        /// title = "事件响应 - 获取触发硬件事件的玩家"
        /// description = "获取触发硬件事件的玩家"
        /// comment = "响应 [硬件] - 按键事件 滚轮事件 窗口大小变化事件"

        public static JPlayer DzGetTriggerKeyPlayer()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetTriggerKeyPlayer"));
            return new JPlayer(handle);
        }


        /// title = "获取war3窗口宽度"
        /// description = "获取魔兽窗口宽度"
        /// comment = ""

        public static int DzGetWindowWidth()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWindowWidth"));
        }


        /// title = "获取魔兽窗口高度"
        /// description = "获取魔兽窗口高度"
        /// comment = ""

        public static int DzGetWindowHeight()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWindowHeight"));
        }


        /// title = "获取魔兽窗口X坐标"
        /// description = "获取魔兽窗口X坐标"
        /// comment = ""

        public static int DzGetWindowX()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWindowX"));
        }


        /// title = "获取魔兽窗口Y坐标"
        /// description = "获取魔兽窗口Y坐标"
        /// comment = ""

        public static int DzGetWindowY()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetWindowY"));
        }

        public static void DzTriggerRegisterWindowResizeEvent(JTrigger trig, bool sync, string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterWindowResizeEvent"), trig.Handle, sync, func);
        }

        public static void DzTriggerRegisterWindowResizeEventByCode(JTrigger trig, bool sync, Action funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterWindowResizeEventByCode"), trig.Handle, sync, funcHandle);
        }


        /// title = "硬件 - 当前游戏窗口是否活动窗口"
        /// description = "当前游戏窗口是否活动窗口"
        /// comment = "切游戏出去后会返回false, 返回值是异步的，因为每个玩家的窗口都不同"

        public static bool DzIsWindowActive()
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzIsWindowActive"));
        }


        /// title = "设置可破坏物位置 [BZAPI]"
        /// description = "设置 ${可破坏物} 的坐标为 (${x}, ${y})"
        /// comment = ""

        public static void DzDestructablePosition(JDestructable d, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzDestructablePosition"), d.Handle, x, y);
        }


        /// title = "设置单位位置 - 本地调用 [BZAPI]"
        /// description = "设置 ${单位} 的坐标为 (${x}, ${y})"
        /// comment = ""

        public static void DzSetUnitPosition(JUnit whichUnit, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitPosition"), whichUnit.Handle, x, y);
        }


        /// title = "异步执行函数"
        /// description = "异步执行函数 ${funcName}"
        /// comment = ""

        public static void DzExecuteFunc(string funcName)
        {
            War3.CallNative(War3.GetNativeFunction("DzExecuteFunc"), funcName);
        }


        /// title = "获取鼠标指向的单位(异步)"
        /// description = "鼠标指向的单位"
        /// comment = ""

        public static JUnit DzGetUnitUnderMouse()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitUnderMouse"));
            return new JUnit(handle);
        }


        /// title = "替换单位贴图 [BZAPI]"
        /// description = "替换 ${单位} 的 贴图:${path} TexId:${texId})"
        /// comment = "只能替换模型中有Replaceable ID x 贴图的模型，ID为索引。不会替换大头像中的模型"

        public static void DzSetUnitTexture(JUnit whichUnit, string path, int texId)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitTexture"), whichUnit.Handle, path, texId);
        }


        /// title = "设置内存数值"
        /// description = "设置内存数据 ${地址} = ${数值}"
        /// comment = ""

        public static void DzSetMemory(int address, float value)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetMemory"), address, value);
        }


        /// title = "替换单位类型 [BZAPI]"
        /// description = "替换 ${单位} 的 单位类型为:${type}"
        /// comment = "会替换大头像中的模型"

        public static void DzSetUnitID(JUnit whichUnit, int id)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitID"), whichUnit.Handle, id);
        }


        /// title = "替换单位模型 [BZAPI]"
        /// description = "替换 ${单位} 的 模型:${path}"
        /// comment = "不会替换大头像中的模型"

        public static void DzSetUnitModel(JUnit whichUnit, string path)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetUnitModel"), whichUnit.Handle, path);
        }


        /// title = "原生 - 设置小地图背景贴图"
        /// description = "修改小地图背景贴图为 ${bottomHeight}"
        /// comment = ""

        public static void DzSetWar3MapMap(string map)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetWar3MapMap"), map);
        }


        /// title = "获取客户端语言"
        /// description = "获取客户端语言"
        /// comment = "对不同语言客户端返回不同"

        public static string DzGetLocale()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetLocale"));
        }


        /// title = "获取升级所需经验 "
        /// description = "获取单位 ${unit} 的 ${level}级 升级所需经验"
        /// comment = ""

        public static int DzGetUnitNeededXP(JUnit whichUnit, int level)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetUnitNeededXP"), whichUnit.Handle, level);
        }


        /// title = "数据同步"
        /// description = "标签为 ${prefix} 的数据被同步 | 来自平台:${server}"
        /// comment = "来自平台的参数填false"

        public static void DzTriggerRegisterSyncData(JTrigger trig, string prefix, bool server)
        {
            War3.CallNative(War3.GetNativeFunction("DzTriggerRegisterSyncData"), trig.Handle, prefix, server);
        }


        /// title = "同步游戏数据"
        /// description = "同步 标签：${prefix}  发送数据：${data}"
        /// comment = ""

        public static void DzSyncData(string prefix, string data)
        {
            War3.CallNative(War3.GetNativeFunction("DzSyncData"), prefix, data);
        }


        /// title = "事件响应 - 获取同步的数据前缀 "
        /// description = "获取同步的数据前缀"
        /// comment = "响应 [同步] - 同步消息事件"

        public static string DzGetTriggerSyncPrefix()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetTriggerSyncPrefix"));
        }


        /// title = "事件响应 - 获取同步的数据"
        /// description = "获取同步的数据"
        /// comment = "响应 [同步] - 同步消息事件"

        public static string DzGetTriggerSyncData()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetTriggerSyncData"));
        }


        /// title = "事件响应 - 获取同步数据的玩家"
        /// description = "获取同步数据的玩家"
        /// comment = "响应 [同步] - 同步消息事件"

        public static JPlayer DzGetTriggerSyncPlayer()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetTriggerSyncPlayer"));
            return new JPlayer(handle);
        }


        /// title = "同步游戏数据（指定长度）"
        /// description = "同步 标签：${prefix}  发送数据：${data} 数据长度：${dataLen}"
        /// comment = "可以按长度同步数据"

        public static void DzSyncBuffer(string prefix, string data, int dataLen)
        {
            War3.CallNative(War3.GetNativeFunction("DzSyncBuffer"), prefix, data, dataLen);
        }


        /// title = "同步游戏数据（立即）"
        /// description = "立即同步 标签：${prefix}  发送数据：${data}"
        /// comment = ""

        public static void DzSyncDataImmediately(string prefix, string data)
        {
            War3.CallNative(War3.GetNativeFunction("DzSyncDataImmediately"), prefix, data);
        }


        /// title = "原生 - 隐藏界面元素"
        /// description = "隐藏所有界面UI"
        /// comment = "不再在地图初始化时调用则会残留小地图和时钟模型"

        public static void DzFrameHideInterface()
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameHideInterface"));
        }


        /// title = "原生 - 修改游戏渲染黑边范围"
        /// description = "修改游戏渲染黑边: 上方高度:${upperHeight} 下方高度:${bottomHeight}"
        /// comment = ""

        public static void DzFrameEditBlackBorders(float upperHeight, float bottomHeight)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameEditBlackBorders"), upperHeight, bottomHeight);
        }


        /// title = "原生 - 单位大头像"
        /// description = "单位大头像"
        /// comment = "小地图右侧的大头像"

        public static int DzFrameGetPortrait()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetPortrait"));
        }


        /// title = "原生 - 小地图"
        /// description = "小地图"
        /// comment = ""

        public static int DzFrameGetMinimap()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetMinimap"));
        }


        /// title = "原生 - 技能按钮"
        /// description = "技能按钮:(${row}, ${calumn})"
        /// comment = "参考物编中的技能按钮(x,y)坐标"

        public static int DzFrameGetCommandBarButton(int row, int column)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetCommandBarButton"), row, column);
        }


        /// title = "原生 - 英雄按钮"
        /// description = "英雄按钮:${buttnoid}"
        /// comment = "左侧的英雄头像，参数表示第N+1个英雄，索引从0开始"

        public static int DzFrameGetHeroBarButton(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetHeroBarButton"), buttonId);
        }


        /// title = "原生 - 英雄血条"
        /// description = "英雄血条:${buttnoid}"
        /// comment = "左侧的英雄头像下的血条，参数表示第N+1个英雄，索引从0开始"

        public static int DzFrameGetHeroHPBar(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetHeroHPBar"), buttonId);
        }


        /// title = "原生 - 英雄蓝条"
        /// description = "英雄蓝条:${buttnoid}"
        /// comment = "左侧的英雄头像下的蓝条，参数表示第N+1个英雄，索引从0开始"

        public static int DzFrameGetHeroManaBar(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetHeroManaBar"), buttonId);
        }


        /// title = "原生 - 物品栏按钮"
        /// description = "物品栏按钮:${buttnoid}"
        /// comment = "索引从0开始"

        public static int DzFrameGetItemBarButton(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetItemBarButton"), buttonId);
        }


        /// title = "原生 - 小地图按钮"
        /// description = "小地图按钮:${buttnoid}"
        /// comment = "小地图右侧竖排按钮，索引从0开始"

        public static int DzFrameGetMinimapButton(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetMinimapButton"), buttonId);
        }


        /// title = "原生 - 界面按钮"
        /// description = "界面按钮:${buttnoid}"
        /// comment = "左上的菜单等按钮，索引从0开始"

        public static int DzFrameGetUpperButtonBarButton(int buttonId)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetUpperButtonBarButton"), buttonId);
        }


        /// title = "原生 - 鼠标提示"
        /// description = "鼠标提示"
        /// comment = "鼠标移动到物品或技能按钮上显示的提示窗，初始位于技能栏上方"

        public static int DzFrameGetTooltip()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetTooltip"));
        }


        /// title = "原生 - 玩家聊天信息框"
        /// description = "玩家聊天信息框"
        /// comment = ""

        public static int DzFrameGetChatMessage()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetChatMessage"));
        }


        /// title = "原生 - 系统消息框"
        /// description = "系统消息框"
        /// comment = "包含显示消息给玩家 及 显示Debug消息 等，"

        public static int DzFrameGetUnitMessage()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetUnitMessage"));
        }


        /// title = "原生 - 上方消息框"
        /// description = "上方消息框"
        /// comment = "高维修费用 等消息"

        public static int DzFrameGetTopMessage()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetTopMessage"));
        }


        /// title = "取 RGBA 色值"
        /// description = "将RGB(Alph:${Alpha}，R:${Red} G:${Green} B:${Blue} 转换为色值"
        /// comment = "返回一个整数，用于设置Frame颜色"

        public static int DzGetColor(int r, int g, int b, int a)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetColor"), r, g, b, a);
        }

        public static void DzFrameSetUpdateCallback(string func)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetUpdateCallback"), func);
        }

        public static void DzFrameSetUpdateCallbackByCode(Action funcHandle)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetUpdateCallbackByCode"), funcHandle);
        }


        /// title = "显示/隐藏"
        /// description = "设置 ${frame} 显示:${bottomHeight}"
        /// comment = ""

        public static void DzFrameShow(int frame, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameShow"), frame, enable);
        }


        /// title = "新建Frame"
        /// description = "新建Frame 名字:${frame} 父节点:${parent} ID:${Id}"
        /// comment = "名字为fdf文件中的名字，ID默认填0。重复创建同名Frame会导致游戏退出时显示崩溃消息，如需避免可以使用Tag创建"

        public static int DzCreateFrame(string frame, int parent, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzCreateFrame"), frame, parent, id);
        }

        public static int DzCreateSimpleFrame(string frame, int parent, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzCreateSimpleFrame"), frame, parent, id);
        }


        /// title = "销毁"
        /// description = "销毁 ${frame}"
        /// comment = "销毁一个被重复创建过的Frame会导致游戏崩溃，重复创建同名Frame请使用Tag创建"

        public static void DzDestroyFrame(int frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzDestroyFrame"), frame);
        }


        /// title = "加载Toc文件列表"
        /// description = "加载--> ${fileName.toc}"
        /// comment = "载入自己的fdf列表文件"

        public static void DzLoadToc(string fileName)
        {
            War3.CallNative(War3.GetNativeFunction("DzLoadToc"), fileName);
        }


        /// title = "设置相对位置"
        /// description = "设置 ${frame} 的 ${Point} 锚点 (跟随Frame-->${relativeFrame} 的 ${relativePoint} 锚点) 偏移(${x}, ${y})"
        /// comment = ""

        public static void DzFrameSetPoint(int frame, int point, int relativeFrame, int relativePoint, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetPoint"), frame, point, relativeFrame, relativePoint, x, y);
        }


        /// title = "设置绝对位置"
        /// description = "设置 ${frame} 的 ${Point} 锚点 在 (${x}, ${y})"
        /// comment = ""

        public static void DzFrameSetAbsolutePoint(int frame, int point, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetAbsolutePoint"), frame, point, x, y);
        }


        /// title = "清空所有锚点"
        /// description = "清空 ${frame} 的 全部锚点"
        /// comment = ""

        public static void DzFrameClearAllPoints(int frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameClearAllPoints"), frame);
        }


        /// title = "启用/禁用"
        /// description = "设置 ${frame} 启用:${bottomHeight}"
        /// comment = ""

        public static void DzFrameSetEnable(int name, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetEnable"), name, enable);
        }


        /// title = "注册UI事件回调(func name)[观战、录像不响应]"
        /// description = "注册 ${frame} 的 ${事件类型} 事件 运行:${func name} 是否同步:${sync}"
        /// comment = ""

        public static void DzFrameSetScript(int frame, int eventId, string func, bool sync)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScript"), frame, eventId, func, sync);
        }


        /// title = "注册UI事件回调(func handle)[观战、录像不响应]"
        /// description = "注册 ${frame} 的 ${事件类型} 事件 运行:${code handle} 是否同步:${sync}"
        /// comment = "运行触发器时需要打开同步：执行会阻止它原本的功能继续响应"

        public static void DzFrameSetScriptByCode(int frame, int eventId, Action funcHandle, bool sync)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptByCode"), frame, eventId, funcHandle, sync);
        }


        /// title = "注册UI事件回调-异步(func handle)[观战、录像不响应][new]"
        /// description = "注册 ${frame} 的 ${事件类型} 事件 运行:${code handle} 是否同步:${sync}"
        /// comment = "注册UI事件回调-异步，可以在游戏、录像、观战等所有模式响应"

        public static void DzFrameSetScriptBlock(int frame, int eventId, Action funcHandle, bool sync)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptBlock"), frame, eventId, funcHandle, sync);
        }


        /// title = "注册UI事件回调-异步(func name)[观战、录像可响应][new]"
        /// description = "注册 ${frame} 的 ${事件类型} 事件 运行:${func name}"
        /// comment = ""

        public static void DzFrameSetScriptAsync(int frame, int eventId, string funcName)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptAsync"), frame, eventId, funcName);
        }


        /// title = "注册UI事件回调-异步(func handle)[观战、录像可响应][new]"
        /// description = "注册 ${frame} 的 ${事件类型} 事件 运行:${code handle}"
        /// comment = "注册UI事件回调-异步，可以在游戏、录像、观战等所有模式响应"

        public static void DzFrameSetScriptByCodeAsync(int frame, int eventId, Action func)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptByCodeAsync"), frame, eventId, func);
        }


        /// title = "注册UI事件回调-异步(func handle)[观战、录像可响应][new]"
        /// description = "注册 ${frame} 的 ${事件类型} 事件 运行:${code handle}"
        /// comment = "注册UI事件回调-异步，可以在游戏、录像、观战等所有模式响应，该函数执行会阻止它原本的功能继续响应"

        public static void DzFrameSetScriptBlockAsync(int frame, int eventId, Action func)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScriptBlockAsync"), frame, eventId, func);
        }


        /// title = "事件响应 - 获取触发ui的玩家"
        /// description = "获取触发ui的玩家"
        /// comment = ""

        public static JPlayer DzGetTriggerUIEventPlayer()
        {
            var handle = War3.CallNative<int>(War3.GetNativeFunction("DzGetTriggerUIEventPlayer"));
            return new JPlayer(handle);
        }


        /// title = "事件响应 - 触发的Frame"
        /// description = "触发的Frame"
        /// comment = ""

        public static int DzGetTriggerUIEventFrame()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetTriggerUIEventFrame"));
        }


        /// title = "获取子Frame"
        /// description = "获取名字为 ${name} 的子Frame  ID:${Id}"
        /// comment = "ID默认填0，同名时优先获取最后被创建的。非Simple类的Frame类型都用此函数来获取子Frame。"

        public static int DzFrameFindByName(string name, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameFindByName"), name, id);
        }


        /// title = "获取子SimpleFrame"
        /// description = "获取名字为 ${name} 的子SimpleFrame  ID:${Id}"
        /// comment = "ID默认填0，同名时优先获取最后被创建的。SimpleFrame为fdf中的Frame类型。"

        public static int DzSimpleFrameFindByName(string name, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzSimpleFrameFindByName"), name, id);
        }


        /// title = "获取子SimpleFontString"
        /// description = "获取名字为 ${name} 的子SimpleFontString  ID:${Id}"
        /// comment = "ID默认填0，同名时优先获取最后被创建的。SimpleFontString为fdf中的Frame类型。"

        public static int DzSimpleFontStringFindByName(string name, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzSimpleFontStringFindByName"), name, id);
        }


        /// title = "获取子SimpleTexture"
        /// description = "获取名字为 ${name} 的子SimpleTexture  ID:${Id}"
        /// comment = "ID默认填0，同名时优先获取最后被创建的。SimpleTexture为fdf中的Frame类型。"

        public static int DzSimpleTextureFindByName(string name, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzSimpleTextureFindByName"), name, id);
        }


        /// title = "原生 - 游戏UI"
        /// description = "游戏UI"
        /// comment = "一般用作创建自定义UI的父节点"

        public static int DzGetGameUI()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetGameUI"));
        }


        /// title = "点击"
        /// description = "点击 ${frame}"
        /// comment = ""

        public static void DzClickFrame(int frame)
        {
            War3.CallNative(War3.GetNativeFunction("DzClickFrame"), frame);
        }


        /// title = "原生 - 修改屏幕比例(FOV)"
        /// description = "修改屏幕比例(FOV):${val}"
        /// comment = ""

        public static void DzSetCustomFovFix(float value)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetCustomFovFix"), value);
        }


        /// title = "开启/关闭宽屏模式"
        /// description = "${enable} 宽屏模式"
        /// comment = ""

        public static void DzEnableWideScreen(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzEnableWideScreen"), enable);
        }


        /// title = "设置文本"
        /// description = "设置 ${frame} 的文本为 ${string}"
        /// comment = "(支持EditBox, TextFrame, TextArea, SimpleFontString、GlueEditBoxWar3、SlashChatBox、TimerTextFrame、TextButtonFrame、GlueTextButton)"

        public static void DzFrameSetText(int frame, string text)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetText"), frame, text);
        }


        /// title = "获取 Frame 内的文字"
        /// description = "获取 ${buttnoid} 的文字"
        /// comment = "（支持EditBox, TextFrame, TextArea, SimpleFontString）"

        public static string DzFrameGetText(int frame)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzFrameGetText"), frame);
        }


        /// title = "设置字数限制"
        /// description = "设置 ${frame} 的字数限制为 ${size}"
        /// comment = ""

        public static void DzFrameSetTextSizeLimit(int frame, int size)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTextSizeLimit"), frame, size);
        }


        /// title = "获取 Frame 的 字数限制"
        /// description = "获取 ${frame} 的字数限制"
        /// comment = "（支持EditBox）"

        public static int DzFrameGetTextSizeLimit(int frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetTextSizeLimit"), frame);
        }

        public static void DzFrameSetTextColor(int frame, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTextColor"), frame, color);
        }


        /// title = "鼠标所在的Frame控件指针"
        /// description = "鼠标所在的Frame控件指针"
        /// comment = "不是所有类型的Frame都能响应鼠标，能响应的有BUTTON，TEXT等"

        public static int DzGetMouseFocus()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetMouseFocus"));
        }


        /// title = "移动所有锚点到Frame"
        /// description = "移动 ${frame} 的 所有锚点 到 ${frame} 上"
        /// comment = ""

        public static bool DzFrameSetAllPoints(int frame, int relativeFrame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameSetAllPoints"), frame, relativeFrame);
        }


        /// title = "设置焦点"
        /// description = "设置 ${frame} 获取焦点 ${enable}"
        /// comment = ""

        public static bool DzFrameSetFocus(int frame, bool enable)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameSetFocus"), frame, enable);
        }


        /// title = "设置模型"
        /// description = "设置 ${frame} 的模型文件为 ${modelFile} ModelType:${modelType} Flag:${flag}"
        /// comment = ""

        public static void DzFrameSetModel(int frame, string modelFile, int modelType, int flag)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetModel"), frame, modelFile, modelType, flag);
        }


        /// title = "控件是否启用"
        /// description = "${frame} 是否启用"
        /// comment = ""

        public static bool DzFrameGetEnable(int frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameGetEnable"), frame);
        }


        /// title = "设置透明度(0-255)"
        /// description = "设置 ${frame} 的透明度为 ${alpha}"
        /// comment = ""

        public static void DzFrameSetAlpha(int frame, int alpha)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetAlpha"), frame, alpha);
        }


        /// title = "获取 Frame 的 透明度(0-255)"
        /// description = "获取 ${Frame} 的透明度"
        /// comment = ""

        public static int DzFrameGetAlpha(int frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetAlpha"), frame);
        }


        /// title = "设置动画"
        /// description = "设置 ${frame} 播放序号 ${alpha} 的动画  自动播放:${autocast}"
        /// comment = ""

        public static void DzFrameSetAnimate(int frame, int animId, bool autocast)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetAnimate"), frame, animId, autocast);
        }


        /// title = "设置动画进度"
        /// description = "设置 ${frame} 的动画进度为:${offset}"
        /// comment = "自动播放为false是可用"

        public static void DzFrameSetAnimateOffset(int frame, float offset)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetAnimateOffset"), frame, offset);
        }


        /// title = "设置贴图"
        /// description = "设置 ${frame} 的贴图为:${texture} 是否平铺 ${flag}"
        /// comment = "（支持Backdrop、SimpleStatusBar）"

        public static void DzFrameSetTexture(int frame, string texture, int flag)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTexture"), frame, texture, flag);
        }


        /// title = "设置缩放"
        /// description = "设置 ${frame} 的缩放 ${scale}"
        /// comment = ""

        public static void DzFrameSetScale(int frame, float scale)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetScale"), frame, scale);
        }


        /// title = "设置提示"
        /// description = "设置 ${frame} 的提示Frame为 ${tooltip} "
        /// comment = "设置tooltip"

        public static void DzFrameSetTooltip(int frame, int tooltip)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTooltip"), frame, tooltip);
        }


        /// title = "限制鼠标移动"
        /// description = "限制鼠标在 ${frame} 内: ${enable} "
        /// comment = ""

        public static void DzFrameCageMouse(int frame, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameCageMouse"), frame, enable);
        }


        /// title = "获取当前值"
        /// description = "获取 ${frame} 当前值"
        /// comment = "（支持Slider、SimpleStatusBar、StatusBar）"

        public static float DzFrameGetValue(int frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetValue"), frame);
        }


        /// title = "设置最大/最小值"
        /// description = "设置 ${frame} 的 最小值为 ${Min} 最大值为 ${Max}"
        /// comment = "（支持Slider、SimpleStatusBar、StatusBar）"

        public static void DzFrameSetMinMaxValue(int frame, float minValue, float maxValue)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetMinMaxValue"), frame, minValue, maxValue);
        }


        /// title = "设置步进值"
        /// description = "设置 ${frame} 的 步进值 为 ${step}"
        /// comment = "（支持Slider）"

        public static void DzFrameSetStepValue(int frame, float step)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetStepValue"), frame, step);
        }


        /// title = "设置当前值"
        /// description = "设置 ${frame} 的 当前值 为 ${value}"
        /// comment = "（支持Slider、SimpleStatusBar、StatusBar）"

        public static void DzFrameSetValue(int frame, float value)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetValue"), frame, value);
        }


        /// title = "设置大小"
        /// description = "设置 ${frame} （宽 ${w} 高 ${h}）"
        /// comment = ""

        public static void DzFrameSetSize(int frame, float w, float h)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetSize"), frame, w, h);
        }


        /// title = "新建Frame [Tag]"
        /// description = "创建 类型:${type} 名字:${frame} 父节点:${parent} 模版:${template} ID:${Id}"
        /// comment = "此处名字可以自定义，类型和模版填写fdf文件中的内容。通过此函数创建的Frame无法获取到子Frame。"

        public static int DzCreateFrameByTagName(string frameType, string name, int parent, string template, int id)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzCreateFrameByTagName"), frameType, name, parent, template, id);
        }


        /// title = "设置颜色"
        /// description = "设置 ${frame} 颜色 ${color}"
        /// comment = ""

        public static void DzFrameSetVertexColor(int frame, int color)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetVertexColor"), frame, color);
        }

        public static void DzOriginalUIAutoResetPoint(bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzOriginalUIAutoResetPoint"), enable);
        }


        /// title = "设置优先级"
        /// description = "设置 ${frame} 优先级:${int}"
        /// comment = ""

        public static void DzFrameSetPriority(int frame, int priority)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetPriority"), frame, priority);
        }


        /// title = "设置父窗口 "
        /// description = "设置 ${frame} 的父窗口为 ${frame2}"
        /// comment = ""

        public static void DzFrameSetParent(int frame, int parent)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetParent"), frame, parent);
        }


        /// title = "获取 Frame 的 高度"
        /// description = "获取 ${frame} 的高度"
        /// comment = ""

        public static float DzFrameGetHeight(int frame)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzFrameGetHeight"), frame);
        }


        /// title = "设置字体 "
        /// description = "设置 ${frame} 的字体为 ${font}, 大小 ${height}, flag ${flag}"
        /// comment = "支持EditBox、SimpleFontString、SimpleMessageFrame以及非SimpleFrame类型的例如TEXT，flag作用未知"

        public static void DzFrameSetFont(int frame, string fileName, float height, int flag)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetFont"), frame, fileName, height, flag);
        }


        /// title = "获取 Frame 的 Parent"
        /// description = "获取 ${frame} 的 Parent"
        /// comment = ""

        public static int DzFrameGetParent(int frame)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzFrameGetParent"), frame);
        }


        /// title = "设置对齐方式"
        /// description = "设置 ${frame} 的对齐方式为 ${align}"
        /// comment = "支持TextFrame、SimpleFontString、SimpleMessageFrame"

        public static void DzFrameSetTextAlignment(int frame, int align)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameSetTextAlignment"), frame, align);
        }


        /// title = "界面 - 获取控件的全局名字"
        /// description = "获取控件 ${frame} 的全局名字"
        /// comment = "相当于获取 <<新建Frame [Tag]:DzCreateFrameByTagName>> 函数第2个参数"

        public static string DzFrameGetName(int frame)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzFrameGetName"), frame);
        }


        /// title = "获取魔兽窗口宽度"
        /// description = "获取魔兽窗口宽度"
        /// comment = "不包括魔兽窗口边框"

        public static int DzGetClientWidth()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetClientWidth"));
        }


        /// title = "获取魔兽窗口高度"
        /// description = "获取魔兽窗口高度"
        /// comment = "不包括魔兽窗口边框"

        public static int DzGetClientHeight()
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzGetClientHeight"));
        }


        /// title = "控件是否显示"
        /// description = "${frame} 是否显示"
        /// comment = ""

        public static bool DzFrameIsVisible(int frame)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzFrameIsVisible"), frame);
        }


        /// title = "显示/隐藏SimpleFrame"
        /// description = "设置 ${frame} 显示:${bottomHeight}"
        /// comment = "只针对SimpleFrame类型控件"

        public static void DzSimpleFrameShow(int frame, bool enable)
        {
            War3.CallNative(War3.GetNativeFunction("DzSimpleFrameShow"), frame, enable);
        }


        /// title = "追加文本"
        /// description = "追加 ${frame} 的文本 ${string}"
        /// comment = "支持TextArea"

        public static void DzFrameAddText(int frame, string text)
        {
            War3.CallNative(War3.GetNativeFunction("DzFrameAddText"), frame, text);
        }


        /// title = "沉默单位"
        /// description = "设置单位 ${单位} 的沉默状态为 ${true}"
        /// comment = ""

        public static void DzUnitSilence(JUnit whichUnit, bool disable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitSilence"), whichUnit.Handle, disable);
        }


        /// title = "禁用攻击"
        /// description = "设置单位 ${单位}  ${true} 物理攻击"
        /// comment = ""

        public static void DzUnitDisableAttack(JUnit whichUnit, bool disable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitDisableAttack"), whichUnit.Handle, disable);
        }


        /// title = "禁用道具"
        /// description = "设置单位 ${单位}  ${true} 物品栏"
        /// comment = ""

        public static void DzUnitDisableInventory(JUnit whichUnit, bool disable)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitDisableInventory"), whichUnit.Handle, disable);
        }


        /// title = "刷新小地图"
        /// description = "刷新小地图"
        /// comment = ""

        public static void DzUpdateMinimap()
        {
            War3.CallNative(War3.GetNativeFunction("DzUpdateMinimap"));
        }


        /// title = "修改单位透明度"
        /// description = "设置单位 ${单位} 的透明度为${透明度}  ${true} 强制更新"
        /// comment = ""

        public static void DzUnitChangeAlpha(JUnit whichUnit, int alpha, bool forceUpdate)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitChangeAlpha"), whichUnit.Handle, alpha, forceUpdate);
        }


        /// title = "修改单位选中状态"
        /// description = "设置单位 ${单位}  ${state} 被选中"
        /// comment = ""

        public static void DzUnitSetCanSelect(JUnit whichUnit, bool state)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitSetCanSelect"), whichUnit.Handle, state);
        }


        /// title = "修改单位是否可以被设置为目标"
        /// description = "设置单位 ${单位}  ${state} 设置为目标"
        /// comment = ""

        public static void DzUnitSetTargetable(JUnit whichUnit, bool state)
        {
            War3.CallNative(War3.GetNativeFunction("DzUnitSetTargetable"), whichUnit.Handle, state);
        }


        /// title = "保存内存数据"
        /// description = "保存内存数据： ${state} "
        /// comment = ""

        public static void DzSaveMemoryCache(string cache)
        {
            War3.CallNative(War3.GetNativeFunction("DzSaveMemoryCache"), cache);
        }


        /// title = "读取内存数据"
        /// description = "读取内存数据"
        /// comment = ""

        public static string DzGetMemoryCache()
        {
            return War3.CallNative<string>(War3.GetNativeFunction("DzGetMemoryCache"));
        }


        /// title = "设置加速倍率"
        /// description = "设置加速倍率： ${ratio} "
        /// comment = ""

        public static void DzSetSpeed(float ratio)
        {
            War3.CallNative(War3.GetNativeFunction("DzSetSpeed"), ratio);
        }


        /// title = "转换地图坐标为屏幕坐标-异步"
        /// description = "转换地图坐标（ ${x}，${Y}，${z}）为屏幕坐标执行动作"
        /// comment = "动作里用“转换后的屏幕X坐标”和“转换后的屏幕Y坐标”去读取"

        public static bool DzConvertWorldPosition(float x, float y, float z, Action callback)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("DzConvertWorldPosition"), x, y, z, callback);
        }


        /// title = "获取转换后的屏幕 X 坐标"
        /// description = "获取转换后的屏幕 X 坐标"
        /// comment = "用于转换世界坐标为屏幕坐标动作后面"

        public static float DzGetConvertWorldPositionX()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetConvertWorldPositionX"));
        }


        /// title = "获取转换后的屏幕 Y 坐标"
        /// description = "获取转换后的屏幕 Y 坐标"
        /// comment = "用于转换世界坐标为屏幕坐标动作后面"

        public static float DzGetConvertWorldPositionY()
        {
            return War3.CallNative<float>(War3.GetNativeFunction("DzGetConvertWorldPositionY"));
        }


        /// title = "创建技能按钮 "
        /// description = "在 ${界面} 的创建技能按钮：贴图:${path} 名称:${name} 描述：${desc})"
        /// comment = ""

        public static int DzCreateCommandButton(int parent, string icon, string name, string desc)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("DzCreateCommandButton"), parent, icon, name, desc);
        }


        /// title = "注册鼠标事件"
        /// description = "任意玩家 ${key} ${actionkey} 时"
        /// comment = "请使用“获取触发硬件事件的玩家”来获取触发玩家"

        public static void DzTriggerRegisterMouseEventTrg(JTrigger trg, int status, int btn)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterMouseEvent(trg, btn, status, true, null);
        }


        /// title = "键盘事件"
        /// description = "任意玩家 ${keyaction} ${key} 时"
        /// comment = "请使用“获取触发硬件事件的玩家”来获取触发玩家"

        public static void DzTriggerRegisterKeyEventTrg(JTrigger trg, int status, int btn)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterKeyEvent(trg, btn, status, true, null);
        }


        /// title = "鼠标移动事件"
        /// description = "任意玩家移动鼠标"
        /// comment = "请使用“获取触发硬件事件的玩家”来获取触发玩家"

        public static void DzTriggerRegisterMouseMoveEventTrg(JTrigger trg)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterMouseMoveEvent(trg, true, null);
        }


        /// title = "鼠标滚轮事件"
        /// description = "任意玩家滑动鼠标滚轮"
        /// comment = "请使用“获取触发硬件事件的玩家”来获取触发玩家，滚轮的方向由“获取滚轮变化值”的正负来判断"

        public static void DzTriggerRegisterMouseWheelEventTrg(JTrigger trg)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterMouseWheelEvent(trg, true, null);
        }


        /// title = "窗口大小变化事件"
        /// description = "任意玩家改变窗口大小"
        /// comment = "请使用“获取触发硬件事件的玩家”来获取触发玩家。这个事件会在玩家拖动窗口大小时频繁触发"

        public static void DzTriggerRegisterWindowResizeEventTrg(JTrigger trg)
        {
            if (trg == null)
            {
                return;
            }
            DzTriggerRegisterWindowResizeEvent(trg, true, null);
        }


        /// title = "转换 Frame 为 整数"
        /// description = "转换 ${Frame} 为 整数"
        /// comment = ""

        public static int DzF2I(int i)
        {
            return i;
        }


        /// title = "转换 整数 为 Frame"
        /// description = "转换 ${Integer} 为 Frame"
        /// comment = ""

        public static int DzI2F(int i)
        {
            return i;
        }


        /// title = "转换 按键 为 整数"
        /// description = "转换 ${Key} 为 整数"
        /// comment = ""

        public static int DzK2I(int i)
        {
            return i;
        }


        /// title = "转换 整数 为 按键"
        /// description = "转换 ${Integer} 为 按键"
        /// comment = ""

        public static int DzI2K(int i)
        {
            return i;
        }


        /// title = "玩家获得地图商城道具事件（实时）"
        /// description = "注册玩家获得地图商城道具事件（实时）"
        /// comment = "玩家背包中新获得了当前地图商城道具的回调事件，用于地图实现玩家在游戏内商城购买成功后在游戏内立即生效。'触发的商城道具事件的玩家'、'触发的商城道具'和'商城道具最后变动的数量'使用。"

        public static void DzTriggerRegisterMallItemSyncData(JTrigger trig)
        {
            DzTriggerRegisterSyncData(trig, "DZMIA", true);
        }


        /// title = "玩家消耗地图商城道具事件（实时）"
        /// description = "注册玩家消耗地图商城道具事件（实时）"
        /// comment = "玩家背包中消耗或使用商城道具的回调事件。可在事件内配合'触发的商城道具事件的玩家'、'触发的商城道具'和'商城道具最后变动的数量'使用。"

        public static void DzTriggerRegisterMallItemConsumeEvent(JTrigger trig)
        {
            DzTriggerRegisterSyncData(trig, "DZMIC", true);
        }


        /// title = "玩家删除地图商城道具事件（实时）"
        /// description = "注册玩家删除地图商城道具事件（实时）"
        /// comment = "玩家背包中删除地图商城道具的回调事件。可在事件内配合'触发的商城道具事件的玩家'和'商城道具最后变动的数量'使用。（该事件一般不太可能用到，一般为商城商品被删除才会触发）"

        public static void DzTriggerRegisterMallItemRemoveEvent(JTrigger trig)
        {
            DzTriggerRegisterSyncData(trig, "DZMID", true);
        }


        /// title = "事件响应 - 触发的商城道具事件的玩家"
        /// description = "触发的商城道具事件的玩家"
        /// comment = "可以用于【玩家获取商城道具事件】、【玩家消耗使用商城道具事件】和【玩家删除商城道具事件】后。"

        public static JPlayer DzGetTriggerMallItemPlayer()
        {
            return DzGetTriggerSyncPlayer();
        }


        /// title = "事件响应 - 触发的商城道具（实时）"
        /// description = "触发的商城道具"
        /// comment = "可以用于【玩家获取商城道具事件】、【玩家消耗使用商城道具事件】和【玩家删除商城道具事件】后。"

        public static string DzGetTriggerMallItem()
        {
            return DzGetTriggerSyncData();
        }


    }
}
