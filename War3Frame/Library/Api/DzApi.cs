using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Frame;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace War3Frame.Library.Api
{
    public static class DzApi
    {

        public static void KKApiTriggerRegisterBackendLogicDelete(trigger trig)
        {
            DzTriggerRegisterSyncData(trig, "DZBLD", true);
        }
    }
}
