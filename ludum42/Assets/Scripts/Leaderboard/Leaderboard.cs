using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use GlobalstatsIO to store/fetch player stats from a leaderboard.
/// 
/// API: https://github.com/globalstats-io/unity-library
/// </summary>
public class Leaderboard : MonoBehaviour
{
    GlobalstatsIO gs = new GlobalstatsIO();

    string user_name = "AlusTest1";
    Dictionary<string, string> playerStats = new Dictionary<string, string>();

    public void SumbmitStats()
    {
        playerStats.Add("expNorm", PlayerData.current.currentExp.ToString());

        if (gs.share("", user_name, playerStats))
        {
            // Success
            Debug.Log("success");
        }
        else
        {
            // An Error occured
            Debug.Log("fail");
        }
    }
}
