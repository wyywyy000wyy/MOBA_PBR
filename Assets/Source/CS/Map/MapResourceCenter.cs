using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;

[LuaCallCSharp]
public static class MapResourceCenter 
{


    //public class ResrouceToken
    //{
    //    WeakReference<>
    //}

    class ResourceCache
    {
        public Stack<GameObject> stack;
        GameObject ins;

        public ResourceCache(GameObject obj, int capacity = 1)
        {
            ins = obj;
            stack = new Stack<GameObject>(capacity);
        }

        public void Release(GameObject o)
        {
            stack.Push(o);
        }

        public GameObject New()
        {
            GameObject ret;
            if (stack.Count > 0)
            {
                ret = stack.Pop();
            }
            else
            {
                ret = GameObject.Instantiate(ins);
            }
            return ret;
        }
    }
    

    static Dictionary<int, ResourceConfig> configs = new Dictionary<int, ResourceConfig>();

    static Dictionary<int, ResourceCache> caches = new Dictionary<int, ResourceCache>();

    class ResourceConfig
    {
        public int id;
        public string path;
    }

    public static GameObject GetResource(int resourceId)
    {
        ResourceCache cache = caches[resourceId];
        if(cache == null)
        {
            return null;
        }

        return cache.New();
    }

    public static void PreLoad(int resourceId)
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
            ResourceCache cache = caches[resourceId];
            if(cache != null)
            {
                return;
            }
            cache = new ResourceCache(g);
            caches[resourceId] = cache;
        });
    }

    public static void RegisterConfig(int id, string path)
    {
        ResourceConfig conf = new ResourceConfig();
        conf.id = id;
        conf.path = path;

        configs[id] = conf;
    }
}
