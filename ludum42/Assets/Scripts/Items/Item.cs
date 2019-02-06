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
    public string itemName;
    public int wrath = 0;
    public int pride = 0;
    public int lust = 0;
    public string effectDescription; // how big is the wrath, pride, lust bonus
    public string flavorText;
    public SpriteRenderer itemImage;

    public bool isUniqueItem = false;
    public int uniqueItemID = -1;  
    private UniqueItemGenerator uniqueItemGenerator; // sets unique item properties
    #endregion

    #region AUDIO
    AudioSource audioSource;
    public AudioClip itemInteractSFX;
    float sfxVolume = 0.6f;
    #endregion

    private void Start()
    {
        GetComponents();
        DetermineIfUniqueItem();
        itemID = largestItemID;
        largestItemID++;
        GenerateItemProperties();
    }

    private void DetermineIfUniqueItem()
    {
        if (gameObject.GetComponent<UniqueItemGenerator>() != null)
        {
            isUniqueItem = true;
            uniqueItemGenerator = gameObject.GetComponent<UniqueItemGenerator>();
        }
    }

    /// <summary>
    /// Intitialize necessary variables with getcomponent method
    /// </summary>
    private void GetComponents()
    {
        playerInventory = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerInventory>();
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
            GenerateCommonItemProperties();
        }
        else
        {
            GenerateUniqueItemProperties();
        }
    }

    void GenerateCommonItemProperties()
    {
        // get name
        itemName = ItemGenerator.current.GetItemName(itemID, itemType);

        // get stats
        ItemGenerator.current.SetItemStats(this);
    }

    void GenerateUniqueItemProperties()
    {
        uniqueItemID = uniqueItemGenerator.RollUniqueItem();
        itemType = UniqueItemProperties.current.uniqueItems[uniqueItemID].type;
        flavorText = UniqueItemProperties.current.uniqueItems[uniqueItemID].flavorText;
        itemName = UniqueItemProperties.current.uniqueItems[uniqueItemID].name;
        SetUniqueItemSpriteAndSFX();

        // Unique item stats are set in UniqueItemGenerator
        ItemGenerator.current.SetUniqueItemProperties(this);
    }

    /// <summary>
    /// Set the sprite and other game object properties if the item is unique
    /// </summary>
    private void SetUniqueItemSpriteAndSFX()
    {
        GameObject itemChildObject = gameObject.transform.GetChild(0).gameObject;

        // set sprite
        itemChildObject.GetComponent<SpriteRenderer>().sprite = uniqueItemGenerator.GetUniqueSprite();

        // set highlighted sprite
        ChangeSpriteOnHover changeSpriteOnHover = itemChildObject.GetComponent<ChangeSpriteOnHover>();
        changeSpriteOnHover.spriteOnMouseOver = uniqueItemGenerator.GetUniqueHighlightedSprite();
        changeSpriteOnHover.defaultSprite = uniqueItemGenerator.GetUniqueSprite();

        // set item sprite
        itemImage.sprite = uniqueItemGenerator.GetUniqueSprite();

        // set audio
        itemInteractSFX = uniqueItemGenerator.GetItemSFX();
    }

    public void AttemptPickUp()
    {
        // item is on ground and player is standing near it
        if (currentState == ItemState.OnGround && canBePickedUp)
        {
            AddToBackpack();
            PlayPickUpSFX();
        }
        /*// item is on ground, but player is far away
        else if (currentState == ItemState.OnGround)
        {
            
        }*/
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
