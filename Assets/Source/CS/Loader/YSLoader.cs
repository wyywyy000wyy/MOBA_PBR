using UnityEngine;
using XLua;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Example : MonoBehaviour
{
    IEnumerator DownloadAndCacheAssetBundle(string uri, string manifestBundlePath)
    {
        //Load the manifest
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestBundlePath);
        AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        //Create new cache
        string today = DateTime.Today.ToLongDateString();
        Directory.CreateDirectory(today);
        Cache newCache = Caching.AddCache(today);

        //Set current cache for writing to the new cache if the cache is valid
        if (newCache.valid)
            Caching.currentCacheForWriting = newCache;

        //Download the bundle
        Hash128 hash = manifest.GetAssetBundleHash("bundleName");
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, hash, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

        //Get all the cached versions
        List<Hash128> listOfCachedVersions = new List<Hash128>();
        Caching.GetCachedVersions(bundle.name, listOfCachedVersions);

        if (!AssetBundleContainsAssetIWantToLoad(bundle))     //Or any conditions you want to check on your new asset bundle
        {
            //If our criteria wasn't met, we can remove the new cache and revert back to the most recent one
            Caching.currentCacheForWriting = Caching.GetCacheAt(Caching.cacheCount);
            Caching.RemoveCache(newCache);

            for (int i = listOfCachedVersions.Count - 1; i > 0; i--)
            {
                //Load a different bundle from a different cache
                request = UnityWebRequestAssetBundle.GetAssetBundle(uri, listOfCachedVersions[i], 0);
                yield return request.SendWebRequest();
                bundle = DownloadHandlerAssetBundle.GetContent(request);

                //Check and see if the newly loaded bundle from the cache meets your criteria
                if (AssetBundleContainsAssetIWantToLoad(bundle))
                    break;
            }
        }
        else
        {
            //This is if we only want to keep 5 local caches at any time
            if (Caching.cacheCount > 5)
                Caching.RemoveCache(Caching.GetCacheAt(1));     //Removes the oldest user created cache
        }
    }

    bool AssetBundleContainsAssetIWantToLoad(AssetBundle bundle)
    {
        return (bundle.LoadAsset<GameObject>("MyAsset") != null);     //this could be any conditional
    }
}
public class Example2 : MonoBehaviour
{
    public static class CacheWithPriority
    {
        public enum ResolutionType
        {
            High,
            Medium,
            Low,
        }
        static readonly Dictionary<ResolutionType, Cache> ResolutionCaches = new Dictionary<ResolutionType, Cache>();

        public static void InitResolutionCaches()
        {
            string highResPath = "HighRes";
            string medResPath = "MedRes";
            string lowResPath = Application.streamingAssetsPath;

            //Create cache paths
            Directory.CreateDirectory(highResPath);
            Directory.CreateDirectory(medResPath);

            //Create the caches and add them to a Dictionary
            ResolutionCaches.Add(ResolutionType.High, Caching.AddCache(highResPath));
            ResolutionCaches.Add(ResolutionType.Medium, Caching.AddCache(medResPath));
            ResolutionCaches.Add(ResolutionType.Low, Caching.AddCache(lowResPath));
        }

        public static void PrioritizeCacheForLoading(ResolutionType resolutionToPrioritize)
        {
            //Move cache to the start of the queue
            Caching.MoveCacheBefore(ResolutionCaches[resolutionToPrioritize], Caching.GetCacheAt(0));
        }

        public static void SetResolutionCacheForWriting(ResolutionType resolutionToWriteTo)
        {
            Caching.currentCacheForWriting = ResolutionCaches[resolutionToWriteTo];
        }
    }

    AssetBundle currentTextureAssetBundle;
    IEnumerator RearrangeCacheOrderExample(string manifestBundlePath)
    {
        CacheWithPriority.InitResolutionCaches();

        //Load the manifest
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestBundlePath);
        AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        //We know we want to start loading from the Low Resolution Cache
        CacheWithPriority.PrioritizeCacheForLoading(CacheWithPriority.ResolutionType.Low);

