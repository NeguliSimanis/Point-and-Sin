using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Code taken from here: https://www.youtube.com/watch?v=oNz4I0RfsEg
/// </summary>
public class FadeOut : MonoBehaviour
{
    [SerializeField]
    Image imageToFadeOut;
    [SerializeField]
    Text textToFadeOut;

    float fadeSpeed = 0.015f;

    private void OnEnable()
    {
        Debug.Log("HEY");
        StartCoroutine("FadeOutImage");
        StartCoroutine("FadeOutText");
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
