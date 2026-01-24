using War3Frame;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        public ConfigPath Config { get; set; }
        public string ProjectName { get; set; }
        public string Vendor { get; set; }
        /// <summary>
        /// 临时目录 Temp->
        /// </summary>
        public string Temp { get; set; }
        public string TempBuildPath { get; set; }
        /// <summary>
        /// Projects目录 Pwd->Projects
        /// </summary>
        public string Projects { get; set; }
        public string TemplateDemo { get; set; }
        public string Template { get; set; }
        public string PwdProject { get; set; }
        public BuildModeEnum BuildMode { get; set; }

        public War3Sounds War3SoundsYaml { get; set; }
        /// <summary>
        /// 构建项目目录 Temp->Mode->Project
        /// </summary>
        public string BuildDstPath { get; set; }
        //public Assets AssetsPathConfig { get; set; }
        public bool IsSkip { get; set; }
        public CommandManager(ConfigPath configPath, string project, BuildModeEnum buildMode = BuildModeEnum.Test)
        {
            Config = configPath;
            ProjectName = project;
            Vendor = Path.Combine(Config.Pwd, "Vendor");
            Projects = Path.Combine(Config.Pwd, "Projects");
            TemplateDemo = Path.Combine(Projects, "demo");
            Template = Path.Combine(Config.Pwd, "FrameBuild", "Template");
            Temp = Path.Combine(Config.Pwd, ".temp");
            PwdProject = Path.Combine(Projects, ProjectName);
            TempBuildPath = Path.Combine(Temp, ProjectName);
            BuildMode = buildMode;
            BuildDstPath = Path.Combine(Temp, BuildMode.ToString(), ProjectName);
            IsSkip = false;
        }

        public class War3Sounds
        {
            public List<SoundItem> sounds { get; set; }
        }

        public class SoundItem
        {
            public string path { get; set; }
            public int duration { get; set; }
        }
    }
}
