using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    
    [SerializeField] Camera mainCamera;
    [SerializeField] RawImage rawImage;
    [SerializeField] Transform player;

    [Header("Camera Follow Settings")]
    [SerializeField] float cameraSmoothTime = 7; // the bigger this number is the longer it takes your camera to catch up

    private Vector3 cameraVelocity;

    [Header("Camera Overviewing Settings")] 
    [SerializeField] float cameraSpeed = 10;

    [Header("Camera Rotation Settings")]
    public float angleThreshold = 0.05f;
    public float incrementAngle = 45f;
    
    public float targetAngle = 45f;
    private float currentAngle = 0f;
    public float mouseSensitivity = 8f;
    public float rotationSpeed = 5f;
    
    [Header("Blit to Viewport")]
    [SerializeField] public Vector3 offset = Vector3.zero;
    private Vector3 wsOrigin;
    public float pixelW;
    public float pixelH;

    private void Start()
    {
        // Fraction of pixel size to screen size
        pixelW = 1f / mainCamera.scaledPixelWidth;
        pixelH = 1f / mainCamera.scaledPixelHeight;
        
        var uvRect = rawImage.uvRect;
        
        // Offsetting vertical and horizontal positions by 1 pixel
        uvRect.x = pixelW;
        uvRect.y = pixelH;
        
        // Shrinking the screen size by 2 pixels from each side
        uvRect.width = 1f - 2 * pixelW;
        uvRect.height = 1f - 2 * pixelH;
        
        rawImage.uvRect = uvRect;
    }

    private void Awake()
    {
        wsOrigin = transform.position;
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        // float mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButton(0))
        {
            targetAngle += mouseX * mouseSensitivity;
        }
        else
        {
            targetAngle = Mathf.Round(targetAngle / incrementAngle);
            targetAngle *= incrementAngle;
        }

        if (targetAngle < 0) targetAngle += 360;
        else if (targetAngle > 360) targetAngle -= 360;

        currentAngle = Mathf.LerpAngle(transform.eulerAngles.y,
            targetAngle, rotationSpeed * Time.deltaTime);

        var originalRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(30, currentAngle, 0);
        if (Mathf.Abs(targetAngle - currentAngle) > angleThreshold)
        {
            wsOrigin = transform.position;
            offset = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        if (PlayerInputManager.instance.cameraMovementInput != Vector2.zero)
        {
            var movementIn = PlayerInputManager.instance.cameraMovementInput.normalized;
            Vector3 movementDirection = transform.up * movementIn.y + transform.right * movementIn.x;

            transform.position += Time.deltaTime * cameraSpeed * movementDirection;
        }
        else if (player is not null)
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp
            (transform.position,
                player.transform.position,
                ref cameraVelocity,
                cameraSmoothTime * Time.deltaTime);

            // transform.position = targetCameraPosition;
        }

        AdjustCameraPosition();
    }

    private void AdjustCameraPosition()
    {
        // Pixels per Unit
        float ppu = mainCamera.scaledPixelHeight / mainCamera.orthographicSize / 2;
        Vector3 pos = transform.position - transform.TransformVector(offset);
        // Convert the ( origin->position ) vector from World Space to Screen Space
        Vector3 ssPos = transform.InverseTransformVector(pos - wsOrigin);
        // Snap the Screen Space position vector to the closest Screen Space texel
        Vector3 ssPosSnapped = new Vector3(
            Mathf.Round(ssPos.x * ppu),
            Mathf.Round(ssPos.y * ppu),
            Mathf.Round(ssPos.z * ppu)) / ppu;
        // Convert the snapped Screen Space position vector to World Space and add back to the origin in World Space
        transform.position = wsOrigin + transform.TransformVector(ssPosSnapped);
        // Convert the difference between the initial and snapped positions from World Space to Screen Space
        offset = ssPosSnapped - ssPos;
        
        var uvRect = rawImage.uvRect;
        uvRect.x = (1f - offset.x * ppu) * pixelW;
        uvRect.y = (1f - offset.y * ppu) * pixelH;
        rawImage.uvRect = uvRect;
    }
}