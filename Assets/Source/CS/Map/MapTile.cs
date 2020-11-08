using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile
{
    public Vector3 basePos = new Vector3();

    public MapTile(int id, MapTileData data)
    {
        tile_id = id;
        this.data = data;
    }


    int tile_id;
    MapTileData data;
    
    public MapBlock[] blocks;//= new MapRegion[MAP_REGION_COUNT_X * MAP_REGION_COUNT_Y];



}
