using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Transform sceneCamera;

    public GameObject mapLevel0;
    public GameObject mapLevel1;
    public GameObject mapLevel2;

    public float scaleValue0 = 10;
    public float scaleValue1 = 20;

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
