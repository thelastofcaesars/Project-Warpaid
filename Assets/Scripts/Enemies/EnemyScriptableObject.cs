using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemySO", fileName = "EnemySO.asset", order = 1)]
[System.Serializable]
public class EnemyScriptableObject : ScriptableObject
{
    static public EnemyScriptableObject S; // This Scriptable Object is an unprotected Singleton

    public EnemyScriptableObject()
    {
        S = this; // Assign the Singleton as part of the constructor.
    }

    public string enemyName = "New Enemy";
    public float enemyHealth = 100f;
    public float enemyArmor = 0f;
    public float enemyReflex = 10f;
    
    public float minVel = 5f;
    public float maxVel = 10f;
    public float maxAngularVel = 10f;
    public int enemyLevel = 2; 
    public float enemyScale = 0.25f;

    public string dropKind = "none";
    public int[] pointsForEnemyLevel = { 0, 400, 200, 100 };

    public GameObject[] enemyPrefabs;
    public GameObject[] enemyParticlePrefabs;
    public GameObject[] enemyDropPrefabs;
    public GameObject enemyBullet;

    public GameObject GetEnemyPrefab()
    {
        int ndx = Random.Range(0, enemyPrefabs.Length);
        return enemyPrefabs[ndx];
    }

    public GameObject GetEnemyParticlePrefab()
    {
        int ndx = Random.Range(0, enemyParticlePrefabs.Length);
        return enemyParticlePrefabs[ndx];
    }

    /*public GameObject GetEnemyDropPrefab()
    {
        int ndx = Random.Range(0, enemyDropPrefabs.Length);
        return enemyDropPrefabs[ndx];
    }*/
    public GameObject GetEnemyDropPrefab()
    {
        return Warpaid.DropSO.GetDropPrefab(dropKind);
    }
    public GameObject GetEnemyBullet()
    {
        return enemyBullet;
    }
}
