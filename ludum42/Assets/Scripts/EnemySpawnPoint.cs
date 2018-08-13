using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    bool isVisible;
    bool enemySpawned = false;
    bool isCooldown = false;
    float spawnCooldown = 3f;
    float nextSpawnTime;
    Renderer renderer;
    [SerializeField] GameObject enemy;
    public int aliveEnemyCount = 0;
    int maxAliveEnemyCount = 1;
    int totalSpawnedCount = 0;
    [SerializeField]
    int spawnPointDifficulty;


    private void OnBecameVisible()
    {
        isVisible = true;
        //Debug.Log("visible");
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
        //Debug.Log("ivisible");
    }

    void Start()
    {
        if (!isVisible)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (!isVisible && aliveEnemyCount < maxAliveEnemyCount)
        {
            //Debug.Log("SPAWNED");
            GameObject newEnemy = Instantiate(enemy, transform.position, transform.rotation);
            newEnemy.GetComponent<EnemyController>().maxHP += totalSpawnedCount + spawnPointDifficulty;
            newEnemy.GetComponent<EnemyController>().parentSpawnPoint = this;
            aliveEnemyCount++;
            totalSpawnedCount++;
        }
        //Debug.Log("COROTINE called");
        StartCoroutine(SpawnCooldown());
    }


    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        SpawnEnemy();
    }
}
