using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemState
{
    OnGround,
    InBackpack,
    Equipped
}

public class Item : MonoBehaviour
{
    PlayerInventory playerInventory;
    string playerTag = "Player";

    public ItemType itemType;
    public ItemState currentState;
    public bool canBePickedUp = false;

    #region ITEM PROPERTIES
    static int largestItemID = 0; // used only when generating item ID
    int itemID;
    public string itemName;
    int wrath = 0; 
    int pride = 0;
    int lust = 0; 

    public SpriteRenderer itemImage;
    #endregion



    private void Start()
    {
        itemID = largestItemID;
        largestItemID++;

        playerInventory = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerInventory>();
        GenerateItemProperties();
    }

    private void GenerateItemProperties()
    {
        // get image
        itemImage = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        // intialize Item generator
        if (ItemGenerator.current == null)
        {
            ItemGenerator.current = new ItemGenerator();
        }

        // get name
        itemName = ItemGenerator.current.GetItemName(itemID, itemType);
        Debug.Log(itemName);

        // get stats
    }


    private void OnMouseDown()
    {
        if (currentState == ItemState.OnGround && canBePickedUp)
        {
            AddToBackpack();
        }
    }

    private void AddToBackpack()
    {
        //Debug.Log("added to packpack");
        canBePickedUp = false;
        currentState = ItemState.InBackpack;
        playerInventory.AddItemToInventory(this);
        DisableChildrenAndColliders();
    }

    /// <summary>
    /// Hides children game objects (item image, etc) when it is equipped
    /// </summary>
    /// <param name="hide"></param>
    private void DisableChildrenAndColliders(bool hide = true)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        if (gameObject.GetComponent<CircleCollider2D>() != null)
        {
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
        }
    }
}
