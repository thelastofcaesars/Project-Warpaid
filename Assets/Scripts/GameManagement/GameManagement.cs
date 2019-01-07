using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
public class GameManagement : MonoBehaviour
{
    public GameObject menuPanelObject;
    public static bool isMenuShown = true;
    public static bool goMenuShow = false;
    public static bool isInGame = false;
    public static string sceneName = "Scene_01";
    void Update()
    {

        if (goMenuShow)
        {
            goMenuShow = false;
            ShowMenuPanel();
        }
        if (isInGame && CrossPlatformInputManager.GetButtonDown("Cancel"))
        {
            ShowMenuPanel();
        }
        if (isInGame)
        {
            LoadLevel(sceneName);
        }
    }

    public void ShowMenuPanel()
    {
        if (isMenuShown == false)
        {
            menuPanelObject.SetActive(true);
            isMenuShown = true;
        }
        else
        {
            menuPanelObject.SetActive(false);
            isMenuShown = false;
        }
    }
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
