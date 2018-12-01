using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public int slotID;
    public bool isFilled = false;
    public PlayerInventory playerInventory;
    private Item itemInSlot;

    [SerializeField]
    Image itemImage;
    [SerializeField]
    ItemInfoPanel itemInfoPanel;
 

    public void AddItemInSlot(Item itemToAdd)
    {
        isFilled = true;
        itemInSlot = itemToAdd;
        ShowItemImage();
    }

    private void ShowItemImage()
    {
        itemImage.enabled = true;
        itemImage.sprite = itemInSlot.itemImage.sprite;
    }

    private void OnMouseOver()
    {
        Debug.Log("hey");
        if (isFilled)
            itemInfoPanel.DisplayItemInfo();
    }

    private void OnMouseExit()
    {
        if (isFilled)
            itemInfoPanel.DisplayItemInfo(false);
    }
}
