using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUIText : MonoBehaviour
{

    string[] uiTexts =
    {
        "THOU MUST OBTAIN THE BLOOD OF THE VIRGIN \n\n'TIS GUARDED BY THE SKULL CLERIC \n\nDEATH SHALL NOT THWART THEE",
        "ABANDON HOPE ALL YE WHO ENTER HERE"
    };

	public void SetText(int textID)
    {
        gameObject.GetComponent<Text>().text = uiTexts[textID];
    }

}
