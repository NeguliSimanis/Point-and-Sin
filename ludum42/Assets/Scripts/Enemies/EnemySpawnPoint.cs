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

    float enemyBrutalMoveSpeedBuffPerKill = 0.1f; // all enemies in brutal mode move faster by this amount
    int enemyBrutalDamageBuffPerKill = 1;
    float enemyBrutalSightBuff = 1.2f;

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
        //Debug.Log("I am awake " + Time.time);
        StartCoroutine(BeginSpawnLoopAfterXSeconds());
       /* if (!isVisible)
        {
            SpawnEnemy();
        } */  
    }

    private IEnumerator BeginSpawnLoopAfterXSeconds()
    {
        yield return new WaitForSeconds(spawnCooldown);
        //Debug.Log("first corouting is working " + Time.time);
        BeginSpawnLoop();
    }

    public void BeginSpawnLoop()
    {
        if (!isVisible)
        {
            SpawnEnemy();
        }
    }

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
                        newEnemyController.isFinalBoss = false;
                    }
                }

                newEnemy = Instantiate(newEnemy, transform.position, transform.rotation);
                BuffEnemy(newEnemy.GetComponent<EnemyController>());


                //Debug.Log("spawning enemy " + Time.time);

                // update stats
                aliveEnemyCount++;
                totalSpawnedCount++;
            }
        }
        //Debug.Log("COROTINE called");
        StartCoroutine(SpawnCooldown());
    }

    /// <summary>
    /// Increases enemy stats depending on the game mode and spawn difficulty
    /// </summary>
    private void BuffEnemy(EnemyController enemyToBuff)
    {
        // buff hp
        enemyToBuff.maxHP += totalSpawnedCount + spawnPointDifficulty;
        enemyToBuff.currentHP += totalSpawnedCount + spawnPointDifficulty;

        //buff brutal stuff
        if (PlayerData.current.isPlayingBrutalMode)
        {
            enemyToBuff.moveSpeed += totalSpawnedCount * enemyBrutalMoveSpeedBuffPerKill;
            enemyToBuff.damagePerAttack += totalSpawnedCount * enemyBrutalDamageBuffPerKill;
            enemyToBuff.sightRadius += enemyBrutalSightBuff;
        }

        enemyToBuff.parentSpawnPoint = this;
    }

    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        
        SpawnEnemy();
    }
}
