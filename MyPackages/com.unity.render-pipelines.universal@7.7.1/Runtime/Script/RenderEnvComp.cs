using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderEnvComp : MonoBehaviour
{
    // Start is called before the first frame update
    public float fov = 60;
    public float near = 0.1f;
    public float far = 10;
    public int d = 1024;
    void Start()
    {
        UnityEngine.Rendering.Universal.Internal.RenderOpaquePass.m_EnvComp = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public MeshRenderer renderer;
    public void SetVirtualMartrix(Matrix4x4 m)
    {
        if(renderer)
        {
            renderer.material.SetMatrix("virtualMatrix", m);
        }
    }

}
