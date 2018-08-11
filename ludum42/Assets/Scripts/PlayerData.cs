using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public static PlayerData current;

    #region GAME STATE
    public bool isGamePaused = false;
    #endregion

    #region MOVEMENT
    public float moveSpeed = 0.8f;
    #endregion

    #region LIFE
    public int currentLife;
    public int maxLife = 100;
    #endregion

    #region MELEE ATTACK
    public int meleeDamage = 10;
    public float meleeAttackCooldown = 0.3f;
    #endregion

    public PlayerData()
    {
        isGamePaused = false;
        Reset();
    }

    public void Reset()
    {
        //Debug.Log("RESET");
        isGamePaused = false;
        currentLife = maxLife;
    }

    public void DamagePlayer(int damageAmount)
    {
        if (damageAmount > currentLife)
            currentLife = 0;
        else
            currentLife -= damageAmount;
    }
}
