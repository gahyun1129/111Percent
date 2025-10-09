using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance {  get; private set; }
    private void Awake()
    {
        if ( instance != null && instance != this )
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    
    private List<GameObject> enemies = new List<GameObject>();

    [SerializeField] EnemySpawn spawner;
    public void StartSpawnEnemy(int count) => spawner.StartSpawn(count);
    public void AddEnemy(GameObject go) => enemies.Add(go);
}
