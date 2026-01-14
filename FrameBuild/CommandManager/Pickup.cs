using Serilog;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        //生成临时资源区 Project -> temp
        public void Pickup()
        {
            var tempDir = Path.Combine(Config.Pwd, ".temp", ProjectName);
            var w3xDir = Path.Combine(Projects, ProjectName, "w3x");
            if (Directory.Exists(Path.Combine(w3xDir, "table")) is false)
            {
                Directory.CreateDirectory(Path.Combine(w3xDir, "table"));
            }
            // 复制project的w3x文件
            if (Directory.Exists(tempDir) is false)
            {
                //DirectoryExtensions.CopyDir(Path.Combine(w3xDir, "map"), Path.Combine(tempDir, "map"));
                //Log.Debug("构建临时区[w3x(map)->map]");
                DirectoryExtensions.CopyDir(Path.Combine(w3xDir, "table"), Path.Combine(tempDir, "table"));
                Log.Debug("构建临时区[w3x(table)->table]");
                DirectoryExtensions.CopyDir(Path.Combine(Template, "lni", "w3x2lni"), Path.Combine(tempDir, "w3x2lni"));
                Log.Debug("构建临时区[lni(w3x2lni)->w3x2lni]");
                File.Copy(Path.Combine(Template, "lni", "x.w3x"), Path.Combine(tempDir, "x.w3x"));
                Log.Debug("构建临时区[lni(.w3x)->.w3x]");
            }
            if (Directory.Exists(Path.Combine(tempDir, "map")))
                Directory.Delete(Path.Combine(tempDir, "map"), true);
            DirectoryExtensions.CopyDir(Path.Combine(w3xDir, "map"), Path.Combine(tempDir, "map"));
            // map
            var war3mapMap = Path.Combine(PwdProject, "w3x", "war3mapMap.blp");
            if (File.Exists(Path.Combine(tempDir, "map", "war3mapMap.blp")))
                File.Delete(Path.Combine(tempDir, "map", "war3mapMap.blp"));
            // 物编ini判定
            var tableDir = Path.Combine(w3xDir, "table");
            Log.Information("覆盖同步[w3x(map)->map]");
            if (Directory.GetLastWriteTime(tableDir) > Directory.GetLastWriteTime(Path.Combine(tempDir, "table")))
            {
                Directory.Delete(tableDir, true);
                DirectoryExtensions.CopyDir(tableDir, Path.Combine(tempDir, "table"));
                Log.Information("更新同步[w3x(table)->table]");
            }
            // 资源判定
            if (Directory.Exists(Path.Combine(tempDir, "resource")))
                Directory.Delete(Path.Combine(tempDir, "resource"), true);
            DirectoryExtensions.CopyDir(Path.Combine(Template, "lni", "resource"), Path.Combine(tempDir, "resource"));
            Log.Information("覆盖同步[lni(resource)->resource]");
            // 小地图判定
            if (File.GetLastWriteTime(war3mapMap) > File.GetLastWriteTime(Path.Combine(tempDir, "resource", "war3mapMap.blp")))
            {
                File.Delete(Path.Combine(tempDir, "resource", "war3mapMap.blp"));
                File.Copy(war3mapMap, Path.Combine(tempDir, "resource", "war3mapMap.blp"));
                Log.Information("更新同步[.tmp(war3mapMap)->w3x/war3mapMap]");
            }
        }
    }
}
