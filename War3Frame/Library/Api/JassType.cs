namespace War3Frame;

public class JHandle
{
    public JHandle()
    {
        Handle = IntPtr.Zero;
    }

    public JHandle(IntPtr JHandle)
    {
        Handle = JHandle;
    }

    public JHandle(int JHandle)
    {
        Handle = new IntPtr(JHandle);
    }

    public IntPtr Handle { get; protected set; }

    public static explicit operator bool(JHandle h)
    {
        return h?.Handle != IntPtr.Zero;
    }
}

public class JAgent : JHandle
{
    public JAgent()
    {
    }

    public JAgent(IntPtr JHandle) : base(JHandle)
    {
    }

    public JAgent(int JHandle) : base(JHandle)
    {
    }
}

public class JEvent : JAgent
{
    public JEvent()
    {
    }

    public JEvent(IntPtr JHandle) : base(JHandle)
    {
    }

    public JEvent(int JHandle) : base(JHandle)
    {
    }
}

public class JPlayer : JAgent
{
    public JPlayer()
    {
    }

    public JPlayer(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPlayer(int JHandle) : base(JHandle)
    {
    }
}

public class JWidget : JAgent
{
    public JWidget()
    {
    }

    public JWidget(IntPtr JHandle) : base(JHandle)
    {
    }

    public JWidget(int JHandle) : base(JHandle)
    {
    }
}

public class JUnit : JWidget
{
    public JUnit()
    {
    }

    public JUnit(IntPtr JHandle) : base(JHandle)
    {
    }

    public JUnit(int JHandle) : base(JHandle)
    {
    }
}

public class JDestructable : JWidget
{
    public JDestructable()
    {
    }

    public JDestructable(IntPtr JHandle) : base(JHandle)
    {
    }

    public JDestructable(int JHandle) : base(JHandle)
    {
    }
}

public class JItem : JWidget
{
    public JItem()
    {
    }

    public JItem(IntPtr JHandle) : base(JHandle)
    {
    }

    public JItem(int JHandle) : base(JHandle)
    {
    }
}

public class JAbility : JAgent
{
    public JAbility()
    {
    }

    public JAbility(IntPtr JHandle) : base(JHandle)
    {
    }

    public JAbility(int JHandle) : base(JHandle)
    {
    }
}

public class JBuff : JAbility
{
    public JBuff()
    {
    }

    public JBuff(IntPtr JHandle) : base(JHandle)
    {
    }

    public JBuff(int JHandle) : base(JHandle)
    {
    }
}

public class JForce : JAgent
{
    public JForce()
    {
    }

    public JForce(IntPtr JHandle) : base(JHandle)
    {
    }

    public JForce(int JHandle) : base(JHandle)
    {
    }
}

public class JGroup : JAgent
{
    public JGroup()
    {
    }

    public JGroup(IntPtr JHandle) : base(JHandle)
    {
    }

    public JGroup(int JHandle) : base(JHandle)
    {
    }
}

public class JTrigger : JAgent
{
    public JTrigger()
    {
    }

    public JTrigger(IntPtr JHandle) : base(JHandle)
    {
    }

    public JTrigger(int JHandle) : base(JHandle)
    {
    }
}

public class JTriggerCondition : JAgent
{
    public JTriggerCondition()
    {
    }

    public JTriggerCondition(IntPtr JHandle) : base(JHandle)
    {
    }

    public JTriggerCondition(int JHandle) : base(JHandle)
    {
    }
}

public class JTriggerAction : JHandle
{
    public JTriggerAction()
    {
    }

    public JTriggerAction(IntPtr JHandle) : base(JHandle)
    {
    }

    public JTriggerAction(int JHandle) : base(JHandle)
    {
    }
}

public class JTimer : JAgent
{
    public JTimer()
    {
    }

    public JTimer(IntPtr JHandle) : base(JHandle)
    {
    }

    public JTimer(int JHandle) : base(JHandle)
    {
    }
}

public class JLocation : JAgent
{
    public JLocation()
    {
    }

    public JLocation(IntPtr JHandle) : base(JHandle)
    {
    }

    public JLocation(int JHandle) : base(JHandle)
    {
    }
}

public class JRegion : JAgent
{
    public JRegion()
    {
    }

    public JRegion(IntPtr JHandle) : base(JHandle)
    {
    }

