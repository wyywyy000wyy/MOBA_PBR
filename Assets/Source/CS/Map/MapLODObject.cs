using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLODObject : MonoBehaviour
{
    //public int resource_id_0; 
    //public int resource_id_1; 
    //public int resource_id_2;

    public int[] resource = new int[3];

    LinkedListNode<MapLODObject> p;
    MapResourceProxy res;
    int level = -1;

    public void Show(int l)
    {
        if(level == l)
        {
            return;
        }
        level = l;
        if(res != null)
        {
            res.Release();
        }
        res = MapResourceCenter.GetResource(resource[level]);

        res.resource.transform.SetParent(this.transform);
    }

    //void LateUpdate()
    //{
    //    if()
    //}
}
