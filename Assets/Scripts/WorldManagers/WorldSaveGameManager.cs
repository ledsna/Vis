using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    public PlayerManager player; 

    [Header("World Scene Index")]
    public int worldSceneIndex = 1;

    [Header("Save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

    [Header("Current Character Data")]
    public PlayerSaveData playerData;
    private string saveFileName = "saveFile";

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
        if (!saveFileDataWriter.CheckIfFileExists())
        {
            playerData = new PlayerSaveData();
            NewGame();
        }
        else
        {
            playerData = new PlayerSaveData();
            NewGame();
        }
    }

    private void NewGame()
    {
        // saves the newly created character stats, and items (when creation screen is added)
        SaveGame();
        StartCoroutine(LoadWorldScene());
    }

    public void LoadGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        // generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        playerData = saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());
    }

    public void SaveGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        // generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        // pass the players info, from game, to their save file
        player.SaveGameDataToCurrentCharacterData(ref playerData);

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

    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        player.LoadGameDataFromCurrentCharacterData(ref playerData);

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