using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXOnTriggerEnter : MonoBehaviour
{
    [SerializeField]
    AudioClip SFXtoPlay;
    [SerializeField]
    float SFXVolume;
    bool hasPlayedSFX = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasPlayedSFX)
            return;
        if (collision.gameObject.tag == "Player")
        {
            hasPlayedSFX = true;
            gameObject.GetComponent<AudioSource>().PlayOneShot(SFXtoPlay, SFXVolume);
        }
    }


}


