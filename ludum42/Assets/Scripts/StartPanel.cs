using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : MonoBehaviour
{


    void Start()
    {
        if (PlayerData.current == null)
            PlayerData.current = new PlayerData();
        PauseGame(true);
    }

    public void PauseGame(bool pause)
    {
        PlayerData.current.Pause(pause);
    }
}