    public JRegion(int JHandle) : base(JHandle)
    {
    }
}

public class JRect : JAgent
{
    public JRect()
    {
    }

    public JRect(IntPtr JHandle) : base(JHandle)
    {
    }

    public JRect(int JHandle) : base(JHandle)
    {
    }
}

public class JBoolExpr : JAgent
{
    public JBoolExpr()
    {
    }

    public JBoolExpr(IntPtr JHandle) : base(JHandle)
    {
    }

    public JBoolExpr(int JHandle) : base(JHandle)
    {
    }
}

public class JSound : JAgent
{
    public JSound()
    {
    }

    public JSound(IntPtr JHandle) : base(JHandle)
    {
    }

    public JSound(int JHandle) : base(JHandle)
    {
    }
}

public class JConditionFunc : JBoolExpr
{
    public JConditionFunc()
    {
    }

    public JConditionFunc(IntPtr JHandle) : base(JHandle)
    {
    }

    public JConditionFunc(int JHandle) : base(JHandle)
    {
    }
}

public class JFilterFunc : JBoolExpr
{
    public JFilterFunc()
    {
    }

    public JFilterFunc(IntPtr JHandle) : base(JHandle)
    {
    }

    public JFilterFunc(int JHandle) : base(JHandle)
    {
    }
}

public class JUnitPool : JHandle
{
    public JUnitPool()
    {
    }

    public JUnitPool(IntPtr JHandle) : base(JHandle)
    {
    }

    public JUnitPool(int JHandle) : base(JHandle)
    {
    }
}

public class JItemPool : JHandle
{
    public JItemPool()
    {
    }

    public JItemPool(IntPtr JHandle) : base(JHandle)
    {
    }

    public JItemPool(int JHandle) : base(JHandle)
    {
    }
}

public class JRace : JHandle
{
    public JRace()
    {
    }

    public JRace(IntPtr JHandle) : base(JHandle)
    {
    }

    public JRace(int JHandle) : base(JHandle)
    {
    }
}

public class JAllianceType : JHandle
{
    public JAllianceType()
    {
    }

    public JAllianceType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JAllianceType(int JHandle) : base(JHandle)
    {
    }
}

public class JRacePreference : JHandle
{
    public JRacePreference()
    {
    }

    public JRacePreference(IntPtr JHandle) : base(JHandle)
    {
    }

    public JRacePreference(int JHandle) : base(JHandle)
    {
    }
}

public class JGameState : JHandle
{
    public JGameState()
    {
    }

    public JGameState(IntPtr JHandle) : base(JHandle)
    {
    }

    public JGameState(int JHandle) : base(JHandle)
    {
    }
}

public class JIGameState : JGameState
{
    public JIGameState()
    {
    }

    public JIGameState(IntPtr JHandle) : base(JHandle)
    {
    }

    public JIGameState(int JHandle) : base(JHandle)
    {
    }
}

public class JFGameState : JGameState
{
    public JFGameState()
    {
    }

    public JFGameState(IntPtr JHandle) : base(JHandle)
    {
    }

    public JFGameState(int JHandle) : base(JHandle)
    {
    }
}

public class JPlayerState : JHandle
{
    public JPlayerState()
    {
    }

    public JPlayerState(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPlayerState(int JHandle) : base(JHandle)
    {
    }
}

public class JPlayerScore : JHandle
{
    public JPlayerScore()
    {
    }

    public JPlayerScore(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPlayerScore(int JHandle) : base(JHandle)
    {
    }
}

public class JPlayerGameResult : JHandle
{
    public JPlayerGameResult()
    {
    }

    public JPlayerGameResult(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPlayerGameResult(int JHandle) : base(JHandle)
    {
    }
}

public class JUnitState : JHandle
{
    public JUnitState()
    {
    }

    public JUnitState(IntPtr JHandle) : base(JHandle)
    {
    }

    public JUnitState(int JHandle) : base(JHandle)
    {
    }
}

public class JAiDifficulty : JHandle
{
    public JAiDifficulty()
    {
    }

    public JAiDifficulty(IntPtr JHandle) : base(JHandle)
    {
    }

    public JAiDifficulty(int JHandle) : base(JHandle)
    {
    }
}

public class JEventId : JHandle
{
    public JEventId()
    {
    }

