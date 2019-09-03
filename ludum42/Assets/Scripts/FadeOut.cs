using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fades out selected UI image, text, or sprite upon enabling
/// 
/// Some noted usage:
///     - fading out enemy shadows upon death
/// 
/// Code taken from here: https://www.youtube.com/watch?v=oNz4I0RfsEg
/// </summary>
public class FadeOut : MonoBehaviour
{
    [SerializeField]
    bool fadeOutOnEnable = true;
    [SerializeField]
    Image imageToFadeOut;
    [SerializeField]
    Text textToFadeOut;
    [SerializeField]
    SpriteRenderer[] spritesToFadeOut;
    SpriteRenderer spriteToFadeOut;

    float fadeSpeed = 0.015f;
    [SerializeField]
    float spriteFadeSpeed = 0.2f;
    [SerializeField]
    float spriteFadeInterval = 1f;

    private void OnEnable()
    {
        if (fadeOutOnEnable)
        {
            StartCoroutine("FadeOutImage");
            StartCoroutine("FadeOutText");
        }
        
    }

    public void FadeSprite()
    {
        // find which of the given sprites is active and fade it out
        for (int i = 0; i < spritesToFadeOut.Length; i++)
        {
            if(spritesToFadeOut[i].gameObject.activeInHierarchy)
            {
                Debug.Log("sprite found!");
                spriteToFadeOut = spritesToFadeOut[i];
            }
        }
        Debug.Log("called!");
        StartCoroutine("FadeOutSprite");
    }
    
    IEnumerator FadeOutSprite()
    {
        Debug.Log("corotine1!");
        yield return new WaitForSeconds(spriteFadeInterval);
        Debug.Log("corotine!" + gameObject.name);
        for (float f = 1f; f >= -0.05f; f -= spriteFadeSpeed)
        {
            Color c = spriteToFadeOut.color;
            c.a = f;
            spriteToFadeOut.color = c;
            yield return new WaitForSeconds(0.07f);
        }
    }

    IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(3f);
        for (float f = 1f; f >= -0.05f; f -= fadeSpeed)
        {
            Color c = textToFadeOut.color;
            c.a = f;
            textToFadeOut.color = c;
            yield return new WaitForSeconds(0.07f);
        }
    }

    IEnumerator FadeOutImage()
    {
        yield return new WaitForSeconds(3f);
        for (float f = 1f; f >= -0.05f; f-= fadeSpeed)
        {
            Color c = imageToFadeOut.color;
            c.a = f;
            imageToFadeOut.color = c;
            yield return new WaitForSeconds(0.07f);
        }
        this.gameObject.SetActive(false);
    }
}
