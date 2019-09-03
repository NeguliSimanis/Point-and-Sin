using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fades in selected UI image, text upon enabling
/// </summary>
public class FadeIn : MonoBehaviour
{
    bool skipImageFade = false;
    bool skipTextFade = false;

    [SerializeField]
    bool fadeInOnEnable = true;
    [SerializeField]
    Image[] imagesToFadeIn;
    [SerializeField]
    Text[] textsToFadeIn;

    float fadeSpeed = 0.03f;

    private void OnEnable()
    {
        if (fadeInOnEnable & !skipTextFade & !skipTextFade)
        {
            SetImagesToTransparent();
            SetTextsToTransparent();
            StartCoroutine("FadeInImage");
            StartCoroutine("FadeInText");
        }
    }
    
    public void SkipFade(bool skipText, bool skipImage)
    {
        if (skipText)
        {
            SetTextsToTransparent(false);
            skipTextFade = true;
        }
        if (skipImage)
        {
            SetImagesToTransparent(false);
            skipImageFade = true;
        }
    }

    void SetImagesToTransparent(bool setToTransparent = true)
    {
        if (setToTransparent)
        {
            for (int i = 0; i < imagesToFadeIn.Length; i++)
            {
                Color c = imagesToFadeIn[i].color;
                c.a = 0;
                imagesToFadeIn[i].color = c;
            }
        }
        else
        {
            for (int i = 0; i < imagesToFadeIn.Length; i++)
            {
                Color c = imagesToFadeIn[i].color;
                c.a = 1;
                imagesToFadeIn[i].color = c;
            }
        }
    }


    void SetTextsToTransparent(bool setToTransparent = true)
    {
        if (setToTransparent)
        {
            for (int i = 0; i < textsToFadeIn.Length; i++)
            {
                Color c = textsToFadeIn[i].color;
                c.a = 0;
                textsToFadeIn[i].color = c;
            }
        }
        else
        {
            for (int i = 0; i < textsToFadeIn.Length; i++)
            {
                Color c = textsToFadeIn[i].color;
                c.a = 1;
                textsToFadeIn[i].color = c;
            }
        }
    }

    IEnumerator FadeInText()
    {
        yield return new WaitForSeconds(0.2f);
        if (!skipImageFade)
        {
            for (float f = 0f; f <= 1f; f += fadeSpeed)
            {
                for (int i = 0; i < textsToFadeIn.Length; i++)
                {
                    Color c = textsToFadeIn[i].color;
                    c.a = f;
                    textsToFadeIn[i].color = c;
                }
                yield return new WaitForSeconds(0.07f);
            }
        }
        else
        {
            SetImagesToTransparent(false);
        }
    }

    IEnumerator FadeInImage()
    {
        yield return new WaitForSeconds(0.2f);
        if (!skipImageFade)
        {
            for (float f = 0f; f <= 1f; f += fadeSpeed)
            {
                for (int i = 0; i < imagesToFadeIn.Length; i++)
                {
                    Color c = imagesToFadeIn[i].color;
                    c.a = f;
                    imagesToFadeIn[i].color = c;
                }
                yield return new WaitForSeconds(0.07f);
            }
        }
        else
        {
            SetTextsToTransparent(false);
        }
    }

}
