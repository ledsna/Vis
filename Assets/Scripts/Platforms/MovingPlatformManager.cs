using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovingPlatformManager : MonoBehaviour
{
    [SerializeField] private WaypointManager waypointManager;

    [Header("Platform Settings")]
    [SerializeField] private float velocity;
    private int targetWaypointIndex;
    private Transform previousWaypoint;
    private Transform targetWaypoint;
    private float timeToWaypoint;
    private float elapsedTime;

    private void Start()
    {
        TargetNextWaypoint();
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        float elapsedPercentage = elapsedTime / timeToWaypoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(previousWaypoint.position, targetWaypoint.position, elapsedPercentage);
        transform.rotation = Quaternion.Lerp(previousWaypoint.rotation, targetWaypoint.rotation, elapsedPercentage);

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

    private void OnCollisionEnter(Collision other)
    {
        other.transform.SetParent(transform);

    }

    private void OnCollisionExit(Collision other)
    {
        other.transform.SetParent(null);
    }
}
