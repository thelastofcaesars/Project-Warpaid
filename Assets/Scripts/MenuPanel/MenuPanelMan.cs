using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelMan : MonoBehaviour
{
    public static bool isAuthorsShown = false;
    public static bool goAuthorsShow = false;

    public static bool isHighscoresShown = false;
    public static bool goHighscoresShow = false;

    public static bool isLoadGameShown = false;
    public static bool goLoadGameShow = false;

    public static bool isMultiplayerShown = false;
    public static bool goMultiplayerShow = false;

    public static bool isSaveGameShown = false;
    public static bool goSaveGameShow = false;

    public void Authors()
    {
        MenuPanel.isMoreWindows = true;
        GameManagement.goMenuShow = true;
        goAuthorsShow = true;
    }

    public void Highscores()
    {
        MenuPanel.isMoreWindows = true;
        GameManagement.goMenuShow = true;
        goHighscoresShow = true;
    }

    public void LoadGame()
    {
        MenuPanel.isMoreWindows = true;
        GameManagement.goMenuShow = true;
        goLoadGameShow = true;
    }
    public void MultiplayerGame()
    {
        MenuPanel.isMoreWindows = true;
        GameManagement.goMenuShow = true;
        goMultiplayerShow = true;
    }
    public void NewGame()
    {
        GameManagement.goMenuShow = true;
        GameManagement.isInGame = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SaveGame()
    {
        MenuPanel.isMoreWindows = true;
        GameManagement.goMenuShow = true;
        goSaveGameShow = true;
    }
}
