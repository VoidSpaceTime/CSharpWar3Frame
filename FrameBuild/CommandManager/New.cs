using Serilog;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {

        /*
         * 创建一个类库工程
         * 将Template脚本文件复制到指定目录
         * 将w3x文件复制到指定目录
         */
        public bool New()
        {
            if (Directory.Exists(Config.Pwd + "/Projects") is not true)
            {
                Directory.CreateDirectory(Config.Pwd + "/Projects");
            }
            if (ProjectName.StartsWith("_"))
            {
                Log.Error("项目名不合法(下划线“_”开始的名称已被禁用)"); return false;
            }
            if (ProjectName is "")
            {
                Log.Error("不允许存在空名称项目"); return false;
            }
            var projectPath =PwdProject;
            var damoDir = Path.Combine(Projects, "demo");
            if (Directory.Exists(projectPath))
            {
                Log.Error($"项目已存在,请使用run {ProjectName} 命令直接测试"); return false;
            }
            else if (Directory.Exists(damoDir))
            {
                Directory.CreateDirectory(projectPath);
                DirectoryExtensions.CopyDir(damoDir, projectPath);
                File.Move(Path.Combine(projectPath, "demo.csproj"), Path.Combine(projectPath, $"{ProjectName}.csproj"));
            }
            else
            {
                Log.Error("未找到模板项目，请确保Projects/demo目录存在");
                return false;
            }
            // 生成备份w3x目录

            Log.Information($"项目 {ProjectName} 创建成功!");
            Log.Information($"你可以输入 we {ProjectName} 编辑地图信息");
            Log.Information($"你可以输入 run {ProjectName} 直接调试");
            return true;
        }
    }
}
