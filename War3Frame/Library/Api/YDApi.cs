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
        public static JAbility EXGetUnitAbility(JUnit u, int abilcode)
        {
            return War3.CallNative<JAbility>(War3.GetNativeFunction("EXGetUnitAbility"), u, abilcode);
        }

        public static JAbility EXGetUnitAbilityByIndex(JUnit u, int index)
        {
            return War3.CallNative<JAbility>(War3.GetNativeFunction("EXGetUnitAbilityByIndex"), u, index);
        }

        public static int EXGetAbilityId(JAbility abil)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("EXGetAbilityId"), abil);
        }

        public static float EXGetAbilityState(JAbility abil, int state_type)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("EXGetAbilityState"), abil, state_type);
        }

        public static bool EXSetAbilityState(JAbility abil, int state_type, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetAbilityState"), abil, state_type, value);
        }

        public static float EXGetAbilityDataReal(JAbility abil, int level, int data_type)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("EXGetAbilityDataReal"), abil, level, data_type);
        }

        public static bool EXSetAbilityDataReal(JAbility abil, int level, int data_type, float value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetAbilityDataReal"), abil, level, data_type, value);
        }

        public static int EXGetAbilityDataInteger(JAbility abil, int level, int data_type)
        {
            return War3.CallNative<int>(War3.GetNativeFunction("EXGetAbilityDataInteger"), abil, level, data_type);
        }

        public static bool EXSetAbilityDataInteger(JAbility abil, int level, int data_type, int value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetAbilityDataInteger"), abil, level, data_type, value);
        }

        public static string EXGetAbilityDataString(JAbility abil, int level, int data_type)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("EXGetAbilityDataString"), abil, level, data_type);
        }

        public static bool EXSetAbilityDataString(JAbility abil, int level, int data_type, string value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetAbilityDataString"), abil, level, data_type, value);
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
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetAbilityAEmeDataA"), abil, unitid);
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
            return War3.CallNative<string>(War3.GetNativeFunction("EXGetItemDataString"), itemcode, data_type);
        }

        public static bool EXSetItemDataString(int itemcode, int data_type, string value)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetItemDataString"), itemcode, data_type, value);
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
            return War3.CallNative<int>(War3.GetNativeFunction("EXGetEventDamageData"), edd_type);
        }

        public static bool EXSetEventDamage(float amount)
        {
            return War3.CallNative<bool>(War3.GetNativeFunction("EXSetEventDamage"), amount);
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
            return War3.CallNative<float>(War3.GetNativeFunction("EXGetEffectX"), e);
        }

        // title = "Y轴坐标 [JAPI] [New!]"
        public static float EXGetEffectY(JEffect e)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("EXGetEffectY"), e);
        }

        // title = "高度 [JAPI] [New!]"
        public static float EXGetEffectZ(JEffect e)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("EXGetEffectZ"), e);
        }

        // title = "移动到坐标 [JAPI] [New!]"
        public static void EXSetEffectXY(JEffect e, float x, float y)
        {
            War3.CallNative(War3.GetNativeFunction("EXSetEffectXY"), e, x, y);
        }

        // title = "设置高度 [JAPI] [New!]"
        public static void EXSetEffectZ(JEffect e, float z)
        {
            War3.CallNative(War3.GetNativeFunction("EXSetEffectZ"), e, z);
        }

        // title = "大小 [JAPI] [New!]"
        public static float EXGetEffectSize(JEffect e)
        {
            return War3.CallNative<float>(War3.GetNativeFunction("EXGetEffectSize"), e);
        }

        // title = "设置大小 [JAPI] [New!]"
        public static void EXSetEffectSize(JEffect e, float size)
        {
            War3.CallNative(War3.GetNativeFunction("EXSetEffectSize"), e, size);
        }

        // title = "绕X轴旋转 [JAPI] [New!]"
        public static void EXEffectMatRotateX(JEffect e, float angle)
        {
            War3.CallNative(War3.GetNativeFunction("EXEffectMatRotateX"), e, angle);
        }

        // title = "绕Y轴旋转 [JAPI] [New!]"
        public static void EXEffectMatRotateY(JEffect e, float angle)
        {
            War3.CallNative(War3.GetNativeFunction("EXEffectMatRotateY"), e, angle);
        }

        // title = "绕Z轴旋转 [JAPI] [New!]"
        public static void EXEffectMatRotateZ(JEffect e, float angle)
        {
            War3.CallNative(War3.GetNativeFunction("EXEffectMatRotateZ"), e, angle);
        }

        // title = "缩放 [JAPI] [New!]"
        public static void EXEffectMatScale(JEffect e, float x, float y, float z)
        {
            War3.CallNative(War3.GetNativeFunction("EXEffectMatScale"), e, x, y, z);
        }

        // title = "重置变换 [JAPI] [New!]"
        public static void EXEffectMatReset(JEffect e)
        {
            War3.CallNative(War3.GetNativeFunction("EXEffectMatReset"), e);
        }

        // title = "设置动画速度 [JAPI] [New!]"
        public static void EXSetEffectSpeed(JEffect e, float speed)
        {
            War3.CallNative(War3.GetNativeFunction("EXSetEffectSpeed"), e, speed);
        }

        // title = "移动到点 [JAPI] [New!]"
        public static void YDWESetEffectLoc(JEffect e, JLocation loc)
        {
            EXSetEffectXY(e, JassApi.GetLocationX(loc), JassApi.GetLocationY(loc));
        }

        public static void EXDisplayChat(JPlayer p, int chat_recipient, string message)
        {
            War3.CallNative(War3.GetNativeFunction("EXDisplayChat"), p, chat_recipient, message);
        }

        // title = "模拟玩家聊天 [JAPI]"
        public static void YDWEDisplayChat(JPlayer p, int chat_recipient, string message)
        {
            EXDisplayChat(p, chat_recipient, message);
        }

        public static string EXExecuteScript(string script)
        {
            return War3.CallNative<string>(War3.GetNativeFunction("EXExecuteScript"), script);
        }

        // title = "设置单位面向角度 [JAPI] [New!]"
        public static void EXSetUnitFacing(JUnit u, float angle)
        {
            War3.CallNative(War3.GetNativeFunction("EXSetUnitFacing"), u, angle);
        }

        public static void EXPauseUnit(JUnit u, bool flag)
        {
            War3.CallNative(War3.GetNativeFunction("EXPauseUnit"), u, flag);
        }

        // title = "设置单位的碰撞类型 [JAPI] [New!]"
        public static void EXSetUnitCollisionType(bool enable, JUnit u, int t)
        {
            War3.CallNative(War3.GetNativeFunction("EXSetUnitCollisionType"), enable, u, t);
        }

        // title = "设置单位的移动类型 [JAPI] [New!]"
        public static void EXSetUnitMoveType(JUnit u, int t)
        {
            War3.CallNative(War3.GetNativeFunction("EXSetUnitMoveType"), u, t);
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
