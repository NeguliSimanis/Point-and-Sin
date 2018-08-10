using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public static PlayerData current;

    #region MOVEMENT
    public float moveSpeed = 0.8f;
    #endregion

    #region LIFE
    public int currentLife;
    public int maxLife = 100;
    #endregion

    public PlayerData()
    {
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
