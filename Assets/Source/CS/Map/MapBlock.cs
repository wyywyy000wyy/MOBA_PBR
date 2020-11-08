﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MapContainerObject
{
    // Start is called before the first frame update
    Map.MAP_CELL_TYPE[] cells;

    public Vector3 basePos = new Vector3();

    MapTile parentTile;
    int block_id;
    MapBlockData data;

    GameObject terrain;

    List<MapResourceProxy> resources = new List<MapResourceProxy>();


    GameObject GetResource(int resrouce_id)
    {
        MapResourceProxy res = MapResourceCenter.GetResource(resrouce_id);
        resources.Add(res);
        return res.resource;
    }



    public MapBlock(MapTile tile, int id,MapBlockData data)
    {
        parentTile = tile;
        block_id = id;
        this.data = data;
    }

    public void ReleaseResource()
    {
        for(int i = resources.Count - 1; i >= 0; --i)
        {
            resources[i].Release();
            resources.RemoveAt(i);
        }
    }

    public void Show()
    {
        if(terrain == null)
        {
            terrain = GetResource(data.terrain);
            terrain.transform.position = basePos;
        }
        terrain.SetActive(true);
    }

    public void ShowDetail()
    {
        
    }

    public void HideDetail()
    {

    }

    public void Hide()
    {
        if(terrain)
        {
            terrain.SetActive(false);
        }
    }
}
