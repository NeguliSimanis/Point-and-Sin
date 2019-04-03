using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    [SerializeField]
    CharacterPanel characterPanel;
    [SerializeField]
    GameObject itemFlairTextPanel;

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
        itemInSlot.PlayPickUpSFX();

        // adds item to equipped slot
        playerInventory.EquipItem(itemInSlot);

        // applies the effect of item
        itemInSlot.AddStatBoost();
        characterPanel.UpdateSinPointsText();

        // removes the item from backpack slot
        RemoveItemFromSlot();
    }

    public void RemoveItemFromSlot()
    {
        itemInSlot.PlayPickUpSFX();

        isFilled = false;
        itemImage.enabled = false;
        
        // hide additional window with item info
        itemInfoPanel.DisplayItemInfo(itemInSlot, false);

        // hide flair text panel
        itemFlairTextPanel.gameObject.SetActive(false);

        // removes the positive effect of the item when it is unequipped
        if (!isBackpackSlot)
        {
            itemInSlot.AddStatBoost(-1);
            characterPanel.UpdateSinPointsText();
            itemInSlot.currentState = ItemState.InBackpack;
        }
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

        if (!isBackpackSlot)
            itemInSlot.currentState = ItemState.Equipped;

        if (isItemUnequipped)
        {
            itemInfoPanel.DisplayItemInfo(itemInSlot, false);
            itemFlairTextPanel.gameObject.SetActive(false);
            
        }
    }

    private void ShowItemImage()
    {
        itemImage.enabled = true;
        itemImage.sprite = itemInSlot.itemImage.sprite;
    }

    /// <summary>
    /// This method is used instead of onMouseEnter, because it cannot be used for UI elements
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if (isFilled)
        {
            Debug.Log("mouse over fille " + Time.time);
            itemInfoPanel.DisplayItemInfo(itemInSlot);
            if (itemInSlot.isUniqueItem)
            {
                itemFlairTextPanel.SetActive(true);
                itemFlairTextPanel.GetComponent<SetUIText>().SetItemFlavorText(itemInSlot.flavorText);
            }
        }
        slotBackgroundImage.color = highlightColor;
    }

    /// <summary>
    /// This method is used instead of onMouseExit, because it cannot be used for UI elements
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isFilled)
        {
            itemInfoPanel.DisplayItemInfo(itemInSlot, false);
            itemFlairTextPanel.SetActive(false);
        }
        slotBackgroundImage.color = defaultColor;
    }
}
