using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class UsePlayerSO : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab ;

    [SerializeField]
    private PlayerScriptableObject player1;

    private void Start()
    {
        MakePlayer();
    }
    void MakePlayer()
    {
        var playerInstance1 = Instantiate(playerPrefab);
        playerInstance1.transform.position = player1.objectPosition;
    }
}
