//#define DEBUG_VerboseConsoleLogging
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// everything must be static in this class
public static class SaveGameManager
{
    private static SaveFileList saveFileList;
    private static SaveFile saveFile;
    private static string filePath;
    private static string mainFilePath;

    // LOCK, if true, prevents the game from saving. This avoids issues that can
    //  happen while loading files.
    static public bool LOCK
    {
        get;
        private set;
    }


    static SaveGameManager()
    {
        LOCK = false;
        mainFilePath = Application.dataPath + "/Warpaid.save"; // need to change to persistentDataPath when publicize 
        filePath = Application.dataPath + "PlayerOne.save"; // need to change to persistentDataPath when publicize 

#if DEBUG_VerboseConsoleLogging
        Debug.Log("SaveGameManager:Awake() – Path: " + filePath);
#endif

        saveFileList = new SaveFileList();
        saveFile = new SaveFile();
    }

    public static void SaveMainData()
    {
        // Do not save when is LOCKed
        if (LOCK) return;

        // import data to saveFile
        saveFileList = Warpaid.GetSaveFileList();


        string jsonSaveFile = JsonUtility.ToJson(saveFileList, true);

        File.WriteAllText(mainFilePath, jsonSaveFile);
#if DEBUG_VerboseConsoleLogging
        Debug.Log("SaveGameManager:SaveMainData() – Path: " + mainFilePath);
        Debug.Log("SaveGameManager:SaveMainData() – JSON: " + jsonSaveFile);
#endif
    }

    public static void LoadMainData()
    {
        if (File.Exists(mainFilePath))
        {
            string dataAsJson = File.ReadAllText(mainFilePath);
#if DEBUG_VerboseConsoleLogging
            Debug.Log("SaveGameManager:LoadMainData() – File text is:\n" + dataAsJson);
#endif

            try
            {
                saveFileList = JsonUtility.FromJson<SaveFileList>(dataAsJson);
            }
            catch
            {
                Debug.LogWarning("SaveGameManager:LoadMainData() - Save was malformed.\n" + dataAsJson);
                return;
            }
#if DEBUG_VerboseConsoleLogging
            Debug.Log("SaveGameManager:LoadMainData() – Successfully loaded save file.");
#endif
            LOCK = true;
            // Load list of savefiles etc
            //saveFileList = 

            LOCK = false;
        }
        else
        {
#if DEBUG_VerboseConsoleLogging
            Debug.LogWarning("SaveGameManager:LoadMainData() – Unable to find save file. "
                + "This is NOT totally fine. Make it now!");
#endif
            LOCK = true;
            // Init achievements and everything if needed
            // Init list of savefiles etc
            //saveFileList = 
            // sth just like this AchievementManager.ClearStepsAndAchievements();

            LOCK = false;
        }
    }

    public static void Save()
    {
        // Do not save when is LOCKed
        if (LOCK) return;

        // import data to saveFile
        saveFile.stepRecords = AchievementManager.GetStepRecords();
        saveFile.achievements = AchievementManager.GetAchievements();



        string jsonSaveFile = JsonUtility.ToJson(saveFile, true);

        File.WriteAllText(filePath, jsonSaveFile);
#if DEBUG_VerboseConsoleLogging
        Debug.Log("SaveGameManager:Save() – Path: " + filePath);
        Debug.Log("SaveGameManager:Save() – JSON: " + jsonSaveFile);
#endif
    }

    public static void Load()
    {
        if(File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
#if DEBUG_VerboseConsoleLogging
            Debug.Log("SaveGameManager:Load() – File text is:\n" + dataAsJson);
#endif
            
            try
            {
                saveFile = JsonUtility.FromJson<SaveFile>(dataAsJson);
            }
            catch
            {
                Debug.LogWarning("SaveGameManager:Load() - Save was malformed.\n" + dataAsJson);
                return;
            }
#if DEBUG_VerboseConsoleLogging
            Debug.Log("SaveGameManager:Load() – Successfully loaded save file.");
#endif
            LOCK = true;
            // Load the Achievements, cash, money, level, parts, ship etc etc
            AchievementManager.LoadDataFromSaveFile(saveFile);
            LOCK = false;
        }
        else
        {
#if DEBUG_VerboseConsoleLogging
            Debug.LogWarning("SaveGameManager:Load() – Unable to find save file. "
                + "This is totally fine if you've never gotten a game over "
                + "or completed an Achievement, which is when the game is saved.");
#endif
            LOCK = true;
            // Init achievements and everything if needed
            AchievementManager.ClearStepsAndAchievements();


            LOCK = false;
        }
    }
    static public void DeleteSave()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            saveFile = new SaveFile();
            Debug.Log("SaveGameManager:DeleteSave() – Successfully deleted save file.");
        }
        else
        {
            Debug.LogWarning("SaveGameManager:DeleteSave() – Unable to find and delete save file!"
                + " This is absolutely fine if you've never saved or have just deleted the file.");
        }
        // Lock the file to prevent any saving
        LOCK = true;

        // Select base stats etc.

        // Clear Achievements & Steps
        AchievementManager.ClearStepsAndAchievements();

        // Unlock the file
        LOCK = false;
    }
    internal static bool CheckHighScore(int score)
    {
        if(score > saveFile.highScore)
        {
            saveFile.highScore = score;
            return true;
        }
        return false;
    }
}

// A class must be serializable to be converted to and from JSON by JsonUtility.
[System.Serializable]
public class SaveFile
{
    public string playerName = "PlayerOne";
    public StepRecord[] stepRecords;
    public Achievement[] achievements;
    public int highScore = 10000;
}

[System.Serializable]
public class SaveFileList
{
    public List<SaveFile> saveFiles;
}