using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    
    [SerializeField] Camera mainCamera;
    [FormerlySerializedAs("rawImage")] [SerializeField] RawImage screenTexture;
    // [SerializeField]
    Transform player;

    [Header("Targeted Settings")]
    [SerializeField] float cameraSmoothTime = 7;
    private Vector3 currentVelocity;

    [Header("Untargeted Settings")] 
    [SerializeField] float cameraSpeed = 10;

    [Header("Rotation Settings")]
    [SerializeField] float angleIncrement = 45f;
    [SerializeField] float targetAngle = 45f;
    [SerializeField] float mouseSensitivity = 8f;
    [SerializeField] float rotationSpeed = 5f;
    private float angleThreshold = 0.05f;
    private float currentAngle;
    
    [Header("Zoom settings")]
    [SerializeField] float zoomSpeed = 5000f; // Speed of zoom
    [SerializeField] float minZoom = 1f; // Minimum zoom level
    [SerializeField] float maxZoom = 20f; // Maximum zoom level
    [SerializeField] float zoomSmoothness = 10f; // Smoothness of the zoom transition
    private float targetZoom;
    private float zoomLerpRate;
    private float zoom = 1;
    
    [Header("Blit to Viewport")]
    private Vector3 offsetSS = Vector3.zero;
    private Vector3 originWS;
    private float pixelW;
    private float pixelH;

    private void Start()
    {
        // Fraction of pixel size to screen size
        pixelW = 1f / mainCamera.scaledPixelWidth;
        pixelH = 1f / mainCamera.scaledPixelHeight;
        // Offsetting vertical and horizontal positions by 1 pixel
        //  and shrinking the screen size by 2 pixels from each side
        // mainCamera.pixelRect = new Rect(1, 1, mainCamera.pixelWidth - 2, mainCamera.pixelHeight - 2);
        screenTexture.uvRect = new Rect(pixelW, pixelH, 1f - 2 * pixelW, 1f - 2 * pixelH);
        
        originWS = transform.position;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        // Application.targetFrameRate = -1; // Uncapped
        float mouseX = Input.GetAxis("Mouse X");
        // float mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButton(0))
            // While holding LMB, the Camera will follow the cursor
            targetAngle += mouseX * mouseSensitivity;
        else
        {
            // After that, it'll snap to the closest whole increment angle
            targetAngle = Mathf.Round(targetAngle / angleIncrement);
            targetAngle *= angleIncrement;
        }

        targetAngle = (targetAngle + 360) % 360;
        currentAngle = Mathf.LerpAngle(transform.eulerAngles.y,
            targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(30, currentAngle, 0);

        if (Mathf.Abs(targetAngle - currentAngle) < angleThreshold) return;
        originWS = transform.position;
        offsetSS = Vector3.zero;
    }

    private void LateUpdate()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // Get mouse wheel input
        if (scroll != 0)
        {
            targetZoom += scroll * zoomSpeed; // Calculate target zoom level based on input
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom); // Clamp target zoom to min/max bounds
            zoomLerpRate = 1f - Mathf.Pow(1f - zoomSmoothness * Time.deltaTime, 3);
            // mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, zoomLerpRate);
            Zoom(Mathf.Lerp(zoom, targetZoom, zoomLerpRate));
            zoom = targetZoom;
        }
        
        // Unlocked camera
        if (PlayerInputManager.instance.cameraMovementInput != Vector2.zero)
        {
            // Normalize movement to ensure consistent speed
            Vector2 directionSS = PlayerInputManager.instance.cameraMovementInput.normalized;
            Vector3 directionWS = transform.right * directionSS.x + transform.up * directionSS.y;
            transform.position += Time.deltaTime * cameraSpeed * directionWS;
        }
        // Locked camera
        else if (player is not null)
        {
            Vector3 targetCameraPosition = 
                Vector3.SmoothDamp(transform.position, 
                    player.transform.position, 
                    ref currentVelocity, 
                    cameraSmoothTime * Time.deltaTime);

            transform.position = targetCameraPosition;
        }
        AdjustCameraPosition();
    }

    private Vector3 ToWorldSpace(Vector3 vector)
    {
        return transform.TransformVector(vector);
    }

    private Vector3 ToScreenSpace(Vector3 vector)
    {
        return transform.InverseTransformVector(vector);
    }

    private void Zoom(float targetZoom)
    {
        Rect uvRect = screenTexture.uvRect;
        uvRect.width = (1f - 2 * pixelW) / targetZoom;
        uvRect.height = (1f - 2 * pixelH) / targetZoom;
        uvRect.x = pixelW + (1f - uvRect.width) / 2;
        uvRect.y = pixelH + (1f - uvRect.height) / 2;
        screenTexture.uvRect = uvRect;
    }
    
    private void AdjustCameraPosition()
    {
        // Calculate Pixels per Unit
        float ppu = mainCamera.scaledPixelHeight / mainCamera.orthographicSize / 2;
        
        // Convert the ( origin --> current position) vector from World Space to Screen Space and add the offset
        Vector3 toCurrentPosSS = ToScreenSpace(transform.position - originWS) + offsetSS;
        // Snap the Screen Space position vector to the closest Screen Space texel
        Vector3 toCurrentSnappedPosSS = new Vector3(
            Mathf.Round(toCurrentPosSS.x * ppu),
            Mathf.Round(toCurrentPosSS.y * ppu),
            Mathf.Round(toCurrentPosSS.z * ppu)) / ppu;
        
        // Convert the displacement vector to World Space and add to the origin in World Space
        transform.position = originWS + ToWorldSpace(toCurrentSnappedPosSS);
        // Difference between the initial and snapped positions
        offsetSS = toCurrentPosSS - toCurrentSnappedPosSS;

        Rect uvRect = screenTexture.uvRect;
        
        // Offset the Viewport by 1 - offset pixels in both dimensions
        uvRect.x = (1f + offsetSS.x * ppu) * pixelW + (1f - uvRect.width) / 2;
        uvRect.y = (1f + offsetSS.y * ppu) * pixelH + (1f - uvRect.height) / 2;
        // Blit to Viewport
        screenTexture.uvRect = uvRect;
    }
}
