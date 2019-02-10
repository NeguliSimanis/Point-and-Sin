using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenManager : MonoBehaviour
{

    [SerializeField]
    GameObject mainButtonPanel;

    #region default UI properties
    [SerializeField]
    Text defaultGameModeText;
    #endregion

    #region brutal UI properties
    // UI after brutal mode is unlocked
    [SerializeField]
    GameObject brutalModeButton;

    float brutalButtonSpacing = 15f;
    float brutalButtonHeight = 45; // resize from 54 to 45
    
    // text displayed on normal mode button when brutal mode is unlocked
    string defaultGameModeTextB = "NORMAL";
    #endregion

    private void Start()
    {
        // set up player data
        if (PlayerData.current == null)
            PlayerData.current = new PlayerData();

        CheckIfBrutalModeEnabled();
    }
	
	void CheckIfBrutalModeEnabled()
    {
        if (PlayerData.current.isBrutalUnlocked)
        {
            SetupBrutalUILayout();
        }
    }

    void SetupBrutalUILayout()
    {
        brutalModeButton.SetActive(true);
        defaultGameModeText.text = defaultGameModeTextB;
        SetMainButtonPanelSpacing();
        ResizeMainButtonPanelButtons();
        //mainButtonDefaultLayout.position = mainButtonBrutalLayout.position;
    }

    void SetMainButtonPanelSpacing()
    {
        mainButtonPanel.GetComponent<VerticalLayoutGroup>().spacing = brutalButtonSpacing;
    }

    /// <summary>
    /// Resizes the buttons so that they would fit nicely in the screen once brutal mode is unlocked
    /// </summary>
    void ResizeMainButtonPanelButtons()
    {
        foreach (Transform panelButton in mainButtonPanel.transform)
        {
            RectTransform currentRT = panelButton.gameObject.GetComponent<RectTransform>();
            currentRT.sizeDelta = new Vector2(currentRT.sizeDelta.x, brutalButtonHeight);
        }
    }
}
