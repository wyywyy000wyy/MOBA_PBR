using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine.U2D;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class XManifest
{
    public static XManifest Instance { get; internal set; }
    public static CacheType BundleDefaultCacheType = XManifest.CacheType.AllTime;

    public bool isExternal { get; private set; }

    static XManifest()
    {
        Instance = new XManifest();
    }

    public const string name = "manifest";
    private Dictionary<string, Pack> _packs = new Dictionary<string, Pack>();
    private Dictionary<string, File> _files = new Dictionary<string, File>();
    private Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();
    private Dictionary<string, SpritePack> _spritePacks = new Dictionary<string, SpritePack>();
    private Dictionary<string, SpritePack> _spritepackRelation = new Dictionary<string, SpritePack>();
    private List<BundlePackRule> _bundleRules = new List<BundlePackRule>();
    private Dictionary<string, AssetInfo> _fastIndexs = new Dictionary<string, AssetInfo>();
    private Dictionary<string, string> _pathABNameRelation = new Dictionary<string, string>();

#if UNITY_EDITOR
    public static string GetPlatformFolder(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "android";
            case BuildTarget.iOS:
                return "ios";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "windows";
            case BuildTarget.StandaloneOSX:
                return "osx";
            default:
                return null;
        }
    }
    public static string resOutputPath = System.IO.Path.Combine(System.Environment.CurrentDirectory,
        string.Format("build/bundle/{0}", XManifest.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget))).Replace("\\", "/");

    public static string manifestOutputPath = System.IO.Path.Combine(resOutputPath, name).Replace("\\", "/");

    public static string updateResOutputPath = System.IO.Path.Combine(System.Environment.CurrentDirectory,
            string.Format("build/res/{0}", XManifest.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget))).Replace("\\", "/");

    public static string updateRenameResOutputPath = System.IO.Path.Combine(System.Environment.CurrentDirectory,
        string.Format("build/update/{0}", XManifest.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget))).Replace("\\", "/");

#endif

    public static string GetPlatformFolder(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            case RuntimePlatform.WindowsPlayer:
                return "windows";
            case RuntimePlatform.OSXPlayer:
                return "osx";
            default:
                return null;
        }
    }

    public static string GetPlatformFolder()
    {
#if UNITY_EDITOR
        return GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget);
#else
        return GetPlatformFolder(Application.platform);
#endif
    }

    public AssetInfo Find(string name)
    {
        AssetInfo index = null;
        _fastIndexs.TryGetValue(name, out index);
        return index;
    }
#if UNITY_EDITOR
    public void Load(string path)
    {
        Clear();
        System.IO.StreamReader sr = new System.IO.StreamReader(path);
        string data = sr.ReadToEnd();
        sr.Close();
        XManifestInfo info = XManifestInfo.Parse(data);
        _ParseContent(info);
    }
