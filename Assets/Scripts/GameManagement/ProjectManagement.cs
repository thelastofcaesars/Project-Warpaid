using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectManagement : MonoBehaviour
{
    public GameObject menuPanelObject;
    public static bool isMenuShown = true;
    public GameObject updatePanel;
    public Text updateText;
    public string updatingText = "WORK-IN-PROGRESS";

    void Start()
    {
        updatePanel.SetActive(true);
        updateText.text = updatingText;
        menuPanelObject.SetActive(true);
    }
}
