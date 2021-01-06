using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
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
    public static readonly int MAP_CELL_COUNT_X = 20;
    public static readonly int MAP_CELL_COUNT_Y = 20;

    public static readonly int MAP_BLOCK_COUNT_TOTAL_X = MAP_BLOCK_COUNT_X * MAP_TILE_COUNT_X;
    public static readonly int MAP_BLOCK_COUNT_TOTAL_Y = MAP_BLOCK_COUNT_Y * MAP_TILE_COUNT_Y;

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

    public static readonly int MAP_DEMANSION_X = MAP_TILE_DEMANSION_X * MAP_TILE_COUNT_X;
    public static readonly int MAP_DEMANSION_Y = MAP_TILE_DEMANSION_Y * MAP_TILE_COUNT_Y;


    public static readonly int MAP_START_X = 0;//-MAP_DEMANSION_X / 2;
    public static readonly int MAP_START_Y = 0;//-MAP_DEMANSION_Y / 2;


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

    public float scaleValue0 = 20;
    public float scaleValue1 = 40;

    Vector3 targetPos;
    Vector3 tpos = new Vector3(0,0,0);

    //MapRegion[] regions = new MapRegion[MAP_REGION_COUNT_X * MAP_REGION_COUNT_Y];
    MapTile[] tiles = new MapTile[MAP_TILE_COUNT];

    MapData md;
    public enum MAP_CELL_TYPE
    {
        Terrain =       0x00000000,

        Building =      0x10000000,


    }

    MapBlock[] blocks = new MapBlock[MAP_BLOCK_COUNT * MAP_TILE_COUNT];

    
    Vector2Int[] LeftTop = new Vector2Int[3];
    Vector2Int[] BottomRight = new Vector2Int[3];

    Vector2Int LeftTopOld = new Vector2Int();
    Vector2Int BottomRightOld = new Vector2Int();

    bool run = false;

    //BlockResource[] blocks;

    static readonly int MAX_BLOCK = 10 * 10;

    int[]  viewX =  { 2, 4 ,8};
    int[] viewY = { 2, 4 , 8};
    int[] viewY2 = { 2, 2, 2 };
    int curBlock = -1;
    int showBlocks = 0;
    int curLevel = 1;
    int preLvel = -1;

    bool in_view(int x, int y)
    {
        if (LeftTop[curLevel].x > x || BottomRight[curLevel].x < x)
            return false;
        if (LeftTop[curLevel].y > y || BottomRight[curLevel].y < y)
            return false;
        return true;
    }

    public void Move(int x, int y, int level = 0)
    {

        LeftTop[curLevel].x = Mathf.Clamp(x-viewX[curLevel], 0, MAP_BLOCK_COUNT_TOTAL_X - 1);
        LeftTop[curLevel].y = Mathf.Clamp(y- viewY2[curLevel], 0, MAP_BLOCK_COUNT_TOTAL_Y - 1);


        BottomRight[curLevel].x = Mathf.Clamp(x + viewX[curLevel], 0, MAP_BLOCK_COUNT_TOTAL_X - 1);
        BottomRight[curLevel].y = Mathf.Clamp(y + viewY[curLevel], 0, MAP_BLOCK_COUNT_TOTAL_Y - 1);

        for(int ry = LeftTopOld.y; ry <= BottomRightOld.y; ++ry)
        {
            for(int rx = LeftTopOld.x; rx <= BottomRightOld.x; ++rx)
            {
                if(!in_view(rx, ry))
                {
                    int block_id = ry * MAP_BLOCK_COUNT_TOTAL_X + rx;
                    ReleaseResource(block_id);
                }
            }
        }

        Debug.LogFormat("LeftTopOld ={0},{1} BottomRightOld = {2},{3}", LeftTopOld.x, LeftTopOld.y, BottomRightOld.x, BottomRightOld.y);
        Debug.LogFormat("LeftTop[curLevel] ={0},{1} BottomRight[curLevel] = {2},{3}", LeftTop[curLevel].x, LeftTop[curLevel].y, BottomRight[curLevel].x, BottomRight[curLevel].y);

 
        showBlocks = (BottomRight[curLevel].x - LeftTop[curLevel].x) * (BottomRight[curLevel].y - LeftTop[curLevel].y);

        int startX = Mathf.Max(0, x - viewX[curLevel]);
        int startY = Mathf.Max(0, y - viewY2[curLevel]);
        int endX = Mathf.Min(MAP_BLOCK_COUNT_TOTAL_X - 1, x + viewX[curLevel]);
        int endY = Mathf.Min(MAP_BLOCK_COUNT_TOTAL_Y - 1, y + viewY[curLevel]);

        for (int i = startY; i <= endY; ++i)
        {
            for(int j = startX; j <= endX; ++j)
            {
                int block_id = i * MAP_BLOCK_COUNT_TOTAL_X + j;
                MapBlock mb = blocks[block_id];
                mb.Show(curLevel);
            }
        }

        LeftTopOld.x = LeftTop[curLevel].x;
        LeftTopOld.y = LeftTop[curLevel].y;

        BottomRightOld.x = BottomRight[curLevel].x;
        BottomRightOld.y = BottomRight[curLevel].y;
    }

    void ReleaseResource(int block_id)
    {
        MapBlock mb = blocks[block_id];
        //Debug.LogFormat("ReleaseResource block {0}", block_id);
        mb.ReleaseResource();
    }

    void Start()
    {
        ins = this;
        for(int i = 0; i < 3; ++i)
        {
            LeftTop[i] = new Vector2Int();
            BottomRight[i] = new Vector2Int();
            //LeftTopOld[i] = new Vector2Int();
            //BottomRightOld[i] = new Vector2Int();
        }
    }


    public void StartMap()
    {
        sceneCamera = sceneCameraTrans.GetComponent<Camera>();
        plane = new Plane();
        DontDestroyOnLoad(this);

        md = XLoader.Load<MapData>("Map/MapContainer/map_data.asset");

        for(int ty = 0; ty < MAP_TILE_COUNT_Y; ++ty)
        {
            for(int tx = 0; tx < MAP_TILE_COUNT_X; ++tx)
            {
                int tile_id = ty * MAP_TILE_COUNT_X + tx;
                MapTileData td = md.tileList[tile_id];
                MapTile tile = new MapTile(tile_id, td);

                tile.basePos.x = tx * MAP_TILE_DEMANSION_X;
                tile.basePos.z = ty * MAP_TILE_DEMANSION_Y;

                tiles[tile_id] = tile;

                for(int by = 0; by < MAP_BLOCK_COUNT_Y; ++by)
                {
                    for(int bx = 0; bx < MAP_BLOCK_COUNT_X; ++bx)
                    {
                        int yy = ty * MAP_BLOCK_COUNT_Y + by;
                        int xx = tx * MAP_BLOCK_COUNT_X + bx;
                        int block_id = yy * MAP_BLOCK_COUNT_TOTAL_X + xx;
                        MapBlock block = new MapBlock(tile, block_id, td.blockList[by*MAP_BLOCK_COUNT_X + bx]);
                        block.basePos.x = tile.basePos.x + bx * MAP_BLOCK_DEMANSION_X;
                        block.basePos.z = tile.basePos.z + by * MAP_BLOCK_DEMANSION_Y;
                        blocks[block_id] = block;
                    }
                }
            }
        }

        sceneCameraTrans.position = new Vector3(MAP_DEMANSION_X / 2, 10, MAP_DEMANSION_Y / 2);
        run = true;
    }

    void Update()
    {

        if (!run)
            return;

        Vector3 cp = sceneCameraTrans.position;

        tpos.z = cp.y;
        targetPos = cp + tpos;

        curLevel = GetDisplayLevel();

        ShowAtPos(targetPos);
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

    public int PosToBlockId(Vector3 targetPos)
    {
        targetPos.x += (MAP_BLOCK_DEMANSION_X / 2);
        targetPos.z += (MAP_BLOCK_DEMANSION_Y / 2);
        int px = (int)targetPos.x / MAP_BLOCK_DEMANSION_X;
        int py = (int)targetPos.z / MAP_BLOCK_DEMANSION_Y;

        return py * MAP_BLOCK_COUNT_TOTAL_X + px;
    }

    void ShowAtPos(Vector3 targetPos)
    {
        int block_id = PosToBlockId(targetPos);
        if(curBlock == block_id && preLvel == curLevel)
        {
            return;
        }
        Move(block_id % MAP_BLOCK_COUNT_TOTAL_X, block_id / MAP_BLOCK_COUNT_TOTAL_X, curLevel);
        curBlock = block_id;
        preLvel = curLevel;

        Debug.LogFormat("showBlocks {0}", showBlocks);
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
    //MapCache<MapTile> tileCache = new MapCache<MapTile>();
    //MapCache<MapBlock> blockCache = new MapCache<MapBlock>();

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
