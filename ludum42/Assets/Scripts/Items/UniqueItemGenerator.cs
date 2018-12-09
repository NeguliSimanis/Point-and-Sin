using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueItemGenerator : MonoBehaviour
{
    int currentItemID;

    [SerializeField]
    Sprite[] itemSprites;

    [SerializeField]
    Sprite[] itemHighlightedSprites;

    [SerializeField]
    AudioClip[] itemSFXs;

	void Start ()
    {
        RollUniqueItem();
        SetUniqueItemProperties();
	}

    /// <summary>
    /// We know that this is a unique item.
    /// 
    /// Determine which unique item it is
    /// </summary>
    void RollUniqueItem()
    {
        if (UniqueItemProperties.current == null)
            UniqueItemProperties.current = new UniqueItemProperties();

        // GET TOTAL NUMBER OF UNIQUE ITEMS
        int numberOfUniques = UniqueItemProperties.current.uniqueItems.Length - 1;
        ///Debug.Log("there are " + numberOfUniques + " uniques in the game");

        // GET TOTAL CHANCE OF DROPPING UNIQUE ITEMS
        float dropChanceSum = 0;
        for  (int i = 0; i <= numberOfUniques; i++)
        {
            dropChanceSum += UniqueItemProperties.current.uniqueItems[i].dropChance;    
        }
        ///Debug.Log("The sum of dropchances is " + dropChanceSum);

        // ROLL RANDOM AND CHECK
        bool uniqueDropFound = false;
        float randomUniqueRoll = Random.Range(0, dropChanceSum);
        float currentItemDoesNotDropChance = dropChanceSum;
        ///Debug.Log("random roll is " + randomUniqueRoll);

        // go through every unique in the game
        for (int i = numberOfUniques; i >= 0; i--)
        {
            if (!uniqueDropFound)
            {
                ///Debug.Log("Checking drop chance " + currentItemDoesNotDropChance);
                if (randomUniqueRoll >= currentItemDoesNotDropChance - UniqueItemProperties.current.uniqueItems[i].dropChance)
                {
                    uniqueDropFound = true;
                    currentItemID = i;
                   /// Debug.Log("Found at " + currentItemDoesNotDropChance);
                }
                else
                {
                    currentItemDoesNotDropChance -= UniqueItemProperties.current.uniqueItems[i].dropChance;
                }
            }
            if (uniqueDropFound)
            {
                break;
            }
        }
        // currentItemID = (Random.Range(0, PlayerData.current.uniqueItemDropRates.Length - 1));
    }
	
    void SetUniqueItemProperties()
    {
        #region GAME OBJECT PROPERTIES
        GameObject itemChildObject = gameObject.transform.GetChild(0).gameObject;

        // set sprite
        itemChildObject.GetComponent<SpriteRenderer>().sprite =
            itemSprites[currentItemID];

        // set highlighted sprite
        ChangeSpriteOnHover changeSpriteOnHover = itemChildObject.GetComponent<ChangeSpriteOnHover>();
        changeSpriteOnHover.spriteOnMouseOver = itemHighlightedSprites[currentItemID];
        changeSpriteOnHover.defaultSprite = itemSprites[currentItemID];
        #endregion

        #region ITEM PROPERTIES
        Item currentItem = gameObject.GetComponent<Item>();
        currentItem.isUniqueItem = true;

        // set sprite
        currentItem.itemImage.sprite = itemSprites[currentItemID];

        // set sound 
        currentItem.itemInteractSFX = itemSFXs[currentItemID];

        // set name
        currentItem.itemName = UniqueItemProperties.current.uniqueItems[currentItemID].name;

        // set effect
        currentItem.wrath = UniqueItemProperties.current.uniqueItems[currentItemID].wrath;
        currentItem.pride = UniqueItemProperties.current.uniqueItems[currentItemID].pride;
        currentItem.lust = UniqueItemProperties.current.uniqueItems[currentItemID].lust;

        // set type
        currentItem.itemType = UniqueItemProperties.current.uniqueItems[currentItemID].type;

        // set flavor text
        currentItem.flavorText = UniqueItemProperties.current.uniqueItems[currentItemID].flavorText;
        #endregion
    }

}
