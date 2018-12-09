using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUIText : MonoBehaviour
{
    [SerializeField]
    Text itemFlavorTextUI;

    string[] uiTexts =
    {
        "THOU MUST OBTAIN THE BLOOD OF THE VIRGIN \n\n'TIS GUARDED BY THE SKULL CLERIC \n\nDEATH SHALL NOT THWART THEE",
        "ABANDON HOPE ALL YE WHO ENTER HERE"
    };

	public void SetIntroText(int textID)
    {
        gameObject.GetComponent<Text>().text = uiTexts[textID];
    }

    public void SetItemFlavorText(string itemFlavorText)
    {
        itemFlavorTextUI.text = itemFlavorText;
    }
}
