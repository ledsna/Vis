using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform GetWaypoint(int waypointIndex)
    {
        return transform.GetChild(waypointIndex);
    }

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        return (currentWaypointIndex + 1) % transform.childCount;
    }
}
