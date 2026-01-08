using Serilog;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        public void Clear()
        {
            File.Delete(Config.War3 + "/dz_w3_plugin.ini");
            Log.Information("清理魔兽存档完毕");
            Directory.Delete(Temp);
            Log.Information("清理魔兽存档完毕");

        }
    }
}
