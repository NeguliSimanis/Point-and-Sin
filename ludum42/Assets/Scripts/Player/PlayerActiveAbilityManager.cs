using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveAbility
{
    public PlayerActiveAbilityTypes abilityType;
    public GameObject icon;
    public ActiveAbilityButton uiButton;
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
    ActiveAbilityButton[] activeAbilityButtons;
    [SerializeField]
    Image[] activeAbilityCooldownBars; // must be in the same order as ActiveAbilityIcons
    [SerializeField]
    Transform activeAbilityPosition;
    [SerializeField]
    Transform[] inactiveAbilityPositions;          
    
    /// <summary>
    /// e.g. fireball, melee - 
    /// Contains a list of objects with info about ability type and UI icon
    /// </summary>
    List <ActiveAbility> activeAbilities;// = new ActiveAbility[2];
    public ActiveAbility currentActiveAbility;

    int numberOfAbilities;
    int numberOfInactiveAbilities; // numberOfAbilities - 1
    int activeAbilityID = 0;
    PlayerController playerController;

    private void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();

        numberOfAbilities = activeAbilityIcons.Length;
        numberOfInactiveAbilities = numberOfAbilities - 1;

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
            activeAbilities[i].uiButton = activeAbilityButtons[i];

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

    public void SetCurrentActiveAbility(GameObject activeAbilityIcon)
    {
        for (int i = 0; i < numberOfAbilities; i++)
        {
            if (activeAbilities[i].icon == activeAbilityIcon)
            {
                currentActiveAbility = activeAbilities[i];
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newActiveAbilityID">If -1 - ability is switched via mouse wheel;
    /// else - it was selected via UI button and must switch ability with this ID</param>
    /// <param name="selectNextSkill">If true - mousewheel was scrolled down and will select next skill in list;
    /// else - select previous item in list </param>
    public void SwitchActiveAbility(int newActiveAbilityID = -1, bool selectNextSkill = true)
    {
        // player used mouse wheel to switch between skills
        if (newActiveAbilityID == -1)
        {
            // mouse wheek down
            if (selectNextSkill)
            {
                activeAbilityID++;

                // end of list reached - select first ability in list
                if (activeAbilityID >= numberOfAbilities)
                {
                    activeAbilityID = 0;
                }
            }
            // mouse wheel up
            else
            {
                activeAbilityID--;

                // beginning of list reached - select last ability in list
                if (activeAbilityID < 0)
                {
                    activeAbilityID = numberOfAbilities-1;
                }
            }
            currentActiveAbility = activeAbilities[activeAbilityID];
            //Debug.Log("ABILITY " + activeAbilities[activeAbilityID].activeAbilityType + " selected");
        }
        //if (activeAbilities)
        ShowActiveAbilityIcon();
        HideInactiveAbilityIcons();
    }

    /// <summary>
    /// Activates the active ability icon object and moves it to the correct position
    /// </summary>
    private void ShowActiveAbilityIcon()
    {
        currentActiveAbility.icon.gameObject.SetActive(true);
        currentActiveAbility.icon.gameObject.transform.position = activeAbilityPosition.position;
        currentActiveAbility.uiButton.currentState = ActiveAbilityButton.ButtonState.ActiveSkillDefault;
    }

    /// <summary>
    /// goes through the list of all ability icons and hides the ones that are not currently active
    /// <para> AND </para> 
    /// <para> moves the inactive icons to the relevant inactive positions </para> 
    /// </summary>
    public void HideInactiveAbilityIcons()
    {
        for (int i = 0; i < numberOfAbilities; i++)
        {
            if (activeAbilities[i] != currentActiveAbility)
            {
                // SET INACTIVE ICON POSITION
                SetInactiveAbilityPosition(i,i);

                // DISABLE INACTIVE ABILITY ICON GAME OBJECT
                activeAbilities[i].icon.gameObject.SetActive(false);

                // UPDATE ICON BUTTON STATE
                activeAbilities[i].uiButton.currentState = ActiveAbilityButton.ButtonState.Other;
            }
        }
    }

    /// <summary>
    /// Displays or hides the selection of active skills
    ///  <para> AND </para> 
    ///  <para> repositions ability icons to relevant positions if necessary </para> 
    /// </summary>
    /// <param name="display">if false then hide</param>
    /// <param name="repositionIcons">true if active ability has just been changed</param>
    public void DisplayAvailableAbilityIcons(bool display = true, bool repositionIcons = false)
    {
        for (int i = 0; i < numberOfAbilities; i++)
        {
            // displays or hides available abilities
            if (activeAbilities[i] != currentActiveAbility)
            {
                DisplayAbilityIcon(i, display);
            }

            // also repositions available abilities
            if (repositionIcons)
            {
                SetInactiveAbilityPosition(i, i);  
            }
        }
        // sets position of active skill icon
        if (repositionIcons)
        {
            currentActiveAbility.icon.gameObject.transform.position = activeAbilityPosition.position;
        }
    }

    /// <summary>
    /// displays or hides the ability icon and updates the child button state
    /// </summary>
    /// <param name="abilityID"></param>
    /// <param name="display"></param>
    private void DisplayAbilityIcon(int abilityID, bool display)
    {
        activeAbilities[abilityID].icon.gameObject.SetActive(display);
        // updates button state if it becomes hidden
        if (!display)
        {
            activeAbilities[abilityID].uiButton.currentState = ActiveAbilityButton.ButtonState.Other;
        }
        else
        {
            activeAbilities[abilityID].uiButton.currentState = ActiveAbilityButton.ButtonState.InactiveSkillOpen;
        }
    }

    private void SetInactiveAbilityPosition(int abilityID, int positionID)
    {
        positionID = abilityID;
        if (positionID > numberOfInactiveAbilities - 1)
        {
            positionID = numberOfInactiveAbilities - 1;
        }
        activeAbilities[abilityID].icon.gameObject.transform.position = inactiveAbilityPositions[positionID].position;
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
