using System.Collections;
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

    //Dictionary<int, MapLODObject> objs = new Dictionary<int, MapLODObject>();

    LinkedList<MapLODObject> obj_list = new LinkedList<MapLODObject>();
    LinkedList<MapLODObject> high_obj_list = new LinkedList<MapLODObject>();

    int prelevel = -1;

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
        terrain = null;

        LinkedListNode<MapLODObject> lp = obj_list.First;
        while (lp != null)
        {
            lp.Value.hide();
            lp = lp.Next;
        }

        LinkedListNode<MapLODObject> lp2 = high_obj_list.First;
        while (lp2 != null)
        {
            lp2.Value.hide();
            lp2 = lp2.Next;
        }
        

        prelevel = -1;
    }

    public void Show(int level)
    {
        if(prelevel == level)
        {
            return;
        }

        if(level < 2 && terrain == null)
        {
            terrain = GetResource(data.terrain);
            terrain.transform.position = basePos;
        }
        if(level < 2)
        {
            terrain.SetActive(true);
        }
        else
        {
            if(terrain != null)
            {
                terrain.SetActive(false);
            }
        }

        //if (level <= 1)
        {
            ShowDetail();
        }
        //else
        //{
        //    HideDetail();
        //}

        if(prelevel < 2 || level < 2)
        {
            LinkedListNode<MapLODObject> lp = obj_list.First;
            while (lp != null)
            {
                lp.Value.Show(level);
                lp = lp.Next;
            }
        }
        {
            LinkedListNode<MapLODObject> lp = high_obj_list.First;
            while (lp != null)
            {
                lp.Value.Show(level);
                lp = lp.Next;
            }
        }
        prelevel = level;

    }

    public void ShowDetail()
    {
        if(obj_list.First != null)
        {
            return;
        }
        int count = 100;
        for(int i = 0; i < count; ++i)
        {
            int res_obj_id = Random.Range(1003,1010);
            Vector3 pos = new Vector3(basePos.x + Random.Range((float)-Map.MAP_BLOCK_DEMANSION_X / 2, Map.MAP_BLOCK_DEMANSION_X / 2), basePos.y, basePos.z + Random.Range((float)-Map.MAP_BLOCK_DEMANSION_Y / 2, Map.MAP_BLOCK_DEMANSION_Y / 2));
            Spawn(res_obj_id, pos);
        }

        count = 10;
        for (int i = 0; i < count; ++i)
        {
            int res_obj_id = Random.Range(1002, 1002);
            Vector3 pos = new Vector3(basePos.x + Random.Range((float)-Map.MAP_BLOCK_DEMANSION_X / 2, Map.MAP_BLOCK_DEMANSION_X / 2), basePos.y, basePos.z + Random.Range((float)-Map.MAP_BLOCK_DEMANSION_Y / 2, Map.MAP_BLOCK_DEMANSION_Y / 2));
            Spawn(res_obj_id, pos);
        }

        count = 2;
        for (int i = 0; i < count; ++i)
        {
            int res_obj_id = Random.Range(1001, 1001);
            Vector3 pos = new Vector3(basePos.x + Random.Range((float)-Map.MAP_BLOCK_DEMANSION_X / 2, Map.MAP_BLOCK_DEMANSION_X / 2), basePos.y, basePos.z + Random.Range((float)-Map.MAP_BLOCK_DEMANSION_Y / 2, Map.MAP_BLOCK_DEMANSION_Y / 2));
            Spawn(res_obj_id, pos);
        }
    }

    public void Spawn(int res_obj_id, Vector3 pos)
    {
        MapResourceProxy res = MapResourceCenter.GetResource(res_obj_id);
        MapLODObject obj = res.resource.GetComponent<MapLODObject>();
        obj.transform.position = pos;
        if(obj.resource[2] != 0)
        {
            obj.p = high_obj_list.AddLast(obj);
        }
        else
        {
            obj.p = obj_list.AddLast(obj);
        }
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
        prelevel = -1;
    }
}
