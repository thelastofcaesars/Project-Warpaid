using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemySO", fileName = "EnemySO.asset")]
[System.Serializable]
public class EnemyScriptableObject : ScriptableObject
{
    static public EnemyScriptableObject S; // This Scriptable Object is an unprotected Singleton

    public EnemyScriptableObject()
    {
        S = this; // Assign the Singleton as part of the constructor.
    }

    public string objectName = "New Enemy";
    public float enemyHealth;
    public float enemyArmor;
    public float enemyReflex;

    public float minVel = 5;
    public float maxVel = 10;
    public float maxAngularVel = 10;
    public int enemyLevel = 2; 
    public float enemyScale = 0.25f;

    public int[] pointsForEnemyLevel = { 0, 400, 200, 100 };

    public GameObject[] enemyPrefabs;
    public GameObject[] enemyParticlePrefabs;
    public GameObject[] enemyDropPrefabs;

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

    public GameObject GetEnemyDropPrefab()
    {
        int ndx = Random.Range(0, enemyDropPrefabs.Length);
        return enemyDropPrefabs[ndx];
    }
}
