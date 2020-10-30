using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
public class BundleNameHelper : AssetPostprocessor
{
    private static List<BundlePackRule> _allRules = new List<BundlePackRule>();
    private static Dictionary<string, uint> _artTex = new Dictionary<string, uint>();
    private static bool _artTexCollected = false;

    public static List<BundlePackRule> rules { get { return _allRules; } }
    public static void initPackRules()
    {
        _allRules.Clear();
        _artTex.Clear();
        _artTexCollected = false;

        //Art目录中图集
        _allRules.Add(new BundlePackRule("Tex/", BundlePackRuleDefine.PackToRootFolder));
        //根据情况在这里配置命名规则，会自动对Defines.AssetBoundleSourcePath目录下的所有文件设置boudle名
        _allRules.Add(new BundlePackRule("scene/", BundlePackRuleDefine.PackOneByOne));
        _allRules.Add(new BundlePackRule("font/", BundlePackRuleDefine.PackOneByOne));
        _allRules.Add(new BundlePackRule("tower/", BundlePackRuleDefine.PackOneByOne));
        _allRules.Add(new BundlePackRule("enemy/", BundlePackRuleDefine.PackOneByOne));
        _allRules.Add(new BundlePackRule("ui/", BundlePackRuleDefine.PackOneByOne));

        _allRules.Add(new BundlePackRule("sound/", BundlePackRuleDefine.PackToParentFolder));
        _allRules.Add(new BundlePackRule("effect/chouka", BundlePackRuleDefine.PackToSelfFolderByUnderline));
        _allRules.Add(new BundlePackRule("effect/bullet", BundlePackRuleDefine.PackToSelfFolderByUnderline));
        _allRules.Add(new BundlePackRule("effect/", BundlePackRuleDefine.PackToParentFolder));
        _allRules.Add(new BundlePackRule("image/", BundlePackRuleDefine.PackToParentFolder));
    }

