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
    [SerializeField]
    GameObject itemEquippedLabel;
    [SerializeField]
    GameObject itemNotEquippedLabel;

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

            if (itemToDisplay.currentState == ItemState.Equipped)
            {
                //Debug.Log("item is equipped");
                itemEquippedLabel.SetActive(true);
                itemNotEquippedLabel.SetActive(false);
            }
            else
            {
                //Debug.Log("item is NOT equipped");
                itemEquippedLabel.SetActive(false);
                itemNotEquippedLabel.SetActive(true);
            }
        }
        else
        {
            //Debug.Log("hiding item info");
        }
    }
}