    public JEventId(IntPtr JHandle) : base(JHandle)
    {
    }

    public JEventId(int JHandle) : base(JHandle)
    {
    }
}

public class JGameEvent : JEventId
{
    public JGameEvent()
    {
    }

    public JGameEvent(IntPtr JHandle) : base(JHandle)
    {
    }

    public JGameEvent(int JHandle) : base(JHandle)
    {
    }
}

public class JPlayerEvent : JEventId
{
    public JPlayerEvent()
    {
    }

    public JPlayerEvent(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPlayerEvent(int JHandle) : base(JHandle)
    {
    }
}

public class JPlayerUnitEvent : JEventId
{
    public JPlayerUnitEvent()
    {
    }

    public JPlayerUnitEvent(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPlayerUnitEvent(int JHandle) : base(JHandle)
    {
    }
}

public class JUnitEvent : JEventId
{
    public JUnitEvent()
    {
    }

    public JUnitEvent(IntPtr JHandle) : base(JHandle)
    {
    }

    public JUnitEvent(int JHandle) : base(JHandle)
    {
    }
}

public class JLimitOp : JEventId
{
    public JLimitOp()
    {
    }

    public JLimitOp(IntPtr JHandle) : base(JHandle)
    {
    }

    public JLimitOp(int JHandle) : base(JHandle)
    {
    }
}

public class JWidgetEvent : JEventId
{
    public JWidgetEvent()
    {
    }

    public JWidgetEvent(IntPtr JHandle) : base(JHandle)
    {
    }

    public JWidgetEvent(int JHandle) : base(JHandle)
    {
    }
}

public class JDialogEvent : JEventId
{
    public JDialogEvent()
    {
    }

    public JDialogEvent(IntPtr JHandle) : base(JHandle)
    {
    }

    public JDialogEvent(int JHandle) : base(JHandle)
    {
    }
}

public class JUnitType : JHandle
{
    public JUnitType()
    {
    }

    public JUnitType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JUnitType(int JHandle) : base(JHandle)
    {
    }
}

public class JGameSpeed : JHandle
{
    public JGameSpeed()
    {
    }

    public JGameSpeed(IntPtr JHandle) : base(JHandle)
    {
    }

    public JGameSpeed(int JHandle) : base(JHandle)
    {
    }
}

public class JGameDifficulty : JHandle
{
    public JGameDifficulty()
    {
    }

    public JGameDifficulty(IntPtr JHandle) : base(JHandle)
    {
    }

    public JGameDifficulty(int JHandle) : base(JHandle)
    {
    }
}

public class JGameType : JHandle
{
    public JGameType()
    {
    }

    public JGameType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JGameType(int JHandle) : base(JHandle)
    {
    }
}

public class JMapFlag : JHandle
{
    public JMapFlag()
    {
    }

    public JMapFlag(IntPtr JHandle) : base(JHandle)
    {
    }

    public JMapFlag(int JHandle) : base(JHandle)
    {
    }
}

public class JMapVisibility : JHandle
{
    public JMapVisibility()
    {
    }

    public JMapVisibility(IntPtr JHandle) : base(JHandle)
    {
    }

    public JMapVisibility(int JHandle) : base(JHandle)
    {
    }
}

public class JMapSetting : JHandle
{
    public JMapSetting()
    {
    }

    public JMapSetting(IntPtr JHandle) : base(JHandle)
    {
    }

    public JMapSetting(int JHandle) : base(JHandle)
    {
    }
}

public class JMapDensity : JHandle
{
    public JMapDensity()
    {
    }

    public JMapDensity(IntPtr JHandle) : base(JHandle)
    {
    }

    public JMapDensity(int JHandle) : base(JHandle)
    {
    }
}

public class JMapControl : JHandle
{
    public JMapControl()
    {
    }

    public JMapControl(IntPtr JHandle) : base(JHandle)
    {
    }

    public JMapControl(int JHandle) : base(JHandle)
    {
    }
}

public class JPlayerSlotState : JHandle
{
    public JPlayerSlotState()
    {
    }

    public JPlayerSlotState(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPlayerSlotState(int JHandle) : base(JHandle)
    {
    }
}

public class JVolumeGroup : JHandle
{
    public JVolumeGroup()
    {
    }

