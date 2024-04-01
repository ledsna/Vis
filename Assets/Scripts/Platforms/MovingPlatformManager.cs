using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovingPlatformManager : MonoBehaviour
{
    [SerializeField] private WaypointManager waypointManager;
    private MovingPlatformTrigger trigger;

    [Header("Platform Settings")]
    [SerializeField] private float velocity;
    private int targetWaypointIndex;
    private Transform previousWaypoint;
    private Transform targetWaypoint;
    private float timeToWaypoint;
    private float elapsedTime;

    private void Awake()
    {
        trigger = GetComponentInChildren<MovingPlatformTrigger>();
    }

    private void Start()
    {
        TargetNextWaypoint();
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        float elapsedPercentage = elapsedTime / timeToWaypoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        
        // Calculate the new position and rotation
        Vector3 newPosition = Vector3.Lerp(previousWaypoint.position, targetWaypoint.position, elapsedPercentage);
        Quaternion newRotation = Quaternion.Lerp(previousWaypoint.rotation, targetWaypoint.rotation, elapsedPercentage);
        
        Vector3 deltaPosition = newPosition - transform.position;
        Quaternion deltaRotation = newRotation * Quaternion.Inverse(transform.rotation);
        transform.position = newPosition;
        transform.rotation = newRotation;

        if (trigger.player)
        {
            trigger.player.transform.rotation *= deltaRotation;
            trigger.player.locomotionManager.SetExternalForces(deltaPosition, deltaRotation);
        }
    
        if (elapsedPercentage >= 1)
        {
            TargetNextWaypoint();
        }
    }
    
    private void TargetNextWaypoint()
    {
        previousWaypoint = waypointManager.GetWaypoint(targetWaypointIndex);
        targetWaypointIndex = waypointManager.GetNextWaypointIndex(targetWaypointIndex);
        targetWaypoint = waypointManager.GetWaypoint(targetWaypointIndex);

        elapsedTime = 0;
        float distanceToWaypoint = Vector3.Distance(previousWaypoint.position, targetWaypoint.position);
        timeToWaypoint = distanceToWaypoint / velocity;
    }
}
