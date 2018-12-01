using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator
{
    public static ItemGenerator current;

    int lastItemID;
    int attributeBonusForNextItem = 0;

    float additionalPrefixChance = 0.12f;
    float additionalSuffixChance = 0.15f;

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
        "Paw"
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
        "Sclera"
    };
    #endregion

    #region HEART NAMES
    string[] heartNames =
    {
        "Soul",
        "Heart",
        "Desire",
        "Atrium"
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
        "Depraved"
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
        "of Rage"
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
        if (hasPrefix == false && Random.Range(0,1) < additionalPrefixChance)
        {
            hasPrefix = true;
            attributeBonusForNextItem++;
        }
        // roll chance for additional suffix
        else if (hasSuffix == false && Random.Range(0, 1) < additionalSuffixChance)
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

    private bool GetRandomBool()
    {
        if (Random.Range(0,1) > 0.49f)
        {
            return true;
        }
        return false;
    }
}
