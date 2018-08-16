using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SinTreeSkill
{
    public int skillID;
    public string skillName;
    public int skillCost;
    public string skillDescription;
    public SinTreeSkill[] requiredSkills;       
}

// TODO - REMAKE THIS TO DICTIONARY DATA STRUCTURE
public class SinTree
{
    //public List<SinTreeSkill> allSkills;
    public List<SinTreeSkill> activeSkills;

    #region skills
    public SinTreeSkill aspectOfSloth = new SinTreeSkill();
    #endregion

    public SinTree()
    {
        #region SKILL O
        aspectOfSloth.skillName = "Aspect of Sloth";
        aspectOfSloth.skillID = 0;
        aspectOfSloth.skillCost = 1;
        aspectOfSloth.requiredSkills = null;
        aspectOfSloth.skillDescription = "Enemy killed by a melee attack will serve you until its body is destroyed. Can have up to 1 minion alive at a time";
        //allSkills.Add(aspectOfSloth);
        #endregion
    }

    //public void 
}
