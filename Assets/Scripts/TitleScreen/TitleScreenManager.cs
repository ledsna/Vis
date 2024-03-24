using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    [SerializeField] private GameObject mainMenuObject;

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

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
        mainMenuObject.SetActive(false);
        Debug.Log("New Game!");
    }

    public void LoadGame()
    {
        WorldSaveGameManager.instance.LoadGame();
        mainMenuObject.SetActive(false);
        Debug.Log("Loading Game!");
    }
}