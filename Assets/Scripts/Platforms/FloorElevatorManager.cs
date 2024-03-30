using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorElevatorManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(WorldSaveGameManager.instance.LoadNextScene());
            Debug.Log("Reached end!");
        }
    }
}
