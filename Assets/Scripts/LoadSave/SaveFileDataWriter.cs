using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveFileDataWriter
{
    public string saveDataDirectoryPath = "";
    public string saveFileName = "";

    // before we create a new safe file, we must check to see if one of this character slot already exists
    public bool CheckIfFileExists()
    {
        return File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    // used to delete character save files
    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    // used to create a save file upon starting new game
    public void CreateNewCharacterSaveFile(PlayerSaveData characterData)
    {
        // make a path to save the file
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        try
        {
            // create the directory the file will be written to, if it does not already exist
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("Creating save file, at save path: " + savePath);

            // serialize the c# game data object into json
            string dataToStore = JsonUtility.ToJson(characterData, true);

            // write the file to our system
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter fileWriter = new StreamWriter(stream))
                {
                    fileWriter.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR WHILE TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED" + savePath + "\n" + ex);
        }
    }

    // used to load a save file upon loading a previous game
    public PlayerSaveData LoadSaveFile()
    {
        PlayerSaveData characterData = null;

        // make a path to save the file
        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if (File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // deserialize the data from json back to unity
                characterData = JsonUtility.FromJson<PlayerSaveData>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.LogError("ERROR WHILE TRYING TO LOAD CHARACTER DATA" + loadPath + "\n" + ex);
            }
        }

        return characterData;
    }

}