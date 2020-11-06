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
    public static readonly int MAP_BLOCK_COUNT_X = 8;
    public static readonly int MAP_BLOCK_COUNT_Y = 8;
    public static readonly int MAP_CELL_COUNT_X = 10;
    public static readonly int MAP_CELL_COUNT_Y = 10;

    public static readonly int MAP_REGION_COUNT = MAP_REGION_COUNT_X * MAP_REGION_COUNT_Y;
    public static readonly int MAP_SECTION_COUNT = MAP_SECTION_COUNT_X * MAP_SECTION_COUNT_Y;
    public static readonly int MAP_TILE_COUNT = MAP_TILE_COUNT_X * MAP_TILE_COUNT_Y;
    public static readonly int MAP_BLOCK_COUNT = MAP_BLOCK_COUNT_X * MAP_BLOCK_COUNT_Y;
    public static readonly int MAP_CELL_COUNT = MAP_CELL_COUNT_X * MAP_CELL_COUNT_Y;

    public static readonly int MAP_CELL_DEMANSION_X = 1;
    public static readonly int MAP_CELL_DEMANSION_Y = 1;

    public static readonly int MAP_BLOCK_DEMANSION_X= MAP_CELL_DEMANSION_X * MAP_CELL_COUNT_X;
    public static readonly int MAP_BLOCK_DEMANSION_Y= MAP_CELL_DEMANSION_Y * MAP_CELL_COUNT_Y;
    public static readonly int MAP_TILE_DEMANSION_X = MAP_BLOCK_DEMANSION_X * MAP_BLOCK_COUNT_X;
    public static readonly int MAP_TILE_DEMANSION_Y = MAP_BLOCK_DEMANSION_Y * MAP_BLOCK_COUNT_Y;

    public static readonly int MAP_SECTION_DEMANSION_X = MAP_TILE_DEMANSION_X * MAP_TILE_COUNT_X;
    public static readonly int MAP_SECTION_DEMANSION_Y = MAP_TILE_DEMANSION_Y * MAP_TILE_COUNT_Y;

    public static readonly int MAP_REGION_DEMANSION_X = MAP_SECTION_DEMANSION_X * MAP_SECTION_COUNT_X;
    public static readonly int MAP_REGION_DEMANSION_Y = MAP_SECTION_DEMANSION_Y * MAP_SECTION_COUNT_Y;

    public static readonly int MAP_DEMANSION_X = MAP_REGION_DEMANSION_X * MAP_REGION_COUNT_X;
    public static readonly int MAP_DEMANSION_Y = MAP_REGION_DEMANSION_Y * MAP_REGION_COUNT_Y;


    public static readonly int MAP_START_X = -MAP_DEMANSION_X / 2;
    public static readonly int MAP_START_Y = -MAP_DEMANSION_Y / 2;


    static Map ins;

    public static Map Instance
    {
        get { return ins; }
    }

    public Transform sceneCameraTrans;
    Camera sceneCamera;
    Plane plane;
    Ray ray;

    public GameObject mapLevel0;
    public GameObject mapLevel1;
    public GameObject mapLevel2;

    public float scaleValue0 = 10;
    public float scaleValue1 = 20;

    Vector3 targetPos;
    Vector3 tpos = new Vector3(0,0,0);

    MapRegion[] regions = new MapRegion[MAP_REGION_COUNT_X * MAP_REGION_COUNT_Y];


    public enum MAP_CELL_TYPE
    {
        Terrain =       0x00000000,

        Building =      0x10000000,


    }


    void Start()
    {
        sceneCamera = sceneCameraTrans.GetComponent<Camera>();
        plane = new Plane();
        DontDestroyOnLoad(this);
        ins = this;
    }

    void Update()
    {
        Vector3 cp = sceneCameraTrans.position;

        tpos.z = cp.y;
        targetPos = cp + tpos;

        int dl = GetDisplayLevel();

        //plane.Raycast()
        //ray.origin = cp;
        //ray.direction = sceneCameraTrans.forward;

        //plane.Raycast(ray, )

        //    local ray = U.Camera.main:ScreenPointToRay(U.Vector3(screen_pos.x, screen_pos.y, 0))
        //local hit, dis = plane:Raycast(ray)
    }

    int GetDisplayLevel()
    {
        if (sceneCameraTrans.position.y <= scaleValue0)
        {
            return 0;
        }
        else if (sceneCameraTrans.position.y <= scaleValue1)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    void ShowAtPos(Vector3 targetPos)
    {

    }

    void LateUpdate()
    {
        //if(sceneCameraTrans.position.y <= scaleValue0)
        //{
        //    mapLevel0.SetActive(true);
        //    mapLevel1.SetActive(false);
        //    mapLevel2.SetActive(false);
        //}
        //else if (sceneCameraTrans.position.y <= scaleValue1)
        //{
        //    mapLevel0.SetActive(false);
        //    mapLevel1.SetActive(true);
        //    mapLevel2.SetActive(false);
        //}
        //else
        //{
        //    mapLevel0.SetActive(false);
        //    mapLevel1.SetActive(false);
        //    mapLevel2.SetActive(true);
        //}
    }

    private class MapCache<T> where T : new()
    {
        public Stack<T> stack;

        public MapCache(int capacity=1)
        {
            stack = new Stack<T>(capacity);
        }

        public void Release(T o)
        {
            stack.Push(o);
        }

        public T New()
        {
            T ret;
            if(stack.Count > 0)
            {
                ret = stack.Pop();
            }
            else
            {
                ret = new T();
            }
            return ret;
        }
    }

    MapCache<MapRegion> regionCache = new MapCache<MapRegion>();
    MapCache<MapSection> sectionCache = new MapCache<MapSection>();
    MapCache<MapTile> tileCache = new MapCache<MapTile>();
    MapCache<MapBlock> blockCache = new MapCache<MapBlock>();

    public MapSection NewMapSection()
    {
        return sectionCache.New();
    }

    public void ReleaseMapSection(MapSection sect)
    {
        sectionCache.Release(sect);
    }

    public T GetMapObject<T>() where T: new()
    {
        return new T();
    }
}
