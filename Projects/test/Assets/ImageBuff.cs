using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace War3Frame
{
    public partial class AssetsInit
    {
        public static void ImageBuffInit(Library.Assets assets)
        {

            assets.AddImage
                ("icon/ability/Invulnerable");
            assets.AddImage("icon/ability/SigntDay", "别名");

        }
    }
}
