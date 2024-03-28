using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject pauseMenuObject;

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
        mainMenuObject.SetActive(true);
        pauseMenuObject.SetActive(false);
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
        mainMenuObject.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("New Game!");
    }

    public void LoadGame()
    {
        WorldSaveGameManager.instance.LoadGame();
        mainMenuObject.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Loading Game!");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void Pause()
    {
        if (mainMenuObject.activeSelf)
        {
            return;
        }
        
        WorldUtilityManager.instance.gameIsPause = true;
        pauseMenuObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        WorldUtilityManager.instance.gameIsPause = false;
        pauseMenuObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenMenu()
    {
        WorldSaveGameManager.instance.SaveGame();
        pauseMenuObject.SetActive(false);
        mainMenuObject.SetActive(true);
    }
}