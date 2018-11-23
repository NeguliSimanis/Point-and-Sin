using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveAbility
{
    public PlayerActiveAbilityTypes abilityType;
    public GameObject icon;
    public Image cooldownBar;
}

/// <summary>
/// Stores information about player's active abilities
/// </summary>
public class PlayerActiveAbilityManager : MonoBehaviour
{
    /// <summary>
    /// must be in the same order as PlayerActiveAbilityTypes
    /// </summary>
    [Header("Active ability UI")]
    [SerializeField] 
    GameObject[] activeAbilityIcons;
    [SerializeField]
    Image[] activeAbilityCooldownBars; // must be in the same order as ActiveAbilityIcons
    [SerializeField]
    Transform activeAbilityPosition;
    
    /// <summary>
    /// e.g. fireball, melee - 
    /// Contains a list of objects with info about ability type and UI icon
    /// </summary>
    List <ActiveAbility> activeAbilities;// = new ActiveAbility[2];
    public ActiveAbility currentActiveAbility;

    int numberOfAbilities;
    int activeAbilityID = 0;
    PlayerController playerController;

    private void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        numberOfAbilities = activeAbilityIcons.Length;
        InitializeActiveAbilityList();

        currentActiveAbility = activeAbilities[0];
    }

    private void InitializeActiveAbilityList()
    {
        activeAbilities = new List<ActiveAbility>();

        for (int i = 0; i < numberOfAbilities; i++)
        {
         
            activeAbilities.Add(new ActiveAbility());
            activeAbilities[i].icon = activeAbilityIcons[i];
            activeAbilities[i].cooldownBar = activeAbilityCooldownBars[i];

            // FIREBALL - ability ID 0
            if (i == 0)
            {
                activeAbilities[i].abilityType = PlayerActiveAbilityTypes.SpellFireball;
            }
                 
            // MELEE - ability ID 1
            else if (i == 1)
            {
                activeAbilities[i].abilityType = PlayerActiveAbilityTypes.MeleeSword;
            }
        }
    }

    public void SwitchActiveAbility(int newActiveAbilityID = -1)
    {
        // select next ability from array
        if (newActiveAbilityID == -1)
        {
            activeAbilityID++;

            // end of array reached - select first ability in array
            if (activeAbilityID >= numberOfAbilities)
            {
                activeAbilityID = 0;
            }
            currentActiveAbility = activeAbilities[activeAbilityID];

            //Debug.Log("ABILITY " + activeAbilities[activeAbilityID].activeAbilityType + " selected");
        }
        //if (activeAbilities)
        ShowActiveAbilityIcon();
        HideInactiveAbilityIcon();
    }

    /// <summary>
    /// Activates the active ability icon object and moves it to the correct position
    /// </summary>
    private void ShowActiveAbilityIcon()
    {
        currentActiveAbility.icon.gameObject.SetActive(true);
        currentActiveAbility.icon.gameObject.transform.position = activeAbilityPosition.position;
    }

    /// <summary>
    /// goes through the list of all ability icons and hides the ones that are not currently active
    /// </summary>
    private void HideInactiveAbilityIcon()
    {
        for (int i = 0; i < numberOfAbilities; i++)
        {
            if (activeAbilities[i] != currentActiveAbility)
            {
                activeAbilities[i].icon.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        UpdateActiveAbilityCooldownBar();
    }

    private void UpdateActiveAbilityCooldownBar()
    {
        // MELEE COOLDOWN BAR
        if (currentActiveAbility.abilityType == PlayerActiveAbilityTypes.MeleeSword)
        {
           
            if (playerController.hasMeleeAttackedAtLeastOnce)
            {
                Debug.Log("hey bois");
                // bar fill
                currentActiveAbility.cooldownBar.fillAmount = (Time.time - playerController.meleeAttackAnimStartTime) /
                    (PlayerData.current.meleeAttackCooldown + playerController.meleeAttackAnimation.length);                
            }
        }

        // FIREBALL COOLDOWN BAR
        else if (currentActiveAbility.abilityType == PlayerActiveAbilityTypes.SpellFireball)
        {
            if (playerController.hasCastSpellAtLeastOnce)
            {
                // bar fill
                currentActiveAbility.cooldownBar.fillAmount = (Time.time - playerController.fireballCooldownStartTime) /
                    (PlayerData.current.fireballCastCooldown + playerController.spellcastAnimation.length);

                // bar color changes if insufficient mana
                if (PlayerData.current.currentMana < PlayerData.current.fireballManaCost)
                {
                    currentActiveAbility.cooldownBar.color = new Color(0.613f, 0.362f, 0.362f);
                }
                else
                {
                    currentActiveAbility.cooldownBar.color = Color.white;
                }
            }
        }
    }

}
