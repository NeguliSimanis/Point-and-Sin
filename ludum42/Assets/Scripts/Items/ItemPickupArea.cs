using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupArea : MonoBehaviour
{
    string playerTag = "Player";
    [SerializeField]
    Item item;
    [SerializeField]
    bool attemptPickUpInThisScript = false; // set to true in editor if object is highlighted on mouse over

    private void OnMouseDown()
    {
        if (attemptPickUpInThisScript)
        {
            Debug.Log("attempted");
            item.AttemptPickUp();
            item.PlayPickUpSFX();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == playerTag)
        {
            item.canBePickedUp = true;
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
