using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformManager : MonoBehaviour
{
    private float fallDelay = 1f;
    private float destroyDelay = 2f;
    public float respawnDelay = 5f;

    private Rigidbody rigidbody;
    private MovingPlatformTrigger trigger;
    
    public Vector3 startPosition;
    public Quaternion startRotation;

    public bool isFalling = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        trigger = GetComponentInChildren<MovingPlatformTrigger>();
        
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rigidbody.isKinematic = false;
        
        yield return new WaitForSeconds(destroyDelay);
        rigidbody.isKinematic = true; // reset the Rigidbody to kinematic
        transform.rotation = startRotation; // Reset rotation
        rigidbody.velocity = Vector3.zero; // Reset velocity
        rigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
        isFalling = false;
        
        yield return new WaitForSeconds(respawnDelay);
        transform.position = startPosition; // Reset position
    }

    private void Update()
    {
        if (trigger.player && !isFalling)
        {
            StartCoroutine(Fall());
            isFalling = true;
        }
    }
}
