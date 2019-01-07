using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Objects/Enemy", order = 2)]
public class EnemyScriptableObject : ScriptableObject
{
    public string objectName = "New Enemy";
    public Vector3 objectPosition;
    public int objectLevel;
    public int objectDrop;
    public float objectHealth;
    public float objectArmor;
    public float objectSpeed;
    public float objectReflex;
}
