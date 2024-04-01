using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject pauseMenuObject;
    [SerializeField] private GameObject endGameObject;
    
    [Header("Fade")]
    public Image fadeOutUIImage;
    public float fadeSpeed = 0.8f;

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

    public void OpenEndGame()
    {
        Time.timeScale = 0f;
        pauseMenuObject.SetActive(false);
        endGameObject.SetActive(true);
    }
    
    public IEnumerator FadeIn(float time)
    {
        yield return Fade(1, 0, time);
    }

    public IEnumerator FadeOut(float time)
    {
        yield return Fade(0, 1, time);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float time)
    {
        Debug.Log("let's goooooo");
        float elapsedTime = 0.0f;
        Color c = fadeOutUIImage.color;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / time);
            fadeOutUIImage.color = c;
            yield return null;
        }
    }
}