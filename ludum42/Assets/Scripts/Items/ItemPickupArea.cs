using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupArea : MonoBehaviour
{
    string playerTag = "Player";
    [SerializeField]
    Item item;

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
