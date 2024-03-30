using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    public PlayerManager player; 

    [Header("Save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

    [Header("Character Data")]
    public PlayerSaveData playerData;
    private string saveFileName = "saveFile";
    private int currentSceneIndex;

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

    public void AttemptToCreateNewGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        // check if we can create a new save file (check for other existing files first)
        saveFileDataWriter.saveFileName = saveFileName;

        // if this profile slot is not taken, we gonna use it
        playerData = new PlayerSaveData(); 
        NewGame();
    }

    private void NewGame()
    {
        // saves the newly created character stats, and items (when creation screen is added)
        SaveGame(true);
        LoadGame();
    }

    public void LoadGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        // generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        playerData = saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadScene(1));
    }

    public void SaveGame(bool newGame=false)
    {
        saveFileDataWriter = new SaveFileDataWriter();
        // generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        if (newGame)
        {
            playerData.xPosition = 16f;
            playerData.yPosition = 2.3f;
            playerData.zPosition = -1;
        }
        else
        {
            // pass the players info, from game, to their save file
            player.SaveGameDataToCurrentCharacterData(ref playerData);
        }

        // write that info onto a json file, saved to this machine
        saveFileDataWriter.CreateNewCharacterSaveFile(playerData);
    }

    public void DeleteGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        saveFileDataWriter.DeleteSaveFile();
    }

    public IEnumerator LoadScene(int sceneIndex)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        currentSceneIndex = sceneIndex;

        player.LoadGameDataFromCurrentCharacterData(ref playerData);

        yield return null;
    }

    public IEnumerator LoadNextScene()
    {
        currentSceneIndex = (currentSceneIndex + 1) % 6;
        
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentSceneIndex);

        yield return null;
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            SaveGame();
        }
    }
}