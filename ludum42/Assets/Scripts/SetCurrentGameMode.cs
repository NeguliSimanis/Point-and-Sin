using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCurrentGameMode : MonoBehaviour
{

	public void SetBrutalMode()
    {
        PlayerData.current.isPlayingBrutalMode = true;
    }

    public void SetDefaultMode()
    {
        PlayerData.current.isPlayingBrutalMode = false;
    }
}
