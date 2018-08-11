using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public void Load()
    {
        Debug.Log("called");
        PlayerData.current.Reset();
        Debug.Log(PlayerData.current.isGamePaused);
        SceneManager.LoadScene("SampleScene");
    }
}
