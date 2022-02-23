using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderEnvComp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Rendering.Universal.Internal.RenderOpaquePass.m_EnvComp = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
