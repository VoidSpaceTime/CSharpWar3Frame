using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame
{
    public class JHandle
    {
        public IntPtr Handle { get; protected set; }

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

        public static explicit operator bool(JHandle h)
        {
            return h?.Handle != IntPtr.Zero;
        }
    }

    public class JAgent : JHandle
    {
        public JAgent() : base() { }
        public JAgent(IntPtr JHandle) : base(JHandle) { }
        public JAgent(int JHandle) : base(JHandle) { }
    }

    public class JEvent : JAgent
    {
        public JEvent() : base() { }
        public JEvent(IntPtr JHandle) : base(JHandle) { }
        public JEvent(int JHandle) : base(JHandle) { }
    }

    public class JPlayer : JAgent
    {
        public JPlayer() : base() { }
        public JPlayer(IntPtr JHandle) : base(JHandle) { }
        public JPlayer(int JHandle) : base(JHandle) { }
    }

    public class JWidget : JAgent
    {
        public JWidget() : base() { }
        public JWidget(IntPtr JHandle) : base(JHandle) { }
        public JWidget(int JHandle) : base(JHandle) { }
    }

    public class JUnit : JWidget
    {
        public JUnit() : base() { }
        public JUnit(IntPtr JHandle) : base(JHandle) { }
        public JUnit(int JHandle) : base(JHandle) { }
    }

    public class JDestructable : JWidget
    {
        public JDestructable() : base() { }
        public JDestructable(IntPtr JHandle) : base(JHandle) { }
        public JDestructable(int JHandle) : base(JHandle) { }
    }

    public class JItem : JWidget
    {
        public JItem() : base() { }
        public JItem(IntPtr JHandle) : base(JHandle) { }
        public JItem(int JHandle) : base(JHandle) { }
    }

    public class JAbility : JAgent
    {
        public JAbility() : base() { }
        public JAbility(IntPtr JHandle) : base(JHandle) { }
        public JAbility(int JHandle) : base(JHandle) { }
    }

    public class JBuff : JAbility
    {
        public JBuff() : base() { }
        public JBuff(IntPtr JHandle) : base(JHandle) { }
        public JBuff(int JHandle) : base(JHandle) { }
    }

    public class JForce : JAgent
    {
        public JForce() : base() { }
        public JForce(IntPtr JHandle) : base(JHandle) { }
        public JForce(int JHandle) : base(JHandle) { }
    }

    public class JGroup : JAgent
    {
        public JGroup() : base() { }
        public JGroup(IntPtr JHandle) : base(JHandle) { }
        public JGroup(int JHandle) : base(JHandle) { }
    }

    public class JTrigger : JAgent
    {
        public JTrigger() : base() { }
        public JTrigger(IntPtr JHandle) : base(JHandle) { }
        public JTrigger(int JHandle) : base(JHandle) { }
    }

    public class JTriggerCondition : JAgent
    {
        public JTriggerCondition() : base() { }
        public JTriggerCondition(IntPtr JHandle) : base(JHandle) { }
        public JTriggerCondition(int JHandle) : base(JHandle) { }
    }

    public class JTriggerAction : JHandle
    {
        public JTriggerAction() : base() { }
        public JTriggerAction(IntPtr JHandle) : base(JHandle) { }
        public JTriggerAction(int JHandle) : base(JHandle) { }
    }

    public class JTimer : JAgent
    {
        public JTimer() : base() { }
        public JTimer(IntPtr JHandle) : base(JHandle) { }
        public JTimer(int JHandle) : base(JHandle) { }
    }

    public class JLocation : JAgent
    {
        public JLocation() : base() { }
        public JLocation(IntPtr JHandle) : base(JHandle) { }
        public JLocation(int JHandle) : base(JHandle) { }
    }

    public class JRegion : JAgent
    {
        public JRegion() : base() { }
        public JRegion(IntPtr JHandle) : base(JHandle) { }
        public JRegion(int JHandle) : base(JHandle) { }
    }

    public class JRect : JAgent
    {
        public JRect() : base() { }
        public JRect(IntPtr JHandle) : base(JHandle) { }
        public JRect(int JHandle) : base(JHandle) { }
    }

    public class JBoolExpr : JAgent
    {
        public JBoolExpr() : base() { }
        public JBoolExpr(IntPtr JHandle) : base(JHandle) { }
        public JBoolExpr(int JHandle) : base(JHandle) { }
    }

    public class JSound : JAgent
    {
        public JSound() : base() { }
        public JSound(IntPtr JHandle) : base(JHandle) { }
        public JSound(int JHandle) : base(JHandle) { }
    }

    public class JConditionFunc : JBoolExpr
    {
        public JConditionFunc() : base() { }
        public JConditionFunc(IntPtr JHandle) : base(JHandle) { }
        public JConditionFunc(int JHandle) : base(JHandle) { }
    }

    public class JFilterFunc : JBoolExpr
    {
        public JFilterFunc() : base() { }
        public JFilterFunc(IntPtr JHandle) : base(JHandle) { }
        public JFilterFunc(int JHandle) : base(JHandle) { }
    }

    public class JUnitPool : JHandle
    {
        public JUnitPool() : base() { }
        public JUnitPool(IntPtr JHandle) : base(JHandle) { }
        public JUnitPool(int JHandle) : base(JHandle) { }
    }

    public class JItemPool : JHandle
    {
        public JItemPool() : base() { }
        public JItemPool(IntPtr JHandle) : base(JHandle) { }
        public JItemPool(int JHandle) : base(JHandle) { }
    }

    public class JRace : JHandle
    {
        public JRace() : base() { }
        public JRace(IntPtr JHandle) : base(JHandle) { }
        public JRace(int JHandle) : base(JHandle) { }
    }

    public class JAllianceType : JHandle
    {
        public JAllianceType() : base() { }
        public JAllianceType(IntPtr JHandle) : base(JHandle) { }
        public JAllianceType(int JHandle) : base(JHandle) { }
    }

    public class JRacePreference : JHandle
    {
        public JRacePreference() : base() { }
        public JRacePreference(IntPtr JHandle) : base(JHandle) { }
        public JRacePreference(int JHandle) : base(JHandle) { }
    }

    public class JGameState : JHandle
    {
        public JGameState() : base() { }
        public JGameState(IntPtr JHandle) : base(JHandle) { }
        public JGameState(int JHandle) : base(JHandle) { }
    }

    public class JIGameState : JGameState
    {
        public JIGameState() : base() { }
        public JIGameState(IntPtr JHandle) : base(JHandle) { }
        public JIGameState(int JHandle) : base(JHandle) { }
    }

    public class JFGameState : JGameState
    {
        public JFGameState() : base() { }
        public JFGameState(IntPtr JHandle) : base(JHandle) { }
        public JFGameState(int JHandle) : base(JHandle) { }
    }

    public class JPlayerState : JHandle
    {
        public JPlayerState() : base() { }
        public JPlayerState(IntPtr JHandle) : base(JHandle) { }
        public JPlayerState(int JHandle) : base(JHandle) { }
    }

    public class JPlayerScore : JHandle
    {
        public JPlayerScore() : base() { }
        public JPlayerScore(IntPtr JHandle) : base(JHandle) { }
        public JPlayerScore(int JHandle) : base(JHandle) { }
    }

    public class JPlayerGameResult : JHandle
    {
        public JPlayerGameResult() : base() { }
        public JPlayerGameResult(IntPtr JHandle) : base(JHandle) { }
        public JPlayerGameResult(int JHandle) : base(JHandle) { }
    }

    public class JUnitState : JHandle
    {
        public JUnitState() : base() { }
        public JUnitState(IntPtr JHandle) : base(JHandle) { }
        public JUnitState(int JHandle) : base(JHandle) { }
    }

    public class JAiDifficulty : JHandle
    {
        public JAiDifficulty() : base() { }
        public JAiDifficulty(IntPtr JHandle) : base(JHandle) { }
        public JAiDifficulty(int JHandle) : base(JHandle) { }
    }

    public class JEventId : JHandle
    {
        public JEventId() : base() { }
        public JEventId(IntPtr JHandle) : base(JHandle) { }
        public JEventId(int JHandle) : base(JHandle) { }
    }

    public class JGameEvent : JEventId
    {
        public JGameEvent() : base() { }
        public JGameEvent(IntPtr JHandle) : base(JHandle) { }
        public JGameEvent(int JHandle) : base(JHandle) { }
    }

    public class JPlayerEvent : JEventId
    {
        public JPlayerEvent() : base() { }
        public JPlayerEvent(IntPtr JHandle) : base(JHandle) { }
        public JPlayerEvent(int JHandle) : base(JHandle) { }
    }

    public class JPlayerUnitEvent : JEventId
    {
        public JPlayerUnitEvent() : base() { }
        public JPlayerUnitEvent(IntPtr JHandle) : base(JHandle) { }
        public JPlayerUnitEvent(int JHandle) : base(JHandle) { }
    }

    public class JUnitEvent : JEventId
    {
        public JUnitEvent() : base() { }
        public JUnitEvent(IntPtr JHandle) : base(JHandle) { }
        public JUnitEvent(int JHandle) : base(JHandle) { }
    }

    public class JLimitOp : JEventId
    {
        public JLimitOp() : base() { }
        public JLimitOp(IntPtr JHandle) : base(JHandle) { }
        public JLimitOp(int JHandle) : base(JHandle) { }
    }

    public class JWidgetEvent : JEventId
    {
        public JWidgetEvent() : base() { }
        public JWidgetEvent(IntPtr JHandle) : base(JHandle) { }
        public JWidgetEvent(int JHandle) : base(JHandle) { }
    }

    public class JDialogEvent : JEventId
    {
        public JDialogEvent() : base() { }
        public JDialogEvent(IntPtr JHandle) : base(JHandle) { }
        public JDialogEvent(int JHandle) : base(JHandle) { }
    }

    public class JUnitType : JHandle
    {
        public JUnitType() : base() { }
        public JUnitType(IntPtr JHandle) : base(JHandle) { }
        public JUnitType(int JHandle) : base(JHandle) { }
    }

    public class JGameSpeed : JHandle
    {
        public JGameSpeed() : base() { }
        public JGameSpeed(IntPtr JHandle) : base(JHandle) { }
        public JGameSpeed(int JHandle) : base(JHandle) { }
    }

    public class JGameDifficulty : JHandle
    {
        public JGameDifficulty() : base() { }
        public JGameDifficulty(IntPtr JHandle) : base(JHandle) { }
        public JGameDifficulty(int JHandle) : base(JHandle) { }
    }

    public class JGameType : JHandle
    {
        public JGameType() : base() { }
        public JGameType(IntPtr JHandle) : base(JHandle) { }
        public JGameType(int JHandle) : base(JHandle) { }
    }

    public class JMapFlag : JHandle
    {
        public JMapFlag() : base() { }
        public JMapFlag(IntPtr JHandle) : base(JHandle) { }
        public JMapFlag(int JHandle) : base(JHandle) { }
    }

    public class JMapVisibility : JHandle
    {
        public JMapVisibility() : base() { }
        public JMapVisibility(IntPtr JHandle) : base(JHandle) { }
        public JMapVisibility(int JHandle) : base(JHandle) { }
    }

    public class JMapSetting : JHandle
    {
        public JMapSetting() : base() { }
        public JMapSetting(IntPtr JHandle) : base(JHandle) { }
        public JMapSetting(int JHandle) : base(JHandle) { }
    }

    public class JMapDensity : JHandle
    {
        public JMapDensity() : base() { }
        public JMapDensity(IntPtr JHandle) : base(JHandle) { }
        public JMapDensity(int JHandle) : base(JHandle) { }
    }

    public class JMapControl : JHandle
    {
        public JMapControl() : base() { }
        public JMapControl(IntPtr JHandle) : base(JHandle) { }
        public JMapControl(int JHandle) : base(JHandle) { }
    }

    public class JPlayerSlotState : JHandle
    {
        public JPlayerSlotState() : base() { }
        public JPlayerSlotState(IntPtr JHandle) : base(JHandle) { }
        public JPlayerSlotState(int JHandle) : base(JHandle) { }
    }

    public class JVolumeGroup : JHandle
    {
        public JVolumeGroup() : base() { }
        public JVolumeGroup(IntPtr JHandle) : base(JHandle) { }
        public JVolumeGroup(int JHandle) : base(JHandle) { }
    }

    public class JCameraField : JHandle
    {
        public JCameraField() : base() { }
        public JCameraField(IntPtr JHandle) : base(JHandle) { }
        public JCameraField(int JHandle) : base(JHandle) { }
    }

    public class JCameraSetup : JHandle
    {
        public JCameraSetup() : base() { }
        public JCameraSetup(IntPtr JHandle) : base(JHandle) { }
        public JCameraSetup(int JHandle) : base(JHandle) { }
    }

    public class JPlayerColor : JHandle
    {
        public JPlayerColor() : base() { }
        public JPlayerColor(IntPtr JHandle) : base(JHandle) { }
        public JPlayerColor(int JHandle) : base(JHandle) { }
    }

    public class JPlacement : JHandle
    {
        public JPlacement() : base() { }
        public JPlacement(IntPtr JHandle) : base(JHandle) { }
        public JPlacement(int JHandle) : base(JHandle) { }
    }

    public class JStartLocPrio : JHandle
    {
        public JStartLocPrio() : base() { }
        public JStartLocPrio(IntPtr JHandle) : base(JHandle) { }
        public JStartLocPrio(int JHandle) : base(JHandle) { }
    }

    public class JRarityControl : JHandle
    {
        public JRarityControl() : base() { }
        public JRarityControl(IntPtr JHandle) : base(JHandle) { }
        public JRarityControl(int JHandle) : base(JHandle) { }
    }

    public class JBlendMode : JHandle
    {
        public JBlendMode() : base() { }
        public JBlendMode(IntPtr JHandle) : base(JHandle) { }
        public JBlendMode(int JHandle) : base(JHandle) { }
    }

    public class JTexMapFlags : JHandle
    {
        public JTexMapFlags() : base() { }
        public JTexMapFlags(IntPtr JHandle) : base(JHandle) { }
        public JTexMapFlags(int JHandle) : base(JHandle) { }
    }

    public class JEffect : JAgent
    {
        public JEffect() : base() { }
        public JEffect(IntPtr JHandle) : base(JHandle) { }
        public JEffect(int JHandle) : base(JHandle) { }
    }

    public class JEffectType : JHandle
    {
        public JEffectType() : base() { }
        public JEffectType(IntPtr JHandle) : base(JHandle) { }
        public JEffectType(int JHandle) : base(JHandle) { }
    }

    public class JWeatherEffect : JHandle
    {
        public JWeatherEffect() : base() { }
        public JWeatherEffect(IntPtr JHandle) : base(JHandle) { }
        public JWeatherEffect(int JHandle) : base(JHandle) { }
    }

    public class JTerrainDeformation : JHandle
    {
        public JTerrainDeformation() : base() { }
        public JTerrainDeformation(IntPtr JHandle) : base(JHandle) { }
        public JTerrainDeformation(int JHandle) : base(JHandle) { }
    }

    public class JFogState : JHandle
    {
        public JFogState() : base() { }
        public JFogState(IntPtr JHandle) : base(JHandle) { }
        public JFogState(int JHandle) : base(JHandle) { }
    }

    public class JFogModifier : JAgent
    {
        public JFogModifier() : base() { }
        public JFogModifier(IntPtr JHandle) : base(JHandle) { }
        public JFogModifier(int JHandle) : base(JHandle) { }
    }

    public class JDialog : JAgent
    {
        public JDialog() : base() { }
        public JDialog(IntPtr JHandle) : base(JHandle) { }
        public JDialog(int JHandle) : base(JHandle) { }
    }

    public class JButton : JAgent
    {
        public JButton() : base() { }
        public JButton(IntPtr JHandle) : base(JHandle) { }
        public JButton(int JHandle) : base(JHandle) { }
    }

    public class JQuest : JAgent
    {
        public JQuest() : base() { }
        public JQuest(IntPtr JHandle) : base(JHandle) { }
        public JQuest(int JHandle) : base(JHandle) { }
    }

    public class JQuestItem : JAgent
    {
        public JQuestItem() : base() { }
        public JQuestItem(IntPtr JHandle) : base(JHandle) { }
        public JQuestItem(int JHandle) : base(JHandle) { }
    }

    public class JDefeatCondition : JAgent
    {
        public JDefeatCondition() : base() { }
        public JDefeatCondition(IntPtr JHandle) : base(JHandle) { }
        public JDefeatCondition(int JHandle) : base(JHandle) { }
    }

    public class JTimerDialog : JAgent
    {
        public JTimerDialog() : base() { }
        public JTimerDialog(IntPtr JHandle) : base(JHandle) { }
        public JTimerDialog(int JHandle) : base(JHandle) { }
    }

    public class JLeaderboard : JAgent
    {
        public JLeaderboard() : base() { }
        public JLeaderboard(IntPtr JHandle) : base(JHandle) { }
        public JLeaderboard(int JHandle) : base(JHandle) { }
    }

    public class JMultiboard : JAgent
    {
        public JMultiboard() : base() { }
        public JMultiboard(IntPtr JHandle) : base(JHandle) { }
        public JMultiboard(int JHandle) : base(JHandle) { }
    }

    public class JMultiboardItem : JAgent
    {
        public JMultiboardItem() : base() { }
        public JMultiboardItem(IntPtr JHandle) : base(JHandle) { }
        public JMultiboardItem(int JHandle) : base(JHandle) { }
    }

    public class JTrackable : JAgent
    {
        public JTrackable() : base() { }
        public JTrackable(IntPtr JHandle) : base(JHandle) { }
        public JTrackable(int JHandle) : base(JHandle) { }
    }

    public class JGameCache : JAgent
    {
        public JGameCache() : base() { }
        public JGameCache(IntPtr JHandle) : base(JHandle) { }
        public JGameCache(int JHandle) : base(JHandle) { }
    }

    public class JVersion : JHandle
    {
        public JVersion() : base() { }
        public JVersion(IntPtr JHandle) : base(JHandle) { }
        public JVersion(int JHandle) : base(JHandle) { }
    }

    public class JItemType : JHandle
    {
        public JItemType() : base() { }
        public JItemType(IntPtr JHandle) : base(JHandle) { }
        public JItemType(int JHandle) : base(JHandle) { }
    }

    public class JTextTag : JHandle
    {
        public JTextTag() : base() { }
        public JTextTag(IntPtr JHandle) : base(JHandle) { }
        public JTextTag(int JHandle) : base(JHandle) { }
    }

    public class JAttackType : JHandle
    {
        public JAttackType() : base() { }
        public JAttackType(IntPtr JHandle) : base(JHandle) { }
        public JAttackType(int JHandle) : base(JHandle) { }
    }

    public class JDamageType : JHandle
    {
        public JDamageType() : base() { }
        public JDamageType(IntPtr JHandle) : base(JHandle) { }
        public JDamageType(int JHandle) : base(JHandle) { }
    }

    public class JWeaponType : JHandle
    {
        public JWeaponType() : base() { }
        public JWeaponType(IntPtr JHandle) : base(JHandle) { }
        public JWeaponType(int JHandle) : base(JHandle) { }
    }

    public class JSoundType : JHandle
    {
        public JSoundType() : base() { }
        public JSoundType(IntPtr JHandle) : base(JHandle) { }
        public JSoundType(int JHandle) : base(JHandle) { }
    }

    public class JLightning : JHandle
    {
        public JLightning() : base() { }
        public JLightning(IntPtr JHandle) : base(JHandle) { }
        public JLightning(int JHandle) : base(JHandle) { }
    }

    public class JPathingType : JHandle
    {
        public JPathingType() : base() { }
        public JPathingType(IntPtr JHandle) : base(JHandle) { }
        public JPathingType(int JHandle) : base(JHandle) { }
    }

    public class JImage : JHandle
    {
        public JImage() : base() { }
        public JImage(IntPtr JHandle) : base(JHandle) { }
        public JImage(int JHandle) : base(JHandle) { }
    }

    public class JUbersplat : JHandle
    {
        public JUbersplat() : base() { }
        public JUbersplat(IntPtr JHandle) : base(JHandle) { }
        public JUbersplat(int JHandle) : base(JHandle) { }
    }

    public class JHashtable : JAgent
    {
        public JHashtable() : base() { }
        public JHashtable(IntPtr JHandle) : base(JHandle) { }
        public JHashtable(int JHandle) : base(JHandle) { }
    }
}

