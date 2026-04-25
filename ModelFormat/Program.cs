// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using FastMDX;

var sourceFolder = args.Length >= 1 ? args[0] : string.Empty;
var isRename = false;

sourceFolder = @"D:\Game\war3\xlik\.tmp\_local\demo\resource\war3mapModel";
if (sourceFolder == string.Empty)
{
    Console.WriteLine("传入文件夹地址为空");
    return;
}

if (!Directory.Exists(sourceFolder))
{
    Console.WriteLine("传入文件夹地址不存在");
    return;
}

var targetFolder = sourceFolder + "_Format";
if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);
var modelsFile = Directory.GetFiles(sourceFolder, "*.mdx", SearchOption.AllDirectories).ToList();
modelsFile.AddRange(Directory.GetFiles(sourceFolder, "*.mdl", SearchOption.AllDirectories).ToList());
var count = 0;
foreach (var modelFile in modelsFile)
{
    var modelFolder = Path.Combine(targetFolder, Path.GetFileNameWithoutExtension(modelFile));
    if (!Directory.Exists(modelFolder)) Directory.CreateDirectory(modelFolder);
    try
    {
        var model = new MDX(modelFile);
        for (var i = 0; i < model.Textures.Count(); i++)
        {
            var flag = false;
            var texture = model.Textures[i];
            if (texture.ReplaceableId is 0)
            {
                var texturePath = texture.Name;
                var ext = Path.GetExtension(texturePath).ToLower();
                var textureName = Path.ChangeExtension(texturePath, ext);

                texturePath = Path.Combine(Directory.GetParent(modelFile).FullName, textureName);
                if (File.Exists(texturePath))
                {
                    flag = true;
                }
                else
                {
                    var folderName = "war3mapModel";
                    var pattern = $@"^(.*?[/\\]{Regex.Escape(folderName)})(?=[/\\]|$)";
                    var m = Regex.Match(modelFile, pattern, RegexOptions.IgnoreCase);
                    var xlikTextureDir = Directory.GetParent(m.Value).FullName;
                    texturePath = Path.Combine(xlikTextureDir, textureName);
                    if (File.Exists(texturePath)) flag = true;
                }

                if (flag)
                {
                    if (isRename)
                        texture.Name = (Path.GetFileNameWithoutExtension(modelFile) + $"_{i}").ToLower();
                    else
                        texture.Name = Path.GetFileName(texturePath).ToLower();
                    File.Copy(texturePath, Path.Combine(modelFolder, texture.Name), true);
                }
            }
        }

        model.SaveTo(Path.Combine(modelFolder, Path.GetFileName(modelFile)));
        count++;
    }
    catch
    {
        throw new Exception("模型打开失败:" + modelFile);
    }
}

Console.WriteLine($"完成模型格式化，共处理模型数量：{count},失败:{modelsFile.Count() - count}");
Console.ReadLine();