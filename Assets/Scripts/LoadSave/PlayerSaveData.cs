using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// since we want to reference this data for every save file, this script is not a monobehaviour and is instead serializable
public class PlayerSaveData
{
    [Header("Time Played")]
    public float secondsPlayed;

    [Header("Current Floor")] 
    public int floor = 1;

    [Header("World Coordinates")]
    public float xPosition = 0;
    public float yPosition = 2;
    public float zPosition = 0;
}