// 或者

using System.Diagnostics;
using CommandLine;
using Friflo.Json.Burst;
using Serilog;
using War3FrameBuild;
using War3FrameBuild.CommandManager;

namespace CSharpWar3FrameConsole
{
    internal static class Program
    {
        public static ConfigPath PathConfig { get; set; }
        public static CommandManager CommandManager { get; set; }
        [Verb("run", HelpText = "运行项目")]
        class RunOptions
        {
            // 1. 位置参数
            [Value(0, MetaName = "ProjectName", Required = true, HelpText = "项目名称")]
            public string ProjectName { get; set; }

            // --- 互斥参数组 ---

            // 选项 A: -b / --build (归属于 "DebugSet" 组)
            [Option('b', "build", SetName = "DebugSet", HelpText = "构建项目 (Debug模式)")]
            public bool BuildDebug { get; set; }

            // 选项 B: -r / --release (归属于 "ReleaseSet" 组)
            // 这里的 SetName 必须和上面不同，解析器才会认为它们是互斥的
            [Option('r', "release", SetName = "ReleaseSet", HelpText = "构建项目 (Release模式)")]
            public bool BuildRelease { get; set; }

            // --- 通用参数 (不设置 SetName，则两个组都能用) ---

            // -t / --test
            [Option('n', "noTest", HelpText = "关闭测试")]
            public bool NoRunTests { get; set; }
            [Option('c', "cache", HelpText = "启用缓存构建")]
            public bool CacheBuild { get; set; }

            // --- 辅助属性：将两个 Bool 转换成一个 Enum (推荐做法) ---
            // 这样你在业务逻辑里就不用写 if(b) else if(r) 了
            public BuildModeEnum CurrentBuildMode
            {
                get
                {
                    if (BuildRelease) return BuildModeEnum.Release;
                    if (BuildDebug) return BuildModeEnum.Build;
                    return BuildModeEnum.Build;
                }
            }
        }
        [Verb("we", HelpText = "WE编辑运行项目")]
        class WeOptions
        {
            [Value(0, MetaName = "ProjectName", Required = true, HelpText = "项目名称")]
            public string ProjectName { get; set; }

        }
        [Verb("new", HelpText = "新建项目")]
        class NewOptions
        {
            [Value(0, MetaName = "ProjectName", Required = true, HelpText = "项目名称")]
            public string ProjectName { get; set; }
        }
        [Verb("multi", HelpText = "新建项目")]
        class MultiOptions
        {
            [Value(0, Default = 2, Required = false, HelpText = "项目名称")]
            public int Count { get; set; } = 2;
        }
        async static Task Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // 日志和配置初始化
            ApplicationBuilderExtensions.LogRegister();
            var configFlag = ApplicationBuilderExtensions.ConfigLoad(out var pathConfig);
            if (!configFlag && pathConfig is null)
            {
                return;
            }




            // 关键点：泛型里填入 <RunOptions, WeOptions>
            // 库会根据用户输入的第一个词（run 或 we）自动匹配到对应的类
            Parser.Default.ParseArguments<RunOptions, WeOptions, NewOptions, MultiOptions>(args)
                .WithParsed<RunOptions>(async opts =>
                {
                    CommandManager = new CommandManager(pathConfig, opts.ProjectName, opts.CurrentBuildMode); // 项目名后续传入
                    await RunCommand(opts);
                }) // 如果匹配到 run
                .WithParsed<WeOptions>(async opts =>
                {
                    CommandManager = new CommandManager(pathConfig, opts.ProjectName); // 项目名后续传入
                    await WeCommand(opts);
                })   // 如果匹配到 we
                .WithParsed<NewOptions>(async opts =>
                {
                    CommandManager = new CommandManager(pathConfig, opts.ProjectName); // 项目名后续传入
                    await NewCommand(opts);
                })   // 如果匹配到 we
                .WithParsed<MultiOptions>(ops =>
                {
                    CommandManager = new CommandManager(pathConfig, ""); // 项目名后续传入
                    MultiCommand(ops);
                })
                .WithNotParsed((e) => throw new Exception("无匹配命令:" + e.ToString()));                     // 如果都没匹配上
        }


        private static async Task NewCommand(NewOptions options)
        {
            CommandManager.New();
            //Log.Verbose($"新建项目: {demo}");
        }

        private static async Task RunCommand(RunOptions options)
        {
            await CommandManager.Run(options.CacheBuild, options.NoRunTests);
            Console.WriteLine($"运行项目: {options.ProjectName}");

        }

        private static async Task WeCommand(WeOptions options)
        {
            CommandManager.ProjectName = options.ProjectName;
            await CommandManager.WE();

        }
        static void MultiCommand(MultiOptions options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                Log.Information("启动魔兽争霸III");
                var path = Path.Combine(CommandManager.Config.We, "bin", "YDWEConfig.exe");
                var psi = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = false,
                };
                psi.ArgumentList.Add("-launchwar3");
                Task.Delay(1000);
                //var bo3 = File.Exists(Path.Combine(Config.We, "bin", "WEConfig.exe"));

                using var war3Psi = Process.Start(psi);

                var war3Count = Process.GetProcesses()
                           .Count(p => string.Equals(p.ProcessName, "war3", StringComparison.OrdinalIgnoreCase));
                Log.Information($"当前魔兽争霸III进程数量: {war3Count}");
                if (i < war3Count)
                {
                    i--;
                }
            }
        }
    }
}