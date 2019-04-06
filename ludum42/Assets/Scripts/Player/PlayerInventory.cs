using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{

    //List<Item> playerInventory = new List<Item>(); // all items that player has picked up, but are not equipped

    #region BACKPACK - NOT EQUIPPED
    GameObject[] inventorySlots;
    [SerializeField]
    GameObject inventorySlotContainer;
    int inventorySize = 16;
    #endregion

    #region EQUIPPED ITEMS
    [SerializeField]
    InventorySlot equippedEyeSlot;
    [SerializeField]
    InventorySlot equippedLeftHandSlot;
    [SerializeField]
    InventorySlot equippedRightHandSlot;
    [SerializeField]
    InventorySlot equippedHeartSlot;
    #endregion

    public void EquipItem(Item itemToEquip)
    {
        // EQUIP EYE
        if (itemToEquip.itemType == ItemType.Eye)
        {
            if (equippedEyeSlot.isFilled)
                EmptyEquippedSlot(equippedEyeSlot);
            equippedEyeSlot.AddItemInSlot(itemToEquip);
        }
        // EQUIP HEART
        else if (itemToEquip.itemType == ItemType.Heart)
        {
            if (equippedHeartSlot.isFilled)
                EmptyEquippedSlot(equippedHeartSlot);
            equippedHeartSlot.AddItemInSlot(itemToEquip);
        }
        // EQUIP HAND
        else if (itemToEquip.itemType == ItemType.Hand)
        {
            // equip hand in right slot if both slots are full
            if (equippedLeftHandSlot.isFilled && equippedRightHandSlot.isFilled)
            {
                EmptyEquippedSlot(equippedRightHandSlot);
                equippedRightHandSlot.AddItemInSlot(itemToEquip);
            }
            // equip in rights slot if it's empty
            else if (!equippedRightHandSlot.isFilled)
            {
                equippedRightHandSlot.AddItemInSlot(itemToEquip);
            }
            // equip hand in left slot if right slot is full
            else if (!equippedLeftHandSlot.isFilled)
            {
                equippedLeftHandSlot.AddItemInSlot(itemToEquip);
            }

        }
    }

    private void EmptyEquippedSlot(InventorySlot slotToEmpty)
    {
        AddItemToBackpack(slotToEmpty.itemInSlot);
        slotToEmpty.RemoveItemFromSlot();
        slotToEmpty.isFilled = false;

        /*if (slotToEmpty == equippedEyeSlot)
        {
            Debug.Log("eeeee");
            equippedEyeSlot.itemInSlot = null;
        }
        else if (slotToEmpty == equippedHeartSlot)
        {
            Debug.Log("eeeee");
            equippedHeartSlot.itemInSlot = null;
        }
        else if (slotToEmpty == equippedLeftHandSlot)
        {
            Debug.Log("eeeee");
            equippedLeftHandSlot.itemInSlot = null;
        }
        else if (slotToEmpty == equippedRightHandSlot)
        {
            Debug.Log("eeeee");
            equippedRightHandSlot.itemInSlot = null;
        }*/
    }

    private void Start()
    {
        InitializeInventorySlots();
    }

    private void InitializeInventorySlots()
    {
        inventorySlots = new GameObject[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots[i] = inventorySlotContainer.transform.GetChild(i).transform.gameObject;
            inventorySlots[i].GetComponent<InventorySlot>().playerInventory = this;
            //Debug.Log(inventorySlots[i].name);
        }

    }

    private int FindNextFreeSlot()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (!inventorySlots[i].GetComponent<InventorySlot>().isFilled)
                return i;
        }
        return 0;
    }


    public void AddItemToBackpack(Item itemToAdd)
    {
        //playerInventory.Add(itemToAdd);
        inventorySlots[FindNextFreeSlot()].GetComponent<InventorySlot>().AddItemInSlot(itemToAdd);
    }

    public int CountEquippedItemsOfType(ItemType type)
    {
        int itemCount = 0;

        // go through all inventory slots
        if (equippedEyeSlot.itemInSlot != null && equippedEyeSlot.itemInSlot.itemType == type)
        {
            itemCount++;
        }
        if (equippedHeartSlot.itemInSlot != null && equippedHeartSlot.itemInSlot.itemType == type)
        {
            itemCount++;
        }
        if (equippedLeftHandSlot.itemInSlot != null && equippedLeftHandSlot.itemInSlot.itemType == type)
        {
            itemCount++;
        }
        if (equippedRightHandSlot.itemInSlot != null && equippedRightHandSlot.itemInSlot.itemType == type)
        {
            itemCount++;
        }
        Debug.Log("equipped " + itemCount + " items of this type. Time: " + Time.time);
        return itemCount;
    }

    public Item GetEquippedItemOfType(ItemType type, bool returnFirstResult = true)
    {
        if (equippedEyeSlot.itemInSlot != null && equippedEyeSlot.itemInSlot.itemType == type)
        {
            return equippedEyeSlot.itemInSlot;
        }
        if (equippedHeartSlot.itemInSlot != null && equippedHeartSlot.itemInSlot.itemType == type)
        {
            return equippedHeartSlot.itemInSlot;
        }
        if (equippedLeftHandSlot.itemInSlot != null && equippedLeftHandSlot.itemInSlot.itemType == type)
        {
            if (returnFirstResult)
                return equippedLeftHandSlot.itemInSlot;
        }
        if (equippedRightHandSlot.itemInSlot != null && equippedRightHandSlot.itemInSlot.itemType == type)
        {
            return equippedRightHandSlot.itemInSlot;
        }
        return null;
    }
}
