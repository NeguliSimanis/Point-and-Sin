﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemDropper : MonoBehaviour
{
    [SerializeField]
    GameObject specialItem;

    [SerializeField]
    GameObject victoryItem;

    [SerializeField]
    GameObject eye;

    [SerializeField]
    GameObject hand;

    [SerializeField]
    GameObject heart;

    public void DropVictoryItem()
    {
        GameObject drop = Instantiate(victoryItem);
        drop.GetComponent<Item>().isVictoryItem = true;
        drop.transform.position = gameObject.transform.position;
    }

    /// <summary>
    /// Roll chance to drop a special item (available only in brutal mode when killing minibosses)
    /// If unlucky, roll a chance for regular item
    /// </summary>
    public void RollChanceForSpecialItem()
    {
        /*
        if (Random.Range(0f,1f) <= PlayerData.current.uniqueItemDropRate)
        {*/
        if (true)
        {             
            GameObject drop = Instantiate(specialItem);
            drop.transform.position = gameObject.transform.position;
        }
        else
        {
            DropItem();
        }
    }


    public void DropItem()
    {
        // check chance that item will actually drop
        if (Random.Range(0f, 1f) > PlayerData.current.itemDropRate)
        {
            //Debug.Log("item not dropped");
            return;
        }

        GameObject itemToDrop = heart;

        // 
        if (Random.Range(0, PlayerData.current.armDropRate + PlayerData.current.eyeDropRate + PlayerData.current.heartDropRate) >
            PlayerData.current.armDropRate + PlayerData.current.eyeDropRate)
        {
            //Debug.Log("Dropping heart");
        }
        //
        else if (Random.Range(0, PlayerData.current.armDropRate + PlayerData.current.eyeDropRate + PlayerData.current.heartDropRate) >
            PlayerData.current.armDropRate)
        {
            //Debug.Log("Dropping eye");
            itemToDrop = eye;
        }
        //
        else
        {
           // Debug.Log("Dropping arm");
            itemToDrop = hand;
        }
        
        GameObject drop = Instantiate(itemToDrop);
        
        drop.transform.position = gameObject.transform.position;
    }

}