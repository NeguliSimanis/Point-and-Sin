using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileBorderType
{
    Top,
    TopLeft,
    TopRight,
    Left,
    BottomLeft,
    Bottom,
    BottomRight,
    Right,
    None
}

public class TileBorder : MonoBehaviour
{
    [SerializeField]
    TileBorderType tileBorder;

    PlayerMovement playerMovement;

    /*private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("PlayerLeg").transform.parent.gameObject.GetComponent<PlayerMovement>();
    }

    private void GetPlayerMovementComponent(GameObject playerObject)
    {
        playerMovement = playerObject.transform.parent.gameObject.GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLeg"))
        {
            EnableMovementDirection(false, collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLeg"))
        {
            EnableMovementDirection(true, collision.gameObject);
        }
    }

    private void EnableMovementDirection(bool enable, GameObject playerObject)
    {
        if (playerMovement == null)
            GetPlayerMovementComponent(playerObject);
        //Debug.Log("enabling player movement in direction " + enable);
        playerMovement.EnableMovementDirection(enable, tileBorder);
    }*/
}
