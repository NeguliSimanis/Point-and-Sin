using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public static PlayerData current;

    #region STATS
    public int enemiesKilled = 0;
    public float playTime = 0;
    #endregion

    #region GAME STATE
    public bool isGamePaused = false;
   // public bool allowGamePause = false;
    public bool canPlayBackgroundMusic = false;
    #endregion

    #region PLAYER STATE
    public bool playerWoundDetected = false;
    #endregion

    #region MOVEMENT
    public float moveSpeed = 0.8f;
    #endregion

    #region LIFE
    public int currentLife;
    public int maxLife = 100;
    private int defaultMaxLife = 100;
    private int lifePerLevel = 5;
    private int prideLifeIncrease = 5;
    #endregion

    #region MANA
    public int currentMana = 50;
    public int maxMana = 50;
    private int defaultMaxMana = 50;
    private int manaPerLevel = 3;
    private float prideMaxManaIncrease = 0.05f;

    // MANA REGEN
    public int manaRegenPerSecond = 5;
    private int defaultManaRegenPerSecond = 5;
    private int lustManaRegenIncrease = 1;
    public float manaRegenInterval = 0.2f;
    public int manaRegenPerInterval;
    #endregion

    #region SPELLS
    public int fireballDamage = 6;
    private int defaultFireballDamage = 6;
    public int fireballManaCost = 20;
    private int defaultFireballCost = 20;
    public float fireballCastCooldown = 1.2f;
    private float defaultFireballCastCooldown = 1.2f;

    // LUST MODIFIERS
    private float lustFireballCooldownReduce = 0.96f;

    // WRATH MODIFIERS
    private int wrathFireballCostIncrease = 1;
    private int wrathFireballDamageIncrease = 2;
    #endregion

    #region MELEE ATTACK
    public int meleeDamage = 8;
    private int defaultMeleeDamage = 8;
    private int wrathMeleeDamageIncrease = 1;
    public float meleeAttackCooldown = 0.3f;

    public float meleeCritChance = 0.01f;
    private float defaultMeleeCritChance = 0.01f;
    public float meleeCriticalEffect = 1.5f;
    private float lustMeleeCriticalIncrease = 0.005f;
    #endregion

    #region LEVELLING
    public int currentExp = 0;
    public int currentLevel = 1;
    public int requiredExp = 90;
    private int defaultRequiredExp = 90;
    #endregion

    #region SKILLS
    public int sinPoints = 0;
    public int wrath = 1;
    public int pride = 1;
    public int lust = 1;
    public int sloth = 0;
    #endregion

    #region MINIONS
    public bool summonMinionOnMeleeKill = false;       // Used to test the first skill. TO-DO: CHANGE THIS
    public int currentMinions = 0;
    public int maxMinions = 0;
    public List<EnemyController> minions = new List<EnemyController>();
    #endregion

    #region SIN TREE
    public int sinTreePoints = 0;
    public int killsRequiredForNextPoint = 1;   // if you die before killing this number of enemies, you don't get points
    #endregion

    public PlayerData()
    {
        if (SinTree.current == null)
            SinTree.current = new SinTree();
        isGamePaused = false;
        Reset();
    }

    public void Reset()
    {
        // RESET MINIONS
        minions.Clear();

        // reset sin tree effects
        if (SinTree.allSkills[0].skillLV == 0)
        {
            summonMinionOnMeleeKill = false; 
            currentMinions = 0;
            maxMinions = 0;
        }

        // reset game state
        isGamePaused = false;

        // reset stats
        enemiesKilled = 0;
        playTime = 0;
        currentMinions = 0;

        // reset life
        currentLife = defaultMaxLife;
        maxLife = defaultMaxLife;

        // reset mana
        currentMana = defaultMaxMana;
        maxMana = defaultMaxMana;
        GetManaRegenPerInterval();

        // reset level
        currentLevel = 1;

        // reset sin points
        sinPoints = 0;
        wrath = 1;
        pride = 1;
        lust = 1;
        sloth = 1;

        // reset exp
        currentExp = 0;
        requiredExp = defaultRequiredExp;
    }

   /* public SinTreeSkill UpgradeSinTreeSkill(int skillID)
    {
        SinTreeSkill skillToUpgrade = playerSinTree.allSkills[skillID];

        // spend resources
        sinTreePoints -= skillToUpgrade.skillCost;

        // update skill data
        skillToUpgrade.skillCost += 2;
        skillToUpgrade.skillLV += 1;

        if (skillToUpgrade.skillID == 0)
        {
            summonMinionOnMeleeKill = true;
            maxMinions++;
        }
        return skillToUpgrade;
    }

    public SinTreeSkill GetSkillInfo(int skillID)
    {
        return playerSinTree.allSkills[skillID];
    }*/

    public void Pause(bool isPaused)
    {
        isGamePaused = isPaused;
    }

    public void AddExp(int expGained)
    {
        currentExp += expGained;
        if (currentExp >= requiredExp)
        {
            currentExp = currentExp - requiredExp; 
            LevelUp();
        }
    }

    #region UPGRADING SINS
    public void AddWrath(int amount)
    {
        wrath += amount;

        // melee damage increase
        meleeDamage = defaultMeleeDamage + (wrath - 1) * wrathMeleeDamageIncrease;
        //Debug.Log("Melee damage " + meleeDamage);

        // fireball damage increase
        fireballDamage = defaultFireballDamage + (wrath - 1) * wrathFireballDamageIncrease;
        //Debug.Log("Fireball damage " + fireballDamage);

        // fireball cost increase
        fireballManaCost = defaultFireballCost + (wrath - 1) * wrathFireballCostIncrease;
        //Debug.Log("Fireball cost " + fireballManaCost);
    }

    public void AddPride(int amount)
    {
        pride += amount;

        // max mana increase
        maxMana = (int)(defaultMaxMana * (1f + (pride - 1) * prideMaxManaIncrease));
        //Debug.Log("Max mana " + maxMana);

        // max life increase
        maxLife = (int)(defaultMaxLife + (currentLevel - 1) * lifePerLevel + (pride - 1) * prideLifeIncrease);
       // Debug.Log("Max life " + maxLife);
        currentLife = maxLife;
    }

    public void AddLust(int amount)
    {
        lust += amount;

        // mana regen increase
        manaRegenPerSecond = defaultManaRegenPerSecond + (lust - 1) * lustManaRegenIncrease;
        //Debug.Log("Mana regen " + manaRegenPerSecond);

        // reduce spell cooldown
        float totalLustCooldownEffect = defaultFireballCastCooldown;
        for (int i = 0; i < lust - 1; i++)
        {
            totalLustCooldownEffect *= lustFireballCooldownReduce;
        }
        fireballCastCooldown = totalLustCooldownEffect;
        //Debug.Log("Fireball cooldown " + fireballCastCooldown);

        // increase melee critical chance
        meleeCritChance = defaultMeleeCritChance + (lust - 1) * lustMeleeCriticalIncrease;
        //Debug.Log("Melee crit chance " + meleeCritChance);
    }

    public void AddSloth(int amount)
    {
        sloth += amount;
    }
    #endregion

    private void LevelUp()
    {
        currentLevel++;
        sinPoints += 2;
        maxLife += lifePerLevel;
        maxMana += manaPerLevel;
        currentLife = maxLife;
        currentMana = maxMana;
        requiredExp = (int)(requiredExp * 1.3f);
    }

    void GetManaRegenPerInterval()
    {
        manaRegenPerInterval = (int)(manaRegenPerSecond * (manaRegenInterval/1f));
    }

    public void DamagePlayer(int damageAmount)
    {
        playerWoundDetected = true;
        if (damageAmount > currentLife)
            currentLife = 0;
        else
            currentLife -= damageAmount;
    }

    // returns the cost of cheapest sin tree skill
    public int CheapestSinTreeSkill()
    {
        // TODO
        return -1;
    }
}
