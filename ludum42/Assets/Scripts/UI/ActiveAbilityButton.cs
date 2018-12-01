using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveAbilityButton : MonoBehaviour
{
    [SerializeField]
    PlayerActiveAbilityManager playerActiveAbilityManager;

    /// <summary>
    /// <para> ActiveSkillDefault - click to open inactive skill selection; </para> 
    /// <para> ActiveSkillOpen - click to close inactive skill selection and return to Active Skill Default; </para> 
    /// <para> InactiveSkillOpen - click on this to set this button as ActiveSkillDefault; </para> 
    /// <para> Other - default state </para> 
    /// </summary>
    public enum ButtonState
    {
        ActiveSkillDefault,
        ActiveSkillOpen,    
        InactiveSkillOpen,  
        Other               
    };

    public ButtonState currentState;
    private Button abilityButton;

	void Start ()
    {
        //currentState = ButtonState.Other;
        abilityButton = gameObject.GetComponent<Button>();
        abilityButton.onClick.AddListener(ProcessClickInput);
    }
	
    void ProcessClickInput()
    {
        // open inactive skill selection
        if (currentState == ButtonState.ActiveSkillDefault)
        {
            playerActiveAbilityManager.DisplayAvailableAbilityIcons();
            currentState = ButtonState.ActiveSkillOpen;
        }

        // close inactive skill selection and return to Active Skill Default
        else if (currentState == ButtonState.ActiveSkillOpen)
        {
            playerActiveAbilityManager.DisplayAvailableAbilityIcons(false);
            currentState = ButtonState.ActiveSkillDefault;
        }

        // set this button as ActiveSkillDefault
        else if (currentState == ButtonState.InactiveSkillOpen)
        {
            playerActiveAbilityManager.SetCurrentActiveAbility(transform.parent.gameObject);
            playerActiveAbilityManager.DisplayAvailableAbilityIcons(false, true);
            currentState = ButtonState.ActiveSkillDefault;
        }
    }

    public void ReactToOtherButtonClick(ButtonState otherButtonState)
    {
        // enable this game object
        if (otherButtonState == ButtonState.ActiveSkillDefault)
        {
            gameObject.SetActive(true);
        }

        // disable this game object
        else if (otherButtonState == ButtonState.ActiveSkillOpen)
        {
            playerActiveAbilityManager.HideInactiveAbilityIcons();
        }

        // set this button as Other state, disable game object
        else if (otherButtonState == ButtonState.InactiveSkillOpen)
        {
            currentState = ButtonState.Other;
            gameObject.SetActive(false);
        }
    }

}
