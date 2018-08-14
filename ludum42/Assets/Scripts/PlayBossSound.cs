using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBossSound : MonoBehaviour
{

    [SerializeField]
    AudioClip bossWarningSFX;
    [SerializeField]
    float bossWarningSFXVolume;
    bool hasPlayedBossSFX = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasPlayedBossSFX)
            return;
        if (collision.gameObject.tag == "Player")
        {
            hasPlayedBossSFX = true;
            gameObject.GetComponent<AudioSource>().PlayOneShot(bossWarningSFX, bossWarningSFXVolume);
        }
    }


}
