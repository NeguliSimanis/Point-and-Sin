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

    // level up
    [Header("Level Up")]
    [SerializeField]
    AudioClip lvUpSFX;
    float lvUPSFXVolume = 0.1f;

    // melee
    [Header("Melee Attack")]
    [SerializeField]
    AudioClip meleeSFX;
    float meleeSFXVolume = 0.2f;

    // wounded
    [Header("Wounded")]
    [SerializeField]
    AudioClip[] playerWoundedSFX;
    float playerWoundedSFXVolume = 0.8F;
    int playerWoundedSFXCount;

    // walking 
    [Header("Walking")]
    [SerializeField]
    bool playWalkSFX;
    [SerializeField]
    AudioClip playerWalkingSFX;
    float playerWalkingSFXVolume = 0.4F;
    public float walkingSFXCooldown = 2.8f;
    public float walkingSFXCooldownResetTime;
    public bool isWalkingSFXCooldown = false;


    private void Start()
    {
        playerWoundedSFXCount = playerWoundedSFX.Length - 1;
    }

    public void PlayWoundedSFX()
    {
        audioSource.PlayOneShot(playerWoundedSFX[Random.Range(0, playerWoundedSFXCount)], playerWoundedSFXVolume);
    }

    public void PlayWalkingSFX()
    {
        if (playWalkSFX)
            audioSource.PlayOneShot(playerWalkingSFX, playerWalkingSFXVolume);
    }

    public void PlayLvUpSFX()
    {
        audioSource.PlayOneShot(lvUpSFX, lvUPSFXVolume);
    }

    public void PlayMeleeSFX()
    {
        audioSource.PlayOneShot(meleeSFX, meleeSFXVolume);
    }
}

