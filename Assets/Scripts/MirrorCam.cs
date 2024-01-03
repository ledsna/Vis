using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode, AddComponentMenu("Rendering/Planar Reflections Probe")]
public class MirrorCam : MonoBehaviour {

    [Range(1, 4)] public int targetTextureID = 1;
    [Space(10)]
    public bool useCustomNormal;
    public Vector3 customNormal;
    [Space(10)]
    [Range(0.01f, 1.0f)] public float reflectionsQuality = 1f;
    public float farClipPlane = 1000;
    public bool renderSkybox;
    [Space(10)] public bool renderInEditor;

    public Camera mirror;
    private Skybox mirrorSkybox;
    private RenderTexture renderTexture;

    private void OnEnable ()
    {
        mirror = GetComponent<Camera>();
        mirror.targetTexture = renderTexture;
        RenderPipelineManager.beginCameraRendering += PreRender;
    }

    private void OnDisable() {
        RenderPipelineManager.beginCameraRendering -= PreRender;
        if (renderTexture) renderTexture.Release();
    }

    private void PreRender(ScriptableRenderContext context, Camera viewer)
    {
        if (viewer.cameraType == CameraType.Reflection || viewer.cameraType == CameraType.Preview) return;
        if (!renderInEditor && viewer.cameraType == CameraType.SceneView) return;
        
        UpdateProbeSettings(viewer);

        Vector3 normal = GetNormal();
        UpdateProbeTransform(viewer.transform, normal);
        CalculateObliqueProjection(viewer, normal);

        UniversalRenderPipeline.RenderSingleCamera(context, mirror);

        mirror.targetTexture.SetGlobalShaderProperty("_Reflection" + targetTextureID);
    }
    
    private void UpdateProbeSettings(Camera viewer) {
        mirror.enabled = false;
        mirror.CopyFrom(viewer);
        mirror.farClipPlane = farClipPlane;
        mirror.usePhysicalProperties = false;
        mirror.backgroundColor = Color.black;
        mirror.cameraType = CameraType.Reflection;
        mirror.clearFlags = renderSkybox ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
        
        var width = (int) (viewer.pixelWidth * reflectionsQuality);
        var height = (int) (viewer.pixelHeight * reflectionsQuality);
        if (renderTexture && renderTexture.width == width && renderTexture.height == height) return;
        if (renderTexture) renderTexture.Release();
        renderTexture = new RenderTexture(width, height, 24);
        mirror.targetTexture = renderTexture;
    }
    
    private Vector3 GetNormal () {
        if (!useCustomNormal) return transform.parent.up;
        return customNormal.Equals(Vector3.zero) ? Vector3.up : customNormal.normalized;
    }

    private void UpdateProbeTransform(Transform viewer, Vector3 normal)
    {
        var pixelSizeInUnits = mirror.orthographicSize * 2 / viewer.GetComponent<Camera>().scaledPixelHeight; // Units per Pixel

        var viewerPosition = viewer.position;
        var proj = normal * Vector3.Dot(normal, viewerPosition - (transform.parent.position + pixelSizeInUnits * GetNormal()));
        transform.position = viewerPosition - 2 * proj;
        
        var probeForward = Vector3.Reflect(viewer.forward, normal);
        var probeUp = Vector3.Reflect(viewer.up, normal);
        transform.LookAt(transform.position + probeForward, probeUp);
    }
    
    private void CalculateObliqueProjection (Camera viewer, Vector3 normal)
    {
        // 0.00007 Units at least
        var pixelSizeInUnits = mirror.orthographicSize * 2 / viewer.scaledPixelHeight; // Units per Pixel
        var viewMatrix = mirror.worldToCameraMatrix;
        var viewPosition = viewMatrix.MultiplyPoint(transform.parent.position + pixelSizeInUnits * GetNormal());
        var viewNormal = viewMatrix.MultiplyVector(normal).normalized;
        var plane = new Vector4(viewNormal.x, viewNormal.y, viewNormal.z,
            -Vector3.Dot(viewPosition, viewNormal));
        mirror.projectionMatrix = mirror.CalculateObliqueMatrix(plane);
    }
}