        //Load the low res bundle from StreamingAssets
        UnityWebRequest lowRequest = UnityWebRequestAssetBundle.GetAssetBundle("lowResBundlePath",
            manifest.GetAssetBundleHash("lowResBundle"), 0);
        yield return lowRequest;
        currentTextureAssetBundle = DownloadHandlerAssetBundle.GetContent(lowRequest);

        //In the background we can start downloading our higher resolution bundles
        StartCoroutine(StartDownloadHigherResolutionBundles(manifest));

        //Do work with low res bundle while the higher resolutions download...
    }

    IEnumerator StartDownloadHigherResolutionBundles(AssetBundleManifest manifest)
    {
        CacheWithPriority.SetResolutionCacheForWriting(CacheWithPriority.ResolutionType.Medium);
        UnityWebRequest medRequest = UnityWebRequestAssetBundle.GetAssetBundle("medResBundleUrl", manifest.GetAssetBundleHash("medResBundle"), 0);
        medRequest.SendWebRequest();

        while (!medRequest.isDone)
            yield return null;
        SwitchTextureBundleTo(CacheWithPriority.ResolutionType.Medium, medRequest);

        //Now you'll be using the medium resolution bundle

        CacheWithPriority.SetResolutionCacheForWriting(CacheWithPriority.ResolutionType.High);
        UnityWebRequest highRequest = UnityWebRequestAssetBundle.GetAssetBundle("highResBundleUrl", manifest.GetAssetBundleHash("highResBundle"), 0);
        highRequest.SendWebRequest();

        while (!highRequest.isDone)
            yield return null;
        SwitchTextureBundleTo(CacheWithPriority.ResolutionType.High, highRequest);

        //Do work with the high resolution bundle now...
    }

    void SwitchTextureBundleTo(CacheWithPriority.ResolutionType typeToSwitchTo, UnityWebRequest request)
    {
        //For performance, we tell the Caching system what cache we want it to search first
        CacheWithPriority.PrioritizeCacheForLoading(typeToSwitchTo);
        //Unload our current texture bundle
        currentTextureAssetBundle.Unload(true);
        //Load the new one from the passed in UnityWebRequest
        currentTextureAssetBundle = DownloadHandlerAssetBundle.GetContent(request);
    }
}

[LuaCallCSharp]
public class YSLoader : MonoBehaviour
{
    public static YSLoader _ins;
    void Start()
    {
        _ins = this;
    }
    public static void init()
    {
        var task = Addressables.InitializeAsync();
        task.WaitForCompletion();
        
    }

    public void DownloadAndCacheAssetBundle(string url, Hash128 hash, System.Action<UnityEngine.Object> onComplete)
    {
        StartCoroutine(_DownloadAndCacheAssetBundle(url, hash, onComplete));
    }

    IEnumerator _DownloadAndCacheAssetBundle(string url, Hash128 hash, Action<UnityEngine.Object> onComplete)
    {
                UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

        onComplete(bundle);
    }

    public static void UpdateCatalogs(string key)
    {
        var op = Addressables.UpdateCatalogs(new List<string>() { key });
        //op.WaitForCompletion();
    }

    public static object LoadAssetAsync(string key, System.Action<UnityEngine.Object> onComplete)
    {
        return Addressables.LoadAssetsAsync<UnityEngine.Object>(key, onComplete);
    }

    public static async void InstantiateAsync(string key, System.Action<UnityEngine.Object> onComplete)
    {
        var task = Addressables.InstantiateAsync(key);
        await task.Task;
        onComplete(task.Result);
    }

    //public static object LoadAssetAsync(string key, System.Action<UnityEngine.Object> onComplete)
    //{
    //    return Addressables.LoadAssetsAsync<UnityEngine.Texture2D>(key, (Texture2D tex)=> { onComplete(tex); });
    //}
}