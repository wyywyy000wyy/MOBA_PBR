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
    }

    public void Show(int level)
    {
        if(terrain == null)
        {
            terrain = GetResource(data.terrain);
            terrain.transform.position = basePos;
        }
        terrain.SetActive(true);

        if (level == 0)
        {
            ShowDetail();
        }
        else
        {
            HideDetail();
        }

        LinkedListNode<MapLODObject> lp = obj_list.First;
        while (lp != null)
        {
            lp.Value.Show(level);
            lp = lp.Next;
        }
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
            int res_obj_id = Random.Range(1001,1004);
            Vector3 pos = new Vector3(basePos.x + Random.Range((float)-Map.MAP_BLOCK_DEMANSION_X / 2, Map.MAP_BLOCK_DEMANSION_X / 2), basePos.y, basePos.z + Random.Range((float)-Map.MAP_BLOCK_DEMANSION_Y / 2, Map.MAP_BLOCK_DEMANSION_Y / 2));
            Spawn(res_obj_id, pos);
        }
    }

    public void Spawn(int res_obj_id, Vector3 pos)
    {
        MapResourceProxy res = MapResourceCenter.GetResource(res_obj_id);
        MapLODObject obj = res.resource.GetComponent<MapLODObject>();
        obj.transform.position = pos;
        obj.p = obj_list.AddLast(obj);
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
