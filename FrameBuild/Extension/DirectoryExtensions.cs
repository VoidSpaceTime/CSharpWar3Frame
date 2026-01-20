using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace War3FrameBuild.Extension
{
    public static class DirectoryExtensions
    {
        public static void CopyDir(string sourceDir, string targetDir)
        {
            if (string.IsNullOrEmpty(sourceDir)) throw new ArgumentException("sourceDir is null or empty", nameof(sourceDir));
            if (string.IsNullOrEmpty(targetDir)) throw new ArgumentException("targetDir is null or empty", nameof(targetDir));

            // normalize full paths
            var sourceFull = Path.GetFullPath(sourceDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var targetFull = Path.GetFullPath(targetDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // don't attempt to copy into the same directory or into a subdirectory of the source
            if (string.Equals(sourceFull, targetFull, StringComparison.OrdinalIgnoreCase) ||
                targetFull.StartsWith(sourceFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // ensure source exists
            if (!Directory.Exists(sourceFull))
                throw new DirectoryNotFoundException($"Source directory not found: {sourceFull}");

            // create target if missing
            if (!Directory.Exists(targetFull))
                Directory.CreateDirectory(targetFull);

            // enumerate entries in source
            var entries = Directory.GetFileSystemEntries(sourceFull);
            foreach (var entry in entries)
            {
                var entryFull = Path.GetFullPath(entry);

                // skip entries that are inside the target directory to avoid infinite recursion
                if (string.Equals(entryFull, targetFull, StringComparison.OrdinalIgnoreCase) ||
                    entryFull.StartsWith(targetFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var name = Path.GetFileName(entryFull);
                var destPath = Path.Combine(targetFull, name);

                if (Directory.Exists(entryFull))
                {
                    CopyDir(entryFull, destPath);
                }
                else
                {
                    File.Copy(entryFull, destPath, true);
                }
            }
        }
    }
}