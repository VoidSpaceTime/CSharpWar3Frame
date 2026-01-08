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
            // 创建目标文件夹
            if (!Directory.Exists(sourceDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // 复制所有文件
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destFile, true);
            }

            // 递归复制所有子文件夹
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                string destSubDir = Path.Combine(targetDir, dirName);
                CopyDir(dir, destSubDir);
            }
        }

    }
}
