using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueItemProperties
{
    public static UniqueItemProperties current;

    public UniqueItem[] uniqueItems =
    {
        // Blood of the Virgin - dropchance = 2x (16/12/2018) 
        new UniqueItem("Blood of the Virgin", ItemType.Heart, 2f, 6, 6, 6, "Innocence trapped in glass"),  

        // Skull of the Reaper - dropchance = 1x (16/12/2018) 
        new UniqueItem("Skull of the Reaper", ItemType.Eye, 1f, 13, 6, 0, "In this world,\nonly two things are inevitable" ),
    };
    /*int itemID;
    string name;
    ItemType type;
    float dropChance;

    int wrath;
    int pride;
    int lust;*/



    /*public float uniqueItemDropRate = 0.06f;    // chance to drop unique item in brutal mode
    public float[] uniqueItemDropRates =        // the likelihood to drop a specific unique item
    {
        0f, // chance to drop item 0 - Blood of the Virgin ( 2 )
        4f,  // chance to drop item 1 - Skull of the Reaper ( 1 )
    }; */

}