#endif
    public void Initialize(bool useExternalFolder) // useExternalFolder:是否允许从外部储存目录读取
    {
        Clear();
        isExternal = false;

        string data = string.Empty;
        string extPath = System.IO.Path.Combine(Application.persistentDataPath, name);
        if (useExternalFolder && System.IO.File.Exists(extPath))
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(extPath);
            data = sr.ReadToEnd();
            sr.Close();

            Debug.LogFormat("...Load Manifest in ext path: {0}...", extPath);

            if (string.IsNullOrEmpty(data))
            {
                Debug.LogFormat("...ext Manifest damaged, delete now: {0}...", extPath);
                System.IO.File.Delete(extPath);
            }
            else
            {
                isExternal = true;
            }
        }
        else
        {
            data = ResHelper.LoadTextFromStreamingAssets(System.IO.Path.Combine(Application.streamingAssetsPath, name));
        }

        if (string.IsNullOrEmpty(data))
        {
            Debug.LogFormat("...Manifest not exist...", extPath);
            return;
        }

        XManifestInfo info = XManifestInfo.Parse(data);
        if (info != null)
        {
            _ParseContent(info);
        }
        else
        {
            Debug.LogError("...Parse Manifest info failed...");
        }
    }

    private void _ParseContent(XManifestInfo info)
    {
        if (info.files != null)
        {
            for (int i = 0; i < info.files.Count; i++)
                Add(info.files[i]);
        }

        if (info.packs != null)
        {
            for (int i = 0; i < info.packs.Count; i++)
                Add(info.packs[i]);
        }

        if (info.scenes != null)
        {
            for (int i = 0; i < info.scenes.Count; i++)
                Add(info.scenes[i]);
        }

        if (info.spritePacks != null)
        {
            for (int i = 0; i < info.spritePacks.Count; i++)
                Add(info.spritePacks[i]);
        }
        if (info.bundleRules != null)
        {
            for (int i = 0; i < info.bundleRules.Count; i++)
            {
                AddBundleRule(info.bundleRules[i]);
            }
        }
    }

    public void Remove(Entry entry)
    {
        switch (entry.Type())
        {
            case EntryType.Pack:
                _packs.Remove(entry.name);
                break;
            case EntryType.File:
                _files.Remove(entry.name);
                break;
            case EntryType.Scene:
                _scenes.Remove(entry.name);
                break;
            case EntryType.SpritePack:
                _spritePacks.Remove(entry.name);
                break;
        }
    }

    public void Add(Entry entry)
    {
        switch (entry.Type())
        {
            case EntryType.Pack:
                _packs.Add(entry.name, entry as Pack);
                break;
            case EntryType.File:
                _files.Add(entry.name, entry as File);
                break;
            case EntryType.Scene:
                _scenes.Add(entry.name, entry as Scene);
                break;
            case EntryType.SpritePack:
                _spritePacks.Add(entry.name, entry as SpritePack);
                break;
        }
    }

    public void Add(Pack pack)
    {
        _packs.Add(pack.name, pack);
    }

    public void Add(File file)
    {
        _files.Add(file.name, file);
        AddFastIndex(file.name, new AssetInfo() { location = file.location, fullName = string.Format("{0}.{1}", file.name, file.checksum) });
    }

    public void Add(Scene scene)
    {
        _scenes.Add(scene.name, scene);
    }

    public void Add(SpritePack spritePack)
    {
        _spritePacks.Add(spritePack.name, spritePack);
        if (spritePack.packable != null)
        {
            foreach (var pa in spritePack.packable)
            {
                if (!_spritepackRelation.ContainsKey(pa))
                {
                    _spritepackRelation.Add(pa, spritePack);
                }
                else
                {
                    Debug.LogError(string.Format("{0} packed in muti file:{1} and {2}", pa, _spritepackRelation[pa].name, spritePack.name));
                }
            }
        }
    }

    public void AddBundleRule(BundlePackRule rule)
    {
        _bundleRules.Add(rule);
    }

    public string GetABNameByPath(string filePath)
    {
        if (!_pathABNameRelation.ContainsKey(filePath))
        {
            _pathABNameRelation[filePath] = BundlePackRule.CalculateBundleNameByPath(filePath, _bundleRules);
        }
        return _pathABNameRelation[filePath];
    }

    public SpritePack FindPackBySprite(string spritePath)
    {
        while (true)
        {
            if (_spritepackRelation.ContainsKey(spritePath))
            {
                return _spritepackRelation[spritePath];
            }
            var lastidx = spritePath.LastIndexOf("/");
            if (lastidx > 0)
            {
                spritePath = spritePath.Substring(0, lastidx);
            }
            else
            {
                return null;
            }
        }
    }

    public void AddFastIndex(string name, AssetInfo index)
    {
        string key = name;
        if (System.IO.Path.HasExtension(name))
            key = System.IO.Path.ChangeExtension(name, null);

#if UNITY_EDITOR
        //Debug.LogFormat("AddFastIndex key({0}) name({1}) location({2})", key, name, index.location);
#endif
        _fastIndexs[key] = index;
    }

    public void Clear()
    {
        _packs.Clear();
        _files.Clear();
        _scenes.Clear();
        _spritePacks.Clear();
        _fastIndexs.Clear();
        _spritepackRelation.Clear();
        _bundleRules.Clear();
    }

    public Pack GetPack(string name)
    {
        Pack pack = null;
        _packs.TryGetValue(name, out pack);
        return pack;
    }

    public File GetFile(string name)
    {
        File file = null;
        _files.TryGetValue(name, out file);
        return file;
    }

    public Scene GetScene(string name)
    {
        Scene scene = null;
        _scenes.TryGetValue(name, out scene);
        return scene;
    }

    public SpritePack GetSpritePack(string name)
    {
        SpritePack spritePack = null;

        _spritePacks.TryGetValue(name, out spritePack);
        return spritePack;
    }

    public IEnumerable GetPacks()
    {
        return _packs;
    }

    public IEnumerable GetScenes()
    {
        return _scenes;
    }

    public IEnumerable GetSpritePacks()
    {
        return _spritePacks;
    }

    public IEnumerable GetFiles()
    {
        return _files;
    }

    public IEnumerable GetBundleRules()
    {
        return _bundleRules;
    }

