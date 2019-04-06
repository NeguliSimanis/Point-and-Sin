using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item becomes marked for pickup when the player clicks on it.
/// Stores the most recently clicked item in PlayerController.cs and when the player enters the item pickup area, it is picked up.
/// 
/// PROBLEM - what if player clicks on item, then elsewhere and then goes near the item pickup area?
/// In this instance the item shouldn't be picked up
/// 
/// 06.01.2019 - Sīmanis Mikoss
/// </summary>
public class ItemPickupArea : MonoBehaviour
{
    private string playerTag = "Player";
    private PlayerController playerController;

    [SerializeField]
    private Item item; // the item that has this pickup area

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerController>();
    }

    private void OnMouseDown()
    {
        //Debug.Log(item.name + " pickup area: " + Time.time);
        playerController.itemAwaitingPickup = item;
        playerController.isWalkingToPickUpItem = true;

        playerController.lastItemPickupCommandTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == playerTag)
        {
            ManageItemPickupArea();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == playerTag)
        {
            ManageItemPickupArea();
        }
    }

    private void ManageItemPickupArea()
    {
        /// STEP 1
        // player has entered the pickup area, so the item is near enough to be picked up
        item.canBePickedUp = true;
        /*if (!playerController.isWalkingToPickUpItem)
          Debug.Log("managing!");

        if (item != playerController.itemAwaitingPickup)
            Debug.Log("wjat managing!");*/
        /// STEP 2
        // player was walking to pickup an item and this is that item
        if (playerController.isWalkingToPickUpItem && item == playerController.itemAwaitingPickup)
        {
            /// STEP 3
            // the last clicked thing was this item
            if (Mathf.Approximately(playerController.lastItemPickupCommandTime, playerController.lastClickedTime))
            {
                // pick up item
                item.AttemptPickUp();
                playerController.isWalkingToPickUpItem = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == playerTag)
        {
            item.canBePickedUp = false;
        }
    }
}
