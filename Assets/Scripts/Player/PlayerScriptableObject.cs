using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Objects/Player", order = 1)]
public class PlayerScriptableObject : ScriptableObject
{
    public string objectName = "New Player";
    public Vector3 objectPosition;
    public int objectLifes;
    public int objectModel;
    public int objectLevel;
    public int objectDrop;
    public float objectHealth;
    public float objectArmor;
    public float objectSpeed;
    public float objectReflex;
}
