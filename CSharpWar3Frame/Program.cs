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

        [Verb("multi", HelpText = "多开")]
        class MultiOptions
        {
            [Value(0, Default = 2, Required = false, HelpText = "额外启动的客户端数量，必须大于0")]
            public int Count { get; set; } = 2;
        }

        static async Task<int> Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ApplicationBuilderExtensions.LogRegister();
            try
            {
                if (!ApplicationBuilderExtensions.ConfigLoad(out var config)) return 1;
                return await Parser.Default.ParseArguments<RunOptions, WeOptions, NewOptions, MultiOptions>(args)
                    .MapResult(
                        async (RunOptions options) => await new CommandManager(config, options.ProjectName, options.CurrentBuildMode)
                            .Run(options.CacheBuild, options.NoRunTests) ? 0 : 1,
                        (WeOptions options) => Task.FromResult(new CommandManager(config, options.ProjectName).WE() ? 0 : 1),
                        (NewOptions options) => Task.FromResult(new CommandManager(config, options.ProjectName).New() ? 0 : 1),
                        async (MultiOptions options) => await new CommandManager(config, "").LaunchAdditionalAsync(options.Count) ? 0 : 1,
                        errors => Task.FromResult(errors.Any(e => e is HelpRequestedError or HelpVerbRequestedError or VersionRequestedError) ? 0 : 1));
            }
            catch (Exception exception)
            {
                Log.Error(exception, "命令失败: {Message}", exception.Message);
                return 1;
            }
        }
    }
}