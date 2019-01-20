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
    public int playerLifes = 1;
    public int playerArmors = 0;

    public float bulletDelta = 1.1f;

    [Header("Boosts")]
    public float bulletTime = 0f;
    public float reflex = 0f;
    public float speedBoost = 0f;
    public float freezeTime = 0f;
    public float energy = 0f;

    [Header("PlayerPrefs")]
    public float minVel = 5f;
    public float maxVel = 10f;
    public float maxAngularVel = 10f;
    public float scale = 0.25f;
    public int   level = 1;
    

    public GameObject[] partPrefabs; // maybe need to look after changable parts, not all prefabs
    public GameObject[] particlePrefabs;
    public GameObject playerBullet;


    public GameObject GetPlayerPrefab()
    {
        int ndx = Random.Range(0, partPrefabs.Length);
        return partPrefabs[ndx];
    }

    public GameObject GetPlayerParticlePrefab()
    {
        int ndx = Random.Range(0, particlePrefabs.Length);
        return particlePrefabs[ndx];
    }
    public GameObject GetPlayerBullet()
    {
        return playerBullet;
    }

}
