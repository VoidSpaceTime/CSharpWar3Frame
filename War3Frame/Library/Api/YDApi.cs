using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace War3Frame.Library.Api
{
    public static class YDApi
    {
        // 执行脚本
        /// @CSharpLua.Template = "Jass.Japi["EXExecuteScript"]({0})"
        public static extern void EXExecuteScript(string script);
        // 重置特效变换
        // 清空所有的旋转和缩放，重置为初始状态
        /// @CSharpLua.Template = "Jass.Japi["EXEffectMatReset"]({0})"
        public static extern void EXEffectMatReset(int whichEffect);
        //  特效绕X轴旋转 angle 度
        // 多次调用，效果会叠加，不想叠加需要先重置为初始状态
        /// @CSharpLua.Template = "Jass.Japi["YD_EffectMatRotateX"]({0}, {1})"
        public static extern void YD_EffectMatRotateX(int whichEffect, float angle);
        // 特效绕Y轴旋转 angle 度
        // 多次调用，效果会叠加，不想叠加需要先重置为初始状态
        /// @CSharpLua.Template = "Jass.Japi["EXEffectMatRotateY"]({0}, {1})"
        public static extern void EXEffectMatRotateY(int whichEffect, float angle);
        // 设置特效的X轴缩放，Y轴缩放，Z轴缩放
        // 多次调用，效果会叠加，不想叠加需要先重置为初始状态。设置为2,2,2时相当于大小变为2倍。设置为负数时，就是镜像翻转
        /// @CSharpLua.Template = "Jass.Japi["EXEffectMatScale"]({0}, {1}, {2}, {3})"
        public static extern void EXEffectMatScale(int whichEffect, float x, float y, float z);
        // 获取特效大小
        /// @CSharpLua.Template = "Jass.Japi["EXGetEffectSize"]({0})"
        public static extern float EXGetEffectSize(int whichEffect);
        // 获取特效X轴坐标
        /// @CSharpLua.Template = "Jass.Japi["EXGetEffectX"]({0})"
        public static extern float EXGetEffectX(int whichEffect);
        // 获取特效Y轴坐标
        /// @CSharpLua.Template = "Jass.Japi["EXGetEffectY"]({0})"
        public static extern float EXGetEffectY(int whichEffect);
        // 获取特效Z轴坐标
        /// @CSharpLua.Template = "Jass.Japi["EXGetEffectZ"]({0})"
        public static extern float EXGetEffectZ(int whichEffect);
        // 设置特效大小
        /// @CSharpLua.Template = "Jass.Japi["EXSetEffectSize"]({0}, {1})"
        public static extern void EXSetEffectSize(int whichEffect, float size);
        // 设置特效速度
        /// @CSharpLua.Template = "Jass.Japi["EXSetEffectSpeed"]({0}, {1})"
        public static extern void EXSetEffectSpeed(int whichEffect, float speed);
        // 移动特效到坐标
        /// @CSharpLua.Template = "Jass.Japi["EXSetEffectXY"]({0}, {1}, {2})"
        public static extern void EXSetEffectXY(int whichEffect, float x, float y);
        // 设置特效高度
        /// @CSharpLua.Template = "Jass.Japi["EXSetEffectZ"]({0}, {1})"
        public static extern void EXSetEffectZ(int whichEffect, float z);

        // 获取技能整数数据
        /// @CSharpLua.Template = "Jass.Japi["EXGetAbilityDataInteger"]({0}, {1}, {2})"
        public static extern int EXGetAbilityDataInteger(int whichAbility, int level, int dataType);

        // 获取技能实数数据
        /// @CSharpLua.Template = "Jass.Japi["EXGetAbilityDataReal"]({0}, {1}, {2})"
        public static extern float EXGetAbilityDataReal(int whichAbility, int level, int dataType);

        // 获取技能字符串数据
        /// @CSharpLua.Template = "Jass.Japi["EXGetAbilityDataString"]({0}, {1}, {2})"
        public static extern string EXGetAbilityDataString(int whichAbility, int level, int dataType);

        // 获取技能ID
        /// @CSharpLua.Template = "Jass.Japi["EXGetAbilityId"]({0})"
        public static extern int EXGetAbilityId(int whichAbility);

        // 获取技能状态
        /// @CSharpLua.Template = "Jass.Japi["EXGetAbilityState"]({0}, {1})"
        public static extern float EXGetAbilityState(int whichAbility, int stateType);

        // 设置技能状态
        /// @CSharpLua.Template = "Jass.Japi["EXSetAbilityState"]({0}, {1}, {2})"
        public static extern bool EXSetAbilityState(int ability, int stateType, float value);

        // 设置技能变身数据A
        /// @CSharpLua.Template = "Jass.Japi["EXSetAbilityAEmeDataA"]({0}, {1})"
        public static extern bool EXSetAbilityAEmeDataA(int whichAbility, int unitId);

        // 设置技能整数数据
        /// @CSharpLua.Template = "Jass.Japi["EXSetAbilityDataInteger"]({0}, {1}, {2}, {3})"
        public static extern bool EXSetAbilityDataInteger(int whichAbility, int level, int dataType, int value);

        // 设置技能实数数据
        /// @CSharpLua.Template = "Jass.Japi["EXSetAbilityDataReal"]({0}, {1}, {2}, {3})"
        public static extern bool EXSetAbilityDataReal(int whichAbility, int level, int dataType, float value);

        // 设置技能字符串数据
        /// @CSharpLua.Template = "Jass.Japi["EXSetAbilityDataString"]({0}, {1}, {2}, {3})"
        public static extern bool EXSetAbilityDataString(int whichAbility, int level, int dataType, string value);

        // 获取单位技能
        /// @CSharpLua.Template = "Jass.Japi["EXGetUnitAbility"]({0}, {1})"
        public static extern int EXGetUnitAbility(int whichUnit, int abilityId);

        // 根据索引获取单位技能
        /// @CSharpLua.Template = "Jass.Japi["EXGetUnitAbilityByIndex"]({0}, {1})"
        public static extern int EXGetUnitAbilityByIndex(int whichUnit, int index);

        // 设置单位类型字符串组数据
        /// @CSharpLua.Template = "Jass.Japi["EXSetUnitArrayString"]({0}, {1}, {2}, {3})"
        public static extern bool EXSetUnitArrayString(int uid, int id, int n, string name);

        // 设置单位类型整数数据
        /// @CSharpLua.Template = "Jass.Japi["EXSetUnitInteger"]({0}, {1}, {2})"
        public static extern bool EXSetUnitInteger(int uid, int id, int n);

        // 暂停单位
        /// @CSharpLua.Template = "Jass.Japi["EXPauseUnit"]({0}, {1})"
        public static extern void EXPauseUnit(int whichUnit, bool enable);

        // 设置单位的碰撞类型
        /// @CSharpLua.Template = "Jass.Japi["EXSetUnitCollisionType"]({0}, {1}, {2})"
        public static extern bool EXSetUnitCollisionType(bool enable, int u, int t);

        // 设置单位面向角度
        /// @CSharpLua.Template = "Jass.Japi["EXSetUnitFacing"]({0}, {1})"
        public static extern void EXSetUnitFacing(int u, float angle);

        // 设置单位的移动类型
        /// @CSharpLua.Template = "Jass.Japi["EXSetUnitMoveType"]({0}, {1})"
        public static extern bool EXSetUnitMoveType(int u, int t);

        // 获取单位属性
        /// @CSharpLua.Template = "Jass.Japi["GetUnitState"]({0}, {1})"
        public static extern float GetUnitState(int whichUnit, int state);

        // 设置单位属性
        /// @CSharpLua.Template = "Jass.Japi["SetUnitState"]({0}, {1}, {2}, {3})"
        public static extern void SetUnitState(int whichUnit, int state, float value);

        // 单位变身（需要额外实现，Lua代码为复合逻辑，C#需用方法实现）
        public static void YD_UnitTransform(int whichUnit, int abilityId, int targetId)
        {
            int ab = EXGetUnitAbility(whichUnit, abilityId);
            JassApi.Common.UnitAddAbility(whichUnit, abilityId);
            EXSetAbilityDataInteger(ab, 1, 117, JassApi.Common.GetUnitTypeId(whichUnit));
            EXSetAbilityAEmeDataA(ab, JassApi.Common.GetUnitTypeId(whichUnit));
            JassApi.Common.UnitRemoveAbility(whichUnit, abilityId);
            JassApi.Common.UnitAddAbility(whichUnit, abilityId);
            EXSetAbilityAEmeDataA(ab, targetId);
            JassApi.Common.UnitRemoveAbility(whichUnit, abilityId);
        }

        // 获取buff字符串数据
        /// @CSharpLua.Template = "Jass.Japi["EXGetBuffDataString"]({0}, {1})"
        public static extern string EXGetBuffDataString(int buffCode, int dataType);

        // 设置buff字符串数据
        /// @CSharpLua.Template = "Jass.Japi["EXSetBuffDataString"]({0}, {1}, {2})"
        public static extern bool EXSetBuffDataString(int buffCode, int dataType, string value);

        // 设置物品字符串数据
        /// @CSharpLua.Template = "Jass.Japi["EXSetItemDataString"]({0}, {1}, {2})"
        public static extern bool EXSetItemDataString(int itemCode, int dataType, string value);

        // 获取物品字符串数据
        /// @CSharpLua.Template = "Jass.Japi["EXGetItemDataString"]({0}, {1})"
        public static extern string EXGetItemDataString(int itemCode, int dataType);

        // 设置伤害值
        /// @CSharpLua.Template = "Jass.Japi["EXSetEventDamage"]({0})"
        public static extern void EXSetEventDamage(float amount);

        // 伤害值
        /// @CSharpLua.Template = "Jass.Japi["GetEventDamage"]()"
        public static extern float GetEventDamage();

        // 获取伤害事件数据
        /// @CSharpLua.Template = "Jass.Japi["EXGetEventDamageData"]({0})"
        public static extern int EXGetEventDamageData(int eddType);

        // 是物理伤害
        public static bool YD_IsEventPhysicalDamage()
        {
            return EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_IS_PHYSICAL) != 0;
        }

        // 是攻击伤害
        public static bool YD_IsEventAttackDamage()
        {
            return EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_IS_ATTACK) != 0;
        }

        // 是远程伤害
        public static bool YD_IsEventRangedDamage()
        {
            return EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_IS_RANGED) != 0;
        }

        // 单位所受伤害的伤害类型是 damageType
        public static bool YD_IsEventDamageType(int damageType)
        {
            return damageType == JassApi.Common.ConvertDamageType(EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_DAMAGE_TYPE));
        }

        // 单位所受伤害的武器类型是 weaponType
        public static bool YD_IsEventWeaponType(int weaponType)
        {
            return weaponType == JassApi.Common.ConvertWeaponType(EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_WEAPON_TYPE));
        }

        // 单位所受伤害的攻击类型是 attackType
        public static bool YD_IsEventAttackType(int attackType)
        {
            return attackType == JassApi.Common.ConvertAttackType(EXGetEventDamageData(Blizzard.EVENT_DAMAGE_DATA_ATTACK_TYPE));
        }
    }
}
