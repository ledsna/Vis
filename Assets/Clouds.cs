using UnityEngine;

public class MoveCloudShadows : MonoBehaviour
{
    public float rotationSpeed = 0.2f;

    void Update()
    {
        // Rotate the light around its Y axis at the specified speed
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}