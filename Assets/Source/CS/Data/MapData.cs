using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MapBlockData 
{
    public int terrain;
    public int landscape;
}

[Serializable]
public class MapTileData
{
    public List<MapBlockData> blockList = new List<MapBlockData>();
}


[Serializable]
[CreateAssetMenuAttribute(fileName = "map_data", menuName = "Game/MapData")]
public class MapData : ScriptableObject
{
    public List<MapTileData> tileList = new List<MapTileData>();
}