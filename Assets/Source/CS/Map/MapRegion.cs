using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRegion: MapContainerObject
{
    MapSection[] sections;//= new MapRegion[MAP_REGION_COUNT_X * MAP_REGION_COUNT_Y];

    public static readonly int Level = 0;

    void Show(Vector3 tp, int level)
    {
        if(sections == null)
        {
            sections = new MapSection[Map.MAP_SECTION_COUNT];
        }
        if(level > Level)
        {
            ShowDetail();
        }
    }

    void ShowDetail()
    {
        for(int i =0; i < sections.Length; ++i)
        {
            MapSection sect = sections[i];
            if(sect == null)
            {
                sect = Map.Instance.NewMapSection();
                sections[i] = sect;
            }

        }
    }

    void HideDetail()
    {
        for (int i = 0; i < sections.Length; ++i)
        {
            MapSection sect = sections[i];
            if (sect == null)
            {
                sect = Map.Instance.NewMapSection();
                sections[i] = sect;
            }

        }
    }

    void Hide(Vector3 tp, int level)
    {

    }
}
