using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] Camera mainCamera;
    
    [SerializeField] Transform player;
    
    [Header("Camera Settings")]
    [SerializeField] float cameraSmoothSpeed = 7; // the bigger this number is the longer it takes your camera to catch up
    private Vector3 cameraVelocity;

    [Header("Camera Free Rotation Settings")]
    [SerializeField] float cameraLookSpeed = 10;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (PlayerInputManager.instance.cameraMovementInput != Vector2.zero)
        {
            Vector3 movementDirection = transform.forward * PlayerInputManager.instance.cameraMovementInput.y +
                                        transform.right * PlayerInputManager.instance.cameraMovementInput.x;
            movementDirection.Normalize();
            movementDirection.y = 0;

            transform.position += Time.deltaTime * cameraLookSpeed * movementDirection;
        }
        else if (player is not null)
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp
            (transform.position,
            player.transform.position,
            ref cameraVelocity,
            cameraSmoothSpeed * Time.deltaTime);

            transform.position = targetCameraPosition;
        }
        
        AdjustCameraPosition();
    }
    
    bool IsApproximately(float value, float target, float threshold = 0.01f)
    {
        return Mathf.Abs(value - target) < threshold;
    }

    private void AdjustCameraPosition()
    {
        
        float pixelsPerUnit = mainCamera.scaledPixelHeight / mainCamera.orthographicSize / 4;
        Vector3 euler = transform.localEulerAngles;

        if (IsApproximately(euler.y, 45f) || IsApproximately(euler.y, 135f) ||
            IsApproximately(euler.y, 225f) || IsApproximately(euler.y, 315f))
        {
            pixelsPerUnit /= Mathf.Sqrt(2);
        }
        
        Debug.Log(euler.y);
        Debug.Log(pixelsPerUnit);
        
        float newPosX = Mathf.Round(transform.position.x * pixelsPerUnit) / pixelsPerUnit;
        
        float newPosY = Mathf.Round(transform.position.y * pixelsPerUnit) / pixelsPerUnit;
            
        float newPosZ = Mathf.Round(transform.position.z * pixelsPerUnit) / pixelsPerUnit;

        transform.position = new Vector3(newPosX, newPosY, newPosZ);
    }
}
