using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLODObject : MonoBehaviour
{
    //public int resource_id_0; 
    //public int resource_id_1; 
    //public int resource_id_2;

    public int[] resource = new int[3];

    public LinkedListNode<MapLODObject> p;
    public MapResourceProxy res;
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
            res = null;
        }

        if(resource[level] == 0)
        {
            return;
        }

        this.gameObject.SetActive(true);

        res = MapResourceCenter.GetResource(resource[level]);

        res.resource.transform.localPosition = Vector3.zero;
        res.resource.transform.localScale = Vector3.one * 0.01f;
        res.resource.transform.Rotate(new Vector3(0,Random.Range(0,360),0));
        res.resource.transform.SetParent(this.transform,false);

        res.resource.SetActive(true);
    }

    public void hide()
    {
        level = -1;
        if (res != null)
        {
            res.Release();
            res = null;
        }
    }

    //void LateUpdate()
    //{
    //    if()
    //}
}
