using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class RenderTerrainMap : MonoBehaviour
{
    public RenderTexture tempTex; // Temporary texture for rendering

    
    public Camera camToDrawWith;
    // [SerializeField] private LayerMask layer; // Layer to render
    [SerializeField] private Renderer[] renderers; // Objects to render
    [SerializeField] private Terrain[] terrains; // Unity terrains to render
    public int resolution = 2048; // Map resolution
    public float adjustScaling = 2.5f; // Padding the total size
    [SerializeField] private bool RealTimeDiffuse; // Toggle for real-time updating
    public float repeatRate = 5f; // Update rate for real-time diffuse map generation
    private Bounds bounds; // Combined bounds of all renderers and terrains

    public UniversalRendererData rendererData; // Reference to the UniversalRendererData
    private LayerMask originalOpaqueLayerMask; // To store the original opaque layer mask
    private LayerMask originalTransparentLayerMask; // To store the original transparent layer mask

    void OnEnable()
    {
        bounds = new Bounds(transform.position, Vector3.zero);
        // tempTex = new RenderTexture(resolution, resolution, 24);
        GetBounds();
        SetUpCam();
        DrawDiffuseMap();
    }

    void Start()
    {
        if (RealTimeDiffuse)
        {
            InvokeRepeating("UpdateTex", 1f, repeatRate);
        }
    }

    void GetBounds()
    {
        foreach (Renderer renderer in renderers)
        {
            if (bounds.size == Vector3.zero)
            {
                bounds = new Bounds(renderer.transform.position, Vector3.zero);
            }
            bounds.Encapsulate(renderer.bounds);
        }

        foreach (Terrain terrain in terrains)
        {
            if (bounds.size == Vector3.zero)
            {
                bounds = new Bounds(terrain.transform.position, Vector3.zero);
            }
            Vector3 terrainCenter = terrain.GetPosition() + terrain.terrainData.bounds.center;
            Bounds worldBounds = new Bounds(terrainCenter, terrain.terrainData.bounds.size);
            bounds.Encapsulate(worldBounds);
        }
    }

    void UpdateTex()
    {
        DrawDiffuseMap();
    }

    public void DrawDiffuseMap()
    {
        DrawToMap("_TerrainDiffuse");
    }

    void DrawToMap(string target)
    {
        camToDrawWith.enabled = true;
        camToDrawWith.targetTexture = tempTex;
        camToDrawWith.depthTextureMode = DepthTextureMode.Depth;

        Shader.SetGlobalFloat("_OrthographicCamSizeTerrain", camToDrawWith.orthographicSize);
        Shader.SetGlobalVector("_OrthographicCamPosTerrain", camToDrawWith.transform.position);
        camToDrawWith.Render();
        Shader.SetGlobalTexture(target, tempTex);

        camToDrawWith.enabled = false;
    }

    void SetUpCam()
    {
        if (camToDrawWith == null)
        {
            camToDrawWith = GetComponentInChildren<Camera>();
        }
        float size = bounds.size.magnitude;
        // camToDrawWith.cullingMask = layer;
        camToDrawWith.orthographicSize = size / adjustScaling;
        camToDrawWith.transform.parent = null;
        camToDrawWith.transform.position = bounds.center + new Vector3(0, bounds.extents.y + 5f, 0);
        camToDrawWith.transform.LookAt(bounds.center);
        camToDrawWith.transform.parent = transform;
    }
}
