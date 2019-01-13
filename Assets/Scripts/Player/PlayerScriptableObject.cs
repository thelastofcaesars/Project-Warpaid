using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerSO", fileName = "PlayerSO.asset", order = 2)]
[System.Serializable]
public class PlayerScriptableObject : ScriptableObject
{
    static public PlayerScriptableObject S;// This Scriptable Object is an unprotected Singleton

    public PlayerScriptableObject()
    {
        S = this; // Assign the Singleton as part of the constructor.
    }
    public string playerName = "New Player";
    public int playerLifes = 2; 
    public float playerHealth = 100f;
    public float playerArmor = 0f;
    public float playerReflex = 10f;

    public float minVel = 5f;
    public float maxVel = 10f;
    public float maxAngularVel = 10f;
    public int playerLevel = 1;
    public float playerScale = 0.25f;

    public GameObject[] playerPrefabs; // maybe need to look after changable parts, not all prefabs
    public GameObject[] playerParticlePrefabs;
    public GameObject playerBullet;


    public GameObject GetPlayerPrefab()
    {
        int ndx = Random.Range(0, playerPrefabs.Length);
        return playerPrefabs[ndx];
    }

    public GameObject GetPlayerParticlePrefab()
    {
        int ndx = Random.Range(0, playerParticlePrefabs.Length);
        return playerParticlePrefabs[ndx];
    }
    public GameObject GetPlayerBullet()
    {
        return playerBullet;
    }

}
