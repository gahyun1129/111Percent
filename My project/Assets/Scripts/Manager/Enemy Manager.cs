using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    [SerializeField] Transform SpawnPoint;
    private float spawnTime = 0.5f;
    private float spacing = 0.8f;

    private List<GameObject> enemies = new List<GameObject>();

    [SerializeField] EnemySpawn spawner;

    public void StartSpawnEnemy(int count)
    {
        spawner.SetSpawnData(spawnTime, spacing, SpawnPoint);
        spawner.StartSpawn(count);
    }
    public void AddEnemy(GameObject go)
    {
        enemies.Add(go);
    }
    public GameObject GetEnemy()
    {
        if ( enemies.Count == 0)
        {
            return null;
        }
        return enemies[0];
    }
    public void DeleteEnemy(GameObject go)
    {
        enemies.Remove(go);
        Destroy(go);

        if ( enemies.Count == 0)
        {
            // 게임 오버(win)
        }
        else
        {
            for ( int i = 0; i < enemies.Count; i++ )
            {
                enemies[i].GetComponent<Enemy>().SetTarget(SpawnPoint.position + Vector3.right * spacing * i);
            }
        }
    }
    
}
