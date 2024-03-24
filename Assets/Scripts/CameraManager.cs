using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    
    [SerializeField] Camera mainCamera;
    [SerializeField] RawImage rawImage;
    public Transform player;

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
    private float currentAngle = 0f;
    
    [Header("Blit to Viewport")]
    private Vector3 snapOffset = Vector3.zero;
    private float pixelW;
    private float pixelH;
    private Vector3 wsOrigin;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        // Fraction of pixel size to screen size
        pixelW = 1f / mainCamera.scaledPixelWidth;
        pixelH = 1f / mainCamera.scaledPixelHeight;
        // Offsetting vertical and horizontal positions by 1 pixel
        //  and shrinking the screen size by 2 pixels from each side
        // mainCamera.pixelRect = new Rect(1, 1, mainCamera.pixelWidth - 2, mainCamera.pixelHeight - 2);
        rawImage.uvRect = new Rect(pixelW, pixelH, 1f - 2 * pixelW, 1f - 2 * pixelH);
        
        wsOrigin = transform.position;
    }

    void Update()
    {
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
        
        if (Mathf.Abs(targetAngle - currentAngle) > angleThreshold) return;
        wsOrigin = transform.position;
        snapOffset = Vector3.zero;
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

    private void AdjustCameraPosition()
    {
        // Calculate Pixels per Unit
        float ppu = mainCamera.scaledPixelHeight / mainCamera.orthographicSize / 2;
        // Current unsnapped position = previous snapped position + unsnapped offset
        Vector3 pos = transform.position - transform.TransformVector(snapOffset);
        // Convert the ( origin->position ) vector from World Space to Screen Space
        Vector3 ssPos = transform.InverseTransformVector(pos - wsOrigin);
        // Snap the Screen Space position vector to the closest Screen Space texel
        Vector3 ssPosSnapped = new Vector3(
            Mathf.Round(ssPos.x * ppu),
            Mathf.Round(ssPos.y * ppu),
            Mathf.Round(ssPos.z * ppu)) / ppu;
        
        // Convert the snapped Screen Space position vector to World Space and add back to the origin in World Space
        transform.position = wsOrigin + transform.TransformVector(ssPosSnapped);
        // Difference between the initial and snapped positions from World Space to Screen Space
        snapOffset = ssPosSnapped - ssPos;

        Rect uvRect = rawImage.uvRect;
        // Offset the Viewport by 1 - offset pixels in both dimensions
        
        uvRect.x = (1f - snapOffset.x * ppu) * pixelW;
        uvRect.y = (1f - snapOffset.y * ppu) * pixelH;
        // Blit to Viewport
        rawImage.uvRect = uvRect;
    }
}