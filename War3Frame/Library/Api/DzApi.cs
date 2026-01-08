using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    public static class DzApi
    {
        // 原生 - 游戏UI
        // 一般用作创建自定义UI的父节点
        /// @CSharpLua.Template = "Jass.Japi["DzGetGameUI"]()"
        public static extern void DzGetGameUI();

        // 原生 - 玩家聊天信息框
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetChatMessage"]()"
        public static extern void DzFrameGetChatMessage();

        // 原生 - 技能按钮
        // 技能按钮:(row, column)
        // 参考物编中的技能按钮(x,y)坐标
        // (x,y)对应(column,row)反一下
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetCommandBarButton"]({0}, {1})"
        public static extern void DzFrameGetCommandBarButton(float row, float column);

        // 原生 - 英雄按钮
        // 左侧的英雄头像，参数表示第N+1个英雄，索引从0开始
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetHeroBarButton"]({0})"
        public static extern void DzFrameGetHeroBarButton(float buttonId);

        // 原生 - 英雄血条
        // 左侧的英雄头像下的血条，参数表示第N+1个英雄，索引从0开始
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetHeroHPBar"]({0})"
        public static extern void DzFrameGetHeroHPBar(float buttonId);

        // 原生 - 英雄蓝条
        // 左侧的英雄头像下的蓝条，参数表示第N+1个英雄，索引从0开始
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetHeroManaBar"]({0})"
        public static extern void DzFrameGetHeroManaBar(float buttonId);

        // 原生 - 物品栏按钮
        // 索引从0开始
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetItemBarButton"]({0})"
        public static extern void DzFrameGetItemBarButton(float buttonId);

        // 原生 - 小地图
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetMinimap"]()"
        public static extern void DzFrameGetMinimap();

        // 原生 - 小地图按钮
        // 小地图右侧竖排按钮，索引从0开始
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetMinimapButton"]({0})"
        public static extern void DzFrameGetMinimapButton(float buttonId);

        // 原生 - 设置小地图背景贴图
        /// @CSharpLua.Template = "Jass.Japi["DzSetWar3MapMap"]({0})"
        public static extern void DzSetWar3MapMap(string blp);

        // 原生 - 单位大头像
        // 小地图右侧的大头像
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetPortrait"]()"
        public static extern void DzFrameGetPortrait();

        // 原生 - 鼠标提示
        // 鼠标移动到物品或技能按钮上显示的提示窗，初始位于技能栏上方
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetTooltip"]()"
        public static extern void DzFrameGetTooltip();

        // 原生 - 上方消息框
        // 高维修费用 等消息
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetTopMessage"]()"
        public static extern void DzFrameGetTopMessage();

        // 原生 - 系统消息框
        // 包含显示消息给玩家 及 显示Debug消息等
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetUnitMessage"]()"
        public static extern void DzFrameGetUnitMessage();

        // 原生 - 界面按钮
        // 左上的菜单等按钮，索引从0开始
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetUpperButtonBarButton"]({0})"
        public static extern void DzFrameGetUpperButtonBarButton(float buttonId);

        // 原生 - 框选控件
        // 获取鼠标当前框选单位头像控件
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetInfoPanelSelectButton"]({0})"
        public static extern void DzFrameGetInfoPanelSelectButton(float index);

        // 原生 - BUFF控件
        // 获取BUFF控件地址
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetInfoPanelBuffButton"]({0})"
        public static extern void DzFrameGetInfoPanelBuffButton(float index);

        // 原生 - 农民控件
        // 获取农民控件地址
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetPeonBar"]()"
        public static extern void DzFrameGetPeonBar();

        // 原生 - 技能自动施法指示器
        // 获取技能自动施法指示器，控件类型为sprite frame
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetCommandBarButtonAutoCastIndicator"]({0})"
        public static extern void DzFrameGetCommandBarButtonAutoCastIndicator(int frame);

        // 原生 - 技能冷却指示器
        // 获取技能冷却指示器，控件类型为sprite frame
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetCommandBarButtonCooldownIndicator"]({0})"
        public static extern void DzFrameGetCommandBarButtonCoolDownIndicator(int frame);

        // 原生 - 技能右下角数字文本框体
        // 获取技能右下角数字文本框体
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetCommandBarButtonNumberOverlay"]({0})"
        public static extern void DzFrameGetCommandBarButtonNumberOverlay(int frame);

        // 原生 - 获取技能右下角数字文本控件
        // 获取技能右下角数字文本控件
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetCommandBarButtonNumberText"]({0})"
        public static extern void DzFrameGetCommandBarButtonNumberText(int frame);

        // 原生 - 游戏提示信息界面
        // 如建筑升级完成提示
        // 配合设置游戏提示信息一起用
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetWorldFrameMessage"]()"
        public static extern void DzFrameGetWorldFrameMessage();

        // 设置游戏提示信息
        // 设置游戏提示信息，如建造完成，技能没有目标等
        /// @CSharpLua.Template = "Jass.Japi["DzSimpleMessageFrameAddMessage"]({0},{1},{2},{3},{4})"
        public static extern void DzSimpleMessageFrameAddMessage(int frame, string text, float color, float duration, bool permanent);

        // 清理游戏提示信息
        /// @CSharpLua.Template = "Jass.Japi["DzSimpleMessageFrameClear"]({0})"
        public static extern void DzSimpleMessageFrameClear(int frame);

        // 新建Frame
        // 名字为fdf文件中的名字，ID默认填0。重复创建同名Frame会导致游戏退出时显示崩溃消息，如需避免可以使用Tag创建
        /// @CSharpLua.Template = "Jass.Japi["DzCreateFrame"]({0},{1},{2})"
        public static extern void DzCreateFrame(string frame, float parent, int id);

        // 新建Frame[Tag]
        // 此处名字可以自定义，类型和模版填写fdf文件中的内容。通过此函数创建的Frame无法获取到子Frame
        /// @CSharpLua.Template = "Jass.Japi["DzCreateFrameByTagName"]({0},{1},{2},{3},{4})"
        public static extern void DzCreateFrameByTagName(string frameType, string name, float parent, string template, int id);

        /// @CSharpLua.Template = "Jass.Japi["DzCreateSimpleFrame"]({0},{1},{2})"
        public static extern void DzCreateSimpleFrame(string frame, float parent, int id);

        // 销毁Frame
        // 销毁一个被重复创建过的Frame会导致游戏崩溃，重复创建同名Frame请使用Tag创建
        /// @CSharpLua.Template = "Jass.Japi["DzDestroyFrame"]({0})"
        public static extern void DzDestroyFrame(int frameId);

        // 限制鼠标移动，在frame内
        /// @CSharpLua.Template = "Jass.Japi["DzFrameCageMouse"]({0},{1})"
        public static extern void DzFrameCageMouse(int frame, bool enable);

        // 清空frame所有锚点
        /// @CSharpLua.Template = "Jass.Japi["DzFrameClearAllPoints"]({0})"
        public static extern void DzFrameClearAllPoints(int frame);

        // 获取名字为name的子FrameID:Id"
        // ID默认填0，同名时优先获取最后被创建的。非Simple类的Frame类型都用此函数来获取子Frame
        /// @CSharpLua.Template = "Jass.Japi["DzFrameFindByName"]({0},{1})"
        public static extern void DzFrameFindByName(string name, int id);

        // 获取Frame的透明度(0-255)
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetAlpha"]({0})"
        public static extern void DzFrameGetAlpha(int frame);

        // frame控件是否启用
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetEnable"]({0})"
        public static extern void DzFrameGetEnable(int frame);

        // 获取Frame的宽度
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetWidth"]({0})"
        public static extern void DzFrameGetWidth(int frame);

        // 设置模型Frame播放动画（编号）
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetAnimateByIndex"]({0},{1}.{2})"
        public static extern void DzFrameSetAnimateByIndex(int frame, float index, float flag);

        // 获取Frame的高度
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetHeight"]({0})"
        public static extern void DzFrameGetHeight(int frame);

        // 获取 Frame 的名称
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetName"]({0})"
        public static extern void DzFrameGetName(int frame);

        // 获取 Frame 的 Parent
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetParent"]({0})"
        public static extern void DzFrameGetParent(int frame);

        // 获取 Frame 内的文字
        // 支持EditBox, TextFrame, TextArea, SimpleFontString
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetText"]({0})"
        public static extern void DzFrameGetText(int frame);

        // 获取 Frame 的字数限制
        // 支持EditBox
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetTextSizeLimit"]({0})"
        public static extern void DzFrameGetTextSizeLimit(int frame);

        // 获取frame当前值
        // 支持Slider、SimpleStatusBar、StatusBar
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetValue"]({0})"
        public static extern void DzFrameGetValue(int frame);

        // 获取指定Frame的子控件
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetChild"]({0},{1})"
        public static extern void DzFrameGetChild(int frame, float index);

        // 获取指定Frame的子控件数量
        /// @CSharpLua.Template = "Jass.Japi["DzFrameGetChildrenCount"]({0})"
        public static extern void DzFrameGetChildrenCount(int frame);

        // 设置绝对位置
        // 设置 frame 的 Point 锚点 在 (x, y)
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetAbsolutePoint"]({0},{1},{2},{3})"
        public static extern void DzFrameSetAbsolutePoint(int frame, float point, float x, float y);

        // 移动所有锚点到Frame
        // 移动 frame 的 所有锚点 到 relativeFrame 上
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetAllPoints"]({0},{1})"
        public static extern void DzFrameSetAllPoints(int frame, float relativeFrame);

        // 设置frame的透明度(0-255)
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetAlpha"]({0},{1})"
        public static extern void DzFrameSetAlpha(int frame, float alpha);

        // 设置动画
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetAnimate"]({0},{1},{2})"
        public static extern void DzFrameSetAnimate(int frame, float animId, bool autoCast);

        // 设置动画进度
        // 自动播放为false时可用
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetAnimateOffset"]({0},{1})"
        public static extern void DzFrameSetAnimateOffset(int frame, float offset);

        // 启用/禁用 frame
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetEnable"]({0}, {1})"
        public static extern void DzFrameSetEnable(int frame, bool enable);

        // 设置frame获取焦点
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetFocus"]({0}, {1})"
        public static extern void DzFrameSetFocus(int frame, bool enable);

        // 设置字体
        // 设置 frame 的字体为 font, 大小 height, flag flag
        // 支持EditBox、SimpleFontString、SimpleMessageFrame以及非SimpleFrame类型的例如TEXT，flag作用未知
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetFont"]({0}, {1}, {2}, {3})"
        public static extern void DzFrameSetFont(int frame, string fileName, float height, float flag);

        // 设置最大/最小值
        // 设置 frame 的 最小值为 Min 最大值为 Max
        // 支持Slider、SimpleStatusBar、StatusBar
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetMinMaxValue"]({0}, {1}, {2})"
        public static extern void DzFrameSetMinMaxValue(int frame, float minValue, float maxValue);

        // 设置模型
        // 设置 frame 的模型文件为 modelFile ModelType:modelType Flag:flag
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetModel"]({0}, {1}, {2}, {3})"
        public static extern void DzFrameSetModel(int frame, string modelFile, float modelType, float flag);

        // 设置父窗口
        // 设置 frame 的父窗口为 parent
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetParent"]({0}, {1})"
        public static extern void DzFrameSetParent(int frame, float parent);

        // 设置相对位置
        // 设置 frame 的 Point 锚点 (跟随relativeFrame 的 relativePoint 锚点) 偏移(x, y)
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetPoint"]({0}, {1}, {2}, {3}, {4})"
        public static extern void DzFrameSetPoint(int frame, float point, float relativeFrame, float relativePoint, float x, float y);

        // 设置优先级
        // 设置 frame 优先级:int
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetPriority"]({0}, {1})"
        public static extern void DzFrameSetPriority(int frame, float priority);

        // 设置缩放
        // 设置 frame 的缩放 scale
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetScale"]({0}, {1})"
        public static extern void DzFrameSetScale(int frame, float scale);

        // 设置frame大小
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetSize"]({0}, {1}, {2})"
        public static extern void DzFrameSetSize(int frame, float w, float h);

        // 设置frame步进值
        // 支持Slider
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetStepValue"]({0}, {1})"
        public static extern void DzFrameSetStepValue(int frame, float step);

        // 设置frame文本
        // 支持EditBox, TextFrame, TextArea, SimpleFontString、GlueEditBoxWar3、SlashChatBox、TimerTextFrame、TextButtonFrame、GlueTextButton
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetText"]({0}, {1})"
        public static extern void DzFrameSetText(int frame, string text);

        // 设置frame文本对齐方式
        // 支持TextFrame、SimpleFontString、SimpleMessageFrame
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetTextAlignment"]({0}, {1})"
        public static extern void DzFrameSetTextAlignment(int frame, float align);

        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetTextColor"]({0}, {1})"
        public static extern void DzFrameSetTextColor(int frame, float color);

        // 设置frame字数限制
        // 支持EditBox
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetTextSizeLimit"]({0}, {1})"
        public static extern void DzFrameSetTextSizeLimit(int frame, float limit);

        // 设置frame贴图
        // 支持Backdrop、SimpleStatusBar
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetTexture"]({0}, {1}, {2})"
        public static extern void DzFrameSetTexture(int frame, string texture, float flag);

        // 设置提示
        // 设置 frame 的提示Frame为 tooltip
        // 设置tooltip
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetTooltip"]({0}, {1})"
        public static extern void DzFrameSetTooltip(int frame, float tooltip);

        // 设置frame当前值
        // 支持Slider、SimpleStatusBar、StatusBar
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetValue"]({0}, {1})"
        public static extern void DzFrameSetValue(int frame, float value);

        // 设置frame颜色
        /// @CSharpLua.Template = "Jass.Japi["DzFrameSetVertexColor"]({0}, {1})"
        public static extern void DzFrameSetVertexColor(int frame, float vertexColor);

        // 设置frame显示与否
        /// @CSharpLua.Template = "Jass.Japi["DzFrameShow"]({0}, {1})"
        public static extern void DzFrameShow(int frame, bool enable);

        // 游戏界面是否限制在游戏窗口内
        // 当为false时游戏里的界面可以拖到游戏外面
        /// @CSharpLua.Template = "Jass.Japi["DzFrameEnableClipRect"]({0})"
        public static extern void DzFrameEnableClipRect(bool enable);

        // 获取子SimpleFontString
        // ID默认填0，同名时优先获取最后被创建的。SimpleFontString为fdf中的Frame类型
        /// @CSharpLua.Template = "Jass.Japi["DzSimpleFontStringFindByName"]({0}, {1})"
        public static extern void DzSimpleFontStringFindByName(string name, int id);

        // 获取子SimpleFrame
        // ID默认填0，同名时优先获取最后被创建的。SimpleFrame为fdf中的Frame类型
        /// @CSharpLua.Template = "Jass.Japi["DzSimpleFrameFindByName"]({0}, {1})"
        public static extern void DzSimpleFrameFindByName(string name, int id);

        // 获取子SimpleTexture
        // ID默认填0，同名时优先获取最后被创建的。SimpleTexture为fdf中的Frame类型
        /// @CSharpLua.Template = "Jass.Japi["DzSimpleTextureFindByName"]({0}, {1})"
        public static extern void DzSimpleTextureFindByName(string name, int id);

        // [不明]自动重置原生UI锚点
        /// @CSharpLua.Template = "Jass.Japi["DzOriginalUIAutoResetPoint"]({0})"
        public static extern void DzOriginalUIAutoResetPoint(bool enable);

        // 隐藏界面元素
        // 不在地图初始化时调用则会残留小地图和时钟模型
        /// @CSharpLua.Template = "Jass.Japi["DzFrameHideInterface"]()"
        public static extern void DzFrameHideInterface();

        // 修改游戏渲染黑边
        // 上下加起来不要大于0.6
        /// @CSharpLua.Template = "Jass.Japi["DzFrameEditBlackBorders"]({0}, {1})"
        public static extern void DzFrameEditBlackBorders(float upperHeight, float bottomHeight);

        // 使用宽屏模式
        // 地图可以根据自身特点，强制打开或关闭的宽屏优化支持功能。
        // 开启宽屏模式可以解决单位被拉伸显得比较“胖”的问题。
        /// @CSharpLua.Template = "Jass.Japi["DzEnableWideScreen"]({0})"
        public static extern void DzEnableWideScreen(bool enable);

        // 修改屏幕比例(FOV)
        /// @CSharpLua.Template = "Jass.Japi["DzSetCustomFovFix"]({0})"
        public static extern void DzSetCustomFovFix(float value);

        // 获取魔兽窗口高度
        /// @CSharpLua.Template = "Jass.Japi["DzGetWindowHeight"]()"
        public static extern void DzGetWindowHeight();

        // 获取魔兽窗口宽度
        /// @CSharpLua.Template = "Jass.Japi["DzGetWindowWidth"]()"
        public static extern void DzGetWindowWidth();

        // 获取魔兽客户端高度
        // 不包括魔兽窗口边框
        /// @CSharpLua.Template = "Jass.Japi["DzGetClientHeight"]()"
        public static extern void DzGetClientHeight();

        // 获取魔兽客户端宽度
        // 不包括魔兽窗口边框
        /// @CSharpLua.Template = "Jass.Japi["DzGetClientWidth"]()"
        public static extern void DzGetClientWidth();

        // 获取魔兽窗口X坐标
        /// @CSharpLua.Template = "Jass.Japi["DzGetWindowX"]()"
        public static extern void DzGetWindowX();

        // 获取魔兽窗口Y坐标
        /// @CSharpLua.Template = "Jass.Japi["DzGetWindowY"]()"
        public static extern void DzGetWindowY();

        // 设置魔兽窗口大小
        /// @CSharpLua.Template = "Jass.Japi["DzChangeWindowSize"]({0}, {1})"
        public static extern void DzChangeWindowSize(float width, float height);

        // 判断游戏窗口是否处于活动状态
        /// @CSharpLua.Template = "Jass.Japi["DzIsWindowActive"]()"
        public static extern void DzIsWindowActive();

        // 判断按键是否按下
        /// @CSharpLua.Template = "Jass.Japi["DzIsKeyDown"]({0})"
        public static extern void DzIsKeyDown(float iKey);

        // 事件响应 - 获取触发的按键
        // 响应 [硬件] - 按键事件
        /// @CSharpLua.Template = "Jass.Japi["DzGetTriggerKey"]()"
        public static extern void DzGetTriggerKey();

        // 事件响应 - 获取触发硬件事件的玩家
        // 响应 [硬件] - 按键事件 滚轮事件 窗口大小变化事件
        /// @CSharpLua.Template = "Jass.Japi["DzGetTriggerKeyPlayer"]()"
        public static extern void DzGetTriggerKeyPlayer();

        // 鼠标是否在游戏内
        /// @CSharpLua.Template = "Jass.Japi["DzIsMouseOverUI"]()"
        public static extern void DzIsMouseOverUI();

        // 设置鼠标的坐标
        /// @CSharpLua.Template = "Jass.Japi["DzSetMousePos"]({0}, {1})"
        public static extern void DzSetMousePos(float x, float y);

        // 鼠标所在的Frame控件指针
        // 不是所有类型的Frame都能响应鼠标，能响应的有BUTTON，TEXT等
        /// @CSharpLua.Template = "Jass.Japi["DzGetMouseFocus"]()"
        public static extern void DzGetMouseFocus();

        // 获取鼠标在游戏内的坐标X
        /// @CSharpLua.Template = "Jass.Japi["DzGetMouseTerrainX"]()"
        public static extern void DzGetMouseTerrainX();

        // 获取鼠标在游戏内的坐标Y
        /// @CSharpLua.Template = "Jass.Japi["DzGetMouseTerrainY"]()"
        public static extern void DzGetMouseTerrainY();

        // 获取鼠标在游戏内的坐标Z
        /// @CSharpLua.Template = "Jass.Japi["DzGetMouseTerrainZ"]()"
        public static extern void DzGetMouseTerrainZ();

        // 获取鼠标在屏幕的坐标X
        /// @CSharpLua.Template = "Jass.Japi["DzGetMouseX"]()"
        public static extern void DzGetMouseX();

        // 获取鼠标游戏窗口坐标X
        /// @CSharpLua.Template = "Jass.Japi["DzGetMouseXRelative"]()"
        public static extern void DzGetMouseXRelative();

        // 获取鼠标在屏幕的坐标Y
        /// @CSharpLua.Template = "Jass.Japi["DzGetMouseY"]()"
        public static extern void DzGetMouseY();

        // 获取鼠标游戏窗口坐标Y
        /// @CSharpLua.Template = "Jass.Japi["DzGetMouseYRelative"]()"
        public static extern void DzGetMouseYRelative();

        // 获取鼠标指向的单位
        /// @CSharpLua.Template = "Jass.Japi["DzGetUnitUnderMouse"]()"
        public static extern void DzGetUnitUnderMouse();

        // 事件响应 - 获取滚轮变化值
        // 响应 [硬件] - 鼠标滚轮事件，正负区分上下
        /// @CSharpLua.Template = "Jass.Japi["DzGetWheelDelta"]()"
        public static extern void DzGetWheelDelta();

        // 设置可破坏物位置
        /// @CSharpLua.Template = "Jass.Japi["DzDestructablePosition"]({0}, {1}, {2})"
        public static extern void DzDestructablePosition(float d, float x, float y);

        // 新建地形装饰物
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadCreate"]({0}, {1}, {2}, {3}, {4}. {5}, {6})"
        public static extern void DzDoodadCreate(int id, float var, float x, float y, float z, float rotate, float scale);

        // 设置装饰物模型
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetModel"]({0}, {1})"
        public static extern void DzDoodadSetModel(float doodad, string path);

        // 删除装饰物
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadRemove"]({0})"
        public static extern void DzDoodadRemove(float doodad);

        // 让指定装饰物播放动画
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetAnimation"]({0}, {1}, {2})"
        public static extern void DzDoodadSetAnimation(float doodad, string animName, bool animRandom);

        // 设置装饰物动画播放速度
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetTimeScale"]({0}, {1})"
        public static extern void DzDoodadSetTimeScale(float doodad, float scale);

        // 设置装饰物位置到坐标xyz
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetPosition"]({0}, {1}, {2}, {3})"
        public static extern void DzDoodadSetPosition(float doodad, float x, float y, float z);

        // 设置装饰物颜色
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetColor"]({0}, {1}, {2}, {3}, {4})"
        public static extern void DzDoodadSetColor(float doodad, float r, float g, float b, float a);

        // 设置装饰物显示/隐藏
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetVisible"]({0}, {1})"
        public static extern void DzDoodadSetVisible(float doodad, bool enable);

        // 设置装饰物队伍颜色
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetTeamColor"]({0}, {1})"
        public static extern void DzDoodadSetTeamColor(float doodad, object color);

        // 设置装饰物尺寸
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetOrientMatrixScale"]({0}, {1}, {2}, {3})"
        public static extern void DzDoodadSetOrientMatrixScale(float doodad, float x, float y, float z);

        // 重置指定装饰物大小
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetOrientMatrixResize"]({0})"
        public static extern void DzDoodadSetOrientMatrixResize(float doodad);

        // 设置装饰物旋转
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadSetOrientMatrixRotate"]({0}, {1}, {2}, {3}, {4})"
        public static extern void DzDoodadSetOrientMatrixRotate(float doodad, float angle, float axisX, float axisY, float axisZ);

        // 获取装饰物的X坐标
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadGetX"]({0})"
        public static extern void DzDoodadGetX(float doodad);

        // 获取装饰物的Y坐标
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadGetY"]({0})"
        public static extern void DzDoodadGetY(float doodad);

        // 获取装饰物的Z坐标
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadGetZ"]({0})"
        public static extern void DzDoodadGetZ(float doodad);

        // 获取装饰物动画播放速度
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadGetTimeScale"]({0})"
        public static extern void DzDoodadGetTimeScale(float doodad);

        // 获取装饰物当前动画编号
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadGetCurrentAnimationIndex"]({0})"
        public static extern void DzDoodadGetCurrentAnimationIndex(float doodad);

        // 获取装饰物动画数量
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadGetAnimationCount"]({0})"
        public static extern void DzDoodadGetAnimationCount(float doodad);

        // 获取装饰物某个动画名
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadGetAnimationName"]({0}, {1})"
        public static extern void DzDoodadGetAnimationName(float doodad, float index);

        // 获取装饰物某个动画时间
        /// @CSharpLua.Template = "Jass.Japi["DzDoodadGetAnimationTime"]({0}, {1})"
        public static extern void DzDoodadGetAnimationTime(float doodad, float index);

        // 降低玩家科技等级
        /// @CSharpLua.Template = "Jass.Japi["DzRemovePlayerTechResearched"]({0}, {1}, {2})"
        public static extern void DzRemovePlayerTechResearched(int whichPlayer, float techId, float removeLevels);

        // 替换单位类型
        // 替换whichUnit的单位类型为:id
        // 会替换大头像中的模型
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitID"]({0}, {1})"
        public static extern void DzSetUnitID(int whichUnit, int id);

        // 替换单位模型
        // 替换whichUnit的模型:path(必须带.mdl)
        // 不会替换大头像中的模型
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitModel"]({0}, {1})"
        public static extern void DzSetUnitModel(int whichUnit, string model);

        // 设置指定单位的名字
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitName"]({0}, {1})"
        public static extern void DzSetUnitName(int whichUnit, string name);

        // 设置指定单位描述
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitDescription"]({0}, {1})"
        public static extern void DzSetUnitDescription(int whichUnit, string description);

        // 设置单位头像模型
        // 用于修改指定单位的头像模型，预设英雄模型直接作为头像模型会显示异常，需找到对应的大头像地址进行绑定。
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitPortrait"]({0}, {1})"
        public static extern void DzSetUnitPortrait(int whichUnit, string model);

        // 设置指定英雄的称谓
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitProperName"]({0}, {1})"
        public static extern void DzSetUnitProperName(int whichUnit, string name);

        // 设置单位类型名
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitTypeName"]({0}, {1})"
        public static extern void DzSetUnitTypeName(float uid, string name);

        // 设置英雄类型称谓名
        /// @CSharpLua.Template = "Jass.Japi["DzSetHeroTypeProperName"]({0}, {1})"
        public static extern void DzSetHeroTypeProperName(float uid, string name);

        // 单位缩放
        /// @CSharpLua.Template = "Jass.Japi["DzSetWidgetSpriteScale"]({0}, {1})"
        public static extern void DzSetWidgetSpriteScale(int whichUnit, float scale);

        // 设置单位位置 - 本地调用
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitPosition"]({0}, {1}, {2})"
        public static extern void DzSetUnitPosition(int whichUnit, float x, float y);

        // 替换单位贴图
        // 只能替换模型中有Replaceable ID x 贴图的模型，ID为索引。不会替换大头像中的模型
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitTexture"]({0}, {1}, {2})"
        public static extern void DzSetUnitTexture(int whichUnit, string path, float texId);

        // 设置单位的鼠标指向ui和血条显示/隐藏
        // 隐藏鼠标指向物品/单位时显示的血条和UI
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitPreselectUIVisible"]({0}, {1})"
        public static extern void DzSetUnitPreselectUIVisible(int whichUnit, bool visible);

        // 是否单位的护甲类型
        /// @CSharpLua.Template = "Jass.Japi["DzIsUnitDefenseType"]({0}, {1})"
        public static extern void DzIsUnitDefenseType(int whichUnit, float defenseType);

        // 设置单位的护甲类型
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitDefenseType"]({0}, {1})"
        public static extern void DzSetUnitDefenseType(int whichUnit, float defenseType);

        // 是否单位的攻击类型
        /// @CSharpLua.Template = "Jass.Japi["DzIsUnitAttackType"]({0}, {1}, {2})"
        public static extern void DzIsUnitAttackType(int whichUnit, float index, object attackType);

        // 设置单位的攻击类型
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitAttackType"]({0}, {1}, {2})"
        public static extern void DzSetUnitAttackType(int whichUnit, float index, object attackType);

        // 禁用攻击
        // 设置单位的攻击状态，重复设置同一状态会引发bug
        /// @CSharpLua.Template = "Jass.Japi["DzUnitDisableAttack"]({0}, {1})"
        public static extern void DzUnitDisableAttack(int whichUnit, bool disable);

        // 禁用道具
        // 设置单位的道具状态，重复设置同一状态会引发bug
        /// @CSharpLua.Template = "Jass.Japi["DzUnitDisableInventory"]({0}, {1})"
        public static extern void DzUnitDisableInventory(int whichUnit, bool disable);

        // 沉默单位
        // 设置单位的沉默状态，重复设置同一状态会引发bug
        /// @CSharpLua.Template = "Jass.Japi["DzUnitSilence"]({0}, {1})"
        public static extern void DzUnitSilence(int whichUnit, bool disable);

        // 修改单位透明度
        /// @CSharpLua.Template = "Jass.Japi["DzUnitChangeAlpha"]({0}, {1}, {2})"
        public static extern void DzUnitChangeAlpha(int whichUnit, float alpha, bool forceUpdate);

        // 设置单位是否可以选中
        /// @CSharpLua.Template = "Jass.Japi["DzUnitSetCanSelect"]({0}, {1})"
        public static extern void DzUnitSetCanSelect(int whichUnit, bool state);

        // 设置单位是否可以被设置为目标
        /// @CSharpLua.Template = "Jass.Japi["DzUnitSetTargetable"]({0}, {1})"
        public static extern void DzUnitSetTargetAble(int whichUnit, bool state);

        // 设置单位移动类型
        // 设置单位实例的移动类型
        /// @CSharpLua.Template = "Jass.Japi["DzUnitSetMoveType"]({0}, {1})"
        public static extern void DzUnitSetMoveType(int whichUnit, string moveType);

        // 设置单位普攻弹道速度
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitMissileSpeed"]({0}, {1})"
        public static extern void DzSetUnitMissileSpeed(int whichUnit, float speed);

        // 设置单位普攻弹道模型
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitMissileModel"]({0}, {1})"
        public static extern void DzSetUnitMissileModel(int whichUnit, string path);

        // 设置单位普攻弹道弧度
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitMissileArc"]({0}, {1})"
        public static extern void DzSetUnitMissileArc(int whichUnit, float arc);

        // 设置单位普攻弹道自导允许
        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitMissileHoming"]({0}, {1})"
        public static extern void DzSetUnitMissileHoming(int whichUnit, bool enable);

        // 获取升级所需经验
        // 获取单位 unit 的 level级 升级所需经验
        /// @CSharpLua.Template = "Jass.Japi["DzGetUnitNeededXP"]({0}, {1})"
        public static extern void DzGetUnitNeededXP(int whichUnit, float level);

        // 找出单位的指定技能
        /// @CSharpLua.Template = "Jass.Japi["DzUnitFindAbility"]({0}, {1})"
        public static extern void DzUnitFindAbility(int whichUnit, float abilityCode);

        // 获取单位普攻技能
        /// @CSharpLua.Template = "Jass.Japi["DzGetAttackAbility"]({0})"
        public static extern void DzGetAttackAbility(int whichUnit);

        // 结束单位普攻技能CD
        /// @CSharpLua.Template = "Jass.Japi["DzAttackAbilityEndCooldown"]({0})"
        public static extern void DzAttackAbilityEndCoolDown(int whichUnit);

        // 设置道具模型
        /// @CSharpLua.Template = "Jass.Japi["DzItemSetModel"]({0}, {1})"
        public static extern void DzItemSetModel(float whichItem, string path);

        // 设置道具颜色
        /// @CSharpLua.Template = "Jass.Japi["DzItemSetVertexColor"]({0}, {1})"
        public static extern void DzItemSetVertexColor(float whichItem, float color);

        // 设置道具透明度
        /// @CSharpLua.Template = "Jass.Japi["DzItemSetAlpha"]({0}, {1})"
        public static extern void DzItemSetAlpha(float whichItem, float alpha);

        // 设置物品头像
        /// @CSharpLua.Template = "Jass.Japi["DzItemSetPortrait"]({0}, {1})"
        public static extern void DzItemSetPortrait(float whichItem, string modelPath);

        // 设置技能数据-字符串
        /// @CSharpLua.Template = "Jass.Japi["DzAbilitySetStringData"]({0}, {1}, {2})"
        public static extern void DzAbilitySetStringData(float whichAbility, string key, string value);

        // 设置技能启用/禁用
        // 只对主动技能有效，可以对不同单位使用。
        /// @CSharpLua.Template = "Jass.Japi["DzAbilitySetEnable"]({0}, {1}, {2})"
        public static extern void DzAbilitySetEnable(float whichAbility, bool enable, bool hideUI);

        // 复活单位/英雄
        /// @CSharpLua.Template = "Jass.Japi["DzReviveUnit"]({0}, {1}, {2}, {3}, {4})"
        public static extern void DzReviveUnit(int whichUnit, float hp, float mp, float x, float y);

        /// @CSharpLua.Template = "Jass.Japi["DzSetUnitDataCacheInteger"]({0}, {1}, {2}, {3})"
        public static extern void DzSetUnitDataCacheInteger(int whichUnit, int id, float index, float v);

        /// @CSharpLua.Template = "Jass.Japi["DzUnitUIAddLevelArrayInteger"]({0}, {1}, {2}, {3})"
        public static extern void DzUnitUIAddLevelArrayInteger(int whichUnit, int id, float lv, float v);

        // 绑定特效
        // 绑定特效到对象上，可以给单位/物品绑定
        /// @CSharpLua.Template = "Jass.Japi["DzBindEffect"]({0}, {1}, {2})"
        public static extern void DzBindEffect(float widget, string attach, float whichEffect);

        // 解除绑定特效
        // 可以让绑定在单位身上的特效分离出来，被分离的特效能设置坐标、缩放
        /// @CSharpLua.Template = "Jass.Japi["DzUnbindEffect"]({0})"
        public static extern void DzUnbindEffect(float whichEffect);

        // 设置特效透明度
        /// @CSharpLua.Template = "Jass.Japi["DzSetEffectVertexAlpha"]({0}, {1})"
        public static extern void DzSetEffectVertexAlpha(float whichEffect, float alpha);

        // 获取特效透明度
        /// @CSharpLua.Template = "Jass.Japi["DzGetEffectVertexAlpha"]({0})"
        public static extern void DzGetEffectVertexAlpha(float whichEffect);

        // 设置特效颜色
        // 设置特效颜色，透明无效
        /// @CSharpLua.Template = "Jass.Japi["DzSetEffectVertexColor"]({0}, {1}, {2}, {3}, {4})"
        public static extern void DzSetEffectVertexColor(float whichEffect, float r, float g, float b, float a);

        // 获取特效颜色
        /// @CSharpLua.Template = "Jass.Japi["DzGetEffectVertexColor"]({0})"
        public static extern void DzGetEffectVertexColor(float whichEffect);

        // 设置特效坐标，立即移动
        /// @CSharpLua.Template = "Jass.Japi["DzSetEffectPos"]({0}, {1}, {2}, {3})"
        public static extern void DzSetEffectPos(float whichEffect, float x, float y, float z);

        // 特效缩放
        /// @CSharpLua.Template = "Jass.Japi["DzSetEffectScale"]({0}, {1})"
        public static extern void DzSetEffectScale(float whichEffect, float scale);

        // 设置特效播放（编号）动画
        // 按照动画编号播放特效动画
        /// @CSharpLua.Template = "Jass.Japi["DzSetEffectAnimation"]({0}, {1}, {2})"
        public static extern void DzSetEffectAnimation(float whichEffect, float index, float flag);

        // 设置特效播放（动作名）动画
        // 按照动作名播放特效动画
        /// @CSharpLua.Template = "Jass.Japi["DzPlayEffectAnimation"]({0}, {1}, {2})"
        public static extern void DzPlayEffectAnimation(float whichEffect, string anim, string link);

        // 设置显示/隐藏
        /// @CSharpLua.Template = "Jass.Japi["DzSetEffectVisible"]({0}, {1})"
        public static extern void DzSetEffectVisible(float whichEffect, bool enable);

        // 设置特效的模型
        /// @CSharpLua.Template = "Jass.Japi["DzSetEffectModel"]({0}, {1})"
        public static extern void DzSetEffectModel(float whichEffect, string model);

        // 设置特效队伍颜色
        /// @CSharpLua.Template = "Jass.Japi["DzSetEffectTeamColor"]({0}, {1})"
        public static extern void DzSetEffectTeamColor(float whichEffect, float playerId);

        // 转换屏幕坐标到世界x坐标
        /// @CSharpLua.Template = "Jass.Japi["DzConvertScreenPositionX"]({0}, {1})"
        public static extern void DzConvertScreenPositionX(float x, float y);

        // 转换屏幕坐标到世界y坐标
        /// @CSharpLua.Template = "Jass.Japi["DzConvertScreenPositionY"]({0}, {1})"
        public static extern void DzConvertScreenPositionY(float x, float y);

        // 转换世界坐标为屏幕坐标[异步]
        /// @CSharpLua.Template = "Jass.Japi["DzConvertWorldPosition"]({0}, {1}, {2}, {3})"
        public static extern void DzConvertWorldPosition(float x, float y, float z, object callback);

        // 获取转换后的屏幕 X 坐标
        // 用于转换世界坐标为屏幕坐标[异步]callback中
        /// @CSharpLua.Template = "Jass.Japi["DzGetConvertWorldPositionX"]()"
        public static extern void DzGetConvertWorldPositionX();

        // 获取转换后的屏幕 Y 坐标
        // 用于转换世界坐标为屏幕坐标[异步]callback中
        /// @CSharpLua.Template = "Jass.Japi["DzGetConvertWorldPositionY"]()"
        public static extern void DzGetConvertWorldPositionY();

        // 转换世界坐标为屏幕深度[同步]
        /// @CSharpLua.Template = "Jass.Japi["DzConvertWorldPositionDepth"]({0}, {1}, {2})"
        public static extern void DzConvertWorldPositionDepth(float x, float y, float z);

        // 转换世界坐标为屏幕x坐标[同步]
        /// @CSharpLua.Template = "Jass.Japi["DzConvertWorldPositionX"]({0}, {1}, {2})"
        public static extern void DzConvertWorldPositionX(float x, float y, float z);

        // 转换世界坐标为屏幕y坐标[同步]
        /// @CSharpLua.Template = "Jass.Japi["DzConvertWorldPositionY"]({0}, {1}, {2})"
        public static extern void DzConvertWorldPositionY(float x, float y, float z);

        // 转换地图坐标为小地图x坐标
        // 转化地图坐标为小地图X坐标，小地图左下角为（0,0）
        /// @CSharpLua.Template = "Jass.Japi["DzFrameWorldToMinimapPosX"]({0}, {1})"
        public static extern void DzFrameWorldToMinimapPosX(float x, float y);

        // 转换地图坐标为小地图y坐标
        // 转化地图坐标为小地图y坐标，小地图左下角为（0,0）
        /// @CSharpLua.Template = "Jass.Japi["DzFrameWorldToMinimapPosY"]({0}, {1})"
        public static extern void DzFrameWorldToMinimapPosY(float x, float y);

        // 打开/关闭自定义单位的小地图图标
        // 打开关闭自定义单位的小地图图标，配合自定义指定单位的小地图图标使用
        /// @CSharpLua.Template = "Jass.Japi["DzWidgetSetMinimapIconEnable"]({0}, {1})"
        public static extern void DzWidgetSetMinimapIconEnable(int whichUnit, bool enable);

        // 自定义指定单位的小地图图标
        // 图标大小只支持16*16，设置图标之前需要打开指定单位的小地图图标显示
        // 需要 J.UnitSetUsesAltIcon(whichUnit, true) 开启中立建筑的小地图特殊标志
        /// @CSharpLua.Template = "Jass.Japi["DzWidgetSetMinimapIcon"]({0}, {1})"
        public static extern void DzWidgetSetMinimapIcon(int whichUnit, string path);

        // 加载Toc文件列表
        /// @CSharpLua.Template = "Jass.Japi["DzGetLocale"]()"
        public static extern void DzGetLocale();

        // 获取客户端语言
        // 对不同语言客户端返回不同
        /// @CSharpLua.Template = "Jass.Japi["DzLoadToc"]({0})"
        public static extern void DzLoadToc(string fileName);

        // 设置内存数值
        // 设置内存数据 address=value
        /// @CSharpLua.Template = "Jass.Japi["DzSetMemory"]({0}, {1})"
        public static extern void DzSetMemory(float address, float value);

        // 保存内存数据
        /// @CSharpLua.Template = "Jass.Japi["DzSaveMemoryCache"]({0})"
        public static extern void DzSaveMemoryCache(string cache);

        // 设置加速倍率
        /// @CSharpLua.Template = "Jass.Japi["DzSetSpeed"]({0})"
        public static extern void DzSetSpeed(float ratio);

        // 获取字符串数量
        /// @CSharpLua.Template = "Jass.Japi["DzGetJassStringTableCount"]()"
        public static extern void DzGetJassStringTableCount();

        // 清除所有模型内存缓存
        /// @CSharpLua.Template = "Jass.Japi["DzModelRemoveAllFromCache"]()"
        public static extern void DzModelRemoveAllFromCache();

        // 清除指定模型内存缓存
        /// @CSharpLua.Template = "Jass.Japi["DzModelRemoveFromCache"]({0})"
        public static extern void DzModelRemoveFromCache(string path);

        // 异步执行函数
        // 执行脱离代码段之内
        /// @CSharpLua.Template = "Jass.Japi["DzExecuteFunc"]({0})"
        public static extern void DzExecuteFunc(string funcName);

        // 取 RGBA 色值
        // 将RGBA转换为色值，返回一个整数，用于设置DZ其他接口如Frame、Effect的颜色
        /// @CSharpLua.Template = "Jass.Japi["DzGetColor"]({0}, {1}, {2}, {3})"
        public static extern void DzGetColor(float r, float g, float b, float a);

        // 获取商店目标
        // 获取指定商店选中指定玩家的哪个单位
        /// @CSharpLua.Template = "Jass.Japi["DzGetActivePatron"]({0}, {1})"
        public static extern void DzGetActivePatron(float whichStore, int whichPlayer);

        // 获取玩家选中的第n个单位
        // 异步返回玩家选中的单位
        /// @CSharpLua.Template = "Jass.Japi["DzGetLocalSelectUnit"]({0})"
        public static extern void DzGetLocalSelectUnit(float index);

        // 获取玩家选中的单位数量
        // 异步获取玩家选中的单位数量，返回整数
        /// @CSharpLua.Template = "Jass.Japi["DzGetLocalSelectUnitCount"]()"
        public static extern void DzGetLocalSelectUnitCount();

        // 获取当前选择的单位
        // 异步获取当前预览窗口显示的单位
        /// @CSharpLua.Template = "Jass.Japi["DzGetSelectedLeaderUnit"]()"
        public static extern void DzGetSelectedLeaderUnit();

        // 获取聊天窗是否打开
        // 获取当前玩家聊天窗是否打开，异步获取
        /// @CSharpLua.Template = "Jass.Japi["DzIsChatBoxOpen"]()"
        public static extern void DzIsChatBoxOpen();

        // 获取FPS帧数
        /// @CSharpLua.Template = "Jass.Japi["DzGetFPS"]()"
        public static extern void DzGetFPS();

        // 设置FPS显示/隐藏
        /// @CSharpLua.Template = "Jass.Japi["DzToggleFPS"]()"
        public static extern void DzToggleFPS(bool show);

        // 解锁魔兽高清图片的512像素限制
        /// @CSharpLua.Template = "Jass.Japi["DzUnlockBlpSizeLimit"]({0})"
        public static extern void DzUnlockBlpSizeLimit(bool enable);

        // 解锁JASS字节码限制
        /// @CSharpLua.Template = "Jass.Japi["DzUnlockOpCodeLimit"]({0})"
        public static extern void DzUnlockOpCodeLimit(bool enable);

        // 设置剪切板内容
        /// @CSharpLua.Template = "Jass.Japi["DzSetClipboard"]({0})"
        public static extern void DzSetClipboard(string content);

        // 打开QQ群链接
        // 调用后打开QQ群链接，必须以http://qm.qq.com开头，每分钟只会触发一次。
        /// @CSharpLua.Template = "Jass.Japi["DzOpenQQGroupUrl"]({0})"
        public static extern void DzOpenQQGroupUrl(bool url);

        // 监听建筑选位置
        /// @CSharpLua.Template = "Jass.Japi["DzRegisterOnBuildLocal"]({0})"
        public static extern void DzRegisterOnBuildLocal(object func);

        // 获取建造的命令id
        // 用于监听建筑选位置后
        /// @CSharpLua.Template = "Jass.Japi["DzGetOnBuildOrderId"]()"
        public static extern void DzGetOnBuildOrderId();

        // 获取建造的命令类型
        // 用于监听建筑选位置后
        /// @CSharpLua.Template = "Jass.Japi["DzGetOnBuildOrderType"]()"
        public static extern void DzGetOnBuildOrderType();

        // 获取预建造对象
        // 用于监听建筑选位置后
        /// @CSharpLua.Template = "Jass.Japi["DzGetOnBuildAgent"]()"
        public static extern void DzGetOnBuildAgent();

        // 监听技能预选目标
        /// @CSharpLua.Template = "Jass.Japi["DzRegisterOnTargetLocal"]({0})"
        public static extern void DzRegisterOnTargetLocal(object func);

        // 获取监听到的技能
        // 用于监听技能预选后
        /// @CSharpLua.Template = "Jass.Japi["DzGetOnTargetAbilId"]()"
        public static extern void DzGetOnTargetAbilityId();

        // 获取监听到技能预选命令
        // 用于监听技能预选后
        /// @CSharpLua.Template = "Jass.Japi["DzGetOnTargetOrderId"]()"
        public static extern void DzGetOnTargetOrderId();

        // 获取监听到技能预选命令类型
        // 用于监听技能预选后
        /// @CSharpLua.Template = "Jass.Japi["DzGetOnTargetOrderType"]()"
        public static extern void DzGetOnTargetOrderType();

        // 获取监听到技能预选目标
        // 用于监听技能预选后
        /// @CSharpLua.Template = "Jass.Japi["DzGetOnTargetAgent"]()"
        public static extern void DzGetOnTargetAgent();

        // 获取监听到技能预选目标
        // 用于监听技能预选后
        /// @CSharpLua.Template = "Jass.Japi["DzGetOnTargetInstantTarget"]()"
        public static extern void DzGetOnTargetInstantTarget();

        // 同步游戏数据
        // 同步 标签：prefix  发送数据：data
        /// @CSharpLua.Template = "Jass.Japi["DzSyncData"]({0}, {1})"
        public static extern void DzSyncData(string prefix, string data);

        // 同步游戏数据(立刻)
        // 立即同步 标签：prefix  发送数据：data
        /// @CSharpLua.Template = "Jass.Japi["DzSyncDataImmediately"]({0}, {1})"
        public static extern void DzSyncDataImmediately(string prefix, string data);

        // 同步游戏数据（指定长度）
        // 可以按长度同步数据
        /// @CSharpLua.Template = "Jass.Japi["DzSyncBuffer"]({0}, {1}, {2})"
        public static extern void DzSyncBuffer(string prefix, string data, float len);

        // 数据同步
        // 标签为 prefix 的数据被同步 | 来自平台:server
        // 来自平台的参数填false
        /// @CSharpLua.Template = "Jass.Japi["DzTriggerRegisterSyncData"]({0}, {1}, {2})"
        public static extern bool DzTriggerRegisterSyncData(float trig, string prefix, bool server);

        // 事件响应 - 获取同步的数据
        // 响应 [同步] - 同步消息事件
        /// @CSharpLua.Template = "Jass.Japi["DzGetTriggerSyncData"]()"
        public static extern string DzGetTriggerSyncData();

        // 事件响应 - 获取同步数据的玩家
        // 响应 [同步] - 同步消息事件
        /// @CSharpLua.Template = "Jass.Japi["DzGetTriggerSyncPlayer"]()"
        public static extern int DzGetTriggerSyncPlayer();

        // 注册实时购买商品事件（玩家获得平台道具事件）
        // 玩家在游戏中购买商城道具触发，可以配合打开商城界面使用，读取用实时购买玩家和实时购买商品
        // 玩家背包中新获得了当前地图道具的回调事件，用于地图实现玩家在游戏内商城购买成功后在游戏内立即生效。
        // 可在事件内配合<事件响应 - 获取同步数据的玩家><事件响应 - 获取同步的数据>获得回调数据
        public static void DzTriggerRegisterMallItemSyncData(float trig)
        {
            DzTriggerRegisterSyncData(trig, "DZMIA", true);
        }

        // 玩家消耗/使用商城道具事件
        // 注册玩家消耗地图商城道具事件（实时）
        // 玩家背包中消耗或使用商城道具的回调事件。
        // 可在事件内配合[触发的商城道具事件的玩家]、[触发的商城道具]和[商城道具最后变动的数量]使用。
        public static void DzTriggerRegisterMallItemConsumeEvent(float trig)
        {
            DzTriggerRegisterSyncData(trig, "DZMIC", true);
        }

        // 玩家删除商城道具事件
        // 注册玩家删除地图商城道具事件（实时）
        // 玩家背包中删除地图商城道具的回调事件。
        // 可在事件内配合[触发的商城道具事件的玩家]和[商城道具最后变动的数量]使用。（该事件一般不太可能用到，一般为商城商品被删除才会触发）
        public static void DzTriggerRegisterMallItemRemoveEvent(float trig)
        {
            DzTriggerRegisterSyncData(trig, "DZMID", true);
        }

        // 事件响应 - 实时获得地图商城道具的玩家
        // 实时获得地图商城道具的玩家
        // 获取是哪位玩家获得了平台道具。仅限在玩家实时获得地图商城道具事件内使用。
        public static int DzGetTriggerMallItemPlayer()
        {
            return DzGetTriggerSyncPlayer();
        }

        // 事件响应 - 实时获得的地图商城道具
        // 实时获得的地图商城道具
        // 获取实时购买的地图商城道具。仅限在玩家实时获得地图商城道具事件内使用。
        public static string DzGetTriggerMallItem()
        {
            return DzGetTriggerSyncData();
        }

        // 全局存档变化事件
        // 本局游戏或其他游戏保存的全局存档都会触发这个事件，请使用[同步]分类下的获取同步数据来获得发生变化的全局存档KEY值
        public static void DzMap_Global_ChangeMsg(float trig)
        {
            DzTriggerRegisterSyncData(trig, "DZGAU", true);
        }

        // 获取公会名称
        /// @CSharpLua.Template = "Jass.Japi["DzAPI_Map_GetGuildName"]({0})"
        public static extern string DzAPI_Map_GetGuildName(int whichPlayer);

        // 获取全局服务器存档值
        /// @CSharpLua.Template = "Jass.Japi["DzAPI_Map_GetMapConfig"]({0})"
        public static extern int DzAPI_Map_GetMapConfig(string key);

        // 玩家是否拥有该商城道具（平台地图商城）
        // 平台地图商城玩家拥有该道具返还true
        /// @CSharpLua.Template = "Jass.Japi["DzAPI_Map_HasMallItem"]({0}, {1})"
        public static extern bool DzAPI_Map_HasMallItem(int whichPlayer, string key);

        /// @CSharpLua.Template = "Jass.Japi["RequestExtraBooleanData"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
        public static extern bool RequestExtraBooleanData(int dataType, int? whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6);

        /// @CSharpLua.Template = "Jass.Japi["RequestExtraIntegerData"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
        public static extern int RequestExtraIntegerData(int dataType, int? whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6);

        /// @CSharpLua.Template = "Jass.Japi["RequestExtraRealData"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
        public static extern float RequestExtraRealData(int dataType, int? whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6);

        /// @CSharpLua.Template = "Jass.Japi["RequestExtraStringData"]({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})"
        public static extern string RequestExtraStringData(int dataType, int? whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6);

        // 活动完成
        // 完成平台活动[RPG大厅限定]
        // 用作完成某个任务，发奖励
        public static void DzMap_MissionComplete(int whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(1, whichPlayer, key, value, false, 0, 0, 0);
        }

        // 用作取服务器上的活动数据
        public static string DzMap_GetActivityData()
        {
            return RequestExtraStringData(2, null, null, null, false, 0, 0, 0);
        }

        // 获取玩家地图等级
        public static int DzMap_GetMapLevel(int whichPlayer)
        {
            return RequestExtraIntegerData(3, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 保存服务器存档
        // [错误代码]
        // 1750 地图未开通服务器存档功能
        // 1753 存档数据不一致
        // 1757 上传频率超限
        // 1758 超过每局最大值
        // 1759 数据类型不正确
        // 1191 存档变量Key长度超过64位
        // 1192 存档数量超过上限
        // 1941 服务器存档写入频率异常
        // 1250 增加的值超出上限（服务器存档防刷）
        public static bool DzMap_SaveServerValue(int whichPlayer, string key, string value)
        {
            return RequestExtraBooleanData(4, whichPlayer, key, value, false, 0, 0, 0);
        }

        // 获取服务器存档
        // [错误代码]
        // 1190 存档初始化加载失败
        public static string DzMap_GetServerValue(int whichPlayer, string key)
        {
            return RequestExtraStringData(5, whichPlayer, key, null, false, 0, 0, 0);
        }

        // 玩家是否参与过内测
        public static bool DzMap_IsPresetAward(int whichPlayer)
        {
            if (IsServerAlready(whichPlayer))
            {
                return "1" == DzMap_GetServerValue(whichPlayer, "preset_map_award");
            }
            return false;
        }

        // 读取加载服务器存档时的错误码
        public static int DzMap_GetServerValueErrorCode(int whichPlayer)
        {
            return RequestExtraIntegerData(6, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 读取玩家服务器存档成功
        // 如果返回false代表读取失败,反之成功,之后游戏里平台不会再发送“服务器保存失败”的信息，所以希望地图作者在游戏开始给玩家发下信息服务器存档是否正确读取。
        public static bool IsServerAlready(int whichPlayer)
        {
            var res = int.TryParse(DzMap_GetServerValueErrorCode(whichPlayer).ToString(), out int code);
            return res && code == 0;
        }

        // 统计-提交地图数据
        // 设置房间显示的数据
        // 为服务器存档显示的数据，对应作者之家的房间key
        public static void DzMap_Stat_SetStat(int whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(7, whichPlayer, key, value, false, 0, 0, 0);
        }

        // 天梯-统计数据
        // 天梯提交字符串数据
        public static void DzMap_Ladder_SetStat(int whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(8, whichPlayer, key, value, false, 0, 0, 0);
        }

        // 天梯提交获得称号
        public static void DzMap_Ladder_SubmitTitle(int whichPlayer, string value)
        {
            DzMap_Ladder_SetStat(whichPlayer, value, "1");
        }

        // 设置玩家额外分
        public static void DzMap_Ladder_SubmitPlayerExtraExp(int whichPlayer, string value)
        {
            DzMap_Ladder_SetStat(whichPlayer, "ExtraExp", value);
        }

        // 天梯-统计数据
        public static void DzMap_Ladder_SetPlayerStat(int whichPlayer, string key, string value)
        {
            RequestExtraIntegerData(9, whichPlayer, key, value, false, 0, 0, 0);
        }

        // 天梯提交玩家排名
        public static void DzMap_Ladder_SubmitPlayerRank(int whichPlayer, int value)
        {
            DzMap_Ladder_SetPlayerStat(whichPlayer, "RankIndex", value.ToString());
        }

        // 检查是否大厅地图
        // 判断当前地图是否rpg大厅来的
        public static bool DzMap_IsRPGLobby()
        {
            return RequestExtraBooleanData(10, null, null, null, false, 0, 0, 0);
        }

        // 获取当前游戏时间
        // 获取创建地图的游戏时间
        // 时间换算为时间戳
        public static float DzMap_GetGameStartTime()
        {
            return RequestExtraIntegerData(11, null, null, null, false, 0, 0, 0);
        }

        // 判断地图是否在RPG天梯
        public static bool DzMap_IsRPGLadder()
        {
            return RequestExtraBooleanData(12, null, null, null, false, 0, 0, 0);
        }

        // 本局游戏的地图模式
        // 获取本局游戏所选择地图模式，地图模式均由作者在作者之家进行配置
        // 包括天梯排位赛模式、快速匹配模式、建房间时房主所选定的地图模式
        public static bool DzMap_GetMatchType()
        {
            return RequestExtraBooleanData(13, null, null, null, false, 0, 0, 0);
        }

        // 获取天梯等级
        // 取值1~25，青铜V是1级
        public static bool DzMap_GetLadderLevel(int whichPlayer)
        {
            return RequestExtraBooleanData(14, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 获取天梯排名
        // 排名>1000的获取值为0
        public static bool DzMap_GetLadderRank(int whichPlayer)
        {
            return RequestExtraBooleanData(17, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 获取玩家地图等级排名
        // 排名>100的获取值为0
        public static bool DzMap_GetMapLevelRank(int whichPlayer)
        {
            return RequestExtraBooleanData(18, whichPlayer, null, null, false, 0, 0, 0);
        }
        // 获取公会职责
        // 获取公会职责 Member=10 Admin=20 Leader=30
        public static bool DzMap_GetGuildRole(int whichPlayer)
        {
            return RequestExtraBooleanData(20, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 地图配置参数
        // 获取作者在作者之家配置的地图参数（原只读类型的地图全局存档）
        // 作者可以通过此接口实现节日活动开关、口令等功能
        public static string DzMap_GetMapConfig(string key)
        {
            return RequestExtraStringData(21, null, key, null, false, 0, 0, 0);
        }

        // 读取服务器装备数据
        public static int DzMap_GetServerArchiveEquip(int whichPlayer, string key)
        {
            return RequestExtraIntegerData(26, whichPlayer, key, null, false, 0, 0, 0);
        }

        // 读取服务器Boss掉落装备类型
        public static string DzMap_GetServerArchiveDrop(int whichPlayer, string key)
        {
            return RequestExtraStringData(27, whichPlayer, key, null, false, 0, 0, 0);
        }

        // 触发boss击杀
        public static void DzMap_OrpgTrigger(int whichPlayer, string key)
        {
            RequestExtraIntegerData(28, whichPlayer, key, null, false, 0, 0, 0);
        }

        // 获取玩家是否平台尊享会员
        public static int DzMap_GetPlatformVIP(int whichPlayer)
        {
            return RequestExtraIntegerData(30, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 玩家是否平台VIP
        public static bool DzMap_IsPlatformVIP(int whichPlayer)
        {
            return DzMap_GetPlatformVIP(whichPlayer) > 0;
        }

        // 服务器公共存档组保存
        // 存储服务器存档组，服务器存档组有100个KEY，每个KEY64个字符串长度，使用前请在作者之家服务器存档组进行设置
        public static bool DzMap_SavePublicArchive(int whichPlayer, string key, string value)
        {
            return RequestExtraBooleanData(31, whichPlayer, key, value, false, 0, 0, 0);
        }

        // 读取公共服务器存档组数据
        // 服务器存档组有100个KEY，每个KEY64个字符长度，可以多张地图读取和保存，使用前先在作者之家服务器存档组设置
        public static string DzMap_GetPublicArchive(int whichPlayer, string key)
        {
            return RequestExtraStringData(32, whichPlayer, key, null, false, 0, 0, 0);
        }

        // 使用地图商城道具（局数型）
        // 扣减玩家背包中的局数型道具1个，多次对同一个道具调用也只扣减1次。
        // 需先通过玩家地图商城道具剩余数量确保玩家背包中的道具剩余数量大于0。
        public static int DzMap_UseConsumablesItem(int whichPlayer, string key)
        {
            return RequestExtraIntegerData(33, whichPlayer, key, null, false, 0, 0, 0);
        }

        // 在游戏内的关键行为操作进行埋点，以便进行游戏内的玩家行为数据统计分析（比如某个英雄选择次数）
        // 上报前需先在作者之家创建埋点。
        public static bool DzMap_Statistics(int whichPlayer, string evtKey, string eventType, int value)
        {
            return RequestExtraBooleanData(34, whichPlayer, evtKey, eventType, false, value, 0, 0);
        }

        // 读取全局存档
        // 只读的公共存档请使用另一个API读取
        public static string DzMap_Global_GetStoreString(string key)
        {
            return RequestExtraStringData(36, JassApi.Common.GetLocalPlayer(), key, null, false, 0, 0, 0);
        }

        // 保存全局存档
        // 将变量数据存储到平台服务器上的全局存档中，
        // 保存时会受到开发者平台所配置的保存规则限制。
        // 保存成功后本局游戏及同一时间正在进行的其他游戏局内的所有玩家都会收到全局存档变化事件的事件广播。
        public static bool DzMap_Global_StoreString(string key, string value)
        {
            return RequestExtraBooleanData(37, JassApi.Common.GetLocalPlayer(), key, value, false, 0, 0, 0);
        }

        // 读取服务器存档（区分大小写）
        // 从服务器上读取变量数据，存档变量Key区分大小写
        // [错误代码]
        // 1190 存档初始化加载失败
        public static string DzMap_ServerArchive(int whichPlayer, string key)
        {
            return RequestExtraStringData(38, whichPlayer, key, null, false, 0, 0, 0);
        }

        // 存储服务器存档（区分大小写）
        // 将变量存储到服务器，存档变量Key区分大小写。
        // 数据保存时会受到开发者平台配置的写入规则限制（防刷分、只增等）
        // [错误代码]
        // 1750 地图未开通服务器存档功能
        // 1753 存档数据不一致
        // 1757 上传频率超限
        // 1758 超过每局最大值
        // 1191 存档变量Key长度超过64位
        // 1192 存档数量超过上限
        // 1250 增加的值超出上限（服务器存档防刷）
        public static bool DzMap_SaveServerArchive(int whichPlayer, string key, string value)
        {
            return RequestExtraBooleanData(39, whichPlayer, key, value, false, 0, 0, 0);
        }

        // 本局游戏是否快速匹配
        // 判断玩家是否是通过匹配模式进入游戏
        // 具体模式ID使用 获取天梯和匹配的模式 获取
        public static bool DzMap_IsRPGQuickMatch()
        {
            return RequestExtraBooleanData(40, null, null, null, false, 0, 0, 0);
        }

        // 玩家平台道具剩余数量
        // 获取玩家 key 指定道具的剩余数量
        // 仅对次数消耗型商品有效
        public static int DzMap_GetMallItemCount(int whichPlayer, string key)
        {
            return RequestExtraIntegerData(41, whichPlayer, key, null, false, 0, 0, 0);
        }

        // 使用地图商城道具（数量型）
        // 扣减玩家背包中的数量消耗型道具，可以多次调用
        // 使用玩家 key 商城道具 value 次
        // 仅对次数消耗型商品有效，只能使用不能恢复，请谨慎使用
        public static bool DzMap_ConsumeMallItem(int whichPlayer, string key, int value)
        {
            return RequestExtraBooleanData(42, whichPlayer, key, null, false, value, 0, 0);
        }

        // 修改平台功能设置
        // 地图可以根据自身特点，强制开启/关闭游戏内辅助功能
        public static bool DzMap_EnablePlatformSettings(int whichPlayer, int option, bool enable)
        {
            return RequestExtraBooleanData(43, whichPlayer, null, null, enable, option, 0, 0);
        }

        // 获取玩家中游戏局数
        public static int DzMap_PlayedGames(int whichPlayer)
        {
            return RequestExtraIntegerData(45, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 获取玩家的评论次数
        // 该功能已失效，始终返回1
        public static int DzMap_CommentCount(int whichPlayer)
        {
            return RequestExtraIntegerData(46, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 获取玩家平台好友数量
        public static int DzMap_FriendCount(int whichPlayer)
        {
            return RequestExtraIntegerData(47, whichPlayer, null, null, false, 0, 0, 0);
        }

        // [废弃]玩家是鉴赏家
        // 评论里的鉴赏家
        public static int DzMap_IsConnoisseur(int whichPlayer)
        {
            return RequestExtraIntegerData(47, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 玩家登录的是战网账号
        public static bool DzMap_IsBattleNetAccount(int whichPlayer)
        {
            return RequestExtraBooleanData(49, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 玩家是地图作者
        public static bool DzMap_IsAuthor(int whichPlayer)
        {
            return RequestExtraBooleanData(50, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 地图评论次数
        // 获取该图总评论次数
        public static int DzMap_CommentTotalCount()
        {
            return RequestExtraIntegerData(51, null, null, null, false, 0, 0, 0);
        }

        // [别名] DzAPI_Map_CommentTotalCount1
        // 获取自定义排行榜玩家排名
        // 100名以外的玩家排名为0
        // 该功能适用于作者之家-服务器存档-自定义排行榜
        public static int DzMap_CustomRanking(int whichPlayer, int id)
        {
            return RequestExtraIntegerData(52, whichPlayer, null, null, false, id, 0, 0);
        }

        // 玩家标记
        public static bool DzMap_PlayerFlags(int whichPlayer, int label)
        {
            return RequestExtraBooleanData(53, whichPlayer, null, null, false, label, 0, 0);
        }
        // [别名] DzAPI_Map_Returns
        // 玩家曾经是地图回流用户
        // 超过7天未玩地图的用户再次登录被称为地图回流用户，地图回流BUFF会存在7天，7天后消失。平台回流用户的BUFF存在15天，15天后消失。建议设置奖励，鼓励玩家回来玩地图！
        public static bool DzMap_IsMapReturnUsed(int whichPlayer)
        {
            return DzMap_PlayerFlags(whichPlayer, 1);
        }

        // 玩家当前是平台回流用户
        // 超过7天未玩地图的用户再次登录被称为地图回流用户，地图回流BUFF会存在7天，7天后消失。平台回流用户的BUFF存在15天，15天后消失。建议设置奖励，鼓励玩家回来玩地图！
        public static bool DzMap_IsPlatformReturn(int whichPlayer)
        {
            return DzMap_PlayerFlags(whichPlayer, 2);
        }

        // 玩家曾经是平台回流用户
        // 超过7天未玩地图的用户再次登录被称为地图回流用户，地图回流BUFF会存在7天，7天后消失。平台回流用户的BUFF存在15天，15天后消失。建议设置奖励，鼓励玩家回来玩地图！
        public static bool DzMap_IsPlatformReturnUsed(int whichPlayer)
        {
            return DzMap_PlayerFlags(whichPlayer, 4);
        }

        // 玩家当前是地图回流用户
        // 超过7天未玩地图的用户再次登录被称为地图回流用户，地图回流BUFF会存在7天，7天后消失。平台回流用户的BUFF存在15天，15天后消失。建议设置奖励，鼓励玩家回来玩地图！
        public static bool DzMap_IsMapReturn(int whichPlayer)
        {
            return DzMap_PlayerFlags(whichPlayer, 8);
        }

        // 玩家收藏过地图
        public static bool DzMap_IsCollected(int whichPlayer)
        {
            return DzMap_PlayerFlags(whichPlayer, 16);
        }

        // 获取玩家在指定地图的地图签到数据
        // 玩家每天登录游戏后，自动签到
        public static int DzMap_ContinuousCount(int whichPlayer, int id)
        {
            return RequestExtraIntegerData(54, whichPlayer, null, null, false, id, 0, 0);
        }

        // 玩家是真实玩家
        // 用于区别平台AI玩家。现在平台已经添加虚拟电脑玩家，不用再担心匹配没人问题了！如果你的地图有AI，试试在作者之家开启这个功能吧！
        public static bool DzMap_IsPlayer(int whichPlayer)
        {
            return RequestExtraBooleanData(55, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 所有地图的总游戏时长
        public static int DzMap_MapsTotalPlayed(int whichPlayer)
        {
            return RequestExtraIntegerData(56, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 指定地图的地图等级
        // 可以在作者之家看到指定地图的id
        public static int DzMap_MapsLevel(int whichPlayer, int mapId)
        {
            return RequestExtraIntegerData(57, whichPlayer, null, null, false, mapId, 0, 0);
        }

        // 指定地图的平台金币消耗
        // 可以在作者之家看到指定地图的id
        public static int DzMap_MapsConsumeGold(int whichPlayer, int mapId)
        {
            return RequestExtraIntegerData(58, whichPlayer, null, null, false, mapId, 0, 0);
        }

        // 指定地图的平台木头消耗
        // 可以在作者之家看到指定地图的id
        public static int DzMap_MapsConsumeLumber(int whichPlayer, int mapId)
        {
            return RequestExtraIntegerData(59, whichPlayer, null, null, false, mapId, 0, 0);
        }

        // [别名] DzAPI_Map_MapsConsumeLv1
        // 指定地图消费在（1~199）区间
        public static bool DzMap_MapsConsume_1_199(int whichPlayer, int mapId)
        {
            return RequestExtraBooleanData(60, whichPlayer, null, null, false, mapId, 0, 0);
        }

        // [别名] DzAPI_Map_MapsConsumeLv2
        // 指定地图消费在（200~499）区间
        public static bool DzMap_MapsConsume_200_499(int whichPlayer, int mapId)
        {
            return RequestExtraBooleanData(61, whichPlayer, null, null, false, mapId, 0, 0);
        }

        // [别名] DzAPI_Map_MapsConsumeLv3
        // 指定地图消费在（500~999）区间
        public static bool DzMap_MapsConsume_500_999(int whichPlayer, int mapId)
        {
            return RequestExtraBooleanData(62, whichPlayer, null, null, false, mapId, 0, 0);
        }

        // [别名] DzAPI_Map_MapsConsumeLv4
        // 指定地图消费在（1000+）以上
        public static bool DzMap_MapsConsume_1000(int whichPlayer, int mapId)
        {
            return RequestExtraBooleanData(63, whichPlayer, null, null, false, mapId, 0, 0);
        }

        // 玩家是否装备指定平台装饰、皮肤
        // 检查玩家是否装备着指定平台装饰（仅限平台和地图的合作装饰）。
        // 访问授权限制
        // 高级接口，仅限有出品平台和地图合作装饰的地图使用，平台道具ID请联系平台运营人员提供
        public static bool DzMap_IsPlayerUsingSkin(int whichPlayer, int skinType, int id)
        {
            return RequestExtraBooleanData(64, whichPlayer, null, null, false, skinType, id, 0);
        }

        // 获取论坛数据
        // 是否发过贴子,是否版主时，返回为1代表肯定
        public static int DzMap_GetForumData(int whichPlayer, int data)
        {
            return RequestExtraIntegerData(65, whichPlayer, null, null, false, data, 0, 0);
        }

        // 游戏中弹出商城道具购买界面
        // 可以在游戏里打开指定商城道具购买界面（包括下架商品）,商品购买之后，请配合实时购买触发功能使用
        public static int DzMap_OpenMall(int whichPlayer, string key)
        {
            return RequestExtraIntegerData(66, whichPlayer, key, null, false, 0, 0, 0);
        }

        // 玩家最近一次上安利墙时间
        // 获取玩家最近一次在当前地图的优质评论被推荐上安利墙的时间
        public static int DzMap_GetLastRecommendTime(int whichPlayer)
        {
            return RequestExtraIntegerData(67, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 获取玩家在当前地图的宝箱累计抽取次数
        public static int DzMap_GetLotteryUsedCount(int whichPlayer)
        {
            return RequestExtraIntegerData(68, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 上报本局游戏玩家数据
        // 上报本局游戏的玩家数据，比如战斗力、杀敌数等。
        // 以下数据项 key 由平台统一定义，请勿随意自行上传：
        // RankIndex: 乱斗模式排名
        // InnerGameMode: 地图模式名称
        // GameResult: 游戏结果（上报后立即结束游戏），1=胜利，0=失败
        // GameResultNoEnd: 游戏结果（上报后不会立即结束游戏），1=胜利，0=失败
        public static int DzMap_GameResult_CommitData(int whichPlayer, string key, string value)
        {
            return RequestExtraIntegerData(69, whichPlayer, key, value, false, 0, 0, 0);
        }

        // 玩家本局游戏距上一局游戏的时间差
        // 查询该玩家上次玩游戏时间至本次玩游戏时间的差值，可以利用此接口实现离线收益之类的功能。
        // 访问授权限制
        // 高级接口，需要授权后才允许使用
        public static int DzMap_GetSinceLastPlayedSeconds(int whichPlayer)
        {
            return RequestExtraIntegerData(70, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 打开U币快速购买窗口
        // 弹出提示框询问玩家是否使用U币直接购买指定道具，作者需已在商城上架对应商品（商品信息中的道具和数量与接口所请求的参数一致）。
        // 如果前一次购买的提示框未关闭的情况下再次调用此接口，后续请求无效。
        // 购买成功后可通过玩家获得平台道具事件实现在游戏内立即生效
        // 访问授权限制
        // 高级接口，需要授权后才允许使用。
        public static bool DzMap_CancelQuickBuy(int whichPlayer, string key, int count, int seconds)
        {
            return RequestExtraBooleanData(72, whichPlayer, key, null, false, count, seconds, 0);
        }

        // 关闭U币快速购买窗口
        // 关闭最后一次打开的U币快速购买窗口，结合打开U币快速购买窗口使用。
        // 适用于游戏场景切换后，之前的提示购买已不再适用的情况，比如游戏开始前1分钟可以更换英雄，提示玩家购买英雄更换道具，1分钟后关闭提示防止玩家误购买。
        // 访问授权限制
        // 高级接口，需要授权后才允许使用。
        public static bool DzMap_CancelQuickBuy(int whichPlayer)
        {
            return RequestExtraBooleanData(73, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 本局游戏是否处于平台自测服
        public static bool DzMap_IsMapTest()
        {
            return RequestExtraBooleanData(74, null, null, null, false, 0, 0, 0);
        }

        // 玩家地图商城道具是否读取成功
        // 判断本局游戏中玩家的商城道具是否正确加载
        public static bool DzMap_PlayerLoadedItems(int whichPlayer)
        {
            return RequestExtraBooleanData(77, whichPlayer, null, null, false, 0, 0, 0);
        }

        // 自定义排行榜上榜人数
        // 访问授权限制
        // 高级接口，需要授权后才允许使用。
        public static int DzMap_CustomRankCount(int id)
        {
            return RequestExtraIntegerData(78, null, null, null, false, id, 0, 0);
        }

        // 获取排行榜上指定排名的用户名称
        // 访问授权限制
        // 高级接口，需要授权后才允许使用。
        /// @CSharpLua.Template = "Jass.Japi["DzMap_CustomRankPlayerName"]"
        public static string DzMap_CustomRankPlayerName(int id, int ranking)
        {
            return RequestExtraStringData(79, null, null, null, false, id, ranking, 0);
        }

        // 自定义排行榜上的玩家数值
        // 获取指定自定义排行榜上指定名次的玩家数值（排行榜值）。
        // 访问授权限制
        // 高级接口，需要授权后才允许使用。
        /// @CSharpLua.Template = "Jass.Japi["DzMap_CustomRankValue"]"
        public static int DzMap_CustomRankValue(int id, int ranking)
        {
            return RequestExtraIntegerData(80, null, null, null, false, id, ranking, 0);
        }

        // 获取玩家在平台的完整昵称（基础昵称#编号）
        /// @CSharpLua.Template = "Jass.Japi["DzMap_GetPlayerUserName"]"
        public static string DzMap_GetPlayerUserName(int whichPlayer)
        {
            return RequestExtraStringData(81, whichPlayer, null, null, false, 0, 0, 0);
        }

    }
}
