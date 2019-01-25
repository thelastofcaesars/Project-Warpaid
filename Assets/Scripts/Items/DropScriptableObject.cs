﻿using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DropSO", fileName = "DropSO.asset", order = 3)]
[System.Serializable]
public class DropScriptableObject : ScriptableObject
{
    static public DropScriptableObject S; // This Scriptable Object is an unprotected Singleton

    public DropScriptableObject()
    {
        S = this; // Assign the Singleton as part of the constructor.
    }

    public string packageName = "New Drop Package";

    public GameObject[] dropPrefabs;
    public GameObject[] allDropPrefabs;
    public GameObject[] customDropPrefabs;
    public GameObject[] motherDropPrefabs;
    public GameObject[] bossDropPrefabs;

    public GameObject GetDropPrefab(string kindOfDrop)
    {
        int ndx;
        switch (kindOfDrop)
        {
            case "drop":
                ndx = Random.Range(0, dropPrefabs.Length);
                return dropPrefabs[ndx];
            case "all":
                ndx = Random.Range(0, allDropPrefabs.Length);
                return allDropPrefabs[ndx];
            case "custom":
                ndx = Random.Range(0, customDropPrefabs.Length);
                return customDropPrefabs[ndx];
            case "mother":
                ndx = Random.Range(0, motherDropPrefabs.Length);
                return motherDropPrefabs[ndx];
            case "boss":
                ndx = Random.Range(0, bossDropPrefabs.Length);
                return bossDropPrefabs[ndx];
            default:
                Debug.Log("DropScriptableObject:GetDropPrefab - invalid index -> out of arrays");
                break;
        }
        Debug.Log("Returned first found element");
        return dropPrefabs[0];
    }
}
