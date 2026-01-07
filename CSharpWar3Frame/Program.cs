using Microsoft.Extensions.Hosting;
using Serilog;

// 或者
using System.CommandLine;
using War3FrameBuild;
using War3FrameBuild.CommandManager; // 某些版本下扩展方法在此命名空间

namespace CSharpWar3FrameConsole
{
    internal class Program
    {
        public static ConfigPath PathConfig { get; set; }
        public static CommandManager CommandManager { get; set; }
        static async Task Main(string[] args)
        {
            // 日志和配置初始化
            ApplicationBuilderExtensions.LogRegister();
            var configFlag = ApplicationBuilderExtensions.ConfigLoad(out var pathConfig);
            if (!configFlag && pathConfig != null)
            {
                return;
            }
            PathConfig = pathConfig;

            // 命令注册
            CommendRegister(args);


        }
        private static void CommendRegister(string[] args)
        {
            // 命令解析调用
            var rootCommand = new RootCommand("lik CLI");

            NewCommand(rootCommand);
            RunCommand(rootCommand);
            WECommand(rootCommand);

            rootCommand.Parse(args).Invoke();
        }
        private static void NewCommand(RootCommand rootCommand)
        {
            var newCommand = new Command("new", "创建新项目");
            var projectArg = new Argument<string>("project");

            // 设置处理函数
            newCommand.SetAction(parseResult =>
            {
                var demo = parseResult.GetValue(projectArg);
                CommandManager.New();
                //Log.Verbose($"新建项目: {demo}");
            });
            newCommand.Add(projectArg);
            rootCommand.Subcommands.Add(newCommand);
        }
        private static void RunCommand(RootCommand rootCommand)
        {
            // run 子命令
            var run = new Command("run", "运行指定项目");
            var projectArg = new Argument<string>("project") { Description = "要运行的项目名" };
            // 为每个模式创建 Option<bool>，注册三种别名：-x, -x~, -x!
            var optLocal = new Option<bool>(aliases: new[] { "-l", "-l~", "-l!" }, name: "本地模式");
            var optDebug = new Option<bool>(aliases: new[] { "-t", "-t~", "-t!" }, name: "调试模式");
            var optBuild = new Option<bool>(aliases: new[] { "-b", "-b~", "-b!" }, name: "构建模式");
            var optDist = new Option<bool>(aliases: new[] { "-d", "-d~", "-d!" }, name: "发行模式");
            var optProd = new Option<bool>(aliases: new[] { "-r", "-r~", "-r!" }, name: "成品模式");

            run.Add(projectArg);
            rootCommand.Subcommands.Add(run);
            run.Options.Add(optLocal);
            run.Options.Add(optDebug);
            run.Options.Add(optBuild);
            run.Options.Add(optDist);
            run.Options.Add(optProd);
            // 设置处理函数
            run.SetAction(parseResult =>
            {
                var project = parseResult.GetValue(projectArg);
                CommandManager = new CommandManager(PathConfig, project); // 项目名后续传入
                // 查找哪个模式选项被指定（如果用户指定多个模式，视为错误）
                var modeResults = new[]
                {
                new { Name = BuildModeEnum.Local,  Option = optLocal },
                new { Name = BuildModeEnum.Test,  Option = optDebug },
                new { Name = BuildModeEnum.Build,  Option = optBuild },
                new { Name =BuildModeEnum.Dist,   Option = optDist },
                new { Name = BuildModeEnum.Release,Option = optProd }
            }
                .Select(m => new
                {
                    m.Name,
                    Result = parseResult.GetResult(m.Option),
                })
                .Where(x => x.Result != null)
                /*        .Select(m => new
                        {
                            m.Name,
                            m.Result,
                            isCache = m.Result.ToString().Contains("~"),
                            isSemi = m.Result.ToString().Contains("!"),
                        })*/
                .ToArray();

                BuildModeEnum selectedMode;
                bool isCache = false;
                bool isSemi = false;
                if (modeResults.Length == 0)
                {
                    // 默认本地模式
                    selectedMode = BuildModeEnum.Local;
                }
                else if (modeResults.Length > 1)
                {
                    Log.Error("错误：不能同时指定多个模式（例如 -l 与 -t 等不能一起使用）。");
                    return;
                }
                else
                {
                    selectedMode = modeResults[0].Name;
                    // 读取实际使用的 token 来判断是否带 ~ 或 !
                    var tokens = modeResults[0].Result.Tokens;
                    if (tokens != null && tokens.Count > 0)
                    {
                        var tokenValue = tokens[0].Value; // 例如 "-l~"
                        if (tokenValue.TrimEnd().EndsWith("~")) isCache = true;
                        else if (tokenValue.TrimEnd().EndsWith("!")) isSemi = true;
                    }
                    CommandManager.BuildMode = selectedMode;
                    CommandManager.Run(isCache, isSemi);
                }


                Console.WriteLine($"运行项目: {project}");
            });

        }
        private static void WECommand(RootCommand rootCommand)
        {
            var weCommand = new Command("we", "启动WE编辑器");
            var projectArg = new Argument<string>("project");
            // 设置处理函数
            weCommand.SetAction(async parseResult =>
            {
                var demo = parseResult.GetValue(projectArg);
                CommandManager = new CommandManager(PathConfig, demo); // 项目名后续传入
                await CommandManager.WE();

            });
            weCommand.Add(projectArg);
            rootCommand.Subcommands.Add(weCommand);
        }
    }
}
