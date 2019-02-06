﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region CURRENT STATE
    private bool isAlive = true;
    private bool isDeathAnimation = false;
    private bool canPauseGame = true;

    #region MOVEMENT STATE
    private bool isFacingRight = true;
    public bool isWalking = false;
    private bool isWalkingInObstacle = false;       // detected collision with background - player has to stop walking
    private bool isIdleA = false;                   // is in idle animation state A
    private bool preparingIdleAnimationA = false;   // true if cooldown for idle animation A is started
    private bool preparingIdleAnimationB = false;
    private bool isMovementLocked = false;          // happens when player atack anim plays
    public bool isWaitingToMove = false;            // true when movement is locked (e.g. casting spell) and has to remember the last clicked position where you shall move later
    #endregion

    public bool isNearEnemy = false;              // used to check if player can melee attack
    public int nearEnemyID = -1;
    public int targetEnemyID = -2;

    private bool isRegeneratingMana = false;
    public bool isCastingSpell = false;
    private bool isFireballCooldown = false;
    public bool hasCastSpellAtLeastOnce = false;
    public bool hasMeleeAttackedAtLeastOnce = false;

    private int lastKnownPlayerLevel;
    #endregion

    #region PICKING UP ITEMS
    public bool isWalkingToPickUpItem = false;
    public Item itemAwaitingPickup;
    public float lastItemPickupCommandTime;
    #endregion

    #region PLAYER INPUT & CONTROLS
    /// MOUSE INPUT
    public float lastClickedTime; // used for correctly managing picking up items        
    private bool hasRightClickedRecently = false; // True if right click more recent than left click. Used to reset isWaitingToMove  
    public bool isClickingOnUI = false; // don't allow movement when clicking on certain UI elements 
    public bool isMouseOverEnemy = false; // used by CursorController.cs to detect whether to animate cursor. NB! DOESN'T WORK ON SKULLBOSSS              
    #endregion

    #region MOVEMENT
    float minMousePressTime = 0.1f; // if you press mouse for a shorter duration than this, it will be considered a click
    float mousePressStartTime;
    bool isCountingMousePress = false;
    bool isMousePress = false;

    public Vector2 targetPosition;
    public Vector2 dirNormalized;
    #endregion

    #region COMPONENTS
    Rigidbody2D rigidBody2D;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    ToggleActiveState characterPanelToggle;
    [SerializeField]
    ToggleActiveState inventoryPanelToggle;

    [SerializeField]
    Image healthBar;
    [SerializeField]
    Image manaBar;
    [SerializeField]
    Image expBar;
    [SerializeField]
    Image fireballCooldownBar;
    [SerializeField]
    GameObject defeatPanel;
    UnspentSkillpointCheck unspentSkillPointCheck;
    [SerializeField]
    GameObject skillPointNotification; // active if player has unspent skillpoints
    [SerializeField]
    GameObject victoryScreen;
    [SerializeField]
    Text brutalVictoryText;
    string brutalVictoryString = "IN BRUTAL MODE";
    #endregion

    #region ANIMATION
    float waitTimeBeforeIdleB = 5f;
    float idleAnimBStartTime;
    float waitTimeBeforeIdleA = 0.5f;
    float idleAnimAStartTime;
    public float meleeAttackAnimStartTime;

    [Header("ANIMATION")]
    [SerializeField]
    Animator playerAnimator;
    public AnimationClip meleeAttackAnimation;
    public AnimationClip spellcastAnimation;
    [SerializeField]
    AnimationClip victoryAnimation;

    string player_ClipName;
    AnimatorClipInfo[] player_CurrentClipInfo;
    #endregion

    #region ATTACK and TARGETTING
    [Header("ATTACK AND TARGETTING")]
    //private EnemyController currentEnemy;
    public EnemyController lastHoveredEnemy; // last enemy that you hovered mouse over
    public List<EnemyController> enemiesInMeleeRange = new List<EnemyController>(); // all enemies that may get damaged if you perform a melee attack

    private bool isAttacking = false;
    private bool isAttackCooldown = false;

    #endregion

    #region SPELLCASTING
    [Header("SPELLS")]
    [SerializeField]
    Transform fireballExitPoint;
    [SerializeField]
    GameObject fireBall;
    private float spellcastEndTime;          // when casting animation is over
    public float fireballCooldownStartTime; // when you can cast fireball again
    private Vector2 fireballTargetPosition; 
    #endregion

    #region AUDIO
    [Header("AUDIO")]
    [SerializeField]
    PlayerSFX playerSFX;
    [SerializeField]
    GameObject audioManager;
    AudioSource audioSource;
    #endregion

    #region ACTIVE ABILITY
    PlayerActiveAbilityManager playerActiveAbilityManager;
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
        InitializeVariables();
        GetComponents();
    }

    private void InitializeVariables()
    {
        unspentSkillPointCheck = new UnspentSkillpointCheck();
        lastKnownPlayerLevel = PlayerData.current.currentLevel;
    }

    private void GetComponents()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        audioSource = gameObject.GetComponent<AudioSource>();
        playerActiveAbilityManager = gameObject.GetComponent<PlayerActiveAbilityManager>();
    }

    public void WinGame()
    {
        PlayerData.current.isGamePaused = true;
        victoryScreen.SetActive(true);
        victoryScreen.GetComponent<AudioSource>().enabled = true;

        if (PlayerData.current.isPlayingBrutalMode)
        {
            brutalVictoryText.text = brutalVictoryString;
        }

        StartCoroutine(Win());
    }

    private IEnumerator Win()
    {
        yield return new WaitForSeconds(victoryAnimation.length);
        audioManager.SetActive(false);
    }

    /// <summary>
    /// Get animator info so that we can check a particular animation clip is playing
    /// </summary>
    private void GetAnimatorInfo()
    {
        player_CurrentClipInfo = this.playerAnimator.GetCurrentAnimatorClipInfo(0);
        player_ClipName = player_CurrentClipInfo[0].clip.name;
    }

    void Update()
    {
        UpdateHUD();
        GetAnimatorInfo();
        if (!isAlive)
        {
            Die();
        }
        if (PlayerData.current.isGamePaused || !isAlive)
        {
            return;
        }
        ListenForDamageTaken();
        ListenForPlayerDefeat();
        ListenToLVChange();
        UpdateTimePlayed();

        // PLAYER INPUT
        ListenForGamePause();
        ListenForMenuOpen();            // opening character panel / inventory via keyboard input
        ListenForActiveSkillSwitch();
        ManageLeftMouseInput();         // for walking
        ManageRightMouseInput();        // for active ability

        CheckWherePlayerIsFacing();
        // END MELEE ATTACK STATE
        if (isAttacking && Time.time > meleeAttackAnimStartTime + meleeAttackAnimation.length)
        {
            preparingIdleAnimationB = false;
            isAttacking = false;
        }
        // END SPELL CAST STATE
        if (isCastingSpell && Time.time > spellcastEndTime)
        {
            preparingIdleAnimationB = false;
            isCastingSpell = false;
        }
        // MANA REGEN
        if (PlayerData.current.maxMana > PlayerData.current.currentMana)
        {
            if (!isRegeneratingMana)
            {
                isRegeneratingMana = true;
                StartCoroutine(RegenerateMana());
            }
        }
    }

    void ListenForMenuOpen()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            characterPanelToggle.Toggle();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanelToggle.Toggle();
        }
    }

    /// <summary>
    /// For using active ability such as fireball or sword attack
    /// </summary>
    void ManageRightMouseInput()
    {
        // RIGHT CLICK
        if (Input.GetMouseButtonDown(1))
        {
            hasRightClickedRecently = true;
            isWaitingToMove = false;
            GetTargetPositionAndDirection();
            CheckWherePlayerIsFacing();

            UseActiveAbility();
        }
        // RIGHT MOUSE BUTTON PRESS
        else if (Input.GetMouseButton(1))
        {
            UseActiveAbility();
        }
    }

    void UseActiveAbility()
    {
        // cast fireball
        if (playerActiveAbilityManager.currentActiveAbility.abilityType == PlayerActiveAbilityTypes.SpellFireball)
        {
            GetFireballTargetPosition();
            StartSpellcasting();
        }
        // melee attack
        else if (playerActiveAbilityManager.currentActiveAbility.abilityType == PlayerActiveAbilityTypes.MeleeSword)
        {
            StartMeleeSwordAttacking();
        }
    }

    void ListenForActiveSkillSwitch()
    {
        if (!PlayerData.current.isGamePaused)
        {
            // SCROLLL UP - select previous skill in list
            if (Input.mouseScrollDelta.y > 0)
            {
                ToggleActiveSkill(false);
            }
            // SCROLLL DOWN - select next skill in list
            else if (Input.mouseScrollDelta.y < 0)
            {
                ToggleActiveSkill(true);
            }
        }
    }

    /// <summary>
    /// Switches between player active skils depending on mouse wheel input
    /// </summary>
    /// <param name="selectNextSkill">if this is true - select next active skill in list, if false - the previous skill </param>
    void ToggleActiveSkill(bool selectNextSkill = true)
    {
        playerActiveAbilityManager.SwitchActiveAbility();
    }

    /// <summary>
    /// Moves the main character if the player has clicked or is holding left mouse
    /// </summary>
    void ManageLeftMouseInput()
    {
        // WALKING
        // walking - player holds mouse key
        if (Input.GetMouseButton(0))
        {
            hasRightClickedRecently = false;
            

            // Move only if the cursor is not above UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                GetTargetPositionAndDirection();
                CheckIfPlayerIsWalking();
            }
        }
        // walking - player clicks
        if (Input.GetMouseButtonDown(0))
        {
            // only consider it a mouse press if player has held the key down for minMousePressTime seconds
            if (!isCountingMousePress)
            {
                isCountingMousePress = true;
                mousePressStartTime = Time.time + minMousePressTime;
            }
            else if (Time.time > mousePressStartTime)
            {
                isMousePress = true;
                CheckIfPlayerIsWalking();
            }
            // store last clicked time
            lastClickedTime = Time.time;
        }

        // resetting mouse press to move
        if (Input.GetMouseButtonUp(0))
        {
            isMousePress = false;
            isCountingMousePress = false;
        }
    }

    void UpdateTimePlayed()
    {
        PlayerData.current.playTime += Time.deltaTime;
    }

    void ListenToLVChange()
    {
        if (lastKnownPlayerLevel != PlayerData.current.currentLevel)
        {
            // PLAY LV UP SFX
            lastKnownPlayerLevel = PlayerData.current.currentLevel;
            playerSFX.PlayLvUpSFX();

            // this check is added to fix a bug where you dont regen mana after level up
            isRegeneratingMana = false;

            // update relevant UI
            // update skill points notification
            unspentSkillPointCheck.ShowUIIfEnoughSkillPoints(skillPointNotification);
        }
    }

    void ListenForDamageTaken()
    {
        // player is wounded
        if (PlayerData.current.playerWoundDetected == true)
        {
            PlayerData.current.playerWoundDetected = false;
            //dirNormalized = dirNormalized * -1f;
            if (isAlive)
            {
                playerAnimator.SetTrigger("damageTaken");
                playerSFX.PlayWoundedSFX();
            }
        }
    }

    private void StartSpellcasting()
    {
        // fireball cast cooldown still active
        if (Time.time < fireballCooldownStartTime + PlayerData.current.fireballCastCooldown + spellcastAnimation.length)
        {
            return;
        }

        if (PlayerData.current.currentMana >= PlayerData.current.fireballManaCost)
        {
            hasCastSpellAtLeastOnce = true;
            isCastingSpell = true;
            spellcastEndTime = Time.time + spellcastAnimation.length;
            fireballCooldownStartTime = Time.time;
            playerAnimator.SetTrigger("castSpellA");
            StartCoroutine(ShootFireBall());
            PlayerData.current.currentMana -= PlayerData.current.fireballManaCost;
        }
    }

    // shoots fireball after animation is over
    private IEnumerator ShootFireBall()
    {
        yield return new WaitForSeconds(spellcastAnimation.length);
        GameObject projectile = Instantiate(fireBall, fireballExitPoint.position, fireballExitPoint.rotation, fireballExitPoint);
        projectile.GetComponent<Fireball>().StartFireball(isFacingRight, fireballTargetPosition);
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
        // ANIMATIONEventSystem.current.IsPointerOverGameObject
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
        if (Time.time > idleAnimAStartTime && isAlive)
        {
            isIdleA = true;
            if (!preparingIdleAnimationB)
                idleAnimBStartTime = Time.time + waitTimeBeforeIdleB;
            preparingIdleAnimationB = true;
            playerAnimator.SetBool("startIdleA", isIdleA);
        }
        if (preparingIdleAnimationB && Time.time > idleAnimBStartTime)
        {
            playerAnimator.SetTrigger("playIdleB");
            preparingIdleAnimationB = false;
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
        if (!isDeathAnimation)
        {
            isDeathAnimation = true;
            canPauseGame = false;
            PlayerData.current.isGamePaused = true;

            // GET SIN TREE POINTS
            if (PlayerData.current.killsRequiredForNextPoint < PlayerData.current.enemiesKilled)
            {
                PlayerData.current.sinTreePoints++;
                PlayerData.current.killsRequiredForNextPoint++;
            }

            playerAnimator.SetBool("isDead", true);
            StartCoroutine(DisplayDefeatPanelAfterXSeconds(2f));
        }
    }

    private IEnumerator DisplayDefeatPanelAfterXSeconds(float xSeconds)
    {
        yield return new WaitForSeconds(xSeconds);
        defeatPanel.SetActive(true);
    }

    void ListenForPlayerDefeat()
    {
        if (PlayerData.current.currentLife == 0)
            isAlive = false;
    }

    void ListenForGamePause()
    {
        if ((Input.GetKeyDown(KeyCode.P) || (Input.GetKeyDown(KeyCode.Escape) && canPauseGame)))
        {
            pauseMenu.SetActive(true);
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

        // update active skil cooldown - this is donne in PlayerActiveAbilityManager.cs

        /*if (hasCastSpellAtLeastOnce)
        {
            // bar fill
            fireballCooldownBar.fillAmount = (Time.time - fireballCooldownStartTime) / (PlayerData.current.fireballCastCooldown + spellcastAnimation.length);

            // bar color changes if insufficient mana
            if (PlayerData.current.currentMana < PlayerData.current.fireballManaCost)
                fireballCooldownBar.color = new Color(0.613f, 0.362f, 0.362f);
            else
                fireballCooldownBar.color = Color.white;
        }*/
    }

    void GetFireballTargetPosition()
    {
        fireballTargetPosition = Input.mousePosition;
        fireballTargetPosition = Camera.main.ScreenToWorldPoint(fireballTargetPosition);
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

    /// <summary>
    /// If attack is not on cooldown - play melee anim, sfx; if also near enemies - damage them
    /// </summary>
    private void StartMeleeSwordAttacking()
    {
        // Check if the sword attack is not on cooldown
        if (Time.time <
            meleeAttackAnimStartTime + PlayerData.current.meleeAttackCooldown + meleeAttackAnimation.length)
        {
            isAttackCooldown = true;
        }
        else
        {
            isAttackCooldown = false;
        }
        if (isAttacking || isAttackCooldown)
        {
            return;
        }

        // Start attack
        hasMeleeAttackedAtLeastOnce = true;
        PlayMeleeSwordAttackAnimation();
        if (isNearEnemy)
        {
            MeleeAttack();
        }
    }

    /// <summary>
    /// locks player movement and plays the attack animation and sfx
    /// </summary>
    private void PlayMeleeSwordAttackAnimation()
    {
        isAttacking = true;
        isWalking = false;

        
        meleeAttackAnimStartTime = Time.time;
        playerAnimator.SetTrigger("meleeAttack");
        playerSFX.PlayMeleeSFX();
    }

    /// <summary>
    /// Deal melee damage to all enemies within range with a chance of critical hit
    /// </summary>
    private void MeleeAttack()
    {
        int meleeDamage = PlayerData.current.meleeDamage;

        // roll critical strike
        if (Random.Range(0f, 1f) <= PlayerData.current.meleeCritChance)
        {
            Debug.Log("CRITICAL");
            meleeDamage = (int)(meleeDamage * PlayerData.current.meleeCriticalEffect);
        }

        // deal damage
        foreach (EnemyController enemyInMeleeRange in enemiesInMeleeRange)
        {
            enemyInMeleeRange.TakeDamage(meleeDamage, DamageSource.PlayerMelee);
        }
    }

    public void CheckIfPlayerIsWalking()
    {
        // Player near target position - stop walking
        if (Vector2.Distance(targetPosition, transform.position) <= 0.02f)
        {
            isWalking = false;
            isWaitingToMove = false;
        }
        // movement locked due to melee attack animation
        else if (isAttacking)
        {
            isWalking = false;
            // make sure that you only register movement commands since after attack command
            if (!hasRightClickedRecently)
                isWaitingToMove = true;
        }
        // movement locked due to fireball animation
        else if (isCastingSpell)
        {
            isWalking = false;
            // make sure that you only register movement commands since after attack command
            if (!hasRightClickedRecently)
                isWaitingToMove = true;
        }
        else if (isWalkingInObstacle)
        {
            isWalking = false;
            isWaitingToMove = false;
            isWalkingInObstacle = false;
        }
        else if (isClickingOnUI)
        {
            isWalking = false;
            isWaitingToMove = false;
            isClickingOnUI = false;
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
            //Debug.Log("walked into obstacle");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Background")
        {
            isWalkingInObstacle = false;
            //Debug.Log("walked OUT");
        }
    }

    /*private void MovePlayer()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * PlayerData.current.moveSpeed * Time.deltaTime;
    }*/
}