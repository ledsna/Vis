using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class Outlines : ScriptableRendererFeature {

    [System.Serializable]
    private class OutlineSettings {

        //[Header("General Outline Settings")]
        //public Color outlineColor = Color.black;
        //[Range(0.0f, 20.0f)]
        public float depthOutlineScale = 1.0f;

        //[Header("Normal Outline Settings")] 
        //public Color normalColor = Color.white;
		//[Range(0.0f, 20.0f)]
        public float normalsOutlineScale = 1.0f;

        [Header("Depth Settings")]
        [Range(0.0f, 200.0f)]
        public float depthThreshold = 1f;
        //[Range(0.0f, 500.0f)]
        // public float robertsCrossMultiplier = 100.0f;

        [Header("Normal Settings")]
        [Range(0.0f, 1.0f)]
        public float normalThreshold = 0.4f;

		[FormerlySerializedAs("highlightIntensity")]
        [Header("Normal Highlight Intensity")]
		[Range(1.0f, 10f)]
		public float highlightPower = 1.5f;

		[FormerlySerializedAs("shadowIntensity")]
        [Header("Depth Shadow Intensity")]
		[Range(-1.0f, 5.0f)]
		public float shadowPower = 0.5f;

        // [Header("Depth Normal Relation Settings")]
        // [Range(0.0f, 2.0f)]
        // public float steepAngleThreshold = 0.0f;
        // [Range(0.0f, 500.0f)]
        // public float steepAngleMultiplier = 0.0f;

    }

    [System.Serializable]
    private class NormalsTextureSettings {

        [Header("Normals Texture Settings")]
        public RenderTextureFormat colorFormat;
        public int depthBufferBits = 16;
        public FilterMode filterMode;
        public Color backgroundColor = Color.black;

        [Header("Normal Texture Object Draw Settings")]
        public PerObjectData perObjectData;
        public bool enableDynamicBatching;
        public bool enableInstancing;

    }

    private class NormalsTexturePass : ScriptableRenderPass {

        private NormalsTextureSettings normalsTextureSettings;
        private FilteringSettings filteringSettings;
        private FilteringSettings occluderFilteringSettings;

        private readonly List<ShaderTagId> shaderTagIdList;
        private readonly Material normalsMaterial;
        private readonly Material occludersMaterial;
        private readonly RenderTargetHandle normals;

        public NormalsTexturePass(RenderPassEvent renderPassEvent, LayerMask layerMask, LayerMask occluderLayerMask, NormalsTextureSettings settings) {
            this.renderPassEvent = renderPassEvent;
            normalsTextureSettings = settings;
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
            occluderFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, occluderLayerMask);

            shaderTagIdList = new List<ShaderTagId> {
                new ("UniversalForward"),
                new ("UniversalForwardOnly"),
                new ("LightweightForward"),
                new ("SRPDefaultUnlit")
            };

            normals.Init("_CameraNormalTexture");
            normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormals"));

            occludersMaterial = new Material(Shader.Find("Hidden/UnlitColor"));
            occludersMaterial.SetColor("_Color", normalsTextureSettings.backgroundColor);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
            normalsTextureDescriptor.colorFormat = normalsTextureSettings.colorFormat;
            normalsTextureDescriptor.depthBufferBits = normalsTextureSettings.depthBufferBits;
            cmd.GetTemporaryRT(normals.id, normalsTextureDescriptor, normalsTextureSettings.filterMode);

            ConfigureTarget(normals.Identifier());
            ConfigureClear(ClearFlag.All, normalsTextureSettings.backgroundColor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (!normalsMaterial || !occludersMaterial)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("CameraNormalsTextureCreation"))) {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                DrawingSettings drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                drawSettings.perObjectData = normalsTextureSettings.perObjectData;
                drawSettings.enableDynamicBatching = normalsTextureSettings.enableDynamicBatching;
                drawSettings.enableInstancing = normalsTextureSettings.enableInstancing;
                drawSettings.overrideMaterial = normalsMaterial;

                DrawingSettings occluderSettings = drawSettings;
                occluderSettings.overrideMaterial = occludersMaterial;

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
                context.DrawRenderers(renderingData.cullResults, ref occluderSettings, ref occluderFilteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(normals.id);
        }

    }

    private class OutlinePass : ScriptableRenderPass {
        private readonly Material OutlineMaterial;
        RenderTargetIdentifier cameraColorTarget;
        RenderTargetIdentifier temporaryBuffer;
        int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");

        public OutlinePass(RenderPassEvent renderPassEvent, OutlineSettings settings) {
            this.renderPassEvent = renderPassEvent;

            OutlineMaterial = new Material(Shader.Find("Hidden/OutlineShader"));
            // OutlineMaterial = new Material(Shader.Find("Hidden/Outlines"));
            OutlineMaterial.SetFloat("_DepthOutlineScale", settings.depthOutlineScale);
            OutlineMaterial.SetFloat("_HighlightPower", settings.highlightPower);
            OutlineMaterial.SetFloat("_ShadowPower", settings.shadowPower);
            OutlineMaterial.SetFloat("_NormalOutlineScale", settings.normalsOutlineScale);
            OutlineMaterial.SetFloat("_DepthThreshold", settings.depthThreshold);
            OutlineMaterial.SetFloat("_NormalThreshold", settings.normalThreshold);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            RenderTextureDescriptor temporaryTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            temporaryTargetDescriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(temporaryBufferID, temporaryTargetDescriptor, FilterMode.Bilinear);
            temporaryBuffer = new RenderTargetIdentifier(temporaryBufferID);

            cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (!OutlineMaterial)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines"))) {

                Blit(cmd, cameraColorTarget, temporaryBuffer);
                Blit(cmd, temporaryBuffer, cameraColorTarget, OutlineMaterial);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(temporaryBufferID);
        }

    }

    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    [SerializeField] private LayerMask outlinesLayerMask;
    [SerializeField] private LayerMask outlinesOccluderLayerMask;
    
    [SerializeField] private OutlineSettings outlineSettings = new OutlineSettings();
    [FormerlySerializedAs("viewSpaceNormalsTextureSettings")] [SerializeField] private NormalsTextureSettings normalsTextureSettings = new NormalsTextureSettings();

    private NormalsTexturePass _normalsTexturePass;
    private OutlinePass _outlinePass;
    
    public override void Create() {
        if (renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
            renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

        _normalsTexturePass = new NormalsTexturePass(renderPassEvent, outlinesLayerMask, outlinesOccluderLayerMask, normalsTextureSettings);
        _outlinePass = new OutlinePass(renderPassEvent, outlineSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // if (renderingData.cameraData.camera.cameraType != CameraType.Game) return;
        // TAG ANY CAMERA THAT SHOULD RENDER OUTLINES WITH MainCamera TAG
        if (!renderingData.cameraData.camera.CompareTag("MainCamera")) return;
        renderer.EnqueuePass(_normalsTexturePass);
        renderer.EnqueuePass(_outlinePass);
    }

}
