using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapResourceCache
{
    Stack<MapResourceProxy> stack;
    GameObject ins;



    public MapResourceCache(GameObject obj, int capacity = 1)
    {
        ins = obj;
        stack = new Stack<MapResourceProxy>(capacity);
    }

    public void Release(MapResourceProxy o)
    {
        o.resource.SetActive(false);
        stack.Push(o);
    }

    public GameObject Require()
    {
        return null;
    }

    public MapResourceProxy New()
    {
        MapResourceProxy ret;
        if (stack.Count > 0)
        {
            ret = stack.Pop();
        }
        else
        {
            ret = new MapResourceProxy(this, GameObject.Instantiate(ins));
        }
        ret.resource.SetActive(false);
        return ret;
    }
}