    public JVolumeGroup(IntPtr JHandle) : base(JHandle)
    {
    }

    public JVolumeGroup(int JHandle) : base(JHandle)
    {
    }
}

public class JCameraField : JHandle
{
    public JCameraField()
    {
    }

    public JCameraField(IntPtr JHandle) : base(JHandle)
    {
    }

    public JCameraField(int JHandle) : base(JHandle)
    {
    }
}

public class JCameraSetup : JHandle
{
    public JCameraSetup()
    {
    }

    public JCameraSetup(IntPtr JHandle) : base(JHandle)
    {
    }

    public JCameraSetup(int JHandle) : base(JHandle)
    {
    }
}

public class JPlayerColor : JHandle
{
    public JPlayerColor()
    {
    }

    public JPlayerColor(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPlayerColor(int JHandle) : base(JHandle)
    {
    }
}

public class JPlacement : JHandle
{
    public JPlacement()
    {
    }

    public JPlacement(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPlacement(int JHandle) : base(JHandle)
    {
    }
}

public class JStartLocPrio : JHandle
{
    public JStartLocPrio()
    {
    }

    public JStartLocPrio(IntPtr JHandle) : base(JHandle)
    {
    }

    public JStartLocPrio(int JHandle) : base(JHandle)
    {
    }
}

public class JRarityControl : JHandle
{
    public JRarityControl()
    {
    }

    public JRarityControl(IntPtr JHandle) : base(JHandle)
    {
    }

    public JRarityControl(int JHandle) : base(JHandle)
    {
    }
}

public class JBlendMode : JHandle
{
    public JBlendMode()
    {
    }

    public JBlendMode(IntPtr JHandle) : base(JHandle)
    {
    }

    public JBlendMode(int JHandle) : base(JHandle)
    {
    }
}

public class JTexMapFlags : JHandle
{
    public JTexMapFlags()
    {
    }

    public JTexMapFlags(IntPtr JHandle) : base(JHandle)
    {
    }

    public JTexMapFlags(int JHandle) : base(JHandle)
    {
    }
}

public class JEffect : JAgent
{
    public JEffect()
    {
    }

    public JEffect(IntPtr JHandle) : base(JHandle)
    {
    }

    public JEffect(int JHandle) : base(JHandle)
    {
    }
}

public class JEffectType : JHandle
{
    public JEffectType()
    {
    }

    public JEffectType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JEffectType(int JHandle) : base(JHandle)
    {
    }
}

public class JWeatherEffect : JHandle
{
    public JWeatherEffect()
    {
    }

    public JWeatherEffect(IntPtr JHandle) : base(JHandle)
    {
    }

    public JWeatherEffect(int JHandle) : base(JHandle)
    {
    }
}

public class JTerrainDeformation : JHandle
{
    public JTerrainDeformation()
    {
    }

    public JTerrainDeformation(IntPtr JHandle) : base(JHandle)
    {
    }

    public JTerrainDeformation(int JHandle) : base(JHandle)
    {
    }
}

public class JFogState : JHandle
{
    public JFogState()
    {
    }

    public JFogState(IntPtr JHandle) : base(JHandle)
    {
    }

    public JFogState(int JHandle) : base(JHandle)
    {
    }
}

public class JFogModifier : JAgent
{
    public JFogModifier()
    {
    }

    public JFogModifier(IntPtr JHandle) : base(JHandle)
    {
    }

    public JFogModifier(int JHandle) : base(JHandle)
    {
    }
}

public class JDialog : JAgent
{
    public JDialog()
    {
    }

    public JDialog(IntPtr JHandle) : base(JHandle)
    {
    }

    public JDialog(int JHandle) : base(JHandle)
    {
    }
}

public class JButton : JAgent
{
    public JButton()
    {
    }

    public JButton(IntPtr JHandle) : base(JHandle)
    {
    }

    public JButton(int JHandle) : base(JHandle)
    {
    }
}

public class JQuest : JAgent
{
    public JQuest()
    {
    }

    public JQuest(IntPtr JHandle) : base(JHandle)
    {
    }

    public JQuest(int JHandle) : base(JHandle)
    {
    }
}

public class JQuestItem : JAgent
{
    public JQuestItem()
    {
    }

    public JQuestItem(IntPtr JHandle) : base(JHandle)
    {
    }

