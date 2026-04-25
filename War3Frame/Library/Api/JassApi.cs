namespace War3Frame;

public static partial class JassApi
{
    private static readonly IntPtr _convertRacePtr = War3.GetNativeFunction("ConvertRace");
    private static readonly IntPtr _convertAllianceTypePtr = War3.GetNativeFunction("ConvertAllianceType");
    private static readonly IntPtr _convertRacePrefPtr = War3.GetNativeFunction("ConvertRacePref");
    private static readonly IntPtr _convertIGameStatePtr = War3.GetNativeFunction("ConvertIGameState");
    private static readonly IntPtr _convertFGameStatePtr = War3.GetNativeFunction("ConvertFGameState");
    private static readonly IntPtr _convertPlayerStatePtr = War3.GetNativeFunction("ConvertPlayerState");
    private static readonly IntPtr _convertPlayerScorePtr = War3.GetNativeFunction("ConvertPlayerScore");
    private static readonly IntPtr _convertPlayerGameResultPtr = War3.GetNativeFunction("ConvertPlayerGameResult");
    private static readonly IntPtr _convertUnitStatePtr = War3.GetNativeFunction("ConvertUnitState");
    private static readonly IntPtr _convertAIDifficultyPtr = War3.GetNativeFunction("ConvertAIDifficulty");
    private static readonly IntPtr _convertGameEventPtr = War3.GetNativeFunction("ConvertGameEvent");
    private static readonly IntPtr _convertPlayerEventPtr = War3.GetNativeFunction("ConvertPlayerEvent");
    private static readonly IntPtr _convertPlayerUnitEventPtr = War3.GetNativeFunction("ConvertPlayerUnitEvent");
    private static readonly IntPtr _convertWidgetEventPtr = War3.GetNativeFunction("ConvertWidgetEvent");
    private static readonly IntPtr _convertDialogEventPtr = War3.GetNativeFunction("ConvertDialogEvent");
    private static readonly IntPtr _convertUnitEventPtr = War3.GetNativeFunction("ConvertUnitEvent");
    private static readonly IntPtr _convertLimitOpPtr = War3.GetNativeFunction("ConvertLimitOp");
    private static readonly IntPtr _convertUnitTypePtr = War3.GetNativeFunction("ConvertUnitType");
    private static readonly IntPtr _convertGameSpeedPtr = War3.GetNativeFunction("ConvertGameSpeed");
    private static readonly IntPtr _convertPlacementPtr = War3.GetNativeFunction("ConvertPlacement");
    private static readonly IntPtr _convertStartLocPrioPtr = War3.GetNativeFunction("ConvertStartLocPrio");
    private static readonly IntPtr _convertGameDifficultyPtr = War3.GetNativeFunction("ConvertGameDifficulty");
    private static readonly IntPtr _convertGameTypePtr = War3.GetNativeFunction("ConvertGameType");
    private static readonly IntPtr _convertMapFlagPtr = War3.GetNativeFunction("ConvertMapFlag");
    private static readonly IntPtr _convertMapVisibilityPtr = War3.GetNativeFunction("ConvertMapVisibility");
    private static readonly IntPtr _convertMapSettingPtr = War3.GetNativeFunction("ConvertMapSetting");
    private static readonly IntPtr _convertMapDensityPtr = War3.GetNativeFunction("ConvertMapDensity");
    private static readonly IntPtr _convertMapControlPtr = War3.GetNativeFunction("ConvertMapControl");
    private static readonly IntPtr _convertPlayerColorPtr = War3.GetNativeFunction("ConvertPlayerColor");
    private static readonly IntPtr _convertPlayerSlotStatePtr = War3.GetNativeFunction("ConvertPlayerSlotState");
    private static readonly IntPtr _convertVolumeGroupPtr = War3.GetNativeFunction("ConvertVolumeGroup");
    private static readonly IntPtr _convertCameraFieldPtr = War3.GetNativeFunction("ConvertCameraField");
    private static readonly IntPtr _convertBlendModePtr = War3.GetNativeFunction("ConvertBlendMode");
    private static readonly IntPtr _convertRarityControlPtr = War3.GetNativeFunction("ConvertRarityControl");
    private static readonly IntPtr _convertTexMapFlagsPtr = War3.GetNativeFunction("ConvertTexMapFlags");
    private static readonly IntPtr _convertFogStatePtr = War3.GetNativeFunction("ConvertFogState");
    private static readonly IntPtr _convertEffectTypePtr = War3.GetNativeFunction("ConvertEffectType");
    private static readonly IntPtr _convertVersionPtr = War3.GetNativeFunction("ConvertVersion");
    private static readonly IntPtr _convertItemTypePtr = War3.GetNativeFunction("ConvertItemType");
    private static readonly IntPtr _convertAttackTypePtr = War3.GetNativeFunction("ConvertAttackType");
    private static readonly IntPtr _convertDamageTypePtr = War3.GetNativeFunction("ConvertDamageType");
    private static readonly IntPtr _convertWeaponTypePtr = War3.GetNativeFunction("ConvertWeaponType");
    private static readonly IntPtr _convertSoundTypePtr = War3.GetNativeFunction("ConvertSoundType");
    private static readonly IntPtr _convertPathingTypePtr = War3.GetNativeFunction("ConvertPathingType");
    private static readonly IntPtr _orderIdPtr = War3.GetNativeFunction("OrderId");
    private static readonly IntPtr _orderId2StringPtr = War3.GetNativeFunction("OrderId2String");
    private static readonly IntPtr _unitIdPtr = War3.GetNativeFunction("UnitId");
    private static readonly IntPtr _unitId2StringPtr = War3.GetNativeFunction("UnitId2String");
    private static readonly IntPtr _abilityIdPtr = War3.GetNativeFunction("AbilityId");
    private static readonly IntPtr _abilityId2StringPtr = War3.GetNativeFunction("AbilityId2String");
    private static readonly IntPtr _getObjectNamePtr = War3.GetNativeFunction("GetObjectName");
    private static readonly IntPtr _deg2RadPtr = War3.GetNativeFunction("Deg2Rad");
    private static readonly IntPtr _rad2DegPtr = War3.GetNativeFunction("Rad2Deg");
    private static readonly IntPtr _sinPtr = War3.GetNativeFunction("Sin");
    private static readonly IntPtr _cosPtr = War3.GetNativeFunction("Cos");
    private static readonly IntPtr _tanPtr = War3.GetNativeFunction("Tan");
    private static readonly IntPtr _asinPtr = War3.GetNativeFunction("Asin");
    private static readonly IntPtr _acosPtr = War3.GetNativeFunction("Acos");
    private static readonly IntPtr _atanPtr = War3.GetNativeFunction("Atan");
    private static readonly IntPtr _atan2Ptr = War3.GetNativeFunction("Atan2");
    private static readonly IntPtr _squareRootPtr = War3.GetNativeFunction("SquareRoot");
    private static readonly IntPtr _powPtr = War3.GetNativeFunction("Pow");
    private static readonly IntPtr _i2RPtr = War3.GetNativeFunction("I2R");
    private static readonly IntPtr _r2IPtr = War3.GetNativeFunction("R2I");
    private static readonly IntPtr _i2SPtr = War3.GetNativeFunction("I2S");
    private static readonly IntPtr _r2SPtr = War3.GetNativeFunction("R2S");
    private static readonly IntPtr _r2SWPtr = War3.GetNativeFunction("R2SW");
    private static readonly IntPtr _s2IPtr = War3.GetNativeFunction("S2I");
    private static readonly IntPtr _s2RPtr = War3.GetNativeFunction("S2R");
    private static readonly IntPtr _getHandleIdPtr = War3.GetNativeFunction("GetHandleId");
    private static readonly IntPtr _subStringPtr = War3.GetNativeFunction("SubString");
    private static readonly IntPtr _stringLengthPtr = War3.GetNativeFunction("StringLength");
    private static readonly IntPtr _stringCasePtr = War3.GetNativeFunction("StringCase");
    private static readonly IntPtr _stringHashPtr = War3.GetNativeFunction("StringHash");
    private static readonly IntPtr _getLocalizedStringPtr = War3.GetNativeFunction("GetLocalizedString");
    private static readonly IntPtr _getLocalizedHotkeyPtr = War3.GetNativeFunction("GetLocalizedHotkey");
    private static readonly IntPtr _setMapNamePtr = War3.GetNativeFunction("SetMapName");
    private static readonly IntPtr _setMapDescriptionPtr = War3.GetNativeFunction("SetMapDescription");
    private static readonly IntPtr _setTeamsPtr = War3.GetNativeFunction("SetTeams");
    private static readonly IntPtr _setPlayersPtr = War3.GetNativeFunction("SetPlayers");
    private static readonly IntPtr _defineStartLocationPtr = War3.GetNativeFunction("DefineStartLocation");
    private static readonly IntPtr _defineStartLocationLocPtr = War3.GetNativeFunction("DefineStartLocationLoc");
    private static readonly IntPtr _setStartLocPrioCountPtr = War3.GetNativeFunction("SetStartLocPrioCount");
    private static readonly IntPtr _setStartLocPrioPtr = War3.GetNativeFunction("SetStartLocPrio");
    private static readonly IntPtr _getStartLocPrioSlotPtr = War3.GetNativeFunction("GetStartLocPrioSlot");
    private static readonly IntPtr _getStartLocPrioPtr = War3.GetNativeFunction("GetStartLocPrio");
    private static readonly IntPtr _setGameTypeSupportedPtr = War3.GetNativeFunction("SetGameTypeSupported");
    private static readonly IntPtr _setMapFlagPtr = War3.GetNativeFunction("SetMapFlag");
    private static readonly IntPtr _setGamePlacementPtr = War3.GetNativeFunction("SetGamePlacement");
    private static readonly IntPtr _setGameSpeedPtr = War3.GetNativeFunction("SetGameSpeed");
    private static readonly IntPtr _setGameDifficultyPtr = War3.GetNativeFunction("SetGameDifficulty");
    private static readonly IntPtr _setResourceDensityPtr = War3.GetNativeFunction("SetResourceDensity");
    private static readonly IntPtr _setCreatureDensityPtr = War3.GetNativeFunction("SetCreatureDensity");
    private static readonly IntPtr _getTeamsPtr = War3.GetNativeFunction("GetTeams");
    private static readonly IntPtr _getPlayersPtr = War3.GetNativeFunction("GetPlayers");
    private static readonly IntPtr _isGameTypeSupportedPtr = War3.GetNativeFunction("IsGameTypeSupported");
    private static readonly IntPtr _getGameTypeSelectedPtr = War3.GetNativeFunction("GetGameTypeSelected");
    private static readonly IntPtr _isMapFlagSetPtr = War3.GetNativeFunction("IsMapFlagSet");
    private static readonly IntPtr _getGamePlacementPtr = War3.GetNativeFunction("GetGamePlacement");
    private static readonly IntPtr _getGameSpeedPtr = War3.GetNativeFunction("GetGameSpeed");
    private static readonly IntPtr _getGameDifficultyPtr = War3.GetNativeFunction("GetGameDifficulty");
    private static readonly IntPtr _getResourceDensityPtr = War3.GetNativeFunction("GetResourceDensity");
    private static readonly IntPtr _getCreatureDensityPtr = War3.GetNativeFunction("GetCreatureDensity");
    private static readonly IntPtr _getStartLocationXPtr = War3.GetNativeFunction("GetStartLocationX");
    private static readonly IntPtr _getStartLocationYPtr = War3.GetNativeFunction("GetStartLocationY");
    private static readonly IntPtr _getStartLocationLocPtr = War3.GetNativeFunction("GetStartLocationLoc");
    private static readonly IntPtr _setPlayerTeamPtr = War3.GetNativeFunction("SetPlayerTeam");
    private static readonly IntPtr _setPlayerStartLocationPtr = War3.GetNativeFunction("SetPlayerStartLocation");
    private static readonly IntPtr _forcePlayerStartLocationPtr = War3.GetNativeFunction("ForcePlayerStartLocation");
    private static readonly IntPtr _setPlayerColorPtr = War3.GetNativeFunction("SetPlayerColor");
    private static readonly IntPtr _setPlayerAlliancePtr = War3.GetNativeFunction("SetPlayerAlliance");
    private static readonly IntPtr _setPlayerTaxRatePtr = War3.GetNativeFunction("SetPlayerTaxRate");
    private static readonly IntPtr _setPlayerRacePreferencePtr = War3.GetNativeFunction("SetPlayerRacePreference");
    private static readonly IntPtr _setPlayerRaceSelectablePtr = War3.GetNativeFunction("SetPlayerRaceSelectable");
    private static readonly IntPtr _setPlayerControllerPtr = War3.GetNativeFunction("SetPlayerController");
    private static readonly IntPtr _setPlayerNamePtr = War3.GetNativeFunction("SetPlayerName");
    private static readonly IntPtr _setPlayerOnScoreScreenPtr = War3.GetNativeFunction("SetPlayerOnScoreScreen");
    private static readonly IntPtr _getPlayerTeamPtr = War3.GetNativeFunction("GetPlayerTeam");
    private static readonly IntPtr _getPlayerStartLocationPtr = War3.GetNativeFunction("GetPlayerStartLocation");
    private static readonly IntPtr _getPlayerColorPtr = War3.GetNativeFunction("GetPlayerColor");
    private static readonly IntPtr _getPlayerSelectablePtr = War3.GetNativeFunction("GetPlayerSelectable");
    private static readonly IntPtr _getPlayerControllerPtr = War3.GetNativeFunction("GetPlayerController");
    private static readonly IntPtr _getPlayerSlotStatePtr = War3.GetNativeFunction("GetPlayerSlotState");
    private static readonly IntPtr _getPlayerTaxRatePtr = War3.GetNativeFunction("GetPlayerTaxRate");
    private static readonly IntPtr _isPlayerRacePrefSetPtr = War3.GetNativeFunction("IsPlayerRacePrefSet");
    private static readonly IntPtr _getPlayerNamePtr = War3.GetNativeFunction("GetPlayerName");
    private static readonly IntPtr _createTimerPtr = War3.GetNativeFunction("CreateTimer");
    private static readonly IntPtr _destroyTimerPtr = War3.GetNativeFunction("DestroyTimer");
    private static readonly IntPtr _timerStartPtr = War3.GetNativeFunction("TimerStart");
    private static readonly IntPtr _timerGetElapsedPtr = War3.GetNativeFunction("TimerGetElapsed");
    private static readonly IntPtr _timerGetRemainingPtr = War3.GetNativeFunction("TimerGetRemaining");
    private static readonly IntPtr _timerGetTimeoutPtr = War3.GetNativeFunction("TimerGetTimeout");
    private static readonly IntPtr _pauseTimerPtr = War3.GetNativeFunction("PauseTimer");
    private static readonly IntPtr _resumeTimerPtr = War3.GetNativeFunction("ResumeTimer");
    private static readonly IntPtr _getExpiredTimerPtr = War3.GetNativeFunction("GetExpiredTimer");
    private static readonly IntPtr _createGroupPtr = War3.GetNativeFunction("CreateGroup");
    private static readonly IntPtr _destroyGroupPtr = War3.GetNativeFunction("DestroyGroup");
    private static readonly IntPtr _groupAddUnitPtr = War3.GetNativeFunction("GroupAddUnit");
    private static readonly IntPtr _groupRemoveUnitPtr = War3.GetNativeFunction("GroupRemoveUnit");
    private static readonly IntPtr _groupClearPtr = War3.GetNativeFunction("GroupClear");
    private static readonly IntPtr _groupEnumUnitsOfTypePtr = War3.GetNativeFunction("GroupEnumUnitsOfType");
    private static readonly IntPtr _groupEnumUnitsOfPlayerPtr = War3.GetNativeFunction("GroupEnumUnitsOfPlayer");

    private static readonly IntPtr _groupEnumUnitsOfTypeCountedPtr =
        War3.GetNativeFunction("GroupEnumUnitsOfTypeCounted");

    private static readonly IntPtr _groupEnumUnitsInRectPtr = War3.GetNativeFunction("GroupEnumUnitsInRect");

    private static readonly IntPtr _groupEnumUnitsInRectCountedPtr =
        War3.GetNativeFunction("GroupEnumUnitsInRectCounted");

    private static readonly IntPtr _groupEnumUnitsInRangePtr = War3.GetNativeFunction("GroupEnumUnitsInRange");

    private static readonly IntPtr _groupEnumUnitsInRangeOfLocPtr =
        War3.GetNativeFunction("GroupEnumUnitsInRangeOfLoc");

    private static readonly IntPtr _groupEnumUnitsInRangeCountedPtr =
        War3.GetNativeFunction("GroupEnumUnitsInRangeCounted");

    private static readonly IntPtr _groupEnumUnitsInRangeOfLocCountedPtr =
        War3.GetNativeFunction("GroupEnumUnitsInRangeOfLocCounted");

    private static readonly IntPtr _groupEnumUnitsSelectedPtr = War3.GetNativeFunction("GroupEnumUnitsSelected");
    private static readonly IntPtr _groupImmediateOrderPtr = War3.GetNativeFunction("GroupImmediateOrder");
    private static readonly IntPtr _groupImmediateOrderByIdPtr = War3.GetNativeFunction("GroupImmediateOrderById");
    private static readonly IntPtr _groupPointOrderPtr = War3.GetNativeFunction("GroupPointOrder");
    private static readonly IntPtr _groupPointOrderLocPtr = War3.GetNativeFunction("GroupPointOrderLoc");
    private static readonly IntPtr _groupPointOrderByIdPtr = War3.GetNativeFunction("GroupPointOrderById");
    private static readonly IntPtr _groupPointOrderByIdLocPtr = War3.GetNativeFunction("GroupPointOrderByIdLoc");
    private static readonly IntPtr _groupTargetOrderPtr = War3.GetNativeFunction("GroupTargetOrder");
    private static readonly IntPtr _groupTargetOrderByIdPtr = War3.GetNativeFunction("GroupTargetOrderById");
    private static readonly IntPtr _forGroupPtr = War3.GetNativeFunction("ForGroup");
    private static readonly IntPtr _firstOfGroupPtr = War3.GetNativeFunction("FirstOfGroup");
    private static readonly IntPtr _createForcePtr = War3.GetNativeFunction("CreateForce");
    private static readonly IntPtr _destroyForcePtr = War3.GetNativeFunction("DestroyForce");
    private static readonly IntPtr _forceAddPlayerPtr = War3.GetNativeFunction("ForceAddPlayer");
    private static readonly IntPtr _forceRemovePlayerPtr = War3.GetNativeFunction("ForceRemovePlayer");
    private static readonly IntPtr _forceClearPtr = War3.GetNativeFunction("ForceClear");
    private static readonly IntPtr _forceEnumPlayersPtr = War3.GetNativeFunction("ForceEnumPlayers");
    private static readonly IntPtr _forceEnumPlayersCountedPtr = War3.GetNativeFunction("ForceEnumPlayersCounted");
    private static readonly IntPtr _forceEnumAlliesPtr = War3.GetNativeFunction("ForceEnumAllies");
    private static readonly IntPtr _forceEnumEnemiesPtr = War3.GetNativeFunction("ForceEnumEnemies");
    private static readonly IntPtr _forForcePtr = War3.GetNativeFunction("ForForce");
    private static readonly IntPtr _rectPtr = War3.GetNativeFunction("Rect");
    private static readonly IntPtr _rectFromLocPtr = War3.GetNativeFunction("RectFromLoc");
    private static readonly IntPtr _removeRectPtr = War3.GetNativeFunction("RemoveRect");
    private static readonly IntPtr _setRectPtr = War3.GetNativeFunction("SetRect");
    private static readonly IntPtr _setRectFromLocPtr = War3.GetNativeFunction("SetRectFromLoc");
    private static readonly IntPtr _moveRectToPtr = War3.GetNativeFunction("MoveRectTo");
    private static readonly IntPtr _moveRectToLocPtr = War3.GetNativeFunction("MoveRectToLoc");
    private static readonly IntPtr _getRectCenterXPtr = War3.GetNativeFunction("GetRectCenterX");
    private static readonly IntPtr _getRectCenterYPtr = War3.GetNativeFunction("GetRectCenterY");
    private static readonly IntPtr _getRectMinXPtr = War3.GetNativeFunction("GetRectMinX");
    private static readonly IntPtr _getRectMinYPtr = War3.GetNativeFunction("GetRectMinY");
    private static readonly IntPtr _getRectMaxXPtr = War3.GetNativeFunction("GetRectMaxX");
    private static readonly IntPtr _getRectMaxYPtr = War3.GetNativeFunction("GetRectMaxY");
    private static readonly IntPtr _createRegionPtr = War3.GetNativeFunction("CreateRegion");
    private static readonly IntPtr _removeRegionPtr = War3.GetNativeFunction("RemoveRegion");
    private static readonly IntPtr _regionAddRectPtr = War3.GetNativeFunction("RegionAddRect");
    private static readonly IntPtr _regionClearRectPtr = War3.GetNativeFunction("RegionClearRect");
    private static readonly IntPtr _regionAddCellPtr = War3.GetNativeFunction("RegionAddCell");
    private static readonly IntPtr _regionAddCellAtLocPtr = War3.GetNativeFunction("RegionAddCellAtLoc");
    private static readonly IntPtr _regionClearCellPtr = War3.GetNativeFunction("RegionClearCell");
    private static readonly IntPtr _regionClearCellAtLocPtr = War3.GetNativeFunction("RegionClearCellAtLoc");
    private static readonly IntPtr _locationPtr = War3.GetNativeFunction("Location");
    private static readonly IntPtr _removeLocationPtr = War3.GetNativeFunction("RemoveLocation");
    private static readonly IntPtr _moveLocationPtr = War3.GetNativeFunction("MoveLocation");
    private static readonly IntPtr _getLocationXPtr = War3.GetNativeFunction("GetLocationX");
    private static readonly IntPtr _getLocationYPtr = War3.GetNativeFunction("GetLocationY");
    private static readonly IntPtr _getLocationZPtr = War3.GetNativeFunction("GetLocationZ");
    private static readonly IntPtr _isUnitInRegionPtr = War3.GetNativeFunction("IsUnitInRegion");
    private static readonly IntPtr _isPointInRegionPtr = War3.GetNativeFunction("IsPointInRegion");
    private static readonly IntPtr _isLocationInRegionPtr = War3.GetNativeFunction("IsLocationInRegion");
    private static readonly IntPtr _getWorldBoundsPtr = War3.GetNativeFunction("GetWorldBounds");
    private static readonly IntPtr _createTriggerPtr = War3.GetNativeFunction("CreateTrigger");
    private static readonly IntPtr _destroyTriggerPtr = War3.GetNativeFunction("DestroyTrigger");
    private static readonly IntPtr _resetTriggerPtr = War3.GetNativeFunction("ResetTrigger");
    private static readonly IntPtr _enableTriggerPtr = War3.GetNativeFunction("EnableTrigger");
    private static readonly IntPtr _disableTriggerPtr = War3.GetNativeFunction("DisableTrigger");
    private static readonly IntPtr _isTriggerEnabledPtr = War3.GetNativeFunction("IsTriggerEnabled");
    private static readonly IntPtr _triggerWaitOnSleepsPtr = War3.GetNativeFunction("TriggerWaitOnSleeps");
    private static readonly IntPtr _isTriggerWaitOnSleepsPtr = War3.GetNativeFunction("IsTriggerWaitOnSleeps");
    private static readonly IntPtr _getFilterUnitPtr = War3.GetNativeFunction("GetFilterUnit");
    private static readonly IntPtr _getEnumUnitPtr = War3.GetNativeFunction("GetEnumUnit");
    private static readonly IntPtr _getFilterDestructablePtr = War3.GetNativeFunction("GetFilterDestructable");
    private static readonly IntPtr _getEnumDestructablePtr = War3.GetNativeFunction("GetEnumDestructable");
    private static readonly IntPtr _getFilterItemPtr = War3.GetNativeFunction("GetFilterItem");
    private static readonly IntPtr _getEnumItemPtr = War3.GetNativeFunction("GetEnumItem");
    private static readonly IntPtr _getFilterPlayerPtr = War3.GetNativeFunction("GetFilterPlayer");
    private static readonly IntPtr _getEnumPlayerPtr = War3.GetNativeFunction("GetEnumPlayer");
    private static readonly IntPtr _getTriggeringTriggerPtr = War3.GetNativeFunction("GetTriggeringTrigger");
    private static readonly IntPtr _getTriggerEventIdPtr = War3.GetNativeFunction("GetTriggerEventId");
    private static readonly IntPtr _getTriggerEvalCountPtr = War3.GetNativeFunction("GetTriggerEvalCount");
    private static readonly IntPtr _getTriggerExecCountPtr = War3.GetNativeFunction("GetTriggerExecCount");
    private static readonly IntPtr _executeFuncPtr = War3.GetNativeFunction("ExecuteFunc");
    private static readonly IntPtr _andPtr = War3.GetNativeFunction("And");
    private static readonly IntPtr _orPtr = War3.GetNativeFunction("Or");
    private static readonly IntPtr _notPtr = War3.GetNativeFunction("Not");
    private static readonly IntPtr _conditionPtr = War3.GetNativeFunction("Condition");
    private static readonly IntPtr _destroyConditionPtr = War3.GetNativeFunction("DestroyCondition");
    private static readonly IntPtr _filterPtr = War3.GetNativeFunction("Filter");
    private static readonly IntPtr _destroyFilterPtr = War3.GetNativeFunction("DestroyFilter");
    private static readonly IntPtr _destroyBoolExprPtr = War3.GetNativeFunction("DestroyBoolExpr");

    private static readonly IntPtr _triggerRegisterVariableEventPtr =
        War3.GetNativeFunction("TriggerRegisterVariableEvent");

    private static readonly IntPtr _triggerRegisterTimerEventPtr = War3.GetNativeFunction("TriggerRegisterTimerEvent");

    private static readonly IntPtr _triggerRegisterTimerExpireEventPtr =
        War3.GetNativeFunction("TriggerRegisterTimerExpireEvent");

    private static readonly IntPtr _triggerRegisterGameStateEventPtr =
        War3.GetNativeFunction("TriggerRegisterGameStateEvent");

    private static readonly IntPtr _triggerRegisterDialogEventPtr =
        War3.GetNativeFunction("TriggerRegisterDialogEvent");

    private static readonly IntPtr _triggerRegisterDialogButtonEventPtr =
        War3.GetNativeFunction("TriggerRegisterDialogButtonEvent");

    private static readonly IntPtr _getEventGameStatePtr = War3.GetNativeFunction("GetEventGameState");
    private static readonly IntPtr _triggerRegisterGameEventPtr = War3.GetNativeFunction("TriggerRegisterGameEvent");
    private static readonly IntPtr _getWinningPlayerPtr = War3.GetNativeFunction("GetWinningPlayer");

    private static readonly IntPtr _triggerRegisterEnterRegionPtr =
        War3.GetNativeFunction("TriggerRegisterEnterRegion");

    private static readonly IntPtr _getTriggeringRegionPtr = War3.GetNativeFunction("GetTriggeringRegion");
    private static readonly IntPtr _getEnteringUnitPtr = War3.GetNativeFunction("GetEnteringUnit");

    private static readonly IntPtr _triggerRegisterLeaveRegionPtr =
        War3.GetNativeFunction("TriggerRegisterLeaveRegion");

    private static readonly IntPtr _getLeavingUnitPtr = War3.GetNativeFunction("GetLeavingUnit");

    private static readonly IntPtr _triggerRegisterTrackableHitEventPtr =
        War3.GetNativeFunction("TriggerRegisterTrackableHitEvent");

    private static readonly IntPtr _triggerRegisterTrackableTrackEventPtr =
        War3.GetNativeFunction("TriggerRegisterTrackableTrackEvent");

    private static readonly IntPtr _getTriggeringTrackablePtr = War3.GetNativeFunction("GetTriggeringTrackable");
    private static readonly IntPtr _getClickedButtonPtr = War3.GetNativeFunction("GetClickedButton");
    private static readonly IntPtr _getClickedDialogPtr = War3.GetNativeFunction("GetClickedDialog");

    private static readonly IntPtr _getTournamentFinishSoonTimeRemainingPtr =
        War3.GetNativeFunction("GetTournamentFinishSoonTimeRemaining");

    private static readonly IntPtr _getTournamentFinishNowRulePtr =
        War3.GetNativeFunction("GetTournamentFinishNowRule");

    private static readonly IntPtr _getTournamentFinishNowPlayerPtr =
        War3.GetNativeFunction("GetTournamentFinishNowPlayer");

    private static readonly IntPtr _getTournamentScorePtr = War3.GetNativeFunction("GetTournamentScore");
    private static readonly IntPtr _getSaveBasicFilenamePtr = War3.GetNativeFunction("GetSaveBasicFilename");

    private static readonly IntPtr _triggerRegisterPlayerEventPtr =
        War3.GetNativeFunction("TriggerRegisterPlayerEvent");

    private static readonly IntPtr _getTriggerPlayerPtr = War3.GetNativeFunction("GetTriggerPlayer");

    private static readonly IntPtr _triggerRegisterPlayerUnitEventPtr =
        War3.GetNativeFunction("TriggerRegisterPlayerUnitEvent");

    private static readonly IntPtr _getLevelingUnitPtr = War3.GetNativeFunction("GetLevelingUnit");
    private static readonly IntPtr _getLearningUnitPtr = War3.GetNativeFunction("GetLearningUnit");
    private static readonly IntPtr _getLearnedSkillPtr = War3.GetNativeFunction("GetLearnedSkill");
    private static readonly IntPtr _getLearnedSkillLevelPtr = War3.GetNativeFunction("GetLearnedSkillLevel");
    private static readonly IntPtr _getRevivableUnitPtr = War3.GetNativeFunction("GetRevivableUnit");
    private static readonly IntPtr _getRevivingUnitPtr = War3.GetNativeFunction("GetRevivingUnit");
    private static readonly IntPtr _getAttackerPtr = War3.GetNativeFunction("GetAttacker");
    private static readonly IntPtr _getRescuerPtr = War3.GetNativeFunction("GetRescuer");
    private static readonly IntPtr _getDyingUnitPtr = War3.GetNativeFunction("GetDyingUnit");
    private static readonly IntPtr _getKillingUnitPtr = War3.GetNativeFunction("GetKillingUnit");
    private static readonly IntPtr _getDecayingUnitPtr = War3.GetNativeFunction("GetDecayingUnit");
    private static readonly IntPtr _getConstructingStructurePtr = War3.GetNativeFunction("GetConstructingStructure");
    private static readonly IntPtr _getCancelledStructurePtr = War3.GetNativeFunction("GetCancelledStructure");
    private static readonly IntPtr _getConstructedStructurePtr = War3.GetNativeFunction("GetConstructedStructure");
    private static readonly IntPtr _getResearchingUnitPtr = War3.GetNativeFunction("GetResearchingUnit");
    private static readonly IntPtr _getResearchedPtr = War3.GetNativeFunction("GetResearched");
    private static readonly IntPtr _getTrainedUnitTypePtr = War3.GetNativeFunction("GetTrainedUnitType");
    private static readonly IntPtr _getTrainedUnitPtr = War3.GetNativeFunction("GetTrainedUnit");
    private static readonly IntPtr _getDetectedUnitPtr = War3.GetNativeFunction("GetDetectedUnit");
    private static readonly IntPtr _getSummoningUnitPtr = War3.GetNativeFunction("GetSummoningUnit");
    private static readonly IntPtr _getSummonedUnitPtr = War3.GetNativeFunction("GetSummonedUnit");
    private static readonly IntPtr _getTransportUnitPtr = War3.GetNativeFunction("GetTransportUnit");
    private static readonly IntPtr _getLoadedUnitPtr = War3.GetNativeFunction("GetLoadedUnit");
    private static readonly IntPtr _getSellingUnitPtr = War3.GetNativeFunction("GetSellingUnit");
    private static readonly IntPtr _getSoldUnitPtr = War3.GetNativeFunction("GetSoldUnit");
    private static readonly IntPtr _getBuyingUnitPtr = War3.GetNativeFunction("GetBuyingUnit");
    private static readonly IntPtr _getSoldItemPtr = War3.GetNativeFunction("GetSoldItem");
    private static readonly IntPtr _getChangingUnitPtr = War3.GetNativeFunction("GetChangingUnit");
    private static readonly IntPtr _getChangingUnitPrevOwnerPtr = War3.GetNativeFunction("GetChangingUnitPrevOwner");
    private static readonly IntPtr _getManipulatingUnitPtr = War3.GetNativeFunction("GetManipulatingUnit");
    private static readonly IntPtr _getManipulatedItemPtr = War3.GetNativeFunction("GetManipulatedItem");
    private static readonly IntPtr _getOrderedUnitPtr = War3.GetNativeFunction("GetOrderedUnit");
    private static readonly IntPtr _getIssuedOrderIdPtr = War3.GetNativeFunction("GetIssuedOrderId");
    private static readonly IntPtr _getOrderPointXPtr = War3.GetNativeFunction("GetOrderPointX");
    private static readonly IntPtr _getOrderPointYPtr = War3.GetNativeFunction("GetOrderPointY");
    private static readonly IntPtr _getOrderPointLocPtr = War3.GetNativeFunction("GetOrderPointLoc");
    private static readonly IntPtr _getOrderTargetPtr = War3.GetNativeFunction("GetOrderTarget");

    private static readonly IntPtr _getOrderTargetDestructablePtr =
        War3.GetNativeFunction("GetOrderTargetDestructable");

    private static readonly IntPtr _getOrderTargetItemPtr = War3.GetNativeFunction("GetOrderTargetItem");
    private static readonly IntPtr _getOrderTargetUnitPtr = War3.GetNativeFunction("GetOrderTargetUnit");
    private static readonly IntPtr _getSpellAbilityUnitPtr = War3.GetNativeFunction("GetSpellAbilityUnit");
    private static readonly IntPtr _getSpellAbilityIdPtr = War3.GetNativeFunction("GetSpellAbilityId");
    private static readonly IntPtr _getSpellAbilityPtr = War3.GetNativeFunction("GetSpellAbility");
    private static readonly IntPtr _getSpellTargetLocPtr = War3.GetNativeFunction("GetSpellTargetLoc");
    private static readonly IntPtr _getSpellTargetXPtr = War3.GetNativeFunction("GetSpellTargetX");
    private static readonly IntPtr _getSpellTargetYPtr = War3.GetNativeFunction("GetSpellTargetY");

    private static readonly IntPtr _getSpellTargetDestructablePtr =
        War3.GetNativeFunction("GetSpellTargetDestructable");

    private static readonly IntPtr _getSpellTargetItemPtr = War3.GetNativeFunction("GetSpellTargetItem");
    private static readonly IntPtr _getSpellTargetUnitPtr = War3.GetNativeFunction("GetSpellTargetUnit");

    private static readonly IntPtr _triggerRegisterPlayerAllianceChangePtr =
        War3.GetNativeFunction("TriggerRegisterPlayerAllianceChange");

    private static readonly IntPtr _triggerRegisterPlayerStateEventPtr =
        War3.GetNativeFunction("TriggerRegisterPlayerStateEvent");

    private static readonly IntPtr _getEventPlayerStatePtr = War3.GetNativeFunction("GetEventPlayerState");

    private static readonly IntPtr _triggerRegisterPlayerChatEventPtr =
        War3.GetNativeFunction("TriggerRegisterPlayerChatEvent");

    private static readonly IntPtr _getEventPlayerChatStringPtr = War3.GetNativeFunction("GetEventPlayerChatString");

    private static readonly IntPtr _getEventPlayerChatStringMatchedPtr =
        War3.GetNativeFunction("GetEventPlayerChatStringMatched");

    private static readonly IntPtr _triggerRegisterDeathEventPtr = War3.GetNativeFunction("TriggerRegisterDeathEvent");
    private static readonly IntPtr _getTriggerUnitPtr = War3.GetNativeFunction("GetTriggerUnit");

    private static readonly IntPtr _triggerRegisterUnitStateEventPtr =
        War3.GetNativeFunction("TriggerRegisterUnitStateEvent");

    private static readonly IntPtr _getEventUnitStatePtr = War3.GetNativeFunction("GetEventUnitState");
    private static readonly IntPtr _triggerRegisterUnitEventPtr = War3.GetNativeFunction("TriggerRegisterUnitEvent");
    private static readonly IntPtr _getEventDamagePtr = War3.GetNativeFunction("GetEventDamage");
    private static readonly IntPtr _getEventDamageSourcePtr = War3.GetNativeFunction("GetEventDamageSource");
    private static readonly IntPtr _getEventDetectingPlayerPtr = War3.GetNativeFunction("GetEventDetectingPlayer");

    private static readonly IntPtr _triggerRegisterFilterUnitEventPtr =
        War3.GetNativeFunction("TriggerRegisterFilterUnitEvent");

    private static readonly IntPtr _getEventTargetUnitPtr = War3.GetNativeFunction("GetEventTargetUnit");

    private static readonly IntPtr _triggerRegisterUnitInRangePtr =
        War3.GetNativeFunction("TriggerRegisterUnitInRange");

    private static readonly IntPtr _triggerAddConditionPtr = War3.GetNativeFunction("TriggerAddCondition");
    private static readonly IntPtr _triggerRemoveConditionPtr = War3.GetNativeFunction("TriggerRemoveCondition");
    private static readonly IntPtr _triggerClearConditionsPtr = War3.GetNativeFunction("TriggerClearConditions");
    private static readonly IntPtr _triggerAddActionPtr = War3.GetNativeFunction("TriggerAddAction");
    private static readonly IntPtr _triggerRemoveActionPtr = War3.GetNativeFunction("TriggerRemoveAction");
    private static readonly IntPtr _triggerClearActionsPtr = War3.GetNativeFunction("TriggerClearActions");
    private static readonly IntPtr _triggerSleepActionPtr = War3.GetNativeFunction("TriggerSleepAction");
    private static readonly IntPtr _triggerWaitForSoundPtr = War3.GetNativeFunction("TriggerWaitForSound");
    private static readonly IntPtr _triggerEvaluatePtr = War3.GetNativeFunction("TriggerEvaluate");
    private static readonly IntPtr _triggerExecutePtr = War3.GetNativeFunction("TriggerExecute");
    private static readonly IntPtr _triggerExecuteWaitPtr = War3.GetNativeFunction("TriggerExecuteWait");
    private static readonly IntPtr _triggerSyncStartPtr = War3.GetNativeFunction("TriggerSyncStart");
    private static readonly IntPtr _triggerSyncReadyPtr = War3.GetNativeFunction("TriggerSyncReady");
    private static readonly IntPtr _getWidgetLifePtr = War3.GetNativeFunction("GetWidgetLife");
    private static readonly IntPtr _setWidgetLifePtr = War3.GetNativeFunction("SetWidgetLife");
    private static readonly IntPtr _getWidgetXPtr = War3.GetNativeFunction("GetWidgetX");
    private static readonly IntPtr _getWidgetYPtr = War3.GetNativeFunction("GetWidgetY");
    private static readonly IntPtr _getTriggerWidgetPtr = War3.GetNativeFunction("GetTriggerWidget");
    private static readonly IntPtr _createDestructablePtr = War3.GetNativeFunction("CreateDestructable");
    private static readonly IntPtr _createDestructableZPtr = War3.GetNativeFunction("CreateDestructableZ");
    private static readonly IntPtr _createDeadDestructablePtr = War3.GetNativeFunction("CreateDeadDestructable");
    private static readonly IntPtr _createDeadDestructableZPtr = War3.GetNativeFunction("CreateDeadDestructableZ");
    private static readonly IntPtr _removeDestructablePtr = War3.GetNativeFunction("RemoveDestructable");
    private static readonly IntPtr _killDestructablePtr = War3.GetNativeFunction("KillDestructable");

    private static readonly IntPtr _setDestructableInvulnerablePtr =
        War3.GetNativeFunction("SetDestructableInvulnerable");

    private static readonly IntPtr _isDestructableInvulnerablePtr =
        War3.GetNativeFunction("IsDestructableInvulnerable");

    private static readonly IntPtr _enumDestructablesInRectPtr = War3.GetNativeFunction("EnumDestructablesInRect");
    private static readonly IntPtr _getDestructableTypeIdPtr = War3.GetNativeFunction("GetDestructableTypeId");
    private static readonly IntPtr _getDestructableXPtr = War3.GetNativeFunction("GetDestructableX");
    private static readonly IntPtr _getDestructableYPtr = War3.GetNativeFunction("GetDestructableY");
    private static readonly IntPtr _setDestructableLifePtr = War3.GetNativeFunction("SetDestructableLife");
    private static readonly IntPtr _getDestructableLifePtr = War3.GetNativeFunction("GetDestructableLife");
    private static readonly IntPtr _setDestructableMaxLifePtr = War3.GetNativeFunction("SetDestructableMaxLife");
    private static readonly IntPtr _getDestructableMaxLifePtr = War3.GetNativeFunction("GetDestructableMaxLife");
    private static readonly IntPtr _destructableRestoreLifePtr = War3.GetNativeFunction("DestructableRestoreLife");

    private static readonly IntPtr _queueDestructableAnimationPtr =
        War3.GetNativeFunction("QueueDestructableAnimation");

    private static readonly IntPtr _setDestructableAnimationPtr = War3.GetNativeFunction("SetDestructableAnimation");

    private static readonly IntPtr _setDestructableAnimationSpeedPtr =
        War3.GetNativeFunction("SetDestructableAnimationSpeed");

    private static readonly IntPtr _showDestructablePtr = War3.GetNativeFunction("ShowDestructable");

    private static readonly IntPtr _getDestructableOccluderHeightPtr =
        War3.GetNativeFunction("GetDestructableOccluderHeight");

    private static readonly IntPtr _setDestructableOccluderHeightPtr =
        War3.GetNativeFunction("SetDestructableOccluderHeight");

    private static readonly IntPtr _getDestructableNamePtr = War3.GetNativeFunction("GetDestructableName");
    private static readonly IntPtr _getTriggerDestructablePtr = War3.GetNativeFunction("GetTriggerDestructable");
    private static readonly IntPtr _createItemPtr = War3.GetNativeFunction("CreateItem");
    private static readonly IntPtr _removeItemPtr = War3.GetNativeFunction("RemoveItem");
    private static readonly IntPtr _getItemPlayerPtr = War3.GetNativeFunction("GetItemPlayer");
    private static readonly IntPtr _getItemTypeIdPtr = War3.GetNativeFunction("GetItemTypeId");
    private static readonly IntPtr _getItemXPtr = War3.GetNativeFunction("GetItemX");
    private static readonly IntPtr _getItemYPtr = War3.GetNativeFunction("GetItemY");
    private static readonly IntPtr _setItemPositionPtr = War3.GetNativeFunction("SetItemPosition");
    private static readonly IntPtr _setItemDropOnDeathPtr = War3.GetNativeFunction("SetItemDropOnDeath");
    private static readonly IntPtr _setItemDroppablePtr = War3.GetNativeFunction("SetItemDroppable");
    private static readonly IntPtr _setItemPawnablePtr = War3.GetNativeFunction("SetItemPawnable");
    private static readonly IntPtr _setItemPlayerPtr = War3.GetNativeFunction("SetItemPlayer");
    private static readonly IntPtr _setItemInvulnerablePtr = War3.GetNativeFunction("SetItemInvulnerable");
    private static readonly IntPtr _isItemInvulnerablePtr = War3.GetNativeFunction("IsItemInvulnerable");
    private static readonly IntPtr _setItemVisiblePtr = War3.GetNativeFunction("SetItemVisible");
    private static readonly IntPtr _isItemVisiblePtr = War3.GetNativeFunction("IsItemVisible");
    private static readonly IntPtr _isItemOwnedPtr = War3.GetNativeFunction("IsItemOwned");
    private static readonly IntPtr _isItemPowerupPtr = War3.GetNativeFunction("IsItemPowerup");
    private static readonly IntPtr _isItemSellablePtr = War3.GetNativeFunction("IsItemSellable");
    private static readonly IntPtr _isItemPawnablePtr = War3.GetNativeFunction("IsItemPawnable");
    private static readonly IntPtr _isItemIdPowerupPtr = War3.GetNativeFunction("IsItemIdPowerup");
    private static readonly IntPtr _isItemIdSellablePtr = War3.GetNativeFunction("IsItemIdSellable");
    private static readonly IntPtr _isItemIdPawnablePtr = War3.GetNativeFunction("IsItemIdPawnable");
    private static readonly IntPtr _enumItemsInRectPtr = War3.GetNativeFunction("EnumItemsInRect");
    private static readonly IntPtr _getItemLevelPtr = War3.GetNativeFunction("GetItemLevel");
    private static readonly IntPtr _getItemTypePtr = War3.GetNativeFunction("GetItemType");
    private static readonly IntPtr _setItemDropIDPtr = War3.GetNativeFunction("SetItemDropID");
    private static readonly IntPtr _getItemNamePtr = War3.GetNativeFunction("GetItemName");
    private static readonly IntPtr _getItemChargesPtr = War3.GetNativeFunction("GetItemCharges");
    private static readonly IntPtr _setItemChargesPtr = War3.GetNativeFunction("SetItemCharges");
    private static readonly IntPtr _getItemUserDataPtr = War3.GetNativeFunction("GetItemUserData");
    private static readonly IntPtr _setItemUserDataPtr = War3.GetNativeFunction("SetItemUserData");
    private static readonly IntPtr _createUnitPtr = War3.GetNativeFunction("CreateUnit");
    private static readonly IntPtr _createUnitByNamePtr = War3.GetNativeFunction("CreateUnitByName");
    private static readonly IntPtr _createUnitAtLocPtr = War3.GetNativeFunction("CreateUnitAtLoc");
    private static readonly IntPtr _createUnitAtLocByNamePtr = War3.GetNativeFunction("CreateUnitAtLocByName");
    private static readonly IntPtr _createCorpsePtr = War3.GetNativeFunction("CreateCorpse");
    private static readonly IntPtr _killUnitPtr = War3.GetNativeFunction("KillUnit");
    private static readonly IntPtr _removeUnitPtr = War3.GetNativeFunction("RemoveUnit");
    private static readonly IntPtr _showUnitPtr = War3.GetNativeFunction("ShowUnit");
    private static readonly IntPtr _setUnitStatePtr = War3.GetNativeFunction("SetUnitState");
    private static readonly IntPtr _setUnitXPtr = War3.GetNativeFunction("SetUnitX");
    private static readonly IntPtr _setUnitYPtr = War3.GetNativeFunction("SetUnitY");
    private static readonly IntPtr _setUnitPositionPtr = War3.GetNativeFunction("SetUnitPosition");
    private static readonly IntPtr _setUnitPositionLocPtr = War3.GetNativeFunction("SetUnitPositionLoc");
    private static readonly IntPtr _setUnitFacingPtr = War3.GetNativeFunction("SetUnitFacing");
    private static readonly IntPtr _setUnitFacingTimedPtr = War3.GetNativeFunction("SetUnitFacingTimed");
    private static readonly IntPtr _setUnitMoveSpeedPtr = War3.GetNativeFunction("SetUnitMoveSpeed");
    private static readonly IntPtr _setUnitFlyHeightPtr = War3.GetNativeFunction("SetUnitFlyHeight");
    private static readonly IntPtr _setUnitTurnSpeedPtr = War3.GetNativeFunction("SetUnitTurnSpeed");
    private static readonly IntPtr _setUnitPropWindowPtr = War3.GetNativeFunction("SetUnitPropWindow");
    private static readonly IntPtr _setUnitAcquireRangePtr = War3.GetNativeFunction("SetUnitAcquireRange");
    private static readonly IntPtr _setUnitCreepGuardPtr = War3.GetNativeFunction("SetUnitCreepGuard");
    private static readonly IntPtr _getUnitAcquireRangePtr = War3.GetNativeFunction("GetUnitAcquireRange");
    private static readonly IntPtr _getUnitTurnSpeedPtr = War3.GetNativeFunction("GetUnitTurnSpeed");
    private static readonly IntPtr _getUnitPropWindowPtr = War3.GetNativeFunction("GetUnitPropWindow");
    private static readonly IntPtr _getUnitFlyHeightPtr = War3.GetNativeFunction("GetUnitFlyHeight");

    private static readonly IntPtr _getUnitDefaultAcquireRangePtr =
        War3.GetNativeFunction("GetUnitDefaultAcquireRange");

    private static readonly IntPtr _getUnitDefaultTurnSpeedPtr = War3.GetNativeFunction("GetUnitDefaultTurnSpeed");
    private static readonly IntPtr _getUnitDefaultPropWindowPtr = War3.GetNativeFunction("GetUnitDefaultPropWindow");
    private static readonly IntPtr _getUnitDefaultFlyHeightPtr = War3.GetNativeFunction("GetUnitDefaultFlyHeight");
    private static readonly IntPtr _setUnitOwnerPtr = War3.GetNativeFunction("SetUnitOwner");
    private static readonly IntPtr _setUnitColorPtr = War3.GetNativeFunction("SetUnitColor");
    private static readonly IntPtr _setUnitScalePtr = War3.GetNativeFunction("SetUnitScale");
    private static readonly IntPtr _setUnitTimeScalePtr = War3.GetNativeFunction("SetUnitTimeScale");
    private static readonly IntPtr _setUnitBlendTimePtr = War3.GetNativeFunction("SetUnitBlendTime");
    private static readonly IntPtr _setUnitVertexColorPtr = War3.GetNativeFunction("SetUnitVertexColor");
    private static readonly IntPtr _queueUnitAnimationPtr = War3.GetNativeFunction("QueueUnitAnimation");
    private static readonly IntPtr _setUnitAnimationPtr = War3.GetNativeFunction("SetUnitAnimation");
    private static readonly IntPtr _setUnitAnimationByIndexPtr = War3.GetNativeFunction("SetUnitAnimationByIndex");

    private static readonly IntPtr _setUnitAnimationWithRarityPtr =
        War3.GetNativeFunction("SetUnitAnimationWithRarity");

    private static readonly IntPtr _addUnitAnimationPropertiesPtr =
        War3.GetNativeFunction("AddUnitAnimationProperties");

    private static readonly IntPtr _setUnitLookAtPtr = War3.GetNativeFunction("SetUnitLookAt");
    private static readonly IntPtr _resetUnitLookAtPtr = War3.GetNativeFunction("ResetUnitLookAt");
    private static readonly IntPtr _setUnitRescuablePtr = War3.GetNativeFunction("SetUnitRescuable");
    private static readonly IntPtr _setUnitRescueRangePtr = War3.GetNativeFunction("SetUnitRescueRange");
    private static readonly IntPtr _setHeroStrPtr = War3.GetNativeFunction("SetHeroStr");
    private static readonly IntPtr _setHeroAgiPtr = War3.GetNativeFunction("SetHeroAgi");
    private static readonly IntPtr _setHeroIntPtr = War3.GetNativeFunction("SetHeroInt");
    private static readonly IntPtr _getHeroStrPtr = War3.GetNativeFunction("GetHeroStr");
    private static readonly IntPtr _getHeroAgiPtr = War3.GetNativeFunction("GetHeroAgi");
    private static readonly IntPtr _getHeroIntPtr = War3.GetNativeFunction("GetHeroInt");
    private static readonly IntPtr _unitStripHeroLevelPtr = War3.GetNativeFunction("UnitStripHeroLevel");
    private static readonly IntPtr _getHeroXPPtr = War3.GetNativeFunction("GetHeroXP");
    private static readonly IntPtr _setHeroXPPtr = War3.GetNativeFunction("SetHeroXP");
    private static readonly IntPtr _getHeroSkillPointsPtr = War3.GetNativeFunction("GetHeroSkillPoints");
    private static readonly IntPtr _unitModifySkillPointsPtr = War3.GetNativeFunction("UnitModifySkillPoints");
    private static readonly IntPtr _addHeroXPPtr = War3.GetNativeFunction("AddHeroXP");
    private static readonly IntPtr _setHeroLevelPtr = War3.GetNativeFunction("SetHeroLevel");
    private static readonly IntPtr _getHeroLevelPtr = War3.GetNativeFunction("GetHeroLevel");
    private static readonly IntPtr _getUnitLevelPtr = War3.GetNativeFunction("GetUnitLevel");
    private static readonly IntPtr _getHeroProperNamePtr = War3.GetNativeFunction("GetHeroProperName");
    private static readonly IntPtr _suspendHeroXPPtr = War3.GetNativeFunction("SuspendHeroXP");
    private static readonly IntPtr _isSuspendedXPPtr = War3.GetNativeFunction("IsSuspendedXP");
    private static readonly IntPtr _selectHeroSkillPtr = War3.GetNativeFunction("SelectHeroSkill");
    private static readonly IntPtr _getUnitAbilityLevelPtr = War3.GetNativeFunction("GetUnitAbilityLevel");
    private static readonly IntPtr _decUnitAbilityLevelPtr = War3.GetNativeFunction("DecUnitAbilityLevel");
    private static readonly IntPtr _incUnitAbilityLevelPtr = War3.GetNativeFunction("IncUnitAbilityLevel");
    private static readonly IntPtr _setUnitAbilityLevelPtr = War3.GetNativeFunction("SetUnitAbilityLevel");
    private static readonly IntPtr _reviveHeroPtr = War3.GetNativeFunction("ReviveHero");
    private static readonly IntPtr _reviveHeroLocPtr = War3.GetNativeFunction("ReviveHeroLoc");
    private static readonly IntPtr _setUnitExplodedPtr = War3.GetNativeFunction("SetUnitExploded");
    private static readonly IntPtr _setUnitInvulnerablePtr = War3.GetNativeFunction("SetUnitInvulnerable");
    private static readonly IntPtr _pauseUnitPtr = War3.GetNativeFunction("PauseUnit");
    private static readonly IntPtr _isUnitPausedPtr = War3.GetNativeFunction("IsUnitPaused");
    private static readonly IntPtr _setUnitPathingPtr = War3.GetNativeFunction("SetUnitPathing");
    private static readonly IntPtr _clearSelectionPtr = War3.GetNativeFunction("ClearSelection");
    private static readonly IntPtr _selectUnitPtr = War3.GetNativeFunction("SelectUnit");
    private static readonly IntPtr _getUnitPointValuePtr = War3.GetNativeFunction("GetUnitPointValue");
    private static readonly IntPtr _getUnitPointValueByTypePtr = War3.GetNativeFunction("GetUnitPointValueByType");
    private static readonly IntPtr _unitAddItemPtr = War3.GetNativeFunction("UnitAddItem");
    private static readonly IntPtr _unitAddItemByIdPtr = War3.GetNativeFunction("UnitAddItemById");
    private static readonly IntPtr _unitAddItemToSlotByIdPtr = War3.GetNativeFunction("UnitAddItemToSlotById");
    private static readonly IntPtr _unitRemoveItemPtr = War3.GetNativeFunction("UnitRemoveItem");
    private static readonly IntPtr _unitRemoveItemFromSlotPtr = War3.GetNativeFunction("UnitRemoveItemFromSlot");
    private static readonly IntPtr _unitHasItemPtr = War3.GetNativeFunction("UnitHasItem");
    private static readonly IntPtr _unitItemInSlotPtr = War3.GetNativeFunction("UnitItemInSlot");
    private static readonly IntPtr _unitInventorySizePtr = War3.GetNativeFunction("UnitInventorySize");
    private static readonly IntPtr _unitDropItemPointPtr = War3.GetNativeFunction("UnitDropItemPoint");
    private static readonly IntPtr _unitDropItemSlotPtr = War3.GetNativeFunction("UnitDropItemSlot");
    private static readonly IntPtr _unitDropItemTargetPtr = War3.GetNativeFunction("UnitDropItemTarget");
    private static readonly IntPtr _unitUseItemPtr = War3.GetNativeFunction("UnitUseItem");
    private static readonly IntPtr _unitUseItemPointPtr = War3.GetNativeFunction("UnitUseItemPoint");
    private static readonly IntPtr _unitUseItemTargetPtr = War3.GetNativeFunction("UnitUseItemTarget");
    private static readonly IntPtr _getUnitXPtr = War3.GetNativeFunction("GetUnitX");
    private static readonly IntPtr _getUnitYPtr = War3.GetNativeFunction("GetUnitY");
    private static readonly IntPtr _getUnitLocPtr = War3.GetNativeFunction("GetUnitLoc");
    private static readonly IntPtr _getUnitFacingPtr = War3.GetNativeFunction("GetUnitFacing");
    private static readonly IntPtr _getUnitMoveSpeedPtr = War3.GetNativeFunction("GetUnitMoveSpeed");
    private static readonly IntPtr _getUnitDefaultMoveSpeedPtr = War3.GetNativeFunction("GetUnitDefaultMoveSpeed");
    private static readonly IntPtr _getUnitStatePtr = War3.GetNativeFunction("GetUnitState");
    private static readonly IntPtr _getOwningPlayerPtr = War3.GetNativeFunction("GetOwningPlayer");
    private static readonly IntPtr _getUnitTypeIdPtr = War3.GetNativeFunction("GetUnitTypeId");
    private static readonly IntPtr _getUnitRacePtr = War3.GetNativeFunction("GetUnitRace");
    private static readonly IntPtr _getUnitNamePtr = War3.GetNativeFunction("GetUnitName");
    private static readonly IntPtr _getUnitFoodUsedPtr = War3.GetNativeFunction("GetUnitFoodUsed");
    private static readonly IntPtr _getUnitFoodMadePtr = War3.GetNativeFunction("GetUnitFoodMade");
    private static readonly IntPtr _getFoodMadePtr = War3.GetNativeFunction("GetFoodMade");
    private static readonly IntPtr _getFoodUsedPtr = War3.GetNativeFunction("GetFoodUsed");
    private static readonly IntPtr _setUnitUseFoodPtr = War3.GetNativeFunction("SetUnitUseFood");
    private static readonly IntPtr _getUnitRallyPointPtr = War3.GetNativeFunction("GetUnitRallyPoint");
    private static readonly IntPtr _getUnitRallyUnitPtr = War3.GetNativeFunction("GetUnitRallyUnit");
    private static readonly IntPtr _getUnitRallyDestructablePtr = War3.GetNativeFunction("GetUnitRallyDestructable");
    private static readonly IntPtr _isUnitInGroupPtr = War3.GetNativeFunction("IsUnitInGroup");
    private static readonly IntPtr _isUnitInForcePtr = War3.GetNativeFunction("IsUnitInForce");
    private static readonly IntPtr _isUnitOwnedByPlayerPtr = War3.GetNativeFunction("IsUnitOwnedByPlayer");
    private static readonly IntPtr _isUnitAllyPtr = War3.GetNativeFunction("IsUnitAlly");
    private static readonly IntPtr _isUnitEnemyPtr = War3.GetNativeFunction("IsUnitEnemy");
    private static readonly IntPtr _isUnitVisiblePtr = War3.GetNativeFunction("IsUnitVisible");
    private static readonly IntPtr _isUnitDetectedPtr = War3.GetNativeFunction("IsUnitDetected");
    private static readonly IntPtr _isUnitInvisiblePtr = War3.GetNativeFunction("IsUnitInvisible");
    private static readonly IntPtr _isUnitFoggedPtr = War3.GetNativeFunction("IsUnitFogged");
    private static readonly IntPtr _isUnitMaskedPtr = War3.GetNativeFunction("IsUnitMasked");
    private static readonly IntPtr _isUnitSelectedPtr = War3.GetNativeFunction("IsUnitSelected");
    private static readonly IntPtr _isUnitRacePtr = War3.GetNativeFunction("IsUnitRace");
    private static readonly IntPtr _isUnitTypePtr = War3.GetNativeFunction("IsUnitType");
    private static readonly IntPtr _isUnitPtr = War3.GetNativeFunction("IsUnit");
    private static readonly IntPtr _isUnitInRangePtr = War3.GetNativeFunction("IsUnitInRange");
    private static readonly IntPtr _isUnitInRangeXYPtr = War3.GetNativeFunction("IsUnitInRangeXY");
    private static readonly IntPtr _isUnitInRangeLocPtr = War3.GetNativeFunction("IsUnitInRangeLoc");
    private static readonly IntPtr _isUnitHiddenPtr = War3.GetNativeFunction("IsUnitHidden");
    private static readonly IntPtr _isUnitIllusionPtr = War3.GetNativeFunction("IsUnitIllusion");
    private static readonly IntPtr _isUnitInTransportPtr = War3.GetNativeFunction("IsUnitInTransport");
    private static readonly IntPtr _isUnitLoadedPtr = War3.GetNativeFunction("IsUnitLoaded");
    private static readonly IntPtr _isHeroUnitIdPtr = War3.GetNativeFunction("IsHeroUnitId");
    private static readonly IntPtr _isUnitIdTypePtr = War3.GetNativeFunction("IsUnitIdType");
    private static readonly IntPtr _unitShareVisionPtr = War3.GetNativeFunction("UnitShareVision");
    private static readonly IntPtr _unitSuspendDecayPtr = War3.GetNativeFunction("UnitSuspendDecay");
    private static readonly IntPtr _unitAddTypePtr = War3.GetNativeFunction("UnitAddType");
    private static readonly IntPtr _unitRemoveTypePtr = War3.GetNativeFunction("UnitRemoveType");
    private static readonly IntPtr _unitAddAbilityPtr = War3.GetNativeFunction("UnitAddAbility");
    private static readonly IntPtr _unitRemoveAbilityPtr = War3.GetNativeFunction("UnitRemoveAbility");
    private static readonly IntPtr _unitMakeAbilityPermanentPtr = War3.GetNativeFunction("UnitMakeAbilityPermanent");
    private static readonly IntPtr _unitRemoveBuffsPtr = War3.GetNativeFunction("UnitRemoveBuffs");
    private static readonly IntPtr _unitRemoveBuffsExPtr = War3.GetNativeFunction("UnitRemoveBuffsEx");
    private static readonly IntPtr _unitHasBuffsExPtr = War3.GetNativeFunction("UnitHasBuffsEx");
    private static readonly IntPtr _unitCountBuffsExPtr = War3.GetNativeFunction("UnitCountBuffsEx");
    private static readonly IntPtr _unitAddSleepPtr = War3.GetNativeFunction("UnitAddSleep");
    private static readonly IntPtr _unitCanSleepPtr = War3.GetNativeFunction("UnitCanSleep");
    private static readonly IntPtr _unitAddSleepPermPtr = War3.GetNativeFunction("UnitAddSleepPerm");
    private static readonly IntPtr _unitCanSleepPermPtr = War3.GetNativeFunction("UnitCanSleepPerm");
    private static readonly IntPtr _unitIsSleepingPtr = War3.GetNativeFunction("UnitIsSleeping");
    private static readonly IntPtr _unitWakeUpPtr = War3.GetNativeFunction("UnitWakeUp");
    private static readonly IntPtr _unitApplyTimedLifePtr = War3.GetNativeFunction("UnitApplyTimedLife");
    private static readonly IntPtr _unitIgnoreAlarmPtr = War3.GetNativeFunction("UnitIgnoreAlarm");
    private static readonly IntPtr _unitIgnoreAlarmToggledPtr = War3.GetNativeFunction("UnitIgnoreAlarmToggled");
    private static readonly IntPtr _unitResetCooldownPtr = War3.GetNativeFunction("UnitResetCooldown");

    private static readonly IntPtr _unitSetConstructionProgressPtr =
        War3.GetNativeFunction("UnitSetConstructionProgress");

    private static readonly IntPtr _unitSetUpgradeProgressPtr = War3.GetNativeFunction("UnitSetUpgradeProgress");
    private static readonly IntPtr _unitPauseTimedLifePtr = War3.GetNativeFunction("UnitPauseTimedLife");
    private static readonly IntPtr _unitSetUsesAltIconPtr = War3.GetNativeFunction("UnitSetUsesAltIcon");
    private static readonly IntPtr _unitDamagePointPtr = War3.GetNativeFunction("UnitDamagePoint");
    private static readonly IntPtr _unitDamageTargetPtr = War3.GetNativeFunction("UnitDamageTarget");
    private static readonly IntPtr _issueImmediateOrderPtr = War3.GetNativeFunction("IssueImmediateOrder");
    private static readonly IntPtr _issueImmediateOrderByIdPtr = War3.GetNativeFunction("IssueImmediateOrderById");
    private static readonly IntPtr _issuePointOrderPtr = War3.GetNativeFunction("IssuePointOrder");
    private static readonly IntPtr _issuePointOrderLocPtr = War3.GetNativeFunction("IssuePointOrderLoc");
    private static readonly IntPtr _issuePointOrderByIdPtr = War3.GetNativeFunction("IssuePointOrderById");
    private static readonly IntPtr _issuePointOrderByIdLocPtr = War3.GetNativeFunction("IssuePointOrderByIdLoc");
    private static readonly IntPtr _issueTargetOrderPtr = War3.GetNativeFunction("IssueTargetOrder");
    private static readonly IntPtr _issueTargetOrderByIdPtr = War3.GetNativeFunction("IssueTargetOrderById");
    private static readonly IntPtr _issueInstantPointOrderPtr = War3.GetNativeFunction("IssueInstantPointOrder");

    private static readonly IntPtr _issueInstantPointOrderByIdPtr =
        War3.GetNativeFunction("IssueInstantPointOrderById");

    private static readonly IntPtr _issueInstantTargetOrderPtr = War3.GetNativeFunction("IssueInstantTargetOrder");

    private static readonly IntPtr _issueInstantTargetOrderByIdPtr =
        War3.GetNativeFunction("IssueInstantTargetOrderById");

    private static readonly IntPtr _issueBuildOrderPtr = War3.GetNativeFunction("IssueBuildOrder");
    private static readonly IntPtr _issueBuildOrderByIdPtr = War3.GetNativeFunction("IssueBuildOrderById");

    private static readonly IntPtr _issueNeutralImmediateOrderPtr =
        War3.GetNativeFunction("IssueNeutralImmediateOrder");

    private static readonly IntPtr _issueNeutralImmediateOrderByIdPtr =
        War3.GetNativeFunction("IssueNeutralImmediateOrderById");

    private static readonly IntPtr _issueNeutralPointOrderPtr = War3.GetNativeFunction("IssueNeutralPointOrder");

    private static readonly IntPtr _issueNeutralPointOrderByIdPtr =
        War3.GetNativeFunction("IssueNeutralPointOrderById");

    private static readonly IntPtr _issueNeutralTargetOrderPtr = War3.GetNativeFunction("IssueNeutralTargetOrder");

    private static readonly IntPtr _issueNeutralTargetOrderByIdPtr =
        War3.GetNativeFunction("IssueNeutralTargetOrderById");

    private static readonly IntPtr _getUnitCurrentOrderPtr = War3.GetNativeFunction("GetUnitCurrentOrder");
    private static readonly IntPtr _setResourceAmountPtr = War3.GetNativeFunction("SetResourceAmount");
    private static readonly IntPtr _addResourceAmountPtr = War3.GetNativeFunction("AddResourceAmount");
    private static readonly IntPtr _getResourceAmountPtr = War3.GetNativeFunction("GetResourceAmount");
    private static readonly IntPtr _waygateGetDestinationXPtr = War3.GetNativeFunction("WaygateGetDestinationX");
    private static readonly IntPtr _waygateGetDestinationYPtr = War3.GetNativeFunction("WaygateGetDestinationY");
    private static readonly IntPtr _waygateSetDestinationPtr = War3.GetNativeFunction("WaygateSetDestination");
    private static readonly IntPtr _waygateActivatePtr = War3.GetNativeFunction("WaygateActivate");
    private static readonly IntPtr _waygateIsActivePtr = War3.GetNativeFunction("WaygateIsActive");
    private static readonly IntPtr _addItemToAllStockPtr = War3.GetNativeFunction("AddItemToAllStock");
    private static readonly IntPtr _addItemToStockPtr = War3.GetNativeFunction("AddItemToStock");
    private static readonly IntPtr _addUnitToAllStockPtr = War3.GetNativeFunction("AddUnitToAllStock");
    private static readonly IntPtr _addUnitToStockPtr = War3.GetNativeFunction("AddUnitToStock");
    private static readonly IntPtr _removeItemFromAllStockPtr = War3.GetNativeFunction("RemoveItemFromAllStock");
    private static readonly IntPtr _removeItemFromStockPtr = War3.GetNativeFunction("RemoveItemFromStock");
    private static readonly IntPtr _removeUnitFromAllStockPtr = War3.GetNativeFunction("RemoveUnitFromAllStock");
    private static readonly IntPtr _removeUnitFromStockPtr = War3.GetNativeFunction("RemoveUnitFromStock");
    private static readonly IntPtr _setAllItemTypeSlotsPtr = War3.GetNativeFunction("SetAllItemTypeSlots");
    private static readonly IntPtr _setAllUnitTypeSlotsPtr = War3.GetNativeFunction("SetAllUnitTypeSlots");
    private static readonly IntPtr _setItemTypeSlotsPtr = War3.GetNativeFunction("SetItemTypeSlots");
    private static readonly IntPtr _setUnitTypeSlotsPtr = War3.GetNativeFunction("SetUnitTypeSlots");
    private static readonly IntPtr _getUnitUserDataPtr = War3.GetNativeFunction("GetUnitUserData");
    private static readonly IntPtr _setUnitUserDataPtr = War3.GetNativeFunction("SetUnitUserData");
    private static readonly IntPtr _playerPtr = War3.GetNativeFunction("Player");
    private static readonly IntPtr _getLocalPlayerPtr = War3.GetNativeFunction("GetLocalPlayer");
    private static readonly IntPtr _isPlayerAllyPtr = War3.GetNativeFunction("IsPlayerAlly");
    private static readonly IntPtr _isPlayerEnemyPtr = War3.GetNativeFunction("IsPlayerEnemy");
    private static readonly IntPtr _isPlayerInForcePtr = War3.GetNativeFunction("IsPlayerInForce");
    private static readonly IntPtr _isPlayerObserverPtr = War3.GetNativeFunction("IsPlayerObserver");
    private static readonly IntPtr _isVisibleToPlayerPtr = War3.GetNativeFunction("IsVisibleToPlayer");
    private static readonly IntPtr _isLocationVisibleToPlayerPtr = War3.GetNativeFunction("IsLocationVisibleToPlayer");
    private static readonly IntPtr _isFoggedToPlayerPtr = War3.GetNativeFunction("IsFoggedToPlayer");
    private static readonly IntPtr _isLocationFoggedToPlayerPtr = War3.GetNativeFunction("IsLocationFoggedToPlayer");
    private static readonly IntPtr _isMaskedToPlayerPtr = War3.GetNativeFunction("IsMaskedToPlayer");
    private static readonly IntPtr _isLocationMaskedToPlayerPtr = War3.GetNativeFunction("IsLocationMaskedToPlayer");
    private static readonly IntPtr _getPlayerRacePtr = War3.GetNativeFunction("GetPlayerRace");
    private static readonly IntPtr _getPlayerIdPtr = War3.GetNativeFunction("GetPlayerId");
    private static readonly IntPtr _getPlayerUnitCountPtr = War3.GetNativeFunction("GetPlayerUnitCount");
    private static readonly IntPtr _getPlayerTypedUnitCountPtr = War3.GetNativeFunction("GetPlayerTypedUnitCount");
    private static readonly IntPtr _getPlayerStructureCountPtr = War3.GetNativeFunction("GetPlayerStructureCount");
    private static readonly IntPtr _getPlayerStatePtr = War3.GetNativeFunction("GetPlayerState");
    private static readonly IntPtr _getPlayerScorePtr = War3.GetNativeFunction("GetPlayerScore");
    private static readonly IntPtr _getPlayerAlliancePtr = War3.GetNativeFunction("GetPlayerAlliance");
    private static readonly IntPtr _getPlayerHandicapPtr = War3.GetNativeFunction("GetPlayerHandicap");
    private static readonly IntPtr _getPlayerHandicapXPPtr = War3.GetNativeFunction("GetPlayerHandicapXP");
    private static readonly IntPtr _setPlayerHandicapPtr = War3.GetNativeFunction("SetPlayerHandicap");
    private static readonly IntPtr _setPlayerHandicapXPPtr = War3.GetNativeFunction("SetPlayerHandicapXP");
    private static readonly IntPtr _setPlayerTechMaxAllowedPtr = War3.GetNativeFunction("SetPlayerTechMaxAllowed");
    private static readonly IntPtr _getPlayerTechMaxAllowedPtr = War3.GetNativeFunction("GetPlayerTechMaxAllowed");
    private static readonly IntPtr _addPlayerTechResearchedPtr = War3.GetNativeFunction("AddPlayerTechResearched");
    private static readonly IntPtr _setPlayerTechResearchedPtr = War3.GetNativeFunction("SetPlayerTechResearched");
    private static readonly IntPtr _getPlayerTechResearchedPtr = War3.GetNativeFunction("GetPlayerTechResearched");
    private static readonly IntPtr _getPlayerTechCountPtr = War3.GetNativeFunction("GetPlayerTechCount");
    private static readonly IntPtr _setPlayerUnitsOwnerPtr = War3.GetNativeFunction("SetPlayerUnitsOwner");
    private static readonly IntPtr _cripplePlayerPtr = War3.GetNativeFunction("CripplePlayer");
    private static readonly IntPtr _setPlayerAbilityAvailablePtr = War3.GetNativeFunction("SetPlayerAbilityAvailable");
    private static readonly IntPtr _setPlayerStatePtr = War3.GetNativeFunction("SetPlayerState");
    private static readonly IntPtr _removePlayerPtr = War3.GetNativeFunction("RemovePlayer");
    private static readonly IntPtr _cachePlayerHeroDataPtr = War3.GetNativeFunction("CachePlayerHeroData");
    private static readonly IntPtr _setFogStateRectPtr = War3.GetNativeFunction("SetFogStateRect");
    private static readonly IntPtr _setFogStateRadiusPtr = War3.GetNativeFunction("SetFogStateRadius");
    private static readonly IntPtr _setFogStateRadiusLocPtr = War3.GetNativeFunction("SetFogStateRadiusLoc");
    private static readonly IntPtr _fogMaskEnablePtr = War3.GetNativeFunction("FogMaskEnable");
    private static readonly IntPtr _isFogMaskEnabledPtr = War3.GetNativeFunction("IsFogMaskEnabled");
    private static readonly IntPtr _fogEnablePtr = War3.GetNativeFunction("FogEnable");
    private static readonly IntPtr _isFogEnabledPtr = War3.GetNativeFunction("IsFogEnabled");
    private static readonly IntPtr _createFogModifierRectPtr = War3.GetNativeFunction("CreateFogModifierRect");
    private static readonly IntPtr _createFogModifierRadiusPtr = War3.GetNativeFunction("CreateFogModifierRadius");

    private static readonly IntPtr _createFogModifierRadiusLocPtr =
        War3.GetNativeFunction("CreateFogModifierRadiusLoc");

    private static readonly IntPtr _destroyFogModifierPtr = War3.GetNativeFunction("DestroyFogModifier");
    private static readonly IntPtr _fogModifierStartPtr = War3.GetNativeFunction("FogModifierStart");
    private static readonly IntPtr _fogModifierStopPtr = War3.GetNativeFunction("FogModifierStop");
    private static readonly IntPtr _versionGetPtr = War3.GetNativeFunction("VersionGet");
    private static readonly IntPtr _versionCompatiblePtr = War3.GetNativeFunction("VersionCompatible");
    private static readonly IntPtr _versionSupportedPtr = War3.GetNativeFunction("VersionSupported");
    private static readonly IntPtr _endGamePtr = War3.GetNativeFunction("EndGame");
    private static readonly IntPtr _changeLevelPtr = War3.GetNativeFunction("ChangeLevel");
    private static readonly IntPtr _restartGamePtr = War3.GetNativeFunction("RestartGame");
    private static readonly IntPtr _reloadGamePtr = War3.GetNativeFunction("ReloadGame");
    private static readonly IntPtr _setCampaignMenuRacePtr = War3.GetNativeFunction("SetCampaignMenuRace");
    private static readonly IntPtr _setCampaignMenuRaceExPtr = War3.GetNativeFunction("SetCampaignMenuRaceEx");
    private static readonly IntPtr _forceCampaignSelectScreenPtr = War3.GetNativeFunction("ForceCampaignSelectScreen");
    private static readonly IntPtr _loadGamePtr = War3.GetNativeFunction("LoadGame");
    private static readonly IntPtr _saveGamePtr = War3.GetNativeFunction("SaveGame");
    private static readonly IntPtr _renameSaveDirectoryPtr = War3.GetNativeFunction("RenameSaveDirectory");
    private static readonly IntPtr _removeSaveDirectoryPtr = War3.GetNativeFunction("RemoveSaveDirectory");
    private static readonly IntPtr _copySaveGamePtr = War3.GetNativeFunction("CopySaveGame");
    private static readonly IntPtr _saveGameExistsPtr = War3.GetNativeFunction("SaveGameExists");
    private static readonly IntPtr _syncSelectionsPtr = War3.GetNativeFunction("SyncSelections");
    private static readonly IntPtr _setFloatGameStatePtr = War3.GetNativeFunction("SetFloatGameState");
    private static readonly IntPtr _getFloatGameStatePtr = War3.GetNativeFunction("GetFloatGameState");
    private static readonly IntPtr _setIntegerGameStatePtr = War3.GetNativeFunction("SetIntegerGameState");
    private static readonly IntPtr _getIntegerGameStatePtr = War3.GetNativeFunction("GetIntegerGameState");
    private static readonly IntPtr _setTutorialClearedPtr = War3.GetNativeFunction("SetTutorialCleared");
    private static readonly IntPtr _setMissionAvailablePtr = War3.GetNativeFunction("SetMissionAvailable");
    private static readonly IntPtr _setCampaignAvailablePtr = War3.GetNativeFunction("SetCampaignAvailable");
    private static readonly IntPtr _setOpCinematicAvailablePtr = War3.GetNativeFunction("SetOpCinematicAvailable");
    private static readonly IntPtr _setEdCinematicAvailablePtr = War3.GetNativeFunction("SetEdCinematicAvailable");
    private static readonly IntPtr _getDefaultDifficultyPtr = War3.GetNativeFunction("GetDefaultDifficulty");
    private static readonly IntPtr _setDefaultDifficultyPtr = War3.GetNativeFunction("SetDefaultDifficulty");

    private static readonly IntPtr _setCustomCampaignButtonVisiblePtr =
        War3.GetNativeFunction("SetCustomCampaignButtonVisible");

    private static readonly IntPtr _getCustomCampaignButtonVisiblePtr =
        War3.GetNativeFunction("GetCustomCampaignButtonVisible");

    private static readonly IntPtr _doNotSaveReplayPtr = War3.GetNativeFunction("DoNotSaveReplay");
    private static readonly IntPtr _dialogCreatePtr = War3.GetNativeFunction("DialogCreate");
    private static readonly IntPtr _dialogDestroyPtr = War3.GetNativeFunction("DialogDestroy");
    private static readonly IntPtr _dialogClearPtr = War3.GetNativeFunction("DialogClear");
    private static readonly IntPtr _dialogSetMessagePtr = War3.GetNativeFunction("DialogSetMessage");
    private static readonly IntPtr _dialogAddButtonPtr = War3.GetNativeFunction("DialogAddButton");
    private static readonly IntPtr _dialogAddQuitButtonPtr = War3.GetNativeFunction("DialogAddQuitButton");
    private static readonly IntPtr _dialogDisplayPtr = War3.GetNativeFunction("DialogDisplay");
    private static readonly IntPtr _reloadGameCachesFromDiskPtr = War3.GetNativeFunction("ReloadGameCachesFromDisk");
    private static readonly IntPtr _initGameCachePtr = War3.GetNativeFunction("InitGameCache");
    private static readonly IntPtr _saveGameCachePtr = War3.GetNativeFunction("SaveGameCache");
    private static readonly IntPtr _storeIntegerPtr = War3.GetNativeFunction("StoreInteger");
    private static readonly IntPtr _storeRealPtr = War3.GetNativeFunction("StoreReal");
    private static readonly IntPtr _storeBooleanPtr = War3.GetNativeFunction("StoreBoolean");
    private static readonly IntPtr _storeUnitPtr = War3.GetNativeFunction("StoreUnit");
    private static readonly IntPtr _storeStringPtr = War3.GetNativeFunction("StoreString");
    private static readonly IntPtr _syncStoredIntegerPtr = War3.GetNativeFunction("SyncStoredInteger");
    private static readonly IntPtr _syncStoredRealPtr = War3.GetNativeFunction("SyncStoredReal");
    private static readonly IntPtr _syncStoredBooleanPtr = War3.GetNativeFunction("SyncStoredBoolean");
    private static readonly IntPtr _syncStoredUnitPtr = War3.GetNativeFunction("SyncStoredUnit");
    private static readonly IntPtr _syncStoredStringPtr = War3.GetNativeFunction("SyncStoredString");
    private static readonly IntPtr _haveStoredIntegerPtr = War3.GetNativeFunction("HaveStoredInteger");
    private static readonly IntPtr _haveStoredRealPtr = War3.GetNativeFunction("HaveStoredReal");
    private static readonly IntPtr _haveStoredBooleanPtr = War3.GetNativeFunction("HaveStoredBoolean");
    private static readonly IntPtr _haveStoredUnitPtr = War3.GetNativeFunction("HaveStoredUnit");
    private static readonly IntPtr _haveStoredStringPtr = War3.GetNativeFunction("HaveStoredString");
    private static readonly IntPtr _flushGameCachePtr = War3.GetNativeFunction("FlushGameCache");
    private static readonly IntPtr _flushStoredMissionPtr = War3.GetNativeFunction("FlushStoredMission");
    private static readonly IntPtr _flushStoredIntegerPtr = War3.GetNativeFunction("FlushStoredInteger");
    private static readonly IntPtr _flushStoredRealPtr = War3.GetNativeFunction("FlushStoredReal");
    private static readonly IntPtr _flushStoredBooleanPtr = War3.GetNativeFunction("FlushStoredBoolean");
    private static readonly IntPtr _flushStoredUnitPtr = War3.GetNativeFunction("FlushStoredUnit");
    private static readonly IntPtr _flushStoredStringPtr = War3.GetNativeFunction("FlushStoredString");
    private static readonly IntPtr _getStoredIntegerPtr = War3.GetNativeFunction("GetStoredInteger");
    private static readonly IntPtr _getStoredRealPtr = War3.GetNativeFunction("GetStoredReal");
    private static readonly IntPtr _getStoredBooleanPtr = War3.GetNativeFunction("GetStoredBoolean");
    private static readonly IntPtr _getStoredStringPtr = War3.GetNativeFunction("GetStoredString");
    private static readonly IntPtr _restoreUnitPtr = War3.GetNativeFunction("RestoreUnit");
    private static readonly IntPtr _initHashtablePtr = War3.GetNativeFunction("InitHashtable");
    private static readonly IntPtr _saveIntegerPtr = War3.GetNativeFunction("SaveInteger");
    private static readonly IntPtr _saveRealPtr = War3.GetNativeFunction("SaveReal");
    private static readonly IntPtr _saveBooleanPtr = War3.GetNativeFunction("SaveBoolean");
    private static readonly IntPtr _saveStrPtr = War3.GetNativeFunction("SaveStr");
    private static readonly IntPtr _savePlayerHandlePtr = War3.GetNativeFunction("SavePlayerHandle");
    private static readonly IntPtr _saveWidgetHandlePtr = War3.GetNativeFunction("SaveWidgetHandle");
    private static readonly IntPtr _saveDestructableHandlePtr = War3.GetNativeFunction("SaveDestructableHandle");
    private static readonly IntPtr _saveItemHandlePtr = War3.GetNativeFunction("SaveItemHandle");
    private static readonly IntPtr _saveUnitHandlePtr = War3.GetNativeFunction("SaveUnitHandle");
    private static readonly IntPtr _saveAbilityHandlePtr = War3.GetNativeFunction("SaveAbilityHandle");
    private static readonly IntPtr _saveTimerHandlePtr = War3.GetNativeFunction("SaveTimerHandle");
    private static readonly IntPtr _saveTriggerHandlePtr = War3.GetNativeFunction("SaveTriggerHandle");

    private static readonly IntPtr _saveTriggerConditionHandlePtr =
        War3.GetNativeFunction("SaveTriggerConditionHandle");

    private static readonly IntPtr _saveTriggerActionHandlePtr = War3.GetNativeFunction("SaveTriggerActionHandle");
    private static readonly IntPtr _saveTriggerEventHandlePtr = War3.GetNativeFunction("SaveTriggerEventHandle");
    private static readonly IntPtr _saveForceHandlePtr = War3.GetNativeFunction("SaveForceHandle");
    private static readonly IntPtr _saveGroupHandlePtr = War3.GetNativeFunction("SaveGroupHandle");
    private static readonly IntPtr _saveLocationHandlePtr = War3.GetNativeFunction("SaveLocationHandle");
    private static readonly IntPtr _saveRectHandlePtr = War3.GetNativeFunction("SaveRectHandle");
    private static readonly IntPtr _saveBooleanExprHandlePtr = War3.GetNativeFunction("SaveBooleanExprHandle");
    private static readonly IntPtr _saveSoundHandlePtr = War3.GetNativeFunction("SaveSoundHandle");
    private static readonly IntPtr _saveEffectHandlePtr = War3.GetNativeFunction("SaveEffectHandle");
    private static readonly IntPtr _saveUnitPoolHandlePtr = War3.GetNativeFunction("SaveUnitPoolHandle");
    private static readonly IntPtr _saveItemPoolHandlePtr = War3.GetNativeFunction("SaveItemPoolHandle");
    private static readonly IntPtr _saveQuestHandlePtr = War3.GetNativeFunction("SaveQuestHandle");
    private static readonly IntPtr _saveQuestItemHandlePtr = War3.GetNativeFunction("SaveQuestItemHandle");
    private static readonly IntPtr _saveDefeatConditionHandlePtr = War3.GetNativeFunction("SaveDefeatConditionHandle");
    private static readonly IntPtr _saveTimerDialogHandlePtr = War3.GetNativeFunction("SaveTimerDialogHandle");
    private static readonly IntPtr _saveLeaderboardHandlePtr = War3.GetNativeFunction("SaveLeaderboardHandle");
    private static readonly IntPtr _saveMultiboardHandlePtr = War3.GetNativeFunction("SaveMultiboardHandle");
    private static readonly IntPtr _saveMultiboardItemHandlePtr = War3.GetNativeFunction("SaveMultiboardItemHandle");
    private static readonly IntPtr _saveTrackableHandlePtr = War3.GetNativeFunction("SaveTrackableHandle");
    private static readonly IntPtr _saveDialogHandlePtr = War3.GetNativeFunction("SaveDialogHandle");
    private static readonly IntPtr _saveButtonHandlePtr = War3.GetNativeFunction("SaveButtonHandle");
    private static readonly IntPtr _saveTextTagHandlePtr = War3.GetNativeFunction("SaveTextTagHandle");
    private static readonly IntPtr _saveLightningHandlePtr = War3.GetNativeFunction("SaveLightningHandle");
    private static readonly IntPtr _saveImageHandlePtr = War3.GetNativeFunction("SaveImageHandle");
    private static readonly IntPtr _saveUbersplatHandlePtr = War3.GetNativeFunction("SaveUbersplatHandle");
    private static readonly IntPtr _saveRegionHandlePtr = War3.GetNativeFunction("SaveRegionHandle");
    private static readonly IntPtr _saveFogStateHandlePtr = War3.GetNativeFunction("SaveFogStateHandle");
    private static readonly IntPtr _saveFogModifierHandlePtr = War3.GetNativeFunction("SaveFogModifierHandle");
    private static readonly IntPtr _saveAgentHandlePtr = War3.GetNativeFunction("SaveAgentHandle");
    private static readonly IntPtr _saveHashtableHandlePtr = War3.GetNativeFunction("SaveHashtableHandle");
    private static readonly IntPtr _loadIntegerPtr = War3.GetNativeFunction("LoadInteger");
    private static readonly IntPtr _loadRealPtr = War3.GetNativeFunction("LoadReal");
    private static readonly IntPtr _loadBooleanPtr = War3.GetNativeFunction("LoadBoolean");
    private static readonly IntPtr _loadStrPtr = War3.GetNativeFunction("LoadStr");
    private static readonly IntPtr _loadPlayerHandlePtr = War3.GetNativeFunction("LoadPlayerHandle");
    private static readonly IntPtr _loadWidgetHandlePtr = War3.GetNativeFunction("LoadWidgetHandle");
    private static readonly IntPtr _loadDestructableHandlePtr = War3.GetNativeFunction("LoadDestructableHandle");
    private static readonly IntPtr _loadItemHandlePtr = War3.GetNativeFunction("LoadItemHandle");
    private static readonly IntPtr _loadUnitHandlePtr = War3.GetNativeFunction("LoadUnitHandle");
    private static readonly IntPtr _loadAbilityHandlePtr = War3.GetNativeFunction("LoadAbilityHandle");
    private static readonly IntPtr _loadTimerHandlePtr = War3.GetNativeFunction("LoadTimerHandle");
    private static readonly IntPtr _loadTriggerHandlePtr = War3.GetNativeFunction("LoadTriggerHandle");

    private static readonly IntPtr _loadTriggerConditionHandlePtr =
        War3.GetNativeFunction("LoadTriggerConditionHandle");

    private static readonly IntPtr _loadTriggerActionHandlePtr = War3.GetNativeFunction("LoadTriggerActionHandle");
    private static readonly IntPtr _loadTriggerEventHandlePtr = War3.GetNativeFunction("LoadTriggerEventHandle");
    private static readonly IntPtr _loadForceHandlePtr = War3.GetNativeFunction("LoadForceHandle");
    private static readonly IntPtr _loadGroupHandlePtr = War3.GetNativeFunction("LoadGroupHandle");
    private static readonly IntPtr _loadLocationHandlePtr = War3.GetNativeFunction("LoadLocationHandle");
    private static readonly IntPtr _loadRectHandlePtr = War3.GetNativeFunction("LoadRectHandle");
    private static readonly IntPtr _loadBooleanExprHandlePtr = War3.GetNativeFunction("LoadBooleanExprHandle");
    private static readonly IntPtr _loadSoundHandlePtr = War3.GetNativeFunction("LoadSoundHandle");
    private static readonly IntPtr _loadEffectHandlePtr = War3.GetNativeFunction("LoadEffectHandle");
    private static readonly IntPtr _loadUnitPoolHandlePtr = War3.GetNativeFunction("LoadUnitPoolHandle");
    private static readonly IntPtr _loadItemPoolHandlePtr = War3.GetNativeFunction("LoadItemPoolHandle");
    private static readonly IntPtr _loadQuestHandlePtr = War3.GetNativeFunction("LoadQuestHandle");
    private static readonly IntPtr _loadQuestItemHandlePtr = War3.GetNativeFunction("LoadQuestItemHandle");
    private static readonly IntPtr _loadDefeatConditionHandlePtr = War3.GetNativeFunction("LoadDefeatConditionHandle");
    private static readonly IntPtr _loadTimerDialogHandlePtr = War3.GetNativeFunction("LoadTimerDialogHandle");
    private static readonly IntPtr _loadLeaderboardHandlePtr = War3.GetNativeFunction("LoadLeaderboardHandle");
    private static readonly IntPtr _loadMultiboardHandlePtr = War3.GetNativeFunction("LoadMultiboardHandle");
    private static readonly IntPtr _loadMultiboardItemHandlePtr = War3.GetNativeFunction("LoadMultiboardItemHandle");
    private static readonly IntPtr _loadTrackableHandlePtr = War3.GetNativeFunction("LoadTrackableHandle");
    private static readonly IntPtr _loadDialogHandlePtr = War3.GetNativeFunction("LoadDialogHandle");
    private static readonly IntPtr _loadButtonHandlePtr = War3.GetNativeFunction("LoadButtonHandle");
    private static readonly IntPtr _loadTextTagHandlePtr = War3.GetNativeFunction("LoadTextTagHandle");
    private static readonly IntPtr _loadLightningHandlePtr = War3.GetNativeFunction("LoadLightningHandle");
    private static readonly IntPtr _loadImageHandlePtr = War3.GetNativeFunction("LoadImageHandle");
    private static readonly IntPtr _loadUbersplatHandlePtr = War3.GetNativeFunction("LoadUbersplatHandle");
    private static readonly IntPtr _loadRegionHandlePtr = War3.GetNativeFunction("LoadRegionHandle");
    private static readonly IntPtr _loadFogStateHandlePtr = War3.GetNativeFunction("LoadFogStateHandle");
    private static readonly IntPtr _loadFogModifierHandlePtr = War3.GetNativeFunction("LoadFogModifierHandle");
    private static readonly IntPtr _loadHashtableHandlePtr = War3.GetNativeFunction("LoadHashtableHandle");
    private static readonly IntPtr _haveSavedIntegerPtr = War3.GetNativeFunction("HaveSavedInteger");
    private static readonly IntPtr _haveSavedRealPtr = War3.GetNativeFunction("HaveSavedReal");
    private static readonly IntPtr _haveSavedBooleanPtr = War3.GetNativeFunction("HaveSavedBoolean");
    private static readonly IntPtr _haveSavedStringPtr = War3.GetNativeFunction("HaveSavedString");
    private static readonly IntPtr _haveSavedHandlePtr = War3.GetNativeFunction("HaveSavedHandle");
    private static readonly IntPtr _removeSavedIntegerPtr = War3.GetNativeFunction("RemoveSavedInteger");
    private static readonly IntPtr _removeSavedRealPtr = War3.GetNativeFunction("RemoveSavedReal");
    private static readonly IntPtr _removeSavedBooleanPtr = War3.GetNativeFunction("RemoveSavedBoolean");
    private static readonly IntPtr _removeSavedStringPtr = War3.GetNativeFunction("RemoveSavedString");
    private static readonly IntPtr _removeSavedHandlePtr = War3.GetNativeFunction("RemoveSavedHandle");
    private static readonly IntPtr _flushParentHashtablePtr = War3.GetNativeFunction("FlushParentHashtable");
    private static readonly IntPtr _flushChildHashtablePtr = War3.GetNativeFunction("FlushChildHashtable");
    private static readonly IntPtr _getRandomIntPtr = War3.GetNativeFunction("GetRandomInt");
    private static readonly IntPtr _getRandomRealPtr = War3.GetNativeFunction("GetRandomReal");
    private static readonly IntPtr _createUnitPoolPtr = War3.GetNativeFunction("CreateUnitPool");
    private static readonly IntPtr _destroyUnitPoolPtr = War3.GetNativeFunction("DestroyUnitPool");
    private static readonly IntPtr _unitPoolAddUnitTypePtr = War3.GetNativeFunction("UnitPoolAddUnitType");
    private static readonly IntPtr _unitPoolRemoveUnitTypePtr = War3.GetNativeFunction("UnitPoolRemoveUnitType");
    private static readonly IntPtr _placeRandomUnitPtr = War3.GetNativeFunction("PlaceRandomUnit");
    private static readonly IntPtr _createItemPoolPtr = War3.GetNativeFunction("CreateItemPool");
    private static readonly IntPtr _destroyItemPoolPtr = War3.GetNativeFunction("DestroyItemPool");
    private static readonly IntPtr _itemPoolAddItemTypePtr = War3.GetNativeFunction("ItemPoolAddItemType");
    private static readonly IntPtr _itemPoolRemoveItemTypePtr = War3.GetNativeFunction("ItemPoolRemoveItemType");
    private static readonly IntPtr _placeRandomItemPtr = War3.GetNativeFunction("PlaceRandomItem");
    private static readonly IntPtr _chooseRandomCreepPtr = War3.GetNativeFunction("ChooseRandomCreep");
    private static readonly IntPtr _chooseRandomNPBuildingPtr = War3.GetNativeFunction("ChooseRandomNPBuilding");
    private static readonly IntPtr _chooseRandomItemPtr = War3.GetNativeFunction("ChooseRandomItem");
    private static readonly IntPtr _chooseRandomItemExPtr = War3.GetNativeFunction("ChooseRandomItemEx");
    private static readonly IntPtr _setRandomSeedPtr = War3.GetNativeFunction("SetRandomSeed");
    private static readonly IntPtr _setTerrainFogPtr = War3.GetNativeFunction("SetTerrainFog");
    private static readonly IntPtr _resetTerrainFogPtr = War3.GetNativeFunction("ResetTerrainFog");
    private static readonly IntPtr _setUnitFogPtr = War3.GetNativeFunction("SetUnitFog");
    private static readonly IntPtr _setTerrainFogExPtr = War3.GetNativeFunction("SetTerrainFogEx");
    private static readonly IntPtr _displayTextToPlayerPtr = War3.GetNativeFunction("DisplayTextToPlayer");
    private static readonly IntPtr _displayTimedTextToPlayerPtr = War3.GetNativeFunction("DisplayTimedTextToPlayer");

    private static readonly IntPtr _displayTimedTextFromPlayerPtr =
        War3.GetNativeFunction("DisplayTimedTextFromPlayer");

    private static readonly IntPtr _clearTextMessagesPtr = War3.GetNativeFunction("ClearTextMessages");
    private static readonly IntPtr _setDayNightModelsPtr = War3.GetNativeFunction("SetDayNightModels");
    private static readonly IntPtr _setSkyModelPtr = War3.GetNativeFunction("SetSkyModel");
    private static readonly IntPtr _enableUserControlPtr = War3.GetNativeFunction("EnableUserControl");
    private static readonly IntPtr _enableUserUIPtr = War3.GetNativeFunction("EnableUserUI");
    private static readonly IntPtr _suspendTimeOfDayPtr = War3.GetNativeFunction("SuspendTimeOfDay");
    private static readonly IntPtr _setTimeOfDayScalePtr = War3.GetNativeFunction("SetTimeOfDayScale");
    private static readonly IntPtr _getTimeOfDayScalePtr = War3.GetNativeFunction("GetTimeOfDayScale");
    private static readonly IntPtr _showInterfacePtr = War3.GetNativeFunction("ShowInterface");
    private static readonly IntPtr _pauseGamePtr = War3.GetNativeFunction("PauseGame");
    private static readonly IntPtr _unitAddIndicatorPtr = War3.GetNativeFunction("UnitAddIndicator");
    private static readonly IntPtr _addIndicatorPtr = War3.GetNativeFunction("AddIndicator");
    private static readonly IntPtr _pingMinimapPtr = War3.GetNativeFunction("PingMinimap");
    private static readonly IntPtr _pingMinimapExPtr = War3.GetNativeFunction("PingMinimapEx");
    private static readonly IntPtr _enableOcclusionPtr = War3.GetNativeFunction("EnableOcclusion");
    private static readonly IntPtr _setIntroShotTextPtr = War3.GetNativeFunction("SetIntroShotText");
    private static readonly IntPtr _setIntroShotModelPtr = War3.GetNativeFunction("SetIntroShotModel");
    private static readonly IntPtr _enableWorldFogBoundaryPtr = War3.GetNativeFunction("EnableWorldFogBoundary");
    private static readonly IntPtr _playModelCinematicPtr = War3.GetNativeFunction("PlayModelCinematic");
    private static readonly IntPtr _playCinematicPtr = War3.GetNativeFunction("PlayCinematic");
    private static readonly IntPtr _forceUIKeyPtr = War3.GetNativeFunction("ForceUIKey");
    private static readonly IntPtr _forceUICancelPtr = War3.GetNativeFunction("ForceUICancel");
    private static readonly IntPtr _displayLoadDialogPtr = War3.GetNativeFunction("DisplayLoadDialog");
    private static readonly IntPtr _setAltMinimapIconPtr = War3.GetNativeFunction("SetAltMinimapIcon");
    private static readonly IntPtr _disableRestartMissionPtr = War3.GetNativeFunction("DisableRestartMission");
    private static readonly IntPtr _createTextTagPtr = War3.GetNativeFunction("CreateTextTag");
    private static readonly IntPtr _destroyTextTagPtr = War3.GetNativeFunction("DestroyTextTag");
    private static readonly IntPtr _setTextTagTextPtr = War3.GetNativeFunction("SetTextTagText");
    private static readonly IntPtr _setTextTagPosPtr = War3.GetNativeFunction("SetTextTagPos");
    private static readonly IntPtr _setTextTagPosUnitPtr = War3.GetNativeFunction("SetTextTagPosUnit");
    private static readonly IntPtr _setTextTagColorPtr = War3.GetNativeFunction("SetTextTagColor");
    private static readonly IntPtr _setTextTagVelocityPtr = War3.GetNativeFunction("SetTextTagVelocity");
    private static readonly IntPtr _setTextTagVisibilityPtr = War3.GetNativeFunction("SetTextTagVisibility");
    private static readonly IntPtr _setTextTagSuspendedPtr = War3.GetNativeFunction("SetTextTagSuspended");
    private static readonly IntPtr _setTextTagPermanentPtr = War3.GetNativeFunction("SetTextTagPermanent");
    private static readonly IntPtr _setTextTagAgePtr = War3.GetNativeFunction("SetTextTagAge");
    private static readonly IntPtr _setTextTagLifespanPtr = War3.GetNativeFunction("SetTextTagLifespan");
    private static readonly IntPtr _setTextTagFadepointPtr = War3.GetNativeFunction("SetTextTagFadepoint");

    private static readonly IntPtr _setReservedLocalHeroButtonsPtr =
        War3.GetNativeFunction("SetReservedLocalHeroButtons");

    private static readonly IntPtr _getAllyColorFilterStatePtr = War3.GetNativeFunction("GetAllyColorFilterState");
    private static readonly IntPtr _setAllyColorFilterStatePtr = War3.GetNativeFunction("SetAllyColorFilterState");
    private static readonly IntPtr _getCreepCampFilterStatePtr = War3.GetNativeFunction("GetCreepCampFilterState");
    private static readonly IntPtr _setCreepCampFilterStatePtr = War3.GetNativeFunction("SetCreepCampFilterState");

    private static readonly IntPtr _enableMinimapFilterButtonsPtr =
        War3.GetNativeFunction("EnableMinimapFilterButtons");

    private static readonly IntPtr _enableDragSelectPtr = War3.GetNativeFunction("EnableDragSelect");
    private static readonly IntPtr _enablePreSelectPtr = War3.GetNativeFunction("EnablePreSelect");
    private static readonly IntPtr _enableSelectPtr = War3.GetNativeFunction("EnableSelect");
    private static readonly IntPtr _createTrackablePtr = War3.GetNativeFunction("CreateTrackable");
    private static readonly IntPtr _createQuestPtr = War3.GetNativeFunction("CreateQuest");
    private static readonly IntPtr _destroyQuestPtr = War3.GetNativeFunction("DestroyQuest");
    private static readonly IntPtr _questSetTitlePtr = War3.GetNativeFunction("QuestSetTitle");
    private static readonly IntPtr _questSetDescriptionPtr = War3.GetNativeFunction("QuestSetDescription");
    private static readonly IntPtr _questSetIconPathPtr = War3.GetNativeFunction("QuestSetIconPath");
    private static readonly IntPtr _questSetRequiredPtr = War3.GetNativeFunction("QuestSetRequired");
    private static readonly IntPtr _questSetCompletedPtr = War3.GetNativeFunction("QuestSetCompleted");
    private static readonly IntPtr _questSetDiscoveredPtr = War3.GetNativeFunction("QuestSetDiscovered");
    private static readonly IntPtr _questSetFailedPtr = War3.GetNativeFunction("QuestSetFailed");
    private static readonly IntPtr _questSetEnabledPtr = War3.GetNativeFunction("QuestSetEnabled");
    private static readonly IntPtr _isQuestRequiredPtr = War3.GetNativeFunction("IsQuestRequired");
    private static readonly IntPtr _isQuestCompletedPtr = War3.GetNativeFunction("IsQuestCompleted");
    private static readonly IntPtr _isQuestDiscoveredPtr = War3.GetNativeFunction("IsQuestDiscovered");
    private static readonly IntPtr _isQuestFailedPtr = War3.GetNativeFunction("IsQuestFailed");
    private static readonly IntPtr _isQuestEnabledPtr = War3.GetNativeFunction("IsQuestEnabled");
    private static readonly IntPtr _questCreateItemPtr = War3.GetNativeFunction("QuestCreateItem");
    private static readonly IntPtr _questItemSetDescriptionPtr = War3.GetNativeFunction("QuestItemSetDescription");
    private static readonly IntPtr _questItemSetCompletedPtr = War3.GetNativeFunction("QuestItemSetCompleted");
    private static readonly IntPtr _isQuestItemCompletedPtr = War3.GetNativeFunction("IsQuestItemCompleted");
    private static readonly IntPtr _createDefeatConditionPtr = War3.GetNativeFunction("CreateDefeatCondition");
    private static readonly IntPtr _destroyDefeatConditionPtr = War3.GetNativeFunction("DestroyDefeatCondition");

    private static readonly IntPtr _defeatConditionSetDescriptionPtr =
        War3.GetNativeFunction("DefeatConditionSetDescription");

    private static readonly IntPtr _flashQuestDialogButtonPtr = War3.GetNativeFunction("FlashQuestDialogButton");
    private static readonly IntPtr _forceQuestDialogUpdatePtr = War3.GetNativeFunction("ForceQuestDialogUpdate");
    private static readonly IntPtr _createTimerDialogPtr = War3.GetNativeFunction("CreateTimerDialog");
    private static readonly IntPtr _destroyTimerDialogPtr = War3.GetNativeFunction("DestroyTimerDialog");
    private static readonly IntPtr _timerDialogSetTitlePtr = War3.GetNativeFunction("TimerDialogSetTitle");
    private static readonly IntPtr _timerDialogSetTitleColorPtr = War3.GetNativeFunction("TimerDialogSetTitleColor");
    private static readonly IntPtr _timerDialogSetTimeColorPtr = War3.GetNativeFunction("TimerDialogSetTimeColor");
    private static readonly IntPtr _timerDialogSetSpeedPtr = War3.GetNativeFunction("TimerDialogSetSpeed");
    private static readonly IntPtr _timerDialogDisplayPtr = War3.GetNativeFunction("TimerDialogDisplay");
    private static readonly IntPtr _isTimerDialogDisplayedPtr = War3.GetNativeFunction("IsTimerDialogDisplayed");

    private static readonly IntPtr _timerDialogSetRealTimeRemainingPtr =
        War3.GetNativeFunction("TimerDialogSetRealTimeRemaining");

    private static readonly IntPtr _createLeaderboardPtr = War3.GetNativeFunction("CreateLeaderboard");
    private static readonly IntPtr _destroyLeaderboardPtr = War3.GetNativeFunction("DestroyLeaderboard");
    private static readonly IntPtr _leaderboardDisplayPtr = War3.GetNativeFunction("LeaderboardDisplay");
    private static readonly IntPtr _isLeaderboardDisplayedPtr = War3.GetNativeFunction("IsLeaderboardDisplayed");
    private static readonly IntPtr _leaderboardGetItemCountPtr = War3.GetNativeFunction("LeaderboardGetItemCount");

    private static readonly IntPtr _leaderboardSetSizeByItemCountPtr =
        War3.GetNativeFunction("LeaderboardSetSizeByItemCount");

    private static readonly IntPtr _leaderboardAddItemPtr = War3.GetNativeFunction("LeaderboardAddItem");
    private static readonly IntPtr _leaderboardRemoveItemPtr = War3.GetNativeFunction("LeaderboardRemoveItem");

    private static readonly IntPtr _leaderboardRemovePlayerItemPtr =
        War3.GetNativeFunction("LeaderboardRemovePlayerItem");

    private static readonly IntPtr _leaderboardClearPtr = War3.GetNativeFunction("LeaderboardClear");

    private static readonly IntPtr _leaderboardSortItemsByValuePtr =
        War3.GetNativeFunction("LeaderboardSortItemsByValue");

    private static readonly IntPtr _leaderboardSortItemsByPlayerPtr =
        War3.GetNativeFunction("LeaderboardSortItemsByPlayer");

    private static readonly IntPtr _leaderboardSortItemsByLabelPtr =
        War3.GetNativeFunction("LeaderboardSortItemsByLabel");

    private static readonly IntPtr _leaderboardHasPlayerItemPtr = War3.GetNativeFunction("LeaderboardHasPlayerItem");
    private static readonly IntPtr _leaderboardGetPlayerIndexPtr = War3.GetNativeFunction("LeaderboardGetPlayerIndex");
    private static readonly IntPtr _leaderboardSetLabelPtr = War3.GetNativeFunction("LeaderboardSetLabel");
    private static readonly IntPtr _leaderboardGetLabelTextPtr = War3.GetNativeFunction("LeaderboardGetLabelText");
    private static readonly IntPtr _playerSetLeaderboardPtr = War3.GetNativeFunction("PlayerSetLeaderboard");
    private static readonly IntPtr _playerGetLeaderboardPtr = War3.GetNativeFunction("PlayerGetLeaderboard");
    private static readonly IntPtr _leaderboardSetLabelColorPtr = War3.GetNativeFunction("LeaderboardSetLabelColor");
    private static readonly IntPtr _leaderboardSetValueColorPtr = War3.GetNativeFunction("LeaderboardSetValueColor");
    private static readonly IntPtr _leaderboardSetStylePtr = War3.GetNativeFunction("LeaderboardSetStyle");
    private static readonly IntPtr _leaderboardSetItemValuePtr = War3.GetNativeFunction("LeaderboardSetItemValue");
    private static readonly IntPtr _leaderboardSetItemLabelPtr = War3.GetNativeFunction("LeaderboardSetItemLabel");
    private static readonly IntPtr _leaderboardSetItemStylePtr = War3.GetNativeFunction("LeaderboardSetItemStyle");

    private static readonly IntPtr _leaderboardSetItemLabelColorPtr =
        War3.GetNativeFunction("LeaderboardSetItemLabelColor");

    private static readonly IntPtr _leaderboardSetItemValueColorPtr =
        War3.GetNativeFunction("LeaderboardSetItemValueColor");

    private static readonly IntPtr _createMultiboardPtr = War3.GetNativeFunction("CreateMultiboard");
    private static readonly IntPtr _destroyMultiboardPtr = War3.GetNativeFunction("DestroyMultiboard");
    private static readonly IntPtr _multiboardDisplayPtr = War3.GetNativeFunction("MultiboardDisplay");
    private static readonly IntPtr _isMultiboardDisplayedPtr = War3.GetNativeFunction("IsMultiboardDisplayed");
    private static readonly IntPtr _multiboardMinimizePtr = War3.GetNativeFunction("MultiboardMinimize");
    private static readonly IntPtr _isMultiboardMinimizedPtr = War3.GetNativeFunction("IsMultiboardMinimized");
    private static readonly IntPtr _multiboardClearPtr = War3.GetNativeFunction("MultiboardClear");
    private static readonly IntPtr _multiboardSetTitleTextPtr = War3.GetNativeFunction("MultiboardSetTitleText");
    private static readonly IntPtr _multiboardGetTitleTextPtr = War3.GetNativeFunction("MultiboardGetTitleText");

    private static readonly IntPtr _multiboardSetTitleTextColorPtr =
        War3.GetNativeFunction("MultiboardSetTitleTextColor");

    private static readonly IntPtr _multiboardGetRowCountPtr = War3.GetNativeFunction("MultiboardGetRowCount");
    private static readonly IntPtr _multiboardGetColumnCountPtr = War3.GetNativeFunction("MultiboardGetColumnCount");
    private static readonly IntPtr _multiboardSetColumnCountPtr = War3.GetNativeFunction("MultiboardSetColumnCount");
    private static readonly IntPtr _multiboardSetRowCountPtr = War3.GetNativeFunction("MultiboardSetRowCount");
    private static readonly IntPtr _multiboardSetItemsStylePtr = War3.GetNativeFunction("MultiboardSetItemsStyle");
    private static readonly IntPtr _multiboardSetItemsValuePtr = War3.GetNativeFunction("MultiboardSetItemsValue");

    private static readonly IntPtr _multiboardSetItemsValueColorPtr =
        War3.GetNativeFunction("MultiboardSetItemsValueColor");

    private static readonly IntPtr _multiboardSetItemsWidthPtr = War3.GetNativeFunction("MultiboardSetItemsWidth");
    private static readonly IntPtr _multiboardSetItemsIconPtr = War3.GetNativeFunction("MultiboardSetItemsIcon");
    private static readonly IntPtr _multiboardGetItemPtr = War3.GetNativeFunction("MultiboardGetItem");
    private static readonly IntPtr _multiboardReleaseItemPtr = War3.GetNativeFunction("MultiboardReleaseItem");
    private static readonly IntPtr _multiboardSetItemStylePtr = War3.GetNativeFunction("MultiboardSetItemStyle");
    private static readonly IntPtr _multiboardSetItemValuePtr = War3.GetNativeFunction("MultiboardSetItemValue");

    private static readonly IntPtr _multiboardSetItemValueColorPtr =
        War3.GetNativeFunction("MultiboardSetItemValueColor");

    private static readonly IntPtr _multiboardSetItemWidthPtr = War3.GetNativeFunction("MultiboardSetItemWidth");
    private static readonly IntPtr _multiboardSetItemIconPtr = War3.GetNativeFunction("MultiboardSetItemIcon");
    private static readonly IntPtr _multiboardSuppressDisplayPtr = War3.GetNativeFunction("MultiboardSuppressDisplay");
    private static readonly IntPtr _setCameraPositionPtr = War3.GetNativeFunction("SetCameraPosition");
    private static readonly IntPtr _setCameraQuickPositionPtr = War3.GetNativeFunction("SetCameraQuickPosition");
    private static readonly IntPtr _setCameraBoundsPtr = War3.GetNativeFunction("SetCameraBounds");
    private static readonly IntPtr _stopCameraPtr = War3.GetNativeFunction("StopCamera");
    private static readonly IntPtr _resetToGameCameraPtr = War3.GetNativeFunction("ResetToGameCamera");
    private static readonly IntPtr _panCameraToPtr = War3.GetNativeFunction("PanCameraTo");
    private static readonly IntPtr _panCameraToTimedPtr = War3.GetNativeFunction("PanCameraToTimed");
    private static readonly IntPtr _panCameraToWithZPtr = War3.GetNativeFunction("PanCameraToWithZ");
    private static readonly IntPtr _panCameraToTimedWithZPtr = War3.GetNativeFunction("PanCameraToTimedWithZ");
    private static readonly IntPtr _setCinematicCameraPtr = War3.GetNativeFunction("SetCinematicCamera");
    private static readonly IntPtr _setCameraRotateModePtr = War3.GetNativeFunction("SetCameraRotateMode");
    private static readonly IntPtr _setCameraFieldPtr = War3.GetNativeFunction("SetCameraField");
    private static readonly IntPtr _adjustCameraFieldPtr = War3.GetNativeFunction("AdjustCameraField");
    private static readonly IntPtr _setCameraTargetControllerPtr = War3.GetNativeFunction("SetCameraTargetController");
    private static readonly IntPtr _setCameraOrientControllerPtr = War3.GetNativeFunction("SetCameraOrientController");
    private static readonly IntPtr _createCameraSetupPtr = War3.GetNativeFunction("CreateCameraSetup");
    private static readonly IntPtr _cameraSetupSetFieldPtr = War3.GetNativeFunction("CameraSetupSetField");
    private static readonly IntPtr _cameraSetupGetFieldPtr = War3.GetNativeFunction("CameraSetupGetField");

    private static readonly IntPtr _cameraSetupSetDestPositionPtr =
        War3.GetNativeFunction("CameraSetupSetDestPosition");

    private static readonly IntPtr _cameraSetupGetDestPositionLocPtr =
        War3.GetNativeFunction("CameraSetupGetDestPositionLoc");

    private static readonly IntPtr _cameraSetupGetDestPositionXPtr =
        War3.GetNativeFunction("CameraSetupGetDestPositionX");

    private static readonly IntPtr _cameraSetupGetDestPositionYPtr =
        War3.GetNativeFunction("CameraSetupGetDestPositionY");

    private static readonly IntPtr _cameraSetupApplyPtr = War3.GetNativeFunction("CameraSetupApply");
    private static readonly IntPtr _cameraSetupApplyWithZPtr = War3.GetNativeFunction("CameraSetupApplyWithZ");

    private static readonly IntPtr _cameraSetupApplyForceDurationPtr =
        War3.GetNativeFunction("CameraSetupApplyForceDuration");

    private static readonly IntPtr _cameraSetupApplyForceDurationWithZPtr =
        War3.GetNativeFunction("CameraSetupApplyForceDurationWithZ");

    private static readonly IntPtr _cameraSetTargetNoisePtr = War3.GetNativeFunction("CameraSetTargetNoise");
    private static readonly IntPtr _cameraSetSourceNoisePtr = War3.GetNativeFunction("CameraSetSourceNoise");
    private static readonly IntPtr _cameraSetTargetNoiseExPtr = War3.GetNativeFunction("CameraSetTargetNoiseEx");
    private static readonly IntPtr _cameraSetSourceNoiseExPtr = War3.GetNativeFunction("CameraSetSourceNoiseEx");
    private static readonly IntPtr _cameraSetSmoothingFactorPtr = War3.GetNativeFunction("CameraSetSmoothingFactor");
    private static readonly IntPtr _setCineFilterTexturePtr = War3.GetNativeFunction("SetCineFilterTexture");
    private static readonly IntPtr _setCineFilterBlendModePtr = War3.GetNativeFunction("SetCineFilterBlendMode");
    private static readonly IntPtr _setCineFilterTexMapFlagsPtr = War3.GetNativeFunction("SetCineFilterTexMapFlags");
    private static readonly IntPtr _setCineFilterStartUVPtr = War3.GetNativeFunction("SetCineFilterStartUV");
    private static readonly IntPtr _setCineFilterEndUVPtr = War3.GetNativeFunction("SetCineFilterEndUV");
    private static readonly IntPtr _setCineFilterStartColorPtr = War3.GetNativeFunction("SetCineFilterStartColor");
    private static readonly IntPtr _setCineFilterEndColorPtr = War3.GetNativeFunction("SetCineFilterEndColor");
    private static readonly IntPtr _setCineFilterDurationPtr = War3.GetNativeFunction("SetCineFilterDuration");
    private static readonly IntPtr _displayCineFilterPtr = War3.GetNativeFunction("DisplayCineFilter");
    private static readonly IntPtr _isCineFilterDisplayedPtr = War3.GetNativeFunction("IsCineFilterDisplayed");
    private static readonly IntPtr _setCinematicScenePtr = War3.GetNativeFunction("SetCinematicScene");
    private static readonly IntPtr _endCinematicScenePtr = War3.GetNativeFunction("EndCinematicScene");
    private static readonly IntPtr _forceCinematicSubtitlesPtr = War3.GetNativeFunction("ForceCinematicSubtitles");
    private static readonly IntPtr _getCameraMarginPtr = War3.GetNativeFunction("GetCameraMargin");
    private static readonly IntPtr _getCameraBoundMinXPtr = War3.GetNativeFunction("GetCameraBoundMinX");
    private static readonly IntPtr _getCameraBoundMinYPtr = War3.GetNativeFunction("GetCameraBoundMinY");
    private static readonly IntPtr _getCameraBoundMaxXPtr = War3.GetNativeFunction("GetCameraBoundMaxX");
    private static readonly IntPtr _getCameraBoundMaxYPtr = War3.GetNativeFunction("GetCameraBoundMaxY");
    private static readonly IntPtr _getCameraFieldPtr = War3.GetNativeFunction("GetCameraField");
    private static readonly IntPtr _getCameraTargetPositionXPtr = War3.GetNativeFunction("GetCameraTargetPositionX");
    private static readonly IntPtr _getCameraTargetPositionYPtr = War3.GetNativeFunction("GetCameraTargetPositionY");
    private static readonly IntPtr _getCameraTargetPositionZPtr = War3.GetNativeFunction("GetCameraTargetPositionZ");

    private static readonly IntPtr _getCameraTargetPositionLocPtr =
        War3.GetNativeFunction("GetCameraTargetPositionLoc");

    private static readonly IntPtr _getCameraEyePositionXPtr = War3.GetNativeFunction("GetCameraEyePositionX");
    private static readonly IntPtr _getCameraEyePositionYPtr = War3.GetNativeFunction("GetCameraEyePositionY");
    private static readonly IntPtr _getCameraEyePositionZPtr = War3.GetNativeFunction("GetCameraEyePositionZ");
    private static readonly IntPtr _getCameraEyePositionLocPtr = War3.GetNativeFunction("GetCameraEyePositionLoc");
    private static readonly IntPtr _newSoundEnvironmentPtr = War3.GetNativeFunction("NewSoundEnvironment");
    private static readonly IntPtr _createSoundPtr = War3.GetNativeFunction("CreateSound");

    private static readonly IntPtr _createSoundFilenameWithLabelPtr =
        War3.GetNativeFunction("CreateSoundFilenameWithLabel");

    private static readonly IntPtr _createSoundFromLabelPtr = War3.GetNativeFunction("CreateSoundFromLabel");
    private static readonly IntPtr _createMIDISoundPtr = War3.GetNativeFunction("CreateMIDISound");
    private static readonly IntPtr _setSoundParamsFromLabelPtr = War3.GetNativeFunction("SetSoundParamsFromLabel");
    private static readonly IntPtr _setSoundDistanceCutoffPtr = War3.GetNativeFunction("SetSoundDistanceCutoff");
    private static readonly IntPtr _setSoundChannelPtr = War3.GetNativeFunction("SetSoundChannel");
    private static readonly IntPtr _setSoundVolumePtr = War3.GetNativeFunction("SetSoundVolume");
    private static readonly IntPtr _setSoundPitchPtr = War3.GetNativeFunction("SetSoundPitch");
    private static readonly IntPtr _setSoundPlayPositionPtr = War3.GetNativeFunction("SetSoundPlayPosition");
    private static readonly IntPtr _setSoundDistancesPtr = War3.GetNativeFunction("SetSoundDistances");
    private static readonly IntPtr _setSoundConeAnglesPtr = War3.GetNativeFunction("SetSoundConeAngles");
    private static readonly IntPtr _setSoundConeOrientationPtr = War3.GetNativeFunction("SetSoundConeOrientation");
    private static readonly IntPtr _setSoundPositionPtr = War3.GetNativeFunction("SetSoundPosition");
    private static readonly IntPtr _setSoundVelocityPtr = War3.GetNativeFunction("SetSoundVelocity");
    private static readonly IntPtr _attachSoundToUnitPtr = War3.GetNativeFunction("AttachSoundToUnit");
    private static readonly IntPtr _startSoundPtr = War3.GetNativeFunction("StartSound");
    private static readonly IntPtr _stopSoundPtr = War3.GetNativeFunction("StopSound");
    private static readonly IntPtr _killSoundWhenDonePtr = War3.GetNativeFunction("KillSoundWhenDone");
    private static readonly IntPtr _setMapMusicPtr = War3.GetNativeFunction("SetMapMusic");
    private static readonly IntPtr _clearMapMusicPtr = War3.GetNativeFunction("ClearMapMusic");
    private static readonly IntPtr _playMusicPtr = War3.GetNativeFunction("PlayMusic");
    private static readonly IntPtr _playMusicExPtr = War3.GetNativeFunction("PlayMusicEx");
    private static readonly IntPtr _stopMusicPtr = War3.GetNativeFunction("StopMusic");
    private static readonly IntPtr _resumeMusicPtr = War3.GetNativeFunction("ResumeMusic");
    private static readonly IntPtr _playThematicMusicPtr = War3.GetNativeFunction("PlayThematicMusic");
    private static readonly IntPtr _playThematicMusicExPtr = War3.GetNativeFunction("PlayThematicMusicEx");
    private static readonly IntPtr _endThematicMusicPtr = War3.GetNativeFunction("EndThematicMusic");
    private static readonly IntPtr _setMusicVolumePtr = War3.GetNativeFunction("SetMusicVolume");
    private static readonly IntPtr _setMusicPlayPositionPtr = War3.GetNativeFunction("SetMusicPlayPosition");

    private static readonly IntPtr _setThematicMusicPlayPositionPtr =
        War3.GetNativeFunction("SetThematicMusicPlayPosition");

    private static readonly IntPtr _setSoundDurationPtr = War3.GetNativeFunction("SetSoundDuration");
    private static readonly IntPtr _getSoundDurationPtr = War3.GetNativeFunction("GetSoundDuration");
    private static readonly IntPtr _getSoundFileDurationPtr = War3.GetNativeFunction("GetSoundFileDuration");
    private static readonly IntPtr _volumeGroupSetVolumePtr = War3.GetNativeFunction("VolumeGroupSetVolume");
    private static readonly IntPtr _volumeGroupResetPtr = War3.GetNativeFunction("VolumeGroupReset");
    private static readonly IntPtr _getSoundIsPlayingPtr = War3.GetNativeFunction("GetSoundIsPlaying");
    private static readonly IntPtr _getSoundIsLoadingPtr = War3.GetNativeFunction("GetSoundIsLoading");
    private static readonly IntPtr _registerStackedSoundPtr = War3.GetNativeFunction("RegisterStackedSound");
    private static readonly IntPtr _unregisterStackedSoundPtr = War3.GetNativeFunction("UnregisterStackedSound");
    private static readonly IntPtr _addWeatherEffectPtr = War3.GetNativeFunction("AddWeatherEffect");
    private static readonly IntPtr _removeWeatherEffectPtr = War3.GetNativeFunction("RemoveWeatherEffect");
    private static readonly IntPtr _enableWeatherEffectPtr = War3.GetNativeFunction("EnableWeatherEffect");
    private static readonly IntPtr _terrainDeformCraterPtr = War3.GetNativeFunction("TerrainDeformCrater");
    private static readonly IntPtr _terrainDeformRipplePtr = War3.GetNativeFunction("TerrainDeformRipple");
    private static readonly IntPtr _terrainDeformWavePtr = War3.GetNativeFunction("TerrainDeformWave");
    private static readonly IntPtr _terrainDeformRandomPtr = War3.GetNativeFunction("TerrainDeformRandom");
    private static readonly IntPtr _terrainDeformStopPtr = War3.GetNativeFunction("TerrainDeformStop");
    private static readonly IntPtr _terrainDeformStopAllPtr = War3.GetNativeFunction("TerrainDeformStopAll");
    private static readonly IntPtr _addSpecialEffectPtr = War3.GetNativeFunction("AddSpecialEffect");
    private static readonly IntPtr _addSpecialEffectLocPtr = War3.GetNativeFunction("AddSpecialEffectLoc");
    private static readonly IntPtr _addSpecialEffectTargetPtr = War3.GetNativeFunction("AddSpecialEffectTarget");
    private static readonly IntPtr _destroyEffectPtr = War3.GetNativeFunction("DestroyEffect");
    private static readonly IntPtr _addSpellEffectPtr = War3.GetNativeFunction("AddSpellEffect");
    private static readonly IntPtr _addSpellEffectLocPtr = War3.GetNativeFunction("AddSpellEffectLoc");
    private static readonly IntPtr _addSpellEffectByIdPtr = War3.GetNativeFunction("AddSpellEffectById");
    private static readonly IntPtr _addSpellEffectByIdLocPtr = War3.GetNativeFunction("AddSpellEffectByIdLoc");
    private static readonly IntPtr _addSpellEffectTargetPtr = War3.GetNativeFunction("AddSpellEffectTarget");
    private static readonly IntPtr _addSpellEffectTargetByIdPtr = War3.GetNativeFunction("AddSpellEffectTargetById");
    private static readonly IntPtr _addLightningPtr = War3.GetNativeFunction("AddLightning");
    private static readonly IntPtr _addLightningExPtr = War3.GetNativeFunction("AddLightningEx");
    private static readonly IntPtr _destroyLightningPtr = War3.GetNativeFunction("DestroyLightning");
    private static readonly IntPtr _moveLightningPtr = War3.GetNativeFunction("MoveLightning");
    private static readonly IntPtr _moveLightningExPtr = War3.GetNativeFunction("MoveLightningEx");
    private static readonly IntPtr _getLightningColorAPtr = War3.GetNativeFunction("GetLightningColorA");
    private static readonly IntPtr _getLightningColorRPtr = War3.GetNativeFunction("GetLightningColorR");
    private static readonly IntPtr _getLightningColorGPtr = War3.GetNativeFunction("GetLightningColorG");
    private static readonly IntPtr _getLightningColorBPtr = War3.GetNativeFunction("GetLightningColorB");
    private static readonly IntPtr _setLightningColorPtr = War3.GetNativeFunction("SetLightningColor");
    private static readonly IntPtr _getAbilityEffectPtr = War3.GetNativeFunction("GetAbilityEffect");
    private static readonly IntPtr _getAbilityEffectByIdPtr = War3.GetNativeFunction("GetAbilityEffectById");
    private static readonly IntPtr _getAbilitySoundPtr = War3.GetNativeFunction("GetAbilitySound");
    private static readonly IntPtr _getAbilitySoundByIdPtr = War3.GetNativeFunction("GetAbilitySoundById");
    private static readonly IntPtr _getTerrainCliffLevelPtr = War3.GetNativeFunction("GetTerrainCliffLevel");
    private static readonly IntPtr _setWaterBaseColorPtr = War3.GetNativeFunction("SetWaterBaseColor");
    private static readonly IntPtr _setWaterDeformsPtr = War3.GetNativeFunction("SetWaterDeforms");
    private static readonly IntPtr _getTerrainTypePtr = War3.GetNativeFunction("GetTerrainType");
    private static readonly IntPtr _getTerrainVariancePtr = War3.GetNativeFunction("GetTerrainVariance");
    private static readonly IntPtr _setTerrainTypePtr = War3.GetNativeFunction("SetTerrainType");
    private static readonly IntPtr _isTerrainPathablePtr = War3.GetNativeFunction("IsTerrainPathable");
    private static readonly IntPtr _setTerrainPathablePtr = War3.GetNativeFunction("SetTerrainPathable");
    private static readonly IntPtr _createImagePtr = War3.GetNativeFunction("CreateImage");
    private static readonly IntPtr _destroyImagePtr = War3.GetNativeFunction("DestroyImage");
    private static readonly IntPtr _showImagePtr = War3.GetNativeFunction("ShowImage");
    private static readonly IntPtr _setImageConstantHeightPtr = War3.GetNativeFunction("SetImageConstantHeight");
    private static readonly IntPtr _setImagePositionPtr = War3.GetNativeFunction("SetImagePosition");
    private static readonly IntPtr _setImageColorPtr = War3.GetNativeFunction("SetImageColor");
    private static readonly IntPtr _setImageRenderPtr = War3.GetNativeFunction("SetImageRender");
    private static readonly IntPtr _setImageRenderAlwaysPtr = War3.GetNativeFunction("SetImageRenderAlways");
    private static readonly IntPtr _setImageAboveWaterPtr = War3.GetNativeFunction("SetImageAboveWater");
    private static readonly IntPtr _setImageTypePtr = War3.GetNativeFunction("SetImageType");
    private static readonly IntPtr _createUbersplatPtr = War3.GetNativeFunction("CreateUbersplat");
    private static readonly IntPtr _destroyUbersplatPtr = War3.GetNativeFunction("DestroyUbersplat");
    private static readonly IntPtr _resetUbersplatPtr = War3.GetNativeFunction("ResetUbersplat");
    private static readonly IntPtr _finishUbersplatPtr = War3.GetNativeFunction("FinishUbersplat");
    private static readonly IntPtr _showUbersplatPtr = War3.GetNativeFunction("ShowUbersplat");
    private static readonly IntPtr _setUbersplatRenderPtr = War3.GetNativeFunction("SetUbersplatRender");
    private static readonly IntPtr _setUbersplatRenderAlwaysPtr = War3.GetNativeFunction("SetUbersplatRenderAlways");
    private static readonly IntPtr _setBlightPtr = War3.GetNativeFunction("SetBlight");
    private static readonly IntPtr _setBlightRectPtr = War3.GetNativeFunction("SetBlightRect");
    private static readonly IntPtr _setBlightPointPtr = War3.GetNativeFunction("SetBlightPoint");
    private static readonly IntPtr _setBlightLocPtr = War3.GetNativeFunction("SetBlightLoc");
    private static readonly IntPtr _createBlightedGoldminePtr = War3.GetNativeFunction("CreateBlightedGoldmine");
    private static readonly IntPtr _isPointBlightedPtr = War3.GetNativeFunction("IsPointBlighted");
    private static readonly IntPtr _setDoodadAnimationPtr = War3.GetNativeFunction("SetDoodadAnimation");
    private static readonly IntPtr _setDoodadAnimationRectPtr = War3.GetNativeFunction("SetDoodadAnimationRect");
    private static readonly IntPtr _startMeleeAIPtr = War3.GetNativeFunction("StartMeleeAI");
    private static readonly IntPtr _startCampaignAIPtr = War3.GetNativeFunction("StartCampaignAI");
    private static readonly IntPtr _commandAIPtr = War3.GetNativeFunction("CommandAI");
    private static readonly IntPtr _pauseCompAIPtr = War3.GetNativeFunction("PauseCompAI");
    private static readonly IntPtr _getAIDifficultyPtr = War3.GetNativeFunction("GetAIDifficulty");
    private static readonly IntPtr _removeGuardPositionPtr = War3.GetNativeFunction("RemoveGuardPosition");
    private static readonly IntPtr _recycleGuardPositionPtr = War3.GetNativeFunction("RecycleGuardPosition");
    private static readonly IntPtr _removeAllGuardPositionsPtr = War3.GetNativeFunction("RemoveAllGuardPositions");
    private static readonly IntPtr _cheatPtr = War3.GetNativeFunction("Cheat");
    private static readonly IntPtr _isNoVictoryCheatPtr = War3.GetNativeFunction("IsNoVictoryCheat");
    private static readonly IntPtr _isNoDefeatCheatPtr = War3.GetNativeFunction("IsNoDefeatCheat");
    private static readonly IntPtr _preloadPtr = War3.GetNativeFunction("Preload");
    private static readonly IntPtr _preloadEndPtr = War3.GetNativeFunction("PreloadEnd");
    private static readonly IntPtr _preloadStartPtr = War3.GetNativeFunction("PreloadStart");
    private static readonly IntPtr _preloadRefreshPtr = War3.GetNativeFunction("PreloadRefresh");
    private static readonly IntPtr _preloadEndExPtr = War3.GetNativeFunction("PreloadEndEx");
    private static readonly IntPtr _preloadGenClearPtr = War3.GetNativeFunction("PreloadGenClear");
    private static readonly IntPtr _preloadGenStartPtr = War3.GetNativeFunction("PreloadGenStart");
    private static readonly IntPtr _preloadGenEndPtr = War3.GetNativeFunction("PreloadGenEnd");
    private static readonly IntPtr _preloaderPtr = War3.GetNativeFunction("Preloader");

    public static JRace ConvertRace(int i)
    {
        var handle = War3.CallNative<int>(_convertRacePtr, i);
        return new JRace(handle);
    }

    public static JAllianceType ConvertAllianceType(int i)
    {
        var handle = War3.CallNative<int>(_convertAllianceTypePtr, i);
        return new JAllianceType(handle);
    }

    public static JRacePreference ConvertRacePref(int i)
    {
        var handle = War3.CallNative<int>(_convertRacePrefPtr, i);
        return new JRacePreference(handle);
    }

    public static JIGameState ConvertIGameState(int i)
    {
        var handle = War3.CallNative<int>(_convertIGameStatePtr, i);
        return new JIGameState(handle);
    }

    public static JFGameState ConvertFGameState(int i)
    {
        var handle = War3.CallNative<int>(_convertFGameStatePtr, i);
        return new JFGameState(handle);
    }

    public static JPlayerState ConvertPlayerState(int i)
    {
        var handle = War3.CallNative<int>(_convertPlayerStatePtr, i);
        return new JPlayerState(handle);
    }

    public static JPlayerScore ConvertPlayerScore(int i)
    {
        var handle = War3.CallNative<int>(_convertPlayerScorePtr, i);
        return new JPlayerScore(handle);
    }

    public static JPlayerGameResult ConvertPlayerGameResult(int i)
    {
        var handle = War3.CallNative<int>(_convertPlayerGameResultPtr, i);
        return new JPlayerGameResult(handle);
    }

    public static JUnitState ConvertUnitState(int i)
    {
        var handle = War3.CallNative<int>(_convertUnitStatePtr, i);
        return new JUnitState(handle);
    }

    public static JAiDifficulty ConvertAIDifficulty(int i)
    {
        var handle = War3.CallNative<int>(_convertAIDifficultyPtr, i);
        return new JAiDifficulty(handle);
    }

    public static JGameEvent ConvertGameEvent(int i)
    {
        var handle = War3.CallNative<int>(_convertGameEventPtr, i);
        return new JGameEvent(handle);
    }

    public static JPlayerEvent ConvertPlayerEvent(int i)
    {
        var handle = War3.CallNative<int>(_convertPlayerEventPtr, i);
        return new JPlayerEvent(handle);
    }

    public static JPlayerUnitEvent ConvertPlayerUnitEvent(int i)
    {
        var handle = War3.CallNative<int>(_convertPlayerUnitEventPtr, i);
        return new JPlayerUnitEvent(handle);
    }

    public static JWidgetEvent ConvertWidgetEvent(int i)
    {
        var handle = War3.CallNative<int>(_convertWidgetEventPtr, i);
        return new JWidgetEvent(handle);
    }

    public static JDialogEvent ConvertDialogEvent(int i)
    {
        var handle = War3.CallNative<int>(_convertDialogEventPtr, i);
        return new JDialogEvent(handle);
    }

    public static JUnitEvent ConvertUnitEvent(int i)
    {
        var handle = War3.CallNative<int>(_convertUnitEventPtr, i);
        return new JUnitEvent(handle);
    }

    public static JLimitOp ConvertLimitOp(int i)
    {
        var handle = War3.CallNative<int>(_convertLimitOpPtr, i);
        return new JLimitOp(handle);
    }

    public static JUnitType ConvertUnitType(int i)
    {
        var handle = War3.CallNative<int>(_convertUnitTypePtr, i);
        return new JUnitType(handle);
    }

    public static JGameSpeed ConvertGameSpeed(int i)
    {
        var handle = War3.CallNative<int>(_convertGameSpeedPtr, i);
        return new JGameSpeed(handle);
    }

    public static JPlacement ConvertPlacement(int i)
    {
        var handle = War3.CallNative<int>(_convertPlacementPtr, i);
        return new JPlacement(handle);
    }

    public static JStartLocPrio ConvertStartLocPrio(int i)
    {
        var handle = War3.CallNative<int>(_convertStartLocPrioPtr, i);
        return new JStartLocPrio(handle);
    }

    public static JGameDifficulty ConvertGameDifficulty(int i)
    {
        var handle = War3.CallNative<int>(_convertGameDifficultyPtr, i);
        return new JGameDifficulty(handle);
    }

    public static JGameType ConvertGameType(int i)
    {
        var handle = War3.CallNative<int>(_convertGameTypePtr, i);
        return new JGameType(handle);
    }

    public static JMapFlag ConvertMapFlag(int i)
    {
        var handle = War3.CallNative<int>(_convertMapFlagPtr, i);
        return new JMapFlag(handle);
    }

    public static JMapVisibility ConvertMapVisibility(int i)
    {
        var handle = War3.CallNative<int>(_convertMapVisibilityPtr, i);
        return new JMapVisibility(handle);
    }

    public static JMapSetting ConvertMapSetting(int i)
    {
        var handle = War3.CallNative<int>(_convertMapSettingPtr, i);
        return new JMapSetting(handle);
    }

    public static JMapDensity ConvertMapDensity(int i)
    {
        var handle = War3.CallNative<int>(_convertMapDensityPtr, i);
        return new JMapDensity(handle);
    }

    public static JMapControl ConvertMapControl(int i)
    {
        var handle = War3.CallNative<int>(_convertMapControlPtr, i);
        return new JMapControl(handle);
    }

    public static JPlayerColor ConvertPlayerColor(int i)
    {
        var handle = War3.CallNative<int>(_convertPlayerColorPtr, i);
        return new JPlayerColor(handle);
    }

    public static JPlayerSlotState ConvertPlayerSlotState(int i)
    {
        var handle = War3.CallNative<int>(_convertPlayerSlotStatePtr, i);
        return new JPlayerSlotState(handle);
    }

    public static JVolumeGroup ConvertVolumeGroup(int i)
    {
        var handle = War3.CallNative<int>(_convertVolumeGroupPtr, i);
        return new JVolumeGroup(handle);
    }

    public static JCameraField ConvertCameraField(int i)
    {
        var handle = War3.CallNative<int>(_convertCameraFieldPtr, i);
        return new JCameraField(handle);
    }

    public static JBlendMode ConvertBlendMode(int i)
    {
        var handle = War3.CallNative<int>(_convertBlendModePtr, i);
        return new JBlendMode(handle);
    }

    public static JRarityControl ConvertRarityControl(int i)
    {
        var handle = War3.CallNative<int>(_convertRarityControlPtr, i);
        return new JRarityControl(handle);
    }

    public static JTexMapFlags ConvertTexMapFlags(int i)
    {
        var handle = War3.CallNative<int>(_convertTexMapFlagsPtr, i);
        return new JTexMapFlags(handle);
    }

    public static JFogState ConvertFogState(int i)
    {
        var handle = War3.CallNative<int>(_convertFogStatePtr, i);
        return new JFogState(handle);
    }

    public static JEffectType ConvertEffectType(int i)
    {
        var handle = War3.CallNative<int>(_convertEffectTypePtr, i);
        return new JEffectType(handle);
    }

    public static JVersion ConvertVersion(int i)
    {
        var handle = War3.CallNative<int>(_convertVersionPtr, i);
        return new JVersion(handle);
    }

    public static JItemType ConvertItemType(int i)
    {
        var handle = War3.CallNative<int>(_convertItemTypePtr, i);
        return new JItemType(handle);
    }

    public static JAttackType ConvertAttackType(int i)
    {
        var handle = War3.CallNative<int>(_convertAttackTypePtr, i);
        return new JAttackType(handle);
    }

    public static JDamageType ConvertDamageType(int i)
    {
        var handle = War3.CallNative<int>(_convertDamageTypePtr, i);
        return new JDamageType(handle);
    }

    public static JWeaponType ConvertWeaponType(int i)
    {
        var handle = War3.CallNative<int>(_convertWeaponTypePtr, i);
        return new JWeaponType(handle);
    }

    public static JSoundType ConvertSoundType(int i)
    {
        var handle = War3.CallNative<int>(_convertSoundTypePtr, i);
        return new JSoundType(handle);
    }

    public static JPathingType ConvertPathingType(int i)
    {
        var handle = War3.CallNative<int>(_convertPathingTypePtr, i);
        return new JPathingType(handle);
    }

    public static int OrderId(string orderIdString)
    {
        return War3.CallNative<int>(_orderIdPtr, orderIdString);
    }

    public static string OrderId2String(int orderId)
    {
        return War3.CallNative<string>(_orderId2StringPtr, orderId);
    }

    public static int UnitId(string unitIdString)
    {
        return War3.CallNative<int>(_unitIdPtr, unitIdString);
    }

    public static string UnitId2String(int unitId)
    {
        return War3.CallNative<string>(_unitId2StringPtr, unitId);
    }

    public static int AbilityId(string abilityIdString)
    {
        return War3.CallNative<int>(_abilityIdPtr, abilityIdString);
    }

    public static string AbilityId2String(int abilityId)
    {
        return War3.CallNative<string>(_abilityId2StringPtr, abilityId);
    }


    /// title = "物体名称 [C]"
    /// description = "${物体ID} 的名称"
    /// comment = "如'A01Z',物体编辑器中物体的名字"
    public static string GetObjectName(int objectId)
    {
        return War3.CallNative<string>(_getObjectNamePtr, objectId);
    }


    /// title = "转换角度为弧度"
    /// description = "转换角度 ${Degrees} 为弧度"
    /// comment = ""
    public static float Deg2Rad(float degrees)
    {
        return War3.CallNative<float>(_deg2RadPtr, degrees);
    }


    /// title = "转换弧度为角度"
    /// description = "转换弧度 ${Radians} 为角度"
    /// comment = ""
    public static float Rad2Deg(float radians)
    {
        return War3.CallNative<float>(_rad2DegPtr, radians);
    }


    /// title = "正弦(弧度) [R]"
    /// description = "Sin(${Angle})"
    /// comment = "采用弧度制计算. "
    public static float Sin(float radians)
    {
        return War3.CallNative<float>(_sinPtr, radians);
    }


    /// title = "余弦(弧度) [R]"
    /// description = "Cos(${Angle})"
    /// comment = "采用弧度制计算. "
    public static float Cos(float radians)
    {
        return War3.CallNative<float>(_cosPtr, radians);
    }


    /// title = "正切(弧度) [R]"
    /// description = "Tan(${Angle})"
    /// comment = "采用弧度制计算. "
    public static float Tan(float radians)
    {
        return War3.CallNative<float>(_tanPtr, radians);
    }


    /// title = "反正弦(弧度) [R]"
    /// description = "Asin(${数值})"
    /// comment = "采用弧度制计算. 返回弧度取值-π/2 — π/2. "
    public static float Asin(float y)
    {
        return War3.CallNative<float>(_asinPtr, y);
    }


    /// title = "反余弦(弧度) [R]"
    /// description = "Acos(${数值})"
    /// comment = "采用弧度制计算. 返回弧度取值0 — π. "
    public static float Acos(float x)
    {
        return War3.CallNative<float>(_acosPtr, x);
    }


    /// title = "反正切(弧度) [R]"
    /// description = "Atan(${数值})"
    /// comment = "采用弧度制计算. 返回弧度取值-π/2 — π/2. "
    public static float Atan(float x)
    {
        return War3.CallNative<float>(_atanPtr, x);
    }


    /// title = "反正切(Y:X)(弧度) [R]"
    /// description = "Atan(${Y} : ${X})"
    /// comment = "采用弧度制计算. 返回弧度取值-π/2 — π/2. "
    public static float Atan2(float y, float x)
    {
        return War3.CallNative<float>(_atan2Ptr, y, x);
    }


    /// title = "平方根"
    /// description = "${实数} 的平方根"
    /// comment = ""
    public static float SquareRoot(float x)
    {
        return War3.CallNative<float>(_squareRootPtr, x);
    }


    /// title = "幂运算"
    /// description = "${实数} 的 ${实数} 次幂"
    /// comment = ""
    public static float Pow(float x, float power)
    {
        return War3.CallNative<float>(_powPtr, x, power);
    }


    /// title = "转换整数为实数"
    /// description = "转换 ${Integer} 为实数"
    /// comment = ""
    public static float I2R(int i)
    {
        return War3.CallNative<float>(_i2RPtr, i);
    }


    /// title = "转换实数为整数"
    /// description = "转换 ${Real} 为整数"
    /// comment = "舍弃小数部分."
    public static int R2I(float r)
    {
        return War3.CallNative<int>(_r2IPtr, r);
    }


    /// title = "转换整数为字符串"
    /// description = "转换 ${Integer} 为字符串"
    /// comment = ""
    public static string I2S(int i)
    {
        return War3.CallNative<string>(_i2SPtr, i);
    }


    /// title = "转换实数为字符串"
    /// description = "转换 ${Real} 为字符串"
    /// comment = ""
    public static string R2S(float r)
    {
        return War3.CallNative<string>(_r2SPtr, r);
    }


    /// title = "格式转换实数为字符串"
    /// description = "转换 ${Real} 为字符串,最小宽度: ${Width} ,小数位数: ${Precision}"
    /// comment = "如: 转换(1.234, 7, 2)后为''   1.23''. 转换(1.234, 2, 5)后为''1.23400''."
    public static string R2SW(float r, int width, int precision)
    {
        return War3.CallNative<string>(_r2SWPtr, r, width, precision);
    }


    /// title = "转换字符串为整数"
    /// description = "转换 ${String} 为整数"
    /// comment = ""
    public static int S2I(string s)
    {
        return War3.CallNative<int>(_s2IPtr, s);
    }


    /// title = "转换字符串为实数"
    /// description = "转换 ${String} 为实数"
    /// comment = ""
    public static float S2R(string s)
    {
        return War3.CallNative<float>(_s2RPtr, s);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 获取对象的h2i值 [C]"
    /// description = "转换 ${Handle} 为整数"
    /// comment = "创建一个对应该handle的临时密钥,可以在哈希表中作为索引号使用.当该handle被彻底销毁时,密钥会被回收."
    public static int GetHandleId(JHandle h)
    {
        return War3.CallNative<int>(_getHandleIdPtr, h.Handle);
    }


    /// title = "截取字符串 [R]"
    /// description = "截取 ${字符串} 的 ${Start} - ${End} 字节部分(不包括首字节)"
    /// comment = "例: 截取''Grunts stink''的2 - 4字节部分 = ''un''."
    public static string SubString(string source, int start, int end)
    {
        return War3.CallNative<string>(_subStringPtr, source, start, end);
    }


    /// title = "字符串长度"
    /// description = "${String}的长度"
    /// comment = ""
    public static int StringLength(string s)
    {
        return War3.CallNative<int>(_stringLengthPtr, s);
    }


    /// title = "大小写转换"
    /// description = "转换 ${字符串} 为 ${Lower/Upper Case} 形式"
    /// comment = ""
    public static string StringCase(string source, bool upper)
    {
        return War3.CallNative<string>(_stringCasePtr, source, upper);
    }


    /// title = "获取字符串的哈希值"
    /// description = "(${String})的哈希值"
    /// comment = "获取一个对应该字符串的密钥，不同的字符串的密钥基本不可能相同，也很难找到两个不同的字符串他们有着相同的密钥。可以用于制作密码等功能。"
    public static int StringHash(string s)
    {
        return War3.CallNative<int>(_stringHashPtr, s);
    }


    /// title = "本地字符串 [R]"
    /// description = "本地字符串: ${文字}"
    /// comment = "获取ui\\framedef\\globalstrings.fdf中定义的字符串."
    public static string GetLocalizedString(string source)
    {
        return War3.CallNative<string>(_getLocalizedStringPtr, source);
    }


    /// title = "本地热键 "
    /// description = "本地热键: ${文字}"
    /// comment = "获取ui\\miscui.txt中定义的热键."
    public static int GetLocalizedHotkey(string source)
    {
        return War3.CallNative<int>(_getLocalizedHotkeyPtr, source);
    }

    public static void SetMapName(string name)
    {
        War3.CallNative(_setMapNamePtr, name);
    }

    public static void SetMapDescription(string description)
    {
        War3.CallNative(_setMapDescriptionPtr, description);
    }

    public static void SetTeams(int teamcount)
    {
        War3.CallNative(_setTeamsPtr, teamcount);
    }

    public static void SetPlayers(int playercount)
    {
        War3.CallNative(_setPlayersPtr, playercount);
    }

    public static void DefineStartLocation(int whichStartLoc, float x, float y)
    {
        War3.CallNative(_defineStartLocationPtr, whichStartLoc, x, y);
    }

    public static void DefineStartLocationLoc(int whichStartLoc, JLocation whichLocation)
    {
        War3.CallNative(_defineStartLocationLocPtr, whichStartLoc, whichLocation.Handle);
    }

    public static void SetStartLocPrioCount(int whichStartLoc, int prioSlotCount)
    {
        War3.CallNative(_setStartLocPrioCountPtr, whichStartLoc, prioSlotCount);
    }

    public static void SetStartLocPrio(int whichStartLoc, int prioSlotIndex, int otherStartLocIndex,
        JStartLocPrio priority)
    {
        War3.CallNative(_setStartLocPrioPtr, whichStartLoc, prioSlotIndex, otherStartLocIndex, priority.Handle);
    }

    public static int GetStartLocPrioSlot(int whichStartLoc, int prioSlotIndex)
    {
        return War3.CallNative<int>(_getStartLocPrioSlotPtr, whichStartLoc, prioSlotIndex);
    }

    public static JStartLocPrio GetStartLocPrio(int whichStartLoc, int prioSlotIndex)
    {
        var handle = War3.CallNative<int>(_getStartLocPrioPtr, whichStartLoc, prioSlotIndex);
        return new JStartLocPrio(handle);
    }

    public static void SetGameTypeSupported(JGameType whichGameType, bool value)
    {
        War3.CallNative(_setGameTypeSupportedPtr, whichGameType.Handle, value);
    }


    /// title = "设置地图参数"
    /// description = "设置 ${Map Flag} 为 ${On/Off}"
    /// comment = ""
    public static void SetMapFlag(JMapFlag whichMapFlag, bool value)
    {
        War3.CallNative(_setMapFlagPtr, whichMapFlag.Handle, value);
    }

    public static void SetGamePlacement(JPlacement whichPlacementType)
    {
        War3.CallNative(_setGamePlacementPtr, whichPlacementType.Handle);
    }


    /// title = "设定游戏速度"
    /// description = "设定游戏速度为 ${Speed}"
    /// comment = "你可以通过'游戏 - 锁定游戏速度'动作来锁定游戏速度."
    public static void SetGameSpeed(JGameSpeed whichspeed)
    {
        War3.CallNative(_setGameSpeedPtr, whichspeed.Handle);
    }


    /// title = "设置游戏难度 [R]"
    /// description = "设置当前游戏难度为 ${GameDifficulty}"
    /// comment = "游戏难度只是作为运行AI的一个参考值,没有AI的地图该功能无用."
    public static void SetGameDifficulty(JGameDifficulty whichdifficulty)
    {
        War3.CallNative(_setGameDifficultyPtr, whichdifficulty.Handle);
    }

    public static void SetResourceDensity(JMapDensity whichdensity)
    {
        War3.CallNative(_setResourceDensityPtr, whichdensity.Handle);
    }

    public static void SetCreatureDensity(JMapDensity whichdensity)
    {
        War3.CallNative(_setCreatureDensityPtr, whichdensity.Handle);
    }


    /// title = "队伍数量"
    /// description = "队伍数量"
    /// comment = ""
    public static int GetTeams()
    {
        return War3.CallNative<int>(_getTeamsPtr);
    }


    /// title = "玩家数量"
    /// description = "玩家数量"
    /// comment = "地图编辑器中开启的玩家数量(1-12)."
    public static int GetPlayers()
    {
        return War3.CallNative<int>(_getPlayersPtr);
    }

    public static bool IsGameTypeSupported(JGameType whichGameType)
    {
        return War3.CallNative<bool>(_isGameTypeSupportedPtr, whichGameType.Handle);
    }

    public static JGameType GetGameTypeSelected()
    {
        var handle = War3.CallNative<int>(_getGameTypeSelectedPtr);
        return new JGameType(handle);
    }


    /// title = "地图参数设置"
    /// description = "${Map Flag} 已设置"
    /// comment = ""
    public static bool IsMapFlagSet(JMapFlag whichMapFlag)
    {
        return War3.CallNative<bool>(_isMapFlagSetPtr, whichMapFlag.Handle);
    }

    public static JPlacement GetGamePlacement()
    {
        var handle = War3.CallNative<int>(_getGamePlacementPtr);
        return new JPlacement(handle);
    }


    /// title = "当前游戏速度"
    /// description = "当前游戏速度"
    /// comment = ""
    public static JGameSpeed GetGameSpeed()
    {
        var handle = War3.CallNative<int>(_getGameSpeedPtr);
        return new JGameSpeed(handle);
    }


    /// title = "当前游戏难度"
    /// description = "当前游戏难度"
    /// comment = ""
    public static JGameDifficulty GetGameDifficulty()
    {
        var handle = War3.CallNative<int>(_getGameDifficultyPtr);
        return new JGameDifficulty(handle);
    }

    public static JMapDensity GetResourceDensity()
    {
        var handle = War3.CallNative<int>(_getResourceDensityPtr);
        return new JMapDensity(handle);
    }

    public static JMapDensity GetCreatureDensity()
    {
        var handle = War3.CallNative<int>(_getCreatureDensityPtr);
        return new JMapDensity(handle);
    }

    public static float GetStartLocationX(int whichStartLocation)
    {
        return War3.CallNative<float>(_getStartLocationXPtr, whichStartLocation);
    }

    public static float GetStartLocationY(int whichStartLocation)
    {
        return War3.CallNative<float>(_getStartLocationYPtr, whichStartLocation);
    }

    public static JLocation GetStartLocationLoc(int whichStartLocation)
    {
        var handle = War3.CallNative<int>(_getStartLocationLocPtr, whichStartLocation);
        return new JLocation(handle);
    }


    /// title = "设置玩家队伍"
    /// description = "设置 ${Player} 的队伍为 ${队伍ID}"
    /// comment = ""
    public static void SetPlayerTeam(JPlayer whichPlayer, int whichTeam)
    {
        War3.CallNative(_setPlayerTeamPtr, whichPlayer.Handle, whichTeam);
    }

    public static void SetPlayerStartLocation(JPlayer whichPlayer, int startLocIndex)
    {
        War3.CallNative(_setPlayerStartLocationPtr, whichPlayer.Handle, startLocIndex);
    }

    public static void ForcePlayerStartLocation(JPlayer whichPlayer, int startLocIndex)
    {
        War3.CallNative(_forcePlayerStartLocationPtr, whichPlayer.Handle, startLocIndex);
    }


    /// title = "改变玩家颜色 [R]"
    /// description = "将 ${Player} 的玩家颜色改为 ${Color}"
    /// comment = "不改变现有单位的颜色."
    public static void SetPlayerColor(JPlayer whichPlayer, JPlayerColor color)
    {
        War3.CallNative(_setPlayerColorPtr, whichPlayer.Handle, color.Handle);
    }


    /// title = "设置联盟状态(指定项目) [R]"
    /// description = "命令 ${Player} 对 ${Player} 设置 ${Alliance Type} ${On/Off}"
    /// comment = "注意:可以对玩家自己设置联盟状态. 可用来实现一些特殊效果."
    public static void SetPlayerAlliance(JPlayer sourcePlayer, JPlayer otherPlayer, JAllianceType whichAllianceSetting,
        bool value)
    {
        War3.CallNative(_setPlayerAlliancePtr, sourcePlayer.Handle, otherPlayer.Handle, whichAllianceSetting.Handle,
            value);
    }


    /// title = "设置税率 [R]"
    /// description = "设置 ${Player} 交纳给 ${Player} 的 ${Resource} 所得税为 ${Rate} %"
    /// comment = "缴纳所得税所损失的资源可以通过'玩家得分'的'税务损失的黄金/木材'来获取. 所得税最高为100%. 且玩家1对玩家2和玩家3都交纳80%所得税.则玩家1采集黄金时将给玩家2 8黄金,玩家3 2黄金."
    public static void SetPlayerTaxRate(JPlayer sourcePlayer, JPlayer otherPlayer, JPlayerState whichResource, int rate)
    {
        War3.CallNative(_setPlayerTaxRatePtr, sourcePlayer.Handle, otherPlayer.Handle, whichResource.Handle, rate);
    }

    public static void SetPlayerRacePreference(JPlayer whichPlayer, JRacePreference whichRacePreference)
    {
        War3.CallNative(_setPlayerRacePreferencePtr, whichPlayer.Handle, whichRacePreference.Handle);
    }

    public static void SetPlayerRaceSelectable(JPlayer whichPlayer, bool value)
    {
        War3.CallNative(_setPlayerRaceSelectablePtr, whichPlayer.Handle, value);
    }

    public static void SetPlayerController(JPlayer whichPlayer, JMapControl controlType)
    {
        War3.CallNative(_setPlayerControllerPtr, whichPlayer.Handle, controlType.Handle);
    }


    /// title = "更改名字"
    /// description = "更改 ${Player} 的名字为 ${文字}"
    /// comment = ""
    public static void SetPlayerName(JPlayer whichPlayer, string name)
    {
        War3.CallNative(_setPlayerNamePtr, whichPlayer.Handle, name);
    }


    /// title = "显示/隐藏计分屏显示 [R]"
    /// description = "设置 ${Player} ${Show/Hide} 在计分屏的显示."
    /// comment = ""
    public static void SetPlayerOnScoreScreen(JPlayer whichPlayer, bool flag)
    {
        War3.CallNative(_setPlayerOnScoreScreenPtr, whichPlayer.Handle, flag);
    }


    /// title = "玩家队伍"
    /// description = "${Player} 所属队伍编号"
    /// comment = ""
    public static int GetPlayerTeam(JPlayer whichPlayer)
    {
        return War3.CallNative<int>(_getPlayerTeamPtr, whichPlayer.Handle);
    }

    public static int GetPlayerStartLocation(JPlayer whichPlayer)
    {
        return War3.CallNative<int>(_getPlayerStartLocationPtr, whichPlayer.Handle);
    }


    /// title = "玩家颜色"
    /// description = "${Player} 的颜色"
    /// comment = ""
    public static JPlayerColor GetPlayerColor(JPlayer whichPlayer)
    {
        var handle = War3.CallNative<int>(_getPlayerColorPtr, whichPlayer.Handle);
        return new JPlayerColor(handle);
    }

    public static bool GetPlayerSelectable(JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_getPlayerSelectablePtr, whichPlayer.Handle);
    }


    /// title = "玩家控制者"
    /// description = "${Player} 的控制者"
    /// comment = ""
    public static JMapControl GetPlayerController(JPlayer whichPlayer)
    {
        var handle = War3.CallNative<int>(_getPlayerControllerPtr, whichPlayer.Handle);
        return new JMapControl(handle);
    }


    /// title = "玩家游戏状态"
    /// description = "${Player} 的游戏状态"
    /// comment = ""
    public static JPlayerSlotState GetPlayerSlotState(JPlayer whichPlayer)
    {
        var handle = War3.CallNative<int>(_getPlayerSlotStatePtr, whichPlayer.Handle);
        return new JPlayerSlotState(handle);
    }


    /// title = "玩家税率 [R]"
    /// description = "${Player} 需要交纳给 ${Player} 的 ${Resource} 所得税"
    /// comment = "所得税取值范围0-100."
    public static int GetPlayerTaxRate(JPlayer sourcePlayer, JPlayer otherPlayer, JPlayerState whichResource)
    {
        return War3.CallNative<int>(_getPlayerTaxRatePtr, sourcePlayer.Handle, otherPlayer.Handle,
            whichResource.Handle);
    }


    /// title = "玩家的种族选择"
    /// description = "${Player} 选择了 ${RacePreference}"
    /// comment = ""
    public static bool IsPlayerRacePrefSet(JPlayer whichPlayer, JRacePreference pref)
    {
        return War3.CallNative<bool>(_isPlayerRacePrefSetPtr, whichPlayer.Handle, pref.Handle);
    }


    /// title = "玩家名字"
    /// description = "${Player} 的名字"
    /// comment = ""
    public static string GetPlayerName(JPlayer whichPlayer)
    {
        return War3.CallNative<string>(_getPlayerNamePtr, whichPlayer.Handle);
    }


    /// title = "新建计时器 [R]"
    /// description = "新建的计时器"
    /// comment = "会创建计时器."
    public static JTimer CreateTimer()
    {
        var timerHandle = War3.CallNative<int>(_createTimerPtr);
        return new JTimer(timerHandle);
    }


    /// title = "删除计时器 [R]"
    /// description = "删除 ${计时器}"
    /// comment = "一般来说,计时器并不需要删除.只为某些有特别需求的用户提供."
    public static void DestroyTimer(JTimer whichTimer)
    {
        War3.CallNative(_destroyTimerPtr, whichTimer.Handle);
    }


    /// title = "运行计时器 [C]"
    /// description = "运行 ${计时器}，周期: ${时间} 秒，模式: ${模式}，运行函数: ${函数}"
    /// comment = "等同于TimerStart"
    public static int TimerStart(JTimer whichTimer, float timeout, bool periodic, Action handlerFunc)
    {
        return War3.CallNative<int>(_timerStartPtr, whichTimer.Handle, timeout, periodic, handlerFunc);
    }


    /// title = "逝去时间"
    /// description = "${计时器} 的逝去时间"
    /// comment = ""
    public static float TimerGetElapsed(JTimer whichTimer)
    {
        return War3.CallNative<float>(_timerGetElapsedPtr, whichTimer.Handle);
    }


    /// title = "剩余时间"
    /// description = "${计时器} 的剩余时间"
    /// comment = ""
    public static float TimerGetRemaining(JTimer whichTimer)
    {
        return War3.CallNative<float>(_timerGetRemainingPtr, whichTimer.Handle);
    }


    /// title = "设置时间"
    /// description = "${计时器} 设置的时间"
    /// comment = ""
    public static float TimerGetTimeout(JTimer whichTimer)
    {
        return War3.CallNative<float>(_timerGetTimeoutPtr, whichTimer.Handle);
    }


    /// title = "暂停计时器 [R]"
    /// description = "暂停 ${计时器}"
    /// comment = ""
    public static void PauseTimer(JTimer whichTimer)
    {
        War3.CallNative(_pauseTimerPtr, whichTimer.Handle);
    }


    /// title = "恢复计时器 [R]"
    /// description = "恢复 ${计时器}"
    /// comment = ""
    public static void ResumeTimer(JTimer whichTimer)
    {
        War3.CallNative(_resumeTimerPtr, whichTimer.Handle);
    }


    /// title = "到期的计时器"
    /// description = "到期的计时器"
    /// comment = "响应'时间 - 计时器到期'事件."
    public static JTimer GetExpiredTimer()
    {
        var handle = War3.CallNative<int>(_getExpiredTimerPtr);
        return new JTimer(handle);
    }


    /// title = "新建的单位组 [R]"
    /// description = "新建的空单位组"
    /// comment = "会创建单位组."
    public static JGroup CreateGroup()
    {
        var handle = War3.CallNative<int>(_createGroupPtr);
        return new JGroup(handle);
    }


    /// title = "删除单位组 [R]"
    /// description = "删除 ${单位组}"
    /// comment = ""
    public static void DestroyGroup(JGroup whichGroup)
    {
        War3.CallNative(_destroyGroupPtr, whichGroup.Handle);
    }


    /// title = "添加单位 [R]"
    /// description = "为 ${单位组} 添加 ${单位}"
    /// comment = "并不影响单位本身."
    public static void GroupAddUnit(JGroup whichGroup, JUnit whichUnit)
    {
        War3.CallNative(_groupAddUnitPtr, whichGroup.Handle, whichUnit.Handle);
    }


    /// title = "移除单位 [R]"
    /// description = "为 ${单位组} 删除 ${单位}"
    /// comment = "并不影响单位本身."
    public static void GroupRemoveUnit(JGroup whichGroup, JUnit whichUnit)
    {
        War3.CallNative(_groupRemoveUnitPtr, whichGroup.Handle, whichUnit.Handle);
    }


    /// title = "清空单位组"
    /// description = "清空 ${单位组} 内所有单位"
    /// comment = "并不影响单位本身."
    public static void GroupClear(JGroup whichGroup)
    {
        War3.CallNative(_groupClearPtr, whichGroup.Handle);
    }

    public static void GroupEnumUnitsOfType(JGroup whichGroup, string unitname, JBoolExpr filter)
    {
        War3.CallNative(_groupEnumUnitsOfTypePtr, whichGroup.Handle, unitname, filter.Handle);
    }

    public static void GroupEnumUnitsOfPlayer(JGroup whichGroup, JPlayer whichPlayer, JBoolExpr filter)
    {
        War3.CallNative(_groupEnumUnitsOfPlayerPtr, whichGroup.Handle, whichPlayer.Handle, filter.Handle);
    }

    public static void GroupEnumUnitsOfTypeCounted(JGroup whichGroup, string unitname, JBoolExpr filter, int countLimit)
    {
        War3.CallNative(_groupEnumUnitsOfTypeCountedPtr, whichGroup.Handle, unitname, filter.Handle, countLimit);
    }

    public static void GroupEnumUnitsInRect(JGroup whichGroup, JRect r, JBoolExpr filter)
    {
        War3.CallNative(_groupEnumUnitsInRectPtr, whichGroup.Handle, r.Handle, filter.Handle);
    }

    public static void GroupEnumUnitsInRectCounted(JGroup whichGroup, JRect r, JBoolExpr filter, int countLimit)
    {
        War3.CallNative(_groupEnumUnitsInRectCountedPtr, whichGroup.Handle, r.Handle, filter.Handle, countLimit);
    }


    /// title = "选取单位添加到单位组(坐标)"
    /// description = "为 ${单位组} 添加以( ${坐标X} , ${坐标Y} )为圆心，${半径} 为半径的圆范围内，满足 ${条件} 的单位"
    /// comment = ""
    public static void GroupEnumUnitsInRange(JGroup whichGroup, float x, float y, float radius, JBoolExpr filter)
    {
        War3.CallNative(_groupEnumUnitsInRangePtr, whichGroup.Handle, x, y, radius, filter.Handle);
    }


    /// title = "选取单位添加到单位组(点)"
    /// description = "为 ${单位组} 添加以 ${点} 为圆心，${半径} 为半径的圆范围内，满足 ${条件} 的单位"
    /// comment = ""
    public static void GroupEnumUnitsInRangeOfLoc(JGroup whichGroup, JLocation whichLocation, float radius,
        JBoolExpr filter)
    {
        War3.CallNative(_groupEnumUnitsInRangeOfLocPtr, whichGroup.Handle, whichLocation.Handle, radius, filter.Handle);
    }


    /// title = "选取单位添加到单位组(坐标)(不建议使用)"
    /// description = "为 ${单位组} 添加以( ${坐标X} , ${坐标Y} )为圆心，${半径} 为半径的圆范围内，满足 ${条件} 的单位。无效项( ${N} )"
    /// comment = "最后一项是无效项，建议用上一个UI"
    public static void GroupEnumUnitsInRangeCounted(JGroup whichGroup, float x, float y, float radius, JBoolExpr filter,
        int countLimit)
    {
        War3.CallNative(_groupEnumUnitsInRangeCountedPtr, whichGroup.Handle, x, y, radius, filter.Handle, countLimit);
    }


    /// title = "选取单位添加到单位组(点)(不建议使用)"
    /// description = "为 ${单位组} 添加以 ${点} 为圆心，${半径} 为半径的圆范围内，满足 ${条件} 的单位。无效项( ${N} )"
    /// comment = "最后一项是无效项，建议用上一个UI"
    public static void GroupEnumUnitsInRangeOfLocCounted(JGroup whichGroup, JLocation whichLocation, float radius,
        JBoolExpr filter, int countLimit)
    {
        War3.CallNative(_groupEnumUnitsInRangeOfLocCountedPtr, whichGroup.Handle, whichLocation.Handle, radius,
            filter.Handle, countLimit);
    }

    public static void GroupEnumUnitsSelected(JGroup whichGroup, JPlayer whichPlayer, JBoolExpr filter)
    {
        War3.CallNative(_groupEnumUnitsSelectedPtr, whichGroup.Handle, whichPlayer.Handle, filter.Handle);
    }


    /// title = "发布命令(无目标)"
    /// description = "对 ${单位组}发布 ${Order}"
    /// comment = "最多只能对单位组中12个单位发布命令."
    public static bool GroupImmediateOrder(JGroup whichGroup, string order)
    {
        return War3.CallNative<bool>(_groupImmediateOrderPtr, whichGroup.Handle, order);
    }


    /// title = "发布命令(无目标)(ID)"
    /// description = "对 ${单位组}发布 ${Order}"
    /// comment = "最多只能对单位组中12个单位发布命令."
    public static bool GroupImmediateOrderById(JGroup whichGroup, int order)
    {
        return War3.CallNative<bool>(_groupImmediateOrderByIdPtr, whichGroup.Handle, order);
    }


    /// title = "发布命令(指定坐标) [R]"
    /// description = "对 ${单位组}发布 ${Order} 命令,目标点:(${X},${Y})"
    /// comment = "最多只能对单位组中12个单位发布命令."
    public static bool GroupPointOrder(JGroup whichGroup, string order, float x, float y)
    {
        return War3.CallNative<bool>(_groupPointOrderPtr, whichGroup.Handle, order, x, y);
    }


    /// title = "发布命令(指定点)"
    /// description = "对 ${单位组}发布 ${Order} 命令,目标: ${指定点}"
    /// comment = "最多只能对单位组中12个单位发布命令."
    public static bool GroupPointOrderLoc(JGroup whichGroup, string order, JLocation whichLocation)
    {
        return War3.CallNative<bool>(_groupPointOrderLocPtr, whichGroup.Handle, order, whichLocation.Handle);
    }


    /// title = "发布命令(指定坐标)(ID)"
    /// description = "对 ${单位组}发布 ${Order} 命令,目标点:(${X},${Y})"
    /// comment = "最多只能对单位组中12个单位发布命令."
    public static bool GroupPointOrderById(JGroup whichGroup, int order, float x, float y)
    {
        return War3.CallNative<bool>(_groupPointOrderByIdPtr, whichGroup.Handle, order, x, y);
    }


    /// title = "发布命令(指定点)(ID)"
    /// description = "对 ${单位组}发布 ${Order} 命令,目标: ${指定点}"
    /// comment = "最多只能对单位组中12个单位发布命令."
    public static bool GroupPointOrderByIdLoc(JGroup whichGroup, int order, JLocation whichLocation)
    {
        return War3.CallNative<bool>(_groupPointOrderByIdLocPtr, whichGroup.Handle, order, whichLocation.Handle);
    }


    /// title = "发布命令(指定单位)"
    /// description = "对 ${单位组} 发布 ${Order} 命令,目标: ${单位}"
    /// comment = "最多只能对单位组中12个单位发布命令."
    public static bool GroupTargetOrder(JGroup whichGroup, string order, JWidget targetWidget)
    {
        return War3.CallNative<bool>(_groupTargetOrderPtr, whichGroup.Handle, order, targetWidget.Handle);
    }


    /// title = "发布命令(指定单位)(ID)"
    /// description = "对 ${单位组} 发布 ${Order} 命令,目标: ${单位}"
    /// comment = "最多只能对单位组中12个单位发布命令."
    public static bool GroupTargetOrderById(JGroup whichGroup, int order, JWidget targetWidget)
    {
        return War3.CallNative<bool>(_groupTargetOrderByIdPtr, whichGroup.Handle, order, targetWidget.Handle);
    }


    /// title = "选取单位组内单位做动作"
    /// description = "选取 ${单位组} 内所有单位 ${做动作}"
    /// comment = "使用'选取单位'来取代相应的单位. 对于单位组内每个单位都会运行一次动作(包括死亡的,不包括隐藏的). 等待不能在组动作中运行."
    public static void ForGroup(JGroup whichGroup, Action callback)
    {
        War3.CallNative(_forGroupPtr, whichGroup.Handle, callback);
    }


    /// title = "单位组中第一个单位"
    /// description = "${单位组} 中第一个单位"
    public static JUnit FirstOfGroup(JGroup whichGroup)
    {
        var handle = War3.CallNative<int>(_firstOfGroupPtr, whichGroup.Handle);
        return new JUnit(handle);
    }


    /// title = "新建玩家组 [R]"
    /// description = "新建空玩家组"
    /// comment = "会创建玩家组."
    public static JForce CreateForce()
    {
        var handle = War3.CallNative<int>(_createForcePtr);
        return new JForce(handle);
    }


    /// title = "删除玩家组 [R]"
    /// description = "删除 ${玩家组}"
    /// comment = "注意: 不要删除系统预置的玩家组."
    public static void DestroyForce(JForce whichForce)
    {
        War3.CallNative(_destroyForcePtr, whichForce.Handle);
    }


    /// title = "添加玩家 [R]"
    /// description = " ${玩家组} 添加 ${玩家}"
    /// comment = "并不影响玩家本身."
    public static void ForceAddPlayer(JForce whichForce, JPlayer whichPlayer)
    {
        War3.CallNative(_forceAddPlayerPtr, whichForce.Handle, whichPlayer.Handle);
    }


    /// title = "移除玩家 [R]"
    /// description = "从 ${玩家组} 中移除 ${玩家}"
    /// comment = "并不影响玩家本身."
    public static void ForceRemovePlayer(JForce whichForce, JPlayer whichPlayer)
    {
        War3.CallNative(_forceRemovePlayerPtr, whichForce.Handle, whichPlayer.Handle);
    }


    /// title = "清空玩家组"
    /// description = "清空 ${玩家组} 内所有玩家"
    /// comment = "并不影响玩家本身."
    public static void ForceClear(JForce whichForce)
    {
        War3.CallNative(_forceClearPtr, whichForce.Handle);
    }

    public static void ForceEnumPlayers(JForce whichForce, JBoolExpr filter)
    {
        War3.CallNative(_forceEnumPlayersPtr, whichForce.Handle, filter.Handle);
    }

    public static void ForceEnumPlayersCounted(JForce whichForce, JBoolExpr filter, int countLimit)
    {
        War3.CallNative(_forceEnumPlayersCountedPtr, whichForce.Handle, filter.Handle, countLimit);
    }

    public static void ForceEnumAllies(JForce whichForce, JPlayer whichPlayer, JBoolExpr filter)
    {
        War3.CallNative(_forceEnumAlliesPtr, whichForce.Handle, whichPlayer.Handle, filter.Handle);
    }

    public static void ForceEnumEnemies(JForce whichForce, JPlayer whichPlayer, JBoolExpr filter)
    {
        War3.CallNative(_forceEnumEnemiesPtr, whichForce.Handle, whichPlayer.Handle, filter.Handle);
    }


    /// title = "选取玩家组内玩家做动作"
    /// description = "选取 ${玩家组} 内所有玩家 ${做动作}"
    /// comment = "玩家组动作中可使用'选取玩家'来获取对应的玩家. 等待不能在组动作中运行."
    public static void ForForce(JForce whichForce, Action callback)
    {
        War3.CallNative(_forForcePtr, whichForce.Handle, callback);
    }


    /// title = "新建矩形区域(指定边角坐标)"
    /// description = "左下角为(${X1}, ${Y1}),右上角为(${X2}, ${Y2})的矩形区域"
    /// comment = "会创建矩形区域."
    public static JRect Rect(float minx, float miny, float maxx, float maxy)
    {
        var handle = War3.CallNative<int>(_rectPtr, minx, miny, maxx, maxy);
        return new JRect(handle);
    }


    /// title = "新建矩形区域(指定边角点)"
    /// description = "左下角为 ${点1} ,右上角为 ${点2} 的矩形区域"
    /// comment = "会创建矩形区域."
    public static JRect RectFromLoc(JLocation min, JLocation max)
    {
        var handle = War3.CallNative<int>(_rectFromLocPtr, min.Handle, max.Handle);
        return new JRect(handle);
    }


    /// title = "删除矩形区域 [R]"
    /// description = "删除 ${矩形区域}"
    /// comment = ""
    public static void RemoveRect(JRect whichRect)
    {
        War3.CallNative(_removeRectPtr, whichRect.Handle);
    }


    /// title = "设置矩形区域(指定坐标) [R]"
    /// description = "重新设置 ${矩形区域} ,左下角坐标为(${X},${Y}), 右上角坐标为(${X},${Y})"
    /// comment = "该区域必须是一个变量. 重新设置矩形区域的大小和位置."
    public static void SetRect(JRect whichRect, float minx, float miny, float maxx, float maxy)
    {
        War3.CallNative(_setRectPtr, whichRect.Handle, minx, miny, maxx, maxy);
    }


    /// title = "设置矩形区域(指定点) [R]"
    /// description = "重新设置 ${矩形区域} ,左下角点为 ${点} 右上角点为 ${点}"
    /// comment = "该区域必须是一个变量. 重新设置矩形区域的大小和位置."
    public static void SetRectFromLoc(JRect whichRect, JLocation min, JLocation max)
    {
        War3.CallNative(_setRectFromLocPtr, whichRect.Handle, min.Handle, max.Handle);
    }


    /// title = "移动矩形区域(指定坐标) [R]"
    /// description = "移动 ${矩形区域} 到(${X},${Y})"
    /// comment = "该区域必须是一个变量. 目标点将作为该区域的新中心点."
    public static void MoveRectTo(JRect whichRect, float newCenterX, float newCenterY)
    {
        War3.CallNative(_moveRectToPtr, whichRect.Handle, newCenterX, newCenterY);
    }


    /// title = "移动矩形区域(指定点)"
    /// description = "移动 ${矩形区域} 到 ${目标点}"
    /// comment = "该区域必须是一个变量. 目标点将作为该区域的新中心点."
    public static void MoveRectToLoc(JRect whichRect, JLocation newCenterLoc)
    {
        War3.CallNative(_moveRectToLocPtr, whichRect.Handle, newCenterLoc.Handle);
    }


    /// title = "中心X坐标"
    /// description = "${矩形区域} 的中心X坐标"
    /// comment = ""
    public static float GetRectCenterX(JRect whichRect)
    {
        return War3.CallNative<float>(_getRectCenterXPtr, whichRect.Handle);
    }


    /// title = "中心Y坐标"
    /// description = "${矩形区域} 的中心Y坐标"
    /// comment = ""
    public static float GetRectCenterY(JRect whichRect)
    {
        return War3.CallNative<float>(_getRectCenterYPtr, whichRect.Handle);
    }


    /// title = "左下角X坐标"
    /// description = "${矩形区域} 的左下角X坐标"
    /// comment = ""
    public static float GetRectMinX(JRect whichRect)
    {
        return War3.CallNative<float>(_getRectMinXPtr, whichRect.Handle);
    }


    /// title = "左下角Y坐标"
    /// description = "${矩形区域} 的左下角Y坐标"
    /// comment = ""
    public static float GetRectMinY(JRect whichRect)
    {
        return War3.CallNative<float>(_getRectMinYPtr, whichRect.Handle);
    }


    /// title = "右上角X坐标"
    /// description = "${矩形区域} 的右上角X坐标"
    /// comment = ""
    public static float GetRectMaxX(JRect whichRect)
    {
        return War3.CallNative<float>(_getRectMaxXPtr, whichRect.Handle);
    }


    /// title = "右上角Y坐标"
    /// description = "${矩形区域} 的右上角Y坐标"
    /// comment = ""
    public static float GetRectMaxY(JRect whichRect)
    {
        return War3.CallNative<float>(_getRectMaxYPtr, whichRect.Handle);
    }


    /// title = "新建区域 [R]"
    /// description = "新建区域"
    /// comment = "会创建一个新的不规则区域,如果不往该区域添加点或地区,则该区域无效果."
    public static JRegion CreateRegion()
    {
        var handle = War3.CallNative<int>(_createRegionPtr);
        return new JRegion(handle);
    }


    /// title = "删除不规则区域 [R]"
    /// description = "删除 ${不规则区域}"
    /// comment = ""
    public static void RemoveRegion(JRegion whichRegion)
    {
        War3.CallNative(_removeRegionPtr, whichRegion.Handle);
    }


    /// title = "添加区域 [R]"
    /// description = "对 ${不规则区域} 添加 ${矩形区域}"
    /// comment = "区域是游戏中一个游戏地区的集合体,可以包含地区和点."
    public static void RegionAddRect(JRegion whichRegion, JRect r)
    {
        War3.CallNative(_regionAddRectPtr, whichRegion.Handle, r.Handle);
    }


    /// title = "移除区域 [R]"
    /// description = "在 ${不规则区域} 中移除 ${矩形区域}"
    /// comment = ""
    public static void RegionClearRect(JRegion whichRegion, JRect r)
    {
        War3.CallNative(_regionClearRectPtr, whichRegion.Handle, r.Handle);
    }


    /// title = "添加单元点(指定坐标) [R]"
    /// description = "对 ${不规则区域} 添加单元点: (${X},${Y})"
    /// comment = "单元点大小为32x32."
    public static void RegionAddCell(JRegion whichRegion, float x, float y)
    {
        War3.CallNative(_regionAddCellPtr, whichRegion.Handle, x, y);
    }


    /// title = "添加单元点(指定点) [R]"
    /// description = "对 ${不规则区域} 添加单元点: ${点}"
    /// comment = "单元点大小为32x32."
    public static void RegionAddCellAtLoc(JRegion whichRegion, JLocation whichLocation)
    {
        War3.CallNative(_regionAddCellAtLocPtr, whichRegion.Handle, whichLocation.Handle);
    }


    /// title = "移除单元点(指定坐标) [R]"
    /// description = "在 ${不规则区域} 中移除单元点: (${X},${Y})"
    /// comment = "单元点大小为32x32."
    public static void RegionClearCell(JRegion whichRegion, float x, float y)
    {
        War3.CallNative(_regionClearCellPtr, whichRegion.Handle, x, y);
    }


    /// title = "移除单元点(指定点) [R]"
    /// description = "在 ${不规则区域} 中移除单元点: ${点}"
    /// comment = "单元点大小为32x32."
    public static void RegionClearCellAtLoc(JRegion whichRegion, JLocation whichLocation)
    {
        War3.CallNative(_regionClearCellAtLocPtr, whichRegion.Handle, whichLocation.Handle);
    }


    /// title = "坐标点"
    /// description = "坐标(${X}, ${Y})"
    /// comment = "会创建点."
    public static JLocation Location(float x, float y)
    {
        var handle = War3.CallNative<int>(_locationPtr, x, y);
        return new JLocation(handle);
    }


    /// title = "清除点 [R]"
    /// description = "清除 ${点}"
    /// comment = "点是堆积最多的垃圾资源,不需要再使用的点都要记得清除掉."
    public static void RemoveLocation(JLocation whichLocation)
    {
        War3.CallNative(_removeLocationPtr, whichLocation.Handle);
    }


    /// title = "移动点 [R]"
    /// description = "移动 ${点} 到(${X},${Y})"
    /// comment = "该点必须是一个变量. 因为移动一个不可重用的点是无意义的."
    public static void MoveLocation(JLocation whichLocation, float newX, float newY)
    {
        War3.CallNative(_moveLocationPtr, whichLocation.Handle, newX, newY);
    }


    /// title = "点的X轴坐标"
    /// description = "${点} 的X轴坐标"
    /// comment = ""
    public static float GetLocationX(JLocation whichLocation)
    {
        return War3.CallNative<float>(_getLocationXPtr, whichLocation.Handle);
    }


    /// title = "点的Y轴坐标"
    /// description = "${点} 的Y轴坐标"
    /// comment = ""
    public static float GetLocationY(JLocation whichLocation)
    {
        return War3.CallNative<float>(_getLocationYPtr, whichLocation.Handle);
    }


    /// title = "点的Z轴高度 [R]"
    /// description = "${点} 的Z轴高度"
    /// comment = ""
    public static float GetLocationZ(JLocation whichLocation)
    {
        return War3.CallNative<float>(_getLocationZPtr, whichLocation.Handle);
    }


    /// title = "在不规则区域内 [R]"
    /// description = "${不规则区域} 内存在 ${单位}"
    /// comment = ""
    public static bool IsUnitInRegion(JRegion whichRegion, JUnit whichUnit)
    {
        return War3.CallNative<bool>(_isUnitInRegionPtr, whichRegion.Handle, whichUnit.Handle);
    }


    /// title = "包含坐标"
    /// description = "${不规则区域} 内包含坐标(${X},${Y})"
    /// comment = "TC_REGION"
    public static bool IsPointInRegion(JRegion whichRegion, float x, float y)
    {
        return War3.CallNative<bool>(_isPointInRegionPtr, whichRegion.Handle, x, y);
    }


    /// title = "包含点"
    /// description = "${不规则区域} 内包含点: ${点}"
    /// comment = "TC_REGION"
    public static bool IsLocationInRegion(JRegion whichRegion, JLocation whichLocation)
    {
        return War3.CallNative<bool>(_isLocationInRegionPtr, whichRegion.Handle, whichLocation.Handle);
    }

    public static JRect GetWorldBounds()
    {
        var handle = War3.CallNative<int>(_getWorldBoundsPtr);
        return new JRect(handle);
    }


    /// title = "新建触发 [R]"
    /// description = "新建触发"
    /// comment = "会创建一个新的触发器,如果对该功能不熟悉请慎用."
    public static JTrigger CreateTrigger()
    {
        var handle = War3.CallNative<int>(_createTriggerPtr);
        return new JTrigger(handle);
    }


    /// title = "删除触发器 [R]"
    /// description = "删除 ${触发器}"
    /// comment = "对不再使用的触发器可以使用该动作来删除."
    public static void DestroyTrigger(JTrigger whichTrigger)
    {
        War3.CallNative(_destroyTriggerPtr, whichTrigger.Handle);
    }

    public static void ResetTrigger(JTrigger whichTrigger)
    {
        War3.CallNative(_resetTriggerPtr, whichTrigger.Handle);
    }


    /// title = "开启触发"
    /// description = "开启 ${Trigger}"
    /// comment = ""
    public static void EnableTrigger(JTrigger whichTrigger)
    {
        War3.CallNative(_enableTriggerPtr, whichTrigger.Handle);
    }


    /// title = "关闭触发"
    /// description = "关闭 ${Trigger}"
    /// comment = ""
    public static void DisableTrigger(JTrigger whichTrigger)
    {
        War3.CallNative(_disableTriggerPtr, whichTrigger.Handle);
    }


    /// title = "触发开启"
    /// description = "${触发} 处于开启状态"
    /// comment = ""
    public static bool IsTriggerEnabled(JTrigger whichTrigger)
    {
        return War3.CallNative<bool>(_isTriggerEnabledPtr, whichTrigger.Handle);
    }

    public static void TriggerWaitOnSleeps(JTrigger whichTrigger, bool flag)
    {
        War3.CallNative(_triggerWaitOnSleepsPtr, whichTrigger.Handle, flag);
    }

    public static bool IsTriggerWaitOnSleeps(JTrigger whichTrigger)
    {
        return War3.CallNative<bool>(_isTriggerWaitOnSleepsPtr, whichTrigger.Handle);
    }


    /// title = "匹配单位"
    /// description = "匹配单位"
    /// comment = "在'选取单位满足条件'之类功能的条件中,指代被判断单位."
    public static JUnit GetFilterUnit()
    {
        var handle = War3.CallNative<int>(_getFilterUnitPtr);
        return new JUnit(handle);
    }


    /// title = "选取单位"
    /// description = "选取单位"
    /// comment = "使用'选取单位做动作'时, 指代相应的单位."
    public static JUnit GetEnumUnit()
    {
        var handle = War3.CallNative<int>(_getEnumUnitPtr);
        return new JUnit(handle);
    }


    /// title = "匹配的可破坏物"
    /// description = "匹配的可破坏物"
    /// comment = "在'选取可破坏物满足条件'之类功能的条件中,指代被判断的可破坏物."
    public static JDestructable GetFilterDestructable()
    {
        var handle = War3.CallNative<int>(_getFilterDestructablePtr);
        return new JDestructable(handle);
    }


    /// title = "选取的可破坏物"
    /// description = "选取的可破坏物"
    /// comment = "使用'选取可破坏物做动作'时, 指代相应的可破坏物."
    public static JDestructable GetEnumDestructable()
    {
        var handle = War3.CallNative<int>(_getEnumDestructablePtr);
        return new JDestructable(handle);
    }


    /// title = "匹配物品"
    /// description = "匹配物品"
    /// comment = "在'选取物品满足条件'之类功能的条件中,指代被判断单位."
    public static JItem GetFilterItem()
    {
        var handle = War3.CallNative<int>(_getFilterItemPtr);
        return new JItem(handle);
    }


    /// title = "选取物品"
    /// description = "选取物品"
    /// comment = "使用'选取物品做动作'时, 指代相应的物品."
    public static JItem GetEnumItem()
    {
        var handle = War3.CallNative<int>(_getEnumItemPtr);
        return new JItem(handle);
    }


    /// title = "匹配玩家"
    /// description = "匹配玩家"
    /// comment = "在'选取玩家满足条件'之类功能的条件中,指代被判断玩家."
    public static JPlayer GetFilterPlayer()
    {
        var handle = War3.CallNative<int>(_getFilterPlayerPtr);
        return new JPlayer(handle);
    }


    /// title = "选取玩家"
    /// description = "选取玩家"
    /// comment = "使用'选取玩家做动作'时, 指代相应的玩家."
    public static JPlayer GetEnumPlayer()
    {
        var handle = War3.CallNative<int>(_getEnumPlayerPtr);
        return new JPlayer(handle);
    }


    /// title = "当前触发"
    /// description = "当前触发"
    /// comment = "当前所运行的触发器."
    public static JTrigger GetTriggeringTrigger()
    {
        var handle = War3.CallNative<int>(_getTriggeringTriggerPtr);
        return new JTrigger(handle);
    }

    public static JEventId GetTriggerEventId()
    {
        var handle = War3.CallNative<int>(_getTriggerEventIdPtr);
        return new JEventId(handle);
    }


    /// title = "触发条件判断次数"
    /// description = "${Trigger} 的触发条件判断次数"
    /// comment = ""
    public static int GetTriggerEvalCount(JTrigger whichTrigger)
    {
        return War3.CallNative<int>(_getTriggerEvalCountPtr, whichTrigger.Handle);
    }


    /// title = "触发动作运行次数"
    /// description = "${Trigger} 的触发动作运行次数"
    /// comment = ""
    public static int GetTriggerExecCount(JTrigger whichTrigger)
    {
        return War3.CallNative<int>(_getTriggerExecCountPtr, whichTrigger.Handle);
    }


    /// title = "运行函数 [R]"
    /// description = "运行: ${函数名}"
    /// comment = "使用该功能运行的函数与触发独立, 只能运行自定义无参数函数."
    public static void ExecuteFunc(string funcName)
    {
        War3.CallNative(_executeFuncPtr, funcName);
    }

    public static JBoolExpr And(JBoolExpr operandA, JBoolExpr operandB)
    {
        var handle = War3.CallNative<int>(_andPtr, operandA.Handle, operandB.Handle);
        return new JBoolExpr(handle);
    }

    public static JBoolExpr Or(JBoolExpr operandA, JBoolExpr operandB)
    {
        var handle = War3.CallNative<int>(_orPtr, operandA.Handle, operandB.Handle);
        return new JBoolExpr(handle);
    }

    public static JBoolExpr Not(JBoolExpr operand)
    {
        var handle = War3.CallNative<int>(_notPtr, operand.Handle);
        return new JBoolExpr(handle);
    }

    public static JConditionFunc Condition(Action func)
    {
        var handle = War3.CallNative<int>(_conditionPtr, func);
        return new JConditionFunc(handle);
    }

    public static void DestroyCondition(JConditionFunc c)
    {
        War3.CallNative(_destroyConditionPtr, c.Handle);
    }

    public static JFilterFunc Filter(Action func)
    {
        var handle = War3.CallNative<int>(_filterPtr, func);
        return new JFilterFunc(handle);
    }

    public static void DestroyFilter(JFilterFunc f)
    {
        War3.CallNative(_destroyFilterPtr, f.Handle);
    }

    public static void DestroyBoolExpr(JBoolExpr e)
    {
        War3.CallNative(_destroyBoolExprPtr, e.Handle);
    }


    /// title = "实数变量事件"
    /// description = "${变量} 的值 ${Operation} ${值}"
    /// comment = "这个事件只适用于实数类型的变量."
    public static JEvent TriggerRegisterVariableEvent(JTrigger whichTrigger, string varName, JLimitOp opcode,
        float limitval)
    {
        var handle = War3.CallNative<int>(_triggerRegisterVariableEventPtr, whichTrigger.Handle, varName, opcode.Handle,
            limitval);
        return new JEvent(handle);
    }

    public static JEvent TriggerRegisterTimerEvent(JTrigger whichTrigger, float timeout, bool periodic)
    {
        var handle = War3.CallNative<int>(_triggerRegisterTimerEventPtr, whichTrigger.Handle, timeout, periodic);
        return new JEvent(handle);
    }

    public static JEvent TriggerRegisterTimerExpireEvent(JTrigger whichTrigger, JTimer t)
    {
        var handle = War3.CallNative<int>(_triggerRegisterTimerExpireEventPtr, whichTrigger.Handle, t.Handle);
        return new JEvent(handle);
    }

    public static JEvent TriggerRegisterGameStateEvent(JTrigger whichTrigger, JGameState whichState, JLimitOp opcode,
        float limitval)
    {
        var handle = War3.CallNative<int>(_triggerRegisterGameStateEventPtr, whichTrigger.Handle, whichState.Handle,
            opcode.Handle, limitval);
        return new JEvent(handle);
    }

    public static JEvent TriggerRegisterDialogEvent(JTrigger whichTrigger, JDialog whichDialog)
    {
        var handle = War3.CallNative<int>(_triggerRegisterDialogEventPtr, whichTrigger.Handle, whichDialog.Handle);
        return new JEvent(handle);
    }


    /// title = "对话框按钮被点击 [R]"
    /// description = "${对话框按钮} 被点击"
    /// comment = "指定对话框按钮被点击,该事件一般需要在其他触发为其添加."
    public static JEvent TriggerRegisterDialogButtonEvent(JTrigger whichTrigger, JButton whichButton)
    {
        var handle =
            War3.CallNative<int>(_triggerRegisterDialogButtonEventPtr, whichTrigger.Handle, whichButton.Handle);
        return new JEvent(handle);
    }

    public static JGameState GetEventGameState()
    {
        var handle = War3.CallNative<int>(_getEventGameStatePtr);
        return new JGameState(handle);
    }


    /// title = "比赛游戏事件"
    /// description = "该游戏将在 ${Event Type} 后结束"
    /// comment = "该事件只出现在Battle.net的自动匹配游戏."
    public static JEvent TriggerRegisterGameEvent(JTrigger whichTrigger, JGameEvent whichGameEvent)
    {
        var handle = War3.CallNative<int>(_triggerRegisterGameEventPtr, whichTrigger.Handle, whichGameEvent.Handle);
        return new JEvent(handle);
    }

    public static JPlayer GetWinningPlayer()
    {
        var handle = War3.CallNative<int>(_getWinningPlayerPtr);
        return new JPlayer(handle);
    }


    /// title = "单位进入不规则区域(指定条件) [R]"
    /// description = "单位进入 ${区域} 并满足 ${条件}"
    /// comment = "使用'事件响应 - 进入的单位'来响应进入该区域的单位. 该事件需要在其他触发为其添加."
    public static JEvent TriggerRegisterEnterRegion(JTrigger whichTrigger, JRegion whichRegion, JBoolExpr filter)
    {
        var handle = War3.CallNative<int>(_triggerRegisterEnterRegionPtr, whichTrigger.Handle, whichRegion.Handle,
            filter.Handle);
        return new JEvent(handle);
    }


    /// title = "触发区域 [R]"
    /// description = "触发区域"
    /// comment = "响应单位进入/离开不规则区域事件."
    public static JRegion GetTriggeringRegion()
    {
        var handle = War3.CallNative<int>(_getTriggeringRegionPtr);
        return new JRegion(handle);
    }


    /// title = "进入的单位"
    /// description = "进入的单位"
    /// comment = "响应'单位进入区域'单位事件."
    public static JUnit GetEnteringUnit()
    {
        var handle = War3.CallNative<int>(_getEnteringUnitPtr);
        return new JUnit(handle);
    }


    /// title = "单位离开不规则区域(指定条件) [R]"
    /// description = "单位离开 ${区域} 并满足 ${条件}"
    /// comment = "使用'事件响应 - 离开的单位'来响应离开该区域的单位. 该事件需要在其他触发为其添加."
    public static JEvent TriggerRegisterLeaveRegion(JTrigger whichTrigger, JRegion whichRegion, JBoolExpr filter)
    {
        var handle = War3.CallNative<int>(_triggerRegisterLeaveRegionPtr, whichTrigger.Handle, whichRegion.Handle,
            filter.Handle);
        return new JEvent(handle);
    }


    /// title = "离开的单位"
    /// description = "离开的单位"
    /// comment = "响应'单位离开区域'单位事件."
    public static JUnit GetLeavingUnit()
    {
        var handle = War3.CallNative<int>(_getLeavingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "鼠标点击可追踪物 [R]"
    /// description = "鼠标点击 ${可追踪物}"
    /// comment = ""
    public static JEvent TriggerRegisterTrackableHitEvent(JTrigger whichTrigger, JTrackable t)
    {
        var handle = War3.CallNative<int>(_triggerRegisterTrackableHitEventPtr, whichTrigger.Handle, t.Handle);
        return new JEvent(handle);
    }


    /// title = "鼠标移动到追踪对象 [R]"
    /// description = "鼠标移动到 ${可追踪物}"
    /// comment = ""
    public static JEvent TriggerRegisterTrackableTrackEvent(JTrigger whichTrigger, JTrackable t)
    {
        var handle = War3.CallNative<int>(_triggerRegisterTrackableTrackEventPtr, whichTrigger.Handle, t.Handle);
        return new JEvent(handle);
    }


    /// title = "事件响应 - 触发可追踪物 [R]"
    /// description = "事件响应 - 触发可追踪物"
    /// comment = ""
    public static JTrackable GetTriggeringTrackable()
    {
        var handle = War3.CallNative<int>(_getTriggeringTrackablePtr);
        return new JTrackable(handle);
    }

    public static JButton GetClickedButton()
    {
        var handle = War3.CallNative<int>(_getClickedButtonPtr);
        return new JButton(handle);
    }

    public static JDialog GetClickedDialog()
    {
        var handle = War3.CallNative<int>(_getClickedDialogPtr);
        return new JDialog(handle);
    }


    /// title = "比赛剩余时间"
    /// description = "比赛剩余时间"
    /// comment = "响应'比赛事件'游戏将要结束. 单位为秒."
    public static float GetTournamentFinishSoonTimeRemaining()
    {
        return War3.CallNative<float>(_getTournamentFinishSoonTimeRemainingPtr);
    }


    /// title = "比赛结束规则"
    /// description = "比赛结束规则"
    /// comment = "1表示游戏开始时间已经超过限定时,无法以平局结束. 其他值表示游戏还在初期阶段,此时退出游戏将以平局结束.."
    public static int GetTournamentFinishNowRule()
    {
        return War3.CallNative<int>(_getTournamentFinishNowRulePtr);
    }

    public static JPlayer GetTournamentFinishNowPlayer()
    {
        var handle = War3.CallNative<int>(_getTournamentFinishNowPlayerPtr);
        return new JPlayer(handle);
    }


    /// title = "对战比赛得分"
    /// description = "${Player} 的当前对战比赛得分"
    /// comment = "对战游戏时如果游戏时间过长,系统将以该值来决定胜负."
    public static int GetTournamentScore(JPlayer whichPlayer)
    {
        return War3.CallNative<int>(_getTournamentScorePtr, whichPlayer.Handle);
    }


    /// title = "存档文件名"
    /// description = "存档文件名"
    /// comment = "响应'游戏 - 保存进度'事件."
    public static string GetSaveBasicFilename()
    {
        return War3.CallNative<string>(_getSaveBasicFilenamePtr);
    }

    public static JEvent TriggerRegisterPlayerEvent(JTrigger whichTrigger, JPlayer whichPlayer,
        JPlayerEvent whichPlayerEvent)
    {
        var handle = War3.CallNative<int>(_triggerRegisterPlayerEventPtr, whichTrigger.Handle, whichPlayer.Handle,
            whichPlayerEvent.Handle);
        return new JEvent(handle);
    }


    /// title = "触发玩家"
    /// description = "触发玩家"
    /// comment = ""
    public static JPlayer GetTriggerPlayer()
    {
        var handle = War3.CallNative<int>(_getTriggerPlayerPtr);
        return new JPlayer(handle);
    }

    public static JEvent TriggerRegisterPlayerUnitEvent(JTrigger whichTrigger, JPlayer whichPlayer,
        JPlayerUnitEvent whichPlayerUnitEvent, JBoolExpr filter)
    {
        var filterHandle = filter is null ? 0 : filter.Handle;
        var handle = War3.CallNative<int>(_triggerRegisterPlayerUnitEventPtr, whichTrigger.Handle, whichPlayer.Handle,
            whichPlayerUnitEvent.Handle, filterHandle);
        return new JEvent(handle);
    }


    /// title = "升级的英雄"
    /// description = "升级的英雄"
    /// comment = "响应'提升等级'单位事件."
    public static JUnit GetLevelingUnit()
    {
        var handle = War3.CallNative<int>(_getLevelingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "学习技能的英雄"
    /// description = "学习技能的英雄"
    /// comment = "响应'学习技能'单位事件."
    public static JUnit GetLearningUnit()
    {
        var handle = War3.CallNative<int>(_getLearningUnitPtr);
        return new JUnit(handle);
    }


    /// title = "学习技能 [R]"
    /// description = "学习技能"
    /// comment = "响应'学习技能'单位事件, 指代被学习的技能."
    public static int GetLearnedSkill()
    {
        return War3.CallNative<int>(_getLearnedSkillPtr);
    }


    /// title = "学习技能等级"
    /// description = "学习技能等级"
    /// comment = "响应'学习技能'单位事件,指代被学习技能的等级. 注意,该值为学习后的等级."
    public static int GetLearnedSkillLevel()
    {
        return War3.CallNative<int>(_getLearnedSkillLevelPtr);
    }


    /// title = "可复活英雄"
    /// description = "可复活英雄"
    /// comment = "响应'变为可复活的'单位事件."
    public static JUnit GetRevivableUnit()
    {
        var handle = War3.CallNative<int>(_getRevivableUnitPtr);
        return new JUnit(handle);
    }


    /// title = "复活英雄"
    /// description = "复活英雄"
    /// comment = "响应'开始/取消/完成复活'单位事件."
    public static JUnit GetRevivingUnit()
    {
        var handle = War3.CallNative<int>(_getRevivingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "攻击单位"
    /// description = "攻击单位"
    /// comment = "响应'被攻击'单位事件."
    public static JUnit GetAttacker()
    {
        var handle = War3.CallNative<int>(_getAttackerPtr);
        return new JUnit(handle);
    }

    public static JUnit GetRescuer()
    {
        var handle = War3.CallNative<int>(_getRescuerPtr);
        return new JUnit(handle);
    }


    /// title = "死亡单位"
    /// description = "死亡单位"
    /// comment = "响应'死亡'单位事件."
    public static JUnit GetDyingUnit()
    {
        var handle = War3.CallNative<int>(_getDyingUnitPtr);
        return new JUnit(handle);
    }

    public static JUnit GetKillingUnit()
    {
        var handle = War3.CallNative<int>(_getKillingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "腐化的单位"
    /// description = "腐化的单位"
    /// comment = "响应'开始腐化'单位事件."
    public static JUnit GetDecayingUnit()
    {
        var handle = War3.CallNative<int>(_getDecayingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "正在建造的建筑"
    /// description = "正在建造的建筑"
    /// comment = "响应'开始建造建筑'单位事件."
    public static JUnit GetConstructingStructure()
    {
        var handle = War3.CallNative<int>(_getConstructingStructurePtr);
        return new JUnit(handle);
    }


    /// title = "被取消的建筑"
    /// description = "被取消的建筑"
    /// comment = "响应'取消建造建筑'单位事件."
    public static JUnit GetCancelledStructure()
    {
        var handle = War3.CallNative<int>(_getCancelledStructurePtr);
        return new JUnit(handle);
    }


    /// title = "完成的建筑"
    /// description = "完成的建筑"
    /// comment = "响应'完成建造建筑'单位事件."
    public static JUnit GetConstructedStructure()
    {
        var handle = War3.CallNative<int>(_getConstructedStructurePtr);
        return new JUnit(handle);
    }


    /// title = "研究科技的单位"
    /// description = "研究科技的单位"
    /// comment = "响应'开始/取消/完成科技研究'单位事件."
    public static JUnit GetResearchingUnit()
    {
        var handle = War3.CallNative<int>(_getResearchingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "被研究科技"
    /// description = "被研究科技"
    /// comment = "响应'开始/取消/完成科技研究'单位事件."
    public static int GetResearched()
    {
        return War3.CallNative<int>(_getResearchedPtr);
    }


    /// title = "训练单位类型"
    /// description = "训练单位类型"
    /// comment = "响应'开始/取消/完成训练单位'单位事件, 指代所训练的单位类型."
    public static int GetTrainedUnitType()
    {
        return War3.CallNative<int>(_getTrainedUnitTypePtr);
    }


    /// title = "训练单位"
    /// description = "训练单位"
    /// comment = "响应'完成训练单位'单位事件,指代被训练单位."
    public static JUnit GetTrainedUnit()
    {
        var handle = War3.CallNative<int>(_getTrainedUnitPtr);
        return new JUnit(handle);
    }

    public static JUnit GetDetectedUnit()
    {
        var handle = War3.CallNative<int>(_getDetectedUnitPtr);
        return new JUnit(handle);
    }


    /// title = "召唤者"
    /// description = "召唤者"
    /// comment = "响应'召唤单位'单位事件."
    public static JUnit GetSummoningUnit()
    {
        var handle = War3.CallNative<int>(_getSummoningUnitPtr);
        return new JUnit(handle);
    }


    /// title = "召唤单位"
    /// description = "召唤单位"
    /// comment = "响应'召唤单位'单位事件,指代被召唤单位."
    public static JUnit GetSummonedUnit()
    {
        var handle = War3.CallNative<int>(_getSummonedUnitPtr);
        return new JUnit(handle);
    }

    public static JUnit GetTransportUnit()
    {
        var handle = War3.CallNative<int>(_getTransportUnitPtr);
        return new JUnit(handle);
    }

    public static JUnit GetLoadedUnit()
    {
        var handle = War3.CallNative<int>(_getLoadedUnitPtr);
        return new JUnit(handle);
    }


    /// title = "贩卖者"
    /// description = "贩卖者"
    /// comment = "响应'出售单位','出售物品'或'抵押物品'单位事件."
    public static JUnit GetSellingUnit()
    {
        var handle = War3.CallNative<int>(_getSellingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "被贩卖单位"
    /// description = "被贩卖单位"
    /// comment = "响应'出售单位'单位事件."
    public static JUnit GetSoldUnit()
    {
        var handle = War3.CallNative<int>(_getSoldUnitPtr);
        return new JUnit(handle);
    }


    /// title = "购买者"
    /// description = "购买者"
    /// comment = "响应'出售单位','出售物品'或'抵押物品'单位事件."
    public static JUnit GetBuyingUnit()
    {
        var handle = War3.CallNative<int>(_getBuyingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "被售出物品"
    /// description = "被售出物品"
    /// comment = "响应'出售物品'或'抵押物品'单位事件."
    public static JItem GetSoldItem()
    {
        var handle = War3.CallNative<int>(_getSoldItemPtr);
        return new JItem(handle);
    }


    /// title = "被改变所有者的单位"
    /// description = "被改变所有者的单位"
    /// comment = "响应'改变所有者'单位事件."
    public static JUnit GetChangingUnit()
    {
        var handle = War3.CallNative<int>(_getChangingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "原所有者"
    /// description = "原所有者"
    /// comment = "响应'改变所有者'单位事件,指代单位原来的所有者."
    public static JPlayer GetChangingUnitPrevOwner()
    {
        var handle = War3.CallNative<int>(_getChangingUnitPrevOwnerPtr);
        return new JPlayer(handle);
    }


    /// title = "操作物品的单位"
    /// description = "操作物品的单位"
    /// comment = "响应'使用/获得/丢失物品'单位事件."
    public static JUnit GetManipulatingUnit()
    {
        var handle = War3.CallNative<int>(_getManipulatingUnitPtr);
        return new JUnit(handle);
    }


    /// title = "被操作物品"
    /// description = "被操作物品"
    /// comment = "响应'使用/得到/丢弃物品'单位事件."
    public static JItem GetManipulatedItem()
    {
        var handle = War3.CallNative<int>(_getManipulatedItemPtr);
        return new JItem(handle);
    }


    /// title = "发布命令的单位"
    /// description = "发布命令的单位"
    /// comment = "响应'发布命令'单位事件."
    public static JUnit GetOrderedUnit()
    {
        var handle = War3.CallNative<int>(_getOrderedUnitPtr);
        return new JUnit(handle);
    }

    public static int GetIssuedOrderId()
    {
        return War3.CallNative<int>(_getIssuedOrderIdPtr);
    }


    /// title = "命令发布点X坐标 [R]"
    /// description = "命令发布点X坐标"
    /// comment = "用坐标代替点可以省去创建、删除点的麻烦."
    public static float GetOrderPointX()
    {
        return War3.CallNative<float>(_getOrderPointXPtr);
    }


    /// title = "命令发布点Y坐标 [R]"
    /// description = "命令发布点Y坐标"
    /// comment = "用坐标代替点可以省去创建、删除点的麻烦."
    public static float GetOrderPointY()
    {
        return War3.CallNative<float>(_getOrderPointYPtr);
    }


    /// title = "命令发布点"
    /// description = "命令发布点"
    /// comment = "响应'发布指定点目标命令'单位事件. 会创建点."
    public static JLocation GetOrderPointLoc()
    {
        var handle = War3.CallNative<int>(_getOrderPointLocPtr);
        return new JLocation(handle);
    }

    public static JWidget GetOrderTarget()
    {
        var handle = War3.CallNative<int>(_getOrderTargetPtr);
        return new JWidget(handle);
    }


    /// title = "命令发布目标(可破坏物)"
    /// description = "命令发布目标"
    /// comment = "响应'发布指定物体目标命令'单位事件并以可破坏物为目标时."
    public static JDestructable GetOrderTargetDestructable()
    {
        var handle = War3.CallNative<int>(_getOrderTargetDestructablePtr);
        return new JDestructable(handle);
    }


    /// title = "命令发布目标"
    /// description = "命令发布目标"
    /// comment = "响应'发布指定物体目标命令'单位事件并以物品为目标时."
    public static JItem GetOrderTargetItem()
    {
        var handle = War3.CallNative<int>(_getOrderTargetItemPtr);
        return new JItem(handle);
    }


    /// title = "命令发布目标"
    /// description = "命令发布目标"
    /// comment = "响应'发布指定物体目标命令'单位事件并以单位为目标时."
    public static JUnit GetOrderTargetUnit()
    {
        var handle = War3.CallNative<int>(_getOrderTargetUnitPtr);
        return new JUnit(handle);
    }


    /// title = "施法单位"
    /// description = "施法单位"
    /// comment = "响应'施放技能'单位事件."
    public static JUnit GetSpellAbilityUnit()
    {
        var handle = War3.CallNative<int>(_getSpellAbilityUnitPtr);
        return new JUnit(handle);
    }


    /// title = "施放技能"
    /// description = "施放技能"
    /// comment = "响应施放技能单位事件, 指代被施放的技能."
    public static int GetSpellAbilityId()
    {
        return War3.CallNative<int>(_getSpellAbilityIdPtr);
    }

    public static JAbility GetSpellAbility()
    {
        var handle = War3.CallNative<int>(_getSpellAbilityPtr);
        return new JAbility(handle);
    }


    /// title = "技能施放点"
    /// description = "技能施放点"
    /// comment = "响应'技能施放'单位事件. 注意技能施放结束将无法捕获该点. 会创建点."
    public static JLocation GetSpellTargetLoc()
    {
        var handle = War3.CallNative<int>(_getSpellTargetLocPtr);
        return new JLocation(handle);
    }


    /// title = "技能施放点X坐标"
    /// description = "获取技能目标点的X坐标"
    /// comment = "这是1.24的函数，但已加入函数库，在1.20也可以使用。"
    public static float GetSpellTargetX()
    {
        return War3.CallNative<float>(_getSpellTargetXPtr);
    }


    /// title = "技能施放点Y坐标"
    /// description = "获取技能目标点的Y坐标"
    /// comment = "这是1.24的函数，但已加入函数库，在1.20也可以使用。"
    public static float GetSpellTargetY()
    {
        return War3.CallNative<float>(_getSpellTargetYPtr);
    }


    /// title = "技能施放目标(可破坏物)"
    /// description = "技能施放目标"
    /// comment = "响应'施放技能'单位事件并以可破坏物为目标时. 注意: 技能施放结束将无法捕获施放目标."
    public static JDestructable GetSpellTargetDestructable()
    {
        var handle = War3.CallNative<int>(_getSpellTargetDestructablePtr);
        return new JDestructable(handle);
    }


    /// title = "技能施放目标"
    /// description = "技能施放目标"
    /// comment = "响应施放技能单位事件并以物品为目标时. 注意: 技能施放结束将无法捕获施放目标."
    public static JItem GetSpellTargetItem()
    {
        var handle = War3.CallNative<int>(_getSpellTargetItemPtr);
        return new JItem(handle);
    }


    /// title = "技能施放目标"
    /// description = "技能施放目标"
    /// comment = "响应'施放技能'单位事件并以单位为目标时. 注意: 技能施放结束将无法捕获施放目标."
    public static JUnit GetSpellTargetUnit()
    {
        var handle = War3.CallNative<int>(_getSpellTargetUnitPtr);
        return new JUnit(handle);
    }


    /// title = "联盟状态更改(指定项目)"
    /// description = "${Player} 更改 ${Alliance Type} 设置"
    /// comment = "当改变项目为【共享单位】时，(触发玩家)会不生效，此时不建议使用【任意玩家】事件。"
    public static JEvent TriggerRegisterPlayerAllianceChange(JTrigger whichTrigger, JPlayer whichPlayer,
        JAllianceType whichAlliance)
    {
        var handle = War3.CallNative<int>(_triggerRegisterPlayerAllianceChangePtr, whichTrigger.Handle,
            whichPlayer.Handle, whichAlliance.Handle);
        return new JEvent(handle);
    }


    /// title = "属性事件"
    /// description = "${玩家} 的 ${Property} 属性 ${Operation} ${值}"
    /// comment = ""
    public static JEvent TriggerRegisterPlayerStateEvent(JTrigger whichTrigger, JPlayer whichPlayer,
        JPlayerState whichState, JLimitOp opcode, float limitval)
    {
        var handle = War3.CallNative<int>(_triggerRegisterPlayerStateEventPtr, whichTrigger.Handle, whichPlayer.Handle,
            whichState.Handle, opcode.Handle, limitval);
        return new JEvent(handle);
    }

    public static JPlayerState GetEventPlayerState()
    {
        var handle = War3.CallNative<int>(_getEventPlayerStatePtr);
        return new JPlayerState(handle);
    }


    /// title = "输入聊天信息"
    /// description = "${玩家} 输入 ${Text} ,信息过滤方式 ${Match Type}"
    /// comment = "事件ID是(096)"
    public static JEvent TriggerRegisterPlayerChatEvent(JTrigger whichTrigger, JPlayer whichPlayer,
        string chatMessageToDetect, bool exactMatchOnly)
    {
        var handle = War3.CallNative<int>(_triggerRegisterPlayerChatEventPtr, whichTrigger.Handle, whichPlayer.Handle,
            chatMessageToDetect, exactMatchOnly);
        return new JEvent(handle);
    }


    /// title = "输入的聊天信息"
    /// description = "输入的聊天信息"
    /// comment = ""
    public static string GetEventPlayerChatString()
    {
        return War3.CallNative<string>(_getEventPlayerChatStringPtr);
    }


    /// title = "匹配的聊天信息"
    /// description = "匹配的聊天信息"
    /// comment = ""
    public static string GetEventPlayerChatStringMatched()
    {
        return War3.CallNative<string>(_getEventPlayerChatStringMatchedPtr);
    }


    /// title = "可破坏物死亡"
    /// description = "${可破坏物} 死亡"
    /// comment = "使用'事件响应 - 死亡的可破坏物'来获取死亡物体."
    public static JEvent TriggerRegisterDeathEvent(JTrigger whichTrigger, JWidget whichWidget)
    {
        var handle = War3.CallNative<int>(_triggerRegisterDeathEventPtr, whichTrigger.Handle, whichWidget.Handle);
        return new JEvent(handle);
    }


    /// title = "触发单位"
    /// description = "触发单位"
    /// comment = ""
    public static JUnit GetTriggerUnit()
    {
        var handle = War3.CallNative<int>(_getTriggerUnitPtr);
        return new JUnit(handle);
    }

    public static JEvent TriggerRegisterUnitStateEvent(JTrigger whichTrigger, JUnit whichUnit, JUnitState whichState,
        JLimitOp opcode, float limitval)
    {
        var handle = War3.CallNative<int>(_triggerRegisterUnitStateEventPtr, whichTrigger.Handle, whichUnit.Handle,
            whichState.Handle, opcode.Handle, limitval);
        return new JEvent(handle);
    }

    public static JUnitState GetEventUnitState()
    {
        var handle = War3.CallNative<int>(_getEventUnitStatePtr);
        return new JUnitState(handle);
    }


    /// title = "指定单位事件"
    /// description = "${指定单位} ${事件}"
    /// comment = ""
    public static JEvent TriggerRegisterUnitEvent(JTrigger whichTrigger, JUnit whichUnit, JUnitEvent whichEvent)
    {
        var handle = War3.CallNative<int>(_triggerRegisterUnitEventPtr, whichTrigger.Handle, whichUnit.Handle,
            whichEvent.Handle);
        return new JEvent(handle);
    }


    /// title = "伤害值"
    /// description = "单位所受伤害"
    /// comment = "响应'受到伤害'单位事件,指代单位所受伤害."
    public static float GetEventDamage()
    {
        return War3.CallNative<float>(_getEventDamagePtr);
    }


    /// title = "伤害来源"
    /// description = "伤害来源"
    /// comment = "响应'受到伤害'单位事件."
    public static JUnit GetEventDamageSource()
    {
        var handle = War3.CallNative<int>(_getEventDamageSourcePtr);
        return new JUnit(handle);
    }

    public static JPlayer GetEventDetectingPlayer()
    {
        var handle = War3.CallNative<int>(_getEventDetectingPlayerPtr);
        return new JPlayer(handle);
    }

    public static JEvent TriggerRegisterFilterUnitEvent(JTrigger whichTrigger, JUnit whichUnit, JUnitEvent whichEvent,
        JBoolExpr filter)
    {
        var handle = War3.CallNative<int>(_triggerRegisterFilterUnitEventPtr, whichTrigger.Handle, whichUnit.Handle,
            whichEvent.Handle, filter.Handle);
        return new JEvent(handle);
    }


    /// title = "事件目标单位"
    /// description = "事件目标单位"
    /// comment = "响应'注意到/获取攻击目标'单位事件,指代目标单位."
    public static JUnit GetEventTargetUnit()
    {
        var handle = War3.CallNative<int>(_getEventTargetUnitPtr);
        return new JUnit(handle);
    }

    public static JEvent TriggerRegisterUnitInRange(JTrigger whichTrigger, JUnit whichUnit, float range,
        JBoolExpr filter)
    {
        var handle = War3.CallNative<int>(_triggerRegisterUnitInRangePtr, whichTrigger.Handle, whichUnit.Handle, range,
            filter.Handle);
        return new JEvent(handle);
    }

    public static JTriggerCondition TriggerAddCondition(JTrigger whichTrigger, JBoolExpr condition)
    {
        var handle = War3.CallNative<int>(_triggerAddConditionPtr, whichTrigger.Handle, condition.Handle);
        return new JTriggerCondition(handle);
    }

    public static void TriggerRemoveCondition(JTrigger whichTrigger, JTriggerCondition whichCondition)
    {
        War3.CallNative(_triggerRemoveConditionPtr, whichTrigger.Handle, whichCondition.Handle);
    }

    public static void TriggerClearConditions(JTrigger whichTrigger)
    {
        War3.CallNative(_triggerClearConditionsPtr, whichTrigger.Handle);
    }

    public static JTriggerAction TriggerAddAction(JTrigger whichTrigger, Action actionFunc)
    {
        var handle = War3.CallNative<int>(_triggerAddActionPtr, whichTrigger.Handle, actionFunc);
        return new JTriggerAction(handle);
    }

    public static void TriggerRemoveAction(JTrigger whichTrigger, JTriggerAction whichAction)
    {
        War3.CallNative(_triggerRemoveActionPtr, whichTrigger.Handle, whichAction.Handle);
    }

    public static void TriggerClearActions(JTrigger whichTrigger)
    {
        War3.CallNative(_triggerClearActionsPtr, whichTrigger.Handle);
    }


    /// title = "等待(玩家时间)"
    /// description = "等待 ${Time} 秒"
    /// comment = "该延迟功能受真实时间的影响(也就是玩家机器上的时间). 因此每个玩家所延迟的时间可能不一致."
    public static void TriggerSleepAction(float timeout)
    {
        War3.CallNative(_triggerSleepActionPtr, timeout);
    }

    public static void TriggerWaitForSound(JSound s, float offset)
    {
        War3.CallNative(_triggerWaitForSoundPtr, s.Handle, offset);
    }


    /// title = "触发条件成立"
    /// description = "${触发} 的条件成立"
    /// comment = ""
    public static bool TriggerEvaluate(JTrigger whichTrigger)
    {
        return War3.CallNative<bool>(_triggerEvaluatePtr, whichTrigger.Handle);
    }


    /// title = "运行触发(无视条件)"
    /// description = "运行 ${触发} (无视条件)"
    /// comment = "无视事件和条件,运行触发动作."
    public static void TriggerExecute(JTrigger whichTrigger)
    {
        War3.CallNative(_triggerExecutePtr, whichTrigger.Handle);
    }

    public static void TriggerExecuteWait(JTrigger whichTrigger)
    {
        War3.CallNative(_triggerExecuteWaitPtr, whichTrigger.Handle);
    }

    public static void TriggerSyncStart()
    {
        War3.CallNative(_triggerSyncStartPtr);
    }

    public static void TriggerSyncReady()
    {
        War3.CallNative(_triggerSyncReadyPtr);
    }

    public static float GetWidgetLife(JWidget whichWidget)
    {
        return War3.CallNative<float>(_getWidgetLifePtr, whichWidget.Handle);
    }

    public static void SetWidgetLife(JWidget whichWidget, float newLife)
    {
        War3.CallNative(_setWidgetLifePtr, whichWidget.Handle, newLife);
    }

    public static float GetWidgetX(JWidget whichWidget)
    {
        return War3.CallNative<float>(_getWidgetXPtr, whichWidget.Handle);
    }

    public static float GetWidgetY(JWidget whichWidget)
    {
        return War3.CallNative<float>(_getWidgetYPtr, whichWidget.Handle);
    }

    public static JWidget GetTriggerWidget()
    {
        var handle = War3.CallNative<int>(_getTriggerWidgetPtr);
        return new JWidget(handle);
    }

    public static JDestructable CreateDestructable(int objectid, float x, float y, float face, float scale,
        int variation)
    {
        var handle = War3.CallNative<int>(_createDestructablePtr, objectid, x, y, face, scale, variation);
        return new JDestructable(handle);
    }


    /// title = "新建可破坏物 [R]"
    /// description = "新建的 ${可破坏物类型} 在(${X},${Y},${Z}),面向角度: ${Direction} 尺寸缩放: ${Scale} 样式: ${Variation}"
    /// comment = "坐标为(X,Y,Z)格式. 面向角度采用角度制,0度为正东方向,90度为正北方向."
    public static JDestructable CreateDestructableZ(int objectid, float x, float y, float z, float face, float scale,
        int variation)
    {
        var handle = War3.CallNative<int>(_createDestructableZPtr, objectid, x, y, z, face, scale, variation);
        return new JDestructable(handle);
    }

    public static JDestructable CreateDeadDestructable(int objectid, float x, float y, float face, float scale,
        int variation)
    {
        var handle = War3.CallNative<int>(_createDeadDestructablePtr, objectid, x, y, face, scale, variation);
        return new JDestructable(handle);
    }


    /// title = "新建可破坏物(死亡的) [R]"
    /// description = "新建死亡的 ${可破坏物类型} 在(${X},${Y},${Z\"),面向角度: \"}${Direction} 尺寸缩放: ${Scale} 样式: ${Variation}"
    /// comment = "坐标为(X,Y,Z)格式. 面向角度采用角度制,0度为正东方向,90度为正北方向."
    public static JDestructable CreateDeadDestructableZ(int objectid, float x, float y, float z, float face,
        float scale, int variation)
    {
        var handle = War3.CallNative<int>(_createDeadDestructableZPtr, objectid, x, y, z, face, scale, variation);
        return new JDestructable(handle);
    }


    /// title = "删除"
    /// description = "删除 ${可破坏物}"
    /// comment = ""
    public static void RemoveDestructable(JDestructable d)
    {
        War3.CallNative(_removeDestructablePtr, d.Handle);
    }


    /// title = "杀死"
    /// description = "杀死 ${可破坏物}"
    /// comment = ""
    public static void KillDestructable(JDestructable d)
    {
        War3.CallNative(_killDestructablePtr, d.Handle);
    }

    public static void SetDestructableInvulnerable(JDestructable d, bool flag)
    {
        War3.CallNative(_setDestructableInvulnerablePtr, d.Handle, flag);
    }

    public static bool IsDestructableInvulnerable(JDestructable d)
    {
        return War3.CallNative<bool>(_isDestructableInvulnerablePtr, d.Handle);
    }

    public static void EnumDestructablesInRect(JRect r, JBoolExpr filter, Action actionFunc)
    {
        War3.CallNative(_enumDestructablesInRectPtr, r.Handle, filter.Handle, actionFunc);
    }


    /// title = "指定可破坏物的类型"
    /// description = "${可破坏物} 的类型"
    /// comment = ""
    public static int GetDestructableTypeId(JDestructable d)
    {
        return War3.CallNative<int>(_getDestructableTypeIdPtr, d.Handle);
    }


    /// title = "可破坏物所在X轴坐标 [R]"
    /// description = "${可破坏物} 所在X轴坐标"
    /// comment = ""
    public static float GetDestructableX(JDestructable d)
    {
        return War3.CallNative<float>(_getDestructableXPtr, d.Handle);
    }


    /// title = "可破坏物所在Y轴坐标 [R]"
    /// description = "${可破坏物} 所在Y轴坐标"
    public static float GetDestructableY(JDestructable d)
    {
        return War3.CallNative<float>(_getDestructableYPtr, d.Handle);
    }


    /// title = "设置生命值(指定值)"
    /// description = "设置 ${可破坏物} 的生命值为 ${Value}"
    /// comment = ""
    public static void SetDestructableLife(JDestructable d, float life)
    {
        War3.CallNative(_setDestructableLifePtr, d.Handle, life);
    }


    /// title = "生命值"
    /// description = "${可破坏物} 的当前生命值"
    /// comment = ""
    public static float GetDestructableLife(JDestructable d)
    {
        return War3.CallNative<float>(_getDestructableLifePtr, d.Handle);
    }

    public static void SetDestructableMaxLife(JDestructable d, float max)
    {
        War3.CallNative(_setDestructableMaxLifePtr, d.Handle, max);
    }


    /// title = "最大生命值"
    /// description = "${可破坏物} 的最大生命值"
    /// comment = ""
    public static float GetDestructableMaxLife(JDestructable d)
    {
        return War3.CallNative<float>(_getDestructableMaxLifePtr, d.Handle);
    }


    /// title = "复活"
    /// description = "复活 ${Destructible} ,设置生命值为 ${Value} 并 ${Show/Hide} 生长动画"
    /// comment = ""
    public static void DestructableRestoreLife(JDestructable d, float life, bool birth)
    {
        War3.CallNative(_destructableRestoreLifePtr, d.Handle, life, birth);
    }

    public static void QueueDestructableAnimation(JDestructable d, string whichAnimation)
    {
        War3.CallNative(_queueDestructableAnimationPtr, d.Handle, whichAnimation);
    }

    public static void SetDestructableAnimation(JDestructable d, string whichAnimation)
    {
        War3.CallNative(_setDestructableAnimationPtr, d.Handle, whichAnimation);
    }


    /// title = "改变可破坏物动画播放速度 [R]"
    /// description = "改变 ${可破坏物} 的动画播放速度为正常的 ${Percent}倍"
    /// comment = "设置1倍动画播放速度来恢复正常状态."
    public static void SetDestructableAnimationSpeed(JDestructable d, float speedFactor)
    {
        War3.CallNative(_setDestructableAnimationSpeedPtr, d.Handle, speedFactor);
    }


    /// title = "显示/隐藏 [R]"
    /// description = "设置 ${可破坏物} 的状态为 ${Show/Hide}"
    /// comment = "隐藏的可破坏物不被显示,但仍影响通行和视线."
    public static void ShowDestructable(JDestructable d, bool flag)
    {
        War3.CallNative(_showDestructablePtr, d.Handle, flag);
    }


    /// title = "闭塞高度"
    /// description = "${可破坏物} 的闭塞高度"
    /// comment = ""
    public static float GetDestructableOccluderHeight(JDestructable d)
    {
        return War3.CallNative<float>(_getDestructableOccluderHeightPtr, d.Handle);
    }


    /// title = "设置闭塞高度"
    /// description = "设置 ${可破坏物} 的闭塞高度为 ${Height}"
    /// comment = ""
    public static void SetDestructableOccluderHeight(JDestructable d, float height)
    {
        War3.CallNative(_setDestructableOccluderHeightPtr, d.Handle, height);
    }


    /// title = "物件名字"
    /// description = "${物件} 的名字"
    /// comment = ""
    public static string GetDestructableName(JDestructable d)
    {
        return War3.CallNative<string>(_getDestructableNamePtr, d.Handle);
    }

    public static JDestructable GetTriggerDestructable()
    {
        var handle = War3.CallNative<int>(_getTriggerDestructablePtr);
        return new JDestructable(handle);
    }


    /// title = "新建物品 [R]"
    /// description = "新建 ${物品} 在(${X},${Y})"
    /// comment = ""
    public static JItem CreateItem(int itemid, float x, float y)
    {
        var handle = War3.CallNative<int>(_createItemPtr, itemid, x, y);
        return new JItem(handle);
    }


    /// title = "删除"
    /// description = "删除 ${物品}"
    /// comment = ""
    public static void RemoveItem(JItem whichItem)
    {
        War3.CallNative(_removeItemPtr, whichItem.Handle);
    }


    /// title = "物品所属玩家"
    /// description = "${物品} 的所属玩家"
    /// comment = "与持有者无关,默认为中立被动玩家."
    public static JPlayer GetItemPlayer(JItem whichItem)
    {
        var handle = War3.CallNative<int>(_getItemPlayerPtr, whichItem.Handle);
        return new JPlayer(handle);
    }


    /// title = "指定物品的类型"
    /// description = "${物品} 的类型"
    /// comment = ""
    public static int GetItemTypeId(JItem i)
    {
        return War3.CallNative<int>(_getItemTypeIdPtr, i.Handle);
    }


    /// title = "物品的X轴坐标 [R]"
    /// description = "${物品} 的X轴坐标"
    /// comment = ""
    public static float GetItemX(JItem i)
    {
        return War3.CallNative<float>(_getItemXPtr, i.Handle);
    }


    /// title = "物品的Y轴坐标 [R]"
    /// description = "${物品} 的Y轴坐标"
    /// comment = ""
    public static float GetItemY(JItem i)
    {
        return War3.CallNative<float>(_getItemYPtr, i.Handle);
    }


    /// title = "移动物品到坐标(立即)(指定坐标) [R]"
    /// description = "移动 ${物品} 到(${X},${Y})"
    /// comment = ""
    public static void SetItemPosition(JItem i, float x, float y)
    {
        War3.CallNative(_setItemPositionPtr, i.Handle, x, y);
    }

    public static void SetItemDropOnDeath(JItem whichItem, bool flag)
    {
        War3.CallNative(_setItemDropOnDeathPtr, whichItem.Handle, flag);
    }

    public static void SetItemDroppable(JItem i, bool flag)
    {
        War3.CallNative(_setItemDroppablePtr, i.Handle, flag);
    }


    /// title = "设置物品可否抵押"
    /// description = "设置 ${物品} ${Pawnable/Unpawnable}"
    /// comment = "不可抵押物品不能被卖到商店."
    public static void SetItemPawnable(JItem i, bool flag)
    {
        War3.CallNative(_setItemPawnablePtr, i.Handle, flag);
    }

    public static void SetItemPlayer(JItem whichItem, JPlayer whichPlayer, bool changeColor)
    {
        War3.CallNative(_setItemPlayerPtr, whichItem.Handle, whichPlayer.Handle, changeColor);
    }

    public static void SetItemInvulnerable(JItem whichItem, bool flag)
    {
        War3.CallNative(_setItemInvulnerablePtr, whichItem.Handle, flag);
    }


    /// title = "物品无敌"
    /// description = "${物品} 是无敌的"
    /// comment = ""
    public static bool IsItemInvulnerable(JItem whichItem)
    {
        return War3.CallNative<bool>(_isItemInvulnerablePtr, whichItem.Handle);
    }


    /// title = "显示/隐藏 [R]"
    /// description = "设置 ${物品} 的状态为: ${Show/Hide}"
    /// comment = "只对在地面的物品有效,不会影响在物品栏中的物品. 单位通过触发得到一个隐藏物品时,会自动显示该物品."
    public static void SetItemVisible(JItem whichItem, bool show)
    {
        War3.CallNative(_setItemVisiblePtr, whichItem.Handle, show);
    }


    /// title = "物品可见 [R]"
    /// description = "${物品} 是可见的"
    /// comment = "物品不被隐藏且不被单位持有时即为可见的."
    public static bool IsItemVisible(JItem whichItem)
    {
        return War3.CallNative<bool>(_isItemVisiblePtr, whichItem.Handle);
    }


    /// title = "物品被持有"
    /// description = "${物品} 是被持有的"
    /// comment = "在物品栏中的物品都是被持有的. 就算单位正处于死亡状态."
    public static bool IsItemOwned(JItem whichItem)
    {
        return War3.CallNative<bool>(_isItemOwnedPtr, whichItem.Handle);
    }


    /// title = "物品是拾取时自动使用的 [R]"
    /// description = "${物品} 是拾取时自动使用类物品"
    /// comment = ""
    public static bool IsItemPowerup(JItem whichItem)
    {
        return War3.CallNative<bool>(_isItemPowerupPtr, whichItem.Handle);
    }


    /// title = "物品可被市场随机出售 [R]"
    /// description = "${物品} 可被市场随机出售"
    /// comment = ""
    public static bool IsItemSellable(JItem whichItem)
    {
        return War3.CallNative<bool>(_isItemSellablePtr, whichItem.Handle);
    }


    /// title = "物品可被抵押 [R]"
    /// description = "${物品} 可被抵押"
    /// comment = ""
    public static bool IsItemPawnable(JItem whichItem)
    {
        return War3.CallNative<bool>(_isItemPawnablePtr, whichItem.Handle);
    }

    public static bool IsItemIdPowerup(int itemId)
    {
        return War3.CallNative<bool>(_isItemIdPowerupPtr, itemId);
    }

    public static bool IsItemIdSellable(int itemId)
    {
        return War3.CallNative<bool>(_isItemIdSellablePtr, itemId);
    }

    public static bool IsItemIdPawnable(int itemId)
    {
        return War3.CallNative<bool>(_isItemIdPawnablePtr, itemId);
    }

    public static void EnumItemsInRect(JRect r, JBoolExpr filter, Action actionFunc)
    {
        War3.CallNative(_enumItemsInRectPtr, r.Handle, filter.Handle, actionFunc);
    }


    /// title = "物品等级"
    /// description = "${物品} 的物品等级"
    /// comment = ""
    public static int GetItemLevel(JItem whichItem)
    {
        return War3.CallNative<int>(_getItemLevelPtr, whichItem.Handle);
    }


    /// title = "指定物品的分类"
    /// description = "${物品} 的分类"
    /// comment = ""
    public static JItemType GetItemType(JItem whichItem)
    {
        var handle = War3.CallNative<int>(_getItemTypePtr, whichItem.Handle);
        return new JItemType(handle);
    }


    /// title = "设置重生神符的产生单位类型"
    /// description = "设置 ${物品} 产生 ${单位类型}"
    /// comment = "设置重生神符对应的单位类型后，当英雄吃了重生神符，则会产生指定类型的单位。"
    public static void SetItemDropID(JItem whichItem, int unitId)
    {
        War3.CallNative(_setItemDropIDPtr, whichItem.Handle, unitId);
    }


    /// title = "物品名字"
    /// description = "${物品} 的名字"
    /// comment = ""
    public static string GetItemName(JItem whichItem)
    {
        return War3.CallNative<string>(_getItemNamePtr, whichItem.Handle);
    }


    /// title = "使用次数"
    /// description = "${物品} 的使用次数"
    /// comment = "无限使用物品将返回0."
    public static int GetItemCharges(JItem whichItem)
    {
        return War3.CallNative<int>(_getItemChargesPtr, whichItem.Handle);
    }


    /// title = "设置物品使用次数"
    /// description = "设置 ${物品} 的使用次数为 ${Charges}"
    /// comment = "设置为0可以使物品能无限次使用."
    public static void SetItemCharges(JItem whichItem, int charges)
    {
        War3.CallNative(_setItemChargesPtr, whichItem.Handle, charges);
    }


    /// title = "物品自定义值"
    /// description = "${物品} 的自定义值"
    /// comment = "可以使用'物品 - 设置自定义值'来设置该值."
    public static int GetItemUserData(JItem whichItem)
    {
        return War3.CallNative<int>(_getItemUserDataPtr, whichItem.Handle);
    }


    /// title = "设置物品自定义值"
    /// description = "设置 ${物品} 的自定义值为 ${Index}"
    /// comment = "物品自定义值只用于触发器. 可以用来为物品绑定一个整型数据."
    public static void SetItemUserData(JItem whichItem, int data)
    {
        War3.CallNative(_setItemUserDataPtr, whichItem.Handle, data);
    }


    /// title = "新建单位(指定坐标) [R]"
    /// description = "新建 ${玩家} 的 ${单位} 在(${X},${Y}),面向角度:${Face} 度"
    /// comment = "在坐标创建单位，不能被'最后创建的单位'获得。"
    public static JUnit CreateUnit(JPlayer id, int unitid, float x, float y, float face)
    {
        var handle = War3.CallNative<int>(_createUnitPtr, id.Handle, unitid, x, y, face);
        return new JUnit(handle);
    }

    public static JUnit CreateUnitByName(JPlayer whichPlayer, string unitname, float x, float y, float face)
    {
        var handle = War3.CallNative<int>(_createUnitByNamePtr, whichPlayer.Handle, unitname, x, y, face);
        return new JUnit(handle);
    }


    /// title = "新建单位(指定点) [R]"
    /// description = "新建 ${玩家} 的 ${单位} 在 ${点} 面向角度:${Face} 度"
    /// comment = ""
    public static JUnit CreateUnitAtLoc(JPlayer id, int unitid, JLocation whichLocation, float face)
    {
        var handle = War3.CallNative<int>(_createUnitAtLocPtr, id.Handle, unitid, whichLocation.Handle, face);
        return new JUnit(handle);
    }

    public static JUnit CreateUnitAtLocByName(JPlayer id, string unitname, JLocation whichLocation, float face)
    {
        var handle = War3.CallNative<int>(_createUnitAtLocByNamePtr, id.Handle, unitname, whichLocation.Handle, face);
        return new JUnit(handle);
    }


    /// title = "新建尸体 [R]"
    /// description = "新建 ${玩家} 的 ${单位} 的尸体在(${X},${Y}),面向角度:${Face} 度"
    /// comment = ""
    public static JUnit CreateCorpse(JPlayer whichPlayer, int unitid, float x, float y, float face)
    {
        var handle = War3.CallNative<int>(_createCorpsePtr, whichPlayer.Handle, unitid, x, y, face);
        return new JUnit(handle);
    }


    /// title = "杀死"
    /// description = "杀死 ${单位}"
    /// comment = ""
    public static void KillUnit(JUnit whichUnit)
    {
        War3.CallNative(_killUnitPtr, whichUnit.Handle);
    }


    /// title = "删除"
    /// description = "删除 ${单位}"
    /// comment = "被删除的单位不会留下尸体. 如果是英雄则不能再被复活."
    public static void RemoveUnit(JUnit whichUnit)
    {
        War3.CallNative(_removeUnitPtr, whichUnit.Handle);
    }


    /// title = "显示/隐藏 [R]"
    /// description = "设置 ${单位} 的状态为 ${显示/隐藏}"
    /// comment = "隐藏单位不会被'区域内单位'所选取."
    public static void ShowUnit(JUnit whichUnit, bool show)
    {
        War3.CallNative(_showUnitPtr, whichUnit.Handle, show);
    }


    /// title = "设置单位属性 [R]"
    /// description = "设置 ${单位} 的 ${属性} 为 ${Value}"
    /// comment = ""
    public static void SetUnitState(JUnit whichUnit, JUnitState whichUnitState, float newVal)
    {
        War3.CallNative(_setUnitStatePtr, whichUnit.Handle, whichUnitState.Handle, newVal);
    }


    /// title = "设置X坐标 [R]"
    /// description = "设置 ${单位} 的X坐标为 ${X}"
    /// comment = "注意如果坐标超出地图边界是会出错的."
    public static void SetUnitX(JUnit whichUnit, float newX)
    {
        War3.CallNative(_setUnitXPtr, whichUnit.Handle, newX);
    }


    /// title = "设置Y坐标 [R]"
    /// description = "设置 ${单位} 的Y坐标为 ${Y}"
    /// comment = "注意如果坐标超出地图边界是会出错的."
    public static void SetUnitY(JUnit whichUnit, float newY)
    {
        War3.CallNative(_setUnitYPtr, whichUnit.Handle, newY);
    }


    /// title = "移动单位(立即)(指定坐标) [R]"
    /// description = "立即移动 ${单位} 到(${X},${Y})"
    /// comment = ""
    public static void SetUnitPosition(JUnit whichUnit, float newX, float newY)
    {
        War3.CallNative(_setUnitPositionPtr, whichUnit.Handle, newX, newY);
    }


    /// title = "移动单位(立即)(指定点)"
    /// description = "立即移动 ${单位} 到 ${指定点}"
    /// comment = ""
    public static void SetUnitPositionLoc(JUnit whichUnit, JLocation whichLocation)
    {
        War3.CallNative(_setUnitPositionLocPtr, whichUnit.Handle, whichLocation.Handle);
    }


    /// title = "设置单位面向角度 [R]"
    /// description = "设置 ${单位} 的面向角度为 ${Angle} 度"
    /// comment = "面向角度采用角度制,0度为正东方向,90度为正北方向。速度等于单位的转身速度。"
    public static void SetUnitFacing(JUnit whichUnit, float facingAngle)
    {
        War3.CallNative(_setUnitFacingPtr, whichUnit.Handle, facingAngle);
    }


    /// title = "设置单位面向角度(指定时间)"
    /// description = "设置 ${单位} 的面向角度为 ${Angle} 度,使用时间 ${Time} 秒"
    /// comment = "面向角度采用角度制,0度为正东方向,90度为正北方向。不能超过单位的转身速度。"
    public static void SetUnitFacingTimed(JUnit whichUnit, float facingAngle, float duration)
    {
        War3.CallNative(_setUnitFacingTimedPtr, whichUnit.Handle, facingAngle, duration);
    }


    /// title = "设置移动速度"
    /// description = "设置 ${单位} 的移动速度为 ${Speed}"
    /// comment = ""
    public static void SetUnitMoveSpeed(JUnit whichUnit, float newSpeed)
    {
        War3.CallNative(_setUnitMoveSpeedPtr, whichUnit.Handle, newSpeed);
    }

    public static void SetUnitFlyHeight(JUnit whichUnit, float newHeight, float rate)
    {
        War3.CallNative(_setUnitFlyHeightPtr, whichUnit.Handle, newHeight, rate);
    }

    public static void SetUnitTurnSpeed(JUnit whichUnit, float newTurnSpeed)
    {
        War3.CallNative(_setUnitTurnSpeedPtr, whichUnit.Handle, newTurnSpeed);
    }


    /// title = "改变单位转向角度(弧度制) [R]"
    /// description = "改变 ${单位} 的转向角度为 ${数值} (弧度制)"
    /// comment = "设置单位转身时的转向角度. 数值越大转向幅度越大. "
    public static void SetUnitPropWindow(JUnit whichUnit, float newPropWindowAngle)
    {
        War3.CallNative(_setUnitPropWindowPtr, whichUnit.Handle, newPropWindowAngle);
    }

    public static void SetUnitAcquireRange(JUnit whichUnit, float newAcquireRange)
    {
        War3.CallNative(_setUnitAcquireRangePtr, whichUnit.Handle, newAcquireRange);
    }


    /// title = "锁定指定单位的警戒点 [R]"
    /// description = "设置 ${单位} 的警戒点: ${option}"
    /// comment = "锁定并防止 AI 脚本改动单位警戒点."
    public static void SetUnitCreepGuard(JUnit whichUnit, bool creepGuard)
    {
        War3.CallNative(_setUnitCreepGuardPtr, whichUnit.Handle, creepGuard);
    }


    /// title = "当前主动攻击范围"
    /// description = "${单位} 的当前主动攻击范围"
    /// comment = ""
    public static float GetUnitAcquireRange(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitAcquireRangePtr, whichUnit.Handle);
    }


    /// title = "当前转身速度"
    /// description = "${单位} 的当前转身速度"
    /// comment = "转身速度表示单位改变面向方向时的速度. 数值越小表示转身越慢."
    public static float GetUnitTurnSpeed(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitTurnSpeedPtr, whichUnit.Handle);
    }


    /// title = "当前转向角度(弧度制) [R]"
    /// description = "${单位} 的当前转向角度(弧度制)"
    /// comment = "单位转身时的转向角度. 数值越大转向幅度越大. "
    public static float GetUnitPropWindow(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitPropWindowPtr, whichUnit.Handle);
    }


    /// title = "当前飞行高度"
    /// description = "${单位} 的当前飞行高度"
    /// comment = "飞行单位可以直接改变飞行高度. 其他单位通过添加/删除 替换为飞行单位的变身技能(如乌鸦形态)之后,也能改变飞行高度."
    public static float GetUnitFlyHeight(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitFlyHeightPtr, whichUnit.Handle);
    }


    /// title = "默认主动攻击范围"
    /// description = "${单位} 的默认主动攻击范围"
    /// comment = ""
    public static float GetUnitDefaultAcquireRange(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitDefaultAcquireRangePtr, whichUnit.Handle);
    }


    /// title = "默认转身速度"
    /// description = "${单位} 的默认转身速度"
    /// comment = "转身速度表示单位改变面向方向时的速度. 数值越小表示转身越慢."
    public static float GetUnitDefaultTurnSpeed(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitDefaultTurnSpeedPtr, whichUnit.Handle);
    }

    public static float GetUnitDefaultPropWindow(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitDefaultPropWindowPtr, whichUnit.Handle);
    }


    /// title = "默认飞行高度"
    /// description = "${单位} 的默认飞行高度"
    /// comment = "飞行单位可以直接改变飞行高度. 其他单位通过添加/删除 替换为飞行单位的变身技能(如乌鸦形态)之后,也能改变飞行高度."
    public static float GetUnitDefaultFlyHeight(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitDefaultFlyHeightPtr, whichUnit.Handle);
    }


    /// title = "改变所属"
    /// description = "改变 ${单位} 所属为 ${Player} 并 ${Change/Retain Color}"
    /// comment = ""
    public static void SetUnitOwner(JUnit whichUnit, JPlayer whichPlayer, bool changeColor)
    {
        War3.CallNative(_setUnitOwnerPtr, whichUnit.Handle, whichPlayer.Handle, changeColor);
    }


    /// title = "改变队伍颜色"
    /// description = "改变 ${单位} 的队伍颜色为 ${Color}"
    /// comment = "改变队伍颜色并不会改变单位所属."
    public static void SetUnitColor(JUnit whichUnit, JPlayerColor whichColor)
    {
        War3.CallNative(_setUnitColorPtr, whichUnit.Handle, whichColor.Handle);
    }


    /// title = "改变单位尺寸(按倍数) [R]"
    /// description = "改变 ${单位} 的尺寸缩放为:(${X},${Y},${Z})"
    /// comment = "缩放尺寸使用(长,宽,高)格式."
    public static void SetUnitScale(JUnit whichUnit, float scaleX, float scaleY, float scaleZ)
    {
        War3.CallNative(_setUnitScalePtr, whichUnit.Handle, scaleX, scaleY, scaleZ);
    }


    /// title = "改变单位动画播放速度(按倍数) [R]"
    /// description = "改变 ${单位} 的动画播放速度为正常速度的 ${Percent} 倍"
    /// comment = "设置1倍动画播放速度来恢复正常状态."
    public static void SetUnitTimeScale(JUnit whichUnit, float timeScale)
    {
        War3.CallNative(_setUnitTimeScalePtr, whichUnit.Handle, timeScale);
    }

    public static void SetUnitBlendTime(JUnit whichUnit, float blendTime)
    {
        War3.CallNative(_setUnitBlendTimePtr, whichUnit.Handle, blendTime);
    }


    /// title = "改变单位的颜色(RGB:0-255) [R]"
    /// description = "改变 ${单位} 的颜色值: (${Red},${Green},${Blue}), 透明值: ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). 大多数单位使用(255,255,255)的颜色值和255的Alpha值. 透明值为0是不可见的.颜色值和Alpha值取值范围为0-255."
    public static void SetUnitVertexColor(JUnit whichUnit, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_setUnitVertexColorPtr, whichUnit.Handle, red, green, blue, alpha);
    }

    public static void QueueUnitAnimation(JUnit whichUnit, string whichAnimation)
    {
        War3.CallNative(_queueUnitAnimationPtr, whichUnit.Handle, whichAnimation);
    }


    /// title = "播放单位动画"
    /// description = "播放 ${Unit} 的 ${动画名} 动作"
    /// comment = "通过 '重置单位动作' 恢复到普通的动作."
    public static void SetUnitAnimation(JUnit whichUnit, string whichAnimation)
    {
        War3.CallNative(_setUnitAnimationPtr, whichUnit.Handle, whichAnimation);
    }


    /// title = "播放单位指定序号动动作 [R]"
    /// description = "播放 ${单位} 的第${序号} 号动作"
    /// comment = "可以指定播放所有的单位动画,不过需要自己多尝试.每个单位的动作序号不一样的."
    public static void SetUnitAnimationByIndex(JUnit whichUnit, int whichAnimation)
    {
        War3.CallNative(_setUnitAnimationByIndexPtr, whichUnit.Handle, whichAnimation);
    }


    /// title = "播放单位动运作(指定概率)"
    /// description = "播放 ${单位} 的 ${Animation Name} 动作,只用 ${Rarity} 动作"
    /// comment = "通过 '重置单位动作' 恢复到普通的动作."
    public static void SetUnitAnimationWithRarity(JUnit whichUnit, string whichAnimation, JRarityControl rarity)
    {
        War3.CallNative(_setUnitAnimationWithRarityPtr, whichUnit.Handle, whichAnimation, rarity.Handle);
    }


    /// title = "添加/删除 单位动画附加名 [R]"
    /// description = "给 ${单位} 附加动作 ${Tag} ,状态为 ${Add/Remove}"
    /// comment = "比如恶魔猎手添加'alternate'会显示为恶魔形态;农民添加'gold'则为背负黄金形态."
    public static void AddUnitAnimationProperties(JUnit whichUnit, string animProperties, bool add)
    {
        War3.CallNative(_addUnitAnimationPropertiesPtr, whichUnit.Handle, animProperties, add);
    }


    /// title = "锁定身体朝向"
    /// description = "锁定 ${单位} 的 ${Source} 朝向 ${目标单位} ,偏移坐标 (${X}, ${Y}, ${Z})"
    /// comment = "单位的该身体部件会一直朝向目标单位的偏移坐标点处,直到使用'重置身体朝向'. 坐标偏移以目标单位脚下为坐标原点."
    public static void SetUnitLookAt(JUnit whichUnit, string whichBone, JUnit lookAtTarget, float offsetX,
        float offsetY, float offsetZ)
    {
        War3.CallNative(_setUnitLookAtPtr, whichUnit.Handle, whichBone, lookAtTarget.Handle, offsetX, offsetY, offsetZ);
    }


    /// title = "重置身体朝向"
    /// description = "重置 ${单位} 的身体朝向"
    /// comment = "恢复单位的身体朝向为正常状态."
    public static void ResetUnitLookAt(JUnit whichUnit)
    {
        War3.CallNative(_resetUnitLookAtPtr, whichUnit.Handle);
    }


    /// title = "设置可否营救(对玩家) [R]"
    /// description = "设置 ${单位} 对 ${玩家}${Rescuable/Unrescuable}"
    /// comment = ""
    public static void SetUnitRescuable(JUnit whichUnit, JPlayer byWhichPlayer, bool flag)
    {
        War3.CallNative(_setUnitRescuablePtr, whichUnit.Handle, byWhichPlayer.Handle, flag);
    }


    /// title = "设置营救范围"
    /// description = "设置 ${单位} 的营救范围为 ${Range}"
    /// comment = ""
    public static void SetUnitRescueRange(JUnit whichUnit, float range)
    {
        War3.CallNative(_setUnitRescueRangePtr, whichUnit.Handle, range);
    }


    /// title = "设置英雄力量 [R]"
    /// description = "设置 ${英雄} 的力量为 ${Value} ,(${Permanent}永久奖励)"
    /// comment = "永久奖励貌似无效项,不需要理会."
    public static void SetHeroStr(JUnit whichHero, int newStr, bool permanent)
    {
        War3.CallNative(_setHeroStrPtr, whichHero.Handle, newStr, permanent);
    }


    /// title = "设置英雄敏捷 [R]"
    /// description = "设置 ${英雄} 的敏捷为 ${Value} ,(${Permanent}永久奖励)"
    /// comment = "永久奖励貌似无效项,不需要理会."
    public static void SetHeroAgi(JUnit whichHero, int newAgi, bool permanent)
    {
        War3.CallNative(_setHeroAgiPtr, whichHero.Handle, newAgi, permanent);
    }


    /// title = "设置英雄智力 [R]"
    /// description = "设置 ${英雄} 的智力为 ${Value} ,(${Permanent}永久奖励)"
    /// comment = "永久奖励貌似无效项,不需要理会."
    public static void SetHeroInt(JUnit whichHero, int newInt, bool permanent)
    {
        War3.CallNative(_setHeroIntPtr, whichHero.Handle, newInt, permanent);
    }


    /// title = "英雄力量 [R]"
    /// description = "${英雄} 的力量值(${Include/Exclude} 加成)"
    /// comment = ""
    public static int GetHeroStr(JUnit whichHero, bool includeBonuses)
    {
        return War3.CallNative<int>(_getHeroStrPtr, whichHero.Handle, includeBonuses);
    }


    /// title = "英雄敏捷 [R]"
    /// description = "${英雄} 的敏捷值(${Include/Exclude} 加成)"
    /// comment = ""
    public static int GetHeroAgi(JUnit whichHero, bool includeBonuses)
    {
        return War3.CallNative<int>(_getHeroAgiPtr, whichHero.Handle, includeBonuses);
    }


    /// title = "英雄智力 [R]"
    /// description = "${英雄} 的智力值(${Include/Exclude} 加成)"
    /// comment = ""
    public static int GetHeroInt(JUnit whichHero, bool includeBonuses)
    {
        return War3.CallNative<int>(_getHeroIntPtr, whichHero.Handle, includeBonuses);
    }


    /// title = "降低等级 [R]"
    /// description = "降低 ${Hero} ${Level} 个等级"
    /// comment = "只能降低等级. 英雄经验将重置为该等级的初始值."
    public static bool UnitStripHeroLevel(JUnit whichHero, int howManyLevels)
    {
        return War3.CallNative<bool>(_unitStripHeroLevelPtr, whichHero.Handle, howManyLevels);
    }


    /// title = "英雄经验值"
    /// description = "${英雄} 的经验值"
    /// comment = ""
    public static int GetHeroXP(JUnit whichHero)
    {
        return War3.CallNative<int>(_getHeroXPPtr, whichHero.Handle);
    }


    /// title = "设置经验值"
    /// description = "设置 ${Hero} 的经验值为 ${Quantity} , ${Show/Hide} 升级动画"
    /// comment = "经验值不能倒退."
    public static void SetHeroXP(JUnit whichHero, int newXpVal, bool showEyeCandy)
    {
        War3.CallNative(_setHeroXPPtr, whichHero.Handle, newXpVal, showEyeCandy);
    }


    /// title = "未分配技能点数"
    /// description = "${英雄} 的未分配技能点数"
    /// comment = ""
    public static int GetHeroSkillPoints(JUnit whichHero)
    {
        return War3.CallNative<int>(_getHeroSkillPointsPtr, whichHero.Handle);
    }


    /// title = "添加剩余技能点 [R]"
    /// description = "增加 ${英雄} ${Value} 点剩余技能点"
    /// comment = ""
    public static bool UnitModifySkillPoints(JUnit whichHero, int skillPointDelta)
    {
        return War3.CallNative<bool>(_unitModifySkillPointsPtr, whichHero.Handle, skillPointDelta);
    }


    /// title = "增加经验值 [R]"
    /// description = "增加 ${Hero} ${Quantity} 点经验值, ${Show/Hide} 升级动画"
    /// comment = "经验值不能倒退."
    public static void AddHeroXP(JUnit whichHero, int xpToAdd, bool showEyeCandy)
    {
        War3.CallNative(_addHeroXPPtr, whichHero.Handle, xpToAdd, showEyeCandy);
    }


    /// title = "设置等级"
    /// description = "设置 ${Hero} 的英雄等级为 ${Level} , ${Show/Hide} 升级动画"
    /// comment = "如果等级有变动,英雄经验将重置为该等级的初始值."
    public static void SetHeroLevel(JUnit whichHero, int level, bool showEyeCandy)
    {
        War3.CallNative(_setHeroLevelPtr, whichHero.Handle, level, showEyeCandy);
    }


    /// title = "英雄等级"
    /// description = "${英雄} 的英雄等级"
    /// comment = ""
    public static int GetHeroLevel(JUnit whichHero)
    {
        return War3.CallNative<int>(_getHeroLevelPtr, whichHero.Handle);
    }


    /// title = "单位等级"
    /// description = "${单位} 的等级"
    /// comment = "对于英雄则会返回其英雄等级."
    public static int GetUnitLevel(JUnit whichUnit)
    {
        return War3.CallNative<int>(_getUnitLevelPtr, whichUnit.Handle);
    }


    /// title = "英雄称谓"
    /// description = "${Hero} 的称谓"
    /// comment = "如圣骑士会返回'无惧的布赞恩'而不是'圣骑士'."
    public static string GetHeroProperName(JUnit whichHero)
    {
        return War3.CallNative<string>(_getHeroProperNamePtr, whichHero.Handle);
    }


    /// title = "允许/禁止经验获取 [R]"
    /// description = "${Enable/Disable} ${Hero} 的经验获取"
    /// comment = ""
    public static void SuspendHeroXP(JUnit whichHero, bool flag)
    {
        War3.CallNative(_suspendHeroXPPtr, whichHero.Handle, flag);
    }


    /// title = "经验不可获得"
    /// description = "${Hero} 不可获得经验"
    /// comment = "可使用'英雄 - 允许/禁止经验获取'来设置该项."
    public static bool IsSuspendedXP(JUnit whichHero)
    {
        return War3.CallNative<bool>(_isSuspendedXPPtr, whichHero.Handle);
    }


    /// title = "学习技能"
    /// description = "命令 ${Hero} 学习技能 ${Skill}"
    /// comment = "只有当英雄有剩余技能点时有效."
    public static void SelectHeroSkill(JUnit whichHero, int abilcode)
    {
        War3.CallNative(_selectHeroSkillPtr, whichHero.Handle, abilcode);
    }


    /// title = "单位技能等级 [R]"
    /// description = "${单位} 的 ${技能} 技能等级"
    /// comment = "如果单位没有该技能,则返回0."
    public static int GetUnitAbilityLevel(JUnit whichUnit, int abilcode)
    {
        return War3.CallNative<int>(_getUnitAbilityLevelPtr, whichUnit.Handle, abilcode);
    }


    /// title = "降低技能等级 [R]"
    /// description = "使 ${单位} 的 ${技能} 等级降低1级"
    /// comment = "改变死亡单位的光环技能会导致魔兽崩溃."
    public static int DecUnitAbilityLevel(JUnit whichUnit, int abilcode)
    {
        return War3.CallNative<int>(_decUnitAbilityLevelPtr, whichUnit.Handle, abilcode);
    }


    /// title = "提升技能等级 [R]"
    /// description = "使 ${单位} 的 ${技能} 等级提升1级"
    /// comment = "改变死亡单位的光环技能会导致魔兽崩溃."
    public static int IncUnitAbilityLevel(JUnit whichUnit, int abilcode)
    {
        return War3.CallNative<int>(_incUnitAbilityLevelPtr, whichUnit.Handle, abilcode);
    }


    /// title = "设置技能等级 [R]"
    /// description = "设置 ${单位} 的 ${技能} 等级为 ${Level}"
    /// comment = "改变死亡单位的光环技能会导致魔兽崩溃."
    public static int SetUnitAbilityLevel(JUnit whichUnit, int abilcode, int level)
    {
        return War3.CallNative<int>(_setUnitAbilityLevelPtr, whichUnit.Handle, abilcode, level);
    }


    /// title = "立即复活(指定坐标) [R]"
    /// description = "立即复活 ${英雄} 在(${X},${Y}), ${Show/Hide} 复活动画"
    /// comment = "如果英雄正在祭坛复活,则会退回部分花费(默认为100%)."
    public static bool ReviveHero(JUnit whichHero, float x, float y, bool doEyecandy)
    {
        return War3.CallNative<bool>(_reviveHeroPtr, whichHero.Handle, x, y, doEyecandy);
    }


    /// title = "立即复活(指定点)"
    /// description = "立即复活 ${英雄} 在 ${指定点} , ${Show/Hide} 复活动画"
    /// comment = "如果英雄正在祭坛复活,则会退回部分花费(默认为100%)."
    public static bool ReviveHeroLoc(JUnit whichHero, JLocation loc, bool doEyecandy)
    {
        return War3.CallNative<bool>(_reviveHeroLocPtr, whichHero.Handle, loc.Handle, doEyecandy);
    }

    public static void SetUnitExploded(JUnit whichUnit, bool exploded)
    {
        War3.CallNative(_setUnitExplodedPtr, whichUnit.Handle, exploded);
    }


    /// title = "设置无敌/可攻击"
    /// description = "设置 ${单位} ${Invulnerable/Vulnerable}"
    /// comment = ""
    public static void SetUnitInvulnerable(JUnit whichUnit, bool flag)
    {
        War3.CallNative(_setUnitInvulnerablePtr, whichUnit.Handle, flag);
    }


    /// title = "暂停/恢复 [R]"
    /// description = "设置 ${单位} ${Pause/Unpause}"
    /// comment = ""
    public static void PauseUnit(JUnit whichUnit, bool flag)
    {
        War3.CallNative(_pauseUnitPtr, whichUnit.Handle, flag);
    }

    public static bool IsUnitPaused(JUnit whichHero)
    {
        return War3.CallNative<bool>(_isUnitPausedPtr, whichHero.Handle);
    }


    /// title = "设置碰撞开关"
    /// description = "设置 ${单位} ${On/Off} 碰撞"
    /// comment = "关闭碰撞的单位无视障碍物,但其他单位仍视其为障碍物."
    public static void SetUnitPathing(JUnit whichUnit, bool flag)
    {
        War3.CallNative(_setUnitPathingPtr, whichUnit.Handle, flag);
    }


    /// title = "清空选择(所有玩家)"
    /// description = "清空所有玩家的选择"
    /// comment = "使玩家取消选择所有已选单位."
    public static void ClearSelection()
    {
        War3.CallNative(_clearSelectionPtr);
    }

    public static void SelectUnit(JUnit whichUnit, bool flag)
    {
        War3.CallNative(_selectUnitPtr, whichUnit.Handle, flag);
    }


    /// title = "单位附加值"
    /// description = "${单位} 的附加值"
    /// comment = "单位附加值不可改变. 可以做一些特殊用途. 比如TD地图中的建筑贩卖价格."
    public static int GetUnitPointValue(JUnit whichUnit)
    {
        return War3.CallNative<int>(_getUnitPointValuePtr, whichUnit.Handle);
    }


    /// title = "单位附加值(指定单位类型)"
    /// description = "${单位类型} 的附加值"
    /// comment = "单位附加值不可改变. 可以做一些特殊用途. 比如TD地图中的建筑贩卖价格."
    public static int GetUnitPointValueByType(int unitType)
    {
        return War3.CallNative<int>(_getUnitPointValueByTypePtr, unitType);
    }


    /// title = "给予物品 [R]"
    /// description = "给予 ${单位} ${物品}"
    /// comment = ""
    public static bool UnitAddItem(JUnit whichUnit, JItem whichItem)
    {
        return War3.CallNative<bool>(_unitAddItemPtr, whichUnit.Handle, whichItem.Handle);
    }

    public static JItem UnitAddItemById(JUnit whichUnit, int itemId)
    {
        var handle = War3.CallNative<int>(_unitAddItemByIdPtr, whichUnit.Handle, itemId);
        return new JItem(handle);
    }


    /// title = "新建物品到指定物品栏 [R]"
    /// description = "给予 ${单位} ${物品类型} 并放在物品栏# ${数值}"
    /// comment = "注意: 物品栏编号从0-5,而不是1-6. 该动作创建的物品不被'最后创建的物品'所记录."
    public static bool UnitAddItemToSlotById(JUnit whichUnit, int itemId, int itemSlot)
    {
        return War3.CallNative<bool>(_unitAddItemToSlotByIdPtr, whichUnit.Handle, itemId, itemSlot);
    }

    public static void UnitRemoveItem(JUnit whichUnit, JItem whichItem)
    {
        War3.CallNative(_unitRemoveItemPtr, whichUnit.Handle, whichItem.Handle);
    }

    public static JItem UnitRemoveItemFromSlot(JUnit whichUnit, int itemSlot)
    {
        var handle = War3.CallNative<int>(_unitRemoveItemFromSlotPtr, whichUnit.Handle, itemSlot);
        return new JItem(handle);
    }


    /// title = "持有物品"
    /// description = "${Hero} 拥有 ${物品}"
    /// comment = ""
    public static bool UnitHasItem(JUnit whichUnit, JItem whichItem)
    {
        return War3.CallNative<bool>(_unitHasItemPtr, whichUnit.Handle, whichItem.Handle);
    }


    /// title = "单位持有物品"
    /// description = "${单位} 物品栏第 ${Index} 格的物品"
    /// comment = "第一个单位格的位置为0."
    public static JItem UnitItemInSlot(JUnit whichUnit, int itemSlot)
    {
        var handle = War3.CallNative<int>(_unitItemInSlotPtr, whichUnit.Handle, itemSlot);
        return new JItem(handle);
    }

    public static int UnitInventorySize(JUnit whichUnit)
    {
        return War3.CallNative<int>(_unitInventorySizePtr, whichUnit.Handle);
    }


    /// title = "发布丢弃物品命令(指定坐标) [R]"
    /// description = "命令 ${单位} 丢弃物品 ${物品} 到坐标:(${X},${Y})"
    /// comment = ""
    public static bool UnitDropItemPoint(JUnit whichUnit, JItem whichItem, float x, float y)
    {
        return War3.CallNative<bool>(_unitDropItemPointPtr, whichUnit.Handle, whichItem.Handle, x, y);
    }


    /// title = "移动物品到物品栏 [R]"
    /// description = "命令 ${单位} 移动 ${物品} 到物品栏# ${Index}"
    /// comment = "只有当单位持有该物品时才有效. 注意: 该函数中物品栏编号从0-5,而不是1-6."
    public static bool UnitDropItemSlot(JUnit whichUnit, JItem whichItem, int slot)
    {
        return War3.CallNative<bool>(_unitDropItemSlotPtr, whichUnit.Handle, whichItem.Handle, slot);
    }

    public static bool UnitDropItemTarget(JUnit whichUnit, JItem whichItem, JWidget target)
    {
        return War3.CallNative<bool>(_unitDropItemTargetPtr, whichUnit.Handle, whichItem.Handle, target.Handle);
    }


    /// title = "使用物品(无目标)"
    /// description = "命令 ${单位} 使用 ${物品}"
    /// comment = ""
    public static bool UnitUseItem(JUnit whichUnit, JItem whichItem)
    {
        return War3.CallNative<bool>(_unitUseItemPtr, whichUnit.Handle, whichItem.Handle);
    }


    /// title = "使用物品(指定坐标)"
    /// description = "命令 ${单位} 使用 ${物品} ,目标坐标:(${X},${Y})"
    /// comment = ""
    public static bool UnitUseItemPoint(JUnit whichUnit, JItem whichItem, float x, float y)
    {
        return War3.CallNative<bool>(_unitUseItemPointPtr, whichUnit.Handle, whichItem.Handle, x, y);
    }


    /// title = "使用物品(对单位)"
    /// description = "命令 ${单位} 使用 ${物品} ,目标: ${单位}"
    /// comment = ""
    public static bool UnitUseItemTarget(JUnit whichUnit, JItem whichItem, JWidget target)
    {
        return War3.CallNative<bool>(_unitUseItemTargetPtr, whichUnit.Handle, whichItem.Handle, target.Handle);
    }


    /// title = "单位所在X轴坐标 [R]"
    /// description = "${单位} 所在X轴坐标"
    /// comment = ""
    public static float GetUnitX(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitXPtr, whichUnit.Handle);
    }


    /// title = "单位所在Y轴坐标 [R]"
    /// description = "${单位} 所在Y轴坐标"
    /// comment = ""
    public static float GetUnitY(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitYPtr, whichUnit.Handle);
    }


    /// title = "单位位置"
    /// description = "${单位} 的位置"
    /// comment = "会创建点."
    public static JLocation GetUnitLoc(JUnit whichUnit)
    {
        var handle = War3.CallNative<int>(_getUnitLocPtr, whichUnit.Handle);
        return new JLocation(handle);
    }


    /// title = "面向角度"
    /// description = "${单位} 的面向角度"
    /// comment = "采用角度制. 0度为正东方向, 90度为正北方向."
    public static float GetUnitFacing(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitFacingPtr, whichUnit.Handle);
    }


    /// title = "当前移动速度"
    /// description = "${单位} 的当前移动速度"
    /// comment = ""
    public static float GetUnitMoveSpeed(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitMoveSpeedPtr, whichUnit.Handle);
    }


    /// title = "默认移动速度"
    /// description = "${单位} 的默认移动速度"
    /// comment = ""
    public static float GetUnitDefaultMoveSpeed(JUnit whichUnit)
    {
        return War3.CallNative<float>(_getUnitDefaultMoveSpeedPtr, whichUnit.Handle);
    }


    /// title = "属性 [R]"
    /// description = "${单位} 的 ${Property}"
    /// comment = ""
    public static float GetUnitState(JUnit whichUnit, JUnitState whichUnitState)
    {
        return War3.CallNative<float>(_getUnitStatePtr, whichUnit.Handle, whichUnitState.Handle);
    }


    /// title = "单位所有者"
    /// description = "${单位} 的所有者"
    /// comment = ""
    public static JPlayer GetOwningPlayer(JUnit whichUnit)
    {
        var handle = War3.CallNative<int>(_getOwningPlayerPtr, whichUnit.Handle);
        return new JPlayer(handle);
    }


    /// title = "指定单位的类型"
    /// description = "${单位} 的类型"
    /// comment = ""
    public static int GetUnitTypeId(JUnit whichUnit)
    {
        return War3.CallNative<int>(_getUnitTypeIdPtr, whichUnit.Handle);
    }


    /// title = "单位种族"
    /// description = "${单位} 所属种族"
    /// comment = "物体编辑器中设置的单位所属种族."
    public static JRace GetUnitRace(JUnit whichUnit)
    {
        var handle = War3.CallNative<int>(_getUnitRacePtr, whichUnit.Handle);
        return new JRace(handle);
    }


    /// title = "单位名字"
    /// description = "${单位} 的名字"
    /// comment = ""
    public static string GetUnitName(JUnit whichUnit)
    {
        return War3.CallNative<string>(_getUnitNamePtr, whichUnit.Handle);
    }


    /// title = "单位使用人口数量"
    /// description = "${单位} 使用的人口数量"
    /// comment = ""
    public static int GetUnitFoodUsed(JUnit whichUnit)
    {
        return War3.CallNative<int>(_getUnitFoodUsedPtr, whichUnit.Handle);
    }


    /// title = "单位提供人口数量"
    /// description = "${单位} 提供的人口数量"
    /// comment = ""
    public static int GetUnitFoodMade(JUnit whichUnit)
    {
        return War3.CallNative<int>(_getUnitFoodMadePtr, whichUnit.Handle);
    }


    /// title = "单位提供人口数量(指定单位类型)"
    /// description = "${单位类型} 提供的人口数量"
    /// comment = ""
    public static int GetFoodMade(int unitId)
    {
        return War3.CallNative<int>(_getFoodMadePtr, unitId);
    }


    /// title = "单位使用人口数量(指定单位类型)"
    /// description = "${单位类型} 使用的人口数量"
    /// comment = ""
    public static int GetFoodUsed(int unitId)
    {
        return War3.CallNative<int>(_getFoodUsedPtr, unitId);
    }


    /// title = "允许/禁止 人口占用 [R]"
    /// description = "设置 ${单位} : ${Enable/Disable} 其人口占用"
    /// comment = ""
    public static void SetUnitUseFood(JUnit whichUnit, bool useFood)
    {
        War3.CallNative(_setUnitUseFoodPtr, whichUnit.Handle, useFood);
    }


    /// title = "单位集结点"
    /// description = "${单位} 的集结点位置"
    /// comment = "如果单位没有设置集结点,则返回null. 设置自己为集结点可取消集结点设置. 会创建点."
    public static JLocation GetUnitRallyPoint(JUnit whichUnit)
    {
        var handle = War3.CallNative<int>(_getUnitRallyPointPtr, whichUnit.Handle);
        return new JLocation(handle);
    }


    /// title = "单位集结点目标"
    /// description = "${单位} 的集结点目标"
    /// comment = "如果指定单位没有设置集结点到单位目标,则返回null."
    public static JUnit GetUnitRallyUnit(JUnit whichUnit)
    {
        var handle = War3.CallNative<int>(_getUnitRallyUnitPtr, whichUnit.Handle);
        return new JUnit(handle);
    }


    /// title = "单位集结点目标"
    /// description = "${单位} 的集结点目标"
    /// comment = "如果指定单位没有设置集结点到可破坏物上,则返回null."
    public static JDestructable GetUnitRallyDestructable(JUnit whichUnit)
    {
        var handle = War3.CallNative<int>(_getUnitRallyDestructablePtr, whichUnit.Handle);
        return new JDestructable(handle);
    }


    /// title = "在单位组"
    /// description = "${单位} 在 ${单位组} 中"
    /// comment = ""
    public static bool IsUnitInGroup(JUnit whichUnit, JGroup whichGroup)
    {
        return War3.CallNative<bool>(_isUnitInGroupPtr, whichUnit.Handle, whichGroup.Handle);
    }


    /// title = "是玩家组里玩家的单位"
    /// description = "${单位} 属于 ${玩家组} 里的玩家"
    /// comment = "判断单位是否属于这个玩家组里的玩家。"
    public static bool IsUnitInForce(JUnit whichUnit, JForce whichForce)
    {
        return War3.CallNative<bool>(_isUnitInForcePtr, whichUnit.Handle, whichForce.Handle);
    }


    /// title = "是玩家的单位"
    /// description = "${单位} 属于 ${Player}"
    /// comment = "判断单位是否属于这个玩家。"
    public static bool IsUnitOwnedByPlayer(JUnit whichUnit, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isUnitOwnedByPlayerPtr, whichUnit.Handle, whichPlayer.Handle);
    }


    /// title = "是玩家的同盟单位"
    /// description = "${单位} 是 ${Player} 的同盟单位"
    /// comment = "包括中立状态. 单向判断玩家对单位是否为不侵犯状态."
    public static bool IsUnitAlly(JUnit whichUnit, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isUnitAllyPtr, whichUnit.Handle, whichPlayer.Handle);
    }


    /// title = "是玩家的敌对单位"
    /// description = "${单位} 是 ${Player} 的敌对单位"
    /// comment = "不包括中立状态. 单向判断玩家对单位是否为敌对侵犯."
    public static bool IsUnitEnemy(JUnit whichUnit, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isUnitEnemyPtr, whichUnit.Handle, whichPlayer.Handle);
    }


    /// title = "单位可见"
    /// description = "${单位} 对 ${Player} 可见"
    /// comment = ""
    public static bool IsUnitVisible(JUnit whichUnit, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isUnitVisiblePtr, whichUnit.Handle, whichPlayer.Handle);
    }


    /// title = "被检测到"
    /// description = "${单位} 处在 ${玩家} 的真实视野范围内"
    /// comment = "用来判断单位在这个玩家反隐形范围内，注：不包含该玩家同盟的反隐范围。"
    public static bool IsUnitDetected(JUnit whichUnit, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isUnitDetectedPtr, whichUnit.Handle, whichPlayer.Handle);
    }


    /// title = "单位不可见"
    /// description = "${单位} 对 ${Player} 不可见"
    /// comment = ""
    public static bool IsUnitInvisible(JUnit whichUnit, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isUnitInvisiblePtr, whichUnit.Handle, whichPlayer.Handle);
    }


    /// title = "单位在迷雾中"
    /// description = "${单位} 在 ${Player} 的迷雾范围内"
    /// comment = "黑色阴影内的单位不被计算在内."
    public static bool IsUnitFogged(JUnit whichUnit, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isUnitFoggedPtr, whichUnit.Handle, whichPlayer.Handle);
    }


    /// title = "单位在黑色阴影中"
    /// description = "${单位} 在 ${Player} 的黑色阴影内"
    /// comment = ""
    public static bool IsUnitMasked(JUnit whichUnit, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isUnitMaskedPtr, whichUnit.Handle, whichPlayer.Handle);
    }


    /// title = "被玩家选择"
    /// description = "${单位} 被 ${Player} 选择"
    /// comment = ""
    public static bool IsUnitSelected(JUnit whichUnit, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isUnitSelectedPtr, whichUnit.Handle, whichPlayer.Handle);
    }


    /// title = "单位种族检查"
    /// description = "${单位} 是 ${Race}"
    /// comment = ""
    public static bool IsUnitRace(JUnit whichUnit, JRace whichRace)
    {
        return War3.CallNative<bool>(_isUnitRacePtr, whichUnit.Handle, whichRace.Handle);
    }


    /// title = "单位类别检查"
    /// description = "${单位} 是 ${Type}"
    /// comment = ""
    public static bool IsUnitType(JUnit whichUnit, JUnitType whichUnitType)
    {
        return War3.CallNative<bool>(_isUnitTypePtr, whichUnit.Handle, whichUnitType.Handle);
    }


    /// title = "单位检查"
    /// description = "${单位} 与 ${单位}相同"
    /// comment = "用来判断两个单位是否相等。"
    public static bool IsUnit(JUnit whichUnit, JUnit whichSpecifiedUnit)
    {
        return War3.CallNative<bool>(_isUnitPtr, whichUnit.Handle, whichSpecifiedUnit.Handle);
    }


    /// title = "在指定单位范围内 [R]"
    /// description = "${单位} 在距离 ${指定单位} ${范围} 范围内"
    /// comment = ""
    public static bool IsUnitInRange(JUnit whichUnit, JUnit otherUnit, float distance)
    {
        return War3.CallNative<bool>(_isUnitInRangePtr, whichUnit.Handle, otherUnit.Handle, distance);
    }


    /// title = "在指定坐标范围内 [R]"
    /// description = "${单位} 在距离坐标(${X},${Y}) ${范围} 范围内"
    /// comment = ""
    public static bool IsUnitInRangeXY(JUnit whichUnit, float x, float y, float distance)
    {
        return War3.CallNative<bool>(_isUnitInRangeXYPtr, whichUnit.Handle, x, y, distance);
    }


    /// title = "在指定点范围内 [R]"
    /// description = "${单位} 在距离 ${指定点} ${范围} 范围内"
    /// comment = ""
    public static bool IsUnitInRangeLoc(JUnit whichUnit, JLocation whichLocation, float distance)
    {
        return War3.CallNative<bool>(_isUnitInRangeLocPtr, whichUnit.Handle, whichLocation.Handle, distance);
    }

    public static bool IsUnitHidden(JUnit whichUnit)
    {
        return War3.CallNative<bool>(_isUnitHiddenPtr, whichUnit.Handle);
    }

    public static bool IsUnitIllusion(JUnit whichUnit)
    {
        return War3.CallNative<bool>(_isUnitIllusionPtr, whichUnit.Handle);
    }

    public static bool IsUnitInTransport(JUnit whichUnit, JUnit whichTransport)
    {
        return War3.CallNative<bool>(_isUnitInTransportPtr, whichUnit.Handle, whichTransport.Handle);
    }

    public static bool IsUnitLoaded(JUnit whichUnit)
    {
        return War3.CallNative<bool>(_isUnitLoadedPtr, whichUnit.Handle);
    }


    /// title = "单位类型是英雄单位"
    /// description = "${UnitType} 是英雄单位"
    public static bool IsHeroUnitId(int unitId)
    {
        return War3.CallNative<bool>(_isHeroUnitIdPtr, unitId);
    }


    /// title = "单位类别检查(指定单位类型)"
    /// description = "${单位类型} 是 ${Type}"
    /// comment = ""
    public static bool IsUnitIdType(int unitId, JUnitType whichUnitType)
    {
        return War3.CallNative<bool>(_isUnitIdTypePtr, unitId, whichUnitType.Handle);
    }


    /// title = "共享视野 [R]"
    /// description = "设置 ${单位} 的视野对 ${Player} ${on/off}"
    /// comment = ""
    public static void UnitShareVision(JUnit whichUnit, JPlayer whichPlayer, bool share)
    {
        War3.CallNative(_unitShareVisionPtr, whichUnit.Handle, whichPlayer.Handle, share);
    }


    /// title = "暂停尸体腐烂 [R]"
    /// description = " 设置 ${单位} 的尸体腐烂状态: ${Suspend/Resume}"
    /// comment = "只对已完成死亡动作的尸体有效."
    public static void UnitSuspendDecay(JUnit whichUnit, bool suspend)
    {
        War3.CallNative(_unitSuspendDecayPtr, whichUnit.Handle, suspend);
    }


    /// title = "添加类别 [R]"
    /// description = "为 ${单位} 添加 ${Classification} 类别"
    /// comment = "已去除所有无效类别."
    public static bool UnitAddType(JUnit whichUnit, JUnitType whichUnitType)
    {
        return War3.CallNative<bool>(_unitAddTypePtr, whichUnit.Handle, whichUnitType.Handle);
    }


    /// title = "删除类别 [R]"
    /// description = "为 ${单位} 删除 ${Classification} 类别"
    /// comment = "已去除所有无效类别."
    public static bool UnitRemoveType(JUnit whichUnit, JUnitType whichUnitType)
    {
        return War3.CallNative<bool>(_unitRemoveTypePtr, whichUnit.Handle, whichUnitType.Handle);
    }


    /// title = "添加技能 [R]"
    /// description = "为 ${单位} 添加 ${技能}"
    /// comment = ""
    public static bool UnitAddAbility(JUnit whichUnit, int abilityId)
    {
        return War3.CallNative<bool>(_unitAddAbilityPtr, whichUnit.Handle, abilityId);
    }


    /// title = "删除技能 [R]"
    /// description = "为 ${单位} 删除 ${技能}"
    /// comment = ""
    public static bool UnitRemoveAbility(JUnit whichUnit, int abilityId)
    {
        return War3.CallNative<bool>(_unitRemoveAbilityPtr, whichUnit.Handle, abilityId);
    }


    /// title = "设置技能永久性 [R]"
    /// description = "设置 ${单位} ${是否} ${技能} 永久性"
    /// comment = "如触发添加给单位的技能就是非永久性的,非永久性技能在变身并回复之后会丢失掉. 这类情况就需要设置技能永久性."
    public static bool UnitMakeAbilityPermanent(JUnit whichUnit, bool permanent, int abilityId)
    {
        return War3.CallNative<bool>(_unitMakeAbilityPermanentPtr, whichUnit.Handle, permanent, abilityId);
    }


    /// title = "删除魔法效果(指定极性) [R]"
    /// description = "删除 ${单位} 的附带Buff,(${Include/Exclude} 正面Buff, ${Include/Exclude} 负面Buff)"
    public static void UnitRemoveBuffs(JUnit whichUnit, bool removePositive, bool removeNegative)
    {
        War3.CallNative(_unitRemoveBuffsPtr, whichUnit.Handle, removePositive, removeNegative);
    }


    /// title = "删除魔法效果(详细类别) [R]"
    /// description = "删除 ${单位} 的附带Buff,(${Include/Exclude} 正面Buff, ${Include/Exclude} 负面Buff${Include/Exclude} 魔法Buff, ${Include/Exclude} 物理Buff${Include/Exclude} 生命周期, ${Include/Exclude} 光环效果${Include/Exclude} 不可驱散Buff)"
    public static void UnitRemoveBuffsEx(JUnit whichUnit, bool removePositive, bool removeNegative, bool magic,
        bool physical, bool timedLife, bool aura, bool autoDispel)
    {
        War3.CallNative(_unitRemoveBuffsExPtr, whichUnit.Handle, removePositive, removeNegative, magic, physical,
            timedLife, aura, autoDispel);
    }

    public static bool UnitHasBuffsEx(JUnit whichUnit, bool removePositive, bool removeNegative, bool magic,
        bool physical, bool timedLife, bool aura, bool autoDispel)
    {
        return War3.CallNative<bool>(_unitHasBuffsExPtr, whichUnit.Handle, removePositive, removeNegative, magic,
            physical, timedLife, aura, autoDispel);
    }


    /// title = "拥有Buff数量 [R]"
    /// description = "${单位} 的附带Buff数量,(${Include/Exclude} 正面Buff, ${Include/Exclude} 负面Buff${Include/Exclude} 魔法Buff, ${Include/Exclude} 物理Buff${Include/Exclude} 生命周期, ${Include/Exclude} 光环效果${Include/Exclude} 不可驱散Buff)"
    /// comment = ""
    public static int UnitCountBuffsEx(JUnit whichUnit, bool removePositive, bool removeNegative, bool magic,
        bool physical, bool timedLife, bool aura, bool autoDispel)
    {
        return War3.CallNative<int>(_unitCountBuffsExPtr, whichUnit.Handle, removePositive, removeNegative, magic,
            physical, timedLife, aura, autoDispel);
    }

    public static void UnitAddSleep(JUnit whichUnit, bool add)
    {
        War3.CallNative(_unitAddSleepPtr, whichUnit.Handle, add);
    }

    public static bool UnitCanSleep(JUnit whichUnit)
    {
        return War3.CallNative<bool>(_unitCanSleepPtr, whichUnit.Handle);
    }


    /// title = "控制单位睡眠状态"
    /// description = "使 ${单位} ${Sleep/Remain Awake}"
    /// comment = "使用该功能前必须用触发为单位添加'一直睡眠'技能."
    public static void UnitAddSleepPerm(JUnit whichUnit, bool add)
    {
        War3.CallNative(_unitAddSleepPermPtr, whichUnit.Handle, add);
    }


    /// title = "允许控制睡眠状态"
    /// description = "允许控制 ${单位} 的睡眠状态"
    /// comment = "即该单位拥有'一直睡眠'技能."
    public static bool UnitCanSleepPerm(JUnit whichUnit)
    {
        return War3.CallNative<bool>(_unitCanSleepPermPtr, whichUnit.Handle);
    }

    public static bool UnitIsSleeping(JUnit whichUnit)
    {
        return War3.CallNative<bool>(_unitIsSleepingPtr, whichUnit.Handle);
    }

    public static void UnitWakeUp(JUnit whichUnit)
    {
        War3.CallNative(_unitWakeUpPtr, whichUnit.Handle);
    }


    /// title = "设置生命周期 [R]"
    /// description = "为 ${单位} 设置 ${Buff Type} 类型的生命周期,持续时间为 ${Duration} 秒"
    /// comment = ""
    public static void UnitApplyTimedLife(JUnit whichUnit, int buffId, float duration)
    {
        War3.CallNative(_unitApplyTimedLifePtr, whichUnit.Handle, buffId, duration);
    }

    public static bool UnitIgnoreAlarm(JUnit whichUnit, bool flag)
    {
        return War3.CallNative<bool>(_unitIgnoreAlarmPtr, whichUnit.Handle, flag);
    }

    public static bool UnitIgnoreAlarmToggled(JUnit whichUnit)
    {
        return War3.CallNative<bool>(_unitIgnoreAlarmToggledPtr, whichUnit.Handle);
    }


    /// title = "重置技能CD"
    /// description = "重置 ${单位} 的所有技能冷却时间"
    /// comment = "如果要重置单一技能的CD,可以通过删除技能+添加技能+设置技能等级来完成."
    public static void UnitResetCooldown(JUnit whichUnit)
    {
        War3.CallNative(_unitResetCooldownPtr, whichUnit.Handle);
    }


    /// title = "设置建筑建造进度条"
    /// description = "设置 ${Building} 的建造进度条为 ${Progress}%"
    /// comment = "只作用于正在建造的建筑."
    public static void UnitSetConstructionProgress(JUnit whichUnit, int constructionPercentage)
    {
        War3.CallNative(_unitSetConstructionProgressPtr, whichUnit.Handle, constructionPercentage);
    }


    /// title = "设置建筑升级进度条"
    /// description = "设置 ${Building} 的升级进度条为 ${Progress}%"
    /// comment = "只作用于正在升级的建筑. 是建筑A升级为建筑B的升级,不是科技的研究."
    public static void UnitSetUpgradeProgress(JUnit whichUnit, int upgradePercentage)
    {
        War3.CallNative(_unitSetUpgradeProgressPtr, whichUnit.Handle, upgradePercentage);
    }


    /// title = "暂停/恢复生命周期 [R]"
    /// description = "使 ${单位} ${Pause/Unpause} 生命周期"
    /// comment = "只有召唤单位有生命周期."
    public static void UnitPauseTimedLife(JUnit whichUnit, bool flag)
    {
        War3.CallNative(_unitPauseTimedLifePtr, whichUnit.Handle, flag);
    }

    public static void UnitSetUsesAltIcon(JUnit whichUnit, bool flag)
    {
        War3.CallNative(_unitSetUsesAltIconPtr, whichUnit.Handle, flag);
    }


    /// title = "伤害区域 [R]"
    /// description = "命令 ${单位} 在 ${Seconds} 秒后对半径为 ${Size} 圆心为(${X},${Y})的范围造成 ${Amount} 点伤害(${是} 攻击伤害, ${是}远程攻击) 攻击类型: ${AttackType} 伤害类型: ${DamageType} 装甲类型: ${WeaponType}"
    /// comment = "该动作不会打断单位动作. 由该动作伤害/杀死单位同样正常触发'受到伤害'和'死亡'单位事件."
    public static bool UnitDamagePoint(JUnit whichUnit, float delay, float radius, float x, float y, float amount,
        bool attack, bool ranged, JAttackType attackType, JDamageType damageType, JWeaponType weaponType)
    {
        return War3.CallNative<bool>(_unitDamagePointPtr, whichUnit.Handle, delay, radius, x, y, amount, attack, ranged,
            attackType.Handle, damageType.Handle, weaponType.Handle);
    }


    /// title = "伤害目标 [R]"
    /// description = "命令 ${单位} 对 ${Target} 造成 ${Amount} 点伤害(${是} 攻击伤害, ${是}远程攻击) 攻击类型: ${AttackType} 伤害类型: ${DamageType} 武器类型: ${WeaponType}"
    /// comment = "该动作不会打断单位动作. 由该动作伤害/杀死单位同样正常触发'受到伤害'和'死亡'单位事件."
    public static bool UnitDamageTarget(JUnit whichUnit, JWidget target, float amount, bool attack, bool ranged,
        JAttackType attackType, JDamageType damageType, JWeaponType weaponType)
    {
        return War3.CallNative<bool>(_unitDamageTargetPtr, whichUnit.Handle, target.Handle, amount, attack, ranged,
            attackType.Handle, damageType.Handle, weaponType.Handle);
    }


    /// title = "发布命令(无目标)"
    /// description = "对 ${单位} 发布 ${Order} 命令"
    /// comment = ""
    public static bool IssueImmediateOrder(JUnit whichUnit, string order)
    {
        return War3.CallNative<bool>(_issueImmediateOrderPtr, whichUnit.Handle, order);
    }


    /// title = "发布命令(无目标)(ID)"
    /// description = "对 ${单位} 发布 ${Order} 命令"
    /// comment = ""
    public static bool IssueImmediateOrderById(JUnit whichUnit, int order)
    {
        return War3.CallNative<bool>(_issueImmediateOrderByIdPtr, whichUnit.Handle, order);
    }


    /// title = "发布命令(指定坐标) [R]"
    /// description = "对 ${单位} 发布 ${Order} 命令到坐标:(${X},${Y})"
    /// comment = ""
    public static bool IssuePointOrder(JUnit whichUnit, string order, float x, float y)
    {
        return War3.CallNative<bool>(_issuePointOrderPtr, whichUnit.Handle, order, x, y);
    }


    /// title = "发布命令(指定点)"
    /// description = "对 ${单位} 发布 ${Order} 命令到目标点: ${指定点}"
    /// comment = ""
    public static bool IssuePointOrderLoc(JUnit whichUnit, string order, JLocation whichLocation)
    {
        return War3.CallNative<bool>(_issuePointOrderLocPtr, whichUnit.Handle, order, whichLocation.Handle);
    }


    /// title = "发布命令(指定坐标)(ID)"
    /// description = "对 ${单位} 发布 ${Order} 命令到坐标:(${X},${Y})"
    /// comment = ""
    public static bool IssuePointOrderById(JUnit whichUnit, int order, float x, float y)
    {
        return War3.CallNative<bool>(_issuePointOrderByIdPtr, whichUnit.Handle, order, x, y);
    }


    /// title = "发布命令(指定点)(ID)"
    /// description = "对 ${单位} 发布 ${Order} 命令到目标点: ${指定点}"
    /// comment = ""
    public static bool IssuePointOrderByIdLoc(JUnit whichUnit, int order, JLocation whichLocation)
    {
        return War3.CallNative<bool>(_issuePointOrderByIdLocPtr, whichUnit.Handle, order, whichLocation.Handle);
    }


    /// title = "发布命令(指定单位)"
    /// description = "对 ${单位} 发布 ${Order} 命令到目标: ${单位}"
    /// comment = ""
    public static bool IssueTargetOrder(JUnit whichUnit, string order, JWidget targetWidget)
    {
        return War3.CallNative<bool>(_issueTargetOrderPtr, whichUnit.Handle, order, targetWidget.Handle);
    }


    /// title = "发布命令(指定单位)(ID)"
    /// description = "对 ${单位} 发布 ${Order} 命令到目标: ${单位}"
    /// comment = ""
    public static bool IssueTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget)
    {
        return War3.CallNative<bool>(_issueTargetOrderByIdPtr, whichUnit.Handle, order, targetWidget.Handle);
    }

    public static bool IssueInstantPointOrder(JUnit whichUnit, string order, float x, float y,
        JWidget instantTargetWidget)
    {
        return War3.CallNative<bool>(_issueInstantPointOrderPtr, whichUnit.Handle, order, x, y,
            instantTargetWidget.Handle);
    }

    public static bool IssueInstantPointOrderById(JUnit whichUnit, int order, float x, float y,
        JWidget instantTargetWidget)
    {
        return War3.CallNative<bool>(_issueInstantPointOrderByIdPtr, whichUnit.Handle, order, x, y,
            instantTargetWidget.Handle);
    }

    public static bool IssueInstantTargetOrder(JUnit whichUnit, string order, JWidget targetWidget,
        JWidget instantTargetWidget)
    {
        return War3.CallNative<bool>(_issueInstantTargetOrderPtr, whichUnit.Handle, order, targetWidget.Handle,
            instantTargetWidget.Handle);
    }

    public static bool IssueInstantTargetOrderById(JUnit whichUnit, int order, JWidget targetWidget,
        JWidget instantTargetWidget)
    {
        return War3.CallNative<bool>(_issueInstantTargetOrderByIdPtr, whichUnit.Handle, order, targetWidget.Handle,
            instantTargetWidget.Handle);
    }

    public static bool IssueBuildOrder(JUnit whichPeon, string unitToBuild, float x, float y)
    {
        return War3.CallNative<bool>(_issueBuildOrderPtr, whichPeon.Handle, unitToBuild, x, y);
    }


    /// title = "发布建造命令(指定坐标) [R]"
    /// description = "命令 ${单位} 建造 ${单位类型} 在坐标:(${X},${Y})"
    /// comment = ""
    public static bool IssueBuildOrderById(JUnit whichPeon, int unitId, float x, float y)
    {
        return War3.CallNative<bool>(_issueBuildOrderByIdPtr, whichPeon.Handle, unitId, x, y);
    }


    /// title = "发布中介命令(无目标)"
    /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令"
    /// comment = "可以用来对非本玩家单位发布命令."
    public static bool IssueNeutralImmediateOrder(JPlayer forWhichPlayer, JUnit neutralStructure, string unitToBuild)
    {
        return War3.CallNative<bool>(_issueNeutralImmediateOrderPtr, forWhichPlayer.Handle, neutralStructure.Handle,
            unitToBuild);
    }


    /// title = "发布中介命令(无目标)(ID)"
    /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令"
    /// comment = "可以用来对非本玩家单位发布命令."
    public static bool IssueNeutralImmediateOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId)
    {
        return War3.CallNative<bool>(_issueNeutralImmediateOrderByIdPtr, forWhichPlayer.Handle, neutralStructure.Handle,
            unitId);
    }


    /// title = "发布中介命令(指定坐标)"
    /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令到坐标:(${X},${Y})"
    /// comment = "可以用来对非本玩家单位发布命令."
    public static bool IssueNeutralPointOrder(JPlayer forWhichPlayer, JUnit neutralStructure, string unitToBuild,
        float x, float y)
    {
        return War3.CallNative<bool>(_issueNeutralPointOrderPtr, forWhichPlayer.Handle, neutralStructure.Handle,
            unitToBuild, x, y);
    }


    /// title = "发布中介命令(指定坐标)(ID)"
    /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令到坐标:(${X},${Y})"
    /// comment = "可以用来对非本玩家单位发布命令."
    public static bool IssueNeutralPointOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId, float x,
        float y)
    {
        return War3.CallNative<bool>(_issueNeutralPointOrderByIdPtr, forWhichPlayer.Handle, neutralStructure.Handle,
            unitId, x, y);
    }


    /// title = "发布中介命令(指定单位)"
    /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令到目标: ${单位}"
    /// comment = "可以用来对非本玩家单位发布命令."
    public static bool IssueNeutralTargetOrder(JPlayer forWhichPlayer, JUnit neutralStructure, string unitToBuild,
        JWidget target)
    {
        return War3.CallNative<bool>(_issueNeutralTargetOrderPtr, forWhichPlayer.Handle, neutralStructure.Handle,
            unitToBuild, target.Handle);
    }


    /// title = "发布中介命令(指定单位)(ID)"
    /// description = "使 ${玩家} 对 ${单位} 发布 ${Order} 命令到目标: ${单位}"
    /// comment = "可以用来对非本玩家单位发布命令."
    public static bool IssueNeutralTargetOrderById(JPlayer forWhichPlayer, JUnit neutralStructure, int unitId,
        JWidget target)
    {
        return War3.CallNative<bool>(_issueNeutralTargetOrderByIdPtr, forWhichPlayer.Handle, neutralStructure.Handle,
            unitId, target.Handle);
    }


    /// title = "当前命令ID"
    /// description = "${单位} 的当前命令ID."
    /// comment = ""
    public static int GetUnitCurrentOrder(JUnit whichUnit)
    {
        return War3.CallNative<int>(_getUnitCurrentOrderPtr, whichUnit.Handle);
    }


    /// title = "设置黄金储量"
    /// description = "设置 ${金矿} 的黄金储量为 ${Quantity}"
    /// comment = ""
    public static void SetResourceAmount(JUnit whichUnit, int amount)
    {
        War3.CallNative(_setResourceAmountPtr, whichUnit.Handle, amount);
    }

    public static void AddResourceAmount(JUnit whichUnit, int amount)
    {
        War3.CallNative(_addResourceAmountPtr, whichUnit.Handle, amount);
    }


    /// title = "储金量"
    /// description = "${金矿} 的储金量"
    /// comment = "只对金矿有效."
    public static int GetResourceAmount(JUnit whichUnit)
    {
        return War3.CallNative<int>(_getResourceAmountPtr, whichUnit.Handle);
    }


    /// title = "传送门目的地X坐标"
    /// description = "${传送门} 的目的地X坐标"
    /// comment = ""
    public static float WaygateGetDestinationX(JUnit waygate)
    {
        return War3.CallNative<float>(_waygateGetDestinationXPtr, waygate.Handle);
    }


    /// title = "传送门目的地Y坐标"
    /// description = "${传送门} 的目的地Y坐标"
    /// comment = ""
    public static float WaygateGetDestinationY(JUnit waygate)
    {
        return War3.CallNative<float>(_waygateGetDestinationYPtr, waygate.Handle);
    }


    /// title = "设置传送门目的坐标 [R]"
    /// description = "设置 ${传送门} 的目的地为(${X},${Y})"
    /// comment = ""
    public static void WaygateSetDestination(JUnit waygate, float x, float y)
    {
        War3.CallNative(_waygateSetDestinationPtr, waygate.Handle, x, y);
    }

    public static void WaygateActivate(JUnit waygate, bool activate)
    {
        War3.CallNative(_waygateActivatePtr, waygate.Handle, activate);
    }

    public static bool WaygateIsActive(JUnit waygate)
    {
        return War3.CallNative<bool>(_waygateIsActivePtr, waygate.Handle);
    }


    /// title = "添加物品(所有市场)"
    /// description = "添加 ${物品类型} 到所有市场并设置库存量: ${Count} 最大库存量: ${Max}"
    /// comment = "影响所有拥有'出售物品'技能的单位."
    public static void AddItemToAllStock(int itemId, int currentStock, int stockMax)
    {
        War3.CallNative(_addItemToAllStockPtr, itemId, currentStock, stockMax);
    }

    public static void AddItemToStock(JUnit whichUnit, int itemId, int currentStock, int stockMax)
    {
        War3.CallNative(_addItemToStockPtr, whichUnit.Handle, itemId, currentStock, stockMax);
    }


    /// title = "添加单位(所有市场)"
    /// description = "添加 ${单位类型} 到所有市场并设置库存量: ${Count} 最大库存量: ${Max}"
    /// comment = "影响所有拥有'出售单位'技能的单位."
    public static void AddUnitToAllStock(int unitId, int currentStock, int stockMax)
    {
        War3.CallNative(_addUnitToAllStockPtr, unitId, currentStock, stockMax);
    }

    public static void AddUnitToStock(JUnit whichUnit, int unitId, int currentStock, int stockMax)
    {
        War3.CallNative(_addUnitToStockPtr, whichUnit.Handle, unitId, currentStock, stockMax);
    }


    /// title = "删除物品(所有市场)"
    /// description = "删除 ${物品类型} 从所有市场"
    /// comment = "影响所有拥有'出售物品'技能的单位."
    public static void RemoveItemFromAllStock(int itemId)
    {
        War3.CallNative(_removeItemFromAllStockPtr, itemId);
    }

    public static void RemoveItemFromStock(JUnit whichUnit, int itemId)
    {
        War3.CallNative(_removeItemFromStockPtr, whichUnit.Handle, itemId);
    }


    /// title = "删除单位(所有市场)"
    /// description = "删除 ${单位类型} 从所有市场"
    /// comment = "影响所有拥有'出售单位'技能的单位."
    public static void RemoveUnitFromAllStock(int unitId)
    {
        War3.CallNative(_removeUnitFromAllStockPtr, unitId);
    }

    public static void RemoveUnitFromStock(JUnit whichUnit, int unitId)
    {
        War3.CallNative(_removeUnitFromStockPtr, whichUnit.Handle, unitId);
    }


    /// title = "限制物品种类(所有市场)"
    /// description = "限制所有市场的可出售物品种类数为 ${Quantity}"
    /// comment = "影响所有拥有'出售物品'技能的单位."
    public static void SetAllItemTypeSlots(int slots)
    {
        War3.CallNative(_setAllItemTypeSlotsPtr, slots);
    }


    /// title = "限制单位种类(所有市场)"
    /// description = "限制所有市场的可出售单位种类数为 ${Quantity}"
    /// comment = "影响所有拥有'出售单位'技能的单位."
    public static void SetAllUnitTypeSlots(int slots)
    {
        War3.CallNative(_setAllUnitTypeSlotsPtr, slots);
    }


    /// title = "限制物品种类(指定市场)"
    /// description = "限制 ${Marketplace} 的可出售物品种类数为 ${Quantity}"
    /// comment = "只影响有'出售物品'技能的单位."
    public static void SetItemTypeSlots(JUnit whichUnit, int slots)
    {
        War3.CallNative(_setItemTypeSlotsPtr, whichUnit.Handle, slots);
    }


    /// title = "限制单位种类(指定市场)"
    /// description = "限制 ${Marketplace} 的可出售单位种类数为 ${Quantity}"
    /// comment = "只影响有'出售单位'技能的单位."
    public static void SetUnitTypeSlots(JUnit whichUnit, int slots)
    {
        War3.CallNative(_setUnitTypeSlotsPtr, whichUnit.Handle, slots);
    }


    /// title = "单位自定义值"
    /// description = "${单位} 的自定义值"
    /// comment = "可使用'单位 - 设置自定义值'来设置该值."
    public static int GetUnitUserData(JUnit whichUnit)
    {
        return War3.CallNative<int>(_getUnitUserDataPtr, whichUnit.Handle);
    }


    /// title = "设置自定义值"
    /// description = "设置 ${单位} 的自定义值为 ${Index}"
    /// comment = "单位自定义值仅用于触发器. 可用来给单位绑定一个整型数据."
    public static void SetUnitUserData(JUnit whichUnit, int data)
    {
        War3.CallNative(_setUnitUserDataPtr, whichUnit.Handle, data);
    }

    public static JPlayer Player(int number)
    {
        var handle = War3.CallNative<int>(_playerPtr, number);
        return new JPlayer(handle);
    }


    /// title = "本地玩家 [R]"
    /// description = "本地玩家"
    /// comment = "指代玩家自己,所以对每个玩家返回值都不一样. 如果不清楚该函数的话千万别用,因为很可能因为不同步而导致掉线."
    public static JPlayer GetLocalPlayer()
    {
        var handle = War3.CallNative<int>(_getLocalPlayerPtr);
        return new JPlayer(handle);
    }


    /// title = "是玩家的盟友"
    /// description = "${Player} 是 ${Player} 的盟友"
    /// comment = "包括中立状态. 单向判断玩家A对玩家B联盟不侵犯,即表示玩家A是玩家B的盟友."
    public static bool IsPlayerAlly(JPlayer whichPlayer, JPlayer otherPlayer)
    {
        return War3.CallNative<bool>(_isPlayerAllyPtr, whichPlayer.Handle, otherPlayer.Handle);
    }


    /// title = "是玩家的敌人"
    /// description = "${Player} 是 ${Player} 的敌人"
    /// comment = "不包括中立状态. 单向判断玩家A对玩家B敌对侵犯,即表示玩家A是玩家B的盟友."
    public static bool IsPlayerEnemy(JPlayer whichPlayer, JPlayer otherPlayer)
    {
        return War3.CallNative<bool>(_isPlayerEnemyPtr, whichPlayer.Handle, otherPlayer.Handle);
    }


    /// title = "在玩家组"
    /// description = "${Player} 在 ${玩家组} 中"
    /// comment = ""
    public static bool IsPlayerInForce(JPlayer whichPlayer, JForce whichForce)
    {
        return War3.CallNative<bool>(_isPlayerInForcePtr, whichPlayer.Handle, whichForce.Handle);
    }


    /// title = "玩家是裁判或观察者 [R]"
    /// description = "${Player}是裁判或观察者"
    /// comment = ""
    public static bool IsPlayerObserver(JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isPlayerObserverPtr, whichPlayer.Handle);
    }


    /// title = "坐标可见"
    /// description = "坐标(${x},${y}) 对 ${玩家} 可见"
    /// comment = ""
    public static bool IsVisibleToPlayer(float x, float y, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isVisibleToPlayerPtr, x, y, whichPlayer.Handle);
    }


    /// title = "点可见"
    /// description = "${指定点}对 ${Player} 可见"
    /// comment = ""
    public static bool IsLocationVisibleToPlayer(JLocation whichLocation, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isLocationVisibleToPlayerPtr, whichLocation.Handle, whichPlayer.Handle);
    }


    /// title = "坐标在迷雾中"
    /// description = "坐标(${x},${y}) 在 ${玩家} 的迷雾范围内"
    /// comment = "黑色阴影内的坐标不被计算在内。"
    public static bool IsFoggedToPlayer(float x, float y, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isFoggedToPlayerPtr, x, y, whichPlayer.Handle);
    }


    /// title = "点在迷雾中"
    /// description = "${指定点} 在 ${Player} 的迷雾范围内"
    /// comment = "黑色阴影内的点不被计算在内."
    public static bool IsLocationFoggedToPlayer(JLocation whichLocation, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isLocationFoggedToPlayerPtr, whichLocation.Handle, whichPlayer.Handle);
    }


    /// title = "坐标在黑色阴影中"
    /// description = "坐标(${x},${y}) 在 ${玩家} 的黑色阴影内"
    /// comment = ""
    public static bool IsMaskedToPlayer(float x, float y, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isMaskedToPlayerPtr, x, y, whichPlayer.Handle);
    }


    /// title = "点在黑色阴影中"
    /// description = "${指定点} 在 ${Player} 的黑色阴影内"
    /// comment = ""
    public static bool IsLocationMaskedToPlayer(JLocation whichLocation, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_isLocationMaskedToPlayerPtr, whichLocation.Handle, whichPlayer.Handle);
    }


    /// title = "玩家的种族"
    /// description = "${Player} 的种族"
    /// comment = ""
    public static JRace GetPlayerRace(JPlayer whichPlayer)
    {
        var handle = War3.CallNative<int>(_getPlayerRacePtr, whichPlayer.Handle);
        return new JRace(handle);
    }


    /// title = "玩家ID - 1 [R]"
    /// description = "${Player} 的玩家ID - 1"
    /// comment = "玩家ID取值1-16."
    public static int GetPlayerId(JPlayer whichPlayer)
    {
        return War3.CallNative<int>(_getPlayerIdPtr, whichPlayer.Handle);
    }


    /// title = "非建筑单位数量"
    /// description = "${Player} 拥有的非建筑单位数量(${Include/Exclude} 未完成的)"
    /// comment = ""
    public static int GetPlayerUnitCount(JPlayer whichPlayer, bool includeIncomplete)
    {
        return War3.CallNative<int>(_getPlayerUnitCountPtr, whichPlayer.Handle, includeIncomplete);
    }

    public static int GetPlayerTypedUnitCount(JPlayer whichPlayer, string unitName, bool includeIncomplete,
        bool includeUpgrades)
    {
        return War3.CallNative<int>(_getPlayerTypedUnitCountPtr, whichPlayer.Handle, unitName, includeIncomplete,
            includeUpgrades);
    }


    /// title = "建筑数量"
    /// description = "${Player} 拥有的建筑数量(${Include/Exclude} 未完成的)"
    /// comment = ""
    public static int GetPlayerStructureCount(JPlayer whichPlayer, bool includeIncomplete)
    {
        return War3.CallNative<int>(_getPlayerStructureCountPtr, whichPlayer.Handle, includeIncomplete);
    }


    /// title = "玩家属性"
    /// description = "${Player} ${Property}"
    /// comment = ""
    public static int GetPlayerState(JPlayer whichPlayer, JPlayerState whichPlayerState)
    {
        return War3.CallNative<int>(_getPlayerStatePtr, whichPlayer.Handle, whichPlayerState.Handle);
    }


    /// title = "玩家得分"
    /// description = "${Player} ${Score}"
    /// comment = ""
    public static int GetPlayerScore(JPlayer whichPlayer, JPlayerScore whichPlayerScore)
    {
        return War3.CallNative<int>(_getPlayerScorePtr, whichPlayer.Handle, whichPlayerScore.Handle);
    }


    /// title = "联盟状态设置"
    /// description = "${Player} 对 ${Player} 开启 ${Alliance Type}"
    /// comment = ""
    public static bool GetPlayerAlliance(JPlayer sourcePlayer, JPlayer otherPlayer, JAllianceType whichAllianceSetting)
    {
        return War3.CallNative<bool>(_getPlayerAlliancePtr, sourcePlayer.Handle, otherPlayer.Handle,
            whichAllianceSetting.Handle);
    }

    public static float GetPlayerHandicap(JPlayer whichPlayer)
    {
        return War3.CallNative<float>(_getPlayerHandicapPtr, whichPlayer.Handle);
    }

    public static float GetPlayerHandicapXP(JPlayer whichPlayer)
    {
        return War3.CallNative<float>(_getPlayerHandicapXPPtr, whichPlayer.Handle);
    }


    /// title = "设置生命上限 [R]"
    /// description = "设置 ${Player} 的生命障碍为正常的 ${Percent}倍"
    /// comment = "生命上限影响玩家拥有单位的生命最大值. 生命之书并不受生命上限限制,所以对英雄血量可能会有偏差."
    public static void SetPlayerHandicap(JPlayer whichPlayer, float handicap)
    {
        War3.CallNative(_setPlayerHandicapPtr, whichPlayer.Handle, handicap);
    }


    /// title = "设置经验获得率 [R]"
    /// description = "设置 ${Player} 的经验获得率为正常的 ${Value} 倍"
    /// comment = ""
    public static void SetPlayerHandicapXP(JPlayer whichPlayer, float handicap)
    {
        War3.CallNative(_setPlayerHandicapXPPtr, whichPlayer.Handle, handicap);
    }

    public static void SetPlayerTechMaxAllowed(JPlayer whichPlayer, int techid, int maximum)
    {
        War3.CallNative(_setPlayerTechMaxAllowedPtr, whichPlayer.Handle, techid, maximum);
    }

    public static int GetPlayerTechMaxAllowed(JPlayer whichPlayer, int techid)
    {
        return War3.CallNative<int>(_getPlayerTechMaxAllowedPtr, whichPlayer.Handle, techid);
    }


    /// title = "增加科技等级"
    /// description = "增加 ${玩家} 的 ${科技} 科技 ${整数} 级"
    /// comment = "科技等级不能倒退。"
    public static void AddPlayerTechResearched(JPlayer whichPlayer, int techid, int levels)
    {
        War3.CallNative(_addPlayerTechResearchedPtr, whichPlayer.Handle, techid, levels);
    }

    public static void SetPlayerTechResearched(JPlayer whichPlayer, int techid, int setToLevel)
    {
        War3.CallNative(_setPlayerTechResearchedPtr, whichPlayer.Handle, techid, setToLevel);
    }

    public static bool GetPlayerTechResearched(JPlayer whichPlayer, int techid, bool specificonly)
    {
        return War3.CallNative<bool>(_getPlayerTechResearchedPtr, whichPlayer.Handle, techid, specificonly);
    }

    public static int GetPlayerTechCount(JPlayer whichPlayer, int techid, bool specificonly)
    {
        return War3.CallNative<int>(_getPlayerTechCountPtr, whichPlayer.Handle, techid, specificonly);
    }

    public static void SetPlayerUnitsOwner(JPlayer whichPlayer, int newOwner)
    {
        War3.CallNative(_setPlayerUnitsOwnerPtr, whichPlayer.Handle, newOwner);
    }

    public static void CripplePlayer(JPlayer whichPlayer, JForce toWhichPlayers, bool flag)
    {
        War3.CallNative(_cripplePlayerPtr, whichPlayer.Handle, toWhichPlayers.Handle, flag);
    }


    /// title = "允许/禁用 技能 [R]"
    /// description = "设置 ${Player} 的 ${技能} 为 ${Enable/Disable}"
    /// comment = "设置玩家能否使用该技能."
    public static void SetPlayerAbilityAvailable(JPlayer whichPlayer, int abilid, bool avail)
    {
        War3.CallNative(_setPlayerAbilityAvailablePtr, whichPlayer.Handle, abilid, avail);
    }


    /// title = "设置属性"
    /// description = "设置 ${Player} 的 ${Property} 为 ${Value}"
    /// comment = ""
    public static void SetPlayerState(JPlayer whichPlayer, JPlayerState whichPlayerState, int value)
    {
        War3.CallNative(_setPlayerStatePtr, whichPlayer.Handle, whichPlayerState.Handle, value);
    }


    /// title = "踢除玩家"
    /// description = "踢除 ${Player} ，玩家的游戏结果为 ${文字}"
    /// comment = ""
    public static void RemovePlayer(JPlayer whichPlayer, JPlayerGameResult gameResult)
    {
        War3.CallNative(_removePlayerPtr, whichPlayer.Handle, gameResult.Handle);
    }

    public static void CachePlayerHeroData(JPlayer whichPlayer)
    {
        War3.CallNative(_cachePlayerHeroDataPtr, whichPlayer.Handle);
    }


    /// title = "设置地图迷雾(矩形区域) [R]"
    /// description = "为 ${玩家} 设置 ${FogStateVisible} 在 ${矩形区域} (对盟友 ${共享} 视野)"
    /// comment = ""
    public static void SetFogStateRect(JPlayer forWhichPlayer, JFogState whichState, JRect where, bool useSharedVision)
    {
        War3.CallNative(_setFogStateRectPtr, forWhichPlayer.Handle, whichState.Handle, where.Handle, useSharedVision);
    }


    /// title = "设置地图迷雾(圆范围) [R]"
    /// description = "为 ${玩家} 设置 ${FogStateVisible} 在圆心为(${X},${Y}) 半径为 ${数值} 的范围, (对盟友 ${共享} 视野)"
    /// comment = ""
    public static void SetFogStateRadius(JPlayer forWhichPlayer, JFogState whichState, float centerx, float centerY,
        float radius, bool useSharedVision)
    {
        War3.CallNative(_setFogStateRadiusPtr, forWhichPlayer.Handle, whichState.Handle, centerx, centerY, radius,
            useSharedVision);
    }

    public static void SetFogStateRadiusLoc(JPlayer forWhichPlayer, JFogState whichState, JLocation center,
        float radius, bool useSharedVision)
    {
        War3.CallNative(_setFogStateRadiusLocPtr, forWhichPlayer.Handle, whichState.Handle, center.Handle, radius,
            useSharedVision);
    }


    /// title = "启用/禁用黑色阴影 [R]"
    /// description = "${启用/禁用} 黑色阴影"
    /// comment = ""
    public static void FogMaskEnable(bool enable)
    {
        War3.CallNative(_fogMaskEnablePtr, enable);
    }


    /// title = "黑色阴影开启"
    /// description = "黑色阴影开启"
    /// comment = ""
    public static bool IsFogMaskEnabled()
    {
        return War3.CallNative<bool>(_isFogMaskEnabledPtr);
    }


    /// title = "启用/禁用 战争迷雾 [R]"
    /// description = "${启用/禁用} 战争迷雾"
    /// comment = ""
    public static void FogEnable(bool enable)
    {
        War3.CallNative(_fogEnablePtr, enable);
    }


    /// title = "战争迷雾开启"
    /// description = "战争迷雾开启"
    /// comment = ""
    public static bool IsFogEnabled()
    {
        return War3.CallNative<bool>(_isFogEnabledPtr);
    }


    /// title = "新建可见度修正器(矩形区域) [R]"
    /// description = "新建的 ${玩家} 可见度修正器. 可见度: ${FogStateVisible} 影响区域: ${矩形区域} (对盟友 ${共享} 视野, ${覆盖} 单位视野)"
    /// comment = "会创建可见度修正器."
    public static JFogModifier CreateFogModifierRect(JPlayer forWhichPlayer, JFogState whichState, JRect where,
        bool useSharedVision, bool afterUnits)
    {
        var handle = War3.CallNative<int>(_createFogModifierRectPtr, forWhichPlayer.Handle, whichState.Handle,
            where.Handle, useSharedVision, afterUnits);
        return new JFogModifier(handle);
    }


    /// title = "新建可见度修正器(圆范围) [R]"
    /// description = "新建的 ${玩家} 可见度修正器. 可见度: ${FogStateVisible} 圆心坐标:(${X},${Y}) 半径: ${数值} (对盟友 ${共享} 视野, ${覆盖} 单位视野)"
    /// comment = "会创建可见度修正器."
    public static JFogModifier CreateFogModifierRadius(JPlayer forWhichPlayer, JFogState whichState, float centerx,
        float centerY, float radius, bool useSharedVision, bool afterUnits)
    {
        var handle = War3.CallNative<int>(_createFogModifierRadiusPtr, forWhichPlayer.Handle, whichState.Handle,
            centerx, centerY, radius, useSharedVision, afterUnits);
        return new JFogModifier(handle);
    }

    public static JFogModifier CreateFogModifierRadiusLoc(JPlayer forWhichPlayer, JFogState whichState,
        JLocation center, float radius, bool useSharedVision, bool afterUnits)
    {
        var handle = War3.CallNative<int>(_createFogModifierRadiusLocPtr, forWhichPlayer.Handle, whichState.Handle,
            center.Handle, radius, useSharedVision, afterUnits);
        return new JFogModifier(handle);
    }


    /// title = "删除可见度修正器"
    /// description = "删除 ${Visibility Modifier}"
    /// comment = ""
    public static void DestroyFogModifier(JFogModifier whichFogModifier)
    {
        War3.CallNative(_destroyFogModifierPtr, whichFogModifier.Handle);
    }


    /// title = "启用可见度修正器"
    /// description = "启用 ${Visibility Modifier}"
    /// comment = ""
    public static void FogModifierStart(JFogModifier whichFogModifier)
    {
        War3.CallNative(_fogModifierStartPtr, whichFogModifier.Handle);
    }


    /// title = "禁用可见度修正器"
    /// description = "禁用 ${Visibility Modifier}"
    /// comment = ""
    public static void FogModifierStop(JFogModifier whichFogModifier)
    {
        War3.CallNative(_fogModifierStopPtr, whichFogModifier.Handle);
    }

    public static JVersion VersionGet()
    {
        var handle = War3.CallNative<int>(_versionGetPtr);
        return new JVersion(handle);
    }

    public static bool VersionCompatible(JVersion whichVersion)
    {
        return War3.CallNative<bool>(_versionCompatiblePtr, whichVersion.Handle);
    }

    public static bool VersionSupported(JVersion whichVersion)
    {
        return War3.CallNative<bool>(_versionSupportedPtr, whichVersion.Handle);
    }

    public static void EndGame(bool doScoreScreen)
    {
        War3.CallNative(_endGamePtr, doScoreScreen);
    }


    /// title = "切换关卡 [R]"
    /// description = "切换到关卡: ${Filename} (${Show/Skip} 计分屏)"
    /// comment = ""
    public static void ChangeLevel(string newLevel, bool doScoreScreen)
    {
        War3.CallNative(_changeLevelPtr, newLevel, doScoreScreen);
    }

    public static void RestartGame(bool doScoreScreen)
    {
        War3.CallNative(_restartGamePtr, doScoreScreen);
    }

    public static void ReloadGame()
    {
        War3.CallNative(_reloadGamePtr);
    }

    public static void SetCampaignMenuRace(JRace r)
    {
        War3.CallNative(_setCampaignMenuRacePtr, r.Handle);
    }

    public static void SetCampaignMenuRaceEx(int campaignIndex)
    {
        War3.CallNative(_setCampaignMenuRaceExPtr, campaignIndex);
    }

    public static void ForceCampaignSelectScreen()
    {
        War3.CallNative(_forceCampaignSelectScreenPtr);
    }

    public static void LoadGame(string saveFileName, bool doScoreScreen)
    {
        War3.CallNative(_loadGamePtr, saveFileName, doScoreScreen);
    }


    /// title = "保存进度 [R]"
    /// description = "保存游戏进度为: ${Filename}"
    /// comment = ""
    public static void SaveGame(string saveFileName)
    {
        War3.CallNative(_saveGamePtr, saveFileName);
    }

    public static bool RenameSaveDirectory(string sourceDirName, string destDirName)
    {
        return War3.CallNative<bool>(_renameSaveDirectoryPtr, sourceDirName, destDirName);
    }

    public static bool RemoveSaveDirectory(string sourceDirName)
    {
        return War3.CallNative<bool>(_removeSaveDirectoryPtr, sourceDirName);
    }

    public static bool CopySaveGame(string sourceSaveName, string destSaveName)
    {
        return War3.CallNative<bool>(_copySaveGamePtr, sourceSaveName, destSaveName);
    }


    /// title = "游戏存档存在"
    /// description = "${存档文件} 已存在"
    /// comment = ""
    public static bool SaveGameExists(string saveName)
    {
        return War3.CallNative<bool>(_saveGameExistsPtr, saveName);
    }

    public static void SyncSelections()
    {
        War3.CallNative(_syncSelectionsPtr);
    }

    public static void SetFloatGameState(JFGameState whichFloatGameState, float value)
    {
        War3.CallNative(_setFloatGameStatePtr, whichFloatGameState.Handle, value);
    }

    public static float GetFloatGameState(JFGameState whichFloatGameState)
    {
        return War3.CallNative<float>(_getFloatGameStatePtr, whichFloatGameState.Handle);
    }

    public static void SetIntegerGameState(JIGameState whichIntegerGameState, int value)
    {
        War3.CallNative(_setIntegerGameStatePtr, whichIntegerGameState.Handle, value);
    }

    public static int GetIntegerGameState(JIGameState whichIntegerGameState)
    {
        return War3.CallNative<int>(_getIntegerGameStatePtr, whichIntegerGameState.Handle);
    }

    public static void SetTutorialCleared(bool cleared)
    {
        War3.CallNative(_setTutorialClearedPtr, cleared);
    }

    public static void SetMissionAvailable(int campaignNumber, int missionNumber, bool available)
    {
        War3.CallNative(_setMissionAvailablePtr, campaignNumber, missionNumber, available);
    }

    public static void SetCampaignAvailable(int campaignNumber, bool available)
    {
        War3.CallNative(_setCampaignAvailablePtr, campaignNumber, available);
    }

    public static void SetOpCinematicAvailable(int campaignNumber, bool available)
    {
        War3.CallNative(_setOpCinematicAvailablePtr, campaignNumber, available);
    }

    public static void SetEdCinematicAvailable(int campaignNumber, bool available)
    {
        War3.CallNative(_setEdCinematicAvailablePtr, campaignNumber, available);
    }

    public static JGameDifficulty GetDefaultDifficulty()
    {
        var handle = War3.CallNative<int>(_getDefaultDifficultyPtr);
        return new JGameDifficulty(handle);
    }

    public static void SetDefaultDifficulty(JGameDifficulty g)
    {
        War3.CallNative(_setDefaultDifficultyPtr, g.Handle);
    }

    public static void SetCustomCampaignButtonVisible(int whichButton, bool visible)
    {
        War3.CallNative(_setCustomCampaignButtonVisiblePtr, whichButton, visible);
    }

    public static bool GetCustomCampaignButtonVisible(int whichButton)
    {
        return War3.CallNative<bool>(_getCustomCampaignButtonVisiblePtr, whichButton);
    }


    /// title = "关闭游戏录像功能 [R]"
    /// description = "关闭游戏录像功能"
    /// comment = "游戏结束时不保存游戏录像."
    public static void DoNotSaveReplay()
    {
        War3.CallNative(_doNotSaveReplayPtr);
    }


    /// title = "新建对话框 [R]"
    /// description = "新建对话框"
    /// comment = "创建新的对话框."
    public static JDialog DialogCreate()
    {
        var handle = War3.CallNative<int>(_dialogCreatePtr);
        return new JDialog(handle);
    }


    /// title = "删除 [R]"
    /// description = "删除 ${对话框}"
    /// comment = "将对话框清除出内存.一般来说对话框并不需要删除."
    public static void DialogDestroy(JDialog whichDialog)
    {
        War3.CallNative(_dialogDestroyPtr, whichDialog.Handle);
    }

    public static void DialogClear(JDialog whichDialog)
    {
        War3.CallNative(_dialogClearPtr, whichDialog.Handle);
    }

    public static void DialogSetMessage(JDialog whichDialog, string messageText)
    {
        War3.CallNative(_dialogSetMessagePtr, whichDialog.Handle, messageText);
    }


    /// title = "添加对话框按钮 [R]"
    /// description = "给 ${对话框} 添加按钮, 使用标题: ${文字} 快捷键: ${HotKey}"
    /// comment = "会创建对话框按钮."
    public static JButton DialogAddButton(JDialog whichDialog, string buttonText, int hotkey)
    {
        var handle = War3.CallNative<int>(_dialogAddButtonPtr, whichDialog.Handle, buttonText, hotkey);
        return new JButton(handle);
    }


    /// title = "添加退出游戏按钮 [R]"
    /// description = "为 ${对话框} 添加退出游戏按钮(${跳过} 计分屏) 按钮标题为: ${文字} 快捷键为: ${HotKey}"
    /// comment = "该函数创建的按钮并不被纪录到'最后创建的对话框按钮'.当该按钮被点击时会退出游戏"
    public static JButton DialogAddQuitButton(JDialog whichDialog, bool doScoreScreen, string buttonText, int hotkey)
    {
        var handle = War3.CallNative<int>(_dialogAddQuitButtonPtr, whichDialog.Handle, doScoreScreen, buttonText,
            hotkey);
        return new JButton(handle);
    }


    /// title = "显示/隐藏 [R]"
    /// description = "对 ${Player} 设置 ${对话框} 的状态为 ${Show/Hide}"
    /// comment = "对话框不能应用于地图初始化事件."
    public static void DialogDisplay(JPlayer whichPlayer, JDialog whichDialog, bool flag)
    {
        War3.CallNative(_dialogDisplayPtr, whichPlayer.Handle, whichDialog.Handle, flag);
    }


    /// title = "读取本地缓存数据"
    /// description = "从本地硬盘读取缓存数据"
    /// comment = "只对单机游戏有效,从本地硬盘读取缓存数据,主要用来实现战役关卡间的数据传递."
    public static bool ReloadGameCachesFromDisk()
    {
        return War3.CallNative<bool>(_reloadGameCachesFromDiskPtr);
    }


    /// title = "新建游戏缓存 [R]"
    /// description = "新建游戏缓存: ${缓存名}"
    /// comment = "创建一个新的游戏缓存,一个地图最多只有有256个游戏缓存."
    public static JGameCache InitGameCache(string campaignFile)
    {
        var handle = War3.CallNative<int>(_initGameCachePtr, campaignFile);
        return new JGameCache(handle);
    }

    public static bool SaveGameCache(JGameCache whichCache)
    {
        return War3.CallNative<bool>(_saveGameCachePtr, whichCache.Handle);
    }


    /// title = "记录整数"
    /// description = "缓存: ${Game Cache}  类别名: ${Category} 使用名称: ${文字} 记录: ${整数}"
    /// comment = "使用'游戏缓存 - 读取整数'来读取该数值. 名称和类别名不能包含空格."
    public static void StoreInteger(JGameCache cache, string missionKey, string key, int value)
    {
        War3.CallNative(_storeIntegerPtr, cache.Handle, missionKey, key, value);
    }


    /// title = "记录实数"
    /// description = "缓存: ${Game Cache}  类别名: ${Category} 使用名称: ${文字} 记录: ${实数}"
    /// comment = "使用'游戏缓存 - 读取实数'来读取该数值. 名称和类别名不能包含空格."
    public static void StoreReal(JGameCache cache, string missionKey, string key, float value)
    {
        War3.CallNative(_storeRealPtr, cache.Handle, missionKey, key, value);
    }


    /// title = "记录布尔值"
    /// description = "缓存: ${Game Cache}  类别名: ${Category} 使用名称: ${文字} 记录: ${布尔值}"
    /// comment = "使用'游戏缓存 - 读取布尔值'来读取该值. 名称和类别名不能包含空格."
    public static void StoreBoolean(JGameCache cache, string missionKey, string key, bool value)
    {
        War3.CallNative(_storeBooleanPtr, cache.Handle, missionKey, key, value);
    }

    public static bool StoreUnit(JGameCache cache, string missionKey, string key, JUnit whichUnit)
    {
        return War3.CallNative<bool>(_storeUnitPtr, cache.Handle, missionKey, key, whichUnit.Handle);
    }


    /// title = "记录字符串"
    /// description = "缓存: ${Game Cache}  类别名: ${Category} 使用名称: ${文字} 记录: ${字符串}"
    /// comment = "使用'游戏缓存 - 读取字符串'来读取该值. 名称和类别名不能包含空格."
    public static bool StoreString(JGameCache cache, string missionKey, string key, string value)
    {
        return War3.CallNative<bool>(_storeStringPtr, cache.Handle, missionKey, key, value);
    }

    public static void SyncStoredInteger(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_syncStoredIntegerPtr, cache.Handle, missionKey, key);
    }

    public static void SyncStoredReal(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_syncStoredRealPtr, cache.Handle, missionKey, key);
    }

    public static void SyncStoredBoolean(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_syncStoredBooleanPtr, cache.Handle, missionKey, key);
    }

    public static void SyncStoredUnit(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_syncStoredUnitPtr, cache.Handle, missionKey, key);
    }

    public static void SyncStoredString(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_syncStoredStringPtr, cache.Handle, missionKey, key);
    }

    public static bool HaveStoredInteger(JGameCache cache, string missionKey, string key)
    {
        return War3.CallNative<bool>(_haveStoredIntegerPtr, cache.Handle, missionKey, key);
    }

    public static bool HaveStoredReal(JGameCache cache, string missionKey, string key)
    {
        return War3.CallNative<bool>(_haveStoredRealPtr, cache.Handle, missionKey, key);
    }

    public static bool HaveStoredBoolean(JGameCache cache, string missionKey, string key)
    {
        return War3.CallNative<bool>(_haveStoredBooleanPtr, cache.Handle, missionKey, key);
    }

    public static bool HaveStoredUnit(JGameCache cache, string missionKey, string key)
    {
        return War3.CallNative<bool>(_haveStoredUnitPtr, cache.Handle, missionKey, key);
    }

    public static bool HaveStoredString(JGameCache cache, string missionKey, string key)
    {
        return War3.CallNative<bool>(_haveStoredStringPtr, cache.Handle, missionKey, key);
    }


    /// title = "删除缓存 [C]"
    /// description = "删除 ${GameCache}"
    /// comment = "删除并清空该缓存的所有数据.和原版UI完全一致，但却不能兼容原版UI，它的存在完全是在添乱啊"
    public static void FlushGameCache(JGameCache cache)
    {
        War3.CallNative(_flushGameCachePtr, cache.Handle);
    }


    /// title = "删除类别"
    /// description = "删除缓存 ${GameCache} 中 ${Category} 类别"
    /// comment = "清空该类别下的所有缓存数据."
    public static void FlushStoredMission(JGameCache cache, string missionKey)
    {
        War3.CallNative(_flushStoredMissionPtr, cache.Handle, missionKey);
    }

    public static void FlushStoredInteger(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_flushStoredIntegerPtr, cache.Handle, missionKey, key);
    }

    public static void FlushStoredReal(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_flushStoredRealPtr, cache.Handle, missionKey, key);
    }

    public static void FlushStoredBoolean(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_flushStoredBooleanPtr, cache.Handle, missionKey, key);
    }

    public static void FlushStoredUnit(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_flushStoredUnitPtr, cache.Handle, missionKey, key);
    }

    public static void FlushStoredString(JGameCache cache, string missionKey, string key)
    {
        War3.CallNative(_flushStoredStringPtr, cache.Handle, missionKey, key);
    }


    /// title = "缓存读取整数 [C]"
    /// description = "从${Game Cache}中读取整数值,类别: ${Category},名称: ${文字}"
    /// comment = "如果该值不存在则返回0."
    public static int GetStoredInteger(JGameCache cache, string missionKey, string key)
    {
        return War3.CallNative<int>(_getStoredIntegerPtr, cache.Handle, missionKey, key);
    }


    /// title = "缓存读取实数 [C]"
    /// description = "从 ${Game Cache} 中读取实数,类别: ${Category} 名称: ${文字}"
    /// comment = "如果该值不存在则返回0."
    public static float GetStoredReal(JGameCache cache, string missionKey, string key)
    {
        return War3.CallNative<float>(_getStoredRealPtr, cache.Handle, missionKey, key);
    }


    /// title = "读取布尔值[R]"
    /// description = "从${Game Cache}中读取布尔值,类别: ${Category},名称: ${文字}"
    /// comment = "如果该值不存在则返回false."
    public static bool GetStoredBoolean(JGameCache cache, string missionKey, string key)
    {
        return War3.CallNative<bool>(_getStoredBooleanPtr, cache.Handle, missionKey, key);
    }


    /// title = "读取字符串 [C]"
    /// description = "从 ${Game Cache} 中读取字符串,类别: ${Category} 名称: ${文字}"
    /// comment = "如果该值不存在,则返回空字符串. 注意,空字符串不等于null"
    public static string GetStoredString(JGameCache cache, string missionKey, string key)
    {
        return War3.CallNative<string>(_getStoredStringPtr, cache.Handle, missionKey, key);
    }

    public static JUnit RestoreUnit(JGameCache cache, string missionKey, string key, JPlayer forWhichPlayer, float x,
        float y, float facing)
    {
        var handle = War3.CallNative<int>(_restoreUnitPtr, cache.Handle, missionKey, key, forWhichPlayer.Handle, x, y,
            facing);
        return new JUnit(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 新建哈希表 [C]"
    /// description = "创建一个新的哈希表"
    /// comment = "您可以使用哈希表来储存临时数据"
    public static JHashtable InitHashtable()
    {
        var handle = War3.CallNative<int>(_initHashtablePtr);
        return new JHashtable(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存整数 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存整数 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取整数' 可以取出保存的值"
    public static void SaveInteger(JHashtable table, int parentKey, int childKey, int value)
    {
        War3.CallNative(_saveIntegerPtr, table.Handle, parentKey, childKey, value);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存实数 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存实数 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取实数' 可以取出保存的值"
    public static void SaveReal(JHashtable table, int parentKey, int childKey, float value)
    {
        War3.CallNative(_saveRealPtr, table.Handle, parentKey, childKey, value);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存布尔 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存布尔 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取布尔' 可以取出保存的值"
    public static void SaveBoolean(JHashtable table, int parentKey, int childKey, bool value)
    {
        War3.CallNative(_saveBooleanPtr, table.Handle, parentKey, childKey, value);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存字符串 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存字符串 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取字符串' 可以取出保存的值"
    public static bool SaveStr(JHashtable table, int parentKey, int childKey, string value)
    {
        return War3.CallNative<bool>(_saveStrPtr, table.Handle, parentKey, childKey, value);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存玩家 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存玩家 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取玩家' 可以取出保存的值"
    public static bool SavePlayerHandle(JHashtable table, int parentKey, int childKey, JPlayer whichPlayer)
    {
        return War3.CallNative<bool>(_savePlayerHandlePtr, table.Handle, parentKey, childKey, whichPlayer.Handle);
    }

    public static bool SaveWidgetHandle(JHashtable table, int parentKey, int childKey, JWidget whichWidget)
    {
        return War3.CallNative<bool>(_saveWidgetHandlePtr, table.Handle, parentKey, childKey, whichWidget.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存可破坏物 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存可破坏物 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取可破坏物' 可以取出保存的值"
    public static bool SaveDestructableHandle(JHashtable table, int parentKey, int childKey,
        JDestructable whichDestructable)
    {
        return War3.CallNative<bool>(_saveDestructableHandlePtr, table.Handle, parentKey, childKey,
            whichDestructable.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存物品 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存物品 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取物品' 可以取出保存的值"
    public static bool SaveItemHandle(JHashtable table, int parentKey, int childKey, JItem whichItem)
    {
        return War3.CallNative<bool>(_saveItemHandlePtr, table.Handle, parentKey, childKey, whichItem.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存单位 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存单位 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取单位' 可以取出保存的值"
    public static bool SaveUnitHandle(JHashtable table, int parentKey, int childKey, JUnit whichUnit)
    {
        return War3.CallNative<bool>(_saveUnitHandlePtr, table.Handle, parentKey, childKey, whichUnit.Handle);
    }

    public static bool SaveAbilityHandle(JHashtable table, int parentKey, int childKey, JAbility whichAbility)
    {
        return War3.CallNative<bool>(_saveAbilityHandlePtr, table.Handle, parentKey, childKey, whichAbility.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存计时器 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存计时器 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取计时器' 可以取出保存的值"
    public static bool SaveTimerHandle(JHashtable table, int parentKey, int childKey, JTimer whichTimer)
    {
        return War3.CallNative<bool>(_saveTimerHandlePtr, table.Handle, parentKey, childKey, whichTimer.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存触发器 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存触发器 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取触发器' 可以取出保存的值"
    public static bool SaveTriggerHandle(JHashtable table, int parentKey, int childKey, JTrigger whichTrigger)
    {
        return War3.CallNative<bool>(_saveTriggerHandlePtr, table.Handle, parentKey, childKey, whichTrigger.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存触发条件 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存触发条件 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取触发条件' 可以取出保存的值"
    public static bool SaveTriggerConditionHandle(JHashtable table, int parentKey, int childKey,
        JTriggerCondition whichTriggercondition)
    {
        return War3.CallNative<bool>(_saveTriggerConditionHandlePtr, table.Handle, parentKey, childKey,
            whichTriggercondition.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存触发动作 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存触发动作 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取触发动作' 可以取出保存的值"
    public static bool SaveTriggerActionHandle(JHashtable table, int parentKey, int childKey,
        JTriggerAction whichTriggeraction)
    {
        return War3.CallNative<bool>(_saveTriggerActionHandlePtr, table.Handle, parentKey, childKey,
            whichTriggeraction.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存触发事件 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存触发事件 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取触发事件' 可以取出保存的值"
    public static bool SaveTriggerEventHandle(JHashtable table, int parentKey, int childKey, JEvent whichEvent)
    {
        return War3.CallNative<bool>(_saveTriggerEventHandlePtr, table.Handle, parentKey, childKey, whichEvent.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存玩家组 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存玩家组 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取玩家组' 可以取出保存的值"
    public static bool SaveForceHandle(JHashtable table, int parentKey, int childKey, JForce whichForce)
    {
        return War3.CallNative<bool>(_saveForceHandlePtr, table.Handle, parentKey, childKey, whichForce.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存单位组 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存单位组 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取单位组' 可以取出保存的值"
    public static bool SaveGroupHandle(JHashtable table, int parentKey, int childKey, JGroup whichGroup)
    {
        return War3.CallNative<bool>(_saveGroupHandlePtr, table.Handle, parentKey, childKey, whichGroup.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存点 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存点 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取点' 可以取出保存的值"
    public static bool SaveLocationHandle(JHashtable table, int parentKey, int childKey, JLocation whichLocation)
    {
        return War3.CallNative<bool>(_saveLocationHandlePtr, table.Handle, parentKey, childKey, whichLocation.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存区域(矩型) [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存区域(矩型) ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取区域(矩型)' 可以取出保存的值"
    public static bool SaveRectHandle(JHashtable table, int parentKey, int childKey, JRect whichRect)
    {
        return War3.CallNative<bool>(_saveRectHandlePtr, table.Handle, parentKey, childKey, whichRect.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存布尔表达式 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存布尔表达式 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取布尔表达式' 可以取出保存的值"
    public static bool SaveBooleanExprHandle(JHashtable table, int parentKey, int childKey, JBoolExpr whichBoolexpr)
    {
        return War3.CallNative<bool>(_saveBooleanExprHandlePtr, table.Handle, parentKey, childKey,
            whichBoolexpr.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存音效 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存音效 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取音效' 可以取出保存的值"
    public static bool SaveSoundHandle(JHashtable table, int parentKey, int childKey, JSound whichSound)
    {
        return War3.CallNative<bool>(_saveSoundHandlePtr, table.Handle, parentKey, childKey, whichSound.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存特效 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存特效 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取特效' 可以取出保存的值"
    public static bool SaveEffectHandle(JHashtable table, int parentKey, int childKey, JEffect whichEffect)
    {
        return War3.CallNative<bool>(_saveEffectHandlePtr, table.Handle, parentKey, childKey, whichEffect.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存单位池 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存单位池 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取单位池' 可以取出保存的值"
    public static bool SaveUnitPoolHandle(JHashtable table, int parentKey, int childKey, JUnitPool whichUnitpool)
    {
        return War3.CallNative<bool>(_saveUnitPoolHandlePtr, table.Handle, parentKey, childKey, whichUnitpool.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存物品池 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存物品池 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取物品池' 可以取出保存的值"
    public static bool SaveItemPoolHandle(JHashtable table, int parentKey, int childKey, JItemPool whichItempool)
    {
        return War3.CallNative<bool>(_saveItemPoolHandlePtr, table.Handle, parentKey, childKey, whichItempool.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存任务 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存任务 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取任务' 可以取出保存的值"
    public static bool SaveQuestHandle(JHashtable table, int parentKey, int childKey, JQuest whichQuest)
    {
        return War3.CallNative<bool>(_saveQuestHandlePtr, table.Handle, parentKey, childKey, whichQuest.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存任务要求 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存任务要求 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取任务要求' 可以取出保存的值"
    public static bool SaveQuestItemHandle(JHashtable table, int parentKey, int childKey, JQuestItem whichQuestitem)
    {
        return War3.CallNative<bool>(_saveQuestItemHandlePtr, table.Handle, parentKey, childKey, whichQuestitem.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存失败条件 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存失败条件 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取失败条件' 可以取出保存的值"
    public static bool SaveDefeatConditionHandle(JHashtable table, int parentKey, int childKey,
        JDefeatCondition whichDefeatcondition)
    {
        return War3.CallNative<bool>(_saveDefeatConditionHandlePtr, table.Handle, parentKey, childKey,
            whichDefeatcondition.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存计时器窗口 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存计时器窗口 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取计时器窗口' 可以取出保存的值"
    public static bool SaveTimerDialogHandle(JHashtable table, int parentKey, int childKey,
        JTimerDialog whichTimerdialog)
    {
        return War3.CallNative<bool>(_saveTimerDialogHandlePtr, table.Handle, parentKey, childKey,
            whichTimerdialog.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存排行榜 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存排行榜 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取排行榜' 可以取出保存的值"
    public static bool SaveLeaderboardHandle(JHashtable table, int parentKey, int childKey,
        JLeaderboard whichLeaderboard)
    {
        return War3.CallNative<bool>(_saveLeaderboardHandlePtr, table.Handle, parentKey, childKey,
            whichLeaderboard.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存多面板 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存多面板 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取多面板' 可以取出保存的值"
    public static bool SaveMultiboardHandle(JHashtable table, int parentKey, int childKey, JMultiboard whichMultiboard)
    {
        return War3.CallNative<bool>(_saveMultiboardHandlePtr, table.Handle, parentKey, childKey,
            whichMultiboard.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存多面板项目 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存多面板项目 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取多面板项目' 可以取出保存的值"
    public static bool SaveMultiboardItemHandle(JHashtable table, int parentKey, int childKey,
        JMultiboardItem whichMultiboarditem)
    {
        return War3.CallNative<bool>(_saveMultiboardItemHandlePtr, table.Handle, parentKey, childKey,
            whichMultiboarditem.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存可追踪物 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存可追踪物 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取可追踪物' 可以取出保存的值"
    public static bool SaveTrackableHandle(JHashtable table, int parentKey, int childKey, JTrackable whichTrackable)
    {
        return War3.CallNative<bool>(_saveTrackableHandlePtr, table.Handle, parentKey, childKey, whichTrackable.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存对话框 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存对话框 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取对话框' 可以取出保存的值"
    public static bool SaveDialogHandle(JHashtable table, int parentKey, int childKey, JDialog whichDialog)
    {
        return War3.CallNative<bool>(_saveDialogHandlePtr, table.Handle, parentKey, childKey, whichDialog.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存对话框按钮 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存对话框按钮 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取对话框按钮' 可以取出保存的值"
    public static bool SaveButtonHandle(JHashtable table, int parentKey, int childKey, JButton whichButton)
    {
        return War3.CallNative<bool>(_saveButtonHandlePtr, table.Handle, parentKey, childKey, whichButton.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存漂浮文字 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存漂浮文字 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取漂浮文字' 可以取出保存的值"
    public static bool SaveTextTagHandle(JHashtable table, int parentKey, int childKey, JTextTag whichTexttag)
    {
        return War3.CallNative<bool>(_saveTextTagHandlePtr, table.Handle, parentKey, childKey, whichTexttag.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存闪电效果 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存闪电效果 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取闪电效果' 可以取出保存的值"
    public static bool SaveLightningHandle(JHashtable table, int parentKey, int childKey, JLightning whichLightning)
    {
        return War3.CallNative<bool>(_saveLightningHandlePtr, table.Handle, parentKey, childKey, whichLightning.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存图像 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存图像 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取图像' 可以取出保存的值"
    public static bool SaveImageHandle(JHashtable table, int parentKey, int childKey, JImage whichImage)
    {
        return War3.CallNative<bool>(_saveImageHandlePtr, table.Handle, parentKey, childKey, whichImage.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存地面纹理变化 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存地面纹理变化 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取地面纹理变化' 可以取出保存的值"
    public static bool SaveUbersplatHandle(JHashtable table, int parentKey, int childKey, JUbersplat whichUbersplat)
    {
        return War3.CallNative<bool>(_saveUbersplatHandlePtr, table.Handle, parentKey, childKey, whichUbersplat.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存区域(不规则) [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存区域(不规则) ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取区域(不规则)' 可以取出保存的值"
    public static bool SaveRegionHandle(JHashtable table, int parentKey, int childKey, JRegion whichRegion)
    {
        return War3.CallNative<bool>(_saveRegionHandlePtr, table.Handle, parentKey, childKey, whichRegion.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存迷雾状态 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存迷雾状态 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取迷雾状态' 可以取出保存的值"
    public static bool SaveFogStateHandle(JHashtable table, int parentKey, int childKey, JFogState whichFogState)
    {
        return War3.CallNative<bool>(_saveFogStateHandlePtr, table.Handle, parentKey, childKey, whichFogState.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存可见度修正器 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存可见度修正器 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取可见度修正器' 可以取出保存的值"
    public static bool SaveFogModifierHandle(JHashtable table, int parentKey, int childKey,
        JFogModifier whichFogModifier)
    {
        return War3.CallNative<bool>(_saveFogModifierHandlePtr, table.Handle, parentKey, childKey,
            whichFogModifier.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存实体对象 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存对象 ${Value}"
    /// comment = "实体对象即一切需要计算变量连接数的对象"
    public static bool SaveAgentHandle(JHashtable table, int parentKey, int childKey, JAgent whichAgent)
    {
        return War3.CallNative<bool>(_saveAgentHandlePtr, table.Handle, parentKey, childKey, whichAgent.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 保存哈希表 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 中保存哈希表 ${Value}"
    /// comment = "使用 '哈希表 - 从哈希表提取哈希表' 可以取出保存的值"
    public static bool SaveHashtableHandle(JHashtable table, int parentKey, int childKey, JHashtable whichHashtable)
    {
        return War3.CallNative<bool>(_saveHashtableHandlePtr, table.Handle, parentKey, childKey, whichHashtable.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取整数 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取整数"
    /// comment = "如果不存在则返回0"
    public static int LoadInteger(JHashtable table, int parentKey, int childKey)
    {
        return War3.CallNative<int>(_loadIntegerPtr, table.Handle, parentKey, childKey);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取实数 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取实数"
    /// comment = "如果不存在则返回0.00"
    public static float LoadReal(JHashtable table, int parentKey, int childKey)
    {
        return War3.CallNative<float>(_loadRealPtr, table.Handle, parentKey, childKey);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取布尔 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取布尔"
    /// comment = "如果不存在则返回False"
    public static bool LoadBoolean(JHashtable table, int parentKey, int childKey)
    {
        return War3.CallNative<bool>(_loadBooleanPtr, table.Handle, parentKey, childKey);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取字符串 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取字符串"
    /// comment = "如果不存在则返回空"
    public static string LoadStr(JHashtable table, int parentKey, int childKey)
    {
        return War3.CallNative<string>(_loadStrPtr, table.Handle, parentKey, childKey);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取玩家 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取玩家"
    /// comment = "如果不存在则返回空"
    public static JPlayer LoadPlayerHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadPlayerHandlePtr, table.Handle, parentKey, childKey);
        return new JPlayer(handle);
    }

    public static JWidget LoadWidgetHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadWidgetHandlePtr, table.Handle, parentKey, childKey);
        return new JWidget(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取可破坏物 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取可破坏物"
    /// comment = "如果不存在则返回空"
    public static JDestructable LoadDestructableHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadDestructableHandlePtr, table.Handle, parentKey, childKey);
        return new JDestructable(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取物品 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取物品"
    /// comment = "如果不存在则返回空"
    public static JItem LoadItemHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadItemHandlePtr, table.Handle, parentKey, childKey);
        return new JItem(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取单位 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取单位"
    /// comment = "如果不存在则返回空"
    public static JUnit LoadUnitHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadUnitHandlePtr, table.Handle, parentKey, childKey);
        return new JUnit(handle);
    }

    public static JAbility LoadAbilityHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadAbilityHandlePtr, table.Handle, parentKey, childKey);
        return new JAbility(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取计时器 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取计时器"
    /// comment = "如果不存在则返回空"
    public static JTimer LoadTimerHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadTimerHandlePtr, table.Handle, parentKey, childKey);
        return new JTimer(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取触发器 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取触发器"
    /// comment = "如果不存在则返回空"
    public static JTrigger LoadTriggerHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadTriggerHandlePtr, table.Handle, parentKey, childKey);
        return new JTrigger(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取触发条件 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取触发条件"
    /// comment = "如果不存在则返回空"
    public static JTriggerCondition LoadTriggerConditionHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadTriggerConditionHandlePtr, table.Handle, parentKey, childKey);
        return new JTriggerCondition(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取触发动作 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取触发动作"
    /// comment = "如果不存在则返回空"
    public static JTriggerAction LoadTriggerActionHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadTriggerActionHandlePtr, table.Handle, parentKey, childKey);
        return new JTriggerAction(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取触发事件 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取触发事件"
    /// comment = "如果不存在则返回空"
    public static JEvent LoadTriggerEventHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadTriggerEventHandlePtr, table.Handle, parentKey, childKey);
        return new JEvent(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取玩家组 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取玩家组"
    /// comment = "如果不存在则返回空"
    public static JForce LoadForceHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadForceHandlePtr, table.Handle, parentKey, childKey);
        return new JForce(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取单位组 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取单位组"
    /// comment = "如果不存在则返回空"
    public static JGroup LoadGroupHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadGroupHandlePtr, table.Handle, parentKey, childKey);
        return new JGroup(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取点 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取点"
    /// comment = "如果不存在则返回空"
    public static JLocation LoadLocationHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadLocationHandlePtr, table.Handle, parentKey, childKey);
        return new JLocation(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取区域(矩型) [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取区域(矩型)"
    /// comment = "如果不存在则返回空"
    public static JRect LoadRectHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadRectHandlePtr, table.Handle, parentKey, childKey);
        return new JRect(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取布尔表达式 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取布尔表达式"
    /// comment = "如果不存在则返回空"
    public static JBoolExpr LoadBooleanExprHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadBooleanExprHandlePtr, table.Handle, parentKey, childKey);
        return new JBoolExpr(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取音效 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取音效"
    /// comment = "如果不存在则返回空"
    public static JSound LoadSoundHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadSoundHandlePtr, table.Handle, parentKey, childKey);
        return new JSound(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取特效 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取特效"
    /// comment = "如果不存在则返回空"
    public static JEffect LoadEffectHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadEffectHandlePtr, table.Handle, parentKey, childKey);
        return new JEffect(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取单位池 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取单位池"
    /// comment = "如果不存在则返回空"
    public static JUnitPool LoadUnitPoolHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadUnitPoolHandlePtr, table.Handle, parentKey, childKey);
        return new JUnitPool(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取物品池 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取物品池"
    /// comment = "如果不存在则返回空"
    public static JItemPool LoadItemPoolHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadItemPoolHandlePtr, table.Handle, parentKey, childKey);
        return new JItemPool(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取任务 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取任务"
    /// comment = "如果不存在则返回空"
    public static JQuest LoadQuestHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadQuestHandlePtr, table.Handle, parentKey, childKey);
        return new JQuest(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取任务要求 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取任务要求"
    /// comment = "如果不存在则返回空"
    public static JQuestItem LoadQuestItemHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadQuestItemHandlePtr, table.Handle, parentKey, childKey);
        return new JQuestItem(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取失败条件 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取失败条件"
    /// comment = "如果不存在则返回空"
    public static JDefeatCondition LoadDefeatConditionHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadDefeatConditionHandlePtr, table.Handle, parentKey, childKey);
        return new JDefeatCondition(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取计时器窗口 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取计时器窗口"
    /// comment = "如果不存在则返回空"
    public static JTimerDialog LoadTimerDialogHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadTimerDialogHandlePtr, table.Handle, parentKey, childKey);
        return new JTimerDialog(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取排行榜 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取排行榜"
    /// comment = "如果不存在则返回空"
    public static JLeaderboard LoadLeaderboardHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadLeaderboardHandlePtr, table.Handle, parentKey, childKey);
        return new JLeaderboard(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取多面板 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取多面板"
    /// comment = "如果不存在则返回空"
    public static JMultiboard LoadMultiboardHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadMultiboardHandlePtr, table.Handle, parentKey, childKey);
        return new JMultiboard(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取多面板项目 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取多面板项目"
    /// comment = "如果不存在则返回空"
    public static JMultiboardItem LoadMultiboardItemHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadMultiboardItemHandlePtr, table.Handle, parentKey, childKey);
        return new JMultiboardItem(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取可追踪物 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取可追踪物"
    /// comment = "如果不存在则返回空"
    public static JTrackable LoadTrackableHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadTrackableHandlePtr, table.Handle, parentKey, childKey);
        return new JTrackable(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取对话框 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取对话框"
    /// comment = "如果不存在则返回空"
    public static JDialog LoadDialogHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadDialogHandlePtr, table.Handle, parentKey, childKey);
        return new JDialog(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取对话框按钮 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取对话框按钮"
    /// comment = "如果不存在则返回空"
    public static JButton LoadButtonHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadButtonHandlePtr, table.Handle, parentKey, childKey);
        return new JButton(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取漂浮文字 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取漂浮文字"
    /// comment = "如果不存在则返回空"
    public static JTextTag LoadTextTagHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadTextTagHandlePtr, table.Handle, parentKey, childKey);
        return new JTextTag(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取闪电效果 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取闪电效果"
    /// comment = "如果不存在则返回空"
    public static JLightning LoadLightningHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadLightningHandlePtr, table.Handle, parentKey, childKey);
        return new JLightning(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取图象 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取图象"
    /// comment = "如果不存在则返回空"
    public static JImage LoadImageHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadImageHandlePtr, table.Handle, parentKey, childKey);
        return new JImage(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取地面纹理变化 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取地面纹理变化"
    /// comment = "如果不存在则返回空"
    public static JUbersplat LoadUbersplatHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadUbersplatHandlePtr, table.Handle, parentKey, childKey);
        return new JUbersplat(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取区域(不规则) [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取区域(不规则)"
    /// comment = "如果不存在则返回空"
    public static JRegion LoadRegionHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadRegionHandlePtr, table.Handle, parentKey, childKey);
        return new JRegion(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取迷雾状态 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取迷雾状态"
    /// comment = "如果不存在则返回空"
    public static JFogState LoadFogStateHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadFogStateHandlePtr, table.Handle, parentKey, childKey);
        return new JFogState(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取可见度修正器 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取可见度修正器"
    /// comment = "如果不存在则返回空"
    public static JFogModifier LoadFogModifierHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadFogModifierHandlePtr, table.Handle, parentKey, childKey);
        return new JFogModifier(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 从哈希表提取哈希表 [C]"
    /// description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value} 内提取哈希表"
    /// comment = "如果不存在则返回空"
    public static JHashtable LoadHashtableHandle(JHashtable table, int parentKey, int childKey)
    {
        var handle = War3.CallNative<int>(_loadHashtableHandlePtr, table.Handle, parentKey, childKey);
        return new JHashtable(handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 哈希项存有整数值
    /// <new>
    ///     "
    ///     description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有整数值"
    ///     comment = ""
    public static bool HaveSavedInteger(JHashtable table, int parentKey, int childKey)
    {
        return War3.CallNative<bool>(_haveSavedIntegerPtr, table.Handle, parentKey, childKey);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 哈希项存有实数值
    /// <new>
    ///     "
    ///     description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有实数值"
    ///     comment = ""
    public static bool HaveSavedReal(JHashtable table, int parentKey, int childKey)
    {
        return War3.CallNative<bool>(_haveSavedRealPtr, table.Handle, parentKey, childKey);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 哈希项存有布尔值
    /// <new>
    ///     "
    ///     description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有布尔值"
    ///     comment = ""
    public static bool HaveSavedBoolean(JHashtable table, int parentKey, int childKey)
    {
        return War3.CallNative<bool>(_haveSavedBooleanPtr, table.Handle, parentKey, childKey);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 哈希项存有字符串
    /// <new>
    ///     "
    ///     description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有字符串"
    ///     comment = ""
    public static bool HaveSavedString(JHashtable table, int parentKey, int childKey)
    {
        return War3.CallNative<bool>(_haveSavedStringPtr, table.Handle, parentKey, childKey);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 哈希项存有句柄
    /// <new>
    ///     "
    ///     description = "在 ${Hashtable} 的主索引 ${Value} 子索引 ${Value}  存储有句柄"
    ///     comment = ""
    public static bool HaveSavedHandle(JHashtable table, int parentKey, int childKey)
    {
        return War3.CallNative<bool>(_haveSavedHandlePtr, table.Handle, parentKey, childKey);
    }


    /// title = "清理哈希项存储的整数值
    /// <new>
    ///     "
    ///     description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的整数值"
    ///     comment = "清空哈希表主索引下的整数值"
    public static void RemoveSavedInteger(JHashtable table, int parentKey, int childKey)
    {
        War3.CallNative(_removeSavedIntegerPtr, table.Handle, parentKey, childKey);
    }


    /// title = "清理哈希项存储的实数值
    /// <new>
    ///     "
    ///     description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的实数值"
    ///     comment = "清空哈希表主索引下的实数值"
    public static void RemoveSavedReal(JHashtable table, int parentKey, int childKey)
    {
        War3.CallNative(_removeSavedRealPtr, table.Handle, parentKey, childKey);
    }


    /// title = "清理哈希项存储的布尔值
    /// <new>
    ///     "
    ///     description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的布尔值"
    ///     comment = "清空哈希表主索引下的布尔值"
    public static void RemoveSavedBoolean(JHashtable table, int parentKey, int childKey)
    {
        War3.CallNative(_removeSavedBooleanPtr, table.Handle, parentKey, childKey);
    }


    /// title = "清理哈希项存储的字符串
    /// <new>
    ///     "
    ///     description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的字符串"
    ///     comment = "清空哈希表主索引下的字符串"
    public static void RemoveSavedString(JHashtable table, int parentKey, int childKey)
    {
        War3.CallNative(_removeSavedStringPtr, table.Handle, parentKey, childKey);
    }


    /// title = "清理哈希项存储的句柄
    /// <new>
    ///     "
    ///     description = "清空 ${Hashtable}  主索引  ${Value}  子索引${childKey}  之内的句柄"
    ///     comment = "清空哈希表主索引下的句柄"
    public static void RemoveSavedHandle(JHashtable table, int parentKey, int childKey)
    {
        War3.CallNative(_removeSavedHandlePtr, table.Handle, parentKey, childKey);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 清空哈希表 [C]"
    /// description = "清空 ${Hashtable}"
    /// comment = "清空哈希表所有数据"
    public static void FlushParentHashtable(JHashtable table)
    {
        War3.CallNative(_flushParentHashtablePtr, table.Handle);
    }


    /// title = "
    /// <
    /// 1
    /// .
    /// 2
    /// 4
    /// >
    /// 清空哈希表主索引 [C]"
    /// description = "清空 ${Hashtable} 中位于主索引 ${Value}  之内的所有数据"
    /// comment = "清空哈希表主索引下的所有数据"
    public static void FlushChildHashtable(JHashtable table, int parentKey)
    {
        War3.CallNative(_flushChildHashtablePtr, table.Handle, parentKey);
    }


    /// title = "随机整数"
    /// description = "随机整数,最小值: ${Minimum} 最大值: ${Maximum}"
    /// comment = ""
    public static int GetRandomInt(int lowBound, int highBound)
    {
        return War3.CallNative<int>(_getRandomIntPtr, lowBound, highBound);
    }


    /// title = "随机实数"
    /// description = "随机实数,最小值: ${Minimum} 最大值: ${Maximum}"
    /// comment = ""
    public static float GetRandomReal(float lowBound, float highBound)
    {
        return War3.CallNative<float>(_getRandomRealPtr, lowBound, highBound);
    }


    /// title = "新建单位池 [R]"
    /// description = "新建的空单位池"
    /// comment = "会创建单位池。"
    public static JUnitPool CreateUnitPool()
    {
        var handle = War3.CallNative<int>(_createUnitPoolPtr);
        return new JUnitPool(handle);
    }


    /// title = "删除单位池 [R]"
    /// description = "删除 ${单位池}"
    /// comment = ""
    public static void DestroyUnitPool(JUnitPool whichPool)
    {
        War3.CallNative(_destroyUnitPoolPtr, whichPool.Handle);
    }


    /// title = "添加单位类型 [R]"
    /// description = "在 ${单位池} 中添加一个 ${单位} 比重为 ${数值}"
    /// comment = "比重越高被选择的机率越大"
    public static void UnitPoolAddUnitType(JUnitPool whichPool, int unitId, float weight)
    {
        War3.CallNative(_unitPoolAddUnitTypePtr, whichPool.Handle, unitId, weight);
    }


    /// title = "删除单位类型 [R]"
    /// description = "从 ${单位池} 中删除 ${单位}"
    /// comment = ""
    public static void UnitPoolRemoveUnitType(JUnitPool whichPool, int unitId)
    {
        War3.CallNative(_unitPoolRemoveUnitTypePtr, whichPool.Handle, unitId);
    }


    /// title = "选择放置单位 [R]"
    /// description = "从 ${单位池} 中为 ${玩家} 任意选择一个单位并放置到点( ${X} , ${Y} ) 面向 ${度}"
    /// comment = "从单位池中随机选取一个单位类型."
    public static JUnit PlaceRandomUnit(JUnitPool whichPool, JPlayer forWhichPlayer, float x, float y, float facing)
    {
        var handle = War3.CallNative<int>(_placeRandomUnitPtr, whichPool.Handle, forWhichPlayer.Handle, x, y, facing);
        return new JUnit(handle);
    }


    /// title = "新建物品池 [R]"
    /// description = "新建的空物品池"
    /// comment = "会创建物品池."
    public static JItemPool CreateItemPool()
    {
        var handle = War3.CallNative<int>(_createItemPoolPtr);
        return new JItemPool(handle);
    }


    /// title = "删除物品池 [R]"
    /// description = "删除 ${物品池}"
    /// comment = ""
    public static void DestroyItemPool(JItemPool whichItemPool)
    {
        War3.CallNative(_destroyItemPoolPtr, whichItemPool.Handle);
    }


    /// title = "添加物品类型 [R]"
    /// description = "在 ${物品池} 中添加一个 ${物品} 比重为 ${数值}"
    /// comment = "比重越高被选择的机率越大."
    public static void ItemPoolAddItemType(JItemPool whichItemPool, int itemId, float weight)
    {
        War3.CallNative(_itemPoolAddItemTypePtr, whichItemPool.Handle, itemId, weight);
    }


    /// title = "删除物品类型 [R]"
    /// description = "从 ${物品池} 中删除 ${物品}"
    /// comment = ""
    public static void ItemPoolRemoveItemType(JItemPool whichItemPool, int itemId)
    {
        War3.CallNative(_itemPoolRemoveItemTypePtr, whichItemPool.Handle, itemId);
    }


    /// title = "选择放置物品 [R]"
    /// description = "从 ${物品池} 中任意选择一个物品并放置到( ${X} , ${Y} )点"
    /// comment = ""
    public static JItem PlaceRandomItem(JItemPool whichItemPool, float x, float y)
    {
        var handle = War3.CallNative<int>(_placeRandomItemPtr, whichItemPool.Handle, x, y);
        return new JItem(handle);
    }

    public static int ChooseRandomCreep(int level)
    {
        return War3.CallNative<int>(_chooseRandomCreepPtr, level);
    }

    public static int ChooseRandomNPBuilding()
    {
        return War3.CallNative<int>(_chooseRandomNPBuildingPtr);
    }

    public static int ChooseRandomItem(int level)
    {
        return War3.CallNative<int>(_chooseRandomItemPtr, level);
    }

    public static int ChooseRandomItemEx(JItemType whichType, int level)
    {
        return War3.CallNative<int>(_chooseRandomItemExPtr, whichType.Handle, level);
    }


    /// title = "设置随机种子"
    /// description = "设置随机种子数为：${整数}"
    /// comment = "设置游戏的随机种子，随机种子会影响随机整数，攻击骰子之类的随机数。"
    public static void SetRandomSeed(int seed)
    {
        War3.CallNative(_setRandomSeedPtr, seed);
    }

    public static void SetTerrainFog(float a, float b, float c, float d, float e)
    {
        War3.CallNative(_setTerrainFogPtr, a, b, c, d, e);
    }

    public static void ResetTerrainFog()
    {
        War3.CallNative(_resetTerrainFogPtr);
    }

    public static void SetUnitFog(float a, float b, float c, float d, float e)
    {
        War3.CallNative(_setUnitFogPtr, a, b, c, d, e);
    }


    /// title = "设置迷雾 [R]"
    /// description = "迷雾风格: ${Style}, Z轴开始端: ${Z-Start}, Z轴结束端: ${Z-End}, 密度: ${Density} 颜色:(${Red},${Green},${Blue})"
    /// comment = "颜色格式为(红,绿,蓝). 取值范围0.00-1.00."
    public static void SetTerrainFogEx(int style, float zstart, float zend, float density, float red, float green,
        float blue)
    {
        War3.CallNative(_setTerrainFogExPtr, style, zstart, zend, density, red, green, blue);
    }


    /// title = "对玩家显示文本消息(自动限时) [R]"
    /// description = "对 ${玩家} 在屏幕位移(${X},${Y})处显示文本: ${文字}"
    /// comment = "显示时间取决于文字长度. 位移的取值在0-1之间. 可使用'本地玩家'实现对所有玩家发送消息."
    public static void DisplayTextToPlayer(JPlayer toPlayer, float x, float y, string message)
    {
        War3.CallNative(_displayTextToPlayerPtr, toPlayer.Handle, x, y, message);
    }


    /// title = "对玩家显示文本消息(指定时间) [R]"
    /// description = "对 ${玩家} 在屏幕位移(${X},${Y})处显示 ${时间} 秒的文本信息: ${文字}"
    /// comment = "位移的取值在0-1之间. 可使用'本地玩家[R]'实现对所有玩家发送消息."
    public static void DisplayTimedTextToPlayer(JPlayer toPlayer, float x, float y, float duration, string message)
    {
        War3.CallNative(_displayTimedTextToPlayerPtr, toPlayer.Handle, x, y, duration, message);
    }

    public static void DisplayTimedTextFromPlayer(JPlayer toPlayer, float x, float y, float duration, string message)
    {
        War3.CallNative(_displayTimedTextFromPlayerPtr, toPlayer.Handle, x, y, duration, message);
    }


    /// title = "清空文本信息(所有玩家) [R]"
    /// description = "清空玩家屏幕上的文本信息"
    /// comment = ""
    public static void ClearTextMessages()
    {
        War3.CallNative(_clearTextMessagesPtr);
    }

    public static void SetDayNightModels(string terrainDNCFile, string unitDNCFile)
    {
        War3.CallNative(_setDayNightModelsPtr, terrainDNCFile, unitDNCFile);
    }


    /// title = "设置天空"
    /// description = "设置天空模型为 ${Sky}"
    /// comment = ""
    public static void SetSkyModel(string skyModelFile)
    {
        War3.CallNative(_setSkyModelPtr, skyModelFile);
    }


    /// title = "启用/禁用玩家控制权(所有玩家) [R]"
    /// description = "${启用/禁用} 玩家控制权"
    /// comment = ""
    public static void EnableUserControl(bool b)
    {
        War3.CallNative(_enableUserControlPtr, b);
    }

    public static void EnableUserUI(bool b)
    {
        War3.CallNative(_enableUserUIPtr, b);
    }

    public static void SuspendTimeOfDay(bool b)
    {
        War3.CallNative(_suspendTimeOfDayPtr, b);
    }


    /// title = "设置昼夜时间流逝速度 [R]"
    /// description = "设置昼夜时间流逝速度为默认值的 ${Value}倍"
    /// comment = "设置100%来恢复正常值. 该值并不影响游戏速度."
    public static void SetTimeOfDayScale(float r)
    {
        War3.CallNative(_setTimeOfDayScalePtr, r);
    }

    public static float GetTimeOfDayScale()
    {
        return War3.CallNative<float>(_getTimeOfDayScalePtr);
    }


    /// title = "开启/关闭 信箱模式(所有玩家) [R]"
    /// description = "${开启/关闭} 信箱模式,转换时间为 ${Duration} 秒"
    /// comment = "使用电影镜头模式,隐藏游戏界面."
    public static void ShowInterface(bool flag, float fadeDuration)
    {
        War3.CallNative(_showInterfacePtr, flag, fadeDuration);
    }


    /// title = "暂停/恢复游戏 [R]"
    /// description = "${暂停/恢复} 游戏"
    /// comment = ""
    public static void PauseGame(bool flag)
    {
        War3.CallNative(_pauseGamePtr, flag);
    }


    /// title = "闪动指示器(对单位) [R]"
    /// description = "对 ${单位} 闪动指示器,使用颜色:(${Red}%, ${Green}%, ${Blue}%) Alpha通道值: ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和Alpha通道值取值范围为0-255."
    public static void UnitAddIndicator(JUnit whichUnit, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_unitAddIndicatorPtr, whichUnit.Handle, red, green, blue, alpha);
    }

    public static void AddIndicator(JWidget whichWidget, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_addIndicatorPtr, whichWidget.Handle, red, green, blue, alpha);
    }


    /// title = "小地图信号(所有玩家) [R]"
    /// description = "对所有玩家发送小地图信号到坐标(${X},${Y}) 持续时间: ${Duration} 秒"
    /// comment = ""
    public static void PingMinimap(float x, float y, float duration)
    {
        War3.CallNative(_pingMinimapPtr, x, y, duration);
    }


    /// title = "小地图信号(指定颜色)(所有玩家) [R]"
    /// description = "对所有玩家发送小地图信号到坐标(${X},${Y}) 持续时间: ${Duration} 秒, 信号颜色:(${Red},${Green},${Blue}) 信号类型: ${Style}"
    /// comment = "颜色格式为(红,绿,蓝). 颜色值取值范围为0-255."
    public static void PingMinimapEx(float x, float y, float duration, int red, int green, int blue, bool extraEffects)
    {
        War3.CallNative(_pingMinimapExPtr, x, y, duration, red, green, blue, extraEffects);
    }


    /// title = "允许/禁止闭塞(所有玩家) [R]"
    /// description = "${Enable/Disable} 闭塞"
    /// comment = ""
    public static void EnableOcclusion(bool flag)
    {
        War3.CallNative(_enableOcclusionPtr, flag);
    }

    public static void SetIntroShotText(string introText)
    {
        War3.CallNative(_setIntroShotTextPtr, introText);
    }

    public static void SetIntroShotModel(string introModelPath)
    {
        War3.CallNative(_setIntroShotModelPtr, introModelPath);
    }


    /// title = "允许/禁止 边界染色(所有玩家) [R]"
    /// description = "${Enable/Disable} 边界染色,应用于所有玩家"
    /// comment = "禁用边界染色时边界为普通地形,不显示为黑色,但仍是不可通行的."
    public static void EnableWorldFogBoundary(bool b)
    {
        War3.CallNative(_enableWorldFogBoundaryPtr, b);
    }

    public static void PlayModelCinematic(string modelName)
    {
        War3.CallNative(_playModelCinematicPtr, modelName);
    }

    public static void PlayCinematic(string movieName)
    {
        War3.CallNative(_playCinematicPtr, movieName);
    }

    public static void ForceUIKey(string key)
    {
        War3.CallNative(_forceUIKeyPtr, key);
    }

    public static void ForceUICancel()
    {
        War3.CallNative(_forceUICancelPtr);
    }

    public static void DisplayLoadDialog()
    {
        War3.CallNative(_displayLoadDialogPtr);
    }


    /// title = "设置小地图特殊标志"
    /// description = "设置小地图特殊标志为 ${Image}"
    /// comment = "必须使用16x16的图像."
    public static void SetAltMinimapIcon(string iconPath)
    {
        War3.CallNative(_setAltMinimapIconPtr, iconPath);
    }


    /// title = "禁用 重新开始任务按钮"
    /// description = "设置 重新开始任务按钮可以点击为 ${}"
    /// comment = "当单人游戏时，可以设置重新开始任务按钮能否允许点击。"
    public static void DisableRestartMission(bool flag)
    {
        War3.CallNative(_disableRestartMissionPtr, flag);
    }


    /// title = "新建漂浮文字 [R]"
    /// description = "新建的空漂浮文字"
    /// comment = "会创建漂浮文字."
    public static JTextTag CreateTextTag()
    {
        var handle = War3.CallNative<int>(_createTextTagPtr);
        return new JTextTag(handle);
    }

    public static void DestroyTextTag(JTextTag t)
    {
        War3.CallNative(_destroyTextTagPtr, t.Handle);
    }


    /// title = "改变文字内容 [R]"
    /// description = "改变 ${Floating Text} 的内容为 ${文字} ,字体大小: ${Size}"
    /// comment = "采用原始字体大小单位. 字体大小不能超过0.5."
    public static void SetTextTagText(JTextTag t, string s, float height)
    {
        War3.CallNative(_setTextTagTextPtr, t.Handle, s, height);
    }


    /// title = "改变位置(坐标) [R]"
    /// description = "改变 ${Floating Text} 的位置为(${X},${Y}) ,Z轴高度为 ${Z}"
    /// comment = ""
    public static void SetTextTagPos(JTextTag t, float x, float y, float heightOffset)
    {
        War3.CallNative(_setTextTagPosPtr, t.Handle, x, y, heightOffset);
    }

    public static void SetTextTagPosUnit(JTextTag t, JUnit whichUnit, float heightOffset)
    {
        War3.CallNative(_setTextTagPosUnitPtr, t.Handle, whichUnit.Handle, heightOffset);
    }


    /// title = "改变颜色 [R]"
    /// description = "改变 ${Floating Text} 的颜色为(${Red},${Green},${Blue}) 透明值为 ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). 透明值0为不可见. 颜色值和透明值取值范围为0-255."
    public static void SetTextTagColor(JTextTag t, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_setTextTagColorPtr, t.Handle, red, green, blue, alpha);
    }


    /// title = "设置速率 [R]"
    /// description = "设置 ${Floating Text} 的X轴速率: ${XSpeed} ,Y轴速率: ${YSpeed}"
    /// comment = "对移动后的漂浮文字设置速率,该漂浮文字会先回到原点再向设定的角度移动. 这里的1约等于游戏中的1800速度."
    public static void SetTextTagVelocity(JTextTag t, float xvel, float yvel)
    {
        War3.CallNative(_setTextTagVelocityPtr, t.Handle, xvel, yvel);
    }


    /// title = "显示/隐藏 (所有玩家) [R]"
    /// description = "对所有玩家 ${Show/Hide} ${Floating Text}"
    /// comment = ""
    public static void SetTextTagVisibility(JTextTag t, bool flag)
    {
        War3.CallNative(_setTextTagVisibilityPtr, t.Handle, flag);
    }

    public static void SetTextTagSuspended(JTextTag t, bool flag)
    {
        War3.CallNative(_setTextTagSuspendedPtr, t.Handle, flag);
    }

    public static void SetTextTagPermanent(JTextTag t, bool flag)
    {
        War3.CallNative(_setTextTagPermanentPtr, t.Handle, flag);
    }

    public static void SetTextTagAge(JTextTag t, float age)
    {
        War3.CallNative(_setTextTagAgePtr, t.Handle, age);
    }

    public static void SetTextTagLifespan(JTextTag t, float lifespan)
    {
        War3.CallNative(_setTextTagLifespanPtr, t.Handle, lifespan);
    }

    public static void SetTextTagFadepoint(JTextTag t, float fadepoint)
    {
        War3.CallNative(_setTextTagFadepointPtr, t.Handle, fadepoint);
    }


    /// title = "保留英雄图标"
    /// description = "为玩家保留 ${Number} 个左上角英雄图标."
    /// comment = "因为共享单位而被控制的其他玩家英雄的图标将在保留位置之后开始显示."
    public static void SetReservedLocalHeroButtons(int reserved)
    {
        War3.CallNative(_setReservedLocalHeroButtonsPtr, reserved);
    }


    /// title = "联盟颜色显示设置"
    /// description = "联盟颜色显示设置"
    /// comment = "0为不开启. 1为小地图显示. 2为小地图和游戏都显示."
    public static int GetAllyColorFilterState()
    {
        return War3.CallNative<int>(_getAllyColorFilterStatePtr);
    }


    /// title = "设置联盟颜色显示"
    /// description = "设置联盟颜色显示状态为 ${State}"
    /// comment = "0为不开启. 1为小地图显示. 2为小地图和游戏都显示. 相当于游戏中Alt+A功能."
    public static void SetAllyColorFilterState(int state)
    {
        War3.CallNative(_setAllyColorFilterStatePtr, state);
    }


    /// title = "小地图中立生物显示开启"
    /// description = "小地图中立生物显示开启"
    /// comment = ""
    public static bool GetCreepCampFilterState()
    {
        return War3.CallNative<bool>(_getCreepCampFilterStatePtr);
    }


    /// title = "设置小地图中立生物显示"
    /// description = "小地图 ${Show/Hide} 中立生物"
    /// comment = "相当于游戏中Alt+R功能."
    public static void SetCreepCampFilterState(bool state)
    {
        War3.CallNative(_setCreepCampFilterStatePtr, state);
    }


    /// title = "允许/禁用小地图按钮"
    /// description = "${Enable/Disable} 联盟颜色显示按钮, ${Enable/Disable} 中立生物显示按钮"
    /// comment = ""
    public static void EnableMinimapFilterButtons(bool enableAlly, bool enableCreep)
    {
        War3.CallNative(_enableMinimapFilterButtonsPtr, enableAlly, enableCreep);
    }


    /// title = "允许/禁用框选"
    /// description = "${Enable/Disable} 框选功能 (${Enable/Disable} 显示选择框)"
    /// comment = ""
    public static void EnableDragSelect(bool state, bool ui)
    {
        War3.CallNative(_enableDragSelectPtr, state, ui);
    }


    /// title = "允许/禁用预选"
    /// description = "${Enable/Disable} 预选功能 (${Enable/Disable} 显示预选圈,生命槽,物体信息)"
    /// comment = ""
    public static void EnablePreSelect(bool state, bool ui)
    {
        War3.CallNative(_enablePreSelectPtr, state, ui);
    }


    /// title = "允许/禁用选择"
    /// description = "${Enable/Disable} 选择和取消选择功能 (${Enable/Disable} 显示选择圈)"
    /// comment = "禁用选择后仍可以通过触发来选择物体. 只有允许选择功能时才会显示选择圈."
    public static void EnableSelect(bool state, bool ui)
    {
        War3.CallNative(_enableSelectPtr, state, ui);
    }


    /// title = "新建可追踪物 [R]"
    /// description = "新建的可追踪物, 使用模型: ${模型名字} 所在位置: ( ${X轴} , ${Y轴} ) 面向角度: ${数值} 度"
    /// comment = "可用来响应鼠标的移动和点击. 会创建可追踪物."
    public static JTrackable CreateTrackable(string trackableModelPath, float x, float y, float facing)
    {
        var handle = War3.CallNative<int>(_createTrackablePtr, trackableModelPath, x, y, facing);
        return new JTrackable(handle);
    }


    /// title = "新建任务 [R]"
    /// description = "新建任务 "
    /// comment = "新建一个任务.注：这条毫无用处，别用——everguo"
    public static JQuest CreateQuest()
    {
        var handle = War3.CallNative<int>(_createQuestPtr);
        return new JQuest(handle);
    }

    public static void DestroyQuest(JQuest whichQuest)
    {
        War3.CallNative(_destroyQuestPtr, whichQuest.Handle);
    }

    public static void QuestSetTitle(JQuest whichQuest, string title)
    {
        War3.CallNative(_questSetTitlePtr, whichQuest.Handle, title);
    }

    public static void QuestSetDescription(JQuest whichQuest, string description)
    {
        War3.CallNative(_questSetDescriptionPtr, whichQuest.Handle, description);
    }

    public static void QuestSetIconPath(JQuest whichQuest, string iconPath)
    {
        War3.CallNative(_questSetIconPathPtr, whichQuest.Handle, iconPath);
    }

    public static void QuestSetRequired(JQuest whichQuest, bool required)
    {
        War3.CallNative(_questSetRequiredPtr, whichQuest.Handle, required);
    }

    public static void QuestSetCompleted(JQuest whichQuest, bool completed)
    {
        War3.CallNative(_questSetCompletedPtr, whichQuest.Handle, completed);
    }

    public static void QuestSetDiscovered(JQuest whichQuest, bool discovered)
    {
        War3.CallNative(_questSetDiscoveredPtr, whichQuest.Handle, discovered);
    }

    public static void QuestSetFailed(JQuest whichQuest, bool failed)
    {
        War3.CallNative(_questSetFailedPtr, whichQuest.Handle, failed);
    }


    /// title = "启用/禁用 任务 [R]"
    /// description = "设置 ${Quest} ${Enable/Disable}"
    /// comment = "被禁用的任务将不会显示在任务列表."
    public static void QuestSetEnabled(JQuest whichQuest, bool enabled)
    {
        War3.CallNative(_questSetEnabledPtr, whichQuest.Handle, enabled);
    }


    /// title = "是主要任务"
    /// description = "${Quest} 是主要任务"
    /// comment = ""
    public static bool IsQuestRequired(JQuest whichQuest)
    {
        return War3.CallNative<bool>(_isQuestRequiredPtr, whichQuest.Handle);
    }


    /// title = "任务完成"
    /// description = "${Quest} 已完成"
    /// comment = ""
    public static bool IsQuestCompleted(JQuest whichQuest)
    {
        return War3.CallNative<bool>(_isQuestCompletedPtr, whichQuest.Handle);
    }


    /// title = "任务被发现"
    /// description = "${Quest} 已被发现"
    /// comment = ""
    public static bool IsQuestDiscovered(JQuest whichQuest)
    {
        return War3.CallNative<bool>(_isQuestDiscoveredPtr, whichQuest.Handle);
    }


    /// title = "任务失败"
    /// description = "${Quest} 失败"
    /// comment = ""
    public static bool IsQuestFailed(JQuest whichQuest)
    {
        return War3.CallNative<bool>(_isQuestFailedPtr, whichQuest.Handle);
    }


    /// title = "任务激活"
    /// description = "${Quest} 已激活"
    /// comment = ""
    public static bool IsQuestEnabled(JQuest whichQuest)
    {
        return War3.CallNative<bool>(_isQuestEnabledPtr, whichQuest.Handle);
    }

    public static JQuestItem QuestCreateItem(JQuest whichQuest)
    {
        var handle = War3.CallNative<int>(_questCreateItemPtr, whichQuest.Handle);
        return new JQuestItem(handle);
    }

    public static void QuestItemSetDescription(JQuestItem whichQuestItem, string description)
    {
        War3.CallNative(_questItemSetDescriptionPtr, whichQuestItem.Handle, description);
    }

    public static void QuestItemSetCompleted(JQuestItem whichQuestItem, bool completed)
    {
        War3.CallNative(_questItemSetCompletedPtr, whichQuestItem.Handle, completed);
    }


    /// title = "任务项目完成"
    /// description = "${Quest Requirement} 已完成"
    /// comment = ""
    public static bool IsQuestItemCompleted(JQuestItem whichQuestItem)
    {
        return War3.CallNative<bool>(_isQuestItemCompletedPtr, whichQuestItem.Handle);
    }

    public static JDefeatCondition CreateDefeatCondition()
    {
        var handle = War3.CallNative<int>(_createDefeatConditionPtr);
        return new JDefeatCondition(handle);
    }

    public static void DestroyDefeatCondition(JDefeatCondition whichCondition)
    {
        War3.CallNative(_destroyDefeatConditionPtr, whichCondition.Handle);
    }

    public static void DefeatConditionSetDescription(JDefeatCondition whichCondition, string description)
    {
        War3.CallNative(_defeatConditionSetDescriptionPtr, whichCondition.Handle, description);
    }

    public static void FlashQuestDialogButton()
    {
        War3.CallNative(_flashQuestDialogButtonPtr);
    }

    public static void ForceQuestDialogUpdate()
    {
        War3.CallNative(_forceQuestDialogUpdatePtr);
    }


    /// title = "新建计时器窗口 [R]"
    /// description = "为 ${计时器} 新建计时窗口"
    /// comment = "为一个计时器创建一个新建计时器窗口."
    public static JTimerDialog CreateTimerDialog(JTimer t)
    {
        var handle = War3.CallNative<int>(_createTimerDialogPtr, t.Handle);
        return new JTimerDialog(handle);
    }

    public static void DestroyTimerDialog(JTimerDialog whichDialog)
    {
        War3.CallNative(_destroyTimerDialogPtr, whichDialog.Handle);
    }

    public static void TimerDialogSetTitle(JTimerDialog whichDialog, string title)
    {
        War3.CallNative(_timerDialogSetTitlePtr, whichDialog.Handle, title);
    }


    /// title = "改变计时器窗口文字颜色 [R]"
    /// description = "改变 ${Timer Window} 文字颜色为(${Red},${Green},${Blue}) 透明值为: ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和透明值值取值范围为0-255."
    public static void TimerDialogSetTitleColor(JTimerDialog whichDialog, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_timerDialogSetTitleColorPtr, whichDialog.Handle, red, green, blue, alpha);
    }


    /// title = "改变计时器窗口计时颜色 [R]"
    /// description = "改变 ${Timer Window} 的计间颜色为(${Red},${Green},${Blue}) 透明值为: ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和透明值值取值范围为0-255."
    public static void TimerDialogSetTimeColor(JTimerDialog whichDialog, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_timerDialogSetTimeColorPtr, whichDialog.Handle, red, green, blue, alpha);
    }


    /// title = "设置计时器窗口速率 [R]"
    /// description = "设置 ${Timer Window} 的时间流逝速度为 ${Factor} 倍"
    /// comment = " 同时计时器显示时间也会随之变化. 就是说60秒的计时器设置为2倍速则显示时间也会变为120秒."
    public static void TimerDialogSetSpeed(JTimerDialog whichDialog, float speedMultFactor)
    {
        War3.CallNative(_timerDialogSetSpeedPtr, whichDialog.Handle, speedMultFactor);
    }


    /// title = "显示/隐藏 计时器窗口(所有玩家) [R]"
    /// description = "设置 ${计时器窗口} 的状态为${Show/Hide}"
    /// comment = "计时器窗口不能在地图初始化时显示."
    public static void TimerDialogDisplay(JTimerDialog whichDialog, bool display)
    {
        War3.CallNative(_timerDialogDisplayPtr, whichDialog.Handle, display);
    }

    public static bool IsTimerDialogDisplayed(JTimerDialog whichDialog)
    {
        return War3.CallNative<bool>(_isTimerDialogDisplayedPtr, whichDialog.Handle);
    }

    public static void TimerDialogSetRealTimeRemaining(JTimerDialog whichDialog, float timeRemaining)
    {
        War3.CallNative(_timerDialogSetRealTimeRemainingPtr, whichDialog.Handle, timeRemaining);
    }


    /// title = "新建排行榜 [R]"
    /// description = "新建的空排行榜"
    /// comment = "会创建排行榜."
    public static JLeaderboard CreateLeaderboard()
    {
        var handle = War3.CallNative<int>(_createLeaderboardPtr);
        return new JLeaderboard(handle);
    }

    public static void DestroyLeaderboard(JLeaderboard lb)
    {
        War3.CallNative(_destroyLeaderboardPtr, lb.Handle);
    }


    /// title = "显示/隐藏 [R]"
    /// description = "设置 ${排行榜} ${Show/Hide}"
    /// comment = "排行榜不能在地图初始化时显示."
    public static void LeaderboardDisplay(JLeaderboard lb, bool show)
    {
        War3.CallNative(_leaderboardDisplayPtr, lb.Handle, show);
    }

    public static bool IsLeaderboardDisplayed(JLeaderboard lb)
    {
        return War3.CallNative<bool>(_isLeaderboardDisplayedPtr, lb.Handle);
    }


    /// title = "行数"
    /// description = "${Leaderboard} 的行数"
    /// comment = ""
    public static int LeaderboardGetItemCount(JLeaderboard lb)
    {
        return War3.CallNative<int>(_leaderboardGetItemCountPtr, lb.Handle);
    }

    public static void LeaderboardSetSizeByItemCount(JLeaderboard lb, int count)
    {
        War3.CallNative(_leaderboardSetSizeByItemCountPtr, lb.Handle, count);
    }

    public static void LeaderboardAddItem(JLeaderboard lb, string label, int value, JPlayer p)
    {
        War3.CallNative(_leaderboardAddItemPtr, lb.Handle, label, value, p.Handle);
    }

    public static void LeaderboardRemoveItem(JLeaderboard lb, int index)
    {
        War3.CallNative(_leaderboardRemoveItemPtr, lb.Handle, index);
    }

    public static void LeaderboardRemovePlayerItem(JLeaderboard lb, JPlayer p)
    {
        War3.CallNative(_leaderboardRemovePlayerItemPtr, lb.Handle, p.Handle);
    }


    /// title = "清空 [R]"
    /// description = "清空 ${排行榜}"
    /// comment = "清空排行榜中所有内容."
    public static void LeaderboardClear(JLeaderboard lb)
    {
        War3.CallNative(_leaderboardClearPtr, lb.Handle);
    }

    public static void LeaderboardSortItemsByValue(JLeaderboard lb, bool ascending)
    {
        War3.CallNative(_leaderboardSortItemsByValuePtr, lb.Handle, ascending);
    }

    public static void LeaderboardSortItemsByPlayer(JLeaderboard lb, bool ascending)
    {
        War3.CallNative(_leaderboardSortItemsByPlayerPtr, lb.Handle, ascending);
    }

    public static void LeaderboardSortItemsByLabel(JLeaderboard lb, bool ascending)
    {
        War3.CallNative(_leaderboardSortItemsByLabelPtr, lb.Handle, ascending);
    }

    public static bool LeaderboardHasPlayerItem(JLeaderboard lb, JPlayer p)
    {
        return War3.CallNative<bool>(_leaderboardHasPlayerItemPtr, lb.Handle, p.Handle);
    }

    public static int LeaderboardGetPlayerIndex(JLeaderboard lb, JPlayer p)
    {
        return War3.CallNative<int>(_leaderboardGetPlayerIndexPtr, lb.Handle, p.Handle);
    }

    public static void LeaderboardSetLabel(JLeaderboard lb, string label)
    {
        War3.CallNative(_leaderboardSetLabelPtr, lb.Handle, label);
    }

    public static string LeaderboardGetLabelText(JLeaderboard lb)
    {
        return War3.CallNative<string>(_leaderboardGetLabelTextPtr, lb.Handle);
    }


    /// title = "设置玩家使用的排行榜 [R]"
    /// description = "设置 ${Player} 使用 ${排行榜}"
    /// comment = "每个玩家只能显示一个排行榜."
    public static void PlayerSetLeaderboard(JPlayer toPlayer, JLeaderboard lb)
    {
        War3.CallNative(_playerSetLeaderboardPtr, toPlayer.Handle, lb.Handle);
    }

    public static JLeaderboard PlayerGetLeaderboard(JPlayer toPlayer)
    {
        var handle = War3.CallNative<int>(_playerGetLeaderboardPtr, toPlayer.Handle);
        return new JLeaderboard(handle);
    }


    /// title = "设置文字颜色 [R]"
    /// description = "设置 ${Leaderboard} 的文字颜色为(${Red},${Green},${Blue}) Alpha通道值为: ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和Alpha通道值取值范围为0-255."
    public static void LeaderboardSetLabelColor(JLeaderboard lb, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_leaderboardSetLabelColorPtr, lb.Handle, red, green, blue, alpha);
    }


    /// title = "设置数值颜色 [R]"
    /// description = "设置 ${Leaderboard} 的数值颜色为(${Red},${Green},${Blue}) Alpha通道值为: ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). Alpha通道值0为不可见. 颜色值和Alpha通道值取值范围为0-255."
    public static void LeaderboardSetValueColor(JLeaderboard lb, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_leaderboardSetValueColorPtr, lb.Handle, red, green, blue, alpha);
    }

    public static void LeaderboardSetStyle(JLeaderboard lb, bool showLabel, bool showNames, bool showValues,
        bool showIcons)
    {
        War3.CallNative(_leaderboardSetStylePtr, lb.Handle, showLabel, showNames, showValues, showIcons);
    }

    public static void LeaderboardSetItemValue(JLeaderboard lb, int whichItem, int val)
    {
        War3.CallNative(_leaderboardSetItemValuePtr, lb.Handle, whichItem, val);
    }

    public static void LeaderboardSetItemLabel(JLeaderboard lb, int whichItem, string val)
    {
        War3.CallNative(_leaderboardSetItemLabelPtr, lb.Handle, whichItem, val);
    }

    public static void LeaderboardSetItemStyle(JLeaderboard lb, int whichItem, bool showLabel, bool showValue,
        bool showIcon)
    {
        War3.CallNative(_leaderboardSetItemStylePtr, lb.Handle, whichItem, showLabel, showValue, showIcon);
    }

    public static void LeaderboardSetItemLabelColor(JLeaderboard lb, int whichItem, int red, int green, int blue,
        int alpha)
    {
        War3.CallNative(_leaderboardSetItemLabelColorPtr, lb.Handle, whichItem, red, green, blue, alpha);
    }

    public static void LeaderboardSetItemValueColor(JLeaderboard lb, int whichItem, int red, int green, int blue,
        int alpha)
    {
        War3.CallNative(_leaderboardSetItemValueColorPtr, lb.Handle, whichItem, red, green, blue, alpha);
    }


    /// title = "新建多面板 [R]"
    /// description = "新建的空多面板"
    /// comment = "会创建多面板."
    public static JMultiboard CreateMultiboard()
    {
        var handle = War3.CallNative<int>(_createMultiboardPtr);
        return new JMultiboard(handle);
    }

    public static void DestroyMultiboard(JMultiboard lb)
    {
        War3.CallNative(_destroyMultiboardPtr, lb.Handle);
    }


    /// title = "显示/隐藏 [R]"
    /// description = "设置 ${Multiboard} ${Show/Hide}"
    /// comment = "多面板不能在地图初始化时显示."
    public static void MultiboardDisplay(JMultiboard lb, bool show)
    {
        War3.CallNative(_multiboardDisplayPtr, lb.Handle, show);
    }


    /// title = "多面板显示"
    /// description = "${Multiboard} 是显示的"
    /// comment = ""
    public static bool IsMultiboardDisplayed(JMultiboard lb)
    {
        return War3.CallNative<bool>(_isMultiboardDisplayedPtr, lb.Handle);
    }


    /// title = "最大/最小化 [R]"
    /// description = "设置 ${Multiboard} ${Minimize/Maximize}"
    /// comment = "最小化的多面板只显示标题."
    public static void MultiboardMinimize(JMultiboard lb, bool minimize)
    {
        War3.CallNative(_multiboardMinimizePtr, lb.Handle, minimize);
    }


    /// title = "多面板最小化"
    /// description = "${Multiboard} 是最小化的"
    /// comment = ""
    public static bool IsMultiboardMinimized(JMultiboard lb)
    {
        return War3.CallNative<bool>(_isMultiboardMinimizedPtr, lb.Handle);
    }


    /// title = "清空多面板"
    /// description = "清空 ${Multiboard}"
    /// comment = "清空该多面板中的所有行和列."
    public static void MultiboardClear(JMultiboard lb)
    {
        War3.CallNative(_multiboardClearPtr, lb.Handle);
    }


    /// title = "设置标题"
    /// description = "设置 ${Multiboard} 的标题为 ${文字}"
    /// comment = ""
    public static void MultiboardSetTitleText(JMultiboard lb, string label)
    {
        War3.CallNative(_multiboardSetTitleTextPtr, lb.Handle, label);
    }


    /// title = "多面板标题"
    /// description = "${Multiboard} 的标题"
    /// comment = ""
    public static string MultiboardGetTitleText(JMultiboard lb)
    {
        return War3.CallNative<string>(_multiboardGetTitleTextPtr, lb.Handle);
    }


    /// title = "设置标题颜色 [R]"
    /// description = "设置 ${Multiboard} 的标题颜色为(${Red},${Green},${Blue}), Alpha值为 ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255."
    public static void MultiboardSetTitleTextColor(JMultiboard lb, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_multiboardSetTitleTextColorPtr, lb.Handle, red, green, blue, alpha);
    }


    /// title = "行数"
    /// description = "${Multiboard} 的行数"
    /// comment = ""
    public static int MultiboardGetRowCount(JMultiboard lb)
    {
        return War3.CallNative<int>(_multiboardGetRowCountPtr, lb.Handle);
    }


    /// title = "列数"
    /// description = "${Multiboard} 的列数"
    /// comment = ""
    public static int MultiboardGetColumnCount(JMultiboard lb)
    {
        return War3.CallNative<int>(_multiboardGetColumnCountPtr, lb.Handle);
    }


    /// title = "设置列数"
    /// description = "设置 ${Multiboard} 的列数为 ${Columns}"
    /// comment = ""
    public static void MultiboardSetColumnCount(JMultiboard lb, int count)
    {
        War3.CallNative(_multiboardSetColumnCountPtr, lb.Handle, count);
    }


    /// title = "设置行数"
    /// description = "设置 ${Multiboard} 的行数为 ${Rows}"
    /// comment = ""
    public static void MultiboardSetRowCount(JMultiboard lb, int count)
    {
        War3.CallNative(_multiboardSetRowCountPtr, lb.Handle, count);
    }


    /// title = "设置所有项目显示风格 [R]"
    /// description = "设置 ${多面板} 的所有项目显示风格: ${Show/Hide} 文字 ${Show/Hide} 图标"
    public static void MultiboardSetItemsStyle(JMultiboard lb, bool showValues, bool showIcons)
    {
        War3.CallNative(_multiboardSetItemsStylePtr, lb.Handle, showValues, showIcons);
    }


    /// title = "设置所有项目文本 [R]"
    /// description = "设置 ${多面板} 的所有项目文本为 ${文字}"
    public static void MultiboardSetItemsValue(JMultiboard lb, string value)
    {
        War3.CallNative(_multiboardSetItemsValuePtr, lb.Handle, value);
    }


    /// title = "设置所有项目颜色 [R]"
    /// description = "设置 ${多面板} 的所有项目颜色为(${Red},${Green},${Blue}), Alpha值为 ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255."
    public static void MultiboardSetItemsValueColor(JMultiboard lb, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_multiboardSetItemsValueColorPtr, lb.Handle, red, green, blue, alpha);
    }


    /// title = "设置所有项目宽度 [R]"
    /// description = "设置 ${多面板} 的所有项目宽度为 ${Width} 倍屏幕宽度"
    public static void MultiboardSetItemsWidth(JMultiboard lb, float width)
    {
        War3.CallNative(_multiboardSetItemsWidthPtr, lb.Handle, width);
    }


    /// title = "设置所有项目图标 [R]"
    /// description = "设置 ${多面板} 的所有项目图标为 ${Icon File}"
    public static void MultiboardSetItemsIcon(JMultiboard lb, string iconPath)
    {
        War3.CallNative(_multiboardSetItemsIconPtr, lb.Handle, iconPath);
    }


    /// title = "多面板项目 [R]"
    /// description = "${多面板} 的第 ${X} 行,第 ${Y} 列项"
    /// comment = "(0,0)作为多面板首项,会创建多面板项目."
    public static JMultiboardItem MultiboardGetItem(JMultiboard lb, int row, int column)
    {
        var handle = War3.CallNative<int>(_multiboardGetItemPtr, lb.Handle, row, column);
        return new JMultiboardItem(handle);
    }


    /// title = "删除多面板项目 [R]"
    /// description = "删除 ${多面板项目}"
    /// comment = "并不会影响对多面板的显示. 多面板项目指向多面板但不附属于多面板."
    public static void MultiboardReleaseItem(JMultiboardItem mbi)
    {
        War3.CallNative(_multiboardReleaseItemPtr, mbi.Handle);
    }


    /// title = "设置指定项目显示风格 [R]"
    /// description = "设置 ${多面板项目} 的显示风格: ${Show/Hide} 文字 ${Show/Hide} 图标"
    /// comment = ""
    public static void MultiboardSetItemStyle(JMultiboardItem mbi, bool showValue, bool showIcon)
    {
        War3.CallNative(_multiboardSetItemStylePtr, mbi.Handle, showValue, showIcon);
    }


    /// title = "设置指定项目文本 [R]"
    /// description = "设置 ${多面板项目} 的项目文本为 ${文字}"
    /// comment = ""
    public static void MultiboardSetItemValue(JMultiboardItem mbi, string val)
    {
        War3.CallNative(_multiboardSetItemValuePtr, mbi.Handle, val);
    }


    /// title = "设置指定项目颜色 [R]"
    /// description = "设置 ${多面板项目} 的项目颜色为(${Red},${Green},${Blue}), Alpha值为 ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围为0-255."
    public static void MultiboardSetItemValueColor(JMultiboardItem mbi, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_multiboardSetItemValueColorPtr, mbi.Handle, red, green, blue, alpha);
    }


    /// title = "设置指定项目宽度 [R]"
    /// description = "设置 ${多面板项目} 的项目宽度为 ${Width} 倍屏幕宽度"
    public static void MultiboardSetItemWidth(JMultiboardItem mbi, float width)
    {
        War3.CallNative(_multiboardSetItemWidthPtr, mbi.Handle, width);
    }


    /// title = "设置指定项目图标 [R]"
    /// description = "设置 ${多面板项目} 的项目图标为 ${Icon File}"
    public static void MultiboardSetItemIcon(JMultiboardItem mbi, string iconFileName)
    {
        War3.CallNative(_multiboardSetItemIconPtr, mbi.Handle, iconFileName);
    }


    /// title = "显示/隐藏多面板模式 [R]"
    /// description = "${打开/关闭} 隐藏多面板模式"
    /// comment = "隐藏多面板模式将无法显示多面板."
    public static void MultiboardSuppressDisplay(bool flag)
    {
        War3.CallNative(_multiboardSuppressDisplayPtr, flag);
    }

    public static void SetCameraPosition(float x, float y)
    {
        War3.CallNative(_setCameraPositionPtr, x, y);
    }


    /// title = "设置空格键转向点(所有玩家) [R]"
    /// description = "设置玩家的空格键转向点为(${X},${Y})"
    /// comment = "按下空格键时镜头转向的位置."
    public static void SetCameraQuickPosition(float x, float y)
    {
        War3.CallNative(_setCameraQuickPositionPtr, x, y);
    }


    /// title = "设置可用镜头区域(所有玩家) [R]"
    /// description = "设置玩家可用镜头区域: 左下角(${X},${Y}), 左上角(${X},${Y}), 右上角(${X},${Y}), 右下角(${X},${Y})"
    /// comment = "该动作同样会影响小地图的显示. 但小地图的图片是无法改变的. 实际可用区域要大于可用镜头区域."
    public static void SetCameraBounds(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
        War3.CallNative(_setCameraBoundsPtr, x1, y1, x2, y2, x3, y3, x4, y4);
    }


    /// title = "停止播放镜头(所有玩家) [R]"
    /// description = "让所有玩家停止播放镜头"
    /// comment = "比如在平移镜头的过程中可用该动作来中断平移."
    public static void StopCamera()
    {
        War3.CallNative(_stopCameraPtr);
    }


    /// title = "重置游戏镜头(所有玩家) [R]"
    /// description = "重置玩家镜头为游戏默认状态,持续  ${Time} 秒"
    /// comment = ""
    public static void ResetToGameCamera(float duration)
    {
        War3.CallNative(_resetToGameCameraPtr, duration);
    }

    public static void PanCameraTo(float x, float y)
    {
        War3.CallNative(_panCameraToPtr, x, y);
    }


    /// title = "平移镜头(所有玩家)(限时) [R]"
    /// description = "平移玩家镜头到(${X},${Y}),持续 ${Time} 秒"
    /// comment = ""
    public static void PanCameraToTimed(float x, float y, float duration)
    {
        War3.CallNative(_panCameraToTimedPtr, x, y, duration);
    }

    public static void PanCameraToWithZ(float x, float y, float zOffsetDest)
    {
        War3.CallNative(_panCameraToWithZPtr, x, y, zOffsetDest);
    }


    /// title = "指定高度平移镜头(所有玩家)(限时) [R]"
    /// description = "平移玩家镜头到(${X},${Y}),镜头距离地面高度为 ${Z},持续 ${Time} 秒"
    /// comment = "在指定移动路径上镜头不会低于地面高度."
    public static void PanCameraToTimedWithZ(float x, float y, float zOffsetDest, float duration)
    {
        War3.CallNative(_panCameraToTimedWithZPtr, x, y, zOffsetDest, duration);
    }


    /// title = "播放电影镜头(所有玩家) [R]"
    /// description = "对所有玩家播放电影镜头: ${Camera File}"
    /// comment = "在'Objects\\CinematicCameras'目录下有一些电影镜头,可用Mpq工具来查询."
    public static void SetCinematicCamera(string cameraModelFile)
    {
        War3.CallNative(_setCinematicCameraPtr, cameraModelFile);
    }


    /// title = "指定点旋转镜头(所有玩家)(弧度)(限时) [R]"
    /// description = "以(${X},${Y})为中心,旋转弧度为${Rad}, 持续: ${Time} 秒"
    /// comment = ""
    public static void SetCameraRotateMode(float x, float y, float radiansToSweep, float duration)
    {
        War3.CallNative(_setCameraRotateModePtr, x, y, radiansToSweep, duration);
    }


    /// title = "设置镜头属性(所有玩家)(限时) [R]"
    /// description = "设置玩家的镜头属性 ${Field} 为 ${数值},持续 ${Time} 秒"
    /// comment = ""
    public static void SetCameraField(JCameraField whichField, float value, float duration)
    {
        War3.CallNative(_setCameraFieldPtr, whichField.Handle, value, duration);
    }

    public static void AdjustCameraField(JCameraField whichField, float offset, float duration)
    {
        War3.CallNative(_adjustCameraFieldPtr, whichField.Handle, offset, duration);
    }


    /// title = "锁定镜头到单位(所有玩家) [R]"
    /// description = "锁定玩家镜头到 ${单位}, 偏移坐标(${X}, ${Y}) ,使用 ${Rotation Source}"
    /// comment = "偏移坐标(X,Y)以单位脚底为原点坐标."
    public static void SetCameraTargetController(JUnit whichUnit, float xoffset, float yoffset, bool inheritOrientation)
    {
        War3.CallNative(_setCameraTargetControllerPtr, whichUnit.Handle, xoffset, yoffset, inheritOrientation);
    }


    /// title = "锁定镜头到单位(固定镜头源)(所有玩家) [R]"
    /// description = "锁定玩家镜头到 ${单位}, 偏移坐标(${X}, ${Y})"
    /// comment = "偏移坐标(X,Y)以单位脚底为原点坐标."
    public static void SetCameraOrientController(JUnit whichUnit, float xoffset, float yoffset)
    {
        War3.CallNative(_setCameraOrientControllerPtr, whichUnit.Handle, xoffset, yoffset);
    }

    public static JCameraSetup CreateCameraSetup()
    {
        var handle = War3.CallNative<int>(_createCameraSetupPtr);
        return new JCameraSetup(handle);
    }

    public static void CameraSetupSetField(JCameraSetup whichSetup, JCameraField whichField, float value,
        float duration)
    {
        War3.CallNative(_cameraSetupSetFieldPtr, whichSetup.Handle, whichField.Handle, value, duration);
    }


    /// title = "镜头属性(指定镜头) [R]"
    /// description = "${镜头} 的 ${Camera Field}"
    /// comment = ""
    public static float CameraSetupGetField(JCameraSetup whichSetup, JCameraField whichField)
    {
        return War3.CallNative<float>(_cameraSetupGetFieldPtr, whichSetup.Handle, whichField.Handle);
    }

    public static void CameraSetupSetDestPosition(JCameraSetup whichSetup, float x, float y, float duration)
    {
        War3.CallNative(_cameraSetupSetDestPositionPtr, whichSetup.Handle, x, y, duration);
    }


    /// title = "镜头目标点"
    /// description = "${镜头} 的目标点"
    /// comment = "会创建点."
    public static JLocation CameraSetupGetDestPositionLoc(JCameraSetup whichSetup)
    {
        var handle = War3.CallNative<int>(_cameraSetupGetDestPositionLocPtr, whichSetup.Handle);
        return new JLocation(handle);
    }

    public static float CameraSetupGetDestPositionX(JCameraSetup whichSetup)
    {
        return War3.CallNative<float>(_cameraSetupGetDestPositionXPtr, whichSetup.Handle);
    }

    public static float CameraSetupGetDestPositionY(JCameraSetup whichSetup)
    {
        return War3.CallNative<float>(_cameraSetupGetDestPositionYPtr, whichSetup.Handle);
    }

    public static void CameraSetupApply(JCameraSetup whichSetup, bool doPan, bool panTimed)
    {
        War3.CallNative(_cameraSetupApplyPtr, whichSetup.Handle, doPan, panTimed);
    }

    public static void CameraSetupApplyWithZ(JCameraSetup whichSetup, float zDestOffset)
    {
        War3.CallNative(_cameraSetupApplyWithZPtr, whichSetup.Handle, zDestOffset);
    }


    /// title = "应用镜头(所有玩家)(限时) [R]"
    /// description = "将 ${镜头} 应用方式设置为 ${Apply Method},持续 ${Time} 秒"
    /// comment = ""
    public static void CameraSetupApplyForceDuration(JCameraSetup whichSetup, bool doPan, float forceDuration)
    {
        War3.CallNative(_cameraSetupApplyForceDurationPtr, whichSetup.Handle, doPan, forceDuration);
    }

    public static void CameraSetupApplyForceDurationWithZ(JCameraSetup whichSetup, float zDestOffset,
        float forceDuration)
    {
        War3.CallNative(_cameraSetupApplyForceDurationWithZPtr, whichSetup.Handle, zDestOffset, forceDuration);
    }

    public static void CameraSetTargetNoise(float mag, float velocity)
    {
        War3.CallNative(_cameraSetTargetNoisePtr, mag, velocity);
    }

    public static void CameraSetSourceNoise(float mag, float velocity)
    {
        War3.CallNative(_cameraSetSourceNoisePtr, mag, velocity);
    }


    /// title = "摇晃镜头目标(所有玩家) [R]"
    /// description = "摇晃玩家的镜头源, 摇晃幅度: ${Magnitude} 速率: ${Velocity} 摇晃方式: ${方式}"
    /// comment = "使用'镜头 - 重置镜头'或设置摇晃幅度和速率为0来停止摇晃."
    public static void CameraSetTargetNoiseEx(float mag, float velocity, bool vertOnly)
    {
        War3.CallNative(_cameraSetTargetNoiseExPtr, mag, velocity, vertOnly);
    }


    /// title = "摇晃镜头源(所有玩家) [R]"
    /// description = "摇晃玩家的镜头源, 摇晃幅度: ${Magnitude} 速率: ${Velocity} 摇晃方式: ${方式}"
    /// comment = "使用'镜头 - 重置镜头'或设置摇晃幅度和速率为0来停止摇晃."
    public static void CameraSetSourceNoiseEx(float mag, float velocity, bool vertOnly)
    {
        War3.CallNative(_cameraSetSourceNoiseExPtr, mag, velocity, vertOnly);
    }

    public static void CameraSetSmoothingFactor(float factor)
    {
        War3.CallNative(_cameraSetSmoothingFactorPtr, factor);
    }

    public static void SetCineFilterTexture(string filename)
    {
        War3.CallNative(_setCineFilterTexturePtr, filename);
    }

    public static void SetCineFilterBlendMode(JBlendMode whichMode)
    {
        War3.CallNative(_setCineFilterBlendModePtr, whichMode.Handle);
    }

    public static void SetCineFilterTexMapFlags(JTexMapFlags whichFlags)
    {
        War3.CallNative(_setCineFilterTexMapFlagsPtr, whichFlags.Handle);
    }

    public static void SetCineFilterStartUV(float minu, float minv, float maxu, float maxv)
    {
        War3.CallNative(_setCineFilterStartUVPtr, minu, minv, maxu, maxv);
    }

    public static void SetCineFilterEndUV(float minu, float minv, float maxu, float maxv)
    {
        War3.CallNative(_setCineFilterEndUVPtr, minu, minv, maxu, maxv);
    }

    public static void SetCineFilterStartColor(int red, int green, int blue, int alpha)
    {
        War3.CallNative(_setCineFilterStartColorPtr, red, green, blue, alpha);
    }

    public static void SetCineFilterEndColor(int red, int green, int blue, int alpha)
    {
        War3.CallNative(_setCineFilterEndColorPtr, red, green, blue, alpha);
    }

    public static void SetCineFilterDuration(float duration)
    {
        War3.CallNative(_setCineFilterDurationPtr, duration);
    }

    public static void DisplayCineFilter(bool flag)
    {
        War3.CallNative(_displayCineFilterPtr, flag);
    }

    public static bool IsCineFilterDisplayed()
    {
        return War3.CallNative<bool>(_isCineFilterDisplayedPtr);
    }

    public static void SetCinematicScene(int portraitUnitId, JPlayerColor color, string speakerTitle, string text,
        float sceneDuration, float voiceoverDuration)
    {
        War3.CallNative(_setCinematicScenePtr, portraitUnitId, color.Handle, speakerTitle, text, sceneDuration,
            voiceoverDuration);
    }

    public static void EndCinematicScene()
    {
        War3.CallNative(_endCinematicScenePtr);
    }

    public static void ForceCinematicSubtitles(bool flag)
    {
        War3.CallNative(_forceCinematicSubtitlesPtr, flag);
    }

    public static float GetCameraMargin(int whichMargin)
    {
        return War3.CallNative<float>(_getCameraMarginPtr, whichMargin);
    }

    public static float GetCameraBoundMinX()
    {
        return War3.CallNative<float>(_getCameraBoundMinXPtr);
    }

    public static float GetCameraBoundMinY()
    {
        return War3.CallNative<float>(_getCameraBoundMinYPtr);
    }

    public static float GetCameraBoundMaxX()
    {
        return War3.CallNative<float>(_getCameraBoundMaxXPtr);
    }

    public static float GetCameraBoundMaxY()
    {
        return War3.CallNative<float>(_getCameraBoundMaxYPtr);
    }


    /// title = "镜头属性(当前镜头)"
    /// description = "当前镜头的 ${Camera Field}"
    /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."
    public static float GetCameraField(JCameraField whichField)
    {
        return War3.CallNative<float>(_getCameraFieldPtr, whichField.Handle);
    }


    /// title = "当前镜头目标X坐标"
    /// description = "当前镜头目标X坐标"
    /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."
    public static float GetCameraTargetPositionX()
    {
        return War3.CallNative<float>(_getCameraTargetPositionXPtr);
    }


    /// title = "当前镜头目标Y坐标"
    /// description = "当前镜头目标Y坐标"
    /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."
    public static float GetCameraTargetPositionY()
    {
        return War3.CallNative<float>(_getCameraTargetPositionYPtr);
    }


    /// title = "当前镜头目标Z坐标"
    /// description = "当前镜头目标Z坐标"
    /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."
    public static float GetCameraTargetPositionZ()
    {
        return War3.CallNative<float>(_getCameraTargetPositionZPtr);
    }


    /// title = "当前镜头目标点"
    /// description = "当前镜头目标点"
    /// comment = "会创建点. 注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."
    public static JLocation GetCameraTargetPositionLoc()
    {
        var handle = War3.CallNative<int>(_getCameraTargetPositionLocPtr);
        return new JLocation(handle);
    }


    /// title = "当前镜头源X坐标"
    /// description = "当前镜头源X坐标"
    /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."
    public static float GetCameraEyePositionX()
    {
        return War3.CallNative<float>(_getCameraEyePositionXPtr);
    }


    /// title = "当前镜头源Y坐标"
    /// description = "当前镜头源Y坐标"
    /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."
    public static float GetCameraEyePositionY()
    {
        return War3.CallNative<float>(_getCameraEyePositionYPtr);
    }


    /// title = "当前镜头源Z坐标"
    /// description = "当前镜头源Z坐标"
    /// comment = "注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."
    public static float GetCameraEyePositionZ()
    {
        return War3.CallNative<float>(_getCameraEyePositionZPtr);
    }


    /// title = "当前镜头源位置"
    /// description = "当前镜头源位置"
    /// comment = "会创建点. 注意:该函数对各玩家返回值不同,请确定你知道自己在做什么,否则很容易引起掉线."
    public static JLocation GetCameraEyePositionLoc()
    {
        var handle = War3.CallNative<int>(_getCameraEyePositionLocPtr);
        return new JLocation(handle);
    }


    /// title = "设置环境音效 [new]"
    /// description = "设置环境音效为 ${environmentName}"
    /// comment = ""
    public static void NewSoundEnvironment(string environmentName)
    {
        War3.CallNative(_newSoundEnvironmentPtr, environmentName);
    }


    /// title = "创建声音 [new]"
    /// description = "创建声音，路径：${fileName}，是否循环播放 ${looping}，是否为3D声音 ${is3D}，超出范围是否停止 ${stopwhenoutofrange}，淡入速率 ${fadeInRate}，淡出速率 ${fadeOutRate}，效果 ${eaxSetting}"
    /// comment = ""
    public static JSound CreateSound(string fileName, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate,
        int fadeOutRate, string eaxSetting)
    {
        var handle = War3.CallNative<int>(_createSoundPtr, fileName, looping, is3D, stopwhenoutofrange, fadeInRate,
            fadeOutRate, eaxSetting);
        return new JSound(handle);
    }


    /// title = "根据文件名和标签创建声音 [new]"
    /// description = "创建声音， 路径：${fileName} ，是否循环播放 ${looping}，是否为3D声音 ${is3D}，超出范围是否停止 ${stopwhenoutofrange}，淡入速率 ${fadeInRate}，淡出速率 ${fadeOutRate} 设置标签 ${SLKEntryName}"
    /// comment = ""
    public static JSound CreateSoundFilenameWithLabel(string fileName, bool looping, bool is3D, bool stopwhenoutofrange,
        int fadeInRate, int fadeOutRate, string SLKEntryName)
    {
        var handle = War3.CallNative<int>(_createSoundFilenameWithLabelPtr, fileName, looping, is3D, stopwhenoutofrange,
            fadeInRate, fadeOutRate, SLKEntryName);
        return new JSound(handle);
    }


    /// title = "从标签创建声音 [new]"
    /// description = "根据标签 ${soundLabel} 创建声音，是否循环播放 ${looping}，是否为3D声音 ${is3D}，超出范围是否停止 ${stopwhenoutofrange}，淡入速率 ${fadeInRate}，淡出速率 ${fadeOutRate}"
    /// comment = ""
    public static JSound CreateSoundFromLabel(string soundLabel, bool looping, bool is3D, bool stopwhenoutofrange,
        int fadeInRate, int fadeOutRate)
    {
        var handle = War3.CallNative<int>(_createSoundFromLabelPtr, soundLabel, looping, is3D, stopwhenoutofrange,
            fadeInRate, fadeOutRate);
        return new JSound(handle);
    }


    /// title = "创建MIDI音乐 [new]"
    /// description = "创建MIDI音乐，标签为 ${soundLabel}，淡入速率 ${fadeInRate}，淡出速率 ${fadeOutRate}"
    /// comment = ""
    public static JSound CreateMIDISound(string soundLabel, int fadeInRate, int fadeOutRate)
    {
        var handle = War3.CallNative<int>(_createMIDISoundPtr, soundLabel, fadeInRate, fadeOutRate);
        return new JSound(handle);
    }


    /// title = "设置声音标签 [new]"
    /// description = "设置 ${soundLabel} 的标签：${soundHandle}"
    /// comment = ""
    public static void SetSoundParamsFromLabel(JSound soundHandle, string soundLabel)
    {
        War3.CallNative(_setSoundParamsFromLabelPtr, soundHandle.Handle, soundLabel);
    }

    public static void SetSoundDistanceCutoff(JSound soundHandle, float cutoff)
    {
        War3.CallNative(_setSoundDistanceCutoffPtr, soundHandle.Handle, cutoff);
    }


    /// title = "设置声音的声道 [new]"
    /// description = "设置 ${soundHandle} 的声道：${channel}"
    /// comment = ""
    public static void SetSoundChannel(JSound soundHandle, int channel)
    {
        War3.CallNative(_setSoundChannelPtr, soundHandle.Handle, channel);
    }


    /// title = "设置音效音量 [R]"
    /// description = "设置 ${音效} 的音量为 ${Volume}"
    /// comment = "音量取值范围0-127."
    public static void SetSoundVolume(JSound soundHandle, int volume)
    {
        War3.CallNative(_setSoundVolumePtr, soundHandle.Handle, volume);
    }

    public static void SetSoundPitch(JSound soundHandle, float pitch)
    {
        War3.CallNative(_setSoundPitchPtr, soundHandle.Handle, pitch);
    }


    /// title = "设置音效播放时间点 [R]"
    /// description = "设置 ${音效} 的播放时间点为 ${Offset} 毫秒"
    /// comment = "音效必须是正在播放的. 不能用于3D音效."
    public static void SetSoundPlayPosition(JSound soundHandle, int millisecs)
    {
        War3.CallNative(_setSoundPlayPositionPtr, soundHandle.Handle, millisecs);
    }


    /// title = "设置3D音效衰减范围"
    /// description = "设置 ${3D音效} 的衰减最小范围: ${数值} 最大范围: ${数值}"
    /// comment = "该动作仅用于3D音效. 注意不一定要达到最大范围,音量衰减到一定程度也会变没的."
    public static void SetSoundDistances(JSound soundHandle, float minDist, float maxDist)
    {
        War3.CallNative(_setSoundDistancesPtr, soundHandle.Handle, minDist, maxDist);
    }


    /// title = "设置声音的角度 [new]"
    /// description = "设置 ${soundHandle} 的内圆锥角度： ${inside}，外圆锥角度：${outside}，外圆锥音量：${outsideVolume}"
    /// comment = ""
    public static void SetSoundConeAngles(JSound soundHandle, float inside, float outside, int outsideVolume)
    {
        War3.CallNative(_setSoundConeAnglesPtr, soundHandle.Handle, inside, outside, outsideVolume);
    }


    /// title = "设置声音的传播方向 [new]"
    /// description = "设置 ${soundHandle} 的传播方向(${x}, ${y}, ${z})"
    /// comment = ""
    public static void SetSoundConeOrientation(JSound soundHandle, float x, float y, float z)
    {
        War3.CallNative(_setSoundConeOrientationPtr, soundHandle.Handle, x, y, z);
    }


    /// title = "设置3D音效位置(指定坐标) [R]"
    /// description = "设置 ${3D音效} 的播放位置为(${X},${Y}), Z轴高度为 ${Z}"
    /// comment = "该动作仅用于3D音效."
    public static void SetSoundPosition(JSound soundHandle, float x, float y, float z)
    {
        War3.CallNative(_setSoundPositionPtr, soundHandle.Handle, x, y, z);
    }


    /// title = "设置声音的速度 [new]"
    /// description = "设置 ${soundHandle} 的速度设置为 (${x}, ${y}, ${z})"
    /// comment = ""
    public static void SetSoundVelocity(JSound soundHandle, float x, float y, float z)
    {
        War3.CallNative(_setSoundVelocityPtr, soundHandle.Handle, x, y, z);
    }

    public static void AttachSoundToUnit(JSound soundHandle, JUnit whichUnit)
    {
        War3.CallNative(_attachSoundToUnitPtr, soundHandle.Handle, whichUnit.Handle);
    }

    public static void StartSound(JSound soundHandle)
    {
        War3.CallNative(_startSoundPtr, soundHandle.Handle);
    }

    public static void StopSound(JSound soundHandle, bool killWhenDone, bool fadeOut)
    {
        War3.CallNative(_stopSoundPtr, soundHandle.Handle, killWhenDone, fadeOut);
    }

    public static void KillSoundWhenDone(JSound soundHandle)
    {
        War3.CallNative(_killSoundWhenDonePtr, soundHandle.Handle);
    }


    /// title = "设置背景音乐列表 [R]"
    /// description = "设置背景音乐列表为: ${Music} , ${允许/禁止} 随机播放, 开始播放序号为 ${Index}"
    /// comment = "可指定播放文件或播放目录."
    public static void SetMapMusic(string musicName, bool random, int index)
    {
        War3.CallNative(_setMapMusicPtr, musicName, random, index);
    }

    public static void ClearMapMusic()
    {
        War3.CallNative(_clearMapMusicPtr);
    }

    public static void PlayMusic(string musicName)
    {
        War3.CallNative(_playMusicPtr, musicName);
    }

    public static void PlayMusicEx(string musicName, int frommsecs, int fadeinmsecs)
    {
        War3.CallNative(_playMusicExPtr, musicName, frommsecs, fadeinmsecs);
    }

    public static void StopMusic(bool fadeOut)
    {
        War3.CallNative(_stopMusicPtr, fadeOut);
    }

    public static void ResumeMusic()
    {
        War3.CallNative(_resumeMusicPtr);
    }


    /// title = "播放主题音乐 [C]"
    /// description = "播放 ${Music Theme} 主题音乐"
    /// comment = "播放主题音乐一次,然后恢复原来的音乐."
    public static void PlayThematicMusic(string musicFileName)
    {
        War3.CallNative(_playThematicMusicPtr, musicFileName);
    }


    /// title = "跳播主题音乐 [R]"
    /// description = "播放 ${Music Theme} 主题音乐,跳过开始 ${Offset} 毫秒"
    public static void PlayThematicMusicEx(string musicFileName, int frommsecs)
    {
        War3.CallNative(_playThematicMusicExPtr, musicFileName, frommsecs);
    }


    /// title = "停止主题音乐[C]"
    /// description = "停止正在播放的主题音乐"
    /// comment = ""
    public static void EndThematicMusic()
    {
        War3.CallNative(_endThematicMusicPtr);
    }


    /// title = "设置背景音乐音量 [R]"
    /// description = "设置背景音乐音量为 ${Volume}"
    /// comment = "音量取值范围为0-127."
    public static void SetMusicVolume(int volume)
    {
        War3.CallNative(_setMusicVolumePtr, volume);
    }


    /// title = "设置背景音乐播放时间点 [R]"
    /// description = "设置当前背景音乐的播放时间点为 ${Offset} 毫秒"
    public static void SetMusicPlayPosition(int millisecs)
    {
        War3.CallNative(_setMusicPlayPositionPtr, millisecs);
    }


    /// title = "设置主题音乐播放时间点 [R]"
    /// description = "设置当前主题音乐播放时间点为 ${Offset} 毫秒"
    /// comment = ""
    public static void SetThematicMusicPlayPosition(int millisecs)
    {
        War3.CallNative(_setThematicMusicPlayPositionPtr, millisecs);
    }

    public static void SetSoundDuration(JSound soundHandle, int duration)
    {
        War3.CallNative(_setSoundDurationPtr, soundHandle.Handle, duration);
    }

    public static int GetSoundDuration(JSound soundHandle)
    {
        return War3.CallNative<int>(_getSoundDurationPtr, soundHandle.Handle);
    }

    public static int GetSoundFileDuration(string musicFileName)
    {
        return War3.CallNative<int>(_getSoundFileDurationPtr, musicFileName);
    }


    /// title = "设置多通道音量 [R]"
    /// description = "设置 ${Volume Channel} 的音量为 ${Volume}"
    /// comment = "音量取值范围0-1."
    public static void VolumeGroupSetVolume(JVolumeGroup vgroup, float scale)
    {
        War3.CallNative(_volumeGroupSetVolumePtr, vgroup.Handle, scale);
    }

    public static void VolumeGroupReset()
    {
        War3.CallNative(_volumeGroupResetPtr);
    }


    /// title = "声音在播放 [new]"
    /// description = " ${soundHandle} 在播放"
    /// comment = ""
    public static bool GetSoundIsPlaying(JSound soundHandle)
    {
        return War3.CallNative<bool>(_getSoundIsPlayingPtr, soundHandle.Handle);
    }


    /// title = "声音在加载 [new]"
    /// description = " ${soundHandle} 在加载"
    /// comment = ""
    public static bool GetSoundIsLoading(JSound soundHandle)
    {
        return War3.CallNative<bool>(_getSoundIsLoadingPtr, soundHandle.Handle);
    }


    /// title = "注册堆叠音效 [new]"
    /// description = "设置 ${soundHandle} 的堆叠，是否按位置来处理堆叠：${byPosition},堆叠区域的宽度：${rectwidth}, 堆叠区域的高度：${rectheight}"
    /// comment = ""
    public static void RegisterStackedSound(JSound soundHandle, bool byPosition, float rectwidth, float rectheight)
    {
        War3.CallNative(_registerStackedSoundPtr, soundHandle.Handle, byPosition, rectwidth, rectheight);
    }


    /// title = "取消注册的堆叠音效设置 [new]"
    /// description = "取消 ${soundHandle} 的堆叠，是否按位置来处理堆叠：${byPosition},堆叠区域的宽度：${rectwidth}, 堆叠区域的高度：${rectheight}"
    /// comment = ""
    public static void UnregisterStackedSound(JSound soundHandle, bool byPosition, float rectwidth, float rectheight)
    {
        War3.CallNative(_unregisterStackedSoundPtr, soundHandle.Handle, byPosition, rectwidth, rectheight);
    }


    /// title = "新建天气效果 [R]"
    /// description = "新建的在 ${矩形区域} 的天气效果: ${WeatherId}"
    /// comment = "会创建天气效果."
    public static JWeatherEffect AddWeatherEffect(JRect where, int effectID)
    {
        var handle = War3.CallNative<int>(_addWeatherEffectPtr, where.Handle, effectID);
        return new JWeatherEffect(handle);
    }

    public static void RemoveWeatherEffect(JWeatherEffect whichEffect)
    {
        War3.CallNative(_removeWeatherEffectPtr, whichEffect.Handle);
    }


    /// title = "启用/禁用 天气效果"
    /// description = "设置 ${Weather Effect} 的状态为: ${On/Off}"
    /// comment = "可以使用'环境 - 创建天气效果'动作来创建天气效果."
    public static void EnableWeatherEffect(JWeatherEffect whichEffect, bool enable)
    {
        War3.CallNative(_enableWeatherEffectPtr, whichEffect.Handle, enable);
    }


    /// title = "新建地形变化:弹坑 [R]"
    /// description = "新建的弹坑变形. 中心坐标:(${X},${Y}) 半径: ${Radius} 深度: ${Depth} 持续时间: ${Duration} 毫秒, 变化类型: ${Type}"
    /// comment = "深度可取负数. 永久地形变化在保存游戏时不会被记录."
    public static JTerrainDeformation TerrainDeformCrater(float x, float y, float radius, float depth, int duration,
        bool permanent)
    {
        var handle = War3.CallNative<int>(_terrainDeformCraterPtr, x, y, radius, depth, duration, permanent);
        return new JTerrainDeformation(handle);
    }


    /// title = "新建地形变化:波纹 [R]"
    /// description = "新建的波纹变形. 中心坐标:(${X},${Y}) 最终半径: ${Radius} 深度: ${Depth} 持续时间: ${Duration} 毫秒, 变化次数: ${Count} 面波数: ${SpaceWave} 总波数: ${TimeWave} 初始半径率: ${数值} 变化类型: ${Type}"
    /// comment = "初始半径率=初始半径/最终半径."
    public static JTerrainDeformation TerrainDeformRipple(float x, float y, float radius, float depth, int duration,
        int count, float spaceWaves, float timeWaves, float radiusStartPct, bool limitNeg)
    {
        var handle = War3.CallNative<int>(_terrainDeformRipplePtr, x, y, radius, depth, duration, count, spaceWaves,
            timeWaves, radiusStartPct, limitNeg);
        return new JTerrainDeformation(handle);
    }


    /// title = "新建地形变化:冲击波 [R]"
    /// description = "新建的冲击波变形. 起始坐标:(${X},${Y}) 波方向:(${X},${Y}) 波距离: ${distance} 波速度: ${speed} 波半径: ${radius} 深度: ${Depth} 变形效果持续时间: ${Delay} 毫秒, 变化次数: ${Count}"
    /// comment = "深度可取负数. 方向以(X,Y)坐标形式表示,如(1,1)表示45度."
    public static JTerrainDeformation TerrainDeformWave(float x, float y, float dirX, float dirY, float distance,
        float speed, float radius, float depth, int trailTime, int count)
    {
        var handle = War3.CallNative<int>(_terrainDeformWavePtr, x, y, dirX, dirY, distance, speed, radius, depth,
            trailTime, count);
        return new JTerrainDeformation(handle);
    }


    /// title = "新建地形变化:随机 [R]"
    /// description = "新建的随机变形. 中心坐标:(${X},${Y}) 半径: ${Radius} 最小高度变化: ${Depth} 最大高度变化: ${Depth} 持续时间: ${Duration} 毫秒, 变化周期: ${Duration} 毫秒"
    /// comment = ""
    public static JTerrainDeformation TerrainDeformRandom(float x, float y, float radius, float minDelta,
        float maxDelta, int duration, int updateInterval)
    {
        var handle = War3.CallNative<int>(_terrainDeformRandomPtr, x, y, radius, minDelta, maxDelta, duration,
            updateInterval);
        return new JTerrainDeformation(handle);
    }


    /// title = "停止地形变化 [R]"
    /// description = "停止 ${Terrain Deformation} ,衰退时间: ${Duration} 毫秒"
    /// comment = "地形变化会平滑地过渡到无."
    public static void TerrainDeformStop(JTerrainDeformation deformation, int duration)
    {
        War3.CallNative(_terrainDeformStopPtr, deformation.Handle, duration);
    }


    /// title = "停止所有地形变化"
    /// description = "停止所有地形变化"
    /// comment = "包括由技能引起的地形变化."
    public static void TerrainDeformStopAll()
    {
        War3.CallNative(_terrainDeformStopAllPtr);
    }


    /// title = "新建特效(创建到坐标) [R]"
    /// description = "新建特效 ${Model File} 在(${X},${Y})处"
    /// comment = "会创建特效."
    public static JEffect AddSpecialEffect(string modelName, float x, float y)
    {
        var handle = War3.CallNative<int>(_addSpecialEffectPtr, modelName, x, y);
        return new JEffect(handle);
    }


    /// title = "新建特效(创建到点) [R]"
    /// description = "新建特效 ${Model File} 在 ${指定点} 处"
    /// comment = "会创建特效."
    public static JEffect AddSpecialEffectLoc(string modelName, JLocation where)
    {
        var handle = War3.CallNative<int>(_addSpecialEffectLocPtr, modelName, where.Handle);
        return new JEffect(handle);
    }


    /// title = "新建特效(创建到单位) [R]"
    /// description = "新建特效 ${Model File} 并绑定到 ${单位} 的 ${Attachment Point} 附加点上"
    /// comment = "会创建特效."
    public static JEffect AddSpecialEffectTarget(string modelName, JWidget targetWidget, string attachPointName)
    {
        var handle = War3.CallNative<int>(_addSpecialEffectTargetPtr, modelName, targetWidget.Handle, attachPointName);
        return new JEffect(handle);
    }

    public static void DestroyEffect(JEffect whichEffect)
    {
        War3.CallNative(_destroyEffectPtr, whichEffect.Handle);
    }

    public static JEffect AddSpellEffect(string abilityString, JEffectType t, float x, float y)
    {
        var handle = War3.CallNative<int>(_addSpellEffectPtr, abilityString, t.Handle, x, y);
        return new JEffect(handle);
    }

    public static JEffect AddSpellEffectLoc(string abilityString, JEffectType t, JLocation where)
    {
        var handle = War3.CallNative<int>(_addSpellEffectLocPtr, abilityString, t.Handle, where.Handle);
        return new JEffect(handle);
    }


    /// title = "新建特效(指定技能，创建到坐标) [R]"
    /// description = "${技能} 的 ${EffectType} , 创建到坐标(${X},${Y})"
    /// comment = "会创建特效."
    public static JEffect AddSpellEffectById(int abilityId, JEffectType t, float x, float y)
    {
        var handle = War3.CallNative<int>(_addSpellEffectByIdPtr, abilityId, t.Handle, x, y);
        return new JEffect(handle);
    }


    /// title = "新建特效(指定技能，创建到点) [R]"
    /// description = "${技能} 的 ${EffectType} , 创建到 ${指定点}"
    /// comment = "会创建特效."
    public static JEffect AddSpellEffectByIdLoc(int abilityId, JEffectType t, JLocation where)
    {
        var handle = War3.CallNative<int>(_addSpellEffectByIdLocPtr, abilityId, t.Handle, where.Handle);
        return new JEffect(handle);
    }

    public static JEffect AddSpellEffectTarget(string modelName, JEffectType t, JWidget targetWidget,
        string attachPoint)
    {
        var handle = War3.CallNative<int>(_addSpellEffectTargetPtr, modelName, t.Handle, targetWidget.Handle,
            attachPoint);
        return new JEffect(handle);
    }


    /// title = "新建特效(指定技能，创建到单位) [R]"
    /// description = "${技能} 的 ${EffectType} , 绑定到 ${单位} 的 ${String} 附加点"
    /// comment = "会创建特效."
    public static JEffect AddSpellEffectTargetById(int abilityId, JEffectType t, JWidget targetWidget,
        string attachPoint)
    {
        var handle = War3.CallNative<int>(_addSpellEffectTargetByIdPtr, abilityId, t.Handle, targetWidget.Handle,
            attachPoint);
        return new JEffect(handle);
    }


    /// title = "新建闪电效果 [R]"
    /// description = "新建闪电效果: ${闪电效果} (${Boolean}检查可见性) 起始点:(${X},${Y}) 终结点:(${X},${Y})"
    /// comment = "会创建闪电效果. 允许检查可见性则在起始点和终结点都不可见时将不创建闪电效果."
    public static JLightning AddLightning(string codeName, bool checkVisibility, float x1, float y1, float x2, float y2)
    {
        var handle = War3.CallNative<int>(_addLightningPtr, codeName, checkVisibility, x1, y1, x2, y2);
        return new JLightning(handle);
    }


    /// title = "新建闪电效果(指定Z轴) [R]"
    /// description = "新建闪电效果: ${闪电效果} (${Boolean}检查可见性) 起始点:(${X},${Y},${Z}) 终结点:(${X},${Y},${Z})"
    /// comment = "会创建闪电效果. 允许检查可见性则在起始点和终结点都不可见时将不创建闪电效果."
    public static JLightning AddLightningEx(string codeName, bool checkVisibility, float x1, float y1, float z1,
        float x2, float y2, float z2)
    {
        var handle = War3.CallNative<int>(_addLightningExPtr, codeName, checkVisibility, x1, y1, z1, x2, y2, z2);
        return new JLightning(handle);
    }

    public static bool DestroyLightning(JLightning whichBolt)
    {
        return War3.CallNative<bool>(_destroyLightningPtr, whichBolt.Handle);
    }

    public static bool MoveLightning(JLightning whichBolt, bool checkVisibility, float x1, float y1, float x2, float y2)
    {
        return War3.CallNative<bool>(_moveLightningPtr, whichBolt.Handle, checkVisibility, x1, y1, x2, y2);
    }


    /// title = "移动闪电效果(指定坐标) [R]"
    /// description = "移动 ${Lightning} 到新位置,(${Boolean} 检查可见性) 新起始点: (${X},${Y},${Z}) 新终结点: (${X},${Y},${Z})"
    /// comment = "可指定Z坐标. 允许检查可见性则在指定起始点和终结点都不可见时将不移动闪电效果."
    public static bool MoveLightningEx(JLightning whichBolt, bool checkVisibility, float x1, float y1, float z1,
        float x2, float y2, float z2)
    {
        return War3.CallNative<bool>(_moveLightningExPtr, whichBolt.Handle, checkVisibility, x1, y1, z1, x2, y2, z2);
    }

    public static float GetLightningColorA(JLightning whichBolt)
    {
        return War3.CallNative<float>(_getLightningColorAPtr, whichBolt.Handle);
    }

    public static float GetLightningColorR(JLightning whichBolt)
    {
        return War3.CallNative<float>(_getLightningColorRPtr, whichBolt.Handle);
    }

    public static float GetLightningColorG(JLightning whichBolt)
    {
        return War3.CallNative<float>(_getLightningColorGPtr, whichBolt.Handle);
    }

    public static float GetLightningColorB(JLightning whichBolt)
    {
        return War3.CallNative<float>(_getLightningColorBPtr, whichBolt.Handle);
    }

    public static bool SetLightningColor(JLightning whichBolt, float r, float g, float b, float a)
    {
        return War3.CallNative<bool>(_setLightningColorPtr, whichBolt.Handle, r, g, b, a);
    }

    public static string GetAbilityEffect(string abilityString, JEffectType t, int index)
    {
        return War3.CallNative<string>(_getAbilityEffectPtr, abilityString, t.Handle, index);
    }

    public static string GetAbilityEffectById(int abilityId, JEffectType t, int index)
    {
        return War3.CallNative<string>(_getAbilityEffectByIdPtr, abilityId, t.Handle, index);
    }

    public static string GetAbilitySound(string abilityString, JSoundType t)
    {
        return War3.CallNative<string>(_getAbilitySoundPtr, abilityString, t.Handle);
    }

    public static string GetAbilitySoundById(int abilityId, JSoundType t)
    {
        return War3.CallNative<string>(_getAbilitySoundByIdPtr, abilityId, t.Handle);
    }


    /// title = "地形悬崖高度(指定坐标) [R]"
    /// description = "坐标(${X},${Y})处的地形悬崖高度"
    /// comment = "悬崖高度:深水区为0, 浅水区为1, 平原为2, 之后每层+1."
    public static int GetTerrainCliffLevel(float x, float y)
    {
        return War3.CallNative<int>(_getTerrainCliffLevelPtr, x, y);
    }


    /// title = "设置水颜色 [R]"
    /// description = "设置水颜色为:(${Red},${Green},${Blue}), 透明值为: ${Transparency}"
    /// comment = "颜色格式为(红,绿,蓝). 透明值0为不可见. 颜色值和透明道值取值范围为0-255."
    public static void SetWaterBaseColor(int red, int green, int blue, int alpha)
    {
        War3.CallNative(_setWaterBaseColorPtr, red, green, blue, alpha);
    }


    /// title = "开启/关闭 水面变形"
    /// description = "${On/Off} 水面变形"
    /// comment = "开启时当发生地形变化时水面高度也会随着变化. 对永久变形无效."
    public static void SetWaterDeforms(bool val)
    {
        War3.CallNative(_setWaterDeformsPtr, val);
    }


    /// title = "指定坐标地形 [R]"
    /// description = "坐标(${X},${Y})处的地形"
    /// comment = ""
    public static int GetTerrainType(float x, float y)
    {
        return War3.CallNative<int>(_getTerrainTypePtr, x, y);
    }


    /// title = "地形样式(指定坐标) [R]"
    /// description = "坐标(${X},${Y})处的地形样式"
    /// comment = ""
    public static int GetTerrainVariance(float x, float y)
    {
        return War3.CallNative<int>(_getTerrainVariancePtr, x, y);
    }


    /// title = "改变地形类型(指定坐标) [R]"
    /// description = "改变(${X},${Y})处的地形为 ${Terrain Type} ,使用样式: ${Variation} 范围: ${Area} 形状: ${Shape}"
    /// comment = "地形样式-1表示随机样式. 范围即地形编辑器中的刷子大小.1表示128x128范围"
    public static void SetTerrainType(float x, float y, int terrainType, int variation, int area, int shape)
    {
        War3.CallNative(_setTerrainTypePtr, x, y, terrainType, variation, area, shape);
    }


    /// title = "地形通行状态关闭(指定坐标) [R]"
    /// description = "坐标(${X},${Y})处的 ${Pathing Type} 通行状态为关闭"
    /// comment = "指定类型单位不能通行即通行状态为关闭. 如该点不能造建筑就是'建造'通行状态为关闭. 可使用'环境 - 设置地形通行状态'来改变通行状态."
    public static bool IsTerrainPathable(float x, float y, JPathingType t)
    {
        return War3.CallNative<bool>(_isTerrainPathablePtr, x, y, t.Handle);
    }


    /// title = "设置地形通行状态(指定坐标) [R]"
    /// description = "设置(${X},${Y})处单元点的 ${Pathing} 地形通行状态为: ${On/Off}"
    /// comment = "例:设置'建造'通行状态为开,则该点可以建造建筑. 一个单元点范围为32x32."
    public static void SetTerrainPathable(float x, float y, JPathingType t, bool flag)
    {
        War3.CallNative(_setTerrainPathablePtr, x, y, t.Handle, flag);
    }


    /// title = "新建图像 [R]"
    /// description = "新建图像: ${Image} 大小:(${X},${Y},${Z}) 创建点:(${X},${Y},${Z}) 原点坐标:(${X},${Y},${Z}) 图像类型: ${Type}"
    /// comment = "使用'图像 - 设置永久渲染状态'动作才能显示图像. 大小、创建点和原点格式为(X,Y,Z). 创建点作为图像的左下角位置. 会创建图像."
    public static JImage CreateImage(string file, float sizeX, float sizeY, float sizeZ, float posX, float posY,
        float posZ, float originX, float originY, float originZ, int imageType)
    {
        var handle = War3.CallNative<int>(_createImagePtr, file, sizeX, sizeY, sizeZ, posX, posY, posZ, originX,
            originY, originZ, imageType);
        return new JImage(handle);
    }


    /// title = "删除"
    /// description = "删除 ${图像}"
    /// comment = ""
    public static void DestroyImage(JImage whichImage)
    {
        War3.CallNative(_destroyImagePtr, whichImage.Handle);
    }


    /// title = "显示/隐藏 [R]"
    /// description = "设置 ${Image} ${Show/Hide}"
    /// comment = ""
    public static void ShowImage(JImage whichImage, bool flag)
    {
        War3.CallNative(_showImagePtr, whichImage.Handle, flag);
    }


    /// title = "设置图像高度"
    /// description = "设置 ${Image} ${Enable/Disable} Z轴显示,并设置高度为 ${Height}"
    /// comment = "实际显示高度为图像高度+Z轴偏移. 只有允许Z轴显示时才有效."
    public static void SetImageConstantHeight(JImage whichImage, bool flag, float height)
    {
        War3.CallNative(_setImageConstantHeightPtr, whichImage.Handle, flag, height);
    }


    /// title = "改变图像位置(指定坐标) [R]"
    /// description = "改变 ${Image} 的位置为(${X},${Y}),Z轴偏移为 ${Z}"
    /// comment = "指图像的左下角位置."
    public static void SetImagePosition(JImage whichImage, float x, float y, float z)
    {
        War3.CallNative(_setImagePositionPtr, whichImage.Handle, x, y, z);
    }


    /// title = "改变图像颜色 [R]"
    /// description = "设置 ${Image} 的颜色值为(${Red},${Green},${Blue}) Alpha值为 ${Alpha}"
    /// comment = "颜色格式为(红,绿,蓝). Alpha值为0是不可见的. 颜色值和Alpha值取值范围0-255."
    public static void SetImageColor(JImage whichImage, int red, int green, int blue, int alpha)
    {
        War3.CallNative(_setImageColorPtr, whichImage.Handle, red, green, blue, alpha);
    }


    /// title = "设置图像渲染状态"
    /// description = "设置 ${Image} : ${Enable/Disable} 显示状态"
    /// comment = "未发现有任何作用."
    public static void SetImageRender(JImage whichImage, bool flag)
    {
        War3.CallNative(_setImageRenderPtr, whichImage.Handle, flag);
    }


    /// title = "设置图像永久渲染状态"
    /// description = "设置 ${Image} : ${Enable/Disable} 永久显示状态"
    /// comment = "要显示图像则必须开启该项."
    public static void SetImageRenderAlways(JImage whichImage, bool flag)
    {
        War3.CallNative(_setImageRenderAlwaysPtr, whichImage.Handle, flag);
    }


    /// title = "图像水面显示状态"
    /// description = "设置 ${Image} : ${Enable/Disable} 水面显示, ${Enable/Disable} 水的Alpha通道"
    /// comment = "前者设置图像在水面或是水底显示. 后者设置图像是否受水的Alpha通道影响. "
    public static void SetImageAboveWater(JImage whichImage, bool flag, bool useWaterAlpha)
    {
        War3.CallNative(_setImageAboveWaterPtr, whichImage.Handle, flag, useWaterAlpha);
    }


    /// title = "改变图像类型"
    /// description = "改变 ${Image} 类型为 ${Type}"
    /// comment = ""
    public static void SetImageType(JImage whichImage, int imageType)
    {
        War3.CallNative(_setImageTypePtr, whichImage.Handle, imageType);
    }


    /// title = "新建地面纹理变化 [R]"
    /// description = "新建的地面纹理变化在点(${X},${Y}),使用图像: ${Type} 颜色值为(${Red},${Green},${Blue}) Alpha值为${Transparency} (${Enable/Disable} 暂停状态, ${Enble/Disable} 跳过出生动画)"
    /// comment = "颜色值和Alpha值取值范围0-255. 使用'地面纹理变化 - 设置永久渲染状态' 来显示创建的纹理变化. 暂停状态表示动画播放完毕后是否继续保留该纹理变化. 会创建纹理变化."
    public static JUbersplat CreateUbersplat(float x, float y, string name, int red, int green, int blue, int alpha,
        bool forcePaused, bool noBirthTime)
    {
        var handle = War3.CallNative<int>(_createUbersplatPtr, x, y, name, red, green, blue, alpha, forcePaused,
            noBirthTime);
        return new JUbersplat(handle);
    }


    /// title = "删除地面纹理变化"
    /// description = "删除 ${Ubersplat}"
    /// comment = ""
    public static void DestroyUbersplat(JUbersplat whichSplat)
    {
        War3.CallNative(_destroyUbersplatPtr, whichSplat.Handle);
    }


    /// title = "重置地面纹理变化"
    /// description = "重置 ${Ubersplat}"
    /// comment = ""
    public static void ResetUbersplat(JUbersplat whichSplat)
    {
        War3.CallNative(_resetUbersplatPtr, whichSplat.Handle);
    }


    /// title = "结束地面纹理变化"
    /// description = "结束 ${Ubersplat}"
    /// comment = "在动画播放完毕时自动清除该地面纹理变化."
    public static void FinishUbersplat(JUbersplat whichSplat)
    {
        War3.CallNative(_finishUbersplatPtr, whichSplat.Handle);
    }


    /// title = "显示/隐藏 地面纹理变化[R]"
    /// description = "设置 ${Ubersplat} 状态为 ${Show/Hide}"
    /// comment = ""
    public static void ShowUbersplat(JUbersplat whichSplat, bool flag)
    {
        War3.CallNative(_showUbersplatPtr, whichSplat.Handle, flag);
    }


    /// title = "设置渲染状态"
    /// description = "设置 ${Ubersplat} : ${Enable/Disable} 渲染状态"
    /// comment = "未发现有任何作用."
    public static void SetUbersplatRender(JUbersplat whichSplat, bool flag)
    {
        War3.CallNative(_setUbersplatRenderPtr, whichSplat.Handle, flag);
    }


    /// title = "设置永久渲染状态"
    /// description = "设置 ${Ubersplat} : ${Enable/Disable} 永久渲染状态"
    /// comment = "要显示地面纹理变化则必须开启该项."
    public static void SetUbersplatRenderAlways(JUbersplat whichSplat, bool flag)
    {
        War3.CallNative(_setUbersplatRenderAlwaysPtr, whichSplat.Handle, flag);
    }


    /// title = "创建/删除荒芜地表(圆范围)(指定坐标) [R]"
    /// description = "为 ${Player} 在圆心为(${X},${Y}),半径为 ${R} 的圆范围内 ${Create/Remove} 一块荒芜地表"
    /// comment = ""
    public static void SetBlight(JPlayer whichPlayer, float x, float y, float radius, bool addBlight)
    {
        War3.CallNative(_setBlightPtr, whichPlayer.Handle, x, y, radius, addBlight);
    }


    /// title = "创建/删除荒芜地表(矩形区域) [R]"
    /// description = "为 ${Player} 在 ${Region} ${Create/Remove} 一块荒芜地表"
    /// comment = ""
    public static void SetBlightRect(JPlayer whichPlayer, JRect r, bool addBlight)
    {
        War3.CallNative(_setBlightRectPtr, whichPlayer.Handle, r.Handle, addBlight);
    }

    public static void SetBlightPoint(JPlayer whichPlayer, float x, float y, bool addBlight)
    {
        War3.CallNative(_setBlightPointPtr, whichPlayer.Handle, x, y, addBlight);
    }

    public static void SetBlightLoc(JPlayer whichPlayer, JLocation whichLocation, float radius, bool addBlight)
    {
        War3.CallNative(_setBlightLocPtr, whichPlayer.Handle, whichLocation.Handle, radius, addBlight);
    }


    /// title = "新建不死族金矿 [R]"
    /// description = "新建 ${玩家} 的不死族金矿在(${X},${Y}),面向角度:${Face} 度"
    /// comment = ""
    public static JUnit CreateBlightedGoldmine(JPlayer id, float x, float y, float face)
    {
        var handle = War3.CallNative<int>(_createBlightedGoldminePtr, id.Handle, x, y, face);
        return new JUnit(handle);
    }


    /// title = "坐标点被荒芜地表覆盖 [R]"
    /// description = "坐标点(${X},${Y})被荒芜地表覆盖"
    /// comment = ""
    public static bool IsPointBlighted(float x, float y)
    {
        return War3.CallNative<bool>(_isPointBlightedPtr, x, y);
    }


    /// title = "播放圆范围内地形装饰物动画 [R]"
    /// description = "选取圆心为(${X},${Y}),半径为 ${半径} 的圆范围内的 ${装饰物类型}(选取方式:${选取方式}), 做 ${Animation Name} 动作(${允许/禁止} 随机播放)"
    /// comment = "特殊动画名: 'show', 'hide', 'soundon', 'soundoff'. 随机播放:比如某装饰物有好几个'stand'动作,则允许该项时会随机抽取某个动作播放,而禁止该项时只播放首个动作."
    public static void SetDoodadAnimation(float x, float y, float radius, int doodadID, bool nearestOnly,
        string animName, bool animRandom)
    {
        War3.CallNative(_setDoodadAnimationPtr, x, y, radius, doodadID, nearestOnly, animName, animRandom);
    }


    /// title = "播放矩形区域内地形装饰物动画 [R]"
    /// description = "播放 ${Rect} 内所有 ${装饰物类型} 的 ${Animation Name} 动作(${允许/禁止} 随机播放)"
    /// comment = "特殊动画名: 'show', 'hide', 'soundon', 'soundoff'. 随机播放:比如某装饰物有好几个'stand'动作,则允许该项时会随机抽取某个动作播放,而禁止该项时只播放首个动作."
    public static void SetDoodadAnimationRect(JRect r, int doodadID, string animName, bool animRandom)
    {
        War3.CallNative(_setDoodadAnimationRectPtr, r.Handle, doodadID, animName, animRandom);
    }


    /// title = "启用对战AI"
    /// description = "为 ${Player} 启用对战AI: ${Script}"
    /// comment = "AI只能对电脑玩家使用,当运行该动作后,与之配匹的电脑玩家会强制执行该AI脚本."
    public static void StartMeleeAI(JPlayer num, string script)
    {
        War3.CallNative(_startMeleeAIPtr, num.Handle, script);
    }


    /// title = "启用战役AI"
    /// description = "为 ${Player} 启用战役AI: ${Script}"
    /// comment = "AI只能对电脑玩家使用,当运行该动作后,与之配匹的电脑玩家会强制执行该AI脚本."
    public static void StartCampaignAI(JPlayer num, string script)
    {
        War3.CallNative(_startCampaignAIPtr, num.Handle, script);
    }


    /// title = "发送AI命令"
    /// description = "对 ${Player} 发送AI命令:(${命令}, ${数据})"
    /// comment = "发送的AI命令将被AI脚本所使用."
    public static void CommandAI(JPlayer num, int command, int data)
    {
        War3.CallNative(_commandAIPtr, num.Handle, command, data);
    }


    /// title = "暂停/恢复 AI脚本运行 [R]"
    /// description = "设定 ${Player} ${暂停/恢复} 当前AI脚本的运行"
    /// comment = "事实上该函数是有问题的,可以这么理解:设玩家当前AI脚本的运行状态R为0,暂停1次则R+1,恢复1次则R-1,仅当R=0时该玩家才会运行AI. 在使用前请先理解这段话的意思."
    public static void PauseCompAI(JPlayer p, bool pause)
    {
        War3.CallNative(_pauseCompAIPtr, p.Handle, pause);
    }


    /// title = "玩家的AI难度"
    /// description = "${Player} 的对战AI难度"
    /// comment = "对非AI玩家返回普通难度."
    public static JAiDifficulty GetAIDifficulty(JPlayer num)
    {
        var handle = War3.CallNative<int>(_getAIDifficultyPtr, num.Handle);
        return new JAiDifficulty(handle);
    }


    /// title = "忽视指定单位的警戒点"
    /// description = "忽视 ${单位} 的警戒点"
    /// comment = "单位将不会自动返回原警戒点. 一个很有用的功能就是刷怪进攻时忽视单位警戒范围的话,怪就不会想家了."
    public static void RemoveGuardPosition(JUnit hUnit)
    {
        War3.CallNative(_removeGuardPositionPtr, hUnit.Handle);
    }


    /// title = "恢复指定单位的警戒点"
    /// description = "恢复 ${单位} 的警戒点"
    /// comment = "这个动作通过 AI 来恢复特定单位的警戒点."
    public static void RecycleGuardPosition(JUnit hUnit)
    {
        War3.CallNative(_recycleGuardPositionPtr, hUnit.Handle);
    }


    /// title = "忽视所有单位的警戒点"
    /// description = "忽视 ${Player} 的所有单位的警戒点"
    /// comment = "单位将不会自动返回原警戒点. 一个很有用的功能就是刷怪进攻时忽视单位警戒范围的话,怪就不会想家了."
    public static void RemoveAllGuardPositions(JPlayer num)
    {
        War3.CallNative(_removeAllGuardPositionsPtr, num.Handle);
    }


    /// title = "输入作弊码 [R]"
    /// description = "输入作弊码: ${String}"
    /// comment = "作弊码只在单机有效."
    public static void Cheat(string cheatStr)
    {
        War3.CallNative(_cheatPtr, cheatStr);
    }


    /// title = "无法胜利 [R]"
    /// description = "玩家开启作弊模式: 无法胜利"
    /// comment = "单机作弊码开启的模式."
    public static bool IsNoVictoryCheat()
    {
        return War3.CallNative<bool>(_isNoVictoryCheatPtr);
    }


    /// title = "无法失败 [R]"
    /// description = "玩家开启作弊模式: 无法失败"
    /// comment = "单机作弊码开启的模式."
    public static bool IsNoDefeatCheat()
    {
        return War3.CallNative<bool>(_isNoDefeatCheatPtr);
    }


    /// title = "预载文件"
    /// description = "预载 ${文件}"
    /// comment = "可以事先载入文件并调入到游戏内存,以加快游戏的速度."
    public static void Preload(string filename)
    {
        War3.CallNative(_preloadPtr, filename);
    }


    /// title = "开始预载"
    /// description = "开始预载, 超时设置 ${Time} 秒"
    /// comment = "将文件调入到游戏内存中."
    public static void PreloadEnd(float timeout)
    {
        War3.CallNative(_preloadEndPtr, timeout);
    }

    public static void PreloadStart()
    {
        War3.CallNative(_preloadStartPtr);
    }

    public static void PreloadRefresh()
    {
        War3.CallNative(_preloadRefreshPtr);
    }

    public static void PreloadEndEx()
    {
        War3.CallNative(_preloadEndExPtr);
    }

    public static void PreloadGenClear()
    {
        War3.CallNative(_preloadGenClearPtr);
    }

    public static void PreloadGenStart()
    {
        War3.CallNative(_preloadGenStartPtr);
    }

    public static void PreloadGenEnd(string filename)
    {
        War3.CallNative(_preloadGenEndPtr, filename);
    }


    /// title = "批量预载"
    /// description = "预载所有在 ${文件} 中列出的文件"
    /// comment = ""
    public static void Preloader(string filename)
    {
        War3.CallNative(_preloaderPtr, filename);
    }
}
