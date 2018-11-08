using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays player character sfx when called from PlayerController.cs
/// TODO: move all player sounds to here
/// </summary>
public class PlayerSFX : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    AudioClip[] playerWoundedSFX;
    float playerWoundedSFXVolume = 0.8F;
    int playerWoundedSFXCount;


    private void Start()
    {
        playerWoundedSFXCount = playerWoundedSFX.Length - 1;
    }

    public void PlayWoundedSFX()
    {
        audioSource.PlayOneShot(playerWoundedSFX[Random.Range(0, playerWoundedSFXCount)], playerWoundedSFXVolume);
    }

}
