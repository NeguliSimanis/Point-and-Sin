using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame// : MonoBehaviour
{
    public static PauseGame current;


    public void Pause(GameObject pauseMenu)
    {
        pauseMenu.SetActive(true);
        PlayerData.current.isGamePaused = !PlayerData.current.isGamePaused;
    }

    private void CreatePlayerData()
    {
        if (PlayerData.current == null)
            PlayerData.current = new PlayerData();
    }
}
