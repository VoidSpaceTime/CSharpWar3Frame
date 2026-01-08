using Serilog;
using System.Collections.Concurrent;

namespace War3FrameBuild.CommandManager
{

    public partial class CommandManager
    {
        private DateTime fsnSec;

        private List<string> watchDir = new List<string>();
        readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers = new(StringComparer.OrdinalIgnoreCase);
        // 记录最近事件时间，防止短时间内重复触发

        public void WatchDir(string dir)
        {
            if (Directory.Exists(dir))
            {
                if (_watchers.ContainsKey(dir)) return;
                var watcher = new FileSystemWatcher(dir)
                {
                    IncludeSubdirectories = true,
                    Filter = "*",
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size,
                    EnableRaisingEvents = true
                };
                watcher.Created += OnFsEvent;
                watcher.Changed += OnFsEvent;
                watcher.Deleted += OnFsEvent;
            }
        }
        void OnFsEvent(object sender, FileSystemEventArgs e)
        {
            var now = DateTime.UtcNow;
            var mode = e.ChangeType;
            if ((now - fsnSec) <= TimeSpan.FromSeconds(5))
            {
                return;
            }
            if (Path.GetExtension(e.FullPath).ToLower() is ".cs")
            {
                return;
            }
            // 跳过包含builtIn的路径





            //Compiler();

        }
        public void Hot()
        {
            fsnSec = DateTime.UtcNow;
            Log.Verbose("全局热更新生效中...");
            WatchDir(Path.Combine(Config.Pwd, "Library"));
            WatchDir(Path.Combine(Config.Assets, "war3mapFont"));
            WatchDir(Path.Combine(Config.Assets, "war3mapUI"));
            WatchDir(Path.Combine(Projects, ProjectName));
            WatchDir(Path.Combine(Temp, BuildMode.ToString(), ProjectName, "map"));
        }
    }
}
