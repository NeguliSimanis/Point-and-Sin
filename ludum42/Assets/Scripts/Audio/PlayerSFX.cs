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

    #region walking
    /* Selects a random step sound and plays it when called outside of this script
     * 
     *           */
    [Header("Walking")]
    [SerializeField]
    bool playWalkSFX;

    [SerializeField]
    AudioClip [] playerWalkingSFX;
    float playerWalkingSFXVolume;// = 0.5F;
    [SerializeField]
    float playerWalkingSFXMaxVolume = 0.2f;
    [SerializeField]
    float playerWalkingSFWMinVolume = 0.04f;
    int lastPlayedWalkSFXID = 0;
    int playerWalkSFXCount; // how many walking sfx are in the given array
    #endregion


    private void Start()
    {
        playerWoundedSFXCount = playerWoundedSFX.Length - 1;
        playerWalkSFXCount = playerWalkingSFX.Length - 1;
    }

    public void PlayWoundedSFX()
    {
        audioSource.PlayOneShot(playerWoundedSFX[Random.Range(0, playerWoundedSFXCount)], playerWoundedSFXVolume);
    }


    private int GetRandomWalkingSfxID()
    {
        int tempID = Random.Range(0, playerWalkSFXCount);
        if (playerWalkSFXCount > 0)
        {
            while (tempID == lastPlayedWalkSFXID)
            {
                tempID = Random.Range(0, playerWalkSFXCount);
            }
        }
        lastPlayedWalkSFXID = tempID;
        //Debug.Log("WALLK ID: " + tempID);
        return tempID;
    }

    private float RandomWalkingSFXVolume()
    {
        return Random.Range(playerWalkingSFWMinVolume, playerWalkingSFXMaxVolume);
    }

    public void PlayWalkingSFX()
    {
        if (!playWalkSFX)
            return;

        audioSource.PlayOneShot(playerWalkingSFX[GetRandomWalkingSfxID()], RandomWalkingSFXVolume());
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

