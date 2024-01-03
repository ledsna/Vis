using System.Collections;
using System.Collections.Generic;
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
    private bool cameraIsMoving = false;


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

            transform.position += movementDirection * cameraLookSpeed * Time.deltaTime;
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

    private void AdjustCameraPosition()
    {
        float pixelsPerUnit = mainCamera.scaledPixelHeight / mainCamera.orthographicSize / 2;
        
        float newPosX = Mathf.Round(transform.position.x * pixelsPerUnit) / pixelsPerUnit;
        float xDifference = newPosX - transform.position.x;
        
        float newPosY = Mathf.Round(transform.position.y * pixelsPerUnit) / pixelsPerUnit;
        float yDifferece = newPosY - transform.position.y; 
            
        float newPosZ = Mathf.Round(transform.position.z * pixelsPerUnit) / pixelsPerUnit;
        float zDifference = newPosZ - transform.position.z;

        transform.position = new Vector3(newPosX, newPosY, newPosZ);
    }
}