    public JQuestItem(int JHandle) : base(JHandle)
    {
    }
}

public class JDefeatCondition : JAgent
{
    public JDefeatCondition()
    {
    }

    public JDefeatCondition(IntPtr JHandle) : base(JHandle)
    {
    }

    public JDefeatCondition(int JHandle) : base(JHandle)
    {
    }
}

public class JTimerDialog : JAgent
{
    public JTimerDialog()
    {
    }

    public JTimerDialog(IntPtr JHandle) : base(JHandle)
    {
    }

    public JTimerDialog(int JHandle) : base(JHandle)
    {
    }
}

public class JLeaderboard : JAgent
{
    public JLeaderboard()
    {
    }

    public JLeaderboard(IntPtr JHandle) : base(JHandle)
    {
    }

    public JLeaderboard(int JHandle) : base(JHandle)
    {
    }
}

public class JMultiboard : JAgent
{
    public JMultiboard()
    {
    }

    public JMultiboard(IntPtr JHandle) : base(JHandle)
    {
    }

    public JMultiboard(int JHandle) : base(JHandle)
    {
    }
}

public class JMultiboardItem : JAgent
{
    public JMultiboardItem()
    {
    }

    public JMultiboardItem(IntPtr JHandle) : base(JHandle)
    {
    }

    public JMultiboardItem(int JHandle) : base(JHandle)
    {
    }
}

public class JTrackable : JAgent
{
    public JTrackable()
    {
    }

    public JTrackable(IntPtr JHandle) : base(JHandle)
    {
    }

    public JTrackable(int JHandle) : base(JHandle)
    {
    }
}

public class JGameCache : JAgent
{
    public JGameCache()
    {
    }

    public JGameCache(IntPtr JHandle) : base(JHandle)
    {
    }

    public JGameCache(int JHandle) : base(JHandle)
    {
    }
}

public class JVersion : JHandle
{
    public JVersion()
    {
    }

    public JVersion(IntPtr JHandle) : base(JHandle)
    {
    }

    public JVersion(int JHandle) : base(JHandle)
    {
    }
}

public class JItemType : JHandle
{
    public JItemType()
    {
    }

    public JItemType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JItemType(int JHandle) : base(JHandle)
    {
    }
}

public class JTextTag : JHandle
{
    public JTextTag()
    {
    }

    public JTextTag(IntPtr JHandle) : base(JHandle)
    {
    }

    public JTextTag(int JHandle) : base(JHandle)
    {
    }
}

public class JAttackType : JHandle
{
    public JAttackType()
    {
    }

    public JAttackType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JAttackType(int JHandle) : base(JHandle)
    {
    }
}

public class JDamageType : JHandle
{
    public JDamageType()
    {
    }

    public JDamageType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JDamageType(int JHandle) : base(JHandle)
    {
    }
}

public class JWeaponType : JHandle
{
    public JWeaponType()
    {
    }

    public JWeaponType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JWeaponType(int JHandle) : base(JHandle)
    {
    }
}

public class JSoundType : JHandle
{
    public JSoundType()
    {
    }

    public JSoundType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JSoundType(int JHandle) : base(JHandle)
    {
    }
}

public class JLightning : JHandle
{
    public JLightning()
    {
    }

    public JLightning(IntPtr JHandle) : base(JHandle)
    {
    }

    public JLightning(int JHandle) : base(JHandle)
    {
    }
}

public class JPathingType : JHandle
{
    public JPathingType()
    {
    }

    public JPathingType(IntPtr JHandle) : base(JHandle)
    {
    }

    public JPathingType(int JHandle) : base(JHandle)
    {
    }
}

public class JImage : JHandle
{
    public JImage()
    {
    }

    public JImage(IntPtr JHandle) : base(JHandle)
    {
    }

    public JImage(int JHandle) : base(JHandle)
    {
    }
}

public class JUbersplat : JHandle
{
    public JUbersplat()
    {
    }

    public JUbersplat(IntPtr JHandle) : base(JHandle)
    {
    }

    public JUbersplat(int JHandle) : base(JHandle)
    {
    }
}

public class JHashtable : JAgent
{
    public JHashtable()
    {
    }

    public JHashtable(IntPtr JHandle) : base(JHandle)
    {
    }

    public JHashtable(int JHandle) : base(JHandle)
    {
    }
}