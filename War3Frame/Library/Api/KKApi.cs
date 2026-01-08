namespace War3Frame.Library.Api
{
    // JAPI自kk库，包含kk平台引擎自带的japi方法，方法以 KK_ 开头

    public static class KKApi
    {
        // [别名] KKApiTriggerRegisterBackendLogicUpdata
        // 注册随机存档更新事件
        // 当玩家随机存档更新的时候触发该事件。用“当前变动的随机存档”来获取变动的随机存档key。
        public static void KK_TriggerRegisterBackendLogicUpdate(int trig)
        {
            DzApi.DzTriggerRegisterSyncData(trig, "DZBLU", true);
        }
        // 注册随机存档删除事件
        // 当玩家随机存档删除的时候触发该事件。用“当前变动的随机存档”来获取变动的随机存档key。
        public static void KK_TriggerRegisterBackendLogicDelete(int trig)
        {
            DzApi.DzTriggerRegisterSyncData(trig, "DZBLD", true);
        }
        // 获取变动的随机存档
        // 用在注册随机存档更新和删除事件之后
        public static string KK_GetSyncBackendLogic()
        {
            return DzApi.DzGetTriggerSyncData();
        }
        // 注册天梯投降事件
        // 当玩家在天梯投降时候触发该事件。用“获取投降的队伍id”来获取。
        public static void KK_TriggerRegisterLadderSurrender(int trig)
        {
            DzApi.DzTriggerRegisterSyncData(trig, "DZSR", true);
        }
        // 获取天梯投降的队伍ID
        // 用于天梯投降事件动作里
        public static int KK_GetLadderSurrenderTeamId()
        {
            var res = int.TryParse(DzApi.DzGetTriggerSyncData(), out int id);
            if (res) return id;
            return -1;
        }
        //设置单位整数物编数据
        public static void KK_SetUnitDataCacheInteger(int whichUnit, int field, int value)
        {
            DzApi.DzSetUnitDataCacheInteger(whichUnit, field, 0, value);
        }
        //设置单位物编数据(建筑升级)
        public static void KK_UnitUIAddUpgradesIds(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 94, id, v);
        }
        //设置单位物编数据(农民可建造建筑)
        public static void KK_UnitUIAddBuildsIds(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 100, id, v);
        }
        //设置单位物编数据(可研究的科技)
        public static void KK_UnitUIAddResearchesIds(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 112, id, v);
        }
        //设置单位物编数据(可训练的单位)
        public static void KK_UnitUIAddTrainsIds(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 106, id, v);
        }
        //设置单位物编数据(出售的单位)
        public static void KK_UnitUIAddSellsUnitIds(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 118, id, v);
        }
        //设置单位物编数据(出售的物品)
        public static void KK_UnitUIAddSellsItemIds(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 124, id, v);
        }
        //设置单位物编数据(制造的物品)
        public static void KK_UnitUIAddMakesItemIds(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 130, id, v);
        }
        // 设置单位物编数据(科技需求)[单位]
        public static void KK_UnitUIAddRequiresUnitCode(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 166, id, v);
        }
        // 设置单位物编数据(科技需求)[科技]
        public static void KK_UnitUIAddRequiresTechCode(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 166, id, v);
        }
        //  设置单位物编数据(科技需求值)
        public static void KK_UnitUIAddRequiresAmounts(int whichUnit, int id, int v)
        {
            DzApi.DzUnitUIAddLevelArrayInteger(whichUnit, 172, id, v);
        }
        // 获取服务器存档限制余额
        // 获取指定服务器存档变量的天/周上限余额，需要在开发者平台配置服务器存档防刷。
        // 访问授权限制
        // 高级接口，需要授权后才允许使用。
        public static int KK_GetServerValueLimitLeft(int whichPlayer, string key)
        {
            return DzApi.RequestExtraIntegerData(82, whichPlayer, key, null, false, 0, 0, 0);
        }
        // 随机只读存档-生成随机数
        // 通知服务器端产生一个随机数，并将随机数保存至指定的只读型存档变量Key中。
        // 生成随机数时需要关联一个组ID，该组ID可以在开发者平台进行防刷分管理，同组ID下各个Key共享CD和次数。
        public static bool KK_RequestBackendLogic(int whichPlayer, string key, string groupKey)
        {
            return DzApi.RequestExtraBooleanData(83, whichPlayer, key, groupKey, false, 0, 0, 0);
        }
        // 随机只读存档-判断随机数是否为空
        // 判断服务器端所生成的随机数是否为空。
        public static bool KK_CheckBackendLogicExists(int whichPlayer, string key)
        {
            return DzApi.RequestExtraBooleanData(84, whichPlayer, key, null, false, 0, 0, 0);
        }
        // 随机只读存档-读取随机数的值
        // 读取服务器端所产生的随机数的值。
        public static bool KK_GetBackendLogicIntResult(int whichPlayer, string key)
        {
            return DzApi.RequestExtraBooleanData(85, whichPlayer, key, null, false, 0, 0, 0);
        }
        // 获取后端逻辑生成结果 字符串
        public static string KK_GetBackendLogicStrResult(int whichPlayer, string key)
        {
            return DzApi.RequestExtraStringData(86, whichPlayer, key, null, false, 0, 0, 0);
        }
        // 随机只读存档-读取随机数的生成时间
        // 读取服务器端所产生随机数的生成时间。
        public static int KK_GetBackendLogicUpdateTime(int whichPlayer, string key)
        {
            return DzApi.RequestExtraIntegerData(87, whichPlayer, key, null, false, 0, 0, 0);
        }
        // 随机只读存档-读取随机数的组ID
        // 读取指定的随机只读存档变量Key最后一次是由哪个组ID所生成的。
        public static string KK_GetBackendLogicGroup(int whichPlayer, string key)
        {
            return DzApi.RequestExtraStringData(88, whichPlayer, key, null, false, 0, 0, 0);
        }
        // 随机只读存档-删除随机数
        // 删除指定的随机只读存档变量Key中所保存的随机数
        public static bool KK_RemoveBackendLogicResult(int whichPlayer, string key)
        {
            return DzApi.RequestExtraBooleanData(89, whichPlayer, key, null, false, 0, 0, 0);
        }
        // 是否在平台正常游戏中
        // 主要试用于平台运行中区分正常游戏和观战模式，返回true代表是正常游戏模式，反之为观战模式
        public static bool KK_IsGameMode()
        {
            return DzApi.RequestExtraBooleanData(90, null, null, null, false, 0, 0, 0);
        }
        // 初始化平台键位显示设置
        // 设置whichPlayer的第n套方案的键位key,设置描述data
        // 初始化键位设置会显示在平台改键界面上，最多2套方案
        public static bool KK_InitializeGameKey(int whichPlayer, int setIndex, string k, string data)
        {
            var str = $"[{{\"name\":\"{data}\",\"key\":\"{k}\"]}}";
            return DzApi.RequestExtraBooleanData(91, whichPlayer, str, null, false, setIndex, 0, 0);
        }
        // 获取当前玩家在平台的身份类型（主播/职业选手）
        public static bool KK_IsPlayerIdentityType(int whichPlayer, int id)
        {
            return DzApi.RequestExtraBooleanData(92, whichPlayer, null, null, false, id, 0, 0);
        }
        // 判断是否是蓝V
        public static bool KK_Map_IsBlueVIP(int whichPlayer)
        {
            return KK_IsPlayerIdentityType(whichPlayer, 3);
        }
        // 判断是否是红V
        public static bool KK_Map_IsRedVIP(int whichPlayer)
        {
            return KK_IsPlayerIdentityType(whichPlayer, 4);
        }
        // 获取玩家的平台ID
        // 返回的是一个32位的字符串
        public static string KK_PlayerGUID(int whichPlayer)
        {
            return DzApi.RequestExtraStringData(93, whichPlayer, null, null, false, 0, 0, 0);
        }
        // 玩家地图任务状态
        // 获取玩家地图任务状态，需要先在作者之家创建地图任务并生成任务ID。
        public static bool KK_IsTaskInProgress(int whichPlayer, int setIndex, int taskStatus)
        {
            return DzApi.RequestExtraIntegerData(94, whichPlayer, null, null, false, setIndex, taskStatus, 0) == taskStatus;
        }
        // 玩家地图任务当前进度
        // 获取玩家地图任务状态，需要先在作者之家创建地图任务并生成任务ID。
        public static int KK_QueryTaskCurrentProgress(int whichPlayer, int setIndex)
        {
            return DzApi.RequestExtraIntegerData(95, whichPlayer, null, null, false, setIndex, 0, 0);
        }
        // 玩家地图任务总进度
        // 获取玩家地图任务状态，需要先在作者之家创建地图任务并生成任务ID。
        public static int KK_QueryTaskTotalProgress(int whichPlayer, int setIndex)
        {
            return DzApi.RequestExtraIntegerData(96, whichPlayer, null, null, false, setIndex, 0, 0);
        }
        // 获取玩家成就是否完成
        public static bool KK_IsAchievementCompleted(int whichPlayer, string id)
        {
            return DzApi.RequestExtraBooleanData(98, whichPlayer, id, null, false, 0, 0, 0);
        }
        //获取玩家地图成就点数
        public static int KK_AchievementPoints(int whichPlayer)
        {
            return DzApi.RequestExtraIntegerData(99, whichPlayer, null, null, false, 0, 0, 0);
        }
        // 判断游戏时长是否满足条件
        public static bool KK_PlayedTime(int whichPlayer, int minHours, int maxHours)
        {
            return DzApi.RequestExtraBooleanData(100, whichPlayer, null, null, false, minHours, maxHours, 0);
        }
        // 获取随机存档剩余次数
        // 今日的剩余次数
        public static int KK_RandomSaveGameCount(int whichPlayer, string groupKey)
        {
            return DzApi.RequestExtraIntegerData(101, whichPlayer, groupKey, null, false, 0, 0, 0);
        }
        // 开始批量保存存档
        // 对添加批量保存存档条目进行保存
        public static bool KK_BeginBatchSaveArchive(int whichPlayer)
        {
            return DzApi.RequestExtraBooleanData(102, whichPlayer, null, null, false, 0, 0, 0);
        }
        //添加批量保存存档条目
        public static bool KK_AddBatchSaveArchive(int whichPlayer, string key, string value, bool caseInsensitive)
        {
            return DzApi.RequestExtraBooleanData(103, whichPlayer, key, value, caseInsensitive, 0, 0, 0);
        }
        //结束批量保存存档
        public static bool KK_EndBatchSaveArchive(int whichPlayer, bool abandon)
        {
            return DzApi.RequestExtraBooleanData(104, whichPlayer, null, null, abandon, 0, 0, 0);
        }
        //获取玩家所在公会等级
        public static int KK_GetGuildLevel(int whichPlayer)
        {
            return DzApi.RequestExtraIntegerData(106, whichPlayer, null, null, false, 0, 0, 0);
        }
        // 获取宠物探险次数
        public static int KK_MapExplorationNum(int whichPlayer)
        {
            return DzApi.RequestExtraIntegerData(107, whichPlayer, null, null, false, 0, 0, 0);
        }
        // 获取宠物探险时间
        public static int KK_MapExplorationTime(int whichPlayer)
        {
            return DzApi.RequestExtraIntegerData(108, whichPlayer, null, null, false, 0, 0, 0);
        }
        // 测试大厅预约人数
        public static int KK_MapOrderNum()
        {
            return DzApi.RequestExtraIntegerData(109, null, null, null, false, 0, 0, 0);
        }
        // 获取商城道具最后变动的数量（新增/删除）
        // 事件响应 - 商城道具最后变动的数量
        // 获取的是当次玩家商城背包新增或消耗的数量，如果是时效型道具获取的是剩余时间
        // 可以用于【玩家获取商城道具事件】、【玩家消耗使用商城道具事件】和【玩家删除商城道具事件】后
        public static int KK_GetMallItemUpdateCount(int whichPlayer, string key)
        {
            return DzApi.RequestExtraIntegerData(110, whichPlayer, key, null, false, 0, 0, 0);
        }
        //发送云脚本数据
        public static bool KK_MlScriptEvent(int whichPlayer, string eventName, string payload)
        {
            return DzApi.RequestExtraBooleanData(1009, whichPlayer, eventName, payload, false, 0, 0, 0);
        }
    }
}
