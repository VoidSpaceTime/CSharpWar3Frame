using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace War3Frame.Library
{
    public class Assets
    {
        public Dictionary<string, Image> Images = new();
        public Dictionary<string, Model> Models = new();
        public Dictionary<string, Sound> Sounds = new();
        public Dictionary<string, VcmSound> VcmSounds = new();
        public Dictionary<string, V3dSound> V3dSounds = new();
        public Dictionary<string, VwpSound> VwpSounds = new();
        public List<string> UIKit = new();
        public Dictionary<string, UI> UIAssets = new();
        public Dictionary<string, Font> Fonts = new();
        public string Terrain = string.Empty;
        public string Selection = string.Empty;
        public string Loading = string.Empty;
        public string Preview = string.Empty;

        public record Image(string path, string? pickPath);
        public record Model(string path, string? pickPath);
        public record Sound(string path, string? pickPath, int duration, int volume);
        public record VcmSound(string path, string? pickPath, int duration, int volume);
        public record V3dSound(string path, string? pickPath, int duration, int volume);
        public record VwpSound(string alias, Dictionary<string, List<(string pickPath, int duration)>>? soundTypeDic = null);
        public record UI(string file, string? pickPath); // 需要核对 xlik ui 处理z
        public record Font(float crWide, float zhWide, float h);

        #region Add Methods
        public void AddImage(string path, string? alias = null, string? pickPath = null)
        {
            Images.Add(alias ?? path, new(path, pickPath));
        }
        public void AddModel(string path, string? alias = null, string? pickPath = null)
        {
            Models.Add(alias ?? path, new(path, pickPath));
        }
        public void AddSounds(string path, string? alias, string? pickPath = null, int duration = 0, int volume = 127)
        {
            volume = Math.Min(volume, 127);
            Sounds.Add(alias ?? path, new(path, pickPath, duration, volume));
        }
        public void AddVCM(string path, string? alias, string? pickPath = null, int duration = 0, int volume = 127)
        {
            volume = Math.Min(volume, 127);
            VcmSounds.Add(alias ?? path, new(path, pickPath, duration, volume));
        }
        public void AddV3D(string path, string? alias, string? pickPath = null, int duration = 0, int volume = 127)
        {
            volume = Math.Min(volume, 127);
            V3dSounds.Add(alias ?? path, new(path, pickPath, duration, volume));
        }
        public void AddVWP(string folder, VwpSound? vwpSound = null)
        {
            if (vwpSound is not null && vwpSound.alias is not "")
            {
                VwpSounds.Add(folder, vwpSound);
            }
        }
        public void AddFont(string file, float crWide = 0.65f, float zhWide = 1.03f, float h = 1.126f)
        {
            Fonts.Add(file, new(crWide, zhWide, h));
        }
        public void AddTerrain(string dir)
        {
            Terrain = dir;
        }
        public void AddUIKit(string dir)
        {
            UIKit.Add(dir);
        }
        public void AddUIAssets(string dir, string file, string pickPath)
        {
            UIAssets.Add(dir, new(file, pickPath));
        }
        public void AddSelection(string dir)
        {
            Selection = dir;
        }
        public void AddLoading(string fileName)
        {
            Loading = fileName;
        }
        public void AddPreview(string fileName)
        {
            Preview = fileName;
        }
        #endregion



    }
}