#if UNITY_EDITOR
    public bool Exists(string fullpath)
    {
        string key = fullpath;
        if (System.IO.Path.HasExtension(fullpath))
            key = System.IO.Path.ChangeExtension(fullpath, null);

        return _fastIndexs.ContainsKey(key);
    }
#endif

    public void SaveToPersistentDataPath()
    {
        var path = System.IO.Path.Combine(Application.persistentDataPath, name);
        Save(path, false);
    }

    public void Save(string path, bool pretty = true, int MainVersion = 0, int SubVersion = 0)
    {
        List<Pack> packs = new List<Pack>();
        foreach (Pack pack in _packs.Values)
        {
            packs.Add(pack);
        }

        List<File> files = new List<File>();
        foreach (File file in _files.Values)
        {
            files.Add(file);
        }

        List<Scene> scenes = new List<Scene>();
        foreach (Scene scene in _scenes.Values)
        {
            scenes.Add(scene);
        }

        List<SpritePack> spritePacks = new List<SpritePack>();
        foreach (SpritePack spritePack in _spritePacks.Values)
        {
            spritePacks.Add(spritePack);
        }

        List<BundlePackRule> bundleRules = new List<BundlePackRule>();
        foreach (BundlePackRule bundleRule in _bundleRules)
        {
            bundleRules.Add(bundleRule);
        }

//#if UNITY_EDITOR

//        #region CACHE
//        BundleExtraCacheInfo cacheInfo = null;
//        List<string> cacheInScene = null;
//        List<string> cacheAllTime = null;
//        List<string> notCache = null;
//        string infoText = BundleExtraInfoUtils.GetBundleExtraCacheInfoText();
//        if (!string.IsNullOrEmpty(infoText))
//        {
//            cacheInfo = LitJson.JsonMapper.ToObject<BundleExtraCacheInfo>(infoText);
//            if (cacheInfo != null)
//            {
//                cacheInScene = BundleExtraInfoUtils.ArrayToList<string>(cacheInfo.CacheInScene);
//                if (cacheInScene == null)
//                    cacheInScene = new List<string>();
//                cacheAllTime = BundleExtraInfoUtils.ArrayToList<string>(cacheInfo.CacheAllTime);
//                if (cacheAllTime == null)
//                    cacheAllTime = new List<string>();

//                notCache = BundleExtraInfoUtils.ArrayToList<string>(cacheInfo.NotCache);
//                if (notCache == null)
//                    notCache = new List<string>();

//                foreach (Pack pack in packs)
//                {
//                    SetupBundleCacheType(pack, notCache, cacheInScene, cacheAllTime);
//                }

//                foreach (Scene scene in scenes)
//                {
//                    SetupBundleCacheType(scene, notCache, cacheInScene, cacheAllTime);
//                }
//            }
//        }
//        else
//            Debug.LogError("NOT FOUND BUNDLE EXTRA INFO TEXT!!!!!!!!!!!!");

//        #endregion      //CACHE


//#endif

        XManifestInfo manifest = new XManifestInfo();
        manifest.packs = packs;
        manifest.files = files;
        manifest.scenes = scenes;
        manifest.spritePacks = spritePacks;
        manifest.bundleRules = bundleRules;
        manifest.MainVersion = MainVersion;
        manifest.SubVersion = SubVersion;

        Debug.LogFormat("Mainfest_Save {0}, {1}", MainVersion, SubVersion);

        System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        if (fs != null)
        {
            StringBuilder sb = new StringBuilder();
            System.IO.StringWriter writer = new System.IO.StringWriter(sb);
            //LitJson.JsonMapper.ToJson(manifest,
            //                          new LitJson.JsonWriter(writer)
            //                          {
            //                              PrettyPrint = pretty
            //                          });

            byte[] buff = Encoding.UTF8.GetBytes(sb.ToString());

            fs.Write(buff, 0, buff.Length);
            fs.Close();
        }
    }

    private void SetupBundleCacheType(Entry entry, List<string> notCache, List<string> cacheInScene, List<string> cacheAllTime)
    {
        if (cacheInScene.Contains(entry.name))
        {
            entry.cacheType = CacheType.InScene;
        }
        else if (cacheAllTime.Contains(entry.name))
        {
            entry.cacheType = CacheType.AllTime;
        }
        else if (notCache.Contains(entry.name))
        {
            entry.cacheType = CacheType.None;
        }
        else
        {
            entry.cacheType = XManifest.BundleDefaultCacheType;
        }
    }
    public void DeleteLocal()
    {
        string extPath = System.IO.Path.Combine(Application.persistentDataPath, name);
        if (System.IO.File.Exists(extPath))
            System.IO.File.Delete(extPath);

        Debug.LogFormat("[XManifest.DeleteLocal] {0}", extPath);
    }

    public Queue<EntryDiff> GenerateManifestDiff(XManifestInfo newManifest)
    {
        Queue<EntryDiff> diffManifest = new Queue<EntryDiff>();

        // pack diff
        for (int i = 0; i < newManifest.packs.Count; i++)
        {
            bool diff = true;
            Pack pack = newManifest.packs[i];
            Pack localPack;
            if (_packs.TryGetValue(pack.name, out localPack))
            {
                if (localPack.checksum == pack.checksum)
                    diff = false;
            }

            if (diff)
                diffManifest.Enqueue(new EntryDiff() { type = EntryType.Pack, local = localPack, remote = pack });
        }

        // file diff
        for (int i = 0; i < newManifest.files.Count; i++)
        {
            bool diff = true;
            File file = newManifest.files[i];
            File localFile;
            if (_files.TryGetValue(file.name, out localFile))
            {
                if (localFile.checksum == file.checksum)
                    diff = false;
            }

            if (diff)
                diffManifest.Enqueue(new EntryDiff() { type = EntryType.File, local = localFile, remote = file });
        }

        // scene diff
        for (int i = 0; i < newManifest.scenes.Count; i++)
        {
            bool diff = true;
            Scene scene = newManifest.scenes[i];
            Scene localScene;
            if (_scenes.TryGetValue(scene.name, out localScene))
            {
                if (localScene.bundle == scene.bundle)
                    diff = false;
            }

            if (diff)
                diffManifest.Enqueue(new EntryDiff() { type = EntryType.Scene, local = localScene, remote = scene });
        }

        // spritepack diff
        for (int i = 0; i < newManifest.spritePacks.Count; i++)
        {
            bool diff = true;
            SpritePack spritePack = newManifest.spritePacks[i];
            SpritePack localSpritePack;
            if (_spritePacks.TryGetValue(spritePack.name, out localSpritePack))
            {
                if (localSpritePack.realName == spritePack.realName)
                    diff = false;
            }

            if (diff)
                diffManifest.Enqueue(new EntryDiff() { type = EntryType.SpritePack, local = localSpritePack, remote = spritePack });
        }

        return diffManifest;
    }

    public void ClearUnuseAsset()
    {
        if (Directory.Exists(LuaFilePicker.dataBasePath))
        {
            FileInfo[] allFiles = new DirectoryInfo(LuaFilePicker.dataBasePath).GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in allFiles)
            {
                var arr = file.Name.Split('.');
                if (arr.Length == 2)
                {
                    var name = arr[0];
                    uint checksum = 0;
                    try
                    {
                        checksum = uint.Parse(arr[1]);
                    }
                    catch
                    {

                    }
                    var record = GetFile(name);
                    if (record == null || record.checksum != checksum)
                    {
                        Debug.Log("delete file:" + file.Name);
                        file.Delete();
                    }
                }
            }
        }

        if (Directory.Exists(AssetBundleManager.dataBasePath))
        {
            FileInfo[] allFiles = new DirectoryInfo(AssetBundleManager.dataBasePath).GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in allFiles)
            {
                var arr = file.Name.Split('.');
                if (arr.Length == 2)
                {
                    var name = arr[0];
                    uint checksum = 0;
                    try
                    {
                        checksum = uint.Parse(arr[1]);
                    }
                    catch
                    {

                    }
                    var record = GetPack(name);
                    if (record == null || record.checksum != checksum)
                    {
                        Debug.Log("delete bundle:" + file.Name);
                        file.Delete();
                    }
                }
            }
        }
    }

    public bool Verify()
    {
        if(!isExternal){
            return true;
        }
        
        foreach (var pairs in _packs)
        {
            var pack = pairs.Value;
            if (pack.location == Location.Data)
            {
                if (!System.IO.File.Exists(Path.Combine(AssetBundleManager.dataBasePath, string.Format("{0}.{1}", pack.name, pack.checksum))))
                {
                    return false;
                }
            }
        }
        foreach (var pairs in _files)
        {
            var file = pairs.Value;
            if (file.location == Location.Data)
            {
                if (!System.IO.File.Exists(Path.Combine(LuaFilePicker.dataBasePath, string.Format("{0}.{1}", file.name, file.checksum))))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void RepleaceWith(XManifestInfo newManifest)
    {
        foreach (var pack in newManifest.packs)
        {
            if (System.IO.File.Exists(Path.Combine(AssetBundleManager.dataBasePath, string.Format("{0}.{1}", pack.name, pack.checksum))))
            {
                pack.location = Location.Data;
            }
            else
            {
                pack.location = Location.Streaming;
            }
        }

        foreach (var file in newManifest.files)
        {
            if (System.IO.File.Exists(Path.Combine(LuaFilePicker.dataBasePath, string.Format("{0}.{1}", file.name, file.checksum))))
            {
                file.location = Location.Data;
            }
            else
            {
                file.location = Location.Streaming;
            }
        }

        Clear();
        _ParseContent(newManifest);
        XManifest.Instance.SaveToPersistentDataPath();
    }

    public static string GetLocalFileUrl(string path)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return "file:///" + path;
            default:
                return "file://" + path;
        }
    }


    public enum Location
    {
        Resource, Data, Streaming, Bundle
    }

    public enum EntryType { None, Pack, File, Scene, SpritePack };
    public enum CacheType { None, InScene, AllTime };
    public enum PackageType { None, Package1, Package2, Package3 };
    public enum CompressionType { None, Zip };
    public class Entry
    {

        protected EntryType type = EntryType.None;
        public EntryType Type() { return type; }

        public uint checksum { get; set; }
        public string name { get; set; }
        public Location location { get; set; }
        public CacheType cacheType { get; set; }
        public CompressionType compressionType { get; set; }
        public uint size { get; set; }
    }

    public class File : Entry
    {
        public File() { type = EntryType.File; }
    }

    public class Pack : Entry
    {
        public string[] dependencies { get; set; }

        public Pack() { type = EntryType.Pack; }
        public bool NoBundle() { return location == Location.Resource; }
    }

    public class Scene : Entry
    {
        public string bundle { get; set; }

        public Scene() { type = EntryType.Scene; }
    }

    public class SpritePack : Entry
    {
        public string realName { get; set; }
        public string[] packable { get; set; }
        public SpritePack() { type = EntryType.SpritePack; }
    }

    public class XManifestInfo
    {
        public int MainVersion { get; set; }
        public int SubVersion { get; set; }
        public List<Pack> packs { get; set; }
        public List<File> files { get; set; }
        public List<Scene> scenes { get; set; }
        public List<SpritePack> spritePacks { get; set; }
        public List<BundlePackRule> bundleRules { get; set; }

        public static XManifestInfo Parse(string data)
        {
            return new XManifestInfo();
            //return LitJson.JsonMapper.ToObject<XManifestInfo>(data);
        }

        public void Init()
        {
            packs = new List<Pack>();
            files = new List<File>();
            scenes = new List<Scene>();
            spritePacks = new List<SpritePack>();
            bundleRules = new List<BundlePackRule>();
            MainVersion = 0;
            SubVersion = 0;
        }
    }

    public class AssetInfo
    {
        public Location location { get; set; }
        public string bundle { get; set; }
        public string fullName { get; set; }
    }

    public class EntryDiff
    {
        public EntryType type = EntryType.None;
        public Entry local;
        public Entry remote;
    }

}
