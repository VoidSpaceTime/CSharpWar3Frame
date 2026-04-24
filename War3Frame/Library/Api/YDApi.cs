using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using War3Frame;
using War3Frame.Library.Api;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace War3Frame.Library.Api
{
    public static class YDApi
    {
        private static readonly IntPtr _eXGetUnitAbilityPtr = War3.GetNativeFunction("EXGetUnitAbility");
        private static readonly IntPtr _eXGetUnitAbilityByIndexPtr = War3.GetNativeFunction("EXGetUnitAbilityByIndex");
        private static readonly IntPtr _eXGetAbilityIdPtr = War3.GetNativeFunction("EXGetAbilityId");
        private static readonly IntPtr _eXGetAbilityStatePtr = War3.GetNativeFunction("EXGetAbilityState");
        private static readonly IntPtr _eXSetAbilityStatePtr = War3.GetNativeFunction("EXSetAbilityState");
        private static readonly IntPtr _eXGetAbilityDataRealPtr = War3.GetNativeFunction("EXGetAbilityDataReal");
        private static readonly IntPtr _eXSetAbilityDataRealPtr = War3.GetNativeFunction("EXSetAbilityDataReal");
        private static readonly IntPtr _eXGetAbilityDataIntegerPtr = War3.GetNativeFunction("EXGetAbilityDataInteger");
        private static readonly IntPtr _eXSetAbilityDataIntegerPtr = War3.GetNativeFunction("EXSetAbilityDataInteger");
        private static readonly IntPtr _eXGetAbilityDataStringPtr = War3.GetNativeFunction("EXGetAbilityDataString");
        private static readonly IntPtr _eXSetAbilityDataStringPtr = War3.GetNativeFunction("EXSetAbilityDataString");
        private static readonly IntPtr _eXSetAbilityAEmeDataAPtr = War3.GetNativeFunction("EXSetAbilityAEmeDataA");
        private static readonly IntPtr _eXGetItemDataStringPtr = War3.GetNativeFunction("EXGetItemDataString");
        private static readonly IntPtr _eXSetItemDataStringPtr = War3.GetNativeFunction("EXSetItemDataString");
        private static readonly IntPtr _eXGetEventDamageDataPtr = War3.GetNativeFunction("EXGetEventDamageData");
        private static readonly IntPtr _eXSetEventDamagePtr = War3.GetNativeFunction("EXSetEventDamage");
        private static readonly IntPtr _eXGetEffectXPtr = War3.GetNativeFunction("EXGetEffectX");
        private static readonly IntPtr _eXGetEffectYPtr = War3.GetNativeFunction("EXGetEffectY");
        private static readonly IntPtr _eXGetEffectZPtr = War3.GetNativeFunction("EXGetEffectZ");
        private static readonly IntPtr _eXSetEffectXYPtr = War3.GetNativeFunction("EXSetEffectXY");
        private static readonly IntPtr _eXSetEffectZPtr = War3.GetNativeFunction("EXSetEffectZ");
        private static readonly IntPtr _eXGetEffectSizePtr = War3.GetNativeFunction("EXGetEffectSize");
        private static readonly IntPtr _eXSetEffectSizePtr = War3.GetNativeFunction("EXSetEffectSize");
        private static readonly IntPtr _eXEffectMatRotateXPtr = War3.GetNativeFunction("EXEffectMatRotateX");
        private static readonly IntPtr _eXEffectMatRotateYPtr = War3.GetNativeFunction("EXEffectMatRotateY");
        private static readonly IntPtr _eXEffectMatRotateZPtr = War3.GetNativeFunction("EXEffectMatRotateZ");
        private static readonly IntPtr _eXEffectMatScalePtr = War3.GetNativeFunction("EXEffectMatScale");
        private static readonly IntPtr _eXEffectMatResetPtr = War3.GetNativeFunction("EXEffectMatReset");
        private static readonly IntPtr _eXSetEffectSpeedPtr = War3.GetNativeFunction("EXSetEffectSpeed");
        private static readonly IntPtr _eXDisplayChatPtr = War3.GetNativeFunction("EXDisplayChat");
        private static readonly IntPtr _eXExecuteScriptPtr = War3.GetNativeFunction("EXExecuteScript");
        private static readonly IntPtr _eXSetUnitFacingPtr = War3.GetNativeFunction("EXSetUnitFacing");
        private static readonly IntPtr _eXPauseUnitPtr = War3.GetNativeFunction("EXPauseUnit");
        private static readonly IntPtr _eXSetUnitCollisionTypePtr = War3.GetNativeFunction("EXSetUnitCollisionType");
        private static readonly IntPtr _eXSetUnitMoveTypePtr = War3.GetNativeFunction("EXSetUnitMoveType");

        public static JAbility EXGetUnitAbility(JUnit u, int abilcode)
        {
            var handle = War3.CallNative<int>(_eXGetUnitAbilityPtr, u, abilcode);
            return new JAbility(handle);
        }

        public static JAbility EXGetUnitAbilityByIndex(JUnit u, int index)
        {
            var handle = War3.CallNative<int>(_eXGetUnitAbilityByIndexPtr, u, index);
            return new JAbility(handle);
        }

        public static int EXGetAbilityId(JAbility abil)
        {
            return War3.CallNative<int>(_eXGetAbilityIdPtr, abil);
        }

        public static float EXGetAbilityState(JAbility abil, int state_type)
        {
            return War3.CallNative<float>(_eXGetAbilityStatePtr, abil, state_type);
        }

        public static bool EXSetAbilityState(JAbility abil, int state_type, float value)
        {
            return War3.CallNative<bool>(_eXSetAbilityStatePtr, abil, state_type, value);
        }

        public static float EXGetAbilityDataReal(JAbility abil, int level, int data_type)
        {
            return War3.CallNative<float>(_eXGetAbilityDataRealPtr, abil, level, data_type);
        }

        public static bool EXSetAbilityDataReal(JAbility abil, int level, int data_type, float value)
        {
            return War3.CallNative<bool>(_eXSetAbilityDataRealPtr, abil, level, data_type, value);
        }

        public static int EXGetAbilityDataInteger(JAbility abil, int level, int data_type)
        {
            return War3.CallNative<int>(_eXGetAbilityDataIntegerPtr, abil, level, data_type);
        }

        public static bool EXSetAbilityDataInteger(JAbility abil, int level, int data_type, int value)
        {
            return War3.CallNative<bool>(_eXSetAbilityDataIntegerPtr, abil, level, data_type, value);
        }

        public static string EXGetAbilityDataString(JAbility abil, int level, int data_type)
        {
            return War3.CallNative<string>(_eXGetAbilityDataStringPtr, abil, level, data_type);
        }

        public static bool EXSetAbilityDataString(JAbility abil, int level, int data_type, string value)
        {
            return War3.CallNative<bool>(_eXSetAbilityDataStringPtr, abil, level, data_type, value);
        }

        // title = "技能属性 [JAPI]"
        public static float YDWEGetUnitAbilityState(JUnit u, int abilcode, int state_type)
        {
            return EXGetAbilityState(EXGetUnitAbility(u, abilcode), state_type);
        }

        // title = "技能数据 (整数) [JAPI]"
        public static int YDWEGetUnitAbilityDataInteger(JUnit u, int abilcode, int level, int data_type)
        {
            return EXGetAbilityDataInteger(EXGetUnitAbility(u, abilcode), level, data_type);
        }

        // title = "技能数据 (实数) [JAPI]"
        public static float YDWEGetUnitAbilityDataReal(JUnit u, int abilcode, int level, int data_type)
        {
            return EXGetAbilityDataReal(EXGetUnitAbility(u, abilcode), level, data_type);
        }

        // title = "技能数据 (字符串) [JAPI]"
        public static string YDWEGetUnitAbilityDataString(JUnit u, int abilcode, int level, int data_type)
        {
            return EXGetAbilityDataString(EXGetUnitAbility(u, abilcode), level, data_type);
        }

        // title = "设置技能属性 [JAPI]"
        public static bool YDWESetUnitAbilityState(JUnit u, int abilcode, int state_type, float value)
        {
            return EXSetAbilityState(EXGetUnitAbility(u, abilcode), state_type, value);
        }

        // title = "设置技能数据 (整数) [JAPI]"
        public static bool YDWESetUnitAbilityDataInteger(JUnit u, int abilcode, int level, int data_type, int value)
        {
            return EXSetAbilityDataInteger(EXGetUnitAbility(u, abilcode), level, data_type, value);
        }

        // title = "设置技能数据 (实数) [JAPI]"
        public static bool YDWESetUnitAbilityDataReal(JUnit u, int abilcode, int level, int data_type, float value)
        {
            return EXSetAbilityDataReal(EXGetUnitAbility(u, abilcode), level, data_type, value);
        }

        // title = "设置技能数据 (字符串) [JAPI]"
        public static bool YDWESetUnitAbilityDataString(JUnit u, int abilcode, int level, int data_type, string value)
        {
            return EXSetAbilityDataString(EXGetUnitAbility(u, abilcode), level, data_type, value);
        }

        public static bool EXSetAbilityAEmeDataA(JAbility abil, int unitid)
        {
            return War3.CallNative<bool>(_eXSetAbilityAEmeDataAPtr, abil, unitid);
        }

        /*        // title = "单位变身 [JAPI]"
                public static void YDWEUnitTransform(JUnit u, int abilcode, int targetid)
                {
                    UnitAddAbility(u, abilcode);
                    EXSetAbilityDataInteger(EXGetUnitAbility(u, abilcode), 1, ABILITY_DATA_UNITID, GetUnitTypeId(u));
                    EXSetAbilityAEmeDataA(EXGetUnitAbility(u, abilcode), GetUnitTypeId(u));
                    UnitRemoveAbility(u, abilcode);
                    UnitAddAbility(u, abilcode);
                    EXSetAbilityAEmeDataA(EXGetUnitAbility(u, abilcode), targetid);
                    UnitRemoveAbility(u, abilcode);
                }*/

        public static string EXGetItemDataString(int itemcode, int data_type)
        {
            return War3.CallNative<string>(_eXGetItemDataStringPtr, itemcode, data_type);
        }

        public static bool EXSetItemDataString(int itemcode, int data_type, string value)
        {
            return War3.CallNative<bool>(_eXSetItemDataStringPtr, itemcode, data_type, value);
        }

        // title = "物品数据 (字符串) [JAPI]"
        public static string YDWEGetItemDataString(int itemcode, int data_type)
        {
            return EXGetItemDataString(itemcode, data_type);
        }

        // title = "设置物品数据 (字符串) [JAPI]"
        public static bool YDWESetItemDataString(int itemcode, int data_type, string value)
        {
            return EXSetItemDataString(itemcode, data_type, value);
        }

        public static int EXGetEventDamageData(int edd_type)
        {
            return War3.CallNative<int>(_eXGetEventDamageDataPtr, edd_type);
        }

        public static bool EXSetEventDamage(float amount)
        {
            return War3.CallNative<bool>(_eXSetEventDamagePtr, amount);
        }

        // title = "是物理伤害 [JAPI]"
        public static bool YDWEIsEventPhysicalDamage()
        {
            return 0 != EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_IS_PHYSICAL);
        }

        // title = "是攻击伤害 [JAPI]"
        public static bool YDWEIsEventAttackDamage()
        {
            return 0 != EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_IS_ATTACK);
        }

        // title = "是远程伤害 [JAPI]"
        public static bool YDWEIsEventRangedDamage()
        {
            return 0 != EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_IS_RANGED);
        }

        // title = "伤害类型 [JAPI]"
        public static bool YDWEIsEventDamageType(int JDamageType)
        {
            return JDamageType == EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_DAMAGE_TYPE);
        }

        // title = "武器类型 [JAPI]"
        public static bool YDWEIsEventWeaponType(int JWeaponType)
        {
            return JWeaponType == EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_WEAPON_TYPE);
        }

        // title = "攻击类型 [JAPI]"
        public static bool YDWEIsEventAttackType(int JAttackType)
        {
            return JAttackType == EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_ATTACK_TYPE);
        }

        // title = "设置伤害值 [JAPI]"
        public static bool YDWESetEventDamage(float amount)
        {
            return EXSetEventDamage(amount);
        }

        // title = "X轴坐标 [JAPI] [New!]"
        public static float EXGetEffectX(JEffect e)
        {
            return War3.CallNative<float>(_eXGetEffectXPtr, e);
        }

        // title = "Y轴坐标 [JAPI] [New!]"
        public static float EXGetEffectY(JEffect e)
        {
            return War3.CallNative<float>(_eXGetEffectYPtr, e);
        }

        // title = "高度 [JAPI] [New!]"
        public static float EXGetEffectZ(JEffect e)
        {
            return War3.CallNative<float>(_eXGetEffectZPtr, e);
        }

        // title = "移动到坐标 [JAPI] [New!]"
        public static void EXSetEffectXY(JEffect e, float x, float y)
        {
            War3.CallNative(_eXSetEffectXYPtr, e, x, y);
        }

        // title = "设置高度 [JAPI] [New!]"
        public static void EXSetEffectZ(JEffect e, float z)
        {
            War3.CallNative(_eXSetEffectZPtr, e, z);
        }

        // title = "大小 [JAPI] [New!]"
        public static float EXGetEffectSize(JEffect e)
        {
            return War3.CallNative<float>(_eXGetEffectSizePtr, e);
        }

        // title = "设置大小 [JAPI] [New!]"
        public static void EXSetEffectSize(JEffect e, float size)
        {
            War3.CallNative(_eXSetEffectSizePtr, e, size);
        }

        // title = "绕X轴旋转 [JAPI] [New!]"
        public static void EXEffectMatRotateX(JEffect e, float angle)
        {
            War3.CallNative(_eXEffectMatRotateXPtr, e, angle);
        }

        // title = "绕Y轴旋转 [JAPI] [New!]"
        public static void EXEffectMatRotateY(JEffect e, float angle)
        {
            War3.CallNative(_eXEffectMatRotateYPtr, e, angle);
        }

        // title = "绕Z轴旋转 [JAPI] [New!]"
        public static void EXEffectMatRotateZ(JEffect e, float angle)
        {
            War3.CallNative(_eXEffectMatRotateZPtr, e, angle);
        }

        // title = "缩放 [JAPI] [New!]"
        public static void EXEffectMatScale(JEffect e, float x, float y, float z)
        {
            War3.CallNative(_eXEffectMatScalePtr, e, x, y, z);
        }

        // title = "重置变换 [JAPI] [New!]"
        public static void EXEffectMatReset(JEffect e)
        {
            War3.CallNative(_eXEffectMatResetPtr, e);
        }

        // title = "设置动画速度 [JAPI] [New!]"
        public static void EXSetEffectSpeed(JEffect e, float speed)
        {
            War3.CallNative(_eXSetEffectSpeedPtr, e, speed);
        }

        // title = "移动到点 [JAPI] [New!]"
        public static void YDWESetEffectLoc(JEffect e, JLocation loc)
        {
            EXSetEffectXY(e, JassApi.GetLocationX(loc), JassApi.GetLocationY(loc));
        }

        public static void EXDisplayChat(JPlayer p, int chat_recipient, string message)
        {
            War3.CallNative(_eXDisplayChatPtr, p, chat_recipient, message);
        }

        // title = "模拟玩家聊天 [JAPI]"
        public static void YDWEDisplayChat(JPlayer p, int chat_recipient, string message)
        {
            EXDisplayChat(p, chat_recipient, message);
        }

        public static string EXExecuteScript(string script)
        {
            return War3.CallNative<string>(_eXExecuteScriptPtr, script);
        }

        // title = "设置单位面向角度 [JAPI] [New!]"
        public static void EXSetUnitFacing(JUnit u, float angle)
        {
            War3.CallNative(_eXSetUnitFacingPtr, u, angle);
        }

        public static void EXPauseUnit(JUnit u, bool flag)
        {
            War3.CallNative(_eXPauseUnitPtr, u, flag);
        }

        // title = "设置单位的碰撞类型 [JAPI] [New!]"
        public static void EXSetUnitCollisionType(bool enable, JUnit u, int t)
        {
            War3.CallNative(_eXSetUnitCollisionTypePtr, enable, u, t);
        }

        // title = "设置单位的移动类型 [JAPI] [New!]"
        public static void EXSetUnitMoveType(JUnit u, int t)
        {
            War3.CallNative(_eXSetUnitMoveTypePtr, u, t);
        }

        // title = "单位添加晕眩 [JAPI]"
        public static void YDWEUnitAddStun(JUnit u)
        {
            EXPauseUnit(u, true);
        }

        // title = "单位移除晕眩 [JAPI]"
        public static void YDWEUnitRemoveStun(JUnit u)
        {
            EXPauseUnit(u, false);
        }
    }
}
