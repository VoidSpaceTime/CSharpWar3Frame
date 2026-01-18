using IniParser;
using IniParser.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using NAudio.Wave;
using Serilog;
using System.Reflection;
using War3Frame.Library;
using War3FrameBuild.Extension;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static War3Frame.Library.Assets;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        #region Build Methods
        public readonly Dictionary<string, (string Path, string Name, List<string> Extention)> AssetsTypes = new()
        {
            { "image",("war3mapImage","【图片】Image",new List<string>{ ".tga",".blp" }) },
            { "model",("war3mapModel","【模型】Model",new List<string>{ ".mdl",".mdx" }) },
            { "bgm",("war3mapBgm","【乐曲】Bgm",new List<string>{  ".mp3", ".wav"  }) },
            { "vcm",("war3mapVoice","【音效】Vcm",new List<string>{ ".mp3", ".wav" }) },
            { "v3d",("war3mapVoice","【音效】V3d",new List<string>{ ".mp3", ".wav"}) },
            { "vwp",("war3mapVwp","【音效】Vwp",new List<string>{ ".yaml" }) },
            { "vwp-voice",("war3mapVoice","【音效】Vwp",new List<string>{ ".mp3", ".wav" }) },
        };
        private class AssetsPickPathRewriter : CSharpSyntaxRewriter
        {
            private readonly CommandManager commandManager;
            public AssetsPickPathRewriter(CommandManager manager)
            {
                commandManager = manager;
            }

            // helper: 执行前已确保 argument 非 null
            // 统一提取参数字符串，去掉两边引号并保留单个反斜线
            // 例如："Sound\\Interface\\Error.wav" -> Sound\Interface\Error.wav
            // 或当表达式为字符串字面量时直接取其 Token.ValueText
            private string GetArgumentString(ArgumentSyntax arg)
            {
                var expr = arg.Expression;
                if (expr is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    return lit.Token.ValueText;
                }
                var s = arg.ToString();
                if (s.StartsWith("\"") && s.EndsWith("\""))
                {
                    s = s.Substring(1, s.Length - 2);
                }
                // 反斜线规范为单个
                s = s.Replace("\\\\", "\\");
                return s;
            }
            public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                if (node.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var flag = false;
                    var arguments = node.ArgumentList.Arguments.ToList();
                    // 构建新的参数列表
                    var newArguments = new List<ExpressionSyntax>();
                    if (arguments.Count >= 1)
                    {
                        switch (memberAccess.Name.Identifier.Text)
                        {
                            case "AddSelecation":
                                {
                                    var absDir = Path.Combine(commandManager.Config.Assets, "war3mapSelection", GetArgumentString(arguments[0]).Replace('/', '\\'));
                                    if (Directory.Exists(absDir))
                                    {
                                        DirectoryExtensions.CopyDir(absDir, Path.Combine(commandManager.BuildDstPath, "resource", "ReplaceableTextures", "Selection"));
                                        GetArgumentString(arguments[0]);
                                        flag = true;
                                    }
                                    else
                                    {
                                        Log.Warning($"【选择圈】组件 不存在：{absDir}");
                                        DirectoryExtensions.CopyDir(Path.Combine(commandManager.Template, "lni", "assets", "Selection"), Path.Combine(commandManager.BuildDstPath, "resource", "ReplaceableTextures", "Selection"));
                                        Log.Information("【选择圈】引入：Lni");
                                    }
                                    break;
                                }
                            case "AddTerrain":
                                {
                                    var dir = GetArgumentString(arguments[0]);
                                    if (dir != "")
                                    {
                                        var terrainDir = Path.Combine(commandManager.Config.Assets, "war3mapTerrain", dir);
                                        if (Directory.Exists(terrainDir))
                                        {
                                            bool flagT = true;
                                            var cliffDir = Path.Combine(terrainDir, "Cliff");
                                            if (Directory.Exists(cliffDir) is false)
                                            {
                                                Log.Error($"【地形贴图】组件：{dir} Cliff悬崖贴图丢失");
                                                flagT = false;
                                            }
                                            var terrainArtDir = Path.Combine(terrainDir, "TerrainArt");
                                            if (Directory.Exists(terrainArtDir) is false)
                                            {
                                                Log.Error($"【地形贴图】组件：{dir} TerrainArt地表贴图丢失");
                                                flagT = false;
                                            }
                                            if (flagT)
                                            {
                                                DirectoryExtensions.CopyDir(cliffDir, Path.Combine(commandManager.BuildDstPath, "resource", "ReplaceableTextures", "Cliff"));
                                                DirectoryExtensions.CopyDir(terrainArtDir, Path.Combine(commandManager.BuildDstPath, "resource", "TerrainArt"));
                                            }
                                            else
                                            {
                                                Log.Error($"【地形贴图】引入：{dir} 存在问题已中止");
                                            }
                                        }
                                        else
                                        {
                                            Log.Error($"【地形贴图】组件：{dir} 不存在");
                                        }
                                    }
                                    break;
                                }
                            case "AddFont":
                                {
                                    var file = GetArgumentString(arguments[0]);
                                    var ext = Path.GetExtension(file);
                                    if (ext is "")
                                    {
                                        file = Path.ChangeExtension(file, ".ttf");
                                    }
                                    else if (ext is not ".ttf")
                                    {
                                        Log.Warning($"【字体】文件不支持 {file} 格式");
                                        file = "default";
                                    }
                                    var fontFile = Path.Combine(commandManager.Config.Assets, "war3mapFont", file);
                                    if (File.Exists(fontFile) is false)
                                    {
                                        Log.Warning($"【字体】文件{fontFile} 不存在");
                                        file = "default";
                                    }
                                    Log.Information($"【字体】引入：{file}");
                                    if (file is "default")
                                    {
                                        File.Copy(Path.Combine(commandManager.Template, "lni", "assets", "ARHei2月液晶.ttf"), Path.Combine(commandManager.BuildDstPath, "map", "fonts.ttf"), true);
                                    }

                                    else
                                    {
                                        File.Copy(fontFile, Path.Combine(commandManager.BuildDstPath, "map", "fonts.ttf"), true);
                                    }
                                    break;
                                }
                            case "AddLoading":
                                {
                                    var directory = GetArgumentString(arguments[0]);
                                    File.Delete(Path.Combine(commandManager.BuildDstPath, "resource", "Framework", "LoadingScreen.mdx"));
                                    var loadingPath = Path.Combine(commandManager.Config.Assets, "war3MapLoading", directory);
                                    var loadingFile = Path.Combine(commandManager.Config.Assets, "war3MapLoading", directory + ".tga");
                                    bool loaded = false;
                                    if (Directory.Exists(loadingPath))
                                    {
                                        File.Copy(Path.Combine(commandManager.Template, "lni", "assets", "LoadingScreenDir.mdx"), Path.Combine(commandManager.BuildDstPath, "resource", "Framework", "LoadingScreen.mdx"), true);
                                        string[] loadingSites = new[] { "pic", "bc", "bg" };
                                        foreach (var s in loadingSites)
                                        {
                                            loadingFile = Path.Combine(loadingPath, s + ".tga");
                                            if (File.Exists(loadingFile))
                                            {
                                                File.Copy(loadingFile, Path.Combine(commandManager.BuildDstPath, "resource", "Framework", "LoadingScreen" + s + ".tga"), true);
                                                loaded = true;
                                            }
                                            else
                                            {
                                                Log.Information($"【载入图】引入拼图文件：{s}.不存在");
                                            }
                                            loaded = true;
                                        }
                                    }
                                    else if (File.Exists(loadingFile))
                                    {
                                        File.Copy(Path.Combine(commandManager.Template, "lni", "assets", "LoadingScreenFile.mdx"), Path.Combine(commandManager.BuildDstPath, "resource", "Framework", "LoadingScreen.mdx"), true);
                                        File.Copy(Path.Combine(loadingFile), Path.Combine(commandManager.BuildDstPath, "resource", "Framework", "LoadingScreen.tga"), true);
                                        loaded = true;
                                    }
                                    if (loaded)
                                    {
                                        var parser = new FileIniDataParser();
                                        var iniPath = Path.Combine(commandManager.BuildDstPath, "table", "w3i.ini");
                                        if (!File.Exists(iniPath))
                                        {
                                            Log.Error("【载入图】w3i.ini 文件不存在");
                                        }
                                        else
                                        {
                                            IniData data = parser.ReadFile(iniPath);
                                            data["载入图"]["路径"] = "Framework\\LoadingScreen.mdx";
                                            // 保存
                                            parser.WriteFile("config.ini", data);
                                        }
                                    }
                                    else
                                    {
                                        Log.Error("【载入图】套件：" + directory + " 不存在");
                                    }
                                    break;
                                }
                            case "AddPreview":
                                {
                                    var file = GetArgumentString(arguments[0]);
                                    if (file is "")
                                    {
                                        Log.Debug("【预览图】无资源引入");
                                    }
                                    else
                                    {
                                        if (Path.GetExtension(file) == "")
                                        {
                                            file = file + ".tga";
                                        }
                                        var previewFile = Path.Combine(commandManager.Config.Assets, "war3mapPreview", file);
                                        if (File.Exists(previewFile))
                                        {
                                            File.Copy(previewFile, Path.Combine(commandManager.BuildDstPath, "resource", "war3mapPreview.tga"), true);
                                            Log.Information($"【预览图】引入：{file}");
                                        }
                                    }
                                    break;
                                }
                            case "AddImage":
                                {
                                    var (status, isWar3, pickPath, sourcePath) = commandManager.AnalysisFile("image", GetArgumentString(arguments[0]), commandManager.IsSkip);
                                    newArguments.Add(arguments[0].Expression);
                                    if (status is true && arguments.Count < 3)
                                    {
                                        // 如果第二个参数为空字符串，则替换为 null
                                        if (arguments.Count > 1)
                                        {

                                            newArguments.Add(arguments[1].Expression);
                                        }
                                        else
                                        {
                                            newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
                                        }
                                        newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(pickPath)));
                                        flag = true;
                                    }
                                    break;
                                }
                            case "AddModel":
                                {
                                    var (status, isWar3, pickPath, sourcePath) = commandManager.AnalysisFile("model", GetArgumentString(arguments[0]), commandManager.IsSkip);
                                    pickPath = Path.ChangeExtension(pickPath, ".mdl");
                                    newArguments.Add(arguments[0].Expression);
                                    if (status is true && arguments.Count < 3)
                                    {
                                        // 如果第二个参数为空字符串，则替换为 null
                                        if (GetArgumentString(arguments[1]) is "")
                                        {
                                            newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
                                        }
                                        else
                                        {
                                            newArguments.Add(arguments[1].Expression);
                                        }
                                        newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(pickPath)));
                                        flag = true;
                                    }
                                    else
                                    {
                                        newArguments = arguments.Select(arg => arg.Expression).ToList();
                                    }
                                    break;
                                }
                            case "AddBGM":
                                {
                                    var (status, isWar3, pickPath, sourcePath) = commandManager.AnalysisFile("bgm", GetArgumentString(arguments[0]).Replace("\"", ""), commandManager.IsSkip);
                                    newArguments.Add(arguments[0].Expression);
                                    if (status is true && arguments.Count < 5)
                                    {
                                        int duration = 0;
                                        if (isWar3)
                                        {
                                            duration = commandManager.GetWar3AuidoDuration(pickPath);
                                        }
                                        else
                                        {
                                            duration = commandManager.GetAudioFileDuration(sourcePath);

                                        }
                                        // 如果第二个参数为空字符串，则替换为 null
                                        if (arguments.Count > 1 && GetArgumentString(arguments[1]) is "")
                                        {
                                            newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
                                        }
                                        else
                                        {
                                            newArguments.Add(arguments[1].Expression);
                                        }
                                        newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(pickPath)));
                                        newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(duration)));
                                        if (arguments.Count > 4)
                                        {
                                            newArguments.Add(arguments[4].Expression);
                                        }
                                        flag = true;
                                    }
                                    break;
                                }
                            case "AddVCM":
                                {
                                    var (status, isWar3, pickPath, sourcePath) = commandManager.AnalysisFile("vcm", GetArgumentString(arguments[0]), commandManager.IsSkip);
                                    newArguments.Add(arguments[0].Expression);
                                    if (status is true && arguments.Count < 5)
                                    {
                                        int duration = 0;
                                        if (isWar3)
                                        {
                                            duration = commandManager.GetWar3AuidoDuration(pickPath);
                                        }
                                        else
                                        {
                                            duration = commandManager.GetAudioFileDuration(sourcePath);

                                        }
                                        // 如果第二个参数为空字符串，则替换为 null
                                        if (arguments.Count > 1 && GetArgumentString(arguments[1]) is "")
                                        {
                                            newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
                                        }
                                        else
                                        {
                                            newArguments.Add(arguments[1].Expression);
                                        }
                                        newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(pickPath)));
                                        newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(duration)));
                                        if (arguments.Count > 4)
                                        {
                                            newArguments.Add(arguments[4].Expression);
                                        }
                                        flag = true;
                                    }
                                    break;
                                }
                            case "AddV3D":
                                {
                                    var (status, isWar3, pickPath, sourcePath) = commandManager.AnalysisFile("v3d", GetArgumentString(arguments[0]).Replace("\"", ""), commandManager.IsSkip);
                                    newArguments.Add(arguments[0].Expression);
                                    if (status is true && arguments.Count < 5)
                                    {
                                        int duration = 0;
                                        if (isWar3)
                                        {
                                            duration = commandManager.GetWar3AuidoDuration(pickPath);
                                        }
                                        else
                                        {
                                            duration = commandManager.GetAudioFileDuration(sourcePath);

                                        }
                                        // 如果第二个参数为空字符串，则替换为 null
                                        if (GetArgumentString(arguments[1]) is "" && arguments.Count > 1)
                                        {
                                            newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
                                        }
                                        else
                                        {
                                            newArguments.Add(arguments[1].Expression);
                                        }
                                        newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(pickPath)));
                                        newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(duration)));
                                        if (arguments.Count > 4)
                                        {
                                            newArguments.Add(arguments[4].Expression);
                                        }
                                        else
                                        {
                                            newArguments.Add(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
                                        }
                                        flag = true;
                                    }
                                    break;
                                }
                            case "AddVWP":
                                {
                                    var (status, isWar3, pickPath, sourcePath) = commandManager.AnalysisFile("vwp", GetArgumentString(arguments[0]).Replace("\"", ""), commandManager.IsSkip);

                                    newArguments.Add(arguments[0].Expression);
                                    // 如果使用原生资源
                                    if (status is true && File.Exists(sourcePath))
                                    {
                                        string yaml = File.ReadAllText(sourcePath);
                                        var war3SoundDurationConfig = new DeserializerBuilder()
                                           .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                           .Build()
                                           .Deserialize<Dictionary<string, List<string>>>(yaml);

                                        var dir = war3SoundDurationConfig.ToDictionary(
                                             kv => kv.Key,
                                             kv => kv.Value
                                                     .Select(conf => (pickPath: conf, durtion: commandManager.GetWar3AuidoDuration(conf)))
                                                     .ToList()
                                         );
                                        var vwp = new VwpSound(GetArgumentString(arguments[0]), dir);

                                        foreach (var kv in vwp.soundTypeDic)
                                        {
                                            // key: "xxx"
                                            // 为字典的 key 构建一个字符串字面量表达式，表示字典元素的键
                                            var keyExpr = SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(vwp.alias));

                                            // 构建 tuple 列表 ( "pickPath", duration )
                                            // 对 kv.Value（一个 List<(string pickPath, int duration)>）中的每一个元素 t：
                                            // - 创建一个 TupleExpression，内部包含两个 ArgumentSyntax：
                                            //   1) 字符串字面量：t.pickPath
                                            //   2) 数值字面量：t.duration
                                            // 最终将所有 TupleExpression 转为 ExpressionSyntax 列表
                                            var tupleExprs = kv.Value
                                                .Select(t => (ExpressionSyntax)SyntaxFactory.TupleExpression(
                                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(new[]
                                                    {
                                                    SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(t.pickPath))),
                                                    SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(t.duration)))
                                                    })))
                                                .ToList();

                                            // new List<(string pickPath, int duration)> { (..), (..), ... }
                                            // 使用 ObjectCreationExpression 创建一个 List<(string pickPath, int duration)> 的实例表达式
                                            // 并通过 WithInitializer 提供一个集合初始化器，初始化器使用上面构建的 tupleExprs 作为集合元素
                                            var listCreation = SyntaxFactory.ObjectCreationExpression(
                                                    SyntaxFactory.ParseTypeName("List<(string pickPath, int duration)>"))
                                                .WithInitializer(SyntaxFactory.InitializerExpression(
                                                    // 使用集合初始化器语法（{ ... }）
                                                    SyntaxKind.CollectionInitializerExpression,
                                                    // 将 tupleExprs（每个都是一个 TupleExpression）作为初始化列表
                                                    SyntaxFactory.SeparatedList(tupleExprs)));

                                            // 字典元素初始化器：{ "key", new List<...>{ ... } }
                                            // 为字典的一个元素创建复杂初始器：复杂元素初始化器的第一个子表达式是 keyExpr（键），
                                            // 第二个子表达式是上面创建的 listCreation（值）
                                            var complexInit = SyntaxFactory.InitializerExpression(
                                                SyntaxKind.ComplexElementInitializerExpression,
                                                SyntaxFactory.SeparatedList<ExpressionSyntax>(new ExpressionSyntax[] { keyExpr, listCreation }));
                                            // 将这个字典元素初始化表达式加入到 newArguments（用于后续生成参数或初始化代码）
                                            newArguments.Add(complexInit);
                                        }
                                        flag = true;
                                    }
                                    break;
                                }
                            case "AddUI":
                                {
                                    var uiPath = Path.Combine(commandManager.Config.Assets, "war3mapUI");
                                    var folder = GetArgumentString(arguments[0]).Replace("\"", "");
                                    newArguments.Add(arguments[0].Expression);

                                    if (Directory.Exists(Path.Combine(uiPath, folder)))
                                    {
                                        var tips = $"【套件】引入：{folder}";
                                        var uiFolder = Path.Combine(uiPath, folder);
                                        //fdf
                                        var fdf = Path.Combine(uiFolder, "main.fdf");
                                        if (File.Exists(fdf))
                                        {
                                            File.Copy(fdf, Path.Combine(uiPath, "UI", $"{folder}.fdf"), true);
                                            var appendFdf = Path.Combine(commandManager.BuildDstPath, "map", "UI", $"{folder}.fdf");
                                        }
                                        //assets
                                        var assetsDir = Path.Combine(uiFolder, "assets");
                                        if (Directory.Exists(assetsDir))
                                        {
                                            Directory.EnumerateFiles(assetsDir).ToList().ForEach(file =>
                                            {
                                                var destFile = Path.Combine(commandManager.BuildDstPath, "resource", folder, file);
                                                if (Path.GetExtension(file).ToLower() == ".mdx")
                                                {
                                                    destFile = Path.ChangeExtension(destFile, ".mdl");
                                                }
                                                File.Copy(file, destFile, true);
                                            });
                                        }
                                        //script
                                        var scriptsDir = Path.Combine(uiFolder, "scripts");
                                        if (Directory.Exists(scriptsDir))
                                        {

                                        }
                                        flag = true;
                                    }
                                    else
                                    {
                                        Log.Error("【套件】" + folder + " 不存在");
                                    }

                                    break;
                                }
                        }
                    }

                    // 如果 flag 为 true，则返回新的节点；否则保留原始调用
                    if (flag)
                    {
                        // var argList = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(elementInits));
                        // 修正为：将 elementInits（ExpressionSyntax 列表）转换为 ArgumentSyntax 列表
                        var argList = SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                newArguments.Select(expr => SyntaxFactory.Argument(expr))
                            )
                        );
                        return node.WithArgumentList(argList);
                    }
                }
                return base.VisitInvocationExpression(node);
            }
        }

        /// <summary>
        /// 判断资源文件是否存在，并返回游戏内部资源路径
        /// </summary>
        /// <typeparam name="TKind">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="alias">别名</param>
        /// <param name="skipDetect">是否跳过检测</param>
        /// <returns>状态和路径的元组</returns>
        public (bool status, bool isWar3, string pickPath, string sourcePath) AnalysisFile(string kind, string path, bool skipDetect)
        {
            var analyser = path.Replace("\\\\\\\\", "\\\\");
            var ext = Path.GetExtension(analyser).ToLower();
            var isContaine = AssetsTypes.TryGetValue(kind, out var support);
            var status = true;
            if (ext == "" && isContaine)
            {
                ext = support.Extention.First();
                analyser = Path.ChangeExtension(analyser, ext);
            }
            var isWar3 = false;
            var sourcePath = Path.Combine(BuildDstPath, "resource", support.Path, analyser); //资源文件
            var asPath = string.Empty;
            // 是否资源文件
            if (File.Exists(sourcePath))
            {
                // 复制assets文件到temp
                if (!skipDetect)
                {
                    asPath = Path.Combine(support.Path, analyser);
                    var dstPath = Path.Combine(BuildDstPath, "resource", support.Path, analyser);
                    File.Copy(sourcePath, dstPath, true);
                }
            }
            else // 否则projects资源文件
            {
                var r = analyser;
                if (analyser.StartsWith("resource\\"))
                {
                    r = r.Replace("resource\\", "");
                }
                sourcePath = Path.Combine(Projects, ProjectName, "w3x", "resource", r);
                if (File.Exists(sourcePath))
                {
                    asPath = Path.Combine("resource", r);
                }
                else //原生资源
                {
                    sourcePath = string.Empty;
                    isWar3 = true;
                    asPath = analyser;

                }
            }
            if (asPath == string.Empty)
            {
                status = false;
                Log.Error($"{kind}-资源文件不存在：{path}");
            }
            asPath = asPath.Replace("/", "\\");
            return (status, isWar3, asPath, sourcePath);
        }

        public async Task SupplementAssetsPackPath(string[] targetPaths)
        {
            foreach (var path in targetPaths)
            {
                var code = await File.ReadAllTextAsync(path);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();
                // 要处理toc

                var rewriter = new AssetsPickPathRewriter(this);
                var newRoot = rewriter.Visit(root);
                if (newRoot is not null)
                {

                    await File.WriteAllTextAsync(path, newRoot.ToFullString());
                }
            }
            return;
        }

        #endregion

        public int GetAudioFileDuration(string file)
        {
            if (Path.GetExtension(file) is ".wav")
            {
                using (var reader = new WaveFileReader(file))
                {
                    return (int)reader.TotalTime.TotalMilliseconds;
                }
            }
            else if (Path.GetExtension(file) is ".mp3")
            {
                using (var reader = new Mp3FileReader(file))
                {
                    return (int)reader.TotalTime.TotalMilliseconds;
                }
            }
            else
            {
                Log.Error($"音频文件不存在：{file}");
                return 0;
            }
        }

        public int GetWar3AuidoDuration(string path)
        {
            var yamlPath = Path.Combine(Template, "war3sounds.yaml");
            if (File.Exists(yamlPath))
            {
                if (War3SoundsYaml is null)
                {
                    string yaml = File.ReadAllText(yamlPath);
                    var war3SoundDurationConfig = new DeserializerBuilder()
                       .WithNamingConvention(UnderscoredNamingConvention.Instance)
                       .Build();
                    War3SoundsYaml = war3SoundDurationConfig.Deserialize<War3Sounds>(yaml);
                }

                var dur = War3SoundsYaml.sounds.Where(o => o.path == path).FirstOrDefault(new SoundItem() { path = string.Empty, duration = 0 });
                if (dur.duration != 0)
                {
                    return dur.duration;
                }
                else
                {
                    Log.Error($"(原生)音频文件不存在：{path}");
                    return 0;
                }
            }
            else
            {
                Log.Error($"(原生)音频配置不存在：{path}");
                return 0;
            }
        }
    }
}
