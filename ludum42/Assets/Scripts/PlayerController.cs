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

    private bool isRegeneratingMana = false;
    private bool isCastingSpell = false;

    private int lastKnownPlayerLevel;
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
    [SerializeField] Image manaBar;
    [SerializeField] Image expBar;
    [SerializeField] GameObject defeatPanel;
    #endregion

    #region ANIMATION
    float waitTimeBeforeIdleA = 0.1f;
    float idleAnimAStartTime;
    float meleeAttackAnimStartTime;
    [SerializeField] Animator playerAnimator;
    [SerializeField] AnimationClip meleeAttackAnimation;
    [SerializeField] AnimationClip spellcastAnimation;
    #endregion

    #region ATTACK and TARGETTING
    EnemyController currentEnemy;
    #endregion

    #region SPELLCASTING
    [SerializeField] Transform fireballExitPoint;
    [SerializeField] GameObject fireBall;
    private float spellcastEndTime;
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
        lastKnownPlayerLevel = PlayerData.current.currentLevel;
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateHUD();
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
        if (Input.GetMouseButtonDown(1))
        {
            GetTargetPositionAndDirection();
            CheckWherePlayerIsFacing();
            StartSpellcasting();
        }
        if (isWalking)
        {
            CheckIfPlayerIsWalking();
            MovePlayer(); 
        }
        CheckWherePlayerIsFacing();
        // END MELEE ATTACK STATE
        if (isAttacking && Time.time > meleeAttackAnimStartTime + meleeAttackAnimation.length + 0.01f)
        {
            isAttacking = false;
        }
        // END SPELL CAST STATE
        if (isCastingSpell && Time.time > spellcastEndTime)
        {
            isCastingSpell = false;
        }
        // MANA REGEN
        if (PlayerData.current.maxMana > PlayerData.current.currentMana)
        {
            // this check is added to fix a bug where you dont regen mana after level up
            if (lastKnownPlayerLevel != PlayerData.current.currentLevel)
            {
                lastKnownPlayerLevel = PlayerData.current.currentLevel;
                isRegeneratingMana = false;
            }
            if (!isRegeneratingMana)
            {
                isRegeneratingMana = true;
                StartCoroutine(RegenerateMana());
            }
        }
    }

    private void StartSpellcasting()
    {
        if (PlayerData.current.currentMana >= PlayerData.current.fireballManaCost)
        {
            isCastingSpell = true;
            spellcastEndTime = Time.time + spellcastAnimation.length;
            playerAnimator.SetTrigger("castSpellA");
            ShootFireBall();
            PlayerData.current.currentMana -= PlayerData.current.fireballManaCost;
        }
    }

    private void ShootFireBall()
    {
        GameObject projectile = Instantiate(fireBall, fireballExitPoint.position, fireballExitPoint.rotation, fireballExitPoint);
        projectile.GetComponent<Fireball>().StartFireball(isFacingRight);
        projectile.transform.parent = null;
    }

    public IEnumerator RegenerateMana()
    {
        while (PlayerData.current.maxMana > PlayerData.current.currentMana)
        {
            PlayerData.current.currentMana += PlayerData.current.manaRegenPerInterval;
            if (PlayerData.current.currentMana >= PlayerData.current.maxMana)
            {
                PlayerData.current.currentMana = PlayerData.current.maxMana;
                isRegeneratingMana = false;
            }
            yield return new WaitForSeconds(PlayerData.current.manaRegenInterval);
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
        if (isAttacking || isCastingSpell)
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

    void UpdateHUD()
    {
        // update hp bar
        healthBar.fillAmount = (PlayerData.current.currentLife * 1f) / PlayerData.current.maxLife;

        // update mana bar
        manaBar.fillAmount = (PlayerData.current.currentMana * 1f) / PlayerData.current.maxMana;

        // update exp bar
        expBar.fillAmount = (PlayerData.current.currentExp * 1f) / PlayerData.current.requiredExp;
    }

    void GetTargetPositionAndDirection()
    {
        targetPosition = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
        GetDirNormalized(targetPosition);
    }

    void GetDirNormalized(Vector2 sourceVector)
    {
        dirNormalized = new Vector2(sourceVector.x - transform.position.x, sourceVector.y - transform.position.y);
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
        // movement locked due to fireball animation
        else if (isCastingSpell)
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
            Debug.Log("walked into obstacle");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Background")
        {
            isWalkingInObstacle = false;
            Debug.Log("walked OUT");
        }
    }

    void MovePlayer()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * PlayerData.current.moveSpeed * Time.deltaTime;
    }
}
