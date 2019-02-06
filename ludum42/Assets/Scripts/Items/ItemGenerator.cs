using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator
{
    public static ItemGenerator current;

    int lastItemID;
    static int attributeBonusForNextItem = 0;

    float additionalPrefixChance = 0.04f;
    float additionalSuffixChance = 0.03f;

    #region special items
    // Stored in uniqueItemProperties and generated in UniqueItemGenerator
    /*string [] specialItemNames =
    {
        "Blood of the Virgin",
        "Skull of the Reaper"
    };
    int specialItemWrath = 6;
    int specialItemPride = 6;
    int specialItemLust = 6;*/
    #endregion

    #region NAME GENERATION
    #region ARM NAMES
    string[] armNames =
    {
        "Arm",
        "Strongarm",
        "Hand",
        "Claw",
        "Prong",
        "Fingers",
        "Fist",
        "Stump",
        "Paw",
        "Nails",
        "Fingernails",
        "Clasp"
    };  

    #endregion

    #region EYE NAMES
    string[] eyeNames =
    {
        "Eyes",
        "Eyeball",
        "Pupils",
        "Retina",
        "Eye",
        "Lens",
        "Sclera",
        "Gaze"
    };
    #endregion

    #region HEART NAMES
    string[] heartNames =
    {
        "Soul",
        "Heart",
        "Desire",
        "Atrium",
        "Chest"
    };
    #endregion

    #region PREFIXES
    string[] prefixes =
    {
        "Poet’s",
        "Mad",
        "Flaming",
        "Dark",
        "Cursed",
        "Possesed",
        "Bloody",
        "Sacrificial",
        "Virgin",
        "Hallowed",
        "Pale",
        "Eneel’s",
        "Madman’s",
        "Alien",
        "Depraved",
        "Dog's",
        "Putrid",
        "Pulsating"
    };
    #endregion

    #region SUFFIXES
    string[] suffixes =
    {
        "of Desire",
        "of Wrath",
        "of Jealousy",
        "of the Poet",
        "of Insanity",
        "of Envy",
        "of Madness",
        "of the Lunatic",
        "of the Depths",
        "of Flame",
        "of Greed",
        "of Lust",
        "of Sloth",
        "of the Demon",
        "of the Undead",
        "of Sacrifice",
        "of Blood",
        "of the Virgin",
        "of Sorrow",
        "of Depravity",
        "of Vengeance",
        "of Love",
        "of Titans",
        "of Passion",
        "of Rage",
        "of the Spider",
        "of Torment"
    };
    #endregion
    #endregion
    public string GetItemName(int itemID, ItemType itemType)
    {
        attributeBonusForNextItem = 1;
        bool hasPrefix = false;
        bool hasSuffix = false;

        // guaranteed prefix
        if (GetRandomBool())
        {
            hasPrefix = true;
        }
        // guaranteed suffix
        else
        {
            hasSuffix = true;
        }

        // roll chance for additional prefix
        if (hasPrefix == false && Random.Range(0f, 1f) < additionalPrefixChance)
        {
            hasPrefix = true;
            attributeBonusForNextItem++;
        }
        // roll chance for additional suffix
        else if (hasSuffix == false && Random.Range(0f, 1f) < additionalSuffixChance)
        {
            hasSuffix = true;
            attributeBonusForNextItem++;
        }

        // get full name
        if (hasPrefix && hasSuffix)
        {
            if (itemType == ItemType.Heart)
            {
                // prefix
                return prefixes[Random.Range(0, prefixes.Length-1)] +
                // organ name
                    " " + heartNames[Random.Range(0, heartNames.Length-1)] +
                // suffix
                    " " + suffixes[Random.Range(0, suffixes.Length-1)];
            }
            else if (itemType == ItemType.Hand)
            {
                // prefix
                return prefixes[Random.Range(0, prefixes.Length - 1)] +
                // organ name
                    " " + armNames[Random.Range(0, armNames.Length - 1)] +
                // suffix
                    " " + suffixes[Random.Range(0, suffixes.Length - 1)];

            }
            else if (itemType == ItemType.Eye)
            {
                // prefix
                return prefixes[Random.Range(0, prefixes.Length - 1)] +
                // organ name
                    " " + eyeNames[Random.Range(0, eyeNames.Length - 1)] +
                // suffix
                    " " + suffixes[Random.Range(0, suffixes.Length - 1)];

            }
        }
        else if (hasPrefix)
        {
            if (itemType == ItemType.Heart)
            {
                // prefix
                return prefixes[Random.Range(0, prefixes.Length - 1)] +
                // organ name
                    " " + heartNames[Random.Range(0, heartNames.Length - 1)];
            }
            else if (itemType == ItemType.Hand)
            {

                // prefix
                return prefixes[Random.Range(0, prefixes.Length - 1)] +
                // organ name
                    " " + armNames[Random.Range(0, armNames.Length - 1)];
            }
            else if (itemType == ItemType.Eye)
            {
                // prefix
                return prefixes[Random.Range(0, prefixes.Length - 1)] +
                    // organ name
                    " " + eyeNames[Random.Range(0, eyeNames.Length - 1)];
            }
        }
        // has only suffix
        else
        {
            if (itemType == ItemType.Heart)
            {
                // organ name
                return heartNames[Random.Range(0, heartNames.Length - 1)] +
                // suffix
                    " " + suffixes[Random.Range(0, suffixes.Length - 1)];
            }
            else if (itemType == ItemType.Hand)
            {
                // organ name
                return armNames[Random.Range(0, armNames.Length - 1)] +
                // suffix
                    " " + suffixes[Random.Range(0, suffixes.Length - 1)];
            }
            else if (itemType == ItemType.Eye)
            {
                // organ name
                return eyeNames[Random.Range(0, eyeNames.Length - 1)] +
                // suffix
                    " " + suffixes[Random.Range(0, suffixes.Length - 1)];
            }
        }
        return "BOI";
    }

    public void SetItemStats(Item itemToBoost)
    {
        for (int i = attributeBonusForNextItem*2 + Random.Range(0, 3); i > 0; i--)
        {
            // roll chance for each stat to be increased
            int statRoll = Random.Range(0, 3);

            // increase WRATH
            if (statRoll == 0)
            {
                itemToBoost.wrath++;
            }
            // increase PRIDE
            else if (statRoll == 1)
            {
                itemToBoost.pride++;
            }
            // increase LUST
            else if (statRoll == 2)
            {
                itemToBoost.lust++;
            }
        }
        SetItemEffectDescription(itemToBoost);
    }

    private void SetItemEffectDescription(Item itemToDescribe)
    {
        string itemDescription = "";
        bool hasWrath = false;
        bool hasPride = false;

        if (itemToDescribe.wrath > 0)
        {
            itemDescription = "+ " + itemToDescribe.wrath + " wrath";
            hasWrath = true;
        }
        if (itemToDescribe.pride > 0)
        {
            if (hasWrath)
                itemDescription = itemDescription + '\n' + "+ " + itemToDescribe.pride + " pride";
            else
                itemDescription = "+ " + itemToDescribe.pride + " pride";
            hasPride = true;
        }
        if (itemToDescribe.lust > 0)
        {
            if (hasWrath || hasPride)
                itemDescription = itemDescription + '\n' + "+ " + itemToDescribe.lust + " lust";
            else
                itemDescription = "+ " + itemToDescribe.lust + " lust";
        }
        itemToDescribe.effectDescription = itemDescription;

    }

    public void SetUniqueItemProperties(Item uniqueItem)
    {
        SetUniqueItemStats(uniqueItem);
        SetItemEffectDescription(uniqueItem);
    }

    private void SetUniqueItemStats(Item uniqueItem)
    {
        uniqueItem.wrath = UniqueItemProperties.current.uniqueItems[uniqueItem.uniqueItemID].wrath;
        uniqueItem.pride = UniqueItemProperties.current.uniqueItems[uniqueItem.uniqueItemID].pride;
        uniqueItem.lust = UniqueItemProperties.current.uniqueItems[uniqueItem.uniqueItemID].lust;
    }

    private bool GetRandomBool()
    {
        if (Random.Range(0f,1f) > 0.49f)
        {
            return true;
        }
        return false;
    }
}
