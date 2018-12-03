using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
   
    // spawn cooldown
    bool enemySpawned = false;
    bool isCooldown = false;
    float spawnCooldown = 3f;
    float nextSpawnTime;

    // visibility
    bool isVisible;
    Renderer renderer;

    // enemy types
    [SerializeField]
    GameObject enemy;
    [SerializeField]
    GameObject brutalEnemy;

    // enemy count
    public int aliveEnemyCount = 0;
    int maxAliveEnemyCount = 1;
    int totalSpawnedCount = 0;

    // dificulty
    [SerializeField]
    int spawnPointDifficulty;

    float brutalEnemyChance = 0.4f;
    int brutalEnemyHP = 20;

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
        Debug.Log("I am awake " + Time.time);
        //StartCoroutine(BeginSpawnLoopAfterXSeconds());
        if (!isVisible)
        {
            SpawnEnemy();
        }   
    }

    /*private IEnumerator BeginSpawnLoopAfterXSeconds()
    {
        yield return new WaitForSeconds(spawnCooldown);
        Debug.Log("first corouting is working " + Time.time);
        BeginSpawnLoop();
    }

    public void BeginSpawnLoop()
    {
        if (!isVisible)
        {
            SpawnEnemy();
        }
    }*/

    void SpawnEnemy()
    {
        if (!isVisible)   
        {
           // Debug.Log("Spawn point invisible " + Time.time);
            if (aliveEnemyCount < maxAliveEnemyCount)
            {
                GameObject newEnemy = enemy;
                //GameObject newEnemy = enemy;
                // spawn miniboss in brutal mode
                if (PlayerData.current.isPlayingBrutalMode)
                {
                   // Debug.Log("IS PLAYING BRYTAL");
                    if (Random.Range(0f, 1f) < brutalEnemyChance)
                    {
                       // Debug.Log("Should spawn BRUTAL");
                        newEnemy = brutalEnemy;

                        EnemyController newEnemyController = newEnemy.GetComponent<EnemyController>();
                        newEnemyController.maxHP = brutalEnemyHP;
                        newEnemyController.currentHP = brutalEnemyHP;
                    }
                }

                newEnemy = Instantiate(newEnemy, transform.position, transform.rotation);
                EnemyController newEnemyControllerB = newEnemy.GetComponent<EnemyController>();
                newEnemyControllerB.maxHP += totalSpawnedCount + spawnPointDifficulty;
                newEnemyControllerB.currentHP += totalSpawnedCount + spawnPointDifficulty;
                newEnemyControllerB.parentSpawnPoint = this;

                Debug.Log("spawning enemy " + Time.time);

                // update stats
                aliveEnemyCount++;
                totalSpawnedCount++;
            }
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
