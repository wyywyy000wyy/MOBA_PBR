using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapResourceProxy
{
    MapResourceCache parent;
    //int proxyId;
    GameObject _resource;


    public GameObject resource
    {
        get { return _resource; }
    }

    public MapResourceProxy(MapResourceCache p, GameObject resource)
    {
        parent = p;
        //this.proxyId = proxyId;
        this._resource = resource;
    }

    public void Release()
    {
        parent.Release(this);
    }

    GameObject requireResource
    {
        get
        {
            if (this.resource != null)
            {
                return resource;
            }


            return resource;
        }
    }
}
