using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }
    private void Awake()
    {
        if ( Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }  

        Instance = this;
    }

    private int stageNum = 0;
    private int enemyNum = 10;

    private void Start()
    {
        EnemyManager.instance.StartSpawnEnemy(enemyNum);
    }
}
