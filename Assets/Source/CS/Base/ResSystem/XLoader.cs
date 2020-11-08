#if UNITY_EDITOR
//#define USE_BUNDLE_IN_EDITOR // test
using UnityEditor;
#endif


using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using XLua;
using System.IO;
using System.Collections.Generic;

[LuaCallCSharp]
public static class XLoader
{
    static Dictionary<string, Object> cacheMap = new Dictionary<string, Object>();

    public static Object GetCache(string path, System.Type t)
    {
        //string key = path + "" + t.ToString();
        //if (cacheMap.ContainsKey(key))
        //{
        //    return cacheMap[key];
        //}
        return null;
    }

    public static void PushCache(string path, System.Type t, Object o)
    {
        //string key = path + "" + t.ToString();
        //if (!cacheMap.ContainsKey(key))
        //{
        //    cacheMap.Add(key, o);
        //}
    }


    public static T Load<T>(string path) where T : Object
    {
        return (Load(path, typeof(T)) as T);
    }

    public static Coroutine LoadAsync<T>(string path, System.Action<Object> callback) where T : Object
    {
        return LoadAsync(path, typeof(T), callback);
    }
    public static Coroutine LoadAsync(string path, System.Type type, System.Action<Object> callback)
    {
        var cache = GetCache(path, type);
        if (cache != null)
        {
            callback(cache);
            return null;
        }
        return _StartCoroutine(_DoLoadAsync(path, type, (o) =>
        {
            PushCache(path, type, o);
            callback(o);
        }));
    }

    public static Coroutine LoadBundleAsync(string name, System.Action<Object> callback)
    {
        return _StartCoroutine(_DoLoadBundleAsync(name, callback));
    }

    public static Object[] LoadMulti(string[] paths)
    {
        Object[] results = new Object[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            results[i] = Load(paths[i], typeof(Object));
        }
        return results;
    }

    public static Coroutine LoadMultiAsync(string[] paths, System.Action<Object[]> callback)
    {
        return _StartCoroutine(_DoLoadMultiAsync(paths, callback));
    }

    public static void LoadScene(string name)
    {
#if UNITY_EDITOR && ! USE_BUNDLE_IN_EDITOR
        SceneManager.LoadScene(name);
#else
        Debug.LogFormat("XLoader.LoadScene loading: {0}", name);
        AssetBundleManager.Instance.ReleaseSceneCachedBundleOnSceneSwitch();

        XManifest.Scene info = XManifest.Instance.GetScene(name);

        if (info != null)
        {
            XManifest.Pack packInfo = XManifest.Instance.GetPack(info.bundle);
            switch (packInfo.location)
            {
                case XManifest.Location.Resource:
                    Debug.LogErrorFormat("XLoader.LoadScene can't load bundled scene {0} in resource!", name);
                    break;
                default:
                    Debug.LogErrorFormat("[XLoader] can't load scene in sync load {0}", name);
                    break;
            }
        }
        else
        {
            SceneManager.LoadScene(name);
        }
#endif
    }

    public static Coroutine LoadSceneAsync(string name, System.Action<bool> callback)
    {
        AssetBundleManager.Instance.ReleaseSceneCachedBundleOnSceneSwitch();
        return CoroutineHelper.Run(_DoLoadSceneAsync(name, callback));
    }
    public static void LoadSpriteAsync(string spritePath, System.Action<Object> callback)
    {
#if UNITY_EDITOR && ! USE_BUNDLE_IN_EDITOR
        LoadAsync(spritePath, typeof(Sprite), callback);
#else
        var type = typeof(UnityEngine.U2D.SpriteAtlas);
        string name = spritePath.ToLower();
        XManifest.AssetInfo info = XManifest.Instance.Find(name);

        if (info != null)
        {
            LoadAsync(spritePath, type, callback);
        }
        else
        {
            var spritepack = XManifest.Instance.FindPackBySprite(spritePath);
            if (spritepack != null)
            {
                LoadAsync(spritepack.realName, type, (obj) =>
                {
                    callback((obj as UnityEngine.U2D.SpriteAtlas).GetSprite(Path.GetFileName(spritePath)));
                });
            }
            else
            {
                Debug.LogErrorFormat("{0} can't find any where", spritePath);
            }
        }
#endif
    }

