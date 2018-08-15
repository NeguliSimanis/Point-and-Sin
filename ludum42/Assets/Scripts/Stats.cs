using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays player stats on UI text
/// </summary>

public class Stats : MonoBehaviour

{
    Text statsText;

    private void Start()
    {
        GetTextComponent();    
    }

    void GetTextComponent()
    {
        statsText = gameObject.GetComponent<Text>();
    }

    void OnEnable()
    {
        if (statsText == null)
            GetTextComponent();
        DisplayStats();
    }

    private void DisplayStats()
    {
        // declare time played variables
        string totalTimeString;
        int totalMinutes;
        int seconds;
        string totalMinutesString;
        string secondsString;

        // calculate total minutes played
        totalMinutes = (int)Mathf.Floor(PlayerData.current.playTime / 60f);
        totalMinutesString = totalMinutes.ToString() + " minutes";

        // calculate remaining seconds
        seconds = (int)PlayerData.current.playTime - 60 * totalMinutes;
        secondsString = seconds + " seconds";

        // concat time string
        totalTimeString = "\n TIME: " + totalMinutesString + " " +secondsString;

        // concat stats string
        statsText.text = "LEVEL: " + PlayerData.current.currentLevel.ToString()
                + "\n SOULS PURGED: " + PlayerData.current.enemiesKilled.ToString()
                + totalTimeString;
    }
}
