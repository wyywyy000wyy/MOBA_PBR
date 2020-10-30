using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameConfig
{
    public class VersionInfo
    {
        public int MainVersion { get; set; }
        public int SubVersion { get; set; }
        public uint ManifestChecksum { get; set; }
        public string ResRoot { get; set; }
        public string ResRootPublish { get; set; }
        public string Area { get; set; }
        public string GetVersion()
        {
            return string.Format("{0}.{1}.{2}", MainVersion, SubVersion / 100, SubVersion % 100);
        }

        public int GetVersionCode()
        {
            return (int)(MainVersion * 1000000) + SubVersion;
        }

        public void SetVersionCode(int code)
        {
            SubVersion = code % 1000000;
            MainVersion = (code - SubVersion) / 1000000;
        }

        void Init()
        {
            MainVersion = 0;
            SubVersion = 0;
            ManifestChecksum = 0;
            ResRoot = string.Empty;
            ResRootPublish = string.Empty;
            Area = string.Empty;
        }

        public VersionInfo()
        {
            Init();
        }
        public VersionInfo(int main, int sub)
        {
            Init();
            MainVersion = main;
            SubVersion = sub;
        }
        public VersionInfo(int code)
        {
            Init();
            SetVersionCode(code);
        }
    }

#if RELEASE
    public static VersionInfo version = new VersionInfo(GameVersion.code);
#else
    public static VersionInfo version = new VersionInfo(1, 0);
#endif
}
