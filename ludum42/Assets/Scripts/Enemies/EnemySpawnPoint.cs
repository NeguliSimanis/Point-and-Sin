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
    GameObject[] enemies;
    [SerializeField]
    GameObject brutalEnemy;

    // enemy count
    public int aliveEnemyCount = 0;
    int maxAliveEnemyCount = 1;
    int totalSpawnedCount = 0;

    #region dificulty
    [SerializeField]
    int spawnPointDifficulty;

    float demonSpawnChance = 0.25f;
    float brutalDemonChanceIncrease = 0.2f;
    float brutalEnemyChance = 0.4f;
    int brutalEnemyHP = 20;

    float enemyBrutalMoveSpeedBuffPerKill = 0.1f; // all enemies in brutal mode move faster by this amount
    int enemyBrutalDamageBuffPerKill = 1;
    float enemyBrutalSightBuff = 1.2f;
    #endregion

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

    private GameObject RollChanceForSpawningDemonEnemy()
    {
        GameObject newEnemy = enemies[0];
        float currentDemonSpawnChance = demonSpawnChance;
        if (PlayerData.current.isPlayingBrutalMode)
        {
            currentDemonSpawnChance += brutalDemonChanceIncrease;
        }
        if (Random.Range(0f, 1f) < currentDemonSpawnChance)
        {
            newEnemy = enemies[1];
        }
        return newEnemy;
    }

    void SpawnEnemy()
    {
        if (!isVisible)   
        {
           // Debug.Log("Spawn point invisible " + Time.time);
            if (aliveEnemyCount < maxAliveEnemyCount)
            {
                GameObject newEnemy = RollChanceForSpawningDemonEnemy();

                if (PlayerData.current.isPlayingBrutalMode)
                {

                    // spawn miniboss in brutal mode
                    if (Random.Range(0f, 1f) < brutalEnemyChance)
                    {
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
