using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public Transform player;

    [Header("Camera Settings")]
    [SerializeField] float cameraSmoothSpeed = 7; // the bigger this number is the longer it takes your camera to catch up
    private Vector3 cameraVelocity;
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

            transform.position += movementDirection * cameraLookSpeed * Time.deltaTime;

            return;
        }

        if (player != null)
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp
            (transform.position,
            player.transform.position,
            ref cameraVelocity,
            cameraSmoothSpeed * Time.deltaTime);

            transform.position = targetCameraPosition;
        }
    }
}
