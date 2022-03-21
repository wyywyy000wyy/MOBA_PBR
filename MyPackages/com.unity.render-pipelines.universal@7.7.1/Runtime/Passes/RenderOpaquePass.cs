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
            m_EnvComp = GameObject.Find("Plane (5)")?.GetComponent<RenderEnvComp>();
            int dm = 1024;
            if(m_EnvComp != null)
            {
                dm = m_EnvComp.d;
            }
            m_RenderDescroptor = new RenderTextureDescriptor(dm, dm, RenderTextureFormat.ARGB32, 32);
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

        void ModifyProjectionMatrix(ref Matrix4x4 project, Vector4 clipPlane)
        {
            Matrix4x4 matrix = project;
            //float matrix[16];
            Vector4 q;
            //// Grab the current projection matrix from OpenGL.
            //glGetFloatv(GL_PROJECTION_MATRIX, matrix);
            q.x = (Mathf.Sign(clipPlane.x) + matrix[8]) / matrix[0];
            q.y = (Mathf.Sign(clipPlane.y) + matrix[9]) / matrix[5];
            q.z = -1.0F;
            q.w = (1.0F + matrix[10]) / matrix[14];
                // Calculate the scaled plane vector using Equation (5.68)
                // and replace the third row of the projection matrix.
                Vector4 c = clipPlane * (2.0F / Vector4.Dot(clipPlane, q));
                    matrix[2] = c.x;
            matrix[6] = c.y;
            matrix[10] = c.z + 1.0F;
            matrix[14] = c.w;
            project = matrix;
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

            //Camera VirtualCamera = GameObject.Find("VirtualCamera")?.GetComponent<Camera>();

            Camera renderCamera = renderingData.cameraData.camera;
            CameraData cameraData = renderingData.cameraData ;//new CameraData();
            m_VirtualCamera = cameraData.camera;



            Matrix4x4 virtualViewMatrix = m_VirtualCamera.worldToCameraMatrix ;
            Matrix4x4 virtualProjectMatrix = m_VirtualCamera.projectionMatrix;

            if (m_EnvComp != null)
            {
                Vector3 right = m_EnvComp.transform.right;
                Vector3 forward = -m_EnvComp.transform.up;
                Vector3 up = m_EnvComp.transform.forward;
                Vector3 mirrorPos = m_EnvComp.transform.position;
                //Matrix4x4 rot = new Matrix4x4(
                //    right,
                //    up,
                //    -forward,
                //    new Vector4(worldPos.x, worldPos.y, worldPos.z, 1));
                //virtualViewMatrix = rot.inverse;

                Camera virtualCamera = new Camera();

                Vector3 planeUp = m_EnvComp.transform.up;
                Vector3 cameraPos = renderCamera.transform.position;
                Vector3 cameraUp = renderCamera.transform.up;
                Vector3 view = mirrorPos - Vector3.Reflect(mirrorPos - cameraPos, planeUp);

                Vector3 cameraTarget = cameraPos + renderCamera.transform.forward;

                
                
                Vector3 viewTarget = mirrorPos - Vector3.Reflect(mirrorPos - cameraTarget, planeUp);
                //{
                //    //TMP
                //    view = new Vector3(24.047180520002573f, -39.43195857764696f, 145.84470054505104f);
                //    viewTarget = new Vector3(23.884495678809213f,-39.42903627610944f,144.8580267881285f);
                //}

                Vector3 viewForward = (viewTarget - view).normalized;
                Vector3 viewUp = Vector3.Reflect(cameraUp, planeUp);
                Vector3 viewRight = Vector3.Cross(viewUp, viewForward);

                Vector4 u = viewRight;// renderCamera.transform.right;
                u.w = -Vector3.Dot(u, view);
                Vector4 v = viewUp;// renderCamera.transform.up;
                v.w = -Vector3.Dot(v, view);
                Vector4 n = -viewForward;// renderCamera.transform.forward;
                n.w = -Vector3.Dot(n, view);

                virtualViewMatrix.SetRow(0, u);
                virtualViewMatrix.SetRow(1, v);
                virtualViewMatrix.SetRow(2, n);
                virtualViewMatrix.SetRow(3, new Vector4(0,0,0,1));

                //VirtualCamera.transform.position = view;
                //VirtualCamera.transform.up = viewUp;
                //VirtualCamera.transform.right = viewRight;
                //VirtualCamera.transform.forward = viewForward;

                //VirtualCamera.projectionMatrix = m_VirtualCamera.projectionMatrix;
                //VirtualCamera.worldToCameraMatrix = virtualViewMatrix;


                //virtualCamera.projectionMatrix = m_VirtualCamera.projectionMatrix;
                //virtualCamera.worldToCameraMatrix = virtualViewMatrix;

                Matrix4x4 virtualCameraWorldMatrix = new Matrix4x4(
                    viewRight,
                    viewUp,
                    viewForward,
                    new Vector4(view.x, view.y, view.z,1)
                    );

                Plane mirrorPlane = new Plane(planeUp, mirrorPos);
                Vector4 clipPlane;// = new Vector4(mirrorPlane.normal.x, mirrorPlane.normal.y, mirrorPlane.normal.z, mirrorPlane.distance);
                {
                    Vector3 virtualCameraSpaceNormal = virtualViewMatrix.MultiplyVector(planeUp);
                    Vector3 virtualCameraSpacePos = virtualViewMatrix.MultiplyPoint(mirrorPos);

                    Plane plane = new Plane(virtualCameraSpaceNormal, virtualCameraSpacePos);

                    clipPlane = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
                    //Matrix4x4 normalMatrix = virtualCameraWorldMatrix.transpose;
                    //Vector3 mn = normalMatrix.MultiplyVector(mirrorPlane.normal).normalized;
                    //Vector3 point = mirrorPlane.normal * (-mirrorPlane.distance);
                    //float constant = -Vector3.Dot(point, mn);
                    //clipPlane = new Vector4(mn.x, mn.y, mn.z, constant);
                }


                //clipPlane = virtualViewMatrix * clipPlane;
                Matrix4x4 textureMatrix = new Matrix4x4(
                    new Vector4(0.5f,0.0f,0.0f,0.5f),
                    new Vector4(0.0f,0.5f,0.0f,0.5f),
                    new Vector4(0.0f,0.0f,0.5f,0.5f),
                    new Vector4(0.0f,0.0f,0.0f,1.0f)
                    );
                m_EnvComp.SetVirtualMartrix(virtualProjectMatrix * virtualViewMatrix * m_EnvComp.transform.localToWorldMatrix);
                virtualProjectMatrix = renderCamera.CalculateObliqueMatrix(clipPlane);
                //ModifyProjectionMatrix(ref virtualProjectMatrix, clipPlane);
            }


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

                context.DrawSkybox(renderingData.cameraData.camera);
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
