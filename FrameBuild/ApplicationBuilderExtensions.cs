using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace War3FrameBuild;

public static class ApplicationBuilderExtensions
{
    public static void LogRegister()
    {
        var customTheme = new SystemConsoleTheme(new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
        {
            [ConsoleThemeStyle.Text] = new() { Foreground = ConsoleColor.Gray },
            [ConsoleThemeStyle.LevelInformation] = new() { Foreground = ConsoleColor.Blue },
            [ConsoleThemeStyle.LevelWarning] = new() { Foreground = ConsoleColor.Yellow },
            [ConsoleThemeStyle.LevelError] = new() { Foreground = ConsoleColor.Red },
            [ConsoleThemeStyle.LevelDebug] = new() { Foreground = ConsoleColor.Magenta },
            [ConsoleThemeStyle.LevelVerbose] = new() { Foreground = ConsoleColor.Green }
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
                .WithNamingConvention(UnderscoredNamingConvention.Instance) // see height_in_inches in sample yml 
                .Build();

            pathConfig = deserializer.Deserialize<ConfigPath>(yamlFile);
            if (!ValidateConfig(pathConfig, out var error))
            {
                Log.Error("配置无效: {Error}", error);
                return false;
            }
            Log.Information("配置文件加载成功");
            return true;
        }

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
        using (File.Create(configPath))
        {
            ;
        }

        File.WriteAllText(configPath, yaml);
        Log.Warning($"配置文件不存在，已创建默认配置文件，请根据注释修改配置后重新运行，路径: {configPath}");
        return isExists;
    }
    internal static bool ValidateConfig(ConfigPath? config, out string error)
    {
        error = "";
        if (config == null) { error = "配置内容为空"; return false; }
        if (!Directory.Exists(config.We) || !(File.Exists(Path.Combine(config.We, "WE.exe")) || File.Exists(Path.Combine(config.We, "KKWE.exe")))
            || !File.Exists(Path.Combine(config.We, "bin", "YDWEConfig.exe"))) error = "WE.exe/KKWE.exe 或 bin/YDWEConfig.exe 不存在";
        else if (!Directory.Exists(config.War3) || !File.Exists(Path.Combine(config.War3, "war3.exe"))) error = "war3.exe 不存在";
        else if (!Directory.Exists(config.W3x2lni) || !File.Exists(Path.Combine(config.W3x2lni, "w2l.exe"))) error = "w2l.exe 不存在";
        else if (!Directory.Exists(config.Pwd)) error = "框架目录不存在";
        else if (!Directory.Exists(config.Assets)) error = "资源目录不存在";
        return error.Length == 0;
    }
}