using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public enum EnemyType {Succubus };

    #region DATA variables
    [SerializeField] EnemyType type;

    float moveSpeed = 0.13f;
    public int enemyID = 0;
    private int expDrop = 40; // how much exp is gained by killing this mofo
    private int maxHP = 12;
    private int currentHP = 12;

    [SerializeField] float sightRadius = 3f;    // if player moves closer than this, he will be noticed
    [SerializeField] float attackCooldown = 1f; // deals damage to player once per this interval
    [SerializeField] int damagePerAttack = 35;  // how much damage is dealt in one attack
    #endregion

    #region ENEMY PROJECTILES
    [SerializeField] GameObject enemyProjectile;
    [SerializeField] Transform projectileExitPoint;
    #endregion

    #region STATE variables
    bool isPlayerVisible = false; // if true, move towards player to attack. If false, patrol the area
    bool isNearPlayer = false; // if true, stop to attack the player
    bool isPlayerInProjectileRange = false;
    bool isAttacking = false;
    bool isFacingRight = true;
    bool isWalking = false;
    bool isIdleA = false; // is in idle animation state A
    bool isDying = false; // can't move when dying
    #endregion

    #region MOVEMENT variables
    private Vector2 targetPosition;
    private Vector2 dirNormalized;
    private Transform targetTransform;
    #endregion

    #region OTHER variables
    [SerializeField] Transform playerTransform;
    #endregion

    #region ANIMATION
    [SerializeField] Animator enemyAnimator;
    [SerializeField] AnimationClip deathAnimation;
    [SerializeField] AnimationClip attackAnimation;
    #endregion

    #region AUDIO
    [Header("Audio")]
    AudioSource audioControl;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] float deathSFXVolume = 0.9f;
    #endregion

    private void Start()
    {
        if (EnemyData.current == null)
            EnemyData.current = new EnemyData();
        enemyID = EnemyData.current.GetEnemyID();
    }

    void Update ()
    {
        if (PlayerData.current.isGamePaused)
            return;
        CheckIfPlayerVisible();
        if (!isPlayerVisible)
        {
            PatrolArea();
        }
        else
        {
            FollowPlayer();
        }
        CheckWhereEnemyIsFacing();
    }

    void LateUpdate()
    {
        // ANIMATION
        enemyAnimator.SetBool("isWalking", isWalking);
        enemyAnimator.SetBool("startIdleA", isIdleA);
    }

    void CheckWhereEnemyIsFacing()
    {
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

    public void StandbyToMeleeAttackPlayer()
    {
        // succubus is ranged
        if (type == EnemyType.Succubus)
            return;
        isNearPlayer = true;
        if (!isAttacking)
        {
            isAttacking = true;
            enemyAnimator.SetBool("isAttacking",true);
            StartCoroutine(AttackPlayer());
        }
    }

    public void StopStandbyToMeleeAttackPlayer()
    {
        // succubus is ranged
        if (type == EnemyType.Succubus)
        {
            Debug.Log("not succubi");
            return;
        }
        isNearPlayer = false;
        isAttacking = false;
        enemyAnimator.SetBool("isAttacking", false);
    }

    public void StandbyToShootPlayer()
    {
        if (type != EnemyType.Succubus)
        {
            return;
        }
        isPlayerInProjectileRange = true;
        if (!isAttacking)
        {
            isAttacking = true;
            enemyAnimator.SetBool("isAttacking", true);
            StartCoroutine(ShootPlayer());
        }
    }

    public void StopStandbyToShootPlayer()
    {
        if (type != EnemyType.Succubus)
        {
            return;
        }
        isPlayerInProjectileRange = false;
        isAttacking = false;
        enemyAnimator.SetBool("isAttacking", false);
    }

    private IEnumerator AttackPlayer()
    {
        while (isAttacking)
        {
            yield return new WaitForSeconds(attackCooldown);
            PlayerData.current.DamagePlayer(damagePerAttack);
        }
    }

    private IEnumerator ShootPlayer()
    {
        while (isAttacking)
        {
            yield return new WaitForSeconds(attackCooldown);

            //PlayerData.current.DamagePlayer(damagePerAttack);
        }
    }

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(enemyProjectile, projectileExitPoint.position, projectileExitPoint.rotation, projectileExitPoint);
        projectile.GetComponent<EnemyProjectile>().StartProjectile(isFacingRight);
        projectile.transform.parent = null;
    }

    void CheckIfPlayerVisible()
    {
        if (Vector2.Distance(playerTransform.position, transform.position) <= sightRadius)
        {
            isPlayerVisible = true;
            isWalking = true;
        }
        else
        {
            isPlayerVisible = false;
            isIdleA = true;
            isWalking = false;
        }
    }

    void PatrolArea()
    {

    }

    void FollowPlayer()
    {
        if (isNearPlayer)
            return;
        targetPosition = playerTransform.position;
        GetTargetPositionAndDirection();
        MoveEnemy();
    }

    void GetTargetPositionAndDirection()
    {
        dirNormalized = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        dirNormalized = dirNormalized.normalized;
    }

    void MoveEnemy()
    {
        if (!isDying)
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHP = currentHP - damageAmount;
        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        if (!isDying)
        {

            isDying = true;
            PlayerData.current.AddExp(expDrop);

            // DEATH AUDIO
            audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
            audioControl.PlayOneShot(deathSFX, deathSFXVolume);

            // DEATH ANIMATION
            enemyAnimator.SetBool("isDead", true);
            StartCoroutine(DestroyAfterXSeconds(deathAnimation.length));
        }
    }

    private IEnumerator DestroyAfterXSeconds(float xSeconds)
    {
        yield return new WaitForSeconds(xSeconds);
        Destroy(gameObject);
    }
}
