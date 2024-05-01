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
    [SerializeField] RawImage rawImage;
    // [SerializeField]
    Transform player;

    [Header("Locked Settings")]
    [SerializeField] float cameraSmoothTime = 7;
    private Vector3 currentVelocity;

    [Header("Unlocked Settings")] 
    [SerializeField] float cameraSpeed = 10;

    [Header("Rotation Settings")]
    [SerializeField] float angleIncrement = 45f;
    [SerializeField] float targetAngle = 45f;
    [SerializeField] float mouseSensitivity = 8f;
    [SerializeField] float rotationSpeed = 5f;
    private float angleThreshold = 0.05f;
    private float currentAngle;
    
    [Header("Blit to Viewport")]
    private Vector3 ssOffset = Vector3.zero;
    private float pixelW;
    private float pixelH;
    private Vector3 wsOrigin;

    private void Start()
    {
        // Fraction of pixel size to screen size
        pixelW = 1f / mainCamera.scaledPixelWidth;
        pixelH = 1f / mainCamera.scaledPixelHeight;
        // Offsetting vertical and horizontal positions by 1 pixel
        //  and shrinking the screen size by 2 pixels from each side
        // mainCamera.pixelRect = new Rect(1, 1, mainCamera.pixelWidth - 2, mainCamera.pixelHeight - 2);
        rawImage.uvRect = new Rect(pixelW, pixelH, 1f - 2 * pixelW, 1f - 2 * pixelH);
        
        wsOrigin = transform.position;
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
        wsOrigin = transform.position;
        ssOffset = Vector3.zero;

        // AdjustCameraPosition();
    }

    private void LateUpdate()
    {
        // Unlocked camera
        if (PlayerInputManager.instance.cameraMovementInput != Vector2.zero)
        {
            // Normalize movement to ensure consistent speed
            Vector2 ssDirection = PlayerInputManager.instance.cameraMovementInput.normalized;
            Vector3 wsDirection = transform.right * ssDirection.x + transform.up * ssDirection.y;
            transform.position += Time.deltaTime * cameraSpeed * wsDirection;
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

    private void AdjustCameraPosition()
    {
        // Calculate Pixels per Unit
        float ppu = mainCamera.scaledPixelHeight / mainCamera.orthographicSize / 2;
        
        // Convert the ( origin --> current position) vector from World Space to Screen Space and add the offset
        Vector3 ssToCurrentPos = ToScreenSpace(transform.position - wsOrigin) + ssOffset;
        // Snap the Screen Space position vector to the closest Screen Space texel
        Vector3 ssToCurrentSnappedPos = new Vector3(
            Mathf.Round(ssToCurrentPos.x * ppu),
            Mathf.Round(ssToCurrentPos.y * ppu),
            Mathf.Round(ssToCurrentPos.z * ppu)) / ppu;
        
        // Convert the displacement vector to World Space and add to the origin in World Space
        transform.position = wsOrigin + ToWorldSpace(ssToCurrentSnappedPos);
        // Difference between the initial and snapped positions
        ssOffset = ssToCurrentPos - ssToCurrentSnappedPos;

        Rect uvRect = rawImage.uvRect;
        // Offset the Viewport by 1 - offset pixels in both dimensions
        
        uvRect.x = (1f + ssOffset.x * ppu) * pixelW;
        uvRect.y = (1f + ssOffset.y * ppu) * pixelH;
        // Blit to Viewport
        rawImage.uvRect = uvRect;
    }
}
