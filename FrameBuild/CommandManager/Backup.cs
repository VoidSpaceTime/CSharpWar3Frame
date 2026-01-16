using Serilog;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        //  生成备份w3x目录 temp -> project
        public void Backup()
        {
            var tempDir = Path.Combine(Config.Pwd, ".temp", ProjectName);
            var w3xDir = Path.Combine(Projects, ProjectName, "w3x", "map");
            if (Directory.GetLastWriteTime(Path.Combine(tempDir, "map")) > Directory.GetLastWriteTime(w3xDir))
            {
                Directory.Delete(w3xDir, true);
                DirectoryExtensions.CopyDir(Path.Combine(tempDir, "map"), w3xDir);
                Log.Information("备份完成[.tmp(地图备份)->w3x/map]");
            }
            var war3mapMap = Path.Combine(PwdProject, "w3x", "war3mapMap.blp");
            if (File.Exists(war3mapMap) is false)
            {
                File.Copy(Path.Combine(Template, "w3x", "war3mapMap.blp"), war3mapMap);
                Log.Information("修正备份[lni(war3mapMap)->w3x / war3mapMap]");
            }
            if (File.GetLastWriteTime(Path.Combine(tempDir, "resource", "war3mapMap.blp")) > File.GetLastWriteTime(war3mapMap))
            {
                File.Delete(war3mapMap);
                File.Copy(Path.Combine(tempDir, "resource", "war3mapMap.blp"), war3mapMap);
                Log.Information("更新同步[.tmp(war3mapMap)->w3x/war3mapMap]");
            }
            var tableDir = Path.Combine(Projects, ProjectName, "w3x", "table");
            if (Directory.GetLastWriteTime(Path.Combine(tempDir, "table")) > Directory.GetLastWriteTime(tableDir))
            {
                Directory.Delete(tableDir, true);
                DirectoryExtensions.CopyDir(Path.Combine(tempDir, "table"), tableDir);
                Log.Information("同步完成[.tmp(原生物编)->w3x/table]");
            }
        }
    }
}
