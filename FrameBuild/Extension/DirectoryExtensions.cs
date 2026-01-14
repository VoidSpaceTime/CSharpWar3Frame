using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace War3FrameBuild.Extension
{
    public static class DirectoryExtensions
    {
        public static void CopyDir(string sourceDir, string targetDir)
        {

            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加
                if (targetDir[targetDir.Length - 1] != Path.DirectorySeparatorChar)
                {
                    targetDir += Path.DirectorySeparatorChar;
                }
                // 判断目标目录是否存在如果不存在则新建
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                // string[] fileList = Directory.GetFiles（srcPath）；
                string[] fileList = Directory.GetFileSystemEntries(sourceDir);
                // 遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (Directory.Exists(file))
                    {
                        CopyDir(file, targetDir + Path.GetFileName(file));
                    }
                    // 否则直接Copy文件
                    else
                    {
                        File.Copy(file, targetDir + Path.GetFileName(file), true);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }


        }
    }
}