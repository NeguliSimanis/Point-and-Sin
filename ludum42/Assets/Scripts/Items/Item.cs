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
    public bool isVictoryItem = false; // pick up this item to win the game
    public bool isUniqueItem = false;
    public string itemName;
    public int wrath = 0;
    public int pride = 0;
    public int lust = 0;
    public string effectDescription; // how big is the wrath, pride, lust bonus
    public string flavorText;
    public SpriteRenderer itemImage;
    #endregion

    #region AUDIO
    AudioSource audioSource;
    public AudioClip itemInteractSFX;
    float sfxVolume = 0.6f;
    #endregion

    private void Start()
    {
        itemID = largestItemID;
        largestItemID++;

        playerInventory = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerInventory>();
        GenerateItemProperties();

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlayPickUpSFX()
    {
        audioSource.PlayOneShot(itemInteractSFX, sfxVolume);
    }

    public void GenerateItemProperties()
    {
        // get image
        itemImage = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        // intialize Item generator
        if (ItemGenerator.current == null)
        {
            ItemGenerator.current = new ItemGenerator();
        }

        if (!isUniqueItem)
        {
            // get name
            itemName = ItemGenerator.current.GetItemName(itemID, itemType);

            // get stats
            ItemGenerator.current.SetItemStats(this);
        }
        else
        {
            // Unique item properties are set in UniqueItemGenerator
            ItemGenerator.current.SetSpecialItemProperties(this);
        }
    }   


    public void AttemptPickUp()
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
        playerInventory.AddItemToBackpack(this);
        DisableChildrenAndColliders();

        if (isVictoryItem)
        {
            PlayerData.current.isBrutalUnlocked = true;
            playerInventory.gameObject.GetComponent<PlayerController>().WinGame();
        }
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

    public string GetItemTypeString()
    {
        if (itemType == ItemType.Eye)
        {
            return "Eyes";
        }
        else if (itemType == ItemType.Heart)
        {
            return "Heart";
        }
        else if (itemType == ItemType.Hand)
        {
            return "Hand";
        }
        return "BOI";
    }

    /// <summary>
    /// Changes player stats in PlayerData when this item is equipped or unequipped.
    /// </summary>
    /// <param name="multiplier">multiplier at which the change is applied. Change to -1 when unequipping an item</param>
    public void AddStatBoost(int multiplier = 1)
    {
        if (wrath > 0)
            PlayerData.current.AddWrath(wrath * multiplier);
        if (pride > 0)
            PlayerData.current.AddPride(pride * multiplier, true);
        if (lust > 0)
            PlayerData.current.AddLust(lust * multiplier);
    }
}
