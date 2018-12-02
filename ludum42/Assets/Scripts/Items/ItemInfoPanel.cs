using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    [SerializeField]
    Text itemName;
    [SerializeField]
    Text itemStats;
    [SerializeField]
    Text itemType;
    [SerializeField]
    Image itemImage;

	public void DisplayItemInfo(Item itemToDisplay, bool display = true)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(display);
        }
        if (display)
        {
            itemName.text = itemToDisplay.itemName;
            itemImage.sprite = itemToDisplay.itemImage.sprite;
            itemType.text = itemToDisplay.GetItemTypeString();
            itemStats.text = itemToDisplay.effectDescription;
            //Debug.Log("displaying item info");

        }
        else
        {
            //Debug.Log("hiding item info");
        }
    }
}
