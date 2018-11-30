using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseEnemySO : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private EnemyScriptableObject myEnemy1;
    [SerializeField]
    private EnemyScriptableObject myEnemy2;

    void Start()
    {
        var enemyInstance1 = Instantiate(enemyPrefab);
        enemyInstance1.transform.position = myEnemy1.objectPosition;

        var enemyInstance2 = Instantiate(enemyPrefab);
        enemyInstance2.transform.position = myEnemy2.objectPosition;
    }

}
