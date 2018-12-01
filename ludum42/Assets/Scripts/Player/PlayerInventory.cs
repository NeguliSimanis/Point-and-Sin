using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    List<Item> playerInventory = new List<Item>(); // all items that player has picked up, but are not equipped

    GameObject[] inventorySlots;
    [SerializeField]
    GameObject inventorySlotContainer;
    int inventorySize = 1;
    
    //= new GameObject[];

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
        return 0;
    }

    public void AddItemToInventory(Item itemToAdd)
    {
        playerInventory.Add(itemToAdd);
        inventorySlots[FindNextFreeSlot()].GetComponent<InventorySlot>().AddItemInSlot(itemToAdd);
    }
}
