using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace UnityEngine.Rendering.Universal.Internal
{

public class RenderOpaquePass : ScriptableRenderPass
{
        public static RenderEnvComp m_EnvComp;
        RenderTargetHandle m_TargetAttachment;
        RenderTextureDescriptor m_RenderDescroptor;

        Camera m_VirtualCamera;
        float clipBias = 0;


        FilteringSettings m_FilteringSettings;
        RenderStateBlock m_RenderStateBlock;
        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        string m_ProfilerTag = "RenderOpaquePass";
        ProfilingSampler m_ProfilingSampler;
        bool m_IsOpaque;

        static readonly int s_DrawObjectPassDataPropID = Shader.PropertyToID("_DrawObjectPassData");

        public RenderOpaquePass(bool opaque, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference)
        {
            m_TargetAttachment.Init("_RenderOpaquePassTexture");
            m_RenderDescroptor = new RenderTextureDescriptor(1024,1024,RenderTextureFormat.ARGB32, 32);
            m_VirtualCamera = new Camera();

            m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            renderPassEvent = evt;

            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_IsOpaque = opaque;

            if (stencilState.enabled)
            {
                m_RenderStateBlock.stencilReference = stencilReference;
                m_RenderStateBlock.mask = RenderStateMask.Stencil;
                m_RenderStateBlock.stencilState = stencilState;
            }
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(m_TargetAttachment.id, m_RenderDescroptor);
            ConfigureTarget(m_TargetAttachment.Identifier());
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // if(m_EnvComp == null)
            // {
            //     return;
            // }
            if(renderingData.cameraData.isSceneViewCamera )
                return;

            m_EnvComp = null;
            m_EnvComp = GameObject.Find("Plane (5)")?.GetComponent<RenderEnvComp>();

            CameraData cameraData = renderingData.cameraData ;//new CameraData();
            m_VirtualCamera = cameraData.camera;



            Matrix4x4 virtualViewMatrix =  m_EnvComp != null ? Matrix4x4.LookAt(m_EnvComp.transform.position, m_EnvComp.transform.position - m_EnvComp.transform.up, m_EnvComp.transform.forward).inverse : m_VirtualCamera.worldToCameraMatrix ;
            if(m_EnvComp != null)
            {
                Vector3 right = m_EnvComp.transform.right;
                Vector3 forward = -m_EnvComp.transform.up;
                Vector3 up = m_EnvComp.transform.forward;
                Vector3 worldPos = m_EnvComp.transform.position;
                Matrix4x4 rot = new Matrix4x4(
                    right,
                    up,
                    -forward,
                    new Vector4(worldPos.x, worldPos.y, worldPos.z, 1));
                virtualViewMatrix = rot.inverse;
            }
            
            Matrix4x4 virtualProjectMatrix = m_EnvComp != null ? Matrix4x4.Perspective(m_EnvComp.fov, m_EnvComp.transform.localScale.x / m_EnvComp.transform.localScale.z, m_EnvComp.near, m_EnvComp.far) : m_VirtualCamera.projectionMatrix;

            Plane mirrorPlane = new Plane();
            Vector4 clipPlane = new Vector4( mirrorPlane.normal.x, mirrorPlane.normal.y, mirrorPlane.normal.z, mirrorPlane.distance); 
            Vector4 q = new Vector4();

            q.x = ( Mathf.Sign( clipPlane.x ) + virtualProjectMatrix[ 8 ] ) / virtualProjectMatrix[ 0 ];
			q.y = ( Mathf.Sign( clipPlane.y ) + virtualProjectMatrix[ 9 ] ) / virtualProjectMatrix[ 5 ];
			q.z = - 1.0f;
			q.w = ( 1.0f + virtualProjectMatrix[ 10 ] ) / virtualProjectMatrix[ 14 ]; // Calculate the scaled plane vector

            clipPlane *= (2.0f / (Vector4.Dot(clipPlane,  q )));

            //if (m_EnvComp != null)
            //{
            //    virtualProjectMatrix[2] = clipPlane.x;
            //    virtualProjectMatrix[6] = clipPlane.y;
            //    virtualProjectMatrix[10] = clipPlane.z + 1.0f - clipBias;
            //    virtualProjectMatrix[14] = clipPlane.w;
            //}


            if (!m_VirtualCamera.TryGetCullingParameters(m_VirtualCamera, out var cullingParameters))
                return;
            CullingResults cullResults = context.Cull(ref cullingParameters);
            RenderingData virutalRenderingData = new RenderingData();

            virutalRenderingData.cullResults = cullResults;
            virutalRenderingData.cameraData = cameraData;
            virutalRenderingData.lightData = renderingData.lightData;
            virutalRenderingData.shadowData = renderingData.shadowData;
            virutalRenderingData.postProcessingData = renderingData.postProcessingData;
            virutalRenderingData.supportsDynamicBatching = renderingData.supportsDynamicBatching;
            virutalRenderingData.perObjectData = renderingData.perObjectData;
            virutalRenderingData.postProcessingEnabled = renderingData.postProcessingEnabled;
            virutalRenderingData.killAlphaInFinalBlit = renderingData.killAlphaInFinalBlit;


            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

		/// <summary>

            // cmd.SetGlobalMatrix(ShaderPropertyId.viewMatrix, viewMatrix);
            // cmd.SetGlobalMatrix(ShaderPropertyId.projectionMatrix, projectionMatrix);
            // cmd.SetGlobalMatrix(ShaderPropertyId.viewAndProjectionMatrix, viewAndProjectionMatrix);

            // cmd.SetGlobalVector(ShaderPropertyId.worldSpaceCameraPos, camera.transform.position);
            // cmd.SetGlobalVector(ShaderPropertyId.screenParams, new Vector4(cameraWidth, cameraHeight, 1.0f + 1.0f / cameraWidth, 1.0f + 1.0f / cameraHeight));
            // cmd.SetGlobalVector(ShaderPropertyId.scaledScreenParams, new Vector4(scaledCameraWidth, scaledCameraHeight, 1.0f + 1.0f / scaledCameraWidth, 1.0f + 1.0f / scaledCameraHeight));
            // cmd.SetGlobalVector(ShaderPropertyId.zBufferParams, zBufferParams);
            // cmd.SetGlobalVector(ShaderPropertyId.orthoParams, orthoParams);

            // RenderingUtils.SetViewAndProjectionMatrices(cmd, virtualViewMatrix, virtualProjectMatrix, false);

            cmd.SetViewProjectionMatrices(virtualViewMatrix, virtualProjectMatrix);


            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                cmd.ClearRenderTarget(true, true, Color.black);

                Vector4 drawObjectPassData = new Vector4(0.0f, 0.0f, 0.0f, (m_IsOpaque) ? 1.0f : 0.0f);
                cmd.SetGlobalVector(s_DrawObjectPassDataPropID, drawObjectPassData);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                // Camera camera = renderingData.cameraData.camera;
                var sortFlags = (m_IsOpaque) ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
                var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);
                var filterSettings = m_FilteringSettings;

                #if UNITY_EDITOR
                // When rendering the preview camera, we want the layer mask to be forced to Everything
                if (renderingData.cameraData.isPreviewCamera)
                {
                    filterSettings.layerMask = -1;
                }
                #endif

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings, ref m_RenderStateBlock);

                // Render objects that did not match any shader pass with error shader
                // RenderingUtils.RenderObjectsWithError(context, ref renderingData.cullResults, camera, filterSettings, SortingCriteria.None);
            
            }
            // RenderingUtils.SetViewAndProjectionMatrices(cmd, cameraData.GetViewMatrix(), cameraData.GetGPUProjectionMatrix(), false);
            cmd.ClearRenderTarget(true,false,Color.black);
            cmd.SetViewProjectionMatrices(cameraData.GetViewMatrix(), cameraData.GetGPUProjectionMatrix());
            // renderingData.cameraData.renderer.SetPerCameraShaderVariables(cmd, ref renderingData.cameraData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        /// <inheritdoc/>
        public void Execute2(ScriptableRenderContext context, ref RenderingData renderingData)
        {

            
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // Global render pass data containing various settings.
                // x,y,z are currently unused
                // w is used for knowing whether the object is opaque(1) or alpha blended(0)
                Vector4 drawObjectPassData = new Vector4(0.0f, 0.0f, 0.0f, (m_IsOpaque) ? 1.0f : 0.0f);
                cmd.SetGlobalVector(s_DrawObjectPassDataPropID, drawObjectPassData);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                Camera camera = renderingData.cameraData.camera;
                var sortFlags = (m_IsOpaque) ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
                var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);
                var filterSettings = m_FilteringSettings;

                #if UNITY_EDITOR
                // When rendering the preview camera, we want the layer mask to be forced to Everything
                if (renderingData.cameraData.isPreviewCamera)
                {
                    filterSettings.layerMask = -1;
                }
                #endif

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings, ref m_RenderStateBlock);

                // Render objects that did not match any shader pass with error shader
                RenderingUtils.RenderObjectsWithError(context, ref renderingData.cullResults, camera, filterSettings, SortingCriteria.None);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

}
