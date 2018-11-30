using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGame : MonoBehaviour
{
    
    void Update()
    {
        GameManagement.goMenuShow = true;
        GameManagement.isInGame = true;
    }
}
