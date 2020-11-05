using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    public static readonly int MAP_REGION_COUNT_X = 8;
    public static readonly int MAP_REGION_COUNT_Y = 8;
    public static readonly int MAP_SECTION_COUNT_X = 8;
    public static readonly int MAP_SECTION_COUNT_Y = 8;
    public static readonly int MAP_TILE_COUNT_X = 8;
    public static readonly int MAP_TILE_COUNT_Y = 8;
    public static readonly int MAP_BLOCK_COUNT_X = 10;
    public static readonly int MAP_BLOCK_COUNT_Y = 10;

    public Transform sceneCamera;

    public GameObject mapLevel0;
    public GameObject mapLevel1;
    public GameObject mapLevel2;

    public float scaleValue0 = 10;
    public float scaleValue1 = 20;

    MapRegion[] regions = new MapRegion[MAP_REGION_COUNT_X * MAP_REGION_COUNT_Y];

    public enum MAP_CELL_TYPE
    {
        Terrain =       0x00000000,

        Building =      0x10000000,


    }


    void Start()
    {
        
    }


    void LateUpdate()
    {
        if(sceneCamera.position.y <= scaleValue0)
        {
            mapLevel0.SetActive(true);
            mapLevel1.SetActive(false);
            mapLevel2.SetActive(false);
        }
        else if (sceneCamera.position.y <= scaleValue1)
        {
            mapLevel0.SetActive(false);
            mapLevel1.SetActive(true);
            mapLevel2.SetActive(false);
        }
        else
        {
            mapLevel0.SetActive(false);
            mapLevel1.SetActive(false);
            mapLevel2.SetActive(true);
        }
    }

}
