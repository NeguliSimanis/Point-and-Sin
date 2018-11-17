using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSounds : MonoBehaviour
{
    [SerializeField]
    AudioClip lavaSFX;
    AudioSource audioSource;
    float lavaSFXCooldown = 4f;
    float lavaSFXVolume = 0.3f;
    bool isNearLava = false;
    bool isLavaSFXEnabled = false;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void EnableLavaSFX()
    {
        isNearLava = true;
    }

    public void DisableLavaSFX()
    {
        isNearLava = false;
    }
	

	void Update ()
    {
        if (!isNearLava)
        {
            isLavaSFXEnabled = false;
            return;
        }

        if (!isLavaSFXEnabled)
        {
            isLavaSFXEnabled = true;
            StartCoroutine(PlayLavaSFX());
        }  
	}

    private IEnumerator PlayLavaSFX()
    {
        while (isNearLava)
        {
            audioSource.PlayOneShot(lavaSFX, lavaSFXVolume);
            yield return new WaitForSeconds(lavaSFX.length + lavaSFXCooldown);
        }
    }
}
