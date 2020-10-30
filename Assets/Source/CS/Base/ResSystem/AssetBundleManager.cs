using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class AssetBundleManager
{
    public static AssetBundleManager Instance { get; internal set; }

    static AssetBundleManager()
    {
        Instance = new AssetBundleManager();
    }

    public const string bundleFolder = "bundle";

#if UNITY_EDITOR
    public static string assetBundleOutputPath = System.IO.Path.Combine(System.Environment.CurrentDirectory,
        string.Format("build/bundle/{0}", XManifest.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget))).Replace("\\", "/");
#endif

	public static string dataBasePath = System.IO.Path.Combine(Application.persistentDataPath, bundleFolder);

    private static string streamBasePath = System.IO.Path.Combine(Application.streamingAssetsPath, bundleFolder).Replace("\\", "/");


    private Dictionary<string, AssetBundleInfo> _bundles = new Dictionary<string, AssetBundleInfo>();
    private List<AssetBundleInfo> _bundleInfos = new List<AssetBundleInfo>();

    public void Initialize()
    {
        foreach (KeyValuePair<string, AssetBundleInfo> item in _bundles)
        {
            AssetBundleInfo info = item.Value;
            if (info.isDone)
                info.bundle.Unload(false);
        }
        _bundles.Clear();
        _bundleInfos.Clear();

        foreach (KeyValuePair<string, XManifest.Pack> item in XManifest.Instance.GetPacks())
        {
            AssetBundleInfo info = new AssetBundleInfo(item.Value);
            _bundles.Add(item.Key, info);
            _bundleInfos.Add(info);
        }

        Resources.UnloadUnusedAssets();
    }

    public void Update()
    {
        _ReleaseBundle(XManifest.CacheType.None);
    }

    public AssetBundleInfo GetAssetBundleInfo(string bundleName)
    {
        AssetBundleInfo loaded = null;
        _bundles.TryGetValue(bundleName, out loaded);
        return loaded;
    }
    public Object LoadAsset(XManifest.AssetInfo info, System.Type type)
    {
        return _DoLoadAsset(info, type);
    }
    public Coroutine LoadAssetAsync(XManifest.AssetInfo info, System.Type type, System.Action<Object> callback)
    {
        return CoroutineHelper.Run(_DoLoadAssetAsync(info, type, callback));
    }

    public Coroutine LoadBundleAsync(string name, System.Action<Object> callback)
    {
        return CoroutineHelper.Run(_DoLoadBundleAsync(name, callback));
    }

    public Coroutine LoadScene(XManifest.Scene info)
    {
        return CoroutineHelper.Run(_DoLoadScene(info));
    }

    public void ReleaseSceneCachedBundleOnSceneSwitch()
    {
        _ReleaseBundle(XManifest.CacheType.InScene);
    }

    private void _ReleaseBundle(XManifest.CacheType cacheType)
    {
        float now = Time.time;
        for (int i = 0; i < _bundleInfos.Count; i++)
        {
            AssetBundleInfo info = _bundleInfos[i];
            if (info.isDone && info.Unused(now))
            {
                if (info.pack.cacheType != cacheType && info.pack.cacheType != XManifest.CacheType.None)
                    continue;
#if !RELEASE
                if (info.Ref() < 0)
                    Debug.LogErrorFormat("AssetManager.Unload bundle:{0} refCount({1}) incorrect!", info.pack.name, info.Ref());
#endif

                info.ResetRef();

                for (int depIndex = 0; depIndex < info.pack.dependencies.Length; depIndex++)
                {
                    string dependence = info.pack.dependencies[depIndex];
                    AssetBundleInfo dependenceInfo = GetAssetBundleInfo(dependence);
                    if (dependenceInfo != null)
                        dependenceInfo.RemoveDepended(info.pack.name);
                }
            }
        }

        for (int i = 0; i < _bundleInfos.Count; i++)
        {
            AssetBundleInfo info = _bundleInfos[i];
            if (info.isDone && info.Unused(now))
            {
                if (info.pack.cacheType != cacheType && info.pack.cacheType != XManifest.CacheType.None)
                    continue;
                if (info.DependedCount() == 0)
                {
                    info.bundle.Unload(false);
                    info.bundle = null;
                    info.isDone = false;

                    Debug.LogFormat("AssetManager.UnloadAssetBundle bundle:{0} unloaded ref:{1} cache type: {2}", info.pack.name, info.Ref(), info.pack.cacheType.ToString());
                }
            }
        }
    }

    private Object _DoLoadAsset(XManifest.AssetInfo info, System.Type type)
    {
        AssetBundleInfo bundleInfo = GetAssetBundleInfo(info.bundle);

        if (!bundleInfo.isDone)
        {
            _LoadAssetBundle(bundleInfo);
        }

        if (bundleInfo.bundle == null)
        {
            return null;
        }
        bundleInfo.IncRef(Time.time);
        bundleInfo.DecRef();
        return bundleInfo.bundle.LoadAsset(System.IO.Path.Combine(Defines.AssetBundleSourcePath, info.fullName), type);
    }

    private AssetBundle _LoadAssetBundle(AssetBundleInfo bundleInfo, bool loadDependence = true)
    {
        string bundleName = bundleInfo.pack.name;
        if (bundleInfo.isDone)
        {
            return bundleInfo.bundle;
        }
        if (loadDependence)
        {
            _LoadDependencies(bundleInfo.pack);
        }

        var assetBundle = AssetBundle.LoadFromFile(bundleInfo.url);

        if (assetBundle != null)
        {
            bundleInfo.bundle = assetBundle;
            bundleInfo.isDone = true;
            Debug.LogFormat("AssetBundleManager._LoadAssetBundle load done bundle: {0}", bundleName);
            return bundleInfo.bundle;
        }
        else
        {
            Debug.LogErrorFormat("AssetBundleManager._LoadAssetBundle can't load bundle: {0}", bundleName);
        }
        return null;
    }

    private void _LoadDependencies(XManifest.Pack pack)
    {
        for (int i = 0; i < pack.dependencies.Length; i++)
        {
            AssetBundleInfo bundleInfo = GetAssetBundleInfo(pack.dependencies[i]);
            if (bundleInfo != null)
            {
                if (!bundleInfo.isDone)
                    _LoadAssetBundle(bundleInfo, false);
                bundleInfo.AddDepended(pack.name);
            }
        }
    }

    private IEnumerator _LoadDependenciesAsync(XManifest.Pack pack)
    {
        for (int i = 0; i < pack.dependencies.Length; i++)
        {
            AssetBundleInfo bundleInfo = GetAssetBundleInfo(pack.dependencies[i]);
            if (bundleInfo != null)
            {
                if (!bundleInfo.isDone)
                    CoroutineHelper.Run(_LoadAssetBundleAsync(bundleInfo, false));
                bundleInfo.AddDepended(pack.name);
            }
        }
        for (int i = 0; i < pack.dependencies.Length; i++)
        {
            AssetBundleInfo bundleInfo = GetAssetBundleInfo(pack.dependencies[i]);
            if (bundleInfo != null)
            {
                while (bundleInfo.isLoading)
                    yield return null;
            }
        }
    }

    private IEnumerator _LoadAssetBundleAsync(AssetBundleInfo bundleInfo, bool loadDependence = true)
    {
        while (bundleInfo.isLoading)
            yield return null;

        if (bundleInfo.isDone)
            yield break;

        Debug.LogFormat("AssetBundleManager._LoadAssetBundleAsync {0}", bundleInfo.url);

        bundleInfo.isLoading = true;
        if (loadDependence)
            yield return CoroutineHelper.Run(_LoadDependenciesAsync(bundleInfo.pack));

        string bundleName = bundleInfo.pack.name;
        string bundleNameV = bundleName;// + "." + bundleInfo.pack.checksum;
        string loadpath = null;

#if !USE_BUNDLE_IN_EDITOR && (UNITY_EDITOR || UNITY_STANDALONE || !PUBLISH || ENABLE_GM)
        string search_path = System.IO.Path.Combine(Application.persistentDataPath, bundleNameV); 
        string search_path2 = System.IO.Path.Combine(Defines.LuaFileSearchPath[2], bundleNameV);
        if (System.IO.File.Exists(search_path))
        {
            loadpath = search_path;
            Debug.LogFormat("AssetBundleManager._LoadAssetBundleAsync search_path {0}", search_path);
        }
        else if(System.IO.File.Exists(search_path2))
        {
            loadpath = search_path2;
            Debug.LogFormat("AssetBundleManager._LoadAssetBundleAsync search_path {0}", search_path);
        }
#endif
        if (loadpath == null)
        {
            loadpath = bundleInfo.url;
        }
        AssetBundleCreateRequest loader = AssetBundle.LoadFromFileAsync(loadpath);
        yield return loader;

        if (bundleInfo.isDone) // loaded by sync method
        {
            if (loader.isDone && loader.assetBundle != null)
            {
                loader.assetBundle.Unload(false);
            }
            yield break;
        }

        if (!loader.isDone)
        {
            bundleInfo.isLoading = false;
            Debug.LogErrorFormat("AssetBundleManager.LoadAssetBundleAsync can't async load bundle: {0} reason: {1}", bundleName, "NOT FOUND!");
        }
        else
        {
            bundleInfo.isLoading = false;
            if (loader.assetBundle != null)
            {
                bundleInfo.bundle = loader.assetBundle;
                bundleInfo.isDone = true;
                Debug.LogFormat("AssetBundleManager.LoadAssetBundleAsync async load done bundle: {0}", bundleName);
            }
            else
            {
                Debug.LogErrorFormat("AssetBundleManager.LoadAssetBundleAsync can't async load bundle: {0}", bundleName);
            }
        }
    }

    private IEnumerator _DoLoadAssetAsync(XManifest.AssetInfo info, System.Type type, System.Action<Object> callback)
    {
        AssetBundleInfo bundleInfo = GetAssetBundleInfo(info.bundle);
        if (!bundleInfo.isDone)
        {
            yield return CoroutineHelper.Run(_LoadAssetBundleAsync(bundleInfo));
        }

        if (bundleInfo.isDone)
        {
            bundleInfo.IncRef(Time.time);

            AssetBundleRequest request = null;
            request = bundleInfo.bundle.LoadAssetAsync(System.IO.Path.Combine(Defines.AssetBundleSourcePath, info.fullName), type);
            yield return request;

            bundleInfo.DecRef();

            callback(request.asset);
        }
    }

    private IEnumerator _DoLoadBundleAsync(string name, System.Action<Object> callback)
    {
        AssetBundleInfo bundleInfo = GetAssetBundleInfo(name);
        if (!bundleInfo.isDone)
        {
            yield return CoroutineHelper.Run(_LoadAssetBundleAsync(bundleInfo));
        }

        if (bundleInfo.isDone)
        {
            bundleInfo.IncRef(Time.time);
            bundleInfo.DecRef();
            callback(null);
        }
    }


    private IEnumerator _DoLoadScene(XManifest.Scene info)
    {
        AssetBundleInfo bundleInfo = GetAssetBundleInfo(info.bundle);
        if (!bundleInfo.isDone)
        {
            yield return CoroutineHelper.Run(_LoadAssetBundleAsync(bundleInfo));
        }

        if (bundleInfo.isDone)
        {
            bundleInfo.IncRef(Time.time);

            yield return SceneManager.LoadSceneAsync(info.name);

            bundleInfo.DecRef();
        }
    }

    public class AssetBundleInfo
    {
        public XManifest.Pack pack;
        public AssetBundle bundle;
        public bool isDone;
        public bool isLoading;
        private int refCount;
        private float lastReadTime;

        private HashSet<string> depended = new HashSet<string>();

        public AssetBundleInfo(XManifest.Pack info)
        {
            pack = info;
            bundle = null;
            isDone = false;
            isLoading = false;
            refCount = 0;
            lastReadTime = 0;
        }

        public int Ref() { return refCount; }
        public void IncRef(float time)
        {
            refCount++;
            lastReadTime = time;
        }
        public void DecRef() { refCount--; }
        public bool Unused(float time)
        {
            return refCount <= 0;
        }
        public void ResetRef()
        {
            refCount = 0;
            lastReadTime = 0;
        }

        public int DependedCount() { return depended.Count; }
        public void AddDepended(string bundleName)
        {
            depended.Add(bundleName);
            lastReadTime = Time.time;
        }

        public void RemoveDepended(string bundleName)
        {
            depended.Remove(bundleName);
        }

        public string url
        {
            get
            {
                switch (pack.location)
                {
                    case XManifest.Location.Data:
                        {
                            return System.IO.Path.Combine(AssetBundleManager.dataBasePath, string.Format("{0}.{1}",pack.name,pack.checksum));
                        }

                    case XManifest.Location.Streaming:
                        {
                            return System.IO.Path.Combine(AssetBundleManager.streamBasePath, string.Format("{0}.{1}",pack.name,pack.checksum));
                        }

                    default:
                        return "";
                }
            }
        }

    }
}

