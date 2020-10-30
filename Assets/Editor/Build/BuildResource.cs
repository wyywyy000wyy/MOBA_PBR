using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class BuildResource : Editor
{
    public static int VersionCode
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("version"))
                {
                    return int.Parse(arg.Split(":"[0])[1]);
                }
            }
            return 0;
        }
    }

    public static string assetBundleOutputPath = System.IO.Path.Combine(System.Environment.CurrentDirectory,
        string.Format("build/bundle/{0}", XManifest.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget))).Replace("\\", "/");
    public const string assetBasePath = "assets/resources";

    [MenuItem("Build/Build Resource")]
    public static void Build()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        Build(buildTarget);
        GenerateVersionInfo(buildTarget);
    }

    [MenuItem("Build/Build Resource ios")]
    public static void BuildIos()
    {
        BuildTarget buildTarget = BuildTarget.iOS;
        Build(buildTarget);
        GenerateVersionInfo(buildTarget);
    }

    public static void BuildWithVersionCode()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        Build(buildTarget);
        GenerateVersionInfo(buildTarget, VersionCode);
    }

    [MenuItem("Build/Check/Build Resource - Only Rebuild Lua")]
    public static void BuildOnlyLua()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        AssetDatabase.RemoveUnusedAssetBundleNames();

        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);

        BuildLua.GenLuaBytes();             // 生成lua

        GenerateFileAllocationTable(buildTarget); // 根据lua和assetbundle生成manifest

        _PrepareResources(System.IO.Path.Combine(Application.dataPath, "StreamingAssets"), false); // 将资源拷贝入 StreamingAssets
        if (Directory.Exists(XManifest.updateRenameResOutputPath))
            Directory.Delete(XManifest.updateRenameResOutputPath, true); //清理上次的资源
        _PrepareResources(XManifest.updateRenameResOutputPath, true);   // 准备自动更新资源

        // gen manifest.txt into StreamingAssets
        AssetDatabase.Refresh();

        Debug.Log("<color=yellow>[Build] Build Resource Complete!</color>");
    }

    public static void Build(BuildTarget buildTarget)
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);

        BuildLua.GenLuaBytes();             // 生成lua
        _BuildAllAssetBundles(buildTarget); // 生成assetBundle

        GenerateFileAllocationTable(buildTarget); // 根据lua和assetbundle生成manifest

        _PrepareResources(System.IO.Path.Combine(Application.dataPath, "StreamingAssets"), false); // 将资源拷贝入 StreamingAssets
        if (Directory.Exists(XManifest.updateRenameResOutputPath))
            Directory.Delete(XManifest.updateRenameResOutputPath, true); //清理上次的资源
        _PrepareResources(XManifest.updateRenameResOutputPath, true);   // 准备自动更新资源

        // gen manifest.txt into StreamingAssets
        AssetDatabase.Refresh();

        Debug.Log("<color=yellow>[Build] Build Resource Complete!</color>");
    }

    static void _BuildAllAssetBundles(BuildTarget buildTarget)
    {
        string outputPath = assetBundleOutputPath;

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        AssetDatabase.Refresh();
        AssetDatabase.RemoveUnusedAssetBundleNames();
        BundleNameHelper.CheckAndSetAllBundleName();

        //ForceRebuildAssetBundle: 强制刷新图集bundle 与 prefeb的引用关系
        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DisableWriteTypeTree;//| BuildAssetBundleOptions.ForceRebuildAssetBundle;
        BuildPipeline.BuildAssetBundles(outputPath, option, buildTarget);
        Debug.Log("[BuildAllAssetBundles] at:" + outputPath);
    }

    static void GetAllFile(DirectoryInfo directory, List<string> list)
    {
        foreach (FileSystemInfo fsi in directory.GetFileSystemInfos())
        {
            if (fsi is System.IO.FileInfo)
            {
                if (!fsi.Extension.Equals(".meta") && !string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(fsi.Name)))
                    list.Add(fsi.FullName);
            }
            else
            {
                GetAllFile((DirectoryInfo)fsi, list);
            }
        }
    }

    [MenuItem("Assets/Tools/单AB测试")]
    static void BuildSignleAB()
    {
        UnityEngine.Object[] objects = Selection.objects;
        List<AssetBundleBuild> all = new List<AssetBundleBuild>();
        for (int i = 0; i < objects.Length; i++)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            string p = AssetDatabase.GetAssetPath(objects[i]);
            build.assetBundleName = p.Replace("/", ".");
            build.assetNames = new string[] { p };
            all.Add(build);
        }
        QuickAcessUtils.CheckDirectory("单AB测试", false);
        BuildPipeline.BuildAssetBundles("单AB测试", all.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }

    public static void GetFileInfo(string path, out uint checksum, out uint size)
    {
        FileInfo info = new FileInfo(path);
        if (info == null)
        {
            Debug.LogError("path not exsit:" + path);
            checksum = 0;
            size = 0;
        }
        else
        {
            size = (uint)info.Length;
            checksum = GHelper.ComputeFileChecksum(path);
        }
    }

    public static void GenerateFileAllocationTable(BuildTarget buildTarget)
    {
        string targetName = XManifest.GetPlatformFolder(buildTarget);
        string bundlePath = assetBundleOutputPath;

        EditorUtility.DisplayProgressBar("Generate Manifest", "loading bundle manifest", 1 / 2);

        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(bundlePath, targetName));

        if (bundle == null)
        {
            Debug.LogError("Build Resrouce First.");
            EditorUtility.ClearProgressBar();
            return;
        }

        XManifest.Instance.Clear();

        try
        {
            string manifestSavePath = XManifest.manifestOutputPath;

            // gen packs
            AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] bundles = manifest.GetAllAssetBundles();
            EditorUtility.DisplayProgressBar("Generate Manifest", "compute bundle hash", 0);
            for (int i = 0; i < bundles.Length; i++)
            {
                string bundleName = bundles[i];

                XManifest.Pack pack = new XManifest.Pack();
                pack.name = bundleName;
                uint checksum = 0;
                uint size = 0;
                GetFileInfo(Path.Combine(bundlePath, bundleName), out checksum, out size);
                pack.checksum = checksum;
                pack.size = size;
                pack.location = XManifest.Location.Streaming;
                //pack.preloadType = XManifest.PreloadType.Loading;
                List<string> dependencies = new List<string>(manifest.GetAllDependencies(bundleName));
                dependencies.Remove(bundleName);
                pack.dependencies = dependencies.ToArray();

                XManifest.Instance.Add(pack);

                foreach (string file in AssetDatabase.GetAssetPathsFromAssetBundle(bundleName))
                {
                    if (Path.GetExtension(file) == ".unity")    // gen scenes
                    {
                        XManifest.Scene scene = new XManifest.Scene();
                        scene.name = Path.GetFileNameWithoutExtension(file);
                        scene.bundle = bundleName;
                        XManifest.Instance.Add(scene);
                    }
                    else if (Path.GetExtension(file) == ".spriteatlas") // gen sporitepack
                    {
                        var name = Path.GetFileNameWithoutExtension(file);

                        if (XManifest.Instance.GetSpritePack(name) != null)
                        {
                            throw new System.Exception("can't have mutip spriteatlas named:" + file);
                        }
                        List<string> allPackable = new List<string>();
                        var sa = AssetDatabase.LoadAssetAtPath<UnityEngine.U2D.SpriteAtlas>(file);

                        var textureNameMap = new Dictionary<string, bool>();
                        foreach (var pa in UnityEditor.U2D.SpriteAtlasExtensions.GetPackables(sa))
                        {
                            var assetPath = AssetDatabase.GetAssetPath(pa.GetInstanceID());
                            assetPath = assetPath.Replace(Defines.AssetBundleSourcePath + "/", null);
                            assetPath = Path.ChangeExtension(assetPath, null);

                            allPackable.Add(assetPath);
                        }

                        XManifest.SpritePack spritePack = new XManifest.SpritePack();
                        spritePack.name = name;
                        spritePack.packable = allPackable.ToArray();
                        var realName = file.Replace(Defines.AssetBundleSourcePath + "/", null).ToLower().Replace("\\", "/");
                        if (System.IO.Path.HasExtension(realName))
                            realName = System.IO.Path.ChangeExtension(realName, null);

                        spritePack.realName = realName;

                        XManifest.Instance.Add(spritePack);
                    }
                }

                EditorUtility.DisplayProgressBar("Generate Manifest", "compute bundle hash", i / (float)bundles.Length);
            }

            // record luas
            List<string> luaFiles = new List<string>();
            GetAllFile(new DirectoryInfo(Defines.LuaByteCodeOutPath), luaFiles);
            foreach (string fileName in luaFiles)
            {
                string name = Path.GetFileNameWithoutExtension(fileName);
                if (!XManifest.Instance.Exists(name))
                {
                    XManifest.File file = new XManifest.File();
                    file.name = name;
                    file.location = XManifest.Location.Streaming;
                    uint checksum = 0;
                    uint size = 0;
                    GetFileInfo(fileName, out checksum, out size);
                    file.checksum = checksum;
                    file.size = size;
                    XManifest.Instance.Add(file);
                }
                else
                {
                    Debug.LogError("[GenerateFileAllocationTable] repeat name:" + name);
                }
            }

            // gen rules
            BundleNameHelper.initPackRules();
            foreach (BundlePackRule rule in BundleNameHelper.rules)
            {
                XManifest.Instance.AddBundleRule(rule);
            }

            GameConfig.VersionInfo version = new GameConfig.VersionInfo(VersionCode);

            XManifestSave(manifestSavePath, true);
            Debug.Log("[GenerateFileAllocationTable] Mainfest 生成成功：" + manifestSavePath);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

        EditorUtility.DisplayProgressBar("Generate Manifest", "success!", 1.0f);

        bundle.Unload(true);
        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
    }

    static void XManifestSave(string path, bool pretty = true)
    {
        GameConfig.VersionInfo version = new GameConfig.VersionInfo(VersionCode);
        XManifest.Instance.Save(path, pretty, version.MainVersion, version.SubVersion);

    }

    static void _PrepareResources(string outPath, bool forUpdate)
    {
        bool zipLua = false;
        bool pertty = true;
        bool copyManifest = false;
        if (forUpdate)
        {
            zipLua = true;
            pertty = false;
            copyManifest = true;
        }

        XManifest.Instance.Load(Path.Combine(XManifest.resOutputPath, XManifest.name).Replace("\\", "/"));

        int total = 0;
        total += (XManifest.Instance.GetPacks() as Dictionary<string, XManifest.Pack>).Count;
        total += (XManifest.Instance.GetFiles() as Dictionary<string, XManifest.File>).Count;
        int current = 0;

        //copy bundles
        string bundleSrcPath = XManifest.resOutputPath.Replace("\\", "/");
        string bundleAimPath = Path.Combine(outPath, AssetBundleManager.bundleFolder);

        if (Directory.Exists(bundleAimPath))
            Directory.Delete(bundleAimPath, true);
        Directory.CreateDirectory(bundleAimPath);

        foreach (KeyValuePair<string, XManifest.Pack> item in XManifest.Instance.GetPacks())
        {
            XManifest.Pack pack = item.Value;
            File.Copy(Path.Combine(bundleSrcPath, pack.name), Path.Combine(bundleAimPath, Path.ChangeExtension(pack.name, null) + "." + pack.checksum), true);

            EditorUtility.DisplayProgressBar("Prepare Bundle Resource", pack.name, current++ / (float)total);
        }

        // copy luas
        string luaScrPath = Defines.LuaByteCodeOutPath;
        string luaAimPath = Path.Combine(outPath, LuaFilePicker.fileFolder);
        if (Directory.Exists(luaAimPath))
            Directory.Delete(luaAimPath, true);
        Directory.CreateDirectory(luaAimPath);

        foreach (KeyValuePair<string, XManifest.File> item in XManifest.Instance.GetFiles())
        {
            XManifest.File file = item.Value;
            var srcName = Path.Combine(luaScrPath, file.name);
            var srcPath = Path.Combine(luaScrPath, file.name);
            var aimName = Path.ChangeExtension(file.name, null) + "." + file.checksum;
            var aimPath = Path.Combine(luaAimPath, aimName);

            File.Copy(srcPath, aimPath, true);

            EditorUtility.DisplayProgressBar("Prepare Bundle Resource", file.name, current++ / (float)total);
        }

        // copy/save manifest
        if (copyManifest)
        {
            uint manifestChecksum = 0;
            uint manifestSize = 0;
            var manifestAimPath = Path.Combine(outPath, XManifest.name);
            //XManifest.Instance.Save(manifestAimPath, pertty);
            XManifestSave(manifestAimPath, pertty);

            GetFileInfo(manifestAimPath, out manifestChecksum, out manifestSize);
            string destName = manifestAimPath + ("." + manifestChecksum);
            File.Copy(manifestAimPath, destName);
        }
        else
        {
            //XManifest.Instance.Save(Path.Combine(outPath, XManifest.name), pertty);
            XManifestSave(Path.Combine(outPath, XManifest.name), pertty);

        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    public static void GenerateVersionInfo(BuildTarget buildTarget, int versionCode = 0)
    {
        if (versionCode == 0)
            versionCode = GameVersion.code;

        GameConfig.VersionInfo version = new GameConfig.VersionInfo(versionCode);

        string mainifestPath = Path.Combine(XManifest.updateRenameResOutputPath, XManifest.name);

        if (File.Exists(mainifestPath))
        {
            version.ManifestChecksum = GHelper.ComputeFileChecksum(mainifestPath);
        }

        version.ResRoot = "http://cdn-tf02.dev.tapenjoy.com/";
        version.ResRootPublish = "http://lf3-ma18cdn-cn.dailygn.com/obj/light-game-cn/ma18/tf02/";

        string content = LitJson.JsonMapper.ToJson(version);
        Debug.Log(string.Format("version content: {0}", content));

        string path = Path.Combine(Environment.CurrentDirectory, string.Format("build/ver/{0}", XManifest.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget)));
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        Directory.CreateDirectory(path);

        string name = Path.Combine(path, "ver.txt");
        Debug.Log(String.Format("version file save at: {0}", name));

        try
        {
            FileStream fs = new FileStream(name, FileMode.Create);
            System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding(false);
            StreamWriter sw = new StreamWriter(fs, utf8);
            sw.Write(content);
            sw.Close();
            fs.Close();
        }
        catch (IOException e)
        {
            Debug.Log(e.Message);
        }

        //GenerateShareConfigJson(version.GetVersionCode());
        GenerateInnerVersionInfo(buildTarget, versionCode);
    }

    public static void GenerateInnerVersionInfo(BuildTarget buildTarget, int versionCode = 0)
    {
        if (versionCode == 0)
            versionCode = GameVersion.code;

        GameConfig.VersionInfo version = new GameConfig.VersionInfo(versionCode);

        string mainifestPath = Path.Combine(XManifest.updateRenameResOutputPath, XManifest.name);

        if (File.Exists(mainifestPath))
        {
            version.ManifestChecksum = GHelper.ComputeFileChecksum(mainifestPath);
        }

        version.ResRoot = "http://cdn-tf02.dev.tapenjoy.com/";
        version.ResRootPublish = "http://lf3-ma18cdn-cn.dailygn.com/obj/light-game-cn/ma18/tf02/";

        string content = LitJson.JsonMapper.ToJson(version);
        Debug.Log(string.Format("version content: {0}", content));

        string path = Path.Combine(Environment.CurrentDirectory, string.Format("build/ver_inner/{0}", XManifest.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget)));
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        Directory.CreateDirectory(path);

        string name = Path.Combine(path, "ver.txt");
        Debug.Log(String.Format("version file save at: {0}", name));

        try
        {
            FileStream fs = new FileStream(name, FileMode.Create);
            System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding(false);
            StreamWriter sw = new StreamWriter(fs, utf8);
            sw.Write(content);
            sw.Close();
            fs.Close();
        }
        catch (IOException e)
        {
            Debug.Log(e.Message);
        }
    }

    public static void GenerateShareConfigJson(int versionCode)
    {
        GameConfig.VersionInfo version = new GameConfig.VersionInfo(versionCode);
        Hashtable ht = new Hashtable();
        ht.Add("androidSchema", "fruitapp://com.guangyue.tf02.android");
        ht.Add("link", "https://cdn-tf02-tt.tapenjoy.com/fruit_tower/open");
        ht.Add("iosSchema", "fruitapp://");
        ht.Add("androidDownload", "http://lf3-ma18cdn-cn.dailygn.com/obj/light-game-cn/ma18/tf02/android/tower_v" + version.MainVersion + "_" + version.SubVersion + ".apk");
        ht.Add("iosDownload", "http://itunes.apple.com/cn/app/id1520151694?mt=8");
        string result = MiniJSON.jsonEncode(ht);
        File.WriteAllText("share/cfg_share.json", result);
    }
}
