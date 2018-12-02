using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventorySlot : MonoBehaviour
{

    public bool isBackpackSlot = true; 

    public int slotID;
    public bool isFilled = false;
    public PlayerInventory playerInventory;
    public Item itemInSlot;

    [SerializeField]
    Image itemImage;
    [SerializeField]
    ItemInfoPanel itemInfoPanel;

    #region HIGHLIGHTING
    Color highlightColor = Color.yellow;
    Color defaultColor;
    Image slotBackgroundImage;
    #endregion



    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(HandleButtonClick);

        slotBackgroundImage = gameObject.GetComponent<Image>();
        defaultColor = slotBackgroundImage.color;
    }

    private void HandleButtonClick()
    {
        if (isFilled)
        {
            if (isBackpackSlot)
            {
                EquipItemInSlot();
            }
            else
            {
                playerInventory.AddItemToBackpack(itemInSlot);
                RemoveItemFromSlot();
            }
        }
    }

    private void EquipItemInSlot()
    {
        playerInventory.EquipItem(itemInSlot);
        RemoveItemFromSlot();
    }

    public void RemoveItemFromSlot()
    {
        isFilled = false;
        itemImage.enabled = false;
        
        // hide additional window with item info
        itemInfoPanel.DisplayItemInfo(itemInSlot, false);
    }

    /// <summary>
    /// Adds item player to slot from the ground or from currently equipped items.
    /// <para>Hides item info window if the item was unequipped </para>
    /// </summary>
    /// <param name="itemToAdd"></param>
    /// <param name="isItemUnequipped">true if item was removed from currently equipped items</param>
    public void AddItemInSlot(Item itemToAdd, bool isItemUnequipped = false)
    {
        isFilled = true;
        itemInSlot = itemToAdd;
        ShowItemImage();

        if (isItemUnequipped)
        {
            itemInfoPanel.DisplayItemInfo(itemInSlot, false);
        }
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
        slotBackgroundImage.color = highlightColor;
    }

    private void OnMouseExit()
    {
        if (isFilled)
            itemInfoPanel.DisplayItemInfo(itemInSlot,false);
        slotBackgroundImage.color = defaultColor;
    }
}
