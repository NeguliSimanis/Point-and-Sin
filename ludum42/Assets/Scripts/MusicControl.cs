using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour
{
    bool isPlayingBackgroundMusic = false;
    [SerializeField] AudioSource audioSource;

	void Update ()
    {
        if (!isPlayingBackgroundMusic && PlayerData.current.canPlayBackground)
        {
            Debug.Log("dis workd");
            isPlayingBackgroundMusic = true;
            PlayBackgroundMusic();
        }
	}

    public void PlayBackgroundMusic()
    {
        audioSource.enabled = true;
    }
}
