using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueItem
{
    public int itemID;
    public string name;
    public string flavorText;
    public ItemType type;
    public float dropChance;

    public int wrath;
    public int pride;
    public int lust;

    public UniqueItem(string itemName, ItemType itemType, float itemDropChance, int itemWrath, int itemPride, int itemLust, string itemFlavor)
    {
        name = itemName;
        type = itemType;
        dropChance = itemDropChance;
        wrath = itemWrath;
        pride = itemPride;
        lust = itemLust;
        flavorText = itemFlavor;
    }
}
