using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;

[LuaCallCSharp]
public static class MapResourceCenter 
{
    static Dictionary<int, ResourceConfig> configs = new Dictionary<int, ResourceConfig>();

    static Dictionary<int, MapResourceCache> caches = new Dictionary<int, MapResourceCache>();

    class ResourceConfig
    {
        public int id;
        public string path;
    }

    public static MapResourceProxy GetResource(int resourceId)
    {
        MapResourceCache cache;//= caches[resourceId];
        if(!caches.TryGetValue(resourceId, out cache))
        {
            Debug.LogErrorFormat("not resrouce {0}", resourceId);
            return null;
        }

        return cache.New();
    }

    public static void Start()
    {
        //configs = new Dictionary<int, ResourceConfig>();
        //caches = new Dictionary<int, MapResourceCache>();
    }

    public static void PreLoad(int resourceId, Action cb)
    {
        ResourceConfig conf = configs[resourceId];
        if(conf == null)
        {
            Debug.LogErrorFormat("RreLoad error, not conf {0}", resourceId);
            return;
        }


        XLoader.LoadAsync<GameObject>(conf.path, (UnityEngine.Object go) =>
        {
            GameObject g = go as GameObject;
            if(MapResourceCenter.caches.ContainsKey(resourceId))
            {
                return;
            }

            g.transform.localScale = new Vector3((float)Map.MAP_CELL_DEMANSION_X, (float)Map.MAP_CELL_DEMANSION_Y, (float)Map.MAP_CELL_DEMANSION_X);

            MapResourceCache cache = new MapResourceCache(g);
            Debug.LogFormat("PreLoad {0}", resourceId);
            caches[resourceId] = cache;
            cb();
        });
    }

    public static void RegisterConfig(int id, string path)
    {
        Debug.LogFormat("RegisterConfig {0}", id);
        ResourceConfig conf = new ResourceConfig();
        conf.id = id;
        conf.path = path;

        configs[id] = conf;
    }
}
