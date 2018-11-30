using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class MenuPanel : MonoBehaviour
{
    public GameObject highscoresPanel;
    public GameObject authorsPanel;

    void Update()
    {
        if(Highscores.goHighscoresShow)
        {
            Highscores.goHighscoresShow = false;
            ShowHighscores();
        }
        else if (Highscores.isHighscoresShown && CrossPlatformInputManager.GetButtonDown("Cancel"))
        {
            ShowHighscores();
        }
        if (Authors.goAuthorsShow)
        {
            Authors.goAuthorsShow = false;
            ShowAuthors();
        }
        else if (Authors.isAuthorsShown && CrossPlatformInputManager.GetButtonDown("Cancel"))
        {
            ShowAuthors();
        }
    }

    public void ShowHighscores()
    {
        if (Highscores.isHighscoresShown == false)
        {
            highscoresPanel.SetActive(true);
            Highscores.isHighscoresShown = true;
            GameManagement.goMenuShow = true;
        }
        else
        {
            highscoresPanel.SetActive(false);
            Highscores.isHighscoresShown = false;
            GameManagement.goMenuShow = true;
        }
    }
    public void ShowAuthors()
    {
        if (Authors.isAuthorsShown == false)
        {
            authorsPanel.SetActive(true);
            Authors.isAuthorsShown = true;
            GameManagement.goMenuShow = true;
        }
        else
        {
            authorsPanel.SetActive(false);
            Authors.isAuthorsShown = false;
            GameManagement.goMenuShow = true;
        }
    }
}
