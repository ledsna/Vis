using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;

    [Header("Layers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask environmentLayers;

    [Header("Game State")] public bool gameIsPause = false;
    
    private void Awake()
    {
        if (instance is null)
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

    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }

    public LayerMask GetEnvironmentLayers()
    {
        return environmentLayers;
    }
}