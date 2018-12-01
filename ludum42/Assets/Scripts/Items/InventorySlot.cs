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
        if (isFilled)
            itemInfoPanel.DisplayItemInfo(itemInSlot);
    }

    private void OnMouseExit()
    {
        if (isFilled)
            itemInfoPanel.DisplayItemInfo(itemInSlot,false);
    }
}
