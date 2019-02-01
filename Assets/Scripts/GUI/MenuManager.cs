using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Tooltip("Index for new and restarted game scene")]
    public int firstSceneIndex = 1;

    public void Authors()
    {
        ;
    }

    public void Highscores()
    {
        ;
    }

    public void LoadGame()
    {
        SaveGameManager.Load();
    }
    public void MultiplayerGame()
    {
        ;
    }
    public void NewGame()
    {
        SaveGameManager.DeleteSave();
        // start new game();
        UnityEngine.SceneManagement.SceneManager.LoadScene(firstSceneIndex);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SaveGame()
    {

        // need to create reminder button here save/cancel ("hey, this is early-access now, so it has only one available slot for saving and loading")
        SaveGameManager.Save();
    }
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void DirectToReplace()
    {
        AchievementManager.AchievementStep(Achievement.eStepType.directorRoom, 1);
        PlayerShip.DirectToReplace(new PlayerInfo(3, 2, 1.1f, 1f, 1f, 1f, 1f, 1f, true));
    }
    
}
