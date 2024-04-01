using System;
using UnityEngine;
using System.Collections;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager instance;

    public GameObject[] platforms; // Assign in the inspector
    public float respawnDelay = 5f;

    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this one
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void DisablePlatform(GameObject platform)
    {
        platform.SetActive(false);
        StartCoroutine(RespawnPlatform(platform));
    }

    private IEnumerator RespawnPlatform(GameObject platform)
    {
        yield return new WaitForSeconds(respawnDelay);
        platform.SetActive(true);
        Rigidbody rb = platform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        FallingPlatformManager fpm = gameObject.GetComponent<FallingPlatformManager>();
        platform.transform.position = fpm.startPosition;
        platform.transform.rotation = fpm.startRotation;
        fpm.isFalling = false;
    }
}