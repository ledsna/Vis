using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot_Isometric : MonoBehaviour
{
    public float targetAngle = 45f;
    public float currentAngle = 0f;
    public float mouseSensitivity = 8f;
    public float rotationSpeed = 5f;

    public Transform player;

    [Header("Camera Settings")]
    private float cameraSmoothSpeed = 1; // the bigger this number is the longer it takes your camera to catch up
    private Vector3 cameraVelocity;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButton(0))
        {
            targetAngle += mouseX * mouseSensitivity;
        }
        else
        {
            targetAngle = Mathf.Round(targetAngle / 45);
            targetAngle *= 45;
        }

        if (targetAngle < 0)
        {
            targetAngle += 360;
        }

        if (targetAngle > 360)
        {
            targetAngle -= 360;
        }

        currentAngle = Mathf.LerpAngle(transform.eulerAngles.y,
            targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(30, currentAngle, 0);

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
