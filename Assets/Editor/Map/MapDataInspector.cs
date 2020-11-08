using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapData))]
public class MapDataInspector : Editor
{
    // Start is called before the first frame update
    MapData mapData;

    int blockTerrainCount = 4;
    void OnEnable()
    {
        mapData = (MapData)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        blockTerrainCount = EditorGUILayout.IntField("地块种类数量", blockTerrainCount);

        if (GUILayout.Button("Generate"))
        {
            mapData.tileList.Clear();
            mapData.tileList.Capacity = Map.MAP_TILE_COUNT;
            for (int i = 0; i < Map.MAP_TILE_COUNT; ++i)
            {
                MapTileData td = new MapTileData();
                td.blockList.Capacity = Map.MAP_BLOCK_COUNT;
                for (int j = 0; j < Map.MAP_BLOCK_COUNT; ++j)
                {
                    MapBlockData bd = new MapBlockData();
                    bd.terrain = Random.Range(1, blockTerrainCount);
                    td.blockList.Add(bd);
                }
                mapData.tileList.Add(td);
;            }
            EditorUtility.SetDirty(mapData);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

}
