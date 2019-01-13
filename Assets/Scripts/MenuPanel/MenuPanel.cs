using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class MenuPanel : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject authorsPanel;
    public GameObject highscorePanel;
    public GameObject loadgamePanel;
    public GameObject multiplayerPanel;
    public GameObject savegamePanel;

    public static bool isMoreWindows = false;

    /*
     * void Update()
    {
        //MenuPanel's management -> to optimize
        //Enums? Maybe switch
        if (isMoreWindows)
        {
            if (MenuPanelMan.goHighscoresShow)
            {
                MenuPanelMan.goHighscoresShow = false;
                ShowHighscores();
            }
            else if (MenuPanelMan.isHighscoresShown && CrossPlatformInputManager.GetButtonDown("Cancel"))
            {
                ShowHighscores();
            }

            else if (MenuPanelMan.goAuthorsShow)
            {
                MenuPanelMan.goAuthorsShow = false;
                ShowAuthors();
            }
            else if (MenuPanelMan.isAuthorsShown && CrossPlatformInputManager.GetButtonDown("Cancel"))
            {
                ShowAuthors();
            }

            else if (MenuPanelMan.goLoadGameShow)
            {
                MenuPanelMan.goLoadGameShow = false;
                ShowLoad();
            }
            else if (MenuPanelMan.isLoadGameShown && CrossPlatformInputManager.GetButtonDown("Cancel"))
            {
                ShowLoad();
            }

            else if (MenuPanelMan.goMultiplayerShow)
            {
                MenuPanelMan.goMultiplayerShow = false;
                ShowMultiplayer();
            }
            else if (MenuPanelMan.isMultiplayerShown && CrossPlatformInputManager.GetButtonDown("Cancel"))
            {
                ShowMultiplayer();
            }

            else if (MenuPanelMan.goSaveGameShow)
            {
                MenuPanelMan.goSaveGameShow = false;
                ShowSave();
            }
            else if (MenuPanelMan.isSaveGameShown && CrossPlatformInputManager.GetButtonDown("Cancel"))
            {
                ShowSave();
            }

        }
    }
    #region ShowPanel
    public void ShowAuthors()
    {
        if (MenuPanelMan.isAuthorsShown == false)
        {
            authorsPanel.SetActive(true);
            MenuPanelMan.isAuthorsShown = true;
        }
        else
        {
            authorsPanel.SetActive(false);
            MenuPanelMan.isAuthorsShown = false;
            Warpaid.goMenuShow = true;
            isMoreWindows = false;
        }
    }

    public void ShowHighscores()
    {
        if (MenuPanelMan.isHighscoresShown == false)
        {
            highscorePanel.SetActive(true);
            MenuPanelMan.isHighscoresShown = true;
        }
        else
        {
            highscorePanel.SetActive(false);
            MenuPanelMan.isHighscoresShown = false;
            Warpaid.goMenuShow = true;
            isMoreWindows = false;
        }
    }

    public void ShowLoad()
    {
        if (MenuPanelMan.isLoadGameShown == false)
        {
            loadgamePanel.SetActive(true);
            MenuPanelMan.isLoadGameShown = true;
        }
        else
        {
            loadgamePanel.SetActive(false);
            MenuPanelMan.isLoadGameShown = false;
            GameManagement.goMenuShow = true;
            isMoreWindows = false;
        }
    }

    public void ShowMultiplayer()
    {
        if (MenuPanelMan.isMultiplayerShown == false)
        {
            multiplayerPanel.SetActive(true);
            MenuPanelMan.isMultiplayerShown = true;
        }
        else
        {
            multiplayerPanel.SetActive(false);
            MenuPanelMan.isMultiplayerShown = false;
            Warpaid.goMenuShow = true;
            isMoreWindows = false;
        }
    }

    public void ShowSave()
    {
        if (MenuPanelMan.isSaveGameShown == false)
        {
            savegamePanel.SetActive(true);
            MenuPanelMan.isSaveGameShown = true;
        }
        else
        {
            savegamePanel.SetActive(false);
            MenuPanelMan.isSaveGameShown = false;
            Warpaid.goMenuShow = true;
            isMoreWindows = false;
        }
    }
    #endregion end of ShowPanel
    */
}
