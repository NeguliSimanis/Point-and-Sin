using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region CURRENT STATE
    private bool isAlive = true;
    private bool canPauseGame = true;
    private bool isFacingRight = true;
    private bool isWalking = false;
    private bool isWalkingInObstacle = false;     // detected collision with background - player has to stop walking
    private bool isIdleA = false;                 // is in idle animation state A
    private bool preparingIdleAnimationA = false; // true if cooldown for idle animation A is started
    private bool isMovementLocked = false;        // happens when player atack anim plays

    public bool isNearEnemy = false;              // used to check if player can melee attack
    public int nearEnemyID = -1;
    public int targetEnemyID = -2;

    private float attackCooldownResetTime;
    private bool isAttacking = false;
    private bool isAttackCooldown = false;
    #endregion

    #region MOVEMENT
    private Vector2 targetPosition;
    private Vector2 dirNormalized;
    #endregion

    #region COMPONENTS
    Rigidbody2D rigidBody2D;
    #endregion

    #region UI
    [SerializeField] Image healthBar;
    [SerializeField] GameObject defeatPanel;
    #endregion

    #region ANIMATION
    float waitTimeBeforeIdleA = 0.1f;
    float idleAnimAStartTime;
    float meleeAttackAnimStartTime;
    [SerializeField] Animator playerAnimator;
    [SerializeField] AnimationClip meleeAttackAnimation;
    #endregion

    #region ATTACK and TARGETTING
    EnemyController currentEnemy;
    #endregion

    private void Awake()
    {
        LoadPlayerData();
    } 

    void LoadPlayerData()
    {
        if (PlayerData.current == null)
            PlayerData.current = new PlayerData();
        PlayerData.current.isGamePaused = false;
    }

    void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateHealthBar();
        ListenForGamePause();
        ListenForPlayerDefeat();
        if (!isAlive)
            Die();
        if (PlayerData.current.isGamePaused)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            GetTargetPositionAndDirection();
            CheckIfPlayerIsWalking();
        }
        if (isWalking)
        {
            CheckIfPlayerIsWalking();
            MovePlayer(); 
        }
        CheckWherePlayerIsFacing();
        if (isAttacking && Time.time > meleeAttackAnimStartTime + meleeAttackAnimation.length + 0.01f)
        {
            isAttacking = false;
        }
    }

    void LateUpdate()
    {
        // ANIMATION
        playerAnimator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            preparingIdleAnimationA = false;
            isIdleA = false;
        }
        // set start time for playing idle animation A
        if (!isWalking && !preparingIdleAnimationA)
        {
            preparingIdleAnimationA = true;
            idleAnimAStartTime = Time.time + waitTimeBeforeIdleA;
        }
        // start playing idle animation A
        if (Time.time > idleAnimAStartTime)
        {
            isIdleA = true;
            playerAnimator.SetBool("startIdleA", isIdleA); 
        }
    }

    void CheckWherePlayerIsFacing()
    {
        if (isAttacking)
            return;
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

    void Die()
    {
        canPauseGame = false;
        PlayerData.current.isGamePaused = true;
        defeatPanel.SetActive(true);    
    }

    void ListenForPlayerDefeat()
    {
        if (PlayerData.current.currentLife == 0)
            isAlive = false;
    }

    void ListenForGamePause()
    {
        if (Input.GetKeyDown(KeyCode.P) && canPauseGame)
        {
            PlayerData.current.isGamePaused = !PlayerData.current.isGamePaused;
        }
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = (PlayerData.current.currentLife * 1f) / PlayerData.current.maxLife;
    }

    void GetTargetPositionAndDirection()
    {
        targetPosition = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
        dirNormalized = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        dirNormalized = dirNormalized.normalized;
    }

    public void TargetEnemy(int enemyID, EnemyController target)
    {
        if (isAttacking || isAttackCooldown)
            return;
        targetEnemyID = enemyID;
        currentEnemy = target;
        if (nearEnemyID == enemyID)
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        isAttacking = true;
        isWalking = false;
        meleeAttackAnimStartTime = Time.time;
        playerAnimator.SetTrigger("meleeAttack");

        // deal damage
        currentEnemy.TakeDamage(PlayerData.current.meleeDamage);
    }

    void CheckIfPlayerIsWalking()
    {
        if (Vector2.Distance(targetPosition, transform.position) <= 0.01f)
        {
            isWalking = false;
        }
        // movement locked due to melee attack animation
        else if (isAttacking)
        {
            isWalking = false;
        }
        else if (isWalkingInObstacle)
        {
            isWalking = false;
            isWalkingInObstacle = false;
        }
        else
        {
            isWalking = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Background")
        {
            isWalkingInObstacle = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Background")
        {
            isWalkingInObstacle = false;
        }
    }

    void MovePlayer()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * PlayerData.current.moveSpeed * Time.deltaTime;
    }
}
