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
    public static readonly int MAP_CELL_COUNT_X = 10;
    public static readonly int MAP_CELL_COUNT_Y = 10;

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

    public float scaleValue0 = 10;
    public float scaleValue1 = 20;

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

    
    Vector2Int LeftTop;
    Vector2Int BottomRight;

    bool run = false;

    //BlockResource[] blocks;

    static readonly int MAX_BLOCK = 10 * 10;

    int viewX = 3;
    int viewY = 3;
    int viewY2 = 1;
    int curBlock = -1;
    int preX = 0;
    int preY = 0;
    int showBlocks = 0;

    public void Move(int x, int y)
    {

        LeftTop.x = Mathf.Clamp(Mathf.Min(x - viewX, LeftTop.x), 0, MAP_BLOCK_COUNT_TOTAL_X - 1);
        LeftTop.y = Mathf.Clamp(Mathf.Min(y - viewY2, LeftTop.y), 0, MAP_BLOCK_COUNT_TOTAL_Y - 1);

        BottomRight.x = Mathf.Clamp(Mathf.Max(x + viewX, BottomRight.x), 0, MAP_BLOCK_COUNT_TOTAL_X - 1);
        BottomRight.y = Mathf.Clamp(Mathf.Max(y + viewY, BottomRight.y), 0, MAP_BLOCK_COUNT_TOTAL_Y - 1);

        int dx = BottomRight.x - LeftTop.x;
        int dy = BottomRight.y - LeftTop.y;

        Debug.LogErrorFormat("{0}, {1}, {2}, {3}, {4}, {5}", LeftTop.x, LeftTop.y, BottomRight.x, BottomRight.y, dx, dy);
        if (Mathf.Abs(x - preX) > 1 || Mathf.Abs(y - preY) > 1)
        {
            for (int ty = LeftTop.y; ty <= BottomRight.y; ++ty)
            {
                for (int tx = LeftTop.x; tx <= BottomRight.x; ++tx)
                {
                    int block_id = ty * MAP_BLOCK_COUNT_TOTAL_X + tx;
                    ReleaseResource(block_id);
                }
            }
            dx = 0;
            dy = 0;

            LeftTop.x = Mathf.Clamp(x - viewX, 0, MAP_BLOCK_COUNT_TOTAL_X - 1);
            LeftTop.y = Mathf.Clamp(y - viewY2, 0, MAP_BLOCK_COUNT_TOTAL_Y - 1);

            BottomRight.x = Mathf.Clamp(x + viewX, 0, MAP_BLOCK_COUNT_TOTAL_X - 1);
            BottomRight.y = Mathf.Clamp(y + viewY, 0, MAP_BLOCK_COUNT_TOTAL_Y - 1);
        }
        preX = x;
        preY = y;

        if (dx * dy > MAX_BLOCK)
        {
            if(dy > dx)
            {
                if(dy * 2> LeftTop.y + BottomRight.y)
                {
                    int py = LeftTop.y;
                    LeftTop.y++;

                    for(int i = LeftTop.x; i <= BottomRight.x; ++i)
                    {
                        int block_id = py * MAP_BLOCK_COUNT_TOTAL_X + i;
                        ReleaseResource(block_id);
                    }
                }
                else
                {
                    int py = BottomRight.y;
                    BottomRight.y--;

                    for (int i = LeftTop.x; i <= BottomRight.x; ++i)
                    {
                        int block_id = py * MAP_BLOCK_COUNT_TOTAL_X + i;
                        ReleaseResource(block_id);
                    }
                }
            }
            else
            {
                if(dx * 2 > LeftTop.x + BottomRight.x)
                {
                    int px = LeftTop.x;
                    LeftTop.x++;

                    for (int i = LeftTop.y; i <= BottomRight.y; ++i)
                    {
                        int block_id = i * MAP_BLOCK_COUNT_TOTAL_X + px;
                        ReleaseResource(block_id);
                    }
                }
                else
                {
                    int px = BottomRight.x;
                    BottomRight.x--;

                    for (int i = LeftTop.y; i <= BottomRight.y; ++i)
                    {
                        int block_id = i * MAP_BLOCK_COUNT_TOTAL_X + px;
                        ReleaseResource(block_id);
                    }
                }
            }
        }

        showBlocks = (BottomRight.x - LeftTop.x) * (BottomRight.y - LeftTop.y);

        int startX = Mathf.Max(0, x - viewX);
        int startY = Mathf.Max(0, y - viewY2);
        int endX = Mathf.Min(MAP_BLOCK_COUNT_TOTAL_X - 1, x + viewX);
        int endY = Mathf.Min(MAP_BLOCK_COUNT_TOTAL_Y - 1, y + viewY);

        for (int i = startY; i <= endY; ++i)
        {
            for(int j = startX; j <= endX; ++j)
            {
                int block_id = i * MAP_BLOCK_COUNT_TOTAL_X + j;
                MapBlock mb = blocks[block_id];
                mb.Show();
            }
        }
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
                        int block_id = yy * MAP_TILE_COUNT_X * MAP_BLOCK_COUNT_X + xx;
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

        int dl = GetDisplayLevel();

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
        int px = (int)targetPos.x / MAP_BLOCK_DEMANSION_X;
        int py = (int)targetPos.z / MAP_BLOCK_DEMANSION_Y;

        return py * MAP_BLOCK_COUNT_TOTAL_X + px;
    }

    void ShowAtPos(Vector3 targetPos)
    {
        int block_id = PosToBlockId(targetPos);
        if(curBlock == block_id)
        {
            return;
        }
        Move(block_id % MAP_BLOCK_COUNT_TOTAL_X, block_id / MAP_BLOCK_COUNT_TOTAL_X);
        curBlock = block_id;

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
