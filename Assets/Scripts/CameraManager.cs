using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public Transform player;

    [Header("Camera Settings")]
    private float cameraSmoothSpeed = 1; // the bigger this number is the longer it takes your camera to catch up
    private Vector3 cameraVelocity;

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
