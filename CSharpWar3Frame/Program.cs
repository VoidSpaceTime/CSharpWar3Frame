using Serilog;

// 或者
using System.CommandLine;
using War3FrameBuild;
using War3FrameBuild.CommandManager;

namespace CSharpWar3FrameConsole
{
    internal class Program
    {
        public static ConfigPath PathConfig { get; set; }
        public static CommandManager CommandManager { get; set; }

        static async Task Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // 日志和配置初始化
            ApplicationBuilderExtensions.LogRegister();
            var configFlag = ApplicationBuilderExtensions.ConfigLoad(out var pathConfig);
            if (!configFlag && pathConfig != null)
            {
                return;
            }

            PathConfig = pathConfig;
            CommandManager = new CommandManager(PathConfig, args[1]); // 项目名后续传入
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
            // 为每个模式创建 Option<bool>，注册三种别名：-x, -x`, -x!
            //var optLocal = new Option<bool>(aliases: new[] { "-l", "-l`", "-l!" }, name: "本地模式");
            var optDebug = new Option<bool>(aliases: new[] { "-t", "-t`", "-t!" }, name: "调试模式");
            var optBuild = new Option<bool>(aliases: new[] { "-b", "-b`", "-b!" }, name: "构建模式");
            //var optDist = new Option<bool>(aliases: new[] { "-d", "-d`", "-d!" }, name: "发行模式");
            var optProd = new Option<bool>(aliases: new[] { "-r", "-r`", "-r!" }, name: "成品模式");

            run.Add(projectArg);
            rootCommand.Subcommands.Add(run);
            //run.Options.Add(optLocal);
            run.Options.Add(optDebug);
            run.Options.Add(optBuild);
            //run.Options.Add(optDist);
            run.Options.Add(optProd);
            // 设置处理函数
            run.SetAction(async parseResult =>
            {
                var project = parseResult.GetValue(projectArg);
                // 查找哪个模式选项被指定（如果用户指定多个模式，视为错误）
                var modeResults = new[]
                {
                new { Name = BuildModeEnum.Test,  Option = optDebug },
                new { Name = BuildModeEnum.Build,  Option = optBuild },
                new { Name = BuildModeEnum.Release,Option = optProd }
                }
                .Select(m => new
                {
                    m.Name,
                    Result = parseResult.GetResult(m.Option),
                })
                .Where(x => x.Result.IdentifierToken != null)
                .ToArray();

                BuildModeEnum selectedMode;
                bool isCache = false;
                bool isSemi = false;
                if (modeResults.Length == 0)
                {
                    // 默认本地模式
                    selectedMode = BuildModeEnum.Build;
                }
                else
                {
                    selectedMode = modeResults.First().Name;
                    // 读取实际使用的 token 来判断是否带 ~ 或 !
                    var tokens = modeResults.First().Result.IdentifierToken.Value;
                    if (modeResults.Length > 0)
                    {

                        if (tokens.TrimEnd().Contains("`")) isCache = true;
                        if (tokens.TrimEnd().Contains("!")) isSemi = true;
                    }
                }

                if (modeResults.Length > 1)
                {
                    Log.Error("错误：不能同时指定多个模式（例如 -l 与 -t 等不能一起使用）。");
                    return;
                }

                CommandManager.BuildMode = selectedMode;
                CommandManager.BuildDstPath = Path.Combine(CommandManager.Temp, selectedMode.ToString(), project);
                await CommandManager.Run(isCache, isSemi);


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
                await CommandManager.WE();
            });
            weCommand.Add(projectArg);
            rootCommand.Subcommands.Add(weCommand);
        }
    }
}