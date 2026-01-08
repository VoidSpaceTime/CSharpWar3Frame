
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace War3FrameBuild
{
    public static class ApplicationBuilderExtensions
    {
        public static void LogRegister()
        {
            var customTheme = new SystemConsoleTheme(new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
            {
                [ConsoleThemeStyle.Text] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Gray },
                [ConsoleThemeStyle.LevelInformation] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Blue }, 
                [ConsoleThemeStyle.LevelWarning] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Yellow },
                [ConsoleThemeStyle.LevelError] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Red },
                [ConsoleThemeStyle.LevelDebug] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Magenta }, 
                [ConsoleThemeStyle.LevelVerbose] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Green }
            });

            // 注册日志
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.Console(theme: customTheme)
               .CreateLogger();

/*            Log.Information("这是信息日志（蓝色）");
            Log.Debug("这是Debug日志（洋红）");
            Log.Warning("这是警告日志（黄色）");
            Log.Error("这是错误日志（红色）");
            Log.Verbose("这是详述日志（绿色）");*/
        }
        public static bool ConfigLoad(out ConfigPath pathConfig)
        {
            var mainPath = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(mainPath, "appsettings.yml");
            var isExists = File.Exists(configPath);
            if (isExists)
            {
                var yamlFile = File.ReadAllText(configPath);
                var deserializer = new DeserializerBuilder()
               .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
               .Build();

                pathConfig = deserializer.Deserialize<ConfigPath>(yamlFile);
                if (!Directory.Exists(pathConfig.We) && File.Exists(pathConfig.We + "\\WE.exe"))
                {
                    Log.Warning($"WE工具目录或WE.exe文件不存在不存在，请配置WE工具目录");
                    return false;
                }
                if (!Directory.Exists(pathConfig.War3) && File.Exists(pathConfig.War3 + "\\War3.exe"))
                {
                    Log.Warning($"war3游戏目录或War3.exe文件不存在不存在，请配置war3目录");
                    return false;
                }
                if (!Directory.Exists(pathConfig.W3x2lni) && File.Exists(pathConfig.W3x2lni + "\\w2l.exe"))
                {
                    Log.Warning($"W3x2lni工具目录或w2l.exe文件不存在，请配置W3x2lni工具目录");
                    return false;
                }
                if (!Directory.Exists(pathConfig.Pwd))
                {
                    Log.Warning($"框架核心目录不存在，请配置框架核心目录");
                    return false;
                }
                if (!Directory.Exists(pathConfig.Assets))
                {
                    Log.Warning($"Assets目录不存在，请配置框架Assets目录");
                    return false;
                }
                Log.Information("配置文件加载成功");
                return true;
            }
            else
            {
                pathConfig = null;
                var yaml = @"
# 魔兽争霸3客户端文件路径
# Warcraft 3 client file path
war3: ""D:/Game/war3/war3Dev""

# 框架根目录（设置空则自动定为exe工具执行所在目录）
# Framework root directory (set empty to automatically specify the directory where the exe tool is executed)
pwd: ""G:/CSharp/CSharpWar3Frame""

# WE工具目录
# WE tool catalogue
we: ""G:/CSharp/CSharpWar3Frame/Vendor/WE""

# w3x2lni工具目录
# w3x2lni tools directory
w3x2lni: ""G:/CSharp/CSharpWar3Frame/Vendor/w3x2lni""

# assets资源目录
# assets resource directory
assets: ""./assets""
";
                using (File.Create(configPath)) ;
                File.WriteAllText(configPath, yaml);
                Log.Warning($"配置文件不存在，已创建默认配置文件，请根据注释修改配置后重新运行，路径: {configPath}");
            }
            return isExists;
        }

    }
}
