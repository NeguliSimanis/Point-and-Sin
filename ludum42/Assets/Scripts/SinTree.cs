using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinTreeSkill
{
    public int skillID;
    public int skillLV;
    public int skillMaxLV;
    public string skillName;
    public int skillCost;
    public string skillDescription;
    public List<string> skillLevelEffect;
    public SinTreeSkill[] requiredSkills;       
}


public class SinTree
{
    public static SinTree current = new SinTree();

    //public List<SinTreeSkill> allSkills;
    public static List<SinTreeSkill> activeSkills;
    public static List<SinTreeSkill> allSkills;

    #region skills
    public SinTreeSkill sinSkill0;
    #endregion

    public SinTree()
    {
        activeSkills = new List<SinTreeSkill>();
        allSkills = new List<SinTreeSkill>();
        sinSkill0 = new SinTreeSkill();

        #region SKILL O
        sinSkill0.skillName = "Aspect of Astaroth";
        sinSkill0.skillDescription = "Resurrect enemy killed by melee attack as your minion.";
        sinSkill0.skillLevelEffect = new List<string>();
        sinSkill0.skillLevelEffect.Add("up to 1 minion alive at a time");
        sinSkill0.skillLevelEffect.Add("up to 2 minions alive at a time");
        sinSkill0.skillLevelEffect.Add("up to 3 minions alive at a time");

        sinSkill0.skillID = 0;
        sinSkill0.skillLV = 0;
        sinSkill0.skillMaxLV = 3;
        sinSkill0.skillCost = 1;
        sinSkill0.requiredSkills = null;
        allSkills.Add(sinSkill0); 
        #endregion
    }

    public SinTreeSkill UpgradeSinTreeSkill(int skillID)
    {
        // spend resources
        PlayerData.current.sinTreePoints -= allSkills[skillID].skillCost;

        // update skill data
        allSkills[skillID].skillCost += 1;
        allSkills[skillID].skillLV += 1;

        if (allSkills[skillID].skillID == 0)
        {
            PlayerData.current.summonMinionOnMeleeKill = true;
            PlayerData.current.maxMinions++;
            Debug.Log("max minions " + PlayerData.current.maxMinions);
        }
       
        return allSkills[skillID];
        /*
        SinTreeSkill skillToUpgrade = allSkills[skillID];

        // spend resources
        PlayerData.current.sinTreePoints -= skillToUpgrade.skillCost;

        // update skill data
        skillToUpgrade.skillCost += 2;
        skillToUpgrade.skillLV += 1;

        if (skillToUpgrade.skillID == 0)
        {
            PlayerData.current.summonMinionOnMeleeKill = true;
            PlayerData.current.maxMinions++;
        }
        Debug.Log("Sin tree cost " + skillToUpgrade.skillCost);
        Debug.Log("Sin tree level " + skillToUpgrade.skillLV);
        return skillToUpgrade;*/
    }

    public SinTreeSkill GetSkillInfo(int skillID)
    {
        return allSkills[skillID];
    }
    //public void 
}
