using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame.Library.Api
{
    public class jhandle
    {
        public IntPtr handle { get; protected set; }

        public jhandle()
        {
            handle = IntPtr.Zero;
        }

        public jhandle(IntPtr handle)
        {
            this.handle = handle;
        }

        public jhandle(int handle)
        {
            this.handle = new IntPtr(handle);
        }

        public static explicit operator bool(jhandle h)
        {
            return h?.handle != IntPtr.Zero;
        }
    }

    public class jagent : jhandle
    {
        public jagent() : base() { }
        public jagent(IntPtr handle) : base(handle) { }
        public jagent(int handle) : base(handle) { }
    }

    public class jevent : jagent
    {
        public jevent() : base() { }
        public jevent(IntPtr handle) : base(handle) { }
        public jevent(int handle) : base(handle) { }
    }

    public class jplayer : jagent
    {
        public jplayer() : base() { }
        public jplayer(IntPtr handle) : base(handle) { }
        public jplayer(int handle) : base(handle) { }
    }

    public class jwidget : jagent
    {
        public jwidget() : base() { }
        public jwidget(IntPtr handle) : base(handle) { }
        public jwidget(int handle) : base(handle) { }
    }

    public class junit : jwidget
    {
        public junit() : base() { }
        public junit(IntPtr handle) : base(handle) { }
        public junit(int handle) : base(handle) { }
    }

    public class jdestructable : jwidget
    {
        public jdestructable() : base() { }
        public jdestructable(IntPtr handle) : base(handle) { }
        public jdestructable(int handle) : base(handle) { }
    }

    public class jitem : jwidget
    {
        public jitem() : base() { }
        public jitem(IntPtr handle) : base(handle) { }
        public jitem(int handle) : base(handle) { }
    }

    public class jability : jagent
    {
        public jability() : base() { }
        public jability(IntPtr handle) : base(handle) { }
        public jability(int handle) : base(handle) { }
    }

    public class jbuff : jability
    {
        public jbuff() : base() { }
        public jbuff(IntPtr handle) : base(handle) { }
        public jbuff(int handle) : base(handle) { }
    }

    public class jforce : jagent
    {
        public jforce() : base() { }
        public jforce(IntPtr handle) : base(handle) { }
        public jforce(int handle) : base(handle) { }
    }

    public class jgroup : jagent
    {
        public jgroup() : base() { }
        public jgroup(IntPtr handle) : base(handle) { }
        public jgroup(int handle) : base(handle) { }
    }

    public class jtrigger : jagent
    {
        public jtrigger() : base() { }
        public jtrigger(IntPtr handle) : base(handle) { }
        public jtrigger(int handle) : base(handle) { }
    }

    public class jtriggercondition : jagent
    {
        public jtriggercondition() : base() { }
        public jtriggercondition(IntPtr handle) : base(handle) { }
        public jtriggercondition(int handle) : base(handle) { }
    }

    public class jtriggeraction : jhandle
    {
        public jtriggeraction() : base() { }
        public jtriggeraction(IntPtr handle) : base(handle) { }
        public jtriggeraction(int handle) : base(handle) { }
    }

    public class jtimer : jagent
    {
        public jtimer() : base() { }
        public jtimer(IntPtr handle) : base(handle) { }
        public jtimer(int handle) : base(handle) { }
    }

    public class jlocation : jagent
    {
        public jlocation() : base() { }
        public jlocation(IntPtr handle) : base(handle) { }
        public jlocation(int handle) : base(handle) { }
    }

    public class jregion : jagent
    {
        public jregion() : base() { }
        public jregion(IntPtr handle) : base(handle) { }
        public jregion(int handle) : base(handle) { }
    }

    public class jrect : jagent
    {
        public jrect() : base() { }
        public jrect(IntPtr handle) : base(handle) { }
        public jrect(int handle) : base(handle) { }
    }

    public class jboolexpr : jagent
    {
        public jboolexpr() : base() { }
        public jboolexpr(IntPtr handle) : base(handle) { }
        public jboolexpr(int handle) : base(handle) { }
    }

    public class jsound : jagent
    {
        public jsound() : base() { }
        public jsound(IntPtr handle) : base(handle) { }
        public jsound(int handle) : base(handle) { }
    }

    public class jconditionfunc : jboolexpr
    {
        public jconditionfunc() : base() { }
        public jconditionfunc(IntPtr handle) : base(handle) { }
        public jconditionfunc(int handle) : base(handle) { }
    }

    public class jfilterfunc : jboolexpr
    {
        public jfilterfunc() : base() { }
        public jfilterfunc(IntPtr handle) : base(handle) { }
        public jfilterfunc(int handle) : base(handle) { }
    }

    public class junitpool : jhandle
    {
        public junitpool() : base() { }
        public junitpool(IntPtr handle) : base(handle) { }
        public junitpool(int handle) : base(handle) { }
    }

    public class jitempool : jhandle
    {
        public jitempool() : base() { }
        public jitempool(IntPtr handle) : base(handle) { }
        public jitempool(int handle) : base(handle) { }
    }

    public class jrace : jhandle
    {
        public jrace() : base() { }
        public jrace(IntPtr handle) : base(handle) { }
        public jrace(int handle) : base(handle) { }
    }

    public class jalliancetype : jhandle
    {
        public jalliancetype() : base() { }
        public jalliancetype(IntPtr handle) : base(handle) { }
        public jalliancetype(int handle) : base(handle) { }
    }

    public class jracepreference : jhandle
    {
        public jracepreference() : base() { }
        public jracepreference(IntPtr handle) : base(handle) { }
        public jracepreference(int handle) : base(handle) { }
    }

    public class jgamestate : jhandle
    {
        public jgamestate() : base() { }
        public jgamestate(IntPtr handle) : base(handle) { }
        public jgamestate(int handle) : base(handle) { }
    }

    public class jigamestate : jgamestate
    {
        public jigamestate() : base() { }
        public jigamestate(IntPtr handle) : base(handle) { }
        public jigamestate(int handle) : base(handle) { }
    }

    public class jfgamestate : jgamestate
    {
        public jfgamestate() : base() { }
        public jfgamestate(IntPtr handle) : base(handle) { }
        public jfgamestate(int handle) : base(handle) { }
    }

    public class jplayerstate : jhandle
    {
        public jplayerstate() : base() { }
        public jplayerstate(IntPtr handle) : base(handle) { }
        public jplayerstate(int handle) : base(handle) { }
    }

    public class jplayerscore : jhandle
    {
        public jplayerscore() : base() { }
        public jplayerscore(IntPtr handle) : base(handle) { }
        public jplayerscore(int handle) : base(handle) { }
    }

    public class jplayergameresult : jhandle
    {
        public jplayergameresult() : base() { }
        public jplayergameresult(IntPtr handle) : base(handle) { }
        public jplayergameresult(int handle) : base(handle) { }
    }

    public class junitstate : jhandle
    {
        public junitstate() : base() { }
        public junitstate(IntPtr handle) : base(handle) { }
        public junitstate(int handle) : base(handle) { }
    }

    public class jaidifficulty : jhandle
    {
        public jaidifficulty() : base() { }
        public jaidifficulty(IntPtr handle) : base(handle) { }
        public jaidifficulty(int handle) : base(handle) { }
    }

    public class jeventid : jhandle
    {
        public jeventid() : base() { }
        public jeventid(IntPtr handle) : base(handle) { }
        public jeventid(int handle) : base(handle) { }
    }

    public class jgameevent : jeventid
    {
        public jgameevent() : base() { }
        public jgameevent(IntPtr handle) : base(handle) { }
        public jgameevent(int handle) : base(handle) { }
    }

    public class jplayerevent : jeventid
    {
        public jplayerevent() : base() { }
        public jplayerevent(IntPtr handle) : base(handle) { }
        public jplayerevent(int handle) : base(handle) { }
    }

    public class jplayerunitevent : jeventid
    {
        public jplayerunitevent() : base() { }
        public jplayerunitevent(IntPtr handle) : base(handle) { }
        public jplayerunitevent(int handle) : base(handle) { }
    }

    public class junitevent : jeventid
    {
        public junitevent() : base() { }
        public junitevent(IntPtr handle) : base(handle) { }
        public junitevent(int handle) : base(handle) { }
    }

    public class jlimitop : jeventid
    {
        public jlimitop() : base() { }
        public jlimitop(IntPtr handle) : base(handle) { }
        public jlimitop(int handle) : base(handle) { }
    }

    public class jwidgetevent : jeventid
    {
        public jwidgetevent() : base() { }
        public jwidgetevent(IntPtr handle) : base(handle) { }
        public jwidgetevent(int handle) : base(handle) { }
    }

    public class jdialogevent : jeventid
    {
        public jdialogevent() : base() { }
        public jdialogevent(IntPtr handle) : base(handle) { }
        public jdialogevent(int handle) : base(handle) { }
    }

    public class junittype : jhandle
    {
        public junittype() : base() { }
        public junittype(IntPtr handle) : base(handle) { }
        public junittype(int handle) : base(handle) { }
    }

    public class jgamespeed : jhandle
    {
        public jgamespeed() : base() { }
        public jgamespeed(IntPtr handle) : base(handle) { }
        public jgamespeed(int handle) : base(handle) { }
    }

    public class jgamedifficulty : jhandle
    {
        public jgamedifficulty() : base() { }
        public jgamedifficulty(IntPtr handle) : base(handle) { }
        public jgamedifficulty(int handle) : base(handle) { }
    }

    public class jgametype : jhandle
    {
        public jgametype() : base() { }
        public jgametype(IntPtr handle) : base(handle) { }
        public jgametype(int handle) : base(handle) { }
    }

    public class jmapflag : jhandle
    {
        public jmapflag() : base() { }
        public jmapflag(IntPtr handle) : base(handle) { }
        public jmapflag(int handle) : base(handle) { }
    }

    public class jmapvisibility : jhandle
    {
        public jmapvisibility() : base() { }
        public jmapvisibility(IntPtr handle) : base(handle) { }
        public jmapvisibility(int handle) : base(handle) { }
    }

    public class jmapsetting : jhandle
    {
        public jmapsetting() : base() { }
        public jmapsetting(IntPtr handle) : base(handle) { }
        public jmapsetting(int handle) : base(handle) { }
    }

    public class jmapdensity : jhandle
    {
        public jmapdensity() : base() { }
        public jmapdensity(IntPtr handle) : base(handle) { }
        public jmapdensity(int handle) : base(handle) { }
    }

    public class jmapcontrol : jhandle
    {
        public jmapcontrol() : base() { }
        public jmapcontrol(IntPtr handle) : base(handle) { }
        public jmapcontrol(int handle) : base(handle) { }
    }

    public class jplayerslotstate : jhandle
    {
        public jplayerslotstate() : base() { }
        public jplayerslotstate(IntPtr handle) : base(handle) { }
        public jplayerslotstate(int handle) : base(handle) { }
    }

    public class jvolumegroup : jhandle
    {
        public jvolumegroup() : base() { }
        public jvolumegroup(IntPtr handle) : base(handle) { }
        public jvolumegroup(int handle) : base(handle) { }
    }

    public class jcamerafield : jhandle
    {
        public jcamerafield() : base() { }
        public jcamerafield(IntPtr handle) : base(handle) { }
        public jcamerafield(int handle) : base(handle) { }
    }

    public class jcamerasetup : jhandle
    {
        public jcamerasetup() : base() { }
        public jcamerasetup(IntPtr handle) : base(handle) { }
        public jcamerasetup(int handle) : base(handle) { }
    }

    public class jplayercolor : jhandle
    {
        public jplayercolor() : base() { }
        public jplayercolor(IntPtr handle) : base(handle) { }
        public jplayercolor(int handle) : base(handle) { }
    }

    public class jplacement : jhandle
    {
        public jplacement() : base() { }
        public jplacement(IntPtr handle) : base(handle) { }
        public jplacement(int handle) : base(handle) { }
    }

    public class jstartlocprio : jhandle
    {
        public jstartlocprio() : base() { }
        public jstartlocprio(IntPtr handle) : base(handle) { }
        public jstartlocprio(int handle) : base(handle) { }
    }

    public class jraritycontrol : jhandle
    {
        public jraritycontrol() : base() { }
        public jraritycontrol(IntPtr handle) : base(handle) { }
        public jraritycontrol(int handle) : base(handle) { }
    }

    public class jblendmode : jhandle
    {
        public jblendmode() : base() { }
        public jblendmode(IntPtr handle) : base(handle) { }
        public jblendmode(int handle) : base(handle) { }
    }

    public class jtexmapflags : jhandle
    {
        public jtexmapflags() : base() { }
        public jtexmapflags(IntPtr handle) : base(handle) { }
        public jtexmapflags(int handle) : base(handle) { }
    }

    public class jeffect : jagent
    {
        public jeffect() : base() { }
        public jeffect(IntPtr handle) : base(handle) { }
        public jeffect(int handle) : base(handle) { }
    }

    public class jeffecttype : jhandle
    {
        public jeffecttype() : base() { }
        public jeffecttype(IntPtr handle) : base(handle) { }
        public jeffecttype(int handle) : base(handle) { }
    }

    public class jweathereffect : jhandle
    {
        public jweathereffect() : base() { }
        public jweathereffect(IntPtr handle) : base(handle) { }
        public jweathereffect(int handle) : base(handle) { }
    }

    public class jterraindeformation : jhandle
    {
        public jterraindeformation() : base() { }
        public jterraindeformation(IntPtr handle) : base(handle) { }
        public jterraindeformation(int handle) : base(handle) { }
    }

    public class jfogstate : jhandle
    {
        public jfogstate() : base() { }
        public jfogstate(IntPtr handle) : base(handle) { }
        public jfogstate(int handle) : base(handle) { }
    }

    public class jfogmodifier : jagent
    {
        public jfogmodifier() : base() { }
        public jfogmodifier(IntPtr handle) : base(handle) { }
        public jfogmodifier(int handle) : base(handle) { }
    }

    public class jdialog : jagent
    {
        public jdialog() : base() { }
        public jdialog(IntPtr handle) : base(handle) { }
        public jdialog(int handle) : base(handle) { }
    }

    public class jbutton : jagent
    {
        public jbutton() : base() { }
        public jbutton(IntPtr handle) : base(handle) { }
        public jbutton(int handle) : base(handle) { }
    }

    public class jquest : jagent
    {
        public jquest() : base() { }
        public jquest(IntPtr handle) : base(handle) { }
        public jquest(int handle) : base(handle) { }
    }

    public class jquestitem : jagent
    {
        public jquestitem() : base() { }
        public jquestitem(IntPtr handle) : base(handle) { }
        public jquestitem(int handle) : base(handle) { }
    }

    public class jdefeatcondition : jagent
    {
        public jdefeatcondition() : base() { }
        public jdefeatcondition(IntPtr handle) : base(handle) { }
        public jdefeatcondition(int handle) : base(handle) { }
    }

    public class jtimerdialog : jagent
    {
        public jtimerdialog() : base() { }
        public jtimerdialog(IntPtr handle) : base(handle) { }
        public jtimerdialog(int handle) : base(handle) { }
    }

    public class jleaderboard : jagent
    {
        public jleaderboard() : base() { }
        public jleaderboard(IntPtr handle) : base(handle) { }
        public jleaderboard(int handle) : base(handle) { }
    }

    public class jmultiboard : jagent
    {
        public jmultiboard() : base() { }
        public jmultiboard(IntPtr handle) : base(handle) { }
        public jmultiboard(int handle) : base(handle) { }
    }

    public class jmultiboarditem : jagent
    {
        public jmultiboarditem() : base() { }
        public jmultiboarditem(IntPtr handle) : base(handle) { }
        public jmultiboarditem(int handle) : base(handle) { }
    }

    public class jtrackable : jagent
    {
        public jtrackable() : base() { }
        public jtrackable(IntPtr handle) : base(handle) { }
        public jtrackable(int handle) : base(handle) { }
    }

    public class jgamecache : jagent
    {
        public jgamecache() : base() { }
        public jgamecache(IntPtr handle) : base(handle) { }
        public jgamecache(int handle) : base(handle) { }
    }

    public class jversion : jhandle
    {
        public jversion() : base() { }
        public jversion(IntPtr handle) : base(handle) { }
        public jversion(int handle) : base(handle) { }
    }

    public class jitemtype : jhandle
    {
        public jitemtype() : base() { }
        public jitemtype(IntPtr handle) : base(handle) { }
        public jitemtype(int handle) : base(handle) { }
    }

    public class jtexttag : jhandle
    {
        public jtexttag() : base() { }
        public jtexttag(IntPtr handle) : base(handle) { }
        public jtexttag(int handle) : base(handle) { }
    }

    public class jattacktype : jhandle
    {
        public jattacktype() : base() { }
        public jattacktype(IntPtr handle) : base(handle) { }
        public jattacktype(int handle) : base(handle) { }
    }

    public class jdamagetype : jhandle
    {
        public jdamagetype() : base() { }
        public jdamagetype(IntPtr handle) : base(handle) { }
        public jdamagetype(int handle) : base(handle) { }
    }

    public class jweapontype : jhandle
    {
        public jweapontype() : base() { }
        public jweapontype(IntPtr handle) : base(handle) { }
        public jweapontype(int handle) : base(handle) { }
    }

    public class jsoundtype : jhandle
    {
        public jsoundtype() : base() { }
        public jsoundtype(IntPtr handle) : base(handle) { }
        public jsoundtype(int handle) : base(handle) { }
    }

    public class jlightning : jhandle
    {
        public jlightning() : base() { }
        public jlightning(IntPtr handle) : base(handle) { }
        public jlightning(int handle) : base(handle) { }
    }

    public class jpathingtype : jhandle
    {
        public jpathingtype() : base() { }
        public jpathingtype(IntPtr handle) : base(handle) { }
        public jpathingtype(int handle) : base(handle) { }
    }

    public class jimage : jhandle
    {
        public jimage() : base() { }
        public jimage(IntPtr handle) : base(handle) { }
        public jimage(int handle) : base(handle) { }
    }

    public class jubersplat : jhandle
    {
        public jubersplat() : base() { }
        public jubersplat(IntPtr handle) : base(handle) { }
        public jubersplat(int handle) : base(handle) { }
    }

    public class jhashtable : jagent
    {
        public jhashtable() : base() { }
        public jhashtable(IntPtr handle) : base(handle) { }
        public jhashtable(int handle) : base(handle) { }
    }
}