    public static Object Load(string path, System.Type type)
    {
        Debug.LogFormat("[XLoader] Load: ({0} : {1})", path, type.ToString());

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogErrorFormat("[XLoader.Load] sent empty/null path!");
            return null;
        }
        Object obj = null;
#if UNITY_EDITOR 
        obj = Resources.Load(Path.ChangeExtension(path, null), type);

        if (obj == null)
        {
            obj = _LoadObjInEditorAt(path, type);
        }

        if(obj != null)
        {
            return obj;
        }

        //return obj;
#endif
        //#else
        string name = Path.ChangeExtension(path, null);
        //UnityEngine.Object obj = null;

        //Debug.LogFormat("[XLoader] can't find Asset({0} : {1}) in cache: ", name, type.ToString());
        XManifest.AssetInfo info = XManifest.Instance.Find(name);
        if (info != null)
        {
            switch (info.location)
            {
                case XManifest.Location.Resource:
                    {
                        obj = Resources.Load(name, type);
                    }
                    break;
                case XManifest.Location.Bundle:
                    {
                        obj = AssetBundleManager.Instance.LoadAsset(info, type);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            string bundleName = XManifest.Instance.GetABNameByPath(name);
            if (!string.IsNullOrEmpty(bundleName))
            {
                XManifest.Pack pack = XManifest.Instance.GetPack(bundleName);
                if(pack == null)
                {
                    Debug.LogErrorFormat("[XLoader.Load] can not find bundle pack: [{0}] for res [{1}]",bundleName,name);
                    return null;
                }
                XManifest.Location local = pack.NoBundle() ? XManifest.Location.Resource : XManifest.Location.Bundle;
                XManifest.AssetInfo assetInfo = new XManifest.AssetInfo() { location = local, bundle = pack.name, fullName = path };

                switch (local)
                {
                    case XManifest.Location.Resource:
                        {
                            obj = Resources.Load(name, type);
                        }
                        break;
                    case XManifest.Location.Bundle:
                        {
                            obj = AssetBundleManager.Instance.LoadAsset(assetInfo, type);
                        }
                        break;
                    default:
                        break;
                }

                if (obj != null)
                {
                    XManifest.Instance.AddFastIndex(path, assetInfo);
                }
            }
            else
            {
                obj = Resources.Load(name, type);
            }
        }

        if (obj == null)
        {
            if (info != null)
                Debug.LogErrorFormat("[XLoader] Can't find {0} in Location({1})", name, info.location);
            else
                Debug.LogErrorFormat("[XLoader] Can't find {0} in Resources", name);
        }
        else
        {
            Debug.LogFormat("[XLoader] ({0} : {1}) Loaded.", name, type.ToString());
        }

        return obj;
//#endif
    }

    private static Coroutine _StartCoroutine(IEnumerator em)
    {
        return CoroutineHelper.Run(em);
    }

#if UNITY_EDITOR && ! USE_BUNDLE_IN_EDITOR
    private static Object _LoadObjInEditorAt(string path, System.Type type)
    {
        var fullPath = System.IO.Path.Combine(Defines.AssetBundleSourcePath, path);
        return AssetDatabase.LoadAssetAtPath(fullPath, type);
    }
#endif

    public static void LoadShader(string name)
    {
        AssetBundleManager.Instance.LoadBundleAsync(name, delegate (Object result) { });
    }

    private static IEnumerator _DoLoadAsync(string path, System.Type type, System.Action<Object> callback)
    {

#if UNITY_EDITOR && !USE_BUNDLE_IN_EDITOR
        ResourceRequest request = Resources.LoadAsync(Path.ChangeExtension(path, null), type);
        yield return request;
        Object obj = request.asset;
        if (obj == null)
        {
            obj = _LoadObjInEditorAt(path, type);
        }
        callback(obj);
#else
        //Debug.LogFormat("[XLoader] AsyncLoading: ({0} : {1})", path, type.ToString());
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogErrorFormat("[XLoader.Load] sent empty/null path!");
            yield break;
        }


        string name = Path.ChangeExtension(path, null);
        XManifest.AssetInfo info = XManifest.Instance.Find(name);
        UnityEngine.Object obj = null;
        if (info != null)
        {
            switch (info.location)
            {
                case XManifest.Location.Resource:
                    {
                        ResourceRequest request = Resources.LoadAsync(name, type);
                        yield return request;
                        obj = request.asset;
                    }
                    break;
                case XManifest.Location.Bundle:
                    {
                        yield return AssetBundleManager.Instance.LoadAssetAsync(info, type, delegate (Object result) { obj = result; });
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            string bundleName = XManifest.Instance.GetABNameByPath(name);
            if (!string.IsNullOrEmpty(bundleName))
            {
                XManifest.Pack pack = XManifest.Instance.GetPack(bundleName);
                if(pack == null)
                {
                   Debug.LogErrorFormat("[XLoader.LoadAsync] can not find bundle pack: [{0}] for res [{1}]",bundleName,name);
                    callback(null);
                    yield break;
                }

                XManifest.Location local = pack.NoBundle() ? XManifest.Location.Resource : XManifest.Location.Bundle;
                XManifest.AssetInfo assetInfo = new XManifest.AssetInfo() { location = local, bundle = pack.name, fullName = path };

                switch (local)
                {
                    case XManifest.Location.Resource:
                        {
                            ResourceRequest request = Resources.LoadAsync(name, type);
                            yield return request;
                            obj = request.asset;
                        }
                        break;
                    case XManifest.Location.Bundle:
                        {
                            yield return AssetBundleManager.Instance.LoadAssetAsync(assetInfo, type, delegate (Object result) { obj = result; });
                        }
                        break;
                    default:
                        break;
                }

                if (obj != null)
                {
                    XManifest.Instance.AddFastIndex(path, assetInfo);
                }
            }
            else
            {
                ResourceRequest request = Resources.LoadAsync(name, type);
                yield return request;
                obj = request.asset;
            }
        }


        if (obj == null)
        {
            if (info != null)
                Debug.LogErrorFormat("[XLoader] Can't find {0} in Location({1})", name, info.location);
            else
                Debug.LogErrorFormat("[XLoader] Can't find {0} in Resources", name);
        }

        callback(obj);
#endif
    }

    private static IEnumerator _DoLoadBundleAsync(string bundleName, System.Action<Object> callback)
    {
#if UNITY_EDITOR && !USE_BUNDLE_IN_EDITOR
        callback(null);
        yield break;
#else
        //Debug.LogFormat("[XLoader] AsyncLoading: ({0} : {1})", path, type.ToString());
        if (string.IsNullOrEmpty(bundleName))
        {
            Debug.LogErrorFormat("[XLoader.LoadBundle] sent empty/null name!");
            yield break;
        }

        XManifest.Pack pack = XManifest.Instance.GetPack(bundleName);
        if (pack == null)
        {
            Debug.LogErrorFormat("[XLoader.LoadBundle] can not find bundle pack: [{0}]", bundleName);
            callback(null);
            yield break;
        }

        XManifest.Location local = pack.NoBundle() ? XManifest.Location.Resource : XManifest.Location.Bundle;
        switch (local)
        {
            case XManifest.Location.Bundle:
                {
                    yield return AssetBundleManager.Instance.LoadBundleAsync(bundleName, delegate (Object result) {  });
                }
                break;
            default:
                break;
        }
        callback(null);
#endif
    }

    private static Coroutine _LoadAsync(string path, System.Type type, System.Action<Object, int> callback, int param)
    {
        if (path == null)
        {
            Debug.LogError("path");
        }

        return _StartCoroutine(_DoLoadAsync(path, type, delegate (Object obj)
        {
            callback(obj, param);
        }));
    }


    private static IEnumerator _DoLoadMultiAsync(string[] paths, System.Action<Object[]> callback)
    {
        Object[] results = new Object[paths.Length];
        bool[] loadDone = new bool[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            loadDone[i] = false;
            _LoadAsync(paths[i], typeof(Object), delegate (Object obj, int index)
            {
                results[index] = obj;
                loadDone[index] = true;
            }, i);
        }

        for (int i = 0; i < paths.Length; i++)
            while (!loadDone[i])
                yield return null;

        callback(results);
    }

    private static IEnumerator _DoLoadSceneAsync(string name, System.Action<bool> callback)
    {
#if UNITY_EDITOR && ! USE_BUNDLE_IN_EDITOR
        yield return SceneManager.LoadSceneAsync(name);
#else
        Debug.LogFormat("XLoader.LoadScene loading: {0}", name);
        XManifest.Scene info = XManifest.Instance.GetScene(name);

        if (info != null)
        {
            Debug.LogFormat("XLoader.LoadScene bundle loading: {0}", name);
            XManifest.Pack packInfo = XManifest.Instance.GetPack(info.bundle);
            switch (packInfo.location)
            {
                case XManifest.Location.Resource:
                    Debug.LogErrorFormat("XLoader.LoadScene can't load bundled scene {0} in resource!", name);
                    break;
                default:
                    yield return AssetBundleManager.Instance.LoadScene(info);
                    break;
            }
        }
        else
        {
            Debug.LogFormat("XLoader.LoadScene native loading: {0}", name);
            yield return SceneManager.LoadSceneAsync(name);
        }
#endif
        string loadedName = SceneManager.GetActiveScene().name;
        bool success = loadedName == name ? true : false;
        Debug.LogFormat("XLoader.LoadScene {0} loaded", loadedName);
        callback(success);
    }

    private static void _SpriteAtlasRequested(string tag, System.Action<UnityEngine.U2D.SpriteAtlas> callback)
    {
#if UNITY_EDITOR && !USE_BUNDLE_IN_EDITOR

        var obj = _LoadObjInEditorAt(string.Format("spriteatlas/{0}.spriteatlas", tag), typeof(UnityEngine.U2D.SpriteAtlas));
        if (obj == null)
        {
            //Debug.LogErrorFormat("XLoader._SpriteAtlasRequested : put {0} in folder {1}", tag, Defines.SpriteAltasPath);
        }
        callback(obj as UnityEngine.U2D.SpriteAtlas);
#else
        Debug.Log("XLoader._SpriteAtlasRequested: " + tag);
        XManifest.SpritePack info = XManifest.Instance.GetSpritePack(tag);
        if (info == null)
        {
            Debug.LogErrorFormat("XLoader._SpriteAtlasRequested can't find spriteAtlas {0} in resource!", tag);
        }
        else
        {
            _StartCoroutine(_DoLoadAsync(info.realName, typeof(UnityEngine.U2D.SpriteAtlas), delegate (Object obj)
                {
                    callback(obj as UnityEngine.U2D.SpriteAtlas);
                })
            );
        }
#endif
    }

    public static AsyncOperation UnloadUnusedAssets()
    {
        return Resources.UnloadUnusedAssets();
    }

    public static void Initialize(bool useExternalFolder)
    {
        XManifest.Instance.Initialize(useExternalFolder);
        AssetBundleManager.Instance.Initialize();
        UnityEngine.U2D.SpriteAtlasManager.atlasRequested += _SpriteAtlasRequested;
    }

    [BlackList]
    public static void Update()
    {
        AssetBundleManager.Instance.Update();
    }
}
