using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControl : MonoBehaviour
{
    // Start is called before the first frame update
    Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame


    Vector3 pressPos;
    int pressId = 0;
    Quaternion rot;
    Vector3 angle;
    public float moveFactor = 0.02f;
    public float rorateFactor = 0.25f;
    public float scrollFactor = 1.0f;

    Vector3 cp;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            pressId = 2;
            pressPos = Input.mousePosition;
            rot = transform.rotation;
            angle = rot.eulerAngles;
        }
        else if(Input.GetMouseButtonDown(2))
        {
            pressId = 1;
            pressPos = Input.mousePosition;
            cp = transform.position;
        }

        if (Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(1))
        {
            pressId = 0;
        }

        if(pressId > 0)
        {
            var dt = Input.mousePosition - pressPos;

            if(pressId == 1)
            {

                Vector3 dp = transform.right;
                dp.y = 0;
                transform.position = cp + (dp * dt.x + Vector3.up * dt.y) * -moveFactor;
            }
            else if(pressId == 2)
            {
                float dx = dt.y * -rorateFactor;
                float dy = dt.x * rorateFactor;

                transform.rotation = Quaternion.Euler(angle[0] + dx, angle[1] + dy, angle[2]);
            }
        }

        transform.position = transform.position + transform.forward * Input.mouseScrollDelta.y * scrollFactor;
    }
}
