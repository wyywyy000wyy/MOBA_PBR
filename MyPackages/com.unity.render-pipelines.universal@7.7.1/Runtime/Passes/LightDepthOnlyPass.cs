using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Render all objects that have a 'DepthOnly' pass into the given depth buffer.
    ///
    /// You can use this pass to prime a depth buffer for subsequent rendering.
    /// Use it as a z-prepass, or use it to generate a depth buffer.
    /// </summary>
    public class LightDepthOnlyPass : ScriptableRenderPass
    {
        int kDepthBufferBits = 32;

        int treeDem = 1024;

        RenderTextureDescriptor m_RenderDescroptor;

        Material m_Material;


        FilteringSettings m_FilteringSettings;
        RenderStateBlock m_RenderStateBlock;
        const string m_ProfilerTag = "LightDepthOnlyPass";
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
        ShaderTagId m_ShaderTagId = new ShaderTagId("DepthOnly");
        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

        RenderTexture rt;

        RenderTargetHandle m_TargetAttachment;
        static readonly int s_DrawObjectPassDataPropID = Shader.PropertyToID("_DrawObjectPassData");

        /// <summary>
        /// Create the LightDepthOnlyPass
        /// </summary>
        public LightDepthOnlyPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference)
        {
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));

            //m_TargetAttachment.Init("_RectifiedShadowTexture");
            m_Material = new Material(Shader.Find("YSTech/RectifiedShadow"));
            m_Material.SetColor("_Color", Color.black);
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            renderPassEvent = evt;
            m_RenderDescroptor = new RenderTextureDescriptor(treeDem, treeDem, RenderTextureFormat.ARGB32, 32);
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            if (stencilState.enabled)
            {
                m_RenderStateBlock.stencilReference = stencilReference;
                m_RenderStateBlock.mask = RenderStateMask.Stencil;
                m_RenderStateBlock.stencilState = stencilState;
            }
            rt = new RenderTexture(m_RenderDescroptor);

        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            //cmd.GetTemporaryRT(m_TargetAttachment.id, m_RenderDescroptor, FilterMode.Point);
            //cmd.GetTemporaryRT(m_TargetAttachment.id, m_RenderDescroptor, FilterMode.Point);
            //ConfigureTarget(m_TargetAttachment.Identifier());
            ConfigureTarget(new RenderTargetIdentifier(rt));
            //ConfigureClear(ClearFlag.All, Color.black);
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isSceneViewCamera)
                return;
            GameObject lightObj = GameObject.Find("Directional Light");
            if (lightObj == null)
                return;
            Transform lightTrans = lightObj.transform;
            Light light = lightTrans.GetComponent<Light>();

            Vector3 right = lightTrans.right;
            Vector3 forward = -lightTrans.up;
            Vector3 up = lightTrans.forward;
            Vector4 u = right;// renderCamera.transform.right;
            u.w = -Vector3.Dot(u, lightTrans.position);
            Vector4 v = up;// renderCamera.transform.up;
            v.w = -Vector3.Dot(v, lightTrans.position);
            Vector4 n = -forward;// renderCamera.transform.forward;
            n.w = -Vector3.Dot(n, lightTrans.position);

            Matrix4x4 virtualViewMatrix = new Matrix4x4();
            Matrix4x4 virtualProjMatrix = new Matrix4x4();

            virtualViewMatrix.SetRow(0, u);
            virtualViewMatrix.SetRow(1, v);
            virtualViewMatrix.SetRow(2, n);
            virtualViewMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            ref CameraData cameraData = ref renderingData.cameraData;

            //renderingData.cullResults.ComputeDirectionalShadowMatricesAndCullingPrimitives

            //Matrix4x4 projectMat = Matrix4x4.Ortho()

            //m_EnvComp.SetVirtualMartrix(virtualProjectMatrix * virtualViewMatrix * m_EnvComp.transform.localToWorldMatrix);
            //cmd.SetViewProjectionMatrices(virtualViewMatrix, cameraData.GetGPUProjectionMatrix());


            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                CullingResults cullResults = renderingData.cullResults;

                ShadowSplitData splitData;
                bool success = cullResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(0,
                    0, 1, new Vector3(1,0,0), 2048, 0f, out virtualViewMatrix, out virtualProjMatrix,
                    out splitData);

                cmd.SetViewProjectionMatrices(virtualViewMatrix, virtualProjMatrix);
                cmd.SetGlobalMatrix("_G_MAP_LIGHT_SHADOW_MATRIX", virtualViewMatrix * virtualProjMatrix);

                cmd.ClearRenderTarget(true, true, Color.black);
                Vector4 drawObjectPassData = new Vector4(0.0f, 0.0f, 0.0f, true ? 1.0f : 0.0f);
                cmd.SetGlobalVector(s_DrawObjectPassDataPropID, drawObjectPassData);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();




                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);
                drawSettings.overrideMaterial = m_Material;
                //drawSettings.perObjectData = PerObjectData.None;

                Camera camera = cameraData.camera;
                if (cameraData.isStereoEnabled)
                {
                    context.StartMultiEye(camera, eyeIndex);
                }
                var filterSettings = m_FilteringSettings;

#if UNITY_EDITOR
                // When rendering the preview camera, we want the layer mask to be forced to Everything
                if (renderingData.cameraData.isPreviewCamera)
                {
                    filterSettings.layerMask = -1;
                }
#endif
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings, ref m_RenderStateBlock);
                //context.DrawSkybox(renderingData.cameraData.camera);

            }
            cmd.SetGlobalTexture("_RectifiedShadowTexture", rt.depthBuffer);
            //cmd.ClearRenderTarget(true, false, Color.black);
            cmd.SetViewProjectionMatrices(cameraData.GetViewMatrix(), cameraData.GetGPUProjectionMatrix());

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            context.Submit();

            var pre = RenderTexture.active;

            RenderTexture.active = rt;

            for (int i = 0; i < treeDem; ++i)
            {
                for(int j = 0; j < treeDem; ++j)
                {
                    //rt.get
                }
            }

            RenderTexture.active = pre;
        }

        /// <inheritdoc/>
        //public override void FrameCleanup(CommandBuffer cmd)
        //{
        //    if (cmd == null)
        //        throw new ArgumentNullException("cmd");

        //    if(m_TargetAttachment != RenderTargetHandle.CameraTarget)
        //    {
        //        cmd.ReleaseTemporaryRT(m_TargetAttachment.id);
        //        m_TargetAttachment = RenderTargetHandle.CameraTarget;
        //    }
        //}
    }
}