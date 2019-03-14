using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnspentSkillpointCheck
{
    public void ShowUIIfEnoughSkillPoints(GameObject notificationUI)
    {
        // update skill points notification
        if (PlayerData.current.sinPoints > 0) // || PlayerData.current.sinTreePoints > 0) 
        {
            notificationUI.SetActive(true);
        }
        else
        {
            notificationUI.SetActive(false);
        }
    }
}
