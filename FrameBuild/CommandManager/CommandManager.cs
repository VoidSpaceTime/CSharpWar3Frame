using War3Frame.Library;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        public ConfigPath Config { get; set; }
        public string ProjectName { get; set; }
        public string Vendor { get; set; }
        public string Temp { get; set; }
        public string Projects { get; set; }
        public string Template { get; set; }
        public BuildModeEnum BuildMode { get; set; }
        public string BuildDstPath { get; set; } // 构建项目目录
        public Assets AssetsPathConfig { get; set; }
        public bool IsSkip { get; set; }
        public CommandManager(ConfigPath configPath, string project, BuildModeEnum buildMode = BuildModeEnum.Test)
        {
            Config = configPath;
            ProjectName = project;
            Vendor = Path.Combine(Config.Pwd, "Vendor");
            Projects = Path.Combine(Config.Pwd, "Projects");
            Temp = Path.Combine(Config.Pwd, ".temp");
            BuildMode = buildMode;
            BuildDstPath = Path.Combine(Temp, BuildMode.ToString(), ProjectName);
            IsSkip = false;
        }
    }
}
