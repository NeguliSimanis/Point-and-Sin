using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    #region DATA variables
    float moveSpeed = 0.13f;
    public int enemyID = 0;
    private int expDrop = 40; // how much exp is gained by killing this mofo
    private int maxHP = 12;
    private int currentHP = 12;
    [SerializeField] float sightRadius = 3f;    // if player moves closer than this, he will be noticed
    [SerializeField] float attackCooldown = 1f; // deals damage to player once per this interval
    [SerializeField] int damagePerAttack = 35;  // how much damage is dealt in one attack
    #endregion

    #region STATE variables
    bool isPlayerVisible = false; // if true, move towards player to attack. If false, patrol the area
    bool isNearPlayer = false; // if true, stop to attack the player
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
    #endregion
	
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

    public void StandbyToAttackPlayer()
    {     
        isNearPlayer = true;
        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(AttackPlayer());
        }
    }

    public void StopStandbyToAttackPlayer()
    {
        isNearPlayer = false;
        isAttacking = false;
    }

    private IEnumerator AttackPlayer()
    {
        while (isAttacking)
        {
            yield return new WaitForSeconds(attackCooldown);
            PlayerData.current.DamagePlayer(damagePerAttack);
        }
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
        PlayerData.current.AddExp(expDrop);
        isDying = true;
        enemyAnimator.SetBool("isDead", true);
        StartCoroutine(DestroyAfterXSeconds(deathAnimation.length));
    }

    private IEnumerator DestroyAfterXSeconds(float xSeconds)
    {
        yield return new WaitForSeconds(xSeconds);
        Destroy(gameObject);
    }
}
