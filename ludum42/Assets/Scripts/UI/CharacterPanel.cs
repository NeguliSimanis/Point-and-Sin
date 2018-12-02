using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour {

    bool isSinTreePanelActive = false;
    UnspentSkillpointCheck unspentSkillpointCheck;
    [SerializeField] GameObject skillPointNotification;

    [SerializeField] Text currentLVText;
    [SerializeField] Text currentSinPointsText;

    [Header("Sins")]
    [SerializeField] Text wrathPointsText;
    [SerializeField] Text pridePointsText;
    [SerializeField] Text lustPointsText;
    [SerializeField] Text slothPointsText;

    [SerializeField] Button addWrathButton;
    [SerializeField] Button addPrideButton;
    [SerializeField] Button addLustButton;
    [SerializeField] Button addSlothButton;

    #region SIN TREE
    int activeSkillID = 0;
    SinTreeSkill activeSinTreeSkill;

    [Header("Sin Tree")]
    [SerializeField] Text skillMaxLVIndicator; // e.g. 1/3
    [SerializeField] Text sinTreeSkillCost;
    [SerializeField] Text sinTreePointsText;

    [SerializeField] Text currentLVEffect;
   // [SerializeField] Text nextLVEffect;
    [SerializeField] Button upgradeSinSkillButton;
    #endregion

    void Start ()
    {
        // SINS
        UpdateSinPointsText();

        // TREE
        unspentSkillpointCheck = new UnspentSkillpointCheck();
        DisplaySinTreeSkill(0);
        UpdateSinTree();

        Debug.Log("Level " + activeSinTreeSkill.skillLV);
        Debug.Log("Cost " + activeSinTreeSkill.skillCost);
    }

    public void UpdateSinTree()
    {
        ShowCurrentSouls();
        UpdateSinTreeUpgradeButton();
        ShowActiveSkillLVEffect();
        ShowActiveSkillCost();
        ShowActiveSkillLV();
    }

    private void ShowActiveSkillLVEffect()
    {
        if (activeSinTreeSkill.skillLV == 0)
        {
            currentLVEffect.text = "Next level: " + activeSinTreeSkill.skillLevelEffect[activeSinTreeSkill.skillLV];
        }
        else if (activeSinTreeSkill.skillLV == activeSinTreeSkill.skillMaxLV)
        {
            currentLVEffect.text = activeSinTreeSkill.skillLevelEffect[activeSinTreeSkill.skillLV - 1] + " (max)";
        }
        else
        {
            currentLVEffect.text = "Current level: " + activeSinTreeSkill.skillLevelEffect[activeSinTreeSkill.skillLV - 1]
                + "\n\n" +
                "Next level: " + activeSinTreeSkill.skillLevelEffect[activeSinTreeSkill.skillLV];
        }
    }

    public void ShowCurrentSouls()
    {
        // how many sin tree points player has
        sinTreePointsText.text = PlayerData.current.sinTreePoints.ToString() + " souls";
    } 

    public void DisplaySinTreeSkill(int skillID)
    {
        activeSkillID = skillID;
        activeSinTreeSkill = SinTree.current.GetSkillInfo(skillID);
    }

    public void UpgradeSinTreeSkill()
    {
        Debug.Log("upgrading " + SinTree.allSkills[activeSkillID].skillName);
        activeSinTreeSkill = SinTree.current.UpgradeSinTreeSkill(activeSkillID);


        UpdateSinTree();
    }

    private void UpdateSinTreeUpgradeButton()
    {
        // disable upgrade button if insufficient number of points
        // disable upgrade button if max skill level reached
        if (PlayerData.current.sinTreePoints < activeSinTreeSkill.skillCost) // || activesSinTreeSkill.skillMaxLV == activesSinTreeSkill.skillLV
        {
            upgradeSinSkillButton.interactable = false;
            skillPointNotification.SetActive(false);
        }
    }

    private void ShowActiveSkillCost()
    {
        //sinTreeSkillCost.text = SinTree.current.allSkills[activeSkillID].skillCost.ToString() + " souls";
        sinTreeSkillCost.text = activeSinTreeSkill.skillCost.ToString() + " souls";
    }

    private void ShowActiveSkillLV()
    {
        skillMaxLVIndicator.text = activeSinTreeSkill.skillLV.ToString() 
            + "/" + activeSinTreeSkill.skillMaxLV.ToString();
    }

    public void ShowSinTree(bool showTree)
    {
        isSinTreePanelActive = showTree;
    }

    #region Increasing sins
    public void AddWrath()
    {
        PlayerData.current.AddWrath(1);
        SpendSinPoints();
    }

    public void AddPride()
    {
        PlayerData.current.AddPride(1);
        SpendSinPoints();
    }

    public void AddLust()
    {
        PlayerData.current.AddLust(1);
        SpendSinPoints();
    }

    public void AddSloth()
    {
        PlayerData.current.AddSloth(1);
        SpendSinPoints();
    }
    public void SpendSinPoints()
    {
        PlayerData.current.sinPoints--;
        UpdateSinPointsText();
        unspentSkillpointCheck.ShowUIIfEnoughSkillPoints(skillPointNotification);
    }
    #endregion

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        if (!isSinTreePanelActive)
        {
            UpdateSkillButtons();
            UpdateCurrentLVAndSinPointText();
        }
    }

    void UpdateSkillButtons()
    {
        if (PlayerData.current.sinPoints > 0)
        {
            ActivateSkillButtons(true);
        }
        else
        {
            ActivateSkillButtons(false);
        }
    }

    void ActivateSkillButtons(bool hasSkillPoints)
    {
        addWrathButton.gameObject.SetActive(hasSkillPoints);
        addPrideButton.gameObject.SetActive(hasSkillPoints);
        addLustButton.gameObject.SetActive(hasSkillPoints);
        addSlothButton.gameObject.SetActive(hasSkillPoints);
    }

    void UpdateCurrentLVAndSinPointText()
    {
        if (PlayerData.current.sinPoints > 0)
        {
            currentLVText.text = "LV " + PlayerData.current.currentLevel;
            currentSinPointsText.gameObject.SetActive(true);
            currentSinPointsText.text = PlayerData.current.sinPoints + " sin points";
        }
        else
        {
            currentSinPointsText.gameObject.SetActive(false);
        }
    }
    public void UpdateSinPointsText()
    {
        wrathPointsText.text = PlayerData.current.wrath.ToString();
        pridePointsText.text = PlayerData.current.pride.ToString();
        lustPointsText.text = PlayerData.current.lust.ToString();
        slothPointsText.text = PlayerData.current.sloth.ToString();
    }
}
