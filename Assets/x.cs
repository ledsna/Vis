using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]

[RequireComponent(typeof(Camera))]
public class CameraOpaqueLayerSwitcher : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    [SerializeField] private LayerMask transparentLayer;

    [SerializeField] private UniversalRendererData rendererData;
    private LayerMask originalOpaqueLayerMask;
    private LayerMask originalTransparentLayerMask;
    private bool layerMaskChanged = false;

    void Awake()
    {
        // Correct method name from OnAwake to Awake
        // Backup the original opaque and transparent layer masks
        originalOpaqueLayerMask = rendererData.opaqueLayerMask;
        originalTransparentLayerMask = rendererData.transparentLayerMask;
    }

    void OnEnable()
    {
        // Subscribe to render events
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    void OnDisable()
    {
        // Restore the original layer masks if they were changed
        if (layerMaskChanged)
        {
            rendererData.opaqueLayerMask = originalOpaqueLayerMask;
            rendererData.transparentLayerMask = originalTransparentLayerMask;
        }

        // Unsubscribe from render events
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == GetComponent<Camera>())
        {
            // Set the opaque and transparent layer masks before rendering
            rendererData.opaqueLayerMask = layer;
            rendererData.transparentLayerMask = transparentLayer;
            layerMaskChanged = true;
        }
    }

    private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == GetComponent<Camera>() && layerMaskChanged)
        {
            // Restore the original opaque and transparent layer masks after rendering
            rendererData.opaqueLayerMask = originalOpaqueLayerMask;
            rendererData.transparentLayerMask = originalTransparentLayerMask;
            layerMaskChanged = false;
        }
    }
}
