using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    #region DATA variables
    float moveSpeed = 0.13f;
    [SerializeField] float sightRadius = 3f;    // if player moves closer than this, he will be noticed
    [SerializeField] float attackCooldown = 1f; // deals damage to player once per this interval
    [SerializeField] int damagePerAttack = 35;  // how much damage is dealt in one attack
    #endregion

    #region STATE variables
    bool isPlayerVisible = false; // if true, move towards player to attack. If false, patrol the area
    bool isNearPlayer = false; // if true, stop to attack the player
    bool isAttacking = false;
    #endregion

    #region MOVEMENT variables
    private Vector2 targetPosition;
    private Vector2 dirNormalized;
    private Transform targetTransform;
    #endregion

    #region OTHER variables
    [SerializeField] Transform playerTransform;
    #endregion

    void Start ()
    {
		
	}
	
	void Update ()
    {
        CheckIfPlayerVisible();
        if (!isPlayerVisible)
        {
            PatrolArea();
        }
        else
        {
            FollowPlayer();
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
        Debug.Log("yay");
        while (isAttacking)
        {
            yield return new WaitForSeconds(attackCooldown);
            PlayerData.current.DamagePlayer(damagePerAttack);
            Debug.Log("attacking!");
        }
    }

    void CheckIfPlayerVisible()
    {
        if (Vector2.Distance(playerTransform.position, transform.position) <= sightRadius)
        {
            isPlayerVisible = true;
        }
        else
        {
            isPlayerVisible = false;
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
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * moveSpeed * Time.deltaTime;
    }
}
