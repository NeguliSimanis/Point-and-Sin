using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

    public enum EnemyType {Succubus, SkullBoss };
    public enum EnemyState { Idle, FollowPlayer, AttackPlayer, AttackEnemy, FollowEnemy, Dying}

    /*
    /// <summary>
    /// Far from player:
    ///     - regular enemy follows player if player visible 
    ///     - minion follows player always
    /// Near player:
    ///     - minion does not move (unless detects enemy)
    ///     - regular enemy stops moving and attacks if player visible
    /// </summary>
    public enum RelativePosition { FarFromPlayer, NearPlayer, NextToPlayer}
    */

    #region DATA variables
    [SerializeField] EnemyType type;
    public bool isFinalBoss = false;
  
    public float moveSpeed = 0.23f;
    public int enemyID = 0;
    [SerializeField]
    private int expDrop = 40; // how much exp is gained by killing this mofo
    public int maxHP = 12;
    public int currentHP = 12;

    [SerializeField] float attackCooldown;      // deals damage to player once per this interval
    public int damagePerAttack;  // how much damage is dealt in one attack
    #endregion

    #region ENEMY PROJECTILES
    [SerializeField] GameObject enemyProjectile;
    [SerializeField] Transform projectileExitPoint;
    private float lastProjectileShootTime;
    private bool isShootingCoroutineLoop = false;
    #endregion

    #region STATE variables
    EnemyState currentState = EnemyState.Idle;
    bool seenPlayerAtLeastOnce = false;

    bool isAttacking = false;
    bool isFacingRight = true;
    bool isWalking = false;
    bool isIdleA = false; // is in idle animation state A
    bool isDying = false; // can't move when dying
    #endregion
    
    #region SIGHT
    
    public float sightRadius = 3f;          // if player moves closer than this, he will be noticed    
    bool isPlayerVisible = false;           // if true, move towards player to attack. If false, patrol the area
    bool isNearPlayer = false;              // if true, stop to attack the player
    bool isPlayerInProjectileRange = false;
    public bool isNearTargetEnemy = false;  // used for minion targetting

    bool forgotPlayerPosition = false; // 
    float enemyMemory = 5f; // how long will the enemy chase the player if he's not visible
    float enemyForgetsPlayerTime; // when will enemy forget to chase the player if it's not visible
    //RelativePosition enemyPosition;
    #endregion

    #region MINION
    public bool isPlayerMinion = false;    // if true, ignore player and attack enemies
    float minionFollowRadius = 0.5f;       // if player exits this radius, follow him - usually smaller than sight radius
    [SerializeField] GameObject minionSightController; // should be allocated in editor to all units that can be turned to minions
    #endregion

    #region DEATH
    DamageSource fatalBlowSource; // what damage caused death
    [SerializeField]
    GameObject victoryItem;
    #endregion

    #region MOVEMENT and TARGETTING 
    private Vector2 targetPosition;
    private Vector2 dirNormalized;
    private Transform targetTransform;
    public Transform otherEnemyTransform;
    #endregion

    #region OTHER variables
    [SerializeField] Transform playerTransform;
    public EnemySpawnPoint parentSpawnPoint;
    private GameObject enemyCopy; // used to spawn a player minion when the enemy is destroyed
    #endregion

    #region ANIMATION
    [Header("Animations")]
    public Animator enemyAnimator;
    [SerializeField] AnimationClip deathAnimation;
    [SerializeField] AnimationClip attackAnimation;
    #endregion

    #region AUDIO
   
    AudioSource enemyAudioSource;
    AudioSource audioControl;
     [Header("Audio")]
    [SerializeField] AudioClip shootSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip woundedSFX;
    [SerializeField] AudioClip[] noticePlayerSFX;
    int noticePlayerSFXCount;
    int lastPlayedNoticePlayerSFX_ID = -1;
    [SerializeField] float shootSFXVolume;
    [SerializeField] float deathSFXVolume = 0.9f;
    [SerializeField] float woundedSFXVolume;
    [SerializeField] float noticePlayerSFXVolume;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] Image bossLifeBar;
    [SerializeField] GameObject bossLifeBarObject;
    #endregion

    public string GetEnemyName()
    {
        if (type == EnemyType.SkullBoss)
        {
            return "LESSER SKULL CLERIC";
        }
        else
        {
            return "SUCCUBUS";
        }
    }

    private void Start()
    {
        if (EnemyData.current == null)
            EnemyData.current = new EnemyData();
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (PlayerData.current == null)
            PlayerData.current = new PlayerData();

        enemyID = EnemyData.current.GetEnemyID();
        enemyAudioSource = gameObject.GetComponent<AudioSource>();
        currentHP = maxHP;
        noticePlayerSFXCount = noticePlayerSFX.Length;

        //  prepare a copy of this object in case it is neeeded to spawn a minion
        enemyCopy = this.gameObject;
        
        // change stats for minion
        if (isPlayerMinion)
        {
            moveSpeed *= 1.5f;
        }
    }

    void Update ()
    {
        if (PlayerData.current.isGamePaused)
        {
            return;
        }
        if (!isDying)
        {
            if (!isPlayerVisible || forgotPlayerPosition)
            {
                CheckIfTargetVisible();
            }
            else
            {
                ForgetTargetPosition();
            }

            #region Movement behaviour
            CheckWhereEnemyIsFacing();
            // regular enemy follows player if it is visible
            if (isPlayerVisible && !isPlayerMinion)
            {
                FollowPlayer();
            }           
            else if (isPlayerMinion)
            {
                if (isNearTargetEnemy)
                    FollowOtherEnemy();
                else if (!isPlayerVisible)
                    FollowPlayer();
            }
            else
            {
                enemyAnimator.SetBool("isWalking", false);
                PatrolArea();
            }   
            #endregion
        }

        if (isPlayerMinion)
        {
            return;
        }
        // show boss hp bar
        if (type == EnemyType.SkullBoss && seenPlayerAtLeastOnce)
        {
            UpdateBossHPBar();
        }
    }


    private void ForgetTargetPosition()
    {
        if (Time.time > enemyForgetsPlayerTime)
        {
            forgotPlayerPosition = true;
        }
    }

    void CheckWhereEnemyIsFacing()
    {
        // check if isFacing right variable has correct value
        if (isFacingRight && transform.localScale.x < 0)
        {
           // Debug.Log("mistake - isn't facing right");
            isFacingRight = false;
        }
        else if (!isFacingRight && transform.localScale.x > 0)
        {
           // Debug.Log("mistake - is facing right");
            isFacingRight = true;
        }

        // modify local scale to fit the direction where enemy is facing
        if (isFacingRight && dirNormalized.x < 0)
        {
            isFacingRight = false;
            gameObject.transform.localScale = new Vector2(-1f, 1f);
        }
        else if (!isFacingRight && dirNormalized.x > 0)
        {
            isFacingRight = true;
            gameObject.transform.localScale = new Vector2(1f, 1f);
        }
    }

    #region ATTACKING LOGIC
    public void StandbyToMeleeAttackPlayer()
    {
        // succubus, skullboss is ranged
        if ((type == EnemyType.Succubus || type == EnemyType.SkullBoss) == true)
        {
            return;
        }
        isNearPlayer = true;
        // minion doesn't attack player
        if (isPlayerMinion)
        {
            return;
        }
        if (!isAttacking)
        {
            isAttacking = true;
            enemyAnimator.SetBool("isAttacking",true);
            StartCoroutine(AttackTarget());
        }
    }

    public void StopStandbyToMeleeAttackPlayer()
    {
        // succubus, skullboss is ranged
        if ((type == EnemyType.Succubus || type == EnemyType.SkullBoss) == true)
        {
            return;
        }
        isNearPlayer = false;
        // minion doesn't attack player
        if (isPlayerMinion)
        {
            return;
        }
        isAttacking = false;
        enemyAnimator.SetBool("isAttacking", false);
    }

    public void StandbyToShootTarget()
    {
        if ((type == EnemyType.Succubus || type == EnemyType.SkullBoss) == false)
        {
            return;
        }
        isPlayerInProjectileRange = true;

        if (!isAttacking && !isShootingCoroutineLoop)
        {
            if (isPlayerMinion)
                currentState = EnemyState.AttackEnemy;
            else
                currentState = EnemyState.AttackPlayer;
        
            isAttacking = true;
            enemyAnimator.SetBool("isAttacking", true);
            StartCoroutine(ShootTarget());
        }
    }

    public void StopStandbyToShootTarget()
    {
        if ((type == EnemyType.Succubus || type == EnemyType.SkullBoss) == false)
        {
            return;
        }
        isPlayerInProjectileRange = false;
        
        currentState = EnemyState.Idle;
        isAttacking = false;
        enemyAnimator.SetBool("isAttacking", false);
    }

    private IEnumerator AttackTarget()
    {
        while (isAttacking)
        {
            yield return new WaitForSeconds(attackCooldown);
            PlayerData.current.DamagePlayer(damagePerAttack);
        }
    }

    private IEnumerator ShootTarget()
    {
        yield return new WaitForSeconds(attackAnimation.length);
        if (isAttacking && !isDying)
        {
            lastProjectileShootTime = Time.time;
            ShootProjectile();
            StartCoroutine(ShootCooldown());
            enemyAnimator.SetBool("isAttacking", false);
        }
        else
        {
            isShootingCoroutineLoop = false;
        }
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        if (isAttacking)
        {
            isShootingCoroutineLoop = true;
            enemyAnimator.SetBool("isAttacking", true);
            StartCoroutine(ShootTarget());
        }
        else
        {
            isShootingCoroutineLoop = false;
        }
    }

    void ShootProjectile()
    {
        enemyAudioSource.PlayOneShot(shootSFX, shootSFXVolume);
        GameObject projectile = Instantiate(enemyProjectile, projectileExitPoint.position, projectileExitPoint.rotation, projectileExitPoint);
        EnemyProjectile projectileController = projectile.GetComponent<EnemyProjectile>();
        if (isPlayerMinion)
            projectileController.isMinionProjectile = true;
        projectileController.StartProjectile(isFacingRight, damagePerAttack);
        projectile.transform.parent = null;
    }
    #endregion
    void CheckIfTargetVisible()
    {
        float distanceToCheck = sightRadius;
        if (isPlayerMinion)
        {
            distanceToCheck = minionFollowRadius;
        }
        if (Vector2.Distance(playerTransform.position, transform.position) <= distanceToCheck)
        {
            NoticePlayer();
        }
        else
        {
            UnnoticePlayer();
        }
    }

    void UnnoticePlayer()
    {
        isPlayerVisible = false;
        isIdleA = !isPlayerMinion;
        //isWalking = isPlayerMinion;
        enemyAnimator.SetBool("isWalking", isPlayerMinion);
    }

    void NoticePlayer()
    {
        //Debug.Log("NOTICING " + Time.time);
        // enemy will chase player until this time and then stop if he's no longer visible
        enemyForgetsPlayerTime = Time.time + enemyMemory;
        forgotPlayerPosition = false;

        if (isPlayerVisible == false)
        {
            //Debug.Log("NOTICING 2 " + Time.time);
            isPlayerVisible = true;
            if (type == EnemyType.SkullBoss && !isPlayerMinion)
            {
                PlaySFXBossNoticesPlayer();
                if (seenPlayerAtLeastOnce == false)
                {
                    seenPlayerAtLeastOnce = true;
                    ShowBossHPBar();
                }
            }
        }
        //isWalking = !isPlayerMinion;
        enemyAnimator.SetBool("isWalking", !isPlayerMinion);
        //enemyAnimator.SetBool("isWalking", isPlayerMinion);
        isIdleA = isPlayerMinion;
    }

    /// <summary>
    /// Chooses one of the bosss voice lines and plays it when player is noticed
    /// </summary>
    void PlaySFXBossNoticesPlayer()
    {
        int currentSFX_ID = lastPlayedNoticePlayerSFX_ID;
        // play the random line the first time the method is called
        if (lastPlayedNoticePlayerSFX_ID == -1)
        {
            Debug.Log("case 1");
            currentSFX_ID = Random.Range(0, (noticePlayerSFXCount-1));
        }
        // play the next line each time method is called
        else if (lastPlayedNoticePlayerSFX_ID < noticePlayerSFXCount - 1)
        {
            Debug.Log("case 2");
            currentSFX_ID = lastPlayedNoticePlayerSFX_ID + 1;
        }
        // start from the beginning of voice line array if last line reached
        else if (lastPlayedNoticePlayerSFX_ID >= noticePlayerSFXCount - 1)
        {
            Debug.Log("case 3");
            currentSFX_ID = 0;
        }
        enemyAudioSource.PlayOneShot(noticePlayerSFX[currentSFX_ID], noticePlayerSFXVolume);
        lastPlayedNoticePlayerSFX_ID = currentSFX_ID;
        Debug.Log("last played sfx - " + lastPlayedNoticePlayerSFX_ID + ". current sfx -  " + currentSFX_ID);
    }

    void ShowBossHPBar()
    {
        if (!PlayerData.current.isPlayingBrutalMode)
            bossLifeBarObject.SetActive(true); 
    }

    void UpdateBossHPBar()
    {
        if (!PlayerData.current.isPlayingBrutalMode)
            bossLifeBar.fillAmount = (float)currentHP / (float)maxHP;
    }

    void PatrolArea()
    {
        //Soon
    }

    void FollowPlayer()
    {
        //Debug.Log("FOLLOWING " + Time.time);
        if (!isPlayerMinion && (isNearPlayer || isPlayerInProjectileRange))
        {
            enemyAnimator.SetBool("isWalking", false);
            return;
        }
        targetPosition = playerTransform.position;
        GetTargetPositionAndDirection();
        MoveEnemy();
    }

    // only player minions do this
    void FollowOtherEnemy()
    {
        // prioritize following player if he is far
        if (!isPlayerVisible)
        {
            FollowPlayer();
            return; 
        }
        // stop and shoot if near enemey
        if (isNearTargetEnemy)
        {
            return;
        }
        targetPosition = otherEnemyTransform.position;
        GetTargetPositionAndDirection();
        MoveEnemy();
    }

    void GetTargetPositionAndDirection()
    {
        dirNormalized = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y).normalized;
    }

    void MoveEnemy()
    {
        //CheckWhereEnemyIsFacing();
        enemyAnimator.SetBool("isWalking", true);
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(int damageAmount, DamageSource damageSource)
    {
        fatalBlowSource = damageSource;
        if (damageSource == DamageSource.PlayerFireball || damageSource == DamageSource.PlayerMelee)
        {
            //Debug.Log("NOTICE");
            NoticePlayer();
        }
        enemyAnimator.SetTrigger("damaged");
        enemyAudioSource.PlayOneShot(woundedSFX, woundedSFXVolume);
        currentHP = currentHP - damageAmount;
        if (currentHP <= 0)
            Die();
    }

    public void Die()
    {
        if (!isDying)
        {
            isDying = true;

            // roll chance to drop item and drop item
            if (type != EnemyType.SkullBoss)
                gameObject.GetComponent<EnemyItemDropper>().DropCommonItem();

            // update player stats
            if (!isPlayerMinion && fatalBlowSource != DamageSource.Undefined)
            {
                PlayerData.current.AddExp(expDrop);
                PlayerData.current.enemiesKilled++;
            }
            else
            {
                PlayerData.current.currentMinions--;
            }

            // check player victory condition OR defeating skull in brutal mode
            if (type == EnemyType.SkullBoss)
            {
                HandleSkullDefeat();
            }

            // play death audio
            enemyAudioSource.PlayOneShot(deathSFX, deathSFXVolume);

            // play death animation
            enemyAnimator.SetBool("isDead", true);

            // register death in parent spawn point
            if (parentSpawnPoint != null)
            {
                parentSpawnPoint.aliveEnemyCount--;
            }

            // start destroying game object
            StartCoroutine(DestroyAfterXSeconds(deathAnimation.length));
        }
    }

    private IEnumerator DestroyAfterXSeconds(float xSeconds)
    {
        yield return new WaitForSeconds(xSeconds);

        // player has a skill that summons a minion upon killing enemy
        if (PlayerData.current.summonMinionOnMeleeKill && !isPlayerMinion && fatalBlowSource == DamageSource.PlayerMelee)
        {
            RespawnAsPlayerMinion();
        }
       // Debug.Log(PlayerData.current.enemiesKilled + " enemies killed");
        Destroy(gameObject);
    }

    private void RespawnAsPlayerMinion()
    {
        // player has more minions than allowed, destroy the old minion before replacing it with a fresh one
        if (PlayerData.current.currentMinions >= PlayerData.current.maxMinions)
        {
            Debug.Log("current minions: " + PlayerData.current.currentMinions + "| MAX minions: " + PlayerData.current.maxMinions);
            PlayerData.current.minions[0].fatalBlowSource = DamageSource.Undefined;
            PlayerData.current.minions[0].Die();
        }

        // instantiate minion
        GameObject copy = Instantiate(enemyCopy, transform.position, transform.rotation);
        EnemyController minionController = copy.GetComponent<EnemyController>();
        minionController.isPlayerMinion = true;
        copy.tag = "PlayerMinion";

        // update player data
        PlayerData.current.currentMinions++;
        PlayerData.current.minions.Add(minionController);

        // add enemy detection to minion
        Instantiate(minionSightController, transform.position, transform.rotation, copy.transform);

        minionController.enemyAnimator.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4117647f, 1, 0.5094506f);
    }

    private void OnDestroy()
    {
        if(isPlayerMinion)
        {
            PlayerData.current.minions.Remove(gameObject.GetComponent<EnemyController>());
        }
    }

    public EnemyController.EnemyType GetEnemyType()
    {
        return type;
    }

    private void HandleSkullDefeat()
    {
        if (isFinalBoss)
        {
            //bossLifeBar.transform.parent.gameObject.SetActive(false);
            bossLifeBarObject.SetActive(false);
            victoryItem.gameObject.SetActive(true);
        }
        if (!PlayerData.current.isPlayingBrutalMode)
        {
            PlayerData.current.sinTreePoints++;
            //gameObject.GetComponent<EnemyItemDropper>().DropVictoryItem();
            //playerTransform.gameObject.GetComponent<PlayerController>().WinGame();
        }
        else
        {   
            gameObject.GetComponent<EnemyItemDropper>().RollChanceToDropUniqueItem();
        }
    }

}
