using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    public static GameObject menuPanelObject;
    public static bool isMenuShown = true;
    void Update()
    {
        if(Input.GetKeyDown("Fire1"))
        {
            showMenuPanel();
        }
    }

    void showMenuPanel()
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
}
