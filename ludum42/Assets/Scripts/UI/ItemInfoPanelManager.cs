using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calls methods from ItemInfoPanels in child objects
/// </summary>
public class ItemInfoPanelManager : MonoBehaviour
{
    [SerializeField] GameObject oneItemInfoPanel;
    [SerializeField] GameObject twoItemInfoPanel;
    [SerializeField] GameObject threeItemInfoPanel;

    PlayerInventory playerInventory;
    int itemsToHighlightCount = 0;
    Item selectedItem;

    public void DisplayItemInfo(Item item, PlayerInventory inventory, bool display, bool isItemEquipped)
    {
        selectedItem = item;
        playerInventory = inventory;
        if (!display)
        {
            HideAllItemInfo(true);
        }
        else
        {
            //Debug.Log("Item to display: " + item.itemName + ". Time: " + Time.time);
            itemsToHighlightCount = playerInventory.CountEquippedItemsOfType(item.itemType) + 1;

            //Debug.Log(isItemEquipped);
            if (isItemEquipped)
            {
                itemsToHighlightCount = 1;
            }

            switch (itemsToHighlightCount)
            {
                case 1:
                    ShowOneItemInfo();
                    break;
                case 2:
                    ShowTwoItemInfo();
                    break;
                case 3:
                    ShowThreeItemInfo();
                    break;
                default:
                    Debug.Log("Item info display ERROR");
                    break;
            }
        }
    }
    private void HideAllItemInfo(bool hide)
    {
        oneItemInfoPanel.SetActive(!hide);
        twoItemInfoPanel.SetActive(!hide);
        threeItemInfoPanel.SetActive(!hide);
    }

    private void HideAllButThisItemInfo(bool hide, int hideCount = 3)
    {
        switch (hideCount)
        {
            // show 1st panel
            case 1:
                oneItemInfoPanel.SetActive(!hide);
                twoItemInfoPanel.SetActive(hide);
                threeItemInfoPanel.SetActive(hide);

                break;
            // show 2nd panel
            case 2:
                oneItemInfoPanel.SetActive(hide);
                twoItemInfoPanel.SetActive(!hide);
                threeItemInfoPanel.SetActive(hide);
                break;
            // show 3d panel
            case 3:
                oneItemInfoPanel.SetActive(hide);
                twoItemInfoPanel.SetActive(hide);
                threeItemInfoPanel.SetActive(!hide);
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
    }

    private void ShowOneItemInfo()
    {
        HideAllButThisItemInfo(false, 1);
        GameObject activeItemInfoPanel = GetActiveItemInfoPanel(oneItemInfoPanel, 0); 
        activeItemInfoPanel.GetComponent<ItemInfoPanel>().DisplayItemInfo(selectedItem, true);
    }
    private void ShowTwoItemInfo()
    {
        HideAllButThisItemInfo(false, 2);

        // display selected item in inventory
        GameObject activeItemInfoPanel = GetActiveItemInfoPanel(twoItemInfoPanel, 0);
        activeItemInfoPanel.GetComponent<ItemInfoPanel>().DisplayItemInfo(selectedItem, true);

        // find the equipped item that can be swapped with the selected item
        Item otherItem = playerInventory.GetEquippedItemOfType(selectedItem.itemType);

        // display equipped item
        activeItemInfoPanel = GetActiveItemInfoPanel(twoItemInfoPanel, 1);
        activeItemInfoPanel.GetComponent<ItemInfoPanel>().DisplayItemInfo(otherItem, true);
    }
    private void ShowThreeItemInfo()
    {
        HideAllButThisItemInfo(false, 3);

        // display selected item in inventory
        GameObject activeItemInfoPanel = GetActiveItemInfoPanel(threeItemInfoPanel, 0);
        activeItemInfoPanel.GetComponent<ItemInfoPanel>().DisplayItemInfo(selectedItem, true);

        // find the equipped item 1 that can be swapped with the selected item
        Item otherItem = playerInventory.GetEquippedItemOfType(selectedItem.itemType);

        // display equipped item 1
        activeItemInfoPanel = GetActiveItemInfoPanel(threeItemInfoPanel, 1);
        activeItemInfoPanel.GetComponent<ItemInfoPanel>().DisplayItemInfo(otherItem, true);

        // find the equipped item 1 that can be swapped with the selected item
        Item otherItem2 = playerInventory.GetEquippedItemOfType(selectedItem.itemType, false);

        // display equipped item 1
        activeItemInfoPanel = GetActiveItemInfoPanel(threeItemInfoPanel, 2);
        activeItemInfoPanel.GetComponent<ItemInfoPanel>().DisplayItemInfo(otherItem2, true);
    }

    /// <summary>
    /// Find the child game object that has ItemInfoPanel component
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="firstChildID"></param>
    private GameObject GetActiveItemInfoPanel(GameObject parent, int firstChildID)
    {
        GameObject activeItemInfoPanel = parent.transform.GetChild(firstChildID).transform.GetChild(0).transform.GetChild(0).gameObject;
        return activeItemInfoPanel;
    }
}
