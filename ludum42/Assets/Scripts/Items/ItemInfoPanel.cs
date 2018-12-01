using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    [SerializeField]
    Text itemName;

	public void DisplayItemInfo(Item itemToDisplay, bool display = true)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(display);
        }
        if (display)
        {
            itemName.text = itemToDisplay.itemName;
            //Debug.Log("displaying item info");
           
        }
        else
        {
            //Debug.Log("hiding item info");
        }
    }
}