    [MenuItem("Build/Check/检测设置所有bundle")]
    public static void CheckAndSetAllBundleName()
    {
        initPackRules();

        var parentPath = Path.GetFullPath(Application.dataPath);
        var imagePath = Path.GetFullPath(Defines.SpritePackerSourceImagePath);
        FileInfo[] allFiles = new DirectoryInfo("Assets/").GetFiles("*.*", SearchOption.AllDirectories);
        uint i = 0;
        EditorUtility.ClearProgressBar();
        foreach (FileInfo file in allFiles)
        {
            EditorUtility.DisplayProgressBar("检测资源", "" + file, i * 1f / allFiles.Length);
            ++i;
            if (file.FullName.Contains(".DS_Store"))
                continue;
            if (file.Extension != ".meta" && file.Extension != "")
            {
                var abName = CalABNameByPath(file.FullName);
                var fileName = Path.GetFileName(file.FullName);
                var bundleName = Path.GetFileNameWithoutExtension(abName);
                var subPath = file.FullName.Substring(parentPath.Length + 1, file.FullName.Length - parentPath.Length - 1);

                Debug.LogFormat("{0}=>{1}=>{2}",file.FullName, subPath, abName);

                AssetImporter assetImporter = AssetImporter.GetAtPath("Assets/" + subPath);
                if (file.FullName.StartsWith(imagePath) && assetImporter is TextureImporter)
                {
                    if (assetImporter != null && (assetImporter.assetBundleName != bundleName || ((assetImporter as TextureImporter).spritePackingTag != bundleName)))
                    {
                        Debug.LogFormat("<color=yellow>[CheckAllBundleName] 设置/修改 {0} to {1} tag:{2}</color>", file, abName, abName);
                        (assetImporter as TextureImporter).spritePackingTag = bundleName;
                        assetImporter.SetAssetBundleNameAndVariant(bundleName, Defines.AssetBoundleSuffix);
                        assetImporter.SaveAndReimport();
                    }
                }
                else
                {
                    bundleName = abName.Trim();
                    if (assetImporter != null && assetImporter.assetBundleName != bundleName)
                    {
                        Debug.LogFormat("<color=yellow>[CheckArtBundleName] 设置/修改 {0} to {1}</color>", file, bundleName);
                        if (abName.Length != bundleName.Length)
                        {
                            Debug.LogErrorFormat("{0} fail!", file);
                        }
                        else
                        {

                            assetImporter.SetAssetBundleNameAndVariant(bundleName, Defines.AssetBoundleSuffix);
                            assetImporter.SaveAndReimport();
                        }
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();    //必须刷新一次，不然导ab包时用的是缓存文件，内容没有改变
        Debug.Log("<color=yellow>CheckAllBundleName Done.</color>");
    }

    // AssetBoundleSourcePath/a/b.txt 应该传入参数 a.b  同 resource.load一样 ，不用加后缀
    public static string CalABNameByPath(string path)
    {
        string _name = CalArtABNameByPath(path);
        if (!string.IsNullOrEmpty(_name)) {
            return _name;
        }
        var parentPath = Path.GetFullPath(Defines.AssetBundleSourcePath);
        path = Path.GetFullPath(path);
        if (!path.StartsWith(parentPath))
        {
            return "";
        }
        var subPath = path.Substring(parentPath.Length + 1, path.Length - parentPath.Length - 1);

        subPath = Path.Combine(Path.GetDirectoryName(subPath), Path.GetFileNameWithoutExtension(subPath));
        subPath = subPath.Replace('\\', '/');
        path = subPath;

        var abName = BundlePackRule.CalculateBundleNameByPath(path, _allRules);
        if (string.IsNullOrEmpty(abName))
        {
            return ""; //不返回null，因为未命名的asset其assetImporter 返为""
        }
        else
        {
            var suffix = Defines.AssetBoundleSuffix;
            if (!string.IsNullOrEmpty(suffix))
            {
                suffix = "." + suffix;
            }
            return abName + suffix;
        }
    }

    [MenuItem("Build/Check/检测设置Art bundle")]
    public static void CheckAndSetArtBundleName()
    {
        initPackRules();

        var parentPath = Path.GetFullPath(Application.dataPath);
        FileInfo[] allFiles = new DirectoryInfo(Defines.AssetArtEffectTexSourcePath + "/").GetFiles("*.*", SearchOption.AllDirectories);
        uint i = 0;
        EditorUtility.ClearProgressBar();
        List<string> bundleNameList = new List<string>();
        foreach (FileInfo file in allFiles)
        {
            EditorUtility.DisplayProgressBar("检测资源", "" + file, i * 1f / allFiles.Length);
            ++i;
            if (file.FullName.Contains(".DS_Store"))
                continue;
            if (file.Extension != ".meta" && file.Extension != "")
            {
                var abName = CalArtABNameByPath(file.FullName);
                var bundleName = abName.Trim();
                var subPath = file.FullName.Substring(parentPath.Length + 1, file.FullName.Length - parentPath.Length - 1);

                Debug.LogFormat("{0}=>{1}=>{2}", file.FullName, subPath, bundleName);

                AssetImporter assetImporter = AssetImporter.GetAtPath("Assets/" + subPath);
                if (assetImporter != null && assetImporter.assetBundleName != bundleName)
                {
                    Debug.LogFormat("<color=yellow>[CheckArtBundleName] 设置/修改 {0} to {1}</color>", file, bundleName);
                    if (abName.Length != bundleName.Length) {
                        Debug.LogErrorFormat("{0} fail!", file);
                    } else
                    {

                    assetImporter.SetAssetBundleNameAndVariant(bundleName, Defines.AssetBoundleSuffix);
                    assetImporter.SaveAndReimport();
                    }
                }
                if (!string.IsNullOrEmpty(bundleName) && bundleNameList.IndexOf(bundleName) == -1)
                    bundleNameList.Add(bundleName);
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();    //必须刷新一次，不然导ab包时用的是缓存文件，内容没有改变
        Debug.Log("<color=yellow>CheckArtBundleName Done.</color>");
        foreach (var name in bundleNameList)
            Debug.Log(name);
    }

    //Art目录只设置Effect/Tex
    public static string CalArtABNameByPath(string path)
    {
        var parentPath = Path.GetFullPath(Defines.AssetArtEffectSourcePath);
        path = Path.GetFullPath(path);
        if (!path.StartsWith(parentPath))
        {
            return "";
        }
        if (path.EndsWith(".mat") || path.EndsWith(".png"))
        {
        } else
        {
            return "";
        }
        var appPath = Path.GetFullPath(Application.dataPath);
        string src_guid = AssetDatabase.AssetPathToGUID("Assets/"+path.Substring(appPath.Length + 1, path.Length - appPath.Length - 1));
        var subPath = path.Substring(parentPath.Length + 1, path.Length - parentPath.Length - 1);
        subPath = Path.Combine(Path.GetDirectoryName(subPath), Path.GetFileNameWithoutExtension(subPath));
        subPath = subPath.Replace('\\', '/');
        var abName = BundlePackRule.CalculateBundleNameByPath(subPath, _allRules);
        if (string.IsNullOrEmpty(abName))
        {
            return ""; //不返回null，因为未命名的asset其assetImporter 返为""
        }
        else
        {
            if (!_artTexCollected)
            {
                var prefabFiles = Directory.GetFiles(Defines.AssetArtEffectPrefabSourcePath, "*.*", SearchOption.AllDirectories);
                foreach (var file in prefabFiles)
                {
                    string[] dependenciesFiles = AssetDatabase.GetDependencies(file, true);
                    foreach (var name in dependenciesFiles)
                    {
                        if (name.StartsWith(Defines.AssetArtEffectTexSourcePath) && (name.EndsWith(".mat") || name.EndsWith(".png")))
                        {
                            string guid = AssetDatabase.AssetPathToGUID(name);
                            if (!_artTex.ContainsKey(guid))
                                _artTex[guid] = 1;
                            else
                                ++_artTex[guid];
                        }
                    }
                }
                _artTexCollected = true;
            }
            if (!_artTex.ContainsKey(src_guid) || _artTex[src_guid] <= 1 )
            {
                return "";
            }
            var suffix = Defines.AssetBoundleSuffix;
            if (!string.IsNullOrEmpty(suffix))
            {
                suffix = "." + suffix;
            }
            return abName + suffix;
        }
    }

    // [MenuItem("Build/Check/图集引用是否重复遗漏")]

    static void AutoChangeBundleName(string asset)
    {
        if (asset.EndsWith(".cs"))
        {
            return;
        }

        AssetImporter importer = AssetImporter.GetAtPath(asset);
        if (importer == null)
        {
            return;
        }

        string newName = "";
        if ((asset.StartsWith(Defines.AssetBundleSourcePath + "/") || asset.StartsWith(Defines.AssetArtEffectTexSourcePath + "/")) && !ProjectWindowUtil.IsFolder(importer.GetInstanceID()))
        {
            newName = BundleNameHelper.CalABNameByPath(asset);
        }

        if (string.IsNullOrEmpty(importer.assetBundleName) && string.IsNullOrEmpty(newName))
        {
            return;
        }

        string bundleName = Path.GetFileNameWithoutExtension(newName);
        if (asset.StartsWith(Defines.SpritePackerSourceImagePath) && importer is TextureImporter)
        {
            if (importer.assetBundleName != bundleName || (importer as TextureImporter).spritePackingTag != bundleName)
            {
                (importer as TextureImporter).spritePackingTag = bundleName;
                importer.SetAssetBundleNameAndVariant(bundleName, Defines.AssetBoundleSuffix);
                importer.SaveAndReimport();
            }
        }
        else
        {
            if (importer.assetBundleName != bundleName)
            {
                importer.SetAssetBundleNameAndVariant(bundleName, Defines.AssetBoundleSuffix);
                importer.SaveAndReimport();
            }
        }
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        initPackRules();

        foreach (string asset in importedAssets)
        {
            AutoChangeBundleName(asset);
            if (asset.StartsWith(Defines.AssetBundleSceneSourcePath + "/") && asset.EndsWith(".unity"))
            {
                List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>();
                bool exisit = false;
                foreach (EditorBuildSettingsScene sceneSetting in EditorBuildSettings.scenes)
                {
                    sceneSetting.enabled = true;
                    newScenes.Add(sceneSetting);
                    if (Path.GetFileName(sceneSetting.path) == Path.GetFileName(asset))
                    {
                        exisit = true;
                        break;
                    }
                }

                if (!exisit)
                {
                    newScenes.Add(new EditorBuildSettingsScene(asset, true));
                    Debug.Log("auto add scene " + asset);
                    EditorBuildSettings.scenes = newScenes.ToArray();
                }
            }
        }

        foreach (string asset in movedAssets)
        {
            AutoChangeBundleName(asset);
        }
    }

    public void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName)
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }
}
