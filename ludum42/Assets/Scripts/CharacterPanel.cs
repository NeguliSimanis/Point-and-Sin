using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour {

    [SerializeField] Text wrathPointsText;
    [SerializeField] Text pridePointsText;
    [SerializeField] Text lustPointsText;
    [SerializeField] Text slothPointsText;

    [SerializeField] Button addWrathButton;
    [SerializeField] Button addPrideButton;
    [SerializeField] Button addLustButton;
    [SerializeField] Button addSlothButton;

    [SerializeField] Text currentLVText;
    [SerializeField] Text currentSinPointsText;

    void Start ()
    {
        UpdateSinPointsText();
	}

    public void AddWrath()
    {
        PlayerData.current.AddWrath(1);
        PlayerData.current.sinPoints--;
        UpdateSinPointsText();
    }

    public void AddPride()
    {
        PlayerData.current.AddPride(1);
        PlayerData.current.sinPoints--;
        UpdateSinPointsText();
    }

    public void AddLust()
    {
        PlayerData.current.AddLust(1);
        PlayerData.current.sinPoints--;
        UpdateSinPointsText();
    }

    public void AddSloth()
    {
        PlayerData.current.AddSloth(1);
        PlayerData.current.sinPoints--;
        UpdateSinPointsText();
    }
    private void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        UpdateSkillButtons();
        UpdateCurrentLVAndSinPointText();
    }

    void UpdateSkillButtons()
    {
        if (PlayerData.current.sinPoints > 0)
        {
            addWrathButton.gameObject.SetActive(true);
            addPrideButton.gameObject.SetActive(true);
            addLustButton.gameObject.SetActive(true);
            addSlothButton.gameObject.SetActive(true);
        }
        else
        {
            addWrathButton.gameObject.SetActive(false);
            addPrideButton.gameObject.SetActive(false);
            addLustButton.gameObject.SetActive(false);
            addSlothButton.gameObject.SetActive(false);
        }
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
    void UpdateSinPointsText()
    {
        wrathPointsText.text = PlayerData.current.wrath.ToString();
        pridePointsText.text = PlayerData.current.pride.ToString();
        lustPointsText.text = PlayerData.current.lust.ToString();
        slothPointsText.text = PlayerData.current.sloth.ToString();
    }
}
