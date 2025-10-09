using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform SpawnPoint;
    private float spawnTime = 0.5f;
    private float spacing = 0.8f;
    public void StartSpawn(int count) => StartCoroutine(SpawnEnemy(count));
    IEnumerator SpawnEnemy(int count)
    {
        int i = 0;

        while (i < count)
        {
            GameObject go = Instantiate(enemyPrefab, SpawnPoint.position + Vector3.right * spacing * i, SpawnPoint.rotation);
            EnemyManager.instance.AddEnemy(go);

            i++;
            yield return new WaitForSeconds(spawnTime);
        }

        yield return null;
    }
}
