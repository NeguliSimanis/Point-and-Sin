using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenManager : MonoBehaviour
{
    // default UI
    [SerializeField]
    RectTransform mainButtonDefaultLayout;
    [SerializeField]
    Text defaultGameModeText;

    // UI after brutal mode is unlocked
    [SerializeField]
    GameObject brutalModeButton;
    [SerializeField]
    RectTransform mainButtonBrutalLayout;

    string defaultGameModeTextB = "NORMAL";

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
        mainButtonDefaultLayout.position = mainButtonBrutalLayout.position;
    }
}
